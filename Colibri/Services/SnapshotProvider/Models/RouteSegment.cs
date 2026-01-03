namespace Colibri.Services.SnapshotProvider.Models;

public sealed class RouteSegment(bool isParameter, string? parameterName, string? literal)
{
    public readonly bool IsParameter = isParameter;
    public readonly string? ParameterName = parameterName;
    public readonly string? Literal = literal;
}