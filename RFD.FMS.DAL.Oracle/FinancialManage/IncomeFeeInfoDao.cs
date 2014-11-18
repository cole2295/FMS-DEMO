using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.Model.FinancialManage;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public partial class IncomeFeeInfoDao : OracleDao, IIncomeFeeInfoDao
	{

        private const string TableName = @"FMS_IncomeFeeInfo";
		
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
			strSql.Append(string.Format(" where {0} = :{0}","WaybillNO"));
			var sqlParams = new List<OracleParameter>()
											{
												new OracleParameter(string.Format(":{0}","WaybillNO"),waybillno)
											};
			return Convert.ToInt64(OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray())) > 0;
		}


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(FMS_IncomeFeeInfo model)
        {
            if (model.IncomeFeeID <= 0)
            {
                model.IncomeFeeID = GetIdNew("SEQ_FMS_INCOMEFEEINFO");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("insert into {0}(", TableName));
            strSql.Append(" IncomeFeeID ,");
            strSql.Append(" WaybillNO ,");
            strSql.Append(" IsAccount ,");
            strSql.Append(" AccountStandard ,");
            strSql.Append(" AccountFare ,");
            strSql.Append(" IsProtected ,");
            strSql.Append(" ProtectedStandard ,");
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
            strSql.Append(" IsDeductAcount,");
            strSql.Append(" IsChange,");
            strSql.Append(" AreaType");
            strSql.Append(" ,ISCOD ");
            strSql.Append(") values (");
            strSql.Append(" :IncomeFeeID , ");
            strSql.Append(" :WaybillNO , ");
            strSql.Append(" :IsAccount , ");
            strSql.Append(" :AccountStandard , ");
            strSql.Append(" :AccountFare , ");
            strSql.Append(" :IsProtected , ");
            strSql.Append(" :ProtectedStandard , ");
            strSql.Append(" :ProtectedFee , ");
            strSql.Append(" :IsReceive , ");
            strSql.Append(" :ReceiveStandard , ");
            strSql.Append(" :ReceiveFee , ");
            strSql.Append(" :CreateBy , ");
            strSql.Append(" :CreateTime , ");
            strSql.Append(" :UpdateBy , ");
            strSql.Append(" :UpdateTime , ");
            strSql.Append(" :IsDeleted , ");
            strSql.Append(" :TransferPayType , ");
            strSql.Append(" :DeputizeAmount , ");
            strSql.Append(" :POSReceiveStandard , ");
            strSql.Append(" :POSReceiveFee , ");
            strSql.Append(" :CashReceiveServiceStandard , ");
            strSql.Append(" :CashReceiveServiceFee , ");
            strSql.Append(" :POSReceiveServiceStandard , ");
            strSql.Append(" :POSReceiveServiceFee , ");
            strSql.Append(" :ExpressReceiveBasicDeduct , ");
            strSql.Append(" :ExpressSendBasicDeduct , ");
            strSql.Append(" :ExpressAreaDeduct , ");
            strSql.Append(" :ExpressWeightDeduct , ");
            strSql.Append(" :ProgramReceiveBasicDeduct , ");
            strSql.Append(" :ProgramSendBasicDeduct , ");
            strSql.Append(" :ProgramAreaDeduct , ");
            strSql.Append(" :ProgramWeightDeduct , ");
            strSql.Append(" :IsDeductAcount,  ");
            strSql.Append(" :IsChange,  ");
            strSql.Append(" :AreaType  ");
            strSql.Append(" ,:ISCOD  ");
            strSql.Append(") ");

            OracleParameter[] parameters = 
            {
                new OracleParameter(string.Format(":{0}","IncomeFeeID"), model.IncomeFeeID),
				new OracleParameter(string.Format(":{0}","WaybillNO"), model.WaybillNO),
				new OracleParameter(string.Format(":{0}","IsAccount"), model.IsAccount),
				new OracleParameter(string.Format(":{0}","AccountStandard"), model.AccountStandard),
				new OracleParameter(string.Format(":{0}","AccountFare"), model.AccountFare),
				new OracleParameter(string.Format(":{0}","IsProtected"), model.IsProtected),
				new OracleParameter(string.Format(":{0}","ProtectedStandard"), model.ProtectedStandard),
				new OracleParameter(string.Format(":{0}","ProtectedFee"), model.ProtectedFee),
				new OracleParameter(string.Format(":{0}","IsReceive"), model.IsReceive),
				new OracleParameter(string.Format(":{0}","ReceiveStandard"), model.ReceiveStandard),
				new OracleParameter(string.Format(":{0}","ReceiveFee"), model.ReceiveFee),
				new OracleParameter(string.Format(":{0}","CreateBy"), model.CreateBy),
				new OracleParameter(string.Format(":{0}","CreateTime"), model.CreateTime),
				new OracleParameter(string.Format(":{0}","UpdateBy"), model.UpdateBy),
				new OracleParameter(string.Format(":{0}","UpdateTime"), model.UpdateTime),
				new OracleParameter(string.Format(":{0}","IsDeleted"), model.IsDeleted),
				new OracleParameter(string.Format(":{0}","TransferPayType"), model.TransferPayType),
				new OracleParameter(string.Format(":{0}","DeputizeAmount"), model.DeputizeAmount),
				new OracleParameter(string.Format(":{0}","POSReceiveStandard"), model.POSReceiveStandard),
				new OracleParameter(string.Format(":{0}","POSReceiveFee"), model.POSReceiveFee),
				new OracleParameter(string.Format(":{0}","CashReceiveServiceStandard"), model.CashReceiveServiceStandard),
				new OracleParameter(string.Format(":{0}","CashReceiveServiceFee"), model.CashReceiveServiceFee),
				new OracleParameter(string.Format(":{0}","POSReceiveServiceStandard"), model.POSReceiveServiceStandard),
				new OracleParameter(string.Format(":{0}","POSReceiveServiceFee"), model.POSReceiveServiceFee),
				new OracleParameter(string.Format(":{0}","ExpressReceiveBasicDeduct"), model.ExpressReceiveBasicDeduct),
				new OracleParameter(string.Format(":{0}","ExpressSendBasicDeduct"), model.ExpressSendBasicDeduct),
				new OracleParameter(string.Format(":{0}","ExpressAreaDeduct"), model.ExpressAreaDeduct),
				new OracleParameter(string.Format(":{0}","ExpressWeightDeduct"), model.ExpressWeightDeduct),
				new OracleParameter(string.Format(":{0}","ProgramReceiveBasicDeduct"), model.ProgramReceiveBasicDeduct),
				new OracleParameter(string.Format(":{0}","ProgramSendBasicDeduct"), model.ProgramSendBasicDeduct),
				new OracleParameter(string.Format(":{0}","ProgramAreaDeduct"), model.ProgramAreaDeduct),
				new OracleParameter(string.Format(":{0}","ProgramWeightDeduct"), model.ProgramWeightDeduct),
				new OracleParameter(string.Format(":{0}","IsDeductAcount"), model.IsDeductAcount),
				new OracleParameter(string.Format(":{0}","IsChange"), 1),
                new OracleParameter(string.Format(":{0}","AreaType"), model.AreaType),
                new OracleParameter(string.Format(":{0}","ISCOD"), model.ISCod),
            };

            int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败!");

            return (int)model.IncomeFeeID;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FMS_IncomeFeeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("update {0} set ", TableName));

            strSql.Append(" IsAccount = :IsAccount,");
            strSql.Append(" AccountStandard = :AccountStandard,");
            strSql.Append(" AccountFare = :AccountFare,");
            strSql.Append(" IsProtected = :IsProtected,");
            strSql.Append(" ProtectedStandard = :ProtectedStandard,");
            strSql.Append(" ProtectedFee = :ProtectedFee,");
            strSql.Append(" IsReceive = :IsReceive,");
            strSql.Append(" ReceiveStandard = :ReceiveStandard,");
            strSql.Append(" ReceiveFee = :ReceiveFee,");
            strSql.Append(" CreateBy = :CreateBy,");
            strSql.Append(" UpdateBy = :UpdateBy,");
            strSql.Append(" UpdateTime = sysdate,");
            strSql.Append(" IsDeleted = :IsDeleted,");
            strSql.Append(" TransferPayType = :TransferPayType,");
            strSql.Append(" DeputizeAmount = :DeputizeAmount,");
            strSql.Append(" POSReceiveStandard = :POSReceiveStandard,");
            strSql.Append(" POSReceiveFee = :POSReceiveFee,");
            strSql.Append(" CashReceiveServiceStandard = :CashReceiveServiceStandard,");
            strSql.Append(" CashReceiveServiceFee = :CashReceiveServiceFee,");
            strSql.Append(" POSReceiveServiceStandard = :POSReceiveServiceStandard,");
            strSql.Append(" POSReceiveServiceFee = :POSReceiveServiceFee,");
            strSql.Append(" ExpressReceiveBasicDeduct = :ExpressReceiveBasicDeduct,");
            strSql.Append(" ExpressSendBasicDeduct = :ExpressSendBasicDeduct,");
            strSql.Append(" ExpressAreaDeduct = :ExpressAreaDeduct,");
            strSql.Append(" ExpressWeightDeduct = :ExpressWeightDeduct,");
            strSql.Append(" ProgramReceiveBasicDeduct = :ProgramReceiveBasicDeduct,");
            strSql.Append(" ProgramSendBasicDeduct = :ProgramSendBasicDeduct,");
            strSql.Append(" ProgramAreaDeduct = :ProgramAreaDeduct,");
            strSql.Append(" ProgramWeightDeduct = :ProgramWeightDeduct,");
            strSql.Append(" IsDeductAcount = :IsDeductAcount,");
            strSql.Append(" AreaType = :AreaType,");
            strSql.Append(" IsCod = :IsCod,");
            strSql.Append(" IsChange = :IsChange");

            strSql.Append(string.Format(" where {0} = :{0}","WaybillNO"));

            OracleParameter[] parameters = 
            {
				new OracleParameter(string.Format(":{0}","WaybillNO"), model.WaybillNO),
                new OracleParameter(string.Format(":{0}","IsAccount"), model.IsAccount),
                new OracleParameter(string.Format(":{0}","AccountStandard"), model.AccountStandard),
                new OracleParameter(string.Format(":{0}","AccountFare"), model.AccountFare),
                new OracleParameter(string.Format(":{0}","IsProtected"), model.IsProtected),
                new OracleParameter(string.Format(":{0}","ProtectedStandard"), model.ProtectedStandard),
                new OracleParameter(string.Format(":{0}","ProtectedFee"), model.ProtectedFee),
                new OracleParameter(string.Format(":{0}","IsReceive"), model.IsReceive),
                new OracleParameter(string.Format(":{0}","ReceiveStandard"), model.ReceiveStandard),
                new OracleParameter(string.Format(":{0}","ReceiveFee"), model.ReceiveFee),
                new OracleParameter(string.Format(":{0}","CreateBy"), model.CreateBy),
                new OracleParameter(string.Format(":{0}","UpdateBy"), model.UpdateBy),
                new OracleParameter(string.Format(":{0}","IsDeleted"), model.IsDeleted),
                new OracleParameter(string.Format(":{0}","TransferPayType"), model.TransferPayType),
                new OracleParameter(string.Format(":{0}","DeputizeAmount"), model.DeputizeAmount),
                new OracleParameter(string.Format(":{0}","POSReceiveStandard"), model.POSReceiveStandard),
                new OracleParameter(string.Format(":{0}","POSReceiveFee"), model.POSReceiveFee),
                new OracleParameter(string.Format(":{0}","CashReceiveServiceStandard"),model.CashReceiveServiceStandard),								
                new OracleParameter(string.Format(":{0}","CashReceiveServiceFee"), model.CashReceiveServiceFee),
                new OracleParameter(string.Format(":{0}","POSReceiveServiceStandard"), model.POSReceiveServiceStandard),
                new OracleParameter(string.Format(":{0}","POSReceiveServiceFee"), model.POSReceiveServiceFee),
                new OracleParameter(string.Format(":{0}","ExpressReceiveBasicDeduct"), model.ExpressReceiveBasicDeduct),
                new OracleParameter(string.Format(":{0}","ExpressSendBasicDeduct"), model.ExpressSendBasicDeduct),	
                new OracleParameter(string.Format(":{0}","ExpressAreaDeduct"), model.ExpressAreaDeduct),
                new OracleParameter(string.Format(":{0}","ExpressWeightDeduct"), model.ExpressWeightDeduct),
                new OracleParameter(string.Format(":{0}","ProgramReceiveBasicDeduct"), model.ProgramReceiveBasicDeduct),
                new OracleParameter(string.Format(":{0}","ProgramSendBasicDeduct"), model.ProgramSendBasicDeduct),	
                new OracleParameter(string.Format(":{0}","ProgramAreaDeduct"), model.ProgramAreaDeduct),
                new OracleParameter(string.Format(":{0}","ProgramWeightDeduct"), model.ProgramWeightDeduct),
                new OracleParameter(string.Format(":{0}","IsDeductAcount"), model.IsDeductAcount),	
                new OracleParameter(string.Format(":{0}","AreaType"), model.AreaType),	
                new OracleParameter(string.Format(":{0}","IsCod"), model.ISCod),	
                new OracleParameter(string.Format(":{0}","IsChange"), 1)
            };

            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

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

            strSql.Append(" IsAccount = :IsAccount,");
            strSql.Append(" AccountFare = :AccountFare,");
            
            strSql.Append(" UpdateBy = :UpdateBy,");
            strSql.Append(" UpdateTime = sysdate,");
            strSql.Append(" TransferPayType = :TransferPayType,");
            strSql.Append(" IsChange = :IsChange");

            strSql.Append(string.Format(" where {0} = :{0}", "WaybillNO"));

            OracleParameter[] parameters = 
            {								
                new OracleParameter(string.Format(":{0}","WaybillNO"), model.WaybillNO),
                new OracleParameter(string.Format(":{0}","IsAccount"), model.IsAccount),
                new OracleParameter(string.Format(":{0}","AccountFare"), model.AccountFare),
                
                new OracleParameter(string.Format(":{0}","UpdateBy"), model.UpdateBy),
                new OracleParameter(string.Format(":{0}","TransferPayType"), model.TransferPayType),
                new OracleParameter(string.Format(":{0}","IsChange"), 1)
            };

            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

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
            strSql.Append(string.Format(" where {0} = :{0}", "WaybillNO"));
            var sqlParams = new List<OracleParameter>()
											{
												new OracleParameter(string.Format(":{0}","WaybillNO"),waybillno)
											};
            var model = new FMS_IncomeFeeInfo();
            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
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
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item =>
                {
                    strSql.Append(string.Format(" and {0} = :{0}", item.Key));
                    sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value));
                });
            }
            var modelList = new List<FMS_IncomeFeeInfo>();
            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
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
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item => sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value)));
            }
            var obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, sqlParams.ToArray());
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
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item =>
                {
                    strSql.Append(string.Format(" and {0} = :{0}", item.Key));
                    sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value));
                });
            }
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray()).Tables[0];
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
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item => sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value)));
            }
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, sqlParams.ToArray()).Tables[0];
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
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item => sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value)));
            }
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, sqlParams.ToArray()).Tables[0];
        }
        /// <summary>
        /// 查询出未计算费用的
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetInComeFeeInfo(int TopNum)
        {
            string strSql =
                @"WITH t AS (
SELECT 
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM FMS_IncomeBaseInfo fibi
INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-3
        AND fibi.ReturnTime <to_date(to_char(sysdate-1/24,'yyyy-mm-dd HH24:MI:SS'),'yyyy-mm-dd HH24:MI:SS')
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
    AND rownum < :SqlNum
    AND fibi.CreateTime > trunc(sysdate-90)
        --AND fibi.WaybillNo =11210040000212
UNION ALL
SELECT 
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM FMS_IncomeBaseInfo fibi
INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>sysdate-3
        AND fibi.BackStationTime <to_date(to_char(sysdate-1/24,'yyyy-mm-dd HH24:MI:SS'),'yyyy-mm-dd HH24:MI:SS')
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType in (0,3)--妥投
        AND rownum < :SqlNum
        AND fibi.CreateTime > trunc(sysdate-90)
        --AND fibi.WaybillNo =11210040000212
UNION ALL
SELECT 
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM FMS_IncomeBaseInfo fibi
INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0 
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-3
        AND fibi.ReturnTime <to_date(to_char(sysdate-1/24,'yyyy-mm-dd HH24:MI:SS'),'yyyy-mm-dd HH24:MI:SS')
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus in(7,13)--拒收入库
        AND rownum < :SqlNum
        AND fibi.CreateTime > trunc(sysdate-90)
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN FMS_MerchantDeliverFee fmdf ON t.MerchantID=fmdf.MerchantID AND fmdf.Status=2 AND fmdf.DistributionCode=t.DistributionCode";

            OracleParameter[] parameters = { new OracleParameter(":SqlNum", OracleDbType.Decimal) };
            parameters[0].Value = TopNum;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection,180, CommandType.Text, strSql, parameters).Tables[0];
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
                    FROM FMS_IncomeBaseInfo fibi
                    INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO AND fifi.IsDeleted=0
                    LEFT JOIN FMS_MerchantDeliverFee fmdf ON fibi.MerchantID=fmdf.MerchantID AND fmdf.Status=2 AND fmdf.DistributionCode=fibi.DistributionCode                    
                    WHERE  fibi.WaybillNo= :WaybillNO AND fibi.CreateTime > trunc(sysdate-90) ";
            OracleParameter[] parameters = { new OracleParameter(":WaybillNO", OracleDbType.Decimal) };
            parameters[0].Value = WaybillNO;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public DataTable GetAccountError9()
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fifi.IsAccount,fibi.BackStationTime,fibi.BackStationStatus
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=9
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fifi.IsAccount,fibi.BackStationTime,fibi.BackStationStatus
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=9
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>sysdate-46
        AND fibi.BackStationTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fifi.IsAccount,fibi.BackStationTime,fibi.BackStationStatus
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=9
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
        AND fibi.CreateTime > trunc(sysdate-90)
		)
SELECT * FROM t
";

            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetAccountError45(int errorType)
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=:ErrorType
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=:ErrorType
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>sysdate-46
        AND fibi.BackStationTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=:ErrorType
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
        AND fibi.CreateTime > trunc(sysdate-90)
		)
SELECT t.*,mbi.MerchantName,ec.CompanyName,ael1.AreaType,fsdf.BasicDeliverFee,pca.ProvinceName,pca.CityName,pca.AreaName
 FROM t
JOIN MerchantBaseInfo mbi ON mbi.ID=t.MerchantID
JOIN ExpressCompany ec ON ec.ExpressCompanyID=t.ExpressCompanyID
JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,a.CreatTime,a.IsDeleted
 FROM Area a  JOIN City c
 ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID) pca ON pca.AreaID=t.AreaID
LEFT JOIN AreaExpressLevelIncome ael1  ON ael1.AreaID = t.AreaID
                                                         AND ael1.IsEnable IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN FMS_MerchantDeliverFee fmdf ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN FMS_StationDeliverFee fsdf ON fsdf.MerchantID = t.MerchantID AND fmdf.Status=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0
";
            OracleParameter[] parameters ={
                                              new OracleParameter(":ErrorType",OracleDbType.Decimal),
                                         };
            parameters[0].Value = errorType;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetAccountError3()
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=3
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=3
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>sysdate-46
        AND fibi.BackStationTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount=3
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
        AND fibi.CreateTime > trunc(sysdate-90)
		)
SELECT t.*,mbi.MerchantName,ec.CompanyName,pca.ProvinceName,pca.CityName,pca.AreaName
 FROM t
JOIN MerchantBaseInfo mbi ON mbi.ID=t.MerchantID
JOIN ExpressCompany ec ON ec.ExpressCompanyID=t.ExpressCompanyID
JOIN (SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,a.CreatTime,a.IsDeleted
 FROM Area a JOIN City c
 ON a.CityID=c.CityID JOIN Province p ON c.ProvinceID = p.ProvinceID) pca ON pca.AreaID=t.AreaID
LEFT JOIN FMS_MerchantDeliverFee fmdf ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode AND fmdf.Status='2'
";

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, sql).Tables[0];
        }

        public DataTable GetClearDatalist()
        {
            string sql = @"
WITH t AS (
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode,fifi.IncomeFeeID
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount IN (3,4,5,9)
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode,fifi.IncomeFeeID
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount IN (3,4,5,9)
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>sysdate-46
        AND fibi.BackStationTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType=0--妥投
        AND fibi.CreateTime > trunc(sysdate-90)
UNION ALL
SELECT fibi.WaybillNo,fibi.MerchantID,fibi.ExpressCompanyID,fibi.AreaID,fibi.DistributionCode,fifi.IncomeFeeID
FROM FMS_IncomeBaseInfo fibi
JOIN FMS_IncomeFeeInfo fifi
ON fibi.WaybillNo = fifi.WaybillNO
WHERE (1=1)
		AND IsAccount IN (3,4,5,9)
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
        AND fibi.ReturnTime>to_date('2012-10-20','yyyy-mm-dd')
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus=7--拒收入库
        AND fibi.CreateTime > trunc(sysdate-90)
		)
SELECT t.*
 FROM t
";
            return OracleHelper.ExecuteDataset(ReadOnlyConnection,180, CommandType.Text, sql).Tables[0];
        }

        public bool UpdateIncomeFeeIsAccount(Int64 incomeFeeId)
        {
            string sql = @"UPDATE FMS_IncomeFeeInfo SET IsAccount=0,UpdateTime=sysdate WHERE IncomeFeeID=:IncomeFeeID";
            OracleParameter[] parameters ={
                                           new OracleParameter(":IncomeFeeID",OracleDbType.Decimal),
                                      };
            parameters[0].Value = incomeFeeId;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 查询出未计算费用 未计算46天内的
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetHistoryInComeFeeInfoDeliver(int TopNum)
        {
            string strSql =
                @"WITH t AS (
SELECT 
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM FMS_IncomeBaseInfo fibi
INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.BackStationTime>sysdate-46
		AND FIBI.BackStationStatus ='3' AND fibi.WaybillType in('0','3')--妥投
        AND rownum < :SqlNum
        AND fibi.CreateTime > trunc(sysdate-90)
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN AreaExpressLevelIncome ael1 ON ael1.AreaID = t.AreaID
                                                         AND ael1.IsEnable IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN FMS_MerchantDeliverFee fmdf ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN FMS_StationDeliverFee fsdf ON fsdf.MerchantID = t.MerchantID AND fmdf.Status=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0 ";

            OracleParameter[] parameters = { new OracleParameter(":SqlNum", TopNum) };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection,120, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 查询出未计算费用 未计算46天内的
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetHistoryInComeFeeInfoReturn(int TopNum)
        {
            string strSql =
                @"WITH t AS (
SELECT 
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM FMS_IncomeBaseInfo fibi
INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0 
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
		AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
		AND FIBI.BackStationStatus ='5' and  fibi.SubStatus in(7,13)--拒收入库
        AND rownum < :SqlNum
        AND fibi.CreateTime > trunc(sysdate-90)
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN AreaExpressLevelIncome ael1 ON ael1.AreaID = t.AreaID
                                                         AND ael1.IsEnable IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN FMS_MerchantDeliverFee fmdf ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN FMS_StationDeliverFee fsdf ON fsdf.MerchantID = t.MerchantID AND fmdf.Status=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0 ";

            OracleParameter[] parameters = { new OracleParameter(":SqlNum", TopNum) };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 查询出未计算费用 未计算46天内的
        /// </summary>
        /// <param name="TopNum"></param>
        /// <returns></returns>
        public DataTable GetHistoryInComeFeeInfoVisit(int TopNum)
        {
            string strSql =
                @"WITH t AS (
SELECT 
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM FMS_IncomeBaseInfo fibi
INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO                            
WHERE (1=1) 
		AND fifi.IsAccount=0
		AND fifi.IsDeleted=0
		AND fibi.MerchantID NOT IN (8,9) 
        AND fibi.DistributionCode='rfd'
        AND fibi.ReturnTime>sysdate-46
		AND FIBI.BackStationStatus ='3' AND fibi.SubStatus IN (6) AND fibi.WaybillType IN (1,2)--退换货入库
        AND rownum < :SqlNum
        AND fibi.CreateTime > trunc(sysdate-90)
        --AND fibi.WaybillNo =11210040000212
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN AreaExpressLevelIncome ael1 ON ael1.AreaID = t.AreaID
                                                         AND ael1.IsEnable IN (1,2) 
                                                         AND ael1.MerchantID=t.MerchantID
                                                         AND ael1.WareHouseID=t.ExpressCompanyID
                                                         AND ael1.DistributionCode=t.DistributionCode
LEFT JOIN FMS_MerchantDeliverFee fmdf ON t.MerchantID=fmdf.MerchantID AND fmdf.DistributionCode=t.DistributionCode
LEFT JOIN FMS_StationDeliverFee fsdf ON fsdf.MerchantID = t.MerchantID AND fmdf.Status=2 AND fsdf.AreaType= ael1.AreaType 
							AND  fsdf.ExpressCompanyID=t.ExpressCompanyID AND fsdf.DistributionCode=t.DistributionCode AND fsdf.IsDeleted=0 ";

            OracleParameter[] parameters = { new OracleParameter(":SqlNum", TopNum) };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 120, CommandType.Text, strSql, parameters).Tables[0];
        }


        public bool UpdateEvalStatusByIncomeFeeID(List<long> incomeFeeIDs)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < incomeFeeIDs.Count; i++)
            {
                parameters.Add(new OracleParameter(":ID" + i, OracleDbType.Int64) { Value = DataConvert.ToLong(incomeFeeIDs[i]) });
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(":ID" + i);
            }

            string sql = String.Format(@"update FMS_IncomeFeeInfo set IsAccount=0 where IncomeFeeID in ({0})", sb.ToString());

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql,parameters.ToArray()) > 0;
        }
        public DataTable GetIncomeDeliveryFeeInfo(List<long> incomeFeeIDs)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < incomeFeeIDs.Count; i++)
            {
                parameters.Add(new OracleParameter(":ID" + i, OracleDbType.Int64) { Value = DataConvert.ToLong(incomeFeeIDs[i]) });
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(":ID" + i);
            }

            string sql = string.Format(
                @"select feeInfo.IncomeFeeID,feeInfo.WaybillNo,feeInfo.AreaType,Info.accountweight,feeInfo.AccountStandard,feeInfo.AccountFare
                           from FMS_IncomeFeeInfo feeInfo join FMS_IncomeBaseInfo Info on feeInfo.WaybillNo=Info.WaybillNo
                           where feeInfo.IncomeFeeID in ({0})", sb.ToString());
            
            var ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,parameters.ToArray());
            return ds.Tables[0];
        }
        public bool ExsitIncomeFeeInfoByNo(long waybillNo, long incomeFeeID)
        {
            string sql = @"select count(1) from FMS_IncomeFeeInfo where WaybillNo =:WaybillNo and IncomeFeeID =:IncomeFeeID";
            OracleParameter[] parameter = {
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo},
                                              new OracleParameter(":IncomeFeeID",OracleDbType.Int64){Value = incomeFeeID}
                                          };
            var ret = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameter);
            return Convert.ToInt32(ret) > 0;
        }
        public DataTable ExsitIncomeFeeInfoByNo(long waybillNo)
        {
            string sql = @"select IncomeFeeID from FMS_IncomeFeeInfo where WaybillNo =:WaybillNo ";
            OracleParameter[] parameter = {
                                              new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo},
                                             };
            var ret = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameter);
            return ret.Tables[0];
        }

        public DataTable GetInComeFeeInfoByOrderNo(long orderno)
        {
            string strSql =
                 @"WITH t AS (
SELECT 
		fibi.WaybillNo,fibi.WaybillType,fibi.MerchantID,fibi.DeliverStationID
        ,fibi.BackStationStatus,fibi.NeedPayAmount,fibi.ProtectedAmount,fibi.SignType
        ,fibi.WayBillInfoWeight,fibi.SubStatus,fibi.AccountWeight,fibi.AcceptType,fifi.AreaType
        ,fibi.AreaID,fibi.ExpressCompanyID,fibi.DistributionCode,fibi.WaybillCategory
FROM FMS_IncomeBaseInfo fibi
INNER JOIN FMS_IncomeFeeInfo fifi ON fibi.WaybillNo=fifi.WaybillNO                            
 where 1=1 AND fibi.WaybillNo =:WaybillNo
        )
SELECT t.*,fmdf.ReceiveFeeRate,
fmdf.ReceivePOSFeeRate,fmdf.CashServiceFee,fmdf.POSServiceFee,fmdf.ProtectedParmer,
fmdf.RefuseFeeRate,fmdf.VisitReturnsFee,fmdf.VisitReturnsVFee,fmdf.VisitChangeFee,
fmdf.ExtraReceiveFeeRate,fmdf.ExtraReceivePOSFeeRate,fmdf.ExtraCashServiceFee,fmdf.ExtraPOSServiceFee,fmdf.ExtraProtected,
fmdf.ExtraRefuseFeeRate,fmdf.ExtraVisitReturnsFee,fmdf.ExtraVisitReturnsVFee,fmdf.ExtraVisitChangeFee,fmdf.IsCategory,
fmdf.WeightType,WeightValueRule
FROM t
LEFT JOIN FMS_MerchantDeliverFee fmdf ON t.MerchantID=fmdf.MerchantID AND fmdf.Status=2 AND fmdf.DistributionCode=t.DistributionCode";

            OracleParameter[] parameters = { new OracleParameter(":WaybillNo", OracleDbType.Int64) };
            parameters[0].Value = orderno;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, 180, CommandType.Text, strSql, parameters).Tables[0];
        }
	}
}
