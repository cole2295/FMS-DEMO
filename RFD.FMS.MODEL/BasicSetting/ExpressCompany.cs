using System;

namespace RFD.FMS.Model
{
    /// <summary>
    /// 实体类ExpressCompany 。(属性说明自动提取数据库字段的描述信息)
    /// 组织结构，部门
    /// </summary>
    [Serializable]
    public class ExpressCompany
    {
        private int _expresscompanyid;
        private int _expressCompanyOldID;
        private int _expressCompanyVjiaOldID;
        private string _expresscompanycode;
        private string _companyname;
        private string _companyallname;
        private string _simplespell;
        private string _provinceid;
        private string _cityid;
        private int? _parentid;
        private int _companyflag;
        private string _maincontacter;
        private string _levelcode;
        private int? _maxorderlimit;
        private bool _isreplacement;
        private bool _ispos;
        private bool _iscod;
        private bool _ispda;
        private string _address;
        private string _email;
        private string _contacterphone;
        private decimal? _deposit;
        private int? _sorting;
        private bool _isdeleted;
        private int? _creatby;
        private DateTime? _creattime;
        private int? _creatstationid;
        private int? _updateby;
        private DateTime? _updatetime;
        private int? _updatestationid;
        private string _districtID;
        private string _permissionStr;
        private string _distributioncode;//add by wangyongc 2011-09-23
        private bool _isreturn;//add by taozui 2011-10-25

        public string _sortStr;

        public string PermissionStr
        {
            get { return _permissionStr; }
            set { _permissionStr = value; }
        }
        /// <summary>
        /// 是否支持上门退货（1支持0不支持
        /// </summary>
        public bool IsReturn
        {
            set { _isreturn = value; }
            get { return _isreturn; }
        }
        /// <summary>
        /// 排序
        /// </summary>
        public string SortStr
        {
            set { _sortStr = value; }
            get { return _sortStr; }
        }

        /// <summary>
        /// 组织机构内码
        /// </summary>
        public int ExpressCompanyID
        {
            set { _expresscompanyid = value; }
            get { return _expresscompanyid; }
        }

        /// <summary>
        /// 组织机构内码
        /// </summary>
        public int ExpressCompanyOldID
        {
            set { _expressCompanyOldID = value; }
            get { return _expressCompanyOldID; }
        }

        /// <summary>
        /// 组织机构内码
        /// </summary>
        public int ExpressCompanyVjiaOldID
        {
            set { _expressCompanyVjiaOldID = value; }
            get { return _expressCompanyVjiaOldID; }
        }

        /// <summary>
        /// 组织结构代码
        /// </summary>
        public string ExpressCompanyCode
        {
            set { _expresscompanycode = value; }
            get { return _expresscompanycode; }
        }
        /// <summary>
        /// 组织机构名称
        /// </summary>
        public string CompanyName
        {
            set { _companyname = value; }
            get { return _companyname; }
        }
        /// <summary>
        /// 组织机构全称
        /// </summary>
        public string CompanyAllName
        {
            set { _companyallname = value; }
            get { return _companyallname; }
        }
        /// <summary>
        /// 简拼如国贸站gmz
        /// </summary>
        public string SimpleSpell
        {
            set { _simplespell = value; }
            get { return _simplespell; }
        }
        /// <summary>
        /// 所在省份
        /// </summary>
        public string ProvinceID
        {
            set { _provinceid = value; }
            get { return _provinceid; }
        }
        /// <summary>
        /// 所在城市
        /// </summary>
        public string CityID
        {
            set { _cityid = value; }
            get { return _cityid; }
        }
        /// <summary>
        /// 上级编码
        /// </summary>
        public int? ParentID
        {
            set { _parentid = value; }
            get { return _parentid; }
        }
        /// <summary>
        /// 部门类型（0行政部门、1分拣中心、2站点、3加盟商）
        /// </summary>
        public int CompanyFlag
        {
            set { _companyflag = value; }
            get { return _companyflag; }
        }
        /// <summary>
        /// 主要联系人
        /// </summary>
        public string MainContacter
        {
            set { _maincontacter = value; }
            get { return _maincontacter; }
        }
        /// <summary>
        /// 查询码
        /// </summary>
        public string LevelCode
        {
            set { _levelcode = value; }
            get { return _levelcode; }
        }
        /// <summary>
        /// 最大接单量
        /// </summary>
        public int? MaxOrderLimit
        {
            set { _maxorderlimit = value; }
            get { return _maxorderlimit; }
        }
        /// <summary>
        /// 是否支持上门换货(1支持0不支持)
        /// </summary>
        public bool IsReplacement
        {
            set { _isreplacement = value; }
            get { return _isreplacement; }
        }
        /// <summary>
        /// 是否支持POS机（1支持0不支持
        /// </summary>
        public bool IsPOS
        {
            set { _ispos = value; }
            get { return _ispos; }
        }
        /// <summary>
        /// 是否支持货到付款1支持0不支持
        /// </summary>
        public bool IsCod
        {
            set { _iscod = value; }
            get { return _iscod; }
        }
        /// <summary>
        /// 是否支PDA
        /// </summary>
        public bool IsPDA
        {
            set { _ispda = value; }
            get { return _ispda; }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            set { _address = value; }
            get { return _address; }
        }
        /// <summary>
        /// 站点邮箱
        /// </summary>
        public string Email
        {
            set { _email = value; }
            get { return _email; }
        }
        /// <summary>
        /// 主要联系人电话
        /// </summary>
        public string ContacterPhone
        {
            set { _contacterphone = value; }
            get { return _contacterphone; }
        }
        /// <summary>
        /// 加盟商押金
        /// </summary>
        public decimal? Deposit
        {
            set { _deposit = value; }
            get { return _deposit; }
        }
        /// <summary>
        /// 排序方式
        /// </summary>
        public int? Sorting
        {
            set { _sorting = value; }
            get { return _sorting; }
        }
        /// <summary>
        /// 删除标志(0正常1已删除)
        /// </summary>
        public bool IsDeleted
        {
            set { _isdeleted = value; }
            get { return _isdeleted; }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreatBy
        {
            set { _creatby = value; }
            get { return _creatby; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatTime
        {
            set { _creattime = value; }
            get { return _creattime; }
        }
        /// <summary>
        /// 创建人所在单位
        /// </summary>
        public int? CreatStationID
        {
            set { _creatstationid = value; }
            get { return _creatstationid; }
        }
        /// <summary>
        /// 最后更新人
        /// </summary>
        public int? UpdateBy
        {
            set { _updateby = value; }
            get { return _updateby; }
        }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }
        /// <summary>
        /// 最后更新人所在站点
        /// </summary>
        public int? UpdateStationID
        {
            set { _updatestationid = value; }
            get { return _updatestationid; }
        }

        /// <summary>
        /// 所在大区
        /// </summary>
        public string DistrictId
        {
            get { return _districtID; }
            set { _districtID = value; }
        }

        /// <summary> 
        /// 配送商编号 add by wangyongc 2011-09-23
        /// </summary>
        public string DistributionCode
        {
            get { return _distributioncode; }
            set { _distributioncode = value; }
        }

        public int TopCODCompanyID { get; set; }
    }
}
