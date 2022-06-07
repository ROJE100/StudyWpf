using Caliburn.Micro;
using System.Windows;
using WpfCaliburnApp.ViewModels;

namespace WpfCaliburnApp
{
    /// <summary>
    /// 시작 윈도우 지정 클래스!!!
    /// </summary>
    public class Bootstrapper:BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();    //Bootstrapper를 초기화 해주는 메서드
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(sender, e);
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
