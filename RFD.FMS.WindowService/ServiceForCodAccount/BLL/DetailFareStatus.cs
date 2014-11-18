using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsServiceInterface;
using System.Data;
using ServiceForCodAccount.Common;
using RFD.FMS.Util;
using RFD.FMS.Service.COD;

namespace ServiceForCodAccount.BLL
{
	public class DetailFareStatus : IService
	{
		#region IService 成员

		public void DealDetail(TaskModel taskModel)
		{
			_taskModel = taskModel;

			StatAndChangeStatus();
		}

		#endregion

		private TaskModel _taskModel;
        ICODBaseInfoService cODBaseInfoService = ServiceLocator.GetService<ICODBaseInfoService>();

		private void StatAndChangeStatus()
		{
			StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}\r\n订单号：\r\n{1}\r\n\r\n", EnumHelper.GetDescription(EnumList.EnumErrorType.E8), GetError8()));
            sb.Append(string.Format("{0}\r\n订单号：\r\n{1}\r\n\r\n", EnumHelper.GetDescription(EnumList.EnumErrorType.E9), GetError9()));
			sb.Append(string.Format("{0}\r\n订单号：\r\n{1}\r\n\r\n",EnumHelper.GetDescription(EnumList.EnumErrorType.E7) , GetError7()));
			sb.Append(string.Format("{0}\r\n订单配送公司：\r\n{1}\r\n\r\n", EnumHelper.GetDescription(EnumList.EnumErrorType.E6), GetError6()));
			sb.Append(string.Format("{0}\r\n订单号：\r\n{1}\r\n\r\n", EnumHelper.GetDescription(EnumList.EnumErrorType.E5), GetError5()));
			sb.Append(string.Format("{0}\r\n商家	配送公司	仓库/分拣	区域：\r\n{1}\r\n\r\n", EnumHelper.GetDescription(EnumList.EnumErrorType.E4), GetError4()));
			sb.Append(string.Format("{0}\r\n商家	配送公司	仓库/分拣	区域：\r\n{1}\r\n\r\n", EnumHelper.GetDescription(EnumList.EnumErrorType.E3), GetError3()));
			Common.Common.SendLogEmail(_taskModel, "COD结算每日错误统计分析", sb.ToString(), null);
			//还原标识位
			ChangeBackStatus();
		}

        private string GetError9()
        {
            DataTable dt = cODBaseInfoService.GetError9(Common.Common.AccountDays);
            if (dt == null || dt.Rows.Count <= 0)
                return "无";

            StringBuilder sb = new StringBuilder();
            int n = 0;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append(dr["WaybillNO"]);
                if (n % 10 == 0)
                    sb.Append("\r\n");
                else
                    sb.Append(",");
                n++;
            }

            return sb.ToString();
        }

        private string GetError8()
        {
            DataTable dt = cODBaseInfoService.GetError8(Common.Common.AccountDays);
            if (dt == null || dt.Rows.Count <= 0)
                return "无";

            StringBuilder sb = new StringBuilder();
            int n = 0;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append(dr["WaybillNO"]);
                if (n % 10 == 0)
                    sb.Append("\r\n");
                else
                    sb.Append(",");
                n++;
            }

            return sb.ToString();
        }

		private string GetError7()
		{
            DataTable dt = cODBaseInfoService.GetError7(Common.Common.AccountDays);
			if (dt == null || dt.Rows.Count <= 0)
				return "无";

			StringBuilder sb = new StringBuilder();
			int n = 0;
			foreach (DataRow dr in dt.Rows)
			{
				sb.Append(dr["WaybillNO"]);
				if (n % 10 == 0)
					sb.Append("\r\n");
				else
					sb.Append(",");
				n++;
			}

			return sb.ToString();
		}

		private string GetError6()
		{
            DataTable dt = cODBaseInfoService.GetError6(Common.Common.AccountDays);
			if (dt == null || dt.Rows.Count <= 0)
				return "无";

			StringBuilder sb = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				sb.Append(dr["DeliverStationID"] + "  " + dr["CompanyName"]);
				sb.Append("\r\n");
			}

			return sb.ToString();
		}

		private string GetError5()
		{
            DataTable dt = cODBaseInfoService.GetError5(Common.Common.AccountDays);
			if (dt == null || dt.Rows.Count <= 0)
				return "无";

			StringBuilder sb = new StringBuilder();
			int n = 0;
			foreach (DataRow dr in dt.Rows)
			{
				sb.Append(dr["WaybillNO"]);
				if (n % 10 == 0)
					sb.Append("\r\n");
				else
					sb.Append(",");
				n++;
			}

			return sb.ToString();
		}

		public string GetError4()
		{
            DataTable dt = cODBaseInfoService.GetError34((int)EnumList.EnumErrorType.E4, Common.Common.AccountDays);
			if (dt == null || dt.Rows.Count <= 0)
				return "无";

			StringBuilder sb = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				sb.Append(dr["MerchantName"]+"("+dr["MerchantID"]+")	");
				sb.Append(dr["CompanyName"]+"("+dr["DeliverStationID"]+")	");
				sb.Append(dr["WarehouseName"]+"("+dr["Warehouseid"]+")	");
				sb.Append(dr["AreaName"]+"("+dr["AreaID"]+")	");
				sb.Append("\r\n");
			}

			return sb.ToString();
		}

		public string GetError3()
		{
            DataTable dt = cODBaseInfoService.GetError34((int)EnumList.EnumErrorType.E3, Common.Common.AccountDays);
			if (dt == null || dt.Rows.Count <= 0)
				return "无";

			StringBuilder sb = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				sb.Append(dr["MerchantName"] + "(" + dr["MerchantID"] + ")	");
				sb.Append(dr["CompanyName"] + "(" + dr["DeliverStationID"] + ")	");
				sb.Append(dr["WarehouseName"] + "(" + dr["Warehouseid"] + ")	");
				sb.Append(dr["AreaName"] + "(" + dr["AreaID"] + ")	");
				sb.Append("\r\n");
			}

			return sb.ToString();
		}

		private void ChangeBackStatus()
		{
			//ChangeDeliver();
			//ChangeReturns();
            DataTable dt = cODBaseInfoService.GetDeliver(Common.Common.AccountDays);
			if (dt == null || dt.Rows.Count <= 0)
				return;
			List<string> noList = new List<string>();
			foreach (DataRow dr in dt.Rows)
			{
				noList.Add(dr["ID"].ToString());
				if (noList.Count == _taskModel.insertNum)
				{
					cODBaseInfoService.ChangeDeliverBack(noList);
					noList.Clear();
				}
			}
			if (noList.Count > 0)
			{
				cODBaseInfoService.ChangeDeliverBack(noList);
			}
		}

		//public void ChangeDeliver()
		//{
		//    DataTable dt = cODBaseInfoService.GetDeliver();
		//    if (dt == null || dt.Rows.Count <= 0)
		//        return;
		//    List<string> noList = new List<string>();
		//    foreach (DataRow dr in dt.Rows)
		//    {
		//        noList.Add(dr["WaybillNO"].ToString());
		//        if (noList.Count == _taskModel.insertNum)
		//        {
		//            cODBaseInfoService.ChangeDeliverBack(noList);
		//            noList.Clear();
		//        }
		//    }
		//    if (noList.Count > 0)
		//    {
		//        cODBaseInfoService.ChangeDeliverBack(noList);
		//    }
		//}

		//public void ChangeReturns()
		//{
		//    DataTable dt = cODBaseInfoService.GetReturns();
		//    if (dt == null || dt.Rows.Count <= 0)
		//        return;

		//    List<string> noList = new List<string>();
		//    foreach (DataRow dr in dt.Rows)
		//    {
		//        noList.Add(dr["WaybillNO"].ToString());
		//        if (noList.Count == _taskModel.insertNum)
		//        {
		//            cODBaseInfoService.ChangeReturnsBack(noList);
		//            noList.Clear();
		//        }
		//    }
		//    if (noList.Count > 0)
		//    {
		//        cODBaseInfoService.ChangeDeliverBack(noList);
		//    }
		//}
	}
}
