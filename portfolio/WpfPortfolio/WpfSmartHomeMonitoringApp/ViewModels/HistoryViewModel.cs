using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Modles;

namespace WpfSmartHomeMonitoringApp.ViewModels
{

    public class HistoryViewModel:Screen
    {
        private BindableCollection<DivisionModel> divisions;
        private DivisionModel selectedDivision;
        private string startDate;
        private string initStartDate;
        private string endDate;
        private string initEndDate;
        private int totalCount;
        private PlotModel historyModel; // OxyPlot : 220613, SMG. smartHomeModel -> hisyoryModel로 변경
        /*Divisions
         * DivisionVal
         * SelectedDivision
         * StartDate
         * InitStartDate
         * EndDate
         * InitEndDate
         * TotalCount
            SearchIoTData,HistoryModel*/

        public BindableCollection<DivisionModel> Divisions
        {
            get => divisions; set
            {
                divisions = value;
                NotifyOfPropertyChange(() => Divisions);
            }
        }
        public DivisionModel SelectedDivision
        {
            get => selectedDivision; set
            {
                selectedDivision = value;
                NotifyOfPropertyChange(() => SelectedDivision);
            }
        }
        public string StartDate
        {
            get => startDate; set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }
        public string InitStartDate
        {
            get => initStartDate; set
            {
                initStartDate = value;
                NotifyOfPropertyChange(() => InitStartDate);
            }
        }
        public string EndDate
        {
            get => endDate; set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        public string InitEndDate
        {
            get => initEndDate; set
            {
                initEndDate = value;
                NotifyOfPropertyChange(() => InitEndDate);
            }
        }
        public int TotalCount
        {
            get => totalCount; set
            {
                totalCount = value;
                NotifyOfPropertyChange(() => TotalCount);
            }
        }
        public PlotModel HistoryModel //220613, SMG. smartHomeModel -> hisyoryModel로 변경
        {
            get => historyModel; set
            {
                historyModel = value;
                NotifyOfPropertyChange(() => HistoryModel);
        
            }
        }
        public HistoryViewModel()
        {
            Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            InitControl();  
            
        }

        private void InitControl()
        {
            Divisions = new BindableCollection<DivisionModel> //콤보박스용 데이터 생성 
            {
                new DivisionModel {KeyVal=0,DivisionVal="-- Select --"},
                new DivisionModel {KeyVal=1,DivisionVal="DINNING"},
                new DivisionModel {KeyVal=2,DivisionVal="LIVING"},
                new DivisionModel {KeyVal=3,DivisionVal="BED"},
                new DivisionModel {KeyVal=4,DivisionVal="BATH"},
            };
            // Select를 선택해서 초기화
            SelectedDivision = Divisions.Where(v => v.DivisionVal.Contains("Select")).FirstOrDefault();

            InitStartDate = DateTime.Now.ToShortDateString(); // 2022-06-10
            InitEndDate = DateTime.Now.AddDays(1).ToShortDateString();  //2022-06-11
        }

        // 검색 메서드

        public void SearchIoTData()
        {
            // Validation check
            if(SelectedDivision.KeyVal==0) // Select
            {
                MessageBox.Show("검색할 방을 선택하세요.");
                return;
            }      

            if(DateTime.Parse(StartDate)>DateTime.Parse(EndDate))
            {
                MessageBox.Show("시작일이 종료일보다 최신일 수 없습니다.");
                return;
            }

            TotalCount = 0;

            using(SqlConnection conn=new SqlConnection(Commons.CONNSTRING))
            {
                string strQuery = @"SELECT Id,DevId,CurrTime,Temp,Humid
                                    FROM TblSmartHome
                                    WHERE DevId=@DevId
                                    AND CurrTime BETWEEN @StartDate AND @EndDate
                                    ORDER BY Id ASC ";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strQuery, conn);

                    SqlParameter parmDevId = new SqlParameter("@DevId", SelectedDivision.DivisionVal);
                    cmd.Parameters.Add(parmDevId); 
                    SqlParameter parmStartDate = new SqlParameter("@StartDate", StartDate);
                    cmd.Parameters.Add(parmStartDate); 
                    SqlParameter parmEndDate = new SqlParameter("@EndDate", EndDate);
                    cmd.Parameters.Add(parmEndDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    int i = 0;
                    //start of chart process 220613 추가
                    PlotModel tmp = new PlotModel() 
                    {
                        Title = $"{SelectedDivision.DivisionVal}Histories" 
                    }; // 220613 차트처리 임시 플롯모델 >> 다른곳에 전달하기 위함

                    var l = new Legend //범례,OxyPlot.Wpf > LegendsDemo 참조
                    {
                        LegendBorder = OxyColors.Black,
                        LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                        LegendPosition = LegendPosition.RightTop,
                        LegendPlacement = LegendPlacement.Inside
                    };
                    tmp.Legends.Add(l);
                    
                    LineSeries seriesTemp = new LineSeries 
                    {
                        Color= OxyColor.FromRgb(72,159,22), 
                        Title = "Temperature",
                        MarkerSize=4,
                        MarkerType=MarkerType.Circle 
                    }; // 온도 값을 라인차트로 담을 객체

                    LineSeries seriesHumid = new LineSeries
                    {
                        Color = OxyColor.FromRgb(179, 122, 31),
                        Title = "Humidity",
                        MarkerType = MarkerType.Triangle
                    };
                     while(reader.Read())
                    {
                        //var temp=reader["Temp"];
                        //Temp,Humid 차트데이터를 생성
                        var smartHomeModel = new SmartHomeModel();
                        smartHomeModel.DevId = reader["DevId"].ToString();
                        smartHomeModel.CurrTime = DateTime.Parse(reader["currTime"].ToString());
                        smartHomeModel.Temp = Convert.ToDouble(reader["Temp"]);
                        smartHomeModel.Humid = Convert.ToDouble(reader["Humid"]);

                        seriesTemp.Points.Add(new DataPoint(i,Convert.ToDouble(reader["Temp"])));
                        seriesHumid.Points.Add(new DataPoint(i,Convert.ToDouble(reader["Humid"])));
                        i++;
                    }

                    TotalCount = i;   // 검색한 데이터 총 갯수

                    
                    tmp.Series.Add(seriesTemp);
                    tmp.Series.Add(seriesHumid);

                    HistoryModel = tmp;
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Error{ ex.Message}");
                    return;
                }

            }
        }

        
    }
}
