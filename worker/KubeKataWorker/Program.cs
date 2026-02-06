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

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
