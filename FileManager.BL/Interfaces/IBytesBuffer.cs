using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileManager.BL.Interfaces
{
    public interface IBytesBuffer
    {
        Task<ArraySegment<byte>> GetEmptySegmentAsync(CancellationToken token);

        Task<ArraySegment<byte>> GetFilledSegmentAsync(CancellationToken token);

        Task FreeSegmentAsync(ArraySegment<byte> segment, CancellationToken token);

        Task FillSegmentAsync(ArraySegment<byte> segment, int newSize, CancellationToken token);

        Task<bool> OutputAvailableAsync(CancellationToken cancellationToken);

        void CompleteAdding();

        void Reset(int bufferSize, int bunchSize);
    }
}