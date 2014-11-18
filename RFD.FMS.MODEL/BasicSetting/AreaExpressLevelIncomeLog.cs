using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class AreaExpressLevelIncomeLog
    {
        public AreaExpressLevelIncomeLog()
        { }
        #region Model
        private int _logid;
        private string _areaid;
        private int _expresscompanyid;
        private int _merchantid;
        private string _warehouseid;
        private int _areatype;
        private string _logtext;
        private int _enable;
        private int _createby;
        private DateTime _createtime;
        private string _distributionCode;
        /// <summary>
        /// 自增列
        /// </summary>
        public int LogID
        {
            set { _logid = value; }
            get { return _logid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string AreaID
        {
            set { _areaid = value; }
            get { return _areaid; }
        }

        /// <summary>
        /// 配送公司
        /// </summary>
        public int ExpressCompanyID
        {
            get { return _expresscompanyid; }
            set { _expresscompanyid = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MerchantID
        {
            set { _merchantid = value; }
            get { return _merchantid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string WarehouseId
        {
            set { _warehouseid = value; }
            get { return _warehouseid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int AreaType
        {
            set { _areatype = value; }
            get { return _areatype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LogText
        {
            set { _logtext = value; }
            get { return _logtext; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Enable
        {
            set { _enable = value; }
            get { return _enable; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CreateBy
        {
            set { _createby = value; }
            get { return _createby; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 配送公司编码
        /// </summary>
        public string DistributionCode
        {
            set { _distributionCode = value; }
            get { return _distributionCode; }
        }
        #endregion Model
    }
}
