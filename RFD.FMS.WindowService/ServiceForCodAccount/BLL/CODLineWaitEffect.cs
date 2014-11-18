using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsServiceInterface;
using ServiceForCodAccount.Model;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.MODEL;

namespace ServiceForCodAccount.BLL
{
	/// <summary>
	/// 待生效同步
	/// </summary>
	public class CODLineWaitEffect : IService
	{
		#region IService 成员

		public void DealDetail(TaskModel taskModel)
		{
			this._taskModel = taskModel;
            string hourTime = DateTime.Now.ToString("HH:mm");
            if (hourTime == _taskModel.FixTime.ToString("HH:mm"))
            {
                if (!string.IsNullOrEmpty(_taskModel.SyncStartTime))
                    DateTime.TryParse(_taskModel.SyncStartTime, out _synDate);

                if (string.IsNullOrEmpty(_synDate.ToString()) || _synDate < DateTime.Parse("2011/07/01"))
                    _synDate = DateTime.Now;//每天凌晨0点0分1秒同步当天的

                WaitEffect();
            }
		}

		#endregion

		private TaskModel _taskModel;
		private DateTime _synDate;
        IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();

		private void WaitEffect()
		{
			try
			{
                List<FMS_CODLine> codLineWaitEffectList = deliveryPriceService.GetCODLineWaitEffect(_synDate.ToShortDateString());
				if (codLineWaitEffectList == null || codLineWaitEffectList.Count <= 0)
					return;

                foreach (FMS_CODLine codLineWaitEffect in codLineWaitEffectList)
				{
					UpdateLine(codLineWaitEffect);
				}
			}
			catch (Exception ex)
			{
				_taskModel.Logger.Error("COD待生效:" + ex.Message);//写日志
				if (_taskModel.EmailNotify)
					_taskModel.Loggeremail.Error("COD待生效:" + ex.Message + "\r\n" + ex);//发送邮件
			}
		}

        private void UpdateLine(FMS_CODLine codLineWaitEffect)
		{
			try
			{
                if (!deliveryPriceService.UpdateLine(codLineWaitEffect))
					throw new Exception("更新失败");
			}
			catch (Exception ex)
			{
				_taskModel.Logger.Error("COD待生效更新:" + ex.Message);//写日志
				if (_taskModel.EmailNotify)
                    _taskModel.Loggeremail.Error("COD待生效更新:" + ex.Message + "\r\n Exception1:" + ex);//发送邮件
			}
		}
	}
}
