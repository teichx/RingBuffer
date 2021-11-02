using TingTuffer.Base;

namespace TingTuffer.Examples.Api.Publisher
{
    public class TingTufferPublisher : RingBufferBase<Publisher>
    {
        public TingTufferPublisher() { }
        public TingTufferPublisher(int size) : base(size) { }

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
