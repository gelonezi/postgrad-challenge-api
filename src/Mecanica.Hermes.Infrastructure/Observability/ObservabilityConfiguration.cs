using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Mecanica.Hermes.Infrastructure.Observability;

public static class ObservabilityConfiguration
{
    public static void AddObservability(this WebApplicationBuilder builder)
    {
        if (!EnvironmentDetector.IsKubernetes)
            return;

        var serviceName = builder.Configuration["Observability:ServiceName"] ?? "mecanica-hermes-api";
        var serviceVersion = typeof(ObservabilityConfiguration).Assembly
            .GetName().Version?.ToString() ?? "unknown";

        builder.Services.AddOpenTelemetry()
            .UseOtlpExporter()                          // reads OTEL_EXPORTER_OTLP_* env vars
            .ConfigureResource(resource =>
            {
                resource
                    .AddService(serviceName, serviceVersion: serviceVersion)
                    .AddContainerDetector()             // injects container.id from cgroup
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["k8s.pod.name"]        = EnvironmentDetector.PodName,
                        ["k8s.namespace.name"]  = EnvironmentDetector.PodNamespace,
                        ["k8s.node.name"]       = EnvironmentDetector.NodeName,
                        ["k8s.cluster.name"]    = EnvironmentDetector.ClusterName,
                        ["k8s.deployment.name"] = "mecanica-hermes-api",
                        ["host.name"]           = Environment.MachineName,
                    });
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(ActivitySources.ApplicationName)
                    .AddNpgsql()
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                        opts.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health") &&
                                             !ctx.Request.Path.StartsWithSegments("/healthz");
                    })
                    .AddHttpClientInstrumentation();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(ActivitySources.ApplicationName)
                    .AddMeter(BusinessMetrics.MeterName)
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsqlInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            });
    }
}
