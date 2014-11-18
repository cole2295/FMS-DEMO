using ServiceForCodAccount.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Util;

namespace RFD.FMS.TestProject.ForCodAccount
{
    
    
    /// <summary>
    ///这是 CommonTest 的测试类，旨在
    ///包含所有 CommonTest 单元测试
    ///</summary>
    [TestClass()]
    public class CommonTest
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
        ///GetAccountWhere 的测试
        ///</summary>
        [TestMethod()]
        public void GetAccountWhereTest()
        {
            FMS_CODBaseInfo detail = new FMS_CODBaseInfo
            {
                WaybillNO = 9120592813929,
                ID = 34463309,
                DeliverTime=DataConvert.ToDateTime("2012-10-22 15:15:21.377"),
                MerchantID = 5589,
                DeliverStationID = 55,
                WaybillType="0",
                WarehouseId="194",
                WareHouseType=2,
                AccountWeight = 0.860M,
                AreaID = "420116",
                AreaType=1,
                TopCODCompanyID=55,
                DistributionCode="rfd",
                NeedPayAmount=648.00M,
                NeedBackAmount = 0.00M
            }; // TODO: 初始化为适当的值
            FMS_CODBaseInfo detailExpected = null; // TODO: 初始化为适当的值
            ServiceForCodAccount.Common.Common.GetAccountWhere(ref detail);
            //Assert.AreEqual(detailExpected, detail);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }
    }
}
