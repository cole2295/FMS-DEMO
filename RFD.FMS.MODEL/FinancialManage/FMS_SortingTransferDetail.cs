using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    public class FMS_SortingTransferDetail
    {
        /// <summary>
        /// 
        /// </summary>
        private string _detailID;
        public string  DetailKID { 
            get { return _detailID; }

            set { _detailID = value; } 
        }

        private int _merchantID;
        public int MerchantID
        {
            get { return _merchantID; }
            set { _merchantID = value; }
        }

        private Int64 _waybillNo;
        public  Int64 WaybillNO
        {
            get { return _waybillNo; }
            set { _waybillNo = value; }
        }

        private string _waybillType;
        public string WaybillType
        {
            get{ return _waybillType; }
            set { _waybillType = value; }

        }

        private int? _sortingCenter;
        public int? SortingCenter
        {
            get { return _sortingCenter; }
            set { _sortingCenter = value; }
        }

        private int? _tSortingCenter;
        public int? TSortingCenter
        {
            get { return _tSortingCenter; }
            set { _tSortingCenter = value; }
        }

        private string _returnSortingCenter;
        public  string ReturnSortingCenter
        {
            get { return _returnSortingCenter; }
            set { _returnSortingCenter = value; }

        }

        private int? _sortingMerchantID;
        public int? SortingMerchantID
        {
            get { return _sortingMerchantID; }
            set { _sortingMerchantID = value; }
        }

        private string _createCityID;
        public string CreateCityID
        {
            get { return _createCityID; }
            set { _createCityID = value; }
        }

        private string _sortingCityID;
        public string SortingCityID
        {

            get { return _sortingCityID; }
            set { _sortingCityID = value; }
        }

        private int? _deliverStationID;
        public int? DeliverStationID
        {
            get { return _deliverStationID; }
            set { _deliverStationID = value; }
        }

        private int? _topCODCompanyID ;
        public int? TopCODCompanyID
        {
            get { return _topCODCompanyID; }
            set { _topCODCompanyID = value; }
        }

        private DateTime? _toStationTime;
        public DateTime? ToStationTime
        {
            get { return _toStationTime; }
            set { _toStationTime = value; }
        }

        private DateTime? _outBoundTime;
        public DateTime? OutBoundTime
        {
            get { return _outBoundTime; }
            set { _outBoundTime = value; }
        }

        private DateTime? _inSortingTime;
        public  DateTime? InSortingTime
        {
            get { return _inSortingTime; }
            set { _inSortingTime = value; }
        }

        private DateTime? _returnTime;
        public  DateTime? ReturnTime
        {
            get { return _returnTime; }
            set { _returnTime = value; }
        }

        private string _distributionCode;
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }

        private int _isAccount;
        public  int IsAccount
        {
            get { return _isAccount; }
            set { _isAccount = value; }
        }

        private decimal? _accountFare;
        public  decimal? AccountFare
        {
            get { return _accountFare; }
            set { _accountFare = value; }
        }

        private string _accountFormula;
        public string AccountFormula
        {
            get { return _accountFormula; }
            set { _accountFormula = value; }
        }

        private bool _isDelete;
        public bool IsDelete
        {
            get { return _isDelete; }
            set { _isDelete = value; }
        }

        private DateTime _createTime;
        public  DateTime CreateTime
        {
          get
          {
            return _createTime;
          }
          set
          {
            _createTime = value;
          }
        }

        private DateTime _updateTime;
        public  DateTime UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        private int _outType;
        public  int OutType
        {
            get { return _outType; }
            set { _outType = value; }
        }

        private int _intoType;
        public  int IntoType
        {
            get { return _intoType; }
            set { _intoType = value; }
        }
    }
}
