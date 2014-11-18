using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using RFD.FMS.DAL.FinancialManage;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Model.UnionPay;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Model.FinancialManage;
using System.ServiceModel;

namespace RFD.FMS.Service.FinancialManage
{
    [ServiceContract]
    public interface IFMSInterfaceService
    {
        [OperationContract]
        bool AdCODBaseInfoList(List<FMS_CODBaseInfo> modelList);

        [OperationContract]
        int AddCODBaseInfo(FMS_CODBaseInfo model);
       
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        [OperationContract]
        FMS_CODBaseInfo GetModel(long id);

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        [OperationContract]
        FMS_CODBaseInfo GetCODBaseInfoModelByWaybillNO(Int64 waybillNo);
       
        /// <summary>
        /// 根据条件得到一个对象实体集
        /// </summary>
        [OperationContract]
        List<FMS_CODBaseInfo> GetCODBaseInfoModelList(Dictionary<string, object> searchParams);

        /// <summary>
        /// 更改金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateAmountByID(FMS_CODBaseInfo info);

        /// <summary>
        /// 将制定ID置为停用isdeleted=1
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateIsDeletedByID(FMS_CODBaseInfo info);

        [OperationContract]
        bool ExistsIncomeBaseInfo(Int64 incomeid);

        /// <summary>
        /// 增加一条数据
        /// </summary>
        [OperationContract]
        int AddIncomeBaseInfo(FMS_IncomeBaseInfo model);

        /// <summary>
        /// 更新一条数据
        /// </summary>
        [OperationContract]
        bool UpdateIncomeBaseInfo(FMS_IncomeBaseInfo model);

        /// <summary>
        /// 更新一条数据
        /// </summary>
        [OperationContract]
        bool UpdateIncomeBaseInfoStatus(FMS_IncomeBaseInfo model);

        /// <summary>
        /// 更新一条数据
        /// </summary>
        [OperationContract]
        bool UpdateBackStatus(FMS_IncomeBaseInfo model);

        /// <summary>
        /// 增加一条数据
        /// </summary>
        [OperationContract]
        int AddIncomeFeeInfo(FMS_IncomeFeeInfo model);

        /// <summary>
        /// 归班更新一条数据
        /// </summary>
        [OperationContract]
        bool UpdateInComeFeeInfoByBackStation(FMS_IncomeFeeInfo model);

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// 业务调用 WaybillAdd.aspx 运单修改
        [OperationContract]
        FMS_IncomeBaseInfo GetIncomeBaseInfoModelByWaybillNO(long waybillNo);

        /// <summary>
        /// 更新金额
        /// </summary>
        /// 业务调用 WaybillAdd.aspx 运单修复
        [OperationContract]
        bool UpdateIncomeBaseInfoAmount(FMS_IncomeBaseInfo info);

        /// <summary>
        /// 更新无效状态
        /// </summary>
        /// 业务调用 InefficacyWaybillList.aspx 异常订单确定置为无效是否需要返款
        [OperationContract]
        bool UpdateInefficacyStatus(long waybillNo, int inefficacyStatus);

        /// 业务调用 AddExpressOrder.aspx 快递单计算重量
        [OperationContract]
        int GetMerchantDeliverFee(int merchantId);

        /// 业务调用 多个地方调用
        [OperationContract]
        decimal GetVolumeParmer(int MerchantID);

        int GetMerchantIsCategory(int MerchantID, string DistributionCode);

        int GetMerchantWeightType(int MerchantID, string DistributionCode);

        #region 财务收款查询相关

        [OperationContract]
        bool ExistsExpressReceiveFeeInfo(long waybillNo);

        [OperationContract]
        bool AddExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel);

        [OperationContract]
        bool UpdateExpressReceiveFeeInfo(FMS_IncomeExpressReceiveFeeInfo receiveModel);

        [OperationContract]
        bool DeleteExpressFeeInfo(long waybillNo);

        [OperationContract]
        bool ExistsProjectReceiveFeeInfo(long waybillNo);

        [OperationContract]
        bool UpdateProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel);

        [OperationContract]
        bool InsertProjectReceiveFeeInfo(FMS_IncomeReceiveFeeInfo receiveModel);

        [OperationContract]
        bool DeleteProjectReceiveFeeInfo(long waybillNo);

        [OperationContract]
       bool UpdateExpressParameter(long waybillNo, Dictionary<EnumExpressParameter, string> parameters);

        #endregion
    }
}