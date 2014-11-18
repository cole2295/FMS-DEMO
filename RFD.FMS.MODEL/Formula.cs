using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Model;

namespace RFD.FMS.MODEL
{
	/// <summary>
	/// 计费公式表
	/// </summary>
	public class Formula: BaseDataModel<int>
	{
		/// <summary>
		/// 计费公式名称
		/// </summary>
		public string FormulaName { get; set; }
		/// <summary>
		/// 公式模板，如：({0} + {1}) * {2}
		/// </summary>
		public string FormulaTemplate { get; set; }
	}
}
