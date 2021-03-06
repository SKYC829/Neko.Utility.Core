﻿using System;

namespace Neko.Utility.Core.Configurations
{
    /// <summary>
    /// 日志的配置信息
    /// </summary>
    [Serializable]
    public class LogConfiguration
    {
        /// <summary>
        /// 配置信息的单例
        /// </summary>
        public static LogConfiguration Instance { get; private set; }

        /// <summary>
        /// 记录日志最小间隔(单位毫秒)
        /// </summary>
        public int RecordMinimumInterval { get; set; }

        /// <summary>
        /// 记录日志的路径
        /// </summary>
        public string LogPath { get; set; }

        /// <summary>
        /// 记录日志文件的文件名
        /// </summary>
        public string LogFileName { get; set; }

        /// <summary>
        /// 记录日志的等级
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 是否输出到控制台
        /// </summary>
        public bool AddConsole { get; set; }

        /// <summary>
        /// 是否输出到输出窗口<see cref="System.Diagnostics.Debug.Print"/>
        /// </summary>
        public bool AddDebug { get; set; }

        /// <summary>
        /// 是否输出到Windows系统日志
        /// <para>只在Windows系统下生效</para>
        /// </summary>
        public bool WriteToEventLog { get; set; }

        /// <summary>
        /// 是否不生成日志文件
        /// <para>如果只希望输出到Windows系统日志的话</para>
        /// </summary>
        public bool NoLocalFile { get; set; }

        private LogConfiguration()
        {
            RecordMinimumInterval = 1;
            LogPath = "Temp/Log";
            LogFileName = string.Format("{0:yyyyMMdd}.log", DateTime.Today);
            LogLevel = LogLevel.Information;
            WriteToEventLog = false;
            NoLocalFile = false;
        }

        /// <summary>
        /// 获取配置信息的单例对象
        /// </summary>
        /// <returns></returns>
        public static LogConfiguration GetConfiguration()
        {
            if (Instance == null)
            {
                Instance = new LogConfiguration();
            }
            return Instance;
        }
    }
}
