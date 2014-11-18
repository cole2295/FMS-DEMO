using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class AreaExpressLevelIncome
    {
        
        public AreaExpressLevelIncome()
        { }

        #region Model
        private int _autoid;
        private string _areaid;
        private string _areaname;
        private int _expresscompanyid;
        private string _companyname;
        private int _merchantid;
        private string _merchantname;
        private string _warehouseid;
        private int _areatype;
        private int _enable = 1;
        private int? _effectareatype;
        private DateTime? _dodate;
        private int _createby = 0;
        private DateTime _createtime = DateTime.Now;
        private int _updateby = 0;
        private DateTime _updatetime = DateTime.Now;
        private int _auditstatus;
        private int _auditby = 0;
        private DateTime _audittime = DateTime.Now;
        private int _effectdeleted;
        private string _distributionCode;
        private string _goodsCategoryCode;
        private string _goodsCategoryName;
        public int _isExpress;
        private string _expressName;

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
        /// 分拣中心名称
        /// </summary>
        public string CompanyName
        {
            set { _companyname = value; }
            get { return _companyname; }
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
        /// 商家名称
        /// </summary>
        public string MerchantName
        {
            set { _merchantname = value; }
            get { return _merchantname; }
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
        public int CreateBy
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
        public int UpdateBy
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
        public int AuditBy
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
        /// 配送公司编码
        /// </summary>
        public string DistributionCode
        {
            set { _distributionCode = value; }
            get { return _distributionCode; }
        }

        /// <summary>
        /// 货物品类编号
        /// </summary>
        public string GoodsCategoryCode
        {
            set { _goodsCategoryCode = value; }
            get { return _goodsCategoryCode; }
        }

        /// <summary>
        /// 货物品类名称
        /// </summary>
        public string GoodsCategoryName
        {
            set { _goodsCategoryName = value; }
            get { return _goodsCategoryName; }
        }

        /// <summary>
        /// 是否走配送商逻辑，0不走，1走
        /// </summary>
        public int IsExpress
        {
            set { _isExpress = value; }
            get { return _isExpress; }
        }
        /// <summary>
        /// 配送商
        /// </summary>
        public string ExpressName
        {
            set { _expressName = value; }
            get { return _expressName; }
        }
        #endregion
    }
    [Serializable]
    public class AreaLevelIncomeSearchModel
    {
        /// <summary>
        /// 省ID
        /// </summary>
        public String ProvinceID { get; set; }

        /// <summary>
        /// 市ID
        /// </summary>
        public String CityID { get; set; }

        /// <summary>
        /// 区ID
        /// </summary>
        public String AreaID { get; set; }

        /// <summary>
        /// 配送公司
        /// </summary>
        public int ExpressCompanyID { get; set; }

        /// <summary>
        /// 区域类型
        /// </summary>
        public int AreaType { get; set; }

        /// <summary>
        /// 分拣中心
        /// </summary>
        public String WareHouse { get; set; }

        /// <summary>
        /// 商家ID
        /// </summary>
        public Int32 MerchantID { get; set; }
        
        /// <summary>
        /// 审核状态
        /// </summary>
        public Int32 AuditStatus { get; set; }

        /// <summary>
        /// 配送公司CODE
        /// </summary>
        public String DistributionCode { get; set; }

        /// <summary>
        /// 货物品类
        /// </summary>
        public String GoodsCategoryCode { get; set; }
        /// <summary>
        /// 配送商
        /// </summary>
        public Int32 IsExpress { get; set; }

    }
}
