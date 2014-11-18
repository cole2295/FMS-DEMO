using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.AdoNet;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data.Extension;

namespace RFD.FMS.DAL.FinancialManage
{
    public partial class IncomeFeeInfoDao : SqlServerDao, IIncomeFeeInfoDao
	{

        private const string TableName = @"LMS_RFD.dbo.FMS_IncomeFeeInfo";
		
		private const string PagingTemplate = @"SELECT  RowIndex ,
														T.*
												FROM    ( SELECT    T2.* ,
																	ROW_NUMBER() OVER ( ORDER BY {0} DESC ) AS RowIndex
															FROM   ( {1} )  T2
														) AS T
												WHERE   T.RowIndex > {2}
												AND T.RowIndex <= {3}";
		
		public IncomeFeeInfoDao()
		{
		}
		
		public bool Exists(Int64 waybillno)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append(string.Format("select count(1) from {0}",TableName));
			strSql.Append(string.Format(" where {0} = @{0}","WaybillNO"));
			var sqlParams = new List<SqlParameter>()
											{
												new SqlParameter(string.Format("@{0}","WaybillNO"),waybillno)
											};
			return Convert.ToInt64(SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray())) > 0;
		}


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(FMS_IncomeFeeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" WaybillNO , ");
            strSql.Append(" IsAccount , ");
            strSql.Append(" AccountStandard , ");
            strSql.Append(" AccountFare , ");
            strSql.Append(" IsProtected , ");
            strSql.Append(" ProtectedStandard , ");
            strSql.Append(" ProtectedFee , ");
            strSql.Append(" IsReceive , ");
            strSql.Append(" ReceiveStandard , ");
            strSql.Append(" ReceiveFee , ");
            strSql.Append(" CreateBy , ");
            strSql.Append(" CreateTime , ");
            strSql.Append(" UpdateBy , ");
            strSql.Append(" UpdateTime , ");
            strSql.Append(" IsDeleted , ");
            strSql.Append(" TransferPayType , ");
            strSql.Append(" DeputizeAmount , ");
            strSql.Append(" POSReceiveStandard , ");
            strSql.Append(" POSReceiveFee , ");
            strSql.Append(" CashReceiveServiceStandard , ");
            strSql.Append(" CashReceiveServiceFee , ");
            strSql.Append(" POSReceiveServiceStandard , ");
            strSql.Append(" POSReceiveServiceFee , ");
            strSql.Append(" ExpressReceiveBasicDeduct , ");
            strSql.Append(" ExpressSendBasicDeduct , ");
            strSql.Append(" ExpressAreaDeduct , ");
            strSql.Append(" ExpressWeightDeduct , ");
            strSql.Append(" ProgramReceiveBasicDeduct , ");
            strSql.Append(" ProgramSendBasicDeduct , ");
            strSql.Append(" ProgramAreaDeduct , ");
            strSql.Append(" ProgramWeightDeduct , ");
            strSql.Append(" IsDeductAcount,  ");
            strSql.Append(" IsChange,  ");
            strSql.Append(" AreaType  ");
            strSql.Append(") values (");
            strSql.Append(" @WaybillNO , ");
            strSql.Append(" @IsAccount , ");
            strSql.Append(" @AccountStandard , ");
            strSql.Append(" @AccountFare , ");
            strSql.Append(" @IsProtected , ");
            strSql.Append(" @ProtectedStandard , ");
            strSql.Append(" @ProtectedFee , ");
            strSql.Append(" @IsReceive , ");
            strSql.Append(" @ReceiveStandard , ");
            strSql.Append(" @ReceiveFee , ");
            strSql.Append(" @CreateBy , ");
            strSql.Append(" @CreateTime , ");
            strSql.Append(" @UpdateBy , ");
            strSql.Append(" @UpdateTime , ");
            strSql.Append(" @IsDeleted , ");
            strSql.Append(" @TransferPayType , ");
            strSql.Append(" @DeputizeAmount , ");
            strSql.Append(" @POSReceiveStandard , ");
            strSql.Append(" @POSReceiveFee , ");
            strSql.Append(" @CashReceiveServiceStandard , ");
            strSql.Append(" @CashReceiveServiceFee , ");
            strSql.Append(" @POSReceiveServiceStandard , ");
            strSql.Append(" @POSReceiveServiceFee , ");
            strSql.Append(" @ExpressReceiveBasicDeduct , ");
            strSql.Append(" @ExpressSendBasicDeduct , ");
            strSql.Append(" @ExpressAreaDeduct , ");
            strSql.Append(" @ExpressWeightDeduct , ");
            strSql.Append(" @ProgramReceiveBasicDeduct , ");
            strSql.Append(" @ProgramSendBasicDeduct , ");
            strSql.Append(" @ProgramAreaDeduct , ");
            strSql.Append(" @ProgramWeightDeduct , ");
            strSql.Append(" @IsDeductAcount,  ");
            strSql.Append(" @IsChange,  ");
            strSql.Append(" @AreaType  ");
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY");

            SqlParameter[] parameters = {
											new SqlParameter(string.Format("@{0}","WaybillNO"), model.WaybillNO),
											new SqlParameter(string.Format("@{0}","IsAccount"), model.IsAccount),
											new SqlParameter(string.Format("@{0}","AccountStandard"), model.AccountStandard),
											new SqlParameter(string.Format("@{0}","AccountFare"), model.AccountFare),
											new SqlParameter(string.Format("@{0}","IsProtected"), model.IsProtected),
											new SqlParameter(string.Format("@{0}","ProtectedStandard"), model.ProtectedStandard),
											new SqlParameter(string.Format("@{0}","ProtectedFee"), model.ProtectedFee),
											new SqlParameter(string.Format("@{0}","IsReceive"), model.IsReceive),
											new SqlParameter(string.Format("@{0}","ReceiveStandard"), model.ReceiveStandard),
											new SqlParameter(string.Format("@{0}","ReceiveFee"), model.ReceiveFee),
											new SqlParameter(string.Format("@{0}","CreateBy"), model.CreateBy),
											new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
											new SqlParameter(string.Format("@{0}","UpdateBy"), model.UpdateBy),
											new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
											new SqlParameter(string.Format("@{0}","IsDeleted"), model.IsDeleted),
											new SqlParameter(string.Format("@{0}","TransferPayType"), model.TransferPayType),
											new SqlParameter(string.Format("@{0}","DeputizeAmount"), model.DeputizeAmount),
											new SqlParameter(string.Format("@{0}","POSReceiveStandard"), model.POSReceiveStandard),
											new SqlParameter(string.Format("@{0}","POSReceiveFee"), model.POSReceiveFee),
											new SqlParameter(string.Format("@{0}","CashReceiveServiceStandard"), model.CashReceiveServiceStandard),
											new SqlParameter(string.Format("@{0}","CashReceiveServiceFee"), model.CashReceiveServiceFee),
											new SqlParameter(string.Format("@{0}","POSReceiveServiceStandard"), model.POSReceiveServiceStandard),
											new SqlParameter(string.Format("@{0}","POSReceiveServiceFee"), model.POSReceiveServiceFee),
											new SqlParameter(string.Format("@{0}","ExpressReceiveBasicDeduct"), model.ExpressReceiveBasicDeduct),
											new SqlParameter(string.Format("@{0}","ExpressSendBasicDeduct"), model.ExpressSendBasicDeduct),
											new SqlParameter(string.Format("@{0}","ExpressAreaDeduct"), model.ExpressAreaDeduct),
											new SqlParameter(string.Format("@{0}","ExpressWeightDeduct"), model.ExpressWeightDeduct),
											new SqlParameter(string.Format("@{0}","ProgramReceiveBasicDeduct"), model.ProgramReceiveBasicDeduct),
											new SqlParameter(string.Format("@{0}","ProgramSendBasicDeduct"), model.ProgramSendBasicDeduct),
											new SqlParameter(string.Format("@{0}","ProgramAreaDeduct"), model.ProgramAreaDeduct),
											new SqlParameter(string.Format("@{0}","ProgramWeightDeduct"), model.ProgramWeightDeduct),
											new SqlParameter(string.Format("@{0}","IsDeductAcount"), model.IsDeductAcount),
									        new SqlParameter(string.Format("@{0}","IsChange"), true),
                                            new SqlParameter(string.Format("@{0}","AreaType"), model.AreaType),
                                        };

            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (obj == null)
            {
                return 0;
            }
            
            return Convert.ToInt32(obj);
        }


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FMS_IncomeFeeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("update {0} set ", TableName));

            strSql.Append(" WaybillNO = @WaybillNO,");
            strSql.Append(" IsAccount = @IsAccount,");
            strSql.Append(" AccountStandard = @AccountStandard,");
            strSql.Append(" AccountFare = @AccountFare,");
            strSql.Append(" IsProtected = @IsProtected,");
            strSql.Append(" ProtectedStandard = @ProtectedStandard,");
            strSql.Append(" ProtectedFee = @ProtectedFee,");
            strSql.Append(" IsReceive = @IsReceive,");
            strSql.Append(" ReceiveStandard = @ReceiveStandard,");
            strSql.Append(" ReceiveFee = @ReceiveFee,");
            strSql.Append(" CreateBy = @CreateBy,");
            strSql.Append(" CreateTime = @CreateTime,");
            strSql.Append(" UpdateBy = @UpdateBy,");
            strSql.Append(" UpdateTime = @UpdateTime,");
            strSql.Append(" IsDeleted = @IsDeleted,");
            strSql.Append(" TransferPayType = @TransferPayType,");
            strSql.Append(" DeputizeAmount = @DeputizeAmount,");
            strSql.Append(" POSReceiveStandard = @POSReceiveStandard,");
            strSql.Append(" POSReceiveFee = @POSReceiveFee,");
            strSql.Append(" CashReceiveServiceStandard = @CashReceiveServiceStandard,");
            strSql.Append(" CashReceiveServiceFee = @CashReceiveServiceFee,");
            strSql.Append(" POSReceiveServiceStandard = @POSReceiveServiceStandard,");
            strSql.Append(" POSReceiveServiceFee = @POSReceiveServiceFee,");
            strSql.Append(" ExpressReceiveBasicDeduct = @ExpressReceiveBasicDeduct,");
            strSql.Append(" ExpressSendBasicDeduct = @ExpressSendBasicDeduct,");
            strSql.Append(" ExpressAreaDeduct = @ExpressAreaDeduct,");
            strSql.Append(" ExpressWeightDeduct = @ExpressWeightDeduct,");
            strSql.Append(" ProgramReceiveBasicDeduct = @ProgramReceiveBasicDeduct,");
            strSql.Append(" ProgramSendBasicDeduct = @ProgramSendBasicDeduct,");
            strSql.Append(" ProgramAreaDeduct = @ProgramAreaDeduct,");
            strSql.Append(" ProgramWeightDeduct = @ProgramWeightDeduct,");
            strSql.Append(" IsDeductAcount = @IsDeductAcount,");
            strSql.Append(" AreaType = @AreaType,");
            strSql.Append(" IsCod = @IsCod,");
            strSql.Append(" IsChange = @IsChange");

            strSql.Append(string.Format(" where {0} = @{0}","WaybillNO"));

            SqlParameter[] parameters = 
            {
				new SqlParameter(string.Format("@{0}","IncomeFeeID"), model.IncomeFeeID),								
                new SqlParameter(string.Format("@{0}","WaybillNO"), model.WaybillNO),
                new SqlParameter(string.Format("@{0}","IsAccount"), model.IsAccount),
                new SqlParameter(string.Format("@{0}","AccountStandard"), model.AccountStandard),
                new SqlParameter(string.Format("@{0}","AccountFare"), model.AccountFare),
                new SqlParameter(string.Format("@{0}","IsProtected"), model.IsProtected),
                new SqlParameter(string.Format("@{0}","ProtectedStandard"), model.ProtectedStandard),
                new SqlParameter(string.Format("@{0}","ProtectedFee"), model.ProtectedFee),
                new SqlParameter(string.Format("@{0}","IsReceive"), model.IsReceive),
                new SqlParameter(string.Format("@{0}","ReceiveStandard"), model.ReceiveStandard),
                new SqlParameter(string.Format("@{0}","ReceiveFee"), model.ReceiveFee),
                new SqlParameter(string.Format("@{0}","CreateBy"), model.CreateBy),
                new SqlParameter(string.Format("@{0}","CreateTime"), model.CreateTime),
                new SqlParameter(string.Format("@{0}","UpdateBy"), model.UpdateBy),
                new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
                new SqlParameter(string.Format("@{0}","IsDeleted"), model.IsDeleted),
                new SqlParameter(string.Format("@{0}","TransferPayType"), model.TransferPayType),
                new SqlParameter(string.Format("@{0}","DeputizeAmount"), model.DeputizeAmount),
                new SqlParameter(string.Format("@{0}","POSReceiveStandard"), model.POSReceiveStandard),
                new SqlParameter(string.Format("@{0}","POSReceiveFee"), model.POSReceiveFee),
                new SqlParameter(string.Format("@{0}","CashReceiveServiceStandard"),model.CashReceiveServiceStandard),								
                new SqlParameter(string.Format("@{0}","CashReceiveServiceFee"), model.CashReceiveServiceFee),
                new SqlParameter(string.Format("@{0}","POSReceiveServiceStandard"), model.POSReceiveServiceStandard),
                new SqlParameter(string.Format("@{0}","POSReceiveServiceFee"), model.POSReceiveServiceFee),
                new SqlParameter(string.Format("@{0}","ExpressReceiveBasicDeduct"), model.ExpressReceiveBasicDeduct),
                new SqlParameter(string.Format("@{0}","ExpressSendBasicDeduct"), model.ExpressSendBasicDeduct),	
                new SqlParameter(string.Format("@{0}","ExpressAreaDeduct"), model.ExpressAreaDeduct),
                new SqlParameter(string.Format("@{0}","ExpressWeightDeduct"), model.ExpressWeightDeduct),
                new SqlParameter(string.Format("@{0}","ProgramReceiveBasicDeduct"), model.ProgramReceiveBasicDeduct),
                new SqlParameter(string.Format("@{0}","ProgramSendBasicDeduct"), model.ProgramSendBasicDeduct),	
                new SqlParameter(string.Format("@{0}","ProgramAreaDeduct"), model.ProgramAreaDeduct),
                new SqlParameter(string.Format("@{0}","ProgramWeightDeduct"), model.ProgramWeightDeduct),
                new SqlParameter(string.Format("@{0}","IsDeductAcount"), model.IsDeductAcount),	
                new SqlParameter(string.Format("@{0}","AreaType"), model.AreaType),	
                new SqlParameter(string.Format("@{0}","IsCod"), model.IsCod),	
                new SqlParameter(string.Format("@{0}","IsChange"), true)
            };

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rows > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 归班更新一条数据
        /// </summary>
        public bool UpdateByBackStation(FMS_IncomeFeeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("update {0} set ", TableName));

            strSql.Append(" IsAccount = @IsAccount,");
            strSql.Append(" AccountFare = @AccountFare,");
            
            strSql.Append(" UpdateBy = @UpdateBy,");
            strSql.Append(" UpdateTime = @UpdateTime,");
            strSql.Append(" TransferPayType = @TransferPayType,");
            strSql.Append(" IsChange = @IsChange");

            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNO"));

            SqlParameter[] parameters = 
            {								
                new SqlParameter(string.Format("@{0}","WaybillNO"), model.WaybillNO),
                new SqlParameter(string.Format("@{0}","IsAccount"), model.IsAccount),
                new SqlParameter(string.Format("@{0}","AccountFare"), model.AccountFare),
                
                new SqlParameter(string.Format("@{0}","UpdateBy"), model.UpdateBy),
                new SqlParameter(string.Format("@{0}","UpdateTime"), model.UpdateTime),
                new SqlParameter(string.Format("@{0}","TransferPayType"), model.TransferPayType),
                new SqlParameter(string.Format("@{0}","IsChange"), true)
            };

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rows > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_IncomeFeeInfo GetModel(DataRow row)
        {
            var model = new FMS_IncomeFeeInfo();

            if (row["IncomeFeeID"].ToString() != "")
            {
                model.IncomeFeeID = Int64.Parse(row["IncomeFeeID"].ToString());
            }
            if (row["WaybillNO"].ToString() != "")
            {
                model.WaybillNO = Int64.Parse(row["WaybillNO"].ToString());
            }
            if (row["IsAccount"].ToString() != "")
            {
                model.IsAccount = Int32.Parse(row["IsAccount"].ToString());
            }
            model.AccountStandard = row["AccountStandard"].ToString();
            if (row["AccountFare"].ToString() != "")
            {
                model.AccountFare = System.Decimal.Parse(row["AccountFare"].ToString());
            }
            if (row["IsProtected"].ToString() != "")
            {
                model.IsProtected = Int32.Parse(row["IsProtected"].ToString());
            }
            if (row["ProtectedStandard"].ToString() != "")
            {
                model.ProtectedStandard = System.Decimal.Parse(row["ProtectedStandard"].ToString());
            }
            if (row["ProtectedFee"].ToString() != "")
            {
                model.ProtectedFee = System.Decimal.Parse(row["ProtectedFee"].ToString());
            }
            if (row["IsReceive"].ToString() != "")
            {
                model.IsReceive = Int32.Parse(row["IsReceive"].ToString());
            }
            if (row["ReceiveStandard"].ToString() != "")
            {
                model.ReceiveStandard = System.Decimal.Parse(row["ReceiveStandard"].ToString());
            }
            if (row["ReceiveFee"].ToString() != "")
            {
                model.ReceiveFee = System.Decimal.Parse(row["ReceiveFee"].ToString());
            }
            if (row["CreateBy"].ToString() != "")
            {
                model.CreateBy = Int32.Parse(row["CreateBy"].ToString());
            }
            if (row["CreateTime"].ToString() != "")
            {
                model.CreateTime = System.DateTime.Parse(row["CreateTime"].ToString());
            }
            if (row["UpdateBy"].ToString() != "")
            {
                model.UpdateBy = Int32.Parse(row["UpdateBy"].ToString());
            }
            if (row["UpdateTime"].ToString() != "")
            {
                model.UpdateTime = System.DateTime.Parse(row["UpdateTime"].ToString());
            }
            if (row["TransferPayType"].ToString() != "")
            {
                model.TransferPayType = Int32.Parse(row["TransferPayType"].ToString());
            }
            if (row["DeputizeAmount"].ToString() != "")
            {
                model.DeputizeAmount = System.Decimal.Parse(row["DeputizeAmount"].ToString());
            }
            if (row["POSReceiveStandard"].ToString() != "")
            {
                model.POSReceiveStandard = System.Decimal.Parse(row["POSReceiveStandard"].ToString());
            }
            if (row["POSReceiveFee"].ToString() != "")
            {
                model.POSReceiveFee = System.Decimal.Parse(row["POSReceiveFee"].ToString());
            }
            if (row["CashReceiveServiceStandard"].ToString() != "")
            {
                model.CashReceiveServiceStandard = System.Decimal.Parse(row["CashReceiveServiceStandard"].ToString());
            }
            if (row["CashReceiveServiceFee"].ToString() != "")
            {
                model.CashReceiveServiceFee = System.Decimal.Parse(row["CashReceiveServiceFee"].ToString());
            }
            if (row["POSReceiveServiceStandard"].ToString() != "")
            {
                model.POSReceiveServiceStandard = System.Decimal.Parse(row["POSReceiveServiceStandard"].ToString());
            }
            if (row["POSReceiveServiceFee"].ToString() != "")
            {
                model.POSReceiveServiceFee = System.Decimal.Parse(row["POSReceiveServiceFee"].ToString());
            }
            if (row["ExpressReceiveBasicDeduct"].ToString() != "")
            {
                model.ExpressReceiveBasicDeduct = System.Decimal.Parse(row["ExpressReceiveBasicDeduct"].ToString());
            }
            if (row["ExpressSendBasicDeduct"].ToString() != "")
            {
                model.ExpressSendBasicDeduct = System.Decimal.Parse(row["ExpressSendBasicDeduct"].ToString());
            }
            if (row["ExpressAreaDeduct"].ToString() != "")
            {
                model.ExpressAreaDeduct = System.Decimal.Parse(row["ExpressAreaDeduct"].ToString());
            }
            if (row["ExpressWeightDeduct"].ToString() != "")
            {
                model.ExpressWeightDeduct = System.Decimal.Parse(row["ExpressWeightDeduct"].ToString());
            }
            if (row["ProgramReceiveBasicDeduct"].ToString() != "")
            {
                model.ProgramReceiveBasicDeduct = System.Decimal.Parse(row["ProgramReceiveBasicDeduct"].ToString());
            }
            if (row["ProgramSendBasicDeduct"].ToString() != "")
            {
                model.ProgramSendBasicDeduct = System.Decimal.Parse(row["ProgramSendBasicDeduct"].ToString());
            }
            if (row["ProgramAreaDeduct"].ToString() != "")
            {
                model.ProgramAreaDeduct = System.Decimal.Parse(row["ProgramAreaDeduct"].ToString());
            }
            if (row["ProgramWeightDeduct"].ToString() != "")
            {
                model.ProgramWeightDeduct = System.Decimal.Parse(row["ProgramWeightDeduct"].ToString());
            }
            if (row["IsDeductAcount"].ToString() != "")
            {
                model.IsDeductAcount = Int32.Parse(row["IsDeductAcount"].ToString());
            }
            if (row["AreaType"].ToString() != "")
            {
                model.AreaType = DataConvert.ToInt(row["AreaType"].ToString());
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_IncomeFeeInfo GetModel(Int64 waybillno)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select IncomeFeeID, WaybillNO, IsAccount, AccountStandard, AccountFare, IsProtected, ProtectedStandard, ProtectedFee, IsReceive, ReceiveStandard, ReceiveFee, CreateBy, CreateTime, UpdateBy, UpdateTime, IsDeleted, TransferPayType, DeputizeAmount, POSReceiveStandard, POSReceiveFee, CashReceiveServiceStandard, CashReceiveServiceFee, POSReceiveServiceStandard, POSReceiveServiceFee, ExpressReceiveBasicDeduct, ExpressSendBasicDeduct, ExpressAreaDeduct, ExpressWeightDeduct, ProgramReceiveBasicDeduct, ProgramSendBasicDeduct, ProgramAreaDeduct, ProgramWeightDeduct, IsDeductAcount,AreaType  ");
            strSql.Append(string.Format("  from {0} ", TableName));
            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNO"));
            var sqlParams = new List<SqlParameter>()
											{
												new SqlParameter(string.Format("@{0}","WaybillNO"),waybillno)
											};
            var model = new FMS_IncomeFeeInfo();
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                model = GetModel(ds.Tables[0].Rows[0]);
            }
            return model;
        }

        /// <summary>
        /// 根据条件得到一个对象实体集
        /// </summary>
        public List<FMS_IncomeFeeInfo> GetModelList(Dictionary<string, object> searchParams)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select IncomeFeeID, WaybillNO, IsAccount, AccountStandard, AccountFare, IsProtected, ProtectedStandard, ProtectedFee, IsReceive, ReceiveStandard, ReceiveFee, CreateBy, CreateTime, UpdateBy, UpdateTime, IsDeleted, TransferPayType, DeputizeAmount, POSReceiveStandard, POSReceiveFee, CashReceiveServiceStandard, CashReceiveServiceFee, POSReceiveServiceStandard, POSReceiveServiceFee, ExpressReceiveBasicDeduct, ExpressSendBasicDeduct, ExpressAreaDeduct, ExpressWeightDeduct, ProgramReceiveBasicDeduct, ProgramSendBasicDeduct, ProgramAreaDeduct, ProgramWeightDeduct, IsDeductAcount  ");
            strSql.Append(string.Format("  from {0} ", TableName));
            strSql.Append(" where 1 = 1 ");
            var sqlParams = new List<SqlParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item =>
                {
                    strSql.Append(string.Format(" and {0} = @{0}", item.Key));
                    sqlParams.Add(new SqlParameter(string.Format("@{0}", item.Key), item.Value));
                });
            }
            var modelList = new List<FMS_IncomeFeeInfo>();
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    modelList.Add(GetModel(row));
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获取指定条件的结果集行数
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public int GetDataTableCount(string searchString, Dictionary<string, object> searchParams)
        {
            var sqlStr = string.Format(@"SELECT COUNT(*) FROM {0} {1}", TableName, searchString);
            var sqlParams = new List<SqlParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item => sqlParams.Add(new SqlParameter(string.Format("@{0}", item.Key), item.Value)));
            }
            var obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, sqlParams.ToArray());
            int i = 0;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out i);
            }
            return i;
        }

        /// <summary>
        /// 获取指定条件结果集
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public DataTable GetDataTable(Dictionary<string, object> searchParams)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select IncomeFeeID, WaybillNO, IsAccount, AccountStandard, AccountFare, IsProtected, ProtectedStandard, ProtectedFee, IsReceive, ReceiveStandard, ReceiveFee, CreateBy, CreateTime, UpdateBy, UpdateTime, IsDeleted, TransferPayType, DeputizeAmount, POSReceiveStandard, POSReceiveFee, CashReceiveServiceStandard, CashReceiveServiceFee, POSReceiveServiceStandard, POSReceiveServiceFee, ExpressReceiveBasicDeduct, ExpressSendBasicDeduct, ExpressAreaDeduct, ExpressWeightDeduct, ProgramReceiveBasicDeduct, ProgramSendBasicDeduct, ProgramAreaDeduct, ProgramWeightDeduct, IsDeductAcount  ");
            strSql.Append(string.Format("  from {0} ", TableName));
            strSql.Append(" where 1 = 1 ");
            var sqlParams = new List<SqlParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item =>
                {
                    strSql.Append(string.Format(" and {0} = @{0}", item.Key));
                    sqlParams.Add(new SqlParameter(string.Format("@{0}", item.Key), item.Value));
                });
            }
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray()).Tables[0];
        }

        /// <summary>
        /// 根据指定条件指定排序获取结果集
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortColumn"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string searchString, string sortColumn, Dictionary<string, object> searchParams)
        {
            var sqlStr = string.Format(@"SELECT * FROM {0} {1} ORDER BY {2} DESC", TableName, searchString, sortColumn);
            var sqlParams = new List<SqlParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item => sqlParams.Add(new SqlParameter(string.Format("@{0}", item.Key), item.Value)));
            }
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, sqlParams.ToArray()).Tables[0];
        }

        /// <summary>
        /// 根据指定条件指定排序获取结果集带分页
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortColumn"></param>
        /// <param name="searchParams"></param>
        /// <param name="rowStart"></param>
        /// <param name="rowEnd"></param>
        /// <returns></returns>
        public DataTable GetPageDataTable(string searchString, string sortColumn, Dictionary<string, object> searchParams, int rowStart, int rowEnd)
        {
            var sqlStr = string.Format(@"SELECT * FROM {0} {1}", TableName, searchString);

            sqlStr = string.Format(PagingTemplate, sortColumn, sqlStr, rowStart, rowEnd);
            var sqlParams = new List<SqlParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item => sqlParams.Add(new SqlParameter(string.Format("@{0}", item.Key), item.Value)));
            }
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, sqlParams.ToArray()).Tables[0];
        }
        /// <summary>
        /// 查询出未计算费用的
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetInComeFeeInfo(int TopNum)
        {
            string strSql =
                @"
WITH t AS (
SELECT TOP (@SqlNum)  
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
INNER JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK) ON fibi.WaybillNo=fifi.WaybillNO
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-3
        AND fibi.ReturnTime <dateadd(hour,-1,getdate())
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
        --AND fibi.WaybillNo =11210040000212
UNION ALL
SELECT TOP (@SqlNum)  
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
INNER JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK) ON fibi.WaybillNo=fifi.WaybillNO
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>getdate()-3
        AND fibi.BackStationTime <dateadd(hour,-1,getdate())
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
        --AND fibi.WaybillNo =11210040000212
UNION ALL
SELECT TOP (@SqlNum)  
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
INNER JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK) ON fibi.WaybillNo=fifi.WaybillNO        
WHERE (1=1) 
		AND fifi.IsAccount=0 
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-3
        AND fibi.ReturnTime <dateadd(hour,-1,getdate())
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON t.MerchantID=fmdf.MerchantID AND fmdf.[Status]=2 AND fmdf.DistributionCode=t.DistributionCode
";

            SqlParameter[] parameters = { new SqlParameter("@SqlNum", TopNum) };

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection,180, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 查询出未计算费用的订单
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetInComeFeeInfo(string WaybillNO)
        {
            string strSql =
                @"SELECT fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
                        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
                        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
                        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory,fmdf.ReceiveFeeRate
                        ,fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer
                        ,fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee
                        ,fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected
                        ,fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory
                        ,fmdf.WeightType,WeightValueRule
                    FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
                    INNER JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK) ON fibi.WaybillNo=fifi.WaybillNO AND fifi.IsDeleted=0
                    LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON fibi.MerchantID=fmdf.MerchantID AND fmdf.[Status]=2
                                                                                 AND fmdf.DistributionCode=fibi.DistributionCode                 
                    WHERE  fibi.WaybillNo= @WaybillNO ";
            SqlParameter[] parameters = { new SqlParameter("@WaybillNO", WaybillNO) };
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public DataTable GetAccountError9()
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fifi.IsAccount,fibi.BackStationTime,fibi.BackStationStatus
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=9
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
UNION ALL
SELECT fibi.WaybillNo,fifi.IsAccount,fibi.BackStationTime,fibi.BackStationStatus
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=9
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>getdate()-46
        AND fibi.BackStationTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
UNION ALL
SELECT fibi.WaybillNo,fifi.IsAccount,fibi.BackStationTime,fibi.BackStationStatus
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=9
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
		)
SELECT * FROM t
";

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120,CommandType.Text,sql).Tables[0];
        }

        public DataTable GetAccountError45(int errorType)
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=@ErrorType
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=@ErrorType
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>getdate()-46
        AND fibi.BackStationTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=@ErrorType
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
		)
SELECT t.*,mbi.MerchantName,ec.CompanyName,ael1.AreaType,fsdf.BasicDeliverFee,pca.ProvinceName,pca.CityName,pca.AreaName
 FROM t
JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID=t.MerchantID
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=t.ExpressCompanyID
JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,a.CreatTime,a.IsDeleted
 FROM RFD_PMS.dbo.Area a  JOIN RFD_PMS.dbo.City c
 ON a.CityID=c.CityID JOIN RFD_PMS.dbo.Province p  ON c.ProvinceID = p.ProvinceID) pca ON pca.AreaID=t.AreaID
LEFT JOIN RFD_FMS.dbo.AreaExpressLevelIncome ael1 (NOLOCK) ON ael1.AreaID = t.AreaID
                                                         AND ael1.[Enable] IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_StationDeliverFee fsdf(NOLOCK) ON fsdf.MerchantID = t.MerchantID AND fmdf.[Status]=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0
";
            SqlParameter[] parameters ={
                                           new SqlParameter("@ErrorType",SqlDbType.Int),
                                      };
            parameters[0].Value = errorType;
            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetAccountError3()
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=3
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=3
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>getdate()-46
        AND fibi.BackStationTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=3
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
		)
SELECT t.*,mbi.MerchantName,ec.CompanyName,pca.ProvinceName,pca.CityName,pca.AreaName
 FROM t
JOIN RFD_PMS.dbo.MerchantBaseInfo mbi(NOLOCK) ON mbi.ID=t.MerchantID
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=t.ExpressCompanyID
JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,a.CreatTime,a.IsDeleted
 FROM RFD_PMS.dbo.Area a  JOIN RFD_PMS.dbo.City c
 ON a.CityID=c.CityID JOIN RFD_PMS.dbo.Province p  ON c.ProvinceID = p.ProvinceID) pca ON pca.AreaID=t.AreaID
LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode AND fmdf.Status='2'
";

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetClearDatalist()
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode,fifi.IncomeFeeID
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount IN (3,4,5,9)
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode,fifi.IncomeFeeID
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount IN (3,4,5,9)
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>getdate()-46
        AND fibi.BackStationTime>'2012-10-20'
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode,fifi.IncomeFeeID
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK)
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount IN (3,4,5,9)
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime>'2012-10-20'
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
		)
SELECT t.*
 FROM t
";
            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection,180, CommandType.Text, sql).Tables[0];
        }

        public bool UpdateIncomeFeeIsAccount(Int64 incomeFeeId)
        {
            string sql = @"UPDATE LMS_RFD.dbo.FMS_IncomeFeeInfo SET IsAccount=0,UpdateTime=GETDATE() WHERE IncomeFeeID=@IncomeFeeID";
            SqlParameter[] parameters ={
                                           new SqlParameter("@IncomeFeeID",SqlDbType.BigInt),
                                      };
            parameters[0].Value = incomeFeeId;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql,parameters)>0;
        }

        /// <summary>
        /// 查询出未计算费用的 46天内存在未计算的
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetHistoryInComeFeeInfoDeliver(int TopNum)
        {
            string strSql =
                @"

WITH t AS (
SELECT TOP (@SqlNum)  
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
INNER JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK) ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>getdate()-46
        AND fibi.BackStationTime<getdate()-3
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN RFD_FMS.dbo.AreaExpressLevelIncome ael1 (NOLOCK) ON ael1.AreaID = t.AreaID
                                                         AND ael1.[Enable] IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_StationDeliverFee fsdf(NOLOCK) ON fsdf.MerchantID = t.MerchantID AND fmdf.[Status]=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0
";

            SqlParameter[] parameters = { new SqlParameter("@SqlNum", TopNum) };

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 查询出未计算费用的 46天内存在未计算的 拒收
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetHistoryInComeFeeInfoReturn(int TopNum)
        {
            string strSql =
                @"

WITH t AS (
SELECT TOP (@SqlNum)  
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
INNER JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK) ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0 
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime<getdate()-3
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN RFD_FMS.dbo.AreaExpressLevelIncome ael1 (NOLOCK) ON ael1.AreaID = t.AreaID
                                                         AND ael1.[Enable] IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_StationDeliverFee fsdf(NOLOCK) ON fsdf.MerchantID = t.MerchantID AND fmdf.[Status]=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0
";

            SqlParameter[] parameters = { new SqlParameter("@SqlNum", TopNum) };

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 查询出未计算费用的 46天内存在未计算的 上门退
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetHistoryInComeFeeInfoVisit(int TopNum)
        {
            string strSql =
                @"

WITH t AS (
SELECT TOP (@SqlNum)  
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM LMS_RFD.dbo.FMS_IncomeBaseInfo fibi(NOLOCK)
INNER JOIN LMS_RFD.dbo.FMS_IncomeFeeInfo fifi(NOLOCK) ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>getdate()-46
        AND fibi.ReturnTime<getdate()-3
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN RFD_FMS.dbo.AreaExpressLevelIncome ael1 (NOLOCK) ON ael1.AreaID = t.AreaID
                                                         AND ael1.[Enable] IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN RFD_FMS.dbo.FMS_StationDeliverFee fsdf(NOLOCK) ON fsdf.MerchantID = t.MerchantID AND fmdf.[Status]=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0
";

            SqlParameter[] parameters = { new SqlParameter("@SqlNum", TopNum) };

            return SqlHelperEx.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters).Tables[0];
        }
        public bool UpdateEvalStatusByIncomeFeeID(List<long> incomeFeeIDs)
        {
            throw new NotImplementedException();
        }
        public DataTable GetIncomeDeliveryFeeInfo(List<long> incomeFeeIDs)
        {
            throw new NotImplementedException();
        }

        public bool ExsitIncomeFeeInfoByNo(long waybillNo, long incomeFeeID)
        {
             throw new NotImplementedException();
        }
        public DataTable ExsitIncomeFeeInfoByNo(long waybillNo)
        {
            throw new NotImplementedException();
        }

	}
    
}
