using System.Collections.Immutable;

namespace Colibri.Services.SnapshotProvider.Models;

public sealed class TransportConfig
{
    public required ImmutableArray<HttpMessageInvoker> Invokers { get; init; }
}