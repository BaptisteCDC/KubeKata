using KubeKataApp.Application.DTOs;
using KubeKataApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KubeKataApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminAppService _adminAppService;
    private readonly IDelayProvider _delayProvider;

    public AdminController(IAdminAppService adminAppService, IDelayProvider delayProvider)
    {
        _adminAppService = adminAppService;
        _delayProvider = delayProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var admins = await _adminAppService.GetAllAdminsAsync();
        return Ok(admins);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var admin = await _adminAppService.GetAdminByIdAsync(id);
        if (admin == null) return NotFound();
        return Ok(admin);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdminDto createDto)
    {
        var admin = await _adminAppService.CreateAdminAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = admin.Id }, admin);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAdminDto updateDto)
    {
        var success = await _adminAppService.UpdateAdminAsync(id, updateDto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _adminAppService.DeleteAdminAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _adminAppService.RequestPasswordResetAsync(request.Email);
        return Ok(new { Message = "If an account exists for this email, a reset link has been sent." });
    }

    [HttpPost("config/delay")]
    public IActionResult SetDelay([FromBody] DelayConfigRequest request)
    {
        _delayProvider.SetDelay(request.DelayMs);
        return Ok(new { Message = $"Simulated delay set to {request.DelayMs}ms" });
    }
}
