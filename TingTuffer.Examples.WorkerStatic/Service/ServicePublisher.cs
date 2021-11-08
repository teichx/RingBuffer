using RabbitMQ.Client;

namespace TingTuffer.Examples.WorkerStatic.Service
{
    internal static class ServicePublisher
    {
        public const string TopicName = "amq.direct";
        public const string QueueWithRingBuffer = "queue.with.ting.tuffer";
        public const string QueueWithoutRingBuffer = "queue.without.ting.tuffer";

        public static string Publish(IModel model, bool useTingTuffer)
        {
            var routingKey = useTingTuffer
                ? QueueWithRingBuffer
                : QueueWithoutRingBuffer;

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

                return $"[{DateTime.UtcNow}] Published [{(useTingTuffer ? 'x' : ' ')}] ring buffer {id}";
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

        public static void CreateQueues(IModel model)
        {
            model.QueueDeclare(QueueWithRingBuffer, false, true, true, null);
            model.QueueBind(QueueWithRingBuffer, TopicName, QueueWithRingBuffer, null);

            model.QueueDeclare(QueueWithoutRingBuffer, false, true, true, null);
            model.QueueBind(QueueWithoutRingBuffer, TopicName, QueueWithoutRingBuffer, null);
        }

        public static void ShowResults(IModel model, TimeSpan timelapse)
        {
            try
            {
                var withRingBuffer = model.MessageCount(QueueWithRingBuffer);
                var withoutRingBuffer = model.MessageCount(QueueWithoutRingBuffer);

                Console.WriteLine("{0} messages published using ring buffer", withRingBuffer);
                Console.WriteLine("{0} messagespublished without ring buffer", withoutRingBuffer);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("{0}% faster using ring buffer in this example", (withRingBuffer / withoutRingBuffer));
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
