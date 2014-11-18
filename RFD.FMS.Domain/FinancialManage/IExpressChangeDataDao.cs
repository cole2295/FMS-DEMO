using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Model;

namespace RFD.FMS.Domain.FinancialManage
{
    public interface IExpressChangeDataDao
    {
        /// <summary>
        /// 收入表更新接货分拣中心
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <param name="sortingCenterId">分拣中心</param>
        bool UpdateIncomeSortingCenter(long waybillNo, int sortingCenterId);

        /// <summary>
        /// 收入表更新配送站点和结算配送公司
        /// </summary>
        bool UpdateIncomeDeliverStation(long waybillNo, int stationId, int topCompanyId);

        /// <summary>
        /// 查询支出配送费实体
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <returns>配送费实体</returns>
        FMS_CODBaseInfo GetCODBaseInfo(long waybillNo);

        /// <summary>
        /// 插入一条支出配送费实体
        /// 抵消掉原配送费
        /// </summary>
        /// <param name="model">配送费实体</param>
        void CODDeductionFee(FMS_CODBaseInfo model);

        /// <summary>
        /// 新增支出配送费实体
        /// </summary>
        /// <param name="model">配送费实体</param>
        void AddCODModel(FMS_CODBaseInfo model);

        /// <summary>
        /// 是否已经妥投
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <returns>是否妥投</returns>
        bool IsSendSuccess(long waybillNo);

        /// <summary>
        /// 删除提成
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        void DeleteDeduct(long waybillNo);

        /// <summary>
        /// 修改付款方式：到付，现付
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <param name="paymentType">付款方式</param>
        bool UpdateExpressPaymentType(long waybillNo,int paymentType);

        /// <summary>
        /// 修改结算方式(1现结，3月结)
        /// </summary>
        /// <param name="waybillNo">运单号</param>
        /// <param name="accountType">结算方式</param>
        bool UpdateExpressAccountType(long waybillNo,int accountType);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="waybillNo"></param>
        /// <param name="acceptType"></param>
        bool UpdateIncomeAcceptType(long waybillNo,string acceptType);

        bool UpdateCODBaseInfo(FMS_CODBaseInfo model);

        bool UpdateExpressAcceptType(long waybillNo, string acceptType);

        bool UpdateExpressDeliverFee(long waybillNo, decimal deliverFee);

        bool UpdateIncomeMerchantId(long waybillNo, int merchantId);

        bool UpdateIncomeProtectedPrices(long waybillNo, decimal protectedPrices);

        bool UpdateIncomeProtectFee(long waybillNo, decimal protectFee);

        bool UpdateExpressReceiveProtectFee(long waybillNo, decimal protectFee);

        bool UpdateCodProtectFee(long waybillNo, decimal protectFee);

        bool UpdateCODProtectedPrices(long waybillNo, decimal protectedPrices);

        bool UpdateIncomeGoodsPayment(long waybillNo, decimal goodsPayment);

        bool UpdateIncomeAccountWeight(long waybillNo, decimal accountWeight);

        bool UpdateIncomeMerchantWeight(long waybillNo, decimal merchantWeight);

        bool UpdateCodMerchantWeight(long waybillNo, decimal merchantWeight);


    }
}
