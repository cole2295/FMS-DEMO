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

namespace RFD.FMS.ServiceImpl.SortingManage
{
    public class SortingFeeService : ISortingFeeService
    {
        private ISortingFeeDao _feeDao;
        private IAccountOperatorLogDao _accountOperatorLogDao;
        public int AddSortingFee(FMS_SortingFeeModel model)
        {
           
             using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
             {
                 int id = _feeDao.AddSortingFee(model);
                 if (id == 0)  return 0;
                 if (id == -1) return -1;
                 string logText="增加拣运商信息，未审核（添加人:"+model.CreateBy+"拣运费:"+model.AccountFare+"," +
                                "商家编号:"+model.MerchantID+",分拣中心编号:"+model.SortingCenterID+
                                 "城市编号:"+model.CityID+",费用类型:"+model.FareType+",生效时间:"+model.EffectDate+"）添加时间:"+
                                 DateTime.Now;
                 if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID,
                     model.CreateBy, logText, 9))
                     return -1;
                 scope.Complete();
             }
            return 1;
        }


        public DataTable GetSortingFee(FMS_SortingFeeModel model)
        {
            return _feeDao.GetSoringFee(model);
        }


        public DataTable GetSortingFeeModel(string SortingFeeID)
        {
            return _feeDao.GetSortingFeeModel(SortingFeeID);
          
        }

        public DataTable GetSortingFeeWaitModel(string SortingFeeWaitID)
        {
            return _feeDao.GetSortingFeeWaitModel(SortingFeeWaitID);
        }

        public int UpdateSortingFee(FMS_SortingFeeModel model)
        {
            using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {
                int id = _feeDao.UpdateSortingFee(model);
                if (id == 0) return 0;
                if (id == -1) return -1;
                string logText = "修改拣运商信息，未审核（修改人:"+model.UpdateBy+"拣运费:" + model.AccountFare + "," +
                                "商家编号:" + model.MerchantID + ",分拣中心编号:" + model.SortingCenterID +
                                 "城市编号:" + model.CityID + ",费用类型:" + model.FareType + ",生效时间:" + model.EffectDate + "）添加时间"+DateTime.Now;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID,
                    model.UpdateBy, logText, 9))
                    return -1;
                scope.Complete();
            }
            return 1;
        }


        public int Delete(FMS_SortingFeeModel model)
        {
            using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {

                int id = _feeDao.Delete(model);
                if (id == 0) return 0;
                if (id == -1) return -1;
                string logText = "拣运商信息置无效，操作人"+model.UpdateBy;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID,
                    model.UpdateBy, logText, 9))
                    return -1;
                scope.Complete();
            }
            return 1;
        }


        public DataTable GetSortingFeeWait(FMS_SortingFeeModel model)
        {
            return _feeDao.GetSoringFeeWait(model);
        }


        public int AddSortingFeeWait(FMS_SortingFeeModel model)
        {
            using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {
                int id = _feeDao.AddSortingFeeWait(model);
                if (id == 0) return 0;
                if (id == -1) return -1;
                string logText = "增加拣运商待生效信息，未审核（添加人:" + model.CreateBy +",生效时间:" + model.EffectDate+ ",）添加时间:" +
                                DateTime.Now;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID,
                    model.CreateBy, logText, 9))
                    return -1;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeWaitID, model.CreateBy, logText+",对应生效表ID:"+model.SortingFeeID, 10))
                    return -1;
                scope.Complete();
            }
            return 1;
        }


        public int UpdateSortingFeeWait(FMS_SortingFeeModel model)
        {
            using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {
                int id = _feeDao.UpdateSortingFeeWait(model);
                if (id == 0) return 0;
                if (id == -1) return -1;
                string logText = "更新拣运商待生效信息，未审核（添加人:" + model.CreateBy + ",生效时间:" + model.EffectDate + ",）添加时间:" +
                                DateTime.Now;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeWaitID, model.UpdateBy, logText + ",对应生效表ID:" + model.SortingFeeID, 10))
                    return -1;
                scope.Complete();
            }
            return 1;
        }


        public FMS_SortingFeeModel GetSmallSortingFeeModel(string SortingFeeID)
        {
            var dt = _feeDao.GetSmallSortingFeeModel(SortingFeeID);
            FMS_SortingFeeModel model = new FMS_SortingFeeModel()
                                            {
                                                SortingCenterID = DataConvert.ToInt(dt.Rows[0]["SortingCenterID"]),
                                                SortingFeeID = dt.Rows[0]["SortingFeeID"].ToString(),
                                                SortingMerchantID = DataConvert.ToInt(dt.Rows[0]["SortingMerchantID"]),
                                                MerchantID = DataConvert.ToInt(dt.Rows[0]["MerchantID"]),
                                                CityID =  dt.Rows[0]["CityID"].ToString(),
                                                FareType = DataConvert.ToInt(dt.Rows[0]["FareType"]),
                                                Status = DataConvert.ToInt(dt.Rows[0]["Status"]),
                                                IsAccountBill = DataConvert.ToInt(dt.Rows[0]["IsAccountBill"]),
                                                WaybillCount = DataConvert.ToInt(dt.Rows[0]["WaybillCount"]),
                                                AccountFare = dt.Rows[0]["AccountFare"].ToString(),
                                                EffectDate = Convert.ToDateTime(dt.Rows[0]["EffectDate"].ToString())
                                                
                                            };
            return model;
        }


        public int AuditSortingFee(FMS_SortingFeeModel model)
        {
             using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
            {
                int id = _feeDao.AuditSortingFee(model);
                if (id == 0) return 0;
                if (id == -1) return -1;
                string logText = "更新拣运商信息，已审核（审核人:" + model.AuditBy + "),添加时间:" +
                                DateTime.Now;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID,
                    model.AuditBy, logText, 9))
                    return -1;
                model.Status = 1;
                int wid = _feeDao.AddSortingFeeWait(model);
                if(wid ==0) return 0;
                if (wid == -1) return -1;
                 logText = "添加拣运商待生效信息，已审核（审核人:" + model.AuditBy + "),添加时间:" +
                                DateTime.Now;
                if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeWaitID, model.AuditBy, logText + ",对应生效表ID:" + model.SortingFeeID, 10))
                    return -1;
                scope.Complete();
            }
            return 1;
        }
        

       public int  AuditSortingFeeWait(FMS_SortingFeeModel model)
       {
           using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
           {
               int id = _feeDao.AuditSortingFeeWait(model);
               if (id == 0) return 0;
               if (id == -1) return -1;
               string logText = "更新拣运商待生效信息，已审核（审核人:" + model.AuditBy + ")";
               if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeWaitID, model.AuditBy, logText + ",对应生效表ID:" + model.SortingFeeID, 10))
                   return -1;
               scope.Complete();
           }
           return 1; 
       }



       public int BackSortingFee(FMS_SortingFeeModel model)
       {
           using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
           {
               int id = _feeDao.BackSortingFee(model);
               int wid = _feeDao.DeleteWait(model);
               if (id == 0) return 0;
               if (id == -1) return -1;
               if (wid < 0) return -1;
               string logText = "置回拣运商信息，已置回（审核人:" + model.AuditBy + ")";

               if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID, model.UpdateBy, logText, 9))
                   return -1;
               scope.Complete();
           }
           return 1; 
       }


       public int BackSortingFeeWait(FMS_SortingFeeModel model)
       {
           using (IUnitOfWork scope = TranScopeFactory.CreateOracleUnit())
           {
               int id = _feeDao.BackSortingFeeWait(model);
               if (id == 0) return 0;
               if (id == -1) return -1;
               string logText = "置回拣运商待生效信息，已置回（审核人:" + model.AuditBy + ")";

               if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeWaitID, model.UpdateBy, logText , 10))
                   return -1;
               scope.Complete();
           }
           return 1;
       }





       public FMS_SortingFeeModel GetSmallSortingFeeWaitModel(string SortingFeeWaitID)
       {
           var dt = _feeDao.GetSmallSortingFeeWaitModel(SortingFeeWaitID);
           FMS_SortingFeeModel model = new FMS_SortingFeeModel()
           {
               SortingCenterID = DataConvert.ToInt(dt.Rows[0]["SortingCenterID"]),
               SortingFeeID = dt.Rows[0]["SortingFeeID"].ToString(),
               SortingMerchantID = DataConvert.ToInt(dt.Rows[0]["SortingMerchantID"]),
               MerchantID = DataConvert.ToInt(dt.Rows[0]["MerchantID"]),
               CityID = dt.Rows[0]["CityID"].ToString(),
               FareType = DataConvert.ToInt(dt.Rows[0]["FareType"]),
               Status = DataConvert.ToInt(dt.Rows[0]["Status"]),
               IsAccountBill = DataConvert.ToInt(dt.Rows[0]["IsAccountBill"]),
               WaybillCount = DataConvert.ToInt(dt.Rows[0]["WaybillCount"]),
               AccountFare = dt.Rows[0]["AccountFare"].ToString(),
               EffectDate = Convert.ToDateTime(dt.Rows[0]["EffectDate"].ToString()),
               SortingFeeWaitID = dt.Rows[0]["SortingFeeWaitID"].ToString(),
 

           };
           return model;
       }


       public string Dispposed(int rowNum)
       {
        
               DataTable dt = _feeDao.GetSoringFeeWait(rowNum);
               List<FMS_SortingFeeModel> listModel = GetSortingFeeModelList(dt);
               if(listModel.Count >0)
               {
                   UpdateSortingFeeWaitForEffect(listModel);
                   UpdateSortingFeeForEffect(listModel);
                   AddSortingFeeHis(listModel);
                   return "待生效更新" + dt.Rows.Count+"生效并添加历史表,执行时间:"+DateTime.Now.ToString();
               }
           return string.Empty;

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
                   if (id <= 0) return ;
                   string logText = "更新拣运商生效信息，已生效（审核人:" + model.AuditBy + ")";
                   if (!_accountOperatorLogDao.AddOperatorLogLog(model.SortingFeeID, 1, logText , 9))
                       return ;
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
           foreach (DataRow dr in dt.Rows  )
           {
               FMS_SortingFeeModel model= new FMS_SortingFeeModel()
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
           return _feeDao.ExsitSortingFeeWait(model);
       }


       public string GetAccountFareByMerchant(int SortingMerchantID, int SortingCenterID, string CityID, int MerchantID, DateTime SDate,int Type)
       {
           //string Price = "5";
           string Price= _feeDao.GetAccountFareByMerchant(SortingMerchantID, SortingCenterID, CityID, MerchantID, SDate,Type);
           return string.IsNullOrEmpty(Price) ? string.Empty : Price;
       }
    }

     
 }

