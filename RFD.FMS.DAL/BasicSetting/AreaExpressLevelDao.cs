using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using System.Xml;

namespace RFD.FMS.DAL.BasicSetting
{
    public class AreaExpressLevelDao : SqlServerDao, IAreaExpressLevelDao
    {
        private string sqlStr = "";

		public DataTable SearchArea(string provinceId, string cityId, string areaId, string stationId, string merchantId, string distributionCode,ref PageInfo pi)
        {
            sqlStr = @"SELECT DISTINCT
								p.ProvinceID ,
								p.ProvinceName ,
								c.CityID ,
								c.CityName ,
								a.AreaID ,
								a.AreaName ,
								IsAreaType = CASE WHEN ( ael.AreaType IS NOT NULL
														 OR ael.EffectAreaType IS NOT NULL
													   )
													   AND Enable IN ( 1,2, 3 ) THEN '√'
												  ELSE ''
											 END
						FROM    rfd_pms.dbo.Province p ( NOLOCK )
								JOIN rfd_pms.dbo.City c ( NOLOCK ) ON c.ProvinceID = p.ProvinceID
								JOIN rfd_pms.dbo.Area a ( NOLOCK ) ON a.CityID = c.CityID
								LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = a.AreaID 
																			AND ael.Enable IN ( 1,2, 3 ) 
																			AND ael.DistributionCode=@DistributionCode 
																			{1}
						WHERE   p.IsDeleted = 0
								AND c.IsDeleted = 0
								AND a.IsDeleted = 0
							  {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            StringBuilder sbWhere1 = new StringBuilder();

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

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND a.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere1.Append(" AND ael.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(stationId))
            {
                sbWhere1.Append(" AND ael.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = stationId });
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString(), sbWhere1.ToString());

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable SearchAreaType(string areaId, string distributionCode,ref PageInfo pi)
        {
            sqlStr = @"SELECT  ael.AutoID ,
								ael.AreaID ,
								ael.ExpressCompanyID ,
								ec.CompanyName ,
								ael.WareHouseID ,
								WareHouseName = CASE ael.WareHouseType
												  WHEN 1 THEN w.WareHouseName
												  WHEN 2 THEN w1.CompanyName
												  ELSE ''
												END ,
								mbi.ID ,
								mbi.MerchantName ,
								AreaType = CASE WHEN ael.[Enable] = 3 THEN NULL
												ELSE ael.AreaType
										   END ,
								EffectAreaType = CASE WHEN AuditStatus IN ( 0, 1, 3 )
													  THEN ael.EffectAreaType
													  ELSE NULL
												 END ,
								AuditStatusStr = CASE AuditStatus
												   WHEN 0 THEN '未审核'
												   WHEN 1 THEN '已审核'
												   WHEN 2 THEN '已同步'
												   WHEN 3 THEN '置回'
												   ELSE NULL
												 END,
								EnableStr=CASE ael.[Enable] WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END
						FROM    AreaExpressLevel ael ( NOLOCK )
								LEFT JOIN rfd_pms.dbo.Warehouse w ( NOLOCK ) ON w.WareHouseID = ael.WareHouseID
								LEFT JOIN rfd_pms.dbo.ExpressCompany w1 ( NOLOCK ) ON w1.ExpressCompanyID = ael.WareHouseID
																			  AND w1.CompanyFlag = 1
								JOIN rfd_pms.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = ael.ExpressCompanyID
								JOIN rfd_pms.dbo.MerchantBaseInfo mbi ( NOLOCK ) ON mbi.ID = ael.MerchantID
						WHERE   ael.[Enable] IN ( 1,2, 3 ) {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND ael.AreaID=@AreaID ");
                parameters.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar, 50) { Value = areaId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND ael.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable SearchAreaTypeByAutoId(string autoId)
        {
            sqlStr = @"SELECT   ael.AutoID,
								ael.AreaID,
								ael.AreaID,
							    ael.ExpressCompanyID,
							    ael.WareHouseID,
								ael.Enable,
							    ael.AreaType,
								ael.WareHouseType,
								ael.MerchantID,
								ael.ProductID
						FROM   AreaExpressLevel ael(NOLOCK)
						WHERE AutoID=@AutoID";
            SqlParameter[] parameters ={
										   new SqlParameter("@AutoID",SqlDbType.Int),
									  };
            parameters[0].Value = autoId;
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, parameters).Tables[0];
        }

        public bool AddAreaType(AreaExpressLevel areaExpressLevel)
        {
            if (string.IsNullOrEmpty(areaExpressLevel.WareHouseID))
            {
                sqlStr = @"	SET @Flag=-1
							IF NOT EXISTS(
								  SELECT 1 FROM AreaExpressLevel(NOLOCK)
								  WHERE  ExpressCompanyID = @ExpressCompanyID
										 AND AreaID = @AreaID
										 AND ISNULL(WareHouseID,'')!=''
										 AND MerchantID=@MerchantID
										 AND [Enable] in (1,2,3)
                                         AND DistributionCode=@DistributionCode
										)
							BEGIN 
								IF NOT EXISTS(
									  SELECT 1
									  FROM   AreaExpressLevel(NOLOCK)
									  WHERE  ExpressCompanyID = @ExpressCompanyID
											 AND AreaID = @AreaID
											 AND MerchantID=@MerchantID
											 AND [Enable] in (1,2,3)
                                             AND DistributionCode=@DistributionCode
								  )
							   BEGIN
									{0}
									SET @Flag=@@ROWCOUNT
								END
								ELSE
								BEGIN
									SET @Flag=0
								END
							END
							ELSE
							BEGIN
								SET @Flag=-2
							END";
            }
            else
            {
                sqlStr = @"	SET @Flag=-1
							IF NOT EXISTS(
								  SELECT 1 FROM AreaExpressLevel(NOLOCK)
								  WHERE  ExpressCompanyID = @ExpressCompanyID
										 AND AreaID = @AreaID
										 AND ISNULL(WareHouseID,'')=''
										 AND MerchantID=@MerchantID
										 AND [Enable] in (1,2,3)
                                         AND DistributionCode=@DistributionCode
										)
							BEGIN 
								IF NOT EXISTS(
									  SELECT 1
									  FROM   AreaExpressLevel(NOLOCK)
									  WHERE  ExpressCompanyID = @ExpressCompanyID
											 AND AreaID = @AreaID
											 AND ISNULL(WareHouseID,'')=@WareHouseID
											 AND MerchantID=@MerchantID
											 AND [Enable] in (1,2,3)
                                             AND DistributionCode=@DistributionCode
								  )
							   BEGIN
									{0}
									SET @Flag=@@ROWCOUNT
								END
								ELSE
								BEGIN
									SET @Flag=0
								END
							END
							ELSE
							BEGIN
								SET @Flag=-2
							END";
            }

            string insertSql = @"INSERT INTO AreaExpressLevel
									   (AreaID
									   ,ExpressCompanyID
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
										,WareHouseType
										,MerchantID
										,ProductID
                                        ,DistributionCode
                                        ,IsChange
										)
								 VALUES
									   (@AreaID
									   ,@ExpressCompanyID
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
										,@WareHouseType
										,@MerchantID
										,@ProductID
                                        ,@DistributionCode
                                        ,@IsChange
										)";

            sqlStr = string.Format(sqlStr, insertSql);
            SqlParameter[] parameters ={
										   new SqlParameter("@AreaID",SqlDbType.NVarChar,100),
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
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
										   new SqlParameter("@WareHouseType",SqlDbType.Int),
										   new SqlParameter("@MerchantID",SqlDbType.Int),
										   new SqlParameter("@ProductID",SqlDbType.Int),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
            parameters[0].Value = areaExpressLevel.AreaID;
            parameters[1].Value = areaExpressLevel.ExpressCompanyID;
            parameters[2].Value = areaExpressLevel.WareHouseID;
            parameters[3].Value = areaExpressLevel.AreaType;
            parameters[4].Value = areaExpressLevel.Enable;
            parameters[5].Value = areaExpressLevel.EffectAreaType;
            parameters[6].Value = areaExpressLevel.DoDate;
            parameters[7].Value = areaExpressLevel.CreateBy;
            parameters[8].Value = areaExpressLevel.UpdateBy;
            parameters[9].Value = areaExpressLevel.AuditStatus;
            parameters[10].Value = areaExpressLevel.AuditBy;
            parameters[11].Direction = ParameterDirection.Output;
            parameters[12].Value = areaExpressLevel.WareHouseType;
            parameters[13].Value = areaExpressLevel.MerchantID;
            parameters[14].Value = areaExpressLevel.ProductID;
            parameters[15].Value = areaExpressLevel.DistributionCode;
            parameters[16].Value = true;

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);

            return (int)parameters[11].Value > 0 ? true : false;
        }

        public bool UpdateAreaType(AreaExpressLevel areaExpressLevel, out int autoId)
        {
            if (string.IsNullOrEmpty(areaExpressLevel.WareHouseID))
            {
                sqlStr = @"	SET @Flag=-1
						IF NOT EXISTS(
								  SELECT 1 FROM AreaExpressLevel(NOLOCK)
								  WHERE  ExpressCompanyID = @ExpressCompanyID
										 AND AreaID = @AreaID
										 AND ISNULL(WareHouseID,'')!=''
										 AND MerchantID=@MerchantID
										 AND [Enable] in (1,2,3)
                                         AND DistributionCode=@DistributionCode
										)
							BEGIN 
								IF EXISTS(
									  SELECT 1
									  FROM   AreaExpressLevel(NOLOCK)
									  WHERE  ExpressCompanyID = @ExpressCompanyID
											 AND AreaID = @AreaID
											 AND MerchantID=@MerchantID
											 AND [Enable] in (1,2,3)
                                             AND DistributionCode=@DistributionCode
								  )
							   BEGIN
									{0}
								END
								ELSE
								BEGIN
									SET @Flag=0
								END
							END	
							ELSE
							BEGIN
								SET @Flag=-2
							END";
            }
            else
            {
                sqlStr = @"	SET @Flag=-1
						IF NOT EXISTS(
								  SELECT 1 FROM AreaExpressLevel(NOLOCK)
								  WHERE  ExpressCompanyID = @ExpressCompanyID
										 AND AreaID = @AreaID
										 AND ISNULL(WareHouseID,'')=''
										 AND MerchantID=@MerchantID
										 AND [Enable] in (1,2,3)
                                         AND DistributionCode=@DistributionCode
										)
							BEGIN 
								IF NOT EXISTS(
									  SELECT 1
									  FROM   AreaExpressLevel(NOLOCK)
									  WHERE  ExpressCompanyID = @ExpressCompanyID
											 AND AreaID = @AreaID
											 AND ISNULL(WareHouseID,'')=@WareHouseID
											 AND MerchantID=@MerchantID
											 AND [Enable] in (1,2,3)
                                             AND DistributionCode=@DistributionCode
								  )
							   BEGIN
									{0}
								END
								ELSE
								BEGIN
									SET @Flag=0
								END
							END
							ELSE
							BEGIN
								SET @Flag=-2
							END";
            }
            string updateSql = @"UPDATE AreaExpressLevel
							SET    EffectAreaType = @EffectAreaType,
								   WareHouseID = @WareHouseID,
								   ProductID = @ProductID,
								   UpdateBy = @UpdateBy,
								   UpdateTime = GETDATE(),
								   AuditStatus = @AuditStatus,
                                   IsChange=@IsChange
							WHERE  AreaID = @AreaID
								   AND ExpressCompanyID = @ExpressCompanyID
								   --AND WareHouseID=@WareHouseID
								   AND MerchantID=@MerchantID
								   AND [Enable] in (1,2,3)
                                   AND DistributionCode=@DistributionCode
							SET @Flag=@@ROWCOUNT
							SELECT @AutoID=AutoID FROM AreaExpressLevel(NOLOCK) 
									WHERE AreaID = @AreaID AND ExpressCompanyID = @ExpressCompanyID AND [Enable] in (1,2,3) AND DistributionCode=@DistributionCode";
            sqlStr = string.Format(sqlStr, updateSql);
            SqlParameter[] parameters ={
										   new SqlParameter("@AreaID",SqlDbType.NVarChar,100),
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
										   new SqlParameter("@WareHouseID",SqlDbType.NVarChar,40),
										   new SqlParameter("@EffectAreaType",SqlDbType.Int),
										   new SqlParameter("@UpdateBy",SqlDbType.NVarChar,100),
										   new SqlParameter("@AuditStatus",SqlDbType.Int),
										   new SqlParameter("@Flag",SqlDbType.Int),
										   new SqlParameter("@AutoID",SqlDbType.Int),
										   new SqlParameter("@MerchantID",SqlDbType.Int),
										   new SqlParameter("@ProductID",SqlDbType.Int),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
            parameters[0].Value = areaExpressLevel.AreaID;
            parameters[1].Value = areaExpressLevel.ExpressCompanyID;
            parameters[2].Value = areaExpressLevel.WareHouseID;
            parameters[3].Value = areaExpressLevel.EffectAreaType;
            parameters[4].Value = areaExpressLevel.UpdateBy;
            parameters[5].Value = areaExpressLevel.AuditStatus;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[7].Direction = ParameterDirection.Output;
            parameters[8].Value = areaExpressLevel.MerchantID;
            parameters[9].Value = areaExpressLevel.ProductID;
            parameters[10].Value = areaExpressLevel.DistributionCode;
            parameters[11].Value = true;

            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            autoId = string.IsNullOrEmpty(parameters[7].Value.ToString()) ? 0 : (int)parameters[7].Value;
            return (int)parameters[6].Value > 0 ? true : false;
        }

        public bool DeleteAreaType(string autoId, string updateBy)
        {
            sqlStr = @"UPDATE AreaExpressLevel SET [Enable]=2,AuditStatus=0,UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange WHERE AutoID=@AutoID";
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

        #region 区域类型审批 by wangxue

        //查询区域类型
        public DataTable SearchAreaExpressCompanyLevel(int status, string areaid, string cityid, string provinceid, int expresscompanyid, int areatype, int warehousetype, string warehouseid, int merchantid, string distributionCode, ref PageInfo pi)
        {

            string sql =
                string.Format(
                    @"select ael.areaid,min(ael.autoid) as id from areaexpresslevel ael(nolock) where ael.auditStatus={0} AND DistributionCode=@DistributionCode", status);

            string sqlcount =
                string.Format(
                    @"select ael.areaid,min(ael.autoid) as id from areaexpresslevel ael(nolock) where ael.auditStatus={0} AND DistributionCode=@DistributionCode", status);


            SqlParameter[] parameters = {
					new SqlParameter("@areaid", SqlDbType.NVarChar,100),
					new SqlParameter("@cityid", SqlDbType.NVarChar,100),
					new SqlParameter("@provinceid", SqlDbType.NVarChar,100),
                    new SqlParameter("@expresscompanyid", SqlDbType.Int,4),
                    new SqlParameter("@areatype", SqlDbType.Int,4),
                    new SqlParameter("@warehousetype", SqlDbType.Int,4),
                    new SqlParameter("@warehouseid", SqlDbType.NVarChar,100),
                    new SqlParameter("@merchantid", SqlDbType.Int,4),
                    new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50)
					};

            parameters[8].Value = distributionCode;

            if (expresscompanyid > 0)
            {
                sql += " and ael.expresscompanyid=@expresscompanyid";
                sqlcount += " and ael.expresscompanyid=@expresscompanyid";
                parameters[3].Value = expresscompanyid;
            }

            if (areatype > 0)
            {
                sql += " and ael.areatype=@areatype";
                sqlcount += " and ael.areatype=@areatype";
                parameters[4].Value = areatype;
            }

            if (warehousetype > 0)
            {
                sql += " and ael.warehousetype=@warehousetype ";
                sqlcount += " and ael.warehousetype=@warehousetype";
                parameters[5].Value = warehousetype;
            }

            if (warehousetype > 0 && !string.IsNullOrEmpty(warehouseid))
            {
                sql += " and ael.warehouseid=@warehouseid ";
                sqlcount += " and ael.warehouseid=@warehouseid";
                parameters[6].Value = warehouseid;
            }

            if (merchantid > 0)
            {
                sql += " and ael.merchantid=@merchantid";
                sqlcount += " and ael.merchantid=@merchantid";
                parameters[7].Value = merchantid;
            }

            sql += " group by ael.areaid ";
            sqlcount += " group by ael.areaid";

            string strResult = string.Format(@"select p.provincename,cityname,areaname,a1.areaid,a1.dodate,e.employeename,a1.AuditTime,s.statusname,a1.auditstatus,
                                                CASE a1.warehousetype WHEN 2 THEN e1.companyname WHEN 1 THEN w.warehousename ELSE NULL END warehouseExpressCompany,
                                                b.merchantname
                                                from ({0}) as a
                                                join areaexpresslevel a1(nolock) on a1.autoid=a.id
                                                join rfd_pms.dbo.Area ar(nolock) on a.areaid=ar.areaid
                                                join rfd_pms.dbo.City c(nolock) on ar.cityid=c.cityid
                                                join rfd_pms.dbo.Province p(nolock)  on c.provinceid=p.ProvinceID 
                                                join rfd_pms.dbo.StatusInfo s(nolock) on a1.auditstatus=s.statusno and s.statustypeno='306'
                                                left join rfd_pms.dbo.employee e(nolock) on a1.AuditBy=e.employeeid
                                                LEFT JOIN rfd_pms.dbo.Warehouse w (nolock) ON w.warehouseid=a1.warehouseid AND a1.warehousetype=1
                                                LEFT JOIN rfd_pms.dbo.ExpressCompany e1(nolock) ON e1.expresscompanyid=a1.warehouseid AND a1.warehousetype=2
                                                LEFT JOIN rfd_pms.dbo.MerchantBaseInfo b(nolock) ON b.id=a1.merchantid where 1=1 ", sql);

            StringBuilder count = new StringBuilder();
            count.AppendFormat(
                                @"select count(1) from ({0}) as a
                                join areaexpresslevel a1(nolock) on a1.autoid=a.id
                                join rfd_pms.dbo.Area ar(nolock) on a.areaid=ar.areaid
                                join rfd_pms.dbo.City c(nolock) on ar.cityid=c.cityid
                                join rfd_pms.dbo.Province p(nolock)  on c.provinceid=p.ProvinceID 
                                join rfd_pms.dbo.StatusInfo s(nolock) on a1.auditstatus=s.statusno and s.statustypeno='306'
                                left join rfd_pms.dbo.employee e(nolock) on a1.AuditBy=e.employeeid
                                LEFT JOIN rfd_pms.dbo.Warehouse w (nolock) ON w.warehouseid=a1.warehouseid AND a1.warehousetype=1
                                LEFT JOIN rfd_pms.dbo.ExpressCompany e1(nolock) ON e1.expresscompanyid=a1.warehouseid AND a1.warehousetype=2
                                LEFT JOIN rfd_pms.dbo.MerchantBaseInfo b(nolock) ON b.id=a1.merchantid where 1=1 ", sqlcount);



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

        public DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, string distributionCode)
        {
            string strResult = string.Format(@"select ael.autoid,ael.areaid,ael.expresscompanyid,ael.dodate,a.areaname,ec.companyname,ael.areatype,ael.effectareatype,e.employeename,case when e.employeename is not null then ael.updatetime else null end as updatetime,
												EnableStr=CASE ael.[Enable] WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END,
                                               ael.warehouseid,ael.merchantid,ael.productid,ael.warehousetype from areaexpresslevel ael (nolock) 
                                               join rfd_pms.dbo.ExpressCompany ec (nolock) on ael.expresscompanyid=ec.expresscompanyid
                                               join rfd_pms.dbo.Area a(nolock) on ael.areaid=a.areaid
                                               left join rfd_pms.dbo.employee e(nolock) on ael.updateby=e.employeeid
                                               where 1=1 and ael.[Enable] in (1,2,3) and ael.auditstatus={0} AND ael.DistributionCode=@DistributionCode", status);

            SqlParameter[] parameters = {
					new SqlParameter("@areaid", SqlDbType.NVarChar,100),
                    new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50),
                                        };

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ael.areaid=@areaid";
                parameters[0].Value = areaid;
            }

            parameters[1].Value = distributionCode;

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters).Tables[0];
        }

        public DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, int expresscompanyid, int areatype, int warehousetype, string warehouseid, int merchantid)
        {
            string strResult = string.Format(@"select ael.autoid,ael.areaid,ael.expresscompanyid,ael.dodate,a.areaname,ec.companyname,ael.areatype,ael.effectareatype,e.employeename,case when e.employeename is not null then ael.updatetime else null end as updatetime,
												EnableStr=CASE ael.[Enable] WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END,
                                               ael.warehouseid,ael.merchantid,ael.productid,ael.warehousetype from areaexpresslevel ael (nolock) 
                                               join rfd_pms.dbo.ExpressCompany ec (nolock) on ael.expresscompanyid=ec.expresscompanyid
                                               join rfd_pms.dbo.Area a(nolock) on ael.areaid=a.areaid
                                               left join rfd_pms.dbo.employee e(nolock) on ael.updateby=e.employeeid
                                               where 1=1 and ael.[Enable] in (1,2,3) and ael.auditstatus={0}", status);

            SqlParameter[] parameters = {
					new SqlParameter("@areaid", SqlDbType.NVarChar,100),
                    new SqlParameter("@expresscompanyid", SqlDbType.Int,4),
                    new SqlParameter("@areatype", SqlDbType.Int,4),
                    new SqlParameter("@warehousetype", SqlDbType.Int,4),
                    new SqlParameter("@warehouseid", SqlDbType.NVarChar,100),
                    new SqlParameter("@merchantid", SqlDbType.Int,4),
                    new SqlParameter("@productid", SqlDbType.Int,4)};

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ael.areaid=@areaid";
                parameters[0].Value = areaid;
            }

            if (expresscompanyid > 0)
            {
                strResult += " and ael.expresscompanyid=@expresscompanyid";
                parameters[1].Value = expresscompanyid;
            }

            if (areatype > 0)
            {
                strResult += " and ael.areatype=@areatype";
                parameters[2].Value = areatype;
            }

            if (warehousetype > 0)
            {
                strResult += " and ael.warehousetype=@warehousetype";
                parameters[3].Value = warehousetype;

                if (!string.IsNullOrEmpty(warehouseid))
                {
                    strResult += " and ael.warehouseid=@warehouseid";
                    parameters[4].Value = warehouseid;
                }
            }

            if (merchantid > 0)
            {
                strResult += " and ael.merchantid=@merchantid";
                parameters[5].Value = merchantid;
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters).Tables[0];
        }

        public bool SetAreaExpressCompanyLeverAudit(int autoid, DateTime doDate, int auditBy, DateTime audittime)
        {
            string str = string.Format(@"update areaexpresslevel set dodate='{0}',auditstatus=1,auditby={1},audittime='{2}',IsChange={3}
                                               where 1=1 and auditstatus=0 ", doDate, auditBy, audittime,true);

            SqlParameter[] parameters = {
					new SqlParameter("@autoid", SqlDbType.Int,4)};

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

        public bool SetAreaExpressCompanyLeverAuditEx(int autoid, DateTime doDate, int auditBy, DateTime audittime, int auditstatus)
        {
            string str = string.Format(@"update areaexpresslevel set dodate='{0}',auditstatus=1,auditby={1},audittime='{2}',IsChange=@IsChange
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

            return true;
        }

        //新加日志
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool AddAreaExpLevelLog(AreaExpressLevelLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into AreaExpressLevelLog(");
            strSql.Append("AreaID,ExpressCompanyID,WarehouseId,AreaType,LogText,Enable,CreateBy,CreateTime,WareHouseType,MerchantID,ProductID,DistributionCode,IsChange)");
            strSql.Append(" values (");
            strSql.Append("@AreaID,@ExpressCompanyID,@WarehouseId,@AreaType,@LogText,@Enable,@CreateBy,@CreateTime,@WareHouseType,@MerchantID,@ProductID,@DistributionCode,@IsChange)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@AreaID", SqlDbType.NVarChar,100),
					new SqlParameter("@ExpressCompanyID", SqlDbType.Int,4),
					new SqlParameter("@WarehouseId", SqlDbType.NVarChar,40),
					new SqlParameter("@AreaType", SqlDbType.Int,4),
					new SqlParameter("@LogText", SqlDbType.NVarChar,250),
					new SqlParameter("@Enable", SqlDbType.TinyInt,1),
					new SqlParameter("@CreateBy", SqlDbType.NVarChar,100),
					new SqlParameter("@CreateTime", SqlDbType.DateTime),
					new SqlParameter("@WareHouseType", SqlDbType.Int),
					new SqlParameter("@MerchantID", SqlDbType.Int),
					new SqlParameter("@ProductID", SqlDbType.Int),
                    new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50),
                    new SqlParameter("@IsChange", SqlDbType.Bit)
										};
            parameters[0].Value = model.AreaID;
            parameters[1].Value = model.ExpressCompanyID;
            parameters[2].Value = model.WarehouseId;
            parameters[3].Value = model.AreaType;
            parameters[4].Value = model.LogText;
            parameters[5].Value = model.Enable;
            parameters[6].Value = model.CreateBy;
            parameters[7].Value = model.CreateTime;
            parameters[8].Value = model.WareHouseType;
            parameters[9].Value = model.MerchantID;
            parameters[10].Value = model.ProductID;
            parameters[11].Value = model.DistributionCode;
            parameters[12].Value = true;

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

        /// <summary>
        ///  查询记录总数量
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int GetOrderInfoCount(string sql, SqlParameter[] parameters)
        {
            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnString, CommandType.Text, sql, parameters);
        }

        #endregion

        /// <summary>
        /// 返回待生效的区域类型
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public DataTable AreaExpressCompanyLevelNum(int num, DateTime nowDate)
        {
            string strResult = string.Format(@"select top {0} autoid,areaid,expresscompanyid,warehouseid,areatype,enable,effectareatype,dodate,auditstatus,warehousetype,merchantid,productid from areaexpresslevel ael(nolock)
                                              where ael.auditstatus=1 and dodate<='{1}' ", num, nowDate);

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult).Tables[0];
        }

        //更新区域类型
        public bool AreaExpressCompanyLevelUpdate(int autoid)
        {
            string str = string.Format(@"update areaexpresslevel set auditstatus=2,areatype=effectareatype,IsChange=@IsChange
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

        public bool AreaExpressCompanyLevelAdd(int autoid)
        {
            string str = string.Format(@"update areaexpresslevel set auditstatus=2,enable=1,areatype=effectareatype,IsChange=@IsChange
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

        public bool AreaExpressCompanyLevelDel(int autoid)
        {
            string str = string.Format(@"update areaexpresslevel set auditstatus=2,enable=0,IsChange=@IsChange
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

        public DataSet GetExportData()
        {
            sqlStr = @"WITH    t AS ( SELECT   ExpressCompanyID ,
												CompanyName ,
												CompanyAllName
									   FROM     rfd_pms.dbo.ExpressCompany(NOLOCK)
									   WHERE    CompanyFlag in (3)
												AND ParentID=0
												AND DistributionCode <> 'rfd'
												AND IsDeleted = 0
									   UNION ALL
									   SELECT   ExpressCompanyID ,
												CompanyName ,
												CompanyAllName
									   FROM     rfd_pms.dbo.ExpressCompany(NOLOCK)
									   WHERE    CompanyFlag = 2
												AND DistributionCode = 'rfd'
												AND IsDeleted = 0
									 )
							SELECT  *
							FROM    t;

						WITH    t AS ( SELECT   w.WarehouseId ,
												w.WarehouseName
									   FROM     rfd_pms.dbo.Warehouse w ( NOLOCK )
									   WHERE    w.[Enable] = 1
									   UNION ALL
									   SELECT   'S_'+CONVERT(NVARCHAR(20),ExpressCompanyID ),
												CompanyName
									   FROM     rfd_pms.dbo.ExpressCompany (NOLOCK) ec
									   WHERE    IsDeleted = 0
												AND CompanyFlag = 1
									 )
							SELECT  *
							FROM    t;

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

						SELECT  ID ,
								MerchantName
						FROM    rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi
						WHERE   IsDeleted = 0;";
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
        }

        //设置置回区域类型
        public bool ReSetAreaExpressCompanyLevel(int autoid, int auditby, DateTime audittime)
        {
            string str = string.Format(@"update areaexpresslevel set auditstatus=3,auditby={0},audittime='{1}',IsChange=@IsChange
                                               where 1=1 and auditstatus=0 ", auditby, audittime);

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

        /// <summary>
        /// 创建仓库查询xml
        /// </summary>
        /// <param name="houseStr">字符串</param>
        /// <returns></returns>
        private XmlDocument CreateHouseXml(string houseStr)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xe;
            string[] houses;
            xe = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(xe);
            houses = houseStr.Split(',');
            for (int i = 0; i < houses.Length; i++)
            {
                xe = xmlDoc.CreateElement("id");
                if (houses[i].Trim().Contains("s"))
                {
                    xe.SetAttribute("v", houses[i].Trim().Replace("s", ""));
                    xe.SetAttribute("t", "2");
                }
                else
                {
                    xe.SetAttribute("v", houses[i].Trim());
                    xe.SetAttribute("t", "1");
                }
                xmlDoc.DocumentElement.AppendChild(xe);
            }
            return xmlDoc;
        }

        public DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode, PageInfo pi)
        {
            sqlStr = @"DECLARE @whTable TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL )
						INSERT  INTO @whTable( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseXml.nodes('/root/id') AS T ( x )
						SELECT  @records = COUNT(1)
						FROM    AreaExpressLevel (NOLOCK) ael
								JOIN rfd_pms.dbo.Area (NOLOCK) a ON ael.AreaID = a.AreaID AND a.IsDeleted=0
								JOIN rfd_pms.dbo.City (NOLOCK) c ON a.CityID = c.CityID AND c.IsDeleted=0
								JOIN rfd_pms.dbo.Province (NOLOCK) p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
								JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = ael.ExpressCompanyID AND ec.IsDeleted=0
								JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = ael.MerchantID AND mbi.IsDeleted=0
								JOIN rfd_pms.dbo.StatusInfo (NOLOCK) si ON si.StatusNO = ael.AuditStatus
															   AND si.StatusTypeNO = 306 AND si.IsDelete=0
								LEFT JOIN rfd_pms.dbo.Warehouse (NOLOCK) w ON w.WarehouseId = ael.WareHouseID
																	  AND ael.WareHouseType = 1 AND w.Enable=1
								LEFT JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) w1 ON w1.ExpressCompanyID = ael.WareHouseID
																			AND ael.WareHouseType = 2 AND w1.IsDeleted=0
								WHERE (ael.Enable<>0) {0}

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

						WITH    t AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY ael.CreateTime DESC ) AS rowNo ,
												ael.AutoID,
												ael.ExpressCompanyID ,
												ec.CompanyName ,
												p.ProvinceID ,
												p.ProvinceName ,
												c.CityID ,
												c.CityName ,
												a.AreaID ,
												a.AreaName ,
												AreaType = CASE WHEN ael.Enable = 1 THEN ael.AreaType
																ELSE NULL
														   END ,
												EffectAreaType = CASE WHEN ael.Enable IN ( 1, 2, 3 )
																	  THEN ael.EffectAreaType
																	  ELSE NULL
																 END ,
												ael.MerchantID,
												mbi.MerchantName ,
												ael.ProductID,
												si.StatusName AS AuditStatusStr ,
												EnableStr = CASE ael.Enable
															  WHEN 0 THEN '已删除'
															  WHEN 1 THEN '可用'
															  WHEN 2 THEN '待删除'
															  WHEN 3 THEN '新增'
															END ,
												ael.WareHouseID,
												ael.WareHouseType,
												WarehouseName = CASE ael.WareHouseType
																  WHEN 1 THEN w.WarehouseName
																  WHEN 2 THEN w1.CompanyName
																  ELSE NULL
																END
									   FROM     AreaExpressLevel (NOLOCK) ael
												JOIN rfd_pms.dbo.Area (NOLOCK) a ON ael.AreaID = a.AreaID AND a.IsDeleted=0
												JOIN rfd_pms.dbo.City (NOLOCK) c ON a.CityID = c.CityID AND c.IsDeleted=0
												JOIN rfd_pms.dbo.Province (NOLOCK) p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
												JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = ael.ExpressCompanyID AND ec.IsDeleted=0
												JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = ael.MerchantID AND mbi.IsDeleted=0
												JOIN rfd_pms.dbo.StatusInfo (NOLOCK) si ON si.StatusNO = ael.AuditStatus
																			   AND si.StatusTypeNO = 306 AND si.IsDelete=0
												LEFT JOIN rfd_pms.dbo.Warehouse (NOLOCK) w ON w.WarehouseId = ael.WareHouseID
																					  AND ael.WareHouseType = 1 AND w.Enable=1
												LEFT JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) w1 ON w1.ExpressCompanyID = ael.WareHouseID
																					  AND ael.WareHouseType = 2 AND w1.IsDeleted=0
												WHERE (ael.Enable<>0) {0}
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
                sbWhere.Append(" AND ael.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND ael.AreaType=@AreaType ");
                parameters.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND ael.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND ael.AuditStatus=@AuditStatus ");
                parameters.Add(new SqlParameter("@AuditStatus", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND a.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            if (!string.IsNullOrEmpty(wareHouse))
            {
                sbWhere.Append(@" AND EXISTS ( SELECT 1 FROM   @whTable wt WHERE  ael.WareHouseID = wt.HouseID and ael.WareHouseType=wt.HouseType )");
               
            }
            parameters.Add(new SqlParameter("@houseXml", SqlDbType.Xml) { Value = CreateHouseXml(wareHouse).InnerXml });
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
						DECLARE @whTable TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL )
						INSERT  INTO @whTable( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseXml.nodes('/root/id') AS T ( x )
						SELECT   ael.AutoID AS 日志编号,
								ec.CompanyName AS 配送公司,
								p.ProvinceName  AS 省,
								c.CityName  AS 市,
								a.AreaName AS  区,
								生效区域类型 = CASE WHEN ael.Enable = 1 THEN ael.AreaType
												ELSE null
										   END ,
								首次新增区域类型 = CASE WHEN ael.Enable IN ( 1, 2, 3 )
													  THEN ael.EffectAreaType
													  ELSE null
												 END ,
								mbi.MerchantName AS 商家,
								si.StatusName AS 审核状态 ,
								生效状态 = CASE ael.Enable
											  WHEN 0 THEN '已删除'
											  WHEN 1 THEN '可用'
											  WHEN 2 THEN '待删除'
											  WHEN 3 THEN '新增'
											END
					   FROM     AreaExpressLevel (NOLOCK) ael
								JOIN rfd_pms.dbo.Area (NOLOCK) a ON ael.AreaID = a.AreaID AND a.IsDeleted=0
								JOIN rfd_pms.dbo.City (NOLOCK) c ON a.CityID = c.CityID AND c.IsDeleted=0
								JOIN rfd_pms.dbo.Province (NOLOCK) p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
								JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = ael.ExpressCompanyID AND ec.IsDeleted=0
								JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = ael.MerchantID AND mbi.IsDeleted=0
								JOIN rfd_pms.dbo.StatusInfo (NOLOCK) si ON si.StatusNO = ael.AuditStatus
															   AND si.StatusTypeNO = 306 AND si.IsDelete=0
								LEFT JOIN rfd_pms.dbo.Warehouse (NOLOCK) w ON w.WarehouseId = ael.WareHouseID
																	  AND ael.WareHouseType = 1 AND w.Enable=1
								LEFT JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) w1 ON w1.ExpressCompanyID = ael.WareHouseID
																	  AND ael.WareHouseType = 2 AND w1.IsDeleted=0
								WHERE (ael.Enable<>0) {0}";
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
                sbWhere.Append(" AND ael.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND ael.AreaType=@AreaType ");
                parameters.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND ael.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND ael.AuditStatus=@AuditStatus ");
                parameters.Add(new SqlParameter("@AuditStatus", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND a.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            if (!string.IsNullOrEmpty(wareHouse))
            {
                sbWhere.Append(@" AND EXISTS ( SELECT 1 FROM   @whTable wt WHERE  ael.WareHouseID = wt.HouseID and ael.WareHouseType=wt.HouseType )");
                
            }
            parameters.Add(new SqlParameter("@houseXml", SqlDbType.Xml) { Value = CreateHouseXml(wareHouse).InnerXml });
            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string wareHouseType, string merchantId, string productId, string distributionCode)
        {
            sqlStr = @"SELECT  aell.LogID ,
        p.ProvinceName ,
        c.CityName ,
        a.AreaName ,
        ec.CompanyName ,
        mbi.MerchantName ,
        EmployeeName = CASE aell.CreateBy
                         WHEN '0' THEN '系统'
                         ELSE e.EmployeeName
                       END ,
        aell.CreateTime ,
        aell.LogText
FROM    AreaExpressLevelLog (NOLOCK) aell
        JOIN rfd_pms.dbo.Area (NOLOCK) a ON aell.AreaID = a.AreaID
                                    AND a.IsDeleted = 0
        JOIN rfd_pms.dbo.City (NOLOCK) c ON c.CityID = a.CityID
                                    AND c.IsDeleted = 0
        JOIN rfd_pms.dbo.Province (NOLOCK) p ON p.ProvinceID = c.ProvinceID
                                        AND p.IsDeleted = 0
        JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = aell.ExpressCompanyID
                                               AND ec.IsDeleted = 0
        JOIN rfd_pms.dbo.MerchantBaseInfo (NOLOCK) mbi ON mbi.ID = aell.MerchantID
                                                  AND mbi.IsDeleted = 0
        LEFT JOIN rfd_pms.dbo.Warehouse (NOLOCK) w ON w.WarehouseId = aell.WarehouseId
                                              AND w.Enable = 1
        LEFT JOIN rfd_pms.dbo.ExpressCompany (NOLOCK) w1 ON w1.ExpressCompanyID = aell.WarehouseId
                                                    AND w1.IsDeleted = 0
        LEFT JOIN rfd_pms.dbo.employee (NOLOCK) e ON e.EmployeeID = CONVERT(INT, aell.CreateBy)
		WHERE (1=1) {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=@AreaID ");
                parameters.Add(new SqlParameter("@AreaID", SqlDbType.NVarChar,50) { Value = areaId });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND aell.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aell.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(productId))
            {
                sbWhere.Append(" AND aell.ProductID=@ProductID ");
                parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = productId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aell.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            if (!string.IsNullOrEmpty(wareHouse))
            {
                if (int.Parse(wareHouseType) == 2)
                    sbWhere.Append(" AND aell.WareHouseID = '" + wareHouse.Replace("S_", "") + "' AND aell.WareHouseType=2");
                else
                    sbWhere.Append(" AND aell.WareHouseID = '" + wareHouse + "' AND aell.WareHouseType=1");
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, parameters.ToArray());
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        #region cod计算服务
        public DataTable GetAreaType(int expressComapnyId, int merchantId)
        {
            string sql = @"SELECT  ael.AutoId ,
									ael.AreaID ,
									ael.ExpressCompanyID ,
									ael.WareHouseID ,
									ael.WareHouseType ,
									ael.MerchantID ,
									ael.AreaType
							FROM    AreaExpressLevel AS ael WITH(NOLOCK)
							WHERE   ael.Enable IN (1,2)
									AND ael.ExpressCompanyID = @ExpressCompanyID
									AND ael.MerchantID = @MerchantID";
            SqlParameter[] parameters ={
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
										   new SqlParameter("@MerchantID",SqlDbType.Int),
									  };
            parameters[0].Value = expressComapnyId;
            parameters[1].Value = merchantId;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
        #endregion
        public DataTable SearchSecondAreaType(string areaID, string areaType, string stationID, string merchantID, string distributionCode,ref PageInfo pi)
        {
            throw new Exception("sql下未实现");
        }
    }
}
