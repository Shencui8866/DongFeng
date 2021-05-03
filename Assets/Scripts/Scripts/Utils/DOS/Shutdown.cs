using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Threading;
using System;

namespace Josing.Utils
{
    public static class Shutdown
    {
        /// <summary>
        /// 重启
        /// </summary>
        /// <param name="time"></param>
        public static void Restart(int time = 0)
        {
            //重启计算机
            Process.Start("shutdown", @"-r -t " + time);
        }
        /// <summary>
        /// 注销
        /// </summary>
        public static void Logout()
        {
            //注销计算机
            Process.Start("shutdown", @"/l");
        }

        /// <summary>
        /// 关机
        /// </summary>
        /// <param name="time"></param>
        public static void TurnOff(int time = 0)
        {
            //关机
            Process.Start("shutdown", @"-s -t " + time);
        }

        public static string RunCMD(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";  //确定程序名
            p.StartInfo.Arguments = @"C:\Users\admin>" + command;  //指定程式命令行
            p.StartInfo.UseShellExecute = false;   //是否使用Shell
            p.StartInfo.RedirectStandardInput = true;   //重定向输入
            p.StartInfo.RedirectStandardOutput = true;   //重定向输出
            p.StartInfo.RedirectStandardError = true;    //重定向输出错误
            p.StartInfo.CreateNoWindow = false;        //设置不显示窗口
            p.Start();
            return p.StandardOutput.ReadToEnd();     //输出流取得命令行结果
        }
    }
}
