using RabbitMQ.Client;
using TingTuffer.Examples.WorkerStatic.Service;

namespace TingTuffer.Examples.WorkerStatic
{
    public class WorkerConnectionAlwayOn : BackgroundService
    {
        private readonly ILogger<WorkerConnectionAlwayOn> _logger;

        public WorkerConnectionAlwayOn(ILogger<WorkerConnectionAlwayOn> logger)
            => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var model = new ConnectionFactory()
                .CreateConnection()
                .CreateModel();

            var task = Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(ServicePublisher.Publish(model, EnumQueue.ConnectionAlwaysOn));
                }
            }, stoppingToken);

            await task;
        }
    }
}