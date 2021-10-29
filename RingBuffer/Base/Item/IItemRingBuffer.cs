﻿namespace RingBuffer.Base.Item
{
    public interface IItemRingBuffer<T> : IDisposable where T : IDisposable
    {
        T Item { get; }
    }
}
