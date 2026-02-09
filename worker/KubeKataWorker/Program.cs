using KubeKataWorker;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("KubeKataWorker"))
        .AddMeter("KubeKata.Worker")
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusHttpListener(options => options.UriPrefixes = new[] { "http://+:9464/" }));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddSingleton<IMessageTracker, PostgreSqlMessageTracker>();
}
else
{
    builder.Services.AddSingleton<IMessageTracker, InMemoryMessageTracker>();
}

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
