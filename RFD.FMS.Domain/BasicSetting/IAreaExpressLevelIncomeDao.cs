using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IAreaExpressLevelIncomeDao
    {
        //查询区域类型
        DataTable SearchAreaMerchantLevel(int status, string areaid, string cityid, string provinceid, int merchantid, int areatype, int expresscompanyid,string distributionCode, ref PageInfo pi);

        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
		DataTable SearchArea(string provinceId, string cityId, string areaId, string merchantId, string distributionCode);



        DataTable SearchAreaMerchantLevelDetail(string areaid, int status, string distributionCode);

        /// <summary>
        /// 区域详细信息1
        /// </summary>
        /// <param name="areaid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        DataTable SearchAreaMerchantLevelDetail(string areaid, int status, int merchantId, int areatype, int expresscompanyid, string distributionCode);

        /// <summary>
        /// 查询区域类型
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        DataTable SearchAreaType(string areaId, string distributionCode);

        /// <summary>
        /// 根据唯一ID，主键查询
        /// </summary>
        /// <param name="autoId"></param>
        /// <returns></returns>
        DataTable SearchAreaTypeByAutoId(string autoId);

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="areaExpressLevelIncome"></param>
        /// <returns></returns>
        bool AddAreaType(ref AreaExpressLevelIncome areaExpressLevelIncome);

        /// <summary>
        /// 更新区域
        /// </summary>
        /// <param name="areaExpressLevelIncome"></param>
        /// <returns></returns>
        bool UpdateAreaTypeV2(AreaExpressLevelIncome areaExpressLevelIncome);
        /// <summary>
        /// 更新配送商
        /// </summary>
        /// <param name="areaExpressLevelIncome"></param>
        /// <returns></returns>
        bool UpdateExpressV2(AreaExpressLevelIncome areaExpressLevelIncome);

        /// <summary>
        /// 更新区域
        /// </summary>
        /// <param name="areaExpressLevelIncome"></param>
        /// <returns></returns>
        bool UpdateAreaType(AreaExpressLevelIncome areaExpressLevelIncome, out int autoId);

        /// <summary>
        /// 删除区域类型
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        bool DeleteAreaType(string autoId, int updateBy);

        //添加日志
        bool AddAreaExpLevelIncomeLog(AreaExpressLevelIncomeLog areaExpressLevelIncomeLog);


        //设置审批
        bool SetAreaMerchantLeverAudit(int autoid, DateTime doDate, int auditBy, DateTime auditTime);

        //设置审批new
        bool SetAreaMerchantLeverAuditEx(int autoid, DateTime doDate, int auditBy, int auditstatus, DateTime auditTime);

        //返回待生效的区域
        DataTable AreaMerchantLevelNum(int num, DateTime nowDate);

        //更新区域类型
        bool AreaMerchantLevelUpdate(int autoid);

        //添加区域类型
        bool AreaMerchantLevelAdd(int autoid);

        //删除区域类型
        bool AreaMerchantLevelDel(int autoid);


        //设置置回
        bool ReSetAreaMerchantLevel(int autoid);


        //获取导入基础数据
        DataSet GetExportData();

        //获取分拣中心
        DataTable GetSortingCenter();

        DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode, PageInfo pi);

        DataTable SearchAreaTypeExprotList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode);
        /// <summary>
        /// 查询操作日志
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string merchantId, string distributionCode);

        int GetAreaLevelIncomeListStat(AreaLevelIncomeSearchModel searchModel);

        DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel,PageInfo pi);

        DataTable GetAreaLevelIncomeExprotList(AreaLevelIncomeSearchModel searchModel);

        bool UpdateAreaLevelIncomeStatus(AreaExpressLevelIncome model);

        DataTable GetWaitEffectList();

        int GetAreaTypeByCondition(AreaLevelIncomeSearchModel searchModel);
        /// <summary>
        /// 根据id获取区域配送商类型设置信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>返回结果</returns>
        DataTable GetAreaExpressByID(int id);
        /// <summary>
        /// 根据查询条件查询是否存在设置
        /// </summary>
        /// <param name="areaExpressLevelIncome">查询条件对象</param>
        /// <returns>true 不存在，false存在</returns>
        bool ExistAreaExpress( AreaExpressLevelIncome areaExpressLevelIncome);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel);
    }
}
