using System.Collections.Immutable;

namespace Colibri.Snapshots.TransportSnapshot.Models;

public sealed class TransportConfig
{
    public required ImmutableArray<HttpMessageInvoker> Invokers { get; init; }
}