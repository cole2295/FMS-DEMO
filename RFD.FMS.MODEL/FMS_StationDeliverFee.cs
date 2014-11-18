using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.Enumeration;

namespace RFD.FMS.MODEL
{
	/// <summary>
	/// 站点配送费
	/// </summary>
	public class FMS_StationDeliverFee : FMS_DeliverFee
	{
		/// <summary>
		/// 该商家所包含的站点
		/// </summary>
		public int StationID { get; set; }

		/// <summary>
		/// 站点名称
		/// </summary>
		public string StationName { get; set; }

        /// <summary>
        /// 站点所在省份
        /// </summary>
        public string ProvinceID { get; set; }

        /// <summary>
        /// 站点所在城市
        /// </summary>
        public string CityID { get; set; }

        /// <summary>
        /// 是否选择所有状态
        /// </summary>
        public bool bChooseAllStatus { get; set; }

        /// <summary>
        /// 维护状态
        /// </summary>
        public MaintainStatus MaintainedStatus { get; set; }

        /// <summary>
        /// 开始站点最后更新时间
        /// </summary>
        public DateTime? LastStartUpdateTime { get; set; }

        /// <summary>
        /// 结束站点最后更新时间
        /// </summary>
        public DateTime? LastEndUpdateTime { get; set; }

		/// <summary>
		/// 分拣中心
		/// </summary>
		public int ExpressCompanyID { get; set; }

        /// <summary>
        /// 分拣中心名称
        /// </summary>
        public string ExpressCompanyName { get; set; }

		/// <summary>
		/// 是否分拣中心
		/// </summary>
		public int IsCenterSort { get; set; }

		/// <summary>
		/// 区域类型
		/// </summary>
		public int AreaType { get; set; }

        /// <summary>
        /// 配送公司编码
        /// </summary>
        public string DistributionCode { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime EffectDate { get; set; }

        /// <summary>
        /// 生效表ID
        /// </summary>
        public string EffectKid { get; set; }

        /// <summary>
        /// 货物品类编码
        /// </summary>
        public string GoodsCategoryCode { get; set; }

        /// <summary>
        /// 是否区分COD
        /// </summary>
        public int IsCod { get; set; }

        /// <summary>
        /// 非COD价格公式
        /// </summary>
        public string DeliverFee { get; set; }
        /// <summary>
        /// 是否走配送商逻辑（0不走，1走）
        /// </summary>
        public int IsExpress { get; set; }
	}
}
