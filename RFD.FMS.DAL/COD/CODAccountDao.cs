using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.COD;
using System.Xml;

namespace RFD.FMS.DAL.COD
{
    public class CODAccountDao : SqlServerDao, ICODAccountDao
    {
		private string strSql = "";

		public DataTable GetCompanyIdByTopCODCompanyID(string topCodCompanyId)
		{
			strSql = @" SELECT  ec.ExpressCompanyID ,
        ec.CompanyName ,
        ec.ParentID ,
        TopCODCompanyID
FROM    RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK )
WHERE   ec.TopCODCompanyID = @TopCODCompanyID";
			SqlParameter[] parameters ={
                                           new SqlParameter("@TopCODCompanyID",SqlDbType.Int)
                                      };
			parameters[0].Value = topCodCompanyId;
			return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
		}

		public DataTable GetCompanyIdByRfd()
		{
			strSql = @"SELECT  ec1.ExpressCompanyID ,
        ec1.CompanyName ,
        ec1.ParentID ,
        ec1.TopCODCompanyID
FROM    RFD_PMS.dbo.ExpressCompany AS ec1 ( NOLOCK )
WHERE   ec1.ParentID IN (
        SELECT  ec.ExpressCompanyID
        FROM    RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK )
        WHERE   ec.CompanyFlag = 1
                AND ec.DistributionCode = 'rfd'
                AND ec.IsDeleted = 0
                AND ec.ParentID <> 11 )
        AND ec1.IsDeleted = 0";
			return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
		}

		public DataTable SearchUniteAccount(CODSearchCondition condition)
		{
			#region sql
			strSql = @"	DECLARE @Ex TABLE(ExpressCompanyID INT NULL )
						INSERT  INTO @Ex( ExpressCompanyID) 
							SELECT  T.x.value('./@v', 'int') AS ExpressCompanyID
							FROM    @ExpressComapnyXml.nodes('/root/id') AS T ( x )
						DECLARE @whTableD TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL )
						INSERT  INTO @whTableD( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseD.nodes('/root/id') AS T ( x )
						DECLARE @whTableR TABLE( HouseID NVARCHAR(20) NULL,HouseType INT NULL)
						INSERT  INTO @whTableR( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseR.nodes('/root/id') AS T ( x )
						DECLARE @whTableV TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL)
						INSERT  INTO @whTableV( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseV.nodes('/root/id') AS T ( x ) ;
						WITH  groups AS ( SELECT   ExpressCompanyID ,
												AreaType ,
												SUM(FormCount) AS DeliveryNum ,
												SUM(Fare) AS Fare ,
												Formula ,
												'D' AS CounType,
												MerchantID
									   FROM     FMS_CODDeliveryCount (NOLOCK) d
									   WHERE    AccountDate BETWEEN @DdateS AND @DdateE
												AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = d.ExpressCompanyID)
												AND d.MerchantID=@MerchantID
												AND EXISTS ( SELECT 1
															 FROM   @whTableD whd
															 WHERE  whd.HouseID = d.WareHouseID AND whd.HouseType=d.WareHouseType)
												AND AccountNO = ''
												AND DeliveryType=0
												AND DeleteFlag = 0
									   GROUP BY AreaType ,ExpressCompanyID ,Formula,MerchantID
									UNION ALL
									SELECT ExpressCompanyID,
											   AreaType,
											   SUM(FormCount) AS DeliveryNum,
											   SUM(Fare) AS Fare,
											   Formula,
											   'DV' AS CounType,
											   MerchantID
										FROM   FMS_CODDeliveryCount(NOLOCK) d
										WHERE  AccountDate BETWEEN  @DdateS AND @DdateE
											   AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = d.ExpressCompanyID)
											   AND d.MerchantID=@MerchantID
											   AND EXISTS (
													   SELECT 1
													   FROM   @whTableD whd
													   WHERE  whd.HouseID = d.WareHouseID AND whd.HouseType=d.WareHouseType)
											   AND AccountNO = ''
											   AND DeliveryType=1
											   AND DeleteFlag = 0
										GROUP BY AreaType,ExpressCompanyID,Formula,MerchantID
									   UNION ALL
									   SELECT   ExpressCompanyID ,
												AreaType ,
												SUM(FormCount) AS ReturnsNum ,
												SUM(Fare) AS Fare ,
												Formula ,
												'R' AS CounType,
												MerchantID
									   FROM     RFD_FMS.dbo.FMS_CODReturnsCount (NOLOCK) r
									   WHERE    AccountDate BETWEEN @DdateS AND @DdateE
												AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = r.ExpressCompanyID)
												AND r.MerchantID=@MerchantID
												AND EXISTS ( SELECT 1
															 FROM   @whTableR whd
															 WHERE  whd.HouseID = r.WareHouseID AND whd.HouseType=r.WareHouseType )
												AND AccountNO = ''
												AND ReturnsType=0
												AND DeleteFlag = 0
									   GROUP BY AreaType ,ExpressCompanyID ,Formula,MerchantID
									   UNION ALL
										SELECT ExpressCompanyID,
											   AreaType,
											   SUM(FormCount) AS ReturnsNum,
											   SUM(Fare) AS Fare,
											   Formula,
											   'RV' AS CounType,
											   MerchantID
										FROM   RFD_FMS.dbo.FMS_CODReturnsCount(NOLOCK) r
										WHERE  AccountDate BETWEEN @DdateS AND @DdateE
											   AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = r.ExpressCompanyID)
											   AND r.MerchantID=@MerchantID
											   AND EXISTS (
													   SELECT 1
													   FROM   @whTableR whd
													   WHERE  whd.HouseID = r.WareHouseID AND whd.HouseType=r.WareHouseType)
											   AND AccountNO = ''
											   AND ReturnsType=1
											   AND DeleteFlag = 0
										GROUP BY AreaType,ExpressCompanyID,Formula,MerchantID
										UNION ALL
									   SELECT   ExpressCompanyID ,
												AreaType ,
												SUM(FormCount) AS VisitReturnsNum ,
												SUM(Fare) AS Fare ,
												Formula ,
												'V' AS CounType,
												MerchantID
									   FROM     RFD_FMS.dbo.FMS_CODVisitReturnsCount (NOLOCK) v
									   WHERE    AccountDate BETWEEN @DdateS AND @DdateE
												AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = v.ExpressCompanyID)
												AND v.MerchantID=@MerchantID
												AND EXISTS ( SELECT 1
															 FROM   @whTableR whd
															 WHERE  whd.HouseID = v.WareHouseID  AND whd.HouseType=v.WareHouseType)
												AND AccountNO = ''
												AND DeleteFlag = 0
									   GROUP BY AreaType ,ExpressCompanyID ,Formula,MerchantID
									 )
							SELECT  0 as AccountDetailID,g.ExpressCompanyID , AreaType ,Formula ,
									MAX(CASE CounType WHEN 'D' THEN DeliveryNum ELSE 0 END) AS DeliveryNum,
									MAX(CASE CounType WHEN 'DV' THEN DeliveryNum ELSE 0 END) AS DeliveryVNum,
									MAX(CASE CounType WHEN 'R' THEN DeliveryNum ELSE 0 END) AS ReturnsNum,
									MAX(CASE CounType WHEN 'RV' THEN DeliveryNum ELSE 0 END) AS ReturnsVNum,
									MAX(CASE CounType WHEN 'V' THEN DeliveryNum ELSE 0 END) AS VisitReturnsNum,
									MAX(CASE CounType WHEN 'D' THEN Fare ELSE 0 END) AS DFare,
									MAX(CASE CounType WHEN 'DV' THEN Fare ELSE 0 END) AS DVFare,
									MAX(CASE CounType WHEN 'R' THEN Fare ELSE 0 END) AS RFare,
									MAX(CASE CounType WHEN 'RV' THEN Fare ELSE 0 END) AS RVFare,
									MAX(CASE CounType WHEN 'V' THEN Fare ELSE 0 END) AS VFare,
									0.00 AS DatumFare ,0.00 AS Allowance ,0.00 AS KPI ,0.00 AS POSPrice ,0.00 AS StrandedPrice ,
									0.00 AS IntercityLose ,0.00 AS OtherCost ,0.00 AS Fare ,g.MerchantID,m.MERCHANTNAME,
									ec.CompanyName,0 AS DataType,0 AS AccountNum
							FROM    groups g
							LEFT JOIN RFD_PMS.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = g.ExpressCompanyID
							JOIN RFD_PMS.dbo.MERCHANTBASEINFO AS m(NOLOCK) ON g.MerchantID=m.ID
							GROUP BY g.ExpressCompanyID ,AreaType ,Formula,ec.CompanyName,m.MERCHANTNAME,g.MerchantID";
			#endregion

			SqlParameter[] parameters = {
			                            new SqlParameter("@houseD",SqlDbType.Xml), 
										new SqlParameter("@houseR",SqlDbType.Xml), 
										new SqlParameter("@houseV",SqlDbType.Xml),
										new SqlParameter("@DdateS", SqlDbType.Date),
										new SqlParameter("@DdateE", SqlDbType.Date),
										new SqlParameter("@RdateS", SqlDbType.Date),
										new SqlParameter("@RdateE", SqlDbType.Date),
										new SqlParameter("@VdateS", SqlDbType.Date),
										new SqlParameter("@VdateE", SqlDbType.Date),
										new SqlParameter("@express", SqlDbType.Int),
										new SqlParameter("@ExpressComapnyXml", SqlDbType.Xml),
										new SqlParameter("@MerchantID", SqlDbType.Int)
									};
			parameters[0].Value = condition.HouseD;
			parameters[1].Value = condition.HouseR;
			parameters[2].Value = condition.HouseV;
			parameters[3].Value = condition.Date_D_S;
			parameters[4].Value = condition.Date_D_E;
			parameters[5].Value = condition.Date_R_S;
			parameters[6].Value = condition.Date_R_E;
			parameters[7].Value = condition.Date_V_S;
			parameters[8].Value = condition.Date_V_E;
			parameters[9].Value = condition.ExpressCompanyID;
			parameters[10].Value = condition.ExpressCompanyChilds;
			parameters[11].Value = condition.MerchantID;

			return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
		}

        public DataTable GetChanageCountList(CODSearchCondition searchCondition)
        {
            throw new Exception("SQL 中没有实现");
        }

        public bool BatchChangeCountAccountNO(string accountIds, string createBy, string AccountNo, string tableName)
        {
            throw new Exception("SQL 中没有实现");
        }

		public bool ChanageCountAcountNO(CODSearchCondition searchCondition, string createBy, string accountNo)
		{
			#region sql
			strSql = @"DECLARE @Ex TABLE(ExpressCompanyID INT NULL )
						INSERT  INTO @Ex( ExpressCompanyID) 
							SELECT  T.x.value('./@v', 'int') AS ExpressCompanyID
							FROM    @ExpressComapnyXml.nodes('/root/id') AS T ( x )
						DECLARE @whTableD TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL )
						INSERT  INTO @whTableD( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseD.nodes('/root/id') AS T ( x )
						DECLARE @whTableR TABLE( HouseID NVARCHAR(20) NULL,HouseType INT NULL)
						INSERT  INTO @whTableR( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseR.nodes('/root/id') AS T ( x )
						DECLARE @whTableV TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL)
						INSERT  INTO @whTableV( HouseID,HouseType) 
							SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
							FROM    @houseV.nodes('/root/id') AS T ( x ) ;
						UPDATE  FMS_CODDeliveryCount
						SET     AccountNO = @AccountNO ,
								UpdateBy = @UpdateBy ,
								UpdateTime = GETDATE(),
                                IsChange=@IsChange
						WHERE   AccountDate BETWEEN @DdateS AND @DdateE
								AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = FMS_CODDeliveryCount.ExpressCompanyID)
								AND MerchantID = @MerchantID
								AND EXISTS ( SELECT 1
											 FROM   @whTableD whd
											 WHERE  whd.HouseID = FMS_CODDeliveryCount.WareHouseID AND whd.HouseType=FMS_CODDeliveryCount.WareHouseType)
								AND AccountNO = ''
								AND DeleteFlag = 0
						UPDATE  RFD_FMS.dbo.FMS_CODReturnsCount
						SET     AccountNO = @AccountNO ,
								UpdateBy = @UpdateBy ,
								UpdateTime = GETDATE(),
                                IsChange=@IsChange
						WHERE   AccountDate BETWEEN @RdateS AND @RdateE
								AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = FMS_CODReturnsCount.ExpressCompanyID)
								AND MerchantID = @MerchantID
								AND EXISTS ( SELECT 1
											 FROM   @whTableR whr
											 WHERE  whr.HouseID = FMS_CODReturnsCount.WareHouseID AND whr.HouseType=FMS_CODReturnsCount.WareHouseType)
								AND AccountNO = ''
								AND DeleteFlag = 0
						UPDATE  RFD_FMS.dbo.FMS_CODVisitReturnsCount
						SET     AccountNO = @AccountNO ,
								UpdateBy = @UpdateBy ,
								UpdateTime = GETDATE(),
                                IsChange=@IsChange
						WHERE   AccountDate BETWEEN @VdateS AND @VdateE
								AND EXISTS ( SELECT 1
															 FROM   @Ex e
															 WHERE  e.ExpressCompanyID = FMS_CODVisitReturnsCount.ExpressCompanyID)
								AND MerchantID = @MerchantID
								AND EXISTS ( SELECT 1
											 FROM   @whTableV whv
											 WHERE  whv.HouseID = FMS_CODVisitReturnsCount.WareHouseID AND whv.HouseType=FMS_CODVisitReturnsCount.WareHouseType)
								AND AccountNO = ''
								AND DeleteFlag = 0";
			#endregion
			SqlParameter[] parameters = {
					                            	new SqlParameter("@houseD",SqlDbType.Xml), 
													new SqlParameter("@houseR",SqlDbType.Xml), 
													new SqlParameter("@houseV",SqlDbType.Xml),
													new SqlParameter("@DdateS", SqlDbType.Date),
													new SqlParameter("@DdateE", SqlDbType.Date),
													new SqlParameter("@RdateS", SqlDbType.Date),
													new SqlParameter("@RdateE", SqlDbType.Date),
													new SqlParameter("@VdateS", SqlDbType.Date),
													new SqlParameter("@VdateE", SqlDbType.Date),
													new SqlParameter("@ExpressComapnyXml", SqlDbType.Xml),
													new SqlParameter("@UpdateBy", SqlDbType.NVarChar,40),
													new SqlParameter("@AccountNO", SqlDbType.NVarChar,20),
													new SqlParameter("@MerchantID", SqlDbType.Int),
                                                    new SqlParameter("@IsChange", SqlDbType.Bit)
					                            };
			parameters[0].Value = searchCondition.HouseD;
			parameters[1].Value = searchCondition.HouseR;
			parameters[2].Value = searchCondition.HouseV;
			parameters[3].Value = searchCondition.Date_D_S;
			parameters[4].Value = searchCondition.Date_D_E;
			parameters[5].Value = searchCondition.Date_R_S;
			parameters[6].Value = searchCondition.Date_R_E;
			parameters[7].Value = searchCondition.Date_V_S;
			parameters[8].Value = searchCondition.Date_V_E;
			parameters[9].Value = searchCondition.ExpressCompanyChilds;
			parameters[10].Value = createBy;
			parameters[11].Value = accountNo;
			parameters[12].Value = searchCondition.MerchantID;
            parameters[13].Value = true;

			return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters)>0;
		}

		public bool AddAccountDetail(DataRow dr,string createBy,string accountNo)
		{
			#region sql
			strSql = @"INSERT  INTO FMS_CODAccountDetail
									( AccountNO ,
									  ExpressCompanyID ,
									  AreaType ,
									  DeliveryNum ,
									  DeliveryVNum ,
									  ReturnsNum ,
									  ReturnsVNum ,
									  VisitReturnsNum ,
									  Formula ,
									  DatumFare ,
									  Allowance,
									  KPI,
									  POSPrice,
									  StrandedPrice,
									  IntercityLose,
									  OtherCost,
									  Fare,
									  DataType,
									  CreateBy ,
									  CreateTime ,
									  UpdateBy ,
									  UpdateTime,
									  MerchantID,
									  WareHouseID,
									  AccountNum,
                                      IsChange
									)
							VALUES  ( @AccountNO ,
									  @ExpressCompanyID ,
									  @AreaType ,
									  @DeliveryNum ,
									  @DeliveryVNum ,
									  @ReturnsNum ,
									  @ReturnsVNum ,
									  @VisitReturnsNum ,
									  @Formula ,
									  @DatumFare ,
									  @Allowance,
									  @KPI,
									  @POSPrice,
									  @StrandedPrice,
									  @IntercityLose,
									  @OtherCost,
									  @Fare,
									  @DataType,
									  @CreateBy ,
									  GETDATE() ,
									  '' ,
									  GETDATE(),
										@MerchantID,
										@WareHouseID,
										@AccountNum,
                                        @IsChange
									)";
			#endregion
			SqlParameter[] parameters = {
			                             	new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20),
			                             	new SqlParameter("@ExpressCompanyID", SqlDbType.Int),
			                             	new SqlParameter("@AreaType", SqlDbType.SmallInt),
			                             	new SqlParameter("@DeliveryNum", SqlDbType.Int),
											new SqlParameter("@DeliveryVNum", SqlDbType.Int),
			                             	new SqlParameter("@ReturnsNum", SqlDbType.Int),
			                             	new SqlParameter("@ReturnsVNum", SqlDbType.Int),
			                             	new SqlParameter("@VisitReturnsNum", SqlDbType.Int),
			                             	new SqlParameter("@Formula", SqlDbType.NVarChar, 100),
			                             	new SqlParameter("@DatumFare", SqlDbType.Decimal, 18),
											new SqlParameter("@Allowance", SqlDbType.Decimal, 18),
											new SqlParameter("@KPI", SqlDbType.Decimal, 18),
											new SqlParameter("@POSPrice", SqlDbType.Decimal, 18),
											new SqlParameter("@StrandedPrice", SqlDbType.Decimal, 18),
											new SqlParameter("@IntercityLose", SqlDbType.Decimal, 18),
											new SqlParameter("@OtherCost", SqlDbType.Decimal, 18),
											new SqlParameter("@Fare", SqlDbType.Decimal, 18),
											new SqlParameter("@DataType", SqlDbType.SmallInt),
			                             	new SqlParameter("@CreateBy", SqlDbType.NVarChar, 40),
											new SqlParameter("@MerchantID", SqlDbType.Int),
											new SqlParameter("@WareHouseID", SqlDbType.NVarChar,20),
											new SqlParameter("@AccountNum", SqlDbType.Int),
                                            new SqlParameter("@IsChange", SqlDbType.Bit)
			                             };
			parameters[0].Value = accountNo;
			parameters[1].Value = dr["ExpressCompanyID"];
			parameters[2].Value = dr["AreaType"];
			parameters[3].Value = dr["DeliveryNum"];
			parameters[4].Value = dr["DeliveryVNum"];
			parameters[5].Value = dr["ReturnsNum"];
			parameters[6].Value = dr["ReturnsVNum"];
			parameters[7].Value = dr["VisitReturnsNum"];
			parameters[8].Value = dr["Formula"];
			parameters[9].Value = dr["DatumFare"];
			parameters[10].Value = dr["Allowance"];
			parameters[11].Value = dr["KPI"];
			parameters[12].Value = dr["POSPrice"];
			parameters[13].Value = dr["StrandedPrice"];
			parameters[14].Value = dr["IntercityLose"];
			parameters[15].Value = dr["OtherCost"];
			parameters[16].Value = dr["Fare"];
			parameters[17].Value = dr["DataType"];
			parameters[18].Value = createBy;
			parameters[19].Value = dr["MerchantID"];
			parameters[20].Value = "0";
			parameters[21].Value = dr["AccountNum"];
            parameters[22].Value = true;

			return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
		}

		public bool AddAccount(CODSearchCondition searchCondition, string createBy, string accountNo)
		{
			#region sql
			strSql = @"INSERT INTO FMS_CODAccount
								( AccountNO ,
								  ExpressCompanyID ,
								  AccountDate ,
								  AccountStatus ,
								  CreateBy ,
								  CreateTime ,
								  UpdateBy ,
								  UpdateTime ,
								  AuditBy ,
								  AuditTime,
									DeliveryDateStr,
									DeliveryDateEnd,
									ReturnsDateStr,
									ReturnsDateEnd,
									VisitReturnsDateStr,
									VisitReturnsDateEnd,
									DeliveryHouse,
									ReturnsHouse,
									VisitReturnsHouse,
									AccountType,
									MerchantID,
                                    IsDifference,
                                    IsChange
								)
						VALUES  ( @AccountNO ,
								  @ExpressCompanyID ,
								  GETDATE() ,
								  @AccountStatus ,
								  @CreateBy ,
								  GETDATE() ,
								  '' ,
								  GETDATE() ,
								  '' ,
								  GETDATE(),
									@DeliveryDateStr,
									@DeliveryDateEnd,
									@ReturnsDateStr,
									@ReturnsDateEnd,
									@VisitReturnsDateStr,
									@VisitReturnsDateEnd,
									@DeliveryHouse,
									@ReturnsHouse,
									@VisitReturnsHouse,
									@AccountType,
									@MerchantID,
                                    @IsDifference,
                                    @IsChange
								)";
			#endregion

			SqlParameter[] parameters = {
					                             	new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20),
													new SqlParameter("@AccountStatus", SqlDbType.SmallInt),
					                             	new SqlParameter("@CreateBy", SqlDbType.NVarChar, 40),
													new SqlParameter("@DeliveryHouse", SqlDbType.NVarChar,1000),
													new SqlParameter("@ReturnsHouse", SqlDbType.NVarChar,1000),
													new SqlParameter("@VisitReturnsHouse", SqlDbType.NVarChar,1000),
													new SqlParameter("@DeliveryDateStr", SqlDbType.Date),
													new SqlParameter("@DeliveryDateEnd", SqlDbType.Date),
													new SqlParameter("@ReturnsDateStr", SqlDbType.Date),
													new SqlParameter("@ReturnsDateEnd", SqlDbType.Date),
													new SqlParameter("@VisitReturnsDateStr", SqlDbType.Date),
													new SqlParameter("@VisitReturnsDateEnd", SqlDbType.Date),
													new SqlParameter("@ExpressCompanyID", SqlDbType.Int),
													new SqlParameter("@AccountType", SqlDbType.Int),
													new SqlParameter("@MerchantID", SqlDbType.Int),
                                                    new SqlParameter("@IsDifference", SqlDbType.Int),
                                                    new SqlParameter("@IsChange", SqlDbType.Bit)
					                             };
			parameters[0].Value = accountNo;
			parameters[1].Value = (int)EnumAccountAudit.A1;
			parameters[2].Value = createBy;
			parameters[3].Value = searchCondition.HouseD;
			parameters[4].Value = searchCondition.HouseR;
			parameters[5].Value = searchCondition.HouseV;
			parameters[6].Value = searchCondition.Date_D_S;
			parameters[7].Value = searchCondition.Date_D_E; ;
			parameters[8].Value = searchCondition.Date_R_S;
			parameters[9].Value = searchCondition.Date_R_E;
			parameters[10].Value = searchCondition.Date_V_S;
			parameters[11].Value = searchCondition.Date_V_E;
			parameters[12].Value = searchCondition.ExpressCompanyID;
			parameters[13].Value = searchCondition.AccountType;
			parameters[14].Value = searchCondition.MerchantID;
            parameters[15].Value = searchCondition.IsDifference;
            parameters[16].Value = true;

			return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
		}

		public bool DeleteAccountNo(string accountNo, string updateBy)
		{
            strSql = @"UPDATE RFD_FMS.dbo.FMS_CODAccount SET DeleteFlag=1,UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange WHERE AccountNO=@AccountNO AND DeleteFlag=0
					   UPDATE RFD_FMS.dbo.FMS_CODAccountDetail SET DeleteFlag=1,UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange WHERE AccountNO=@AccountNO AND DeleteFlag=0
					   UPDATE RFD_FMS.dbo.FMS_CODDeliveryCount SET AccountNO='',UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange WHERE AccountNO=@AccountNO
					   UPDATE RFD_FMS.dbo.FMS_CODReturnsCount SET AccountNO='',UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange WHERE AccountNO=@AccountNO
					   UPDATE RFD_FMS.dbo.FMS_CODVisitReturnsCount SET AccountNO='',UpdateBy=@UpdateBy,UpdateTime=GETDATE(),IsChange=@IsChange WHERE AccountNO=@AccountNO";
			SqlParameter[] parameters = {
					                        new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20),
											new SqlParameter("@UpdateBy", SqlDbType.NVarChar, 40),
                                            new SqlParameter("@IsChange", SqlDbType.NVarChar, 40)
										};
			parameters[0].Value = accountNo;
			parameters[1].Value = updateBy;
            parameters[2].Value = true;

			return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
		}

        public DataTable SearchAccount(string auditStatus, string expressCompanyId, string accountDateS, string accountDateE, string accountNo, string merchantId, PageInfo pi, bool isPage)
		{
			#region sql
			StringBuilder str = new StringBuilder();
			if (isPage)
				str.Append(@"
SELECT @records=COUNT(1)
FROM   FMS_CODAccount(NOLOCK) a
	   INNER JOIN FMS_CODAccountDetail(NOLOCK) ad
			ON  A.AccountNO = ad.AccountNO
			AND ad.DataType = 2
			AND ad.DeleteFlag = 0
			AND a.DeleteFlag = 0
	   INNER JOIN StatusCodeInfo d(NOLOCK)
			ON  a.AccountStatus = d.CodeNo
			AND d.CodeType = 'CODAccount'
	   INNER JOIN RFD_PMS.dbo.ExpressCompany(NOLOCK) ec
			ON  ec.ExpressCompanyID = a.ExpressCompanyID
		INNER JOIN RFD_PMS.dbo.MerchantBaseInfo(NOLOCK) mbi
			ON  mbi.ID = a.MerchantID
WHERE  a.DeleteFlag=0 {0}

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

			str.Append(@"  SELECT  ROW_NUMBER() OVER ( ORDER BY a.AccountNO DESC ) AS 序号 ,
							   a.AccountNO as 结算单号,
							   mbi.MerchantName as 商家,
							   ec.CompanyName as 配送商,
							   a.AccountDate as 结算时间,
							   ad.AccountNum as 结算单量,
							   ad.Fare as 实际结算运费,
							   d.CodeDesc AS  结算状态,
							   ad.DeliveryNum as 普通发货数,
							   ad.DeliveryVNum as 上门换发货数,
							   ad.ReturnsNum as 普通拒收数,
							   ad.ReturnsVNum as 上门换拒收数,
							   ad.VisitReturnsNum as 上门退货订单数,
							   ad.DatumFare as 基准运费汇总,
							   ad.Allowance as 超区补助汇总,
							   ad.KPI as KPI考核汇总,
							   ad.POSPrice as POS机手续费汇总,
							   ad.StrandedPrice as 滞留扣款汇总,
							   ad.IntercityLose as 城际丢失扣款,
							   ad.OtherCost as 其他费用,
							   ad.CreateBy as 创建人,
							   ad.CreateTime as 创建时间,
							   ad.UpdateBy as 最后修改人,
							   ad.UpdateTime as 最后修改时间,
							   a.AuditBy as 审核人,
							   a.AuditTime as 审核时间
						FROM   FMS_CODAccount(NOLOCK) a
							   INNER JOIN FMS_CODAccountDetail(NOLOCK) ad
									ON  A.AccountNO = ad.AccountNO
									AND ad.DataType = 2
									AND ad.DeleteFlag = 0
									AND a.DeleteFlag = 0
							   INNER JOIN StatusCodeInfo d(NOLOCK)
									ON  a.AccountStatus = d.CodeNo
									AND d.CodeType = 'CODAccount'
							   INNER JOIN RFD_PMS.dbo.ExpressCompany(NOLOCK) ec
									ON  ec.ExpressCompanyID = a.ExpressCompanyID
								INNER JOIN RFD_PMS.dbo.MerchantBaseInfo(NOLOCK) mbi
									ON  mbi.ID = a.MerchantID
						WHERE  a.DeleteFlag=0 {0}");
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
            parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pi.PageSize });
            parameters.Add(new SqlParameter("@pageNo", SqlDbType.Int) { Value = pi.CurrentPageIndex });

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND a.AccountStatus=@AccountStatus ");
                parameters.Add(new SqlParameter("@AccountStatus", SqlDbType.Int) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND a.ExpressCompanyID=@ExpressCompanyID ");
                parameters.Add(new SqlParameter("@ExpressCompanyID", SqlDbType.Int) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(accountDateS))
            {
                sbWhere.Append(" AND a.AccountDate>=@AccountDateS ");
                parameters.Add(new SqlParameter("@AccountDateS", SqlDbType.DateTime) { Value = accountDateS });
            }

            if (!string.IsNullOrEmpty(accountDateE))
            {
                sbWhere.Append(" AND a.AccountDate<=@AccountDateE ");
                parameters.Add(new SqlParameter("@AccountDateE", SqlDbType.DateTime) { Value = accountDateE });
            }

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND a.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar,20) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND a.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
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

        public DataTable SearchAccountCondition(string accountNo, bool flag)
		{
			strSql = @"SELECT fc.AccountNO,
							   fc.ExpressCompanyID,
							   ec.CompanyName,
							   fc.AccountDate,
							   fc.AccountStatus,
							   fc.DeliveryDateStr,
							   fc.DeliveryDateEnd,
							   fc.ReturnsDateStr,
							   fc.ReturnsDateEnd,
							   fc.VisitReturnsDateStr,
							   fc.VisitReturnsDateEnd,
							   fc.DeliveryHouse,
							   fc.ReturnsHouse,
							   fc.VisitReturnsHouse,
							   fc.AccountType,
							   fc.MerchantID,
							   mbi.MerchantName,
                               fc.IsDifference
						FROM   FMS_CODAccount fc(NOLOCK)
						JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID = fc.ExpressCompanyID
						JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID = fc.MerchantID
						WHERE  fc.DeleteFlag = 0
							   {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fc.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20) { Value = accountNo });
            }

            strSql = string.Format(strSql, sbWhere.ToString());

			if(flag)
                return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray()).Tables[0];
			else
                return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters.ToArray()).Tables[0];
		}

        public DataTable SearchAccountDetail(string accountNo, string dataType, bool flag)
		{
			strSql = @"SELECT  fc.AccountDetailID ,
								fc.AccountNO ,
								fc.ExpressCompanyID ,
								fc.WareHouseID ,
								fc.AreaType ,
								fc.DeliveryNum ,
								fc.DeliveryVNum ,
								fc.ReturnsNum ,
								fc.ReturnsVNum ,
								fc.VisitReturnsNum ,
								fc.Formula ,
								fc.DatumFare ,
								fc.Allowance ,
								fc.KPI ,
								fc.POSPrice ,
								fc.StrandedPrice ,
								fc.IntercityLose ,
								fc.OtherCost ,
								fc.Fare ,
								m.MERCHANTNAME,
								ec.CompanyName,
								fc.DataType,
								fc.AccountNum
						FROM    FMS_CODAccountDetail (NOLOCK) fc
								JOIN RFD_PMS.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = fc.ExpressCompanyID
								JOIN RFD_PMS.dbo.MERCHANTBASEINFO AS m(NOLOCK) ON m.ID=fc.MerchantID
						WHERE   ( fc.DeleteFlag=0 ) {0}
						ORDER BY fc.WareHouseID ,
								fc.AreaType ,
								fc.DataType ASC ";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fc.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar,20) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(dataType))
            {
                sbWhere.Append(" AND fc.DataType=@DataType ");
                parameters.Add(new SqlParameter("@DataType", SqlDbType.Int) { Value = dataType });
            }

            strSql = string.Format(strSql, sbWhere.ToString());

			if (flag)
                return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters.ToArray()).Tables[0];
			else
                return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters.ToArray()).Tables[0];
		}

		public bool UpdateAccountDetailFee(CODAccountDetail accountDetail, string updateBy)
		{
			strSql = @"UPDATE  FMS_CODAccountDetail
								SET     Allowance = @Allowance ,
										KPI = @KPI ,
										POSPrice = @POSPrice ,
										StrandedPrice = @StrandedPrice ,
										IntercityLose = @IntercityLose ,
										OtherCost = @OtherCost ,
										Fare = DatumFare+@Fare ,
										UpdateBy = @UpdateBy ,
										UpdateTime = GETDATE(),
                                        IsChange=@IsChange
								WHERE   AccountNO = @AccountNO AND DataType=2";
			SqlParameter[] parameters = {
											new SqlParameter("@AccountNO",SqlDbType.NVarChar,20),
											new SqlParameter("@Allowance",SqlDbType.Decimal,18),
											new SqlParameter("@KPI",SqlDbType.Decimal,18),
											new SqlParameter("@POSPrice",SqlDbType.Decimal,18),
											new SqlParameter("@StrandedPrice",SqlDbType.Decimal,18),
											new SqlParameter("@IntercityLose",SqlDbType.Decimal,18),
											new SqlParameter("@OtherCost",SqlDbType.Decimal,18),
											new SqlParameter("@Fare",SqlDbType.Decimal,18),
											new SqlParameter("@UpdateBy",SqlDbType.NVarChar,40),
                                            new SqlParameter("@IsChange",SqlDbType.Bit)
										};
			parameters[0].Value = accountDetail.AccountNO;
			parameters[1].Value = accountDetail.Allowance;
			parameters[2].Value = accountDetail.KPI;
			parameters[3].Value = accountDetail.POSPrice;
			parameters[4].Value = accountDetail.StrandedPrice;
			parameters[5].Value = accountDetail.IntercityLose;
			parameters[6].Value = accountDetail.OtherCost;
			parameters[7].Value = accountDetail.Allowance + accountDetail.KPI + accountDetail.POSPrice + 
									accountDetail.StrandedPrice + accountDetail.IntercityLose + accountDetail.OtherCost;
			parameters[8].Value = updateBy;
            parameters[9].Value = true;

			return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
		}

		public bool UpdateAccountAuditStatus(string accountNo,int auditStatus,string updateBy)
		{
			strSql = @"UPDATE FMS_CODAccount
						SET    AccountStatus = @AccountStatus,
							   UpdateBy = @UpdateBy,
							   UpdateTime = GETDATE(),
							   AuditBy=@AuditBy,
							   AuditTime=GETDATE(),
                               IsChange=@IsChange
						WHERE  AccountNO = @AccountNO";
			SqlParameter[] parameters ={
										   new SqlParameter("@AccountNO",SqlDbType.NVarChar,20),
										   new SqlParameter("@AccountStatus",SqlDbType.Int),
										   new SqlParameter("@UpdateBy",SqlDbType.NVarChar,40),
										   new SqlParameter("@AuditBy",SqlDbType.NVarChar,40),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };
			parameters[0].Value = accountNo;
			parameters[1].Value = auditStatus;
			parameters[2].Value = updateBy;
			parameters[3].Value = updateBy;
            parameters[4].Value = true;

			return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
		}

        public DataTable GetAreaFare(string accountNo)
		{
			strSql = @"WITH t AS (
							  SELECT AreaType,
									 SUM(fc.DeliveryNum) AS DeliveryNum,
									 SUM(fc.DeliveryVNum) AS DeliveryVNum,
									 SUM(fc.ReturnsNum) AS ReturnsNum,
									 SUM(fc.ReturnsVNum) AS ReturnsVNum,
									 SUM(fc.VisitReturnsNum) AS VisitReturnsNum,
									 SUM(fc.AccountNum) AS AccountNum,
									 fc.Formula,
									 SUM(fc.DatumFare) AS DatumFare,
									 SUM(fc.Allowance) AS Allowance,
									 SUM(fc.KPI) AS KPI,
									 SUM(fc.POSPrice) AS POSPrice,
									 SUM(fc.StrandedPrice) AS StrandedPrice,
									 SUM(fc.IntercityLose) AS IntercityLose,
									 SUM(fc.OtherCost) AS OtherCost,
									 SUM(fc.Fare) AS Fare
							  FROM   FMS_CODAccountDetail fc(NOLOCK)
							  WHERE  fc.DataType = 0
									 AND fc.DeleteFlag=0
									 {0}
							  GROUP BY
									 fc.AreaType,
									 fc.Formula
							  UNION ALL
							  SELECT -1 AS AreaType,
									 DeliveryNum,
									 DeliveryVNum,
									 ReturnsNum,
									 ReturnsVNum,
									 VisitReturnsNum,
									 AccountNum,
									 '' AS Formula,
									 DatumFare,
									 Allowance,
									 KPI,
									 POSPrice,
									 StrandedPrice,
									 IntercityLose,
									 OtherCost,
									 Fare
							  FROM   FMS_CODAccountDetail fc(NOLOCK)
							  WHERE  fc.DataType = 2
									 AND fc.DeleteFlag=0
									 {0}
						  )
				SELECT *
				FROM   t";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fc.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20) { Value = accountNo });
            }

            strSql = string.Format(strSql, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnString, CommandType.Text, strSql, parameters.ToArray()).Tables[0];
		}

        public DataTable GetDetail(string searchType, CODSearchCondition searchCondition, ref PageInfo pi)
		{
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql=string.Empty;
            string sqlCondition = string.Empty;
            parameters.Add(new SqlParameter("@records", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pages", SqlDbType.Int) { Direction = ParameterDirection.Output });
            parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pi.PageSize });
            parameters.Add(new SqlParameter("@pageNo", SqlDbType.Int) { Value = pi.CurrentPageIndex });

            List<SqlParameter> parameterTmp = TransToSearchData(searchType, searchCondition, out sql,out sqlCondition, true);
            if (parameterTmp == null)
                return null;

            parameters.AddRange(parameterTmp);
            sql = string.Format(sql,sqlCondition);

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters.ToArray());
            pi.ItemCount = (int)parameters[0].Value;
            pi.PageCount = (int)parameters[1].Value;
			if (ds == null || ds.Tables.Count <= 0)
				return null;
			else
				return ds.Tables[0];
		}

        private string GetSearchExpressCompanyID(string expressCompanyId)
        {
            string expressCompanyChildsId = string.Empty;
            if (expressCompanyId == "11")
            {
                expressCompanyChildsId = CreateHouseXml(GetCompanyIdByRfd(), "ExpressCompanyID").InnerXml;
            }
            else
            {
                expressCompanyChildsId = CreateHouseXml(GetCompanyIdByTopCODCompanyID(expressCompanyId), "ExpressCompanyID").InnerXml;
            }

            return expressCompanyChildsId;
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
                if (houses[i].Trim().Contains("S_"))
                {
                    xe.SetAttribute("v", houses[i].Trim().Replace("S_", ""));
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

        /// <summary>
        /// 创建仓库查询xml
        /// </summary>
        /// <param name="dtHouse">数据表</param>
        /// <returns></returns>
        private XmlDocument CreateHouseXml(DataTable dtHouse, string columnName)
        {
            StringBuilder sbHouse = new StringBuilder();
            foreach (DataRow dr in dtHouse.Rows)
            {
                sbHouse.Append(dr[columnName].ToString() + ",");
            }

            return CreateHouseXml(sbHouse.ToString().TrimEnd(','));
        }

        private List<SqlParameter> TransToSearchData(string searchType, CODSearchCondition searchCondition,out string sql,out string sqlCondition, bool isPage)
        {
            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            string houseXml = string.Empty;
            if (!string.IsNullOrEmpty(searchCondition.MerchantID))
            {
                sbWhere.Append(" AND fcbi.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = searchCondition.MerchantID });
            }

            switch (searchType)
            {
                case "D":
                    sbWhere.Append(" AND fcbi.Flag=@Flag ");
                    parameters.Add(new SqlParameter("@Flag", SqlDbType.Int) { Value = 1 });

                    sbWhere.Append(" AND fcbi.WaybillType=@WaybillType ");
                    parameters.Add(new SqlParameter("@WaybillType", SqlDbType.NVarChar, 20) { Value = "0" });

                    sbWhere.Append(" AND fcbi.DeliverTime>=@DeliverTimeS ");
                    parameters.Add(new SqlParameter("@DeliverTimeS", SqlDbType.DateTime) { Value = searchCondition.Date_D_S.ToShortDateString() });

                    sbWhere.Append(" AND fcbi.DeliverTime<=@DeliverTimeE ");
                    parameters.Add(new SqlParameter("@DeliverTimeE", SqlDbType.DateTime) { Value = searchCondition.Date_D_E.AddDays(1).ToShortDateString() });

                    sql = GetDeliverDetailSql(true, searchType);
                    houseXml = CreateHouseXml(searchCondition.HouseD).InnerXml;
                    break;
                case "DV":
                    sbWhere.Append(" AND fcbi.Flag=@Flag ");
                    parameters.Add(new SqlParameter("@Flag", SqlDbType.Int) { Value = 1 });

                    sbWhere.Append(" AND fcbi.WaybillType=@WaybillType ");
                    parameters.Add(new SqlParameter("@WaybillType", SqlDbType.NVarChar, 20) { Value = "1" });

                    sbWhere.Append(" AND fcbi.DeliverTime>=@DeliverTimeS ");
                    parameters.Add(new SqlParameter("@DeliverTimeS", SqlDbType.DateTime) { Value = searchCondition.Date_D_S.ToShortDateString() });

                    sbWhere.Append(" AND fcbi.DeliverTime<=@DeliverTimeE ");
                    parameters.Add(new SqlParameter("@DeliverTimeE", SqlDbType.DateTime) { Value = searchCondition.Date_D_E.AddDays(1).ToShortDateString() });

                    sql = GetDeliverDetailSql(true, searchType);
                    houseXml = CreateHouseXml(searchCondition.HouseD).InnerXml;
                    break;
                case "R":
                    sbWhere.Append(" AND fcbi.Flag=@Flag ");
                    parameters.Add(new SqlParameter("@Flag", SqlDbType.Int) { Value = 0 });

                    sbWhere.Append(" AND fcbi.WaybillType=@WaybillType ");
                    parameters.Add(new SqlParameter("@WaybillType", SqlDbType.NVarChar, 20) { Value = "0" });

                    sbWhere.Append(" AND fcbi.ReturnTime>=@ReturnTimeS ");
                    parameters.Add(new SqlParameter("@ReturnTimeS", SqlDbType.DateTime) { Value = searchCondition.Date_R_S.ToShortDateString() });

                    sbWhere.Append(" AND fcbi.ReturnTime<=@ReturnTimeE ");
                    parameters.Add(new SqlParameter("@ReturnTimeE", SqlDbType.DateTime) { Value = searchCondition.Date_R_E.AddDays(1).ToShortDateString() });

                    sql = GetReturnsDetailSql(true, searchType);
                    houseXml = CreateHouseXml(searchCondition.HouseR).InnerXml;
                    break;
                case "RV":
                    sbWhere.Append(" AND fcbi.Flag=@Flag ");
                    parameters.Add(new SqlParameter("@Flag", SqlDbType.Int) { Value = 0 });

                    sbWhere.Append(" AND fcbi.WaybillType=@WaybillType ");
                    parameters.Add(new SqlParameter("@WaybillType", SqlDbType.NVarChar, 20) { Value = "1" });

                    sbWhere.Append(" AND fcbi.ReturnTime>=@ReturnTimeS ");
                    parameters.Add(new SqlParameter("@ReturnTimeS", SqlDbType.DateTime) { Value = searchCondition.Date_R_S.ToShortDateString() });

                    sbWhere.Append(" AND fcbi.ReturnTime<=@ReturnTimeE ");
                    parameters.Add(new SqlParameter("@ReturnTimeE", SqlDbType.DateTime) { Value = searchCondition.Date_R_E.AddDays(1).ToShortDateString() });

                    sql = GetReturnsDetailSql(true, searchType);
                    houseXml = CreateHouseXml(searchCondition.HouseR).InnerXml;
                    break;
                case "V":
                    sbWhere.Append(" AND fcbi.Flag=@Flag ");
                    parameters.Add(new SqlParameter("@Flag", SqlDbType.Int) { Value = 1 });

                    sbWhere.Append(" AND fcbi.WaybillType=@WaybillType ");
                    parameters.Add(new SqlParameter("@WaybillType", SqlDbType.NVarChar, 20) { Value = "2" });

                    sbWhere.Append(" AND fcbi.ReturnTime>=@ReturnTimeS ");
                    parameters.Add(new SqlParameter("@ReturnTimeS", SqlDbType.DateTime) { Value = searchCondition.Date_V_S.ToShortDateString() });

                    sbWhere.Append(" AND fcbi.ReturnTime<=@ReturnTimeE ");
                    parameters.Add(new SqlParameter("@ReturnTimeE", SqlDbType.DateTime) { Value = searchCondition.Date_V_E.AddDays(1).ToShortDateString() });

                    sql = GetVisitReturnsDetailSql(true, searchType);
                    houseXml = CreateHouseXml(searchCondition.HouseV).InnerXml;
                    break;
                default:
                    sql = "";
                    break;
            }
            if (string.IsNullOrEmpty(houseXml))
            {
                sql = "";
                sqlCondition = "";
                return null;
            }

            sbWhere.Append(@" AND EXISTS ( SELECT 1 FROM   @Ex e WHERE  e.ExpressCompanyID = fcbi.DeliverStationID) ");
            parameters.Add(new SqlParameter("@houseXml", SqlDbType.Xml) { Value = houseXml });

            parameters.Add(new SqlParameter("@searchType", SqlDbType.NVarChar) { Value = searchType });
            parameters.Add(new SqlParameter("@ExpressComapnyXml", SqlDbType.Xml) { Value = GetSearchExpressCompanyID(searchCondition.ExpressCompanyID) });

            sqlCondition = sbWhere.ToString();
            return parameters;
        }

        private string GetDeliverDetailSql(bool isPage, string searchType)
        {
            StringBuilder sbSql = new StringBuilder();
            #region 参数

            sbSql.Append(@"
DECLARE @Ex TABLE(ExpressCompanyID INT NULL )
INSERT  INTO @Ex( ExpressCompanyID) 
	SELECT  T.x.value('./@v', 'int') AS ExpressCompanyID
	FROM    @ExpressComapnyXml.nodes('/root/id') AS T ( x )
DECLARE @whTable TABLE
    (
      HouseID NVARCHAR(50) NULL,
      HouseType int NULL
    )
INSERT  INTO @whTable
        ( HouseID ,HouseType
        )
        SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,
				T.x.value('./@t', 'int') AS HouseType
        FROM    @houseXml.nodes('/root/id') AS T ( x )
");
            #endregion
            #region 总数
            if (isPage)
            {
                //总数
                sbSql.Append(@"
;WITH t AS (
SELECT  fcbi.WaybillNO,
		Warehouseid = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE CONVERT(NVARCHAR(20), fcbi.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), fcbi.FinalExpressCompanyID) END,
        WareHouseType = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ELSE 2 END
FROM    LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi(NOLOCK)
WHERE   fcbi.IsDeleted = 0
        {0}
        )
SELECT @records = COUNT(1) 
FROM t
WHERE EXISTS ( SELECT 1
                     FROM   @whTable wt
                     WHERE  t.WarehouseId = wt.HouseID AND wt.HouseType=t.WareHouseType )
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
SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1");
            }
            #endregion
            #region 明细子查询
            //明细
            sbSql.Append(@"
 ;WITH    t AS ( SELECT   fcbi.WaybillNO ,--订单号
                        fcbi.DeliverTime,--发货时间
                        Warehouseid = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE CONVERT(NVARCHAR(20), fcbi.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), fcbi.FinalExpressCompanyID) END,
						WareHouseType = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN 
							CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ELSE 2 END,
                        fcbi.DeliverStationID,--配送公司
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.AccountWeight,
                        fcbi.Fare,
                        fcbi.FareFormula,
                        fcbi.ADDRESS
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi(NOLOCK)
               WHERE    fcbi.IsDeleted = 0
                        {0}
             )");
            #endregion
            if (isPage)
            {
                sbSql.Append(@"
             SELECT * FROM (");
            }
            sbSql.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.DeliverTime DESC ) AS 序号 ,");

            if (searchType == "D")
                sbSql.Append(@" '普通发货' AS 类型 ,");
            else
                sbSql.Append(@" '上门换发货' AS 类型 ,");

            sbSql.Append(@"t.WaybillNO AS 订单号,
			mbi.MerchantName AS 商家,
			ec.CompanyName AS 配送商,
			发货仓库=CASE WHEN w.WarehouseName IS NULL THEN ec2.CompanyName ELSE w.WarehouseName END ,
			t.DeliverTime AS 发货时间,
			ael.AreaType AS 区域类型,
			t.AccountWeight AS 重量,
            t.Fare  AS 运费,
            t.FareFormula AS 计算公式,
            t.ADDRESS AS 地址
    FROM    t
            JOIN RFD_PMS.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = t.DeliverStationID
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi(NOLOCK) ON mbi.ID = t.MerchantID
            LEFT JOIN RFD_PMS.dbo.Warehouse(NOLOCK) w ON t.WarehouseId = w.WarehouseId AND t.WareHouseType=1
            LEFT JOIN RFD_PMS.dbo.ExpressCompany AS ec2(NOLOCK) ON ec2.ExpressCompanyID = t.WarehouseId 
																AND t.WareHouseType=2 
																AND ec2.CompanyFlag=1
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
			WHERE EXISTS ( SELECT 1
						 FROM   @whTable wt
						 WHERE  t.WarehouseId = wt.HouseID AND wt.HouseType=t.WareHouseType ) 
");
            if (isPage)
            {
                sbSql.Append(@"
            ) aa
    WHERE aa.序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1 ; 
");
            }

            return sbSql.ToString();
        }

        private string GetReturnsDetailSql(bool isPage, string searchType)
        {
            StringBuilder sbSql = new StringBuilder();
            #region 参数
            sbSql.Append(@"
DECLARE @Ex TABLE(ExpressCompanyID INT NULL )
INSERT  INTO @Ex( ExpressCompanyID) 
	SELECT  T.x.value('./@v', 'int') AS ExpressCompanyID
	FROM    @ExpressComapnyXml.nodes('/root/id') AS T ( x )
DECLARE @whTable TABLE
    (
      HouseID NVARCHAR(50) NULL,
      HouseType int NULL
    )
INSERT  INTO @whTable
        ( HouseID ,HouseType
        )
        SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,
				T.x.value('./@t', 'int') AS HouseType
        FROM    @houseXml.nodes('/root/id') AS T ( x )
");
            #endregion
            #region 总数 分页
            if (isPage)
            {
                //总数
                sbSql.Append(@"
;WITH t AS (
SELECT  fcbi.WaybillNO,
		ReturnWarehouseid = CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.ReturnWareHouseID
								ELSE CONVERT(NVARCHAR(20), fcbi.ReturnExpressCompanyID)
							END ,
        ReturnWareHouseType = CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END        
FROM    LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi(NOLOCK)
WHERE   fcbi.IsDeleted = 0
        {0}
        )
SELECT @records = COUNT(1) 
FROM t
WHERE EXISTS ( SELECT 1
                     FROM   @whTable wt
                     WHERE  t.ReturnWarehouseId = wt.HouseID AND wt.HouseType=t.ReturnWareHouseType )
                     
--PRINT @records
   
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
SET @rowStart = ( @pageNo - 1 ) * @pageSize + 1");
            }
            #endregion
            #region 明细 子查询
            sbSql.Append(@" ;                     
WITH    t AS ( SELECT   fcbi.WaybillNO ,--订单号
                        fcbi.DeliverTime,--发货时间
                        Warehouseid = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE CONVERT(NVARCHAR(20), fcbi.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), fcbi.FinalExpressCompanyID) END,
						WareHouseType = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN 
							CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ELSE 2 END,
                        fcbi.ReturnTime,
                        ReturnWarehouseid = CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.ReturnWareHouseID
								ELSE CONVERT(NVARCHAR(20), fcbi.ReturnExpressCompanyID)
							END ,
						ReturnWareHouseType = CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END,
                        fcbi.DeliverStationID,--配送公司
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.AccountWeight,
                        fcbi.Fare,
                        fcbi.FareFormula,
                        fcbi.ADDRESS
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi(NOLOCK)
               WHERE    fcbi.IsDeleted = 0
                        {0}
             )");
            #endregion
            if (isPage)
            {
                sbSql.Append(@"
             SELECT * FROM (");
            }
            sbSql.Append(@"
    SELECT  ROW_NUMBER() OVER ( ORDER BY t.DeliverTime DESC ) AS 序号 ,");

            if (searchType == "R")
                sbSql.Append(@" '普通拒收' AS 类型,");
            else if (searchType == "RV")
                sbSql.Append(@" '上门换拒收' AS 类型,");
            else if (searchType == "V")
                sbSql.Append(@" '上门退货' AS 类型,");

            sbSql.Append(@"t.WaybillNO AS 订单号,
			mbi.MerchantName AS 商家,
			ec.CompanyName AS 配送商,
			发货仓库=CASE WHEN w.WarehouseName IS NULL THEN ec2.CompanyName ELSE w.WarehouseName END ,
			t.ReturnTime AS 入库时间,
			入库仓库=CASE WHEN w1.WarehouseName IS NULL THEN ec3.CompanyName ELSE w1.WarehouseName END ,
			t.DeliverTime AS 发货时间,
			ael.AreaType AS 区域类型,
			t.AccountWeight  AS 重量,
            t.Fare  AS 运费,
            t.FareFormula AS 计算公式,
            t.ADDRESS AS 地址
    FROM    t
            JOIN RFD_PMS.dbo.ExpressCompany (NOLOCK) ec ON ec.ExpressCompanyID = t.DeliverStationID
            JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi(NOLOCK) ON mbi.ID = t.MerchantID
            LEFT JOIN RFD_PMS.dbo.Warehouse(NOLOCK) w ON t.WarehouseId = w.WarehouseId AND t.WareHouseType=1
            LEFT JOIN RFD_PMS.dbo.ExpressCompany AS ec2(NOLOCK) ON ec2.ExpressCompanyID = t.WarehouseId 
																AND t.WareHouseType=2 
																AND ec2.CompanyFlag=1
			LEFT JOIN RFD_PMS.dbo.Warehouse(NOLOCK) w1 ON t.ReturnWarehouseId = w1.WarehouseId AND t.ReturnWareHouseType=1
            LEFT JOIN RFD_PMS.dbo.ExpressCompany AS ec3(NOLOCK) ON ec3.ExpressCompanyID = t.ReturnWarehouseId 
																AND t.ReturnWareHouseType=2 
																AND ec3.CompanyFlag=1
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
			WHERE EXISTS ( SELECT 1
						 FROM   @whTable wt
						 WHERE  t.ReturnWarehouseId = wt.HouseID AND wt.HouseType=t.ReturnWareHouseType ) 
            ");
            if (isPage)
            {
                sbSql.Append(@"
            ) aa
    WHERE aa.序号 BETWEEN @rowStart AND @rowStart + @pageSize - 1 ; 
");
            }
            return sbSql.ToString();
        }

        private string GetVisitReturnsDetailSql(bool isPage, string searchType)
        {

            return GetReturnsDetailSql(isPage, searchType);
        }

        public DataTable GetExportDetail(string searchType, CODSearchCondition searchCondition, bool isDifference)
		{
            string sql = string.Empty;
            string sqlCondition=string.Empty;
            List<SqlParameter> parameter = TransToSearchData(searchType, searchCondition, out sql,out sqlCondition, true);
            if (parameter == null)
                return null;

            if (isDifference)
                sqlCondition += " AND fcbi.IsFare<>1";

            sql = string.Format(sql, sqlCondition);

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameter.ToArray());
			if (ds == null || ds.Tables.Count <= 0)
				return null;
			else
				return ds.Tables[0];
		}

		public DataTable GetErrorLog(CODSearchCondition condition)
		{
			strSql = @"
DECLARE @Ex TABLE(ExpressCompanyID INT NULL )
INSERT  INTO @Ex( ExpressCompanyID) 
	SELECT  T.x.value('./@v', 'int') AS ExpressCompanyID
	FROM    @ExpressComapnyXml.nodes('/root/id') AS T ( x )
DECLARE @whTableD TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL )
INSERT  INTO @whTableD( HouseID,HouseType) 
	SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
	FROM    @houseD.nodes('/root/id') AS T ( x )
DECLARE @whTableR TABLE( HouseID NVARCHAR(20) NULL,HouseType INT NULL)
INSERT  INTO @whTableR( HouseID,HouseType) 
	SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
	FROM    @houseR.nodes('/root/id') AS T ( x )
DECLARE @whTableV TABLE(HouseID NVARCHAR(20) NULL,HouseType INT NULL)
INSERT  INTO @whTableV( HouseID,HouseType) 
	SELECT  T.x.value('./@v', 'nvarchar(20)') AS HouseID,T.x.value('./@t', 'int') AS HouseType 
	FROM    @houseV.nodes('/root/id') AS T ( x ) ;
 
WITH    T AS ( SELECT   csl.StatisticsDate ,
						'发货' AS DeliveryType ,
                        csl.Reasons ,
                        csl.WareHouseID ,
                        csl.ExpressCompanyID
               FROM     RFD_FMS.dbo.FMS_CodStatsLog csl ( NOLOCK )
               WHERE    EXISTS (SELECT 1
                                     FROM   @Ex
                                     WHERE  csl.ExpressCompanyID =ExpressCompanyID)
                        AND csl.StatisticsDate >= @DateD_S
                        AND csl.StatisticsDate <= @DateD_E
                        AND csl.IsSuccess = 0
                        AND csl.StatisticsType = 1
                        AND EXISTS ( SELECT 1
                                     FROM   @whTableD
                                     WHERE  HouseID = csl.WareHouseID )
               UNION ALL
               SELECT   csl.StatisticsDate ,
						'拒收' AS DeliveryType ,
                        csl.Reasons ,
                        csl.WareHouseID ,
                        csl.ExpressCompanyID
               FROM     RFD_FMS.dbo.FMS_CodStatsLog csl ( NOLOCK )
               WHERE    EXISTS (SELECT 1
                                     FROM   @Ex
                                     WHERE  csl.ExpressCompanyID =ExpressCompanyID)
                        AND csl.StatisticsDate >= @DateR_S
                        AND csl.StatisticsDate <= @DateR_E
                        AND csl.IsSuccess = 0
                        AND csl.StatisticsType = 2
                        AND EXISTS ( SELECT 1
                                     FROM   @whTableR
                                     WHERE  HouseID = csl.WareHouseID )
               UNION ALL
               SELECT   csl.StatisticsDate ,
                        '上门退货' AS DeliveryType ,
                        csl.Reasons ,
                        csl.WareHouseID ,
                        csl.ExpressCompanyID
               FROM     RFD_FMS.dbo.FMS_CodStatsLog csl ( NOLOCK )
               WHERE    EXISTS (SELECT 1
                                     FROM   @Ex
                                     WHERE  csl.ExpressCompanyID =ExpressCompanyID)
                        AND csl.StatisticsDate >= @DateV_S
                        AND csl.StatisticsDate <= @DateV_E
                        AND csl.IsSuccess = 0
                        AND csl.StatisticsType = 3
                        AND EXISTS ( SELECT 1
                                     FROM   @whTableV
                                     WHERE  HouseID = csl.WareHouseID )
             )
    SELECT  t.DeliveryType ,
            t.StatisticsDate ,
            t.Reasons ,
            ec.CompanyName ,
            w.WarehouseName
    FROM    T t
            JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = t.ExpressCompanyID
            JOIN RFD_PMS.dbo.warehouse w ( NOLOCK ) ON w.WareHouseID = t.WareHouseID";

			SqlParameter[] parameters = {
			                            	new SqlParameter("@houseD",SqlDbType.Xml), 
											new SqlParameter("@houseR",SqlDbType.Xml), 
											new SqlParameter("@houseV",SqlDbType.Xml),
											new SqlParameter("@DateD_S", SqlDbType.Date),
											new SqlParameter("@DateD_E", SqlDbType.Date),
											new SqlParameter("@DateR_S", SqlDbType.Date),
											new SqlParameter("@DateR_E", SqlDbType.Date),
											new SqlParameter("@DateV_S", SqlDbType.Date),
											new SqlParameter("@DateV_E", SqlDbType.Date),
											new SqlParameter("@express", SqlDbType.Int),
											new SqlParameter("@ExpressComapnyXml", SqlDbType.Xml)
										};
			parameters[0].Value = condition.HouseD;
			parameters[1].Value = condition.HouseR;
			parameters[2].Value = condition.HouseV;
			parameters[3].Value = condition.Date_D_S;
			parameters[4].Value = condition.Date_D_E;
			parameters[5].Value = condition.Date_R_S;
			parameters[6].Value = condition.Date_R_E;
			parameters[7].Value = condition.Date_V_S;
			parameters[8].Value = condition.Date_V_E;
			parameters[9].Value = condition.ExpressCompanyID;
			parameters[10].Value = condition.ExpressCompanyChilds;
			DataSet ds= SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
			if (ds == null || ds.Tables.Count <= 0)
				return null;
			else
				return ds.Tables[0];
		}
    }
}
