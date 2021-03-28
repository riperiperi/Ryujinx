using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Ryujinx.Graphics.GAL.Multithreading.Model
{
    interface ISpanRef {
        Span<T> Get<T>(int length) where T : unmanaged;
        void Dispose<T>(int length) where T : unmanaged;
    }

    class ObjectSpanReference : ISpanRef
    {
        private byte[] _data;
        
        public ObjectSpanReference(ReadOnlySpan<byte> data)
        {
            _data = data.ToArray();
        }

        public Span<T> Get<T>(int size) where T : unmanaged
        {
            return MemoryMarshal.Cast<byte, T>(new Span<byte>(_data));
        }

        public void Dispose<T>(int size) where T : unmanaged
        {

        }
    }

    /// <summary>
    /// Resources are disposed in the order they come in, so no holes are created in the used area.
    /// </summary>
    class CircularSpanPool : ISpanRef
    {
        private byte[] _pool;
        private int _size;

        private int _producerPtr;
        private int _producerSkipPosition = -1;
        private int _consumerPtr;

        public CircularSpanPool(int size)
        {
            _size = size;
            _pool = new byte[size];
        }

        public ISpanRef Produce<T>(ReadOnlySpan<T> data) where T : unmanaged
        {
            int size = data.Length * Unsafe.SizeOf<T>();

            // Wrapping aware circular queue.
            // If there's no space at the end of the pool for this span, we can't fragment it.
            // So just loop back around to the start. Remember the last skipped position.

            bool wraparound = _producerPtr + size >= _size;
            int index = wraparound ? 0 : _producerPtr;

            // _consumerPtr is from another thread, and we're taking it without a lock, so treat this as a snapshot in the past.
            // We know that it will always be before or equal to the producer pointer, and it cannot pass it.
            // This is enough to reason about if there is space in the queue for the data, even if we're checking against an outdated value.

            int consumer = _consumerPtr;
            bool beforeConsumer = _producerPtr < consumer;

            if (size > _size - 1 || (wraparound && beforeConsumer) || ((index < consumer || wraparound) && index + size >= consumer))
            {
                // Just get an array in the following situations:
                // - The data is too large to fit in the pool.
                // - A wraparound would happen but the consumer would be covered by it.
                // - The producer would catch up to the consumer as a result.

                return new ObjectSpanReference(MemoryMarshal.Cast<T, byte>(data));
            }

            data.CopyTo(MemoryMarshal.Cast<byte, T>(new Span<byte>(_pool).Slice(index, size)));

            if (wraparound)
            {
                _producerSkipPosition = _producerPtr;
            }

            _producerPtr = index + size;

            return this;
        }

        public Span<T> Get<T>(int length) where T : unmanaged
        {
            int size = length * Unsafe.SizeOf<T>();

            if (_consumerPtr == Interlocked.CompareExchange(ref _producerSkipPosition, -1, _consumerPtr))
            {
                _consumerPtr = 0;
            }

            return MemoryMarshal.Cast<byte, T>(new Span<byte>(_pool).Slice(_consumerPtr, size));
        }

        public void Dispose<T>(int length) where T : unmanaged
        {
            int size = length * Unsafe.SizeOf<T>();

            _consumerPtr = _consumerPtr + size;
        }
    }
}
