using Neko.Utility.Core.Configurations;
using Neko.Utility.Core.Data;
using Neko.Utility.Core.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

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
        /// 记录Windows系统日志
        /// </summary>
        private static EventLog _log;

        /// <summary>
        /// 正在输出日志委托方法
        /// </summary>
        public static ParameterDelegateCode OnWriteLog { get; set; }

        /// <summary>
        /// 是否允许输出到Windows的系统日志
        /// </summary>
        public static bool CanUseEventLog { get { return RuntimeInformation.IsOSPlatform(OSPlatform.Windows); } }

        static LogUtil()
        {
            _logCache = new Dictionary<string, LogInfo>();
            _logQueue = new Queue<LogInfo>();
            StartLog();
        }

        /// <summary>
        /// 开启线程输出日志到文件
        /// </summary>
        private static void StartLog()
        {
            if (CanUseEventLog)
            {
                if (!EventLog.SourceExists("Neko.Utility"))
                {
                    try
                    {
                        EventLog.CreateEventSource("Neko.Utility", AppDomain.CurrentDomain.FriendlyName);
                    }
                    catch (SecurityException)
                    {
                        throw;
                    }
                }
                _log = new EventLog(AppDomain.CurrentDomain.FriendlyName);
                _log.Source = "Neko.Utility";
            }
            ThreadUtil.RunLoop(new IntervalInfo()
            {
                Interval = 100,
                ExecuteCode = WriteLogToFile
            }).Name = "Output log thread";
        }

        /// <summary>
        /// 记录<inheritdoc cref="LogLevel.Information"/>
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public static void WriteInformation(string logMessage, params object[] messageParameters)
        {
            WriteLog(LogLevel.Information, string.Format(logMessage, messageParameters));
        }

        /// <summary>
        /// 记录<inheritdoc cref="LogLevel.Warning"/>
        /// </summary>
        /// <param name="warningException">可能会发生的异常</param>
        /// <param name="logMessage">日志信息</param>
        /// <param name="messageParameters">日志信息的参数</param>
        public static void WriteWarning(Exception warningException, string logMessage, params object[] messageParameters)
        {
            WriteLog(LogLevel.Warning, string.Format(logMessage, messageParameters), warningException);
        }

        /// <summary>
        /// 记录<inheritdoc cref="LogLevel.Exception"/>
        /// </summary>
        /// <param name="innerException">异常信息</param>
        /// <param name="caption">错误提示</param>
        public static void WriteException(Exception innerException, string caption = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = innerException.Message;
            }
            WriteLog(LogLevel.Exception, caption, innerException);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logLevel">日志类型</param>
        /// <param name="logMessage">日志信息</param>
        /// <param name="innerException">异常信息</param>
        public static void WriteLog(LogLevel logLevel, string logMessage = null, Exception innerException = null)
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

        private static void WriteLogToFile()
        {
            _logConfiguration = LogConfiguration.GetConfiguration();
            if (_logQueue.Count < 1)
            {
                return;
            }
            StreamWriter writer = null;
            if (!_logConfiguration.NoLocalFile)
            {
                string logFileFullName = string.Format("{0}/{1}", _logConfiguration.LogPath, _logConfiguration.LogFileName);
                string logPath = Path.GetDirectoryName(logFileFullName);
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                writer = new StreamWriter(logFileFullName, true, Encoding.Default);
            }
            do
            {
                try
                {
                    if (_logQueue.Count == 0)
                    {
                        break;
                    }
                    LogInfo logInfo = null;
                    lock (_logQueue)
                    {
                        logInfo = _logQueue.Dequeue();
                    }
                    if (logInfo == null)
                    {
                        continue;
                    }
                    if (logInfo.LogLevel < _logConfiguration.LogLevel)
                    {
                        continue;
                    }
                    if (logInfo.InnerException != null)
                    {
                        if (string.IsNullOrEmpty(logInfo.LogMessage))
                        {
                            logInfo.LogMessage = string.Format("{0}\r\n{1}", logInfo.InnerException.Message, logInfo.InnerException.StackTrace);
                        }
                        else
                        {
                            logInfo.LogMessage = string.Format("{0}\r\n错误信息:{1}\r\n异常堆栈:{2}", logInfo.LogMessage, logInfo.InnerException.Message, logInfo.InnerException.StackTrace);
                        }
                        string cacheKey = EncryptionUtil.EncryptMD5(logInfo.LogMessage);
                        LogInfo cacheInfo = DictionaryUtil.Get<LogInfo>(_logCache, cacheKey);
                        if (cacheInfo == null)
                        {
                            _logCache[cacheKey] = cacheInfo;
                        }
                        else
                        {
                            cacheInfo.LogCount++;
                            if ((logInfo.LogTime - cacheInfo.LogTime).TotalMinutes < 5)
                            {
                                continue;
                            }
                            logInfo.LogMessage = string.Format("{0}(5分钟内触发了{1}次)", logInfo.LogMessage, cacheInfo.LogCount);
                            cacheInfo.LogCount = 0;
                            cacheInfo.LogTime = logInfo.LogTime;
                        }
                    }
                    if (string.IsNullOrEmpty(logInfo.LogMessage))
                    {
                        continue;
                    }
                    string logMessage = string.Format("[{0:HH:mm:ss}][{1}]:\r\n{2}", logInfo.LogTime, logInfo.LogLevel, logInfo.LogMessage);
                    if (_logConfiguration.AddConsole)
                    {
                        Console.WriteLine(logMessage);
                    }
                    if (_logConfiguration.AddDebug)
                    {
                        Debug.Print(logMessage);
                    }
                    if (writer != null)
                    {
                        writer.WriteLine(logMessage);
                    }
                    if (CanUseEventLog && _logConfiguration.WriteToEventLog)
                    {
                        EventLogEntryType logType = EventLogEntryType.Information;
                        switch (logInfo.LogLevel)
                        {
                            case LogLevel.Warning:
                                logType = EventLogEntryType.Warning;
                                break;
                            case LogLevel.Exception:
                                logType = EventLogEntryType.Error;
                                break;
                            case LogLevel.Information:
                            default:
                                logType = EventLogEntryType.Information;
                                break;
                        }
                        if (logInfo.LogLevel != LogLevel.Track)
                        {
                            _log.WriteEntry(logMessage, logType);
                        }
                    }
                    OnWriteLog?.Invoke(logInfo.LogLevel, logMessage);
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
                finally
                {
                    if (writer != null && _logQueue.Count == 0)
                    {
                        Thread.Sleep(100);
                    }
                }
            } while (true);
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
                writer = null;
            }
        }
    }
}
