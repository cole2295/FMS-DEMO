using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsServiceInterface;
using System.Threading;
using ServiceForCodAccount.Model;
using ServiceForCodAccount.Common;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;
using RFD.FMS.MODEL.COD;

namespace ServiceForCodAccount.BLL
{
	/// <summary>
	/// 上门退运费明细
	/// </summary>
	public class VisitReturnsDetail : IService
	{

		private TaskModel _taskModel;
        ICODBaseInfoService cODBaseInfoService = ServiceLocator.GetService<ICODBaseInfoService>();

		#region 方法
		private void DetailProcess()
		{
			try
			{
                IList<FMS_CODBaseInfo> details = cODBaseInfoService.GetVisitReturnDetails(Common.Common.AccountDays, int.Parse(_taskModel.topNum), _taskModel.SyncStartTime);
				if (details == null || details.Count <= 0)
					return;

				StringBuilder sbErrorMsg = new StringBuilder();
                List<FMS_CODBaseInfo> detailInsert = new List<FMS_CODBaseInfo>();
                List<FMS_CODBaseInfo> detailError = new List<FMS_CODBaseInfo>();
                foreach (FMS_CODBaseInfo d in details)
				{
                    FMS_CODBaseInfo detail = d;
					string error = Common.Common.JudgeDetailInfo(ref detail);
					if (!string.IsNullOrEmpty(error))
					{
						detailError.Add(detail);
                        if (!Common.Common.JudgeNotAccountExpress(detail)
                            && detail.ErrorType != (int)EnumList.EnumErrorType.E11
                            && detail.ErrorType != (int)EnumList.EnumErrorType.E12)
						    sbErrorMsg.Append(error + "\n");
						continue;
					}

					detailInsert.Add(detail);
					if (detailInsert.Count == _taskModel.insertNum)//有剩余情况
					{
						try
						{
							UpdateCodFare(detailInsert);
						}
						catch (Exception ex)
						{
							Common.Common.SendLogEmail(_taskModel, "上门退明细服务异常-4:", ex.Message, ex);
						}
						detailInsert.Clear();
					}
				}
				if (detailInsert.Count > 0)
				{
					try
					{
						UpdateCodFare(detailInsert);
					}
					catch (Exception ex)
					{
						Common.Common.SendLogEmail(_taskModel, "上门退明细服务异常-3:", ex.Message, ex);
					}
				}

				if (detailError.Count > 0)
				{
					UpdateError(detailError);
                    if (sbErrorMsg.Length > 0)
					    Common.Common.SendLogEmail(_taskModel, "上门退明细服务异常-2:", sbErrorMsg.ToString(), null);
				}
				Thread.Sleep(1000);
			}
			catch (Exception ex)
			{
				Common.Common.SendLogEmail(_taskModel, "上门退明细服务异常:", ex.Message, ex);
			}
		}

        private void UpdateCodFare(List<FMS_CODBaseInfo> detailInsert)
		{
			if (detailInsert == null || detailInsert.Count <= 0)
				return;

			StringBuilder sb = new StringBuilder();
            foreach (FMS_CODBaseInfo detail in detailInsert)
			{
                if (!cODBaseInfoService.UpdateCodFare(detail))
				{
					sb.Append(Common.Common.DetailInfoErrorMsg(detail, "入库失败") + "\n");
				}
			}

			if (sb != null && sb.Length > 0)
			{
				Common.Common.SendLogEmail(_taskModel, "上门退明细服务异常-1:", sb.ToString(), null);
			}
		}

        public void UpdateError(List<FMS_CODBaseInfo> detailError)
		{
            foreach (FMS_CODBaseInfo detail in detailError)
			{
                cODBaseInfoService.UpdateBackError(detail);
			}
		}

		#endregion

		#region IService 成员

		public void DealDetail(TaskModel taskModel)
		{
			_taskModel = taskModel;

			DetailProcess();
		}

		#endregion
	}
}
