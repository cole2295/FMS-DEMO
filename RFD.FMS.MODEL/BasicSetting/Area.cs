using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    public class Area
    {
        /// <summary>
        /// 地区ID
        /// </summary>
        public string AreaID { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        public string CityID { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal TransferFee { get; set; }

        /// <summary>
        ///排序 
        /// </summary>
        public int Sorting { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int CreatBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }
        /// <summary>
        /// 创建人所在站点
        /// </summary>
        public int CreatStation { get; set; }

        /// <summary>
        /// 最后更新人
        /// </summary>
        public int UpdateBy { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 最后更新人所在站点
        /// </summary>
        public int UpdateStation { get; set; }

        /// <summary>
        /// 是否启用Gis分配
        /// renyu 0902
        /// </summary>
        public bool? GisStatus { get; set; }

        /// <summary>
        /// 配送商企业ID号
        /// </summary>
        public string DistributionCode
        {
            get;
            set;
        }
    }
}
