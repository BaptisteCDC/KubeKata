using KubeKataApp.Application.Interfaces;
using KubeKataApp.Application.Services;
using KubeKataApp.Domain.Repositories;
using KubeKataApp.Infrastructure.Middleware;
using KubeKataApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DDD Registrations
builder.Services.AddSingleton<IAdminRepository, InMemoryAdminRepository>();
builder.Services.AddScoped<IAdminAppService, AdminAppService>();
builder.Services.AddSingleton<IDelayProvider, DelayProvider>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Custom Middleware for Simulated Delay
app.UseMiddleware<SimulatedDelayMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
