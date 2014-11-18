using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IAreaExpressLevelService
    {
        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        DataTable SearchArea(string provinceId, string cityId, string areaId, string stationId, string merchantId, string distributionCode,ref PageInfo pi);

        /// <summary>
        /// 区域类型查询
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        DataTable SearchAreaType(string areaId, string distributionCode,ref PageInfo pi);

        /// <summary>
        /// 添加区域类型
        /// </summary>
        /// <param name="areaExpressLevels"></param>
        /// <returns></returns>
        bool AddAreaType(List<AreaExpressLevel> areaExpressLevels, out string msg);

        /// <summary>
        /// 更新区域类型
        /// </summary>
        /// <param name="areaExpressLevels"></param>
        /// <returns></returns>
        bool UpdateAreaType(List<AreaExpressLevel> areaExpressLevels, out string msg);

        /// <summary>
        /// 删除区域类型
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        bool DeleteAreaType(IList<KeyValuePair<string, string>> keyValuePairs, string updateBy);

        /// <summary>
        /// 批量导入区域类型
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        bool ExportAreaType(DataTable dt, int createBy, out DataTable dtError, string distributionCode);

        #region 区域类型审批

        //查询区域类型
        DataTable SearchAreaExpressCompanyLevel(int status, string areaid, string cityid, string provinceid, int expresscompanyid, int areatype, int type, string warehouseid, int merchantid,string distributionCode, ref PageInfo pi);

        //查询区域类型信息
        DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, string distributionCode);

        //查询区域类型详细信息
        DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, int expresscompanyid, int areatype, int warehousetype, string warehouseid, int merchantid);

        bool SetAreaExpressCompanyLeverAudit(string areaid, DateTime doDate, AreaExpressLevelLog areaExpressLevelLog);

        /// <summary>
        /// 添加审批审核
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="doDate"></param>
        /// <param name="areaExpressLevelLog"></param>
        /// <returns></returns>
        bool SetAreaExpressCompanyLeverAuditEx(string areaid, DateTime doDate, int auditstatus, AreaExpressLevelLog areaExpressLevelLog);

        #endregion

        DataTable AreaExpressCompanyLevelNum(int num, DateTime nowDate);

        bool AreaExpressCompanyLevelUpdate(int autoid, string areaid, int expresscompanyid, int areatype, string warehouseid, int warehousetype, int merchant, int productid);

        bool AreaExpressCompanyLevelAdd(int autoid, string areaid, int expresscompanyid, int areatype, string warehouseid, int warehousetype, int merchant, int productid);

        bool AreaExpressCompanyLevelDel(int autoid, string areaid, int expresscompanyid, int areatype, string warehouseid, int warehousetype, int merchant, int productid);

        //设置需置回的地区
        bool ReSetAreaExpressCompanyLevel(string areaid, AreaExpressLevelLog areaExpressLevelLog);

        /// <summary>
        /// 区域类型查询
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="CityID"></param>
        /// <param name="AreaID"></param>
        /// <param name="ExpressCompanyID"></param>
        /// <param name="AreaType"></param>
        /// <param name="WareHouse"></param>
        /// <param name="MerchantID"></param>
        /// <param name="ProductID"></param>
        /// <param name="?"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        DataTable SearchAreaTypeList(string provinceId, string CityID, string AreaID, string ExpressCompanyID, string AreaType, string WareHouse, string MerchantID, string AuditStatus,string distributionCode, ref PageInfo pi);

        DataTable SearchAreaTypeExprotList(string provinceId, string CityID, string AreaID, string ExpressCompanyID, string AreaType, string WareHouse, string MerchantID, string AuditStatus, string distributionCode);
        /// <summary>
        /// 查询操作日志
        /// </summary>
        /// <param name="AreaID"></param>
        /// <param name="ExpressCompanyID"></param>
        /// <param name="WareHouse"></param>
        /// <param name="WareHouseType"></param>
        /// <param name="MerchantID"></param>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        DataTable SearchAreaTypeLog(string AreaID, string ExpressCompanyID, string WareHouse, string WareHouseType, string MerchantID, string ProductID, string distributionCode);

        DataTable GetAreaType(int expressComapnyId, int merchantId);

        DataTable SearchSecondAreaType(string areaID, string areaType, string stationID, string merchantID, string distributionCode,ref PageInfo pi);
    }
}
