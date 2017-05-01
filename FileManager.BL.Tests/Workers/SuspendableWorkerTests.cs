

using System;
using System.Threading;
using SystemInterface.IO;
using FileManager.BL.Interfaces;
using FileManager.BL.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileManager.BL.Tests.Workers
{
    [TestClass]
    public abstract class SuspendableWorkerTests
    {
        protected string FilePath = "testPath";
        protected Mock<IBytesBuffer> BufferMock;
        protected Mock<IFile> FileMock;
        protected CancellationTokenSource CancelTokenSource;

        [SetUp]
        public virtual void SetUp()
        {
            BufferMock = new Mock<IBytesBuffer>();
            FileMock = new Mock<IFile>();
            CancelTokenSource = new CancellationTokenSource();
        }

        [Test]
        public void ShouldCreateInstance()
        {
            Should.NotThrow(() => CreateInstance());
        }

        [Test]
        public void ShouldPause()
        {
            // Given
            var currentState = WorkerState.Unstarted;
            var expectedState = WorkerState.Suspended;
            var instance = CreateInstance();
            instance.CurrentState.Subscribe(x => currentState = x);

            // When
            instance.Pause();

            // Then
            currentState.ShouldBe(expectedState);
        }

        [Test]
        public void ShouldResume()
        {
            // Given
            var currentState = WorkerState.Unstarted;
            var expectedState = WorkerState.Running;
            var instance = CreateInstance();
            instance.CurrentState.Subscribe(x => currentState = x);

            // When
            instance.Pause();
            instance.Resume();

            // Then
            currentState.ShouldBe(expectedState);
        }

        [Test]
        public void ShouldCancel()
        {
            // Given
            var currentState = WorkerState.Unstarted;
            var expectedState = WorkerState.Stopped;
            var instance = CreateInstance();
            instance.CurrentState.Subscribe(x => currentState = x);

            // When
            instance.Cancel();

            // Then
            currentState.ShouldBe(expectedState);
        }

        public abstract ISuspendableWorker CreateInstance();
    }
}
