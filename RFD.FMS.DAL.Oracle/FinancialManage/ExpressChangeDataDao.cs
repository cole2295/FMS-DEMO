using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Oracle.ApplicationBlocks.Data;
using Oracle.DataAccess.Client;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.COD;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class ExpressChangeDataDao :OracleDao, IExpressChangeDataDao
    {
        public bool UpdateIncomeSortingCenter(long waybillNo, int sortingCenterId)
        {
            string sql = "update FMS_IncomeBaseInfo set ExpressCompanyID=:sortingCenterId ,UpdateTime = sysDate where WaybillNO=:WaybillNO";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":sortingCenterId",OracleDbType.Int32){Value = sortingCenterId},
                new OracleParameter(":WaybillNO",OracleDbType.Int64){Value = waybillNo}
            };

          return  OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }

        public bool UpdateIncomeDeliverStation(long waybillNo, int stationId, int topCompanyId)
        {
            string sql = @"update FMS_IncomeBaseInfo 
                set DeliverStationID=:DeliverStationID,TopCODCompanyID=:TopCODCompanyID,UpdateTime = sysDate
                where WaybillNo=:WaybillNo";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":DeliverStationID",OracleDbType.Int32){Value = stationId},
                new OracleParameter(":TopCODCompanyID",OracleDbType.Int32){Value = topCompanyId},
                new OracleParameter(":WaybillNo",OracleDbType.Int64){Value = waybillNo}
            };

           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }

        public MODEL.COD.FMS_CODBaseInfo GetCODBaseInfo(long waybillNo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT fci.INFOID as ID, fci.MediumID, fci.WaybillNO, fci.MerchantID, 
                                fci.WaybillType, fci.Flag, fci.DeliverStationID, fci.TopCODCompanyID, 
                                fci.WarehouseId, fci.ExpressCompanyID, fci.RfdAcceptTime, 
                                fci.RfdAcceptDate, fci.FinalExpressCompanyID, fci.DeliverTime, 
                                fci.DeliverDate, fci.ReturnWareHouseID, fci.ReturnExpressCompanyID, 
                                fci.TotalAmount, fci.PaidAmount, fci.NeedPayAmount, 
                                fci.BackAmount, fci.NeedBackAmount, fci.AccountWeight, 
                                fci.AreaID, fci.AreaType, fci.BoxsNo, fci.Address, 
                                fci.CreateTime, fci.UpdateTime, fci.IsDeleted, fci.ReturnTime, fci.ReturnDate, 
                                fci.IsFare, fci.Fare, fci.FareFormula, fci.OperateType, fci.ProtectedPrice, 
                                fci.DistributionCode, fci.CurrentDistributionCode 
                            FROM FMS_CODBaseInfo fci 
                            WHERE fci.WaybillNo =:WaybillNo and fci.OperateType in (1,3)
                                ORDER BY fci.INFOID DESC");

            var sqlParams = new List<OracleParameter>()
			{
				new OracleParameter(string.Format(":{0}","WaybillNo"),waybillNo)
			};

            var model = new FMS_CODBaseInfo();

            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());

            if (ds.Tables[0].Rows.Count > 0)
            {
                model = GetCODBaseInfoModel(ds.Tables[0].Rows[0]);//一条
            }

            return model;
        }

        private FMS_CODBaseInfo GetCODBaseInfoModel(DataRow row)
        {
            var model = new FMS_CODBaseInfo();
            if (row["ID"].ToString() != "")
            {
                model.ID = Int64.Parse(row["ID"].ToString());
            }
            if (row["MediumID"].ToString() != "")
            {
                model.MediumID = Int64.Parse(row["MediumID"].ToString());
            }
            if (row["WaybillNO"].ToString() != "")
            {
                model.WaybillNO = Int64.Parse(row["WaybillNO"].ToString());
            }
            if (row["MerchantID"].ToString() != "")
            {
                model.MerchantID = Int32.Parse(row["MerchantID"].ToString());
            }
            model.WaybillType = row["WaybillType"].ToString();
            if (row["Flag"].ToString() != "")
            {
                model.Flag = Int32.Parse(row["Flag"].ToString());
            }
            if (row["DeliverStationID"].ToString() != "")
            {
                model.DeliverStationID = Int32.Parse(row["DeliverStationID"].ToString());
            }
            if (row["TopCODCompanyID"].ToString() != "")
            {
                model.TopCODCompanyID = Int32.Parse(row["TopCODCompanyID"].ToString());
            }
            model.WarehouseId = row["WarehouseId"].ToString();
            if (row["ExpressCompanyID"].ToString() != "")
            {
                model.ExpressCompanyID = Int32.Parse(row["ExpressCompanyID"].ToString());
            }
            if (row["RfdAcceptTime"].ToString() != "")
            {
                model.RfdAcceptTime = System.DateTime.Parse(row["RfdAcceptTime"].ToString());
            }
            if (row["RfdAcceptDate"].ToString() != "")
            {
                model.RfdAcceptDate = System.DateTime.Parse(row["RfdAcceptDate"].ToString());
            }
            if (row["FinalExpressCompanyID"].ToString() != "")
            {
                model.FinalExpressCompanyID = Int32.Parse(row["FinalExpressCompanyID"].ToString());
            }
            if (row["DeliverTime"].ToString() != "")
            {
                model.DeliverTime = System.DateTime.Parse(row["DeliverTime"].ToString());
            }
            if (row["DeliverDate"].ToString() != "")
            {
                model.DeliverDate = System.DateTime.Parse(row["DeliverDate"].ToString());
            }
            model.ReturnWareHouseID = row["ReturnWareHouseID"].ToString();
            if (row["ReturnExpressCompanyID"].ToString() != "")
            {
                model.ReturnExpressCompanyID = Int32.Parse(row["ReturnExpressCompanyID"].ToString());
            }
            if (row["TotalAmount"].ToString() != "")
            {
                model.TotalAmount = System.Decimal.Parse(row["TotalAmount"].ToString());
            }
            if (row["PaidAmount"].ToString() != "")
            {
                model.PaidAmount = System.Decimal.Parse(row["PaidAmount"].ToString());
            }
            if (row["NeedPayAmount"].ToString() != "")
            {
                model.NeedPayAmount = System.Decimal.Parse(row["NeedPayAmount"].ToString());
            }
            if (row["BackAmount"].ToString() != "")
            {
                model.BackAmount = System.Decimal.Parse(row["BackAmount"].ToString());
            }
            if (row["NeedBackAmount"].ToString() != "")
            {
                model.NeedBackAmount = System.Decimal.Parse(row["NeedBackAmount"].ToString());
            }
            if (row["AccountWeight"].ToString() != "")
            {
                model.AccountWeight = System.Decimal.Parse(row["AccountWeight"].ToString());
            }
            model.AreaID = row["AreaID"].ToString();
            if (row["AreaType"].ToString() != "")
            {
                model.AreaType = Int32.Parse(row["AreaType"].ToString());
            }
            model.BoxsNo = row["BoxsNo"].ToString();
            model.Address = row["Address"].ToString();
            if (row["CreateTime"].ToString() != "")
            {
                model.CreateTime = System.DateTime.Parse(row["CreateTime"].ToString());
            }
            if (row["UpdateTime"].ToString() != "")
            {
                model.UpdateTime = System.DateTime.Parse(row["UpdateTime"].ToString());
            }
            if (row["IsDeleted"].ToString() != "")
            {
                if ((row["IsDeleted"].ToString() == "1") || (row["IsDeleted"].ToString().ToLower() == "true"))
                {
                    model.IsDeleted = true;
                }
                else
                {
                    model.IsDeleted = false;
                }
            }
            if (row["ReturnTime"].ToString() != "")
            {
                model.ReturnTime = System.DateTime.Parse(row["ReturnTime"].ToString());
            }
            if (row["ReturnDate"].ToString() != "")
            {
                model.ReturnDate = row["ReturnDate"].ToString();
            }
            if (row["IsFare"].ToString() != "")
            {
                model.IsFare = Int32.Parse(row["IsFare"].ToString());
            }
            if (row["Fare"].ToString() != "")
            {
                model.Fare = System.Decimal.Parse(row["Fare"].ToString());
            }
            model.FareFormula = row["FareFormula"].ToString();
            if (row["OperateType"].ToString() != "")
            {
                model.OperateType = Int32.Parse(row["OperateType"].ToString());
            }
            if (row["ProtectedPrice"].ToString() != "")
            {
                model.ProtectedPrice = System.Decimal.Parse(row["ProtectedPrice"].ToString());
            }
            model.DistributionCode = row["DistributionCode"].ToString();
            model.CurrentDistributionCode = row["CurrentDistributionCode"].ToString();

            return model;
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
            
            string sql =
                 @"Update FMS_ExpressReceiveFeeInfo set TransferPayType=:paymentType,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":paymentType", OracleDbType.Int32)
                                                  {Value = paymentType},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }

        public bool UpdateExpressAccountType(long waybillNo, int accountType)
        {
            string sql =
                @"Update FMS_ExpressReceiveFeeInfo set TransferPayType=:accountType,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":accountType", OracleDbType.Int32)
                                                  {Value = accountType},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;

        }
        public bool UpdateIncomeMerchantId(long waybillNo,int merchantId)
        {
            string sql =
                @"Update FMS_IncomeBaseInfo set MerchantID=:merchantId,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":merchantId", OracleDbType.Int32)
                                                  {Value = merchantId},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }
        public bool UpdateIncomeProtectFee(long waybillNo, decimal protectFee)
        {
            string sql =
             @"Update FMS_IncomeBaseInfo set ProtectedAmount=:protectFee,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":protectFee", OracleDbType.Decimal)
                                                  {Value = protectFee},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }
        public bool UpdateCodProtectFee(long waybillNo, decimal protectFee)
        {
            string sql =
            @"Update FMS_CodBaseInfo set ProtectedPrice=:protectFee,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":protectFee", OracleDbType.Decimal)
                                                  {Value = protectFee},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }
        public bool UpdateExpressReceiveProtectFee(long waybillNo, decimal protectFee)
        {
            string sql =
            @"Update FMS_ExpressReceiveFeeInfo set DinsureFee=:protectFee,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":protectFee", OracleDbType.Decimal)
                                                  {Value = protectFee},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;  
        }

        public bool UpdateIncomeProtectedPrices(long waybillNo, decimal protectedPrices)
        {
            string sql =
                @"Update FMS_IncomeBaseInfo set ProtectedAmount=:protectedPrices,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":protectedPrices", OracleDbType.Decimal)
                                                  {Value = protectedPrices},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }
        public bool UpdateIncomeMerchantWeight(long waybillNo,decimal merchantWeight)
        {
            string sql =
            @"Update FMS_IncomeBaseInfo set AccountWeight=:merchantWeight,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":merchantWeight", OracleDbType.Decimal)
                                                  {Value = merchantWeight},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }
        public bool UpdateCodMerchantWeight(long waybillNo,decimal merchantWeight)
        {
            string sql =
             @"Update FMS_CodBaseInfo set AccountWeight=:merchantWeight,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":merchantWeight", OracleDbType.Decimal)
                                                  {Value = merchantWeight},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
          return  OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;  
        }


        public bool UpdateIncomeGoodsPayment(long waybillNo, decimal goodsPayment)
        {
            string sql =
            @"Update FMS_IncomeBaseInfo set NeedPayAmount=:goodsPayment,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":goodsPayment", OracleDbType.Decimal)
                                                  {Value = goodsPayment},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }
        public  bool UpdateIncomeAccountWeight(long waybillNo, decimal accountWeight)
        {
            string sql =
           @"Update FMS_IncomeBaseInfo set AccountWeight=:accountWeight,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":accountWeight", OracleDbType.Decimal)
                                                  {Value = accountWeight},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0; 
        }
        public bool UpdateCODProtectedPrices(long waybillNo, decimal protectedPrices)
        {
            string sql =
                @"Update FMS_CodBaseInfo set ProtectedPrice=:protectedPrices,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":protectedPrices", OracleDbType.Decimal)
                                                  {Value = protectedPrices},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }


        public bool UpdateExpressDeliverFee(long waybillNo, decimal deliverFee)
        {
            string sql =
                @"Update FMS_ExpressReceiveFeeInfo set TransferFee=:deliverFee,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":deliverFee", OracleDbType.Decimal)
                                                  {Value = deliverFee},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
          return  OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }

        public bool UpdateIncomeAcceptType(long waybillNo, string acceptType)
        {
            string sql =
                @"Update FMS_IncomeBaseInfo set AcceptType =:acceptType,UpdateTime = sysDate where WaybillNo =:WaybillNo ";
            OracleParameter[] parameters = {
                                              new OracleParameter(":acceptType", OracleDbType.Varchar2)
                                                  {Value = acceptType},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;

        }


        public bool UpdateCODBaseInfo(FMS_CODBaseInfo model)
        {
            string sql =
                @"Update FMS_CODBaseInfo set DeliverStationID =:DeliverStationID,TopCODCompanyID=:TopCODCompanyID,UpdateTime = sysDate 
                          where InfoID =:InfoID ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":DeliverStationID", OracleDbType.Int32)
                                                   {Value = model.DeliverStationID},
                                               new OracleParameter(":TopCODCompanyID", OracleDbType.Int32)
                                                   {Value = model.TopCODCompanyID},
                                               new OracleParameter(":InfoID", OracleDbType.Int64) {Value = model.ID}
                                           };
          return  OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;

        }


        public bool UpdateExpressAcceptType(long waybillNo, string acceptType)
        {
            string sql =
           @"Update FMS_ExpressReceiveFeeInfo set AcceptType=:acceptType,UpdateTime = sysDate where WaybillNo = :WaybillNo ";

            OracleParameter[] parameters = {
                                              new OracleParameter(":acceptType", OracleDbType.Varchar2)
                                                  {Value = acceptType},
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                          };
           return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters)>0;
        }
    }
}
