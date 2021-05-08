using Ryujinx.Graphics.Gpu.Memory;
using Ryujinx.Graphics.Gpu.State;
using System;
using System.Runtime.InteropServices;

namespace Ryujinx.Graphics.Gpu.Engine
{
    partial class Methods
    {
        private const int MaxUboSize = 4096;

        // State associated with direct uniform buffer updates.
        // This state is used to attempt to batch together consecutive updates.
        private Memory.Buffer _ubLastWritten;
        private ulong _ubBeginCpuAddress = 0;
        private ulong _ubFollowUpAddress = 0;
        private int[] _ubData = new int[MaxUboSize];
        private int _ubIntCount = 0;

        /// <summary>
        /// Flushes any queued ubo updates.
        /// </summary>
        private void FlushUboUpdate()
        {
            if (_ubLastWritten != null)
            {
                Span<byte> data = MemoryMarshal.Cast<int, byte>(new Span<int>(_ubData, 0, _ubIntCount));
                _ubLastWritten.SetData(_ubBeginCpuAddress, data);

                _ubFollowUpAddress = 0;
                _ubLastWritten = null;
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

            ulong currentCpuAddress = _ubBeginCpuAddress + (ulong)_ubIntCount * 4;

            if (_ubFollowUpAddress != address || !_ubLastWritten.FullyContains(currentCpuAddress, 4) || _ubIntCount + 1 >= MaxUboSize)
            {
                FlushUboUpdate();

                _ubFollowUpAddress = address;
                _ubIntCount = 0;

                UboCacheEntry entry = BufferManager.TranslateCreateAndGetUbo(address, 4);
                _ubBeginCpuAddress = entry.Address;
                _ubLastWritten = entry.Buffer;
            }

            _context.PhysicalMemory.WriteUntracked(_ubBeginCpuAddress + (ulong)_ubIntCount * 4, MemoryMarshal.Cast<int, byte>(MemoryMarshal.CreateSpan(ref argument, 1)));

            _ubFollowUpAddress = address + 4;
            _ubData[_ubIntCount++] = argument;

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

            ulong currentCpuAddress = _ubBeginCpuAddress + (ulong)_ubIntCount * 4;

            if (_ubFollowUpAddress != address || !_ubLastWritten.FullyContains(currentCpuAddress, size) || _ubIntCount + data.Length >= MaxUboSize)
            {
                FlushUboUpdate();

                _ubFollowUpAddress = address;
                _ubIntCount = 0;

                UboCacheEntry entry = BufferManager.TranslateCreateAndGetUbo(address, size);
                _ubBeginCpuAddress = entry.Address;
                _ubLastWritten = entry.Buffer;
            }

            _context.PhysicalMemory.WriteUntracked(_ubBeginCpuAddress + (ulong)_ubIntCount * 4, MemoryMarshal.Cast<int, byte>(data));

            _ubFollowUpAddress = address + size;
            data.CopyTo(new Span<int>(_ubData, _ubIntCount, data.Length));
            _ubIntCount += data.Length;

            state.SetUniformBufferOffset(uniformBuffer.Offset + data.Length * 4);
        }
    }
}