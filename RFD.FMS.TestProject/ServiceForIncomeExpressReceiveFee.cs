using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Model.FinancialManage;

namespace RFD.FMS.TestProject
{
    /// <summary>
    /// ServiceForIncomeExpressReceiveFee 的摘要说明
    /// </summary>
    [TestClass]
    public class ServiceForIncomeExpressReceiveFee
    {
        public ServiceForIncomeExpressReceiveFee()
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
           IFMS_ReceiveFeeInfoDao dao = new FMS_ReceiveFeeInfoDao();
            FMS_IncomeExpressReceiveFeeInfo info = new FMS_IncomeExpressReceiveFeeInfo()
                                                       {
                                                           //FeeID = 625,
                                                           WaybillNO = 9120594144139,
                                                           MerchantID = 30,
                                                           DeliverManID = 6770,
                                                           Status ="3" ,
                                                           WaybillCreatTime = DateTime.Now,
                                                           ReceiveStationID = 155,
                                                           //diliverstationid = 703,
                                                           DistributionCode = "rfd",
                                                           TransferFee = 144,
                                                           //dinsureff = 0,
                                                           TransferPayType = 1,
                                                           AcceptType = "现金",
                                                           CreateTime = DateTime.Now,
                                                           UpdateTime = DateTime.Now,
                                                           IsDeleted = true
                                                       };
            var dt = dao.AddExpressReceiveFeeInfo(info);
            if (dt)
            {
                
            }


        }
    }
}
