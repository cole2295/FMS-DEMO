using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsServiceInterface;
using System.Data;
using Microsoft.JScript.Vsa;
using ServiceForCodAccount.Model;
using System.Text.RegularExpressions;
using System.Net;
using RFD.FMS.Util;
using System.Configuration;
using RFD.FMS.MODEL.COD;

namespace ServiceForCodAccount.Common
{
	public class Common
	{
        /// <summary>
        /// 计算时时间往前推多少天
        /// </summary>
        public static readonly int AccountDays = ConfigurationManager.AppSettings["CODAccountDays"] == null ? 46 : int.Parse(ConfigurationManager.AppSettings["CODAccountDays"]);
        public static readonly string NotAccountExpress = ConfigurationManager.AppSettings["NotAccountExpress"] == null ? "" : ConfigurationManager.AppSettings["NotAccountExpress"];

        /// <summary>
        /// 判断是否不计算的配送公司
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static bool JudgeNotAccountExpress(FMS_CODBaseInfo detail)
        {
            return NotAccountExpress.Contains(","+detail.DeliverStationID + ",");
        }

		/// <summary>
		/// 获取本机IP
		/// </summary>
		/// <returns></returns>
		public static string GetMachineIp()
		{
			string strHostName = Dns.GetHostName(); //得到本机的主机名
			IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
			string strAddr = ipEntry.AddressList[0].ToString(); //假设本地主机为单网卡
			return strAddr;
		}

		public static bool IsNumber(String strNumber)
		{
			Regex objNotNumberPattern = new Regex("[^0-9.-]");
			Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
			String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
			String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
			Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

			return !objNotNumberPattern.IsMatch(strNumber) &&
			!objTwoDotPattern.IsMatch(strNumber) &&
			!objTwoMinusPattern.IsMatch(strNumber) &&
			objNumberPattern.IsMatch(strNumber);
		}

		/// <summary>
		/// 判断是否金额
		/// </summary>
		/// <param name="fare"></param>
		/// <returns></returns>
		public static bool IsFare(string fare)
		{
			return Regex.IsMatch(fare, @"^(-)?(\d)*(\.(\d){1,2})?$");
		}

		/// <summary>
		/// 取指定日期月份所在第一天
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime GetStartMonth(DateTime dateTime)
		{
			int year = dateTime.Year;
			int month = dateTime.Month;
			return new DateTime(year, month, 1);
		}

		/// <summary>
		/// 取指定日期下一个月所在第一天
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime GetEndMonth(DateTime dateTime)
		{
			DateTime dateTimeNew = dateTime.AddMonths(1);
			int year = dateTimeNew.Year;
			int month = dateTimeNew.Month;
			return new DateTime(year, month, 1);
		}

		/// <summary>
		/// 判断列名是否存在，值是否不为null
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="dr"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static bool JudgeColumnContains(DataTable dt, DataRow dr, string columnName)
		{
			if (dt.Columns.Contains(columnName) && dr[columnName] != DBNull.Value)
				return true;

			return false;
		}

		/// <summary>
		/// 计算价格 主要是针对公式
		/// </summary>
		/// <param name="isEchelon"></param>
		/// <param name="priceFormula"></param>
		/// <param name="weight"></param>
		/// <returns></returns>
        public static void AccountFare(ref FMS_CODBaseInfo detail)
		{
			detail.Fare = GetFare(detail);
		}

		/// <summary>
		/// 计算价格 主要是针对公式
		/// </summary>
		/// <param name="isEchelon"></param>
		/// <param name="priceFormula"></param>
		/// <param name="weight"></param>
		/// <returns></returns>
        public static decimal AccountFare(FMS_CODBaseInfo detail)
		{
			return GetFare(detail);
		}

        private static decimal GetFare(FMS_CODBaseInfo detail)
		{
			decimal fare = 0;
			string formula = detail.FareFormula;
			decimal weight = detail.AccountWeight;
			int isEchelon = detail.IsEchelon;

            //负数取零
            Match m = Regex.Match(formula, @"(负数取零(\(重量-[\d\.]+\)))");
            if (m.Success)
            {
                string str = m.Groups[1].Value;
                string str1 = m.Groups[2].Value;
                string str2 = str.Replace("负数取零", "Number");
                str2 = "(" + str2 + ">0?" + str1 + ":0)";
                formula = formula.Replace(str, str2);
            }

			//当-0.5表示宅急送公式，小于0.5的按0.5算，目的为取整得到0 公式纠正后可以删除这个逻辑，以上面的判断即可
			if (formula.Contains("-0.5") && weight < 0.5M)
				weight = 0.5M;

			try
			{
				if (isEchelon == (int)EnumIsEchelon.Price)
				{
					if (!decimal.TryParse(formula, out fare))
					{
						if (!string.IsNullOrEmpty(weight.ToString()) && weight.ToString() != "0.00")
						{
                            formula = formula.Replace("向上取整", "Math.ceil").Replace("不取整", "").Replace("取整", "Math.floor").Replace("重量", "{0}");
							fare = Convert.ToDecimal(EvalJScript(string.Format(formula, weight)));
						}
						else
							fare = 0;
					}
				}
				else if (isEchelon == (int)EnumIsEchelon.Formula)
				{
					if (!string.IsNullOrEmpty(weight.ToString()) && weight.ToString() != "0.00")
					{
                        formula = formula.Replace("向上取整", "Math.ceil").Replace("不取整", "").Replace("取整", "Math.floor").Replace("重量", "{0}");
						fare = Convert.ToDecimal(EvalJScript(string.Format(formula, weight)));
					}
					else
						fare = 0;
				}
				else
					fare = 0;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return fare;
		}

		public static object EvalJScript(string JScript)
		{
			object Result = null;
			try
			{
				VsaEngine Engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
				Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return Result;
		}

		/// <summary>
		/// 获取对应的线路价格
		/// </summary>
        public static void GetAccountWhere(ref FMS_CODBaseInfo detail)
		{
			DataTable dtLine = GetCodLineCache(detail);
			string formulaTmp = string.Empty;
			int isEchelonTmp = 0;
            int isCodTmp = 0;
            SearchFormula(dtLine, detail, out formulaTmp, out isEchelonTmp, out isCodTmp);
			detail.FareFormula = formulaTmp;
			detail.IsEchelon = isEchelonTmp;
            detail.IsCOD = isCodTmp;
		}

		/// <summary>
		/// 获取对应的线路价格
		/// </summary>
        public static void GetAccountWhere(FMS_CODBaseInfo detail, out string formula, out int isEchelon, out int isCod)
		{
			if (string.IsNullOrEmpty(detail.DeliverTime.ToString()) || detail.DeliverTime == DateTime.MinValue)
			{
				formula = "";
				isEchelon = 0;
                isCod=0;
				return;
			}

			DataTable dtLine = GetCodLineCache(detail);
			string formulaTmp = string.Empty;
			int isEchelonTmp = 0;
            int isCodTmp = 0;
            SearchFormula(dtLine, detail, out formulaTmp, out isEchelonTmp, out isCodTmp);
			formula = formulaTmp;
			isEchelon = isEchelonTmp;
            isCod = isCodTmp;

		}

        private static DataTable GetCodLineCache(FMS_CODBaseInfo detail)
		{
			if (DateTime.Parse(detail.DeliverTime.ToString()).Month == DateTime.Now.Month)//当前月价格
				return DataCache.GetCodLineTable(detail.TopCODCompanyID, int.Parse(detail.AreaType.ToString()),detail.DistributionCode);
			else
			{
				DateTime dt = DateTime.Parse(detail.DeliverTime.ToString()).AddMonths(1);
                return DataCache.GetCodLineHistory(dt.Year, dt.Month, detail.TopCODCompanyID, int.Parse(detail.AreaType.ToString()), detail.DistributionCode);
			}
			//历史以每月1号统计上一个月的入库，以1号所在月记载，规则修改，此地方也需要修改 
		}

        private static void SearchFormula(DataTable dt, FMS_CODBaseInfo detail, out string formula, out int isEchelon, out int isCod)
		{
			if (dt == null || dt.Rows.Count <= 0)
			{
				formula = "";
				isEchelon = 0;
                isCod = 0;
				return;
			}

			string formulaTmp = string.Empty;
			int isEchelonTmp = 0;
            int isCodTmp = 0;
			DataRow[] drs;
			//查询符合的价格
			string searchStr = "ExpressCompanyID={0} AND AreaType={1} AND WareHouseID='{2}' AND MerchantID={3}";
			drs = dt.Select(string.Format(searchStr, detail.TopCODCompanyID, detail.AreaType, detail.WarehouseId, detail.MerchantID));
			if (!string.IsNullOrEmpty(detail.WarehouseId) && drs.Length > 0)
			{
				foreach (DataRow dr in drs)
				{
					if (detail.MerchantID != 8 && detail.MerchantID != 9)//外单、分拣中心
					{
						//多条取仓库对应的先
						if (dr["WareHouseID"].ToString() == detail.WarehouseId && int.Parse(dr["WareHouseType"].ToString()) == 2)
						{
                            //formulaTmp = dr["PriceFormula"].ToString();
                            //isEchelonTmp = int.Parse(dr["IsEchelon"].ToString());
                            GetJudgeIsCodForFormula(dr, detail, out formulaTmp, out isEchelonTmp, out isCodTmp);
							break;
						}
					}
					else
					{
						//多条取仓库对应的先
						if (dr["WareHouseID"].ToString() == detail.WarehouseId && int.Parse(dr["WareHouseType"].ToString()) == 1)
						{
                            //formulaTmp = dr["PriceFormula"].ToString();
                            //isEchelonTmp = int.Parse(dr["IsEchelon"].ToString());
                            GetJudgeIsCodForFormula(dr, detail, out formulaTmp, out isEchelonTmp, out isCodTmp);
							break;
						}
					}
				}
			}
			else
			{
				searchStr = "ExpressCompanyID={0} AND AreaType={1} AND IsEchelon=2 AND MerchantID={2}";//无仓库梯次收费 只取第一条
				drs = dt.Select(string.Format(searchStr, detail.TopCODCompanyID, detail.AreaType, detail.MerchantID));
				if (drs.Length > 0)
				{
                    //formulaTmp = drs[0]["PriceFormula"].ToString();
                    //isEchelonTmp = int.Parse(drs[0]["IsEchelon"].ToString());
                    GetJudgeIsCodForFormula(drs[0], detail, out formulaTmp, out isEchelonTmp, out isCodTmp);
				}
			}
			formula = formulaTmp;
			isEchelon = isEchelonTmp;
            isCod = isCodTmp;
		}

        private static void GetJudgeIsCodForFormula(DataRow dr, FMS_CODBaseInfo detail, out string formula, out int isEchelon, out int isCod)
        {
            //只有普通单 waybilltype=0 才区分COD
            if (detail.WaybillType == "0")
            {
                if (detail.NeedPayAmount > 0)
                {
                    //取COD公式 PriceFormula
                    formula = dr["PriceFormula"].ToString();
                }
                else
                {
                    formula = dr["Formula"].ToString();
                }
                isEchelon = int.Parse(dr["IsEchelon"].ToString());
                isCod = int.Parse(dr["IsCOD"].ToString());
            }
            else
            {
                formula = dr["PriceFormula"].ToString();
                isEchelon = int.Parse(dr["IsEchelon"].ToString());
                isCod = 0;//不区分
            }
        }

		/// <summary>
		/// 获取邮政的区域类型
		/// </summary>
		/// <param name="dtAreaType"></param>
		/// <param name="expressCompanyId"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="AreaId"></param>
		/// <returns></returns>
		//public static void GetAreaTypePost(ref CodAccountDetail codAccountDetail)
		//{
		//    DataTable dtAreaType = DataCache.GetPostAreaType();
		//    if (dtAreaType.Rows.Count <= 0)
		//        return;

		//    int areaType = 0;
		//    string query = "ExpressCompanyId={0} AND WareHouseID='{1}' AND AreaId='{2}'";
		//    DataRow[] drs = dtAreaType.Select(string.Format(query, codAccountDetail.Destination, codAccountDetail.WareHouseID, codAccountDetail.AreaID));
		//    if (drs.Length > 0)
		//    {
		//        areaType = int.Parse(drs[0]["AreaType"].ToString());
		//    }
		//    else
		//    {
		//        areaType = 0;
		//    }

		//    codAccountDetail.AreaType = areaType;
		//}

		public static string GetIP()
		{
			string strHostName = Dns.GetHostName(); //得到本机的主机名
			IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
			string strAddr = ipEntry.AddressList[0].ToString(); //假设本地主机为单网卡
			return strAddr;
		}

		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="title">标题</param>
		/// <param name="message">信息</param>
		/// <param name="ex">异常</param>
		public static void SendLogEmail(TaskModel _taskModel, string title, string message, Exception ex)
		{
			_taskModel.Logger.Error(title + "\n" + message);//写日志
			if (_taskModel.EmailNotify)
				_taskModel.Loggeremail.Error(GetIP() + "\n" + title + "\n" + message + "\n" + (ex == null ? "" : ex.ToString()));//发送邮件
		}


        public static string JudgeDetailInfo(ref FMS_CODBaseInfo detail)
		{
			if (string.IsNullOrEmpty(detail.DeliverStationID.ToString()) ||
				string.IsNullOrEmpty(detail.WarehouseId) ||
				string.IsNullOrEmpty(detail.MerchantID.ToString()) ||
				string.IsNullOrEmpty(detail.TopCODCompanyID.ToString()) ||
				detail.TopCODCompanyID <= 0 || 
				string.IsNullOrEmpty(detail.DeliverTime.ToString()) ||
				detail.DeliverTime==DateTime.MinValue ||
                string.IsNullOrEmpty(detail.DistributionCode)
				)
			{
                detail.ErrorType = (int)EnumList.EnumErrorType.E7;
                return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E7));
			}

            if (JudgeNotAccountExpress(detail))
            {
                detail.ErrorType = (int)EnumList.EnumErrorType.E12;
                return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E12));
            }

            if (detail.DistributionCode == "rfd")
            {
                DataTable dt = DataCache.GetRFDSite();
                DataRow[] drs = dt.Select("ExpressCompanyID=" + detail.DeliverStationID);
                if (detail.MerchantID != 8 &&
                    detail.MerchantID != 9 &&
                    drs.Length >= 1)
                {
                    detail.Fare = 0.00M;
                    detail.FareFormula = "0.00";
                    detail.ErrorType = (int)EnumList.EnumErrorType.E11;
                    return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E11));
                }
            }

            if (string.IsNullOrEmpty(detail.AreaID.ToString()) ||
                detail.AreaID == "0")
            {
                detail.ErrorType = (int)EnumList.EnumErrorType.E8;
                return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E8));
            }

			if (string.IsNullOrEmpty(detail.AreaType.ToString()) ||
				detail.AreaType == 0)
			{
				detail.ErrorType = (int)EnumList.EnumErrorType.E3;
				return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E3));
			}

			GetAccountWhere(ref detail);
			if (string.IsNullOrEmpty(detail.FareFormula) && detail.IsEchelon == 0)
			{
				detail.ErrorType = (int)EnumList.EnumErrorType.E4;
				return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E4));
			}

            //重量小于等于0时 且 需要通过公式计算价格
            if (detail.IsEchelon == (int)EnumIsEchelon.Formula && (string.IsNullOrEmpty(detail.AccountWeight.ToString()) || detail.AccountWeight.ToString() != "0.00"))
            {
                detail.ErrorType = (int)EnumList.EnumErrorType.E9;
                return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E9));
            }

			AccountFare(ref detail);
			if (string.IsNullOrEmpty(detail.Fare.ToString()) || detail.Fare <= 0)
			{
				detail.ErrorType = (int)EnumList.EnumErrorType.E5;
				return DetailInfoErrorMsg(detail, EnumHelper.GetDescription(EnumList.EnumErrorType.E5));
			}

			return null;
		}

        private static void GetExpressCompanyID(DataTable dtDistribution, int expressCompanyId, ref FMS_CODBaseInfo detail)
		{
			DataView dv = new DataView(dtDistribution);
			dv.RowFilter = "ExpressCompanyID=" + expressCompanyId;
			if (dv.Count > 0)
			{
				if (int.Parse(dv[0]["CompanyFlag"].ToString()) == 3)
				{
					detail.TopCODCompanyID = int.Parse(dv[0]["ExpressCompanyID"].ToString());
					return;
				}
				
				GetExpressCompanyID(dtDistribution,int.Parse(dv[0]["ParentID"].ToString()), ref detail);
			}
		}

        private static void GetAreaType(ref FMS_CODBaseInfo detail)
		{
			DataTable dtAreaType = DataCache.GetAreaType(detail.TopCODCompanyID, detail.MerchantID);
			if (dtAreaType == null || dtAreaType.Rows.Count <= 0)
			{
				detail.AreaType = 0;
				return;
			}

			DataView dv = new DataView(dtAreaType);
			dv.RowFilter = string.Format("AreaID='{0}' AND WareHouseID='{1}' AND WareHouseType={2}", detail.AreaID,detail.WarehouseId, detail.WareHouseType);
			if (dv.Count > 0)
			{
				detail.AreaType = int.Parse(dv[0]["AreaType"].ToString());
				return;
			}

			dv.RowFilter = string.Format("AreaID='{0}'", detail.AreaID);
			if (dv.Count > 0)
			{
				detail.AreaType = int.Parse(dv[0]["AreaType"].ToString());
				return;
			}

			detail.AreaType = 0;
		}

        public static string DetailInfoErrorMsg(FMS_CODBaseInfo detail, string msg)
		{
            //其他配送公司处理
            if (detail.DistributionCode != "rfd")
            {
                if (!string.IsNullOrEmpty(detail.ErrorType.ToString()))
                {
                    if (detail.ErrorType > (int)EnumList.EnumErrorType.E2)
                    {
                        detail.ErrorType = (int)EnumList.EnumErrorType.E12;
                        msg = EnumHelper.GetDescription(EnumList.EnumErrorType.E12);
                    }
                }
            }
			string errStr = "订单号：{0} 配送公司：{1} 商家：{2} 区域类型：{3} 区域：{4} 仓库：{5} 公式：{6} 重量：{7} 发货时间：{8} 运费：{9} {10}";
			return string.Format(errStr, detail.WaybillNO, detail.DeliverStationID + "(" + detail.TopCODCompanyID + ")",
								detail.MerchantID, detail.AreaType,
								detail.AreaID, detail.WarehouseId, detail.FareFormula, 
								detail.AccountWeight, detail.DeliverTime,
								detail.Fare, msg);
		}
	}
}
