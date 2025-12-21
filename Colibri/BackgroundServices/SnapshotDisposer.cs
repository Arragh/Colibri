using System.Collections.Concurrent;
using Colibri.Interfaces.Services.Http;

namespace Colibri.BackgroundServices;

public class SnapshotDisposer : BackgroundService
{
    private readonly ConcurrentQueue<ITransport> _queue = new();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var count = _queue.Count;

            for (var i = 0; i < count; i++)
            {
                if (!_queue.TryDequeue(out var transport))
                {
                    break;
                }
                
                if (transport.ReadyToDispose && transport is IDisposable d)
                {
                    d.Dispose();
                }
                else
                {
                    _queue.Enqueue(transport);
                }
            }
            
            await Task.Delay(5000, stoppingToken);
        }
        
        while (_queue.TryDequeue(out var transport))
        {
            try
            {
                if (transport is IDisposable d)
                {
                    d.Dispose();
                }
            }
            catch
            {
                Console.WriteLine("TROLOLO");
            }
        }
    }

    internal void Enqueue(ITransport transport)
    {
        _queue.Enqueue(transport);
    }
}