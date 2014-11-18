using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.QueryStatistics
{
    public class CodDailyModel : IComparable
    {
        /// <summary>
        /// 站点或配送公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 如风达金额
        /// </summary>
        public decimal RFDFee { get; set; }

        /// <summary>
        /// Vancl金额
        /// </summary>
        public decimal VanclFee { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public long OrderNo { get; set; }
        /// <summary>
        /// 对比备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 如风达单量
        /// </summary>
        public int RFDCount { get; set; }

        /// <summary>
        /// Vancl单量
        /// </summary>
        public int VanclCount { get; set; }

        public int CompareTo(object obj)
        {
            if(obj is CodDailyModel)
            {
                CodDailyModel model = obj as CodDailyModel;
                return this.OrderNo > model.OrderNo ? 1 : this.OrderNo == model.OrderNo ? 0 : this.OrderNo < model.OrderNo?-1:1;
            }
            throw new ArgumentException("object is not a CodDailyModel");    
        }
    }


}
