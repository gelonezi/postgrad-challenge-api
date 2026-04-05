using Mecanica.Hermes.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Mecanica.Hermes.Infrastructure.DependencyInjection;

public static class WebApplicationBuilderExtensions
{
    private static bool IsHealthCheckLog(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("RequestPath", out var rp) &&
            rp is ScalarValue { Value: string requestPath } &&
            requestPath.StartsWith("/health"))
            return true;

        if (logEvent.Properties.TryGetValue("Path", out var p) &&
            p is ScalarValue { Value: string path } &&
            path.StartsWith("/health"))
            return true;

        if (logEvent.Properties.TryGetValue("EndpointName", out var en) &&
            en is ScalarValue { Value: string endpointName } &&
            endpointName.Contains("Health"))
            return true;

        return false;
    }

    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        Log.Information("Application starting up in {EnvironmentName} mode", builder.Environment.EnvironmentName);

        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((ctx, serviceProvider, lc) =>
        {
            lc
                .ReadFrom.Configuration(ctx.Configuration)
                .Filter.ByExcluding(IsHealthCheckLog)
                .Enrich.WithProperty("ApplicationName", "MecanicaHermes")
                .Enrich.WithSpan()
                .Enrich.With<LogLevelEnricher>();

            if (!EnvironmentDetector.IsKubernetes)
            {
                lc.WriteTo.Console();
                return;
            }

            var licenseKey = ctx.Configuration["Observability:LicenseKey"]
                             ?? Environment.GetEnvironmentVariable("NEW_RELIC_LICENSE_KEY")
                             ?? throw new InvalidOperationException(
                                 "NEW_RELIC_LICENSE_KEY is required in Kubernetes.");

            var otlpEndpoint = ctx.Configuration["Observability:OtlpLogsEndpoint"]
                               ?? "https://otlp.nr-data.net:4318/v1/logs";

            lc.WriteTo.OpenTelemetry(opts =>
            {
                opts.Endpoint = otlpEndpoint;
                opts.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf;
                opts.Headers = new Dictionary<string, string>
                {
                    ["api-key"] = licenseKey
                };
                opts.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = "mecanica-hermes-api",
                    ["k8s.pod.name"] = EnvironmentDetector.PodName,
                    ["k8s.namespace.name"] = EnvironmentDetector.PodNamespace,
                    ["k8s.cluster.name"] = EnvironmentDetector.ClusterName,
                };
            });
        });
    }

    public static void UseSerilogConfiguration(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
    }
}
