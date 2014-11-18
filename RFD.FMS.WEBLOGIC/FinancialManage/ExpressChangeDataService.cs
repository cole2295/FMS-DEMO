using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.FinancialManage;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Model;
using RFD.FMS.Util;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.WEBLOGIC.FinancialManage
{
    public class ExpressChangeDataService : IExpressChangeDataService
    {
        private IExpressChangeDataDao Dao;

        public bool ChangeSortingCenter(long waybillNo, int sortingCenterId)
        {
            throw new NotImplementedException();
            return false;
            //FMS_IncomeBaseInfo修改ExpressCompanyID分拣中心
            //Dao.UpdateIncomeSortingCenter(waybillNo, sortingCenterId);

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            //incomeService.UpdateEvalStatus(waybillNo);

            //薛毅提供接口
        }

        public bool ChangeDeliverStation(long waybillNo, int stationId)
        {
            throw new NotImplementedException();
            return false;
            //var companyService = ServiceLocator.GetService<IExpressCompanyDao>();

            //ExpressCompany expressModel = companyService.GetModel(stationId);

            ////FMS_IncomeBaseInfo修改DeliverStationID，TopCODCompanyID

            //Dao.UpdateIncomeDeliverStation(waybillNo,stationId,expressModel.TopCODCompanyID);

            ////FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            //incomeService.UpdateEvalStatus(waybillNo);

            //FMS_CODBaseInfo把原纪录冲掉，新生成一条纪录。
            //FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);

            //Dao.CODDeductionFee(model);

            //model.DeliverStationID = stationId;
            //model.TopCODCompanyID = expressModel.TopCompanyId;

            //Dao.AddCODModel(model);

            //重新计算提成

            //bool isSendSuccess = Dao.IsSendSuccess(waybillNo);

            //if (isSendSuccess)
            //{
            //    Dao.DeleteDeduct(waybillNo);
            //}

            //薛毅提供接口
        }

        public bool ChangePaymentType(long waybillNo, int paymentType)
        {
            throw new NotImplementedException();
            return false;
            //修改快递收款查询表
            //Dao.UpdateExpressPaymentType(waybillNo,paymentType);
        }

        public bool ChangeAccountType(long waybillNo, int accountType)
        {
            throw new NotImplementedException();
            return false;
            //修改快递收款查询表
            //Dao.UpdateExpressAccountType(waybillNo,accountType);
        }

        public bool ChangeAcceptType(long waybillNo, int acceptType)
        {
            throw new NotImplementedException();
            return false;
            //FMS_IncomeBaseInfo修改AcceptType分拣中心
            //Dao.UpdateIncomeAcceptType(waybillNo,acceptType);

            //Dao.UpdateExpressAcceptType(waybillNo,acceptType);

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //Dao.ReEvalIncomeFee(waybillNo);
        }

        public bool ChangeDeliverFee(long waybillNo, decimal deliverFee)
        {
            throw new NotImplementedException();
            return false;
            //修改快递收款查询
            //Dao.UpdateExpressDeliverFee(waybillNo,deliverFee);
        }

        public bool ChangeMerchant(long waybillNo, int merchantId)
        {
            throw new NotImplementedException();
            return false;
            //收入直接改重新计算费用
            //Dao.UpdateIncomeMerchantId(waybillNo,merchantId);

            //Dao.ReEvalIncomeFee(waybillNo);

            //支出添加一条抵消，再生成一条重新计算费用
            //FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);

            //Dao.CODDeductionFee(model);

            //model.MerchantID = merchantId;

            //Dao.AddCODModel(model);
        }

        public bool ChangeProtectedPrices(long waybillNo, decimal protectedPrices)
        {
            throw new NotImplementedException();
            return false;
            //支出直接改
            //Dao.UpdateIncomeProtectedPrices(waybillNo,protectedPrices);

            //收入直接改重新计算配送费
            //Dao.UpdateCODProtectedPrices(waybillNo,protectedPrices);

            //Dao.ReEvalCodFee(waybillNo);
        }

        public bool ChangeGoodsPayment(long waybillNo, decimal goodsPayment)
        {
            throw new NotImplementedException();
            return false;
            //收入直接改重新计算费用
            //Dao.UpdateIncomeGoodsPayment(waybillNo,goodsPayment);

            //Dao.ReEvalIncomeFee(waybillNo);

            //支出添加一条抵消，再生成一条重新计算费用
            //FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);

            //Dao.CODDeductionFee(model);

            //model.NeedPayAmount = goodsPayment;

            //Dao.AddCODModel(model);
        }

        public bool ChangeAccountWeight(long waybillNo, decimal accountWeight)
        {
            throw new NotImplementedException();
            return false;
            //收入直接改重新计算
            //Dao.UpdateIncomeAccountWeight(waybillNo,accountWeight);

            //Dao.ReEvalIncomeFee(waybillNo);

            //修改提成

        }


        public bool ChangeAcceptType(long waybillNo, string AcceptType)
        {
            throw new NotImplementedException();
            return false;
        }
        public bool ChangeMerchantWeight(long waybillNo,decimal merchantWeight)
        {
            throw new NotImplementedException();
            return false;
        }
        public bool ChangeFactorage(long waybillNo,decimal Factorage)
        {
            throw new NotImplementedException();
            return false;
        }
        public bool ChangeProtectFee(long waybillNo, decimal protectFee)
        {
            throw new NotImplementedException();
            return false;
        }

    }
}
