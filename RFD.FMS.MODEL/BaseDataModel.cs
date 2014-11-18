using System;

namespace RFD.FMS.Model
{
    [Serializable]
    public abstract class BaseDataModel<V>
    {
        private V _id;
        private int? _createuser;
        private int? _updateuser;
        private DateTime? _createtime;
        private DateTime? _updatetime;
		private string _createUserCode;
		private string _updateUserCode;
        private bool _isdelete;
        private int? _createdeptid;
        private int? _updatedeptid;
        private bool isNew = true;

        /// <summary>
        /// 主键
        /// </summary>
        public virtual V ID
        {
            set { _id = value; }
            get { return _id; }
        }

        /// <summary>
        /// 建立用户
        /// </summary>
        public int? CreateUser
        {
            set { _createuser = value; }
            get { return _createuser; }
        }
        /// <summary>
        /// 更新用户
        /// </summary>
        public int? UpdateUser
        {
            set { _updateuser = value; }
            get { return _updateuser; }
        }
        /// <summary>
        /// 建立时间
        /// </summary>
        public DateTime? CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }

		public string CreateUserCode
		{
			set { _createUserCode = value; }
			get { return _createUserCode; }
		}

		public string UpdateUserCode
		{
			set { _updateUserCode = value; }
			get { return _updateUserCode; }
		}

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete
        {
            set { _isdelete = value; }
            get { return _isdelete; }
        }

        /// <summary>
        /// 建立部门
        /// </summary>
        public int? CreateDeptId
        {
            set { _createdeptid = value; }
            get { return _createdeptid; }
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        public int? UpdateDeptId
        {
            set { _updatedeptid = value; }
            get { return _updatedeptid; }
        }

        /// <summary>
        /// 是否为新生成的对象,
        /// 还是从数据库中取的已存在的
        /// </summary>
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }
    }
}
