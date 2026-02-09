namespace KubeKataWorker;

public interface IMessageTracker
{
    Task<bool> IsProcessedAsync(Guid messageId);
    Task MarkAsProcessedAsync(Guid messageId, string username);
    Task EnsureSchemaAsync();
}
