using System.Diagnostics;

namespace Mecanica.Hermes.Infrastructure.Observability;

public static class ActivitySources
{
    public const string ApplicationName = "MecanicaHermes";

    public static readonly ActivitySource Api =
        new(ApplicationName, typeof(ActivitySources).Assembly.GetName().Version?.ToString() ?? "1.0");
}
