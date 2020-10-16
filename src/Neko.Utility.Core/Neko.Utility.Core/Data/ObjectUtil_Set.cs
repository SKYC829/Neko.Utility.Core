using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace Neko.Utility.Core.Data
{
    /// <summary>
    /// <see cref="object"/>对象帮助类(关于设置对象的值的部分)
    /// <para>包含了一些快速操作<see cref="object"/>对象的方法</para>
    /// </summary>
    public sealed partial class ObjectUtil
    {
        /// <summary>
        /// 设置一个对象<see cref="object"/>的某个字段的值
        /// </summary>
        /// <param name="target">要设置的对象<see cref="object"/></param>
        /// <param name="fieldName">要设置值的字段名称</param>
        /// <param name="fieldValue">要设置的值</param>
        public static void Set(object target, string fieldName, object fieldValue)
        {
            if (target == null)
            {
                return;
            }
            Type targetType = target.GetType();
            if (targetType.IsValueType || targetType.IsEnum) //IEnumerable的IsEnum居然是false
            {
                return;
            }
            else if (target is IDictionary)
            {
                (target as IDictionary)[fieldName] = fieldValue;
            }
            else if (target is DataRow)
            {
                RowUtil.Set(target as DataRow, fieldName, fieldValue);
            }
            else if (target is DataTable)
            {
                DataTable dataTable = target as DataTable;
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Set(row, fieldName, fieldValue);
                    }
                }
            }
            else if (target is JObject)
            {
                (target as JObject)[fieldName] = new JValue(fieldValue);
            }
            else if (targetType.IsClass)
            {
                Set(targetType, target, fieldName, fieldValue);
            }
        }

        /// <summary>
        /// 设置一个对象<see cref="object"/>的某个字段的值
        /// </summary>
        /// <param name="targetType">要设置的对象<see cref="object"/>的类型</param>
        /// <param name="target">要设置的对象<see cref="object"/></param>
        /// <param name="fieldName">要设置值的字段名称</param>
        /// <param name="fieldValue">要设置的值</param>
        public static void Set(Type targetType, object target, string fieldName, object fieldValue)
        {
            PropertyInfo property = targetType.GetProperty(fieldName);
            if (property != null)
            {
                Type propertyType = property.PropertyType;
                object propertyValue = StringUtil.Get(propertyType, fieldValue);
                property.SetValue(target, propertyValue);
                return;
            }
            FieldInfo field = targetType.GetField(fieldName);
            if (field != null)
            {
                Type fieldType = field.FieldType;
                field.SetValue(target, StringUtil.Get(fieldType, fieldValue));
            }
        }
    }
}
