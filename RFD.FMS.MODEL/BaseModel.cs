using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
	[Serializable]
	public class BaseModel
	{
		public Int32 CreateBy { get; set; }
		public DateTime CreateTime { get; set; }
		public Int32 UpdateBy { get; set; }
		public DateTime UpdateTime { get; set; }
		public Int32 AuditBy { get; set; }
		public DateTime AuditTime { get; set; }
		public Int32 IsDeleted { get; set; }

        /// <summary>
        /// 配送商编码
        /// </summary>
        public String DistributionCode { get; set; }
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建站点
        /// </summary>
        public string CreateStation { get; set; }
        /// <summary>
        /// 修改站点
        /// </summary>
        public string UpdateStation { get; set; }

	}
}
