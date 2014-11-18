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
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.DAL.FinancialManage
{
    public partial class IncomeBaseInfoDao : SqlServerDao, IIncomeBaseInfoDao
	{

        private const string TableName = @"LMS_RFD.dbo.FMS_IncomeBaseInfo";
		
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
            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNo"));
			var sqlParams = new List<SqlParameter>()
											{
												new SqlParameter(string.Format("@{0}","WaybillNo"),incomeid)
											};
			return Convert.ToInt64(SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), sqlParams.ToArray())) > 0;
		}
		
		
		
		
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(FMS_IncomeBaseInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append(string.Format("insert into {0}(",TableName));			
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
															strSql.Append(" @WaybillNo , ");
												strSql.Append(" @WaybillType , ");
												strSql.Append(" @MerchantID , ");
												strSql.Append(" @ExpressCompanyID , ");
												strSql.Append(" @FinalExpressCompanyID , ");
												strSql.Append(" @DeliverStationID , ");
												strSql.Append(" @TopCODCompanyID , ");
												strSql.Append(" @RfdAcceptTime , ");
												strSql.Append(" @DeliverTime , ");
												strSql.Append(" @ReturnTime , ");
												strSql.Append(" @ReturnExpressCompanyID , ");
												strSql.Append(" @BackStationTime , ");
												strSql.Append(" @BackStationStatus , ");
												strSql.Append(" @ProtectedAmount , ");
												strSql.Append(" @TotalAmount , ");
												strSql.Append(" @PaidAmount , ");
												strSql.Append(" @NeedPayAmount , ");
												strSql.Append(" @BackAmount , ");
												strSql.Append(" @NeedBackAmount , ");
												strSql.Append(" @AccountWeight , ");
												strSql.Append(" @AreaID , ");
												strSql.Append(" @ReceiveAddress , ");
												strSql.Append(" @SignType , ");
												strSql.Append(" @InefficacyStatus , ");

                                                strSql.Append(" @ReceiveStationID , ");
                                                strSql.Append(" @ReceiveDeliverManID , ");
                                                strSql.Append(" @DistributionCode , ");
                                                strSql.Append(" @CurrentDistributionCode , ");

                                                strSql.Append(" @WayBillInfoWeight , ");
                                                strSql.Append(" @SubStatus , ");
                                                strSql.Append(" @AcceptType , ");
                                                strSql.Append(" @CustomerOrder , ");
                                                strSql.Append(" @OriginDepotNo , ");
                                                strSql.Append(" @PeriodAccountCode , ");
                                                strSql.Append(" @WaybillCategory , ");
												strSql.Append(" @createtime , ");
												strSql.Append(" @updatetime,  ");
                                                strSql.Append(" @IsChange,  ");
                                                strSql.Append(" @DeliverCode  ");
									strSql.Append(") ");
			strSql.Append(";select @@IDENTITY");
			 SqlParameter[] parameters = {
											new SqlParameter(string.Format("@{0}","WaybillNo"), model.WaybillNo),
											new SqlParameter(string.Format("@{0}","WaybillType"), model.WaybillType),
											new SqlParameter(string.Format("@{0}","MerchantID"), model.MerchantID),
											new SqlParameter(string.Format("@{0}","ExpressCompanyID"), model.ExpressCompanyID),
											new SqlParameter(string.Format("@{0}","FinalExpressCompanyID"), model.FinalExpressCompanyID),
											new SqlParameter(string.Format("@{0}","DeliverStationID"), model.DeliverStationID),
											new SqlParameter(string.Format("@{0}","TopCODCompanyID"), model.TopCODCompanyID),
											new SqlParameter(string.Format("@{0}","RfdAcceptTime"), model.RfdAcceptTime),
											new SqlParameter(string.Format("@{0}","DeliverTime"), model.DeliverTime),
											new SqlParameter(string.Format("@{0}","ReturnTime"), model.ReturnTime),
											new SqlParameter(string.Format("@{0}","ReturnExpressCompanyID"), model.ReturnExpressCompanyID),
											new SqlParameter(string.Format("@{0}","BackStationTime"), model.BackStationTime),
											new SqlParameter(string.Format("@{0}","BackStationStatus"), model.BackStationStatus),
											new SqlParameter(string.Format("@{0}","ProtectedAmount"), model.ProtectedAmount),
											new SqlParameter(string.Format("@{0}","TotalAmount"), model.TotalAmount),
											new SqlParameter(string.Format("@{0}","PaidAmount"), model.PaidAmount),
											new SqlParameter(string.Format("@{0}","NeedPayAmount"), model.NeedPayAmount),
											new SqlParameter(string.Format("@{0}","BackAmount"), model.BackAmount),
											new SqlParameter(string.Format("@{0}","NeedBackAmount"), model.NeedBackAmount),
											new SqlParameter(string.Format("@{0}","AccountWeight"), model.AccountWeight),
											new SqlParameter(string.Format("@{0}","AreaID"), model.AreaID),
											new SqlParameter(string.Format("@{0}","ReceiveAddress"), model.ReceiveAddress),
											new SqlParameter(string.Format("@{0}","SignType"), model.SignType),
											new SqlParameter(string.Format("@{0}","InefficacyStatus"), model.InefficacyStatus),
											new SqlParameter(string.Format("@{0}","ReceiveStationID"), model.ReceiveStationID),
											new SqlParameter(string.Format("@{0}","ReceiveDeliverManID"), model.ReceiveDeliverManID),
											new SqlParameter(string.Format("@{0}","DistributionCode"), model.DistributionCode),
											new SqlParameter(string.Format("@{0}","CurrentDistributionCode"), model.CurrentDistributionCode),

                                            new SqlParameter(string.Format("@{0}","WayBillInfoWeight"), model.WayBillInfoWeight),
											new SqlParameter(string.Format("@{0}","SubStatus"), model.SubStatus),
                                            new SqlParameter(string.Format("@{0}","AcceptType"), model.AcceptType),
                                            new SqlParameter(string.Format("@{0}","CustomerOrder"), model.CustomerOrder),
                                            new SqlParameter(string.Format("@{0}","OriginDepotNo"), model.OriginDepotNo),
                                            new SqlParameter(string.Format("@{0}","PeriodAccountCode"), model.PeriodAccountCode),
                                            new SqlParameter(string.Format("@{0}","WaybillCategory"), model.WaybillCategory),
											new SqlParameter(string.Format("@{0}","createtime"), model.createtime),
											new SqlParameter(string.Format("@{0}","updatetime"), model.updatetime),
                                            new SqlParameter(string.Format("@{0}","IsChange"), true),
                                            new SqlParameter(string.Format("@{0}","DeliverCode"), model.DeliverCode)
                                         };

			object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(), parameters);

			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32(obj);
			}		
		}
		
		
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(FMS_IncomeBaseInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append(string.Format("update {0} set ",TableName));
														            
			//strSql.Append(" WaybillNo = @WaybillNo ,	 ");
										            
			strSql.Append(" WaybillType = @WaybillType ,	 ");
										            
			strSql.Append(" MerchantID = @MerchantID ,	 ");
										            
			strSql.Append(" ExpressCompanyID = @ExpressCompanyID ,	 ");
										            
			strSql.Append(" FinalExpressCompanyID = @FinalExpressCompanyID ,	 ");
										            
			strSql.Append(" DeliverStationID = @DeliverStationID ,	 ");
										            
			strSql.Append(" TopCODCompanyID = @TopCODCompanyID ,	 ");
										            
			strSql.Append(" RfdAcceptTime = @RfdAcceptTime ,	 ");
										            
			strSql.Append(" DeliverTime = @DeliverTime ,	 ");
										            
			strSql.Append(" ReturnTime = @ReturnTime ,	 ");
										            
			strSql.Append(" ReturnExpressCompanyID = @ReturnExpressCompanyID ,	 ");
										            
			strSql.Append(" BackStationTime = @BackStationTime ,	 ");
										            
			strSql.Append(" BackStationStatus = @BackStationStatus ,	 ");
										            
			strSql.Append(" ProtectedAmount = @ProtectedAmount ,	 ");
										            
			strSql.Append(" TotalAmount = @TotalAmount ,	 ");
										            
			strSql.Append(" PaidAmount = @PaidAmount ,	 ");
										            
			strSql.Append(" NeedPayAmount = @NeedPayAmount ,	 ");
										            
			strSql.Append(" BackAmount = @BackAmount ,	 ");
										            
			strSql.Append(" NeedBackAmount = @NeedBackAmount ,	 ");
										            
			strSql.Append(" AccountWeight = @AccountWeight ,	 ");
										            
			strSql.Append(" AreaID = @AreaID ,	 ");
										            
			strSql.Append(" ReceiveAddress = @ReceiveAddress ,	 ");
										            
			strSql.Append(" SignType = @SignType ,	 ");

            strSql.Append(" CustomerOrder = @CustomerOrder ,	 ");

            strSql.Append(" OriginDepotNo = @OriginDepotNo ,	 ");

            strSql.Append(" InefficacyStatus = @InefficacyStatus ,	 ");

            strSql.Append(" WaybillCategory = @WaybillCategory ,	 ");
										            
			//strSql.Append(" createtime = @createtime ,	 ");
										            
			strSql.Append(" updatetime = @updatetime,  ");

            strSql.Append(" IsChange = @IsChange  ");

            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNo"));

		    SqlParameter[] parameters = {
		                                    new SqlParameter(string.Format("@{0}", "IncomeID"), model.IncomeID),
		                                    new SqlParameter(string.Format("@{0}", "WaybillNo"), model.WaybillNo),
		                                    new SqlParameter(string.Format("@{0}", "WaybillType"), model.WaybillType),
		                                    new SqlParameter(string.Format("@{0}", "MerchantID"), model.MerchantID),
		                                    new SqlParameter(string.Format("@{0}", "ExpressCompanyID"), model.ExpressCompanyID),
		                                    new SqlParameter(string.Format("@{0}", "FinalExpressCompanyID"),model.FinalExpressCompanyID),
		                                    new SqlParameter(string.Format("@{0}", "DeliverStationID"), model.DeliverStationID),
		                                    new SqlParameter(string.Format("@{0}", "TopCODCompanyID"), model.TopCODCompanyID),
		                                    new SqlParameter(string.Format("@{0}", "RfdAcceptTime"), model.RfdAcceptTime),
		                                    new SqlParameter(string.Format("@{0}", "DeliverTime"), model.DeliverTime),
		                                    new SqlParameter(string.Format("@{0}", "ReturnTime"), model.ReturnTime),
		                                    new SqlParameter(string.Format("@{0}", "ReturnExpressCompanyID"),model.ReturnExpressCompanyID),
		                                    new SqlParameter(string.Format("@{0}", "BackStationTime"), model.BackStationTime),
		                                    new SqlParameter(string.Format("@{0}", "BackStationStatus"),model.BackStationStatus),
		                                    new SqlParameter(string.Format("@{0}", "ProtectedAmount"), model.ProtectedAmount),
		                                    new SqlParameter(string.Format("@{0}", "TotalAmount"), model.TotalAmount),
		                                    new SqlParameter(string.Format("@{0}", "PaidAmount"), model.PaidAmount),
		                                    new SqlParameter(string.Format("@{0}", "NeedPayAmount"), model.NeedPayAmount),
		                                    new SqlParameter(string.Format("@{0}", "BackAmount"), model.BackAmount),
		                                    new SqlParameter(string.Format("@{0}", "NeedBackAmount"), model.NeedBackAmount),
		                                    new SqlParameter(string.Format("@{0}", "AccountWeight"), model.AccountWeight),
		                                    new SqlParameter(string.Format("@{0}", "AreaID"), model.AreaID),
		                                    new SqlParameter(string.Format("@{0}", "ReceiveAddress"), model.ReceiveAddress),
		                                    new SqlParameter(string.Format("@{0}", "SignType"), model.SignType),
                                            new SqlParameter(string.Format("@{0}", "CustomerOrder"), model.CustomerOrder),
                                            new SqlParameter(string.Format("@{0}", "OriginDepotNo"), model.OriginDepotNo),
		                                    new SqlParameter(string.Format("@{0}", "InefficacyStatus"), model.InefficacyStatus), 
		                                    new SqlParameter(string.Format("@{0}", "WaybillCategory"), model.WaybillCategory), 
                                            //new SqlParameter(string.Format("@{0}", "createtime"), model.createtime),
		                                    new SqlParameter(string.Format("@{0}", "updatetime"), model.updatetime),
                                            new SqlParameter(string.Format("@{0}", "IsChange"), true)
		                                };

			int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

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

            //strSql.Append(" WaybillNo = @WaybillNo ,	 ");

            //strSql.Append(" WaybillType = @WaybillType ,	 ");

            //strSql.Append(" MerchantID = @MerchantID ,	 ");

            //strSql.Append(" ExpressCompanyID = @ExpressCompanyID ,	 ");

            strSql.Append(" FinalExpressCompanyID = @FinalExpressCompanyID ,	 ");

            strSql.Append(" DeliverStationID = @DeliverStationID ,	 ");

            strSql.Append(" TopCODCompanyID = @TopCODCompanyID ,	 ");

            //strSql.Append(" RfdAcceptTime = @RfdAcceptTime ,	 ");

            strSql.Append(" DeliverTime = @DeliverTime ,	 ");

            //strSql.Append(" ReturnTime = @ReturnTime ,	 ");

            //strSql.Append(" ReturnExpressCompanyID = @ReturnExpressCompanyID ,	 ");

            strSql.Append(" BackStationTime = @BackStationTime ,	 ");

            strSql.Append(" BackStationStatus = @BackStationStatus ,	 ");

            //strSql.Append(" ProtectedAmount = @ProtectedAmount ,	 ");

            //strSql.Append(" TotalAmount = @TotalAmount ,	 ");

            //strSql.Append(" PaidAmount = @PaidAmount ,	 ");

            //strSql.Append(" NeedPayAmount = @NeedPayAmount ,	 ");

            //strSql.Append(" BackAmount = @BackAmount ,	 ");

            //strSql.Append(" NeedBackAmount = @NeedBackAmount ,	 ");

            strSql.Append(" AccountWeight = @AccountWeight ,	 ");

            //strSql.Append(" AreaID = @AreaID ,	 ");

            //strSql.Append(" ReceiveAddress = @ReceiveAddress ,	 ");

            strSql.Append(" SignType = @SignType ,	 ");

            //strSql.Append(" InefficacyStatus = @InefficacyStatus ,	 ");

            strSql.Append(" AcceptType = @AcceptType,  ");

            strSql.Append(" PeriodAccountCode = @PeriodAccountCode,  ");

            strSql.Append(" updatetime = @updatetime,  ");

            strSql.Append(" IsChange = @IsChange  ");

            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNo"));
            SqlParameter[] parameters = {
		                                    //new SqlParameter(string.Format("@{0}", "IncomeID"), model.IncomeID),
		                                    new SqlParameter(string.Format("@{0}", "WaybillNo"), model.WaybillNo),
		                                    //new SqlParameter(string.Format("@{0}", "WaybillType"), model.WaybillType),
		                                    //new SqlParameter(string.Format("@{0}", "MerchantID"), model.MerchantID),
		                                    //new SqlParameter(string.Format("@{0}", "ExpressCompanyID"), model.ExpressCompanyID),
		                                    new SqlParameter(string.Format("@{0}", "FinalExpressCompanyID"),model.FinalExpressCompanyID),
                                            new SqlParameter(string.Format("@{0}", "DeliverStationID"), model.DeliverStationID),
                                            new SqlParameter(string.Format("@{0}", "TopCODCompanyID"), model.TopCODCompanyID),
                                            //new SqlParameter(string.Format("@{0}", "RfdAcceptTime"), model.RfdAcceptTime),
		                                    new SqlParameter(string.Format("@{0}", "DeliverTime"), model.DeliverTime),
                                            //new SqlParameter(string.Format("@{0}", "ReturnTime"), model.ReturnTime),
                                            //new SqlParameter(string.Format("@{0}", "ReturnExpressCompanyID"),model.ReturnExpressCompanyID),
		                                    new SqlParameter(string.Format("@{0}", "BackStationTime"), model.BackStationTime),
		                                    new SqlParameter(string.Format("@{0}", "BackStationStatus"),model.BackStationStatus),
                                            ////new SqlParameter(string.Format("@{0}", "ProtectedAmount"), model.ProtectedAmount),
                                            ////new SqlParameter(string.Format("@{0}", "TotalAmount"), model.TotalAmount),
                                            ////new SqlParameter(string.Format("@{0}", "PaidAmount"), model.PaidAmount),
                                            ////new SqlParameter(string.Format("@{0}", "NeedPayAmount"), model.NeedPayAmount),
                                            ////new SqlParameter(string.Format("@{0}", "BackAmount"), model.BackAmount),
                                            ////new SqlParameter(string.Format("@{0}", "NeedBackAmount"), model.NeedBackAmount),
                                            new SqlParameter(string.Format("@{0}", "AccountWeight"), model.AccountWeight),
                                            ////new SqlParameter(string.Format("@{0}", "AreaID"), model.AreaID),
                                            ////new SqlParameter(string.Format("@{0}", "ReceiveAddress"), model.ReceiveAddress),
		                                    new SqlParameter(string.Format("@{0}", "SignType"), model.SignType),
                                            //new SqlParameter(string.Format("@{0}", "InefficacyStatus"), model.InefficacyStatus), 
                                            new SqlParameter(string.Format("@{0}", "AcceptType"), model.AcceptType),
                                            new SqlParameter(string.Format("@{0}", "PeriodAccountCode"), model.PeriodAccountCode),
		                                    new SqlParameter(string.Format("@{0}", "updatetime"), model.updatetime),
                                            new SqlParameter(string.Format("@{0}", "IsChange"), true)
		                                };

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rows > 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateBackStation(FMS_IncomeBaseInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("update {0} set ", TableName));

            strSql.Append(" BackStationTime = null ,	 ");

            strSql.Append(" BackStationStatus ='' ,	 ");

            strSql.Append(" updatetime = @updatetime,  ");
            strSql.Append(" IsChange = @IsChange  ");

            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNo"));

            SqlParameter[] parameters = {
		                                    
		                                    new SqlParameter(string.Format("@{0}", "WaybillNo"), model.WaybillNo),
		                                    new SqlParameter(string.Format("@{0}", "BackStationTime"), model.BackStationTime),
		                                    new SqlParameter(string.Format("@{0}", "BackStationStatus"),model.BackStationStatus),
		                                    new SqlParameter(string.Format("@{0}", "updatetime"), model.updatetime),
                                            new SqlParameter(string.Format("@{0}", "IsChange"), true)
		                                };

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

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

            //strSql.Append(" WaybillNo = @WaybillNo ,	 ");
            //strSql.Append(" WaybillType = @WaybillType ,	 ");
            //strSql.Append(" MerchantID = @MerchantID ,	 ");
            //strSql.Append(" ExpressCompanyID = @ExpressCompanyID ,	 ");
            //strSql.Append(" FinalExpressCompanyID = @FinalExpressCompanyID ,	 ");
            //strSql.Append(" DeliverStationID = @DeliverStationID ,	 ");
            //strSql.Append(" TopCODCompanyID = @TopCODCompanyID ,	 ");
            //strSql.Append(" RfdAcceptTime = @RfdAcceptTime ,	 ");
            //strSql.Append(" DeliverTime = @DeliverTime ,	 ");
            strSql.Append(" ReturnTime = @ReturnTime ,	 ");
            strSql.Append(" ReturnExpressCompanyID = @ReturnExpressCompanyID ,	 ");
            //strSql.Append(" BackStationTime = @BackStationTime ,	 ");
            //strSql.Append(" BackStationStatus = @BackStationStatus ,	 ");
            //strSql.Append(" ProtectedAmount = @ProtectedAmount ,	 ");
            //strSql.Append(" TotalAmount = @TotalAmount ,	 ");
            //strSql.Append(" PaidAmount = @PaidAmount ,	 ");
            //strSql.Append(" NeedPayAmount = @NeedPayAmount ,	 ");
            //strSql.Append(" BackAmount = @BackAmount ,	 ");
            //strSql.Append(" NeedBackAmount = @NeedBackAmount ,	 ");
            strSql.Append(" AccountWeight = @AccountWeight ,	 ");
            //strSql.Append(" AreaID = @AreaID ,	 ");
            //strSql.Append(" ReceiveAddress = @ReceiveAddress ,	 ");
            strSql.Append(" SubStatus = @SubStatus,");
            strSql.Append(" AcceptType = @AcceptType,");
            strSql.Append(" updatetime = @updatetime,");
            strSql.Append(" IsChange = @IsChange");
            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNo"));
            SqlParameter[] parameters = {
		                                    //new SqlParameter(string.Format("@{0}", "IncomeID"), model.IncomeID),
		                                    new SqlParameter(string.Format("@{0}", "WaybillNo"), model.WaybillNo),
		                                    //new SqlParameter(string.Format("@{0}", "WaybillType"), model.WaybillType),
		                                    //new SqlParameter(string.Format("@{0}", "MerchantID"), model.MerchantID),
		                                    //new SqlParameter(string.Format("@{0}", "ExpressCompanyID"), model.ExpressCompanyID),
		                                    //new SqlParameter(string.Format("@{0}", "FinalExpressCompanyID"),model.FinalExpressCompanyID),
                                            //new SqlParameter(string.Format("@{0}", "DeliverStationID"), model.DeliverStationID),
                                            //new SqlParameter(string.Format("@{0}", "TopCODCompanyID"), model.TopCODCompanyID),
                                            //new SqlParameter(string.Format("@{0}", "RfdAcceptTime"), model.RfdAcceptTime),
		                                    //new SqlParameter(string.Format("@{0}", "DeliverTime"), model.DeliverTime),
                                            new SqlParameter(string.Format("@{0}", "ReturnTime"), model.ReturnTime),
                                            new SqlParameter(string.Format("@{0}", "ReturnExpressCompanyID"),model.ReturnExpressCompanyID),
                                            //new SqlParameter(string.Format("@{0}", "BackStationTime"), model.BackStationTime),
                                            //new SqlParameter(string.Format("@{0}", "BackStationStatus"),model.BackStationStatus),
                                            ////new SqlParameter(string.Format("@{0}", "ProtectedAmount"), model.ProtectedAmount),
                                            ////new SqlParameter(string.Format("@{0}", "TotalAmount"), model.TotalAmount),
                                            ////new SqlParameter(string.Format("@{0}", "PaidAmount"), model.PaidAmount),
                                            ////new SqlParameter(string.Format("@{0}", "NeedPayAmount"), model.NeedPayAmount),
                                            ////new SqlParameter(string.Format("@{0}", "BackAmount"), model.BackAmount),
                                            ////new SqlParameter(string.Format("@{0}", "NeedBackAmount"), model.NeedBackAmount),
                                            new SqlParameter(string.Format("@{0}", "AccountWeight"), model.AccountWeight),
                                            ////new SqlParameter(string.Format("@{0}", "AreaID"), model.AreaID),
                                            ////new SqlParameter(string.Format("@{0}", "ReceiveAddress"), model.ReceiveAddress),
		                                    //new SqlParameter(string.Format("@{0}", "SignType"), model.SignType),
                                            //new SqlParameter(string.Format("@{0}", "InefficacyStatus"), model.InefficacyStatus), 
		                                    new SqlParameter(string.Format("@{0}", "updatetime"), model.updatetime),
                                            new SqlParameter(string.Format("@{0}","SubStatus"), model.SubStatus),
                                            new SqlParameter(string.Format("@{0}","AcceptType"), model.AcceptType),
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
			strSql.Append(string.Format(" where {0} = @{0}","IncomeID"));
			var sqlParams = new List<SqlParameter>()
											{
												new SqlParameter(string.Format("@{0}","IncomeID"),incomeid)
											};
			var model=new FMS_IncomeBaseInfo();
			DataSet ds= SqlHelper.ExecuteDataset(Connection,  CommandType.Text,strSql.ToString(),sqlParams.ToArray());
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
            strSql.Append(string.Format(" where {0} = @{0}", "WaybillNO"));
            var sqlParams = new List<SqlParameter>()
											{
												new SqlParameter(string.Format("@{0}","WaybillNO"),waybillNo)
											};
            var model = new FMS_IncomeBaseInfo();
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
		public List<FMS_IncomeBaseInfo> GetModelList(Dictionary<string, object> searchParams)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append(@"select IncomeID, WaybillNo, WaybillType, MerchantID, ExpressCompanyID, FinalExpressCompanyID, DeliverStationID, TopCODCompanyID, RfdAcceptTime, 
            DeliverTime, ReturnTime, ReturnExpressCompanyID, BackStationTime, BackStationStatus, ProtectedAmount, TotalAmount, PaidAmount, NeedPayAmount, BackAmount, NeedBackAmount, AccountWeight, AreaID, ReceiveAddress, SignType, InefficacyStatus, createtime, updatetime  ");			
			strSql.Append(string.Format("  from {0} ",TableName));
			strSql.Append(" where 1 = 1 ");
			var sqlParams = new List<SqlParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>
						{
							strSql.Append(string.Format(" and {0} = @{0}",item.Key));
							sqlParams.Add(new SqlParameter(string.Format("@{0}",item.Key), item.Value));
						});
			}
			var modelList=new List<FMS_IncomeBaseInfo>();
			DataSet ds= SqlHelper.ExecuteDataset(Connection,  CommandType.Text,strSql.ToString(),sqlParams.ToArray());
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
			var sqlParams = new List<SqlParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>sqlParams.Add(new SqlParameter(string.Format("@{0}",item.Key), item.Value)));
			}
			var obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, sqlParams.ToArray());
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
			var sqlParams = new List<SqlParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>
						{
							strSql.Append(string.Format(" and {0} = @{0}",item.Key));
							sqlParams.Add(new SqlParameter(string.Format("@{0}",item.Key), item.Value));
						});
			}
			return SqlHelper.ExecuteDataset(Connection,  CommandType.Text,strSql.ToString(),sqlParams.ToArray()).Tables[0];
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
			var sqlParams = new List<SqlParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>sqlParams.Add(new SqlParameter(string.Format("@{0}",item.Key), item.Value)));
			}
			return SqlHelper.ExecuteDataset(Connection,  CommandType.Text,sqlStr,sqlParams.ToArray()).Tables[0];
		}
		
		/// <summary>
        /// 根据指定条件指定排序获取结果集带分页
        /// </summary>
		public DataTable GetPageDataTable(string searchString,string sortColumn,Dictionary<string, object> searchParams, int rowStart, int rowEnd)
		{
			var sqlStr = string.Format(@"SELECT * FROM {0} {1}", TableName, searchString);

			sqlStr = string.Format(PagingTemplate, sortColumn, sqlStr, rowStart, rowEnd);
			var sqlParams = new List<SqlParameter>();
			if (searchParams != null)
			{
				searchParams.ToList().ForEach(item =>sqlParams.Add(new SqlParameter(string.Format("@{0}",item.Key), item.Value)));
			}
		   return SqlHelper.ExecuteDataset(Connection,  CommandType.Text,sqlStr,sqlParams.ToArray()).Tables[0];
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
                    CASE WHEN w.MerchantID=4596 THEN (SELECT TOP 1 OrderNO FROM LMS_RFD.dbo.OrderThirdPartyRelation WHERE WaybillNO=w.WaybillNO) ELSE w.CustomerOrder END CustomerOrder,
                    w.WaybillType,w.WarehouseId,w.[Status],w.MerchantID,w.CreatStation,ob.OutBoundStation AS FinalExpressCompanyID,w.DeliverStationID,w.CreatTime,w.DeliverTime,
                    w.ReturnTime,w.ReturnWareHouse,w.ReturnExpressCompanyId,wbs.CreatTime AS BackStationTime,wbs.SignStatus AS BackStationStatus,wsi.ProtectedPrice AS ProtectedAmount,
                    CASE 
                        WHEN w.MerchantID IN (8, 9) THEN (
                                ISNULL(Amount, 0) + ISNULL(additionalprice, 0) + 
                                ISNULL(TransferFee, 0)
                            )
                        WHEN (
                                ISNULL(Amount, 0) + ISNULL(additionalprice, 0) + 
                                ISNULL(TransferFee, 0)
                            ) >=isnull(wsi.NeedAmount,0)+isnull(wsi.PaidAmount,0) THEN (
                                ISNULL(Amount, 0) + ISNULL(additionalprice, 0) + 
                                ISNULL(TransferFee, 0)
                            )
                        ELSE ISNULL(wsi.NeedAmount, 0) + ISNULL(wsi.PaidAmount, 0)
                    END AS TotalAmount,wsi.PaidAmount ,wsi.NeedAmount AS NeedPayAmount,wsi.FactBackAmount AS BackAmount,wsi.NeedBackAmount AS NeedBackAmount,
                    CASE fmdf.WeightType
                        WHEN 0 THEN ISNULL(wi.MerchantWeight, 0)
                        WHEN 1 THEN ISNULL(wi.WayBillInfoWeight, 0)
                        WHEN 2 THEN CASE 
                                        WHEN ISNULL(wi.MerchantWeight, 0) > ISNULL(wi.WayBillInfoVolumeWeight, 0) THEN 
                                            ISNULL(wi.MerchantWeight, 0)
                                        ELSE ISNULL(wi.WayBillInfoVolumeWeight, 0)
                                    END
			            WHEN 3 THEN ISNULL(wi.MerchantWeight, 0)
			            WHEN 4 THEN 0
                        ELSE 0
                    END AS AccountWeight,
                    pca.AreaID,
                    (case wtsi.ReceiveProvince WHEN '北京' THEN '' WHEN '天津' THEN '' WHEN '上海' THEN '' WHEN '重庆' THEN '' else wtsi.ReceiveProvince end)+wtsi.ReceiveCity+wtsi.ReceiveArea+wtsi.ReceiveAddress as ReceiveAddress,
                    wsi.SignType,w.InefficacyStatus,w.ReceiveStationID,w.ReceiveDeliverManID,wsi.TransferPayType,wsi.DeputizeAmount,w.DistributionCode,w.CurrentDistributionCode,wsi.TransferFee,wsi.AcceptType,
                    (SELECT ec.TopCODCompanyID FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) JOIN RFD_PMS.dbo.ExpressCompany ec1(NOLOCK) ON ec.TopCODCompanyID=ec1.ExpressCompanyID WHERE ec.IsDeleted=0 AND ec.ExpressCompanyID=w.DeliverStationID) AS TopCODCompanyID,
                    w.BackStatus,wi.WayBillInfoWeight,w.CustomerOrder,wee.OriginDepotNo,
                    wsi.PeriodAccountCode

                    FROM LMS_RFD.dbo.Waybill w(NOLOCK) 
                    INNER JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON w.WaybillNO=wsi.WaybillNO
                    LEFT JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON wsi.BackStationInofID=wbs.WaybillBackStationID
                    INNER JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON w.WaybillNO=wi.WaybillNO
                    INNER JOIN LMS_RFD.dbo.WaybillTakeSendInfo wtsi(NOLOCK) ON w.WaybillNO=wtsi.WaybillNO
                    LEFT JOIN LMS_RFD.dbo.OutBound ob(NOLOCK) ON w.OutBoundID=ob.OutBoundID
                    left join LMS_RFD.dbo.WaybillExpressExtend wee(nolock) on w.WaybillNO=wee.WaybillNo
                    LEFT JOIN RFD_FMS.dbo.FMS_MerchantDeliverFee fmdf(NOLOCK) ON  fmdf.MerchantID = w.MerchantID AND fmdf.[Status]=2 AND fmdf.DistributionCode=w.DistributionCode
                    LEFT JOIN (
					            SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a2.AreaID,a2.AreaName 
					            FROM RFD_PMS.dbo.Province AS p(NOLOCK) 
                                JOIN RFD_PMS.dbo.City AS c(NOLOCK) ON p.ProvinceID = c.ProvinceID
					            JOIN RFD_PMS.dbo.Area AS a2(NOLOCK) ON c.CityID = a2.CityID
				              ) pca ON pca.ProvinceName=wtsi.ReceiveProvince AND pca.CityName=wtsi.ReceiveCity AND pca.AreaName=wtsi.ReceiveArea
                    WHERE w.WaybillNO=@WaybillNO";

            SqlParameter[] parameters = { new SqlParameter("@WaybillNO",  waybillNO) };

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
        }

        /// <summary>
        /// 更新金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateAmount(FMS_IncomeBaseInfo info)
        {
            string @sqlStr = @"UPDATE LMS_RFD.dbo.FMS_IncomeBaseInfo SET NeedPayAmount = @NeedPayAmount,NeedBackAmount = @NeedBackAmount,updatetime = GETDATE(),IsChange=@IsChange WHERE IncomeID=@IncomeID ";

            SqlParameter[] parameters ={
                                           new SqlParameter("@IncomeID",SqlDbType.BigInt),
                                           new SqlParameter("@NeedPayAmount",SqlDbType.Decimal),
                                           new SqlParameter("@NeedBackAmount",SqlDbType.Decimal),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
                                      };
            parameters[0].Value = info.IncomeID;
            parameters[1].Value = info.NeedPayAmount;
            parameters[2].Value = info.NeedBackAmount;
            parameters[3].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        /// <summary>
        /// 更新无效状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateInefficacyStatus(Int64 waybillNo, int inefficacyStatus)
        {
            string @sqlStr = @"UPDATE LMS_RFD.dbo.FMS_IncomeBaseInfo SET InefficacyStatus = @InefficacyStatus,IsChange=@IsChange WHERE WaybillNo=@WaybillNo ";

            SqlParameter[] parameters ={
                                           new SqlParameter("@WaybillNo",SqlDbType.BigInt),
                                           new SqlParameter("@InefficacyStatus",SqlDbType.Int),
                                           new SqlParameter("@IsChange",SqlDbType.Bit)
                                      };
            parameters[0].Value = waybillNo;
            parameters[1].Value = inefficacyStatus;
            parameters[2].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        public DataTable GetIncomeDailyReport(string beginTime, string endTime, string merchantIds, string distributionCode)
        {
            string merchantCondition = "";

            if (merchantIds.Trim().Length != 0)
            {
                merchantCondition = String.Format(" AND MerchantID in ({0}) ",merchantIds);
            }
            #region sql
            string sql = String.Format(@"
WITH t AS (
--普通单妥投，所有费用全算
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(IsNull(AccountFare,0)) AccountFare,
    SUM(IsNull(ProtectedFee,0)) ProtectedFee,
    SUM(IsNull(POSReceiveFee,0)+IsNull(ReceiveFee,0)) ReceiveFee,
    SUM(IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) ServiceFee,
    SUM(IsNull(AccountFare,0) + IsNull(ProtectedFee,0) + IsNull(POSReceiveFee,0) + IsNull(ReceiveFee,0) + IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) AllFee
from LMS_RFD.dbo.FMS_IncomeBaseInfo bInfo(nolock)
inner join LMS_RFD.dbo.FMS_IncomeFeeInfo fInfo(nolock) on bInfo.WaybillNo=fInfo.WaybillNO
inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='3' and bInfo.WaybillType='0'
  and MerchantID NOT IN (8,9)
  {0}
  and BackStationTime >= @beginTime
  and BackStationTime < @endTime
  and bInfo.DistributionCode=@DistributionCode
  group by MerchantName
UNION ALL
--换货单妥投入库，所有费用全算
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(IsNull(AccountFare,0)) AccountFare,
    SUM(IsNull(ProtectedFee,0)) ProtectedFee,
    SUM(IsNull(POSReceiveFee,0)+IsNull(ReceiveFee,0)) ReceiveFee,
    SUM(IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) ServiceFee,
    SUM(IsNull(AccountFare,0) + IsNull(ProtectedFee,0) + IsNull(POSReceiveFee,0) + IsNull(ReceiveFee,0) + IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) AllFee
from LMS_RFD.dbo.FMS_IncomeBaseInfo bInfo(nolock)
inner join LMS_RFD.dbo.FMS_IncomeFeeInfo fInfo(nolock) on bInfo.WaybillNo=fInfo.WaybillNO
inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='3' and bInfo.WaybillType='1'
  and MerchantID NOT IN (8,9)
  {0}
  and ReturnTime >= @beginTime
  and ReturnTime < @endTime
  and bInfo.DistributionCode=@DistributionCode
  group by MerchantName
UNION ALL
--上门退货单退货入库，AccountFare、ProtectedFee
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(IsNull(AccountFare,0)) AccountFare,
    SUM(IsNull(ProtectedFee,0)) ProtectedFee,
    0 ReceiveFee,
    0 ServiceFee,
    SUM(IsNull(AccountFare,0) + IsNull(ProtectedFee,0)) AllFee
from LMS_RFD.dbo.FMS_IncomeBaseInfo bInfo(nolock)
inner join LMS_RFD.dbo.FMS_IncomeFeeInfo fInfo(nolock) on bInfo.WaybillNo=fInfo.WaybillNO
inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='3' and bInfo.WaybillType='2'
  and MerchantID NOT IN (8,9)
  {0}
  and ReturnTime >= @beginTime
  and ReturnTime < @endTime
  and bInfo.DistributionCode=@DistributionCode
  group by MerchantName
UNION ALL
--普通单、换货单拒收入库，AccountFare
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(IsNull(AccountFare,0)) AccountFare,
    0 ProtectedFee,
    0 ReceiveFee,
    0 ServiceFee,
    SUM(IsNull(AccountFare,0)) AllFee
from LMS_RFD.dbo.FMS_IncomeBaseInfo bInfo(nolock)
inner join LMS_RFD.dbo.FMS_IncomeFeeInfo fInfo(nolock) on bInfo.WaybillNo=fInfo.WaybillNO
inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='5' and bInfo.WaybillType IN ('0','1')
  and MerchantID NOT IN (8,9)
  {0}
  and ReturnTime >= @beginTime
  and ReturnTime < @endTime
  and bInfo.DistributionCode=@DistributionCode
  group by MerchantName
UNION ALL
--上门退单拒收，AccountFare
select mInfo.MerchantName,
  count(1) CountNum,
    SUM(IsNull(AccountFare,0)) AccountFare,
    0 ProtectedFee,
    0 ReceiveFee,
    0 ServiceFee,
    SUM(IsNull(AccountFare,0)) AllFee
from LMS_RFD.dbo.FMS_IncomeBaseInfo bInfo(nolock)
inner join LMS_RFD.dbo.FMS_IncomeFeeInfo fInfo(nolock) on bInfo.WaybillNo=fInfo.WaybillNO
inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
where bInfo.BackStationStatus='5' and bInfo.WaybillType IN ('2')
  and MerchantID NOT IN (8,9)
  {0}
  and BackStationTime >= @beginTime
  and BackStationTime < @endTime
  and bInfo.DistributionCode=@DistributionCode
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
            #endregion
            SqlParameter[] parameters =
            {
                new SqlParameter("@beginTime",SqlDbType.DateTime){ Value=DataConvert.ToDateTime(beginTime)},
                new SqlParameter("@endTime",SqlDbType.DateTime){ Value=DataConvert.ToDateTime(endTime)},
                new SqlParameter("@DistributionCode",SqlDbType.VarChar){ Value=distributionCode }
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetIncomeDailyReportSum(string beginTime, string endTime, string merchantIds, string distributionCode)
        {
            string merchantCondition = "";

            if (merchantIds.Trim().Length != 0)
            {
                merchantCondition = String.Format("and MerchantID in ({0})", merchantIds);
            }

            string sql = String.Format(@"
                with result as (select count(1) 结算单量,
                    SUM(IsNull(AccountFare,0)) 配送费,
                    SUM(IsNull(ProtectedFee,0)) 保价费,
                    SUM(IsNull(POSReceiveFee,0)+IsNull(ReceiveFee,0)) 手续费,
                    SUM(IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) 服务费,
                    SUM(IsNull(AccountFare,0) + IsNull(ProtectedFee,0) + IsNull(POSReceiveFee,0) + IsNull(ReceiveFee,0) + IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) 应收合计
                from LMS_RFD.dbo.FMS_IncomeBaseInfo bInfo(nolock)
                inner join LMS_RFD.dbo.FMS_IncomeFeeInfo fInfo(nolock) on bInfo.WaybillNo=fInfo.WaybillNO
                inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
                where bInfo.BackStationStatus='3' and bInfo.WaybillType ='0'
                  and MerchantID!=8
                  and MerchantID!=9
                  {0}
                  and BackStationTime >= @beginTime
                  and BackStationTime <= @endTime
                  and bInfo.DistributionCode=@DistributionCode
                  group by MerchantName
                union all
                select count(1) 结算单量,
                    SUM(IsNull(AccountFare,0)) 配送费,
                    SUM(IsNull(ProtectedFee,0)) 保价费,
                    SUM(IsNull(POSReceiveFee,0)+IsNull(ReceiveFee,0)) 手续费,
                    SUM(IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) 服务费,
                    SUM(IsNull(AccountFare,0) + IsNull(ProtectedFee,0) + IsNull(POSReceiveFee,0) + IsNull(ReceiveFee,0) + IsNull(POSReceiveServiceFee,0) + IsNull(CashReceiveServiceFee,0)) 应收合计
                from LMS_RFD.dbo.FMS_IncomeBaseInfo bInfo(nolock)
                inner join LMS_RFD.dbo.FMS_IncomeFeeInfo fInfo(nolock) on bInfo.WaybillNo=fInfo.WaybillNO
                inner join RFD_PMS.dbo.MerchantBaseInfo mInfo(nolock) on bInfo.MerchantID=mInfo.ID
                where bInfo.BackStationStatus='3' and bInfo.WaybillType in ('1','2')
                  and MerchantID!=8
                  and MerchantID!=9
                  {0}
                  and ReturnTime >= @beginTime
                  and ReturnTime <= @endTime
                  and bInfo.DistributionCode=@DistributionCode
                  group by MerchantName
                )
                select 
                    SUM(结算单量) WaybillCount,
                    SUM(配送费) AccountFare,
                    SUM(保价费) ProtectedFee,
                    SUM(手续费) ReceiveFee,
                    SUM(服务费) POSReceiveServiceFee,
                    SUM(应收合计) SumFee,
                    cast(SUM(应收合计)/SUM(结算单量) as numeric(18,2)) AvgFee
                from result  
                ", merchantCondition);

            SqlParameter[] parameters =
            {
                new SqlParameter("@beginTime",SqlDbType.DateTime){ Value=DataConvert.ToDateTime(beginTime)},
                new SqlParameter("@endTime",SqlDbType.DateTime){ Value=DataConvert.ToDateTime(endTime)},
                new SqlParameter("@DistributionCode",SqlDbType.VarChar){ Value=distributionCode }
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable GetDeliverFeeParameter(long waybillNo, string distributionCode)
        {
            string sql = @"select baseInfo.WaybillNo,
	            baseInfo.AccountWeight Weight,
	            areaIncome.AreaType Area,
	            feeInfo.AccountStandard Formula,
	            feeInfo.AccountFare DeliverFee
            from LMS_RFD.dbo.FMS_IncomeBaseInfo baseInfo(nolock)
            inner join LMS_RFD.dbo.FMS_IncomeFeeInfo feeInfo(nolock) on baseInfo.WaybillNo=feeInfo.WaybillNO
            inner join RFD_FMS.dbo.AreaExpressLevelIncome areaIncome(nolock) on baseInfo.AreaID=areaIncome.AreaID 
	            and baseInfo.MerchantID=areaIncome.MerchantID 
	            and baseInfo.ExpressCompanyID=areaIncome.WareHouseID
            where baseInfo.WaybillNO=@WaybillNO and baseInfo.DistributionCode=@DistributionCode 
            order by feeInfo.CreateTime desc";

            SqlParameter[] parameters =
            {
                new SqlParameter("@WaybillNO",SqlDbType.BigInt){ Value=waybillNo },
                new SqlParameter("@DistributionCode",SqlDbType.VarChar){ Value=distributionCode }
            };

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public bool SaveDeliverFee(DeliverFeeModel model)
        {
            string sql = "update LMS_RFD.dbo.FMS_IncomeBaseInfo set AccountWeight=@AccountWeight,IsChange=1 where WaybillNo=@WaybillNO";

            SqlParameter[] parameters =
            {
                new SqlParameter("@AccountWeight",SqlDbType.Decimal){ Value = model.Weight },
                new SqlParameter("@WaybillNO",SqlDbType.BigInt){ Value = model.WaybillNO }
            };

            if (SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) != 1) return false;

            string sql1 = @"update LMS_RFD.dbo.FMS_IncomeFeeInfo 
	            set AccountStandard=@AccountStandard,AreaType=@AreaType,AccountFare=@AccountFare,IsChange=1,IsAccount=1
	            where WaybillNo=@WaybillNO";

            SqlParameter[] parameters1 =
            {
                new SqlParameter("@AccountStandard",SqlDbType.VarChar){ Value = model.Formula },
                new SqlParameter("@AreaType",SqlDbType.Int){ Value = model.Area },
                new SqlParameter("@AccountFare",SqlDbType.Decimal){ Value = model.DeliverFee },
                new SqlParameter("@WaybillNO",SqlDbType.BigInt){ Value = model.WaybillNO }
            };

            if (SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql1, parameters1) != 1) return false;

            return true;
        }


        public bool UpdateEvalStatus(long waybillNo)
        {
            string sql = @"update LMS_RFD.dbo.FMS_IncomeFeeInfo set IsAccount=0 where WaybillNO=@WaybillNO";

            SqlParameter[] parameters =
            {
                new SqlParameter("@WaybillNO",SqlDbType.BigInt){ Value = waybillNo }
            };

            if (SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) != 1) return false;

            return true;
        }

        public bool UpdateEvalStatus(string waybillNos)
        {
            string sql = String.Format(@"update LMS_RFD.dbo.FMS_IncomeFeeInfo set IsAccount=0 where WaybillNO in ({0})", waybillNos);

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql) > 0;
        }
    }
}
