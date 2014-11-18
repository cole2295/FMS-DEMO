using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;
using System.Xml;
using System.Data.SqlClient;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Domain.COD;
using RFD.FMS.Model;
using RFD.FMS.Service.COD;
using AutoMapper;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
	public class MerchantDeliverFee : IMerchantDeliverFee
	{
		public MerchantDeliverFee()
		{

		}

		private IMerchantDeliverFee OracleService;
		private IMerchantDeliverFeeDao _merchantDeliverFeeDao;
		private IAccountOperatorLogDao _accountOperatorLogDao;

        public void MapAreaExpressLevelIncome(FMS_StationDeliverFee info, ref FMS_StationDeliverFee newInfo)
        {
            Mapper.CreateMap<FMS_StationDeliverFee, FMS_StationDeliverFee>();
            newInfo = Mapper.Map(info, newInfo, info.GetType(), typeof(FMS_StationDeliverFee)) as FMS_StationDeliverFee;
        }

        public bool BatchAddDeliverFee(List<FMS_StationDeliverFee> feeList,out string msg)
        {
            if (OracleService != null)
            {
                return OracleService.BatchAddDeliverFee(feeList,out msg);
            }
            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                StringBuilder sbError = new StringBuilder();
                foreach (FMS_StationDeliverFee f in feeList)
                {
                    int id = 0;
                    AddDeliverFee(f, out id);
                    if (id == 0)
                    {
                        msg = "添加失败";
                        return false;
                    }
                    if (id == -1)
                    {
                        sbError.Append(f.MerchantName+" "+f.GoodsCategoryCode+" "+f.ExpressCompanyName+" "+f.AreaType+"<br>");
                    }
                }
                msg = sbError.ToString();
                work.Complete();
                return true;
            }
        }

		public string AddDeliverFee(FMS_StationDeliverFee stationDeliverFee, out int id)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					_merchantDeliverFeeDao.AddStationDeliverFee(stationDeliverFee, out id);
                    //if (id == 0) return "新增失败";
                    //if (id == -1) return "已存在";
                    if (id > 0)
                    {
                        if (!_accountOperatorLogDao.AddOperatorLogLog(id.ToString(), int.Parse(stationDeliverFee.CreateUser.ToString()), "新增，未审核", 2))
                            id = 0;
                    }
					work.Complete();
				}
				//if (OracleService != null)
				//{
				//    OracleService.AddDeliverFee(stationDeliverFee, out id);
				//}

				return "";
			}
			catch (Exception ex)
			{
				id = 0;
				return "新增失败";
			}
		}

		public string UpdateDeliverFee(FMS_StationDeliverFee stationDeliverFee)
		{
            if (OracleService != null)
            {
                return OracleService.UpdateDeliverFee(stationDeliverFee);
            }
			try
			{
				FMS_StationDeliverFee stationDeliverFeeTmp = GetDeliverFeeById(stationDeliverFee.ID);
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					if (!_merchantDeliverFeeDao.UpdateDeliverFee(stationDeliverFee)) return "更新失败";

					if (!_accountOperatorLogDao.AddOperatorLogLog(stationDeliverFee.ID.ToString(), int.Parse(stationDeliverFee.CreateUser.ToString()),
							string.Format("更新(配送价格：{0}更新为{1})，未审核", stationDeliverFeeTmp.BasicDeliverFee, stationDeliverFee.BasicDeliverFee), 2))
						return "更新失败";

					work.Complete();
				}
				//if (OracleService != null)
				//{
				//    OracleService.UpdateDeliverFee(stationDeliverFee);
				//}

				return "更新成功";
			}
			catch (Exception ex)
			{
				return "更新失败";
			}
		}

        public string AddWaitDeliverFee(FMS_StationDeliverFee stationDeliverFee)
        {
            if (OracleService != null)
            {
                return OracleService.AddWaitDeliverFee(stationDeliverFee);
            }

            throw new Exception("sql中没有实现待生效");
        }

        public string UpdateWaitDeliverFee(FMS_StationDeliverFee stationDeliverFee)
        {
            if (OracleService != null)
            {
                return OracleService.UpdateWaitDeliverFee(stationDeliverFee);
            }

            throw new Exception("sql中没有实现待生效");
        }

		public FMS_StationDeliverFee GetDeliverFeeById(int id)
		{
			if (OracleService!=null)
			{
				return OracleService.GetDeliverFeeById(id);
			}
			DataTable dt = _merchantDeliverFeeDao.GetDeliverFeeById(id);

			if (dt == null || dt.Rows.Count <= 0)
				return null;

			FMS_StationDeliverFee stationDeliverFee = new FMS_StationDeliverFee();
			stationDeliverFee.MerchantID = int.Parse(dt.Rows[0]["MerchantID"].ToString());
			stationDeliverFee.StationID = int.Parse(dt.Rows[0]["StationID"].ToString());
			stationDeliverFee.StationName = dt.Rows[0]["CompanyName"].ToString();
			stationDeliverFee.BasicDeliverFee = dt.Rows[0]["BasicDeliverFee"].ToString();
			stationDeliverFee.Status = (EnumCODAudit)int.Parse(dt.Rows[0]["Status"].ToString());
			stationDeliverFee.ExpressCompanyID = string.IsNullOrEmpty(dt.Rows[0]["ExpressCompanyID"].ToString()) ? 0 : int.Parse(dt.Rows[0]["ExpressCompanyID"].ToString());
			stationDeliverFee.IsCenterSort = int.Parse(dt.Rows[0]["IsCenterSort"].ToString());
			stationDeliverFee.AreaType = int.Parse(dt.Rows[0]["AreaType"].ToString());
            stationDeliverFee.GoodsCategoryCode = dt.Rows[0]["GoodsCategoryCode"].ToString();
            stationDeliverFee.MerchantName = dt.Rows[0]["MerchantName"].ToString();
            stationDeliverFee.IsCod = DataConvert.ToInt(dt.Rows[0]["IsCod"].ToString(),0);
            stationDeliverFee.DeliverFee = dt.Rows[0]["DeliverFee"].ToString();
			return stationDeliverFee;
		}

        public FMS_StationDeliverFee GetWaitDeliverFeeById(string id)
        {
            if (OracleService != null)
            {
                return OracleService.GetWaitDeliverFeeById(id);
            }
            DataTable dt = _merchantDeliverFeeDao.GetWaitDeliverFeeById(id);

            if (dt == null || dt.Rows.Count <= 0)
                return null;

            FMS_StationDeliverFee stationDeliverFee = new FMS_StationDeliverFee();
            stationDeliverFee.MerchantID = int.Parse(dt.Rows[0]["MerchantID"].ToString());
            stationDeliverFee.StationID = int.Parse(dt.Rows[0]["StationID"].ToString());
            stationDeliverFee.StationName = dt.Rows[0]["CompanyName"].ToString();
            stationDeliverFee.BasicDeliverFee = dt.Rows[0]["BasicDeliverFee"].ToString();
            stationDeliverFee.Status = (EnumCODAudit)int.Parse(dt.Rows[0]["Status"].ToString());
            stationDeliverFee.ExpressCompanyID = string.IsNullOrEmpty(dt.Rows[0]["ExpressCompanyID"].ToString()) ? 0 : int.Parse(dt.Rows[0]["ExpressCompanyID"].ToString());
            stationDeliverFee.IsCenterSort = int.Parse(dt.Rows[0]["IsCenterSort"].ToString());
            stationDeliverFee.AreaType = int.Parse(dt.Rows[0]["AreaType"].ToString());
            stationDeliverFee.EffectDate = DateTime.Parse(dt.Rows[0]["EffectDate"].ToString());
            stationDeliverFee.EffectKid = dt.Rows[0]["EffectKid"].ToString();
            stationDeliverFee.GoodsCategoryCode = dt.Rows[0]["GoodsCategoryCode"].ToString();
            stationDeliverFee.MerchantName = dt.Rows[0]["MerchantName"].ToString();
            stationDeliverFee.IsCod = DataConvert.ToInt(dt.Rows[0]["IsCod"].ToString(), 0);
            stationDeliverFee.DeliverFee = dt.Rows[0]["DeliverFee"].ToString();
            return stationDeliverFee;
        }

		public bool DeleteDeliverFee(IList<KeyValuePair<string, string>> checkList, int updateBy)
		{
            if (OracleService != null)
            {
                return OracleService.DeleteDeliverFee(checkList, updateBy);
            }
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					foreach (KeyValuePair<string, string> k in checkList)
					{
						if (!_merchantDeliverFeeDao.DeleteDeliverFee(int.Parse(k.Key), updateBy)) return false;
						if (!_accountOperatorLogDao.AddOperatorLogLog(k.Key, updateBy, "删除", 2)) return false;
					}
					work.Complete();
				}
				//if (OracleService != null)
				//{
				//    OracleService.DeleteDeliverFee(checkList, updateBy);
				//}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool UpdateDeliverFeeStatus(IList<KeyValuePair<string, string>> checkList, int auditBy, int status)
		{
            if (OracleService != null)
            {
                return OracleService.UpdateDeliverFeeStatus(checkList, auditBy, status);
            }
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateUnit())
				{
					foreach (KeyValuePair<string, string> k in checkList)
					{
						if (!_merchantDeliverFeeDao.UpdateDeliverFeeStatus(int.Parse(k.Key), status, auditBy)) return false;
						if (!_accountOperatorLogDao.AddOperatorLogLog(k.Key, auditBy, status == 1 ? "已审核" : "置回", 2)) return false;
					}
					work.Complete();
				}
				//if (OracleService!=null)
				//{
				//    OracleService.UpdateDeliverFeeStatus(checkList, auditBy, status);
				//}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

        public bool UpdateWaitDeliverFeeStatus(IList<KeyValuePair<string, string>> checkList, int auditBy, int status)
        {
            if (OracleService != null)
            {
                return OracleService.UpdateWaitDeliverFeeStatus(checkList, auditBy, status);
            }

            throw new Exception("sql中没有实现待生效");
        }

        public int GetWaitDeliverFeeyFeeId(int feeid)
        {
            if(OracleService!=null)
            {
                return OracleService.GetWaitDeliverFeeyFeeId(feeid);
            }
            return _merchantDeliverFeeDao.GetWaitDeliverFeeyFeeId(feeid);
        }

        public DataTable GetDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, bool isWait, string categoryCode,ref PageInfo pi)
		{
			if (OracleService!=null)
			{
				return OracleService.GetDeliverFeeList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus,
                                                       distributionCode, isWait, categoryCode,ref pi);
			}
            //待生效未实现SQL
            int countNum = _merchantDeliverFeeDao.GetDeliverFeeStat(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode);
            if (countNum <= 0)
                return null;

            pi.ItemCount = countNum;
            return _merchantDeliverFeeDao.GetDeliverFeeList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, pi);
		}

		public bool ImportFee(DataTable dt, int createBy, out DataTable dtError, string distributionCode)
		{
            if (OracleService != null)
            {
                return OracleService.ImportFee(dt, createBy, out  dtError, distributionCode);
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
			FMS_StationDeliverFee m;
			DataSet ds = _merchantDeliverFeeDao.GetExportData();
            IGoodsCategoryService goodsCategoryService = ServiceLocator.GetService<IGoodsCategoryService>();
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

			IExpressCompanyService expressCompany = ServiceLocator.GetService<IExpressCompanyService>();
			ExpressCompany ecModel = expressCompany.GetCompanyModelByDistributionCode(distributionCode);

			foreach (DataRow dr in dt.Rows)
			{
				if (!string.IsNullOrEmpty(dr[0].ToString()) ||
					!string.IsNullOrEmpty(dr[1].ToString()) ||
					!string.IsNullOrEmpty(dr[2].ToString()) ||
                    !string.IsNullOrEmpty(dr[4].ToString()) ||
                    !string.IsNullOrEmpty(dr[5].ToString()) ||
                    !string.IsNullOrEmpty(dr[6].ToString())
					)
				{
					sbError = new StringBuilder();
					m = new FMS_StationDeliverFee();
					m.StationID = ecModel.ExpressCompanyID;
					m.IsCenterSort = 1;
					m.CreateUser = createBy;
					m.DistributionCode = distributionCode;
					if (JudgeDataRow(dr, ds, out sbError, ref m))
					{
						m.Status = EnumCODAudit.A2;//默认未审核
						int id = 0;
						string str = AddDeliverFee(m, out id);
						if (id==0)
						{
							r = dtError.NewRow();
							r.ItemArray = dr.ItemArray;
							r["错误描述"] = "新增失败";
							dtError.Rows.Add(r);
						}
                        if (id == -1)
                        {
                            r = dtError.NewRow();
                            r.ItemArray = dr.ItemArray;
                            r["错误描述"] = "已存在";
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

		private bool JudgeDataRow(DataRow dr, DataSet ds, out StringBuilder sbError, ref FMS_StationDeliverFee m)
		{
			sbError = new StringBuilder();
			object obj;
			DataRow[] drs;
			bool flag = true;
			#region 商家
			obj = dr[0];
			if (obj.ToString().Trim() != "")
			{
				drs = ds.Tables[3].Select(string.Format("MerchantName='{0}'", obj.ToString().Trim()));
				if (drs.Length == 1)
					m.MerchantID = drs[0]["ID"].ToString().TryGetInt();
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
			obj = dr[1];
			if (obj.ToString().Trim() != "")
			{
				drs = ds.Tables[1].Select(string.Format("CompanyName='{0}'", obj.ToString().Trim()));
				if (drs.Length == 1 && m.IsCenterSort == 1)
				{
					m.ExpressCompanyID = drs[0]["ExpressCompanyID"].ToString().TryGetInt();
				}
				else if (drs.Length <= 0 && m.IsCenterSort == 1)
				{
					flag = false;
					sbError.Append(" 分拣中心未能找到 /");
				}
			}
			else
			{
				if (m.IsCenterSort == 1)
				{
					flag = false;
					sbError.Append(" 分拣中心不能为空 /");
				}
				else
				{
					m.ExpressCompanyID = 0;
				}
			}
			#endregion

			#region 区域类型
			obj = dr[2];
			if (obj.ToString().Trim() != "")
			{
				int n;
				if (isNumberic(obj.ToString().Trim(), out n))
				{
					if (ds.Tables[2].Select("CodeNo='" + n + "'").Length > 0)
					{
						m.AreaType = n;
					}
					else
					{
						flag = false;
						sbError.Append(" 区域类型不在范围内 /");
					}
				}
				else
				{
					flag = false;
					sbError.Append(" 区域类型只接受数字 /");
				}
			}
			else
			{
				flag = false;
				sbError.Append(" 区域类型不能为空 /");
			}
			#endregion

            #region 商家基础信息设置验证
            int merchantId = m.MerchantID;
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
            obj = dr[3];
            if (obj.ToString().Trim() != "" && rowList.Count > 0)
            {
                if (isCategory == 1)
                {
                    drs = ds.Tables["dtCategory"].Select(string.Format("Name='{0}' AND MerchantID={1}", obj.ToString().Trim(), m.MerchantID));
                    if (drs.Length == 1)
                    {
                        m.GoodsCategoryCode = drs[0]["Code"].ToString();
                    }
                    else
                    {
                        flag = false;
                        sbError.Append(" 商家下没有此货物品类 /");
                    }
                }
                else
                {
                    flag = false;
                    sbError.Append(" 此商家不按货物品类设置 /");
                }
            }
            #endregion

            #region 是否区分COD
            obj = dr[4];
            string isCod = obj.ToString().Trim();
            if (isCod != "")
            {
                if (isCod != "" && (isCod == "是" || isCod == "否"))
                {
                    m.IsCod = isCod == "是" ? 1 : 0;
                    if (dr[5].ToString() == dr[6].ToString() && m.IsCod == 1)
                    {
                        flag = false;
                        sbError.Append(" 区分COD时，两公式不能相等 /");
                    }
                    if (dr[5].ToString() != dr[6].ToString() && m.IsCod == 0)
                    {
                        flag = false;
                        sbError.Append(" 不区分COD时，两公式必须相等 /");
                    }
                }
                else
                {
                    flag = false;
                    sbError.Append(" 是否区分COD错误，只能“是”或“否” /");
                }
            }
            else
            {
                flag = false;
                sbError.Append(" 是否区分COD不能为空 /");
            }
            #endregion

            #region COD价格公式
            obj = dr[5];
            if (obj.ToString().Trim() != "")
            {
                m.BasicDeliverFee = obj.ToString().Trim().Replace("（", "(").Replace("）", ")");
            }
            else
            {
                flag = false;
                sbError.Append(" COD价格或公式不能为空 /");
            }
            #endregion

            #region 非COD价格公式
            obj = dr[6];
            if (obj.ToString().Trim() != "")
            {
                m.DeliverFee = obj.ToString().Trim().Replace("（", "(").Replace("）", ")");
            }
            else
            {
                flag = false;
                sbError.Append(" 非COD价格或公式不能为空 /");
            }
            #endregion
			return flag;
		}

		public static bool isNumberic(string str, out int n)
		{
			n = 0;
			try
			{
				n = Convert.ToInt32(str);
				return true;
			}
			catch
			{
				return false;
			}
        }

        #region 生效服务
        public DataTable GetWaitFeeList()
        {
            if (OracleService != null)
            {
                return OracleService.GetWaitFeeList();
            }
            return _merchantDeliverFeeDao.GetDeliverFeeEffect();
        }

        public bool UpdateToEffect(DataRow dr)
        {
            if (dr == null) return true;
            FMS_StationDeliverFee model = new FMS_StationDeliverFee();
            model.MerchantID=int.Parse(dr["MerchantID"].ToString());
            model.StationID = int.Parse(dr["StationID"].ToString());
            model.BasicDeliverFee = dr["BasicDeliverFee"].ToString();
            model.UpdateUser = dr["UpdateBy"] == DBNull.Value ? 0 : int.Parse(dr["UpdateBy"].ToString());
            model.UpdateTime = dr["UpdateTime"] == DBNull.Value ? DateTime.Now : DateTime.Parse(dr["UpdateTime"].ToString());
            model.UpdateUserCode =dr["UpdateCode"].ToString();
            model.AuditBy = dr["AuditBy"]== DBNull.Value ?0:int.Parse(dr["AuditBy"].ToString());
            model.AuditTime = dr["AuditTime"] == DBNull.Value ? DateTime.Now : DateTime.Parse(dr["AuditTime"].ToString());
            model.AuditCode = dr["AuditCode"].ToString();
            model.Status = (EnumCODAudit)int.Parse(dr["Status"].ToString());
            model.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
            model.IsCenterSort = int.Parse(dr["IsCenterSort"].ToString());
            model.AreaType = int.Parse(dr["AreaType"].ToString());
            model.ID = int.Parse(dr["FeeID"].ToString());
            model.EffectKid = dr["EffectKid"].ToString();
            model.EffectDate = DateTime.Parse(dr["EffectDate"].ToString());
            model.GoodsCategoryCode = dr["GoodsCategoryCode"].ToString();
            model.DeliverFee = dr["DeliverFee"].ToString();
            model.IsCod = DataConvert.ToInt(dr["IsCod"].ToString(),0);
            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                if(!_merchantDeliverFeeDao.UpdateToEffect(model)) return false;

                if (!_accountOperatorLogDao.AddOperatorLogLog(model.ID.ToString(), 0,
                            string.Format("已生效(生效时间：{0},对应待生效编号：{1})", model.EffectDate, model.EffectKid), 2))
                    return false;
                work.Complete();
            }
            return true;
        }

        public bool DeleteWaitStationDeliverFee(string effectKid)
        {
            if (OracleService != null)
            {
                return OracleService.DeleteWaitStationDeliverFee(effectKid);
            }
            throw new Exception("sql 没有实现");
        }

	    public DataTable GetExportDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, bool isWait, string categoryCode)
	    {
	        if (OracleService!=null)
	        {
	            return OracleService.GetExportDeliverFeeList(merchantId, expressCompanyId, areaType, sortCenterId,
	                                                         auditStatus, distributionCode, isWait, categoryCode);
	        }
            throw  new Exception("sql 没有实现");
	    }

	    #endregion
    }
}
