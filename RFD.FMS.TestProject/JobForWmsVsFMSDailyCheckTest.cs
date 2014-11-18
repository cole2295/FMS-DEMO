using ServiceForWmsVsFMSDailyCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RFD.FMS.MODEL.QueryStatistics;
using System.Collections.Generic;

namespace RFD.FMS.TestProject
{
    
    
    /// <summary>
    ///这是 JobForWmsVsFMSDailyCheckTest 的测试类，旨在
    ///包含所有 JobForWmsVsFMSDailyCheckTest 单元测试
    ///</summary>
    [TestClass()]
    public class JobForWmsVsFMSDailyCheckTest
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
        ///CheckCODVisitDaily 的测试
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("ServiceForWmsVsFMSDailyCheck.exe")]
        //public void CheckCODVisitDailyTest()
        //{
        //    JobForWmsVsFMSDailyCheck_Accessor target = new JobForWmsVsFMSDailyCheck_Accessor(); // TODO: 初始化为适当的值
        //    DateTime sTime = DateTime.Parse("2012-10-18 00:00:00"); // TODO: 初始化为适当的值
        //    DateTime eTime = DateTime.Parse("2012-10-18 23:59:59"); // TODO: 初始化为适当的值
        //    IList<CodDailyModel> VisitDetailsByCompany = new List<CodDailyModel>(); // TODO: 初始化为适当的值
        //    IList<CodDailyModel> VisitDetailsByCompanyExpected = null; // TODO: 初始化为适当的值
        //    IList<CodDailyModel> expected = null; // TODO: 初始化为适当的值
        //    IList<CodDailyModel> actual;
        //    actual = target.CheckCODVisitDaily(sTime, eTime, ref VisitDetailsByCompany);
        //    //Assert.AreEqual(VisitDetailsByCompanyExpected, VisitDetailsByCompany);
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("验证此测试方法的正确性。");
        //}
    }
}
