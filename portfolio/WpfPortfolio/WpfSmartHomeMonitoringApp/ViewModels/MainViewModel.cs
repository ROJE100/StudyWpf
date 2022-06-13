using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class MainViewModel:Conductor<object> //Screen에는 ActivateItem[Async] 메서드가 없음
    {
        public MainViewModel()
        {
            DisplayName = "SmartHome Monitoring v2.0"; // 윈도우 타이틀, 제목 MainView.xaml에서 Title 내용을 삭제
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if(Commons.MQTT_CLIENT.IsConnected)
            {
                Commons.MQTT_CLIENT.Disconnect();
                Commons.MQTT_CLIENT = null;
                //비활성화 처리
            }
            return base.OnDeactivateAsync(close, cancellationToken);
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

        public void ExitToolbar()
        {
            Environment.Exit(0);
        }

        // Start 메뉴, 아이콘 눌렀을때 처리할 이벤트
        public void PopInfoDialog()
        {
            
            TaskPopup();
        }

        public void StartSubscribe()
        {
            TaskPopup();
        }



        private void TaskPopup()
        {
            // CustomPopupView
            var winManager = new WindowManager();
            var result = winManager.ShowDialogAsync(new CustomPopupViewModel("New Broker"));

            if (result.Result == true)
            {
                ActivateItemAsync(new DataBaseViewModel());
            }
        }

        public void PopInfoView()
        {
            var winManager = new WindowManager();
            winManager.ShowDialogAsync(new CustomInfoViewModel("About"));

        }
    }
}
