using RFD.FMS.WEBLOGIC.FinancialManage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.TestProject.Sql
{
    
    
    /// <summary>
    ///这是 WaybillForIncomeBaseInfoServiceTest 的测试类，旨在
    ///包含所有 WaybillForIncomeBaseInfoServiceTest 单元测试
    ///</summary>
    [TestClass()]
    public class WaybillForIncomeBaseInfoServiceTest
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
        ///UpdateIncomeFeeInfoDao 的测试  更新配送费
        ///</summary>
        [TestMethod()]
        public void UpdateIncomeFeeInfoDaoTest1()
        {
            WaybillForIncomeBaseInfoService target = new WaybillForIncomeBaseInfoService(); // TODO: 初始化为适当的值
            List<string> WaybollNOList = new List<string> { "332927937" }; // TODO: 初始化为适当的值
            target.UpdateIncomeFeeInfoDao(WaybollNOList);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///GetIncomeErrorLog 的测试 收入历史错误查询
        ///</summary>
        [TestMethod()]
        public void GetIncomeErrorLogTest()
        {
            WaybillForIncomeBaseInfoService target = new WaybillForIncomeBaseInfoService(); // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetIncomeErrorLog();
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///DisposeEffect 的测试 待生效
        ///</summary>
        [TestMethod()]
        public void DisposeEffectTest()
        {
            WaybillForIncomeBaseInfoService target = new WaybillForIncomeBaseInfoService(); // TODO: 初始化为适当的值
            int rowCount = 1; // TODO: 初始化为适当的值
            target.DisposeEffect(rowCount);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///InsertIntoInComeBaseInfo 的测试
        ///</summary>
        [TestMethod()]
        public void InsertIntoInComeBaseInfoTest()
        {
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            WaybillForIncomeBaseInfoService target = new WaybillForIncomeBaseInfoService(); // TODO: 初始化为适当的值
            string[] waybillnos = { "11303210000039", "11303210000040", "11303210000041", "11303210000042", "11303210000043", "11303210000044", "11303210000045", "11303210000046", "11303210000047", "11303210000048" };
            foreach (string s in waybillnos)
            {
                WaybillStatusChangeLog Logmodel = new WaybillStatusChangeLog
                {
                    WaybillNo = long.Parse(s),
                    Status="3"
                }; // TODO: 初始化为适当的值
                
                actual = target.InsertIntoInComeBaseInfo(Logmodel);
            }
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///AccountIncomeHistory 的测试
        ///</summary>
        [TestMethod()]
        public void AccountIncomeHistoryTest()
        {
            WaybillForIncomeBaseInfoService target = new WaybillForIncomeBaseInfoService(); // TODO: 初始化为适当的值
            int rowCount = 50; // TODO: 初始化为适当的值
            for (int i = 0; i < 27; i++)
            {
                target.AccountIncomeHistory(rowCount);
            }
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }
    }
}
