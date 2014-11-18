using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using System.ComponentModel;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Model.UnionPay;
using RFD.FMS.Util;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Domain.AudiMgmt;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.WEBLOGIC.AudiMgmt
{
	[DataObject]
	public class FinanceService : RFD.FMS.Service.AudiMgmt.IFinanceService 
	{
        private IFinanceDao _financeDao;

		#region 常量
		private static readonly string WAYBILL_SOURCE = "Sources";
		private static readonly string WAYBILL_NO = "WayBillNo";
		private static readonly string SOURCE_NAME = "SourceName";
		private static readonly string CASH_SUM_AMOUNT = "CashRealInSum";
		private static readonly string POS_SUM_AMOUNT = "PosRealInSum";
		private static readonly string ACCEPT_AMOUNT = "AcceptAmount";
		private static readonly string BACK_AMOUNT = "CashRealOutSum";
		private static readonly string SAVE_AMOUNT = "SaveAmount";
		private static readonly string FINANCE_STATUS = "FinanceStatus";
		private static readonly string CONFIRM_AMOUNT = "RealInCome";
		private static readonly string DAILY_TIME = "DailyTime";
		private static readonly string WAYBILL_TYPE = "WaybillType";
		private static readonly string NEED_AMOUNT = "NeedPrice";
		private static readonly string NEED_BACK_AMOUNT = "NeedReturnPrice";
		private static readonly string FACT_AMOUNT = "PriceDiff";
		private static readonly string FACT_BACK_AMOUNT = "PriceReturnCash";
		private static readonly string COMSSION_STANDARD = "DeductMoney";
		#endregion

		/// <summary>
		/// 根据查询条件获取汇总数据描述
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public string GetTotalFinanceDataTip(SearchCondition condition)
		{
			var tip = new StringBuilder();
			var total = _financeDao.GetTotalFinanceData(condition, true);
			if (total.IsEmpty()) return String.Empty;
			var dr = total.AsEnumerable().FirstOrDefault();
			if (StringUtil.IsEmptyDataRow(dr)) return String.Empty;
			tip.AppendFormat(@"<hr color='#999999'/>
                                          现金成功单量：<font color='blue'><b>{0}</b></font>，
                                          POS机成功单量：<font color='blue'><b>{1}</b></font>，
                                          成功单量：<font color='blue'><b>{2}</b></font>，
                                          成功金额：<font color='blue'><b>{3}</b></font>，
                                          退款订单量：<font color='blue'><b>{4}</b></font>，
                                          退款金额：<font color='blue'><b>{5}</b></font>，
                                          存款金额：<font color='blue'><b>{6}</b></font>
                                          ",
										  dr["CashWaybillCount"].ConvertToInt(),
										  dr["POSWaybillCount"].ConvertToInt(),
										  dr["SucessWaybillCount"].ConvertToInt(),
										  dr["AcceptAmount"].ConvertToDecimal(),
										  dr["BackWaybillCount"].ConvertToInt(),
										  dr["CashRealOutSum"].ConvertToDecimal(),
										  dr["SaveAmount"].ConvertToDecimal());
			return tip.ToString();
		}


        /// <summary>
        /// 根据查询条件获取汇总数据描述
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public string GetTotalFinanceDataTipNew(SearchCondition condition)
        {
            var tip = new StringBuilder();
            var total = _financeDao.GetTotalFinanceDataNew(condition, true);
            if (total.IsEmpty()) return String.Empty;
            var dr = total.AsEnumerable().FirstOrDefault();
            if (StringUtil.IsEmptyDataRow(dr)) return String.Empty;
            tip.AppendFormat(@"<hr color='#999999'/>
                                          现金成功单量：<font color='blue'><b>{0}</b></font>，
                                          POS机成功单量：<font color='blue'><b>{1}</b></font>，
                                          成功单量：<font color='blue'><b>{2}</b></font>，
                                          成功金额：<font color='blue'><b>{3}</b></font>，
                                          退款订单量：<font color='blue'><b>{4}</b></font>，
                                          退款金额：<font color='blue'><b>{5}</b></font>，
                                          存款金额：<font color='blue'><b>{6}</b></font>
                                          ",
                                          dr["CashWaybillCount"].ConvertToInt(),
                                          dr["POSWaybillCount"].ConvertToInt(),
                                          dr["SucessWaybillCount"].ConvertToInt(),
                                          dr["AcceptAmount"].ConvertToDecimal(),
                                          dr["BackWaybillCount"].ConvertToInt(),
                                          dr["CashRealOutSum"].ConvertToDecimal(),
                                          dr["SaveAmount"].ConvertToDecimal());
            return tip.ToString();
        }

        /// <summary>
        /// 根据查询条件获取汇总数据描述
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public string GetTotalFinanceDataTipNewV2(SearchCondition condition)
        {
            var tip = new StringBuilder();
            var total = _financeDao.GetTotalFinanceDataNewV2(condition, true);
            if (total.IsEmpty()) return String.Empty;
            var dr = total.AsEnumerable().FirstOrDefault();
            if (StringUtil.IsEmptyDataRow(dr)) return String.Empty;
            tip.AppendFormat(@"<hr color='#999999'/>
                                          现金成功单量：<font color='blue'><b>{0}</b></font>，
                                          POS机成功单量：<font color='blue'><b>{1}</b></font>，
                                          成功单量：<font color='blue'><b>{2}</b></font>，
                                          成功金额：<font color='blue'><b>{3}</b></font>，
                                          退款订单量：<font color='blue'><b>{4}</b></font>，
                                          退款金额：<font color='blue'><b>{5}</b></font>，
                                          存款金额：<font color='blue'><b>{6}</b></font>
                                          ",
                                          dr["CashWaybillCount"].ConvertToInt(),
                                          dr["POSWaybillCount"].ConvertToInt(),
                                          dr["SucessWaybillCount"].ConvertToInt(),
                                          dr["AcceptAmount"].ConvertToDecimal(),
                                          dr["BackWaybillCount"].ConvertToInt(),
                                          dr["CashRealOutSum"].ConvertToDecimal(),
                                          dr["SaveAmount"].ConvertToDecimal());
            return tip.ToString();
        }

		/// <summary>
		/// 根据查询条件获取汇总数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetTotalFinanceData(SearchCondition condition)
		{
			return _financeDao.GetTotalFinanceData(condition, false);
		}

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNew(SearchCondition condition)
        {
            return _financeDao.GetTotalFinanceDataNew(condition, false);
        }

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNewV2(SearchCondition condition)
        {
            return _financeDao.GetTotalFinanceDataNewV2(condition, false);
        }

        /// <summary>
        /// 根据查询条件获取成功的运单汇总数据 (其他配送商使用)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNewDisV2(SearchCondition condition)
        {
            return _financeDao.GetTotalFinanceDataNewDisV2(condition, false);
        }

         /// <summary>
        /// 根据查询条件获取成功的运单汇总数据 (其他配送商使用)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="displayTotalCount">是否显示汇总的数量</param>
        /// <returns></returns>
        public DataTable GetTotalFinanceDataNewDis(SearchCondition condition)
        {
            return _financeDao.GetTotalFinanceDataNewDis(condition, false);
        }

	    /// <summary>
		/// 根据查询条件获取每天的统计数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetTotalFinanceDailyData(SearchCondition condition)
		{
			return _financeDao.GetTotalFinanceDailyData(condition);
		}
		/// <summary>
		/// 根据查询条件获取明细数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetDetailsFinanceData(SearchCondition condition)
		{
			return _financeDao.GetDetailsFinanceData(condition);
		}

        /// <summary>
        /// 根据查询条件获取成功的运单明细数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceDataNewV2(SearchCondition condition)
        {
            return _financeDao.GetDetailsFinanceDataNewV2(condition);
        }

         /// <summary>
        /// 根据查询条件获取成功的运单明细数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceDataNew(SearchCondition condition)
        {
            return _financeDao.GetDetailsFinanceDataNew(condition);
        }

	    /// <summary>
		/// 根据查询条件获取所有明细数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetAllDetailsFinanceData(SearchCondition condidtion)
		{
			return _financeDao.GetAllDetailsFinanceData(condidtion);
		}

        /// <summary>
        /// 根据查询条件获取明细数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDetailsFinanceDailyDataV2(SearchCondition condition)
        {
            return _financeDao.GetDetailsFinanceDailyDataV2(condition);
        }

		/// <summary>
		/// 根据查询条件获取明细数据
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetDetailsFinanceDailyData(SearchCondition condition)
		{
			return _financeDao.GetDetailsFinanceDailyData(condition);
		}

		/// <summary>
		///  根据查询条件获取系统财务收款报表
		/// </summary>
		/// <param name="condition">查询条件</param>
		/// <returns></returns>
		public DataTable GetSystemWaybillInfo(SearchCondition condition)
		{
			return _financeDao.GetSystemWaybillInfo(condition);
		}

		/// <summary>
		/// 更新当前站点的汇总数据
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public bool ConfirmDailyTotalAmount(FMS_StationDailyFinanceSum model)
		{
			return _financeDao.UpdateDailyTotalAmount(model);
		}

        /// <summary>
        /// 更新当前站点的汇总数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ConfirmDailyTotalAmountV2(FMS_StationDailyFinanceSum model)
        {
            return _financeDao.UpdateDailyTotalAmountV2(model);
        }

		/// <summary>
		/// 绑定订单来源
		/// </summary>
		/// <param name="bindControl"></param>
		public void BindOrderSource(ListControl bindControl)
		{
			var statusInfoDao = ServiceLocator.GetService<IStatusInfoDao>();
            bindControl.DataSource = statusInfoDao.GetStatusInfoByTypeNo(3);
			bindControl.DataTextField = "StatusName";
			bindControl.DataValueField = "StatusNo";
			bindControl.DataBind();
			//插入"请选择"默认项
			bindControl.Items.Insert(0, "--请选择--");
		}

		public bool UpdateMount(string WaybillNO, decimal Mount, int type)
		{
			if (type == 0)
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					_financeDao.UpdateWaybillsigninfoMount(WaybillNO, Mount, Mount);
					_financeDao.UpdateWaybillbackstationMount(WaybillNO, Mount, Mount);
					work.Complete();
					return true;
				}
			}
			else
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					_financeDao.UpdateWaybillsigninfoBackMount(WaybillNO, Mount, Mount, Mount);
					_financeDao.UpdateWaybillbackstationBackMount(WaybillNO, Mount, Mount);
					work.Complete();
					return true;
				}
			}
		}

		//hemingyu 2012-01-09 滞留订单核对导出
		public Dictionary<string, DataTable> CheckDelayOrders(SearchCondition condition, out string exportResult)
		{
			var reports = new Dictionary<string, DataTable>();
			var data = _financeDao.GetWaybillListByOrderNo(condition);
			var result = new StringBuilder();
			result.Append(@"<hr color='#999999'/>");
			if (data.IsEmpty())
			{
				result.Append("当前的订单号不存在滞留核对，请重新导入订单进程核对！");
				exportResult = result.ToString();
				return null;
			}
			foreach (DataRow dr in data.Rows)
			{
				var delay = dr["IsDelay"].ToString();
				if (!reports.Keys.Contains(delay))
				{
					reports.Add(delay, data.Clone());
				}
				reports[delay].ImportRow(dr);
			}
			//执行导出
			result.AppendFormat(@"当前需要核对的订单数：<font color='blue'><b>{0}</b></font>，
                                  妥投/拒收入库/退换货入库订单数：<font color='blue'><b>{1}</b></font>，
                                  滞留订单数：<font color='blue'><b>{2}</b></font>",
								  data.Rows.Count,
								  reports.Keys.Contains("1") ? reports["1"].Rows.Count : 0,
								  reports.Keys.Contains("0") ? reports["0"].Rows.Count : 0);
			exportResult = result.ToString();
			return reports;
		}

		public void ExportDelayReports(Dictionary<string, DataTable> reports, SearchCondition condition)
		{
			CreateExportReportPath(condition.ExportPath);
            if (reports == null)
                throw new Exception("没有报表可以导出");
			foreach (KeyValuePair<string, DataTable> kvp in reports)
			{
                //var fileName = String.Format(@"{0}\{1}.csv",
                //    condition.ExportPath,
                //    kvp.Key == "1" ? "妥投-拒收入库-退换货入库订单表" : "滞留订单表"
                //);
				kvp.Value.Columns.Remove("ID");
				kvp.Value.Columns.Remove("BackStatus");
				kvp.Value.Columns.Remove("Status");
				kvp.Value.Columns.Remove("IsDelay");
                CSVExport.WriteCSVFileByPath(kvp.Value, condition.ExportPath, kvp.Key == "1" ? "妥投-拒收入库-退换货入库订单表" : "滞留订单表",false);
			}
			CompressExportReports(condition);
		}

		private void CompressExportReports(SearchCondition condition)
		{
			var zip = new ZipUtil();
			var error = String.Empty;
			var fileName = condition.ExportPath + ".7z";
			var result = zip.CompressDirectory(condition.ExportPath, fileName, out error);
			if (result)
			{
				ExcelHelper.ExportByWeb(condition.ExportPath, fileName);
			}
		}

		private void CreateExportReportPath(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

        public DataTable GetTransferFinanceSumData(SearchCondition condition)
        {
            return _financeDao.GetTransferFinanceSumData(condition);
        }

        /// <summary>
        /// 通过查询条件得到快递模式配送费明细信息 add by wangyongc 2012-03-27 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable GetTransferFinanceDetailData(SearchCondition condition)
        {
            return _financeDao.GetTransferFinanceDetailData(condition);
        }

        public DataTable GetTransferFinanceSumDataV2(SearchCondition condition)
        {
            var receiveFeeInfoDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveFeeInfoDao.GetTransferFinanceSumDataV2(condition);
        }

        public DataTable GetTransferFinanceDetailDataV2(SearchCondition condition)
        {
            var receiveFeeInfoDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveFeeInfoDao.GetTransferFinanceDetailDataV2(condition);
        }

        public DataTable GetAllDetailsFinanceDataV2(SearchCondition condidtion)
        {
            var receiveFeeInfoDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveFeeInfoDao.GetAllDetailsFinanceDataV2(condidtion);
        }

        public DataTable GetDetailsFinanceDataV2(SearchCondition condition)
        {
            var receiveFeeInfoDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveFeeInfoDao.GetDetailsFinanceDataV2(condition);
        }

        public DataTable GetTotalFinanceDataV2(SearchCondition condition)
        {
            var receiveFeeInfoDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            return receiveFeeInfoDao.GetTotalFinanceDataV2(condition, false);
        }

        public string GetTotalFinanceDataTipV2(SearchCondition condition)
        {
            var receiveFeeInfoDao = ServiceLocator.GetService<IFMS_ReceiveFeeInfoDao>();

            var tip = new StringBuilder();
            var total = receiveFeeInfoDao.GetTotalFinanceDataV2(condition, true);
            if (total.IsEmpty()) return String.Empty;
            var dr = total.AsEnumerable().FirstOrDefault();
            if (StringUtil.IsEmptyDataRow(dr)) return String.Empty;
            tip.AppendFormat(@"<hr color='#999999'/>
                                          现金成功单量：<font color='blue'><b>{0}</b></font>，
                                          POS机成功单量：<font color='blue'><b>{1}</b></font>，
                                          成功单量：<font color='blue'><b>{2}</b></font>，
                                          成功金额：<font color='blue'><b>{3}</b></font>，
                                          退款订单量：<font color='blue'><b>{4}</b></font>，
                                          退款金额：<font color='blue'><b>{5}</b></font>，
                                          存款金额：<font color='blue'><b>{6}</b></font>
                                          ",
                                          dr["CashWaybillCount"].ConvertToInt(),
                                          dr["POSWaybillCount"].ConvertToInt(),
                                          dr["SucessWaybillCount"].ConvertToInt(),
                                          dr["AcceptAmount"].ConvertToDecimal(),
                                          dr["BackWaybillCount"].ConvertToInt(),
                                          dr["CashRealOutSum"].ConvertToDecimal(),
                                          dr["SaveAmount"].ConvertToDecimal());
            return tip.ToString();
        }

        #region 财务收款查询账期查询
        public DataTable GetAccountTotalData(SearchCondition condidtion)
        {
            return _financeDao.GetTotalFinanceData(condidtion, false);
        }

        public DataTable GetAccountDetailsData(SearchCondition condidtion)
        {
            return _financeDao.GetDetailsFinanceData(condidtion);
        }
        #endregion
    }
}
