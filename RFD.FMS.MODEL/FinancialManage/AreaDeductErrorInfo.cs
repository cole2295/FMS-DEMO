using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    public class AreaDeductErrorInfo
    {
        /// <summary>
        /// 主键
        /// </summary>		
        public Int64 AreaDeductErrorInfoId { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>		
        public Int64 WaybillNO { get; set; }

        /// <summary>
        /// 站点ID
        /// </summary>
        public int StationId { get; set; }

        /// <summary>
        /// 提成类型（默认为区域提成）
        /// </summary>
        public string DeductType { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>		
        public String AddressInfo { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>		
        public String KeyWords { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>		
        public String AreaCommision { get; set; }

        /// <summary>
        /// 异常类型
        /// </summary>		
        public String Errortype { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>		
        public String ErrorInfo { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>		
        public String DisposeStatus { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>		
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>		
        public int CreateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>		
        public System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>		
        public int UpdateBy { get; set; }


    }
}
