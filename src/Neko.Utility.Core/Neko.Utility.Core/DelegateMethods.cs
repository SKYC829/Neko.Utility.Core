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
    /// <param name="args">参数</param>
    public delegate void ParameterDelegateCode(params object[] args);

    /// <summary>
    /// 压缩/解压文件时的委托方法
    /// </summary>
    /// <param name="fileName">正在压缩/解压的文件</param>
    /// <param name="compressedSize">已经压缩/解压的文件大小</param>
    /// <param name="totalSize">正在压缩/解压的文件总大小</param>
    /// <param name="isFolder">当前是否是文件夹</param>
    public delegate void CompressDelegateCode(string fileName, uint compressedSize, long totalSize, bool isFolder);
}
