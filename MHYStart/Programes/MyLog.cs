using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MHYStart.Programes
{
    internal class MyLog
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="type">大类</param>
        /// <param name="message">消息内容</param>
        private static void Log(string type, object? message)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - {type} Message：{message}");
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="type">大类</param>
        /// <param name="tag">标签</param>
        /// <param name="message">消息内容</param>
        private static void Log(string type, string? tag, object? message)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - {type} Tag：{tag} Message：{message}");
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="value">消息内容</param>
        public static void Debug(object? value)
        {
            Log("Debug", value);
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="value">消息内容</param>
        public static void Debug(string tag, object? value)
        {

            Log("Debug", tag, value);
        }


        /// <summary>
        /// 运行日志
        /// </summary>
        /// <param name="value">消息内容</param>
        public static void Information(object? value)
        {
            Log("Information", value);
        }

        /// <summary>
        /// 运行日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="value">消息内容</param>
        public static void Information(string tag, object? value)
        {
            Log("Information", tag, value);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="value">消息内容</param>
        public static void Warning(object? value)
        {
            Log("Warning", value);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="value">消息内容</param>
        public static void Warning(string tag, object? value)
        {
            Log("Warning", tag, value);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="value">消息内容</param>
        public static void Error(object? value)
        {
            Log("Error", value);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="value">消息内容</param>
        public static void Error(string tag, object? value)
        {
            Log("Error", tag, value);
        }

        public static void ShowMessage(string msg, string title = "提示")
        {
            MessageBox.Show(msg, title);
        }
    }
}
