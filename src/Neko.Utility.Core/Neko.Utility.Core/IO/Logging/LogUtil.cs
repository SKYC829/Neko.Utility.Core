using Neko.Utility.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neko.Utility.Core.IO.Logging
{
    /// <summary>
    /// 日志帮助类
    /// <para>输出日志信息到文件</para>
    /// </summary>
    public sealed class LogUtil
    {
        /// <summary>
        /// 历史日志缓存
        /// </summary>
        private static Dictionary<string, LogInfo> _logCache;

        /// <summary>
        /// 输出日志队列
        /// </summary>
        private static Queue<LogInfo> _logQueue;

        /// <summary>
        /// 日志配置信息
        /// </summary>
        private static LogConfiguration _logConfiguration;

        /// <summary>
        /// 正在输出日志委托方法
        /// </summary>
        public static ParameterDelegateCode OnWriteLog { get; set; }

        static LogUtil()
        {
            _logCache = new Dictionary<string, LogInfo>();
            _logQueue = new Queue<LogInfo>();
        }

        /// <summary>
        /// 记录<inheritdoc cref="LogLevel.Information"/>
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public static void WriteInformation(string logMessage,params object[] messageParameters)
        {
            WriteLog(LogLevel.Information, string.Format(logMessage, messageParameters));
        }

        /// <summary>
        /// 记录<inheritdoc cref="LogLevel.Warning"/>
        /// </summary>
        /// <param name="warningException">可能会发生的异常</param>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public static void WriteWarning(Exception warningException,string logMessage,params object[] messageParameters)
        {
            WriteLog(LogLevel.Warning, string.Format(logMessage, messageParameters), warningException);
        }

        /// <summary>
        /// 记录<inheritdoc cref="LogLevel.Exception"/>
        /// </summary>
        /// <param name="innerException">异常信息</param>
        /// <param name="caption">错误提示</param>
        public static void WriteException(Exception innerException,string caption)
        {
            WriteLog(LogLevel.Exception, caption, innerException);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logLevel">日志类型</param>
        /// <param name="logMessage">日志信息</param>
        /// <param name="innerException">异常信息</param>
        public static void WriteLog(LogLevel logLevel,string logMessage = null,Exception innerException = null)
        {
            LogInfo logInfo = new LogInfo(logLevel, DateTime.Now, logMessage, innerException);
            WriteLog(logInfo);
        }

        /// <summary>
        /// 添加日志到输出日志队列
        /// </summary>
        /// <param name="logInfo">日志信息实体类</param>
        internal static void WriteLog(LogInfo logInfo)
        {
            Enqueue(logInfo);
        }

        /// <summary>
        /// 添加日志到输出日志队列
        /// </summary>
        /// <param name="logInfo">日志信息实体类</param>
        private static void Enqueue(LogInfo logInfo)
        {
            lock (_logQueue)
            {
                _logQueue.Enqueue(logInfo);
            }
        }
    }
}
