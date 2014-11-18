using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class WaybillStatusChangeLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 运单编号
        /// </summary>
        public long WaybillNo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string CustomerOrder { get; set; }

        /// <summary>
        /// 状态变化环节
        /// </summary>
        public int CurNode { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 当前子状态
        /// </summary>
        public int SubStatus { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int? MerchantID { get; set; }

        /// <summary>
        /// 配送商编号
        /// </summary>
        public string DistributionCode { get; set; }

        /// <summary>
        /// 配送站点
        /// </summary>
        public int? DeliverStationID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int CreateBy { get; set; }

        /// <summary>
        /// 创建部门
        /// </summary>
        public int CreateStation { get; set; }

        /// <summary>
        /// 是否同步
        /// </summary>
        public bool IsSyn { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 操作类型【相关模块】
        /// </summary>
        public int? OperateType { get; set; }

        /// <summary>
        /// LMS同步到TMS状态
        /// </summary>
        public int? TmsSyncStatus { get; set; }
    }
}
