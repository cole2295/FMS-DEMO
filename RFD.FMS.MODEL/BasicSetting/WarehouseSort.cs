using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class WarehouseSort:BaseModel
    {
        public int ID { get; set; }
        public int SortationID { get; set; }
        public string WarehouseID { get; set; }
        public bool HasWarehouse { get; set; }
        public bool isExport { get; set; }
    }
}
