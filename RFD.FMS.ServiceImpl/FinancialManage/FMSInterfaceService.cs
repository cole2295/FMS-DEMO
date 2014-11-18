using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.DAL.Oracle.BasicSetting;
using RFD.FMS.DAL.Oracle.COD;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.MODEL.COD;
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
using RFD.FMS.MODEL.Enumeration;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class FMSInterfaceService : IFMSInterfaceService
    {
        private RFD.FMS.ServiceImpl.Mail.Mail mail = new RFD.FMS.ServiceImpl.Mail.Mail();
        ICODBaseInfoDao _cODBaseInfoDao=new CODBaseInfoDao() ;
         IIncomeBaseInfoDao service = new IncomeBaseInfoDao();
         IIncomeFeeInfoDao feeInfoDao = new IncomeFeeInfoDao();
         IMerchantDao merchantDao = new MerchantDao();
         IFMS_ReceiveFeeInfoDao receiveDao = new FMS_ReceiveFeeInfoDao();
        public bool AdCODBaseInfoList(List<FMS_CODBaseInfo> modelList)
        {
            if(modelList==null || modelList.Count<=0)
                return true;
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
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


            return _cODBaseInfoDao.Add(model);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_CODBaseInfo GetModel(Int64 id)
        {


            return _cODBaseInfoDao.GetModel(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_CODBaseInfo GetCODBaseInfoModelByWaybillNO(Int64 waybillNo)
        {
            //CODBaseInfoDao service = new CODBaseInfoDao();

            return _cODBaseInfoDao.GetModelByWaybillNO(waybillNo);
        }

        /// <summary>
        /// 根据条件得到一个对象实体集
        /// </summary>
        public List<FMS_CODBaseInfo> GetCODBaseInfoModelList(Dictionary<string, object> searchParams)
        {


            return _cODBaseInfoDao.GetModelList(searchParams);
        }

        /// <summary>
        /// 更改金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateAmountByID(FMS_CODBaseInfo info)
        {


            return _cODBaseInfoDao.UpdateAmountByID(info);
        }

        /// <summary>
        /// 将制定ID置为停用isdeleted=1
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateIsDeletedByID(FMS_CODBaseInfo info)
        {


            return _cODBaseInfoDao.UpdateIsDeletedByID(info);
        }

        public bool ExistsIncomeBaseInfo(Int64 incomeid)
        {
            

            return service.Exists(incomeid);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeBaseInfo(FMS_IncomeBaseInfo model)
        {
            

            return service.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateIncomeBaseInfo(FMS_IncomeBaseInfo model)
        {
            

            return service.Update(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateIncomeBaseInfoStatus(FMS_IncomeBaseInfo model)
        {
            

            return service.UpdateStatus(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateBackStatus(FMS_IncomeBaseInfo model)
        {
            

            return service.UpdateBackStatus(model);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeFeeInfo(FMS_IncomeFeeInfo model)
        {



            return feeInfoDao.Add(model);
        }

        /// <summary>
        /// 归班更新一条数据
        /// </summary>
        public bool UpdateInComeFeeInfoByBackStation(FMS_IncomeFeeInfo model)
        {


            return feeInfoDao.UpdateByBackStation(model);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// 业务调用 WaybillAdd.aspx 运单修改
        public FMS_IncomeBaseInfo GetIncomeBaseInfoModelByWaybillNO(long waybillNo)
        {
            

            return service.GetModelByWaybillNO(waybillNo);
        }

        /// <summary>
        /// 更新金额
        /// </summary>
        /// 业务调用 WaybillAdd.aspx 运单修复
        public bool UpdateIncomeBaseInfoAmount(FMS_IncomeBaseInfo info)
        {
            

            return service.UpdateAmount(info);
        }

        /// <summary>
        /// 更新无效状态
        /// </summary>
        /// 业务调用 InefficacyWaybillList.aspx 异常订单确定置为无效是否需要返款
        public bool UpdateInefficacyStatus(long waybillNo, int inefficacyStatus)
        {
            

            return service.UpdateInefficacyStatus(waybillNo, inefficacyStatus);
        }

        /// 业务调用 AddExpressOrder.aspx 快递单计算重量
        public int GetMerchantDeliverFee(int merchantId)
        {
            

            return merchantDao.GetMerchantDeliverFee(merchantId);
        }

        /// 业务调用 多个地方调用
        public decimal GetVolumeParmer(int MerchantID)
        {


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
            SearchCondition condition = null;
            PageInfo pi = new PageInfo(100);
            RFD.FMS.Domain.FinancialManage.IFMS_MerchantDeliverFeeDao merchantFeeDao = new DAL.Oracle.FinancialManage.FMS_MerchantDeliverFeeDao();
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
            SearchCondition condition = null;
            PageInfo pi = new PageInfo(100);
            RFD.FMS.Domain.FinancialManage.IFMS_MerchantDeliverFeeDao merchantFeeDao = new DAL.Oracle.FinancialManage.FMS_MerchantDeliverFeeDao();
            DataTable dt = merchantFeeDao.GetMerchantDeliverFeeList(condition, pi);
            if (dt == null || dt.Rows.Count <= 0)
                return -1;
            return DataConvert.ToInt(dt.Rows[0]["WeightType"], -1);
        }

        public bool ExistsExpressReceiveFeeInfo(long waybillNo)
        {
           

            return receiveDao.ExistsExpressReceiveFeeInfo(waybillNo);
        }

        public bool AddExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel)
        {
            

            return receiveDao.AddExpressReceiveFeeInfo(receiveModel);
        }

        public bool UpdateExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel)
        {
            

            return receiveDao.UpdateExpressReceiveFeeInfo(receiveModel);
        }

        public bool DeleteExpressFeeInfo(long waybillNo)
        {
            

            return receiveDao.DeleteExpressFeeInfo(waybillNo); 
        }

        public bool ExistsProjectReceiveFeeInfo(long waybillNo)
        {
            

            return receiveDao.ExistsProjectReceiveFeeInfo(waybillNo); 
        }

        public bool UpdateProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel)
        {
            

            return receiveDao.UpdateProjectReceiveFeeInfo(receiveModel); 
        }

        public bool InsertProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel)
        {
            

            return receiveDao.InsertProjectReceiveFeeInfo(receiveModel);
        }

        public bool DeleteProjectReceiveFeeInfo(long waybillNo)
        {
            

            return receiveDao.DeleteProjectReceiveFeeInfo(waybillNo); 
        }


        public bool UpdateExpressParameter(long waybillNo, Dictionary<EnumExpressParameter, string> parameters)
        {
            
            IExpressChangeDataService service = new ExpressChangeDataService();
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    foreach (var item in parameters)
                    {
                        //修改分拣中心
                        if (item.Key == EnumExpressParameter.SortingCenterId)
                        {
                            if (!service.ChangeSortingCenter(waybillNo, DataConvert.ToInt(item.Value)))
                                return false;
                            continue;
                            //FMS_IncomeBaseInfo修改ExpressCompanyID分拣中心
                            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
                            //薛毅提供接口
                        }

                        //修改站点
                        if (item.Key == EnumExpressParameter.ExpressCompanyid)
                        {
                            if (!service.ChangeDeliverStation(waybillNo, DataConvert.ToInt(item.Value)))
                                return false;
                            continue;

                            //FMS_IncomeBaseInfo修改DeliverStationID，TopCODCompanyID
                            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
                            //FMS_CODBaseInfo把原纪录冲掉，新生成一条纪录。
                            //重新计算提成
                            //薛毅提供接口
                        }

                        //修改付款方式(1寄方付,2到方付)
                        if (item.Key == EnumExpressParameter.AccountType)
                        {
                            if (!service.ChangePaymentType(waybillNo, DataConvert.ToInt(item.Value)))
                                return false;
                            continue;
                            //修改快递收款查询表
                        }

                        //修改结算方式(1现结，3月结)
                        if (item.Key == EnumExpressParameter.TransferPayType)
                        {
                            if (service.ChangeAccountType(waybillNo, DataConvert.ToInt(item.Value)) == false)
                                return false;
                                continue;
                            //修改快递收款查询表
                        }

                        //修改支付方式(1现金,2POS机)
                        if (item.Key == EnumExpressParameter.AcceptType)
                        {
                            if (!service.ChangeAcceptType(waybillNo, item.Value))
                                return false;
                            continue;
                            //FMS_IncomeBaseInfo修改AcceptType分拣中心
                            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
                        }

                        //修改运费
                        if (item.Key == EnumExpressParameter.DeliverFee)
                        {
                            if (!service.ChangeDeliverFee(waybillNo, DataConvert.ToDecimal(item.Value)))
                                return false;
                            continue;
                            //修改快递收款查询
                        }

                        //修改商家
                        if (item.Key == EnumExpressParameter.MerchantId)
                        {
                            if (!service.ChangeMerchant(waybillNo, DataConvert.ToInt(item.Value)))
                                return false;
                            continue;
                            //收入直接改重新计算费用
                            //支出添加一条抵消，再生成一条重新计算费用
                            // 修该 FMS_codbaseinfo
                        }

                        //修改保价金额
                        if (item.Key == EnumExpressParameter.ProtectPrices)
                        {
                            if (!service.ChangeProtectedPrices(waybillNo, DataConvert.ToDecimal(item.Value)))
                                return false;
                            continue;
                            //支出直接改
                            // 修该 FMS_codbaseinfo
                            //收入直接改重新计算配送费
                        }

                        //修改保价费
                        if (item.Key == EnumExpressParameter.ProtectFee)
                        {
                            // 改incomebaseinfo 改保价金额,incomefeeinfo 改计算状态 isaccount  codbaseinfo 改保价金额，  expressreceive 改保价金额
                            if (!service.ChangeProtectFee(waybillNo, DataConvert.ToDecimal(item.Value)))
                                return false;
                            continue;
                        }

                        //修改代收货款
                        if (item.Key == EnumExpressParameter.Goodspayment)
                        {
                            if (!service.ChangeGoodsPayment(waybillNo, DataConvert.ToDecimal(item.Value)))
                                return false;
                            continue;
                            //收入直接改重新计算费用 incomebaseinfo表 needpayamount
                            //支出添加一条抵消，再生成一条重新计算费用
                        }

                        //修改代收货款手续费
                        if (item.Key == EnumExpressParameter.Factorage)
                        {
                            //incomefeeinfo isacount = 0
                            if (!service.ChangeFactorage(waybillNo, DataConvert.ToDecimal(item.Value)))
                                return false;
                            continue;
                        }

                        //修改取件重量
                        if (item.Key == EnumExpressParameter.FinancialWeight)
                        {
                            if (!service.ChangeAccountWeight(waybillNo, DataConvert.ToDecimal(item.Value)))
                                return false;
                            continue;
                            //收入直接改重新计算incomebaseinfo

                        }

                        //修改客户重量
                        if (item.Key == EnumExpressParameter.MerchantWeight)
                        {
                            // 改incomebaseinfo 改重量,incomefeeinfo 改计算状态 isaccount  codbaseinfo 改重量，  expressreceive 改重量
                            if (!service.ChangeMerchantWeight(waybillNo, DataConvert.ToDecimal(item.Value)))
                                return false;
                            continue;
                        }
                    }
                    work.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                mail.SendMailToUser("财务快递信息修改接口异常", "运单号：" + waybillNo + "\t\n操作失败" + ex.Message + ex.StackTrace, "liyongf@rufengda.com;zengwei@vancl.cn");

                return false;
            }
        }
    }
}
