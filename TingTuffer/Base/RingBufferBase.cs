using TingTuffer.Base.Item;
using System.Collections.Concurrent;

namespace TingTuffer.Base
{
    public abstract class RingBufferBase<T> : IRingBufferBase<T> where T : IDisposable
    {
        const int DefaultSize = 6;

        public int RingSize { get; private set; }

        readonly ConcurrentQueue<T> RingQueue;

        public RingBufferBase(int tamanho = DefaultSize)
        {
            RingSize = tamanho;
            RingQueue = new ConcurrentQueue<T>(FillItems());
        }

        IEnumerable<T> FillItems()
            => Enumerable.Range(0, RingSize)
                .Select(_ => CreateFactory());

        public IItemRingBuffer<T> GetItem()
            => new ItemRingBuffer<T>(RingQueue, CreateFactory, Validate);

        protected abstract T CreateFactory();
        protected abstract bool Validate(T item);

        public void Dispose()
        {
            while (RingQueue.IsEmpty is false)
            {
                if (RingQueue.TryDequeue(out T? item))
                {
                    item.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
