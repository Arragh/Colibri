namespace Colibri.Services.RoutingState.Models;


/*
 * RoutingSnapshot должен быть readonly record struct — лучше производительность.
 * Но это не точно, так что пока под вопросом.
 */
public class RoutingSnapshot
{
    public required ClusterConfig[] Clusters { get; init; }
}