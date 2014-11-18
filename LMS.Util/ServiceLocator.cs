using System;
using System.Collections;
using Common.Logging;
using Spring.Context;
using Spring.Context.Support;

namespace RFD.FMS.Util
{
    /// <summary>
    /// û��ȫ�����DI��Dependency Injection��֮ǰ����Ҫʹ�ø�Locator��
    /// ����Spring��Ӧ�á�
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
        /// ��ȡһ��Service��ʵ���������ȡδ��<see cref="ServiceLocator"/>
        /// ע���Service����<see cref="ServiceLocator"/>�᷵��Spring�����õ�ʵ����
        /// Spring������ʵ��������Ӧ����ѭlowerCamalCaseԭ�򡣱��磬Ա��
        /// ����INeedService���ӿڵ�ʵ����Spring�������ļ���Ӧ����һ����Ӧ��id��
        /// ��nameΪ��needService���Ķ��塣
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
        /// ��ȡָ�����͵�һ�����ʵ��������᷵��Spring�����õ�ʵ����Spring��
        /// ����ʵ��������Ӧ����ѭlowerCamalCaseԭ��
        /// </summary>
        public static T GetObject<T>() where T : class
        {
            string typeName = typeof(T).Name;
            typeName = typeName[0].ToString().ToLower() + typeName.Substring(1, typeName.Length - 1);
            return Context.GetObject(typeName) as T;
        }

        /// <summary>
        /// ��ȡָ�����͵�һ�����ʵ��������᷵��Spring������id��nameΪ
        /// <param name="objectName"></param>��ʵ����
        /// </summary>
        public static T GetObject<T>(string objectName) where T : class
        {
            return Context.GetObject(objectName) as T;
        }

        /// <summary>
        /// ��ȡָ�����͵�һ�����ʵ��������᷵��Spring������id��nameΪ
        /// <param name="objectName"></param>��ʵ����
        /// </summary>
        public static object GetObject(string objectName)
        {
            return Context.GetObject(objectName);
        }

        /// <summary>
        /// ��ʼ����������̬��Ա�Զ���ʼ��
        /// </summary>
        public static void Init()
        {
        }
    }
}