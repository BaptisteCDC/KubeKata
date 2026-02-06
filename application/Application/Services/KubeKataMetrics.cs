using System.Diagnostics.Metrics;

namespace KubeKataApp.Application.Services;

public class KubeKataMetrics
{
    public const string MeterName = "KubeKata.App";
    private readonly Counter<long> _adminsCreatedCounter;
    private readonly Counter<long> _httpRequestCounter;

    public KubeKataMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _adminsCreatedCounter = meter.CreateCounter<long>(
            "kubekata_admins_created_total",
            description: "Total number of admin accounts created");

        _httpRequestCounter = meter.CreateCounter<long>(
            "kubekata_http_requests_total",
            description: "Total number of HTTP requests with action name");
    }

    public void RecordAdminCreated()
    {
        _adminsCreatedCounter.Add(1);
    }

    public void RecordRequest(string action, string method, int statusCode)
    {
        _httpRequestCounter.Add(1, 
            new KeyValuePair<string, object?>("action", action),
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("status", statusCode));
    }
}
