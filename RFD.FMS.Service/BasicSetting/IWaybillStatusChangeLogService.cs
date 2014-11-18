using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IWaybillStatusChangeLogService
	{
        /// <summary>
        /// 根据编号获取实体
        /// </summary>
        /// <param name="id">日志编号</param>
        /// <returns>日志实体</returns>
        WaybillStatusChangeLog GetModel(long id);

        /// <summary>
        /// 更新状态日志为已同步
        /// </summary>
        /// <param name="ids">同步日志Id集合</param>
        /// <returns>是否成功</returns>
        bool UpdateSynStatus(long id);

        /// <summary>
        /// 查询需要同步的日志信息
        /// </summary>
        /// <param name="count">每次同步的条数</param>
        /// <param name="condition">查询条件</param>
        /// <returns>日志集合</returns>
        DataTable GetSynWaybillLogs(int count);

        /// <summary>
        /// 把row转换为object
        /// </summary>
        /// <param name="row">数据Row</param>
        /// <returns>对象</returns>
        WaybillStatusChangeLog DataRowToObject(DataRow row);

        /// <summary>
        /// 记录失败的推送日志
        /// </summary>
        /// <param name="logId">日志编号</param>
        /// <param name="key">接收程序标识</param>
        void RecordFailseLog(long logId,string key);

        /// <summary>
        /// 根据类key获取
        /// 重新推送日志
        /// </summary>
        /// <param name="classKey">类标识</param>
        /// <returns>失败的日志</returns>
        DataTable GetFailseLogByClassKey(string classKey);

        /// <summary>
        /// 更新失败的日志为失效
        /// </summary>
        /// <param name="id">日志Id</param>
        void UpdateFailseLog(long id);
	}
}
