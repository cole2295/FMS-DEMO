using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.Util;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IAreaExpressLevelDao
    {
        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
		DataTable SearchArea(string provinceId, string cityId, string areaId, string stationId, string merchantId, string distributionCode,ref PageInfo pi);

        /// <summary>
        /// 查询区域类型
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        DataTable SearchAreaType(string areaId, string distributionCode,ref PageInfo pi);

        /// <summary>
        /// 根据唯一ID，主键查询
        /// </summary>
        /// <param name="autoId"></param>
        /// <returns></returns>
        DataTable SearchAreaTypeByAutoId(string autoId);

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="areaExpressLevel"></param>
        /// <returns></returns>
        bool AddAreaType(AreaExpressLevel areaExpressLevel);

        /// <summary>
        /// 更新区域
        /// </summary>
        /// <param name="areaExpressLevel"></param>
        /// <returns></returns>
        bool UpdateAreaType(AreaExpressLevel areaExpressLevel, out int autoId);

        /// <summary>
        /// 删除区域类型
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        bool DeleteAreaType(string autoId, string updateBy);

        #region 区域类型审批
        //查询区域类型
        DataTable SearchAreaExpressCompanyLevel(int status, string areaid, string cityid, string provinceid, int expresscompanyid, int areatype, int type, string warehouseid, int merchantid,string distributionCode, ref PageInfo pi);

        /// <summary>
        ///查询区域信息
        /// </summary>
        /// <param name="areaid"></param>
        /// <returns></returns>
        DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, string distributionCode);

        /// <summary>
        /// 查询区域详细信息
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="status"></param>
        /// <param name="expresscompanyid"></param>
        /// <param name="areatype"></param>
        /// <param name="warehousetype"></param>
        /// <param name="warehouseid"></param>
        /// <param name="merchantid"></param>
        /// <param name="productid"></param>
        /// <returns></returns>
        DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, int expresscompanyid, int areatype, int warehousetype, string warehouseid, int merchantid);

        //设置审批
        bool SetAreaExpressCompanyLeverAudit(int autoid, DateTime doDate, int auditBy, DateTime auditTime);

        bool SetAreaExpressCompanyLeverAuditEx(int autoid, DateTime doDate, int auditBy, DateTime auditTime, int auditstatus);

        //添加日志
        bool AddAreaExpLevelLog(AreaExpressLevelLog areaExpressLevelLog);
        #endregion

        DataTable AreaExpressCompanyLevelNum(int num, DateTime nowDate);

        bool AreaExpressCompanyLevelUpdate(int autoid);

        bool AreaExpressCompanyLevelAdd(int autoid);

        bool AreaExpressCompanyLevelDel(int autoid);

        /// <summary>
        /// 得到批量导入的基础信息
        /// </summary>
        /// <returns></returns>
        DataSet GetExportData();

        //设置置回区域类型
        bool ReSetAreaExpressCompanyLevel(int autoid, int auditby, DateTime audittime);

        DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode, PageInfo pi);

        DataTable SearchAreaTypeExprotList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode);
        /// <summary>
        /// 查询操作日志
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string wareHouseType, string merchantId, string productId, string distributionCode);

        DataTable GetAreaType(int expressComapnyId, int merchantId);
        DataTable SearchSecondAreaType(string areaID, string areaType, string stationID, string merchantID, string distributionCode,ref PageInfo pi);
    }
}
