using System;

namespace LMS.Model
{
    /// <summary>
    /// 实体类EmployeeRole 。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class EmployeeRole
    {
        #region Model
        private int _employeeroleid;
        private int _employeeid;
        private int _roleid;
        private bool _isdeleted;
        private int _creatby;
        private int _creatstation;
        private DateTime _creattime;
        private int _updateby;
        private int _updatesation;
        private DateTime _updatetime;
        /// <summary>
        /// 主键
        /// </summary>
        public int EmployeeRoleID
        {
            set { _employeeroleid = value; }
            get { return _employeeroleid; }
        }
        /// <summary>
        /// 员工编号
        /// </summary>
        public int EmployeeID
        {
            set { _employeeid = value; }
            get { return _employeeid; }
        }
        /// <summary>
        /// 角色编号
        /// </summary>
        public int RoleID
        {
            set { _roleid = value; }
            get { return _roleid; }
        }
        /// <summary>
        /// 删除标志0正常1已删除
        /// </summary>
        public bool IsDeleted
        {
            set { _isdeleted = value; }
            get { return _isdeleted; }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public int CreatBy
        {
            set { _creatby = value; }
            get { return _creatby; }
        }
        /// <summary>
        /// 创建人所在站点
        /// </summary>
        public int CreatStation
        {
            set { _creatstation = value; }
            get { return _creatstation; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime
        {
            set { _creattime = value; }
            get { return _creattime; }
        }
        /// <summary>
        /// 最后更新人
        /// </summary>
        public int UpdateBy
        {
            set { _updateby = value; }
            get { return _updateby; }
        }
        /// <summary>
        /// 最后更新人所在站点
        /// </summary>
        public int UpdateSation
        {
            set { _updatesation = value; }
            get { return _updatesation; }
        }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }
        #endregion Model

    }
}
