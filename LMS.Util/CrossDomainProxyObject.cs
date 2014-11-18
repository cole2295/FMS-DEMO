using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RFD.FMS.Util
{

    public class DomainFactory : IDisposable
    {
        private static AppDomain _appDomain;

        private DomainFactory() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName"></param>
        public DomainFactory(string domainName)
        {
            _appDomain = AppDomain.CreateDomain(domainName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public T CreateDomainObject<T>(string assemblyName, string typeName)
        {
            var obj = (T)_appDomain.CreateInstanceFromAndUnwrap(assemblyName, typeName);
            return obj;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_appDomain != null)
                AppDomain.Unload(_appDomain);
        }

        #endregion
    }

    public class CrossDomainProxyObject : MarshalByRefObject
    {
        /// <summary>
        /// 当权程序集
        /// </summary>
        private Assembly CurrentAssembly
        {
            get
            {
                return GetType().Assembly;
            }
        }

        /// <summary>
        /// 加载指定程序集
        /// </summary>
        /// <param name="assemblyFullPath"></param>
        /// <returns></returns>
        private static Assembly LoadAssembly(string assemblyFullPath)
        {
            if (!File.Exists(assemblyFullPath))
                throw new FileNotFoundException(string.Format("程序集文件未找到：{0}", assemblyFullPath));
            return Assembly.LoadFile(assemblyFullPath);
        }

        /// <summary>
        /// 调用当前程序集方法
        /// </summary>
        /// <param name="fullClassName"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Invoke(string fullClassName, string methodName, params Object[] args)
        {
            return Invoke(CurrentAssembly, fullClassName, methodName, args);
        }


        /// <summary>
        /// 调用指定程序集方法
        /// </summary>
        /// <param name="assemblyFullPath"></param>
        /// <param name="fullClassName"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Invoke(string assemblyFullPath, string fullClassName, string methodName, params Object[] args)
        {

            try
            {
                var currentAssembly = LoadAssembly(assemblyFullPath);
                return Invoke(currentAssembly, fullClassName, methodName, args);
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 调用加载程序集方法
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="fullClassName"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool Invoke(Assembly assembly, string fullClassName, string methodName, params Object[] args)
        {
            if (assembly == null)
                return false;
            Type tp = assembly.GetType(fullClassName);
            if (tp == null)
                return false;
            MethodInfo method = tp.GetMethod(methodName);
            if (method == null)
                return false;
            Object obj = Activator.CreateInstance(tp);
            method.Invoke(obj, args);
            return true;
        }

        /// <summary>
        /// 获取字段名称列表
        /// </summary>
        /// <param name="assemblyFullPath"></param>
        /// <param name="fullClassName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllGetFields(string assemblyFullPath, string fullClassName, BindingFlags bindingFlags)
        {
            var currentAssembly = LoadAssembly(assemblyFullPath);
            return GetAllGetFields(currentAssembly, fullClassName, bindingFlags);
        }


        /// <summary>
        /// 获取字段名称列表
        /// </summary>
        /// <param name="fullClassName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllGetFields(string fullClassName, BindingFlags bindingFlags)
        {
            return GetAllGetFields(CurrentAssembly, fullClassName, bindingFlags);
        }

        /// <summary>
        /// 获取字段名称列表
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="fullClassName"></param>
        /// <param name="bindingFlags">BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic;</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetAllGetFields(Assembly assembly, string fullClassName, BindingFlags bindingFlags)
        {
            var rValues = new Dictionary<string, string>();
            try
            {
                var type = assembly.GetType(fullClassName, true, false);
                var fields = type.GetFields(bindingFlags);
                fields.ToList().ForEach(field =>
                {
                    if (!rValues.ContainsKey(field.Name))
                    {
                        try
                        {
                            rValues.Add(field.Name, field.FieldType.FullName);
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch { }
                        // ReSharper restore EmptyGeneralCatchClause
                    }
                });
                return rValues;
            }
            catch { return rValues; }
        }



        /// <summary>
        /// 获取属性名称列表
        /// </summary>
        /// <param name="assemblyFullPath"></param>
        /// <param name="fullClassName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllGetProperties(string assemblyFullPath, string fullClassName, BindingFlags bindingFlags)
        {
            var currentAssembly = LoadAssembly(assemblyFullPath);
            return GetAllGetProperties(currentAssembly, fullClassName, bindingFlags);
        }


        /// <summary>
        /// 获取属性名称列表
        /// </summary>
        /// <param name="fullClassName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllGetProperties(string fullClassName, BindingFlags bindingFlags)
        {
            return GetAllGetProperties(CurrentAssembly, fullClassName, bindingFlags);
        }


        /// <summary>
        /// 获取属性名称列表
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="fullClassName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetAllGetProperties(Assembly assembly, string fullClassName, BindingFlags bindingFlags)
        {
            var rValues = new Dictionary<string, string>();
            try
            {
                var type = assembly.GetType(fullClassName, true, false);
                var properties = type.GetProperties(bindingFlags);
                properties.ToList().ForEach(field =>
                {
                    if (!rValues.ContainsKey(field.Name))
                    {
                        try
                        {
                            rValues.Add(field.Name, field.PropertyType.FullName);
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch { }
                        // ReSharper restore EmptyGeneralCatchClause
                    }
                });
                return rValues;
            }
            catch { return rValues; }
        }

    }
}
