using Neko.Utility.Core.IO.Logging;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Neko.Utility.Core.Common
{
    public sealed partial class SerializeUtil
    {
        /// <summary>
        /// 将一个对象序列化为xml格式的二进制数据
        /// </summary>
        /// <param name="fromObject">要序列化的对象</param>
        /// <returns></returns>
        public static byte[] ToXml(object fromObject)
        {
            byte[] buffer = new byte[0];
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(fromObject.GetType());
                    xmlSerializer.Serialize(ms, fromObject);
                    buffer = new byte[(int)ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(buffer, 0, buffer.Length);
                    //return buffer;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw ex;
            }
            return buffer;
        }

        /// <summary>
        /// 将一个xml数据反序列化为一个对象
        /// </summary>
        /// <param name="xmlString">xml字符串</param>
        /// <returns></returns>
        public static object FromXml(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }
            return FromXml(Encoding.Default.GetBytes(xmlString));
        }

        /// <summary>
        /// 将一个xml数据反序列化为一个对象
        /// </summary>
        /// <param name="xmlBytes">xml数据的二进制数组</param>
        /// <returns></returns>
        public static object FromXml(byte[] xmlBytes)
        {
            if (xmlBytes == null || xmlBytes.Length <= 0)
            {
                return null;
            }
            object result = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(xmlBytes))
                {
                    result = FromXml(ms);
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 将一个xml数据反序列化为一个对象
        /// </summary>
        /// <param name="xmlStream">xml数据流</param>
        /// <returns></returns>
        public static object FromXml(Stream xmlStream)
        {
            if (xmlStream == null)
            {
                return null;
            }
            return FromXml<object>(xmlStream);
        }

        /// <summary>
        /// 将一个xml数据反序列化为一个对象
        /// </summary>
        /// <typeparam name="Tvalue">对象类型</typeparam>
        /// <param name="xmlString">xml字符串</param>
        /// <returns></returns>
        public static Tvalue FromXml<Tvalue>(string xmlString)
        {
            Tvalue result = default(Tvalue);
            if (string.IsNullOrEmpty(xmlString))
            {
                return result;
            }
            return FromXml<Tvalue>(Encoding.Default.GetBytes(xmlString));
        }

        /// <summary>
        /// 将一个xml数据反序列化为一个对象
        /// </summary>
        /// <typeparam name="Tvalue">对象类型</typeparam>
        /// <param name="xmlBytes">xml数据的二进制数组</param>
        /// <returns></returns>
        public static Tvalue FromXml<Tvalue>(byte[] xmlBytes)
        {
            Tvalue result = default(Tvalue);
            if (xmlBytes == null || xmlBytes.Length <= 0)
            {
                return result;
            }
            try
            {
                using (MemoryStream ms = new MemoryStream(xmlBytes))
                {
                    result = FromXml<Tvalue>(ms);
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 将一个xml数据反序列化为一个对象
        /// </summary>
        /// <typeparam name="Tvalue">对象类型</typeparam>
        /// <param name="xmlStream">xml数据流</param>
        /// <returns></returns>
        public static Tvalue FromXml<Tvalue>(Stream xmlStream)
        {
            Tvalue result = default(Tvalue);
            if (xmlStream == null)
            {
                return result;
            }
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Tvalue));
                object serializeResult = xmlSerializer.Deserialize(xmlStream);
                Type resultType = serializeResult.GetType();
                if (resultType.GetInterface(nameof(IConvertible)) != null)
                {
                    result = (Tvalue)Convert.ChangeType(serializeResult, typeof(Tvalue));
                }
                else
                {
                    result = (Tvalue)serializeResult;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw ex;
            }
            return result;
        }
    }
}
