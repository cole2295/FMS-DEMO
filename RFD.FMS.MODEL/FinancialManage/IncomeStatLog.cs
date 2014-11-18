using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    public class IncomeStatLog
    {
        private Int32 _logid;
        /// <summary>
        /// 主键
        /// </summary>		
        public Int32 LogID
        {
            get { return _logid; }
            set { _logid = value; }
        }
        private Int32? _statisticstype;
        /// <summary>
        /// 类型
        /// </summary>		
        public Int32? StatisticsType
        {
            get { return _statisticstype; }
            set { _statisticstype = value; }
        }
        private Int32? _merchantid;
        /// <summary>
        /// 商家
        /// </summary>		
        public Int32? MerchantID
        {
            get { return _merchantid; }
            set { _merchantid = value; }
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
        private Int32? _expresscompanyid;
        /// <summary>
        /// 分拣中心
        /// </summary>		
        public Int32? ExpressCompanyID
        {
            get { return _expresscompanyid; }
            set { _expresscompanyid = value; }
        }
        private System.DateTime? _statisticsdate;
        /// <summary>
        /// 统计日期
        /// </summary>		
        public System.DateTime? StatisticsDate
        {
            get { return _statisticsdate; }
            set { _statisticsdate = value; }
        }
        private Int32? _issuccess;
        /// <summary>
        /// 是否成功
        /// </summary>		
        public Int32? IsSuccess
        {
            get { return _issuccess; }
            set { _issuccess = value; }
        }
        private String _reasons;
        /// <summary>
        /// 原因
        /// </summary>		
        public String Reasons
        {
            get { return _reasons; }
            set { _reasons = value; }
        }
        private String _ip;
        /// <summary>
        /// Ip
        /// </summary>		
        public String Ip
        {
            get { return _ip; }
            set { _ip = value; }
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
        private System.DateTime? _updatetime;
        /// <summary>
        /// 更新时间
        /// </summary>		
        public System.DateTime? UpdateTime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
        }
    }
}
