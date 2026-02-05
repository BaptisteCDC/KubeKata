namespace KubeKataApp.Application.DTOs;

public record AdminAccountDto(Guid Id, string Username, string Email, DateTime CreatedAt);

public record CreateAdminDto(string Username, string Email, string Password);

public record UpdateAdminDto(string Username, string Email);

public record ForgotPasswordRequest(string Email);

public record DelayConfigRequest(int DelayMs);
