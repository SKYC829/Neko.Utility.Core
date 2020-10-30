using Neko.Utility.Core.Configurations;
using System;

namespace Neko.Utility.Core.IO.Logging
{
    /// <summary>
    /// 日志信息实体类
    /// </summary>
    [Serializable]
    internal class LogInfo
    {
        /// <summary>
        /// 记录日志的时间
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 记录日志的信息
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// 记录的异常信息
        /// </summary>
        public Exception InnerException { get; set; }

        /// <summary>
        /// 短时间内同一日志出现的次数
        /// </summary>
        public int LogCount { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel LogLevel { get; set; }

        public LogInfo(LogLevel logLevel, DateTime logTime, string logMessage) : this(logLevel, logTime, logMessage, null)
        {

        }

        public LogInfo(LogLevel logLevel, DateTime logTime, Exception innerException) : this(logLevel, logTime, innerException.Message, innerException)
        {

        }

        public LogInfo(LogLevel logLevel, DateTime logTime, string logMessage, Exception innerException)
        {
            LogLevel = logLevel;
            LogTime = logTime;
            LogMessage = logMessage;
            InnerException = innerException;
        }
    }
}
