using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.COD;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.COD;
using System.Collections.Generic;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.Oracle.COD
{
    public class CODBaseInfoDao : OracleDao, ICODBaseInfoDao
	{
        public int Add(FMS_CODBaseInfo model)
        {
            //if (model.ID <= 0)
            //{
                model.ID = GetIdNew("SEQ_FMS_CODBASEINFO");
            //}

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into FMS_CODBaseInfo(");
            strSql.Append(" INFOID , ");
            strSql.Append(" MediumID , ");
            strSql.Append(" WaybillNO , ");
            strSql.Append(" MerchantID , ");
            strSql.Append(" WaybillType , ");
            strSql.Append(" Flag , ");
            strSql.Append(" DeliverStationID , ");
            strSql.Append(" TopCODCompanyID , ");
            strSql.Append(" WarehouseId , ");
            strSql.Append(" ExpressCompanyID , ");
            strSql.Append(" RfdAcceptTime , ");
            strSql.Append(" RfdAcceptDate , ");
            strSql.Append(" FinalExpressCompanyID , ");
            strSql.Append(" DeliverTime , ");
            strSql.Append(" DeliverDate , ");
            strSql.Append(" ReturnWareHouseID , ");
            strSql.Append(" ReturnExpressCompanyID , ");
            strSql.Append(" TotalAmount , ");
            strSql.Append(" PaidAmount , ");
            strSql.Append(" NeedPayAmount , ");
            strSql.Append(" BackAmount , ");
            strSql.Append(" NeedBackAmount , ");
            strSql.Append(" AccountWeight , ");
            strSql.Append(" AreaID , ");
            strSql.Append(" AreaType , ");
            strSql.Append(" BoxsNo , ");
            strSql.Append(" Address , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" IsDeleted , ");
            strSql.Append(" ReturnTime , ");
            strSql.Append(" ReturnDate , ");
            strSql.Append(" IsFare , ");
            strSql.Append(" Fare , ");
            strSql.Append(" FareFormula , ");
            strSql.Append(" OperateType , ");
            strSql.Append(" ProtectedPrice , ");
            strSql.Append(" DistributionCode , ");
            strSql.Append(" CurrentDistributionCode,  ");
            strSql.Append(" IsChange  ");
            strSql.Append(") values (");
            strSql.Append(" :INFOID , ");
            strSql.Append(" :MediumID , ");
            strSql.Append(" :WaybillNO , ");
            strSql.Append(" :MerchantID , ");
            strSql.Append(" :WaybillType , ");
            strSql.Append(" :Flag , ");
            strSql.Append(" :DeliverStationID , ");
            strSql.Append(" :TopCODCompanyID , ");
            strSql.Append(" :WarehouseId , ");
            strSql.Append(" :ExpressCompanyID , ");
            strSql.Append(" to_date(:RfdAcceptTime,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" to_date(:RfdAcceptDate,'yyyy-mm-dd') , ");
            strSql.Append(" :FinalExpressCompanyID , ");
            strSql.Append(" to_date(:DeliverTime,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" to_date(:DeliverDate,'yyyy-mm-dd') , ");
            strSql.Append(" :ReturnWareHouseID , ");
            strSql.Append(" :ReturnExpressCompanyID , ");
            strSql.Append(" :TotalAmount , ");
            strSql.Append(" :PaidAmount , ");
            strSql.Append(" :NeedPayAmount , ");
            strSql.Append(" :BackAmount , ");
            strSql.Append(" :NeedBackAmount , ");
            strSql.Append(" :AccountWeight , ");
            strSql.Append(" :AreaID , ");
            strSql.Append(" :AreaType , ");
            strSql.Append(" :BoxsNo , ");
            strSql.Append(" :Address , ");
            strSql.Append(" to_date(:CreateTime,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" to_date(:UpdateTime,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" :IsDeleted , ");
            strSql.Append(" to_date(:ReturnTime,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" to_date(:ReturnDate,'yyyy-mm-dd') , ");
            strSql.Append(" :IsFare , ");
            strSql.Append(" :Fare , ");
            strSql.Append(" :FareFormula , ");
            strSql.Append(" :OperateType , ");
            strSql.Append(" :ProtectedPrice , ");
            strSql.Append(" :DistributionCode , ");
            strSql.Append(" :CurrentDistributionCode,  ");
            strSql.Append(" :IsChange  ");
            strSql.Append(") ");
            OracleParameter[] parameters = {
                                            new OracleParameter(string.Format(":{0}","INFOID"), model.ID),
											new OracleParameter(string.Format(":{0}","MediumID"), model.MediumID),
											new OracleParameter(string.Format(":{0}","WaybillNO"), model.WaybillNO),
											new OracleParameter(string.Format(":{0}","MerchantID"), model.MerchantID),
											new OracleParameter(string.Format(":{0}","WaybillType"), model.WaybillType),
											new OracleParameter(string.Format(":{0}","Flag"), model.Flag),
											new OracleParameter(string.Format(":{0}","DeliverStationID"), model.DeliverStationID),
											new OracleParameter(string.Format(":{0}","TopCODCompanyID"), model.TopCODCompanyID),
											new OracleParameter(string.Format(":{0}","WarehouseId"), model.WarehouseId),
											new OracleParameter(string.Format(":{0}","ExpressCompanyID"), model.ExpressCompanyID),
											new OracleParameter(string.Format(":{0}","RfdAcceptTime"), Convert.ToDateTime(model.RfdAcceptTime).ToString("yyyy-MM-dd HH:mm:ss")),
											new OracleParameter(string.Format(":{0}","RfdAcceptDate"), Convert.ToDateTime(model.RfdAcceptDate).ToString("yyyy-MM-dd")),
											new OracleParameter(string.Format(":{0}","FinalExpressCompanyID"), model.FinalExpressCompanyID),
											new OracleParameter(string.Format(":{0}","DeliverTime"), Convert.ToDateTime(model.DeliverTime).ToString("yyyy-MM-dd HH:mm:ss")),
											new OracleParameter(string.Format(":{0}","DeliverDate"), Convert.ToDateTime(model.DeliverDate).ToString("yyyy-MM-dd")),
											new OracleParameter(string.Format(":{0}","ReturnWareHouseID"), model.ReturnWareHouseID),
											new OracleParameter(string.Format(":{0}","ReturnExpressCompanyID"), model.ReturnExpressCompanyID),
											new OracleParameter(string.Format(":{0}","TotalAmount"), model.TotalAmount),
											new OracleParameter(string.Format(":{0}","PaidAmount"), model.PaidAmount),
											new OracleParameter(string.Format(":{0}","NeedPayAmount"), model.NeedPayAmount),
											new OracleParameter(string.Format(":{0}","BackAmount"), model.BackAmount),
											new OracleParameter(string.Format(":{0}","NeedBackAmount"), model.NeedBackAmount),
											new OracleParameter(string.Format(":{0}","AccountWeight"), model.AccountWeight),
											new OracleParameter(string.Format(":{0}","AreaID"), model.AreaID),
											new OracleParameter(string.Format(":{0}","AreaType"), model.AreaType),
											new OracleParameter(string.Format(":{0}","BoxsNo"), model.BoxsNo),
											new OracleParameter(string.Format(":{0}","Address"), model.Address),
											new OracleParameter(string.Format(":{0}","CreateTime"), Convert.ToDateTime(model.CreateTime).ToString("yyyy-MM-dd HH:mm:ss")),
											new OracleParameter(string.Format(":{0}","UpdateTime"), Convert.ToDateTime(model.UpdateTime).ToString("yyyy-MM-dd HH:mm:ss")),
											new OracleParameter(string.Format(":{0}","IsDeleted"), model.IsDeleted?1:0),
											new OracleParameter(string.Format(":{0}","ReturnTime"), Convert.ToDateTime(model.ReturnTime).ToString("yyyy-MM-dd HH:mm:ss")),
											new OracleParameter(string.Format(":{0}","ReturnDate"), Convert.ToDateTime(model.ReturnDate).ToString("yyyy-MM-dd")),
											new OracleParameter(string.Format(":{0}","IsFare"), model.IsFare),
											new OracleParameter(string.Format(":{0}","Fare"), model.Fare),
											new OracleParameter(string.Format(":{0}","FareFormula"), model.FareFormula),
											new OracleParameter(string.Format(":{0}","OperateType"), model.OperateType),
											new OracleParameter(string.Format(":{0}","ProtectedPrice"), model.ProtectedPrice),
											new OracleParameter(string.Format(":{0}","DistributionCode"), model.DistributionCode),
											new OracleParameter(string.Format(":{0}","CurrentDistributionCode"), model.CurrentDistributionCode),
									        new OracleParameter(string.Format(":{0}","IsChange"), 1)
                                         };

            object obj = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (obj == null)
            {
                return 0;
            }

            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_CODBaseInfo GetModel(long id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select INFOID as ID, MediumID, WaybillNO, MerchantID, WaybillType, Flag, DeliverStationID, TopCODCompanyID, WarehouseId, ExpressCompanyID, RfdAcceptTime, RfdAcceptDate, FinalExpressCompanyID, DeliverTime, DeliverDate, ReturnWareHouseID, ReturnExpressCompanyID, TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, AreaType, BoxsNo, Address, CreateTime, UpdateTime, IsDeleted, ReturnTime, ReturnDate, IsFare, Fare, FareFormula, OperateType, ProtectedPrice, DistributionCode, CurrentDistributionCode  ");
            strSql.Append("  from FMS_CODBaseInfo ");
            strSql.Append(string.Format(" where {0} = :{0}", "INFOID"));

            var sqlParams = new List<OracleParameter>()
			{
				new OracleParameter(string.Format(":{0}","INFOID"),id)
			};

            var model = new FMS_CODBaseInfo();
            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                model = GetCODBaseInfoModel(ds.Tables[0].Rows[0]);
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_CODBaseInfo GetModelByWaybillNO(Int64 waybillNo)
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
                                ORDER BY fci.MediumID DESC");

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

        /// <summary>
        /// 根据条件得到一个对象实体集
        /// </summary>
        public List<FMS_CODBaseInfo> GetModelList(Dictionary<string, object> searchParams)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select INFOID as ID, MediumID, WaybillNO, MerchantID, WaybillType, Flag, DeliverStationID, TopCODCompanyID, WarehouseId, ExpressCompanyID, RfdAcceptTime, RfdAcceptDate, FinalExpressCompanyID, DeliverTime, DeliverDate, ReturnWareHouseID, ReturnExpressCompanyID, TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, AreaType, BoxsNo, Address, CreateTime, UpdateTime, IsDeleted, ReturnTime, ReturnDate, IsFare, Fare, FareFormula, OperateType, ProtectedPrice, DistributionCode, CurrentDistributionCode  ");
            strSql.Append(" from FMS_CODBaseInfo ");
            strSql.Append(" where 1 = 1 ");
            var sqlParams = new List<OracleParameter>();

            if (searchParams != null)
            {
                foreach (var item in searchParams)
                {
                    strSql.Append(string.Format(" and {0} = :{0}", item.Key));
                    sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value));
                }
            }

            var modelList = new List<FMS_CODBaseInfo>();
            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    modelList.Add(GetCODBaseInfoModel(row));
                }
            }
            return modelList;
        }

        /// <summary>
        /// 更改金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateAmountByID(FMS_CODBaseInfo info)
        {
            string sql = @"UPDATE FMS_CODBaseInfo SET NeedPayAmount = :NeedPayAmount,NeedBackAmount = :NeedBackAmount,IsChange=:IsChange WHERE INFOID=:ID";
            OracleParameter[] parameters ={
                                           new OracleParameter(":NeedPayAmount",OracleDbType.Decimal),
                                           new OracleParameter(":NeedBackAmount",OracleDbType.Decimal),
                                           new OracleParameter(":ID",OracleDbType.Decimal),
                                           new OracleParameter(":IsChange",OracleDbType.Decimal)
                                      };

            parameters[0].Value = info.NeedPayAmount;
            parameters[1].Value = info.NeedBackAmount;
            parameters[2].Value = info.ID;
            parameters[3].Value = 1;

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 将制定ID置为停用isdeleted=1
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateIsDeletedByID(FMS_CODBaseInfo info)
        {
            string sql = @"UPDATE FMS_CODBaseInfo SET IsDeleted=1,IsChange=1,UpdateTime=SysDate WHERE INFOID=:ID and IsDeleted=0";

            OracleParameter[] parameters =
            {
                new OracleParameter(":ID",OracleDbType.Decimal)
            };

            parameters[0].Value = info.ID;

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
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

        #region COD配送费计算
        /// <summary>
        /// 获取发货明细
        /// </summary>
        /// <param name="accountDays"></param>
        /// <param name="tops"></param>
        /// <param name="syncStartTime"></param>
        /// <returns></returns>
        public List<FMS_CODBaseInfo> GetDeliverDetails(int accountDays, int tops, string syncStartTime)
        {
            #region sql
            string strSql = @"
WITH    t AS ( SELECT 
						fcbi.infoID,
                        fcbi.WaybillNo ,--订单号
                        fcbi.DeliverTime ,--发货时间
                        fcbi.MerchantID ,--商家ID
                        fcbi.DeliverStationID ,--配送站ID    
                        fcbi.ExpressCompanyID ,
                        fcbi.WarehouseId ,
                        fcbi.WaybillType ,--发货类型
                        fcbi.FinalExpressCompanyID ,
                        fcbi.AccountWeight ,
                        fcbi.AreaID ,
                        fcbi.TopCODCompanyID,
                        fcbi.DistributionCode,
                        fcbi.NeedPayAmount,
                        fcbi.NeedBackAmount
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                      AND fcbi.Flag=1
                      AND fcbi.WaybillType IN ( '0', '1','3' )
                      AND fcbi.IsFare = 0
                      AND fcbi.DeliverTime > :CreatTimeStr
                      AND ROWNUM<=:Tops
             )
    SELECT  t.infoID,
      t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
                CASE WHEN t.MerchantID IN ( 8, 9 )
                THEN t.Warehouseid
                ELSE cast(t.ExpressCompanyID as varchar2(40)) END 
                          ELSE
                          cast(t.FinalExpressCompanyID as varchar2(40)) END Warehouseid,
            CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
                  CASE WHEN t.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                            END ELSE 2 END WareHouseType,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
            t.TopCODCompanyID,
            t.DistributionCode,
            t.NeedPayAmount,
            t.NeedBackAmount
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
";
            #endregion
            OracleParameter[] parameters ={
										   new OracleParameter(":Tops",OracleDbType.Decimal),
										   new OracleParameter(":CreatTimeStr",OracleDbType.Date),
									  };
            parameters[0].Value = string.IsNullOrEmpty(tops.ToString()) ? 100 : tops;
            parameters[1].Value = string.IsNullOrEmpty(syncStartTime) ?
                DateTime.Now.AddDays(-accountDays)
                : DateTime.Parse(syncStartTime);

            DataTable dt = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            return TransformToDetailModel(dt);
        }

        /// <summary>
        /// 获取拒收明细
        /// </summary>
        /// <param name="accountDays"></param>
        /// <param name="tops"></param>
        /// <param name="syncStartTime"></param>
        /// <returns></returns>
        public List<FMS_CODBaseInfo> GetReturnDetails(int accountDays, int tops, string syncStartTime)
        {
            string strSql1 = @"WITH    t AS ( SELECT 
                        fcbi.INFOID ,
                        fcbi.WaybillNo ,--订单号
                        fcbi.DeliverTime ,--发货时间
                        fcbi.MerchantID ,--商家ID
                        fcbi.DeliverStationID ,--配送站ID    
                        fcbi.ExpressCompanyID ,
                        fcbi.WarehouseId ,
                        fcbi.WaybillType ,--发货类型
                        fcbi.ReturnTime ,
                        fcbi.ReturnExpressCompanyId ,
                        fcbi.ReturnWareHouseID ,
                        fcbi.AreaID ,
                        fcbi.AccountWeight ,
                        fcbi.TopCODCompanyID,
                        fcbi.FinalExpressCompanyID,
                        fcbi.DistributionCode,
                        fcbi.NeedPayAmount,
                        fcbi.NeedBackAmount
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND WaybillType IN ( '0', '1','3')
                        AND fcbi.Flag = 0
                        AND fcbi.IsFare = 0
                        AND fcbi.ReturnTime > :ReturnTimeStr
                        AND ROWNUM<:Tops
             )
    SELECT  t.INFOID,
      t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            t.ReturnTime ,
            CASE WHEN t.MerchantID IN ( 8, 9 )
                                   THEN t.ReturnWareHouseID
                                   ELSE cast(t.ReturnExpressCompanyId as varchar2(40))
                              END ReturnWareHouse,
            CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
                CASE WHEN t.MerchantID IN ( 8, 9 )
                THEN t.Warehouseid
                ELSE cast(t.ExpressCompanyID as varchar2(40))END 
                          ELSE
                          cast(t.FinalExpressCompanyID as varchar2(40)) END Warehouseid,
             CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
                  CASE WHEN t.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                            END ELSE 2 END WareHouseType,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
      t.TopCODCompanyID,
            t.DistributionCode,
            t.NeedPayAmount,
            t.NeedBackAmount
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCODCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''";
            OracleParameter[] parameters1 ={
										   new OracleParameter(":Tops",OracleDbType.Decimal),
										   new OracleParameter(":ReturnTimeStr",OracleDbType.Date),
									  };
            parameters1[0].Value = string.IsNullOrEmpty(tops.ToString()) ? 100 : tops;
            parameters1[1].Value = string.IsNullOrEmpty(syncStartTime) ?
                DateTime.Now.AddDays(-accountDays)// DateTime.Parse("2012-01-31")
                : DateTime.Parse(syncStartTime);

            DataTable dt1 = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql1, parameters1).Tables[0];

            string strSql = @"
WITH    t AS ( SELECT
                        fcbi.INFOID ,
                        fcbi.WaybillNo ,--订单号
                        fcbi.CreateTime AS DeliverTime ,--发货时间
                        fcbi.MerchantID ,--商家ID
                        fcbi.DeliverStationID ,--配送站ID    
                        fcbi.ExpressCompanyID ,
                        fcbi.WarehouseId ,
                        fcbi.WaybillType ,--发货类型
                        fcbi.CreateTime AS ReturnTime ,
                        fcbi.FinalExpressCompanyID AS ReturnExpressCompanyId ,
                        fcbi.WarehouseId AS ReturnWareHouseID ,
                        fcbi.AreaID ,
                        fcbi.AccountWeight ,
                        fcbi.TopCODCompanyID,
                        fcbi.FinalExpressCompanyID,
                        fcbi.DistributionCode,
                        fcbi.NeedPayAmount,
                        fcbi.NeedBackAmount
         FROM     FMS_CODBaseInfo fcbi 
         WHERE    fcbi.IsDeleted = 0
                        AND WaybillType IN ( '0', '1','3')
                        AND fcbi.IsFare = 0
                        AND fcbi.OperateType in (2,5) 
                        AND fcbi.Flag=0
                        AND fcbi.CreateTime > :ReturnTimeStr
                        AND ROWNUM<:Tops
             )
    SELECT  t.INFOID,
      t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            t.ReturnTime ,
            CASE WHEN t.MerchantID IN ( 8, 9 )
                                   THEN t.ReturnWareHouseID
                                   ELSE cast(t.ReturnExpressCompanyId as varchar2(40))
                              END ReturnWareHouse,
            CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
                CASE WHEN t.MerchantID IN ( 8, 9 )
                THEN t.Warehouseid
                ELSE cast(t.ExpressCompanyID as varchar2(40))END 
                          ELSE
                          cast(t.FinalExpressCompanyID as varchar2(40)) END Warehouseid,
             CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
                  CASE WHEN t.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                            END ELSE 2 END WareHouseType,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
      t.TopCODCompanyID,
            t.DistributionCode,
            t.NeedPayAmount,
            t.NeedBackAmount
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCODCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''";
            OracleParameter[] parameters ={
										   new OracleParameter(":Tops",OracleDbType.Decimal),
										   new OracleParameter(":ReturnTimeStr",OracleDbType.Date),
									  };
            parameters[0].Value = string.IsNullOrEmpty(tops.ToString()) ? 100 : tops;
            parameters[1].Value = string.IsNullOrEmpty(syncStartTime) ?
                DateTime.Now.AddDays(-accountDays)// DateTime.Parse("2012-01-31")
                : DateTime.Parse(syncStartTime);

            DataTable dt2 = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];


            DataTable dt = CreateReturnDetailsDataTable();
            UniteDatatable(ref dt, dt1);
            UniteDatatable(ref dt, dt2);

            return TransformToDetailModel(dt);
        }

        private DataTable CreateReturnDetailsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("INFOID", typeof(Int64));
            dt.Columns.Add("WaybillNo", typeof(Int64));
            dt.Columns.Add("DeliverTime", typeof(DateTime));
            dt.Columns.Add("MerchantID", typeof(Int32));
            dt.Columns.Add("DeliverStationID", typeof(Int32));
            dt.Columns.Add("WaybillType", typeof(String));
            dt.Columns.Add("ReturnTime", typeof(DateTime));
            dt.Columns.Add("ReturnWareHouse", typeof(String));
            dt.Columns.Add("Warehouseid", typeof(String));
            dt.Columns.Add("WareHouseType", typeof(Int32));
            dt.Columns.Add("AccountWeight", typeof(Decimal));
            dt.Columns.Add("AreaID", typeof(String));
            dt.Columns.Add("AreaType", typeof(Int32));
            dt.Columns.Add("TopCODCompanyID", typeof(Int32));
            dt.Columns.Add("DistributionCode", typeof(String));
            dt.Columns.Add("NeedPayAmount", typeof(Decimal));
            dt.Columns.Add("NeedBackAmount", typeof(Decimal));
            return dt;
        }

        /// <summary>
        /// 获取上门退明细
        /// </summary>
        /// <param name="accountDays"></param>
        /// <param name="tops"></param>
        /// <param name="syncStartTime"></param>
        /// <returns></returns>
        public List<FMS_CODBaseInfo> GetVisitReturnDetails(int accountDays, int tops, string syncStartTime)
        {
            string strSql = @"
WITH    t AS ( SELECT
                        fcbi.InfoID ,
                        fcbi.WaybillNo ,--订单号
                        fcbi.DeliverTime ,--发货时间
                        fcbi.MerchantID ,--商家ID
                        fcbi.DeliverStationID ,--配送站ID    
                        fcbi.ExpressCompanyID ,
                        fcbi.WarehouseId ,
                        fcbi.WaybillType ,--发货类型
                        fcbi.ReturnTime ,
                        fcbi.ReturnExpressCompanyId ,
                        fcbi.ReturnWareHouseID ,
                        fcbi.AccountWeight ,
                        fcbi.AreaID ,
                        fcbi.TopCODCompanyID,
                        fcbi.FinalExpressCompanyID,
                        fcbi.DistributionCode,
                        fcbi.NeedPayAmount,
                        fcbi.NeedBackAmount
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND WaybillType = '2'
                        AND fcbi.Flag = 1
                        AND fcbi.IsFare = 0
                        AND fcbi.ReturnTime > :ReturnTimeStr
                        AND ROWNUM<:Tops
             )
    SELECT  t.InfoID ,
            t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            t.ReturnTime ,
            CASE WHEN t.MerchantID IN ( 8, 9 )
                                   THEN t.ReturnWareHouseID
                                   ELSE cast(t.ReturnExpressCompanyId as varchar2(40))
                              END ReturnWareHouse,
            CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
                CASE WHEN t.MerchantID IN ( 8, 9 )
                THEN t.Warehouseid
                ELSE cast(t.ExpressCompanyID as varchar2(40)) END 
                          ELSE
                          cast(t.FinalExpressCompanyID as varchar2(40)) END Warehouseid,
            CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null  THEN 
                  CASE WHEN t.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                            END ELSE 2 END WareHouseType,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
            t.TopCODCompanyID,
            t.DistributionCode,
            t.NeedPayAmount,
            t.NeedBackAmount
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCODCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''";
            OracleParameter[] parameters ={
										   new OracleParameter(":Tops",OracleDbType.Decimal),
										   new OracleParameter(":ReturnTimeStr",OracleDbType.Date),
									  };
            parameters[0].Value = string.IsNullOrEmpty(tops.ToString()) ? 100 : tops;
            parameters[1].Value = string.IsNullOrEmpty(syncStartTime) ?
                DateTime.Now.AddDays(-accountDays) // DateTime.Parse("2012-01-31")
                : DateTime.Parse(syncStartTime);

            DataTable dt = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            return TransformToDetailModel(dt);
        }

        private List<FMS_CODBaseInfo> TransformToDetailModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return null;

            List<FMS_CODBaseInfo> detailModels = new List<FMS_CODBaseInfo>();
            foreach (DataRow dr in dt.Rows)
            {
                FMS_CODBaseInfo detailModel = new FMS_CODBaseInfo();
                detailModel.ID = long.Parse(dr["INFOID"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "AreaType"))
                    detailModel.AreaType = string.IsNullOrEmpty(dr["AreaType"].ToString()) ? 0 : int.Parse(dr["AreaType"].ToString());
                if (dr["DeliverTime"] != DBNull.Value)
                    detailModel.DeliverTime = DateTime.Parse(dr["DeliverTime"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "MerchantId"))
                    detailModel.MerchantID = int.Parse(dr["MerchantId"].ToString());
                detailModel.WarehouseId = dr["WarehouseId"].ToString();
                detailModel.WareHouseType = int.Parse(dr["WareHouseType"].ToString());
                detailModel.AccountWeight = string.IsNullOrEmpty(dr["AccountWeight"].ToString()) ?
                                                        0 : decimal.Parse(dr["AccountWeight"].ToString());
                detailModel.WaybillNO = long.Parse(dr["WaybillNo"].ToString());
                detailModel.WaybillType = dr["WaybillType"].ToString();
                detailModel.DeliverStationID = int.Parse(dr["DeliverStationId"].ToString());
                detailModel.AreaID = dr["AreaID"].ToString();
                detailModel.TopCODCompanyID = string.IsNullOrEmpty(dr["TopCODCompanyID"].ToString()) ? 0 : int.Parse(dr["TopCODCompanyID"].ToString());
                detailModel.DistributionCode = dr["DistributionCode"].ToString();
                detailModel.NeedPayAmount = string.IsNullOrEmpty(dr["NeedPayAmount"].ToString()) ? 0 : decimal.Parse(dr["NeedPayAmount"].ToString());
                detailModel.NeedBackAmount = string.IsNullOrEmpty(dr["NeedBackAmount"].ToString()) ? 0 : decimal.Parse(dr["NeedBackAmount"].ToString());

                if (Common.JudgeColumnContains(dt, dr, "ReturnTime"))
                    detailModel.ReturnTime = DateTime.Parse(dr["ReturnTime"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "ReturnWareHouse"))
                    detailModel.ReturnWareHouseID = dr["ReturnWareHouse"].ToString();
                detailModels.Add(detailModel);
            }
            return detailModels;
        }

        /// <summary>
        /// 更新入库
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool UpdateCodFare(FMS_CODBaseInfo detail)
        {
            string sql = @"UPDATE FMS_CODBaseInfo
						SET    Fare = :Fare,
							   FareFormula = :FareFormula,IsCOD=:IsCOD,
							   IsFare=1,IsChange=1,AreaType=:AreaType,UpdateTime=sysdate
						WHERE  infoID = :infoID";
            OracleParameter[] parameters ={
										   new OracleParameter(":Fare",OracleDbType.Decimal),
										   new OracleParameter(":FareFormula",OracleDbType.Varchar2,300),
										   new OracleParameter(":infoID",OracleDbType.Decimal),
                                           new OracleParameter(":IsCOD",OracleDbType.Decimal),
                                           new OracleParameter(":AreaType",OracleDbType.Decimal),
									  };
            parameters[0].Value = detail.Fare;
            parameters[1].Value = detail.FareFormula;
            parameters[2].Value = detail.ID;
            parameters[3].Value = detail.IsCOD;
            parameters[4].Value = detail.AreaType;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
            //return true;
        }

        public bool UpdateBackError(FMS_CODBaseInfo detail)
        {
            string sql = @"UPDATE FMS_CODBaseInfo
						SET   IsFare=:IsFare,IsChange=1,AreaType=:AreaType,UpdateTime=sysdate
						WHERE  infoID = :infoID";
            OracleParameter[] parameters ={
										   new OracleParameter(":infoID",OracleDbType.Decimal),
										   new OracleParameter(":IsFare",OracleDbType.Decimal),
                                           new OracleParameter(":AreaType",OracleDbType.Decimal),
									  };
            parameters[0].Value = detail.ID;
            parameters[1].Value = detail.ErrorType;
            parameters[2].Value = string.IsNullOrEmpty(detail.AreaType.ToString()) || detail.AreaType == 0 ? -1 : detail.AreaType;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
            //return true;
        }
        #endregion

        #region COD日统计

        #region 发货统计
        /// <summary>
        /// 查询统计的仓库、配送商、总数
        /// </summary>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public List<CodStatsLogModel> GetDeliverToDayStatsInfo(DateTime accountDate)
        {
            string sql = @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        fcbi.MerchantID,
                        CASE WHEN fcbi.FinalExpressCompanyID = 0 or fcbi.FinalExpressCompanyID is null
                                           THEN CASE WHEN fcbi.MerchantID IN ( 8, 9 )
                                                     THEN fcbi.Warehouseid
                                                     ELSE cast(fcbi.ExpressCompanyID as varchar2(40))
                                                END
                                           ELSE cast(fcbi.FinalExpressCompanyID as varchar2(40))
                                      END Warehouseid,
                        CASE WHEN fcbi.FinalExpressCompanyID= 0 or fcbi.FinalExpressCompanyID is null
                                             THEN CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                                                  END ELSE 2
                                        END WareHouseType
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
            AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1','3' )
                        AND fcbi.DeliverTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fcbi.DeliverTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
             )
    SELECT  :CreatTimeStr AS StatisticsDate ,
            t.DeliverStationID AS ExpressCompanyID,
            t.Warehouseid ,
            COUNT(t.WaybillNO) AS FormCount ,
            1 AS StatisticsType ,
            t.WareHouseType,
            t.MerchantID
    FROM    t
    GROUP BY t.DeliverStationID ,
            t.Warehouseid ,
            t.WareHouseType,
            t.MerchantID
    ORDER BY t.DeliverStationID ,
            t.Warehouseid
";
            OracleParameter[] parameters = { 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
            return TransformToCodStatsLogModel(dt);
        }

        /// <summary>
        /// 根据配送商、仓库返回总数
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public int GetDeliverAllCountByExpressWareHose(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = GetDeliverHouseType1Sql(false);
            else
                sql = GetDeliverHouseType2Sql(false);
            OracleParameter[] parameters = { 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":Warehouseid",OracleDbType.Varchar2,40),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;

            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 根据配送商、仓库返回存在COD发货运费值的总数
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public int GetDeliverFareCountByExpressWareHouse(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = GetDeliverHouseType1Sql(true);
            else
                sql = GetDeliverHouseType2Sql(true);
            OracleParameter[] parameters = { 
											new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":Warehouseid",OracleDbType.Varchar2,40),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        private string GetDeliverHouseType1Sql(bool isFare)
        {
            string sql = @"
SELECT   COUNT(1)
FROM     FMS_CODBaseInfo fcbi 
WHERE    fcbi.IsDeleted = 0
		AND fcbi.Flag=1
        AND fcbi.WaybillType IN ( '0', '1','3' )
        AND fcbi.DeliverTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
        AND fcbi.DeliverTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
        AND fcbi.DeliverStationID = :DeliverStationID
        AND fcbi.Warehouseid = :Warehouseid
        AND fcbi.MerchantID = :MerchantID
        --AND ISNULL(fcbi.FinalExpressCompanyID, 0) = 0
		{0}
";
            if (isFare)
                sql = string.Format(sql, "	AND fcbi.IsFare = 1 ");
            else
                sql = string.Format(sql, "");
            return sql;
        }

        private string GetDeliverHouseType2Sql(bool isFare)
        {
            string sql = @"
WITH    t AS ( SELECT   fcbi.WaybillNO
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fcbi.DeliverTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.ExpressCompanyID = :Warehouseid
                        AND fcbi.MerchantID = :MerchantID
                        AND (fcbi.FinalExpressCompanyID= 0 or fcbi.FinalExpressCompanyID is null)
                        {0}
               UNION ALL
               SELECT   fcbi.WaybillNO
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fcbi.DeliverTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.FinalExpressCompanyID = :Warehouseid
                        AND fcbi.MerchantID = :MerchantID
                        AND fcbi.FinalExpressCompanyID > 0
                        {0}               
             )
    SELECT  COUNT(1)
    FROM    t
";
            if (isFare)
                sql = string.Format(sql, "	AND fcbi.IsFare = 1 ");
            else
                sql = string.Format(sql, "");
            return sql;
        }

        /// <summary>
        /// 按天结算统计
        /// </summary>
        /// <returns></returns>
        public List<CodStatsModel> GetDeliverAccountByDay(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = DeliverDayStatByHouseType1();
            else
                sql = DeliverDayStatByHouseType2();
            OracleParameter[] parameters ={
										new OracleParameter(":CreatTimeStr",OracleDbType.Varchar2),
										new OracleParameter(":CreatTimeEnd",OracleDbType.Varchar2),
										new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
										new OracleParameter(":WareHouseID",OracleDbType.Varchar2,40),
                                        new OracleParameter(":MerchantID",OracleDbType.Decimal)
									 };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
            return TransformToCodStatsModel(dt);
        }

        private string DeliverDayStatByHouseType1()
        {
            return @"
WITH    t AS ( SELECT   WaybillNO ,
                        DeliverStationID ,
                        fcbi.Warehouseid as Warehouseid ,
                        fcbi.WaybillType,
                        fcbi.AccountWeight,
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.ExpressCompanyID,
                        fcbi.FareFormula,
                        fcbi.Fare
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1','3')
                        AND fcbi.DeliverTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fcbi.DeliverTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
						AND fcbi.WareHouseID = :WareHouseID
                        AND fcbi.MerchantID = :MerchantID
                        --AND ISNULL(fcbi.FinalExpressCompanyID,0) = 0
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
			t.WareHouseID,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(case when t.AccountWeight is null then 0 else t.AccountWeight end) AS WEIGHT ,
            t.WaybillType AS DeliveryType,
            1 AS WareHouseType,
			t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.FareFormula,
			t.MerchantID
";
        }

        private string DeliverDayStatByHouseType2()
        {
            return @"
WITH    t AS ( SELECT   WaybillNO ,
                        DeliverStationID ,
                        CASE WHEN fcbi.FinalExpressCompanyID=0 or fcbi.FinalExpressCompanyID is null THEN 
								             CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								                  THEN fcbi.Warehouseid
								                  ELSE cast(fcbi.ExpressCompanyID as varchar2(40)) END 
                          ELSE
                          cast(fcbi.FinalExpressCompanyID as varchar2(40)) END Warehouseid,
                        fcbi.WaybillType,
                        fcbi.AccountWeight,
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.ExpressCompanyID,
                        fcbi.FareFormula,
                        fcbi.Fare
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
						            AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fcbi.DeliverTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.FinalExpressCompanyID = :WareHouseID
                        AND fcbi.MerchantID = :MerchantID
                        AND (fcbi.FinalExpressCompanyID=0 or fcbi.FinalExpressCompanyID is null)
                UNION ALL
                SELECT   WaybillNO ,
                        DeliverStationID ,
                        CASE WHEN fcbi.FinalExpressCompanyID=0 or fcbi.FinalExpressCompanyID is null THEN 
                            CASE WHEN fcbi.MerchantID IN ( 8, 9 )
                            THEN fcbi.Warehouseid
                            ELSE cast(fcbi.ExpressCompanyID as varchar2(40)) END 
                          ELSE
                          cast(fcbi.FinalExpressCompanyID as varchar2(40)) END Warehouseid,
                        fcbi.WaybillType,
                        fcbi.AccountWeight,
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.ExpressCompanyID,
                        fcbi.FareFormula,
                        fcbi.Fare
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= to_date(:CreatTimeStr,'yyyy-mm-dd')
                        AND fcbi.DeliverTime < to_date(:CreatTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.FinalExpressCompanyID = :WareHouseID
                        AND fcbi.MerchantID = :MerchantID
                        AND fcbi.FinalExpressCompanyID> 0
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(case when t.AccountWeight is null then 0 else t.AccountWeight end) AS WEIGHT ,
            t.WaybillType AS DeliveryType,
            2 AS WareHouseType,
            t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.FareFormula,
            t.MerchantID
";
        }

        /// <summary>
        /// 插入结算统计表
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        public bool InsertDeliverAccount(List<CodStatsModel> codStatsList, string date)
        {
            StringBuilder sbSqlList = new StringBuilder();

            string existsSql = @"
  SELECT count(*) into {0} FROM FMS_CODDeliveryCount WHERE WareHouseID = '{1}'
									AND ExpressCompanyID ={2} AND AreaType ={3} AND AccountDate=to_date('{4}','yyyy-mm-dd') 
									AND DeliveryType='{5}' AND WareHouseType={6} AND Formula='{7}' AND MerchantID={8};
  if({0}<=0) then
  begin
    {9}
    :all_Count := :all_Count + sql%rowcount;
  end;
  end if;
";

            string insertSql = @" INSERT INTO FMS_CODDeliveryCount ( ACCOUNTID,AccountNO,WareHouseID,ExpressCompanyID,
									AreaType,Weight,AccountDate,FormCount,Fare,Formula,CreateBy,CreateTime,
									UpdateBy,UpdateTime,DeliveryType,WareHouseType,MerchantID,IsChange,DELETEFLAG)  VALUES ";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            int i = 0;
            parameterList.Add(new OracleParameter(":all_Count", OracleDbType.Decimal) { Direction = ParameterDirection.Output });
            string formart = ":{0}{1}{2}";
            string sql = string.Empty;
            foreach (CodStatsModel codStats in codStatsList)
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append(string.Format(formart, "ACCOUNTID", i, ","));
                sbSql.Append("'',");
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "Weight", i, ","));
                sbSql.Append("to_date('" + date + "','yyyy-mm-dd'),");
                sbSql.Append(string.Format(formart, "FormCount", i, ","));
                sbSql.Append(string.Format(formart, "Fare", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append("0,");
                sbSql.Append("sysdate,");
                sbSql.Append("0,");
                sbSql.Append("sysdate,");
                sbSql.Append(string.Format(formart, "DeliveryType", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "IsChange", i, ","));
                sbSql.Append(string.Format(formart, "DELETEFLAG", i, ""));
                sbSql.Append(");");

                parameterList.Add(new OracleParameter(string.Format(formart, "ACCOUNTID", i, ""), GetIdNew("seq_fms_coddeliverycount")));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseID", i, ""), codStats.WareHouseID));
                parameterList.Add(new OracleParameter(string.Format(formart, "ExpressCompanyID", i, ""), codStats.ExpressCompanyID));
                parameterList.Add(new OracleParameter(string.Format(formart, "AreaType", i, ""), codStats.AreaType));
                parameterList.Add(new OracleParameter(string.Format(formart, "Weight", i, ""), codStats.Weight));
                parameterList.Add(new OracleParameter(string.Format(formart, "FormCount", i, ""), codStats.FormCount));
                parameterList.Add(new OracleParameter(string.Format(formart, "Fare", i, ""), codStats.Fare));
                parameterList.Add(new OracleParameter(string.Format(formart, "Formula", i, ""), string.IsNullOrEmpty(codStats.Formula) ? "" : codStats.Formula));
                parameterList.Add(new OracleParameter(string.Format(formart, "DeliveryType", i, ""), codStats.DeliveryType));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseType", i, ""), codStats.WareHouseType));
                parameterList.Add(new OracleParameter(string.Format(formart, "MerchantID", i, ""), codStats.MerchantID));
                parameterList.Add(new OracleParameter(string.Format(formart, "IsChange", i, ""), 1));
                parameterList.Add(new OracleParameter(string.Format(formart, "DELETEFLAG", i, ""), OracleDbType.Decimal) { Value = 0 });

                parameterList.Add(new OracleParameter(string.Format(formart, "v_count", i, ""), OracleDbType.Decimal) { Value=0});

                sql = string.Format(existsSql, string.Format(formart, "v_Count", i, ""), codStats.WareHouseID, codStats.ExpressCompanyID,
                                codStats.AreaType, date, codStats.DeliveryType, codStats.WareHouseType, codStats.Formula, codStats.MerchantID, sbSql.ToString());

                sbSqlList.Append(sql);

                i++;
            }
            string sqlStr = string.Format(@"
                                            begin 
                                                :all_Count := 0;
                                                {0}
                                            end;", sbSqlList.ToString());
            OracleParameter[] parameters = parameterList.ToArray();
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            bool flag = DataConvert.ToInt(parameters[0].Value, 0) > 0;
            return flag;
        }
        #endregion

        #region 拒收统计
        /// <summary>
        /// 查询统计的仓库、配送商、总数
        /// </summary>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public List<CodStatsLogModel> GetReturnToDayStatsInfo(DateTime accountDate)
        {
            string sql = @"WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        fcbi.MerchantID,
                        CASE WHEN fcbi.ReturnWareHouseID is null or fcbi.ReturnWareHouseID = ''
                                           THEN cast(fcbi.ReturnExpressCompanyID as varchar2(40))
                                           ELSE fcbi.ReturnWareHouseID
                                      END WareHouseID,
                        CASE WHEN fcbi.ReturnWareHouseID is null or fcbi.ReturnWareHouseID= '' THEN 2 ELSE 1
                                        END WareHouseType
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.Flag = 0
                        AND fcbi.WaybillType IN ( '0', '1','3')
                        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
             )
    SELECT  :ReturnTimeStr AS StatisticsDate ,
            t.DeliverStationID  AS ExpressCompanyID,
            t.Warehouseid ,
            COUNT(t.WaybillNO) AS FormCount ,
            2 AS StatisticsType ,
            t.WareHouseType,
            t.MerchantID
    FROM    t
    GROUP BY t.DeliverStationID ,
            t.Warehouseid ,
            t.WareHouseType,
            t.MerchantID
    ORDER BY t.DeliverStationID ,
            t.Warehouseid";
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt1 = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];


            sql = @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        fcbi.MerchantID,
                        CASE WHEN fcbi.WarehouseId is null or fcbi.WarehouseId = '' 
                                           THEN cast(fcbi.FinalExpressCompanyID as varchar2(40))
                                           ELSE fcbi.WarehouseId
                                      END WareHouseID,
                        CASE WHEN fcbi.WarehouseId is null or fcbi.WarehouseId = '' THEN 2 ELSE 1
                                        END WareHouseType
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.Flag = 0
                        AND fcbi.WaybillType IN ( '0', '1','3')
                        AND fcbi.CreateTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.CreateTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        AND fcbi.OperateType in (2,5)
             )
    SELECT  :ReturnTimeStr AS StatisticsDate ,
            t.DeliverStationID  AS ExpressCompanyID,
            t.Warehouseid ,
            COUNT(t.WaybillNO) AS FormCount ,
            2 AS StatisticsType ,
            t.WareHouseType,
            t.MerchantID
    FROM    t
    GROUP BY t.DeliverStationID ,
            t.Warehouseid ,
            t.WareHouseType,
            t.MerchantID
    ORDER BY t.DeliverStationID ,
            t.Warehouseid";
            OracleParameter[] parameters1 = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2)
										};
            parameters1[0].Value = accountDate.ToShortDateString();
            parameters1[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt2 = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters1).Tables[0];

            DataTable dt = CreateDataTable();
            UniteDatatable(ref dt, dt1);
            UniteDatatable(ref dt, dt2);

            return TransformToCodStatsLogModel(dt);
        }

        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("StatisticsDate", typeof(DateTime));
            dt.Columns.Add("ExpressCompanyID", typeof(Int32));
            dt.Columns.Add("Warehouseid", typeof(String));
            dt.Columns.Add("FormCount", typeof(Int32));
            dt.Columns.Add("StatisticsType", typeof(Int32));
            dt.Columns.Add("WareHouseType", typeof(Int32));
            dt.Columns.Add("MerchantID", typeof(Int32));
            return dt;
        }

        private void UniteDatatable(ref DataTable dtAll, DataTable data)
        {
            if (data == null || data.Rows.Count <= 0)
                return;
            foreach (DataRow dr in data.Rows)
            {
                //DataRow drNew = dtAll.NewRow();
                //drNew["StatisticsDate"] = dr["StatisticsDate"];
                dtAll.ImportRow(dr);
            }
        }

        /// <summary>
        /// 根据配送商、仓库返回总数
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public int GetReturnAllCountByExpressWareHose(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = GetReturnHouseType1Sql(false);
            else
                sql = GetReturnHouseType2Sql(false);
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnWareHouse",OracleDbType.Varchar2,40),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 根据配送商、仓库返回存在COD发货运费值的总数
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public int GetReturnFareCountByExpressWareHouse(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = GetReturnHouseType1Sql(true);
            else
                sql = GetReturnHouseType2Sql(true);
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnWareHouse",OracleDbType.Varchar2,40),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        private string GetReturnHouseType1Sql(bool isFare)
        {
            string sql = @"

with t as(
SELECT  fcbi.WaybillNO
FROM    FMS_CODBaseInfo fcbi 
WHERE   fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1','3')
        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fcbi.DeliverStationID = :DeliverStationID
        AND fcbi.ReturnWareHouseID = :ReturnWareHouse
        AND fcbi.MerchantID = :MerchantID
        {0}
UNION ALL
SELECT  fcbi.WaybillNO
FROM    FMS_CODBaseInfo fcbi 
WHERE   fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1','3')
        AND fcbi.OperateType in (2,5) 
        AND fcbi.CreateTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fcbi.CreateTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fcbi.DeliverStationID = :DeliverStationID
        AND fcbi.WarehouseId = :ReturnWareHouse
        AND fcbi.MerchantID = :MerchantID
        {0}
)
SELECT COUNT(1) FROM t
";
            if (isFare)
                sql = string.Format(sql, " AND fcbi.IsFare = 1 ");
            else
                sql = string.Format(sql, "");
            return sql;
        }

        private string GetReturnHouseType2Sql(bool isFare)
        {
            string sql = @"
with t as(
SELECT fcbi.WaybillNO
FROM     FMS_CODBaseInfo fcbi 
WHERE    fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1' ,'3')
        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fcbi.DeliverStationID = :DeliverStationID
        AND fcbi.ReturnExpressCompanyID = :ReturnWareHouse
        AND fcbi.MerchantID = :MerchantID
        {0}
UNION ALL
SELECT fcbi.WaybillNO
FROM     FMS_CODBaseInfo fcbi 
WHERE    fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1','3' )
        AND fcbi.OperateType in (2,5) 
        AND fcbi.CreateTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fcbi.CreateTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fcbi.DeliverStationID = :DeliverStationID
        AND fcbi.FinalExpressCompanyID = :ReturnWareHouse
        AND fcbi.MerchantID = :MerchantID
        {0}
)
SELECT COUNT(1) FROM t
";
            if (isFare)
                sql = string.Format(sql, " AND fcbi.IsFare = 1 ");
            else
                sql = string.Format(sql, "");
            return sql;
        }

        /// <summary>
        /// 按天结算统计
        /// </summary>
        /// <returns></returns>
        public List<CodStatsModel> GetReturnAccountByDay(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = ReturnDayStatByHouseType1();
            else
                sql = ReturnDayStatByHouseType2();
            OracleParameter[] parameters ={
										new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
										new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
										new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
										new OracleParameter(":ReturnWareHouse",OracleDbType.Varchar2,40),
                                        new OracleParameter(":MerchantID",OracleDbType.Decimal),
									 };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
            return TransformToCodStatsModel(dt);
        }

        private string ReturnDayStatByHouseType1()
        {
            return @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null
                                           THEN cast(fcbi.ReturnExpressCompanyID as varchar(40))
                                           ELSE fcbi.ReturnWareHouseID
                                      END WareHouseID,
                        CASE WHEN fcbi.ReturnWareHouseID is null or fcbi.ReturnWareHouseID = '' THEN 2
                                             ELSE 1
                                        END WareHouseType,
						fcbi.Fare,
						fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1','3' )
                        AND fcbi.Flag = 0
                        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.ReturnWareHouseID = :ReturnWareHouse
                        AND fcbi.MerchantID = :MerchantID
			UNION ALL
			SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        CASE WHEN fcbi.WarehouseId = '' or fcbi.WarehouseId is null
                                           THEN cast(fcbi.FinalExpressCompanyID as varchar(40))
                                           ELSE fcbi.WarehouseId
                                      END WareHouseID,
                        CASE WHEN fcbi.WarehouseId= '' or fcbi.WarehouseId is null THEN 2
                                             ELSE 1
                                        END WareHouseType,
                        fcbi.Fare,
                        fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1','3' )
                        AND fcbi.Flag = 0
                        AND fcbi.CreateTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.CreateTime <to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        AND fcbi.OperateType in (2,5) 
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.WarehouseId = :ReturnWareHouse
                        AND fcbi.MerchantID = :MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(case when t.AccountWeight is null then 0 else t.AccountWeight end) AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            1 AS WareHouseType,
            t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (
                                                         1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.FareFormula,
            t.MerchantID
";
        }

        private string ReturnDayStatByHouseType2()
        {
            return @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null
                                           THEN cast(fcbi.ReturnExpressCompanyID as varchar2(40))
                                           ELSE fcbi.ReturnWareHouseID
                                      END WareHouseID,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null THEN 2
                                             ELSE 1
                                        END WareHouseType,
						            fcbi.Fare,
						            fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1' ,'3')
                        AND fcbi.Flag = 0
                        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.ReturnExpressCompanyID = :ReturnWareHouse
                        AND fcbi.MerchantID = :MerchantID
			UNION ALL
			SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        CASE WHEN fcbi.WarehouseId = '' or fcbi.WarehouseId is null
                                           THEN cast(fcbi.FinalExpressCompanyID as varchar2(40))
                                           ELSE fcbi.WarehouseId
                                      END WareHouseID,
                        CASE WHEN fcbi.WarehouseId is null or fcbi.WarehouseId= '' THEN 2
                                             ELSE 1
                                        END WareHouseType,
						            fcbi.Fare,
						            fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1','3' )
                        AND fcbi.Flag = 0
                        AND fcbi.CreateTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.CreateTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        AND fcbi.OperateType in (2,5) 
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.FinalExpressCompanyID = :ReturnWareHouse
                        AND fcbi.MerchantID = :MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(case when t.AccountWeight is null then 0 else t.AccountWeight end) AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            2 AS WareHouseType,
			      t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.FareFormula,
            t.MerchantID
";
        }

        /// <summary>
        /// 插入结算统计表
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        public bool InsertReturnsAccount(List<CodStatsModel> codStatsList, string date)
        {
            StringBuilder sbSqlList = new StringBuilder();
            string existsSql = @"
  SELECT count(1) into {0} FROM FMS_CODReturnsCount WHERE WareHouseID = '{1}' AND ExpressCompanyID ={2}
                                                        AND AreaType ={3} AND AccountDate=to_date('{4}','yyyy-mm-dd') AND ReturnsType='{5}'
                                                        AND WareHouseType={6} AND Formula='{7}' AND MerchantID={8};
  if({0}<=0) then
  begin
    {9}
    :all_Count := :all_Count + sql%rowcount;
  end;
  end if;
";
            string insertSql = @" INSERT INTO FMS_CODReturnsCount (ACCOUNTID, AccountNO,WareHouseID,ExpressCompanyID,
									AreaType,Weight,AccountDate,FormCount,Fare,Formula,CreateBy,CreateTime,
									UpdateBy,UpdateTime,ReturnsType,WareHouseType,MerchantID,IsChange,DELETEFLAG)  VALUES ";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            int i = 0;
            parameterList.Add(new OracleParameter(":all_Count", OracleDbType.Decimal) { Direction = ParameterDirection.Output });
            string formart = ":{0}{1}{2}";
            string sql = string.Empty;
            foreach (CodStatsModel codStats in codStatsList)
            {
                StringBuilder sbSql = new StringBuilder();
                
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append(string.Format(formart, "ACCOUNTID", i, ","));
                sbSql.Append("'',");
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "Weight", i, ","));
                sbSql.Append("to_date('" + date + "','yyyy-mm-dd'),");
                sbSql.Append(string.Format(formart, "FormCount", i, ","));
                sbSql.Append(string.Format(formart, "Fare", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append("0,");
                sbSql.Append("SYSDATE,");
                sbSql.Append("0,");
                sbSql.Append("SYSDATE,");
                sbSql.Append(string.Format(formart, "ReturnsType", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "IsChange", i, ","));
                sbSql.Append(string.Format(formart, "DELETEFLAG", i, ""));
                sbSql.Append(");");
                parameterList.Add(new OracleParameter(string.Format(formart, "ACCOUNTID", i, ""), GetIdNew("SEQ_FMS_CODRETURNSCOUNT")));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseID", i, ""), codStats.WareHouseID));
                parameterList.Add(new OracleParameter(string.Format(formart, "ExpressCompanyID", i, ""), codStats.ExpressCompanyID));
                parameterList.Add(new OracleParameter(string.Format(formart, "AreaType", i, ""), codStats.AreaType));
                parameterList.Add(new OracleParameter(string.Format(formart, "Weight", i, ""), codStats.Weight));
                parameterList.Add(new OracleParameter(string.Format(formart, "FormCount", i, ""), codStats.FormCount));
                parameterList.Add(new OracleParameter(string.Format(formart, "Fare", i, ""), codStats.Fare));
                parameterList.Add(new OracleParameter(string.Format(formart, "Formula", i, ""), codStats.Formula));
                parameterList.Add(new OracleParameter(string.Format(formart, "ReturnsType", i, ""), codStats.ReturnsType));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseType", i, ""), codStats.WareHouseType));
                parameterList.Add(new OracleParameter(string.Format(formart, "MerchantID", i, ""), codStats.MerchantID));
                parameterList.Add(new OracleParameter(string.Format(formart, "IsChange", i, ""), 1));
                parameterList.Add(new OracleParameter(string.Format(formart, "DELETEFLAG", i, ""), OracleDbType.Decimal) { Value = 0 });

                parameterList.Add(new OracleParameter(string.Format(formart, "v_count", i, ""), OracleDbType.Decimal) { Value = 0 });

                sql = string.Format(existsSql, string.Format(formart, "v_Count", i, ""), codStats.WareHouseID, codStats.ExpressCompanyID,
                                codStats.AreaType, date, codStats.ReturnsType, codStats.WareHouseType, codStats.Formula, codStats.MerchantID, sbSql.ToString());
                sbSqlList.Append(sql);
                i++;
            }
            string sqlStr = string.Format(@"
                                            begin 
                                                :all_Count := 0;
                                                {0}
                                            end;", sbSqlList.ToString());
            OracleParameter[] parameters = parameterList.ToArray();
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            bool flag = DataConvert.ToInt(parameters[0].Value, 0) > 0;
            return flag;
        }
        #endregion

        #region 上门退统计
        /// <summary>
        /// 查询统计的仓库、配送商、总数
        /// </summary>
        /// <param name="accountDate"></param>
        /// <returns></returns>
        public List<CodStatsLogModel> GetVisitToDayStatsInfo(DateTime accountDate)
        {
            string sql = @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        fcbi.MerchantID,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null
                                           THEN cast(fcbi.ReturnExpressCompanyID as varchar2(40))
                                           ELSE fcbi.ReturnWareHouseID
                                      END WareHouseID,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null THEN 2 ELSE 1
                                        END WareHouseType
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.Flag = 1
                        AND fcbi.WaybillType ='2'
                        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
             )
    SELECT  :ReturnTimeStr AS StatisticsDate ,
            t.DeliverStationID  AS ExpressCompanyID,
            t.Warehouseid ,
            COUNT(t.WaybillNO) AS FormCount ,
            3 AS StatisticsType ,
            t.WareHouseType,
            t.MerchantID
    FROM    t
    GROUP BY t.DeliverStationID ,
            t.Warehouseid ,
            t.WareHouseType,
            t.MerchantID
    ORDER BY t.DeliverStationID ,
            t.Warehouseid";
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
            return TransformToCodStatsLogModel(dt);
        }

        /// <summary>
        /// 根据配送商、仓库返回总数
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public int GetVisitAllCountByExpressWareHose(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = GetVisitHouseType1Sql(false);
            else
                sql = GetVisitHouseType2Sql(false);
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnWareHouse",OracleDbType.Varchar2,40),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return (int)OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 根据配送商、仓库返回存在COD发货运费值的总数
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public int GetVisitFareCountByExpressWareHouse(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = GetVisitHouseType1Sql(true);
            else
                sql = GetVisitHouseType2Sql(true);
            OracleParameter[] parameters = { 
											new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
											new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
											new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
											new OracleParameter(":ReturnWareHouse",OracleDbType.Varchar2,40),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters));
        }

        private string GetVisitHouseType1Sql(bool isFare)
        {
            string sql = @"
SELECT  COUNT(1)
FROM    FMS_CODBaseInfo fcbi 
WHERE   fcbi.IsDeleted = 0
        AND fcbi.Flag = 1
        AND fcbi.WaybillType ='2'
        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fcbi.DeliverStationID = :DeliverStationID
        AND fcbi.ReturnWareHouseID = :ReturnWareHouse
        AND fcbi.MerchantID = :MerchantID
		{0}
";
            if (isFare)
                sql = string.Format(sql, " AND fcbi.IsFare = 1 ");
            else
                sql = string.Format(sql, "");
            return sql;
        }

        private string GetVisitHouseType2Sql(bool isFare)
        {
            string sql = @"
SELECT   COUNT(1)
FROM     FMS_CODBaseInfo fcbi 
WHERE    fcbi.IsDeleted = 0
        AND fcbi.Flag = 1
        AND fcbi.WaybillType ='2'
        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
        AND fcbi.DeliverStationID = :DeliverStationID
        AND fcbi.ReturnExpressCompanyID = :ReturnWareHouse
        AND fcbi.MerchantID = :MerchantID
		{0}
";
            if (isFare)
                sql = string.Format(sql, " AND fcbi.IsFare = 1 ");
            else
                sql = string.Format(sql, "");
            return sql;
        }

        /// <summary>
        /// 按天结算统计
        /// </summary>
        /// <returns></returns>
        public List<CodStatsModel> GetVisitAccountByDay(CodStatsLogModel codStatsLog)
        {
            string sql = string.Empty;
            if (codStatsLog.WareHouseType == 1)
                sql = VisitDayStatByHouseType1();
            else
                sql = VisitDayStatByHouseType2();
            OracleParameter[] parameters ={
										new OracleParameter(":ReturnTimeStr",OracleDbType.Varchar2),
										new OracleParameter(":ReturnTimeEnd",OracleDbType.Varchar2),
										new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
										new OracleParameter(":ReturnWareHouse",OracleDbType.Varchar2,40),
                                        new OracleParameter(":MerchantID",OracleDbType.Decimal),
									 };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
            return TransformToCodStatsModel(dt);
        }

        private string VisitDayStatByHouseType1()
        {
            return @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null
                                           THEN cast(fcbi.ReturnExpressCompanyID as varchar2(40))
                                           ELSE fcbi.ReturnWareHouseID
                                      END WareHouseID,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null THEN 2
                                             ELSE 1
                                        END WareHouseType,
                        fcbi.Fare,
                        fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType ='2'
                        AND fcbi.Flag = 1
                        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.ReturnWareHouseID = :ReturnWareHouse
                        AND fcbi.MerchantID = :MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(case when t.AccountWeight is null then 0 else t.AccountWeight end) AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            1 AS WareHouseType,
            t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN (
                                                         1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID, '') = ''
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.FareFormula,
            t.MerchantID
";
        }

        private string VisitDayStatByHouseType2()
        {
            return @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null
                                           THEN cast(fcbi.ReturnExpressCompanyID as varchar(40))
                                           ELSE fcbi.ReturnWareHouseID
                                      END WareHouseID,
                        CASE WHEN fcbi.ReturnWareHouseID = '' or fcbi.ReturnWareHouseID is null THEN 2
                                             ELSE 1
                                        END WareHouseType,
                        fcbi.Fare,
                        fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     FMS_CODBaseInfo fcbi 
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType ='2'
                        AND fcbi.Flag = 1
                        AND fcbi.ReturnTime >= to_date(:ReturnTimeStr,'yyyy-mm-dd')
                        AND fcbi.ReturnTime < to_date(:ReturnTimeEnd,'yyyy-mm-dd')
                        AND fcbi.DeliverStationID = :DeliverStationID
                        AND fcbi.ReturnExpressCompanyID = :ReturnWareHouse
                        AND fcbi.MerchantID = :MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(case when t.AccountWeight is null then 0 else t.AccountWeight end)  AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            2 AS WareHouseType,
			      t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = t.AreaID
                                                         AND ael.IsEnable IN ( 1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
    GROUP BY t.DeliverStationID ,
            t.WareHouseID ,
            ael.AreaType ,
            t.WaybillType,
            t.FareFormula,
            t.MerchantID
";
        }

        /// <summary>
        /// 插入结算统计表
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        public bool InsertVisitReturnsAccount(List<CodStatsModel> codStatsList, string date)
        {
            StringBuilder sbSqlList = new StringBuilder();
            string existsSql = @"
  SELECT count(*) into {0} FROM FMS_CODVisitReturnsCount WHERE WareHouseID = '{1}' 
									AND ExpressCompanyID ={2} AND AreaType ={3} 
                                    AND AccountDate=to_date('{4}','yyyy-mm-dd')  AND WareHouseType={5} AND Formula='{6}' AND MerchantID={7};
  if({0}<=0) then
  begin
    {8}
    :all_Count := :all_Count + sql%rowcount;
  end;
  end if;
";
            string insertSql = @" INSERT INTO FMS_CODVisitReturnsCount (AccountID, AccountNO,WareHouseID,ExpressCompanyID,
									AreaType,Weight,AccountDate,FormCount,Fare,Formula,CreateBy,CreateTime,
                                    UpdateBy,UpdateTime,WareHouseType,MerchantID,IsChange,DELETEFLAG)  VALUES ";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            int i = 0;
            parameterList.Add(new OracleParameter(":all_Count", OracleDbType.Decimal) { Direction = ParameterDirection.Output });
            string formart = ":{0}{1}{2}";
            string sql = string.Empty;
            foreach (CodStatsModel codStats in codStatsList)
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append(string.Format(formart, "AccountID", i, ","));
                sbSql.Append("'',");
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "Weight", i, ","));
                sbSql.Append("to_date('" + date + "','yyyy-mm-dd'),");
                sbSql.Append(string.Format(formart, "FormCount", i, ","));
                sbSql.Append(string.Format(formart, "Fare", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append("0,");
                sbSql.Append("SYSDATE,");
                sbSql.Append("0,");
                sbSql.Append("SYSDATE,");
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "IsChange", i, ","));
                sbSql.Append(string.Format(formart, "DELETEFLAG", i, ""));
                sbSql.Append(");");
                parameterList.Add(new OracleParameter(string.Format(formart, "AccountID", i, ""), GetIdNew("seq_fms_codvisitreturnscount")));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseID", i, ""), codStats.WareHouseID));
                parameterList.Add(new OracleParameter(string.Format(formart, "ExpressCompanyID", i, ""), codStats.ExpressCompanyID));
                parameterList.Add(new OracleParameter(string.Format(formart, "AreaType", i, ""), codStats.AreaType));
                parameterList.Add(new OracleParameter(string.Format(formart, "Weight", i, ""), codStats.Weight));
                parameterList.Add(new OracleParameter(string.Format(formart, "FormCount", i, ""), codStats.FormCount));
                parameterList.Add(new OracleParameter(string.Format(formart, "Fare", i, ""), codStats.Fare));
                parameterList.Add(new OracleParameter(string.Format(formart, "Formula", i, ""), codStats.Formula));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseType", i, ""), codStats.WareHouseType));
                parameterList.Add(new OracleParameter(string.Format(formart, "MerchantID", i, ""), codStats.MerchantID));
                parameterList.Add(new OracleParameter(string.Format(formart, "IsChange", i, ""), 1));
                parameterList.Add(new OracleParameter(string.Format(formart, "DELETEFLAG", i, ""), OracleDbType.Decimal) { Value = 0 });

                parameterList.Add(new OracleParameter(string.Format(formart, "v_count", i, ""), OracleDbType.Decimal) { Value = 0 });

                sql = string.Format(existsSql, string.Format(formart, "v_Count", i, ""), codStats.WareHouseID, codStats.ExpressCompanyID,
                                codStats.AreaType, date, codStats.WareHouseType, codStats.Formula, codStats.MerchantID, sbSql.ToString());
                sbSqlList.Append(sql);

                i++;
            }
            string sqlStr = string.Format(@"
                                            begin 
                                                :all_Count := 0;
                                                {0}
                                            end;", sbSqlList.ToString());
            OracleParameter[] parameters = parameterList.ToArray();
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
            bool flag = DataConvert.ToInt(parameters[0].Value, 0) > 0;
            return flag;
        }
        #endregion

        /// <summary>
        /// 转换对象LIST
        /// </summary>
        /// <returns></returns>
        public static List<CodStatsLogModel> TransformToCodStatsLogModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return null;

            List<CodStatsLogModel> codStatsLogList = new List<CodStatsLogModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CodStatsLogModel codStatsLog = new CodStatsLogModel();

                if (Common.JudgeColumnContains(dt, dr, "CodStatsID"))
                    codStatsLog.CodStatsID = long.Parse(dr["CodStatsID"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "CreateTime"))
                    codStatsLog.CreateTime = DateTime.Parse(dr["CreateTime"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "StatisticsType"))
                    codStatsLog.StatisticsType = int.Parse(dr["StatisticsType"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "ExpressCompanyID"))
                    codStatsLog.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "FormCount"))
                    codStatsLog.FormCount = int.Parse(dr["FormCount"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "Ip"))
                    codStatsLog.Ip = dr["Ip"].ToString();
                if (Common.JudgeColumnContains(dt, dr, "IsSuccess"))
                    codStatsLog.IsSuccess = int.Parse(dr["IsSuccess"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "Reasons"))
                    codStatsLog.Reasons = dr["Reasons"].ToString();
                if (Common.JudgeColumnContains(dt, dr, "StatisticsDate"))
                    codStatsLog.StatisticsDate = DateTime.Parse(dr["StatisticsDate"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "UpdateTime"))
                    codStatsLog.UpdateTime = DateTime.Parse(dr["UpdateTime"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "WareHouseID"))
                    codStatsLog.WareHouseID = dr["WareHouseID"].ToString();
                if (Common.JudgeColumnContains(dt, dr, "WareHouseType"))
                    codStatsLog.WareHouseType = int.Parse(dr["WareHouseType"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "MerchantID"))
                    codStatsLog.MerchantID = int.Parse(dr["MerchantID"].ToString());

                codStatsLogList.Add(codStatsLog);
            }

            return codStatsLogList;
        }

        /// <summary>
        /// 转换为日统计modelList
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<CodStatsModel> TransformToCodStatsModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return null;

            List<CodStatsModel> codStatsList = new List<CodStatsModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CodStatsModel codStats = new CodStatsModel();
                if (Common.JudgeColumnContains(dt, dr, "ExpressCompanyID"))
                    codStats.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "WareHouseID"))
                    codStats.WareHouseID = dr["WareHouseID"].ToString();
                if (Common.JudgeColumnContains(dt, dr, "Weight"))
                    codStats.Weight = decimal.Parse(dr["Weight"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "Formula"))
                    codStats.Formula = dr["Formula"].ToString();
                if (Common.JudgeColumnContains(dt, dr, "FormCount"))
                    codStats.FormCount = int.Parse(dr["FormCount"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "Fare"))
                    codStats.Fare = decimal.Parse(dr["Fare"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "AreaType"))
                    codStats.AreaType = int.Parse(dr["AreaType"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "DeliveryType"))
                    codStats.DeliveryType = int.Parse(dr["DeliveryType"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "ReturnsType"))
                    codStats.ReturnsType = int.Parse(dr["ReturnsType"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "WareHouseType"))
                    codStats.WareHouseType = int.Parse(dr["WareHouseType"].ToString());
                if (Common.JudgeColumnContains(dt, dr, "MerchantID"))
                    codStats.MerchantID = int.Parse(dr["MerchantID"].ToString());
                codStatsList.Add(codStats);
            }
            return codStatsList;
        }


        #endregion

        #region log
        /// <summary>
        /// 查询是否存在
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public bool JudgeLogExists(CodStatsLogModel codStatsLog)
        {
            string sql = @"SELECT COUNT(1) FROM FMS_CodStatsLog 
							WHERE StatisticsType=:StatisticsType 
									AND StatisticsDate=to_date(:StatisticsDate,'yyyy-mm-dd')
									AND WareHouseID=:WareHouseID 
									AND ExpressCompanyID=:ExpressCompanyID
									AND WareHouseType=:WareHouseType
                                    AND MerchantID=:MerchantID";
            OracleParameter[] parameters ={
										   new OracleParameter(":StatisticsDate",OracleDbType.Varchar2),
										   new OracleParameter(":StatisticsType",OracleDbType.Decimal),
										   new OracleParameter(":WareHouseID",OracleDbType.Varchar2,40),
										   new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
										   new OracleParameter(":WareHouseType",OracleDbType.Decimal),
                                           new OracleParameter(":MerchantID",OracleDbType.Decimal)
									  };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsType;
            parameters[2].Value = codStatsLog.WareHouseID;
            parameters[3].Value = codStatsLog.ExpressCompanyID;
            parameters[4].Value = codStatsLog.WareHouseType;
            parameters[5].Value = codStatsLog.MerchantID;
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 更新日志
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public bool UpdateStatisticsLog(CodStatsLogModel codStatsLog)
        {

            string sql = @"UPDATE FMS_CodStatsLog
					SET    IsSuccess = :IsSuccess,
						   Ip = :Ip,
						   Reasons = :Reasons,
						   UpdateTime = SYSDATE
					WHERE  StatisticsType = :StatisticsType
						   AND StatisticsDate = :StatisticsDate
						   AND WareHouseID = :WareHouseID
						   AND ExpressCompanyID = :ExpressCompanyID
						   AND WareHouseType=:WareHouseType
                           AND MerchantID=:MerchantID
						   --AND IsSuccess=0";
            OracleParameter[] parameters = {
											new OracleParameter(":StatisticsDate",OracleDbType.Date),
											new OracleParameter(":IsSuccess",OracleDbType.Decimal),
											new OracleParameter(":Reasons",OracleDbType.Varchar2,1000),
											new OracleParameter(":Ip",OracleDbType.Varchar2,100),
											new OracleParameter(":StatisticsType",OracleDbType.Decimal),
											new OracleParameter(":WareHouseID",OracleDbType.Varchar2,40),
											new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
											new OracleParameter(":WareHouseType",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsDate;
            parameters[1].Value = codStatsLog.IsSuccess;
            parameters[2].Value = codStatsLog.Reasons;
            parameters[3].Value = codStatsLog.Ip;
            parameters[4].Value = codStatsLog.StatisticsType;
            parameters[5].Value = codStatsLog.WareHouseID;
            parameters[6].Value = codStatsLog.ExpressCompanyID;
            parameters[7].Value = codStatsLog.WareHouseType;
            parameters[8].Value = codStatsLog.MerchantID;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public bool WriteStatisticsLog(CodStatsLogModel codStatsLog)
        {
            string Sql = @"INSERT INTO FMS_CodStatsLog(CODSTATSID,StatisticsType,StatisticsDate,IsSuccess,
                                                        Reasons,Ip,WareHouseID,ExpressCompanyID,WareHouseType,MerchantID,
                                                        createtime,updatetime,ischange)
								VALUES (:CODSTATSID,:StatisticsType,to_date(:StatisticsDate,'yyyy-mm-dd'),:IsSuccess,
                                        :Reasons,:Ip,:WareHouseID,:ExpressCompanyID,:WareHouseType,:MerchantID,sysdate,sysdate,1)";
            OracleParameter[] parameters = {
											new OracleParameter(":StatisticsType",OracleDbType.Decimal),
											new OracleParameter(":StatisticsDate",OracleDbType.Varchar2),
											new OracleParameter(":IsSuccess",OracleDbType.Decimal),
											new OracleParameter(":Reasons",OracleDbType.Varchar2,1000),
											new OracleParameter(":Ip",OracleDbType.Varchar2,100),
											new OracleParameter(":WareHouseID",OracleDbType.Varchar2,40),
											new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
											new OracleParameter(":WareHouseType",OracleDbType.Decimal),
                                            new OracleParameter(":MerchantID",OracleDbType.Decimal),
                                            new OracleParameter(":CODSTATSID",OracleDbType.Decimal)
										};
            parameters[0].Value = codStatsLog.StatisticsType;
            parameters[1].Value = codStatsLog.StatisticsDate.ToString("yyyy-MM-dd");
            parameters[2].Value = codStatsLog.IsSuccess;
            parameters[3].Value = codStatsLog.Reasons;
            parameters[4].Value = codStatsLog.Ip;
            parameters[5].Value = codStatsLog.WareHouseID;
            parameters[6].Value = codStatsLog.ExpressCompanyID;
            parameters[7].Value = codStatsLog.WareHouseType;
            parameters[8].Value = codStatsLog.MerchantID;
            parameters[9].Value = GetIdNew("seq_fms_codstatslog");
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, Sql, parameters) > 0;
        }

        /// <summary>
        /// 查询历史错误
        /// </summary>
        /// <param name="statisticsType"></param>
        /// <param name="dateRemove"></param>
        /// <returns></returns>
        public List<CodStatsLogModel> GetStatsLogError(int statisticsType, string dateRemove, int accountDays)
        {
            string Sql = @"SELECT DISTINCT StatisticsDate,
								   ExpressCompanyID,
								   WareHouseID,
								   0 AS FormCount,
								   StatisticsType,
								   WareHouseType,
                                   MerchantID
							FROM   FMS_CodStatsLog
							WHERE  StatisticsType = :StatisticsType
								   AND IsSuccess = 0
								   --AND StatisticsDate <> to_date(:StatisticsDate,'yyyy-mm-dd')
								   AND CreateTime > SYSDATE -:BeforeCreateTimeDays";

            //            string Sql = @"SELECT DISTINCT StatisticsDate,
            //								   ExpressCompanyID,
            //								   WareHouseID,
            //								   0 AS FormCount,
            //								   StatisticsType,
            //								   WareHouseType
            //							FROM   FMS_CodStatsLogWHERE  StatisticsType = @StatisticsType and CodStatsID in (990,1008)";
            OracleParameter[] parameters = { 
											new OracleParameter(":StatisticsType",OracleDbType.Decimal),
											//new OracleParameter(":StatisticsDate",OracleDbType.Varchar2),
                                            new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
										};
            parameters[0].Value = statisticsType;
            //parameters[1].Value = dateRemove;
            parameters[1].Value = accountDays;
            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sql, parameters).Tables[0];

            return TransformToCodStatsLogModel(dt);
        }
        #endregion

        #region 检查错误
        public DataTable GetDeliver(int accountDays)
        {
            string strSql = @"

WITH t AS (
SELECT fcbi.infoID ID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1','3') AND fcbi.DeliverTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.infoID ID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1','3') AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.infoID ID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1','3') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.infoID ID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
)
SELECT ID FROM t
";

            OracleParameter[] parameters ={
                                           new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public bool ChangeDeliverBack(List<string> noList)
        {
            string updateSql = " UPDATE FMS_CODBaseInfo SET  IsFare=0,IsChange=1,UpdateTime=sysdate WHERE  infoID = :infoID{0} ";
            StringBuilder sbSql = new StringBuilder();
            List<OracleParameter> parList = new List<OracleParameter>();
            parList.Add(new OracleParameter(":all_Count", OracleDbType.Decimal) { Direction = ParameterDirection.Output });
            int n = 0;
            foreach (string s in noList)
            {
                sbSql.AppendFormat(updateSql, n);
                parList.Add(new OracleParameter(":infoID" + n, s));

                sbSql.Append(" ;  :all_Count := :all_Count + sql%rowcount;");
                n++;
            }
            string sql = string.Format(@"begin
                            :all_Count :=0;
                            {0}
                            end;", sbSql.ToString());
            OracleParameter[] parameterList = parList.ToArray();
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameterList);
            bool flag = DataConvert.ToInt(parameterList[0].Value, 0) > 0;
            return flag;
        }

        public DataTable GetError8(int accountDays)
        {
            string strSql = @"
WITH t AS (
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
)
SELECT WaybillNO ,AreaID FROM t";
            OracleParameter[] parameters ={
                                           new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError9(int accountDays)
        {
            string strSql = @"

WITH t AS (
SELECT fcbi.WaybillNO  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
)
SELECT WaybillNO FROM t";
            OracleParameter[] parameters ={
                                           new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        /// <summary>
        /// 不存在配送商或仓库
        /// </summary>
        /// <returns></returns>
        public DataTable GetError7(int accountDays)
        {
            string strSql = @"
WITH t AS (
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
)
SELECT WaybillNO ,DeliverStationID ,ExpressCompanyID ,WarehouseId FROM t";
            OracleParameter[] parameters ={
                                           new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError6(int accountDays)
        {
            string strSql = @"
WITH t AS (
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
)
    SELECT  DISTINCT
            DeliverStationID ,
            ec.CompanyName
    FROM    t
            JOIN ExpressCompany ec  ON t.DeliverStationID = ec.ExpressCompanyID";
            OracleParameter[] parameters ={
                                           new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError5(int accountDays)
        {
            string strSql = @"
WITH t AS (
SELECT fcbi.WaybillNO  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO   FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO   FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=sysdate-:BeforeCreateTimeDays
UNION
SELECT fcbi.WaybillNO  FROM FMS_CODBaseInfo fcbi WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=sysdate-:BeforeCreateTimeDays
)
SELECT WaybillNO FROM t";
            OracleParameter[] parameters ={
                                           new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError34(int errorType, int accountDays)
        {
            string strSql = @"
            WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.MerchantID ,
                        fcbi.DeliverStationID ,
                        fcbi.ExpressCompanyID ,
                        fcbi.WarehouseId,
                        fcbi.FinalExpressCompanyID,
                        fcbi.AreaID
               FROM     FMS_CODBaseInfo fcbi WHERE fcbi.IsFare = :ErrorType 
						AND fcbi.DeliverTime>sysdate-:BeforeCreateTimeDays
             )
    SELECT  DISTINCT
            t.MerchantID ,
            mbi.MerchantName,
            t.DeliverStationID ,
            ec.CompanyName,
            CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
								CASE WHEN t.MerchantID IN ( 8, 9 )
								THEN t.Warehouseid
								ELSE cast(t.ExpressCompanyID as varchar2(40))END 
                          ELSE
                          cast(t.FinalExpressCompanyID as varchar2(40)) END Warehouseid,
			CASE WHEN t.FinalExpressCompanyID=0 or t.FinalExpressCompanyID is null THEN 
								CASE WHEN t.MerchantID IN ( 8, 9 )
								THEN w.WarehouseName
								ELSE w1.CompanyName END 
                          ELSE
                          w2.CompanyName END WarehouseName,
            a.AreaName,
            t.AreaID
    FROM    t
            JOIN area a  ON a.AreaID = t.AreaID
			JOIN ExpressCompany ec ON ec.ExpressCompanyID=t.DeliverStationID
			LEFT JOIN ExpressCompany w1 ON w1.ExpressCompanyID=t.ExpressCompanyID AND w1.CompanyFlag=1
			LEFT JOIN ExpressCompany w2 ON w2.ExpressCompanyID=t.FinalExpressCompanyID AND w2.CompanyFlag=1
			LEFT JOIN Warehouse w ON w.WarehouseId=t.WarehouseId
			JOIN MerchantBaseInfo mbi  ON mbi.ID=t.MerchantID";
            OracleParameter[] parameters ={
										   new OracleParameter(":ErrorType",OracleDbType.Decimal),
                                           new OracleParameter(":BeforeCreateTimeDays",OracleDbType.Decimal)
									  };
            parameters[0].Value = errorType;
            parameters[1].Value = accountDays;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }
        #endregion


        public DataTable GetDeliveryList(FMS_CODBaseInfoCheck model)
        {
            string Sqlstr =
                @"
 
  WITH t AS ( 
                SELECT   fcbi.MerchantID ,
                         fcbi.TopCODCompanyID ,
                         fcbi.WaybillNO,ael.AreaType,
                         fcbi.Accountweight,
                         fcbi.fare,
                         fcbi.address,
                         fcbi.delivertime,
                         fcbi.finalexpresscompanyid,
                         fcbi.Warehouseid,
                         fcbi.ExpressCompanyID,
                         fcbi.deliverstationid,
                         fcbi.WaybillType
                FROM     FMS_CODBaseInfo fcbi					
                LEFT JOIN AreaExpressLevel ael ON ael.AreaID = fcbi.AreaID
                                 AND ael.IsEnable IN (1, 2)
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ael.WareHouseID IS NULL
								
               WHERE ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') 
               AND fcbi.DistributionCode= :DistributionCode
               AND fcbi.IsDeleted=0  
               AND fcbi.TopCODCompanyID = :DistributionCompany  
               AND fcbi.Flag=1  
               AND fcbi.WaybillType IN ('0','1')  
               AND fcbi.DeliverTime>=:StartTime
               AND fcbi.DeliverTime<:EndTime
";  
           if(!string.IsNullOrEmpty(model.MerchantIDs))
           {
               Sqlstr += string.Format(@"  AND fcbi.MerchantID in ({0})", model.MerchantIDs);
           }
           Sqlstr +=@"
                --谁配送的
               UNION ALL
               SELECT  fcbi.MerchantID ,
                       fcbi.TopCODCompanyID ,
                       fcbi.WaybillNO,
                       ael.AreaType,
                       fcbi.Accountweight,
                       fcbi.fare,
                       fcbi.address,
                       fcbi.delivertime,
                       fcbi.finalexpresscompanyid,
                       fcbi.Warehouseid ,
                       fcbi.ExpressCompanyID,
                       fcbi.deliverstationid,
                       fcbi.WaybillType
               FROM    FMS_CODBaseInfo fcbi
						   JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               LEFT JOIN AreaExpressLevel ael ON ael.AreaID = fcbi.AreaID
                         AND ael.IsEnable IN (1, 2 )
                         AND ael.expresscompanyid = fcbi.TopCodCompanyID
                         AND ael.MerchantID = fcbi.MerchantID
                         AND ael.WareHouseID IS NULL
				WHERE ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') 
                         AND fcbi.DistributionCode<>:DistributionCode
                         AND ec.DistributionCode= :DistributionCode 
                         AND fcbi.IsDeleted=0  
                         AND fcbi.TopCODCompanyID =:DistributionCompany    
                         AND fcbi.Flag=1  
                         AND fcbi.WaybillType IN ('0','1')  
                         AND fcbi.DeliverTime>=:StartTime 
                         AND fcbi.DeliverTime<:EndTime 
              ";
           if (!string.IsNullOrEmpty(model.MerchantIDs))
           {
               Sqlstr += string.Format(@"  AND fcbi.MerchantID in ({0})", model.MerchantIDs);
           }

            Sqlstr += @"
             )
    SELECT  
			mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
            t.AreaType 区域类型 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            t.Fare AS 配送费,
            t.Address AS 收货人地址 ,
            t.DeliverTime AS 最终发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
            	 ec3.CompanyName
				 ELSE ec4.CompanyName END
            END AS 末级发货仓名称 ,
             t.WaybillType AS 订单类型,
            '1' AS 状态 
    FROM    t
            JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN ExpressCompany ec2 ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1 ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":DistributionCompany", OracleDbType.Int32),
                                               new OracleParameter(":DistributionCode",OracleDbType.NVarchar2), 
                                               new OracleParameter(":StartTime", OracleDbType.Date),
                                               new OracleParameter(":EndTime", OracleDbType.Date)
                                           };
            parameters[0].Value = model.DistributionCompany;
            parameters[1].Value = model.DistributionCode;
            parameters[2].Value = model.STime;
            parameters[3].Value = model.ETime;

           var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sqlstr, parameters);
           return ds.Tables[0];

        }


        public DataTable GetReturnList(FMS_CODBaseInfoCheck model)
        {
            string Sqlstr =
                @"WITH    t AS ( 
           	
            --谁接的单
            
			SELECT	fcbi.MerchantID,
			        fcbi.TopCODCompanyID,
			        ael.AreaType,
			        fcbi.WaybillNo,
			        fcbi.AccountWeight,
			        fcbi.fare,
			        fcbi.Address,
			        fcbi.DeliverTime,
			        fcbi.finalexpresscompanyid,
                    fcbi.Warehouseid ,
                    fcbi.ExpressCompanyID,
                    fcbi.WaybillType   ,
                    fcbi.DeliverStationID 
                    FROM FMS_CODBaseInfo  fcbi 
						
                    LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = fcbi.AreaID
                                 AND ael.ISEnable IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ael.WareHouseID IS NULL
								WHERE ( 1 = 1 ) AND fcbi.OperateType not in (2,5) 
								AND fcbi.DistributionCode=:DistributionCode  
								AND fcbi.IsDeleted=0  
								AND fcbi.TopCODCompanyID =:DistributionCompany
								AND fcbi.Flag=0  
								AND fcbi.WaybillType IN ('0','1')  
								AND fcbi.ReturnTime>=:StartTime
								AND fcbi.ReturnTime<:EndTime  
  ";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr += @"
            UNION ALL
			SELECT	fcbi.MerchantID,
			        fcbi.TopCODCompanyID,
			        ael.AreaType,
			        fcbi.WaybillNo,
			        fcbi.AccountWeight,
			        fcbi.fare,
			        fcbi.Address,
			        fcbi.DeliverTime,
			        fcbi.finalexpresscompanyid,
                    fcbi.Warehouseid ,
                    fcbi.ExpressCompanyID,
                    fcbi.WaybillType 
                    ,fcbi.DeliverStationID 
                    FROM  FMS_CODBaseInfo  fcbi 
			            
                    LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = fcbi.AreaID
                                 AND ael.ISEnable IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ael.WareHouseID IS NULL
				     WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) 
				     AND fcbi.DistributionCode=:DistributionCode  
				     AND fcbi.IsDeleted=0  
				     AND fcbi.TopCODCompanyID =:DistributionCompany 
				     AND fcbi.Flag=0  
				     AND fcbi.WaybillType IN ('0','1')  
				     AND fcbi.CreateTime>= :StartTime 
             AND fcbi.CreateTime<:EndTime
			";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr +=
                @"
            UNION ALL
            --谁配送的单
            SELECT	fcbi.MerchantID,
			        fcbi.TopCODCompanyID,
			        ael.AreaType,
			        fcbi.WaybillNo,
			        fcbi.AccountWeight,
			        fcbi.fare,
			        fcbi.Address,
			        fcbi.DeliverTime,
			        fcbi.finalexpresscompanyid,
              fcbi.Warehouseid ,
              fcbi.ExpressCompanyID,
              fcbi.WaybillType   
              ,fcbi.DeliverStationID 
              FROM  FMS_CODBaseInfo  fcbi 
              JOIN ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
						
              LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = fcbi.AreaID
                       AND ael.ISEnable IN (1, 2 )
                       AND ael.expresscompanyid = fcbi.TopCodCompanyID
                       AND ael.MerchantID = fcbi.MerchantID
                      AND ael.WareHouseID IS NULL
					WHERE ( 1 = 1 ) AND fcbi.OperateType not in (2,5) 
				    AND fcbi.DistributionCode<>:DistributionCode 
				    AND ec.DistributionCode= :DistributionCode  
				    AND fcbi.IsDeleted=0  
				    AND fcbi.TopCODCompanyID = :DistributionCompany 
				    AND fcbi.Flag=0  
				    AND fcbi.WaybillType IN ('0','1')  
				    AND fcbi.ReturnTime>=:StartTime 
				    AND fcbi.ReturnTime<:EndTime 
";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr +=@"
      UNION ALL
			SELECT	fcbi.MerchantID,
			        fcbi.TopCODCompanyID,
			        ael.AreaType,
			        fcbi.WaybillNo,
			        fcbi.AccountWeight,
			        fcbi.fare,
			        fcbi.Address,
			        fcbi.DeliverTime,
			        fcbi.finalexpresscompanyid,
                    fcbi.Warehouseid ,
                    fcbi.ExpressCompanyID,
                    fcbi.WaybillType    ,
                    fcbi.DeliverStationID
                    FROM  FMS_CODBaseInfo  fcbi 
                    JOIN  ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
			        LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = fcbi.AreaID
                                 AND ael.ISEnable IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ael.WareHouseID IS NULL
				    WHERE ( 1 = 1 ) AND fcbi.OperateType in (2,5) 
				    AND fcbi.DistributionCode<>:DistributionCode 
				    AND ec.DistributionCode= :DistributionCode
				    AND fcbi.IsDeleted=0  
				    AND fcbi.TopCODCompanyID = :DistributionCompany
				    AND fcbi.Flag=0  AND fcbi.WaybillType IN ('0','1')  
				    AND fcbi.CreateTime>=:StartTime 
				    AND fcbi.CreateTime<:EndTime 
";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr +=@"
             )
     select mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
             t.AreaType 区域类型 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            t.Fare AS 配送费,
            t.Address AS 收货人地址 ,
            t.DeliverTime AS 最终发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
            	 ec3.CompanyName
				 ELSE ec4.CompanyName END
            END AS 末级发货仓名称,
            t.WaybillType AS 订单类型,
            '2' AS 状态 
            
    FROM    t
            JOIN MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
            JOIN ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN ExpressCompany ec2 ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN Warehouse wh ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
	        
	        ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":DistributionCode", OracleDbType.NVarchar2),
                                               new OracleParameter(":DistributionCompany", OracleDbType.Int32),
                                               new OracleParameter(":StartTime",OracleDbType.Date),
                                               new OracleParameter(":EndTime",OracleDbType.Date) 
                                           };
            parameters[0].Value = model.DistributionCode;
            parameters[1].Value = model.DistributionCompany;
            parameters[2].Value = model.STime;
            parameters[3].Value = model.ETime;

            var ds =OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sqlstr, parameters);
            return ds.Tables[0];
        }


        public DataTable GetVisitList(FMS_CODBaseInfoCheck model)
        {
            string Sqlstr =
                @"
              WITH    t AS ( 
               

                --谁接的
      SELECT  fcbi.MerchantID,
			        fcbi.TopCODCompanyID,
			        ael.AreaType,
			        fcbi.WaybillNo,
			        fcbi.AccountWeight,
			        fcbi.fare,
			        fcbi.Address,
			        fcbi.DeliverTime,
			        fcbi.finalexpresscompanyid,
                    fcbi.Warehouseid ,
                    fcbi.ExpressCompanyID,
                    fcbi.WaybillType   
                    ,fcbi.DeliverStationID  
                    
                    FROM    FMS_CODBaseInfo  fcbi 
						    LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = fcbi.AreaID
                                 AND ael.IsEnable IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ael.WareHouseID IS NULL
								
                    WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>to_date('2012-05-22 23:59:59','yyyy-mm-dd hh24:mi:ss') 
                              AND fcbi.DistributionCode=:DistributionCode  
                              AND fcbi.IsDeleted=0  
                              AND fcbi.TopCODCompanyID = :DistributionCompany
                              AND fcbi.Flag=1  
                              AND fcbi.WaybillType IN ('1','2')  
                              AND fcbi.ReturnTime>= :StartTime
                              AND fcbi.ReturnTime<:EndTime
";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr +=@"
                UNION ALL
                --谁配送的
      SELECT  fcbi.MerchantID,
			        fcbi.TopCODCompanyID,
			        ael.AreaType,
			        fcbi.WaybillNo,
			        fcbi.AccountWeight,
			        fcbi.fare,
			        fcbi.Address,
			        fcbi.DeliverTime,
			        fcbi.finalexpresscompanyid,
                    fcbi.Warehouseid ,
                    fcbi.ExpressCompanyID,
                    fcbi.WaybillType   
                    ,fcbi.DeliverStationID 
                    
                    FROM FMS_CODBaseInfo  fcbi 
				                  JOIN  PS_PMS.ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
                          LEFT JOIN AreaExpressLevel ael  ON ael.AreaID = fcbi.AreaID
                                 AND ael.IsEnable IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ael.WareHouseID Is NULL
					 WHERE ( 1 = 1 ) AND fcbi.RfdAcceptTime> to_date('2012-05-22 ','yyyy-mm-dd hh24:mi:ss')
					                 AND fcbi.DistributionCode<>:DistributionCode 
					                 AND ec.DistributionCode= :DistributionCode  
					                 AND fcbi.IsDeleted=0  
					                 AND fcbi.TopCODCompanyID =:DistributionCompany 
					                 AND fcbi.Flag=1  
					                 AND fcbi.WaybillType IN ('1','2')  
					                 AND fcbi.ReturnTime>=:StartTime
					                 AND fcbi.ReturnTime<:EndTime
            ";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr +=@"
             )
             select 
             mbi.MerchantName AS 商家 ,
            CASE WHEN NVL(ec.AccountCompanyName,'')=''  THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
             t.AreaType 区域类型 ,
            t.WaybillNO AS 订单号 ,
            NVL(t.AccountWeight, 0) AS 结算重量 ,
            t.Fare AS 配送费,
            t.Address AS 收货人地址 ,
            t.DeliverTime AS 最终发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
            	 ec3.CompanyName
				 ELSE ec4.CompanyName END
            END AS 末级发货仓名称,
            t.WaybillType AS 订单类型,
            '3' AS 状态 
            
    FROM    t
            JOIN PS_PMS.MerchantBaseInfo mbi ON t.MerchantID = mbi.ID
            JOIN PS_PMS.ExpressCompany ec ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN PS_PMS.ExpressCompany ec2 ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN PS_PMS.Warehouse wh ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN PS_PMS.ExpressCompany ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN PS_PMS.ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
            ";
          OracleParameter []parameters = {
                                             new OracleParameter(":DistributionCode",OracleDbType.NVarchar2),
                                             new OracleParameter(":DistributionCompany",OracleDbType.Int32),
                                             new OracleParameter(":StartTime",OracleDbType.Date),
                                             new OracleParameter(":EndTime",OracleDbType.Date) 
                                         };
       
            parameters[0].Value = model.DistributionCode;
            parameters[1].Value = model.DistributionCompany;
            parameters[2].Value = model.STime;
            parameters[3].Value = model.ETime;

            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sqlstr, parameters);
            return ds.Tables[0];
        }


        public DataTable GetDeliverFeeParameter(long waybillNo, string distributionCode)
        {
            string sql = @"select baseInfo.FareFormula Formula,
                    baseInfo.InfoID InfoID,
	                baseInfo.AccountWeight Weight,
	                baseInfo.AreaType Area,
	                baseInfo.Fare DeliverFee,
                    baseInfo.IsFare
                from FMS_CODBaseInfo baseInfo
                where IsDeleted=0 
                    and baseInfo.WaybillNO=:WaybillNO 
                    and baseInfo.DistributionCode=:DistributionCode
                order by baseInfo.CreateTime desc";

            OracleParameter[] parameters =
            {
                new OracleParameter(":WaybillNO",OracleDbType.Int64){ Value=waybillNo },
                new OracleParameter(":DistributionCode",OracleDbType.Varchar2){ Value=distributionCode }
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public bool SaveDeliverFee(MODEL.FinancialManage.DeliverFeeModel model)
        {
            string sql = @"update FMS_CODBaseInfo 
                set FareFormula=:FareFormula,
                    AccountWeight=:AccountWeight,
                    AreaType=:AreaType,
                    Fare=:Fare,
                    IsChange=1,
                    IsFare=1
                where WaybillNO=:WaybillNO";

            OracleParameter[] parameters =
            {
                new OracleParameter(":FareFormula",OracleDbType.Varchar2){ Value=model.Formula },
                new OracleParameter(":AccountWeight",OracleDbType.Decimal){ Value=model.Weight },
                new OracleParameter(":AreaType",OracleDbType.Int32){ Value=model.Area },
                new OracleParameter(":Fare",OracleDbType.Decimal){ Value=model.DeliverFee },
                new OracleParameter(":WaybillNO",OracleDbType.Int64){ Value=model.WaybillNO }
            };

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }


        public bool UpdateEvalStatus(long waybillNo)
        {
            string sql = @"update FMS_CODBaseInfo set IsFare=0 where WaybillNO=:WaybillNO";

            OracleParameter[] parameters =
            {
                new OracleParameter(":WaybillNO",OracleDbType.Int64){ Value = waybillNo }
            };

            if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) != 1) return false;

            return true;
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            string sql = String.Format(@"update FMS_CODBaseInfo set IsFare=0 where WaybillNO in ({0})", waybillNos);

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }


        public DataTable GetCODDeliveryFeeInfo(string InfoIDs)
        {
            string[] ss = InfoIDs.Split(',');
            string param = "";
            for (int i = 0; i < ss.Length; i++)
            {
                param += ":ID" + i + ",";
            }
            param = param.Trim(',');
            string sql =string.Format(
                @"select Info.InfoID,Info.AreaType,Info.WaybillNo,Info.accountweight,Info.FareFormula,Info.Fare,ec.CompanyName
                           from FMS_CODBaseInfo Info Join PS_PMS.ExpressCompany ec 
                           on Info.TopCODCompanyID = ec.ExpressCompanyID 
                           where Info.InfoID in ({0})", InfoIDs);
            List<OracleParameter> parameters = new List<OracleParameter>();
            for (int i = 0; i < ss.Length; i++)
            {
                parameters.Add(new OracleParameter(":ID"+i,OracleDbType.Int64){Value = DataConvert.ToLong(ss[i])});
            }
            var ds =OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql);
            return ds.Tables[0];
        }


        public bool SaveDeliverFeeByID(MODEL.FinancialManage.DeliverFeeModel model)
        {
            string sql = @"update FMS_CODBaseInfo 
                set FareFormula=:FareFormula,
                    AccountWeight=:AccountWeight,
                    AreaType=:AreaType,
                    Fare=:Fare,
                    IsChange=1,
                    IsFare=1
                where InfoID=:InfoID";

            OracleParameter[] parameters =
            {
                new OracleParameter(":FareFormula",OracleDbType.Varchar2){ Value=model.Formula },
                new OracleParameter(":AccountWeight",OracleDbType.Decimal){ Value=model.Weight },
                new OracleParameter(":AreaType",OracleDbType.Int32){ Value=model.Area },
                new OracleParameter(":Fare",OracleDbType.Decimal){ Value=model.DeliverFee },
                new OracleParameter(":InfoID",OracleDbType.Int64){ Value=model.InfoID }
            };

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }


        public bool ExsitCodBaseInfoByNo(long waybillNo,long infoID)
        {
            string sql = @"select count(1) from FMS_CODBaseInfo where WaybillNo =:WaybillNo and InfoID =:InfoID";
            OracleParameter[] parameter = {
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo},
                                              new OracleParameter(":InfoID",OracleDbType.Int64){Value = infoID}
                                          };
            var ret =OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameter);
            return Convert.ToInt32(ret) > 0;
        }


        public bool UpdateEvalStatusByInfoID(string infoIDs)
        {
            string sql = String.Format(@"update FMS_CODBaseInfo set IsFare=0 where InfoID in ({0})", infoIDs);

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }
    }
}
