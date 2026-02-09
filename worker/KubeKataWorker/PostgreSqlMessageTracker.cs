using Dapper;
using Npgsql;

namespace KubeKataWorker;

public class PostgreSqlMessageTracker : IMessageTracker
{
    private readonly string _connectionString;
    private readonly ILogger<PostgreSqlMessageTracker> _logger;

    public PostgreSqlMessageTracker(IConfiguration configuration, ILogger<PostgreSqlMessageTracker> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    public async Task EnsureSchemaAsync()
    {
        try 
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS processed_messages (
                    message_id UUID PRIMARY KEY,
                    username TEXT NOT NULL,
                    processed_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                );
            ");
            _logger.LogInformation("Database schema for Worker verified/created.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure database schema for Worker.");
            throw;
        }
    }

    public async Task<bool> IsProcessedAsync(Guid messageId)
    {
        using var db = new NpgsqlConnection(_connectionString);
        return await db.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM processed_messages WHERE message_id = @Id)", 
            new { Id = messageId });
    }

    public async Task MarkAsProcessedAsync(Guid messageId, string username)
    {
        using var db = new NpgsqlConnection(_connectionString);
        await db.ExecuteAsync(
            "INSERT INTO processed_messages (message_id, username, processed_at) VALUES (@Id, @Username, @Now)", 
            new { Id = messageId, Username = username, Now = DateTime.UtcNow });
    }
}
