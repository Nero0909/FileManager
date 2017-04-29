using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemInterface.IO;
using SystemWrapper.IO;
using FileManager.BL.Interfaces;
using FileManager.BL.Interfaces.Unity;
using FileManager.BL.Interfaces.Workers;
using FileManager.BL.Unity;
using FileManager.BL.Workers;
using Microsoft.Practices.Unity;

namespace FileManager.BL
{
    public class FileManagerBLContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IFileReader, FileReader>();
            Container.RegisterType<IFileWriter, FileWriter>();
            Container.RegisterType<IBytesBuffer, BytesBuffer>();
            Container.RegisterType<IFile, FileWrap>();

            Container.RegisterType(typeof(IFactory<>), typeof(Factory<>));

            Container.RegisterType<IThreadsController, ThreadsController>();
        }
    }
}
