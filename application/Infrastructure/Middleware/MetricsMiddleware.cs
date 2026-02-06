using KubeKataApp.Application.Services;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace KubeKataApp.Infrastructure.Middleware;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;

    public MetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, KubeKataMetrics metrics)
    {
        await _next(context);

        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var actionName = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ActionName;
            if (!string.IsNullOrEmpty(actionName))
            {
                metrics.RecordRequest(actionName, context.Request.Method, context.Response.StatusCode);
            }
        }
    }
}
