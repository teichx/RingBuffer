using RabbitMQ.Client;
using RingBuffer.Examples.Api.Dto;

namespace RingBuffer.Examples.Api.Publisher
{
    public class Publisher : IDisposable
    {
        public readonly IModel Model;
        public Publisher()
            => (Model) = (CreateModel());

        public ValueTask<ResponseDto> PublishWithRingBuffer(Guid id)
            => PublishBase(id, true);

        public ValueTask<ResponseDto> PublishWithoutRingBuffer(Guid id)
            => PublishBase(id, false);

        ValueTask<ResponseDto> PublishBase(Guid id, bool useRingBuffer)
        {
            var withOrWithout = useRingBuffer
                    ? "with"
                    : "without";

            try
            {
                var routingKey = useRingBuffer
                    ? QueueWithRingBuffer
                    : QueueWithoutRingBuffer;

                var body = id.ToByteArray();

                Model.BasicPublish(
                    exchange: Topic,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: null,
                    body: body
                );

                Console.WriteLine($"[{DateTime.UtcNow}] Published [{withOrWithout} ring buffer] {id}");

                return ValueTask.FromResult(new ResponseDto(id, true));
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}] Exception on publishing in [{withOrWithout} queue] message {id} | when exception {e.Message}");
                return ValueTask.FromResult(new ResponseDto(id, false));
            }
        }

        public const string QueueWithRingBuffer = "queue.with.ring.buffer";
        public const string QueueWithoutRingBuffer = "queue.without.ring.buffer";
        public const string Topic = "amq.direct";

        public static void CreateQueue()
        {
            var model = CreateModel();

            QueueDeclare(model, QueueWithRingBuffer);
            QueueDeclare(model, QueueWithoutRingBuffer);
        }

        static void QueueDeclare(IModel model, string queueName)
        {
            model.QueueDeclare(queueName, false, true, true, null);
            model.QueueBind(queueName, Topic, queueName, null);
        }

        public static IModel CreateModel()
        {
            IConnectionFactory conectionFactory = new ConnectionFactory
            {
                HostName = "ring-buffer-rabbitmq"
            };
            var connection = conectionFactory.CreateConnection();
            var model = connection.CreateModel();

            return model;
        }

        public void Dispose()
        {
            Model.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
