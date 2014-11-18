using System;
using System.Data;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL.BasicSetting;
using System.Collections.Generic;
using RFD.FMS.Util;
using System.Text;
using System.Data.SqlClient;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet;


namespace RFD.FMS.DAL.BasicSetting
{
	/*
     * (C)Copyright 2011-2012 如风达信息管理系统
     * 
     * 模块名称：状态信息查询（数据层）
     * 说明：查询状态信息
     * 作者：高毅勤
     * 创建日期：2011-03-02 12:11:00
     * 修改人：
     * 修改时间：
     * 修改记录：
     */
    public class StatusCodeInfoDao : SqlServerDao, IStatusCodeInfoDao
	{
        public int Exists(string codeType, string codeNo, string distributionCode)
        {
            string sqlStr = "select count(*) from StatusCodeInfo (nolock) where CodeType=@CodeType and CodeNo=@CodeNo and DistributionCode=@DistributionCode and Enabled=1 ";

            SqlParameter[] parameters ={
                                           new SqlParameter("@CodeType",SqlDbType.VarChar,20),
                                           new SqlParameter("@CodeNo",SqlDbType.VarChar,20),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                      };
            parameters[0].Value = codeType;
            parameters[1].Value = codeNo;
            parameters[2].Value = distributionCode;

            object obj = SqlHelper.ExecuteScalar(Connection, CommandType.Text, sqlStr,parameters);

            return DataConvert.ToInt(obj);
        }

		/// <summary>
		/// 根据状态类型编号获取所有状态
		/// </summary>
		/// <param name="statusTypeNo"></param>
		/// <returns></returns>
        public DataTable GetStatusInfoByCodeType(string codeType, string distributionCode, bool isEnabled)
		{
            string strSql = @"select CodeType,CodeNo,CodeDesc,Enabled,DistributionCode
                    from StatusCodeInfo (nolock)  where CodeType=@CodeType and DistributionCode=@DistributionCode {0} order by orderby"; 
            if (isEnabled)
            {
                strSql = string.Format(strSql, " and Enabled=1 ");
            }
            else
            {
                strSql = string.Format(strSql, "");
            }
            SqlParameter[] parameters ={
                                           new SqlParameter("@CodeType",SqlDbType.VarChar,20),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                      };
            parameters[0].Value = codeType;
            parameters[1].Value = distributionCode;
            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
			return dataTable;	
		}

        public StatusCodeInfo GetModel(string codeType, string codeNo, string distributionCode, bool isEnabled)
        {
            string strSql = @"select CodeType,CodeNo,CodeDesc,Enabled,DistributionCode 
                                from StatusCodeInfo (nolock)  where CodeType=@CodeType and CodeNo=@CodeNo and DistributionCode=@DistributionCode {0} ";

            if (isEnabled)
            {
                strSql = string.Format(strSql, " and Enabled=1 ");
            }
            else
            {
                strSql = string.Format(strSql, "");
            }

            SqlParameter[] parameters ={
                                           new SqlParameter("@CodeType",SqlDbType.VarChar,20),
                                           new SqlParameter("@CodeNo",SqlDbType.VarChar,20),
                                           new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                                      };
            parameters[0].Value = codeType;
            parameters[1].Value = codeNo;
            parameters[2].Value = distributionCode;

            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql,parameters).Tables[0];

            if (dataTable.Rows.Count == 0) return null;

            DataRow dataRow = dataTable.Rows[0];

            StatusCodeInfo codeInfo = new StatusCodeInfo();

            codeInfo.CodeType = DataConvert.ToString(dataRow["CodeType"]);
            codeInfo.CodeNo = DataConvert.ToString(dataRow["CodeNo"]);
            codeInfo.CodeDesc = DataConvert.ToString(dataRow["CodeDesc"]);
            codeInfo.Enable = (DataConvert.ToInt(dataRow["Enabled"]) == 1 ? true : false);
            codeInfo.DistributionCode = DataConvert.ToString(dataRow["DistributionCode"]);
            return codeInfo;
        }

        public IList<string> GetAllTypes()
        {
            string strSql = @"select distinct CodeType from StatusCodeInfo(NOLOCK)";

            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];

            IList<string> types = new List<string>();

            DataRow dataRow = null;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataRow = dataTable.Rows[i];

                types.Add(DataConvert.ToString(dataRow["CodeType"]));
            }

            return types;
        }

        public IList<StatusCodeInfo> GetListByType(string codeType, string distributionCode, bool isEnabled)
        {
            IList<StatusCodeInfo> codeInfos = new List<StatusCodeInfo>();

            DataTable table = GetStatusInfoByCodeType(codeType, distributionCode, isEnabled);

            DataRow row = null;

            StatusCodeInfo codeInfo;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                codeInfo = new StatusCodeInfo();
                codeInfo.CodeType = DataConvert.ToString(row["CodeType"]);
                codeInfo.CodeNo = DataConvert.ToString(row["CodeNo"]);
                codeInfo.CodeDesc = DataConvert.ToString(row["CodeDesc"]);
                //codeInfo.DistributionCode = DataConvert.ToString(row["DistributionCode"]);
                codeInfo.Enable = int.Parse(row["Enabled"].ToString())>0;
                codeInfos.Add(codeInfo);
            }

            return codeInfos;
        }

        public bool Add(StatusCodeInfo codeInfo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into StatusCodeInfo(");
            strSql.Append("CodeType,CodeNo,CodeDesc,Enabled,DistributionCode,OrderBy,CreatBy,CreatDate,UpdateBy,UpdateDate,IsChange)");
            strSql.Append(" values (");
            strSql.Append("@CodeType,@CodeNo,@CodeDesc,@Enabled,@DistributionCode,@OrderBy,@CreatBy,getdate(),@UpdateBy,getdate(),@IsChange)");
    
            SqlParameter[] parameters = {
				new SqlParameter("@CodeType", SqlDbType.VarChar,20),
                new SqlParameter("@CodeNo", SqlDbType.VarChar,20),
				new SqlParameter("@CodeDesc", SqlDbType.VarChar,50),
                new SqlParameter("@Enabled",SqlDbType.Int,2),
                new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                new SqlParameter("@OrderBy",SqlDbType.Int,4),
                new SqlParameter("@CreatBy",SqlDbType.NVarChar,20),
                new SqlParameter("@UpdateBy",SqlDbType.NVarChar,20),
                new SqlParameter("@IsChange",SqlDbType.Bit)
                                        };
            parameters[0].Value = codeInfo.CodeType;
            parameters[1].Value = codeInfo.CodeNo;
            parameters[2].Value = codeInfo.CodeDesc;
            parameters[3].Value = (codeInfo.Enable == true ? 1 : 0);
            parameters[4].Value = codeInfo.DistributionCode;
            parameters[5].Value = codeInfo.OrderBy;
            parameters[6].Value = codeInfo.CreatBy;
            parameters[7].Value = codeInfo.UpdateBy;
            parameters[8].Value = true;

            int count = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            return true;
        }

        public bool Update(StatusCodeInfo codeInfo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update StatusCodeInfo set ");
            //strSql.Append("CodeType=@CodeType,");
            //strSql.Append("CodeNo=@CodeNo,");
            strSql.Append("CodeDesc=@CodeDesc,");
            strSql.Append("Enabled=@Enabled,");
            strSql.Append("UpdateBy=@UpdateBy,");
            strSql.Append("UpdateDate=getdate(),");
            strSql.Append("IsChange=@IsChange");
            strSql.Append(" where CodeType=@CodeType and CodeNo=@CodeNo");
            SqlParameter[] parameters = {
				new SqlParameter("@CodeType", SqlDbType.VarChar,20),
				new SqlParameter("@CodeNo", SqlDbType.VarChar,20),
                new SqlParameter("@CodeDesc", SqlDbType.VarChar,50),
				new SqlParameter("@Enabled", SqlDbType.Int,2),
                new SqlParameter("@UpdateBy",SqlDbType.NVarChar,20),
                new SqlParameter("@IsChange",SqlDbType.Bit)
                                        };
            parameters[0].Value = codeInfo.CodeType;
            parameters[1].Value = codeInfo.CodeNo;
            parameters[2].Value = codeInfo.CodeDesc;
            parameters[3].Value = (codeInfo.Enable == true ? 1 : 0);
            parameters[4].Value = codeInfo.UpdateBy;
            parameters[5].Value = true;

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            return true;
        }

        public bool Delete(string codeType, string codeNo,string updateBy)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("UPDATE StatusCodeInfo set Enabled=0,UpdateBy=@UpdateBy, UpdateDate=getdate(),IsChange=@IsChange");
            strSql.Append(" where CodeType=@CodeType and CodeNo=@CodeNo ");

            SqlParameter[] parameters = 
            { 
                new SqlParameter("@CodeType", SqlDbType.VarChar, 20),
                new SqlParameter("@CodeNo", SqlDbType.VarChar, 20),
                new SqlParameter("@UpdateBy", SqlDbType.VarChar, 20),
                new SqlParameter("@IsChange", SqlDbType.Bit)
            };

            parameters[0].Value = codeType;
            parameters[1].Value = codeNo;
            parameters[2].Value = updateBy;
            parameters[3].Value = true;

            int rows = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            return true;
        }

        public void GetMaxNoAndOrderBy(string codeType, out string codeNo, out int orderBy)
        {
            string sql = @"SELECT @CodeNo=MAX(CAST(CodeNo as int)) ,@OrderBy=MAX(OrderBy) FROM dbo.StatusCodeInfo sci(NOLOCK) WHERE CodeType=@CodeType ";
            SqlParameter[] parameters ={
                                           new SqlParameter("@CodeType",SqlDbType.VarChar,20),
                                           new SqlParameter("@CodeNo",SqlDbType.VarChar,20),
                                           new SqlParameter("@OrderBy",SqlDbType.Int),
                                      };
            parameters[0].Value = codeType;
            parameters[1].Direction = ParameterDirection.Output;
            parameters[2].Direction = ParameterDirection.Output;
            int n = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);

            codeNo = string.IsNullOrEmpty(parameters[1].Value.ToString()) ? "0" : parameters[1].Value.ToString();
            orderBy = parameters[2].Value == DBNull.Value ? 0 : (int)parameters[2].Value;
            
        }
	}
}