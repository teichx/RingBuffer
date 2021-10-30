using RingBuffer.Base;

namespace RingBuffer.Examples.Api.Publisher
{
    public class RingBufferPublisher : RingBufferBase<Publisher>
    {
        public RingBufferPublisher() { }
        public RingBufferPublisher(int size) : base(size) { }

        protected override Publisher CreateFactory()
            => new();

        // verify if your need recreate a connection
        protected override bool Validate(Publisher item)
        {
            var valid = item.Model is not null && item.Model.IsOpen;

            if(valid is false)
                Console.WriteLine($"[{DateTime.UtcNow}] need recreate a connection");

            return valid;
        }
    }
}
