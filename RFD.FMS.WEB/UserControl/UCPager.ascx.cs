using System;
using System.Data;



namespace RFD.FMS.WEB.UserControl
{
    public partial class UCPager : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshButton();
        }

        public event Action<DataTable> DoFrist;
        public event Action<DataTable> DoPre;
        public event Action<DataTable> DoNext;
        public event Action<DataTable> DoLast;

        public QueryPager QueryPager
        {
            get
            {
                return ViewState["QueryPager"] as QueryPager;
            }
            set
            {
                ViewState["QueryPager"] = value;
            }
        }

        public DataTable Init(QueryPager queryPager)
        {
            QueryPager = queryPager;

            return queryPager.Next();
        }

        protected void btnFrist_Click(object sender, EventArgs e)
        {
            DataTable table = QueryPager.Frist();

            if (table == null) return;

            RefreshButton();

            if (DoFrist != null)
            {
                DoFrist(table);
            }
        }

        protected void btnPre_Click(object sender, EventArgs e)
        {
            DataTable table = QueryPager.Pre();

            if (table == null) return;

            RefreshButton();

            if (DoPre != null)
            {
                DoPre(table);
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            DataTable table = QueryPager.Next();

            if (table == null) return;

            RefreshButton();

            if (DoNext != null)
            {
                DoNext(table);
            }
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            DataTable table = QueryPager.Last();

            if (table == null) return;

            RefreshButton();

            if (DoNext != null)
            {
                DoNext(table);
            }
        }

        private void RefreshButton()
        {
            QueryPager pager = QueryPager;

            if (pager.IsFrist() == true)
            {
                btnFrist.Enabled = false;
            }
            else
            {
                btnFrist.Enabled = true;
            }

            if (pager.IsLast() == true)
            {
                btnLast.Enabled = false;
            }
            else
            {
                btnLast.Enabled = true;
            }

            if (pager.HasNext() == true)
            {
                btnNext.Enabled = true;
            }
            else
            {
                btnNext.Enabled = false;
            }

            if (pager.HasPre() == true)
            {
                btnPre.Enabled = true;
            }
            else
            {
                btnPre.Enabled = false;
            }
        }
    }
}