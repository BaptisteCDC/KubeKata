using KubeKataApp.Application.DTOs;

namespace KubeKataApp.Application.Interfaces;

public interface IAdminAppService
{
    Task<IEnumerable<AdminAccountDto>> GetAllAdminsAsync();
    Task<AdminAccountDto?> GetAdminByIdAsync(Guid id);
    Task<AdminAccountDto> CreateAdminAsync(CreateAdminDto createDto);
    Task<bool> UpdateAdminAsync(Guid id, UpdateAdminDto updateDto);
    Task<bool> DeleteAdminAsync(Guid id);
    Task<bool> RequestPasswordResetAsync(string email);
}
