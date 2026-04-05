namespace Mecanica.Hermes.Infrastructure.DependencyInjection;

public static class EnvironmentDetector
{
    public static bool IsKubernetes =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST"));

    public static string PodName =>
        Environment.GetEnvironmentVariable("MY_POD_NAME") ?? "unknown";

    public static string PodNamespace =>
        Environment.GetEnvironmentVariable("MY_POD_NAMESPACE") ?? "unknown";

    public static string NodeName =>
        Environment.GetEnvironmentVariable("MY_NODE_NAME") ?? "unknown";

    public static string ClusterName =>
        Environment.GetEnvironmentVariable("NEW_RELIC_METADATA_KUBERNETES_CLUSTER_NAME")
        ?? Environment.GetEnvironmentVariable("MY_CLUSTER_NAME")
        ?? "unknown";
}
