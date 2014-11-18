using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.FinancialManage
{
    [Serializable]
    public partial class FMS_DeductBaseInfo
    {
        /// <summary>
        /// 主键
        /// </summary>		
        public Int64 DeductID { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>		
        public Int64 WaybillNO { get; set; }

        /// <summary>
        /// 运单类型
        /// </summary>		
        public String WaybillType { get; set; }
        
        /// <summary>
        /// 运单状态
        /// </summary>		
        public String Status { get; set; }
        
        /// <summary>
        /// 商家Id
        /// </summary>		
        public Int32? MerchantID { get; set; }
        
        /// <summary>
        /// 商家类型
        /// </summary>		
        public Int32? MerchantType { get; set; }
        
        /// <summary>
        /// 配送站
        /// </summary>		
        public Int32 DeliverStationID { get; set; }
        
        /// <summary>
        /// 配送员ID
        /// </summary>		
        public Int32? DeliverManID { get; set; }
    
        /// <summary>
        /// 接单时间
        /// </summary>		
        public System.DateTime? RfdAcceptTime { get; set; }
        
        /// <summary>
        /// 归班时间
        /// </summary>		
        public System.DateTime? BackStationTime { get; set; }

        /// <summary>
        /// 保价金额
        /// </summary>		
        public System.Decimal? ProtectedPrice { get; set; }
        
        /// <summary>
        /// 运单总金额
        /// </summary>		
        public System.Decimal? Amount { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>		
        public System.Decimal? NeedAmount { get; set; }
        
        /// <summary>
        /// 应退金额
        /// </summary>		
        public System.Decimal? NeedBackAmount { get; set; }

        /// <summary>
        /// 接单站点
        /// </summary>		
        public Int32? ReceiveStationID { get; set; }
        
        /// <summary>
        /// 接单配送员
        /// </summary>		
        public Int32? ReceiveDeliverManID { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>		
        public String AcceptType { get; set; }
        
        /// <summary>
        /// 实际重量
        /// </summary>		
        public System.Decimal? WayBillInfoWeight { get; set; }

        /// <summary>
        /// 派件站长提成
        /// </summary>		
        public System.Decimal? AreaSendDeduct { get; set; }

        /// <summary>
        /// 取件站长提成
        /// </summary>		
        public System.Decimal? AreaReceiveDeduct { get; set; }

        /// <summary>
        /// 是否计算提成
        /// </summary>		
        public Int32? IsDeductAcount { get; set; }
        
        /// <summary>
        /// 货物属性
        /// </summary>		
        public String WaybillProperty { get; set; }

        /// <summary>
        /// 货物品类
        /// </summary>		
        public String WaybillCategory { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 客户地址
        /// </summary>		
        public String ReceiveAddress { get; set; }

        /// <summary>
        /// 取件地址
        /// </summary>		
        public String SendAddress { get; set; }
        /// <summary>
        /// 提成
        /// </summary>		
        public decimal? AdjustCommission { get; set; }
    }
}
