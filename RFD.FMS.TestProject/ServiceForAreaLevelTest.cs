using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.COD;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.ServiceImpl.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.TestProject
{
    /// <summary>
    /// ServiceForAreaLevelTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ServiceForAreaLevelTest
    {
        public ServiceForAreaLevelTest()
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

            IFMSInterfaceService service = new FMSInterfaceService();
     //       var ret = service.DeleteExpressFeeInfo(9120250306695);

            FMS_IncomeReceiveFeeInfo receiveModel = new FMS_IncomeReceiveFeeInfo()
                                                        {
                                                            AcceptType = "现金s",
                                                            BackStationCreateTime = DateTime.Now,
                                                            ComeFrom = 2,
                                                            CreateTime = DateTime.Now,
                                                            CustomerOrder = "10101010",
                                                            DeliverManID = 101,
                                                            DiscributionCode = "rfdss",
                                                            FactAmount = 2,
                                                            DeliverStationID = 347
                                                            ,
                                                            FactBackAmount = 3,
                                                            FeeID = 1000000,
                                                            FinancialStatus = 1,
                                                            IsDeleted = false,
                                                            MerchantID = 8,
                                                            NeedAmount = 40,
                                                            NeedBackAmount = 0,
                                                            POSCode = "1234"
                                                            ,
                                                            SignInfoCreateTime = DateTime.Now,
                                                            SignStatus = "已签收",
                                                            Sources = 0,
                                                            UpdateTime = DateTime.Now,
                                                            WaybillCreateTime = DateTime.Now,
                                                            WaybillNO = 10101010,
                                                            WaybillType = "0"
                                                        };
            FMS_IncomeExpressReceiveFeeInfo model = new FMS_IncomeExpressReceiveFeeInfo()
                                                        {
                                                            AcceptType = "现金s",
                                                            BackStationCreateTime = DateTime.Now,
                                                          
                                                            CreateTime = DateTime.Now,
                                                            CustomerOrder = "10101010",
                                                            DeliverManID = 101,
                                                          
                                                            DeliverStationID = 347
                                                            ,
                                                          
                                                            FeeID = 1000000,
                                                           
                                                            IsDeleted = false,
                                                            MerchantID = 8,
                                                
                                                            POSCode = "123456"
                                                            ,
                                                         
                                                  
                                                            Sources = 0,
                                                            UpdateTime = DateTime.Now,
                                                     
                                                            WaybillNO = 10101010,
                                                            DinsureFee = 10,
                                                            DistributionCode = "rfd",
                                                            ReceiveStationID = 343,
                                                            Status = "3",
                                                            TransferFee = 20,
                                                            TransferPayType = 1,
                                                            WaybillCreatTime = DateTime.Now


                                                     
                                                        };

           //var ret =service.UpdateExpressReceiveFeeInfo(model);
           // var ret = service.ExistsExpressReceiveFeeInfo(10101010);

           // decimal  ret = service.GetVolumeParmer(54);

            //decimal ret = service.GetMerchantDeliverFee(48);

           // var ret = service.UpdateInefficacyStatus(332927937, 2);

            //var ret = service.GetIncomeBaseInfoModelByWaybillNO(412092921142);

            //ret.DeliverStationID = 1010;
            //ret.NeedPayAmount = 100;
            //ret.NeedBackAmount = 200;
          

            //service.UpdateIncomeBaseInfoAmount(ret);

            service.UpdateInComeFeeInfoByBackStation(new FMS_IncomeFeeInfo()
                                                         {AccountFare = 2,
                                                             AccountStandard = "fdf",
                                                             AccountWeight = 2,
                                                             AreaType = 1,
                                                             CashReceiveServiceFee = 3,
                                                             CashReceiveServiceStandard = 4,
                                                             CreateBy = 1,
                                                             CreateTime = DateTime.Now,
                                                             DeputizeAmount = 22,
                                                             ExpressAreaDeduct = 34,
                                                             ExpressReceiveBasicDeduct = 2,
                                                             ExpressSendBasicDeduct = 3,
                                                             ExpressWeightDeduct = 4,
                                                             IncomeFeeID = 7946940,
                                                             IsAccount = 0,
                                                             IsCod = 1,
                                                             IsDeductAcount = 1,
                                                             IsDeleted = 0,
                                                             IsProtected = 1,
                                                             IsReceive = 1,
                                                             POSReceiveFee = 3,
                                                             POSReceiveServiceFee = 2,
                                                             POSReceiveStandard =3,
                                                             POSReceiveServiceStandard = 1,
                                                             ProgramSendBasicDeduct = 2,
                                                             ProgramAreaDeduct = 1,
                                                             ProgramReceiveBasicDeduct = 3,
                                                             ProgramWeightDeduct = 1,
                                                             ProtectedFee = 34,
                                                             ProtectedStandard = 2,
                                                             ReceiveFee = 2,
                                                             ReceiveStandard = 3,
                                                             TransferPayType = 1,
                                                             UpdateTime = DateTime.Now,
                                                             UpdateBy = 1,
                                                             WaybillNO = 412092921142

                                                           
                                                         });





        }
    }
}
