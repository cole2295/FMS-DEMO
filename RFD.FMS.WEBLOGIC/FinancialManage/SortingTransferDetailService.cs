using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
   public class SortingTransferDetailService :ISortingTransferDetailService
   {
       private ISortingTransferDetailDao _sortingTransferDetailDao;
       private ISortingTransferDetailService OracleService;
       public DataTable SortingTransferAndToStationDetail(SortingDetail Model)
       {
           if(OracleService != null)
           {
               return OracleService.SortingTransferAndToStationDetail(Model);
           }
           var dt=_sortingTransferDetailDao.SortingTransferAndToStationDetail(Model);
           dt.Columns.Add("SortingMerchantName");
           foreach (DataRow datarow in dt.Rows)
           {
               datarow["SortingMerchantName"] = "北京柏松物流有限公司";
           }
           return dt;
       }

       public DataTable ExportSortingTransferAndToStationDetail(SortingDetail Model)
       {
           if(OracleService != null)
           {
               return OracleService.ExportSortingTransferAndToStationDetail(Model);
           }
           var dt = _sortingTransferDetailDao.ExportSortingTransferAndToStationDetail(Model);
           //dt.Columns.Add("配送商名称",typeof(string));
           //foreach (DataRow datarow in dt.Rows)
           //{
           //    datarow["配送商名称"] = "北京柏松物流有限公司";
           //}
           return dt;
       }



        public DataTable SortingToCityDetail(SortingDetail Model)
        {
           if(OracleService != null)
           {
               return OracleService.SortingToCityDetail(Model);
           }
            var dt = _sortingTransferDetailDao.SortingToCityDetail(Model);
            dt.Columns.Add("SortingMerchantName");
            foreach (DataRow datarow in dt.Rows)
            {
                datarow["SortingMerchantName"] = "北京柏松物流有限公司";
            }
            return dt;
        }

        public DataTable ExportSortingToCityDetail(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.ExportSortingToCityDetail(Model);
            }
            var dt = _sortingTransferDetailDao.ExportSortingToCityDetail(Model);

            return dt;
        }




        public DataTable ReturnToSortingCenterDetail(SortingDetail Model)
        {
           if(OracleService !=null)
           {
               return OracleService.ReturnToSortingCenterDetail(Model);
           }
            var dt=  _sortingTransferDetailDao.ReturnToSortingCenterDetail(Model);
           dt.Columns.Add("SortingMerchantName");
           foreach (DataRow datarow in dt.Rows)
           {
               datarow["SortingMerchantName"] = "北京柏松物流有限公司";
           }
           return dt;
        }

      public DataTable ExportReturnToSortingCenterDetail(SortingDetail Model)
       {
          if(OracleService != null)
          {
              return OracleService.ExportReturnToSortingCenterDetail(Model);
          }
          var dt = _sortingTransferDetailDao.ExportReturnToSortingCenterDetail(Model);
           return dt;
       }



        public DataTable MerchantToSortingCenterDetail(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.MerchantToSortingCenterDetail(Model);
            }
            var dt= _sortingTransferDetailDao.MerchantToSortingCenterDetail(Model);
            dt.Columns.Add("SortingMerchantName");
            foreach (DataRow datarow in dt.Rows)
            {
                datarow["SortingMerchantName"] = "北京柏松物流有限公司";
            }
            return dt;
        }

       public  DataTable ExportMerchantToSortingCenterDetail(SortingDetail Model)
       {
           if(OracleService != null)
           {
               return OracleService.ExportMerchantToSortingCenterDetail(Model);
           }
           
           var dt = _sortingTransferDetailDao.ExportMerchantToSortingCenterDetail(Model);

           return dt;
       }



        public int CountSortingTransferAndToStationDetail(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.CountSortingTransferAndToStationDetail(Model);
            }
            return _sortingTransferDetailDao.CountSortingTransferAndStationDetail(Model);
        }


        public int CountSortingToCityDetail(SortingDetail Model)
        {
            if(OracleService != null)
            {
                return OracleService.CountSortingToCityDetail(Model);
            }
            return _sortingTransferDetailDao.CountSortingToCityDetail(Model);
        }

        public int CountReturnToSortingCenterDetail(SortingDetail Model)
        {
           if(OracleService != null)
           {
               return OracleService.CountReturnToSortingCenterDetail(Model);
           }
            return _sortingTransferDetailDao.CountReturnToSortingCenterDetail(Model);
        }

        public int CountMerchantToSortingCenterDetail(SortingDetail Model)
        {
           if(OracleService !=null)
           {
               return OracleService.CountMerchantToSortingCenterDetail(Model);
           }
            return _sortingTransferDetailDao.CountMerchantToSortingCenterDetail(Model);
        }


        
   }
}
