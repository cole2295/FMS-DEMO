using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IDeliveryPriceDao
    {
        int AddDeliveryPrice(RFD.FMS.MODEL.FMS_CODLine codLine, string cODLineNO);
        bool AddDeliveryPriceLog(string codLineNo, string createBy, string logText, int logType);
        int AddEffectDeliveryPrice(RFD.FMS.MODEL.FMS_CODLine codLine);
        bool DeleteDeliveryPrice(string cODLineNO, string updateBy);
        System.Collections.Generic.IList<RFD.FMS.MODEL.FMS_CODLine> GetBackList();
        System.Data.DataTable GetCodLine(int expressCompanyId, int areaType, string distributionCode);
        System.Data.DataTable GetCodLineHistory(int year, int month, int expressCompanyId, int areaType, string distributionCode);
        System.Collections.Generic.List<RFD.FMS.MODEL.FMS_CODLine> GetCODLineWaitEffect(string Date);
        System.Data.DataTable GetDeliveryPriceHistoryList(string lineNoXml);
        System.Data.DataTable GetDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId, string distributionCode, int IsCod, RFD.FMS.MODEL.PageInfo pi, bool isPage);
        System.Data.DataTable GetDeliveryPriceLog(string lineNo, string dateStr, string dateEnd, string distributionCode);
        System.Collections.Generic.List<RFD.FMS.MODEL.FMS_CODLine> GetEffectCodLine(DateTime effectDateStr, DateTime effectDateEnd);
        System.Data.DataTable GetEffectDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId, string distributionCode, int IsCod, RFD.FMS.MODEL.PageInfo pi, bool isPage);
        System.Data.DataSet GetExportData();
        System.Data.DataTable GetListByCodLineNo(string codLineNo);
        System.Data.DataTable GetListByEffectCodLineNo(string codLineNo);
        System.Data.DataTable GetMerchant(string disCode);
        bool Insert(System.Collections.Generic.IList<RFD.FMS.MODEL.FMS_CODLine> codLineList, string month, string year);
        bool UpdateDeliveryPrice(RFD.FMS.MODEL.FMS_CODLine codLine);
        bool UpdateDeliveryPriceAuditStatus(string cODLineNO, string auditBy, int auditStatus);
        bool UpdateEffectCodLine(RFD.FMS.MODEL.FMS_CODLine codLine);
        bool UpdateEffectDeliveryPriceAuditStatus(string cODLineNO, string auditBy, int auditStatus);
        bool UpdateLineForCODLine(RFD.FMS.MODEL.FMS_CODLine clwe);
        bool UpdateLineForCODLineWaitEffect(RFD.FMS.MODEL.FMS_CODLine clwe);
        int UpdateToDelete(string year, string month);
    }
}
