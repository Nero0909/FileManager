using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.Testing;
using NUnit.Framework;
using Shouldly;

namespace FileManager.BL.Tests
{
    [TestClass]
    public class BytesBufferTests
    {
        private const int PageSize = 4 * 1024;
        private readonly CancellationToken _token = new CancellationToken();

        [Test]
        public async Task ShouldGetCorrectFreeSegments()
        {
            // Given
            var bufferSize = 10000;
            var lastSegmentCount = bufferSize - PageSize * 2;
            var buffer = new BytesBuffer(bufferSize);

            // When
            var segment1 = await buffer.GetEmptySegmentAsync(_token);
            var segment2 = await buffer.GetEmptySegmentAsync(_token);
            var segment3 = await buffer.GetEmptySegmentAsync(_token);

            // Then
            segment1.Count.ShouldBe(PageSize);
            segment1.Offset.ShouldBe(0);
            segment2.Count.ShouldBe(PageSize);
            segment2.Offset.ShouldBe(PageSize);
            segment3.Count.ShouldBe(lastSegmentCount);
            segment3.Offset.ShouldBe(PageSize * 2);
        }
        [Test]
        public async Task ShouldGetCorrectOneFreeSegment()
        {
            // Given
            var bufferSize = PageSize / 2;
            var buffer = new BytesBuffer(bufferSize);

            // When
            var segment1 = await buffer.GetEmptySegmentAsync(_token);

            // Then
            segment1.Count.ShouldBe(bufferSize);
            segment1.Offset.ShouldBe(0);
        }

        [Test]
        public async Task ShouldWaitForFreeSection()
        {
            // Given
            var buffer = new BytesBuffer(PageSize);

            // When
            await buffer.GetEmptySegmentAsync(_token);
            var task = buffer.GetEmptySegmentAsync(_token);

            // Then
            await AsyncAssert.NeverCompletesAsync(task);
        }

        [Test]
        public async Task ShouldWaitForFilledSectionInEmptyBuffer()
        {
            // Given
            var buffer = new BytesBuffer(PageSize);

            // When
            var task = buffer.GetFilledSegmentAsync(_token);

            // Then
            await AsyncAssert.NeverCompletesAsync(task);
        }

        [Test]
        public async Task ShouldChangeSegmentSize()
        {
            // Given
            var buffer = new BytesBuffer(PageSize);
            var newSize = 100;

            // When
            var segment = await buffer.GetEmptySegmentAsync(_token);
            await buffer.FillSegmentAsync(segment, newSize, _token);
            var filledSegment = await buffer.GetFilledSegmentAsync(_token);

            // Then
            filledSegment.Count.ShouldBe(newSize);
        }

        [Test]
        public async Task ShouldWaitForFilledSegment()
        {
            // Given
            var buffer = new BytesBuffer(PageSize);

            // When
            var segment = await buffer.GetEmptySegmentAsync(_token);
            await buffer.FillSegmentAsync(segment, segment.Count, _token);
            var filledSegment = await buffer.GetFilledSegmentAsync(_token);
            var task = buffer.GetFilledSegmentAsync(_token);

            // Then
            await AsyncAssert.NeverCompletesAsync(task);}

    }
}