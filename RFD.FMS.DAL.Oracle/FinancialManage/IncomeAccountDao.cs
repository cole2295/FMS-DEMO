using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using System.Data.SqlClient;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class IncomeAccountDao : OracleDao, IIncomeAccountDao
	{
        public string SqlStr { get; set; }

        public DataTable GetUniteAccount(IncomeSearchCondition condition)
        {
            SqlStr = @"
WITH    t AS ( SELECT   CASE WHEN fidc.CountType = 0 THEN 'D'
                                            ELSE 'DV'
                                       END CountTypeStr,
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
               FROM     FMS_IncomeDeliveryCount  fidc
               WHERE    fidc.IsDeleted = 0
						AND (fidc.AccountNO='' or fidc.AccountNO is null)
                        AND fidc.MerchantID = :MerchantID
                        AND fidc.CountDate >= :dateStr
                        AND fidc.CountDate <= :dateEnd
               GROUP BY fidc.ExpressCompanyID ,
                        fidc.AreaType ,
                        fidc.Formula ,
                        fidc.CountType
               UNION ALL
               SELECT     CASE WHEN firc.CountType = 0 THEN 'R'
                                            ELSE 'RV'
                                       END CountTypeStr ,
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
               FROM FMS_IncomeReturnsCount  firc
               WHERE    firc.IsDeleted = 0
						AND (firc.AccountNO='' or firc.AccountNO is null)
                        AND firc.MerchantID = :MerchantID
                        AND firc.CountDate >= :dateStr
                        AND firc.CountDate <= :dateEnd
               GROUP BY firc.ExpressCompanyID ,
                        firc.AreaType ,
                        firc.Formula ,
                        firc.CountType
               UNION ALL
               SELECT   CASE WHEN fivrc.CountType = 0 THEN 'V'
                                            ELSE 'VV'
                                       END CountTypeStr,
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
               FROM FMS_IncomeVisitReturnsCount  fivrc
               WHERE    fivrc.IsDeleted = 0
						AND (fivrc.AccountNO='' or fivrc.AccountNO is null)
                        AND fivrc.MerchantID = :MerchantID
                        AND fivrc.CountDate >= :dateStr
                        AND fivrc.CountDate <= :dateEnd
               GROUP BY fivrc.ExpressCompanyID ,
                        fivrc.AreaType ,
                        fivrc.Formula ,
                        fivrc.CountType
               UNION ALL
               SELECT   CASE WHEN fiofc.CountType = 1 THEN 'D'
                                            WHEN fiofc.CountType = 2 THEN 'DV'
                                            WHEN fiofc.CountType = 3 THEN 'R'
                                            WHEN fiofc.CountType = 4 THEN 'RV'
                                            WHEN fiofc.CountType = 5 THEN 'V'
                                            WHEN fiofc.CountType = 6 THEN 'VV'
                                       END CountTypeStr,
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
               FROM FMS_IncomeOtherFeeCount  fiofc
               WHERE    fiofc.IsDeleted = 0
						AND (fiofc.AccountNO='' or fiofc.AccountNO is null)
                        AND fiofc.MerchantID = :MerchantID
                        AND fiofc.CountDate >= :dateStr
                        AND fiofc.CountDate <= :dateEnd
               GROUP BY fiofc.ExpressCompanyID ,
                        fiofc.AreaType ,
                        fiofc.CountType,
                        fiofc.ProtectedStandard,
                        fiofc.ReceiveStandard,
                        fiofc.ReceivePOSStandard
             )
   SELECT  t.ExpressCompanyID,AreaType,
			sum(CASE CountTypeStr WHEN 'D' THEN CountNum ELSE 0 END) AS DeliveryNum,
			max(CASE CountTypeStr WHEN 'D' THEN Formula ELSE '' END) AS DeliveryStandard,
			sum(CASE CountTypeStr WHEN 'D' THEN CountFare ELSE 0 END) AS DeliveryFare,
			sum(CASE CountTypeStr WHEN 'DV' THEN CountNum ELSE 0 END) AS DeliveryVNum,
			max(CASE CountTypeStr WHEN 'DV' THEN Formula ELSE '' END) AS DeliveryVStandard,
			sum(CASE CountTypeStr WHEN 'DV' THEN CountFare ELSE 0 END) AS DeliveryVFare,
			sum(CASE CountTypeStr WHEN 'R' THEN CountNum ELSE 0 END) AS ReturnsNum,
			max(CASE CountTypeStr WHEN 'R' THEN Formula ELSE '' END) AS RetrunsStandard,
			sum(CASE CountTypeStr WHEN 'R' THEN CountFare ELSE 0 END) AS RetrunsFare,
			sum(CASE CountTypeStr WHEN 'RV' THEN CountNum ELSE 0 END) AS ReturnsVNum,
			max(CASE CountTypeStr WHEN 'RV' THEN Formula ELSE '' END) AS ReturnsVStandard,
			sum(CASE CountTypeStr WHEN 'RV' THEN CountFare ELSE 0 END) AS ReturnsVFare,
			sum(CASE CountTypeStr WHEN 'V' THEN CountNum ELSE 0 END) AS VisitReturnsNum,
			MAX(CASE CountTypeStr WHEN 'V' THEN Formula ELSE '' END) AS VisitReturnsStandard,
			sum(CASE CountTypeStr WHEN 'V' THEN CountFare ELSE 0 END) AS VisitReturnsFare,
			sum(CASE CountTypeStr WHEN 'VV' THEN CountNum ELSE 0 END) AS VisitReturnsVNum,
			MAX(CASE CountTypeStr WHEN 'VV' THEN Formula ELSE '' END) AS VisitReturnsVStandard,
			sum(CASE CountTypeStr WHEN 'VV' THEN CountFare ELSE 0 END) AS VisitReturnsVFare,
			MAX(PFormula) AS ProtectedStandard,
			SUM(PCountFare) AS ProtectedFee,
			MAX(RFormula) AS ReceiveStandard,
			SUM(RCountFare) AS ReceiveFee,
			MAX(RPFormula) AS ReceivePOSStandard,
			SUM(RPCountFare) AS ReceivePOSFee,
			ec.CompanyName,
			0 AS DataType
    FROM    t
    JOIN ExpressCompany  ec ON t.ExpressCompanyID=ec.ExpressCompanyID AND ec.CompanyFlag=1
    GROUP BY t.ExpressCompanyID,t.AreaType,ec.CompanyName
    ORDER BY t.ExpressCompanyID,t.AreaType
";
            OracleParameter[] parameters ={
										 new OracleParameter(":MerchantID",OracleDbType.Int32),
										 new OracleParameter(":dateStr",OracleDbType.Date),
										 new OracleParameter(":dateEnd",OracleDbType.Date),
									};
            parameters[0].Value = condition.MerchantID;
            parameters[1].Value = condition.DateStr;
            parameters[2].Value = condition.DateEnd;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters);
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public bool ChanageCountAcountNO(IncomeSearchCondition condition, int createBy, string accountNo)
        {
            SqlStr = @"
 begin
                            :n := 0;
UPDATE  FMS_IncomeDeliveryCount
SET     AccountNO = :AccountNO ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   MerchantID = :MerchantID
        AND CountDate >=:DateStr
        AND CountDate <= :DateEnd
		AND (AccountNO='' or AccountNO is null);
     :n := :n+sql%rowcount;    
UPDATE  FMS_IncomeReturnsCount
SET     AccountNO = :AccountNO ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   MerchantID = :MerchantID
        AND CountDate >= :DateStr
        AND CountDate <= :DateEnd
		AND (AccountNO='' or AccountNO is null);
       :n := :n+sql%rowcount; 
UPDATE FMS_IncomeVisitReturnsCount
SET     AccountNO = :AccountNO ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   MerchantID = :MerchantID
        AND CountDate >= :DateStr
        AND CountDate <= :DateEnd
		AND (AccountNO='' or AccountNO is null);
 :n := :n+sql%rowcount;       
UPDATE FMS_IncomeOtherFeeCount
SET     AccountNO = :AccountNO ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   MerchantID = :MerchantID
        AND CountDate >=:DateStr
        AND CountDate <= :DateEnd
		AND (AccountNO='' or AccountNO is null);
		 :n := :n+sql%rowcount;
end;
";

            OracleParameter[] parameters ={
										 new OracleParameter(":MerchantID",OracleDbType.Int32),
										 new OracleParameter(":dateStr",OracleDbType.Date),
										 new OracleParameter(":dateEnd",OracleDbType.Date),
										 new OracleParameter(":AccountNO",OracleDbType.Varchar2,40),
										 new OracleParameter(":UpdateBy",OracleDbType.Int32),
										  new OracleParameter(":n", OracleDbType.Decimal),
									};
            parameters[0].Value = condition.MerchantID;
            parameters[1].Value = condition.DateStr;
			parameters[2].Value = condition.DateEnd; ;
            parameters[3].Value = accountNo;
            parameters[4].Value = createBy;
			parameters[5].Direction = ParameterDirection.Output;
             OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters);
			 int n = DataConvert.ToInt(parameters[0].Value);
			 return n >= 0;
		}

        public bool AddAccountDetail(IncomeAccountDetail m, int createBy, string accountNo)
        {
            SqlStr = @"
INSERT INTO FMS_IncomeAccountDetail
        (
		  DETAILID,
		  AccountNO ,
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
          DeliveryFee,
          DiscountFee
        )
VALUES  ( 
          :DETAILID,
          :AccountNO ,
          :ExpressCompanyID ,
          :AreaType ,
          :DeliveryNum ,
          :DeliveryVNum ,
          :ReturnsNum ,
          :ReturnsVNum ,
          :VisitReturnsNum ,
          :VisitReturnsVNum ,
          :DeliveryStandard ,
          :DeliveryFare ,
          :DeliveryVStandard ,
          :DeliveryVFare ,
          :RetrunsStandard ,
          :RetrunsFare ,
          :ReturnsVStandard ,
          :ReturnsVFare ,
          :VisitReturnsStandard ,
          :VisitReturnsFare ,
          :VisitReturnsVStandard ,
          :VisitReturnsVFare ,
          :ProtectedStandard ,
          :ProtectedFee ,
          :ReceiveStandard ,
          :ReceiveFee ,
          :ReceivePOSStandard ,
          :ReceivePOSFee ,
          :OtherFee ,
          :Fare ,
          :DataType ,
          :CreateBy ,
          SysDate ,
          0 ,
          SysDate ,
          0,
          :DeliveryFee,
          :DiscountFee

        )
";

            OracleParameter[] parameters ={
											new OracleParameter(":AccountNO",OracleDbType.Varchar2,40),
											new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
											new OracleParameter(":AreaType",OracleDbType.Decimal),
											new OracleParameter(":DeliveryNum",OracleDbType.Decimal),
											new OracleParameter(":DeliveryVNum",OracleDbType.Decimal),
											new OracleParameter(":ReturnsNum",OracleDbType.Decimal),
											new OracleParameter(":ReturnsVNum",OracleDbType.Decimal),
											new OracleParameter(":VisitReturnsNum",OracleDbType.Decimal),
											new OracleParameter(":VisitReturnsVNum",OracleDbType.Decimal),
											new OracleParameter(":DeliveryStandard",OracleDbType.Varchar2,300),
											new OracleParameter(":DeliveryFare",OracleDbType.Decimal),
											new OracleParameter(":DeliveryVStandard",OracleDbType.Varchar2,300),
											new OracleParameter(":DeliveryVFare",OracleDbType.Decimal),
											new OracleParameter(":RetrunsStandard",OracleDbType.Varchar2,300),
											new OracleParameter(":RetrunsFare",OracleDbType.Decimal),
											new OracleParameter(":ReturnsVStandard",OracleDbType.Varchar2,300),
											new OracleParameter(":ReturnsVFare",OracleDbType.Decimal),
											new OracleParameter(":VisitReturnsStandard",OracleDbType.Varchar2,300),
											new OracleParameter(":VisitReturnsFare",OracleDbType.Decimal),
											new OracleParameter(":VisitReturnsVStandard",OracleDbType.Varchar2,300),
											new OracleParameter(":VisitReturnsVFare",OracleDbType.Decimal),
											new OracleParameter(":ProtectedStandard",OracleDbType.Decimal),
											new OracleParameter(":ProtectedFee",OracleDbType.Decimal),
											new OracleParameter(":ReceiveStandard",OracleDbType.Decimal),
											new OracleParameter(":ReceiveFee",OracleDbType.Decimal),
											new OracleParameter(":ReceivePOSStandard",OracleDbType.Decimal),
											new OracleParameter(":ReceivePOSFee",OracleDbType.Decimal),
											new OracleParameter(":OtherFee",OracleDbType.Decimal),
											new OracleParameter(":Fare",OracleDbType.Decimal),
											new OracleParameter(":DataType",OracleDbType.Decimal),
											new OracleParameter(":CreateBy",OracleDbType.Decimal),
											new OracleParameter(":DETAILID",OracleDbType.Decimal), 
                                            new OracleParameter(":DeliveryFee",OracleDbType.Decimal), 
                                            new OracleParameter(":DiscountFee",OracleDbType.Decimal), 
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
			parameters[31].Value = GetIdNew("SEQ_FMS_IncomeAccountDetail");
            parameters[32].Value = m.DeliveryFee;
            parameters[33].Value = m.DiscountFee;
        	int obj = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters);
			return obj > 0;
        }

        public bool AddAccount(IncomeSearchCondition condition, int createBy, string accountNo)
        {
            SqlStr = @"
INSERT INTO FMS_IncomeAccount
        ( ACCOUNTID,
		  AccountNO ,
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
          IsPushed
        )
VALUES  ( :ACCOUNTID,
          :AccountNO ,
          :MerchantID ,
          :AccountStatus ,
          :CreateBy ,
          SysDate ,
          0 ,
          SysDate ,
          0 ,
          SysDate ,
          :SearchDateStr ,
          :SearchDateEnd ,
          0,
          0
        )
";

            OracleParameter[] parameters ={
										 new OracleParameter(":MerchantID",OracleDbType.Decimal),
										 new OracleParameter(":SearchDateStr",OracleDbType.Date),
										 new OracleParameter(":SearchDateEnd",OracleDbType.Date),
										 new OracleParameter(":AccountNO",OracleDbType.Varchar2,40),
										 new OracleParameter(":CreateBy",OracleDbType.Decimal),
										 new OracleParameter(":AccountStatus",OracleDbType.Decimal),
									     new OracleParameter(":ACCOUNTID",OracleDbType.Decimal), 
									};
            parameters[0].Value = condition.MerchantID;
            parameters[1].Value = condition.DateStr;
            parameters[2].Value = condition.DateEnd;
            parameters[3].Value = accountNo;
            parameters[4].Value = createBy;
            parameters[5].Value = (int)EnumAccountAudit.A1;
			parameters[6].Value = GetIdNew("SEQ_FMS_IncomeAccount");
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
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
        fiad.DeliveryFee,
        fiad.DiscountFee,
        fiad.OtherFee,
        fiad.Fare
FROM    FMS_IncomeAccount fia
        JOIN FMS_IncomeAccountDetail fiad ON fia.AccountNO = fiad.AccountNO
                                                              AND fiad.DataType = 2
        JOIN StatusCodeInfo sci ON fia.AccountStatus = sci.CodeNo
                                                     AND sci.CodeType = 'CODAccount'
        JOIN MerchantBaseInfo  mbi ON fia.MerchantID = mbi.ID
WHERE   fia.IsDeleted = 0 and fiad.IsDeleted = 0 {0}
";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(accountStatus) && accountStatus != "-1")
            {
                sbWhere.Append(" AND fia.AccountStatus=:AccountStatus ");
                parameters.Add(new OracleParameter(":AccountStatus", OracleDbType.Decimal) { Value = accountStatus });
            }

            if (!string.IsNullOrEmpty(merchantId) && merchantId != "-1")
            {
                sbWhere.Append(" AND fia.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(dateStr))
            {
                sbWhere.Append(" AND fia.CreateTime>=:CreateTimeS ");
                parameters.Add(new OracleParameter(":CreateTimeS", OracleDbType.Date) { Value = DataConvert.ToDateTime(dateStr) });
            }

            if (!string.IsNullOrEmpty(dateEnd))
            {
                sbWhere.Append(" AND fia.CreateTime<=:CreateTimeE ");
                parameters.Add(new OracleParameter(":CreateTimeE", OracleDbType.Date) { Value =DataConvert.ToDateTime(dateEnd) });
            }

            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fia.AccountNO<=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 40) { Value = accountNo });
            }

            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
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
        fiad.DeliveryFee,
        fiad.DiscountFee,
        fiad.OtherFee,
        fiad.Fare,
        fiad.DataType
FROM    FMS_IncomeAccountDetail  fiad
LEFT JOIN ExpressCompany  ec ON fiad.ExpressCompanyID = ec.ExpressCompanyID AND ec.CompanyFlag=1
WHERE   fiad.IsDeleted = 0 {0}
ORDER BY fiad.ExpressCompanyID,fiad.AreaType,fiad.DataType
";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fiad.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 40) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(detailId))
            {
                sbWhere.Append(" AND fiad.DetailID=:DetailID ");
                parameters.Add(new OracleParameter(":DetailID", OracleDbType.Decimal) { Value = detailId });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters.ToArray()).Tables[0];
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
        fiad.DeliveryFee,
        fiad.DiscountFee,
        fiad.OtherFee,
        fiad.Fare,
        fiad.DataType,
        fiad.DeliveryFee,
        fiad.DiscountFee
FROM    FMS_IncomeAccountDetail  fiad 
left JOIN PS_PMS.ExpressCompany  ec ON fiad.stationid = ec.ExpressCompanyID --AND ec.CompanyFlag=1
WHERE   fiad.IsDeleted = 0 and fiad.datatype in (0,2) {0}
ORDER BY fiad.ExpressCompanyID,fiad.AreaType,fiad.DataType
";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fiad.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 40) { Value = accountNo });
            }

            if (!string.IsNullOrEmpty(dateType))
            {
                sbWhere.Append(" AND fiad.DataType=:DataType ");
                parameters.Add(new OracleParameter(":DataType", OracleDbType.Decimal) { Value = dateType });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public DataTable GetAccountSearchCondition(string accountNo)
        {
            SqlStr = @"
SELECT  fia.AccountNO ,
        fia.MerchantID ,
        fia.SearchDateStr ,
        fia.SearchDateEnd ,
        mbi.MerchantName
FROM    FMS_IncomeAccount  fia
JOIN MerchantBaseInfo  mbi ON mbi.ID = fia.MerchantID
WHERE   fia.IsDeleted = 0 {0}
";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fia.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 20) { Value = accountNo });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public bool DeleteCount(string accountNo, int updateBy)
        {
            SqlStr = @" BEGIN 
:n := 0;
UPDATE  FMS_IncomeDeliveryCount
SET     AccountNO = '' ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   AccountNO = :AccountNO AND IsDeleted=0;
:n := :n+sql%rowcount;

UPDATE  FMS_IncomeVisitReturnsCount
SET     AccountNO = '' ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   AccountNO = :AccountNO AND IsDeleted=0;
:n := :n+sql%rowcount;

UPDATE  FMS_IncomeReturnsCount
SET     AccountNO = '' ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   AccountNO = :AccountNO AND IsDeleted=0;
:n := :n+sql%rowcount;

UPDATE  FMS_IncomeOtherFeeCount
SET     AccountNO = '' ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   AccountNO = :AccountNO AND IsDeleted=0;
:n := :n+sql%rowcount;

END;
";
            OracleParameter[] parameters ={
                                         new OracleParameter(":n",OracleDbType.Decimal), 
										 new OracleParameter(":AccountNO",OracleDbType.Varchar2,40),
										 new OracleParameter(":UpdateBy",OracleDbType.Decimal),
									};
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = accountNo;
            parameters[2].Value = updateBy;
            //SqlStr = SqlStr.Replace("\r\n", " ");
           OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters);

            return DataConvert.ToInt(parameters[0].Value)> 0;
        }

        public bool DeleteAccount(string accountNo, int updateBy)
        {
            SqlStr = @" BEGIN 
:n :=0;     
UPDATE  FMS_IncomeAccount
SET     IsDeleted = 1 ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   AccountNO = :AccountNO
        AND IsDeleted = 0;
:n := :n+sql%rowcount;
     
UPDATE  FMS_IncomeAccountDetail
SET     IsDeleted = 1 ,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   AccountNO = :AccountNO
        AND IsDeleted = 0;
:n := :n+sql%rowcount;

END;
";

            OracleParameter[] parameters ={
                                         new OracleParameter(":n",OracleDbType.Decimal), 
										 new OracleParameter(":AccountNO",OracleDbType.Varchar2,40),
										 new OracleParameter(":UpdateBy",OracleDbType.Decimal),
									};
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = accountNo;
            parameters[2].Value = updateBy;
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters);
            return DataConvert.ToInt(parameters[0].Value) > 0;
        }

        public bool UpdateAccountDetailFee(IncomeAccountDetail i)
        {
            SqlStr = @"
UPDATE  FMS_IncomeAccountDetail
SET     OtherFee = :OtherFee ,
		Fare=DeliveryFare+DeliveryVFare+RetrunsFare+ReturnsVFare
			+VisitReturnsFare+VisitReturnsVFare+ProtectedFee
			+ReceiveFee+ReceivePOSFee+:OtherFee+ DeliveryFee+DiscountFee,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   DataType = 1
        AND DetailID = :DetailID
        AND IsDeleted=0
";
            OracleParameter[] parameters ={
										 new OracleParameter(":DetailID",OracleDbType.Long),
										 new OracleParameter(":OtherFee",OracleDbType.Decimal),
										 new OracleParameter(":UpdateBy",OracleDbType.Decimal),
									};
            parameters[0].Value = i.DetailID;
            parameters[1].Value = i.OtherFee;
            parameters[2].Value = i.UpdateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateAccountDetailFeeAll(IncomeAccountDetail i)
        {
            SqlStr = @"
SELECT  :AccountNO = AccountNO
FROM    FMS_IncomeAccountDetail  fiad
WHERE   DetailID = :DetailID
        AND fiad.IsDeleted = 0

SELECT  :OtherFee = SUM(OtherFee)
FROM    FMS_IncomeAccountDetail  fiad
WHERE   AccountNO = :AccountNO
        AND fiad.DataType = 1
        AND fiad.IsDeleted = 0

UPDATE  FMS_IncomeAccountDetail
SET     OtherFee = :OtherFee ,
		Fare=DeliveryFare+DeliveryVFare+RetrunsFare+ReturnsVFare
			+VisitReturnsFare+VisitReturnsVFare+ProtectedFee
			+ReceiveFee+ReceivePOSFee+:OtherFee+ DeliveryFee+DiscountFee,
        UpdateBy = :UpdateBy ,
        UpdateTime = SysDate
WHERE   DataType = 2
        AND AccountNO = :AccountNO
        AND IsDeleted = 0
";
            OracleParameter[] parameters = {
											new OracleParameter(":DetailID",OracleDbType.Long),
											new OracleParameter(":UpdateBy",OracleDbType.Decimal),
											new OracleParameter(":AccountNO",OracleDbType.Varchar2,40), 
											new OracleParameter(":OtherFee",OracleDbType.Decimal,18,ParameterDirection.Input,false,18,2,"OtherFee",DataRowVersion.Current,"0.00"),
										 };
            parameters[0].Value = i.DetailID;
            parameters[1].Value = i.UpdateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        /// <summary>
        /// 修改结算各项总金额
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool UpdateAccountDetailFeeAllByAccountNo(IncomeAccountDetail i)
        {
            SqlStr = @"
            UPDATE  FMS_IncomeAccountDetail
            SET     OtherFee = :OtherFee,ReceiveFee=:ReceiveFee,ReceivePOSFee=:ReceivePOSFee,ProtectedFee=:ProtectedFee,
                    OverAreaSubsidy=:OverAreaSubsidy,KPI=:KPI,LostDeduction=:LostDeduction,ResortDeduction=:ResortDeduction,
                    DeliveryFee=:DeliveryFee,DiscountFee=:DiscountFee,
		            Fare=DeliveryFare+DeliveryVFare+RetrunsFare+ReturnsVFare
			            +VisitReturnsFare+VisitReturnsVFare+ProtectedFee
			            +ReceiveFee+ReceivePOSFee+:OtherFee+:OverAreaSubsidy+:KPI+:LostDeduction+:ResortDeduction+:DeliveryFee+:DiscountFee,
                    UpdateBy = :UpdateBy ,
                    UpdateTime = SysDate
            WHERE   DataType = 2
                    AND AccountNO = :AccountNO
                    AND IsDeleted = 0
            ";
            OracleParameter[] parameters = {
											new OracleParameter(":UpdateBy",OracleDbType.Decimal),
											new OracleParameter(":AccountNO",OracleDbType.Varchar2,40), 
											new OracleParameter(":OtherFee",OracleDbType.Decimal,18),
                                            new OracleParameter(":ReceiveFee",OracleDbType.Decimal,18),
                                            new OracleParameter(":ReceivePOSFee",OracleDbType.Decimal,18),
                                            new OracleParameter(":ProtectedFee",OracleDbType.Decimal,18),
                                            new OracleParameter(":OverAreaSubsidy",OracleDbType.Decimal,18),
                                            new OracleParameter(":KPI",OracleDbType.Decimal,18),
                                            new OracleParameter(":LostDeduction",OracleDbType.Decimal,18),
                                            new OracleParameter(":ResortDeduction",OracleDbType.Decimal,18),
                                            new OracleParameter(":DeliveryFee",OracleDbType.Decimal,18),
                                            new OracleParameter(":DiscountFee",OracleDbType.Decimal,18), 
                                            
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
            parameters[10].Value = i.DeliveryFee;
            parameters[11].Value = i.DiscountFee;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateAccountStatus(string accountNo, int auditBy, int status)
        {
            SqlStr = @"
UPDATE  FMS_IncomeAccount
SET     AccountStatus = :AccountStatus ,
        AuditBy = :AuditBy ,
        AuditTime = SysDate
WHERE   AccountNO = :AccountNO
		AND IsDeleted=0
";
            OracleParameter[] parameters = {
											new OracleParameter(":AccountStatus",OracleDbType.Decimal),
											new OracleParameter(":AuditBy",OracleDbType.Decimal),
											new OracleParameter(":AccountNO",OracleDbType.Varchar2,40), 
										 };
            parameters[0].Value = status;
            parameters[1].Value = auditBy;
            parameters[2].Value = accountNo;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public DataTable GetAccountAreaSummary(string accountNo)
        {
            SqlStr = @"
                SELECT  AreaType ,
                        SUM(fiad.DeliveryNum) + SUM(fiad.ReturnsNum)  
                        + SUM(fiad.VisitReturnsNum) as DeliveryNum  ,
                         SUM(fiad.DeliveryFare) + SUM(fiad.RetrunsFare)
                        + SUM(fiad.VisitReturnsFare) as DeliveryFare  ,
                          SUM(fiad.DeliveryVNum) + SUM(fiad.ReturnsVNum)
                        + SUM(fiad.VisitReturnsVNum) as ReturnsVNum ,
                         SUM(fiad.DeliveryVFare) + SUM(fiad.ReturnsVFare)
                        + SUM(fiad.VisitReturnsVFare) as DeliveryVFare,
                        SUM(fiad.ProtectedFee) as ProtectedFee,
                         SUM(fiad.ReceiveFee) as ReceiveFee,
                        SUM(fiad.ReceivePOSFee)  as  ReceivePOSFee ,
                        SUM(nvl(fiad.DELIVERYFEE,0)) AS DeliveryFee,
                        SUM(nvl(fiad.DISCOUNTFEE,0)) AS DiscountFee,
                          SUM(fiad.OtherFee) as OtherFee
                FROM    FMS_IncomeAccountDetail  fiad
                WHERE   fiad.IsDeleted = 0
                        AND fiad.DataType = 0
                        {0}
                GROUP BY fiad.AreaType
                ";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(accountNo))
            {
                sbWhere.Append(" AND fiad.AccountNO=:AccountNO ");
                parameters.Add(new OracleParameter(":AccountNO", OracleDbType.Varchar2, 40) { Value = accountNo });
            }
            SqlStr = string.Format(SqlStr, sbWhere.ToString());
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        #region 收入结算相关服务方法
        /// <summary>
        /// 返回商家列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMerchantInfo(string distributionCode)
        {
            SqlStr = @" SELECT m.ID,m.MerchantName
 FROM DistributionMerchantRelation dmr
JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0 AND m.Sources = 2
AND dmr.DistributionCode=:DistributionCode 
ORDER BY m.CreatTime
                    ";
            OracleParameter[] parameterList ={
                                              new OracleParameter(":DistributionCode",OracleDbType.Varchar2)
                                         };
            parameterList[0].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameterList.ToArray()).Tables[0];
        }
        /// <summary>
        /// 返回商家列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMerchantInfo()
        {
            SqlStr = @"select id,merchantname from merchantbaseinfo mf
                    where mf.id not in (8,9) and isdeleted=0
                    ";
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr).Tables[0];
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
                        FinalExpressID = CASE WHEN NVL(fibi.FinalExpressCompanyID,0)=0 THEN 
							 CONVERT(NVARCHAR(20), fibi.ExpressCompanyID) 
                          ELSE
                             CONVERT(NVARCHAR(20), fibi.FinalExpressCompanyID) END
               FROM FMS_IncomeBaseInfo AS fibi
                join FMS_IncomeFeeInfo as fifi  on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.backstationtime >= :CountStartDate
						and fibi.backstationtime<:CountEndDate
						and fibi.MerchantID=:MerchantId
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
            LEFT JOIN AreaExpressLevelIncome ael ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1,2)
                                                         AND ael.expresscompanyid = t.FinalExpressID
                                                         AND ael.MerchantID = t.MerchantID
           
";

            OracleParameter[] parameters ={
										   new OracleParameter(":MerchantId",OracleDbType.Decimal),
										   new OracleParameter(":CountStartDate",OracleDbType.Date),
                                            new OracleParameter(":CountEndDate",OracleDbType.Date),
									  };
            parameters[0].Value = merchantid;
            parameters[1].Value = countDate;
            parameters[2].Value = countDate.AddDays(1);

            DataTable dt = OracleHelper.ExecuteDataset(Connection, CommandType.Text, SqlStr, parameters).Tables[0];
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
                          CASE WHEN NVL(fibi.FinalExpressCompanyID,0)=0 THEN 
							 to_char(fibi.ExpressCompanyID) 
                          ELSE
                             to_char(fibi.FinalExpressCompanyID) END warehouseid,
                        fifi.AccountStandard,
                        fifi.AccountFare
               FROM     FMS_IncomeBaseInfo  fibi
               join     fms_IncomeFeeInfo fifi  
               on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fibi.BackstationTime <= to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fibi.MerchantID = :MerchantID
                        AND fibi.BackStationstatus=3
                        AND fibi.DeliverStationID=:DeliverStationID
                        AND fibi.FinalExpressCompanyID=:FinalExpressCompanyID
             )
    SELECT  t.DeliverStationID,
			t.warehouseid,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.AccountFare) AS Fare ,
            t.AccountStandard AS Formula ,
            SUM(NVL(t.AccountWeight,0)) AS WEIGHT ,
            t.WaybillType
    FROM    t
            LEFT JOIN AreaExpressLevelIncome ael ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1,2)
                                                         AND ael.warehouseId = t.warehouseid
                                                         AND ael.MerchantID = t.MerchantID
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.AccountStandard
";
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
                                            new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
                                            new OracleParameter(":FinalExpressCompanyID",OracleDbType.Decimal)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverstationid;
            parameters[4].Value = expresscompanyid;

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
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
                                    CASE WHEN NVL(fibi.FinalExpressCompanyID,0)=0 THEN 
                                                     to_char(fibi.ExpressCompanyID) 
                                                  ELSE
                                                     to_char(fibi.FinalExpressCompanyID) END warehouseid
                           FROM     FMS_IncomeBaseInfo  fibi
                           join     fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0
                                    AND fibi.WaybillType IN ( '0', '1' )
                                    AND fibi.BackstationTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                                    AND fibi.BackstationTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                                    AND fibi.MerchantID = :MerchantID
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
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
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
               FROM FMS_IncomeBaseInfo  fibi
				join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fibi.BackstationTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fibi.DeliverStationID = :DeliverStationID
                        AND fibi.ExpressCompanyID = :Warehouseid
                        AND fibi.MerchantID = :MerchantID
                        AND NVL(fibi.FinalExpressCompanyID, 0) = 0
                        AND fibi.BackStationstatus=3
               UNION ALL
               SELECT   fibi.WaybillNO
               FROM     FMS_IncomeBaseInfo  fibi
				join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fibi.BackstationTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fibi.DeliverStationID = :DeliverStationID
                        AND fibi.FinalExpressCompanyID = :Warehouseid
                        AND fibi.MerchantID = :MerchantID
                        AND NVL(fibi.FinalExpressCompanyID, 0) > 0
                        AND fibi.BackStationstatus=3             
             )
    SELECT  COUNT(1)
    FROM    t";
            OracleParameter[] parameters = { 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Int32),
											new OracleParameter(":Warehouseid",OracleDbType.Int32),
                                            new OracleParameter(":MerchantID",OracleDbType.Int32)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = warehouseid;
            parameters[4].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));

        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeDeliveryAccount(IncomeDeliveryCount model)
        {
            if (model.CountID <= 0)
            {
                model.CountID = GetIdNew("SEQ_FMS_INCOMEDELIVERYCOUNT");
            }

            string TableName = @"FMS_IncomeDeliveryCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" CountID , ");
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
            strSql.Append(" StationID  ");
            strSql.Append(") values (");
            strSql.Append(" :CountID , ");
            strSql.Append(" :AccountNO , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :ExpressCompanyID , ");
            strSql.Append(" :AreaType , ");
            strSql.Append(" :Weight , ");
            strSql.Append(" :CountType , ");
            strSql.Append(" :CountDate , ");
            strSql.Append(" :CountNum , ");
            strSql.Append(" :Fare , ");
            strSql.Append(" :Formula , ");
            strSql.Append(" :CreateBy , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateBy , ");
            strSql.Append(" :UpdateTime , ");
            strSql.Append(" :IsDeleted , ");
            strSql.Append(" :StationID  ");
            strSql.Append(") ");
            OracleParameter[] parameters = 
            {
                new OracleParameter(string.Format(":{0}","CountID"), model.CountID),
		        new OracleParameter(string.Format(":{0}","AccountNO"), model.AccountNO),
		        new OracleParameter(string.Format(":{0}","MerchantID"), model.MerchantID),
		        new OracleParameter(string.Format(":{0}","ExpressCompanyID"), model.ExpressCompanyID),
		        new OracleParameter(string.Format(":{0}","AreaType"), model.AreaType),
		        new OracleParameter(string.Format(":{0}","Weight"), model.Weight),
		        new OracleParameter(string.Format(":{0}","CountType"), model.CountType),
		        new OracleParameter(string.Format(":{0}","CountDate"), model.CountDate),
		        new OracleParameter(string.Format(":{0}","CountNum"), model.CountNum),
		        new OracleParameter(string.Format(":{0}","Fare"), model.Fare),
		        new OracleParameter(string.Format(":{0}","Formula"), model.Formula),
		        new OracleParameter(string.Format(":{0}","CreateBy"), model.CreateBy),
		        new OracleParameter(string.Format(":{0}","CreateTime"), model.CreateTime),
		        new OracleParameter(string.Format(":{0}","UpdateBy"), model.UpdateBy),
		        new OracleParameter(string.Format(":{0}","UpdateTime"), model.UpdateTime),
		        new OracleParameter(string.Format(":{0}","IsDeleted"), model.IsDeleted),
		        new OracleParameter(string.Format(":{0}","StationID"), model.StationID)	
            };
            object obj = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

             return Convert.ToInt32(obj);
           
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddIncomeStatLog(IncomeStatLog model)
        {
            if (model.LogID <= 0)
            {
                model.LogID = GetIdNew("SEQ_FMS_INCOMESTATLOG");
            }

            string TableName = @"FMS_IncomeStatLog";

            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" LogID, ");
            strSql.Append(" StatisticsType , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" StationID , ");
            strSql.Append(" ExpressCompanyID , ");
            strSql.Append(" StatisticsDate , ");
            strSql.Append(" IsSuccess , ");
            strSql.Append(" Reasons , ");
            strSql.Append(" Ip , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateTime  ");
            strSql.Append(") values (");
            strSql.Append(" :LogID , ");
            strSql.Append(" :StatisticsType , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :StationID , ");
            strSql.Append(" :ExpressCompanyID , ");
            strSql.Append(" :StatisticsDate , ");
            strSql.Append(" :IsSuccess , ");
            strSql.Append(" :Reasons , ");
            strSql.Append(" :Ip , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateTime  ");
            strSql.Append(") ");
            OracleParameter[] parameters = 
            {
                new OracleParameter(string.Format(":{0}","LogID"), model.LogID),
				new OracleParameter(string.Format(":{0}","StatisticsType"), model.StatisticsType),
				new OracleParameter(string.Format(":{0}","MerchantID"), model.MerchantID),
				new OracleParameter(string.Format(":{0}","StationID"), model.StationID),
				new OracleParameter(string.Format(":{0}","ExpressCompanyID"), model.ExpressCompanyID),
				new OracleParameter(string.Format(":{0}","StatisticsDate"), model.StatisticsDate),
				new OracleParameter(string.Format(":{0}","IsSuccess"), model.IsSuccess),
				new OracleParameter(string.Format(":{0}","Reasons"), model.Reasons),
				new OracleParameter(string.Format(":{0}","Ip"), model.Ip),
				new OracleParameter(string.Format(":{0}","CreateTime"), model.CreateTime),
				new OracleParameter(string.Format(":{0}","UpdateTime"), model.UpdateTime)	
            };

            int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败!");

            return model.LogID;
        }

        public bool UpdateIncomeStatLog(IncomeStatLog model)
        {

            StringBuilder strSql = new StringBuilder();

            strSql.Append("update FMS_IncomeStatLog set ");

            strSql.Append(" IsSuccess = :IsSuccess ,	 ");

            strSql.Append(" Reasons = :Reasons ,	 ");

            strSql.Append(" UpdateTime = :UpdateTime  ");

            strSql.Append(string.Format(" where {0} = :{0}", "LogID"));
            OracleParameter[] parameters = {
								new OracleParameter(string.Format(":{0}","LogID"), model.LogID),		
                                new OracleParameter(string.Format(":{0}","IsSuccess"), model.IsSuccess),				
                                new OracleParameter(string.Format(":{0}","Reasons"), model.Reasons),					
                                new OracleParameter(string.Format(":{0}","UpdateTime"), model.UpdateTime),		
                                        };
            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
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
            SqlStr = @"
SELECT  count(1)
FROM    FMS_IncomeAccount  fia
JOIN MerchantBaseInfo  mbi ON mbi.ID = fia.MerchantID
WHERE   fia.IsDeleted = 0 and fia.ACCOUNTSTATUS>1 and fia.IsPushed=0  {0}
";
            var sbWhere = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (MerchantID > 0)
            {
                sbWhere.Append(" AND fia.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Int32) { Value = MerchantID });
            }
            if (benginDate > DateTime.MinValue)
            {
                sbWhere.Append(" AND fia.SearchDateStr>=:SearchDateStr ");
                parameters.Add(new OracleParameter(":SearchDateStr", OracleDbType.Date) { Value = benginDate });
            }
            if (endDate > DateTime.MinValue)
            {
                sbWhere.Append(" AND fia.SearchDateEnd<:SearchDateEnd ");
                parameters.Add(new OracleParameter(":SearchDateEnd", OracleDbType.Date) { Value = endDate });
            }

            SqlStr = string.Format(SqlStr, sbWhere);
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, SqlStr,
                                                    this.ToParameters(parameters.ToArray()));
            if (obj!=null)
            {
                return Convert.ToInt32(obj) > 0;
            }
            else
            {
                return false;
            }
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
            SqlStr = @"
WITH t AS (
--普通单和上门换货单
SELECT  fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo  fibi
join     fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE    fifi.IsDeleted = 0
	     AND fibi.WaybillType IN ( '0', '1' )
		 AND fibi.BackstationTime >= :BeginTime
		 AND fibi.BackstationTime < :EndTime
		 AND fibi.MerchantID = :MerchantID
		 AND fibi.BackStationstatus=3
UNION ALL
 --拒收  
SELECT  fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo  fibi
join     fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE   fifi.IsDeleted = 0
		AND fibi.WaybillType IN ( '0', '1' )
		AND fibi.ReturnTime >= :BeginTime
		AND fibi.ReturnTime < :EndTime
		and fibi.MerchantId=:MerchantID
		And fibi.backstationstatus=5
UNION ALL 		
--退货单
SELECT   fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo fibi
join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE    fifi.IsDeleted = 0 
         AND fibi.WaybillType ='2'
         and fibi.BackstationStatus=3
         AND fibi.ReturnTime >= :BeginTime
         AND fibi.ReturnTime < :EndTime
         and fibi.MerchantId=:MerchantID
--签单返回
UNION ALL 		
--退货单
SELECT   fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo fibi
join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE    fifi.IsDeleted = 0 
         AND fibi.WaybillType ='3'
         and fibi.BackstationStatus=13
         AND fibi.ReturnTime >= :BeginTime
         AND fibi.ReturnTime <= :EndTime
         and fibi.MerchantId=:MerchantID                        
) 
SELECT 
       fibi.WaybillNo ,
       fibi.CustomerOrder,
       fibi.DeliverCode,
       s3.StatusName BillType,
       t.MerchantName ,
       mbi2.MerchantName FinancialMerchantName,
       mbi1.MerchantName SubMerchantName ,
       mbi1.MerchantCode SubMerchantCode,
       d.DistributionName ,
       ec.CompanyName ,
       fibi.RfdAcceptTime AcceptTime ,
       t.CompanyName SortCompanyName,
       fibi.BackStationTime ,
       s2.StatusName Status,
       fibi.ReturnTime ,
       s.StatusName ReturnStatus,
       fibi.AccountWeight ,
       CASE WHEN NVL(gc.GoodsCategoryName,'')='' THEN fibi.WaybillCategory ELSE gc.GoodsCategoryName END GoodsCategory,
       t.AccountFare ,
       t.AccountStandard ,
       fibi.NeedPayAmount ,
       NVL(fibi.NeedBackAmount, 0) NeedBackAmount,
       NVL(fibi.ProtectedAmount, 0) ProtectedAmount,
       s1.StatusName VoildStatus,
       fibi.AcceptType PayType,
       t.AreaType ,
       PCA.ProvinceName Province,
       PCA.CityName  City,
       PCA.AreaName Area,
       fibi.ReceiveAddress Address,
       t.ProtectedFee ,
       t.CashReceiveServiceFee ,
       t.ReceiveFee ,
       t.POSReceiveServiceFee ,
       t.POSReceiveFee 
FROM   FMS_IncomeBaseInfo fibi join (
select
		IncomeID,AccountFare,AccountStandard,MerchantName,CompanyName ,fifi.ProtectedFee,fifi.IncomeFeeID,
fifi.CashReceiveServiceFee,fifi.POSReceiveServiceFee,fifi.ReceiveFee,fifi.POSReceiveFee,
fifi.AreaType
FROM   FMS_IncomeBaseInfo fibi
       JOIN FMS_IncomeFeeInfo fifi
			ON fifi.WaybillNo = fibi.WaybillNo
       JOIN MerchantBaseInfo mbi
            ON  mbi.id = fibi.merchantid
       JOIN ExpressCompany ec2
            ON  ec2.expresscompanyid = fibi.ExpressCompanyID
       JOIN (SELECT DISTINCT * FROM t ) t ON fibi.WAYBILLNO=t.WAYBILLNO 

)t on fibi.IncomeID=t.IncomeID
       LEFT JOIN MerchantBaseInfo mbi1
            ON  mbi1.MerchantCode = fibi.OriginDepotNo and mbi1.IsSubMerchant=1
       LEFT JOIN MerchantBaseInfo mbi2
            ON  mbi2.PeriodAccountCode = fibi.PeriodAccountCode  AND NVL(fibi.PeriodAccountCode,'')<>''
       LEFT JOIN ExpressCompany ec--未分配无效时
            ON  ec.ExpressCompanyID = fibi.DeliverStationID
	   LEFT JOIN Distribution d--未分配无效时
            ON  d.DistributionCode = ec.DistributionCode
       LEFT JOIN StatusInfo s
             ON s.statusTypeNO = 5 AND CAST(s.StatusNO AS INT) = fibi.SubStatus
       LEFT JOIN StatusInfo s1
            ON s1.statusTypeNO = 308 AND CAST(s1.StatusNO AS INT) = fibi.InefficacyStatus
       LEFT JOIN StatusInfo s2
            ON s2.statusTypeNO = 1 AND s2.StatusNO = fibi.BackStationStatus
       LEFT JOIN StatusInfo s3
			ON s3.statusTypeNO = 2 AND s3.StatusNO = fibi.WaybillType
       LEFT JOIN GoodsCategory gc ON gc.GoodsCategoryCode=fibi.WaybillCategory
       LEFT JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName
                    FROM Area a  JOIN City c
                    ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID
                    WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0) PCA ON PCA.AreaID=fibi.AreaID
";
         
            var parameters = new List<OracleParameter>
                                 {
                                     new OracleParameter(":MerchantID", OracleDbType.Int32) {Value = MerchantID},
                                     new OracleParameter(":BeginTime", OracleDbType.Date) {Value = benginDate},
                                     new OracleParameter(":EndTime", OracleDbType.Date) {Value = endDate}
                                 };
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
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
        fiad.ReceiveFee ProtectedFee,
        fiad.ReceivePOSFee,
        fiad.DeliveryFee,
        fiad.DiscountFee,
        fiad.OtherFee,
        fiad.Fare,
        fiad.LOSTDEDUCTION,
        fiad.KPI DelayedDeductions 
FROM    FMS_IncomeAccount fia
        JOIN FMS_IncomeAccountDetail fiad ON fia.AccountNO = fiad.AccountNO
                                                              AND fiad.DataType = 2
        JOIN StatusCodeInfo sci ON fia.AccountStatus = sci.CodeNo
                                                     AND sci.CodeType = 'CODAccount'
        JOIN MerchantBaseInfo  mbi ON fia.MerchantID = mbi.ID
WHERE   fia.IsDeleted = 0 and fiad.IsDeleted = 0 and fia.ACCOUNTSTATUS>1 and fia.IsPushed=0 
";
            var sbWhere = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (MerchantID > 0)
            {
                sbWhere.Append(" AND fia.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Int32) { Value = MerchantID });
            }
            if (benginDate > DateTime.MinValue)
            {
                sbWhere.Append(" AND fia.SearchDateStr>=:SearchDateStr ");
                parameters.Add(new OracleParameter(":SearchDateStr", OracleDbType.Date) { Value = benginDate });
            }
            if (endDate > DateTime.MinValue)
            {
                sbWhere.Append(" AND fia.SearchDateEnd<=:SearchDateEnd ");
                parameters.Add(new OracleParameter(":SearchDateEnd", OracleDbType.Date) { Value = endDate });
            }

            SqlStr = string.Format(SqlStr, sbWhere);
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public DataTable GetAccountSearchCondition(int MerchantID, DateTime benginDate, DateTime endDate)
        {
            SqlStr = @"
SELECT  fia.ACCOUNTID,
        fia.AccountNO ,
        fia.MerchantID ,
        fia.SearchDateStr ,
        fia.SearchDateEnd ,
        mbi.MerchantName
FROM    FMS_IncomeAccount  fia
JOIN MerchantBaseInfo  mbi ON mbi.ID = fia.MerchantID
WHERE   fia.IsDeleted = 0 and fia.ACCOUNTSTATUS>1 and fia.IsPushed=0  {0}
";
            var sbWhere = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (MerchantID>0)
            {
                sbWhere.Append(" AND fia.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Int32) { Value = MerchantID });
            }
            if (benginDate>DateTime.MinValue)
            {
                sbWhere.Append(" AND fia.SearchDateStr>=:SearchDateStr ");
                parameters.Add(new OracleParameter(":SearchDateStr", OracleDbType.Date) { Value = benginDate });
            }
            if (endDate > DateTime.MinValue)
            {
                sbWhere.Append(" AND fia.SearchDateEnd<:SearchDateEnd ");
                parameters.Add(new OracleParameter(":SearchDateEnd", OracleDbType.Date) { Value = endDate });
            }

            SqlStr = string.Format(SqlStr, sbWhere);
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public bool IsEixstReceive(int merchantid, DateTime accountDate)
    	{
			const string sql = @"
                        SELECT count(1)
                        FROM    FMS_IncomeOtherFeeCount fibi
                        WHERE  1=1
                                AND fibi.ReturnTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                                AND fibi.ReturnTime <to_date(:CreatTimeEnd,'yyyy-mm-dd')
                                and fibi.MerchantId=:MerchantID
";
			OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2)
										};
			parameters[0].Value = merchantid;
			parameters[1].Value = accountDate.ToShortDateString();
			parameters[2].Value = accountDate.AddDays(1).ToShortDateString();

			var obj = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
			return obj!=null && Convert.ToInt32(obj)>0;
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
                        FROM    FMS_IncomeBaseInfo  fibi
					            join FMS_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
                        WHERE   fifi.IsDeleted = 0
                                AND fibi.WaybillType IN ( '0', '1' )
                                AND fibi.ReturnTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                                AND fibi.ReturnTime <to_date(:CreatTimeEnd,'yyyy-mm-dd')
                                and fibi.MerchantId=:MerchantID
                                And fibi.backstationstatus=5
                     )
            SELECT  :CreatTimeStr AS StatisticsDate ,
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
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2)
										};
            parameters[0].Value = merchantid;
            parameters[1].Value = accountdate.ToShortDateString();
            parameters[2].Value = accountdate.AddDays(1).ToShortDateString();

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
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
                            FROM  FMS_IncomeBaseInfo fibi 
                            join FMS_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
                            WHERE fifi.IsDeleted = 0
                            and fifi.IsAccount=1
                            AND fibi.WaybillType IN ( '0', '1' )
                            AND fibi.ReturnTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                            AND fibi.ReturnTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                            AND fibi.DeliverStationID = :DeliverStationID
                            AND fibi.ReturnExpressCompanyID = :ReturnExpressId
                            AND fibi.MerchantID = :MerchantID
                            And fibi.backstationstatus=5
                            )
                            SELECT COUNT(1) FROM t";
            OracleParameter[] parameters = { 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnExpressId",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
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
                           FROM     FMS_IncomeBaseInfo fibi
						            join FMS_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0
                                    AND fibi.WaybillType IN ( '0', '1' )
                                    AND fibi.ReturnTime >=to_date(:CreatTimeStr,'yyyy-mm-dd')
                                    AND fibi.ReturnTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                                    AND fibi.MerchantID = :MerchantID
                                    AND fibi.DeliverStationID = :DeliverStationID
                                    AND fibi.ReturnExpressCompanyID = :ReturnExpressId
                                    And fibi.backstationstatus=5
                         )
                SELECT  t.DeliverStationID,
                        t.ReturnExpressCompanyID ,
                        ael.AreaType ,
                        COUNT(1) AS FormCount ,
                        SUM(t.AccountFare) AS Fare ,
                        t.AccountStandard AS Formula ,
                        SUM(NVL(t.AccountWeight, 0)) AS WEIGHT,
                        t.WaybillType
                FROM    t
                        LEFT JOIN AreaExpressLevelIncome ael ON ael.AreaID = t.AreaID
                                                                     AND ael.IsEnable IN (1,2)
                                                                     AND ael.warehouseId = t.ReturnExpressCompanyID
                                                                     AND ael.MerchantID = t.MerchantID
                GROUP BY t.DeliverStationID ,
                        t.ReturnExpressCompanyID ,
                        ael.AreaType ,
                        t.WaybillType,
                        t.AccountStandard";
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
                                            new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
                                            new OracleParameter(":ReturnExpressId",OracleDbType.Decimal)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverStationId;
            parameters[4].Value = returnExpressId;

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 收入结算拒收增加一条数据
        /// </summary>
        public int AddIncomeReturnsCountInfo(IncomeReturnsCount model)
        {
            if (model.CountID <= 0)
            {
                model.CountID = GetIdNew("SEQ_FMS_INCOMERETURNSCOUNT");
            }

            string TableName = @"FMS_IncomeReturnsCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" CountID , ");
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
            strSql.Append(" StationID  ");
            strSql.Append(") values (");
            strSql.Append(" :CountID , ");
            strSql.Append(" :AccountNO , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :ExpressCompanyID , ");
            strSql.Append(" :AreaType , ");
            strSql.Append(" :Weight , ");
            strSql.Append(" :CountType , ");
            strSql.Append(" :CountDate , ");
            strSql.Append(" :CountNum , ");
            strSql.Append(" :Fare , ");
            strSql.Append(" :Formula , ");
            strSql.Append(" :CreateBy , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateBy , ");
            strSql.Append(" :UpdateTime , ");
            strSql.Append(" :IsDeleted , ");
            strSql.Append(" :StationID  ");
            strSql.Append(") ");
            OracleParameter[] parameters = 
            {
                new OracleParameter(string.Format(":{0}","CountID"), model.CountID),
				new OracleParameter(string.Format(":{0}","AccountNO"), model.AccountNO),
				new OracleParameter(string.Format(":{0}","MerchantID"), model.MerchantID),
				new OracleParameter(string.Format(":{0}","ExpressCompanyID"), model.ExpressCompanyID),
				new OracleParameter(string.Format(":{0}","AreaType"), model.AreaType),
				new OracleParameter(string.Format(":{0}","Weight"), model.Weight),
				new OracleParameter(string.Format(":{0}","CountType"), model.CountType),
				new OracleParameter(string.Format(":{0}","CountDate"), model.CountDate),
				new OracleParameter(string.Format(":{0}","CountNum"), model.CountNum),
				new OracleParameter(string.Format(":{0}","Fare"), model.Fare),
				new OracleParameter(string.Format(":{0}","Formula"), model.Formula),
				new OracleParameter(string.Format(":{0}","CreateBy"), model.CreateBy),
				new OracleParameter(string.Format(":{0}","CreateTime"), model.CreateTime),
				new OracleParameter(string.Format(":{0}","UpdateBy"), model.UpdateBy),
				new OracleParameter(string.Format(":{0}","UpdateTime"), model.UpdateTime),
				new OracleParameter(string.Format(":{0}","IsDeleted"), model.IsDeleted),
				new OracleParameter(string.Format(":{0}","StationID"), model.StationID)	
            };

            int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败!");

            return (int)model.CountID;
        }

        /// <summary>
        /// 收入结算上门退增加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddIncomeVisitReturnsCountInfo(IncomeVisitReturnsCount model)
        {
            if (model.CountID <= 0)
            {
                model.CountID = GetIdNew("SEQ_INCOMEVISITRETURNSCOUNT");
            }

            string TableName = @"FMS_IncomeVisitReturnsCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" CountID , ");
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
            strSql.Append(" StationID  ");
            strSql.Append(") values (");
            strSql.Append(" :CountID , ");
            strSql.Append(" :AccountNO , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :ExpressCompanyID , ");
            strSql.Append(" :AreaType , ");
            strSql.Append(" :Weight , ");
            strSql.Append(" :CountType , ");
            strSql.Append(" :CountDate , ");
            strSql.Append(" :CountNum , ");
            strSql.Append(" :Fare , ");
            strSql.Append(" :Formula , ");
            strSql.Append(" :CreateBy , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateBy , ");
            strSql.Append(" :UpdateTime , ");
            strSql.Append(" :IsDeleted , ");
            strSql.Append(" :StationID  ");
            strSql.Append(") ");

            OracleParameter[] parameters = 
            {
                new OracleParameter(string.Format(":{0}","CountID"), model.CountID),
		        new OracleParameter(string.Format(":{0}","AccountNO"), model.AccountNO),
		        new OracleParameter(string.Format(":{0}","MerchantID"), model.MerchantID),
		        new OracleParameter(string.Format(":{0}","ExpressCompanyID"), model.ExpressCompanyID),
		        new OracleParameter(string.Format(":{0}","AreaType"), model.AreaType),
		        new OracleParameter(string.Format(":{0}","Weight"), model.Weight),
		        new OracleParameter(string.Format(":{0}","CountType"), model.CountType),
		        new OracleParameter(string.Format(":{0}","CountDate"), model.CountDate),
		        new OracleParameter(string.Format(":{0}","CountNum"), model.CountNum),
		        new OracleParameter(string.Format(":{0}","Fare"), model.Fare),
		        new OracleParameter(string.Format(":{0}","Formula"), model.Formula),
		        new OracleParameter(string.Format(":{0}","CreateBy"), model.CreateBy),
		        new OracleParameter(string.Format(":{0}","CreateTime"), model.CreateTime),
		        new OracleParameter(string.Format(":{0}","UpdateBy"), model.UpdateBy),
		        new OracleParameter(string.Format(":{0}","UpdateTime"), model.UpdateTime),
		        new OracleParameter(string.Format(":{0}","IsDeleted"), model.IsDeleted),
		        new OracleParameter(string.Format(":{0}","StationID"), model.StationID)									
            };

            int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败!");

            return (int)model.CountID;
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
               FROM     FMS_IncomeBaseInfo fibi
			   join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0 
                        AND fibi.WaybillType ='2'
                        and fibi.BackstationStatus=3
                        AND fibi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fibi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        and fibi.MerchantId=:MerchantID
union   
SELECT   fibi.WaybillNO ,
                        fibi.DeliverStationID ,
                        fibi.MerchantID,
                        fibi.ReturnExpressCompanyID
               FROM     FMS_IncomeBaseInfo fibi
			   join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0 
                        AND fibi.WaybillType ='2'
						and Fibi.BackstationStatus=5
                        AND fibi.backstationtime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fibi.backstationtime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        and fibi.MerchantId=:MerchantID
             )
    SELECT  :ReturnTimeStr AS StatisticsDate ,
            t.DeliverStationID,
            t.ReturnExpressCompanyID ,
            COUNT(t.WaybillNO) AS FormCount ,
            3 AS StatisticsType
    FROM    t
    GROUP BY t.DeliverStationID ,
            t.ReturnExpressCompanyID";
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2)
										};
            parameters[0].Value = merchantid;
            parameters[1].Value = accountdate.ToShortDateString();
            parameters[2].Value = accountdate.AddDays(1).ToShortDateString();

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
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
            string sql = @"select sum(a.a1) as num from (SELECT   COUNT(1) as a1 FROM  FMS_IncomeBaseInfo fibi
        join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 and fifi.IsAccount=1 AND fibi.WaybillType ='2' and fibi.backstationstatus=3
        AND fibi.ReturnTime >= to_char(:ReturnTimeStr,'yyyy-mm-dd')
        AND fibi.ReturnTime < to_char(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fibi.DeliverStationID = :DeliverStationID
        AND fibi.ReturnExpressCompanyID = :ReturnExpressId
        AND fibi.MerchantID = :MerchantID
union
SELECT   COUNT(1) as a1 FROM  FMS_IncomeBaseInfo fibi
        join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 and fifi.IsAccount=1 AND fibi.WaybillType ='2' and fibi.backstationstatus=5
        AND fibi.backstationtime >= to_char(:ReturnTimeStr,'yyyy-mm-dd')
        AND fibi.backstationtime < to_char(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fibi.DeliverStationID = :DeliverStationID
        AND fibi.MerchantID = :MerchantID) as a";
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnExpressId",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
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
                           FROM     FMS_IncomeBaseInfo fibi
						            join FMS_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0 AND fibi.WaybillType ='2'
                                    And fibi.BackStationStatus=3
                                    AND fibi.ReturnTime >=to_date(:CreatTimeStr,'yyyy-mm-dd')
                                    AND fibi.ReturnTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                                    AND fibi.DeliverStationID = :DeliverStationID
                                    AND fibi.ReturnExpressCompanyID = :ReturnExpressId
                                    AND fibi.MerchantID = :MerchantID
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
                           FROM     FMS_IncomeBaseInfo fibi
						            join FMS_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
                           WHERE    fifi.IsDeleted = 0 AND fibi.WaybillType ='2'
                                    And fibi.BackStationStatus=5
                                    AND fibi.backstationtime >=to_date(:CreatTimeStr,'yyyy-mm-dd')
                                    AND fibi.backstationtime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                                    AND fibi.MerchantID = :MerchantID
                                    AND fibi.DeliverStationID = :DeliverStationID
                         )
                SELECT  t.DeliverStationID,
                        t.ReturnExpressCompanyID ,
                        ael.AreaType ,
                        COUNT(1) AS FormCount ,
                        SUM(t.AccountFare) AS Fare ,
                        t.AccountStandard AS Formula ,
                        SUM(NVL(t.AccountWeight, 0)) AS WEIGHT,
                        case when t.BackStationStatus='3' then '0' else '1' end as CountType
                FROM    t
                        LEFT JOIN AreaExpressLevelIncome ael ON ael.AreaID = t.AreaID
                                                                     AND ael.IsEnable IN (1,2)
                                                                     AND ael.warehouseId = t.ReturnExpressCompanyID
                                                                     AND ael.MerchantID = t.MerchantID
                GROUP BY t.DeliverStationID ,
                        t.ReturnExpressCompanyID,
                        ael.AreaType ,
                        t.BackStationStatus,
                        t.AccountStandard";
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
                                            new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
                                            new OracleParameter(":ReturnExpressId",OracleDbType.Decimal)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverStationId;
            parameters[4].Value = returnExpressId;

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
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
            FROM  FMS_IncomeBaseInfo fibi
            join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0 
            AND fibi.backstationtime >=to_date(:BackStationTimeStr,'yyyy-mm-dd')--归班时间
            AND fibi.backstationtime < to_date(:BackStationTimeEnd,'yyyy-mm-dd')--归班时间
            AND fibi.MerchantID = :MerchantID
            AND fibi.backStationStatus in (3,5)
            ) 
            select
            t.deliverstationid,
            COUNT(1) AS FormCount--订单数量
             from t
             GROUP BY t.deliverstationid";
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":BackStationTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":BackStationTimeEnd",OracleDbType.Varchar2)
										};
            parameters[0].Value = merchantid;
            parameters[1].Value = accountdate.ToShortDateString();
            parameters[2].Value = accountdate.AddDays(1).ToShortDateString();

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        public int CollateIncomeOtherReceiveFee(int merchantid, DateTime accountDate, int deliverstationid)
        {
            string sql = @" with t as(
            SELECT fibi.waybillno,
            fibi.merchantid,
            fibi.deliverstationid
            FROM  FMS_IncomeBaseInfo fibi
            join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0 and fifi.IsReceive=1
            AND fibi.backstationtime >=to_date(:BackStationTimeStr,'yyyy-mm-dd') --归班时间
            AND fibi.backstationtime < to_date(:BackStationTimeEnd,'yyyy-mm-dd')--归班时间
            AND fibi.MerchantID = :MerchantID
            And fibi.deliverstationid=:DeliverStationID
            AND fibi.backStationStatus in (3,5)
            ) 
            select count(1) from t
             ";
            OracleParameter[] parameters = { 
											new OracleParameter(":BackStationTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":BackStationTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
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

            FROM  FMS_IncomeBaseInfo fibi
            join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0 
            AND fibi.backstationtime >= to_date(:BackStationTimeStr,'yyyy-mm-dd')--归班时间
            AND fibi.backstationtime < to_date(:BackStationTimeEnd,'yyyy-mm-dd')--归班时间
            AND fibi.MerchantID = :MerchantID
            AND fibi.backStationStatus in (3,5)
            AND fibi.deliverStationId=:deliverStationId
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
            OracleParameter[] parameters = { 
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal), 
											new OracleParameter(":BackStationTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":BackStationTimeEnd",OracleDbType.Varchar2),
                                            new OracleParameter(":deliverStationId",OracleDbType.Decimal)
										};
            parameters[0].Value = merchantId;
            parameters[1].Value = accountDate.ToShortDateString();
            parameters[2].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[3].Value = deliverStationId;

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        public int AddIncomeOtherFeeCount(IncomeOtherFeeCount model)
        {
            if (model.CountID <= 0)
            {
                model.CountID = GetIdNew("SEQ_FMS_INCOMEOTHERFEECOUNT");
            }

            string TableName = @"FMS_IncomeOtherFeeCount";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" CountID , ");
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
            strSql.Append(" POSServesFee  ");
            strSql.Append(") values (");
            strSql.Append(" :CountID , ");
            strSql.Append(" :AccountNO , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :ExpressCompanyID , ");
            strSql.Append(" :AreaType , ");
            strSql.Append(" :CountType , ");
            strSql.Append(" :CountDate , ");
            strSql.Append(" :ProtectedStandard , ");
            strSql.Append(" :ProtectedFee , ");
            strSql.Append(" :ReceiveStandard , ");
            strSql.Append(" :ReceiveFee , ");
            strSql.Append(" :ReceivePOSStandard , ");
            strSql.Append(" :ReceivePOSFee , ");
            strSql.Append(" :CreateBy , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateBy , ");
            strSql.Append(" :UpdateTime , ");
            strSql.Append(" :IsDeleted , ");
            strSql.Append(" :StationID , ");
            strSql.Append(" :ServesStandard , ");
            strSql.Append(" :ServesFee , ");
            strSql.Append(" :POSServesStandard , ");
            strSql.Append(" :POSServesFee  ");
            strSql.Append(") ");

            OracleParameter[] parameters = 
            {
                new OracleParameter(string.Format(":{0}","CountID"), model.CountID),
		        new OracleParameter(string.Format(":{0}","AccountNO"), model.AccountNO),
		        new OracleParameter(string.Format(":{0}","MerchantID"), model.MerchantID),
		        new OracleParameter(string.Format(":{0}","ExpressCompanyID"), model.ExpressCompanyID),
		        new OracleParameter(string.Format(":{0}","AreaType"), model.AreaType),
		        new OracleParameter(string.Format(":{0}","CountType"), model.CountType),
		        new OracleParameter(string.Format(":{0}","CountDate"), model.CountDate),
		        new OracleParameter(string.Format(":{0}","ProtectedStandard"), model.ProtectedStandard),
		        new OracleParameter(string.Format(":{0}","ProtectedFee"), model.ProtectedFee),
		        new OracleParameter(string.Format(":{0}","ReceiveStandard"), model.ReceiveStandard),
		        new OracleParameter(string.Format(":{0}","ReceiveFee"), model.ReceiveFee),
		        new OracleParameter(string.Format(":{0}","ReceivePOSStandard"), model.ReceivePOSStandard),
		        new OracleParameter(string.Format(":{0}","ReceivePOSFee"), model.ReceivePOSFee),
		        new OracleParameter(string.Format(":{0}","CreateBy"), model.CreateBy),
		        new OracleParameter(string.Format(":{0}","CreateTime"), model.CreateTime),
		        new OracleParameter(string.Format(":{0}","UpdateBy"), model.UpdateBy),
		        new OracleParameter(string.Format(":{0}","UpdateTime"), model.UpdateTime),
		        new OracleParameter(string.Format(":{0}","IsDeleted"), model.IsDeleted),
		        new OracleParameter(string.Format(":{0}","StationID"), model.StationID),
		        new OracleParameter(string.Format(":{0}","ServesStandard"), model.ServesStandard),
		        new OracleParameter(string.Format(":{0}","ServesFee"), model.ServesFee),
		        new OracleParameter(string.Format(":{0}","POSServesStandard"), model.POSServesStandard),
		        new OracleParameter(string.Format(":{0}","POSServesFee"), model.POSServesFee)									
            };

            int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败!");

            return (int)model.CountID;
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
            UpdateTime From fms_incomestatlog fisl where fisl.issuccess=0 and StatisticsDate<=:acountDate";
            OracleParameter[] parameters = { 
											new OracleParameter(":acountDate",OracleDbType.Varchar2)
										};

            parameters[0].Value = countdate.AddDays(-2).ToShortDateString();
            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
            return dt;
        }

        public int IncomeDeliveryAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            string sql = @"WITH    t AS ( SELECT   fibi.WaybillNO
               FROM     FMS_IncomeBaseInfo  fibi
				join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						--AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fibi.BackstationTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fibi.DeliverStationID = :DeliverStationID
                        AND fibi.ExpressCompanyID = :Warehouseid
                        AND fibi.MerchantID = :MerchantID
                        AND NVL(fibi.FinalExpressCompanyID, 0) = 0
                        AND fibi.BackStationStatus=3
               UNION ALL
               SELECT   fibi.WaybillNO
               FROM     FMS_IncomeBaseInfo AS fibi
				join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
               WHERE    fifi.IsDeleted = 0
						--AND fifi.IsAccount=1
                        AND fibi.WaybillType IN ( '0', '1' )
                        AND fibi.BackstationTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fibi.BackstationTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fibi.DeliverStationID = :DeliverStationID
                        AND fibi.FinalExpressCompanyID = :Warehouseid
                        AND fibi.MerchantID = :MerchantID
                        AND NVL(fibi.FinalExpressCompanyID, 0) > 0 
                        AND fibi.BackStationStatus=3            
             )
    SELECT  COUNT(1)
    FROM    t";
            OracleParameter[] parameters = { 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":Warehouseid",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = warehouseid;
            parameters[4].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));

        }

        public int IncomeReturnsAccountHistory(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid)
        {
            string sql = @"with t as(
                            SELECT fibi.WaybillNO
                            FROM  FMS_IncomeBaseInfo fibi 
                            join FMS_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
                            WHERE fifi.IsDeleted = 0
                            --and fifi.IsAccount=1
                            AND fibi.WaybillType IN ( '0', '1' )
                            AND fibi.ReturnTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                            AND fibi.ReturnTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                            AND fibi.DeliverStationID = :DeliverStationID
                            AND fibi.ReturnExpressCompanyID = :ReturnExpressId
                            AND fibi.MerchantID = :MerchantID
                            AND fibi.BackStationStatus=5
                            )
                            SELECT COUNT(1) FROM t";
            OracleParameter[] parameters = { 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnExpressId",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        public int IncomeVisitReturnsAccountHistory(int merchantid, DateTime accountDate, int returnexpresscompanyId, int deliverstationid)
        {
            string sql = @"select sum(a.a1) as num from (SELECT   COUNT(1) as a1 FROM  FMS_IncomeBaseInfo fibi
        join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 
        --and fifi.IsAccount=1 
        AND fibi.WaybillType ='2' 
        and fibi.backstationstatus=3
        AND fibi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fibi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fibi.DeliverStationID = :DeliverStationID
        AND fibi.ReturnExpressCompanyID = :ReturnExpressId
        AND fibi.MerchantID = :MerchantID
union
SELECT   COUNT(1) as a1 FROM  FMS_IncomeBaseInfo fibi
        join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
        WHERE  fifi.IsDeleted = 0 
        --and fifi.IsAccount=1 
        AND fibi.WaybillType ='2' 
        and fibi.backstationstatus=5
        AND fibi.backstationtime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fibi.backstationtime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fibi.DeliverStationID = :DeliverStationID
        AND fibi.MerchantID = :MerchantID) as a ";
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnExpressId",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = returnexpresscompanyId;
            parameters[4].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        public int IncomeOtherReceiveFeeHistory(int merchantid, DateTime accountDate, int deliverstationid)
        {
            string sql = @" with t as(
            SELECT fibi.waybillno,
            fibi.merchantid,
            fibi.deliverstationid
            FROM  FMS_IncomeBaseInfo fibi
            join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
            WHERE  fifi.IsDeleted = 0
            --and fifi.IsReceive=1
            AND fibi.backstationtime >=to_date(:BackStationTimeStr,'yyyy-mm-dd') --归班时间
            AND fibi.backstationtime < to_date(:BackStationTimeEnd,'yyyy-mm-dd')--归班时间
            AND fibi.MerchantID = :MerchantID
            And fibi.deliverstationid=:DeliverStationID
            AND fibi.BackStationStatus in (3,5)
            ) 
            select count(1) from t
             ";
            OracleParameter[] parameters = { 
											new OracleParameter(":BackStationTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":BackStationTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();
            parameters[2].Value = deliverstationid;
            parameters[3].Value = merchantid;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        public int IsIncomeHistoryCount(DateTime countdate)
        {
            string sql = @"
            select count(1) From fms_incomestatlog fisl where StatisticsDate>=to_date(:acountDate,'yyyy-mm-dd') and StatisticsDate<to_date(:acountEndDate,'yyyy-mm-dd')";
            OracleParameter[] parameters = { 
											new OracleParameter(":acountDate",OracleDbType.Varchar2),
                                            new OracleParameter(":acountEndDate",OracleDbType.Varchar2)
										};

            parameters[0].Value = countdate.ToShortDateString();
            parameters[1].Value = countdate.AddDays(1).ToShortDateString();

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        #endregion

        public DataTable GetNormalIncomeAccountDetail(int MerchantID, DateTime bengindate, DateTime enddate, List<int> BillType)
        {
            SqlStr = @"
WITH t AS (
--普通单和上门换货单
SELECT  fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo  fibi
join     fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE    fifi.IsDeleted = 0
	     AND fibi.WaybillType IN ( '0', '1' )
		 AND fibi.BackstationTime >= :BeginTime
		 AND fibi.BackstationTime < :EndTime
		 AND fibi.MerchantID = :MerchantID
		 AND fibi.BackStationstatus=3
UNION ALL
 --拒收  
SELECT  fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo  fibi
join     fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE   fifi.IsDeleted = 0
		AND fibi.WaybillType IN ( '0', '1' )
		AND fibi.ReturnTime >= :BeginTime
		AND fibi.ReturnTime < :EndTime
		and fibi.MerchantId=:MerchantID
		And fibi.backstationstatus=5
UNION ALL 		
--退货单
SELECT   fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo fibi
join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE    fifi.IsDeleted = 0 
         AND fibi.WaybillType ='2'
         and fibi.BackstationStatus=3
         AND fibi.ReturnTime >= :BeginTime
         AND fibi.ReturnTime < :EndTime
         and fibi.MerchantId=:MerchantID
--签单返回
UNION ALL 		
--退货单
SELECT   fibi.WaybillNO 
FROM     FMS_IncomeBaseInfo fibi
join fms_IncomeFeeInfo fifi  on fibi.waybillno=fifi.waybillno
WHERE    fifi.IsDeleted = 0 
         AND fibi.WaybillType ='3'
         and fibi.BackstationStatus=13
         AND fibi.ReturnTime >= :BeginTime
         AND fibi.ReturnTime < :EndTime
         and fibi.MerchantId=:MerchantID                        
) 
SELECT 
       fibi.WaybillNo ,
       fibi.CustomerOrder,
       fibi.DeliverCode,
       s3.StatusName BillType,
       t.MerchantName ,
       mbi2.MerchantName FinancialMerchantName,
       mbi1.MerchantName SubMerchantName ,
       mbi1.MerchantCode SubMerchantCode,
       d.DistributionName ,
       ec.CompanyName ,
       fibi.RfdAcceptTime AcceptTime ,
       t.CompanyName SortCompanyName,
       fibi.BackStationTime ,
       s2.StatusName Status,
       fibi.ReturnTime ,
       s.StatusName ReturnStatus,
       fibi.AccountWeight ,
       CASE WHEN NVL(gc.GoodsCategoryName,'')='' THEN fibi.WaybillCategory ELSE gc.GoodsCategoryName END GoodsCategory,
       t.AccountFare ,
       t.AccountStandard ,
       fibi.NeedPayAmount ,
       NVL(fibi.NeedBackAmount, 0) NeedBackAmount,
       NVL(fibi.ProtectedAmount, 0) ProtectedAmount,
       s1.StatusName VoildStatus,
       fibi.AcceptType PayType,
       t.AreaType ,
       PCA.ProvinceName Province,
       PCA.CityName  City,
       PCA.AreaName Area,
       fibi.ReceiveAddress Address,
       t.ProtectedFee ,
       t.CashReceiveServiceFee ,
       t.ReceiveFee ,
       t.POSReceiveServiceFee ,
       t.POSReceiveFee 
FROM   FMS_IncomeBaseInfo fibi join (
select
		IncomeID,AccountFare,AccountStandard,MerchantName,CompanyName ,fifi.ProtectedFee,fifi.IncomeFeeID,
fifi.CashReceiveServiceFee,fifi.POSReceiveServiceFee,fifi.ReceiveFee,fifi.POSReceiveFee,
fifi.AreaType
FROM   FMS_IncomeBaseInfo fibi
       JOIN FMS_IncomeFeeInfo fifi
			ON fifi.WaybillNo = fibi.WaybillNo
       JOIN MerchantBaseInfo mbi
            ON  mbi.id = fibi.merchantid
       JOIN ExpressCompany ec2
            ON  ec2.expresscompanyid = fibi.ExpressCompanyID
       JOIN (SELECT DISTINCT * FROM t ) t ON fibi.WAYBILLNO=t.WAYBILLNO 

)t on fibi.IncomeID=t.IncomeID
       LEFT JOIN MerchantBaseInfo mbi1
            ON  mbi1.MerchantCode = fibi.OriginDepotNo and mbi1.IsSubMerchant=1
       LEFT JOIN MerchantBaseInfo mbi2
            ON  mbi2.PeriodAccountCode = fibi.PeriodAccountCode  AND NVL(fibi.PeriodAccountCode,'')<>''
       LEFT JOIN ExpressCompany ec--未分配无效时
            ON  ec.ExpressCompanyID = fibi.DeliverStationID
	   LEFT JOIN Distribution d--未分配无效时
            ON  d.DistributionCode = fibi.DistributionCode
       LEFT JOIN StatusInfo s
             ON s.statusTypeNO = 5 AND CAST(s.StatusNO AS INT) = fibi.SubStatus
       LEFT JOIN StatusInfo s1
            ON s1.statusTypeNO = 308 AND CAST(s1.StatusNO AS INT) = fibi.InefficacyStatus
       LEFT JOIN StatusInfo s2
            ON s2.statusTypeNO = 1 AND s2.StatusNO = fibi.BackStationStatus
       LEFT JOIN StatusInfo s3
			ON s3.statusTypeNO = 2 AND s3.StatusNO = fibi.WaybillType
       LEFT JOIN GoodsCategory gc ON gc.GoodsCategoryCode=fibi.WaybillCategory
       LEFT JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName
                    FROM Area a  JOIN City c
                    ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID
                    WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0) PCA ON PCA.AreaID=fibi.AreaID
where 1=1 ";

  
            var parameters = new List<OracleParameter>
                                 {
                                     new OracleParameter(":MerchantID", OracleDbType.Int32) {Value = MerchantID},
                                     new OracleParameter(":BeginTime", OracleDbType.Date) {Value = bengindate},
                                     new OracleParameter(":EndTime", OracleDbType.Date) {Value = enddate}
                                 };
            string billtypestr = string.Empty;
            if (BillType!=null&&BillType.Count>0)
            {
                int n = 1;
                foreach (var i in BillType)
                {
                    billtypestr += "  fibi.WaybillType=:WaybillType" + n + " ";
                    if (n<BillType.Count)
                    {
                        billtypestr += " or ";
                    }
                    parameters.Add(new OracleParameter(":WaybillType" + n.ToString(CultureInfo.InvariantCulture),OracleDbType.Varchar2,20){Value = i});
                    n++;
                }
            }
            if (!string.IsNullOrEmpty(billtypestr))
            {
                SqlStr += " and (" + billtypestr + ")";
            }
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }
    }
}
