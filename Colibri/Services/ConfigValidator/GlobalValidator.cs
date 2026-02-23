namespace Colibri.Services.ConfigValidator;

public sealed class GlobalValidator
{
    public ClusterValidator Clusters { get; } = new();
    public RouteValidator Routes { get; } = new();
    public CrossReferenceValidator CrossReferences { get; } = new();
}