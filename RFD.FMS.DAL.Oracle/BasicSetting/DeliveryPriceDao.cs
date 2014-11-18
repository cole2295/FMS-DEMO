using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using System.Data;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.Util.OraclePageCommon;
using RFD.FMS.MODEL;
using System.Data.SqlClient;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.BasicSetting;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
	/// <summary>
	/// COD价格维护
	/// </summary>
    public class DeliveryPriceDao : OracleDao, IDeliveryPriceDao
	{
        private string strSql = "";

        public DataTable GetMerchant(string disCode)
        {
            strSql = @"SELECT m.ID,m.MerchantName
 FROM DistributionMerchantRelation dmr
JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
AND dmr.DistributionCode=:DistributionCode
ORDER BY m.CreatTime";
            OracleParameter[] parameters ={
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = disCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

		public DataTable GetDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId, string distributionCode, int IsCod, PageInfo pi, bool isPage)
		{
			#region sql
			string str =
				@"SELECT  codl.LineID ,
                codl.CODLineNO ,
                codl.ExpressCompanyID ,
                ec.CompanyName ,
                codl.IsEchelon ,
                decode(codl.IsEchelon,1,'是','否') IsEchelonStr,
                codl.WareHouseID ,
                （CASE codl.WareHouseType
                          WHEN 1 THEN w.WarehouseName
                          WHEN 2 THEN w1.CompanyName
                          ELSE ''
                        END）WarehouseName ,
                codl.AreaType ,
                decode(codl.IsCOD,0,'否','是') IsCODStr,
                codl.PriceFormula ,
                                codl.Formula ,
                codl.LineStatus ,
                sci.CodeDesc AS LineStatusStr ,
                codl.AuditStatus ,
                sci1.CodeDesc AS AuditStatusStr ,
                codl.AuditBy ,
                codl.AuditTime ,
                codl.CreateBy ,
                codl.CreateTime ,
                codl.UpdateBy ,
                codl.UpdateTime ,
                codl.DeleteFlag ,
                mbi.MerchantName
             FROM    FMS_CODLine  codl
                JOIN StatusCodeInfo sci ON sci.CodeNo = codl.LineStatus
                                    AND sci.CodeType = 'AreaTypeLine'
                JOIN StatusCodeInfo sci1 ON sci1.CodeNo = codl.AuditStatus
                                     AND sci1.CodeType = 'AreaTypeAudit'
                JOIN ExpressCompany ec ON ec.ExpressCompanyID = codl.ExpressCompanyID
                LEFT JOIN Warehouse w ON w.WarehouseId = codl.WarehouseId
                LEFT JOIN ExpressCompany w1 ON w1.ExpressCompanyID = codl.WarehouseId
                                      AND w1.IsDeleted = 0
                                      AND w1.CompanyFlag = 1
                JOIN MerchantBaseInfo mbi ON mbi.ID=codl.MerchantID
            WHERE   ( codl.DeleteFlag = 0 ) {0} ";
			
			#endregion

			StringBuilder sbWhere = new StringBuilder();
			List<OracleParameter> parameters = new List<OracleParameter>();
			if (!string.IsNullOrEmpty(expressCompanyId))
			{
				sbWhere.Append(" AND codl.ExpressCompanyID=:ExpressCompanyID ");
				parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = expressCompanyId });
			}

			if (!string.IsNullOrEmpty(lineStatus))
			{
				sbWhere.Append(" AND codl.LineStatus=:LineStatus ");
				parameters.Add(new OracleParameter(":LineStatus", OracleDbType.Decimal) { Value = lineStatus });
			}

			if (!string.IsNullOrEmpty(auditStatus))
			{
				sbWhere.Append(" AND codl.AuditStatus=:AuditStatus ");
				parameters.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) { Value = auditStatus });
			}

			if (!string.IsNullOrEmpty(areaType))
			{
				sbWhere.Append(" AND codl.AreaType=:AreaType ");
				parameters.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) { Value = areaType });
			}

			if (!string.IsNullOrEmpty(wareHouse))
			{
				sbWhere.Append(" AND codl.WareHouseID=:WareHouseID ");
				parameters.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2, 40) { Value = wareHouse });
			}

			if (!string.IsNullOrEmpty(wareHouseType))
			{
				sbWhere.Append(" AND codl.WareHouseType=:WareHouseType ");
				parameters.Add(new OracleParameter(":WareHouseType", OracleDbType.Decimal) { Value = wareHouseType });
			}

			if (!string.IsNullOrEmpty(merchantId))
			{
				sbWhere.Append(" AND codl.MerchantID=:MerchantID ");
				parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
			}

			if (!string.IsNullOrEmpty(distributionCode))
			{
				sbWhere.Append(" AND codl.DistributionCode=:DistributionCode ");
				parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			}

			if (IsCod > -1)
			{
				sbWhere.Append(" AND codl.IsCOD=:IsCOD ");
				parameters.Add(new OracleParameter(":IsCOD", OracleDbType.Decimal) { Value = IsCod });
			}
			
			str = string.Format(str, sbWhere);

			if (isPage)
			{
				IPagedDataTable aa = PageCommon.GetPagedData(ReadOnlyConnection, str, " codl.CODLineNO DESC", new PaginatorDTO { PageSize = pi.PageSize, PageNo = pi.CurrentPageIndex },
													   this.ToParameters(parameters.ToArray()));
				pi.ItemCount = aa.RecordCount;
				pi.PageCount = aa.PageCount;
				return aa.ContentData;
			}
			DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, str, this.ToParameters(parameters.ToArray()));
			if (ds == null || ds.Tables.Count <= 0)
			{
				return null;
			}
			return ds.Tables[0];
			
		}

        public DataTable GetListByCodLineNo(string codLineNo)
        {
            strSql = @"SELECT  codl.LineID ,
								codl.CODLineNO ,
								codl.ExpressCompanyID ,
								ec.CompanyName ,
								codl.IsEchelon ,
								(CASE WareHouseType
												WHEN 1 THEN codl.WareHouseID
												WHEN 2 THEN 'S_' || codl.WareHouseID
												ELSE ''
											  END ) WareHouseID,
								codl.AreaType ,
								codl.IsCOD,
								codl.PriceFormula ,
                                codl.Formula ,
								codl.LineStatus ,
								codl.AuditStatus ,
								codl.MerchantID ,
								codl.ProductID,
                                codl.DistributionCode,
                                mbi.MerchantName
						FROM    FMS_CODLine  codl
								JOIN ExpressCompany ec ON ec.ExpressCompanyID = codl.ExpressCompanyID
                                JOIN MerchantBaseInfo mbi ON mbi.ID = codl.MerchantID
						WHERE   ( DeleteFlag = 0 ) and codl.CODLineNO=:CODLineNO";
            OracleParameter[] parameters = {
                                            new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
                                       };
            parameters[0].Value = codLineNo;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public int AddDeliveryPrice(FMS_CODLine codLine, string cODLineNO)
        {
            if (string.IsNullOrEmpty(codLine.WareHouseID))
            {
            	strSql = @"SELECT count(1) From FMS_CODLine  
								WHERE ExpressCompanyID={0} AND MerchantID={1} AND AreaType={2} AND DeleteFlag=0 AND DistributionCode='{3}'";
				strSql = string.Format(strSql, codLine.ExpressCompanyID,codLine.MerchantID,codLine.AreaType,codLine.DistributionCode);
            }
            else
            {
				strSql = @"SELECT count(1) From FMS_CODLine  
								WHERE ExpressCompanyID={0} AND MerchantID={1} AND AreaType={2} AND DeleteFlag=0 AND DistributionCode='{3}' AND WareHouseID='{4}' AND IsEchelon={5} ";
				strSql = string.Format(strSql, codLine.ExpressCompanyID, codLine.MerchantID, codLine.AreaType, codLine.DistributionCode,codLine.WareHouseID,codLine.IsEchelon);
                
            }

			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql, null);
			if (obj != null)
			{
				if (Convert.ToInt32(obj) > 0)
				{
					return 0;
				}
			}
			strSql = @"INSERT  INTO FMS_CODLine
										( LINEID,CODLineNO ,
										  ExpressCompanyID ,
										  IsEchelon ,
										  WareHouseID ,
										  AreaType ,
										  PriceFormula ,
										  LineStatus ,
										  AuditStatus ,
										  AuditBy ,
										  AuditTime ,
										  CreateBy ,
										  CreateTime ,
										  UpdateBy ,
										  UpdateTime,
										  WareHouseType,
										  MerchantID,
										  ProductID,
                                          DistributionCode,
                                          IsCOD,
                                          Formula,
										  DELETEFLAG,
                                          IsChange
										)
								VALUES  ( seq_fms_codline.nextval,:CODLineNO ,
										  :ExpressCompanyID ,
										  :IsEchelon ,
										  :WareHouseID ,
										  :AreaType ,
										  :PriceFormula ,
										  :LineStatus ,
										  :AuditStatus ,
										  '0' ,
										  SysDate ,
										  :CreateBy ,
										  SysDate ,
										  '0' ,
										  SysDate,
										  :WareHouseType,
										  :MerchantID,
										  :ProductID,
                                          :DistributionCode	,
                                          :IsCOD,
                                          :Formula,
										  0,
                                          :IsChange
										)";
            OracleParameter[] parameters = {
			                          		new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
											new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
											new OracleParameter(":IsEchelon",OracleDbType.Int32),
											new OracleParameter(":WareHouseID",OracleDbType.Varchar2,40),
											new OracleParameter(":AreaType",OracleDbType.Int32),
											new OracleParameter(":PriceFormula",OracleDbType.Varchar2,400),
											new OracleParameter(":LineStatus",OracleDbType.Int32),
											new OracleParameter(":AuditStatus",OracleDbType.Int32),
											new OracleParameter(":CreateBy",OracleDbType.Varchar2,80),
											new OracleParameter(":WareHouseType",OracleDbType.Decimal),
											new OracleParameter(":MerchantID",OracleDbType.Decimal),
											new OracleParameter(":ProductID",OracleDbType.Decimal),
                                            new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                            new OracleParameter(":IsCOD",OracleDbType.Decimal),
                                            new OracleParameter(":Formula",OracleDbType.Varchar2,300),
                                            new OracleParameter(":IsChange",OracleDbType.Decimal),
										};

            parameters[0].Value = cODLineNO;
            parameters[1].Value = codLine.ExpressCompanyID;
            parameters[2].Value = codLine.IsEchelon;
            parameters[3].Value = codLine.WareHouseID;
            parameters[4].Value = codLine.AreaType;
            parameters[5].Value = codLine.PriceFormula;
            parameters[6].Value = codLine.LineStatus;
            parameters[7].Value = codLine.AuditStatus;
            parameters[8].Value = codLine.CreateBy;
            parameters[9].Value = codLine.WareHouseType;
            parameters[10].Value = codLine.MerchantID;
            parameters[11].Value = codLine.ProductID;
            parameters[12].Value = codLine.DistributionCode;
            parameters[13].Value = codLine.IsCOD;
            parameters[14].Value = codLine.Formula;
            parameters[15].Value = 1;
        	if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters)!=1)
        	{
        		return -1;
        	}
			return 1;
        }

        public bool UpdateDeliveryPrice(FMS_CODLine codLine)
        {
            strSql = @"UPDATE  FMS_CODLine
							SET     ExpressCompanyID = :ExpressCompanyID ,
									IsEchelon = :IsEchelon ,
									WareHouseID = :WareHouseID ,
									WareHouseType=:WareHouseType,
									MerchantID=:MerchantID,
									ProductID=:ProductID,
									AreaType = :AreaType ,
                                    IsCOD = :IsCOD ,
									PriceFormula = :PriceFormula ,
                                    Formula = :Formula ,
									LineStatus = :LineStatus ,
									UpdateBy = :UpdateBy ,
									UpdateTime = SysDate,
									AuditStatus=2,
									AuditBy=0,
									AuditTime=SysDate,
                                    IsChange=1
							WHERE   CODLineNO = :CODLineNO AND DeleteFlag=0";
            OracleParameter[] parameters ={
										   new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
										   new OracleParameter(":IsEchelon",OracleDbType.Decimal),
										   new OracleParameter(":WareHouseID",OracleDbType.Varchar2,80),
										   new OracleParameter(":AreaType",OracleDbType.Decimal),
                                           new OracleParameter(":IsCOD",OracleDbType.Decimal),
										   new OracleParameter(":PriceFormula",OracleDbType.Varchar2,300),
                                           new OracleParameter(":Formula",OracleDbType.Varchar2,300),
										   new OracleParameter(":LineStatus",OracleDbType.Decimal),
										   new OracleParameter(":UpdateBy",OracleDbType.Varchar2,200),
										   new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
										   new OracleParameter(":WareHouseType",OracleDbType.Decimal),
										   new OracleParameter(":MerchantID",OracleDbType.Decimal),
										   new OracleParameter(":ProductID",OracleDbType.Decimal),
									  };
            parameters[0].Value = codLine.ExpressCompanyID;
            parameters[1].Value = codLine.IsEchelon;
            parameters[2].Value = codLine.WareHouseID;
            parameters[3].Value = codLine.AreaType;
            parameters[4].Value = codLine.IsCOD;
            parameters[5].Value = codLine.PriceFormula;
            parameters[6].Value = codLine.Formula;
            parameters[7].Value = codLine.LineStatus;
            parameters[8].Value = codLine.UpdateBy;
            parameters[9].Value = codLine.CODLineNO;
            parameters[10].Value = codLine.WareHouseType;
            parameters[11].Value = codLine.MerchantID;
            parameters[12].Value = codLine.ProductID;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public bool DeleteDeliveryPrice(string cODLineNO, string updateBy)
        {
            strSql = @"UPDATE FMS_CODLine SET    DeleteFlag = 1,UpdateBy = :UpdateBy, UpdateTime = SysDate,IsChange=1
												WHERE CODLineNO=:CODLineNO";
            OracleParameter[] parameters ={
										   new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
										   new OracleParameter(":UpdateBy",OracleDbType.Varchar2,200)
									  };
            parameters[0].Value = cODLineNO;
            parameters[1].Value = updateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public bool UpdateDeliveryPriceAuditStatus(string cODLineNO, string auditBy, int auditStatus)
        {
            strSql = @"UPDATE FMS_CODLine SET    AuditStatus = :AuditStatus,AuditBy = :AuditBy, AuditTime = SysDate,IsChange=1
												WHERE CODLineNO=:CODLineNO AND DeleteFlag=0";
            OracleParameter[] parameters ={
										   new OracleParameter(":AuditStatus",OracleDbType.Decimal),
										   new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
										   new OracleParameter(":AuditBy",OracleDbType.Varchar2,200)
									  };
            parameters[0].Value = auditStatus;
            parameters[1].Value = cODLineNO;
            parameters[2].Value = auditBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        /// <summary>
        /// 删除日志增加
        /// </summary>
        /// <returns></returns>
        public bool AddDeliveryPriceLog(string codLineNo, string createBy, string logText, int logType)
        {
            strSql = @"INSERT INTO FMS_CODOperatorLog
							   (logid,
							   PK_NO
							   ,CreateBy
							   ,CreateTime
							   ,LogText
							   ,LogType
                               ,IsChange)
						 VALUES
							   (seq_fms_codoperatorlog.nextval,
							   :PK_NO
							   ,:CreateBy
							   ,SysDate
							   ,:LogText
								,:LogType
                                ,:IsChange)";
            OracleParameter[] parameters ={
										   new OracleParameter(":PK_NO",OracleDbType.Varchar2,40),
										   new OracleParameter(":CreateBy",OracleDbType.Varchar2,80),
										   new OracleParameter(":LogText",OracleDbType.Varchar2,500),
										   new OracleParameter(":LogType",OracleDbType.Decimal),
                                           new OracleParameter(":IsChange",OracleDbType.Decimal),
									  };
            parameters[0].Value = codLineNo;
            parameters[1].Value = createBy;
            parameters[2].Value = logText;
            parameters[3].Value = logType;
            parameters[4].Value = 1;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public DataTable GetDeliveryPriceHistoryList(string lineNoXml)
        {
            string distinctColumnSql = @"with t as (select distinct bakyear||bakmonth as StatColumn from fms_codlinehistory) select * from t order by StatColumn";
            DataTable dtColumn = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, distinctColumnSql).Tables[0];
            if (dtColumn == null || dtColumn.Rows.Count <= 0)
                return null;

            StringBuilder sbSearchSql = new StringBuilder();
            sbSearchSql.Append("SELECT CODLineNO as 线路编号, ");
            int n = 1;
            foreach (DataRow dr in dtColumn.Rows)
            {
                sbSearchSql.Append("max(case when bakyear||bakmonth='" + dr["StatColumn"] + "' then PriceFormula end) \"" + dr["StatColumn"] + "\",");
                //if (n != dtColumn.Rows.Count)
                //    sbSearchSql.Append(",");
                sbSearchSql.Append("max(case when bakyear||bakmonth='" + dr["StatColumn"] + "' then Formula end) \"" + dr["StatColumn"] + "非\"");
                if (n != dtColumn.Rows.Count)
                    sbSearchSql.Append(",");
                n++;

            }
            sbSearchSql.Append(" FROM FMS_CODLineHistory ");
            if (!string.IsNullOrEmpty(lineNoXml))
                sbSearchSql.Append(" WHERE CODLineNO in (" + lineNoXml + ")");
            sbSearchSql.Append(" GROUP BY CODLineNO");
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sbSearchSql.ToString()).Tables[0];
        }

        public DataTable GetDeliveryPriceLog(string lineNo, string dateStr, string dateEnd, string distributionCode)
        {
            strSql = @"SELECT fcl.LogID,
							   fcl.PK_NO,
							   fcl.CreateBy,
							   fcl.CreateTime,
							   fcl.LogText
						FROM   FMS_CODOperatorLog fcl
                        JOIN FMS_CODLine fcl1 ON fcl.PK_NO=fcl1.CODLineNO
                        WHERE (1=1)
                        {0} order by fcl.CreateTime desc";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(lineNo))
            {
                sbWhere.Append(" AND fcl.PK_NO=:PK_NO ");
                parameters.Add(new OracleParameter(":PK_NO", OracleDbType.Varchar2,40) { Value = lineNo });
            }

            if (!string.IsNullOrEmpty(dateStr))
            {
				//sbWhere.Append(" AND fcl.CreateTime>=:dateStr ");//to_date('2012-03-01 10:00:00','yyyy-mm-dd hh24:mi:ss')
				//parameters.Add(new OracleParameter(":dateStr",OracleDbType.Date) { Value = dateStr });
            	sbWhere.Append(" AND fcl.CreateTime>= to_date('" + dateStr + "','yyyy-mm-dd hh24:mi:ss')");
            }

            if (!string.IsNullOrEmpty(dateEnd))
            {
                //parameters.Add(new OracleParameter(":dateEnd", OracleDbType.Date) { Value = dateEnd });
				sbWhere.Append(" AND fcl.CreateTime<= to_date('" + dateEnd + "','yyyy-mm-dd hh24:mi:ss')");
            }

            sbWhere.Append(" AND fcl.LogType=1 ");

            strSql = string.Format(strSql, sbWhere.ToString());
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public int AddEffectDeliveryPrice(FMS_CODLine codLine)
        {
			if (string.IsNullOrEmpty(codLine.CODLineNO) || string.IsNullOrEmpty(codLine.DistributionCode))
			{
				return -1;
			}

			strSql = @"SELECT count(1) From FMS_CODLineWaitEffect  WHERE CODLineNO='{0}' AND DeleteFlag=0 AND DistributionCode='{1}'";
			strSql = string.Format(strSql, codLine.CODLineNO, codLine.DistributionCode);
			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql, null);
			if (obj != null)
			{
				if (Convert.ToInt32(obj) > 0)
				{
					return 0;
				}
			}
			strSql = @"			INSERT  INTO FMS_CODLineWaitEffect
									( LINEID,
									  CODLineNO ,
									  ExpressCompanyID ,
									  IsEchelon ,
									  WareHouseID ,
									  AreaType ,
									  PriceFormula ,
									  LineStatus ,
									  AuditStatus ,
									  AuditBy ,
									  AuditTime ,
									  CreateBy ,
									  CreateTime ,
									  UpdateBy ,
									  UpdateTime,
									  EffectDate,
									  WareHouseType,								
									  MerchantID,
									  ProductID,
                                      DistributionCode,
                                      IsCOD,
                                      Formula,
                                      IsChange,
                                      DELETEFLAG
									)
							VALUES  ( SEQ_FMS_CODLINEWAITEFFect.Nextval,
							          :CODLineNO ,
									  :ExpressCompanyID ,
									  :IsEchelon ,
									  :WareHouseID ,
									  :AreaType ,
									  :PriceFormula ,
									  :LineStatus ,
									  :AuditStatus ,
									  '0' ,
									  SysDate ,
									  :CreateBy ,
									  SysDate ,
									  '0' ,
									  SysDate,
									  :EffectDate,
									  :WareHouseType,								
									  :MerchantID,
									  :ProductID,
                                      :DistributionCode,
                                      :IsCOD,
                                      :Formula,
                                      1,
                                      0
									)
							 ";

			OracleParameter[] parameters = {
			                          		new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
											new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
											new OracleParameter(":IsEchelon",OracleDbType.Int16),
											new OracleParameter(":WareHouseID",OracleDbType.Varchar2,40),
											new OracleParameter(":AreaType",OracleDbType.Int16),
											new OracleParameter(":PriceFormula",OracleDbType.Varchar2,400),
											new OracleParameter(":LineStatus",OracleDbType.Int16),
											new OracleParameter(":AuditStatus",OracleDbType.Int16),
											new OracleParameter(":CreateBy",OracleDbType.Varchar2,80),
											new OracleParameter(":EffectDate",OracleDbType.Date),
											new OracleParameter(":WareHouseType",OracleDbType.Decimal),
											new OracleParameter(":MerchantID",OracleDbType.Decimal),
											new OracleParameter(":ProductID",OracleDbType.Decimal),
                                            new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                            new OracleParameter(":IsCOD",OracleDbType.Decimal),
                                            new OracleParameter(":Formula",OracleDbType.Varchar2,300),
										};

			parameters[0].Value = codLine.CODLineNO;
			parameters[1].Value = codLine.ExpressCompanyID;
			parameters[2].Value = codLine.IsEchelon;
			parameters[3].Value = codLine.WareHouseID;
			parameters[4].Value = codLine.AreaType;
			parameters[5].Value = codLine.PriceFormula;
			parameters[6].Value = codLine.LineStatus;
			parameters[7].Value = codLine.AuditStatus;
			parameters[8].Value = codLine.CreateBy;
			parameters[9].Value = codLine.EffectDate;
			parameters[10].Value = codLine.WareHouseType;
			parameters[11].Value = codLine.MerchantID;
			parameters[12].Value = codLine.ProductID;
			parameters[13].Value = codLine.DistributionCode;
			parameters[14].Value = codLine.IsCOD;
			parameters[15].Value = codLine.Formula;
			if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) != 1)
			{
				return -1;
			}
			return 1;
        }

        public DataTable GetListByEffectCodLineNo(string codLineNo)
        {
            strSql = @"SELECT  codl.LineID ,
								codl.CODLineNO ,
								codl.ExpressCompanyID ,
								ec.CompanyName ,
								codl.IsEchelon ,
								(CASE WareHouseType
												WHEN 1 THEN codl.WareHouseID
												WHEN 2 THEN 'S_'|| codl.WareHouseID
												ELSE ''
											  END) WareHouseID ,
								codl.AreaType ,
								codl.IsCOD,
								codl.PriceFormula ,
                                codl.Formula ,
								codl.LineStatus ,
								codl.AuditStatus ,
								codl.EffectDate ,
								codl.MerchantID ,
								codl.ProductID,
                                codl.DistributionCode,
                                mbi.MerchantName
						FROM    FMS_CODLineWaitEffect  codl
								JOIN ExpressCompany ec ON ec.ExpressCompanyID = codl.ExpressCompanyID
                                JOIN MerchantBaseInfo mbi ON mbi.ID = codl.MerchantID
						WHERE   ( DeleteFlag = 0 ) AND codl.CODLineNO=:CODLineNO";

            OracleParameter[] parameters ={
                                              new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
                                         };
            parameters[0].Value = codLineNo;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql, parameters).Tables[0];
        }

        public bool UpdateEffectCodLine(FMS_CODLine codLine)
        {
            strSql = @"UPDATE  FMS_CODLineWaitEffect
							SET     ExpressCompanyID = :ExpressCompanyID ,
									IsEchelon = :IsEchelon ,
									WareHouseID = :WareHouseID ,
									WareHouseType=:WareHouseType,
									MerchantID=:MerchantID,
									ProductID=:ProductID,
									AreaType = :AreaType ,
                                    IsCOD=:IsCOD,
									PriceFormula = :PriceFormula ,
                                    Formula = :Formula ,
									LineStatus = :LineStatus ,
									UpdateBy = :UpdateBy ,
									UpdateTime = SysDate,
									AuditStatus=2,
									AuditBy='0',
									AuditTime=SysDate,
									EffectDate=:EffectDate,
                                    IsChange=1
							WHERE   CODLineNO = :CODLineNO AND DeleteFlag=0";
            OracleParameter[] parameters ={
										   new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
										   new OracleParameter(":IsEchelon",OracleDbType.Decimal),
										   new OracleParameter(":WareHouseID",OracleDbType.Varchar2,80),
										   new OracleParameter(":AreaType",OracleDbType.Decimal),
                                           new OracleParameter(":IsCOD",OracleDbType.Decimal),
										   new OracleParameter(":PriceFormula",OracleDbType.Varchar2,300),
                                           new OracleParameter(":Formula",OracleDbType.Varchar2,300),
										   new OracleParameter(":LineStatus",OracleDbType.Decimal),
										   new OracleParameter(":UpdateBy",OracleDbType.Varchar2,200),
										   new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
										   new OracleParameter(":EffectDate",OracleDbType.Date),
										   new OracleParameter(":WareHouseType",OracleDbType.Decimal),
										   new OracleParameter(":MerchantID",OracleDbType.Decimal),
										   new OracleParameter(":ProductID",OracleDbType.Decimal),
									  };
            parameters[0].Value = codLine.ExpressCompanyID;
            parameters[1].Value = codLine.IsEchelon;
            parameters[2].Value = codLine.WareHouseID;
            parameters[3].Value = codLine.AreaType;
            parameters[4].Value = codLine.IsCOD;
            parameters[5].Value = codLine.PriceFormula;
            parameters[6].Value = codLine.Formula;
            parameters[7].Value = codLine.LineStatus;
            parameters[8].Value = codLine.UpdateBy;
            parameters[9].Value = codLine.CODLineNO;
            parameters[10].Value = codLine.EffectDate;
            parameters[11].Value = codLine.WareHouseType;
            parameters[12].Value = codLine.MerchantID;
            parameters[13].Value = codLine.ProductID;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

		public DataTable GetEffectDeliveryPriceList(string expressCompanyId, string lineStatus, string auditStatus, string areaType, string wareHouse, string wareHouseType, bool waitEffect, string merchantId, string distributionCode, int IsCod, PageInfo pi, bool isPage)
		{
			#region sql

			string str =
				@"SELECT        codl.LineID ,
								codl.CODLineNO ,
								codl.ExpressCompanyID ,
								ec.CompanyName,
								codl.IsEchelon ,
								(CASE WHEN codl.IsEchelon=1 THEN '是' ELSE '否' END) IsEchelonStr,
								codl.WareHouseID ,
								(CASE codl.WareHouseType
												  WHEN 1 THEN w.WarehouseName
												  WHEN 2 THEN w1.CompanyName
												  ELSE ''
												END ) WarehouseName,
								codl.AreaType ,
                                (CASE WHEN codl.IsCOD=0 THEN '否' else '是' end) IsCODStr,
								codl.PriceFormula ,
                                codl.Formula ,
								codl.LineStatus ,
								sci.CodeDesc AS LineStatusStr ,
								codl.AuditStatus ,
								sci1.CodeDesc AS AuditStatusStr ,
								codl.AuditBy ,
								codl.AuditTime ,
								codl.CreateBy ,
								codl.CreateTime ,
								codl.UpdateBy ,
								codl.UpdateTime ,
								codl.DeleteFlag,
								codl.EffectDate,
								mbi.MerchantName
						FROM    FMS_CODLineWaitEffect  codl
						JOIN StatusCodeInfo sci ON sci.CodeNo=codl.LineStatus AND sci.CodeType='AreaTypeLine'
						JOIN StatusCodeInfo sci1 ON sci1.CodeNo=codl.AuditStatus AND sci1.CodeType='AreaTypeAudit'
						JOIN ExpressCompany ec ON ec.ExpressCompanyID=codl.ExpressCompanyID
						LEFT JOIN Warehouse w ON w.WarehouseId=codl.WarehouseId
						LEFT JOIN ExpressCompany w1 ON w1.ExpressCompanyID = codl.WarehouseId
																		  AND w1.IsDeleted = 0
																		  AND w1.CompanyFlag = 1
						JOIN MerchantBaseInfo mbi ON mbi.ID=codl.MerchantID
						WHERE (codl.DeleteFlag=0) {0} ";

			#endregion
			StringBuilder sbWhere = new StringBuilder();
			List<OracleParameter> parameters = new List<OracleParameter>();
			if (!string.IsNullOrEmpty(expressCompanyId))
			{
				sbWhere.Append(" AND codl.ExpressCompanyID=:ExpressCompanyID ");
				parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = expressCompanyId });
			}

			if (!string.IsNullOrEmpty(lineStatus))
			{
				sbWhere.Append(" AND codl.LineStatus=:LineStatus ");
				parameters.Add(new OracleParameter(":LineStatus", OracleDbType.Decimal) { Value = lineStatus });
			}

			if (!string.IsNullOrEmpty(auditStatus))
			{
				sbWhere.Append(" AND codl.AuditStatus=:AuditStatus ");
				parameters.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) { Value = auditStatus });
			}

			if (!string.IsNullOrEmpty(areaType))
			{
				sbWhere.Append(" AND codl.AreaType=:AreaType ");
				parameters.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) { Value = areaType });
			}

			if (!string.IsNullOrEmpty(wareHouse))
			{
				sbWhere.Append(" AND codl.WareHouseID=:WareHouseID ");
				parameters.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2, 40) { Value = wareHouse });
			}

			if (!string.IsNullOrEmpty(wareHouseType))
			{
				sbWhere.Append(" AND codl.WareHouseType=:WareHouseType ");
				parameters.Add(new OracleParameter(":WareHouseType", OracleDbType.Decimal) { Value = wareHouseType });
			}

			if (!string.IsNullOrEmpty(merchantId))
			{
				sbWhere.Append(" AND codl.MerchantID=:MerchantID ");
				parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
			}

			if (!string.IsNullOrEmpty(distributionCode))
			{
				sbWhere.Append(" AND codl.DistributionCode=:DistributionCode ");
				parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
			}

			if (IsCod > -1)
			{
				sbWhere.Append(" AND codl.IsCOD=:IsCOD ");
				parameters.Add(new OracleParameter(":IsCOD", OracleDbType.Decimal) { Value = IsCod });
			}

			str = string.Format(str, sbWhere);

			if (isPage)
			{
				IPagedDataTable aa = PageCommon.GetPagedData(ReadOnlyConnection, str, " codl.CODLineNO DESC", new PaginatorDTO { PageSize = pi.PageSize, PageNo = pi.CurrentPageIndex },
													   this.ToParameters(parameters.ToArray()));
				pi.ItemCount = aa.RecordCount;
				pi.PageCount = aa.PageCount;
				return aa.ContentData;
			}
			DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, str, this.ToParameters(parameters.ToArray()));
			if (ds == null || ds.Tables.Count <= 0)
			{
				return null;
			}
			return ds.Tables[0];
			
		}

        public bool UpdateEffectDeliveryPriceAuditStatus(string cODLineNO, string auditBy, int auditStatus)
        {
            strSql = @"UPDATE FMS_CODLineWaitEffect SET    AuditStatus = :AuditStatus,AuditBy = :AuditBy, AuditTime = SysDate,IsChange=1
												WHERE CODLineNO=:CODLineNO AND DeleteFlag=0";
            OracleParameter[] parameters ={
										   new OracleParameter(":AuditStatus",OracleDbType.Decimal),
										   new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
										   new OracleParameter(":AuditBy",OracleDbType.Varchar2,200)
									  };
            parameters[0].Value = auditStatus;
            parameters[1].Value = cODLineNO;
            parameters[2].Value = auditBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql, parameters) > 0;
        }

        public DataSet GetExportData()
        {
            DataSet ds=new DataSet();
            strSql = @"with t as(
						select ExpressCompanyID,CompanyName,CompanyAllName from ExpressCompany where CompanyFlag in (1,2,3) and DistributionCode<>'rfd' and IsDeleted=0
						union all
						select ExpressCompanyID,CompanyName,CompanyAllName from ExpressCompany where CompanyFlag=2 and DistributionCode='rfd' and IsDeleted=0)
						select * from t";
            DataSet dsExpress = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql);
            DataTable dtExpress = dsExpress.Tables[0].Copy();
            dtExpress.TableName = "dtExpress";
            ds.Tables.Add(dtExpress);

            strSql = @"WITH    t AS ( SELECT to_char(w.WarehouseId) WarehouseId,
       to_char(w.WarehouseName) WarehouseName
  FROM Warehouse w
 WHERE w.Enable = 1
UNION ALL
SELECT to_char('S_' || ExpressCompanyID), to_char(CompanyName)
  FROM ExpressCompany ec
 WHERE IsDeleted = 0
   AND CompanyFlag = 1
									 )
							SELECT  *
							FROM    t";
            DataSet dsWarehouse = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql);
            DataTable dtWarehouse = dsWarehouse.Tables[0].Copy();
            dtWarehouse.TableName = "dtWarehouse";
            ds.Tables.Add(dtWarehouse);

            strSql = @"
						SELECT sci.CodeNo
						FROM   StatusCodeInfo sci
						WHERE  sci.CodeType = 'AreaType'
							   AND sci.Enabled = 1";
            DataSet dsAreaType = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql);
            DataTable dtAreaType = dsAreaType.Tables[0].Copy();
            dtAreaType.TableName = "dtAreaType";
            ds.Tables.Add(dtAreaType);

            strSql = @"SELECT  ID ,
								MerchantName
						FROM    MerchantBaseInfo  mbi
						WHERE   IsDeleted = 0";
            DataSet dsMerchant = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strSql);
            DataTable dtMerchant = dsMerchant.Tables[0].Copy();
            dtMerchant.TableName = "dtMerchant";
            ds.Tables.Add(dtMerchant);
            return ds;
        }

        #region 配送费计算查询
        /// <summary>
        /// 获取可用的Cod线路价格
        /// </summary>
        /// <returns></returns>
        public DataTable GetCodLine(int expressCompanyId, int areaType, string distributionCode)
        {
            string sql = @" SELECT fcl.CODLineNO,
                            fcl.ExpressCompanyID,
                            fcl.IsEchelon,
                            fcl.WareHouseID,
                            fcl.AreaType,
                            fcl.PriceFormula,
                            fcl.IsCOD,
                            fcl.Formula,
                            fcl.WareHouseType,
                            fcl.MerchantID,
                            fcl.DistributionCode
                    FROM   FMS_CODLine fcl
                    WHERE  fcl.AuditStatus = 1
                            AND fcl.LineStatus = 1
                            AND fcl.DeleteFlag = 0
                            AND fcl.ExpressCompanyID = :ExpressCompanyID
                            AND fcl.AreaType = :AreaType
                            AND fcl.DistributionCode = :DistributionCode";

            OracleParameter[] parameters ={
                                           new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
                                           new OracleParameter(":AreaType",OracleDbType.Decimal),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                      };
            parameters[0].Value = expressCompanyId;
            parameters[1].Value = areaType;
            parameters[2].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 返回当前月前4个月的历史
        /// </summary>
        /// <returns></returns>
        public DataTable GetCodLineHistory(int year, int month, int expressCompanyId, int areaType, string distributionCode)
        {
            string sql = @"SELECT tch.CODLineNO,
                                   tch.ExpressCompanyID,
                                   tch.IsEchelon,
                                   tch.WareHouseID,
                                   tch.AreaType,
                                   tch.PriceFormula,
                                   tch.IsCOD,
                                   tch.Formula,
                                   tch.BakYear,
                                   tch.BakMonth,
                                   tch.WareHouseType,
                                   tch.MerchantID,
                                   tch.DistributionCode
                            FROM   FMS_CODLineHistory tch
                            WHERE  tch.BakYear = :year
                                   AND tch.BakMonth = :month
                                   AND AuditStatus = 1
                                   AND LineStatus = 1
                                   AND DeleteFlag = 0
                                   AND ExpressCompanyID = :ExpressCompanyID
                                   AND AreaType = :AreaType
                                   AND DistributionCode = :DistributionCode";
            OracleParameter[] parameters ={
                                           new OracleParameter(":year",OracleDbType.Varchar2),
                                           new OracleParameter(":month",OracleDbType.Varchar2),
                                           new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
                                           new OracleParameter(":AreaType",OracleDbType.Decimal),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100)
                                      };
            parameters[0].Value = year;
            parameters[1].Value = month > 9 ? month.ToString() : "0" + month;
            parameters[2].Value = expressCompanyId;
            parameters[3].Value = areaType;
            parameters[4].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }
        #endregion

        #region cod价格变更校对

        /// <summary>
        /// 获取价格变更记录
        /// </summary>
        /// <param name="effectDate"></param>
        /// <returns></returns>
        public List<FMS_CODLine> GetEffectCodLine(DateTime effectDateStr, DateTime effectDateEnd)
        {
            string sql = @"SELECT tc.CODLineNO,
                           tc.ExpressCompanyID,
                           tc.IsEchelon,
                           tc.WareHouseID,
                           tc.AreaType,
                           tc.PriceFormula,
                           tc.IsCOD,
                           tc.Formula
                    FROM   FMS_CODLine tc
                    WHERE  tc.AuditTime >= :AuditTimeStr
                           AND tc.AuditTime < :AuditTimeEnd
                           AND tc.AuditStatus = 1
                           AND tc.LineStatus = 1
                           AND tc.DeleteFlag = 0";

            OracleParameter[] parameters ={
                                           new OracleParameter(":AuditTimeStr",OracleDbType.Date),
                                           new OracleParameter(":AuditTimeEnd",OracleDbType.Date)
                                      };
            parameters[0].Value = effectDateStr;
            parameters[1].Value = effectDateEnd;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters);
            if (ds.Tables.Count > 0)
                return TransEffectAreaTypeObjectList(ds.Tables[0]);
            else
                return null;
        }

        private List<FMS_CODLine> TransEffectAreaTypeObjectList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return null;

            List<FMS_CODLine> codLineList = new List<FMS_CODLine>();
            foreach (DataRow dr in dt.Rows)
            {
                FMS_CODLine codLine = new FMS_CODLine();
                if (dr["CODLineNO"] != DBNull.Value)
                    codLine.CODLineNO = dr["CODLineNO"].ToString();

                if (dr["ExpressCompanyId"] != DBNull.Value)
                    codLine.ExpressCompanyID = int.Parse(dr["ExpressCompanyId"].ToString());

                if (dr["IsEchelon"] != DBNull.Value)
                    codLine.IsEchelon = int.Parse(dr["IsEchelon"].ToString());

                if (dr["WareHouseID"] != DBNull.Value)
                    codLine.WareHouseID = dr["WareHouseID"].ToString();

                if (dr["AreaType"] != DBNull.Value)
                    codLine.AreaType = int.Parse(dr["AreaType"].ToString());

                if (dr["PriceFormula"] != DBNull.Value)
                    codLine.PriceFormula = dr["PriceFormula"].ToString();

                if (dr["IsCOD"] != DBNull.Value)
                    codLine.IsCOD = int.Parse(dr["IsCOD"].ToString());

                if (dr["Formula"] != DBNull.Value)
                    codLine.Formula = dr["Formula"].ToString();

                codLineList.Add(codLine);
            }
            return codLineList;
        }
        #endregion

        #region CODLine备份

        public bool Insert(IList<FMS_CODLine> codLineList, string month, string year)
        {
            string existsSql = @"
  select count(*) into {0} from FMS_CODLineHistory WHERE CODLineNO = '{1}' AND BakYear ='{2}' AND BakMonth ='{3}';
  if({0}<=0) then
  begin
    {4}
    :all_Count := :all_Count + sql%rowcount;
  end;
  end if;
";
            string insertSql = @" INSERT INTO FMS_CODLineHistory(LineID,CODLineNO,ExpressCompanyID,IsEchelon,
                                    WareHouseID,AreaType,PriceFormula,LineStatus,AuditStatus,AuditBy,AuditTime,CreateBy,CreateTime,
                                    UpdateBy,UpdateTime,DeleteFlag,BakYear,BakMonth,WareHouseType,MerchantID,ProductID,DistributionCode,
                                    Formula,IsCOD,IsChange) VALUES ";
           
            StringBuilder sbSqlList=new StringBuilder();
            List<OracleParameter> parameterList = new List<OracleParameter>();
            int i = 0;
            string formart = ":{0}{1}{2}";
            string sql = string.Empty;
            parameterList.Add(new OracleParameter(":all_Count", OracleDbType.Decimal) { Direction = ParameterDirection.Output });
            foreach (FMS_CODLine d in codLineList)
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(insertSql);
                sbSql.Append("(");
                sbSql.Append(string.Format(formart, "LineID", i, ","));
                sbSql.Append(string.Format(formart, "CODLineNO", i, ","));
                sbSql.Append(string.Format(formart, "ExpressCompanyID", i, ","));
                sbSql.Append(string.Format(formart, "IsEchelon", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseID", i, ","));
                sbSql.Append(string.Format(formart, "AreaType", i, ","));
                sbSql.Append(string.Format(formart, "PriceFormula", i, ","));
                sbSql.Append(string.Format(formart, "LineStatus", i, ","));
                sbSql.Append(string.Format(formart, "AuditStatus", i, ","));
                sbSql.Append(string.Format(formart, "AuditBy", i, ","));
                sbSql.Append(string.Format(formart, "AuditTime", i, ","));
                sbSql.Append(string.Format(formart, "CreateBy", i, ","));
                sbSql.Append(string.Format(formart, "CreateTime", i, ","));
                sbSql.Append(string.Format(formart, "UpdateBy", i, ","));
                sbSql.Append(string.Format(formart, "UpdateTime", i, ","));
                sbSql.Append(string.Format(formart, "DeleteFlag", i, ","));
                sbSql.Append(string.Format(formart, "BakYear", i, ","));
                sbSql.Append(string.Format(formart, "BakMonth", i, ","));
                sbSql.Append(string.Format(formart, "WareHouseType", i, ","));
                sbSql.Append(string.Format(formart, "MerchantID", i, ","));
                sbSql.Append(string.Format(formart, "ProductID", i, ","));
                sbSql.Append(string.Format(formart, "DistributionCode", i, ","));
                sbSql.Append(string.Format(formart, "Formula", i, ","));
                sbSql.Append(string.Format(formart, "IsCOD", i, ","));
                sbSql.Append(1);
                sbSql.Append(");");

                parameterList.Add(new OracleParameter(string.Format(formart, "LineID", i, ""), GetIdNew("seq_fms_codlinehistory")));
                parameterList.Add(new OracleParameter(string.Format(formart, "CODLineNO", i, ""), d.CODLineNO));
                parameterList.Add(new OracleParameter(string.Format(formart, "ExpressCompanyID", i, ""), d.ExpressCompanyID));
                parameterList.Add(new OracleParameter(string.Format(formart, "IsEchelon", i, ""), d.IsEchelon));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseID", i, ""), d.WareHouseID));
                parameterList.Add(new OracleParameter(string.Format(formart, "AreaType", i, ""), d.AreaType));
                parameterList.Add(new OracleParameter(string.Format(formart, "PriceFormula", i, ""), d.PriceFormula));
                parameterList.Add(new OracleParameter(string.Format(formart, "LineStatus", i, ""), d.LineStatus));
                parameterList.Add(new OracleParameter(string.Format(formart, "AuditStatus", i, ""), d.AuditStatus));
                parameterList.Add(new OracleParameter(string.Format(formart, "AuditBy", i, ""), d.AuditBy));
                parameterList.Add(new OracleParameter(string.Format(formart, "AuditTime", i, ""), d.AuditTime));
                parameterList.Add(new OracleParameter(string.Format(formart, "CreateBy", i, ""), d.CreateBy));
                parameterList.Add(new OracleParameter(string.Format(formart, "CreateTime", i, ""), d.CreateTime));
                parameterList.Add(new OracleParameter(string.Format(formart, "UpdateBy", i, ""), d.UpdateBy));
                parameterList.Add(new OracleParameter(string.Format(formart, "UpdateTime", i, ""), d.UpdateTime));
                parameterList.Add(new OracleParameter(string.Format(formart, "DeleteFlag", i, ""), d.DeleteFlag?1:0));
                parameterList.Add(new OracleParameter(string.Format(formart, "BakYear", i, ""), year));
                parameterList.Add(new OracleParameter(string.Format(formart, "BakMonth", i, ""), month));
                parameterList.Add(new OracleParameter(string.Format(formart, "WareHouseType", i, ""), d.WareHouseType));
                parameterList.Add(new OracleParameter(string.Format(formart, "MerchantID", i, ""), d.MerchantID));
                parameterList.Add(new OracleParameter(string.Format(formart, "ProductID", i, ""), d.ProductID));
                parameterList.Add(new OracleParameter(string.Format(formart, "DistributionCode", i, ""), d.DistributionCode));
                parameterList.Add(new OracleParameter(string.Format(formart, "Formula", i, ""), d.Formula));
                parameterList.Add(new OracleParameter(string.Format(formart, "IsCOD", i, ""), d.IsCOD));
                parameterList.Add(new OracleParameter(string.Format(formart, "v_Count", i, ""), OracleDbType.Decimal) { Value = 0 });
                sql = string.Format(existsSql,string.Format(formart, "v_Count", i, ""), d.CODLineNO, year, month, sbSql.ToString());

                sbSqlList.Append(sql);
                i++;
            }
            List<OracleParameter> parameterLists = new List<OracleParameter>();
            parameterLists.AddRange(parameterList);
            string sqlStr = string.Format(@"
                                            begin 
                                                :all_Count := 0;
                                                {0}
                                            end;", sbSqlList.ToString());
            object n = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameterLists.ToArray());
            bool flag = DataConvert.ToInt(parameterLists[0].Value, 0) > 0;
            return flag;
        }

        public int UpdateToDelete(string year, string month)
        {
            string sql = "UPDATE FMS_CODLineHistory SET DeleteFlag=1 WHERE BakMonth='{0}' AND BakYear='{1}' AND DeleteFlag=0";
            //Sql = "delete from FMS_CODLineHistory WHERE BakMonth='{0}' AND BakYear='{1}'";
            sql = string.Format(sql, month, year);
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql);
        }

        /// <summary>
        /// 获取整个需要备份
        /// </summary>
        /// <returns></returns>
        public IList<FMS_CODLine> GetBackList()
        {
            string sql = @"SELECT CODLineNO,ExpressCompanyID,IsEchelon,WareHouseID,AreaType,PriceFormula,LineStatus,
                            AuditStatus,AuditBy,AuditTime,CreateBy,CreateTime,UpdateBy,UpdateTime,DeleteFlag,
                            WareHouseType,MerchantID,ProductID,DistributionCode,Formula,IsCOD
                    FROM FMS_CODLine";
            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
            IList<FMS_CODLine> codLineList = new List<FMS_CODLine>();
            TableToModel(dt, ref codLineList);

            return codLineList;
        }

        private void TableToModel(DataTable dt, ref IList<FMS_CODLine> codLineList)
        {
            FMS_CODLine codLine;
            foreach (DataRow dr in dt.Rows)
            {
                codLine = new FMS_CODLine();
                codLine.CODLineNO = dr["CODLineNO"].ToString();
                codLine.ExpressCompanyID = DataConvert.ToInt(dr["ExpressCompanyID"].ToString());
                codLine.IsEchelon = DataConvert.ToInt(dr["IsEchelon"].ToString());
                codLine.WareHouseID = dr["WareHouseID"].ToString();
                codLine.AreaType = DataConvert.ToInt(dr["AreaType"].ToString());
                codLine.PriceFormula = dr["PriceFormula"].ToString();
                codLine.LineStatus = DataConvert.ToInt(dr["LineStatus"].ToString());
                codLine.AuditStatus = DataConvert.ToInt(dr["AuditStatus"].ToString());
                codLine.AuditBy = dr["AuditBy"].ToString();
                codLine.AuditTime = DateTime.Parse(DataConvert.ToDateTime(dr["AuditTime"]).ToString());
                codLine.CreateBy = dr["CreateBy"].ToString();
                codLine.CreateTime = DateTime.Parse(DataConvert.ToDateTime(dr["CreateTime"]).ToString());
                codLine.UpdateBy = dr["UpdateBy"].ToString();
                codLine.UpdateTime = DateTime.Parse(DataConvert.ToDateTime(dr["UpdateTime"]).ToString());
                codLine.DeleteFlag = DataConvert.ToBoolean(dr["DeleteFlag"]);
                codLine.WareHouseType = DataConvert.ToInt(dr["WareHouseType"].ToString());
                codLine.MerchantID = DataConvert.ToInt(dr["MerchantID"].ToString());
                codLine.ProductID = DataConvert.ToInt(dr["ProductID"].ToString());
                codLine.DistributionCode = dr["DistributionCode"].ToString();
                codLine.IsCOD = DataConvert.ToInt(dr["IsCOD"].ToString());
                codLine.Formula = dr["Formula"].ToString();
                codLineList.Add(codLine);
            }
        }
        #endregion

        #region COD待生效
        public List<FMS_CODLine> GetCODLineWaitEffect(string Date)
        {
            string sql = @"SELECT tcwe.LineID,
                           tcwe.CODLineNO,
                           tcwe.ExpressCompanyID,
                           tcwe.IsEchelon,
                           tcwe.WareHouseID,
                           tcwe.AreaType,
                           tcwe.PriceFormula,
                           tcwe.EffectDate,
                           tcwe.Formula,
                           tcwe.IsCOD
                    FROM   FMS_CODLineWaitEffect tcwe
                    WHERE  tcwe.AuditStatus = 1
                           AND tcwe.LineStatus = 1
                           AND DeleteFlag = 0
                           AND tcwe.EffectDate = to_date(:EffectDate,'yyyy-mm-dd')";
            OracleParameter[] parameters ={
                                           new OracleParameter(":EffectDate",OracleDbType.Varchar2)
                                      };
            parameters[0].Value = DateTime.Parse(Date).ToString("yyyy-MM-dd") ;
            DataTable dt = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];

            return CreateCODLineWaitEffectList(dt);
        }

        private List<FMS_CODLine> CreateCODLineWaitEffectList(DataTable dt)
        {
            List<FMS_CODLine> clweList = new List<FMS_CODLine>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    FMS_CODLine clwe = new FMS_CODLine();
                    clwe.LineID = int.Parse(dr["LineID"].ToString());
                    clwe.CODLineNO = dr["CODLineNO"].ToString();
                    clwe.ExpressCompanyID = int.Parse(dr["ExpressCompanyID"].ToString());
                    clwe.IsEchelon = int.Parse(dr["IsEchelon"].ToString());
                    clwe.WareHouseID = dr["WareHouseID"].ToString();
                    clwe.AreaType = int.Parse(dr["AreaType"].ToString());
                    clwe.PriceFormula = dr["PriceFormula"].ToString();
                    clwe.EffectDate = DateTime.Parse(dr["EffectDate"].ToString());
                    clwe.Formula = dr["Formula"].ToString();
                    clwe.IsCOD = int.Parse(dr["IsCOD"].ToString());
                    clweList.Add(clwe);
                }
                return clweList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新已生效价格 回改待生效状态
        /// </summary>
        /// <param name="clwe"></param>
        /// <returns></returns>
        public bool UpdateLineForCODLine(FMS_CODLine clwe)
        {
            string sql = @"UPDATE FMS_CODLine
                    SET    PriceFormula = :PriceFormula,
                            Formula = :Formula,
                            IsCOD = :IsCOD,
                            AuditStatus = 1,
                            UpdateBy = 0 ,
                            UpdateTime = :UpdateTime,
                            AuditBy = 0 ,
                            AuditTime = SYSDATE
                    WHERE  CODLineNO = :CODLineNO";//AuditTime取当前同步时间，是为校对准备，避免8-15待生效审核却在9-1生效的情况
            OracleParameter[] parameters ={
                                            new OracleParameter(":PriceFormula",OracleDbType.Varchar2,300),
                                            new OracleParameter(":Formula",OracleDbType.Varchar2,300),
                                            new OracleParameter(":IsCOD",OracleDbType.Decimal),
                                            new OracleParameter(":CODLineNO",OracleDbType.Varchar2,40),
                                            new OracleParameter(":UpdateTime",OracleDbType.Date)
                                        };
            parameters[0].Value = clwe.PriceFormula;
            parameters[1].Value = clwe.Formula;
            parameters[2].Value = clwe.IsCOD;
            parameters[3].Value = clwe.CODLineNO;
            parameters[4].Value = clwe.EffectDate;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public bool UpdateLineForCODLineWaitEffect(FMS_CODLine clwe)
        {
            string sql = @"UPDATE FMS_CODLineWaitEffect
                    SET    DeleteFlag = 1
                    WHERE  LineID = :LineID";
            OracleParameter[] parameters1 ={
                                            new OracleParameter(":LineID",OracleDbType.Decimal)
                                        };
            parameters1[0].Value = clwe.LineID;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters1) > 0;
        }

        #endregion
    }
}
