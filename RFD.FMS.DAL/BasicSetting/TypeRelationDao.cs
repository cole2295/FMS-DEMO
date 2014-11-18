using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.BasicSetting;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Util;
using RFD.FMS.MODEL;

namespace RFD.FMS.DAL.BasicSetting
{
    public class TypeRelationDao : SqlServerDao, ITypeRelationDao
    {
        public DataTable SearchMerchantRelation(string typeSource, string distributionCode)
        {
            string sql = @"
WITH t AS (
SELECT m.ID,m.MerchantName
	FROM RFD_PMS.dbo.DistributionMerchantRelation dmr(NOLOCK)
	JOIN RFD_PMS.dbo.MERCHANTBASEINFO m(NOLOCK) ON dmr.MerchantId=m.ID 
	JOIN RFD_PMS.dbo.DISTRIBUTION d(NOLOCK) ON d.DistributionCode = dmr.DistributionCode
	WHERE dmr.IsDeleted=0 AND m.IsDeleted=0 AND d.IsDelete=0
	AND dmr.DistributionCode=@DistributionCode 
)
SELECT ftr.TypeRelationKid,@CodeType AS TypeRelationName,ftr.DistributionCode,ftr.TypeCodeNo,sci.CodeDesc,t.ID AS RelationID,ftr.RelationNameID,t.MerchantName AS RelationName,ftr.IsDelete,ftr.CreateBy,ftr.UpdateBy
 FROM t
LEFT JOIN FMS_TypeRelation ftr(NOLOCK) ON t.ID=ftr.RelationNameID AND TypeRelationName=@CodeType AND ftr.DistributionCode=@DistributionCode AND ftr.IsDelete=0
LEFT JOIN StatusCodeInfo sci(NOLOCK) ON sci.CodeNo=ftr.TypeCodeNo AND sci.CodeType=@CodeType
";
            SqlParameter[] parameters ={
                                           new SqlParameter("@CodeType",SqlDbType.VarChar,20),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                      };
            parameters[0].Value = typeSource;
            parameters[1].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public DataTable SearchExpressRelation(string typeSource, string distributionCode)
        {
            string sql = @"
WITH t AS (
SELECT DISTINCT
        ec.TopCODCompanyID AS ExpressCompanyID,
        CompanyName = CASE 
                           WHEN ISNULL(ec2.AccountCompanyName, '') = '' THEN ec2.CompanyName
                           ELSE ec2.AccountCompanyName
                      END
 FROM   RFD_PMS.dbo.ExpressCompany AS ec(NOLOCK)
        JOIN RFD_PMS.dbo.ExpressCompany AS ec2(NOLOCK)
             ON  ec.TopCODCompanyID = ec2.ExpressCompanyID
 WHERE  ec.DistributionCode <> @DistributionCode
        AND ISNULL(ec.TopCODCompanyID, 0) > 0
        AND ec.CompanyFlag=3
        AND ec.IsDeleted = 0 
)
SELECT ftr.TypeRelationKid,@CodeType AS TypeRelationName,ftr.DistributionCode,ftr.TypeCodeNo,sci.CodeDesc,t.ExpressCompanyID AS RelationID,ftr.RelationNameID,t.CompanyName AS RelationName,ftr.IsDelete,ftr.CreateBy,ftr.UpdateBy
 FROM t
LEFT JOIN FMS_TypeRelation ftr(NOLOCK) ON t.ExpressCompanyID=ftr.RelationNameID AND TypeRelationName=@CodeType AND ftr.DistributionCode=@DistributionCode AND ftr.IsDelete=0
LEFT JOIN StatusCodeInfo sci(NOLOCK) ON sci.CodeNo=ftr.TypeCodeNo AND sci.CodeType=@CodeType
";
            SqlParameter[] parameters ={
                                           new SqlParameter("@CodeType",SqlDbType.VarChar,20),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                      };
            parameters[0].Value = typeSource;
            parameters[1].Value = distributionCode;
            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters).Tables[0];
        }

        public bool Exists(TypeRelationModel model)
        {
            string sql = @"
    SELECT count(1)
  FROM FMS_TypeRelation
  WHERE TypeRelationName=@TypeRelationName 
AND RelationNameID=@RelationNameID 
AND DistributionCode=@DistributionCode 
AND TypeCodeNo=@TypeCodeNo
AND IsDelete=0
";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@TypeRelationName", SqlDbType.NVarChar, 50) { Value = model.TypeRelationName });
            parameterList.Add(new SqlParameter("@RelationNameID", SqlDbType.Int) { Value = model.RelationNameID });
            parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = model.DistributionCode });
            parameterList.Add(new SqlParameter("@TypeCodeNo", SqlDbType.VarChar, 20) { Value = model.TypeCodeNo });
            object n = SqlHelper.ExecuteScalar(ReadOnlyConnection, CommandType.Text, sql, ToParameters(parameterList.ToArray()));
            return DataConvert.ToInt(n) > 0;
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
  WHERE TypeRelationName=@TypeRelationName 
AND RelationNameID=@RelationNameID 
AND DistributionCode=@DistributionCode 
AND IsDelete=0
";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@TypeRelationName", SqlDbType.NVarChar, 50) { Value = model.TypeRelationName });
            parameterList.Add(new SqlParameter("@RelationNameID", SqlDbType.Int) { Value = model.RelationNameID });
            parameterList.Add(new SqlParameter("@DistributionCode", SqlDbType.NVarChar, 50) { Value = model.DistributionCode });

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameterList.ToArray());
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0 || ds.Tables[0].Rows.Count > 1)
                return null;

            return TransforToTypeRelationModel(ds.Tables[0].Rows[0]);
        }

        public TypeRelationModel TransforToTypeRelationModel(DataRow dr)
        {
            TypeRelationModel model = new TypeRelationModel();
            model.CreateBy = dr["CreateBy"] == DBNull.Value ? 0 : int.Parse(dr["CreateBy"].ToString());
            model.UpdateBy = dr["UpdateBy"] == DBNull.Value ? 0 : int.Parse(dr["UpdateBy"].ToString());
            model.IsDelete = dr["IsDelete"] == DBNull.Value ? true : bool.Parse(dr["IsDelete"].ToString());
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
          UpdateTime,
          IsChange
        )
VALUES  ( @TypeRelationKid ,
          @TypeRelationName ,
          @DistributionCode ,
          @TypeCodeNo ,
          @RelationNameID ,
          @IsDelete ,
          @CreateBy ,
          GETDATE() ,
          @UpdateBy ,
          GETDATE(),
          @IsChange
        )    
";
            SqlParameter[] parameters ={
                                          new SqlParameter("@TypeRelationKid",SqlDbType.VarChar,20),
                                          new SqlParameter("@TypeRelationName",SqlDbType.NVarChar,50),
                                          new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                          new SqlParameter("@TypeCodeNo",SqlDbType.VarChar,20),
                                          new SqlParameter("@RelationNameID",SqlDbType.Int),
                                          new SqlParameter("@IsDelete",SqlDbType.Bit),
                                          new SqlParameter("@CreateBy",SqlDbType.Int),
                                          new SqlParameter("@UpdateBy",SqlDbType.Int),
                                          new SqlParameter("@IsChange",SqlDbType.Bit)
                                      };
            parameters[0].Value = model.TypeRelationKid;
            parameters[1].Value = model.TypeRelationName;
            parameters[2].Value = model.DistributionCode;
            parameters[3].Value = model.TypeCodeNo;
            parameters[4].Value = model.RelationNameID;
            parameters[5].Value = model.IsDelete;
            parameters[6].Value = model.CreateBy;
            parameters[7].Value = model.UpdateBy;
            parameters[8].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

        public bool Update(TypeRelationModel model)
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.Append("UPDATE FMS_TypeRelation SET ");
            sbSql.Append(" TypeCodeNo=@TypeCodeNo, ");
            sbSql.Append(" UpdateBy=@UpdateBy, ");
            sbSql.Append(" UpdateTime=getdate(), ");
            sbSql.Append(" IsChange=@IsChange ");
            sbSql.Append(" where TypeRelationKid=@TypeRelationKid ");
            SqlParameter[] parameters ={
                                          new SqlParameter("@TypeRelationKid",SqlDbType.VarChar,20),
                                          new SqlParameter("@TypeCodeNo",SqlDbType.VarChar,20),
                                          new SqlParameter("@UpdateBy",SqlDbType.Int),
                                          new SqlParameter("@IsChange",SqlDbType.Bit)
                                      };
            parameters[0].Value = model.TypeRelationKid;
            parameters[1].Value = model.TypeCodeNo;
            parameters[2].Value = model.UpdateBy;
            parameters[3].Value = true;

            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sbSql.ToString(), parameters) > 0;
        }

        public bool Delete(TypeRelationModel model)
        {
            string sql = @"UPDATE FMS_TypeRelation SET IsDelete=1,UpdateBy=@UpdateBy,UpdateTime=getdate(),IsChange=1 WHERE TypeRelationKid=@TypeRelationKid ";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@UpdateBy", SqlDbType.Int) { Value = model.UpdateBy });
            parameterList.Add(new SqlParameter("@TypeRelationKid", SqlDbType.VarChar,20) { Value = model.TypeRelationKid });
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql.ToString(), parameterList.ToArray()) > 0;
        }

        public DataTable SearchPeriodMerchantRelation(string typeSource, string distributionCode,string relationName,ref PageInfo pi)
        {
            throw new Exception("SQL 没有实现");
        }

        public DataTable SearchPeriodExpressRelation(string typeSource, string distributionCode,string relationName, ref PageInfo pi)
        {
            throw new Exception("SQL 没有实现");
        }
    }
}
