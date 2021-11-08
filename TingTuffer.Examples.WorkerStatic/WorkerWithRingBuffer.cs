using TingTuffer.Examples.WorkerStatic.IoC;
using TingTuffer.Examples.WorkerStatic.Service;

namespace TingTuffer.Examples.WorkerStatic
{
    public class WorkerWithRingBuffer : BackgroundService
    {
        private readonly ILogger<WorkerWithRingBuffer> _logger;

        public WorkerWithRingBuffer(ILogger<WorkerWithRingBuffer> logger)
            => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var item = RingBufferIoC.RabbitMQ.GetItem();

                    _logger.LogInformation(ServicePublisher.Publish(item.Item, true));
                }
            }, stoppingToken);

            await task;
        }
    }
}