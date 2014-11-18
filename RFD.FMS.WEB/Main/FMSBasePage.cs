using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web;
using System.Text;
using RFD.FMS.Util;
using RFD.FMS.Util.Cache;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.UserControl;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Service.AudiMgmt;

namespace RFD.FMS.WEB.Main
{
    public abstract class FMSBasePage : BasePage
    {
        protected IFinanceService service = ServiceLocator.GetService<IFinanceService>();
        private DataTable totalData = default(DataTable);
        private DataTable detailData = default(DataTable);

        #region 属性

        protected virtual DataTable TotalData
        {
            get
            {
                if (totalData.IsNullData())
                {
                    var condition = BuildSearchCondition(SearchType.Total);
                    //绑定汇总数据
                    var data = GetTotalData(condition);
                    totalData = data;
                }
                return totalData;
            }
            set
            {
                totalData = value;
            }
        }
        public virtual DataTable DetailsData
        {
            get
            {
                if (detailData.IsNullData())
                {
                    var condition = BuildSearchCondition(SearchType.Details);
                    if (condition == null)
                    {
                        detailData = null;
                    }
                    else
                    {
                        var data = GetDetailsData(condition);
                        detailData = data;
                    }
                }
                return detailData;
            }
            set
            {
                detailData = value;
            }
        }
        protected string TotalDataCacheKey
        {
            get
            {
                if (ViewState["TotalDataCacheKey"].IsNullData())
                {
                    ViewState["TotalDataCacheKey"] = Guid.NewGuid().ToString();
                }
                return ViewState["TotalDataCacheKey"].ToString();
            }
            set
            {
                ViewState["TotalDataCacheKey"] = value;
            }
        }
        protected string DetailsDataCacheKey
        {
            get
            {
                if (ViewState["DetailsDataCacheKey"].IsNullData())
                {
                    ViewState["DetailsDataCacheKey"] = Guid.NewGuid().ToString();
                }
                return ViewState["DetailsDataCacheKey"].ToString();
            }
            set
            {
                ViewState["DetailsDataCacheKey"] = value;
            }
        }
        /// <summary>
        /// 排序列
        /// </summary>
        protected string GridViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] != null &&
                    !String.IsNullOrEmpty(ViewState["SortExpression"].ToString()))
                    return ViewState["SortExpression"].ToString();
                return DefaultSortField;
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }
        /// <summary>
        /// 排序方向
        /// </summary>
        protected SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["SortDirection"] == null)
                    ViewState["SortDirection"] = DefaultSortDirection;
                return (SortDirection)ViewState["SortDirection"];
            }
            set
            {
                ViewState["SortDirection"] = value;
            }
        }
        /// <summary>
        /// 默认排序列
        /// </summary>
        protected virtual string DefaultSortField
        {
            get
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 默认排序方向
        /// </summary>
        protected virtual SortDirection DefaultSortDirection
        {
            get
            {
                return SortDirection.Descending;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 注册分页事件并显示分页控件
        /// </summary>
        protected void ShowPager(Pager pager, string recordCount)
        {
            var count = recordCount.ConvertToInt(0);
            pager.Visible = count > pager.PageSize;
        }
        /// <summary>
        /// 绑定支付方式
        /// </summary>
        /// <param name="control">绑定控件</param>
        protected void BindPayType(ListControl control)
        {
            var data = EnumHelper.GetEnumValueAndDescriptions<PaymentType>();
            control.BindListData(data);
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        protected void ClearCache(SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.Total:
                    TotalData = null;
                    DetailsData = null;
                    //TotalDataCacheKey = null;
                    //DetailsDataCacheKey = null;
                    break;
                case SearchType.Details:
                    DetailsData = null;
                    //DetailsDataCacheKey = null;
                    break;
            }
        }
        /// <summary>
        /// 显示提示文本
        /// </summary>
        /// <param name="searchType">搜索类型</param>
        /// <param name="condition">搜索条件</param>
        /// <returns></returns>
        protected virtual string ShowTipText(SearchType searchType, SearchCondition condition)
        {
            var tip = new StringBuilder();
            if (!condition.IsNullData())
            {
                switch (searchType)
                {
                    case SearchType.Total:
                        if (condition.DeliverStation != 0)
                        {
                            tip.AppendFormat("<font color='red'><b> {0}</b></font>: ", condition.StationName);
                        }
                        if (condition.Source != -1)
                        {
                            tip.AppendFormat(" 来源为<font color='blue'><b>{0}</b></font>", EnumHelper.GetDescription((WaybillSourse)condition.Source));
                        }
                        if (condition.MerchantID != 0)
                        {
                            tip.AppendFormat(" 商家为<font color='blue'><b>{0}</b></font>", condition.MerchantName);
                        }
                        if (condition.PayType != -1)
                        {
                            tip.AppendFormat(" 支付方式为<font color='blue'><b>{0}</b></font>", EnumHelper.GetDescription((PaymentType)condition.PayType));
                        }
                        if (!condition.BeginTime.IsNullData() && !condition.EndTime.IsNullData())
                        {
                            tip.AppendFormat("<font color='blue'><b> [{0}]</b></font>至<font color='blue'><b>[{1}]</b></font>的", condition.BeginTime.ToString("yyyy-MM-dd"), condition.EndTime.ToString("yyyy-MM-dd"));
                        }
                        tip.Append("汇总数据: ");
                        break;
                    case SearchType.Details:
                        if (!condition.StationName.IsNullData())
                        {
                            tip.AppendFormat("<font color='red'><b> {0}</b></font>: ", condition.StationName);
                        }
                        if (!condition.MerchantName.IsNullData())
                        {
                            tip.AppendFormat(" 来源为<font color='blue'><b>{0}</b></font>", condition.MerchantName);
                        }
                        if (condition.PayType != -1)
                        {
                            tip.AppendFormat(" 支付方式为<font color='blue'><b>{0}</b></font>", EnumHelper.GetDescription((PaymentType)condition.PayType));
                        }
                        if (!condition.BeginTime.IsNullData())
                        {
                            tip.AppendFormat(" <font color='blue'><b> [{0}]</b></font>的", condition.BeginTime.ToString("yyyy-MM-dd"));
                        }
                        tip.Append("明细数据: ");
                        break;
                }
            }
            return tip.ToString();
        }
        /// <summary>
        /// 对GirdView排序
        /// </summary>
        /// <param name="condition">搜索条件</param>
        protected void Sorting(SearchCondition condition)
        {
            if (GridViewSortDirection == SortDirection.Descending)
            {
                GridViewSortDirection = SortDirection.Ascending;
                condition.Direction = "ASC";
            }
            else
            {
                GridViewSortDirection = SortDirection.Descending;
                condition.Direction = "DESC";
            }
            //绑定汇总数据
            TotalData = GetTotalData(condition);
            BindTotalData();
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="searchType">搜索类别</param>
        protected void Search(SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.Total:
                    BindTotalData();
                    break;
                case SearchType.AccountPeriodTotal:
                    BindTotalData();
                    break;
                case SearchType.Details:
                    BindDetailsData();
                    break;
            }
        }
        /// <summary>
        /// 根据导出文件格式(Excel,PDF)导出数据
        /// </summary>
        /// <param name="format">导出格式</param>
        /// <param name="searchType">搜索类别</param>
        /// <param name="data">导出的数据源</param>
        protected virtual void Export(ExportFileFormat format, SearchType searchType, DataTable data)
        {
            var columns = GetExportGridViewHeaders(searchType);
            var title = GetExportTitle(searchType);
            var ignoreColumns = GetExportIgnoreColumns(searchType);
            var fixColumns = GetExportFixColumns(searchType);
            //对导出所有明细做单独处理
            if (searchType == SearchType.AllDetails || searchType == SearchType.AccountPeriodAllDetails)
            {
                //不使用缓存
                data = GetAllDetailsData(BuildSearchCondition(searchType));
                if (data.IsEmpty())
                {
                    Alert("当前时间段无明细数据！");
                    return;
                }
            }
            if (!data.IsEmpty())
            {
                try
                {
                    switch (format)
                    {
                        case ExportFileFormat.Excel:
                            ExportExcel(data, columns, ignoreColumns, title);
                            break;
                        case ExportFileFormat.PDF:
                            ExportPDF(data, columns, ignoreColumns, title);
                            break;
                        case ExportFileFormat.CSV:
                            if (fixColumns != null)
                                ExportCSV(data, columns, ignoreColumns, title, fixColumns);
                            else
                                ExportCSV(data, columns, ignoreColumns, title);
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Alert(ex.Message);
                    return;
                }
            }
        }
        protected virtual SearchCondition BuildSearchCondition(SearchType searchType) { return null; }
        protected virtual DataTable GetTotalData(SearchCondition condition) { return null; }
        protected virtual DataTable GetDetailsData(SearchCondition condition) { return null; }
        protected virtual DataTable GetAllDetailsData(SearchCondition condition) { return null; }
        protected virtual void BindTotalData() { }
        protected virtual void BindDetailsData() { }
        protected virtual void HideGridviewColumn(SearchType searchType) { }
        protected virtual void ShowGridviewColumn(SearchType searchType) { }
        protected virtual void AspNetPager_PageChanged(object sender, EventArgs e) { }
        protected virtual void ReloadScript(LoadScriptType type) { }
        protected virtual void RegisterPager() { }
        protected virtual string[] GetExportGridViewHeaders(SearchType searchType) { return null; }
        protected virtual string GetExportTitle(SearchType searchType) { return string.Empty; }
        protected virtual string[] GetExportIgnoreColumns(SearchType searchType) { return null; }
        protected virtual string[] GetExportFixColumns(SearchType searchType) { return null; }
        #endregion
    }
}
