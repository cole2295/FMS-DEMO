using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface IWaybillStatusChangeLogDao
	{
        /// <summary>
        /// 获取需要同步的日志集合
        /// </summary>
        /// <param name="count">一次查询需要同步的数据条数</param>
        /// <returns>日志集合</returns>
        DataTable GetSynWaybillLogs(int count);

        WaybillStatusChangeLog DataRowToObject(DataRow row);

        /// <summary>
        /// 更新状态标识为已同步
        /// </summary>
        /// <param name="ids">编号集合</param>
        /// <returns></returns>
        bool UpdateSynStatus(long id);

        /// <summary>
        /// 根据主键集合获取状态变更日志
        /// </summary>
        /// <param name="ids">主键集合</param>
        /// <returns>变更日志</returns>
        List<WaybillStatusChangeLog> GetWaybillStatus(List<string> ids);

        /// <summary>
        /// 记录推送失败的记录
        /// </summary>
        /// <param name="logId">日志编号</param>
        /// <param name="key">接收类标识</param>
        void RecordFailseLog(long logId,string key);

        /// <summary>
        /// 根据类标识获取推送失败的日志
        /// </summary>
        /// <param name="classkey">推送类标识</param>
        /// <returns>失败日志信息</returns>
        DataTable GetFailseLogByClassKey(string classkey);

        /// <summary>
        /// 更新推送成功标识
        /// </summary>
        /// <param name="id">日志编号</param>
        void UpdateFailseLog(long id);
	}
}
