﻿using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class DataBaseViewModel:Screen
    {
        
        private string brokerUrl;
        public string BrokerUrl
        {
            get { return brokerUrl; }
            set
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);
            }
        }

        private string topic;
        public string Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }

        private string connString;
        public string ConnString
        {
            get { return connString; }
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }

        private string dbLog;
        public string DbLog
        {
            get { return dbLog; }
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }
        
        private bool isConnected;
       
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }


        public DataBaseViewModel()
        {
            BrokerUrl=Commons.BROKERHOST = "localhost"; //MQRR Broker IP 설정
            Topic = Commons.PUB_TOPIC = "home/device/#";    //Multiple Topic
            //Single Level wildcard +
            //Multi Level wildcard #
            ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            
            if(Commons.IS_CONNECT)
            {
                IsConnected = true;
                ConnectDb();
            }

        }

        /// <summary>
        /// DB연결+MQTT Broker 접속
        /// </summary>
        public void ConnectDb()
        {
            if(IsConnected)
            {
                Commons.MQTT_CLIENT = new MqttClient(BrokerUrl);

                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected != true)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");
                        Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                           new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        UpdateText(">>> MQTT Broker Connected");
                        IsConnected= Commons.IS_CONNECT = true;

                    }
                }
                catch (Exception)
                {

                   //pass
                }
            }
            else //접속끄기
            {
                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived; //이벤트 연결 삭제
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateText(">>> MQTT Broker Disconnected...");
                        IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception )
                {

                    // pass
                }
            }
        }
        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }


        #region Encoding UTF8

        //모든 나라의 언어를 표현하기 위해 사용

        #endregion
        /// <summary>
        /// Subscribe한 메시지 처리해주는 이벤트핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            UpdateText(message);  //센서데이터 출력
            SetDataBase(message/*,e.Topic*/); //DB에 저장
        }

        private void SetDataBase(string message/*,string topic*/)
        {
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message); //Dictionary<key,
            //

            Debug.WriteLine(currDatas);

            using(SqlConnection conn =new SqlConnection(ConnString))
            {
                conn.Open();
                string strInQuery = @"INSERT INTO TblSmartHome
                                               (DevId
                                               , CurrTime
                                               , Temp
                                               , Humid)
                                         VALUES
                                               (@DevId
                                               , @CurrTime
                                               , @Temp
                                               , @Humid)";

                try
                {
                    SqlCommand cmd = new SqlCommand(strInQuery, conn);
                    SqlParameter parmDevId = new SqlParameter("@DEvId", currDatas["DevId"]);
                    cmd.Parameters.Add(parmDevId);
                    SqlParameter parmCurrTime = new SqlParameter("@CurrTime", DateTime.Parse(currDatas["CurrTime"]));
                    //날짜형으로 변환필요!
                    cmd.Parameters.Add(parmCurrTime);
                    SqlParameter parmTemp = new SqlParameter("@Temp", currDatas["Temp"]);
                    cmd.Parameters.Add(parmTemp);
                    SqlParameter parmHumid = new SqlParameter("@Humid", currDatas["Humid"]);
                    cmd.Parameters.Add(parmHumid);

                    if (cmd.ExecuteNonQuery() == 1)
                        UpdateText(">>> DB Inserted.");     //저장성공
                    else
                        UpdateText(">>> DB Failed!!!!!");   //저장실패
                }
                catch (Exception ex)
                {

                    UpdateText($">>> DB Error! {ex.Message}"); //예외
                }

            }// using문 사용이유 conn.Close() 불필요
        }
    }
}