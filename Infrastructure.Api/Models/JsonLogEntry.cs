namespace Infrastructure.Api;

internal sealed class JsonLogEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public string LogLevel { get; set; }
    public string Category { get; set; }
    public string Exception { get; set; }
    public object Message { get; set; }
    public IDictionary<string, object> Scope { get; } = new Dictionary<string, object>(StringComparer.Ordinal);
}
