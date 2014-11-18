using RFD.FMS.ServiceImpl.COD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.Service.COD;
using System.Collections.Generic;

namespace RFD.FMS.TestProject
{
    
    
    /// <summary>
    ///这是 CODBaseInfoServiceTest 的测试类，旨在
    ///包含所有 CODBaseInfoServiceTest 单元测试
    ///</summary>
    [TestClass()]
    public class CODBaseInfoServiceTest
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

        ICODBaseInfoService target = ServiceLocator.GetService<ICODBaseInfoService>();
        /// <summary>
        ///GetDeliver 的测试
        ///</summary>
        [TestMethod()]
        public void GetDeliverTest()
        {
             // TODO: 初始化为适当的值
            int accountDays = 46; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetDeliver(accountDays);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///ChangeDeliverBack 的测试
        ///</summary>
        [TestMethod()]
        public void ChangeDeliverBackTest()
        {
            List<string> noList = new List<string>() { "21166917", "21166918", "21166919", "21166920", "21166921", "21166922", "21166923", "21166924", "21166925" }; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = target.ChangeDeliverBack(noList);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetError8 的测试
        ///</summary>
        [TestMethod()]
        public void GetError8Test()
        {
            int accountDays = 46; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetError8(accountDays);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetError9 的测试
        ///</summary>
        [TestMethod()]
        public void GetError9Test()
        {
            int accountDays = 46; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetError9(accountDays);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetError7 的测试
        ///</summary>
        [TestMethod()]
        public void GetError7Test()
        {
            int accountDays = 46; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetError7(accountDays);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetError6 的测试
        ///</summary>
        [TestMethod()]
        public void GetError6Test()
        {
            int accountDays = 46; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetError6(accountDays);
        }

        /// <summary>
        ///GetError34 的测试
        ///</summary>
        [TestMethod()]
        public void GetError34Test()
        {
            int errorType = 3; // TODO: 初始化为适当的值
            int accountDays = 46; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetError34(errorType, accountDays);
        }

        /// <summary>
        ///GetError5 的测试
        ///</summary>
        [TestMethod()]
        public void GetError5Test()
        {
            int accountDays = 46; // TODO: 初始化为适当的值
            DataTable expected = null; // TODO: 初始化为适当的值
            DataTable actual;
            actual = target.GetError5(accountDays);
        }
    }
}
