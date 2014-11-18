using System;
using System.Collections.Generic;
using System.ServiceModel;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.Service.FinancialManage
{
    [ServiceContract]
    public interface IFMSInterfaceForEDIService
    {
        /// <summary>
        /// 根据商家id和记账日期返回账单明细
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        [OperationContract]
        IList<ExternalIncomeAccountDetail> GetIncomeAccountDetail(int MerchantID, DateTime benginDate, DateTime endDate);
        /// <summary>
        /// 根据商家id和记账日期返回账单总表
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间 </param>
        /// <param name="endDate">结束时间 </param>
        /// <returns>查询结果</returns>
        [OperationContract]
        ExternalIncomeAccount GetIncomeAccount(int MerchantID, DateTime benginDate, DateTime endDate);
        /// <summary>
        /// 根据商家id、记账日期、运单类型返回账单明细
        /// </summary>
        /// <param name="MerchantID">商家id</param>
        /// <param name="benginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        ///0	普通订单
        ///1	上门换货单
        ///2	上门退货单
        ///3	签单返回
        /// <param name="BillType">运单类型组0,1,2,3</param>
        /// <returns></returns>
        [OperationContract]
        IList<ExternalIncomeAccountDetail> GetIncomeAccountDetailByBillType(int MerchantID, DateTime benginDate,
                                                                        DateTime endDate, List<int> BillType);

    }
}