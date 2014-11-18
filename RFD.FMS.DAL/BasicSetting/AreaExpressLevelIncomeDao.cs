using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.BasicSetting
{
    public class AreaExpressLevelIncomeDao : SqlServerDao, IAreaExpressLevelIncomeDao
    {
        private string sqlStr = "";

		public DataTable SearchArea(string provinceId, string cityId, string areaId, string merchantId, string distributionCode)
        {
            sqlStr = @"SELECT distinct p.ProvinceID,
							  p.ProvinceName,
							  c.CityID,
							  c.CityName,
							  a.AreaID,
							  a.AreaName,
								IsAreaType = CASE WHEN ( aeli.AreaType IS NOT NULL
														 OR aeli.EffectAreaType IS NOT NULL
													   )
													   AND Enable IN ( 1,2, 3 ) THEN '√'
												  ELSE ''
											 END
					   FROM   rfd_pms.dbo.Province p(NOLOCK)
							  JOIN rfd_pms.dbo.City c(NOLOCK)
								   ON  c.ProvinceID = p.ProvinceID
							  JOIN rfd_pms.dbo.Area a(NOLOCK)
								   ON  a.CityID = c.CityID
							  LEFT JOIN AreaExpressLevelIncome aeli(NOLOCK)
								   ON  aeli.AreaID = a.AreaID AND aeli.DistributionCode=@DistributionCode
					   WHERE  p.IsDeleted = 0
							  AND c.IsDeleted = 0
							  AND a.IsDeleted = 0
							  {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(provinceId))
            {
                sbWhere.Append(" AND p.ProvinceID=@ProvinceID ");
                parameters.Add(new SqlParameter("@ProvinceID", SqlDbType.NVarChar,10) { Value = provinceId });
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                sbWhere.Append(" AND c.CityID=@CityID ");
                parameters.Add(new SqlParameter("@CityID", SqlDbType.NVarChar, 10) { Value = cityId });
            }

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=@AreaID ");
                parameters.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar, 50) { Value = areaId });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aeli.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(merchantId) && int.Parse(merchantId) > 0)
            {
                sbWhere.Append(" AND aeli.[Enable] in (1,2,3) ");
            }

            parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50) { Value = distributionCode });

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable SearchAreaType(string areaId, string distributionCode)
        {
            sqlStr = @"SELECT aeli.AutoID,
							   aeli.AreaID,
							   aeli.MerchantID,
							   mbi.MerchantName,
							   aeli.WareHouseID,
							   ec.CompanyName,
							   ec1.CompanyName AS ExpressCompanyName,
							   AreaType=case when aeli.[Enable]=3 then null else aeli.AreaType end,
							   EffectAreaType=case when AuditStatus in (0,1,3) THEN aeli.EffectAreaType ELSE NULL end,
							   AuditStatusStr=case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已同步' when 3 then '置回' else null end,
							   EnableStr=CASE aeli.[Enable] WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END
						FROM   AreaExpressLevelIncome aeli(NOLOCK)
							   LEFT JOIN rfd_pms.dbo.ExpressCompany ec(NOLOCK)
									ON  ec.ExpressCompanyID = aeli.WareHouseID
							   JOIN rfd_pms.dbo.MerchantBaseInfo mbi(NOLOCK) 
									ON mbi.ID = aeli.MerchantID
							   JOIN rfd_pms.dbo.ExpressCompany ec1(NOLOCK)
									ON aeli.ExpressCompanyID=ec1.ExpressCompanyID
						WHERE aeli.[Enable] in( 1,2,3) {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND aeli.AreaID=@AreaID ");
                parameters.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar, 10) { Value = areaId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aeli.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable SearchAreaTypeByAutoId(string autoId)
        {
            sqlStr = @"SELECT   aeli.AutoID,
								aeli.AreaID,
								aeli.ExpressCompanyID,
							    aeli.MerchantID,
							    aeli.WareHouseID,
								aeli.Enable,
							    aeli.AreaType,
								aeli.EffectAreaType,
								aeli.DoDate,
								aeli.CreateBy,
								aeli.CreateTime,
								aeli.UpdateBy,
								aeli.UpdateTime,
								aeli.AuditBy,
								aeli.AuditStatus,
                                aeli.DistributionCode
						FROM   AreaExpressLevelIncome aeli(NOLOCK)
						WHERE AutoID=@AutoID";
            SqlParameter[] parameters ={
										   new SqlParameter("@AutoID",SqlDbType.Int),
									  };
            parameters[0].Value = autoId;
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, parameters).Tables[0];
        }

        public bool AddAreaType(ref AreaExpressLevelIncome areaExpressLevelIncome)
        {
            string judgeSql = string.Empty;
            if (string.IsNullOrEmpty(areaExpressLevelIncome.GoodsCategoryCode))
            {
                judgeSql = @"
                            SELECT 1
							  FROM   AreaExpressLevelIncome(NOLOCK)
							  WHERE  MerchantID = @MerchantID
									 AND ExpressCompanyID=@ExpressCompanyID
									 AND AreaID = @AreaID
									 AND ISNULL(WareHouseID,'')=@WareHouseID
									 AND [Enable] in (1,2,3)
                                     AND DistributionCode=@DistributionCode
                            ";
            }
            else
            {
                judgeSql = @"
                            SELECT 1
							  FROM   AreaExpressLevelIncome(NOLOCK)
							  WHERE  MerchantID = @MerchantID
									 AND ExpressCompanyID=@ExpressCompanyID
									 AND AreaID = @AreaID
									 AND ISNULL(WareHouseID,'')=@WareHouseID
									 AND [Enable] in (1,2,3)
                                     AND DistributionCode=@DistributionCode
                                     AND GoodsCategoryCode=@GoodsCategoryCode
                            ";
            }

            sqlStr = @"	SET @Flag=-1
						IF NOT EXISTS(
							  {0}
						  )
					   BEGIN
							INSERT INTO AreaExpressLevelIncome
								   (AreaID
								   ,MerchantID
								   ,WareHouseID
								   ,AreaType
								   ,Enable
								   ,EffectAreaType
								   ,DoDate
								   ,CreateBy
								   ,CreateTime
								   ,UpdateBy
								   ,UpdateTime
									,AuditStatus
									,AuditBy
									,AuditTime
									,ExpressCompanyID
                                    ,DistributionCode
                                    ,IsChange
                                    ,GoodsCategoryCode
									)
							 VALUES
								   (@AreaID
								   ,@MerchantID
								   ,@WareHouseID
								   ,@AreaType
								   ,@Enable
								   ,@EffectAreaType
								   ,@DoDate
								   ,@CreateBy
								   ,GETDATE()
								   ,@UpdateBy
								   ,GETDATE()
									,@AuditStatus
									,@AuditBy
									,GETDATE()
									,@ExpressCompanyID
                                    ,@DistributionCode
                                    ,@IsChange
                                    ,@GoodsCategoryCode
									)
							SET @Flag=@@IDENTITY
						END
						ELSE
						BEGIN
							SET @Flag=0
						END
						";

            sqlStr = string.Format(sqlStr, judgeSql);
            SqlParameter[] parameters ={
										   new SqlParameter("@AreaID",SqlDbType.NVarChar,100),
										   new SqlParameter("@MerchantID",SqlDbType.Int),
										   new SqlParameter("@WareHouseID",SqlDbType.NVarChar,40),
										   new SqlParameter("@AreaType",SqlDbType.Int),
										   new SqlParameter("@Enable",SqlDbType.TinyInt),
										   new SqlParameter("@EffectAreaType",SqlDbType.Int),
										   new SqlParameter("@DoDate",SqlDbType.DateTime),
										   new SqlParameter("@CreateBy",SqlDbType.NVarChar,100),
										   new SqlParameter("@UpdateBy",SqlDbType.NVarChar,100),
										   new SqlParameter("@AuditStatus",SqlDbType.Int),
										   new SqlParameter("@AuditBy",SqlDbType.NVarChar,100),
										   new SqlParameter("@Flag",SqlDbType.Int),
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                           new SqlParameter("@IsChange",SqlDbType.Bit),
                                           new SqlParameter("@GoodsCategoryCode",SqlDbType.NVarChar,10),
									  };
            parameters[0].Value = areaExpressLevelIncome.AreaID;
            parameters[1].Value = areaExpressLevelIncome.MerchantID;
            parameters[2].Value = areaExpressLevelIncome.WareHouseID;
            parameters[3].Value = areaExpressLevelIncome.AreaType;
            parameters[4].Value = areaExpressLevelIncome.Enable;
            parameters[5].Value = areaExpressLevelIncome.EffectAreaType;
            parameters[6].Value = areaExpressLevelIncome.DoDate;
            parameters[7].Value = areaExpressLevelIncome.CreateBy;
            parameters[8].Value = areaExpressLevelIncome.UpdateBy;
            parameters[9].Value = areaExpressLevelIncome.AuditStatus;
            parameters[10].Value = areaExpressLevelIncome.AuditBy;
            parameters[11].Direction = ParameterDirection.Output;
            parameters[12].Value = areaExpressLevelIncome.ExpressCompanyID;
            parameters[13].Value = areaExpressLevelIncome.DistributionCode;
            parameters[14].Value = true;
            parameters[15].Value = areaExpressLevelIncome.GoodsCategoryCode;
            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            areaExpressLevelIncome.AutoId = (int)parameters[11].Value;
            return (int)parameters[11].Value > 0 ? true : false;
        }

        public bool UpdateExpressV2(AreaExpressLevelIncome areaExpressLevelIncome)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAreaType(AreaExpressLevelIncome areaExpressLevelIncome, out int autoId)
        {
            sqlStr = @"	SET @Flag=-1
						IF EXISTS(
							  SELECT 1
							  FROM   AreaExpressLevelIncome(NOLOCK)
							  WHERE  ExpressCompanyID=@ExpressCompanyID
									 AND MerchantID = @MerchantID
									 AND AreaID = @AreaID
									 AND ISNULL(WareHouseID,'')=@WareHouseID
									 AND [Enable] in (1,2,3)
                                     AND DistributionCode=@DistributionCode
						  )
					   BEGIN
							UPDATE AreaExpressLevelIncome
							SET    EffectAreaType = @EffectAreaType,
								   UpdateBy = @UpdateBy,
								   UpdateTime = GETDATE(),
								   AuditStatus = @AuditStatus,
                                   IsChange=@IsChange
							WHERE  AreaID = @AreaID
								   AND ExpressCompanyID=@ExpressCompanyID
								   AND MerchantID = @MerchantID
								   AND [Enable] in (1,2,3)
                                   AND DistributionCode=@DistributionCode
							SET @Flag=@@ROWCOUNT
							SELECT @AutoID=AutoID FROM AreaExpressLevelIncome(NOLOCK) WHERE ExpressCompanyID=@ExpressCompanyID AND 
										AreaID = @AreaID AND MerchantID = @MerchantID AND [Enable] in (1,2,3) AND DistributionCode=@DistributionCode
						END
						ELSE
						BEGIN
							SET @Flag=0
						END
						";
            SqlParameter[] parameters ={
										   new SqlParameter("@AreaID",SqlDbType.NVarChar,100),
										   new SqlParameter("@MerchantID",SqlDbType.Int),
										   new SqlParameter("@WareHouseID",SqlDbType.NVarChar,40),
										   new SqlParameter("@EffectAreaType",SqlDbType.Int),
										   new SqlParameter("@UpdateBy",SqlDbType.NVarChar,100),
										   new SqlParameter("@AuditStatus",SqlDbType.Int),
										   new SqlParameter("@Flag",SqlDbType.Int),
										   new SqlParameter("@AutoID",SqlDbType.Int),
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
            parameters[0].Value = areaExpressLevelIncome.AreaID;
            parameters[1].Value = areaExpressLevelIncome.MerchantID;
            parameters[2].Value = areaExpressLevelIncome.WareHouseID;
            parameters[3].Value = areaExpressLevelIncome.EffectAreaType;
            parameters[4].Value = areaExpressLevelIncome.UpdateBy;
            parameters[5].Value = areaExpressLevelIncome.AuditStatus;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[7].Direction = ParameterDirection.Output;
            parameters[8].Value = areaExpressLevelIncome.ExpressCompanyID;
            parameters[9].Value = areaExpressLevelIncome.DistributionCode;
            parameters[10].Value = true;

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);

            autoId = string.IsNullOrEmpty(parameters[7].Value.ToString()) ? 0 : (int)parameters[7].Value;

            return (int)parameters[6].Value > 0 ? true : false;
        }

        public bool UpdateAreaTypeV2(AreaExpressLevelIncome areaExpressLevelIncome)
        {
            sqlStr = @"	UPDATE AreaExpressLevelIncome
							SET    EffectAreaType = @EffectAreaType,
								   UpdateBy = @UpdateBy,
								   UpdateTime = getdate(),
								   AuditStatus = @AuditStatus,
                                   IsChange=@IsChange
							WHERE  AutoID = @AutoID";
            SqlParameter[] parameters =
            {
                new SqlParameter("@EffectAreaType",SqlDbType.Int),
                new SqlParameter("@UpdateBy",SqlDbType.Int),
                new SqlParameter("@AuditStatus",SqlDbType.Int),
                new SqlParameter("@AutoID",SqlDbType.Int),
                new SqlParameter("@IsChange",SqlDbType.Bit),
            };
            parameters[0].Value = areaExpressLevelIncome.EffectAreaType;
            parameters[1].Value = areaExpressLevelIncome.UpdateBy;
            parameters[2].Value = areaExpressLevelIncome.AuditStatus;
            parameters[3].Value = areaExpressLevelIncome.AutoId;
            parameters[4].Value = true;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        public bool DeleteAreaType(string autoId, int updateBy)
        {
            sqlStr = @"UPDATE AreaExpressLevelIncome SET [Enable]=2,AuditStatus=0,UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange WHERE AutoID=@AutoID";
            SqlParameter[] parameters ={
										   new SqlParameter("@AutoID",SqlDbType.Int),
										   new SqlParameter("@UpdateBy",SqlDbType.NVarChar,100),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									   };
            parameters[0].Value = autoId;
            parameters[1].Value = updateBy;
            parameters[2].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        //新加日志
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool AddAreaExpLevelIncomeLog(AreaExpressLevelIncomeLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into AreaExpressLevelIncomeLog(");
            strSql.Append("AreaID,MerchantID,WarehouseId,AreaType,LogText,Enable,CreateBy,CreateTime,ExpressCompanyID,DistributionCode,IsChange)");
            strSql.Append(" values (");
            strSql.Append("@AreaID,@MerchantID,@WarehouseId,@AreaType,@LogText,@Enable,@CreateBy,GETDATE(),@ExpressCompanyID,@DistributionCode,@IsChange)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = 
            {
				new SqlParameter("@AreaID", SqlDbType.NVarChar,100),
				new SqlParameter("@MerchantID", SqlDbType.Int,4),
				new SqlParameter("@WarehouseId", SqlDbType.NVarChar,40),
				new SqlParameter("@AreaType", SqlDbType.Int,4),
				new SqlParameter("@LogText", SqlDbType.NVarChar,250),
				new SqlParameter("@Enable", SqlDbType.TinyInt,1),
				new SqlParameter("@CreateBy", SqlDbType.NVarChar,100),
				new SqlParameter("@ExpressCompanyID", SqlDbType.Int),
                new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50),
                new SqlParameter("@IsChange", SqlDbType.Bit)
			};
            parameters[0].Value = model.AreaID;
            parameters[1].Value = model.MerchantID;
            parameters[2].Value = model.WarehouseId;
            parameters[3].Value = model.AreaType;
            parameters[4].Value = model.LogText;
            parameters[5].Value = model.Enable;
            parameters[6].Value = model.CreateBy;
            parameters[7].Value = model.ExpressCompanyID;
            parameters[8].Value = model.DistributionCode;
            parameters[9].Value = true;

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rowCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //查询区域类型
        public DataTable SearchAreaMerchantLevel(int status, string areaid, string cityid, string provinceid, int merchantid, int areatype, string distributionCode, ref PageInfo pi)
        {
            string strResult = @"select p.provincename,cityname,areaname,a1.areaid,a1.dodate,e.employeename,a1.AuditTime,s.statusname,a1.auditstatus
                                                from (select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael(nolock) where ael.auditStatus=@auditStatus and DistributionCode=@DistributionCode
                                                group by ael.areaid ) as a
                                                join AreaExpressLevelIncome a1(nolock) on a1.autoid=a.id
                                                join rfd_pms.dbo.Area ar(nolock) on a.areaid=ar.areaid
                                                join rfd_pms.dbo.City c(nolock) on ar.cityid=c.cityid
                                                join rfd_pms.dbo.Province p(nolock)  on c.provinceid=p.ProvinceID 
                                                join rfd_pms.dbo.StatusInfo s(nolock) on a1.auditstatus=s.statusno and s.statustypeno='306'
                                                left join rfd_pms.dbo.employee e(nolock) on a1.auditby=e.employeeid
                                                 where 1=1 ";


            StringBuilder count = new StringBuilder();
            count.AppendFormat(
                                @"select count(1) from (select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael(nolock) where ael.auditStatus=@auditStatus and DistributionCode=@DistributionCode
                                group by ael.areaid ) as a
                                join AreaExpressLevelIncome a1(nolock) on a1.autoid=a.id
                                join rfd_pms.dbo.Area ar(nolock) on a.areaid=ar.areaid
                                join rfd_pms.dbo.City c(nolock) on ar.cityid=c.cityid
                                join rfd_pms.dbo.Province p(nolock)  on c.provinceid=p.ProvinceID 
                                join rfd_pms.dbo.StatusInfo s(nolock) on a1.auditstatus=s.statusno and s.statustypeno='306'
                                left join rfd_pms.dbo.employee e(nolock) on a1.auditby=e.employeeid
                                 where 1=1 ");

            SqlParameter[] parameters = {
					new SqlParameter("@areaid", SqlDbType.NVarChar,100),
					new SqlParameter("@cityid", SqlDbType.NVarChar,100),
					new SqlParameter("@provinceid", SqlDbType.NVarChar,100),
                    new SqlParameter("@merchantid", SqlDbType.Int,4),
                    new SqlParameter("@areatype", SqlDbType.Int,4),
                    new SqlParameter("@auditStatus", SqlDbType.Int,4),
                    new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50)
					};


            parameters[5].Value = status;
            parameters[6].Value = distributionCode;

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ar.areaid=@areaid";
                count.Append(" and ar.areaid=@areaid");
                parameters[0].Value = areaid;
            }

            if (!string.IsNullOrEmpty(cityid))
            {
                strResult += " and c.cityid=@cityid";
                count.Append(" and c.cityid=@cityid");
                parameters[1].Value = cityid;
            }

            if (!string.IsNullOrEmpty(provinceid))
            {
                strResult += " and p.provinceid=@provinceid";
                count.Append(" and p.provinceid=@provinceid");
                parameters[2].Value = provinceid;
            }

            if (merchantid > 0)
            {
                strResult += " and a1.merchantid=@merchantid";
                count.Append(" and a1.merchantid=@merchantid");
                parameters[3].Value = merchantid;
            }

            if (areatype > 0)
            {
                strResult += " and a1.areatype=@areatype";
                count.Append(" and a1.areatype=@areatype");
                parameters[4].Value = areatype;
            }
            int itemcount = Convert.ToInt32(GetOrderInfoCount(count.ToString(), parameters));
            string newSqlQuery = "";
            pi.SetItemCount(itemcount);
            int begin = pi.CurrentPageBeginItemIndex;
            int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;


            if (begin > 1)
            {
                newSqlQuery = String.Format(" SELECT * FROM ( SELECT  ROW_NUMBER() over(order by areaid ) as rowno,allrecord.* FROM ( " + strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno>={0} AND rowno<{1} ", begin, end);
            }
            else
            {
                newSqlQuery = String.Format(" SELECT * FROM (  SELECT  ROW_NUMBER() over(order by areaid) as rowno,allrecord.* FROM ( " + strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno<{0} ", end);
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, newSqlQuery, parameters).Tables[0];
        }

        //查询区域类型 (收入)
        public DataTable SearchAreaMerchantLevel(int status, string areaid, string cityid, string provinceid, int merchantid, int areatype, int expresscompanyid,string distributionCode, ref PageInfo pi)
        {
            string sql =
                    @"select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael(nolock) where ael.auditStatus=@auditStatus AND ael.DistributionCode=@DistributionCode";


            string sqlcount =
                    @"select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael(nolock) where ael.auditStatus=@auditStatus AND ael.DistributionCode=@DistributionCode";

            SqlParameter[] parameters = {
					new SqlParameter("@areaid", SqlDbType.NVarChar,100),
					new SqlParameter("@cityid", SqlDbType.NVarChar,100),
					new SqlParameter("@provinceid", SqlDbType.NVarChar,100),
                    new SqlParameter("@merchantid", SqlDbType.Int,4),
                    new SqlParameter("@areatype", SqlDbType.Int,4),
                    new SqlParameter("@expresscompanyid", SqlDbType.Int,4),
                    new SqlParameter("@auditStatus", SqlDbType.Int,4),
                    new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50)
					};

            parameters[6].Value = status;
            parameters[7].Value = distributionCode;

            if (merchantid > 0)
            {
                sql += " and ael.merchantid=@merchantid";
                sqlcount += " and ael.merchantid=@merchantid";
                parameters[3].Value = merchantid;
            }

            if (areatype > 0)
            {
                sql += " and ael.areatype=@areatype";
                sqlcount += " and ael.areatype=@areatype";
                parameters[4].Value = areatype;
            }

            if (expresscompanyid > 0)
            {
                sql += " and ael.expresscompanyid=@expresscompanyid";
                sqlcount += " and ael.expresscompanyid=@expresscompanyid";
                parameters[5].Value = expresscompanyid;
            }

            sql += " group by ael.areaid ";
            sqlcount += " group by ael.areaid";


            string strResult = string.Format(@"select p.provincename,cityname,areaname,a1.areaid,a1.dodate,e.employeename,a1.AuditTime,s.statusname,a1.auditstatus
                                                from ({0}) as a
                                                join AreaExpressLevelIncome a1(nolock) on a1.autoid=a.id
                                                join RFD_PMS.dbo.Area ar(nolock) on a.areaid=ar.areaid
                                                join RFD_PMS.dbo.City c(nolock) on ar.cityid=c.cityid
                                                join RFD_PMS.dbo.Province p(nolock)  on c.provinceid=p.ProvinceID 
                                                join RFD_PMS.dbo.StatusInfo s(nolock) on a1.auditstatus=s.statusno and s.statustypeno='306'
                                                left join RFD_PMS.dbo.employee e(nolock) on a1.auditby=e.employeeid
                                                 where 1=1 ", sql);


            StringBuilder count = new StringBuilder();
            count.AppendFormat(
                                @"select count(1) from ({0}) as a
                                join AreaExpressLevelIncome a1(nolock) on a1.autoid=a.id
                                join RFD_PMS.dbo.Area ar(nolock) on a.areaid=ar.areaid
                                join RFD_PMS.dbo.City c(nolock) on ar.cityid=c.cityid
                                join RFD_PMS.dbo.Province p(nolock)  on c.provinceid=p.ProvinceID 
                                join RFD_PMS.dbo.StatusInfo s(nolock) on a1.auditstatus=s.statusno and s.statustypeno='306'
                                left join RFD_PMS.dbo.employee e(nolock) on a1.auditby=e.employeeid
                                 where 1=1 ", sqlcount);

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ar.areaid=@areaid";
                count.Append(" and ar.areaid=@areaid");
                parameters[0].Value = areaid;
            }

            if (!string.IsNullOrEmpty(cityid))
            {
                strResult += " and c.cityid=@cityid";
                count.Append(" and c.cityid=@cityid");
                parameters[1].Value = cityid;
            }

            if (!string.IsNullOrEmpty(provinceid))
            {
                strResult += " and p.provinceid=@provinceid";
                count.Append(" and p.provinceid=@provinceid");
                parameters[2].Value = provinceid;
            }

            int itemcount = Convert.ToInt32(GetOrderInfoCount(count.ToString(), parameters));
            string newSqlQuery = "";
            pi.SetItemCount(itemcount);
            int begin = pi.CurrentPageBeginItemIndex;
            int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;


            if (begin > 1)
            {
                newSqlQuery = String.Format(" SELECT * FROM ( SELECT  ROW_NUMBER() over(order by areaid ) as rowno,allrecord.* FROM ( " + strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno>={0} AND rowno<{1} ", begin, end);
            }
            else
            {
                newSqlQuery = String.Format(" SELECT * FROM (  SELECT  ROW_NUMBER() over(order by areaid) as rowno,allrecord.* FROM ( " + strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno<{0} ", end);
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, newSqlQuery, parameters).Tables[0];
        }

        /// <summary>
        ///  查询记录总数量
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int GetOrderInfoCount(string sql, SqlParameter[] parameters)
        {
            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnString, CommandType.Text, sql, parameters);
        }

        //查询区域类型信息
        public DataTable SearchAreaMerchantLevelDetail(string areaid, int status, string distributionCode)
        {
            string strResult = @"select ael.autoid,ael.areaid,ael.dodate,a.areaname,ael.merchantid,mci.merchantName,ael.areatype,ael.effectareatype,e.employeename,case when employeename is not null then ael.updatetime else null end as updatetime ,
												EnableStr=CASE ael.[Enable] WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END,ael.warehouseid,ael.expresscompanyid
												from AreaExpressLevelIncome ael (nolock) 
                                               join rfd_pms.dbo.MerchantBaseInfo mci (nolock) on ael.merchantid=mci.id
                                               join rfd_pms.dbo.Area a(nolock) on ael.areaid=a.areaid
                                               left join rfd_pms.dbo.employee e(nolock) on ael.updateby=e.employeeid
                                               where 1=1 and ael.auditstatus=@auditstatus AND ael.DistributionCode=@DistributionCode";

            SqlParameter[] parameters = {
					                        new SqlParameter("@areaid", SqlDbType.NVarChar,100),
                                            new SqlParameter("@auditstatus", SqlDbType.Int,4),
                                            new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50)
                                        };

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ael.areaid=@areaid";
                parameters[0].Value = areaid;
            }
            parameters[1].Value = status;
            parameters[2].Value = distributionCode;

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters).Tables[0];
        }

        //查询区域类型详细信息
        public DataTable SearchAreaMerchantLevelDetail(string areaid, int status, int merchantId, int areatype, int expresscompanyid, string distributionCode)
        {
            string strResult = string.Format(@"select ael.autoid,ael.areaid,ael.dodate,a.areaname,ael.merchantid,mci.merchantName,ael.areatype,ael.effectareatype,e.employeename,case when employeename is not null then ael.updatetime else null end as updatetime,ec.companyname,
                                               EnableStr=CASE ael.[Enable] WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END,ec1.CompanyName as WareHouseName
                                               from AreaExpressLevelIncome ael (nolock) 
                                               join RFD_PMS.dbo.MerchantBaseInfo mci (nolock) on ael.merchantid=mci.id
                                               join RFD_PMS.dbo.Area a(nolock) on ael.areaid=a.areaid
                                               left join RFD_PMS.dbo.employee e(nolock) on ael.updateby=e.employeeid
                                               left join RFD_PMS.dbo.expresscompany ec(nolock) on ael.expresscompanyid=ec.expresscompanyid
                                               left join rfd_pms.dbo.ExpressCompany ec1(nolock) on ec1.expressCompanyID=ael.WareHouseID and ec1.CompanyFlag=1
                                               where 1=1 and ael.auditstatus=@auditstatus AND ael.DistributionCode=@DistributionCode ");

            SqlParameter[] parameters = {
					new SqlParameter("@areaid", SqlDbType.NVarChar,100),
                    new SqlParameter("@merchantId", SqlDbType.Int,4),
                    new SqlParameter("@areatype", SqlDbType.Int,4),
                    new SqlParameter("@expresscompanyid", SqlDbType.Int,4),
                    new SqlParameter("@auditstatus", SqlDbType.Int,4),
                    new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50)
                                        };
            parameters[4].Value = status;
            parameters[5].Value = distributionCode;
            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ael.areaid=@areaid";
                parameters[0].Value = areaid;
            }

            if (merchantId > 0)
            {
                strResult += " and ael.merchantid=@merchantId";
                parameters[1].Value = merchantId;
            }

            if (areatype > 0)
            {
                strResult += " and ael.areatype=@areatype";
                parameters[2].Value = areatype;
            }

            if (expresscompanyid > 0)
            {
                strResult += " and ael.expresscompanyid=@expresscompanyid";
                parameters[3].Value = expresscompanyid;
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters).Tables[0];
        }

        //设置生效
        public bool SetAreaMerchantLeverAudit(int autoid, DateTime doDate, int auditBy, DateTime audittime)
        {
            string str = string.Format(@"update AreaExpressLevelIncome set dodate='{0}',auditstatus=1,auditby={1},audittime='{2}',IsChange=@IsChange
                                               where 1=1 and auditstatus=0 ", doDate, auditBy, audittime);

            SqlParameter[] parameters =
            {
				new SqlParameter("@autoid", SqlDbType.Int,4),
                new SqlParameter("@IsChange", SqlDbType.Bit) { Value=true }
            };

            if (autoid > 0)
            {
                str += " and autoid=@autoid";
                parameters[0].Value = autoid;
            }

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
            if (rowCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //设置生效New
        public bool SetAreaMerchantLeverAuditEx(int autoid, DateTime doDate, int auditBy, int auditstatus, DateTime audittime)
        {
            string str = string.Format(@"update AreaExpressLevelIncome set dodate='{0}',auditstatus=1,auditby={1},audittime='{2}',IsChange=@IsChange
                                               where 1=1 and auditstatus={3} ", doDate, auditBy, audittime, auditstatus);

            SqlParameter[] parameters = 
            {
			    new SqlParameter("@autoid", SqlDbType.Int,4),
                new SqlParameter("@IsChange", SqlDbType.Bit) { Value=true }
            };

            if (autoid > 0)
            {
                str += " and autoid=@autoid";
                parameters[0].Value = autoid;
            }

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
            if (rowCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //返回待生效的区域
        public DataTable AreaMerchantLevelNum(int num, DateTime nowDate)
        {
            string strResult = string.Format(@"select top {0} autoid,areaid,merchantid,warehouseid,areatype,enable,effectareatype,dodate,auditstatus,expresscompanyid from areaexpresslevelincome ael(nolock)
                                              where ael.auditstatus=1 and dodate<='{1}' ", num, nowDate);

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult).Tables[0];
        }

        //更新区域类型
        public bool AreaMerchantLevelUpdate(int autoid)
        {
            string str = string.Format(@"update areaexpresslevelincome set auditstatus=2,areatype=effectareatype,IsChange=@IsChange
                                               where auditstatus=1 and Enable=1 ");

            SqlParameter[] parameters = 
            {
			    new SqlParameter("@autoid", SqlDbType.Int,4),
                new SqlParameter("@IsChange", SqlDbType.Bit) { Value=true }
            };

            if (autoid > 0)
            {
                str += " and autoid=@autoid";
                parameters[0].Value = autoid;
            }
            else
            {
                return false;
            }

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
            if (rowCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //添加区域类型
        public bool AreaMerchantLevelAdd(int autoid)
        {
            string str = string.Format(@"update areaexpresslevelincome set auditstatus=2,enable=1,areatype=effectareatype,IsChange=@IsChange
                                               where auditstatus=1 and Enable=3 ");

            SqlParameter[] parameters = 
            {
				new SqlParameter("@autoid", SqlDbType.Int,4),
                new SqlParameter("@IsChange", SqlDbType.Bit) { Value=true }
            };

            if (autoid > 0)
            {
                str += " and autoid=@autoid";
                parameters[0].Value = autoid;
            }
            else
            {
                return false;
            }

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
            if (rowCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //删除区域类型
        public bool AreaMerchantLevelDel(int autoid)
        {
            string str = string.Format(@"update areaexpresslevelincome set auditstatus=2,enable=0,IsChange=@IsChange
                                               where auditstatus=1 and Enable=2 ");

            SqlParameter[] parameters = 
            {
				new SqlParameter("@autoid", SqlDbType.Int,4),
                new SqlParameter("@IsChange", SqlDbType.Bit) { Value=true }
            };

            if (autoid > 0)
            {
                str += " and autoid=@autoid";
                parameters[0].Value = autoid;
            }
            else
            {
                return false;
            }

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
            if (rowCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //设置置回
        public bool ReSetAreaMerchantLevel(int autoid)
        {
            string str = string.Format(@"update AreaExpressLevelIncome set auditstatus=3,IsChange=@IsChange 
                                               where 1=1 and auditstatus=0 ");

            SqlParameter[] parameters = 
            {
				new SqlParameter("@autoid", SqlDbType.Int,4),
                new SqlParameter("@IsChange", SqlDbType.Bit) { Value=true }
            };

            if (autoid > 0)
            {
                str += " and autoid=@autoid";
                parameters[0].Value = autoid;
            }
            else
            {
                return false;
            }

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
            if (rowCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public DataTable GetSortingCenter()
        {
            sqlStr = @"SELECT  ExpressCompanyID ,
								CompanyName
						FROM    rfd_pms.dbo.ExpressCompany(NOLOCK)
						WHERE   CompanyFlag = 1
								AND IsDeleted = 0;";

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr).Tables[0];
        }

        public DataSet GetExportData()
        {
            sqlStr = @"SELECT  ID ,
								MerchantName
						FROM    rfd_pms.dbo.MerchantBaseInfo(NOLOCK)
						WHERE   IsDeleted = 0;

						SELECT p.ProvinceID,
							   p.ProvinceName,
							   c.CityID,
							   c.CityName,
							   a.AreaID,
							   a.AreaName
						FROM   rfd_pms.dbo.Province p(NOLOCK)
							   JOIN rfd_pms.dbo.City c(NOLOCK)
									ON  c.ProvinceID = p.ProvinceID
							   JOIN rfd_pms.dbo.Area a(NOLOCK)
									ON  a.CityID = c.CityID
						WHERE  p.IsDeleted = 0
							   AND c.IsDeleted = 0
							   AND a.IsDeleted = 0;

						SELECT si.StatusNO
						FROM   rfd_pms.dbo.StatusInfo si(NOLOCK)
						WHERE  si.StatusTypeNO = '305'
							   AND si.IsDelete = 0;

						SELECT  ExpressCompanyID ,
								CompanyName
						FROM    rfd_pms.dbo.ExpressCompany(NOLOCK)
						WHERE   CompanyFlag = 1
								AND IsDeleted = 0;";
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
        }

        public DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode, PageInfo pi)
        {
            sqlStr = @"
						SELECT  @records = COUNT(1)
						FROM    AreaExpressLevelIncome AS aeli (NOLOCK)
								JOIN rfd_pms.dbo.Area (NOLOCK) a ON aeli.AreaID = a.AreaID AND a.IsDeleted=0
								JOIN rfd_pms.dbo.City (NOLOCK) c ON a.CityID = c.CityID AND c.IsDeleted=0
								JOIN rfd_pms.dbo.Province (NOLOCK) p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
								JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = aeli.ExpressCompanyID AND ec.IsDeleted=0
								JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = aeli.MerchantID AND mbi.IsDeleted=0
								WHERE (aeli.Enable<>0) {0}

						IF ( @records IS NULL
							 OR @records = 0
						   ) 
							BEGIN
								SET @records = 0
								SET @pages = 0
								RETURN
							END
						SET @pages = @records / @pageSize
						IF ( @records % @pageSize > 0 ) 
							SET @pages = @pages + 1
																							
						DECLARE @rowStart INT
						SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1 ;

						WITH    t AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY aeli.CreateTime DESC ) AS rowNo ,
												aeli.AutoID,
												aeli.ExpressCompanyID ,
                                                aeli.DistributionCode,
												ec.CompanyName ,
												p.ProvinceID ,
												p.ProvinceName ,
												c.CityID ,
												c.CityName ,
												a.AreaID ,
												a.AreaName ,
												AreaType = CASE WHEN aeli.Enable = 1 THEN aeli.AreaType
																ELSE NULL
														   END ,
												EffectAreaType = CASE WHEN aeli.Enable IN ( 1, 2, 3 )
																	  THEN aeli.EffectAreaType
																	  ELSE NULL
																 END ,
												aeli.MerchantID,
												mbi.MerchantName ,
												si.StatusName AS AuditStatusStr ,
												EnableStr = CASE aeli.Enable
															  WHEN 0 THEN '已删除'
															  WHEN 1 THEN '可用'
															  WHEN 2 THEN '待删除'
															  WHEN 3 THEN '新增'
															END ,
												aeli.WareHouseID,
												w1.CompanyName AS SortCenterName
									   FROM     AreaExpressLevelIncome AS aeli (NOLOCK)
												JOIN rfd_pms.dbo.Area (NOLOCK) a ON aeli.AreaID = a.AreaID AND a.IsDeleted=0
												JOIN rfd_pms.dbo.City (NOLOCK) c ON a.CityID = c.CityID AND c.IsDeleted=0
												JOIN rfd_pms.dbo.Province (NOLOCK) p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
												JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = aeli.ExpressCompanyID AND ec.IsDeleted=0
												JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = aeli.MerchantID AND mbi.IsDeleted=0
												JOIN rfd_pms.dbo.StatusInfo (NOLOCK) si ON si.StatusNO = aeli.AuditStatus
																			   AND si.StatusTypeNO = 306 AND si.IsDelete=0
												LEFT JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) w1 ON w1.ExpressCompanyID = aeli.WareHouseID AND w1.IsDeleted=0
												WHERE (aeli.Enable<>0) {0}
									 )
							SELECT  *
							FROM    t
							WHERE   rowNo BETWEEN @rowStart AND @rowStart + @pageSize - 1 ;";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@records", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pages", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pi.PageSize });
            parameters.Add(new SqlParameter("@pageNo", SqlDbType.Int) { Value = pi.CurrentPageIndex });

            if (!string.IsNullOrEmpty(provinceId))
            {
                sbWhere.Append(" AND p.ProvinceID=@ProvinceID ");
                parameters.Add(new SqlParameter("@ProvinceID", SqlDbType.NVarChar,10) { Value = provinceId });
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                sbWhere.Append(" AND c.CityID=@CityID ");
                parameters.Add(new SqlParameter("@CityID", SqlDbType.NVarChar, 10) { Value = cityId });
            }

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=@AreaID ");
                parameters.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar, 50) { Value = areaId });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND aeli.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND aeli.AreaType=@AreaType ");
                parameters.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aeli.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND aeli.AuditStatus=@AuditStatus ");
                parameters.Add(new SqlParameter("@AuditStatus", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aeli.distributionCode=@distributionCode ");
                parameters.Add(new SqlParameter("@distributionCode", SqlDbType.NVarChar,50) { Value = distributionCode });
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray());
            pi.ItemCount = (int)parameters[0].Value;
            pi.PageCount = (int)parameters[1].Value;
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchAreaTypeExprotList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode)
        {
            sqlStr = @"
SELECT  aeli.AutoID AS 日志编号 ,
        ec.CompanyName AS 配送公司 ,
        p.ProvinceName AS 省 ,
        c.CityName AS 市 ,
        a.AreaName AS 区 ,
        生效区域类型 = CASE WHEN aeli.Enable = 1 THEN aeli.AreaType
                      ELSE NULL
                 END ,
        首次新增区域类型 = CASE WHEN aeli.Enable IN ( 1, 2, 3 )
                        THEN aeli.EffectAreaType
                        ELSE NULL
                   END ,
        mbi.MerchantName AS 商家 ,
        si.StatusName AS 审核状态 ,
        生效状态 = CASE aeli.Enable
                 WHEN 0 THEN '已删除'
                 WHEN 1 THEN '可用'
                 WHEN 2 THEN '待删除'
                 WHEN 3 THEN '新增'
               END ,
        分拣中心 = w1.CompanyName
FROM    AreaExpressLevelIncome AS aeli ( NOLOCK )
        JOIN rfd_pms.dbo.Area (NOLOCK) a ON aeli.AreaID = a.AreaID
                                            AND a.IsDeleted = 0
        JOIN rfd_pms.dbo.City (NOLOCK) c ON a.CityID = c.CityID
                                            AND c.IsDeleted = 0
        JOIN rfd_pms.dbo.Province (NOLOCK) p ON c.ProvinceID = p.ProvinceID
                                                AND p.IsDeleted = 0
        JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = aeli.ExpressCompanyID
                                                       AND ec.IsDeleted = 0
        JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = aeli.MerchantID
                                                          AND mbi.IsDeleted = 0
        JOIN rfd_pms.dbo.StatusInfo (NOLOCK) si ON si.StatusNO = aeli.AuditStatus
                                                   AND si.StatusTypeNO = 306
                                                   AND si.IsDelete = 0
        LEFT JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) w1 ON w1.ExpressCompanyID = aeli.WareHouseID
                                                            AND w1.IsDeleted = 0
WHERE   ( aeli.Enable <> 0 ) {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(provinceId))
            {
                sbWhere.Append(" AND p.ProvinceID=@ProvinceID ");
                parameters.Add(new SqlParameter("@ProvinceID", SqlDbType.NVarChar, 10) { Value = provinceId });
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                sbWhere.Append(" AND c.CityID=@CityID ");
                parameters.Add(new SqlParameter("@CityID", SqlDbType.NVarChar, 10) { Value = cityId });
            }

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=@AreaID ");
                parameters.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar, 50) { Value = areaId });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND aeli.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND aeli.AreaType=@AreaType ");
                parameters.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aeli.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND aeli.AuditStatus=@AuditStatus ");
                parameters.Add(new SqlParameter("@AuditStatus", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aeli.distributionCode=@distributionCode ");
                parameters.Add(new SqlParameter("@distributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string merchantId, string distributionCode)
        {
            sqlStr = @"
SELECT  aelil.LogID ,
        p.ProvinceName ,
        c.CityName ,
        a.AreaName ,
        ec.CompanyName ,
        w1.CompanyName as SortCenterName,
        mbi.MerchantName ,
        EmployeeName = CASE aelil.CreateBy
                         WHEN '0' THEN '系统'
                         ELSE e.EmployeeName
                       END ,
        aelil.CreateTime ,
        aelil.LogText
FROM  AreaExpressLevelIncomeLog AS aelil(NOLOCK)
        JOIN rfd_pms.dbo.Area (NOLOCK) a ON aelil.AreaID = a.AreaID
                                    AND a.IsDeleted = 0
        JOIN rfd_pms.dbo.City (NOLOCK) c ON c.CityID = a.CityID
                                    AND c.IsDeleted = 0
        JOIN rfd_pms.dbo.Province (NOLOCK) p ON p.ProvinceID = c.ProvinceID
                                        AND p.IsDeleted = 0
        JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = aelil.ExpressCompanyID
                                               AND ec.IsDeleted = 0
        JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = aelil.MerchantID
                                                  AND mbi.IsDeleted = 0
        LEFT JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) w1 ON w1.ExpressCompanyID = aelil.WarehouseId
                                                    AND w1.IsDeleted = 0
        LEFT JOIN rfd_pms.dbo.employee (NOLOCK) e ON e.EmployeeID = CONVERT(INT, aelil.CreateBy)
		WHERE (1=1) {0} order by aelil.CreateTime desc";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=@AreaID ");
                parameters.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar,50) { Value = areaId });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND aelil.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aelil.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aelil.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            if (!string.IsNullOrEmpty(wareHouse))
            {
                sbWhere.Append(" AND aelil.WareHouseID=@WareHouseID ");
                parameters.Add(new SqlParameter("@WareHouseID", SqlDbType.NVarChar, 20) { Value = wareHouse });
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray());
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        #region 优化后
        public int GetAreaLevelIncomeListStat(AreaLevelIncomeSearchModel searchModel)
        {
            string sql = @"
SELECT count(1)
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM RFD_PMS.dbo.Area a(NOLOCK) JOIN RFD_PMS.dbo.City AS c(NOLOCK)
		 ON a.CityID=c.CityID JOIN RFD_PMS.dbo.Province AS p(NOLOCK)  ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) 
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=@DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag
	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
		JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
		JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=@DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN AreaExpressLevelIncome aeli(NOLOCK)
 ON aeli.Enable in (1,2,3) AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS INT) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=@DistributionCode
	WHERE (1=1) {0}
";
            string sqlWhere = string.Empty;
            List<SqlParameter> parameterList = BuildCondition(searchModel, out sqlWhere);

            sql = string.Format(sql,sqlWhere);

            var n = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray());
            return int.Parse(n.ToString());
        }

        private List<SqlParameter> BuildCondition(AreaLevelIncomeSearchModel searchModel, out string sqlWhere)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            StringBuilder sbWhere = new StringBuilder();
            parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = searchModel.DistributionCode });
            if (!string.IsNullOrEmpty(searchModel.ProvinceID))
            {
                sbWhere.Append(" AND pca.ProvinceID=@ProvinceID ");
                parameterList.Add(new SqlParameter("@ProvinceID", SqlDbType.NVarChar, 10) { Value = searchModel.ProvinceID });
            }

            if (!string.IsNullOrEmpty(searchModel.CityID))
            {
                sbWhere.Append(" AND pca.CityID=@CityID ");
                parameterList.Add(new SqlParameter("@CityID", SqlDbType.NVarChar, 10) { Value = searchModel.CityID });
            }

            if (!string.IsNullOrEmpty(searchModel.AreaID))
            {
                sbWhere.Append(" AND pca.AreaID=@AreaID ");
                parameterList.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar, 50) { Value = searchModel.AreaID });
            }

            if (searchModel.MerchantID>0)
            {
                sbWhere.Append(" AND merchant.MerchantID=@MerchantID ");
                parameterList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = searchModel.MerchantID });
            }

            if (!string.IsNullOrEmpty(searchModel.WareHouse))
            {
                sbWhere.Append(" AND express.ExpressCompanyID in (" + searchModel.WareHouse + ") ");
            }

            if (searchModel.AuditStatus == -1)
            {
                sbWhere.Append(" AND aeli.AuditStatus is null ");
            }

            if (searchModel.AuditStatus > -1)
            {
                sbWhere.Append(" AND aeli.AuditStatus=@AuditStatus ");
                parameterList.Add(new SqlParameter("@AuditStatus", SqlDbType.Int) { Value = searchModel.AuditStatus });
            }

            if (searchModel.AreaType > 0)
            {
                sbWhere.Append(" AND aeli.AreaType=@AreaType ");
                parameterList.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = searchModel.AreaType });
            }

            if (!string.IsNullOrEmpty(searchModel.GoodsCategoryCode ))
            {
                sbWhere.Append(" AND aeli.GoodsCategoryCode in (" + searchModel.GoodsCategoryCode + ")");
            }

            sqlWhere = sbWhere.ToString();
            return parameterList;
        }

        public DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel, PageInfo pi)
        {
            #region sql
            string sql = @"
WITH t AS 
(
SELECT ROW_NUMBER() OVER(ORDER BY pca.AreaID) AS SerialNo,
pca.ProvinceID,
pca.ProvinceName,
pca.CityID,
pca.CityName,
pca.AreaID,
pca.AreaName,
express.ExpressCompanyID,
express.CompanyName,
merchant.MerchantID,
merchant.MerchantName,
aeli.goodscategorycode,
case  when aeli.AutoId IS NULL then '' when gc.goodscategoryname is null then '否' else gc.goodscategoryname end goodscategoryname,
aeli.AutoId,
aeli.AreaType,
CASE WHEN aeli.AreaType=0 then '' else aeli.AreaType end AreaTypeStr,
aeli.EffectAreaType,
aeli.Enable,
CASE aeli.Enable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END EnableStr,
CASE WHEN aeli.AutoId IS NULL THEN -1 ELSE aeli.AuditStatus END AuditStatus,
CASE WHEN aeli.AutoId IS NULL THEN '待维护' ELSE case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已生效' when 3 then '置回' else NULL END END AuditStatusStr,
aeli.DoDate
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM RFD_PMS.dbo.Area a(NOLOCK) JOIN RFD_PMS.dbo.City AS c(NOLOCK)
		 ON a.CityID=c.CityID JOIN RFD_PMS.dbo.Province AS p(NOLOCK)  ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) 
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=@DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag
	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
		JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
		JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=@DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN AreaExpressLevelIncome aeli(NOLOCK)
 ON aeli.Enable in (1,2,3) AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS INT) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=@DistributionCode
    LEFT JOIN RFD_PMS.dbo.goodscategory gc(nolock) on gc.goodscategorycode=aeli.goodscategorycode
	WHERE (1=1) {0}
	)
	SELECT * FROM t WHERE t.SerialNo BETWEEN @rowStart AND @rowEnd
";
            #endregion
            string sqlWhere = string.Empty;
            List<SqlParameter> parameterList = BuildCondition(searchModel, out sqlWhere);
            parameterList.Add(new SqlParameter("@rowStart", SqlDbType.Int) { Value = pi.CurrentPageStartRowNum });
            parameterList.Add(new SqlParameter("@rowEnd", SqlDbType.Int) { Value = pi.CurrentPageEndRowNum });
            sql = string.Format(sql, sqlWhere);

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray()).Tables[0];
        }

        public DataTable GetAreaLevelIncomeExprotList(AreaLevelIncomeSearchModel searchModel)
        {
            string sql = @"
SELECT ROW_NUMBER() OVER(ORDER BY pca.AreaID) AS 序号,
pca.ProvinceName 省份,
pca.CityName 城市,
pca.AreaName 地区,
express.CompanyName 分拣中心,
merchant.MerchantName 商家,
case  when aeli.AutoId IS NULL then '' when gc.goodscategoryname is null then '否' else gc.goodscategoryname end 货物品类,
CASE WHEN aeli.AreaType=0 then '' else aeli.AreaType end 生效区域类型,
aeli.EffectAreaType 待生效区域类型,
CASE aeli.Enable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END 生效状态,
CASE WHEN aeli.AutoId IS NULL THEN '待维护' ELSE case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已生效' when 3 then '置回' else NULL END END 审批状态,
aeli.DoDate 生效时间
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM RFD_PMS.dbo.Area a(NOLOCK) JOIN RFD_PMS.dbo.City AS c(NOLOCK)
		 ON a.CityID=c.CityID JOIN RFD_PMS.dbo.Province AS p(NOLOCK)  ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) 
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=@DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag
	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
		JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
		JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=@DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN AreaExpressLevelIncome aeli(NOLOCK)
 ON aeli.Enable in (1,2,3) AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS INT) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=@DistributionCode
    LEFT JOIN RFD_PMS.dbo.goodscategory gc(nolock) on gc.goodscategorycode=aeli.goodscategorycode
	WHERE (1=1) {0}
";
            string sqlWhere = string.Empty;
            List<SqlParameter> parameterList = BuildCondition(searchModel, out sqlWhere);
            sql = string.Format(sql, sqlWhere);
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray()).Tables[0];
        }

        public bool UpdateAreaLevelIncomeStatus(AreaExpressLevelIncome model)
        {
            string sql = @"UPDATE AreaExpressLevelIncome SET AuditStatus=@AuditStatus,AuditBy=@AuditBy,AuditTime=getdate(),IsChange=1{0} WHERE AutoId=@AutoId ";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@AuditStatus", SqlDbType.Decimal) { Value = model.AuditStatus });
            parameterList.Add(new SqlParameter("@AuditBy", SqlDbType.Decimal) { Value = model.AuditBy });
            parameterList.Add(new SqlParameter("@AutoId", SqlDbType.Decimal) { Value = model.AutoId });
            if (!string.IsNullOrEmpty(model.DoDate.ToString()))
            {
                sql = string.Format(sql, ",DoDate=@DoDate");
                parameterList.Add(new SqlParameter("@DoDate", SqlDbType.Date) { Value = model.DoDate });
            }
            else
            {
                sql = string.Format(sql, "");
            }
            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameterList.ToArray());
            return rowCount > 0;
        }

        public DataTable GetWaitEffectList()
        {
            string strResult = @"select autoid,areaid,merchantid,warehouseid,areatype,enable,effectareatype,dodate,auditstatus,expresscompanyid,GoodsCategoryCode 
                                        from areaexpresslevelincome ael(nolock)
                                              where ael.auditstatus=1 and dodate<=@dodate ";
            SqlParameter[] parameters ={
                                           new SqlParameter("@dodate",SqlDbType.Date),
                                      };
            parameters[0].Value = DateTime.Now.ToString("yyyy-MM-dd");
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult,parameters).Tables[0];
        }

        /// <summary>
        /// 根据区、分拣、品类、商家获取区域类型
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public int GetAreaTypeByCondition(AreaLevelIncomeSearchModel searchModel)
        {
            string sql = @"SELECT AreaType FROM RFD_FMS.dbo.AreaExpressLevelIncome ael1(NOLOCK) WHERE ael1.[Enable] IN (1, 2) {0}";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(searchModel.GoodsCategoryCode))
            {
                sqlWhere.Append(" AND ael1.GoodsCategoryCode =@GoodsCategoryCode");
                parameterList.Add(new SqlParameter("@GoodsCategoryCode", SqlDbType.NVarChar,10) { Value = searchModel.GoodsCategoryCode });
            }
            if (searchModel.MerchantID>0)
            {
                sqlWhere.Append(" AND ael1.MerchantID =@MerchantID");
                parameterList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = searchModel.MerchantID });
            }
            if (!string.IsNullOrEmpty(searchModel.WareHouse ))
            {
                sqlWhere.Append(" AND ael1.WareHouseID =@WareHouseID");
                parameterList.Add(new SqlParameter("@WareHouseID", SqlDbType.NVarChar, 20) { Value = searchModel.WareHouse });
            }
            if (!string.IsNullOrEmpty(searchModel.AreaID))
            {
                sqlWhere.Append(" AND ael1.AreaID =@AreaID");
                parameterList.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar, 50) { Value = searchModel.AreaID });
            }
            if (!string.IsNullOrEmpty(searchModel.DistributionCode))
            {
                sqlWhere.Append(" AND ael1.DistributionCode =@DistributionCode");
                parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = searchModel.DistributionCode });
            }
            sql = string.Format(sql,sqlWhere.ToString());
            object n = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray());
            return DataConvert.ToInt(n, 0);
        }

        public DataTable GetAreaExpressByID(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExistAreaExpress( AreaExpressLevelIncome areaExpressLevelIncome)
        {
            throw new NotImplementedException();
        }

        #endregion


        public DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel)
        {
            throw new NotImplementedException();
        }

        public DataTable GetAreaTypeByConditionToDt(AreaLevelIncomeSearchModel incomeSearchModel)
        {
            throw new NotImplementedException();
        }
    }
}
