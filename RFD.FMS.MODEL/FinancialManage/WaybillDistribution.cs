using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    /// <summary>
    /// WaybillDistribution:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class WaybillDistribution
    {
        public WaybillDistribution()
        { }
        #region Model
        private DateTime? _intotime;
        private long? _waybillno;
        private string _waybilltype;
        private decimal? _needamount;
        private decimal? _needbackamount;
        private decimal? _factamount;
        private decimal? _factbackamount;
        private string _signstatus;
        private string _accepttype;
        private string _poscode;
        private DateTime? _signtime;
        private string _employeename;
        private string _deductnotes;
        private string _intoStationType;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? IntoTime
        {
            set { _intotime = value; }
            get { return _intotime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long? WaybillNO
        {
            set { _waybillno = value; }
            get { return _waybillno; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string WaybillType
        {
            set { _waybilltype = value; }
            get { return _waybilltype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? NeedAmount
        {
            set { _needamount = value; }
            get { return _needamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? NeedBackAmount
        {
            set { _needbackamount = value; }
            get { return _needbackamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FactAmount
        {
            set { _factamount = value; }
            get { return _factamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FactBackAmount
        {
            set { _factbackamount = value; }
            get { return _factbackamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SignStatus
        {
            set { _signstatus = value; }
            get { return _signstatus; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string AcceptType
        {
            set { _accepttype = value; }
            get { return _accepttype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string POSCode
        {
            set { _poscode = value; }
            get { return _poscode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? SignTime
        {
            set { _signtime = value; }
            get { return _signtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EmployeeName
        {
            set { _employeename = value; }
            get { return _employeename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DeductNotes
        {
            set { _deductnotes = value; }
            get { return _deductnotes; }
        }

        //public string IntoStationType
        //{
        //    get { return _intoStationType; }
        //    set { _intoStationType = value; }
        //}

        #endregion Model

    }
}
