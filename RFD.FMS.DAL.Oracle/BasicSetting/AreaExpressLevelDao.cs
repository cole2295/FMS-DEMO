using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using System.Data.SqlClient;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.DataAccess.Client;
using RFD.FMS.MODEL;
using System.Xml;
using RFD.FMS.Util.OraclePageCommon;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
	public class AreaExpressLevelDao : OracleDao, IAreaExpressLevelDao
	{
		private string sqlStr = "";

		public DataTable SearchArea(string provinceId, string cityId, string areaId, string stationId, string merchantId, string distributionCode,ref PageInfo pi)
		{
            sqlStr = @"with tmp as (SELECT DISTINCT  p.ProvinceID,
			                p.ProvinceName,
			                c.CityID,
			                c.CityName,
			                a.AreaID,
			                a.AreaName,
			                (CASE WHEN ( ael.AreaType IS NOT NULL
														 OR ael.EffectAreaType IS NOT NULL
													   )
													   AND IsEnable IN ( 1,2, 3 ) THEN '√'
												  ELSE ''
											 END) IsAreaType
			
			  FROM Province p
			  JOIN City c
			    ON c.ProvinceID = p.ProvinceID
			  JOIN Area a
			    ON a.CityID = c.CityID
			  LEFT JOIN AreaExpressLevel ael
			    ON ael.AreaID = a.AreaID
			   AND ael.IsEnable IN (1, 2, 3) 
			                                      AND ael.DistributionCode=:DistributionCode 
			            WHERE   p.IsDeleted = 0
			                AND c.IsDeleted = 0
			                AND a.IsDeleted = 0 
							{0} ";

			StringBuilder sbWhere = new StringBuilder();
			List<OracleParameter> parameters = new List<OracleParameter>();

			if (!string.IsNullOrEmpty(provinceId))
			{
				sbWhere.Append(" AND p.ProvinceID=:ProvinceID ");
				parameters.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 20) { Value = provinceId });
			}

			if (!string.IsNullOrEmpty(cityId))
			{
				sbWhere.Append(" AND c.CityID=:CityID ");
				parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 20) { Value = cityId });
			}

			if (!string.IsNullOrEmpty(areaId))
			{
				sbWhere.Append(" AND a.AreaID=:AreaID ");
				parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) { Value = areaId });
			}

			if (!string.IsNullOrEmpty(distributionCode))
			{
				sbWhere.Append(" AND a.DistributionCode=:DistributionCode ");
				parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			}

			if (!string.IsNullOrEmpty(stationId))
			{
				sbWhere.Append(" AND ael.ExpressCompanyID=:ExpressCompanyID ");
				parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = stationId });
			}
		    sbWhere.Append(")");
		  sqlStr = string.Format(sqlStr, sbWhere.ToString());
            int itemcount = Convert.ToInt32(GetOrderInfoCount(sqlStr.ToString()+"select count(*) from tmp", parameters.ToArray()));
            string newSqlQuery = "";
            pi.PageSize = 20;
            pi.SetItemCount(itemcount);
            int begin = pi.CurrentPageBeginItemIndex;
            int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;

            if (begin > 1)
            {
                newSqlQuery = 
                String.Format(sqlStr.ToString() +
                "select * from (select ROW_NUMBER() over(order by tmp.areaid) rowno,ProvinceID,ProvinceName,CityID,CityName,AreaID,AreaName,IsAreaType from tmp )WHERE rowno>={0} AND rowno<{1} ", begin, end);
            }
            else
            {
                newSqlQuery = 
                String.Format(sqlStr.ToString() + 
                "select * from (select ROW_NUMBER() over(order by tmp.areaid) rowno,ProvinceID,ProvinceName,CityID,CityName,AreaID,AreaName,IsAreaType from tmp )WHERE   rowno<{0} ", end);
            }
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, newSqlQuery, ToParameters(parameters.ToArray())).Tables[0];
		}

		public DataTable SearchAreaType(string areaId, string distributionCode,ref PageInfo pi)
		{
            sqlStr = @"SELECT ROW_NUMBER() over(order by ael.areaid) rowno  , ael.AutoID ,
								ael.AreaID ,
								ael.ExpressCompanyID ,
								ec.CompanyName ,
								ael.WareHouseID ,
								( CASE ael.WareHouseType
												  WHEN 1 THEN w.WareHouseName
												  WHEN 2 THEN w1.CompanyName
												  ELSE ''
												END) WareHouseName ,
								mbi.ID ,
								mbi.MerchantName ,
								(CASE WHEN ael.IsEnable = 3 THEN NULL
												ELSE ael.AreaType
										   END) AreaType ,
								( CASE WHEN AuditStatus IN ( 0, 1, 3 )
													  THEN ael.EffectAreaType
													  ELSE NULL
												 END) EffectAreaType ,
								( CASE AuditStatus
												   WHEN 0 THEN '未审核'
												   WHEN 1 THEN '已审核'
												   WHEN 2 THEN '已同步'
												   WHEN 3 THEN '置回'
												   ELSE NULL
												 END) AuditStatusStr,
								(CASE ael.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END) EnableStr
						FROM    AreaExpressLevel ael
								LEFT JOIN Warehouse w ON w.WareHouseID = ael.WareHouseID
								LEFT JOIN ExpressCompany w1 ON w1.ExpressCompanyID = ael.WareHouseID
																			  AND w1.CompanyFlag = 1
								JOIN ExpressCompany ec ON ec.ExpressCompanyID = ael.ExpressCompanyID
								JOIN MerchantBaseInfo mbi ON mbi.ID = ael.MerchantID
						WHERE   ael.IsEnable IN ( 1,2, 3 ) {0}";
			StringBuilder sbWhere = new StringBuilder();
			List<OracleParameter> parameters = new List<OracleParameter>();

			if (!string.IsNullOrEmpty(areaId))
			{
				sbWhere.Append(" AND ael.AreaID=:AreaID ");
				parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) { Value = areaId });
			}

			if (!string.IsNullOrEmpty(distributionCode))
			{
				sbWhere.Append(" AND ael.DistributionCode=:DistributionCode ");
				parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			}
            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            int itemcount = Convert.ToInt32(GetOrderInfoCount(string.Format("select count(*) from ({0})",sqlStr), parameters.ToArray()));
            string newSqlQuery = "";
		    pi.PageSize = 20;
            pi.SetItemCount(itemcount);
            int begin = pi.CurrentPageBeginItemIndex;
            int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;

            if (begin > 1)
            {
                newSqlQuery = String.Format(" SELECT * FROM (  " + sqlStr.ToString() + "  ) WHERE rowno>={0} AND rowno<{1} ", begin, end);
            }
            else
            {
                newSqlQuery = String.Format(" SELECT * FROM (  " + sqlStr.ToString() + "  ) WHERE  rowno<{0} ",  end);
            }
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, newSqlQuery, ToParameters(parameters.ToArray())).Tables[0];
		}
        public DataTable SearchSecondAreaType(string areaID, string areaType, string stationID, string merchantID, string distributionCode,ref PageInfo pi)
         {
             sqlStr = @"SELECT  ROW_NUMBER() over(order by ael.areaid) rowno  , ael.AutoID ,
								ael.AreaID ,
								ael.ExpressCompanyID ,
								ec.CompanyName ,
								ael.WareHouseID ,
								( CASE ael.WareHouseType
												  WHEN 1 THEN w.WareHouseName
												  WHEN 2 THEN w1.CompanyName
												  ELSE ''
												END) WareHouseName ,
								mbi.ID ,
								mbi.MerchantName ,
								(CASE WHEN ael.IsEnable = 3 THEN NULL
												ELSE ael.AreaType
										   END) AreaType ,
								( CASE WHEN AuditStatus IN ( 0, 1, 3 )
													  THEN ael.EffectAreaType
													  ELSE NULL
												 END) EffectAreaType ,
								( CASE AuditStatus
												   WHEN 0 THEN '未审核'
												   WHEN 1 THEN '已审核'
												   WHEN 2 THEN '已同步'
												   WHEN 3 THEN '置回'
												   ELSE NULL
												 END) AuditStatusStr,
								(CASE ael.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END) EnableStr
						FROM    AreaExpressLevel ael
								LEFT JOIN Warehouse w ON w.WareHouseID = ael.WareHouseID
								LEFT JOIN ExpressCompany w1 ON w1.ExpressCompanyID = ael.WareHouseID
																			  AND w1.CompanyFlag = 1
								JOIN ExpressCompany ec ON ec.ExpressCompanyID = ael.ExpressCompanyID
								JOIN MerchantBaseInfo mbi ON mbi.ID = ael.MerchantID
						WHERE   ael.IsEnable IN ( 1,2, 3 ) {0}";
             StringBuilder sbWhere = new StringBuilder();
             List<OracleParameter> parameters = new List<OracleParameter>();
             if (!string.IsNullOrEmpty(areaID))
             {
                 sbWhere.Append(" AND ael.AreaID=:AreaID ");
                 parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) { Value = areaID });
             }

             if (!string.IsNullOrEmpty(areaType))
             {
                 sbWhere.Append(" AND ael.AreaType=:AreaType ");
                 parameters.Add(new OracleParameter(":AreaType", OracleDbType.Varchar2, 100) { Value = areaType });
             }

             if (!string.IsNullOrEmpty(distributionCode))
             {
                 sbWhere.Append(" AND ael.DistributionCode=:DistributionCode ");
                 parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
             }
             if (!string.IsNullOrEmpty(stationID))
             {
                 sbWhere.Append(" AND ael.ExpressCompanyID=:ExpressCompanyID ");
                 parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Varchar2, 100) { Value = stationID });
             }
             if(!string.IsNullOrEmpty(merchantID))
             {
                 sbWhere.Append(" AND ael.merchantID=:merchantID ");
                 parameters.Add(new OracleParameter(":merchantID", OracleDbType.Varchar2, 100) { Value = merchantID });
             }
             sqlStr = string.Format(sqlStr, sbWhere.ToString());
             int itemcount = Convert.ToInt32(GetOrderInfoCount(string.Format("select count(*) from ({0})", sqlStr), parameters.ToArray()));
             string newSqlQuery = "";
             pi.PageSize = 20;
             pi.SetItemCount(itemcount);
             int begin = pi.CurrentPageBeginItemIndex;
             int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;

             if (begin > 1)
             {
                 newSqlQuery = String.Format(" SELECT * FROM (  " + sqlStr.ToString() + "  ) WHERE rowno>={0} AND rowno<{1} ", begin, end);
             }
             else
             {
                 newSqlQuery = String.Format(" SELECT * FROM (  " + sqlStr.ToString() + "  ) WHERE  rowno<{0} ", end);
             }
             return OracleHelper.ExecuteDataset(Connection, CommandType.Text, newSqlQuery, ToParameters(parameters.ToArray())).Tables[0];

         }

		public DataTable SearchAreaTypeByAutoId(string autoId)
		{
			sqlStr = @"SELECT   ael.AutoID,
								ael.AreaID,
								ael.AreaID,
							    ael.ExpressCompanyID,
							    ael.WareHouseID,
								ael.IsEnable,
							    ael.AreaType,
								ael.WareHouseType,
								ael.MerchantID,
								ael.ProductID
						FROM   AreaExpressLevel ael
						WHERE AutoID=:AutoID";
			OracleParameter[] parameters ={
										   new OracleParameter(":AutoID",OracleDbType.Decimal),
									  };
			parameters[0].Value = autoId;
			return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, parameters).Tables[0];
		}

		public bool AddAreaType(AreaExpressLevel areaExpressLevel)
		{
			sqlStr = @"SELECT count(1) FROM AreaExpressLevel
								  WHERE  ExpressCompanyID = :ExpressCompanyID
										 AND AreaID = :AreaID
										 AND NVL(WareHouseID,'')!=''
										 AND MerchantID=:MerchantID
										 AND IsEnable in (1,2,3)
                                         AND DistributionCode=:DistributionCode";
		    OracleParameter[] para0 = {
		                                       new OracleParameter(":ExpressCompanyID", OracleDbType.Int32),
                                               new OracleParameter(":AreaID",OracleDbType.Varchar2),
                                               new OracleParameter(":MerchantID",OracleDbType.Int32),
                                               new OracleParameter(":DistributionCode",OracleDbType.Varchar2)

		                                   };

		    para0[0].Value = areaExpressLevel.ExpressCompanyID;
		    para0[1].Value = areaExpressLevel.AreaID;
		    para0[2].Value = areaExpressLevel.MerchantID;
		    para0[3].Value = areaExpressLevel.DistributionCode;
            //sqlStr = String.Format(sqlStr, areaExpressLevel.ExpressCompanyID, areaExpressLevel.AreaID,
            //                       areaExpressLevel.MerchantID, areaExpressLevel.DistributionCode)};
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, para0) ?? 0;
			if (Convert.ToInt32(obj) > 0)
			{
				return false;
			}

			if (string.IsNullOrEmpty(areaExpressLevel.WareHouseID))
			{
				sqlStr = @"SELECT count(1)
									  FROM   AreaExpressLevel
									  WHERE  ExpressCompanyID = :ExpressCompanyID
											 AND AreaID = :AreaID
											 AND MerchantID= :MerchantID
											 AND IsEnable in (1,2,3)
                                             AND DistributionCode= :DistributionCode";

                OracleParameter[] para1 = {
		                                       new OracleParameter(":ExpressCompanyID", OracleDbType.Int32),
                                               new OracleParameter(":AreaID",OracleDbType.Varchar2),
                                               new OracleParameter(":MerchantID",OracleDbType.Int32),
                                               new OracleParameter(":DistributionCode",OracleDbType.Varchar2)

		                                   };

                para1[0].Value = areaExpressLevel.ExpressCompanyID;
                para1[1].Value = areaExpressLevel.AreaID;
                para1[2].Value = areaExpressLevel.MerchantID;
                para1[3].Value = areaExpressLevel.DistributionCode;

				//sqlStr = String.Format(sqlStr, areaExpressLevel.ExpressCompanyID, areaExpressLevel.AreaID,
		 // areaExpressLevel.MerchantID, areaExpressLevel.DistributionCode);
				obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr,para1) ?? 0;
				if (Convert.ToInt32(obj) > 0)
				{
					return false;
				}
			}
			else
			{
				sqlStr = @"SELECT count(1)
									  FROM   AreaExpressLevel
									  WHERE  ExpressCompanyID = :ExpressCompanyID
											 AND AreaID = :AreaID
											 AND MerchantID=:MerchantID
											 AND IsEnable in (1,2,3)
                                             AND DistributionCode=:DistributionCode
											 AND NVL(WareHouseID,'')=:WareHouseID ";
                OracleParameter[] para2 = {
		                                       new OracleParameter(":ExpressCompanyID", OracleDbType.Int32),
                                               new OracleParameter(":AreaID",OracleDbType.Varchar2),
                                               new OracleParameter(":MerchantID",OracleDbType.Int32),
                                               new OracleParameter(":DistributionCode",OracleDbType.Varchar2),
                                               new OracleParameter(":WareHouseID",OracleDbType.Varchar2)

		                                   };

                para2[0].Value = areaExpressLevel.ExpressCompanyID;
                para2[1].Value = areaExpressLevel.AreaID;
                para2[2].Value = areaExpressLevel.MerchantID;
                para2[3].Value = areaExpressLevel.DistributionCode;
			    para2[4].Value = areaExpressLevel.WareHouseID;
				//sqlStr = String.Format(sqlStr, areaExpressLevel.ExpressCompanyID, areaExpressLevel.AreaID,
		 // areaExpressLevel.MerchantID, areaExpressLevel.DistributionCode, areaExpressLevel.WareHouseID);
				obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, para2) ?? 0;
				if (Convert.ToInt32(obj) > 0)
				{
					return false;
				}
			}

			sqlStr = @"INSERT INTO AreaExpressLevel
									   (AUTOID
                                       ,AreaID
									   ,ExpressCompanyID
									   ,WareHouseID
									   ,AreaType
									   ,IsEnable
									   ,EffectAreaType
									   ,DoDate
									   ,CreateBy
									   ,CreateTime
									   ,UpdateBy
									   ,UpdateTime
										,AuditStatus
										,AuditBy
										,AuditTime
										,WareHouseType
										,MerchantID
										,ProductID
                                        ,DistributionCode
										)
								 VALUES
									   (:AUTOID
                                        ,:AreaID
									   ,:ExpressCompanyID
									   ,:WareHouseID
									   ,:AreaType
									   ,:IsEnable
									   ,:EffectAreaType
									   ,:DoDate
									   ,:CreateBy
									   ,SysDate
									   ,:UpdateBy
									   ,SysDate
										,:AuditStatus
										,:AuditBy
										,SysDate
										,:WareHouseType
										,:MerchantID
										,:ProductID
                                        ,:DistributionCode
										)";

			OracleParameter[] parameters ={
                                           new OracleParameter(":AUTOID", OracleDbType.Decimal),
										   new OracleParameter(":AreaID", OracleDbType.Varchar2,200),
										   new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
										   new OracleParameter(":WareHouseID",OracleDbType.Varchar2,80),
										   new OracleParameter(":AreaType",OracleDbType.Decimal),
										   new OracleParameter(":IsEnable",OracleDbType.Decimal),
										   new OracleParameter(":EffectAreaType",OracleDbType.Decimal),
										   new OracleParameter(":DoDate",OracleDbType.Date),
										   new OracleParameter(":CreateBy",OracleDbType.Varchar2,200),
										   new OracleParameter(":UpdateBy",OracleDbType.Varchar2,200),
										   new OracleParameter(":AuditStatus",OracleDbType.Decimal),
										   new OracleParameter(":AuditBy",OracleDbType.Varchar2,200),
										   new OracleParameter(":WareHouseType",OracleDbType.Decimal),
										   new OracleParameter(":MerchantID",OracleDbType.Decimal),
										   new OracleParameter(":ProductID",OracleDbType.Decimal),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
									  };
            parameters[0].Value = GetIdNew("SEQ_AREAEXPRESSLEVEL");
			parameters[1].Value = areaExpressLevel.AreaID;
			parameters[2].Value = areaExpressLevel.ExpressCompanyID;
			parameters[3].Value = areaExpressLevel.WareHouseID;
			parameters[4].Value = areaExpressLevel.AreaType;
			parameters[5].Value = areaExpressLevel.Enable;
			parameters[6].Value = areaExpressLevel.EffectAreaType;
			parameters[7].Value = areaExpressLevel.DoDate;
			parameters[8].Value = areaExpressLevel.CreateBy;
			parameters[9].Value = areaExpressLevel.UpdateBy;
			parameters[10].Value = areaExpressLevel.AuditStatus;
			parameters[11].Value = areaExpressLevel.AuditBy;
			parameters[12].Value = areaExpressLevel.WareHouseType;
			parameters[13].Value = areaExpressLevel.MerchantID;
			parameters[14].Value = areaExpressLevel.ProductID;
			parameters[15].Value = areaExpressLevel.DistributionCode;
			int iCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters);
			return iCount > 0 ? true : false;
		}

		public bool UpdateAreaType(AreaExpressLevel areaExpressLevel, out int autoId)
		{
			sqlStr = @"SELECT count(1) FROM AreaExpressLevel
								  WHERE  ExpressCompanyID = {0}
										 AND AreaID = '{1}'
										 AND NVL(WareHouseID,'')!=''
										 AND MerchantID={2}
										 AND IsEnable in (1,2,3)
                                         AND DistributionCode='{3}'";
			sqlStr = String.Format(sqlStr, areaExpressLevel.ExpressCompanyID, areaExpressLevel.AreaID,
								   areaExpressLevel.MerchantID, areaExpressLevel.DistributionCode);
			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, null) ?? 0;
			if (Convert.ToInt32(obj) > 0)
			{
				autoId = 0;
				return false;
			}

			if (string.IsNullOrEmpty(areaExpressLevel.WareHouseID))
			{
				sqlStr = @"SELECT count(1)
									  FROM   AreaExpressLevel
									  WHERE  ExpressCompanyID = {0}
											 AND AreaID = '{1}'
											 AND MerchantID={2}
											 AND IsEnable in (1,2,3)
                                             AND DistributionCode='{3}'";
				sqlStr = String.Format(sqlStr, areaExpressLevel.ExpressCompanyID, areaExpressLevel.AreaID,
		  areaExpressLevel.MerchantID, areaExpressLevel.DistributionCode);
				obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, null) ?? 0;
				if (Convert.ToInt32(obj) == 0)
				{
					autoId = 0;
					return false;
				}
			}
			else
			{
				sqlStr = @"SELECT count(1)
									  FROM   AreaExpressLevel
									  WHERE  ExpressCompanyID = {0}
											 AND AreaID = '{1}'
											 AND MerchantID={2}
											 AND IsEnable in (1,2,3)
                                             AND DistributionCode='{3}'
											 AND NVL(WareHouseID,'')='{4}' ";
				sqlStr = String.Format(sqlStr, areaExpressLevel.ExpressCompanyID, areaExpressLevel.AreaID,
		  areaExpressLevel.MerchantID, areaExpressLevel.DistributionCode, areaExpressLevel.WareHouseID);
				obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, null) ?? 0;
				if (Convert.ToInt32(obj) == 0)
				{
					autoId = 0;
					return false;
				}
			}
			 sqlStr = @"SELECT AutoID FROM AreaExpressLevel WHERE AreaID = :AreaID AND ExpressCompanyID = :ExpressCompanyID AND IsEnable in (1,2,3) AND DistributionCode=:DistributionCode";
             OracleParameter[] parameters1 ={
										   new OracleParameter(":AreaID",OracleDbType.Varchar2,200),
										   new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal), 
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
									  };
             parameters1[0].Value = areaExpressLevel.AreaID;
             parameters1[1].Value = areaExpressLevel.ExpressCompanyID;
             parameters1[2].Value = areaExpressLevel.DistributionCode;
             obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, parameters1) ?? 0;
			 autoId = Convert.ToInt32(obj);
			 sqlStr = @"UPDATE AreaExpressLevel
							SET    EffectAreaType = :EffectAreaType,
								   WareHouseID = :WareHouseID,
								   ProductID = :ProductID,
								   UpdateBy = :UpdateBy,
								   UpdateTime = SysDate,
								   AuditStatus = :AuditStatus
							WHERE  AreaID = :AreaID
								   AND ExpressCompanyID = :ExpressCompanyID
								   AND MerchantID=:MerchantID
								   AND IsEnable in (1,2,3)
                                   AND DistributionCode=:DistributionCode
                  ";
			OracleParameter[] parameters ={
										   new OracleParameter(":AreaID",OracleDbType.Varchar2,200),
										   new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
										   new OracleParameter(":WareHouseID",OracleDbType.Varchar2,80),
										   new OracleParameter(":EffectAreaType",OracleDbType.Decimal),
										   new OracleParameter(":UpdateBy",OracleDbType.Varchar2,200),
										   new OracleParameter(":AuditStatus",OracleDbType.Decimal),
										   new OracleParameter(":MerchantID",OracleDbType.Decimal),
										   new OracleParameter(":ProductID",OracleDbType.Decimal),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
									  };
			parameters[0].Value = areaExpressLevel.AreaID;
			parameters[1].Value = areaExpressLevel.ExpressCompanyID;
			parameters[2].Value = areaExpressLevel.WareHouseID;
			parameters[3].Value = areaExpressLevel.EffectAreaType;
			parameters[4].Value = areaExpressLevel.UpdateBy;
			parameters[5].Value = areaExpressLevel.AuditStatus;
			parameters[6].Value = areaExpressLevel.MerchantID;
			parameters[7].Value = areaExpressLevel.ProductID;
			parameters[8].Value = areaExpressLevel.DistributionCode;
			return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0 ? true : false;
		}

		public bool DeleteAreaType(string autoId, string updateBy)
		{
			sqlStr = @"UPDATE AreaExpressLevel SET IsEnable=2,AuditStatus=0,UpdateBy=:UpdateBy,UpdateTime=SysDate WHERE AutoID=:AutoID";
			OracleParameter[] parameters ={
										   new OracleParameter(":AutoID",OracleDbType.Decimal),
										   new OracleParameter(":UpdateBy",OracleDbType.Varchar2,200),
									   };
			parameters[0].Value = autoId;
			parameters[1].Value = updateBy;
			return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
		}

		#region 区域类型审批 by wangxue

		//查询区域类型
		public DataTable SearchAreaExpressCompanyLevel(int status, string areaid, string cityid, string provinceid, int expresscompanyid, int areatype, int warehousetype, string warehouseid, int merchantid, string distributionCode, ref PageInfo pi)
		{

			string sql =
				string.Format(
					@"select ael.areaid,min(ael.autoid) as id from areaexpresslevel ael where ael.auditStatus={0} AND DistributionCode=:DistributionCode", status);

			string sqlcount =
				string.Format(
					@"select ael.areaid,min(ael.autoid) as id from areaexpresslevel ael where ael.auditStatus={0} AND DistributionCode=:DistributionCode", status);
			List<OracleParameter> parameters = new List<OracleParameter>();
			parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });

			if (expresscompanyid > 0)
			{
				sql += " and ael.expresscompanyid=:expresscompanyid";
				sqlcount += " and ael.expresscompanyid=:expresscompanyid";
				parameters.Add(new OracleParameter(":expresscompanyid", OracleDbType.Decimal, 4) { Value = expresscompanyid });
			}

			if (areatype > 0)
			{
				sql += " and ael.areatype=:areatype";
				sqlcount += " and ael.areatype=:areatype";
				parameters.Add(new OracleParameter(":areatype", OracleDbType.Decimal, 4) { Value = areatype });
			}

			if (warehousetype > 0)
			{
				sql += " and ael.warehousetype=:warehousetype ";
				sqlcount += " and ael.warehousetype=:warehousetype";
				parameters.Add(new OracleParameter(":warehousetype", OracleDbType.Decimal, 4) { Value = warehousetype });
			}

			if (warehousetype > 0 && !string.IsNullOrEmpty(warehouseid))
			{
				sql += " and ael.warehouseid=:warehouseid ";
				sqlcount += " and ael.warehouseid=:warehouseid";
				parameters.Add(new OracleParameter(":warehouseid", OracleDbType.Varchar2, 200) { Value = warehouseid });
			}

			if (merchantid > 0)
			{
				sql += " and ael.merchantid=:merchantid";
				sqlcount += " and ael.merchantid=:merchantid";
				parameters.Add(new OracleParameter(":merchantid", OracleDbType.Decimal, 4) { Value = merchantid });
			}

			sql += " group by ael.areaid ";
			sqlcount += " group by ael.areaid";

			string strResult = string.Format(@"select p.provincename,cityname,areaname,a1.areaid,a1.dodate,e.employeename,a1.AuditTime,s.statusname,a1.auditstatus,
                                                CASE a1.warehousetype WHEN 2 THEN e1.companyname WHEN 1 THEN w.warehousename ELSE NULL END warehouseExpressCompany,
                                                b.merchantname
                                                from ({0})  a
                                                join areaexpresslevel a1 on a1.autoid=a.id
                                                join Area ar on a.areaid=ar.areaid
                                                join City c on ar.cityid=c.cityid
                                                join Province p  on c.provinceid=p.ProvinceID 
                                                join StatusInfo s on a1.auditstatus=s.statusno and s.statustypeno='306'
                                                left join employee e on a1.AuditBy=e.employeeid
                                                LEFT JOIN Warehouse w  ON w.warehouseid=a1.warehouseid AND a1.warehousetype=1
                                                LEFT JOIN ExpressCompany e1 ON e1.expresscompanyid=a1.warehouseid AND a1.warehousetype=2
                                                LEFT JOIN MerchantBaseInfo b ON b.id=a1.merchantid where 1=1 ", sql);

			StringBuilder count = new StringBuilder();
			count.AppendFormat(
								@"select count(1) from ({0})  a
                                join areaexpresslevel a1 on a1.autoid=a.id
                                join Area ar on a.areaid=ar.areaid
                                join City c on ar.cityid=c.cityid
                                join Province p  on c.provinceid=p.ProvinceID 
                                join StatusInfo s on a1.auditstatus=s.statusno and s.statustypeno='306'
                                left join employee e on a1.AuditBy=e.employeeid
                                LEFT JOIN Warehouse w  ON w.warehouseid=a1.warehouseid AND a1.warehousetype=1
                                LEFT JOIN ExpressCompany e1 ON e1.expresscompanyid=a1.warehouseid AND a1.warehousetype=2
                                LEFT JOIN MerchantBaseInfo b ON b.id=a1.merchantid where 1=1 ", sqlcount);

			if (!string.IsNullOrEmpty(areaid))
			{
				strResult += " and ar.areaid=:areaid";
				count.Append(" and ar.areaid=:areaid");
				parameters.Add(new OracleParameter(":areaid", OracleDbType.Varchar2, 200) { Value = areaid });
			}

			if (!string.IsNullOrEmpty(cityid))
			{
				strResult += " and c.cityid=:cityid";
				count.Append(" and c.cityid=:cityid");
				parameters.Add(new OracleParameter(":cityid", OracleDbType.Varchar2, 200) { Value = cityid });
			}

			if (!string.IsNullOrEmpty(provinceid))
			{
				strResult += " and p.provinceid=:provinceid";
				count.Append(" and p.provinceid=:provinceid");
				parameters.Add(new OracleParameter(":provinceid", OracleDbType.Varchar2, 200) { Value = provinceid });
			}

			int itemcount = Convert.ToInt32(GetOrderInfoCount(count.ToString(), parameters.ToArray()));
			string newSqlQuery = "";
			pi.SetItemCount(itemcount);
			int begin = pi.CurrentPageBeginItemIndex;
			int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;

			if (begin > 1)
			{
				newSqlQuery = String.Format(" SELECT * FROM ( SELECT  ROW_NUMBER() over(order by areaid )  rowno,allrecord.* FROM ( " + strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno>={0} AND rowno<{1} ", begin, end);
			}
			else
			{
				newSqlQuery = String.Format(" SELECT * FROM (  SELECT  ROW_NUMBER() over(order by areaid)  rowno,allrecord.* FROM ( " + strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno<{0} ", end);
			}
			OracleParameter[] commandParametersNew = PageCommon.ToParameters(parameters.ToArray());
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, newSqlQuery, commandParametersNew).Tables[0];
		}

		public DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, string distributionCode)
		{
			string strResult = string.Format(@"select ael.autoid,ael.areaid,ael.expresscompanyid,ael.dodate,a.areaname,ec.companyname,ael.areatype,ael.effectareatype,e.employeename,case when e.employeename is not null then ael.updatetime else null end as updatetime,
												(CASE ael.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END) EnableStr,
                                               ael.warehouseid,ael.merchantid,ael.productid,ael.warehousetype 
                                                from areaexpresslevel ael  
                                               join ExpressCompany ec  on ael.expresscompanyid=ec.expresscompanyid
                                               join Area a on ael.areaid=a.areaid
                                               left join employee e on ael.updateby=e.employeeid
                                               where 1=1 and ael.IsEnable in (1,2,3) and ael.auditstatus={0} AND ael.DistributionCode=:DistributionCode", status);

			List<OracleParameter> parameters = new List<OracleParameter>();
			parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			if (!string.IsNullOrEmpty(areaid))
			{
				strResult += " and ael.areaid=:areaid";
				parameters.Add(new OracleParameter(":areaid", OracleDbType.Varchar2, 200) { Value = areaid });
			}
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters.ToArray()).Tables[0];
		}

		public DataTable SearchAreaExpressCompanyLevelDetail(string areaid, int status, int expresscompanyid, int areatype, int warehousetype, string warehouseid, int merchantid)
		{
			string strResult = string.Format(@"select ael.autoid,ael.areaid,ael.expresscompanyid,ael.dodate,a.areaname,ec.companyname,ael.areatype,ael.effectareatype,e.employeename,case when e.employeename is not null then ael.updatetime else null end as updatetime,
												(CASE ael.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END) EnableStr,
                                               ael.warehouseid,ael.merchantid,ael.productid,ael.warehousetype from areaexpresslevel ael  
                                               join ExpressCompany ec  on ael.expresscompanyid=ec.expresscompanyid
                                               join Area a on ael.areaid=a.areaid
                                               left join employee e on ael.updateby=e.employeeid
                                               where 1=1 and ael.IsEnable in (1,2,3) and ael.auditstatus={0}", status);

			List<OracleParameter> parameters = new List<OracleParameter>();

			if (!string.IsNullOrEmpty(areaid))
			{
				strResult += " and ael.areaid=:areaid";
				parameters.Add(new OracleParameter(":areaid", OracleDbType.Varchar2, 200) { Value = areaid });
			}

			if (expresscompanyid > 0)
			{
				strResult += " and ael.expresscompanyid=:expresscompanyid";
				parameters.Add(new OracleParameter(":expresscompanyid", OracleDbType.Decimal, 4) { Value = expresscompanyid });
			}

			if (areatype > 0)
			{
				strResult += " and ael.areatype=:areatype";
				parameters.Add(new OracleParameter(":areatype", OracleDbType.Decimal, 4) { Value = areatype });
			}

			if (warehousetype > 0)
			{
				strResult += " and ael.warehousetype=:warehousetype";
				parameters.Add(new OracleParameter(":warehousetype", OracleDbType.Decimal, 4) { Value = warehousetype });

				if (!string.IsNullOrEmpty(warehouseid))
				{
					strResult += " and ael.warehouseid=:warehouseid";
					parameters.Add(new OracleParameter(":warehouseid", OracleDbType.Varchar2, 200) { Value = warehouseid });
				}
			}

			if (merchantid > 0)
			{
				strResult += " and ael.merchantid=:merchantid";
				parameters.Add(new OracleParameter(":merchantid", OracleDbType.Decimal, 4) { Value = merchantid });
			}

			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters.ToArray()).Tables[0];
		}

		public bool SetAreaExpressCompanyLeverAudit(int autoid, DateTime doDate, int auditBy, DateTime audittime)
		{
			string str = string.Format(@"update areaexpresslevel set dodate='{0}',auditstatus=1,auditby={1},audittime=sysdate
                                               where 1=1 and auditstatus=0 ", doDate, auditBy);

			List<OracleParameter> parameters = new List<OracleParameter>();
			if (autoid > 0)
			{
				str += " and autoid=:autoid";
				parameters.Add(new OracleParameter(":autoid", OracleDbType.Decimal, 4) { Value = autoid });
			}

			int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters.ToArray());
			if (rowCount == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool SetAreaExpressCompanyLeverAuditEx(int autoid, DateTime doDate, int auditBy, DateTime audittime, int auditstatus)
		{
			string str = string.Format(@"update areaexpresslevel set dodate=to_date('{0}','yyyy-mm-dd'),auditstatus=1,auditby={1},audittime=sysdate
                                               where 1=1 and auditstatus={2} ", doDate.ToString("yyyy-MM-dd"), auditBy, auditstatus);

			OracleParameter[] parameters = {
					new OracleParameter(":autoid", OracleDbType.Decimal,4)};

			if (autoid > 0)
			{
				str += " and autoid=:autoid";
				parameters[0].Value = autoid;
			}

			int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
			if (rowCount == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		//新加日志
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool AddAreaExpLevelLog(AreaExpressLevelLog model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into AreaExpressLevelLog(");
			strSql.Append("LOGID,AreaID,ExpressCompanyID,WarehouseId,AreaType,LogText,IsEnable,CreateBy,CreateTime,WareHouseType,MerchantID,ProductID,DistributionCode)");
			strSql.Append(" values (");
			strSql.Append(":LogID,:AreaID,:ExpressCompanyID,:WarehouseId,:AreaType,:LogText,:IsEnable,:CreateBy,:CreateTime,:WareHouseType,:MerchantID,:ProductID,:DistributionCode)");
			OracleParameter[] parameters = {
					new OracleParameter(":AreaID", OracleDbType.Varchar2,200),
					new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal,4),
					new OracleParameter(":WarehouseId", OracleDbType.Varchar2,80),
					new OracleParameter(":AreaType", OracleDbType.Decimal,4),
					new OracleParameter(":LogText", OracleDbType.Varchar2,500),
					new OracleParameter(":IsEnable", OracleDbType.Decimal,1),
					new OracleParameter(":CreateBy", OracleDbType.Varchar2,200),
					new OracleParameter(":CreateTime", OracleDbType.Date),
					new OracleParameter(":WareHouseType", OracleDbType.Decimal),
					new OracleParameter(":MerchantID", OracleDbType.Decimal),
					new OracleParameter(":ProductID", OracleDbType.Decimal),
                    new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100),
                    new OracleParameter(":LogID", OracleDbType.Decimal),
										};
			parameters[0].Value = model.AreaID;
			parameters[1].Value = model.ExpressCompanyID;
			parameters[2].Value = model.WarehouseId;
			parameters[3].Value = model.AreaType;
			parameters[4].Value = model.LogText;
			parameters[5].Value = model.Enable;
			parameters[6].Value = model.CreateBy;
			parameters[7].Value = model.CreateTime;
			parameters[8].Value = model.WareHouseType;
			parameters[9].Value = model.MerchantID;
			parameters[10].Value = model.ProductID;
			parameters[11].Value = model.DistributionCode;
            parameters[12].Value = GetIdNew("SEQ_AREAEXPRESSLEVELLOG");
			int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

			if (rowCount == 0)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		///  查询记录总数量
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public int GetOrderInfoCount(string sql, OracleParameter[] parameters)
		{
			return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnString, CommandType.Text, sql, parameters), 0);
		}

		#endregion

		/// <summary>
		/// 返回待生效的区域类型
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public DataTable AreaExpressCompanyLevelNum(int num, DateTime nowDate)
		{
			string strResult = @"select autoid,areaid,expresscompanyid,warehouseid,areatype,Isenable,effectareatype,dodate,auditstatus,warehousetype,merchantid,productid from areaexpresslevel ael
                                              where ael.auditstatus=1 and rownum <= :num and dodate<= :nowDate ";
		    OracleParameter[] parameters = {
                                               new OracleParameter(":num",OracleDbType.Int32){Value = num} ,
                                               new OracleParameter(":nowDate",OracleDbType.Date){Value = nowDate} 
		                                   };

			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult,parameters).Tables[0];
		}

		//更新区域类型
		public bool AreaExpressCompanyLevelUpdate(int autoid)
		{
			string str = string.Format(@"update areaexpresslevel set auditstatus=2,areatype=effectareatype
                                               where auditstatus=1 and IsEnable=1 ");

			OracleParameter[] parameters = {
					new OracleParameter(":autoid", OracleDbType.Decimal,4)};

			if (autoid > 0)
			{
				str += " and autoid=:autoid";
				parameters[0].Value = autoid;
			}
			else
			{
				return false;
			}

			int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
			if (rowCount == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool AreaExpressCompanyLevelAdd(int autoid)
		{
			string str = string.Format(@"update areaexpresslevel set auditstatus=2,Isenable=1,areatype=effectareatype
                                               where auditstatus=1 and IsEnable=3 ");

			OracleParameter[] parameters = {
					new OracleParameter(":autoid", OracleDbType.Decimal,4)};

			if (autoid > 0)
			{
				str += " and autoid=:autoid";
				parameters[0].Value = autoid;
			}
			else
			{
				return false;
			}

			int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
			if (rowCount == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool AreaExpressCompanyLevelDel(int autoid)
		{
			string str = string.Format(@"update areaexpresslevel set auditstatus=2,Isenable=0
                                               where auditstatus=1 and IsEnable=2 ");

			OracleParameter[] parameters = {
					new OracleParameter(":autoid", OracleDbType.Decimal,4)};

			if (autoid > 0)
			{
				str += " and autoid=:autoid";
				parameters[0].Value = autoid;
			}
			else
			{
				return false;
			}

			int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
			if (rowCount == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public DataSet GetExportData()
		{
            DataSet ds = new DataSet();
            sqlStr = @"WITH    t AS ( SELECT   ExpressCompanyID ,
												CompanyName ,
												CompanyAllName
									   FROM     ExpressCompany
									   WHERE    CompanyFlag in (3)
												AND ParentID=0
												AND DistributionCode <> 'rfd'
												AND IsDeleted = 0
									   UNION ALL
									   SELECT   ExpressCompanyID ,
												CompanyName ,
												CompanyAllName
									   FROM     ExpressCompany
									   WHERE    CompanyFlag = 2
												AND DistributionCode = 'rfd'
												AND IsDeleted = 0
									 )
							SELECT  *
							FROM    t";
            DataSet dsExpress = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtExpress = dsExpress.Tables[0].Copy();
            dtExpress.TableName = "dtExpress";
            ds.Tables.Add(dtExpress);

            sqlStr = @"WITH    t AS ( SELECT   to_char(w.WarehouseId)WarehouseId ,
                        to_char(w.WarehouseName) WarehouseName
                     FROM     Warehouse w
                     WHERE    w.Enable = 1
                     UNION ALL
                     SELECT   to_char('S_'||ExpressCompanyID),
                        to_char(CompanyName) 
                     FROM     ExpressCompany  ec
                     WHERE    IsDeleted = 0
                        AND CompanyFlag = 1
									 )
							SELECT  *
							FROM    t";
            DataSet dsWareHouse = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtWareHouse = dsWareHouse.Tables[0].Copy();
            dtWareHouse.TableName = "dtWareHouse";
            ds.Tables.Add(dtWareHouse);

            sqlStr = @"SELECT p.ProvinceID,
							   p.ProvinceName,
							   c.CityID,
							   c.CityName,
							   a.AreaID,
							   a.AreaName
						FROM   Province p
							   JOIN City c
									ON  c.ProvinceID = p.ProvinceID
							   JOIN Area a
									ON  a.CityID = c.CityID
						WHERE  p.IsDeleted = 0
							   AND c.IsDeleted = 0
							   AND a.IsDeleted = 0";
            DataSet dsPCA = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtPCA = dsPCA.Tables[0].Copy();
            dtPCA.TableName = "dtPCA";
            ds.Tables.Add(dtPCA);

            sqlStr = @"SELECT si.StatusNO
						FROM   StatusInfo si
						WHERE  si.StatusTypeNO = '305'
							   AND si.IsDelete = 0";
            DataSet dsAreaType = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtAreaType = dsAreaType.Tables[0].Copy();
            dtAreaType.TableName = "dtAreaType";
            ds.Tables.Add(dtAreaType);

			sqlStr = @"SELECT  ID ,
								MerchantName
						FROM    MerchantBaseInfo  mbi
						WHERE   IsDeleted = 0";
            DataSet dsMerchant = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtMerchant = dsMerchant.Tables[0].Copy();
            dtMerchant.TableName = "dtMerchant";
            ds.Tables.Add(dtMerchant);
			return ds;
		}

		//设置置回区域类型
		public bool ReSetAreaExpressCompanyLevel(int autoid, int auditby, DateTime audittime)
		{
			string str = string.Format(@"update areaexpresslevel set auditstatus=3,auditby={0},audittime=sysdate
                                               where 1=1 and auditstatus=0 ", auditby);

			OracleParameter[] parameters = {
					new OracleParameter(":autoid", OracleDbType.Decimal,4)};

			if (autoid > 0)
			{
				str += " and autoid=:autoid";
				parameters[0].Value = autoid;
			}
			else
			{
				return false;
			}

			int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, str.ToString(), parameters);
			if (rowCount == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode, PageInfo pi)
		{
			sqlStr = @"SELECT   ael.AutoID,
												ael.ExpressCompanyID ,
												ec.CompanyName ,
												p.ProvinceID ,
												p.ProvinceName ,
												c.CityID ,
												c.CityName ,
												a.AreaID ,
												a.AreaName ,
												(CASE WHEN ael.IsEnable = 1 THEN ael.AreaType
																ELSE NULL
														   END) AreaType ,
												( CASE WHEN ael.IsEnable IN ( 1, 2, 3 )
																	  THEN ael.EffectAreaType
																	  ELSE NULL
																 END ) EffectAreaType,
												ael.MerchantID,
												mbi.MerchantName ,
												ael.ProductID,
												si.StatusName AS AuditStatusStr ,
												( CASE ael.IsEnable
															  WHEN 0 THEN '已删除'
															  WHEN 1 THEN '可用'
															  WHEN 2 THEN '待删除'
															  WHEN 3 THEN '新增'
															END ) EnableStr,
												ael.WareHouseID,
												ael.WareHouseType,
												( CASE ael.WareHouseType
																  WHEN 1 THEN w.WarehouseName
																  WHEN 2 THEN w1.CompanyName
																  ELSE NULL
																END) WarehouseName
									   FROM     AreaExpressLevel  ael
												JOIN Area  a ON ael.AreaID = a.AreaID AND a.IsDeleted=0
												JOIN City  c ON a.CityID = c.CityID AND c.IsDeleted=0
												JOIN Province  p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
												JOIN ExpressCompany  ec ON ec.ExpressCompanyID = ael.ExpressCompanyID AND ec.IsDeleted=0
												JOIN MerchantBaseInfo  mbi ON mbi.ID = ael.MerchantID AND mbi.IsDeleted=0
												JOIN StatusInfo  si ON si.StatusNO = ael.AuditStatus
																			   AND si.StatusTypeNO = 306 AND si.IsDelete=0
												LEFT JOIN Warehouse  w ON w.WarehouseId = ael.WareHouseID
																					  AND ael.WareHouseType = 1 AND w.Enable=1
												LEFT JOIN ExpressCompany  w1 ON w1.ExpressCompanyID = ael.WareHouseID
																					  AND ael.WareHouseType = 2 AND w1.IsDeleted=0
												WHERE (ael.IsEnable<>0) {0}";

			StringBuilder sbWhere = new StringBuilder();
			List<OracleParameter> parameters = new List<OracleParameter>();

			if (!string.IsNullOrEmpty(provinceId))
			{
				sbWhere.Append(" AND p.ProvinceID=:ProvinceID ");
				parameters.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 20) { Value = provinceId });
			}

			if (!string.IsNullOrEmpty(cityId))
			{
				sbWhere.Append(" AND c.CityID=:CityID ");
				parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 20) { Value = cityId });
			}

			if (!string.IsNullOrEmpty(areaId))
			{
				sbWhere.Append(" AND a.AreaID=:AreaID ");
				parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) { Value = areaId });
			}

			if (!string.IsNullOrEmpty(expressCompanyId))
			{
				sbWhere.Append(" AND ael.ExpressCompanyID=:ExpressCompanyID ");
				parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = expressCompanyId });
			}

			if (!string.IsNullOrEmpty(areaType))
			{
				sbWhere.Append(" AND ael.AreaType=:AreaType ");
				parameters.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) { Value = areaType });
			}

			if (!string.IsNullOrEmpty(merchantId))
			{
				sbWhere.Append(" AND ael.MerchantID=:MerchantID ");
				parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
			}

			if (!string.IsNullOrEmpty(auditStatus))
			{
				sbWhere.Append(" AND ael.AuditStatus=:AuditStatus ");
				parameters.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) { Value = auditStatus });
			}

			if (!string.IsNullOrEmpty(distributionCode))
			{
				sbWhere.Append(" AND a.DistributionCode=:DistributionCode ");
				parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			}

			sqlStr = string.Format(sqlStr, sbWhere.ToString());
			IPagedDataTable aa = PageCommon.GetPagedData(ReadOnlyConnection, sqlStr, " ael.CreateTime DESC", new PaginatorDTO { PageSize = pi.PageSize, PageNo = pi.CurrentPageIndex },
																				  this.ToParameters(parameters.ToArray()));
			pi.ItemCount = aa.RecordCount;
			pi.PageCount = aa.PageCount;
			return aa.ContentData;

		}

		public DataTable SearchAreaTypeExprotList(string provinceId, string cityId, string areaId, string expressCompanyId, string areaType, string wareHouse, string merchantId, string auditStatus, string distributionCode)
		{
			sqlStr = @"
						SELECT   ael.AutoID AS 日志编号,
								ec.CompanyName AS 配送公司,
								p.ProvinceName  AS 省,
								c.CityName  AS 市,
								a.AreaName AS  区,
								(CASE WHEN ael.IsEnable = 1 THEN ael.AreaType
												ELSE null
										   END) 生效区域类型 ,
								(CASE WHEN ael.IsEnable IN ( 1, 2, 3 )
													  THEN ael.EffectAreaType
													  ELSE null
												 END ) 首次新增区域类型,
								mbi.MerchantName AS 商家,
								si.StatusName AS 审核状态 ,
								(CASE ael.IsEnable
											  WHEN 0 THEN '已删除'
											  WHEN 1 THEN '可用'
											  WHEN 2 THEN '待删除'
											  WHEN 3 THEN '新增'
											END) 生效状态
					   FROM     AreaExpressLevel  ael
								JOIN Area  a ON ael.AreaID = a.AreaID AND a.IsDeleted=0
								JOIN City  c ON a.CityID = c.CityID AND c.IsDeleted=0
								JOIN Province  p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
								JOIN ExpressCompany  ec ON ec.ExpressCompanyID = ael.ExpressCompanyID AND ec.IsDeleted=0
								JOIN MerchantBaseInfo  mbi ON mbi.ID = ael.MerchantID AND mbi.IsDeleted=0
								JOIN StatusInfo  si ON si.StatusNO = ael.AuditStatus
															   AND si.StatusTypeNO = 306 AND si.IsDelete=0
								LEFT JOIN Warehouse  w ON w.WarehouseId = ael.WareHouseID
																	  AND ael.WareHouseType = 1 AND w.Enable=1
								LEFT JOIN ExpressCompany  w1 ON w1.ExpressCompanyID = ael.WareHouseID
																	  AND ael.WareHouseType = 2 AND w1.IsDeleted=0
								WHERE (ael.IsEnable<>0) {0}";
			StringBuilder sbWhere = new StringBuilder();
			List<OracleParameter> parameters = new List<OracleParameter>();

			if (!string.IsNullOrEmpty(provinceId))
			{
				sbWhere.Append(" AND p.ProvinceID=:ProvinceID ");
				parameters.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 20) { Value = provinceId });
			}

			if (!string.IsNullOrEmpty(cityId))
			{
				sbWhere.Append(" AND c.CityID=:CityID ");
				parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 20) { Value = cityId });
			}

			if (!string.IsNullOrEmpty(areaId))
			{
				sbWhere.Append(" AND a.AreaID=:AreaID ");
				parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) { Value = areaId });
			}

			if (!string.IsNullOrEmpty(expressCompanyId))
			{
				sbWhere.Append(" AND ael.ExpressCompanyID=:ExpressCompanyID ");
				parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = expressCompanyId });
			}

			if (!string.IsNullOrEmpty(areaType))
			{
				sbWhere.Append(" AND ael.AreaType=:AreaType ");
				parameters.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) { Value = areaType });
			}

			if (!string.IsNullOrEmpty(merchantId))
			{
				sbWhere.Append(" AND ael.MerchantID=:MerchantID ");
				parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
			}

			if (!string.IsNullOrEmpty(auditStatus))
			{
				sbWhere.Append(" AND ael.AuditStatus=:AuditStatus ");
				parameters.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) { Value = auditStatus });
			}

			if (!string.IsNullOrEmpty(distributionCode))
			{
				sbWhere.Append(" AND a.DistributionCode=:DistributionCode ");
				parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			}

			//if (!string.IsNullOrEmpty(wareHouse))
			//{
			//    sbWhere.Append(@" AND EXISTS ( SELECT 1 FROM   :whTable wt WHERE  ael.WareHouseID = wt.HouseID and ael.WareHouseType=wt.HouseType )");

			//}
			//parameters.Add(new OracleParameter(":houseXml", OracleDbType.Varchar2, 1000) { Value = CreateHouseXml(wareHouse).InnerXml });
			sqlStr = string.Format(sqlStr, sbWhere.ToString());
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, ToParameters(parameters.ToArray())).Tables[0];
		}

		public DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string wareHouseType, string merchantId, string productId, string distributionCode)
		{
			sqlStr = @"SELECT  aell.LogID ,
        p.ProvinceName ,
        c.CityName ,
        a.AreaName ,
        ec.CompanyName ,
        mbi.MerchantName ,
        (CASE aell.CreateBy
                         WHEN '0' THEN '系统'
                         ELSE e.EmployeeName
                       END ) EmployeeName,
        aell.CreateTime ,
        aell.LogText
FROM    AreaExpressLevelLog  aell
        JOIN Area  a ON aell.AreaID = a.AreaID
                                    AND a.IsDeleted = 0
        JOIN City  c ON c.CityID = a.CityID
                                    AND c.IsDeleted = 0
        JOIN Province  p ON p.ProvinceID = c.ProvinceID
                                        AND p.IsDeleted = 0
        JOIN ExpressCompany  ec ON ec.ExpressCompanyID = aell.ExpressCompanyID
                                               AND ec.IsDeleted = 0
        JOIN MerchantBaseInfo  mbi ON mbi.ID = aell.MerchantID
                                                  AND mbi.IsDeleted = 0
        LEFT JOIN Warehouse  w ON w.WarehouseId = aell.WarehouseId
                                              AND w.Enable = 1
        LEFT JOIN ExpressCompany  w1 ON w1.ExpressCompanyID = aell.WarehouseId
                                                    AND w1.IsDeleted = 0
        LEFT JOIN employee  e ON e.EmployeeID =aell.CreateBy
		WHERE (1=1) {0} order by aell.CreateTime desc";

			StringBuilder sbWhere = new StringBuilder();
			List<OracleParameter> parameters = new List<OracleParameter>();

			if (!string.IsNullOrEmpty(areaId))
			{
				sbWhere.Append(" AND a.AreaID=:AreaID ");
				parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) { Value = areaId });
			}

			if (!string.IsNullOrEmpty(expressCompanyId))
			{
				sbWhere.Append(" AND aell.ExpressCompanyID=:ExpressCompanyID ");
				parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = expressCompanyId });
			}

			if (!string.IsNullOrEmpty(merchantId))
			{
				sbWhere.Append(" AND aell.MerchantID=:MerchantID ");
				parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
			}

			if (!string.IsNullOrEmpty(productId))
			{
				sbWhere.Append(" AND aell.ProductID=:ProductID ");
				parameters.Add(new OracleParameter(":ProductID", OracleDbType.Decimal) { Value = productId });
			}

			if (!string.IsNullOrEmpty(distributionCode))
			{
				sbWhere.Append(" AND aell.DistributionCode=:DistributionCode ");
				parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			}

			if (!string.IsNullOrEmpty(wareHouse))
			{
				if (int.Parse(wareHouseType) == 2)
					sbWhere.Append(" AND aell.WareHouseID = '" + wareHouse.Replace("S_", "") + "' AND aell.WareHouseType=2");
				else
					sbWhere.Append(" AND aell.WareHouseID = '" + wareHouse + "' AND aell.WareHouseType=1");
			}

			sqlStr = string.Format(sqlStr, sbWhere.ToString());
			DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr, this.ToParameters(parameters.ToArray()));
			if (ds == null || ds.Tables.Count <= 0)
				return null;
			else
				return ds.Tables[0];
		}

		#region cod计算服务
		public DataTable GetAreaType(int expressComapnyId, int merchantId)
		{
			string sql = @"SELECT  ael.AutoId ,
									ael.AreaID ,
									ael.ExpressCompanyID ,
									ael.WareHouseID ,
									ael.WareHouseType ,
									ael.MerchantID ,
									ael.AreaType
							FROM    AreaExpressLevel AS ael WITH(NOLOCK)
							WHERE   ael.IsEnable IN (1,2)
									AND ael.ExpressCompanyID = :ExpressCompanyID
									AND ael.MerchantID = :MerchantID";
			OracleParameter[] parameters ={
										   new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
										   new OracleParameter(":MerchantID",OracleDbType.Decimal),
									  };
			parameters[0].Value = expressComapnyId;
			parameters[1].Value = merchantId;
			return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
		}
		#endregion
	}
}
