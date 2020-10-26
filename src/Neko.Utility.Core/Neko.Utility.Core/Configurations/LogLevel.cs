using System;

namespace Neko.Utility.Core.Configurations
{
    /// <summary>
    /// 日志等级
    /// </summary>
    [Serializable]
    public enum LogLevel
    {
        /// <summary>
        /// 步骤日志,一般用于开发
        /// </summary>
        Track,
        /// <summary>
        /// 普通信息日志
        /// </summary>
        Information,
        /// <summary>
        /// 警告信息日志
        /// </summary>
        Warning,
        /// <summary>
        /// 异常信息日志
        /// </summary>
        Exception
    }
}
