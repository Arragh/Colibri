using System.Collections.Immutable;

namespace Colibri.Services.Snapshot.Models;

public sealed class TransportConfig
{
    public required ImmutableArray<HttpMessageInvoker> Invokers { get; init; }
}