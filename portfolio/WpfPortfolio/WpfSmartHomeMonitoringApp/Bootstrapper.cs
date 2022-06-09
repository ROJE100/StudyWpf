using Caliburn.Micro;
using System.Windows;
using WpfSmartHomeMonitoringApp.ViewModels;

namespace WpfSmartHomeMonitoringApp
{
    public class Bootstrapper:BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // base.OnStartup(sender, e);
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
