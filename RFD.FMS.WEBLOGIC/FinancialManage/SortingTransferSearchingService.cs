using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Service.SoringManage;
using RFD.FMS.Util;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
     public class SortingTransferSearchingService :ISortingTransferSearchingService
     {
         private ISortingTransferSearchingDao _sortingTransferSearchingDao;
         private ISortingTransferSearchingService OracleService;
        public DataTable SortingTransferAndToStationDetail(SortingDetail Model)
         {
             if (OracleService != null)
             {
                 return OracleService.SortingTransferAndToStationDetail(Model);
             }
             var dt = _sortingTransferSearchingDao.SortingTransferAndToStationDetail(Model);
             dt.Columns.Add("SortingMerchantName");
             foreach (DataRow dr in dt.Rows)
             {
                 dr["SortingMerchantName"] = "北京柏松物流有限公司";
             }
             return dt;
         }

        public DataTable SortingToCityDetail(SortingDetail Model)
        {
            if (OracleService != null)
            {
                return OracleService.SortingToCityDetail(Model);
            }
            var dt= _sortingTransferSearchingDao.SortingToCityDetail(Model);
            dt.Columns.Add("SortingMerchantName");
            foreach (DataRow dr in dt.Rows)
            {
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
            }
            return dt;
        }

        public DataTable ReturnToSortingCenterDetail(SortingDetail Model)
        {
            if (OracleService != null)
            {
                return OracleService.ReturnToSortingCenterDetail(Model);
            }
            var dt = _sortingTransferSearchingDao.ReturnToSortingCenterDetail(Model);
            dt.Columns.Add("SortingMerchantName");
            foreach (DataRow dr in dt.Rows)
            {
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
            }
            return dt;
        }

        public DataTable MerchantToSortingCenterDetail(SortingDetail Model)
        {
            if (OracleService != null)
            {
                return OracleService.MerchantToSortingCenterDetail(Model);
            }
            var dt = _sortingTransferSearchingDao.MerchantToSortingCenterDetail(Model);
           dt.Columns.Add("SortingMerchantName");
           foreach (DataRow dr in dt.Rows)
           {
               dr["SortingMerchantName"] = "北京柏松物流有限公司";
           }
           return dt;
        }


        public DataTable SortingTransferAndToStationDaily(SortingDetail Model)
        {
            if (OracleService != null)
            {
                return OracleService.SortingTransferAndToStationDaily(Model);
            }

            var dt = SortingTransferAndToStationMerchant(Model);

            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SDate = p["SDate"],
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SDate,
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
                
            });
            DataTable newdt= new DataTable();
            newdt.Columns.Add("SDate", typeof(string));
            newdt.Columns.Add("SoringMerchantID",typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof (string));
            newdt.Columns.Add("City", typeof (string));
            newdt.Columns.Add("WaybillSum", typeof (string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                  DataRow dr = newdt.NewRow();
                  dr["SDate"] = q.SDate.ToString();
                  dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                  dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                  dr["City"] = q.City.ToString();
                  dr["WaybillSum"] = q.WaybillSum.ToString();
                  dr["Fee"] = q.Fee.ToString();
                  dr["SortingMerchantName"] = "北京柏松物流有限公司";
                  dr["Code"] = "109";
                   newdt.Rows.Add(dr);
            }


            return newdt;
        }

        public DataTable SortingToCityDaily(SortingDetail Model)
        {
            if (OracleService != null)
            {
                return OracleService.SortingToCityDaily(Model);
            }

            var dt = SortingToCityMerchant(Model);
            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SDate = p["SDate"],
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SDate,
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
            });
            DataTable newdt = new DataTable();
            newdt.Columns.Add("SDate", typeof(string));
            newdt.Columns.Add("SoringMerchantID", typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof(string));
            newdt.Columns.Add("City", typeof(string));
            newdt.Columns.Add("WaybillSum", typeof(string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                DataRow dr = newdt.NewRow();
                dr["SDate"] = q.SDate.ToString();
                dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                dr["City"] = q.City.ToString();
                dr["WaybillSum"] = q.WaybillSum.ToString();
                dr["Fee"] = q.Fee.ToString();
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
                dr["Code"] = "109";
                newdt.Rows.Add(dr);
            }


            return newdt;
        }

        public DataTable ReturnToSortingCenterDaily(SortingDetail Model)
        {
            if (OracleService != null)
            {
                return OracleService.ReturnToSortingCenterDaily(Model);
            }
            var dt = ReturnToSortingCenterMerchant(Model);
            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SDate = p["SDate"],
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SDate,
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
            });
            DataTable newdt = new DataTable();
            newdt.Columns.Add("SDate", typeof(string));
            newdt.Columns.Add("SoringMerchantID", typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof(string));
            newdt.Columns.Add("City", typeof(string));
            newdt.Columns.Add("WaybillSum", typeof(string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                DataRow dr = newdt.NewRow();
                dr["SDate"] = q.SDate.ToString();
                dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                dr["City"] = q.City.ToString();
                dr["WaybillSum"] = q.WaybillSum.ToString();
                dr["Fee"] = q.Fee.ToString();
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
                dr["Code"] = "109";
                newdt.Rows.Add(dr);
            }


            return newdt;
        }

        public DataTable MerchantToSortingCenterDaily(SortingDetail Model)
        {
            if (OracleService != null)
            {
                return OracleService.MerchantToSortingCenterDaily(Model);
            }

            var dt = MerchantToSortingCenterMerchant(Model);
            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SDate = p["SDate"],
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SDate,
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
            });
            DataTable newdt = new DataTable();
            newdt.Columns.Add("SDate", typeof(string));
            newdt.Columns.Add("SoringMerchantID", typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof(string));
            newdt.Columns.Add("City", typeof(string));
            newdt.Columns.Add("WaybillSum", typeof(string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                DataRow dr = newdt.NewRow();
                dr["SDate"] = q.SDate.ToString();
                dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                dr["City"] = q.City.ToString();
                dr["WaybillSum"] = q.WaybillSum.ToString();
                dr["Fee"] = q.Fee.ToString();
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
                dr["Code"] = "109";
                newdt.Rows.Add(dr);
            }


            return newdt;
        }


        public DataTable SortingTransferAndToStationMerchant(SortingDetail Model)
        {
           if(OracleService != null)
           {
               return OracleService.SortingTransferAndToStationMerchant(Model);
           }
            try
            {
                var dt = _sortingTransferSearchingDao.SortingTransferAndToStationMerchant(Model);
                var SortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
                dt.Columns.Add("SortingMerchantName");
                dt.Columns.Add("Code");
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Fee", typeof(string));
                foreach (DataRow dr in dt.Rows)
                {
                    dr["SortingMerchantName"] = "北京柏松物流有限公司";
                    dr["Code"] = "109";
                    int SortingMerchantID = Convert.ToInt32(dr["Code"]);
                    int SortingCenterID = Convert.ToInt32(dr["TSortingCenterID"]);
                    string CityID = dr["CityID"].ToString();
                    int MerchantID = Convert.ToInt32(dr["MerchantID"]);
                    DateTime SDate = Convert.ToDateTime(dr["SDate"]);
                    dr["price"] = SortingFeeSrv.GetAccountFareByMerchant(SortingMerchantID, SortingCenterID, CityID,
                                                                         MerchantID, SDate, Model.ItemType);
                    if (!string.IsNullOrEmpty(dr["price"].ToString()))
                    {
                        dr["Fee"] = (Convert.ToDecimal(dr["price"].ToString()) * Convert.ToInt32(dr["WaybillSum"])).ToString();
                    }
                    else
                    {
                        dr["Fee"] = "0";
                        dr["Price"] = "价格未维护或未生效";
                    }

                }

                return dt;
            }
            catch (Exception ex)
            {
                
                throw new Exception("SqlServer未实现，请在demo下操作！"+ex.Message);
            }
          
        }

        public DataTable SortingToCityMerchant(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.SortingToCityMerchant(Model);
            }
            try
            {
                var dt = _sortingTransferSearchingDao.SortingToCityMerchant(Model);
                var SortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
                dt.Columns.Add("SortingMerchantName");
                dt.Columns.Add("Code");
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Fee", typeof(string));
                foreach (DataRow dr in dt.Rows)
                {
                    dr["SortingMerchantName"] = "北京柏松物流有限公司";
                    dr["Code"] = "109";
                    int SortingMerchantID = Convert.ToInt32(dr["Code"]);
                    int SortingCenterID = Convert.ToInt32(dr["SortingCenterID"]);
                    string CityID = dr["CityID"].ToString();
                    int MerchantID = Convert.ToInt32(dr["MerchantID"]);
                    DateTime SDate = Convert.ToDateTime(dr["SDate"]);
                    dr["Price"] = SortingFeeSrv.GetAccountFareByMerchant(SortingMerchantID, SortingCenterID, CityID,
                                                                         MerchantID, SDate, 2);
                    if (!string.IsNullOrEmpty(dr["Price"].ToString()))
                    {
                        dr["Fee"] = (Convert.ToDecimal(dr["Price"]) * Convert.ToInt32(dr["WaybillSum"])).ToString();
                    }
                    else
                    {
                        dr["Fee"] = "0";
                        dr["Price"] = "价格未维护或未生效";
                    }

                }
                return dt;
            }
            catch (Exception ex)
            {
                
                throw new Exception("Sqlserver未实现，请在demo下操作"+ ex.Message);
            }
           
        }

        public DataTable ReturnToSortingCenterMerchant(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.ReturnToSortingCenterMerchant(Model);
            }
            try
            {
                var dt = _sortingTransferSearchingDao.ReturnToSortingCenterMerchant(Model);
                var SortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
                dt.Columns.Add("SortingMerchantName");
                dt.Columns.Add("Code");
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Fee", typeof(string));
                foreach (DataRow dr in dt.Rows)
                {
                    dr["SortingMerchantName"] = "北京柏松物流有限公司";
                    dr["Code"] = "109";
                    int SortingMerchantID = Convert.ToInt32(dr["Code"]);
                    int SortingCenterID = Convert.ToInt32(dr["SortingCenter"]);
                    string CityID = dr["CityID"].ToString();
                    int MerchantID = Convert.ToInt32(dr["MerchantID"]);
                    DateTime SDate = Convert.ToDateTime(dr["SDate"]);
                    dr["price"] = SortingFeeSrv.GetAccountFareByMerchant(SortingMerchantID, SortingCenterID, CityID,
                                                                         MerchantID, SDate, 2);
                    if (!string.IsNullOrEmpty(dr["price"].ToString()))
                    {
                        dr["Fee"] = (Convert.ToDecimal(dr["price"]) * Convert.ToInt32(dr["WaybillSum"])).ToString();
                    }
                    else
                    {
                        dr["Fee"] = "0";
                        dr["Price"] = "价格未维护或未生效";
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                
                throw  new Exception("SqlServer未实现，请在demo下操作" +ex.Message);
            }
        }

        public DataTable MerchantToSortingCenterMerchant(SortingDetail Model)
        {
           if(OracleService != null)
           {
               return OracleService.MerchantToSortingCenterMerchant(Model);
           }
            try
            {
                var dt = _sortingTransferSearchingDao.MerchantToSortingCenterMerchant(Model);
                var SortingFeeSrv = ServiceLocator.GetService<ISortingFeeService>();
                dt.Columns.Add("SortingMerchantName");
                dt.Columns.Add("Code");
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Fee", typeof(string));
                foreach (DataRow dr in dt.Rows)
                {
                    dr["SortingMerchantName"] = "北京柏松物流有限公司";
                    dr["Code"] = "109";
                    int SortingMerchantID = Convert.ToInt32(dr["Code"]);
                    int SortingCenterID = Convert.ToInt32(dr["SortingCenterID"]);
                    string CityID = dr["CityID"].ToString();
                    int MerchantID = Convert.ToInt32(dr["MerchantID"]);
                    DateTime SDate = Convert.ToDateTime(dr["SDate"]);
                    dr["price"] = SortingFeeSrv.GetAccountFareByMerchant(SortingMerchantID, SortingCenterID, CityID,
                                                                         MerchantID, SDate, 2);
                    if (!string.IsNullOrEmpty(dr["price"].ToString()))
                    {
                        dr["Fee"] = (Convert.ToDecimal(dr["price"]) * Convert.ToInt32(dr["WaybillSum"])).ToString();
                    }
                    else
                    {
                        dr["Fee"] = "0";
                        dr["Price"] = "价格未维护或未生效";
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                
                throw new Exception("Sqlserver未实现，请在demo下操作！"+ ex.Message);
            }
        }


        public DataTable SortingTransferAndToStationAll(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.SortingTransferAndToStationAll(Model);
            }
            var dt = SortingTransferAndToStationMerchant(Model);
            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
            });
            DataTable newdt = new DataTable();
            newdt.Columns.Add("StatisticsType", typeof (string));
            newdt.Columns.Add("SoringMerchantID", typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof(string));
            newdt.Columns.Add("City", typeof(string));
            newdt.Columns.Add("WaybillSum", typeof(string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                DataRow dr = newdt.NewRow();
                dr["StatisticsType"] = "合计";
                dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                dr["City"] = q.City.ToString();
                dr["WaybillSum"] = q.WaybillSum.ToString();
                dr["Fee"] = q.Fee.ToString();
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
                dr["Code"] = "109";
                newdt.Rows.Add(dr);
            }


            return newdt;
        }

        public DataTable SortingToCityAll(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.SortingToCityAll(Model);
            }
            var dt = SortingToCityMerchant(Model);
            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
            });
            DataTable newdt = new DataTable();
            newdt.Columns.Add("StatisticsType", typeof(string));
            newdt.Columns.Add("SoringMerchantID", typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof(string));
            newdt.Columns.Add("City", typeof(string));
            newdt.Columns.Add("WaybillSum", typeof(string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                DataRow dr = newdt.NewRow();
                dr["StatisticsType"] = "合计";
                dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                dr["City"] = q.City.ToString();
                dr["WaybillSum"] = q.WaybillSum.ToString();
                dr["Fee"] = q.Fee.ToString();
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
                dr["Code"] = "109";
                newdt.Rows.Add(dr);
            }


            return newdt;
        }

         public DataTable ReturnToSortingCenterAll(SortingDetail Model)
        {
            if(OracleService !=null)
            {
                return OracleService.ReturnToSortingCenterAll(Model);
            }
            var dt = ReturnToSortingCenterMerchant(Model);
            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
            });
            DataTable newdt = new DataTable();
            newdt.Columns.Add("StatisticsType", typeof(string));
            newdt.Columns.Add("SoringMerchantID", typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof(string));
            newdt.Columns.Add("City", typeof(string));
            newdt.Columns.Add("WaybillSum", typeof(string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                DataRow dr = newdt.NewRow();
                dr["StatisticsType"] = "合计";
                dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                dr["City"] = q.City.ToString();
                dr["WaybillSum"] = q.WaybillSum.ToString();
                dr["Fee"] = q.Fee.ToString();
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
                dr["Code"] = "109";
                newdt.Rows.Add(dr);
            }


            return newdt;
        }

        public DataTable MerchantToSortingCenterAll(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.MerchantToSortingCenterAll(Model);
            }
            var dt = MerchantToSortingCenterMerchant(Model);
            var query = dt.AsEnumerable().GroupBy(p => new
            {
                SoringMerchantID = p["SoringMerchantID"],
                SortingCenterAll = p["SortingCenterAll"],
                City = p["City"]
            }).Select(m => new
            {
                m.Key.SoringMerchantID,
                m.Key.SortingCenterAll,
                m.Key.City,
                WaybillSum = m.Sum(k => Convert.ToDecimal(k["WaybillSum"].ToString())),
                Fee = m.Sum(k => Convert.ToDecimal(k["Fee"].ToString()))
            });
            DataTable newdt = new DataTable();
            newdt.Columns.Add("StatisticsType", typeof(string));
            newdt.Columns.Add("SoringMerchantID", typeof(string));
            newdt.Columns.Add("SortingCenterAll", typeof(string));
            newdt.Columns.Add("City", typeof(string));
            newdt.Columns.Add("WaybillSum", typeof(string));
            newdt.Columns.Add("SortingMerchantName");
            newdt.Columns.Add("Code");
            newdt.Columns.Add("Price");
            newdt.Columns.Add("Fee");
            foreach (var q in query)
            {
                DataRow dr = newdt.NewRow();
                dr["StatisticsType"] = "合计";
                dr["SoringMerchantID"] = q.SoringMerchantID.ToString();
                dr["SortingCenterAll"] = q.SortingCenterAll.ToString();
                dr["City"] = q.City.ToString();
                dr["WaybillSum"] = q.WaybillSum.ToString();
                dr["Fee"] = q.Fee.ToString();
                dr["SortingMerchantName"] = "北京柏松物流有限公司";
                dr["Code"] = "109";
                newdt.Rows.Add(dr);
            }


            return newdt;
        }
     }
}
