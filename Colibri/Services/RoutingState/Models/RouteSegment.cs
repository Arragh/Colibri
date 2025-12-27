namespace Colibri.Services.RoutingState.Models;

public readonly struct RouteSegment(bool isParameter, string? parameterName, string? literal)
{
    public readonly bool IsParameter = isParameter;
    public readonly string? ParameterName = parameterName;
    public readonly string? Literal = literal;
}