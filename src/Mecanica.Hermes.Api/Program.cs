using System.Text.Json.Serialization;
using Mecanica.Hermes.Api;
using Mecanica.Hermes.Api.Endpoints.Clientes;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico;
using Mecanica.Hermes.Api.Endpoints.Produtos;
using Mecanica.Hermes.Api.Middleware;
using Mecanica.Hermes.Infrastructure.DependencyInjection;
using Mecanica.Hermes.Infrastructure.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

if (builder.Environment.EnvironmentName != "Testing")
    builder.AddSerilogConfiguration();

builder.AddObservability();

builder.Services.ConfigureInfrastructureServices(typeof(IApiAssemblyMarker).Assembly);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready", "database"]);

builder.Host.ConfigureHostBuilder();

var app = builder.Build();

if (builder.Environment.EnvironmentName != "Testing")
    app.UseSerilogConfiguration();

app.UseCorrelationId();
app.UseGlobalExceptionHandler();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseDevelopmentAuthentication();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false, // only the always-healthy "self" check
});
app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("ready"),
});
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("ready"),
});

app.MapClientesEndpoints();
app.MapProdutosEndpoints();
app.MapServicosEndpoints();
app.MapOrdensDeServicoEndpoints();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

lifetime.ApplicationStarted.Register(() =>
    logger.LogInformation("Aplicação iniciada com sucesso."));

await app.MigrateDatabaseAsync();

await app.RunAsync();
