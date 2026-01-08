using System.Collections.Immutable;
using Colibri.Snapshots.TransportSnapshot.Models;

namespace Colibri.Snapshots.TransportSnapshot;

public sealed class TransportRuntimeSnapshot
{
    public ImmutableArray<TransportConfig> Transports { get; init; }
}