using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using RFD.FMS.MODEL;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.COD;

namespace RFD.FMS.DAL.FinancialManage
{
    public class ExpressChangeDataDao : SqlServerDao, IExpressChangeDataDao
    {
        public bool UpdateIncomeSortingCenter(long waybillNo, int sortingCenterId)
        {
            string sql = "update LMS_RFD.dbo.FMS_IncomeBaseInfo set ExpressCompanyID=@ExpressCompanyID where WaybillNO=@WaybillNO";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@ExpressCompanyID",waybillNo),
                new SqlParameter("@WaybillNO",sortingCenterId)
            };

           return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }

        public bool UpdateIncomeDeliverStation(long waybillNo, int stationId, int topCompanyId)
        {
            string sql = @"update FMS_IncomeBaseInfo 
                set DeliverStationID=@DeliverStationID,TopCODCompanyID=@TopCODCompanyID
                where WaybillNo=@WaybillNo";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@DeliverStationID",stationId),
                new SqlParameter("@TopCODCompanyID",topCompanyId),
                new SqlParameter("@WaybillNo",waybillNo)
            };

           return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }

        public MODEL.COD.FMS_CODBaseInfo GetCODBaseInfo(long waybillNo)
        {
            throw new NotImplementedException();
        }

        public void CODDeductionFee(MODEL.COD.FMS_CODBaseInfo model)
        {
            throw new NotImplementedException();
        }

        public void AddCODModel(MODEL.COD.FMS_CODBaseInfo model)
        {
            throw new NotImplementedException();
        }

        public bool IsSendSuccess(long waybillNo)
        {
            throw new NotImplementedException();
        }

        public void DeleteDeduct(long waybillNo)
        {
            throw new NotImplementedException();
        }

        public bool UpdateExpressPaymentType(long waybillNo, int paymentType)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateExpressAccountType(long waybillNo, int accountType)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateIncomeAcceptType(long waybillNo, string acceptType)
        {
            throw new NotImplementedException();
            return false;
        }


        public bool UpdateCODBaseInfo(MODEL.COD.FMS_CODBaseInfo model)
        {
            throw new NotImplementedException();
            return false;
        }


        public bool UpdateExpressAcceptType(long waybillNo, string acceptType)
        {
            throw new NotImplementedException();
            return false;
        }


        public bool UpdateExpressDeliverFee(long waybillNo, decimal deliverFee)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateIncomeMerchantId(long waybillNo, int merchantId)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateIncomeProtectedPrices(long waybillNo, decimal protectedPrices)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateIncomeProtectFee(long waybillNo, decimal protectFee)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateExpressReceiveProtectFee(long waybillNo, decimal protectFee)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateCodProtectFee(long waybillNo, decimal protectFee)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateCODProtectedPrices(long waybillNo, decimal protectedPrices)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateIncomeGoodsPayment(long waybillNo, decimal goodsPayment)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateIncomeAccountWeight(long waybillNo, decimal accountWeight)
        {
            throw new NotImplementedException();
            return false;
        }

        public bool UpdateIncomeMerchantWeight(long waybillNo, decimal merchantWeight)
        {
            throw new NotImplementedException();
            return false;
        }
        public bool UpdateCodMerchantWeight(long waybillNo, decimal merchantWeight)
        {
            throw new NotImplementedException();
            return false;
        }
    }
}
