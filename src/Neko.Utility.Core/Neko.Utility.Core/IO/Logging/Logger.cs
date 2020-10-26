using Neko.Utility.Core.Common;
using Neko.Utility.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neko.Utility.Core.IO.Logging
{
    /// <summary>
    /// 记录日志类
    /// <para>注意:此日志类默认的记录日志文件路径与<see cref="Neko.Logging.Core"/>
    /// 的日志文件相冲突,<br/>建议不要一起使用以避免不必要的麻烦
    /// </para>
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 日志配置信息
        /// </summary>
        private readonly LogConfiguration _logConfiguration;

        /// <summary>
        /// 获取日志记录的时间的委托方法
        /// </summary>
        private static EmptyDelegateCode<DateTime> _getNow;

        /// <summary>
        /// 保存开始记录日志的时间
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// 保存结束记录日志的时间
        /// </summary>
        private DateTime _endTime;

        /// <summary>
        /// 日志记录周期耗时(单位:毫秒)
        /// <para>开始调用这个类直到下次调用<see cref="Commit"/>的时间</para>
        /// </summary>
        private double _timeConsuming;

        /// <summary>
        /// 日志记录总耗时(单位:毫秒)
        /// <para>开始调用这个类直到下次调用<see cref="WriteLog"/>的时间</para>
        /// </summary>
        private double _totalTimeConsuming;

        /// <summary>
        /// 日志内容
        /// </summary>
        private StringBuilder _logMessage;

        /// <summary>
        /// 日志队列
        /// </summary>
        private Queue<LogInfo> _logQueue;

        static Logger()
        {
            _getNow = delegate ()
            {
                return DateTime.Now;
            };
        }

        public Logger() : this(string.Empty)
        {

        }

        public Logger(string logTitle) : this(LogConfiguration.GetConfiguration(), logTitle)
        {

        }

        public Logger(LogConfiguration logConfiguration, string logTitle = null)
        {
            _logConfiguration = logConfiguration;
            Reset();
            Begin(logTitle);
        }

        /// <summary>
        /// 重置日志信息
        /// </summary>
        private void Reset()
        {
            _startTime = _getNow();
            _endTime = _getNow();
            _timeConsuming = 0d;
            _totalTimeConsuming = 0d;
            _logMessage = new StringBuilder();
            _logQueue = new Queue<LogInfo>();
        }

        /// <summary>
        /// 开始记录日志,初始化<see cref="_startTime"/>
        /// </summary>
        /// <param name="logTitle">日志标题</param>
        private void Begin(string logTitle)
        {
            _startTime = _getNow();
            if (!string.IsNullOrEmpty(logTitle))
            {
                _logMessage.Append(string.Format("-----------------------------{0}-----------------------------", logTitle));
            }
        }

        /// <summary>
        /// 提交日志信息
        /// <para>如果要输出日志,请使用<see cref="WriteLog"/>方法</para>
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public void Commit(LogLevel logLevel, string logMessage, params object[] messageParameters)
        {
            string message = logMessage;
            try
            {
                message = string.Format(message, messageParameters);
            }
            finally
            {
                Commit(logLevel, message);
            }
        }

        /// <summary>
        /// 提交日志信息
        /// <para>如果要输出日志,请使用<see cref="WriteLog"/>方法</para>
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <param name="logMessage">日志信息</param>
        internal void Commit(LogLevel logLevel, string logMessage)
        {
            Commit();
            if (!string.IsNullOrEmpty(logMessage))
            {
                _logMessage.AppendFormat("{0}.(耗时:{1:F2}毫秒.总耗时:{2:F2}毫秒).", logMessage, _timeConsuming, _totalTimeConsuming);
            }
            lock (_logQueue)
            {
                LogInfo logInfo = new LogInfo(logLevel, DateTime.Now, _logMessage.ToString());
                _logQueue.Enqueue(logInfo);
                _logMessage = new StringBuilder();
            }
        }

        /// <summary>
        /// 提交日志信息
        /// <para>如果要输出日志,请使用<see cref="WriteLog"/>方法</para>
        /// </summary>
        public void Commit()
        {
            _endTime = _getNow();
            double timeInterval = (_endTime - _startTime).TotalMilliseconds;
            _timeConsuming = timeInterval - _totalTimeConsuming;
            _totalTimeConsuming = timeInterval;
        }

        /// <summary>
        /// 提交<inheritdoc cref="LogLevel.Track"/>
        /// <para>如果要输出日志,请使用<see cref="WriteLog"/>方法</para>
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public void CommitTrack(string logMessage, params object[] messageParameters)
        {
            Commit(LogLevel.Track, logMessage, messageParameters);
        }

        /// <summary>
        /// 提交<inheritdoc cref="LogLevel.Information"/>
        /// <para>如果要输出日志,请使用<see cref="WriteLog"/>方法</para>
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public void CommitInformation(string logMessage, params object[] messageParameters)
        {
            Commit(LogLevel.Information, logMessage, messageParameters);
        }

        /// <summary>
        /// 提交<inheritdoc cref="LogLevel.Warning"/>
        /// <para>如果要输出日志,请使用<see cref="WriteLog"/>方法</para>
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public void CommitWarning(string logMessage, params object[] messageParameters)
        {
            Commit(LogLevel.Warning, logMessage, messageParameters);
        }

        /// <summary>
        /// 提交<inheritdoc cref="LogLevel.Exception"/>
        /// <para>如果要输出日志,请使用<see cref="WriteLog"/>方法</para>
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public void CommitException(string logMessage, params object[] messageParameters)
        {
            Commit(LogLevel.Exception, logMessage, messageParameters);
        }

        /// <summary>
        /// 输出日志到日志文件
        /// </summary>
        public void WriteLog()                                                                                
        {
            if (_totalTimeConsuming < _logConfiguration.RecordMinimumInterval)
            {
                return;
            }
            try
            {
                do
                {
                    LogInfo logInfo = _logQueue.Dequeue();
                    LogUtil.WriteLog(logInfo);
                } while (_logQueue.Count > 0);
            }
            catch (Exception ex)
            {
                //TODO:
            }
            finally
            {
                Reset();
            }
        }
    }
}
