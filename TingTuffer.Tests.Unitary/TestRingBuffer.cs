using Moq;
using Moq.Protected;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using TingTuffer.Base;
using Xunit;

namespace TingTuffer.Tests.Unitary
{
    public class TestRingBuffer
    {
        [Fact]
        public void TestAssignableFromInterface()
        {
            var mock = new Mock<RingBufferBase<Stream>>(1);

            Assert.IsAssignableFrom<IRingBufferBase<Stream>>(mock.Object);
        }

        [Fact]
        public void TestAssignableFromIDisposable()
        {
            var mock = new Mock<RingBufferBase<Stream>>(1);

            Assert.IsAssignableFrom<IDisposable>(mock.Object);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(24)]
        public void TestAssignRingSize(int ringSize)
        {
            var mock = new Mock<RingBufferBase<IDisposable>>(ringSize);

            Assert.Equal(ringSize, mock.Object.RingSize);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(6)]
        public void TestFillBuffer(int ringSize)
        {
            var mock = new Mock<RingBufferBase<MemoryStream>>(ringSize);

            var counter = 0;

            mock.Protected()
                .Setup<MemoryStream>("CreateFactory")
                .Returns(() => new MemoryStream(counter++));

            var ringQueue = (ConcurrentQueue<MemoryStream>?)typeof(RingBufferBase<MemoryStream>)
                .GetRuntimeFields()
                .FirstOrDefault(x => "RingQueue".Equals(x.Name))
                ?.GetValue(mock.Object);

            var capacityResult = ringQueue?.Select(x => x.Capacity);

            var expectedResult = Enumerable.Range(0, ringSize)
                .Select(x => x);

            Assert.Equal(ringSize, capacityResult?.Count());
            Assert.Equal(expectedResult, capacityResult);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(6)]
        public void TestDisposeShouldClearRing(int ringSize)
        {
            var mock = new Mock<RingBufferBase<IDisposable>>(ringSize);
            var mockItem = new Mock<IDisposable>();

            mock.Protected()
                .Setup<IDisposable>("CreateFactory")
                .Returns(mockItem.Object);

            var ringQueue = (ConcurrentQueue<IDisposable>?)typeof(RingBufferBase<IDisposable>)
                .GetRuntimeFields()
                .FirstOrDefault(x => "RingQueue".Equals(x.Name))
                ?.GetValue(mock.Object);

            var previousDisposeRingCount = ringQueue?.Count;
            mock.Object.Dispose();

            Assert.Equal(ringSize, previousDisposeRingCount);
            Assert.Equal(0, ringQueue?.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void TestDisposeShouldDisposeItens(int ringSize)
        {
            var mock = new Mock<RingBufferBase<IDisposable>>(ringSize);

            var mockItem = new Mock<IDisposable>();
            mockItem.Setup(x => x.Dispose())
                .Verifiable();

            mock.Protected()
                .Setup<IDisposable>("CreateFactory")
                .Returns(mockItem.Object);

            mock.Object.Dispose();
            Assert.Equal(ringSize, mockItem.Invocations.Count);
        }
    }
}
