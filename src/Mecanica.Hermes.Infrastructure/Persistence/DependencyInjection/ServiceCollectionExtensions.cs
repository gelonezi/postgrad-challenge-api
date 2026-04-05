using System.Diagnostics;
using System.Text.RegularExpressions;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Infrastructure.Persistence.Events;
using Mecanica.Hermes.Infrastructure.Persistence.UnitOfWork;
using Mecanica.Hermes.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Mecanica.Hermes.Infrastructure.Persistence.DependencyInjection;

internal static partial class ServiceCollectionExtensions
{
    public static void AddEntityFrameworkConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, EfUnitOfWork>();
        services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

        var connectionString = Environment.GetEnvironmentVariable(VariableNames.DefaultConnectionString);

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.ConfigureTracing(tracing => tracing
            .ConfigureCommandSpanNameProvider(cmd => BuildSpanName(cmd.CommandText))
            .ConfigureCommandEnrichmentCallback((activity, cmd) =>
            {
                activity.SetTag("db.statement", cmd.CommandText);

                var match = SqlOperationRegex().Match(cmd.CommandText);
                if (!match.Success) return;

                var op = match.Groups["op"].Value.Split(' ')[0].ToUpperInvariant();
                var table = match.Groups["table"].Value
                    .Replace("\"", "").Replace("public.", "").Trim();

                // Required by New Relic APM Databases view (OTel semconv v1.33)
                activity.SetTag("db.operation.name", op);
                activity.SetTag("db.collection.name", table);
            }));

        var dataSource = dataSourceBuilder.Build();

        services.AddSingleton(dataSource);

        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(dataSource,
                b => b.MigrationsAssembly(typeof(IInfrastructureAssemblyMarker).Namespace)
                    .MigrationsHistoryTable("__EFMigrationsHistory", "public"));
        });
    }

    private static string BuildSpanName(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return "Postgres";

        var match = SqlOperationRegex().Match(sql);
        if (!match.Success)
            return "Postgres";

        var operation = match.Groups["op"].Value.ToLowerInvariant();
        var table = match.Groups["table"].Value.ToLowerInvariant()
            .Replace("\"", "").Replace("public.", "");

        return $"{table} {operation}";
    }

    [GeneratedRegex(
        @"^\s*(?<op>SELECT|INSERT\s+INTO|UPDATE|DELETE\s+FROM)\s+(?<table>""?[\w.]+""\s*\.?\s*""?[\w.]+""?)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SqlOperationRegex();
}
