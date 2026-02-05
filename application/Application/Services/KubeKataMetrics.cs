using System.Diagnostics.Metrics;

namespace KubeKataApp.Application.Services;

public class KubeKataMetrics
{
    public const string MeterName = "KubeKata.App";
    private readonly Counter<long> _adminsCreatedCounter;

    public KubeKataMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _adminsCreatedCounter = meter.CreateCounter<long>(
            "kubekata_admins_created_total",
            description: "Total number of admin accounts created");
    }

    public void RecordAdminCreated()
    {
        _adminsCreatedCounter.Add(1);
    }
}
