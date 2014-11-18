using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util.Security;
using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using RFD.FMS.Util;
using RFD.FMS.Domain;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Model;


namespace RFD.FMS.DAL.BasicSetting
{
    public class ExpressCompanyDao : SqlServerDao, IExpressCompanyDao
    {
        /// <summary>
        /// 获取部门信息列表
        /// </summary>
        public DataSet GetExpressCompanyList(ExpressCompany model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"select ec.ExpressCompanyID  序号,ProvinceName 所在省份, CityName 所在城市,
                                                                   (select top 1 CompanyName from RFD_PMS.dbo.ExpressCompany ec1(nolock) where ec1.ExpressCompanyID=ec.  ParentID) 上级部门 ,
                                                             ExpressCompanyCode 部门编码,CompanyName 部门名称,SimpleSpell 简拼,CompanyAllName 部门全称,      
                                                                   (select top 1 statusName from RFD_PMS.dbo.StatusInfo f(nolock) where f.statustypeno=301 and f.statusno= ec.CompanyFlag) 部门类型,
                                                             (SELECT count(EmployeeID)   FROM RFD_PMS.dbo.Employee ee where ee.StationID=ec.ExpressCompanyID) as 人员配备,
                                                              MaxOrderLimit 订单峰值,
                                                              case IsReplacement when 0 then '不支持' when 1 then '支持' end 是否支持上门换货,
                                                              case IsPOS when 0 then '不支持' when 1 then '支持' end 是否支持POS机,
                                                              case IsPDA when 0 then '不支持' when 1 then '支持' end 是否支持PDA机,
                                                              MainContacter 主要联系人,Address 通讯地址,ContacterPhone 联系人电话,ec.Email 电子邮件,Deposit 加盟商押金,
                                                              case ec.IsDeleted  when 0 then '正常' when 1 then '已停用' end 停用标志 
                                        FROM RFD_PMS.dbo.ExpressCompany ec(nolock),RFD_PMS.dbo.Province p(nolock),RFD_PMS.dbo.City c(nolock) Where ec.ProvinceID=p.ProvinceID
                                        and ec.CityID=c.CityID");
            if (model.ExpressCompanyID != 0)
            {
                strSql.Append(string.Format(" And ExpressCompanyID={0} ", model.ExpressCompanyID));
            }
            if (!string.IsNullOrEmpty(model.ProvinceID))
            {
                strSql.Append(string.Format(" And ec.ProvinceID='{0}' ", model.ProvinceID));
            }
            if (!string.IsNullOrEmpty(model.CityID))
            {
                strSql.Append(string.Format(" And ec.CityID='{0}' ", model.CityID));
            }
            if (model.CompanyFlag != 999999)
            {
                strSql.Append(string.Format(" And CompanyFlag={0} ", model.CompanyFlag));
            }
            if (!string.IsNullOrEmpty(model.CompanyName))
            {
                strSql.Append(string.Format(" And (CompanyName like '%{0}%'  or SimpleSpell like  '%{0}%')", model.CompanyName));
            }
            if (!string.IsNullOrEmpty(model.ExpressCompanyCode))
            {
                strSql.Append(string.Format(" And ExpressCompanyCode='{0}' ", model.ExpressCompanyCode));
            }
            strSql.Append(model.IsDeleted ? " And ec.IsDeleted=1 " : " And  ec.IsDeleted=0 ");
            if (!string.IsNullOrEmpty(model.SortStr))
            {
                strSql.Append(model.SortStr);
            }
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
        }
        /// <summary>
        /// 获取部门信息列表(非全部)
        /// </summary>
        public DataSet GetExpressCompanyListLess(ExpressCompany model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"select ec.ExpressCompanyID  序号,
               (select top 1 CompanyName from RFD_PMS.dbo.ExpressCompany ec1(nolock) where ec1.ExpressCompanyID=ec.ParentID) 上级部门 ,ExpressCompanyCode 部门编码,CompanyName 部门名称,SimpleSpell 简拼,
               (select top 1 statusName from RFD_PMS.dbo.StatusInfo f(nolock) where f.statustypeno=301 and f.statusno= ec.CompanyFlag) 部门类型,
               (SELECT count(EmployeeID) FROM RFD_PMS.dbo.Employee ee where ee.StationID=ec.ExpressCompanyID) as 人员配备,MaxOrderLimit 订单峰值,
                  case IsReplacement when 0 then '不支持' when 1 then '支持' end 是否支持上门换货,
                  case IsPOS when 0 then '不支持' when 1 then '支持' end 是否支持POS机,
                  case IsPDA when 0 then '不支持' when 1 then '支持' end 是否支持PDA机,
                  case ec.IsDeleted  when 0 then '正常' when 1 then '已停用' end 停用标志 
                FROM RFD_PMS.dbo.ExpressCompany ec(nolock),RFD_PMS.dbo.Province p(nolock),RFD_PMS.dbo.City c(nolock) Where ec.ProvinceID=p.ProvinceID and ec.CityID=c.CityID");

            strSql.Append(string.Format(" And ec.DistributionCode='{0}'", model.DistributionCode));

            if (model.ExpressCompanyID != 0)
            {
                strSql.Append(string.Format(" And ExpressCompanyID={0} ", model.ExpressCompanyID));
            }
            if (!string.IsNullOrEmpty(model.ProvinceID))
            {
                strSql.Append(string.Format(" And ec.ProvinceID='{0}' ", model.ProvinceID));
            }
            if (!string.IsNullOrEmpty(model.CityID))
            {
                strSql.Append(string.Format(" And ec.CityID='{0}' ", model.CityID));
            }
            if (model.CompanyFlag != 999999)
            {
                strSql.Append(string.Format(" And CompanyFlag={0} ", model.CompanyFlag));
            }
            if (!string.IsNullOrEmpty(model.CompanyName))
            {
                strSql.Append(string.Format(" And (CompanyName like '%{0}%'  or SimpleSpell like  '%{0}%')", model.CompanyName));
            }
            if (!string.IsNullOrEmpty(model.ExpressCompanyCode))
            {
                strSql.Append(string.Format(" And ExpressCompanyCode='{0}' ", model.ExpressCompanyCode));
            }
            strSql.Append(model.IsDeleted ? " And ec.IsDeleted=1 " : " And  ec.IsDeleted=0 ");
            if (!string.IsNullOrEmpty(model.SortStr))
            {
                strSql.Append(model.SortStr);
            }
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
        }

		public DataSet GetSiteExpressCompanyList(ExpressCompany model)
		{
            string sql = @"WITH    t AS ( SELECT   ExpressCompanyID ,
		                                            ParentID ,
		                                            CompanyFlag ,
		                                            ExpressCompanyCode ,
		                                            CompanyName ,
		                                            SimpleSpell ,
		                                            MaxOrderLimit ,
		                                            IsReplacement ,
		                                            IsPOS ,
		                                            IsPDA ,
		                                            IsDeleted
                                            FROM     RFD_PMS.dbo.ExpressCompany(NOLOCK)
                                            WHERE    CompanyFlag =3
		                                            AND ParentID=0
		                                            AND DistributionCode <> @DistributionCode
		                                            AND IsDeleted = 0
		                                            UNION ALL
                                            SELECT   ExpressCompanyID ,
		                                            ParentID ,
		                                            CompanyFlag ,
		                                            ExpressCompanyCode ,
		                                            CompanyName ,
		                                            SimpleSpell ,
		                                            MaxOrderLimit ,
		                                            IsReplacement ,
		                                            IsPOS ,
		                                            IsPDA ,
		                                            IsDeleted
                                            FROM     RFD_PMS.dbo.ExpressCompany(NOLOCK)
                                            WHERE    CompanyFlag =2
		                                            AND DistributionCode = @DistributionCode
		                                            AND IsDeleted = 0
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT TOP 1
													CompanyName
										  FROM      RFD_PMS.dbo.ExpressCompany ec1 ( NOLOCK )
										  WHERE     ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT TOP 1
													statusName
										  FROM      RFD_PMS.dbo.StatusInfo f ( NOLOCK )
										  WHERE     f.statustypeno = 301
													AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      RFD_PMS.dbo.Employee ee
										  WHERE     ee.StationID = t.ExpressCompanyID
										) AS 人员配备 ,
										MaxOrderLimit 订单峰值 ,
										CASE IsReplacement
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持上门换货 ,
										CASE IsPOS
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持POS机 ,
										CASE IsPDA
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持PDA机 ,
										CASE t.IsDeleted
										  WHEN 0 THEN '正常'
										  WHEN 1 THEN '已停用'
										END 停用标志
								FROM    t where (t.IsDeleted=0) ";

			if (!string.IsNullOrEmpty(model.CompanyName))
			{
				sql += string.Format(" And (CompanyName like '%{0}%'  or SimpleSpell like  '%{0}%')", model.CompanyName);
			}
			if (!string.IsNullOrEmpty(model.ExpressCompanyCode))
			{
				sql += string.Format(" And ExpressCompanyCode='{0}' ", model.ExpressCompanyCode);
			}
			if (!string.IsNullOrEmpty(model.SortStr))
			{
				sql += model.SortStr;
			}

            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = model.DistributionCode;
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
		}

        public DataSet GetCompanySiteList(ExpressCompany model)
        {
            string sql = @"WITH    t AS ( SELECT   ExpressCompanyID ,
		                                            ParentID ,
		                                            CompanyFlag ,
		                                            ExpressCompanyCode ,
		                                            CompanyName ,
		                                            SimpleSpell ,
		                                            MaxOrderLimit ,
		                                            IsReplacement ,
		                                            IsPOS ,
		                                            IsPDA ,
		                                            IsDeleted
                                            FROM     RFD_PMS.dbo.ExpressCompany(NOLOCK)
                                            WHERE    CompanyFlag =2
		                                            AND DistributionCode = @DistributionCode
		                                            AND IsDeleted = 0
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT TOP 1
													CompanyName
										  FROM      RFD_PMS.dbo.ExpressCompany ec1 ( NOLOCK )
										  WHERE     ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT TOP 1
													statusName
										  FROM      RFD_PMS.dbo.StatusInfo f ( NOLOCK )
										  WHERE     f.statustypeno = 301
													AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      RFD_PMS.dbo.Employee ee
										  WHERE     ee.StationID = t.ExpressCompanyID
										) AS 人员配备 ,
										MaxOrderLimit 订单峰值 ,
										CASE IsReplacement
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持上门换货 ,
										CASE IsPOS
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持POS机 ,
										CASE IsPDA
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持PDA机 ,
										CASE t.IsDeleted
										  WHEN 0 THEN '正常'
										  WHEN 1 THEN '已停用'
										END 停用标志
								FROM    t where (t.IsDeleted=0) ";

            if (!string.IsNullOrEmpty(model.CompanyName))
            {
                sql += string.Format(" And (CompanyName like '%{0}%'  or SimpleSpell like  '%{0}%')", model.CompanyName);
            }
            if (!string.IsNullOrEmpty(model.ExpressCompanyCode))
            {
                sql += string.Format(" And ExpressCompanyCode='{0}' ", model.ExpressCompanyCode);
            }
            if (!string.IsNullOrEmpty(model.SortStr))
            {
                sql += model.SortStr;
            }

            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = model.DistributionCode;
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 根据简拼查询站点编号和名称
        /// </summary>
        /// <param name="SimpleSpell">简拼</param>
        /// <returns></returns>
        public DataTable GetStationListBySimpleSpell(string SimpleSpell)
        {
            string strSql = string.Format("select ExpressCompanyID,CompanyName from RFD_PMS.dbo.ExpressCompany(nolock) where  SimpleSpell='{0}' or CompanyName='{0}'", SimpleSpell.Trim());
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];
        }

        public DataTable GetStationListByType(int companyType)
        {
            string strSql = string.Format("SELECT ExpressCompanyID,ExpressCompanyCode,CompanyName FROM RFD_PMS.dbo.ExpressCompany (nolock)  WHERE IsDeleted=0 and companyflag={0} ", companyType);
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
        }

        public DataTable GetSortingList()
        {
            string strSql = string.Format("SELECT ExpressCompanyID,ExpressCompanyCode,CompanyName FROM RFD_PMS.dbo.ExpressCompany (nolock)  WHERE IsDeleted=0 and companyflag=1 and DistributionCode = 'rfd' ");
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
        }

        public DataTable GetCompanyByUserId(int userId)
        {
            string sql = "select company.ExpressCompanyID as id,company.CompanyName as name from RFD_PMS.dbo.ExpressCompany company(NOLOCK) inner join RFD_PMS.dbo.Employee emp(NOLOCK) on company.ExpressCompanyID = emp.StationID where emp.EmployeeID = " + userId;

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0]; 
        }

        /// <summary>
        /// 获取区域站点组织结构
        /// </summary>
        /// <returns></returns>
        public DataTable GetAreaOrganizationCompany()
        {
            var strSql = @"SELECT dt.DistrictID,DistrictName,exp2.ExpressCompanyID as funDepartID,exp2.CompanyName as funDepartName,
		                    pv.ProvinceID,ProvinceName,ct.CityID,CityName,exp1.ExpressCompanyID,exp1.CompanyName
	                    FROM RFD_PMS.dbo.District dt (NOLOCK)
		                JOIN RFD_PMS.dbo.Province pv(NOLOCK) ON dt.DistrictID = pv.DistrictID
							                 AND dt.IsDeleted = 0
							                 AND pv.IsDeleted = 0
		                JOIN RFD_PMS.dbo.City ct(NOLOCK) ON ct.ProvinceID = pv.ProvinceID
						                 AND ct.IsDeleted = 0
		                JOIN RFD_PMS.dbo.ExpressCompany(NOLOCK) exp1 ON ct.CityID = exp1.CityID
								                   AND exp1.IsDeleted = 0
								                   AND exp1.CompanyFlag != 0
                                                   AND exp1.DistributionCode = '{0}'
		                left Join RFD_PMS.dbo.ExpressCompany exp2(NOLOCK) on exp2.DistrictID = dt.DistrictID
								                   AND exp2.CompanyFlag = 6
								                   AND exp2.IsDeleted = 0
                                                   AND exp2.DistributionCode = '{0}'
                        where exp1.CompanyFlag = 2 or exp1.CompanyFlag = 5
	                    ORDER BY dt.DistrictID,ct.ProvinceID,ct.CityID,ct.ExpressCompanyID";

            strSql = String.Format(strSql, AppConfigHelper.AppSettings("RfdCode"));

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];
        }

        /// <summary>
        /// 获取职能部门组织结构
        /// </summary>
        /// <returns></returns>
        public DataTable GetExpressCompanyByType(params int[] types)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(@"select ExpressCompanyID as id, CompanyName as name, ParentID as pid,CompanyFlag as flag from RFD_PMS.dbo.ExpressCompany(NOLOCK)");

            if (types.Length != 0)
            {
                builder.Append(" where CompanyFlag in ");
                builder.Append("(");

                for (int i = 0; i < types.Length; i++)
                {
                    builder.Append(types[i]);

                    if (i != (types.Length - 1))
                    {
                        builder.Append(",");
                    }
                }

                builder.Append(")");
            }

            builder.Append(" and IsDeleted = 0 order by name ");

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, builder.ToString()).Tables[0];
        }

		public DataSet GetThirdExpressCompanyList(ExpressCompany model)
		{
			string sql = @"WITH    t AS ( SELECT   ExpressCompanyID ,
													ParentID ,
													CompanyFlag ,
													ExpressCompanyCode ,
													CompanyName ,
													SimpleSpell ,
													MaxOrderLimit ,
													IsReplacement ,
													IsPOS ,
													IsPDA ,
													IsDeleted
										   FROM     RFD_PMS.dbo.ExpressCompany(NOLOCK)
										   WHERE    CompanyFlag = 3
													AND ParentID=0
													AND DistributionCode <> @DistributionCode
													AND IsDeleted = 0
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT TOP 1
													CompanyName
										  FROM      RFD_PMS.dbo.ExpressCompany ec1 ( NOLOCK )
										  WHERE     ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT TOP 1
													statusName
										  FROM      RFD_PMS.dbo.StatusInfo f ( NOLOCK )
										  WHERE     f.statustypeno = 301
													AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      RFD_PMS.dbo.Employee ee
										  WHERE     ee.StationID = t.ExpressCompanyID
										) AS 人员配备 ,
										MaxOrderLimit 订单峰值 ,
										CASE IsReplacement
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持上门换货 ,
										CASE IsPOS
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持POS机 ,
										CASE IsPDA
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持PDA机 ,
										CASE t.IsDeleted
										  WHEN 0 THEN '正常'
										  WHEN 1 THEN '已停用'
										END 停用标志
								FROM    t where (t.IsDeleted=0) ";

			if (!string.IsNullOrEmpty(model.CompanyName))
			{
				sql += string.Format(" And (CompanyName like '%{0}%'  or SimpleSpell like  '%{0}%')", model.CompanyName);
			}

			if (!string.IsNullOrEmpty(model.ExpressCompanyCode))
			{
				sql += string.Format(" And ExpressCompanyCode='{0}' ", model.ExpressCompanyCode);
			}

			if (!string.IsNullOrEmpty(model.SortStr))
			{
				sql += model.SortStr;
			}

            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = model.DistributionCode;

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
		}

        public DataSet GetThirdRFDList(ExpressCompany model)
        {
            string sql = @"WITH    t AS ( SELECT   ExpressCompanyID ,
													ParentID ,
													CompanyFlag ,
													ExpressCompanyCode ,
													CompanyName ,
													SimpleSpell ,
													MaxOrderLimit ,
													IsReplacement ,
													IsPOS ,
													IsPDA ,
													IsDeleted
										   FROM     RFD_PMS.dbo.ExpressCompany(NOLOCK)
										   WHERE    CompanyFlag in (3)
													AND ParentID=0
													AND DistributionCode <> @DistributionCode
													AND IsDeleted = 0
													OR ExpressCompanyID=11
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT TOP 1
													CompanyName
										  FROM      RFD_PMS.dbo.ExpressCompany ec1 ( NOLOCK )
										  WHERE     ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT TOP 1
													statusName
										  FROM      RFD_PMS.dbo.StatusInfo f ( NOLOCK )
										  WHERE     f.statustypeno = 301
													AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      RFD_PMS.dbo.Employee ee
										  WHERE     ee.StationID = t.ExpressCompanyID
										) AS 人员配备 ,
										MaxOrderLimit 订单峰值 ,
										CASE IsReplacement
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持上门换货 ,
										CASE IsPOS
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持POS机 ,
										CASE IsPDA
										  WHEN 0 THEN '不支持'
										  WHEN 1 THEN '支持'
										END 是否支持PDA机 ,
										CASE t.IsDeleted
										  WHEN 0 THEN '正常'
										  WHEN 1 THEN '已停用'
										END 停用标志
								FROM    t where (t.IsDeleted=0) ";

            if (!string.IsNullOrEmpty(model.CompanyName))
            {
                sql += string.Format(" And (CompanyName like '%{0}%'  or SimpleSpell like  '%{0}%')", model.CompanyName);
            }
            if (!string.IsNullOrEmpty(model.ExpressCompanyCode))
            {
                sql += string.Format(" And ExpressCompanyCode='{0}' ", model.ExpressCompanyCode);
            }
            if (!string.IsNullOrEmpty(model.SortStr))
            {
                sql += model.SortStr;
            }

            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = model.DistributionCode;

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
        }

        public DataTable GetCompanyIDvsName()
        {
            string sql = "select ExpressCompanyID as ID, CompanyName as Name from RFD_PMS.dbo.ExpressCompany";

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetCompanyByIds(string ids)
        {
            string sql = String.Format("select ExpressCompanyID as ID, CompanyName as Name,accountcompanyname from RFD_PMS.dbo.ExpressCompany where ExpressCompanyID in ({0})", ids);

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

		public DataTable GetExpressBySortCenterID(string sortCenterId)
		{
			string sql = @"SELECT ec.ExpressCompanyID FROM RFD_PMS.dbo.ExpressCompany AS ec(NOLOCK)
							WHERE ec.IsDeleted=0 AND ec.DistributionCode = 'rfd' AND ec.ParentID IN ({0})";
			sql = string.Format(sql, sortCenterId);
			return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
		}

        public DataSet GetThirdCompanyList(string distributionCode)
		{
			string sql = @"
			SELECT DISTINCT
        ec.TopCODCompanyID AS ExpressCompanyID,
        CompanyName = CASE 
                           WHEN ISNULL(ec2.AccountCompanyName, '') = '' THEN ec2.CompanyName
                           ELSE ec2.AccountCompanyName
                      END
 FROM   RFD_PMS.dbo.ExpressCompany AS ec(NOLOCK)
        JOIN RFD_PMS.dbo.ExpressCompany AS ec2(NOLOCK)
             ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
 WHERE  ec.DistributionCode <> @DistributionCode
        AND ISNULL(ec.TopCODCompanyID, 0) > 0
        AND ec.CompanyFlag=3
        AND ec.IsDeleted = 0
			";

            SqlParameter[] parameter ={
                                          new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                     };
            parameter[0].Value = distributionCode;
			return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,parameter);
		}

        public DataSet GetRFDSiteList(string distributionCode)
		{
			string sql = @"
	SELECT  ec.ExpressCompanyID ,
        ec.CompanyName
FROM    RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK )
WHERE   ec.IsDeleted = 0
        AND ec.DistributionCode = @DistributionCode
        AND ec.CompanyFlag = 2
ORDER BY ec.CompanyName
";
            SqlParameter[] parameter ={
                                          new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                     };
            parameter[0].Value = distributionCode;
			return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,parameter);
		}

        public DataSet GetRFDSortCenterList(string distributionCode)
		{
			string sql = @"
	SELECT  ec.ExpressCompanyID ,
        ec.CompanyName
FROM    RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK )
WHERE   ec.IsDeleted = 0
        AND ec.DistributionCode = @DistributionCode
        AND ec.CompanyFlag = 1
		{0}
ORDER BY ec.CompanyName
";

            if (distributionCode == "rfd")
            {
                sql = string.Format(sql, " AND ec.ParentID<>11 ");
            }
            else
            {
                sql = string.Format(sql, "");
            }
            SqlParameter[] parameter ={
                                          new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                     };
            parameter[0].Value = distributionCode;
			return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,parameter);
		}

        public string GetDistributionNameByCode(string distributionCode)
        {
            string sql = string.Format(@"
	        SELECT TOP 1 d.DistributionName
            FROM   RFD_PMS.dbo.Distribution d
            WHERE  d.DistributionCode = '{0}'", distributionCode);
            return Convert.ToString(SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql));
        }

        #region IExpressCompanyDao 成员

        public DataTable GetCompanyAndStation(string companyIds)
        {
            string sql = String.Format("select ExpressCompanyID as Id from RFD_PMS.dbo.ExpressCompany a where a.IsDeleted=0 and a.CompanyFlag in(2,3) and a.DistributionCode in (select DistributionCode from RFD_PMS.dbo.ExpressCompany a where a.ExpressCompanyID in ({0}))", companyIds);

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        #endregion
    

    /// <summary>
        /// 依据部门ID获取部门Model
        /// </summary>
        public ExpressCompany GetModel(int ExpressCompanyID)
        {
            string sql = @"select 
                top 1 ExpressCompanyID,ExpressCompanyOldID,ExpressCompanyCode,
                      CompanyName,CompanyAllName,SimpleSpell,ProvinceID,
                      DistrictID,CityID,ParentID,CompanyFlag,MainContacter,
                      LevelCode,MaxOrderLimit,IsReplacement,IsPOS,
                      IsCod,Address,Email,ContacterPhone,Deposit,Sorting,
                      IsDeleted,CreatBy,CreatTime,CreatStationID,UpdateBy,
                      UpdateTime,UpdateStationID,DistributionCode,ISReturn,
                      TopCODCompanyID 
                from rfd_pms.dbo.ExpressCompany(nolock) 
                where ExpressCompanyID=@ExpressCompanyID";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value=ExpressCompanyID }
            };
            
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return TransforToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查询配送公司
        /// </summary>
        /// <param name="DistributionCode"></param>
        /// <returns></returns>
        public ExpressCompany GetCompanyModelByDistributionCode(string DistributionCode)
        {
            var strSql = new StringBuilder();
            strSql.Append(
                "select  top 1 ExpressCompanyID,ExpressCompanyOldID,ExpressCompanyCode,CompanyName,CompanyAllName,SimpleSpell,ProvinceID,DistrictID,CityID,ParentID,CompanyFlag,MainContacter,LevelCode,MaxOrderLimit,IsReplacement,IsPOS,IsCod,Address,Email,ContacterPhone,Deposit,Sorting,IsDeleted,CreatBy,CreatTime,CreatStationID,UpdateBy,UpdateTime,UpdateStationID,DistributionCode,ISReturn,TopCODCompanyID from rfd_pms.dbo.ExpressCompany  (nolock) ");
            //,ISReturn

            strSql.Append(" where ParentID=0 and CompanyFlag=3 AND DistributionCode=@DistributionCode ");
            SqlParameter[] parameters = {
                                            new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50)
                                        };
            parameters[0].Value = DistributionCode;

            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return TransforToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据站点ID获取分拣中心一级名称
        /// </summary>
        /// <param name="ExpressCompanyID"></param>
        /// <returns></returns>
        public ExpressCompany GetFirstLevelSortCenter(int ExpressCompanyID)
        {
            var strSql = new StringBuilder();
            strSql.Append(
                @"select  top 1 ec2.ExpressCompanyID,ec2.ExpressCompanyOldID,ec2.ExpressCompanyCode,ec2.CompanyName,ec2.CompanyAllName,
                    ec2.SimpleSpell,ec2.ProvinceID,ec2.DistrictID,ec2.CityID,ec2.ParentID,ec2.CompanyFlag,
                    ec2.MainContacter,ec2.LevelCode,ec2.MaxOrderLimit,ec2.IsReplacement,ec2.IsPOS,ec2.IsCod,ec2.Address,
                    ec2.Email,ec2.ContacterPhone,ec2.Deposit,ec2.Sorting,ec2.IsDeleted,ec2.CreatBy,ec2.CreatTime,ec2.CreatStationID,
                    ec2.UpdateBy,ec2.UpdateTime,ec2.UpdateStationID,ec2.DistributionCode,ec2.ISReturn,ec.TopCODCompanyID
                    from rfd_pms.dbo.ExpressCompany ec (nolock) 
                        join rfd_pms.dbo.ExpressCompany ec1 (nolock) on ec.ParentID=ec1.ExpressCompanyID
                        join rfd_pms.dbo.ExpressCompany ec2 (nolock) on ec1.ParentID=ec2.ExpressCompanyID
                    ");

            strSql.Append(" where ec.ExpressCompanyID=@ExpressCompanyID ");
            SqlParameter[] parameters = {
                                            new SqlParameter("@ExpressCompanyID", SqlDbType.Int, 4)
                                        };
            parameters[0].Value = ExpressCompanyID;


            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return TransforToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据站点ID获取城市分拣中心名称
        /// </summary>
        /// <param name="ExpressCompanyID"></param>
        /// <returns></returns>
        public ExpressCompany GetCitySortCenterByStationID(int ExpressCompanyID)
        {
            var strSql = new StringBuilder();
            strSql.Append(
                @"select  top 1 ec1.ExpressCompanyID,ec1.ExpressCompanyOldID,ec1.ExpressCompanyCode,ec1.CompanyName,ec1.CompanyAllName,
                    ec1.SimpleSpell,ec1.ProvinceID,ec1.DistrictID,ec1.CityID,ec1.ParentID,ec1.CompanyFlag,
                    ec1.MainContacter,ec1.LevelCode,ec1.MaxOrderLimit,ec1.IsReplacement,ec1.IsPOS,ec1.IsCod,ec1.Address,
                    ec1.Email,ec1.ContacterPhone,ec1.Deposit,ec1.Sorting,ec1.IsDeleted,ec1.CreatBy,ec1.CreatTime,ec1.CreatStationID,
                    ec1.UpdateBy,ec1.UpdateTime,ec1.UpdateStationID,ec1.DistributionCode,ec1.ISReturn,ec.TopCODCompanyID
                    from rfd_pms.dbo.ExpressCompany ec (nolock)
                        join rfd_pms.dbo.ExpressCompany ec1 (nolock) on ec.ParentID=ec1.ExpressCompanyID
                    ");

            strSql.Append(" where ec.ExpressCompanyID=@ExpressCompanyID ");
            SqlParameter[] parameters = {
                                            new SqlParameter("@ExpressCompanyID", SqlDbType.Int, 4)
                                        };
            parameters[0].Value = ExpressCompanyID;


            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return TransforToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public DataTable GetFirstLevelSortCenterDt(string distributionCode)
        {
            string sql = @"SELECT  ec.ExpressCompanyID ,
        ec.CompanyName,
        ec2.CompanyName AS SortName
FROM    RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK )
		JOIN RFD_PMS.dbo.ExpressCompany AS ec1 ( NOLOCK ) ON ec.ParentID=ec1.ExpressCompanyID
		JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec1.ParentID=ec2.ExpressCompanyID
WHERE   ec.IsDeleted = 0
        AND ec.DistributionCode = @DistributionCode
        AND ec.CompanyFlag = 2
ORDER BY ec.CompanyName";

            SqlParameter[] parameter ={
                                          new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                     };
            parameter[0].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameter).Tables[0];
        }

        private ExpressCompany TransforToModel(DataRow dr)
        {
            var model = new ExpressCompany();

            model.ExpressCompanyID = DataConvert.ToInt(dr["ExpressCompanyID"]);
            model.ExpressCompanyOldID = DataConvert.ToInt(dr["ExpressCompanyOldID"]);
            model.ExpressCompanyCode = DataConvert.ToString(dr["ExpressCompanyCode"]);
            model.CompanyName = DataConvert.ToString(dr["CompanyName"]);
            model.CompanyAllName = DataConvert.ToString(dr["CompanyAllName"]);
            model.SimpleSpell = DataConvert.ToString(dr["SimpleSpell"]);
            model.DistrictId = DataConvert.ToString(dr["DistrictID"]);
            model.ProvinceID = DataConvert.ToString(dr["ProvinceID"]);
            model.CityID = DataConvert.ToString(dr["CityID"]);
            model.ParentID = DataConvert.ToInt(dr["ParentID"]);
            model.CompanyFlag = DataConvert.ToInt(dr["CompanyFlag"]);
            model.MainContacter = DataConvert.ToString(dr["MainContacter"]);
            model.LevelCode = DataConvert.ToString(dr["LevelCode"]);
            model.MaxOrderLimit = DataConvert.ToInt(dr["MaxOrderLimit"]);
            model.IsReplacement = DataConvert.ToBoolean(dr["IsReplacement"]);
            model.IsReturn = DataConvert.ToBoolean(dr["ISReturn"]);
            model.IsPOS = DataConvert.ToBoolean(dr["IsPOS"]);
            model.IsCod = DataConvert.ToBoolean(dr["IsCod"]);
            model.Address = DataConvert.ToString(dr["Address"]);
            model.Email = DataConvert.ToString(dr["Email"]);
            model.ContacterPhone = DataConvert.ToString(dr["ContacterPhone"]);
            model.Deposit = DataConvert.ToDecimal(dr["Deposit"]);
            model.Sorting = DataConvert.ToInt(dr["Sorting"]);
            model.IsDeleted = DataConvert.ToBoolean(dr["IsDeleted"]);;
            model.CreatBy = DataConvert.ToInt(dr["CreatBy"]);
            model.CreatTime = DataConvert.ToDateTime(dr["CreatTime"]);
            model.CreatStationID = DataConvert.ToInt(dr["CreatStationID"]);
            model.UpdateBy = DataConvert.ToInt(dr["UpdateBy"]);
            model.UpdateTime = DataConvert.ToDateTime(dr["UpdateTime"]);
            model.UpdateStationID = DataConvert.ToInt(dr["UpdateStationID"]);
            model.DistributionCode = DataConvert.ToString(dr["DistributionCode"]);
            model.TopCODCompanyID = DataConvert.ToInt(dr["TopCODCompanyID"]);

            return model;
        }

        public DataTable GetDistribution(string distributionCode)
        {
            string sql = @"SELECT  ec.ExpressCompanyID ,
									ec.CompanyName ,
									ec.DistributionCode,
									ec.ParentID,
									ec.CompanyFlag
							FROM    RFD_PMS.dbo.ExpressCompany AS ec
							WHERE   ec.DistributionCode=@DistributionCode
									AND ec.IsDeleted = 0";
            SqlParameter[] parameters ={
										   new SqlParameter("@DistributionCode",SqlDbType.NVarChar),
									  };
            parameters[0].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public int ExecSql(string sql)
        {
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql);
        }

        /// <summary>
        /// 根据配送商ID，查询配送商ID和站点ID
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DataTable GetExpressAndSiteById(string ids)
        {
            if (string.IsNullOrEmpty(ids)) return null;
            string sql = string.Format(@"SELECT ExpressCompanyID,CompanyName FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) 
WHERE DistributionCode IN (
SELECT DistributionCode FROM RFD_PMS.dbo.ExpressCompany ec1(NOLOCK) WHERE ec1.ExpressCompanyID IN ({0}))
AND ec.CompanyFlag IN (2,3) AND ec.IsDeleted=0", ids);

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }
    }
}
