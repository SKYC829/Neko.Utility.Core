using Neko.Utility.Core.Common;
using Neko.Utility.Core.Data;
using Neko.Utility.Core.IO.Logging;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Neko.Utility.Core.IO
{
    /*
     * RSA加密解密类参考至以下文档:
     * https://www.jb51.net/article/62858.htm => 原生的加密解密方式,加密长度有限制
     * https://blog.csdn.net/weixin_38211198/article/details/107404282 => 修改后的加密解密方式,加密长度无限制,使用此方法不知道为什么可以作到私钥加密公钥解密,也许是因为我的私钥和公钥都是直接生成的,不是通过像是证书那样计算的原因
     */
    public sealed partial class EncryptionUtil
    {
        /// <summary>
        /// 生成一对随机的公钥和私钥
        /// </summary>
        /// <returns>
        /// <see cref="ValueTuple{T1,T2}.Item1"/>是私钥<br/><see cref="ValueTuple{T1,T2}.Item2"/>是公钥
        /// </returns>
        public static ValueTuple<string, string> GeneralRSAKey()
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            return new ValueTuple<string, string>(provider.ToXmlString(true), provider.ToXmlString(false));
        }

        /// <summary>
        /// <inheritdoc cref="EncryptRSA(object, string)"/>并转换为Base64字符串
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <param name="publicKey">RSA公钥(因为C#原生的只支持公钥加密私钥解密,就假装这是私钥好了)</param>
        /// <returns></returns>
        public static string EncryptRSAToString(object content, string publicKey)
        {
            byte[] encryptResult = EncryptRSA(content, publicKey);
            return Convert.ToBase64String(encryptResult);
        }

        /// <summary>
        /// RSA非对称加密
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <param name="publicKey">RSA公钥(因为C#原生的只支持公钥加密私钥解密,就假装这是私钥好了)</param>
        /// <returns></returns>
        public static byte[] EncryptRSA(object content, string publicKey)
        {
            if (StringUtil.IsNullOrEmpty(content) || StringUtil.IsNullOrEmpty(publicKey))
            {
                return null;
            }
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(publicKey);
            byte[] result = new byte[0];
            try
            {
                int bufferSize = (provider.KeySize / 8) - 11;
                byte[] buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(SerializeUtil.ToBinary(content)))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        while (inputStream.Position < inputStream.Length)
                        {
                            int readNum = inputStream.Read(buffer, 0, buffer.Length);
                            byte[] encryptBlock = new byte[readNum];
                            Buffer.BlockCopy(buffer, 0, encryptBlock, 0, readNum);
                            byte[] encryptResult = provider.Encrypt(encryptBlock, false);
                            outputStream.Write(encryptResult, 0, encryptResult.Length);
                        }
                        result = outputStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                provider.Dispose();
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="DeEncryptRSA(byte[], string)"/>
        /// </summary>
        /// <typeparam name="Tobject">对象类型</typeparam>
        /// <param name="encryptContent">要解密的数据,如果是一个对象,该对象必须实现了<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">RSA私钥(因为C#原生的只支持公钥加密私钥解密,就假装这是公钥好了)</param>
        /// <returns></returns>
        public static Tobject DeEncryptRSA<Tobject>(string encryptContent, string privateKey)
        {
            Tobject result = default(Tobject);
            if (StringUtil.IsNullOrEmpty(encryptContent) || StringUtil.IsNullOrEmpty(privateKey))
            {
                return result;
            }
            try
            {
                byte[] contentBytes = Convert.FromBase64String(encryptContent);
                object deencryptResult = DeEncryptRSA(contentBytes, privateKey);
                Type resultType = deencryptResult.GetType();
                if (resultType.GetInterface(nameof(IConvertible)) != null)
                {
                    result = (Tobject)Convert.ChangeType(deencryptResult, typeof(Tobject));
                }
                else
                {
                    result = (Tobject)deencryptResult;
                }
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("解密时无法转换对象类型!", ex);
            }
            return result;
        }

        /// <summary>
        /// RSA非对称解密
        /// </summary>
        /// <param name="encryptBytes">要解密的二进制数据,如果是一个对象,该对象必须实现了<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">RSA私钥(因为C#原生的只支持公钥加密私钥解密,就假装这是公钥好了)</param>
        /// <returns></returns>
        public static object DeEncryptRSA(byte[] encryptBytes, string privateKey)
        {
            if (StringUtil.IsNullOrEmpty(encryptBytes) || StringUtil.IsNullOrEmpty(privateKey))
            {
                return null;
            }
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(privateKey);
            object result = null;
            try
            {
                int bufferSize = (provider.KeySize / 8)/* - 11*/;
                byte[] buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(encryptBytes))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        while (inputStream.Position < inputStream.Length)
                        {
                            int readNum = inputStream.Read(buffer, 0, buffer.Length);
                            byte[] encryptBlock = new byte[readNum];
                            Buffer.BlockCopy(buffer, 0, encryptBlock, 0, readNum);
                            byte[] deencryptResult = provider.Decrypt(encryptBlock, false);
                            outputStream.Write(deencryptResult, 0, deencryptResult.Length);
                        }
                        byte[] binaryBytes = outputStream.ToArray();
                        result = SerializeUtil.FromBinary(binaryBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                provider.Dispose();
            }
            return result;
        }
    }
}
