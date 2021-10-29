using RingBuffer.Base.Item;

namespace RingBuffer.Base
{
    public interface IRingBufferBase<T> : IDisposable where T : IDisposable
    {
        IItemRingBuffer<T> GetItem();
    }
}
