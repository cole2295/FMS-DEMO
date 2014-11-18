using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class WaybillStatusChangeLogService : IWaybillStatusChangeLogService
	{
        private IWaybillStatusChangeLogDao Dao;

        #region IWaybillStatusChangeLogService 成员

        public WaybillStatusChangeLog GetModel(long id)
        {
            List<string> ids = new List<string>();

            ids.Add(id.ToString());

            List<WaybillStatusChangeLog> models = GetWaybillStatusById(ids);

            if (models.Count > 0) return models[0];

            return null;
        }

        public bool UpdateSynStatus(long id)
        {
            return Dao.UpdateSynStatus(id);
        }

        public DataTable GetSynWaybillLogs(int count)
        {
            return Dao.GetSynWaybillLogs(count);
        }

        public WaybillStatusChangeLog DataRowToObject(DataRow row)
        {
            return Dao.DataRowToObject(row);
        }

        public List<WaybillStatusChangeLog> GetWaybillStatusById(List<string> ids)
        {
            return Dao.GetWaybillStatus(ids);
        }

        public void RecordFailseLog(long logId, string key)
        {
            Dao.RecordFailseLog(logId,key);
        }

        public DataTable GetFailseLogByClassKey(string classKey)
        {
            return Dao.GetFailseLogByClassKey(classKey);
        }

        public void UpdateFailseLog(long id)
        {
            Dao.UpdateFailseLog(id);
        }

	    #endregion
    }
}
