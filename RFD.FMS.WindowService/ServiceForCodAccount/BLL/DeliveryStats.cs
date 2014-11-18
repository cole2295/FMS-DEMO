using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsServiceInterface;
using ServiceForCodAccount.Model;
using ServiceForCodAccount.Common;
using System.Data;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;
using RFD.FMS.MODEL.COD;

namespace ServiceForCodAccount.BLL
{
	/// <summary>
	/// 发货统计
	/// </summary>
	public class DeliveryStats : IService
	{
		#region IService 成员

		public void DealDetail(TaskModel taskModel)
		{
			_taskModel = taskModel;
			if (!string.IsNullOrEmpty(_taskModel.SyncStartTime))
			{
				if (!DateTime.TryParse(_taskModel.SyncStartTime, out _accountDate))
					return;
			}
			else
				_accountDate = DateTime.Now.AddDays(-1);

			//当前统计
			StatsToday();
			//历史错误统计
			StatsHistory();
		}

		#endregion

		private TaskModel _taskModel;
		private DateTime _accountDate;//当前结算时间

        ICODBaseInfoService cODBaseInfoService = ServiceLocator.GetService<ICODBaseInfoService>();

		private void StatsToday()
		{
			try
			{
                List<CodStatsLogModel> codStatsLogListTmp = cODBaseInfoService.GetDeliverToDayStatsInfo(_accountDate);
				List<CodStatsLogModel> codStatsLogList = Collate(codStatsLogListTmp);//待统计对象列表
				if (codStatsLogList == null || codStatsLogList.Count <= 0)
					return;
				foreach (CodStatsLogModel codStatsLog in codStatsLogList)
				{
					_accountDate = codStatsLog.StatisticsDate;
					StatsByDay(codStatsLog);
				}
			}
			catch (Exception ex)
			{
				Common.Common.SendLogEmail(_taskModel,"发货按天统计当前获取异常-1:", ex.Message, ex);
			}
		}

		private void StatsHistory()
		{
			try
			{
                List<CodStatsLogModel> codStatsLogListTmp = cODBaseInfoService.GetStatsLogError((int)EnumCsType.T1, _accountDate.ToShortDateString(),Common.Common.AccountDays);
				List<CodStatsLogModel> codStatsLogList = Collate(codStatsLogListTmp);//待统计对象列表
				if (codStatsLogList == null || codStatsLogList.Count <= 0)
					return;
				foreach (CodStatsLogModel codStatsLog in codStatsLogList)
				{
					_accountDate = codStatsLog.StatisticsDate;
					StatsByDay(codStatsLog);
				}
			}
			catch (Exception ex)
			{
				Common.Common.SendLogEmail(_taskModel,"发货按天统计历史获取异常-1:", ex.Message, ex);
			}
		}

		private void StatsByDay(CodStatsLogModel codStatsLog)
		{
			try
			{
                List<CodStatsModel> codStatsListTmp = cODBaseInfoService.GetDeliverAccountByDay(codStatsLog);//获取统计
				if (codStatsListTmp == null || codStatsListTmp.Count <= 0)
				{
					Common.Common.SendLogEmail(_taskModel, "发货按天统计入库异常-4:", "日期：" + codStatsLog.StatisticsDate +
                                                " 配送商：" + codStatsLog.ExpressCompanyID + " 仓库：" + codStatsLog.WareHouseID + " 商家" + codStatsLog.MerchantID, null);
					return;
				}
				List<CodStatsModel> codStatsList = new List<CodStatsModel>();
				foreach (CodStatsModel codStats in codStatsListTmp)
				{
					codStatsList.Add(codStats);
					if (codStatsList.Count == _taskModel.insertNum)
					{
						try
						{
                            cODBaseInfoService.InsertDeliverAccount(codStatsList, codStatsLog.StatisticsDate.ToShortDateString());
						}
						catch (Exception ex)
						{
                            Common.Common.SendLogEmail(_taskModel, "发货按天统计入库异常-3:", ex.Message + "\n" + 
                                codStatsLog.ExpressCompanyID + " " + codStatsLog.WareHouseID + " " + codStatsLog.MerchantID, ex);
						}
						codStatsList.Clear();
					}
				}
				//最后一次未提交的
				if (codStatsList.Count > 0)
				{
					try
					{
                        cODBaseInfoService.InsertDeliverAccount(codStatsList, codStatsLog.StatisticsDate.ToShortDateString());
					}
					catch (Exception ex)
					{
                        Common.Common.SendLogEmail(_taskModel, "发货按天统计入库异常-2:", ex.Message + "\n" + codStatsLog.ExpressCompanyID + " " + 
                            codStatsLog.WareHouseID + " " + codStatsLog.MerchantID, ex);
					}
				}
			}
			catch (Exception ex)
			{
                Common.Common.SendLogEmail(_taskModel, "发货按天统计入库异常-1:", ex.Message + "\n" + codStatsLog.ExpressCompanyID + " " +
                    codStatsLog.WareHouseID + " " + codStatsLog.MerchantID, ex);
			}
		}

		/// <summary>
		/// 校对
		/// </summary>
		private List<CodStatsLogModel> Collate(List<CodStatsLogModel> codStatsLogListTmp)
		{
			try
			{
				if (codStatsLogListTmp==null || codStatsLogListTmp.Count <= 0)
					return null;
				
				int AllCount = 0;
				int FareCount = 0;
				string errorStr = "配送商：{0} 仓库：{1} 商家：{2} 统计时间：{3} 相差 {4} 量";
				StringBuilder sbError = new StringBuilder();
				List<CodStatsLogModel> codStatsLogList = new List<CodStatsLogModel>();
				foreach (CodStatsLogModel codStatsLog in codStatsLogListTmp)
				{
                    if (!StatsCommon.JudgeRFDSite(codStatsLog))
                    {
                        continue;
                    }

					if (DateTime.TryParse(codStatsLog.StatisticsDate.ToString(), out _accountDate))
					{
						if (codStatsLog.FormCount == 0)//当前的无序查询总数，历史的查询总数
                            AllCount = cODBaseInfoService.GetDeliverAllCountByExpressWareHose(codStatsLog);
						else
							AllCount = codStatsLog.FormCount;

                        FareCount = cODBaseInfoService.GetDeliverFareCountByExpressWareHouse(codStatsLog);
						if (FareCount == AllCount)
						{
							codStatsLog.IsSuccess = 1;
							codStatsLog.Reasons = "";
							codStatsLogList.Add(codStatsLog);
							StatsCommon.WriteLog(codStatsLog);//写一致日志
						}
						else
						{
							codStatsLog.IsSuccess = 0;
							codStatsLog.Reasons = _accountDate.ToShortDateString() + " 相差 " + (AllCount - FareCount).ToString() + " 量";
							StatsCommon.WriteLog(codStatsLog);//错误处理
                            sbError.Append(string.Format(errorStr, codStatsLog.ExpressCompanyID, codStatsLog.WareHouseID, codStatsLog.MerchantID,
													_accountDate.ToShortDateString(), (AllCount - FareCount).ToString()) + "\n");
						}
					}
				}
				if (sbError.Length > 0)
				{
					Common.Common.SendLogEmail(_taskModel,"发货按天统计校对-2:", sbError.ToString(), null);
				}
				return codStatsLogList;
			}
			catch (Exception ex)
			{
				Common.Common.SendLogEmail(_taskModel, "发货按天统计校对-1:", ex.Message, ex);
				return null;
			}
		}
	}
}
