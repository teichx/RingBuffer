namespace RingBuffer.Examples.Api.Publisher
{
    public class Publisher : IDisposable
    {
        public Publisher()
        {
            // simulate time to connection
            Thread.Sleep(15);
        }

        public ValueTask<bool> Publish(Guid _)
        {
            // simulate time to publish
            Thread.Sleep(25);
            return ValueTask.FromResult(true);
        }

        public void Dispose()
        {
            // dispose connection
        }
    }
}
