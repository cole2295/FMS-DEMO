using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.SoringManage;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.SortingManage
{
    public partial class SortingFeeAudit : BasePage
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                InitForm();
                BindData();
            }

            else
            {
                Pager1.PagerPageChanged += AspNetPager_PageChanged;
            }
        }

         private void InitForm ()
        {
            
            DateTime sTime = DateTime.Now.AddDays(-7);
            DateTime eTime = DateTime.Now;
            txtBeginTime.Text = sTime.ToString("yyyy-MM-dd");
            txtEndTime.Text = eTime.ToString("yyyy-MM-dd");
        }
        private  void BindData()
        {
            try
            {
                ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
                DataTable dt = new DataTable();
                if (WaitChk.Checked)
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
                    gvSortingFeeAudit.DataSource = dt;
                    gvSortingFeeAudit.DataBind();

                    gvSortingFeeAudit.Rows[0].Cells.Clear();
                    gvSortingFeeAudit.Rows[0].Cells.Add(new TableCell());
                    gvSortingFeeAudit.Rows[0].Cells[0].ColumnSpan = gvSortingFeeAudit.Columns.Count;
                    gvSortingFeeAudit.Rows[0].Cells[0].Text = "没有数据";
                    gvSortingFeeAudit.Rows[0].Cells[0].Style.Add("text-align", "center");
                }
                else
                {
                    Pager1.RecordCount = dt.Rows.Count;
                    PagedDataSource pds = new PagedDataSource();
                    pds.AllowPaging = true;
                    pds.PageSize = Pager1.PageSize;
                    pds.CurrentPageIndex = Pager1.CurrentPageIndex - 1;
                    pds.DataSource = dt.DefaultView;

                    gvSortingFeeAudit.DataSource = pds;
                    gvSortingFeeAudit.DataBind();
                }
           
            }
            catch (Exception ex)
            {
                
               Alert("sqlServer未实现，请到demo下操作！");
            }
          

        }
        protected void AuditBtn_Click(object sender, EventArgs e)
        {
            ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
            var checkList =GridViewCheckList;
            IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
            if(checkList.Count == 0)
            {
                Alert("至少选择一项");
                return;
            }
            foreach (var p in checkList)
            {
                if (p.Value != EnumHelper.GetDescription(SoringStatus.S0))
                {
                    Alert("只能选择待审核项");
                    return;
                }
            }

            foreach (var p in checkList)
            {
                if (WaitChk.Checked)
                {

                    var model = sortingFeeSrv.GetSmallSortingFeeWaitModel(p.Key);
                    model.Status = 1;
                    model.AuditBy = Userid;
                    model.UpdateBy = Userid;

                    int ret = sortingFeeSrv.AuditSortingFeeWait(model);
                    if (ret == 1)
                    {
                        Alert("审核成功！");
                    }
                    else
                    {
                        Alert("审核失败！");
                    }


                }
                else
                {
                    var model = sortingFeeSrv.GetSmallSortingFeeModel(p.Key);
                    model.CreateBy = Userid;
                    model.UpdateBy = Userid;
                    model.AuditBy = Userid;
                    model.SortingFeeWaitID = iDGenerate.ServiceNewId("FMS_SortingFeeWait", "SortingFeeWaitID");
                    int ret = sortingFeeSrv.AuditSortingFee(model);
                    if (ret == 1)
                    {
                        Alert("审核成功！");
                    }
                    else
                    {
                        Alert("审核失败！");
                    }
                }
                

            }
            BindData();
        }

        private FMS_SortingFeeModel InitModel()
        {
            var model = new FMS_SortingFeeModel();
            model.StartTime = Convert.ToDateTime(DataConvert.ToDayBegin(Convert.ToDateTime(txtBeginTime.Text.Trim())));
            model.EndTime = Convert.ToDateTime(DataConvert.ToDayEnd(Convert.ToDateTime(txtEndTime.Text.Trim())));
            model.FareType = Convert.ToInt32(ItemDDL.SelectedValue);
            model.CreateBy = base.Userid;
            model.UpdateBy = base.Userid;
            model.AuditBy = base.Userid;
            model.Status = Convert.ToInt32(StatusDDL.SelectedValue);
            return model;
        }

        private void AspNetPager_PageChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void gvSortingFeeAudit_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
            IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
           
            if (e.CommandName == "Audit")
            {
                if (WaitChk.Checked)
                {

                    var model = sortingFeeSrv.GetSmallSortingFeeWaitModel(e.CommandArgument.ToString());
                    model.Status = 1;
                    model.AuditBy = Userid;
                    model.UpdateBy = Userid;
               
                    int ret = sortingFeeSrv.AuditSortingFeeWait(model);
                    if (ret == 1)
                    {
                        Alert("审核成功！");
                    }
                    else
                    {
                        Alert("审核失败！");
                    }


                }
                else
                {
                    var model = sortingFeeSrv.GetSmallSortingFeeModel(e.CommandArgument.ToString());
                    model.CreateBy = Userid;
                    model.UpdateBy = Userid;
                    model.AuditBy = Userid;
                    model.SortingFeeWaitID = iDGenerate.ServiceNewId("FMS_SortingFeeWait", "SortingFeeWaitID");
                    int ret = sortingFeeSrv.AuditSortingFee(model);
                    if(ret == 1)
                    {
                        Alert("审核成功！");
                    }
                    else
                    {
                        Alert("审核失败！");
                    }
                }
                
            }
            else if (e.CommandName == "Back")
            {

                if (WaitChk.Checked)
                {
                    var model = new FMS_SortingFeeModel()
                                    {
                                        SortingFeeWaitID = e.CommandArgument.ToString(),
                                        CreateBy = Userid,
                                        UpdateBy = Userid,
                                        AuditBy =  Userid
                                    };
                    int ret = sortingFeeSrv.BackSortingFeeWait(model);
                    if (ret == 1)
                    {
                        Alert("置回成功！");
                    }
                    else
                    {
                        Alert("置回失败！");
                    }
                }
                else
                {
                    var model = sortingFeeSrv.GetSmallSortingFeeModel(e.CommandArgument.ToString());
                    model.CreateBy = Userid;
                    model.UpdateBy = Userid;
                    model.AuditBy = Userid;
                    int ret = sortingFeeSrv.BackSortingFee(model);
                    if (ret == 1)
                    {
                        Alert("置回成功！");
                    }
                    else
                    {
                        Alert("置回失败！");
                    }
                }
            }

            BindData();
        }

        protected void SearchBtn_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void WaitChk_CheckedChanged(object sender, EventArgs e)
        {
         
            BindData();
            
        }

        public IList<KeyValuePair<string, string>> GridViewCheckList
        {
            get { return GridViewHelper.GetSelectedRows<string>(gvSortingFeeAudit, "cbCheck", 10); }
        }

        protected void BackBtn_Click(object sender, EventArgs e)
        {
            ISortingFeeService sortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
            var checkList = GridViewCheckList;
            IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
            if (checkList.Count == 0)
            {
                Alert("至少选择一项");
                return;
            }
            
            foreach (var p in checkList)
            {
                 if (WaitChk.Checked)
                {
                    var model = new FMS_SortingFeeModel()
                                    {
                                        SortingFeeWaitID = p.Key,
                                        CreateBy = Userid,
                                        UpdateBy = Userid,
                                        AuditBy =  Userid
                                    };
                    int ret = sortingFeeSrv.BackSortingFeeWait(model);
                    if (ret == 1)
                    {
                        Alert("置回成功！");
                    }
                    else
                    {
                        Alert("置回失败！");
                    }
                }
                else
                {
                    var model = sortingFeeSrv.GetSmallSortingFeeModel(p.Key);
                    model.CreateBy = Userid;
                    model.UpdateBy = Userid;
                    model.AuditBy = Userid;
                    int ret = sortingFeeSrv.BackSortingFee(model);
                    if (ret == 1)
                    {
                        Alert("置回成功！");
                    }
                    else
                    {
                        Alert("置回失败！");
                    }
                }
            }

            BindData();
         }
        
       
    }
}