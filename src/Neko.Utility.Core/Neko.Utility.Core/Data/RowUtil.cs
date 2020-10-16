using System;
using System.Data;

namespace Neko.Utility.Core.Data
{
    /// <summary>
    /// <see cref="DataRow"/>帮助类
    /// <para>可以对<see cref="DataRow"/>进行一些快速操作</para>
    /// </summary>
    public sealed class RowUtil
    {
        /// <summary>
        /// 设置<see cref="DataRow"/>中字段的值
        /// <para>如果字段不存在则会自动添加字段</para>
        /// </summary>
        /// <param name="dataRow">要设置值的<see cref="DataRow"/></param>
        /// <param name="columnName">字段的列名</param>
        /// <param name="fieldValue">字段的值</param>
        public static void Set(DataRow dataRow,string columnName,object fieldValue)
        {
            if(dataRow == null)
            {
                return;
            }
            DataTable parent = dataRow.Table;
            if (!parent.Columns.Contains(columnName))
            {
                AddColumn(parent, columnName);
            }
            dataRow[columnName] = fieldValue;
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的值
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static object Get(DataRow dataRow,string columnName)
        {
            object result = null;
            if(dataRow != null)
            {
                DataTable parent = dataRow.Table;
                if (parent.Columns.Contains(columnName))
                {
                    if(dataRow.RowState != DataRowState.Detached)
                    {
                        result = dataRow[columnName];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的值,并转换为指定类型
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <param name="valueType">要转换的类型</param>
        /// <returns></returns>
        public static object Get(DataRow dataRow,string columnName,Type valueType)
        {
            object result = Get(dataRow, columnName);
            if(result != null)
            {
                result = StringUtil.Get(valueType, result);
            }
            return result;
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的值,并转换为指定类型
        /// </summary>
        /// <typeparam name="Tvalue">要转换的类型</typeparam>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static Tvalue Get<Tvalue>(DataRow dataRow,string columnName)
        {
            object result = Get(dataRow, columnName);
            if (result != null)
            {
                result = StringUtil.Get<Tvalue>(result);
            }
            return (Tvalue)result == null ? default(Tvalue) : (Tvalue)result;
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的<see cref="string"/>值
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static string GetString(DataRow dataRow, string columnName)
        {
            return Get<string>(dataRow, columnName);
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的<see cref="bool"/>值
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static bool GetBoolean(DataRow dataRow, string columnName)
        {
            return Get<bool>(dataRow, columnName);
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的<see cref="int"/>值
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static int GetInt(DataRow dataRow, string columnName)
        {
            return Get<int>(dataRow, columnName);
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的<see cref="DateTime"/>值
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static DateTime? GetDateTime(DataRow dataRow, string columnName)
        {
            return Get<DateTime>(dataRow, columnName);
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的<see cref="double"/>值
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static double GetDouble(DataRow dataRow, string columnName)
        {
            return Get<double>(dataRow, columnName);
        }

        /// <summary>
        /// 获取<see cref="DataRow"/>中字段的<see cref="decimal"/>值
        /// </summary>
        /// <param name="dataRow">要获取值的<see cref="DataRow"/></param>
        /// <param name="columnName">要获取值的列名</param>
        /// <returns></returns>
        public static decimal GetDecimal(DataRow dataRow, string columnName)
        {
            return Get<decimal>(dataRow, columnName);
        }

        /// <summary>
        /// 获取<see cref="DataTable"/>中的第一个<see cref="DataRow"/>
        /// <para>如果<see cref="DataTable"/>中没有数据行或获取不到,则会返回null</para>
        /// </summary>
        /// <param name="dataTable">要获取<see cref="DataRow"/>的<see cref="DataTable"/></param>
        /// <returns></returns>
        public static DataRow GetFirstRow(DataTable dataTable)
        {
            return GetRow(0, dataTable);
        }

        /// <summary>
        /// 获取<see cref="DataTable"/>中指定位置的<see cref="DataRow"/>
        /// <para>如果<see cref="DataTable"/>中没有数据行或获取不到,则会返回null</para>
        /// </summary>
        /// <param name="index"><see cref="DataRow"/>的位置</param>
        /// <param name="dataTable">要获取<see cref="DataRow"/>的<see cref="DataTable"/></param>
        /// <returns></returns>
        public static DataRow GetRow(int index,DataTable dataTable)
        {
            DataRow result = null;
            if(dataTable != null && index >= 0)
            {
                if(index <= dataTable.Rows.Count)
                {
                    result = dataTable.Rows[index];
                }
            }
            return result;
        }

        /// <summary>
        /// 给<see cref="DataTable"/>添加列
        /// </summary>
        /// <param name="dataTable">要添加列的<see cref="DataTable"/></param>
        /// <param name="columnNames">要添加的列名(可以是多个)</param>
        public static void AddColumn(DataTable dataTable,params string[] columnNames)
        {
            DataColumn[] columns = new DataColumn[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
            {
                DataColumn column = new DataColumn(columnNames[i]);
                columns[i] = column;
            }
            AddColumn(dataTable, columns);
        }

        /// <summary>
        /// 给<see cref="DataTable"/>添加列
        /// </summary>
        /// <param name="dataTable">要添加列的<see cref="DataTable"/></param>
        /// <param name="dataColumns">要添加的列名(可以是多个)</param>
        public static void AddColumn(DataTable dataTable,params DataColumn[] dataColumns)
        {
            dataTable.Columns.AddRange(dataColumns);
        }
    }
}
