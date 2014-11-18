using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceForCodAccount.Model;
using System.Data;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;

namespace ServiceForCodAccount.Common
{
	public class StatsCommon
	{
		/// <summary>
		/// 写统计日志
		/// </summary>
		public static void WriteLog(CodStatsLogModel codStatsLog)
		{
            codStatsLog.Ip = Common.GetMachineIp();
            ICODBaseInfoService cODBaseInfoService = ServiceLocator.GetService<ICODBaseInfoService>();
            if (cODBaseInfoService.JudgeLogExists(codStatsLog))
			{
                cODBaseInfoService.UpdateStatisticsLog(codStatsLog);
			}
			else
			{
                if(!string.IsNullOrEmpty(codStatsLog.WareHouseID))
                    cODBaseInfoService.WriteStatisticsLog(codStatsLog);
			}
		}

		/// <summary>
		/// 转换对象LIST
		/// </summary>
		/// <returns></returns>
		public static List<CodStatsLogModel> TransformToCodStatsLogModel(DataTable dt)
		{
			if (dt == null || dt.Rows.Count <= 0)
				return null;

			List<CodStatsLogModel> codStatsLogList = new List<CodStatsLogModel>();
			foreach (DataRow dr in dt.Rows)
			{
				CodStatsLogModel codStatsLog = new CodStatsLogModel();

				if (JudgeColumnContains(dt, dr, "CodStatsID"))
					codStatsLog.CodStatsID = long.Parse(dr["CodStatsID"].ToString());
				if (JudgeColumnContains(dt, dr, "CreateTime"))
					codStatsLog.CreateTime = DateTime.Parse(dr["CreateTime"].ToString());
				if (JudgeColumnContains(dt, dr, "StatisticsType"))
					codStatsLog.StatisticsType = int.Parse(dr["StatisticsType"].ToString());
				if (JudgeColumnContains(dt, dr, "ExpressCompanyID"))
					codStatsLog.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
				if (JudgeColumnContains(dt, dr, "FormCount"))
					codStatsLog.FormCount = int.Parse(dr["FormCount"].ToString());
				if (JudgeColumnContains(dt, dr, "Ip"))
					codStatsLog.Ip = dr["Ip"].ToString();
				if (JudgeColumnContains(dt, dr, "IsSuccess"))
					codStatsLog.IsSuccess = int.Parse(dr["IsSuccess"].ToString());
				if (JudgeColumnContains(dt, dr, "Reasons"))
					codStatsLog.Reasons = dr["Reasons"].ToString();
				if (JudgeColumnContains(dt, dr, "StatisticsDate"))
					codStatsLog.StatisticsDate = DateTime.Parse(dr["StatisticsDate"].ToString());
				if (JudgeColumnContains(dt, dr, "UpdateTime"))
					codStatsLog.UpdateTime = DateTime.Parse(dr["UpdateTime"].ToString());
				if (JudgeColumnContains(dt, dr, "WareHouseID"))
					codStatsLog.WareHouseID = dr["WareHouseID"].ToString();
				if (JudgeColumnContains(dt, dr, "WareHouseType"))
					codStatsLog.WareHouseType = int.Parse(dr["WareHouseType"].ToString());
                if (JudgeColumnContains(dt, dr, "MerchantID"))
                    codStatsLog.MerchantID = int.Parse(dr["MerchantID"].ToString());

				codStatsLogList.Add(codStatsLog);
			}

			return codStatsLogList;
		}

		/// <summary>
		/// 转换为日统计modelList
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static List<CodStatsModel> TransformToCodStatsModel(DataTable dt)
		{
			if (dt == null || dt.Rows.Count <= 0)
				return null;

			List<CodStatsModel> codStatsList = new List<CodStatsModel>();
			foreach (DataRow dr in dt.Rows)
			{
				CodStatsModel codStats = new CodStatsModel();
				if (JudgeColumnContains(dt, dr, "ExpressCompanyID"))
					codStats.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
				if (JudgeColumnContains(dt, dr, "WareHouseID"))
					codStats.WareHouseID = dr["WareHouseID"].ToString();
				if (JudgeColumnContains(dt, dr, "Weight"))
					codStats.Weight = decimal.Parse(dr["Weight"].ToString());
				if (JudgeColumnContains(dt, dr, "Formula"))
					codStats.Formula = dr["Formula"].ToString();
				if (JudgeColumnContains(dt, dr, "FormCount"))
					codStats.FormCount = int.Parse(dr["FormCount"].ToString());
				if (JudgeColumnContains(dt, dr, "Fare"))
					codStats.Fare = decimal.Parse(dr["Fare"].ToString());
				if (JudgeColumnContains(dt, dr, "AreaType"))
					codStats.AreaType = int.Parse(dr["AreaType"].ToString());
				if (JudgeColumnContains(dt, dr, "DeliveryType"))
					codStats.DeliveryType = int.Parse(dr["DeliveryType"].ToString());
				if (JudgeColumnContains(dt, dr, "ReturnsType"))
					codStats.ReturnsType = int.Parse(dr["ReturnsType"].ToString());
				if (JudgeColumnContains(dt, dr, "WareHouseType"))
					codStats.WareHouseType = int.Parse(dr["WareHouseType"].ToString());
				if (JudgeColumnContains(dt, dr, "MerchantID"))
					codStats.MerchantID = int.Parse(dr["MerchantID"].ToString());
				codStatsList.Add(codStats);
			}
			return codStatsList;
		}

		public static bool JudgeColumnContains(DataTable dt,DataRow dr,string columnName)
		{
			if (dt.Columns.Contains(columnName) && dr[columnName] != DBNull.Value)
				return true;

			return false;
		}

        public static bool JudgeRFDSite(CodStatsLogModel codStatsLog)
        {
            DataTable dt = DataCache.GetRFDSite();
            DataRow[] drs = dt.Select("ExpressCompanyID=" + codStatsLog.ExpressCompanyID);
            if (codStatsLog.MerchantID != 8 &&
                codStatsLog.MerchantID != 9 &&
                drs.Length >= 1)
            {
                return false;
            }

            return true;
        }
	}
}
