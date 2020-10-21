using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neko.Utility.Core.Common
{
    /// <summary>
    /// 序列化帮助类
    /// <para>可以帮助快速操作<br/>二进制序列化<br/>json序列化<br/>xml序列化</para>
    /// </summary>
    public sealed partial class SerializeUtil
    {
        /// <summary>
        /// 将一个对象序列化为二进制数据
        /// </summary>
        /// <param name="fromObject">要序列化的对象</param>
        /// <returns></returns>
        public static byte[] ToBinary(object fromObject)
        {
            Type objectType = fromObject.GetType();
            if(objectType.IsClass && objectType.GetCustomAttribute(typeof(SerializableAttribute),true) == null)
            {
                throw new NotSupportedException(string.Format("序列化至二进制的类型{0}必须要标记{1}特性!",objectType.FullName,nameof(SerializableAttribute)));
            }
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, fromObject);
                byte[] buffer = new byte[(int)ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// 将一个二进制数组反序列化为一个对象
        /// </summary>
        /// <typeparam name="Tobject">对象类型</typeparam>
        /// <param name="binaryBytes">二进制数组</param>
        /// <returns></returns>
        public static Tobject FromBinary<Tobject>(byte[] binaryBytes)
        {
            if (binaryBytes == null || binaryBytes.Length <= 0)
            {
                return default(Tobject);
            }
            using (MemoryStream ms = new MemoryStream(binaryBytes))
            {
                return FromBinary<Tobject>(ms);
            }
        }

        /// <summary>
        /// 将一个<see cref="Stream"/>反序列化为一个对象
        /// </summary>
        /// <typeparam name="Tobject">对象类型</typeparam>
        /// <param name="binaryStream">要反序列化的流<see cref="Stream"/></param>
        /// <returns></returns>
        public static Tobject FromBinary<Tobject>(Stream binaryStream)
        {
            Tobject result = default(Tobject);
            try
            {
                object serializeResult = FromBinary(binaryStream);
                Type resultType = serializeResult.GetType();
                if(resultType.GetInterface(nameof(IConvertible)) != null)
                {
                    result = (Tobject)Convert.ChangeType(serializeResult, typeof(Tobject));
                }
                else
                {
                    result = (Tobject)serializeResult;
                }
            }
            catch (InvalidCastException ex)
            {
                //TODO:
            }
            catch (SerializationException ex)
            {
                //TODO:
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 将一个二进制数组反序列化为一个对象
        /// </summary>
        /// <param name="binaryBytes">二进制数组</param>
        /// <returns></returns>
        public static object FromBinary(byte[] binaryBytes)
        {
            if(binaryBytes == null || binaryBytes.Length <= 0)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(binaryBytes))
            {
                return FromBinary(ms);
            }
        }

        /// <summary>
        /// 将一个<see cref="Stream"/>反序列化为一个对象
        /// </summary>
        /// <param name="binaryStream">要反序列化的流<see cref="Stream"/></param>
        /// <returns></returns>
        public static object FromBinary(Stream binaryStream)
        {
            if(binaryStream == null)
            {
                return null;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            object result = null;
            try
            {
                result = binaryFormatter.Deserialize(binaryStream);
            }
            catch (SerializationException ex)
            {
                //TODO:
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}
