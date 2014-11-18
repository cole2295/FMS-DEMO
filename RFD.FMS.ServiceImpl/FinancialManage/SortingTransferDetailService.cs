using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.FinancialManage;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
    public class SortingTransferDetailService : ISortingTransferDetailService
    {
        private ISortingTransferDetailDao _sortingTransferDetailDao;
        public DataTable SortingTransferAndToStationDetail(SortingDetail Model)
        {
            var dt = _sortingTransferDetailDao.SortingTransferAndToStationDetail(Model);
            dt.Columns.Add("SortingMerchantName");
            foreach (DataRow datarow in dt.Rows)
            {
                datarow["SortingMerchantName"] = "北京柏松物流有限公司";
            }
            return dt;
        }

        public DataTable ExportSortingTransferAndToStationDetail(SortingDetail Model)
        {
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
            var dt = _sortingTransferDetailDao.ExportSortingToCityDetail(Model);

            return dt;
        }




        public DataTable ReturnToSortingCenterDetail(SortingDetail Model)
        {
            var dt = _sortingTransferDetailDao.ReturnToSortingCenterDetail(Model);
            dt.Columns.Add("SortingMerchantName");
            foreach (DataRow datarow in dt.Rows)
            {
                datarow["SortingMerchantName"] = "北京柏松物流有限公司";
            }
            return dt;
        }

        public DataTable ExportReturnToSortingCenterDetail(SortingDetail Model)
        {
            var dt = _sortingTransferDetailDao.ExportReturnToSortingCenterDetail(Model);
            return dt;
        }



        public DataTable MerchantToSortingCenterDetail(SortingDetail Model)
        {
            var dt = _sortingTransferDetailDao.MerchantToSortingCenterDetail(Model);
            dt.Columns.Add("SortingMerchantName");
            foreach (DataRow datarow in dt.Rows)
            {
                datarow["SortingMerchantName"] = "北京柏松物流有限公司";
            }
            return dt;
        }

        public DataTable ExportMerchantToSortingCenterDetail(SortingDetail Model)
        {
            var dt = _sortingTransferDetailDao.ExportMerchantToSortingCenterDetail(Model);

            return dt;
        }



        public int CountSortingTransferAndToStationDetail(SortingDetail Model)
        {
            return _sortingTransferDetailDao.CountSortingTransferAndStationDetail(Model);
        }


        public int CountSortingToCityDetail(SortingDetail Model)
        {
            return _sortingTransferDetailDao.CountSortingToCityDetail(Model);
        }

        public int CountReturnToSortingCenterDetail(SortingDetail Model)
        {
            return _sortingTransferDetailDao.CountReturnToSortingCenterDetail(Model);
        }

        public int CountMerchantToSortingCenterDetail(SortingDetail Model)
        {
            return _sortingTransferDetailDao.CountMerchantToSortingCenterDetail(Model);
        }



    }
}

