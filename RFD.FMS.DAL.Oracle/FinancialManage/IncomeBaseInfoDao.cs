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
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public partial class IncomeBaseInfoDao : OracleDao, IIncomeBaseInfoDao
	{

        private const string TableName = @"FMS_IncomeBaseInfo";
		
		private const string PagingTemplate = @"SELECT  RowIndex ,
																					T.*
																			FROM    ( SELECT    T2.* ,
																								ROW_NUMBER() OVER ( ORDER BY {0} DESC ) AS RowIndex
																					  FROM   ( {1} )  T2
																					) AS T
																			WHERE   T.RowIndex > {2}
																			AND T.RowIndex <= {3}";
		
		public IncomeBaseInfoDao()
		{
		}
		
		public bool Exists(Int64 incomeid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append(string.Format("select count(1) from {0}",TableName));
            strSql.Append(string.Format(" where {0} = :{0}", "WaybillNo"));
			var sqlParams = new List<OracleParameter>()
											{
												new OracleParameter(string.Format(":{0}","WaybillNo"),incomeid)
											};
			return Convert.ToInt64(OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray())) > 0;
		}
		
		
		
		
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(FMS_IncomeBaseInfo model)
		{
            if (model.IncomeID <= 0)
            {
                model.IncomeID = GetIdNew("SEQ_FMS_INCOMEBASEINFO");
            }

			StringBuilder strSql=new StringBuilder();
			strSql.Append(string.Format("insert into {0}(",TableName));
            strSql.Append(" IncomeID , ");
			strSql.Append(" WaybillNo , ");
			strSql.Append(" WaybillType , ");
			strSql.Append(" MerchantID , ");
			strSql.Append(" ExpressCompanyID , ");
			strSql.Append(" FinalExpressCompanyID , ");
			strSql.Append(" DeliverStationID , ");
			strSql.Append(" TopCODCompanyID , ");
			strSql.Append(" RfdAcceptTime , ");
			strSql.Append(" DeliverTime , ");
			strSql.Append(" ReturnTime , ");
			strSql.Append(" ReturnExpressCompanyID , ");
			strSql.Append(" BackStationTime , ");
			strSql.Append(" BackStationStatus , ");
			strSql.Append(" ProtectedAmount , ");
			strSql.Append(" TotalAmount , ");
			strSql.Append(" PaidAmount , ");
			strSql.Append(" NeedPayAmount , ");
			strSql.Append(" BackAmount , ");
			strSql.Append(" NeedBackAmount , ");
			strSql.Append(" AccountWeight , ");
			strSql.Append(" AreaID , ");
			strSql.Append(" ReceiveAddress , ");
			strSql.Append(" SignType , ");
			strSql.Append(" InefficacyStatus , ");
            strSql.Append(" ReceiveStationID , ");
            strSql.Append(" ReceiveDeliverManID , ");
            strSql.Append(" DistributionCode , ");
            strSql.Append(" CurrentDistributionCode , ");
            strSql.Append(" WayBillInfoWeight , ");
            strSql.Append(" SubStatus , ");
            strSql.Append(" AcceptType , ");
            strSql.Append(" CustomerOrder , ");
            strSql.Append(" OriginDepotNo , ");
            strSql.Append(" PeriodAccountCode , ");
            strSql.Append(" WaybillCategory , ");
			strSql.Append(" createtime , ");
			strSql.Append(" updatetime,  ");
            strSql.Append(" IsChange,  ");
            strSql.Append(" DeliverCode  ");
			strSql.Append(") values (");
            strSql.Append(" :IncomeID , ");
			strSql.Append(" :WaybillNo , ");
			strSql.Append(" :WaybillType , ");
			strSql.Append(" :MerchantID , ");
			strSql.Append(" :ExpressCompanyID , ");
			strSql.Append(" :FinalExpressCompanyID , ");
			strSql.Append(" :DeliverStationID , ");
			strSql.Append(" :TopCODCompanyID , ");
            strSql.Append(" to_date(:RfdAcceptTime ,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" to_date(:DeliverTime ,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" to_date(:ReturnTime  ,'yyyy-mm-dd hh24:mi:ss'), ");
            strSql.Append(" :ReturnExpressCompanyID , ");
            strSql.Append(" to_date(:BackStationTime ,'yyyy-mm-dd hh24:mi:ss'), ");
			strSql.Append(" :BackStationStatus , ");
			strSql.Append(" :ProtectedAmount , ");
			strSql.Append(" :TotalAmount , ");
			strSql.Append(" :PaidAmount , ");
			strSql.Append(" :NeedPayAmount , ");
			strSql.Append(" :BackAmount , ");
			strSql.Append(" :NeedBackAmount , ");
			strSql.Append(" :AccountWeight , ");
			strSql.Append(" :AreaID , ");
			strSql.Append(" :ReceiveAddress , ");
			strSql.Append(" :SignType , ");
			strSql.Append(" :InefficacyStatus , ");
            strSql.Append(" :ReceiveStationID , ");
            strSql.Append(" :ReceiveDeliverManID , ");
            strSql.Append(" :DistributionCode , ");
            strSql.Append(" :CurrentDistributionCode , ");
            strSql.Append(" :WayBillInfoWeight , ");
            strSql.Append(" :SubStatus , ");
            strSql.Append(" :AcceptType , ");
            strSql.Append(" :CustomerOrder , ");
            strSql.Append(" :OriginDepotNo , ");
            strSql.Append(" :PeriodAccountCode , ");
            strSql.Append(" :WaybillCategory , ");
            strSql.Append(" to_date(:createtime,'yyyy-mm-dd hh24:mi:ss') , ");
            strSql.Append(" to_date(:updatetime,'yyyy-mm-dd hh24:mi:ss'),  ");
            strSql.Append(" :IsChange,  ");
            strSql.Append(" :DeliverCode  ");
			strSql.Append(") ");

			OracleParameter[] parameters = 
            {
                new OracleParameter(string.Format(":{0}","IncomeID"), model.IncomeID),
			    new OracleParameter(string.Format(":{0}","WaybillNo"), model.WaybillNo),
			    new OracleParameter(string.Format(":{0}","WaybillType"), model.WaybillType),
			    new OracleParameter(string.Format(":{0}","MerchantID"), model.MerchantID),
			    new OracleParameter(string.Format(":{0}","ExpressCompanyID"), model.ExpressCompanyID),
			    new OracleParameter(string.Format(":{0}","FinalExpressCompanyID"), model.FinalExpressCompanyID),
			    new OracleParameter(string.Format(":{0}","DeliverStationID"), model.DeliverStationID),
			    new OracleParameter(string.Format(":{0}","TopCODCompanyID"), model.TopCODCompanyID),
			    new OracleParameter(string.Format(":{0}","RfdAcceptTime"), Convert.ToDateTime(model.RfdAcceptTime).ToString("yyyy-MM-dd HH:mm:ss")),
			    new OracleParameter(string.Format(":{0}","DeliverTime"), Convert.ToDateTime(model.DeliverTime).ToString("yyyy-MM-dd HH:mm:ss")),
			    new OracleParameter(string.Format(":{0}","ReturnTime"), Convert.ToDateTime(model.ReturnTime).ToString("yyyy-MM-dd HH:mm:ss")),
			    new OracleParameter(string.Format(":{0}","ReturnExpressCompanyID"), model.ReturnExpressCompanyID),
			    new OracleParameter(string.Format(":{0}","BackStationTime"), Convert.ToDateTime(model.BackStationTime).ToString("yyyy-MM-dd HH:mm:ss")),
			    new OracleParameter(string.Format(":{0}","BackStationStatus"), model.BackStationStatus),
			    new OracleParameter(string.Format(":{0}","ProtectedAmount"), model.ProtectedAmount),
			    new OracleParameter(string.Format(":{0}","TotalAmount"), model.TotalAmount),
			    new OracleParameter(string.Format(":{0}","PaidAmount"), model.PaidAmount),
			    new OracleParameter(string.Format(":{0}","NeedPayAmount"), model.NeedPayAmount),
			    new OracleParameter(string.Format(":{0}","BackAmount"), model.BackAmount),
			    new OracleParameter(string.Format(":{0}","NeedBackAmount"), model.NeedBackAmount),
			    new OracleParameter(string.Format(":{0}","AccountWeight"), model.AccountWeight),
			    new OracleParameter(string.Format(":{0}","AreaID"), model.AreaID),
			    new OracleParameter(string.Format(":{0}","ReceiveAddress"), model.ReceiveAddress),
			    new OracleParameter(string.Format(":{0}","SignType"), model.SignType),
			    new OracleParameter(string.Format(":{0}","InefficacyStatus"), model.InefficacyStatus),
			    new OracleParameter(string.Format(":{0}","ReceiveStationID"), model.ReceiveStationID),
			    new OracleParameter(string.Format(":{0}","ReceiveDeliverManID"), model.ReceiveDeliverManID),
			    new OracleParameter(string.Format(":{0}","DistributionCode"), model.DistributionCode),
			    new OracleParameter(string.Format(":{0}","CurrentDistributionCode"), model.CurrentDistributionCode),
                new OracleParameter(string.Format(":{0}","WayBillInfoWeight"), model.WayBillInfoWeight),
			    new OracleParameter(string.Format(":{0}","SubStatus"), model.SubStatus),
                new OracleParameter(string.Format(":{0}","AcceptType"), model.AcceptType),
                new OracleParameter(string.Format(":{0}","CustomerOrder"), model.CustomerOrder),
                new OracleParameter(string.Format(":{0}","OriginDepotNo"), model.OriginDepotNo),
                new OracleParameter(string.Format(":{0}","PeriodAccountCode"), model.PeriodAccountCode),
                new OracleParameter(string.Format(":{0}","WaybillCategory"), model.WaybillCategory),
			    new OracleParameter(string.Format(":{0}","createtime"), Convert.ToDateTime(model.createtime).ToString("yyyy-MM-dd HH:mm:ss")),
			    new OracleParameter(string.Format(":{0}","updatetime"), Convert.ToDateTime(model.updatetime).ToString("yyyy-MM-dd HH:mm:ss")),
                new OracleParameter(string.Format(":{0}","IsChange"), 1),
                new OracleParameter(string.Format(":{0}","DeliverCode"), model.DeliverCode)
            };

			int count = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (count == 0) throw new Exception("插入失败!");

            return (int)model.IncomeID;
		}
		
		
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(FMS_IncomeBaseInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append(string.Format("update {0} set ",TableName));
										            
			strSql.Append(" WaybillType = :WaybillType ,	 ");
										            
			strSql.Append(" MerchantID = :MerchantID ,	 ");
										            
			strSql.Append(" ExpressCompanyID = :ExpressCompanyID ,	 ");
										            
			strSql.Append(" FinalExpressCompanyID = :FinalExpressCompanyID ,	 ");
										            
			strSql.Append(" DeliverStationID = :DeliverStationID ,	 ");
										            
			strSql.Append(" TopCODCompanyID = :TopCODCompanyID ,	 ");
										            
			strSql.Append(" RfdAcceptTime = :RfdAcceptTime ,	 ");
										            
			strSql.Append(" DeliverTime = :DeliverTime ,	 ");
										            
			strSql.Append(" ReturnTime = :ReturnTime ,	 ");
										            
			strSql.Append(" ReturnExpressCompanyID = :ReturnExpressCompanyID ,	 ");
										            
			strSql.Append(" BackStationTime = :BackStationTime ,	 ");
										            
			strSql.Append(" BackStationStatus = :BackStationStatus ,	 ");
										            
			strSql.Append(" ProtectedAmount = :ProtectedAmount ,	 ");
										            
			strSql.Append(" TotalAmount = :TotalAmount ,	 ");
										            
			strSql.Append(" PaidAmount = :PaidAmount ,	 ");
										            
			strSql.Append(" NeedPayAmount = :NeedPayAmount ,	 ");
										            
			strSql.Append(" BackAmount = :BackAmount ,	 ");
										            
			strSql.Append(" NeedBackAmount = :NeedBackAmount ,	 ");
										            
			strSql.Append(" AccountWeight = :AccountWeight ,	 ");
										            
			strSql.Append(" AreaID = :AreaID ,	 ");
										            
			strSql.Append(" ReceiveAddress = :ReceiveAddress ,	 ");
										            
			strSql.Append(" SignType = :SignType ,	 ");

            strSql.Append(" CustomerOrder = :CustomerOrder ,	 ");

            strSql.Append(" OriginDepotNo = :OriginDepotNo ,	 ");

            strSql.Append(" InefficacyStatus = :InefficacyStatus ,	 ");
										            
			strSql.Append(" updatetime = sysdate,  ");

            strSql.Append(" IsChange = :IsChange  ");

            strSql.Append(string.Format(" where {0} = :{0}", "WaybillNo"));

		    OracleParameter[] parameters = {
		                                    new OracleParameter(string.Format(":{0}", "WaybillNo"), model.WaybillNo),
		                                    new OracleParameter(string.Format(":{0}", "WaybillType"), model.WaybillType),
		                                    new OracleParameter(string.Format(":{0}", "MerchantID"), model.MerchantID),
		                                    new OracleParameter(string.Format(":{0}", "ExpressCompanyID"), model.ExpressCompanyID),
		                                    new OracleParameter(string.Format(":{0}", "FinalExpressCompanyID"),model.FinalExpressCompanyID),
		                                    new OracleParameter(string.Format(":{0}", "DeliverStationID"), model.DeliverStationID),
		                                    new OracleParameter(string.Format(":{0}", "TopCODCompanyID"), model.TopCODCompanyID),
		                                    new OracleParameter(string.Format(":{0}", "RfdAcceptTime"), model.RfdAcceptTime),
		                                    new OracleParameter(string.Format(":{0}", "DeliverTime"), model.DeliverTime),
		                                    new OracleParameter(string.Format(":{0}", "ReturnTime"), model.ReturnTime),
		                                    new OracleParameter(string.Format(":{0}", "ReturnExpressCompanyID"),model.ReturnExpressCompanyID),
		                                    new OracleParameter(string.Format(":{0}", "BackStationTime"), model.BackStationTime),
		                                    new OracleParameter(string.Format(":{0}", "BackStationStatus"),model.BackStationStatus),
		                                    new OracleParameter(string.Format(":{0}", "ProtectedAmount"), model.ProtectedAmount),
		                                    new OracleParameter(string.Format(":{0}", "TotalAmount"), model.TotalAmount),
		                                    new OracleParameter(string.Format(":{0}", "PaidAmount"), model.PaidAmount),
		                                    new OracleParameter(string.Format(":{0}", "NeedPayAmount"), model.NeedPayAmount),
		                                    new OracleParameter(string.Format(":{0}", "BackAmount"), model.BackAmount),
		                                    new OracleParameter(string.Format(":{0}", "NeedBackAmount"), model.NeedBackAmount),
		                                    new OracleParameter(string.Format(":{0}", "AccountWeight"), model.AccountWeight),
		                                    new OracleParameter(string.Format(":{0}", "AreaID"), model.AreaID),
		                                    new OracleParameter(string.Format(":{0}", "ReceiveAddress"), model.ReceiveAddress),
		                                    new OracleParameter(string.Format(":{0}", "SignType"), model.SignType),
                                            new OracleParameter(string.Format(":{0}", "CustomerOrder"), model.CustomerOrder),
                                            new OracleParameter(string.Format(":{0}", "OriginDepotNo"), model.OriginDepotNo),
		                                    new OracleParameter(string.Format(":{0}", "InefficacyStatus"), model.InefficacyStatus), 
                                            new OracleParameter(string.Format(":{0}", "IsChange"), 1)
		                                };

			int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

			if (rows > 0)
			{
				return true;
			}

		    return false;
		}

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateStatus(FMS_IncomeBaseInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("update {0} set ", TableName));

            //strSql.Append(" WaybillNo = :WaybillNo ,	 ");

            //strSql.Append(" WaybillType = :WaybillType ,	 ");

            //strSql.Append(" MerchantID = :MerchantID ,	 ");

            //strSql.Append(" ExpressCompanyID = :ExpressCompanyID ,	 ");

            strSql.Append(" FinalExpressCompanyID = :FinalExpressCompanyID ,	 ");

            strSql.Append(" DeliverStationID = :DeliverStationID ,	 ");

            strSql.Append(" TopCODCompanyID = :TopCODCompanyID ,	 ");

            //strSql.Append(" RfdAcceptTime = :RfdAcceptTime ,	 ");

            strSql.Append(" DeliverTime = :DeliverTime ,	 ");

            //strSql.Append(" ReturnTime = :ReturnTime ,	 ");

            //strSql.Append(" ReturnExpressCompanyID = :ReturnExpressCompanyID ,	 ");

            strSql.Append(" BackStationTime = :BackStationTime ,	 ");

            strSql.Append(" BackStationStatus = :BackStationStatus ,	 ");

            //strSql.Append(" ProtectedAmount = :ProtectedAmount ,	 ");

            //strSql.Append(" TotalAmount = :TotalAmount ,	 ");

            //strSql.Append(" PaidAmount = :PaidAmount ,	 ");

            //strSql.Append(" NeedPayAmount = :NeedPayAmount ,	 ");

            //strSql.Append(" BackAmount = :BackAmount ,	 ");

            //strSql.Append(" NeedBackAmount = :NeedBackAmount ,	 ");

            strSql.Append(" AccountWeight = :AccountWeight ,	 ");

            //strSql.Append(" AreaID = :AreaID ,	 ");

            //strSql.Append(" ReceiveAddress = :ReceiveAddress ,	 ");

            strSql.Append(" SignType = :SignType ,	 ");

            //strSql.Append(" InefficacyStatus = :InefficacyStatus ,	 ");

            strSql.Append(" AcceptType = :AcceptType,  ");

            strSql.Append(" PeriodAccountCode = :PeriodAccountCode,  ");

            strSql.Append(" updatetime = sysdate,  ");

            strSql.Append(" IsChange = :IsChange  ");

            strSql.Append(string.Format(" where {0} = :{0}", "WaybillNo"));
            OracleParameter[] parameters = {
		                                    //new OracleParameter(string.Format(":{0}", "IncomeID"), model.IncomeID),
		                                    new OracleParameter(string.Format(":{0}", "WaybillNo"), model.WaybillNo),
		                                    //new OracleParameter(string.Format(":{0}", "WaybillType"), model.WaybillType),
		                                    //new OracleParameter(string.Format(":{0}", "MerchantID"), model.MerchantID),
		                                    //new OracleParameter(string.Format(":{0}", "ExpressCompanyID"), model.ExpressCompanyID),
		                                    new OracleParameter(string.Format(":{0}", "FinalExpressCompanyID"),model.FinalExpressCompanyID),
                                            new OracleParameter(string.Format(":{0}", "DeliverStationID"), model.DeliverStationID),
                                            new OracleParameter(string.Format(":{0}", "TopCODCompanyID"), model.TopCODCompanyID),
                                            //new OracleParameter(string.Format(":{0}", "RfdAcceptTime"), model.RfdAcceptTime),
		                                    new OracleParameter(string.Format(":{0}", "DeliverTime"), model.DeliverTime),
                                            //new OracleParameter(string.Format(":{0}", "ReturnTime"), model.ReturnTime),
                                            //new OracleParameter(string.Format(":{0}", "ReturnExpressCompanyID"),model.ReturnExpressCompanyID),
		                                    new OracleParameter(string.Format(":{0}", "BackStationTime"), model.BackStationTime),
		                                    new OracleParameter(string.Format(":{0}", "BackStationStatus"),model.BackStationStatus),
                                            ////new OracleParameter(string.Format(":{0}", "ProtectedAmount"), model.ProtectedAmount),
                                            ////new OracleParameter(string.Format(":{0}", "TotalAmount"), model.TotalAmount),
                                            ////new OracleParameter(string.Format(":{0}", "PaidAmount"), model.PaidAmount),
                                            ////new OracleParameter(string.Format(":{0}", "NeedPayAmount"), model.NeedPayAmount),
                                            ////new OracleParameter(string.Format(":{0}", "BackAmount"), model.BackAmount),
                                            ////new OracleParameter(string.Format(":{0}", "NeedBackAmount"), model.NeedBackAmount),
                                            new OracleParameter(string.Format(":{0}", "AccountWeight"), model.AccountWeight),
                                            ////new OracleParameter(string.Format(":{0}", "AreaID"), model.AreaID),
                                            ////new OracleParameter(string.Format(":{0}", "ReceiveAddress"), model.ReceiveAddress),
		                                    new OracleParameter(string.Format(":{0}", "SignType"), model.SignType),
                                            //new OracleParameter(string.Format(":{0}", "InefficacyStatus"), model.InefficacyStatus), 
                                            new OracleParameter(string.Format(":{0}", "AcceptType"), model.AcceptType),
                                            new OracleParameter(string.Format(":{0}", "PeriodAccountCode"), model.PeriodAccountCode),
		                                    //new OracleParameter(string.Format(":{0}", "updatetime"), model.updatetime),
                                            new OracleParameter(string.Format(":{0}", "IsChange"), 1)
		                                };

            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rows > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateBackStatus(FMS_IncomeBaseInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("update {0} set ", TableName));

            //strSql.Append(" WaybillNo = :WaybillNo ,	 ");
            //strSql.Append(" WaybillType = :WaybillType ,	 ");
            //strSql.Append(" MerchantID = :MerchantID ,	 ");
            //strSql.Append(" ExpressCompanyID = :ExpressCompanyID ,	 ");
            //strSql.Append(" FinalExpressCompanyID = :FinalExpressCompanyID ,	 ");
            //strSql.Append(" DeliverStationID = :DeliverStationID ,	 ");
            //strSql.Append(" TopCODCompanyID = :TopCODCompanyID ,	 ");
            //strSql.Append(" RfdAcceptTime = :RfdAcceptTime ,	 ");
            //strSql.Append(" DeliverTime = :DeliverTime ,	 ");
            strSql.Append(" ReturnTime = :ReturnTime ,	 ");
            strSql.Append(" ReturnExpressCompanyID = :ReturnExpressCompanyID ,	 ");
            //strSql.Append(" BackStationTime = :BackStationTime ,	 ");
            //strSql.Append(" BackStationStatus = :BackStationStatus ,	 ");
            //strSql.Append(" ProtectedAmount = :ProtectedAmount ,	 ");
            //strSql.Append(" TotalAmount = :TotalAmount ,	 ");
            //strSql.Append(" PaidAmount = :PaidAmount ,	 ");
            //strSql.Append(" NeedPayAmount = :NeedPayAmount ,	 ");
            //strSql.Append(" BackAmount = :BackAmount ,	 ");
            //strSql.Append(" NeedBackAmount = :NeedBackAmount ,	 ");
            strSql.Append(" AccountWeight = :AccountWeight ,	 ");
            //strSql.Append(" AreaID = :AreaID ,	 ");
            //strSql.Append(" ReceiveAddress = :ReceiveAddress ,	 ");
            strSql.Append(" SubStatus = :SubStatus,");
            strSql.Append(" AcceptType = :AcceptType,");
            strSql.Append(" updatetime = sysdate,");
            strSql.Append(" IsChange = :IsChange");
            strSql.Append(string.Format(" where {0} = :{0}", "WaybillNo"));
            OracleParameter[] parameters = {
		                                    //new OracleParameter(string.Format(":{0}", "IncomeID"), model.IncomeID),
		                                    new OracleParameter(string.Format(":{0}", "WaybillNo"), model.WaybillNo),
		                                    //new OracleParameter(string.Format(":{0}", "WaybillType"), model.WaybillType),
		                                    //new OracleParameter(string.Format(":{0}", "MerchantID"), model.MerchantID),
		                                    //new OracleParameter(string.Format(":{0}", "ExpressCompanyID"), model.ExpressCompanyID),
		                                    //new OracleParameter(string.Format(":{0}", "FinalExpressCompanyID"),model.FinalExpressCompanyID),
                                            //new OracleParameter(string.Format(":{0}", "DeliverStationID"), model.DeliverStationID),
                                            //new OracleParameter(string.Format(":{0}", "TopCODCompanyID"), model.TopCODCompanyID),
                                            //new OracleParameter(string.Format(":{0}", "RfdAcceptTime"), model.RfdAcceptTime),
		                                    //new OracleParameter(string.Format(":{0}", "DeliverTime"), model.DeliverTime),
                                            new OracleParameter(string.Format(":{0}", "ReturnTime"), model.ReturnTime),
                                            new OracleParameter(string.Format(":{0}", "ReturnExpressCompanyID"),model.ReturnExpressCompanyID),
                                            //new OracleParameter(string.Format(":{0}", "BackStationTime"), model.BackStationTime),
                                            //new OracleParameter(string.Format(":{0}", "BackStationStatus"),model.BackStationStatus),
                                            ////new OracleParameter(string.Format(":{0}", "ProtectedAmount"), model.ProtectedAmount),
                                            ////new OracleParameter(string.Format(":{0}", "TotalAmount"), model.TotalAmount),
                                            ////new OracleParameter(string.Format(":{0}", "PaidAmount"), model.PaidAmount),
                                            ////new OracleParameter(string.Format(":{0}", "NeedPayAmount"), model.NeedPayAmount),
                                            ////new OracleParameter(string.Format(":{0}", "BackAmount"), model.BackAmount),
                                            ////new OracleParameter(string.Format(":{0}", "NeedBackAmount"), model.NeedBackAmount),
                                            new OracleParameter(string.Format(":{0}", "AccountWeight"), model.AccountWeight),
                                            ////new OracleParameter(string.Format(":{0}", "AreaID"), model.AreaID),
                                            ////new OracleParameter(string.Format(":{0}", "ReceiveAddress"), model.ReceiveAddress),
		                                    //new OracleParameter(string.Format(":{0}", "SignType"), model.SignType),
                                            //new OracleParameter(string.Format(":{0}", "InefficacyStatus"), model.InefficacyStatus), 
		                                    //new OracleParameter(string.Format(":{0}", "updatetime"), model.updatetime),
                                            new OracleParameter(string.Format(":{0}","SubStatus"), model.SubStatus),
                                            new OracleParameter(string.Format(":{0}","AcceptType"), model.AcceptType),
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
        public FMS_IncomeBaseInfo GetModel(DataRow row)
		{
		    var model = new FMS_IncomeBaseInfo();
		    if (row["IncomeID"].ToString() != "")
		    {
		        model.IncomeID = Int64.Parse(row["IncomeID"].ToString());
		    }
		    if (row["WaybillNo"].ToString() != "")
		    {
		        model.WaybillNo = Int64.Parse(row["WaybillNo"].ToString());
		    }
		    model.WaybillType = row["WaybillType"].ToString();
		    if (row["MerchantID"].ToString() != "")
		    {
		        model.MerchantID = Int32.Parse(row["MerchantID"].ToString());
		    }
		    if (row["ExpressCompanyID"].ToString() != "")
		    {
		        model.ExpressCompanyID = Int32.Parse(row["ExpressCompanyID"].ToString());
		    }
		    if (row["FinalExpressCompanyID"].ToString() != "")
		    {
		        model.FinalExpressCompanyID = Int32.Parse(row["FinalExpressCompanyID"].ToString());
		    }
		    if (row["DeliverStationID"].ToString() != "")
		    {
		        model.DeliverStationID = Int32.Parse(row["DeliverStationID"].ToString());
		    }
		    if (row["TopCODCompanyID"].ToString() != "")
		    {
		        model.TopCODCompanyID = Int32.Parse(row["TopCODCompanyID"].ToString());
		    }
		    if (row["RfdAcceptTime"].ToString() != "")
		    {
		        model.RfdAcceptTime = System.DateTime.Parse(row["RfdAcceptTime"].ToString());
		    }
		    if (row["DeliverTime"].ToString() != "")
		    {
		        model.DeliverTime = System.DateTime.Parse(row["DeliverTime"].ToString());
		    }
		    if (row["ReturnTime"].ToString() != "")
		    {
		        model.ReturnTime = System.DateTime.Parse(row["ReturnTime"].ToString());
		    }
		    if (row["ReturnExpressCompanyID"].ToString() != "")
		    {
		        model.ReturnExpressCompanyID = Int32.Parse(row["ReturnExpressCompanyID"].ToString());
		    }
		    if (row["BackStationTime"].ToString() != "")
		    {
		        model.BackStationTime = System.DateTime.Parse(row["BackStationTime"].ToString());
		    }
		    model.BackStationStatus = row["BackStationStatus"].ToString();
		    if (row["ProtectedAmount"].ToString() != "")
		    {
		        model.ProtectedAmount = System.Decimal.Parse(row["ProtectedAmount"].ToString());
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
		    model.ReceiveAddress = row["ReceiveAddress"].ToString();
		    if (row["SignType"].ToString() != "")
		    {
		        model.SignType = Int32.Parse(row["SignType"].ToString());
		    }
		    if (row["InefficacyStatus"].ToString() != "")
		    {
		        model.InefficacyStatus = Int32.Parse(row["InefficacyStatus"].ToString());
		    }
		    if (row["createtime"].ToString() != "")
		    {
		        model.createtime = System.DateTime.Parse(row["createtime"].ToString());
		    }
		    if (row["updatetime"].ToString() != "")
		    {
		        model.updatetime = System.DateTime.Parse(row["updatetime"].ToString());
		    }

		    return model;
		}

	    /// <summary>
		/// 得到一个对象实体
		/// </summary>
		public FMS_IncomeBaseInfo GetModel(Int64 incomeid)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append(@"select IncomeID, WaybillNo, WaybillType, MerchantID, ExpressCompanyID, FinalExpressCompanyID, DeliverStationID, TopCODCompanyID, RfdAcceptTime, DeliverTime, ReturnTime, ReturnExpressCompanyID, BackStationTime, BackStationStatus, ProtectedAmount, 
TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, ReceiveAddress, SignType, InefficacyStatus, createtime, updatetime  ");			
			strSql.Append(string.Format("  from {0} ",TableName));
			strSql.Append(string.Format(" where {0} = :{0}","IncomeID"));
			var sqlParams = new List<OracleParameter>()
											{
												new OracleParameter(string.Format(":{0}","IncomeID"),incomeid)
											};
			var model=new FMS_IncomeBaseInfo();
			DataSet ds= OracleHelper.ExecuteDataset(Connection,  CommandType.Text,strSql.ToString(),sqlParams.ToArray());
			if(ds.Tables[0].Rows.Count>0)
			{
				model = GetModel(ds.Tables[0].Rows[0]);
			}
			return model;
		}

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FMS_IncomeBaseInfo GetModelByWaybillNO(Int64 waybillNo)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select IncomeID, WaybillNo, WaybillType, MerchantID, ExpressCompanyID, FinalExpressCompanyID, DeliverStationID, TopCODCompanyID, RfdAcceptTime, DeliverTime, ReturnTime, ReturnExpressCompanyID, BackStationTime, BackStationStatus, ProtectedAmount, 
TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, ReceiveAddress, SignType, InefficacyStatus, createtime, updatetime  ");
            strSql.Append(string.Format("  from {0} ", TableName));
            strSql.Append(string.Format(" where {0} = :{0}", "WaybillNO"));
            var sqlParams = new List<OracleParameter>()
											{
												new OracleParameter(string.Format(":{0}","WaybillNO"),OracleDbType.Int64){Value = waybillNo}
											};
            var model = new FMS_IncomeBaseInfo();
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
		public List<FMS_IncomeBaseInfo> GetModelList(Dictionary<string, object> searchParams)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append(@"select IncomeID, WaybillNo, WaybillType, MerchantID, ExpressCompanyID, FinalExpressCompanyID, DeliverStationID, TopCODCompanyID, RfdAcceptTime, 
            DeliverTime, ReturnTime, ReturnExpressCompanyID, BackStationTime, BackStationStatus, ProtectedAmount, TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, ReceiveAddress, SignType, InefficacyStatus, createtime, updatetime  ");			
			strSql.Append(string.Format("  from {0} ",TableName));
			strSql.Append(" where 1 = 1 ");
			var sqlParams = new List<OracleParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>
						{
							strSql.Append(string.Format(" and {0} = :{0}",item.Key));
							sqlParams.Add(new OracleParameter(string.Format(":{0}",item.Key), item.Value));
						});
			}
			var modelList=new List<FMS_IncomeBaseInfo>();
			DataSet ds= OracleHelper.ExecuteDataset(Connection,  CommandType.Text,strSql.ToString(),sqlParams.ToArray());
			if(ds.Tables[0].Rows.Count>0)
			{
				foreach(DataRow row in ds.Tables[0].Rows)
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
		public int GetDataTableCount(string searchString,Dictionary<string, object> searchParams)
		{
			var sqlStr = string.Format(@"SELECT COUNT(*) FROM {0} {1}",TableName,searchString);
			var sqlParams = new List<OracleParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>sqlParams.Add(new OracleParameter(string.Format(":{0}",item.Key), item.Value)));
			}
			var obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, sqlParams.ToArray());
			int i = 0;
			if(obj!=null)
			{
				int.TryParse(obj.ToString(),out i);
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
			strSql.Append(@"select IncomeID, WaybillNo, WaybillType, MerchantID, ExpressCompanyID, FinalExpressCompanyID, DeliverStationID, TopCODCompanyID, 
RfdAcceptTime, DeliverTime, ReturnTime, ReturnExpressCompanyID, BackStationTime, BackStationStatus, ProtectedAmount, TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, ReceiveAddress, SignType, InefficacyStatus, createtime, updatetime  ");			
			strSql.Append(string.Format("  from {0} ",TableName));
			strSql.Append(" where 1 = 1 ");
			var sqlParams = new List<OracleParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>
						{
							strSql.Append(string.Format(" and {0} = :{0}",item.Key));
							sqlParams.Add(new OracleParameter(string.Format(":{0}",item.Key), item.Value));
						});
			}
			return OracleHelper.ExecuteDataset(Connection,  CommandType.Text,strSql.ToString(),sqlParams.ToArray()).Tables[0];
		}
		
		/// <summary>
        /// 根据指定条件指定排序获取结果集
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortColumn"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
		public DataTable GetDataTable(string searchString,string sortColumn,Dictionary<string, object> searchParams)
		{
			var sqlStr = string.Format(@"SELECT * FROM {0} {1} ORDER BY {2} DESC",TableName,searchString,sortColumn);
			var sqlParams = new List<OracleParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>sqlParams.Add(new OracleParameter(string.Format(":{0}",item.Key), item.Value)));
			}
			return OracleHelper.ExecuteDataset(Connection,  CommandType.Text,sqlStr,sqlParams.ToArray()).Tables[0];
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
		public DataTable GetPageDataTable(string searchString,string sortColumn,Dictionary<string, object> searchParams, int rowStart, int rowEnd)
		{
			var sqlStr = string.Format(@"SELECT * FROM {0} {1}", TableName, searchString);

			sqlStr = string.Format(PagingTemplate, sortColumn, sqlStr, rowStart, rowEnd);
			var sqlParams = new List<OracleParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>sqlParams.Add(new OracleParameter(string.Format(":{0}",item.Key), item.Value)));
			}
		   return OracleHelper.ExecuteDataset(Connection,  CommandType.Text,sqlStr,sqlParams.ToArray()).Tables[0];
		}

        /// <summary>
        /// 通过运单号查询收入结算所需信息 add by wangyongc 2012-04-12
        /// </summary>
        /// <param name="waybillNO">运单号</param>
        /// <returns></returns>
        public DataTable GetWaybillInfoByNO(long waybillNO)
        {
            //天猫的单号独立取，规则：对个订单对一个运单，只取其1，多个运单对一个订单，取重复
            string strSql =
                @"SELECT w.WaybillNO,
                    CASE WHEN w.MerchantID=4596 THEN (SELECT TOP 1 OrderNO FROM OrderThirdPartyRelation WHERE WaybillNO=w.WaybillNO) ELSE w.CustomerOrder END CustomerOrder,
                    w.WaybillType,w.WarehouseId,w.[Status],w.MerchantID,w.CreatStation,ob.OutBoundStation AS FinalExpressCompanyID,w.DeliverStationID,w.CreatTime,w.DeliverTime,
                    w.ReturnTime,w.ReturnWareHouse,w.ReturnExpressCompanyId,wbs.CreatTime AS BackStationTime,wbs.SignStatus AS BackStationStatus,wsi.ProtectedPrice AS ProtectedAmount,
                    CASE 
                        WHEN w.MerchantID IN (8, 9) THEN (
                                NVL(Amount, 0) + NVL(additionalprice, 0) + 
                                NVL(TransferFee, 0)
                            )
                        WHEN (
                                NVL(Amount, 0) + NVL(additionalprice, 0) + 
                                NVL(TransferFee, 0)
                            ) >=NVL(wsi.NeedAmount,0)+NVL(wsi.PaidAmount,0) THEN (
                                NVL(Amount, 0) + NVL(additionalprice, 0) + 
                                NVL(TransferFee, 0)
                            )
                        ELSE NVL(wsi.NeedAmount, 0) + NVL(wsi.PaidAmount, 0)
                    END AS TotalAmount,wsi.PaidAmount ,wsi.NeedAmount AS NeedPayAmount,wsi.FactBackAmount AS BackAmount,wsi.NeedBackAmount AS NeedBackAmount,
                    CASE fmdf.WeightType
                        WHEN 0 THEN NVL(wi.MerchantWeight, 0)
                        WHEN 1 THEN NVL(wi.WayBillInfoWeight, 0)
                        WHEN 2 THEN CASE 
                                        WHEN NVL(wi.MerchantWeight, 0) > NVL(wi.WayBillInfoVolumeWeight, 0) THEN 
                                            NVL(wi.MerchantWeight, 0)
                                        ELSE NVL(wi.WayBillInfoVolumeWeight, 0)
                                    END
			            WHEN 3 THEN NVL(wi.MerchantWeight, 0)
			            WHEN 4 THEN 0
                        ELSE 0
                    END AS AccountWeight,
                    pca.AreaID,
                    (case wtsi.ReceiveProvince WHEN '北京' THEN '' WHEN '天津' THEN '' WHEN '上海' THEN '' WHEN '重庆' THEN '' else wtsi.ReceiveProvince end)+wtsi.ReceiveCity+wtsi.ReceiveArea+wtsi.ReceiveAddress as ReceiveAddress,
                    wsi.SignType,w.InefficacyStatus,w.ReceiveStationID,w.ReceiveDeliverManID,wsi.TransferPayType,wsi.DeputizeAmount,w.DistributionCode,w.CurrentDistributionCode,wsi.TransferFee,wsi.AcceptType,
                    (SELECT ec.TopCODCompanyID FROM ExpressCompany ec JOIN ExpressCompany ec1 ON ec.TopCODCompanyID=ec1.ExpressCompanyID WHERE ec.IsDeleted=0 AND ec.ExpressCompanyID=w.DeliverStationID) AS TopCODCompanyID,
                    w.BackStatus,wi.WayBillInfoWeight,w.CustomerOrder,wee.OriginDepotNo,
                    wsi.PeriodAccountCode

                    FROM Waybill w 
                    INNER JOIN WaybillSignInfo wsi ON w.WaybillNO=wsi.WaybillNO
                    LEFT JOIN WaybillBackStation wbs ON wsi.BackStationInofID=wbs.WaybillBackStationID
                    INNER JOIN WaybillInfo wi ON w.WaybillNO=wi.WaybillNO
                    INNER JOIN WaybillTakeSendInfo wtsi ON w.WaybillNO=wtsi.WaybillNO
                    LEFT JOIN OutBound ob ON w.OutBoundID=ob.OutBoundID
                    left join WaybillExpressExtend wee on w.WaybillNO=wee.WaybillNo
                    LEFT JOIN FMS_MerchantDeliverFee fmdf ON  fmdf.MerchantID = w.MerchantID AND fmdf.[Status]=2 AND fmdf.DistributionCode=w.DistributionCode
                    LEFT JOIN (
					            SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a2.AreaID,a2.AreaName 
					            FROM Province AS p 
                                JOIN City AS c ON p.ProvinceID = c.ProvinceID
					            JOIN Area AS a2 ON c.CityID = a2.CityID
				              ) pca ON pca.ProvinceName=wtsi.ReceiveProvince AND pca.CityName=wtsi.ReceiveCity AND pca.AreaName=wtsi.ReceiveArea
                    WHERE w.WaybillNO=:WaybillNO";

            OracleParameter[] parameters = { new OracleParameter(":WaybillNO",  waybillNO) };

            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 更新金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateAmount(FMS_IncomeBaseInfo info)
        {
            string @sqlStr = @"UPDATE FMS_IncomeBaseInfo SET NeedPayAmount = :NeedPayAmount,NeedBackAmount = :NeedBackAmount,updatetime = SysDate,IsChange=:IsChange WHERE IncomeID=:IncomeID ";

            OracleParameter[] parameters ={
                                           new OracleParameter(":IncomeID",OracleDbType.Int64),
                                           new OracleParameter(":NeedPayAmount",OracleDbType.Decimal),
                                           new OracleParameter(":NeedBackAmount",OracleDbType.Decimal),
                                           new OracleParameter(":IsChange",OracleDbType.Decimal)
                                      };
            parameters[0].Value = info.IncomeID;
            parameters[1].Value = info.NeedPayAmount;
            parameters[2].Value = info.NeedBackAmount;
            parameters[3].Value = 1;

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        /// <summary>
        /// 更新无效状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateInefficacyStatus(Int64 waybillNo, int inefficacyStatus)
        {
            string @sqlStr = @"UPDATE FMS_IncomeBaseInfo SET InefficacyStatus = :InefficacyStatus,IsChange=:IsChange WHERE WaybillNo=:WaybillNo ";

            OracleParameter[] parameters ={
                                           new OracleParameter(":WaybillNo",OracleDbType.Int64),
                                           new OracleParameter(":InefficacyStatus",OracleDbType.Decimal),
                                           new OracleParameter(":IsChange",OracleDbType.Int16)
                                      };
            parameters[0].Value = waybillNo;
            parameters[1].Value = inefficacyStatus;
            parameters[2].Value = true;

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }


        public DataTable GetIncomeDailyReport(string beginTime, string endTime, string merchantIds, string discributionCode)
        {
            string merchantCondition = "";

            List<OracleParameter> parameters = new List<OracleParameter>();
            if (merchantIds.Trim().Length != 0)
            { 
               merchantCondition=string.Format(" AND MerchantID IN ({0})", merchantIds.Replace(" ", ""));
            }

            string sql = string.Format(@"
WITH t AS (
--普通单妥投，所有费用全算
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AccountFare,
    SUM(case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END) ProtectedFee,
    SUM(case when POSReceiveFee IS NULL THEN 0 ELSE POSReceiveFee END
        +case when ReceiveFee IS NULL THEN 0 ELSE ReceiveFee END) ReceiveFee,
    SUM(case when POSReceiveServiceFee IS NULL THEN 0 ELSE POSReceiveServiceFee END 
        + case when CashReceiveServiceFee IS NULL THEN 0 ELSE CashReceiveServiceFee END) ServiceFee,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END
        + case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END
        + case when POSReceiveFee IS NULL THEN 0 ELSE POSReceiveFee END
        + case when ReceiveFee IS NULL THEN 0 ELSE ReceiveFee END
        + case when POSReceiveServiceFee IS NULL THEN 0 ELSE POSReceiveServiceFee END
        + case when CashReceiveServiceFee IS NULL THEN 0 ELSE CashReceiveServiceFee END) AllFee
from FMS_IncomeBaseInfo bInfo
inner join FMS_IncomeFeeInfo fInfo on bInfo.WaybillNo=fInfo.WaybillNO
inner join PS_PMS.MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='3' and bInfo.createtime > trunc(sysdate-90)  and bInfo.WaybillType='0'
  and MerchantID NOT IN (8,9)
  {0}
  and BackStationTime >= :beginTime
  and BackStationTime < :endTime
  and bInfo.DistributionCode=:DistributionCode
  group by MerchantName

UNION ALL
-- 签单返回妥投，全部计算
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AccountFare,
    SUM(case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END) ProtectedFee,
    SUM(case when POSReceiveFee IS NULL THEN 0 ELSE POSReceiveFee END
        +case when ReceiveFee IS NULL THEN 0 ELSE ReceiveFee END) ReceiveFee,
    SUM(case when POSReceiveServiceFee IS NULL THEN 0 ELSE POSReceiveServiceFee END 
        + case when CashReceiveServiceFee IS NULL THEN 0 ELSE CashReceiveServiceFee END) ServiceFee,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END
        + case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END
        + case when POSReceiveFee IS NULL THEN 0 ELSE POSReceiveFee END
        + case when ReceiveFee IS NULL THEN 0 ELSE ReceiveFee END
        + case when POSReceiveServiceFee IS NULL THEN 0 ELSE POSReceiveServiceFee END
        + case when CashReceiveServiceFee IS NULL THEN 0 ELSE CashReceiveServiceFee END) AllFee
from  FMS_IncomeBaseInfo bInfo
inner join FMS_IncomeFeeInfo fInfo  on bInfo.WaybillNo=fInfo.WaybillNO
inner join PS_PMS.MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='3' and bInfo.createtime > trunc(sysdate-90) and bInfo.WaybillType='3'
  and MerchantID NOT IN (8,9)
  {0}
  and BackStationTime >= :beginTime
  and BackStationTime < :endTime
  and bInfo.DistributionCode=:DistributionCode
  group by MerchantName

UNION ALL
--换货单妥投入库，所有费用全算
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AccountFare,
    SUM(case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END) ProtectedFee,
    SUM(case when POSReceiveFee IS NULL THEN 0 ELSE POSReceiveFee END
        +case when ReceiveFee IS NULL THEN 0 ELSE ReceiveFee END) ReceiveFee,
    SUM(case when POSReceiveServiceFee IS NULL THEN 0 ELSE POSReceiveServiceFee END 
        + case when CashReceiveServiceFee IS NULL THEN 0 ELSE CashReceiveServiceFee END) ServiceFee,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END
        + case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END
        + case when POSReceiveFee IS NULL THEN 0 ELSE POSReceiveFee END
        + case when ReceiveFee IS NULL THEN 0 ELSE ReceiveFee END
        + case when POSReceiveServiceFee IS NULL THEN 0 ELSE POSReceiveServiceFee END
        + case when CashReceiveServiceFee IS NULL THEN 0 ELSE CashReceiveServiceFee END) AllFee
from FMS_IncomeBaseInfo bInfo
inner join FMS_IncomeFeeInfo fInfo on bInfo.WaybillNo=fInfo.WaybillNO
inner join PS_PMS.MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='3' and bInfo.createtime > trunc(sysdate-90) and bInfo.WaybillType='1'
  and MerchantID NOT IN (8,9)
  {0}
  and ReturnTime >= :beginTime
  and ReturnTime < :endTime
  and bInfo.DistributionCode=:DistributionCode
  group by MerchantName
UNION ALL
--上门退货单退货入库，AccountFare、ProtectedFee
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AccountFare,
    SUM(case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END) ProtectedFee,
    0 ReceiveFee,
    0 ServiceFee,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END + case when ProtectedFee IS NULL THEN 0 ELSE ProtectedFee END) AllFee
from FMS_IncomeBaseInfo bInfo
inner join FMS_IncomeFeeInfo fInfo on bInfo.WaybillNo=fInfo.WaybillNO
inner join PS_PMS.MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='3' and bInfo.createtime > trunc(sysdate-90) and bInfo.WaybillType='2'
  and MerchantID NOT IN (8,9)
  {0}
  and ReturnTime >= :beginTime
  and ReturnTime < :endTime
  and bInfo.DistributionCode=:DistributionCode
  group by MerchantName
UNION ALL
--普通单、换货单拒收入库，AccountFare
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AccountFare,
    0 ProtectedFee,
    0 ReceiveFee,
    0 ServiceFee,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AllFee
from FMS_IncomeBaseInfo bInfo
inner join FMS_IncomeFeeInfo fInfo on bInfo.WaybillNo=fInfo.WaybillNO
inner join PS_PMS.MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='5' and bInfo.createtime > trunc(sysdate-90) and bInfo.WaybillType IN ('0','1','3')
  and MerchantID NOT IN (8,9)
  {0}
  and ReturnTime >= :beginTime
  and ReturnTime < :endTime
  and bInfo.DistributionCode=:DistributionCode
  group by MerchantName
UNION ALL
--上门退单拒收，AccountFare
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AccountFare,
    0 ProtectedFee,
    0 ReceiveFee,
    0 ServiceFee,
    SUM(case when AccountFare IS NULL THEN 0 ELSE AccountFare END) AllFee
from FMS_IncomeBaseInfo bInfo
inner join FMS_IncomeFeeInfo fInfo on bInfo.WaybillNo=fInfo.WaybillNO
inner join PS_PMS.MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='5' and bInfo.createtime > trunc(sysdate-90) and bInfo.WaybillType IN ('2')
  and MerchantID NOT IN (8,9)
  {0}
  and BackStationTime >= :beginTime
  and BackStationTime < :endTime
  and bInfo.DistributionCode=:DistributionCode
  group by MerchantName
  )
select 
	MerchantName 商家,
	sum(CountNum) 结算单量,
	SUM(AccountFare) 配送费,
	SUM(ProtectedFee) 保价费,
	SUM(ReceiveFee) 手续费,
	SUM(ServiceFee) 服务费,
	SUM(AllFee) 应收合计,
	cast(SUM(AllFee)/SUM(CountNum) as numeric(18,2)) 单均收入
from t group by MerchantName", merchantCondition);


            parameters.Add(new OracleParameter(":beginTime", OracleDbType.Date)
                               {Value = DataConvert.ToDateTime(beginTime)});
            parameters.Add(new OracleParameter(":endTime", OracleDbType.Date) {Value = DataConvert.ToDateTime(endTime)});
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) {Value = discributionCode});
                
            

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters.ToArray()).Tables[0];
        }

        public DataTable GetIncomeDailyReportSum(string beginTime, string endTime,string merchantIds, string discributionCode)
        {
            string sql = @"with result as  
                (
                select 
	                case when AccountFare is null then 0 else AccountFare end AccountFare,
	                case when ProtectedFee is null then 0 else ProtectedFee end ProtectedFee,
	                SUM(case when POSReceiveFee is null then 0 else POSReceiveFee end) POSReceiveFee,
	                SUM(case when ReceiveFee is null then 0 else ReceiveFee end) ReceiveFee,
	                SUM(case when POSReceiveServiceFee is null then 0 else POSReceiveServiceFee end) POSReceiveServiceFee,
                    SUM(case when CashReceiveServiceFee is null then 0 else CashReceiveServiceFee end) CashReceiveServiceFee
                from FMS_IncomeBaseInfo bInfo
                inner join FMS_IncomeFeeInfo fInfo on bInfo.WaybillNo=fInfo.WaybillNO
                inner join MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
                where bInfo.BackStationStatus=3 and bInfo.WaybillType ='0'
                  and MerchantID!=8
                  and MerchantID!=9
                  and BackStationTime >= :beginTime
                  and BackStationTime <= :endTime
                  and bInfo.DistributionCode=:DistributionCode
                union all
                select 
	                case when AccountFare is null then 0 else AccountFare end AccountFare,
	                case when ProtectedFee is null then 0 else ProtectedFee end ProtectedFee,
	                SUM(case when POSReceiveFee is null then 0 else POSReceiveFee end) POSReceiveFee,
	                SUM(case when ReceiveFee is null then 0 else ReceiveFee end) ReceiveFee,
	                SUM(case when POSReceiveServiceFee is null then 0 else POSReceiveServiceFee end) POSReceiveServiceFee,
                    SUM(case when CashReceiveServiceFee is null then 0 else CashReceiveServiceFee end) CashReceiveServiceFee
                from FMS_IncomeBaseInfo bInfo
                inner join FMS_IncomeFeeInfo fInfo on bInfo.WaybillNo=fInfo.WaybillNO
                inner join MerchantBaseInfo mInfo on bInfo.MerchantID=mInfo.ID
                where bInfo.BackStationStatus=3 and bInfo.WaybillType in ('1','2')
                  and MerchantID!=8
                  and MerchantID!=9
                  and ReturnTime >= :beginTime 
                  and ReturnTime <= :endTime
                  and bInfo.DistributionCode=:DistributionCode
                )
                select 
	                count(1) WaybillCount,
	                SUM(AccountFare) AccountFare,
	                SUM(ProtectedFee) ProtectedFee,
	                SUM(POSReceiveFee + ReceiveFee) ReceiveFee,
	                SUM(POSReceiveServiceFee+CashReceiveServiceFee) POSReceiveServiceFee,
	                SUM(AccountFare + ProtectedFee + POSReceiveFee + ReceiveFee+POSReceiveServiceFee+CashReceiveServiceFee) SumFee,
	                SUM(AccountFare + ProtectedFee + POSReceiveFee + ReceiveFee+POSReceiveServiceFee+CashReceiveServiceFee) / count(1) AvgFee
                from result 
                ";

            OracleParameter[] parameters =
            {
                new OracleParameter(":beginTime",OracleDbType.Date){ Value=DataConvert.ToDateTime(beginTime)},
                new OracleParameter(":endTime",OracleDbType.Date){ Value=DataConvert.ToDateTime(endTime)},
                new OracleParameter(":DistributionCode",OracleDbType.Varchar2){ Value=discributionCode}
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetDeliverFeeParameter(long waybillNo, string distributionCode)
        {
            string sql = @"select baseInfo.WaybillNo,
	            baseInfo.AccountWeight Weight,
	            areaIncome.AreaType Area,
	            feeInfo.AccountStandard Formula,
	            feeInfo.AccountFare DeliverFee
            from FMS_IncomeBaseInfo baseInfo
            inner join FMS_IncomeFeeInfo feeInfo on baseInfo.WaybillNo=feeInfo.WaybillNO
            inner join AreaExpressLevelIncome areaIncome on baseInfo.AreaID=areaIncome.AreaID 
	            and baseInfo.MerchantID=areaIncome.MerchantID 
	            and baseInfo.ExpressCompanyID=areaIncome.WareHouseID
            where baseInfo.WaybillNO=:WaybillNO and baseInfo.DistributionCode=:DistributionCode 
            order by feeInfo.CreateTime desc";

           OracleParameter[] parameters =
            {
                new OracleParameter(":WaybillNO",OracleDbType.Int64){ Value=waybillNo },
                new OracleParameter(":DistributionCode",OracleDbType.Varchar2){ Value=distributionCode }
            };

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public bool SaveDeliverFee(DeliverFeeModel model)
        {
            string sql = "update FMS_IncomeBaseInfo set AccountWeight=:AccountWeight,IsChange=1 where WaybillNo=:WaybillNO";

            OracleParameter[] parameters =
            {
                new OracleParameter(":AccountWeight",OracleDbType.Decimal){ Value = model.Weight },
                new OracleParameter(":WaybillNO",OracleDbType.Int64){ Value = model.WaybillNO }
            };

            if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) != 1) return false;

            string sql1 = @"update FMS_IncomeFeeInfo 
	            set AccountStandard=:AccountStandard,AreaType=:AreaType,AccountFare=:AccountFare,IsChange=1,IsAccount=1
	            where WaybillNo=:WaybillNO";

            OracleParameter[] parameters1 =
            {
                new OracleParameter(":AccountStandard",OracleDbType.Varchar2){ Value = model.Formula },
                new OracleParameter(":AreaType",OracleDbType.Int32){ Value = model.Area },
                new OracleParameter(":AccountFare",OracleDbType.Decimal){ Value = model.DeliverFee },
                new OracleParameter(":WaybillNO",OracleDbType.Int64){ Value = model.WaybillNO }
            };

            if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql1, parameters1) != 1) return false;

            return true;
        }


        public bool UpdateEvalStatus(long waybillNo)
        {
            string sql = @"update FMS_IncomeFeeInfo set IsAccount=0 where WaybillNO=:WaybillNO";

            OracleParameter[] parameters =
            {
                new OracleParameter(":WaybillNO",OracleDbType.Int64){ Value = waybillNo }
            };

            if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) != 1) return false;

            return true;
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            string sql = String.Format(@"update FMS_IncomeFeeInfo set IsAccount=0 where WaybillNO in ({0})", waybillNos);

            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }

        public DataTable GetInfoByMerchant(int count, string merchantIDs)
        {
            string sql =
                string.Format(
                    @"select * from (
                     SELECT  f1.WaybillNo
                      FROM    fms_incomebaseinfo f1
                      WHERE   f1.createtime >to_date('2012-10-31 23:59:59','yyyy-mm-dd hh24:mi:ss')
                       AND f1.merchantid      IN ({0})
                       AND f1.waybillcategory IS NULL
                        ORDER BY CreateTime DESC ) where rownum<=:count
                      ",
                    merchantIDs);
            OracleParameter[] parameter = {
                                              new OracleParameter(":count", OracleDbType.Int32) {Value = count}
                                          };
            var ds =OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameter);
            return ds.Tables[0];
        }

        public bool UpdateCategoryInfo(long waybillNo,string waybillCategory)
        {
            string sql =
                @" Update fms_incomebaseinfo set  WaybillCategory = :WaybillCategory 
                            where WaybillNo = :WaybillNo";
            OracleParameter[] parameters = {
                                               new OracleParameter(":WaybillCategory", OracleDbType.Varchar2)
                                                   {Value = waybillCategory},
                                               new OracleParameter(":WaybillNo", OracleDbType.Int64) {Value = waybillNo}
                                           };
            int ret = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql,parameters);
            return ret > 0;
        }
	}
}
