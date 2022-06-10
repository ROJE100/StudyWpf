using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class MainViewModel:Conductor<object> //Screen에는 ActivateItem[Async] 메서드가 없음
    {
        public MainViewModel()
        {
            DisplayName = "SmartHome Monitoring v2.0"; // 윈도우 타이틀, 제목 MainView.xaml에서 Title 내용을 삭제
        }

        public void LoadDataBaseView()
        {
            ActivateItemAsync(new DataBaseViewModel());
        }

        public void LoadHistoryView()
        {
            ActivateItemAsync(new HistoryViewModel());
        }
        
        public void LoadRealTimeView()
        {
            ActivateItemAsync(new RealTimeViewModel());
        }


        public void ExitProgram()
        {
            Environment.Exit(0); //오류 없이 프로그램 종료 0 
        }
    }
}
