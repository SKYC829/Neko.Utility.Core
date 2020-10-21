using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Neko.Utility.Core.Common
{
    public sealed partial class SerializeUtil
    {
        /// <summary>
        /// 将一个对象序列化为Json字符串
        /// </summary>
        /// <param name="fromObject">要序列化的对象</param>
        /// <returns></returns>
        public static string ToJson(object fromObject)
        {
            return ToJson(fromObject, false);
        }

        /// <summary>
        /// 将一个对象序列化为Json字符串
        /// </summary>
        /// <param name="fromObject">要序列化的对象</param>
        /// <param name="formatJson">是否格式化json</param>
        /// <returns></returns>
        public static string ToJson(object fromObject,bool formatJson)
        {
            return JsonConvert.SerializeObject(fromObject, formatJson ? Formatting.Indented : Formatting.None);
        }

        /// <summary>
        /// 将一个json字符串反序列化为一个对象
        /// </summary>
        /// <param name="jsonString">json字符串</param>
        /// <returns></returns>
        public static object FromJson(string jsonString)
        {
            return FromJson<object>(jsonString);
        }

        /// <summary>
        /// 将一个json字符串反序列化为一个对象
        /// </summary>
        /// <typeparam name="Tobject">对象类型</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns></returns>
        public static Tobject FromJson<Tobject>(string jsonString)
        {
            Tobject result = default(Tobject);
            try
            {
                result = JsonConvert.DeserializeObject<Tobject>(jsonString);
            }
            catch (JsonException ex)
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
        /// 获取Json中一个节点的值
        /// </summary>
        /// <typeparam name="Tvalue">要获取的值的类型</typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <param name="key">节点名称</param>
        /// <returns></returns>
        public static Tvalue GetJson<Tvalue>(string jsonString,string key)
        {
            Tvalue result = default(Tvalue);
            try
            {
                object jToken = GetJson(jsonString, key);
                Type tokenType = jToken.GetType();
                if(tokenType.GetInterface(nameof(IConvertible)) != null)
                {
                    result = (Tvalue)Convert.ChangeType(jToken, typeof(Tvalue));
                }
                else
                {
                    result = (Tvalue)jToken;
                }
            }
            catch (Exception ex)
            {
                //TODO:
            }
            return result;
        }

        /// <summary>
        /// 获取Json中一个节点的值
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        /// <param name="key">节点名称</param>
        /// <returns></returns>
        public static object GetJson(string jsonString,string key)
        {
            if(string.IsNullOrEmpty(jsonString) || string.IsNullOrEmpty(key))
            {
                return null;
            }
            object jToken = JsonConvert.DeserializeObject(jsonString);
            object result = null;
            try
            {
                result = ((JValue)((JObject)jToken)[key]).Value;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}
