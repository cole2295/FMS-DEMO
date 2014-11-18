using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.SoringManage;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.WEB.SortingManage
{
    public partial class UpadeSortingFee : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
               
                    MerchantSource.DistributionCode = DistributionCode;
                    div1.Visible = false;
                    BuildDistribution();
                    BuildSortingCenter();
                    txtEffectDate.Text = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                   if(!string.IsNullOrEmpty(ID))
                   {
                       BindData(ID,0);
                       if(OpType == "1")
                       {
                           SortingCenterDDL.Enabled = false;
                           MerchantSource.SelectEnable = false;
                           SortingMerchantDDL.Enabled = false;
                           CitySelected.SelectEnable = false;
                           ItemTypeDLL.Enabled = false;
                           txtAccountFare.Enabled = false;
                       }
                   }
                if (!string.IsNullOrEmpty(WaitID))
                {
                    BindData(WaitID,1);

                }
              
               
            }
        }

        private void BindData(string ID,int type)
        {
            ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
            DataTable dt = new DataTable();
            if(type ==0)
            {
               dt = sortingFeeSrv.GetSortingFeeModel(ID);
            }
            else if(type == 1)
            {
                dt = sortingFeeSrv.GetSortingFeeWaitModel(ID);
            }
            
            SortingMerchantDDL.SelectedItem.Value = dt.Rows[0]["SortingMerchantID"].ToString();
            SortingMerchantDDL.SelectedItem.Text = dt.Rows[0]["SortingMerchant"].ToString();

            SortingCenterDDL.SelectedItem.Value = dt.Rows[0]["SortingCenterID"].ToString();
            SortingCenterDDL.SelectedItem.Text = dt.Rows[0]["SortingCenter"].ToString();

            CitySelected.SelectCityID = dt.Rows[0]["CityID"].ToString();
           

            MerchantSource.SelectMerchantSourcesID = dt.Rows[0]["MerchantID"].ToString();
           

            ItemTypeDLL.SelectedItem.Text = dt.Rows[0]["FareTypeStr"].ToString();
            ItemTypeDLL.SelectedItem.Value = dt.Rows[0]["FareType"].ToString();

            txtAccountFare.Text = dt.Rows[0]["AccountFare"].ToString();
        }
        public string OpType
        {
            get
            {
                return GetQueryString("OpType");
            }
        }

        public string ID
        {
            get { return GetQueryString("ID"); }
        }

        public  string WaitID
        {
            get { return GetQueryString("WaitID"); }
        }

        public  string IsChecked
        {
            get { return GetQueryString("IsChecked"); }
        }
        protected void ItemTypeDLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            div1.Visible = ItemTypeDLL.SelectedValue == "5";
        }

        private  void BuildDistribution()
        {
            IDistributionService distributionSrv = ServiceLocator.GetService<IDistributionService>();
            var dt=distributionSrv.GetDistribution(new Distribution());
           
            SortingMerchantDDL.DataTextField = "DistributionName";
            SortingMerchantDDL.DataValueField = "DistributionID";
            SortingMerchantDDL.DataSource = dt;
            SortingMerchantDDL.DataBind();
            SortingMerchantDDL.Items.Insert(0,new ListItem("请选择", "-1"));
        }

        private void BuildSortingCenter()
        {
            IExpressCompanyService ecSrv = ServiceLocator.GetService<IExpressCompanyService>();
            var dt=ecSrv.GetSortingList();
           
            SortingCenterDDL.DataTextField = "CompanyName";
            SortingCenterDDL.DataValueField = "ExpressCompanyID";
            SortingCenterDDL.DataSource = dt;
            SortingCenterDDL.DataBind();
            SortingCenterDDL.Items.Insert(0,new ListItem("请选择", "-1"));
        }
        private  List <FMS_SortingFeeModel> InitModels ()
        {
            List <FMS_SortingFeeModel> models = new List<FMS_SortingFeeModel>();
            IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                              
            string[] cityIds = CitySelected.SelectCityID.Split(',');
            string[] merchantIds = MerchantSource.SelectMerchantSourcesID.Split(',');
 
            for (int i = 0; i < cityIds.Length;i++)
            {
                for(int j=0; j<merchantIds.Length;j++)
                {

                    var model = new FMS_SortingFeeModel
                                    {
                                        SortingFeeID = iDGenerate.ServiceNewId("FMS_SortingFee", "SortingFeeID"),
                                        SortingMerchantID = DataConvert.ToInt(SortingMerchantDDL.SelectedValue),
                                        SortingCenterID = DataConvert.ToInt(SortingCenterDDL.SelectedValue),
                                        CityID = cityIds[i].Trim(),
                                        MerchantID = DataConvert.ToInt(merchantIds[j]),
                                        FareType = DataConvert.ToInt(ItemTypeDLL.SelectedValue),
                                        AccountFare = txtAccountFare.Text.Trim(),
                                        Status = 0,
                                        CreateBy = Userid,
                                        UpdateBy = Userid,
                                        IsDeleted = false,
                                        IsChange = false,
                                        EffectDate = Convert.ToDateTime(txtEffectDate.Text.Trim())

                                    };
                    if (DataConvert.ToInt(ItemTypeDLL.SelectedValue) == 5)
                    {
                        model.WaybillCount = DataConvert.ToInt(txtBillCount.Text.Trim());
                        model.IsAccountBill = IsAccountWaybill.Checked ? 1 : 0;
                    }

                    models.Add(model);
                }

            }
            return models;
        }

        private  FMS_SortingFeeModel Initmodel()
        {
            IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
            FMS_SortingFeeModel model = new FMS_SortingFeeModel
                                            {
                                                SortingFeeID = ID,
                                                SortingCenterID = DataConvert.ToInt(SortingCenterDDL.SelectedValue),
                                                SortingMerchantID = DataConvert.ToInt(SortingMerchantDDL.SelectedValue),
                                                CityID = CitySelected.SelectCityID,
                                                MerchantID = DataConvert.ToInt(MerchantSource.SelectMerchantSourcesID),
                                                AccountFare = txtAccountFare.Text.Trim(),
                                                FareType = DataConvert.ToInt(ItemTypeDLL.SelectedValue),
                                                UpdateBy = base.Userid,
                                                CreateBy = Userid,
                                                Status = 0,
                                                EffectDate =Convert.ToDateTime(txtEffectDate.Text.Trim())
                                            };
            if(OpType =="1")
            {
                model.SortingFeeWaitID = string.IsNullOrEmpty(WaitID)
                                       ? iDGenerate.ServiceNewId("FMS_SortingFeeWait", "SortingFeeWaitID")
                                       : WaitID;
            }
            return model;

        }
        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
                if (string.IsNullOrEmpty(OpType))
                {
                    if (!JudgeAdd())
                    {
                        return;
                    }
                    var models = InitModels();
                    string rl = string.Empty;
                    List<string> results = new List<string>();
                    foreach (var m in models)
                    {
                        results.Add(AddSoringFee(m));
                    }

                    foreach (var result in results)
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            rl += result + "\n";
                        }
                    }

                    if (string.IsNullOrEmpty(rl))
                    {
                        Alert("添加成功！");
                        Response.Redirect("SortingFee.aspx?IsChecked=" + IsChecked);
                    }
                    else
                    {
                        Alert(rl);
                    }
                }
                else if (OpType == "0")
                {


                    if (JudgeSelect())
                    {
                        var model = Initmodel();
                        int ret = sortingFeeSrv.UpdateSortingFee(model);
                        if (ret == 1)
                        {
                            Alert("更新成功！");
                        }
                        else
                        {
                            Alert("更新失败！");
                        }

                    }
                }
                else if (OpType == "1" && !string.IsNullOrEmpty(ID))
                {


                    if (JudgeSelect())
                    {
                        var model = Initmodel();
                        int ret = sortingFeeSrv.AddSortingFeeWait(model);
                        if (ret == 1)
                        {
                            Alert("添加成功！");
                        }
                        else
                        {
                            Alert("更新失败！");
                        }
                    }
                }
                else if (OpType == "1" && !string.IsNullOrEmpty(WaitID))
                {
                    if (JudgeSelect())
                    {
                        var model = Initmodel();
                        int ret = sortingFeeSrv.UpdateSortingFeeWait(model);
                        if (ret == 1)
                        {
                            Alert("更新成功！");
                        }
                        else
                        {
                            Alert("更新失败！");
                        }
                    }
                }

           
            }
            catch (Exception ex)
            {
                
               Alert("SqlServer未实现，请在demo下操作！");
            }
           

        }
        private  bool JudgeAdd()
        {
            string[] cityids = CitySelected.SelectCityID.Split(',');
            string[] merchantids = MerchantSource.SelectMerchantSourcesID.Split(',');
            if (cityids.Length < 1)
            {
                Alert("请选择城市");
                return false;
            }

            if (merchantids.Length < 1)
            {
                Alert("请选择商家");
                return false;
            }
            if (!Regex.IsMatch(txtAccountFare.Text.Trim(), @"(^\d+\.?\d{0,3})$"))
            {
                Alert("拣运费用不合规范！");
                return false;
            }

            if (Convert.ToDateTime(txtEffectDate.Text.Trim()) < Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")))
            {
                Alert("生效日期必须大于今天");
                return false;
            }

            if (SortingCenterDDL.SelectedValue == "-1")
            {
                Alert("请选择分拣中心");
                return false;

            }

            if (ItemTypeDLL.SelectedValue == "-1")
            {
                Alert("请选择项目类型");
                return false;
            }

            if (string.IsNullOrEmpty(txtAccountFare.Text))
            {
                Alert("拣运费不能为空");
                return false;
            }
            return true;
        }
        private bool JudgeSelect()
        {
            string[] cityids = CitySelected.SelectCityID.Split(',');
            string[] merchantids = MerchantSource.SelectMerchantSourcesID.Split(',');
            if(cityids.Length>1)
            {
                Alert("只能更新选择1个城市");
                return false;
            }

            if(merchantids.Length>1)
            {
                Alert("只能更新选择1个商家");
                return false;
            }
            if(!Regex.IsMatch(txtAccountFare.Text.Trim(), @"(^\d+\.?\d{0,3})$"))
            {
                Alert("拣运费用不合规范！");
                return false;
            }

            if(Convert.ToDateTime(txtEffectDate.Text.Trim())<Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")))
            {
                Alert("生效日期必须大于今天");
                return false;
            }

            if (SortingCenterDDL.SelectedValue == "-1")
            {
                Alert("请选择分拣中心");
                return false;

            }

            if(ItemTypeDLL.SelectedValue == "-1")
            {
                Alert("请选择项目类型");
                return false;
            }

            if(string.IsNullOrEmpty(txtAccountFare.Text))
            {
                Alert("拣运费不能为空");
                return false;
            }
            return true;
        }

        private string  AddSoringFee(FMS_SortingFeeModel model)
        {
            ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
            int ret = sortingFeeSrv.AddSortingFee(model);
            string result=string.Empty;
            if(ret == 0)
            {
                result = "拣运商" + SortingMerchantDDL.SelectedItem.Text
                         + "分拣中心：" + SortingCenterDDL.SelectedItem.Text
                         + "城市" + model.CityID + "减运费类型:" + ItemTypeDLL.SelectedItem.Text
                         + "已经存在，请更新";
            }
            if(ret == -1)
            {
                result = "拣运商" + SortingMerchantDDL.SelectedItem.Text
                         + "分拣中心：" + SortingCenterDDL.SelectedItem.Text
                         + "城市" + model.CityID + "减运费类型:" + ItemTypeDLL.SelectedItem.Text
                         + "添加失败";
            }

            return result;
        }

        protected void BackBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("SortingFee.aspx?IsChecked="+IsChecked);
        }
    }
}