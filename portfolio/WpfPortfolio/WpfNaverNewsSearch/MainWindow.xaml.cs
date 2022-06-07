using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfNaverNewsSearch;

namespace WpfNaverNewsSearch
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                /*Commons.ShowMessageAsync("실행", "뉴스검색 실행!");*/
                SearchNaverNews();

            }
        }

        private void SearchNaverNews()
        {
            string keyword = txtSearch.Text;
            string ClientID = "96URoqPWQtYJJ5LqqJUz";
            string ClientSecret = "rw9UXGLcUT";
            string openApiUri = $"https://openapi.naver.com/v1/search/news.json?start=1&display=10&query= {keyword}";
            string result;
            //{ txtStartNum.Text}

            WebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;

            // Naver OpenAPI 실제 요청
            try
            {
                request = WebRequest.Create(openApiUri);
                request.Headers.Add("X-Naver-Client-Id", ClientID); //중요! 네이버에서 데이터를 받아올때 정해진 양식
                request.Headers.Add("X-Naver-Client-Secret", ClientSecret); //중요!

                response = request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);

                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                reader.Close();
                stream.Close();
                response.Close();

            }

            //MessageBox.Show(result);
            var parsedJson = JObject.Parse(result); // string to json

            int total = Convert.ToInt32(parsedJson["total"]);   //전체 검색결과수
            int display = Convert.ToInt32(parsedJson["display"]); //10

            var items = parsedJson["items"];
            var json_array = (JArray)items;         // json_array를 (JArray)items로 형변환
            
            
            List<NewsItem> newsItems = new List<NewsItem>();        // 데이터그리드와 연동

            
            foreach (var item in json_array)
            {
                var temp = DateTime.Parse(item["pubDate"].ToString());
                NewsItem news = new NewsItem()
                {
                    Title = item["title"].ToString(),
                    OriginalLink = item["originallink"].ToString(),
                    Link = item["link"].ToString(),
                    Description = item["description"].ToString(),
                    PubDate = temp.ToString("yyyy-MM-dd HH:mm"),

                };
                
                newsItems.Add(news);

            }

            this.DataContext = newsItems;
        }

        private void dgrResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dgrResult.SelectedItem == null) return; //두번째 검색부터 오류를 제거
            string link = (dgrResult.SelectedItem as NewsItem).Link;
            Process.Start(link);
        }
    }

    public class NewsItem       // 클래스는 명사와 동사의 집합
    {
        public string Title { get; set; }
        public string OriginalLink { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
    }
}
