using Ryujinx.Graphics.Gpu.Memory;
using Ryujinx.Graphics.Gpu.State;
using System;
using System.Runtime.InteropServices;

namespace Ryujinx.Graphics.Gpu.Engine
{
    partial class Methods
    {
        private const int MaxUboSize = 4096;

        private Memory.Buffer _lastWrittenUb;
        private ulong _beginAddress = 0;
        private ulong _beginCpuAddress = 0;
        private ulong _followUpAddress = 0;
        private int[] _data = new int[MaxUboSize];
        private int _intCount = 0;

        /// <summary>
        /// Flushes any queued ubo updates.
        /// </summary>
        private void FlushUboUpdate()
        {
            if (_lastWrittenUb != null)
            {
                Span<byte> data = MemoryMarshal.Cast<int, byte>(new Span<int>(_data, 0, _intCount));
                _context.PhysicalMemory.WriteUntracked(_beginCpuAddress, data);
                _lastWrittenUb.SetData(_beginCpuAddress, data);

                _followUpAddress = 0;
                _lastWrittenUb = null;
            }
        }

        /// <summary>
        /// Updates the uniform buffer data with inline data.
        /// </summary>
        /// <param name="state">Current GPU state</param>
        /// <param name="argument">New uniform buffer data word</param>
        private void UniformBufferUpdate(GpuState state, int argument)
        {
            var uniformBuffer = state.Get<UniformBufferState>(MethodOffset.UniformBufferState);

            ulong address = uniformBuffer.Address.Pack() + (uint)uniformBuffer.Offset;

            ulong currentCpuAddress = _beginCpuAddress + (ulong)_intCount * 4;

            if (_followUpAddress != address || !_lastWrittenUb.FullyContains(currentCpuAddress, 4) || _intCount + 1 >= MaxUboSize)
            {
                FlushUboUpdate();

                _followUpAddress = address;
                _intCount = 0;

                UboCacheEntry entry = BufferManager.TranslateCreateAndGetUbo(address, 4);
                _beginCpuAddress = entry.Address;
                _lastWrittenUb = entry.Buffer;
            }

            _followUpAddress = address + 4;
            _data[_intCount++] = argument;

            state.SetUniformBufferOffset(uniformBuffer.Offset + 4);
        }

        /// <summary>
        /// Updates the uniform buffer data with inline data.
        /// </summary>
        /// <param name="state">Current GPU state</param>
        /// <param name="data">Data to be written to the uniform buffer</param>
        public void UniformBufferUpdate(GpuState state, ReadOnlySpan<int> data)
        {
            var uniformBuffer = state.Get<UniformBufferState>(MethodOffset.UniformBufferState);

            ulong address = uniformBuffer.Address.Pack() + (uint)uniformBuffer.Offset;

            ulong size = (ulong)data.Length * 4;

            ulong currentCpuAddress = _beginCpuAddress + (ulong)_intCount * 4;

            if (_followUpAddress != address || !_lastWrittenUb.FullyContains(currentCpuAddress, size) || _intCount + data.Length >= MaxUboSize)
            {
                FlushUboUpdate();

                _beginAddress = address;
                _followUpAddress = address;
                _intCount = 0;

                UboCacheEntry entry = BufferManager.TranslateCreateAndGetUbo(address, size);
                _beginCpuAddress = entry.Address;
                _lastWrittenUb = entry.Buffer;
            }

            _followUpAddress = address + size;
            data.CopyTo(new Span<int>(_data, _intCount, data.Length));
            _intCount += data.Length;

            state.SetUniformBufferOffset(uniformBuffer.Offset + data.Length * 4);
        }
    }
}