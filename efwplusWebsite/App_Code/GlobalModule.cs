using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace efwplusWebsite.App_Code
{
    public class GlobalModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        System.Timers.Timer timer;

        void context_BeginRequest(object sender, EventArgs e)
        {
            //初始化
            //HttpContext.Current.Response.Write("BeginRequest");
            timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;//执行间隔时间,单位为毫秒  
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            // 定制时间； 比如 在8：00 ：00 的时候执行某个函数  
            int iHour = 8;
            int iMinute = 00;
            int iSecond = 00;

            // 设置　每天的8：00 ：00开始执行程序  
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {
                //定时调用微信接口群发消息
                wxhandler.SendAll();
            }
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}