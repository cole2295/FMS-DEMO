using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.FinancialManage;
using RFD.FMS.MODEL.FinancialManage;

namespace RFD.FMS.DAL.Oracle.FinancialManage
{
    public class AreaDeductErrorInfoDao : OracleDao, IAreaDeductErrorInfoDao
    {
        private const string AreaDeductError_NSEQAME = "PS_LMS.SEQ_AreaDeductErrorInfo";
        private const string TableName = @"PS_LMS.AreaDeductErrorInfo";

        private const string PagingTemplate =
            @"SELECT  RowIndex ,
																					T.*
																			FROM    ( SELECT    T2.* ,
																								ROW_NUMBER() OVER ( ORDER BY {0} DESC ) AS RowIndex
																					  FROM   ( {1} )  T2
																					) AS T
																			WHERE   T.RowIndex > {2}
																			AND T.RowIndex <= {3}";

        public AreaDeductErrorInfoDao()
        {

        }

        public bool Exists(Int64 areadeducterrorinfoid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("select count(1) from {0}", TableName));
            strSql.Append(string.Format(" where {0} = :{0}", "AreaDeductErrorInfoId"));
            var sqlParams = new List<OracleParameter>()
                                {
                                    new OracleParameter(string.Format(":{0}", "AreaDeductErrorInfoId"),
                                                        areadeducterrorinfoid)
                                };
            return
                Convert.ToInt64(OracleHelper.ExecuteScalar(Connection, CommandType.Text, strSql.ToString(),
                                                           sqlParams.ToArray())) > 0;
        }




        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(AreaDeductErrorInfo model)
        {
            decimal AreaDeductErrorID = GetIdNew(AreaDeductError_NSEQAME);
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format(@"
insert into AreaDeductErrorInfo( AreaDeductErrorInfoId ,  waybillNO ,  StationId , DeductType ,  AddressInfo ,  KeyWords ,  AreaCommision ,  Errortype , ErrorInfo ,  DisposeStatus ,  IsDelete ,  CreateTime ,  CreateBy ,
UpdateTime ,  UpdateBy  ) 
SELECT :AreaDeductErrorInfoId ,  :waybillNO ,  :StationId ,  :DeductType , :AddressInfo , :KeyWords ,  :AreaCommision ,  :Errortype , 
:ErrorInfo , :DisposeStatus ,  :IsDelete ,  :CreateTime ,  :CreateBy ,  :UpdateTime ,  :UpdateBy  
from dual where not exists 
(SELECT AREADEDUCTERRORINFOID FROM AREADEDUCTERRORINFO WHERE WAYBILLNO=:waybillNO AND DeductType=:DeductType AND ISDELETE=0)

"));
            OracleParameter[] parameters = {
                                               new OracleParameter(string.Format(":{0}", "AreaDeductErrorInfoId"),AreaDeductErrorID),
                                               new OracleParameter(string.Format(":{0}", "waybillNO"), model.WaybillNO),
                                               new OracleParameter(string.Format(":{0}", "StationId"), model.StationId),
                                               new OracleParameter(string.Format(":{0}", "DeductType"), model.DeductType),
                                               new OracleParameter(string.Format(":{0}", "AddressInfo"),model.AddressInfo),
                                               new OracleParameter(string.Format(":{0}", "KeyWords"), model.KeyWords),
                                               new OracleParameter(string.Format(":{0}", "AreaCommision"),model.AreaCommision),
                                               new OracleParameter(string.Format(":{0}", "Errortype"), model.Errortype),
                                               new OracleParameter(string.Format(":{0}", "ErrorInfo"), model.ErrorInfo),
                                               new OracleParameter(string.Format(":{0}", "DisposeStatus"),model.DisposeStatus),
                                               new OracleParameter(string.Format(":{0}", "IsDelete"), model.IsDelete?1:0),
                                               new OracleParameter(string.Format(":{0}", "CreateTime"), model.CreateTime),
                                               new OracleParameter(string.Format(":{0}", "CreateBy"), model.CreateBy),
                                               new OracleParameter(string.Format(":{0}", "UpdateTime"), model.UpdateTime),
                                               new OracleParameter(string.Format(":{0}", "UpdateBy"), model.UpdateBy)
                                           };
            object obj = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(),
                                                    parameters);
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
        public bool Update(AreaDeductErrorInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("update {0} set ", TableName));


            strSql.Append(" waybillNO = :waybillNO ,	 ");

            strSql.Append(" StationId = :StationId ,	 ");

            strSql.Append(" DeductType = :DeductType ,	 ");

            strSql.Append(" AddressInfo = :AddressInfo ,	 ");

            strSql.Append(" KeyWords = :KeyWords ,	 ");

            strSql.Append(" AreaCommision = :AreaCommision ,	 ");

            strSql.Append(" Errortype = :Errortype ,	 ");

            strSql.Append(" ErrorInfo = :ErrorInfo ,	 ");

            strSql.Append(" DisposeStatus = :DisposeStatus ,	 ");

            strSql.Append(" IsDelete = :IsDelete ,	 ");

            strSql.Append(" CreateTime = :CreateTime ,	 ");

            strSql.Append(" CreateBy = :CreateBy ,	 ");

            strSql.Append(" UpdateTime = :UpdateTime ,	 ");

            strSql.Append(" UpdateBy = :UpdateBy  ");

            strSql.Append(string.Format(" where {0} = :{0}", "AreaDeductErrorInfoId"));
            OracleParameter[] parameters = {
                                               new OracleParameter(string.Format(":{0}", "AreaDeductErrorInfoId"),
                                                                   model.AreaDeductErrorInfoId),
                                               new OracleParameter(string.Format(":{0}", "waybillNO"), model.WaybillNO),
                                               new OracleParameter(string.Format(":{0}", "StationId"), model.StationId),
                                               new OracleParameter(string.Format(":{0}", "DeductType"), model.DeductType)
                                               ,
                                               new OracleParameter(string.Format(":{0}", "AddressInfo"),
                                                                   model.AddressInfo),
                                               new OracleParameter(string.Format(":{0}", "KeyWords"), model.KeyWords),
                                               new OracleParameter(string.Format(":{0}", "AreaCommision"),
                                                                   model.AreaCommision)
                                               ,
                                               new OracleParameter(string.Format(":{0}", "Errortype"), model.Errortype),
                                               new OracleParameter(string.Format(":{0}", "ErrorInfo"), model.ErrorInfo),
                                               new OracleParameter(string.Format(":{0}", "DisposeStatus"),
                                                                   model.DisposeStatus)
                                               , new OracleParameter(string.Format(":{0}", "IsDelete"), model.IsDelete),
                                               new OracleParameter(string.Format(":{0}", "CreateTime"), model.CreateTime)
                                               ,
                                               new OracleParameter(string.Format(":{0}", "CreateBy"), model.CreateBy),
                                               new OracleParameter(string.Format(":{0}", "UpdateTime"), model.UpdateTime)
                                               ,
                                               new OracleParameter(string.Format(":{0}", "UpdateBy"), model.UpdateBy),
                                           };
            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(),
                                                    parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(Int64 areadeducterrorinfoid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("delete from {0} ", TableName));
            strSql.Append(string.Format(" where {0} = :{0}", "AreaDeductErrorInfoId"));
            var sqlParams = new List<OracleParameter>()
                                {
                                    new OracleParameter(string.Format(":{0}", "AreaDeductErrorInfoId"),
                                                        areadeducterrorinfoid)
                                };
            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(),
                                                    sqlParams.ToArray());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AreaDeductErrorInfo GetModel(DataRow row)
        {
            var model = new AreaDeductErrorInfo();
            if (row["AreaDeductErrorInfoId"].ToString() != "")
            {
                model.AreaDeductErrorInfoId = Int64.Parse(row["AreaDeductErrorInfoId"].ToString());
            }
            if (row["waybillNO"].ToString() != "")
            {
                model.WaybillNO = Int64.Parse(row["waybillNO"].ToString());
            }
            if (row["StationId"].ToString() != "")
            {
                model.StationId = Int32.Parse(row["StationId"].ToString());
            }
            model.DeductType = row["DeductType"].ToString();
            model.AddressInfo = row["AddressInfo"].ToString();
            model.KeyWords = row["KeyWords"].ToString();
            model.AreaCommision = row["AreaCommision"].ToString();
            model.Errortype = row["Errortype"].ToString();
            model.ErrorInfo = row["ErrorInfo"].ToString();
            model.DisposeStatus = row["DisposeStatus"].ToString();
            if (row["IsDelete"].ToString() != "")
            {
                if ((row["IsDelete"].ToString() == "1") || (row["IsDelete"].ToString().ToLower() == "true"))
                {
                    model.IsDelete = true;
                }
                else
                {
                    model.IsDelete = false;
                }
            }
            if (row["CreateTime"].ToString() != "")
            {
                model.CreateTime = System.DateTime.Parse(row["CreateTime"].ToString());
            }
            if (row["CreateBy"].ToString() != "")
            {
                model.CreateBy = Int32.Parse(row["CreateBy"].ToString());
            }
            if (row["UpdateTime"].ToString() != "")
            {
                model.UpdateTime = System.DateTime.Parse(row["UpdateTime"].ToString());
            }
            if (row["UpdateBy"].ToString() != "")
            {
                model.UpdateBy = Int32.Parse(row["UpdateBy"].ToString());
            }

            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AreaDeductErrorInfo GetModel(Int64 areadeducterrorinfoid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(
                "select AreaDeductErrorInfoId, waybillNO, StationId, DeductType, AddressInfo, KeyWords, AreaCommision, Errortype, ErrorInfo, DisposeStatus, IsDelete, CreateTime, CreateBy, UpdateTime, UpdateBy  ");
            strSql.Append(string.Format("  from {0} ", TableName));
            strSql.Append(string.Format(" where {0} = :{0}", "AreaDeductErrorInfoId"));
            var sqlParams = new List<OracleParameter>()
                                {
                                    new OracleParameter(string.Format(":{0}", "AreaDeductErrorInfoId"),
                                                        areadeducterrorinfoid)
                                };
            var model = new AreaDeductErrorInfo();
            DataSet ds = OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql.ToString(),
                                                     sqlParams.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                model = GetModel(ds.Tables[0].Rows[0]);
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public List<AreaDeductErrorInfo> GetModelList(Dictionary<string, object> searchParams)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(
                "select AreaDeductErrorInfoId, waybillNO, StationId, DeductType, AddressInfo, KeyWords, AreaCommision, Errortype, ErrorInfo, DisposeStatus, IsDelete, CreateTime, CreateBy, UpdateTime, UpdateBy  ");
            strSql.Append(string.Format("  from {0} ", TableName));
            strSql.Append(" where 1 = 1 ");
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item =>
                {
                    strSql.Append(string.Format(" and {0} = :{0}", item.Key));
                    sqlParams.Add(new OracleParameter(
                                      string.Format(":{0}", item.Key), item.Value));
                });
            }
            var modelList = new List<AreaDeductErrorInfo>();
            DataSet ds = OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql.ToString(),
                                                     sqlParams.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    modelList.Add(GetModel(row));
                }
            }
            return modelList;
        }

        public int GetDataTableCount(string searchString, Dictionary<string, object> searchParams)
        {
            var sqlStr = string.Format(@"SELECT COUNT(*) FROM {0} {1}", TableName, searchString);
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(
                    item => sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value)));
            }
            var obj = OracleHelper.ExecuteScalar(LMSReadOnlyConnection, CommandType.Text, sqlStr, sqlParams.ToArray());
            int i = 0;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out i);
            }
            return i;
        }

        public DataTable GetDataTable(Dictionary<string, object> searchParams)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(
                "select AreaDeductErrorInfoId, waybillNO, StationId, DeductType, AddressInfo, KeyWords, AreaCommision, Errortype, ErrorInfo, DisposeStatus, IsDelete, CreateTime, CreateBy, UpdateTime, UpdateBy  ");
            strSql.Append(string.Format("  from {0} ", TableName));
            strSql.Append(" where 1 = 1 ");
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(item =>
                {
                    strSql.Append(string.Format(" and {0} = :{0}", item.Key));
                    sqlParams.Add(new OracleParameter(
                                      string.Format(":{0}", item.Key), item.Value));
                });
            }
            return
                OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql.ToString(),
                                            sqlParams.ToArray()).Tables[0];
        }

        public DataTable GetDataTable(string searchString, string sortColumn, Dictionary<string, object> searchParams)
        {
            var sqlStr = string.Format(@"SELECT * FROM {0} {1} ORDER BY {2} DESC", TableName, searchString, sortColumn);
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(
                    item => sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value)));
            }
            return
                OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, sqlStr, sqlParams.ToArray()).
                    Tables[0];
        }

        public DataTable GetPageDataTable(string searchString, string sortColumn,
                                          Dictionary<string, object> searchParams, int rowStart, int rowEnd)
        {
            var sqlStr = string.Format(@"SELECT * FROM {0} {1}", TableName, searchString);

            sqlStr = string.Format(PagingTemplate, sortColumn, sqlStr, rowStart, rowEnd);
            var sqlParams = new List<OracleParameter>();
            if (searchParams != null)
            {
                searchParams.ToList().ForEach(
                    item => sqlParams.Add(new OracleParameter(string.Format(":{0}", item.Key), item.Value)));
            }
            return
                OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, sqlStr, sqlParams.ToArray()).
                    Tables[0];
        }


        /// <summary>
        /// 获取没有重新计算的区域提成异常信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetErrorAreaDeductListForService()
        {
            string strSQL =
                @"SELECT ade.AREADEDUCTERRORINFOID,ade.STATIONID, ade.WAYBILLNO, ade.ADDRESSINFO,ade.DEDUCTTYPE,ade.DISPOSESTATUS,ade.ISDELETE  FROM AREADEDUCTERRORINFO ade
                    WHERE ade.ISDELETE=0 AND ade.ERRORTYPE=1 AND rownum<=100 AND ade.UPDATETIME<sysdate-1/144  AND ade.CREATETIME>sysdate-31";

            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSQL);

            return ds.Tables[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areadeducterrorinfoid"></param>
        /// <returns></returns>
        public bool UpdateIsDeleted(Int64 areadeducterrorinfoid)
        {
            string strSql = @"UPDATE AREADEDUCTERRORINFO SET ISDELETE=1,UPDATETIME=sysdate,DISPOSESTATUS='已处理' WHERE AREADEDUCTERRORINFOID=:AREADEDUCTERRORINFOID";
            OracleParameter[] parameters = 
            {
                new OracleParameter(":AREADEDUCTERRORINFOID",areadeducterrorinfoid)
            };
            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(),
                                                    parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areadeducterrorinfoid"></param>
        /// <returns></returns>
        public bool UpdateErrorType(Int64 areadeducterrorinfoid)
        {
            string strSql = @"UPDATE AREADEDUCTERRORINFO SET ERRORTYPE=2,UPDATETIME=sysdate,DISPOSESTATUS='已计算' WHERE AREADEDUCTERRORINFOID=:AREADEDUCTERRORINFOID";
            OracleParameter[] parameters = 
            {
                new OracleParameter(":AREADEDUCTERRORINFOID",areadeducterrorinfoid)
            };
            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(),
                                                    parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取重新计算的区域提成异常信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetErrorTypeAreaDeductListForService(int STATIONID)
        {
            string strSQL =
                @"SELECT ade.AREADEDUCTERRORINFOID,ade.STATIONID, ade.WAYBILLNO, ade.ADDRESSINFO,ade.DEDUCTTYPE,ade.DISPOSESTATUS,ade.ISDELETE  FROM AREADEDUCTERRORINFO ade
                    WHERE ade.ISDELETE=0 AND ade.ERRORTYPE=2 AND STATIONID=:STATIONID AND ade.UPDATETIME<sysdate-1/144 AND ade.CREATETIME>sysdate-31";

            OracleParameter[] parameters = 
            {
                new OracleParameter(":STATIONID",STATIONID)
            };
            DataSet ds = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSQL, parameters);

            return ds.Tables[0];
        }

        /// <summary>
        /// 修改异常原因为可计算
        /// </summary>
        /// <param name="areadeducterrorinfoid"></param>
        /// <returns></returns>
        public bool UpdateErrorTypeForStat(Int64 areadeducterrorinfoid, int ErrorType)
        {
            string strSql = @"UPDATE AREADEDUCTERRORINFO SET ERRORTYPE=:ERRORTYPE,UPDATETIME=sysdate,DISPOSESTATUS='未处理' WHERE AREADEDUCTERRORINFOID=:AREADEDUCTERRORINFOID";
            OracleParameter[] parameters = 
            {
                new OracleParameter(":AREADEDUCTERRORINFOID",areadeducterrorinfoid),
                new OracleParameter(":ERRORTYPE",ErrorType)
            };
            int rows = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(),
                                                    parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}
