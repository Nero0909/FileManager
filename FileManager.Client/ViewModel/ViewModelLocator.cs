using Microsoft.Practices.ServiceLocation;
namespace FileManager.Client.ViewModel
{
    public class ViewModelLocator
    {
        private static readonly Bootstrapper BootStrapper;

        static ViewModelLocator()
        {
            if (BootStrapper == null)
            {
                BootStrapper = new Bootstrapper();
            }

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(BootStrapper.Container));
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}