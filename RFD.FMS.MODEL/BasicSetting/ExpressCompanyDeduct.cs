using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    /// <summary>
    /// 站点提成实体
    /// </summary>
    [Serializable]
    public class ExpressCompanyDeduct
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 配送站ID
        /// </summary>
        public int ExpressCompanyID { get; set; }
        
        /// <summary>
        /// 快递取件提成公式
        /// </summary>
        public string ExpressReceiveDeduct { get; set; }
        
        /// <summary>
        /// 快递派件提成公式
        /// </summary>
        public string ExpressSendDeduct { get; set; }
        
        /// <summary>
        /// 项目取件提成公式
        /// </summary>
        public string ProgramReceiveDeduct { get; set; }
        
        /// <summary>
        /// 项目派件提成公式
        /// </summary>
        public string ProgramSendDeduct { get; set; }
        
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
        /// 是否删除
        /// </summary>
        public int? IsDeleted { get; set; }
        
        /// <summary>
        /// 取件基础提成
        /// </summary>
        public decimal? ReceiveBasicCommission { get; set; }
        
        /// <summary>
        /// 取件基础重量
        /// </summary>
        public decimal? ReceiveBASICWEIGHT { get; set; }
        
        /// <summary>
        /// 取件重量追加提成
        /// </summary>
        public decimal? ReceiveWEIGHTADDCOMMISSION { get; set; }
        
        /// <summary>
        /// 派件基础提成
        /// </summary>
        public decimal? SendBasicCommission { get; set; }
       
        /// <summary>
        /// 派件基础重量
        /// </summary>
        public decimal? SendBASICWEIGHT { get; set; }
        
        /// <summary>
        /// 派件重量追加提成
        /// </summary>
        public decimal? SendWEIGHTADDCOMMISSION { get; set; }
        
        /// <summary>
        /// 状态(0、待生效1、已生效)
        /// </summary>
        public int? Status { get; set; }
    }
}
