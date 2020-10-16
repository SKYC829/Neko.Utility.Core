using System;

namespace Neko.Utility.Core.Data
{
    /// <summary>
    /// 枚举帮助类
    /// <para>可以快速转换一些枚举类型</para>
    /// </summary>
    public sealed class EnumUtil
    {
        /// <summary>
        /// 将一个对象转换为枚举类型
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">要转换的对象</param>
        /// <param name="defaultValue">当转换失败时返回的默认值</param>
        /// <returns></returns>
        public static TEnum Convert<TEnum>(string value, TEnum defaultValue) where TEnum : struct
        {
            return (TEnum)Convert(typeof(TEnum), value, defaultValue);
        }

        /// <summary>
        /// 将一个对象转换为枚举类型
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static TEnum Convert<TEnum>(string value) where TEnum : struct
        {
            return Convert<TEnum>(value, default(TEnum));
        }

        /// <summary>
        /// 将一个对象转换为枚举类型
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="value">要转换的对象</param>
        /// <param name="defaultValue">当转换失败时返回的默认值</param>
        /// <returns></returns>
        public static object Convert(Type enumType, string value, object defaultValue)
        {
            object result = defaultValue;
            if (enumType.IsEnum)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        result = Enum.Parse(enumType, value);
                    }
                    catch (Exception ex)
                    {
                        //TODO:
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将一个对象转换为枚举类型
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="value">要转换的对象</param>
        /// <returns></returns>
        public static object Convert(Type enumType, string value)
        {
            return Convert(enumType, value, 0);
        }
    }
}
