using Neko.Utility.Core.Data;
using Neko.Utility.Core.IO.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Neko.Utility.Core.IO
{
    /// <summary>
    /// Xml帮助类
    /// <para>可以快速获取一个<see cref="XmlElement"/>或者快速从<see cref="XmlElement"/>中获取<see cref="XmlAttribute"/>的值</para>
    /// </summary>
    public sealed class XmlUtil
    {
        /// <summary>
        /// 判断一个<see cref="XmlNodeList"/>是否为空
        /// </summary>
        /// <param name="nodeList">xml节点的集合</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(XmlNodeList nodeList)
        {
            return nodeList == null || nodeList.Count == 0;
        }

        /// <summary>
        /// 根据特性获取一个Xml节点
        /// </summary>
        /// <param name="xmlNode">xml的根节点</param>
        /// <param name="attributeName">特性的名称</param>
        /// <param name="attributeValue">特性的值</param>
        /// <returns></returns>
        public static XmlElement GetElement(XmlNode xmlNode, string attributeName, string attributeValue)
        {
            if (xmlNode == null)
            {
                return null;
            }

            if (StringUtil.IsNullOrEmpty(attributeName))
            {
                return null;
            }

            XmlElement result = xmlNode.ChildNodes.Cast<XmlElement>().Where(p => p != null && p.HasAttribute(attributeName) && p.GetAttribute(attributeName).Equals(attributeValue)).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="GetElementsByTag(XmlNode, string[])"/>
        /// </summary>
        /// <param name="xmlNode">xml的根节点</param>
        /// <param name="tag">xml的标签</param>
        /// <returns></returns>
        public static XmlElement GetElementByTag(XmlNode xmlNode, string tag)
        {
            return GetElementsByTag(xmlNode, tag).FirstOrDefault();
        }

        /// <summary>
        /// 根据xml标签获取xml节点
        /// </summary>
        /// <param name="xmlNode">xml的根节点</param>
        /// <param name="tags">xml的标签</param>
        /// <returns></returns>
        public static IEnumerable<XmlElement> GetElementsByTag(XmlNode xmlNode, params string[] tags)
        {
            List<XmlElement> result = new List<XmlElement>();
            if (xmlNode == null)
            {
                return result;
            }
            if (tags == null || tags.Length == 0)
            {
                result.AddRange(xmlNode.ChildNodes.Cast<XmlElement>());
            }
            else
            {
                result.AddRange(xmlNode.ChildNodes.Cast<XmlElement>().Where(p => Array.IndexOf(tags, p.Name) > -1));
            }
            return result;
        }

        /// <summary>
        /// 获取一个Xml元素上一个特性的值
        /// </summary>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <returns></returns>
        public static string Get(XmlElement xmlElement,string attributeName)
        {
            string result = string.Empty;
            if(xmlElement == null)
            {
                return result;
            }
            if (StringUtil.IsNullOrEmpty(attributeName))
            {
                return result;
            }
            try
            {
                result = xmlElement.GetAttribute(attributeName);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            return result;
        }

        /// <summary>
        /// 设置一个Xml元素上一个特性的值
        /// </summary>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <param name="attributeValue">要设置的值</param>
        public static void Set(XmlElement xmlElement,string attributeName,object attributeValue)
        {
            if(xmlElement == null)
            {
                return;
            }
            if (StringUtil.IsNullOrEmpty(attributeName))
            {
                return;
            }
            xmlElement.SetAttribute(attributeName, StringUtil.GetString(attributeValue));
        }

        /// <summary>
        /// <inheritdoc cref="Get(XmlElement, string)"/>并转换为指定类型
        /// </summary>
        /// <typeparam name="Tvalue">要转换的类型</typeparam>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <returns></returns>
        public static Tvalue Get<Tvalue>(XmlElement xmlElement,string attributeName)
        {
            string attributeValue = Get(xmlElement, attributeName);
            Tvalue result = default(Tvalue);
            try
            {
                if (typeof(Tvalue).IsEnum)
                {
                    result = (Tvalue)Enum.Parse(typeof(Tvalue),attributeValue);
                }
                else
                {
                    result = (Tvalue)Convert.ChangeType(attributeValue, typeof(Tvalue));
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="Get{Tvalue}(XmlElement, string)"/>
        /// </summary>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <returns></returns>
        public static bool GetBool(XmlElement xmlElement,string attributeName)
        {
            return Get<bool>(xmlElement, attributeName);
        }

        /// <summary>
        /// <inheritdoc cref="Get{Tvalue}(XmlElement, string)"/>
        /// </summary>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <returns></returns>
        public static int GetInt(XmlElement xmlElement,string attributeName)
        {
            return Get<int>(xmlElement, attributeName);
        }

        /// <summary>
        /// <inheritdoc cref="Get{Tvalue}(XmlElement, string)"/>
        /// </summary>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <returns></returns>
        public static decimal GetDecimal(XmlElement xmlElement, string attributeName)
        {
            return Get<decimal>(xmlElement, attributeName);
        }

        /// <summary>
        /// <inheritdoc cref="Get{Tvalue}(XmlElement, string)"/>
        /// </summary>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <returns></returns>
        public static double GetDouble(XmlElement xmlElement,string attributeName)
        {
            return Get<double>(xmlElement, attributeName);
        }

        /// <summary>
        /// <inheritdoc cref="Get{Tvalue}(XmlElement, string)"/>
        /// </summary>
        /// <param name="xmlElement">Xml元素</param>
        /// <param name="attributeName">特性的名称</param>
        /// <returns></returns>
        public static float GetFloat(XmlElement xmlElement,string attributeName)
        {
            return Get<float>(xmlElement, attributeName);
        }
    }
}
