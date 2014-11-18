using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;

namespace RFD.FMS.Service.FinancialManage
{
    public interface IExpressChangeDataService
    {
        /// <summary>
        /// 修改分拣中心
        /// </summary>
        bool ChangeSortingCenter(long waybillNo, int sortingCenterId);

        /// <summary>
        /// 修改站点
        /// </summary>
        bool ChangeDeliverStation(long waybillNo, int stationId);

        /// <summary>
        /// 修改付款方向(1寄方付,2到方付)
        /// </summary>
        bool ChangePaymentType(long waybillNo, int paymentType);

        /// <summary>
        /// 修改结算方式(1现结，3月结)
        /// </summary>
        bool ChangeAccountType(long waybillNo, int accountType);

        /// <summary>
        /// 修改支付方式(1现金,2POS机)
        /// </summary>
        bool ChangeAcceptType(long waybillNo, string AcceptType);

        /// <summary>
        /// 修改运费
        /// </summary>
        bool ChangeDeliverFee(long waybillNo, decimal deliverFee);

        /// <summary>
        /// 修改商家
        /// </summary>
        bool ChangeMerchant(long waybillNo, int merchantId);

        /// <summary>
        /// 修改保价金额
        /// </summary>
        bool ChangeProtectedPrices(long waybillNo, decimal protectedPrices);

        /// <summary>
        /// 修改代收货款
        /// </summary>
        bool ChangeGoodsPayment(long waybillNo, decimal goodsPayment);

        /// <summary>
        /// 修改取件重量
        /// </summary>
        bool ChangeAccountWeight(long waybillNo, decimal accountWeight);
        /// <summary>
        /// 修改保价费
        /// </summary>
        bool ChangeProtectFee(long waybillNo, decimal protectFee);
        /// <summary>
        /// 修改代收货款手续费
        /// </summary>
        bool ChangeFactorage(long waybillNo, decimal p);
        /// <summary>
        /// 修改客户重量
        /// </summary>
        bool ChangeMerchantWeight(long waybillNo, decimal p);

    }
}
