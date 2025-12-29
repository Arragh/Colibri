using System.Collections.Immutable;

namespace Colibri.Services.Snapshot.Models;

public sealed class TransportRuntimeSnapshot
{
    public ImmutableArray<TransportConfig> Transports { get; init; }
}