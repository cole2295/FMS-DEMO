using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Model;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IExpressCompanyService
    {
        string GetCompanyAndStation(string companyIds);

        /// <summary>
        /// 依据部门ID获取部门Model
        /// </summary>
        ExpressCompany GetModel(int ExpressCompanyID);

        /// <summary>
        /// 查询配送公司
        /// </summary>
        /// <param name="DistributionCode"></param>
        /// <returns></returns>
        ExpressCompany GetCompanyModelByDistributionCode(string DistributionCode);

        string SearchMergeCompanyName(string expressCompanyIds);

        string SearchMergeCompanyIds(string expressCompanyIds);

        DataSet GetThirdCompanyList(string distributionCode);

        DataSet GetRFDSiteList(string distributionCode);

        DataSet GetRFDSortCenterList(string distributionCode);

        ExpressCompany GetFirstLevelSortCenter(int ExpressCompanyID);

        DataTable GetStationList();

        DataTable GetSortingList();

        DataTable GetDistribution(string distributionCode);

        DataTable GetExpressBySortCenterID(string sortCenterId);

        string GetDistributionNameByCode(string distributionCode);

        DataTable GetStationListBySimpleSpell(string SimpleSpell);

        DataSet GetExpressCompanyListLess(ExpressCompany model);

        DataSet GetSiteExpressCompanyList(ExpressCompany model);

        DataSet GetThirdExpressCompanyList(ExpressCompany model);

        DataSet GetThirdRFDList(ExpressCompany model);

        DataSet GetCompanySiteList(ExpressCompany model);

        DataTable GetCompanyByUserId(int userId);

        DataTable GetAreaOrganizationCompany();

        DataTable GetExpressCompanyByType(params int[] types);

        DataTable GetFirstLevelSortCenterDt(string distributionCode);

        int ExecSql(string sql);

        DataTable GetExpressAndSiteById(string ids);
    }
}
