using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using System.Data;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class WarehouseSortRelationService : IWarehouseSortRelationService
    {
        /// <summary>
        /// 实例化数据层
        /// </summary>
        private IWarehouseSortRelationDao _warehouseSortRelationDao;
        public WarehouseSortRelationService()
        {

        }

		public DataTable GetWarehouseSortRelation(WarehouseSort model)
		{
            return _warehouseSortRelationDao.GetWarehouseSortRelation(model);
		}
        public DataTable GetSortationList(string DistributionCode)
		{
            return _warehouseSortRelationDao.GetSortationList(DistributionCode);
		}
		public DataTable GetWarehouseList()
		{
            return _warehouseSortRelationDao.GetWarehouseList();
		}

		public bool AddWarehouseSortRelation(WarehouseSort model)
		{
            return _warehouseSortRelationDao.InsertWarehouseSortRelation(model);
		}

		public bool EnableWarehouseSortRelation(WarehouseSort model)
		{
            return _warehouseSortRelationDao.UpdateWarehouseSortRelation(model);
		}

		public string GetWarehouseBySortation(string sortid)
		{
            return _warehouseSortRelationDao.GetWarehouseBySortation(sortid);
		}
        /// <summary>
        /// 通过Vjia仓库ID得到如风达仓库ID add by wangyongc 2012-03-02
        /// </summary>
        /// <param name="VjiaID">Vjia仓库ID</param>
        /// <returns></returns>
        public string GetWarehouseIDByVjiaID(string VjiaID)
        {
            return _warehouseSortRelationDao.GetWarehouseIDByVjiaID(VjiaID);
        }

    }
}
