using RabbitMQ.Client;
using TingTuffer.Examples.Api.Dto;

namespace TingTuffer.Examples.Api.Publisher
{
    public class Publisher : IDisposable
    {
        public readonly IModel Model;
        public Publisher()
            => (Model) = (CreateModel());

        public ValueTask<ResponseDto> PublishWithTingTuffer(Guid id)
            => PublishBase(id, true);

        public ValueTask<ResponseDto> PublishWithoutTingTuffer(Guid id)
            => PublishBase(id, false);

        ValueTask<ResponseDto> PublishBase(Guid id, bool useTingTuffer)
        {
            var withOrWithout = useTingTuffer
                    ? "with"
                    : "without";

            try
            {
                var routingKey = useTingTuffer
                    ? QueueWithTingTuffer
                    : QueueWithoutTingTuffer;

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

        public const string QueueWithTingTuffer = "queue.with.ting.tuffer";
        public const string QueueWithoutTingTuffer = "queue.without.ting.tuffer";
        public const string Topic = "amq.direct";

        public static void CreateQueue()
        {
            var model = CreateModel();

            QueueDeclare(model, QueueWithTingTuffer);
            QueueDeclare(model, QueueWithoutTingTuffer);
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
                HostName = "rabbitmq-ting-tuffer"
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
