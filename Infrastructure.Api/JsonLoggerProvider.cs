namespace Infrastructure.Api;

public sealed class JsonLoggerProvider : ILoggerProvider
{
    private readonly LoggerExternalScopeProvider _scopeProvider = new();
    private readonly ConcurrentDictionary<string, JsonLogger> _loggers = new(StringComparer.Ordinal);

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, category => new JsonLogger(Console.Out, category, _scopeProvider));
    }

    public void Dispose()
    {
    }
}
