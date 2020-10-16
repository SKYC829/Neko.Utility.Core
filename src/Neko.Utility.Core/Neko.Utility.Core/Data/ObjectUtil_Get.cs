using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Neko.Utility.Core.Data
{
    /// <summary>
    /// <see cref="object"/>对象帮助类(关于从对象获取值的部分)
    /// <para>包含了一些快速操作<see cref="object"/>对象的方法</para>
    /// </summary>
    public sealed partial class ObjectUtil
    {
        /// <summary>
        /// 获取一个对象<see cref="object"/>中指定字段的值
        /// </summary>
        /// <param name="target">要取值的对象<see cref="object"/></param>
        /// <param name="fieldName">要取值的属性/字段名称</param>
        /// <returns></returns>
        public static object Get(object target, string fieldName)
        {
            object result = null;
            if (target == null)
            {
                return result;
            }
            Type targetType = target.GetType();
            if (targetType.IsValueType || targetType.IsEnum)
            {
                result = null;
            }
            else if (target is IDictionary)
            {
                result = DictionaryUtil.Get(target as IDictionary, fieldName);
            }
            else if (target is DataRow)
            {
                result = RowUtil.Get(target as DataRow, fieldName);
            }
            else if (target is JObject)
            {
                result = Get((target as JObject)[fieldName]);
            }
            else if (targetType.IsClass)
            {
                PropertyInfo property = targetType.GetProperty(fieldName);
                if (property != null)
                {
                    result = property.GetValue(target);
                }
                else
                {
                    FieldInfo field = targetType.GetField(fieldName);
                    if (field != null)
                    {
                        result = field.GetValue(target);
                    }
                }
            }

            if (result != null)
            {
                Type resultType = result.GetType();
                Type resultGenericType = null;
                if (resultType.IsGenericType)
                {
                    resultGenericType = resultType.GetGenericArguments().FirstOrDefault(); //获取第一个泛型类型
                }
                if (result is JArray || result is ArrayList || (resultGenericType != null && (StringUtil.CompareType(resultGenericType, typeof(JObject)) || StringUtil.CompareType(resultGenericType, "System.Object"))))
                {
                    IList resultList = result as IList; //处理Json数组
                    resultList = ConvertList(resultList);
                    if (!(target is JObject))
                    {
                        Set(target, fieldName, resultList);
                    }
                    result = resultList;
                }
                else if (result is JObject)
                {
                    result = DictionaryUtil.Convert(result);
                }
            }
            return result;
        }

        /// <summary>
        /// 将<see cref="object"/>转换为以下类型
        /// <list type="bullet">
        /// <item><see cref="JToken"/></item>
        /// <item><see cref="JValue"/></item>
        /// <item><see cref="JProperty"/></item>
        /// </list>
        /// </summary>
        /// <param name="target">要转换的对象<see cref="object"/></param>
        /// <returns></returns>
        public static object Get(object target)
        {
            object result = target;
            if (target is JToken)
            {
                JToken token = target as JToken;
                if (token == null)
                {
                    result = null;
                }
                else if (token is JValue value)
                {
                    result = value.Value;
                }
                else if (token is JProperty property)
                {
                    result = property.Value;
                }
                else
                {
                    result = token;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取一个对象<see cref="object"/>中指定字段的值
        /// </summary>
        /// <typeparam name="Tvalue">要转换的类型</typeparam>
        /// <param name="target">要取值的对象<see cref="object"/></param>
        /// <param name="fieldName">要取值的属性/字段名称</param>
        /// <returns></returns>
        public static Tvalue Get<Tvalue>(object target, string fieldName)
        {
            object result = Get(target, fieldName);
            return StringUtil.Get<Tvalue>(result);
        }

        /// <summary>
        /// 获取一个对象<see cref="object"/>中指定字段的<see cref="string"/>值
        /// </summary>
        /// <param name="target">要取值的对象</param>
        /// <param name="fieldName">要取值的属性/字段名称</param>
        /// <returns></returns>
        public static string GetString(object target, string fieldName)
        {
            return Get<string>(target, fieldName);
        }

        /// <summary>
        /// 获取一个对象<see cref="object"/>中指定字段的<see cref="double"/>值
        /// </summary>
        /// <param name="target">要取值的对象</param>
        /// <param name="fieldName">要取值的属性/字段名称</param>
        /// <returns></returns>
        public static double GetDouble(object target, string fieldName)
        {
            return Get<double>(target, fieldName);
        }

        /// <summary>
        /// 获取一个对象<see cref="object"/>中指定字段的<see cref="decimal"/>值
        /// </summary>
        /// <param name="target">要取值的对象</param>
        /// <param name="fieldName">要取值的属性/字段名称</param>
        /// <returns></returns>
        public static decimal GetDecimal(object target, string fieldName)
        {
            return Get<decimal>(target, fieldName);
        }

        /// <summary>
        /// 获取一个对象<see cref="object"/>中指定字段的<see cref="bool"/>值
        /// </summary>
        /// <param name="target">要取值的对象</param>
        /// <param name="fieldName">要取值的属性/字段名称</param>
        /// <returns></returns>
        public static bool GetBoolean(object target, string fieldName)
        {
            return Get<bool>(target, fieldName);
        }

        /// <summary>
        /// 获取一个对象<see cref="object"/>中指定字段的<see cref="DateTime"/>值
        /// </summary>
        /// <param name="target">要取值的对象</param>
        /// <param name="fieldName">要取值的属性/字段名称</param>
        /// <returns></returns>
        public static DateTime? GetDateTime(object target, string fieldName)
        {
            return Get<DateTime>(target, fieldName);
        }
    }
}
