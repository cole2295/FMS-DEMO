using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Model;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class ExpressCompanyService : IExpressCompanyService
    {
        private IExpressCompanyDao expressCompanyDao;

        private const int CompanyFlag = 2;

        public string GetCompanyAndStation(string companyIds)
        {
            DataTable table = expressCompanyDao.GetCompanyAndStation(companyIds);

            DataRow row = null;

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                builder.Append(DataConvert.ToString(row["id"]));

                if (i != table.Rows.Count - 1)
                {
                    builder.Append(",");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 依据部门ID获取部门Model
        /// </summary>
        public ExpressCompany GetModel(int ExpressCompanyID)
        {
            return expressCompanyDao.GetModel(ExpressCompanyID);
        }

        /// <summary>
        /// 查询配送公司
        /// </summary>
        /// <param name="DistributionCode"></param>
        /// <returns></returns>
        public ExpressCompany GetCompanyModelByDistributionCode(string DistributionCode)
        {
            return expressCompanyDao.GetCompanyModelByDistributionCode(DistributionCode);
        }

        /// <summary>
        /// 根据ID查询名称，多个按，分割
        /// </summary>
        /// <param name="expressCompanyIds"></param>
        /// <returns></returns>
        public string SearchMergeCompanyName(string expressCompanyIds)
        {
            if (string.IsNullOrEmpty(expressCompanyIds)) return null;
            DataTable dtExpress = expressCompanyDao.GetCompanyByIds(expressCompanyIds);
            if (dtExpress == null || dtExpress.Rows.Count <= 0)
            {
                return null;
            }
            StringBuilder sbNames = new StringBuilder();
            foreach (DataRow dr in dtExpress.Rows)
            {
                sbNames.Append(dr["accountcompanyname"].ToString() + ",");
            }
            return sbNames.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 根据ID，多个按，分割
        /// </summary>
        /// <param name="expressCompanyIds"></param>
        /// <returns></returns>
        public string SearchMergeCompanyIds(string expressCompanyIds)
        {
            if (string.IsNullOrEmpty(expressCompanyIds)) return null;
            DataTable dtExpress = expressCompanyDao.GetExpressAndSiteById(expressCompanyIds);
            if (dtExpress == null || dtExpress.Rows.Count <= 0)
            {
                return null;
            }
            StringBuilder sbIds = new StringBuilder();
            foreach (DataRow dr in dtExpress.Rows)
            {
                sbIds.Append(dr["ExpressCompanyID"].ToString() + ",");
            }
            return sbIds.ToString().TrimEnd(',');
        }

        public DataSet GetThirdCompanyList(string distributionCode)
        {
            return expressCompanyDao.GetThirdCompanyList(distributionCode);
        }

        public DataSet GetRFDSiteList(string distributionCode)
        {
            return expressCompanyDao.GetRFDSiteList(distributionCode);
        }

        public DataSet GetRFDSortCenterList(string distributionCode)
        {
            return expressCompanyDao.GetRFDSortCenterList(distributionCode);
        }

        public ExpressCompany GetFirstLevelSortCenter(int ExpressCompanyID)
        {
            return expressCompanyDao.GetFirstLevelSortCenter(ExpressCompanyID);
        }

        public DataTable GetStationList()
        {
            return expressCompanyDao.GetStationListByType(CompanyFlag);
        }

        public DataTable GetSortingList()
        {
            return expressCompanyDao.GetSortingList();
        }

        public DataTable GetDistribution(string distributionCode)
        {
            return expressCompanyDao.GetDistribution(distributionCode);
        }

        public DataTable GetExpressBySortCenterID(string sortCenterId)
        {
            return expressCompanyDao.GetExpressBySortCenterID(sortCenterId);
        }

        public string GetDistributionNameByCode(string distributionCode)
        {
            return expressCompanyDao.GetDistributionNameByCode(distributionCode);
        }

        public DataTable GetStationListBySimpleSpell(string SimpleSpell)
        {
            return expressCompanyDao.GetStationListBySimpleSpell(SimpleSpell);
        }

        public DataSet GetExpressCompanyListLess(ExpressCompany model)
        {
            return expressCompanyDao.GetExpressCompanyListLess(model);
        }

        public DataSet GetSiteExpressCompanyList(ExpressCompany model)
        {
            return expressCompanyDao.GetSiteExpressCompanyList(model);
        }

        public DataSet GetThirdExpressCompanyList(ExpressCompany model)
        {
            return expressCompanyDao.GetThirdExpressCompanyList(model);
        }

        public DataSet GetThirdRFDList(ExpressCompany model)
        {
            return expressCompanyDao.GetThirdRFDList(model);
        }

        public DataSet GetCompanySiteList(ExpressCompany model)
        {
            return expressCompanyDao.GetCompanySiteList(model);
        }

        public DataTable GetCompanyByUserId(int userId)
        {
            return expressCompanyDao.GetCompanyByUserId(userId);
        }

        public DataTable GetAreaOrganizationCompany()
        {
            return expressCompanyDao.GetAreaOrganizationCompany();
        }

        public DataTable GetExpressCompanyByType(params int[] types)
        {
            return expressCompanyDao.GetExpressCompanyByType(types);
        }

        public DataTable GetFirstLevelSortCenterDt(string distributionCode)
        {
            return expressCompanyDao.GetFirstLevelSortCenterDt(distributionCode);
        }

        public int ExecSql(string sql)
        {
            return expressCompanyDao.ExecSql(sql);
        }

        /// <summary>
        /// 根据配送商ID，查询配送商ID和站点ID
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DataTable GetExpressAndSiteById(string ids)
        {
            return expressCompanyDao.GetExpressAndSiteById(ids);
        }
    }
}
