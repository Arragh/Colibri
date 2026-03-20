namespace Colibri.Services.ConfigValidator;

public sealed class GlobalValidator
{
    public JwtSchemeValidator JwtSchemes { get; } = new();
    public ClusterValidator Clusters { get; } = new();
    public RouteValidator Routes { get; } = new();
    public CrossReferenceValidator CrossReferences { get; } = new();
}