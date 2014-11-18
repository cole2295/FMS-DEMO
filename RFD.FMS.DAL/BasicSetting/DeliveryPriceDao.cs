using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using System.Data.SqlClient;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.BasicSetting
{
	/// <summary>
	/// COD价格维护
	/// </summary>
    public class DeliveryPriceDao : SqlServerDao, IDeliveryPriceDao
	{
        private string strSql = "";

        public DataTable GetMerchant(string disCode)
        {
            strSql = @"SELECT m.ID,m.MerchantName
 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=@DistributionCode
ORDER BY m.CreatTime";
            SqlParameter[] parameters ={
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
                                      };
            parameters[0].Value = disCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public DataTable GetDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId, string distributionCode, int IsCod, PageInfo pi, bool isPage)
        {
            #region sql
            StringBuilder str = new StringBuilder();
            if (isPage)
                str.Append(@"
SELECT @records=COUNT(1)
FROM    FMS_CODLine (NOLOCK) codl
		JOIN StatusCodeInfo sci ( NOLOCK ) ON sci.CodeNo = codl.LineStatus
											  AND sci.CodeType = 'AreaTypeLine'
		JOIN StatusCodeInfo sci1 ( NOLOCK ) ON sci1.CodeNo = codl.AuditStatus
											   AND sci1.CodeType = 'AreaTypeAudit'
		JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = codl.ExpressCompanyID
		LEFT JOIN RFD_PMS.dbo.Warehouse w ( NOLOCK ) ON w.WarehouseId = codl.WarehouseId
		LEFT JOIN RFD_PMS.dbo.ExpressCompany w1 ( NOLOCK ) ON w1.ExpressCompanyID = codl.WarehouseId
												  AND w1.IsDeleted = 0
												  AND w1.CompanyFlag = 1
		JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID=codl.MerchantID
WHERE   ( codl.DeleteFlag = 0 ) {0}

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
SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1

;WITH t AS (
");

            str.Append(@"SELECT  ROW_NUMBER() OVER ( ORDER BY codl.CODLineNO DESC ) AS 序号 ,
							    codl.LineID ,
								codl.CODLineNO ,
								codl.ExpressCompanyID ,
								ec.CompanyName ,
								codl.IsEchelon ,
								IsEchelonStr = CASE WHEN codl.IsEchelon = 1 THEN '是'
													ELSE '否'
											   END ,
								codl.WareHouseID ,
								WarehouseName = CASE codl.WareHouseType
												  WHEN 1 THEN w.WarehouseName
												  WHEN 2 THEN w1.CompanyName
												  ELSE ''
												END ,
								codl.AreaType ,
								IsCODStr=CASE WHEN codl.IsCOD=0 THEN '否' else '是' end,
								codl.PriceFormula ,
                                codl.Formula ,
								codl.LineStatus ,
								sci.CodeDesc AS LineStatusStr ,
								codl.AuditStatus ,
								sci1.CodeDesc AS AuditStatusStr ,
								codl.AuditBy ,
								codl.AuditTime ,
								codl.CreateBy ,
								codl.CreateTime ,
								codl.UpdateBy ,
								codl.UpdateTime ,
								codl.DeleteFlag ,
								mbi.MerchantName
						FROM    FMS_CODLine (NOLOCK) codl
								JOIN StatusCodeInfo sci ( NOLOCK ) ON sci.CodeNo = codl.LineStatus
																	  AND sci.CodeType = 'AreaTypeLine'
								JOIN StatusCodeInfo sci1 ( NOLOCK ) ON sci1.CodeNo = codl.AuditStatus
																	   AND sci1.CodeType = 'AreaTypeAudit'
								JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = codl.ExpressCompanyID
								LEFT JOIN RFD_PMS.dbo.Warehouse w ( NOLOCK ) ON w.WarehouseId = codl.WarehouseId
								LEFT JOIN RFD_PMS.dbo.ExpressCompany w1 ( NOLOCK ) ON w1.ExpressCompanyID = codl.WarehouseId
																		  AND w1.IsDeleted = 0
																		  AND w1.CompanyFlag = 1
								JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID=codl.MerchantID
						WHERE   ( codl.DeleteFlag = 0 ) {0}");

            if (isPage)
                str.Append(@"
)
SELECT * FROM t WHERE 序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1 ; 
");
            #endregion
            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@records", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pages", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value=pi.PageSize });
            parameters.Add(new SqlParameter("@pageNo", SqlDbType.Int) { Value = pi.CurrentPageIndex });

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND codl.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(lineStatus))
            {
                sbWhere.Append(" AND codl.LineStatus=@LineStatus ");
                parameters.Add(new SqlParameter("@LineStatus", SqlDbType.Int) { Value = lineStatus });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND codl.AuditStatus=@AuditStatus ");
                parameters.Add(new SqlParameter("@AuditStatus", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND codl.AreaType=@AreaType ");
                parameters.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(wareHouse))
            {
                sbWhere.Append(" AND codl.WareHouseID=@WareHouseID ");
                parameters.Add(new SqlParameter("@WareHouseID", SqlDbType.NVarChar, 20) { Value = wareHouse });
            }

            if (!string.IsNullOrEmpty(wareHouseType))
            {
                sbWhere.Append(" AND codl.WareHouseType=@WareHouseType ");
                parameters.Add(new SqlParameter("@WareHouseType", SqlDbType.Int) { Value = wareHouseType });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND codl.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND codl.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar,50) { Value = distributionCode });
            }

            if (IsCod>-1)
            {
                sbWhere.Append(" AND codl.IsCOD=@IsCOD ");
                parameters.Add(new SqlParameter("@IsCOD", SqlDbType.Int) { Value = IsCod });
            }

            strSql = string.Format(str.ToString(), sbWhere.ToString());
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            if (isPage)
            {
                pi.ItemCount = (int)parameters[0].Value;
                pi.PageCount = (int)parameters[1].Value;
            }
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable GetListByCodLineNo(string codLineNo)
        {
            strSql = @"SELECT  codl.LineID ,
								codl.CODLineNO ,
								codl.ExpressCompanyID ,
								ec.CompanyName ,
								codl.IsEchelon ,
								WareHouseID = CASE WareHouseType
												WHEN 1 THEN codl.WareHouseID
												WHEN 2 THEN 'S_' + codl.WareHouseID
												ELSE ''
											  END ,
								codl.AreaType ,
								codl.IsCOD,
								codl.PriceFormula ,
                                codl.Formula ,
								codl.LineStatus ,
								codl.AuditStatus ,
								codl.MerchantID ,
								codl.ProductID,
                                codl.DistributionCode,
                                mbi.MerchantName
						FROM    FMS_CODLine (NOLOCK) codl
								JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = codl.ExpressCompanyID
                                JOIN RFD_PMS.dbo.MerchantBaseInfo mbi ( NOLOCK ) ON mbi.ID = codl.MerchantID
						WHERE   ( DeleteFlag = 0 ) and codl.CODLineNO=@CODLineNO";
            SqlParameter[] parameters = {
                                            new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
                                       };
            parameters[0].Value = codLineNo;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public int AddDeliveryPrice(FMS_CODLine codLine, string cODLineNO)
        {
            if (string.IsNullOrEmpty(codLine.WareHouseID))
            {
                strSql = @"SET @Msg=-1
						IF NOT EXISTS (SELECT 1 From FMS_CODLine (nolock) 
								WHERE ExpressCompanyID=@ExpressCompanyID AND MerchantID=@MerchantID AND AreaType=@AreaType AND DeleteFlag=0 AND DistributionCode=@DistributionCode)
						BEGIN
							{0}
						END
						ELSE
						BEGIN
							SET @Msg=0
						END";
            }
            else
            {
                strSql = @"SET @Msg=-1
						IF NOT EXISTS (SELECT 1 FROM FMS_CODLine (nolock) 
								WHERE ExpressCompanyID=@ExpressCompanyID AND MerchantID=@MerchantID AND AreaType=@AreaType
									AND DeleteFlag=0 AND WareHouseID=@WareHouseID AND IsEchelon=@IsEchelon and DistributionCode=@DistributionCode)
						BEGIN
							{0}
						END
						ELSE
						BEGIN
							SET @Msg=0
						END";
            }
            string sqlInsert = @"INSERT INTO FMS_CODLine
										( CODLineNO ,
										  ExpressCompanyID ,
										  IsEchelon ,
										  WareHouseID ,
										  AreaType ,
										  PriceFormula ,
										  LineStatus ,
										  AuditStatus ,
										  AuditBy ,
										  AuditTime ,
										  CreateBy ,
										  CreateTime ,
										  UpdateBy ,
										  UpdateTime,
										  WareHouseType,
										  MerchantID,
										  ProductID,
                                          DistributionCode,
                                          IsCOD,
                                          Formula,
                                          IsChange
										)
								VALUES  ( @CODLineNO ,
										  @ExpressCompanyID ,
										  @IsEchelon ,
										  @WareHouseID ,
										  @AreaType ,
										  @PriceFormula ,
										  @LineStatus ,
										  @AuditStatus ,
										  '' ,
										  GETDATE() ,
										  @CreateBy ,
										  GETDATE() ,
										  '' ,
										  GETDATE(),
										  @WareHouseType,
										  @MerchantID,
										  @ProductID,
                                          @DistributionCode	,
                                          @IsCOD,
                                          @Formula,
                                          @IsChange						
										)
								SET @Msg=1";
            strSql = string.Format(strSql, sqlInsert);
            SqlParameter[] parameters = {
			                          		new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
											new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
											new SqlParameter("@IsEchelon",SqlDbType.SmallInt),
											new SqlParameter("@WareHouseID",SqlDbType.NVarChar,20),
											new SqlParameter("@AreaType",SqlDbType.SmallInt),
											new SqlParameter("@PriceFormula",SqlDbType.NVarChar,200),
											new SqlParameter("@LineStatus",SqlDbType.SmallInt),
											new SqlParameter("@AuditStatus",SqlDbType.SmallInt),
											new SqlParameter("@CreateBy",SqlDbType.NVarChar,40),
											new SqlParameter("@Msg",SqlDbType.SmallInt),
											new SqlParameter("@WareHouseType",SqlDbType.Int),
											new SqlParameter("@MerchantID",SqlDbType.Int),
											new SqlParameter("@ProductID",SqlDbType.Int),
                                            new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                            new SqlParameter("@IsCOD",SqlDbType.Int),
                                            new SqlParameter("@Formula",SqlDbType.NVarChar,150),
                                            new SqlParameter("@IsChange",SqlDbType.Bit)
										};

            parameters[0].Value = cODLineNO;
            parameters[1].Value = codLine.ExpressCompanyID;
            parameters[2].Value = codLine.IsEchelon;
            parameters[3].Value = codLine.WareHouseID;
            parameters[4].Value = codLine.AreaType;
            parameters[5].Value = codLine.PriceFormula;
            parameters[6].Value = codLine.LineStatus;
            parameters[7].Value = codLine.AuditStatus;
            parameters[8].Value = codLine.CreateBy;
            parameters[9].Direction = ParameterDirection.Output;
            parameters[10].Value = codLine.WareHouseType;
            parameters[11].Value = codLine.MerchantID;
            parameters[12].Value = codLine.ProductID;
            parameters[13].Value = codLine.DistributionCode;
            parameters[14].Value = codLine.IsCOD;
            parameters[15].Value = codLine.Formula;
            parameters[16].Value = true;

            int n = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);

            return int.Parse(parameters[9].Value.ToString());
        }

        public bool UpdateDeliveryPrice(FMS_CODLine codLine)
        {
            strSql = @"UPDATE  FMS_CODLine
							SET     ExpressCompanyID = @ExpressCompanyID ,
									IsEchelon = @IsEchelon ,
									WareHouseID = @WareHouseID ,
									WareHouseType=@WareHouseType,
									MerchantID=@MerchantID,
									ProductID=@ProductID,
									AreaType = @AreaType ,
                                    IsCOD = @IsCOD ,
									PriceFormula = @PriceFormula ,
                                    Formula = @Formula ,
									LineStatus = @LineStatus ,
									UpdateBy = @UpdateBy ,
									UpdateTime = GETDATE(),
									AuditStatus=2,
									AuditBy='',
									AuditTime=GETDATE(),
                                    IsChange=@IsChange
							WHERE   CODLineNO = @CODLineNO AND DeleteFlag=0";
            SqlParameter[] parameters ={
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
										   new SqlParameter("@IsEchelon",SqlDbType.Int),
										   new SqlParameter("@WareHouseID",SqlDbType.NVarChar,40),
										   new SqlParameter("@AreaType",SqlDbType.Int),
                                           new SqlParameter("@IsCOD",SqlDbType.Int),
										   new SqlParameter("@PriceFormula",SqlDbType.NVarChar,150),
                                           new SqlParameter("@Formula",SqlDbType.NVarChar,150),
										   new SqlParameter("@LineStatus",SqlDbType.Int),
										   new SqlParameter("@UpdateBy",SqlDbType.NVarChar,100),
										   new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
										   new SqlParameter("@WareHouseType",SqlDbType.Int),
										   new SqlParameter("@MerchantID",SqlDbType.Int),
										   new SqlParameter("@ProductID",SqlDbType.Int),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
            parameters[0].Value = codLine.ExpressCompanyID;
            parameters[1].Value = codLine.IsEchelon;
            parameters[2].Value = codLine.WareHouseID;
            parameters[3].Value = codLine.AreaType;
            parameters[4].Value = codLine.IsCOD;
            parameters[5].Value = codLine.PriceFormula;
            parameters[6].Value = codLine.Formula;
            parameters[7].Value = codLine.LineStatus;
            parameters[8].Value = codLine.UpdateBy;
            parameters[9].Value = codLine.CODLineNO;
            parameters[10].Value = codLine.WareHouseType;
            parameters[11].Value = codLine.MerchantID;
            parameters[12].Value = codLine.ProductID;
            parameters[13].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public bool DeleteDeliveryPrice(string cODLineNO, string updateBy)
        {
            strSql = @"UPDATE FMS_CODLine SET DeleteFlag = 1,UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange
												WHERE CODLineNO=@CODLineNO";
            SqlParameter[] parameters =
            {
				new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
				new SqlParameter("@UpdateBy",SqlDbType.NVarChar,100),
                new SqlParameter("@IsChange",SqlDbType.Bit)
			};

            parameters[0].Value = cODLineNO;
            parameters[1].Value = updateBy;
            parameters[2].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public bool UpdateDeliveryPriceAuditStatus(string cODLineNO, string auditBy, int auditStatus)
        {
            strSql = @"UPDATE FMS_CODLine SET AuditStatus=@AuditStatus,AuditBy=@AuditBy,AuditTime=GETDATE(),IsChange=@IsChange
												WHERE CODLineNO=@CODLineNO AND DeleteFlag=0";
            SqlParameter[] parameters ={
										   new SqlParameter("@AuditStatus",SqlDbType.Int),
										   new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
										   new SqlParameter("@AuditBy",SqlDbType.NVarChar,100),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };

            parameters[0].Value = auditStatus;
            parameters[1].Value = cODLineNO;
            parameters[2].Value = auditBy;
            parameters[3].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        /// <summary>
        /// 删除日志增加
        /// </summary>
        /// <returns></returns>
        public bool AddDeliveryPriceLog(string codLineNo, string createBy, string logText, int logType)
        {
            strSql = @"INSERT INTO RFD_FMS.dbo.FMS_CODOperatorLog
							   (PK_NO
							   ,CreateBy
							   ,CreateTime
							   ,LogText
							   ,LogType
                               ,IsChange)
						 VALUES
							   (@PK_NO
							   ,@CreateBy
							   ,GETDATE()
							   ,@LogText
							   ,@LogType
                               ,@IsChange)";
            SqlParameter[] parameters ={
										   new SqlParameter("@PK_NO",SqlDbType.NVarChar,20),
										   new SqlParameter("@CreateBy",SqlDbType.NVarChar,40),
										   new SqlParameter("@LogText",SqlDbType.NVarChar,250),
										   new SqlParameter("@LogType",SqlDbType.Int),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
            parameters[0].Value = codLineNo;
            parameters[1].Value = createBy;
            parameters[2].Value = logText;
            parameters[3].Value = logType;
            parameters[4].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public DataTable GetDeliveryPriceHistoryList(string lineNoXml)
        {
            strSql = @"DECLARE @Sql VARCHAR(max) 
						SELECT T.x.value('./@v','nvarchar(20)') AS LineID INTO #FMS_CODLineID_TMP FROM @LineID.nodes('/root/id') AS T(x)
						SET @Sql = 'SELECT CODLineNO as 线路编号' 
						SELECT  @Sql = @Sql
								+ ',MAX(CASE CONVERT(varchar(100), Dateadd(""m"",-1,CONVERT(DATETIME,BakYear+BakMonth+''01'')),112) WHEN '''
								+ BakYearMonth
								+ ''' THEN PriceFormula ELSE '''' END) AS [ '
								+ BakYearMonth + '] '
                                + ',MAX(CASE CONVERT(varchar(100), Dateadd(""m"",-1,CONVERT(DATETIME,BakYear+BakMonth+''01'')),112) WHEN '''
								+ BakYearMonth
								+ ''' THEN Formula ELSE '''' END) AS [ '
								+ BakYearMonth + '非] '
						FROM    ( SELECT   DISTINCT
											CONVERT(varchar(100), Dateadd(""m"",-1,CONVERT(DATETIME,BakYear+BakMonth+'01')),112) AS BakYearMonth
								  FROM      FMS_CODLineHistory (NOLOCK)
								) AS a 
						SELECT  @Sql = @Sql
								+ ' FROM FMS_CODLineHistory (NOLOCK) {0} GROUP BY CODLineNO' 
						EXEC(@Sql) 
						drop table #FMS_CODLineID_TMP";
            SqlParameter[] parameters ={
										new SqlParameter("@LineID",SqlDbType.Xml)
									  };
            parameters[0].Value = lineNoXml;
            if (!string.IsNullOrEmpty(lineNoXml))
                strSql = string.Format(strSql, "WHERE EXISTS (SELECT 1 FROM #FMS_CODLineID_TMP (NOLOCK) tmp WHERE tmp.LineID=CODLineNO)");
            else
                strSql = string.Format(strSql, "");
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public DataTable GetDeliveryPriceLog(string lineNo, string dateStr, string dateEnd, string distributionCode)
        {
            strSql = @"SELECT fcl.LogID,
							   fcl.PK_NO,
							   fcl.CreateBy,
							   fcl.CreateTime,
							   fcl.LogText
						FROM   RFD_FMS.dbo.FMS_CODOperatorLog fcl(NOLOCK)
                        JOIN FMS_CODLine fcl1(NOLOCK) ON fcl.PK_NO=fcl1.CODLineNO
                        WHERE (1=1)
                        {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(lineNo))
            {
                sbWhere.Append(" AND fcl.PK_NO=@PK_NO ");
                parameters.Add(new SqlParameter("@PK_NO", SqlDbType.NVarChar, 20) { Value = lineNo });
            }

            if (!string.IsNullOrEmpty(dateStr))
            {
                sbWhere.Append(" AND fcl.CreateTime>=@dateStr ");
                parameters.Add(new SqlParameter("@dateStr", SqlDbType.Date) { Value = dateStr });
            }

            if (!string.IsNullOrEmpty(dateEnd))
            {
                sbWhere.Append(" AND fcl.CreateTime<=@dateEnd ");
                parameters.Add(new SqlParameter("@dateEnd", SqlDbType.Date) { Value = dateEnd });
            }

            sbWhere.Append(" AND fcl.LogType=1 ");

            strSql = string.Format(strSql, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray()).Tables[0];
        }

        public int AddEffectDeliveryPrice(FMS_CODLine codLine)
        {
            strSql = @"SET @Msg=-1
						IF NOT EXISTS (SELECT 1 From RFD_FMS.dbo.FMS_CODLineWaitEffect (nolock) WHERE CODLineNO=@CODLineNO AND DeleteFlag=0 AND DistributionCode=@DistributionCode)
						BEGIN
							INSERT  INTO RFD_FMS.dbo.FMS_CODLineWaitEffect
									( CODLineNO ,
									  ExpressCompanyID ,
									  IsEchelon ,
									  WareHouseID ,
									  AreaType ,
									  PriceFormula ,
									  LineStatus ,
									  AuditStatus ,
									  AuditBy ,
									  AuditTime ,
									  CreateBy ,
									  CreateTime ,
									  UpdateBy ,
									  UpdateTime,
									  EffectDate,
									  WareHouseType,								
									  MerchantID,
									  ProductID,
                                      DistributionCode,
                                      IsCOD,
                                      Formula,
                                      IsChange
									)
							VALUES  ( @CODLineNO ,
									  @ExpressCompanyID ,
									  @IsEchelon ,
									  @WareHouseID ,
									  @AreaType ,
									  @PriceFormula ,
									  @LineStatus ,
									  @AuditStatus ,
									  '' ,
									  GETDATE() ,
									  @CreateBy ,
									  GETDATE() ,
									  '' ,
									  GETDATE(),
									  @EffectDate,
									  @WareHouseType,								
									  @MerchantID,
									  @ProductID,
                                      @DistributionCode,
                                      @IsCOD,
                                      @Formula,
                                      @IsChange			
									)
							SET @Msg=1
						END
						ELSE
						BEGIN
							SET @Msg=0
						END";

            SqlParameter[] parameters = {
			                          		new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
											new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
											new SqlParameter("@IsEchelon",SqlDbType.SmallInt),
											new SqlParameter("@WareHouseID",SqlDbType.NVarChar,20),
											new SqlParameter("@AreaType",SqlDbType.SmallInt),
											new SqlParameter("@PriceFormula",SqlDbType.NVarChar,200),
											new SqlParameter("@LineStatus",SqlDbType.SmallInt),
											new SqlParameter("@AuditStatus",SqlDbType.SmallInt),
											new SqlParameter("@CreateBy",SqlDbType.NVarChar,40),
											new SqlParameter("@Msg",SqlDbType.SmallInt),
											new SqlParameter("@EffectDate",SqlDbType.Date),
											new SqlParameter("@WareHouseType",SqlDbType.Int),
											new SqlParameter("@MerchantID",SqlDbType.Int),
											new SqlParameter("@ProductID",SqlDbType.Int),
                                            new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                            new SqlParameter("@IsCOD",SqlDbType.Int),
                                            new SqlParameter("@Formula",SqlDbType.NVarChar,150),
                                            new SqlParameter("@IsChange",SqlDbType.Bit)
										};

            parameters[0].Value = codLine.CODLineNO;
            parameters[1].Value = codLine.ExpressCompanyID;
            parameters[2].Value = codLine.IsEchelon;
            parameters[3].Value = codLine.WareHouseID;
            parameters[4].Value = codLine.AreaType;
            parameters[5].Value = codLine.PriceFormula;
            parameters[6].Value = codLine.LineStatus;
            parameters[7].Value = codLine.AuditStatus;
            parameters[8].Value = codLine.CreateBy;
            parameters[9].Direction = ParameterDirection.Output;
            parameters[10].Value = codLine.EffectDate;
            parameters[11].Value = codLine.WareHouseType;
            parameters[12].Value = codLine.MerchantID;
            parameters[13].Value = codLine.ProductID;
            parameters[14].Value = codLine.DistributionCode;
            parameters[15].Value = codLine.IsCOD;
            parameters[16].Value = codLine.Formula;
            parameters[17].Value = true;

            int n = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);

            return int.Parse(parameters[9].Value.ToString());
        }

        public DataTable GetListByEffectCodLineNo(string codLineNo)
        {
            strSql = @"SELECT  codl.LineID ,
								codl.CODLineNO ,
								codl.ExpressCompanyID ,
								ec.CompanyName ,
								codl.IsEchelon ,
								WareHouseID = CASE WareHouseType
												WHEN 1 THEN codl.WareHouseID
												WHEN 2 THEN 'S_' + codl.WareHouseID
												ELSE ''
											  END ,
								codl.AreaType ,
								codl.IsCOD,
								codl.PriceFormula ,
                                codl.Formula ,
								codl.LineStatus ,
								codl.AuditStatus ,
								codl.EffectDate ,
								codl.MerchantID ,
								codl.ProductID,
                                codl.DistributionCode,
                                mbi.MerchantName
						FROM    RFD_FMS.dbo.FMS_CODLineWaitEffect (NOLOCK) codl
								JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = codl.ExpressCompanyID
                                JOIN RFD_PMS.dbo.MerchantBaseInfo mbi ( NOLOCK ) ON mbi.ID = codl.MerchantID
						WHERE   ( DeleteFlag = 0 ) AND codl.CODLineNO=@CODLineNO";
            SqlParameter[] parameters ={
                                              new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
                                         };
            parameters[0].Value = codLineNo;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public bool UpdateEffectCodLine(FMS_CODLine codLine)
        {
            strSql = @"UPDATE  RFD_FMS.dbo.FMS_CODLineWaitEffect
							SET     ExpressCompanyID = @ExpressCompanyID ,
									IsEchelon = @IsEchelon ,
									WareHouseID = @WareHouseID ,
									WareHouseType=@WareHouseType,
									MerchantID=@MerchantID,
									ProductID=@ProductID,
									AreaType = @AreaType ,
                                    IsCOD=@IsCOD,
									PriceFormula = @PriceFormula ,
                                    Formula = @Formula ,
									LineStatus = @LineStatus ,
									UpdateBy = @UpdateBy ,
									UpdateTime = GETDATE(),
									AuditStatus=2,
									AuditBy='',
									AuditTime=GETDATE(),
									EffectDate=@EffectDate,
                                    IsChange=@IsChange
							WHERE   CODLineNO = @CODLineNO AND DeleteFlag=0";
            SqlParameter[] parameters ={
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
										   new SqlParameter("@IsEchelon",SqlDbType.Int),
										   new SqlParameter("@WareHouseID",SqlDbType.NVarChar,40),
										   new SqlParameter("@AreaType",SqlDbType.Int),
                                           new SqlParameter("@IsCOD",SqlDbType.Int),
										   new SqlParameter("@PriceFormula",SqlDbType.NVarChar,150),
                                           new SqlParameter("@Formula",SqlDbType.NVarChar,150),
										   new SqlParameter("@LineStatus",SqlDbType.Int),
										   new SqlParameter("@UpdateBy",SqlDbType.NVarChar,100),
										   new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
										   new SqlParameter("@EffectDate",SqlDbType.Date),
										   new SqlParameter("@WareHouseType",SqlDbType.Int),
										   new SqlParameter("@MerchantID",SqlDbType.Int),
										   new SqlParameter("@ProductID",SqlDbType.Int),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
            parameters[0].Value = codLine.ExpressCompanyID;
            parameters[1].Value = codLine.IsEchelon;
            parameters[2].Value = codLine.WareHouseID;
            parameters[3].Value = codLine.AreaType;
            parameters[4].Value = codLine.IsCOD;
            parameters[5].Value = codLine.PriceFormula;
            parameters[6].Value = codLine.Formula;
            parameters[7].Value = codLine.LineStatus;
            parameters[8].Value = codLine.UpdateBy;
            parameters[9].Value = codLine.CODLineNO;
            parameters[10].Value = codLine.EffectDate;
            parameters[11].Value = codLine.WareHouseType;
            parameters[12].Value = codLine.MerchantID;
            parameters[13].Value = codLine.ProductID;
            parameters[14].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public DataTable GetEffectDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId, string distributionCode, int IsCod, PageInfo pi, bool isPage)
        {
            #region sql
            StringBuilder str = new StringBuilder();
            if (isPage)
                str.Append(@"
SELECT @records=COUNT(1)
FROM    RFD_FMS.dbo.FMS_CODLineWaitEffect (NOLOCK) codl
		JOIN StatusCodeInfo sci ( NOLOCK ) ON sci.CodeNo = codl.LineStatus
											  AND sci.CodeType = 'AreaTypeLine'
		JOIN StatusCodeInfo sci1 ( NOLOCK ) ON sci1.CodeNo = codl.AuditStatus
											   AND sci1.CodeType = 'AreaTypeAudit'
		JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = codl.ExpressCompanyID
		LEFT JOIN RFD_PMS.dbo.Warehouse w ( NOLOCK ) ON w.WarehouseId = codl.WarehouseId
		LEFT JOIN RFD_PMS.dbo.ExpressCompany w1 ( NOLOCK ) ON w1.ExpressCompanyID = codl.WarehouseId
												  AND w1.IsDeleted = 0
												  AND w1.CompanyFlag = 1
		JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID=codl.MerchantID
WHERE   ( codl.DeleteFlag = 0 ) {0}

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
SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1

;WITH t AS (
");
            str.Append(@"SELECT ROW_NUMBER() OVER ( ORDER BY codl.CODLineNO DESC ) AS 序号 ,
                                codl.LineID ,
								codl.CODLineNO ,
								codl.ExpressCompanyID ,
								ec.CompanyName,
								codl.IsEchelon ,
								IsEchelonStr=CASE WHEN codl.IsEchelon=1 THEN '是' ELSE '否' END,
								codl.WareHouseID ,
								WarehouseName = CASE codl.WareHouseType
												  WHEN 1 THEN w.WarehouseName
												  WHEN 2 THEN w1.CompanyName
												  ELSE ''
												END ,
								codl.AreaType ,
                                IsCODStr=CASE WHEN codl.IsCOD=0 THEN '否' else '是' end,
								codl.PriceFormula ,
                                codl.Formula ,
								codl.LineStatus ,
								sci.CodeDesc AS LineStatusStr ,
								codl.AuditStatus ,
								sci1.CodeDesc AS AuditStatusStr ,
								codl.AuditBy ,
								codl.AuditTime ,
								codl.CreateBy ,
								codl.CreateTime ,
								codl.UpdateBy ,
								codl.UpdateTime ,
								codl.DeleteFlag,
								codl.EffectDate,
								mbi.MerchantName
						FROM    RFD_FMS.dbo.FMS_CODLineWaitEffect (NOLOCK) codl
						JOIN StatusCodeInfo sci(NOLOCK) ON sci.CodeNo=codl.LineStatus AND sci.CodeType='AreaTypeLine'
						JOIN StatusCodeInfo sci1(NOLOCK) ON sci1.CodeNo=codl.AuditStatus AND sci1.CodeType='AreaTypeAudit'
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=codl.ExpressCompanyID
						LEFT JOIN RFD_PMS.dbo.Warehouse w(NOLOCK) ON w.WarehouseId=codl.WarehouseId
						LEFT JOIN RFD_PMS.dbo.ExpressCompany w1 ( NOLOCK ) ON w1.ExpressCompanyID = codl.WarehouseId
																		  AND w1.IsDeleted = 0
																		  AND w1.CompanyFlag = 1
						JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID=codl.MerchantID
						WHERE (codl.DeleteFlag=0) {0} ");

            if (isPage)
                str.Append(@")
                SELECT * FROM t WHERE 序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1 ; 
                ");

            #endregion

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@records", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pages", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pi.PageSize });
            parameters.Add(new SqlParameter("@pageNo", SqlDbType.Int) { Value = pi.CurrentPageIndex });

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND codl.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(lineStatus))
            {
                sbWhere.Append(" AND codl.LineStatus=@LineStatus ");
                parameters.Add(new SqlParameter("@LineStatus", SqlDbType.Int) { Value = lineStatus });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND codl.AuditStatus=@AuditStatus ");
                parameters.Add(new SqlParameter("@AuditStatus", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND codl.AreaType=@AreaType ");
                parameters.Add(new SqlParameter("@AreaType", SqlDbType.Int) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(wareHouse))
            {
                sbWhere.Append(" AND codl.WareHouseID=@WareHouseID ");
                parameters.Add(new SqlParameter("@WareHouseID", SqlDbType.NVarChar, 20) { Value = wareHouse });
            }

            if (!string.IsNullOrEmpty(wareHouseType))
            {
                sbWhere.Append(" AND codl.WareHouseType=@WareHouseType ");
                parameters.Add(new SqlParameter("@WareHouseType", SqlDbType.Int) { Value = wareHouseType });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND codl.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND codl.DistributionCode=@DistributionCode ");
                parameters.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = distributionCode });
            }

            if (IsCod > -1)
            {
                sbWhere.Append(" AND codl.IsCOD=@IsCOD ");
                parameters.Add(new SqlParameter("@IsCOD", SqlDbType.Int) { Value = IsCod });
            }

            strSql = string.Format(str.ToString(), sbWhere.ToString());
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray());
            if (isPage)
            {
                pi.ItemCount = (int)parameters[0].Value;
                pi.PageCount = (int)parameters[1].Value;
            }
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];

        }

        public bool UpdateEffectDeliveryPriceAuditStatus(string cODLineNO, string auditBy, int auditStatus)
        {
            strSql = @"UPDATE RFD_FMS.dbo.FMS_CODLineWaitEffect SET AuditStatus=@AuditStatus,AuditBy=@AuditBy,AuditTime=GETDATE(),IsChange=@IsChange
												WHERE CODLineNO=@CODLineNO AND DeleteFlag=0";
            SqlParameter[] parameters =
            {
				new SqlParameter("@AuditStatus",SqlDbType.Int),
				new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
				new SqlParameter("@AuditBy",SqlDbType.NVarChar,100),
                new SqlParameter("@IsChange",SqlDbType.Bit)
			};

            parameters[0].Value = auditStatus;
            parameters[1].Value = cODLineNO;
            parameters[2].Value = auditBy;
            parameters[3].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public DataSet GetExportData()
        {
            strSql = @"with t as(
						select ExpressCompanyID,CompanyName,CompanyAllName from RFD_PMS.dbo.ExpressCompany(nolock) where CompanyFlag in (1,2,3) and DistributionCode<>'rfd' and IsDeleted=0
						union all
						select ExpressCompanyID,CompanyName,CompanyAllName from RFD_PMS.dbo.ExpressCompany(nolock) where CompanyFlag=2 and DistributionCode='rfd' and IsDeleted=0)
						select * from t;

						WITH    t AS ( SELECT   w.WarehouseId ,
												w.WarehouseName
									   FROM     RFD_PMS.dbo.Warehouse w ( NOLOCK )
									   WHERE    w.[Enable] = 1
									   UNION ALL
									   SELECT   'S_'+CONVERT(NVARCHAR(20),ExpressCompanyID ),
												CompanyName
									   FROM     RFD_PMS.dbo.ExpressCompany (NOLOCK) ec
									   WHERE    IsDeleted = 0
												AND CompanyFlag = 1
									 )
							SELECT  *
							FROM    t;

						SELECT sci.CodeNo
						FROM   StatusCodeInfo sci(NOLOCK)
						WHERE  sci.CodeType = 'AreaType'
							   AND sci.[Enabled] = 1;

						SELECT  ID ,
								MerchantName
						FROM    RFD_PMS.dbo.MerchantBaseInfo (NOLOCK) mbi
						WHERE   IsDeleted = 0;";
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql);
        }

        #region 配送费计算查询
        /// <summary>
        /// 获取可用的Cod线路价格
        /// </summary>
        /// <returns></returns>
        public DataTable GetCodLine(int expressCompanyId, int areaType, string distributionCode)
        {
            string sql = @" SELECT fcl.CODLineNO,
							fcl.ExpressCompanyID,
							fcl.IsEchelon,
							fcl.WareHouseID,
							fcl.AreaType,
							fcl.PriceFormula,
                            fcl.IsCOD,
                            fcl.Formula,
							fcl.WareHouseType,
							fcl.MerchantID,
                            fcl.DistributionCode
					 FROM   FMS_CODLine fcl(NOLOCK)
					 WHERE  fcl.AuditStatus = 1
							AND fcl.LineStatus = 1
							AND fcl.DeleteFlag = 0
							AND fcl.ExpressCompanyID = @ExpressCompanyID
							AND fcl.AreaType = @AreaType
                            AND fcl.DistributionCode = @DistributionCode";

            SqlParameter[] parameters ={
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
										   new SqlParameter("@AreaType",SqlDbType.Int),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
									  };
            parameters[0].Value = expressCompanyId;
            parameters[1].Value = areaType;
            parameters[2].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 返回当前月前4个月的历史
        /// </summary>
        /// <returns></returns>
        public DataTable GetCodLineHistory(int year, int month, int expressCompanyId, int areaType, string distributionCode)
        {
            string sql = @"SELECT tch.CODLineNO,
								   tch.ExpressCompanyID,
								   tch.IsEchelon,
								   tch.WareHouseID,
								   tch.AreaType,
								   tch.PriceFormula,
                                   tch.IsCOD,
                                   tch.Formula,
								   tch.BakYear,
								   tch.BakMonth,
								   tch.WareHouseType,
								   tch.MerchantID,
                                   tch.DistributionCode
							FROM   FMS_CODLineHistory tch(NOLOCK)
							WHERE  tch.BakYear = @year
								   AND tch.BakMonth = @month
								   AND AuditStatus = 1
								   AND LineStatus = 1
								   AND DeleteFlag = 0
								   AND ExpressCompanyID = @ExpressCompanyID
							       AND AreaType = @AreaType
                                   AND DistributionCode = @DistributionCode";
            SqlParameter[] parameters ={
										   new SqlParameter("@year",SqlDbType.Int),
										   new SqlParameter("@month",SqlDbType.Int),
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
										   new SqlParameter("@AreaType",SqlDbType.Int),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50)
									  };
            parameters[0].Value = year;
            parameters[1].Value = month;
            parameters[2].Value = expressCompanyId;
            parameters[3].Value = areaType;
            parameters[4].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
        #endregion

        #region cod价格变更校对

        /// <summary>
        /// 获取价格变更记录
        /// </summary>
        /// <param name="effectDate"></param>
        /// <returns></returns>
        public List<FMS_CODLine> GetEffectCodLine(DateTime effectDateStr, DateTime effectDateEnd)
        {
            string sql = @"SELECT tc.CODLineNO,
						   tc.ExpressCompanyID,
						   tc.IsEchelon,
						   tc.WareHouseID,
						   tc.AreaType,
						   tc.PriceFormula,
                           tc.IsCOD,
                           tc.Formula
					FROM   FMS_CODLine tc(NOLOCK)
					WHERE  tc.AuditTime >= @AuditTimeStr
						   AND tc.AuditTime < @AuditTimeEnd
						   AND tc.AuditStatus = 1
						   AND tc.LineStatus = 1
						   AND tc.DeleteFlag = 0";

            SqlParameter[] parameters ={
										   new SqlParameter("@AuditTimeStr",SqlDbType.DateTime),
										   new SqlParameter("@AuditTimeEnd",SqlDbType.DateTime)
									  };
            parameters[0].Value = effectDateStr;
            parameters[1].Value = effectDateEnd;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters);
            if (ds.Tables.Count > 0)
                return TransEffectAreaTypeObjectList(ds.Tables[0]);
            else
                return null;
        }

        private List<FMS_CODLine> TransEffectAreaTypeObjectList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return null;

            List<FMS_CODLine> codLineList = new List<FMS_CODLine>();
            foreach (DataRow dr in dt.Rows)
            {
                FMS_CODLine codLine = new FMS_CODLine();
                if (dr["CODLineNO"] != DBNull.Value)
                    codLine.CODLineNO = dr["CODLineNO"].ToString();

                if (dr["ExpressCompanyId"] != DBNull.Value)
                    codLine.ExpressCompanyID = int.Parse(dr["ExpressCompanyId"].ToString());

                if (dr["IsEchelon"] != DBNull.Value)
                    codLine.IsEchelon = int.Parse(dr["IsEchelon"].ToString());

                if (dr["WareHouseID"] != DBNull.Value)
                    codLine.WareHouseID = dr["WareHouseID"].ToString();

                if (dr["AreaType"] != DBNull.Value)
                    codLine.AreaType = int.Parse(dr["AreaType"].ToString());

                if (dr["PriceFormula"] != DBNull.Value)
                    codLine.PriceFormula = dr["PriceFormula"].ToString();

                if (dr["IsCOD"] != DBNull.Value)
                    codLine.IsCOD = int.Parse(dr["IsCOD"].ToString());

                if (dr["Formula"] != DBNull.Value)
                    codLine.Formula = dr["Formula"].ToString();

                codLineList.Add(codLine);
            }
            return codLineList;
        }
        #endregion

        #region CODLine备份

        public bool Insert(IList<FMS_CODLine> codLineList, string month, string year)
        {
            string existsSql = "IF NOT EXISTS(SELECT 1 FROM FMS_CODLineHistory(NOLOCK) WHERE CODLineNO = '{0}' AND BakYear ='{1}' AND BakMonth ='{2}') ";
            string insertSql = @" INSERT INTO FMS_CODLineHistory(CODLineNO,ExpressCompanyID,IsEchelon,
									WareHouseID,AreaType,PriceFormula,LineStatus,AuditStatus,AuditBy,AuditTime,CreateBy,CreateTime,
									UpdateBy,UpdateTime,DeleteFlag,BakYear,BakMonth,WareHouseType,MerchantID,ProductID,DistributionCode,
                                    Formula,IsCOD,IsChange) VALUES ";
            StringBuilder sbSql = new StringBuilder();
            List<SqlParameter> parameterList = new List<SqlParameter>();
            int i = 0;
            string formart = "@{0}{1}{2}";
            foreach (FMS_CODLine d in codLineList)
            {
                sbSql.Append(string.Format(existsSql, d.CODLineNO, year, month));
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append(string.Format(formart, "CODLineNO", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "IsEchelon", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "PriceFormula", i, ","));
                sbSql.Append(string.Format(formart, "LineStatus", i, ","));
                sbSql.Append(string.Format(formart, "AuditStatus", i, ","));
                sbSql.Append(string.Format(formart, "AuditBy", i, ","));
                sbSql.Append(string.Format(formart, "AuditTime", i, ","));
                sbSql.Append(string.Format(formart, "CreateBy", i, ","));
                sbSql.Append(string.Format(formart, "CreateTime", i, ","));
                sbSql.Append(string.Format(formart, "UpdateBy", i, ","));
                sbSql.Append(string.Format(formart, "UpdateTime", i, ","));
                sbSql.Append(string.Format(formart, "DeleteFlag", i, ","));
                sbSql.Append(string.Format(formart, "BakYear", i, ","));
                sbSql.Append(string.Format(formart, "BakMonth", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "ProductID", i, ","));
                sbSql.Append(string.Format(formart, "DistributionCode", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append(string.Format(formart, "IsCOD", i, ","));
                sbSql.Append(1);
                sbSql.Append(")");
                if (i < codLineList.Count - 1)
                {
                    sbSql.Append(" \n ");
                }
                parameterList.Add(new SqlParameter(string.Format(formart, "CODLineNO", i, ""), d.CODLineNO));
                parameterList.Add(new SqlParameter(string.Format(formart, "ExpressCompanyID", i, ""), d.ExpressCompanyID));
                parameterList.Add(new SqlParameter(string.Format(formart, "IsEchelon", i, ""), d.IsEchelon));
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseID", i, ""), d.WareHouseID));
                parameterList.Add(new SqlParameter(string.Format(formart, "AreaType", i, ""), d.AreaType));
                parameterList.Add(new SqlParameter(string.Format(formart, "PriceFormula", i, ""), d.PriceFormula));
                parameterList.Add(new SqlParameter(string.Format(formart, "LineStatus", i, ""), d.LineStatus));
                parameterList.Add(new SqlParameter(string.Format(formart, "AuditStatus", i, ""), d.AuditStatus));
                parameterList.Add(new SqlParameter(string.Format(formart, "AuditBy", i, ""), d.AuditBy));
                parameterList.Add(new SqlParameter(string.Format(formart, "AuditTime", i, ""), d.AuditTime));
                parameterList.Add(new SqlParameter(string.Format(formart, "CreateBy", i, ""), d.CreateBy));
                parameterList.Add(new SqlParameter(string.Format(formart, "CreateTime", i, ""), d.CreateTime));
                parameterList.Add(new SqlParameter(string.Format(formart, "UpdateBy", i, ""), d.UpdateBy));
                parameterList.Add(new SqlParameter(string.Format(formart, "UpdateTime", i, ""), d.UpdateTime));
                parameterList.Add(new SqlParameter(string.Format(formart, "DeleteFlag", i, ""), d.DeleteFlag));
                parameterList.Add(new SqlParameter(string.Format(formart, "BakYear", i, ""), year));
                parameterList.Add(new SqlParameter(string.Format(formart, "BakMonth", i, ""), month));
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseType", i, ""), d.WareHouseType));
                parameterList.Add(new SqlParameter(string.Format(formart, "MerchantID", i, ""), d.MerchantID));
                parameterList.Add(new SqlParameter(string.Format(formart, "ProductID", i, ""), d.ProductID));
                parameterList.Add(new SqlParameter(string.Format(formart, "DistributionCode", i, ""), d.DistributionCode));
                parameterList.Add(new SqlParameter(string.Format(formart, "Formula", i, ""), d.Formula));
                parameterList.Add(new SqlParameter(string.Format(formart, "IsCOD", i, ""), d.IsCOD));
                i++;
            }
            string sql = sbSql.ToString();
            SqlParameter[] parameters = parameterList.ToArray();
            bool flag = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
            return flag;
        }

        public int UpdateToDelete(string year, string month)
        {
            string sql = "UPDATE FMS_CODLineHistory SET DeleteFlag=1,IsChange=1 WHERE BakMonth='{0}' AND BakYear='{1}' AND DeleteFlag=0";
            //Sql = "delete from FMS_CODLineHistory WHERE BakMonth='{0}' AND BakYear='{1}'";
            sql = string.Format(sql, month, year);

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql);
        }

        /// <summary>
        /// 获取整个需要备份
        /// </summary>
        /// <returns></returns>
        public IList<FMS_CODLine> GetBackList()
        {
            string sql = @"SELECT CODLineNO,ExpressCompanyID,IsEchelon,WareHouseID,AreaType,PriceFormula,LineStatus,
							AuditStatus,AuditBy,AuditTime,CreateBy,CreateTime,UpdateBy,UpdateTime,DeleteFlag,
							WareHouseType,MerchantID,ProductID,DistributionCode,Formula,IsCOD
					FROM FMS_CODLine (NOLOCK)";
            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
            IList<FMS_CODLine> codLineList = new List<FMS_CODLine>();
            TableToModel(dt, ref codLineList);

            return codLineList;
        }

        private void TableToModel(DataTable dt, ref IList<FMS_CODLine> codLineList)
        {
            FMS_CODLine codLine;
            foreach (DataRow dr in dt.Rows)
            {
                codLine = new FMS_CODLine();
                codLine.CODLineNO = dr["CODLineNO"].ToString();
                codLine.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
                codLine.IsEchelon = int.Parse(dr["IsEchelon"].ToString());
                codLine.WareHouseID = dr["WareHouseID"].ToString();
                codLine.AreaType = int.Parse(dr["AreaType"].ToString());
                codLine.PriceFormula = dr["PriceFormula"].ToString();
                codLine.LineStatus = int.Parse(dr["LineStatus"].ToString());
                codLine.AuditStatus = int.Parse(dr["AuditStatus"].ToString());
                codLine.AuditBy = dr["AuditBy"].ToString();
                codLine.AuditTime = DateTime.Parse(dr["AuditTime"].ToString());
                codLine.CreateBy = dr["CreateBy"].ToString();
                codLine.CreateTime = DateTime.Parse(dr["CreateTime"].ToString());
                codLine.UpdateBy = dr["UpdateBy"].ToString();
                codLine.UpdateTime = DateTime.Parse(dr["UpdateTime"].ToString());
                codLine.DeleteFlag = Boolean.Parse(dr["DeleteFlag"].ToString());
                codLine.WareHouseType = int.Parse(dr["WareHouseType"].ToString());
                codLine.MerchantID = int.Parse(dr["MerchantID"].ToString());
                codLine.ProductID = int.Parse(dr["ProductID"].ToString());
                codLine.DistributionCode = dr["DistributionCode"].ToString();
                codLine.IsCOD = int.Parse(dr["IsCOD"].ToString());
                codLine.Formula = dr["Formula"].ToString();
                codLineList.Add(codLine);
            }
        }
        #endregion

        #region COD待生效
        public List<FMS_CODLine> GetCODLineWaitEffect(string Date)
        {
            string sql = @"SELECT tcwe.LineID,
						   tcwe.CODLineNO,
						   tcwe.ExpressCompanyID,
						   tcwe.IsEchelon,
						   tcwe.WareHouseID,
						   tcwe.AreaType,
						   tcwe.PriceFormula,
						   tcwe.EffectDate,
                           tcwe.Formula,
                           tcwe.IsCOD
					FROM   FMS_CODLineWaitEffect tcwe(NOLOCK)
					WHERE  tcwe.AuditStatus = 1
						   AND tcwe.LineStatus = 1
						   AND DeleteFlag = 0
						   AND tcwe.EffectDate = @EffectDate";
            SqlParameter[] parameters ={
										   new SqlParameter("@EffectDate",SqlDbType.Date)
									  };
            parameters[0].Value = Date;
            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];

            return CreateCODLineWaitEffectList(dt);
        }

        private List<FMS_CODLine> CreateCODLineWaitEffectList(DataTable dt)
        {
            List<FMS_CODLine> clweList = new List<FMS_CODLine>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    FMS_CODLine clwe = new FMS_CODLine();
                    clwe.LineID = int.Parse(dr["LineID"].ToString());
                    clwe.CODLineNO = dr["CODLineNO"].ToString();
                    clwe.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
                    clwe.IsEchelon = int.Parse(dr["IsEchelon"].ToString());
                    clwe.WareHouseID = dr["WareHouseID"].ToString();
                    clwe.AreaType = int.Parse(dr["AreaType"].ToString());
                    clwe.PriceFormula = dr["PriceFormula"].ToString();
                    clwe.EffectDate = DateTime.Parse(dr["EffectDate"].ToString());
                    clwe.Formula = dr["Formula"].ToString();
                    clwe.IsCOD = int.Parse(dr["IsCOD"].ToString());
                    clweList.Add(clwe);
                }
                return clweList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新已生效价格 回改待生效状态
        /// </summary>
        /// <param name="clwe"></param>
        /// <returns></returns>
        public bool UpdateLineForCODLine(FMS_CODLine clwe)
        {
            string sql = @"UPDATE FMS_CODLine
					SET    PriceFormula = @PriceFormula,
                            Formula = @Formula,
                            IsCOD = @IsCOD,
							AuditStatus = 1,
							UpdateBy = 0 ,
							UpdateTime = @UpdateTime,
							AuditBy = 0 ,
							AuditTime = GETDATE(),
                            IsChange=@IsChange
					WHERE  CODLineNO = @CODLineNO";//AuditTime取当前同步时间，是为校对准备，避免8-15待生效审核却在9-1生效的情况

            SqlParameter[] parameters =
            {
				new SqlParameter("@PriceFormula",SqlDbType.NVarChar,150),
                new SqlParameter("@Formula",SqlDbType.NVarChar,150),
                new SqlParameter("@IsCOD",SqlDbType.Int),
				new SqlParameter("@CODLineNO",SqlDbType.NVarChar,20),
				new SqlParameter("@UpdateTime",SqlDbType.DateTime),
                new SqlParameter("@IsChange",SqlDbType.Bit)
			};

            parameters[0].Value = clwe.PriceFormula;
            parameters[1].Value = clwe.Formula;
            parameters[2].Value = clwe.IsCOD;
            parameters[3].Value = clwe.CODLineNO;
            parameters[4].Value = clwe.EffectDate;
            parameters[5].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public bool UpdateLineForCODLineWaitEffect(FMS_CODLine clwe)
        {
            string sql = @"UPDATE FMS_CODLineWaitEffect
					SET    DeleteFlag = 1,IsChange=@IsChange
					WHERE  LineID = @LineID";
            SqlParameter[] parameters1 =
            {
				new SqlParameter("@LineID",SqlDbType.BigInt),
                new SqlParameter("@IsChange",SqlDbType.Bit)
			};

            parameters1[0].Value = clwe.LineID;
            parameters1[1].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters1) > 0;
        }

        #endregion
    }
}
