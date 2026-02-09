using System.Collections.Concurrent;

namespace KubeKataWorker;

public class InMemoryMessageTracker : IMessageTracker
{
    private readonly ConcurrentHashSet<Guid> _processedMessages = new();

    public Task<bool> IsProcessedAsync(Guid messageId)
    {
        return Task.FromResult(_processedMessages.Contains(messageId));
    }

    public Task MarkAsProcessedAsync(Guid messageId, string username)
    {
        _processedMessages.Add(messageId);
        return Task.CompletedTask;
    }

    public Task EnsureSchemaAsync() => Task.CompletedTask;
}

// Simple ConcurrentHashSet wrapper for POC
public class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dictionary = new();
    public bool Add(T item) => _dictionary.TryAdd(item, 0);
    public bool Contains(T item) => _dictionary.ContainsKey(item);
}
