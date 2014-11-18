using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet.UnitOfWork;
using System.Data.SqlClient;
using System.Xml;
using RFD.FMS.MODEL.Enumeration;
using System.Threading;
using RFD.FMS.Service.COD;
using AutoMapper;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class AreaExpressLevelIncomeService : IAreaExpressLevelIncomeService
    {
        private IAreaExpressLevelIncomeDao _areaExpressLevelIncomeDao;

        public void MapAreaExpressLevelIncome(AreaExpressLevelIncome info, ref AreaExpressLevelIncome newInfo)
        {
            Mapper.CreateMap<AreaExpressLevelIncome, AreaExpressLevelIncome>();
            newInfo = Mapper.Map(info, newInfo, info.GetType(), typeof(AreaExpressLevelIncome)) as AreaExpressLevelIncome;
        }

        //查询区域类型
        public DataTable SearchAreaMerchantLevel(int status, string areaid, string cityid, string provinceid, int merchantid, int areatype,int expresscompanyid,string distributionCode, ref PageInfo pi)
        {
            return _areaExpressLevelIncomeDao.SearchAreaMerchantLevel(status, areaid, cityid, provinceid, merchantid, areatype, expresscompanyid,distributionCode, ref pi);
        }

        //查询区域信息
        public DataTable SearchAreaMerchantLevelDetail(string areaid, int status, string distributionCode)
        {
            return _areaExpressLevelIncomeDao.SearchAreaMerchantLevelDetail(areaid, status, distributionCode);
        }

        //查询区域详细信息1
        public DataTable SearchAreaMerchantLevelDetail(string areaid, int status,int merchantId,int areatype,int expresscompanyid,string distributionCode)
        {
            return _areaExpressLevelIncomeDao.SearchAreaMerchantLevelDetail(areaid, status, merchantId, areatype, expresscompanyid, distributionCode);
        }

        public DataTable SearchArea(string provinceId, string cityId, string areaId, string merchantId, string distributionCode)
		{
            return _areaExpressLevelIncomeDao.SearchArea(provinceId, cityId, areaId, merchantId, distributionCode);
		}

        public DataTable SearchAreaType(string areaId, string distributionCode)
		{
            return _areaExpressLevelIncomeDao.SearchAreaType(areaId, distributionCode);
		}

		public bool AddAreaType(List<AreaExpressLevelIncome> areaExpressLevelIncomes, out string msg)
		{
			try
			{
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					StringBuilder sbMsg = new StringBuilder();

					foreach (AreaExpressLevelIncome areaExpressLevelIncome in areaExpressLevelIncomes)
					{
                        var areaExpressLevelIncome_tmp = areaExpressLevelIncome;
						//重复
                        if (!_areaExpressLevelIncomeDao.AddAreaType(ref areaExpressLevelIncome_tmp))
						{
                            sbMsg.Append(areaExpressLevelIncome_tmp.CompanyName + "," + areaExpressLevelIncome_tmp.AreaName + "," + areaExpressLevelIncome_tmp.MerchantName + "," + areaExpressLevelIncome_tmp.GoodsCategoryName +","+areaExpressLevelIncome.ExpressName+ " <br>");
						}
						else
						{
                            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                            if (!accountOperatorLogService.AddOperatorLogLog(areaExpressLevelIncome_tmp.AutoId.ToString(), areaExpressLevelIncome_tmp.CreateBy,
                                "新增，未审核", (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6)) { msg = ""; return false; }
						}
					}

					work.Complete();

                    if (sbMsg.ToString().Length > 0)
                    {
                        msg = sbMsg.ToString() + "已维护区域类型，请执行更新操作";
                    }
                    else
                    {
                        msg = "";
                    }

					return true;
				}
			}
			catch (Exception ex)
			{
				msg = "";
				return false;
			}
		}

		public bool UpdateAreaType(List<AreaExpressLevelIncome> areaExpressLevelIncomes, out string msg)
		{
			try
			{
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					StringBuilder sbMsg = new StringBuilder();
					foreach (AreaExpressLevelIncome areaExpressLevelIncome in areaExpressLevelIncomes)
					{
						//重复
						int autoId = 0;
                        if (!_areaExpressLevelIncomeDao.UpdateAreaType(areaExpressLevelIncome, out autoId))
						{
							sbMsg.Append(areaExpressLevelIncome.CompanyName + "," + areaExpressLevelIncome.AreaName + "," + areaExpressLevelIncome.MerchantName + " <br>");
						}
						else
						{
                            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                            if (!accountOperatorLogService.AddOperatorLogLog(autoId.ToString(), areaExpressLevelIncome.UpdateBy,
                                string.Format("修改为{0}，未审核", areaExpressLevelIncome.EffectAreaType), (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6)) { msg = ""; return false; }
						}
					}
					work.Complete();
					if (sbMsg.ToString().Length > 0)
						msg = sbMsg.ToString() + "未维护区域类型，请执行添加操作";
					else
						msg = "";
					return true;
				}
			}
			catch (Exception ex)
			{
				msg = "";
				return false;
			}
		}

        public bool UpdateAreaTypeV2(List<AreaExpressLevelIncome> modelList)
        {
             foreach (AreaExpressLevelIncome areaExpressLevelIncome in modelList)
             {
                 //判断是否重复
                 DataTable dataTable = _areaExpressLevelIncomeDao.GetAreaExpressByID(areaExpressLevelIncome.AutoId);
                 if (dataTable != null && dataTable.Rows.Count > 0)
                 {
                     var areaExpress = new AreaExpressLevelIncome
                                           {
                                               MerchantID = int.Parse(dataTable.Rows[0]["MerchantID"].ToString()),
                                               EffectAreaType = null,
                                               GoodsCategoryCode = dataTable.Rows[0]["GoodsCategoryCode"].ToString(),
                                               WareHouseID = dataTable.Rows[0]["WareHouseID"].ToString(),
                                               ExpressCompanyID =
                                                   int.Parse(dataTable.Rows[0]["ExpressCompanyID"].ToString()),
                                               DistributionCode = dataTable.Rows[0]["DistributionCode"].ToString()
                                               ,
                                               AreaID = dataTable.Rows[0]["AreaID"].ToString()   
                                           };

                     if (!_areaExpressLevelIncomeDao.ExistAreaExpress(areaExpress))
                     {
                         return false;
                     }
                 }
             }
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                foreach (AreaExpressLevelIncome areaExpressLevelIncome in modelList)
                {
                    if (!_areaExpressLevelIncomeDao.UpdateAreaTypeV2(areaExpressLevelIncome)) return false;

                    var accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                    if (!accountOperatorLogService.AddOperatorLogLog(areaExpressLevelIncome.AutoId.ToString(),
                        areaExpressLevelIncome.UpdateBy, string.Format("修改区域类型为{0}，未审核", areaExpressLevelIncome.EffectAreaType),
                        (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6)) return false;
                }

                work.Complete();
                return true;
            }
        }

        public bool UpdateExpressV2(List<AreaExpressLevelIncome> areaExpressLevelIncomes)
        {
            foreach (AreaExpressLevelIncome areaExpressLevelIncome in areaExpressLevelIncomes)
            {
                //判断是否重复
                DataTable dataTable = _areaExpressLevelIncomeDao.GetAreaExpressByID(areaExpressLevelIncome.AutoId);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    var areaExpress = new AreaExpressLevelIncome
                                          {
                                              MerchantID = int.Parse(dataTable.Rows[0]["MerchantID"].ToString()),
                                              EffectAreaType =null,
                                              GoodsCategoryCode = dataTable.Rows[0]["GoodsCategoryCode"].ToString(),
                                              WareHouseID = dataTable.Rows[0]["WareHouseID"].ToString(),
                                              ExpressCompanyID = areaExpressLevelIncome.ExpressCompanyID,
                                              DistributionCode = dataTable.Rows[0]["DistributionCode"].ToString(),
                                              AreaID = dataTable.Rows[0]["AreaID"].ToString()
                                              
                                          };

                    if (!_areaExpressLevelIncomeDao.ExistAreaExpress(areaExpress))
                    {
                        return false;
                    }
                }
            }
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                foreach (AreaExpressLevelIncome areaExpressLevelIncome in areaExpressLevelIncomes)
                {
                    if (!_areaExpressLevelIncomeDao.UpdateExpressV2(areaExpressLevelIncome)) return false;

                    var accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                    if (!accountOperatorLogService.AddOperatorLogLog(areaExpressLevelIncome.AutoId.ToString(),
                        areaExpressLevelIncome.UpdateBy, string.Format("修改配送商为{0}，未审核", areaExpressLevelIncome.ExpressName),
                        (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6)) return false;
                }

                work.Complete();
                return true;
            }
        }

        public bool DeleteAreaType(IList<KeyValuePair<string, string>> keyValuePairs, int updateBy)
		{
			try
			{
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
					{
                        if (!_areaExpressLevelIncomeDao.DeleteAreaType(keyValuePair.Key, updateBy))
							return false;
						else
						{
                            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                            if (!accountOperatorLogService.AddOperatorLogLog(keyValuePair.Key, updateBy,
                                "删除，未审核", (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6)) { return false; }
						}
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
        /// 设置生效
        /// </summary>
        /// <param name="areas"></param>
        /// <param name="doDate"></param>
        /// <param name="areaMerchantLevelIncomeLog"></param>
        /// <returns></returns>
        public bool SetAreaMerchantLeverAudit(string areas, DateTime doDate, AreaExpressLevelIncomeLog areaMerchantLevelIncomeLog)
        {
            try
            {
                string[] areaId = areas.Split(',');
                DataTable dataTable = null;

                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    for (int f = 0; f < areaId.Length; f++)
                    {
                        //获取待审批的区域
                        areaMerchantLevelIncomeLog.AreaID = areaId[f].ToString();
                        dataTable = _areaExpressLevelIncomeDao.SearchAreaMerchantLevelDetail(areaId[f], 0, areaMerchantLevelIncomeLog.DistributionCode);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            int id = int.Parse(dataTable.Rows[i]["autoid"].ToString());
                            _areaExpressLevelIncomeDao.SetAreaMerchantLeverAudit(id, doDate, areaMerchantLevelIncomeLog.CreateBy, DateTime.Now);

                            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                            accountOperatorLogService.AddOperatorLogLog(id.ToString(), 0,
                                "审批成功,设置生效时间为：" + doDate, (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6);
                        }
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
        /// 设置生效new
        /// </summary>
        /// <param name="areas"></param>
        /// <param name="doDate"></param>
        /// <param name="areaMerchantLevelIncomeLog"></param>
        /// <returns></returns>
        public bool SetAreaMerchantLeverAuditEx(string areas, DateTime doDate, int auditstatus,AreaExpressLevelIncomeLog areaMerchantLevelIncomeLog)
        {
            try
            {
                string[] areaId = areas.Split(',');
                DataTable dataTable = null;

                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    for (int f = 0; f < areaId.Length; f++)
                    {
                        //获取待审批的区域
                        areaMerchantLevelIncomeLog.AreaID = areaId[f].ToString();
                        dataTable = _areaExpressLevelIncomeDao.SearchAreaMerchantLevelDetail(areaId[f], auditstatus, areaMerchantLevelIncomeLog.DistributionCode);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            int id = int.Parse(dataTable.Rows[i]["autoid"].ToString());
                            _areaExpressLevelIncomeDao.SetAreaMerchantLeverAuditEx(id, doDate, areaMerchantLevelIncomeLog.CreateBy, auditstatus, DateTime.Now);

                            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                            accountOperatorLogService.AddOperatorLogLog(id.ToString(), 0,
                                "审批成功,设置生效时间为：" + doDate, (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6);
                        }
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

        //返回待生效的区域
        public DataTable AreaMerchantLevelNum(int num, DateTime nowDate)
        {
            return _areaExpressLevelIncomeDao.AreaMerchantLevelNum(num, nowDate);
        }

        //更新收入区域类型
        public bool AreaMerchantLevelUpdate(int autoid, string areaid, int merchantid, int areatype, string warehouseid,int expressId)
        {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {

                    _areaExpressLevelIncomeDao.AreaMerchantLevelUpdate(autoid);

                    IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                    accountOperatorLogService.AddOperatorLogLog(autoid.ToString(), 0,
                        "此区域类型已经生效", (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6);

                    work.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //添加收入区域类型
        public bool AreaMerchantLevelAdd(int autoid, string areaid, int merchantid, int areatype, string warehouseid,int expressId)
        {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {

                    _areaExpressLevelIncomeDao.AreaMerchantLevelAdd(autoid);

                    IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                    accountOperatorLogService.AddOperatorLogLog(autoid.ToString(), 0,
                        "此区域类型已经生效", (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6);

                    work.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //删除收入区域类型
        public bool AreaMerchantLevelDel(int autoid, string areaid, int merchantid, int areatype, string warehouseid,int expressId)
        {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {

                    _areaExpressLevelIncomeDao.AreaMerchantLevelDel(autoid);

                    IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                    accountOperatorLogService.AddOperatorLogLog(autoid.ToString(), 0,
                        "此区域类型已经生效", (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6);

                    work.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
		}

		public DataTable GetSortingCenter()
		{
            return _areaExpressLevelIncomeDao.GetSortingCenter();
		}

		#region 导入
        public bool ExportAreaType(DataTable dt, int createBy, out DataTable dtError, string distributionCode,int expressCompanyId)
		{
			if (dt == null || dt.Rows.Count <= 0)
			{
				dtError = null;
				return false;
			}

			dtError = dt.Clone();
			StringBuilder sbError;
			dtError.Columns.Add("错误描述");
			DataRow r;
            AreaExpressLevelIncome areaExpressLevelIncome;
            IGoodsCategoryService goodsCategoryService = ServiceLocator.GetService<IGoodsCategoryService>();
            DataSet ds = _areaExpressLevelIncomeDao.GetExportData();
            DataTable dtCategory = goodsCategoryService.GetGoodsCategoryByMerchantID(-1, distributionCode).Copy();
            dtCategory.TableName = "dtCategory";
            ds.Tables.Add(dtCategory);

            IDeliverFeeService deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
            var condition = new SearchCondition()
            {
                MerchantID = -1,
                DistributionCode = distributionCode,
            };
            PageInfo pi = new PageInfo(Int32.MaxValue);
            DataTable dtMerchantBaisc = deliverFeeService.BindDeliverFeeList(condition,ref pi).Copy();
            dtMerchantBaisc.TableName = "dtMerchantBaisc";
            ds.Tables.Add(dtMerchantBaisc);

			foreach (DataRow dr in dt.Rows)
			{
				if (!string.IsNullOrEmpty(dr[0].ToString()) ||
					!string.IsNullOrEmpty(dr[1].ToString()) ||
					!string.IsNullOrEmpty(dr[2].ToString()) ||
					!string.IsNullOrEmpty(dr[3].ToString()) ||
					!string.IsNullOrEmpty(dr[4].ToString()) ||
					!string.IsNullOrEmpty(dr[6].ToString())||
                    !string.IsNullOrEmpty(dr[7].ToString())
					)
				{
					sbError = new StringBuilder();
					areaExpressLevelIncome = new AreaExpressLevelIncome();
					if (JudgeDataRow(dr, ds, out sbError, ref areaExpressLevelIncome,expressCompanyId))
					{
                        //areaExpressLevelIncome.ExpressCompanyID = expressCompanyId;
						areaExpressLevelIncome.CreateBy = createBy;
						areaExpressLevelIncome.Enable = 3;
						areaExpressLevelIncome.AuditStatus = (int)AreaLevelStatus.S1;
                        areaExpressLevelIncome.DistributionCode = distributionCode;
                        if (_areaExpressLevelIncomeDao.AddAreaType(ref areaExpressLevelIncome))
						{
                            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                            if (!accountOperatorLogService.AddOperatorLogLog(areaExpressLevelIncome.AutoId.ToString(), createBy,
                                "批量新增，未审核", (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6)) { return false; }
						}
						else
						{
							r = dtError.NewRow();
							r.ItemArray = dr.ItemArray;
							r["错误描述"] = "已维护区域类型";
							dtError.Rows.Add(r);
						}
					}
					else
					{
						r = dtError.NewRow();
						r.ItemArray = dr.ItemArray;
						r["错误描述"] = sbError.ToString().TrimEnd('/');
						dtError.Rows.Add(r);
					}
				}
			}

			return true;
		}

		private bool JudgeDataRow(DataRow dr, DataSet ds, out StringBuilder sbError, ref AreaExpressLevelIncome areaExpressLevelIncome,int expressCompanyId)
		{
            object obj;
            bool flag = true;
            sbError = new StringBuilder();

            #region 省市区
            if (!string.IsNullOrEmpty(dr[0].ToString()) &&
                !string.IsNullOrEmpty(dr[1].ToString()) &&
                !string.IsNullOrEmpty(dr[2].ToString()))
            {
                DataRow[] drs = ds.Tables[1].Select(string.Format("ProvinceName='{0}' and CityName='{1}' and AreaName='{2}'",
                                    dr[0].ToString().Trim(), dr[1].ToString().Trim(), dr[2].ToString().Trim()));
                if (drs.Length == 1)
                    areaExpressLevelIncome.AreaID = drs[0]["AreaID"].ToString();
                else
                {
                    flag = false;
                    sbError.Append(" 区域ID未能找到 /");
                }
            }
            else
            {
                flag = false;
                sbError.Append(" 省市区不能为空 /");
            }
            #endregion

            #region 商家
            obj = dr[3];
            if (obj.ToString().Trim() != "")
            {
                DataRow[] drs = ds.Tables[0].Select(string.Format("MerchantName='{0}'", obj.ToString().Trim()));
                if (drs.Length == 1)
                    areaExpressLevelIncome.MerchantID = int.Parse(drs[0]["ID"].ToString());
                else
                {
                    flag = false;
                    sbError.Append(" 商家ID未能找到 /");
                }
            }
            else
            {
                flag = false;
                sbError.Append(" 商家不能为空 /");
            }
            #endregion

            #region 分拣中心
            obj = dr[4];
            if (obj.ToString().Trim() != "")
            {
                DataRow[] drsHouse = ds.Tables[3].Select(string.Format("CompanyName='{0}'", obj.ToString().Trim()));
                if (drsHouse.Length == 1)
                    areaExpressLevelIncome.WareHouseID = drsHouse[0]["ExpressCompanyID"].ToString();
                else
                {
                    flag = false;
                    sbError.Append(" 分拣中心ID未能找到 /");
                }
            }
            else
            {
                flag = false;
                sbError.Append(" 分拣中心不能为空 /");
            }
            #endregion

            #region 商家基础信息设置验证
            int merchantId = areaExpressLevelIncome.MerchantID;
            int isCategory = -1;
            List<DataRow> rowList = ds.Tables["dtMerchantBaisc"].AsEnumerable().Where(row =>
            {
                return int.Parse(row["MerchantID"].ToString()) == merchantId && int.Parse(row["Status"].ToString()) == (int)MaintainStatus.Audited;
            }).ToList();
            if (rowList.Count == 0)
            {
                flag = false;
                sbError.Append(" 请先设置商家基础信息且已审核后设置 /");
            }
            else
            {
                if (rowList[0]["IsCategory"] == DBNull.Value)
                {
                    flag = false;
                    sbError.Append(" 请先设置商家是否按品类结算且已审核 /");
                }
                else
                {
                    isCategory = int.Parse(rowList[0]["IsCategory"].ToString());
                }
            }
            #endregion

            #region 货物品类
            obj = dr[5];
          if (isCategory == 1)
                { 
                     if (obj.ToString().Trim() != "" && rowList.Count > 0)
                       {
                
                            DataRow[] drs = ds.Tables["dtCategory"].Select(string.Format("Name='{0}' AND MerchantID={1}", obj.ToString().Trim(), merchantId));
                            if (drs.Length == 1)
                            {
                                areaExpressLevelIncome.GoodsCategoryCode = drs[0]["Code"].ToString();
                            }
                            else
                            {
                                flag = false;
                                sbError.Append(" 商家下没有此货物品类 /");
                            }
                        }
                     else if ( string.IsNullOrEmpty(obj.ToString().Trim()))
                     {
                         flag = false;
                         sbError.Append(" 商家货物品类为空 /"); 
                     }
            }  
          else if (obj.ToString().Trim() != "")
          {
              flag = false;
              sbError.Append(" 此商家不按货物品类设置 /");
          }
            #endregion

            #region 区域类型
            obj = dr[6];
            if (obj.ToString().Trim() != "")
            {
                DataRow[] drs = ds.Tables[2].Select(string.Format("StatusNO='{0}'", obj.ToString().Trim()));
                if (drs.Length == 1)
                {
                    areaExpressLevelIncome.EffectAreaType = int.Parse(drs[0]["StatusNO"].ToString());
                }
                else
                {
                    flag = false;
                    sbError.Append(" 区域类型ID未能找到 /");
                }
            }
            else
            {
                flag = false;
                sbError.Append(" 区域类型不能为空 /");
            }
            #endregion

            #region 配送商
            obj = dr[7];
            if (obj.ToString().Trim() != "")
            {
                if (obj.ToString().Trim()=="全部")
                {
                    areaExpressLevelIncome.ExpressCompanyID = expressCompanyId;
                    areaExpressLevelIncome.IsExpress = 0;
                }
                else
                {
                    DataRow[] drs = ds.Tables[4].Select(string.Format("ExpressName='{0}'", obj.ToString().Trim()));
                    if (drs.Length == 1)
                    {
                        areaExpressLevelIncome.ExpressCompanyID = drs[0]["ExpressID"].ToString().TryGetInt();
                        areaExpressLevelIncome.IsExpress = 1;
                    }
                    else
                    {
                        flag = false;
                        sbError.Append(" 配送商未能找到 /");
                    } 
                }
            }
            else
            {
                flag = false;
                sbError.Append(" 配送商不能为空 /");
            }
            #endregion
            return flag;
		}
		#endregion

        //设置置回
        public bool ReSetAreaMerchantLevel(string areas, AreaExpressLevelIncomeLog areaMerchantLevelIncomeLog)
        {
            try
            {
                string[] areaId = areas.Split(',');
                DataTable dataTable = null;

                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    for (int f = 0; f < areaId.Length; f++)
                    {
                        //获取待审批的区域
                        areaMerchantLevelIncomeLog.AreaID = areaId[f].ToString();
                        dataTable = _areaExpressLevelIncomeDao.SearchAreaMerchantLevelDetail(areaId[f], 0, areaMerchantLevelIncomeLog.DistributionCode);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            int id = int.Parse(dataTable.Rows[i]["autoid"].ToString());
                            _areaExpressLevelIncomeDao.ReSetAreaMerchantLevel(id);

                            IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                            accountOperatorLogService.AddOperatorLogLog(id.ToString(), 0,
                                "此区域类型设置已经被置回", (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6);
                        }
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

		public DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId,string auditStatus,string distributionCode, ref PageInfo pi)
		{
            return _areaExpressLevelIncomeDao.SearchAreaTypeList(provinceId, cityId, areaId, expressCompanyId, areaType, wareHouse, merchantId, auditStatus, distributionCode, pi);
		}

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
				xe.SetAttribute("v", houses[i].Trim());
				xmlDoc.DocumentElement.AppendChild(xe);
			}
			return xmlDoc;
		}

        public DataTable SearchAreaTypeExprotList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode)
		{
            return _areaExpressLevelIncomeDao.SearchAreaTypeExprotList(provinceId, cityId, areaId, expressCompanyId, areaType, wareHouse, merchantId, auditStatus, distributionCode);
		}

        public DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string merchantId, string distributionCode)
		{
            return _areaExpressLevelIncomeDao.SearchAreaTypeLog(areaId, expressCompanyId, wareHouse, merchantId, distributionCode);
		}

        public DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel, ref PageInfo pi)
        {
            int rowCount = _areaExpressLevelIncomeDao.GetAreaLevelIncomeListStat(searchModel);

            if (rowCount <= 0)
                return null;
            pi.ItemCount = rowCount;
            return _areaExpressLevelIncomeDao.GetAreaLevelIncomeList(searchModel,pi);
        }

        public DataTable GetAreaLevelIncomeExprotList(AreaLevelIncomeSearchModel searchModel)
        {
            return _areaExpressLevelIncomeDao.GetAreaLevelIncomeExprotList(searchModel);
        }
        public DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel)
        {
            return _areaExpressLevelIncomeDao.GetAreaLevelIncomeList(searchModel);
        }

        public bool UpdateAreaLevelIncomeStatus(List<AreaExpressLevelIncome> modelList)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                foreach (AreaExpressLevelIncome model in modelList)
                {
                    if (!_areaExpressLevelIncomeDao.UpdateAreaLevelIncomeStatus(model))
                    {
                        return false;
                    }
                    else
                    {
                        DataTable dt = _areaExpressLevelIncomeDao.SearchAreaTypeByAutoId(model.AutoId.ToString());
                        IAccountOperatorLogService accountOperatorLogService = ServiceLocator.GetService<IAccountOperatorLogService>();
                        if (!accountOperatorLogService.AddOperatorLogLog(model.AutoId.ToString(), model.AuditBy,
                            EnumHelper.GetDescription((AreaLevelStatus)model.AuditStatus) + (string.IsNullOrEmpty(model.DoDate.ToString()) ? "" : (",生效日期：" + model.DoDate.ToString())), (int)RFD.FMS.MODEL.BizEnums.OperatorlogType.L6)) { return false; }
                    }
                }
                work.Complete();
            }
            return true;
        }

        public void IncomeAreaLevelToEffect(int rowCount)
        {
            //获取生效数据
            DataTable dt = _areaExpressLevelIncomeDao.GetWaitEffectList();
            if (dt == null || dt.Rows.Count <= 0)
                return;

            int n = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (n == rowCount)
                {
                    n = 0;
                    Thread.Sleep(1000);
                }
                AreaExpressLevelIncome model = new AreaExpressLevelIncome();
                model.AutoId = DataConvert.ToInt(dr["autoid"].ToString());
                model.AreaID = dr["AreaID"].ToString();
                model.MerchantID = DataConvert.ToInt(dr["MerchantID"].ToString());
                model.EffectAreaType = DataConvert.ToInt(dr["EffectAreaType"].ToString());
                model.WareHouseID = dr["WareHouseID"].ToString();
                model.ExpressCompanyID = DataConvert.ToInt(dr["ExpressCompanyID"].ToString(), 0);
                model.Enable = DataConvert.ToInt(dr["IsEnable"].ToString(), 0);
                model.AreaType = DataConvert.ToInt(dr["AreaType"].ToString(), 0);

                if (model.Enable == 1)//更新Areatype
                {
                    AreaMerchantLevelUpdate(model.AutoId, model.AreaID, model.MerchantID, int.Parse(model.EffectAreaType.ToString()), model.WareHouseID, model.ExpressCompanyID);
                }
                else if (model.Enable == 3)//新增Areatype
                {
                    AreaMerchantLevelAdd(model.AutoId, model.AreaID, model.MerchantID, int.Parse(model.EffectAreaType.ToString()), model.WareHouseID, model.ExpressCompanyID);
                }
                else if (model.Enable == 2)//删除Areatype
                {
                    AreaMerchantLevelDel(model.AutoId, model.AreaID, model.MerchantID, int.Parse(model.EffectAreaType.ToString()), model.WareHouseID, model.ExpressCompanyID);
                }

                n++;
            }
        }
    }
}
