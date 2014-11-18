using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.MODEL.Enumeration;
using System.Web.UI.HtmlControls;
using RFD.FMS.Util.ControlHelper;
using System.Data;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.COD
{
	public partial class DeliveryPriceEdit : BasePage
	{
        IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();

		protected void Page_Load(object sender, EventArgs e)
		{
            txtEffectDate.Text = DateTime.Now.AddDays(1).ToShortDateString();
			if (!IsPostBack)
			{
                BindAreaType();
                BindLinetType();
                //BindWarehouse(base.DistributionCode);

                txtCodPrice.Attributes["onclick"] = "fnPriceClick('DeliveryPriceChoose.aspx','" + txtCodPrice.ClientID + "')";
                txtPrice.Attributes["onclick"] = "fnPriceClick('DeliveryPriceChoose.aspx','" + txtPrice.ClientID + "')";
				Msg.Font.Bold = true;
				Msg.Font.Size = FontUnit.Large;
				
				if (!string.IsNullOrEmpty(CodLineNo))
				{
					switch (IsEffect)
					{
						case 0:
							InitForm(true, "将对 " + CodLineNo + " 线路进行更改", deliveryPriceService.GetListByCodLineNo(CodLineNo));
							break;
						case 1:
							InitForm(false, "将对 " + CodLineNo + " 线路进行添加待生效价格", deliveryPriceService.GetListByCodLineNo(CodLineNo));
							break;
						case 2:
							InitForm(false, "将对 " + CodLineNo + " 线路进行待生效价格更改", deliveryPriceService.GetListByEffectCodLineNo(CodLineNo));
							break;
                        case 3:
                            InitForm(false, "批量新增待生效价格", null);
                            break;
                        case 4:
                            InitForm(false, "批量更新待生效价格", null);
                            break;
						default:
							break;
					}
				}
				else
					Msg.Text = "将新增一条新的线路";
			}
		}

        public void BindAreaType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(AreaType, "所有", "-1", "AreaType", base.DistributionCode);
        }

        public void BindLinetType()
        {
            IStatusCodeService service = ServiceLocator.GetService<IStatusCodeService>();
            service.BindDropDownListByCodeType(LineStatus, "所有", "-1", "AreaTypeLine", base.DistributionCode);
        }

        //public void BindWarehouse(string disCode)
        //{
        //    DataTable data = new WareHouseService().GetWareHouseSortCenter(disCode);
        //    wareHouse.BindListData(data, "WarehouseName", "WarehouseId", "所有", "-1");
        //}

        public FMS_CODLine CODLine
        {
            get { return ViewState["CODLine"] == null ? null : ViewState["CODLine"] as FMS_CODLine; }
            set { ViewState.Add("CODLine", value); }
        }

		private void InitForm(bool flag,string msg, FMS_CODLine codLine)
		{
            CODLine = codLine;
            if (IsEffect == 3 || IsEffect == 4)
            {
                ucSelectStation.Visible = flag;
                UCMerchantSourceTV.Visible = flag;
                AreaType.Visible = flag;
                LineStatus.Visible = flag;
                lbucSelectStation.Visible = flag;
                lbddlMerchant.Visible = flag;
                lbAreaType.Visible = flag;
                lbLineStatus.Visible = flag;
                labEffectDate.Visible = !flag;
                txtEffectDate.Visible = !flag;
                lbIsCod.Text = "批量更改类型：";
                rbIsCodTrue.Visible = flag;
                rbIsCodFalse.Visible = flag;
                ddlBatchType.Visible = !flag;
            }
            else
            {
                LineStatus.Enabled = flag;
                labEffectDate.Visible = !flag;
                txtEffectDate.Visible = !flag;
                Msg.Text = msg;
                lbIsCod.Text = "是否分COD区：";
                LoadLineData(codLine);
            }
		}

		protected void AreaTypeData_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["codeType"] = "AreaType";
		}

		protected void LineStatusData_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["codeType"] = "AreaTypeLine";
		}

		/// <summary>
		/// 线路编号
		/// </summary>
		private string CodLineNo
		{
			get
			{
				return string.IsNullOrEmpty(Request.QueryString["lineNo"]) ? null : Request["lineNo"].ToString();
			}
		}

		/// <summary>
		/// 0新增、修改，1、待生效新增，2、待生效修改,3、批量新增待生效,4、批量修改待生效
		/// </summary>
		private int IsEffect
		{
			get
			{
				return string.IsNullOrEmpty(Request.QueryString["isEffect"]) ? 0 : int.Parse(Request["isEffect"].ToString());
			}
		}

		private void LoadLineData(FMS_CODLine codLine)
		{
            if (codLine == null )
			{
                if (IsEffect != 3 || IsEffect != 4)
                    return;

				Alert("未取到可编辑数据");
				return;
			}
			ucSelectStation.StationID = codLine.ExpressCompanyID.ToString();
			ucSelectStation.StationName = codLine.CompanyName;
			//IsEchelonTrue.Checked = codLine.IsEchelon == 1 ? true : false;
			//IsEchelonFalse.Checked = codLine.IsEchelon == 2 ? true : false;
			//wareHouse.SelectedValue = string.IsNullOrEmpty(codLine.WareHouseID) ? "-1" : codLine.WareHouseID;
            UCMerchantSourceTV.SelectMerchantName = codLine.MerchantName;
			UCMerchantSourceTV.SelectMerchantID = codLine.MerchantID.ToString();
			AreaType.SelectedValue = string.IsNullOrEmpty(codLine.AreaType.ToString()) ? "-1" : codLine.AreaType.ToString();
            rbIsCodFalse.Checked = codLine.IsCOD == 0 ? true : false;
            rbIsCodTrue.Checked = codLine.IsCOD == 1 ? true : false;
			txtCodPrice.Text = codLine.PriceFormula;
            if (rbIsCodTrue.Checked)
                txtPrice.Text = codLine.Formula;
            else
            {
                txtPrice.Visible = false;
                lbPrice.Visible = false;
                lbCodPrice.Text = "价格";
            }
			LineStatus.SelectedValue = string.IsNullOrEmpty(codLine.LineStatus.ToString()) ? "-1" : codLine.LineStatus.ToString();
			txtEffectDate.Text = string.IsNullOrEmpty(codLine.EffectDate.ToString()) ? DateTime.Now.ToShortDateString() : codLine.EffectDate != DateTime.MinValue ? 
				codLine.EffectDate.ToShortDateString() : DateTime.Now.ToShortDateString() ;

			ucSelectStation.Editable = false;
			//IsEchelonTrue.Enabled = false;
			//IsEchelonFalse.Enabled = false;
			//wareHouse.Enabled = false;
			AreaType.Enabled = false;
			//ddlProduct.Enabled = false;
            UCMerchantSourceTV.Editable = false;
		}

		protected void btOK_Click(object sender, EventArgs e)
		{
            try
            {
                if (!string.IsNullOrEmpty(CodLineNo))
                {
                    switch (IsEffect)
                    {
                        case 0:
                            UpdateCodLine(); break;
                        case 1:
                            AddEffectCodLine(); break;
                        case 2:
                            UpdateEffectCodLine(); break;
                        case 3:
                            BatchAddEffectCodLine(); break;
                        case 4:
                            BatchUpdateEffectCodLine(); break;
                        default:
                            break;
                    }
                }
                else
                {
                    AddCodLine();
                }
            }
            catch(Exception ex)
            {
                Alert("操作失败<br>"+ex.Message);
            }
		}

		private void UpdateCodLine()
		{
			try
			{
				if (!JudgeInput())
					return;
				FMS_CODLine codLine = new FMS_CODLine();
				codLine.ExpressCompanyID = int.Parse(ucSelectStation.StationID);
                //codLine.IsEchelon = IsEchelonTrue.Checked == true ? 1 : 2;
                //if (wareHouse.SelectedValue == "")
                //    codLine.WareHouseType = 0;
                //else if (wareHouse.SelectedValue.Contains("S_"))
                //    codLine.WareHouseType = 2;
                //else
                //    codLine.WareHouseType = 1;

                //codLine.WareHouseID = wareHouse.SelectedValue.Contains("S_") ?
                //    wareHouse.SelectedValue.Replace("S_", "") : wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue;

                codLine.IsEchelon = 2;
                codLine.WareHouseType = 0;
                codLine.WareHouseID = "";

                codLine.MerchantID = CODLine.MerchantID;//等于原来的值
                codLine.ProductID = 1;
				codLine.AreaType = int.Parse(AreaType.SelectedValue);
                GetIsCOD(ref codLine);
				codLine.LineStatus = int.Parse(LineStatus.SelectedValue);
				codLine.UpdateBy = Userid.ToString();
				codLine.AuditStatus = (int)EnumCODAudit.A2;
				codLine.CODLineNO = CodLineNo;
                codLine.DistributionCode = base.DistributionCode;
				if (deliveryPriceService.UpdateDeliveryPrice(codLine))
				{
					//Alert("更新成功");					
                    RunJS("alert('更新成功');window.close();");
				}
				else
					Alert("更新失败");
			}
			catch (Exception ex)
			{
				Alert("更新失败");
			}
		}

		private void AddCodLine()
		{
			try
			{
				if (!JudgeInput())
					return;
				FMS_CODLine codLine = new FMS_CODLine();
                codLine.DistributionCode = base.DistributionCode;
				codLine.ExpressCompanyID = int.Parse(ucSelectStation.StationID);
                //codLine.IsEchelon = IsEchelonTrue.Checked == true ? 1 : 2;
                //if (wareHouse.SelectedValue == "")
                //    codLine.WareHouseType = 0;
                //else if (wareHouse.SelectedValue.Contains("S_"))
                //    codLine.WareHouseType = 2;
                //else
                //    codLine.WareHouseType = 1;

                //codLine.WareHouseID = wareHouse.SelectedValue.Contains("S_") ?
                //    wareHouse.SelectedValue.Replace("S_", "") : wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue;

                codLine.IsEchelon = 2;
                codLine.WareHouseType = 0;
                codLine.WareHouseID = "";

				codLine.MerchantID = int.Parse(UCMerchantSourceTV.SelectMerchantID);
				codLine.ProductID = 1 ;
				codLine.AreaType = int.Parse(AreaType.SelectedValue);
                GetIsCOD(ref codLine);
				codLine.LineStatus = int.Parse(LineStatus.SelectedValue);
				codLine.CreateBy = Userid.ToString();
				codLine.AuditStatus = (int)EnumCODAudit.A2;
				int n = deliveryPriceService.AddDeliveryPrice(codLine);
				if (n == 1)
				{
					Alert("添加成功");
					ClearInput();
				}
				else if (n == 0)
					Alert("已存在");
				else
					Alert("添加失败");
			}
			catch (Exception ex)
			{
				Alert("添加失败<br>" + ex);
			}
		}

		private void ClearInput()
		{
			ucSelectStation.StationID = "ucSelectStation";
			ucSelectStation.Name = "";
            //IsEchelonTrue.Checked = true;
            //IsEchelonFalse.Checked = false;
            //wareHouse.Enabled = true;
			AreaType.SelectedValue = "-1";
            rbIsCodFalse.Checked = false;
            rbIsCodTrue.Checked = true;
			txtCodPrice.Text = "";
            txtPrice.Text = "";
			LineStatus.SelectedValue = "-1";
		}

		private bool JudgeInput()
		{
			if (ucSelectStation.StationID == "ucSelectStation")
			{
				Alert("配送商必填");
				return false;
			}
            //if (IsEchelonTrue.Checked && wareHouse.Enabled && wareHouse.SelectedValue == "-1")
            //{
            //    Alert("仓库必填");
            //    return false;
            //}
			if (AreaType.SelectedValue =="-1")
			{
				Alert("区域类型必填");
				return false;
			}
            string codPriceStr = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
            if (rbIsCodTrue.Checked)
            {
                string priceStr = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                if (codPriceStr == "")
                {
                    Alert("COD价格必填");
                    return false;
                }
                if (priceStr == "")
                {
                    Alert("非COD价格必填");
                    return false;
                }
            }
            else
            {
                if (codPriceStr == "")
                {
                    Alert("COD价格必填");
                    return false;
                }
            }


			if (LineStatus.SelectedValue == "-1")
			{
				Alert("(线路状态必填");
				return false;
			}

			return true;
		}

        //protected void IsEchelonTrue_CheckedChanged(object sender, EventArgs e)
        //{
        //    wareHouse.Enabled = IsEchelonTrue.Checked;
        //}

        //protected void IsEchelonFalse_CheckedChanged(object sender, EventArgs e)
        //{
        //    wareHouse.Enabled = IsEchelonTrue.Checked;
        //}

		private void AddEffectCodLine()
		{
			try
			{
				if (!JudgeInput())
					return;

				DateTime effectDate;
				if (!DateTime.TryParse(Request.Form[txtEffectDate.ClientID.Replace("_", "$")].Trim(), out effectDate))
				{
					Alert("输入合法日期");
					return;
				}
				TimeSpan day = effectDate - DateTime.Now;
				if (day.TotalDays <= 0)
				{
					Alert("生效日期必须大于当天日期");
					return;
				}

				FMS_CODLine codLine = new FMS_CODLine();
                codLine.DistributionCode = base.DistributionCode;
				codLine.CODLineNO = CodLineNo;
				codLine.ExpressCompanyID = int.Parse(ucSelectStation.StationID);
                //codLine.IsEchelon = IsEchelonTrue.Checked == true ? 1 : 2;
                //if (wareHouse.SelectedValue == "")
                //    codLine.WareHouseType = 0;
                //else if (wareHouse.SelectedValue.Contains("S_"))
                //    codLine.WareHouseType = 2;
                //else
                //    codLine.WareHouseType = 1;

                //codLine.WareHouseID = wareHouse.SelectedValue.Contains("S_") ?
                //    wareHouse.SelectedValue.Replace("S_", "") : wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue;

                codLine.IsEchelon = 2;
                codLine.WareHouseType = 0;
                codLine.WareHouseID = "";

				codLine.MerchantID = int.Parse(UCMerchantSourceTV.SelectMerchantID);
				codLine.ProductID =  1 ;
				codLine.AreaType = int.Parse(AreaType.SelectedValue);
                GetIsCOD(ref codLine);
				codLine.LineStatus = int.Parse(LineStatus.SelectedValue);
				codLine.CreateBy = Userid.ToString();
				codLine.AuditStatus = (int)EnumCODAudit.A2;
				codLine.EffectDate = effectDate;
				int n = deliveryPriceService.AddEffectDeliveryPrice(codLine);
				if (n == 1)
				{
					RunJS("alert('添加成功');window.close();");
					
				}
				else if (n == 0)
					RunJS("alert('"+codLine.CODLineNO + "已存在，直接操作 更改待生效价格');window.close()");
				else
					Alert("添加失败");
			}
			catch (Exception ex)
			{
				Alert("添加失败");
			}
		}

		private void UpdateEffectCodLine()
		{
			try
			{
				if (!JudgeInput())
					return;

				DateTime effectDate;
				if (!DateTime.TryParse(Request.Form[txtEffectDate.ClientID.Replace("_","$")].Trim(), out effectDate))
				{
					Alert("输入合法日期");
					return;
				}
				TimeSpan day = effectDate - DateTime.Now;
				if (day.TotalDays <= 0)
				{
					Alert("生效日期必须大于当天日期");
					return;
				}
				FMS_CODLine codLine = new FMS_CODLine();
                codLine.DistributionCode = base.DistributionCode;
				codLine.ExpressCompanyID = int.Parse(ucSelectStation.StationID);
                //codLine.IsEchelon = IsEchelonTrue.Checked == true ? 1 : 2;
                //if (wareHouse.SelectedValue == "")
                //    codLine.WareHouseType = 0;
                //else if (wareHouse.SelectedValue.Contains("S_"))
                //    codLine.WareHouseType = 2;
                //else
                //    codLine.WareHouseType = 1;

                //codLine.WareHouseID = wareHouse.SelectedValue.Contains("S_") ?
                //    wareHouse.SelectedValue.Replace("S_", "") : wareHouse.SelectedValue == "-1" ? "" : wareHouse.SelectedValue;

                codLine.IsEchelon = 2;
                codLine.WareHouseType = 0;
                codLine.WareHouseID = "";

                codLine.MerchantID = CODLine.MerchantID;//等于原来的值
				codLine.ProductID = 1 ;
				codLine.AreaType = int.Parse(AreaType.SelectedValue);
                GetIsCOD(ref codLine);
				codLine.LineStatus = int.Parse(LineStatus.SelectedValue);
				codLine.UpdateBy = Userid.ToString();
				codLine.AuditStatus = (int)EnumCODAudit.A2;
				codLine.CODLineNO = CodLineNo;
				codLine.EffectDate = effectDate;
				if (deliveryPriceService.UpdateEffectCodLine(codLine))
				{
                    RunJS("alert('更新成功');window.close();");
				}
				else
					Alert("更新失败");
			}
			catch (Exception ex)
			{
				Alert("更新失败");
			}
		}

        private void BatchAddEffectCodLine()
        {
            if (ddlBatchType.SelectedValue=="0")
            {
                string codPriceStr = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
                string priceStr = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                if (codPriceStr == "")
                {
                    Alert("COD价格必填");
                    return;
                }
                if (priceStr == "")
                {
                    Alert("非COD价格必填");
                    return;
                }
            }
            else if (ddlBatchType.SelectedValue == "1")
            {
                string codPriceStr = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
                if (codPriceStr == "")
                {
                    Alert("COD价格必填");
                    return;
                }
            }
            else
            {
                string priceStr = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                if (priceStr == "")
                {
                    Alert("非COD价格必填");
                    return;
                }
            }
            DateTime effectDate;
            if (!DateTime.TryParse(Request.Form[txtEffectDate.ClientID.Replace("_", "$")].Trim(), out effectDate))
            {
                Alert("输入合法日期");
                return;
            }
            TimeSpan day = effectDate - DateTime.Now;
            if (day.TotalDays <= 0)
            {
                Alert("生效日期必须大于当天日期");
                return;
            }

            string[] lineNos=CodLineNo.Split(',');
            if (lineNos == null || lineNos.Length <= 0)
            {
                Alert("没有找到需要新增的编号");
                return;
            }
            StringBuilder sbError = new StringBuilder();
            foreach (string lineNo in lineNos)
            {
                FMS_CODLine codLine = deliveryPriceService.GetListByCodLineNo(lineNo);
                GetBatchIsCOD(ref codLine);
                codLine.EffectDate = effectDate;
                codLine.CreateBy = Userid.ToString();
                codLine.AuditStatus = (int)EnumCODAudit.A2;
                codLine.DistributionCode = base.DistributionCode;
                
                int n = deliveryPriceService.AddEffectDeliveryPrice(codLine);
                if (n == 1)
                {
                    
                }
                else if (n == 0)
                    sbError.Append(codLine.CODLineNO + "已存在，直接操作 更改待生效价格;");
                else
                    sbError.Append(codLine.CODLineNO + "添加失败;");
            }

            if (string.IsNullOrEmpty(sbError.ToString()))
                RunJS("alert('添加成功');window.close();");
            else
                RunJS("alert('部分添加成功;以下添加失败;" + sbError.ToString() + "');window.close();");
        }

        private void BatchUpdateEffectCodLine()
        {
            if (ddlBatchType.SelectedValue == "0")
            {
                string codPriceStr = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
                string priceStr = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                if (codPriceStr == "")
                {
                    Alert("COD价格必填");
                    return;
                }
                if (priceStr == "")
                {
                    Alert("非COD价格必填");
                    return;
                }
            }
            else if (ddlBatchType.SelectedValue == "1")
            {
                string codPriceStr = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
                if (codPriceStr == "")
                {
                    Alert("COD价格必填");
                    return;
                }
            }
            else
            {
                string priceStr = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                if (priceStr == "")
                {
                    Alert("非COD价格必填");
                    return;
                }
            }
            DateTime effectDate;
            if (!DateTime.TryParse(Request.Form[txtEffectDate.ClientID.Replace("_", "$")].Trim(), out effectDate))
            {
                Alert("输入合法日期");
                return;
            }
            TimeSpan day = effectDate - DateTime.Now;
            if (day.TotalDays <= 0)
            {
                Alert("生效日期必须大于当天日期");
                return;
            }

            string[] lineNos = CodLineNo.Split(',');
            if (lineNos == null || lineNos.Length <= 0)
            {
                Alert("没有找到需要更新的编号");
                return;
            }
            StringBuilder sbError = new StringBuilder();
            foreach (string lineNo in lineNos)
            {
                FMS_CODLine codLine = deliveryPriceService.GetListByEffectCodLineNo(lineNo);
                GetBatchIsCOD(ref codLine);
                codLine.EffectDate = effectDate;
                codLine.CreateBy = Userid.ToString();
                codLine.AuditStatus = (int)EnumCODAudit.A2;
                codLine.DistributionCode = base.DistributionCode;
                codLine.UpdateBy = Userid.ToString();
                if (deliveryPriceService.UpdateEffectCodLine(codLine))
				{
				}
				else
					sbError.Append(codLine.CODLineNO + "更新失败;");  
            }

            if (string.IsNullOrEmpty(sbError.ToString()))
                RunJS("alert('更新成功');window.close();");
            else
                RunJS("alert('部分更新成功;以下更新失败;" + sbError.ToString() + "');window.close();");
        }

        private void GetBatchIsCOD(ref FMS_CODLine codLine)
        {
            if (ddlBatchType.SelectedValue == "0")
            {
                string codPriceStr = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
                string priceStr = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                if (codPriceStr == priceStr)
                    codLine.IsCOD = 0;
                else
                    codLine.IsCOD = 1;
                codLine.PriceFormula = codPriceStr;
                codLine.Formula = priceStr;
            }
            else if (ddlBatchType.SelectedValue == "1")
            {
                string codPriceStr = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
                if (codPriceStr == codLine.Formula)
                    codLine.IsCOD = 0;
                else
                    codLine.IsCOD = 1;
                codLine.PriceFormula = codPriceStr;
            }
            else
            {
                string priceStr = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                if (priceStr == codLine.PriceFormula)
                    codLine.IsCOD = 0;
                else
                    codLine.IsCOD = 1;
                codLine.Formula = priceStr;
            }
        }

        private void GetIsCOD(ref FMS_CODLine codLine)
        {
            codLine.IsCOD = rbIsCodFalse.Checked ? 0 : 1;
            string codPrice = Request.Form[txtCodPrice.ClientID.Replace("_", "$")].Trim();
            codLine.PriceFormula = codLine.Formula = codPrice;
            if (rbIsCodTrue.Checked)
            {
                string price = Request.Form[txtPrice.ClientID.Replace("_", "$")].Trim();
                codLine.Formula =  price;
            }
        }

        protected void rbIsCodTrue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIsCodTrue.Checked)
            {
                txtPrice.Visible = true;
                lbPrice.Visible = true;
                lbCodPrice.Text = "COD价格";
            }
            else
            {
                txtPrice.Visible = false;
                lbPrice.Visible = false;
                lbCodPrice.Text = "价格";
            }
        }

        protected void rbIsCodFalse_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIsCodFalse.Checked)
            {
                txtPrice.Visible = false;
                lbPrice.Visible = false;
                lbCodPrice.Text = "价格";
            }
            else
            {
                txtPrice.Visible = true;
                lbPrice.Visible = true;
                lbCodPrice.Text = "COD价格";
            }
        }

        protected void ddlBatchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlBatchType.SelectedValue)
            {
                case "0":
                    lbCodPrice.Visible = true;
                    lbPrice.Visible = true;
                    txtCodPrice.Visible = true;
                    txtPrice.Visible = true;
                    break;
                case "1":
                    lbCodPrice.Visible = true;
                    lbPrice.Visible = false;
                    txtCodPrice.Visible = true;
                    txtPrice.Visible = false;
                    break;
                case "2":
                    lbCodPrice.Visible = false;
                    lbPrice.Visible = true;
                    txtCodPrice.Visible = false;
                    txtPrice.Visible = true;
                    break;
                default:
                    break;
            }
        }
	}
}
