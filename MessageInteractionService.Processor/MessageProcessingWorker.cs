namespace MessageInteractionService.Processor;

public class MessageProcessingWorker: IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // start listening to queue
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // stop listening to queue & dispose resources
    }
}