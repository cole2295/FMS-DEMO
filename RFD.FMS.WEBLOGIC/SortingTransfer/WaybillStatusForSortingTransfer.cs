using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.Mail;
using RFD.FMS.Util;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEBLOGIC.SortingTransfer
{
    public class WaybillStatusForSortingTransfer : IWaybillStatusForSortingTransfer, IWaybillStatusObserver
    {

        private ISortingTransferDetailDao SqlServerstdDao = null;
        private ISortingTransferDetailDao OraclestdDao = null;
        private RFD.FMS.WEBLOGIC.Mail.Mail mail = new RFD.FMS.WEBLOGIC.Mail.Mail();
        private string Tips = "";

        public bool DoAction(WaybillStatusChangeLog model)
        {
            //WaybillStatusChangeLog model = ObjModel as WaybillStatusChangeLog;
            try
            {
                if (model.Status != "-5" && model.Status != "-4" && model.Status != "4" && model.Status != "-9")
                {
                    InsertIntoSortingTransferDetail(model);
                }

                return true;
            }
            catch (Exception ex)
            {
                mail.SendMailToUser("运单状态推送服务异常", "运单号：" + model.WaybillNo + "\t\n插入失败" + ex.Message + ex.StackTrace, "zhangrongrong@vancl.cn;");

                return false;
            }
        }


        public int InsertIntoSortingTransferDetail(WaybillStatusChangeLog model)
        {
            try
            {
                var OraclestdDao = new RFD.FMS.DAL.Oracle.FinancialManage.SortingTransferDetailDao();
                if (!OraclestdDao.ExistFMS_SortingTransferDetailByNo(model.WaybillNo))
                {

                    var fmsmodel = GetAllSortingDetail(model.WaybillNo);
                    if (fmsmodel == null)
                        return 1;
                    fmsmodel.IntoType = 0;
                    fmsmodel.OutType = 0;
                    fmsmodel.IsAccount = 0;
                    fmsmodel.IsDelete = false;
                    IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                    fmsmodel.DetailKID = iDGenerate.ServiceNewId("FMS_SortingTransferDetail", "DetailKid");
                    OraclestdDao.Add(fmsmodel);
                }


                if (model.Status == "-1")
                {
                    var fmsmodel = GetMerchantToSortingCenter(model.WaybillNo);
                    if (fmsmodel != null)
                    {
                        string DetailKid = OraclestdDao.ExsitInSorting(fmsmodel);
                        if (string.IsNullOrEmpty(DetailKid))
                        {
                            fmsmodel.IsAccount = 0;
                            fmsmodel.IsDelete = false;
                            fmsmodel.IntoType = 0;
                            fmsmodel.OutType = 0;
                            IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                            fmsmodel.DetailKID = iDGenerate.ServiceNewId("FMS_SortingTransferDetail", "DetailKid");
                            OraclestdDao.Add(fmsmodel);
                        }
                        else
                        {
                            fmsmodel.DetailKID = DetailKid;
                            OraclestdDao.UpdateFMS_MerchantToSortingCenter(fmsmodel);
                        }

                    }
                    /*
                    else
                    {
                        Tips = "订单" + model.WaybillNo + "在入库状态没有抓取到数据,时间" + DateTime.Now;
                        mail.SendMailToUser("运单状态推送服务异常", Tips, "xueyi@vancl.cn;zengwei@vancl.cn;");
                    }
                    */
                }

                if (model.Status == "1")
                {
                    var fmsmodel = GetSortingToStation(model.WaybillNo);

                    if (fmsmodel != null)
                    {
                        foreach (var fmodel in fmsmodel)
                        {
                            string DetailKid = OraclestdDao.ExistIntoStation(fmodel);

                            if (string.IsNullOrEmpty(DetailKid))
                            {
                                fmodel.IntoType = 1;
                                fmodel.OutType = 0;
                                fmodel.IsAccount = 0;
                                fmodel.IsDelete = false;
                                IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                                fmodel.DetailKID = iDGenerate.ServiceNewId("FMS_SortingTransferDetail", "DetailKid");
                                OraclestdDao.Add(fmodel);

                            }
                            else
                            {
                                fmodel.DetailKID = DetailKid;

                                OraclestdDao.UpdateFMS_SortingToStation(fmodel);
                            }
                        }

                    }
                }

                if (model.Status == "10" || model.Status == "-3")
                {

                    var fmsmodel = GetSortingToCity(model.WaybillNo);
                    if (fmsmodel != null)
                    {
                        foreach (var fmodel in fmsmodel)
                        {
                            string DetailKid = OraclestdDao.ExistOutBound(fmodel);
                            if (string.IsNullOrEmpty(DetailKid))
                            {
                                fmodel.IntoType = 0;
                                fmodel.OutType = 1;
                                fmodel.IsAccount = 0;
                                fmodel.IsDelete = false;
                                IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                                fmodel.DetailKID = iDGenerate.ServiceNewId("FMS_SortingTransferDetail", "DetailKid");
                                OraclestdDao.Add(fmodel);
                            }
                            else
                            {
                                fmodel.OutType = 1;
                                fmodel.DetailKID = DetailKid;
                                OraclestdDao.UpdateFMS_SortingToCity(fmodel);
                            }

                        }

                    }
                    else
                    {
                        if (model.Status != "-3")
                        {
                            Tips = "订单" + model.WaybillNo + "在出库状态10没有抓取到数据，时间:" + DateTime.Now;
                            mail.SendMailToUser("运单状态推送服务异常", Tips, "xueyi@vancl.cn;zengwei@vancl.cn;");
                        }

                    }
                }


                if (model.SubStatus == 6 || model.SubStatus == 7 || model.SubStatus == 13)
                {
                    var fmsmodel = GetReturnToSortingCenter(model.WaybillNo);
                    if (fmsmodel != null)
                    {
                        var dt = OraclestdDao.GetTableByNo(model.WaybillNo);
                        if(dt !=null && dt.Rows.Count>0 )
                        {
                            DataRow[] rows = dt.Select("", "CreateTime DESC");
                            fmsmodel.DetailKID = rows[0]["DetailKid"].ToString();
                            OraclestdDao.UpdateFMS_ReturnToSortingCenter(fmsmodel);
                        }
                       
                    }
                    else
                    {
                        Tips = "订单" + model.WaybillNo + "在拒收入库或退换入库状态" + model.SubStatus + "没有抓取到数据，时间:" + DateTime.Now;
                        mail.SendMailToUser("运单状态推送服务异常", Tips, "xueyi@vancl.cn;zengwei@vancl.cn;");
                    }
                }

                return 1;
            }


            catch (Exception ex)
            {
                mail.SendMailToUser("运单状态推送服务异常", ex.Message + ex.StackTrace + "Model.Status =" + model.Status + "Model.WaybillNo =" + model.WaybillNo, "xueyi@vancl.cn;zengwei@vancl.cn;");

                throw ex;
            }

        }



        private FMS_SortingTransferDetail GetAllSortingDetail(Int64 waybillno)
        {
            IWaybillDao waybillDao = ServiceLocator.GetService<IWaybillDao>();
            var dt = waybillDao.GetAllSortingData(waybillno);
            if (dt != null)
            {
                var Model = new FMS_SortingTransferDetail();
                Model.WaybillNO = waybillno;
                Model.WaybillType = dt.Rows[0]["WaybillType"].ToString();
                Model.MerchantID = DataConvert.ToInt(dt.Rows[0]["MerchantID"]);
                Model.DistributionCode = dt.Rows[0]["DistributionCode"].ToString();
                Model.TopCODCompanyID = DataConvert.ToInt(dt.Rows[0]["TopCODCompanyID"]);
                Model.DeliverStationID = DataConvert.ToInt(dt.Rows[0]["DeliverStationID"]);
                Model.IsAccount = 0;
                Model.CreateTime = DateTime.Now;
                Model.UpdateTime = DateTime.Now;
                Model.IsDelete = false;
                Model.OutType = 0;
                Model.IntoType = 0;
                return Model;
            }
            return null;
            //return new FMS_SortingTransferDetail()
            //{
            //    IsAccount = 0,
            //    CreateTime = DateTime.Now,
            //    UpdateTime = DateTime.Now,
            //    IsDelete = false,
            //    WaybillNO = waybillno,
            //    OutType = 0,
            //    IntoType = 0,
            //    MerchantID = 0
            //};
        }

        private List<FMS_SortingTransferDetail> GetSortingToStation(Int64 waybillno)
        {
            IWaybillDao waybillDao = ServiceLocator.GetService<IWaybillDao>();
            var dt = waybillDao.GetSortingToStation(waybillno);
            List<FMS_SortingTransferDetail> list = new List<FMS_SortingTransferDetail>();
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var Model = new FMS_SortingTransferDetail();
                    Model.WaybillNO = waybillno;
                    Model.WaybillType = dt.Rows[i]["WaybillType"].ToString();
                    Model.MerchantID = DataConvert.ToInt(dt.Rows[i]["MerchantID"]);
                    Model.DeliverStationID = DataConvert.ToInt(dt.Rows[i]["IntoStation"]);
                    Model.ToStationTime = DataConvert.ToDateTime(dt.Rows[i]["IntoTime"]);
                    Model.TSortingCenter = DataConvert.ToInt(dt.Rows[i]["OutBoundStation"]);
                    Model.SortingCityID = dt.Rows[i]["CityID"].IsNullData() ? null : dt.Rows[0]["CityID"].ToString();
                    Model.TopCODCompanyID = DataConvert.ToInt(dt.Rows[i]["TopCODCompanyID"]);
                    Model.DistributionCode = dt.Rows[i]["DistributionCode"].ToString();
                    list.Add(Model);
                }

                return list;
            }
            return null;
        }


        private FMS_SortingTransferDetail GetMerchantToSortingCenter(Int64 waybillno)
        {
            IWaybillDao waybillDao = ServiceLocator.GetService<IWaybillDao>();
            var dt = waybillDao.GetMerchantToSortingCenter(waybillno);
            if (dt != null)
            {
                var Model = new FMS_SortingTransferDetail();
                Model.WaybillNO = waybillno;
                Model.WaybillType = dt.Rows[0]["WaybillType"].ToString();
                Model.MerchantID = DataConvert.ToInt(dt.Rows[0]["MerchantID"]);
                Model.DistributionCode = dt.Rows[0]["DistributionCode"].ToString();
                Model.InSortingTime = DataConvert.ToDateTime(dt.Rows[0]["IntoTime"]);
                Model.SortingCenter = DataConvert.ToInt(dt.Rows[0]["CreatStation"]);
                Model.CreateCityID = dt.Rows[0]["CityID"].ToString();
                Model.TopCODCompanyID = DataConvert.ToInt(dt.Rows[0]["TopCODCompanyID"]);
                Model.DeliverStationID = DataConvert.ToInt(dt.Rows[0]["DeliverStationID"]);
                return Model;
            }
            return null;
        }

        private List<FMS_SortingTransferDetail> GetSortingToCity(Int64 waybillno)
        {
            IWaybillDao waybillDao = ServiceLocator.GetService<IWaybillDao>();
            var dt = waybillDao.GetSortingToCity(waybillno);
            List<FMS_SortingTransferDetail> list = new List<FMS_SortingTransferDetail>();
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var Model = new FMS_SortingTransferDetail();
                    Model.WaybillNO = waybillno;
                    Model.WaybillType = dt.Rows[i]["WaybillType"].ToString();
                    Model.MerchantID = DataConvert.ToInt(dt.Rows[i]["MerchantID"]);
                    Model.OutBoundTime = DataConvert.ToDateTime(dt.Rows[i]["OutBoundTime"]);
                    Model.SortingCenter = DataConvert.ToInt(dt.Rows[i]["OutBoundStation"]);
                    Model.TSortingCenter = DataConvert.ToInt(dt.Rows[i]["ToStation"]);
                    Model.CreateCityID = dt.Rows[i]["CityID"].IsNullData() ? null : dt.Rows[i]["CityID"].ToString();
                    Model.DistributionCode = dt.Rows[i]["DistributionCode"].ToString();
                    Model.TopCODCompanyID = DataConvert.ToInt(dt.Rows[i]["TopCODCompanyID"]);
                    Model.DeliverStationID = DataConvert.ToInt(dt.Rows[i]["DeliverStationID"]);
                    Model.OutType = 1;
                    list.Add(Model);
                }

                return list;
            }
            return null;
        }

        private FMS_SortingTransferDetail GetReturnToSortingCenter(long waybillno)
        {
            IWaybillDao waybillDao = ServiceLocator.GetService<IWaybillDao>();
            var dt = waybillDao.GetReturnToSortingCenter(waybillno);
            if (dt != null)
            {
                var Model = new FMS_SortingTransferDetail();
                Model.WaybillNO = waybillno;
                Model.WaybillType = dt.Rows[0]["WaybillType"].ToString();
                Model.MerchantID = DataConvert.ToInt(dt.Rows[0]["MerchantID"]);
                Model.ReturnTime = DataConvert.ToDateTime(dt.Rows[0]["ReturnTime"]);
                Model.ReturnSortingCenter = (DataConvert.ToInt(dt.Rows[0]["MerchantID"]) == 8 ||
                                             DataConvert.ToInt(dt.Rows[0]["MerchantID"]) == 9)
                                             ? dt.Rows[0]["ReturnWarehouse"].ToString() : dt.Rows[0]["ReturnExpressCompanyId"].ToString();
                Model.DistributionCode = dt.Rows[0]["DistributionCode"].ToString();
                Model.TopCODCompanyID = DataConvert.ToInt(dt.Rows[0]["TopCODCompanyID"]);
                Model.DeliverStationID = DataConvert.ToInt(dt.Rows[0]["DeliverStationID"]);
                return Model;
            }
            return null;
        }

        public string GetSqlCondition()
        {
            return null;
        }

        public bool IsFalseToRePush
        {
            get { return true; }
        }

        public string Key
        {
            get { return "FMS_SortingTransferDetailService"; }
        }
    }
}
