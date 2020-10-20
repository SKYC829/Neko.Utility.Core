using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neko.Utility.Core.Common
{
    /// <summary>
    /// <see cref="System.Reflection"/>帮助类
    /// <para>可以快速获取程序集,类型和实例化类型</para>
    /// </summary>
    public sealed class ReferenceUtil
    {
        /// <summary>
        /// 获取当前程序的程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly GetDefaultAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// 根据名称获取程序集
        /// <para>如果<paramref name="assemblyName"/>为空则获取当前程序集</para>
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByName(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                return GetDefaultAssembly();
            }
            assemblyName = assemblyName.TrimStart().TrimEnd();
            Assembly result = Assembly.Load(assemblyName);
            if (result == null)
            {
                throw new FileNotFoundException(string.Format("命名空间{0}不存在!", assemblyName));
            }
            return result;
        }

        /// <summary>
        /// 根据文件路径加载文件下的程序集
        /// <para>如果<paramref name="dllName"/>为空则获取当前程序集</para>
        /// </summary>
        /// <param name="dllName">文件路径</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByDll(string dllName)
        {
            if (string.IsNullOrEmpty(dllName))
            {
                return GetDefaultAssembly();
            }
            dllName = dllName.TrimStart().TrimEnd();
            Assembly result = null;
            try
            {
                result = Assembly.LoadFrom(dllName);
                if (result == null)
                {
                    throw new FileNotFoundException(string.Format("无法加载程序集{0}", dllName));
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(string.Format("文件{0}不存在!", dllName));
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 根据传入的参数自动判断是根据名称获取程序集还是根据文件获取程序集<br/>
        /// 如果<paramref name="assemblyName"/>为空则获取当前程序集
        /// </summary>
        /// <param name="assemblyName">程序集名称或文件路径</param>
        /// <returns></returns>
        public static Assembly GetAssembly(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                return GetDefaultAssembly();
            }
            else if (assemblyName.TrimStart().TrimEnd().ToLower().EndsWith(".dll"))
            {
                return GetAssemblyByDll(assemblyName);
            }
            else
            {
                return GetAssemblyByName(assemblyName);
            }
        }

        /// <summary>
        /// 获取当前程序集下特定的类型
        /// <para>当<paramref name="typeName"/>为类型的简称时(如Program),<br/>获取的是当前程序集下的Program</para>
        /// </summary>
        /// <param name="typeName">类型的名称</param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            Type result = null;
            if (string.IsNullOrEmpty(typeName))
            {
                return result;
            }
            string assemblyName = string.Empty;
            if (typeName.IndexOf(',') > -1)
            {
                string[] typeNameCollection = typeName.Split(',');
                List<string> assemblyNameList = new List<string>();
                assemblyNameList.AddRange(typeNameCollection);
                typeName = assemblyNameList.FirstOrDefault();
                assemblyNameList.RemoveAt(0);
                assemblyName = string.Join(',', assemblyNameList.ToArray()).Trim();
            }
            typeName = typeName.Trim();
            try
            {
                Assembly assembly = GetAssembly(assemblyName);
                result = GetType(assembly, typeName);
                if (result == null)
                {
                    throw new EntryPointNotFoundException(string.Format("程序集{0}中不存在类型{1}!", assembly.FullName, typeName));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取程序集下特定的类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="typeName">类型的名称</param>
        /// <returns></returns>
        public static Type GetType(Assembly assembly, string typeName)
        {
            Type result = null;
            if (string.IsNullOrEmpty(typeName) || assembly == null)
            {
                return result;
            }
            typeName = typeName.Trim();
            result = assembly.GetTypes().FirstOrDefault(p => p.FullName.Equals(typeName) || p.Name.Equals(typeName));
            if (result == null)
            {
                throw new EntryPointNotFoundException(string.Format("程序集{0}中不存在类型{1}!", assembly.FullName, typeName));
            }
            return result;
        }

        /// <summary>
        /// 实例化一个类型
        /// <para>类似依赖注入</para>
        /// </summary>
        /// <typeparam name="Ttype">实力类型</typeparam>
        /// <param name="typeName">类型名称</param>
        /// <param name="constructParams">类型构造函数需要的参数</param>
        /// <returns></returns>
        public static Ttype Instance<Ttype>(string typeName, params object[] constructParams) where Ttype:class,new()
        {
            object result = Instance(typeName, constructParams);
            if(result == null)
            {
                return default(Ttype);
            }
            return result as Ttype;
        }

        /// <summary>
        /// 实例化一个类型
        /// <para>类似依赖注入</para>
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="constructParams">类型构造函数需要的参数</param>
        /// <returns></returns>
        public static object Instance(string typeName,params object[] constructParams)
        {
            object result = null;
            Type targetType = GetType(typeName);
            if(targetType == null)
            {
                return result;
            }
            List<Type> paraTypes = new List<Type>();
            for (int i = 0; i < constructParams.Length; i++)
            {
                object param = constructParams.ElementAt(i);
                if(param == null)
                {
                    throw new ArgumentNullException(nameof(param), "生成构造函数时,构造参数不允许为空!");
                }
                Type paraType = param.GetType();
                paraTypes.Add(paraType);
            }
            ConstructorInfo constructor = targetType.GetConstructor(paraTypes.ToArray());
            result = constructor.Invoke(constructParams);
            return result;
        }
    }
}
