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
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;
using AutoMapper;
using RFD.FMS.WEB.UserControl;

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class SortingTransferDetail : BasePage
    {
        private readonly ISortingTransferDetailService service = ServiceLocator.GetService<ISortingTransferDetailService>();
        private string sDate = DataConvert.ToDayBegin(DateTime.Now.AddDays(-1));
        private string eDate = DataConvert.ToDayEnd(DateTime.Now.AddDays(-1));
        private SortingDetail model;
        private static DataTable table =new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                StationList.DistributionCode = base.DistributionCode;
                DistributionList.DistributionCode = base.DistributionCode;
                SortingCenter.DistributionCode = base.DistributionCode;
                MerchantSource.DistributionCode = base.DistributionCode;
       
               
                BeginTime.Text = sDate;
                EndTime.Text = eDate;
            }
            if (IsPostBack)
            {

                Pager1.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);

            }
        }
        private  void InitModel()
        {
            model = new SortingDetail();
            IDistributionService distributionSrv = ServiceLocator.GetService<IDistributionService>();
      
            //model.CityIDs = ConvertStringIDs(City.SelectCityID);
            model.CityIDs = City.IsSelectAll ? "" : ConvertStringIDs(City.SelectCityID);
            model.StartTime = BeginTime.Text.Trim();
            model.EndTime = EndTime.Text.Trim();
            model.StationIDs = StationList.SelectExpressCompany;
            //model.MerchantIDs = MerchantSource.SelectMerchantSourcesID;

            model.MerchantIDs = MerchantSource.IsSelectAll ? "" : MerchantSource.SelectMerchantSourcesID;
            model.WaybillNo = string.IsNullOrEmpty(WaybillNOtxt.Text.Trim())
                                  ? -1
                                  : DataConvert.ToLong(WaybillNOtxt.Text.Trim());
            model.SortingCenterIDs = !string.IsNullOrEmpty(SortingCenter.SelectExpressCompany)?
                SortingCenter.SelectExpressCompany : Util.Common.ReadSortingCenterID;
            model.DistributionCodes = DistributionList.SelectExpressCompany;
            model.waybillType = WaybillTypeDDL.SelectedValue == "-1" ? string.Empty:
                                WaybillTypeDDL.SelectedValue;
            model.InSortingCount = Util.Common.ReadCount;
            model.DistributionCode = base.DistributionCode;
            model.startRowNum = (Pager1.CurrentPageIndex - 1)*10 + 1;
            model.endRowNum = (Pager1.CurrentPageIndex*10);

        }


        private string ConvertStringIDs(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return string.Empty;
            string[] ss = ids.Split(',');
            string newIds = "";
            for (int i = 0; i < ss.Length; i++)
            {
                newIds += "'" + ss[i] + "',";
            }
            newIds = newIds.Trim(',');
            return newIds;
        }
        private  void SetGridViewVisibleAll()
        {
            for (int i = 1; i < GridView1.Columns.Count; i++)
                GridView1.Columns[i].Visible = true;
        }
        protected void Searchbtn_Click(object sender, EventArgs e)
        {
            BuildPage();
        }
        
        private bool SearchCheck()
        {
            var EDate = DataConvert.ToDateTime(EndTime.Text);
            var SDate = DataConvert.ToDateTime(BeginTime.Text);
            var NDate = DateTime.Now;
            if(SearchingItemDDL.SelectedValue == "-1")
            {
                Alert("请选择查询项！");
                return false;
            }
            
            if(SDate> EDate )
            {
                Alert("开始时间大于结束时间！");
                return false;
            }

            if (EDate >
                DataConvert.ToDateTime(DataConvert.ToDayEnd(NDate))
                ||  SDate >DataConvert.ToDateTime(DataConvert.ToDayEnd(NDate))  )
            {
                Alert("查询日期不能查过当天！");
                return false;
            }
            return true;
        }
        private  void BuildPage()
        {
            if(!SearchCheck())
                return;
            SetGridViewVisibleAll();
            InitModel();
            DataTable dt = new DataTable();
            int Count = 0;
            switch (SearchingItemDDL.SelectedValue)
            {
                case "-1": break;
                case "1":
                    GridView1.Columns[2].Visible = false;
                    GridView1.Columns[3].Visible = false;
                    GridView1.Columns[5].Visible = false;
                    GridView1.Columns[7].Visible = false;
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    GridView1.Columns[12].Visible = false;
                    Count = service.CountSortingTransferAndToStationDetail(model);
                    if(Count>0)
                    {
                        dt = service.SortingTransferAndToStationDetail(model);
                    }
                    break;
                case "2":
                    GridView1.Columns[6].Visible = false;
                    GridView1.Columns[2].Visible = false;
                    GridView1.Columns[4].Visible = false;
                    GridView1.Columns[9].Visible = false;
                   // GridView1.Columns[8].Visible = false;
                    GridView1.Columns[11].Visible = false;
                    GridView1.Columns[12].Visible = false;
                    Count = service.CountSortingToCityDetail(model);
                    if (Count>0)
                    {
                        dt = service.SortingToCityDetail(model);
                    }
                   
                    break;
                case "3":
                    GridView1.Columns[2].Visible = false;
                    GridView1.Columns[3].Visible = false;
                    GridView1.Columns[5].Visible = false;
                    GridView1.Columns[7].Visible = false;
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    GridView1.Columns[12].Visible = false;
                    Count = service.CountSortingTransferAndToStationDetail(model);
                    if ( Count> 0)
                    {
                        dt = service.SortingTransferAndToStationDetail(model);
                    }
                
                    break;
                case "4":
                    GridView1.Columns[3].Visible = false;
                    GridView1.Columns[4].Visible = false;
                    GridView1.Columns[5].Visible = false;
                    GridView1.Columns[6].Visible = false;
                   // GridView1.Columns[8].Visible = false;
                    GridView1.Columns[9].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    GridView1.Columns[11].Visible = false;
                    Count = service.CountReturnToSortingCenterDetail(model);
                    if ( Count> 0)
                    {
                        dt = service.ReturnToSortingCenterDetail(model);
                    }
                    break;
                case "5":
                    GridView1.Columns[3].Visible = false;
                    GridView1.Columns[4].Visible = false;
                    GridView1.Columns[6].Visible = false;
                    GridView1.Columns[7].Visible = false;
                   // GridView1.Columns[8].Visible = false;
                    GridView1.Columns[10].Visible = false;
                    GridView1.Columns[11].Visible = false;
                    GridView1.Columns[12].Visible = false;
                    Count = service.CountMerchantToSortingCenterDetail(model);
                    if(Count >0)
                    {
                        dt = service.MerchantToSortingCenterDetail(model);
                    }
                    break;
                default: break;
              
            }
            table.Clear();
            table = dt;
            //分页处理
            Pager1.RecordCount = Count;
            PagedDataSource pds = new PagedDataSource();
           // pds.AllowPaging = true;
            pds.PageSize = Pager1.PageSize;
            pds.CurrentPageIndex = Pager1.CurrentPageIndex-1;
            pds.DataSource = dt.DefaultView;

            GridView1.DataSource = pds;
            GridView1.DataBind();
        }
        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {

            BuildPage();

        }

        public void MapSortingDetail(SortingDetail info, ref SortingDetail newInfo)
        {
            Mapper.CreateMap<SortingDetail, SortingDetail>();
            newInfo = Mapper.Map(info, newInfo, info.GetType(), typeof(SortingDetail)) as SortingDetail;
        }

        protected void Exportbtn_Click(object sender, EventArgs e)
        {

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

            try
            {
                if (!SearchCheck())
                    return;
                InitModel();
                         
                var asyncTaskHelper = new AsyncTaskHelper<DataTable>();
                DataTable dt = null;
                var startTime =DateTime.Parse(model.StartTime);
                var endTime = DateTime.Parse(model.EndTime);
                var sortList = model.SortingCenterIDs.Split(',');
                for (DateTime d = startTime; d < endTime; )
                {
                    var ed = d.AddDays(2);//天 单位 n+1
                    foreach (string sort in sortList)
                    {
                        var newModel = new SortingDetail();
                        model.SortingCenterIDs = sort;//分拣中心拆分维度
                        model.StartTime = d.ToString();//时间拆分维度
                        if (ed < endTime)
                            model.EndTime = DataConvert.ToDayEnd(ed);
                        else
                            model.EndTime = endTime.ToString();
                        MapSortingDetail(model, ref newModel);
                        string taskName = d.ToString("yyyy-MM-dd") + "_" + sort + "_" + SearchingItemDDL.SelectedValue;
                        switch (SearchingItemDDL.SelectedValue)
                        {
                            case "1":
                                asyncTaskHelper.Add(taskName, () => service.ExportSortingTransferAndToStationDetail(newModel));
                                break;
                            case "2":
                                asyncTaskHelper.Add(taskName, () => service.ExportSortingToCityDetail(newModel));
                                break;
                            case "3":
                                asyncTaskHelper.Add(taskName, () => service.ExportSortingTransferAndToStationDetail(newModel));
                                break;
                            case "4":
                                asyncTaskHelper.Add(taskName, () => service.ExportReturnToSortingCenterDetail(newModel));
                                break;
                            case "5":
                                asyncTaskHelper.Add(taskName, () => service.ExportMerchantToSortingCenterDetail(newModel));
                                break;
                        }
                    }

                    d = DateTime.Parse(DataConvert.ToDayBegin(ed.AddDays(1)));
                }
                dt = asyncTaskHelper.RunAllAndMerge() as DataTable;
                if (dt == null || dt.Rows.Count <= 0)
                {
                    Alert("没有找到可以导出的数据");
                    return;
                }
                CSVExport.ExportFileToClient(dt, reportingTitle, false, download_token_name.Value, download_token_value.Value);
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>" + ex.Message);
            }
           // CSVExport.DataTable2Excel(table, reportingTitle);
        }
    }
}