using System.Collections.Concurrent;

namespace TingTuffer.Base.Item
{
    public struct ItemRingBuffer<T> : IItemRingBuffer<T> where T : IDisposable
    {
        public T Item { get; private set; }

        readonly ConcurrentQueue<T> Buffer;
        readonly Func<T> CreateFunction;
        readonly Func<T, bool> ValidateFunction;

        public ItemRingBuffer(ConcurrentQueue<T> buffer, Func<T> createFunction, Func<T, bool> valid)
        {
            T? current;
            Buffer = buffer;
            CreateFunction = createFunction;
            ValidateFunction = valid;

            while (buffer.TryDequeue(out current) is false)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(5));
            }

            Item = current;
        }

        public void Dispose()
        {
            var valid = ValidateFunction(Item);
            if (valid)
                Buffer.Enqueue(Item);
            else
            {
                var thisInstance = this;
                
                Task.Run(() =>
                {
                    thisInstance.Buffer.Enqueue(thisInstance.CreateFunction());
                    thisInstance.Item.Dispose();
                });
            }


            GC.SuppressFinalize(this);
        }
    }
}
