using Serilog.Context;

namespace Mecanica.Hermes.Infrastructure.Logging;

public sealed class LogContextBuilder : IDisposable
{
    private readonly List<IDisposable> _properties = new();

    public static LogContextBuilder Create() => new();

    public LogContextBuilder WithUserId(string userId)
        => Push("UserId", userId);

    public LogContextBuilder WithOperation(string operation)
        => Push("Operation", operation);

    public LogContextBuilder WithEntityId<T>(T id)
        => Push("EntityId", id?.ToString() ?? "unknown");

    public LogContextBuilder WithCorrelationId(string correlationId)
        => Push("CorrelationId", correlationId);

    public LogContextBuilder With(string key, object? value)
        => Push(key, value);

    private LogContextBuilder Push(string name, object? value)
    {
        _properties.Add(LogContext.PushProperty(name, value));
        return this;
    }

    public void Dispose()
    {
        foreach (var p in _properties)
            p.Dispose();
    }
}
