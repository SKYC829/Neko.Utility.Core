using System;
using System.Collections.Generic;
using System.Text;

namespace Neko.Utility.Core
{
    /// <summary>
    /// 无参的委托方法
    /// </summary>
    public delegate void EmptyDelegateCode();

    /// <summary>
    /// 无参的委托方法
    /// </summary>
    /// <typeparam name="TReturn">返回值类型</typeparam>
    /// <returns></returns>
    public delegate TReturn EmptyDelegateCode<TReturn>();

    /// <summary>
    /// 有参的委托方法
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="args">参数</param>
    public delegate void ParameterDelegateCode(params object[] args);
}
