using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.Util;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using System.Data.SqlClient;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;
using System.Xml;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class MerchantDeliverFeeDao : OracleDao, IMerchantDeliverFeeDao
	{
        public string SqlStr { get; set; }

        /// <summary>
        /// 创建仓库查询xml
        /// </summary>
        /// <param name="houseStr">字符串</param>
        /// <returns></returns>
        private XmlDocument CreateHouseXml(string houseStr)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xe;
            string[] houses;
            xe = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(xe);
            houses = houseStr.Split(',');
            for (int i = 0; i < houses.Length; i++)
            {
                xe = xmlDoc.CreateElement("id");
                xe.SetAttribute("v", houses[i].Trim());
                xmlDoc.DocumentElement.AppendChild(xe);
            }
            return xmlDoc;
        }

        private string BuildSearchCondition(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode, string tableAlias, ref List<OracleParameter> parameterList)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND {0}.MerchantID=:MerchantID ");
                parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND {0}.StationID=:StationID ");
                parameterList.Add(new OracleParameter(":StationID", OracleDbType.Decimal) { Value = expressCompanyId });
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND {0}.AreaType=:AreaType ");
                parameterList.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND {0}.Status=:Status ");
                parameterList.Add(new OracleParameter(":Status", OracleDbType.Decimal) { Value = auditStatus });
            }

            if (!string.IsNullOrEmpty(categoryCode))
            {
                sbWhere.Append(Util.Common.GetOracleInParameterWhereSql("{0}.GoodsCategoryCode","GoodsCategoryCode",false,false));
                parameterList.Add(new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2) { Value = categoryCode.Replace(" ","") });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND {0}.DistributionCode=:DistributionCode ");
                parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
            }

            if (!string.IsNullOrEmpty(sortCenterId))
            {
                sbWhere.Append(Util.Common.GetOracleInParameterWhereSql("{0}.ExpressCompanyID","ExpressCompanyID",false,false));
                parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Varchar2) { Value = sortCenterId });
            }

            return string.Format(sbWhere.ToString(), tableAlias);
        }

        public int GetDeliverFeeStat(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            string sql = @"SELECT  count(1)
						FROM    FMS_StationDeliverFee  fsdf
								JOIN MerchantBaseInfo  mbi ON fsdf.MerchantID = mbi.ID
								JOIN ExpressCompany  ec ON ec.ExpressCompanyID = fsdf.StationID
								JOIN StatusCodeInfo  sci ON cast(sci.CodeNo as NUMBER) = cast(fsdf.Status as NUMBER)
																			 AND sci.CodeType = 'AreaTypeAudit'
                                LEFT JOIN ExpressCompany  ec2 ON ec2.ExpressCompanyID = fsdf.ExpressCompanyID
																				  AND ec2.CompanyFlag = 1
                                LEFT JOIN GoodsCategory gc on gc.GoodsCategoryCode=fsdf.GoodsCategoryCode
						WHERE   ( fsdf.IsDeleted =0 ) {0}";
            string sqlWhere = string.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdf", ref parameters);

            sql = string.Format(sql, sqlWhere);

            object n=OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, this.ToParameters(parameters.ToArray()));
            return DataConvert.ToInt(n, 0);
        }

        public DataTable GetDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode, PageInfo pi)
        {
            SqlStr = @"with t as(SELECT  ROWNUM as nums,
                                fsdf.FEEID as ID,
								fsdf.MerchantID ,
								mbi.MerchantName ,
								fsdf.StationID ,
							    (CASE fsdf.StationID WHEN 11 THEN '全部' 
							     ELSE 
							     CASE  nvl(ec.ACCOUNTCOMPANYNAME,'') 
							     WHEN '' THEN  ec.CompanyName ELSE ec.ACCOUNTCOMPANYNAME END   END) CompanyName ,
								(CASE fsdf.IsCenterSort WHEN 0 THEN '否' WHEN 1 THEN '是' ELSE '否' END) IsCenterSortStr ,
								fsdf.ExpressCompanyID  SortCenterID,
								ec2.CompanyName  SortCenterName,
								fsdf.Status ,
								sci.CodeDesc  AuditStatusStr ,
								fsdf.AreaType ,
								fsdf.BasicDeliverFee,
                                fsdf.DeliverFee,
                                fsdf.IsCod,
                                CASE WHEN fsdf.IsCod=0 THEN '否' ELSE '是' END IsCodStr,
								fsdf.CreateBy ,
								fsdf.CreateTime ,
								fsdf.UpdateBy ,
								fsdf.UpdateTime ,
								fsdf.AuditBy ,
								fsdf.AuditTime,
                                2 as LogType,
                                gc.GoodsCategoryCode,
                                gc.GoodsCategoryName
						FROM    FMS_StationDeliverFee  fsdf
								JOIN MerchantBaseInfo  mbi ON fsdf.MerchantID = mbi.ID
								JOIN ExpressCompany  ec ON ec.ExpressCompanyID = fsdf.StationID
								JOIN StatusCodeInfo  sci ON cast(sci.CodeNo as NUMBER) = cast(fsdf.Status as NUMBER)
																			 AND sci.CodeType = 'AreaTypeAudit'
								LEFT JOIN ExpressCompany  ec2 ON ec2.ExpressCompanyID = fsdf.ExpressCompanyID
																				  AND ec2.CompanyFlag = 1
                                LEFT JOIN GoodsCategory gc on gc.GoodsCategoryCode=fsdf.GoodsCategoryCode
						WHERE   ( fsdf.IsDeleted =0 ) {0} ) select * from t where nums between :RowStr and :RowEnd";

            string sqlWhere = string.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdf", ref parameters);
            List<OracleParameter> parametersTmp = new List<OracleParameter>();
            parametersTmp.Add(new OracleParameter(":RowStr", OracleDbType.Decimal) { Value = pi.CurrentPageStartRowNum });
            parametersTmp.Add(new OracleParameter(":RowEnd", OracleDbType.Decimal) { Value = pi.CurrentPageEndRowNum });

            parameters.AddRange(parametersTmp);

            SqlStr = string.Format(SqlStr, sqlWhere);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public int GetDeliverFeeWaitStat(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            SqlStr = @"	SELECT  count(1)
						FROM    FMS_StationDeliverFeeWait  fsdfw
								JOIN MerchantBaseInfo  mbi ON fsdfw.MerchantID = mbi.ID
								JOIN ExpressCompany  ec ON ec.ExpressCompanyID = fsdfw.StationID
								JOIN StatusCodeInfo  sci ON cast(sci.CodeNo as NUMBER) = cast(fsdfw.Status as NUMBER)
																			 AND sci.CodeType = 'AreaTypeAudit'
						WHERE   ( fsdfw.IsDeleted =0 ) {0}";

            string sqlWhere = string.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdfw", ref parameters);

            SqlStr = string.Format(SqlStr, sqlWhere);

            object n= OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray()));
            return DataConvert.ToInt(n, 0);
        }

        public DataTable GetDeliverFeeWaitList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode, PageInfo pi)
        {
            SqlStr = @"with t as(SELECT  ROWNUM as nums,
                                fsdfw.FEEID,
								fsdfw.MerchantID ,
								mbi.MerchantName ,
								fsdfw.StationID ,
								CASE fsdfw.StationID WHEN 11 THEN '全部'
                                ELSE(
                                CASE WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.CompanyName
                                      ELSE ec.AccountCompanyName
                                END ) END  CompanyName,
								(CASE fsdfw.IsCenterSort WHEN 0 THEN '否' WHEN 1 THEN '是' ELSE '否' END) IsCenterSortStr ,
								fsdfw.ExpressCompanyID  SortCenterID,
								ec2.CompanyName  SortCenterName,
								fsdfw.Status ,
								sci.CodeDesc  AuditStatusStr ,
								fsdfw.AreaType ,
								fsdfw.BasicDeliverFee,
                                fsdfw.DeliverFee,
                                fsdfw.IsCod,
                                CASE WHEN fsdfw.IsCod=0 THEN '否' ELSE '是' END IsCodStr,
								fsdfw.CreateBy ,
								fsdfw.CreateTime ,
								fsdfw.UpdateBy ,
								fsdfw.UpdateTime ,
								fsdfw.AuditBy ,
								fsdfw.AuditTime,
                                fsdfw.EffectDate,
                                fsdfw.EffectKid as ID,
                                5 as LogType,
                                gc.GoodsCategoryCode,
                                gc.GoodsCategoryName
						FROM    FMS_StationDeliverFeeWait  fsdfw
								JOIN MerchantBaseInfo  mbi ON fsdfw.MerchantID = mbi.ID
								JOIN ExpressCompany  ec ON ec.ExpressCompanyID = fsdfw.StationID
								JOIN StatusCodeInfo  sci ON cast(sci.CodeNo as NUMBER) = cast(fsdfw.Status as NUMBER)
																			 AND sci.CodeType = 'AreaTypeAudit'
								LEFT JOIN ExpressCompany  ec2 ON ec2.ExpressCompanyID = fsdfw.ExpressCompanyID
																				  AND ec2.CompanyFlag = 1
                                LEFT JOIN GoodsCategory gc on gc.GoodsCategoryCode=fsdfw.GoodsCategoryCode
						WHERE   ( fsdfw.IsDeleted =0  {0} )) select * from t where nums between :RowStr and :RowEnd";

            string sqlWhere = string.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdfw", ref parameters);
            List<OracleParameter> parametersTmp = new List<OracleParameter>();
            parametersTmp.Add(new OracleParameter(":RowStr", OracleDbType.Decimal) { Value = pi.CurrentPageStartRowNum });
            parametersTmp.Add(new OracleParameter(":RowEnd", OracleDbType.Decimal) { Value = pi.CurrentPageEndRowNum });

            parameters.AddRange(parametersTmp);

            SqlStr = string.Format(SqlStr, sqlWhere);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        /// <summary>
        /// 添加站点配送费信息
        /// </summary>
        /// <param name="model">站点实体</param>
        /// <returns></returns>
        public bool AddStationDeliverFee(FMS_StationDeliverFee model, out int id)
        {
            if (string.IsNullOrEmpty(model.GoodsCategoryCode))
            {
				SqlStr = @"SELECT count(1) FROM FMS_StationDeliverFee  fsdf 
												WHERE fsdf.MerchantID={0} AND fsdf.StationID={1} 
													AND fsdf.AreaType={2} AND fsdf.IsDeleted=0 AND fsdf.DistributionCode='{3}' AND fsdf.ExpressCompanyID={4}";
                SqlStr = String.Format(SqlStr, model.MerchantID, model.StationID, model.AreaType, model.DistributionCode, model.ExpressCompanyID);
            }
            else
            {
				SqlStr = @"SELECT count(1) FROM FMS_StationDeliverFee  fsdf 
												WHERE fsdf.MerchantID={0} AND fsdf.StationID={1} 
													AND fsdf.AreaType={2} AND fsdf.IsDeleted=0 AND fsdf.DistributionCode='{3}'
                                                    AND fsdf.ExpressCompanyID={4} AND GoodsCategoryCode={5}";
                SqlStr = String.Format(SqlStr, model.MerchantID, model.StationID, model.AreaType, model.DistributionCode, model.ExpressCompanyID, model.GoodsCategoryCode);
                
            }
			object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, SqlStr, null);
			if (obj != null)
			{
				if (Convert.ToInt32(obj) > 0)
				{
					id = -1;
					return false;
				}
			}
			
			SqlStr = @"INSERT INTO FMS_StationDeliverFee(FEEID,MerchantID,StationID,BasicDeliverFee,UpdateBy,UpdateTime,
																UpdateCode,AuditBy,AuditTime,AuditCode,AuditResult,
																Status,ExpressCompanyID,IsCenterSort,AreaType,CreateBy,CreateTime
                                                                ,IsDeleted,DistributionCode,IsChange,GoodsCategoryCode,IsCod,DeliverFee,ISEXPRESS)
									VALUES  (:FEEID,:MerchantID,:StationID,:BasicDeliverFee,:UpdateBy,SysDate,
											:UpdateCode,:AuditBy,SysDate,:AuditCode,:AuditResult,:Status,:ExpressCompanyID,
											:IsCenterSort,:AreaType,:CreateBy,SysDate,0,:DistributionCode,:IsChange,:GoodsCategoryCode,:IsCod,:DeliverFee,:ISEXPRESS  )
							";

            OracleParameter[] parameters =
            {
			    new OracleParameter(":FEEID", OracleDbType.Decimal),
				new OracleParameter(":MerchantID", OracleDbType.Decimal),
				new OracleParameter(":StationID", OracleDbType.Decimal),
				new OracleParameter(":BasicDeliverFee", OracleDbType.Varchar2,300),
				new OracleParameter(":UpdateBy", OracleDbType.Decimal),
				new OracleParameter(":UpdateCode", OracleDbType.Varchar2,40),
				new OracleParameter(":AuditBy", OracleDbType.Decimal),
				new OracleParameter(":AuditCode", OracleDbType.Varchar2,40),
				new OracleParameter(":AuditResult", OracleDbType.Decimal) ,
				new OracleParameter(":Status", OracleDbType.Decimal) ,
				new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) ,
				new OracleParameter(":IsCenterSort", OracleDbType.Decimal) ,
				new OracleParameter(":AreaType", OracleDbType.Decimal) ,
				new OracleParameter(":CreateBy", OracleDbType.Decimal) ,
                new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100),
                new OracleParameter(":IsChange", OracleDbType.Decimal),
                new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2,20),
                new OracleParameter(":IsCod", OracleDbType.Decimal),
                new OracleParameter(":DeliverFee", OracleDbType.Varchar2,300),
                new OracleParameter( ":ISEXPRESS",OracleDbType.Decimal)
			};
            model.ID = GetIdNew("SEQ_FMS_STATIONDELIVERFEE");
			parameters[0].Value = model.ID;
            parameters[1].Value = model.MerchantID;
            parameters[2].Value = model.StationID;
            parameters[3].Value = model.BasicDeliverFee;
            parameters[4].Value = model.UpdateUser;
            parameters[5].Value = model.UpdateUserCode;
            parameters[6].Value = model.AuditBy;
            parameters[7].Value = model.AuditCode;
            parameters[8].Value = model.AuditResult;
            parameters[9].Value = model.Status;
            parameters[10].Value = model.ExpressCompanyID;
            parameters[11].Value = model.IsCenterSort;
            parameters[12].Value = model.AreaType;
            parameters[13].Value = model.CreateUser;
            parameters[14].Value = model.DistributionCode;
            parameters[15].Value = 1;
            parameters[16].Value = model.GoodsCategoryCode;
            parameters[17].Value = model.IsCod;
            parameters[18].Value = model.DeliverFee;
            parameters[19].Value = model.IsExpress;
            bool flag = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;

			id = model.ID;

            return flag;
        }

        public bool AddWaitStationDeliverFee(FMS_StationDeliverFee model, out int id)
        {
            if (string.IsNullOrEmpty(model.GoodsCategoryCode.ToString()))
            {
                SqlStr = @"SELECT count(1) FROM FMS_StationDeliverFeeWait  fsdf 
												WHERE fsdf.MerchantID={0} AND fsdf.StationID={1} 
													AND fsdf.AreaType={2} AND fsdf.IsDeleted=0 AND fsdf.DistributionCode='{3}'
                                                    AND fsdf.ExpressCompanyID={4}";
                SqlStr = String.Format(SqlStr, model.MerchantID, model.StationID, model.AreaType, model.DistributionCode, model.ExpressCompanyID);
            }
            else
            {
                SqlStr = @"SELECT count(1) FROM FMS_StationDeliverFeeWait  fsdf 
												WHERE fsdf.MerchantID={0} AND fsdf.StationID={1} 
													AND fsdf.AreaType={2} AND fsdf.IsDeleted=0 AND fsdf.DistributionCode='{3}'
                                                    AND fsdf.ExpressCompanyID={4} AND fsdf.GoodsCategoryCode={5}";
                SqlStr = String.Format(SqlStr, model.MerchantID, model.StationID, model.AreaType, model.DistributionCode, model.ExpressCompanyID, model.GoodsCategoryCode);

            }
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, SqlStr, null);
            if (obj != null)
            {
                if (Convert.ToInt32(obj) > 0)
                {
                    id = -1;
                    return false;
                }
            }
            SqlStr = @"INSERT INTO FMS_StationDeliverFeeWait(FEEID,MerchantID,StationID,BasicDeliverFee,UpdateBy,UpdateTime,
																UpdateCode,AuditBy,AuditTime,AuditCode,AuditResult,
																Status,ExpressCompanyID,IsCenterSort,AreaType,CreateBy,CreateTime,
                                                                IsDeleted,DistributionCode,IsChange,EffectDate,EffectKid,GoodsCategoryCode,IsCod,DeliverFee,ISEXPRESS)
									VALUES  (:FEEID,:MerchantID,:StationID,:BasicDeliverFee,:UpdateBy,SysDate,
											:UpdateCode,:AuditBy,SysDate,:AuditCode,:AuditResult,:Status,:ExpressCompanyID,
											:IsCenterSort,:AreaType,:CreateBy,SysDate,0,:DistributionCode,:IsChange,:EffectDate,:EffectKid,:GoodsCategoryCode,:IsCod,:DeliverFee,:ISEXPRESS  )
							";

            OracleParameter[] parameters =
            {
			    new OracleParameter(":FEEID", OracleDbType.Decimal),
				new OracleParameter(":MerchantID", OracleDbType.Decimal),
				new OracleParameter(":StationID", OracleDbType.Decimal),
				new OracleParameter(":BasicDeliverFee", OracleDbType.Varchar2,300),
				new OracleParameter(":UpdateBy", OracleDbType.Decimal),
				new OracleParameter(":UpdateCode", OracleDbType.Varchar2,40),
				new OracleParameter(":AuditBy", OracleDbType.Decimal),
				new OracleParameter(":AuditCode", OracleDbType.Varchar2,40),
				new OracleParameter(":AuditResult", OracleDbType.Decimal) ,
				new OracleParameter(":Status", OracleDbType.Decimal) ,
				new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) ,
				new OracleParameter(":IsCenterSort", OracleDbType.Decimal) ,
				new OracleParameter(":AreaType", OracleDbType.Decimal) ,
				new OracleParameter(":CreateBy", OracleDbType.Decimal) ,
                new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100),
                new OracleParameter(":IsChange", OracleDbType.Decimal),
                new OracleParameter(":EffectDate", OracleDbType.Date),
                new OracleParameter(":EffectKid", OracleDbType.Varchar2,40),
                new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2,20),
                new OracleParameter(":IsCod", OracleDbType.Decimal),
                new OracleParameter(":DeliverFee", OracleDbType.Varchar2,300),
                new OracleParameter(":ISEXPRESS",OracleDbType.Decimal), 
			};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.MerchantID;
            parameters[2].Value = model.StationID;
            parameters[3].Value = model.BasicDeliverFee;
            parameters[4].Value = model.UpdateUser;
            parameters[5].Value = model.UpdateUserCode;
            parameters[6].Value = model.AuditBy;
            parameters[7].Value = model.AuditCode;
            parameters[8].Value = model.AuditResult;
            parameters[9].Value = model.Status;
            parameters[10].Value = model.ExpressCompanyID;
            parameters[11].Value = model.IsCenterSort;
            parameters[12].Value = model.AreaType;
            parameters[13].Value = model.CreateUser;
            parameters[14].Value = model.DistributionCode;
            parameters[15].Value = 1;
            parameters[16].Value = model.EffectDate;
            parameters[17].Value = model.EffectKid;
            parameters[18].Value = model.GoodsCategoryCode;
            parameters[19].Value = model.IsCod;
            parameters[20].Value = model.DeliverFee;
            parameters[21].Value = model.IsExpress;
            bool flag = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;

            id = model.ID;

            return flag;
        }

        public bool UpdateDeliverFee(FMS_StationDeliverFee model)
        {
            SqlStr = @"UPDATE  FMS_StationDeliverFee
						SET     BasicDeliverFee = :BasicDeliverFee,DeliverFee = :DeliverFee,IsCod = :IsCod,
                                UpdateBy=:UpdateBy,UpdateTime=SysDate,IsChange=1
						WHERE   FEEID = :ID
								AND IsDeleted = 0";
            OracleParameter[] parameters ={
										new OracleParameter(":ID", OracleDbType.Decimal),
										new OracleParameter(":BasicDeliverFee", OracleDbType.Varchar2,300),
										new OracleParameter(":UpdateBy", OracleDbType.Decimal),
                                        new OracleParameter(":DeliverFee", OracleDbType.Varchar2,300),
										new OracleParameter(":IsCod", OracleDbType.Decimal), 
									  };
            parameters[0].Value = model.ID;
            parameters[1].Value = model.BasicDeliverFee;
            parameters[2].Value = model.UpdateUser;
            parameters[3].Value = model.DeliverFee;
            parameters[4].Value = model.IsCod;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateWaitDeliverFee(FMS_StationDeliverFee model)
        {
            SqlStr = @"UPDATE  FMS_StationDeliverFeeWait
						SET     BasicDeliverFee = :BasicDeliverFee,DeliverFee = :DeliverFee,IsCod = :IsCod,
                                UpdateBy=:UpdateBy,UpdateTime=SysDate,IsChange=1,EffectDate=:EffectDate
						WHERE   EffectKid = :EffectKid
								AND IsDeleted = 0";
            OracleParameter[] parameters ={
										new OracleParameter(":EffectKid", OracleDbType.Varchar2,40),
										new OracleParameter(":BasicDeliverFee", OracleDbType.Varchar2,300),
										new OracleParameter(":UpdateBy", OracleDbType.Decimal),
                                        new OracleParameter(":EffectDate", OracleDbType.Date),
                                        new OracleParameter(":DeliverFee", OracleDbType.Varchar2,300),
										new OracleParameter(":IsCod", OracleDbType.Decimal),
									  };
            parameters[0].Value = model.EffectKid;
            parameters[1].Value = model.BasicDeliverFee;
            parameters[2].Value = model.UpdateUser;
            parameters[3].Value = model.EffectDate;
            parameters[4].Value = model.DeliverFee;
            parameters[5].Value = model.IsCod;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public DataTable GetDeliverFeeById(int id)
        {
            SqlStr = @"SELECT  fsdf.MerchantID ,
								fsdf.StationID ,
                                fsdf.IsCod ,
								fsdf.BasicDeliverFee ,
                                fsdf.DeliverFee ,
								fsdf.UpdateBy ,
								fsdf.UpdateTime ,
								fsdf.UpdateCode ,
								fsdf.AuditBy ,
								fsdf.AuditTime ,
								fsdf.AuditCode ,
								fsdf.AuditResult ,
								fsdf.Status ,
								fsdf.ExpressCompanyID ,
								fsdf.IsCenterSort ,
								fsdf.AreaType ,
								fsdf.CreateBy,
								ec.CompanyName ,
								ec2.CompanyName Expressname,
                                fsdf.GoodsCategoryCode,
                                mbi.MerchantName,
                                fsdf.ISEXPRESS
						FROM    FMS_StationDeliverFee  fsdf
						JOIN	ExpressCompany  ec  on fsdf.StationID=ec.ExpressCompanyID
                        JOIN	MerchantBaseInfo mbi on mbi.ID=fsdf.MerchantID
                        JOIN    ExpressCompany  ec2  on fsdf.ExpressCompanyID=ec2.ExpressCompanyID
						WHERE   fsdf.IsDeleted = 0 AND fsdf.FEEID=:ID";
            OracleParameter[] parameters ={
										new OracleParameter(":ID", OracleDbType.Decimal),
									  };
            parameters[0].Value = id;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters).Tables[0];
        }

        public DataTable GetWaitDeliverFeeById(string id)
        {
            SqlStr = @"SELECT  fsdfw.MerchantID ,
								fsdfw.StationID ,
                                fsdfw.IsCod ,
								fsdfw.BasicDeliverFee ,
                                fsdfw.DeliverFee ,
								fsdfw.UpdateBy ,
								fsdfw.UpdateTime ,
								fsdfw.UpdateCode ,
								fsdfw.AuditBy ,
								fsdfw.AuditTime ,
								fsdfw.AuditCode ,
								fsdfw.AuditResult ,
								fsdfw.Status ,
								fsdfw.ExpressCompanyID ,
								fsdfw.IsCenterSort ,
								fsdfw.AreaType ,
								fsdfw.CreateBy,
								ec.CompanyName,
                                fsdfw.EffectDate,
                                fsdfw.EffectKid,
                                fsdfw.goodscategorycode,
                                mbi.MerchantName,
                                ec2.CompanyName Expressname,
                                fsdfw.ISEXPRESS
						FROM    FMS_StationDeliverFeeWait  fsdfw
						JOIN	ExpressCompany  ec  on fsdfw.StationID=ec.ExpressCompanyID
                        JOIN	MerchantBaseInfo mbi on mbi.ID=fsdfw.MerchantID
                        JOIN    ExpressCompany  ec2  on fsdfw.ExpressCompanyID=ec2.ExpressCompanyID
						WHERE   fsdfw.IsDeleted = 0  AND EffectKid=:EffectKid";
            OracleParameter[] parameters ={
										new OracleParameter(":EffectKid", OracleDbType.Varchar2,40),
									  };
            parameters[0].Value = id;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, parameters).Tables[0];
        }

        public bool DeleteDeliverFee(int id, int updateBy)
        {
            SqlStr = @"UPDATE  FMS_StationDeliverFee
						SET     IsDeleted = 1,UpdateBy=:UpdateBy,UpdateTime=SysDate
						WHERE   FEEID = :ID
								AND IsDeleted = 0";
            OracleParameter[] parameters ={
										new OracleParameter(":ID", OracleDbType.Decimal),
										new OracleParameter(":UpdateBy", OracleDbType.Decimal),
									  };
            parameters[0].Value = id;
            parameters[1].Value = updateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateDeliverFeeStatus(int id, int status, int auditBy)
        {
            SqlStr = @"UPDATE  FMS_StationDeliverFee
						SET     Status = :Status,AuditBy=:AuditBy,AuditTime=SysDate,IsChange=1
						WHERE   FEEID = :ID
								AND NVL(IsDeleted,0) = 0";
            OracleParameter[] parameters ={
										new OracleParameter(":ID", OracleDbType.Decimal),
										new OracleParameter(":Status", OracleDbType.Varchar2,5),
										new OracleParameter(":AuditBy", OracleDbType.Decimal),
									  };
            parameters[0].Value = id;
            parameters[1].Value = status;
            parameters[2].Value = auditBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public bool UpdateWaitDeliverFeeStatus(string kid, int status, int auditBy)
        {
            SqlStr = @"UPDATE  FMS_StationDeliverFeeWait
						SET     Status = :Status,AuditBy=:AuditBy,AuditTime=SysDate,IsChange=1
						WHERE   EffectKid = :EffectKid
								AND IsDeleted = 0";
            OracleParameter[] parameters ={
										new OracleParameter(":EffectKid", OracleDbType.Varchar2,40),
										new OracleParameter(":Status", OracleDbType.Varchar2,5),
										new OracleParameter(":AuditBy", OracleDbType.Decimal),
									  };
            parameters[0].Value = kid;
            parameters[1].Value = status;
            parameters[2].Value = auditBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, SqlStr, parameters) > 0;
        }

        public int GetWaitDeliverFeeyFeeId(int feeid)
        {
            string sql = @"select count(1) from fms_stationdeliverfeewait where isdeleted=0 and feeid=:feeid";
            OracleParameter[] parameters ={
                                              new OracleParameter(":feeid",OracleDbType.Decimal),
                                         };
            parameters[0].Value = feeid;
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection,CommandType.Text,sql,parameters));
        }

        public DataSet GetExportData()
        {
            DataSet ds = new DataSet();
            SqlStr = @"
with t as(
select ExpressCompanyID,CompanyName,CompanyAllName from ExpressCompany where CompanyFlag in (1,2,3) and DistributionCode<>'rfd' and IsDeleted=0
union all
select ExpressCompanyID,CompanyName,CompanyAllName from ExpressCompany where CompanyFlag=2 and DistributionCode='rfd' and IsDeleted=0)
select * from t";
            DataSet dsExpress=OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr);
            DataTable dtExpress = dsExpress.Tables[0].Copy();
            dtExpress.TableName = "dtExpress";
            ds.Tables.Add(dtExpress);

            SqlStr = @"
SELECT   ExpressCompanyID,
		CompanyName
FROM     ExpressCompany  ec
WHERE    IsDeleted = 0 AND CompanyFlag = 1";
            DataSet dsSort = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr);
            DataTable dtSort = dsSort.Tables[0].Copy();
            dtSort.TableName = "dtSort";
            ds.Tables.Add(dtSort);

            SqlStr = @"
SELECT sci.CodeNo
FROM   StatusCodeInfo sci
WHERE  sci.CodeType = 'AreaType'
	   AND sci.Enabled = 1";
            DataSet dsAreaType = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr);
            DataTable dtAreaType = dsAreaType.Tables[0].Copy();
            dtAreaType.TableName = "dtAreaType";
            ds.Tables.Add(dtAreaType);

            SqlStr = @"
SELECT  ID ,
		MerchantName
FROM    MerchantBaseInfo  mbi
WHERE   IsDeleted = 0
";
            DataSet dsMerchant = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr);
            DataTable dtMerchant = dsMerchant.Tables[0].Copy();
            dtMerchant.TableName = "dtMerchant";
            ds.Tables.Add(dtMerchant);

            return ds;
        }

        /// <summary>
        /// 获取待生效列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetDeliverFeeEffect()
        {
            string sql = @"
SELECT fsdfw.FEEID,
fsdfw.MERCHANTID,
fsdfw.STATIONID,
fsdfw.BASICDELIVERFEE,
fsdfw.UPDATEBY,
fsdfw.UPDATETIME,
fsdfw.UPDATECODE,
fsdfw.AUDITBY,
fsdfw.AUDITTIME,
fsdfw.AUDITCODE,
fsdfw.AUDITRESULT,
fsdfw.STATUS,
fsdfw.EXPRESSCOMPANYID,
fsdfw.ISCENTERSORT,
fsdfw.AREATYPE,
fsdfw.CREATEBY,
fsdfw.CREATETIME,
fsdfw.ISDELETED,
fsdfw.DISTRIBUTIONCODE,
fsdfw.ISCHANGE,
fsdfw.EffectKid,
fsdfw.EffectDate,
fsdfw.GoodsCategoryCode,
fsdfw.DELIVERFEE,
fsdfw.IsCod
FROM    FMS_StationDeliverFeeWait  fsdfw
WHERE   fsdfw.IsDeleted =0 and Status=1 and EffectDate=to_date(to_char(sysdate,'yyyy-mm-dd'),'yyyy-mm-dd')";

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql).Tables[0];
        }

        public bool UpdateToEffect(FMS_StationDeliverFee model)
        {
            string sql = @"
UPDATE FMS_StationDeliverFee
   SET MerchantID=:MerchantID
,StationID=:StationID
,BasicDeliverFee=:BasicDeliverFee
,UpdateBy=:UpdateBy
,UpdateTime=:UpdateTime
,UpdateCode=:UpdateCode
,AuditBy=:AuditBy
,AuditTime=:AuditTime
,AuditCode=:AuditCode
,Status=:Status
,ExpressCompanyID=:ExpressCompanyID
,IsCenterSort=:IsCenterSort
,AreaType=:AreaType
,IsDeleted=0
,IsChange =1
,GoodsCategoryCode=:GoodsCategoryCode
,DeliverFee=:DeliverFee
,IsCod=:IsCod
	WHERE FEEID=:ID
";
            OracleParameter[] parameters ={
                                           new OracleParameter(":MerchantID",OracleDbType.Decimal),
                                            new OracleParameter(":StationID",OracleDbType.Decimal),
                                            new OracleParameter(":BasicDeliverFee",OracleDbType.Varchar2,300),
                                            new OracleParameter(":UpdateBy",OracleDbType.Decimal),
                                            new OracleParameter(":UpdateTime",OracleDbType.Date),
                                            new OracleParameter(":UpdateCode",OracleDbType.Varchar2,40),
                                            new OracleParameter(":AuditBy",OracleDbType.Decimal),
                                            new OracleParameter(":AuditTime",OracleDbType.Date),
                                            new OracleParameter(":AuditCode",OracleDbType.Varchar2,40),
                                            new OracleParameter(":Status",OracleDbType.Char,5),
                                            new OracleParameter(":ExpressCompanyID",OracleDbType.Decimal),
                                            new OracleParameter(":IsCenterSort",OracleDbType.Decimal),
                                            new OracleParameter(":AreaType",OracleDbType.Decimal),
                                            new OracleParameter(":ID",OracleDbType.Decimal),
                                            new OracleParameter(":GoodsCategoryCode",OracleDbType.Varchar2,20),
                                            new OracleParameter(":DeliverFee",OracleDbType.Varchar2),
                                            new OracleParameter(":IsCod",OracleDbType.Decimal),
                                      };
            parameters[0].Value = model.MerchantID;
            parameters[1].Value = model.StationID;
            parameters[2].Value = model.BasicDeliverFee;
            parameters[3].Value = model.UpdateUser;
            parameters[4].Value = model.UpdateTime;
            parameters[5].Value = model.UpdateUserCode;
            parameters[6].Value = model.AuditBy;
            parameters[7].Value = model.AuditTime;
            parameters[8].Value = model.AuditCode;
            parameters[9].Value = (int)model.Status;
            parameters[10].Value = model.ExpressCompanyID;
            parameters[11].Value = model.IsCenterSort;
            parameters[12].Value = model.AreaType;
            parameters[13].Value = model.ID;
            parameters[14].Value = model.GoodsCategoryCode;
            parameters[15].Value = model.DeliverFee;
            parameters[16].Value = model.IsCod;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public bool DeleteWaitStationDeliverFee(string effectKid)
        {
            string sql = @"
UPDATE FMS_StationDeliverFeeWait
   SET IsDeleted=1
,IsChange =1
,UpdateBy=0
,UpdateTime=sysdate 
WHERE EffectKid=:EffectKid";
            OracleParameter[] parameters ={
                                              new OracleParameter(":EffectKid",OracleDbType.Varchar2,40),
                                         };
            parameters[0].Value = effectKid;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public DataTable GetBasicDeliverFeeByCondition(int merchantId, string warehouseId, int areaType, int isCategory, string waybillCategory, string distributionCode)
        {
            string sql = @"SELECT fsdf.MerchantID,fsdf.StationID,fsdf.GoodsCategoryCode,fsdf.ExpressCompanyID,
                                    fsdf.BasicDeliverFee,fsdf.DeliverFee,fsdf.IsCod,fsdf.ISEXPRESS
                            FROM FMS_StationDeliverFee fsdf WHERE fsdf.Status IN ('1') AND IsDeleted=0 {0}";

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameterList = new List<OracleParameter>();

            if (merchantId > 0)
            {
                sbWhere.Append(" AND fsdf.MerchantID= :MerchantID");
                parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = merchantId });
            }

            if (!string.IsNullOrEmpty(warehouseId))
            {
                sbWhere.Append(" AND fsdf.ExpressCompanyID= :ExpressCompanyID");
                parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = warehouseId });
            }

            if (areaType > 0)
            {
                sbWhere.Append(" AND fsdf.AreaType= :AreaType");
                parameterList.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) { Value = areaType });
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND fsdf.DistributionCode= :DistributionCode");
                parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
            }

            if (isCategory == 1 && !string.IsNullOrEmpty(waybillCategory))
            {
                sbWhere.Append(" AND fsdf.GoodsCategoryCode= :GoodsCategoryCode");
                parameterList.Add(new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2, 20) { Value = waybillCategory });
            }
            sql = string.Format(sql, sbWhere.ToString());
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterList.ToArray()));

            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public DataTable GetExportDeliverFeeWaitList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            SqlStr = @"with t as(SELECT 
								mbi.MerchantName 商家,
								
								CASE fsdfw.StationID WHEN 11 THEN '全部'
                                ELSE(
                                CASE WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.CompanyName
                                      ELSE ec.AccountCompanyName
                                END ) 配送公司,
								(CASE fsdfw.IsCenterSort WHEN 0 THEN '否' WHEN 1 THEN '是' ELSE '否' END) 是否分拣中心 ,
								ec2.CompanyName  分拣中心,
                                fsdfw.AreaType 区域类型,
                                gc.GoodsCategoryName 货物品类，
                                CASE WHEN fsdfw.IsCod=0 THEN '否' ELSE '是' END 是否区分COD,
								fsdfw.BasicDeliverFee COD价格公式,
                                fsdfw.DeliverFee 非COD价格公式,
				                fsdfw.CreateBy 创建人,
								fsdfw.CreateTime 创建时间,
								fsdfw.UpdateBy 更新人,
								fsdfw.UpdateTime 更新时间,
								sci.CodeDesc  审核状态 ,
								fsdfw.AuditBy 审核人,
								fsdfw.AuditTime 审核时间
                                
						FROM    FMS_StationDeliverFeeWait  fsdfw
								JOIN MerchantBaseInfo  mbi ON fsdfw.MerchantID = mbi.ID
								JOIN ExpressCompany  ec ON ec.ExpressCompanyID = fsdfw.StationID
								JOIN StatusCodeInfo  sci ON cast(sci.CodeNo as NUMBER) = cast(fsdfw.Status as NUMBER)
																			 AND sci.CodeType = 'AreaTypeAudit'
								LEFT JOIN ExpressCompany  ec2 ON ec2.ExpressCompanyID = fsdfw.ExpressCompanyID
																				  AND ec2.CompanyFlag = 1
                                LEFT JOIN GoodsCategory gc on gc.GoodsCategoryCode=fsdfw.GoodsCategoryCode
						WHERE   ( fsdfw.IsDeleted =0 ) {0} ) select * from t order by 商家,配送公司,分拣中心";

            string sqlWhere = string.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdfw", ref parameters);
            List<OracleParameter> parametersTmp = new List<OracleParameter>();

            parameters.AddRange(parametersTmp);

            SqlStr = string.Format(SqlStr, sqlWhere);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public DataTable GetExportDeliverFeeList(string merchantId, string expressCompanyId, string areaType, string sortCenterId, string auditStatus, string distributionCode, string categoryCode)
        {
            SqlStr = @"with t as(
								 select mbi.MerchantName 商家 ,
							    (CASE fsdf.StationID WHEN 11 THEN '全部' 
							     ELSE 
							     CASE  nvl(ec.ACCOUNTCOMPANYNAME,'') 
							     WHEN '' THEN  ec.CompanyName ELSE ec.ACCOUNTCOMPANYNAME END   END) 配送公司 ,
								(CASE fsdf.IsCenterSort WHEN 0 THEN '否' WHEN 1 THEN '是' ELSE '否' END) 是否分拣中心 ,
								ec2.CompanyName  分拣中心,fsdf.AreaType 区域类型,
                                gc.GoodsCategoryName 货物品类,
								CASE WHEN fsdf.IsCod=0 THEN '否' ELSE '是' END 是否区分COD,
								fsdf.BasicDeliverFee COD价格公式,
                                fsdf.DeliverFee 非COD价格公式,
                                sci.CodeDesc  审核状态 ,
								fsdf.CreateBy 创建人,
								fsdf.CreateTime 创建时间,
								fsdf.UpdateBy 更新人,
								fsdf.UpdateTime 更新时间,
								fsdf.AuditBy 审核人,
								fsdf.AuditTime 审核时间
                               
						FROM    FMS_StationDeliverFee  fsdf
								JOIN MerchantBaseInfo  mbi ON fsdf.MerchantID = mbi.ID
								JOIN ExpressCompany  ec ON ec.ExpressCompanyID = fsdf.StationID
								JOIN StatusCodeInfo  sci ON cast(sci.CodeNo as NUMBER) = cast(fsdf.Status as NUMBER)
																			 AND sci.CodeType = 'AreaTypeAudit'
								LEFT JOIN ExpressCompany  ec2 ON ec2.ExpressCompanyID = fsdf.ExpressCompanyID
																				  AND ec2.CompanyFlag = 1
                                LEFT JOIN GoodsCategory gc on gc.GoodsCategoryCode=fsdf.GoodsCategoryCode
						WHERE   ( fsdf.IsDeleted =0 ) {0} ) select * from t order by 商家,配送公司,分拣中心";

            string sqlWhere = string.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();

            sqlWhere = BuildSearchCondition(merchantId, expressCompanyId, areaType, sortCenterId, auditStatus, distributionCode, categoryCode, "fsdf", ref parameters);
            List<OracleParameter> parametersTmp = new List<OracleParameter>();
           

            parameters.AddRange(parametersTmp);

            SqlStr = string.Format(SqlStr, sqlWhere);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, SqlStr, this.ToParameters(parameters.ToArray())).Tables[0];
        }
	}
}
