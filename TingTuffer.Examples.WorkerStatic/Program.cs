using TingTuffer.Examples.WorkerStatic;
using TingTuffer.Examples.WorkerStatic.IoC;
using TingTuffer.Examples.WorkerStatic.Service;

// To inicialize connections previous use
var createObjects = RingBufferIoC.RabbitMQ;

var model = ServicePublisher.CreateModel();
ServicePublisher.CreateQueues(model);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerWithoutRingBuffer>();
        //services.AddHostedService<WorkerConnectionAlwayOn>();
        services.AddHostedService<WorkerWithRingBuffer>();
    })
    .Build();


var task = Task.Run(async () =>
{
    var testTime = TimeSpan.FromSeconds(15);
    await Task.Delay(testTime);
    await host.StopAsync();

    ServicePublisher.ShowResults(model, testTime);
});

await host.RunAsync();
await task;

