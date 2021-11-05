using RabbitMQ.Client;
using TingTuffer.Base;

namespace TingTuffer.Examples.DefaultApi.RingBuffer
{
    public class RingBufferRabbitMQ : RingBufferBase<IModel>
    {
        public RingBufferRabbitMQ() { }
        public RingBufferRabbitMQ(int size) : base(size) { }

        protected override IModel CreateFactory()
            => new ConnectionFactory()
                  .CreateConnection()
                  .CreateModel();

        protected override bool Validate(IModel item)
            => item is not null && item.IsOpen;
    }
}
