using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using RFD.FMS.Util;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.MODEL;
using System.Data.SqlClient;
using System.Xml;
using RFD.FMS.MODEL.Enumeration;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class AreaExpressLevelService : IAreaExpressLevelService
    {
		private IAreaExpressLevelService OracleService;
        private IAreaExpressLevelDao _areaExpressLevelDao;

        public AreaExpressLevelService()
		{

		}

        public DataTable SearchArea(string provinceId, string cityId, string areaId, string stationId, string merchantId, string distributionCode,ref PageInfo pi)
		{
        	if (OracleService!=null)
        	{
        		return OracleService.SearchArea(provinceId, cityId, areaId, stationId, merchantId, distributionCode,ref pi);
        	}
            return _areaExpressLevelDao.SearchArea(provinceId, cityId,areaId, stationId, merchantId, distributionCode,ref pi);
		}

        public DataTable SearchAreaType(string areaId, string distributionCode,ref PageInfo pi)
		{
			if (OracleService != null)
			{
				return OracleService.SearchAreaType(areaId, distributionCode,ref pi);
			}
        	return _areaExpressLevelDao.SearchAreaType(areaId, distributionCode,ref pi);
		}

		public bool AddAreaType(List<AreaExpressLevel> areaExpressLevels,out string msg)
		{
            if (OracleService != null)
            {
                return OracleService.AddAreaType(areaExpressLevels, out msg);
            }

			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					StringBuilder sbMsg = new StringBuilder();
					foreach (AreaExpressLevel areaExpressLevel in areaExpressLevels)
					{
						//重复
                        if (!_areaExpressLevelDao.AddAreaType(areaExpressLevel))
						{
							sbMsg.Append(areaExpressLevel.AreaName + "," + areaExpressLevel.CompanyName + " <br>");
						}
						else
						{
							AreaExpressLevelLog areaExpressLevelLog = new AreaExpressLevelLog();
							areaExpressLevelLog.AreaID = areaExpressLevel.AreaID;
							areaExpressLevelLog.ExpressCompanyID = areaExpressLevel.ExpressCompanyID;
							areaExpressLevelLog.WarehouseId = areaExpressLevel.WareHouseID;
							areaExpressLevelLog.AreaType = areaExpressLevel.AreaType;
							areaExpressLevelLog.WareHouseType = areaExpressLevel.WareHouseType;
							areaExpressLevelLog.MerchantID = areaExpressLevel.MerchantID;
							areaExpressLevelLog.ProductID = areaExpressLevel.ProductID;
							areaExpressLevelLog.Enable = areaExpressLevel.Enable;
							areaExpressLevelLog.LogText = "新增，未审核";
							areaExpressLevelLog.CreateBy = areaExpressLevel.CreateBy;
							areaExpressLevelLog.CreateTime = DateTime.Now;
                            areaExpressLevelLog.DistributionCode = areaExpressLevel.DistributionCode;
                            _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
						}
					}
					work.Complete();
					if (sbMsg.ToString().Length > 0)
						msg = sbMsg.ToString()+"已维护区域类型，请执行更新操作";
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

		public bool UpdateAreaType(List<AreaExpressLevel> areaExpressLevels, out string msg)
		{
            if (OracleService != null)
            {
                return OracleService.UpdateAreaType(areaExpressLevels, out msg);
            }
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					StringBuilder sbMsg = new StringBuilder();
					foreach (AreaExpressLevel areaExpressLevel in areaExpressLevels)
					{
						//重复
						int autoId = 0;
                        if (!_areaExpressLevelDao.UpdateAreaType(areaExpressLevel, out autoId))
						{
							sbMsg.Append(areaExpressLevel.AreaName + "," + areaExpressLevel.CompanyName + " <br>");
						}
						else
						{
							AreaExpressLevelLog areaExpressLevelLog = new AreaExpressLevelLog();
                            DataTable dt = _areaExpressLevelDao.SearchAreaTypeByAutoId(autoId.ToString());
							if (dt == null || dt.Rows.Count <= 0)
								continue;
							areaExpressLevelLog.AreaID = areaExpressLevel.AreaID;
							areaExpressLevelLog.ExpressCompanyID = areaExpressLevel.ExpressCompanyID;
							areaExpressLevelLog.WarehouseId = areaExpressLevel.WareHouseID;
							areaExpressLevelLog.WareHouseType = int.Parse(dt.Rows[0]["WareHouseType"].ToString());
							areaExpressLevelLog.MerchantID = areaExpressLevel.MerchantID;
							areaExpressLevelLog.ProductID = areaExpressLevel.ProductID;
							areaExpressLevelLog.AreaType = int.Parse(dt.Rows[0]["AreaType"].ToString());
							areaExpressLevelLog.Enable = int.Parse(dt.Rows[0]["Enable"].ToString());
							areaExpressLevelLog.LogText = string.Format("修改为{0}，未审核", areaExpressLevel.EffectAreaType);
							areaExpressLevelLog.CreateBy = areaExpressLevel.UpdateBy;
							areaExpressLevelLog.CreateTime = DateTime.Now;
                            areaExpressLevelLog.DistributionCode = areaExpressLevel.DistributionCode;
                            _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
						}
					}
					work.Complete();
					if (sbMsg.ToString().Length > 0)
						msg = sbMsg.ToString() + "未维护区域类型或仓库、分拣有出入，请执行添加操作";
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

		public bool DeleteAreaType(IList<KeyValuePair<string, string>> keyValuePairs,string updateBy)
		{
            if (OracleService != null)
            {
                return OracleService.DeleteAreaType(keyValuePairs, updateBy);
            }

			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
					{
                        if (!_areaExpressLevelDao.DeleteAreaType(keyValuePair.Key, updateBy))
							return false;
						else
						{
							AreaExpressLevelLog areaExpressLevelLog = new AreaExpressLevelLog();
                            DataTable dt = _areaExpressLevelDao.SearchAreaTypeByAutoId(keyValuePair.Key);
							if (dt == null || dt.Rows.Count <= 0)
								continue;

							areaExpressLevelLog.AreaID = dt.Rows[0]["AreaID"].ToString();
							areaExpressLevelLog.ExpressCompanyID = int.Parse(dt.Rows[0]["ExpressCompanyID"].ToString());
							areaExpressLevelLog.WarehouseId = dt.Rows[0]["WareHouseID"].ToString();
							areaExpressLevelLog.AreaType = int.Parse(dt.Rows[0]["AreaType"].ToString());
							areaExpressLevelLog.Enable = int.Parse(dt.Rows[0]["Enable"].ToString());
							areaExpressLevelLog.WareHouseType = int.Parse(dt.Rows[0]["WareHouseType"].ToString());
							areaExpressLevelLog.MerchantID = int.Parse(dt.Rows[0]["MerchantID"].ToString());
							areaExpressLevelLog.ProductID = int.Parse(dt.Rows[0]["ProductID"].ToString());
							areaExpressLevelLog.LogText = "删除，未审核";
							areaExpressLevelLog.CreateBy = updateBy;
							areaExpressLevelLog.CreateTime = DateTime.Now;
                            _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
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

        #region 区域类型审批
        //查询区域类型
        public DataTable SearchAreaExpressCompanyLevel(int status, string areaid, string cityid, string provinceid, int expresscompanyid, int areatype,int type,string warehouseid,int merchantid,string distributionCode, ref PageInfo pi)
        {
        	if (OracleService!=null)
        	{
        		return OracleService.SearchAreaExpressCompanyLevel(status, areaid, cityid, provinceid, expresscompanyid,
        		                                                   areatype, type, warehouseid, merchantid, distributionCode,
        		                                                   ref pi);
        	}
            return _areaExpressLevelDao.SearchAreaExpressCompanyLevel(status, areaid, cityid, provinceid, expresscompanyid, areatype, type, warehouseid, merchantid,distributionCode, ref pi);
        }

        //查询区域信息
        public DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, string distributionCode)
        {
        	if (OracleService!=null)
        	{
        		return OracleService.SearchAreaExpressCompanyLevelDetail(areaid, status, distributionCode);
        	}
            return _areaExpressLevelDao.SearchAreaExpressCompanyLevelDetail(areaid, status, distributionCode);
        }

        //查询区域详细信息
        public DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, int expresscompanyid, int areatype, int warehousetype, string warehouseid, int merchantid)
        {
        	if (OracleService!=null)
        	{
        		return OracleService.SearchAreaExpressCompanyLevelDetail(areaid, status, expresscompanyid, areatype,
        		                                                         warehousetype, warehouseid, merchantid);
        	}
            return _areaExpressLevelDao.SearchAreaExpressCompanyLevelDetail(areaid, status, expresscompanyid, areatype, warehousetype, warehouseid, merchantid);
        }

        public bool SetAreaExpressCompanyLeverAudit(string areas, DateTime doDate,AreaExpressLevelLog areaExpressLevelLog)
        {
            try
            {
                string[] areaId= areas.Split(',');
                DataTable dataTable = null;

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    for (int f = 0; f < areaId.Length; f++)
                    {
                        //获取待审批的区域
                        areaExpressLevelLog.AreaID = areaId[f].ToString();
                        dataTable = _areaExpressLevelDao.SearchAreaExpressCompanyLevelDetail(areaId[f], 0, areaExpressLevelLog.DistributionCode);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            int id = int.Parse(dataTable.Rows[i]["autoid"].ToString());
                            _areaExpressLevelDao.SetAreaExpressCompanyLeverAudit(id, doDate, int.Parse(areaExpressLevelLog.CreateBy), DateTime.Now);

                            areaExpressLevelLog.AreaID = dataTable.Rows[i]["areaid"].ToString();
                            areaExpressLevelLog.ExpressCompanyID = int.Parse(dataTable.Rows[i]["expresscompanyid"].ToString());
                            areaExpressLevelLog.AreaType = int.Parse(dataTable.Rows[i]["areatype"].ToString());
                            areaExpressLevelLog.LogText = "审批成功,设置生效时间为：" +doDate;
                            
                            areaExpressLevelLog.WarehouseId = dataTable.Rows[i]["warehouseid"].ToString();

                            if(!string.IsNullOrEmpty(dataTable.Rows[i]["merchantid"].ToString()))
                            {
                                areaExpressLevelLog.MerchantID =  int.Parse(dataTable.Rows[i]["merchantid"].ToString());
                            }

                            if(!string.IsNullOrEmpty(dataTable.Rows[i]["productid"].ToString()))
                            {
                                areaExpressLevelLog.ProductID = int.Parse(dataTable.Rows[i]["productid"].ToString());
                            }

                            if(!string.IsNullOrEmpty(dataTable.Rows[i]["warehousetype"].ToString()))
                            {
                                areaExpressLevelLog.WareHouseType =
                                    int.Parse(dataTable.Rows[i]["warehousetype"].ToString());
                            }

                            _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
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
        /// 添加置回审核
        /// </summary>
        /// <param name="areas"></param>
        /// <param name="doDate"></param>
        /// <param name="auditstatus"></param>
        /// <param name="areaExpressLevelLog"></param>
        /// <returns></returns>
        public bool SetAreaExpressCompanyLeverAuditEx(string areas, DateTime doDate,int auditstatus, AreaExpressLevelLog areaExpressLevelLog)
        {
            if (OracleService != null)
            {
                return OracleService.SetAreaExpressCompanyLeverAuditEx(areas, doDate,auditstatus, areaExpressLevelLog);
            }

            try
            {
                string[] areaId = areas.Split(',');
                DataTable dataTable = null;

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    for (int f = 0; f < areaId.Length; f++)
                    {
                        //获取待审批的区域
                        areaExpressLevelLog.AreaID = areaId[f].ToString();

                        dataTable = _areaExpressLevelDao.SearchAreaExpressCompanyLevelDetail(areaId[f], auditstatus, areaExpressLevelLog.DistributionCode);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            int id = int.Parse(dataTable.Rows[i]["autoid"].ToString());
                            _areaExpressLevelDao.SetAreaExpressCompanyLeverAuditEx(id, doDate, int.Parse(areaExpressLevelLog.CreateBy), DateTime.Now, auditstatus);

                            areaExpressLevelLog.AreaID = dataTable.Rows[i]["areaid"].ToString();
                            areaExpressLevelLog.ExpressCompanyID = int.Parse(dataTable.Rows[i]["expresscompanyid"].ToString());
                            areaExpressLevelLog.AreaType = int.Parse(dataTable.Rows[i]["areatype"].ToString());
                            areaExpressLevelLog.LogText = "审批成功,设置生效时间为：" + doDate;

                            areaExpressLevelLog.WarehouseId = dataTable.Rows[i]["warehouseid"].ToString();

                            if (!string.IsNullOrEmpty(dataTable.Rows[i]["merchantid"].ToString()))
                            {
                                areaExpressLevelLog.MerchantID = int.Parse(dataTable.Rows[i]["merchantid"].ToString());
                            }

                            if (!string.IsNullOrEmpty(dataTable.Rows[i]["productid"].ToString()))
                            {
                                areaExpressLevelLog.ProductID = int.Parse(dataTable.Rows[i]["productid"].ToString());
                            }

                            if (!string.IsNullOrEmpty(dataTable.Rows[i]["warehousetype"].ToString()))
                            {
                                areaExpressLevelLog.WareHouseType =
                                    int.Parse(dataTable.Rows[i]["warehousetype"].ToString());
                            }

                            _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
                        }
                    }

                    work.Complete();
					
                }

				//if (OracleService != null)
				//{
				//    OracleService.SetAreaExpressCompanyLeverAuditEx(areas, doDate, auditstatus, areaExpressLevelLog);
				//}
				return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        public DataTable AreaExpressCompanyLevelNum(int num,DateTime nowDate)
        {
            return _areaExpressLevelDao.AreaExpressCompanyLevelNum(num, nowDate);
        }

	    public bool AreaExpressCompanyLevelUpdate(int autoid,string areaid,int expresscompanyid,int areatype,string warehouseid,int warehousetype,int merchant,int productid)
	    {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {

                    _areaExpressLevelDao.AreaExpressCompanyLevelUpdate(autoid);

                    AreaExpressLevelLog areaExpressLevelLog=new AreaExpressLevelLog();
                    areaExpressLevelLog.AreaID = areaid;
                    areaExpressLevelLog.ExpressCompanyID = expresscompanyid;
                    areaExpressLevelLog.AreaType = areatype;
                    areaExpressLevelLog.LogText = "此区域类型已经生效！";
                    areaExpressLevelLog.WarehouseId = warehouseid;
                    areaExpressLevelLog.Enable = 1;
                    areaExpressLevelLog.CreateBy = "0";
                    areaExpressLevelLog.CreateTime = DateTime.Now;
                    areaExpressLevelLog.WareHouseType = warehousetype;
                    areaExpressLevelLog.MerchantID = merchant;
                    areaExpressLevelLog.ProductID = productid;

                    _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
                        
                    work.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
	    }


        public bool AreaExpressCompanyLevelAdd(int autoid, string areaid, int expresscompanyid, int areatype, string warehouseid,int warehousetype,int merchant, int productid)
	    {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {

                    _areaExpressLevelDao.AreaExpressCompanyLevelAdd(autoid);

                    AreaExpressLevelLog areaExpressLevelLog = new AreaExpressLevelLog();
                    areaExpressLevelLog.AreaID = areaid;
                    areaExpressLevelLog.ExpressCompanyID = expresscompanyid;
                    areaExpressLevelLog.AreaType = areatype;
                    areaExpressLevelLog.LogText = "此区域类型已经生效！";
                    areaExpressLevelLog.WarehouseId = warehouseid;
                    areaExpressLevelLog.Enable = 1;
                    areaExpressLevelLog.CreateBy = "0";
                    areaExpressLevelLog.CreateTime = DateTime.Now;
                    areaExpressLevelLog.WareHouseType = warehousetype;
                    areaExpressLevelLog.MerchantID = merchant;
                    areaExpressLevelLog.ProductID = productid;

                    _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);

                    work.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
	    }

	    public bool AreaExpressCompanyLevelDel(int autoid,string areaid,int expresscompanyid,int areatype,string warehouseid,int warehousetype,int merchant, int productid)
	    {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {

                    _areaExpressLevelDao.AreaExpressCompanyLevelDel(autoid);

                    AreaExpressLevelLog areaExpressLevelLog = new AreaExpressLevelLog();
                    areaExpressLevelLog.AreaID = areaid;
                    areaExpressLevelLog.ExpressCompanyID = expresscompanyid;
                    areaExpressLevelLog.AreaType = areatype;
                    areaExpressLevelLog.LogText = "此区域类型已经生效！";
                    areaExpressLevelLog.WarehouseId = warehouseid;
                    areaExpressLevelLog.Enable = 1;
                    areaExpressLevelLog.CreateBy = "0";
                    areaExpressLevelLog.CreateTime = DateTime.Now;
                    areaExpressLevelLog.WareHouseType = warehousetype;
                    areaExpressLevelLog.MerchantID = merchant;
                    areaExpressLevelLog.ProductID = productid;
                    _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);

                    work.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
	    }

		public bool ExportAreaType(DataTable dt,int createBy,out DataTable dtError, string distributionCode)
		{
            if (OracleService != null)
            {
                return OracleService.ExportAreaType(dt,createBy,out dtError, distributionCode);
            }

			if (dt == null || dt.Rows.Count <= 0)
			{
				dtError = null;
				return false;
			}

			dtError = dt.Clone();
			StringBuilder sbError;
			dtError.Columns.Add("错误描述");
			DataRow r;
			AreaExpressLevel areaExpressLevel;
            DataSet ds = _areaExpressLevelDao.GetExportData();
			foreach (DataRow dr in dt.Rows)
			{
				if (!string.IsNullOrEmpty(dr[0].ToString()) ||
					!string.IsNullOrEmpty(dr[1].ToString()) ||
					!string.IsNullOrEmpty(dr[2].ToString()) ||
					!string.IsNullOrEmpty(dr[3].ToString()) ||
					!string.IsNullOrEmpty(dr[4].ToString()) ||
					!string.IsNullOrEmpty(dr[5].ToString()) 
					)
				{
					sbError = new StringBuilder();
					areaExpressLevel = new AreaExpressLevel();
					if (JudgeDataRow(dr,ds, out sbError, ref areaExpressLevel))
					{
						areaExpressLevel.CreateBy = createBy.ToString();
						areaExpressLevel.Enable = 3;
                        areaExpressLevel.AuditStatus = (int)AreaLevelStatus.S1;
                        areaExpressLevel.DistributionCode = distributionCode;
                        areaExpressLevel.WareHouseID = "";
                        areaExpressLevel.WareHouseType = 0;
                        areaExpressLevel.ProductID = 1;
                        if (_areaExpressLevelDao.AddAreaType(areaExpressLevel))
						{
							AreaExpressLevelLog areaExpressLevelLog = new AreaExpressLevelLog();
							areaExpressLevelLog.AreaID = areaExpressLevel.AreaID;
							areaExpressLevelLog.ExpressCompanyID = areaExpressLevel.ExpressCompanyID;
							areaExpressLevelLog.WarehouseId = areaExpressLevel.WareHouseID;
							areaExpressLevelLog.AreaType = areaExpressLevel.AreaType;
							areaExpressLevelLog.Enable = areaExpressLevel.Enable;
							areaExpressLevelLog.LogText = "批量新增，未审核";
							areaExpressLevelLog.CreateBy = areaExpressLevel.CreateBy;
                            areaExpressLevelLog.WareHouseType = areaExpressLevel.WareHouseType;
                            areaExpressLevelLog.MerchantID = areaExpressLevel.MerchantID;
                            areaExpressLevelLog.ProductID = areaExpressLevel.ProductID;
							areaExpressLevelLog.CreateTime = DateTime.Now;
                            areaExpressLevelLog.DistributionCode = distributionCode;
                            _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
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

		private bool JudgeDataRow(DataRow dr, DataSet ds, out StringBuilder sbError, ref AreaExpressLevel areaExpressLevel)
		{
			object obj;
			bool flag = true;
			sbError = new StringBuilder();
			#region 省市区
			if (!string.IsNullOrEmpty(dr[0].ToString()) &&
				!string.IsNullOrEmpty(dr[1].ToString()) &&
				!string.IsNullOrEmpty(dr[2].ToString()))
			{
				DataRow[] drs = ds.Tables[2].Select(string.Format("ProvinceName='{0}' and CityName='{1}' and AreaName='{2}'",
									dr[0].ToString().Trim(), dr[1].ToString().Trim(), dr[2].ToString().Trim()));
				if (drs.Length == 1)
					areaExpressLevel.AreaID = drs[0]["AreaID"].ToString();
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

			#region 配送商
			obj = dr[3];
			if (obj.ToString().Trim() != "")
			{
				DataRow[] drs = ds.Tables[0].Select(string.Format("CompanyName='{0}'", obj.ToString().Trim()));
				if (drs.Length == 1)
					areaExpressLevel.ExpressCompanyID = int.Parse(drs[0]["ExpressCompanyID"].ToString());
				else
				{
					flag = false;
					sbError.Append(" 配送商ID未能找到 /");
				}
			}
			else
			{
				flag = false;
				sbError.Append(" 配送商不能为空 /");
			}
			#endregion

			#region 仓库/分拣中心
            //obj = dr[4];
            //if (obj.ToString().Trim() != "")
            //{
            //    DataRow[] drsHouse = ds.Tables[1].Select(string.Format("WarehouseName='{0}'", obj.ToString().Trim()));
            //    if (drsHouse.Length == 1)
            //    {
            //        string houseId=drsHouse[0]["WarehouseId"].ToString();
            //        if(houseId.Contains("S_"))
            //        {
            //            areaExpressLevel.WareHouseID= houseId.Replace("S_", "");
            //            areaExpressLevel.WareHouseType = 2;
            //        }
            //        else
            //        {
            //            areaExpressLevel.WareHouseID = houseId;
            //            areaExpressLevel.WareHouseType = 1;
            //        }
            //    }
            //    else
            //    {
            //        flag = false;
            //        sbError.Append(" 仓库ID未能找到 /");
            //    }
            //}
            //else
            //{
            //    areaExpressLevel.WareHouseID = "";
            //    areaExpressLevel.WareHouseType = 0;
            //}
			#endregion

			#region 商家
			obj = dr[4];
			if (obj.ToString().Trim() != "")
			{
				DataRow[] drs = ds.Tables[4].Select(string.Format("MerchantName='{0}'", obj.ToString().Trim()));
				if (drs.Length == 1)
					areaExpressLevel.MerchantID = int.Parse(drs[0]["ID"].ToString());
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

			#region 区域类型
			obj = dr[5];
			if (obj.ToString().Trim() != "")
			{
				DataRow[] drs = ds.Tables[3].Select(string.Format("StatusNO='{0}'", obj.ToString().Trim()));
				if (drs.Length == 1)
				{
					areaExpressLevel.AreaType = int.Parse(drs[0]["StatusNO"].ToString());
					areaExpressLevel.EffectAreaType = int.Parse(drs[0]["StatusNO"].ToString());
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

			

			return flag;
		}

        //设置置回的区域类型
        public bool ReSetAreaExpressCompanyLevel(string areas, AreaExpressLevelLog areaExpressLevelLog)
        {
            if (OracleService != null)
            {
                return OracleService.ReSetAreaExpressCompanyLevel(areas, areaExpressLevelLog);
            }

            try
            {
                string[] areaId = areas.Split(',');
                DataTable dataTable = null;

                using (IUnitOfWork work = TranScopeFactory.CreateUnit())
                {
                    for (int f = 0; f < areaId.Length; f++)
                    {
                        //获取待审批的区域
                        areaExpressLevelLog.AreaID = areaId[f].ToString();
                        dataTable = _areaExpressLevelDao.SearchAreaExpressCompanyLevelDetail(areaId[f], 0, areaExpressLevelLog.DistributionCode);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            int id = int.Parse(dataTable.Rows[i]["autoid"].ToString());
                            _areaExpressLevelDao.ReSetAreaExpressCompanyLevel(id, int.Parse(areaExpressLevelLog.CreateBy), DateTime.Now);

                            areaExpressLevelLog.AreaID = dataTable.Rows[i]["areaid"].ToString();
                            areaExpressLevelLog.ExpressCompanyID = int.Parse(dataTable.Rows[i]["expresscompanyid"].ToString());
                            areaExpressLevelLog.AreaType = int.Parse(dataTable.Rows[i]["areatype"].ToString());
                            areaExpressLevelLog.LogText = "此区域类型设置已经被置回";

                            areaExpressLevelLog.WarehouseId = dataTable.Rows[i]["warehouseid"].ToString();

                            if(!string.IsNullOrEmpty(dataTable.Rows[i]["warehousetype"].ToString()))
                            {
                                areaExpressLevelLog.WareHouseType =
                                    int.Parse(dataTable.Rows[i]["warehousetype"].ToString());
                            }

                            if(!string.IsNullOrEmpty(dataTable.Rows[i]["merchantid"].ToString()))
                            {
                                areaExpressLevelLog.MerchantID = int.Parse(dataTable.Rows[i]["merchantid"].ToString());
                            }

                            if(!string.IsNullOrEmpty(dataTable.Rows[i]["productid"].ToString()))
                            {
                                areaExpressLevelLog.ProductID = int.Parse(dataTable.Rows[i]["productid"].ToString());
                            }

                            _areaExpressLevelDao.AddAreaExpLevelLog(areaExpressLevelLog);
                        }
                    }

                    work.Complete();
                    
                }

				//if (OracleService!=null)
				//{
				//    OracleService.ReSetAreaExpressCompanyLevel(areas, areaExpressLevelLog);
				//}

				return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode, ref PageInfo pi)
		{
        	if (OracleService!=null)
        	{
        		return OracleService.SearchAreaTypeList(provinceId, cityId, areaId, expressCompanyId, areaType, wareHouse,
        		                                        merchantId, auditStatus, distributionCode, ref pi);
        	}
            return _areaExpressLevelDao.SearchAreaTypeList(provinceId, cityId, areaId, expressCompanyId, areaType, wareHouse, merchantId, auditStatus, distributionCode, pi);
		}

        public DataTable SearchAreaTypeExprotList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode)
		{
			if (OracleService != null)
			{
				return OracleService.SearchAreaTypeExprotList(provinceId, cityId, areaId, expressCompanyId, areaType, wareHouse,
				                                              merchantId, auditStatus, distributionCode);
			}
            return _areaExpressLevelDao.SearchAreaTypeExprotList(provinceId, cityId, areaId, expressCompanyId, areaType, wareHouse, merchantId, auditStatus, distributionCode);
		}

        public DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string wareHouseType, string merchantId, string productId, string distributionCode)
		{
			if (OracleService != null)
			{
				return OracleService.SearchAreaTypeLog(areaId, expressCompanyId, wareHouse, wareHouseType, merchantId, productId,
				                                       distributionCode);
			}
        	return _areaExpressLevelDao.SearchAreaTypeLog(areaId, expressCompanyId, wareHouse, wareHouseType, merchantId, productId, distributionCode);
		}

        public DataTable GetAreaType(int expressComapnyId, int merchantId)
        {
            return _areaExpressLevelDao.GetAreaType(expressComapnyId, merchantId);
        }
        public DataTable SearchSecondAreaType(string areaID, string areaType, string stationID, string merchantID, string distributionCode,ref PageInfo pi)
        {
            if (OracleService != null)
            {
                return OracleService.SearchSecondAreaType(areaID, areaType, stationID, merchantID, distributionCode,ref pi);
            }
            throw new Exception("sql未实现");
        }
    }
}
