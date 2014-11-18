using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Service.SoringManage;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;
using RFD.FMS.MODEL.FinancialManage;


namespace RFD.FMS.WEB.SortingManage
{
    public partial class SortingFee : BasePage
    {
       

       
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                WaitChk.Checked = IsChecked == "1";
                InitForm();
                BindData();
            }
            if (IsPostBack)
            {
                Pager1.PagerPageChanged += AspNetPager_PageChanged;    
            }
            
        }

        public string IsChecked
        {
            get { return GetQueryString("IsChecked"); }
        }
        private void BindData()
        {
            try
            {
               ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
               DataTable dt= new DataTable();

            gvSortingFee.Columns[17].Visible = !WaitChk.Checked;
            if(WaitChk.Checked)
            {
                
                dt = sortingFeeSrv.GetSortingFeeWait(InitModel());
            }
            else
            {

                dt = sortingFeeSrv.GetSortingFee(InitModel());
            }
         
            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add(dt.NewRow());
                gvSortingFee.DataSource = dt;
                gvSortingFee.DataBind();
                gvSortingFee.Rows[0].Cells.Clear();
                gvSortingFee.Rows[0].Cells.Add(new TableCell());
                gvSortingFee.Rows[0].Cells[0].ColumnSpan = gvSortingFee.Columns.Count;
                gvSortingFee.Rows[0].Cells[0].Text = "没有数据";
                gvSortingFee.Rows[0].Cells[0].Style.Add("text-align", "center");
            }
            else
            {
                Pager1.RecordCount = dt.Rows.Count;
                PagedDataSource pds = new PagedDataSource();
                pds.AllowPaging = true;
                pds.PageSize = Pager1.PageSize;
                pds.CurrentPageIndex = Pager1.CurrentPageIndex - 1;
                pds.DataSource = dt.DefaultView;
                gvSortingFee.DataSource = pds;
                gvSortingFee.DataBind();
            }
              
            }
            catch (Exception ex)
            {
                
                Alert("Sqlserver未实现，请在demo下进行操作");
            }
           
               
               
            
        }
        private void InitForm ()
        {

            gvSortingFee.Columns[17].Visible = !WaitChk.Checked;
            AddBtn.Enabled = !WaitChk.Checked;
            AddWaitBtn.Enabled = !WaitChk.Checked;
            SortingCenterSelect.DistributionCode = base.DistributionCode;
            DateTime sTime = DateTime.Now.AddDays(-8);
            DateTime eTime = DateTime.Now;
            txtBeginTime.Text = sTime.ToString("yyyy-MM-dd");
            txtEndTime.Text = eTime.ToString("yyyy-MM-dd");
        }

        private FMS_SortingFeeModel InitModel ()
        {
            var model = new FMS_SortingFeeModel();
            model.StartTime= Convert.ToDateTime(DataConvert.ToDayBegin(Convert.ToDateTime(txtBeginTime.Text.Trim())));
            model.EndTime =  Convert.ToDateTime(DataConvert.ToDayEnd(Convert.ToDateTime(txtEndTime.Text.Trim())));
            model.FareType = Convert.ToInt32(ItemDDL.SelectedValue);
            model.CreateBy = base.Userid;
            model.UpdateBy = base.Userid;
            model.AuditBy = base.Userid;

            model.SortingCenterIDs = SortingCenterSelect.SelectExpressCompany;
            model.SortingMerchantIDs = SortingMerchantSelected.SelectDistributionID;
            model.CityIDs = Dealstring(CitySelected.SelectCityID);
            model.Status = Convert.ToInt32(StatusDDL.SelectedValue);
            return model;
        }
       private  string  Dealstring(string s)
       {
           if (string.IsNullOrEmpty(s))
               return string.Empty;
           string[] ss = s.Split(',');
           string ret = "";
           for(int i= 0; i<ss.Length;i++)
           {
               ret += ",'"+ss[i].Trim() + "'";
           }
           return ret.Substring(1);
       }

       protected void AddBtn_Click(object sender, EventArgs e)
       {
           try
           {
               string url = WaitChk.Checked ? "UpadeSortingFee.aspx?IsChecked=1" : "UpadeSortingFee.aspx?IsChecked=0";
               Response.Redirect(url);
           }
           catch (Exception ex)
           {
               
               Alert("SqlServer未实现，请在demo下操作！");
           }
           
       }

       private void AspNetPager_PageChanged(object sender, EventArgs e)
       {
           BindData();
       }

       protected void SearchBtn_Click(object sender, EventArgs e)
       {
           AddBtn.Enabled = !WaitChk.Checked;
           AddWaitBtn.Enabled = !WaitChk.Checked;
           BindData();
       }


       private bool JudgeCheckList(int n, out IList<KeyValuePair<string, string>> checkList)
       {
           checkList = GridViewCheckList;
           if (checkList.Count <= 0)
           {
               Alert("至少选择一项操作");
               return false;
           }

           if (n == 1)
           {
               if (checkList.Count > 1)
               {
                   Alert("能且只能同步操作一项");
                   return false;
               }
               foreach (KeyValuePair<string, string> k in checkList)
               {

                 
                   if (k.Value != EnumHelper.GetDescription(SoringStatus.S3))
                   {
                       Alert("只能操作已生效项");
                       return false;
                   }
               }
           }

           return true;
       }

       public IList<KeyValuePair<string, string>> GridViewCheckList
       {
           get { return GridViewHelper.GetSelectedRows<string>(gvSortingFee, "cbCheck", 10); }
       }

       protected void gvSortingFee_RowCommand(object sender, GridViewCommandEventArgs e)
       {

           ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
           if(e.CommandName== "Change")
           {
               string OpType = WaitChk.Checked ? "1" : "0";
               string ID = e.CommandArgument.ToString();
               string url = WaitChk.Checked
                                ? "UpadeSortingFee.aspx?OpType=" + OpType + "&WaitID=" + ID+"&IsChecked=1"
                                : "UpadeSortingFee.aspx?OpType=" + OpType + "&ID=" + ID+"&IsChecked=0";

               Response.Redirect(url);
           }
           else if(e.CommandName == "Back")
           {
               FMS_SortingFeeModel model = new FMS_SortingFeeModel
                                               {
                                                  UpdateBy = Userid,
                                                  SortingFeeID =e.CommandArgument.ToString()
                                                
                                               };
               if(sortingFeeSrv.Delete(model)>0)
               {
                   Alert("已经置无效");
               }
               else
               {
                   Alert("置无效失败");
               }
              
               BindData();
           }
       }

       protected void AddWaitBtn_Click(object sender, EventArgs e)
       {
           try
           {
               ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
               IList<KeyValuePair<string, string>> checkList;
               if (!JudgeCheckList(1, out checkList))
                   return;
               if (sortingFeeSrv.ExsitSortingFeeWait(new FMS_SortingFeeModel()
               {
                   SortingFeeID = checkList[0].Key
               }))
               {
                   Alert("该待生效存在，请修改待生效！");
                   return;
               }
               string url = "UpadeSortingFee.aspx?OpType=1&ID=" + checkList[0].Key + "&IsChecked=0";
               Response.Redirect(url);
           }
           catch (Exception ex)
           {
               
               Alert("Sqlserver未实现，请在demo下操作！");
           }
        
       }

       protected void WaitChk_CheckedChanged(object sender, EventArgs e)
       {
           BindData();
       }

       
    }
}