using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using System.Data.SqlClient;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;
using System.Xml;
using RFD.FMS.MODEL.Enumeration;

namespace RFD.FMS.DAL.FinancialManage
{
    public class MerchantDeliverFeeDao : SqlServerDao, IMerchantDeliverFeeDao
	{
        public string SqlStr { get; set; }

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
                xe.SetAttribute("v", houses[i].Trim());
                xmlDoc.DocumentElement.AppendChild(xe);
            }
            return xmlDoc;
        }

        private string BuildSearchCondition(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode, string tableAlias, ref List<SqlParameter> parameterList)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND {0}.MerchantID=@MerchantID ");
                parameterList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND {0}.StationID=@StationID ");
                parameterList.Add(new SqlParameter("@StationID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND {0}.AreaType=@AreaType ");
                parameterList.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND {0}.Status=@Status ");
                parameterList.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(categoryCode))
            {
                sbWhere.Append(" AND {0}.GoodsCategoryCode in (" + categoryCode + ") ");
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND {0}.DistributionCode=@DistributionCode ");
                parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar) { Value = distributionCode });
            }

            if (!string.IsNullOrEmpty(sortCenterId))
            {
                sbWhere.Append(" AND {0}.ExpressCompanyID IN (" + sortCenterId + ")");
            }

            return string.Format(sbWhere.ToString(), tableAlias);
        }

        public int GetDeliverFeeStat(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            string sql = @"SELECT  count(1)
						FROM    RFD_FMS.dbo.FMS_StationDeliverFee  fsdf
								JOIN RFD_PMS.dbo.MerchantBaseInfo  mbi ON fsdf.MerchantID = mbi.ID
								JOIN RFD_PMS.dbo.ExpressCompany  ec ON ec.ExpressCompanyID = fsdf.StationID
								JOIN RFD_FMS.dbo.StatusCodeInfo  sci ON cast(sci.CodeNo as int) = cast(fsdf.Status as int)
																			 AND sci.CodeType = 'AreaTypeAudit'
						WHERE   ( fsdf.IsDeleted =0 ) {0}";
            string sqlWhere = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdf", ref parameters);

            sql = string.Format(sql, sqlWhere);

            object n = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, this.ToParameters(parameters.ToArray()));
            return DataConvert.ToInt(n, 0);
        }

        public DataTable GetDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode,PageInfo pi)
        {
            SqlStr = @"with t as(SELECT  
                                ROW_NUMBER() OVER ( ORDER BY fsdf.ID DESC ) AS RowNum,
                                fsdf.ID ,
								fsdf.MerchantID ,
								mbi.MerchantName ,
								fsdf.StationID ,
								ec.CompanyName ,
								IsCenterSortStr=CASE fsdf.IsCenterSort WHEN 0 THEN '否' WHEN 1 THEN '是' ELSE '否' END ,
								fsdf.ExpressCompanyID AS SortCenterID,
								ec2.CompanyName AS SortCenterName,
								fsdf.Status ,
								sci.CodeDesc AS AuditStatusStr ,
								fsdf.AreaType ,
								fsdf.BasicDeliverFee,
                                fsdf.DeliverFee,
                                fsdf.IsCod,
                                CASE WHEN fsdf.IsCod=0 THEN '否' ELSE '是' END IsCodStr,
								fsdf.CreateBy ,
								fsdf.CreateTime ,
								fsdf.UpdateBy ,
								fsdf.UpdateTime ,
								fsdf.AuditBy ,
								fsdf.AuditTime,
                                2 as LogType,
                                gc.GoodsCategoryCode,
                                gc.GoodsCategoryName
						FROM    RFD_FMS.dbo.FMS_StationDeliverFee AS fsdf ( NOLOCK )
								JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON fsdf.MerchantID = mbi.ID
								JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON ec.ExpressCompanyID = fsdf.StationID
								JOIN StatusCodeInfo AS sci ( NOLOCK ) ON sci.CodeNo = fsdf.Status
																			 AND sci.CodeType = 'AreaTypeAudit'
								LEFT JOIN RFD_PMS.dbo.ExpressCompany AS ec2 ( NOLOCK ) ON ec2.ExpressCompanyID = fsdf.ExpressCompanyID
																				  AND ec2.CompanyFlag = 1
                                LEFT JOIN RFD_PMS.dbo.GoodsCategory gc( NOLOCK ) on gc.GoodsCategoryCode=fsdf.GoodsCategoryCode
						WHERE   ( ISNULL(fsdf.IsDeleted,0) = 0 ) {0}) select * from t where RowNum between @RowStr and @RowEnd";

            string sqlWhere = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdf", ref parameters);

            List<SqlParameter> parametersTmp = new List<SqlParameter>();
            parametersTmp.Add(new SqlParameter("@RowStr", SqlDbType.Int) { Value = pi.CurrentPageStartRowNum });
            parametersTmp.Add(new SqlParameter("@RowEnd", SqlDbType.Int) { Value = pi.CurrentPageEndRowNum });

            parameters.AddRange(parametersTmp);

            SqlStr = string.Format(SqlStr, sqlWhere);
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray()).Tables[0];
        }

        public int GetDeliverFeeWaitStat(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public DataTable GetDeliverFeeWaitList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode, PageInfo pi)
        {
            throw new Exception("sql中没有实现待生效");
        }

        /// <summary>
        /// 添加站点配送费信息
        /// </summary>
        /// <param name="model">站点实体</param>
        /// <returns></returns>
        public bool AddStationDeliverFee(FMS_StationDeliverFee model, out int id)
        {
            if (string.IsNullOrEmpty(model.GoodsCategoryCode))
            {
                SqlStr = @"SET @id=0
						IF NOT EXISTS(SELECT 1 FROM RFD_FMS.dbo.FMS_StationDeliverFee AS fsdf(NOLOCK) 
												WHERE fsdf.MerchantID=@MerchantID AND fsdf.StationID=@StationID 
													AND fsdf.AreaType=@AreaType AND fsdf.ExpressCompanyID=@ExpressCompanyID
                                                    AND fsdf.IsDeleted=0 AND fsdf.DistributionCode=@DistributionCode)
						BEGIN
							INSERT INTO RFD_FMS.dbo.FMS_StationDeliverFee( MerchantID ,StationID ,BasicDeliverFee ,UpdateBy ,UpdateTime ,
															UpdateCode ,AuditBy ,AuditTime ,AuditCode ,AuditResult ,
															Status ,ExpressCompanyID ,IsCenterSort ,AreaType ,CreateBy ,CreateTime ,
                                                            IsDeleted,DistributionCode,IsChange,GoodsCategoryCode,IsCod,DeliverFee )
								VALUES  ( @MerchantID ,@StationID ,@BasicDeliverFee ,@UpdateBy ,GETDATE() ,
										@UpdateCode ,@AuditBy ,GETDATE() ,@AuditCode ,@AuditResult ,@Status ,@ExpressCompanyID ,
										@IsCenterSort ,@AreaType ,@CreateBy ,GETDATE() ,0,@DistributionCode,@IsChange,@GoodsCategoryCode,@IsCod,@DeliverFee  )
							SET @id=@@identity
						END
						ELSE
						BEGIN
							SET @id=-1
						END ";
            }
            else
            {
                SqlStr = @"SET @id=0
							IF NOT EXISTS(SELECT 1 FROM RFD_FMS.dbo.FMS_StationDeliverFee AS fsdf(NOLOCK) 
															WHERE fsdf.MerchantID=@MerchantID AND fsdf.StationID=@StationID 
															AND fsdf.AreaType=@AreaType AND fsdf.ExpressCompanyID=@ExpressCompanyID 
                                                            AND fsdf.IsDeleted=0 AND fsdf.DistributionCode=@DistributionCode 
                                                            AND GoodsCategoryCode=@GoodsCategoryCode)
							BEGIN
								INSERT INTO RFD_FMS.dbo.FMS_StationDeliverFee( MerchantID ,StationID ,BasicDeliverFee ,UpdateBy ,UpdateTime ,
																UpdateCode ,AuditBy ,AuditTime ,AuditCode ,AuditResult ,
																Status ,ExpressCompanyID ,IsCenterSort ,AreaType ,CreateBy ,CreateTime ,
                                                                IsDeleted ,DistributionCode,IsChange,GoodsCategoryCode,IsCod,DeliverFee)
									VALUES  ( @MerchantID ,@StationID ,@BasicDeliverFee ,@UpdateBy ,GETDATE() ,
											@UpdateCode ,@AuditBy ,GETDATE() ,@AuditCode ,@AuditResult ,@Status ,@ExpressCompanyID ,
											@IsCenterSort ,@AreaType ,@CreateBy ,GETDATE() ,0,@DistributionCode,@IsChange,@GoodsCategoryCode,@IsCod,@DeliverFee)
								SET @id=@@identity
							END	
							ELSE
							BEGIN
								SET @id=-1
							END  ";
            }

            SqlParameter[] parameters ={
										new SqlParameter("@id", SqlDbType.Int),
										new SqlParameter("@MerchantID", SqlDbType.Int),
										new SqlParameter("@StationID", SqlDbType.Int),
										new SqlParameter("@BasicDeliverFee", SqlDbType.NVarChar,150),
										new SqlParameter("@UpdateBy", SqlDbType.Int),
										new SqlParameter("@UpdateTime", SqlDbType.DateTime),
										new SqlParameter("@UpdateCode", SqlDbType.NVarChar,20),
										new SqlParameter("@AuditBy", SqlDbType.Int),
										new SqlParameter("@AuditTime", SqlDbType.DateTime),
										new SqlParameter("@AuditCode", SqlDbType.NVarChar,20),
										new SqlParameter("@AuditResult", SqlDbType.Int) ,
										new SqlParameter("@Status", SqlDbType.Int) ,
										new SqlParameter("@ExpressCompanyID", SqlDbType.Int) ,
										new SqlParameter("@IsCenterSort", SqlDbType.Int) ,
										new SqlParameter("@AreaType", SqlDbType.Int) ,
										new SqlParameter("@CreateBy", SqlDbType.Int) ,
                                        new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50) ,
                                        new SqlParameter("@IsChange", SqlDbType.Bit) ,
                                        new SqlParameter("@GoodsCategoryCode", SqlDbType.NVarChar,10) ,
                                        new SqlParameter("@IsCod", SqlDbType.Int) ,
                                        new SqlParameter("@DeliverFee", SqlDbType.NVarChar,150) ,
									  };
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = model.MerchantID;
            parameters[2].Value = model.StationID;
            parameters[3].Value = model.BasicDeliverFee;
            parameters[4].Value = model.UpdateUser;
            parameters[5].Value = model.UpdateTime;
            parameters[6].Value = model.UpdateUserCode;
            parameters[7].Value = model.AuditBy;
            parameters[8].Value = model.AuditTime;
            parameters[9].Value = model.AuditCode;
            parameters[10].Value = model.AuditResult;
            parameters[11].Value = model.Status;
            parameters[12].Value = model.ExpressCompanyID;
            parameters[13].Value = model.IsCenterSort;
            parameters[14].Value = model.AreaType;
            parameters[15].Value = model.CreateUser;
            parameters[16].Value = model.DistributionCode;
            parameters[17].Value = true;
            parameters[18].Value = model.GoodsCategoryCode;
            parameters[19].Value = model.IsCod;
            parameters[20].Value = model.DeliverFee;
            bool flag = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;

            id = int.Parse(parameters[0].Value.ToString());

            return flag;
        }

        public bool AddWaitStationDeliverFee(FMS_StationDeliverFee model, out int id)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public bool UpdateDeliverFee(FMS_StationDeliverFee model)
        {
            SqlStr = @"UPDATE  RFD_FMS.dbo.FMS_StationDeliverFee
						SET     BasicDeliverFee = @BasicDeliverFee,DeliverFee = @DeliverFee,IsCod = @IsCod,
                                UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange
						WHERE   ID = @ID
								AND IsDeleted = 0";
            SqlParameter[] parameters ={
										new SqlParameter("@ID", SqlDbType.Int),
										new SqlParameter("@BasicDeliverFee", SqlDbType.NVarChar,150),
										new SqlParameter("@UpdateBy", SqlDbType.Int),
                                        new SqlParameter("@IsChange", SqlDbType.Bit),
                                        new SqlParameter("@DeliverFee", SqlDbType.NVarChar,150),
										new SqlParameter("@IsCod", SqlDbType.Int),
									  };

            parameters[0].Value = model.ID;
            parameters[1].Value = model.BasicDeliverFee;
            parameters[2].Value = model.UpdateUser;
            parameters[3].Value = true;
            parameters[4].Value = model.DeliverFee;
            parameters[5].Value = model.IsCod;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateWaitDeliverFee(FMS_StationDeliverFee model)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public DataTable GetDeliverFeeById(int id)
        {
            SqlStr = @"SELECT  fsdf.MerchantID ,
								fsdf.StationID ,
                                fsdf.IsCod ,
								fsdf.BasicDeliverFee ,
                                fsdf.DeliverFee ,
								fsdf.UpdateBy ,
								fsdf.UpdateTime ,
								fsdf.UpdateCode ,
								fsdf.AuditBy ,
								fsdf.AuditTime ,
								fsdf.AuditCode ,
								fsdf.AuditResult ,
								fsdf.Status ,
								fsdf.ExpressCompanyID ,
								fsdf.IsCenterSort ,
								fsdf.AreaType ,
								fsdf.CreateBy,
								ec.CompanyName,
                                fsdf.GoodsCategoryCode,
                                mbi.MerchantName
						FROM    RFD_FMS.dbo.FMS_StationDeliverFee AS fsdf ( NOLOCK )
						JOIN	RFD_PMS.dbo.ExpressCompany AS ec (nolock) on fsdf.StationID=ec.ExpressCompanyID
                        JOIN	RFD_PMS.dbo.MerchantBaseInfo AS mbi (nolock) on mbi.ID=fsdf.MerchantID
						WHERE   fsdf.IsDeleted = 0 AND fsdf.ID=@ID";
            SqlParameter[] parameters ={
										new SqlParameter("@ID", SqlDbType.Int),
									  };
            parameters[0].Value = id;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters).Tables[0];
        }

        public DataTable GetWaitDeliverFeeById(string id)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public bool DeleteDeliverFee(int id, int updateBy)
        {
            SqlStr = @"UPDATE  RFD_FMS.dbo.FMS_StationDeliverFee
						SET     IsDeleted = 1,UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange
						WHERE   ID = @ID
								AND IsDeleted = 0";
            SqlParameter[] parameters ={
										new SqlParameter("@ID", SqlDbType.Int),
										new SqlParameter("@UpdateBy", SqlDbType.Int),
                                        new SqlParameter("@IsChange", SqlDbType.Bit)
									  };
            parameters[0].Value = id;
            parameters[1].Value = updateBy;
            parameters[2].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateDeliverFeeStatus(int id, int status, int auditBy)
        {
            SqlStr = @"UPDATE  RFD_FMS.dbo.FMS_StationDeliverFee
						SET Status=@Status,AuditBy=@AuditBy,AuditTime=GETDATE(),IsChange=@IsChange
						WHERE   ID = @ID
								AND isnull(IsDeleted,0) = 0";
            SqlParameter[] parameters ={
										new SqlParameter("@ID", SqlDbType.Int),
										new SqlParameter("@Status", SqlDbType.Char,5),
										new SqlParameter("@AuditBy", SqlDbType.Int),
                                        new SqlParameter("@IsChange", SqlDbType.Bit)
									  };

            parameters[0].Value = id;
            parameters[1].Value = status;
            parameters[2].Value = auditBy;
            parameters[3].Value = 1;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateWaitDeliverFeeStatus(string id, int status, int auditBy)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public int GetWaitDeliverFeeyFeeId(int feeid)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public DataSet GetExportData()
        {
            SqlStr = @"
with t as(
select ExpressCompanyID,CompanyName,CompanyAllName from RFD_PMS.dbo.ExpressCompany(nolock) where CompanyFlag in (1,2,3) and DistributionCode<>'rfd' and IsDeleted=0
union all
select ExpressCompanyID,CompanyName,CompanyAllName from RFD_PMS.dbo.ExpressCompany(nolock) where CompanyFlag=2 and DistributionCode='rfd' and IsDeleted=0)
select * from t;

SELECT   ExpressCompanyID,
		CompanyName
FROM     RFD_PMS.dbo.ExpressCompany (NOLOCK) ec
WHERE    IsDeleted = 0 AND CompanyFlag = 1;

SELECT sci.CodeNo
FROM   StatusCodeInfo sci(NOLOCK)
WHERE  sci.CodeType = 'AreaType'
	   AND sci.[Enabled] = 1;

SELECT  ID ,
		MerchantName
FROM    RFD_PMS.dbo.MerchantBaseInfo (NOLOCK) mbi
WHERE   IsDeleted = 0;
";
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr);
        }

        public DataTable GetDeliverFeeEffect()
        {
            throw new Exception("sql中没有实现待生效");
        }

        public bool UpdateToEffect(FMS_StationDeliverFee model)
        {
            string sql = @"
UPDATE FMS_StationDeliverFee
   SET MerchantID=@MerchantID
,StationID=@StationID
,BasicDeliverFee=@BasicDeliverFee
,UpdateBy=@UpdateBy
,UpdateTime=@UpdateTime
,UpdateCode=@UpdateCode
,AuditBy=@AuditBy
,AuditTime=@AuditTime
,AuditCode=@AuditCode
,Status=@Status
,ExpressCompanyID=@ExpressCompanyID
,IsCenterSort=@IsCenterSort
,AreaType=@AreaType
,IsDeleted=0
,IsChange =1 
,GoodsCategoryCode=@GoodsCategoryCode
,DeliverFee=@DeliverFee
,IsCod=@IsCod
	WHERE ID=@ID
";
            SqlParameter[] parameters ={
                                           new SqlParameter("@MerchantID",SqlDbType.Int),
                                            new SqlParameter("@StationID",SqlDbType.Int),
                                            new SqlParameter("@BasicDeliverFee",SqlDbType.NVarChar,150),
                                            new SqlParameter("@UpdateBy",SqlDbType.Int),
                                            new SqlParameter("@UpdateTime",SqlDbType.DateTime),
                                            new SqlParameter("@UpdateCode",SqlDbType.NVarChar,20),
                                            new SqlParameter("@AuditBy",SqlDbType.Int),
                                            new SqlParameter("@AuditTime",SqlDbType.DateTime),
                                            new SqlParameter("@AuditCode",SqlDbType.NVarChar,20),
                                            new SqlParameter("@Status",SqlDbType.Char,5),
                                            new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
                                            new SqlParameter("@IsCenterSort",SqlDbType.Int),
                                            new SqlParameter("@AreaType",SqlDbType.Int),
                                            new SqlParameter("@ID",SqlDbType.Int),
                                            new SqlParameter("@GoodsCategoryCode",SqlDbType.NVarChar,10),
                                            new SqlParameter("@DeliverFee",SqlDbType.NVarChar),
                                            new SqlParameter("@IsCod",SqlDbType.Int),
                                      };
            parameters[0].Value = model.MerchantID;
            parameters[1].Value = model.StationID;
            parameters[2].Value = model.BasicDeliverFee;
            parameters[3].Value = model.UpdateUser;
            parameters[4].Value = model.UpdateTime;
            parameters[5].Value = model.UpdateUserCode;
            parameters[6].Value = model.AuditBy;
            parameters[7].Value = model.AuditTime;
            parameters[8].Value = model.AuditCode;
            parameters[9].Value = (int)model.Status;
            parameters[10].Value = model.ExpressCompanyID;
            parameters[11].Value = model.IsCenterSort;
            parameters[12].Value = model.AreaType;
            parameters[13].Value = model.ID;
            parameters[14].Value = model.GoodsCategoryCode;
            parameters[15].Value = model.DeliverFee;
            parameters[16].Value = model.IsCod;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public bool DeleteWaitStationDeliverFee(string effectKid)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public DataTable GetBasicDeliverFeeByCondition(int merchantId, string warehouseId, int areaType, int isCategory, string waybillCategory, string distributionCode)
        {
            string sql = @"SELECT fsdf.MerchantID,fsdf.StationID,fsdf.GoodsCategoryCode,fsdf.ExpressCompanyID,
                                    fsdf.BasicDeliverFee,fsdf.DeliverFee,fsdf.IsCod
                            FROM RFD_FMS.dbo.FMS_StationDeliverFee fsdf(NOLOCK) 
                            WHERE fsdf.Status IN ('1') AND IsDeleted=0 {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameterList = new List<SqlParameter>();

            if (merchantId > 0)
            {
                sbWhere.Append(" AND fsdf.MerchantID= @MerchantID");
                parameterList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(warehouseId))
            {
                sbWhere.Append(" AND fsdf.ExpressCompanyID= @ExpressCompanyID");
                parameterList.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = warehouseId });
            }

            if (areaType > 0)
            {
                sbWhere.Append(" AND fsdf.AreaType= @AreaType");
                parameterList.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND fsdf.DistributionCode= @DistributionCode");
                parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50) { Value = distributionCode });
            }

            if (isCategory == 1 && !string.IsNullOrEmpty(waybillCategory))
            {
                sbWhere.Append(" AND fsdf.GoodsCategoryCode= @GoodsCategoryCode");
                parameterList.Add(new SqlParameter("@GoodsCategoryCode", SqlDbType.NVarChar,10) { Value = waybillCategory });
            }

            sql = string.Format(sql,sbWhere.ToString());
            DataSet ds= SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray());

            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable GetExportDeliverFeeWaitList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            throw new NotImplementedException();
        }

        public DataTable GetExportDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            throw new NotImplementedException();
        }
	}
}
