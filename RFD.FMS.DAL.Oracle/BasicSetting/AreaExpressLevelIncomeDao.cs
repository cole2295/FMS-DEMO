using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Util.OraclePageCommon;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class AreaExpressLevelIncomeDao : OracleDao, IAreaExpressLevelIncomeDao
    {
        private string sqlStr = "";

        public DataTable SearchArea(string provinceId, string cityId, string areaId, string merchantId,
                                    string distributionCode)
        {
            sqlStr =
                @"SELECT distinct p.ProvinceID,
                p.ProvinceName,
                c.CityID,
                c.CityName,
                a.AreaID,
                a.AreaName,
                ( CASE WHEN ( aeli.AreaType IS NOT NULL
                             OR aeli.EffectAreaType IS NOT NULL
                             )
                             AND isEnable IN ( 1,2, 3 ) THEN '√'
                          ELSE ''
                       END) IsAreaType
             FROM   Province p
                JOIN City c
                   ON  c.ProvinceID = p.ProvinceID
                JOIN Area a
                   ON  a.CityID = c.CityID
                LEFT JOIN AreaExpressLevelIncome aeli
								   ON  aeli.AreaID = a.AreaID AND aeli.DistributionCode=:DistributionCode
					   WHERE  p.IsDeleted = 0
							  AND c.IsDeleted = 0
							  AND a.IsDeleted = 0
							  {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(provinceId))
            {
                sbWhere.Append(" AND p.ProvinceID=:ProvinceID ");
                parameters.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 20) {Value = provinceId});
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                sbWhere.Append(" AND c.CityID=:CityID ");
                parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 20) {Value = cityId});
            }

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=:AreaID ");
                parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) {Value = areaId});
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aeli.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) {Value = merchantId});
            }

            if (!string.IsNullOrEmpty(merchantId) && int.Parse(merchantId) > 0)
            {
                sbWhere.Append(" AND aeli.IsEnable in (1,2,3) ");
            }

            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                               {Value = distributionCode});

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return
                OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr,
                                            ToParameters(parameters.ToArray())).Tables[0];
        }

        public DataTable SearchAreaType(string areaId, string distributionCode)
        {
            sqlStr =
                @"SELECT aeli.AutoID,
							   aeli.AreaID,
							   aeli.MerchantID,
							   mbi.MerchantName,
							   aeli.WareHouseID,
							   ec.CompanyName,
							   ec1.CompanyName AS ExpressCompanyName,
							   (case when aeli.IsEnable=3 then null else aeli.AreaType end) AreaType,
							   (case when AuditStatus in (0,1,3) THEN aeli.EffectAreaType ELSE NULL end) EffectAreaType,
							   (case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已同步' when 3 then '置回' else null end) AuditStatusStr,
							   (CASE aeli.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END) EnableStr 
						FROM   AreaExpressLevelIncome aeli
							   LEFT JOIN ExpressCompany ec
									ON  ec.ExpressCompanyID = aeli.WareHouseID
							   JOIN MerchantBaseInfo mbi 
									ON mbi.ID = aeli.MerchantID
							   JOIN ExpressCompany ec1
									ON aeli.ExpressCompanyID=ec1.ExpressCompanyID
						WHERE aeli.IsEnable in( 1,2,3) {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND aeli.AreaID=:AreaID ");
                parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 20) {Value = areaId});
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aeli.DistributionCode=:DistributionCode ");
                parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                                   {Value = distributionCode});
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return
                OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr,
                                            this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public DataTable SearchAreaTypeByAutoId(string autoId)
        {
            sqlStr =
                @"SELECT   aeli.AutoID,
								aeli.AreaID,
								aeli.ExpressCompanyID,
							    aeli.MerchantID,
							    aeli.WareHouseID,
								aeli.IsEnable,
							    aeli.AreaType,
								aeli.EffectAreaType,
								aeli.DoDate,
								aeli.CreateBy,
								aeli.CreateTime,
								aeli.UpdateBy,
								aeli.UpdateTime,
								aeli.AuditBy,
								aeli.AuditStatus,
                                aeli.DistributionCode
						FROM   AreaExpressLevelIncome aeli
						WHERE AutoID=:AutoID";
            OracleParameter[] parameters = {
                                               new OracleParameter(":AutoID", OracleDbType.Decimal),
                                           };
            parameters[0].Value = autoId;
            return OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, parameters).Tables[0];
        }

        public bool AddAreaType(ref AreaExpressLevelIncome areaExpressLevelIncome)
        {
            sqlStr =
                @"SELECT count(1)
							  FROM   AreaExpressLevelIncome
							  WHERE  (1=1) AND IsEnable in (1,2,3) {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameterList1 = new List<OracleParameter>();
            sbWhere.Append(" AND MerchantID = :MerchantID");
            parameterList1.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal)
                                   {Value = areaExpressLevelIncome.MerchantID});

            sbWhere.Append(" AND AreaID = :AreaID");
            parameterList1.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2)
                                   {Value = areaExpressLevelIncome.AreaID});

            sbWhere.Append(" AND WareHouseID = :WareHouseID");
            parameterList1.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2)
                                   {Value = areaExpressLevelIncome.WareHouseID});

            sbWhere.Append(" AND DistributionCode = :DistributionCode");
            parameterList1.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2)
                                   {Value = areaExpressLevelIncome.DistributionCode});
            //if (areaExpressLevelIncome.EffectAreaType != null)
            //{
            //    sbWhere.Append(" AND EffectAreaType = :EffectAreaType");
            //    parameterList1.Add(new OracleParameter(":EffectAreaType", OracleDbType.Decimal) { Value = areaExpressLevelIncome.EffectAreaType });
            //}
            if (!string.IsNullOrEmpty(areaExpressLevelIncome.GoodsCategoryCode))
            {
                sbWhere.Append(" AND GoodsCategoryCode = :GoodsCategoryCode");
                parameterList1.Add(new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2)
                                       {Value = areaExpressLevelIncome.GoodsCategoryCode});
            }
            //if (areaExpressLevelIncome.IsExpress==1)
            //{
                sbWhere.Append(" AND ExpressCompanyID = :ExpressCompanyID");
                parameterList1.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = areaExpressLevelIncome.ExpressCompanyID });
            //}
            //
            sqlStr = String.Format(sqlStr, sbWhere.ToString());
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, parameterList1.ToArray());
            if (obj != null)
            {
                if (Convert.ToInt32(obj) > 0)
                {
                    return false;
                }
            }
            areaExpressLevelIncome.AutoId = GetIdNew("SEQ_AREAEXPRESSLEVELINCOME");
            sqlStr =
                @"			INSERT INTO AreaExpressLevelIncome
								   (autoid,AreaID
								   ,MerchantID
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
									,ExpressCompanyID
                                    ,DistributionCode
                                    ,IsChange
                                    ,GoodsCategoryCode
									,IsExpress)
							 VALUES
								   (:autoid
                                   ,:AreaID
								   ,:MerchantID
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
									,:ExpressCompanyID
                                    ,:DistributionCode
                                    ,:IsChange
                                    ,:GoodsCategoryCode
									,:IsExpress)
						";
            OracleParameter[] parameters = {
                                               new OracleParameter(":AreaID", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":MerchantID", OracleDbType.Decimal),
                                               new OracleParameter(":WareHouseID", OracleDbType.Varchar2, 80),
                                               new OracleParameter(":AreaType", OracleDbType.Decimal),
                                               new OracleParameter(":IsEnable", OracleDbType.Decimal),
                                               new OracleParameter(":EffectAreaType", OracleDbType.Decimal),
                                               new OracleParameter(":DoDate", OracleDbType.Date),
                                               new OracleParameter(":CreateBy", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":UpdateBy", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":AuditStatus", OracleDbType.Decimal),
                                               new OracleParameter(":AuditBy", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal),
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100),
                                               new OracleParameter(":IsChange", OracleDbType.Decimal),
                                               new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2, 20),
                                               new OracleParameter(":autoid", OracleDbType.Decimal),
                                               new OracleParameter(":IsExpress",OracleDbType.Decimal), 
                                           };
            parameters[0].Value = areaExpressLevelIncome.AreaID;
            parameters[1].Value = areaExpressLevelIncome.MerchantID;
            parameters[2].Value = areaExpressLevelIncome.WareHouseID;
            parameters[3].Value = areaExpressLevelIncome.AreaType;
            parameters[4].Value = areaExpressLevelIncome.Enable;
            parameters[5].Value = areaExpressLevelIncome.EffectAreaType;
            parameters[6].Value = areaExpressLevelIncome.DoDate;
            parameters[7].Value = areaExpressLevelIncome.CreateBy;
            parameters[8].Value = areaExpressLevelIncome.UpdateBy;
            parameters[9].Value = areaExpressLevelIncome.AuditStatus;
            parameters[10].Value = areaExpressLevelIncome.AuditBy;
            parameters[11].Value = areaExpressLevelIncome.ExpressCompanyID;
            parameters[12].Value = areaExpressLevelIncome.DistributionCode;
            parameters[13].Value = 1;
            parameters[14].Value = areaExpressLevelIncome.GoodsCategoryCode;
            parameters[15].Value = areaExpressLevelIncome.AutoId;
            parameters[16].Value = areaExpressLevelIncome.IsExpress;
            if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) != 1)
            {
                return false;
            }
            return true;
        }

        public bool UpdateExpressV2(AreaExpressLevelIncome areaExpressLevelIncome)
        {
            sqlStr =
               @"	UPDATE AreaExpressLevelIncome
							SET    UpdateBy = :UpdateBy,
								   UpdateTime = SysDate,
								   AuditStatus = :AuditStatus,
                                   IsChange=:IsChange,
                                   IsExpress=:IsExpress,
                                   EXPRESSCOMPANYID=:EXPRESSCOMPANYID
							WHERE  AutoID = :AutoID    AND IsEnable in (1,2,3) ";
            OracleParameter[] parameters =
                {
                    new OracleParameter(":UpdateBy", OracleDbType.Decimal),
                    new OracleParameter(":AuditStatus", OracleDbType.Decimal),
                    new OracleParameter(":AutoID", OracleDbType.Decimal),
                    new OracleParameter(":IsChange", OracleDbType.Decimal),
                    new OracleParameter(":EXPRESSCOMPANYID",OracleDbType.Decimal), 
                    new OracleParameter(":IsExpress",OracleDbType.Decimal), 
                };
            parameters[0].Value = areaExpressLevelIncome.UpdateBy;
            parameters[1].Value = areaExpressLevelIncome.AuditStatus;
            parameters[2].Value = areaExpressLevelIncome.AutoId;
            parameters[3].Value = 1;
            parameters[4].Value = areaExpressLevelIncome.ExpressCompanyID;
            parameters[5].Value = areaExpressLevelIncome.IsExpress;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        public bool UpdateAreaType(AreaExpressLevelIncome areaExpressLevelIncome, out int autoId)
        {
            sqlStr =
                @"SELECT count(1)
							  FROM   AreaExpressLevelIncome
							  WHERE  (1=1) and AND IsEnable in (1,2,3) {0};";
            sqlStr = String.Format(sqlStr, areaExpressLevelIncome.ExpressCompanyID, areaExpressLevelIncome.MerchantID,
                                   areaExpressLevelIncome.AreaID, areaExpressLevelIncome.WareHouseID,
                                   areaExpressLevelIncome.DistributionCode);
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameterList1 = new List<OracleParameter>();
            sbWhere.Append(" AND MerchantID = :MerchantID");
            parameterList1.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal)
                                   {Value = areaExpressLevelIncome.MerchantID});

            sbWhere.Append(" AND ExpressCompanyID = :ExpressCompanyID");
            parameterList1.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal)
                                   {Value = areaExpressLevelIncome.ExpressCompanyID});

            sbWhere.Append(" AND AreaID = :AreaID");
            parameterList1.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2)
                                   {Value = areaExpressLevelIncome.AreaID});

            sbWhere.Append(" AND WareHouseID = :WareHouseID");
            parameterList1.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2)
                                   {Value = areaExpressLevelIncome.WareHouseID});

            sbWhere.Append(" AND DistributionCode = :DistributionCode");
            parameterList1.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2)
                                   {Value = areaExpressLevelIncome.DistributionCode});

            sqlStr = String.Format(sqlStr, sbWhere.ToString());
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, parameterList1.ToArray());
            if (obj == null)
            {
                autoId = 0;
                return false;
            }

            if (Convert.ToInt32(obj) == 0)
            {
                autoId = 0;
                return false;
            }

            sqlStr =
                @"	UPDATE AreaExpressLevelIncome
							SET    EffectAreaType = :EffectAreaType,
								   UpdateBy = :UpdateBy,
								   UpdateTime = SysDate,
								   AuditStatus = :AuditStatus
							WHERE  AreaID = :AreaID
								   AND ExpressCompanyID=:ExpressCompanyID
								   AND MerchantID = :MerchantID
								   AND IsEnable in (1,2,3)
                                   AND DistributionCode=:DistributionCode
						";

            OracleParameter[] parameters = {
                                               new OracleParameter(":AreaID", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":MerchantID", OracleDbType.Decimal),
                                               new OracleParameter(":EffectAreaType", OracleDbType.Decimal),
                                               new OracleParameter(":UpdateBy", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":AuditStatus", OracleDbType.Decimal),
                                               new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal),
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100),
                                           };
            parameters[0].Value = areaExpressLevelIncome.AreaID;
            parameters[1].Value = areaExpressLevelIncome.MerchantID;
            parameters[2].Value = areaExpressLevelIncome.EffectAreaType;
            parameters[3].Value = areaExpressLevelIncome.UpdateBy;
            parameters[4].Value = areaExpressLevelIncome.AuditStatus;
            parameters[5].Value = areaExpressLevelIncome.ExpressCompanyID;
            parameters[6].Value = areaExpressLevelIncome.DistributionCode;
            if (OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) <= 0)
            {
                autoId = 0;
                return false;
            }

            sqlStr =
                @"	SELECT AutoID FROM AreaExpressLevelIncome WHERE ExpressCompanyID={0} AND 
										AreaID = '{1}' AND MerchantID = {2} AND IsEnable in (1,2,3) AND DistributionCode='{3}'";
            sqlStr = String.Format(sqlStr, areaExpressLevelIncome.ExpressCompanyID, areaExpressLevelIncome.AreaID,
                                   areaExpressLevelIncome.MerchantID, areaExpressLevelIncome.DistributionCode);
            obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, null);
            if (obj == null)
            {
                autoId = 0;
                return false;
            }

            if (Convert.ToInt32(obj) == 0)
            {
                autoId = 0;
                return false;
            }

            autoId = Convert.ToInt32(obj);
            return true;
        }
        public bool ExistAreaExpress( AreaExpressLevelIncome areaExpressLevelIncome)
        {
            sqlStr =
                   @"SELECT count(1)
							  FROM   AreaExpressLevelIncome
							  WHERE  (1=1) AND IsEnable in (1,2,3) {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameterList1 = new List<OracleParameter>();
            sbWhere.Append(" AND MerchantID = :MerchantID");
            parameterList1.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = areaExpressLevelIncome.MerchantID });

         

            sbWhere.Append(" AND WareHouseID = :WareHouseID");
            parameterList1.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2) { Value = areaExpressLevelIncome.WareHouseID });

            sbWhere.Append(" AND DistributionCode = :DistributionCode");
            parameterList1.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2) { Value = areaExpressLevelIncome.DistributionCode });
            if (!string.IsNullOrEmpty(areaExpressLevelIncome.GoodsCategoryCode))
            {
                sbWhere.Append(" AND GoodsCategoryCode = :GoodsCategoryCode");
                parameterList1.Add(new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2) { Value = areaExpressLevelIncome.GoodsCategoryCode });
            }

            if (areaExpressLevelIncome.EffectAreaType!=null)
            {
                sbWhere.Append(" AND EffectAreaType = :EffectAreaType");
                parameterList1.Add(new OracleParameter(":EffectAreaType", OracleDbType.Decimal) { Value = areaExpressLevelIncome.EffectAreaType });
            }
            if (!string.IsNullOrEmpty(areaExpressLevelIncome.AreaID)&&int.Parse(areaExpressLevelIncome.AreaID)>0)
            {
                sbWhere.Append(" AND AreaID = :AreaID");
                parameterList1.Add(new OracleParameter(":AreaID", OracleDbType.Decimal) { Value = areaExpressLevelIncome.AreaID }); 
            }
           
            //if (areaExpressLevelIncome.IsExpress==1)
            //{
            sbWhere.Append(" AND ExpressCompanyID = :ExpressCompanyID");
            parameterList1.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = areaExpressLevelIncome.ExpressCompanyID });
            //}
            //
            sqlStr = String.Format(sqlStr, sbWhere.ToString());
            object obj = OracleHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr, parameterList1.ToArray());
            if (obj != null)
            {
                if (Convert.ToInt32(obj) > 1)
                {
                    return false;
                }
            }
            return true;
        }

        public DataTable GetAreaExpressByID(int id)
        {
            sqlStr =
                   @"
SELECT * FROM AREAEXPRESSLEVELINCOME
WHERE  (1=1) AND IsEnable in (1,2,3) and  AutoID=:AutoID  ";
        
            var parameterList1 = new List<OracleParameter>
                                     {new OracleParameter(":AutoID", OracleDbType.Decimal) {Value = id}};

            DataSet dataSet = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sqlStr, parameterList1.ToArray());
            if (dataSet != null && dataSet.Tables.Count>0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }
        public bool UpdateAreaTypeV2(AreaExpressLevelIncome areaExpressLevelIncome)
        {
            sqlStr =
                @"	UPDATE AreaExpressLevelIncome
							SET    EffectAreaType = :EffectAreaType,
								   UpdateBy = :UpdateBy,
								   UpdateTime = SysDate,
								   AuditStatus = :AuditStatus,
                                   IsChange=:IsChange                           
							WHERE  AutoID = :AutoID";
            OracleParameter[] parameters =
                {
                    new OracleParameter(":EffectAreaType", OracleDbType.Decimal),
                    new OracleParameter(":UpdateBy", OracleDbType.Decimal),
                    new OracleParameter(":AuditStatus", OracleDbType.Decimal),
                    new OracleParameter(":AutoID", OracleDbType.Decimal),
                    new OracleParameter(":IsChange", OracleDbType.Decimal),
                  
                };
            parameters[0].Value = areaExpressLevelIncome.EffectAreaType;
            parameters[1].Value = areaExpressLevelIncome.UpdateBy;
            parameters[2].Value = areaExpressLevelIncome.AuditStatus;
            parameters[3].Value = areaExpressLevelIncome.AutoId;
            parameters[4].Value = 1;
           
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        public bool DeleteAreaType(string autoId, int updateBy)
        {
            sqlStr =
                @"UPDATE AreaExpressLevelIncome SET IsEnable=2,AuditStatus=0,UpdateBy=:UpdateBy,UpdateTime=SysDate WHERE AutoID=:AutoID";
            OracleParameter[] parameters = {
                                               new OracleParameter(":AutoID", OracleDbType.Decimal),
                                               new OracleParameter(":UpdateBy", OracleDbType.Varchar2, 200),
                                           };
            parameters[0].Value = autoId;
            parameters[1].Value = updateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sqlStr, parameters) > 0;
        }

        //新加日志
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool AddAreaExpLevelIncomeLog(AreaExpressLevelIncomeLog model)
        {
            if (model.LogID <= 0)
            {
                model.LogID = GetIdNew("SEQ_AREAEXPRESSLEVELINCOMELOG");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into AreaExpressLevelIncomeLog(");
            strSql.Append(
                "LOGID,AreaID,MerchantID,WarehouseId,AreaType,LogText,IsEnable,CreateBy,CreateTime,ExpressCompanyID,DistributionCode)");
            strSql.Append(" values (");
            strSql.Append(
                " :LOGID,:AreaID,:MerchantID,:WarehouseId,:AreaType,:LogText,1,:CreateBy,SysDate,:ExpressCompanyID,:DistributionCode)");

            OracleParameter[] parameters = {
                                               new OracleParameter(":AreaID", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":MerchantID", OracleDbType.Decimal, 4),
                                               new OracleParameter(":WarehouseId", OracleDbType.Varchar2, 80),
                                               new OracleParameter(":AreaType", OracleDbType.Decimal, 4),
                                               new OracleParameter(":LogText", OracleDbType.Varchar2, 500),
                                               new OracleParameter(":CreateBy", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal),
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100),
                                               new OracleParameter(":LOGID",OracleDbType.Decimal), 
                                           };
            parameters[0].Value = model.AreaID;
            parameters[1].Value = model.MerchantID;
            parameters[2].Value = model.WarehouseId;
            parameters[3].Value = model.AreaType;
            parameters[4].Value = model.LogText;
            parameters[5].Value = model.CreateBy;
            parameters[6].Value = model.ExpressCompanyID;
            parameters[7].Value = model.DistributionCode;
            parameters[8].Value = model.LogID;

            int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rowCount == 0) return false;

            return true;
        }

        //查询区域类型
        public DataTable SearchAreaMerchantLevel(int status, string areaid, string cityid, string provinceid,
                                                 int merchantid, int areatype, string distributionCode, ref PageInfo pi)
        {
            string strResult =
                @"select p.provincename,cityname,areaname,a1.areaid,a1.dodate,e.employeename,a1.AuditTime,s.statusname,a1.auditstatus
                                                from (select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael where ael.auditStatus=:auditStatus and DistributionCode=:DistributionCode
                                                group by ael.areaid ) as a
                                                join AreaExpressLevelIncome a1 on a1.autoid=a.id
                                                join Area ar on a.areaid=ar.areaid
                                                join City c on ar.cityid=c.cityid
                                                join Province p  on c.provinceid=p.ProvinceID 
                                                join StatusInfo s on a1.auditstatus=s.statusno and s.statustypeno='306'
                                                left join employee e on a1.auditby=e.employeeid
                                                 where 1=1 ";


            StringBuilder count = new StringBuilder();
            count.AppendFormat(
                @"select count(1) from (select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael where ael.auditStatus=:auditStatus and DistributionCode=:DistributionCode
                                group by ael.areaid ) as a
                                join AreaExpressLevelIncome a1 on a1.autoid=a.id
                                join Area ar on a.areaid=ar.areaid
                                join City c on ar.cityid=c.cityid
                                join Province p  on c.provinceid=p.ProvinceID 
                                join StatusInfo s on a1.auditstatus=s.statusno and s.statustypeno='306'
                                left join employee e on a1.auditby=e.employeeid
                                 where 1=1 ");

            OracleParameter[] parameters = {
                                               new OracleParameter(":areaid", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":cityid", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":provinceid", OracleDbType.Varchar2, 200),
                                               new OracleParameter(":merchantid", OracleDbType.Decimal, 4),
                                               new OracleParameter(":areatype", OracleDbType.Decimal, 4),
                                               new OracleParameter(":auditStatus", OracleDbType.Decimal, 4),
                                               new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                                           };


            parameters[5].Value = status;
            parameters[6].Value = distributionCode;

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ar.areaid=:areaid";
                count.Append(" and ar.areaid=:areaid");
                parameters[0].Value = areaid;
            }

            if (!string.IsNullOrEmpty(cityid))
            {
                strResult += " and c.cityid=:cityid";
                count.Append(" and c.cityid=:cityid");
                parameters[1].Value = cityid;
            }

            if (!string.IsNullOrEmpty(provinceid))
            {
                strResult += " and p.provinceid=:provinceid";
                count.Append(" and p.provinceid=:provinceid");
                parameters[2].Value = provinceid;
            }

            if (merchantid > 0)
            {
                strResult += " and a1.merchantid=:merchantid";
                count.Append(" and a1.merchantid=:merchantid");
                parameters[3].Value = merchantid;
            }

            if (areatype > 0)
            {
                strResult += " and a1.areatype=:areatype";
                count.Append(" and a1.areatype=:areatype");
                parameters[4].Value = areatype;
            }
            int itemcount = Convert.ToInt32(GetOrderInfoCount(count.ToString(), parameters));
            string newSqlQuery = "";
            pi.SetItemCount(itemcount);
            int begin = pi.CurrentPageBeginItemIndex;
            int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;


            if (begin > 1)
            {
                newSqlQuery =
                    String.Format(
                        " SELECT * FROM ( SELECT  ROW_NUMBER() over(order by areaid ) as rowno,allrecord.* FROM ( " +
                        strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno>={0} AND rowno<{1} ", begin,
                        end);
            }
            else
            {
                newSqlQuery =
                    String.Format(
                        " SELECT * FROM (  SELECT  ROW_NUMBER() over(order by areaid) as rowno,allrecord.* FROM ( " +
                        strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno<{0} ", end);
            }

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, newSqlQuery, parameters).Tables[0];
        }

        //查询区域类型 (收入)
        public DataTable SearchAreaMerchantLevel(int status, string areaid, string cityid, string provinceid,
                                                 int merchantid, int areatype, int expresscompanyid,
                                                 string distributionCode, ref PageInfo pi)
        {
            string sql =
                @"select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael where ael.auditStatus=:auditStatus AND ael.DistributionCode=:DistributionCode";


            string sqlcount =
                @"select ael.areaid,min(ael.autoid) as id from AreaExpressLevelIncome ael where ael.auditStatus=:auditStatus AND ael.DistributionCode=:DistributionCode";

            List<OracleParameter> parameters = new List<OracleParameter>();
            List<OracleParameter> parameters1 = new List<OracleParameter>();

            //OracleParameter[] parameters = {
            //        new OracleParameter(":areaid", OracleDbType.Varchar2,200),
            //        new OracleParameter(":cityid", OracleDbType.Varchar2,200),
            //        new OracleParameter(":provinceid", OracleDbType.Varchar2,200),
            //        new OracleParameter(":merchantid", OracleDbType.Decimal,4),
            //        new OracleParameter(":areatype", OracleDbType.Decimal,4),
            //        new OracleParameter(":expresscompanyid", OracleDbType.Decimal,4),
            //        new OracleParameter(":auditStatus", OracleDbType.Decimal,4),
            //        new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100)
            //        };

            parameters.Add(new OracleParameter(":auditStatus", OracleDbType.Decimal, 4) {Value = status});
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                               {Value = distributionCode});
            parameters1.Add(new OracleParameter(":auditStatus", OracleDbType.Decimal, 4) {Value = status});
            parameters1.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                                {Value = distributionCode});

            if (merchantid > 0)
            {
                sql += " and ael.merchantid=:merchantid";
                sqlcount += " and ael.merchantid=:merchantid";

                parameters.Add(new OracleParameter(":merchantid", OracleDbType.Decimal, 4) {Value = merchantid});
                parameters1.Add(new OracleParameter(":merchantid", OracleDbType.Decimal, 4) {Value = merchantid});
            }

            if (areatype > 0)
            {
                sql += " and ael.areatype=:areatype";
                sqlcount += " and ael.areatype=:areatype";

                parameters.Add(new OracleParameter(":areatype", OracleDbType.Decimal, 4) {Value = areatype});
                parameters1.Add(new OracleParameter(":areatype", OracleDbType.Decimal, 4) {Value = areatype});
            }

            if (expresscompanyid > 0)
            {
                sql += " and ael.expresscompanyid=:expresscompanyid";
                sqlcount += " and ael.expresscompanyid=:expresscompanyid";

                parameters.Add(new OracleParameter(":expresscompanyid", OracleDbType.Decimal, 4)
                                   {Value = expresscompanyid});
                parameters1.Add(new OracleParameter(":expresscompanyid", OracleDbType.Decimal, 4)
                                    {Value = expresscompanyid});
            }

            sql += " group by ael.areaid ";
            sqlcount += " group by ael.areaid";


            string strResult =
                string.Format(
                    @"select p.provincename,cityname,areaname,a1.areaid,a1.dodate,e.employeename,a1.AuditTime,s.statusname,a1.auditstatus
                                                from ({0}) a
                                                join AreaExpressLevelIncome a1 on a1.autoid=a.id
                                                join Area ar on a.areaid=ar.areaid
                                                join City c on ar.cityid=c.cityid
                                                join Province p  on c.provinceid=p.ProvinceID 
                                                join StatusInfo s on a1.auditstatus=s.statusno and s.statustypeno='306'
                                                left join employee e on a1.auditby=e.employeeid
                                                 where 1=1 ",
                    sql);


            StringBuilder count = new StringBuilder();
            count.AppendFormat(
                @"select count(1) from ({0}) a
                                join AreaExpressLevelIncome a1 on a1.autoid=a.id
                                join Area ar on a.areaid=ar.areaid
                                join City c on ar.cityid=c.cityid
                                join Province p  on c.provinceid=p.ProvinceID 
                                join StatusInfo s on a1.auditstatus=s.statusno and s.statustypeno='306'
                                left join employee e on a1.auditby=e.employeeid
                                 where 1=1 ",
                sqlcount);

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ar.areaid=:areaid";
                count.Append(" and ar.areaid=:areaid");

                parameters.Add(new OracleParameter(":areaid", OracleDbType.Varchar2, 200) {Value = areaid});
                parameters1.Add(new OracleParameter(":areaid", OracleDbType.Varchar2, 200) {Value = areaid});
            }

            if (!string.IsNullOrEmpty(cityid))
            {
                strResult += " and c.cityid=:cityid";
                count.Append(" and c.cityid=:cityid");

                parameters.Add(new OracleParameter(":cityid", OracleDbType.Varchar2, 200) {Value = cityid});
                parameters1.Add(new OracleParameter(":cityid", OracleDbType.Varchar2, 200) {Value = cityid});
            }

            if (!string.IsNullOrEmpty(provinceid))
            {
                strResult += " and p.provinceid=:provinceid";
                count.Append(" and p.provinceid=:provinceid");

                parameters.Add(new OracleParameter(":provinceid", OracleDbType.Varchar2, 200) {Value = provinceid});
                parameters1.Add(new OracleParameter(":provinceid", OracleDbType.Varchar2, 200) {Value = provinceid});
            }

            int itemcount = Convert.ToInt32(GetOrderInfoCount(count.ToString(), parameters.ToArray()));
            string newSqlQuery = "";
            pi.SetItemCount(itemcount);
            int begin = pi.CurrentPageBeginItemIndex;
            int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;


            if (begin > 1)
            {
                newSqlQuery =
                    String.Format(
                        " SELECT * FROM ( SELECT  ROW_NUMBER() over(order by areaid ) as rowno,allrecord.* FROM ( " +
                        strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno>={0} AND rowno<{1} ", begin,
                        end);
            }
            else
            {
                newSqlQuery =
                    String.Format(
                        " SELECT * FROM (  SELECT  ROW_NUMBER() over(order by areaid) as rowno,allrecord.* FROM ( " +
                        strResult.ToString() + "  ) allrecord ) allrecordrowno WHERE rowno<{0} ", end);
            }

            return
                OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, newSqlQuery, parameters1.ToArray()).
                    Tables[0];
        }

        /// <summary>
        ///  查询记录总数量
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int GetOrderInfoCount(string sql, OracleParameter[] parameters)
        {
            return DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnString, CommandType.Text, sql, parameters));
        }

        //查询区域类型信息
        public DataTable SearchAreaMerchantLevelDetail(string areaid, int status, string distributionCode)
        {
            string strResult =
                @"select ael.autoid,ael.areaid,ael.dodate,a.areaname,ael.merchantid,mci.merchantName,ael.areatype,ael.effectareatype,e.employeename,case when employeename is not null then ael.updatetime else null end as updatetime ,
												CASE ael.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END,ael.warehouseid,ael.expresscompanyid
												from AreaExpressLevelIncome ael  
                                               join MerchantBaseInfo mci  on ael.merchantid=mci.id
                                               join Area a on ael.areaid=a.areaid
                                               left join employee e on ael.updateby=e.employeeid
                                               where 1=1 and ael.auditstatus=:auditstatus AND ael.DistributionCode=:DistributionCode";

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":auditstatus", OracleDbType.Decimal, 4) {Value = status});
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                               {Value = distributionCode});

            //OracleParameter[] parameters = {
            //                                new OracleParameter(":areaid", OracleDbType.Varchar2,200),
            //                                new OracleParameter(":auditstatus", OracleDbType.Decimal,4),
            //                                new OracleParameter(":DistributionCode", OracleDbType.Varchar2,100)
            //                            };

            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ael.areaid=:areaid";

                parameters.Add(new OracleParameter(":areaid", OracleDbType.Varchar2, 200) {Value = areaid});
            }

            return
                OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters.ToArray()).
                    Tables[0];
        }

        //查询区域类型详细信息
        public DataTable SearchAreaMerchantLevelDetail(string areaid, int status, int merchantId, int areatype,
                                                       int expresscompanyid, string distributionCode)
        {
            string strResult =
                string.Format(
                    @"select ael.autoid,ael.areaid,ael.dodate,a.areaname,ael.merchantid,mci.merchantName,ael.areatype,ael.effectareatype,e.employeename,case when employeename is not null then ael.updatetime else null end as updatetime,ec.companyname,
                                               (CASE ael.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END) EnableStr,ec1.CompanyName as WareHouseName
                                               from AreaExpressLevelIncome ael  
                                               join MerchantBaseInfo mci  on ael.merchantid=mci.id
                                               join Area a on ael.areaid=a.areaid
                                               left join employee e on ael.updateby=e.employeeid
                                               left join expresscompany ec on ael.expresscompanyid=ec.expresscompanyid
                                               left join ExpressCompany ec1 on ec1.expressCompanyID=ael.WareHouseID and ec1.CompanyFlag=1
                                               where 1=1 and ael.auditstatus=:auditstatus AND ael.DistributionCode=:DistributionCode ");

            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":auditstatus", OracleDbType.Decimal, 4) {Value = status});
            parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                               {Value = distributionCode});
            if (!string.IsNullOrEmpty(areaid))
            {
                strResult += " and ael.areaid=:areaid";
                parameters.Add(new OracleParameter(":areaid", OracleDbType.Varchar2, 200) {Value = areaid});
            }

            if (merchantId > 0)
            {
                strResult += " and ael.merchantid=:merchantId";
                parameters.Add(new OracleParameter(":merchantId", OracleDbType.Decimal, 4) {Value = merchantId});
            }

            if (areatype > 0)
            {
                strResult += " and ael.areatype=:areatype";
                parameters.Add(new OracleParameter(":areatype", OracleDbType.Decimal, 4) {Value = areatype});
            }

            if (expresscompanyid > 0)
            {
                strResult += " and ael.expresscompanyid=:expresscompanyid";
                parameters.Add(new OracleParameter(":expresscompanyid", OracleDbType.Decimal, 4)
                                   {Value = expresscompanyid});
            }

            return
                OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters.ToArray()).
                    Tables[0];
        }

        //设置生效
        public bool SetAreaMerchantLeverAudit(int autoid, DateTime doDate, int auditBy, DateTime audittime)
        {
            string str =
                string.Format(
                    @"update AreaExpressLevelIncome set dodate='{0}',auditstatus=1,auditby={1},audittime='{2}'
                                               where 1=1 and auditstatus=0 ",
                    doDate, auditBy, audittime);

            OracleParameter[] parameters = {
                                               new OracleParameter(":autoid", OracleDbType.Decimal, 4)
                                           };

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

        //设置生效New
        public bool SetAreaMerchantLeverAuditEx(int autoid, DateTime doDate, int auditBy, int auditstatus,
                                                DateTime audittime)
        {
            string str =
                string.Format(
                    @"update AreaExpressLevelIncome set dodate=to_date('{0}','yyyy-mm-dd hh24:mi:ss'),auditstatus=1,auditby={1},audittime=Sysdate
                                               where 1=1 and auditstatus={2} ",
                    doDate, auditBy, auditstatus);

            OracleParameter[] parameters = {
                                               new OracleParameter(":autoid", OracleDbType.Decimal, 4)
                                           };

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

        //返回待生效的区域
        public DataTable AreaMerchantLevelNum(int num, DateTime nowDate)
        {
            string strResult =
                string.Format(
                    @"select autoid,areaid,merchantid,warehouseid,areatype,Isenable,effectareatype,dodate,auditstatus,expresscompanyid from areaexpresslevelincome ael
                                              where ael.auditstatus=1 and dodate<='{1}' and rownum <= {0} ",
                    num, nowDate);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult).Tables[0];
        }

        //更新区域类型
        public bool AreaMerchantLevelUpdate(int autoid)
        {
            string str =
                string.Format(
                    @"update areaexpresslevelincome set auditstatus=2,areatype=effectareatype
                                               where auditstatus=1 and IsEnable=1 ");

            OracleParameter[] parameters = {
                                               new OracleParameter(":autoid", OracleDbType.Decimal)
                                           };

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

        //添加区域类型
        public bool AreaMerchantLevelAdd(int autoid)
        {
            string str =
                string.Format(
                    @"update areaexpresslevelincome set auditstatus=2,Isenable=1,areatype=effectareatype
                                               where auditstatus=1 and IsEnable=3 ");

            OracleParameter[] parameters = {
                                               new OracleParameter(":autoid", OracleDbType.Decimal, 4)
                                           };

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

        //删除区域类型
        public bool AreaMerchantLevelDel(int autoid)
        {
            string str =
                string.Format(
                    @"update areaexpresslevelincome set auditstatus=2,Isenable=0
                                               where auditstatus=1 and IsEnable=2 ");

            OracleParameter[] parameters = {
                                               new OracleParameter(":autoid", OracleDbType.Decimal, 4)
                                           };

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


        //设置置回
        public bool ReSetAreaMerchantLevel(int autoid)
        {
            string str =
                string.Format(
                    @"update AreaExpressLevelIncome set auditstatus=3 
                                               where 1=1 and auditstatus=0 ");

            OracleParameter[] parameters = {
                                               new OracleParameter(":autoid", OracleDbType.Decimal, 4)
                                           };

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


        public DataTable GetSortingCenter()
        {
            sqlStr =
                @"SELECT  ExpressCompanyID ,
								CompanyName
						FROM    ExpressCompany
						WHERE   CompanyFlag = 1
								AND IsDeleted = 0;";

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr).Tables[0];
        }

        public DataSet GetExportData()
        {
            DataSet ds = new DataSet();

            sqlStr = @"SELECT  ID ,
								MerchantName
						FROM    MerchantBaseInfo
						WHERE   IsDeleted = 0";
            DataSet dsMerchant = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtMerchant = dsMerchant.Tables[0].Copy();
            dtMerchant.TableName = "dtMerchant";
            ds.Tables.Add(dtMerchant);

            sqlStr =
                @"SELECT p.ProvinceID,
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

            sqlStr =
                @"
						SELECT si.StatusNO
						FROM   StatusInfo si
						WHERE  si.StatusTypeNO = '305'
							   AND si.IsDelete = 0";
            DataSet dsAreaType = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtAreaType = dsAreaType.Tables[0].Copy();
            dtAreaType.TableName = "dtAreaType";
            ds.Tables.Add(dtAreaType);

            sqlStr =
                @"SELECT  ExpressCompanyID ,
								CompanyName
						FROM    ExpressCompany
						WHERE   CompanyFlag = 1
								AND IsDeleted = 0";
            DataSet dsExpress = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtExpress = dsExpress.Tables[0].Copy();
            dtExpress.TableName = "dtExpress";
            ds.Tables.Add(dtExpress);


            sqlStr =
                @" 
SELECT  ExpressCompanyID AS ExpressID,
 CASE  
 	WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.CompanyName
    ELSE ec.AccountCompanyName
 END ExpressName
FROM    ps_pms.ExpressCompany  ec
WHERE   CompanyFlag = 3 AND IsDeleted = 0";
            DataSet dsThirdExpress = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr);
            DataTable dtThirdExpress = dsThirdExpress.Tables[0].Copy();
            dtThirdExpress.TableName = "dtThirdExpress";
            ds.Tables.Add(dtThirdExpress);
            return ds;
        }

        public DataTable SearchAreaTypeList(string provinceId, string cityId, string areaId, string expressCompanyId,
                                            string areaType, string wareHouse, string merchantId, string auditStatus,
                                            string distributionCode, PageInfo pi)
        {
            sqlStr =
                @"SELECT 						aeli.AutoID,
												aeli.ExpressCompanyID ,
                                                aeli.DistributionCode,
												ec.CompanyName ,
												p.ProvinceID ,
												p.ProvinceName ,
												c.CityID ,
												c.CityName ,
												a.AreaID ,
												a.AreaName ,
												(CASE WHEN aeli.IsEnable = 1 THEN aeli.AreaType
																ELSE NULL
														   END) AreaType ,
												( CASE WHEN aeli.IsEnable IN ( 1, 2, 3 )
																	  THEN aeli.EffectAreaType
																	  ELSE NULL
																 END ) EffectAreaType,
												aeli.MerchantID,
												mbi.MerchantName ,
												si.StatusName  AuditStatusStr ,
												( CASE aeli.IsEnable
															  WHEN 0 THEN '已删除'
															  WHEN 1 THEN '可用'
															  WHEN 2 THEN '待删除'
															  WHEN 3 THEN '新增'
															END ) EnableStr,
												aeli.WareHouseID,
												w1.CompanyName  SortCenterName
									   FROM     AreaExpressLevelIncome  aeli 
												JOIN Area  a ON aeli.AreaID = a.AreaID AND a.IsDeleted=0
												JOIN City  c ON a.CityID = c.CityID AND c.IsDeleted=0
												JOIN Province  p ON c.ProvinceID = p.ProvinceID AND p.IsDeleted=0
												JOIN ExpressCompany  ec ON ec.ExpressCompanyID = aeli.ExpressCompanyID AND ec.IsDeleted=0
												JOIN MerchantBaseInfo  mbi ON mbi.ID = aeli.MerchantID AND mbi.IsDeleted=0
												JOIN StatusInfo  si ON si.StatusNO = aeli.AuditStatus
																			   AND si.StatusTypeNO = 306 AND si.IsDelete=0
												LEFT JOIN ExpressCompany  w1 ON w1.ExpressCompanyID = aeli.WareHouseID AND w1.IsDeleted=0
												WHERE (aeli.IsEnable<>0) {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(provinceId))
            {
                sbWhere.Append(" AND p.ProvinceID=:ProvinceID ");
                parameters.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 20) {Value = provinceId});
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                sbWhere.Append(" AND c.CityID=:CityID ");
                parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 20) {Value = cityId});
            }

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=:AreaID ");
                parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) {Value = areaId});
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND aeli.ExpressCompanyID=:ExpressCompanyID ");
                parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) {Value = expressCompanyId});
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND (aeli.AreaType=:AreaType OR aeli.EffectAreaType=:AreaType )");
                parameters.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) {Value = areaType});
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aeli.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) {Value = merchantId});
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND aeli.AuditStatus=:AuditStatus ");
                parameters.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) {Value = auditStatus});
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aeli.distributionCode=:distributionCode ");
                parameters.Add(new OracleParameter(":distributionCode", OracleDbType.Varchar2, 100)
                                   {Value = distributionCode});
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            IPagedDataTable aa = PageCommon.GetPagedData(ReadOnlyConnection, sqlStr, " aeli.CreateTime DESC",
                                                         new PaginatorDTO
                                                             {PageSize = pi.PageSize, PageNo = pi.CurrentPageIndex},
                                                         this.ToParameters(parameters.ToArray()));
            pi.ItemCount = aa.RecordCount;
            pi.PageCount = aa.PageCount;
            return aa.ContentData;
        }

        public DataTable SearchAreaTypeExprotList(string provinceId, string cityId, string areaId,
                                                  string expressCompanyId, string areaType, string wareHouse,
                                                  string merchantId, string auditStatus, string distributionCode)
        {
            sqlStr =
                @"
SELECT  aeli.AutoID AS 日志编号 ,
        ec.CompanyName AS 配送公司 ,
        p.ProvinceName AS 省 ,
        c.CityName AS 市 ,
        a.AreaName AS 区 ,
        CASE WHEN aeli.IsEnable = 1 THEN aeli.AreaType
                      ELSE NULL 
                END 生效区域类型,
        CASE WHEN aeli.IsEnable IN ( 1, 2, 3 )
                        THEN aeli.EffectAreaType
                        ELSE NULL
                   END 首次新增区域类型,
        mbi.MerchantName AS 商家 ,
        si.StatusName AS 审核状态 ,
        CASE aeli.IsEnable
                 WHEN 0 THEN '已删除'
                 WHEN 1 THEN '可用'
                 WHEN 2 THEN '待删除'
                 WHEN 3 THEN '新增'
               END 生效状态,
        w1.CompanyName 分拣中心
FROM    AreaExpressLevelIncome aeli
        JOIN Area  a ON aeli.AreaID = a.AreaID
                                            AND a.IsDeleted = 0
        JOIN City  c ON a.CityID = c.CityID
                                            AND c.IsDeleted = 0
        JOIN Province  p ON c.ProvinceID = p.ProvinceID
                                                AND p.IsDeleted = 0
        JOIN ExpressCompany  ec ON ec.ExpressCompanyID = aeli.ExpressCompanyID
                                                       AND ec.IsDeleted = 0
        JOIN MerchantBaseInfo  mbi ON mbi.ID = aeli.MerchantID
                                                          AND mbi.IsDeleted = 0
        JOIN StatusInfo  si ON si.StatusNO = aeli.AuditStatus
                                                   AND si.StatusTypeNO = 306
                                                   AND si.IsDelete = 0
        LEFT JOIN ExpressCompany  w1 ON w1.ExpressCompanyID = aeli.WareHouseID
                                                            AND w1.IsDeleted = 0
WHERE   ( aeli.IsEnable <> 0 ) {0}";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(provinceId))
            {
                sbWhere.Append(" AND p.ProvinceID=:ProvinceID ");
                parameters.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 20) {Value = provinceId});
            }

            if (!string.IsNullOrEmpty(cityId))
            {
                sbWhere.Append(" AND c.CityID=:CityID ");
                parameters.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 20) {Value = cityId});
            }

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=:AreaID ");
                parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) {Value = areaId});
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND aeli.ExpressCompanyID=:ExpressCompanyID ");
                parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) {Value = expressCompanyId});
            }

            if (!string.IsNullOrEmpty(areaType))
            {
                sbWhere.Append(" AND (aeli.AreaType=:AreaType OR aeli.EffectAreaType=:AreaType )");
                parameters.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) {Value = areaType});
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aeli.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) {Value = merchantId});
            }

            if (!string.IsNullOrEmpty(auditStatus))
            {
                sbWhere.Append(" AND aeli.AuditStatus=:AuditStatus ");
                parameters.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) {Value = auditStatus});
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aeli.distributionCode=:distributionCode ");
                parameters.Add(new OracleParameter(":distributionCode", OracleDbType.Varchar2, 100)
                                   {Value = distributionCode});
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            return
                OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr,
                                            this.ToParameters(parameters.ToArray())).Tables[0];
        }

        public DataTable SearchAreaTypeLog(string areaId, string expressCompanyId, string wareHouse, string merchantId,
                                           string distributionCode)
        {
            sqlStr =
                @"
SELECT  aelil.LogID ,
        p.ProvinceName ,
        c.CityName ,
        a.AreaName ,
        ec.CompanyName ,
        w1.CompanyName  SortCenterName,
        mbi.MerchantName ,
        ( CASE aelil.CreateBy
                         WHEN 0 THEN '系统'
                         ELSE e.EmployeeName
                       END ) EmployeeName,
        aelil.CreateTime ,
        aelil.LogText
FROM  AreaExpressLevelIncomeLog  aelil
        JOIN Area  a ON aelil.AreaID = a.AreaID
                                    AND a.IsDeleted = 0
        JOIN City  c ON c.CityID = a.CityID
                                    AND c.IsDeleted = 0
        JOIN Province  p ON p.ProvinceID = c.ProvinceID
                                        AND p.IsDeleted = 0
        JOIN ExpressCompany  ec ON ec.ExpressCompanyID = aelil.ExpressCompanyID
                                               AND ec.IsDeleted = 0
        JOIN MerchantBaseInfo  mbi ON mbi.ID = aelil.MerchantID
                                                  AND mbi.IsDeleted = 0
        LEFT JOIN ExpressCompany  w1 ON w1.ExpressCompanyID = aelil.WarehouseId
                                                    AND w1.IsDeleted = 0
        LEFT JOIN employee  e ON e.EmployeeID = aelil.CreateBy
		WHERE (1=1) {0} order by aelil.CreateTime desc";
            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameters = new List<OracleParameter>();

            if (!string.IsNullOrEmpty(areaId))
            {
                sbWhere.Append(" AND a.AreaID=:AreaID ");
                parameters.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) {Value = areaId});
            }

            if (!string.IsNullOrEmpty(expressCompanyId))
            {
                sbWhere.Append(" AND aelil.ExpressCompanyID=:ExpressCompanyID ");
                parameters.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) {Value = expressCompanyId});
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                sbWhere.Append(" AND aelil.MerchantID=:MerchantID ");
                parameters.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) {Value = merchantId});
            }

            if (!string.IsNullOrEmpty(distributionCode))
            {
                sbWhere.Append(" AND aelil.DistributionCode=:DistributionCode ");
                parameters.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                                   {Value = distributionCode});
            }

            if (!string.IsNullOrEmpty(wareHouse))
            {
                sbWhere.Append(" AND aelil.WareHouseID=:WareHouseID ");
                parameters.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2, 40) {Value = wareHouse});
            }

            sqlStr = string.Format(sqlStr, sbWhere.ToString());
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sqlStr,
                                                     this.ToParameters(parameters.ToArray()));
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        #region 优化后

        public int GetAreaLevelIncomeListStat(AreaLevelIncomeSearchModel searchModel)
        {
            string sql =
                @"
SELECT count(1)
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM Area a JOIN City c
		 ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM ExpressCompany ec
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=:DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag

	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM DistributionMerchantRelation dmr
		JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
		JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=:DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN AreaExpressLevelIncome aeli
 ON aeli.IsEnable in (1,2,3)
    AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS number) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=:DistributionCode
	WHERE (1=1) {0}
";
            string sqlWhere = string.Empty;
            List<OracleParameter> parameterList = BuildCondition(searchModel, out sqlWhere);
            if (searchModel.ExpressCompanyID!=0)
            {
                sql = @"
SELECT count(1)
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM Area a JOIN City c
		 ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM ExpressCompany ec
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=:DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag
	JOIN 
	(
	    SELECT DISTINCT
        ec.TopCODCompanyID AS ExpressID,
         CASE 
                           WHEN NVL(ec2.AccountCompanyName, '') = '' THEN ec2.	CompanyName
                           ELSE ec2.AccountCompanyName
                      END ExpressName,1 AS expressFlag 
		 FROM   ExpressCompany ec
		        JOIN ExpressCompany ec2
		             ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
		 WHERE  1=1
		        AND NVL(ec.TopCODCompanyID, 0) > 0
		        AND ec.CompanyFlag=3
		        AND ec.IsDeleted = 0
		        AND ec.ExpressCompanyID=:ExpressCompanyIDs
	) expressS ON expressS.expressFlag=pca.pcaFlag
	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM DistributionMerchantRelation dmr
		JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
		JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=:DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN AreaExpressLevelIncome aeli
 ON aeli.IsEnable in (1,2,3)
    AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS number) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=:DistributionCode
	WHERE (1=1) {0}";
                parameterList.Add(new OracleParameter(":ExpressCompanyIDs", OracleDbType.Decimal) { Value = searchModel.ExpressCompanyID });
            }
            sql = string.Format(sql, sqlWhere);

            return
                DataConvert.ToInt(OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql,
                                                             parameterList.ToArray()));
        }

        private List<OracleParameter> BuildCondition(AreaLevelIncomeSearchModel searchModel, out string sqlWhere)
        {
            List<OracleParameter> parameterList = new List<OracleParameter>();
            StringBuilder sbWhere = new StringBuilder();
            parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                                  {Value = searchModel.DistributionCode});
            if (!string.IsNullOrEmpty(searchModel.ProvinceID))
            {
                sbWhere.Append(" AND pca.ProvinceID=:ProvinceID ");
                parameterList.Add(new OracleParameter(":ProvinceID", OracleDbType.Varchar2, 20)
                                      {Value = searchModel.ProvinceID});
            }

            if (!string.IsNullOrEmpty(searchModel.CityID))
            {
                sbWhere.Append(" AND pca.CityID=:CityID ");
                parameterList.Add(new OracleParameter(":CityID", OracleDbType.Varchar2, 20) {Value = searchModel.CityID});
            }

            if (!string.IsNullOrEmpty(searchModel.AreaID))
            {
                sbWhere.Append(" AND pca.AreaID=:AreaID ");
                parameterList.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100)
                                      {Value = searchModel.AreaID});
            }

            if (searchModel.MerchantID > 0)
            {
                sbWhere.Append(" AND merchant.MerchantID=:MerchantID ");
                parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal)
                                      {Value = searchModel.MerchantID});
            }

            if (!string.IsNullOrEmpty(searchModel.WareHouse))
            {
                sbWhere.Append(" AND express.ExpressCompanyID in (" + searchModel.WareHouse + ") ");
            }

            if (searchModel.AuditStatus == -1)
            {
                sbWhere.Append(" AND aeli.AuditStatus is null ");
            }

            if (searchModel.AuditStatus > -1)
            {
                sbWhere.Append(" AND aeli.AuditStatus=:AuditStatus ");
                parameterList.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal)
                                      {Value = searchModel.AuditStatus});
            }

            if (searchModel.AreaType > 0)
            {
                sbWhere.Append(" AND (aeli.AreaType=:AreaType OR aeli.EffectAreaType=:AreaType )");
                parameterList.Add(new OracleParameter(":AreaType", OracleDbType.Decimal) {Value = searchModel.AreaType});
            }

            if (!string.IsNullOrEmpty(searchModel.GoodsCategoryCode))
            {
                sbWhere.Append(" AND aeli.GoodsCategoryCode in (" + searchModel.GoodsCategoryCode + ")");
            }
            if (searchModel.ExpressCompanyID>0)
            {
                sbWhere.Append(" 	AND (expressS.ExpressID =:ExpressCompanyIDa AND  aeli.ExpressCompanyID=:ExpressCompanyIDa )");
                parameterList.Add(new OracleParameter(":ExpressCompanyIDa", OracleDbType.Decimal) { Value = searchModel.ExpressCompanyID }); 
            }

            sqlWhere = sbWhere.ToString();
            return parameterList;
        }

        public DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel, PageInfo pi)
        {
            #region sql

            string sql =
                @"
WITH t AS 
(
SELECT ROWNUM as SerialNo,
pca.ProvinceID,
pca.ProvinceName,
pca.CityID,
pca.CityName,
pca.AreaID,
pca.AreaName,
express.ExpressCompanyID,
express.CompanyName,
case  when aeli.AutoId IS NULL then 11 ELSE aeli.ExpressCompanyID END ExpressID,
case  when aeli.AutoId IS NULL then '全部' ELSE aeli.companyname  end ExpressName,
merchant.MerchantID,
merchant.MerchantName,
aeli.goodscategorycode,
case  when aeli.AutoId IS NULL then '' when gc.goodscategoryname is null then '否' else gc.goodscategoryname end goodscategoryname,
aeli.AutoId,
aeli.AreaType,
CASE WHEN aeli.AreaType=0 then '' else cast(aeli.AreaType as varchar2(20)) end AreaTypeStr,
aeli.EffectAreaType,
aeli.IsEnable,
CASE aeli.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END EnableStr,
CASE WHEN aeli.AutoId IS NULL THEN -1 ELSE aeli.AuditStatus END AuditStatus,
CASE WHEN aeli.AutoId IS NULL THEN '待维护' ELSE case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已生效' when 3 then '置回' else NULL END END AuditStatusStr,
aeli.DoDate
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM Area a JOIN City c
		 ON a.CityID=c.CityID JOIN Province p ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM ExpressCompany ec
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=:DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag
	
	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM DistributionMerchantRelation dmr
		JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
		JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=:DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN 
	( select ae.AUTOID,AREAID,MERCHANTID,WAREHOUSEID,AREATYPE,ISENABLE,EFFECTAREATYPE,DODATE,CREATEBY,CREATETIME,UPDATEBY,
           UPDATETIME,AUDITSTATUS,AUDITBY,AUDITTIME,ae.EXPRESSCOMPANYID,DISTRIBUTIONCODE,ISCHANGE,GOODSCATEGORYCODE,
           ISEXPRESS,express.companyname from 
	  AreaExpressLevelIncome ae 
   join  
	(
		SELECT ec.ExpressCompanyID,
        CASE WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.CompanyName
              ELSE ec.AccountCompanyName
        END CompanyName,1 AS expressFlag 
        FROM ExpressCompany ec
		WHERE  ec.IsDeleted=0 
	) express on  express.ExpressCompanyID=ae.ExpressCompanyID 
  ) aeli
 ON aeli.IsEnable in (1,2,3) AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS number) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=:DistributionCode
    LEFT JOIN goodscategory gc on gc.goodscategorycode=aeli.goodscategorycode
	WHERE (1=1) {0}
	)
	SELECT SerialNo,
ProvinceID,
ProvinceName,
CityID,
CityName,
AreaID,
AreaName,
ExpressCompanyID,
CompanyName,
ExpressID,
CASE ExpressID  WHEN 11 THEN '全部' ELSE ExpressName END  ExpressName,
MerchantID,
MerchantName,
goodscategorycode,
goodscategoryname,
AutoId,
AreaType,
AreaTypeStr,
EffectAreaType,
IsEnable,
EnableStr,
AuditStatus,
AuditStatusStr,
DoDate FROM t WHERE SerialNo BETWEEN :rowStart AND :rowEnd
";

            #endregion

            string sqlWhere = string.Empty;
            List<OracleParameter> parameterList = BuildCondition(searchModel, out sqlWhere);
            parameterList.Add(new OracleParameter(":rowStart", OracleDbType.Decimal) {Value = pi.CurrentPageStartRowNum});
            parameterList.Add(new OracleParameter(":rowEnd", OracleDbType.Decimal) {Value = pi.CurrentPageEndRowNum});
            if ( searchModel.ExpressCompanyID != 0)
            {
                sql =
                    @"
WITH t AS 
(
SELECT ROWNUM as SerialNo,
pca.ProvinceID,
pca.ProvinceName,
pca.CityID,
pca.CityName,
pca.AreaID,
pca.AreaName,
express.ExpressCompanyID,
express.CompanyName,
case  when aeli.AutoId IS NULL then expressS.ExpressID ELSE aeli.ExpressCompanyID END ExpressID,
case  when aeli.AutoId IS NULL then expressS.ExpressName ELSE aeli.companyname  end ExpressName,
merchant.MerchantID,
merchant.MerchantName,
aeli.goodscategorycode,
case  when aeli.AutoId IS NULL then '' when gc.goodscategoryname is null then '否' else gc.goodscategoryname end goodscategoryname,
aeli.AutoId,
aeli.AreaType,
CASE WHEN aeli.AreaType=0 then '' else cast(aeli.AreaType as varchar2(20)) end AreaTypeStr,
aeli.EffectAreaType,
aeli.IsEnable,
CASE aeli.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END EnableStr,
CASE WHEN aeli.AutoId IS NULL THEN -1 ELSE aeli.AuditStatus END AuditStatus,
CASE WHEN aeli.AutoId IS NULL THEN '待维护' ELSE case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已生效' when 3 then '置回' else NULL END END AuditStatusStr,
aeli.DoDate
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM Area a JOIN City c
		 ON a.CityID=c.CityID JOIN Province p ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM ExpressCompany ec
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=:DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag
	JOIN 
	(
       SELECT ExpressCompanyID
         AS ExpressID,
         CASE 
           WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.	CompanyName
           ELSE ec.AccountCompanyName
           END ExpressName,1 AS expressFlag 
		 FROM   ExpressCompany ec
       WHERE  1=1 AND NVL(ec.TopCODCompanyID, 0) > 0 AND ec.CompanyFlag=3  AND ec.IsDeleted = 0
		     AND ec.ExpressCompanyID=:ExpressCompanyIDs
	) expressS ON expressS.expressFlag=pca.pcaFlag
	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM DistributionMerchantRelation dmr
		JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
		JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=:DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN 
	( select ae.AUTOID,AREAID,MERCHANTID,WAREHOUSEID,AREATYPE,ISENABLE,EFFECTAREATYPE,DODATE,CREATEBY,CREATETIME,UPDATEBY,
           UPDATETIME,AUDITSTATUS,AUDITBY,AUDITTIME,ae.EXPRESSCOMPANYID,DISTRIBUTIONCODE,ISCHANGE,GOODSCATEGORYCODE,
           ISEXPRESS,express.companyname from 
	  AreaExpressLevelIncome ae 
   join  
	(
		SELECT ec.ExpressCompanyID,
        CASE WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.CompanyName
              ELSE ec.AccountCompanyName
        END CompanyName,1 AS expressFlag 
        FROM ExpressCompany ec
		WHERE  ec.IsDeleted=0  
	) express on  express.ExpressCompanyID=ae.ExpressCompanyID 
  ) aeli
 ON aeli.IsEnable in (1,2,3) AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS number) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=:DistributionCode
    LEFT JOIN goodscategory gc on gc.goodscategorycode=aeli.goodscategorycode
	WHERE (1=1) {0}
	)
	SELECT SerialNo,
ProvinceID,
ProvinceName,
CityID,
CityName,
AreaID,
AreaName,
ExpressCompanyID,
CompanyName,
ExpressID,
CASE ExpressID  WHEN 11 THEN '全部' ELSE ExpressName END  ExpressName,
MerchantID,
MerchantName,
goodscategorycode,
goodscategoryname,
AutoId,
AreaType,
AreaTypeStr,
EffectAreaType,
IsEnable,
EnableStr,
AuditStatus,
AuditStatusStr,
DoDate FROM t WHERE SerialNo BETWEEN :rowStart AND :rowEnd";
                parameterList.Add(new OracleParameter(":ExpressCompanyIDs", OracleDbType.Decimal)
                                      {Value = searchModel.ExpressCompanyID});
            }

            sql = string.Format(sql, sqlWhere);

            return
                OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray()).Tables[0
                    ];
        }

        public DataTable GetAreaLevelIncomeExprotList(AreaLevelIncomeSearchModel searchModel)
        {
            string sql =
                @"
SELECT 序号,
省份,
城市,
地区,
分拣中心,
商家,
case  ExpressID WHEN 11 THEN '全部' ELSE ExpressName END 配送商,
货物品类,
生效区域类型,
待生效区域类型,
生效状态,
审批状态,
生效时间

FROM (
SELECT ROWNUM AS 序号,
pca.ProvinceName 省份,
pca.CityName 城市,
pca.AreaName 地区,
express.CompanyName 分拣中心,
merchant.MerchantName 商家,
case  when aeli.AutoId IS NULL then 11 ELSE aeli.ExpressCompanyID END ExpressID,
case  when aeli.AutoId IS NULL then '全部' ELSE  aeli.companyname  end ExpressName,
case  when aeli.AutoId IS NULL then '' when gc.goodscategoryname is null then '否' else gc.goodscategoryname end 货物品类,
CASE WHEN aeli.AreaType=0 then '' else cast(aeli.AreaType as varchar2(20)) end 生效区域类型,
aeli.EffectAreaType 待生效区域类型,
CASE aeli.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END 生效状态,
CASE WHEN aeli.AutoId IS NULL THEN '待维护' ELSE case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已生效' when 3 then '置回' else NULL END END 审批状态,
aeli.DoDate 生效时间
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM Area a JOIN City c
		 ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM ExpressCompany ec
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=:DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag

	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM DistributionMerchantRelation dmr
		JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
		JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=:DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN 
( select ae.AUTOID,AREAID,MERCHANTID,WAREHOUSEID,AREATYPE,ISENABLE,EFFECTAREATYPE,DODATE,CREATEBY,CREATETIME,UPDATEBY,
           UPDATETIME,AUDITSTATUS,AUDITBY,AUDITTIME,ae.EXPRESSCOMPANYID,DISTRIBUTIONCODE,ISCHANGE,GOODSCATEGORYCODE,
           ISEXPRESS,express.companyname from 
	       AreaExpressLevelIncome ae 
   join  
	(
		SELECT ec.ExpressCompanyID,
        CASE WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.CompanyName
              ELSE ec.AccountCompanyName
        END CompanyName,1 AS expressFlag 
        FROM ExpressCompany ec
		WHERE  ec.IsDeleted=0  
	) express on  express.ExpressCompanyID=ae.ExpressCompanyID 
  )
aeli
 ON aeli.IsEnable in (1,2,3) AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS number) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=:DistributionCode
    LEFT JOIN goodscategory gc on gc.goodscategorycode=aeli.goodscategorycode
	WHERE (1=1) {0}
	)
";
            string sqlWhere = string.Empty;
            List<OracleParameter> parameterList = BuildCondition(searchModel, out sqlWhere);
            if (searchModel.IsExpress == 1 && searchModel.ExpressCompanyID != 0)
            {
                sql = @"

SELECT 序号,
省份,
城市,
地区,
分拣中心,
商家,
case  ExpressID WHEN 11 THEN '全部' ELSE ExpressName END 配送商,
货物品类,
生效区域类型,
待生效区域类型,
生效状态,
审批状态,
生效时间

FROM (
SELECT ROWNUM AS 序号,
pca.ProvinceName 省份,
pca.CityName 城市,
pca.AreaName 地区,
express.CompanyName 分拣中心,
merchant.MerchantName 商家,
case  when aeli.AutoId IS NULL then expressS.ExpressID ELSE aeli.ExpressCompanyID END ExpressID,
case  when aeli.AutoId IS NULL then expressS.ExpressName ELSE  aeli.companyname  end ExpressName,
case  when aeli.AutoId IS NULL then '' when gc.goodscategoryname is null then '否' else gc.goodscategoryname end 货物品类,
CASE WHEN aeli.AreaType=0 then '' else cast(aeli.AreaType as varchar2(20)) end 生效区域类型,
aeli.EffectAreaType 待生效区域类型,
CASE aeli.IsEnable WHEN 0 THEN '已删除' WHEN 1 THEN '可用' WHEN 2 THEN '待删除' WHEN 3 THEN '新增' END 生效状态,
CASE WHEN aeli.AutoId IS NULL THEN '待维护' ELSE case aeli.AuditStatus when 0 then '未审核' when 1 then '已审核' when 2 then '已生效' when 3 then '置回' else NULL END END 审批状态,
aeli.DoDate 生效时间
 FROM 
	(	
		SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a.AreaID,a.AreaName,1 AS pcaFlag
		 FROM Area a JOIN City c
		 ON a.CityID=c.CityID JOIN Province p  ON c.ProvinceID = p.ProvinceID
		WHERE  c.IsDeleted=0 AND p.IsDeleted=0 AND a.IsDeleted=0
	) pca
	JOIN 
	(
		SELECT ec.ExpressCompanyID,ec.CompanyName,1 AS expressFlag FROM ExpressCompany ec
		WHERE ec.CompanyFlag=1 AND ec.DistributionCode=:DistributionCode AND ec.IsDeleted=0 AND ec.ParentID<>11
	) express ON express.expressFlag=pca.pcaFlag
	JOIN 
	(
	  SELECT DISTINCT
        ec.TopCODCompanyID AS ExpressID,
         CASE 
                           WHEN NVL(ec2.AccountCompanyName, '') = '' THEN ec2.CompanyName
                           ELSE ec2.AccountCompanyName
                      END ExpressName,1 AS expressFlag 
		 FROM   ExpressCompany ec
		        JOIN ExpressCompany ec2
		             ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
		 WHERE  1=1
		        AND NVL(ec.TopCODCompanyID, 0) > 0
		        AND ec.CompanyFlag=3
		        AND ec.IsDeleted = 0
		        AND ec.ExpressCompanyID=:ExpressCompanyID
	) expressS ON expressS.expressFlag=pca.pcaFlag
	JOIN 
	(
		SELECT m.ID AS MerchantID,m.MerchantName,1 AS merchantFlag
		 FROM DistributionMerchantRelation dmr
		JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
		JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
		WHERE dmr.IsDeleted=0 AND m.ISDELETED=0 AND d.ISDELETE=0
		AND dmr.DistributionCode=:DistributionCode
	) merchant ON merchant.merchantFlag=pca.pcaFlag
	LEFT JOIN 
( select ae.AUTOID,AREAID,MERCHANTID,WAREHOUSEID,AREATYPE,ISENABLE,EFFECTAREATYPE,DODATE,CREATEBY,CREATETIME,UPDATEBY,
           UPDATETIME,AUDITSTATUS,AUDITBY,AUDITTIME,ae.EXPRESSCOMPANYID,DISTRIBUTIONCODE,ISCHANGE,GOODSCATEGORYCODE,
           ISEXPRESS,express.companyname from 
	       AreaExpressLevelIncome ae 
   join  
	(
		SELECT ec.ExpressCompanyID,
        CASE WHEN NVL(ec.AccountCompanyName, '') = '' THEN ec.CompanyName
              ELSE ec.AccountCompanyName
        END CompanyName,1 AS expressFlag 
        FROM ExpressCompany ec
		WHERE  ec.IsDeleted=0  
	) express on  express.ExpressCompanyID=ae.ExpressCompanyID 
  )
aeli
 ON aeli.IsEnable in (1,2,3) AND pca.AreaID=aeli.AreaID 
	AND express.ExpressCompanyID=CAST(aeli.WareHouseID AS number) 
	AND merchant.MerchantID=aeli.MerchantID
	AND aeli.DistributionCode=:DistributionCode
    LEFT JOIN goodscategory gc on gc.goodscategorycode=aeli.goodscategorycode
	WHERE (1=1) {0}
	)";
                parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal) { Value = searchModel.ExpressCompanyID });
            }
            sql = string.Format(sql, sqlWhere);
            return
                OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql,
                                            ToParameters(parameterList.ToArray())).Tables[0];
        }

        public bool UpdateAreaLevelIncomeStatus(AreaExpressLevelIncome model)
        {
            string sql =
                @"UPDATE AreaExpressLevelIncome SET AuditStatus=:AuditStatus,AuditBy=:AuditBy,AuditTime=sysdate,IsChange=1{0} WHERE AutoId=:AutoId ";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) {Value = model.AuditStatus});
            parameterList.Add(new OracleParameter(":AuditBy", OracleDbType.Decimal) {Value = model.AuditBy});
            parameterList.Add(new OracleParameter(":AutoId", OracleDbType.Decimal) {Value = model.AutoId});
            if (!string.IsNullOrEmpty(model.DoDate.ToString()))
            {
                sql = string.Format(sql, ",DoDate=:DoDate");
                parameterList.Add(new OracleParameter(":DoDate", OracleDbType.Date) {Value = model.DoDate});
            }
            else
            {
                sql = string.Format(sql, "");
            }
            int rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql,
                                                        ToParameters(parameterList.ToArray()));
            return rowCount > 0;
        }

        public DataTable GetWaitEffectList()
        {
            string strResult =
                @"select autoid,areaid,merchantid,warehouseid,areatype,isenable,effectareatype,dodate,auditstatus,expresscompanyid 
                                        from areaexpresslevelincome ael
                                              where ael.auditstatus=1 and dodate<=to_date(:dodate,'yyyy-mm-dd') ";
            OracleParameter[] parameters = {
                                               new OracleParameter(":dodate", OracleDbType.Varchar2),
                                           };
            parameters[0].Value = DateTime.Now.ToString("yyyy-MM-dd");
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, strResult, parameters).Tables[0];
        }

        /// <summary>
        /// 根据区、分拣、品类、商家获取区域类型
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public int GetAreaTypeByCondition(AreaLevelIncomeSearchModel searchModel)
        {
            string sql = @"SELECT AreaType FROM AreaExpressLevelIncome ael1 WHERE ael1.IsEnable IN (1, 2) {0}";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(searchModel.GoodsCategoryCode))
            {
                sqlWhere.Append(" AND ael1.GoodsCategoryCode =:GoodsCategoryCode");
                parameterList.Add(new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2, 20)
                                      {Value = searchModel.GoodsCategoryCode});
            }
            if (searchModel.MerchantID > 0)
            {
                sqlWhere.Append(" AND ael1.MerchantID =:MerchantID");
                parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal)
                                      {Value = searchModel.MerchantID});
            }
            if (!string.IsNullOrEmpty(searchModel.WareHouse))
            {
                sqlWhere.Append(" AND ael1.WareHouseID =:WareHouseID");
                parameterList.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2, 40)
                                      {Value = searchModel.WareHouse});
            }
            if (!string.IsNullOrEmpty(searchModel.AreaID))
            {
                sqlWhere.Append(" AND ael1.AreaID =:AreaID");
                parameterList.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100)
                                      {Value = searchModel.AreaID});
            }
            if (!string.IsNullOrEmpty(searchModel.DistributionCode))
            {
                sqlWhere.Append(" AND ael1.DistributionCode =:DistributionCode");
                parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100)
                                      {Value = searchModel.DistributionCode});
            }
            //if (searchModel.ExpressCompanyID>0)
            //{
            //    sqlWhere.Append(" AND ael1.ExpressCompanyID =:ExpressCompanyID");
            //    parameterList.Add(new OracleParameter(":ExpressCompanyID", OracleDbType.Varchar2, 100) { Value = searchModel.DistributionCode });
            //}
            sql = string.Format(sql, sqlWhere.ToString());
            object n = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray());
            return DataConvert.ToInt(n, 0);
        }

        #endregion


        public DataTable GetAreaLevelIncomeList(AreaLevelIncomeSearchModel searchModel)
        {
            string sql = @"SELECT * FROM AreaExpressLevelIncome ael1 WHERE ael1.IsEnable IN (1, 2) {0}";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(searchModel.GoodsCategoryCode))
            {
                sqlWhere.Append(" AND ael1.GoodsCategoryCode =:GoodsCategoryCode");
                parameterList.Add(new OracleParameter(":GoodsCategoryCode", OracleDbType.Varchar2, 20) { Value = searchModel.GoodsCategoryCode });
            }
            if (searchModel.MerchantID > 0)
            {
                sqlWhere.Append(" AND ael1.MerchantID =:MerchantID");
                parameterList.Add(new OracleParameter(":MerchantID", OracleDbType.Decimal) { Value = searchModel.MerchantID });
            }
            if (!string.IsNullOrEmpty(searchModel.WareHouse))
            {
                sqlWhere.Append(" AND ael1.WareHouseID =:WareHouseID");
                parameterList.Add(new OracleParameter(":WareHouseID", OracleDbType.Varchar2, 40) { Value = searchModel.WareHouse });
            }
            if (!string.IsNullOrEmpty(searchModel.AreaID))
            {
                sqlWhere.Append(" AND ael1.AreaID =:AreaID");
                parameterList.Add(new OracleParameter(":AreaID", OracleDbType.Varchar2, 100) { Value = searchModel.AreaID });
            }
            if (!string.IsNullOrEmpty(searchModel.DistributionCode))
            {
                sqlWhere.Append(" AND ael1.DistributionCode =:DistributionCode");
                parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = searchModel.DistributionCode });
            }
            if (searchModel.AuditStatus>0)
            {
                sqlWhere.Append(" AND ael1.AuditStatus =:AuditStatus");
                parameterList.Add(new OracleParameter(":AuditStatus", OracleDbType.Decimal) { Value = searchModel.AuditStatus }); 
            }
            sql = string.Format(sql, sqlWhere.ToString());

            DataSet dataSet = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray());

            if (dataSet!=null&&dataSet.Tables.Count>0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }
    }
}