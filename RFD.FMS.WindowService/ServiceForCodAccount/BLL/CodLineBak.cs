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
	public class CodLineBak : IService
	{
		private TaskModel _taskModel;
		private string year;
		private string month;
        IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();

		#region IService 成员

		public void DealDetail(TaskModel taskModel)
		{
			_taskModel = taskModel;
			year = DateTime.Now.Year.ToString();
			int m = DateTime.Now.Month;
			if (m < 10)
				month = "0" + m;
			else
				month = m.ToString();
            if (DateTime.Now.Day == 1)//每月的第一天启动
            {
				Bak();
			}
			//dal.UpdateToDelete(year, month);
		}

		private void Bak()
		{
			try
			{
                IList<FMS_CODLine> codLineList = deliveryPriceService.GetBackList();
				if (codLineList.Count < 1)
					return;
                IList<FMS_CODLine> codLineListTmp = new List<FMS_CODLine>();
                foreach (FMS_CODLine c in codLineList)
				{
					codLineListTmp.Add(c);
					if (codLineListTmp.Count == _taskModel.insertNum)
					{
						try
						{
                            deliveryPriceService.Insert(codLineListTmp, month, year);
						}
						catch (Exception ex)
						{
							_taskModel.Logger.Error("COD价格按月备份异常:" + ex.Message);//写日志
							if (_taskModel.EmailNotify)
							{
								_taskModel.Loggeremail.Error("COD价格按月备份异常:" + ex.Message);//发送邮件
							}
						}
						codLineListTmp.Clear();
					}
				}
				if (codLineListTmp.Count > 0)
				{
					try
					{
                        deliveryPriceService.Insert(codLineListTmp, month, year);
					}
					catch (Exception ex)
					{
						_taskModel.Logger.Error("COD价格按月备份异常:" + ex.Message);//写日志
						if (_taskModel.EmailNotify)
						{
							_taskModel.Loggeremail.Error("COD价格按月备份异常:" + ex.Message);//发送邮件
						}
					}
				}
			}
			catch (Exception ex)
			{
				_taskModel.Logger.Error("COD价格按月备份异常:" + ex.Message);//写日志
				if (_taskModel.EmailNotify)
				{
					_taskModel.Loggeremail.Error("COD价格按月备份异常:" + ex.Message);//发送邮件
				}
			}
		}

		#endregion
	}
}
