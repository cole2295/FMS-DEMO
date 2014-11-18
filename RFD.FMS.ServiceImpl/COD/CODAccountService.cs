using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.MODEL;
using RFD.FMS.Util;
using System.Data.SqlClient;
using RFD.FMS.Service.COD;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.COD;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.COD
{
	public class CODAccountService : ICODAccountService
	{
        private ICODAccountDao _cODAccountDao;

        private static string[] WareHouseSH = { "0201" };

        public bool JudgeWareHouseContains(string wareHouses)
        {
            List<string> wList = wareHouses.Replace(" ", "").Split(',').ToList();
            int n = 0;
            foreach (string s in WareHouseSH)
            {
                if (wareHouses.Contains(s))
                {
                    n++;
                    wList.Remove(s);
                }
            }
            if (n > 0 && wList.Count > 0)
                return false;
            else if (n > 0 && n != WareHouseSH.Length)
                return false;
            else
                return true;
        }

		/// <summary>
		/// 查询结算明细
		/// </summary>
		/// <returns></returns>
		public DataTable SearchUniteAccount(CODSearchCondition condition)
		{
			DataTable dtAccountResult = _cODAccountDao.SearchUniteAccount(condition);
			if (dtAccountResult != null && dtAccountResult.Rows.Count > 0)
			{
				SearchAccount(ref dtAccountResult,condition.AccountType);
				return dtAccountResult;
			}
			else
				return null;
		}

		/// <summary>
		/// 查询计算基准运费
		/// </summary>
		private void SearchAccount(ref DataTable dtAccountResult, int accountType)
		{
			foreach (DataRow dr in dtAccountResult.Rows)
			{
				switch (accountType)
				{
					case 1://正常：普通发货+上门退发货-普通拒收-上门退拒收+上门退货
						dr["DatumFare"] = decimal.Parse(dr["DFare"].ToString())
									+ decimal.Parse(dr["DVFare"].ToString())
									- decimal.Parse(dr["RFare"].ToString())
									- decimal.Parse(dr["RVFare"].ToString())
									+ decimal.Parse(dr["VFare"].ToString());
						dr["AccountNum"] = int.Parse(dr["DeliveryNum"].ToString()) + int.Parse(dr["DeliveryVNum"].ToString())
									- int.Parse(dr["ReturnsNum"].ToString()) - int.Parse(dr["ReturnsVNum"].ToString()) + int.Parse(dr["VisitReturnsNum"].ToString());
						break;
					case 2://邮局：普通发货+上门换发货-普通拒收-上门换拒收+上门退
						dr["DatumFare"] = decimal.Parse(dr["DFare"].ToString())
									+ decimal.Parse(dr["DVFare"].ToString())
									- decimal.Parse(dr["RFare"].ToString());
						dr["AccountNum"] = int.Parse(dr["DeliveryNum"].ToString()) + int.Parse(dr["DeliveryVNum"].ToString())
									- int.Parse(dr["ReturnsNum"].ToString()) - int.Parse(dr["ReturnsVNum"].ToString()) + int.Parse(dr["VisitReturnsNum"].ToString());
						break;
					case 3://宅急送：普通发货+上门换发货+((普通拒收+上门换拒收+上门退货)*0.8（折扣）)
						dr["DatumFare"] = decimal.Parse(dr["DFare"].ToString())
									+ decimal.Parse(dr["DVFare"].ToString())
									+ ((decimal.Parse(dr["RFare"].ToString()) +
									decimal.Parse(dr["RVFare"].ToString()) + decimal.Parse(dr["VFare"].ToString())) * 0.8M);
						dr["AccountNum"] = int.Parse(dr["DeliveryNum"].ToString()) + int.Parse(dr["DeliveryVNum"].ToString())
									+ int.Parse(dr["ReturnsNum"].ToString()) + int.Parse(dr["ReturnsVNum"].ToString()) + int.Parse(dr["VisitReturnsNum"].ToString());
						break;
					default:
						dr["DatumFare"] = 0;
						dr["AccountNum"] = 0;
						break;
				}
			}
		}

        private bool BatchUpdateCountAcountNO(DataTable dt,string createBy,string AccountNo,string tableName)
        {
            StringBuilder sbList = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                int n = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (n == 2000)
                    {
                        if (!_cODAccountDao.BatchChangeCountAccountNO(sbList.ToString().TrimEnd(','), createBy, AccountNo, tableName)) return false;
                        sbList.Clear();
                        n = 0;
                    }
                    sbList.Append(dr["AccountID"].ToString() + ",");
                    n++;
                }
            }

            if (!string.IsNullOrEmpty(sbList.ToString().TrimEnd(',')))
            {
                if (!_cODAccountDao.BatchChangeCountAccountNO(sbList.ToString().TrimEnd(','), createBy, AccountNo, tableName)) return false;
            }

            return true;
        }

		/// <summary>
		/// 保存结算单
		/// </summary>
		/// <param name="searchCondition"></param>
		/// <param name="AccountDetail"></param>
		/// <param name="createBy"></param>
		/// <param name="AccountNo"></param>
		/// <returns></returns>
		public bool SaveAccount(CODSearchCondition searchCondition, DataTable AccountDetail, string createBy, out string AccountNo)
		{
            string accountNoStr = "PSJ";
            if (searchCondition.MerchantID == "8" && !JudgeWareHouseContains(searchCondition.HouseD) && !JudgeWareHouseContains(searchCondition.HouseR) && !JudgeWareHouseContains(searchCondition.HouseV))
            {
                accountNoStr = "SH-" + accountNoStr;
            }
            INoGenerate noGenerate = ServiceLocator.GetService<INoGenerate>();
            AccountNo = accountNoStr + noGenerate.GetOrderNo(7, 4, true, "yyyyMMdd");
			GetAccountDetailTables(ref AccountDetail);

			using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
			{
                //查找更新集合
                searchCondition.CountType = "D";
                DataTable dtD = _cODAccountDao.GetChanageCountList(searchCondition);
                if (!BatchUpdateCountAcountNO(dtD, createBy, AccountNo, "FMS_CODDeliveryCount")) return false;

                searchCondition.CountType = "R";
                DataTable dtR = _cODAccountDao.GetChanageCountList(searchCondition);
                if (!BatchUpdateCountAcountNO(dtR, createBy, AccountNo, "FMS_CODReturnsCount")) return false;

                searchCondition.CountType = "V";
                DataTable dtV = _cODAccountDao.GetChanageCountList(searchCondition);
                if (!BatchUpdateCountAcountNO(dtV, createBy, AccountNo, "FMS_CODVisitReturnsCount")) return false;
				
				//结算明细入库
				foreach (DataRow dr in AccountDetail.Rows)
				{
					if (!_cODAccountDao.AddAccountDetail(dr, createBy, AccountNo)) return false;
				}
				//结算单入库				
				if (!_cODAccountDao.AddAccount(searchCondition, createBy, AccountNo)) return false;

				work.Complete();
				return true;
			}
		}

		/// <summary>
		/// 处理结算
		/// </summary>
		/// <param name="AccountDetail"></param>
		/// <param name="expressCompanyId"></param>
		private void GetAccountDetailTables(ref DataTable AccountDetail)
		{
			//linq 求某列和
			//var sum = AccountDetail.AsEnumerable().Sum(s => s.Field<Decimal>("DatumFare"));
			DataRow drNew;
			drNew = AccountDetail.NewRow();
			drNew["MerchantID"] = 0;
			drNew["ExpressCompanyID"] = 0;
			drNew["AreaType"] = 999;
			drNew["Formula"] = "";
			drNew["DeliveryNum"] = AccountDetail.Compute("sum(DeliveryNum)", "DataType=0").ToString().TryGetInt();
			drNew["DeliveryVNum"] = AccountDetail.Compute("sum(DeliveryVNum)", "DataType=0").ToString().TryGetInt();
			drNew["ReturnsNum"] = AccountDetail.Compute("sum(ReturnsNum)", "DataType=0").ToString().TryGetInt();
			drNew["ReturnsVNum"] = AccountDetail.Compute("sum(ReturnsVNum)", "DataType=0").ToString().TryGetInt();
			drNew["VisitReturnsNum"] = AccountDetail.Compute("sum(VisitReturnsNum)", "DataType=0").ToString().TryGetInt();
			drNew["DatumFare"] = AccountDetail.Compute("sum(DatumFare)", "DataType=0").ToString().TryGetDecimal();
			drNew["Allowance"] = "0.00";
			drNew["KPI"] = "0.00";
			drNew["POSPrice"] = "0.00";
			drNew["StrandedPrice"] = "0.00";
			drNew["IntercityLose"] = "0.00";
            drNew["CollectionFee"] = "0.00";
            drNew["DeliveryFee"] = "0.00";
			drNew["OtherCost"] = "0.00";
			drNew["Fare"] = AccountDetail.Compute("sum(DatumFare)", "DataType=0").ToString().TryGetDecimal();
			drNew["DataType"] = 2;
			drNew["AccountNum"] = AccountDetail.Compute("sum(AccountNum)", "DataType=0").ToString().TryGetInt();
			AccountDetail.Rows.Add(drNew);
		}

		public DataTable DisposeDataRowValue(DataTable dt)
		{
			int dataType = 0;
			foreach (DataRow dr in dt.Rows)
			{
				dataType = int.Parse(dr["DataType"].ToString());
				if (dataType == 0)
				{
					dr["Allowance"] = DBNull.Value;
					dr["KPI"] = DBNull.Value;
					dr["POSPrice"] = DBNull.Value;
					dr["StrandedPrice"] = DBNull.Value;
					dr["IntercityLose"] = DBNull.Value;
					dr["OtherCost"] = DBNull.Value;
                    dr["CollectionFee"] = DBNull.Value;
                    dr["DeliveryFee"] = DBNull.Value;
					dr["Fare"] = DBNull.Value;

				}
				else
				{
					dr["AreaType"] = DBNull.Value;
				}
			}

			return dt;
		}

		/// <summary>
		/// 逻辑删除结算单
		/// </summary>
		/// <param name="keyValuePairs"></param>
		/// <param name="updateBy"></param>
		/// <returns></returns>
		public bool DeleteAccountNo(IList<KeyValuePair<string, string>> keyValuePairs, string updateBy)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
					{
						if (!_cODAccountDao.DeleteAccountNo(keyValuePair.Key, updateBy)) return false;
					}

					work.Complete();
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		/// <summary>
		/// 结算单查询
		/// </summary>
		/// <param name="auditStatus"></param>
		/// <param name="expressCompanyId"></param>
		/// <param name="accountDateS"></param>
		/// <param name="accountDateE"></param>
		/// <param name="accountNo"></param>
		/// <returns></returns>
		public DataTable SearchAccount(string auditStatus, string expressCompanyId, string accountDateS, string accountDateE, string accountNo, string merchantId,ref PageInfo pi,bool isPage)
		{
            return _cODAccountDao.SearchAccount(auditStatus, expressCompanyId, accountDateS, accountDateE, accountNo, merchantId, pi, isPage);
		}

		/// <summary>
		/// 根据结算单号查询结算信息
		/// </summary>
		/// <param name="accountNo">结算单号</param>
		/// <param name="flag">是否从只读读取 true 只读</param>
		/// <param name="searchCondition">返回查询条件</param>
		/// <returns></returns>
		public DataTable AccountSearchByNo(string accountNo, bool flag, out CODSearchCondition searchCondition)
		{
			searchCondition = GetSearchConditionByNo(accountNo, flag);

            DataTable dtDetail = DisposeDataRowValue(_cODAccountDao.SearchAccountDetail(accountNo,"", flag));
			dtDetail.TableName = "dtDetail";
			if (dtDetail == null || dtDetail.Rows.Count <= 0)
				return null;
			else
				return dtDetail;
		}

		/// <summary>
		/// 根据结算单号查询结算信息
		/// </summary>
		/// <param name="accountNo">结算单号</param>
		/// <param name="searchCondition">返回查询条件</param>
		/// <returns></returns>
		public DataTable SearchAreaFareByNo(string accountNo, out CODSearchCondition searchCondition)
		{
			searchCondition = GetSearchConditionByNo(accountNo, true);
            DataTable dt = _cODAccountDao.GetAreaFare(accountNo);
			return DisposeAreaData(dt);
		}

		private DataTable DisposeAreaData(DataTable dt)
		{
			if (dt == null || dt.Rows.Count <= 0)
				return null;
			DataTable dtResult = dt.Clone();
			dtResult.Columns["AreaType"].DataType = typeof(string);

			foreach (DataRow dr1 in dt.Rows)
			{
				DataRow drResult = dtResult.NewRow();
				drResult.ItemArray = dr1.ItemArray;
				if (int.Parse(drResult.ItemArray[0].ToString()) > 0)
					drResult["Fare"] = drResult["DatumFare"];
				else
				{
					drResult["Formula"] = DBNull.Value;
					drResult["AreaType"] = "合计：";
				}
				dtResult.Rows.Add(drResult);
			}
			#region 相关价格行
			DataRow dr = dtResult.NewRow();
			dr.ItemArray = dtResult.Rows[dt.Rows.Count - 1].ItemArray;
			dr["AreaType"] = DBNull.Value;
			dr["DeliveryNum"] = DBNull.Value;
			dr["DeliveryVNum"] = DBNull.Value;
			dr["ReturnsNum"] = DBNull.Value;
			dr["ReturnsVNum"] = DBNull.Value;
			dr["VisitReturnsNum"] = DBNull.Value;
			dr["AccountNum"] = DBNull.Value;
			dr["Formula"] = DBNull.Value;
			dr["DatumFare"] = DBNull.Value;
			dr["Fare"] = decimal.Parse(dr["Allowance"].ToString()) + decimal.Parse(dr["KPI"].ToString())
										+ decimal.Parse(dr["POSPrice"].ToString()) + decimal.Parse(dr["StrandedPrice"].ToString())
                                        + decimal.Parse(dr["IntercityLose"].ToString()) + decimal.Parse(dr["CollectionFee"].ToString())
                                        + decimal.Parse(dr["DeliveryFee"].ToString()) + decimal.Parse(dr["OtherCost"].ToString());
			dtResult.Rows.InsertAt(dr, dt.Rows.Count - 1);
			#endregion
			return dtResult;
		}

		public CODSearchCondition GetSearchConditionByNo(string accountNo, bool flag)
		{
            DataTable dtCondition = _cODAccountDao.SearchAccountCondition(accountNo, flag);
			dtCondition.TableName = "dtCondition";
			return TransformConditionModel(dtCondition);
		}

		private CODSearchCondition TransformConditionModel(DataTable dtCondition)
		{
			if (dtCondition == null || dtCondition.Rows.Count <=0)
				return null;

			CODSearchCondition sc = new CODSearchCondition();
			sc.AccountNO = dtCondition.Rows[0]["AccountNO"].ToString();
			sc.ExpressCompanyID = dtCondition.Rows[0]["ExpressCompanyIDs"].ToString();
            sc.CompanyName = sc.ExpressCompanyID.Contains(",") ? MergeCompanyName(sc.ExpressCompanyID) : dtCondition.Rows[0]["DisplayCompanyName"].ToString();
            sc.DisplayCompanyName = dtCondition.Rows[0]["DisplayCompanyName"].ToString();
			sc.HouseD = dtCondition.Rows[0]["DeliveryHouse"].ToString();
			sc.HouseR = dtCondition.Rows[0]["ReturnsHouse"].ToString();
			sc.HouseV = dtCondition.Rows[0]["VisitReturnsHouse"].ToString();
			sc.Date_D_S = DateTime.Parse(dtCondition.Rows[0]["DeliveryDateStr"].ToString());
			sc.Date_D_E = DateTime.Parse(dtCondition.Rows[0]["DeliveryDateEnd"].ToString());
			sc.Date_R_S = DateTime.Parse(dtCondition.Rows[0]["ReturnsDateStr"].ToString());
			sc.Date_R_E = DateTime.Parse(dtCondition.Rows[0]["ReturnsDateEnd"].ToString());
			sc.Date_V_S = DateTime.Parse(dtCondition.Rows[0]["VisitReturnsDateStr"].ToString());
			sc.Date_V_E = DateTime.Parse(dtCondition.Rows[0]["VisitReturnsDateEnd"].ToString());
			sc.AccountType = int.Parse(dtCondition.Rows[0]["AccountType"].ToString());
			sc.MerchantID = dtCondition.Rows[0]["MerchantIDs"].ToString();
            sc.MerchantName = MergeMerchantName(sc.MerchantID,true);
            sc.DisplayMerchantName = MergeMerchantName(sc.MerchantID, false);
            sc.IsDifference = dtCondition.Rows[0]["IsDifference"].ToString().TryGetInt();
			return sc;
		}

        private string MergeCompanyName(string expressCompanyIds)
        {
            if (string.IsNullOrEmpty(expressCompanyIds)) return null;
            IExpressCompanyDao expressCompanyService = ServiceLocator.GetService<IExpressCompanyDao>();
            DataTable dtExpress=expressCompanyService.GetCompanyByIds(expressCompanyIds);
            if (dtExpress == null || dtExpress.Rows.Count <= 0)
            {
                return null;
            }
            StringBuilder sbNames = new StringBuilder();
            foreach (DataRow dr in dtExpress.Rows)
            {
                sbNames.Append(dr["accountcompanyname"].ToString() + ",");
            }
            return sbNames.ToString().TrimEnd(',');
        }

        private string MergeMerchantName(string merchantIds,bool isAll)
        {
            IMerchantDao merchantDao = ServiceLocator.GetService<IMerchantDao>();
            DataTable dtMerchant = merchantDao.GetMerchantNameByID(merchantIds);
            if (dtMerchant == null || dtMerchant.Rows.Count <= 0)
            {
                return null;
            }
            StringBuilder sbNames = new StringBuilder();
            foreach (DataRow dr in dtMerchant.Rows)
            {
                sbNames.Append(dr["MerchantName"].ToString()+",");
            }
            if(isAll)
                return sbNames.ToString().TrimEnd(',');
            else
                return dtMerchant.Rows.Count > 1 ? "其他" : sbNames.ToString().TrimEnd(',');
        }

		/// <summary>
		/// 根据明细号获取结算明细相关金额
		/// </summary>
		/// <param name="accountDetailId"></param>
		/// <returns></returns>
		public CODAccountDetail SearchAccountDetail(string accountDetailId)
		{
            DataTable dt = _cODAccountDao.SearchAccountDetail(accountDetailId,"2", true);
			if (dt == null && dt.Rows.Count <= 0)
				return null;
			return TransformToAccountDetail(dt)[0];
		}

		private List<CODAccountDetail> TransformToAccountDetail(DataTable dt)
		{
			List<CODAccountDetail> accountDetailList = new List<CODAccountDetail>();
			foreach (DataRow dr in dt.Rows)
			{
				CODAccountDetail accountDetail = new CODAccountDetail();
				//accountDetail.AccountDetailID = int.Parse(dr["AccountDetailID"].ToString());
				if (dr["Allowance"] != DBNull.Value)
					accountDetail.Allowance = decimal.Parse(dr["Allowance"].ToString());
				else
					accountDetail.Allowance = 0.00M;

				if (dr["KPI"] != DBNull.Value)
					accountDetail.KPI = decimal.Parse(dr["KPI"].ToString());
				else
					accountDetail.KPI = 0.00M;

				if (dr["POSPrice"] != DBNull.Value)
					accountDetail.POSPrice = decimal.Parse(dr["POSPrice"].ToString());
				else
					accountDetail.POSPrice = 0.00M;

				if (dr["StrandedPrice"] != DBNull.Value)
					accountDetail.StrandedPrice = decimal.Parse(dr["StrandedPrice"].ToString());
				else
					accountDetail.StrandedPrice = 0.00M;

				if (dr["IntercityLose"] != DBNull.Value)
					accountDetail.IntercityLose = decimal.Parse(dr["IntercityLose"].ToString());
				else
					accountDetail.IntercityLose = 0.00M;

				if (dr["OtherCost"] != DBNull.Value)
					accountDetail.OtherCost = decimal.Parse(dr["OtherCost"].ToString());
				else
					accountDetail.OtherCost = 0.00M;
                if (dr["CollectionFee"] != DBNull.Value)
                    accountDetail.CollectionFee = decimal.Parse(dr["CollectionFee"].ToString());
                else
                    accountDetail.CollectionFee = 0.00M;
                if (dr["DeliveryFee"] != DBNull.Value)
                    accountDetail.DeliveryFee = decimal.Parse(dr["DeliveryFee"].ToString());
                else
                    accountDetail.DeliveryFee = 0.00M;

				accountDetailList.Add(accountDetail);
			}

			return accountDetailList;
		}

		/// <summary>
		/// 更新相关结算费用
		/// </summary>
		/// <param name="accountDetail"></param>
		/// <returns></returns>
		public bool UpdateAccountFee(CODAccountDetail accountDetail,string updateBy)
		{
			return _cODAccountDao.UpdateAccountDetailFee(accountDetail, updateBy);
		}

		/// <summary>
		/// 更新审核状态
		/// </summary>
		/// <param name="keyValuePairs"></param>
		/// <param name="auditStatus"></param>
		/// <param name="updateBy"></param>
		/// <returns></returns>
		public bool UpdateAccountAuditStatus(IList<KeyValuePair<string, string>> keyValuePairs,int auditStatus,string updateBy)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
					{
						if (!_cODAccountDao.UpdateAccountAuditStatus(keyValuePair.Key, auditStatus, updateBy)) return false;
					}
					work.Complete();
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		/// <summary>
		/// 结算明细查询
		/// </summary>
		/// <returns></returns>
		public DataTable SearchDetail(string searchType,CODSearchCondition searchCondition,ref PageInfo pi)
		{
            return _cODAccountDao.GetDetail(searchType, searchCondition, ref pi);
		}

		public DataTable SearchExportDetail(string exportType, CODSearchCondition searchCondition)
		{
            return _cODAccountDao.GetExportDetail(exportType, searchCondition, false);
		}

		public DataTable GetErrorLog(CODSearchCondition condition)
		{
			return _cODAccountDao.GetErrorLog(condition);
		}

        public DataTable GetDifferenceData(CODSearchCondition condition)
        {
            DataTable dtResult = CreateTable();
            string sql = string.Empty;

            DataTable dtD = _cODAccountDao.GetExportDetail("D", condition, true);
            UniteDifferenceData(ref dtResult, dtD, "普通发货");

            DataTable dtDV = _cODAccountDao.GetExportDetail("DV", condition, true);
            UniteDifferenceData(ref dtResult, dtDV, "上门换发货");

            DataTable dtR = _cODAccountDao.GetExportDetail("R", condition, true);
            UniteDifferenceData(ref dtResult, dtR, "普通拒收");

            DataTable dtRV = _cODAccountDao.GetExportDetail("RV", condition, true);
            UniteDifferenceData(ref dtResult, dtRV, "上门换拒收");

            DataTable dtV = _cODAccountDao.GetExportDetail("V", condition, true);
            UniteDifferenceData(ref dtResult, dtV, "上门退货");

            return dtResult;
        }

        private DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("类型", typeof(String));
            dt.Columns.Add("订单号",typeof(String));
            dt.Columns.Add("发货仓库", typeof(String));
            dt.Columns.Add("发货时间", typeof(String));
            dt.Columns.Add("入库仓库", typeof(String));
            dt.Columns.Add("入库时间", typeof(String));
            dt.Columns.Add("配送商", typeof(String));
            dt.Columns.Add("商家", typeof(String));
            dt.Columns.Add("区域类型", typeof(String));
            dt.Columns.Add("重量", typeof(String));
            dt.Columns.Add("地址", typeof(String));

            return dt;
        }

        private void UniteDifferenceData(ref DataTable dtResult, DataTable dt,string typeName)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return;

            foreach (DataRow dr in dt.Rows)
            {
                DataRow drNew = dtResult.NewRow();

                drNew["类型"] = typeName;
                if (dt.Columns.Contains("订单号"))
                    drNew["订单号"] = dr["订单号"].ToString();

                if (dt.Columns.Contains("发货仓库"))
                    drNew["发货仓库"] = dr["发货仓库"].ToString();

                if (dt.Columns.Contains("发货时间"))
                    drNew["发货时间"] = dr["发货时间"].ToString();

                if (dt.Columns.Contains("入库仓库"))
                    drNew["入库仓库"] = dr["入库仓库"].ToString();

                if (dt.Columns.Contains("入库时间"))
                    drNew["入库时间"] = dr["入库时间"].ToString();

                if (dt.Columns.Contains("配送商"))
                    drNew["配送商"] = dr["配送商"].ToString();

                if (dt.Columns.Contains("商家"))
                    drNew["商家"] = dr["商家"].ToString();

                if (dt.Columns.Contains("区域类型"))
                    drNew["区域类型"] = dr["区域类型"].ToString();

                if (dt.Columns.Contains("重量"))
                    drNew["重量"] = dr["重量"].ToString();

                if (dt.Columns.Contains("地址"))
                    drNew["地址"] = dr["地址"].ToString();

                dtResult.Rows.Add(drNew);
            }
        }

		#region public 
		/// <summary>
		/// 创建仓库查询xml
		/// </summary>
		/// <param name="houseStr">字符串</param>
		/// <returns></returns>
		private XmlDocument CreateHouseXml(string houseStr)
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement xe;
			string[] houses;
			xe = xmlDoc.CreateElement("root");
			xmlDoc.AppendChild(xe);
			houses = houseStr.Split(',');
			for (int i = 0; i < houses.Length; i++)
			{
				xe = xmlDoc.CreateElement("id");
				if (houses[i].Trim().Contains("S_"))
				{
					xe.SetAttribute("v", houses[i].Trim().Replace("S_", ""));
					xe.SetAttribute("t", "2");
				}
				else
				{
					xe.SetAttribute("v", houses[i].Trim());
					xe.SetAttribute("t", "1");
				}
				xmlDoc.DocumentElement.AppendChild(xe);
			}
			return xmlDoc;
		}

		
		#endregion
	}
}
