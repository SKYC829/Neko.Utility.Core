using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neko.Utility.Core.Common
{
    /// <summary>
    /// 调用代码帮助类
    /// </summary>
    public sealed class InvokeCode
    {
        /// <summary>
        /// 要执行的方法的队列
        /// </summary>
        private readonly List<EmptyDelegateCode> _codeQueue;

        /// <summary>
        /// 获取要执行的方法的队列
        /// </summary>
        public IEnumerable<EmptyDelegateCode> CodeStacks { get { return _codeQueue?.ToList(); } }

        /// <summary>
        /// 是否中断执行并跳出方法
        /// </summary>
        public bool IsBreak { get; set; }

        /// <summary>
        /// 当前正在执行的方法是否是方法队列的最后一个方法
        /// </summary>
        public bool IsFinaly { get { return _codeQueue != null && _codeQueue.Count == 0; } }

        public InvokeCode()
        {
            if(_codeQueue == null)
            {
                _codeQueue = new List<EmptyDelegateCode>();
            }
        }

        /// <summary>
        /// 将一个方法添加到要执行的方法的队列列尾
        /// </summary>
        /// <param name="executeCode">委托方法</param>
        public void Add(EmptyDelegateCode executeCode)
        {
            lock (_codeQueue)
            {
                _codeQueue.Add(executeCode);
            }
        }

        /// <summary>
        /// 将一个方法添加到要执行的方法的队列列头
        /// </summary>
        /// <param name="executeCode">委托方法</param>
        public void Shift(EmptyDelegateCode executeCode)
        {
            Insert(0, executeCode);
        }

        /// <summary>
        /// 在队列指定位置插入一个要执行的方法
        /// </summary>
        /// <param name="index">位置索引</param>
        /// <param name="executeCode">委托方法</param>
        public void Insert(int index,EmptyDelegateCode executeCode)
        {
            lock (_codeQueue)
            {
                _codeQueue.Insert(index, executeCode);
            }
        }

        /// <summary>
        /// 删除队列指定位置的一个方法
        /// </summary>
        /// <param name="index">委托方法</param>
        public void RemoveAt(int index)
        {
            lock (_codeQueue)
            {
                EmptyDelegateCode executeCode = _codeQueue.ElementAt(index);
                Remove(executeCode);
            }
        }

        /// <summary>
        /// 从队列中删除一个方法
        /// </summary>
        /// <param name="executeCode">委托方法</param>
        public void Remove(EmptyDelegateCode executeCode)
        {
            lock (_codeQueue)
            {
                _codeQueue.Remove(executeCode);
            }
        }

        /// <summary>
        /// 开始循环执行队列的所有方法
        /// </summary>
        public void Execute()
        {
            while (_codeQueue.Count > 0)
            {
                if (IsBreak)
                {
                    break;
                }
                ExecuteNext();
            }
        }

        /// <summary>
        /// 执行队列中的第一个方法
        /// </summary>
        public void ExecuteNext()
        {
            if (_codeQueue.Count == 0)
            {
                return;
            }
            lock (_codeQueue)
            {
                EmptyDelegateCode executeCode = _codeQueue.FirstOrDefault();
                if (executeCode == null)
                {
                    return;
                }
                Remove(executeCode);
                executeCode.Invoke();
            }
        }

        /// <summary>
        /// 以异步的方式循环执行队列的所有方法
        /// </summary>
        /// <param name="cancelToken">取消异步执行的信号</param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancelToken = default)
        {
            while (_codeQueue.Count > 0)
            {
                if (IsBreak)
                {
                    break;
                }
                await ExecuteNextAsync(cancelToken);
            }
        }

        /// <summary>
        /// 以异步的方式执行队列中的第一个方法
        /// </summary>
        /// <param name="cancelToken">取消异步执行的信号</param>
        /// <returns></returns>
        public async Task ExecuteNextAsync(CancellationToken cancelToken = default)
        {
            if(_codeQueue.Count == 0)
            {
                return;
            }
            EmptyDelegateCode executeCode = null;
            lock (_codeQueue)
            {
                executeCode = _codeQueue.FirstOrDefault();
            }
            if(executeCode == null)
            {
                return;
            }
            Remove(executeCode);
            await Task.WhenAny(Task.Run(new Action(executeCode), cancelToken));
        }
    }
}
