using System.ComponentModel;
using FileManager.Client.Interfaces.Services;
using FileManager.Client.Interfaces.ViewModel;
using FileManager.Client.Services;
using FileManager.Client.ViewModel;
using Microsoft.Practices.Unity;

namespace FileManager.Client
{
    internal sealed class FileManagerClientContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<MainViewModel>();

            Container.RegisterType<IMessageBoxService, MessageBoxService>();
            Container.RegisterType<IFileSystemDialogService, FileSystemDialogService>();

            Container.RegisterType<IConfigurationViewModel, ConfigurationViewModel>();
            Container.RegisterType<IManagementViewModel, ManagementViewModel>();
        }
    }
}
