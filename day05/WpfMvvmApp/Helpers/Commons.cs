using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfMvvmApp.Helpers
{
    public class Commons
    {
        // verbatim string $ 보간(+) 문자열, @ 축자(\) 문자열


        /// <summary>
        /// 이메일 검증
        /// </summary>
        /// <param name="email"></param>
        /// <returns>True 이메일형태가 맞음 </returns>
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"[a - zA - Z0 - 9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");     
        }

        /// <summary>
        /// 나이계산
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CalcAge(DateTime value)
        {
            int middle;
            if (DateTime.Now.Month < value.Month || DateTime.Now.Month == value.Month &&
                DateTime.Now.Day < value.Day)
                middle = DateTime.Now.Year - value.Year - 1;
            else
                middle = DateTime.Now.Year - value.Year;
                
            return middle;
        }
    }
}
