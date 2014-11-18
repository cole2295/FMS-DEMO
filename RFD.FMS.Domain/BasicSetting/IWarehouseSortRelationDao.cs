using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IWarehouseSortRelationDao
    {
        DataTable GetWarehouseSortRelation(WarehouseSort model);
        DataTable GetSortationList(string DistributionCode);
        DataTable GetWarehouseList();
        bool InsertWarehouseSortRelation(WarehouseSort model);
        bool UpdateWarehouseSortRelation(WarehouseSort model);
        string GetWarehouseBySortation(string sortid);

        /// <summary>
        /// 通过Vjia仓库ID得到如风达仓库ID add by wangyongc 2012-03-02
        /// </summary>
        /// <param name="VjiaID">Vjia仓库ID</param>
        /// <returns></returns>
        string GetWarehouseIDByVjiaID(string VjiaID);
    }
}
