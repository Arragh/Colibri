using System.Threading.Channels;
using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Runtime.Snapshots;

public sealed class SnapshotProvider
{
    private GlobalSnapshot _globalSnapshot;
    private GlobalSnapshotBuilder _globalSnapshotBuilder = new();
    
    public SnapshotProvider(
        IOptionsMonitor<ColibriSettings> monitor,
        Channel<IAsyncDisposable> channel)
    {
        _globalSnapshot = _globalSnapshotBuilder.Build(monitor.CurrentValue);

        monitor.OnChange(c =>
        {
            var newGlobalSnapshot = _globalSnapshotBuilder.Build(c);
            var oldGlobalSnapshot = Interlocked.Exchange(ref _globalSnapshot, newGlobalSnapshot);

            if (!channel.Writer.TryWrite(oldGlobalSnapshot))
            {
                Console.WriteLine("SNAPSHOT DISPOSE ERROR"); // TODO: заменить на логгер (когда тот будет добавлен)
            }
        });
    }

    public GlobalSnapshot GlobalSnapshot => Volatile.Read(ref _globalSnapshot);
}