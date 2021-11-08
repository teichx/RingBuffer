using RabbitMQ.Client;
using TingTuffer.Base;
using TingTuffer.Examples.WorkerStatic.RingBuffer;

namespace TingTuffer.Examples.WorkerStatic.IoC
{
    internal static class RingBufferIoC
    {
        static IRingBufferBase<IModel>? rabbitMQ;
        public static IRingBufferBase<IModel> RabbitMQ { get => rabbitMQ ??= new RingBufferRabbitMQ(); }
    }
}
