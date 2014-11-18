using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IAreaDeductErrorInfoDao
    {
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        bool Exists(Int64 areadeducterrorinfoid);

        /// <summary>
        /// 增加一条数据
        /// </summary>
        int Add(AreaDeductErrorInfo model);

        /// <summary>
        /// 更新一条数据
        /// </summary>
        bool Update(AreaDeductErrorInfo model);

        /// <summary>
        /// 删除一条数据
        /// </summary>
        bool Delete(Int64 areadeducterrorinfoid);

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        AreaDeductErrorInfo GetModel(Int64 areadeducterrorinfoid);

        int GetDataTableCount(string searchString, Dictionary<string, object> searchParams);

        DataTable GetDataTable(Dictionary<string, object> searchParams);

        DataTable GetDataTable(string searchString, string sortColumn, Dictionary<string, object> searchParams);

        DataTable GetPageDataTable(string searchString, string sortColumn, Dictionary<string, object> searchParams, int rowStart, int rowEnd);

        /// <summary>
        /// 获取没有重新计算的区域提成异常信息
        /// </summary>
        /// <returns></returns>
        DataTable GetErrorAreaDeductListForService();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areadeducterrorinfoid"></param>
        /// <returns></returns>
        bool UpdateIsDeleted(Int64 areadeducterrorinfoid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areadeducterrorinfoid"></param>
        /// <returns></returns>
        bool UpdateErrorType(Int64 areadeducterrorinfoid);


        /// <summary>
        /// 获取重新计算的区域提成异常信息
        /// </summary>
        /// <returns></returns>
        DataTable GetErrorTypeAreaDeductListForService(int STATIONID);
        /// <summary>
        /// 修改异常原因为可计算
        /// </summary>
        /// <param name="areadeducterrorinfoid"></param>
        /// <returns></returns>
        bool UpdateErrorTypeForStat(Int64 areadeducterrorinfoid, int ErrorType);
    }
}
