using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ServiceForCodAccount.Common
{
	public class EnumList
	{
		public enum EnumErrorType
		{
			/// <summary>
			/// 未计算
			/// </summary>
			[Description("未计算")]
			E1=0,
			/// <summary>
			/// 已计算
			/// </summary>
			[Description("已计算")]
			E2=1,
			/// <summary>
			/// 区域类型未取到
			/// </summary>
			[Description("区域类型未取到")]
			E3=2,
			/// <summary>
			/// 计算公式未取到
			/// </summary>
			[Description("计算公式未取到")]
			E4=3,
			/// <summary>
			/// 计算费用失败
			/// </summary>
			[Description("计算费用失败")]
			E5 = 4,
			/// <summary>
			/// 信息配送商未取到
			/// </summary>
			[Description("信息配送商未取到")]
			E6 = 5,
			/// <summary>
            /// 不存在配送商或仓库或商家
			/// </summary>
			[Description("不存在配送商或仓库或商家")]
			E7 = 6,
            /// <summary>
            /// 区域ID为空
            /// </summary>
            [Description("区域ID为空")]
            E8 = 7,
            /// <summary>
            /// 重量小于等于0
            /// </summary>
            [Description("重量小于等于0")]
            E9 = 8,
            /// <summary>
            /// 如风达配送第三方商家不计算运费
            /// </summary>
            [Description("如风达配送第三方商家不计算运费")]
            E11 = 10,
            /// <summary>
            /// 作废
            /// </summary>
            [Description("作废")]
            E12 = 11
		}
	}
}
