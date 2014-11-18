using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.HSSF.Record.Formula.Functions;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.DAL.FinancialManage;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Service.SoringManage;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC.FinancialManage;
using RFD.FMS.WEBLOGIC.Mail;


namespace RFD.FMS.WEB.test
{
    public partial class WebForm1  :BasePage
    {
        private RFD.FMS.DAL.FinancialManage.WaybillDao waybillDao = new RFD.FMS.DAL.FinancialManage.WaybillDao();
        private  DataTable dt =new DataTable(); 
        private  Mail mail = new Mail();
        private RFD.FMS.DAL.Oracle.FinancialManage.SortingTransferDetailDao stddao = new RFD.FMS.DAL.Oracle.FinancialManage.SortingTransferDetailDao();
        private string Tips;
        private DateTime bTime;
        private int InsertCount = 0;
        private int UpdateCount = 0;

        System.Timers.Timer _timeEffect;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                _timeEffect = new System.Timers.Timer { Interval = 1000 };
                _timeEffect.Elapsed += EffectTimerElapsed;
                _timeEffect.AutoReset = true;
                _timeEffect.Enabled = true;
            }
          
        }

        protected void GoBtn_Click(object sender, EventArgs e)
        {
            
            string minID = minIDtxt.Text.Trim();
            string maxID = maxIDtxt.Text.Trim();
            int result =waybillDao.DeleteData(minID, maxID);
            LabelTips.Text += "执行完成" + "删除数据" + result + "行";

        }

       

      private  void Deal(DataTable dt)
       {
           int ALLcount = dt.Rows.Count;
           int Times = ALLcount/400;
           int Last = ALLcount%400;
           int times = 0;
           DateTime t1 = DateTime.Now;
           for ( times = 0; times < Times; times++)
           {
                DealForOnce(dt, times);
               Thread.Sleep(2000);
           }
           DealForLast(dt,Last,Times);
           DateTime t2 = DateTime.Now;
           TimeSpan timeSpan = t2.Subtract(t1);

           LabelTips.Text += "执行完成！用时" + timeSpan.TotalSeconds + "秒，添加了数据" + InsertCount + "条，更新了数据" + UpdateCount + "条";

       }
      private void DealForLast (DataTable dt, int Last, int Times)
      {
          for (int i=Times*400;i<Times*400+Last; i++)
          {
              long No = DataConvert.ToLong(dt.Rows[i]["WaybillNo"]);
              string Status = DataConvert.ToString(dt.Rows[i]["Status"]);
              int SubStatus = DataConvert.ToInt(dt.Rows[i]["SubStatus"]);
              DoAction(new WaybillStatusChangeLog { WaybillNo = No, Status = Status, SubStatus = SubStatus });
          }
      }
      private  void DealForOnce (DataTable dt , int times)
      {
          for (int i =0+times*400;i<400+times*400;i++)
          {
              long No = DataConvert.ToLong(dt.Rows[i]["WaybillNo"]);
              string Status = DataConvert.ToString(dt.Rows[i]["Status"]);
              int SubStatus = DataConvert.ToInt(dt.Rows[i]["SubStatus"]);
              DoAction(new WaybillStatusChangeLog {WaybillNo = No, Status = Status, SubStatus = SubStatus});
          }
      }


        public bool DoAction(WaybillStatusChangeLog model)
        {
            try
            {
                if (model.Status != "-5" && model.Status!="-4" && model.Status !="4" && model.Status !="-9")
                {
                    InsertIntoSortingTransferDetail(model);
                }

                return true;
            }
            catch (Exception ex)
            {
                mail.SendMailToUser("运单状态推送服务异常", "运单号：" + model.WaybillNo + "\t\n插入失败" + ex.Message + ex.StackTrace, "xueyi@vancl.cn;zengwei@vancl.cn;");

                return false;
            }
        }


        public int InsertIntoSortingTransferDetail(WaybillStatusChangeLog model)
        {

           
            try
            {

                if (!stddao.ExistFMS_SortingTransferDetailByNo(model.WaybillNo))
                {

                    var fmsmodel = GetAllSortingDetail(model.WaybillNo);
                    fmsmodel.IntoType = 0;
                    fmsmodel.OutType = 0;
                    fmsmodel.IsAccount = 0;
                    fmsmodel.IsDelete = false;
                    IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                    fmsmodel.DetailKID = iDGenerate.ServiceNewId("FMS_SortingTransferDetail", "DetailKid");
                    stddao.Add(fmsmodel);
                    InsertCount++;
                }


                if (model.Status == "-1")
                {
                    var fmsmodel = GetMerchantToSortingCenter(model.WaybillNo);
                    if (fmsmodel != null)
                    {
                        string DetailKid = stddao.ExsitInSorting(fmsmodel);
                        if(string.IsNullOrEmpty(DetailKid))
                        {
                            fmsmodel.IsAccount = 0;
                            fmsmodel.IsDelete = false;
                            fmsmodel.IntoType = 0;
                            fmsmodel.OutType = 0;
                            stddao.Add(fmsmodel);
                        }
                        else
                        {
                            fmsmodel.DetailKID = DetailKid;
                            stddao.UpdateFMS_MerchantToSortingCenter(fmsmodel);
                            UpdateCount++;
                        }
                        
                    }
                    else
                    {
                        Tips = "订单" + model.WaybillNo + "在入库状态没有抓取到数据,时间" + DateTime.Now;
                        mail.SendMailToUser("运单状态推送服务异常", Tips, "xueyi@vancl.cn;zengwei@vancl.cn;");
                    }
                }

                if (model.Status == "1")
                {
                    var fmsmodel = GetSortingToStation(model.WaybillNo);

                    if (fmsmodel != null)
                    {
                        foreach (var fmodel in fmsmodel)
                        {
                            string DetailKid = stddao.ExistIntoStation(fmodel);

                            if (string.IsNullOrEmpty(DetailKid))
                            {
                                fmodel.IntoType = 1;
                                fmodel.OutType = 0;
                                fmodel.IsAccount = 0;
                                fmodel.IsDelete = false;
                                IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                                fmodel.DetailKID = iDGenerate.ServiceNewId("FMS_SortingTransferDetail", "DetailKid");
                                stddao.Add(fmodel);
                                InsertCount++;
                            }
                            else
                            {
                                
                                fmodel.DetailKID = DetailKid;

                                stddao.UpdateFMS_SortingToStation(fmodel);
                                UpdateCount++;
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
                            string DetailKid = stddao.ExistOutBound(fmodel);
                            if (string.IsNullOrEmpty(DetailKid))
                            {
                                fmodel.IntoType = 0;
                                fmodel.OutType = 1;
                                fmodel.IsAccount = 0;
                                fmodel.IsDelete = false;
                                IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                                fmodel.DetailKID = iDGenerate.ServiceNewId("FMS_SortingTransferDetail", "DetailKid");
                                stddao.Add(fmodel);
                                InsertCount++;
                            }
                            else
                            {
                                fmodel.OutType = 1;
                                fmodel.DetailKID = DetailKid;
                                stddao.UpdateFMS_SortingToCity(fmodel);
                                UpdateCount++;
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

                if (model.SubStatus == 6 || model.SubStatus == 7)
                {
                    var fmsmodel = GetReturnToSortingCenter(model.WaybillNo);
                     


                    
                    
                    if (fmsmodel != null)
                    {
                        var dt = stddao.GetTableByNo(model.WaybillNo);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataRow[] rows = dt.Select("", "CreateTime DESC");
                            fmsmodel.DetailKID = rows[0]["DetailKid"].ToString();
                            stddao.UpdateFMS_ReturnToSortingCenter(fmsmodel);
                            UpdateCount++;
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
          
            var dt = waybillDao.GetAllSortingData(waybillno);
            if (dt != null)
            {
                var Model = new FMS_SortingTransferDetail();
                Model.WaybillNO = waybillno;
                Model.WaybillType = dt.Rows[0]["WaybillType"].ToString();
                Model.MerchantID = DataConvert.ToInt(dt.Rows[0]["MerchantID"]);
                Model.DistributionCode = dt.Rows[0]["DistributionCode"].ToString();
                Model.TopCODCompanyID = DataConvert.ToInt(dt.Rows[0]["TopCODCompanyID"]);
                Model.IsAccount = 0;
                Model.DeliverStationID = DataConvert.ToInt(dt.Rows[0]["DeliverStationID"]);
                Model.CreateTime = DateTime.Now;
                Model.UpdateTime = DateTime.Now;
                Model.IsDelete = false;
                Model.OutType = 0;
                Model.IntoType = 0;
                return Model;
            }
            return new FMS_SortingTransferDetail()
            {
                IsAccount = 0,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsDelete = false,
                WaybillNO = waybillno,
                OutType = 0,
                IntoType = 0,
                MerchantID = 0
            };
        }

        private List<FMS_SortingTransferDetail> GetSortingToStation(Int64 waybillno)
        {
          
            var dt = waybillDao.GetSortingToStation(waybillno);
            List<FMS_SortingTransferDetail> list = new List<FMS_SortingTransferDetail>();
            if (dt != null)
            { 
                for(int i=0;i<dt.Rows.Count;i++)
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
                    list.Add(Model);
                }

                return list;
            }
            return null;
        }


        private FMS_SortingTransferDetail GetMerchantToSortingCenter(Int64 waybillno)
        {
           
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
                    Model.CreateCityID = dt.Rows[i]["CityID"].IsNullData()?  null : dt.Rows[i]["CityID"].ToString();
                    Model.DistributionCode = dt.Rows[i]["DistributionCode"].ToString();
                    Model.TopCODCompanyID = DataConvert.ToInt(dt.Rows[i]["TopCODCompanyID"]);
                    Model.OutType = 1;
                    Model.DeliverStationID = DataConvert.ToInt(dt.Rows[0]["DeliverStationID"]);
                    list.Add(Model);
                }

                return list;
            }
            return null;
        }

        private FMS_SortingTransferDetail GetReturnToSortingCenter(long waybillno)
        {
          
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

        protected void ModifyBtnByNo_Click(object sender, EventArgs e)
        {
            dt.Clear();
            dt = waybillDao.GetWaybillStatus(WaybillNotxt.Text.Trim());
            for(int i=0; i<dt.Rows.Count;i++)
            {
                long No = DataConvert.ToLong(dt.Rows[i]["WaybillNo"]);
                string Status = DataConvert.ToString(dt.Rows[i]["Status"]);
                int SubStatus = DataConvert.ToInt(dt.Rows[i]["SubStatus"]);
                DoAction(new WaybillStatusChangeLog {Status = Status, SubStatus = SubStatus, WaybillNo = No});
            }
            LabelTips.Text = "执行了"+dt.Rows.Count+"过程";

        }

        protected void GotoBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBeginTime.Text) || string.IsNullOrEmpty(txtEndTime.Text))
            {
                Alert("请设置时间！");
                return;
            }
            dt.Clear();
            InsertCount = 0;
            UpdateCount = 0;
            DateTime sTime = Convert.ToDateTime(txtBeginTime.Text.Trim());
            DateTime eTime = Convert.ToDateTime(txtEndTime.Text.Trim());
            dt = waybillDao.GetWaybillStatus(sTime, eTime);
            LabelTips.Text = "获得数据" + dt.Rows.Count + "行\n"
                               +"添加数据";

            Deal(dt);
        }

        protected void VanclCODBtn_Click(object sender, EventArgs e)
        {
           

        }

        protected void MerchantDeliverFeeBtn_Click(object sender, EventArgs e)
        {
            WaybillForIncomeBaseInfoService target = new WaybillForIncomeBaseInfoService(); // TODO: 初始化为适当的值
            int rowCount = 10; // TODO: 初始化为适当的值
            target.DisposeEffect(rowCount);
            Alert("执行完成！");
        }

        protected void MerchantDeliverFeeModBtn_Click(object sender, EventArgs e)
        {
            IWaybillForIncomeBaseInfoService IncomeBaseService =
                ServiceLocator.GetService<IWaybillForIncomeBaseInfoService>();
            try
            {
                IncomeBaseService.UpdateIncomeFeeInfoDao(5);
            }
            catch (Exception ex)
            {
                Alert(ex.Message + "\n" + ex);
            }
        }

        protected void SortingFeeBtn_Click(object sender, EventArgs e)
        {
            ISortingFeeService srv = ServiceLocator.GetService<ISortingFeeService>();

            try
            {
                  srv.Dispposed(50);
            }
            catch (Exception ex)
            {

                Alert(ex.Message);
            }
            
        }

        protected void TimerTestBtn_Click(object sender, EventArgs e)
        {
           
            _timeEffect = new System.Timers.Timer { Interval = 1000 };
            _timeEffect.Elapsed += EffectTimerElapsed;
            _timeEffect.AutoReset = true;
            _timeEffect.Enabled = true;
        }

          protected void EffectTimerElapsed(object sender, ElapsedEventArgs e)
          {
              Timerlabel.Text= "a:"+DateTime.Now.Second.ToString();
          }
    }


     

    public class WaybillStatusChangeLog
    {
        public long WaybillNo { get; set; }
        public string Status { get; set; }
        public int SubStatus { get; set;}

    }
}