using KubeKataApp.Application.Interfaces;

namespace KubeKataApp.Infrastructure.Middleware;

public class SimulatedDelayMiddleware
{
    private readonly RequestDelegate _next;

    public SimulatedDelayMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IDelayProvider delayProvider)
    {
        if (delayProvider.CurrentDelayMs > 0)
        {
            await Task.Delay(delayProvider.CurrentDelayMs);
        }

        await _next(context);
    }
}
