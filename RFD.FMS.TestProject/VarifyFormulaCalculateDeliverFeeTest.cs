using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFD.FMS.DAL.Oracle.BasicSetting;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.TestProject
{
    /// <summary>
    /// VarifyFormulaCalculateDeliverFeeTest 的摘要说明
    /// </summary>
    [TestClass]
    public class VarifyFormulaCalculateDeliverFeeTest
    {
        public VarifyFormulaCalculateDeliverFeeTest()
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
        public void FormulaTest()
        {
            //
            // TODO: 在此处添加测试逻辑
            //
            long orderno = 9130111069556;
            IncomeFeeInfoDao FeeDao = new IncomeFeeInfoDao();
            DataTable dt = FeeDao.GetInComeFeeInfoByOrderNo(orderno);
            if (dt!=null&&dt.Rows.Count>0)
            {
                FMS_IncomeFeeInfo model=new FMS_IncomeFeeInfo();
                DataRow row = dt.Rows[0];
                int isCategory = DataConvert.ToInt(row["IsCategory"].ToString(), 0);
            var sbErrorMsg = new StringBuilder();
               
            //if (model.AreaType == null || model.AreaType <= 0)
            {
                //重新找区域类型
                model.AreaType = GetAreaType(DataConvert.ToInt(row["MerchantID"]), row["ExpressCompanyID"].ToString(), row["DistributionCode"].ToString(), row["AreaID"].ToString(), isCategory, row["WaybillCategory"].ToString(), int.Parse(row["DeliverStationID"].ToString()));
            }

           
                model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T0;
            
            //找计算公式
            decimal needPayAmount = 0M;
            decimal.TryParse(row["NeedPayAmount"].ToString(), out needPayAmount);//应收金额
            //**********************增加是否走配送商逻辑 zhangrongrong *********************************
            bool flag;
            string weightormula;
            string weightdeliverfee;
            int isCod;
            string s;
            //增加是否走配送商逻辑
            if (GetIncomeFeeMsg(row, model, sbErrorMsg, isCategory, out flag, out s, out weightormula, out weightdeliverfee, out isCod))
                {
                     string weightormulatem = weightormula;
                    
                }
               
            } 
            }
        /// <summary>
        /// 查询AreaType
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="distributionCode"></param>
        /// <param name="areaId"></param>
        /// <param name="isCategory"></param>
        /// <param name="waybillCategory"></param>
        /// <returns></returns>
        private int GetAreaType(int merchantId, string wareHouseId, string distributionCode, string areaId, int isCategory, string waybillCategory)
        {
            AreaLevelIncomeSearchModel incomeSearchModel = new AreaLevelIncomeSearchModel();
            incomeSearchModel.MerchantID = merchantId;
            incomeSearchModel.WareHouse = wareHouseId;
            incomeSearchModel.DistributionCode = distributionCode;
            incomeSearchModel.AreaID = areaId;
            if (isCategory == 1)
            {
                incomeSearchModel.GoodsCategoryCode = waybillCategory;
            }
            var areaExpressLevelIncomeDao = new AreaExpressLevelIncomeDao();
            return areaExpressLevelIncomeDao.GetAreaTypeByCondition(incomeSearchModel);
        }
        /// <summary>
        /// 查询AreaType
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="distributionCode"></param>
        /// <param name="areaId"></param>
        /// <param name="isCategory"></param>
        /// <param name="waybillCategory"></param>
        /// <param name="expressId"></param>
        /// <returns></returns>
        private int GetAreaType(int merchantId, string wareHouseId, string distributionCode, string areaId, int isCategory, string waybillCategory, int? expressId)
        {
            AreaLevelIncomeSearchModel incomeSearchModel = new AreaLevelIncomeSearchModel();
            incomeSearchModel.MerchantID = merchantId;
            incomeSearchModel.WareHouse = wareHouseId;
            incomeSearchModel.DistributionCode = distributionCode;
            incomeSearchModel.AreaID = areaId;
            if (isCategory == 1)
            {
                incomeSearchModel.GoodsCategoryCode = waybillCategory;
            }
            incomeSearchModel.AuditStatus = 2;
            var areaExpressLevelIncomeDao = new AreaExpressLevelIncomeDao();
            DataTable dt = areaExpressLevelIncomeDao.GetAreaLevelIncomeList(incomeSearchModel);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (expressId != null && expressId > 0)
                {
                    DataRow[] dataRows = dt.Select("EXPRESSCOMPANYID='" + expressId + "'");
                    if (dataRows.Length > 0)
                    {
                        return int.Parse(dataRows[0]["AREATYPE"].ToString());
                    }
                    else
                    {
                         dataRows = dt.Select("EXPRESSCOMPANYID='" + 11 + "'");
                        if (dataRows.Length > 0)
                        {
                            return int.Parse(dataRows[0]["AREATYPE"].ToString());
                        }  
                    }
                }
                else
                {
                    DataRow[] dataRows = dt.Select("EXPRESSCOMPANYID='" + 11 + "'");
                    if (dataRows.Length > 0)
                    {
                        return int.Parse(dataRows[0]["AREATYPE"].ToString());
                    }
                }
                return int.Parse(dt.Rows[0]["AREATYPE"].ToString());
            }
            //incomeSearchModel.ExpressCompanyID = expressId;

            return 0;
        }

        private static bool GetIncomeFeeMsg(DataRow row, FMS_IncomeFeeInfo model, StringBuilder sbErrorMsg, int isCategory,
                                        out bool flag, out string s, out string weightormula, out string weightdeliverfee,
                                        out int isCod)
        {
            var merchantDeliverFeeDao = new MerchantDeliverFeeDao();
            //先判断是否走配送商逻辑
            DataTable dtFee = merchantDeliverFeeDao.GetBasicDeliverFeeByCondition(DataConvert.ToInt(row["MerchantID"]),
                                                                                  row["ExpressCompanyID"].ToString(),
                                                                                  DataConvert.ToInt(model.AreaType), isCategory,
                                                                                  row["WaybillCategory"].ToString(),
                                                                                  row["DistributionCode"].ToString());

            if (dtFee == null || dtFee.Rows.Count == 0)
            {
                //没有配送商收入配送费设置信息
                sbErrorMsg.Append(EnumHelper.GetDescription(BizEnums.IncomeFeeAccountType.T4) + "、");
                model.IsAccount = (int)BizEnums.IncomeFeeAccountType.T4;
                {
                    {
                        s = sbErrorMsg.ToString();
                        flag = false;
                        weightormula = null;
                        weightdeliverfee = null;
                        isCod = 0;
                        return true;
                    }
                }
            }
            else
            {
                var datarows =
                    dtFee.Select(string.Format("StationID='{0}'and ISEXPRESS='1'AND ExpressCompanyID='{1}'", row["DeliverStationID"].ToString(), row["ExpressCompanyID"].ToString()));
                if (datarows.Length > 0)
                {
                    weightormula = datarows[0]["BasicDeliverFee"].ToString();
                    isCod = int.Parse(datarows[0]["IsCod"].ToString());
                    weightdeliverfee = datarows[0]["DeliverFee"].ToString();
                    flag = true;
                }
                else
                {
                    var datarowsV =
                        dtFee.Select(string.Format("StationID='{0}'AND ExpressCompanyID='{1}'", "11", row["ExpressCompanyID"].ToString()));
                    if (datarowsV.Length>0)
                    {
                        weightormula = datarowsV[0]["BasicDeliverFee"].ToString();
                        isCod = int.Parse(datarowsV[0]["IsCod"].ToString());
                        weightdeliverfee = datarowsV[0]["DeliverFee"].ToString();
                    }
                    else
                    {
                        flag = false;
                        s = null;
                        weightormula = null;
                        weightdeliverfee = null;
                        isCod = 0;
                        return false;
                    }
                }
            }
            flag = false;
            s = null;
            return false;
        }    //return sbErrorMsg.ToString();
        
        
    }
}
