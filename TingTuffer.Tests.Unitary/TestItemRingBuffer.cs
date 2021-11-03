using System;
using System.Collections.Concurrent;
using System.IO;
using TingTuffer.Base.Item;
using Xunit;

namespace TingTuffer.Tests.Unitary
{
    public class TestItemRingBuffer
    {
        [Fact]
        public void TestAssignableFromInterface()
        {
            var item = new ItemRingBuffer<Stream>();

            Assert.IsAssignableFrom<IItemRingBuffer<Stream>>(item);
        }

        [Fact]
        public void TestAssignableFromIDisposable()
        {
            var item = new ItemRingBuffer<Stream>();

            Assert.IsAssignableFrom<IDisposable>(item);
        }

        [Fact]
        public void MutationTestShouldIsNotEmptyConcurrentQueue()
        {
            var memoryStream = new MemoryStream();
            var concurrentQueue = new ConcurrentQueue<MemoryStream>();
            concurrentQueue.Enqueue(memoryStream);


            var item = new ItemRingBuffer<MemoryStream>(
                buffer: concurrentQueue, 
                createFunction: () => new MemoryStream(), 
                valid: _ => true
            );

            Assert.Empty(concurrentQueue);
            item.Dispose();
            Assert.NotEmpty(concurrentQueue);
        }

        [Fact]
        public void MutationTestShouldDontCreateNewItem()
        {
            var memoryStream = new MemoryStream();
            var concurrentQueue = new ConcurrentQueue<MemoryStream>();
            concurrentQueue.Enqueue(memoryStream);

            var item = new ItemRingBuffer<MemoryStream>(
                buffer: concurrentQueue,
                createFunction: () => new MemoryStream(),
                valid: _ => true
            );

            item.Dispose();

            Assert.Single(concurrentQueue);
            concurrentQueue.TryPeek(out var otherMemoryStream);
            Assert.Same(otherMemoryStream, memoryStream);
        }

        [Fact]
        public void MutationTestShouldCreateNewItem()
        {
            var memoryStream = new MemoryStream();

            var concurrentQueue = new ConcurrentQueue<MemoryStream>();
            concurrentQueue.Enqueue(memoryStream);

            var item = new ItemRingBuffer<MemoryStream>(
                buffer: concurrentQueue,
                createFunction: () => new MemoryStream(),
                valid: _ => false
            );

            item.Dispose();

            while (concurrentQueue.IsEmpty);

            Assert.Single(concurrentQueue);
            concurrentQueue.TryPeek(out var otherMemoryStream);
            Assert.NotSame(otherMemoryStream, memoryStream);
        }
    }
}