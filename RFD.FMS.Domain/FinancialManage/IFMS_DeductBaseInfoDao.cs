using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IFMS_DeductBaseInfoDao
    {
        long Add(RFD.FMS.MODEL.FinancialManage.FMS_DeductSubBaseInfo model);
        
        void DeleteOldDeduct(long waybillNO, int deductType);
        System.Data.DataTable GetExpressReceiveFormula(string stationIds);
        System.Collections.Generic.IList<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> GetExpressReceiveModel(string DistributionCodeList);
        System.Data.DataTable GetExpressSendFormula(string stationIds);
        System.Collections.Generic.IList<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> GetExpressSendModel(string DistributionCodeList);
        System.Data.DataTable GetProjectSendFormula(string stationIds, string categoryCodes);
        System.Collections.Generic.IList<RFD.FMS.MODEL.FinancialManage.FMS_DeductBaseInfo> GetProjectSendModel(string DistributionCodeList);
        decimal GetStationDefaultAreaDeduct(int stationId);
        bool IsCalculateOK(long id);

        IList<long> CheckRepeatDeduct();
        IList<long> RepairExpressDelete();
        IList<long> ClearNotEvalDeduct();
        IList<long> RepairExpressDate();
        IList<long> RepairFalseDelete();
        bool IsDeleted(long deductId);


        /// <summary>
        /// 查询项目单派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        IList<FMS_DeductBaseInfo> GetProjectSendModelByWaybillNO(long WaybillNO);

        void UpdateSubAreaDeduct(FMS_DeductSubBaseInfo model);

        /// <summary>
        /// 查询快递派件计算提成
        /// </summary>
        /// <returns>单据集合</returns>
        IList<FMS_DeductBaseInfo> GetExpressSendModelByWaybillNO(long WaybillNO);
    }
}
