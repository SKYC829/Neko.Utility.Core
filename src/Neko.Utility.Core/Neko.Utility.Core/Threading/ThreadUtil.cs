using Neko.Utility.Core.IO.Logging;
using System;
using System.Threading;

namespace Neko.Utility.Core.Threading
{
    /// <summary>
    /// 线程帮助类
    /// <para>一些线程的快速操作</para>
    /// </summary>
    public sealed class ThreadUtil
    {
        /// <summary>
        /// <inheritdoc cref="Thread.Sleep(int)"/>
        /// </summary>
        /// <param name="interval">休眠时间</param>
        public static void Sleep(int interval)
        {
            Thread.Sleep(interval);
        }

        /// <summary>
        /// 创建并运行一个新线程
        /// </summary>
        /// <param name="executeCode">线程要执行的方法</param>
        /// <returns></returns>
        public static Thread RunThread(EmptyDelegateCode executeCode)
        {
            ThreadStart threadStart = new ThreadStart(delegate ()
            {
                try
                {
                    executeCode?.Invoke();
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            });
            Thread executeThread = new Thread(threadStart);
            executeThread.IsBackground = true;
            executeThread.Start();
            return executeThread;
        }

        /// <summary>
        /// 创建并循环执行一个线程
        /// </summary>
        /// <param name="intervalInfo">线程信息</param>
        /// <returns></returns>
        public static Thread RunLoop(IntervalInfo intervalInfo)
        {
            if(intervalInfo == null)
            {
                return null;
            }
            Thread executeThread = RunThread(delegate ()
            {
                do
                {
                    try
                    {
                        intervalInfo.ExecuteCode?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.WriteException(ex);
                    }
                    finally
                    {
                        Sleep(intervalInfo.Interval);
                    }
                } while (!intervalInfo.Break);
            });
            intervalInfo.CurrentThread = executeThread;
            return executeThread;
        }
    }
}
