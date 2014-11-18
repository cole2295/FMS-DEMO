using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using System.Data.SqlClient;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.COD;
using System.Xml;
using RFD.FMS.Util.OraclePageCommon;

namespace RFD.FMS.DAL.Oracle.COD
{
    public class CODAccountDao : OracleDao, ICODAccountDao
	{
        private string strSql = "";

        public DataTable GetCompanyIdByTopCODCompanyID(string topCodCompanyId)
        {
            strSql = @" SELECT  ec.ExpressCompanyID ,
        ec.CompanyName ,
        ec.ParentID ,
        TopCODCompanyID
FROM    ExpressCompany ec
WHERE   (1=1) {0}";
            strSql = string.Format(strSql,Util.Common.GetOracleInParameterWhereSql("ec.TopCODCompanyID","TopCODCompanyID",false,false));
            OracleParameter[] parameters ={
                                           new OracleParameter(":TopCODCompanyID",OracleDbType.Varchar2,2000)
                                      };
            parameters[0].Value = topCodCompanyId;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public DataTable GetCompanyIdByRfd()
        {
            strSql = @"SELECT  ec1.ExpressCompanyID ,
        ec1.CompanyName ,
        ec1.ParentID ,
        ec1.TopCODCompanyID
FROM    ExpressCompany ec1
WHERE   ec1.ParentID IN (
        SELECT  ec.ExpressCompanyID
        FROM    ExpressCompany ec
        WHERE   ec.CompanyFlag = 1
                AND ec.DistributionCode = 'rfd'
                AND ec.IsDeleted = 0
                AND ec.ParentID <> 11 )
        AND ec1.IsDeleted = 0";
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql).Tables[0];
        }

        public DataTable SearchUniteAccount(CODSearchCondition condition)
        {
            condition.ExpressCompanyChilds = GetSearchExpressCompanyID(condition.ExpressCompanyID);
            #region sql
            string sql = @"			WITH  groups AS ( 
                                       SELECT ExpressCompanyID ,AreaType ,SUM(FormCount) AS DeliveryNum ,SUM(Fare) AS Fare ,Formula ,'D' AS CounType,MerchantID
									   FROM   FMS_CODDeliveryCount  d
									   WHERE  AccountDate BETWEEN :DdateS AND :DdateE
											   AND AccountNO is null
											   AND DeliveryType=0
											   AND DeleteFlag = 0
                                               {0}
									   GROUP BY AreaType ,ExpressCompanyID ,Formula,MerchantID
									   UNION ALL
									   SELECT ExpressCompanyID,AreaType,SUM(FormCount) AS DeliveryNum,SUM(Fare) AS Fare,Formula, 'DV' AS CounType,MerchantID
									   FROM   FMS_CODDeliveryCount d
									   WHERE  AccountDate BETWEEN  :DdateS AND :DdateE
											   AND AccountNO is null
											   AND DeliveryType=1
											   AND DeleteFlag = 0
                                               {0}
										GROUP BY AreaType,ExpressCompanyID,Formula,MerchantID
									   UNION ALL
									   SELECT   ExpressCompanyID ,AreaType ,SUM(FormCount) AS ReturnsNum ,SUM(Fare) AS Fare ,Formula ,'R' AS CounType,MerchantID
									   FROM     FMS_CODReturnsCount  r
									   WHERE    AccountDate BETWEEN :RdateS AND :RdateE
												AND AccountNO is null
												AND ReturnsType=0
												AND DeleteFlag = 0
                                               {1}
									   GROUP BY AreaType ,ExpressCompanyID ,Formula,MerchantID
									   UNION ALL
										SELECT ExpressCompanyID,AreaType,SUM(FormCount) AS ReturnsNum,SUM(Fare) AS Fare,Formula,'RV' AS CounType,MerchantID
										FROM   FMS_CODReturnsCount r
										WHERE  AccountDate BETWEEN :RdateS AND :RdateE
											   AND AccountNO is null
											   AND ReturnsType=1
											   AND DeleteFlag = 0
                                               {1}
										GROUP BY AreaType,ExpressCompanyID,Formula,MerchantID
										UNION ALL
									   SELECT   ExpressCompanyID ,AreaType ,SUM(FormCount) AS VisitReturnsNum ,SUM(Fare) AS Fare ,Formula ,'V' AS CounType,MerchantID
									   FROM     FMS_CODVisitReturnsCount  v
									   WHERE    AccountDate BETWEEN :VdateS AND :VdateE
												AND AccountNO is null
												AND DeleteFlag = 0
                                               {2}
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
									0.00 AS IntercityLose ,0.00 AS CollectionFee ,0.00 AS DeliveryFee ,
                                    0.00 AS OtherCost ,0.00 AS Fare ,g.MerchantID,m.MERCHANTNAME,
									ec.accountcompanyname CompanyName,0 AS DataType,0 AS AccountNum
							FROM    groups g
							LEFT JOIN ExpressCompany  ec ON ec.ExpressCompanyID = g.ExpressCompanyID
							JOIN MERCHANTBASEINFO m ON g.MerchantID=m.ID
							GROUP BY g.ExpressCompanyID ,AreaType ,Formula,ec.accountcompanyname,m.MERCHANTNAME,g.MerchantID";
            #endregion
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":DdateS", OracleDbType.Date) { Value = condition.Date_D_S });
            parameterList.Add(new OracleParameter(":DdateE", OracleDbType.Date) { Value = condition.Date_D_E });
            parameterList.Add(new OracleParameter(":RdateS", OracleDbType.Date) { Value = condition.Date_R_S });
            parameterList.Add(new OracleParameter(":RdateE", OracleDbType.Date) { Value = condition.Date_R_E });
            parameterList.Add(new OracleParameter(":VdateS", OracleDbType.Date) { Value = condition.Date_V_S });
            parameterList.Add(new OracleParameter(":VdateE", OracleDbType.Date) { Value = condition.Date_V_E });
            parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Varchar2) { Value = condition.MerchantID });
            parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Varchar2) { Value = condition.ExpressCompanyChilds });
            
            StringBuilder sb_D = new StringBuilder();
            StringBuilder sb_R = new StringBuilder();
            StringBuilder sb_V = new StringBuilder();

            sb_D.Append(Util.Common.GetOracleInParameterWhereSql("d.MerchantID", "MerchantID", false, false));
            sb_R.Append(Util.Common.GetOracleInParameterWhereSql("r.MerchantID", "MerchantID", false, false));
            sb_V.Append(Util.Common.GetOracleInParameterWhereSql("v.MerchantID", "MerchantID", false, false));

            sb_D.Append(Util.Common.GetOracleInParameterWhereSql("d.ExpressCompanyID", "ExpressCompanyID", false, false));
            sb_R.Append(Util.Common.GetOracleInParameterWhereSql("r.ExpressCompanyID", "ExpressCompanyID", false, false));
            sb_V.Append(Util.Common.GetOracleInParameterWhereSql("v.ExpressCompanyID", "ExpressCompanyID", false, false));

            sb_D.Append(CreateHouseCondition(condition.HouseD, "d.WareHouseID", "d.WareHouseID", "d.WareHouseType", "WareHouseID_D", ref parameterList));
            sb_R.Append(CreateHouseCondition(condition.HouseR, "r.WareHouseID", "r.WareHouseID", "r.WareHouseType", "WareHouseID_R", ref parameterList));
            sb_V.Append(CreateHouseCondition(condition.HouseV, "v.WareHouseID", "v.WareHouseID", "v.WareHouseType", "WareHouseID_V", ref parameterList));

            sql = string.Format(sql, sb_D.ToString(), sb_R.ToString(), sb_V.ToString());

            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, ToParameters(parameterList.ToArray())).Tables[0];
        }

        public DataTable GetChanageCountList(CODSearchCondition searchCondition)
        {
            searchCondition.ExpressCompanyChilds = GetSearchExpressCompanyID(searchCondition.ExpressCompanyID);
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Varchar2) { Value = searchCondition.MerchantID });
            parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Varchar2) { Value = searchCondition.ExpressCompanyChilds });

            #region sql
            string sql = string.Empty;
            if (searchCondition.CountType == "D")
            {
                parameterList.Add(new OracleParameter(":DdateS", OracleDbType.Date) { Value = searchCondition.Date_D_S });
                parameterList.Add(new OracleParameter(":DdateE", OracleDbType.Date) { Value = searchCondition.Date_D_E });
                StringBuilder sb_D = new StringBuilder();
                sb_D.Append(Util.Common.GetOracleInParameterWhereSql("MerchantID", "MerchantID", false, false));
                sb_D.Append(Util.Common.GetOracleInParameterWhereSql("ExpressCompanyID", "ExpressCompanyID", false, false));
                sb_D.Append(CreateHouseCondition(searchCondition.HouseD, "WareHouseID", "WareHouseID", "WareHouseType", "WareHouseID_D", ref parameterList));
                sql = @"SELECT AccountID FROM FMS_CODDeliveryCount 
                            WHERE AccountDate BETWEEN :DdateS AND :DdateE
							AND AccountNO is null
							AND DeleteFlag = 0
                            {0}";
                sql = string.Format(sql, sb_D.ToString());
            }
            else if (searchCondition.CountType == "R")
            {
                parameterList.Add(new OracleParameter(":RdateS", OracleDbType.Date) { Value = searchCondition.Date_R_S });
                parameterList.Add(new OracleParameter(":RdateE", OracleDbType.Date) { Value = searchCondition.Date_R_E });
                StringBuilder sb_R = new StringBuilder();
                sb_R.Append(Util.Common.GetOracleInParameterWhereSql("MerchantID", "MerchantID", false, false));
                sb_R.Append(Util.Common.GetOracleInParameterWhereSql("ExpressCompanyID", "ExpressCompanyID", false, false));
                sb_R.Append(CreateHouseCondition(searchCondition.HouseR, "WareHouseID", "WareHouseID", "WareHouseType", "WareHouseID_R", ref parameterList));
                sql = @"SELECT AccountID FROM FMS_CODReturnsCount 
                            WHERE AccountDate BETWEEN :RdateS AND :RdateE
							AND AccountNO is null
							AND DeleteFlag = 0
                            {0}";
                sql = string.Format(sql, sb_R.ToString());
            }
            else if (searchCondition.CountType == "V")
            {
                parameterList.Add(new OracleParameter(":VdateS", OracleDbType.Date) { Value = searchCondition.Date_V_S });
                parameterList.Add(new OracleParameter(":VdateE", OracleDbType.Date) { Value = searchCondition.Date_V_E });
                StringBuilder sb_V = new StringBuilder();
                sb_V.Append(Util.Common.GetOracleInParameterWhereSql("MerchantID", "MerchantID", false, false));
                sb_V.Append(Util.Common.GetOracleInParameterWhereSql("ExpressCompanyID", "ExpressCompanyID", false, false));
                sb_V.Append(CreateHouseCondition(searchCondition.HouseV, "WareHouseID", "WareHouseID", "WareHouseType", "WareHouseID_V", ref parameterList));
                sql = @"SELECT AccountID FROM FMS_CODVisitReturnsCount 
                            WHERE AccountDate BETWEEN :VdateS AND :VdateE
							AND AccountNO is null
							AND DeleteFlag = 0
                            {0}";
                sql = string.Format(sql, sb_V.ToString());
            }

            #endregion

            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, ToParameters(parameterList.ToArray())).Tables[0];
        }

        public bool BatchChangeCountAccountNO(string accountIds, string createBy, string AccountNo, string tableName)
        {
            string sql = @"UPDATE  {0}
						SET     AccountNO = :AccountNO ,
								UpdateBy = :UpdateBy ,
								UpdateTime = SysDate
						WHERE   (1=1) {1}";
            sql = string.Format(sql, tableName, Util.Common.GetOracleInParameterWhereSql("AccountID", "AccountIDs", false, false));
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":UpdateBy", OracleDbType.Varchar2) { Value = createBy });
            parameterList.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2) { Value = AccountNo });
            parameterList.Add(new OracleParameter(":AccountIDs", OracleDbType.Varchar2) { Value = accountIds });

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, ToParameters(parameterList.ToArray()))>0;
        }

        public bool ChanageCountAcountNO(CODSearchCondition searchCondition, string createBy, string accountNo)
        {
            searchCondition.ExpressCompanyChilds = GetSearchExpressCompanyID(searchCondition.ExpressCompanyID);
            #region sql
            string sql = @"begin
                        :n :=0;
                        UPDATE  FMS_CODDeliveryCount
						SET     AccountNO = :AccountNO ,
								UpdateBy = :UpdateBy ,
								UpdateTime = SysDate
						WHERE   AccountDate BETWEEN :DdateS AND :DdateE
								AND MerchantID = :MerchantID
								AND AccountNO = ''
								AND DeleteFlag = 0
                                {0};
                        :n := :n+sql%rowcount;
						UPDATE  FMS_CODReturnsCount
						SET     AccountNO = :AccountNO ,
								UpdateBy = :UpdateBy ,
								UpdateTime = SysDate
						WHERE   AccountDate BETWEEN :RdateS AND :RdateE
								AND MerchantID = :MerchantID
								AND AccountNO = ''
								AND DeleteFlag = 0
                                {1};
                        :n := :n+sql%rowcount;
						UPDATE  FMS_CODVisitReturnsCount
						SET     AccountNO = :AccountNO ,
								UpdateBy = :UpdateBy ,
								UpdateTime = SysDate
						WHERE   AccountDate BETWEEN :VdateS AND :VdateE
								AND MerchantID = :MerchantID
								AND AccountNO = ''
								AND DeleteFlag = 0
                                {2};
                        :n := :n+sql%rowcount;
                        end;";
            #endregion

            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":n", OracleDbType.Decimal) { Direction = ParameterDirection.Output });
            parameterList.Add(new OracleParameter(":UpdateBy", OracleDbType.Varchar2) { Value = createBy });
            parameterList.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2) { Value = accountNo });
            parameterList.Add(new OracleParameter(":DdateS", OracleDbType.Date) { Value = searchCondition.Date_D_S });
            parameterList.Add(new OracleParameter(":DdateE", OracleDbType.Date) { Value = searchCondition.Date_D_E });
            parameterList.Add(new OracleParameter(":RdateS", OracleDbType.Date) { Value = searchCondition.Date_R_S });
            parameterList.Add(new OracleParameter(":RdateE", OracleDbType.Date) { Value = searchCondition.Date_R_E });
            parameterList.Add(new OracleParameter(":VdateS", OracleDbType.Date) { Value = searchCondition.Date_V_S });
            parameterList.Add(new OracleParameter(":VdateE", OracleDbType.Date) { Value = searchCondition.Date_V_E });
            parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Varchar2) { Value = searchCondition.MerchantID });
            parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Varchar2) { Value = searchCondition.ExpressCompanyChilds });

            StringBuilder sb_D = new StringBuilder();
            StringBuilder sb_R = new StringBuilder();
            StringBuilder sb_V = new StringBuilder();

            sb_D.Append(Util.Common.GetOracleInParameterWhereSql("MerchantID", "MerchantID", false, false));
            sb_R.Append(Util.Common.GetOracleInParameterWhereSql("MerchantID", "MerchantID", false, false));
            sb_V.Append(Util.Common.GetOracleInParameterWhereSql("MerchantID", "MerchantID", false, false));

            sb_D.Append(Util.Common.GetOracleInParameterWhereSql("ExpressCompanyID", "ExpressCompanyID", false, false));
            sb_R.Append(Util.Common.GetOracleInParameterWhereSql("ExpressCompanyID", "ExpressCompanyID", false, false));
            sb_V.Append(Util.Common.GetOracleInParameterWhereSql("ExpressCompanyID", "ExpressCompanyID", false, false));

            sb_D.Append(CreateHouseCondition(searchCondition.HouseD, "WareHouseID", "WareHouseID", "WareHouseType", "WareHouseID_D", ref parameterList));
            sb_R.Append(CreateHouseCondition(searchCondition.HouseR, "WareHouseID", "WareHouseID", "WareHouseType", "WareHouseID_R", ref parameterList));
            sb_V.Append(CreateHouseCondition(searchCondition.HouseV, "WareHouseID", "WareHouseID", "WareHouseType", "WareHouseID_V", ref parameterList));

            sql = string.Format(sql, sb_D.ToString(), sb_R.ToString(), sb_V.ToString());

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, ToParameters(parameterList.ToArray()));
            int n = DataConvert.ToInt(parameterList[0].Value);
            return n>=0;
        }

        public bool AddAccountDetail(DataRow dr, string createBy, string accountNo)
        {
            #region sql
            strSql = @"INSERT  INTO FMS_CODAccountDetail
									(   AccountDetailID,
                                        AccountNO ,
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
                                        CollectionFee,
                                        DeliveryFee,
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
                                        DeleteFlag
									)
							VALUES  ( 
                                        :AccountDetailID,
                                        :AccountNO ,
                                        :ExpressCompanyID ,
                                        :AreaType ,
                                        :DeliveryNum ,
                                        :DeliveryVNum ,
                                        :ReturnsNum ,
                                        :ReturnsVNum ,
                                        :VisitReturnsNum ,
                                        :Formula ,
                                        :DatumFare ,
                                        :Allowance,
                                        :KPI,
                                        :POSPrice,
                                        :StrandedPrice,
                                        :IntercityLose,
                                        :CollectionFee,
                                        :DeliveryFee,
                                        :OtherCost,
                                        :Fare,
                                        :DataType,
                                        :CreateBy ,
                                        SysDate ,
                                        :UpdateBy ,
                                        SysDate,
                                        :MerchantID,
                                        :WareHouseID,
                                        :AccountNum,
                                        :DeleteFlag
									)";
            #endregion
            OracleParameter[] parameters = {
			                             	new OracleParameter(":AccountNO", OracleDbType.Varchar2,40),
			                             	new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal),
			                             	new OracleParameter(":AreaType", OracleDbType.Decimal),
			                             	new OracleParameter(":DeliveryNum", OracleDbType.Decimal),
											new OracleParameter(":DeliveryVNum", OracleDbType.Decimal),
			                             	new OracleParameter(":ReturnsNum", OracleDbType.Decimal),
			                             	new OracleParameter(":ReturnsVNum", OracleDbType.Decimal),
			                             	new OracleParameter(":VisitReturnsNum", OracleDbType.Decimal),
			                             	new OracleParameter(":Formula", OracleDbType.Varchar2,200),
			                             	new OracleParameter(":DatumFare", OracleDbType.Decimal,18),
											new OracleParameter(":Allowance", OracleDbType.Decimal,18),
											new OracleParameter(":KPI", OracleDbType.Decimal,18),
											new OracleParameter(":POSPrice", OracleDbType.Decimal,18),
											new OracleParameter(":StrandedPrice", OracleDbType.Decimal,18),
											new OracleParameter(":IntercityLose", OracleDbType.Decimal,18),
                                            new OracleParameter(":CollectionFee", OracleDbType.Decimal,18),
                                            new OracleParameter(":DeliveryFee", OracleDbType.Decimal,18),
                                            new OracleParameter(":OtherCost", OracleDbType.Decimal,18),
											new OracleParameter(":Fare", OracleDbType.Decimal,18),
											new OracleParameter(":DataType", OracleDbType.Decimal),
			                             	new OracleParameter(":CreateBy", OracleDbType.Varchar2,80),
											new OracleParameter(":MerchantID", OracleDbType.Decimal),
											new OracleParameter(":WareHouseID", OracleDbType.Varchar2,40),
											new OracleParameter(":AccountNum", OracleDbType.Decimal),
                                            new OracleParameter(":AccountDetailID", OracleDbType.Decimal),
                                            new OracleParameter(":UpdateBy", OracleDbType.Varchar2,80),
                                            new OracleParameter(":DeleteFlag", OracleDbType.Decimal),
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
            parameters[15].Value = dr["CollectionFee"];
            parameters[16].Value = dr["DeliveryFee"];
            parameters[17].Value = dr["OtherCost"];
            parameters[18].Value = dr["Fare"];
            parameters[19].Value = dr["DataType"];
            parameters[20].Value = createBy;
            parameters[21].Value = dr["MerchantID"];
            parameters[22].Value = "0";
            parameters[23].Value = dr["AccountNum"];
            parameters[24].Value = GetIdNew("SEQ_FMS_CODACCOUNTDETAIL");
            parameters[25].Value = createBy;
            parameters[26].Value = 0;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public bool AddAccount(CODSearchCondition searchCondition, string createBy, string accountNo)
        {
            #region sql
            strSql = @"INSERT  INTO FMS_CODAccount
								( 
                                    AccountID,
                                    AccountNO ,
                                    ExpressCompanyID,
								    ExpressCompanyIDs ,
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
									MerchantIDs,
                                    IsDifference,
                                    DeleteFlag
								)
						VALUES  ( 
                                    :AccountID,
                                    :AccountNO ,
                                    :ExpressCompanyID,
                                    :ExpressCompanyIDs ,
                                    SysDate ,
                                    :AccountStatus ,
                                    :CreateBy ,
                                    SysDate ,
                                    '' ,
                                    SysDate ,
                                    '' ,
                                    SysDate,
									:DeliveryDateStr,
									:DeliveryDateEnd,
									:ReturnsDateStr,
									:ReturnsDateEnd,
									:VisitReturnsDateStr,
									:VisitReturnsDateEnd,
									:DeliveryHouse,
									:ReturnsHouse,
									:VisitReturnsHouse,
									:AccountType,
									:MerchantID,
                                    :IsDifference,
                                    :DeleteFlag
								)";
            #endregion
            OracleParameter[] parameters = {
					                             	new OracleParameter(":AccountNO", OracleDbType.Varchar2,40),
													new OracleParameter(":AccountStatus", OracleDbType.Decimal),
					                             	new OracleParameter(":CreateBy", OracleDbType.Varchar2,80),
													new OracleParameter(":DeliveryHouse", OracleDbType.Varchar2,2000),
													new OracleParameter(":ReturnsHouse", OracleDbType.Varchar2,2000),
													new OracleParameter(":VisitReturnsHouse", OracleDbType.Varchar2,2000),
													new OracleParameter(":DeliveryDateStr", OracleDbType.Date),
													new OracleParameter(":DeliveryDateEnd", OracleDbType.Date),
													new OracleParameter(":ReturnsDateStr", OracleDbType.Date),
													new OracleParameter(":ReturnsDateEnd", OracleDbType.Date),
													new OracleParameter(":VisitReturnsDateStr", OracleDbType.Date),
													new OracleParameter(":VisitReturnsDateEnd", OracleDbType.Date),
													new OracleParameter(":ExpressCompanyIDs", OracleDbType.Varchar2,2000),
													new OracleParameter(":AccountType", OracleDbType.Decimal),
													new OracleParameter(":MerchantID", OracleDbType.Varchar2,2000),
                                                    new OracleParameter(":IsDifference", OracleDbType.Decimal),
                                                    new OracleParameter(":AccountID", OracleDbType.Decimal),
                                                    new OracleParameter(":DeleteFlag", OracleDbType.Decimal),
                                                    new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal),
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
            parameters[16].Value = GetIdNew("SEQ_FMS_CODACCOUNT");
            parameters[17].Value = 0;
            parameters[18].Value = 0;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public bool DeleteAccountNo(string accountNo, string updateBy)
        {
            strSql = @"
                        begin
                            :n := 0;
                            UPDATE FMS_CODAccount SET DeleteFlag=1,UpdateBy=:UpdateBy,UpdateTime=SysDate WHERE AccountNO=:AccountNO AND DeleteFlag=0;
                            :n := :n+sql%rowcount;
					        UPDATE FMS_CODAccountDetail SET DeleteFlag=1,UpdateBy=:UpdateBy,UpdateTime=SysDate WHERE AccountNO=:AccountNO AND DeleteFlag=0;
                            :n := :n+sql%rowcount;
					        UPDATE FMS_CODDeliveryCount SET AccountNO='',UpdateBy=:UpdateBy,UpdateTime=SysDate WHERE AccountNO=:AccountNO;
                            :n := :n+sql%rowcount;
					        UPDATE FMS_CODReturnsCount SET AccountNO='',UpdateBy=:UpdateBy,UpdateTime=SysDate WHERE AccountNO=:AccountNO;
                            :n := :n+sql%rowcount;
					        UPDATE FMS_CODVisitReturnsCount SET AccountNO='',UpdateBy=:UpdateBy,UpdateTime=SysDate WHERE AccountNO=:AccountNO;
                            :n := :n+sql%rowcount;
                        end;";
            OracleParameter[] parameters = {
                                                new OracleParameter(":n", OracleDbType.Decimal),
					                            new OracleParameter(":AccountNO", OracleDbType.Varchar2,40),
											    new OracleParameter(":UpdateBy", OracleDbType.Varchar2,80)
                                           };
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = accountNo;
            parameters[2].Value = updateBy;
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters);
            int n = DataConvert.ToInt(parameters[0].Value);
            return n >= 0;
        }

        public DataTable SearchAccount(string auditStatus, string expressCompanyId, string accountDateS, string accountDateE, string accountNo, string merchantId, PageInfo pi, bool isPage)
        {

            string countSql=@"SELECT COUNT(1)
FROM   FMS_CODAccount a
	   INNER JOIN FMS_CODAccountDetail ad
			ON  A.AccountNO = ad.AccountNO
			AND ad.DataType = 2
			AND ad.DeleteFlag = 0
			AND a.DeleteFlag = 0
	   INNER JOIN StatusCodeInfo d
			ON  a.AccountStatus = d.CodeNo
			AND d.CodeType = 'CODAccount'
WHERE  a.DeleteFlag=0 {0}";

            StringBuilder str = new StringBuilder();

            str.Append(@"  SELECT  ROWNUM AS 序号 ,
							   a.AccountNO as 结算单号,
							   case when mbi.MerchantName is null then '其他' else mbi.MerchantName end 商家,
							   (select accountcompanyname from expresscompany 
                        where expresscompanyid in (select regexp_substr(a.ExpressCompanyIDs,'[^,]+',1,level) as value_id from dual connect by level<=length(trim(translate(a.ExpressCompanyIDs,translate(a.ExpressCompanyIDs,',',' '),' ')))+1) 
                        and companyflag=3 and rownum=1 ) 配送商,
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
                               ad.CollectionFee as 代收手续费,
							   ad.DeliveryFee as 投递费,
							   ad.OtherCost as 其他费用,
							   ad.CreateBy as 创建人,
							   ad.CreateTime as 创建时间,
							   ad.UpdateBy as 最后修改人,
							   ad.UpdateTime as 最后修改时间,
							   a.AuditBy as 审核人,
							   a.AuditTime as 审核时间
						FROM   FMS_CODAccount a
							   INNER JOIN FMS_CODAccountDetail ad
									ON  A.AccountNO = ad.AccountNO
									AND ad.DataType = 2
									AND ad.DeleteFlag = 0
									AND a.DeleteFlag = 0
							   INNER JOIN StatusCodeInfo d
									ON  a.AccountStatus = d.CodeNo
									AND d.CodeType = 'CODAccount'
							   LEFT JOIN MerchantBaseInfo mbi
									ON  mbi.ID =  case when instr(a.MerchantIDs,',')>0 then 0 else cast(a.MerchantIDs as number) end
						WHERE  a.DeleteFlag=0 {0} ");

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND a.AccountStatus=:AccountStatus ");
                parameters.Add(new OracleParameter(":AccountStatus", OracleDbType.Decimal) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND a.ExpressCompanyIDs=:ExpressCompanyIDs ");
                parameters.Add(new OracleParameter(":ExpressCompanyIDs", OracleDbType.Varchar2) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(accountDateS))
            {
                sbWhere.Append(" AND a.AccountDate>=:AccountDateS ");
                parameters.Add(new OracleParameter(":AccountDateS", OracleDbType.Date) { Value = DataConvert.ToDateTime(accountDateS) });
            }

            if (!string.IsNullOrEmpty(accountDateE))
            {
                sbWhere.Append(" AND a.AccountDate<:AccountDateE ");
                parameters.Add(new OracleParameter(":AccountDateE", OracleDbType.Date) { Value =  DataConvert.ToDateTime(accountDateE) });
            }

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND a.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 20) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND a.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
            }

            strSql = string.Format(str.ToString(), sbWhere.ToString());
            countSql = string.Format(countSql, sbWhere.ToString());            

            if (isPage)
            {
                IPagedDataTable aa = PageCommon.GetPagedData(ReadOnlyConnection, strSql,countSql, " a.AccountNO DESC", new PaginatorDTO { PageSize = pi.PageSize, PageNo = pi.CurrentPageIndex },
                                                       this.ToParameters(parameters.ToArray()));
                pi.ItemCount = DataConvert.ToInt(aa.RecordCount);
                pi.PageCount = DataConvert.ToInt(aa.PageCount);
                return aa.ContentData;
            }

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, ToParameters(parameters.ToArray()));

            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable SearchAccountCondition(string accountNo, bool flag)
        {
            strSql = @"SELECT fc.AccountNO,
							   fc.ExpressCompanyIDs,
							   (select accountcompanyname from expresscompany 
                        where expresscompanyid in (select regexp_substr(fc.ExpressCompanyIDs,'[^,]+',1,level) as value_id from dual connect by level<=length(trim(translate(fc.ExpressCompanyIDs,translate(fc.ExpressCompanyIDs,',',' '),' ')))+1) 
                        and companyflag=3 and rownum=1) DisplayCompanyName,
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
							   fc.MerchantIDs,
                               fc.IsDifference
						FROM   FMS_CODAccount fc
						WHERE  fc.DeleteFlag = 0
							   {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fc.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 40) { Value = accountNo });
            }

            strSql = string.Format(strSql, sbWhere.ToString());

            if (flag)
                return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, this.ToParameters(parameters.ToArray())).Tables[0];
            else
                return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, this.ToParameters(parameters.ToArray())).Tables[0];
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
								ec.accountcompanyname CompanyName,
								fc.DataType,
								fc.AccountNum,
                                fc.CollectionFee,
                                fc.DeliveryFee
						FROM    FMS_CODAccountDetail  fc
								LEFT JOIN ExpressCompany  ec ON ec.ExpressCompanyID = fc.ExpressCompanyID
								LEFT JOIN MERCHANTBASEINFO m ON m.ID=fc.MerchantID
						WHERE   ( fc.DeleteFlag=0 ) {0}
						ORDER BY fc.WareHouseID ,
								fc.AreaType ,
								fc.DataType ASC ";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fc.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 40) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(dataType))
            {
                sbWhere.Append(" AND fc.DataType=:DataType ");
                parameters.Add(new OracleParameter(":DataType", OracleDbType.Decimal) { Value = dataType });
            }

            strSql = string.Format(strSql, sbWhere.ToString());

            if (flag)
                return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, this.ToParameters(parameters.ToArray())).Tables[0];
            else
                return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public bool UpdateAccountDetailFee(CODAccountDetail accountDetail, string updateBy)
        {
            strSql = @"UPDATE  FMS_CODAccountDetail
								SET     Allowance = :Allowance ,
										KPI = :KPI ,
										POSPrice = :POSPrice ,
										StrandedPrice = :StrandedPrice ,
										IntercityLose = :IntercityLose ,
                                        CollectionFee = :CollectionFee,
                                        DeliveryFee = :DeliveryFee,
										OtherCost = :OtherCost ,
										Fare = DatumFare+:Fare ,
										UpdateBy = :UpdateBy ,
										UpdateTime = SysDate
								WHERE   AccountNO = :AccountNO AND DataType=2";
            OracleParameter[] parameters = {
											new OracleParameter(":AccountNO",OracleDbType.Varchar2,40),
											new OracleParameter(":Allowance",OracleDbType.Decimal,18),
											new OracleParameter(":KPI",OracleDbType.Decimal,18),
											new OracleParameter(":POSPrice",OracleDbType.Decimal,18),
											new OracleParameter(":StrandedPrice",OracleDbType.Decimal,18),
											new OracleParameter(":IntercityLose",OracleDbType.Decimal,18),
                                            new OracleParameter(":CollectionFee",OracleDbType.Decimal,18),
                                            new OracleParameter(":DeliveryFee",OracleDbType.Decimal,18),
											new OracleParameter(":OtherCost",OracleDbType.Decimal,18),
											new OracleParameter(":Fare",OracleDbType.Decimal,18),
											new OracleParameter(":UpdateBy",OracleDbType.Varchar2,80)
										};
            parameters[0].Value = accountDetail.AccountNO;
            parameters[1].Value = accountDetail.Allowance;
            parameters[2].Value = accountDetail.KPI;
            parameters[3].Value = accountDetail.POSPrice;
            parameters[4].Value = accountDetail.StrandedPrice;
            parameters[5].Value = accountDetail.IntercityLose;
            parameters[6].Value = accountDetail.CollectionFee;
            parameters[7].Value = accountDetail.DeliveryFee;
            parameters[8].Value = accountDetail.OtherCost;
            parameters[9].Value = accountDetail.Allowance + accountDetail.KPI + accountDetail.POSPrice +
                                    accountDetail.StrandedPrice + accountDetail.IntercityLose + accountDetail.CollectionFee
                                    +accountDetail.DeliveryFee+accountDetail.OtherCost;
            parameters[10].Value = updateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public bool UpdateAccountAuditStatus(string accountNo, int auditStatus, string updateBy)
        {
            strSql = @"UPDATE FMS_CODAccount
						SET    AccountStatus = :AccountStatus,
							   UpdateBy = :UpdateBy,
							   UpdateTime = SysDate,
							   AuditBy=:AuditBy,
							   AuditTime=SysDate
						WHERE  AccountNO = :AccountNO";
            OracleParameter[] parameters ={
										   new OracleParameter(":AccountNO",OracleDbType.Varchar2,40),
										   new OracleParameter(":AccountStatus",OracleDbType.Decimal),
										   new OracleParameter(":UpdateBy",OracleDbType.Varchar2,80),
										   new OracleParameter(":AuditBy",OracleDbType.Varchar2,80),
									  };
            parameters[0].Value = accountNo;
            parameters[1].Value = auditStatus;
            parameters[2].Value = updateBy;
            parameters[3].Value = updateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
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
                                     SUM(fc.CollectionFee) AS CollectionFee,
                                     SUM(fc.DeliveryFee) AS DeliveryFee,
									 SUM(fc.OtherCost) AS OtherCost,
									 SUM(fc.Fare) AS Fare
							  FROM   FMS_CODAccountDetail fc
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
                                     CollectionFee,
                                     DeliveryFee,
									 OtherCost,
									 Fare
							  FROM   FMS_CODAccountDetail fc
							  WHERE  fc.DataType = 2
									 AND fc.DeleteFlag=0
									 {0}
						  )
				SELECT *
				FROM   t";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fc.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 40) { Value = accountNo });
            }

            strSql = string.Format(strSql, sbWhere.ToString());
            return OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public DataTable GetDetail(string searchType, CODSearchCondition searchCondition, ref PageInfo pi)
        {
            string sql = string.Empty;
            string sqlCondition = string.Empty;
            string houseCondition = string.Empty;
            string countSql = string.Empty;

            List<OracleParameter> parameters = TransToSearchData(searchType, searchCondition, out sql, out countSql, out sqlCondition, out houseCondition);
            if (parameters == null)
                return null;

            sql = string.Format(sql, sqlCondition, houseCondition);
            countSql = string.Format(countSql, sqlCondition, houseCondition);

            var count = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, countSql, this.ToParameters(parameters.ToArray()));

            if (DataConvert.ToInt(count) <= 0)
                return null;

            pi.ItemCount = DataConvert.ToInt(count);
            pi.PageCount = Convert.ToInt32(Math.Ceiling(pi.ItemCount * 1.0 / pi.PageSize));
            sql += string.Format(" AND ROWNUM between {0} and {1}", pi.CurrentPageStartRowNum, pi.CurrentPageEndRowNum);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        private string GetSearchExpressCompanyID(string expressCompanyId)
        {
            StringBuilder sbExpress = new StringBuilder();
            DataTable dtExpress = new DataTable();
            if (expressCompanyId == "11")
            {
                dtExpress = GetCompanyIdByRfd();
            }
            else
            {
                dtExpress = GetCompanyIdByTopCODCompanyID(expressCompanyId);
            }

            foreach (DataRow dr in dtExpress.Rows)
            {
                sbExpress.Append(dr["ExpressCompanyID"].ToString() + ",");
            }

            return sbExpress.ToString().TrimEnd(',');
        }

        private string CreateHouseCondition(string houseStr, string houseColumnName, string sortColumnName, string houseTypeColumnName,string parameterName, ref List<OracleParameter> parameterList)
        {
            string[] houses;
            houses = houseStr.Split(',');
            StringBuilder houseValues = new StringBuilder();
            StringBuilder sortValues = new StringBuilder();
            StringBuilder conditionStr=new StringBuilder();
            for (int i = 0; i < houses.Length; i++)
            {
                if (houses[i].Trim().Contains("S_"))
                    sortValues.Append(houses[i].Trim().Replace("S_", "") + ",");
                else
                    houseValues.Append( houses[i].Trim() + ",");
            }
            if (string.IsNullOrEmpty(houseTypeColumnName))
            {
                if (!string.IsNullOrEmpty(houseValues.ToString()))
                {
                    conditionStr.Append(Util.Common.GetOracleInParameterWhereSql(houseColumnName, parameterName+"_E", false, true));
                    parameterList.Add(new OracleParameter(string.Format(":{0}", parameterName + "_E"), OracleDbType.Varchar2) { Value = houseValues.ToString().TrimEnd(',') });
                }
                if (!string.IsNullOrEmpty(sortValues.ToString()))
                {
                    conditionStr.Append(Util.Common.GetOracleInParameterWhereSql(sortColumnName, parameterName + "_S", false, true));
                    parameterList.Add(new OracleParameter(string.Format(":{0}", parameterName + "_S"), OracleDbType.Varchar2) { Value = sortValues.ToString().TrimEnd(',') });
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(houseValues.ToString()))
                {
                    conditionStr.Append(string.Format(" OR ({0} AND {1}=1)", Util.Common.GetOracleInParameterWhereSql(houseColumnName, parameterName + "_E", false, true).Substring(4), houseTypeColumnName));
                    parameterList.Add(new OracleParameter(string.Format(":{0}", parameterName + "_E"), OracleDbType.Varchar2) { Value = houseValues.ToString().TrimEnd(',') });
                }
                if (!string.IsNullOrEmpty(sortValues.ToString()))
                {
                    conditionStr.Append(string.Format(" OR ({0} AND {1}=2)", Util.Common.GetOracleInParameterWhereSql(sortColumnName, parameterName + "_S", false, true).Substring(4), houseTypeColumnName));
                    parameterList.Add(new OracleParameter(string.Format(":{0}", parameterName + "_S"), OracleDbType.Varchar2) { Value = sortValues.ToString().TrimEnd(',') });
                }
            }

            return " AND (" + conditionStr.ToString().Substring(4) + ")";
        }

        private List<OracleParameter> TransToSearchData(string searchType, CODSearchCondition searchCondition, out string sql,out string countSql, out string sqlCondition,out string houseCondition)
        {
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string houseStr = string.Empty;
            string countSqlTmp = string.Empty;
            if (!string.IsNullOrEmpty(searchCondition.MerchantID))
            {
                //sbWhere.Append(" AND fcbi.MerchantID=:MerchantIDTmp ");
                sbWhere.Append(Util.Common.GetOracleInParameterWhereSql("fcbi.MerchantID","MerchantIDTmp",false,false));
                parameters.Add(new OracleParameter(":MerchantIDTmp", OracleDbType.Varchar2) { Value = searchCondition.MerchantID });
            }
            if (!string.IsNullOrEmpty(searchCondition.ExpressCompanyID))
            {
                //sbWhere.Append(string.Format(@" AND fcbi.DeliverStationID in ({0}) ",GetSearchExpressCompanyID(searchCondition.ExpressCompanyID)));
                sbWhere.Append(Util.Common.GetOracleInParameterWhereSql("fcbi.TopCODCompanyID", "ExpressCompanyIDs", false, false));
                parameters.Add(new OracleParameter(":ExpressCompanyIDs", OracleDbType.Varchar2) { Value = searchCondition.ExpressCompanyID });
            }
            switch (searchType)
            {
                case "D":
                    sbWhere.Append(" AND fcbi.Flag=:FlagTmp ");
                    parameters.Add(new OracleParameter(":FlagTmp", OracleDbType.Decimal) { Value = 1 });

                    sbWhere.Append(" AND fcbi.WaybillType=:WaybillType ");
                    parameters.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = "0" });

                    sbWhere.Append(" AND fcbi.DeliverTime>=:DeliverTimeS ");
                    parameters.Add(new OracleParameter(":DeliverTimeS", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_D_S.ToShortDateString()) });

                    sbWhere.Append(" AND fcbi.DeliverTime<=:DeliverTimeE ");
                    parameters.Add(new OracleParameter(":DeliverTimeE", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_D_E.AddDays(1).ToShortDateString()) });

                    sql = GetDeliverDetailSql(searchType, out countSqlTmp);
                    houseStr = CreateHouseCondition(searchCondition.HouseD, "t.WarehouseId", "t.WarehouseId", " t.WareHouseType", "WarehouseId_D", ref parameters);
                    break;
                case "DV":
                    sbWhere.Append(" AND fcbi.Flag=:FlagTmp ");
                    parameters.Add(new OracleParameter(":FlagTmp", OracleDbType.Decimal) { Value = 1 });

                    sbWhere.Append(" AND fcbi.WaybillType=:WaybillType ");
                    parameters.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = "1" });

                    sbWhere.Append(" AND fcbi.DeliverTime>=:DeliverTimeS ");
                    parameters.Add(new OracleParameter(":DeliverTimeS", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_D_S.ToShortDateString()) });

                    sbWhere.Append(" AND fcbi.DeliverTime<=:DeliverTimeE ");
                    parameters.Add(new OracleParameter(":DeliverTimeE", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_D_E.AddDays(1).ToShortDateString()) });

                    sql = GetDeliverDetailSql(searchType, out countSqlTmp);
                    houseStr = CreateHouseCondition(searchCondition.HouseD, "t.WarehouseId", "t.WarehouseId", " t.WareHouseType", "WarehouseId_DV", ref parameters);
                    break;
                case "R":
                    sbWhere.Append(" AND fcbi.Flag=:FlagTmp ");
                    parameters.Add(new OracleParameter(":FlagTmp", OracleDbType.Decimal) { Value = 0 });

                    sbWhere.Append(" AND fcbi.WaybillType=:WaybillType ");
                    parameters.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = "0" });

                    sbWhere.Append(" AND fcbi.ReturnTime>=:ReturnTimeS ");
                    parameters.Add(new OracleParameter(":ReturnTimeS", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_R_S.ToShortDateString()) });

                    sbWhere.Append(" AND fcbi.ReturnTime<=:ReturnTimeE ");
                    parameters.Add(new OracleParameter(":ReturnTimeE", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_R_E.AddDays(1).ToShortDateString()) });

                    sql = GetReturnsDetailSql(searchType, out countSqlTmp);
                    houseStr = CreateHouseCondition(searchCondition.HouseR, "t.ReturnWarehouseId", "t.ReturnWarehouseId", "t.ReturnWareHouseType", "WarehouseId_R", ref parameters);
                    break;
                case "RV":
                    sbWhere.Append(" AND fcbi.Flag=:FlagTmp ");
                    parameters.Add(new OracleParameter(":FlagTmp", OracleDbType.Decimal) { Value = 0 });

                    sbWhere.Append(" AND fcbi.WaybillType=:WaybillType ");
                    parameters.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = "1" });

                    sbWhere.Append(" AND fcbi.ReturnTime>=:ReturnTimeS ");
                    parameters.Add(new OracleParameter(":ReturnTimeS", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_R_S.ToShortDateString()) });

                    sbWhere.Append(" AND fcbi.ReturnTime<=:ReturnTimeE ");
                    parameters.Add(new OracleParameter(":ReturnTimeE", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_R_E.AddDays(1).ToShortDateString()) });

                    sql = GetReturnsDetailSql(searchType, out countSqlTmp);
                    houseStr = CreateHouseCondition(searchCondition.HouseR, "t.ReturnWarehouseId", "t.ReturnWarehouseId", "t.ReturnWareHouseType", "WarehouseId_RV", ref parameters);
                    break;
                case "V":
                    sbWhere.Append(" AND fcbi.Flag=:FlagTmp ");
                    parameters.Add(new OracleParameter(":FlagTmp", OracleDbType.Decimal) { Value = 1 });

                    sbWhere.Append(" AND fcbi.WaybillType=:WaybillType ");
                    parameters.Add(new OracleParameter(":WaybillType", OracleDbType.Varchar2, 40) { Value = "2" });

                    sbWhere.Append(" AND fcbi.ReturnTime>=:ReturnTimeS ");
                    parameters.Add(new OracleParameter(":ReturnTimeS", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_V_S.ToShortDateString()) });

                    sbWhere.Append(" AND fcbi.ReturnTime<=:ReturnTimeE ");
                    parameters.Add(new OracleParameter(":ReturnTimeE", OracleDbType.Date) { Value = DataConvert.ToDateTime(searchCondition.Date_V_E.AddDays(1).ToShortDateString()) });

                    sql = GetVisitReturnsDetailSql(searchType, out countSqlTmp);
                    houseStr = CreateHouseCondition(searchCondition.HouseV, "t.ReturnWarehouseId", "t.ReturnWarehouseId", "t.ReturnWareHouseType", "WarehouseId_V", ref parameters);
                    break;
                default:
                    sql = "";
                    break;
            }

            sqlCondition = sbWhere.ToString();
            houseCondition = houseStr;
            countSql = countSqlTmp;
            return parameters;
        }

        private string GetDeliverDetailSql(string searchType, out string countSql)
        {
            countSql = @"
WITH t AS (
SELECT  fcbi.WaybillNO,
		CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE cast(fcbi.ExpressCompanyID as VARCHAR2(20))END 
                          ELSE
                          cast(fcbi.FinalExpressCompanyID as VARCHAR2(20)) END Warehouseid,
        CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ELSE 2 END WareHouseType
FROM    FMS_CODBaseInfo fcbi
WHERE   fcbi.IsDeleted = 0
		AND fcbi.Flag=1 
        {0}
        )
SELECT COUNT(1) 
FROM t
WHERE (1=1) {1}
";

            StringBuilder sbSql = new StringBuilder();
            //明细
            sbSql.Append(@"
 WITH    t AS ( SELECT   fcbi.WaybillNO ,--订单号
                        fcbi.DeliverTime,--发货时间
                        CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE CAST(fcbi.ExpressCompanyID AS VARCHAR2(20))END 
                          ELSE
                          CAST(fcbi.FinalExpressCompanyID AS VARCHAR2(20)) END Warehouseid,
						CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN 
							CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ELSE 2 END WareHouseType,
                        fcbi.DeliverStationID,--配送公司
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.AccountWeight,
                        fcbi.Fare,
                        fcbi.FareFormula,
                        fcbi.ADDRESS
               FROM     FMS_CODBaseInfo fcbi
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        {0}
             )");
            sbSql.Append(@"
    SELECT  ROWNUM AS 序号 ,");

            if (searchType == "D")
                sbSql.Append(@" '普通发货' AS 类型 ,");
            else
                sbSql.Append(@" '上门换发货' AS 类型 ,");

            sbSql.Append(@"t.WaybillNO AS 订单号,
			mbi.MerchantName AS 商家,
			ec.CompanyName AS 配送商,
			CASE WHEN NVL(w.WarehouseName,'')='' THEN ec2.CompanyName ELSE w.WarehouseName END 发货仓库,
			t.DeliverTime AS 发货时间,
			ael.AreaType AS 区域类型,
			t.AccountWeight AS 重量,
            t.Fare  AS 运费,
            t.FareFormula AS 计算公式,
            t.ADDRESS AS 地址
    FROM    t
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.DeliverStationID
            JOIN MerchantBaseInfo mbi ON mbi.ID = t.MerchantID
            LEFT JOIN Warehouse w ON t.WarehouseId = w.WarehouseId AND t.WareHouseType=1
            LEFT JOIN ExpressCompany  ec2 ON ec2.ExpressCompanyID = t.WarehouseId 
																AND t.WareHouseType=2 
																AND ec2.CompanyFlag=1
            LEFT JOIN AreaExpressLevel ael ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND NVL(ael.WareHouseID,'') = ''
			WHERE (1=1) {1}
");

            return sbSql.ToString();
        }

        private string GetReturnsDetailSql(string searchType,out string countSql)
        {
            countSql = @"
WITH t AS (
SELECT  fcbi.WaybillNO,
		CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.ReturnWareHouseID
								ELSE CAST(fcbi.ReturnExpressCompanyID AS VARCHAR2(20))
							END ReturnWarehouseid,
        CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ReturnWareHouseType       
FROM    FMS_CODBaseInfo fcbi
WHERE   fcbi.IsDeleted = 0
        {0}
        )
SELECT COUNT(1) 
FROM t
WHERE  (1=1) {1}
";

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@"
WITH    t AS ( SELECT   fcbi.WaybillNO ,--订单号
                        fcbi.DeliverTime,--发货时间
                        CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE CAST(fcbi.ExpressCompanyID AS VARCHAR2(20)) END 
                          ELSE
                          cast(fcbi.FinalExpressCompanyID as VARCHAR2(20)) END Warehouseid,
						CASE WHEN NVL(fcbi.FinalExpressCompanyID,0)=0 THEN 
							CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ELSE 2 END WareHouseType,
                        fcbi.ReturnTime,
                        CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.ReturnWareHouseID
								ELSE cast(fcbi.ReturnExpressCompanyID as VARCHAR2(20))
							END ReturnWarehouseid,
						CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1
                             ELSE 2
                        END ReturnWareHouseType,
                        fcbi.DeliverStationID,--配送公司
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.AccountWeight,
                        fcbi.Fare,
                        fcbi.FareFormula,
                        fcbi.ADDRESS
               FROM     FMS_CODBaseInfo fcbi
               WHERE    fcbi.IsDeleted = 0
                        {0}
             )");
            sbSql.Append(@"
    SELECT  ROWNUM AS 序号 ,");

            if (searchType == "R")
                sbSql.Append(@" '普通拒收' AS 类型,");
            else if (searchType == "RV")
                sbSql.Append(@" '上门换拒收' AS 类型,");
            else if (searchType == "V")
                sbSql.Append(@" '上门退货' AS 类型,");

            sbSql.Append(@"t.WaybillNO AS 订单号,
			mbi.MerchantName AS 商家,
			ec.CompanyName AS 配送商,
			CASE WHEN NVL(w.WarehouseName,'')='' THEN ec2.CompanyName ELSE w.WarehouseName END 发货仓库,
			t.ReturnTime AS 入库时间,
			CASE WHEN NVL(w1.WarehouseName,'')='' THEN ec3.CompanyName ELSE w1.WarehouseName END 入库仓库,
			t.DeliverTime AS 发货时间,
			ael.AreaType AS 区域类型,
			t.AccountWeight  AS 重量,
            t.Fare  AS 运费,
            t.FareFormula AS 计算公式,
            t.ADDRESS AS 地址
    FROM    t
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.DeliverStationID
            JOIN MerchantBaseInfo mbi ON mbi.ID = t.MerchantID
            LEFT JOIN Warehouse w ON t.WarehouseId = w.WarehouseId AND t.WareHouseType=1
            LEFT JOIN ExpressCompany  ec2 ON ec2.ExpressCompanyID = t.WarehouseId 
																AND t.WareHouseType=2 
																AND ec2.CompanyFlag=1
			LEFT JOIN Warehouse w1 ON t.ReturnWarehouseId = w1.WarehouseId AND t.ReturnWareHouseType=1
            LEFT JOIN ExpressCompany ec3 ON ec3.ExpressCompanyID = t.ReturnWarehouseId 
																AND t.ReturnWareHouseType=2 
																AND ec3.CompanyFlag=1
            LEFT JOIN AreaExpressLevel ael ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND NVL(ael.WareHouseID,'') = ''
			WHERE (1=1) {1}
            ");
            return sbSql.ToString();
        }

        private string GetVisitReturnsDetailSql(string searchType,out string countSql)
        {

            return GetReturnsDetailSql(searchType, out countSql);
        }

        public DataTable GetExportDetail(string searchType, CODSearchCondition searchCondition, bool isDifference)
        {
            string sql = string.Empty;
            string sqlCondition = string.Empty;
            string houseCondition = string.Empty;
            string countSql = string.Empty;
            List<OracleParameter> parameter = TransToSearchData(searchType, searchCondition, out sql, out countSql, out sqlCondition, out houseCondition);
            if (parameter == null)
                return null;

            if (isDifference)
                sqlCondition += " AND fcbi.IsFare<>1";

            sql = string.Format(sql, sqlCondition,houseCondition);

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameter.ToArray());
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable GetErrorLog(CODSearchCondition condition)
        {
            string sql = @"
WITH    T AS ( SELECT   csl.StatisticsDate ,
						'发货' AS DeliveryType ,
                        csl.Reasons ,
                        csl.WareHouseID ,
                        csl.ExpressCompanyID
               FROM     FMS_CodStatsLog csl
               WHERE    (1=1)
                        AND csl.StatisticsDate >= :DdateS
                        AND csl.StatisticsDate <= :DdateE
                        AND csl.IsSuccess = 0
                        AND csl.StatisticsType = 1
                        {0}
               UNION ALL
               SELECT   csl.StatisticsDate ,
						'拒收' AS DeliveryType ,
                        csl.Reasons ,
                        csl.WareHouseID ,
                        csl.ExpressCompanyID
               FROM     FMS_CodStatsLog csl
               WHERE    (1=1)
                        AND csl.StatisticsDate >= :RdateS
                        AND csl.StatisticsDate <= :RdateE
                        AND csl.IsSuccess = 0
                        AND csl.StatisticsType = 2
                        {1}
               UNION ALL
               SELECT   csl.StatisticsDate ,
                        '上门退货' AS DeliveryType ,
                        csl.Reasons ,
                        csl.WareHouseID ,
                        csl.ExpressCompanyID
               FROM     FMS_CodStatsLog csl
               WHERE    (1=1)
                        AND csl.StatisticsDate >= :VdateS
                        AND csl.StatisticsDate <= :VdateE
                        AND csl.IsSuccess = 0
                        AND csl.StatisticsType = 3
                        {2}
             )
    SELECT  t.DeliveryType ,
            t.StatisticsDate ,
            t.Reasons ,
            ec.CompanyName ,
            w.WarehouseName
    FROM    T t
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.ExpressCompanyID
            JOIN warehouse w ON w.WareHouseID = t.WareHouseID";


            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":DdateS", OracleDbType.Date) { Value = condition.Date_D_S });
            parameterList.Add(new OracleParameter(":DdateE", OracleDbType.Date) { Value = condition.Date_D_E });
            parameterList.Add(new OracleParameter(":RdateS", OracleDbType.Date) { Value = condition.Date_R_S });
            parameterList.Add(new OracleParameter(":RdateE", OracleDbType.Date) { Value = condition.Date_R_E });
            parameterList.Add(new OracleParameter(":VdateS", OracleDbType.Date) { Value = condition.Date_V_S });
            parameterList.Add(new OracleParameter(":VdateE", OracleDbType.Date) { Value = condition.Date_V_E });
            parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Varchar2) { Value = condition.MerchantID });
            parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Varchar2) { Value = condition.ExpressCompanyChilds });

            StringBuilder sb_D = new StringBuilder();
            StringBuilder sb_R = new StringBuilder();
            StringBuilder sb_V = new StringBuilder();

            sb_D.Append(Util.Common.GetOracleInParameterWhereSql("csl.MerchantID", "MerchantID", false, false));
            sb_R.Append(Util.Common.GetOracleInParameterWhereSql("csl.MerchantID", "MerchantID", false, false));
            sb_V.Append(Util.Common.GetOracleInParameterWhereSql("csl.MerchantID", "MerchantID", false, false));

            sb_D.Append(Util.Common.GetOracleInParameterWhereSql("csl.ExpressCompanyID", "ExpressCompanyID", false, false));
            sb_R.Append(Util.Common.GetOracleInParameterWhereSql("csl.ExpressCompanyID", "ExpressCompanyID", false, false));
            sb_V.Append(Util.Common.GetOracleInParameterWhereSql("csl.ExpressCompanyID", "ExpressCompanyID", false, false));

            sb_D.Append(CreateHouseCondition(condition.HouseD, "csl.WareHouseID", "csl.WareHouseID", "csl.WareHouseType", "WareHouseID_D", ref parameterList));
            sb_R.Append(CreateHouseCondition(condition.HouseR, "csl.WareHouseID", "csl.WareHouseID", "csl.WareHouseType", "WareHouseID_R", ref parameterList));
            sb_V.Append(CreateHouseCondition(condition.HouseV, "csl.WareHouseID", "csl.WareHouseID", "csl.WareHouseType", "WareHouseID_V", ref parameterList));

            sql = string.Format(sql, sb_D.ToString(), sb_R.ToString(), sb_V.ToString());

            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterList.ToArray()));
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }
	}
}
