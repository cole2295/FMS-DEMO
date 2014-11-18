using System;
using System.Data;
using System.Text;
using System.Configuration;
using RFD.FMS.Service.COD;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Domain.COD;
using RFD.FMS.MODEL;
using RFD.FMS.Service;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Util;
using RFD.FMS.Service.Mail;

namespace RFD.FMS.ServiceImpl.COD
{
	public class FMS_CODService : IFMS_CODService
	{
		private IFMS_CODDao cODDao;

        public FMS_CODService(IFMS_CODDao dao)
		{
			this.cODDao = dao;
		}

		public bool AddWaybillLmsToMedium(LMS_Syn_FMS_COD model)
		{
		    int result;
            if(model.StationID<=0)
            {
                return false;
            }
            if (model.OperateType == 1 || model.OperateType == 4 || model.OperateType == 5)
            {
                result = cODDao.AddWaybillLmsToMediumV2(model);             
            }
            else
            {
               result= cODDao.AddWaybillLmsToMedium(model);
               
            }
            return result == 1;			
		}

		public bool UpDateMediumForSyn(string ids, int isSyn)
		{
			var result = cODDao.UpDateMediumForSyn(ids, isSyn);
			return result >= 1;
		}

		public DataTable SearchIdsForShip(string topNumber, string synType)
		{
			return cODDao.SearchIdsForShip(topNumber, synType);
		}

		public DataTable SearchIdsForBack(string topNumber, string synType)
		{
			return cODDao.SearchIdsForBack(topNumber, synType);
		}

		public DataTable SearchIdsForBackAdvanced(string topNumber, string synType)
		{
			return cODDao.SearchIdsForBackAdvanced(topNumber, synType);
		}

		public DataTable SearchWaybillnoForBack(string topNumber)
		{
			return cODDao.SearchWaybillnosForBack(topNumber);
		}

		public DataTable SearchInfoForShip(string ids)
		{
			return cODDao.SearchInfoForShip(ids);
		}

		public DataTable SearchInfoForBack(string ids)
		{
			return cODDao.SearchInfoForBack(ids);
		}

		public DataTable SearchInfoForBackAdvanced(string ids)
		{
			return cODDao.SearchInfoForBackAdvanced(ids);
		}

		/// <summary>
		/// 校验正向数据有效性
		/// </summary>
		/// <param name="waybill"></param>
		/// <param name="model"></param>
        /// <param name="reason"></param>
		/// <returns></returns>
		public bool ShouldInsertForShip(DataRow waybill, out FMS_CODBaseInfo model,out string reason)
		{
			model = new FMS_CODBaseInfo();
		    var waybillno = Convert.ToInt64(waybill["WaybillNO"]);
            var mediumId = Convert.ToInt64(waybill["MediumId"]);
		    reason = "";
		    string stationCode;
			var operateType = Convert.ToInt32(waybill["operateType"]);
            string stationDistributionCode = waybill["StationDistributionCode"].ToString();

            #region 第三方配送商内部转站判断逻辑

		    var check = true;
            StringBuilder sbError = new StringBuilder();
            if (operateType == (int)BizEnums.CODOperateType.TransferOUT)
            {
                //转出时，找大于当前对应ID的第一条转入的operateType=3
                stationCode = cODDao.GetStationByIDNOTwo(mediumId, waybillno);
                if(string.IsNullOrEmpty((stationCode)))
                {
                    check = false;
                }
                else if (stationCode == stationDistributionCode &&
                           stationCode.ToLower()!="rfd" &&
                           stationDistributionCode.ToLower() != "rfd")
                {
                    sbError.Append("T");//后期不作处理
                    check = false;
                }
            }
            else if (operateType == (int)BizEnums.CODOperateType.TransferIN)
            {
                //转入时，找小于当前对应ID的第一条转入的operateType=2(从大到小)
                stationCode = cODDao.GetStationByIDNOThird(mediumId, waybillno);
                if (string.IsNullOrEmpty((stationCode)))
                {
                    check = false;
                }
                else if (stationCode == stationDistributionCode &&
                           stationCode.ToLower() != "rfd" &&
                           stationDistributionCode.ToLower() != "rfd")
                {
                    sbError.Append("T");//后期不作处理
                    check = false;
                }
            }

            if (!check)
            {
                sbError.Append("第三方配送商内部转站--"
                         + "waybillno:" + waybillno
                         + "mediumid:" + mediumId
                         + "operateType:" + operateType);
                reason = sbError.ToString();
                return false;
            }
            #endregion 第三方配送商内部转站判断逻辑

            #region 检查
            if (string.IsNullOrEmpty(waybill["WaybillType"].ToString()))
			{
                reason += "C1--Waybill:" + waybillno + "--The WaybillType is null or empty." + "\r\n" + "<br/>";
			    return false;
			}
			if (string.IsNullOrEmpty(waybill["MerchantID"].ToString()))
			{
				if (string.IsNullOrEmpty(waybill["sources"].ToString()))
				{
                    reason += "C2--Waybill:" + waybillno + "--Both MerchantID and sources are null." + "\r\n" + "<br/>";
					return false;
				}
				if (Convert.ToInt32(waybill["sources"].ToString()) == 0)
				{
					waybill["MerchantID"] = 8;
				}
				if (Convert.ToInt32(waybill["sources"].ToString()) == 1)
				{
					waybill["MerchantID"] = 9;
				}
			}

			if (string.IsNullOrEmpty(waybill["StationID"].ToString()))
			{
                reason += "C3--Waybill:" + waybillno + "--The stationid is null." + "\r\n" + "<br/>";
			    return false;
			}
			if (Convert.ToInt32(waybill["StationID"]) <= 0)
			{
                reason += "C4--Waybill:" + waybillno + "--The StationID is no more than 0." + "\r\n" + "<br/>";
			    return false;
			}
            if (string.IsNullOrEmpty(waybill["CreatTime"].ToString()))
            {
                reason += "C5--Waybill:" + waybillno + "--The CreatTime is null." + "\r\n" + "<br/>";
                return false;
            }
			if (Convert.ToInt32(waybill["MerchantID"]) == 8
				|| Convert.ToInt32(waybill["MerchantID"]) == 9)
			{
				if (string.IsNullOrEmpty(waybill["WarehouseId"].ToString()))
				{
                    reason += "C6--Waybill:" + waybillno + "--The Waybill of Vancl or Vjia has no WarehouseId." + "\r\n" + "<br/>";
				    return false;
				}
                //if (string.IsNullOrEmpty(waybill["DeliverTime"].ToString()))
                //    waybill["DeliverTime"] = waybill["CreatTime"];
			}
			else
			{
				if (string.IsNullOrEmpty(waybill["CreatStation"].ToString()))
				{
                    reason += "C7--Waybill:" + waybillno + "--The Waybill of third party of merchants has no CreatStation." + "\r\n" + "<br/>";
				    return false;
				}
			}
			

			if (string.IsNullOrEmpty(waybill["TopCODCompanyID"].ToString()))
			{
                reason += "C8--Waybill:" + waybillno + "--The TopCODCompanyID of this waybill'stationid has not been settled." + "\r\n" + "<br/>";
			    return false;
			}
			if (Convert.ToInt32(waybill["TopCODCompanyID"]) <= 0)
			{
                reason += "C9--Waybill:" + waybillno + "--The TopCODCompanyID of this waybill'stationid is no more than 0." + "\r\n" + "<br/>";
                return false;
			}

            //if (string.IsNullOrEmpty(waybill["AreaID"].ToString()))
            //{
            //    reason += "C10--Waybill:" + waybillno + "--The search of AreaID is not successful." + "\r\n" + "<br/>";
            //    return false;
            //}
            //if (string.IsNullOrEmpty(waybill["ReceiveAddress"].ToString()))
            //{
            //    reason += "C11--Waybill:" + waybillno + "--The ReceiveAddress is null or empty." + "\r\n" + "<br/>";
            //    return false;
            //}


            if (string.IsNullOrEmpty(waybill["Amount"].ToString()))
                waybill["Amount"] = 0;
			if (string.IsNullOrEmpty(waybill["PaidAmount"].ToString()))
				waybill["PaidAmount"] = 0;
			if (string.IsNullOrEmpty(waybill["NeedAmount"].ToString()))
				waybill["NeedAmount"] = 0;
			if (string.IsNullOrEmpty(waybill["TransferFee"].ToString()))
				waybill["TransferFee"] = 0;
			if (string.IsNullOrEmpty(waybill["additionalprice"].ToString()))
				waybill["additionalprice"] = 0;
			if (string.IsNullOrEmpty(waybill["NeedBackAmount"].ToString()))
				waybill["NeedBackAmount"] = 0;
			if (string.IsNullOrEmpty(waybill["FactBackAmount"].ToString()))
				waybill["FactBackAmount"] = 0;
			if (string.IsNullOrEmpty(waybill["WayBillInfoWeight"].ToString()))
				waybill["WayBillInfoWeight"] = 0;
			if (string.IsNullOrEmpty(waybill["WayBillInfoVolumeWeight"].ToString()))
				waybill["WayBillInfoVolumeWeight"] = 0;


			#endregion 检查
			#region 赋值
            
			model.OperateType = Convert.ToInt32(waybill["OperateType"]);
			model.IsFare = 0;
			model.MediumID = Convert.ToInt64(waybill["MediumID"]);
			model.WaybillNO = Convert.ToInt64(waybill["WaybillNO"]);
			model.MerchantID = Convert.ToInt32(waybill["MerchantID"]);
			model.WaybillType = waybill["WaybillType"].ToString();
            if (operateType == Convert.ToInt32(BizEnums.CODOperateType.Delivery)
             || operateType == Convert.ToInt32(BizEnums.CODOperateType.TransferIN))
			{
				model.Flag = 1;
			}
            if (operateType == Convert.ToInt32(BizEnums.CODOperateType.TransferOUT)
            || operateType == Convert.ToInt32(BizEnums.CODOperateType.Rejection)
            || operateType == Convert.ToInt32(BizEnums.CODOperateType.Invalid))
			{
				model.Flag = 0;
			}


			model.DeliverStationID = Convert.ToInt32(waybill["StationID"]);

			model.TopCODCompanyID = Convert.ToInt32(waybill["TopCODCompanyID"]);
			if (model.MerchantID == 8 || model.MerchantID == 9)
			{
				model.WarehouseId = waybill["WarehouseId"].ToString();
				model.ExpressCompanyID = null;
			}
			else
			{
				model.WarehouseId = null;
				model.ExpressCompanyID = Convert.ToInt32(waybill["CreatStation"]);
			}
            //转出转入的，时间以当前时间
            if (operateType == Convert.ToInt32(BizEnums.CODOperateType.TransferIN) ||
                operateType == Convert.ToInt32(BizEnums.CODOperateType.TransferOUT))
            {
                DateTime dt = DateTime.Now;
                model.RfdAcceptTime = dt;
                model.RfdAcceptDate = dt;
            }
            else
            {
                model.RfdAcceptTime = Convert.ToDateTime(waybill["CreatTime"]);
                model.RfdAcceptDate = waybill["CreatTime"].ToString().TryGetDateTime();
            }
			if (!string.IsNullOrEmpty(waybill["OutBoundStation"].ToString()))
			{
				model.FinalExpressCompanyID = Convert.ToInt32(waybill["OutBoundStation"]);
			}
			else
			{
				model.FinalExpressCompanyID = null;
			}

            //先赋值
            if (!string.IsNullOrEmpty(waybill["DeliverTime"].ToString()))
            {
                model.DeliverTime = Convert.ToDateTime(waybill["DeliverTime"]);
                model.DeliverDate = waybill["DeliverTime"].ToString().TryGetDateTime();
            }
            else
            {
                model.DeliverTime = null;
                model.DeliverDate = null;
            }
            //置为无效订单单独处理//转出、转入
            if (operateType == Convert.ToInt32(BizEnums.CODOperateType.Invalid) ||
                operateType == Convert.ToInt32(BizEnums.CODOperateType.TransferIN) ||
                operateType == Convert.ToInt32(BizEnums.CODOperateType.TransferOUT)
                )
            {
                //如果出库时间不为空，需要将出库时间改为当前时间，以便能被结算，否则继续为空，等待出库时间来更新
                if (!string.IsNullOrEmpty(model.DeliverTime.ToString()) && model.DeliverTime>DateTime.MinValue)
                {
                    model.DeliverTime = DateTime.Now;
                    model.DeliverDate = model.DeliverTime.ToString().TryGetDateTime();
                }
            }

			model.AreaID = waybill["AreaID"].ToString();
			if (!string.IsNullOrEmpty(waybill["WayBillBoxNo"].ToString()))
			{
				model.BoxsNo = waybill["WayBillBoxNo"].ToString();
			}
			else
			{
				model.BoxsNo = null;
			}
            model.AreaType = 0;//监测使用，监测结束后需要删除
			model.PaidAmount = Convert.ToDecimal(waybill["PaidAmount"]);
			model.NeedPayAmount = Convert.ToDecimal(waybill["NeedAmount"]);
			model.NeedBackAmount = Convert.ToDecimal(waybill["NeedBackAmount"]);
			model.BackAmount = Convert.ToDecimal(waybill["FactBackAmount"]);

            //订单总价
            decimal allAmount=Convert.ToDecimal(waybill["Amount"]) + Convert.ToDecimal(waybill["TransferFee"]) + Convert.ToDecimal(waybill["additionalprice"]);
            decimal needPaidAmount = Convert.ToDecimal(waybill["NeedAmount"]) + Convert.ToDecimal(waybill["PaidAmount"]);
            if (model.MerchantID == 8 || model.MerchantID == 9)
            {
                model.TotalAmount = allAmount;
            }
            else if (allAmount >= needPaidAmount)
            {
                model.TotalAmount = allAmount;
            }
            else
            {
                model.TotalAmount = needPaidAmount;
            }

            //结算重量
            model.AccountWeight = Convert.ToDecimal(waybill["WayBillInfoWeight"]);//只取分拣称重重量

            //地址
			model.Address =
				waybill["ReceiveProvince"].ToString()
			  + waybill["ReceiveCity"]
			  + waybill["ReceiveArea"]
			  + waybill["ReceiveAddress"];

            //保价金额
            model.ProtectedPrice = waybill["ProtectedPrice"] == DBNull.Value ? 0.00M : waybill["ProtectedPrice"].ToString().TryGetDecimal();
            model.DistributionCode = waybill["DistributionCode"].ToString();
            model.CurrentDistributionCode = waybill["CurrentDistributionCode"].ToString();
            model.ComeFrom = waybill["ComeFrom"] == DBNull.Value ? 0 : Convert.ToInt32(waybill["ComeFrom"]);
            model.IsCOD = 0;//是否区分COD 默认不区分
			#endregion 赋值
			return true;
		}

		public bool ShouldUpdateForBack(DataRow waybill, out FMS_CODBaseInfo model,out string reason)
		{
			model = new FMS_CODBaseInfo();
		    reason = "";
            var mediumid = DataConvert.ToLong(waybill["ID"]);
			if (string.IsNullOrEmpty(waybill["MerchantID"].ToString()))
			{
				if (string.IsNullOrEmpty(waybill["sources"].ToString()))
				{
                    reason += "H1--Mediumid:" + mediumid + "--Both MerchantID and sources are null." + "\r\n" + "<br/>";
					return false;
				}
                if (DataConvert.ToInt(waybill["sources"].ToString()) == 0)
				{
					waybill["MerchantID"] = 8;
				}
                if (DataConvert.ToInt(waybill["sources"].ToString()) == 1)
				{
					waybill["MerchantID"] = 9;
				}
			}

            if (DataConvert.ToInt(waybill["MerchantID"]) == 8
                || DataConvert.ToInt(waybill["MerchantID"]) == 9)
			{
				if (string.IsNullOrEmpty(waybill["ReturnWareHouse"].ToString()))
				{
                    reason += "H2--Mediumid:" + mediumid + "--The waybill of Vancl or Vjia is without ReturnWareHouse." + "\r\n" + "<br/>";
				    return false;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(waybill["ReturnExpressCompanyId"].ToString()))
				{
                    reason += "H3--Mediumid:" + mediumid + "--The waybill of third party of merchant is without ReturnExpressCompanyId." + "\r\n" + "<br/>";
                    return false;
				}
			}
			if (string.IsNullOrEmpty(waybill["ReturnTime"].ToString()))
			{
                reason += "H4--Mediumid:" + mediumid + "--The waybill  is without ReturnTime." + "\r\n" + "<br/>";
			    return false;
			}
            if (string.IsNullOrEmpty(waybill["FactBackAmount"].ToString()))
                waybill["FactBackAmount"] = 0;
			///////////////////////////////////////////////////////
			//model.MediumID = Convert.ToInt64(waybill["MediumID"]); 
			model.MediumID = Convert.ToInt64(waybill["ID"]);
			////
            if (DataConvert.ToInt(waybill["MerchantID"]) == 8
                || DataConvert.ToInt(waybill["MerchantID"]) == 9)
			{
				model.ReturnWareHouseID = waybill["ReturnWareHouse"].ToString();
				model.ReturnExpressCompanyID = null;
			}
			else
			{
				model.ReturnWareHouseID = null;
				model.ReturnExpressCompanyID = Convert.ToInt32(waybill["ReturnExpressCompanyID"]);
			}
            model.ReturnTime = DataConvert.ToDateTime(waybill["ReturnTime"]);
            model.ReturnDate = DateTime.Parse(DataConvert.ToDateTime(waybill["ReturnTime"]).ToString()).ToShortDateString();
            model.DeliverStationID = DataConvert.ToInt(waybill["DeliverStationID"]);
            model.TopCODCompanyID = DataConvert.ToInt(waybill["TopCODCompanyID"]);

            model.BackAmount = DataConvert.ToDecimal(waybill["FactBackAmount"]); 
            return true;
		}

		public bool ShouldUpdateForBackAdvanced(DataRow waybill, out FMS_CODBaseInfo model,out string reason)
		{
			model = new FMS_CODBaseInfo();
            var mediumid = DataConvert.ToLong(waybill["ID"]);
		    reason = "";
			if (string.IsNullOrEmpty(waybill["waybillno"].ToString()))
			{
                reason += "L1--不带这么玩的吧！运单号都木有！Mediumid:" + mediumid.ToString() + DateTime.Now + "\r\n" + "<br/>";
				return false;
			}
			if (string.IsNullOrEmpty(waybill["DeliverTime"].ToString()))
			{
                reason += "L2--不带这么玩的吧！waybill表的DeliverTime都木有！Mediumid:" + mediumid.ToString() + DateTime.Now + "\r\n" + "<br/>";
				return false;
			}
			if (string.IsNullOrEmpty(waybill["OutBoundStation"].ToString()))
			{
                reason += "L3--不带这么玩的吧！OutBound表的OutBoundStation都木有！Mediumid:" + mediumid.ToString() + DateTime.Now + "\r\n" + "<br/>";
				return false;
			}
            model.WaybillNO = DataConvert.ToLong(waybill["waybillno"]);
            model.DeliverTime = DataConvert.ToDateTime(waybill["DeliverTime"]);
			model.DeliverDate = waybill["DeliverTime"].ToString().TryGetDateTime();
            model.FinalExpressCompanyID = DataConvert.ToInt(waybill["OutBoundStation"]);
            model.AccountWeight = DataConvert.ToDecimal(waybill["WayBillInfoWeight"]);
            model.MerchantID = DataConvert.ToInt(waybill["MerchantID"]);
			return true;
		}

		public bool InsertForShip(FMS_CODBaseInfo model)
		{
            if (model.OperateType == Convert.ToInt32(BizEnums.CODOperateType.Invalid))
            {
                //同步成功，同时更新同步标志位为1，否则回滚
                cODDao.UpdateForInvalid(model.WaybillNO);
            }
			var result = cODDao.InsertForShip(model);
		    if(result==0)
		    {
                var mail = ServiceLocator.GetService<IMail>();
                mail.SendMailToUser("财务数据同步报告", "因为重复不能插入数据:" + model.MediumID + "--" + model.WaybillNO, "zengwei@vancl.cn");
		    }
            if(result<0)
            {
                var mail = ServiceLocator.GetService<IMail>();
                mail.SendMailToUser("财务数据同步报告", "不重复但插入数据失败:" + model.MediumID + "--" + model.WaybillNO, "zengwei@vancl.cn");
            }
			return (result == 1 || result ==0) ;//重复数据视为成功
		}

		public bool UpdateForBack(FMS_CODBaseInfo model)
		{
			var result = cODDao.UpdateForBack(model);
			return result >= 1;//暂时这样处理，应当改为result == 1
		}

		public bool AdvancedUpdateForBack(FMS_CODBaseInfo model)
		{
			var result = cODDao.AdvancedUpdateForBack(model);
			return result >= 0;//暂时这样处理，应当改为result == 1
		}

        public string LmsSynFmsForBackTest(string ids)
        {
            return DisposeLmsSynFmsForBack(ids);
        }

        public string LmsSynFmsForBack(string topNumber, string synType)
        {
            var tip = "";
            try
            {
                //从主库获取应当同步的且类型为6的中间表记录ids
                var dt = SearchIdsForBack(topNumber, synType);
                var ids = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["ID"].ToString()))
                        ids += ("," + Convert.ToInt64(dr["ID"]));
                }
                //如果没有获取到id。直接返回
                if (ids.Length < 2)
                {
                    return "E--当前没有需要同步的逆向记录！" + " " + DateTime.Now.ToString() + "\r\n";
                }
                //移除ids字符串第一个字符，即第一个逗号
                ids = ids.Substring(1);

                tip = DisposeLmsSynFmsForBack(ids);
                return string.IsNullOrEmpty(tip) ? "" : tip + "\r\n" + "<br/>" + "<br/>";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string DisposeLmsSynFmsForBack(string ids)
        {
            var tip = "";
            try
            {
                //从只读获取应当更新的订单详情
                var waybillInfo = SearchInfoForBack(ids);
                //获取订单详情记录为0.说明数据关键字段不符合COD结算要求。
                if (waybillInfo.Rows.Count == 0)
                {
                    //更新同步标志位为2，同步未成功等待下次同步。直接返回
                    UpDateMediumForSyn(ids, 2);
                    return "F--未获取到业务数据，第一次逆向同步失败！" + "  Mediumid:" + ids + "    " + DateTime.Now.ToString() + "\r\n";
                }

                foreach (DataRow dr in waybillInfo.Rows)
                {
                    var operateType = Convert.ToInt32(dr["operateType"]);
                    ////中间表ID，用于更新同步标志位
                    var id = Convert.ToInt64(dr["ID"]);
                    FMS_CODBaseInfo modelForBack;
                    var waybillno = Convert.ToInt64(dr["WaybillNo"]);
                    var waybillType = dr["WaybillType"].ToString();
                    var backStatus = dr["BackStatus"].ToString();
                    if (operateType == Convert.ToInt32(BizEnums.CODOperateType.ReverseInbound))
                    {
                        var mediumId = cODDao.SearchUpdateMediumId(waybillno, waybillType, backStatus);
                        if (mediumId <= 0)
                        {
                            var add = new LMS_Syn_FMS_COD();
                            add.CreateBY = "system";
                            add.OperateTime = DateTime.Now;
                            add.OperateType = backStatus == "7" ? Convert.ToInt32(BizEnums.CODOperateType.Rejection) : Convert.ToInt32(BizEnums.CODOperateType.Delivery);
                            add.StationID = dr["StationID"].ToString().TryGetInt();
                            add.WayBillNO = waybillno;
                            UnitForDeliverTimeAndOutBountID(add, id.ToString());
                            tip += "G--无正向数据！" + mediumId.ToString() + "    Waybillno:" + waybillno + "\r\n";
                            continue;
                        }
                        else
                        {
                            dr["ID"] = mediumId;
                        }
                        var reason = "";
                        if (ShouldUpdateForBack(dr, out modelForBack, out reason))
                        {
                            if (!UpdateWorkUnit(modelForBack, id))
                            {
                                UpDateMediumForSyn(id.ToString(), 2);
                                tip += "I--更新数据事务失败！" + mediumId.ToString() + "    " + DateTime.Now + "\r\n";
                            }
                        }
                        else
                        {
                            UpDateMediumForSyn(id.ToString(), 2);
                            tip += reason + DateTime.Now + "\r\n" + "<br/>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tip + "\r\n" + "<br/>" + "<br/>";
        }

        public bool UnitForDeliverTimeAndOutBountID(LMS_Syn_FMS_COD model,string ids)
        {
            using (IUnitOfWork work = ServiceLocator.GetObject<IUnitOfWorkFactory>("AdoNetUnitOfWorkFactory").GetInstance(UnitOfWorkDefinition.DefaultDefinition))
            {
                if(!AddWaybillLmsToMedium(model)) return false;
                if(!UpDateMediumForSyn(ids.ToString(), 2)) return false;
                work.Complete();
            }
            return true;
        }

        /// <summary>
        /// LMS同步FMS--正向数据
        /// </summary>
        /// <param name="topNumber"></param>
        /// <param name="synType"></param>
        public string LmsSynFmsForShipBySynId(string synIds)
        {
            var tip = "";

            try
            {
                //从主库获取应当同步的中间表记录ids
                var dt = cODDao.SearchIdsForShipBySynId(synIds);
                var ids = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["ID"].ToString()))
                        ids += ("," + Convert.ToInt64(dr["ID"]));
                }
                //如果没有获取到id。直接返回
                if (ids.Length < 2)
                {
                    return "A--当前没有需要同步的正向记录！" + " " + DateTime.Now.ToString() + "\r\n";
                }
                //移除ids字符串第一个字符，即第一个逗号
                ids = ids.Substring(1);

                //从只读获取订单详情
                var waybillInfo = SearchInfoForShip(ids);
                //获取订单详情记录为0.说明数据关键字段不符合COD结算要求。
                if (waybillInfo.Rows.Count == 0)
                {
                    //更新同步标志位为2，同步未成功等待下次同步。直接返回
                    UpDateMediumForSyn(ids, 2);
                    return "B--未获取到业务数据，第一次正向同步失败！" + "   " + "Mediumid:" + ids + "  " + DateTime.Now + "\r\n";
                }

                //取到逐条数据校验
                foreach (DataRow dr in waybillInfo.Rows)
                {
                    var operateType = Convert.ToInt32(dr["operateType"]);
                    var mediumId = Convert.ToInt64(dr["MediumId"]);
                    if (operateType != Convert.ToInt32(BizEnums.CODOperateType.ReverseInbound))
                    {
                        FMS_CODBaseInfo modelForShip;
                        string reason = "";
                        if (ShouldInsertForShip(dr, out modelForShip, out reason))
                        {
                            //同步失败，更新同步标志位为2，同步失败，等待再次同步
                            if (!InsertWorkUnit(modelForShip, mediumId))
                            {
                                UpDateMediumForSyn(mediumId.ToString(), 2);
                                tip += "D--插入数据事务失败！" + mediumId.ToString() + "   " + "Waybillno:" + modelForShip.WaybillNO + "   " + DateTime.Now + "\r\n";
                            }
                        }
                        else if (reason.StartsWith("T"))
                        {
                            //更新同步标志位为3
                            UpDateMediumForSyn(mediumId.ToString(), 3);
                            tip += reason + "  " + DateTime.Now + "\r\n" + "<br/>";
                        }
                        else
                        {
                            //数据不合格，同步失败，更新同步标志位为2
                            UpDateMediumForSyn(mediumId.ToString(), 2);
                            tip += reason + "  " + DateTime.Now + "\r\n" + "<br/>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tip + "\r\n" + "<br/>" + "<br/>";
        }

		/// <summary>
		/// LMS同步FMS--正向数据
		/// </summary>
		/// <param name="topNumber"></param>
		/// <param name="synType"></param>
		public string LmsSynFmsForShip(string topNumber, string synType)
		{
			var tip = "";

			try
			{
				//从主库获取应当同步的中间表记录ids
				var dt = SearchIdsForShip(topNumber, synType);
				var ids = "";
				foreach (DataRow dr in dt.Rows)
				{
					if (!String.IsNullOrEmpty(dr["ID"].ToString()))
						ids += ("," + Convert.ToInt64(dr["ID"]));
				}
				//如果没有获取到id。直接返回
				if (ids.Length < 2)
				{
                    return "A--当前没有需要同步的正向记录！" +" "+ DateTime.Now.ToString() + "\r\n";
				}
				//移除ids字符串第一个字符，即第一个逗号
				ids = ids.Substring(1);

				//从只读获取订单详情
				var waybillInfo = SearchInfoForShip(ids);
				//获取订单详情记录为0.说明数据关键字段不符合COD结算要求。
				if (waybillInfo.Rows.Count == 0)
				{
					//更新同步标志位为2，同步未成功等待下次同步。直接返回
					UpDateMediumForSyn(ids, 2);
                    return "B--未获取到业务数据，第一次正向同步失败！" +"   "+"Mediumid:"+ids +"  " + DateTime.Now + "\r\n";
				}

				//取到逐条数据校验
				foreach (DataRow dr in waybillInfo.Rows)
				{
					var operateType = Convert.ToInt32(dr["operateType"]);
					var mediumId = Convert.ToInt64(dr["MediumId"]);
                    if (operateType != Convert.ToInt32(BizEnums.CODOperateType.ReverseInbound))
					{
						FMS_CODBaseInfo modelForShip;
					    string reason ="";
						if (ShouldInsertForShip(dr, out modelForShip,out reason))
						{
							//同步失败，更新同步标志位为2，同步失败，等待再次同步
							if (!InsertWorkUnit(modelForShip, mediumId))
							{
								UpDateMediumForSyn(mediumId.ToString(), 2);
                                tip +=  "D--插入数据事务失败！" + mediumId.ToString() + "   " +"Waybillno:"+ modelForShip.WaybillNO+"   "+DateTime.Now + "\r\n";
							}
						}
						else if(reason.StartsWith("T"))
						{
                            //更新同步标志位为3
                            UpDateMediumForSyn(mediumId.ToString(), 3);
                            tip += reason + "  " + DateTime.Now + "\r\n" + "<br/>";
						}
                        else
						{
							//数据不合格，同步失败，更新同步标志位为2
							UpDateMediumForSyn(mediumId.ToString(), 2);
                            tip += reason + "  " + DateTime.Now + "\r\n" + "<br/>" ;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
            return tip + "\r\n" + "<br/>" + "<br/>";
		}

		/// <summary>
		/// COD正向数据插入记录的事务
		/// </summary>
		/// <param name="modelForShip"></param>
		/// <param name="mediumId"></param>
		/// <returns></returns>
		public bool InsertWorkUnit(FMS_CODBaseInfo modelForShip, long mediumId)
		{
            if (!InsertForShip(modelForShip)) return false;
            if (!UpDateMediumForSyn(mediumId.ToString(), 1)) return false;
			return true;
		}

		public bool UpdateWorkUnit(FMS_CODBaseInfo modelForBack, long mediumId)
		{
			using (IUnitOfWork work = ServiceLocator.GetObject<IUnitOfWorkFactory>("AdoNetUnitOfWorkFactory").GetInstance(UnitOfWorkDefinition.DefaultDefinition))
			{
				if (!UpdateForBack(modelForBack)) return false;
				if (!UpDateMediumForSyn(mediumId.ToString(), 1)) return false;
				work.Complete();
			}
			return true;
		}

		public bool AdvancedUpdateWorkUnit(FMS_CODBaseInfo modelForBack, long id)
		{
			using (IUnitOfWork work = ServiceLocator.GetObject<IUnitOfWorkFactory>("AdoNetUnitOfWorkFactory").GetInstance(UnitOfWorkDefinition.DefaultDefinition))
			{
				if (!AdvancedUpdateForBack(modelForBack)) return false;
				if (!UpDateMediumForSyn(id.ToString(), 1)) return false;
				work.Complete();
			}
			return true;
		}

		public bool Unit(FMS_CODBaseInfo modelForBack, string ids)
		{
            //todo oracle
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
			{
				if (!AdvancedUpdateForBack(modelForBack)) return false;
				if (!UpDateMediumForSyn(ids,1)) return false;
				work.Complete();
			}
			return true;
		}

		bool UpdateWaybillForCOD(long waybillno, int issyn)
		{
			var reslut = cODDao.UpdateWaybillForCOD(waybillno, issyn);
			return reslut >= 1;
        }

        public string SearchAnyInfoOnMedium(string sql)
		{
			var ids = "";
			var dt = cODDao.SearchAnyInfoOnMedium(sql);
			if (dt == null || dt.Rows.Count <= 0)
				return ids;
			foreach (DataRow dr in dt.Rows)
			{
				if (!String.IsNullOrEmpty(dr[0].ToString()))
					ids += ("," + Convert.ToInt64(dr[0]));
			}
			if (ids.Length < 2)
			{
				return ids;
			}
			ids = ids.Substring(1);
			return ids;
		}

		public int UpdateDelete(string sql)
		{
			return cODDao.UpdateDelete(sql);
		}

        DataTable SearchIdsForDeliverTimeAndOutBountStation(string topNumber,string synType)
        {
            return cODDao.SearchIdsForDeliverTimeAndOutBountStation(topNumber, synType);
        }

        /// <summary>
        /// 单元测试
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string SynForDeliverTimeAndOutBountStationForTest(string ids)
        {
            var tip = "";
            try
            {
                //从主库获取应当同步的中间表记录ids
                //var dt = SearchIdsForDeliverTimeAndOutBountStation("100", "0");
                //var ids = "";
                //foreach (DataRow dr in dt.Rows)
                //{
                //    if (!String.IsNullOrEmpty(dr["ID"].ToString()))
                //        ids += ("," + Convert.ToInt64(dr["ID"]));
                //}
                //如果没有获取到id。直接返回
                if (ids.Length < 2)
                {
                    return "A--当前没有需要同步的正向记录！" + " " + DateTime.Now.ToString() + "\r\n";
                }
                //移除ids字符串第一个字符，即第一个逗号
                //ids = ids.Substring(1);

                //从只读获取订单详情
                var waybillInfo = cODDao.SearchInfoForDeliverTimeAndOutBountStation(ids);
                //获取订单详情记录为0.说明数据关键字段不符合COD结算要求。
                if (waybillInfo.Rows.Count == 0)
                {
                    //更新同步标志位为2，同步未成功等待下次同步。直接返回
                    UpDateMediumForSyn(ids, 2);
                    return "--未获取到业务数据，更新DeliverTime,OutBountStation失败！" + DateTime.Now + "\r\n";
                }

                foreach (DataRow dr in waybillInfo.Rows)
                {
                    FMS_CODBaseInfo modelForBack;
                    var waybillno = Convert.ToInt64(dr["WaybillNo"]);
                    var id = Convert.ToInt64(dr["ID"]).ToString();
                    var reason = "";
                    if (ShouldUpdateForBackAdvanced(dr, out modelForBack, out reason))
                    {
                        if (!Unit(modelForBack, id))
                        {
                            cODDao.UpDateMediumForSyn(id, 2);
                            tip += "J--更新数据事务失败！" + waybillno.ToString() + "    " + DateTime.Now + "\r\n";
                        }
                    }
                    else
                    {
                        cODDao.UpDateMediumForSyn(id, 2);
                        tip += reason + DateTime.Now + "\r\n" + "<br/>" + "<br/>";
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tip + "\r\n";
        }

		public string SynForDeliverTimeAndOutBountStation()
		{
			var tip = "";
			try
			{
                //从主库获取应当同步的中间表记录ids
                var dt = SearchIdsForDeliverTimeAndOutBountStation("100", "0");
                var ids = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["ID"].ToString()))
                        ids += ("," + Convert.ToInt64(dr["ID"]));
                }
                //如果没有获取到id。直接返回
                if (ids.Length < 2)
                {
                    return "A--当前没有需要同步的正向记录！" + " " + DateTime.Now.ToString() + "\r\n";
                }
                //移除ids字符串第一个字符，即第一个逗号
                ids = ids.Substring(1);

                //从只读获取订单详情
                var waybillInfo = cODDao.SearchInfoForDeliverTimeAndOutBountStation(ids);
                //获取订单详情记录为0.说明数据关键字段不符合COD结算要求。
                if (waybillInfo.Rows.Count == 0)
                {
                    //更新同步标志位为2，同步未成功等待下次同步。直接返回
                    UpDateMediumForSyn(ids, 2);
                    return "--未获取到业务数据，更新DeliverTime,OutBountStation失败！" + DateTime.Now + "\r\n";
                }

				
				foreach (DataRow dr in waybillInfo.Rows)
				{
					FMS_CODBaseInfo modelForBack;
					var waybillno = Convert.ToInt64(dr["WaybillNo"]);
                    var id = Convert.ToInt64(dr["ID"]).ToString();
				    var reason = "";
					if (ShouldUpdateForBackAdvanced(dr, out modelForBack,out reason))
					{
						if (!Unit(modelForBack, id))
						{
							cODDao.UpDateMediumForSyn(id,2);
							tip += "J--更新数据事务失败！" + waybillno.ToString() +"    "+DateTime.Now+ "\r\n";
						}
					}
					else
					{
                        cODDao.UpDateMediumForSyn(id, 2);
                        tip += reason + DateTime.Now + "\r\n" + "<br/>" + "<br/>";
					}

				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return tip + "\r\n";
		}
	}
}
