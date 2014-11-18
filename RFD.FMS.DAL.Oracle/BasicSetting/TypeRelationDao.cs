using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using System.Data.SqlClient;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Util;
using RFD.FMS.MODEL;

namespace RFD.FMS.DAL.Oracle.BasicSetting
{
    public class TypeRelationDao : OracleDao, ITypeRelationDao
    {
        public DataTable SearchMerchantRelation(string typeSource, string distributionCode)
        {
            string sql = @"
WITH t AS (
SELECT m.ID,m.MerchantName
	FROM DistributionMerchantRelation dmr
	JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
	JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
	WHERE dmr.IsDeleted=0 AND m.IsDeleted=0 AND d.IsDelete=0
	AND dmr.DistributionCode=:DistributionCode 
)
SELECT ftr.TypeRelationKid,:CodeType  TypeRelationName,ftr.DistributionCode,ftr.TypeCodeNo,sci.CodeDesc,t.ID AS RelationID,ftr.RelationNameID,t.MerchantName AS RelationName,ftr.IsDelete,ftr.CreateBy,ftr.UpdateBy
 FROM t
LEFT JOIN FMS_TypeRelation ftr ON t.ID=ftr.RelationNameID AND TypeRelationName=:CodeType AND ftr.DistributionCode=:DistributionCode AND ftr.IsDelete=0
LEFT JOIN StatusCodeInfo sci ON sci.CodeNo=ftr.TypeCodeNo AND sci.CodeType=:CodeType
";
            OracleParameter[] parameters ={
                                           new OracleParameter(":CodeType",OracleDbType.Varchar2,20),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                      };
            parameters[0].Value = typeSource;
            parameters[1].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable SearchExpressRelation(string typeSource, string distributionCode)
        {
            string sql = @"
WITH t AS (
SELECT DISTINCT
        ec.TopCODCompanyID AS ExpressCompanyID,
        (CASE 
                           WHEN NVL(ec2.AccountCompanyName, '') = '' THEN ec2.CompanyName
                           ELSE ec2.AccountCompanyName
                      END) CompanyName
 FROM   ExpressCompany  ec
        JOIN ExpressCompany  ec2
             ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
 WHERE  ec.DistributionCode <> :DistributionCode
        AND NVL(ec.TopCODCompanyID, 0) > 0
        AND ec.CompanyFlag=3
        AND ec.IsDeleted = 0 
)
SELECT ftr.TypeRelationKid,:CodeType AS TypeRelationName,ftr.DistributionCode,ftr.TypeCodeNo,sci.CodeDesc,t.ExpressCompanyID AS RelationID,ftr.RelationNameID,t.CompanyName AS RelationName,ftr.IsDelete,ftr.CreateBy,ftr.UpdateBy
 FROM t
LEFT JOIN FMS_TypeRelation ftr ON t.ExpressCompanyID=ftr.RelationNameID AND TypeRelationName=:CodeType AND ftr.DistributionCode=:DistributionCode AND ftr.IsDelete=0
LEFT JOIN StatusCodeInfo sci ON sci.CodeNo=ftr.TypeCodeNo AND sci.CodeType=:CodeType
";
            OracleParameter[] parameters ={
                                           new OracleParameter(":CodeType",OracleDbType.Varchar2,20),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                      };
            parameters[0].Value = typeSource;
            parameters[1].Value = distributionCode;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public bool Exists(TypeRelationModel model)
        {
            string sql = @"
    SELECT count(1)
  FROM FMS_TypeRelation
  WHERE TypeRelationName=:TypeRelationName 
AND RelationNameID=:RelationNameID 
AND DistributionCode=:DistributionCode 
AND TypeCodeNo=:TypeCodeNo
AND IsDelete=0
";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":TypeRelationName", OracleDbType.Varchar2, 100) { Value = model.TypeRelationName });
            parameterList.Add(new OracleParameter(":RelationNameID", OracleDbType.Decimal) { Value = model.RelationNameID });
            parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = model.DistributionCode });
            parameterList.Add(new OracleParameter(":TypeCodeNo", OracleDbType.Varchar2, 20) { Value = model.TypeCodeNo });
            object n = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterList.ToArray()));
            return DataConvert.ToInt(n)>0;
        }

        public TypeRelationModel GetModel(TypeRelationModel model)
        {
            string sql = @"
    SELECT TypeRelationKid
      ,TypeRelationName
      ,DistributionCode
      ,TypeCodeNo
      ,RelationNameID
      ,IsDelete
      ,CreateBy
      ,CreateTime
      ,UpdateBy
      ,UpdateTime
  FROM FMS_TypeRelation
  WHERE TypeRelationName=:TypeRelationName 
AND RelationNameID=:RelationNameID 
AND DistributionCode=:DistributionCode 
AND IsDelete=0
";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":TypeRelationName", OracleDbType.Varchar2,100) { Value = model.TypeRelationName });
            parameterList.Add(new OracleParameter(":RelationNameID", OracleDbType.Decimal) { Value = model.RelationNameID });
            parameterList.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = model.DistributionCode });
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterList.ToArray()));
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0 || ds.Tables[0].Rows.Count>1)
                return null;

            return TransforToTypeRelationModel(ds.Tables[0].Rows[0]);
        }

        public TypeRelationModel TransforToTypeRelationModel(DataRow dr)
        {
            TypeRelationModel model = new TypeRelationModel();
            model.CreateBy = dr["CreateBy"] == DBNull.Value ? 0 : int.Parse(dr["CreateBy"].ToString());
            model.UpdateBy = dr["UpdateBy"] == DBNull.Value ? 0 : int.Parse(dr["UpdateBy"].ToString());
			model.IsDelete = dr["IsDelete"].ToString() == "1" ? true : false;
            //model.IsDelete = dr["IsDelete"] == DBNull.Value ? true : bool.Parse(dr["IsDelete"].ToString());
            model.RelationNameID = dr["RelationNameID"] == DBNull.Value ? 0 : int.Parse(dr["RelationNameID"].ToString());
            model.TypeCodeNo = dr["TypeCodeNo"] == DBNull.Value ? "" : dr["TypeCodeNo"].ToString();
            model.DistributionCode = dr["DistributionCode"] == DBNull.Value ? "" : dr["DistributionCode"].ToString();
            model.TypeRelationName = dr["TypeRelationName"] == DBNull.Value ? "" : dr["TypeRelationName"].ToString();
            model.TypeRelationKid = dr["TypeRelationKid"] == DBNull.Value ? "" : dr["TypeRelationKid"].ToString();
            model.CreateTime = dr["CreateTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr["CreateTime"].ToString());
            model.UpdateTime = dr["CreateTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr["CreateTime"].ToString());
            return model;
        }

        public bool Add(TypeRelationModel model)
        {
            string sql = @"
	INSERT INTO FMS_TypeRelation
        ( TypeRelationKid ,
          TypeRelationName ,
          DistributionCode ,
          TypeCodeNo ,
          RelationNameID ,
          IsDelete ,
          CreateBy ,
          CreateTime ,
          UpdateBy ,
          UpdateTime
        )
VALUES  ( :TypeRelationKid ,
          :TypeRelationName ,
          :DistributionCode ,
          :TypeCodeNo ,
          :RelationNameID ,
          :IsDelete ,
          :CreateBy ,
          SysDate ,
          :UpdateBy ,
          SysDate
        )    
";
            OracleParameter[] parameters ={
                                          new OracleParameter(":TypeRelationKid",OracleDbType.Varchar2,20),
                                          new OracleParameter(":TypeRelationName",OracleDbType.Varchar2,100),
                                          new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                          new OracleParameter(":TypeCodeNo",OracleDbType.Varchar2,20),
                                          new OracleParameter(":RelationNameID",OracleDbType.Decimal),
                                          new OracleParameter(":IsDelete",OracleDbType.Decimal),
                                          new OracleParameter(":CreateBy",OracleDbType.Decimal),
                                          new OracleParameter(":UpdateBy",OracleDbType.Decimal),
                                      };
            parameters[0].Value = model.TypeRelationKid;
            parameters[1].Value = model.TypeRelationName;
            parameters[2].Value = model.DistributionCode;
            parameters[3].Value = model.TypeCodeNo;
            parameters[4].Value = model.RelationNameID;
            parameters[5].Value = model.IsDelete ? 1 : 0;
            parameters[6].Value = model.CreateBy;
            parameters[7].Value = model.UpdateBy;
            int n= OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) ;
            return n > 0;
        }

        public bool Update(TypeRelationModel model)
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.Append("UPDATE FMS_TypeRelation SET ");
            sbSql.Append(" TypeCodeNo=:TypeCodeNo, ");
            sbSql.Append(" UpdateBy=:UpdateBy, ");
            sbSql.Append(" UpdateTime=SysDate ");
            sbSql.Append(" where TypeRelationKid=:TypeRelationKid ");
            OracleParameter[] parameters ={
                                          new OracleParameter(":TypeRelationKid",OracleDbType.Varchar2,20),
                                          new OracleParameter(":TypeCodeNo",OracleDbType.Varchar2,20),
                                          new OracleParameter(":UpdateBy",OracleDbType.Decimal),
                                      };
            parameters[0].Value = model.TypeRelationKid;
            parameters[1].Value = model.TypeCodeNo;
            parameters[2].Value = model.UpdateBy;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sbSql.ToString(), parameters) > 0;
        }

        public bool Delete(TypeRelationModel model)
        {
            string sql = @"UPDATE FMS_TypeRelation SET IsDelete=1,UpdateBy=:UpdateBy,UpdateTime=sysdate,IsChange=1 WHERE TypeRelationKid=:TypeRelationKid ";
            List<OracleParameter> parameterList = new List<OracleParameter>();
            parameterList.Add(new OracleParameter(":UpdateBy", OracleDbType.Decimal) { Value = model.UpdateBy });
            parameterList.Add(new OracleParameter(":TypeRelationKid", OracleDbType.Varchar2, 20) { Value = model.TypeRelationKid });
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql.ToString(), ToParameters(parameterList.ToArray())) > 0;
        }

        public DataTable SearchPeriodMerchantRelation(string typeSource, string distributionCode, string relationName, ref PageInfo pi)
        {
            string statSql = @"
WITH t AS (
SELECT m.ID,m.MerchantName
	FROM DistributionMerchantRelation dmr
	JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
	JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
	WHERE dmr.IsDeleted=0 AND m.IsDeleted=0 AND d.IsDelete=0
	AND dmr.DistributionCode=:DistributionCode 
{0}
)
SELECT count(1)
 FROM t
LEFT JOIN FMS_TypeRelation ftr ON t.ID=ftr.RelationNameID AND TypeRelationName=:CodeType AND ftr.DistributionCode=:DistributionCode AND ftr.IsDelete=0
LEFT JOIN FMS_AccountPeriod fap ON fap.AccountPeriodKid=ftr.TypeCodeNo AND fap.PeriodRelationName=:CodeType
order by t.ID";

            List<OracleParameter> parameterListTmp = new List<OracleParameter>();
            parameterListTmp.Add(new OracleParameter(":CodeType",OracleDbType.Varchar2,20){Value=typeSource});
            parameterListTmp.Add(new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100){Value=distributionCode});

            StringBuilder sbWhere = new StringBuilder();
            List<OracleParameter> parameterList = new List<OracleParameter>();
            if (!string.IsNullOrWhiteSpace(relationName))
            {
                sbWhere.Append(" AND m.MerchantName like :MerchantName ");
                parameterList.Add(new OracleParameter(":MerchantName", "%" + relationName + "%"));
            }

            statSql = string.Format(statSql, sbWhere);
            parameterListTmp.AddRange(parameterList);

            object n = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, statSql, ToParameters(parameterListTmp.ToArray()));
            pi.ItemCount = DataConvert.ToInt(n, 0);
            pi.PageCount = Convert.ToInt32(Math.Ceiling(pi.ItemCount * 1.0 / pi.PageSize));
            if (pi.ItemCount <= 0)
                return null;

            string sql = @"
with t as(
SELECT rownum rnum,ftr.TypeRelationKid,:CodeType  TypeRelationName,ftr.DistributionCode,ftr.TypeCodeNo,fap.Periodname CodeDesc,m.ID AS RelationID,ftr.RelationNameID,m.MerchantName AS RelationName,ftr.IsDelete,ftr.CreateBy,ftr.UpdateBy
  FROM DistributionMerchantRelation dmr
  JOIN MERCHANTBASEINFO m ON dmr.MerchantId=m.ID 
  JOIN DISTRIBUTION d ON d.DistributionCode = dmr.DistributionCode
LEFT JOIN FMS_TypeRelation ftr ON m.ID=ftr.RelationNameID AND TypeRelationName=:CodeType AND ftr.DistributionCode=:DistributionCode AND ftr.IsDelete=0
LEFT JOIN FMS_AccountPeriod fap ON fap.AccountPeriodKid=ftr.TypeCodeNo AND fap.PeriodRelationName=:CodeType
where
dmr.IsDeleted=0 AND m.IsDeleted=0 AND d.IsDelete=0
  AND dmr.DistributionCode=:DistributionCode {0}
)
select * from t where t.rnum  BETWEEN :pageStr AND :pageEnd
";
            List<OracleParameter> parameterListTmp1 = new List<OracleParameter>();
            parameterListTmp1.Add(new OracleParameter(":CodeType", OracleDbType.Varchar2, 20) { Value = typeSource });
            parameterListTmp1.Add(new OracleParameter(":DistributionCode", OracleDbType.Varchar2, 100) { Value = distributionCode });
            parameterListTmp1.Add(new OracleParameter(":pageStr", OracleDbType.Decimal) { Value = pi.CurrentPageStartRowNum });
            parameterListTmp1.Add(new OracleParameter(":pageEnd", OracleDbType.Decimal) { Value = pi.CurrentPageEndRowNum });

            sql = string.Format(sql, sbWhere);
            parameterListTmp1.AddRange(parameterList);

            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterListTmp1.ToArray())).Tables[0];
        }

        public DataTable SearchPeriodExpressRelation(string typeSource, string distributionCode,string relationName, ref PageInfo pi)
        {
            string statSql = @"
WITH t AS (
SELECT DISTINCT
        ec.TopCODCompanyID AS ExpressCompanyID,
        (CASE 
                           WHEN NVL(ec2.AccountCompanyName, '') = '' THEN ec2.CompanyName
                           ELSE ec2.AccountCompanyName
                      END) CompanyName
 FROM   ExpressCompany  ec
        JOIN ExpressCompany  ec2
             ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
 WHERE  ec.DistributionCode <> :DistributionCode
        AND NVL(ec.TopCODCompanyID, 0) > 0
        AND ec.CompanyFlag=3
        AND ec.IsDeleted = 0 
)
SELECT count(1)
 FROM t
LEFT JOIN FMS_TypeRelation ftr ON t.ExpressCompanyID=ftr.RelationNameID AND TypeRelationName=:CodeType AND ftr.DistributionCode=:DistributionCode AND ftr.IsDelete=0
LEFT JOIN FMS_AccountPeriod fap ON fap.AccountPeriodKid=ftr.TypeCodeNo AND fap.PeriodRelationName=:CodeType
order by t.ExpressCompanyID
";
            OracleParameter[] parameters ={
                                           new OracleParameter(":CodeType",OracleDbType.Varchar2,20),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                      };
            parameters[0].Value = typeSource;
            parameters[1].Value = distributionCode;
            object n = OracleHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, statSql, parameters);
            pi.ItemCount = DataConvert.ToInt(n, 0);
            pi.PageCount = Convert.ToInt32(Math.Ceiling(pi.ItemCount * 1.0 / pi.PageSize));
            if (pi.ItemCount <= 0)
                return null;

            string sql = @"
with t as (
SELECT rownum rnum,ftr.TypeRelationKid,:CodeType AS TypeRelationName,ftr.DistributionCode,ftr.TypeCodeNo,fap.Periodname CodeDesc,ec.TopCODCompanyID AS RelationID,ftr.RelationNameID,CASE WHEN NVL(ec2.AccountCompanyName, '') = '' THEN ec2.CompanyName ELSE ec2.AccountCompanyName END RelationName,ftr.IsDelete,ftr.CreateBy,ftr.UpdateBy
 FROM   ExpressCompany  ec
        JOIN ExpressCompany  ec2 ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
LEFT JOIN FMS_TypeRelation ftr ON ec.ExpressCompanyID=ftr.RelationNameID AND TypeRelationName=:CodeType AND ftr.DistributionCode= :DistributionCode AND ftr.IsDelete=0
LEFT JOIN FMS_AccountPeriod fap ON fap.AccountPeriodKid=ftr.TypeCodeNo AND fap.PeriodRelationName=:CodeType
 WHERE  ec.DistributionCode <> :DistributionCode
        AND NVL(ec.TopCODCompanyID, 0) > 0
        AND ec.CompanyFlag=3
        AND ec.IsDeleted = 0
        )
select * from t where t.rnum  BETWEEN :pageStr AND :pageEnd

";
            OracleParameter[] parameters1 ={
                                           new OracleParameter(":CodeType",OracleDbType.Varchar2,20),
                                           new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                                           new OracleParameter(":pageStr", OracleDbType.Decimal),
                                           new OracleParameter(":pageEnd", OracleDbType.Decimal)
                                      };
            parameters1[0].Value = typeSource;
            parameters1[1].Value = distributionCode;
            parameters1[2].Value = pi.CurrentPageStartRowNum;
            parameters1[3].Value = pi.CurrentPageEndRowNum;
            return OracleHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters1).Tables[0];
        }
    }
}
