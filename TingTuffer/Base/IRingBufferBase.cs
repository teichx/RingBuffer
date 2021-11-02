using TingTuffer.Base.Item;

namespace TingTuffer.Base
{
    public interface IRingBufferBase<T> : IDisposable where T : IDisposable
    {
        IItemRingBuffer<T> GetItem();
    }
}
