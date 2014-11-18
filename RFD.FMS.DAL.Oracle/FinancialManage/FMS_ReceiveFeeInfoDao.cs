using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;
using Oracle.DataAccess.Client;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using System.Globalization;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class FMS_ReceiveFeeInfoDao : OracleDao, IFMS_ReceiveFeeInfoDao
    {
        public bool ExistsExpressReceiveFeeInfo(long waybillNo)
        {
            string sql = "select count(1) from FMS_ExpressReceiveFeeInfo where WaybillNO=:WaybillNO and IsDeleted=0";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":WaybillNO",waybillNo)
            };

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql,parameters)) > 0;
        }

        public bool AddExpressReceiveFeeInfo(Model.FinancialManage.FMS_IncomeExpressReceiveFeeInfo model)
        {
            string sql = @"insert into FMS_EXPRESSRECEIVEFEEINFO 
                        (
                            FeeID,
                            WaybillNo,
                            MerchantID,
                            DeliverManID,
                            STATUS,
                            WaybillCreatTime,
                            ReceiveStationID,
                            DeliverStationID,
                            DistributionCode,
                            TransferFee,
                            DinsureFee,
                            TransferPayType,
                            AcceptType,
                            CreateTime,
                            UpdateTime,
                            IsDeleted,
                            BackStationCreateTime,
                            CustomerOrder,
                            POSCode 
                        )
                        Values
                        (
                            :FeeID,
                            :WaybillNo,
                            :MerchantID,
                            :DeliverManID,
                            :STATUS,
                            :WaybillCreatTime,
                            :ReceiveStationID,
                            :DeliverStationID,
                            :DistributionCode,
                            :TransferFee,
                            :DinsureFee,
                            :TransferPayType,
                            :AcceptType,
                            :CreateTime,
                            :UpdateTime,
                            :IsDeleted,
                            :BackStationCreateTime,
                            :CustomerOrder,
                            :POSCode  
                        )";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":FeeID",GetIdNew("SEQ_FMS_EXPRESSRECEIVEFEEINFO")),
                new OracleParameter(":WaybillNo",model.WaybillNO),
                new OracleParameter(":MerchantID",model.MerchantID),
                new OracleParameter(":DeliverManID",model.DeliverManID),
                new OracleParameter(":STATUS",model.Status),
                new OracleParameter(":WaybillCreatTime",model.WaybillCreatTime),
                new OracleParameter(":ReceiveStationID",model.ReceiveStationID),
                new OracleParameter(":DeliverStationID",model.DeliverStationID),
                new OracleParameter(":DistributionCode",model.DistributionCode),
                new OracleParameter(":TransferFee",model.TransferFee),
                new OracleParameter(":DinsureFee",model.DinsureFee),
                new OracleParameter(":TransferPayType",model.TransferPayType),
                new OracleParameter(":AcceptType",model.AcceptType),
                new OracleParameter(":CreateTime",DateTime.Now),
                new OracleParameter(":UpdateTime",DateTime.Now),
                new OracleParameter(":IsDeleted",model.IsDeleted==true ? 1 : 0),
                new OracleParameter(":BackStationCreateTime",model.BackStationCreateTime),
                new OracleParameter(":CustomerOrder",model.CustomerOrder),
                new OracleParameter(":POSCode",model.POSCode)
            };

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) == 1;
        }

        public bool UpdateExpressReceiveFeeInfo(Model.FinancialManage.FMS_IncomeExpressReceiveFeeInfo model)
        {
            string sql = @"update FMS_EXPRESSRECEIVEFEEINFO set 
                    MerchantID = :MerchantID,
                    DeliverManID = :DeliverManID,
                    STATUS = :STATUS,
                    WaybillCreatTime = :WaybillCreatTime,
                    ReceiveStationID = :ReceiveStationID,
                    DeliverStationID = :DeliverStationID,
                    DistributionCode = :DistributionCode,
                    TransferFee = :TransferFee,
                    DinsureFee = :DinsureFee,
                    TransferPayType = :TransferPayType,
                    AcceptType = :AcceptType,
                    UpdateTime = :UpdateTime,
                    IsDeleted = :IsDeleted,
                    BackStationCreateTime=:BackStationCreateTime,
                    CustomerOrder=:CustomerOrder,
                    POSCode=:POSCode  
                where WaybillNO=:WaybillNO";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":MerchantID",model.MerchantID),
                new OracleParameter(":DeliverManID",model.DeliverManID),
                new OracleParameter(":STATUS",model.Status),
                new OracleParameter(":WaybillCreatTime",model.WaybillCreatTime),
                new OracleParameter(":ReceiveStationID",model.ReceiveStationID),
                new OracleParameter(":DeliverStationID",model.DeliverStationID),
                new OracleParameter(":DistributionCode",model.DistributionCode),
                new OracleParameter(":TransferFee",model.TransferFee),
                new OracleParameter(":DinsureFee",model.DinsureFee),
                new OracleParameter(":TransferPayType",model.TransferPayType),
                new OracleParameter(":AcceptType",model.AcceptType),
                new OracleParameter(":UpdateTime",DateTime.Now),
                new OracleParameter(":IsDeleted",model.IsDeleted == true ? 1 : 0),
                new OracleParameter(":BackStationCreateTime",model.BackStationCreateTime),
                new OracleParameter(":CustomerOrder",model.CustomerOrder),
                new OracleParameter(":POSCode",model.POSCode),
                new OracleParameter(":WaybillNO",model.WaybillNO)
            };

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) == 1;
        }

        public bool DeleteExpressFeeInfo(long waybillNo)
        {
            string sql = "update FMS_EXPRESSRECEIVEFEEINFO set IsDeleted=1 where WaybillNO=:WaybillNO";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":WaybillNO",waybillNo)
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);

            return true;
        }

        public bool ExistsProjectReceiveFeeInfo(long waybillNo)
        {
            try
            {
                string sql = "select count(1) from FMS_RECEIVEFEEINFO where WaybillNO=:WaybillNO";

                OracleParameter[] parameters = 
                {
                    new OracleParameter(":WaybillNO",waybillNo)
                };

                return DataConvert.ToInt(OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql, parameters)) > 0;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool InsertProjectReceiveFeeInfo(Model.FinancialManage.FMS_IncomeReceiveFeeInfo model)
        {
            string sql = @"insert into FMS_RECEIVEFEEINFO 
                    (
                        FeeID,
                        WaybillNo,
                        WaybillType,
                        DeliverManID,
                        SOURCES,
                        ComeFrom,
                        MerchantID,
                        CustomerOrder,
                        WaybillCreateTime,
                        DeliverStationID,
                        AcceptType,
                        BackStationCreateTime,
                        SignStatus,
                        PosCode,
                        FactBackAmount,
                        FactAmount,
                        FinancialStatus,
                        SignInfoCreateTime,
                        CreateTime,
                        UpdateTime,
                        IsDeleted,
                        DistributionCode,
                        NeedAmount,
                        NeedBackAmount,
                        AccountType,
                        TransferPayType  
                    )
                    Values
                    (
                        :FeeID,
                        :WaybillNo,
                        :WaybillType,
                        :DeliverManID,
                        :SOURCES,
                        :ComeFrom,
                        :MerchantID,
                        :CustomerOrder,
                        :WaybillCreateTime,
                        :DeliverStationID,
                        :AcceptType,
                        :BackStationCreateTime,
                        :SignStatus,
                        :PosCode,
                        :FactBackAmount,
                        :FactAmount,
                        :FinancialStatus,
                        :SignInfoCreateTime,
                        :CreateTime,
                        :UpdateTime,
                        :IsDeleted,
                        :DistributionCode,
                        :NeedAmount,
                        :NeedBackAmount,
                        :AccountType,
                        :TransferPayType
                    )";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":FeeID",GetIdNew("SEQ_FMS_RECEIVEFEEINFO")),
                new OracleParameter(":WaybillNo",model.WaybillNO),
                new OracleParameter(":WaybillType",model.WaybillType),
                new OracleParameter(":DeliverManID",model.DeliverManID),
                new OracleParameter(":SOURCES",model.Sources),
                new OracleParameter(":ComeFrom",model.ComeFrom),
                new OracleParameter(":MerchantID",model.MerchantID),
                new OracleParameter(":CustomerOrder",model.CustomerOrder),
                new OracleParameter(":WaybillCreateTime",model.WaybillCreateTime),
                new OracleParameter(":DeliverStationID",model.DeliverStationID),
                new OracleParameter(":AcceptType",model.AcceptType),
                new OracleParameter(":BackStationCreateTime",model.BackStationCreateTime),
                new OracleParameter(":SignStatus",model.SignStatus),
                new OracleParameter(":PosCode",model.POSCode),
                new OracleParameter(":FactBackAmount",model.FactBackAmount),
                new OracleParameter(":FactAmount",model.FactAmount),
                new OracleParameter(":FinancialStatus",model.FinancialStatus),
                new OracleParameter(":SignInfoCreateTime",model.SignInfoCreateTime),
                new OracleParameter(":CreateTime",DateTime.Now),
                new OracleParameter(":UpdateTime",DateTime.Now),
                new OracleParameter(":IsDeleted",model.IsDeleted == true ? 1 : 0),
                new OracleParameter(":DistributionCode",model.DiscributionCode),
                new OracleParameter(":NeedAmount",model.NeedAmount),
                new OracleParameter(":NeedBackAmount",model.NeedBackAmount),
                new OracleParameter(":AccountType",model.AccountType), 
                new OracleParameter(":TransferPayType",model.TransferPayType) 
            };

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) == 1;
        }

        public bool UpdateProjectReceiveFeeInfo(Model.FinancialManage.FMS_IncomeReceiveFeeInfo model)
        {
            string sql = @"update FMS_RECEIVEFEEINFO set
                    WaybillType=:WaybillType,
                    DeliverManID=:DeliverManID,
                    SOURCES=:SOURCES,
                    ComeFrom=:ComeFrom,
                    MerchantID=:MerchantID,
                    CustomerOrder=:CustomerOrder,
                    WaybillCreateTime=:WaybillCreateTime,
                    DeliverStationID=:DeliverStationID,
                    AcceptType=:AcceptType,
                    BackStationCreateTime=:BackStationCreateTime,
                    SignStatus=:SignStatus,
                    PosCode=:PosCode,
                    FactBackAmount=:FactBackAmount,
                    FactAmount=:FactAmount,
                    FinancialStatus=:FinancialStatus,
                    SignInfoCreateTime=:SignInfoCreateTime,
                    UpdateTime=:UpdateTime,
                    IsDeleted=:IsDeleted,
                    DistributionCode=:DistributionCode,
                    NeedAmount=:NeedAmount,
                    NeedBackAmount=:NeedBackAmount,
                    TransferPayType=:TransferPayType,
                    AccountType=:AccountType
                where WaybillNO=:WaybillNO";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":WaybillType",model.WaybillType),
                new OracleParameter(":DeliverManID",model.DeliverManID),
                new OracleParameter(":SOURCES",model.Sources),
                new OracleParameter(":ComeFrom",model.ComeFrom),
                new OracleParameter(":MerchantID",model.MerchantID),
                new OracleParameter(":CustomerOrder",model.CustomerOrder),
                new OracleParameter(":WaybillCreateTime",model.WaybillCreateTime),
                new OracleParameter(":DeliverStationID",model.DeliverStationID),
                new OracleParameter(":AcceptType",model.AcceptType),
                new OracleParameter(":BackStationCreateTime",model.BackStationCreateTime),
                new OracleParameter(":SignStatus",model.SignStatus),
                new OracleParameter(":PosCode",model.POSCode),
                new OracleParameter(":FactBackAmount",model.FactBackAmount),
                new OracleParameter(":FactAmount",model.FactAmount),
                new OracleParameter(":FinancialStatus",model.FinancialStatus),
                new OracleParameter(":SignInfoCreateTime",model.SignInfoCreateTime),
                new OracleParameter(":UpdateTime",DateTime.Now),
                new OracleParameter(":IsDeleted",model.IsDeleted == true ? 1 : 0),
                new OracleParameter(":DistributionCode",model.DiscributionCode),
                new OracleParameter(":NeedAmount",model.NeedAmount),
                new OracleParameter(":NeedBackAmount",model.NeedBackAmount),
                new OracleParameter(":WaybillNO",model.WaybillNO),
                new OracleParameter(":AccountType",model.AccountType), 
                new OracleParameter(":TransferPayType",model.TransferPayType)
            };

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) == 1;
        }

        public bool DeleteProjectReceiveFeeInfo(long waybillNo)
        {
            string sql = "update FMS_RECEIVEFEEINFO set IsDeleted=1 where WaybillNO=:WaybillNO";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":WaybillNO",waybillNo)
            };

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);

            return true;
        }

        public DataTable GetTransferFinanceSumDataV2(SearchCondition condition)
        {
            string strSql1 = @"SELECT  MAX(ec.DistributionCode) DistributionCode,
                        MAX(dis.DistributionName) DistributionName,
                        MAX(ec.ExpressCompanyID) ExpressCompanyID,
                        MAX(ec.CompanyName) CompanyName, 
                        MAX(expressInfo.Sources) Sources,
                        MAX(si.StatusName) SourceName,
                        MAX(expressInfo.MerchantID) MerchantID,
                        MAX(mbi.MerchantCode) MerchantCode,
                        MAX(mbi.MerchantName) MerchantName,
                        SUM(CASE WHEN expressInfo.AcceptType <> 'POS机' THEN 1 ELSE 0 END) CashWaybillCount, 
                        SUM(CASE WHEN expressInfo.AcceptType = 'POS机' THEN 1 ELSE 0 END) POSWaybillCount, 
                        SUM(NVL(expressInfo.TransferFee,0)) TransferFeeSum,   
                        SUM(NVL(expressInfo.DinsureFee,0)) ProtectedPriceSum,
                        (SUM(NVL(expressInfo.TransferFee,0))+ SUM(NVL(expressInfo.DinsureFee,0))) SaveAmount,  
                        TO_Char(expressInfo.BackStationCreateTime,'YYYY-MM-DD') CreateTime
                FROM Fms_ExpressReceivefeeinfo expressInfo  
                   JOIN ExpressCompany ec ON expressInfo.DeliverStationID = ec.ExpressCompanyID
                   LEFT JOIN Distribution dis ON ec.DistributionCode = dis.DistributionCode
                   LEFT JOIN MerchantBaseInfo mbi ON expressInfo.MerchantID = mbi.ID
                   LEFT JOIN StatusInfo si ON expressInfo.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                   LEFT  JOIN Employee e ON expressInfo.DeliverManID=e.EmployeeID
                WHERE  expressInfo.Status='3'
                     AND expressInfo.DistributionCode =:DistributionCode	
                  -- AND expressInfo.MerchantID=30 
                   AND expressInfo.IsDeleted=0 
                   AND expressInfo.TransferPayType=2";

            string strSql2 = @"SELECT  MAX(ec.DistributionCode) DistributionCode, 
                        MAX(dis.DistributionName) DistributionName,
                        MAX(ec.ExpressCompanyID) ExpressCompanyID,
                        MAX(ec.CompanyName) CompanyName, 
                        MAX(expressInfo.Sources) Sources,
                        MAX(si.StatusName) SourceName,
                        MAX(expressInfo.MerchantID) MerchantID,
                        MAX(mbi.MerchantCode) MerchantCode,
                        MAX(mbi.MerchantName) MerchantName,
                        SUM(CASE WHEN expressInfo.AcceptType <> 'POS机' THEN 1 ELSE 0 END) CashWaybillCount, 
                        SUM(CASE WHEN expressInfo.AcceptType = 'POS机' THEN 1 ELSE 0 END) POSWaybillCount, 
                        SUM(NVL(expressInfo.TransferFee,0)) TransferFeeSum,  
                        SUM(NVL(expressInfo.DinsureFee,0)) ProtectedPriceSum,
                        (SUM(NVL(expressInfo.TransferFee,0))+ SUM(NVL(expressInfo.DinsureFee,0))) as SaveAmount, 
                        TO_Char(expressInfo.WaybillCreatTime,'YYYY-MM-DD') CreateTime
                FROM Fms_ExpressReceivefeeinfo expressInfo               
                     JOIN ExpressCompany ec ON expressInfo.ReceiveStationID = ec.ExpressCompanyID
                     LEFT JOIN MerchantBaseInfo mbi ON expressInfo.MerchantID = mbi.ID
                     LEFT JOIN StatusInfo si ON expressInfo.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                     LEFT JOIN Distribution dis ON ec.DistributionCode = dis.DistributionCode
                     LEFT JOIN Employee e ON expressInfo.DeliverManID=e.EmployeeID
                WHERE 1=1  --expressInfo.MerchantID=30
                    AND expressInfo.DistributionCode =:DistributionCode
                    AND expressInfo.IsDeleted=0
                    --AND expressInfo.Status<>'-5' 
                    AND expressInfo.DeliverStationID<>'-1'
                    AND (expressInfo.TransferPayType=1) ";

            List<OracleParameter> parameters = new List<OracleParameter>();

            string where = "";
            string where1 = "";

            if (condition.DeliverStation != 0)
            {
                where += " and expressInfo.DeliverStationID=:DeliverStation";
                where1 += " and expressInfo.ReceiveStationID=:DeliverStation ";
                parameters.Add(new OracleParameter(":DeliverStation", condition.DeliverStation));
            }

            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                where += " and expressInfo.DistributionCode=:DistributionCode";
                where1 += " and expressInfo.DistributionCode=:DistributionCode ";
                parameters.Add(new OracleParameter(":DistributionCode", condition.DistributionCode));
            }

            if (condition.BeginTime != null && condition.EndTime != null)
            {
                where += " and expressInfo.BackStationCreateTime BETWEEN :CreatTime AND :EndTime";
                where1 += " and expressInfo.WaybillCreatTime BETWEEN :CreatTime AND :EndTime";
                parameters.Add(new OracleParameter(":CreatTime", condition.BeginTime));
                parameters.Add(new OracleParameter(":EndTime", condition.EndTime));
            }

            //支付方式
            if (condition.PayType != -1)
            {
                where += " AND expressInfo.AcceptType = :AcceptType ";
                where1 += " AND expressInfo.AcceptType = :AcceptType ";

                parameters.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2, 40) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            if(!string.IsNullOrEmpty(condition.MerchantIDs))
            {
                where += string.Format(" AND expressInfo.MerchantID in ({0})", condition.MerchantIDs);
                where1 += string.Format(" AND expressInfo.MerchantID in ({0})", condition.MerchantIDs);
            }

            strSql1 += where;
            strSql2 += where1;
            strSql1 += " GROUP BY TO_Char(expressInfo.BackStationCreateTime,'YYYY-MM-DD'), ec.ExpressCompanyID, expressInfo.Sources ";
            strSql2 += " GROUP BY TO_Char(expressInfo.WaybillCreatTime,'YYYY-MM-DD') , ec.ExpressCompanyID, expressInfo.Sources  ";

            string strSql = "";

            if (condition.TransferPayType == 0)
            {
                strSql = strSql1 + " UNION ALL " + strSql2;
            }
            else if (condition.TransferPayType != 2)
            {
                strSql = strSql2;
            }
            else
            {
                strSql = strSql1;
            }

            strSql = " SELECT (ROW_NUMBER() OVER (ORDER BY CreateTime)) ID,a.* from (" + strSql + " ) a ";
            parameters.Add(new OracleParameter(":DistributionCode", condition.DistributionCode));
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters.ToArray<OracleParameter>()).Tables[0];
        }

        public DataTable GetTransferFinanceDetailDataV2(SearchCondition condition)
        {
            string strSql1 =
                @"SELECT  ec.CompanyName 配送站,
                    expressInfo.WaybillNO 运单号, 
                    expressInfo.CustomerOrder 订单号, 
                    expressInfo.TransferFee 配送费,
                    expressInfo.DinsureFee 保价费, 
                    '现金' 支付方式,
                    CASE WHEN expressInfo.POSCode IS NULL OR expressInfo.POSCode='' THEN e.POSCode ELSE expressInfo.POSCode END POS终端号,      
                    expressInfo.BackStationCreateTime 统计时间,
                    expressInfo.WaybillCreatTime as 提交时间,
                    mbi.MerchantName as 商家
                FROM Fms_ExpressReceivefeeinfo expressInfo              
		            JOIN ExpressCompany ec ON expressInfo.DeliverStationID = ec.ExpressCompanyID
	                LEFT JOIN MerchantBaseInfo mbi ON expressInfo.MerchantID = mbi.ID  
                    LEFT JOIN Distribution dis ON ec.DistributionCode = dis.DistributionCode
                    LEFT JOIN Employee e ON expressInfo.DeliverManID=e.EmployeeID
                WHERE  expressInfo.DistributionCode=:DistributionCode
                    AND expressInfo.Status='3'	
                   -- AND expressInfo.MerchantID=30 
                    AND expressInfo.IsDeleted=0 
                    AND expressInfo.TransferPayType=2 ";

            string strSql2 =
                @"SELECT ec.CompanyName 配送站,
                       expressInfo.WaybillNO 运单号, 
                       expressInfo.CustomerOrder 订单号, 
                       expressInfo.TransferFee 配送费,
                       expressInfo.DinsureFee 保价费, 
                       '现金' 支付方式,
                       e.POSCode POS终端号,
                       expressInfo.WaybillCreatTime 统计时间,
                       expressInfo.WaybillCreatTime 提交时间，
                       mbi.MerchantName as 商家
                FROM Fms_ExpressReceivefeeinfo expressInfo            
			                 LEFT JOIN ExpressCompany ec ON expressInfo.ReceiveStationID = ec.ExpressCompanyID
			                 LEFT JOIN MerchantBaseInfo mbi ON expressInfo.MerchantID = mbi.ID  
                       LEFT JOIN Distribution dis ON ec.DistributionCode = dis.DistributionCode
                       LEFT JOIN Employee e ON expressInfo.DeliverManID=e.EmployeeID
                WHERE  expressInfo.DistributionCode=:DistributionCode
                        --AND expressInfo.MerchantID=30
	                     AND expressInfo.IsDeleted=0
                       --AND expressInfo.Status<>'-5' 
                       AND expressInfo.DeliverStationID<>'-1'
                       AND (expressInfo.TransferPayType=1 ) ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            string where = "";
            string where1 = "";
            if (condition.DeliverStation != 0)
            {
                where += " and expressInfo.DeliverStationID=:DeliverStation";
                where1 += " and expressInfo.ReceiveStationID=:DeliverStation ";
                parameters.Add(new OracleParameter(":DeliverStation", condition.DeliverStation));
            }
            if (condition.BeginTime != null && condition.EndTime != null)
            {
                where += " and expressInfo.BackStationCreateTime BETWEEN :CreatTime AND :EndTime";
                where1 += " and expressInfo.WaybillCreatTime BETWEEN :CreatTime AND :EndTime";
                parameters.Add(new OracleParameter(":CreatTime", condition.BeginTime));
                parameters.Add(new OracleParameter(":EndTime", condition.EndTime));
            }

            //支付方式
            if (condition.PayType != -1)
            {
                where += " AND expressInfo.AcceptType = :AcceptType ";
                where1 += " AND expressInfo.AcceptType = :AcceptType ";
                parameters.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2, 40) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            if (!string.IsNullOrEmpty(condition.MerchantIDs))
            {
                where += string.Format(" AND expressInfo.MerchantID in ({0})", condition.MerchantIDs);
                where1 += string.Format(" AND expressInfo.MerchantID in ({0})", condition.MerchantIDs);
            }
            strSql1 += where;
            strSql2 += where1;

            string strSql = "";

            if (condition.TransferPayType == 0)
            {
                strSql = strSql1 + " UNION ALL " + strSql2;
            }
            else if (condition.TransferPayType != 2)
            {
                strSql = strSql2;
            }
            else
            {
                strSql = strSql1;
            }

            strSql = " SELECT (ROW_NUMBER() OVER (ORDER BY 提交时间)) AS 序号,a.* from (" + strSql + " ) a ";

            parameters.Add(new OracleParameter(":DistributionCode",condition.DistributionCode));

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters.ToArray<OracleParameter>()).Tables[0];

        }

        public DataTable GetAllDetailsFinanceDataV2(SearchCondition condition)
        {
            IList<OracleParameter> paramList = new List<OracleParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND feeInfo.AcceptType = :AcceptType ");
                paramList.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2, 40) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = :POSBANKID ");
                paramList.Add(new OracleParameter(":POSBANKID", OracleDbType.Varchar2, 40) { Value = condition.POSType });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND feeInfo.Sources = :Source ");
                paramList.Add(new OracleParameter(":Source", OracleDbType.Decimal) { Value = condition.Source });
            }
            //商家条件
            if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
            {
                strWhere.Append(string.Format(" AND feeInfo.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
            }

            var strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT (ROW_NUMBER() OVER (ORDER BY feeInfo.BackStationCreateTime)) ID, 
                                        ec.CompanyName,
                                        feeInfo.WaybillNO WaybillNO, 
                                        feeInfo.CustomerOrder, 
                                        feeInfo.NeedAmount, 
                                        feeInfo.NeedBackAmount, 
                                        feeInfo.AcceptType,
                                        CASE WHEN feeInfo.FinancialStatus IS NULL THEN '未收款'
                                            WHEN feeInfo.FinancialStatus = 0 THEN '未收款'
                                            ELSE '已收款' END FinancialStatus,
                                        CASE WHEN feeInfo.POSCode IS NULL OR feeInfo.POSCode='' THEN e.POSCode 
                                            ELSE feeInfo.POSCode END POSCode,
                                        s.STATUSNAME, 
                                        feeInfo.BackStationCreateTime AS BackStationTime
                                FROM fms_receivefeeinfo feeInfo
                                        JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID
                                        JOIN Employee e ON e.EmployeeID = feeInfo.DeliverManID
                                        LEFT JOIN STATUSINFO s ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
                                WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                                        AND feeInfo.BackStationCreateTime < :EndTime
                                        AND feeInfo.SignStatus = :SignStatus 
                                        AND feeInfo.ISDELETED=0
                                        {0}
                                         AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2
                                        AND ec.DistributionCode = :DistributionCode
                                        AND feeInfo.Sources IN(0,1)
        
                                UNION ALL --第三方配送商的明细数据

                                SELECT (ROW_NUMBER() OVER (ORDER BY feeInfo.BackStationCreateTime)) ID, 
                                        CASE WHEN ec.DistributionCode=:DistributionCode THEN ec.CompanyName
                                            ELSE dis.DistributionName END CompanyName,
                                        feeInfo.WaybillNO WaybillNO, 
                                        feeInfo.CustomerOrder, 
                                        feeInfo.NeedAmount, 
                                        feeInfo.NeedBackAmount, 
                                        feeInfo.AcceptType,
                                        CASE WHEN feeInfo.FinancialStatus IS NULL THEN '未收款'
                                            WHEN feeInfo.FinancialStatus = 0 THEN '未收款'
                                            ELSE '已收款' END FinancialStatus,
                                        CASE WHEN feeInfo.POSCode IS NULL OR feeInfo.POSCode='' THEN e.POSCode 
                                            ELSE feeInfo.POSCode END POSCode,
                                        s.STATUSNAME, 
                                        feeInfo.BackStationCreateTime AS BackStationTime
                                FROM fms_receivefeeinfo feeInfo
                                        JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID
                                        JOIN Distribution dis ON dis.DistributionCode = ec.DistributionCode
                                        JOIN Employee e ON e.EmployeeID = feeInfo.DeliverManID
                                        LEFT JOIN STATUSINFO s ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
                                WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                                        AND feeInfo.BackStationCreateTime < :EndTime
                                        AND feeInfo.SignStatus = :SignStatus   
                                        AND feeInfo.IsDeleted=0
                                        {0}
                                         AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2
                                        AND feeInfo.Sources=2 
                                        AND 
                                        (feeInfo.DistributionCode=:DistributionCode OR ec.DistributionCode= :DistributionCode)", strWhere.ToString());

            //添加参数
            paramList.Add(new OracleParameter(":BeginTime", OracleDbType.Date) { Value = condition.BeginTime });
            paramList.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new OracleParameter(":SignStatus", OracleDbType.Varchar2, 40) { Value = ((int)WayBillStatus.Success).ToString() });
            paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 40) { Value = condition.DistributionCode });

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), paramList.ToArray<OracleParameter>()).Tables[0];
        }

        public DataTable GetDetailsFinanceDataV2(SearchCondition condition)
        {
            IList<OracleParameter> paramList = new List<OracleParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND feeInfo.AcceptType = :AcceptType ");
                paramList.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2, 40) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }

            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = :POSBANKID ");
                paramList.Add(new OracleParameter(":POSBANKID", OracleDbType.Varchar2, 40) { Value = condition.POSType });
            }
            //拼装配送站
            if (condition.DeliverStation != 0)
            {
                strWhere.Append(" AND feeInfo.DeliverStationID = :DeliverStation ");
                paramList.Add(new OracleParameter(":DeliverStation", OracleDbType.Decimal) { Value = condition.DeliverStation });
            }
            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND feeInfo.Sources = :Source ");
                paramList.Add(new OracleParameter(":Source", OracleDbType.Decimal) { Value = condition.Source });
            }
            //商家条件
            if (condition.Source == 0)
            {
                if (!string.IsNullOrEmpty(condition.MerchantID.ToString()))
                {
                    switch (condition.MerchantID)
                    {
                        case 1:
                            strWhere.Append(" AND NVL(feeInfo.ComeFrom,0) NOT IN (18,19) ");//避免null 的差异
                            break;
                        case 2:
                            strWhere.Append(" AND feeInfo.ComeFrom IN (18,19) ");
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (condition.Source == 2)
            {
                if (condition.MerchantID != 0)
                {
                    strWhere.Append(" AND feeInfo.MerchantID = :MerchantID ");
                    paramList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = condition.MerchantID });
                }
            }
            //add by wangyongc 2011-10-25 增加配送商条件的判断
            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                strWhere.Append(" AND ec.DistributionCode = :DistributionCode ");
                paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = condition.DistributionCode });
            }
            //拼装必选条件
            var strSql = new StringBuilder();

            strSql.AppendFormat(@"
                SELECT (ROW_NUMBER() OVER (ORDER BY feeInfo.BackStationCreateTime)) ID, 
                   ec.CompanyName,
                   feeInfo.WaybillNO WaybillNO, 
                   feeInfo.CustomerOrder, 
                   feeInfo.NeedAmount, 
                   feeInfo.NeedBackAmount, 
                   feeInfo.AcceptType,
                   CASE WHEN feeInfo.FinancialStatus IS NULL THEN '未收款'
                        WHEN feeInfo.FinancialStatus = 0 THEN '未收款'
                        ELSE '已收款' END FinancialStatus,
                   CASE WHEN feeInfo.POSCode IS NULL OR feeInfo.POSCode='' THEN e.POSCode 
                        ELSE feeInfo.POSCode END POSCode,
                   s.STATUSNAME, 
                   feeInfo.WaybillCreateTime as IntoTime, 
                   feeInfo.SignInfoCreateTime AS CreateTime
                FROM fms_receivefeeinfo feeInfo
                   JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID
                   JOIN Employee e ON e.EmployeeID = feeInfo.DeliverMan
                   LEFT JOIN STATUSINFO s ON e.POSBANKID=s.STATUSNO AND s.STATUSTYPENO=401
                WHERE  feeInfo.BackStationCreateTime >= :BeginTime 
                   AND feeInfo.BackStationCreateTime < :EndTime
                   AND feeInfo.SignStatus = :SignStatus  
                   AND feeInfo.IsDeleted=0 
                   {0}   
            ", strWhere.ToString());

            //添加参数
            paramList.Add(new OracleParameter(":BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new OracleParameter(":EndTime", SqlDbType.DateTime) { Value = condition.EndTime.AddDays(1) });
            paramList.Add(new OracleParameter(":SignStatus", OracleDbType.Varchar2, 40) { Value = ((int)WayBillStatus.Success).ToString() });

            return OracleHelper.ExecuteDataset(LMSReadOnlyConnection,
                CommandType.Text, strSql.ToString(),
                paramList.ToArray<OracleParameter>()).Tables[0];
        }

        public DataTable GetTotalFinanceDataV2(SearchCondition condition, bool displayTotalCount)
        {
            IList<OracleParameter> paramList = new List<OracleParameter>();//参数列表
            //拼装可选条件
            var strWhere = new StringBuilder();
            //wangyongc 2011-08-19 站点条件
            if (condition.DeliverStation != 0)
            {
                strWhere.Append(" AND feeInfo.DeliverStationID = :DeliverStation ");

                paramList.Add(new OracleParameter(":DeliverStation", OracleDbType.Decimal) { Value = condition.DeliverStation });
            }

            //hemingyu 2011-08-24 商家条件 update by zengwei 20120509 20120817
            if (condition.Source == 0)
            {
                if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
                {
                    switch (condition.MerchantIDs)
                    {
                        case "1":
                            strWhere.Append(" AND NVL(feeInfo.ComeFrom,0) NOT IN (18,19) ");//避免NULL的查询差异
                            break;
                        case "2":
                            strWhere.Append(" AND feeInfo.ComeFrom IN (18,19) ");
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (condition.Source == 2)
            {
                if (!string.IsNullOrEmpty(condition.MerchantIDs.Trim()))
                {
                    strWhere.Append(string.Format(" AND feeInfo.MerchantID IN ({0})", condition.MerchantIDs.Trim()));
                }
            }
            
            //支付方式
            if (condition.PayType != -1)
            {
                strWhere.Append(" AND feeInfo.AcceptType = :AcceptType ");
                paramList.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2, 40) { Value = EnumHelper.GetDescription((PaymentType)condition.PayType) });
            }
            //POS机类型 add by wangyongc 2012-03-05
            if (condition.POSType != -1)
            {
                strWhere.Append(" AND e.POSBANKID = :POSBANKID ");
                paramList.Add(new OracleParameter(":POSBANKID", OracleDbType.Varchar2, 40) { Value = condition.POSType });
            }

            //拼装来源
            if (condition.Source != -1)
            {
                strWhere.Append(" AND feeInfo.Sources = :Source ");
                paramList.Add(new OracleParameter(":Source", OracleDbType.Decimal) { Value = condition.Source });
            }

            string CashForPayment = EnumHelper.GetDescription(PaymentType.Cash);
            string POSForPayment = EnumHelper.GetDescription(PaymentType.POS);

            var strSql = new StringBuilder();
            //拼装必选条件
            if (displayTotalCount)
            {
                //总汇总
                //选择来源时
                if (condition.Source != -1)
                {
                    strSql.AppendFormat(@"    
					    SELECT t.CashWaybillCount, t.POSWaybillCount, (t.CashWaybillCount + t.POSWaybillCount) SucessWaybillCount,
                               t.AcceptAmount, t.BackWaybillCount, t.CashRealOutSum, (t.AcceptAmount - t.CashRealOutSum) SaveAmount
                        FROM 
                        ( 
                          SELECT  
                             SUM(CASE WHEN feeInfo.AcceptType = :Cash THEN 1 ELSE 0 END) CashWaybillCount,  
                             SUM(CASE WHEN feeInfo.AcceptType = :POS THEN 1 ELSE 0 END) POSWaybillCount, 
                             SUM(NVL(feeInfo.FactAmount,0)) AcceptAmount,
                             SUM(CASE WHEN feeInfo.WaybillType = :Returned THEN 1 ELSE 0 END) BackWaybillCount,
                             SUM(NVL(feeInfo.FactBackAmount,0)) CashRealOutSum
                          FROM fms_receivefeeinfo feeInfo 
                             JOIN Employee e ON feeInfo.DeliverManID=e.EmployeeID 
                             JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID               
                          WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                             AND feeInfo.BackStationCreateTime < :EndTime
                             AND feeInfo.SignStatus = :SignStatus 
                             AND feeInfo.IsDeleted = 0
                             AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2                    
                          {0}
                          {1}   --来源为Vancl，Vjia时筛选配送商为如风达
                        ) t 
					", strWhere.ToString(), condition.Source > 1 ? "" : " AND ec.DistributionCode = :DistributionCode");

                    //添加参数
                    paramList.Add(new OracleParameter(":Cash", OracleDbType.Varchar2, 40) { Value = CashForPayment });
                    paramList.Add(new OracleParameter(":POS", OracleDbType.Varchar2, 40) { Value = POSForPayment });
                    paramList.Add(new OracleParameter(":Returned", OracleDbType.Varchar2, 40) { Value = ((int)WayBillType.Returned).ToString() });
                    paramList.Add(new OracleParameter(":BeginTime", OracleDbType.Date) { Value = condition.BeginTime });
                    paramList.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = condition.EndTime.AddDays(1) });
                    paramList.Add(new OracleParameter(":SignStatus", OracleDbType.Varchar2, 40) { Value = ((int)WayBillStatus.Success).ToString() });

                    if (condition.Source <= 1)
                    {
                        paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 40) { Value = condition.DistributionCode });
                    }
                }
                else
                {
                    strSql.AppendFormat(@"    
					    SELECT SUM(t.CashWaybillCount) CashWaybillCount, 
                            SUM(t.POSWaybillCount) POSWaybillCount, 
                            SUM(t.CashWaybillCount) + SUM(t.POSWaybillCount) SucessWaybillCount,
                            SUM(t.AcceptAmount) AcceptAmount, 
                            SUM(t.BackWaybillCount) BackWaybillCount, 
                            SUM(t.CashRealOutSum) CashRealOutSum, 
                            SUM(t.AcceptAmount) - SUM(t.CashRealOutSum) SaveAmount 
                        FROM 
                        ( 
                            SELECT  
                                SUM(CASE WHEN feeInfo.AcceptType = :Cash THEN 1 ELSE 0 END) CashWaybillCount,  
                                SUM(CASE WHEN feeInfo.AcceptType = :POS THEN 1 ELSE 0 END) POSWaybillCount, 
                                SUM(NVL(feeInfo.FactAmount,0)) AcceptAmount,
                                SUM(CASE WHEN feeInfo.WaybillType = :Returned THEN 1 ELSE 0 END) BackWaybillCount,
                                SUM(NVL(feeInfo.FactBackAmount,0)) CashRealOutSum
                            FROM fms_receivefeeinfo feeInfo
                                JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID 
                                JOIN Employee e ON feeInfo.DeliverManID = e.EmployeeID               
                            WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                                AND feeInfo.BackStationCreateTime < :EndTime
                                AND feeInfo.SignStatus = :SignStatus  
                                AND feeInfo.IsDeleted = 0                   
                                {0}
                                AND feeInfo.Sources IN(0,1) 
                                AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2 
                                AND ec.DistributionCode = :DistributionCode
      
                            UNION ALL
  
                            SELECT  
                                SUM(CASE WHEN feeInfo.AcceptType = :Cash THEN 1 ELSE 0 END) CashWaybillCount,  
                                SUM(CASE WHEN feeInfo.AcceptType = :POS THEN 1 ELSE 0 END) POSWaybillCount, 
                                SUM(NVL(feeInfo.FactAmount,0)) AcceptAmount,
                                SUM(CASE WHEN feeInfo.WaybillType = :Returned THEN 1 ELSE 0 END) BackWaybillCount,
                                SUM(NVL(feeInfo.FactBackAmount,0)) CashRealOutSum
                            FROM fms_receivefeeinfo feeInfo
                                JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID 
                                INNER JOIN Employee e ON feeInfo.DeliverManID=e.EmployeeID               
                            WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                                AND feeInfo.BackStationCreateTime < :EndTime
                                AND feeInfo.SignStatus = :SignStatus  
                                AND feeInfo.IsDeleted = 0                   
                                {0}
                                AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2 
                                AND feeInfo.Sources =2 
                                AND ec.DistributionCode <> :DistributionCode 
                                AND feeInfo.DistributionCode = :DistributionCode 
      
                            UNION ALL
  
                            SELECT  
                                SUM(CASE WHEN feeInfo.AcceptType = :Cash THEN 1 ELSE 0 END) CashWaybillCount,  
                                SUM(CASE WHEN feeInfo.AcceptType = :POS THEN 1 ELSE 0 END) POSWaybillCount, 
                                SUM(NVL(feeInfo.FactAmount,0)) AcceptAmount,
                                SUM(CASE WHEN feeInfo.WaybillType = :Returned THEN 1 ELSE 0 END) BackWaybillCount,
                                SUM(NVL(feeInfo.FactBackAmount,0)) CashRealOutSum
                            FROM fms_receivefeeinfo feeInfo
                                JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID 
                                INNER JOIN Employee e ON feeInfo.DeliverManID=e.EmployeeID               
                            WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                                AND feeInfo.BackStationCreateTime < :EndTime
                                AND feeInfo.SignStatus = :SignStatus  
                                AND feeInfo.IsDeleted = 0                   
                                {0}
                                 AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2 
                                AND feeInfo.Sources =2 
                                AND ec.DistributionCode=:DistributionCode 
                                AND feeInfo.DistributionCode=:DistributionCode
                        ) t", strWhere.ToString());

                    //添加参数
                    paramList.Add(new OracleParameter(":Cash", OracleDbType.Varchar2, 40) { Value = CashForPayment });
                    paramList.Add(new OracleParameter(":POS", OracleDbType.Varchar2, 40) { Value = POSForPayment });
                    paramList.Add(new OracleParameter(":Returned", OracleDbType.Varchar2, 40) { Value = ((int)WayBillType.Returned).ToString() });
                    paramList.Add(new OracleParameter(":BeginTime", OracleDbType.Date) { Value = condition.BeginTime });
                    paramList.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = condition.EndTime.AddDays(1) });
                    paramList.Add(new OracleParameter(":SignStatus", OracleDbType.Varchar2, 40) { Value = ((int)WayBillStatus.Success).ToString() });
                    paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 40) { Value = condition.DistributionCode });
                }
            }
            else
            {
                int MerchantIDs = 0;
                int.TryParse(condition.MerchantIDs, out MerchantIDs);

                paramList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = MerchantIDs });

                //站点汇总
                strSql.AppendFormat(@"      
				    SELECT (ROW_NUMBER() OVER (ORDER BY {0} {1})) ID, 
                        t.ExpressCompanyID
                           || '&' 
                           || t.CompanyName 
                           || '&' 
                           || CASE WHEN t.Sources=0 
                                THEN :MerchantID 
                                else t.MerchantID end  
                           || '&' 
                           || t.SourceName 
                           || '&' 
                           || t.Sources 
                           || '&' 
                           || t.CreateTime
                           || '&' 
                           || t.DistributionCode QueryParams,
                        DistributionName, 
                        CompanyName, 
                        MerchantCode,
                        SourceName, 
                        NVL(CashWaybillCount,0) CashWaybillCount , 
                        NVL(POSWaybillCount,0) POSWaybillCount,
                        NVL(AcceptAmount,0) AcceptAmount,
                        NVL(BackWaybillCount,0) BackWaybillCount,
                        NVL(CashRealOutSum,0) CashRealOutSum, 
                        NVL(SaveAmount,0) SaveAmount, 
                        NVL(CreateTime,null) CreateTime 
                    FROM
                    (
                        SELECT  
                            MAX(ec.DistributionCode) DistributionCode, 
                            MAX(DistributionName) DistributionName,
                            ec.ExpressCompanyID,
                            MAX(ec.CompanyName) CompanyName, 
                            feeInfo.Sources,
                            MAX(si.StatusName) SourceName,
                            0 MerchantID,
                            '' MerchantCode,
                            SUM(CASE WHEN feeInfo.AcceptType = :Cash THEN 1 ELSE 0 END) CashWaybillCount,  
                            SUM(CASE WHEN feeInfo.AcceptType = :POS THEN 1 ELSE 0 END) POSWaybillCount, 
                            SUM(feeInfo.FactAmount) AcceptAmount,
                            SUM(CASE WHEN feeInfo.WaybillType = :Returned THEN 1 ELSE 0 END) BackWaybillCount,
                            SUM(feeInfo.FactBackAmount) CashRealOutSum,
                            (SUM(NVL(feeInfo.FactAmount,0)) - SUM(NVL(feeInfo.FactBackAmount,0))) SaveAmount,
                            TO_Char(feeInfo.BackStationCreateTime,'YYYY-MM-DD') CreateTime
                         FROM fms_receivefeeinfo feeInfo               
                            JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID
                            LEFT JOIN MerchantBaseInfo mbi ON feeInfo.MerchantID = mbi.ID
                            LEFT JOIN StatusInfo si ON feeInfo.Sources = CAST(si.StatusNO AS INT) 
                            AND StatusTypeNO = 3   
                            LEFT JOIN Distribution dis ON ec.DistributionCode = dis.DistributionCode
                            LEFT JOIN Employee e ON feeInfo.DeliverManID=e.EmployeeID
                         WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                            AND feeInfo.BackStationCreateTime < :EndTime                       
                            {2}
                            AND feeInfo.Sources IN(0,1)
                            AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2 
                            AND feeInfo.SignStatus = :SignStatus 
                            AND ec.DistributionCode=:DistributionCode 
                            AND feeInfo.IsDeleted=0
                         GROUP BY TO_Char(feeInfo.BackStationCreateTime,'YYYY-MM-DD'),
                            ec.ExpressCompanyID,
                            feeInfo.Sources   
          
                         UNION ALL
     
                         SELECT  
                            MAX(ec.DistributionCode), 
                            MAX(DistributionName),
                            ec.ExpressCompanyID,
                            MAX(ec.CompanyName), 
                            MAX(feeInfo.Sources),
                            MAX(mbi.MerchantName),
                            mbi.ID,
                            Max(mbi.MerchantCode) MerchantCode,
                            SUM(CASE WHEN feeInfo.AcceptType = :Cash THEN 1 ELSE 0 END) CashWaybillCount,  
                            SUM(CASE WHEN feeInfo.AcceptType = :POS THEN 1 ELSE 0 END) POSWaybillCount, 
                            SUM(feeInfo.FactAmount) AcceptAmount,
                            SUM(CASE WHEN feeInfo.WaybillType = :Returned THEN 1 ELSE 0 END) BackWaybillCount,
                            SUM(feeInfo.FactBackAmount) CashRealOutSum,
                            (SUM(NVL(feeInfo.FactAmount,0)) - SUM(NVL(feeInfo.FactBackAmount,0))) SaveAmount,
                            TO_Char(feeInfo.BackStationCreateTime,'YYYY-MM-DD') CreateTime
                         FROM fms_receivefeeinfo feeInfo                
                            JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID
                            LEFT JOIN MerchantBaseInfo mbi ON feeInfo.MerchantID = mbi.ID   
                            LEFT JOIN Distribution dis ON ec.DistributionCode = dis.DistributionCode
                            INNER JOIN Employee e ON feeInfo.DeliverManID=e.EmployeeID
                         WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                            AND feeInfo.BackStationCreateTime < :EndTime
                            AND feeInfo.Sources = 2
                            {2}
                            AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2  
                            AND feeInfo.SignStatus = :SignStatus 
                            AND ec.DistributionCode=:DistributionCode  
                            AND feeInfo.IsDeleted=0                     
                         GROUP BY TO_Char(feeInfo.BackStationCreateTime,'YYYY-MM-DD'), 
                            ec.ExpressCompanyID, 
                            mbi.ID
     
                         --add by wangyongc 2011-10-25 Ôö¼ÓÁËÅäËÍÉÌµÄ²éÑ¯
     
                         UNION ALL

                         SELECT     
                            MAX(ec.DistributionCode) DistributionCode, 
                            MAX(DistributionName) DistributionName,
                            0 as ExpressCompanyID,
                            MAX(DistributionName) CompanyName, 
                            feeInfo.Sources,							   
                            CASE WHEN feeInfo.Sources=2 
                                 THEN MAX(mbi.MerchantName) 
                                 ELSE MAX(si.StatusName) END SourceName,
                            mbi.ID MerchantID,
                            Max(mbi.MerchantCode) MerchantCode,
                            SUM(CASE WHEN feeInfo.AcceptType = :Cash THEN 1 ELSE 0 END) CashWaybillCount,  
                            SUM(CASE WHEN feeInfo.AcceptType = :POS THEN 1 ELSE 0 END) POSWaybillCount, 
                            SUM(NVL(feeInfo.FactAmount,0)) AcceptAmount,
                            SUM(CASE WHEN feeInfo.WaybillType = :Returned THEN 1 ELSE 0 END) BackWaybillCount,
                            SUM(NVL(feeInfo.FactBackAmount,0)) CashRealOutSum,
                            (SUM(NVL(feeInfo.FactAmount,0)) - SUM(NVL(feeInfo.FactBackAmount,0))) SaveAmount,
                            TO_Char(feeInfo.BackStationCreateTime,'YYYY-MM-DD') CreateTime
                         FROM fms_receivefeeinfo feeInfo                
                            JOIN ExpressCompany ec ON feeInfo.DeliverStationID = ec.ExpressCompanyID
                            LEFT JOIN MerchantBaseInfo mbi ON feeInfo.MerchantID = mbi.ID
                            LEFT JOIN StatusInfo si ON feeInfo.Sources = CAST(si.StatusNO AS INT) AND StatusTypeNO = 3   
                            LEFT JOIN Distribution dis ON ec.DistributionCode = dis.DistributionCode
                            INNER JOIN Employee e ON feeInfo.DeliverManID=e.EmployeeID
                         WHERE feeInfo.BackStationCreateTime >= :BeginTime 
                            AND feeInfo.BackStationCreateTime < :EndTime 
                            AND feeInfo.Sources = 2   						
                            {2}
                            AND feeInfo.TransferPayType<>2 AND feeInfo.accountType<>2 
                            AND feeInfo.SignStatus = :SignStatus 
                            AND feeInfo.DistributionCode = :DistributionCode 
                            AND ec.DistributionCode<>:DistributionCode 
                            AND feeInfo.IsDeleted=0
                         GROUP BY TO_Char(feeInfo.BackStationCreateTime,'YYYY-MM-DD'),
                            ec.DistributionCode,
                            feeInfo.Sources,
                            mbi.ID
                    ) t ORDER BY {0} {1}", condition.OrderBy, condition.Direction, strWhere.ToString());

                //添加参数
                paramList.Add(new OracleParameter(":Cash", OracleDbType.Varchar2, 40) { Value = CashForPayment });
                paramList.Add(new OracleParameter(":POS", OracleDbType.Varchar2, 40) { Value = POSForPayment });
                paramList.Add(new OracleParameter(":Returned", OracleDbType.Varchar2, 40) { Value = ((int)WayBillType.Returned).ToString() });
                paramList.Add(new OracleParameter(":BeginTime", OracleDbType.Date) { Value = condition.BeginTime });
                paramList.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = condition.EndTime.AddDays(1) });
                paramList.Add(new OracleParameter(":SignStatus", OracleDbType.Varchar2, 40) { Value = ((int)WayBillStatus.Success).ToString() });
                paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 40) { Value = condition.DistributionCode });
            }

            //返回结果
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<OracleParameter>()).Tables[0];
        }

        public DataTable Test(string sql)
        {
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text,sql);

            return null;
        }


        public void UpdateSynInfo(long waybillNo, string acceptType)
        {
            string sql = "update FMS_EXPRESSRECEIVEFEEINFO set acceptType=:acceptType,updatetime=:updatetime where WaybillNO=:WaybillNO and TransferPayType=1";

            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter(":acceptType", OracleDbType.Varchar2) { Value = acceptType });
            paramList.Add(new OracleParameter(":updatetime", OracleDbType.Date) { Value = DateTime.Now });
            paramList.Add(new OracleParameter(":WaybillNO", OracleDbType.Long) { Value = waybillNo });

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql,paramList.ToArray());
        }

        public DataTable GetNullAcceptTypeValues()
        {
            string sql = "select WaybillNO from FMS_EXPRESSRECEIVEFEEINFO where TransferPayType=1 and AcceptType is Null";

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetRepairData()
        {
            string sql = "select WaybillNO from fms_receivefeeinfo where AcceptType is null and IsDeleted=0";

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        public void UpdateRepairData(long waybillNo, string acceptType)
        {
            string sql = @"update fms_receivefeeinfo set AcceptType=:AcceptType where WaybillNO=:WaybillNO";

            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2) { Value = acceptType });
            paramList.Add(new OracleParameter(":WaybillNO", OracleDbType.Long) { Value = waybillNo });

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql,paramList.ToArray());
        }

        public DataTable GetErrorBackStationTimeData()
        {
            string sql = @"select feeInfo.waybillno,baseInfo.Backstationtime from Fms_Receivefeeinfo feeInfo 
                  inner join fms_incomeBaseInfo baseInfo on feeInfo.Waybillno=baseInfo.Waybillno
                    where feeInfo.Isdeleted=0 
                  and TO_Char(feeInfo.BackStationCreateTime,'YYYY-MM-DD') != TO_Char(baseInfo.BackStationTime,'YYYY-MM-DD')
                  and feeInfo.Createtime > to_date('2012-12-29','yyyy-mm-dd')";

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql).Tables[0];
        }

        public void UpdateBackStationTime(long waybillNo, DateTime backStationTime)
        {
            string sql = "update fms_receivefeeinfo set BackStationCreateTime=:BackStationCreateTime where WaybillNO=:WaybillNO";

            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter(":BackStationCreateTime", OracleDbType.Date) { Value = backStationTime });
            paramList.Add(new OracleParameter(":WaybillNO", OracleDbType.Long) { Value = waybillNo });

            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, paramList.ToArray());
        }
    }
}
