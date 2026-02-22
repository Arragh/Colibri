using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace Colibri.Services;

public sealed class GlobalAsyncDisposer(Channel<IAsyncDisposable> channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await item.DisposeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // TODO: заменить на логгер (когда тот будет добавлен)
            }
        }
    }
}