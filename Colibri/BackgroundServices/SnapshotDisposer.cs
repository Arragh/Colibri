using System.Collections.Concurrent;
using Colibri.Models;

namespace Colibri.BackgroundServices;

public class SnapshotDisposer : BackgroundService
{
    private readonly ConcurrentQueue<RoutingSnapshot> _queue = new();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (_queue.TryDequeue(out var oldSnapshot))
            {
                var transportsCount = oldSnapshot.Transports.Length;
                
                foreach (var t in oldSnapshot.Transports)
                {
                    if (t.ReadyToDispose)
                    {
                        t.Dispose();
                        transportsCount--;
                    }
                }

                if (transportsCount > 0)
                {
                    _queue.Enqueue(oldSnapshot);
                }
            }
            
            await Task.Delay(5000, stoppingToken);
        }
        
        while (_queue.TryDequeue(out var oldSnapshot))
        {
            try
            {
                foreach (var t in oldSnapshot.Transports)
                {
                    t.Dispose();
                }
            }
            catch
            {
                Console.WriteLine("TROLOLO");
            }
        }
    }

    internal void Enqueue(RoutingSnapshot snapshot) => _queue.Enqueue(snapshot);
}