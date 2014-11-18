using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using System.Xml;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Domain.COD;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Model;
using AutoMapper;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
	public class MerchantDeliverFee : IMerchantDeliverFee
	{
        private IMerchantDeliverFeeDao _merchantDeliverFeeDao ;
        private IAccountOperatorLogDao _accountOperatorLogDao;

        public void MapAreaExpressLevelIncome(FMS_StationDeliverFee info, ref FMS_StationDeliverFee newInfo)
        {
            Mapper.CreateMap<FMS_StationDeliverFee, FMS_StationDeliverFee>();
            newInfo = Mapper.Map(info, newInfo, info.GetType(), typeof(FMS_StationDeliverFee)) as FMS_StationDeliverFee;
        }

        public bool BatchAddDeliverFee(List<FMS_StationDeliverFee> feeList, out string msg)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
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
                        sbError.Append(f.MerchantName + " " + f.GoodsCategoryCode + " " + f.ExpressCompanyName + " " + f.AreaType + "<br>");
                    }
                }
                msg = sbError.ToString();
                work.Complete();
                return true;
            }
        }

		public string AddDeliverFee(FMS_StationDeliverFee stationDeliverFee,out int id)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					_merchantDeliverFeeDao.AddStationDeliverFee(stationDeliverFee,out id);
					if (id == 0) return "新增失败";
					if (id == -1) return "已存在,操作更新";

					if (!_accountOperatorLogDao.AddOperatorLogLog(id.ToString(), int.Parse(stationDeliverFee.CreateUser.ToString()), "新增，未审核", 2))
						return "新增失败";

					work.Complete();
					return "新增成功";
				}
			}
			catch (Exception ex)
			{
				id = 0;
				return "新增失败";
			}
		}

		public string UpdateDeliverFee(FMS_StationDeliverFee stationDeliverFee)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					FMS_StationDeliverFee stationDeliverFeeTmp = GetDeliverFeeById(stationDeliverFee.ID);
					
					if (!_merchantDeliverFeeDao.UpdateDeliverFee(stationDeliverFee)) return "更新失败";

					if (!_accountOperatorLogDao.AddOperatorLogLog(stationDeliverFee.ID.ToString(), int.Parse(stationDeliverFee.CreateUser.ToString()),
							string.Format("更新(配送价格：{0}更新为{1})，未审核", stationDeliverFeeTmp.BasicDeliverFee, stationDeliverFee.BasicDeliverFee), 2))
						return "更新失败";

					work.Complete();
					return "更新成功";
				}
			}
			catch (Exception ex)
			{
				return "更新失败";
			}
		}

        public string AddWaitDeliverFee(FMS_StationDeliverFee stationDeliverFee)
        {
            int id = 0;
            string sqlKid = string.Empty;
            string sqlText = string.Empty;
            try
            {
                //生成唯一编号
                IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                stationDeliverFee.EffectKid = iDGenerate.NewId("FMS_StationDeliverFeeWait", "EffectKid");
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    _merchantDeliverFeeDao.AddWaitStationDeliverFee(stationDeliverFee, out id);
                    if (id == 0) return "新增失败";
                    if (id == -1) return "已存在,操作更新待生效";

                    sqlKid = stationDeliverFee.EffectKid;
                    sqlText = "新增待生效，未审核";
                    if (!_accountOperatorLogDao.AddOperatorLogLog(stationDeliverFee.EffectKid, int.Parse(stationDeliverFee.CreateUser.ToString()), "新增待生效，未审核", 5))
                        return "新增失败";

                    work.Complete();
                }

                return "新增成功";
            }
            catch (Exception ex)
            {
                id = 0;
                return "新增失败";
            }
        }

        public string UpdateWaitDeliverFee(FMS_StationDeliverFee stationDeliverFee)
        {
            string sqlText = string.Empty;
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    FMS_StationDeliverFee stationDeliverFeeTmp = GetWaitDeliverFeeById(stationDeliverFee.EffectKid);

                    if (!_merchantDeliverFeeDao.UpdateWaitDeliverFee(stationDeliverFee)) return "更新失败";

                    sqlText = string.Format("更新待生效(配送价格：{0}更新为{1})，未审核", stationDeliverFeeTmp.BasicDeliverFee, stationDeliverFee.BasicDeliverFee);
                    if (!_accountOperatorLogDao.AddOperatorLogLog(stationDeliverFee.EffectKid.ToString(), int.Parse(stationDeliverFee.CreateUser.ToString()),
                            string.Format("更新待生效(配送价格：{0}更新为{1})，未审核", stationDeliverFeeTmp.BasicDeliverFee, stationDeliverFee.BasicDeliverFee), 5))
                        return "更新失败";

                    work.Complete();
                }

                return "更新成功";
            }
            catch (Exception ex)
            {
                return "更新失败";
            }
        }

		public FMS_StationDeliverFee GetDeliverFeeById(int id)
		{
			DataTable dt = _merchantDeliverFeeDao.GetDeliverFeeById(id);

			if (dt == null || dt.Rows.Count <= 0)
				return null;

		    var stationDeliverFee = new FMS_StationDeliverFee
		                                {
		                                    MerchantID = int.Parse(dt.Rows[0]["MerchantID"].ToString()),
		                                    StationID = int.Parse(dt.Rows[0]["StationID"].ToString()),
		                                    StationName = dt.Rows[0]["CompanyName"].ToString(),
		                                    BasicDeliverFee = dt.Rows[0]["BasicDeliverFee"].ToString(),
		                                    Status = (EnumCODAudit) int.Parse(dt.Rows[0]["Status"].ToString()),
		                                    ExpressCompanyID =
		                                        string.IsNullOrEmpty(dt.Rows[0]["ExpressCompanyID"].ToString())
		                                            ? 0
		                                            : int.Parse(dt.Rows[0]["ExpressCompanyID"].ToString()),
		                                    IsCenterSort = int.Parse(dt.Rows[0]["IsCenterSort"].ToString()),
		                                    AreaType = int.Parse(dt.Rows[0]["AreaType"].ToString()),
		                                    GoodsCategoryCode = dt.Rows[0]["GoodsCategoryCode"].ToString(),
		                                    MerchantName = dt.Rows[0]["MerchantName"].ToString(),
		                                    IsCod = DataConvert.ToInt(dt.Rows[0]["IsCod"].ToString(), 0),
		                                    DeliverFee = dt.Rows[0]["DeliverFee"].ToString(),
		                                    IsExpress =
                                                dt.Rows[0]["IsExpress"] == DBNull.Value || string.IsNullOrEmpty(dt.Rows[0]["IsExpress"].ToString())
		                                            ? 0
		                                            : int.Parse(dt.Rows[0]["IsExpress"].ToString()),
		                                    ExpressCompanyName = dt.Rows[0]["ExpressName"].ToString()
		                                };
		    return stationDeliverFee;
		}

        public FMS_StationDeliverFee GetWaitDeliverFeeById(string id)
        {
            DataTable dt = _merchantDeliverFeeDao.GetWaitDeliverFeeById(id);

            if (dt == null || dt.Rows.Count <= 0)
                return null;

            var stationDeliverFee = new FMS_StationDeliverFee
                                        {
                                            MerchantID = int.Parse(dt.Rows[0]["MerchantID"].ToString()),
                                            StationID = int.Parse(dt.Rows[0]["StationID"].ToString()),
                                            StationName = dt.Rows[0]["CompanyName"].ToString(),
                                            BasicDeliverFee = dt.Rows[0]["BasicDeliverFee"].ToString(),
                                            Status = (EnumCODAudit) int.Parse(dt.Rows[0]["Status"].ToString()),
                                            ExpressCompanyID = string.IsNullOrEmpty(dt.Rows[0]["ExpressCompanyID"].ToString())? 0: int.Parse(dt.Rows[0]["ExpressCompanyID"].ToString()),
                                            IsCenterSort = int.Parse(dt.Rows[0]["IsCenterSort"].ToString()),
                                            AreaType = int.Parse(dt.Rows[0]["AreaType"].ToString()),
                                            EffectDate = DateTime.Parse(dt.Rows[0]["EffectDate"].ToString()),
                                            EffectKid = dt.Rows[0]["EffectKid"].ToString(),
                                            GoodsCategoryCode = dt.Rows[0]["GoodsCategoryCode"].ToString(),
                                            MerchantName = dt.Rows[0]["MerchantName"].ToString(),
                                            IsCod = DataConvert.ToInt(dt.Rows[0]["IsCod"].ToString(), 0),
                                            DeliverFee = dt.Rows[0]["DeliverFee"].ToString(),
                                            IsExpress = dt.Rows[0]["IsExpress"] == null || string.IsNullOrEmpty(dt.Rows[0]["IsExpress"].ToString())? 0 : int.Parse(dt.Rows[0]["IsExpress"].ToString()),
                                            ExpressCompanyName = dt.Rows[0]["ExpressName"].ToString()
                                        };
            return stationDeliverFee;
        }

		public bool DeleteDeliverFee(IList<KeyValuePair<string, string>> checkList,int updateBy)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> k in checkList)
					{
						if (!_merchantDeliverFeeDao.DeleteDeliverFee(int.Parse(k.Key), updateBy)) return false;
						if (!_accountOperatorLogDao.AddOperatorLogLog(k.Key, updateBy, "删除", 2)) return false;
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

		public bool UpdateDeliverFeeStatus(IList<KeyValuePair<string, string>> checkList, int auditBy, int status)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> k in checkList)
					{
						if (!_merchantDeliverFeeDao.UpdateDeliverFeeStatus(int.Parse(k.Key),status, auditBy)) return false;
						if (!_accountOperatorLogDao.AddOperatorLogLog(k.Key, auditBy, status == 1 ? "已审核" : "置回", 2)) return false;
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

        public bool UpdateWaitDeliverFeeStatus(IList<KeyValuePair<string, string>> checkList, int auditBy, int status)
        {
            try
            {
                using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
                {
                    foreach (KeyValuePair<string, string> k in checkList)
                    {
                        if (!_merchantDeliverFeeDao.UpdateWaitDeliverFeeStatus(k.Key, status, auditBy)) return false;
                        if (!_accountOperatorLogDao.AddOperatorLogLog(k.Key, auditBy, status == 1 ? "已审核" : "置回", 5)) return false;
                    }

                    work.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetWaitDeliverFeeyFeeId(int feeid)
        {
            return _merchantDeliverFeeDao.GetWaitDeliverFeeyFeeId(feeid);
        }

        public DataTable GetDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, bool isWait, string categoryCode,ref PageInfo pi)
		{
            if (isWait)
            {
                int countNum = _merchantDeliverFeeDao.GetDeliverFeeWaitStat(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode);
                if (countNum <= 0)
                    return null;

                pi.ItemCount = countNum;
                return _merchantDeliverFeeDao.GetDeliverFeeWaitList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode,pi);
            }
            else
            {
                int countNum = _merchantDeliverFeeDao.GetDeliverFeeStat(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode);
                if (countNum <= 0)
                    return null;

                pi.ItemCount = countNum;
                return _merchantDeliverFeeDao.GetDeliverFeeList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, pi);
            }
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

        public bool ImportFee(DataTable dt, int createBy, out DataTable dtError, string distributionCode)
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
			FMS_StationDeliverFee m;
			DataSet ds = _merchantDeliverFeeDao.GetExportData();
            var goodsCategoryService = ServiceLocator.GetService<IGoodsCategoryService>();
            DataTable dtCategory = goodsCategoryService.GetGoodsCategoryByMerchantID(-1, distributionCode).Copy();
            dtCategory.TableName = "dtCategory";
            ds.Tables.Add(dtCategory);

            var deliverFeeService = ServiceLocator.GetService<IDeliverFeeService>();
            var condition = new SearchCondition()
            {
                MerchantID = -1,
                DistributionCode = distributionCode,
            };
            var pi = new PageInfo(Int32.MaxValue);
            var dtMerchantBaisc = deliverFeeService.BindDeliverFeeList(condition, ref pi).Copy();
            dtMerchantBaisc.TableName = "dtMerchantBaisc";
            ds.Tables.Add(dtMerchantBaisc);

            var expressCompany = ServiceLocator.GetService<IExpressCompanyService>();
            var ecModel = expressCompany.GetCompanyModelByDistributionCode(distributionCode);
            var thirdcompanyds = expressCompany.GetThirdCompanyList(distributionCode);
            if (thirdcompanyds!=null&&thirdcompanyds.Tables.Count>0)
            {
                thirdcompanyds.Tables[0].TableName = "dtThirdCompany";
                var tempdt=thirdcompanyds.Tables[0].Copy();
                ds.Tables.Add(thirdcompanyds.Tables[0].Copy());
            }
			foreach (DataRow dr in dt.Rows)
			{
				if (!string.IsNullOrEmpty(dr[0].ToString()) ||
					!string.IsNullOrEmpty(dr[1].ToString()) ||
					!string.IsNullOrEmpty(dr[2].ToString()) ||
                    !string.IsNullOrEmpty(dr[4].ToString()) ||
                    !string.IsNullOrEmpty(dr[5].ToString()) ||
                    !string.IsNullOrEmpty(dr[6].ToString())||
                    !string.IsNullOrEmpty(dr[7].ToString())
					)
				{
					sbError = new StringBuilder();
					m = new FMS_StationDeliverFee();
				    m.IsCenterSort = 1;
                    if (JudgeDataRow(dr, ds, out sbError, ref m, ecModel ))
					{
                       
                       // m.IsCenterSort = 1;
                        m.CreateUser = createBy;
						m.Status = EnumCODAudit.A2;//默认未审核
						int id=0;
						string str = AddDeliverFee(m,out id);
                        if (id == 0)
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

		private bool JudgeDataRow(DataRow dr, DataSet ds, out StringBuilder sbError, ref FMS_StationDeliverFee m,ExpressCompany ecModel)
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
				    m.ExpressCompanyName = drs[0]["CompanyName"].ToString();
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
					if (ds.Tables[2].Select("CodeNo='" + n+"'").Length > 0)
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
            if (obj.ToString().Trim() != "")
            {
                if (isCategory == 1)
                {
                    drs = ds.Tables["dtCategory"].Select(string.Format("Name='{0}' AND MerchantID={1}", obj.ToString().Trim(), merchantId));
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

            #region 配送商
            obj = dr[7];
            if (obj.ToString().Trim() != "")
            {
                if (obj.ToString().Trim()=="全部")
                {
                    m.IsExpress = 0;
                    m.StationID = ecModel.ExpressCompanyID;
                    m.StationName = ecModel.CompanyName;
                    m.DistributionCode ="rfd";
                }
                else
                {
                    drs = ds.Tables["dtThirdCompany"].Select(string.Format("CompanyName='{0}' ", obj.ToString().Trim()));
                    if (drs.Length == 1)
                    {
                        m.IsExpress = 1;
                        m.StationID = int.Parse(drs[0]["ExpressCompanyID"].ToString());
                        m.StationName = drs[0]["CompanyName"].ToString();
                        m.DistributionCode = "rfd";
                    }
                    else
                    {
                        flag = false;
                        sbError.Append(" 没有此配送商 /");
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
            return _merchantDeliverFeeDao.GetDeliverFeeEffect();
        }

        public bool UpdateToEffect(DataRow dr)
        {
            if (dr == null) return true;
            FMS_StationDeliverFee model = new FMS_StationDeliverFee();
            model.MerchantID = int.Parse(dr["MerchantID"].ToString());
            model.StationID = int.Parse(dr["StationID"].ToString());
            model.BasicDeliverFee = dr["BasicDeliverFee"].ToString();
            model.UpdateUser = dr["UpdateBy"] == DBNull.Value || string.IsNullOrEmpty(dr["UpdateBy"].ToString()) ? 0 : int.Parse(dr["UpdateBy"].ToString());
            model.UpdateTime = dr["UpdateTime"] == DBNull.Value || string.IsNullOrEmpty(dr["UpdateTime"].ToString()) ? DateTime.Now : DateTime.Parse(dr["UpdateTime"].ToString());
            model.UpdateUserCode = dr["UpdateCode"].ToString();
            model.AuditBy =dr["AuditBy"]==DBNull.Value||string.IsNullOrEmpty(dr["AuditBy"].ToString())?0: int.Parse(dr["AuditBy"].ToString());
            model.AuditTime =dr["AuditTime"]==DBNull.Value||string.IsNullOrEmpty(dr["AuditTime"].ToString())?DateTime.Now: DateTime.Parse(dr["AuditTime"].ToString());
            model.AuditCode = dr["AuditCode"].ToString();
            model.Status = (EnumCODAudit)int.Parse(dr["Status"].ToString());
            model.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
            model.IsCenterSort = int.Parse(dr["IsCenterSort"].ToString());
            model.AreaType =dr["AreaType"]==DBNull.Value||string.IsNullOrEmpty(dr["AreaType"].ToString())?0: int.Parse(dr["AreaType"].ToString());
            model.ID = int.Parse(dr["FeeID"].ToString());
            model.EffectKid = dr["EffectKid"].ToString();
            model.EffectDate = DateTime.Parse(dr["EffectDate"].ToString());
            model.GoodsCategoryCode = dr["GoodsCategoryCode"].ToString();
            model.DeliverFee = dr["DeliverFee"].ToString();
            model.IsCod =dr["IsCod"]==DBNull.Value||string.IsNullOrEmpty(dr["IsCod"].ToString())?0: DataConvert.ToInt(dr["IsCod"].ToString(), 0);
            using (IUnitOfWork work = TranScopeFactory.CreateUnit())
            {
                if (!_merchantDeliverFeeDao.UpdateToEffect(model)) return false;

                string msg = string.Format("已生效(生效时间：{0},对应待生效编号：{1})", model.EffectDate.ToString("yyyy-MM-dd hh:mm:ss"), model.EffectKid);
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.ID.ToString(), 0,
                            msg, 5))
                    return false;
                work.Complete();
            }
            return true;
        }

        public bool DeleteWaitStationDeliverFee(string effectKid)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                if (!_merchantDeliverFeeDao.DeleteWaitStationDeliverFee(effectKid)) return false;
                if (!_accountOperatorLogDao.AddOperatorLogLog(effectKid, 0, "已生效", 5)) return false;
                work.Complete();
                //return true;
            }
            return true;
        }

	    public DataTable GetExportDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, bool isWait, string categoryCode)
	    {
            if (isWait)
            {
              
                return _merchantDeliverFeeDao.GetExportDeliverFeeWaitList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode);
            }
            else
            {
                return _merchantDeliverFeeDao.GetExportDeliverFeeList(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode);
            }
	    }

	    #endregion
	}
}
