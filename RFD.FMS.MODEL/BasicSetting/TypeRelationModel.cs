using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    public class TypeRelationModel
    {
        private string _typeRelationKid;
        private string _typeRelationName;
        private string _distributionCode;
        private string _typeCodeNo;
        private int _relationNameID;
        private bool _isDelete;
        private int _createBy;
        private DateTime _createTime;
        private int _updateBy;
        private DateTime _updateTime;

        private string _relationName;
        private int _relationId;
        private string _codeDesc;

        /// <summary>
        /// 主键
        /// </summary>
        public string TypeRelationKid
        {
            get { return _typeRelationKid; }
            set { _typeRelationKid = value; }
        }

        /// <summary>
        /// 关系名称
        /// </summary>
        public string TypeRelationName
        {
            get { return _typeRelationName; }
            set { _typeRelationName = value; }
        }
        /// <summary>
        /// 配送公司编码
        /// </summary>
        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }
        /// <summary>
        /// 分类编号
        /// </summary>
        public string TypeCodeNo
        {
            get { return _typeCodeNo; }
            set { _typeCodeNo = value; }
        }
        /// <summary>
        /// 关联名称ID
        /// </summary>
        public int RelationNameID
        {
            get { return _relationNameID; }
            set { _relationNameID = value; }
        }
        /// <summary>
        /// 是否停用
        /// </summary>
        public bool IsDelete
        {
            get { return _isDelete; }
            set { _isDelete = value; }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateBy
        {
            get { return _createBy; }
            set { _createBy = value; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
        /// <summary>
        /// 更新人
        /// </summary>
        public int UpdateBy
        {
            get { return _updateBy; }
            set { _updateBy = value; }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string RelationName
        {
            get { return _relationName; }
            set { _relationName = value; }
        }

        /// <summary>
        /// 名称id
        /// </summary>
        public int RelationId
        {
            get { return _relationId; }
            set { _relationId = value; }
        }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CodeDesc
        {
            get { return _codeDesc; }
            set { _codeDesc = value; }
        }
    }
}
