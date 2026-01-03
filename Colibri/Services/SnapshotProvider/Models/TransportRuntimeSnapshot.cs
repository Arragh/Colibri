using System.Collections.Immutable;

namespace Colibri.Services.SnapshotProvider.Models;

public sealed class TransportRuntimeSnapshot
{
    public ImmutableArray<TransportConfig> Transports { get; init; }
}