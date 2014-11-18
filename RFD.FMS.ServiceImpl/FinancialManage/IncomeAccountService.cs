using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Util;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
	public class IncomeAccountService : IIncomeAccountService
	{
        private IIncomeAccountDao _incomeAccountDao;

		/// <summary>
		/// 查询日统计表
		/// </summary>
		/// <param name="dateStr"></param>
		/// <param name="dateEnd"></param>
		/// <param name="merchantId"></param>
		/// <returns></returns>
		public List<IncomeAccountDetail> GetUniteAccount(IncomeSearchCondition IncomeSearchCondition)
		{
			return TransformToDetailModel(_incomeAccountDao.GetUniteAccount(IncomeSearchCondition));
		}

		private List<IncomeAccountDetail> TransformToDetailModel(DataTable dt)
		{
			if (dt == null || dt.Rows.Count <= 0)
				return null;

			List<IncomeAccountDetail> list = new List<IncomeAccountDetail>();
			foreach (DataRow dr in dt.Rows)
			{
				IncomeAccountDetail m = new IncomeAccountDetail();
				if (dt.Columns.Contains("DetailID") && dr["DetailID"] != DBNull.Value)
					m.DetailID = dr["DetailID"].ToString();
				if (dt.Columns.Contains("AccountNO") && dr["AccountNO"] != DBNull.Value)
					m.AccountNO = dr["AccountNO"].ToString();

                if (dt.Columns.Contains("StationId") && dr["StationId"] != DBNull.Value)//站点ID
                    m.StationId = Int32.Parse(dr["StationId"].ToString());

				m.ExpressCompanyID = Int32.Parse(dr["ExpressCompanyID"].ToString());
				m.CompanyName = dr["CompanyName"].ToString();
				m.AreaType = dr["AreaType"].ToString() == "99999" ? "" : dr["AreaType"].ToString();
				m.DeliveryNum = Int32.Parse(dr["DeliveryNum"].ToString());
				m.DeliveryVNum = Int32.Parse(dr["DeliveryVNum"].ToString());
				m.ReturnsNum = Int32.Parse(dr["ReturnsNum"].ToString());
				m.ReturnsVNum = Int32.Parse(dr["ReturnsVNum"].ToString());
				m.VisitReturnsNum = Int32.Parse(dr["VisitReturnsNum"].ToString());
				m.VisitReturnsVNum = Int32.Parse(dr["VisitReturnsVNum"].ToString());
				m.DeliveryStandard = dr["DeliveryStandard"].ToString();
				m.DeliveryFare = Decimal.Parse(dr["DeliveryFare"].ToString());
				m.DeliveryVStandard = dr["DeliveryVStandard"].ToString();
				m.DeliveryVFare =Decimal.Parse( dr["DeliveryVFare"].ToString());
				m.RetrunsStandard = dr["RetrunsStandard"].ToString();
				m.RetrunsFare = Decimal.Parse(dr["RetrunsFare"].ToString());
				m.ReturnsVStandard = dr["ReturnsVStandard"].ToString();
				m.ReturnsVFare = Decimal.Parse(dr["ReturnsVFare"].ToString());
				m.VisitReturnsStandard = dr["VisitReturnsStandard"].ToString();
				m.VisitReturnsFare = Decimal.Parse(dr["VisitReturnsFare"].ToString());
				m.VisitReturnsVStandard = dr["VisitReturnsVStandard"].ToString();
				m.VisitReturnsVFare =Decimal.Parse( dr["VisitReturnsVFare"].ToString());
				m.ProtectedStandard = dr["ProtectedStandard"].ToString();
				m.ProtectedFee = Decimal.Parse(dr["ProtectedFee"].ToString());
				m.ReceiveStandard = dr["ReceiveStandard"].ToString();
				m.ReceiveFee = Decimal.Parse(dr["ReceiveFee"].ToString());
				m.ReceivePOSStandard =dr["ReceivePOSStandard"].ToString();
				m.ReceivePOSFee =Decimal.Parse( dr["ReceivePOSFee"].ToString());

                if (dt.Columns.Contains("OverAreaSubsidy") && dr["OverAreaSubsidy"] != DBNull.Value)//超区补助
                    m.OverAreaSubsidy = Decimal.Parse(dr["OverAreaSubsidy"].ToString());

                if (dt.Columns.Contains("KPI") && dr["KPI"] != DBNull.Value)//KPI考核
                    m.KPI = Decimal.Parse(dr["KPI"].ToString());

                if (dt.Columns.Contains("LostDeduction") && dr["LostDeduction"] != DBNull.Value)//丢失扣款
                    m.LostDeduction = Decimal.Parse(dr["LostDeduction"].ToString());

                if (dt.Columns.Contains("ResortDeduction") && dr["ResortDeduction"] != DBNull.Value)//滞留扣款
                    m.ResortDeduction = Decimal.Parse(dr["ResortDeduction"].ToString());

				if (dt.Columns.Contains("OtherFee") && dr["OtherFee"] != DBNull.Value)
					m.OtherFee = Decimal.Parse(dr["OtherFee"].ToString());
				if (dt.Columns.Contains("Fare") && dr["Fare"] != DBNull.Value)
					m.Fare = Decimal.Parse(dr["Fare"].ToString());
				m.DataType = Int32.Parse(dr["DataType"].ToString());
                if (dt.Columns.Contains("DeliveryFee") && dr["DeliveryFee"]!=DBNull.Value)
                {
                    m.DeliveryFee = decimal.Parse(dr["DeliveryFee"].ToString());
                }
                if (dt.Columns.Contains("DiscountFee") && dr["DiscountFee"]!=DBNull.Value)
                {
                    m.DiscountFee = decimal.Parse(dr["DiscountFee"].ToString());
                }
				list.Add(m);
			}
			return list;
		}

		/// <summary>
		/// 创建结算表单
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="list"></param>
		/// <param name="createBy"></param>
		/// <param name="AccountNo"></param>
		/// <returns></returns>
		public bool CreateIncomeAccount(IncomeSearchCondition condition,List<IncomeAccountDetail> list,int createBy, out string AccountNo)
		{
            INoGenerate noGenerate = ServiceLocator.GetService<INoGenerate>();
            AccountNo = "SRJ" + noGenerate.GetOrderNo(8, 4, true, "yyyyMMdd");
			BuildAccountDetail(ref list);
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					//统计表绑定结算单号
					if (!_incomeAccountDao.ChanageCountAcountNO(condition, createBy, AccountNo)) return false;
					//结算明细入库
					foreach (IncomeAccountDetail m in list)
					{
						if (!_incomeAccountDao.AddAccountDetail(m, createBy, AccountNo)) return false;
					}
					//结算单入库				
					if (!_incomeAccountDao.AddAccount(condition, createBy, AccountNo)) return false;

					work.Complete();
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public void BuildAccountDetail(ref List<IncomeAccountDetail> list)
		{
			var statDetail = from t in list
					   select new
					   {
						   t.ExpressCompanyID,
						   t.DeliveryNum,
						   t.DeliveryFare,
						   t.DeliveryVNum,
						   t.DeliveryVFare,
						   t.ReturnsNum,
						   t.RetrunsFare,
						   t.ReturnsVNum,
						   t.ReturnsVFare,
						   t.VisitReturnsNum,
						   t.VisitReturnsFare,
						   t.VisitReturnsVNum,
						   t.VisitReturnsVFare,
						   t.ProtectedFee,
						   t.ReceiveFee,
						   t.ReceivePOSFee,
						   t.DataType
					   } into tn
					   where tn.DataType==0
					   select tn;

			var statExpresscompany =
						 from t1 in statDetail
						 group t1 by new { t1.ExpressCompanyID } into g
						 select new IncomeAccountDetail
						 {
							 ExpressCompanyID = g.Key.ExpressCompanyID,
							 AreaType="99999",
							 DeliveryNum = g.Sum(t => t.DeliveryNum),
							 DeliveryFare = g.Sum(t => t.DeliveryFare),
							 DeliveryVNum = g.Sum(t => t.DeliveryVNum),
							 DeliveryVFare = g.Sum(t => t.DeliveryVFare),
							 ReturnsNum = g.Sum(t => t.ReturnsNum),
							 RetrunsFare = g.Sum(t => t.RetrunsFare),
							 ReturnsVNum = g.Sum(t => t.ReturnsVNum),
							 ReturnsVFare = g.Sum(t => t.ReturnsVFare),
							 VisitReturnsNum = g.Sum(t => t.VisitReturnsNum),
							 VisitReturnsFare = g.Sum(t => t.VisitReturnsFare),
							 VisitReturnsVNum = g.Sum(t => t.VisitReturnsVNum),
							 VisitReturnsVFare = g.Sum(t => t.VisitReturnsVFare),
							 ProtectedFee = g.Sum(t => t.ProtectedFee),
							 ReceiveFee = g.Sum(t => t.ReceiveFee),
							 ReceivePOSFee = g.Sum(t => t.ReceivePOSFee),
							 Fare = g.Sum(t => t.DeliveryFare) + g.Sum(t => t.DeliveryVFare) + 
									g.Sum(t => t.RetrunsFare)+g.Sum(t => t.ReturnsVFare)+
									g.Sum(t => t.VisitReturnsFare) + g.Sum(t => t.VisitReturnsVFare) +
									g.Sum(t => t.ProtectedFee) + g.Sum(t => t.ReceiveFee) + g.Sum(t => t.ReceivePOSFee),
							 DataType = 1,
						 };
			foreach (IncomeAccountDetail m in statExpresscompany)
			{
				list.Add(m);
			}

			var all = new IncomeAccountDetail()
			{
				ExpressCompanyID=9999999,
				AreaType="99999",
				DeliveryNum = statDetail.Select(t => t.DeliveryNum).Sum(),
				DeliveryFare = statDetail.Select(t => t.DeliveryFare).Sum(),
				DeliveryVNum = statDetail.Select(t => t.DeliveryVNum).Sum(),
				DeliveryVFare = statDetail.Select(t => t.DeliveryVFare).Sum(),
				ReturnsNum = statDetail.Select(t => t.ReturnsNum).Sum(),
				RetrunsFare = statDetail.Select(t => t.RetrunsFare).Sum(),
				ReturnsVNum = statDetail.Select(t => t.ReturnsVNum).Sum(),
				ReturnsVFare = statDetail.Select(t => t.ReturnsVFare).Sum(),
				VisitReturnsNum = statDetail.Select(t => t.VisitReturnsNum).Sum(),
				VisitReturnsFare = statDetail.Select(t => t.VisitReturnsFare).Sum(),
				VisitReturnsVNum = statDetail.Select(t => t.VisitReturnsVNum).Sum(),
				VisitReturnsVFare = statDetail.Select(t => t.VisitReturnsVFare).Sum(),
				ProtectedFee = statDetail.Select(t => t.ProtectedFee).Sum(),
				ReceiveFee = statDetail.Select(t => t.ReceiveFee).Sum(),
				ReceivePOSFee = statDetail.Select(t => t.ReceivePOSFee).Sum(),
				Fare = statDetail.Select(t => t.DeliveryFare).Sum() + statDetail.Select(t => t.DeliveryVFare).Sum()+
						statDetail.Select(t => t.RetrunsFare).Sum() + statDetail.Select(t => t.ReturnsVFare).Sum()+
						statDetail.Select(t => t.VisitReturnsFare).Sum() + statDetail.Select(t => t.VisitReturnsVFare).Sum()+
						statDetail.Select(t => t.ProtectedFee).Sum() + statDetail.Select(t => t.ReceiveFee).Sum() + statDetail.Select(t => t.ReceivePOSFee).Sum(),
				DataType = 2,
			};

			list.Add(all);					   
		}

		public DataTable GetAccountList(string accountStatus, string merchantId,string dateStr,string dateEnd,string accountNo)
		{
            return _incomeAccountDao.GetAccountList(accountStatus, merchantId, dateStr, dateEnd, accountNo);
		}

		public List<IncomeAccountDetail> GetAccountDetails(string accountNo)
		{
            return TransformToDetailModel(_incomeAccountDao.GetAccountDetail(accountNo,""));
		}

        /// <summary>
        /// 收入结算查询（新）
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public List<IncomeAccountDetail> GetAccountDetailsNew(string accountNo)
        {
            return TransformToDetailModel(_incomeAccountDao.GetAccountDetailNew(accountNo,""));
        }

		public IncomeAccountDetail GetAccountDetail(string detailId)
		{
            List<IncomeAccountDetail> list = TransformToDetailModel(_incomeAccountDao.GetAccountDetail("", detailId));
			if (list == null || list.Count <= 0)
				return null;
			else
				return list[0];
		}

        /// <summary>
        /// 获取结单的总数据
        /// </summary>
        /// <param name="detailId"></param>
        /// <returns></returns>
        public IncomeAccountDetail GetAccountDetailByAccountNo(string accountNo)
        {
            List<IncomeAccountDetail> list = TransformToDetailModel(_incomeAccountDao.GetAccountDetailNew(accountNo,"2"));
            if (list == null || list.Count <= 0)
                return null;
            else
                return list[0];
        }

		public IncomeSearchCondition GetAccountSearchCondition(string accountNo)
		{
            DataTable dt = _incomeAccountDao.GetAccountSearchCondition(accountNo);
			if (dt == null || dt.Rows.Count <= 0)
				return null;
			else
			{
				IncomeSearchCondition i = new IncomeSearchCondition();
				i.AccountNO = dt.Rows[0]["AccountNO"].ToString();
				i.MerchantID = dt.Rows[0]["MerchantID"].ToString();
				i.DateStr = DateTime.Parse(dt.Rows[0]["SearchDateStr"].ToString());
				i.DateEnd = DateTime.Parse(dt.Rows[0]["SearchDateEnd"].ToString());
				i.MerchantName = dt.Rows[0]["MerchantName"].ToString();
				return i;
			}
		}

		public bool DeleteAccount(IList<KeyValuePair<string, string>> checkList, int updateBy)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> k in checkList)
					{
						//统计表
						if (!_incomeAccountDao.DeleteCount(k.Key, updateBy)) return false;
						//结算表				
						if (!_incomeAccountDao.DeleteAccount(k.Key, updateBy)) return false;
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

		public bool UpdateAccountFee(IncomeAccountDetail i)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					//更新当前费用
					if (!_incomeAccountDao.UpdateAccountDetailFee(i)) return false;
					//更新总费用
					if (!_incomeAccountDao.UpdateAccountDetailFeeAll(i)) return false;
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
        /// 修改结算单各项金额
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool UpdateAccountFeeByAccountNo(IncomeAccountDetail i)
        {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    //更新总费用
                    if (!_incomeAccountDao.UpdateAccountDetailFeeAllByAccountNo(i)) return false;
                    work.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

		public bool UpdateAccountStatus(IList<KeyValuePair<string, string>> list, int auditBy, int status)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> k in list)
					{
						if (!_incomeAccountDao.UpdateAccountStatus(k.Key, auditBy, status)) return false;
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

		public DataTable GetAccountAreaSummary(string accountNo)
		{
            DataTable dt = _incomeAccountDao.GetAccountAreaSummary(accountNo);

			return DisposeTotalAreaSummary(dt);
		}

		private DataTable DisposeTotalAreaSummary(DataTable dt)
		{
			if (dt == null || dt.Rows.Count <= 0)
				return null;
			DataTable dtResult = dt.Clone();
			dtResult.Columns["AreaType"].DataType = typeof(string);
			DataRow drTotal = dtResult.NewRow();
			DataColumn dc = new DataColumn("Total", typeof(decimal));
            if (!dtResult.Columns.Contains("Total"))
            {
                dtResult.Columns.Add(dc);
            }
			foreach (DataRow dr in dt.Rows)
			{
				DataRow drResult = dtResult.NewRow();
				drResult.ItemArray = dr.ItemArray;
				drResult["Total"] = dr["DeliveryFare"].ToString().TryGetDecimal() + dr["DeliveryVFare"].ToString().TryGetDecimal() +
									dr["ProtectedFee"].ToString().TryGetDecimal() + dr["ReceiveFee"].ToString().TryGetDecimal() +
									dr["ReceivePOSFee"].ToString().TryGetDecimal() + dr["OtherFee"].ToString().TryGetDecimal()
                                    + dr["DeliveryFee"].ToString().TryGetDecimal() + dr["DiscountFee"].ToString().TryGetDecimal();
				dtResult.Rows.Add(drResult);
				drTotal["DeliveryNum"] = drTotal["DeliveryNum"].ToString().TryGetDecimal() + dr["DeliveryNum"].ToString().TryGetDecimal();
				drTotal["DeliveryFare"] = drTotal["DeliveryFare"].ToString().TryGetDecimal() + dr["DeliveryFare"].ToString().TryGetDecimal();
				drTotal["ReturnsVNum"] = drTotal["ReturnsVNum"].ToString().TryGetDecimal() + dr["ReturnsVNum"].ToString().TryGetDecimal();
				drTotal["DeliveryVFare"] = drTotal["DeliveryVFare"].ToString().TryGetDecimal() + dr["DeliveryVFare"].ToString().TryGetDecimal();
				drTotal["ProtectedFee"] = drTotal["ProtectedFee"].ToString().TryGetDecimal() + dr["ProtectedFee"].ToString().TryGetDecimal();
				drTotal["ReceiveFee"] = drTotal["ReceiveFee"].ToString().TryGetDecimal() + dr["ReceiveFee"].ToString().TryGetDecimal();
				drTotal["ReceivePOSFee"] = drTotal["ReceivePOSFee"].ToString().TryGetDecimal() + dr["ReceivePOSFee"].ToString().TryGetDecimal();
				drTotal["OtherFee"] = drTotal["OtherFee"].ToString().TryGetDecimal() + dr["OtherFee"].ToString().TryGetDecimal();
			    drTotal["DeliveryFee"] = drTotal["DeliveryFee"].ToString().TryGetDecimal() +
			                             dr["DeliveryFee"].ToString().TryGetDecimal();
                drTotal["DiscountFee"] =drTotal["DiscountFee"].ToString().TryGetDecimal()+ dr["DiscountFee"].ToString().TryGetDecimal();
                drTotal["Total"] = drTotal["Total"].ToString().TryGetDecimal() + drResult["Total"].ToString().TryGetDecimal();
			}
			drTotal["AreaType"] = "合计：";
			dtResult.Rows.Add(drTotal);
			return dtResult;
        }


        #region 收入结算相关服务方法

        /// <summary>
        /// 返回所有需要生成日统计报表的商家
        /// </summary>
        /// <returns></returns>
        public DataTable GetMerchantInfo(string distributionCode)
        {
            return _incomeAccountDao.GetMerchantInfo(distributionCode);
        }

        /// <summary>
        /// 按商家，配送站，分拣中心，区域类型，发货方式，结算标准分组，计算发货日统计数据
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeDeliveryAccount(int merchantId, DateTime accountDate, int deliverstationid, int expresscompanyid)
	    {
	       return _incomeAccountDao.GetIncomeDeliveryAccount(merchantId, accountDate,deliverstationid,expresscompanyid);
	    }

        /// <summary>
        /// 按商家，站点，分拣中心分组，计算当日发货日统计数据
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeDeliveryAccountInfo(int merchantId,DateTime accountDate)
        {
            return _incomeAccountDao.GetIncomeDeliveryAccountInfo(merchantId, accountDate);
        }

	    /// <summary>
        /// 校验这个商家，这个站点，这个分拣中心，在当天费用是否都计算过
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="warehouseid"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int CollateIncomeDeliveryAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            return _incomeAccountDao.CollateIncomeDeliveryAccountInfo(merchantid, accountDate, warehouseid, deliverstationid);
        }

        /// <summary>
        /// 插入发货日统计表和日志表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelLog"></param>
        /// <returns></returns>
        public int AddIncomeDeliveryAccount(IncomeDeliveryCount model,IncomeStatLog modelLog)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                _incomeAccountDao.AddIncomeDeliveryAccount(model);
                _incomeAccountDao.AddIncomeStatLog(modelLog);
                work.Complete();
                return 1;
            }
        }

        /// <summary>
        /// 插入日志表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddIncomeStatLog(IncomeStatLog model)
        {
            return _incomeAccountDao.AddIncomeStatLog(model);
        }

        public bool UpdateIncomeStatLog(IncomeStatLog model)
        {
            return _incomeAccountDao.UpdateIncomeStatLog(model);
        }

        /// <summary>
        /// 添加发货日统计表数据
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="countDate"></param>
        public bool AddIncomeDeliveryDetails(int merchantId, DateTime countDate, int deliverstationid, int expresscompanyid)
	    {
	        DataTable dt = _incomeAccountDao.GetIncomeDeliveryAccount(merchantId,countDate,deliverstationid,expresscompanyid);

            if(dt.Rows.Count>0)
            {
                IncomeDeliveryCount model = new IncomeDeliveryCount();

                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //插入收入结算发货表
                        model.AccountNO = "";
                        if (!string.IsNullOrEmpty(dt.Rows[i]["AreaType"].ToString()))
                        { model.AreaType = int.Parse(dt.Rows[i]["AreaType"].ToString()); }
                        
                        model.CountDate = countDate;
                        model.CountNum = int.Parse(dt.Rows[i]["FormCount"].ToString());
                        model.CountType = int.Parse(dt.Rows[i]["WaybillType"].ToString());
                        model.CreateBy = 0;
                        model.CreateTime = DateTime.Now;
                        model.ExpressCompanyID = int.Parse(dt.Rows[i]["warehouseid"].ToString());
                        model.Fare = decimal.Parse(dt.Rows[i]["Fare"].ToString());
                        model.Formula = string.IsNullOrEmpty(dt.Rows[i]["Formula"].ToString()) ? " " : dt.Rows[i]["Formula"].ToString();
                        model.IsDeleted = 0;
                        model.MerchantID = merchantId;
                        model.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        model.UpdateBy = 0;
                        model.UpdateTime = DateTime.Now;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["WEIGHT"].ToString()))
                        { model.Weight = decimal.Parse(dt.Rows[i]["WEIGHT"].ToString()); }
                        

                        _incomeAccountDao.AddIncomeDeliveryAccount(model);
                    }

                    work.Complete();
                    return true;
                }
            }
            return false; 
	    }

        /// <summary>
        /// 当日拒收数据（商家、配送站、分拣中心）
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeReturnsCount(int merchantId, DateTime accountDate)
        {
            return _incomeAccountDao.GetIncomeReturnsCount(merchantId, accountDate);
        }

        /// <summary>
        /// 校验当日拒收数据（商家、配送站、分拣中心）
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="warehouseid"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int CollateIncomeReturnsAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            return _incomeAccountDao.CollateIncomeReturnsAccountInfo(merchantid, accountDate, warehouseid, deliverstationid);
        }

        /// <summary>
        /// 添加拒收日统计表数据
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="countDate"></param>
        public bool AddIncomeReturnsAccountDetails(int merchantId, DateTime countDate,int deliverStationId,int returnExpressId)
        {
            DataTable dt = _incomeAccountDao.GetIncomeReturnsAccount(merchantId, countDate, deliverStationId, returnExpressId);

            if (dt.Rows.Count > 0)
            {
                IncomeReturnsCount model = new IncomeReturnsCount();

                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //插入收入结算发货表
                        model.AccountNO = "";
                        if (!string.IsNullOrEmpty(dt.Rows[i]["AreaType"].ToString()))
                        { model.AreaType = int.Parse(dt.Rows[i]["AreaType"].ToString()); }
                        
                        model.CountDate = countDate;
                        model.CountNum = int.Parse(dt.Rows[i]["FormCount"].ToString());
                        model.CountType = int.Parse(dt.Rows[i]["WaybillType"].ToString());
                        model.CreateBy = 0;
                        model.CreateTime = DateTime.Now;
                        model.ExpressCompanyID = int.Parse(dt.Rows[i]["ReturnExpressCompanyID"].ToString());
                        model.Fare = decimal.Parse(dt.Rows[i]["Fare"].ToString());
                        model.Formula = string.IsNullOrEmpty(dt.Rows[i]["Formula"].ToString()) ? " " : dt.Rows[i]["Formula"].ToString();
                        model.IsDeleted = 0;
                        model.MerchantID = merchantId;
                        model.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        model.UpdateBy = 0;
                        model.UpdateTime = DateTime.Now;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["WEIGHT"].ToString()))
                        { model.Weight = decimal.Parse(dt.Rows[i]["WEIGHT"].ToString()); }
                        

                        _incomeAccountDao.AddIncomeReturnsCountInfo(model);
                    }

                    work.Complete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 收结算-上门1
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeVisitReturnsCount(int merchantId, DateTime accountDate)
        {
            return _incomeAccountDao.GetIncomeVisitReturnsCount(merchantId, accountDate);
        }

        /// <summary>
        /// 收入结算--上门（校验）
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="warehouseid"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int CollateIncomeVisitReturnsAccountInfo(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            return _incomeAccountDao.CollateIncomeVisitReturnsAccountInfo(merchantid, accountDate, warehouseid, deliverstationid);
        }

        /// <summary>
        /// 收入结算--上门（插入数据）
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="countDate"></param>
        /// <returns></returns>
        public bool AddIncomeVisitReturnsAccountDetails(int merchantId, DateTime countDate,int deliverStationId,int returnExpressId)
        {
            //收入结算--上门（站点，返回分拣中心，区域类型，上门退类型，结算标准）
            DataTable dt = _incomeAccountDao.GetIncomeVisitReturnsAccount(merchantId, countDate,deliverStationId,returnExpressId);

            if (dt.Rows.Count > 0)
            {
                IncomeVisitReturnsCount model = new IncomeVisitReturnsCount();

                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //插入收入结算发货表
                        model.AccountNO = "";
                        if (!string.IsNullOrEmpty(dt.Rows[i]["AreaType"].ToString()))
                        { model.AreaType = int.Parse(dt.Rows[i]["AreaType"].ToString()); }
                        
                        model.CountDate = countDate;
                        model.CountNum = int.Parse(dt.Rows[i]["FormCount"].ToString());
                        model.CountType = int.Parse(dt.Rows[i]["CountType"].ToString());
                        model.CreateBy = 0;
                        model.CreateTime = DateTime.Now;
                        model.ExpressCompanyID = int.Parse(dt.Rows[i]["ReturnExpressCompanyID"].ToString());
                        model.Fare = decimal.Parse(dt.Rows[i]["Fare"].ToString());
                        model.Formula = dt.Rows[i]["Formula"].ToString();
                        model.IsDeleted = 0;
                        model.MerchantID = merchantId;
                        model.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        model.UpdateBy = 0;
                        model.UpdateTime = DateTime.Now;

                        if (!string.IsNullOrEmpty(dt.Rows[i]["WEIGHT"].ToString()))
                        { model.Weight = decimal.Parse(dt.Rows[i]["WEIGHT"].ToString()); }
                        

                        _incomeAccountDao.AddIncomeVisitReturnsCountInfo(model);
                    }

                    work.Complete();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 收入结算--其它金额（校验代收货款）
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public DataTable GetIncomeOtherReceiveFee(int merchantId, DateTime accountDate)
        {
            return _incomeAccountDao.GetIncomeOtherReceiveFee(merchantId, accountDate);
        }

        /// <summary>
        /// 收入结算--其它金额（校验代收货款）
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int CollateIncomeOtherReceiveFee(int merchantid, DateTime accountDate, int deliverstationid)
        {
            return _incomeAccountDao.CollateIncomeOtherReceiveFee(merchantid, accountDate,deliverstationid);
        }

        /// <summary>
        /// 收入结算--其它金额（新增代收货款）
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="countDate"></param>
        /// <returns></returns>
        public bool AddIncomeOtherReceiveFee(int merchantId, DateTime countDate,int deliverStationId)
        {
            DataTable dt = _incomeAccountDao.GetIncomeOtherReceiveFeeDetails(merchantId, countDate,deliverStationId);

            if (dt.Rows.Count > 0)
            {
                IncomeOtherFeeCount model = new IncomeOtherFeeCount();

                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //插入收入结算发货表
                        model.AccountNO = "";
                        model.CountDate = countDate;
                        model.CountType = int.Parse(dt.Rows[i]["CountType"].ToString());
                        model.CreateBy = 0;
                        model.CreateTime = DateTime.Now;
                        model.ReceiveFee =string.IsNullOrEmpty(dt.Rows[i]["Fare"].ToString())?0:decimal.Parse(dt.Rows[i]["Fare"].ToString());
                        model.ReceiveStandard = string.IsNullOrEmpty(dt.Rows[i]["Formula"].ToString())?0:decimal.Parse(dt.Rows[i]["Formula"].ToString());
                        model.ReceivePOSFee = string.IsNullOrEmpty(dt.Rows[i]["POSFare"].ToString()) ? 0 : decimal.Parse(dt.Rows[i]["POSFare"].ToString());
                        model.ReceivePOSStandard = string.IsNullOrEmpty(dt.Rows[i]["POSFormula"].ToString()) ? 0 : decimal.Parse(dt.Rows[i]["POSFormula"].ToString());
                        model.ServesFee = string.IsNullOrEmpty(dt.Rows[i]["ServiceFare"].ToString()) ? 0 : decimal.Parse(dt.Rows[i]["ServiceFare"].ToString());
                        model.ServesStandard =string.IsNullOrEmpty(dt.Rows[i]["ServiceFormula"].ToString())?0: decimal.Parse(dt.Rows[i]["ServiceFormula"].ToString());
                        model.POSServesFee = string.IsNullOrEmpty(dt.Rows[i]["POSServiceFare"].ToString()) ? 0 : decimal.Parse(dt.Rows[i]["POSServiceFare"].ToString());
                        model.POSServesStandard =string.IsNullOrEmpty(dt.Rows[i]["POSServiceFormula"].ToString())?0: decimal.Parse(dt.Rows[i]["POSServiceFormula"].ToString()); 
                        model.IsDeleted = 0;
                        model.MerchantID = merchantId;
                        model.StationID = int.Parse(dt.Rows[i]["DeliverStationID"].ToString());
                        model.UpdateBy = 0;
                        model.UpdateTime = DateTime.Now;

                        _incomeAccountDao.AddIncomeOtherFeeCount(model);
                    }

                    work.Complete();
                    return true;
                }
            }
            return false;
        }

        public DataTable GetIncomeHistoryCount(DateTime accountDate)
        {
            return _incomeAccountDao.GetIncomeHistoryCount(accountDate);
        }

        public int IncomeDeliveryAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            return _incomeAccountDao.IncomeDeliveryAccountHistory(merchantid, accountDate, warehouseid, deliverstationid);
        }

        public int IncomeReturnsAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            return _incomeAccountDao.IncomeReturnsAccountHistory(merchantid, accountDate, warehouseid, deliverstationid);
        }

        public int IncomeVisitReturnsAccountHistory(int merchantid, DateTime accountDate, int warehouseid, int deliverstationid)
        {
            return _incomeAccountDao.IncomeVisitReturnsAccountHistory(merchantid, accountDate, warehouseid, deliverstationid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="merchantid"></param>
        /// <param name="accountDate"></param>
        /// <param name="deliverstationid"></param>
        /// <returns></returns>
        public int IncomeOtherReceiveFeeHistory(int merchantid, DateTime accountDate, int deliverstationid)
        {
            return _incomeAccountDao.IncomeOtherReceiveFeeHistory(merchantid, accountDate, deliverstationid);
        }

        public int IsIncomeHistoryCount(DateTime accountDate)
        {
            return _incomeAccountDao.IsIncomeHistoryCount(accountDate);
        }

	    #endregion 
    }
}
