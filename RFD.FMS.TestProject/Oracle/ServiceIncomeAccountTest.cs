using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;
using ServiceIncomeAccount;

namespace RFD.FMS.TestProject.Oracle
{
    /// <summary>
    /// ServiceIncomeAccountTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ServiceIncomeAccountTest
    {
        public ServiceIncomeAccountTest()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
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
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            DateTime countdate = Convert.ToDateTime("2012-10-4");
            IIncomeAccountService incomeAccountService = ServiceLocator.GetService<IIncomeAccountService>();
          //  incomeAccountService.IsIncomeHistoryCount(accountDate);
          //  DataTable merchantData = incomeAccountService.GetMerchantInfo();
          //  int num = merchantData.Rows.Count;
            int merchantid = 8;
            int warehouseid = 625;
            int deliverstationid = 347;
            //DataTable dt = incomeAccountService.GetIncomeDeliveryAccountInfo(merchantid, countdate);
            //var num = incomeAccountService.CollateIncomeDeliveryAccountInfo(merchantid, countdate, warehouseid, deliverstationid);
            //bool isok = incomeAccountService.AddIncomeDeliveryDetails(merchantid, countdate, deliverstationid, warehouseid);
            IncomeStatLog modellog = new IncomeStatLog();
            //modellog.ExpressCompanyID = warehouseid;
            //modellog.Ip = "10.16.89.8";
            //modellog.MerchantID = merchantid;
            //modellog.StationID = deliverstationid;
            //modellog.StatisticsDate = countdate;
            //modellog.StatisticsType = 1;
            //modellog.UpdateTime = DateTime.Now;
            //modellog.CreateTime = DateTime.Now;
            //modellog.IsSuccess = 1;
            //modellog.Reasons = "成功";
            //incomeAccountService.AddIncomeStatLog(modellog);

            //DataTable dt = incomeAccountService.GetIncomeReturnsCount(merchantid, countdate);
            //var num = incomeAccountService.CollateIncomeReturnsAccountInfo(merchantid, countdate, 103, deliverstationid);

            //bool isOk = incomeAccountService.AddIncomeReturnsAccountDetails(8, countdate, 267, 0);
            //incomeAccountService.GetIncomeOtherReceiveFee(8, countdate);
            var num = incomeAccountService.CollateIncomeOtherReceiveFee(merchantid, countdate, deliverstationid);

        }

        /// <summary>
        ///IncomeAccount 的测试
        ///</summary>
        [TestMethod()]
        public void IncomeAccountTest()
        {
            ServiceIncomeAccount.ServiceIncomeAccount target = new ServiceIncomeAccount.ServiceIncomeAccount(); // TODO: 初始化为适当的值
            DateTime accountDate = new DateTime(); // TODO: 初始化为适当的值
            target.IncomeAccount(accountDate);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }
    }
}
