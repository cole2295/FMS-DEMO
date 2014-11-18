using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.DAL.FinancialManage;
using RFD.FMS.DAL.Oracle.BasicSetting;
using RFD.FMS.DAL.Oracle.FinancialManage;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Model;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.Service.COD;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.ServiceImpl.COD;
using RFD.FMS.Util;
using ExpressChangeDataDao = RFD.FMS.DAL.Oracle.FinancialManage.ExpressChangeDataDao;
using IncomeBaseInfoDao = RFD.FMS.DAL.Oracle.FinancialManage.IncomeBaseInfoDao;

namespace RFD.FMS.ServiceImpl.FinancialManage
{
    public class ExpressChangeDataService : IExpressChangeDataService
    {
        //private IExpressChangeDataDao Dao;
        IExpressChangeDataDao Dao=new ExpressChangeDataDao();
        IIncomeBaseInfoDao incomeService=new IncomeBaseInfoDao();
        ICODBaseInfoService codService = new CODBaseInfoService();

        public bool ChangeSortingCenter(long waybillNo, int sortingCenterId)
        {
            //FMS_IncomeBaseInfo修改ExpressCompanyID分拣中心
            if (Dao.UpdateIncomeSortingCenter(waybillNo, sortingCenterId)==false) return false ;

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            if (incomeService.UpdateEvalStatus(waybillNo)==false) return false;
   
            return true;

        }

        public bool ChangeDeliverStation(long waybillNo, int stationId)
        {
            //var companyService = ServiceLocator.GetService<IExpressCompanyDao>();
            IExpressCompanyDao companyService=new ExpressCompanyDao();
            ExpressCompany expressModel = companyService.GetModel(stationId);

            //FMS_IncomeBaseInfo修改DeliverStationID，TopCODCompanyID

            if(Dao.UpdateIncomeDeliverStation(waybillNo, stationId, expressModel.TopCODCompanyID)==false) return false;

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            if (incomeService.UpdateEvalStatus(waybillNo) == false) return false;

            //FMS_CODBaseInfo把原纪录冲掉，新生成一条纪录。
            FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);
            if (model.DeliverTime == null)
            {
                //如果不存在发货记录，更新原数据
                model.TopCODCompanyID = expressModel.TopCODCompanyID;
                model.DeliverStationID = stationId;
                model.IsFare = 0;
                model.Fare = 0;
                model.FareFormula = "";

                if(Dao.UpdateCODBaseInfo(model)==false) return false;
            }
            else
            {
                //如果存在发货记录，增加一条原单拒收记录冲抵，新增发货记录
                int flag = model.Flag;
                int? operateType = model.OperateType;
                //var codService = ServiceLocator.GetService<ICODBaseInfoService>();

                model.Flag = 0;
                model.OperateType = 2;
                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.DeliverTime = DateTime.Now;
                model.DeliverDate = DateTime.Now;
                model.RfdAcceptTime = DateTime.Now;
                model.RfdAcceptTime = DateTime.Now;
                model.ReturnDate = DateTime.Now.ToShortDateString();
                model.ReturnTime = DateTime.Now;
                codService.Add(model);

                model.Flag = flag;
                model.OperateType = operateType;
                model.TopCODCompanyID = expressModel.TopCODCompanyID;
                model.DeliverStationID = stationId;
                model.IsFare = 0;
                model.Fare = 0;
                model.FareFormula = "";
                if(codService.Add(model)==0)return false;
            }

            return true;


            // Dao.CODDeductionFee(model);

            //model.DeliverStationID = stationId;
            //model.TopCODCompanyID = expressModel.TopCompanyId;

            //Dao.AddCODModel(model);

            //重新计算提成

            //bool isSendSuccess = Dao.IsSendSuccess(waybillNo);

            ////if (isSendSuccess)
            ////{
            //    Dao.DeleteDeduct(waybillNo);
            ////}

            //薛毅提供接口
        }

        public bool ChangePaymentType(long waybillNo, int paymentType)
        {
            //修改快递收款查询表
            IFMS_ReceiveFeeInfoDao oracleDao = new FMS_ReceiveFeeInfoDao();
            bool isExists = oracleDao.ExistsExpressReceiveFeeInfo(waybillNo);
            if (isExists==true)
                return Dao.UpdateExpressPaymentType(waybillNo, paymentType);
            FMS_IncomeExpressReceiveFeeInfo receiveModel = GetExpressModel(waybillNo);
            receiveModel.TransferPayType = paymentType;
            return oracleDao.AddExpressReceiveFeeInfo(receiveModel);
        }

        public bool ChangeAccountType(long waybillNo, int accountType)
        {
            //修改快递收款查询表 
            IFMS_ReceiveFeeInfoDao oracleDao = new FMS_ReceiveFeeInfoDao();
            bool isExists = oracleDao.ExistsExpressReceiveFeeInfo(waybillNo);
            if (isExists == true)
                return Dao.UpdateExpressAccountType(waybillNo, accountType);
            FMS_IncomeExpressReceiveFeeInfo receiveModel = GetExpressModel(waybillNo);
            receiveModel.TransferPayType = accountType;
            return oracleDao.AddExpressReceiveFeeInfo(receiveModel);

           // if (Dao.UpdateExpressAccountType(waybillNo, accountType) == false) return false;

           //return true;
        }

        public bool ChangeAcceptType(long waybillNo, string acceptType)
        {

            //FMS_IncomeBaseInfo修改AcceptType分拣中心
            if (Dao.UpdateIncomeAcceptType(waybillNo, acceptType)==false) return false;
            IFMS_ReceiveFeeInfoDao oracleDao = new FMS_ReceiveFeeInfoDao();
            bool isExists = oracleDao.ExistsExpressReceiveFeeInfo(waybillNo);
            if (isExists == true)
            {
                if (Dao.UpdateExpressAcceptType(waybillNo, acceptType) == false) return false;
            }
            else
            {
                FMS_IncomeExpressReceiveFeeInfo receiveModel = GetExpressModel(waybillNo);
                receiveModel.AcceptType = acceptType;
                if (oracleDao.AddExpressReceiveFeeInfo(receiveModel)==false)return false;
            }
            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //Dao.ReEvalIncomeFee(waybillNo);
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            if(incomeService.UpdateEvalStatus(waybillNo)==false) return false; 

            return true;

        }

        public bool ChangeDeliverFee(long waybillNo, decimal deliverFee)
        {
            //修改快递收款查询

            if (Dao.UpdateExpressDeliverFee(waybillNo, deliverFee) == false) return false;

            return true;

        }

        public bool ChangeMerchant(long waybillNo, int merchantId)
        {
           
            //收入直接改重新计算费用
            // Dao.UpdateIncomeMerchantId(waybillNo,merchantId);
            // Dao.ReEvalIncomeFee(waybillNo);

            if(Dao.UpdateIncomeMerchantId(waybillNo, merchantId)==false)return false;
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();
            if(incomeService.UpdateEvalStatus(waybillNo)==false)return false;
            FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);
            if (model.DeliverTime == null)
            {
                //如果不存在发货记录，更新原数据
                //  model.TopCODCompanyID = expressModel.TopCODCompanyID;
                //  model.DeliverStationID = expressModel.ExpressCompanyID;
                model.IsFare = 0;
                model.Fare = 0;
                model.FareFormula = "";
                model.MerchantID = merchantId;

                if (Dao.UpdateCODBaseInfo(model)==false)return false;
            }
            else
            {
                //如果存在发货记录，增加一条原单拒收记录冲抵，新增发货记录
                int flag = model.Flag;
                int? operateType = model.OperateType;
                //var codService = ServiceLocator.GetService<ICODBaseInfoService>();

                model.Flag = 0;
                model.OperateType = 2;
                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.DeliverTime = DateTime.Now;
                model.DeliverDate = DateTime.Now;
                model.RfdAcceptTime = DateTime.Now;
                model.RfdAcceptTime = DateTime.Now;
                model.ReturnDate = DateTime.Now.ToShortDateString();
                model.ReturnTime = DateTime.Now;
                codService.Add(model);

                model.Flag = flag;
                model.OperateType = operateType;
                model.MerchantID = merchantId;
                model.IsFare = 0;
                model.Fare = 0;
                model.FareFormula = "";
                if(codService.Add(model)==0)return false;
            }

            return true;
            //支出添加一条抵消，再生成一条重新计算费用
            //FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);

            //Dao.CODDeductionFee(model);

            //model.MerchantID = merchantId;

            //Dao.AddCODModel(model);
            //FMS_CODBaseInfo把原纪录冲掉，新生成一条纪录。

        }

        public bool ChangeProtectedPrices(long waybillNo, decimal protectedPrices)
        {

                //支出直接改
                if(Dao.UpdateCODProtectedPrices(waybillNo, protectedPrices)==false)return false;

                //收入直接改重新计算配送费
                //Dao.UpdateIncomeProtectedPrices(waybillNo,protectedPrices);
                if (Dao.UpdateIncomeProtectedPrices(waybillNo, protectedPrices)==false) return false;
                //Dao.ReEvalCodFee(waybillNo);
                //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();
                if(incomeService.UpdateEvalStatus(waybillNo)==false)return false;

            return true;

        }

        public bool ChangeProtectFee(long waybillNo, decimal protectFee)
        {

            // 改incomebaseinfo 改保价金额,incomefeeinfo 改计算状态 isaccount  
            //codbaseinfo 改保价金额，  expressreceive 改保价金额DinsureFee
            if (Dao.UpdateIncomeProtectFee(waybillNo, protectFee)==false)return false;
            //incomefeeinfo 改计算状态
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            if(incomeService.UpdateEvalStatus(waybillNo)==false)return false;

            if(Dao.UpdateCodProtectFee(waybillNo, protectFee)==false)return false;

            if(Dao.UpdateExpressReceiveProtectFee(waybillNo, protectFee)==false)return false;

            return true;
        }

        public bool ChangeGoodsPayment(long waybillNo, decimal goodsPayment)
        {

            //收入直接改重新计算费用
            if(Dao.UpdateIncomeGoodsPayment(waybillNo, goodsPayment)==false)return false;

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            if(incomeService.UpdateEvalStatus(waybillNo)==false)return false;
            //FMS_CODBaseInfo把原纪录冲掉，新生成一条纪录。
            FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);
            if (model.DeliverTime == null)
            {
                //如果不存在发货记录，更新原数据
                model.NeedPayAmount = goodsPayment;
                model.IsFare = 0;
                model.Fare = 0;
                model.FareFormula = "";

                if(Dao.UpdateCODBaseInfo(model)==false)return false;
            }
            else
            {
                //如果存在发货记录，增加一条原单拒收记录冲抵，新增发货记录
                int flag = model.Flag;
                int? operateType = model.OperateType;
                //var codService = ServiceLocator.GetService<ICODBaseInfoService>();

                model.Flag = 0;
                model.OperateType = 2;
                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.DeliverTime = DateTime.Now;
                model.DeliverDate = DateTime.Now;
                model.RfdAcceptTime = DateTime.Now;
                model.RfdAcceptTime = DateTime.Now;
                model.ReturnDate = DateTime.Now.ToShortDateString();
                model.ReturnTime = DateTime.Now;
                codService.Add(model);

                model.Flag = flag;
                model.OperateType = operateType;
                model.NeedPayAmount = goodsPayment;
                model.IsFare = 0;
                model.Fare = 0;
                model.FareFormula = "";
                if (codService.Add(model)==0)return false;
            }

            return true;

            //Dao.ReEvalIncomeFee(waybillNo);

            //支出添加一条抵消，再生成一条重新计算费用
            //FMS_CODBaseInfo model = Dao.GetCODBaseInfo(waybillNo);

            //Dao.CODDeductionFee(model);

            //model.NeedPayAmount = goodsPayment;

            //Dao.AddCODModel(model);
        }

        public bool ChangeFactorage(long waybillNo,decimal Factorage)
        {

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            if(incomeService.UpdateEvalStatus(waybillNo)==false)return false;

            return true;
        }

        public bool ChangeAccountWeight(long waybillNo, decimal accountWeight)
        {

            //收入直接改重新计算
            //Dao.UpdateIncomeAccountWeight(waybillNo,accountWeight);
            //Dao.ReEvalIncomeFee(waybillNo);
            if(Dao.UpdateIncomeAccountWeight(waybillNo, accountWeight)==false)return false;

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            if (incomeService.UpdateEvalStatus(waybillNo) == false) return false;

            return true;

        }

        public bool ChangeMerchantWeight(long waybillNo,decimal merchantWeight)
        {

            // 改incomebaseinfo 改重量,incomefeeinfo 改计算状态 isaccount
            //  codbaseinfo 改重量，  
            if(Dao.UpdateIncomeMerchantWeight(waybillNo, merchantWeight)==false)return false;

            //FMS_IncomeFeeInfo修改IsAccount=0重新计算费用
            //var incomeService = ServiceLocator.GetService<IIncomeBaseInfoDao>();

            if(incomeService.UpdateEvalStatus(waybillNo)==false)return false;

            if(Dao.UpdateCodMerchantWeight(waybillNo, merchantWeight)==false)return false;

            return true;
        }
        private FMS_IncomeExpressReceiveFeeInfo GetExpressModel(long waybillno)
        {
            IReceiveFeeInfoDao sqlServerDao = new ReceiveFeeInfoDao();
            FMS_IncomeExpressReceiveFeeInfo model = sqlServerDao.GetExpressModel(waybillno);
            return model;

        }
    }
}
