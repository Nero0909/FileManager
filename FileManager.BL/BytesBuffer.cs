using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileManager.BL.Interfaces;
using Nito.AsyncEx;

namespace FileManager.BL
{
    public sealed class BytesBuffer : IBytesBuffer
    {
        private const int PageSize = 4 * 1024;
        private const int GCThreshold = 100 * 1024 * 1024; //100MB

        private AsyncCollection<int> _emptySegments;
        private AsyncCollection<int> _filledSegments;
        private Dictionary<int, int> _offsetSize;
        private byte[] _buffer;

        public BytesBuffer(int bufferSize)
        {
            Reset(bufferSize);
        }

        public void Reset(int bufferSize, int bunchSize)
        {
            _buffer = new byte[bufferSize];
            _offsetSize = GenerateOffsetSizeMapping(bufferSize, bunchSize);
            _emptySegments = new AsyncCollection<int>(_offsetSize.Keys.Count);
            _filledSegments = new AsyncCollection<int>(_offsetSize.Keys.Count);

            InitializeEmptySegments();
        }

        public void Reset(int bufferSize)
        {
            Reset(bufferSize, PageSize);
        }

        public void Clear()
        {
            var bufferSize = _buffer.Length;

            _buffer = null;
            _offsetSize = null;
            _emptySegments = null;
            _filledSegments = null;

            if (bufferSize >= GCThreshold)
            {
                GC.Collect();
            }
        }

        public async Task<ArraySegment<byte>> GetEmptySegmentAsync(CancellationToken token)
        {
            var emptySegmentOffset = await _emptySegments.TakeAsync(token);
            return new ArraySegment<byte>(_buffer, emptySegmentOffset, _offsetSize[emptySegmentOffset]);
        }

        public async Task<ArraySegment<byte>> GetFilledSegmentAsync(CancellationToken token)
        {
            var filledSegmentOffset = await _filledSegments.TakeAsync(token);
            return new ArraySegment<byte>(_buffer, filledSegmentOffset, _offsetSize[filledSegmentOffset]);
        }

        public async Task FreeSegmentAsync(ArraySegment<byte> segment, CancellationToken token)
        {
            await _emptySegments.AddAsync(segment.Offset, token);
        }

        public async Task FillSegmentAsync(ArraySegment<byte> segment, int newSize, CancellationToken token)
        {
            if (segment.Count != newSize)
            {
                _offsetSize[segment.Offset] = newSize;
            }
           
            await _filledSegments.AddAsync(segment.Offset, token);
        }

        public async Task<bool> OutputAvailableAsync(CancellationToken cancellationToken)
        {
            return await _filledSegments.OutputAvailableAsync(cancellationToken);
        }

        public void CompleteAdding()
        {
            _filledSegments.CompleteAdding();
        }

        private void InitializeEmptySegments()
        {
            foreach (var offset in _offsetSize.Keys)
            {
                _emptySegments.Add(offset);
            }
        }

        private Dictionary<int, int> GenerateOffsetSizeMapping(int bufferSize, int bunchSize)
        {
            var offsetSizeDict = new Dictionary<int, int>();

            var fullSegmentsCount = bufferSize / bunchSize;
            int? lastBuffer = null;
            var mod = bufferSize % bunchSize;
            if (mod != 0)
            {
                lastBuffer = mod;
            }

            var currentOffset = 0;
            for (var i = 0; i < fullSegmentsCount; i++)
            {
                offsetSizeDict.Add(currentOffset, bunchSize);
                currentOffset += bunchSize;
            }

            if (lastBuffer.HasValue)
            {
                offsetSizeDict.Add(bufferSize - lastBuffer.Value, lastBuffer.Value);
            }

            return offsetSizeDict;
        }
    }
}