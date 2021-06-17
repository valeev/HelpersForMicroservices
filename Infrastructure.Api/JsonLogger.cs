using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Api
{
    public sealed class JsonLogger : ILogger
    {
        private readonly TextWriter _writer;
        private readonly string _categoryName;

        internal IExternalScopeProvider ScopeProvider { get; set; }

        public JsonLogger(TextWriter writer, string categoryName, IExternalScopeProvider scopeProvider)
        {
            _writer = writer;
            _categoryName = categoryName;
            ScopeProvider = scopeProvider;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));

            var message = new JsonLogEntry
            {
                Timestamp = DateTimeOffset.UtcNow,
                LogLevel = logLevel.ToString(),
                Category = _categoryName,
                Exception = exception?.ToString(),
                Message = formatter(state, exception),
            };

            // Append the data of all BeginScope and LogXXX parameters to the message dictionary
            // REMOVED: AppendScope(message.Scope, state); - This line in original JsonLogger from Microsoft it's adding duplicated message details
            AppendScope(message.Scope);

            _writer.WriteLine(JsonConvert.SerializeObject(message, Formatting.None));
        }

        private void AppendScope(IDictionary<string, object> dictionary)
        {
            ScopeProvider.ForEachScope((scope, state) => AppendScope(state, scope), dictionary);
        }

        private static void AppendScope(IDictionary<string, object> dictionary, object scope)
        {
            if (scope == null)
                return;

            // The scope can be defined using BeginScope or LogXXX methods.
            // - logger.BeginScope(new { Author = "meziantou" })
            // - logger.LogInformation("Hello {Author}", "meziaantou")
            // Using LogXXX, an object of type FormattedLogValues is created. This type is internal but it implements IReadOnlyList, so we can use it.
            // https://github.com/aspnet/Extensions/blob/cc9a033c6a8a4470984a4cc8395e42b887c07c2e/src/Logging/Logging.Abstractions/src/FormattedLogValues.cs
            if (scope is IReadOnlyList<KeyValuePair<string, object>> formattedLogValues)
            {
                if (formattedLogValues.Count > 0)
                {
                    foreach (var value in formattedLogValues)
                    {
                        // MethodInfo is set by ASP.NET Core when reaching a controller. This type cannot be serialized using JSON.NET, but I don't need it.
                        if (value.Value is MethodInfo)
                            continue;

                        dictionary[value.Key] = value.Value;
                    }
                }
            }
            else
            {
                // The idea is to get the value of all properties of the object and add them to the dictionary.
                //      dictionary["Prop1"] = scope.Prop1;
                //      dictionary["Prop2"] = scope.Prop2;
                //      ...
                // We always log the same objects, so we can create a cache of compiled expressions to fill the dictionary.
                // Using reflection each time would slow down the logger.
                var appendToDictionaryMethod = ExpressionCache.GetOrCreateAppendToDictionaryMethod(scope.GetType());
                appendToDictionaryMethod(dictionary, scope);
            }
        }

        // In ASP.NET Core 3.0 this classes is now internal. This means you need to add it to your code.
        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            private NullScope() { }
            public void Dispose() { }
        }

        // Create and cache compiled expression to fill the dictionary from an object
        private static class ExpressionCache
        {
            public delegate void AppendToDictionary(IDictionary<string, object> dictionary, object o);

            private static readonly ConcurrentDictionary<Type, AppendToDictionary> s_typeCache = new ConcurrentDictionary<Type, AppendToDictionary>();
            private static readonly PropertyInfo _dictionaryIndexerProperty = GetDictionaryIndexer();

            public static AppendToDictionary GetOrCreateAppendToDictionaryMethod(Type type)
            {
                return s_typeCache.GetOrAdd(type, t => CreateAppendToDictionaryMethod(t));
            }

            private static AppendToDictionary CreateAppendToDictionaryMethod(Type type)
            {
                var dictionaryParameter = Expression.Parameter(typeof(IDictionary<string, object>), "dictionary");
                var objectParameter = Expression.Parameter(typeof(object), "o");

                var castedParameter = Expression.Convert(objectParameter, type); // cast o to the actual type

                // Create setter for each properties
                // dictionary["PropertyName"] = o.PropertyName;
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                var setters =
                    from prop in properties
                    where prop.CanRead
                    let indexerExpression = Expression.Property(dictionaryParameter, _dictionaryIndexerProperty, Expression.Constant(prop.Name))
                    let getExpression = Expression.Property(castedParameter, prop.GetMethod)
                    select Expression.Assign(indexerExpression, getExpression);

                var body = new List<Expression>(properties.Length + 1)
                {
                    castedParameter
                };
                body.AddRange(setters);

                var lambdaExpression = Expression.Lambda<AppendToDictionary>(Expression.Block(body), dictionaryParameter, objectParameter);
                return lambdaExpression.Compile();
            }

            // Get the PropertyInfo for IDictionary<string, object>.this[string key]
            private static PropertyInfo GetDictionaryIndexer()
            {
                var indexers = from prop in typeof(IDictionary<string, object>).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                               let indexParameters = prop.GetIndexParameters()
                               where indexParameters.Length == 1 && typeof(string).IsAssignableFrom(indexParameters[0].ParameterType)
                               select prop;

                return indexers.Single();
            }
        }
    }
}
