using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
//using RFD.LMS.ServiceImpl.FinancialManageService;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
    public class WaybillStatusDeal : IWaybillStatusDeal
    {
        private IWaybillStatusChangeLogService LogService;
        private IList<IWaybillStatusObserver> Observers;
        private int Count = 200;

        #region IWaybillStatusDeal 成员

        public void DoDeal()
        {
            IList<long> ids = new List<long>();

            DataTable table = LogService.GetSynWaybillLogs(Count);

            DataRow[] rows = null;

            DataRow row = null;

            WaybillStatusChangeLog model = null;

            foreach (var obServer in Observers)
            {
                rows = table.Select(obServer.GetSqlCondition());

                for (int i = 0; i < rows.Length; i++)
                {
                    row = rows[i];

                    model = LogService.DataRowToObject(row);

                    bool flag=obServer.DoAction(model);

                    if (flag == false && obServer.IsFalseToRePush == true)
                    {
                        LogService.RecordFailseLog(model.ID, obServer.Key);
                    }

                    if (ids.Contains(model.ID) == false)
                    {
                        ids.Add(model.ID);
                    }
                }
            }

            long id = -1;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToLong(row["ID"]);

                LogService.UpdateSynStatus(id);
            }

            ReDeal();
        }

        private void ReDeal()
        {
            try
            {
                WaybillStatusChangeLog model = null;

                foreach (var obServer in Observers)
                {
                    DataTable table = LogService.GetFailseLogByClassKey(obServer.Key);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        model = LogService.DataRowToObject(table.Rows[i]);

                        if (obServer.DoAction(model) == true)
                        {
                            LogService.UpdateFailseLog(DataConvert.ToLong(table.Rows[i]["RecordId"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RFD.FMS.WEBLOGIC.Mail.Mail mail = new RFD.FMS.WEBLOGIC.Mail.Mail();

                mail.SendMailToUser("财务推送失败日志重新推送错误", ex.Message + ex.StackTrace, "liyongf@rufengda.com;zengwei@vancl.cn");
            }
        }

        #endregion
    }
}
