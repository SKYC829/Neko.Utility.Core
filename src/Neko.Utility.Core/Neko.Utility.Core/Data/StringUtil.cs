using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Neko.Utility.Core.Data
{
    /// <summary>
    /// 字符串帮助类
    /// <para>可以对字符串进行一些快速操作</para>
    /// </summary>
    public sealed class StringUtil
    {
        /// <summary>
        /// 判断一个对象是否为空
        /// </summary>
        /// <param name="value">要判断的对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(object value)
        {
            return value == null || string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString());
        }

        /// <summary>
        /// 对比两个类型
        /// </summary>
        /// <param name="fromType">要对比的类型1</param>
        /// <param name="typeName">要对比的类型2的类型名称<see cref="Type.FullName"/></param>
        /// <returns></returns>
        public static bool CompareType(Type fromType, string typeName)
        {
            if (fromType == null)
            {
                return false;
            }
            else if (fromType.ToString() == typeName || fromType.Name == typeName)
            {
                return true;
            }
            else if (fromType.ToString() == "System.Object" || fromType.Name == "Object")
            {
                return false;
            }
            else
            {
                return CompareType(fromType.BaseType, typeName);
            }
        }

        /// <summary>
        /// 对比两个类型
        /// </summary>
        /// <param name="fromType">要对比的类型1</param>
        /// <param name="toType">要对比的类型2</param>
        /// <returns></returns>
        public static bool CompareType(Type fromType, Type toType)
        {
            return CompareType(fromType, toType.ToString());
        }

        /// <summary>
        /// 比较两个对象是否相等
        /// </summary>
        /// <param name="fromValue">要比较的对象1</param>
        /// <param name="toValue">要比较的对象2</param>
        /// <returns></returns>
        public static bool SafeCompare(object fromValue, object toValue)
        {
            return SafeCompare(fromValue, toValue, false);
        }

        /// <summary>
        /// 比较两个字符串是否相等
        /// </summary>
        /// <param name="fromValue">要比较的字符串1</param>
        /// <param name="toValue">要比较的字符串2</param>
        /// <returns></returns>
        public static bool SafeCompare(string fromValue, string toValue)
        {
            return SafeCompare(fromValue, toValue, false);
        }

        /// <summary>
        /// 比较两个对象是否相等
        /// <para>将会把两个对象转换为字符串<see cref="Object.ToString()"/>来进行比较</para>
        /// </summary>
        /// <param name="fromValue">要比较的对象1</param>
        /// <param name="toValue">要比较的对象2</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool SafeCompare(object fromValue, object toValue, bool ignoreCase)
        {
            string fromStr = string.Empty, toStr = string.Empty;
            if (fromValue != null)
            {
                fromStr = fromValue.ToString();
            }
            if (toValue != null)
            {
                toStr = toValue.ToString();
            }
            return SafeCompare(fromStr, toStr, ignoreCase);
        }

        /// <summary>
        /// 比较两个字符串是否相等
        /// </summary>
        /// <param name="fromValue">要比较的字符串1</param>
        /// <param name="toValue">要比较的字符串2</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool SafeCompare(string fromValue, string toValue, bool ignoreCase)
        {
            return !IsNullOrEmpty(fromValue) && !IsNullOrEmpty(toValue) && fromValue.Length == toValue.Length && (string.Compare(fromValue, toValue, ignoreCase, System.Globalization.CultureInfo.InvariantCulture) == 0);
        }

        /// <summary>
        /// 获取当前UTC时间的十三位的Unix时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            return GetTimeStamp(DateTime.UtcNow);
        }

        /// <summary>
        /// 获取指定时间的十三位的Unix时间戳
        /// </summary>
        /// <param name="time">要获取时间戳的时间</param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime time)
        {
            TimeSpan span = time - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(span.TotalSeconds * 1000).ToString();
        }

        /// <summary>
        /// 将十三位的Unix时间戳转换为日期时间
        /// </summary>
        /// <param name="timeStamp">要转换的Unix时间戳</param>
        /// <returns></returns>
        public static DateTime GetTimeStamp(string timeStamp)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                long unixDate = Convert.ToInt64(timeStamp);
                DateTime utcStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                result = utcStart.AddMilliseconds(unixDate).ToLocalTime();
            }
            catch (Exception ex)
            {
                //TODO:
            }
            return result;
        }

        /// <summary>
        /// 将对象转换为特定类型的格式
        /// <list type="bullet">
        /// <item>转换失败将返回类型的默认值</item>
        /// <item>无法转换将返回对象本身</item>
        /// </list>
        /// </summary>
        /// <param name="targetType">指定的类型</param>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static object Get(Type targetType, object value)
        {
            return InternalGet(targetType, value);
        }

        /// <summary>
        /// 将对象转换为特定类型的格式
        /// <list type="bullet">
        /// <item>转换失败将返回类型的默认值</item>
        /// <item>无法转换将返回对象本身</item>
        /// </list>
        /// </summary>
        /// <typeparam name="TObject">指定的类型</typeparam>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static TObject Get<TObject>(object value)
        {
            return (TObject)Get(typeof(TObject), value);
        }

        /// <summary>
        /// 将对象转换为<see cref="string"/>类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static string GetString(object value)
        {
            value = ObjectUtil.Get(value);
            return value == null ? string.Empty : value.ToString();
        }

        /// <summary>
        /// 将对象转换为<see cref="bool"/>类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static bool GetBoolean(object value)
        {
            return GetBoolean(value, false);
        }

        /// <summary>
        /// 将对象转换为<see cref="bool"/>类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <param name="defaultValue">转换失败时返回的默认值</param>
        /// <returns></returns>
        public static bool GetBoolean(object value,bool defaultValue)
        {
            bool result = defaultValue;
            try
            {
                if (!IsNullOrEmpty(value))
                {
                    result = !SafeCompare(value, "0", true) && !SafeCompare(value, "false", true) && !SafeCompare(value, "f", true) && !SafeCompare(value, "no", true) && !SafeCompare(value, "n", true) && !SafeCompare(value, "否", true) && !SafeCompare(value, "错", true) && !SafeCompare(value, "错误", true) && !SafeCompare(value, "不", true) && !SafeCompare(value, "不是", true) && !IsNullOrEmpty(value);
                }
            }
            catch (Exception ex)
            {
                //TODO:
            }
            return result;
        }

        /// <summary>
        /// 将对象转换为<see cref="int"/>类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static int GetInt(object value)
        {
            return InternalGet<int>(value);
        }

        /// <summary>
        /// 将对象转换为<see cref="DateTime?"/>类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static DateTime? GetDateTime(object value)
        {
            return InternalGet<DateTime?>(value);
        }

        /// <summary>
        /// 将对象转换为<see cref="double"/>类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static double GetDouble(object value)
        {
            return InternalGet<double>(value);
        }

        /// <summary>
        /// 将对象转换为<see cref="decimal"/>类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static decimal GetDecimal(object value)
        {
            return InternalGet<decimal>(value);
        }

        internal static Tvalue InternalGet<Tvalue>(object value)
        {
            Tvalue result = default(Tvalue);
            try
            {
                if (!IsNullOrEmpty(value))
                {
                    result = (Tvalue)InternalGet(typeof(Tvalue), value);
                }
            }
            catch (Exception ex)
            {
                //TODO:
            }
            return result;
        }

        internal static object InternalGet(Type targetType,object value)
        {
            object result = null;
            //根据类型调用对应的方法转换数据
            if (targetType.IsEnum)
            {
                string valueStr = GetString(value);
                result = EnumUtil.Convert(targetType, valueStr);
            }
            else if (CompareType(targetType, typeof(string)))
            {
                result = GetString(value);
            }
            else if (CompareType(targetType, typeof(bool)))
            {
                string valueStr = GetString(value);
                result = GetBoolean(valueStr);
            }
            else if (CompareType(targetType, typeof(int)))
            {
                result = GetInt(value);
            }
            else if (CompareType(targetType, typeof(double)))
            {
                result = GetDouble(value);
            }
            else if (CompareType(targetType, typeof(decimal)))
            {
                result = GetDecimal(value);
            }
            else if (CompareType(targetType, typeof(DateTime)) || CompareType(targetType, typeof(DateTime?)))
            {
                result = GetDateTime(value);
            }
            else if (targetType.IsClass && value is JObject) //如果转换的类型是Class并且是JObject类型,先将来源对象序列化为Json字符串，然后再反序列化为要转换的类型
            {
                result = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), targetType);
            }
            else //如果都不是上面的类型，则返回对象本身
            {
                result = value;
            }
            return result;
        }
    }
}
