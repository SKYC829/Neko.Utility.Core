using ICSharpCode.SharpZipLib.Zip;
using Neko.Utility.Core.IO.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neko.Utility.Core.IO
{
    /// <summary>
    /// 压缩文件帮助类
    /// <para>目前仅支持zip(因为zip应用最广泛了)</para>
    /// </summary>
    public sealed class CompressUtil
    {
        /// <summary>
        /// 当前压缩包的文件/文件夹列表
        /// </summary>
        private static List<FileSystemInfo> _zipFileList;

        /// <summary>
        /// 正在压缩事件
        /// </summary>
        public static event CompressDelegateCode OnCompress;

        /// <summary>
        /// 正在解压事件
        /// </summary>
        public static event CompressDelegateCode OnDecompress;

        /// <summary>
        /// <inheritdoc cref="_zipFileList"/>
        /// </summary>
        public static IEnumerable<FileSystemInfo> ZipFileList { get { return _zipFileList.AsEnumerable(); } }

        static CompressUtil()
        {
            _zipFileList = new List<FileSystemInfo>();
        }

        /// <summary>
        /// 添加一个文件或文件夹到压缩文件列表
        /// </summary>
        /// <param name="fileName">文件或文件夹路径</param>
        public static void Add(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            Add(new FileInfo(fileName));
        }

        /// <summary>
        /// 添加一个文件或文件夹到压缩文件列表
        /// </summary>
        /// <param name="fileSystem">文件或文件夹对象</param>
        public static void Add(FileSystemInfo fileSystem)
        {
            if (fileSystem == null)
            {
                return;
            }
            if (!_zipFileList.Exists(p => p.Name.Equals(fileSystem.Name)))
            {
                _zipFileList.Add(fileSystem);
            }
            else
            {
                throw new ArgumentException(string.Format("{0}已在压缩文件列表内,无需再次添加", fileSystem.Name));
            }
        }

        /// <summary>
        /// 从压缩文件列表中移除一个文件或文件夹
        /// </summary>
        /// <param name="fileName">文件或文件夹名称</param>
        public static void Remove(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            Remove(new FileInfo(fileName));
        }

        /// <summary>
        /// 从压缩文件列表中移除一个文件或文件夹
        /// </summary>
        /// <param name="fileSystem">文件或文件夹对象</param>
        public static void Remove(FileSystemInfo fileSystem)
        {
            if (fileSystem == null)
            {
                return;
            }
            if (_zipFileList.Exists(p => p.Name.Equals(fileSystem.Name)))
            {
                _zipFileList.Remove(fileSystem);
            }
        }

        /// <summary>
        /// 打开并读取Zip文件
        /// </summary>
        /// <param name="fileName">Zip文件路径</param>
        /// <returns></returns>
        public static ZipInputStream OpenZip(string fileName)
        {
            ZipInputStream result = null;
            if (File.Exists(fileName))
            {
                result = new ZipInputStream(File.OpenRead(fileName));
                ZipEntry entry = null;
                while ((entry = result.GetNextEntry()) != null)
                {
                    try
                    {
                        Add(new FileInfo(entry.Name));
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 添加一个文件或文件夹到一个已有的压缩文件中
        /// </summary>
        /// <param name="fileSystem">文件或文件夹对象</param>
        /// <param name="zipStream">压缩文件流</param>
        /// <param name="root">文件或文件夹在压缩文件内的父目录</param>
        public static void AddEntry(FileSystemInfo fileSystem, ZipOutputStream zipStream, string root = "")
        {
            if (fileSystem == null || !fileSystem.Exists)
            {
                return;
            }
            if (zipStream == null)
            {
                return;
            }
            ZipEntry zipEntry = null;
            LogUtil.WriteLog(Configurations.LogLevel.Track, string.Format("正在压缩{0}", fileSystem.Name));
            if (fileSystem.Attributes.HasFlag(FileAttributes.Directory))
            {
                zipEntry = new ZipEntry(Path.Combine(root, fileSystem.Name + "\\"));
                zipStream.PutNextEntry(zipEntry);
                OnCompress?.Invoke(fileSystem.Name, 1, 1, true);
                foreach (FileSystemInfo fileSystemInfo in (fileSystem as DirectoryInfo).GetFileSystemInfos())
                {
                    AddEntry(fileSystemInfo, zipStream, zipEntry.Name);
                }
            }
            else
            {
                zipEntry = new ZipEntry(Path.Combine(root, fileSystem.Name));
                zipStream.PutNextEntry(zipEntry);
                try
                {
                    using (FileStream readStream = new FileStream(fileSystem.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] dataBytes = new byte[10240];
                        int readNum = 0;
                        uint totalReadNum = 0;
                        while (readStream.Position < readStream.Length)
                        {
                            readNum = readStream.Read(dataBytes, 0, dataBytes.Length);
                            totalReadNum += (uint)readNum;
                            OnCompress?.Invoke(fileSystem.Name, totalReadNum, readStream.Length, false);
                            zipStream.Write(dataBytes, 0, readNum);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 根据<inheritdoc cref="_zipFileList"/>生成压缩文件
        /// </summary>
        /// <param name="zipFile">压缩文件名称</param>
        public static void Compress(string zipFile)
        {
            Compress(zipFile, null);
        }

        /// <summary>
        /// <inheritdoc cref="_zipFileList"/>
        /// </summary>
        /// <param name="zipFile">压缩文件名称</param>
        /// <param name="passCode">压缩文件密码</param>
        public static void Compress(string zipFile, string passCode)
        {
            LogUtil.WriteLog(Configurations.LogLevel.Track, string.Format("开始生成压缩文件{0}", zipFile));
            zipFile = VerifyFileName(zipFile);
            try
            {
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }
                using (FileStream outputStream = new FileStream(zipFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (ZipOutputStream zipStream = new ZipOutputStream(outputStream))
                    {
                        zipStream.SetLevel(6);
                        zipStream.Password = passCode;
                        foreach (FileSystemInfo file in _zipFileList)
                        {
                            AddEntry(file, zipStream);
                        }
                    }
                }
                if (!VerifyArchive(zipFile))
                {
                    LogUtil.WriteWarning(null, "已成功生成压缩文件,但校验未通过");
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }
                throw ex;
            }
        }

        /// <summary>
        /// 压缩一个字节数组
        /// </summary>
        /// <param name="bytes">要压缩的字节数组</param>
        /// <returns></returns>
        public static byte[] CompressBytes(byte[] bytes)
        {
            byte[] result = bytes;
            if(bytes == null)
            {
                return null;
            }
            try
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (ZipOutputStream inputStream = new ZipOutputStream(outputStream))
                    {
                        inputStream.SetLevel(7);
                        inputStream.Write(bytes, 0, bytes.Length);
                    }
                    result = outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 验证文件名,使文件名已.zip结尾
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private static string VerifyFileName(string fileName)
        {
            if (!Path.GetExtension(fileName).ToLower().Equals(".zip"))
            {
                fileName = string.Format("{0}.zip", fileName);
            }
            return fileName;
        }

        /// <summary>
        /// 校验zip文件是否有错误
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static bool VerifyArchive(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }
            ZipFile zipFile = new ZipFile(fileName);
            return zipFile.TestArchive(true, TestStrategy.FindFirstError, null);
        }

        /// <summary>
        /// 解压zip文件
        /// </summary>
        /// <param name="zipFile">zip文件路径</param>
        /// <param name="unzipPath">要解压到的路径</param>
        public static void Decompress(string zipFile, string unzipPath)
        {
            Decompress(zipFile, unzipPath, null);
        }

        /// <summary>
        /// <inheritdoc cref="Decompress(string, string)"/>
        /// </summary>
        /// <param name="zipFile">zip文件路径</param>
        /// <param name="unzipPath">要解压到的路径</param>
        /// <param name="passCode">压缩文件密码</param>
        public static async void Decompress(string zipFile, string unzipPath, string passCode)
        {
            LogUtil.WriteLog(Configurations.LogLevel.Track, string.Format("开始解压压缩文件{0}", zipFile));
            zipFile = VerifyFileName(zipFile);
            if (!File.Exists(zipFile) || !VerifyArchive(zipFile))
            {
                return;
            }
            if (!Directory.Exists(unzipPath))
            {
                Directory.CreateDirectory(unzipPath);
            }
            using (ZipInputStream zipStream = new ZipInputStream(new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                zipStream.Password = passCode;
                ZipEntry zipEntry = null;
                try
                {
                    while ((zipEntry = zipStream.GetNextEntry()) != null)
                    {
                        LogUtil.WriteLog(Configurations.LogLevel.Track, string.Format("正在解压{0}", zipEntry.Name));
                        if (string.IsNullOrEmpty(zipEntry.Name))
                        {
                            continue;
                        }
                        string fullUnzipPath = Path.Combine(unzipPath, zipEntry.Name);
                        if (zipEntry.IsDirectory)
                        {
                            FileAttributes fileAttributes = FileAttributes.Normal;
                            Directory.CreateDirectory(fullUnzipPath);
                            if (zipEntry.Name.StartsWith('.'))
                            {
                                File.SetAttributes(fullUnzipPath, fileAttributes | FileAttributes.Hidden);
                            }
                            OnDecompress?.Invoke(zipEntry.Name, 1, 1, true);
                        }
                        else if (zipEntry.IsFile)
                        {
                            using (FileStream writeStream = new FileStream(fullUnzipPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                            {
                                int readNum = 0;
                                uint writeNum = 0;
                                byte[] dataBytes = new byte[10240];
                                do
                                {
                                    readNum = await zipStream.ReadAsync(dataBytes, 0, dataBytes.Length);
                                    writeNum += (uint)readNum;
                                    OnDecompress?.Invoke(zipEntry.Name, writeNum, zipEntry.Size, false);
                                    await writeStream.WriteAsync(dataBytes, 0, readNum);
                                } while (readNum > 0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (File.Exists(unzipPath))
                    {
                        File.Delete(unzipPath);
                    }
                    else if (Directory.Exists(unzipPath))
                    {
                        Directory.Delete(unzipPath);
                    }
                    if (ex.Message.Equals("No password set."))
                    {
                        throw new ArgumentNullException("passCode", "请设置解压密码!");
                    }
                    throw ex;
                }
                finally
                {
                    zipStream.Close();
                    zipStream.Dispose();
                    LogUtil.WriteLog(Configurations.LogLevel.Track, "已成功解压压缩文件");
                }
            }
        }

        /// <summary>
        /// 解<inheritdoc cref="CompressBytes(byte[])"/>
        /// </summary>
        /// <param name="bytes">要解压缩的字节数组</param>
        /// <returns></returns>
        public static byte[] DeCompressBytes(byte[] bytes)
        {
            byte[] result = bytes;
            if(bytes == null)
            {
                return null;
            }
            try
            {
                using (MemoryStream inputStream = new MemoryStream(bytes))
                {
                    using (ZipInputStream zipStream = new ZipInputStream(inputStream))
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            byte[] dataBytes = new byte[1024];
                            int readNum = 0;
                            do
                            {
                                readNum = zipStream.Read(dataBytes, 0, dataBytes.Length);
                                outputStream.Write(dataBytes, 0, readNum);
                            } while (readNum>0);
                            result = outputStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
