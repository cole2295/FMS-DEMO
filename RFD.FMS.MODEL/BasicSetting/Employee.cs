using System;

namespace RFD.FMS.Model
{
    /// <summary>
    /// 实体类Employee 。(属性说明自动提取数据库字段的描述信息)
    /// 雇员
    /// </summary>
    [Serializable]
    public class Employee : BaseDataModel<int>
    {
        private int _employeeid;
        private string _employeecode;
        private string _employeeoldcode;
        private string _employeename;
        private bool _sex;
        private string _password;
        private string _address;
        private string _cellphone;
        private string _postid;
        private string _eemail;
        private int? _stationid;
        private decimal? _deposit;
        private string _pdacode;
        private string _poscode;
        private int _sorting;
        private bool _isdeleted;
        private int? _creatby;
        private int? _createstationid;
        private DateTime? _createtime;
        private int? _updateby;
        private DateTime? _updatetime;
        private int? _updatestationid;
        private string _telephone;
        private string _idcard;
        private string _relationman;
        private string _relationphone;
        private string _relation;

        public string _sortStr;

        /// <summary>
        /// 员工编号（内码）
        /// </summary>
        public override int ID
        {
            get { return _employeeid; }
            set { _employeeid = value; }
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
        /// 员工代号
        /// </summary>
        public string EmployeeCode
        {
            set { _employeecode = value; }
            get { return _employeecode; }
        }
        /// <summary>
        /// 员工代号（原来老的编号）
        /// </summary>
        public string EmployeeOldCode
        {
            set { _employeeoldcode = value; }
            get { return _employeeoldcode; }
        }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmployeeName
        {
            set { _employeename = value; }
            get { return _employeename; }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex
        {
            set { _sex = value; }
            get { return _sex; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            set { _password = value; }
            get { return _password; }
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
        /// 手机
        /// </summary>
        public string CellPhone
        {
            set { _cellphone = value; }
            get { return _cellphone; }
        }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostID
        {
            set { _postid = value; }
            get { return _postid; }
        }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string eEmail
        {
            set { _eemail = value; }
            get { return _eemail; }
        }
        /// <summary>
        /// 所在站点
        /// </summary>
        public int? StationID
        {
            set { _stationid = value; }
            get { return _stationid; }
        }
        /// <summary>
        /// 押金
        /// </summary>
        public decimal? Deposit
        {
            set { _deposit = value; }
            get { return _deposit; }
        }
        /// <summary>
        /// PDA号码
        /// </summary>
        public string PDACode
        {
            set { _pdacode = value; }
            get { return _pdacode; }
        }
        /// <summary>
        /// 座机号
        /// </summary>
        public string Telephone
        {
            set { _telephone = value; }
            get { return _telephone; }
        }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard
        {
            set { _idcard = value; }
            get { return _idcard; }
        }
        /// <summary>
        /// 紧急联系人
        /// </summary>
        public string RelationMan
        {
            set { _relationman = value; }
            get { return _relationman; }
        }
        /// <summary>
        /// 紧急联系人电话
        /// </summary>
        public string RelationPhone
        {
            set { _relationphone = value; }
            get { return _relationphone; }
        }
        /// <summary>
        /// 紧急联系人关系
        /// </summary>
        public string Relation
        {
            set { _relation = value; }
            get { return _relation; }
        }
        /// <summary>
        /// POS机号码
        /// </summary>
        public string POSCode
        {
            set { _poscode = value; }
            get { return _poscode; }
        }
        /// <summary>
        /// 排序方式
        /// </summary>
        public int Sorting
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
        /// 创建人所在单位
        /// </summary>
        public int? CreateStationID
        {
            set { _createstationid = value; }
            get { return _createstationid; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 最后一次更新人
        /// </summary>
        public int? UpdateBy
        {
            set { _updateby = value; }
            get { return _updateby; }
        }
        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime? UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }
        /// <summary>
        /// 最后一次更新人所在单位
        /// </summary>
        public int? UpdateStationID
        {
            set { _updatestationid = value; }
            get { return _updatestationid; }
        }
    }
}
