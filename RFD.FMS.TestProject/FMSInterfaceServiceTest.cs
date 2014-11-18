using RFD.FMS.ServiceImpl.FinancialManage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RFD.FMS.MODEL.Enumeration;
using System.Collections.Generic;

namespace RFD.FMS.TestProject
{
    
    
    /// <summary>
    ///这是 FMSInterfaceServiceTest 的测试类，旨在
    ///包含所有 FMSInterfaceServiceTest 单元测试
    ///</summary>
    [TestClass()]
    public class FMSInterfaceServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///UpdateExpressParameter 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateExpressParameterTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            long waybillNo = 9120924598765; // TODO: 初始化为适当的值
            Dictionary<EnumExpressParameter, string> parameters = new Dictionary<EnumExpressParameter, string>();
            parameters.Add(EnumExpressParameter.SortingCenterId, "2");
            //parameters.Add(EnumExpressParameter.MerchantId, "2");
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.UpdateExpressParameter(waybillNo, parameters);
            
        }

        /// <summary>
        ///UpdateExpressParameter 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateExpressParameterTest1()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            long waybillNo = 11303120000002; // TODO: 初始化为适当的值
            Dictionary<EnumExpressParameter, string> parameters = new Dictionary<EnumExpressParameter, string>();
            parameters.Add(EnumExpressParameter.Goodspayment, "30"); 
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.UpdateExpressParameter(waybillNo, parameters);

        }

        /// <summary>
        ///UpdateExpressParameter 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateExpressParameterTest2()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            long waybillNo = 0; // TODO: 初始化为适当的值
            Dictionary<EnumExpressParameter, string> parameters = null; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.UpdateExpressParameter(waybillNo, parameters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
