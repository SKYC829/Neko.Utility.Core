using Neko.Utility.Core.IO.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                    throw ex;
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
            if (intervalInfo == null)
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
                        intervalInfo.Break = true;
                        throw ex;
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

        /// <summary>
        /// 以异步的方式运行一个委托方法
        /// </summary>
        /// <param name="executeCode">委托方法</param>
        /// <returns></returns>
        public static async Task RunAsync(EmptyDelegateCode executeCode)
        {
            await Task.Run(() => executeCode?.Invoke());
        }

        /// <summary>
        /// 以异步的方式循环运行一个委托方法(可中断)
        /// </summary>
        /// <param name="intervalInfo">线程信息</param>
        /// <returns></returns>
        public static async Task RunAsync(IntervalInfo intervalInfo)
        {
            if(intervalInfo == null)
            {
                return;
            }
            await Task.Run(() =>
            {
                while (!intervalInfo.Break)
                {
                    intervalInfo?.ExecuteCode.Invoke();
                    Sleep(intervalInfo.Interval);
                }
            });
        }

        /// <summary>
        /// <inheritdoc cref="RunAsync(EmptyDelegateCode)"/>
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="executeCode">委托方法</param>
        /// <returns></returns>
        public static async Task<TResult> RunAsync<TResult>(EmptyDelegateCode<TResult> executeCode)
        {
            return await Task.Run(() =>
            {
                TResult result = default(TResult);
                if(executeCode != null)
                {
                    result = executeCode.Invoke();
                }
                return result;
            });
        }
    }
}
