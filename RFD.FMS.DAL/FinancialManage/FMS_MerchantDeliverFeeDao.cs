using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.BasicSetting;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.FinancialManage
{
    public class FMS_MerchantDeliverFeeDao : SqlServerDao, IFMS_MerchantDeliverFeeDao
    {
        public string BuildSearchCondition(SearchCondition condition, string tableAlias, ref List<SqlParameter> parameterList)
        {
            StringBuilder sbWhere = new StringBuilder();
            //拼装查询条件
            if (condition.DistributionCode == "rfd")
            {
                sbWhere.Append(" AND mi.Sources = 2 ");//如风达不要vancl、vjia
            }
            if (condition.ID != 0)
            {
                sbWhere.Append(" AND {0}.feeid = @ID ");
                parameterList.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = condition.ID });
            }
            if (condition.MerchantID != -1)
            {
                sbWhere.Append(" AND mi.ID = @MerchantID ");
                parameterList.Add(new SqlParameter("@MerchantID", SqlDbType.Decimal) { Value = condition.MerchantID });
            }
            if (!String.IsNullOrEmpty(condition.MerchantName))
            {
                sbWhere.Append(" AND mi.MerchantName LIKE @MerchantName ");
                parameterList.Add(new SqlParameter("@MerchantName", SqlDbType.NVarChar, 50) { Value = '%' + condition.MerchantName + '%' });
            }
            if (!String.IsNullOrEmpty(condition.SimpleSpell))
            {
                sbWhere.Append(" AND mi.SimpleSpell LIKE @SimpleSpell ");
                parameterList.Add(new SqlParameter("@SimpleSpell", SqlDbType.NVarChar) { Value = '%' + condition.SimpleSpell + '%' });
            }
            if (!String.IsNullOrEmpty(condition.DistributionCode))
            {
                sbWhere.Append(" AND dmr.DistributionCode=@DistributionCode ");
            }
            if (!String.IsNullOrEmpty(condition.StatusList))
                sbWhere.Append(condition.StatusList.Contains("'0'") ? " AND Isnull({0}.Status,0) IN (" + condition.StatusList.Replace("'", "") + ")" : " AND {0}.Status IN (" + condition.StatusList.Replace("'", "") + ")");
            parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = condition.DistributionCode });

            return string.Format(sbWhere.ToString(), tableAlias);
        }

        public int GetMerchantDeliverFeeStat(SearchCondition condition)
        {
            string sql = @"
			    SELECT  count(1)
                FROM RFD_PMS.dbo.MerchantBaseInfo mi
                JOIN RFD_PMS.dbo.DistributionMerchantRelation dmr ON dmr.MerchantId=mi.ID 
                JOIN RFD_PMS.dbo.DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
                LEFT JOIN FMS_MerchantDeliverFee fm ON mi.ID = fm.MerchantID AND fm.DistributionCode=@DistributionCode
                WHERE mi.IsDeleted = 0 AND dmr.IsDeleted=0 AND d.ISDELETE=0 {0}";
            string sqlWhere = string.Empty;
            List<SqlParameter> paramList = new List<SqlParameter>();
            sqlWhere = BuildSearchCondition(condition, "fm", ref paramList);

            sql = string.Format(sql, sqlWhere);
            object n = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, ToParameters(paramList.ToArray()));
            return DataConvert.ToInt(n, 0);
        }

        /// <summary>
        /// 根据查询条件获取商家配送费列表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetMerchantDeliverFeeList(SearchCondition condition,PageInfo pi)
        {
            string sql= @"
			    with t as(SELECT ROW_NUMBER() OVER ( ORDER BY mi.ID DESC ) AS RowNum,
                        fm.ID,mi.ID MerchantID, MerchantName, SimpleSpell, PaymentType, CAST(PaymentPeriod AS NVARCHAR(20)) PaymentPeriod, PaymentPeriodDate, 
                       DeliverFeeType, CAST(DeliverFeePeriod AS NVARCHAR(20)) DeliverFeePeriod, DeliverFeePeriodDate, FeeFactors,FormulaID,
                       CAST(IsUniformedFee AS NVARCHAR(10)) IsUniformedFee,FormulaParamters, RefuseFeeRate, 
                       ReceiveFeeRate, ExtraRefuseFeeRate, 
                       ExtraReceiveFeeRate, CAST(BasicDeliverFee AS NVARCHAR(20)) BasicDeliverFee,
                       ISNULL([Status],0) [Status],ISNULL([Status],0) [CurrentStatus],
                        fm.FirstWeight,fm.StatPramer,fm.AddWeightPrice,fm.FirstWeightPrice,fm.VolumeParmer,fm.ProtectedParmer,
						fm.VisitReturnsFee,fm.ReceivePOSFeeRate,fm.VisitChangeFee,
						fm.CreateBy,fm.CreateTime,fm.UpdateBy,fm.UpdateTime,fm.AuditBy,fm.AuditTime,fm.VisitReturnsVFee,
						fm.CashServiceFee,fm.POSServiceFee,
                        fm.ExtraProtected,
						fm.ExtraVisitReturnsFee,fm.ExtraReceivePOSFeeRate,fm.ExtraVisitChangeFee,
						fm.ExtraVisitReturnsVFee,
						fm.ExtraCashServiceFee,fm.ExtraPOSServiceFee,            
						ReceiveFeeTypeStr=case fm.ReceiveFeeType when 0 then '周期结' when 1 then '月结' else '' end,
						ReceivePOSFeeTypeStr=case fm.ReceivePOSFeeType when 0 then '周期结' when 1 then '月结' else '' end,
						CashServiceTypeStr=case fm.CashServiceType when 0 then '周期结' when 1 then '月结' else '' end,
                        fm.IsCategory,
						IsCategoryStr=case fm.IsCategory when 0 then '非品类结' when 1 then '品类结' else '' end,
                        POSServiceTypeStr=case fm.POSServiceType when 0 then '周期结' when 1 then '月结' else '' end,
                        fm.WeightType,fm.WeightValueRule,3 AS LogType
				FROM RFD_PMS.dbo.MerchantBaseInfo mi(NOLOCK)
                JOIN RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK) ON dmr.MerchantId=mi.ID 
                JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
				LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fm(NOLOCK) ON mi.ID = fm.MerchantID AND fm.DistributionCode=@DistributionCode
                WHERE mi.IsDeleted = 0 AND dmr.IsDeleted=0 AND d.ISDELETE=0 {0}) select * from t where RowNum between @RowStr and @RowEnd";
            string sqlWhere = string.Empty;
            List<SqlParameter> paramList = new List<SqlParameter>();
            sqlWhere = BuildSearchCondition(condition, "fm", ref paramList);

            List<SqlParameter> parametersTmp = new List<SqlParameter>();
            parametersTmp.Add(new SqlParameter("@RowStr", SqlDbType.Int) { Value = pi.CurrentPageStartRowNum });
            parametersTmp.Add(new SqlParameter("@RowEnd", SqlDbType.Int) { Value = pi.CurrentPageEndRowNum });

            paramList.AddRange(parametersTmp);

            sql = string.Format(sql, sqlWhere);
            var result = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql.ToString(), paramList.ToArray<SqlParameter>());
            return result != null ? result.Tables[0] : null;
        }

        public int GetMerchantDeliverFeeEffectStat(SearchCondition condition)
        {
            throw new Exception("没有实现sql 待生效查询");
        }

        public DataTable GetMerchantDeliverFeeEffectList(SearchCondition condition,PageInfo pi)
        {
            throw new Exception("没有实现sql 待生效查询");
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
                       CAST(ReceiveFeeRate AS NVARCHAR(20)) ReceiveFeeRate,CAST(fm.UpdateBy AS NVARCHAR(20)) UpdateBy,fm.UpdateCode,fm.UpdateTime,
                       CAST(AuditBy AS NVARCHAR(20)) AuditBy,AuditCode,AuditTime,CAST(AuditResult AS NVARCHAR(20)) AuditResult,ISNULL(fm.[Status],0) [Status],ISNULL(fm.[Status],0) [CurrentStatus]
				FROM RFD_PMS.dbo.MerchantBaseInfo mi(NOLOCK)
				LEFT JOIN FMS_MerchantDeliverFeeHistory fm(NOLOCK)
				ON mi.ID = fm.MerchantID
                WHERE 1 = 1 
			");
            //拼装查询条件
            if (condition.MerchantID != -1)
            {
                strSql.Append(" AND fm.MerchantID = @MerchantID ");
            }
            strSql.AppendFormat(" ORDER BY fm.UpdateTime DESC");
            IList<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = condition.MerchantID });
            var result = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>());
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
            strSql.Append(" FROM RFD_PMS.dbo.MerchantBaseInfo(NOLOCK)");
            strSql.Append(" WHERE IsDeleted = 0 ");
            var result = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString());
            return result != null ? result.Tables[0] : null;
        }
        /// <summary>
        /// 添加商家配送费信息
        /// </summary>
        /// <param name="model">配送费实体</param>
        /// <returns></returns>
        public int AddMerchantDeliverFee(FMS_MerchantDeliverFee model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
               IF NOT EXISTS(SELECT 1 FROM RFD_FMS.dbo.FMS_MerchantDeliverFee fm(NOLOCK) WHERE MerchantID = @MerchantID AND DistributionCode=@DistributionCode)
               BEGIN
				   INSERT RFD_FMS.dbo.FMS_MerchantDeliverFee(MerchantID,PaymentType,PaymentPeriod,PaymentPeriodDate,
                          DeliverFeeType,DeliverFeePeriod,DeliverFeePeriodDate,
                          FeeFactors,IsUniformedFee,BasicDeliverFee,RefuseFeeRate,ReceiveFeeRate,ExtraRefuseFeeRate,ExtraReceiveFeeRate,FormulaID,FormulaParamters,UpdateBy,UpdateTime,UpdateCode,
                          AuditBy,AuditTime,AuditCode,AuditResult,[Status],FirstWeight,StatPramer,AddWeightPrice,FirstWeightPrice,VolumeParmer,ProtectedParmer,
							VisitReturnsFee,VisitChangeFee,ReceivePOSFeeRate,CreateBy,CreateTime,VisitReturnsVFee,CashServiceFee,POSServiceFee,
                            ExtraProtected,
							ExtraVisitReturnsFee,ExtraVisitChangeFee,ExtraReceivePOSFeeRate,ExtraVisitReturnsVFee,ExtraCashServiceFee,ExtraPOSServiceFee,IsCategory,
							ReceiveFeeType,ReceivePOSFeeType,CashServiceType,POSServiceType,WeightType,WeightValueRule,DistributionCode,IsChange)
				   VALUES(@MerchantID,@PaymentType,@PaymentPeriod,@PaymentPeriodDate,@DeliverFeeType,@DeliverFeePeriod,@DeliverFeePeriodDate,@FeeFactors,
						  @IsUniformedFee,@BasicDeliverFee,@RefuseFeeRate,@ReceiveFeeRate,@ExtraRefuseFeeRate,@ExtraReceiveFeeRate,@FormulaID,@FormulaParamters,@UpdateBy,getdate(),@UpdateCode,
                          @AuditBy,getdate(),@AuditCode, @AuditResult,@Status,@FirstWeight,@StatPramer,@AddWeightPrice,@FirstWeightPrice,@VolumeParmer,
							@ProtectedParmer,@VisitReturnsFee,@VisitChangeFee,@ReceivePOSFeeRate,@CreateBy,getdate(),@VisitReturnsVFee,
							@CashServiceFee,@POSServiceFee,
                            @ExtraProtected,@ExtraVisitReturnsFee,@ExtraVisitChangeFee,@ExtraReceivePOSFeeRate,@ExtraVisitReturnsVFee,
							@ExtraCashServiceFee,@ExtraPOSServiceFee,@IsCategory,
							@ReceiveFeeType,@ReceivePOSFeeType,@CashServiceType,@POSServiceType,@WeightType,@WeightValueRule,@DistributionCode,@IsChange)
                    ;select @n=@@IDENTITY
               END
                else
               begin
                select @n=0
               end
            ");
            IList<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@n", SqlDbType.Int) { Direction = ParameterDirection.Output });
            paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = model.MerchantID });
            paramList.Add(new SqlParameter("@PaymentType", SqlDbType.Int) { Value = (int)model.PaymentType });
            paramList.Add(new SqlParameter("@PaymentPeriod", SqlDbType.Int) { Value = model.PaymentPeriod });
            paramList.Add(new SqlParameter("@PaymentPeriodDate", SqlDbType.DateTime) { Value = model.PaymentPeriodDate });
            paramList.Add(new SqlParameter("@DeliverFeeType", SqlDbType.Int) { Value = (int)model.DeliverFeeType });
            paramList.Add(new SqlParameter("@DeliverFeePeriod", SqlDbType.Int) { Value = model.DeliverFeePeriod });
            paramList.Add(new SqlParameter("@DeliverFeePeriodDate", SqlDbType.DateTime) { Value = model.DeliverFeePeriodDate });
            paramList.Add(new SqlParameter("@FeeFactors", SqlDbType.VarChar, 50) { Value = model.FeeFactors });
            paramList.Add(new SqlParameter("@IsUniformedFee", SqlDbType.Int) { Value = model.IsUniformedFee });
            paramList.Add(new SqlParameter("@BasicDeliverFee", SqlDbType.Decimal, 18) { Value = model.BasicDeliverFee });
            paramList.Add(new SqlParameter("@RefuseFeeRate", SqlDbType.Decimal, 18) { Value = model.RefuseFeeRate });
            paramList.Add(new SqlParameter("@ReceiveFeeRate", SqlDbType.Decimal, 18) { Value = model.ReceiveFeeRate });

            paramList.Add(new SqlParameter("@ExtraRefuseFeeRate", SqlDbType.Decimal, 18) { Value = model.ExtraRefuseFeeRate });
            paramList.Add(new SqlParameter("@ExtraReceiveFeeRate", SqlDbType.Decimal, 18) { Value = model.ExtraReceiveFeeRate });
            
            paramList.Add(new SqlParameter("@FormulaID", SqlDbType.Int) { Value = model.FormulaID });
            paramList.Add(new SqlParameter("@FormulaParamters", SqlDbType.VarChar, 100) { Value = model.FormulaParamters });
            paramList.Add(new SqlParameter("@UpdateBy", SqlDbType.Int) { Value = model.UpdateUser });
            paramList.Add(new SqlParameter("@UpdateCode", SqlDbType.NVarChar, 20) { Value = model.UpdateUserCode });
            paramList.Add(new SqlParameter("@AuditBy", SqlDbType.Int) { Value = model.AuditBy });
            paramList.Add(new SqlParameter("@AuditCode", SqlDbType.NVarChar, 20) { Value = model.AuditCode });
            paramList.Add(new SqlParameter("@AuditResult", SqlDbType.Int) { Value = model.AuditResult });
            paramList.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = (int)model.Status });
            paramList.Add(new SqlParameter("@FirstWeight", SqlDbType.Decimal, 18) { Value = model.FirstWeight });
            paramList.Add(new SqlParameter("@StatPramer", SqlDbType.Decimal, 18) { Value = model.StatPramer });
            paramList.Add(new SqlParameter("@AddWeightPrice", SqlDbType.Decimal, 18) { Value = model.AddWeightPrice });
            paramList.Add(new SqlParameter("@FirstWeightPrice", SqlDbType.Decimal, 18) { Value = model.FirstWeightPrice });
            paramList.Add(new SqlParameter("@VolumeParmer", SqlDbType.Decimal, 18) { Value = model.VolumeParmer });
            paramList.Add(new SqlParameter("@ProtectedParmer", SqlDbType.Decimal, 18) { Value = model.ProtectedParmer });
            paramList.Add(new SqlParameter("@VisitReturnsFee", SqlDbType.Decimal, 18) { Value = model.VisitReturnsFeeRate });
            paramList.Add(new SqlParameter("@VisitChangeFee", SqlDbType.Decimal, 18) { Value = model.VisitChangeFeeRate });
            paramList.Add(new SqlParameter("@ReceivePOSFeeRate", SqlDbType.Decimal, 18) { Value = model.ReceivePosFeeRate });
            paramList.Add(new SqlParameter("@VisitReturnsVFee", SqlDbType.Decimal, 18) { Value = model.VisitReturnsVFeeRate });
            paramList.Add(new SqlParameter("@CashServiceFee", SqlDbType.Decimal, 18) { Value = model.CashServiceFee });
            paramList.Add(new SqlParameter("@POSServiceFee", SqlDbType.Decimal, 18) { Value = model.POSServiceFee });

            paramList.Add(new SqlParameter("@ExtraProtected", SqlDbType.Decimal, 18) { Value = model.ExtraProtected });
            paramList.Add(new SqlParameter("@ExtraVisitReturnsFee", SqlDbType.Decimal, 18) { Value = model.ExtraVisitReturnsFeeRate });
            paramList.Add(new SqlParameter("@ExtraVisitChangeFee", SqlDbType.Decimal, 18) { Value = model.ExtraVisitChangeFeeRate });
            paramList.Add(new SqlParameter("@ExtraReceivePOSFeeRate", SqlDbType.Decimal, 18) { Value = model.ExtraReceivePosFeeRate });
            paramList.Add(new SqlParameter("@ExtraVisitReturnsVFee", SqlDbType.Decimal, 18) { Value = model.ExtraVisitReturnsVFeeRate });
            paramList.Add(new SqlParameter("@ExtraCashServiceFee", SqlDbType.Decimal, 18) { Value = model.ExtraCashServiceFee });
            paramList.Add(new SqlParameter("@ExtraPOSServiceFee", SqlDbType.Decimal, 18) { Value = model.ExtraPOSServiceFee });

            paramList.Add(new SqlParameter("@IsCategory",SqlDbType.Int){Value = model.IsCategory});
            
            paramList.Add(new SqlParameter("@ReceiveFeeType", SqlDbType.Int) { Value = model.ReceiveFeeType });
            paramList.Add(new SqlParameter("@ReceivePOSFeeType", SqlDbType.Int) { Value = model.ReceivePOSFeeType });
            paramList.Add(new SqlParameter("@CashServiceType", SqlDbType.Int) { Value = model.CashServiceType });
            paramList.Add(new SqlParameter("@POSServiceType", SqlDbType.Int) { Value = model.POSServiceType });
            paramList.Add(new SqlParameter("@CreateBy", SqlDbType.Int) { Value = model.CreateUser });
            paramList.Add(new SqlParameter("@WeightType", SqlDbType.Int) { Value = model.WeightType });
            paramList.Add(new SqlParameter("@WeightValueRule", SqlDbType.Int) { Value = model.WeightValueRule });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = model.DistributionCode });
            paramList.Add(new SqlParameter("@IsChange", SqlDbType.Bit) { Value = true });

            bool flag = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
            if (flag)
                return (int)paramList[0].Value;
            else
                return 0;
        }

        public int AddEffectMerchantDeliverFee(FMS_MerchantDeliverFee model)
        {
            throw new Exception("sql中没有实现待生效");
        }

        /// <summary>
        /// 添加商家配送费信息
        /// </summary>
        /// <param name="model">配送费实体</param>
        /// <returns></returns>
        public bool UpdateMerchantDeliverFee(FMS_MerchantDeliverFee model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
               IF EXISTS(SELECT 1 FROM RFD_FMS.dbo.FMS_MerchantDeliverFee fm(NOLOCK) WHERE MerchantID = @MerchantID AND DistributionCode=@DistributionCode)
               BEGIN
                   UPDATE RFD_FMS.dbo.FMS_MerchantDeliverFee 
                   SET    MerchantID = @MerchantID,PaymentType = @PaymentType,PaymentPeriod = @PaymentPeriod,PaymentPeriodDate=@PaymentPeriodDate,
                          DeliverFeeType = @DeliverFeeType,DeliverFeePeriod = @DeliverFeePeriod,DeliverFeePeriodDate=@DeliverFeePeriodDate,
                          FeeFactors = @FeeFactors,IsUniformedFee = @IsUniformedFee,BasicDeliverFee = @BasicDeliverFee,FormulaID = @FormulaID,
                          FormulaParamters = @FormulaParamters, RefuseFeeRate = @RefuseFeeRate, ReceiveFeeRate = @ReceiveFeeRate, 
                          ExtraRefuseFeeRate = @ExtraRefuseFeeRate, ExtraReceiveFeeRate = @ExtraReceiveFeeRate, 
                          UpdateBy = @UpdateBy,UpdateTime=getdate(),UpdateCode = @UpdateCode,AuditBy = @AuditBy, 
                          AuditTime = getdate(), AuditCode = @AuditCode, AuditResult = @AuditResult, [Status] = @Status,
                        FirstWeight=@FirstWeight,StatPramer=@StatPramer,AddWeightPrice=@AddWeightPrice,FirstWeightPrice=@FirstWeightPrice,VolumeParmer=@VolumeParmer,
						ProtectedParmer=@ProtectedParmer,VisitReturnsFee=@VisitReturnsFee,VisitChangeFee=@VisitChangeFee,
						ReceivePOSFeeRate=@ReceivePOSFeeRate,VisitReturnsVFee=@VisitReturnsVFee,
						CashServiceFee=@CashServiceFee,POSServiceFee=@POSServiceFee,
                        ExtraProtected=@ExtraProtected,ExtraVisitReturnsFee=@ExtraVisitReturnsFee,ExtraVisitChangeFee=@ExtraVisitChangeFee,
						ExtraReceivePOSFeeRate=@ExtraReceivePOSFeeRate,ExtraVisitReturnsVFee=@ExtraVisitReturnsVFee,
						ExtraCashServiceFee=@ExtraCashServiceFee,ExtraPOSServiceFee=@ExtraPOSServiceFee,IsCategory = @IsCategory,
						ReceiveFeeType=@ReceiveFeeType,ReceivePOSFeeType=@ReceivePOSFeeType,
						CashServiceType=@CashServiceType,POSServiceType=@POSServiceType,WeightType=@WeightType ,WeightValueRule=@WeightValueRule,IsChange=@IsChange 
                   WHERE  ID = @ID
               END
            ");
            IList<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@MerchantID", SqlDbType.Int) { Value = model.MerchantID });
            paramList.Add(new SqlParameter("@PaymentType", SqlDbType.Int) { Value = (int)model.PaymentType });
            paramList.Add(new SqlParameter("@PaymentPeriod", SqlDbType.Int) { Value = model.PaymentPeriod });
            paramList.Add(new SqlParameter("@PaymentPeriodDate", SqlDbType.DateTime) { Value = model.PaymentPeriodDate });
            paramList.Add(new SqlParameter("@DeliverFeeType", SqlDbType.Int) { Value = (int)model.DeliverFeeType });
            paramList.Add(new SqlParameter("@DeliverFeePeriod", SqlDbType.Int) { Value = model.DeliverFeePeriod });
            paramList.Add(new SqlParameter("@DeliverFeePeriodDate", SqlDbType.DateTime) { Value = model.DeliverFeePeriodDate });
            paramList.Add(new SqlParameter("@FeeFactors", SqlDbType.VarChar, 50) { Value = model.FeeFactors });
            paramList.Add(new SqlParameter("@IsUniformedFee", SqlDbType.Int) { Value = model.IsUniformedFee });
            paramList.Add(new SqlParameter("@BasicDeliverFee", SqlDbType.Decimal, 18) { Value = model.BasicDeliverFee });
            paramList.Add(new SqlParameter("@RefuseFeeRate", SqlDbType.Decimal, 18) { Value = model.RefuseFeeRate });
            paramList.Add(new SqlParameter("@ReceiveFeeRate", SqlDbType.Decimal, 18) { Value = model.ReceiveFeeRate });

            paramList.Add(new SqlParameter("@ExtraRefuseFeeRate", SqlDbType.Decimal, 18) { Value = model.ExtraRefuseFeeRate });
            paramList.Add(new SqlParameter("@ExtraReceiveFeeRate", SqlDbType.Decimal, 18) { Value = model.ExtraReceiveFeeRate });
            
            paramList.Add(new SqlParameter("@FormulaID", SqlDbType.Int) { Value = model.FormulaID });
            paramList.Add(new SqlParameter("@FormulaParamters", SqlDbType.VarChar, 100) { Value = model.FormulaParamters });
            paramList.Add(new SqlParameter("@UpdateBy", SqlDbType.Int) { Value = model.UpdateUser });
            paramList.Add(new SqlParameter("@UpdateCode", SqlDbType.NVarChar, 20) { Value = model.UpdateUserCode });
            paramList.Add(new SqlParameter("@AuditBy", SqlDbType.Int) { Value = model.AuditBy });
            paramList.Add(new SqlParameter("@AuditCode", SqlDbType.NVarChar, 20) { Value = model.AuditCode });
            paramList.Add(new SqlParameter("@AuditResult", SqlDbType.Int) { Value = model.AuditResult });
            paramList.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = (int)model.Status });
            paramList.Add(new SqlParameter("@FirstWeight", SqlDbType.Decimal, 18) { Value = model.FirstWeight });
            paramList.Add(new SqlParameter("@StatPramer", SqlDbType.Decimal, 18) { Value = model.StatPramer });
            paramList.Add(new SqlParameter("@AddWeightPrice", SqlDbType.Decimal, 18) { Value = model.AddWeightPrice });
            paramList.Add(new SqlParameter("@FirstWeightPrice", SqlDbType.Decimal, 18) { Value = model.FirstWeightPrice });
            paramList.Add(new SqlParameter("@VolumeParmer", SqlDbType.Decimal, 18) { Value = model.VolumeParmer });
            paramList.Add(new SqlParameter("@ProtectedParmer", SqlDbType.Decimal, 18) { Value = model.ProtectedParmer });
            paramList.Add(new SqlParameter("@VisitReturnsFee", SqlDbType.Decimal, 18) { Value = model.VisitReturnsFeeRate });
            paramList.Add(new SqlParameter("@VisitChangeFee", SqlDbType.Decimal, 18) { Value = model.VisitChangeFeeRate });
            paramList.Add(new SqlParameter("@ReceivePOSFeeRate", SqlDbType.Decimal, 18) { Value = model.ReceivePosFeeRate });
            paramList.Add(new SqlParameter("@VisitReturnsVFee", SqlDbType.Decimal, 18) { Value = model.VisitReturnsVFeeRate });
            paramList.Add(new SqlParameter("@CashServiceFee", SqlDbType.Decimal, 18) { Value = model.CashServiceFee });
            paramList.Add(new SqlParameter("@POSServiceFee", SqlDbType.Decimal, 18) { Value = model.POSServiceFee });

            paramList.Add(new SqlParameter("@ExtraProtected", SqlDbType.Decimal, 18) { Value = model.ExtraProtected });
            paramList.Add(new SqlParameter("@ExtraVisitReturnsFee", SqlDbType.Decimal, 18) { Value = model.ExtraVisitReturnsFeeRate });
            paramList.Add(new SqlParameter("@ExtraVisitChangeFee", SqlDbType.Decimal, 18) { Value = model.ExtraVisitChangeFeeRate });
            paramList.Add(new SqlParameter("@ExtraReceivePOSFeeRate", SqlDbType.Decimal, 18) { Value = model.ExtraReceivePosFeeRate });
            paramList.Add(new SqlParameter("@ExtraVisitReturnsVFee", SqlDbType.Decimal, 18) { Value = model.ExtraVisitReturnsVFeeRate });
            paramList.Add(new SqlParameter("@ExtraCashServiceFee", SqlDbType.Decimal, 18) { Value = model.ExtraCashServiceFee });
            paramList.Add(new SqlParameter("@ExtraPOSServiceFee", SqlDbType.Decimal, 18) { Value = model.ExtraPOSServiceFee });
            paramList.Add(new SqlParameter("@IsCategory",SqlDbType.Int){Value = model.IsCategory});


            paramList.Add(new SqlParameter("@ReceiveFeeType", SqlDbType.Int) { Value = model.ReceiveFeeType });
            paramList.Add(new SqlParameter("@ReceivePOSFeeType", SqlDbType.Int) { Value = model.ReceivePOSFeeType });
            paramList.Add(new SqlParameter("@CashServiceType", SqlDbType.Int) { Value = model.CashServiceType });
            paramList.Add(new SqlParameter("@POSServiceType", SqlDbType.Int) { Value = model.POSServiceType });
            paramList.Add(new SqlParameter("@WeightType", SqlDbType.Int) { Value = model.WeightType });
            paramList.Add(new SqlParameter("@WeightValueRule", SqlDbType.Int) { Value = model.WeightValueRule });
            paramList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = model.DistributionCode });
            paramList.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID });
            paramList.Add(new SqlParameter("@IsChange", SqlDbType.Bit) { Value = true });

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), paramList.ToArray<SqlParameter>()) > 0;
        }

        public bool UpdateEffectMerchantDeliverFee(FMS_MerchantDeliverFee model)
        {
            throw new Exception("sql中没有实现待生效");
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
					FROM RFD_FMS.dbo.FMS_MerchantDeliverFee(NOLOCK)
					WHERE MerchantID = {0}
					"
                , MerchantID);

            DataTable result = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql.ToString()).Tables[0];
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
            List<SqlParameter> listParams = new List<SqlParameter>();
            StringBuilder strResult = new StringBuilder();
            DataSet result;

            strResult.AppendFormat(@"
				SELECT  pv.ProvinceName,ct.CityName,ec.CompanyName,ec.ExpressCompanyID
				,ISNULL(fsd.BasicDeliverFee,0) AS [BasicDeliverFee],fsd.UpdateCode,EMA.EmployeeName AS [UpdateName],fsd.UpdateTime,fsd.AuditCode
				,EMB.EmployeeName AS [AuditName],fsd.AuditTime,fsd.AuditResult,ISNULL(fsd.Status,0) AS [Status]
				FROM    
					RFD_PMS.dbo.Province pv (NOLOCK)
					JOIN RFD_PMS.dbo.City CT ( NOLOCK ) ON pv.ProvinceID = CT.ProvinceID
					JOIN RFD_PMS.dbo.ExpressCompany EC ( NOLOCK ) ON CT.CityID = EC.CityID
					LEFT JOIN RFD_FMS.dbo.FMS_StationDeliverFee FSD ( NOLOCK ) ON FSD.StationID = EC.ExpressCompanyID {0}
					LEFT JOIN RFD_PMS.dbo.Employee EMA(NOLOCK) ON FSD.UpdateBy = EMA.EmployeeID  
					LEFT JOIN RFD_PMS.dbo.Employee EMB(NOLOCK) ON FSD.AuditBy = EMB.EmployeeID                                         
				"
                , model.MerchantID > 0 ? " AND FSD.MerchantID = @MerchatID" : string.Empty);
            if (model.MerchantID > 0)
            {
                listParams.Add(new SqlParameter("@MerchatID", SqlDbType.Int) { Value = model.MerchantID });
            }
            strResult.Append(" WHERE   ec.CompanyFlag = 2 ");
            if (!string.IsNullOrEmpty(model.ProvinceID))
            {
                strResult.Append(" AND pv.ProvinceID = @ProvinceID");
                listParams.Add(new SqlParameter("@ProvinceID", SqlDbType.NVarChar, 10) { Value = model.ProvinceID });
            }
            if (!string.IsNullOrEmpty(model.CityID))
            {
                strResult.Append(" AND ct.CityID = @CityID");
                listParams.Add(new SqlParameter("@CityID", SqlDbType.NVarChar, 10) { Value = model.CityID });
            }
            if (model.StationID > 0)
            {
                strResult.Append(" AND fsd.StationID = @StationID");
                listParams.Add(new SqlParameter("@StationID", SqlDbType.Int) { Value = model.StationID });
            }
            if (model.LastStartUpdateTime.HasValue)
            {
                strResult.Append(" AND fsd.UpdateTime >= @StartTime");
                listParams.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = model.LastStartUpdateTime.Value });
            }
            if (model.LastEndUpdateTime.HasValue)
            {
                strResult.Append(" AND fsd.UpdateTime < @EndTime");
                listParams.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = model.LastEndUpdateTime.Value });
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

                    result = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult.ToString(), listParams.ToArray());
                    return result != null ? result.Tables[0] : null;
                }
                else
                {
                    strResult.AppendFormat(" AND fsd.Status = {0}", (int)model.MaintainedStatus);
                }
            }
            strResult.Append(" ORDER BY ProvinceName , CityName, CompanyName  ");

            result = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult.ToString(), listParams.ToArray());
            return result != null ? result.Tables[0] : null;
        }

        public bool UpdateMerchantDeliverFeeStatus(int merchantId, int status, int auditBy, string distributionCode, out int id)
        {
            string sql = @"UPDATE RFD_FMS.dbo.FMS_MerchantDeliverFee SET Status=@Status,AuditBy=@AuditBy,AuditTime=GETDATE(),IsChange=@IsChange WHERE MerchantId=@MerchantId AND DistributionCode=@DistributionCode
                            select @ID=ID from RFD_FMS.dbo.FMS_MerchantDeliverFee f(nolock) where f.MerchantId=@MerchantId AND f.DistributionCode=@DistributionCode
";
            SqlParameter[] parameters ={
										   new SqlParameter("@MerchantId",SqlDbType.Int),
										   new SqlParameter("@Status",SqlDbType.Int),
										   new SqlParameter("@AuditBy",SqlDbType.Int),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar),
                                           new SqlParameter("@ID",SqlDbType.Int),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
									  };

            parameters[0].Value = merchantId;
            parameters[1].Value = status;
            parameters[2].Value = auditBy;
            parameters[3].Value = distributionCode;
            parameters[4].Direction = ParameterDirection.Output;
            parameters[5].Value = true;

            bool flag = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;

            if (!flag)
            {
                id = 0;
            }
            else
            {
                id = (int)parameters[4].Value;
            }

            return flag;
        }

        public bool UpdateEffectMerchantDeliverFeeStatus(int merchantId, int status, int auditBy, string distributionCode, out int id)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public int GetEffectMerchantDeliverByMerchantID(int merchantId)
        {
            throw new Exception("sql中没有实现待生效");
        }

        public DataTable GetWaitFeeList()
        {
            throw new Exception("sql中没有实现待生效");
        }

        public bool UpdateToEffect(FMS_MerchantDeliverFee model)
        {
            string sql = @"
UPDATE FMS_MerchantDeliverFee
   SET MerchantID =@MerchantID
      ,PaymentType=@PaymentType
      ,PaymentPeriod=@PaymentPeriod
      ,DeliverFeeType=@DeliverFeeType
      ,DeliverFeePeriod=@DeliverFeePeriod
      ,FeeFactors=@FeeFactors
      ,IsUniformedFee=@IsUniformedFee
      ,BasicDeliverFee=@BasicDeliverFee
      ,FormulaID=@FormulaID
      ,FormulaParamters=@FormulaParamters
      ,UpdateBy=@UpdateBy
      ,UpdateTime=@UpdateTime
      ,UpdateCode=@UpdateCode
      ,AuditBy=@AuditBy
      ,AuditTime=@AuditTime
      ,AuditCode=@AuditCode
      ,AuditResult=@AuditResult
      ,Status=@Status
      ,RefuseFeeRate=@RefuseFeeRate
      ,ReceiveFeeRate=@ReceiveFeeRate
      ,ExtraRefuseFeeRate=@ExtraRefuseFeeRate
      ,ExtraReceiveFeeRate=@ExtraReceiveFeeRate
      ,PaymentPeriodDate=@PaymentPeriodDate
      ,DeliverFeePeriodDate=@DeliverFeePeriodDate
      ,FirstWeight=@FirstWeight
      ,StatPramer=@StatPramer
      ,AddWeightPrice=@AddWeightPrice
      ,FirstWeightPrice=@FirstWeightPrice
      ,VolumeParmer=@VolumeParmer
      ,ProtectedParmer=@ProtectedParmer
      ,VisitReturnsFee=@VisitReturnsFee
      ,VisitChangeFee=@VisitChangeFee
      ,ReceivePOSFeeRate=@ReceivePOSFeeRate
      ,VisitReturnsVFee=@VisitReturnsVFee
      ,CashServiceFee=@CashServiceFee
      ,POSServiceFee=@POSServiceFee
      ,IsCategory = @IsCategory

      ,ExtraProtected=@ExtraProtected
      ,ExtraVisitReturnsFee=@ExtraVisitReturnsFee
      ,ExtraVisitChangeFee=@ExtraVisitChangeFee
      ,ExtraReceivePOSFeeRate=@ExtraReceivePOSFeeRate
      ,ExtraVisitReturnsVFee=@ExtraVisitReturnsVFee
      ,ExtraCashServiceFee=@ExtraCashServiceFee
      ,ExtraPOSServiceFee=@ExtraPOSServiceFee
      
      ,WeightType=@WeightType
      ,WeightValueRule=@WeightValueRule
      ,IsChange=1
 WHERE ID=@ID
";
            SqlParameter[] parameters ={
                                            new SqlParameter("@MerchantID",SqlDbType.Int),
                                            new SqlParameter("@PaymentType",SqlDbType.Int),
                                            new SqlParameter("@PaymentPeriod",SqlDbType.Int),
                                            new SqlParameter("@DeliverFeeType",SqlDbType.Int),
                                            new SqlParameter("@DeliverFeePeriod",SqlDbType.Int),
                                            new SqlParameter("@FeeFactors",SqlDbType.VarChar,50),
                                            new SqlParameter("@IsUniformedFee",SqlDbType.Bit),
                                            new SqlParameter("@BasicDeliverFee",SqlDbType.Decimal),
                                            new SqlParameter("@FormulaID",SqlDbType.Int),
                                            new SqlParameter("@FormulaParamters",SqlDbType.VarChar,100),
                                            new SqlParameter("@UpdateBy",SqlDbType.Int),
                                            new SqlParameter("@UpdateTime",SqlDbType.DateTime),
                                            new SqlParameter("@UpdateCode",SqlDbType.NVarChar,20),
                                            new SqlParameter("@AuditBy",SqlDbType.Int),
                                            new SqlParameter("@AuditTime",SqlDbType.DateTime),
                                            new SqlParameter("@AuditCode",SqlDbType.NVarChar,20),
                                            new SqlParameter("@AuditResult",SqlDbType.Int),
                                            new SqlParameter("@Status",SqlDbType.Int),
                                            new SqlParameter("@RefuseFeeRate",SqlDbType.Decimal),
                                            new SqlParameter("@ReceiveFeeRate",SqlDbType.Decimal),

                                            new SqlParameter("@ExtraRefuseFeeRate",SqlDbType.Decimal),
                                            new SqlParameter("@ExtraReceiveFeeRate",SqlDbType.Decimal),

                                            new SqlParameter("@PaymentPeriodDate",SqlDbType.DateTime),
                                            new SqlParameter("@DeliverFeePeriodDate",SqlDbType.DateTime),
                                            new SqlParameter("@FirstWeight",SqlDbType.Decimal),
                                            new SqlParameter("@StatPramer",SqlDbType.Decimal),
                                            new SqlParameter("@AddWeightPrice",SqlDbType.Decimal),
                                            new SqlParameter("@FirstWeightPrice",SqlDbType.Decimal),
                                            new SqlParameter("@VolumeParmer",SqlDbType.Decimal),
                                            new SqlParameter("@ProtectedParmer",SqlDbType.Decimal),
                                            new SqlParameter("@VisitReturnsFee",SqlDbType.Decimal),
                                            new SqlParameter("@VisitChangeFee",SqlDbType.Decimal),
                                            new SqlParameter("@ReceivePOSFeeRate",SqlDbType.Decimal),
                                            new SqlParameter("@VisitReturnsVFee",SqlDbType.Decimal),
                                            new SqlParameter("@CashServiceFee",SqlDbType.Decimal),
                                            new SqlParameter("@POSServiceFee",SqlDbType.Decimal),

                                            new SqlParameter("@ExtraProtected",SqlDbType.Decimal),
                                            new SqlParameter("@ExtraVisitReturnsFee",SqlDbType.Decimal),
                                            new SqlParameter("@ExtraVisitChangeFee",SqlDbType.Decimal),
                                            new SqlParameter("@ExtraReceivePOSFeeRate",SqlDbType.Decimal),
                                            new SqlParameter("@ExtraVisitReturnsVFee",SqlDbType.Decimal),
                                            new SqlParameter("@ExtraCashServiceFee",SqlDbType.Decimal),
                                            new SqlParameter("@ExtraPOSServiceFee",SqlDbType.Decimal),
                                            new SqlParameter("@IsCategory",SqlDbType.Int), 

                                            new SqlParameter("@ReceiveFeeType",SqlDbType.Int),
                                            new SqlParameter("@ReceivePOSFeeType",SqlDbType.Int),
                                            new SqlParameter("@CashServiceType",SqlDbType.Int),
                                            new SqlParameter("@POSServiceType",SqlDbType.Int),
                                            new SqlParameter("@WeightType",SqlDbType.Int),
                                            new SqlParameter("@WeightValueRule",SqlDbType.Int),
                                            new SqlParameter("@ID",SqlDbType.Int)
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
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public bool DeleteWaitMerchantDeliverFee(string feeid)
        {
            throw new Exception("sql中没有实现待生效");
        }
    }
}
