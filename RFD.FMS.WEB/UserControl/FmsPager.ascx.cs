using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace RFD.FMS.WEB.UserControl
{
	public partial class FmsPager : System.Web.UI.UserControl
	{
		public event Action<DataTable> PageChange;

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public QueryPager QueryPager
		{
			get
			{
				return ViewState[this.ClientID + "QueryPager"] as QueryPager;
			}
			set
			{
				ViewState[this.ClientID + "QueryPager"] = value;
			}
		}

		public DataTable Init(QueryPager queryPager)
		{
			QueryPager = queryPager;
			this.pager.RecordCount = queryPager.AllCount;
			return queryPager.DoQuery(this.pager.CurrentPageIndex - 1, this.pager.PageSize);
		}

		public int PageSize { set { this.pager.PageSize = value; } }

		protected void pager_PageChanged(object sender, EventArgs e)
		{

			var dt = QueryPager.DoQuery(this.pager.CurrentPageIndex - 1, this.pager.PageSize);
			if (PageChange != null)
			{
				PageChange(dt);
			}
		}
	}

}
