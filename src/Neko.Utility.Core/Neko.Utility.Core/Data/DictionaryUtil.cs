using Neko.Utility.Core.IO.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Neko.Utility.Core.Data
{
    /// <summary>
    /// 字典帮助类
    /// <para>可以对键值对进行一些快速操作</para>
    /// </summary>
    public sealed class DictionaryUtil
    {
        /// <summary>
        /// 根据key获取键值对中对应的值
        /// </summary>
        /// <param name="dictionary">要获取值的键值对</param>
        /// <param name="key">要获取值的键</param>
        /// <returns></returns>
        public static object Get(IDictionary dictionary, string key)
        {
            return Get<object>(dictionary as IDictionary<string, object>, key);
        }

        /// <summary>
        /// 根据key获取键值对中对应的值
        /// </summary>
        /// <typeparam name="Tvalue">要取值的类型</typeparam>
        /// <param name="dictionary">要获取值的键值对</param>
        /// <param name="key">要获取值的键</param>
        /// <returns></returns>
        public static Tvalue Get<Tvalue>(IDictionary<string, Tvalue> dictionary, string key)
        {
            return Get<string, Tvalue>(dictionary, key);
        }

        /// <summary>
        /// 根据key获取键值对中对应的值
        /// </summary>
        /// <typeparam name="Tkey">键值对键的类型</typeparam>
        /// <typeparam name="Tvalue">要取值的类型</typeparam>
        /// <param name="dictionary">要获取值的键值对</param>
        /// <param name="key">要获取值的键</param>
        /// <returns></returns>
        public static Tvalue Get<Tkey, Tvalue>(IDictionary<Tkey, Tvalue> dictionary, Tkey key)
        {
            Tvalue result = default(Tvalue);
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                try
                {
                    result = (Tvalue)System.Convert.ChangeType(dictionary[key], typeof(Tvalue));
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidCastException("无法将对象转换为" + typeof(Tvalue), ex);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 将一个对象<see cref="object"/>转换为字典
        /// <para>当<see cref="object"/>为类,且类包含方法时,不建议转换为字典</para>
        /// </summary>
        /// <param name="target">要转换的对象</param>
        /// <returns></returns>
        public static IDictionary Convert(object target)
        {
            IDictionary result = null;
            try
            {
                if (target == null)
                {
                    return result;
                }
                if (target is IDictionary)
                {
                    result = target as IDictionary;
                }
                else
                {
                    string dicJson = JsonConvert.SerializeObject(target);
                    result = JsonConvert.DeserializeObject<IDictionary>(dicJson);
                }
            }
            finally
            {
                if (result == null)
                {
                    result = new Dictionary<string, object>();
                    Type targetType = target.GetType();
                    if (target is JObject)
                    {
                        JObject jObject = target as JObject;
                        foreach (KeyValuePair<string, JToken> token in jObject)
                        {
                            string key = token.Key;
                            if (token.Value is JValue value)
                            {
                                result[key] = value.Value;
                            }
                            else
                            {
                                result[key] = null;
                            }
                        }
                    }
                    else if (targetType.IsClass)
                    {
                        PropertyInfo[] properties = targetType.GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (!property.CanRead)
                            {
                                continue;
                            }
                            result[property.Name] = property.GetValue(target);
                        }
                        FieldInfo[] fields = targetType.GetFields();
                        foreach (FieldInfo field in fields)
                        {
                            string fieldName = field.Name;
                            if (result.Contains(fieldName))
                            {
                                fieldName = string.Format("fi_{0}", fieldName);
                            }
                            result[fieldName] = field.GetValue(target);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据键值对的Key或Value进行排序
        /// <para>使用此方法时,排序字段必须要能够进行减法运算</para>
        /// </summary>
        /// <typeparam name="Tkey">键值对的键</typeparam>
        /// <typeparam name="Tvalue">键值对的值</typeparam>
        /// <param name="dictionary">要排序的键值对</param>
        /// <param name="sortByValue">是否根据值进行排序<para>为true时按照Value排序,否则按照Key排序</para></param>
        /// <returns></returns>
        public static IList<KeyValuePair<Tkey, Tvalue>> SortDictionary<Tkey, Tvalue>(IDictionary<Tkey, Tvalue> dictionary, bool sortByValue = true) where Tvalue : struct
        {
            List<KeyValuePair<Tkey, Tvalue>> results = new List<KeyValuePair<Tkey, Tvalue>>();
            if (dictionary == null || dictionary.Count <= 0)
            {
                return results;
            }
            results.AddRange(dictionary);
            try
            {
                results.Sort(delegate (KeyValuePair<Tkey, Tvalue> previewItem, KeyValuePair<Tkey, Tvalue> nextItem)
                {
                    dynamic previewKey = previewItem.Key;
                    dynamic nextKey = nextItem.Key;
                    dynamic previewValue = previewItem.Value;
                    dynamic nextValue = nextItem.Value;
                    if (sortByValue)
                    {
                        return (int)(nextValue - previewValue);
                    }
                    else
                    {
                        return (int)(nextKey - previewKey);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
    }
}
