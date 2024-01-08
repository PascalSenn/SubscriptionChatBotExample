using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Subscriptions.Example.Chat;

public sealed class ProcessingWorker : BackgroundService
{
    private const int _maxParallelProcessing = 10;

    private readonly IChatMessageProcessor _processor;

    public ProcessingWorker(IChatMessageProcessor processor)
    {
        _processor = processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new Task[_maxParallelProcessing];

        for (var i = 0; i < _maxParallelProcessing; i++)
        {
            tasks[i] = Process(stoppingToken);
        }

        await Task.WhenAll(tasks);
    }

    private async Task Process(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _processor.ProcessAsync(stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch
            {
                // ignored
            }
        }
    }
}
