using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CnsLogTestApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region Log
            //var i = 0;
            //while (true)
            //{
            //    i++;
            //    Console.WriteLine($"Hellow world:{i}");
            //    Debug.WriteLine($"Hellow world:{i}");   //디버그시 출력창에 콘솔값이랑 똑같은 값이 기록
            //                                            // 문제점이 눈에 바로 띄기 때문에 사용
            //    Thread.Sleep(2000); // 2초에 한번씩 출력
            //}
            #endregion

            

            #region SQL 연결
            Commons.LOGGER.Info("DataBase 접속 시도");
            string connString = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            string strQuery = @"SELECT Id
                                ,EmpName
                                ,Salary
                                ,DeptName
                                ,Destination
                                 FROM TblEmployees";

            Commons.LOGGER.Info("DataBase 설정 및 쿼리 작성");

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strQuery, conn);
                    Commons.LOGGER.Warn("접속 실패가 발생할 수 있습니다. 주의하세요");

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                        Console.WriteLine(reader["EmpName"]);
                    }
                }
                Commons.LOGGER.Info("DB 처리 완료!");
            }
            catch (Exception ex)
            {

                Commons.LOGGER.Error($"예외발생! :{ex}");
                Console.WriteLine("예외발생! 관리자에게 문의하세요!!");
            }
            Commons.LOGGER.Info("DataBase 접속 종료!!!!!");
           
            #endregion
        }
    }
}
