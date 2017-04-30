using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemInterface.IO;
using SystemWrapper.IO;
using FileManager.BL.Interfaces;
using FileManager.BL.Interfaces.Reactive;
using FileManager.BL.Interfaces.Unity;
using FileManager.BL.Interfaces.Workers;
using FileManager.BL.Reactive;
using FileManager.BL.Unity;
using FileManager.BL.Workers;
using Microsoft.Practices.Unity;

namespace FileManager.BL
{
    public class FileManagerBLContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IDispatcherProvider, CurrentDispatcherProvider>(Dispatchers.Current, new ContainerControlledLifetimeManager());
            Container.RegisterType<IDispatcherProvider, NewThreadDispatcherProvider>(Dispatchers.NewThread, new ContainerControlledLifetimeManager());

            Container.RegisterType<IFileReader, FileReader>();
            Container.RegisterType<IFileWriter, FileWriter>();
            Container.RegisterType<IBytesBuffer, BytesBuffer>();
            Container.RegisterType<IFile, FileWrap>();
            Container.RegisterType<IDirectory, DirectoryWrap>();

            Container.RegisterType(typeof(IFactory<>), typeof(Factory<>));

            Container.RegisterType<IThreadsController, ThreadsController>();
        }
    }
}
