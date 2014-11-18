using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using System.Data.SqlClient;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.FinancialManage
{
    public class IncomeAccountDao : SqlServerDao, IIncomeAccountDao
	{
        public string SqlStr { get; set; }

        public DataTable GetUniteAccount(IncomeSearchCondition condition)
        {
            SqlStr = @"
WITH    t AS ( SELECT   CountTypeStr = CASE WHEN fidc.CountType = 0 THEN 'D'
                                            ELSE 'DV'
                                       END ,
                        fidc.ExpressCompanyID ,
                        fidc.AreaType ,
                        SUM(fidc.CountNum) AS CountNum ,
                        SUM(fidc.Fare) AS CountFare ,
                        fidc.Formula,
                        0 AS PFormula,
                        0 AS PCountFare,
                        0 AS RFormula,
                        0 AS RCountFare,
                        0 AS RPFormula,
                        0 AS RPCountFare
               FROM     RFD_FMS.dbo.FMS_IncomeDeliveryCount AS fidc ( NOLOCK )
               WHERE    fidc.IsDeleted = 0
						AND ISNULL(fidc.AccountNO,'')=''
                        AND fidc.MerchantID = @MerchantID
                        AND fidc.CountDate >= @dateStr
                        AND fidc.CountDate <= @dateEnd
               GROUP BY fidc.ExpressCompanyID ,
                        fidc.AreaType ,
                        fidc.Formula ,
                        fidc.CountType
               UNION ALL
               SELECT   CountTypeStr = CASE WHEN firc.CountType = 0 THEN 'R'
                                            ELSE 'RV'
                                       END ,
                        firc.ExpressCompanyID ,
                        firc.AreaType ,
                        SUM(firc.CountNum) AS CountNum ,
                        SUM(firc.Fare) AS CountFare ,
                        firc.Formula,
                        0 AS PFormula,
                        0 AS PCountFare,
                        0 AS RFormula,
                        0 AS RCountFare,
                        0 AS RPFormula,
                        0 AS RPCountFare
               FROM RFD_FMS.dbo.FMS_IncomeReturnsCount AS firc ( NOLOCK )
               WHERE    firc.IsDeleted = 0
						AND ISNULL(firc.AccountNO,'')=''
                        AND firc.MerchantID = @MerchantID
                        AND firc.CountDate >= @dateStr
                        AND firc.CountDate <= @dateEnd
               GROUP BY firc.ExpressCompanyID ,
                        firc.AreaType ,
                        firc.Formula ,
                        firc.CountType
               UNION ALL
               SELECT   CountTypeStr = CASE WHEN fivrc.CountType = 0 THEN 'V'
                                            ELSE 'VV'
                                       END ,
                        fivrc.ExpressCompanyID ,
                        fivrc.AreaType ,
                        SUM(fivrc.CountNum) AS CountNum ,
                        SUM(fivrc.Fare) AS CountFare ,
                        fivrc.Formula,
                        0 AS PFormula,
                        0 AS PCountFare,
                        0 AS RFormula,
                        0 AS RCountFare,
                        0 AS RPFormula,
                        0 AS RPCountFare
               FROM RFD_FMS.dbo.FMS_IncomeVisitReturnsCount AS fivrc ( NOLOCK )
               WHERE    fivrc.IsDeleted = 0
						AND ISNULL(fivrc.AccountNO,'')=''
                        AND fivrc.MerchantID = @MerchantID
                        AND fivrc.CountDate >= @dateStr
                        AND fivrc.CountDate <= @dateEnd
               GROUP BY fivrc.ExpressCompanyID ,
                        fivrc.AreaType ,
                        fivrc.Formula ,
                        fivrc.CountType
               UNION ALL
               SELECT   CountTypeStr = CASE WHEN fiofc.CountType = 1 THEN 'D'
                                            WHEN fiofc.CountType = 2 THEN 'DV'
                                            WHEN fiofc.CountType = 3 THEN 'R'
                                            WHEN fiofc.CountType = 4 THEN 'RV'
                                            WHEN fiofc.CountType = 5 THEN 'V'
                                            WHEN fiofc.CountType = 6 THEN 'VV'
                                       END ,
                        fiofc.ExpressCompanyID ,
                        fiofc.AreaType ,
                        0 AS CountNum ,
                        0 AS CountFare ,
                        '' AS Formula,
                        fiofc.ProtectedStandard AS PFormula,
                        SUM(fiofc.ProtectedFee) AS PCountFare,
                        fiofc.ReceiveStandard AS RFormula,
                        SUM(fiofc.ReceiveFee) AS RCountFare,
                        fiofc.ReceivePOSStandard AS RPFormula,
                        SUM(fiofc.ReceivePOSFee) AS RPCountFare
               FROM RFD_FMS.dbo.FMS_IncomeOtherFeeCount AS fiofc ( NOLOCK )
               WHERE    fiofc.IsDeleted = 0
						AND ISNULL(fiofc.AccountNO,'')=''
                        AND fiofc.MerchantID = @MerchantID
                        AND fiofc.CountDate >= @dateStr
                        AND fiofc.CountDate <= @dateEnd
               GROUP BY fiofc.ExpressCompanyID ,
                        fiofc.AreaType ,
                        fiofc.CountType,
                        fiofc.ProtectedStandard,
                        fiofc.ReceiveStandard,
                        fiofc.ReceivePOSStandard
             )
    SELECT  t.ExpressCompanyID,AreaType,
			MAX(CASE CountTypeStr WHEN 'D' THEN CountNum ELSE 0 END) AS DeliveryNum,
			MAX(CASE CountTypeStr WHEN 'D' THEN Formula ELSE '' END) AS DeliveryStandard,
			MAX(CASE CountTypeStr WHEN 'D' THEN CountFare ELSE 0 END) AS DeliveryFare,
			MAX(CASE CountTypeStr WHEN 'DV' THEN CountNum ELSE 0 END) AS DeliveryVNum,
			MAX(CASE CountTypeStr WHEN 'DV' THEN Formula ELSE '' END) AS DeliveryVStandard,
			MAX(CASE CountTypeStr WHEN 'DV' THEN CountFare ELSE 0 END) AS DeliveryVFare,
			MAX(CASE CountTypeStr WHEN 'R' THEN CountNum ELSE 0 END) AS ReturnsNum,
			MAX(CASE CountTypeStr WHEN 'R' THEN Formula ELSE '' END) AS RetrunsStandard,
			MAX(CASE CountTypeStr WHEN 'R' THEN CountFare ELSE 0 END) AS RetrunsFare,
			MAX(CASE CountTypeStr WHEN 'RV' THEN CountNum ELSE 0 END) AS ReturnsVNum,
			MAX(CASE CountTypeStr WHEN 'RV' THEN Formula ELSE '' END) AS ReturnsVStandard,
			MAX(CASE CountTypeStr WHEN 'RV' THEN CountFare ELSE 0 END) AS ReturnsVFare,
			MAX(CASE CountTypeStr WHEN 'V' THEN CountNum ELSE 0 END) AS VisitReturnsNum,
			MAX(CASE CountTypeStr WHEN 'V' THEN Formula ELSE '' END) AS VisitReturnsStandard,
			MAX(CASE CountTypeStr WHEN 'V' THEN CountFare ELSE 0 END) AS VisitReturnsFare,
			MAX(CASE CountTypeStr WHEN 'VV' THEN CountNum ELSE 0 END) AS VisitReturnsVNum,
			MAX(CASE CountTypeStr WHEN 'VV' THEN Formula ELSE '' END) AS VisitReturnsVStandard,
			MAX(CASE CountTypeStr WHEN 'VV' THEN CountFare ELSE 0 END) AS VisitReturnsVFare,
			MAX(PFormula) AS ProtectedStandard,
			SUM(PCountFare) AS ProtectedFee,
			MAX(RFormula) AS ReceiveStandard,
			SUM(RCountFare) AS ReceiveFee,
			MAX(RPFormula) AS ReceivePOSStandard,
			SUM(RPCountFare) AS ReceivePOSFee,
			ec.CompanyName,
			0 AS DataType
    FROM    t
    JOIN RFD_PMS.dbo.ExpressCompany AS ec(NOLOCK) ON t.ExpressCompanyID=ec.ExpressCompanyID AND ec.CompanyFlag=1
    GROUP BY t.ExpressCompanyID,t.AreaType,ec.CompanyName
    ORDER BY t.ExpressCompanyID,t.AreaType
";
            SqlParameter[] parameters ={
										 new SqlParameter("@MerchantID",SqlDbType.Int),
										 new SqlParameter("@dateStr",SqlDbType.Date),
										 new SqlParameter("@dateEnd",SqlDbType.Date),
									};
            parameters[0].Value = condition.MerchantID;
            parameters[1].Value = condition.DateStr;
            parameters[2].Value = condition.DateEnd;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public bool ChanageCountAcountNO(IncomeSearchCondition condition, int createBy, string accountNo)
        {
            SqlStr = @"
UPDATE  RFD_FMS.dbo.FMS_IncomeDeliveryCount
SET     AccountNO = @AccountNO ,
        UpdateBy = @UpdateBy ,
        UpdateTime = GETDATE(),
        IsChange=@IsChange
WHERE   MerchantID = @MerchantID
        AND CountDate >= @DateStr
        AND CountDate <= @DateEnd
		AND ISNULL(AccountNO,'')=''
        
UPDATE  RFD_FMS.dbo.FMS_IncomeReturnsCount
SET     AccountNO = @AccountNO ,
        UpdateBy = @UpdateBy ,
        UpdateTime = GETDATE(),
        IsChange=@IsChange
WHERE   MerchantID = @MerchantID
        AND CountDate >= @DateStr
        AND CountDate <= @DateEnd
		AND ISNULL(AccountNO,'')=''
        
UPDATE RFD_FMS.dbo.FMS_IncomeVisitReturnsCount
SET     AccountNO = @AccountNO ,
        UpdateBy = @UpdateBy ,
        UpdateTime = GETDATE(),
        IsChange=@IsChange
WHERE   MerchantID = @MerchantID
        AND CountDate >= @DateStr
        AND CountDate <= @DateEnd
		AND ISNULL(AccountNO,'')=''
        
UPDATE RFD_FMS.dbo.FMS_IncomeOtherFeeCount
SET     AccountNO = @AccountNO ,
        UpdateBy = @UpdateBy ,
        UpdateTime = GETDATE(),
        IsChange=@IsChange
WHERE   MerchantID = @MerchantID
        AND CountDate >= @DateStr
        AND CountDate <= @DateEnd
		AND ISNULL(AccountNO,'')=''
";

            SqlParameter[] parameters ={
										 new SqlParameter("@MerchantID",SqlDbType.Int),
										 new SqlParameter("@dateStr",SqlDbType.Date),
										 new SqlParameter("@dateEnd",SqlDbType.Date),
										 new SqlParameter("@AccountNO",SqlDbType.NVarChar,20),
										 new SqlParameter("@UpdateBy",SqlDbType.Int),
                                         new SqlParameter("@IsChange",SqlDbType.Bit)
									};
            parameters[0].Value = condition.MerchantID;
            parameters[1].Value = condition.DateStr;
            parameters[2].Value = condition.DateEnd;
            parameters[3].Value = accountNo;
            parameters[4].Value = createBy;
            parameters[5].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool AddAccountDetail(IncomeAccountDetail m, int createBy, string accountNo)
        {
            SqlStr = @"
INSERT INTO RFD_FMS.dbo.FMS_IncomeAccountDetail
        ( AccountNO ,
          ExpressCompanyID ,
          AreaType ,
          DeliveryNum ,
          DeliveryVNum ,
          ReturnsNum ,
          ReturnsVNum ,
          VisitReturnsNum ,
          VisitReturnsVNum ,
          DeliveryStandard ,
          DeliveryFare ,
          DeliveryVStandard ,
          DeliveryVFare ,
          RetrunsStandard ,
          RetrunsFare ,
          ReturnsVStandard ,
          ReturnsVFare ,
          VisitReturnsStandard ,
          VisitReturnsFare ,
          VisitReturnsVStandard ,
          VisitReturnsVFare ,
          ProtectedStandard ,
          ProtectedFee ,
          ReceiveStandard ,
          ReceiveFee ,
          ReceivePOSStandard ,
          ReceivePOSFee ,
          OtherFee ,
          Fare ,
          DataType ,
          CreateBy ,
          CreateTime ,
          UpdateBy ,
          UpdateTime ,
          IsDeleted,
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
          @VisitReturnsVNum ,
          @DeliveryStandard ,
          @DeliveryFare ,
          @DeliveryVStandard ,
          @DeliveryVFare ,
          @RetrunsStandard ,
          @RetrunsFare ,
          @ReturnsVStandard ,
          @ReturnsVFare ,
          @VisitReturnsStandard ,
          @VisitReturnsFare ,
          @VisitReturnsVStandard ,
          @VisitReturnsVFare ,
          @ProtectedStandard ,
          @ProtectedFee ,
          @ReceiveStandard ,
          @ReceiveFee ,
          @ReceivePOSStandard ,
          @ReceivePOSFee ,
          @OtherFee ,
          @Fare ,
          @DataType ,
          @CreateBy ,
          GETDATE() ,
          0 ,
          GETDATE() ,
          0,
          @IsChange
        )
";
            SqlParameter[] parameters ={
											new SqlParameter("@AccountNO",SqlDbType.NVarChar,20),
											new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
											new SqlParameter("@AreaType",SqlDbType.Int),
											new SqlParameter("@DeliveryNum",SqlDbType.Int),
											new SqlParameter("@DeliveryVNum",SqlDbType.Int),
											new SqlParameter("@ReturnsNum",SqlDbType.Int),
											new SqlParameter("@ReturnsVNum",SqlDbType.Int),
											new SqlParameter("@VisitReturnsNum",SqlDbType.Int),
											new SqlParameter("@VisitReturnsVNum",SqlDbType.Int),
											new SqlParameter("@DeliveryStandard",SqlDbType.NVarChar,150),
											new SqlParameter("@DeliveryFare",SqlDbType.Decimal),
											new SqlParameter("@DeliveryVStandard",SqlDbType.NVarChar,150),
											new SqlParameter("@DeliveryVFare",SqlDbType.Decimal),
											new SqlParameter("@RetrunsStandard",SqlDbType.NVarChar,150),
											new SqlParameter("@RetrunsFare",SqlDbType.Decimal),
											new SqlParameter("@ReturnsVStandard",SqlDbType.NVarChar,150),
											new SqlParameter("@ReturnsVFare",SqlDbType.Decimal),
											new SqlParameter("@VisitReturnsStandard",SqlDbType.NVarChar,150),
											new SqlParameter("@VisitReturnsFare",SqlDbType.Decimal),
											new SqlParameter("@VisitReturnsVStandard",SqlDbType.NVarChar,150),
											new SqlParameter("@VisitReturnsVFare",SqlDbType.Decimal),
											new SqlParameter("@ProtectedStandard",SqlDbType.Decimal),
											new SqlParameter("@ProtectedFee",SqlDbType.Decimal),
											new SqlParameter("@ReceiveStandard",SqlDbType.Decimal),
											new SqlParameter("@ReceiveFee",SqlDbType.Decimal),
											new SqlParameter("@ReceivePOSStandard",SqlDbType.Decimal),
											new SqlParameter("@ReceivePOSFee",SqlDbType.Decimal),
											new SqlParameter("@OtherFee",SqlDbType.Decimal),
											new SqlParameter("@Fare",SqlDbType.Decimal),
											new SqlParameter("@DataType",SqlDbType.Int),
											new SqlParameter("@CreateBy",SqlDbType.Int),
                                            new SqlParameter("@IsChange",SqlDbType.Bit)
									   };
            parameters[0].Value = accountNo;
            parameters[1].Value = m.ExpressCompanyID;
            parameters[2].Value = m.AreaType;
            parameters[3].Value = m.DeliveryNum;
            parameters[4].Value = m.DeliveryVNum;
            parameters[5].Value = m.ReturnsNum;
            parameters[6].Value = m.ReturnsVNum;
            parameters[7].Value = m.VisitReturnsNum;
            parameters[8].Value = m.VisitReturnsVNum;
            parameters[9].Value = m.DeliveryStandard;
            parameters[10].Value = m.DeliveryFare;
            parameters[11].Value = m.DeliveryVStandard;
            parameters[12].Value = m.DeliveryVFare;
            parameters[13].Value = m.RetrunsStandard;
            parameters[14].Value = m.RetrunsFare;
            parameters[15].Value = m.ReturnsVStandard;
            parameters[16].Value = m.ReturnsVFare;
            parameters[17].Value = m.VisitReturnsStandard;
            parameters[18].Value = m.VisitReturnsFare;
            parameters[19].Value = m.VisitReturnsVStandard;
            parameters[20].Value = m.VisitReturnsVFare;
            parameters[21].Value = m.ProtectedStandard;
            parameters[22].Value = m.ProtectedFee;
            parameters[23].Value = m.ReceiveStandard;
            parameters[24].Value = m.ReceiveFee;
            parameters[25].Value = m.ReceivePOSStandard;
            parameters[26].Value = m.ReceivePOSFee;
            parameters[27].Value = m.OtherFee;
            parameters[28].Value = m.Fare;
            parameters[29].Value = m.DataType;
            parameters[30].Value = m.CreateBy;
            parameters[31].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool AddAccount(IncomeSearchCondition condition, int createBy, string accountNo)
        {
            SqlStr = @"
INSERT INTO RFD_FMS.dbo.FMS_IncomeAccount
        ( AccountNO ,
          MerchantID ,
          AccountStatus ,
          CreateBy ,
          CreateTime ,
          UpdateBy ,
          UpdateTime ,
          AuditBy ,
          AuditTime ,
          SearchDateStr ,
          SearchDateEnd ,
          IsDeleted,
          IsChange
        )
VALUES  ( @AccountNO ,
          @MerchantID ,
          @AccountStatus ,
          @CreateBy ,
          GETDATE() ,
          0 ,
          GETDATE() ,
          0 ,
          GETDATE() ,
          @SearchDateStr ,
          @SearchDateEnd ,
          0,
          @IsChange
        )
";

            SqlParameter[] parameters ={
										 new SqlParameter("@MerchantID",SqlDbType.Int),
										 new SqlParameter("@SearchDateStr",SqlDbType.Date),
										 new SqlParameter("@SearchDateEnd",SqlDbType.Date),
										 new SqlParameter("@AccountNO",SqlDbType.NVarChar,20),
										 new SqlParameter("@CreateBy",SqlDbType.Int),
										 new SqlParameter("@AccountStatus",SqlDbType.Int),
                                         new SqlParameter("@IsChange",SqlDbType.Bit)
									};
            parameters[0].Value = condition.MerchantID;
            parameters[1].Value = condition.DateStr;
            parameters[2].Value = condition.DateEnd;
            parameters[3].Value = accountNo;
            parameters[4].Value = createBy;
            parameters[5].Value = (int)EnumAccountAudit.A1;
            parameters[6].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public DataTable GetAccountList(string accountStatus, string merchantId, string dateStr, string dateEnd, string accountNo)
        {
            SqlStr = @"
SELECT  fia.AccountID ,
        fia.AccountNO ,
        fia.AccountStatus ,
        sci.CodeDesc AS AccountStatusStr ,
        fia.CreateBy ,
        fia.AuditBy ,
        fia.UpdateBy ,
        fia.CreateTime ,
        fia.UpdateTime ,
        fia.AuditTime ,
        fia.MerchantID ,
        mbi.MerchantName ,
        fiad.DeliveryNum ,
        fiad.DeliveryFare ,
        fiad.DeliveryVNum ,
        fiad.DeliveryVFare ,
        fiad.ReturnsNum ,
        fiad.RetrunsFare ,
        fiad.ReturnsVNum ,
        fiad.ReturnsVFare ,
        fiad.VisitReturnsNum ,
        fiad.VisitReturnsFare ,
        fiad.VisitReturnsVNum ,
        fiad.VisitReturnsVFare ,
        fiad.ProtectedFee ,
        fiad.ReceiveFee ,
        fiad.ReceivePOSFee,
        fiad.OtherFee,
        fiad.Fare
FROM    RFD_FMS.dbo.FMS_IncomeAccount AS fia ( NOLOCK )
        JOIN RFD_FMS.dbo.FMS_IncomeAccountDetail AS fiad ( NOLOCK ) ON fia.AccountNO = fiad.AccountNO
                                                              AND fiad.DataType = 2
        JOIN StatusCodeInfo AS sci ( NOLOCK ) ON fia.AccountStatus = sci.CodeNo
                                                     AND sci.CodeType = 'CODAccount'
        JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON fia.MerchantID = mbi.ID
WHERE   fia.IsDeleted = 0 and fiad.IsDeleted = 0 {0}
";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(accountStatus))
            {
                sbWhere.Append(" AND fia.AccountStatus=@AccountStatus ");
                parameters.Add(new SqlParameter("@AccountStatus", SqlDbType.Int) { Value = accountStatus });
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND fia.MerchantID=@MerchantID ");
                parameters.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(dateStr))
            {
                sbWhere.Append(" AND fia.CreateTime>=@CreateTimeS ");
                parameters.Add(new SqlParameter("@CreateTimeS", SqlDbType.DateTime) { Value = dateStr });
            }

            if (!string.IsNullOrEmpty(dateEnd))
            {
                sbWhere.Append(" AND fia.CreateTime<=@CreateTimeE ");
                parameters.Add(new SqlParameter("@CreateTimeE", SqlDbType.DateTime) { Value = dateEnd });
            }

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fia.AccountNO<=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20) { Value = accountNo });
            }

            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable GetAccountDetail(string accountNo, string detailId)
        {
            SqlStr = @"
SELECT  fiad.DetailID,
		fiad.AreaType,
		fiad.ExpressCompanyID,
		ec.CompanyName,
		fiad.AccountNO,
        fiad.DeliveryNum ,
        fiad.DeliveryStandard ,
        fiad.DeliveryFare ,
        fiad.DeliveryVNum ,
        fiad.DeliveryVStandard ,
        fiad.DeliveryVFare ,
        fiad.ReturnsNum ,
        fiad.RetrunsStandard ,
        fiad.RetrunsFare ,
        fiad.ReturnsVNum ,
        fiad.ReturnsVStandard ,
        fiad.ReturnsVFare ,
        fiad.VisitReturnsNum ,
        fiad.VisitReturnsStandard ,
        fiad.VisitReturnsFare ,
        fiad.VisitReturnsVNum ,
        fiad.VisitReturnsVStandard ,
        fiad.VisitReturnsVFare ,
        fiad.ProtectedStandard ,
        fiad.ProtectedFee ,
        fiad.ReceiveStandard ,
        fiad.ReceiveFee ,
        fiad.ReceivePOSStandard,
        fiad.ReceivePOSFee,
        fiad.OtherFee,
        fiad.Fare,
        fiad.DataType
FROM    RFD_FMS.dbo.FMS_IncomeAccountDetail AS fiad ( NOLOCK )
LEFT JOIN RFD_PMS.dbo.ExpressCompany AS ec(NOLOCK) ON fiad.ExpressCompanyID = ec.ExpressCompanyID AND ec.CompanyFlag=1
WHERE   fiad.IsDeleted = 0 {0}
ORDER BY fiad.ExpressCompanyID,fiad.AreaType,fiad.DataType
";
            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fiad.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar,20) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(detailId))
            {
                sbWhere.Append(" AND fiad.DetailID=@DetailID ");
                parameters.Add(new SqlParameter("@DetailID", SqlDbType.BigInt) { Value = detailId });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray()).Tables[0];
        }

        /// <summary>
        /// 查询结算明细表
        /// </summary>
        /// <param name="scr"></param>
        /// <returns></returns>
        public DataTable GetAccountDetailNew(string accountNo, string dateType)
        {
            SqlStr = @"
SELECT  fiad.DetailID,
		fiad.AreaType,
        fiad.StationId,
		fiad.ExpressCompanyID,
		ec.CompanyName,
		fiad.AccountNO,
        fiad.DeliveryNum ,
        fiad.DeliveryStandard ,
        fiad.DeliveryFare ,
        fiad.DeliveryVNum ,
        fiad.DeliveryVStandard ,
        fiad.DeliveryVFare ,
        fiad.ReturnsNum ,
        fiad.RetrunsStandard ,
        fiad.RetrunsFare ,
        fiad.ReturnsVNum ,
        fiad.ReturnsVStandard ,
        fiad.ReturnsVFare ,
        fiad.VisitReturnsNum ,
        fiad.VisitReturnsStandard ,
        fiad.VisitReturnsFare ,
        fiad.VisitReturnsVNum ,
        fiad.VisitReturnsVStandard ,
        fiad.VisitReturnsVFare ,
        fiad.ProtectedStandard ,
        fiad.ProtectedFee ,
        fiad.ReceiveStandard ,
        fiad.ReceiveFee ,
        fiad.ReceivePOSStandard,
        fiad.ReceivePOSFee,
        fiad.OverAreaSubsidy,
        fiad.KPI,
        fiad.LostDeduction,
        fiad.ResortDeduction,
        fiad.OtherFee,
        fiad.Fare,
        fiad.DataType
FROM    RFD_FMS.dbo.FMS_IncomeAccountDetail AS fiad ( NOLOCK ) 
left JOIN RFD_PMS.dbo.ExpressCompany AS ec(NOLOCK) ON fiad.stationid = ec.ExpressCompanyID --AND ec.CompanyFlag=1
WHERE   fiad.IsDeleted = 0 and fiad.datatype in (0,2) {0}
ORDER BY fiad.ExpressCompanyID,fiad.AreaType,fiad.DataType
";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fiad.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(dateType))
            {
                sbWhere.Append(" AND fiad.DataType=@DataType ");
                parameters.Add(new SqlParameter("@DataType", SqlDbType.Int) { Value = dateType });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray()).Tables[0];
        }

        public DataTable GetAccountSearchCondition(string accountNo)
        {
            SqlStr = @"
SELECT  fia.AccountNO ,
        fia.MerchantID ,
        fia.SearchDateStr ,
        fia.SearchDateEnd ,
        mbi.MerchantName
FROM    RFD_FMS.dbo.FMS_IncomeAccount AS fia ( NOLOCK )
JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi ( NOLOCK ) ON mbi.ID = fia.MerchantID
WHERE   fia.IsDeleted = 0 {0}
";
            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fia.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20) { Value = accountNo });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, SqlStr, parameters.ToArray()).Tables[0];
        }

        public bool DeleteCount(string accountNo, int updateBy)
        {
            SqlStr = @"
                UPDATE  RFD_FMS.dbo.FMS_IncomeDeliveryCount
                SET     AccountNO = '' ,
                        UpdateBy = @UpdateBy ,
                        UpdateTime = GETDATE(),
                        IsChange=@IsChange
                WHERE   AccountNO = @AccountNO AND IsDeleted=0

                UPDATE  RFD_FMS.dbo.FMS_IncomeVisitReturnsCount
                SET     AccountNO = '' ,
                        UpdateBy = @UpdateBy ,
                        UpdateTime = GETDATE(),
                        IsChange=@IsChange
                WHERE   AccountNO = @AccountNO AND IsDeleted=0

                UPDATE  RFD_FMS.dbo.FMS_IncomeReturnsCount
                SET     AccountNO = '' ,
                        UpdateBy = @UpdateBy ,
                        UpdateTime = GETDATE(),
                        IsChange=@IsChange
                WHERE   AccountNO = @AccountNO AND IsDeleted=0

                UPDATE  RFD_FMS.dbo.FMS_IncomeOtherFeeCount
                SET     AccountNO = '' ,
                        UpdateBy = @UpdateBy ,
                        UpdateTime = GETDATE(),
                        IsChange=@IsChange
                WHERE   AccountNO = @AccountNO AND IsDeleted=0
                ";
            SqlParameter[] parameters =
            {
				new SqlParameter("@AccountNO",SqlDbType.NVarChar,20),
				new SqlParameter("@UpdateBy",SqlDbType.Int),
                new SqlParameter("@IsChange",SqlDbType.Bit)
			};

            parameters[0].Value = accountNo;
            parameters[1].Value = updateBy;
            parameters[2].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool DeleteAccount(string accountNo, int updateBy)
        {
            SqlStr = @"
UPDATE  RFD_FMS.dbo.FMS_IncomeAccount
SET     IsDeleted = 1 ,
        UpdateBy = @UpdateBy ,
        UpdateTime = GETDATE(),
        IsChange=@IsChange
WHERE   AccountNO = @AccountNO
        AND IsDeleted = 0
        
UPDATE  RFD_FMS.dbo.FMS_IncomeAccountDetail
SET     IsDeleted = 1 ,
        UpdateBy = @UpdateBy ,
        UpdateTime = GETDATE(),
        IsChange=@IsChange
WHERE   AccountNO = @AccountNO
        AND IsDeleted = 0
";

            SqlParameter[] parameters =
            {
				new SqlParameter("@AccountNO",SqlDbType.NVarChar,20),
				new SqlParameter("@UpdateBy",SqlDbType.Int),
                new SqlParameter("@IsChange",SqlDbType.Bit)
			};

            parameters[0].Value = accountNo;
            parameters[1].Value = updateBy;
            parameters[2].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateAccountDetailFee(IncomeAccountDetail i)
        {
            SqlStr = @"
                UPDATE  RFD_FMS.dbo.FMS_IncomeAccountDetail
                SET     OtherFee = @OtherFee ,
		                Fare=DeliveryFare+DeliveryVFare+RetrunsFare+ReturnsVFare
			                +VisitReturnsFare+VisitReturnsVFare+ProtectedFee
			                +ReceiveFee+ReceivePOSFee+@OtherFee,
                        UpdateBy = @UpdateBy ,
                        UpdateTime = GETDATE(),
                        IsChange=@IsChange
                WHERE   DataType = 1
                        AND DetailID = @DetailID
                        AND IsDeleted=0
                ";
            SqlParameter[] parameters =
            {
				new SqlParameter("@DetailID",SqlDbType.BigInt),
				new SqlParameter("@OtherFee",SqlDbType.Decimal),
				new SqlParameter("@UpdateBy",SqlDbType.Int),
                new SqlParameter("@IsChange",SqlDbType.Bit)
			};

            parameters[0].Value = i.DetailID;
            parameters[1].Value = i.OtherFee;
            parameters[2].Value = i.UpdateBy;
            parameters[3].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateAccountDetailFeeAll(IncomeAccountDetail i)
        {
            SqlStr = @"
SELECT  @AccountNO = AccountNO
FROM    RFD_FMS.dbo.FMS_IncomeAccountDetail AS fiad ( NOLOCK )
WHERE   DetailID = @DetailID
        AND fiad.IsDeleted = 0

SELECT  @OtherFee = SUM(OtherFee)
FROM    RFD_FMS.dbo.FMS_IncomeAccountDetail AS fiad ( NOLOCK )
WHERE   AccountNO = @AccountNO
        AND fiad.DataType = 1
        AND fiad.IsDeleted = 0

UPDATE  RFD_FMS.dbo.FMS_IncomeAccountDetail
SET     OtherFee = @OtherFee ,
		Fare=DeliveryFare+DeliveryVFare+RetrunsFare+ReturnsVFare
			+VisitReturnsFare+VisitReturnsVFare+ProtectedFee
			+ReceiveFee+ReceivePOSFee+@OtherFee,
        UpdateBy = @UpdateBy ,
        UpdateTime = GETDATE(),
        IsChange=@IsChange
WHERE   DataType = 2
        AND AccountNO = @AccountNO
        AND IsDeleted = 0
";
            SqlParameter[] parameters = 
            {
				new SqlParameter("@DetailID",SqlDbType.BigInt),
				new SqlParameter("@UpdateBy",SqlDbType.Int),
				new SqlParameter("@AccountNO",SqlDbType.NVarChar,20), 
				new SqlParameter("@OtherFee",SqlDbType.Decimal,18,ParameterDirection.Input,false,18,2,"OtherFee",DataRowVersion.Current,"0.00"),
                new SqlParameter("@IsChange",SqlDbType.Bit) { Value=true }
			};

            parameters[0].Value = i.DetailID;
            parameters[1].Value = i.UpdateBy;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        /// <summary>
        /// 修改结算各项总金额
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool UpdateAccountDetailFeeAllByAccountNo(IncomeAccountDetail i)
        {
            SqlStr = @"
            UPDATE  RFD_FMS.dbo.FMS_IncomeAccountDetail
            SET     OtherFee = @OtherFee,ReceiveFee=@ReceiveFee,ReceivePOSFee=@ReceivePOSFee,ProtectedFee=@ProtectedFee,
                    OverAreaSubsidy=@OverAreaSubsidy,KPI=@KPI,LostDeduction=@LostDeduction,ResortDeduction=@ResortDeduction,
		            Fare=DeliveryFare+DeliveryVFare+RetrunsFare+ReturnsVFare
			            +VisitReturnsFare+VisitReturnsVFare+ProtectedFee
			            +ReceiveFee+ReceivePOSFee+@OtherFee+@OverAreaSubsidy+@KPI+@LostDeduction+@ResortDeduction,
                    UpdateBy = @UpdateBy ,
                    UpdateTime = GETDATE(),
                    IsChange=@IsChange
            WHERE   DataType = 2
                    AND AccountNO = @AccountNO
                    AND IsDeleted = 0
            ";
            SqlParameter[] parameters = {
											new SqlParameter("@UpdateBy",SqlDbType.Int),
											new SqlParameter("@AccountNO",SqlDbType.NVarChar,20), 
											new SqlParameter("@OtherFee",SqlDbType.Decimal,18),
                                            new SqlParameter("@ReceiveFee",SqlDbType.Decimal,18),
                                            new SqlParameter("@ReceivePOSFee",SqlDbType.Decimal,18),
                                            new SqlParameter("@ProtectedFee",SqlDbType.Decimal,18),
                                            new SqlParameter("@OverAreaSubsidy",SqlDbType.Decimal,18),
                                            new SqlParameter("@KPI",SqlDbType.Decimal,18),
                                            new SqlParameter("@LostDeduction",SqlDbType.Decimal,18),
                                            new SqlParameter("@ResortDeduction",SqlDbType.Decimal,18),
                                            new SqlParameter("@IsChange",SqlDbType.Bit) { Value=true }
										 };
            parameters[0].Value = i.UpdateBy;
            parameters[1].Value = i.AccountNO;
            parameters[2].Value = i.OtherFee;
            parameters[3].Value = i.ReceiveFee;
            parameters[4].Value = i.ReceivePOSFee;
            parameters[5].Value = i.ProtectedFee;
            parameters[6].Value = i.OverAreaSubsidy;
            parameters[7].Value = i.KPI;
            parameters[8].Value = i.LostDeduction;
            parameters[9].Value = i.ResortDeduction;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateAccountStatus(string accountNo, int auditBy, int status)
        {
            SqlStr = @"
UPDATE  RFD_FMS.dbo.FMS_IncomeAccount
SET     AccountStatus = @AccountStatus ,
        AuditBy = @AuditBy ,
        AuditTime = GETDATE(),
        IsChange=@IsChange
WHERE   AccountNO = @AccountNO
		AND IsDeleted=0
";
            SqlParameter[] parameters = {
											new SqlParameter("@AccountStatus",SqlDbType.Int),
											new SqlParameter("@AuditBy",SqlDbType.Int),
											new SqlParameter("@AccountNO",SqlDbType.NVarChar,20), 
                                            new SqlParameter("@IsChange",SqlDbType.Bit) 
										 };
            parameters[0].Value = status;
            parameters[1].Value = auditBy;
            parameters[2].Value = accountNo;
            parameters[3].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public DataTable GetAccountAreaSummary(string accountNo)
        {
            SqlStr = @"
                SELECT  AreaType ,
                        DeliveryNum = SUM(fiad.DeliveryNum) + SUM(fiad.ReturnsNum)
                        + SUM(fiad.VisitReturnsNum) ,
                        DeliveryFare = SUM(fiad.DeliveryFare) + SUM(fiad.RetrunsFare)
                        + SUM(fiad.VisitReturnsFare) ,
                        ReturnsVNum = SUM(fiad.DeliveryVNum) + SUM(fiad.ReturnsVNum)
                        + SUM(fiad.VisitReturnsVNum) ,
                        DeliveryVFare = SUM(fiad.DeliveryVFare) + SUM(fiad.ReturnsVFare)
                        + SUM(fiad.VisitReturnsVFare) ,
                        ProtectedFee = SUM(fiad.ProtectedFee) ,
                        ReceiveFee = SUM(fiad.ReceiveFee) ,
                        ReceivePOSFee = SUM(fiad.ReceivePOSFee) ,
                        OtherFee = SUM(fiad.OtherFee)
                FROM    RFD_FMS.dbo.FMS_IncomeAccountDetail AS fiad ( NOLOCK )
                WHERE   fiad.IsDeleted = 0
                        AND fiad.DataType = 0
                        {0}
                GROUP BY fiad.AreaType
                ";

            StringBuilder sbWhere = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fiad.AccountNO=@AccountNO ");
                parameters.Add(new SqlParameter("@AccountNO", SqlDbType.NVarChar, 20) { Value = accountNo });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray()).Tables[0];
        }

        #region 收入结算相关服务方法
        /// <summary>
        /// 返回商家列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMerchantInfo(string distributionCode)
        {
            SqlStr = @" SELECT m.ID,m.MerchantName
 FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0 AND m.Sources = 2
AND dmr.DistributionCode='rfd' 
ORDER BY m.CreatTime
                    ";
            SqlParameter[] parameterList ={
                                              new SqlParameter(":DistributionCode",SqlDbType.NVarChar)
                                         };
            parameterList[0].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameterList).Tables[0];
        }

        /// <summary>
        /// 返回商家发货明细
        /// </summary>
        /// <returns></returns>
        public DataTable GetIncomeDeliveryAccountDetails(int merchantid, DateTime countDate)
        {
            SqlStr = @"
WITH  t AS ( SELECT   fibi.incomeID,
                        fibi.WaybillNo ,--订单号
                        fibi.DeliverTime ,--发货时间
						fibi.backstationtime, --提交配送结果时间
                        fibi.MerchantID ,--商家ID
                        fibi.DeliverStationID ,--配送站ID    
                        fibi.ExpressCompanyID ,
                        fibi.WaybillType ,--发货类型
                        fibi.FinalExpressCompanyID ,
                        fibi.AccountWeight ,
                        fibi.AreaID ,
                        fibi.TopCODCompanyID,
                        FinalExpressID = CASE WHEN ISNULL(fibi.FinalExpressCompanyID,0)=0 THEN 
							 CONVERT(NVARCHAR(20), fibi.ExpressCompanyID) 
                          ELSE
                             CONVERT(NVARCHAR(20), fibi.FinalExpressCompanyID) END
               FROM LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
                join LMS_RFD.dbo.FMS_IncomeFeeInfo as fifi (nolock) on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.backstationtime >= @CountStartDate
						and fibi.backstationtime<@CountEndDate
						and fibi.MerchantID=@MerchantId
             )
    SELECT  t.incomeID,
			t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            t.FinalExpressID,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
            t.TopCODCompanyID
    FROM    t
            LEFT JOIN AreaExpressLevelIncome ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1,2)
                                                         AND ael.expresscompanyid = t.FinalExpressID
                                                         AND ael.MerchantID = t.MerchantID
           
";

            SqlParameter[] parameters ={
										   new SqlParameter("@MerchantId",SqlDbType.Int),
										   new SqlParameter("@CountStartDate",SqlDbType.DateTime),
                                            new SqlParameter("@CountEndDate",SqlDbType.DateTime),
									  };
            parameters[0].Value = merchantid;
            parameters[1].Value = countDate;
            parameters[2].Value = countDate.AddDays(1);

            DataTable dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, SqlStr, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 按商家返回发货信息
        /// </summary>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeDeliveryAccount(int merchantId, DateTime accountDate, int deliverstationid, int expresscompanyid)
        {
            string sql = @"
WITH    t AS ( SELECT   fibi.WaybillNO ,
                        fibi.DeliverStationID ,
                        fibi.WaybillType,
                        fibi.AccountWeight,
                        fibi.AreaID,
                        fibi.TopCodCompanyID,
                        fibi.MerchantID,
                        warehouseid = CASE WHEN ISNULL(fibi.FinalExpressCompanyID,0)=0 THEN 
							 CONVERT(NVARCHAR(20), fibi.ExpressCompanyID) 
                          ELSE
                             CONVERT(NVARCHAR(20), fibi.FinalExpressCompanyID) END,
                        fifi.AccountStandard,
                        fifi.AccountFare
               FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
               join     LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) 
               on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= @CreatTimeStr
                        AND fibi.BackstationTime < @CreatTimeEnd
                        AND fibi.MerchantID = @MerchantID
                        AND fibi.BackStationstatus=3
                        AND fibi.DeliverStationID=@DeliverStationID
                        AND fibi.FinalExpressCompanyID=@FinalExpressCompanyID
             )
    SELECT  t.DeliverStationID,
			t.warehouseid,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.AccountFare) AS Fare ,
            t.AccountStandard AS Formula ,
            SUM(ISNULL(t.AccountWeight,0)) AS WEIGHT ,
            t.WaybillType
    FROM    t
            LEFT JOIN AreaExpressLevelIncome ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1,2)
                                                         AND ael.warehouseId = t.warehouseid
                                                         AND ael.MerchantID = t.MerchantID
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.AccountStandard
";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
                                            new SqlParameter("@DeliverStationID",SqlDbType.Int),
                                            new SqlParameter("@FinalExpressCompanyID",SqlDbType.Int)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverstationid;
            parameters[4].Value = expresscompanyid;

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 按商家、站点、分拣中心分组，计算发货日统计单量
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeDeliveryAccountInfo(int merchantId, DateTime accountDate)
        {
            string sql = @"WITH    t AS ( 
                            SELECT  fibi.WaybillNO ,
                                    fibi.DeliverStationID ,
                                    fibi.MerchantID,
                                    warehouseid = CASE WHEN ISNULL(fibi.FinalExpressCompanyID,0)=0 THEN 
                                                     CONVERT(NVARCHAR(20), fibi.ExpressCompanyID) 
                                                  ELSE
                                                     CONVERT(NVARCHAR(20), fibi.FinalExpressCompanyID) END
                           FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
                           join     LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0
                                    AND fibi.WaybillType IN ( '0', '1' )
                                    AND fibi.BackstationTime >= @CreatTimeStr
                                    AND fibi.BackstationTime < @CreatTimeEnd
                                    AND fibi.MerchantID = @MerchantID
                                    AND fibi.BackStationstatus=3
                                     )
                        SELECT  t.DeliverStationID,
                                t.warehouseid,--最后出库分拣中心
                                COUNT(1) AS FormCount,--订单数量
                                1 AS StatisticsType,--类型（1、发货2、拒收3、上门）
                                t.MerchantID
                        FROM    t
                                
                        GROUP BY t.DeliverStationID,
                                 t.WareHouseID,
                                 t.merchantid
";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 校验这个商家，这个站点，这个分拣中心，在当天费用是否都计算过
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="warehouseid"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int CollateIncomeDeliveryAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            string sql = @"WITH    t AS ( SELECT   fibi.WaybillNO
               FROM LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
				join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= @CreatTimeStr
                        AND fibi.BackstationTime < @CreatTimeEnd
                        AND fibi.DeliverStationID = @DeliverStationID
                        AND fibi.ExpressCompanyID = @Warehouseid
                        AND fibi.MerchantID = @MerchantID
                        AND ISNULL(fibi.FinalExpressCompanyID, 0) = 0
                        AND fibi.BackStationstatus=3
               UNION ALL
               SELECT   fibi.WaybillNO
               FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
				join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= @CreatTimeStr
                        AND fibi.BackstationTime < @CreatTimeEnd
                        AND fibi.DeliverStationID = @DeliverStationID
                        AND fibi.FinalExpressCompanyID = @Warehouseid
                        AND fibi.MerchantID = @MerchantID
                        AND ISNULL(fibi.FinalExpressCompanyID, 0) > 0
                        AND fibi.BackStationstatus=3             
             )
    SELECT  COUNT(1)
    FROM    t";
            SqlParameter[] parameters = { 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@Warehouseid",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate;
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = warehouseid;
            parameters[4].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);

        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeDeliveryAccount(IncomeDeliveryCount model)
        {
            string TableName = @"RFD_FMS.dbo.FMS_IncomeDeliveryCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" AccountNO , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" ExpressCompanyID , ");
            strSql.Append(" AreaType , ");
            strSql.Append(" Weight , ");
            strSql.Append(" CountType , ");
            strSql.Append(" CountDate , ");
            strSql.Append(" CountNum , ");
            strSql.Append(" Fare , ");
            strSql.Append(" Formula , ");
            strSql.Append(" CreateBy , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateBy , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" IsDeleted , ");
            strSql.Append(" StationID,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" @AccountNO , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @ExpressCompanyID , ");
            strSql.Append(" @AreaType , ");
            strSql.Append(" @Weight , ");
            strSql.Append(" @CountType , ");
            strSql.Append(" @CountDate , ");
            strSql.Append(" @CountNum , ");
            strSql.Append(" @Fare , ");
            strSql.Append(" @Formula , ");
            strSql.Append(" @CreateBy , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateBy , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @IsDeleted , ");
            strSql.Append(" @StationID,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = 
            {
				new SqlParameter(string.Format("@{0}","AccountNO"), model.AccountNO),
				new SqlParameter(string.Format("@{0}","MerchantID"), model.MerchantID),
				new SqlParameter(string.Format("@{0}","ExpressCompanyID"), model.ExpressCompanyID),
				new SqlParameter(string.Format("@{0}","AreaType"), model.AreaType),
				new SqlParameter(string.Format("@{0}","Weight"), model.Weight),
				new SqlParameter(string.Format("@{0}","CountType"), model.CountType),
				new SqlParameter(string.Format("@{0}","CountDate"), model.CountDate),
				new SqlParameter(string.Format("@{0}","CountNum"), model.CountNum),
				new SqlParameter(string.Format("@{0}","Fare"), model.Fare),
				new SqlParameter(string.Format("@{0}","Formula"), model.Formula),
				new SqlParameter(string.Format("@{0}","CreateBy"), model.CreateBy),
				new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
				new SqlParameter(string.Format("@{0}","UpdateBy"), model.UpdateBy),
				new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
				new SqlParameter(string.Format("@{0}","IsDeleted"), model.IsDeleted),
				new SqlParameter(string.Format("@{0}","StationID"), model.StationID),
	            new SqlParameter(string.Format("@{0}","IsChange"), true)	
            };

            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (obj == null) return 0;

            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeStatLog(IncomeStatLog model)
        {
            string TableName = @"RFD_FMS.dbo.FMS_IncomeStatLog";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" StatisticsType , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" StationID , ");
            strSql.Append(" ExpressCompanyID , ");
            strSql.Append(" StatisticsDate , ");
            strSql.Append(" IsSuccess , ");
            strSql.Append(" Reasons , ");
            strSql.Append(" Ip , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateTime,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" @StatisticsType , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @StationID , ");
            strSql.Append(" @ExpressCompanyID , ");
            strSql.Append(" @StatisticsDate , ");
            strSql.Append(" @IsSuccess , ");
            strSql.Append(" @Reasons , ");
            strSql.Append(" @Ip , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateTime,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");

            SqlParameter[] parameters = 
            {
				new SqlParameter(string.Format("@{0}","StatisticsType"), model.StatisticsType),
				new SqlParameter(string.Format("@{0}","MerchantID"), model.MerchantID),
				new SqlParameter(string.Format("@{0}","StationID"), model.StationID),
				new SqlParameter(string.Format("@{0}","ExpressCompanyID"), model.ExpressCompanyID),
				new SqlParameter(string.Format("@{0}","StatisticsDate"), model.StatisticsDate),
				new SqlParameter(string.Format("@{0}","IsSuccess"), model.IsSuccess),
				new SqlParameter(string.Format("@{0}","Reasons"), model.Reasons),
				new SqlParameter(string.Format("@{0}","Ip"), model.Ip),
				new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
				new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
	            new SqlParameter(string.Format("@{0}","IsChange"), true)	
            };

            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        public bool UpdateIncomeStatLog(IncomeStatLog model)
        {

            StringBuilder strSql = new StringBuilder();

            strSql.Append("update RFD_FMS.dbo.FMS_IncomeStatLog set ");

            strSql.Append(" IsSuccess = @IsSuccess ,	 ");

            strSql.Append(" Reasons = @Reasons ,	 ");

            strSql.Append(" UpdateTime = @UpdateTime,  ");
            strSql.Append(" IsChange = @IsChange  ");
            strSql.Append(string.Format(" where {0} = @{0}", "LogID"));
            SqlParameter[] parameters = 
            {
				new SqlParameter(string.Format("@{0}","LogID"), model.LogID),		
                new SqlParameter(string.Format("@{0}","IsSuccess"), model.IsSuccess),				
                new SqlParameter(string.Format("@{0}","Reasons"), model.Reasons),					
                new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
		        new SqlParameter(string.Format("@{0}","IsChange"), true)
            };

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否还有未对接过的账单
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>true代表还有未对接的账单，false代表没有对接的账单</returns>
        public bool IsAccessGetIncomeAccount(int MerchantID, DateTime benginDate, DateTime endDate)
        {
            return false;
        }


        /// <summary>
        /// 根据商家id和记账日期返回账单明细
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        public DataTable GetIncomeAccountDetail(int MerchantID, DateTime benginDate, DateTime endDate)
        {
            return null;
        }

        /// <summary>
        /// 根据商家id和记账日期返回账单总表
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        public DataTable GetUnPushedIncomeAccountAndDetail(int MerchantID, DateTime benginDate, DateTime endDate)
        {
            return null;
        }

        public DataTable GetAccountSearchCondition(int MerchantID, DateTime benginDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 当日拒收数据（商家、配送站、分拣中心）
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountdate"></param>
        /// <returns></returns>
        public DataTable GetIncomeReturnsCount(int merchantid, DateTime accountdate)
        {
            string sql = @"
                        WITH t AS ( 
                        SELECT fibi.WaybillNO,fibi.DeliverStationID,fibi.MerchantID,fibi.ReturnExpressCompanyID
                        FROM    LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
					            join LMS_RFD.dbo.FMS_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
                        WHERE   fifi.IsDeleted = 0
                                AND fibi.WaybillType IN ( '0', '1' )
                                AND fibi.ReturnTime >= @CreatTimeStr
                                AND fibi.ReturnTime < @CreatTimeEnd
                                and fibi.MerchantId=@MerchantID
                                And fibi.backstationstatus=5
                     )
            SELECT  @CreatTimeStr AS StatisticsDate ,
                    t.DeliverStationID ,
                    t.ReturnExpressCompanyID ,
                    COUNT(t.WaybillNO) AS FormCount ,
                    2 AS StatisticsType ,
                    t.MerchantID
            FROM    t
            GROUP BY t.DeliverStationID ,
                    t.ReturnExpressCompanyID ,
                    t.MerchantID
";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime)
										};
            parameters[0].Value = merchantid;
            parameters[1].Value = accountdate.ToShortDateString();
            parameters[2].Value = accountdate.AddDays(1).ToShortDateString();

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 收入结算--拒收（校验）
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="returnexpresscompanyId"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int CollateIncomeReturnsAccountInfo(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid)
        {
            string sql = @"with t as(
                            SELECT fibi.WaybillNO
                            FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi (nolock)
                            join LMS_RFD.dbo.FMS_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
                            WHERE fifi.IsDeleted = 0
                            and fifi.IsAccount=1
                            AND fibi.WaybillType IN ( '0', '1' )
                            AND fibi.ReturnTime >= @CreatTimeStr
                            AND fibi.ReturnTime < @CreatTimeEnd
                            AND fibi.DeliverStationID = @DeliverStationID
                            AND fibi.ReturnExpressCompanyID = @ReturnExpressId
                            AND fibi.MerchantID = @MerchantID
                            And fibi.backstationstatus=5
                            )
                            SELECT COUNT(1) FROM t";
            SqlParameter[] parameters = { 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnExpressId",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 按商家返回拒收信息
        /// </summary>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeReturnsAccount(int merchantId, DateTime accountDate, int deliverStationId, int returnExpressId)
        {
            string sql = @"
            WITH    t AS ( SELECT   fibi.WaybillNO ,
                                    fibi.DeliverStationID ,
                                    fibi.ReturnExpressCompanyID,
						            fifi.AccountFare,
						            fifi.AccountStandard,
                                    fibi.AccountWeight ,
                                    fibi.WaybillType ,
                                    fibi.AreaID ,
                                    fibi.MerchantID
                           FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
						            join LMS_RFD.dbo.FMS_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0
                                    AND fibi.WaybillType IN ( '0', '1' )
                                    AND fibi.ReturnTime >=@CreatTimeStr
                                    AND fibi.ReturnTime < @CreatTimeEnd
                                    AND fibi.MerchantID = @MerchantID
                                    AND fibi.DeliverStationID = @DeliverStationID
                                    AND fibi.ReturnExpressCompanyID = @ReturnExpressId
                                    And fibi.backstationstatus=5
                         )
                SELECT  t.DeliverStationID,
                        t.ReturnExpressCompanyID ,
                        ael.AreaType ,
                        COUNT(1) AS FormCount ,
                        SUM(t.AccountFare) AS Fare ,
                        t.AccountStandard AS Formula ,
                        SUM(ISNULL(t.AccountWeight, 0)) AS WEIGHT,
                        t.WaybillType
                FROM    t
                        LEFT JOIN AreaExpressLevelIncome ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                                     AND ael.[Enable] IN (1,2)
                                                                     AND ael.warehouseId = t.ReturnExpressCompanyID
                                                                     AND ael.MerchantID = t.MerchantID
                GROUP BY t.DeliverStationID ,
                        t.ReturnExpressCompanyID ,
                        ael.AreaType ,
                        t.WaybillType,
                        t.AccountStandard";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
                                            new SqlParameter("@DeliverStationID",SqlDbType.Int),
                                            new SqlParameter("@ReturnExpressId",SqlDbType.Int)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverStationId;
            parameters[4].Value = returnExpressId;

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 收入结算拒收增加一条数据
        /// </summary>
        public int AddIncomeReturnsCountInfo(IncomeReturnsCount model)
        {
            string TableName = @"RFD_FMS.dbo.FMS_IncomeReturnsCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" AccountNO , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" ExpressCompanyID , ");
            strSql.Append(" AreaType , ");
            strSql.Append(" Weight , ");
            strSql.Append(" CountType , ");
            strSql.Append(" CountDate , ");
            strSql.Append(" CountNum , ");
            strSql.Append(" Fare , ");
            strSql.Append(" Formula , ");
            strSql.Append(" CreateBy , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateBy , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" IsDeleted , ");
            strSql.Append(" StationID,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" @AccountNO , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @ExpressCompanyID , ");
            strSql.Append(" @AreaType , ");
            strSql.Append(" @Weight , ");
            strSql.Append(" @CountType , ");
            strSql.Append(" @CountDate , ");
            strSql.Append(" @CountNum , ");
            strSql.Append(" @Fare , ");
            strSql.Append(" @Formula , ");
            strSql.Append(" @CreateBy , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateBy , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @IsDeleted , ");
            strSql.Append(" @StationID,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");

            SqlParameter[] parameters = 
            {
			    new SqlParameter(string.Format("@{0}","AccountNO"), model.AccountNO),
			    new SqlParameter(string.Format("@{0}","MerchantID"), model.MerchantID),
			    new SqlParameter(string.Format("@{0}","ExpressCompanyID"), model.ExpressCompanyID),
			    new SqlParameter(string.Format("@{0}","AreaType"), model.AreaType),
			    new SqlParameter(string.Format("@{0}","Weight"), model.Weight),
			    new SqlParameter(string.Format("@{0}","CountType"), model.CountType),
			    new SqlParameter(string.Format("@{0}","CountDate"), model.CountDate),
			    new SqlParameter(string.Format("@{0}","CountNum"), model.CountNum),
			    new SqlParameter(string.Format("@{0}","Fare"), model.Fare),
			    new SqlParameter(string.Format("@{0}","Formula"), model.Formula),
			    new SqlParameter(string.Format("@{0}","CreateBy"), model.CreateBy),
			    new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
			    new SqlParameter(string.Format("@{0}","UpdateBy"), model.UpdateBy),
			    new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
			    new SqlParameter(string.Format("@{0}","IsDeleted"), model.IsDeleted),
			    new SqlParameter(string.Format("@{0}","StationID"), model.StationID),
				new SqlParameter(string.Format("@{0}","IsChange"), true)					
            };

            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (obj == null) return 0;

            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 收入结算上门退增加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddIncomeVisitReturnsCountInfo(IncomeVisitReturnsCount model)
        {
            string TableName = @"RFD_FMS.dbo.FMS_IncomeVisitReturnsCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" AccountNO , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" ExpressCompanyID , ");
            strSql.Append(" AreaType , ");
            strSql.Append(" Weight , ");
            strSql.Append(" CountType , ");
            strSql.Append(" CountDate , ");
            strSql.Append(" CountNum , ");
            strSql.Append(" Fare , ");
            strSql.Append(" Formula , ");
            strSql.Append(" CreateBy , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateBy , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" IsDeleted , ");
            strSql.Append(" StationID,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" @AccountNO , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @ExpressCompanyID , ");
            strSql.Append(" @AreaType , ");
            strSql.Append(" @Weight , ");
            strSql.Append(" @CountType , ");
            strSql.Append(" @CountDate , ");
            strSql.Append(" @CountNum , ");
            strSql.Append(" @Fare , ");
            strSql.Append(" @Formula , ");
            strSql.Append(" @CreateBy , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateBy , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @IsDeleted , ");
            strSql.Append(" @StationID,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");

            SqlParameter[] parameters = 
            {
				new SqlParameter(string.Format("@{0}","AccountNO"), model.AccountNO),
				new SqlParameter(string.Format("@{0}","MerchantID"), model.MerchantID),
				new SqlParameter(string.Format("@{0}","ExpressCompanyID"), model.ExpressCompanyID),
				new SqlParameter(string.Format("@{0}","AreaType"), model.AreaType),
				new SqlParameter(string.Format("@{0}","Weight"), model.Weight),
				new SqlParameter(string.Format("@{0}","CountType"), model.CountType),
				new SqlParameter(string.Format("@{0}","CountDate"), model.CountDate),
				new SqlParameter(string.Format("@{0}","CountNum"), model.CountNum),
				new SqlParameter(string.Format("@{0}","Fare"), model.Fare),
				new SqlParameter(string.Format("@{0}","Formula"), model.Formula),
				new SqlParameter(string.Format("@{0}","CreateBy"), model.CreateBy),
				new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
				new SqlParameter(string.Format("@{0}","UpdateBy"), model.UpdateBy),
				new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
				new SqlParameter(string.Format("@{0}","IsDeleted"), model.IsDeleted),
				new SqlParameter(string.Format("@{0}","StationID"), model.StationID),
                new SqlParameter(string.Format("@{0}","IsChange"), true)
            };

            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 收入结算--上门1
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountdate"></param>
        /// <returns></returns>
        public DataTable GetIncomeVisitReturnsCount(int merchantid, DateTime accountdate)
        {
            string sql = @"
                        WITH   t AS ( SELECT   fibi.WaybillNO ,
                        fibi.DeliverStationID ,
                        fibi.MerchantID,
                        fibi.ReturnExpressCompanyID
               FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
			   join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0 
                        AND fibi.WaybillType ='2'
                        and fibi.BackstationStatus=3
                        AND fibi.ReturnTime >= @ReturnTimeStr
                        AND fibi.ReturnTime < @ReturnTimeEnd
                        and fibi.MerchantId=@MerchantID
union   
SELECT   fibi.WaybillNO ,
                        fibi.DeliverStationID ,
                        fibi.MerchantID,
                        fibi.ReturnExpressCompanyID
               FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
			   join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0 
                        AND fibi.WaybillType ='2'
						and Fibi.BackstationStatus=5
                        AND fibi.backstationtime >= @ReturnTimeStr
                        AND fibi.backstationtime < @ReturnTimeEnd
                        and fibi.MerchantId=@MerchantID
             )
    SELECT  @ReturnTimeStr AS StatisticsDate ,
            t.DeliverStationID,
            t.ReturnExpressCompanyID ,
            COUNT(t.WaybillNO) AS FormCount ,
            3 AS StatisticsType
    FROM    t
    GROUP BY t.DeliverStationID ,
            t.ReturnExpressCompanyID";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime)
										};
            parameters[0].Value = merchantid;
            parameters[1].Value = accountdate.ToShortDateString();
            parameters[2].Value = accountdate.AddDays(1).ToShortDateString();

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 收入结算--上门（校验）
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="returnexpresscompanyId"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int CollateIncomeVisitReturnsAccountInfo(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid)
        {
            string sql = @"select sum(a.a1) as num from (SELECT   COUNT(1) as a1 FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
        join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 and fifi.IsAccount=1 AND fibi.WaybillType ='2' and fibi.backstationstatus=3
        AND fibi.ReturnTime >= @ReturnTimeStr
        AND fibi.ReturnTime < @ReturnTimeEnd
        AND fibi.DeliverStationID = @DeliverStationID
        AND fibi.ReturnExpressCompanyID = @ReturnExpressId
        AND fibi.MerchantID = @MerchantID
union
SELECT   COUNT(1) as a1 FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
        join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 and fifi.IsAccount=1 AND fibi.WaybillType ='2' and fibi.backstationstatus=5
        AND fibi.backstationtime >= @ReturnTimeStr
        AND fibi.backstationtime < @ReturnTimeEnd
        AND fibi.DeliverStationID = @DeliverStationID
        AND fibi.MerchantID = @MerchantID) as a";
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnExpressId",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 收入结算--上门（站点，返回分拣中心，区域类型，上门退类型，结算标准）
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeVisitReturnsAccount(int merchantId, DateTime accountDate, int deliverStationId, int returnExpressId)
        {
            string sql = @"
            WITH    t AS ( SELECT   fibi.WaybillNO ,
                                    fibi.DeliverStationID ,
                                    fibi.ReturnExpressCompanyID,
						            fifi.AccountFare,
						            fifi.AccountStandard,
                                    fibi.AccountWeight ,
                                    fibi.WaybillType ,
                                    fibi.AreaID ,
                                    fibi.MerchantID,
                                    fibi.BackStationStatus
                           FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
						            join LMS_RFD.dbo.FMS_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0 AND fibi.WaybillType ='2'
                                    And fibi.BackStationStatus=3
                                    AND fibi.ReturnTime >=@CreatTimeStr
                                    AND fibi.ReturnTime < @CreatTimeEnd
                                    AND fibi.DeliverStationID = @DeliverStationID
                                    AND fibi.ReturnExpressCompanyID = @ReturnExpressId
                                    AND fibi.MerchantID = @MerchantID
union
SELECT   fibi.WaybillNO ,
                                    fibi.DeliverStationID ,
                                    fibi.ReturnExpressCompanyID,
						            fifi.AccountFare,
						            fifi.AccountStandard,
                                    fibi.AccountWeight ,
                                    fibi.WaybillType ,
                                    fibi.AreaID ,
                                    fibi.MerchantID,
                                    fibi.BackStationStatus
                           FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
						            join LMS_RFD.dbo.FMS_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0 AND fibi.WaybillType ='2'
                                    And fibi.BackStationStatus=5
                                    AND fibi.backstationtime >=@CreatTimeStr
                                    AND fibi.backstationtime < @CreatTimeEnd
                                    AND fibi.MerchantID = @MerchantID
                                    AND fibi.DeliverStationID = @DeliverStationID
                         )
                SELECT  t.DeliverStationID,
                        t.ReturnExpressCompanyID ,
                        ael.AreaType ,
                        COUNT(1) AS FormCount ,
                        SUM(t.AccountFare) AS Fare ,
                        t.AccountStandard AS Formula ,
                        SUM(ISNULL(t.AccountWeight, 0)) AS WEIGHT,
                        case when t.BackStationStatus='3' then '0' else '1' end as CountType
                FROM    t
                        LEFT JOIN AreaExpressLevelIncome ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                                     AND ael.[Enable] IN (1,2)
                                                                     AND ael.warehouseId = t.ReturnExpressCompanyID
                                                                     AND ael.MerchantID = t.MerchantID
                GROUP BY t.DeliverStationID ,
                        t.ReturnExpressCompanyID,
                        ael.AreaType ,
                        t.BackStationStatus,
                        t.AccountStandard";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
                                            new SqlParameter("@DeliverStationID",SqlDbType.Int),
                                            new SqlParameter("@ReturnExpressId",SqlDbType.Int)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverStationId;
            parameters[4].Value = returnExpressId;

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 收入结算其它金额（代收货款）
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountdate"></param>
        /// <returns></returns>
        public DataTable GetIncomeOtherReceiveFee(int merchantid, DateTime accountdate)
        {
            string sql = @"
            with t as(
            SELECT fibi.waybillno,
            fibi.merchantid,
            fibi.deliverstationid
            FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
            join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0 
            AND fibi.backstationtime >=@BackStationTimeStr --归班时间
            AND fibi.backstationtime < @BackStationTimeEnd--归班时间
            AND fibi.MerchantID = @MerchantID
            AND fibi.backStationStatus in (3,5)
            ) 
            select
            t.deliverstationid,
            COUNT(1) AS FormCount--订单数量
             from t
             GROUP BY t.deliverstationid";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@BackStationTimeStr",SqlDbType.DateTime),
											new SqlParameter("@BackStationTimeEnd",SqlDbType.DateTime)
										};
            parameters[0].Value = merchantid;
            parameters[1].Value = accountdate.ToShortDateString();
            parameters[2].Value = accountdate.AddDays(1).ToShortDateString();

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        public int CollateIncomeOtherReceiveFee(int merchantid, DateTime accountDate, int deliverstationid)
        {
            string sql = @" with t as(
            SELECT fibi.waybillno,
            fibi.merchantid,
            fibi.deliverstationid
            FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
            join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0 and fifi.IsReceive=1
            AND fibi.backstationtime >=@BackStationTimeStr --归班时间
            AND fibi.backstationtime < @BackStationTimeEnd--归班时间
            AND fibi.MerchantID = @MerchantID
            And fibi.deliverstationid=@DeliverStationID
            AND fibi.backStationStatus in (3,5)
            ) 
            select count(1) from t
             ";
            SqlParameter[] parameters = { 
											new SqlParameter("@BackStationTimeStr",SqlDbType.DateTime),
											new SqlParameter("@BackStationTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        public DataTable GetIncomeOtherReceiveFeeDetails(int merchantId, DateTime accountDate, int deliverStationId)
        {
            string sql = @"
            with t as(--计算代收货款
            SELECT fibi.waybillno,
            fibi.merchantid,
            fibi.deliverstationid,
            fibi.waybilltype,
            fibi.BackStationStatus,
            fifi.ReceiveFee,
            fifi.ReceiveStandard,
            POSReceiveStandard,--
            POSReceiveFee,
            CashReceiveServiceStandard,
            CashReceiveServiceFee,
            POSReceiveServiceStandard,
            POSReceiveServiceFee,
            case 
            when fibi.waybilltype=0 and fibi.BackStationStatus=3 then 1
            when fibi.waybilltype=1 and fibi.BackStationStatus=3 then 2
            when fibi.waybilltype=0 and fibi.BackStationStatus=5 then 3
            when fibi.waybilltype=1 and fibi.BackStationStatus=5 then 4
            when fibi.waybilltype=2 and fibi.BackStationStatus=3 then 5
            when fibi.waybilltype=2 and fibi.BackStationStatus=5 then 6 end as StatisticsType

            FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
            join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0 
            AND fibi.backstationtime >= @BackStationTimeStr--归班时间
            AND fibi.backstationtime < @BackStationTimeEnd--归班时间
            AND fibi.MerchantID = @MerchantID
            AND fibi.backStationStatus in (3,5)
            AND fibi.deliverStationId=@deliverStationId
            ) 
            select
            t.deliverstationid
            ,sum(t.ReceiveFee) as Fare,
            t.ReceiveStandard as Formula,
            sum(POSReceiveFee) as POSFare,
            POSReceiveStandard as POSFormula,
            sum(CashReceiveServiceFee) as ServiceFare,
            CashReceiveServiceStandard as ServiceFormula,
            sum(POSReceiveServiceFee) as POSServiceFare,
            POSReceiveServiceStandard as POSServiceFormula,
            COUNT(1) AS FormCount,--订单数量
            t.StatisticsType as CountType
             from t
             GROUP BY t.deliverstationid,
            t.StatisticsType,
            t.ReceiveStandard,
            POSReceiveStandard,
            CashReceiveServiceStandard,
            POSReceiveServiceStandard";
            SqlParameter[] parameters = { 
                                            new SqlParameter("@MerchantID",SqlDbType.Int), 
											new SqlParameter("@BackStationTimeStr",SqlDbType.DateTime),
											new SqlParameter("@BackStationTimeEnd",SqlDbType.DateTime),
                                            new SqlParameter("@deliverStationId",SqlDbType.Int)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverStationId;

            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        public int AddIncomeOtherFeeCount(IncomeOtherFeeCount model)
        {
            string TableName = @"RFD_FMS.dbo.FMS_IncomeOtherFeeCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" AccountNO , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" ExpressCompanyID , ");
            strSql.Append(" AreaType , ");
            strSql.Append(" CountType , ");
            strSql.Append(" CountDate , ");
            strSql.Append(" ProtectedStandard , ");
            strSql.Append(" ProtectedFee , ");
            strSql.Append(" ReceiveStandard , ");
            strSql.Append(" ReceiveFee , ");
            strSql.Append(" ReceivePOSStandard , ");
            strSql.Append(" ReceivePOSFee , ");
            strSql.Append(" CreateBy , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateBy , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" IsDeleted , ");
            strSql.Append(" StationID , ");
            strSql.Append(" ServesStandard , ");
            strSql.Append(" ServesFee , ");
            strSql.Append(" POSServesStandard , ");
            strSql.Append(" POSServesFee,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" @AccountNO , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @ExpressCompanyID , ");
            strSql.Append(" @AreaType , ");
            strSql.Append(" @CountType , ");
            strSql.Append(" @CountDate , ");
            strSql.Append(" @ProtectedStandard , ");
            strSql.Append(" @ProtectedFee , ");
            strSql.Append(" @ReceiveStandard , ");
            strSql.Append(" @ReceiveFee , ");
            strSql.Append(" @ReceivePOSStandard , ");
            strSql.Append(" @ReceivePOSFee , ");
            strSql.Append(" @CreateBy , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateBy , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @IsDeleted , ");
            strSql.Append(" @StationID , ");
            strSql.Append(" @ServesStandard , ");
            strSql.Append(" @ServesFee , ");
            strSql.Append(" @POSServesStandard , ");
            strSql.Append(" @POSServesFee,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
												new SqlParameter(string.Format("@{0}","AccountNO"), model.AccountNO),
												new SqlParameter(string.Format("@{0}","MerchantID"), model.MerchantID),
												new SqlParameter(string.Format("@{0}","ExpressCompanyID"), model.ExpressCompanyID),
												new SqlParameter(string.Format("@{0}","AreaType"), model.AreaType),
												new SqlParameter(string.Format("@{0}","CountType"), model.CountType),
												new SqlParameter(string.Format("@{0}","CountDate"), model.CountDate),
												new SqlParameter(string.Format("@{0}","ProtectedStandard"), model.ProtectedStandard),
												new SqlParameter(string.Format("@{0}","ProtectedFee"), model.ProtectedFee),
												new SqlParameter(string.Format("@{0}","ReceiveStandard"), model.ReceiveStandard),
												new SqlParameter(string.Format("@{0}","ReceiveFee"), model.ReceiveFee),
												new SqlParameter(string.Format("@{0}","ReceivePOSStandard"), model.ReceivePOSStandard),
												new SqlParameter(string.Format("@{0}","ReceivePOSFee"), model.ReceivePOSFee),
												new SqlParameter(string.Format("@{0}","CreateBy"), model.CreateBy),
												new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
												new SqlParameter(string.Format("@{0}","UpdateBy"), model.UpdateBy),
												new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
												new SqlParameter(string.Format("@{0}","IsDeleted"), model.IsDeleted),
												new SqlParameter(string.Format("@{0}","StationID"), model.StationID),
												new SqlParameter(string.Format("@{0}","ServesStandard"), model.ServesStandard),
												new SqlParameter(string.Format("@{0}","ServesFee"), model.ServesFee),
												new SqlParameter(string.Format("@{0}","POSServesStandard"), model.POSServesStandard),
												new SqlParameter(string.Format("@{0}","POSServesFee"), model.POSServesFee),
                                                new SqlParameter(string.Format("@{0}","IsChange"), true)
                                        };
            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        public DataTable GetIncomeHistoryCount(DateTime countdate)
        {
            string sql = @"
            select LogID,
            StatisticsType,
            MerchantID,
            StationID,
            ExpressCompanyID,
            StatisticsDate,
            IsSuccess,
            Reasons,
            Ip,
            CreateTime,
            UpdateTime From RFD_FMS.dbo.fms_incomestatlog fisl(nolock) where fisl.issuccess=0 and StatisticsDate<=@acountDate";
            SqlParameter[] parameters = { 
											new SqlParameter("@acountDate",SqlDbType.DateTime)
										};

            parameters[0].Value = countdate.AddDays(-2).ToShortDateString();
            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        public int IncomeDeliveryAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            string sql = @"WITH    t AS ( SELECT   fibi.WaybillNO
               FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
				join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						--AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= @CreatTimeStr
                        AND fibi.BackstationTime < @CreatTimeEnd
                        AND fibi.DeliverStationID = @DeliverStationID
                        AND fibi.ExpressCompanyID = @Warehouseid
                        AND fibi.MerchantID = @MerchantID
                        AND ISNULL(fibi.FinalExpressCompanyID, 0) = 0
                        AND fibi.BackStationStatus=3
               UNION ALL
               SELECT   fibi.WaybillNO
               FROM     LMS_RFD.dbo.FMS_IncomeBaseInfo AS fibi ( NOLOCK )
				join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						--AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= @CreatTimeStr
                        AND fibi.BackstationTime < @CreatTimeEnd
                        AND fibi.DeliverStationID = @DeliverStationID
                        AND fibi.FinalExpressCompanyID = @Warehouseid
                        AND fibi.MerchantID = @MerchantID
                        AND ISNULL(fibi.FinalExpressCompanyID, 0) > 0 
                        AND fibi.BackStationStatus=3            
             )
    SELECT  COUNT(1)
    FROM    t";
            SqlParameter[] parameters = { 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@Warehouseid",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate;
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = warehouseid;
            parameters[4].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);

        }

        public int IncomeReturnsAccountHistory(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid)
        {
            string sql = @"with t as(
                            SELECT fibi.WaybillNO
                            FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi (nolock)
                            join LMS_RFD.dbo.FMS_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
                            WHERE fifi.IsDeleted = 0
                            --and fifi.IsAccount=1
                            AND fibi.WaybillType IN ( '0', '1' )
                            AND fibi.ReturnTime >= @CreatTimeStr
                            AND fibi.ReturnTime < @CreatTimeEnd
                            AND fibi.DeliverStationID = @DeliverStationID
                            AND fibi.ReturnExpressCompanyID = @ReturnExpressId
                            AND fibi.MerchantID = @MerchantID
                            AND fibi.BackStationStatus=5
                            )
                            SELECT COUNT(1) FROM t";
            SqlParameter[] parameters = { 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnExpressId",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        public int IncomeVisitReturnsAccountHistory(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid)
        {
            string sql = @"select sum(a.a1) as num from (SELECT   COUNT(1) as a1 FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
        join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 
        --and fifi.IsAccount=1 
        AND fibi.WaybillType ='2' 
        and fibi.backstationstatus=3
        AND fibi.ReturnTime >= @ReturnTimeStr
        AND fibi.ReturnTime < @ReturnTimeEnd
        AND fibi.DeliverStationID = @DeliverStationID
        AND fibi.ReturnExpressCompanyID = @ReturnExpressId
        AND fibi.MerchantID = @MerchantID
union
SELECT   COUNT(1) as a1 FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
        join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 
        --and fifi.IsAccount=1 
        AND fibi.WaybillType ='2' 
        and fibi.backstationstatus=5
        AND fibi.backstationtime >= @ReturnTimeStr
        AND fibi.backstationtime < @ReturnTimeEnd
        AND fibi.DeliverStationID = @DeliverStationID
        AND fibi.MerchantID = @MerchantID) as a ";
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnExpressId",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        public int IncomeOtherReceiveFeeHistory(int merchantid, DateTime accountDate, int deliverstationid)
        {
            string sql = @" with t as(
            SELECT fibi.waybillno,
            fibi.merchantid,
            fibi.deliverstationid
            FROM  LMS_RFD.dbo.FMS_IncomeBaseInfo fibi ( NOLOCK )
            join LMS_RFD.dbo.fms_IncomeFeeInfo fifi (nolock) on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0
            --and fifi.IsReceive=1
            AND fibi.backstationtime >=@BackStationTimeStr --归班时间
            AND fibi.backstationtime < @BackStationTimeEnd--归班时间
            AND fibi.MerchantID = @MerchantID
            And fibi.deliverstationid=@DeliverStationID
            AND fibi.BackStationStatus in (3,5)
            ) 
            select count(1) from t
             ";
            SqlParameter[] parameters = { 
											new SqlParameter("@BackStationTimeStr",SqlDbType.DateTime),
											new SqlParameter("@BackStationTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = merchantid;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        public int IsIncomeHistoryCount(DateTime countdate)
        {
            string sql = @"
            select count(1) From RFD_FMS.dbo.fms_incomestatlog fisl(nolock) where StatisticsDate>=@acountDate and StatisticsDate<@acountEndDate";
            SqlParameter[] parameters = { 
											new SqlParameter("@acountDate",SqlDbType.DateTime),
                                            new SqlParameter("@acountEndDate",SqlDbType.DateTime)
										};

            parameters[0].Value = countdate.ToShortDateString();
            parameters[1].Value = countdate.AddDays(1).ToShortDateString();

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        #endregion
    }
}
