using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    [Serializable]
    public partial class FMS_DeductSubBaseInfo
    {
        /// <summary>
        /// 主键
        /// </summary>		
        public Int64 ID { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>		
        public Int64 WaybillNO { get; set; }

        /// <summary>
        /// 计算公式
        /// </summary>		
        public string Formula { get; set; }
        
        /// <summary>
        /// 提成类型(1快递取件，2快递派件，3项目取件，4项目派件)
        /// </summary>		
        public int DeductType { get; set; }
        
        /// <summary>
        /// 站点编号
        /// </summary>		
        public Int32? StationId { get; set; }
        
        /// <summary>
        /// 配送员
        /// </summary>		
        public Int32? DeliverUser { get; set; }
        
        /// <summary>
        /// 基础提成
        /// </summary>		
        public decimal BasicCommission { get; set; }
        
        /// <summary>
        /// 区域提成
        /// </summary>		
        public decimal AreaCommission { get; set; }
    
        /// <summary>
        /// 重量追加提成
        /// </summary>		
        public decimal WeightCommission { get; set; }

        /// <summary>
        /// 总提成金额
        /// </summary>
        public decimal SumCommission { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>		
        public System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>		
        public System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        public int IsDeleted { get; set; }

        /// <summary>
        /// 提成调整
        /// </summary>		
        public decimal AdjustCommission { get; set; }
    }
}
