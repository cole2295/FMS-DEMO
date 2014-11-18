using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using AutoMapper;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class SortingTransferSearching : BasePage
    {
        private readonly ISortingTransferSearchingService stss =
            ServiceLocator.GetService<ISortingTransferSearchingService>();

        private static DataTable table = new DataTable();
        private SortingDetail Model;
        private DataTable dt ;
        private static int flag = -1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                SortingCenter.DistributionCode = base.DistributionCode;
                MerchantSource.DistributionCode = base.DistributionCode;
                string sTime = DataConvert.ToDayBegin(DateTime.Now.AddDays(-1));
                string eTime = DataConvert.ToDayEnd(DateTime.Now.AddDays(-1));
                BeginTime.Text = sTime;
                EndTime.Text = eTime;
            }
            if (IsPostBack)
            {

                Pager1.PagerPageChanged += new EventHandler(AspNetPager_PageChanged);

            }
        }

        private void BuildPage()
        {
           try
           {
               int Count = 0;
               if (!SearchCheck())
                   return;
               InitModel();
               ISortingTransferDetailService detailService = ServiceLocator.GetService<ISortingTransferDetailService>();
               switch (SearchingItemDDL.SelectedValue)
               {
                   case "-1":
                       break;
                   case "1":
                       switch (flag)
                       {
                           case 0:
                               HiddenColumn(1, 0);
                               dt = stss.SortingTransferAndToStationAll(Model);
                               dt = sumCount(dt);
                               break;
                           case 1:
                               HiddenColumn(1, 1);
                               dt = stss.SortingTransferAndToStationDaily(Model);
                               dt = sumCount(dt);
                               break;
                           case 2:
                               HiddenColumn(1, 2);
                               dt = stss.SortingTransferAndToStationMerchant(Model);
                               dt = sumCount(dt);
                               break;
                           case 3:
                               HiddenColumn(1, 3);
                               Count = detailService.CountSortingTransferAndToStationDetail(Model);
                               dt = stss.SortingTransferAndToStationDetail(Model);
                               break;
                       }
                       break;
                   case "2":
                       switch (flag)
                       {
                           case 0:
                               HiddenColumn(2, 0);
                               dt = stss.SortingToCityAll(Model);
                               dt = sumCount(dt);
                               break;
                           case 1:
                               HiddenColumn(2, 1);
                               dt = stss.SortingToCityDaily(Model);
                               dt = sumCount(dt);
                               break;
                           case 2:
                               HiddenColumn(2, 2);
                               dt = stss.SortingToCityMerchant(Model);
                               dt = sumCount(dt);
                               break;
                           case 3:
                               HiddenColumn(2, 3);
                               Count = detailService.CountSortingToCityDetail(Model);
                               dt = stss.SortingToCityDetail(Model);
                               break;
                       }
                       break;
                   case "3":
                       switch (flag)
                       {
                           case 0:
                               HiddenColumn(3, 0);
                               dt = stss.SortingTransferAndToStationAll(Model);
                               dt = sumCount(dt);
                               break;
                           case 1:
                               HiddenColumn(3, 1);
                               dt = stss.SortingTransferAndToStationDaily(Model);
                               dt = sumCount(dt);
                               break;
                           case 2:
                               HiddenColumn(3, 2);
                               dt = stss.SortingTransferAndToStationMerchant(Model);
                               dt = sumCount(dt);
                               break;
                           case 3:
                               HiddenColumn(3, 3);
                               Count = detailService.CountSortingTransferAndToStationDetail(Model);
                               dt = stss.SortingTransferAndToStationDetail(Model);
                               break;
                       }
                       break;
                   case "4":
                       switch (flag)
                       {
                           case 0:
                               HiddenColumn(4, 0);

                               dt = stss.ReturnToSortingCenterAll(Model);
                               dt = sumCount(dt);
                               break;
                           case 1:
                               HiddenColumn(4, 1);

                               dt = stss.ReturnToSortingCenterDaily(Model);
                               dt = sumCount(dt);
                               break;
                           case 2:
                               HiddenColumn(4, 2);

                               dt = stss.ReturnToSortingCenterMerchant(Model);
                               dt = sumCount(dt);
                               break;
                           case 3:
                               HiddenColumn(4, 3);
                               Count = detailService.CountReturnToSortingCenterDetail(Model);
                              // Model.SortingCenterIDs = ConvertStringIDs(Model.SortingCenterIDs);
                               dt = stss.ReturnToSortingCenterDetail(Model);
                               break;
                       }
                       break;
                   case "5":
                       switch (flag)
                       {
                           case 0:
                               HiddenColumn(5, 0);
                               dt = stss.MerchantToSortingCenterAll(Model);
                               dt = sumCount(dt);
                               break;
                           case 1:
                               HiddenColumn(5, 1);
                               dt = stss.MerchantToSortingCenterDaily(Model);
                               dt = sumCount(dt);
                               break;
                           case 2:
                               HiddenColumn(5, 2);
                               dt = stss.MerchantToSortingCenterMerchant(Model);
                               dt = sumCount(dt);
                               break;
                           case 3:
                               HiddenColumn(5, 3);
                               Count = detailService.CountMerchantToSortingCenterDetail(Model);
                               dt = stss.MerchantToSortingCenterDetail(Model);
                               break;
                       }
                       break;
               }
               //分页处理
               table.Clear();
               table = dt;
               if (flag == 3)
                   Pager1.RecordCount = Count;
               else
                   Pager1.RecordCount = dt.Rows.Count;               
               PagedDataSource pds = new PagedDataSource();
               if(flag !=3)
                   pds.AllowPaging = true;
               pds.PageSize = Pager1.PageSize;
               pds.CurrentPageIndex = Pager1.CurrentPageIndex - 1;
               pds.DataSource = dt.DefaultView;

               GridView1.DataSource = pds;
               GridView1.DataBind();
               
           }catch(Exception ex)
           {
               Alert(ex.Message);
           }

         
        }

        private bool SearchCheck()
        {
            var EDate = DataConvert.ToDateTime(EndTime.Text);
            var SDate = DataConvert.ToDateTime(BeginTime.Text);
            var NDate = DateTime.Now;
            if (SearchingItemDDL.SelectedValue == "-1")
            {
                Alert("请选择查询项！");
                return false;
            }

            if (SDate > EDate)
            {
                Alert("开始时间大于结束时间！");
                return false;
            }

            if (EDate >
                DataConvert.ToDateTime(DataConvert.ToDayEnd(NDate))
                || SDate > DataConvert.ToDateTime(DataConvert.ToDayEnd(NDate)))
            {
                Alert("查询日期不能查过当天！");
                return false;
            }
            return true;
        }
        private void InitModel()
        {
            Model = new SortingDetail();
            Model.StartTime = BeginTime.Text.Trim();
            Model.EndTime = EndTime.Text.Trim();
            Model.SortingCenterIDs = string.IsNullOrEmpty(SortingCenter.SelectExpressCompany) ?
                                     Util.Common.ReadSortingCenterID : SortingCenter.SelectExpressCompany;
            //Model.MerchantIDs = MerchantSource.SelectMerchantSourcesID;
            Model.MerchantIDs = MerchantSource.IsSelectAll ? "" : MerchantSource.SelectMerchantSourcesID;
            //Model.CityIDs = ConvertStringIDs(CitySelected.SelectCityID);
            Model.CityIDs = CitySelected.IsSelectAll ? "" : ConvertStringIDs(CitySelected.SelectCityID);
            Model.DistributionCode = base.DistributionCode;
            Model.InSortingCount = Util.Common.ReadCount;
            Model.ItemType = DataConvert.ToInt(SearchingItemDDL.SelectedValue);
            Model.WaybillNo = -1;
            Model.waybillType = string.Empty;
            Model.startRowNum = (Pager1.CurrentPageIndex - 1) * 10 + 1;
            Model.endRowNum = (Pager1.CurrentPageIndex * 10);

        }

        private string ConvertStringIDs(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return string.Empty;
            string[] ss = ids.Split(',');
            string newIds = "";
            for (int i = 0; i < ss.Length;i++ )
            {
                newIds += "'" + ss[i].Trim() + "',";
            }
            newIds = newIds.Trim(',');
            return newIds;
        }

        protected void AspNetPager_PageChanged(object sender, EventArgs e)
        {

            BuildPage();

        }

        protected void DetailExport_Click(object sender, EventArgs e)
        {
            flag = 3;
            BuildPage();
        }

        private DataTable sumCount( DataTable dt)
        {
            DataTable newDt = new DataTable();
            //for(int i=0;i<dt.Columns.Count;i++)
            //{
            //    newDt.Columns.Add(dt.Columns[i].ColumnName);
            //}
            newDt = dt.Clone();
            int sum = 0;
            decimal sumFee = 0;
            for(int i =0;i<dt.Rows.Count;i++)
            {
                sum += DataConvert.ToInt(dt.Rows[i]["WaybillSum"]);
                sumFee += DataConvert.ToDecimal(dt.Rows[i]["Fee"]);
            }
            DataRow row = dt.NewRow();
            if (flag == 0)
                row["StatisticsType"] = "总计";
            else
                row["SDate"] = "合计";

            row["WaybillSum"] = sum.ToString();
            row["Fee"] = sumFee.ToString();
            newDt.Rows.Add(row.ItemArray);
            for (int i = 0; i < dt.Rows.Count;i++)
            {
                newDt.Rows.Add(dt.Rows[i].ItemArray);
            }
            dt.Clear();
            return newDt;
        }
        private void HiddenColumn(int item , int flag)
        {
            for (int i = 1; i < GridView1.Columns.Count;i++ )
            {
                GridView1.Columns[i].Visible = true;
            }
                switch (item)
                {
                    case 1:
                        switch (flag)
                        {
                            case 3:
                                GridView1.Columns[1].Visible = false;
                                GridView1.Columns[2].Visible = false;
                                GridView1.Columns[4].Visible = false;
                                GridView1.Columns[5].Visible = false;
                                GridView1.Columns[6].Visible = false;
                                GridView1.Columns[8].Visible = false;
                                GridView1.Columns[10].Visible = false;
                                GridView1.Columns[12].Visible = false;
                                GridView1.Columns[13].Visible = false;
                                GridView1.Columns[15].Visible = false;
                                GridView1.Columns[19].Visible = false;
                                GridView1.Columns[20].Visible = false;
                                GridView1.Columns[21].Visible = false;
                                GridView1.Columns[22].Visible = false;

                                break;
                            case 2:
                                HiddenForMerchant();
                                break;
                            case 1:
                                HiddenForDaily();
                                break;
                            case 0:
                                HiddenForAll();
                                break;
                        }
                        break;
                    case 2:
                        switch (flag)
                        {
                            case 0:
                                HiddenForAll();
                                break;
                            case 1:
                                HiddenForDaily();
                                break;
                            case 2:
                                HiddenForMerchant();
                                break;
                            case 3:
                                GridView1.Columns[1].Visible = false;
                                GridView1.Columns[2].Visible = false;
                                GridView1.Columns[4].Visible = false;
                                GridView1.Columns[5].Visible = false;
                                GridView1.Columns[7].Visible = false;
                                GridView1.Columns[9].Visible = false;
                                GridView1.Columns[11].Visible = false;
                                GridView1.Columns[12].Visible = false;
                                GridView1.Columns[14].Visible = false;
                                GridView1.Columns[15].Visible = false;
                                GridView1.Columns[19].Visible = false;
                                GridView1.Columns[20].Visible = false;
                                GridView1.Columns[21].Visible = false;
                                GridView1.Columns[22].Visible = false;
                                break;
                        }
                        break;
                    case 3:
                        switch (flag)
                        {
                            case 3:
                                GridView1.Columns[1].Visible = false;
                                GridView1.Columns[2].Visible = false;
                                GridView1.Columns[4].Visible = false;
                                GridView1.Columns[5].Visible = false;
                                GridView1.Columns[6].Visible = false;
                                GridView1.Columns[8].Visible = false;
                                GridView1.Columns[10].Visible = false;
                                GridView1.Columns[12].Visible = false;
                                GridView1.Columns[13].Visible = false;
                                GridView1.Columns[15].Visible = false;
                                GridView1.Columns[19].Visible = false;
                                GridView1.Columns[20].Visible = false;
                                GridView1.Columns[21].Visible = false;
                                GridView1.Columns[22].Visible = false;

                                break;
                            case 2:
                                HiddenForMerchant();
                                break;
                            case 1:
                                HiddenForDaily();
                                break;
                            case 0:
                                HiddenForAll();
                                break;
                        }
                        break;
                    case 4:
                        switch(flag)
                        {
                            case 0:  
                                HiddenForAll();
                                break;
                            case 1:
                                HiddenForDaily();
                                break;
                            case 2:
                                HiddenForMerchant();
                                break;
                            case 3:
                                GridView1.Columns[1].Visible = false;
                                GridView1.Columns[2].Visible = false;
                                GridView1.Columns[4].Visible = false;
                                GridView1.Columns[6].Visible = false;
                                GridView1.Columns[7].Visible = false;
                                GridView1.Columns[8].Visible = false;
                                GridView1.Columns[9].Visible = false;
                                GridView1.Columns[11].Visible = false;
                                GridView1.Columns[12].Visible = false;
                                GridView1.Columns[13].Visible = false;
                                GridView1.Columns[14].Visible = false;
                                GridView1.Columns[19].Visible = false;
                                GridView1.Columns[20].Visible = false;
                                GridView1.Columns[21].Visible = false;
                                GridView1.Columns[22].Visible = false;
                                break;
                        }
                        break;
                    case 5:
                        switch(flag)
                        {
                            case 0:
                                HiddenForAll();
                                break;
                            case 1:
                                HiddenForDaily();
                                break;
                            case 2:
                                HiddenForMerchant();
                                break;
                            case 3:
                                GridView1.Columns[1].Visible = false;
                                GridView1.Columns[2].Visible = false;
                                GridView1.Columns[4].Visible = false;
                                GridView1.Columns[6].Visible = false;
                                GridView1.Columns[7].Visible = false;
                                GridView1.Columns[9].Visible = false;
                                GridView1.Columns[10].Visible = false;
                                GridView1.Columns[11].Visible = false;
                                GridView1.Columns[13].Visible = false;
                                GridView1.Columns[14].Visible = false;
                                GridView1.Columns[15].Visible = false;
                                GridView1.Columns[19].Visible = false;
                                GridView1.Columns[20].Visible = false;
                                GridView1.Columns[21].Visible = false;
                                GridView1.Columns[22].Visible = false;
                                break;
                        }
                        break;
                }

        }
        private void HiddenForDaily()
        {
            GridView1.Columns[1].Visible = false;
            GridView1.Columns[5].Visible = false;
            GridView1.Columns[6].Visible = false;
            GridView1.Columns[7].Visible = false;
            GridView1.Columns[8].Visible = false;
            GridView1.Columns[9].Visible = false;
            GridView1.Columns[10].Visible = false;
            GridView1.Columns[11].Visible = false;
            GridView1.Columns[12].Visible = false;
            GridView1.Columns[13].Visible = false;
            GridView1.Columns[14].Visible = false;
            GridView1.Columns[15].Visible = false;
            GridView1.Columns[16].Visible = false;
            GridView1.Columns[17].Visible = false;
            GridView1.Columns[18].Visible = false;
        }

        private void HiddenForMerchant()
        {
            GridView1.Columns[1].Visible = false;
            GridView1.Columns[5].Visible = false;
            GridView1.Columns[6].Visible = false;
            GridView1.Columns[7].Visible = false;
            GridView1.Columns[8].Visible = false;
            GridView1.Columns[9].Visible = false;
            GridView1.Columns[10].Visible = false;
            GridView1.Columns[11].Visible = false;
            GridView1.Columns[12].Visible = false;
            GridView1.Columns[13].Visible = false;
            GridView1.Columns[14].Visible = false;
            GridView1.Columns[15].Visible = false;
            GridView1.Columns[16].Visible = false;
            GridView1.Columns[17].Visible = false;
           
        }

        private void HiddenForAll()
        {
            GridView1.Columns[2].Visible = false;
            GridView1.Columns[5].Visible = false;
            GridView1.Columns[6].Visible = false;
            GridView1.Columns[7].Visible = false;
            GridView1.Columns[8].Visible = false;
            GridView1.Columns[9].Visible = false;
            GridView1.Columns[10].Visible = false;
            GridView1.Columns[11].Visible = false;
            GridView1.Columns[12].Visible = false;
            GridView1.Columns[13].Visible = false;
            GridView1.Columns[14].Visible = false;
            GridView1.Columns[15].Visible = false;
            GridView1.Columns[16].Visible = false;
            GridView1.Columns[17].Visible = false;
            GridView1.Columns[18].Visible = false;
        }
        protected void DailyExport_Click(object sender, EventArgs e)
        {
            flag = 1;
            BuildPage();
        }

        protected void MerchantExport_Click(object sender, EventArgs e)
        {
            flag = 2;
            BuildPage();
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            flag = 0;
            BuildPage();
        }

        protected void Export_Click(object sender, EventArgs e)
        {

            try
            {
                string reportingTitle = "";
                switch (SearchingItemDDL.SelectedValue)
                {
                    case "1":
                        switch (flag)
                        {
                            case 0:
                                reportingTitle = "运输运费明报表";
                                break;
                            case 1:
                                reportingTitle = "运输运费明日统计报表";
                                break;
                            case 2:
                                reportingTitle = "运输运费明商家统计报表";
                                break;
                            case 3:
                                reportingTitle = "运输运费明细表报表";

                                break;
                        }

                        break;
                    case "2":
                        switch (flag)
                        {
                            case 0:
                                reportingTitle = "分拣到城市报表";
                                break;
                            case 1:
                                reportingTitle = "分拣到城市日统计报表";
                                break;
                            case 2:
                                reportingTitle = "分拣到城市商家统计报表";
                                break;
                            case 3:
                                reportingTitle = "分拣到城市明细报表";
                                break;
                        }

                        break;
                    case "3":
                        switch (flag)
                        {
                            case 0:
                                reportingTitle = "分拣到站点明报表";
                                break;
                            case 1:
                                reportingTitle = "分拣到站点明日统计报表";
                                break;
                            case 2:
                                reportingTitle = "分拣到站点明商家统计报表";
                                break;
                            case 3:
                                reportingTitle = "分拣到站点明细报表";
                                break;
                        }

                        break;
                    case "4":
                        switch (flag)
                        {
                            case 0:
                                reportingTitle = "逆向订单分拣费用报表";
                                break;
                            case 1:
                                reportingTitle = "逆向订单分拣费用日统计报表";
                                break;
                            case 2:
                                reportingTitle = "逆向订单分拣费用商家统计报表";
                                break;
                            case 3:
                                reportingTitle = "逆向订单分拣费用明细报表";
                                break;
                        }

                        break;
                    case "5":
                        switch (flag)
                        {
                            case 0:
                                reportingTitle = "提货费用报表";
                                break;
                            case 1:
                                reportingTitle = "提货费用日统计报表";
                                break;
                            case 2:
                                reportingTitle = "提货费用商家统计报表";
                                break;
                            case 3:
                                reportingTitle = "提货费用明细报表";
                                break;
                        }

                        break;

                }

                if (flag == 3) //明细导出
                {
                    ISortingTransferDetailService service = ServiceLocator.GetService<ISortingTransferDetailService>();
                    if (GridView1.Rows.Count == 0)
                    {
                        Alert("没有数据导出");
                    }
                    try
                    {
                        if (!SearchCheck())
                            return;
                        InitModel();

                        var asyncTaskHelper = new AsyncTaskHelper<DataTable>();
                        DataTable dt = null;
                        var startTime = DateTime.Parse(Model.StartTime);
                        var endTime = DateTime.Parse(Model.EndTime);
                        for (DateTime d = startTime; d < endTime; )
                        {
                            var newModel = new SortingDetail();
                            var ed = d.AddDays(1);
                            Model.StartTime = d.ToString();
                            if (ed < endTime)
                                Model.EndTime = DataConvert.ToDayEnd(ed);
                            else
                                Model.EndTime = endTime.ToString();
                            MapSortingDetail(Model, ref newModel);
                            switch (SearchingItemDDL.SelectedValue)
                            {
                                case "1":
                                    asyncTaskHelper.Add(d.ToString("yyyy-MM-dd"), () => service.ExportSortingTransferAndToStationDetail(newModel));
                                    break;
                                case "2":
                                    asyncTaskHelper.Add(d.ToString("yyyy-MM-dd"), () => service.ExportSortingToCityDetail(newModel));
                                    break;
                                case "3":
                                    asyncTaskHelper.Add(d.ToString("yyyy-MM-dd"), () => service.ExportSortingTransferAndToStationDetail(newModel));
                                    break;
                                case "4":
                                    asyncTaskHelper.Add(d.ToString("yyyy-MM-dd"), () => service.ExportReturnToSortingCenterDetail(newModel));
                                    break;
                                case "5":
                                    asyncTaskHelper.Add(d.ToString("yyyy-MM-dd"), () => service.ExportMerchantToSortingCenterDetail(newModel));
                                    break;
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

                }
                if (flag == 2)
                {
                    table.Columns["SDate"].ColumnName = "日期";
                    table.Columns["SortingCenterAll"].ColumnName = "分拣中心";
                    table.Columns["City"].ColumnName = "城市";
                    table.Columns["MerchantName"].ColumnName = "商家";
                    table.Columns["WaybillSum"].ColumnName = "单量";
                    table.Columns["SortingMerchantName"].ColumnName = "配送商";
                    table.Columns["Price"].ColumnName = "结算单价";
                    table.Columns["Fee"].ColumnName = "结算金额";
                    table.Columns.Remove("SoringMerchantID");
                    if (SearchingItemDDL.SelectedValue == "1" || SearchingItemDDL.SelectedValue == "3")
                        table.Columns.Remove("TSortingCenterID");
                    table.Columns.Remove("CityID");
                    table.Columns.Remove("MerchantID");
                    table.Columns.Remove("Code");
                    if (table == null || table.Rows.Count <= 0)
                    {
                        Alert("没有找到可以导出的数据");
                        return;
                    }
                    CSVExport.ExportFileToClient(table, reportingTitle, false, download_token_name.Value, download_token_value.Value);
                }

                if (flag == 1)
                {
                    table.Columns["SDate"].ColumnName = "日期";
                    table.Columns["SortingCenterAll"].ColumnName = "分拣中心";
                    table.Columns["City"].ColumnName = "城市";
                    table.Columns["WaybillSum"].ColumnName = "单量";
                    table.Columns["SortingMerchantName"].ColumnName = "配送商";
                    table.Columns["Price"].ColumnName = "结算单价";
                    table.Columns["Fee"].ColumnName = "结算金额";
                    table.Columns.Remove("SoringMerchantID");
                    table.Columns.Remove("Code");
                    if (table == null || table.Rows.Count <= 0)
                    {
                        Alert("没有找到可以导出的数据");
                        return;
                    }
                    CSVExport.ExportFileToClient(table, reportingTitle, false, download_token_name.Value, download_token_value.Value);
                }

                if (flag == 0)
                {
                    table.Columns["StatisticsType"].ColumnName = "统计类型";
                    table.Columns["SortingCenterAll"].ColumnName = "分拣中心";
                    table.Columns["City"].ColumnName = "城市";
                    table.Columns["WaybillSum"].ColumnName = "单量";
                    table.Columns["SortingMerchantName"].ColumnName = "配送商";
                    table.Columns["Price"].ColumnName = "结算单价";
                    table.Columns["Fee"].ColumnName = "结算金额";
                    table.Columns.Remove("SoringMerchantID");
                    table.Columns.Remove("Code");
                    if (table == null || table.Rows.Count <= 0)
                    {
                        Alert("没有找到可以导出的数据");
                        return;
                    }
                    CSVExport.ExportFileToClient(table, reportingTitle, false, download_token_name.Value, download_token_value.Value);
                }
            }
            catch (Exception ex)
            {
                Alert("导出失败<br>"+ex.Message);
            }
           
           
        }

        public void MapSortingDetail(SortingDetail info, ref SortingDetail newInfo)
        {
            Mapper.CreateMap<SortingDetail, SortingDetail>();
            newInfo = Mapper.Map(info, newInfo, info.GetType(), typeof(SortingDetail)) as SortingDetail;
        }
    }
}