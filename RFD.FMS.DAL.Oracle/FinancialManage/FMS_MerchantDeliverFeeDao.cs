using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
	public class FMS_MerchantDeliverFeeDao : OracleDao, IFMS_MerchantDeliverFeeDao
	{
        public string BuildSearchCondition(SearchCondition condition, string tableAlias, ref List<OracleParameter> parameterList)
        {
            StringBuilder sbWhere=new StringBuilder();
			//拼装查询条件
			if (condition.DistributionCode == "rfd")
			{
				sbWhere.Append(" AND mi.Sources = 2 ");//如风达不要vancl、vjia
			}
			if (condition.ID != 0)
			{
				sbWhere.Append(" AND {0}.feeid = :ID ");
				parameterList.Add(new OracleParameter(":ID", OracleDbType.Decimal) { Value = condition.ID });
			}
			if (condition.MerchantID != -1)
			{
				sbWhere.Append(" AND mi.ID = :MerchantID ");
				parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = condition.MerchantID });
			}
			if (!String.IsNullOrEmpty(condition.MerchantName))
			{
				sbWhere.Append(" AND mi.MerchantName LIKE :MerchantName ");
				parameterList.Add(new OracleParameter(":MerchantName", OracleDbType.Varchar2, 100) { Value = '%' + condition.MerchantName + '%' });
			}
			if (!String.IsNullOrEmpty(condition.SimpleSpell))
			{
				sbWhere.Append(" AND mi.SimpleSpell LIKE :SimpleSpell ");
				parameterList.Add(new OracleParameter(":SimpleSpell", OracleDbType.Varchar2, 50) { Value = '%' + condition.SimpleSpell + '%' });
			}
			if (!String.IsNullOrEmpty(condition.DistributionCode))
			{
				sbWhere.Append(" AND dmr.DistributionCode=:DistributionCode ");
			}
			if (!String.IsNullOrEmpty(condition.StatusList))
                sbWhere.Append(condition.StatusList.Contains("'0'") ? " AND ({0}.Status is null OR {0}.Status in (" + condition.StatusList.Replace("'", "") + "))" : " AND {0}.Status IN (" + condition.StatusList.Replace("'", "") + ")");
			parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 50) { Value = condition.DistributionCode });

            return string.Format(sbWhere.ToString(),tableAlias);
        }

        public int GetMerchantDeliverFeeStat(SearchCondition condition)
        {
            string sql=@"
			    SELECT  count(1)
                FROM MerchantBaseInfo mi
                JOIN DistributionMerchantRelation dmr ON dmr.MerchantId=mi.ID 
                JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
                LEFT JOIN FMS_MerchantDeliverFee fm ON mi.ID = fm.MerchantID AND fm.DistributionCode=:DistributionCode
                WHERE mi.IsDeleted = 0 AND dmr.IsDeleted=0 AND d.ISDELETE=0 {0}";
            string sqlWhere = string.Empty;
            List<OracleParameter> paramList = new List<OracleParameter>();
            sqlWhere = BuildSearchCondition(condition, "fm", ref paramList);

            sql = string.Format(sql, sqlWhere);
            object n = OracleHelper.ExecuteScalar(ReadOnlyConnection,CommandType.Text,sql,ToParameters(paramList.ToArray()));
            return DataConvert.ToInt(n,0);
        }

		/// <summary>
		/// 根据查询条件获取商家配送费列表
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetMerchantDeliverFeeList(SearchCondition condition,PageInfo pi)
		{
			string sql=@"
			    with t as (SELECT  RowNum as nums,fm.feeid ""ID"",mi.ID MerchantID, MerchantName, SimpleSpell, PaymentType, to_char(PaymentPeriod) PaymentPeriod, PaymentPeriodDate, 
                       DeliverFeeType, to_char(DeliverFeePeriod) DeliverFeePeriod, DeliverFeePeriodDate, FeeFactors,FormulaID,
                       to_char(IsUniformedFee) IsUniformedFee,FormulaParamters, 
                       RefuseFeeRate,
                       ExtraRefuseFeeRate,
                       ReceiveFeeRate,
                       ExtraReceiveFeeRate,
                       BasicDeliverFee,
                       nvl(Status,0) Status,nvl(Status,0) CurrentStatus,
                       fm.FirstWeight,fm.StatPramer,fm.AddWeightPrice,fm.FirstWeightPrice,fm.VolumeParmer,fm.ProtectedParmer,
            fm.ExtraProtected, 
            fm.VisitReturnsFee,
            fm.ExtraVisitReturnsFee,
            fm.ReceivePOSFeeRate,
            fm.ExtraReceivePOSFeeRate,
            fm.VisitChangeFee,
            fm.ExtraVisitChangeFee,
            fm.CreateBy,fm.CreateTime,fm.UpdateBy,fm.UpdateTime,fm.AuditBy,fm.AuditTime,
            fm.VisitReturnsVFee,
            fm.ExtraVisitReturnsVFee,
            fm.CashServiceFee,
            fm.ExtraCashServiceFee,
            fm.POSServiceFee,
            fm.ExtraPOSServiceFee,
            (case fm.ReceiveFeeType when 0 then '周期结' when 1 then '月结' else '' end) ReceiveFeeTypeStr,
            (case fm.ReceivePOSFeeType when 0 then '周期结' when 1 then '月结' else '' end) ReceivePOSFeeTypeStr,
            (case fm.CashServiceType when 0 then '周期结' when 1 then '月结' else '' end) CashServiceTypeStr,
            (case fm.POSServiceType when 0 then '周期结' when 1 then '月结' else '' end) POSServiceTypeStr,
            fm.IsCategory,
            (case fm.IsCategory when 0 then '非品类结' when 1 then '品类结' else '' end)IsCategoryStr,
                        fm.WeightType,fm.WeightValueRule,3 AS LogType
        FROM MerchantBaseInfo mi
                JOIN DistributionMerchantRelation dmr ON dmr.MerchantId=mi.ID 
                JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
        LEFT JOIN FMS_MerchantDeliverFee fm ON mi.ID = fm.MerchantID AND fm.DistributionCode=:DistributionCode
                WHERE mi.IsDeleted = 0 AND dmr.IsDeleted=0 AND d.ISDELETE=0 {0}  ORDER BY mi.ID desc) select * from t where nums between :RowStr and :RowEnd";
			string sqlWhere=string.Empty;
            List<OracleParameter> paramList=new List<OracleParameter>();
            sqlWhere = BuildSearchCondition(condition, "fm", ref paramList);
            List<OracleParameter> parametersTmp = new List<OracleParameter>();
            parametersTmp.Add(new OracleParameter(":RowStr", OracleDbType.Decimal) { Value = pi.CurrentPageStartRowNum });
            parametersTmp.Add(new OracleParameter(":RowEnd", OracleDbType.Decimal) { Value = pi.CurrentPageEndRowNum });

            paramList.AddRange(parametersTmp);
            sql = string.Format(sql,sqlWhere);

            var result = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(paramList.ToArray()));
			return result != null ? result.Tables[0] : null;
		}

        public int GetMerchantDeliverFeeEffectStat(SearchCondition condition)
        {
            string sql=@"
			    SELECT  count(1)
                FROM MerchantBaseInfo mi
                JOIN DistributionMerchantRelation dmr ON dmr.MerchantId=mi.ID 
                JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
                JOIN FMS_MerchantDeliverFeeWait fmw ON mi.ID = fmw.MerchantID AND fmw.DistributionCode=:DistributionCode
                WHERE mi.IsDeleted = 0 AND dmr.IsDeleted=0 AND d.ISDELETE=0  and fmw.isdeleted=0 {0}";
            string sqlWhere = string.Empty;
            List<OracleParameter> paramList = new List<OracleParameter>();
            sqlWhere = BuildSearchCondition(condition, "fmw", ref paramList);

            sql = string.Format(sql, sqlWhere);
            object n = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, ToParameters(paramList.ToArray()));
            return DataConvert.ToInt(n, 0);
        }

        /// <summary>
        /// 根据查询条件获取商家配送费列表 --- 待生效查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetMerchantDeliverFeeEffectList(SearchCondition condition,PageInfo pi)
        {
            string sql= @"
			    with t as (SELECT  RowNum as nums,fmw.feeid ""ID"",mi.ID MerchantID, MerchantName, SimpleSpell, PaymentType, to_char(PaymentPeriod) PaymentPeriod, PaymentPeriodDate, 
                       DeliverFeeType, to_char(DeliverFeePeriod) DeliverFeePeriod, DeliverFeePeriodDate, FeeFactors,FormulaID,
                       to_char(IsUniformedFee) IsUniformedFee,FormulaParamters, RefuseFeeRate, 
                       ReceiveFeeRate, ExtraRefuseFeeRate, ExtraReceiveFeeRate, BasicDeliverFee,
                       nvl(Status,0) Status,nvl(Status,0) CurrentStatus,
                        fmw.FirstWeight,fmw.StatPramer,fmw.AddWeightPrice,fmw.FirstWeightPrice,fmw.VolumeParmer,fmw.ProtectedParmer,
            fmw.VisitReturnsFee,fmw.ReceivePOSFeeRate,fmw.VisitChangeFee,
            fmw.ExtraProtected,
            fmw.ExtraVisitReturnsFee,
            fmw.ExtraReceivePOSFeeRate,
            fmw.ExtraVisitChangeFee,
            fmw.CreateBy,fmw.CreateTime,fmw.UpdateBy,fmw.UpdateTime,fmw.AuditBy,fmw.AuditTime,fmw.VisitReturnsVFee,
            fmw.CashServiceFee,fmw.POSServiceFee,
            fmw.ExtraVisitReturnsVFee,
            fmw.ExtraCashServiceFee,fmw.ExtraPOSServiceFee,
            (case fmw.ReceiveFeeType when 0 then '周期结' when 1 then '月结' else '' end) ReceiveFeeTypeStr,
            (case fmw.ReceivePOSFeeType when 0 then '周期结' when 1 then '月结' else '' end) ReceivePOSFeeTypeStr,
            (case fmw.CashServiceType when 0 then '周期结' when 1 then '月结' else '' end) CashServiceTypeStr,
            (case fmw.POSServiceType when 0 then '周期结' when 1 then '月结' else '' end) POSServiceTypeStr,
             fmw.IsCategory,
            (case fmw.IsCategory when 0 then '非品类结' when 1 then '品类结' else '' end) IsCategoryStr,
                        fmw.WeightType,fmw.WeightValueRule,fmw.EffectDate,4 AS LogType
        FROM MerchantBaseInfo mi
                JOIN DistributionMerchantRelation dmr ON dmr.MerchantId=mi.ID 
                JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
        JOIN FMS_MerchantDeliverFeeWait fmw ON mi.ID = fmw.MerchantID AND fmw.DistributionCode=:DistributionCode
                WHERE mi.IsDeleted = 0 AND dmr.IsDeleted=0 AND d.ISDELETE=0  and fmw.isdeleted=0 {0} ORDER BY mi.ID ) select * from t where nums between :RowStr and :RowEnd
			";
            string sqlWhere = string.Empty;
            List<OracleParameter> paramList = new List<OracleParameter>();
            sqlWhere = BuildSearchCondition(condition, "fmw", ref paramList);
            List<OracleParameter> parametersTmp = new List<OracleParameter>();
            parametersTmp.Add(new OracleParameter(":RowStr", OracleDbType.Decimal) { Value = pi.CurrentPageStartRowNum });
            parametersTmp.Add(new OracleParameter(":RowEnd", OracleDbType.Decimal) { Value = pi.CurrentPageEndRowNum });

            paramList.AddRange(parametersTmp);
            sql = string.Format(sql, sqlWhere);
            var result = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(paramList.ToArray()));
            return result != null ? result.Tables[0] : null;
        }

		/// <summary>
		/// 根据查询条件获取商家配送费历史
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetMerchantDeliverFeeHistory(SearchCondition condition)
		{
			var strSql = new StringBuilder();
			strSql.Append(@"
			    SELECT MerchantID,MerchantName, PaymentType, CAST(PaymentPeriod AS NVARCHAR(20)) PaymentPeriod, 
                       DeliverFeeType, CAST(DeliverFeePeriod AS NVARCHAR(20)) DeliverFeePeriod, FeeFactors,FormulaID,CAST(IsUniformedFee AS NVARCHAR(10)) IsUniformedFee,
				       FormulaParamters, CAST(BasicDeliverFee AS NVARCHAR(20)) BasicDeliverFee,CAST(RefuseFeeRate AS NVARCHAR(20)) RefuseFeeRate, 
                       CAST(ReceiveFeeRate AS NVARCHAR(20)) ReceiveFeeRate,FormulaTemplate,CAST(fm.UpdateBy AS NVARCHAR(20)) UpdateBy,fm.UpdateCode,fm.UpdateTime,
                       CAST(AuditBy AS NVARCHAR(20)) AuditBy,AuditCode,AuditTime,CAST(AuditResult AS NVARCHAR(20)) AuditResult,NVL(fm.Status,0) Status,NVL(fm.Status,0) CurrentStatus
				FROM MerchantBaseInfo mi
				LEFT JOIN FMS_MerchantDeliverFeeHistory fm
				ON mi.ID = fm.MerchantID
				LEFT JOIN Formula fu 
				ON fm.FormulaID = fu.ID
                WHERE 1 = 1 
			");
			//拼装查询条件
			if (condition.MerchantID != -1)
			{
				strSql.Append(" AND fm.MerchantID = :MerchantID ");
			}
			strSql.AppendFormat(" ORDER BY fm.UpdateTime DESC");
			IList<OracleParameter> paramList = new List<OracleParameter>();
			paramList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = condition.MerchantID });
			var result = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<OracleParameter>());
			return result != null ? result.Tables[0] : null;
		}

		/// <summary>
		/// 获得所有的商家列表
		/// </summary>
		public DataTable GetAllMerchantList()
		{
			var dict = new Dictionary<int, string>();
			var strSql = new StringBuilder();
			strSql.Append(" SELECT ID,ProvinceID,CityID,MerchantName,MerchantCode,MerchantFullName,SimpleSpell,Address,Contacter,ContacterPhone,Telephone,Email,OrderPeak,PickMode,IsDeleted,CreatBy,CreatTime,CreateStationID,UpdateBy,UpdateTime,UpdateStationID ");
			strSql.Append(" FROM MerchantBaseInfo");
			strSql.Append(" WHERE IsDeleted = 0 ");
			var result = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString());
			return result != null ? result.Tables[0] : null;
		}

		/// <summary>
		/// 添加商家配送费信息
		/// </summary>
		/// <param name="model">配送费实体</param>
		/// <returns></returns>
		public int AddMerchantDeliverFee(FMS_MerchantDeliverFee model)
		{
			string sqlStr = @"SELECT count(1) FROM FMS_MerchantDeliverFee fm WHERE MerchantID = {0} AND DistributionCode='{1}'";
			sqlStr = String.Format(sqlStr, model.MerchantID, model.DistributionCode);
			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr);
			if (obj != null)
			{
				if (Convert.ToInt32(obj) > 0)
				{
					return 0;
				}
			}
			sqlStr = @"INSERT into FMS_MerchantDeliverFee
                             (FeeID,
                              MerchantID,
                              PaymentType,
                              PaymentPeriod,
                              PaymentPeriodDate,
                              DeliverFeeType,
                              DeliverFeePeriod,
                              DeliverFeePeriodDate,
                              FeeFactors,
                              IsUniformedFee,
                              BasicDeliverFee,
                              RefuseFeeRate,
                              ReceiveFeeRate,
                              ExtraRefuseFeeRate,
                              ExtraReceiveFeeRate,
                              FormulaID,
                              FormulaParamters,
                              UpdateBy,
                              UpdateTime,
                              UpdateCode,
                              AuditBy,
                              AuditTime,
                              AuditCode,
                              AuditResult,
                              Status,
                              FirstWeight,
                              StatPramer,
                              AddWeightPrice,
                              FirstWeightPrice,
                              VolumeParmer,
                              ProtectedParmer,
							  VisitReturnsFee,
                              VisitChangeFee,
                              ReceivePOSFeeRate,
                              CreateBy,
                              CreateTime,
                              VisitReturnsVFee,
                              CashServiceFee,
                              POSServiceFee,
                              
                              ExtraProtected,
							  ExtraVisitReturnsFee,
                              ExtraVisitChangeFee,
                              ExtraReceivePOSFeeRate,
                              ExtraVisitReturnsVFee,
                              ExtraCashServiceFee,
                              ExtraPOSServiceFee,
                              IsCategory,

							  ReceiveFeeType,
                              ReceivePOSFeeType,
                              CashServiceType,
                              POSServiceType,
                              WeightType,
                              WeightValueRule,
                              DistributionCode,
                              IsChange)
				   VALUES(:FeeID,:MerchantID,:PaymentType,:PaymentPeriod,:PaymentPeriodDate,:DeliverFeeType,:DeliverFeePeriod,:DeliverFeePeriodDate,:FeeFactors,
						  :IsUniformedFee,:BasicDeliverFee,:RefuseFeeRate,:ReceiveFeeRate,:ExtraRefuseFeeRate,:ExtraReceiveFeeRate,:FormulaID,:FormulaParamters,:UpdateBy,sysdate,:UpdateCode,
                          :AuditBy,sysdate,:AuditCode, :AuditResult,:Status,:FirstWeight,:StatPramer,:AddWeightPrice,:FirstWeightPrice,:VolumeParmer,
							:ProtectedParmer,:VisitReturnsFee,:VisitChangeFee,:ReceivePOSFeeRate,:CreateBy,sysdate,:VisitReturnsVFee,
							:CashServiceFee,:POSServiceFee, :ExtraProtected,:ExtraVisitReturnsFee,:ExtraVisitChangeFee,:ExtraReceivePOSFeeRate,:ExtraVisitReturnsVFee,
							:ExtraCashServiceFee,:ExtraPOSServiceFee,:IsCategory,
							:ReceiveFeeType,:ReceivePOSFeeType,:CashServiceType,:POSServiceType,:WeightType,:WeightValueRule,:DistributionCode,:IsChange)";
			IList<OracleParameter> paramList = new List<OracleParameter>();
            model.ID = GetIdNew("SEQ_FMS_MERCHANTDELIVERFEE");
            paramList.Add(new OracleParameter(":FeeID", OracleDbType.Decimal) { Value = model.ID });
			paramList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = model.MerchantID });
			paramList.Add(new OracleParameter(":PaymentType", OracleDbType.Decimal) { Value = (int)model.PaymentType });
			paramList.Add(new OracleParameter(":PaymentPeriod", OracleDbType.Decimal) { Value = model.PaymentPeriod });
			paramList.Add(new OracleParameter(":PaymentPeriodDate", OracleDbType.Date) { Value = model.PaymentPeriodDate });
			paramList.Add(new OracleParameter(":DeliverFeeType", OracleDbType.Decimal) { Value = (int)model.DeliverFeeType });
			paramList.Add(new OracleParameter(":DeliverFeePeriod", OracleDbType.Decimal) { Value = model.DeliverFeePeriod });
			paramList.Add(new OracleParameter(":DeliverFeePeriodDate", OracleDbType.Date) { Value = model.DeliverFeePeriodDate });
			paramList.Add(new OracleParameter(":FeeFactors", OracleDbType.Varchar2, 50) { Value = model.FeeFactors });
			paramList.Add(new OracleParameter(":IsUniformedFee", OracleDbType.Decimal) { Value = model.IsUniformedFee });
			paramList.Add(new OracleParameter(":BasicDeliverFee", OracleDbType.Decimal, 18) { Value = model.BasicDeliverFee });
			paramList.Add(new OracleParameter(":RefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.RefuseFeeRate });
			paramList.Add(new OracleParameter(":ReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceiveFeeRate });

            paramList.Add(new OracleParameter(":ExtraRefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraRefuseFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceiveFeeRate });

			paramList.Add(new OracleParameter(":FormulaID", OracleDbType.Decimal) { Value = model.FormulaID });
			paramList.Add(new OracleParameter(":FormulaParamters", OracleDbType.Varchar2, 100) { Value = model.FormulaParamters });
			paramList.Add(new OracleParameter(":UpdateBy", OracleDbType.Decimal) { Value = model.UpdateUser });
			paramList.Add(new OracleParameter(":UpdateCode", OracleDbType.Varchar2, 20) { Value = model.UpdateUserCode });
			paramList.Add(new OracleParameter(":AuditBy", OracleDbType.Decimal) { Value = model.AuditBy });
			paramList.Add(new OracleParameter(":AuditCode", OracleDbType.Varchar2, 20) { Value = model.AuditCode });
			paramList.Add(new OracleParameter(":AuditResult", OracleDbType.Decimal) { Value = model.AuditResult });
			paramList.Add(new OracleParameter(":Status", OracleDbType.Decimal) { Value = (int)model.Status });
			paramList.Add(new OracleParameter(":FirstWeight", OracleDbType.Decimal, 18) { Value = model.FirstWeight });
			paramList.Add(new OracleParameter(":StatPramer", OracleDbType.Decimal, 18) { Value = model.StatPramer });
			paramList.Add(new OracleParameter(":AddWeightPrice", OracleDbType.Decimal, 18) { Value = model.AddWeightPrice });
			paramList.Add(new OracleParameter(":FirstWeightPrice", OracleDbType.Decimal, 18) { Value = model.FirstWeightPrice });
			paramList.Add(new OracleParameter(":VolumeParmer", OracleDbType.Decimal, 18) { Value = model.VolumeParmer });
			paramList.Add(new OracleParameter(":ProtectedParmer", OracleDbType.Decimal, 18) { Value = model.ProtectedParmer });
			paramList.Add(new OracleParameter(":VisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsFeeRate });
			paramList.Add(new OracleParameter(":VisitChangeFee", OracleDbType.Decimal, 18) { Value = model.VisitChangeFeeRate });
			paramList.Add(new OracleParameter(":ReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceivePosFeeRate });
			paramList.Add(new OracleParameter(":VisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsVFeeRate });
			paramList.Add(new OracleParameter(":CashServiceFee", OracleDbType.Decimal, 18) { Value = model.CashServiceFee });
			paramList.Add(new OracleParameter(":POSServiceFee", OracleDbType.Decimal, 18) { Value = model.POSServiceFee });

            paramList.Add(new OracleParameter(":ExtraProtected", OracleDbType.Decimal, 18) { Value = model.ExtraProtected });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitChangeFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitChangeFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceivePosFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsVFeeRate });
            paramList.Add(new OracleParameter(":ExtraCashServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraCashServiceFee });
            paramList.Add(new OracleParameter(":ExtraPOSServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraPOSServiceFee });

            paramList.Add(new OracleParameter(":IsCategory",OracleDbType.Int32){Value = model.IsCategory});

			paramList.Add(new OracleParameter(":ReceiveFeeType", OracleDbType.Decimal) { Value = model.ReceiveFeeType });
			paramList.Add(new OracleParameter(":ReceivePOSFeeType", OracleDbType.Decimal) { Value = model.ReceivePOSFeeType });
			paramList.Add(new OracleParameter(":CashServiceType", OracleDbType.Decimal) { Value = model.CashServiceType });
			paramList.Add(new OracleParameter(":POSServiceType", OracleDbType.Decimal) { Value = model.POSServiceType });
			paramList.Add(new OracleParameter(":CreateBy", OracleDbType.Decimal) { Value = model.CreateUser });
			paramList.Add(new OracleParameter(":WeightType", OracleDbType.Decimal) { Value = model.WeightType });
			paramList.Add(new OracleParameter(":WeightValueRule", OracleDbType.Decimal) { Value = model.WeightValueRule });
			paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 50) { Value = model.DistributionCode });
			paramList.Add(new OracleParameter(":IsChange", OracleDbType.Int32) { Value = 1 });

			bool flag = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, paramList.ToArray<OracleParameter>()) > 0;
			if (flag)
				return model.ID;
			else
				return -1;
		}

        /// <summary>
        /// 添加商家配送费信息--待生效
        /// </summary>
        /// <param name="model">配送费实体</param>
        /// <returns></returns>
        public int AddEffectMerchantDeliverFee(FMS_MerchantDeliverFee model)
        {
            string sqlStr = @"SELECT count(1) FROM FMS_MerchantDeliverFeeWait fmw WHERE MerchantID = {0} AND DistributionCode='{1}' AND fmw.isdeleted=0";
            sqlStr = String.Format(sqlStr, model.MerchantID, model.DistributionCode);
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, null);
            if (obj != null)
            {
                if (Convert.ToInt32(obj) > 0)
                {
                    return 0;
                }
            }
            sqlStr = @"INSERT into FMS_MerchantDeliverFeeWait(FeeID,MerchantID,PaymentType,PaymentPeriod,PaymentPeriodDate,
                          DeliverFeeType,DeliverFeePeriod,DeliverFeePeriodDate,
                          FeeFactors,IsUniformedFee,BasicDeliverFee,RefuseFeeRate,ReceiveFeeRate,ExtraRefuseFeeRate,ExtraReceiveFeeRate,FormulaID,FormulaParamters,UpdateBy,UpdateTime,UpdateCode,
                          AuditBy,AuditTime,AuditCode,AuditResult,Status,FirstWeight,StatPramer,AddWeightPrice,FirstWeightPrice,VolumeParmer,ProtectedParmer,
							VisitReturnsFee,VisitChangeFee,ReceivePOSFeeRate,CreateBy,CreateTime,VisitReturnsVFee,CashServiceFee,POSServiceFee,
                            ExtraProtected,
							ExtraVisitReturnsFee,ExtraVisitChangeFee,ExtraReceivePOSFeeRate,ExtraVisitReturnsVFee,ExtraCashServiceFee,ExtraPOSServiceFee,IsCategory,
							ReceiveFeeType,ReceivePOSFeeType,CashServiceType,POSServiceType,WeightType,WeightValueRule,DistributionCode,IsChange,
                            EffectDate,isdeleted)
				   VALUES(:FeeID,:MerchantID,:PaymentType,:PaymentPeriod,:PaymentPeriodDate,:DeliverFeeType,:DeliverFeePeriod,:DeliverFeePeriodDate,:FeeFactors,
						  :IsUniformedFee,:BasicDeliverFee,:RefuseFeeRate,:ReceiveFeeRate,:ExtraRefuseFeeRate,:ExtraReceiveFeeRate,:FormulaID,:FormulaParamters,:UpdateBy,sysdate,:UpdateCode,
                          :AuditBy,sysdate,:AuditCode, :AuditResult,:Status,:FirstWeight,:StatPramer,:AddWeightPrice,:FirstWeightPrice,:VolumeParmer,
							:ProtectedParmer,:VisitReturnsFee,:VisitChangeFee,:ReceivePOSFeeRate,:CreateBy,sysdate,:VisitReturnsVFee,
							:CashServiceFee,:POSServiceFee,
                            :ExtraProtected,:ExtraVisitReturnsFee,:ExtraVisitChangeFee,:ExtraReceivePOSFeeRate,:ExtraVisitReturnsVFee,
							:ExtraCashServiceFee,:ExtraPOSServiceFee,:IsCategory,
                            :ReceiveFeeType,:ReceivePOSFeeType,:CashServiceType,:POSServiceType,
							:WeightType,:WeightValueRule,:DistributionCode,:IsChange,:EffectDate,:isdeleted)";
            IList<OracleParameter> paramList = new List<OracleParameter>();
            model.ID = GetIdNew("SEQ_MERCHANTDELIVERFEEWAIT");
            paramList.Add(new OracleParameter(":FeeID", OracleDbType.Decimal) { Value = model.ID });
            paramList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = model.MerchantID });
            paramList.Add(new OracleParameter(":PaymentType", OracleDbType.Decimal) { Value = (int)model.PaymentType });
            paramList.Add(new OracleParameter(":PaymentPeriod", OracleDbType.Decimal) { Value = model.PaymentPeriod });
            paramList.Add(new OracleParameter(":PaymentPeriodDate", OracleDbType.Date) { Value = model.PaymentPeriodDate });
            paramList.Add(new OracleParameter(":DeliverFeeType", OracleDbType.Decimal) { Value = (int)model.DeliverFeeType });
            paramList.Add(new OracleParameter(":DeliverFeePeriod", OracleDbType.Decimal) { Value = model.DeliverFeePeriod });
            paramList.Add(new OracleParameter(":DeliverFeePeriodDate", OracleDbType.Date) { Value = model.DeliverFeePeriodDate });
            paramList.Add(new OracleParameter(":FeeFactors", OracleDbType.Varchar2, 50) { Value = model.FeeFactors });
            paramList.Add(new OracleParameter(":IsUniformedFee", OracleDbType.Decimal) { Value = model.IsUniformedFee });
            paramList.Add(new OracleParameter(":BasicDeliverFee", OracleDbType.Decimal, 18) { Value = model.BasicDeliverFee });
            paramList.Add(new OracleParameter(":RefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.RefuseFeeRate });
            paramList.Add(new OracleParameter(":ReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceiveFeeRate });
            paramList.Add(new OracleParameter(":ExtraRefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraRefuseFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceiveFeeRate });
            
            paramList.Add(new OracleParameter(":FormulaID", OracleDbType.Decimal) { Value = model.FormulaID });
            paramList.Add(new OracleParameter(":FormulaParamters", OracleDbType.Varchar2, 100) { Value = model.FormulaParamters });
            paramList.Add(new OracleParameter(":UpdateBy", OracleDbType.Decimal) { Value = model.UpdateUser });
            paramList.Add(new OracleParameter(":UpdateCode", OracleDbType.Varchar2, 20) { Value = model.UpdateUserCode });
            paramList.Add(new OracleParameter(":AuditBy", OracleDbType.Decimal) { Value = model.AuditBy });
            paramList.Add(new OracleParameter(":AuditCode", OracleDbType.Varchar2, 20) { Value = model.AuditCode });
            paramList.Add(new OracleParameter(":AuditResult", OracleDbType.Decimal) { Value = model.AuditResult });
            paramList.Add(new OracleParameter(":Status", OracleDbType.Decimal) { Value = (int)model.Status });
            paramList.Add(new OracleParameter(":FirstWeight", OracleDbType.Decimal, 18) { Value = model.FirstWeight });
            paramList.Add(new OracleParameter(":StatPramer", OracleDbType.Decimal, 18) { Value = model.StatPramer });
            paramList.Add(new OracleParameter(":AddWeightPrice", OracleDbType.Decimal, 18) { Value = model.AddWeightPrice });
            paramList.Add(new OracleParameter(":FirstWeightPrice", OracleDbType.Decimal, 18) { Value = model.FirstWeightPrice });
            paramList.Add(new OracleParameter(":VolumeParmer", OracleDbType.Decimal, 18) { Value = model.VolumeParmer });
            paramList.Add(new OracleParameter(":ProtectedParmer", OracleDbType.Decimal, 18) { Value = model.ProtectedParmer });
            paramList.Add(new OracleParameter(":VisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsFeeRate });
            paramList.Add(new OracleParameter(":VisitChangeFee", OracleDbType.Decimal, 18) { Value = model.VisitChangeFeeRate });
            paramList.Add(new OracleParameter(":ReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceivePosFeeRate });
            paramList.Add(new OracleParameter(":VisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsVFeeRate });
            paramList.Add(new OracleParameter(":CashServiceFee", OracleDbType.Decimal, 18) { Value = model.CashServiceFee });
            paramList.Add(new OracleParameter(":POSServiceFee", OracleDbType.Decimal, 18) { Value = model.POSServiceFee });

            paramList.Add(new OracleParameter(":ExtraProtected", OracleDbType.Decimal, 18) { Value = model.ExtraProtected });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitChangeFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitChangeFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceivePosFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsVFeeRate });
            paramList.Add(new OracleParameter(":ExtraCashServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraCashServiceFee });
            paramList.Add(new OracleParameter(":ExtraPOSServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraPOSServiceFee });
            paramList.Add(new OracleParameter("IsCategory",OracleDbType.Int32){Value = model.IsCategory});
            
            paramList.Add(new OracleParameter(":ReceiveFeeType", OracleDbType.Decimal) { Value = model.ReceiveFeeType });
            paramList.Add(new OracleParameter(":ReceivePOSFeeType", OracleDbType.Decimal) { Value = model.ReceivePOSFeeType });
            paramList.Add(new OracleParameter(":CashServiceType", OracleDbType.Decimal) { Value = model.CashServiceType });
            paramList.Add(new OracleParameter(":POSServiceType", OracleDbType.Decimal) { Value = model.POSServiceType });
            paramList.Add(new OracleParameter(":CreateBy", OracleDbType.Decimal) { Value = model.CreateUser });
            paramList.Add(new OracleParameter(":WeightType", OracleDbType.Decimal) { Value = model.WeightType });
            paramList.Add(new OracleParameter(":WeightValueRule", OracleDbType.Decimal) { Value = model.WeightValueRule });
            paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 50) { Value = model.DistributionCode });
            paramList.Add(new OracleParameter(":IsChange", OracleDbType.Int32) { Value = 1 });
            paramList.Add(new OracleParameter(":EffectDate", OracleDbType.Date) { Value = model.EffectDate });
            paramList.Add(new OracleParameter(":isdeleted", OracleDbType.Decimal) { Value = 0 });
            bool flag = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, paramList.ToArray<OracleParameter>()) > 0;
            if (flag)
                return model.ID;
            else
                return -1;
        }

		/// <summary>
		/// 添加商家配送费信息
		/// </summary>
		/// <param name="model">配送费实体</param>
		/// <returns></returns>
		public bool UpdateMerchantDeliverFee(FMS_MerchantDeliverFee model)
		{
			string sqlStr = @"SELECT count(1) FROM FMS_MerchantDeliverFee fm WHERE MerchantID = {0} AND DistributionCode='{1}'";
			sqlStr = String.Format(sqlStr, model.MerchantID, model.DistributionCode);
			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, null);
			if (obj == null || (Convert.ToInt32(obj) == 0))
			{
				return false;
			}
			sqlStr = @"UPDATE FMS_MerchantDeliverFee 
                   SET    MerchantID = :MerchantID,
                          PaymentType = :PaymentType,
                          PaymentPeriod = :PaymentPeriod,
                          PaymentPeriodDate=:PaymentPeriodDate,
                          DeliverFeeType = :DeliverFeeType,
                          DeliverFeePeriod = :DeliverFeePeriod,
                          DeliverFeePeriodDate=:DeliverFeePeriodDate,
                          FeeFactors = :FeeFactors,
                          IsUniformedFee = :IsUniformedFee,
                          BasicDeliverFee = :BasicDeliverFee,
                          FormulaID = :FormulaID,
                          FormulaParamters = :FormulaParamters, 
                          RefuseFeeRate = :RefuseFeeRate, 
                          ReceiveFeeRate = :ReceiveFeeRate, 
                          ExtraRefuseFeeRate = :ExtraRefuseFeeRate, 
                          ExtraReceiveFeeRate = :ExtraReceiveFeeRate,
                          UpdateBy = :UpdateBy,
                          UpdateTime=sysdate,
                          UpdateCode = :UpdateCode,
                          AuditBy = :AuditBy, 
                          AuditTime = sysdate, 
                          AuditCode = :AuditCode, 
                          AuditResult = :AuditResult, 
                          Status = :Status,
                          FirstWeight=:FirstWeight,
                          StatPramer=:StatPramer,
                          AddWeightPrice=:AddWeightPrice,
                          FirstWeightPrice=:FirstWeightPrice,
                          VolumeParmer=:VolumeParmer,
						  ProtectedParmer=:ProtectedParmer,
                          VisitReturnsFee=:VisitReturnsFee,
                          VisitChangeFee=:VisitChangeFee,
						  ReceivePOSFeeRate=:ReceivePOSFeeRate,
                          VisitReturnsVFee=:VisitReturnsVFee,
						  CashServiceFee=:CashServiceFee,
                          POSServiceFee=:POSServiceFee,

                          ExtraProtected=:ExtraProtected,
                          ExtraVisitReturnsFee=:ExtraVisitReturnsFee,
                          ExtraVisitChangeFee=:ExtraVisitChangeFee,
						  ExtraReceivePOSFeeRate=:ExtraReceivePOSFeeRate,
                          ExtraVisitReturnsVFee=:ExtraVisitReturnsVFee,
						  ExtraCashServiceFee=:ExtraCashServiceFee,
                          ExtraPOSServiceFee=:ExtraPOSServiceFee,
                          
                          IsCategory = :IsCategory,

						  ReceiveFeeType=:ReceiveFeeType,
                          ReceivePOSFeeType=:ReceivePOSFeeType,
                          CashServiceType=:CashServiceType,
                          POSServiceType=:POSServiceType,
						  WeightType=:WeightType ,
                          WeightValueRule=:WeightValueRule,
                          IsChange=:IsChange ,
                          DistributionCode=:DistributionCode
                   WHERE  feeID = :feeID";
			IList<OracleParameter> paramList = new List<OracleParameter>();
			paramList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = model.MerchantID });
			paramList.Add(new OracleParameter(":PaymentType", OracleDbType.Decimal) { Value = (int)model.PaymentType });
			paramList.Add(new OracleParameter(":PaymentPeriod", OracleDbType.Decimal) { Value = model.PaymentPeriod });
			paramList.Add(new OracleParameter(":PaymentPeriodDate", OracleDbType.Date) { Value = model.PaymentPeriodDate });
			paramList.Add(new OracleParameter(":DeliverFeeType", OracleDbType.Decimal) { Value = (int)model.DeliverFeeType });
			paramList.Add(new OracleParameter(":DeliverFeePeriod", OracleDbType.Decimal) { Value = model.DeliverFeePeriod });
			paramList.Add(new OracleParameter(":DeliverFeePeriodDate", OracleDbType.Date) { Value = model.DeliverFeePeriodDate });
			paramList.Add(new OracleParameter(":FeeFactors", OracleDbType.Varchar2, 50) { Value = model.FeeFactors });
			paramList.Add(new OracleParameter(":IsUniformedFee", OracleDbType.Decimal) { Value = model.IsUniformedFee });
			paramList.Add(new OracleParameter(":BasicDeliverFee", OracleDbType.Decimal, 18) { Value = model.BasicDeliverFee });
			paramList.Add(new OracleParameter(":RefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.RefuseFeeRate });
			paramList.Add(new OracleParameter(":ReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceiveFeeRate });

            paramList.Add(new OracleParameter(":ExtraRefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraRefuseFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceiveFeeRate });
			
            paramList.Add(new OracleParameter(":FormulaID", OracleDbType.Decimal) { Value = model.FormulaID });
			paramList.Add(new OracleParameter(":FormulaParamters", OracleDbType.Varchar2, 100) { Value = model.FormulaParamters });
			paramList.Add(new OracleParameter(":UpdateBy", OracleDbType.Decimal) { Value = model.UpdateUser });
			paramList.Add(new OracleParameter(":UpdateCode", OracleDbType.Varchar2, 20) { Value = model.UpdateUserCode });
			paramList.Add(new OracleParameter(":AuditBy", OracleDbType.Decimal) { Value = model.AuditBy });
			paramList.Add(new OracleParameter(":AuditCode", OracleDbType.Varchar2, 20) { Value = model.AuditCode });
			paramList.Add(new OracleParameter(":AuditResult", OracleDbType.Decimal) { Value = model.AuditResult });
			paramList.Add(new OracleParameter(":Status", OracleDbType.Decimal) { Value = (int)model.Status });
			paramList.Add(new OracleParameter(":FirstWeight", OracleDbType.Decimal, 18) { Value = model.FirstWeight });
			paramList.Add(new OracleParameter(":StatPramer", OracleDbType.Decimal, 18) { Value = model.StatPramer });
			paramList.Add(new OracleParameter(":AddWeightPrice", OracleDbType.Decimal, 18) { Value = model.AddWeightPrice });
			paramList.Add(new OracleParameter(":FirstWeightPrice", OracleDbType.Decimal, 18) { Value = model.FirstWeightPrice });
			paramList.Add(new OracleParameter(":VolumeParmer", OracleDbType.Decimal, 18) { Value = model.VolumeParmer });
			paramList.Add(new OracleParameter(":ProtectedParmer", OracleDbType.Decimal, 18) { Value = model.ProtectedParmer });
			paramList.Add(new OracleParameter(":VisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsFeeRate });
			paramList.Add(new OracleParameter(":VisitChangeFee", OracleDbType.Decimal, 18) { Value = model.VisitChangeFeeRate });
			paramList.Add(new OracleParameter(":ReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceivePosFeeRate });
			paramList.Add(new OracleParameter(":VisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsVFeeRate });
			paramList.Add(new OracleParameter(":CashServiceFee", OracleDbType.Decimal, 18) { Value = model.CashServiceFee });
			paramList.Add(new OracleParameter(":POSServiceFee", OracleDbType.Decimal, 18) { Value = model.POSServiceFee });

            paramList.Add(new OracleParameter(":ExtraProtected", OracleDbType.Decimal, 18) { Value = model.ExtraProtected });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitChangeFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitChangeFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceivePosFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsVFeeRate });
            paramList.Add(new OracleParameter(":ExtraCashServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraCashServiceFee });
            paramList.Add(new OracleParameter(":ExtraPOSServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraPOSServiceFee });

            paramList.Add(new OracleParameter(":IsCategory",OracleDbType.Int32){Value = model.IsCategory});
			
            paramList.Add(new OracleParameter(":ReceiveFeeType", OracleDbType.Decimal) { Value = model.ReceiveFeeType });
			paramList.Add(new OracleParameter(":ReceivePOSFeeType", OracleDbType.Decimal) { Value = model.ReceivePOSFeeType });
			paramList.Add(new OracleParameter(":CashServiceType", OracleDbType.Decimal) { Value = model.CashServiceType });
			paramList.Add(new OracleParameter(":POSServiceType", OracleDbType.Decimal) { Value = model.POSServiceType });
			paramList.Add(new OracleParameter(":WeightType", OracleDbType.Decimal) { Value = model.WeightType });
			paramList.Add(new OracleParameter(":WeightValueRule", OracleDbType.Decimal) { Value = model.WeightValueRule });
			paramList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 50) { Value = model.DistributionCode });
            paramList.Add(new OracleParameter(":feeID", OracleDbType.Decimal) { Value = model.ID });
			paramList.Add(new OracleParameter(":IsChange", OracleDbType.Int32) { Value = 1 });

			return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, paramList.ToArray<OracleParameter>()) > 0;
		}

        public bool UpdateEffectMerchantDeliverFee(FMS_MerchantDeliverFee model)
        {
            string sqlStr = @"SELECT count(1) FROM FMS_MerchantDeliverFeeWait fmw WHERE fmw.MerchantID = {0} AND fmw.DistributionCode='{1}' and fmw.isdeleted=0 ";
            sqlStr = String.Format(sqlStr, model.MerchantID, model.DistributionCode);
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, null);
            if (obj == null || (Convert.ToInt32(obj) == 0))
            {
                return false;
            }
            sqlStr = @"UPDATE FMS_MerchantDeliverFeeWait 
                   SET    MerchantID = :MerchantID,PaymentType = :PaymentType,PaymentPeriod = :PaymentPeriod,PaymentPeriodDate=:PaymentPeriodDate,
                          DeliverFeeType = :DeliverFeeType,DeliverFeePeriod = :DeliverFeePeriod,DeliverFeePeriodDate=:DeliverFeePeriodDate,
                          FeeFactors = :FeeFactors,IsUniformedFee = :IsUniformedFee,BasicDeliverFee = :BasicDeliverFee,FormulaID = :FormulaID,
                          FormulaParamters = :FormulaParamters, RefuseFeeRate = :RefuseFeeRate, ReceiveFeeRate = :ReceiveFeeRate, ExtraRefuseFeeRate = :ExtraRefuseFeeRate, ExtraReceiveFeeRate = :ExtraReceiveFeeRate,
                          UpdateBy = :UpdateBy,UpdateTime=sysdate,UpdateCode = :UpdateCode,AuditBy = :AuditBy, 
                          AuditTime = sysdate, AuditCode = :AuditCode, AuditResult = :AuditResult, Status = :Status,
                        FirstWeight=:FirstWeight,StatPramer=:StatPramer,AddWeightPrice=:AddWeightPrice,FirstWeightPrice=:FirstWeightPrice,VolumeParmer=:VolumeParmer,
						ProtectedParmer=:ProtectedParmer,VisitReturnsFee=:VisitReturnsFee,VisitChangeFee=:VisitChangeFee,
						ReceivePOSFeeRate=:ReceivePOSFeeRate,VisitReturnsVFee=:VisitReturnsVFee,
						CashServiceFee=:CashServiceFee,POSServiceFee=:POSServiceFee,

                        ExtraProtected=:ExtraProtected,ExtraVisitReturnsFee=:ExtraVisitReturnsFee,ExtraVisitChangeFee=:ExtraVisitChangeFee,
						ExtraReceivePOSFeeRate=:ExtraReceivePOSFeeRate,ExtraVisitReturnsVFee=:ExtraVisitReturnsVFee,
						ExtraCashServiceFee=:ExtraCashServiceFee,ExtraPOSServiceFee=:ExtraPOSServiceFee,
                        IsCategory = :IsCategory,

						ReceiveFeeType=:ReceiveFeeType,ReceivePOSFeeType=:ReceivePOSFeeType,
						CashServiceType=:CashServiceType,POSServiceType=:POSServiceType,WeightType=:WeightType ,
                        WeightValueRule=:WeightValueRule,IsChange=:IsChange,EffectDate=:EffectDate,isdeleted=:isdeleted
                   WHERE  feeID = :feeID";
            IList<OracleParameter> paramList = new List<OracleParameter>();
            paramList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = model.MerchantID });
            paramList.Add(new OracleParameter(":PaymentType", OracleDbType.Decimal) { Value = (int)model.PaymentType });
            paramList.Add(new OracleParameter(":PaymentPeriod", OracleDbType.Decimal) { Value = model.PaymentPeriod });
            paramList.Add(new OracleParameter(":PaymentPeriodDate", OracleDbType.Date) { Value = model.PaymentPeriodDate });
            paramList.Add(new OracleParameter(":DeliverFeeType", OracleDbType.Decimal) { Value = (int)model.DeliverFeeType });
            paramList.Add(new OracleParameter(":DeliverFeePeriod", OracleDbType.Decimal) { Value = model.DeliverFeePeriod });
            paramList.Add(new OracleParameter(":DeliverFeePeriodDate", OracleDbType.Date) { Value = model.DeliverFeePeriodDate });
            paramList.Add(new OracleParameter(":FeeFactors", OracleDbType.Varchar2, 50) { Value = model.FeeFactors });
            paramList.Add(new OracleParameter(":IsUniformedFee", OracleDbType.Decimal) { Value = model.IsUniformedFee });
            paramList.Add(new OracleParameter(":BasicDeliverFee", OracleDbType.Decimal, 18) { Value = model.BasicDeliverFee });
            paramList.Add(new OracleParameter(":RefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.RefuseFeeRate });
            paramList.Add(new OracleParameter(":ReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceiveFeeRate });

            paramList.Add(new OracleParameter(":ExtraRefuseFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraRefuseFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceiveFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceiveFeeRate });
           
            paramList.Add(new OracleParameter(":FormulaID", OracleDbType.Decimal) { Value = model.FormulaID });
            paramList.Add(new OracleParameter(":FormulaParamters", OracleDbType.Varchar2, 100) { Value = model.FormulaParamters });
            paramList.Add(new OracleParameter(":UpdateBy", OracleDbType.Decimal) { Value = model.UpdateUser });
            paramList.Add(new OracleParameter(":UpdateCode", OracleDbType.Varchar2, 20) { Value = model.UpdateUserCode });
            paramList.Add(new OracleParameter(":AuditBy", OracleDbType.Decimal) { Value = model.AuditBy });
            paramList.Add(new OracleParameter(":AuditCode", OracleDbType.Varchar2, 20) { Value = model.AuditCode });
            paramList.Add(new OracleParameter(":AuditResult", OracleDbType.Decimal) { Value = model.AuditResult });
            paramList.Add(new OracleParameter(":Status", OracleDbType.Decimal) { Value = (int)model.Status });
            paramList.Add(new OracleParameter(":FirstWeight", OracleDbType.Decimal, 18) { Value = model.FirstWeight });
            paramList.Add(new OracleParameter(":StatPramer", OracleDbType.Decimal, 18) { Value = model.StatPramer });
            paramList.Add(new OracleParameter(":AddWeightPrice", OracleDbType.Decimal, 18) { Value = model.AddWeightPrice });
            paramList.Add(new OracleParameter(":FirstWeightPrice", OracleDbType.Decimal, 18) { Value = model.FirstWeightPrice });
            paramList.Add(new OracleParameter(":VolumeParmer", OracleDbType.Decimal, 18) { Value = model.VolumeParmer });
            paramList.Add(new OracleParameter(":ProtectedParmer", OracleDbType.Decimal, 18) { Value = model.ProtectedParmer });
            paramList.Add(new OracleParameter(":VisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsFeeRate });
            paramList.Add(new OracleParameter(":VisitChangeFee", OracleDbType.Decimal, 18) { Value = model.VisitChangeFeeRate });
            paramList.Add(new OracleParameter(":ReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ReceivePosFeeRate });
            paramList.Add(new OracleParameter(":VisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.VisitReturnsVFeeRate });
            paramList.Add(new OracleParameter(":CashServiceFee", OracleDbType.Decimal, 18) { Value = model.CashServiceFee });
            paramList.Add(new OracleParameter(":POSServiceFee", OracleDbType.Decimal, 18) { Value = model.POSServiceFee });

            paramList.Add(new OracleParameter(":ExtraProtected", OracleDbType.Decimal, 18) { Value = model.ExtraProtected });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitChangeFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitChangeFeeRate });
            paramList.Add(new OracleParameter(":ExtraReceivePOSFeeRate", OracleDbType.Decimal, 18) { Value = model.ExtraReceivePosFeeRate });
            paramList.Add(new OracleParameter(":ExtraVisitReturnsVFee", OracleDbType.Decimal, 18) { Value = model.ExtraVisitReturnsVFeeRate });
            paramList.Add(new OracleParameter(":ExtraCashServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraCashServiceFee });
            paramList.Add(new OracleParameter(":ExtraPOSServiceFee", OracleDbType.Decimal, 18) { Value = model.ExtraPOSServiceFee });

            paramList.Add(new OracleParameter(":IsCategory",OracleDbType.Int32){Value = model.IsCategory});
            
            paramList.Add(new OracleParameter(":ReceiveFeeType", OracleDbType.Decimal) { Value = model.ReceiveFeeType });
            paramList.Add(new OracleParameter(":ReceivePOSFeeType", OracleDbType.Decimal) { Value = model.ReceivePOSFeeType });
            paramList.Add(new OracleParameter(":CashServiceType", OracleDbType.Decimal) { Value = model.CashServiceType });
            paramList.Add(new OracleParameter(":POSServiceType", OracleDbType.Decimal) { Value = model.POSServiceType });
            paramList.Add(new OracleParameter(":WeightType", OracleDbType.Decimal) { Value = model.WeightType });
            paramList.Add(new OracleParameter(":WeightValueRule", OracleDbType.Decimal) { Value = model.WeightValueRule });
            paramList.Add(new OracleParameter(":feeID", OracleDbType.Decimal) { Value = model.ID });
            paramList.Add(new OracleParameter(":IsChange", OracleDbType.Decimal) { Value = 1 });
            paramList.Add(new OracleParameter(":EffectDate", OracleDbType.Date) { Value = model.EffectDate });
            paramList.Add(new OracleParameter(":isdeleted", OracleDbType.Decimal) { Value = 0 });

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, paramList.ToArray<OracleParameter>()) > 0;
        }

		/// <summary>
		/// 根据商家ID取得货款结算方式
		/// </summary>
		/// <param name="MerchantID"></param>
		/// <returns></returns>
		public Dictionary<SettleAccountType, int> GetProxyCollectWeeklyData(string MerchantID)
		{
			string strSql = string.Format(@"
					SELECT PaymentType ,PaymentPeriod
					FROM FMS_MerchantDeliverFee
					WHERE MerchantID = {0}
					"
				, MerchantID);

			DataTable result = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString()).Tables[0];
			if (result != null
				&& result.Rows.Count > 0)
			{
				Dictionary<SettleAccountType, int> dicData = new Dictionary<SettleAccountType, int>();
				int nPaymentType = int.Parse(result.Rows[0]["PaymentType"].ToString());
				int nPaymentPeriod = DBNull.Value == result.Rows[0]["PaymentPeriod"] ? 0 : int.Parse(result.Rows[0]["PaymentPeriod"].ToString());
				dicData.Add((SettleAccountType)nPaymentType, nPaymentPeriod);
				return dicData;
			}
			return null;
		}

		/// <summary>
		/// 根据条件搜索站点费用
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public DataTable SearchStationDeliverFee(FMS_StationDeliverFee model)
		{
			List<OracleParameter> listParams = new List<OracleParameter>();
			StringBuilder strResult = new StringBuilder();
			DataSet result;

			strResult.AppendFormat(@"
				SELECT  pv.ProvinceName,ct.CityName,ec.CompanyName,ec.ExpressCompanyID
				,NVL(fsd.BasicDeliverFee,0) AS BasicDeliverFee,fsd.UpdateCode,EMA.EmployeeName AS UpdateName,fsd.UpdateTime,fsd.AuditCode
				,EMB.EmployeeName AS AuditName,fsd.AuditTime,fsd.AuditResult,NVL(fsd.Status,0) AS Status
				FROM    
					Province pv 
					JOIN City CT  ON pv.ProvinceID = CT.ProvinceID
					JOIN ExpressCompany EC  ON CT.CityID = EC.CityID
					LEFT JOIN FMS_StationDeliverFee FSD  ON FSD.StationID = EC.ExpressCompanyID {0}
					LEFT JOIN Employee EMA ON FSD.UpdateBy = EMA.EmployeeID  
					LEFT JOIN Employee EMB ON FSD.AuditBy = EMB.EmployeeID                                         
				"
				, model.MerchantID > 0 ? " AND FSD.MerchantID = :MerchatID" : string.Empty);
			if (model.MerchantID > 0)
			{
				listParams.Add(new OracleParameter(":MerchatID", OracleDbType.Decimal) { Value = model.MerchantID });
			}
			strResult.Append(" WHERE   ec.CompanyFlag = 2 ");
			if (!String.IsNullOrEmpty(model.ProvinceID))
			{
				strResult.Append(" AND pv.ProvinceID = :ProvinceID");
				listParams.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 10) { Value = model.ProvinceID });
			}
			if (!String.IsNullOrEmpty(model.CityID))
			{
				strResult.Append(" AND ct.CityID = :CityID");
				listParams.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 10) { Value = model.CityID });
			}
			if (model.StationID > 0)
			{
				strResult.Append(" AND fsd.StationID = :StationID");
				listParams.Add(new OracleParameter(":StationID", OracleDbType.Decimal) { Value = model.StationID });
			}
			if (model.LastStartUpdateTime.HasValue)
			{
				strResult.Append(" AND fsd.UpdateTime >= :StartTime");
				listParams.Add(new OracleParameter(":StartTime", OracleDbType.Date) { Value = model.LastStartUpdateTime.Value });
			}
			if (model.LastEndUpdateTime.HasValue)
			{
				strResult.Append(" AND fsd.UpdateTime < :EndTime");
				listParams.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = model.LastEndUpdateTime.Value });
			}
			if (!model.bChooseAllStatus)
			{
				if (model.MaintainedStatus == MaintainStatus.Maintain)
				{
					string strCommon = strResult.ToString();
					strResult.Insert(0, ";WITH CTE_Result  AS (");
					strResult.Append(" AND fsd.Status IS NULL");
					strResult.Append(" UNION ALL ");
					strResult.Append(strCommon);
					strResult.AppendFormat(" AND fsd.Status = {0}", (int)model.MaintainedStatus);
					strResult.Append(" ) SELECT * FROM CTE_Result ORDER BY ProvinceName , CityName, CompanyName ");

					result = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult.ToString(), listParams.ToArray());
					return result != null ? result.Tables[0] : null;
				}
				else
				{
					strResult.AppendFormat(" AND fsd.Status = {0}", (int)model.MaintainedStatus);
				}
			}
			strResult.Append(" ORDER BY ProvinceName , CityName, CompanyName  ");

			result = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult.ToString(), listParams.ToArray());
			return result != null ? result.Tables[0] : null;
		}

		public bool UpdateMerchantDeliverFeeStatus(int merchantId, int status, int auditBy, string distributionCode, out int id)
		{
			string sql = @"select f.feeID from FMS_MerchantDeliverFee f where f.MerchantId={0} AND f.DistributionCode='{1}'";
			sql = String.Format(sql, merchantId, distributionCode);
			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql, null);
			if (obj == null)
			{
				id = 0;
				return false;
			}

			id = Convert.ToInt32(obj);

			sql = @"UPDATE FMS_MerchantDeliverFee SET Status=:Status,AuditBy=:AuditBy,AuditTime=Sysdate  WHERE MerchantId=:MerchantId AND DistributionCode=:DistributionCode";
			OracleParameter[] parameters ={
										   new OracleParameter(":MerchantId",OracleDbType.Decimal),
										   new OracleParameter(":Status",OracleDbType.Decimal),
										   new OracleParameter(":AuditBy",OracleDbType.Decimal),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2)
									  };

			parameters[0].Value = merchantId;
			parameters[1].Value = status;
			parameters[2].Value = auditBy;
			parameters[3].Value = distributionCode;

			bool flag = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;

			if (!flag)
			{
				id = 0;
			}
			return flag;
		}

        public bool UpdateEffectMerchantDeliverFeeStatus(int merchantId, int status, int auditBy, string distributionCode, out int id)
        {
            string sql = @"select fmw.feeID from FMS_MerchantDeliverFeeWait fmw where fmw.MerchantId={0} AND fmw.DistributionCode='{1}' and fmw.isdeleted=0";
            sql = String.Format(sql, merchantId, distributionCode);
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql, null);
            if (obj == null)
            {
                id = 0;
                return false;
            }

            id = Convert.ToInt32(obj);

            sql = @"UPDATE FMS_MerchantDeliverFeeWait SET Status=:Status,AuditBy=:AuditBy,AuditTime=Sysdate  WHERE FeeID=:FeeID";
            OracleParameter[] parameters ={
										   new OracleParameter(":FeeID",OracleDbType.Decimal),
										   new OracleParameter(":Status",OracleDbType.Decimal),
										   new OracleParameter(":AuditBy",OracleDbType.Decimal),
									  };

            parameters[0].Value = id;
            parameters[1].Value = status;
            parameters[2].Value = auditBy;

            bool flag = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;

            if (!flag)
            {
                id = 0;
            }
            return flag;
        }

        public int GetEffectMerchantDeliverByMerchantID(int merchantId)
        {
            string sql = "select count(1) from fms_merchantdeliverfeewait where merchantid=:merchantid and isdeleted=0";
            OracleParameter[] parameters ={
                                              new OracleParameter(":merchantid",OracleDbType.Decimal),
                                         };
            parameters[0].Value = merchantId;
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(Connection, CommandType.Text, sql, parameters));
        }

        public DataTable GetWaitFeeList()
        {
            string sql = @"
SELECT fmdfw.ISCHANGE,
fmdfw.UPDATETIME,
fmdfw.UPDATECODE,
fmdfw.AUDITBY,
fmdfw.AUDITTIME,
fmdfw.AUDITCODE,
fmdfw.AUDITRESULT,
fmdfw.STATUS,
fmdfw.REFUSEFEERATE,
fmdfw.RECEIVEFEERATE,
fmdfw.PAYMENTPERIODDATE,
fmdfw.DELIVERFEEPERIODDATE,
fmdfw.FIRSTWEIGHT,
fmdfw.STATPRAMER,
fmdfw.ADDWEIGHTPRICE,
fmdfw.FIRSTWEIGHTPRICE,
fmdfw.VOLUMEPARMER,
fmdfw.PROTECTEDPARMER,
fmdfw.VISITRETURNSFEE,
fmdfw.VISITCHANGEFEE,
fmdfw.RECEIVEPOSFEERATE,
fmdfw.CREATEBY,
fmdfw.CREATETIME,
fmdfw.VISITRETURNSVFEE,
fmdfw.CASHSERVICEFEE,
fmdfw.POSSERVICEFEE,
fmdfw.RECEIVEFEETYPE,
fmdfw.RECEIVEPOSFEETYPE,
fmdfw.CASHSERVICETYPE,
fmdfw.POSSERVICETYPE,
fmdfw.WEIGHTTYPE,
fmdfw.WEIGHTVALUERULE,
fmdfw.DISTRIBUTIONCODE,
fmdfw.FEEID as effectid,
fmdfw.MERCHANTID,
fmdfw.PAYMENTTYPE,
fmdfw.PAYMENTPERIOD,
fmdfw.DELIVERFEETYPE,
fmdfw.DELIVERFEEPERIOD,
fmdfw.FEEFACTORS,
fmdfw.ISUNIFORMEDFEE,
fmdfw.BASICDELIVERFEE,
fmdfw.FORMULAID,
fmdfw.FORMULAPARAMTERS,
fmdfw.UPDATEBY,
fmdfw.Effectdate,
fmdfw.EXTRAREFUSEFEERATE,
fmdfw.EXTRARECEIVEFEERATE,
fmdfw.EXTRAPROTECTED,
fmdfw.EXTRAVISITRETURNSFEE,
fmdfw.EXTRAVISITCHANGEFEE,
fmdfw.EXTRARECEIVEPOSFEERATE,
fmdfw.EXTRAVISITRETURNSVFEE,
fmdfw.EXTRACASHSERVICEFEE,
fmdfw.EXTRAPOSSERVICEFEE,
fmdfw.ISCATEGORY,
fmdf.feeid as id
FROM    FMS_MERCHANTDELIVERFEEWait  fmdfw
join FMS_MERCHANTDELIVERFEE fmdf on fmdf.merchantid=fmdfw.merchantid
WHERE   fmdfw.IsDeleted =0 and fmdfw.Status=2 and fmdfw.EffectDate=to_date(to_char(sysdate,'yyyy-mm-dd'),'yyyy-mm-dd')
";
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public bool UpdateToEffect(FMS_MerchantDeliverFee model)
        {
            string sql = @"
UPDATE FMS_MerchantDeliverFee
   SET MerchantID =:MerchantID
      ,PaymentType=:PaymentType
      ,PaymentPeriod=:PaymentPeriod
      ,DeliverFeeType=:DeliverFeeType
      ,DeliverFeePeriod=:DeliverFeePeriod
      ,FeeFactors=:FeeFactors
      ,IsUniformedFee=:IsUniformedFee
      ,BasicDeliverFee=:BasicDeliverFee
      ,FormulaID=:FormulaID
      ,FormulaParamters=:FormulaParamters
      ,UpdateBy=:UpdateBy
      ,UpdateTime=:UpdateTime
      ,UpdateCode=:UpdateCode
      ,AuditBy=:AuditBy
      ,AuditTime=:AuditTime
      ,AuditCode=:AuditCode
      ,AuditResult=:AuditResult
      ,Status=:Status
      ,RefuseFeeRate=:RefuseFeeRate
      ,ReceiveFeeRate=:ReceiveFeeRate
      ,ExtraRefuseFeeRate=:ExtraRefuseFeeRate
      ,ExtraReceiveFeeRate=:ExtraReceiveFeeRate
      ,PaymentPeriodDate=:PaymentPeriodDate
      ,DeliverFeePeriodDate=:DeliverFeePeriodDate
      ,FirstWeight=:FirstWeight
      ,StatPramer=:StatPramer
      ,AddWeightPrice=:AddWeightPrice
      ,FirstWeightPrice=:FirstWeightPrice
      ,VolumeParmer=:VolumeParmer
      ,ProtectedParmer=:ProtectedParmer
      ,VisitReturnsFee=:VisitReturnsFee
      ,VisitChangeFee=:VisitChangeFee
      ,ReceivePOSFeeRate=:ReceivePOSFeeRate
      ,VisitReturnsVFee=:VisitReturnsVFee
      ,CashServiceFee=:CashServiceFee
      ,POSServiceFee=:POSServiceFee
      ,IsCategory = :IsCategory

      ,ExtraProtected=:ExtraProtected
      ,ExtraVisitReturnsFee=:ExtraVisitReturnsFee
      ,ExtraVisitChangeFee=:ExtraVisitChangeFee
      ,ExtraReceivePOSFeeRate=:ExtraReceivePOSFeeRate
      ,ExtraVisitReturnsVFee=:ExtraVisitReturnsVFee
      ,ExtraCashServiceFee=:ExtraCashServiceFee
      ,ExtraPOSServiceFee=:ExtraPOSServiceFee
      ,ReceiveFeeType=:ReceiveFeeType
      ,ReceivePOSFeeType=:ReceivePOSFeeType
      ,CashServiceType=:CashServiceType
      ,POSServiceType=:POSServiceType
      ,WeightType=:WeightType
      ,WeightValueRule=:WeightValueRule
      ,IsChange=1
 WHERE FeeID=:ID
";
            OracleParameter[] parameters ={
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal),
                                            new OracleParameter(":PaymentType",OracleDbType.Decimal),
                                            new OracleParameter(":PaymentPeriod",OracleDbType.Decimal),
                                            new OracleParameter(":DeliverFeeType",OracleDbType.Decimal),
                                            new OracleParameter(":DeliverFeePeriod",OracleDbType.Decimal),
                                            new OracleParameter(":FeeFactors",OracleDbType.Varchar2, 100),
                                            new OracleParameter(":IsUniformedFee",OracleDbType.Decimal),
                                            new OracleParameter(":BasicDeliverFee",OracleDbType.Decimal),
                                            new OracleParameter(":FormulaID",OracleDbType.Decimal),
                                            new OracleParameter(":FormulaParamters",OracleDbType.Varchar2, 200),
                                            new OracleParameter(":UpdateBy",OracleDbType.Decimal),
                                            new OracleParameter(":UpdateTime",OracleDbType.Date),
                                            new OracleParameter(":UpdateCode",OracleDbType.Varchar2, 40),
                                            new OracleParameter(":AuditBy",OracleDbType.Decimal),
                                            new OracleParameter(":AuditTime",OracleDbType.Date),
                                            new OracleParameter(":AuditCode",OracleDbType.Varchar2, 40),
                                            new OracleParameter(":AuditResult",OracleDbType.Decimal),
                                            new OracleParameter(":Status",OracleDbType.Decimal),
                                            new OracleParameter(":RefuseFeeRate",OracleDbType.Decimal),
                                            new OracleParameter(":ReceiveFeeRate",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraRefuseFeeRate",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraReceiveFeeRate",OracleDbType.Decimal),
                                            new OracleParameter(":PaymentPeriodDate",OracleDbType.Date),
                                            new OracleParameter(":DeliverFeePeriodDate",OracleDbType.Date),
                                            new OracleParameter(":FirstWeight",OracleDbType.Decimal),
                                            new OracleParameter(":StatPramer",OracleDbType.Decimal),
                                            new OracleParameter(":AddWeightPrice",OracleDbType.Decimal),
                                            new OracleParameter(":FirstWeightPrice",OracleDbType.Decimal),
                                            new OracleParameter(":VolumeParmer",OracleDbType.Decimal),
                                            new OracleParameter(":ProtectedParmer",OracleDbType.Decimal),
                                            new OracleParameter(":VisitReturnsFee",OracleDbType.Decimal),
                                            new OracleParameter(":VisitChangeFee",OracleDbType.Decimal),
                                            new OracleParameter(":ReceivePOSFeeRate",OracleDbType.Decimal),
                                            new OracleParameter(":VisitReturnsVFee",OracleDbType.Decimal),
                                            new OracleParameter(":CashServiceFee",OracleDbType.Decimal),
                                            new OracleParameter(":POSServiceFee",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraProtected",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraVisitReturnsFee",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraVisitChangeFee",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraReceivePOSFeeRate",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraVisitReturnsVFee",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraCashServiceFee",OracleDbType.Decimal),
                                            new OracleParameter(":ExtraPOSServiceFee",OracleDbType.Decimal),
                                            new OracleParameter(":IsCategory",OracleDbType.Decimal), 
                                            new OracleParameter(":ReceiveFeeType",OracleDbType.Decimal),
                                            new OracleParameter(":ReceivePOSFeeType",OracleDbType.Decimal),
                                            new OracleParameter(":CashServiceType",OracleDbType.Decimal),
                                            new OracleParameter(":POSServiceType",OracleDbType.Decimal),
                                            new OracleParameter(":WeightType",OracleDbType.Decimal),
                                            new OracleParameter(":WeightValueRule",OracleDbType.Decimal),
                                            new OracleParameter(":ID",OracleDbType.Decimal)
                                      };
            parameters[0].Value = model.MerchantID;
            parameters[1].Value = (int)model.PaymentType;
            parameters[2].Value = model.PaymentPeriod;
            parameters[3].Value = (int)model.DeliverFeeType;
            parameters[4].Value = model.DeliverFeePeriod;
            parameters[5].Value = model.FeeFactors;
            parameters[6].Value = model.IsUniformedFee;
            parameters[7].Value = model.BasicDeliverFee;
            parameters[8].Value = model.FormulaID;
            parameters[9].Value = model.FormulaParamters;
            parameters[10].Value = model.UpdateUser;
            parameters[11].Value = model.UpdateTime;
            parameters[12].Value = model.UpdateUserCode;
            parameters[13].Value = model.AuditBy;
            parameters[14].Value = model.AuditTime;
            parameters[15].Value = model.AuditCode;
            parameters[16].Value = (int)model.AuditResult;
            parameters[17].Value = (int)model.Status;
            parameters[18].Value = model.RefuseFeeRate;
            parameters[19].Value = model.ReceiveFeeRate;

            parameters[20].Value = model.ExtraRefuseFeeRate;
            parameters[21].Value = model.ExtraReceiveFeeRate;

            parameters[22].Value = model.PaymentPeriodDate;
            parameters[23].Value = model.DeliverFeePeriodDate;
            parameters[24].Value = model.FirstWeight;
            parameters[25].Value = model.StatPramer;
            parameters[26].Value = model.AddWeightPrice;
            parameters[27].Value = model.FirstWeightPrice;
            parameters[28].Value = model.VolumeParmer;
            parameters[29].Value = model.ProtectedParmer;
            parameters[30].Value = model.VisitReturnsFeeRate;
            parameters[31].Value = model.VisitChangeFeeRate;
            parameters[32].Value = model.ReceivePosFeeRate;
            parameters[33].Value = model.VisitReturnsVFeeRate;
            parameters[34].Value = model.CashServiceFee;
            parameters[35].Value = model.POSServiceFee;

            parameters[36].Value = model.ExtraProtected;
            parameters[37].Value = model.ExtraVisitReturnsFeeRate;
            parameters[38].Value = model.ExtraVisitChangeFeeRate;
            parameters[39].Value = model.ExtraReceivePosFeeRate;
            parameters[40].Value = model.ExtraVisitReturnsVFeeRate;
            parameters[41].Value = model.ExtraCashServiceFee;
            parameters[42].Value = model.ExtraPOSServiceFee;
            parameters[43].Value = model.IsCategory;

            parameters[44].Value = model.ReceiveFeeType;
            parameters[45].Value = model.ReceivePOSFeeType;
            parameters[46].Value = model.CashServiceType;
            parameters[47].Value = model.POSServiceType;
            parameters[48].Value = model.WeightType;
            parameters[49].Value = model.WeightValueRule;
            parameters[50].Value = model.ID;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public bool DeleteWaitMerchantDeliverFee(string feeid)
        {
            string sql = @"
UPDATE FMS_MERCHANTDELIVERFEEWait
   SET IsDeleted=1
,IsChange =1
,UpdateBy=0
,UpdateTime=sysdate 
WHERE feeid=:feeid";
            OracleParameter[] parameters ={
                                              new OracleParameter(":feeid",OracleDbType.Decimal),
                                         };
            parameters[0].Value = feeid;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }
    }
}
