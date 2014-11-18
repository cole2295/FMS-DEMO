using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL;
using RFD.FMS.Util;
using RFD.FMS.AdoNet.UnitOfWork;
using System.Xml;
using RFD.FMS.MODEL.Enumeration;
using System.Data.SqlClient;
using System.Collections;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
	/// <summary>
	/// COD价格维护类
	/// </summary>
	public class DeliveryPriceService : IDeliveryPriceService
	{
        private IDeliveryPriceDao _deliveryPriceDao;

		/// <summary>
		/// 获取商家
		/// </summary>
		/// <returns></returns>
        public DataTable GetMerchant(string disCode)
		{
            return _deliveryPriceDao.GetMerchant(disCode);
		}

		/// <summary>
		/// 获取COD线路列表
		/// </summary>
		/// <param name="expressCompanyId">配送商</param>
		/// <param name="lineStatus">线路可用状态</param>
		/// <param name="auditStatus">审核状态</param>
		/// <param name="areaType">区域类型</param>
		/// <param name="wareHouse">仓库</param>
		/// <param name="waitEffect">是否待生效</param>
		/// <returns></returns>
        public DataTable GetDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId,string distributionCode,int IsCod, ref PageInfo pi, bool isPage)
		{
            if (waitEffect)
            {
                return _deliveryPriceDao.GetEffectDeliveryPriceList(expressCompanyId, lineStatus, auditStatus, areaType, wareHouse, wareHouseType, waitEffect, merchantId, distributionCode, IsCod, pi, isPage);
            }
            else
            {
                return _deliveryPriceDao.GetDeliveryPriceList(expressCompanyId, lineStatus, auditStatus, areaType, wareHouse, wareHouseType, waitEffect, merchantId, distributionCode, IsCod, pi, isPage);
            }
		}

		/// <summary>
		/// 根据codLineNo查询线路
		/// </summary>
		/// <param name="codLineNo"></param>
		/// <returns></returns>
		public FMS_CODLine GetListByCodLineNo(string codLineNo)
		{
            DataTable dt = _deliveryPriceDao.GetListByCodLineNo(codLineNo);

			return TransformToCodLineModel(dt).Count > 0 ? TransformToCodLineModel(dt)[0] : null;
		}

		/// <summary>
		/// 增加线路
		/// </summary>
		/// <param name="codLine"></param>
		/// <returns></returns>
		public int AddDeliveryPrice(FMS_CODLine codLine)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					string cODLineNO = new NoGenerate().GetOrderNoWithSeed(6, 8, "P");
					//string cODLineNO = codLine.CODLineNO;
					int n= _deliveryPriceDao.AddDeliveryPrice(codLine, cODLineNO);

                    if (n == 1)
                    {
                        if (!AddDeliveryPriceLog(cODLineNO, codLine.CreateBy, "新增，未审核")) return -1;
                    }

					work.Complete();

					return n;
				}
			}
			catch (Exception ex)
			{
				return -1;
			}
		}

		/// <summary>
		/// 更新线路
		/// </summary>
		/// <param name="codLine"></param>
		/// <returns></returns>
		public bool UpdateDeliveryPrice(FMS_CODLine codLine)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					FMS_CODLine codLineTmp = GetListByCodLineNo(codLine.CODLineNO);
					if (_deliveryPriceDao.UpdateDeliveryPrice(codLine))
					{
						StringBuilder sbLog = new StringBuilder();
						if (codLine.PriceFormula != codLineTmp.PriceFormula)
							sbLog.Append("公式：" + codLineTmp.PriceFormula + " 改为 " + codLine.PriceFormula);
						if(codLine.LineStatus != codLineTmp.LineStatus)
							sbLog.Append("线路状态：" + codLineTmp.LineStatus + " 改为 " + codLine.LineStatus);
						if (sbLog.ToString().Length <= 0)
							sbLog.Append("未执行改变");
						if (!AddDeliveryPriceLog(codLine.CODLineNO, codLine.UpdateBy, "更新，未审核，将" + sbLog.ToString())) return false;
					}
					else
						return false;
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
		/// 删除线路、可批量
		/// </summary>
		/// <param name="keyValuePairs"></param>
		/// <returns></returns>
		public bool DeleteDeliveryPrice(IList<KeyValuePair<string, string>> keyValuePairs,string deleteBy)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
					{
						if (!_deliveryPriceDao.DeleteDeliveryPrice(keyValuePair.Key, deleteBy))
							return false;
						else
							if (!AddDeliveryPriceLog(keyValuePair.Key, deleteBy, "删除")) return false;
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
		/// 删除线路、可批量
		/// </summary>
		/// <param name="keyValuePairs"></param>
		/// <returns></returns>
		public bool UpdateDeliveryPriceAuditStatus(IList<KeyValuePair<string, string>> keyValuePairs, string auditBy, int auditStatus)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
					{
						FMS_CODLine codLineTmp = GetListByCodLineNo(keyValuePair.Key);
						if (!_deliveryPriceDao.UpdateDeliveryPriceAuditStatus(keyValuePair.Key, auditBy, auditStatus))
							return false;
						else
						{
							StringBuilder sbLog = new StringBuilder();
							if (codLineTmp.AuditStatus != auditStatus)
								sbLog.Append("审核状态：" + codLineTmp.AuditStatus + " 改为 " + auditStatus);
							if (!AddDeliveryPriceLog(keyValuePair.Key, auditBy, "审核，" + sbLog.ToString())) return false;
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
		/// 写日志
		/// </summary>
		/// <param name="pkNo"></param>
		/// <param name="createBy"></param>
		/// <param name="logText"></param>
		/// <returns></returns>
		private bool AddDeliveryPriceLog(string pkNo, string createBy, string logText)
		{
			return _deliveryPriceDao.AddDeliveryPriceLog(pkNo, createBy, logText, 1);
		}

		/// <summary>
		/// 历史价格列表获取
		/// </summary>
		/// <param name="lineNo"></param>
		/// <returns></returns>
		public DataTable GetDeliveryPriceHistoryList(string lineNo)
		{
            string lineNoStr=string.Empty;
			if (!string.IsNullOrEmpty(lineNo))
				lineNoStr = CreateSearchStr(lineNo.Split(','));

            if (!string.IsNullOrEmpty(lineNoStr))
            {
                return DisposeDeliveryPriceHistoryList(_deliveryPriceDao.GetDeliveryPriceHistoryList(lineNoStr));
            }
            else
            {
                return DisposeDeliveryPriceHistoryList(_deliveryPriceDao.GetDeliveryPriceHistoryList(""));
            }
		}

        private DataTable DisposeDeliveryPriceHistoryList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            ArrayList al = new ArrayList();
            foreach (DataColumn dc in dt.Columns)
            {
                string name = dc.ColumnName;
                if (name != "线路编号")
                    name = GetYearMonth(name);

                al.Add(name);
            }
            al.Sort(1, al.Count - 1, null);

            DataTable dtNew = new DataTable();
            DataColumn dcNew;
            for (int n = 0; n < al.Count; n++)
            {
                dcNew = dtNew.Columns.Add(al[n].ToString(), typeof(string));
            }

            foreach (DataRow dr in dt.Rows)
            {
                DataRow drNew = dtNew.NewRow();
                foreach (DataColumn dcOld in dt.Columns)
                {
                    string name = dcOld.ColumnName;
                    if (name != "线路编号")
                        name = GetYearMonth(name);
                    drNew[name] = dr[dcOld.ColumnName].ToString();
                }
                dtNew.Rows.Add(drNew);
            }

            return dtNew;
        }

        private string GetYearMonth(string yearMonth)
        {
            bool flag = false;
            if (yearMonth.Contains("非"))
            {
                yearMonth = yearMonth.Replace("非", "");
                flag = true;
            }
            string year = yearMonth.Substring(0, 4);
            string month = yearMonth.Substring(4, 2);
            string dateTimeStr = year + "." + month + ".01";
            DateTime dt = DateTime.Parse(dateTimeStr);
            if (flag)
                return dt.AddMonths(-1).ToString("yyyyMM") + "非";
            else
                return dt.AddMonths(-1).ToString("yyyyMM");
        }

        private string CreateSearchStr(string[] ids)
		{
            StringBuilder sbStr = new StringBuilder();
			for (int i = 0; i < ids.Length; i++)
			{
				sbStr.Append("'"+ids[i].ToString()+"',");
			}
			return sbStr.ToString().TrimEnd(',');
		}

		/// <summary>
		/// 查询操作日志
		/// </summary>
		/// <param name="lineNo"></param>
		/// <param name="dateStr"></param>
		/// <param name="dateEnd"></param>
		/// <returns></returns>
        public DataTable GetDeliveryPriceLog(string lineNo, string dateStr, string dateEnd, string distributionCode)
		{
			return _deliveryPriceDao.GetDeliveryPriceLog(lineNo, dateStr, dateEnd, distributionCode);
		}

		public int AddEffectDeliveryPrice(FMS_CODLine codLine)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					int n = _deliveryPriceDao.AddEffectDeliveryPrice(codLine);
					if (n == 1)
						if (!AddDeliveryPriceLog(codLine.CODLineNO, codLine.CreateBy, "新增待生效，未审核")) return -1;
					work.Complete();
					return n;
				}
			}
			catch (Exception ex)
			{
				return -1;
			}
		}

		/// <summary>
		/// 根据codLineNo查询线路
		/// </summary>
		/// <param name="codLineNo"></param>
		/// <returns></returns>
		public FMS_CODLine GetListByEffectCodLineNo(string codLineNo)
		{
            DataTable dt = _deliveryPriceDao.GetListByEffectCodLineNo(codLineNo);
			if (dt == null || dt.Rows.Count <= 0)
				return null;

			return TransformToCodLineModel(dt).Count > 0 ? TransformToCodLineModel(dt)[0] : null;
		}

		private List<FMS_CODLine> TransformToCodLineModel(DataTable dt)
		{
			if (dt == null || dt.Rows.Count <= 0)
				return null;

			List<FMS_CODLine> codLineList = new List<FMS_CODLine>();
			foreach (DataRow dr in dt.Rows)
			{
				FMS_CODLine codLine = new FMS_CODLine();
				codLine.LineID = int.Parse(dr["LineID"].ToString());
				codLine.CODLineNO = dr["CODLineNO"].ToString();
				codLine.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
				codLine.CompanyName = dr["CompanyName"].ToString();
				codLine.IsEchelon = int.Parse(dr["IsEchelon"].ToString());
				codLine.WareHouseID = dr["WareHouseID"].ToString();
				codLine.AreaType = int.Parse(dr["AreaType"].ToString());
				codLine.PriceFormula = dr["PriceFormula"].ToString();
				codLine.LineStatus = int.Parse(dr["LineStatus"].ToString());
				codLine.AuditStatus = int.Parse(dr["AuditStatus"].ToString());
				codLine.EffectDate = dt.Columns.Contains("EffectDate") ? DateTime.Parse(dr["EffectDate"].ToString()) : DateTime.MinValue;
				codLine.MerchantID = int.Parse(dr["MerchantID"].ToString());
				codLine.ProductID = int.Parse(dr["ProductID"].ToString());
                codLine.DistributionCode = dr["DistributionCode"].ToString();
                codLine.IsCOD = int.Parse(dr["IsCOD"].ToString());
                codLine.Formula = dr["Formula"].ToString();
                codLine.MerchantName = dr["MerchantName"].ToString();
				codLineList.Add(codLine);
			}
			return codLineList;
		}

		/// <summary>
		/// 更新待生效
		/// </summary>
		/// <param name="codLine"></param>
		/// <returns></returns>
		public bool UpdateEffectCodLine(FMS_CODLine codLine)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					FMS_CODLine codLineTmp = GetListByEffectCodLineNo(codLine.CODLineNO);
					if (_deliveryPriceDao.UpdateEffectCodLine(codLine))
					{
						StringBuilder sbLog = new StringBuilder();
						if (codLine.PriceFormula != codLineTmp.PriceFormula)
							sbLog.Append("公式：" + codLineTmp.PriceFormula + " 改为 " + codLine.PriceFormula);
						if (codLine.LineStatus != codLineTmp.LineStatus)
							sbLog.Append("线路状态：" + codLineTmp.LineStatus + " 改为 " + codLine.LineStatus);
						if (codLine.EffectDate != codLineTmp.EffectDate)
							sbLog.Append("待生效时间：" + codLineTmp.EffectDate + " 改为 " + codLine.EffectDate);
						if (sbLog.ToString().Length <= 0)
							sbLog.Append("未执行改变");
						if (!AddDeliveryPriceLog(codLine.CODLineNO, codLine.UpdateBy, "更新待生效，未审核，将" + sbLog.ToString())) return false;
					}
					else
						return false;
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
		/// 审核状态
		/// </summary>
		/// <param name="keyValuePairs"></param>
		/// <returns></returns>
		public bool UpdateEffectDeliveryPriceAuditStatus(IList<KeyValuePair<string, string>> keyValuePairs, string auditBy, int auditStatus)
		{
			try
			{
				using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
				{
					foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
					{
						FMS_CODLine codLineTmp = GetListByEffectCodLineNo(keyValuePair.Key);
						if (!_deliveryPriceDao.UpdateEffectDeliveryPriceAuditStatus(keyValuePair.Key, auditBy, auditStatus))
							return false;
						else
						{
							StringBuilder sbLog = new StringBuilder();
							if (codLineTmp.AuditStatus != auditStatus)
								sbLog.Append("审核状态：" + codLineTmp.AuditStatus + " 改为 " + auditStatus);
							if (!AddDeliveryPriceLog(keyValuePair.Key, auditBy, "审核待生效，" + sbLog.ToString())) return false;
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
		/// 批量导入配送价格
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="createBy"></param>
		/// <param name="dtError"></param>
		/// <returns></returns>
        public bool ExportDeliveryPrice(DataTable dt, int createBy, out DataTable dtError, string distributionCode)
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
			FMS_CODLine codLine;
			DataSet ds = _deliveryPriceDao.GetExportData();
			foreach (DataRow dr in dt.Rows)
			{
				if (!string.IsNullOrEmpty(dr[0].ToString()) ||
					!string.IsNullOrEmpty(dr[1].ToString()) ||
					!string.IsNullOrEmpty(dr[2].ToString()) ||
					!string.IsNullOrEmpty(dr[3].ToString()) ||
                    !string.IsNullOrEmpty(dr[4].ToString()) ||
                    !string.IsNullOrEmpty(dr[5].ToString()) ||
					!string.IsNullOrEmpty(dr[6].ToString())
					)
				{
					sbError = new StringBuilder();
					codLine = new FMS_CODLine();
					if (JudgeDataRow(dr, ds, out sbError, ref codLine))
					{
						codLine.CreateBy = createBy.ToString();
						codLine.AuditStatus = (int)EnumCODAudit.A2;//默认未审核
                        codLine.DistributionCode = distributionCode;
                        codLine.ProductID = 1;
                        codLine.IsEchelon = 2;
                        codLine.WareHouseType = 0;
                        codLine.WareHouseID = "";
						int n = AddDeliveryPrice(codLine);
						if (n==0)
						{
							r = dtError.NewRow();
							r.ItemArray = dr.ItemArray;
							r["错误描述"] = "已存在";
							dtError.Rows.Add(r);
						}
						if(n<0)
						{
							r = dtError.NewRow();
							r.ItemArray = dr.ItemArray;
							r["错误描述"] = "插入失败";
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

		/// <summary>
		/// 验证导入字段
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="sbError"></param>
		/// <returns></returns>
		private bool JudgeDataRow(DataRow dr, DataSet ds, out StringBuilder sbError, ref FMS_CODLine codLine)
		{
			sbError = new StringBuilder();
			object obj;
			DataRow[] drs;
			bool flag = true;
			#region 配送商
			obj = dr[0];
			if (obj.ToString().Trim() != "")
			{
				drs = ds.Tables[0].Select(string.Format("CompanyName='{0}'", obj.ToString().Trim()));
				if (drs.Length == 1)
				{
					codLine.ExpressCompanyID = int.Parse(drs[0]["ExpressCompanyID"].ToString());
				}
				else
				{
					flag = false;
					sbError.Append(" 配送商未能找到 /");
				}
			}
			else
			{
				flag = false;
				sbError.Append(" 配送商不能为空 /");
			}
			#endregion

            #region 是否按发货地梯次收费
            //obj = dr[1];
            //string isEchelon = obj.ToString().Trim();
            //if (isEchelon != "")
            //{
            //    if (isEchelon != "" && (isEchelon == "是" || isEchelon == "否"))
            //    {
            //        codLine.IsEchelon = isEchelon == "是" ? 1 : 2;
            //    }
            //    else
            //    {
            //        flag = false;
            //        sbError.Append(" 是否按发货地梯次收费输入错误，只能“是”或“否” /");
            //    }
            //}
            //else
            //{
            //    flag = false;
            //    sbError.Append(" 是否按发货地梯次收费不能为空 /");
            //}
            //#endregion

            //#region 仓库
            //obj = dr[2];
            //if (obj.ToString().Trim() != "")
            //{
            //    drs = ds.Tables[1].Select(string.Format("WarehouseName='{0}'", obj.ToString().Trim()));
            //    if (drs.Length == 1 && codLine.IsEchelon == 1)
            //    {
            //        string houseId = drs[0]["WarehouseId"].ToString();
            //        //codLine.WareHouseID = drs[0]["WarehouseId"].ToString();
            //        if (houseId.Contains("S_"))
            //        {
            //            codLine.WareHouseID = houseId.Replace("S_", "");
            //            codLine.WareHouseType = 2;
            //        }
            //        else
            //        {
            //            codLine.WareHouseID = houseId;
            //            codLine.WareHouseType = 1;
            //        }
            //    }
            //    else if (drs.Length == 1 && codLine.IsEchelon == 2)
            //    {
            //        flag = false;
            //        sbError.Append(" 是否按发货地梯次收费为否时不能存在仓库 /");
            //    }
            //    else if (drs.Length <= 0 && codLine.IsEchelon == 1)
            //    {
            //        flag = false;
            //        sbError.Append(" 仓库未能找到 /");
            //    }
            //}
            //else
            //{
            //    if (codLine.IsEchelon == 1)
            //    {
            //        flag = false;
            //        sbError.Append(" 仓库不能为空 /");
            //    }
            //    else
            //    {
            //        codLine.WareHouseID = "";
            //        codLine.WareHouseType = 0;
            //    }
            //}
            #endregion

			#region 商家
			obj = dr[1];
			if (obj.ToString().Trim() != "")
			{
				drs = ds.Tables[3].Select(string.Format("MerchantName='{0}'", obj.ToString().Trim()));
				if (drs.Length == 1)
					codLine.MerchantID = int.Parse(drs[0]["ID"].ToString());
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
			obj = dr[2];
			if (obj.ToString().Trim() != "")
			{
				int n;
				if (isNumberic(obj.ToString().Trim(), out n))
				{
					if (ds.Tables[2].Select("CodeNo='" + n+"'").Length > 0)
					{
						codLine.AreaType = n;
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

            #region 是否区分COD
            obj = dr[3];
            string isCod = obj.ToString().Trim();
            if (isCod != "")
            {
                if (isCod != "" && (isCod == "是" || isCod == "否"))
                {
                    codLine.IsCOD = isCod == "是" ? 1 : 0;
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
            obj = dr[4];
			if (obj.ToString().Trim() != "")
			{
				codLine.PriceFormula = obj.ToString().Trim();
			}
			else
			{
				flag = false;
				sbError.Append(" COD价格或公式不能为空 /");
			}
			#endregion

            #region 非COD价格公式
            obj = dr[5];
            if (obj.ToString().Trim() != "")
            {
                codLine.Formula = obj.ToString().Trim();
            }
            else
            {
                flag = false;
                sbError.Append(" 非COD价格或公式不能为空 /");
            }
            #endregion

			#region 线路状态
			obj = dr[6];
			string lineStatus = obj.ToString().Trim();
			if (lineStatus != "")
			{
				if (lineStatus != "" && (lineStatus == "可用" || lineStatus == "暂停"))
				{
					codLine.LineStatus = lineStatus == "可用" ? 1 : 0;
				}
				else
				{
					flag = false;
					sbError.Append(" 线路状态错误，只能“可用”或“暂停” /");
				}
			}
			else
			{
				flag = false;
				sbError.Append(" 线路状态不能为空 /");
			}
			#endregion
			return flag;
		}		

        #region 配送费计算查询
        /// <summary>
        /// 获取可用的Cod线路价格
        /// </summary>
        /// <returns></returns>
        public DataTable GetCodLine(int expressCompanyId, int areaType, string distributionCode)
        {
            return _deliveryPriceDao.GetCodLine(expressCompanyId, areaType, distributionCode);
        }

        /// <summary>
        /// 返回当前月前4个月的历史
        /// </summary>
        /// <returns></returns>
        public DataTable GetCodLineHistory(int year, int month, int expressCompanyId, int areaType, string distributionCode)
        {
            return _deliveryPriceDao.GetCodLineHistory( year,  month,  expressCompanyId,  areaType,  distributionCode);
        }
        #endregion

        #region cod价格变更校对
        public List<FMS_CODLine> GetEffectCodLine(DateTime effectDate)
        {
            return _deliveryPriceDao.GetEffectCodLine(GetStartMonth(effectDate), GetEndMonth(effectDate));
        }
        #endregion

        #region CODLine备份
        public bool Insert(IList<FMS_CODLine> codLineList, string month, string year)
        {
            return _deliveryPriceDao.Insert(codLineList, month, year);
        }

        public int UpdateToDelete(string year, string month)
        {
            return _deliveryPriceDao.UpdateToDelete(year, month);
        }

        public IList<FMS_CODLine> GetBackList()
        {
            return _deliveryPriceDao.GetBackList();
        }
        #endregion

        #region COD待生效
        public List<FMS_CODLine> GetCODLineWaitEffect(string Date)
        {
            return _deliveryPriceDao.GetCODLineWaitEffect(Date);
        }

        public bool UpdateLine(FMS_CODLine clwe)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                _deliveryPriceDao.UpdateLineForCODLine(clwe);

                _deliveryPriceDao.UpdateLineForCODLineWaitEffect(clwe);

                work.Complete();
                return true;
            }
        }
        #endregion

        #region public
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
        /// <summary>
        /// 取指定日期月份所在第一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DateTime GetStartMonth(DateTime dateTime)
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
        public DateTime GetEndMonth(DateTime dateTime)
        {
            DateTime dateTimeNew = dateTime.AddMonths(1);
            int year = dateTimeNew.Year;
            int month = dateTimeNew.Month;
            return new DateTime(year, month, 1);
        }
        #endregion
    }
}
