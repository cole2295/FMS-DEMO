using RFD.FMS.ServiceImpl.FinancialManage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RFD.FMS.MODEL.COD;
using System.Collections.Generic;
using RFD.FMS.Model.FinancialManage;

namespace RFD.FMS.TestProject.Oracle
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
        ///AddCODBaseInfo 的测试
        ///</summary>
        [TestMethod()]
        public void AddCODBaseInfoTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            FMS_CODBaseInfo model = new FMS_CODBaseInfo()
            {
                MediumID = 43385926,
                WaybillNO = 11111,
                MerchantID = 2,
                WaybillType = "1",
                Flag = 1,
                DeliverStationID = 6,
                TopCODCompanyID = 6,
                WarehouseId = "0201",
                ExpressCompanyID = 9,
                RfdAcceptTime = DateTime.Now,
                RfdAcceptDate = DateTime.Now,
                FinalExpressCompanyID = 9,
                DeliverTime = DateTime.Now,
                DeliverDate = DateTime.Now,
                ReturnWareHouseID = "",
                ReturnExpressCompanyID = 0,
                TotalAmount = 20,
                PaidAmount = 20,
                NeedPayAmount = 0,
                BackAmount = 0,
                NeedBackAmount = 0,
                AccountWeight = 2.0M,
                AreaID = "460108",
                AreaType = 1,
                BoxsNo = "",
                Address = "北京市",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsDeleted = false,
                ReturnTime = DateTime.Now,
                ReturnDate = DateTime.Now.ToString("yyyy-MM-dd"),
                IsFare = 1,
                Fare = 0,
                FareFormula = "",
                OperateType = 1,
                ProtectedPrice = 0,
                DistributionCode = "rfd",
                CurrentDistributionCode="rfd"
            }; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = target.AddCODBaseInfo(model);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetModel 的测试
        ///</summary>
        [TestMethod()]
        public void GetModelTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            Int64 id = 240041808;
            int expected = 0; // TODO: 初始化为适当的值
            FMS_CODBaseInfo actual;
            actual = target.GetModel(id);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetCODBaseInfoModelByWaybillNO 的测试
        ///</summary>
        [TestMethod()]
        public void GetCODBaseInfoModelByWaybillNOTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            Int64 waybillNo = 11111;
            int expected = 0; // TODO: 初始化为适当的值
            FMS_CODBaseInfo actual;
            actual = target.GetCODBaseInfoModelByWaybillNO(waybillNo);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///GetCODBaseInfoModelList 的测试
        ///</summary>
        [TestMethod()]
        public void GetCODBaseInfoModelListTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            Dictionary<string, object> codSearchModel = new Dictionary<string, object>();
            codSearchModel.Add("WaybillNO", 11111);
            int expected = 0; // TODO: 初始化为适当的值
            List<FMS_CODBaseInfo> actual;
            actual = target.GetCODBaseInfoModelList(codSearchModel);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///UpdateAmountByID 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateAmountByIDTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            bool actual;
            FMS_CODBaseInfo info = new FMS_CODBaseInfo()
            {
                ID = 240041808,
                NeedPayAmount=0,
                NeedBackAmount=10
            };
            actual = target.UpdateAmountByID(info);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///UpdateIsDeletedByID 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateIsDeletedByIDTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            bool actual;
            FMS_CODBaseInfo info = new FMS_CODBaseInfo()
            {
                ID = 240041808,
            };
            actual = target.UpdateIsDeletedByID(info);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///ExistsIncomeBaseInfo 的测试
        ///</summary>
        [TestMethod()]
        public void ExistsIncomeBaseInfoTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            Int64 incomeid = 33292793711; // TODO: 初始化为适当的值
            bool actual;
            actual = target.ExistsIncomeBaseInfo(incomeid);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///AddIncomeBaseInfo 的测试
        ///</summary>
        [TestMethod()]
        public void AddIncomeBaseInfoTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            FMS_IncomeBaseInfo model = new FMS_IncomeBaseInfo()
            {
                WaybillNo = 111111,
                WaybillType = "1",
                MerchantID = 2,
                ExpressCompanyID = 20,
                FinalExpressCompanyID = 20,
                DeliverStationID = 6,
                TopCODCompanyID = 6,
                RfdAcceptTime = DateTime.Now,
                DeliverTime = DateTime.Now,
                ReturnTime = DateTime.Now,
                ReturnExpressCompanyID = 0,
                BackStationTime = DateTime.Now,
                BackStationStatus = "5",
                ProtectedAmount = 20,
                TotalAmount = 20,
                PaidAmount = 20,
                NeedPayAmount = 0,
                BackAmount = 0,
                NeedBackAmount = 0,
                AccountWeight = 2.3M,
                AreaID = "370112",
                ReceiveAddress = "北京市",
                SignType = 3,
                InefficacyStatus = 0,
                ReceiveStationID = 0,
                ReceiveDeliverManID = 0,
                DistributionCode = "rfd",
                CurrentDistributionCode = "rfd",
                WayBillInfoWeight = 2.3M,
                SubStatus = 7,
                AcceptType = "现金",
                CustomerOrder = "111111",
                OriginDepotNo = "",
                PeriodAccountCode = "",
                WaybillCategory = "1009",
                createtime = DateTime.Now,
                updatetime = DateTime.Now,
                DeliverCode = "111111"
            };
            int actual;
            actual = target.AddIncomeBaseInfo(model);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///UpdateIncomeBaseInfo 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateIncomeBaseInfoTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            FMS_IncomeBaseInfo model = new FMS_IncomeBaseInfo()
            {
                WaybillNo = 111111,
                WaybillType = "1",
                MerchantID = 2,
                ExpressCompanyID = 9,
                FinalExpressCompanyID = 9,
                DeliverStationID = 6,
                TopCODCompanyID = 6,
                RfdAcceptTime = DateTime.Now,
                DeliverTime = DateTime.Now,
                ReturnTime = DateTime.Now,
                ReturnExpressCompanyID = 0,
                BackStationTime = DateTime.Now,
                BackStationStatus = "5",
                ProtectedAmount = 20,
                TotalAmount = 20,
                PaidAmount = 20,
                NeedPayAmount = 0,
                BackAmount = 0,
                NeedBackAmount = 0,
                AccountWeight = 2.3M,
                AreaID = "370112",
                ReceiveAddress = "北京市",
                SignType = 3,
                InefficacyStatus = 0,
                ReceiveStationID = 0,
                ReceiveDeliverManID = 0,
                DistributionCode = "rfd",
                CurrentDistributionCode = "rfd",
                WayBillInfoWeight = 2.3M,
                SubStatus = 7,
                AcceptType = "现金",
                CustomerOrder = "111111",
                OriginDepotNo = "",
                PeriodAccountCode = "",
                WaybillCategory = "1009",
                createtime = DateTime.Now,
                updatetime = DateTime.Now,
                DeliverCode = "111111"
            };
            bool actual;
            actual = target.UpdateIncomeBaseInfo(model);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///UpdateIncomeBaseInfoStatus 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateIncomeBaseInfoStatusTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            FMS_IncomeBaseInfo model = new FMS_IncomeBaseInfo()
            {
                FinalExpressCompanyID = 9,
                DeliverStationID = 7,
                TopCODCompanyID = 7,
                DeliverTime = DateTime.Now,
                BackStationTime = DateTime.Now,
                BackStationStatus="5",
                AccountWeight=3.0M,
                SignType=2,
                AcceptType="POS机",
                WaybillNo=111111
            };
            bool actual;
            actual = target.UpdateIncomeBaseInfoStatus(model);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///UpdateBackStatus 的测试
        ///</summary>
        [TestMethod()]
        public void UpdateBackStatusTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            FMS_IncomeBaseInfo model = new FMS_IncomeBaseInfo()
            {
                ReturnTime = DateTime.Now,
                ReturnExpressCompanyID = 20,
                BackStationStatus = "5",
                AccountWeight = 3.1M,
                SubStatus = 6,
                AcceptType = "现金",
                WaybillNo = 111111
            };
            bool actual;
            actual = target.UpdateBackStatus(model);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///AddIncomeFeeInfo 的测试
        ///</summary>
        [TestMethod()]
        public void AddIncomeFeeInfoTest()
        {
            FMSInterfaceService target = new FMSInterfaceService(); // TODO: 初始化为适当的值
            FMS_IncomeFeeInfo model = new FMS_IncomeFeeInfo()
            {
                WaybillNO = 111111,
                IsAccount = 1,
                AccountStandard = "0",
                AccountFare = 0,
                IsProtected = 1,
                ProtectedStandard = 0,
                ProtectedFee = 0,
                IsReceive = 1,
                ReceiveStandard = 0,
                ReceiveFee = 0,
                CreateBy = 0,
                CreateTime = DateTime.Now,
                UpdateBy = 0,
                UpdateTime = DateTime.Now,
                IsDeleted = 0,
                TransferPayType = 0,
                DeputizeAmount = 0,
                POSReceiveStandard = 0,
                POSReceiveFee = 0,
                CashReceiveServiceStandard =0,
                CashReceiveServiceFee = 0,
                POSReceiveServiceStandard = 0,
                POSReceiveServiceFee = 0,
                ExpressReceiveBasicDeduct = 0,
                ExpressSendBasicDeduct = 0,
                ExpressAreaDeduct = 0,
                ExpressWeightDeduct = 0,
                ProgramReceiveBasicDeduct = 0,
                ProgramSendBasicDeduct = 0,
                ProgramAreaDeduct = 0,
                ProgramWeightDeduct = 0,
                IsDeductAcount = 1,
                AreaType = 1,
                IsCod = 1,
            };
            int actual;
            actual = target.AddIncomeFeeInfo(model);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }


    }
}
