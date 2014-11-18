using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class AccountPeriod
    {
        /// <summary>
        /// 唯一序列
        /// </summary>
        public String AccountPeriodKid { get; set; }

        /// <summary>
        /// 账期名称
        /// </summary>
        public String PeriodName { get; set; }

        /// <summary>
        /// 是否启用0、启用,1、不启用
        /// </summary>
        public Int32 IsDeleted { get; set; }

        /// <summary>
        /// 是否启用0、启用,1、不启用
        /// </summary>
        public String IsDeletedStr { get; set; }

        /// <summary>
        /// 账期类型:1、周，2、月，3、自定义
        /// </summary>
        public Int32 PeriodType { get; set; }

        /// <summary>
        /// 子账期类型:周（1、周N次，2、本周结上周）月（无）自定义（无）
        /// </summary>
        public Int32 PeriodTypeChild { get; set; }

        /// <summary>
        /// 账期开始值：周值（,分割），月（日数），自定义（起始日期）
        /// </summary>
        public String PeriodStart { get; set; }

        /// <summary>
        /// 账期类型:是否只结当月费用，未结部分流入下一账期，0否，1是
        /// </summary>
        public Int32 IsMonthPeriod { get; set; }

        /// <summary>
        /// 是否按配送商，0、全部，1、不包含，2包含
        /// </summary>
        public Int32 IsExpress { get; set; }

        /// <summary>
        /// 配送公司ID
        /// </summary>
        public String ExpressIds { get; set; }

        /// <summary>
        /// 配送公司名称
        /// </summary>
        public String ExpressNames { get; set; }

        /// <summary>
        /// 间隔数
        /// </summary>
        public Int32 IntervalNum { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Int32 CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public Int32 UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 使用者分类
        /// </summary>
        public String PeriodRelationName { get; set; }

        /// <summary>
        /// 所属配送公司
        /// </summary>
        public String DistributionCode { get; set; }

        /// <summary>
        /// 模拟账期日期
        /// </summary>
        public String ImitateDate { get; set; }

        /// <summary>
        /// 模拟延续次数
        /// </summary>
        public Int32 ImitateNum { get; set; }

        /// <summary>
        /// 账期列表
        /// </summary>
        public List<AccountPeriodDate> AccountPeriodList { get; set; }

        /// <summary>
        /// 模拟账期信息
        /// </summary>
        public String ImitateAccountPeriod { get; set; }
    }

    [Serializable]
    public class AccountPeriodCondition
    {
        /// <summary>
        /// 使用者分类
        /// </summary>
        public String PeriodRelationName { get; set; }

        /// <summary>
        /// 唯一序列
        /// </summary>
        public String AccountPeriodKid { get; set; }

        /// <summary>
        /// 删除标识
        /// </summary>
        public String IsDeleted { get; set; }
    }

    [Serializable]
    public class AccountPeriodDate
    {
        /// <summary>
        /// 账期日
        /// </summary>
        public DateTime AccountDate { get; set; }

        /// <summary>
        /// 结算范围开始日期 包含当天
        /// </summary>
        public DateTime AccountPeriodStartDate { get; set; }

        /// <summary>
        /// 结算范围结束日期 包含当天
        /// </summary>
        public DateTime AccountPeriodEndDate { get; set; }
    }
}
