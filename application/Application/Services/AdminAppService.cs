using KubeKataApp.Application.DTOs;
using KubeKataApp.Application.Interfaces;
using KubeKataApp.Domain.Entities;
using KubeKataApp.Domain.Repositories;
using KubeKataApp.Domain.ValueObjects;

namespace KubeKataApp.Application.Services;

public class AdminAppService : IAdminAppService
{
    private readonly IAdminRepository _repository;
    private readonly KubeKataMetrics _metrics;

    public AdminAppService(IAdminRepository repository, KubeKataMetrics metrics)
    {
        _repository = repository;
        _metrics = metrics;
    }

    public async Task<IEnumerable<AdminAccountDto>> GetAllAdminsAsync()
    {
        var admins = await _repository.GetAllAsync();
        return admins.Select(MapToDto);
    }

    public async Task<AdminAccountDto?> GetAdminByIdAsync(Guid id)
    {
        var admin = await _repository.GetByIdAsync(id);
        return admin == null ? null : MapToDto(admin);
    }

    public async Task<AdminAccountDto> CreateAdminAsync(CreateAdminDto createDto)
    {
        var admin = new AdminAccount(
            Guid.NewGuid(),
            createDto.Username,
            new Email(createDto.Email),
            createDto.Password // In reality, hash this
        );

        await _repository.AddAsync(admin);
        _metrics.RecordAdminCreated();
        return MapToDto(admin);
    }

    public async Task<bool> UpdateAdminAsync(Guid id, UpdateAdminDto updateDto)
    {
        var admin = await _repository.GetByIdAsync(id);
        if (admin == null) return false;

        admin.UpdateUsername(updateDto.Username);
        admin.UpdateEmail(new Email(updateDto.Email));

        await _repository.UpdateAsync(admin);
        return true;
    }

    public async Task<bool> DeleteAdminAsync(Guid id)
    {
        var admin = await _repository.GetByIdAsync(id);
        if (admin == null) return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var admin = await _repository.GetByEmailAsync(email);
        if (admin == null) return false;

        // Domain logic for password reset would go here
        // e.g., admin.GenerateResetToken();
        
        return true;
    }

    private static AdminAccountDto MapToDto(AdminAccount admin)
    {
        return new AdminAccountDto(admin.Id, admin.Username, admin.Email.Value, admin.CreatedAt);
    }
}
