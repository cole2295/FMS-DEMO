using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    public class IncomeOtherFeeCount
    {
        private Int64 _countid;
        /// <summary>
        /// 自增列
        /// </summary>		
        public Int64 CountID
        {
            get { return _countid; }
            set { _countid = value; }
        }
        private String _accountno;
        /// <summary>
        /// 结算单号
        /// </summary>		
        public String AccountNO
        {
            get { return _accountno; }
            set { _accountno = value; }
        }
        private Int32 _merchantid;
        /// <summary>
        /// 商家ID
        /// </summary>		
        public Int32 MerchantID
        {
            get { return _merchantid; }
            set { _merchantid = value; }
        }
        private Int32 _expresscompanyid;
        /// <summary>
        /// 分拣中心ID
        /// </summary>		
        public Int32 ExpressCompanyID
        {
            get { return _expresscompanyid; }
            set { _expresscompanyid = value; }
        }
        private Int32 _areatype;
        /// <summary>
        /// 区域类型
        /// </summary>		
        public Int32 AreaType
        {
            get { return _areatype; }
            set { _areatype = value; }
        }
        private Int32 _counttype;
        /// <summary>
        /// 类型:1普发，2换发，3普拒，4换拒，5退，6退拒
        /// </summary>		
        public Int32 CountType
        {
            get { return _counttype; }
            set { _counttype = value; }
        }
        private System.DateTime _countdate;
        /// <summary>
        /// 统计日
        /// </summary>		
        public System.DateTime CountDate
        {
            get { return _countdate; }
            set { _countdate = value; }
        }
        private System.Decimal? _protectedstandard;
        /// <summary>
        /// 保价费结算标准
        /// </summary>		
        public System.Decimal? ProtectedStandard
        {
            get { return _protectedstandard; }
            set { _protectedstandard = value; }
        }
        private System.Decimal? _protectedfee;
        /// <summary>
        /// 保价费
        /// </summary>		
        public System.Decimal? ProtectedFee
        {
            get { return _protectedfee; }
            set { _protectedfee = value; }
        }
        private System.Decimal? _receivestandard;
        /// <summary>
        /// 代收货款现金结算标准
        /// </summary>		
        public System.Decimal? ReceiveStandard
        {
            get { return _receivestandard; }
            set { _receivestandard = value; }
        }
        private System.Decimal? _receivefee;
        /// <summary>
        /// 代收货款现金手续费
        /// </summary>		
        public System.Decimal? ReceiveFee
        {
            get { return _receivefee; }
            set { _receivefee = value; }
        }
        private System.Decimal? _receiveposstandard;
        /// <summary>
        /// 代收货款POS结算标准
        /// </summary>		
        public System.Decimal? ReceivePOSStandard
        {
            get { return _receiveposstandard; }
            set { _receiveposstandard = value; }
        }
        private System.Decimal? _receiveposfee;
        /// <summary>
        /// 代收货款POS手续费
        /// </summary>		
        public System.Decimal? ReceivePOSFee
        {
            get { return _receiveposfee; }
            set { _receiveposfee = value; }
        }
        private Int32? _createby;
        /// <summary>
        /// 创建人
        /// </summary>		
        public Int32? CreateBy
        {
            get { return _createby; }
            set { _createby = value; }
        }
        private System.DateTime? _createtime;
        /// <summary>
        /// 创建时间
        /// </summary>		
        public System.DateTime? CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        private Int32? _updateby;
        /// <summary>
        /// 更新人
        /// </summary>		
        public Int32? UpdateBy
        {
            get { return _updateby; }
            set { _updateby = value; }
        }
        private System.DateTime? _updatetime;
        /// <summary>
        /// 更新时间
        /// </summary>		
        public System.DateTime? UpdateTime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
        }
        private System.Byte _isdeleted;
        /// <summary>
        /// 删除标识
        /// </summary>		
        public System.Byte IsDeleted
        {
            get { return _isdeleted; }
            set { _isdeleted = value; }
        }
        private Int32? _stationid;
        /// <summary>
        /// 配送站
        /// </summary>		
        public Int32? StationID
        {
            get { return _stationid; }
            set { _stationid = value; }
        }

        private System.Decimal? _servesstandard;
        /// <summary>
        /// 代收货款现金服务费结算标准
        /// </summary>		
        public System.Decimal? ServesStandard
        {
            get { return _servesstandard; }
            set { _servesstandard = value; }
        }
        private System.Decimal? _servesfee;
        /// <summary>
        /// 代收货款现金服务费
        /// </summary>		
        public System.Decimal? ServesFee
        {
            get { return _servesfee; }
            set { _servesfee = value; }
        }
        private System.Decimal? _posservesstandard;
        /// <summary>
        /// 代收货款POS服务费结算标准
        /// </summary>		
        public System.Decimal? POSServesStandard
        {
            get { return _posservesstandard; }
            set { _posservesstandard = value; }
        }
        private System.Decimal? _posservesfee;
        /// <summary>
        /// 代收货款POS服务费
        /// </summary>		
        public System.Decimal? POSServesFee
        {
            get { return _posservesfee; }
            set { _posservesfee = value; }
        }
    }
}
