using System;
using System.Collections;
using System.Collections.Generic;

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
        public static object Get(IDictionary dictionary,string key)
        {
            return Get<object>(dictionary as IDictionary<string,object>, key);
        }

        /// <summary>
        /// 根据key获取键值对中对应的值
        /// </summary>
        /// <typeparam name="Tvalue">要取值的类型</typeparam>
        /// <param name="dictionary">要获取值的键值对</param>
        /// <param name="key">要获取值的键</param>
        /// <returns></returns>
        public static Tvalue Get<Tvalue>(IDictionary<string,Tvalue> dictionary,string key)
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
        public static Tvalue Get<Tkey,Tvalue>(IDictionary<Tkey,Tvalue> dictionary, Tkey key)
        {
            Tvalue result = default(Tvalue);
            if(dictionary != null && dictionary.ContainsKey(key))
            {
                try
                {
                    result = (Tvalue)Convert.ChangeType(dictionary[key], typeof(Tvalue));
                }
                catch (Exception ex)
                {
                    //TODO:
                }
            }
            return result;
        }
    }
}
