using System.Reflection;
using Mecanica.Hermes.Application;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain;
using Mecanica.Hermes.Infrastructure.Emails.DependencyInjection;
using Mecanica.Hermes.Infrastructure.Observability;
using Mecanica.Hermes.Infrastructure.Persistence.DependencyInjection;
using Mecanica.Hermes.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Mecanica.Hermes.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services, Assembly apiAssemblyMarker)
    {
        services.AddSingleton<BusinessMetrics>();
        services.AddSingleton<IClienteMetrics>(sp => sp.GetRequiredService<BusinessMetrics>());
        services.AddSingleton<IOrdemDeServicoMetrics>(sp => sp.GetRequiredService<BusinessMetrics>());
        services.AddEmailConfiguration();
        services.AddEntityFrameworkConfiguration();

        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = Environment.GetEnvironmentVariable(VariableNames.MediatRLicenseKey);
            cfg.RegisterServicesFromAssemblies(apiAssemblyMarker);
            cfg.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>();
            cfg.RegisterServicesFromAssemblyContaining<IDomainAssemblyMarker>();
            cfg.RegisterServicesFromAssemblyContaining<IInfrastructureAssemblyMarker>();
            cfg.RegisterGenericHandlers = true;
        });

        services.AddAutoMapper(
            cfg => { cfg.LicenseKey = Environment.GetEnvironmentVariable(VariableNames.AutoMapperLicenseKey); },
            apiAssemblyMarker,
            typeof(IApplicationAssemblyMarker).Assembly,
            typeof(IDomainAssemblyMarker).Assembly,
            typeof(IInfrastructureAssemblyMarker).Assembly
        );

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = Environment.GetEnvironmentVariable(VariableNames.AuthAuthority);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Environment.GetEnvironmentVariable(VariableNames.AuthIssuer),
                    ValidateAudience = false
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.AllowClienteScope!, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var scopeClaims = context.User.FindAll("scope");
                    return scopeClaims.Any(scopeClaim =>
                    {
                        var scopes = scopeClaim.Value
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        return scopes.Contains(AuthPolicies.AllowClienteScope) ||
                               scopes.Contains(AuthPolicies.OnlyAdminScope);
                    });
                });
            })
            .AddPolicy(AuthPolicies.OnlyAdminScope!, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var scopeClaims = context.User.FindAll("scope");
                    return scopeClaims.Any(scopeClaim =>
                    {
                        var scopes = scopeClaim.Value
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        return scopes.Contains(AuthPolicies.OnlyAdminScope);
                    });
                });
            });
    }
}