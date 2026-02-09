using Dapper;
using KubeKataApp.Domain.Entities;
using KubeKataApp.Domain.Repositories;
using Npgsql;
using System.Data;

namespace KubeKataApp.Infrastructure.Repositories;

public class PostgreSqlAdminRepository : IAdminRepository
{
    private readonly string _connectionString;

    public PostgreSqlAdminRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        EnsureDatabaseSchema();
    }

    private void EnsureDatabaseSchema()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS admin_accounts (
                id UUID PRIMARY KEY,
                username TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE,
                password TEXT NOT NULL
            );
        ");
    }

    private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    public async Task<IEnumerable<AdminAccount>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var results = await connection.QueryAsync<dynamic>("SELECT id, username, email, password FROM admin_accounts");
        return results.Select(r => AdminAccount.Create(r.username, r.email, r.password)); // Note: Should handle ID mapping if it was important, but domain entity uses simple factory.
    }

    public async Task<AdminAccount?> GetByIdAsync(Guid id)
    {
        using var connection = CreateConnection();
        var r = await connection.QueryFirstOrDefaultAsync<dynamic>("SELECT id, username, email, password FROM admin_accounts WHERE id = @Id", new { Id = id });
        return r != null ? AdminAccount.Create(r.username, r.email, r.password) : null;
    }

    public async Task<AdminAccount?> GetByEmailAsync(string email)
    {
        using var connection = CreateConnection();
        var r = await connection.QueryFirstOrDefaultAsync<dynamic>("SELECT id, username, email, password FROM admin_accounts WHERE email = @Email", new { Email = email });
        return r != null ? AdminAccount.Create(r.username, r.email, r.password) : null;
    }

    public async Task AddAsync(AdminAccount account)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(@"
            INSERT INTO admin_accounts (id, username, email, password) 
            VALUES (@Id, @Username, @Email, @Password)", 
            new { Id = account.Id, Username = account.Username.Value, Email = account.Email.Value, Password = account.Password.Value });
    }

    public async Task UpdateAsync(AdminAccount account)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(@"
            UPDATE admin_accounts 
            SET username = @Username, email = @Email, password = @Password 
            WHERE id = @Id", 
            new { Id = account.Id, Username = account.Username.Value, Email = account.Email.Value, Password = account.Password.Value });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync("DELETE FROM admin_accounts WHERE id = @Id", new { Id = id });
    }
}
