using System;
using System.Data;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.Enumeration;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Domain.COD;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Model;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
	/*
    * (C)Copyright 2011-2012 如风达信息管理系统
    * 
    * 模块名称：配送费维护,审核,查询
    * 说明：配送费维护,审核,查询业务逻辑处理
    * 作者：何名宇
    * 创建日期：2011/08/15
    * 修改人：
    * 修改时间：
    * 修改记录：
    */
	public class DeliverFeeService : IDeliverFeeService
	{
        private IFMS_MerchantDeliverFeeDao _fMS_MerchantDeliverFeeDao;
        private IAccountOperatorLogDao _accountOperatorLogDao;

		#region 常量
		private static readonly string SIMPLE_SPELL = "SimpleSpell";
		private static readonly string PAYMENT_TYPE = "PaymentType";
		private static readonly string PAYMENT_PERIOD = "PaymentPeriod";
		private static readonly string DELIVER_FEE_TYPE = "DeliverFeeType";
		private static readonly string DELIVER_FEE_PERIOD = "DeliverFeePeriod";
		private static readonly string IS_UNIFORMED_FEE = "IsUniformedFee";
		private static readonly string BASIC_DELIVER_FEE = "BasicDeliverFee";
		private static readonly string FORMULA = "FormulaID";
		private static readonly string REFUSE_FEE_RATE = "RefuseFeeRate";
		private static readonly string RECEIVE_FEE_RATE = "ReceiveFeeRate";
		private static readonly string FEE_FACTORS = "FeeFactors";
		private static readonly string UPDATE_USER = "UpdateBy";
		private static readonly string UPDATE_USER_CODE = "UpdateCode";
		private static readonly string AUDIT_USER = "AuditBy";
		private static readonly string AUDIT_USER_CODE = "AuditCode";
		private static readonly string AUDIT_RESULT = "AuditResult";
		private static readonly string STATUS = "Status";
        private static readonly string WeightType = "WeightType";
        private static readonly string WeightValueRule = "WeightValueRule";
        private static readonly string WeightTypeStr = "WeightTypeStr";
        private static readonly string WeightValueRuleStr = "WeightValueRuleStr";
		private static readonly string TEMPLATE_FORMAT_ERROR = "上传表格与模板格式不一致！";
		private static readonly string EMPTY_DATA_ERROR = "省份,城市或部门名称为空！";
		private static readonly string DATA_NOEXIST_ERROR = "省份，城市或部门名称不对应！";
		private static readonly string FEE_INVALID_ERROR = "基本配送费不是数字！";
		#endregion

		/// <summary>
		/// 绑定维护状态到控件上
		/// </summary>
		/// <param name="bindControl">绑定控件</param>
		/// <param name="status">当前状态</param>
		public void BindStatus(ListControl bindControl, MaintainStatus status)
		{
			BindStatus(bindControl, status, false, true);
		}
		/// <summary>
		/// 绑定维护状态到控件上
		/// </summary>
		/// <param name="bindControl">绑定控件</param>
		/// <param name="status">当前状态</param>
		/// <param name="IsAddAllOption">是否添加"全部"选项</param>
		/// <param name="IsDefaultSelected">是否设置默认选中</param>
		public void BindStatus(ListControl bindControl, MaintainStatus status, bool IsAddAllOption, bool IsDefaultSelected)
		{
			var statusList = EnumHelper.GetEnumValueAndDescriptions<MaintainStatus>();
			if (statusList != null)
			{
				//添加"全部"选项
				if (IsAddAllOption)
				{
					bindControl.Items.Insert(0, new ListItem("请选择", ""));
				}
				foreach (KeyValuePair<int, string> kvp in statusList)
				{
					bindControl.Items.Add(new ListItem(kvp.Value, kvp.Key.ToString()));
				}
				//设置默认选中
				if (IsDefaultSelected)
				{
					switch (status)
					{
						case MaintainStatus.Maintain:
							bindControl.SelectedIndex = (int)MaintainStatus.Maintain;
							break;
						case MaintainStatus.Auditing:
							bindControl.SelectedIndex = (int)MaintainStatus.Auditing;
							break;
						case MaintainStatus.Audited:
							bindControl.SelectedIndex = (int)MaintainStatus.Audited;
							break;
					}
				}
			}
		}
		/// <summary>
		/// 绑定计费因素
		/// </summary>
		public void BindFactors(ListControl bindControl)
		{
			var factors = EnumHelper.GetEnumValueAndDescriptions<Factors>();
			if (factors != null)
			{
				foreach (KeyValuePair<int, string> kvp in factors)
				{
					bindControl.Items.Add(new ListItem(kvp.Value, kvp.Key.ToString()));
				}
			}
		}
		/// <summary>
		/// 绑定配送费数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable BindDeliverFeeList(SearchCondition condition,ref PageInfo pi)
		{
            DataTable data = new DataTable();
            if (condition.IsEffect)
            {
                int countNum = _fMS_MerchantDeliverFeeDao.GetMerchantDeliverFeeEffectStat(condition);
                if (countNum <= 0)
                    return null;
                pi.ItemCount = countNum;
                data = _fMS_MerchantDeliverFeeDao.GetMerchantDeliverFeeEffectList(condition,pi);
            }
            else
            {
                int countNum = _fMS_MerchantDeliverFeeDao.GetMerchantDeliverFeeStat(condition);
                if (countNum <= 0)
                    return null;
                pi.ItemCount = countNum;
                data = _fMS_MerchantDeliverFeeDao.GetMerchantDeliverFeeList(condition,pi);
            }
            data.Columns.Add("NewProtectedParmer", typeof(string));
            data.Columns.Add("NewRefuseFeeRate", typeof(string));
            data.Columns.Add("NewVisitReturnsFee", typeof(string));
            data.Columns.Add("NewVisitReturnsVFee", typeof(string));
            data.Columns.Add("NewVisitChangeFee", typeof(string));
            data.Columns.Add("NewPOSServiceFee", typeof(string));
            data.Columns.Add("NewCashServiceFee", typeof(string));
            data.Columns.Add("NewReceivePOSFeeRate", typeof(string));
            data.Columns.Add("NewReceiveFeeRate", typeof(string));
			if (condition.IsRawData)
			{
                data.Columns.Add(WeightTypeStr,typeof(string));
                data.Columns.Add(WeightValueRuleStr, typeof(string));
				foreach (DataRow dr in data.Rows)
				{
					//拼音简写大写
					if (!dr[SIMPLE_SPELL].IsNullData())
					{
						dr[SIMPLE_SPELL] = dr[SIMPLE_SPELL].ToString().ToUpper();
					}
					//基本配送费
					if (!dr[IS_UNIFORMED_FEE].IsNullData())
					{
						var uniform = dr[IS_UNIFORMED_FEE].ToString();
						dr[IS_UNIFORMED_FEE] = uniform == "1" ? "一致" : "不一致";
						if (!dr[BASIC_DELIVER_FEE].IsNullData())
						{
							dr[BASIC_DELIVER_FEE] = StringUtil.FormatMoney(dr[BASIC_DELIVER_FEE]);
						}
					}
					FormatDataTable(dr);
                    DealDataTable(dr);
				}
			}
			return data;
		}
		/// <summary>
		/// 绑定配送费历史数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable BindDeliverFeeListHistory(SearchCondition condition)
		{
			var data = _fMS_MerchantDeliverFeeDao.GetMerchantDeliverFeeHistory(condition);
			foreach (DataRow dr in data.Rows)
			{
				//基本配送费
				if (!dr[IS_UNIFORMED_FEE].IsNullData())
				{
					var uniform = dr[IS_UNIFORMED_FEE].ToString();
					dr[IS_UNIFORMED_FEE] = uniform == "1" ? "一致" : "不一致";
					//if (!dr[BASIC_DELIVER_FEE].IsNullData())
					//{
					dr[BASIC_DELIVER_FEE] = !dr[BASIC_DELIVER_FEE].IsNullData() && uniform == "1" ? StringUtil.FormatMoney(dr[BASIC_DELIVER_FEE]) : "查看";
					//}
				}
				//修改人
				if (!dr[UPDATE_USER].IsNullData() && !dr[UPDATE_USER_CODE].IsNullData())
				{
					dr[UPDATE_USER] = GetUserNameById(int.Parse(dr[UPDATE_USER].ToString()), dr[UPDATE_USER_CODE].ToString());
				}
				//审核人
				if (!dr[AUDIT_USER].IsNullData() && !dr[AUDIT_USER_CODE].IsNullData())
				{
					dr[AUDIT_USER] = GetUserNameById(int.Parse(dr[AUDIT_USER].ToString()), dr[AUDIT_USER_CODE].ToString());
				}
				//审核结果
				if (!dr[AUDIT_RESULT].IsNullData())
				{
					var audit = (AuditResult)int.Parse(dr[AUDIT_RESULT].ToString());
					dr[AUDIT_RESULT] = EnumHelper.GetDescription(audit);
				}
				FormatDataTable(dr);
			}
			return data;
		}
		/// <summary>
		/// 将DataTable格式化输出
		/// </summary>
		/// <param name="dr">数据行</param>
		private void FormatDataTable(DataRow dr)
		{
			//货款结算周期
			if (!dr[PAYMENT_TYPE].IsNullData())
			{
				var settleType = (SettleAccountType)int.Parse(dr[PAYMENT_TYPE].ToString());
				switch (settleType)
				{
					case SettleAccountType.ByDay:
						dr[PAYMENT_PERIOD] = EnumHelper.GetDescription(settleType) + dr[PAYMENT_PERIOD].ToString() + "天";
						break;
					case SettleAccountType.ByMonth:
						dr[PAYMENT_PERIOD] = EnumHelper.GetDescription(settleType);
						break;
					default:
						dr[PAYMENT_PERIOD] = String.Empty;
						break;
				}
			}
			//配送费结算周期
			if (!dr[DELIVER_FEE_TYPE].IsNullData())
			{
				var settleType = (SettleAccountType)int.Parse(dr[DELIVER_FEE_TYPE].ToString());
				switch (settleType)
				{
					case SettleAccountType.ByDay:
						dr[DELIVER_FEE_PERIOD] = EnumHelper.GetDescription(settleType) + dr[DELIVER_FEE_PERIOD].ToString() + "天";
						break;
					case SettleAccountType.ByMonth:
						dr[DELIVER_FEE_PERIOD] = EnumHelper.GetDescription(settleType);
						break;
					default:
						dr[DELIVER_FEE_PERIOD] = String.Empty;
						break;
				}
			}
			//计费因素
			if (!dr[FEE_FACTORS].IsNullData())
			{
				var factors = dr[FEE_FACTORS].ToString().Split('+');
				var temp = string.Empty;
				foreach (var factor in factors)
				{
					var f = (Factors)int.Parse(factor);
					temp += String.IsNullOrEmpty(temp) ? EnumHelper.GetDescription(f) : "+" + EnumHelper.GetDescription(f);
				}
				dr[FEE_FACTORS] = temp;
			}
			//计费公式
			if (!dr[FORMULA].IsNullData())
			{
				//...
			}
			////拒收配送费
			//if (!dr[REFUSE_FEE_RATE].IsNullData())
			//{
			//    dr[REFUSE_FEE_RATE] = StringUtil.FormatMoney(dr[REFUSE_FEE_RATE]);
			//}
			////代收手续费
			//if (!dr[RECEIVE_FEE_RATE].IsNullData())
			//{
			//    dr[RECEIVE_FEE_RATE] = StringUtil.FormatMoney(dr[RECEIVE_FEE_RATE]);
			//}
			//维护状态
			if (!dr[STATUS].IsNullData())
			{
				var status = (MaintainStatus)int.Parse(dr[STATUS].ToString());
				dr[STATUS] = EnumHelper.GetDescription(status);
			}
            //商家结算重量取值类型
            if (!dr[WeightType].IsNullData())
            {
                var weightType = (MerchantWeightType)int.Parse(dr[WeightType].ToString());
                dr[WeightTypeStr] = EnumHelper.GetDescription(weightType);
            }
            else
                dr[WeightTypeStr] = "";

            //商家结算重量取值类型 只有取件重量时显示
            if (!dr[WeightValueRule].IsNullData() && dr[WeightType].ToString().TryGetInt() != (int)MerchantWeightType.W5)
            {
                var weightValueRule = (MerchantWeightValueRule)int.Parse(dr[WeightValueRule].ToString());
                dr[WeightValueRuleStr] = EnumHelper.GetDescription(weightValueRule);
            }
            else
                dr[WeightValueRuleStr] = "";
		}

        public void DealDataTable(DataRow dr)
        {
            if (!dr["ProtectedParmer"].IsNullData())
            {
                dr["NewProtectedParmer"] = DataConvert.ToDecimal(dr["ProtectedParmer"],0) + "+" + DataConvert.ToDecimal(dr["ExtraProtected"],0);
            }
            if (!dr["RefuseFeeRate"].IsNullData())
            {
                dr["NewRefuseFeeRate"] = DataConvert.ToDecimal(dr["RefuseFeeRate"],0) + "+" + DataConvert.ToDecimal(dr["ExtraRefuseFeeRate"],0);
            }
            if (!dr["VisitReturnsFee"].IsNullData())
            {
                dr["NewVisitReturnsFee"] = DataConvert.ToDecimal(dr["VisitReturnsFee"],0) + "+" + DataConvert.ToDecimal(dr["ExtraVisitReturnsFee"],0);
            }
            if (!dr["VisitReturnsVFee"].IsNullData())
            {
                dr["NewVisitReturnsVFee"] = DataConvert.ToDecimal(dr["VisitReturnsVFee"],0) + "+" + DataConvert.ToDecimal(dr["ExtraVisitReturnsVFee"],0);
            }
            if (!dr["VisitChangeFee"].IsNullData())
            {
                dr["NewVisitChangeFee"] = DataConvert.ToDecimal(dr["VisitChangeFee"],0) + "+" + DataConvert.ToDecimal(dr["ExtraVisitChangeFee"],0);
            }
            if (!dr["POSServiceFee"].IsNullData())
            {
                dr["NewPOSServiceFee"] = DataConvert.ToDecimal(dr["POSServiceFee"],0) + "+" + DataConvert.ToDecimal(dr["ExtraPOSServiceFee"],0);
            }
            if (!dr["ReceiveFeeRate"].IsNullData())
            {
                dr["NewReceiveFeeRate"] = DataConvert.ToDecimal(dr["ReceiveFeeRate"],0) + "+" + DataConvert.ToDecimal(dr["ExtraReceiveFeeRate"],0);
            }
            if (!dr["ReceivePOSFeeRate"].IsNullData())
            {
                dr["NewReceivePOSFeeRate"] = DataConvert.ToDecimal(dr["ReceivePOSFeeRate"],0) + "+" + DataConvert.ToDecimal(dr["ExtraReceivePOSFeeRate"],0);
            }
            if (!dr["CashServiceFee"].IsNullData())
            {
                dr["NewCashServiceFee"] = DataConvert.ToDecimal(dr["CashServiceFee"],0) + "+" + DataConvert.ToDecimal(dr["ExtraCashServiceFee"],0);
            }
        }

		//根据用户ID和用户编号获取用户名
		private string GetUserNameById(int userID, string userCode)
		{
            var userDao = ServiceLocator.GetService <IUserDao>();
            if (userDao.Exists(userCode))
			{
                return userDao.GetModel(userID).EmployeeName;
			}
			return string.Empty;
		}
		/// <summary>
		/// 保存配送费信息
		/// </summary>
		/// <param name="merchant">商家</param>
		public bool SaveDeliverFee(FMS_MerchantDeliverFee merchant)
		{
			using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
			{
                int id = _fMS_MerchantDeliverFeeDao.AddMerchantDeliverFee(merchant);
                if (id == 0) throw new Exception("已经存在，执行更新即可");
                if (id == -1) return false;
			    
				string logText = "新增，未审核(货结周期：" + merchant.PaymentType
					+ " " + merchant.PaymentPeriod + "天 开始日期" + merchant.PaymentPeriodDate
					+ ",配结周期：" + merchant.DeliverFeeType + " " + merchant.DeliverFeePeriod + "天 开始日期" + merchant.DeliverFeePeriodDate
					+ ",体积参数：" + merchant.VolumeParmer + ",保价参数：" + merchant.ProtectedParmer +"("+merchant.ExtraProtected+") ,拒收参数：" + merchant.RefuseFeeRate +"("+merchant.ExtraRefuseFeeRate+ ")"
					+ ",上门退参数：" + merchant.VisitReturnsFeeRate+"("+merchant.ExtraVisitReturnsFeeRate+"),上门退拒收参数：" + merchant.VisitReturnsVFeeRate+ "("+merchant.ExtraVisitReturnsFeeRate+")"
					+ ",上门换参数：" + merchant.VisitChangeFeeRate +"("+merchant.ExtraVisitChangeFeeRate+")"
                    
					+ ",代收现金手续参数：" + merchant.ReceiveFeeRate+"("+merchant.ExtraReceiveFeeRate+")" + "(" +(merchant.ReceiveFeeType== 0 ? "周期结":"月结")+ ")"
                    + ",代收POS手续参数：" + merchant.ReceivePosFeeRate + "(" + merchant.ExtraReceivePosFeeRate + ")" + "(" + (merchant.ReceivePOSFeeType == 0 ? "周期结" : "月结") + ")"
                    + ",代收现金服务参数：" + merchant.CashServiceFee + "(" + merchant.ExtraCashServiceFee + ")" + "(" + (merchant.CashServiceType == 0 ? "周期结" : "月结") + ")"
                    + ",代收POS服务参数：" + merchant.POSServiceFee + "(" + merchant.ExtraPOSServiceFee + ")" + "(" + (merchant.POSServiceType == 0 ? "周期结" : "月结") + ")"
				    + ",是否按品类结算："+(merchant.IsCategory == 0 ? "是":"否")+ ")";
                if (!_accountOperatorLogDao.AddOperatorLogLog(id.ToString(),
					int.Parse(merchant.CreateUser.ToString()), logText, 3)) 
					return false;
				scope.Complete();
				return true;
			}
		}

        /// <summary>
        /// 添加待生效
        /// </summary>
        /// <param name="merchant"></param>
        /// <returns></returns>
        public bool SaveEffectDeliverFee(RFD.FMS.MODEL.FMS_MerchantDeliverFee merchant)
        {
            int sqlId = 0;
            string sqlText = string.Empty;
            using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {
                int id = _fMS_MerchantDeliverFeeDao.AddEffectMerchantDeliverFee(merchant);
                if (id == 0) throw new Exception("已经存在待生效，执行更新待生效即可");
                if (id == -1) return false;
                string logText = "新增待生效，未审核(货结周期：" + merchant.PaymentType
                    + " " + merchant.PaymentPeriod + "天 开始日期" + merchant.PaymentPeriodDate
                    + ",配结周期：" + merchant.DeliverFeeType + " " + merchant.DeliverFeePeriod + "天 开始日期" + merchant.DeliverFeePeriodDate
                    + ",体积参数：" + merchant.VolumeParmer + ",保价参数：" + merchant.ProtectedParmer + "(" + merchant.ExtraProtected + ") ,拒收参数：" + merchant.RefuseFeeRate + "(" + merchant.ExtraRefuseFeeRate + ")"
                    + ",上门退参数：" + merchant.VisitReturnsFeeRate + "(" + merchant.ExtraVisitReturnsFeeRate + "),上门退拒收参数：" + merchant.VisitReturnsVFeeRate + "(" + merchant.ExtraVisitReturnsFeeRate + ")"
                    + ",上门换参数：" + merchant.VisitChangeFeeRate + "(" + merchant.ExtraVisitChangeFeeRate + ")"

                    + ",代收现金手续参数：" + merchant.ReceiveFeeRate + "(" + merchant.ExtraReceiveFeeRate + ")" + "(" + (merchant.ReceiveFeeType == 0 ? "周期结" : "月结") + ")"
                    + ",代收POS手续参数：" + merchant.ReceivePosFeeRate + "(" + merchant.ExtraReceivePosFeeRate + ")" + "(" + (merchant.ReceivePOSFeeType == 0 ? "周期结" : "月结") + ")"
                    + ",代收现金服务参数：" + merchant.CashServiceFee + "(" + merchant.ExtraCashServiceFee + ")" + "(" + (merchant.CashServiceType == 0 ? "周期结" : "月结") + ")"
                    + ",代收POS服务参数：" + merchant.POSServiceFee + "(" + merchant.ExtraPOSServiceFee + ")" + "(" + (merchant.POSServiceType == 0 ? "周期结" : "月结") + ")"
                    + ",是否按品类结算：" + (merchant.IsCategory == 0 ? "是" : "否") 
                    + ",待生效时间：" + merchant.EffectDate
                            + ")";
                sqlId = id;
                sqlText = logText;
                if (!_accountOperatorLogDao.AddOperatorLogLog(id.ToString(),
                    int.Parse(merchant.CreateUser.ToString()), logText, 4))
                    return false;
                scope.Complete();
                //return true;
            }



            return true;
        }

		/// <summary>
		/// 更新配送费信息
		/// </summary>
		/// <param name="merchant">商家</param>
		public bool UpdateDeliverFee(FMS_MerchantDeliverFee merchant)
		{
			using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
			{
				if (!_fMS_MerchantDeliverFeeDao.UpdateMerchantDeliverFee(merchant)) return false;
				string logText= "更新，未审核(货结周期：" + merchant.PaymentType
                    + " " + merchant.PaymentPeriod + "天 开始日期" + merchant.PaymentPeriodDate
                    + ",配结周期：" + merchant.DeliverFeeType + " " + merchant.DeliverFeePeriod + "天 开始日期" + merchant.DeliverFeePeriodDate
                    + ",体积参数：" + merchant.VolumeParmer + ",保价参数：" + merchant.ProtectedParmer + "(" + merchant.ExtraProtected + ") ,拒收参数：" + merchant.RefuseFeeRate + "(" + merchant.ExtraRefuseFeeRate + ")"
                    + ",上门退参数：" + merchant.VisitReturnsFeeRate + "(" + merchant.ExtraVisitReturnsFeeRate + "),上门退拒收参数：" + merchant.VisitReturnsVFeeRate + "(" + merchant.ExtraVisitReturnsFeeRate + ")"
                    + ",上门换参数：" + merchant.VisitChangeFeeRate + "(" + merchant.ExtraVisitChangeFeeRate + ")"

                    + ",代收现金手续参数：" + merchant.ReceiveFeeRate + "(" + merchant.ExtraReceiveFeeRate + ")" + "(" + (merchant.ReceiveFeeType == 0 ? "周期结" : "月结") + ")"
                    + ",代收POS手续参数：" + merchant.ReceivePosFeeRate + "(" + merchant.ExtraReceivePosFeeRate + ")" + "(" + (merchant.ReceivePOSFeeType == 0 ? "周期结" : "月结") + ")"
                    + ",代收现金服务参数：" + merchant.CashServiceFee + "(" + merchant.ExtraCashServiceFee + ")" + "(" + (merchant.CashServiceType == 0 ? "周期结" : "月结") + ")"
                    + ",代收POS服务参数：" + merchant.POSServiceFee + "(" + merchant.ExtraPOSServiceFee + ")" + "(" + (merchant.POSServiceType == 0 ? "周期结" : "月结") + ")"
                    + ",是否按品类结算：" + (merchant.IsCategory == 0 ? "是" : "否") + ")";

                if (!_accountOperatorLogDao.AddOperatorLogLog(merchant.ID.ToString(), int.Parse(merchant.UpdateUser.ToString()), logText, 3))
					return false;
				scope.Complete();
				return true;
			}
		}

        /// <summary>
        /// 更新配送费信息
        /// </summary>
        /// <param name="merchant">商家</param>
        public bool UpdateEffectDeliverFee(FMS_MerchantDeliverFee merchant)
        {
            string sqlText = string.Empty;
            using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {
                if (!_fMS_MerchantDeliverFeeDao.UpdateEffectMerchantDeliverFee(merchant)) return false;
                string logText = "更新待生效，未审核(货结周期：" + merchant.PaymentType
                    + " " + merchant.PaymentPeriod + "天 开始日期" + merchant.PaymentPeriodDate
                    + ",配结周期：" + merchant.DeliverFeeType + " " + merchant.DeliverFeePeriod + "天 开始日期" + merchant.DeliverFeePeriodDate
                    + ",体积参数：" + merchant.VolumeParmer + ",保价参数：" + merchant.ProtectedParmer + "(" + merchant.ExtraProtected + ") ,拒收参数：" + merchant.RefuseFeeRate + "(" + merchant.ExtraRefuseFeeRate + ")"
                    + ",上门退参数：" + merchant.VisitReturnsFeeRate + "(" + merchant.ExtraVisitReturnsFeeRate + "),上门退拒收参数：" + merchant.VisitReturnsVFeeRate + "(" + merchant.ExtraVisitReturnsFeeRate + ")"
                    + ",上门换参数：" + merchant.VisitChangeFeeRate + "(" + merchant.ExtraVisitChangeFeeRate + ")"

                    + ",代收现金手续参数：" + merchant.ReceiveFeeRate + "(" + merchant.ExtraReceiveFeeRate + ")" + "(" + (merchant.ReceiveFeeType == 0 ? "周期结" : "月结") + ")"
                    + ",代收POS手续参数：" + merchant.ReceivePosFeeRate + "(" + merchant.ExtraReceivePosFeeRate + ")" + "(" + (merchant.ReceivePOSFeeType == 0 ? "周期结" : "月结") + ")"
                    + ",代收现金服务参数：" + merchant.CashServiceFee + "(" + merchant.ExtraCashServiceFee + ")" + "(" + (merchant.CashServiceType == 0 ? "周期结" : "月结") + ")"
                    + ",代收POS服务参数：" + merchant.POSServiceFee + "(" + merchant.ExtraPOSServiceFee + ")" + "(" + (merchant.POSServiceType == 0 ? "周期结" : "月结") + ")"
                    + ",是否按品类结算：" + (merchant.IsCategory == 0 ? "是" : "否") 
                    + ",待生效时间：" + merchant.EffectDate
                            + ")";
                sqlText = logText;
                if (!_accountOperatorLogDao.AddOperatorLogLog(merchant.ID.ToString(), int.Parse(merchant.UpdateUser.ToString()), logText, 4))
                    return false;
                scope.Complete();
            }

            return true;
        }

		/// <summary>
		/// 获取商家字典
		/// </summary>
		/// <returns></returns>
		public IDictionary<int, string> GetMerchantDictionary()
		{
			var dict = new Dictionary<int, string>();
			var data = _fMS_MerchantDeliverFeeDao.GetAllMerchantList();
			if (data == null || data.Rows.Count == 0) return null;
			foreach (DataRow dr in data.Rows)
			{
				dict.Add(int.Parse(dr["ID"].ToString()), dr["MerchantName"].ToString());
			}
			return dict;
		}
		/// <summary>
		/// 从商家字典中获取商家名称
		/// </summary>
		/// <param name="merchantID">商家编号</param>
		/// <returns></returns>
		public string GetMerchantNameFromDictionary(int merchantID)
		{
			var dictionary = GetMerchantDictionary();
			if (!dictionary.Keys.Contains(merchantID)) return string.Empty;
			return dictionary[merchantID];
		}
		/// <summary>
		/// 检查导入数据的合法性
		/// </summary>
		/// <param name="uploadData">上传的数据</param>
		/// <param name="template">模板</param>
		/// <param name="error">错误信息</param>
		/// <returns></returns>
		public bool CheckImportData(DataTable uploadData, DataTable template, out string error)
		{
			error = string.Empty;
			if (uploadData != null && template != null)
			{
				if (uploadData.Columns.Count != template.Columns.Count)
				{
					error = TEMPLATE_FORMAT_ERROR;
					return false;
				}
				uploadData.Columns.Add("StationID", typeof(int));//增加部门编号列
				for (var i = 0; i < uploadData.Rows.Count; i++)
				{
					var dr = uploadData.Rows[i];
					var stationID = 0;
					if (dr[0].IsNullData() ||
						dr[1].IsNullData() ||
						dr[2].IsNullData() ||
						dr[3].IsNullData())
					{
						error = String.Format("第{0}行" + EMPTY_DATA_ERROR, i);
						return false;
					}
					if (!VerifyData(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), out stationID))
					{
						error = String.Format("第{0}行" + DATA_NOEXIST_ERROR, i);
						return false;
					}
					if (!StringUtil.IsNumeric(dr[3].ToString()))
					{
						error = String.Format("第{0}行" + FEE_INVALID_ERROR, i);
						return false;
					}
					dr["StationID"] = stationID;//将当前站点编号保存起来
				}
			}
			return true;
		}

		/// <summary>
		/// 校验省/市/部门
		/// </summary>
		/// <param name="province">省</param>
		/// <param name="city">市</param>
		/// <param name="station">站点</param>
		/// <param name="stationID">站点编号</param>
		/// <returns></returns>
		private bool VerifyData(string province, string city, string station, out int stationID)
		{
			stationID = 0;
			//校验省
            var provinceDao = ServiceLocator.GetService <IProvinceDao>();
			var provinceId = provinceDao.GetProvinceID(province);
			if (String.IsNullOrEmpty(province) || String.IsNullOrEmpty(provinceId))
			{
				return false;
			}
			//校验市
			var cityDao = ServiceLocator.GetService<ICityDao>();
			var dtCity = cityDao.GetCityList(new City() { CityName = city, ProvinceID = provinceId });
			if (dtCity == null || dtCity.Rows.Count == 0)
			{
				return false;
			}
			//校验站点
			var cityId = dtCity.Rows[0]["CityID"].ToString();
            var stationDao = ServiceLocator.GetService <IExpressCompanyDao>();
			var dtStation = stationDao.GetExpressCompanyList(new ExpressCompany() { ProvinceID = provinceId, CityID = cityId, CompanyName = station, CompanyFlag = 2 });
			if (dtStation == null || dtStation.Tables[0].Rows.Count == 0)
			{
				return false;
			}
			else
			{
				//将当前的站点ID输出
				stationID = int.Parse(dtStation.Tables[0].Rows[0]["序号"].ToString());
			}
			return true;
		}

		/// <summary>
		/// 根据条件搜索站点费用
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public DataTable SearchStationDeliverFee(FMS_StationDeliverFee model)
		{
			return _fMS_MerchantDeliverFeeDao.SearchStationDeliverFee(model);
		}

		public bool UpdateMerchantDeliverFeeStatus(IList<KeyValuePair<string, string>> checkList,int status,int auditBy,string distributionCode)
		{
			using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
			{
				foreach (KeyValuePair<string, string> k in checkList)
				{
                    int id = 0;
                    if (!_fMS_MerchantDeliverFeeDao.UpdateMerchantDeliverFeeStatus(int.Parse(k.Key), status, auditBy, distributionCode,out id)) return false;
					string logText = status == 2 ? "已审核" : status == 3 ? "已置回" : "";
                    if (!_accountOperatorLogDao.AddOperatorLogLog(id.ToString(), auditBy, logText, 3))
						return false;
				}
				work.Complete();
				return true;
			}
		}

        public bool UpdateEffectMerchantDeliverFeeStatus(IList<KeyValuePair<string, string>> checkList, int status, int auditBy, string distributionCode)
        {
            string sqlText = string.Empty;
            List<int> ids=new List<int>();
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                foreach (KeyValuePair<string, string> k in checkList)
                {
                    int id = 0;
                    if (!_fMS_MerchantDeliverFeeDao.UpdateEffectMerchantDeliverFeeStatus(int.Parse(k.Key), status, auditBy, distributionCode, out id)) return false;
                    ids.Add(id);
                    id = 0;
                    string logText = status == 2 ? "已审核" : status == 3 ? "已置回" : "";
                    sqlText = logText;
                    if (!_accountOperatorLogDao.AddOperatorLogLog(id.ToString(), auditBy, logText, 4))
                        return false;
                }
                work.Complete();
            }
            return true;
        }

        public int GetEffectMerchantDeliverByMerchantID(int merchantId)
        {
            return _fMS_MerchantDeliverFeeDao.GetEffectMerchantDeliverByMerchantID(merchantId);
        }

        #region 生效服务
        public DataTable GetWaitFeeList()
        {
            return _fMS_MerchantDeliverFeeDao.GetWaitFeeList();
        }

        public bool UpdateToEffect(DataRow dr)
        {
            FMS_MerchantDeliverFee model = new FMS_MerchantDeliverFee();

            model.ID = DataConvert.ToInt(dr["ID"].ToString());
            model.MerchantID = DataConvert.ToInt(dr["MerchantID"].ToString());
            model.BasicDeliverFee = DataConvert.ToDecimal(dr["BasicDeliverFee"].ToString(), -1M);
            model.PaymentType = (SettleAccountType)DataConvert.ToInt(dr["PaymentType"].ToString());
            model.PaymentPeriod = DataConvert.ToInt(dr["PaymentPeriod"].ToString());
            model.PaymentPeriodDate = DataConvert.ToDateTime(dr["PaymentPeriodDate"].ToString());
            model.DeliverFeeType = (SettleAccountType)DataConvert.ToInt(dr["DeliverFeeType"].ToString());
            model.DeliverFeePeriod = DataConvert.ToInt(dr["DeliverFeePeriod"], 0);
            model.DeliverFeePeriodDate = DataConvert.ToDateTime(dr["DeliverFeePeriodDate"].ToString());
            model.FeeFactors = dr["FeeFactors"].ToString();
            model.IsUniformedFee = DataConvert.ToBoolean(dr["IsUniformedFee"].ToString(), false);
            model.RefuseFeeRate = DataConvert.ToDecimal(dr["RefuseFeeRate"].ToString());
            model.VisitReturnsFeeRate = DataConvert.ToDecimal(dr["VisitReturnsFee"].ToString());
            model.VisitReturnsVFeeRate = DataConvert.ToDecimal(dr["VisitReturnsVFee"].ToString());
            model.VisitChangeFeeRate = DataConvert.ToDecimal(dr["VISITCHANGEFEE"].ToString());
            model.ReceiveFeeType = DataConvert.ToInt(dr["ReceiveFeeType"].ToString());
            model.ReceiveFeeRate = DataConvert.ToDecimal(dr["ReceiveFeeRate"].ToString());
            model.CashServiceType = DataConvert.ToInt(dr["CashServiceType"].ToString());
            model.CashServiceFee = DataConvert.ToDecimal(dr["CashServiceFee"].ToString());
            model.ReceivePOSFeeType = DataConvert.ToInt(dr["ReceivePOSFeeType"].ToString());
            model.ReceivePosFeeRate = DataConvert.ToDecimal(dr["ReceivePosFeeRate"].ToString());
            model.POSServiceType = DataConvert.ToInt(dr["POSServiceType"].ToString());
            model.POSServiceFee = DataConvert.ToDecimal(dr["POSServiceFee"].ToString());
            model.FormulaID = DataConvert.ToInt(dr["FormulaID"].ToString(), 0);
            model.FormulaParamters = dr["FormulaParamters"].ToString();
            model.FirstWeight = DataConvert.ToDecimal(dr["FirstWeight"].ToString());
            model.StatPramer = DataConvert.ToDecimal(dr["StatPramer"].ToString());
            model.AddWeightPrice = DataConvert.ToDecimal(dr["AddWeightPrice"].ToString());
            model.FirstWeightPrice = DataConvert.ToDecimal(dr["FirstWeightPrice"].ToString());
            model.VolumeParmer = DataConvert.ToDecimal(dr["VolumeParmer"].ToString());
            model.ProtectedParmer = DataConvert.ToDecimal(dr["ProtectedParmer"].ToString());
            model.AuditBy = DataConvert.ToInt(dr["AuditBy"].ToString(), 0);
            model.AuditTime = DataConvert.ToDateTime(dr["AuditTime"].ToString());
            model.AuditCode = dr["AuditCode"].ToString();
            model.AuditResult = (AuditResult)DataConvert.ToInt(dr["AuditResult"].ToString());
            model.Status = (MaintainStatus)DataConvert.ToInt(dr["Status"].ToString());
            model.WeightType = DataConvert.ToInt(dr["WeightType"].ToString(), 0);
            model.WeightValueRule = DataConvert.ToInt(dr["WeightValueRule"].ToString(), 0);
            model.DistributionCode = dr["DistributionCode"].ToString();
            model.EffectDate = DataConvert.ToDateTime(dr["EffectDate"].ToString());
            model.UpdateUser = DataConvert.ToInt(dr["UPDATEBY"].ToString(), 0);
            model.UpdateTime = DataConvert.ToDateTime(dr["UpdateTime"].ToString());
            model.EffectID = DataConvert.ToInt(dr["EffectID"].ToString(), 0);
            model.ExtraCashServiceFee = DataConvert.ToDecimal(dr["ExtraCashServiceFee"]);
            model.ExtraPOSServiceFee = DataConvert.ToDecimal(dr["ExtraPOSServiceFee"]);
            model.ExtraProtected = DataConvert.ToDecimal(dr["ExtraProtected"]);
            model.ExtraReceiveFeeRate = DataConvert.ToDecimal(dr["ExtraReceiveFeeRate"]);
            model.ExtraReceivePosFeeRate = DataConvert.ToDecimal(dr["ExtraReceivePosFeeRate"]);
            model.ExtraRefuseFeeRate = DataConvert.ToDecimal(dr["ExtraRefuseFeeRate"]);
            model.ExtraVisitChangeFeeRate = DataConvert.ToDecimal(dr["ExtraVisitChangeFee"]);
            model.ExtraVisitReturnsFeeRate = DataConvert.ToDecimal(dr["ExtraVisitReturnsFee"]);
            model.ExtraVisitReturnsVFeeRate = DataConvert.ToDecimal(dr["ExtraVisitReturnsVFee"]);
            model.IsCategory = DataConvert.ToInt(dr["IsCategory"]);

            using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {
                if (!_fMS_MerchantDeliverFeeDao.UpdateToEffect(model)) return false;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.MerchantID.ToString(), int.Parse(model.UpdateUser.ToString()),
                    string.Format("已生效(生效时间：{0},对应待生效编号：{1})", model.EffectDate, model.EffectID), 3))
                    return false;
                scope.Complete();
            }
            return true;
        }

        public bool DeleteWaitMerchantDeliverFee(string feeid)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
			{
                if (!_fMS_MerchantDeliverFeeDao.DeleteWaitMerchantDeliverFee(feeid)) return false;
                if (!_accountOperatorLogDao.AddOperatorLogLog(feeid, 0, "已生效", 4)) return false;
                work.Complete();
            }

            return true;
        }
        #endregion
    }
}
