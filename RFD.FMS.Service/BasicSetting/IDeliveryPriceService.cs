using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IDeliveryPriceService
    {
        int AddDeliveryPrice(RFD.FMS.MODEL.FMS_CODLine codLine);
        int AddEffectDeliveryPrice(RFD.FMS.MODEL.FMS_CODLine codLine);
        bool DeleteDeliveryPrice(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> keyValuePairs, string deleteBy);
        bool ExportDeliveryPrice(System.Data.DataTable dt, int createBy, out System.Data.DataTable dtError, string distributionCode);
        System.Data.DataTable GetDeliveryPriceHistoryList(string lineNo);
        System.Data.DataTable GetDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId, string distributionCode, int IsCod, ref RFD.FMS.MODEL.PageInfo pi, bool isPage);
        System.Data.DataTable GetDeliveryPriceLog(string lineNo, string dateStr, string dateEnd, string distributionCode);
        RFD.FMS.MODEL.FMS_CODLine GetListByCodLineNo(string codLineNo);
        RFD.FMS.MODEL.FMS_CODLine GetListByEffectCodLineNo(string codLineNo);
        System.Data.DataTable GetMerchant(string disCode);
        bool UpdateDeliveryPrice(RFD.FMS.MODEL.FMS_CODLine codLine);
        bool UpdateDeliveryPriceAuditStatus(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> keyValuePairs, string auditBy, int auditStatus);
        bool UpdateEffectCodLine(RFD.FMS.MODEL.FMS_CODLine codLine);
        bool UpdateEffectDeliveryPriceAuditStatus(System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string, string>> keyValuePairs, string auditBy, int auditStatus);

        #region 配送费计算查询
        /// <summary>
        /// 获取可用的Cod线路价格
        /// </summary>
        /// <returns></returns>
        DataTable GetCodLine(int expressCompanyId, int areaType, string distributionCode);

        /// <summary>
        /// 返回当前月前4个月的历史
        /// </summary>
        /// <returns></returns>
        DataTable GetCodLineHistory(int year, int month, int expressCompanyId, int areaType, string distributionCode);
        #endregion

        #region cod价格变更校对
        List<FMS_CODLine> GetEffectCodLine(DateTime effectDate);
        #endregion

        #region CODLine备份
        bool Insert(IList<FMS_CODLine> codLineList, string month, string year);
        int UpdateToDelete(string year, string month);
        IList<FMS_CODLine> GetBackList();
        #endregion

        #region COD待生效
        List<FMS_CODLine> GetCODLineWaitEffect(string Date);
        bool UpdateLine(FMS_CODLine clwe);
        #endregion
    }
}
