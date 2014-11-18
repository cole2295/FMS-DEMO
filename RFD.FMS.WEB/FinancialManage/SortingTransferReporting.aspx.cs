using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class SortingTransferReporting : BasePage
    {
        private readonly ISortingTransferDetailDao service = ServiceLocator.GetService<ISortingTransferDetailDao>();
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                StationList.DistributionCode = base.DistributionCode;
                DistributionList.DistributionCode = base.DistributionCode;
                SortingCenter.DistributionCode = base.DistributionCode;
                MerchantSource.DistributionCode = base.DistributionCode;

            }
            if (IsPostBack)
            {

                Pager1.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);

            }
        }
        private  SortingDetail InitModel()
        {
            SortingDetail model = new SortingDetail();
            model.SortingMerchantChk = true;
            model.CityChk = City.IsSelectAll;
            model.CityID = stringToStringArray(City.SelectCityID);
            model.MerchantChk = MerchantSource.IsSelectAll;
            model.MerchantID = stringToIntArray(MerchantSource.SelectMerchantSourcesID);
            model.waybillTypeChk = WaybillTypeDDL.SelectedValue == "-1";
            model.waybillType = WaybillTypeDDL.SelectedValue;
            model.StartTime = BeginTime.Text.Trim();
            model.EndTime = EndTime.Text.Trim();
            model.WaybillNo = string.IsNullOrEmpty(WaybillNOtxt.Text)
                                  ? -1
                                  : DataConvert.ToLong(WaybillNOtxt.Text.Trim());
            model.StationChk = StationList.IsSelectAll;
            model.StationID = stringToIntArray(StationList.SelectExpressCompany);

            model.DistributionChk = DistributionList.IsSelectAll;
            model.DistributionCode = DistributionList.SelectExpressCompany;

            model.SortingCenterChk = SortingCenter.IsSelectAll;
            model.SortingCenterID = stringToIntArray(SortingCenter.SelectExpressCompany);


            return model;
        }

        private List<int> stringToIntArray(string ss)
        {
            string[] sArray = ss.Split(new Char[] { ','});
            List<int> list = new List<int>();
            foreach (var s in sArray)
            {
                list.Add(DataConvert.ToInt(s));
            }
            return list;
        }
        private List<string> stringToStringArray(string ss)
        {
            string[] sArray = ss.Split(new Char[] { ',' });
            List<string> list = new List<string>();
            foreach (var s in sArray)
            {
                list.Add(s);
            }
            return list;
        }

        private  void SetGridViewVisibleAll()
        {
            GridView1.Columns[1].Visible = true;
            GridView1.Columns[2].Visible = true;
            GridView1.Columns[3].Visible = true;
            GridView1.Columns[4].Visible = true;
            GridView1.Columns[5].Visible = true;
            GridView1.Columns[6].Visible = true;
            GridView1.Columns[7].Visible = true;
            GridView1.Columns[8].Visible = true; 
            GridView1.Columns[9].Visible = true;
            GridView1.Columns[10].Visible = true;
            GridView1.Columns[11].Visible = true;
            GridView1.Columns[12].Visible = true;
            GridView1.Columns[13].Visible = true;
            GridView1.Columns[14].Visible = true;
        }
        protected void Searchbtn_Click(object sender, EventArgs e)
        {
            if(!SearchCheck())
                return;
            BuildPage();
        }

        private  bool SearchCheck()
        {
            if(SearchingItemDDL.SelectedValue == "-1")
            {
                Alert("请选择查询项目");
            }
            if(string.IsNullOrEmpty(BeginTime.Text) ||string.IsNullOrEmpty(BeginTime.Text) )
            { 
                Alert("请选择查询时间！");
                return false;
            }
            if(!City.IsSelectAll && string.IsNullOrEmpty(City.SelectCityID))
            {
                Alert("请选择城市！");
                return false;
            }
            if(!MerchantSource.IsSelectAll && string.IsNullOrEmpty(MerchantSource.SelectMerchantSourcesID))
            { 
                Alert("请选择商家");
                return false;
            }
            if(!StationList.IsSelectAll && string.IsNullOrEmpty(StationList.SelectExpressCompany))
            { 
                Alert("请选择站点");
                return false;
            }
            if(!SortingCenter.IsSelectAll && string.IsNullOrEmpty(SortingCenter.SelectExpressCompany))
            {
                Alert("请选择分拣中心");
                return false;
            }
            if(!DistributionList.IsSelectAll && string.IsNullOrEmpty(DistributionList.SelectExpressCompany))
            {
                Alert("请选择配送商");
                return false;
            }
            return true;
        }
        private  void BuildPage()
        {
            SetGridViewVisibleAll();
            var model = InitModel();
            DataTable dt = new DataTable();
            switch (SearchingItemDDL.SelectedValue)
            {
                case "-1": break;
                case "1":
                    GridView1.Columns[2].Visible = false;
                    GridView1.Columns[4].Visible = false;
                    GridView1.Columns[6].Visible = false;
                    GridView1.Columns[8].Visible = false;
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[11].Visible = false;
                     dt = service.GetTransferAndSortingToStationDetail(model);
                    break;
                case "2":
                    GridView1.Columns[5].Visible = false;
                    GridView1.Columns[3].Visible = false;
                    GridView1.Columns[8].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    GridView1.Columns[11].Visible = false;
                    dt = service.GetSortingToCityDetail(model);
                    break;
                case "3":
                    GridView1.Columns[2].Visible = false;
                    GridView1.Columns[4].Visible = false;
                    GridView1.Columns[6].Visible = false;
                    GridView1.Columns[8].Visible = false;
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[11].Visible = false;
                    dt = service.GetTransferAndSortingToStationDetail(model);
                
                    break;
                case "4":
                    GridView1.Columns[3].Visible = false;
                    GridView1.Columns[4].Visible = false;
                    GridView1.Columns[5].Visible = false;
                    GridView1.Columns[7].Visible = false;
                    GridView1.Columns[8].Visible = false;
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    dt = service.GetReturnToSortingCenter(model);
                    break;
                case "5":
                    GridView1.Columns[3].Visible = false;
                    GridView1.Columns[5].Visible = false;
                    GridView1.Columns[6].Visible = false;
                    GridView1.Columns[7].Visible = false;
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    GridView1.Columns[11].Visible = false;
                    dt = service.GetMerchantSortingToCenter(model);
                    break;
                default: break;
              
            }
            //分页处理
            Pager1.RecordCount = dt.Rows.Count;
            PagedDataSource pds = new PagedDataSource();
            pds.AllowPaging = true;
            pds.PageSize = Pager1.PageSize;
            pds.CurrentPageIndex = Pager1.CurrentPageIndex - 1;
            pds.DataSource = dt.DefaultView;

            GridView1.DataSource = pds;
            GridView1.DataBind();
        }
        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {

            BuildPage();

        }

        protected void Exportbtn_Click(object sender, EventArgs e)
        {

            var dt = GridViewHelper.GridView2DataTable(GridView1);
            string reportingTitle = "";
            switch (SearchingItemDDL.SelectedValue)
            {
                case "1":
                    reportingTitle = "运输运费明细表报表";
                    break;
                case "2":
                    reportingTitle = "分拣到城市明细表报表";
                    break;
                case "3":
                    reportingTitle = "分拣到站点明细表报表";
                    break;
                case "4":
                    reportingTitle = "逆向订单分拣费用明细报表";
                    break;
                case "5":
                    reportingTitle = "提货费用明细报表";
                    break;
                
            }      
            CSVExport.DataTable2Excel(dt, "配送费结算统计报表");
        }
    }
}