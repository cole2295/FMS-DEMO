using System;
using System.Data;
using System.IO;
using System.Threading;
using Quartz;
using RFD.FMS.Service.QueryStatistics;
using RFD.FMS.Util;
using RFD.FMS.MODEL.QueryStatistics;
using System.Collections.Generic;
using ServiceForWmsVsFMSDailyCheck.ServiceClient;
using System.Text;
using RFD.FMS.Service;
using RFD.FMS.Service.Mail;
using System.Windows.Forms;
namespace ServiceForWmsVsFMSDailyCheck
{
    public class JobForWmsVsFMSDailyCheck : IStatefulJob
    {
        public void Execute(JobExecutionContext context)
        {
            
            DateTime sTime = Common.GetstartTime ;
            DateTime eTime = Common.GetendTime;
            //测试
           // CODClient client = new CODClient();
            //sTime = Convert.ToDateTime(sTime.ToString("yyyy-MM-dd HH:mm:ss"));
            //eTime = Convert.ToDateTime(eTime.ToString("yyyy-MM-dd HH:mm:ss"));
            //CODParameter CodPar = new CODParameter();
            //CodPar.DateChecked = true;
            //CodPar.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
            //CodPar.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
            //CodPar.ExpressId = 223;

            //CODDaily[] delivery = client.GetCODDeliveryDaily(CodPar);
            //var allreturns = client.GetCODReturnsDaily(CodPar);
            
            //CODRETURNS[] Codreturns = client.GetCODReturns(CodPar);
            //foreach (var returnDetail in Codreturns)
            //{
            //   // Common.WriteTest("id="+returnDetail._expressid.ToString()+"公司名称"+returnDetail._express);
            //    Common.WriteTest("订单号："+returnDetail._orderno + "凡客配送公司ID：" + returnDetail._expressid + "vancl金额：" + returnDetail._receivable);
            //}

            //ICODWaybillFeeCheckService service = ServiceLocator.GetService<ICODWaybillFeeCheckService>();
            //IDictionary<int, FMSCODDetails> fmsdicValues = service.GetCODReturnsDailyByStationID(DataConvert.ToDayBegin(sTime), DataConvert.ToDayEnd(eTime), 6);
            //Common.WriteTest("fanke 11 rfd");
            //foreach (var vanclcodDetailse in delivery)
            //{
            //    Common.WriteTest("凡客配送公司ID：" + vanclcodDetailse._expressid + "vancl金额：" + vanclcodDetailse._receivable);
            //}

            //foreach (var fmsdic in fmsdicValues)
            //{
            //    Common.WriteTest("rfd单号：" + fmsdic.Value.WaybillNo + "rfd金额：" + fmsdic.Value.Fee);
            //}


            //var list = VisitDetailsCheck(visits, fmsdicValues);
            //foreach (var returnVancl in returns)
            //{
            //    Common.WriteTest("凡客站点：" +returnVancl._expressid  + "vancl金额：" + returnVancl._receivable);
            //}

            //foreach (var fmsCodDaily in dicValues)
            //{
            //    Common.WriteTest("如风达站点："+fmsCodDaily.Value.CompanyId +"如风达金额："+fmsCodDaily.Value.AllFee);
            //}

            //List<FMSCodDaily> vanclList = new List<FMSCodDaily>();
            //foreach (var returnVancl in returns)
            //{
            //    vanclList.Add(new FMSCodDaily(){AllFee = DataConvert.ToDecimal(returnVancl._receivable),
            //                                    CompanyId = DataConvert.ToInt(returnVancl._expressid),
            //                                    IsChecked = false});
            //}
            //foreach (var listvancl in vanclList)
            //{
            //    foreach (var fmsCodDaily in dicValues)
            //    {
            //        if(listvancl.CompanyId == fmsCodDaily.Value.CompanyId && listvancl.AllFee == fmsCodDaily.Value.AllFee)
            //        {
            //            listvancl.IsChecked = true;
            //            fmsCodDaily.Value.IsChecked = true;
            //        }
            //    }
            //}
            //Common.WriteTest("差异对比情况");
            //foreach (var vanclCodDaily in vanclList)
            //{
            //    if(vanclCodDaily.IsChecked == false)
            //        Common.WriteTest("凡客站点："+vanclCodDaily.CompanyId + "凡客金额："+vanclCodDaily.AllFee);
            //}

            //foreach (var fmsCodDaily in dicValues)
            //{
            //    if (fmsCodDaily.Value.IsChecked == false)
            //        Common.WriteTest("如风达站点：" + fmsCodDaily.Value.CompanyId + "凡客金额：" + fmsCodDaily.Value.AllFee);
            //}
/////////////////////////////////////////////////////
           DailyCheck(sTime,eTime);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deliverys"></param>
        /// <param name="dicValues"></param>
        /// <returns></returns>
        private IList<CodDailyModel> DeliveryDetailsCheck(CODDELIVERY[] deliverys,IDictionary<int,FMSCODDetails>dicValues)
        {
           IList<CodDailyModel> list =new List<CodDailyModel>();
            int flag =0;
            if (deliverys.Length >= dicValues.Count)
            {
                for (int i = 0; i < deliverys.Length; i++)
                {
                    flag = 0;
                    for (int j = 0; j < dicValues.Count; j++)
                    {
                        if (dicValues[j].WaybillNo.ToString() == deliverys[i]._orderno)
                        {
                            flag = 1;
                            if (dicValues[j].Fee != deliverys[i]._receivable)
                            {
                                CodDailyModel model = new CodDailyModel();
                                model.CompanyName = dicValues[i].CompanyName;
                                model.OrderNo = dicValues[j].WaybillNo;
                                model.RFDFee = dicValues[j].Fee;
                                model.VanclFee = DataConvert.ToDecimal(deliverys[i]._receivable);
                                model.Remark = "此单金额有差异";
                                list.Add(model);
                            }
                        }
                    }

                    if (flag == 1)
                    {
                        flag = 0;
                    }
                    else
                    {
                        CodDailyModel model = new CodDailyModel();
                        model.CompanyName=deliverys[i]._express;
                        model.OrderNo = DataConvert.ToLong(deliverys[i]._orderno);
                        model.RFDFee = DataConvert.ToDecimal(0);
                        model.VanclFee = DataConvert.ToDecimal(deliverys[i]._receivable);
                        model.Remark = "如风达查无此单";
                        list.Add(model);
                    }
                }
            }
            else
            {
                for (int i = 0; i < dicValues.Count; i++)
                {
                    flag = 0;
                    for (int j = 0; j < deliverys.Length; j++)
                    {
                        if (dicValues[i].WaybillNo.ToString() == deliverys[j]._orderno)
                        {
                            flag = 1;
                            if (dicValues[i].Fee != deliverys[j]._receivable)
                            {
                                CodDailyModel model = new CodDailyModel();
                                model.CompanyName = dicValues[i].CompanyName;
                                model.OrderNo = dicValues[i].WaybillNo;
                                model.RFDFee = dicValues[i].Fee;
                                model.VanclFee = DataConvert.ToDecimal(deliverys[j]._receivable);
                                model.Remark = "此单金额有差异";
                                list.Add(model);
                            }
                        }
                    }

                    if (flag == 1)
                    {
                        flag = 0;
                    }
                    else
                    {
                        CodDailyModel model = new CodDailyModel();
                        model.CompanyName = dicValues[i].CompanyName;
                        model.OrderNo = DataConvert.ToLong(dicValues[i].WaybillNo);
                        model.RFDFee = dicValues[i].Fee;
                        model.VanclFee = DataConvert.ToDecimal(0);
                        model.Remark = "凡客查无此单";
                        list.Add(model);
                    }
                }
            }
           
                return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sTime"></param>
        /// <param name="eTime"></param>

        private IList<CodDailyModel> ReturnsDetailsCheck(CODRETURNS [] returns, IDictionary<int, FMSCODDetails> dicValues)
        {
            IList<CodDailyModel> list = new List<CodDailyModel>();
            int flag = 0;
            if (returns.Length >= dicValues.Count)
            {
                for (int i = 0; i < returns.Length; i++)
                {
                    flag = 0;
                    for (int j = 0; j < dicValues.Count; j++)
                    {
                        if (dicValues[j].WaybillNo.ToString() == returns[i]._orderno)
                        {
                            flag = 1;
                            if (dicValues[j].Fee != returns[i]._receivable)
                            {
                                CodDailyModel model = new CodDailyModel();
                                model.CompanyName = dicValues[i].CompanyName;
                                model.OrderNo = dicValues[j].WaybillNo;
                                model.RFDFee = dicValues[j].Fee;
                                model.VanclFee = DataConvert.ToDecimal(returns[i]._receivable);
                                model.Remark = "此单金额有差异";
                                list.Add(model);
                            }
                        }
                    }

                    if (flag == 1)
                    {
                        flag = 0;
                    }
                    else
                    {
                        CodDailyModel model = new CodDailyModel();
                        model.CompanyName = returns[i]._express;
                        model.OrderNo = DataConvert.ToLong(returns[i]._orderno);
                        model.RFDFee = DataConvert.ToDecimal(0);
                        model.VanclFee = DataConvert.ToDecimal(returns[i]._receivable);
                        model.Remark="如风达查无此单";
                        list.Add(model);
                    }
                }
            }
            else
            {
                for (int i = 0; i < dicValues.Count; i++)
                {
                    flag = 0;
                    for (int j = 0; j < returns.Length; j++)
                    {
                        if (dicValues[i].WaybillNo.ToString() == returns[j]._orderno)
                        {
                            flag = 1;
                            if (dicValues[i].Fee != returns[j]._receivable)
                            {
                                CodDailyModel model = new CodDailyModel();
                                model.CompanyName = dicValues[i].CompanyName;
                                model.OrderNo = dicValues[i].WaybillNo;
                                model.RFDFee = dicValues[i].Fee;
                                model.VanclFee = DataConvert.ToDecimal(returns[j]._receivable);
                                model.Remark="此单金额有差异";
                                list.Add(model);
                            }
                        }
                    }

                    if (flag == 1)
                    {
                        flag = 0;
                    }
                    else
                    {
                        CodDailyModel model = new CodDailyModel();
                        model.CompanyName = dicValues[i].CompanyName;
                        model.OrderNo = DataConvert.ToLong(dicValues[i].WaybillNo);
                        model.RFDFee = dicValues[i].Fee;
                        model.VanclFee = DataConvert.ToDecimal(0);
                        model.Remark = "凡客查无此单";
                        list.Add(model);
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sTime"></param>
        /// <param name="eTime"></param>

        private IList<CodDailyModel> VisitDetailsCheck(CODVISIT[] visits, IDictionary<int, FMSCODDetails> dicValues)
        {
            IList<CodDailyModel> list = new List<CodDailyModel>();
            int flag = 0;
            if (visits.Length >= dicValues.Count)
            {
                for (int i = 0; i < visits.Length; i++)
                {
                    flag = 0;
                    for (int j = 0; j < dicValues.Count; j++)
                    {
                        if (dicValues[j].WaybillNo.ToString() == visits[i]._orderno)
                        {
                            flag = 1;
                            if (dicValues[j].Fee != visits[i]._shouldrefund)
                            {
                                CodDailyModel model = new CodDailyModel();
                                model.CompanyName = dicValues[i].CompanyName;
                                model.OrderNo = dicValues[j].WaybillNo;
                                model.RFDFee = dicValues[j].Fee;
                                model.VanclFee = DataConvert.ToDecimal(visits[i]._shouldrefund);
                                model.Remark = "此单金额有差异";
                                list.Add(model);
                            }
                        }
                    }

                    if (flag == 0)
                    {
                        CodDailyModel model = new CodDailyModel();
                        model.CompanyName = visits[i]._express;
                        model.OrderNo = DataConvert.ToLong(visits[i]._orderno);
                        model.RFDFee = DataConvert.ToDecimal(0);
                        model.VanclFee = DataConvert.ToDecimal(visits[i]._shouldrefund);
                        model.Remark = "如风达查无此单";
                        list.Add(model);
                    }
                }
            }
            else
            {
                for (int i = 0; i < dicValues.Count; i++)
                {
                    flag = 0;
                    for (int j = 0; j < visits.Length; j++)
                    {
                        if (dicValues[i].WaybillNo.ToString() == visits[j]._orderno)
                        {
                            flag = 1;
                            if (dicValues[i].Fee != visits[j]._shouldrefund)
                            {
                                CodDailyModel model = new CodDailyModel();
                                model.CompanyName = dicValues[i].CompanyName;
                                model.OrderNo = dicValues[i].WaybillNo;
                                model.RFDFee = dicValues[i].Fee;
                                model.VanclFee = DataConvert.ToDecimal(visits[j]._shouldrefund);
                                model.Remark = "此单金额有差异";
                                list.Add(model);
                            }
                        }
                    }

                    if (flag == 1)
                    {
                        flag = 0;
                    }
                    else
                    {
                        CodDailyModel model = new CodDailyModel();
                        model.CompanyName = dicValues[i].CompanyName;
                        model.OrderNo = DataConvert.ToLong(dicValues[i].WaybillNo);
                        model.RFDFee = dicValues[i].Fee;
                        model.VanclFee = DataConvert.ToDecimal(0);
                        model.Remark = "凡客查无此单";
                        list.Add(model);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sTime"></param>
        /// <param name="eTime"></param>
        private  void DailyCheck(DateTime sTime,DateTime eTime)
        {
            try
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("</br>");
                builder.Append("<div>");
                builder.Append("统计时间：");
                if (sTime.ToShortDateString() == eTime.ToShortDateString())
                    builder.Append(sTime.ToShortDateString());
                else
                    builder.Append(sTime.ToShortDateString() + "到" + eTime.ToShortDateString());
                builder.Append("</div>");
                builder.Append("</br>");
                builder.Append("<div align=\"center\">");
                List<CodDailyModel> deliveryDetails = new List<CodDailyModel>();
                IList<CodDailyModel> deliveryDaily = CheckCODDeliveryDaily(sTime,eTime,ref deliveryDetails);
                deliveryDetails.Sort();
                List<CodDailyModel> returnsDetails = new List<CodDailyModel>();
                IList<CodDailyModel> returnsDaily = CheckCODReturnsDaily(sTime,eTime,ref returnsDetails);
                returnsDetails.Sort();
                List<CodDailyModel> visitDetails = new List<CodDailyModel>();
                IList<CodDailyModel> visitDaily = CheckCODVisitDaily(sTime,eTime,ref visitDetails);
                visitDetails.Sort();

                string deliveryDailyString = CreateTable("发货情况对比", deliveryDaily);
                string deliveryDetailsString =CreateDetailsTable("发货详情差异",deliveryDetails);
                string returnsDailyString = CreateTable("退货情况对比", returnsDaily);
                string returnsDetailsString =  CreateDetailsTable("退货详情差异", returnsDetails);
                string visitDailyString = CreateTable("上门退情况对比", visitDaily);
                string visitDetailsString =  CreateDetailsTable("上门退详情差异", visitDetails);
                builder.Append("<table>");

                decimal vanclDeliveryFee = 0;
                decimal rfdDeliveryFee = 0;

                foreach (var item in deliveryDaily)
                {
                    vanclDeliveryFee += item.VanclFee;
                    rfdDeliveryFee += item.RFDFee;
                }

                builder.Append("<tr>");
                builder.Append("<td>");
                builder.Append("凡客发货总金额：");
                builder.Append(vanclDeliveryFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append("如风达发货总金额：");
                builder.Append(rfdDeliveryFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append("差异金额：");
                builder.Append(vanclDeliveryFee - rfdDeliveryFee);
                builder.Append("</td>");
                builder.Append("</tr>");

                decimal vanclReturnsFee = 0;
                decimal rfdReturnsFee = 0;

                foreach (var item in returnsDaily)
                {
                    vanclReturnsFee += item.VanclFee;
                    rfdReturnsFee += item.RFDFee;
                }

                builder.Append("<tr>");
                builder.Append("<td>");
                builder.Append("凡客退货总金额：");
                builder.Append(vanclReturnsFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append("如风达退货总金额：");
                builder.Append(rfdReturnsFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append("差异金额：");
                builder.Append(vanclReturnsFee - rfdReturnsFee);
                builder.Append("</td>");
                builder.Append("</tr>");

                decimal vanclVisitFee = 0;
                decimal rfdVisitFee = 0;

                foreach (var item in visitDaily)
                {
                    vanclVisitFee += item.VanclFee;
                    rfdVisitFee += item.RFDFee;
                }

                builder.Append("<tr>");
                builder.Append("<td>");
                builder.Append("凡客上门退总金额：");
                builder.Append(vanclVisitFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append("如风上门退总金额：");
                builder.Append(rfdVisitFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append("差异金额：");
                builder.Append(vanclVisitFee - rfdVisitFee);
                builder.Append("</td>");
                builder.Append("</tr>");

                builder.Append("</table>");

                builder.Append("</br>");
                StringBuilder Appendixbuilder = new StringBuilder();
                Appendixbuilder.Append("<html>  <head> <title>凡客如风达收款核对明细 </title>  </head>");
                Appendixbuilder.Append("<body> ");
                Appendixbuilder.Append("<table> ");
                Appendixbuilder.Append("<tr>");
                Appendixbuilder.Append("<td align=\"center\" valign=\"top\">");
                Appendixbuilder.Append(deliveryDetailsString);
                Appendixbuilder.Append("</td>");
                Appendixbuilder.Append("<td align=\"center\" valign=\"top\">");
                Appendixbuilder.Append(returnsDetailsString);
                Appendixbuilder.Append("</td>");
                Appendixbuilder.Append("<td align=\"center\" valign=\"top\">");
                Appendixbuilder.Append(visitDetailsString);
                Appendixbuilder.Append("</td>");
                Appendixbuilder.Append("</tr>");
                Appendixbuilder.Append("</table>");

                Appendixbuilder.Append("<table >");
                Appendixbuilder.Append("<tr>");
                Appendixbuilder.Append("<td align=\"center\" valign=\"top\">");
                Appendixbuilder.Append(deliveryDailyString);
                Appendixbuilder.Append("</td>");
                Appendixbuilder.Append("<td> </td>");
                Appendixbuilder.Append("<td align=\"center\" valign=\"top\">");
                Appendixbuilder.Append(returnsDailyString);
                Appendixbuilder.Append("</td>");
                Appendixbuilder.Append("<td> </td>");
                Appendixbuilder.Append("<td align=\"center\" valign=\"top\">");
                Appendixbuilder.Append(visitDailyString);
                Appendixbuilder.Append("</td>");
                Appendixbuilder.Append("</tr>");

                Appendixbuilder.Append("</table>");

                Appendixbuilder.Append("</div>");
                Appendixbuilder.Append("</body> </html>");
                
                Common.WriteTest(DateTime.Now.ToString() +": 生成附件前");
                string path = Common.WriteHtml(Appendixbuilder.ToString());
                Common.WriteTest(DateTime.Now.ToString()+":生成附件，路径："+path);
                List<string> pathes = new List<string>();
                pathes.Add(path);
                Common.SendMailByAttachment( "如风达凡客收款核对", builder.ToString(),pathes);
                //Common.SendMail("如风达凡客收款核对", builder.ToString());
            }
            catch (Exception ex)
            {
                Common.WriteTest(DateTime.Now.ToString()+":进入异常");
                Common.SendFailedMail("如风达凡客收款核对异常", ex.Message);
            }
           
        }

        
        private IList<CodDailyModel> CheckCODDeliveryDaily(DateTime sTime, DateTime eTime, ref List<CodDailyModel> DeliveryDetailsByCompany)
        {
            ICODWaybillFeeCheckService service = ServiceLocator.GetService<ICODWaybillFeeCheckService>();


            
            IDictionary<int, FMSCodDaily> dicValues = service.GetCODDeliveryDaily(DataConvert.ToDayBegin(sTime), DataConvert.ToDayEnd(eTime));

            CODClient client = new CODClient();

            
            CODParameter parameter = new CODParameter();

            parameter.DateChecked = true;
            parameter.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
            parameter.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
            
            CODDaily[] dailys = client.GetCODDeliveryDaily(parameter);


            FMSCodDaily fmsDaily = null;
            CODDaily vanclDaily = null;
            CodDailyModel model = null;
            IList<CodDailyModel> models = new List<CodDailyModel>();

            for (int i = 0; i < dailys.Length; i++)
            {
                vanclDaily = dailys[i];
               
                if (dicValues.ContainsKey(vanclDaily._expressid.Value) == false)
                {
                    if (!Common.ExistInCancelStation(vanclDaily._expressid.Value.ToString()))
                    {
                        if (!Common.ExistInCancelStationEx(vanclDaily._expressid.Value.ToString()))
                        {
                            
                            parameter.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                            CODDELIVERY[] coddeliverys = client.GetCODDelivery(parameter);
                            foreach (var coddelivery in coddeliverys)
                            {
                                var appendItem = new CodDailyModel();
                                appendItem.OrderNo = DataConvert.ToLong(coddelivery._orderno);
                                appendItem.CompanyName = coddelivery._express;
                                appendItem.RFDFee = 0;
                                appendItem.VanclFee = DataConvert.ToDecimal(coddelivery._receivable);
                                appendItem.Remark = "如风达查无此单*";
                                DeliveryDetailsByCompany.Add(appendItem);//
                            }
                        }
                        model = new CodDailyModel();
                        model.CompanyName = vanclDaily._express;
                        model.RFDFee = 0;
                        model.VanclFee = vanclDaily._receivable.Value;
                        models.Add(model);

                        CODParameter codParameter = new CODParameter();
                        codParameter.DateChecked = true;
                        codParameter.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
                        codParameter.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
                        codParameter.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                        CODDELIVERY[] Vdeliverys = client.GetCODDelivery(codParameter);
                        model.RFDCount = 0;
                        model.VanclCount = Vdeliverys.Length;
                        models.Add(model);
                    }
                    continue;
                }
                
                fmsDaily = dicValues[vanclDaily._expressid.Value];

                model = new CodDailyModel();

                model.CompanyName = fmsDaily.CompanyName;
                model.RFDFee = fmsDaily.AllFee;
                model.VanclFee = vanclDaily._receivable.Value;
                
                CODParameter CodPar = new CODParameter();
                CodPar.DateChecked = true;
                CodPar.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
                CodPar.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
                CodPar.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                CODDELIVERY[] deliverys = client.GetCODDelivery(CodPar);
                IDictionary<int, FMSCODDetails> fmsdicValues = service.GetCODDeliveryDailyByStationID(DataConvert.ToDayBegin(sTime),
                                                                      DataConvert.ToDayEnd(eTime), fmsDaily.CompanyId);
                model.RFDCount = fmsdicValues.Keys.Count;
                model.VanclCount = deliverys.Length;


                if (model.RFDFee != model.VanclFee)
                {
                    var list = DeliveryDetailsCheck(deliverys, fmsdicValues);
                    foreach (var item in list)
                    DeliveryDetailsByCompany.Add(item);
                }
                dicValues[vanclDaily._expressid.Value].IsChecked = true;
                models.Add(model);
            }
           
            
            foreach (var delivery in dicValues)
            {
                if (!delivery.Value.IsChecked)
                {
                    var appendModel = new CodDailyModel();
                    appendModel.CompanyName = delivery.Value.CompanyName;
                    appendModel.RFDFee = delivery.Value.AllFee;
                    appendModel.VanclFee = 0;
                    appendModel.VanclCount = 0;
                    //models.Add(appendModel);

                    IDictionary<int, FMSCODDetails> fmsappendValues = service.GetCODDeliveryDailyByStationID(DataConvert.ToDayBegin(sTime),
                                                                              DataConvert.ToDayEnd(eTime), delivery.Value.CompanyId);
                    appendModel.RFDCount = fmsappendValues.Keys.Count;
                     models.Add(appendModel);
                    foreach (var fmsappendValue in fmsappendValues)
                    {
                        var appendItem = new CodDailyModel();
                        appendItem.OrderNo = fmsappendValue.Value.WaybillNo;
                        appendItem.CompanyName = fmsappendValue.Value.CompanyName;
                        appendItem.RFDFee = fmsappendValue.Value.Fee;
                        appendItem.VanclFee = 0;
                        appendItem.Remark = "查此单凡客不在该配送公司";
                        DeliveryDetailsByCompany.Add(appendItem);
                    }
                }
            }
            return models;
        }

        private IList<CodDailyModel> CheckCODReturnsDaily(DateTime sTime,DateTime eTime,ref List<CodDailyModel>ReturnsDetailsByCompany)
        {
            ICODWaybillFeeCheckService service = ServiceLocator.GetService<ICODWaybillFeeCheckService>();

            IDictionary<int, FMSCodDaily> dicValues = service.GetCODReturnsDaily(DataConvert.ToDayBegin(sTime), DataConvert.ToDayEnd(eTime));

            CODClient client = new CODClient();

            CODParameter parameter = new CODParameter();

            parameter.DateChecked = true;
            parameter.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
            parameter.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
            CODDaily[] dailys = client.GetCODReturnsDaily(parameter);
            
            

            FMSCodDaily fmsDaily = null;
            CODDaily vanclDaily = null;
            CodDailyModel model = null;
            IList<CodDailyModel> models = new List<CodDailyModel>();

            for (int i = 0; i < dailys.Length; i++)
            {
                vanclDaily = dailys[i];

                if (dicValues.ContainsKey(vanclDaily._expressid.Value) == false)
                {
                    if (!Common.ExistInCancelStation(vanclDaily._expressid.Value.ToString()))
                    {
                        if (!Common.ExistInCancelStationEx(vanclDaily._expressid.Value.ToString()))
                        {
                            parameter.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                            CODRETURNS[] Codreturns = client.GetCODReturns(parameter);
                            foreach (var codreturn in Codreturns)
                            {
                                var appendItem = new CodDailyModel();
                                appendItem.OrderNo = DataConvert.ToLong(codreturn._orderno);
                                appendItem.CompanyName = codreturn._express;
                                appendItem.RFDFee = 0;
                                appendItem.VanclFee = DataConvert.ToDecimal(codreturn._receivable);
                                appendItem.Remark = "如风达查无此单*";
                                ReturnsDetailsByCompany.Add(appendItem);
                            }
                        }
                        
                        model = new CodDailyModel();
                        model.CompanyName = vanclDaily._express;
                        model.RFDFee = 0;
                        model.VanclFee = vanclDaily._receivable.Value;
                        CODParameter codParameter = new CODParameter();
                        codParameter.DateChecked = true;
                        codParameter.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
                        codParameter.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
                        codParameter.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                        CODRETURNS[] Vreturns = client.GetCODReturns(codParameter);
                       
                        model.RFDCount = 0;
                        model.VanclCount = Vreturns.Length;
                        models.Add(model);
                    }
                    continue;
                }

                fmsDaily = dicValues[vanclDaily._expressid.Value];

               

                model = new CodDailyModel();

                model.CompanyName = fmsDaily.CompanyName;
                model.RFDFee = fmsDaily.AllFee;
                model.VanclFee = vanclDaily._receivable.Value;

                CODParameter CodPar = new CODParameter();
                CodPar.DateChecked = true;
                CodPar.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
                CodPar.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
                CodPar.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                CODRETURNS[] returns = client.GetCODReturns(CodPar);
                IDictionary<int, FMSCODDetails> fmsdicValues = service.GetCODReturnsDailyByStationID(DataConvert.ToDayBegin(sTime),
                                                                      DataConvert.ToDayEnd(eTime), fmsDaily.CompanyId);
                model.RFDCount = fmsdicValues.Keys.Count;
                model.VanclCount = returns.Length;

                if (model.RFDFee != model.VanclFee)
                {
                   
                    var list = ReturnsDetailsCheck(returns, fmsdicValues);
                    foreach (var item in list)
                        ReturnsDetailsByCompany.Add(item);
                }
                dicValues[vanclDaily._expressid.Value].IsChecked = true;
                models.Add(model);
            }
            foreach (var returns in dicValues)
            {
                if(! returns.Value.IsChecked)
                {
                    var appendModel = new CodDailyModel();
                    appendModel.CompanyName = returns.Value.CompanyName;
                    appendModel.RFDFee = returns.Value.AllFee;
                    appendModel.VanclFee = 0;
                    appendModel.VanclCount = 0;
                    //models.Add(appendModel);
                    IDictionary<int, FMSCODDetails> fmsappendValues = service.GetCODReturnsDailyByStationID(DataConvert.ToDayBegin(sTime),
                                                                              DataConvert.ToDayEnd(eTime), returns.Value.CompanyId);
                    appendModel.RFDCount = fmsappendValues.Keys.Count;
                    
                    models.Add(appendModel);
                    foreach (var fmsappendValue in fmsappendValues)
                    {
                        var appendItem = new CodDailyModel();
                        appendItem.OrderNo = fmsappendValue.Value.WaybillNo;
                        appendItem.CompanyName = fmsappendValue.Value.CompanyName;
                        appendItem.RFDFee = fmsappendValue.Value.Fee;
                        appendItem.VanclFee = 0;
                        appendItem.Remark = "查此单凡客不在该配送公司";
                        ReturnsDetailsByCompany.Add(appendItem);
                    }
                }
            }
            return models;
        }

        private IList<CodDailyModel> CheckCODVisitDaily(DateTime sTime, DateTime eTime, ref List<CodDailyModel> VisitDetailsByCompany)
        {
            ICODWaybillFeeCheckService service = ServiceLocator.GetService<ICODWaybillFeeCheckService>();

            IDictionary<int, FMSCodDaily> dicValues = service.GetCODVisitDaily(DataConvert.ToDayBegin(sTime), DataConvert.ToDayEnd(eTime));

            CODClient client = new CODClient();

            CODParameter parameter = new CODParameter();

            parameter.DateChecked = true;
            parameter.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
            parameter.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;

            CODVISIT[] dailys = client.GetCODVisitDaily(parameter);
            //
            //Common.WriteTest("vancl退货订单");
            //foreach (var daily in dailys)
            //{
            //    Common.WriteTest("站点："+daily._expressid+"金额："+daily._shouldrefund+"单号："+daily._express);
            //}
            //Common.WriteTest("rfd退货订单");
            //{
            //    foreach (var fmsCodDaily in dicValues)
            //    {
            //        Common.WriteTest("站点：" + fmsCodDaily.Value.CompanyId + "金额：" + fmsCodDaily.Value.AllFee);
            //    }
            //}
            //
            FMSCodDaily fmsDaily = null;
            CODVISIT vanclDaily = null;
            CodDailyModel model = null;
            IList<CodDailyModel> models = new List<CodDailyModel>();

            for (int i = 0; i < dailys.Length; i++)
            {
                vanclDaily = dailys[i];

                if (dicValues.ContainsKey(vanclDaily._expressid.Value) == false)
                {
                    if (!Common.ExistInCancelStation(vanclDaily._expressid.Value.ToString()))
                    {
                        //
                        //Common.WriteTest("vancl有 rfd没的单子站点" + DataConvert.ToInt(vanclDaily._expressid.Value));
                        //

                        if (!Common.ExistInCancelStationEx(vanclDaily._expressid.Value.ToString()))
                        {
                            parameter.ExpressId = DataConvert.ToInt(dailys[i]._expressid.Value);
                            CODVISIT[] codvisits = client.GetCODVisit(parameter);
                            foreach (var codvisit in codvisits)
                            {
                               
                                var appendItem = new CodDailyModel();
                                appendItem.OrderNo = DataConvert.ToLong(codvisit._orderno);
                                appendItem.CompanyName = codvisit._express;
                                appendItem.RFDFee = 0;
                                appendItem.VanclFee = DataConvert.ToDecimal(codvisit._shouldrefund);
                                appendItem.Remark = "如风达查无此单*";
                                VisitDetailsByCompany.Add(appendItem);
                            }
                        }
                       
                        model = new CodDailyModel();
                        model.CompanyName = vanclDaily._express;
                        model.RFDFee = 0;
                        model.RFDCount = 0;
                        model.VanclFee = vanclDaily._shouldrefund.Value;
                        CODParameter codParameter = new CODParameter();
                        codParameter.DateChecked = true;
                        codParameter.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
                        codParameter.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
                        codParameter.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                        CODVISIT[] Vvisits = client.GetCODVisit(codParameter);

                        model.VanclCount = Vvisits.Length;
                        models.Add(model);
                    }
                    continue;
                }
                

                fmsDaily = dicValues[vanclDaily._expressid.Value];

                //if ((vanclDaily._ordertotalprice.Value - fmsDaily.AllFee) == 0) continue;

                model = new CodDailyModel();
                model.CompanyName = fmsDaily.CompanyName;
                model.RFDFee = fmsDaily.AllFee;
                model.VanclFee = vanclDaily._shouldrefund.Value;

                CODParameter CodPar = new CODParameter();
                CodPar.DateChecked = true;
                CodPar.DtFrom = DataConvert.ToDateTime(DataConvert.ToDayBegin(sTime)).Value;
                CodPar.DtTo = DataConvert.ToDateTime(DataConvert.ToDayEnd(eTime)).Value;
                CodPar.ExpressId = DataConvert.ToInt(vanclDaily._expressid);
                CODVISIT[] visits = client.GetCODVisit(CodPar);
                IDictionary<int, FMSCODDetails> fmsdicValues = service.GetCODVisitDailyByStationID(DataConvert.ToDayBegin(sTime),
                                                                      DataConvert.ToDayEnd(eTime), fmsDaily.CompanyId);
                model.RFDCount = fmsdicValues.Keys.Count;
                model.VanclCount = visits.Length;

                if (model.RFDFee != model.VanclFee)
                {

                    var list = VisitDetailsCheck(visits, fmsdicValues);
                    foreach (var item in list)
                        VisitDetailsByCompany.Add(item);
                }
                dicValues[vanclDaily._expressid.Value].IsChecked = true;
                models.Add(model);
            }
            foreach (var visit in dicValues)
            {
                if(!visit.Value.IsChecked)
                {
                    var appendModel = new CodDailyModel();
                    appendModel.CompanyName = visit.Value.CompanyName;
                    appendModel.RFDFee = visit.Value.AllFee;
                    appendModel.VanclFee = 0;
                    appendModel.VanclCount = 0;
                    IDictionary<int, FMSCODDetails> fmsappendValues = service.GetCODVisitDailyByStationID(DataConvert.ToDayBegin(sTime),
                                                                             DataConvert.ToDayEnd(eTime), visit.Value.CompanyId);
                    appendModel.RFDCount = fmsappendValues.Keys.Count;
                    models.Add(appendModel);
                    foreach (var fmsappendValue in fmsappendValues)
                    {
                        var appendItem = new CodDailyModel();
                        appendItem.OrderNo = fmsappendValue.Value.WaybillNo;
                        appendItem.CompanyName = fmsappendValue.Value.CompanyName;
                        appendItem.RFDFee = fmsappendValue.Value.Fee;
                        appendItem.VanclFee = 0;
                        appendItem.Remark = "查此单凡客不在该配送公司";
                        VisitDetailsByCompany.Add(appendItem);
                    }
                }
            }
            return models;
        }

        private string CreateTable(string title, IList<CodDailyModel> dailys)
        {
            StringBuilder builder = new StringBuilder();

            CodDailyModel model = null;

            builder.Append("<table border=\"1\">");

            builder.Append("<tr align=\"center\">");
            builder.Append("<td colspan=\"4\">");
            builder.Append(title);
            builder.Append("</td>");
            builder.Append("</tr>");

            builder.Append("<tr>");
            builder.Append("<td>");
            builder.Append("站点名");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("凡客");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("凡客单量");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("如风达");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("如风达单量");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("差异金额");
            builder.Append("</td>");
            builder.Append("</tr>");

            for (int i = 0; i < dailys.Count; i++)
            {
                model = dailys[i];
                if (model.RFDFee != model.VanclFee)
                    builder.Append("<tr bgcolor=\"yellow\">");
                else
                    builder.Append("<tr>");

                builder.Append("<td>");
                builder.Append(model.CompanyName);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append(model.VanclFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append(model.VanclCount);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append(model.RFDFee);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append(model.RFDCount);
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append(model.VanclFee - model.RFDFee);
                builder.Append("</td>");

                builder.Append("</tr>");
            }

            builder.Append("</table>");

            return builder.ToString();
        }

        private string CreateDetailsTable(string title, IList<CodDailyModel> dailys)
        {
            StringBuilder builder = new StringBuilder();

            CodDailyModel model = null;

            builder.Append("<table border=\"1\">");

            builder.Append("<tr align=\"center\">");
            builder.Append("<td colspan=\"6\">");
            builder.Append(title);
            builder.Append("</td>");
            builder.Append("</tr>");

            builder.Append("<tr>");
            builder.Append("<td>");
            builder.Append("订单号");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("站点名");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("凡客");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("如风达");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("差异金额");
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append("备注");
            builder.Append("</td>");
            builder.Append("</tr>");

            if (dailys.Count == 0)
            {
                builder.Append("<tr >");

                builder.Append("<td colspan=\"6\" align=\"center\">");
                builder.Append("无差异");
                builder.Append("</td>");
                builder.Append("</tr>");

            }
            for (int i = 0; i < dailys.Count; i++)
            {
               if(dailys[i].VanclFee != dailys[i].RFDFee)
               {
                   model = dailys[i];

                   builder.Append("<tr>");

                   builder.Append("<td>");
                   builder.Append(model.OrderNo.ToString());
                   builder.Append("</td>");
                   builder.Append("<td>");
                   builder.Append(model.CompanyName);
                   builder.Append("</td>");
                   builder.Append("<td>");
                   builder.Append(model.VanclFee);
                   builder.Append("</td>");
                   builder.Append("<td>");
                   builder.Append(model.RFDFee);
                   builder.Append("</td>");
                   builder.Append("<td>");
                   builder.Append(model.VanclFee - model.RFDFee);
                   builder.Append("</td>");
                   builder.Append("<td>");
                   builder.Append(model.Remark);
                   builder.Append("</td>");
                   builder.Append("</tr>");
               }
                
            }

            builder.Append("</table>");
            
            return builder.ToString();
        }
    }
}