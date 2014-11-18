using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Model;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IExpressCompanyDao
    {
        System.Data.DataTable GetAreaOrganizationCompany();
        System.Data.DataTable GetCompanyAndStation(string companyIds);
        System.Data.DataTable GetCompanyByIds(string ids);
        System.Data.DataTable GetCompanyByUserId(int userId);
        System.Data.DataTable GetCompanyIDvsName();
        ExpressCompany GetCompanyModelByDistributionCode(string DistributionCode);
        System.Data.DataSet GetCompanySiteList(ExpressCompany model);
        System.Data.DataTable GetDistribution(string distributionCode);
        string GetDistributionNameByCode(string distributionCode);
        System.Data.DataTable GetExpressBySortCenterID(string sortCenterId);
        System.Data.DataTable GetExpressCompanyByType(params int[] types);
        System.Data.DataSet GetExpressCompanyList(ExpressCompany model);
        System.Data.DataSet GetExpressCompanyListLess(ExpressCompany model);
        ExpressCompany GetFirstLevelSortCenter(int ExpressCompanyID);
        DataTable GetFirstLevelSortCenterDt(string distributionCode);
        ExpressCompany GetCitySortCenterByStationID(int ExpressCompanyID);
        ExpressCompany GetModel(int ExpressCompanyID);
        System.Data.DataSet GetRFDSiteList(string distributionCode);
        System.Data.DataSet GetRFDSortCenterList(string distributionCode);
        System.Data.DataSet GetSiteExpressCompanyList(ExpressCompany model);
        System.Data.DataTable GetSortingList();
        System.Data.DataTable GetStationListBySimpleSpell(string SimpleSpell);
        System.Data.DataTable GetStationListByType(int companyType);
        System.Data.DataSet GetThirdCompanyList(string distributionCode);
        System.Data.DataSet GetThirdExpressCompanyList(ExpressCompany model);
        System.Data.DataSet GetThirdRFDList(ExpressCompany model);

        int ExecSql(string sql);

        DataTable GetExpressAndSiteById(string ids);
    }
}
