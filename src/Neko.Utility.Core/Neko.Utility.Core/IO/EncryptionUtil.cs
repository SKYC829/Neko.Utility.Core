using Neko.Utility.Core.Common;
using Neko.Utility.Core.Data;
using Neko.Utility.Core.IO.Logging;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Neko.Utility.Core.IO
{
    /// <summary>
    /// 加密帮助类
    /// <para>一些常用的加密的快速操作</para>
    /// </summary>
    public sealed partial class EncryptionUtil
    {
        /// <summary>
        /// 验证密钥长度,如果大于长度则会截断密钥
        /// </summary>
        /// <param name="keyBytes">密钥二进制数组</param>
        /// <param name="keyLength">密钥长度</param>
        /// <returns></returns>
        private static byte[] VerifyKeyBytes(byte[] keyBytes, int keyLength)
        {
            if (keyLength <= 0)
            {
                return keyBytes;
            }
            byte[] result = new byte[keyLength];
            try
            {
                if (keyBytes.Length > keyLength)
                {
                    Buffer.BlockCopy(keyBytes, 0, result, 0, keyLength);
                }
                else
                {
                    Buffer.BlockCopy(keyBytes, 0, result, 0, keyBytes.Length);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //LogUtil.WriteException(ex, "在验证密钥时发剩错误,索引超出密钥长度,返回密钥本身!");
                result = keyBytes;
                throw new IndexOutOfRangeException("在验证密钥时发剩错误,索引超出密钥长度", ex);
            }
            return result;
        }

        /// <summary>
        /// 根据<see cref="ICryptoTransform"/>工厂进行对应的加密或解密操作
        /// </summary>
        /// <param name="cryptoTransform">加密或解密的工厂类</param>
        /// <param name="value">要加密或解密的字节数据</param>
        /// <returns></returns>
        public static byte[] Encrypt(ICryptoTransform cryptoTransform, byte[] value)
        {
            byte[] result = null;
            try
            {
                result = cryptoTransform.TransformFinalBlock(value, 0, value.Length);
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            finally
            {
                cryptoTransform.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 创建加密解密用的<see cref="SymmetricAlgorithm"/>对象并设置密钥
        /// </summary>
        /// <typeparam name="TFactory">加密解密对象工厂</typeparam>
        /// <param name="privateKey">密钥</param>
        /// <param name="keyLength">密钥长度</param>
        /// <returns></returns>
        private static SymmetricAlgorithm GetEncryptFactory<TFactory>(string privateKey, int keyLength) where TFactory : SymmetricAlgorithm, new()
        {
            SymmetricAlgorithm managed = new TFactory();
            managed.Mode = CipherMode.CBC;
            managed.Padding = PaddingMode.PKCS7;
            if (StringUtil.IsNullOrEmpty(privateKey))
            {
                privateKey = "19980522";
            }
            byte[] keyBytes = Encoding.Default.GetBytes(privateKey);
            managed.Key = VerifyKeyBytes(keyBytes, keyLength);
            managed.IV = VerifyKeyBytes(keyBytes, keyLength);
            return managed;
        }

        /// <summary>
        /// <inheritdoc cref="EncryptAES(object, string)"/>并转换为Base64字符串
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">加密的密钥</param>
        /// <returns></returns>
        public static string EncryptAESToString(object content, string privateKey)
        {
            byte[] encryptResult = EncryptAES(content, privateKey);
            if (encryptResult == null || encryptResult.Length <= 0)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(encryptResult);
        }

        /// <summary>
        /// AES对称加密
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">加密的密钥</param>
        /// <returns></returns>
        public static byte[] EncryptAES(object content, string privateKey)
        {
            byte[] result = new byte[0];
            if (StringUtil.IsNullOrEmpty(content))
            {
                return result;
            }
            RijndaelManaged managed = (RijndaelManaged)GetEncryptFactory<RijndaelManaged>(privateKey, 16);
            result = Encrypt(managed.CreateEncryptor(), SerializeUtil.ToBinary(content));
            managed.Dispose();
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="DeEncryptAES(byte[], string)"/>
        /// </summary>
        /// <typeparam name="Tobject">对象类型</typeparam>
        /// <param name="encryptContent">加密后的Base64字符串</param>
        /// <param name="privateKey">加密的密钥</param>
        /// <returns></returns>
        public static Tobject DeEncryptAES<Tobject>(string encryptContent, string privateKey)
        {
            Tobject result = default(Tobject);
            if (StringUtil.IsNullOrEmpty(encryptContent))
            {
                return result;
            }
            try
            {
                byte[] contentBytes = Convert.FromBase64String(encryptContent);
                object deencryptResult = DeEncryptAES(contentBytes, privateKey);
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
        /// AES对称解密
        /// </summary>
        /// <param name="encryptBytes">要解密的二进制数据,如果是一个对象,该对象必须实现了<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">加密的私钥</param>
        /// <returns></returns>
        public static object DeEncryptAES(byte[] encryptBytes, string privateKey)
        {
            object result = null;
            if (StringUtil.IsNullOrEmpty(encryptBytes))
            {
                return result;
            }
            RijndaelManaged managed = (RijndaelManaged)GetEncryptFactory<RijndaelManaged>(privateKey, 16);
            try
            {
                byte[] deencryptResult = Encrypt(managed.CreateDecryptor(), encryptBytes);
                result = SerializeUtil.FromBinary<object>(deencryptResult);
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            catch (SerializationException ex)
            {
                throw new SerializationException("解密时序列化失败,无法反序列化对象", ex);
            }
            finally
            {
                managed.Dispose();
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="EncryptDES(object, string)"/>并转换为Base64字符串
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">加密的密钥</param>
        /// <returns></returns>
        public static string EncryptDESToString(object content, string privateKey)
        {
            byte[] encryptResult = EncryptDES(content, privateKey);
            if (encryptResult == null || encryptResult.Length <= 0)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(encryptResult);
        }

        /// <summary>
        /// DES对称加密
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">加密的密钥</param>
        /// <returns></returns>
        public static byte[] EncryptDES(object content, string privateKey)
        {
            byte[] result = new byte[0];
            if (StringUtil.IsNullOrEmpty(content))
            {
                return result;
            }
            DESCryptoServiceProvider provider = (DESCryptoServiceProvider)GetEncryptFactory<DESCryptoServiceProvider>(privateKey, 8);
            result = Encrypt(provider.CreateEncryptor(), SerializeUtil.ToBinary(content));
            provider.Dispose();
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="DeEncryptDES(byte[], string)"/>
        /// </summary>
        /// <typeparam name="Tobject">对象类型</typeparam>
        /// <param name="encryptContent">要解密的数据,如果是一个对象,该对象必须实现了<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">加密的私钥</param>
        /// <returns></returns>
        public static Tobject DeEncryptDES<Tobject>(string encryptContent, string privateKey)
        {
            Tobject result = default(Tobject);
            if (StringUtil.IsNullOrEmpty(encryptContent))
            {
                return result;
            }
            try
            {
                byte[] contentBytes = Convert.FromBase64String(encryptContent);
                object deencryptResult = DeEncryptDES(contentBytes, privateKey);
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
        /// DES对称解密
        /// </summary>
        /// <param name="encryptBytes">要解密的二进制数据,如果是一个对象,该对象必须实现了<see cref="SerializableAttribute"/></param>
        /// <param name="privateKey">加密的私钥</param>
        /// <returns></returns>
        public static object DeEncryptDES(byte[] encryptBytes, string privateKey)
        {
            object result = null;
            if (StringUtil.IsNullOrEmpty(encryptBytes))
            {
                return result;
            }
            DESCryptoServiceProvider provider = (DESCryptoServiceProvider)GetEncryptFactory<DESCryptoServiceProvider>(privateKey, 8);
            try
            {
                byte[] deencryptResult = Encrypt(provider.CreateDecryptor(), encryptBytes);
                result = SerializeUtil.FromBinary<object>(deencryptResult);
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            catch (SerializationException ex)
            {
                throw new SerializationException("解密时序列化失败,无法反序列化对象", ex);
            }
            finally
            {
                provider.Dispose();
            }
            return result;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <returns></returns>
        public static string EncryptMD5(object content)
        {
            if (StringUtil.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            string result = string.Empty;
            try
            {
                byte[] resultBytes = provider.ComputeHash(SerializeUtil.ToBinary(content));
                for (int i = 0; i < resultBytes.Length; i++)
                {
                    result += resultBytes.ElementAt(i).ToString("x2");
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
        /// SHA256加密
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <returns></returns>
        public static string EncryptSHA256(object content)
        {
            if (StringUtil.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            SHA256 managed = SHA256.Create();
            string result = string.Empty;
            try
            {
                byte[] resultBytes = managed.ComputeHash(SerializeUtil.ToBinary(content));
                for (int i = 0; i < resultBytes.Length; i++)
                {
                    result += resultBytes.ElementAt(i).ToString("x2");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                managed.Dispose();
            }
            return result;
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="content">要加密的内容,如果是一个对象必须要实现<see cref="SerializableAttribute"/></param>
        /// <returns></returns>
        public static string EncryptSHA1(object content)
        {
            if (StringUtil.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            SHA1 managed = SHA1.Create();
            string result = string.Empty;
            try
            {
                byte[] resultBytes = managed.ComputeHash(SerializeUtil.ToBinary(content));
                for (int i = 0; i < resultBytes.Length; i++)
                {
                    result += resultBytes.ElementAt(i).ToString("x2");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                managed.Dispose();
            }
            return result;
        }
    }
}
