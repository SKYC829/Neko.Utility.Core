using Neko.Utility.Core.IO.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Neko.Utility.Core.Data
{
    /// <summary>
    /// <see cref="object"/>对象帮助类
    /// <para>包含了一些快速操作<see cref="object"/>对象的方法</para>
    /// </summary>
    public sealed partial class ObjectUtil
    {
        /// <summary>
        /// 判断一个对象<see cref="object"/>是否可以写入指定的类型中
        /// </summary>
        /// <param name="targetType">要写入的类型</param>
        /// <param name="value">要判断的对象<see cref="object"/></param>
        /// <returns></returns>
        public static bool CanWrite(Type targetType, object value)
        {
            if (!targetType.IsClass)
            {
                return true;
            }
            if (StringUtil.CompareType(targetType, typeof(string)))
            {
                return true;
            }
            if (StringUtil.IsNullOrEmpty(value))
            {
                return true;
            }
            if (value is JObject && targetType.IsClass)
            {
                return true;
            }
            return targetType.IsAssignableFrom(value.GetType());
        }

        /// <summary>
        /// 将<see cref="JObject"/>转换为数组
        /// </summary>
        /// <param name="list">要转换的对象</param>
        /// <returns></returns>
        private static IList ConvertList(IList list)
        {
            IList result = new List<object>();
            foreach (object item in list)
            {
                if (item is JObject)
                {
                    result.Add(DictionaryUtil.Convert(item));
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 将一个对象<paramref name="fromObject"/>的所有字段属性的值写入<paramref name="targetObject"/>
        /// </summary>
        /// <param name="fromObject">要写入的对象</param>
        /// <param name="targetObject">被写入的对象</param>
        /// <returns></returns>
        public static object WriteTo(object fromObject, object targetObject)
        {
            object result = targetObject;
            if (fromObject == null)
            {
                return result;
            }
            Type fromType = fromObject.GetType();
            PropertyInfo[] fromProperties = fromType.GetProperties();
            foreach (PropertyInfo property in fromProperties)
            {
                if (!property.CanWrite)
                {
                    continue;
                }
                object propertyValue = Get(fromObject, property.Name);
                if (propertyValue == null || propertyValue.GetType() == typeof(DBNull))
                {
                    propertyValue = null;
                }
                try
                {
                    if (CanWrite(property.PropertyType, propertyValue))
                    {
                        Set(result, property.Name, propertyValue);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            FieldInfo[] fromFields = fromType.GetFields();
            foreach (FieldInfo field in fromFields)
            {
                object fieldValue = Get(fromObject, field.Name);
                try
                {
                    if (CanWrite(field.FieldType, fieldValue))
                    {
                        Set(result, field.Name, fieldValue);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// 将一个对象转换为另一个对象
        /// <para>名称相同的属性/字段将会被赋值到目标对象<paramref name="targetObject"/></para>
        /// </summary>
        /// <typeparam name="TObject">目标对象<paramref name="targetObject"/>的类型</typeparam>
        /// <param name="fromObject">要转换的对象</param>
        /// <param name="targetObject">要被转换的对象</param>
        /// <returns></returns>
        public static TObject Convert<TObject>(object fromObject, TObject targetObject) where TObject : class
        {
            if (fromObject is TObject)
            {
                return fromObject as TObject;
            }

            if (targetObject == null)
            {
                return null;
            }

            WriteTo(fromObject, targetObject);
            return targetObject;
        }

        /// <summary>
        /// 将一个对象转换为另一个类型的对象
        /// <para>名称相同的属性/字段将会被赋值到目标对象</para>
        /// </summary>
        /// <typeparam name="TObject">要被转换到的类型</typeparam>
        /// <param name="fromObject">要转换的对象</param>
        /// <returns></returns>
        public static TObject Convert<TObject>(object fromObject) where TObject : class, new()
        {
            if (fromObject is TObject)
            {
                return fromObject as TObject;
            }

            TObject result = default(TObject);
            return Convert<TObject>(fromObject, result);
        }

        /// <summary>
        /// 将一组<see cref="IList"/>对象转换为<see cref="DataRow"/>添加到一个<see cref="DataTable"/>中
        /// </summary>
        /// <param name="dataTable">要添加的<see cref="DataTable"/></param>
        /// <param name="fromObjects">要转换的对象</param>
        /// <param name="offset"><see cref="DataRow"/>在<see cref="DataTable"/>中的偏移位置
        /// <para>如果<see cref="DataTable"/>的总行数小于此偏移位置则是新增在最后,否则修改已有的<see cref="DataRow"/></para>
        /// </param>
        /// <returns></returns>
        public static DataTable AddTable(DataTable dataTable, IList fromObjects, int offset)
        {
            if (dataTable == null)
            {
                dataTable = new DataTable();
            }

            foreach (object fromObject in fromObjects)
            {
                offset++;
                DataRow dataRow = null;
                if (dataTable.Rows.Count < offset)
                {
                    dataRow = dataTable.NewRow();
                    dataTable.Rows.Add(dataRow);
                }
                else
                {
                    dataRow = dataTable.Rows[offset];
                }
                WriteTo(fromObject, dataRow);
            }
            return dataTable;
        }

        /// <summary>
        /// 将一组<see cref="IList"/>对象转换为<see cref="DataRow"/>添加到一个<see cref="DataTable"/>中
        /// </summary>
        /// <param name="dataTable">要添加的<see cref="DataTable"/></param>
        /// <param name="fromObjects">要转换的对象</param>
        /// <returns></returns>
        public static DataTable AddTable(DataTable dataTable, IList fromObjects)
        {
            return AddTable(dataTable, fromObjects, 0);
        }

        /// <summary>
        /// 将一个对象转换为<see cref="DataRow"/>添加到一个<see cref="DataTable"/>中
        /// </summary>
        /// <param name="dataTable">要添加的<see cref="DataTable"/></param>
        /// <param name="fromObject">要转换的对象</param>
        /// <param name="offset"><see cref="DataRow"/>在<see cref="DataTable"/>中的偏移位置
        /// <para>如果<see cref="DataTable"/>的总行数小于此偏移位置则是新增在最后,否则修改已有的<see cref="DataRow"/></para>
        /// </param>
        /// <returns></returns>
        public static DataTable AddTable(DataTable dataTable, object fromObject, int offset)
        {
            return AddTable(dataTable, new[] { fromObject }, offset);
        }

        /// <summary>
        /// 将一个对象转换为<see cref="DataRow"/>添加到一个<see cref="DataTable"/>中
        /// </summary>
        /// <param name="dataTable">要添加的<see cref="DataTable"/></param>
        /// <param name="fromObject">要转换的对象</param>
        /// <returns></returns>
        public static DataTable AddTable(DataTable dataTable, object fromObject)
        {
            return AddTable(dataTable, fromObject, 0);
        }

        /// <summary>
        /// 将一个对象转换为<see cref="DataRow"/>添加到一个<see cref="DataTable"/>中
        /// </summary>
        /// <param name="fromObject">要转换的对象</param>
        /// <returns></returns>
        public static DataTable ToTable(object fromObject)
        {
            return AddTable(null, fromObject);
        }
    }
}
