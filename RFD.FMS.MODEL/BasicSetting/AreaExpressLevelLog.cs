using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class AreaExpressLevelLog
    {
        #region Model
        private int _logid;
        private string _areaid;
        private int _expresscompanyid;
        private string _warehouseid;
        private int _areatype;
        private string _logtext;
        private int _enable;
        private string _createby;
        private DateTime _createtime;
        private int _warehousetype;
        private int _merchantid;
        private int _productid;
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
        /// 
        /// </summary>
        public int ExpressCompanyID
        {
            set { _expresscompanyid = value; }
            get { return _expresscompanyid; }
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
        public string CreateBy
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
        /// 0、无，1、仓库，2、分拣中心
        /// </summary>
        public int WareHouseType
        {
            set { _warehousetype = value; }
            get { return _warehousetype; }
        }

        /// <summary>
        /// 商家ID
        /// </summary>
        public int MerchantID
        {
            set { _merchantid = value; }
            get { return _merchantid; }
        }

        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductID
        {
            set { _productid = value; }
            get { return _productid; }
        }

        /// <summary>
        /// 配送公司编码
        /// </summary>
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
        #endregion Model
    }
}
