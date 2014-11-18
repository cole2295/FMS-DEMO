using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.BasicSetting
{
    public interface IWaybillStatusObserver
	{
        /// <summary>
        /// 单条数据处理
        /// </summary>
        /// <param name="model">状态日志实体</param>
        /// <returns>是否成功</returns>
        bool DoAction(WaybillStatusChangeLog model);

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns>查询条件</returns>
        string GetSqlCondition();

        /// <summary>
        /// 推送失败是否重新推送
        /// </summary>
        bool IsFalseToRePush { get; }

        /// <summary>
        /// 标识推数类
        /// </summary>
        string Key { get; }
	}
}
