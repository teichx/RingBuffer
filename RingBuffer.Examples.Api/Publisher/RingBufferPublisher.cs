using RingBuffer.Base;

namespace RingBuffer.Examples.Api.Publisher
{
    public class RingBufferPublisher : RingBufferBase<Publisher>
    {
        public RingBufferPublisher() { }
        public RingBufferPublisher(int size) : base(size) { }

        protected override Publisher CreateFactory()
            => new();

        // To simulate dispose always after use
        // it depends on your context
        protected override bool Validate(Publisher item)
            => false;
    }
}
