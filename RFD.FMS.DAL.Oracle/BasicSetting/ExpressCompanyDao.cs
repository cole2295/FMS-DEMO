using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util.Security;
using Oracle.ApplicationBlocks.Data;
using System.Collections.Generic;
using RFD.FMS.Util;
using RFD.FMS.Domain;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Model;


namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class ExpressCompanyDao : OracleDao, IExpressCompanyDao
    {
        /// <summary>
        /// 获取部门信息列表
        /// </summary>
        public DataSet GetExpressCompanyList(ExpressCompany model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"select ec.ExpressCompanyID  序号,ProvinceName 所在省份, CityName 所在城市,
                                                                   (select CompanyName from ExpressCompany ec1 where ec1.ExpressCompanyID=ec.ParentID) 上级部门 ,
                                                             ExpressCompanyCode 部门编码,CompanyName 部门名称,SimpleSpell 简拼,CompanyAllName 部门全称,      
                                                                   (select statusName from StatusInfo f where f.statustypeno=301 and f.statusno= ec.CompanyFlag) 部门类型,
                                                             (SELECT count(EmployeeID)   FROM Employee ee where ee.StationID=ec.ExpressCompanyID) as 人员配备,
                                                              MaxOrderLimit 订单峰值,
                                                              case IsReplacement when 0 then '不支持' when 1 then '支持' end 是否支持上门换货,
                                                              case IsPOS when 0 then '不支持' when 1 then '支持' end 是否支持POS机,
                                                              case IsPDA when 0 then '不支持' when 1 then '支持' end 是否支持PDA机,
                                                              MainContacter 主要联系人,Address 通讯地址,ContacterPhone 联系人电话,ec.Email 电子邮件,Deposit 加盟商押金,
                                                              case ec.IsDeleted  when 0 then '正常' when 1 then '已停用' end 停用标志 
                                        FROM ExpressCompany ec,Province p,City c Where ec.ProvinceID=p.ProvinceID
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
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
        }
        /// <summary>
        /// 获取部门信息列表(非全部)
        /// </summary>
        public DataSet GetExpressCompanyListLess(ExpressCompany model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"select ec.ExpressCompanyID  序号,
               (select CompanyName from ExpressCompany ec1 where ec1.ExpressCompanyID=ec.ParentID) 上级部门 ,ExpressCompanyCode 部门编码,CompanyName 部门名称,SimpleSpell 简拼,
               (select statusName from StatusInfo f where f.statustypeno=301 and f.statusno= ec.CompanyFlag) 部门类型,
               (SELECT count(EmployeeID) FROM Employee ee where ee.StationID=ec.ExpressCompanyID) as 人员配备,MaxOrderLimit 订单峰值,
                  case IsReplacement when 0 then '不支持' when 1 then '支持' end 是否支持上门换货,
                  case IsPOS when 0 then '不支持' when 1 then '支持' end 是否支持POS机,
                  case IsPDA when 0 then '不支持' when 1 then '支持' end 是否支持PDA机,
                  case ec.IsDeleted  when 0 then '正常' when 1 then '已停用' end 停用标志 
                FROM ExpressCompany ec,Province p,City c Where ec.ProvinceID=p.ProvinceID and ec.CityID=c.CityID");

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
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
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
                                            FROM     ExpressCompany
                                            WHERE    CompanyFlag =3
		                                            AND ParentID=0
		                                            AND DistributionCode <> :DistributionCode
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
                                            FROM     ExpressCompany
                                            WHERE    CompanyFlag =2
		                                            AND DistributionCode = :DistributionCode
		                                            AND IsDeleted = 0
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT CompanyName FROM ExpressCompany ec1
										  WHERE ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT statusName FROM StatusInfo f
										  WHERE f.statustypeno = 301 AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      Employee ee
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

            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = model.DistributionCode;
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
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
                                            FROM     ExpressCompany
                                            WHERE    CompanyFlag =2
		                                            AND DistributionCode = :DistributionCode
		                                            AND IsDeleted = 0
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT CompanyName FROM ExpressCompany ec1
										  WHERE ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT statusName FROM StatusInfo f
										  WHERE f.statustypeno = 301 AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      Employee ee
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

            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = model.DistributionCode;
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 根据简拼查询站点编号和名称
        /// </summary>
        /// <param name="SimpleSpell">简拼</param>
        /// <returns></returns>
        public DataTable GetStationListBySimpleSpell(string SimpleSpell)
        {
            string strSql = string.Format("select ExpressCompanyID,CompanyName from ExpressCompany where  SimpleSpell='{0}' or CompanyName='{0}'", SimpleSpell.Trim());
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];
        }

        public DataTable GetStationListByType(int companyType)
        {
            string strSql = string.Format("SELECT ExpressCompanyID,ExpressCompanyCode,CompanyName FROM ExpressCompany   WHERE IsDeleted=0 and companyflag={0} ", companyType);
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
        }

        public DataTable GetSortingList()
        {
            string strSql = string.Format("SELECT ExpressCompanyID,ExpressCompanyCode,CompanyName FROM ExpressCompany   WHERE IsDeleted=0 and companyflag=1 and DistributionCode = 'rfd' ");
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
        }

        public DataTable GetCompanyByUserId(int userId)
        {
            string sql = "select company.ExpressCompanyID as id,company.CompanyName as name from ExpressCompany company inner join Employee emp on company.ExpressCompanyID = emp.StationID where emp.EmployeeID = " + userId;

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0]; 
        }

        /// <summary>
        /// 获取区域站点组织结构
        /// </summary>
        /// <returns></returns>
        public DataTable GetAreaOrganizationCompany()
        {
            var strSql = @"SELECT dt.DistrictID,DistrictName,exp2.ExpressCompanyID as funDepartID,exp2.CompanyName as funDepartName,
		                    pv.ProvinceID,ProvinceName,ct.CityID,CityName,exp1.ExpressCompanyID,exp1.CompanyName
	                    FROM District dt 
		                JOIN Province pv ON dt.DistrictID = pv.DistrictID
							                 AND dt.IsDeleted = 0
							                 AND pv.IsDeleted = 0
		                JOIN City ct ON ct.ProvinceID = pv.ProvinceID
						                 AND ct.IsDeleted = 0
		                JOIN ExpressCompany exp1 ON ct.CityID = exp1.CityID
								                   AND exp1.IsDeleted = 0
								                   AND exp1.CompanyFlag != 0
                                                   AND exp1.DistributionCode = '{0}'
		                left Join ExpressCompany exp2 on exp2.DistrictID = dt.DistrictID
								                   AND exp2.CompanyFlag = 6
								                   AND exp2.IsDeleted = 0
                                                   AND exp2.DistributionCode = '{0}'
                        where exp1.CompanyFlag = 2 or exp1.CompanyFlag = 5
	                    ORDER BY dt.DistrictID,ct.ProvinceID,ct.CityID,ct.ExpressCompanyID";

            strSql = String.Format(strSql, AppConfigHelper.AppSettings("RfdCode"));

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];
        }

        /// <summary>
        /// 获取职能部门组织结构
        /// </summary>
        /// <returns></returns>
        public DataTable GetExpressCompanyByType(params int[] types)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(@"select ExpressCompanyID as id, CompanyName as name, ParentID as pid,CompanyFlag as flag from ExpressCompany");

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

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, builder.ToString()).Tables[0];
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
										   FROM     ExpressCompany
										   WHERE    CompanyFlag = 3
													AND ParentID=0
													AND DistributionCode <> :DistributionCode
													AND IsDeleted = 0
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT CompanyName FROM ExpressCompany ec1
										  WHERE     ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT statusName FROM StatusInfo f
										  WHERE f.statustypeno = 301 AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      Employee ee
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

            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = model.DistributionCode;

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
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
										   FROM     ExpressCompany
										   WHERE    CompanyFlag in (3)
													AND ParentID=0
													AND DistributionCode <> :DistributionCode
													AND IsDeleted = 0
													OR ExpressCompanyID=11
										 )
								SELECT  t.ExpressCompanyID 序号 ,
										( SELECT CompanyName
										  FROM      ExpressCompany ec1
										  WHERE     ec1.ExpressCompanyID = t.ParentID
										) 上级部门 ,
										ExpressCompanyCode 部门编码 ,
										CompanyName 部门名称 ,
										SimpleSpell 简拼 ,
										( SELECT statusName
										  FROM      StatusInfo f
										  WHERE     f.statustypeno = 301
													AND f.statusno = t.CompanyFlag
										) 部门类型 ,
										( SELECT    COUNT(EmployeeID)
										  FROM      Employee ee
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

            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = model.DistributionCode;

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql, parameters);
        }

        public DataTable GetCompanyIDvsName()
        {
            string sql = "select ExpressCompanyID as ID, CompanyName as Name from ExpressCompany";

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetCompanyByIds(string ids)
        {
            string sql = String.Format("select ExpressCompanyID as ID, CompanyName as Name,accountcompanyname from ExpressCompany where ExpressCompanyID in ({0})", ids);

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

		public DataTable GetExpressBySortCenterID(string sortCenterId)
		{
			string sql = @"SELECT ec.ExpressCompanyID FROM ExpressCompany ec
							WHERE ec.IsDeleted=0 AND ec.DistributionCode = 'rfd' AND ec.ParentID IN ({0})";
			sql = string.Format(sql, sortCenterId);
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
		}

        public DataSet GetThirdCompanyList(string distributionCode)
		{
			string sql = @"
			SELECT DISTINCT
        ec.TopCODCompanyID AS ExpressCompanyID,
         CASE 
                           WHEN NVL(ec2.AccountCompanyName, '') = '' THEN ec2.CompanyName
                           ELSE ec2.AccountCompanyName
                      END CompanyName,ec2.DistributionCode
 FROM   ExpressCompany ec
        JOIN ExpressCompany ec2
             ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
 WHERE  ec.DistributionCode <> :DistributionCode
        AND NVL(ec.TopCODCompanyID, 0) > 0
        AND ec.CompanyFlag=3
        AND ec.IsDeleted = 0
			";

            OracleParameter[] parameter ={
                                          new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                     };
            parameter[0].Value = distributionCode;
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,parameter);
		}

        public DataSet GetRFDSiteList(string distributionCode)
		{
			string sql = @"
	SELECT  ec.ExpressCompanyID ,
        ec.CompanyName
FROM    ExpressCompany ec
WHERE   ec.IsDeleted = 0
        AND ec.DistributionCode = :DistributionCode
        AND ec.CompanyFlag = 2
ORDER BY ec.CompanyName
";
            OracleParameter[] parameter ={
                                          new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                     };
            parameter[0].Value = distributionCode;
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,parameter);
		}

        public DataSet GetRFDSortCenterList(string distributionCode)
		{
			string sql = @"
	SELECT  ec.ExpressCompanyID ,
        ec.CompanyName
FROM    ExpressCompany ec
WHERE   ec.IsDeleted = 0
        AND ec.DistributionCode = :DistributionCode
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
            OracleParameter[] parameter ={
                                          new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                     };
            parameter[0].Value = distributionCode;
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,parameter);
		}

        public string GetDistributionNameByCode(string distributionCode)
        {
            string sql = string.Format(@"
	        SELECT d.DistributionName
            FROM   Distribution d
            WHERE  d.DistributionCode = '{0}' and RowNum=1", distributionCode);
            return Convert.ToString(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql));
        }

        #region IExpressCompanyDao 成员

        public DataTable GetCompanyAndStation(string companyIds)
        {
            string sql = String.Format("select ExpressCompanyID as Id from ExpressCompany a where a.IsDeleted=0 and a.CompanyFlag in(2,3) and a.DistributionCode in (select DistributionCode from ExpressCompany a where a.ExpressCompanyID in ({0}))", companyIds);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        #endregion
    

    /// <summary>
        /// 依据部门ID获取部门Model
        /// </summary>
        public ExpressCompany GetModel(int ExpressCompanyID)
        {
            var strSql = new StringBuilder();
            strSql.Append("select ExpressCompanyID,ExpressCompanyOldID,ExpressCompanyCode,CompanyName,CompanyAllName,SimpleSpell,ProvinceID,DistrictID,CityID,ParentID,CompanyFlag,MainContacter,LevelCode,MaxOrderLimit,IsReplacement,IsPOS,IsCod,Address,Email,ContacterPhone,Deposit,Sorting,IsDeleted,CreatBy,CreatTime,CreatStationID,UpdateBy,UpdateTime,UpdateStationID,DistributionCode,ISReturn,TopCODCompanyID from ExpressCompany ");
            strSql.Append(" where ExpressCompanyID=:ExpressCompanyID ");

            OracleParameter[] parameters = 
            {
                new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal, 4)
            };

            parameters[0].Value = ExpressCompanyID;

            
            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
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
                "select ExpressCompanyID,ExpressCompanyOldID,ExpressCompanyCode,CompanyName,CompanyAllName,SimpleSpell,ProvinceID,DistrictID,CityID,ParentID,CompanyFlag,MainContacter,LevelCode,MaxOrderLimit,IsReplacement,IsPOS,IsCod,Address,Email,ContacterPhone,Deposit,Sorting,IsDeleted,CreatBy,CreatTime,CreatStationID,UpdateBy,UpdateTime,UpdateStationID,DistributionCode,ISReturn,TopCODCompanyID from ExpressCompany   ");
            strSql.Append(" where ParentID=0 and CompanyFlag=3 AND DistributionCode=:DistributionCode AND RowNum=1");
            OracleParameter[] parameters = {
                                            new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100)
                                        };
            parameters[0].Value = DistributionCode;

            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
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
                @"select ec2.ExpressCompanyID,ec2.ExpressCompanyOldID,ec2.ExpressCompanyCode,ec2.CompanyName,ec2.CompanyAllName,
                    ec2.SimpleSpell,ec2.ProvinceID,ec2.DistrictID,ec2.CityID,ec2.ParentID,ec2.CompanyFlag,
                    ec2.MainContacter,ec2.LevelCode,ec2.MaxOrderLimit,ec2.IsReplacement,ec2.IsPOS,ec2.IsCod,ec2.Address,
                    ec2.Email,ec2.ContacterPhone,ec2.Deposit,ec2.Sorting,ec2.IsDeleted,ec2.CreatBy,ec2.CreatTime,ec2.CreatStationID,
                    ec2.UpdateBy,ec2.UpdateTime,ec2.UpdateStationID,ec2.DistributionCode,ec2.ISReturn ,ec.TopCODCompanyID
                    from ExpressCompany ec  
                        join ExpressCompany ec1  on ec.ParentID=ec1.ExpressCompanyID
                        join ExpressCompany ec2  on ec1.ParentID=ec2.ExpressCompanyID
                    ");

            strSql.Append(" where ec.ExpressCompanyID=:ExpressCompanyID and RowNum=1");
            OracleParameter[] parameters = {
                                            new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal, 4)
                                        };
            parameters[0].Value = ExpressCompanyID;


            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
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
                @"select ec1.ExpressCompanyID,ec1.ExpressCompanyOldID,ec1.ExpressCompanyCode,ec1.CompanyName,ec1.CompanyAllName,
                         ec1.SimpleSpell,ec1.ProvinceID,ec1.DistrictID,ec1.CityID,ec1.ParentID,ec1.CompanyFlag,
                         ec1.MainContacter,ec1.LevelCode,ec1.MaxOrderLimit,ec1.IsReplacement,ec1.IsPOS,ec1.IsCod,ec1.Address,
                         ec1.Email,ec1.ContacterPhone,ec1.Deposit,ec1.Sorting,ec1.IsDeleted,ec1.CreatBy,ec1.CreatTime,ec1.CreatStationID,
                         ec1.UpdateBy,ec1.UpdateTime,ec1.UpdateStationID,ec1.DistributionCode,ec1.ISReturn ,ec.TopCODCompanyID
                         from ExpressCompany ec  
                             join ExpressCompany ec1  on ec.ParentID=ec1.ExpressCompanyID
                    ");

            strSql.Append(" where ec.ExpressCompanyID=:ExpressCompanyID and RowNum=1");
            OracleParameter[] parameters = {
                                            new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal, 4)
                                        };
            parameters[0].Value = ExpressCompanyID;


            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
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
FROM    ExpressCompany ec
		JOIN ExpressCompany ec1 ON ec.ParentID=ec1.ExpressCompanyID
		JOIN ExpressCompany ec2 ON ec1.ParentID=ec2.ExpressCompanyID
WHERE   ec.IsDeleted = 0
        AND ec.DistributionCode = :DistributionCode
        AND ec.CompanyFlag = 2
ORDER BY ec.CompanyName";

            OracleParameter[] parameter ={
                                          new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                     };
            parameter[0].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameter).Tables[0];
        }

        private ExpressCompany TransforToModel(DataRow dr)
        {
            var model = new ExpressCompany();
            if (dr["ExpressCompanyID"].ToString() != "")
            {
                model.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
            }
            if (dr["ExpressCompanyOldID"].ToString() != "")
            {
                model.ExpressCompanyOldID = int.Parse(dr["ExpressCompanyOldID"].ToString());
            }
            model.ExpressCompanyCode = dr["ExpressCompanyCode"].ToString();
            model.CompanyName = dr["CompanyName"].ToString();
            model.CompanyAllName = dr["CompanyAllName"].ToString();
            model.SimpleSpell = dr["SimpleSpell"].ToString();
            model.DistrictId = dr["DistrictID"].ToString();
            model.ProvinceID = dr["ProvinceID"].ToString();
            model.CityID = dr["CityID"].ToString();
            if (dr["ParentID"].ToString() != "")
            {
                model.ParentID = int.Parse(dr["ParentID"].ToString());
            }
            if (dr["CompanyFlag"].ToString() != "")
            {
                model.CompanyFlag = int.Parse(dr["CompanyFlag"].ToString());
            }
            model.MainContacter = dr["MainContacter"].ToString();
            model.LevelCode = dr["LevelCode"].ToString();
            if (dr["MaxOrderLimit"].ToString() != "")
            {
                model.MaxOrderLimit = int.Parse(dr["MaxOrderLimit"].ToString());
            }
            if (dr["IsReplacement"].ToString() != "")
            {
                if ((dr["IsReplacement"].ToString() == "1") ||
                    (dr["IsReplacement"].ToString().ToLower() == "true"))
                {
                    model.IsReplacement = true;
                }
                else
                {
                    model.IsReplacement = false;
                }
            }
            if (dr["ISReturn"].ToString() != "")
            {
                if ((dr["ISReturn"].ToString() == "1") ||
                    (dr["ISReturn"].ToString().ToLower() == "true"))
                {
                    model.IsReturn = true;
                }
                else
                {
                    model.IsReturn = false;
                }
            }
            if (dr["IsPOS"].ToString() != "")
            {
                if ((dr["IsPOS"].ToString() == "1") ||
                    (dr["IsPOS"].ToString().ToLower() == "true"))
                {
                    model.IsPOS = true;
                }
                else
                {
                    model.IsPOS = false;
                }
            }
            if (dr["IsCod"].ToString() != "")
            {
                if ((dr["IsCod"].ToString() == "1") ||
                    (dr["IsCod"].ToString().ToLower() == "true"))
                {
                    model.IsCod = true;
                }
                else
                {
                    model.IsCod = false;
                }
            }
            model.Address = dr["Address"].ToString();
            model.Email = dr["Email"].ToString();
            model.ContacterPhone = dr["ContacterPhone"].ToString();
            if (dr["Deposit"].ToString() != "")
            {
                model.Deposit = decimal.Parse(dr["Deposit"].ToString());
            }
            if (dr["Sorting"].ToString() != "")
            {
                model.Sorting = int.Parse(dr["Sorting"].ToString());
            }
            if (dr["IsDeleted"].ToString() != "")
            {
                if ((dr["IsDeleted"].ToString() == "1") ||
                    (dr["IsDeleted"].ToString().ToLower() == "true"))
                {
                    model.IsDeleted = true;
                }
                else
                {
                    model.IsDeleted = false;
                }
            }
            if (dr["CreatBy"].ToString() != "")
            {
                model.CreatBy = int.Parse(dr["CreatBy"].ToString());
            }
            if (dr["CreatTime"].ToString() != "")
            {
                model.CreatTime = DateTime.Parse(dr["CreatTime"].ToString());
            }
            if (dr["CreatStationID"].ToString() != "")
            {
                model.CreatStationID = int.Parse(dr["CreatStationID"].ToString());
            }
            if (dr["UpdateBy"].ToString() != "")
            {
                model.UpdateBy = int.Parse(dr["UpdateBy"].ToString());
            }
            if (dr["UpdateTime"].ToString() != "")
            {
                model.UpdateTime = DateTime.Parse(dr["UpdateTime"].ToString());
            }
            if (dr["UpdateStationID"].ToString() != "")
            {
                model.UpdateStationID = int.Parse(dr["UpdateStationID"].ToString());
            }
            if (dr["TopCODCompanyID"].ToString() != "")
            {
                model.TopCODCompanyID = int.Parse(dr["TopCODCompanyID"].ToString());
            }

            model.DistributionCode = dr["DistributionCode"].ToString();

            return model;
        }

        public DataTable GetDistribution(string distributionCode)
        {
            string sql = @"SELECT  ec.ExpressCompanyID ,
									ec.CompanyName ,
									ec.DistributionCode,
									ec.ParentID,
									ec.CompanyFlag
							FROM    RFD_PMS.dbo.ExpressCompany ec
							WHERE   ec.DistributionCode=:DistributionCode
									AND ec.IsDeleted = 0";
            OracleParameter[] parameters ={
										   new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100),
									  };
            parameters[0].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public int ExecSql(string sql)
        {
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql);
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

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }
    }
}
