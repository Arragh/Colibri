using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.RoutingState.Interfaces;

public interface IRoutingState
{
    RoutingSnapshot Snapshot { get; }
}