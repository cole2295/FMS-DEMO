using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    /// <summary>
    /// 项目提成实体
    /// </summary>
    [Serializable]
    public class ExpressCompanyGoodsDeduct
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 配送站ID
        /// </summary>
        public int? ExpressCompanyID { get; set; }
        
        /// <summary>
        /// 货物品类编码
        /// </summary>
        public string GoodsCategoryCode { get; set; }
        
        /// <summary>
        /// 基础提成
        /// </summary>
        public decimal? BasicCommission { get; set; }
        
        /// <summary>
        /// 大件重量
        /// </summary>
        public decimal? WeightCommission { get; set; }
        
        /// <summary>
        /// 重量追加
        /// </summary>
        public decimal? ExtraCommission { get; set; }
        
        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime? UseDate { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreateBy { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        
        /// <summary>
        /// 更新人
        /// </summary>
        public int? UpdateBy { get; set; }
        
        /// <summary>
        /// 是否停用
        /// </summary>
        public int? IsDeleted { get; set; }
    }
}
