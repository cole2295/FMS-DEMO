using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.COD;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Domain.COD;
using System.ServiceModel.Activation;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet.UnitOfWork;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class FMSInterfaceService : IFMSInterfaceService
    {
        //sql
        //IIncomeBaseInfoDao incomeDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();
        //IIncomeFeeInfoDao feeDao =ServiceLocator.GetService<IIncomeFeeInfoDao>();

        //oracle
        RFD.FMS.Domain.FinancialManage.IIncomeBaseInfoDao incomeDao = new DAL.Oracle.FinancialManage.IncomeBaseInfoDao();
        RFD.FMS.Domain.FinancialManage.IIncomeFeeInfoDao feeDao = new DAL.Oracle.FinancialManage.IncomeFeeInfoDao();
        RFD.FMS.Domain.FinancialManage.IFMS_MerchantDeliverFeeDao merchantFeeDao = new DAL.Oracle.FinancialManage.FMS_MerchantDeliverFeeDao();

        public bool AdCODBaseInfoList(List<FMS_CODBaseInfo> modelList)
        {
            if (modelList == null || modelList.Count <= 0)
                return true;
            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                foreach (FMS_CODBaseInfo model in modelList)
                {
                    if (AddCODBaseInfo(model) <= 0) return false;
                }
                work.Complete();
                return true;
            }
        }

        public int AddCODBaseInfo(FMS_CODBaseInfo model)
        {
            var codDao = ServiceLocator.GetService <ICODBaseInfoDao>();

            return codDao.Add(model);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_CODBaseInfo GetModel(Int64 id)
        {
            var codDao = ServiceLocator.GetService<ICODBaseInfoDao>();

            return codDao.GetModel(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_CODBaseInfo GetCODBaseInfoModelByWaybillNO(Int64 waybillNo)
        {
            var codDao = ServiceLocator.GetService<ICODBaseInfoDao>();

            return codDao.GetModelByWaybillNO(waybillNo);        
        }

        /// <summary>
        /// 根据条件得到一个对象实体集
        /// </summary>
        public List<FMS_CODBaseInfo> GetCODBaseInfoModelList(Dictionary<string, object> searchParams)
        {
            var codDao = ServiceLocator.GetService<ICODBaseInfoDao>();

            return codDao.GetModelList(searchParams);
        }

        /// <summary>
        /// 更改金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateAmountByID(FMS_CODBaseInfo info)
        {
            var codDao = ServiceLocator.GetService<ICODBaseInfoDao>();

            return codDao.UpdateAmountByID(info);
        }

        /// <summary>
        /// 将制定ID置为停用isdeleted=1
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateIsDeletedByID(FMS_CODBaseInfo info)
        {
            var codDao = ServiceLocator.GetService<ICODBaseInfoDao>();

            return codDao.UpdateIsDeletedByID(info);
        }

        #region 推数
        public bool ExistsIncomeBaseInfo(Int64 incomeid)
        {
            return incomeDao.Exists(incomeid);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeBaseInfo(FMS_IncomeBaseInfo model)
        {
            return incomeDao.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateIncomeBaseInfo(FMS_IncomeBaseInfo model)
        {
            return incomeDao.Update(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateIncomeBaseInfoStatus(FMS_IncomeBaseInfo model)
        {
            return incomeDao.UpdateStatus(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateBackStatus(FMS_IncomeBaseInfo model)
        {
            return incomeDao.UpdateBackStatus(model);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeFeeInfo(FMS_IncomeFeeInfo model)
        {
            return feeDao.Add(model);
        }

        /// <summary>
        /// 归班更新一条数据
        /// </summary>
        public bool UpdateInComeFeeInfoByBackStation(FMS_IncomeFeeInfo model)
        {
            return feeDao.UpdateByBackStation(model);
        }
        #endregion

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// 业务调用 WaybillAdd.aspx 运单修改
        public FMS_IncomeBaseInfo GetIncomeBaseInfoModelByWaybillNO(long waybillNo)
        {
            var incomeDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            return incomeDao.GetModelByWaybillNO(waybillNo);
        }

        /// <summary>
        /// 更新金额
        /// </summary>
        /// 业务调用 WaybillAdd.aspx 运单修复
        public bool UpdateIncomeBaseInfoAmount(FMS_IncomeBaseInfo info)
        {
            var incomeDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            return incomeDao.UpdateAmount(info);
        }

        /// <summary>
        /// 更新无效状态
        /// </summary>
        /// 业务调用 InefficacyWaybillList.aspx 异常订单确定置为无效是否需要返款
        public bool UpdateInefficacyStatus(long waybillNo, int inefficacyStatus)
        {
            var incomeDao = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            return incomeDao.UpdateInefficacyStatus(waybillNo, inefficacyStatus);
        }

        /// 业务调用 AddExpressOrder.aspx 快递单计算重量
        public int GetMerchantDeliverFee(int merchantId)
        {
            var merchantDao = ServiceLocator.GetService<IMerchantDao>();

            return merchantDao.GetMerchantDeliverFee(merchantId);
        }

        /// 业务调用 多个地方调用
        public decimal GetVolumeParmer(int MerchantID)
        {
            var merchantDao = ServiceLocator.GetService<IMerchantDao>();

            return merchantDao.GetVolumeParmer(MerchantID);
        }

        /// <summary>
        /// 获取商家是否会物品类结算
        /// </summary>
        /// <param name="MerchantID"></param>
        /// <param name="DistributionCode"></param>
        /// <returns></returns>
        public int GetMerchantIsCategory(int MerchantID, string DistributionCode)
        {
            SearchCondition condition = new SearchCondition
            {
                MerchantID = MerchantID,
                DistributionCode = DistributionCode
            };
            PageInfo pi=new PageInfo(100);
            DataTable dt = merchantFeeDao.GetMerchantDeliverFeeList(condition, pi);
            if (dt == null || dt.Rows.Count <= 0)
                return 0;
            return DataConvert.ToInt(dt.Rows[0]["IsCategory"], 0);
        }

        /// <summary>
        /// 获取商家取重取向
        /// </summary>
        /// <param name="MerchantID"></param>
        /// <param name="DistributionCode"></param>
        /// <returns></returns>
        public int GetMerchantWeightType(int MerchantID, string DistributionCode)
        {
            SearchCondition condition = new SearchCondition
            {
                MerchantID = MerchantID,
                DistributionCode=DistributionCode
            };
            PageInfo pi = new PageInfo(100);
            DataTable dt = merchantFeeDao.GetMerchantDeliverFeeList(condition, pi);
            if (dt == null || dt.Rows.Count <= 0)
                return -1;
            return DataConvert.ToInt(dt.Rows[0]["WeightType"], -1);
        }

        public bool ExistsExpressReceiveFeeInfo(long waybillNo)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.ExistsExpressReceiveFeeInfo(waybillNo);
        }

        public bool AddExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.AddExpressReceiveFeeInfo(receiveModel);
        }

        public bool UpdateExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.UpdateExpressReceiveFeeInfo(receiveModel);
        }

        public bool DeleteExpressFeeInfo(long waybillNo)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.DeleteExpressFeeInfo(waybillNo); 
        }

        public bool ExistsProjectReceiveFeeInfo(long waybillNo)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.ExistsProjectReceiveFeeInfo(waybillNo); 
        }

        public bool UpdateProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.UpdateProjectReceiveFeeInfo(receiveModel); 
        }

        public bool InsertProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.InsertProjectReceiveFeeInfo(receiveModel); 
        }

        public bool DeleteProjectReceiveFeeInfo(long waybillNo)
        {
            var receiveDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveDao.DeleteProjectReceiveFeeInfo(waybillNo); 
        }

        public bool UpdateExpressParameter(long waybillNo, Dictionary<EnumExpressParameter, string> parameters)
        {
            return false;
            //var service = ServiceLocator.GetService<IExpressChangeDataService>();

            //foreach (var item in parameters)
            //{
            //    //修改分拣中心
            //    if (item.Key == "SortingCenterId")
            //    {
            //        service.ChangeSortingCenter(waybillNo,DataConvert.ToInt(item.Value));

            //        //FMS_IncomeBaseInfo修改ExpressCompanyID分拣中心
            //        //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //        //薛毅提供接口
            //    }

            //    //修改站点
            //    if (item.Key == "expressCompanyid")
            //    {
            //        service.ChangeDeliverStation(waybillNo, DataConvert.ToInt(item.Value));

            //        //FMS_IncomeBaseInfo修改DeliverStationID，TopCODCompanyID
            //        //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //        //FMS_CODBaseInfo把原纪录冲掉，新生成一条纪录。
            //        //重新计算提成
            //        //薛毅提供接口
            //    }

            //    //修改付款方向(1寄方付,2到方付)
            //    if (item.Key == "PaymentType")
            //    {
            //        service.ChangePaymentType(waybillNo, DataConvert.ToInt(item.Value));

            //        //修改快递收款查询表
            //    }

            //    //修改结算方式(1现结，3月结)
            //    if (item.Key == "AccountType")
            //    {
            //        service.ChangeAccountType(waybillNo, DataConvert.ToInt(item.Value));

            //        //修改快递收款查询表
            //    }

            //    //修改支付方式(1现金,2POS机)
            //    if (item.Key == "AcceptType")
            //    {
            //        service.ChangeAcceptType(waybillNo, DataConvert.ToInt(item.Value));

            //        //FMS_IncomeBaseInfo修改AcceptType分拣中心
            //        //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //    }

            //    //修改运费
            //    if (item.Key == "deliverFee")
            //    {
            //        service.ChangeDeliverFee(waybillNo, DataConvert.ToDecimal(item.Value));

            //        //修改快递收款查询
            //    }

            //    //修改商家
            //    if (item.Key == "merchantId")
            //    {
            //        service.ChangeMerchant(waybillNo, DataConvert.ToInt(item.Value));

            //        //收入直接改重新计算费用
            //        //支出添加一条抵消，再生成一条重新计算费用
            //    }

            //    //修改保价金额
            //    if (item.Key == "ProtectPrices")
            //    {
            //        service.ChangeProtectedPrices(waybillNo, DataConvert.ToDecimal(item.Value));

            //        //支出直接改
            //        //收入直接改重新计算配送费
            //    }

            //    //修改保价费
            //    if (item.Key == "ProtectFee")
            //    {

            //    }

            //    //修改代收货款
            //    if (item.Key == "Goodspayment")
            //    {
            //        service.ChangeGoodsPayment(waybillNo, DataConvert.ToDecimal(item.Value));

            //        //收入直接改重新计算费用
            //        //支出添加一条抵消，再生成一条重新计算费用
            //    }

            //    //修改代收货款手续费
            //    if (item.Key == "Factorage")
            //    {

            //    }

            //    //修改取件重量
            //    if (item.Key == "FinancialWeight")
            //    {
            //        service.ChangeAccountWeight(waybillNo, DataConvert.ToDecimal(item.Value));

            //        //收入直接改重新计算
            //        //修改提成
            //    }

            //    //修改客户重量
            //    if (item.Key == "MerchantWeight")
            //    {

            //    }
            //}
        }
    }
}
