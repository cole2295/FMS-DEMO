using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using System.Data;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Model;
using System.Web.Services;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class MerchantDeliverFeeEdit : BasePage
	{
        IMerchantDeliverFee merchantDeliverFee = ServiceLocator.GetService<IMerchantDeliverFee>();
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                BindAreaType();
                tbFee.Attributes["onclick"] = "fnPriceClick('../COD/DeliveryPriceChoose.aspx','" + tbFee.ClientID + "')";
                tbDeliverFee.Attributes["onclick"] = "fnPriceClick('../COD/DeliveryPriceChoose.aspx','" + tbDeliverFee.ClientID + "')";
                BindWarehouse(base.DistributionCode);
                InitForm();
            }
            
		}

        public void BindAreaType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(ddlAreaType, "所有", "-1", "AreaType", base.DistributionCode);
        }

        public string MerchantID
        {
            get { return UCMerchantSourceTV.SelectMerchantID; }
        }

        public string DisCode
        {
            get { return base.DistributionCode; }
        }

		public string FeeID
		{
			get { return string.IsNullOrEmpty(Request.QueryString["id"]) ? "" : Request.QueryString["id"]; }
		}

        public string EffectKid
        {
            get { return string.IsNullOrEmpty(ViewState["EffectKid"].ToString()) ? "" : ViewState["EffectKid"].ToString(); }
            set { ViewState.Add("EffectKid", value); }
        }

        /// <summary>
        /// 操作类型 1、待生效增加，2、待生效修改
        /// </summary>
        public int OpType
        {
            get { return string.IsNullOrEmpty(Request.QueryString["opType"]) ? 0 : int.Parse(Request.QueryString["opType"]); }
        }

        public int ExpressCompanyID
        {
            get
            {
                return ViewState["ExpressCompanyID"] == null ? 0 : ViewState["ExpressCompanyID"].ToString().TryGetInt();
            }
            set
            {
                ViewState.Add("ExpressCompanyID", value);
            }
        }

        public string CompanyName
        {
            get
            {
                return ViewState["CompanyName"] == null ? null : ViewState["CompanyName"].ToString();
            }
            set
            {
                ViewState.Add("CompanyName", value);
            }
        }

		private void InitForm()
		{
            IExpressCompanyService expressCompany = ServiceLocator.GetService<IExpressCompanyService>();
            //ExpressCompany ecModel = expressCompany.GetCompanyModelByDistributionCode(base.DistributionCode);
            //if (ecModel != null)
            //{
            //    ExpressCompanyID = ecModel.ExpressCompanyID;
            //    CompanyName = ecModel.CompanyName;
            //}
            //txtExpressCompany.Text = CompanyName;

			string msg=string.Empty;
            if (OpType == 0)
            {
                if (FeeID == "")
                {
                    msg = "新增收入配送价格";
                    tbDeliverFee.Attributes.Add("style","display:none");
                    lbFee.Attributes.Add("style","display:none");
                }
                else
                {
                    msg = string.Format("修改编号{0}收入配送价格", FeeID);
                    LoadEditData();
                }
                ShowEffectControls(false, null);
            }
            else
            {
                msg = "待生效收入配送价格操作";
                ShowEffectControls(true, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                LoadEditData();
            }
			lbMsg.Text = msg;
		}

        public void BindWarehouse(string disCode)
        {
            IWareHouseService wareHouseService = ServiceLocator.GetService<IWareHouseService>();
            DataTable data = wareHouseService.GetSortCenter(disCode);
            ddlSortCenter.BindListData(data, "WarehouseName", "WarehouseId", "所有", "-1");
        }

		private void LoadEditData()
		{
			if (FeeID == "")
			{
				RunJS("alert('读取数据失败');window.close();");
			}
            FMS_StationDeliverFee stationDeliverFee=new FMS_StationDeliverFee();
            if (OpType == 2)
            {
                stationDeliverFee = merchantDeliverFee.GetWaitDeliverFeeById(FeeID);
            }
            else
            {
                stationDeliverFee=merchantDeliverFee.GetDeliverFeeById(int.Parse(FeeID));
            }
			if (stationDeliverFee == null)
				return;

             
			ddlAreaType.SelectedValue = stationDeliverFee.AreaType.ToString();
            ddlAreaType.Enabled = false;
            UCMerchantSourceTV.Editable = false;
            UCExpressCompanyTV.SetCheckEnable= false;
		    UCExpressCompanyTV.Editable = false;
            UCMerchantSourceTV.SelectMerchantID = stationDeliverFee.MerchantID.ToString();
            UCMerchantSourceTV.SelectMerchantName = stationDeliverFee.MerchantName;
            MerchantCategoryChecked.Value = stationDeliverFee.GoodsCategoryCode+",";
            IsUpdate.Value = "1";
            UCExpressCompanyTV.SetCheckEnable = stationDeliverFee.IsExpress == 1 ? true : false;
		    UCExpressCompanyTV.SelectExpressID = stationDeliverFee.StationID.ToString();
		    UCExpressCompanyTV.SelectExpressName =stationDeliverFee.StationID==11?"全部": stationDeliverFee.StationName;
            ddlSortCenter.Enabled = false;
			ddlSortCenter.SelectedValue = stationDeliverFee.ExpressCompanyID.ToString();
			tbFee.Text = stationDeliverFee.BasicDeliverFee;
            tbDeliverFee.Text = stationDeliverFee.DeliverFee;
            rbNo.Checked = stationDeliverFee.IsCod == 0 ? true : false;
            rbYes.Checked = stationDeliverFee.IsCod == 1 ? true : false;
            tbDeliverFee.Attributes.Add("style", rbYes.Checked ? "display:block" : "display:none");
            lbFee.Attributes.Add("style", rbYes.Checked ? "display:block" : "display:none");
            if (OpType == 2)
            {
                EffectKid = stationDeliverFee.EffectKid;
                ShowEffectControls(true, stationDeliverFee.EffectDate.ToString("yyyy-MM-dd"));
            }
		}

        private void ShowEffectControls(bool flag, string effectDate)
        {

            txtEffectDate.Visible = flag;
            lbEffectDate.Visible = flag;
            if (flag)
                txtEffectDate.Text = effectDate;
        }

		protected void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
                if (OpType == 0)
                {
                    if (FeeID == "")
                    {
                        AddFee();
                    }
                    else
                    {
                        UpdateFee();
                    }
                }
                else if (OpType == 1)
                {
                    AddWaitFee();
                }
                else
                {
                    UpdateWaitFee();
                }
			}
			catch (Exception ex)
			{
				Alert("操作失败<br>" + ex.Message);
			}
		}

		private void AddFee()
		{
			List<FMS_StationDeliverFee> sdf = BuildModel();
			if (sdf != null)
			{
                string msg = string.Empty;
                bool m = merchantDeliverFee.BatchAddDeliverFee(sdf,out msg);
				if (m)
				{
                    if (!string.IsNullOrEmpty(msg))
                    {
                        Alert("以下价格公式已设置<br>" + msg);
                    }
                    else
                    {
                        RunJS("alert('添加成功');window.close();");
                    }
				}
				else
				{
                    Alert(msg);
				}
			}
		}

		private void UpdateFee()
		{
			List<FMS_StationDeliverFee> sdf = BuildModel();
			if (sdf != null)
			{
                string m = merchantDeliverFee.UpdateDeliverFee(sdf[0]);
				if (m.Contains("成功"))
				{
					RunJS("alert('" + m + "');window.close();");
				}
				else
				{
					Alert(m);
				}
			}
		}

        private void AddWaitFee()
        {
            List<FMS_StationDeliverFee> sdf = BuildModel();
            if (sdf != null)
            {
                string m = merchantDeliverFee.AddWaitDeliverFee(sdf[0]);
                if (m.Contains("成功"))
                {
                    RunJS("alert('" + m + "');window.close();");
                }
                else
                {
                    Alert(m);
                }
            }
        }

        private void UpdateWaitFee()
        {
            List<FMS_StationDeliverFee> sdf = BuildModel();
            
            if (sdf != null)
            {
                string m = merchantDeliverFee.UpdateWaitDeliverFee(sdf[0]);
                if (m.Contains("成功"))
                {
                    RunJS("alert('" + m + "');window.close();");
                }
                else
                {
                    Alert(m);
                }
            }
        }

		private List<FMS_StationDeliverFee> BuildModel()
		{
			if (!JudgeInput())
				return null;

			var sdf = new FMS_StationDeliverFee();
            sdf.DistributionCode = base.DistributionCode;
            if (OpType == 2)
            {
                sdf.EffectKid = EffectKid;
            }
            else
            {
                sdf.ID = FeeID == "" ? 0 : int.Parse(FeeID);
            }

            sdf.MerchantID = int.Parse(UCMerchantSourceTV.SelectMerchantID);
            sdf.MerchantName = UCMerchantSourceTV.SelectMerchantName;
			sdf.AreaType = int.Parse(ddlAreaType.SelectedValue);
			//配送商
            sdf.StationID = ExpressCompanyID;
		    sdf.StationName = CompanyName;
			sdf.IsCenterSort = 1;
            //判断是否走配送商逻辑
            if (UCExpressCompanyTV.SetCheckEnable&& ExpressCompanyID != 11)
		    {
		        sdf.IsExpress = 1;
		    }
		    else
		    {
		        sdf.IsExpress = 0;
		    }
            //分拣中心
            sdf.ExpressCompanyID = int.Parse(ddlSortCenter.SelectedValue.ToString(CultureInfo.InvariantCulture));
            sdf.ExpressCompanyName = ddlSortCenter.SelectedItem.Text;

			
            sdf.IsCod = rbYes.Checked ? 1 : 0;
            if (sdf.IsCod == 1)
            {
                sdf.BasicDeliverFee = Request.Form[tbFee.ClientID.Replace("_", "$")].Trim();
                sdf.DeliverFee = Request.Form[tbDeliverFee.ClientID.Replace("_", "$")].Trim();
            }
            else
            {
                sdf.BasicDeliverFee = Request.Form[tbFee.ClientID.Replace("_", "$")].Trim();
                sdf.DeliverFee = sdf.BasicDeliverFee;
            }
			sdf.CreateUser = Userid; sdf.CreateUserCode = UserCode;
		    if (OpType==2)
		    {
		      sdf.UpdateUser = Userid;
		      sdf.UpdateUserCode = UserCode;
		    }
            
			sdf.Status = EnumCODAudit.A2;
            sdf.EffectDate = txtEffectDate.Visible ? DateTime.Parse(txtEffectDate.Text) : DateTime.MinValue;
            var categoryCode = MerchantCategoryChecked.Value.TrimEnd(',').Split(',');
            var sdfList = new List<FMS_StationDeliverFee>();
            if (categoryCode.Length <= 0)
            {
                sdfList.Add(sdf);
            }
            else
            {
                foreach (string code in categoryCode)
                {
                    var sdfNew = new FMS_StationDeliverFee();
                    merchantDeliverFee.MapAreaExpressLevelIncome(sdf, ref sdfNew);
                    sdfNew.GoodsCategoryCode = code;
                    sdfList.Add(sdfNew);
                }
            }
            return sdfList;
		}
		private bool JudgeInput()
		{
            if (OpType == 1 || OpType == 2)
            {
                DateTime dtEffect;
                if (!DateTime.TryParse(txtEffectDate.Text, out dtEffect))
                {
                    Alert("待生效日期输入错误");
                    return false;
                }

                TimeSpan day = dtEffect - DateTime.Now;
                if (day.TotalDays <= 0)
                {
                    Alert("生效日期必须大于当天日期");
                    return false;
                }
            }

            if (string.IsNullOrEmpty(UCMerchantSourceTV.SelectMerchantID))
			{
				Alert("没有选择商家");
				return false;
			}
            //验证配送商逻辑
            if (UCExpressCompanyTV.SetCheckEnable)
		    {
		        if (string.IsNullOrEmpty( UCExpressCompanyTV.SelectExpressName))
		        {
		           Alert("配送商逻辑需要选择配送商");
		           return false;
		        }
		        else
		        {
		            ExpressCompanyID = int.Parse(UCExpressCompanyTV.SelectExpressID);
		            CompanyName = UCExpressCompanyTV.SelectExpressName;
		        }
		    }
		    else
		    {
                ExpressCompanyID = 11;
		        CompanyName = "全部";
		    }
            if (IsCategory.Value == "1" && string.IsNullOrEmpty(MerchantCategoryChecked.Value))
            {
                Alert("没有选择货物品类");
                return false;
            }

            if (string.IsNullOrEmpty(ddlAreaType.SelectedValue) || ddlAreaType.SelectedValue=="-1")
			{
				Alert("没有选择区域类型");
				return false;
			}
            if (string.IsNullOrEmpty(ddlSortCenter.SelectedValue))
            {
                Alert("没有选择分拣中心");
                return false;
            }
			string priceStr = Request.Form[tbFee.ClientID.Replace("_","$")].Trim();
			if (priceStr == "")
			{
				Alert("价格或公式不能为空");
				return false;
			}

            if (rbYes.Checked)
            {
                string deliverPriceStr = Request.Form[tbDeliverFee.ClientID.Replace("_", "$")].Trim();
                if (deliverPriceStr == "")
                {
                    Alert("非COD价格或公式不能为空");
                    return false;
                }
            }

			return true;
		}

        [WebMethod]
        public static object BindMerchantCategory(string merchantId, string disCode)
        {
            IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();

            var condition = new SearchCondition()
            {
                MerchantID = int.Parse(merchantId),
                StatusList = "'" + (int)MaintainStatus.Audited+"'",
                IsRawData = true,
                DistributionCode = disCode,
            };
            PageInfo pi = new PageInfo(1);
            var data = deliverFeeService.BindDeliverFeeList(condition,ref pi);
            if (data==null || data.Rows.Count == 0)
            {
                OutModel o = new OutModel
                {
                    CategoryCode = "E001",
                    CategoryName = "请先设置商家基础信息且已审核后设置",
                };
                return new { done = false, dataModel = o };
            }

            if (data.Rows[0]["IsCategory"]==DBNull.Value || data.Rows[0]["IsCategory"].ToString() == "0")
            {
                OutModel o = new OutModel
                {
                    CategoryCode = "E002",
                    CategoryName = "不按货物品类结算",
                };
                return new { done = false, dataModel = o };
            }

            IGoodsCategoryService goodsCategoryService=ServiceLocator.GetService<IGoodsCategoryService>();
            DataTable dt = goodsCategoryService.GetGoodsCategoryByMerchantID(int.Parse(merchantId), disCode);

            if (dt == null || dt.Rows.Count <= 0)
            {
                return new { done = false, dataModel = "没有货物品类" };
            }
            List<OutModel> list = new List<OutModel>();
            foreach (DataRow d in dt.Rows)
            {
                OutModel o = new OutModel
                {
                    CategoryCode = d["code"].ToString(),
                    CategoryName = d["name"].ToString(),
                };
                list.Add(o);
            }

            return new { done = true, dataModel = list };
        }

        protected void test_Click(object sender, EventArgs e)
        {

        }
	}

    public class OutModel
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
    }
}
