using System;
using System.Collections;
using Common.Logging;
using Spring.Context;
using Spring.Context.Support;

namespace RFD.FMS.Util
{
    /// <summary>
    /// 没有全面采用DI（Dependency Injection）之前，需要使用该Locator，
    /// 隔离Spring和应用。
    /// </summary>
    public class ServiceLocator
    {
        ///<summary>
        /// Spring's Application Context.
        ///</summary>
        public static readonly IApplicationContext Context;

        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceLocator));

        private static readonly Hashtable Table = new Hashtable();

        static ServiceLocator()
        {
            try
            {
                Context = ContextRegistry.GetContext();
            }
            catch (Exception e)
            {
                Logger.Fatal(m => m("Init spring context failed!!", e));
                throw;
            }
        }

        /// <summary>
        /// 获取一个Service的实例。如果获取未在<see cref="ServiceLocator"/>
        /// 注册的Service，则<see cref="ServiceLocator"/>会返回Spring中配置的实例。
        /// Spring中配置实例的名称应该遵循lowerCamalCase原则。比如，员工
        /// 请求“INeedService”接口的实例，Spring的配置文件中应该有一个对应的id或
        /// 者name为“needService”的定义。
        /// </summary>
        public static T GetService<T>() where T : class
        {
            try
            {
                var service = Table[typeof(T)] as T;
                if (service == null)
                {
                    string serviceName = typeof(T).Name;
                    if (serviceName.StartsWith("I"))
                    {
                        serviceName = serviceName[1].ToString().ToLower() + serviceName.Substring(2, serviceName.Length - 2);
                    }

                    else
                    {
                        throw new InvalidOperationException(
                            "A service must have a interface, and naming convension must follow IUpperCamelCase.");
                    }
                    service = Context.GetObject(serviceName) as T;
                }
                return service;
            }
            catch (Exception e)
            {
                Logger.Fatal(m => m("Init spring context failed!!", e));
                throw;
            }
        }

        /// <summary>
        /// 获取指定类型的一个类的实例，该类会返回Spring中配置的实例。Spring中
        /// 配置实例的名称应该遵循lowerCamalCase原则。
        /// </summary>
        public static T GetObject<T>() where T : class
        {
            string typeName = typeof(T).Name;
            typeName = typeName[0].ToString().ToLower() + typeName.Substring(1, typeName.Length - 1);
            return Context.GetObject(typeName) as T;
        }

        /// <summary>
        /// 获取指定类型的一个类的实例，该类会返回Spring中配置id或name为
        /// <param name="objectName"></param>的实例。
        /// </summary>
        public static T GetObject<T>(string objectName) where T : class
        {
            return Context.GetObject(objectName) as T;
        }

        /// <summary>
        /// 获取指定类型的一个类的实例，该类会返回Spring中配置id或name为
        /// <param name="objectName"></param>的实例。
        /// </summary>
        public static object GetObject(string objectName)
        {
            return Context.GetObject(objectName);
        }

        /// <summary>
        /// 初始化，引发静态成员自动初始化
        /// </summary>
        public static void Init()
        {
        }
    }
}