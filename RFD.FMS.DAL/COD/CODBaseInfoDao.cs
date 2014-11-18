using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.COD;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.COD;
using System.Collections.Generic;
using RFD.FMS.AdoNet;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data.Extension;

namespace RFD.FMS.DAL.COD
{
    public class CODBaseInfoDao : SqlServerDao, ICODBaseInfoDao
	{
        public int Add(FMS_CODBaseInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into LMS_RFD.dbo.FMS_CODBaseInfo(");
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
            strSql.Append(" @MediumID , ");
            strSql.Append(" @WaybillNO , ");
            strSql.Append(" @MerchantID , ");
            strSql.Append(" @WaybillType , ");
            strSql.Append(" @Flag , ");
            strSql.Append(" @DeliverStationID , ");
            strSql.Append(" @TopCODCompanyID , ");
            strSql.Append(" @WarehouseId , ");
            strSql.Append(" @ExpressCompanyID , ");
            strSql.Append(" @RfdAcceptTime , ");
            strSql.Append(" @RfdAcceptDate , ");
            strSql.Append(" @FinalExpressCompanyID , ");
            strSql.Append(" @DeliverTime , ");
            strSql.Append(" @DeliverDate , ");
            strSql.Append(" @ReturnWareHouseID , ");
            strSql.Append(" @ReturnExpressCompanyID , ");
            strSql.Append(" @TotalAmount , ");
            strSql.Append(" @PaidAmount , ");
            strSql.Append(" @NeedPayAmount , ");
            strSql.Append(" @BackAmount , ");
            strSql.Append(" @NeedBackAmount , ");
            strSql.Append(" @AccountWeight , ");
            strSql.Append(" @AreaID , ");
            strSql.Append(" @AreaType , ");
            strSql.Append(" @BoxsNo , ");
            strSql.Append(" @Address , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @IsDeleted , ");
            strSql.Append(" @ReturnTime , ");
            strSql.Append(" @ReturnDate , ");
            strSql.Append(" @IsFare , ");
            strSql.Append(" @Fare , ");
            strSql.Append(" @FareFormula , ");
            strSql.Append(" @OperateType , ");
            strSql.Append(" @ProtectedPrice , ");
            strSql.Append(" @DistributionCode , ");
            strSql.Append(" @CurrentDistributionCode,  ");
            strSql.Append(" @IsChange  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
											new SqlParameter(string.Format("@{0}","MediumID"), model.MediumID),
											new SqlParameter(string.Format("@{0}","WaybillNO"), model.WaybillNO),
											new SqlParameter(string.Format("@{0}","MerchantID"), model.MerchantID),
											new SqlParameter(string.Format("@{0}","WaybillType"), model.WaybillType),
											new SqlParameter(string.Format("@{0}","Flag"), model.Flag),
											new SqlParameter(string.Format("@{0}","DeliverStationID"), model.DeliverStationID),
											new SqlParameter(string.Format("@{0}","TopCODCompanyID"), model.TopCODCompanyID),
											new SqlParameter(string.Format("@{0}","WarehouseId"), model.WarehouseId),
											new SqlParameter(string.Format("@{0}","ExpressCompanyID"), model.ExpressCompanyID),
											new SqlParameter(string.Format("@{0}","RfdAcceptTime"), model.RfdAcceptTime),
											new SqlParameter(string.Format("@{0}","RfdAcceptDate"), model.RfdAcceptDate),
											new SqlParameter(string.Format("@{0}","FinalExpressCompanyID"), model.FinalExpressCompanyID),
											new SqlParameter(string.Format("@{0}","DeliverTime"), model.DeliverTime),
											new SqlParameter(string.Format("@{0}","DeliverDate"), model.DeliverDate),
											new SqlParameter(string.Format("@{0}","ReturnWareHouseID"), model.ReturnWareHouseID),
											new SqlParameter(string.Format("@{0}","ReturnExpressCompanyID"), model.ReturnExpressCompanyID),
											new SqlParameter(string.Format("@{0}","TotalAmount"), model.TotalAmount),
											new SqlParameter(string.Format("@{0}","PaidAmount"), model.PaidAmount),
											new SqlParameter(string.Format("@{0}","NeedPayAmount"), model.NeedPayAmount),
											new SqlParameter(string.Format("@{0}","BackAmount"), model.BackAmount),
											new SqlParameter(string.Format("@{0}","NeedBackAmount"), model.NeedBackAmount),
											new SqlParameter(string.Format("@{0}","AccountWeight"), model.AccountWeight),
											new SqlParameter(string.Format("@{0}","AreaID"), model.AreaID),
											new SqlParameter(string.Format("@{0}","AreaType"), model.AreaType),
											new SqlParameter(string.Format("@{0}","BoxsNo"), model.BoxsNo),
											new SqlParameter(string.Format("@{0}","Address"), model.Address),
											new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
											new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
											new SqlParameter(string.Format("@{0}","IsDeleted"), model.IsDeleted),
											new SqlParameter(string.Format("@{0}","ReturnTime"), model.ReturnTime),
											new SqlParameter(string.Format("@{0}","ReturnDate"), model.ReturnDate),
											new SqlParameter(string.Format("@{0}","IsFare"), model.IsFare),
											new SqlParameter(string.Format("@{0}","Fare"), model.Fare),
											new SqlParameter(string.Format("@{0}","FareFormula"), model.FareFormula),
											new SqlParameter(string.Format("@{0}","OperateType"), model.OperateType),
											new SqlParameter(string.Format("@{0}","ProtectedPrice"), model.ProtectedPrice),
											new SqlParameter(string.Format("@{0}","DistributionCode"), model.DistributionCode),
											new SqlParameter(string.Format("@{0}","CurrentDistributionCode"), model.CurrentDistributionCode),
									        new SqlParameter(string.Format("@{0}","IsChange"), true)
                                         };

            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);

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
            strSql.Append("select ID, MediumID, WaybillNO, MerchantID, WaybillType, Flag, DeliverStationID, TopCODCompanyID, WarehouseId, ExpressCompanyID, RfdAcceptTime, RfdAcceptDate, FinalExpressCompanyID, DeliverTime, DeliverDate, ReturnWareHouseID, ReturnExpressCompanyID, TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, AreaType, BoxsNo, Address, CreateTime, UpdateTime, IsDeleted, ReturnTime, ReturnDate, IsFare, Fare, FareFormula, OperateType, ProtectedPrice, DistributionCode, CurrentDistributionCode  ");
            strSql.Append("  from LMS_RFD.dbo.FMS_CODBaseInfo ");
            strSql.Append(string.Format(" where {0} = @{0}", "ID"));

            var sqlParams = new List<SqlParameter>()
			{
				new SqlParameter(string.Format("@{0}","ID"),id)
			};

            var model = new FMS_CODBaseInfo();
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
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
            strSql.Append(@"SELECT fci.ID, fci.MediumID, fci.WaybillNO, fci.MerchantID, 
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
                            FROM LMS_RFD.dbo.FMS_CODBaseInfo fci(NOLOCK) 
                            WHERE fci.WaybillNo =@WaybillNo and fci.OperateType in (1,3)
                                ORDER BY fci.MediumID DESC");

            var sqlParams = new List<SqlParameter>()
			{
				new SqlParameter(string.Format("@{0}","WaybillNo"),waybillNo)
			};

            var model = new FMS_CODBaseInfo();

            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());

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
            strSql.Append("select ID, MediumID, WaybillNO, MerchantID, WaybillType, Flag, DeliverStationID, TopCODCompanyID, WarehouseId, ExpressCompanyID, RfdAcceptTime, RfdAcceptDate, FinalExpressCompanyID, DeliverTime, DeliverDate, ReturnWareHouseID, ReturnExpressCompanyID, TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, AreaType, BoxsNo, Address, CreateTime, UpdateTime, IsDeleted, ReturnTime, ReturnDate, IsFare, Fare, FareFormula, OperateType, ProtectedPrice, DistributionCode, CurrentDistributionCode  ");
            strSql.Append(" from LMS_RFD.dbo.FMS_CODBaseInfo(nolock) ");
            strSql.Append(" where 1 = 1 ");
            var sqlParams = new List<SqlParameter>();

            if (searchParams != null)
            {
                foreach (var item in searchParams)
                {
                    strSql.Append(string.Format(" and {0} = @{0}", item.Key));
                    sqlParams.Add(new SqlParameter(string.Format("@{0}", item.Key), item.Value));
                }
            }

            var modelList = new List<FMS_CODBaseInfo>();
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
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
            string sql = @"UPDATE LMS_RFD.dbo.FMS_CODBaseInfo SET NeedPayAmount = @NeedPayAmount,NeedBackAmount = @NeedBackAmount,IsChange=@IsChange WHERE ID=@ID";
            SqlParameter[] parameters ={
                                           new SqlParameter("@NeedPayAmount",SqlDbType.Decimal),
                                           new SqlParameter("@NeedBackAmount",SqlDbType.Decimal),
                                           new SqlParameter("@ID",SqlDbType.BigInt),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
                                      };

            parameters[0].Value = info.NeedPayAmount;
            parameters[1].Value = info.NeedBackAmount;
            parameters[2].Value = info.ID;
            parameters[3].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 将制定ID置为停用isdeleted=1
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateIsDeletedByID(FMS_CODBaseInfo info)
        {
            string sql = @"UPDATE LMS_RFD.dbo.FMS_CODBaseInfo SET IsDeleted=1,IsChange=1,UpdateTime=getdate() WHERE ID=@ID and IsDeleted=0";

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID",SqlDbType.BigInt)
            };

            parameters[0].Value = info.ID;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
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
DECLARE @CreateTimeEnd DATETIME
SET @CreateTimeEnd =DATEADD(""hh"",-1,GETDATE());
WITH    t AS ( SELECT TOP ( @Tops )
						fcbi.ID,
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
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.IsFare = 0
                        AND fcbi.DeliverTime > @CreatTimeStr
                        AND fcbi.DeliverTime < @CreateTimeEnd
                        AND fcbi.CreateTime<@CreateTimeEnd
             )
    SELECT  t.ID,
			t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            Warehouseid = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN t.MerchantID IN ( 8, 9 )
								THEN t.Warehouseid
								ELSE CONVERT(NVARCHAR(20), t.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) END,
            WareHouseType = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
									CASE WHEN t.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                            END ELSE 2 END,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
            t.TopCODCompanyID,
            t.DistributionCode,
            t.NeedPayAmount,
            t.NeedBackAmount
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCodCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''
";
            #endregion
            SqlParameter[] parameters ={
										   new SqlParameter("@Tops",SqlDbType.Int),
										   new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
									  };
            parameters[0].Value = string.IsNullOrEmpty(tops.ToString()) ? 100 : tops;
            parameters[1].Value = string.IsNullOrEmpty(syncStartTime) ?
                DateTime.Now.AddDays(-accountDays)
                : DateTime.Parse(syncStartTime);

            DataTable dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
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
            string strSql = @"
DECLARE @CreateTimeEnd DATETIME
SET @CreateTimeEnd =DATEADD(""hh"",-1,GETDATE());
WITH    t AS ( SELECT TOP ( @Tops )
                        fcbi.ID ,
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
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND WaybillType IN ( '0', '1' )
                        AND fcbi.Flag = 0
                        AND fcbi.IsFare = 0
                        AND fcbi.ReturnTime > @ReturnTimeStr 
                        AND fcbi.ReturnTime < @CreateTimeEnd
						AND fcbi.CreateTime<@CreateTimeEnd
              UNION ALL
			   SELECT   TOP ( @Tops )
						fcbi.ID ,
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
			   FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
			   WHERE    fcbi.IsDeleted = 0
                        AND WaybillType IN ( '0', '1' )
                        AND fcbi.IsFare = 0
                        AND fcbi.OperateType in (2,5) 
                        AND fcbi.Flag=0
                        AND fcbi.CreateTime > @ReturnTimeStr  
                        AND fcbi.CreateTime<@CreateTimeEnd
             )
    SELECT  t.ID,
			t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            t.ReturnTime ,
            ReturnWareHouse = CASE WHEN t.MerchantID IN ( 8, 9 )
                                   THEN t.ReturnWareHouseID
                                   ELSE CONVERT(NVARCHAR(20), t.ReturnExpressCompanyId)
                              END ,
            Warehouseid = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN t.MerchantID IN ( 8, 9 )
								THEN t.Warehouseid
								ELSE CONVERT(NVARCHAR(20), t.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) END,
            WareHouseType = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
									CASE WHEN t.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                            END ELSE 2 END,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
			t.TopCODCompanyID,
            t.DistributionCode,
            t.NeedPayAmount,
            t.NeedBackAmount
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCODCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''";
            SqlParameter[] parameters ={
										   new SqlParameter("@Tops",SqlDbType.Int),
										   new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
									  };
            parameters[0].Value = string.IsNullOrEmpty(tops.ToString()) ? 100 : tops;
            parameters[1].Value = string.IsNullOrEmpty(syncStartTime) ?
                DateTime.Now.AddDays(-accountDays)// DateTime.Parse("2012-01-31")
                : DateTime.Parse(syncStartTime);

            DataTable dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            return TransformToDetailModel(dt);
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
DECLARE @CreateTimeEnd DATETIME
SET @CreateTimeEnd =DATEADD(""hh"",-1,GETDATE());
WITH    t AS ( SELECT TOP ( @Tops )
                        fcbi.ID ,
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
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND WaybillType = '2'
                        AND fcbi.Flag = 1
                        AND fcbi.IsFare = 0
                        AND fcbi.ReturnTime > @ReturnTimeStr
						AND fcbi.CreateTime<@CreateTimeEnd
                        AND fcbi.ReturnTime < @CreateTimeEnd
             )
    SELECT  t.ID ,
            t.WaybillNo ,
            t.DeliverTime ,
            t.MerchantID ,
            t.DeliverStationID ,
            t.WaybillType ,
            t.ReturnTime ,
            ReturnWareHouse = CASE WHEN t.MerchantID IN ( 8, 9 )
                                   THEN t.ReturnWareHouseID
                                   ELSE CONVERT(NVARCHAR(20), t.ReturnExpressCompanyId)
                              END ,
            Warehouseid = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN t.MerchantID IN ( 8, 9 )
								THEN t.Warehouseid
								ELSE CONVERT(NVARCHAR(20), t.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) END,
            WareHouseType = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
									CASE WHEN t.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                            END ELSE 2 END,
            t.AccountWeight ,
            t.AreaID ,
            ael.AreaType,
            t.TopCODCompanyID,
            t.DistributionCode,
            t.NeedPayAmount,
            t.NeedBackAmount
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
                                                         AND ael.expresscompanyid = t.TopCODCompanyID
                                                         AND ael.MerchantID = t.MerchantID
                                                         --AND ISNULL(ael.WareHouseID,'') = ''";
            SqlParameter[] parameters ={
										   new SqlParameter("@Tops",SqlDbType.Int),
										   new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
									  };
            parameters[0].Value = string.IsNullOrEmpty(tops.ToString()) ? 100 : tops;
            parameters[1].Value = string.IsNullOrEmpty(syncStartTime) ?
                DateTime.Now.AddDays(-accountDays) // DateTime.Parse("2012-01-31")
                : DateTime.Parse(syncStartTime);

            DataTable dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
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
                detailModel.ID = long.Parse(dr["ID"].ToString());
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
            string sql = @"UPDATE LMS_RFD.dbo.FMS_CODBaseInfo
						SET    Fare = @Fare,
							   FareFormula = @FareFormula,IsCOD=@IsCOD,
							   IsFare=1,IsChange=1,AreaType=@AreaType,UpdateTime=getdate()
						WHERE  [ID] = @ID";
            SqlParameter[] parameters ={
										   new SqlParameter("@Fare",SqlDbType.Decimal),
										   new SqlParameter("@FareFormula",SqlDbType.NVarChar,150),
										   new SqlParameter("@ID",SqlDbType.BigInt),
                                           new SqlParameter("@IsCOD",SqlDbType.Int),
                                           new SqlParameter("@AreaType",SqlDbType.Int),
									  };
            parameters[0].Value = detail.Fare;
            parameters[1].Value = detail.FareFormula;
            parameters[2].Value = detail.ID;
            parameters[3].Value = detail.IsCOD;
            parameters[4].Value = detail.AreaType;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
            //return true;
        }

        public bool UpdateBackError(FMS_CODBaseInfo detail)
        {
            string sql = @"UPDATE LMS_RFD.dbo.FMS_CODBaseInfo
						SET   IsFare=@IsFare,IsChange=1,AreaType=@AreaType,UpdateTime=getdate()
						WHERE  [ID] = @ID";
            SqlParameter[] parameters ={
										   new SqlParameter("@ID",SqlDbType.BigInt),
										   new SqlParameter("@IsFare",SqlDbType.Int),
                                           new SqlParameter("@AreaType",SqlDbType.Int),
									  };
            parameters[0].Value = detail.ID;
            parameters[1].Value = detail.ErrorType;
            parameters[2].Value = string.IsNullOrEmpty(detail.AreaType.ToString()) || detail.AreaType ==0 ? -1 : detail.AreaType;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
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
                        Warehouseid = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID, 0) = 0
                                           THEN CASE WHEN fcbi.MerchantID IN ( 8, 9 )
                                                     THEN fcbi.Warehouseid
                                                     ELSE CONVERT(NVARCHAR(20), fcbi.ExpressCompanyID)
                                                END
                                           ELSE CONVERT(NVARCHAR(20), fcbi.FinalExpressCompanyID)
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,
                                                         0) = 0
                                             THEN CASE WHEN fcbi.MerchantID IN ( 8, 9 ) THEN 1 ELSE 2
                                                  END ELSE 2
                                        END
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= @CreatTimeStr
                        AND fcbi.DeliverTime < @CreatTimeEnd
             )
    SELECT  @CreatTimeStr AS StatisticsDate ,
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
            SqlParameter[] parameters = { 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
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
            SqlParameter[] parameters = { 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@Warehouseid",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;

            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
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
            SqlParameter[] parameters = { 
											new SqlParameter("@CreatTimeStr",SqlDbType.DateTime),
											new SqlParameter("@CreatTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@Warehouseid",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        private string GetDeliverHouseType1Sql(bool isFare)
        {
            string sql = @"
SELECT   COUNT(1)
FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
WHERE    fcbi.IsDeleted = 0
		AND fcbi.Flag=1
        AND fcbi.WaybillType IN ( '0', '1' )
        AND fcbi.DeliverTime >= @CreatTimeStr
        AND fcbi.DeliverTime < @CreatTimeEnd
        AND fcbi.DeliverStationID = @DeliverStationID
        AND fcbi.Warehouseid = @Warehouseid
        AND fcbi.MerchantID = @MerchantID
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
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= @CreatTimeStr
                        AND fcbi.DeliverTime < @CreatTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.ExpressCompanyID = @Warehouseid
                        AND fcbi.MerchantID = @MerchantID
                        AND ISNULL(fcbi.FinalExpressCompanyID, 0) = 0
                        {0}
               UNION ALL
               SELECT   fcbi.WaybillNO
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= @CreatTimeStr
                        AND fcbi.DeliverTime < @CreatTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.FinalExpressCompanyID = @Warehouseid
                        AND fcbi.MerchantID = @MerchantID
                        AND ISNULL(fcbi.FinalExpressCompanyID, 0) > 0
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
            SqlParameter[] parameters ={
										new SqlParameter("@CreatTimeStr",SqlDbType.Date),
										new SqlParameter("@CreatTimeEnd",SqlDbType.Date),
										new SqlParameter("@DeliverStationID",SqlDbType.Int),
										new SqlParameter("@WareHouseID",SqlDbType.NVarChar,20),
                                        new SqlParameter("@MerchantID",SqlDbType.Int)
									 };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;

            DataTable dt = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
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
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= @CreatTimeStr
                        AND fcbi.DeliverTime < @CreatTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
						AND fcbi.WareHouseID = @WareHouseID
                        AND fcbi.MerchantID = @MerchantID
                        --AND ISNULL(fcbi.FinalExpressCompanyID,0) = 0
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
			t.WareHouseID,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(ISNULL(t.AccountWeight,0)) AS WEIGHT ,
            t.WaybillType AS DeliveryType,
            1 AS WareHouseType,
			t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
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
                        Warehouseid = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE CONVERT(NVARCHAR(20), fcbi.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), fcbi.FinalExpressCompanyID) END,
                        fcbi.WaybillType,
                        fcbi.AccountWeight,
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.ExpressCompanyID,
                        fcbi.FareFormula,
                        fcbi.Fare
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= @CreatTimeStr
                        AND fcbi.DeliverTime < @CreatTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
						AND fcbi.ExpressCompanyID = @WareHouseID
                        AND fcbi.MerchantID = @MerchantID
                        AND ISNULL(fcbi.FinalExpressCompanyID,0) = 0
                UNION ALL
                SELECT   WaybillNO ,
                        DeliverStationID ,
                        Warehouseid = CASE WHEN ISNULL(fcbi.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN fcbi.MerchantID IN ( 8, 9 )
								THEN fcbi.Warehouseid
								ELSE CONVERT(NVARCHAR(20), fcbi.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), fcbi.FinalExpressCompanyID) END,
                        fcbi.WaybillType,
                        fcbi.AccountWeight,
                        fcbi.AreaID,
                        fcbi.TopCodCompanyID,
                        fcbi.MerchantID,
                        fcbi.ExpressCompanyID,
                        fcbi.FareFormula,
                        fcbi.Fare
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
						AND fcbi.Flag=1
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.DeliverTime >= @CreatTimeStr
                        AND fcbi.DeliverTime < @CreatTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
						AND fcbi.FinalExpressCompanyID = @WareHouseID
                        AND fcbi.MerchantID = @MerchantID
                        AND ISNULL(fcbi.FinalExpressCompanyID,0) > 0
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
			t.WareHouseID,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(ISNULL(t.AccountWeight,0)) AS WEIGHT ,
            t.WaybillType AS DeliveryType,
            2 AS WareHouseType,
			t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
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
            StringBuilder sbSql = new StringBuilder();
            string existsSql = @"IF NOT EXISTS(SELECT 1 FROM FMS_CODDeliveryCount(NOLOCK) WHERE WareHouseID = '{0}'
									AND ExpressCompanyID ={1} AND AreaType ={2} AND AccountDate='{3}' 
									AND DeliveryType='{4}' AND WareHouseType={5} AND Formula='{6}' AND MerchantID={7}) ";
            string insertSql = @" INSERT INTO FMS_CODDeliveryCount ( AccountNO,WareHouseID,ExpressCompanyID,
									AreaType,Weight,AccountDate,FormCount,Fare,Formula,CreateBy,CreateTime,
									UpdateBy,UpdateTime,DeliveryType,WareHouseType,MerchantID,IsChange)  VALUES ";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            int i = 0;
            string formart = "@{0}{1}{2}";
            foreach (CodStatsModel codStats in codStatsList)
            {
                sbSql.Append(string.Format(existsSql, codStats.WareHouseID, codStats.ExpressCompanyID,
                                codStats.AreaType, date, codStats.DeliveryType, codStats.WareHouseType, codStats.Formula, codStats.MerchantID));
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append("'',");
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "Weight", i, ","));
                sbSql.Append("'" + date + "',");
                sbSql.Append(string.Format(formart, "FormCount", i, ","));
                sbSql.Append(string.Format(formart, "Fare", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append("0,");
                sbSql.Append("GETDATE(),");
                sbSql.Append("'',");
                sbSql.Append("GETDATE(),");
                sbSql.Append(string.Format(formart, "DeliveryType", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "IsChange", i, ""));
                sbSql.Append(")");
                if (i < codStatsList.Count - 1)
                {
                    sbSql.Append(" \n ");
                }
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseID", i, ""), codStats.WareHouseID));
                parameterList.Add(new SqlParameter(string.Format(formart, "ExpressCompanyID", i, ""), codStats.ExpressCompanyID));
                parameterList.Add(new SqlParameter(string.Format(formart, "AreaType", i, ""), codStats.AreaType));
                parameterList.Add(new SqlParameter(string.Format(formart, "Weight", i, ""), codStats.Weight));
                parameterList.Add(new SqlParameter(string.Format(formart, "FormCount", i, ""), codStats.FormCount));
                parameterList.Add(new SqlParameter(string.Format(formart, "Fare", i, ""), codStats.Fare));
                parameterList.Add(new SqlParameter(string.Format(formart, "Formula", i, ""), codStats.Formula));
                parameterList.Add(new SqlParameter(string.Format(formart, "DeliveryType", i, ""), codStats.DeliveryType));
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseType", i, ""), codStats.WareHouseType));
                parameterList.Add(new SqlParameter(string.Format(formart, "MerchantID", i, ""), codStats.MerchantID));
                parameterList.Add(new SqlParameter(string.Format(formart, "IsChange", i, ""), 1));
                i++;
            }
            string sql = sbSql.ToString();
            SqlParameter[] parameters = parameterList.ToArray();
            //return true;
            int n = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);
            return n > 0;
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
                        WareHouseID = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.ReturnExpressCompanyID)
                                           ELSE fcbi.ReturnWareHouseID
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = '' THEN 2 ELSE 1
                                        END
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.Flag = 0
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.ReturnTime >= @ReturnTimeStr
                        AND fcbi.ReturnTime < @ReturnTimeEnd
             )
    SELECT  @ReturnTimeStr AS StatisticsDate ,
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
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt1 = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];


            sql = @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        fcbi.MerchantID,
                        WareHouseID = CASE WHEN ISNULL(fcbi.WarehouseId,'') = '' 
                                           THEN CONVERT(VARCHAR(20), fcbi.FinalExpressCompanyID)
                                           ELSE fcbi.WarehouseId
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.WarehouseId,'') = '' THEN 2 ELSE 1
                                        END
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.Flag = 0
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.CreateTime >= @ReturnTimeStr
                        AND fcbi.CreateTime < @ReturnTimeEnd
                        AND fcbi.OperateType in (2,5)
             )
    SELECT  @ReturnTimeStr AS StatisticsDate ,
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
            SqlParameter[] parameters1 = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime)
										};
            parameters1[0].Value = accountDate.ToShortDateString();
            parameters1[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt2 = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters1).Tables[0];

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
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnWareHouse",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
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
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnWareHouse",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        private string GetReturnHouseType1Sql(bool isFare)
        {
            string sql = @"

with t as(
SELECT  fcbi.WaybillNO
FROM    LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
WHERE   fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1' )
        AND fcbi.ReturnTime >= @ReturnTimeStr
        AND fcbi.ReturnTime < @ReturnTimeEnd
        AND fcbi.DeliverStationID = @DeliverStationID
        AND fcbi.ReturnWareHouseID = @ReturnWareHouse
        AND fcbi.MerchantID = @MerchantID
        {0}
UNION ALL
SELECT  fcbi.WaybillNO
FROM    LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
WHERE   fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1' )
        AND fcbi.OperateType in (2,5) 
        AND fcbi.CreateTime >= @ReturnTimeStr
        AND fcbi.CreateTime < @ReturnTimeEnd
        AND fcbi.DeliverStationID = @DeliverStationID
        AND fcbi.WarehouseId = @ReturnWareHouse
        AND fcbi.MerchantID = @MerchantID
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
FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
WHERE    fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1' )
        AND fcbi.ReturnTime >= @ReturnTimeStr
        AND fcbi.ReturnTime < @ReturnTimeEnd
        AND fcbi.DeliverStationID = @DeliverStationID
        AND fcbi.ReturnExpressCompanyID = @ReturnWareHouse
        AND fcbi.MerchantID = @MerchantID
        {0}
UNION ALL
SELECT fcbi.WaybillNO
FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
WHERE    fcbi.IsDeleted = 0
        AND fcbi.Flag = 0
        AND fcbi.WaybillType IN ( '0', '1' )
        AND fcbi.OperateType in (2,5) 
        AND fcbi.CreateTime >= @ReturnTimeStr
        AND fcbi.CreateTime < @ReturnTimeEnd
        AND fcbi.DeliverStationID = @DeliverStationID
        AND fcbi.FinalExpressCompanyID = @ReturnWareHouse
        AND fcbi.MerchantID = @MerchantID
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
            SqlParameter[] parameters ={
										new SqlParameter("@ReturnTimeStr",SqlDbType.Date),
										new SqlParameter("@ReturnTimeEnd",SqlDbType.Date),
										new SqlParameter("@DeliverStationID",SqlDbType.Int),
										new SqlParameter("@ReturnWareHouse",SqlDbType.NVarChar,40),
                                        new SqlParameter("@MerchantID",SqlDbType.Int),
									 };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            DataTable dt = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
            return TransformToCodStatsModel(dt);
        }

        private string ReturnDayStatByHouseType1()
        {
            return @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        WareHouseID = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.ReturnExpressCompanyID)
                                           ELSE fcbi.ReturnWareHouseID
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = '' THEN 2
                                             ELSE 1
                                        END ,
						fcbi.Fare,
						fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.Flag = 0
                        AND fcbi.ReturnTime >= @ReturnTimeStr
                        AND fcbi.ReturnTime < @ReturnTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.ReturnWareHouseID = @ReturnWareHouse
                        AND fcbi.MerchantID = @MerchantID
			UNION ALL
			SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        WareHouseID = CASE WHEN ISNULL(fcbi.WarehouseId,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.FinalExpressCompanyID)
                                           ELSE fcbi.WarehouseId
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.WarehouseId,'') = '' THEN 2
                                             ELSE 1
                                        END ,
						fcbi.Fare,
						fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.Flag = 0
                        AND fcbi.CreateTime >= @ReturnTimeStr
                        AND fcbi.CreateTime < @ReturnTimeEnd
                        AND fcbi.OperateType in (2,5) 
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.WarehouseId = @ReturnWareHouse
                        AND fcbi.MerchantID = @MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(ISNULL(t.AccountWeight, 0)) AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            1 AS WareHouseType,
			t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (
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
                        WareHouseID = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.ReturnExpressCompanyID)
                                           ELSE fcbi.ReturnWareHouseID
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = '' THEN 2
                                             ELSE 1
                                        END ,
						fcbi.Fare,
						fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.Flag = 0
                        AND fcbi.ReturnTime >= @ReturnTimeStr
                        AND fcbi.ReturnTime < @ReturnTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.ReturnExpressCompanyID = @ReturnWareHouse
                        AND fcbi.MerchantID = @MerchantID
			UNION ALL
			SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        WareHouseID = CASE WHEN ISNULL(fcbi.WarehouseId,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.FinalExpressCompanyID)
                                           ELSE fcbi.WarehouseId
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.WarehouseId,'') = '' THEN 2
                                             ELSE 1
                                        END ,
						fcbi.Fare,
						fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType IN ( '0', '1' )
                        AND fcbi.Flag = 0
                        AND fcbi.CreateTime >= @ReturnTimeStr
                        AND fcbi.CreateTime < @ReturnTimeEnd
                        AND fcbi.OperateType in (2,5) 
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.FinalExpressCompanyID = @ReturnWareHouse
                        AND fcbi.MerchantID = @MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(ISNULL(t.AccountWeight, 0)) AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            2 AS WareHouseType,
			t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (1, 2 )
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
            StringBuilder sbSql = new StringBuilder();
            string existsSql = @"IF NOT EXISTS(SELECT 1 FROM FMS_CODReturnsCount(NOLOCK) WHERE WareHouseID = '{0}' AND ExpressCompanyID ={1} AND AreaType ={2}
									 AND AccountDate='{3}' AND ReturnsType='{4}' AND WareHouseType={5} AND Formula='{6}' AND MerchantID={7}) ";
            string insertSql = @" INSERT INTO FMS_CODReturnsCount ( AccountNO,WareHouseID,ExpressCompanyID,
									AreaType,Weight,AccountDate,FormCount,Fare,Formula,CreateBy,CreateTime,
									UpdateBy,UpdateTime,ReturnsType,WareHouseType,MerchantID,IsChange)  VALUES ";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            int i = 0;
            string formart = "@{0}{1}{2}";
            foreach (CodStatsModel codStats in codStatsList)
            {
                sbSql.Append(string.Format(existsSql, codStats.WareHouseID, codStats.ExpressCompanyID,
                                codStats.AreaType, date, codStats.ReturnsType, codStats.WareHouseType, codStats.Formula, codStats.MerchantID));
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append("'',");
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "Weight", i, ","));
                sbSql.Append("'" + date + "',");
                sbSql.Append(string.Format(formart, "FormCount", i, ","));
                sbSql.Append(string.Format(formart, "Fare", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append("0,");
                sbSql.Append("GETDATE(),");
                sbSql.Append("'',");
                sbSql.Append("GETDATE(),");
                sbSql.Append(string.Format(formart, "ReturnsType", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "IsChange", i, ""));
                sbSql.Append(")");
                if (i < codStatsList.Count - 1)
                {
                    sbSql.Append(" \n ");
                }
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseID", i, ""), codStats.WareHouseID));
                parameterList.Add(new SqlParameter(string.Format(formart, "ExpressCompanyID", i, ""), codStats.ExpressCompanyID));
                parameterList.Add(new SqlParameter(string.Format(formart, "AreaType", i, ""), codStats.AreaType));
                parameterList.Add(new SqlParameter(string.Format(formart, "Weight", i, ""), codStats.Weight));
                parameterList.Add(new SqlParameter(string.Format(formart, "FormCount", i, ""), codStats.FormCount));
                parameterList.Add(new SqlParameter(string.Format(formart, "Fare", i, ""), codStats.Fare));
                parameterList.Add(new SqlParameter(string.Format(formart, "Formula", i, ""), codStats.Formula));
                parameterList.Add(new SqlParameter(string.Format(formart, "ReturnsType", i, ""), codStats.ReturnsType));
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseType", i, ""), codStats.WareHouseType));
                parameterList.Add(new SqlParameter(string.Format(formart, "MerchantID", i, ""), codStats.MerchantID));
                parameterList.Add(new SqlParameter(string.Format(formart, "IsChange", i, ""), 1));
                i++;
            }
            string sql = sbSql.ToString();
            SqlParameter[] parameters = parameterList.ToArray();
            //return true;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
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
                        WareHouseID = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.ReturnExpressCompanyID)
                                           ELSE fcbi.ReturnWareHouseID
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = '' THEN 2 ELSE 1
                                        END
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.Flag = 1
                        AND fcbi.WaybillType ='2'
                        AND fcbi.ReturnTime >= @ReturnTimeStr
                        AND fcbi.ReturnTime < @ReturnTimeEnd
             )
    SELECT  @ReturnTimeStr AS StatisticsDate ,
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
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime)
										};
            parameters[0].Value = accountDate.ToShortDateString();
            parameters[1].Value = accountDate.AddDays(1).ToShortDateString();

            DataTable dt = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
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
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnWareHouse",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
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
            SqlParameter[] parameters = { 
											new SqlParameter("@ReturnTimeStr",SqlDbType.DateTime),
											new SqlParameter("@ReturnTimeEnd",SqlDbType.DateTime),
											new SqlParameter("@DeliverStationID",SqlDbType.Int),
											new SqlParameter("@ReturnWareHouse",SqlDbType.NVarChar,20),
                                            new SqlParameter("@MerchantID",SqlDbType.Int)
										};
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            return (int)SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters);
        }

        private string GetVisitHouseType1Sql(bool isFare)
        {
            string sql = @"
SELECT  COUNT(1)
FROM    LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
WHERE   fcbi.IsDeleted = 0
        AND fcbi.Flag = 1
        AND fcbi.WaybillType ='2'
        AND fcbi.ReturnTime >= @ReturnTimeStr
        AND fcbi.ReturnTime < @ReturnTimeEnd
        AND fcbi.DeliverStationID = @DeliverStationID
        AND fcbi.ReturnWareHouseID = @ReturnWareHouse
        AND fcbi.MerchantID = @MerchantID
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
FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
WHERE    fcbi.IsDeleted = 0
        AND fcbi.Flag = 1
        AND fcbi.WaybillType ='2'
        AND fcbi.ReturnTime >= @ReturnTimeStr
        AND fcbi.ReturnTime < @ReturnTimeEnd
        AND fcbi.DeliverStationID = @DeliverStationID
        AND fcbi.ReturnExpressCompanyID = @ReturnWareHouse
        AND fcbi.MerchantID = @MerchantID
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
            SqlParameter[] parameters ={
										new SqlParameter("@ReturnTimeStr",SqlDbType.Date),
										new SqlParameter("@ReturnTimeEnd",SqlDbType.Date),
										new SqlParameter("@DeliverStationID",SqlDbType.Int),
										new SqlParameter("@ReturnWareHouse",SqlDbType.NVarChar,40),
                                        new SqlParameter("@MerchantID",SqlDbType.Int),
									 };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsDate.AddDays(1).ToShortDateString();
            parameters[2].Value = codStatsLog.ExpressCompanyID;
            parameters[3].Value = codStatsLog.WareHouseID;
            parameters[4].Value = codStatsLog.MerchantID;
            DataTable dt = SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
            return TransformToCodStatsModel(dt);
        }

        private string VisitDayStatByHouseType1()
        {
            return @"
WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.DeliverStationID ,
                        WareHouseID = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.ReturnExpressCompanyID)
                                           ELSE fcbi.ReturnWareHouseID
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = '' THEN 2
                                             ELSE 1
                                        END ,
                fcbi.Fare,
                fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType ='2'
                        AND fcbi.Flag = 1
                        AND fcbi.ReturnTime >= @ReturnTimeStr
                        AND fcbi.ReturnTime < @ReturnTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.ReturnWareHouseID = @ReturnWareHouse
                        AND fcbi.MerchantID = @MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(ISNULL(t.AccountWeight, 0)) AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            1 AS WareHouseType,
			t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN (
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
                        WareHouseID = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = ''
                                           THEN CONVERT(VARCHAR(20), fcbi.ReturnExpressCompanyID)
                                           ELSE fcbi.ReturnWareHouseID
                                      END ,
                        WareHouseType = CASE WHEN ISNULL(fcbi.ReturnWareHouseID,'') = '' THEN 2
                                             ELSE 1
                                        END ,
                fcbi.Fare,
                fcbi.FareFormula,
                        fcbi.AccountWeight ,
                        fcbi.WaybillType ,
                        fcbi.AreaID ,
                        fcbi.TopCodCompanyID ,
                        fcbi.MerchantID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi ( NOLOCK )
               WHERE    fcbi.IsDeleted = 0
                        AND fcbi.WaybillType ='2'
                        AND fcbi.Flag = 1
                        AND fcbi.ReturnTime >= @ReturnTimeStr
                        AND fcbi.ReturnTime < @ReturnTimeEnd
                        AND fcbi.DeliverStationID = @DeliverStationID
                        AND fcbi.ReturnExpressCompanyID = @ReturnWareHouse
                        AND fcbi.MerchantID = @MerchantID
             )
    SELECT  t.DeliverStationID AS ExpressCompanyID ,
            t.WareHouseID ,
            ael.AreaType ,
            COUNT(1) AS FormCount ,
            SUM(t.Fare) AS Fare ,
            t.FareFormula AS Formula ,
            SUM(ISNULL(t.AccountWeight, 0)) AS WEIGHT ,
            t.WaybillType AS ReturnsType ,
            2 AS WareHouseType,
			t.MerchantID
    FROM    t
            LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = t.AreaID
                                                         AND ael.[Enable] IN ( 1, 2 )
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
            StringBuilder sbSql = new StringBuilder();
            string existsSql = @"IF NOT EXISTS(SELECT 1 FROM FMS_CODVisitReturnsCount(NOLOCK) WHERE WareHouseID = '{0}' 
									AND ExpressCompanyID ={1} AND AreaType ={2} AND AccountDate='{3}' AND WareHouseType={4} AND Formula='{5}' AND MerchantID={6}) ";
            string insertSql = @" INSERT INTO FMS_CODVisitReturnsCount ( AccountNO,WareHouseID,ExpressCompanyID,
									AreaType,Weight,AccountDate,FormCount,Fare,Formula,CreateBy,CreateTime,UpdateBy,UpdateTime,WareHouseType,MerchantID,IsChange)  VALUES ";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            int i = 0;
            string formart = "@{0}{1}{2}";
            foreach (CodStatsModel codStats in codStatsList)
            {
                sbSql.Append(string.Format(existsSql, codStats.WareHouseID,
                                codStats.ExpressCompanyID, codStats.AreaType, date, codStats.WareHouseType, codStats.Formula, codStats.MerchantID));
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append("'',");
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "Weight", i, ","));
                sbSql.Append("'" + date + "',");
                sbSql.Append(string.Format(formart, "FormCount", i, ","));
                sbSql.Append(string.Format(formart, "Fare", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append("0,");
                sbSql.Append("GETDATE(),");
                sbSql.Append("'',");
                sbSql.Append("GETDATE(),");
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "IsChange", i, ""));
                sbSql.Append(")");
                if (i < codStatsList.Count - 1)
                {
                    sbSql.Append(" \n ");
                }
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseID", i, ""), codStats.WareHouseID));
                parameterList.Add(new SqlParameter(string.Format(formart, "ExpressCompanyID", i, ""), codStats.ExpressCompanyID));
                parameterList.Add(new SqlParameter(string.Format(formart, "AreaType", i, ""), codStats.AreaType));
                parameterList.Add(new SqlParameter(string.Format(formart, "Weight", i, ""), codStats.Weight));
                parameterList.Add(new SqlParameter(string.Format(formart, "FormCount", i, ""), codStats.FormCount));
                parameterList.Add(new SqlParameter(string.Format(formart, "Fare", i, ""), codStats.Fare));
                parameterList.Add(new SqlParameter(string.Format(formart, "Formula", i, ""), codStats.Formula));
                parameterList.Add(new SqlParameter(string.Format(formart, "WareHouseType", i, ""), codStats.WareHouseType));
                parameterList.Add(new SqlParameter(string.Format(formart, "MerchantID", i, ""), codStats.MerchantID));
                parameterList.Add(new SqlParameter(string.Format(formart, "IsChange", i, ""), 1));
                i++;
            }
            string sql = sbSql.ToString();
            SqlParameter[] parameters = parameterList.ToArray();
            //return true;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
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
            string sql = @"SELECT COUNT(1) FROM FMS_CodStatsLog (NOLOCK) 
							WHERE StatisticsType=@StatisticsType 
									AND StatisticsDate=@StatisticsDate 
									AND WareHouseID=@WareHouseID 
									AND ExpressCompanyID=@ExpressCompanyID
									AND WareHouseType=@WareHouseType
                                    AND MerchantID=@MerchantID";
            SqlParameter[] parameters ={
										   new SqlParameter("@StatisticsDate",SqlDbType.Date),
										   new SqlParameter("@StatisticsType",SqlDbType.SmallInt),
										   new SqlParameter("@WareHouseID",SqlDbType.NVarChar,20),
										   new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
										   new SqlParameter("@WareHouseType",SqlDbType.Int),
                                           new SqlParameter("@MerchantID",SqlDbType.Int)
									  };
            parameters[0].Value = codStatsLog.StatisticsDate.ToShortDateString();
            parameters[1].Value = codStatsLog.StatisticsType;
            parameters[2].Value = codStatsLog.WareHouseID;
            parameters[3].Value = codStatsLog.ExpressCompanyID;
            parameters[4].Value = codStatsLog.WareHouseType;
            parameters[5].Value = codStatsLog.MerchantID;
            return DataConvert.ToInt(SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 更新日志
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public bool UpdateStatisticsLog(CodStatsLogModel codStatsLog)
        {

            string sql = @"UPDATE FMS_CodStatsLog
					SET    IsSuccess = @IsSuccess,
						   Ip = @Ip,
						   Reasons = @Reasons,
						   UpdateTime = GETDATE(),
                           IsChange=@IsChange
					WHERE  StatisticsType = @StatisticsType
						   AND StatisticsDate = @StatisticsDate
						   AND WareHouseID = @WareHouseID
						   AND ExpressCompanyID = @ExpressCompanyID
						   AND WareHouseType=@WareHouseType
                           AND MerchantID=@MerchantID
						   --AND IsSuccess=0";
            SqlParameter[] parameters = {
											new SqlParameter("@StatisticsDate",SqlDbType.Date),
											new SqlParameter("@IsSuccess",SqlDbType.SmallInt),
											new SqlParameter("@Reasons",SqlDbType.NVarChar,500),
											new SqlParameter("@Ip",SqlDbType.NVarChar,50),
											new SqlParameter("@StatisticsType",SqlDbType.SmallInt),
											new SqlParameter("@WareHouseID",SqlDbType.NVarChar,20),
											new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
											new SqlParameter("@WareHouseType",SqlDbType.Int),
                                            new SqlParameter("@MerchantID",SqlDbType.Int),
                                            new SqlParameter("@IsChange",SqlDbType.Bit)
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
            parameters[9].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="codStatsLog"></param>
        /// <returns></returns>
        public bool WriteStatisticsLog(CodStatsLogModel codStatsLog)
        {
            string Sql = @"INSERT INTO FMS_CodStatsLog(StatisticsType,StatisticsDate,IsSuccess,Reasons,Ip,WareHouseID,ExpressCompanyID,WareHouseType,MerchantID,IsChange)
								VALUES (@StatisticsType,@StatisticsDate,@IsSuccess,@Reasons,@Ip,@WareHouseID,@ExpressCompanyID,@WareHouseType,@MerchantID,@IsChange)";
            SqlParameter[] parameters = {
											new SqlParameter("@StatisticsType",SqlDbType.SmallInt),
											new SqlParameter("@StatisticsDate",SqlDbType.Date),
											new SqlParameter("@IsSuccess",SqlDbType.SmallInt),
											new SqlParameter("@Reasons",SqlDbType.NVarChar,500),
											new SqlParameter("@Ip",SqlDbType.NVarChar,50),
											new SqlParameter("@WareHouseID",SqlDbType.NVarChar,20),
											new SqlParameter("@ExpressCompanyID",SqlDbType.Int),
											new SqlParameter("@WareHouseType",SqlDbType.Int),
                                            new SqlParameter("@MerchantID",SqlDbType.Int),
                                            new SqlParameter("@IsChange",SqlDbType.Bit)
										};
            parameters[0].Value = codStatsLog.StatisticsType;
            parameters[1].Value = codStatsLog.StatisticsDate;
            parameters[2].Value = codStatsLog.IsSuccess;
            parameters[3].Value = codStatsLog.Reasons;
            parameters[4].Value = codStatsLog.Ip;
            parameters[5].Value = codStatsLog.WareHouseID;
            parameters[6].Value = codStatsLog.ExpressCompanyID;
            parameters[7].Value = codStatsLog.WareHouseType;
            parameters[8].Value = codStatsLog.MerchantID;
            parameters[9].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, Sql, parameters) > 0;
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
							FROM   FMS_CodStatsLog(NOLOCK)
							WHERE  StatisticsType = @StatisticsType
								   AND IsSuccess = 0
								   AND StatisticsDate <> @StatisticsDate
								   AND CreateTime > GETDATE() -@BeforeCreateTimeDays";

            //            string Sql = @"SELECT DISTINCT StatisticsDate,
            //								   ExpressCompanyID,
            //								   WareHouseID,
            //								   0 AS FormCount,
            //								   StatisticsType,
            //								   WareHouseType
            //							FROM   FMS_CodStatsLog(NOLOCK) WHERE  StatisticsType = @StatisticsType and CodStatsID in (990,1008)";
            SqlParameter[] parameters = { 
											new SqlParameter("@StatisticsType",SqlDbType.SmallInt),
											new SqlParameter("@StatisticsDate",SqlDbType.Date),
                                            new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
										};
            parameters[0].Value = statisticsType;
            parameters[1].Value = dateRemove;
            parameters[2].Value = accountDays;
            DataTable dt = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sql, parameters).Tables[0];

            return TransformToCodStatsLogModel(dt);
        }
        #endregion

        #region 检查错误
        public DataTable GetDeliver(int accountDays)
        {
            string strSql = @"
DECLARE @accountDate DATETIME
SET @accountDate=GETDATE()-@BeforeCreateTimeDays;
WITH t AS (
SELECT fcbi.ID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=@accountDate
UNION
SELECT fcbi.ID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=@accountDate
UNION
SELECT fcbi.ID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=@accountDate
UNION
SELECT fcbi.ID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare>1 AND fcbi.IsFare<9
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=@accountDate
)
SELECT ID FROM t";

            SqlParameter[] parameters ={
                                           new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public bool ChangeDeliverBack(List<string> noList)
        {
            string updateSql = " UPDATE LMS_RFD.dbo.FMS_CODBaseInfo SET  IsFare=0,IsChange=1 WHERE  ID = @ID{0} ";
            StringBuilder sbSql = new StringBuilder();
            List<SqlParameter> parList = new List<SqlParameter>();
            int n = 0;
            foreach (string s in noList)
            {
                sbSql.AppendFormat(updateSql, n);
                parList.Add(new SqlParameter("@ID" + n, s));
                sbSql.Append(" \n ");
                n++;
            }

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sbSql.ToString(), parList.ToArray()) > 0;
        }

        public DataTable GetError8(int accountDays)
        {
            string strSql = @"
DECLARE @accountDate DATETIME
	set @accountDate=GETDATE()-@BeforeCreateTimeDays;
WITH t AS (
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.AreaID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=7
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=@accountDate
)
SELECT WaybillNO ,AreaID FROM t";
            SqlParameter[] parameters ={
                                           new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError9(int accountDays)
        {
            string strSql = @"
DECLARE @accountDate DATETIME
	set @accountDate=GETDATE()-@BeforeCreateTimeDays;
WITH t AS (
SELECT fcbi.WaybillNO  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=@accountDate
UNION
SELECT fcbi.WaybillNO  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=@accountDate
UNION
SELECT fcbi.WaybillNO  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=@accountDate
UNION
SELECT fcbi.WaybillNO FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=8
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=@accountDate
)
SELECT WaybillNO FROM t";
            SqlParameter[] parameters ={
                                           new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
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
DECLARE @accountDate DATETIME
	set @accountDate=GETDATE()-@BeforeCreateTimeDays;
WITH t AS (
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID ,fcbi.ExpressCompanyID ,fcbi.WarehouseId FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=6
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=@accountDate
)
SELECT WaybillNO ,DeliverStationID ,ExpressCompanyID ,WarehouseId FROM t";
            SqlParameter[] parameters ={
                                           new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError6(int accountDays)
        {
            string strSql = @"DECLARE @accountDate DATETIME
	set @accountDate=GETDATE()-@BeforeCreateTimeDays;
WITH t AS (
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=@accountDate
UNION
SELECT fcbi.WaybillNO ,fcbi.DeliverStationID FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=5
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=@accountDate
)
    SELECT  DISTINCT
            DeliverStationID ,
            ec.CompanyName
    FROM    t
            JOIN RFD_PMS.dbo.ExpressCompany AS ec ( NOLOCK ) ON t.DeliverStationID = ec.ExpressCompanyID";
            SqlParameter[] parameters ={
                                           new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError5(int accountDays)
        {
            string strSql = @"DECLARE @accountDate DATETIME
	set @accountDate=GETDATE()-@BeforeCreateTimeDays;
WITH t AS (
SELECT fcbi.WaybillNO  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType IN ('0','1') AND fcbi.DeliverTime>=@accountDate
UNION
SELECT fcbi.WaybillNO   FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.ReturnTime>=@accountDate
UNION
SELECT fcbi.WaybillNO   FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=0 AND fcbi.WaybillType IN ('0','1') AND fcbi.OperateType in (2,5)  AND fcbi.CreateTime>=@accountDate
UNION
SELECT fcbi.WaybillNO  FROM LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi (nolock) WHERE fcbi.IsFare=4
AND fcbi.IsDeleted=0 AND fcbi.Flag=1 AND fcbi.WaybillType ='2' AND fcbi.ReturnTime>=@accountDate
)
SELECT WaybillNO FROM t";
            SqlParameter[] parameters ={
                                           new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
                                      };
            parameters[0].Value = accountDays;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetError34(int errorType, int accountDays)
        {
            string strSql = @"
            DECLARE @accountDate DATETIME
	        set @accountDate=GETDATE()-@BeforeCreateTimeDays;
            WITH    t AS ( SELECT   fcbi.WaybillNO ,
                        fcbi.MerchantID ,
                        fcbi.DeliverStationID ,
                        fcbi.ExpressCompanyID ,
                        fcbi.WarehouseId,
                        fcbi.FinalExpressCompanyID,
                        fcbi.AreaID
               FROM     LMS_RFD.dbo.FMS_CODBaseInfo AS fcbi(NOLOCK) WHERE fcbi.IsFare = @ErrorType 
						AND fcbi.DeliverTime>GETDATE()-40
             )
    SELECT  DISTINCT
            t.MerchantID ,
            mbi.MerchantName,
            t.DeliverStationID ,
            ec.CompanyName,
            Warehouseid = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN t.MerchantID IN ( 8, 9 )
								THEN t.Warehouseid
								ELSE CONVERT(NVARCHAR(20), t.ExpressCompanyID)END 
                          ELSE
                          CONVERT(NVARCHAR(20), t.FinalExpressCompanyID) END ,
			WarehouseName = CASE WHEN ISNULL(t.FinalExpressCompanyID,0)=0 THEN 
								CASE WHEN t.MerchantID IN ( 8, 9 )
								THEN w.WarehouseName
								ELSE w1.CompanyName END 
                          ELSE
                          w2.CompanyName END ,
            a.AreaName,
            t.AreaID
    FROM    t
            JOIN RFD_PMS.dbo.area a ( NOLOCK ) ON a.AreaID = t.AreaID
			JOIN RFD_PMS.dbo.ExpressCompany AS ec(NOLOCK) ON ec.ExpressCompanyID=t.DeliverStationID
			LEFT JOIN RFD_PMS.dbo.ExpressCompany AS w1(NOLOCK) ON w1.ExpressCompanyID=t.ExpressCompanyID AND w1.CompanyFlag=1
			LEFT JOIN RFD_PMS.dbo.ExpressCompany AS w2(NOLOCK) ON w2.ExpressCompanyID=t.FinalExpressCompanyID AND w2.CompanyFlag=1
			LEFT JOIN RFD_PMS.dbo.Warehouse AS w(NOLOCK) ON w.WarehouseId=t.WarehouseId
			JOIN RFD_PMS.dbo.MerchantBaseInfo AS mbi (NOLOCK) ON mbi.ID=t.MerchantID";
            SqlParameter[] parameters ={
										   new SqlParameter("@ErrorType",SqlDbType.Int),
                                           new SqlParameter("@BeforeCreateTimeDays",SqlDbType.Int)
									  };
            parameters[0].Value = errorType;
            parameters[1].Value = accountDays;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters);
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
                         fcbi.DeliverStationID,
                         fcbi.WaybillType
                FROM     LMS_RFD.dbo.FMS_CODBaseInfo fcbi(NOLOCK)					
                LEFT JOIN AreaExpressLevel ael ON ael.AreaID = fcbi.AreaID
                                 AND ael.Enable IN (1, 2)
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND IsNULL(ael.WareHouseID,'')= ''
								
               WHERE ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59'
               AND fcbi.DistributionCode=@DistributionCode 
               AND fcbi.IsDeleted=0  
               AND fcbi.TopCODCompanyID = @DistributionCompany 
               AND fcbi.Flag=1  
               AND fcbi.WaybillType IN ('0','1')  
               AND fcbi.DeliverTime>=@StartTime
               AND fcbi.DeliverTime<@EndTime
           ";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
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
                       fcbi.DeliverStationID,
                       fcbi.WaybillType
               FROM    LMS_RFD.dbo.FMS_CODBaseInfo fcbi(NOLOCK)
						   JOIN RFD_PMS.dbo.ExpressCompany ec ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
               LEFT JOIN AreaExpressLevel ael ON ael.AreaID = fcbi.AreaID
                         AND ael.Enable IN (1, 2 )
                         AND ael.expresscompanyid = fcbi.TopCodCompanyID
                         AND ael.MerchantID = fcbi.MerchantID
                         AND IsNULL(ael.WareHouseID,'')= ''
	           WHERE ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' 
                         AND fcbi.DistributionCode<>@DistributionCode 
                         AND ec.DistributionCode=@DistributionCode  
                         AND fcbi.IsDeleted=0  
                         AND fcbi.TopCODCompanyID = @DistributionCompany   
                         AND fcbi.Flag=1  
                         AND fcbi.WaybillType IN ('0','1')  
                         AND fcbi.DeliverTime>=@StartTime
                         AND fcbi.DeliverTime< @EndTime
";
           if (!string.IsNullOrEmpty(model.MerchantIDs))
           {
               Sqlstr += string.Format(@" AND fcbi.MerchantID in ({0})", model.MerchantIDs);
           }
            Sqlstr +=@"
             )
    SELECT  
			mbi.MerchantName AS 商家 ,
            CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
            t.AreaType 区域类型 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
            t.Fare AS 配送费,
            t.Address AS 收货人地址 ,
            t.DeliverTime AS 最终发日期 ,
            CASE WHEN t.MerchantID IN ( 8, 9 ) THEN wh.WarehouseName
            	ELSE CASE WHEN ec4.CompanyName IS NULL  THEN
            	 ec3.CompanyName
				 ELSE ec4.CompanyName END
            END AS 末级发货仓名称,
            t.WaybillType AS 订单类型,
            '1' AS 状态 
            
    FROM    t
            JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN RFD_PMS.dbo.Warehouse wh(NOLOCK) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec3(NOLOCK) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec4 ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1";
            SqlParameter[] parameters = {
                                               new SqlParameter("@DistributionCompany", SqlDbType.Int),
                                               new SqlParameter("@DistributionCode",SqlDbType.NVarChar), 
                                               new SqlParameter("@StartTime", SqlDbType.DateTime),
                                               new SqlParameter("@EndTime", SqlDbType.DateTime)
                                           };
            parameters[0].Value = model.DistributionCompany;
            parameters[1].Value = model.DistributionCode;
            parameters[2].Value = model.STime;
            parameters[3].Value = model.ETime;

           var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sqlstr, parameters);
           return ds.Tables[0];

        }


        public DataTable GetReturnList(FMS_CODBaseInfoCheck model)
        {
            string Sqlstr =
                @"
                 WITH    t AS ( 
           	
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
                    FROM  LMS_RFD.dbo.FMS_CODBaseInfo  fcbi ( NOLOCK )
						
                    LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = fcbi.AreaID
                                 AND ael.[Enable] IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ISNULL(ael.WareHouseID,'') = ''
								WHERE ( 1 = 1 ) AND fcbi.OperateType not in (2,5) 
								AND fcbi.DistributionCode=@DistributionCode  
								AND fcbi.IsDeleted=0  
								AND fcbi.TopCODCompanyID =@DistributionCompany 
								AND fcbi.Flag=0  
								AND fcbi.WaybillType IN ('0','1')  
								AND fcbi.ReturnTime>=@StartTime 
								AND fcbi.ReturnTime< @EndTime 
";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"  AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }
            Sqlstr +=
                @"
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
                    FROM     LMS_RFD.dbo.FMS_CODBaseInfo  fcbi ( NOLOCK )
			            
                    LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = fcbi.AreaID
                                 AND ael.[Enable] IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ISNULL(ael.WareHouseID,'') = ''
				     WHERE    ( 1 = 1 ) AND fcbi.OperateType in (2,5) 
				     AND fcbi.DistributionCode=@DistributionCode  
				     AND fcbi.IsDeleted=0  
				     AND fcbi.TopCODCompanyID = @DistributionCompany
				     AND fcbi.Flag=0  
				     AND fcbi.WaybillType IN ('0','1')  
				     AND fcbi.CreateTime>=@StartTime
                     AND fcbi.CreateTime< @EndTime
";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"  AND fcbi.MerchantID in ({0})", model.MerchantIDs);
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
                    FROM  LMS_RFD.dbo.FMS_CODBaseInfo  fcbi ( NOLOCK )
                    JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
						
                    LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = fcbi.AreaID
                                 AND ael.[Enable] IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ISNULL(ael.WareHouseID,'') = ''
					WHERE ( 1 = 1 ) AND fcbi.OperateType not in (2,5) 
				    AND fcbi.DistributionCode<>@DistributionCode 
				    AND ec.DistributionCode=@DistributionCode  
				    AND fcbi.IsDeleted=0  
				    AND fcbi.TopCODCompanyID =@DistributionCompany
				    AND fcbi.Flag=0  
				    AND fcbi.WaybillType IN ('0','1')  
				    AND fcbi.ReturnTime>=@StartTime
				    AND fcbi.ReturnTime<@EndTime 
";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@" AND fcbi.MerchantID in ({0})", model.MerchantIDs);
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
                    FROM  LMS_RFD.dbo.FMS_CODBaseInfo  fcbi ( NOLOCK )
                    JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
			        LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = fcbi.AreaID
                                 AND ael.[Enable] IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ISNULL(ael.WareHouseID,'') = ''
				    WHERE ( 1 = 1 ) AND fcbi.OperateType in (2,5) 
				    AND fcbi.DistributionCode<> @DistributionCode 
				    AND ec.DistributionCode= @DistributionCode
				    AND fcbi.IsDeleted=0  
				    AND fcbi.TopCODCompanyID = @DistributionCompany
				    AND fcbi.Flag=0  AND fcbi.WaybillType IN ('0','1')  
				    AND fcbi.CreateTime>=@StartTime
				    AND fcbi.CreateTime< @EndTime 
";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr +=@"
             )
     select mbi.MerchantName AS 商家 ,
            CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
             t.AreaType 区域类型 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
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
            JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN RFD_PMS.dbo.Warehouse wh(NOLOCK) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN RFD_PMS.dbo.ExpressCompany(NOLOCK) ec3 ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT  JOIN RFD_PMS.dbo.ExpressCompany ec4(NOLOCK) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
	        
	        ";
            SqlParameter[] parameters = {
                                               new SqlParameter("@DistributionCode", SqlDbType.NVarChar),
                                               new SqlParameter("@DistributionCompany", SqlDbType.Int),
                                               new SqlParameter("@StartTime",SqlDbType.DateTime),
                                               new SqlParameter("@EndTime",SqlDbType.DateTime) 
                                           };
            parameters[0].Value = model.DistributionCode;
            parameters[1].Value = model.DistributionCompany;
            parameters[2].Value = model.STime;
            parameters[3].Value = model.ETime;

            var ds =SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sqlstr, parameters);
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
                    FROM    LMS_RFD.dbo.FMS_CODBaseInfo  fcbi ( NOLOCK )
						    LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = fcbi.AreaID
                                 AND ael.[Enable] IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ISNULL(ael.WareHouseID,'') = ''
								
                    WHERE    ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' 
                              AND fcbi.DistributionCode=@DistributionCode  
                              AND fcbi.IsDeleted=0  
                              AND fcbi.TopCODCompanyID = @DistributionCompany  
                              AND fcbi.Flag=1  
                              AND fcbi.WaybillType IN ('1','2')  
                              AND fcbi.ReturnTime>= @StartTime
                              AND fcbi.ReturnTime< @EndTime 
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
                    FROM  LMS_RFD.dbo.FMS_CODBaseInfo  fcbi ( NOLOCK )
				          JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=fcbi.DeliverStationID AND fcbi.IsDeleted=0 
                          LEFT JOIN AreaExpressLevel ael ( NOLOCK ) ON ael.AreaID = fcbi.AreaID
                                 AND ael.[Enable] IN (1, 2 )
                                 AND ael.expresscompanyid = fcbi.TopCodCompanyID
                                 AND ael.MerchantID = fcbi.MerchantID
                                 AND ISNULL(ael.WareHouseID,'') = ''
					 WHERE ( 1 = 1 ) AND fcbi.RfdAcceptTime>'2012-05-22 23:59:59' 
					                 AND fcbi.DistributionCode<> @DistributionCode 
					                 AND ec.DistributionCode= @DistributionCode  
					                 AND fcbi.IsDeleted=0  
					                 AND fcbi.TopCODCompanyID = @DistributionCompany
					                 AND fcbi.Flag=1  
					                 AND fcbi.WaybillType IN ('1','2')  
					                 AND fcbi.ReturnTime>= @StartTime 
					                 AND fcbi.ReturnTime< @EndTime 
    ";
            if (!string.IsNullOrEmpty(model.MerchantIDs))
            {
                Sqlstr += string.Format(@"AND fcbi.MerchantID in ({0})", model.MerchantIDs);
            }

            Sqlstr +=@"
             )
            select 
             mbi.MerchantName AS 商家 ,
            CASE WHEN ISNULL(ec.AccountCompanyName,'')='' THEN ec.CompanyName ELSE ec.AccountCompanyName END 结算单位,
             t.AreaType 区域类型 ,
            t.WaybillNO AS 订单号 ,
            ISNULL(t.AccountWeight, 0) AS 结算重量 ,
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
            JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON t.MerchantID = mbi.ID
            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID = t.TopCODCompanyID
            JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK) ON ec2.ExpressCompanyID = t.DeliverStationID
            LEFT JOIN RFD_PMS.dbo.Warehouse wh(NOLOCK) ON wh.Warehouseid = t.Warehouseid
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec3(NOLOCK) ON ec3.expresscompanyid = t.ExpressCompanyID
                                                              AND ec3.CompanyFlag = 1
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec4(NOLOCK) ON ec4.expresscompanyid = t.FinalExpressCompanyID
                                                              AND ec4.CompanyFlag = 1
            ";
         SqlParameter []parameters = {
                                             new SqlParameter("@DistributionCode",SqlDbType.NVarChar),
                                             new SqlParameter("@DistributionCompany",SqlDbType.Int),
                                             new SqlParameter("@StartTime",SqlDbType.DateTime),
                                             new SqlParameter("@EndTime",SqlDbType.DateTime) 
                                         };
       
            parameters[0].Value = model.DistributionCode;
            parameters[1].Value = model.DistributionCompany;
            parameters[2].Value = model.STime;
            parameters[3].Value = model.ETime;

            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, Sqlstr, parameters);
            return ds.Tables[0];
        }

        public DataTable GetDeliverFeeParameter(long waybillNo, string distributionCode)
        {
            string sql = @"select baseInfo.FareFormula Formula,
	                baseInfo.AccountWeight Weight,
	                baseInfo.AreaType Area,
	                baseInfo.Fare DeliverFee,
                    baseInfo.IsFare
                from LMS_RFD.dbo.FMS_CODBaseInfo baseInfo(nolock) 
                where IsDeleted=0 
                    and baseInfo.WaybillNO=@WaybillNO 
                    and baseInfo.DistributionCode=@DistributionCode
                order by baseInfo.CreateTime desc";

            SqlParameter[] parameters =
            {
                new SqlParameter("@WaybillNO",SqlDbType.BigInt){ Value=waybillNo },
                new SqlParameter("@DistributionCode",SqlDbType.VarChar){ Value=distributionCode }
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public bool SaveDeliverFee(MODEL.FinancialManage.DeliverFeeModel model)
        {
            string sql = @"update LMS_RFD.dbo.FMS_CODBaseInfo 
                set FareFormula=@FareFormula,
                    AccountWeight=@AccountWeight,
                    AreaType=@AreaType,
                    Fare=@Fare,
                    IsChange=1,
                    IsFare=1
                where WaybillNO=@WaybillNO";

            SqlParameter[] parameters =
            {
                new SqlParameter("@FareFormula",SqlDbType.VarChar){ Value=model.Formula },
                new SqlParameter("@AccountWeight",SqlDbType.Decimal){ Value=model.Weight },
                new SqlParameter("@AreaType",SqlDbType.Int){ Value=model.Area },
                new SqlParameter("@Fare",SqlDbType.Decimal){ Value=model.DeliverFee },
                new SqlParameter("@WaybillNO",SqlDbType.BigInt){ Value=model.WaybillNO }
            };

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }


        public bool UpdateEvalStatus(long waybillNo)
        {
            string sql = @"update LMS_RFD.dbo.FMS_CODBaseInfo set IsFare=0 where WaybillNO=@WaybillNO";

            SqlParameter[] parameters =
            {
                new SqlParameter("@WaybillNO",SqlDbType.BigInt){ Value = waybillNo }
            };

            if (SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) != 1) return false;

            return true;
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            string sql = String.Format(@"update LMS_RFD.dbo.FMS_CODBaseInfo set IsFare=0 where WaybillNO in ({0})", waybillNos);

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }


        public DataTable GetCODDeliveryFeeInfo(string InfoIDs)
        {
            throw new NotImplementedException();
        }


        public bool SaveDeliverFeeByID(MODEL.FinancialManage.DeliverFeeModel model)
        {
            throw new NotImplementedException();
        }


        public bool ExsitCodBaseInfoByNo(long waybillNo,long infoID)
        {
            throw new NotImplementedException();
        }


        public bool UpdateEvalStatusByInfoID(string infoIDs)
        {
            throw new NotImplementedException();
        }
    }
}
