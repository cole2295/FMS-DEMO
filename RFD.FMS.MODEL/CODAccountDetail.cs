using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
	/// <summary>
	/// COD结算明细
	/// </summary>
	[Serializable]
	public class CODAccountDetail
	{
		private string accountno;
		private decimal _allowance;
		private decimal _kpi;
		private decimal _posprice;
		private decimal _strandedprice;
		private decimal _intercitylose;
		private decimal _othercost;
	    private decimal _collectionfee;
	    private decimal _deliveryfee;

		public string AccountNO
		{
			get { return accountno; }
			set { accountno = value; }
		}

		public decimal Allowance
		{
			get { return _allowance; }
			set { _allowance = value; }
		}

		public decimal KPI
		{
			get { return _kpi; }
			set { _kpi = value; }
		}

		public decimal POSPrice
		{
			get { return _posprice; }
			set { _posprice = value; }
		}

		public decimal StrandedPrice
		{
			get { return _strandedprice; }
			set { _strandedprice = value; }
		}

		public decimal IntercityLose
		{
			get { return _intercitylose; }
			set { _intercitylose = value; }
		}

		public decimal OtherCost
		{
			get { return _othercost; }
			set { _othercost = value; }
		}

	    public decimal CollectionFee
	    {
            get { return _collectionfee; }
            set { _collectionfee = value; }
	    }
	    public decimal DeliveryFee
	    {
            get { return _deliveryfee; }
            set { _deliveryfee = value; }
	    }
	}
}
