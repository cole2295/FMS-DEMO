using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL;
using RFD.FMS.WEB.Main;
using System.Text.RegularExpressions;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class UpdateDeliverFee : BasePage
	{
        IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();

		#region 常量
		private static readonly string PAYMENT_TYPE = "PaymentType";
		private static readonly string PAYMENT_PERIOD = "PaymentPeriod";
		private static readonly string PAYMENT_PERIOD_DATE = "PaymentPeriodDate";
		private static readonly string DELIVER_FEE_TYPE = "DeliverFeeType";
		private static readonly string DELIVER_FEE_PERIOD = "DeliverFeePeriod";
		private static readonly string DELIVER_FEE_PERIOD_DATE = "DeliverFeePeriodDate";
		private static readonly string FEE_FACTORS = "FeeFactors";
		private static readonly string IS_UNIFORMED_FEE = "IsUniformedFee";
		private static readonly string BASIC_DELIVER_FEE = "BasicDeliverFee";
		private static readonly string FORMULA = "FormulaID";
		private static readonly string REFUSE_FEE_RATE = "RefuseFeeRate";
		private static readonly string RECEIVE_FEE_RATE = "ReceiveFeeRate";
		private static readonly string STATUS = "Status";
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                if (OpType != 0)
                {
                    ShowEffectControls(true, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                }
                else
                    ShowEffectControls(false, null);
				BindMerchantDeliverFeeInfo();
			}
		}

        private void ShowEffectControls(bool flag,string effectDate)
        {
            
            txtEffectDate.Visible = flag;
            lbEffect.Visible = flag;
            if (flag)
                txtEffectDate.Text = effectDate;
        }

        private int ID
        {
            get { return ViewState["ID"] == null ? 0 : ViewState["ID"].ToString().TryGetInt(); }
            set { ViewState.Add("ID", value); }
        }


		private void BindMerchantDeliverFeeInfo()
		{
			//绑定商家名称
            string str = "商家：" + deliverFeeService.GetMerchantNameFromDictionary(CurrentMerchant);
            if (OpType == 1) str += "(新增待生效)";
            if (OpType == 2) str += "(更新待生效)";
            this.lblMerchantName.Text = str;
			var condition = new SearchCondition()
			{
				MerchantID = CurrentMerchant,
                DistributionCode=base.DistributionCode,
                IsEffect=(OpType==0 || OpType==1)?false:true,
			};
            PageInfo pi =new PageInfo(1);
            var data = deliverFeeService.BindDeliverFeeList(condition, ref pi);
			if (data == null || data.Rows.Count <= 0)
			{
				RunJS("alert('读取数据错误');window.close();");
				return;
			}
            if (data.Rows[0]["ID"] == DBNull.Value)
                ID = 0;
            else
                ID = int.Parse(data.Rows[0]["ID"].ToString());
			IsUpdate = int.Parse(data.Rows[0]["Status"].ToString());

            DataRow dr = data.Rows[0];

            txtVolumeParmer.Text =  dr["VolumeParmer"].IsNullData() ? "":dr["VolumeParmer"].ToString();
            txtProtectedParmer.Text =dr["ProtectedParmer"].IsNullData()? "":dr["ProtectedParmer"].ToString();
            txtExtraProtected.Text = dr["ExtraProtected"].IsNullData()? "":dr["ExtraProtected"].ToString();
			//绑定拒收配送费和代收手续费
			txtRefuseFee.Text = dr[REFUSE_FEE_RATE].IsNullData()? "" :dr[REFUSE_FEE_RATE].ToString();
            txtExtraRefuseFee.Text = dr["ExtraRefuseFeeRate"].IsNullData()? "":dr["ExtraRefuseFeeRate"].ToString();
			rbCashPeriodRate.Checked = dr["ReceiveFeeTypeStr"].ToString() == "周期结" ? true : rbCashMonthRate.Checked = true;
            txtReceiveFee.Text = dr[RECEIVE_FEE_RATE].IsNullData()?"":dr[RECEIVE_FEE_RATE].ToString();

            txtExtraReceiveFee.Text = dr["ExtraReceiveFeeRate"].IsNullData() ? "" : dr["ExtraReceiveFeeRate"].ToString();
            txtVisitReturnsFee.Text = dr["VisitReturnsFee"].IsNullData() ? "" : dr["VisitReturnsFee"].ToString();
            txtExtraVisitReturnsFee.Text = dr["ExtraVisitReturnsFee"].IsNullData() ? "" : dr["ExtraVisitReturnsFee"].ToString();
            rbPosPeriodRate.Checked = dr["ReceivePOSFeeTypeStr"].ToString() == "周期结" ? true : rbPosMonthRate.Checked = true;
            txtReceivePosFee.Text = dr["ReceivePOSFeeRate"].IsNullData() ? "" : dr["ReceivePOSFeeRate"].ToString();
            txtExtraReceivePosFee.Text = dr["ExtraReceivePOSFeeRate"].IsNullData() ? "" : dr["ExtraReceivePOSFeeRate"].ToString();
            txtVisitChangeFee.Text = dr["VisitChangeFee"].IsNullData() ? "" : dr["VisitChangeFee"].ToString();
            txtExtraVisitChangeFee.Text = dr["ExtraVisitChangeFee"].IsNullData() ? "" : dr["ExtraVisitChangeFee"].ToString();
            txtVisitReturnsVFee.Text = dr["VisitReturnsVFee"].IsNullData() ? "" : dr["VisitReturnsVFee"].ToString();
            txtExtraVisitReturnsVFee.Text = dr["ExtraVisitReturnsVFee"].IsNullData() ? "" : dr["ExtraVisitReturnsVFee"].ToString();
            rbCashPeriodService.Checked = dr["CashServiceTypeStr"].ToString() == "周期结" ? true : rbCashMonthService.Checked = true;
            txtCashServiceFee.Text = dr["CashServiceFee"].IsNullData() ? "" : dr["CashServiceFee"].ToString();
            txtExtraCashServiceFee.Text = dr["ExtraCashServiceFee"].IsNullData() ? "" : dr["ExtraCashServiceFee"].ToString();
            rbPosPeriodService.Checked = dr["POSServiceTypeStr"].ToString() == "周期结" ? true : rbPosMonthService.Checked = true;
            txtPOSServiceFee.Text = dr["POSServiceFee"].IsNullData() ? "" : dr["POSServiceFee"].ToString();
            txtExtraPOSServiceFee.Text = dr["ExtraPOSServiceFee"].IsNullData() ? "" : dr["ExtraPOSServiceFee"].ToString();
            RBIsCategory.Checked = dr["IsCategoryStr"].ToString() == "品类结" ;
		    RBNoCategory.Checked = dr["IsCategoryStr"].ToString() == "非品类结";

            if (!dr["WeightType"].IsNullData())
            {
                setWeightType(dr["WeightType"].ToString());
            }
            if (!dr["WeightValueRule"].IsNullData())
            {
                rbtweighValuetzore.Checked = dr["WeightValueRule"].ToString() == "9" ? true : false;
                rbtweighValuetone.Checked=dr["WeightValueRule"].ToString() == "0"?   true: false;
                rbtweighValuettwo.Checked = dr["WeightValueRule"].ToString() == "1"? true: false;
            }
            if (OpType ==2)
            {
                ShowEffectControls(true, DateTime.Parse(dr["EffectDate"].ToString()).ToString("yyyy-MM-dd"));
            }
		}

		private FMS_MerchantDeliverFee GetMerchantDeliverFee()
		{
		    var merchant = new FMS_MerchantDeliverFee()
		                       {
                                   ID=ID,
		                           MerchantID = CurrentMerchant,
		                           FeeFactors = "",
		                           IsUniformedFee = false,
		                           BasicDeliverFee = (decimal?) null,
		                           FormulaID = (int?) null,

		                           RefuseFeeRate = txtRefuseFee.Text.Trim().TryGetDecimal(),
		                           VisitReturnsFeeRate = txtVisitReturnsFee.Text.Trim().TryGetDecimal(),
		                           VisitReturnsVFeeRate = txtVisitReturnsVFee.Text.Trim().TryGetDecimal(),
		                           VisitChangeFeeRate = txtVisitChangeFee.Text.Trim().TryGetDecimal(),
		                           ReceiveFeeRate = txtReceiveFee.Text.Trim().TryGetDecimal(),
		                           ReceivePosFeeRate = txtReceivePosFee.Text.Trim().TryGetDecimal(),
		                           CashServiceFee = txtCashServiceFee.Text.Trim().TryGetDecimal(),
		                           POSServiceFee = txtPOSServiceFee.Text.Trim().TryGetDecimal(),

		                           ReceiveFeeType = rbCashPeriodRate.Checked ? 0 : 1,
		                           ReceivePOSFeeType = rbPosPeriodRate.Checked ? 0 : 1,
		                           CashServiceType = rbCashPeriodService.Checked ? 0 : 1,
		                           POSServiceType = rbPosPeriodService.Checked ? 0 : 1,

		                           FirstWeight = 0M,
		                           StatPramer = 0M,
		                           AddWeightPrice = 0M,
		                           FirstWeightPrice = 0M,
		                           VolumeParmer = txtVolumeParmer.Text.Trim().TryGetDecimal(),
		                           ProtectedParmer = txtProtectedParmer.Text.TryGetDecimal(),
		                           FormulaParamters = null,
		                           WeightType = getWeightType(),
		                           Status = MaintainStatus.Auditing,
		                           WeightValueRule = rbtweighValuetone.Checked ? 0:rbtweighValuettwo.Checked?1:rbtweighValuetzore.Checked?9:-1,
				                   UpdateBatchNo = DateTime.Now.ToString("yyMMddhhmmss"),
                                   DistributionCode=base.DistributionCode,
                                   EffectDate=string.IsNullOrEmpty(txtEffectDate.Text)?DateTime.Now:DateTime.Parse(txtEffectDate.Text),

                                   ExtraCashServiceFee = txtExtraCashServiceFee.Text.Trim().TryGetDecimal(),
                                   ExtraPOSServiceFee =  txtExtraPOSServiceFee.Text.Trim().TryGetDecimal(),
                                   ExtraProtected = txtExtraProtected.Text.Trim().TryGetDecimal(),
                                   ExtraReceiveFeeRate = txtExtraReceiveFee.Text.Trim().TryGetDecimal(),
                                   ExtraReceivePosFeeRate = txtExtraReceivePosFee.Text.Trim().TryGetDecimal(),
                                   ExtraRefuseFeeRate = txtExtraRefuseFee.Text.Trim().TryGetDecimal(),
                                   ExtraVisitChangeFeeRate = txtExtraVisitChangeFee.Text.Trim().TryGetDecimal(),
                                   ExtraVisitReturnsFeeRate = txtExtraVisitReturnsFee.Text.Trim().TryGetDecimal(),
                                   ExtraVisitReturnsVFeeRate = txtExtraVisitReturnsVFee.Text.Trim().TryGetDecimal(),

                                   IsCategory = RBIsCategory.Checked ? 1 : 0
                                   
                                   
                                   
			};
			return merchant;
		}

        private int getWeightType()
        {
            int WeightType = 0;
            if(rbtweighttwo.Checked)
            {
                WeightType = 1;
            }
            else if(rbtweightThree.Checked)
            {
                WeightType = 2;
            }
            else if (rbtweightfour.Checked)
            {
                WeightType = 3;
            }
            else if (rbtweightfive.Checked)
            {
                WeightType = 4;
            }
            return WeightType;
        }

        private void setWeightType(string WeightTypeValue)
        {
            if(WeightTypeValue=="0")
            {
                rbtweightone.Checked = true;
            }
            else if (WeightTypeValue == "1")
            {
               rbtweighttwo.Checked=true;
            }
            else if (WeightTypeValue == "2")
            {
                rbtweightThree.Checked = true;
            }
            else if (WeightTypeValue == "3")
            {
                rbtweightfour.Checked = true;
            }
            else if (WeightTypeValue == "4")
            {
                rbtweightfive.Checked = true;
            }
        }

	    protected void btnSave_Click(object sender, EventArgs e)
		{
            if (OpType == 0)
            {
                if (IsUpdate == 0)
                {
                    AddDeliverFee();
                }
                else
                {
                    UpDeliverFee();
                }
            }
            else
            {
                if (OpType == 1)
                {
                    //待生效添加
                    AddEffectDeliverFee();
                }

                if (OpType == 2)
                {
                    //待生效更新
                    UpEffectDeliverFee();
                }
            }
		}

		private void AddDeliverFee()
		{
			if (!JudgeInput())
				return;

			FMS_MerchantDeliverFee merchantDeliverFee = GetMerchantDeliverFee();
			merchantDeliverFee.CreateUser = Userid;

            if (deliverFeeService.SaveDeliverFee(merchantDeliverFee))
			{
				RunJS("alert('添加成功');window.close();");
			}
			else
			{
				Alert("添加失败");
			}
		}

		private void UpDeliverFee()
		{
			if (!JudgeInput())
				return;

			FMS_MerchantDeliverFee merchantDeliverFee = GetMerchantDeliverFee();
			merchantDeliverFee.UpdateUser = Userid;
			merchantDeliverFee.UpdateUserCode = UserCode;
            if (deliverFeeService.UpdateDeliverFee(merchantDeliverFee))
			{
				RunJS("alert('更新成功');window.close();");
			}
			else
			{
				Alert("更新失败");
			}
		}

		private bool JudgeInput()
		{
            if (!rbtweightone.Checked && !rbtweighttwo.Checked && !rbtweightThree.Checked && !rbtweightfour.Checked && !rbtweightfive.Checked)
            {
                Alert("请选择结算重量");
                return false;
            }

            //体积
			if (string.IsNullOrEmpty(txtVolumeParmer.Text.Trim()))
			{
				Alert("体积计算参数不能为空");
				return false;
			}
			if (!Regex.IsMatch(txtVolumeParmer.Text.Trim(), @"^\d+(\.\d{0,2})?$"))
			{
				Alert("体积计算参数不符合标准,如：6000.00");
				return false;
			}
			//保价费
			if (string.IsNullOrEmpty(txtProtectedParmer.Text.Trim()))
			{
				Alert("保价费计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtProtectedParmer.Text.Trim())) 
			{
				Alert("保价费计算参数不符合标准,如：0.003");
				return false;
			}

            if (string.IsNullOrEmpty(txtExtraProtected.Text.Trim()))
            {
                Alert("保价费计算参数不能为空");
                return false;
            }

            if (!JudgeCommonDecimal(txtExtraProtected.Text.Trim()))
            {
                Alert("保价费计算参数不符合标准");
                return false;
            }

			//拒收
			if (string.IsNullOrEmpty(txtRefuseFee.Text.Trim()))
			{
				Alert("拒收费计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtRefuseFee.Text.Trim()))
			{
				Alert("拒收计算参数不符合标准,如：0.003");
				return false;
			}
            if (string.IsNullOrEmpty(txtExtraRefuseFee.Text.Trim()))
            {
                Alert("拒收费计算参数不能为空");
                return false;
            }
            if (!JudgeCommonDecimal(txtExtraRefuseFee.Text.Trim()))
            {
                Alert("拒收计算参数不符合标准");
                return false;
            }

			//上门换
			if (string.IsNullOrEmpty(txtVisitChangeFee.Text.Trim()))
			{
				Alert("上门换计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtVisitChangeFee.Text.Trim()))
			{
				Alert("上门换计算换参数不符合标准,如：0.003");
				return false;
			}

            if (string.IsNullOrEmpty(txtExtraVisitChangeFee.Text.Trim()))
            {
                Alert("上门换计算参数不能为空");
                return false;
            }
            if (!JudgeCommonDecimal(txtExtraVisitChangeFee.Text.Trim()))
            {
                Alert("上门换计算换参数不符合标准");
                return false;
            }

			//上门退
			if (string.IsNullOrEmpty(txtVisitReturnsFee.Text.Trim()))
			{
				Alert("上门退计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtVisitReturnsFee.Text.Trim()))
			{
				Alert("上门退计算参数不符合标准,如：0.003");
				return false;
			}

            if (string.IsNullOrEmpty(txtExtraVisitReturnsFee.Text.Trim()))
            {
                Alert("上门退计算参数不能为空");
                return false;
            }
            if (!JudgeCommonDecimal(txtExtraVisitReturnsFee.Text.Trim()))
            {
                Alert("上门退计算参数不符合标准");
                return false;
            }
			//上门退拒收
			if (string.IsNullOrEmpty(txtVisitReturnsVFee.Text.Trim()))
			{
				Alert("上门退拒收计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtVisitReturnsVFee.Text.Trim()))
			{
				Alert("上门退拒收参数不符合标准,如：0.003");
				return false;
			}

            if (string.IsNullOrEmpty(txtExtraVisitReturnsVFee.Text.Trim()))
            {
                Alert("上门退拒收计算参数不能为空");
                return false;
            }
            if (!JudgeCommonDecimal(txtExtraVisitReturnsVFee.Text.Trim()))
            {
                Alert("上门退拒收参数不符合标准");
                return false;
            }
			//代收货款现金 手续费
			if (string.IsNullOrEmpty(txtReceiveFee.Text.Trim()))
			{
				Alert("代收货款现金手续费计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtReceiveFee.Text.Trim()))
			{
				Alert("代收货款现金计手续费算参数不符合标准,如：0.003");
				return false;
			}
            if (string.IsNullOrEmpty(txtExtraReceiveFee.Text.Trim()))
            {
                Alert("代收货款现金手续费计算参数不能为空");
                return false;
            }
            if (!JudgeCommonDecimal(txtExtraReceiveFee.Text.Trim()))
            {
                Alert("代收货款现金计手续费算参数不符合标准");
                return false;
            }

			//代收货款POS 手续费
			if (string.IsNullOrEmpty(txtReceivePosFee.Text.Trim()))
			{
				Alert("代收货款POS手续费计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtReceivePosFee.Text.Trim()))
			{
				Alert("代收货款POS手续费计算参数不符合标准,如：0.003");
				return false;
			}
            if (string.IsNullOrEmpty(txtExtraReceivePosFee.Text.Trim()))
            {
                Alert("代收货款POS手续费计算参数不能为空");
                return false;
            }
            if (!JudgeCommonDecimal(txtExtraReceivePosFee.Text.Trim()))
            {
                Alert("代收货款POS手续费计算参数不符合标准");
                return false;
            }

			//代收货款现金 服务费
			if (string.IsNullOrEmpty(txtCashServiceFee.Text.Trim()))
			{
				Alert("代收货款现金服务费计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtCashServiceFee.Text.Trim()))
			{
				Alert("代收货款现金服务费计算参数不符合标准,如：0.003");
				return false;
			}
            if (string.IsNullOrEmpty(txtExtraCashServiceFee.Text.Trim()))
            {
                Alert("代收货款现金服务费计算参数不能为空");
                return false;
            }
            if (!JudgeCommonDecimal(txtExtraCashServiceFee.Text.Trim()))
            {
                Alert("代收货款现金服务费计算参数不符合标准");
                return false;
            }

			//代收货款POS 服务费
			if (string.IsNullOrEmpty(txtPOSServiceFee.Text.Trim()))
			{
				Alert("代收货款POS服务费计算参数不能为空");
				return false;
			}
			if (!JudgeDecimal(txtPOSServiceFee.Text.Trim()))
			{
				Alert("代收货款POS服务费计算参数不符合标准,如：0.003");
				return false;
			}
            
            if (string.IsNullOrEmpty(txtExtraPOSServiceFee.Text.Trim()))
            {
                Alert("代收货款POS服务费计算参数不能为空");
                return false;
            }
		    if (!JudgeCommonDecimal(txtExtraPOSServiceFee.Text.Trim()))
		    {
                Alert("代收货款POS服务费计算参数不符合标准");
		        return false;
		    }        

            if(!RBIsCategory.Checked && !RBNoCategory.Checked)
            {
                Alert("请选择货物品类");
                return false;
            }

           
            if (OpType == 1 || OpType == 2)
            {
                DateTime dtEffect;
                if(!DateTime.TryParse(txtEffectDate.Text,out dtEffect))
                {
                    Alert("时间格式不正确");
                    return false;
                }
                TimeSpan day = dtEffect - DateTime.Now;
                if (day.TotalDays <= 0)
                {
                    Alert("生效日期必须大于当天日期");
                    return false;
                }
            }

			return true;
		}

		public int CurrentMerchant
		{
			get
			{
				return Request["mid"] != null ? int.Parse(Request["mid"]) : -1;
			}
		}

        /// <summary>
        /// 操作类型
        /// </summary>
        public int OpType
        {
            get
            {
                return Request["opType"] != null ? int.Parse(Request["opType"]) : -1;
            }
        }

		public int IsUpdate
		{
			get { return int.Parse(ViewState["IsUpdate"].ToString()); }
			set { ViewState.Add("IsUpdate", value); }
		}

		private bool JudgeDecimal(string str)
		{
			return Regex.IsMatch(str, @"^[012](\.\d{0,4})?$");
		}

        private bool JudgeCommonDecimal(string str)
        {
            return Regex.IsMatch(str, @"(^\d+\.?\d{0,3})$");
        }
        public void AddEffectDeliverFee()
        {
            try
            {
                if (!JudgeInput())
                    return;

                FMS_MerchantDeliverFee merchantDeliverFee = GetMerchantDeliverFee();
                merchantDeliverFee.CreateUser = Userid;

                if (deliverFeeService.SaveEffectDeliverFee(merchantDeliverFee))
                {
                    RunJS("alert('添加成功');window.close();");
                }
                else
                {
                    Alert("添加失败");
                }
            }
            catch (Exception ex)
            {
                Alert("添加失败<br>"+ex.Message);
            }
        }

        private void UpEffectDeliverFee()
        {
            try
            {
                if (!JudgeInput())
                    return;

                FMS_MerchantDeliverFee merchantDeliverFee = GetMerchantDeliverFee();
                merchantDeliverFee.UpdateUser = Userid;
                merchantDeliverFee.UpdateUserCode = UserCode;
                if (deliverFeeService.UpdateEffectDeliverFee(merchantDeliverFee))
                {
                    RunJS("alert('更新成功');window.close();");
                }
                else
                {
                    Alert("更新失败");
                }
            }
            catch (Exception ex)
            {
                Alert("更新失败<br>"+ex.Message);
            }
        }
	}
}
