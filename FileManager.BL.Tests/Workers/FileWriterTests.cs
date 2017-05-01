using System.Threading;
using System.Threading.Tasks;
using SystemInterface.IO;
using FileManager.BL.Interfaces;
using FileManager.BL.Interfaces.Workers;
using FileManager.BL.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileManager.BL.Tests.Workers
{
    [TestClass]
    public class FileWriterTests : SuspendableWorkerTests
    {
        private Mock<IFileStream> _streamMock;

        [SetUp]
        public override void SetUp()
        {base.SetUp();
            _streamMock = new Mock<IFileStream>();
            FileMock.Setup(x => x.OpenWrite(It.IsAny<string>())).Returns(_streamMock.Object);
        }


        public override ISuspendableWorker CreateInstance()
        {
            return new FileReader(BufferMock.Object, FileMock.Object, CancelTokenSource);
        }
    }
}