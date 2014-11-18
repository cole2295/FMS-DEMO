using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class AreaExpressLevel
    {
        #region Model
        private int _autoid;
        private string _areaid;
        private string _areaname;
        private int _expresscompanyid;
        private string _companyname;
        private string _warehouseid;
        private int _areatype;
        private int _enable = 1;
        private int? _effectareatype;
        private DateTime? _dodate;
        private string _createby = "";
        private DateTime _createtime = DateTime.Now;
        private string _updateby = "";
        private DateTime _updatetime = DateTime.Now;
        private int _auditstatus;
        private string _auditby = "";
        private DateTime _audittime = DateTime.Now;
        private int _effectdeleted;
        private int _warehousetype;
        private int _merchantid;
        private int _productid;
        private string _distributionCode;
        /// <summary>
        /// 自增列
        /// </summary>
        public int AutoId
        {
            set { _autoid = value; }
            get { return _autoid; }
        }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaID
        {
            set { _areaid = value; }
            get { return _areaid; }
        }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName
        {
            set { _areaname = value; }
            get { return _areaname; }
        }

        /// <summary>
        /// 配送商ID
        /// </summary>
        public int ExpressCompanyID
        {
            set { _expresscompanyid = value; }
            get { return _expresscompanyid; }
        }

        /// <summary>
        /// 配送商名称
        /// </summary>
        public string CompanyName
        {
            set { _companyname = value; }
            get { return _companyname; }
        }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string WareHouseID
        {
            set { _warehouseid = value; }
            get { return _warehouseid; }
        }
        /// <summary>
        /// 区域类型
        /// </summary>
        public int AreaType
        {
            set { _areatype = value; }
            get { return _areatype; }
        }
        /// <summary>
        /// 是否有效
        /// </summary>
        public int Enable
        {
            set { _enable = value; }
            get { return _enable; }
        }
        /// <summary>
        /// 待生效状态
        /// </summary>
        public int? EffectAreaType
        {
            set { _effectareatype = value; }
            get { return _effectareatype; }
        }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? DoDate
        {
            set { _dodate = value; }
            get { return _dodate; }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy
        {
            set { _createby = value; }
            get { return _createby; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy
        {
            set { _updateby = value; }
            get { return _updateby; }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int AuditStatus
        {
            set { _auditstatus = value; }
            get { return _auditstatus; }
        }
        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditBy
        {
            set { _auditby = value; }
            get { return _auditby; }
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditTime
        {
            set { _audittime = value; }
            get { return _audittime; }
        }

        /// <summary>
        /// 待删除状态
        /// </summary>
        public int EffectDeleted
        {
            set { _effectdeleted = value; }
            get { return _effectdeleted; }
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
