using KubeKataApp.Domain.Entities;

namespace KubeKataApp.Domain.Repositories;

public interface IAdminRepository
{
    Task<IEnumerable<AdminAccount>> GetAllAsync();
    Task<AdminAccount?> GetByIdAsync(Guid id);
    Task<AdminAccount?> GetByEmailAsync(string email);
    Task AddAsync(AdminAccount account);
    Task UpdateAsync(AdminAccount account);
    Task DeleteAsync(Guid id);
}
