using Microsoft.Extensions.DependencyInjection;
using Neko.Utility.Core.Common;
using Neko.Utility.Core.Data;
using Neko.Utility.Core.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neko.Utility.Core
{
    /// <summary>
    /// 封装的扩展方法
    /// </summary>
    public static class ExtensionCodes
    {
        /// <summary>
        /// <inheritdoc cref="StringUtil.GetTimeStamp(DateTime)"/>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetTimeStamp(this DateTime dateTime)
        {
            return StringUtil.GetTimeStamp(dateTime);
        }

        /// <summary>
        /// 将字符串转换为时间或将时间戳转换为时间
        /// </summary>
        /// <param name="datetimeString"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(this string datetimeString)
        {
            DateTime? result = null;
            try
            {
                //尝试转换时间戳
                result = StringUtil.GetTimeStamp(datetimeString);
            }
            catch
            {
                result = StringUtil.GetDateTime(datetimeString);
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="RowUtil.AddColumn(DataTable, string[])"/>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columns">要添加的列</param>
        public static void AddColumn(this DataTable dataTable,params string[] columns)
        {
            RowUtil.AddColumn(dataTable, columns);
        }

        /// <summary>
        /// <inheritdoc cref="RowUtil.AddColumn(DataTable, string[])"/>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="entityType">要添加到<see cref="DataTable"/>的对象的类型</param>
        public static void AddColumn(this DataTable dataTable,Type entityType)
        {
            if(dataTable == null)
            {
                return;
            }

            foreach (PropertyInfo property in entityType.GetProperties())
            {
                RowUtil.AddColumn(dataTable, property.Name);
            }

            foreach (FieldInfo field in entityType.GetFields())
            {
                string columnName = field.Name;
                if (dataTable.Columns.Contains(columnName))
                {
                    int repeatCount = dataTable.Columns.Cast<DataColumn>().Count(p => p.ColumnName.ToLower().Equals(columnName));
                    columnName = string.Format("{0}_{1}", field.Name, repeatCount);
                    RowUtil.AddColumn(dataTable, columnName);
                }
            }
        }

        /// <summary>
        /// 克隆一个<see cref="DataRow"/>
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static DataRow Clone(this DataRow dataRow)
        {
            if(dataRow == null)
            {
                return null;
            }
            DataRow result = dataRow.Table.NewRow();
            result.ItemArray = dataRow.ItemArray;
            return result;
        }

        /// <summary>
        /// 将一个<see cref="DataRow"/>复制到一个<see cref="DataTable"/>中
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="dataTable"><see cref="DataTable"/></param>
        public static void Copy(this DataRow dataRow,DataTable dataTable)
        {
            if(dataRow == null)
            {
                return;
            }
            if(dataTable == null)
            {
                dataTable = new DataTable();
            }
            RowUtil.AddColumn(dataTable, dataRow.Table.Columns.Cast<DataColumn>().ToArray());
            DataRow templateRow = dataTable.NewRow();
            templateRow.ItemArray = dataRow.ItemArray;
            dataTable.Rows.Add(templateRow);
        }

        /// <summary>
        /// <inheritdoc cref="SerializeUtil.ToBinary(object)"/>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] ObjectToBinary(this object content)
        {
            return SerializeUtil.ToBinary(content);
        }

        /// <summary>
        /// <inheritdoc cref="SerializeUtil.FromBinary{Tobject}(byte[])"/>
        /// </summary>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="binaryBytes">二进制数组</param>
        /// <returns></returns>
        public static Tvalue BinaryToObject<Tvalue>(this byte[] binaryBytes)
        {
            return SerializeUtil.FromBinary<Tvalue>(binaryBytes);
        }

        /// <summary>
        /// <inheritdoc cref="EncryptionUtil.EncryptMD5(object)"/>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string MD5(this object content)
        {
            return EncryptionUtil.EncryptMD5(content);
        }

        /// <summary>
        /// <inheritdoc cref="EncryptionUtil.EncryptSHA1(object)"/>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SHA1(this object content)
        {
            return EncryptionUtil.EncryptSHA1(content);
        }

        /// <summary>
        /// <inheritdoc cref="EncryptionUtil.EncryptSHA256(object)"/>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SHA256(this object content)
        {
            return EncryptionUtil.EncryptSHA256(content);
        }

        /// <summary>
        /// <inheritdoc cref="CompressUtil.CompressBytes(byte[])"/>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] bytes)
        {
            return CompressUtil.CompressBytes(bytes);
        }

        /// <summary>
        /// <inheritdoc cref="CompressUtil.DeCompressBytes(byte[])"/>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] DeCompress(this byte[] bytes)
        {
            return CompressUtil.DeCompressBytes(bytes);
        }

        /// <summary>
        /// <inheritdoc cref="DictionaryUtil.SortDictionary{Tkey, Tvalue}(IDictionary{Tkey, Tvalue}, bool)"/>
        /// </summary>
        /// <typeparam name="TKey">键值对的键</typeparam>
        /// <typeparam name="TValue">键值对的值</typeparam>
        /// <param name="dictionary">要排序的键值对</param>
        /// <param name="sortByValue">是否根据值进行排序<para>为true时按照Value排序,否则按照Key排序</para></param>
        /// <returns></returns>
        public static IList<KeyValuePair<TKey,TValue>> Sort<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,bool sortByValue = true) where TValue : struct
        {
            return DictionaryUtil.SortDictionary<TKey, TValue>(dictionary, sortByValue);
        }

        /// <summary>
        /// 从程序集批量依赖注入
        /// </summary>
        /// <typeparam name="TBaseInterface">依赖注入的所有接口的基接口</typeparam>
        /// <param name="services">服务集</param>
        /// <param name="instanceAssemblyName">依赖注入的接口所在的程序集文件名称</param>
        /// <param name="abstractAssemblyName">依赖注入的实例所在的程序集文件名称</param>
        /// <returns></returns>
        public static IServiceCollection AutoDependencyInject<TBaseInterface>(this IServiceCollection services, string instanceAssemblyName, string abstractAssemblyName)
        {
            Type baseInterfaceType = typeof(TBaseInterface);
            Assembly injectAbstractAssembly = Assembly.LoadFrom(new FileInfo(abstractAssemblyName).FullName);
            Assembly injectInstanceAssembly = Assembly.LoadFrom(new FileInfo(instanceAssemblyName).FullName);
            if (injectAbstractAssembly == null)
            {
                throw new FileNotFoundException("无法加载程序集:[{0}],文件不存在!", abstractAssemblyName);
            }
            if (injectInstanceAssembly == null)
            {
                throw new FileNotFoundException("无法加载程序集:[{0}],文件不存在!", instanceAssemblyName);
            }
            IQueryable<Type> injectAbstractTypes = injectAbstractAssembly.GetTypes().Where(p => p.IsInterface && baseInterfaceType.IsAssignableFrom(p)).AsQueryable(); //得到所有要注入的实现了基类的接口
            foreach (Type injectAbstractType in injectAbstractTypes)
            {
                IQueryable<Type> injectInstanceTypes = injectInstanceAssembly.GetTypes().Where(p => p.IsClass && injectAbstractType.IsAssignableFrom(p) && !p.IsAbstract).AsQueryable(); //得到所有实现了injectAbstractType接口的不是抽象类的类
                foreach (Type injectInstanceType in injectInstanceTypes)
                {
                    IQueryable<Type> interfaceTypes = injectInstanceType.GetInterfaces().Where(p => p != baseInterfaceType).AsQueryable(); //得到要注入的类实现的所有不是基接口的接口
                    foreach (Type interfaceType in interfaceTypes)
                    {
                        //在不考虑性能的情况下,使用AddScoped注入所有对象,如果要考虑性能的话,可以增加一个自定义特性,在要注入的类上边声明特性,然后这里根据特性的类型来判断注入类型
                        services.AddScoped(interfaceType, injectInstanceType);
                    }
                }
            }
            return services;
        }
    }
}
