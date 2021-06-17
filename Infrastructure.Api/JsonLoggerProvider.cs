using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Infrastructure.Api
{
    public sealed class JsonLoggerProvider : ILoggerProvider
    {
        private readonly LoggerExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();
        private readonly ConcurrentDictionary<string, JsonLogger> _loggers = new ConcurrentDictionary<string, JsonLogger>(StringComparer.Ordinal);

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new JsonLogger(Console.Out, category, _scopeProvider));
        }

        public void Dispose()
        {
        }
    }
}
