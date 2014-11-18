using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.COD
{
    public class LMS_Syn_FMS_COD
    {
        private long _waybillNo;
        public long WayBillNO
        {
            set { this._waybillNo = value; }
            get { return this._waybillNo; }
        }

        private int? _operateType;
        public int? OperateType
        {
            set { this._operateType = value; }
            get { return this._operateType; }
        }

        private int? _stationID;
        public int? StationID
        {
            set { this._stationID = value; }
            get { return this._stationID; }
        }

        private DateTime? _cperateTime;
        public DateTime? OperateTime
        {
            set { this._cperateTime = value; }
            get { return this._cperateTime; }
        }

        private string _createBy;
        public string CreateBY
        {
            set { this._createBy = value; }
            get { return this._createBy; }
        }


    }
}
