using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IAreaExpressLevelIncomeService
    {
        void MapAreaExpressLevelIncome(AreaExpressLevelIncome info, ref AreaExpressLevelIncome newInfo);

        //查询区域类型
        DataTable SearchAreaMerchantLevel(int status, string areaid, string cityid, string provinceid, int merchantId, int areatype, int expresscompanyid, string distributionCode, ref PageInfo pi);

        //查询区域类型详细信息
        DataTable SearchAreaMerchantLevelDetail(string areaid, int status, string distributionCode);

        //查询区域类型详细信息1
        DataTable SearchAreaMerchantLevelDetail(string areaid, int status, int merchantId, int areatype, int expresscompanyid, string distributionCode);

        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
		DataTable SearchArea(string provinceId, string cityId, string areaId, string merchantId, string distributionCode);

        /// <summary>
        /// 区域类型查询
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        DataTable SearchAreaType(string areaId, string distributionCode);

        /// <summary>
        /// 添加区域类型
        /// </summary>
        /// <param name="areaExpressLevelIncomes"></param>
        /// <returns></returns>
        bool AddAreaType(List<AreaExpressLevelIncome> areaExpressLevelIncomes, out string msg);

        /// <summary>
        /// 更新区域类型
        /// </summary>
        /// <param name="areaExpressLevelIncomes"></param>
        /// <returns></returns>
        bool UpdateAreaType(List<AreaExpressLevelIncome> areaExpressLevelIncomes, out string msg);

        /// <summary>
        /// 更新区域类型
        /// </summary>
        /// <param name="areaExpressLevelIncomes"></param>
        /// <returns></returns>
        bool UpdateAreaTypeV2(List<AreaExpressLevelIncome> areaExpressLevelIncomes);

        /// <summary>
        /// 更新配送商
        /// </summary>
        /// <param name="areaExpressLevelIncomes"></param>
        /// <returns></returns>
        bool UpdateExpressV2(List<AreaExpressLevelIncome> areaExpressLevelIncomes);
        /// <summary>
        /// 删除区域类型
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        bool DeleteAreaType(IList<KeyValuePair<string, string>> keyValuePairs, int updateBy);

        //设置生效
        bool SetAreaMerchantLeverAudit(string areaid, DateTime doDate, AreaExpressLevelIncomeLog areaExpressLevelIncomeLog);

        //设置生效new
        bool SetAreaMerchantLeverAuditEx(string areaid, DateTime doDate, int auditstatus, AreaExpressLevelIncomeLog areaExpressLevelIncomeLog);

        //返回待生效的区域
        DataTable AreaMerchantLevelNum(int num, DateTime nowDate);

        //更新收入区域类型
        bool AreaMerchantLevelUpdate(int autoid, string areaid, int expresscompanyid, int areatype, string warehouseid, int expressId);

        //添加收入区域类型
        bool AreaMerchantLevelAdd(int autoid, string areaid, int expresscompanyid, int areatype, string warehouseid, int expressId);

        //删除收入区域类型
        bool AreaMerchantLevelDel(int autoid, string areaid, int expresscompanyid, int areatype, string warehouseid, int expressId);

        //设置置回
        bool ReSetAreaMerchantLevel(string areaid, AreaExpressLevelIncomeLog areaExpressLevelIncomeLog);


        //批量导入
        bool ExportAreaType(DataTable dt, int createBy, out DataTable dtError, string distributionCode,int expressCompanyId);

        //获取分拣中心
        DataTable GetSortingCenter();

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
        DataTable SearchAreaTypeLog(string AreaID, string ExpressCompanyID, string WareHouse, string MerchantID, string distributionCode);

        DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel,ref PageInfo pi);

        DataTable GetAreaLevelIncomeExprotList(AreaLevelIncomeSearchModel searchModel);

        bool UpdateAreaLevelIncomeStatus(List<AreaExpressLevelIncome> model);

        void IncomeAreaLevelToEffect(int rowCount);
    }
}
