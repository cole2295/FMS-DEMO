using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceForCodAccount.Model
{
	public enum EnumIsEchelon
	{
		/// <summary>
		/// 金额计算
		/// </summary>
		Price = 2,
		/// <summary>
		/// 公式计算
		/// </summary>
		Formula = 1
	}

	public enum EnumCsType
	{
		/// <summary>
		/// 发货
		/// </summary>
		T1=1,
		/// <summary>
		/// 拒收
		/// </summary>
		T2=2,
		/// <summary>
		/// 上门退
		/// </summary>
		T3=3
	}
}
