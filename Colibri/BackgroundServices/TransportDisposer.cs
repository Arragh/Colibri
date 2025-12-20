using System.Collections.Concurrent;
using Colibri.Interfaces.Services.Http;

namespace Colibri.BackgroundServices;

public class TransportDisposer : BackgroundService
{
    private readonly ConcurrentQueue<ITransport> _queue = new();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var count = _queue.Count;

            for (int i = 0; i < count; i++)
            {
                if (_queue.TryDequeue(out var transport))
                {
                    if (transport is IDisposable disposable)
                    {
                        try
                        {
                            disposable.Dispose();
                            Console.WriteLine($"{transport.GetType().Name} disposed");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            _queue.Enqueue(transport);
                        }
                    }
                }
            }
            
            await Task.Delay(5000, stoppingToken);
        }
    }

    internal void Enqueue(ITransport transport)
    {
        _queue.Enqueue(transport);
        Console.WriteLine($"{transport.GetType().Name} queued");
    }
}