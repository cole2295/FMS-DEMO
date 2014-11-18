using System;
using Wuqi.Webdiyer;

namespace RFD.FMS.WEB.UserControl
{
	public partial class Pager : System.Web.UI.UserControl
	{
		public event EventHandler PagerPageChanged;

		public AspNetPager InnerPager
		{
			get
			{
				return this.AspNetPager;
			}
			set
			{
				this.AspNetPager = value;
			}
		}

		public int PageSize
		{
			get
			{
				return this.AspNetPager.PageSize;
			}
			set
			{
				ViewState["CurrentPageSize"] = value;
				this.AspNetPager.PageSize = value;
			}
		}

		public bool AlwaysShow
		{
			set
			{
				this.AspNetPager.AlwaysShow = value;
			}
		}

		public int CurrentPageIndex
		{
			get
			{

				return this.AspNetPager.CurrentPageIndex;
			}
			set
			{
				ViewState["CurrentPageIndex"] = value;
				this.AspNetPager.CurrentPageIndex = value;
			}
		}

		public int RecordCount
		{
			set
			{
				this.AspNetPager.RecordCount = value;
			}
		}

		public string CustomInfoHTML
		{
			get
			{
				return this.AspNetPager.CustomInfoHTML;
			}
			set
			{
				this.AspNetPager.CustomInfoHTML = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void AspNetPager_PageChanged(object sender, EventArgs e)
		{
			if (PagerPageChanged != null)
			{
				PagerPageChanged(sender, e);
			}

		}
	}
}