using System;
using System.Collections.Generic;
using System.Linq;
using FileManager.BL;
using Microsoft.Practices.Unity;

namespace FileManager.Client
{
    internal sealed class Bootstrapper
    {
        public IUnityContainer Container { get; set; }

        public Bootstrapper()
        {
            Container = new UnityContainer();
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            Container.AddNewExtension<FileManagerBLContainerExtension>();
            Container.AddNewExtension<FileManagerClientContainerExtension>();
        }
    }
}
