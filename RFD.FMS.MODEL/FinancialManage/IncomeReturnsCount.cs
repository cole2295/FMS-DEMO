using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    public class IncomeReturnsCount
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
        private System.Decimal? _weight;
        /// <summary>
        /// 重量
        /// </summary>		
        public System.Decimal? Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        private Int32 _counttype;
        /// <summary>
        /// 拒收类型
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
        private Int32 _countnum;
        /// <summary>
        /// 单量
        /// </summary>		
        public Int32 CountNum
        {
            get { return _countnum; }
            set { _countnum = value; }
        }
        private System.Decimal _fare;
        /// <summary>
        /// 运费
        /// </summary>		
        public System.Decimal Fare
        {
            get { return _fare; }
            set { _fare = value; }
        }
        private String _formula;
        /// <summary>
        /// 结算标准
        /// </summary>		
        public String Formula
        {
            get { return _formula; }
            set { _formula = value; }
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
    }
}
