using System.Security.Claims;

namespace Mecanica.Hermes.Api.Middleware;

public class DevelopmentAuthenticationMiddleware(RequestDelegate next, IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if ((environment.IsDevelopment() || environment.EnvironmentName == "Testing") &&
            context.User.Identity?.IsAuthenticated != true)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "test-user-id"),
                new(ClaimTypes.Name, "Test User"),
                new(ClaimTypes.Email, "test@example.com"),
                new("scope", "mecanica-hermes:admin mecanica-hermes:cliente")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            context.User = new ClaimsPrincipal(identity);
        }

        await next(context);
    }
}

public static class DevelopmentAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseDevelopmentAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DevelopmentAuthenticationMiddleware>();
    }
}
