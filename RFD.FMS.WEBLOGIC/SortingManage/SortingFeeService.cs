using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Domain.COD;
using RFD.FMS.Domain.SortingManage;
using RFD.FMS.MODEL.FinancialManage;
using RFD.FMS.Service.SoringManage;
using RFD.FMS.Util;

namespace RFD.FMS.WEBLOGIC.SortingManage
{
    public class SortingFeeService : ISortingFeeService
    {
        private ISortingFeeDao _feeDao;
        private IAccountOperatorLogDao _accountOperatorLogDao;
        private ISortingFeeService OracleService;
        public int AddSortingFee(FMS_SortingFeeModel model)
        {
           
            if(OracleService !=null)
            {
                return OracleService.AddSortingFee(model);
            }
            else
            {
                throw new Exception("Sql为实现，请到demo下操作！");
            }
        }


        public DataTable GetSortingFee(FMS_SortingFeeModel model)
        {
           if(OracleService !=null)
           {
               return OracleService.GetSortingFee(model);
           }
           else
           {
               return _feeDao.GetSoringFee(model);
           }
        }


        public DataTable GetSortingFeeModel(string SortingFeeID)
        {
           if(OracleService !=null)
           {
               return OracleService.GetSortingFeeModel(SortingFeeID);
           }
           else
           {
               return _feeDao.GetSortingFeeModel(SortingFeeID);
           }
          
        }

        public DataTable GetSortingFeeWaitModel(string SortingFeeWaitID)
        {
           if(OracleService !=null)
           {
               return OracleService.GetSortingFeeWaitModel(SortingFeeWaitID);
           }
           else
           {
               return _feeDao.GetSortingFeeWaitModel(SortingFeeWaitID);
           }
        }

        public int UpdateSortingFee(FMS_SortingFeeModel model)
        {
           if(OracleService !=null)
           {
               return OracleService.UpdateSortingFee(model);
           }
           else
           {
               throw new Exception("Sql为实现，请到demo下操作！");
           }
        }


        public int Delete(FMS_SortingFeeModel model)
        {
            if(OracleService != null)
            {
                return OracleService.Delete(model);
            }
            else
            {
                throw new Exception("Sql为实现，请到demo下操作！");
            }
        }


        public DataTable GetSortingFeeWait(FMS_SortingFeeModel model)
        {
            if(OracleService !=null)
            {
                return OracleService.GetSortingFeeWait(model);
            }
            else
            {
                throw new Exception("Sql为实现，请到demo下操作！");
            }
        }


        public int AddSortingFeeWait(FMS_SortingFeeModel model)
        {
           if(OracleService !=null)
           {
               return OracleService.AddSortingFeeWait(model);
           }
           else
           {
               throw new Exception("Sql为实现，请到demo下操作！");
           }
        }


        public int UpdateSortingFeeWait(FMS_SortingFeeModel model)
        {
           if(OracleService !=null)
           {
               return OracleService.UpdateSortingFeeWait(model);
           }
           else
           {
               throw new Exception("Sql为实现，请到demo下操作！");
           }
        }


        public FMS_SortingFeeModel GetSmallSortingFeeModel(string SortingFeeID)
        {
           if(OracleService !=null)
           {
               return OracleService.GetSmallSortingFeeModel(SortingFeeID);
           }
           else
           {
               throw new Exception("Sql为实现，请到demo下操作！");
           }
        }


        public int AuditSortingFee(FMS_SortingFeeModel model)
        {
            if(OracleService!=null)
            {
                return OracleService.AuditSortingFee(model);
            }
            else
            {
                throw new Exception("Sql为实现，请到demo下操作！");
            }
        }
        

       public int  AuditSortingFeeWait(FMS_SortingFeeModel model)
       {
          if(OracleService !=null)
          {
             return  OracleService.AuditSortingFeeWait(model);
          }
          else
          {
              throw new Exception("Sql为实现，请到demo下操作！");
          }
       }



       public int BackSortingFee(FMS_SortingFeeModel model)
       {
          if(OracleService !=null)
          {
              return OracleService.BackSortingFee(model);
          }
          else
          {
              throw new Exception("Sql为实现，请到demo下操作！");
          }
       }


       public int BackSortingFeeWait(FMS_SortingFeeModel model)
       {
           if(OracleService !=null)
           {
               return OracleService.BackSortingFeeWait(model);
           }
           else
           {
               throw new Exception("Sql为实现，请到demo下操作！");
           }
       }





       public FMS_SortingFeeModel GetSmallSortingFeeWaitModel(string SortingFeeWaitID)
       {
          if(OracleService !=null)
          {
              return OracleService.GetSmallSortingFeeWaitModel(SortingFeeWaitID);
          }
          else
          {
              throw new Exception("Sql为实现，请到demo下操作！");
          }
       }


       public string Dispposed(int rowNum)
       {
           if(OracleService!=null)
           {
              return OracleService.Dispposed(rowNum);
           }
           else
           {
               throw new Exception("Sql为实现，请到demo下操作！");
           }
       }
       private void AddSortingFeeHis(List<FMS_SortingFeeModel> listModel)
       {
           foreach (var model in listModel)
           {
               _feeDao.AddSortingFeeHis(model);
           }
       }

       private void UpdateSortingFeeForEffect(List<FMS_SortingFeeModel> listModel)
       {
           foreach (var model in listModel)
           {

               using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
               {
                   int id = _feeDao.UpdateSortingFeeForEffect(model);
                   if (id <= 0) return;
                   string logText = "更新拣运商生效信息，已生效（审核人:" + model.AuditBy + ")";
                   if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID, 1, logText, 9))
                       return;
                   scope.Complete();
               }

           }
       }

       private void UpdateSortingFeeWaitForEffect(List<FMS_SortingFeeModel> listModel)
       {
           foreach (var model in listModel)
           {

               _feeDao.UpdateSortingFeeWaitForEffect(model);

           }
       }


       private List<FMS_SortingFeeModel> GetSortingFeeModelList(DataTable dt)
       {
           List<FMS_SortingFeeModel> models = new List<FMS_SortingFeeModel>();
           foreach (DataRow dr in dt.Rows)
           {
               FMS_SortingFeeModel model = new FMS_SortingFeeModel()
               {
                   SortingFeeWaitID = dr["SortingFeeWaitID"].ToString(),
                   SortingMerchantID = DataConvert.ToInt(dr["SortingMerchantID"]),
                   SortingCenterID = DataConvert.ToInt(dr["SortingCenterID"]),
                   CityID = dr["CityID"].ToString(),
                   MerchantID = DataConvert.ToInt(dr["MerchantID"]),
                   FareType = DataConvert.ToInt(dr["FareType"]),
                   AccountFare = dr["AccountFare"].ToString(),
                   IsAccountBill = DataConvert.ToInt(dr["IsAccountBill"]),
                   WaybillCount = DataConvert.ToInt(dr["WaybillCount"]),
                   AuditBy = DataConvert.ToInt(dr["AuditBy"]),
                   EffectDate = Convert.ToDateTime(dr["EffectDate"]),
                   SortingFeeID = dr["SortingFeeID"].ToString()
               };
               models.Add(model);
           }

           return models;

       }

       public bool ExsitSortingFeeWait(FMS_SortingFeeModel model)
       {
           if (OracleService != null)
           {
              return OracleService.ExsitSortingFeeWait(model);
           }
           else
           {
               throw new Exception("Sql为实现，请到demo下操作！");
           }
       }


       public string GetAccountFareByMerchant(int SortingMerchantID, int SortingCenterID, string CityID, int MerchantID,DateTime SDate,int Type)
       {
           if (OracleService != null)
           {
               return OracleService.GetAccountFareByMerchant(SortingMerchantID, SortingCenterID, CityID, MerchantID, SDate,Type);
           }
           else
           {
               throw  new Exception("Sql为实现，请到demo下操作！");
           }
       }
    }
}
