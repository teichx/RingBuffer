using RabbitMQ.Client;

namespace TingTuffer.Examples.WorkerStatic.Service
{
    internal enum EnumQueue
    {
        WithRingBuffer,
        WithoutRingBuffer,
        ConnectionAlwaysOn,
    }

    internal static class ServicePublisher
    {
        public const string TopicName = "amq.direct";
        public const string QueueWithRingBuffer = "queue.with.ting.tuffer";
        public const string QueueWithoutRingBuffer = "queue.without.ting.tuffer";
        public const string QueueConnectionAlwayOn = "queue.connection.always.on";

        static string GetRoutingKey(EnumQueue enumQueue)
            => enumQueue switch
            {
                EnumQueue.WithRingBuffer => QueueWithRingBuffer,
                EnumQueue.WithoutRingBuffer => QueueWithoutRingBuffer,
                EnumQueue.ConnectionAlwaysOn => QueueConnectionAlwayOn,
                _ => string.Empty,
            };

        public static string Publish(IModel model, EnumQueue enumQueue)
        {
            var routingKey = GetRoutingKey(enumQueue);

            var id = Guid.NewGuid();
            try
            {
                var body = id.ToByteArray();

                model.BasicPublish(
                    exchange: TopicName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: null,
                    body: body
                );

                return $"[{DateTime.UtcNow}] Published [{routingKey}] ring buffer {id}";
            }
            catch (Exception e)
            {
                return $"[{DateTime.UtcNow}] Exception on publishing in [{routingKey} queue] message {id} | when exception {e.Message}";
            }
        }

        public static IModel CreateModel()
            => new ConnectionFactory()
                .CreateConnection()
                .CreateModel();

        static void QueueDeclare(IModel model, string queue)
        {
            model.QueueDeclare(queue, false, true, true, null);
            model.QueueBind(queue, TopicName, queue, null);
        }

        public static void CreateQueues(IModel model)
            => new List<string> { 
                QueueWithoutRingBuffer,
                QueueWithRingBuffer,
                QueueConnectionAlwayOn,
            }.ForEach(x => QueueDeclare(model, x));

        public static void ShowResults(IModel model, TimeSpan timelapse)
        {
            try
            {
                var withRingBuffer = model.MessageCount(QueueWithRingBuffer);
                var withoutRingBuffer = model.MessageCount(QueueWithoutRingBuffer);
                var connectionAlwaysOn = model.MessageCount(QueueConnectionAlwayOn);
                var useAlwaysOn = connectionAlwaysOn > 0;


                Console.WriteLine("{0} messages published using ring buffer", withRingBuffer);
                Console.WriteLine("{0} messages published without ring buffer", withoutRingBuffer);
                if(useAlwaysOn)
                    Console.WriteLine("{0} messages published maintain connection opened", connectionAlwaysOn);


                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("{0}% faster using ring buffer compare to without ring buffer", (withRingBuffer / withoutRingBuffer));

                if(useAlwaysOn)
                    Console.WriteLine("{0}% faster using ring buffer compare to maintain opened connection", (withRingBuffer / connectionAlwaysOn));

                Console.ResetColor();

                Console.WriteLine("{0} seconds for duration", timelapse.TotalSeconds);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e.Message);
            }
        }
    }
}
