using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using RFD.FMS.Util.Security;
using RFD.FMS.AdoNet;
using RFD.FMS.Model;

namespace RFD.FMS.DAL.BasicSetting
{
	/// <summary>
	/// 雇员信息，这个跟UserDao是一个数据访问实体，因为FMS
    /// 实体的风格跟LMS不一样，所以新建了一个实体，
    /// 能够方便的访问雇员信息
	/// </summary>
    public class EmployeeDao : SqlServerDao
	{
        #region IDataAccess<Employee,int> 成员

        public bool Exists(int ID)
        {
            string strSql = string.Format("select count(*) from RFD_PMS.dbo.Employee (nolock) where EmployeeID={0}", ID);

            object resultCount = SqlHelper.ExecuteScalar(Connection, CommandType.Text, strSql);

            if (DataConvert.ToInt(resultCount) > 0) return true;

            return false;
        }

        public int Add(Employee model)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("insert into RFD_PMS.dbo.Employee(");
            strSql.Append("EmployeeCode,EmployeeOldCode,EmployeeName,Sex,PassWord,Address,CellPhone,PostID,eEmail,StationID,Deposit,PDACode,POSCode,Sorting,IsDeleted,CreatBy,CreateStationID,UpdateBy,UpdateStationID,Telephone,IDCard,RelationMan,RelationPhone,Relation)");
            strSql.Append(" values (");
            strSql.Append("@EmployeeCode,@EmployeeOldCode,@EmployeeName,@Sex,@PassWord,@Address,@CellPhone,@PostID,@eEmail,@StationID,@Deposit,@PDACode,@POSCode,@Sorting,@IsDeleted,@CreatBy,@CreateStationID,@UpdateBy,@UpdateStationID,@Telephone,@IDCard,@RelationMan,@RelationPhone,@Relation)");
            strSql.Append(";select @@IDENTITY");

            SqlParameter[] parameters = 
            {
                new SqlParameter("@EmployeeCode", SqlDbType.NVarChar, 20),
                new SqlParameter("@EmployeeOldCode", SqlDbType.NVarChar, 20),
                new SqlParameter("@EmployeeName", SqlDbType.NVarChar, 50),
                new SqlParameter("@Sex", SqlDbType.Bit, 1),
                new SqlParameter("@PassWord", SqlDbType.NVarChar, 100),
                new SqlParameter("@Address", SqlDbType.NVarChar, 100),
                new SqlParameter("@CellPhone", SqlDbType.NVarChar, 15),
                new SqlParameter("@PostID", SqlDbType.NVarChar, 50),
                new SqlParameter("@eEmail", SqlDbType.NVarChar, 50),
                new SqlParameter("@StationID", SqlDbType.Int, 4),
                new SqlParameter("@Deposit", SqlDbType.Decimal, 9),
                new SqlParameter("@PDACode", SqlDbType.NVarChar, 20),
                new SqlParameter("@POSCode", SqlDbType.NVarChar, 20),
                new SqlParameter("@Sorting", SqlDbType.Int, 4),
                new SqlParameter("@IsDeleted", SqlDbType.Bit, 1),
                new SqlParameter("@CreatBy", SqlDbType.Int, 4),
                new SqlParameter("@CreateStationID", SqlDbType.Int, 4),
                new SqlParameter("@UpdateBy", SqlDbType.Int, 4),
                new SqlParameter("@UpdateStationID", SqlDbType.Int, 4),
                new SqlParameter("@Telephone", SqlDbType.NVarChar,50),
                new SqlParameter("@IDCard", SqlDbType.NVarChar,50),
                new SqlParameter("@RelationMan", SqlDbType.NVarChar,50),
                new SqlParameter("@RelationPhone", SqlDbType.NVarChar,50),
                new SqlParameter("@Relation", SqlDbType.NVarChar,50)
            };

            parameters[0].Value = model.EmployeeCode;
            parameters[1].Value = model.EmployeeOldCode;
            parameters[2].Value = model.EmployeeName;
            parameters[3].Value = model.Sex;
            parameters[4].Value = MD5.Encrypt(model.PassWord);
            parameters[5].Value = model.Address;
            parameters[6].Value = model.CellPhone;
            parameters[7].Value = model.PostID;
            parameters[8].Value = model.eEmail;
            parameters[9].Value = model.StationID;
            parameters[10].Value = model.Deposit;
            parameters[11].Value = model.PDACode;
            parameters[12].Value = model.POSCode;
            parameters[13].Value = model.Sorting;
            parameters[14].Value = model.IsDeleted;
            parameters[15].Value = model.CreatBy;
            parameters[16].Value = model.CreateStationID;
            parameters[17].Value = model.UpdateBy;
            parameters[18].Value = model.UpdateStationID;
            parameters[19].Value = model.Telephone;
            parameters[20].Value = model.IDCard;
            parameters[21].Value = model.RelationMan;
            parameters[22].Value = model.RelationPhone;
            parameters[23].Value = model.Relation;

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rowCount == 0) throw new Exception("新增“员工表”记录失败！");

            return rowCount;
        }

        public bool Update(Employee model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update RFD_PMS.dbo.Employee set ");
            strSql.Append("EmployeeCode=@EmployeeCode,");
            strSql.Append("EmployeeOldCode=@EmployeeOldCode,");
            strSql.Append("EmployeeName=@EmployeeName,");
            strSql.Append("Sex=@Sex,");
            strSql.Append("Address=@Address,");
            strSql.Append("Telephone=@Telephone,");
            strSql.Append("IDCard=@IDCard,");
            strSql.Append("RelationMan=@RelationMan,");
            strSql.Append("RelationPhone=@RelationPhone,");
            strSql.Append("Relation=@Relation,");
            strSql.Append("CellPhone=@CellPhone,");
            strSql.Append("PostID=@PostID,");
            strSql.Append("eEmail=@eEmail,");
            strSql.Append("StationID=@StationID,");
            strSql.Append("Deposit=@Deposit,");
            strSql.Append("PDACode=@PDACode,");
            strSql.Append("POSCode=@POSCode,");
            strSql.Append("Sorting=@Sorting,");
            strSql.Append("IsDeleted=@IsDeleted,");
            strSql.Append("CreatBy=@CreatBy,");
            strSql.Append("CreateStationID=@CreateStationID,");
            strSql.Append("CreateTime=@CreateTime,");
            strSql.Append("UpdateBy=@UpdateBy,");
            strSql.Append("UpdateTime=@UpdateTime,");
            strSql.Append("UpdateStationID=@UpdateStationID");
            strSql.Append(" where EmployeeID=@EmployeeID ");

            SqlParameter[] parameters = 
            {
				new SqlParameter("@EmployeeID", SqlDbType.Int,4),
				new SqlParameter("@EmployeeCode", SqlDbType.NVarChar,20),
				new SqlParameter("@EmployeeOldCode", SqlDbType.NVarChar,20),
				new SqlParameter("@EmployeeName", SqlDbType.NVarChar,50),
				new SqlParameter("@Sex", SqlDbType.Bit,1),
				new SqlParameter("@Address", SqlDbType.NVarChar,100),
				new SqlParameter("@Telephone", SqlDbType.NVarChar,50),
				new SqlParameter("@IDCard", SqlDbType.NVarChar,50),
				new SqlParameter("@RelationMan", SqlDbType.NVarChar,50),
				new SqlParameter("@RelationPhone", SqlDbType.NVarChar,50),
				new SqlParameter("@Relation", SqlDbType.NVarChar,50),
				new SqlParameter("@CellPhone", SqlDbType.NVarChar,15),
				new SqlParameter("@PostID", SqlDbType.NVarChar,50),
				new SqlParameter("@eEmail", SqlDbType.NVarChar,50),
				new SqlParameter("@StationID", SqlDbType.Int,4),
				new SqlParameter("@Deposit", SqlDbType.Decimal,9),
				new SqlParameter("@PDACode", SqlDbType.NVarChar,20),
				new SqlParameter("@POSCode", SqlDbType.NVarChar,20),
				new SqlParameter("@Sorting", SqlDbType.Int,4),
				new SqlParameter("@IsDeleted", SqlDbType.Bit,1),
				new SqlParameter("@CreatBy", SqlDbType.Int,4),
				new SqlParameter("@CreateStationID", SqlDbType.Int,4),
				new SqlParameter("@CreateTime", SqlDbType.DateTime),
				new SqlParameter("@UpdateBy", SqlDbType.Int,4),
				new SqlParameter("@UpdateTime", SqlDbType.DateTime),
				new SqlParameter("@UpdateStationID", SqlDbType.Int,4)
            };

            parameters[0].Value = model.ID;
            parameters[1].Value = model.EmployeeCode;
            parameters[2].Value = model.EmployeeOldCode;
            parameters[3].Value = model.EmployeeName;
            parameters[4].Value = model.Sex;
            parameters[5].Value = model.Address;
            parameters[6].Value = model.Telephone;
            parameters[7].Value = model.IDCard;
            parameters[8].Value = model.RelationMan;
            parameters[9].Value = model.RelationPhone;
            parameters[10].Value = model.Relation;
            parameters[11].Value = model.CellPhone;
            parameters[12].Value = model.PostID;
            parameters[13].Value = model.eEmail;
            parameters[14].Value = model.StationID;
            parameters[15].Value = model.Deposit;
            parameters[16].Value = model.PDACode;
            parameters[17].Value = model.POSCode;
            parameters[18].Value = model.Sorting;
            parameters[19].Value = model.IsDeleted;
            parameters[20].Value = model.CreatBy;
            parameters[21].Value = model.CreateStationID;
            parameters[22].Value = model.CreateTime;
            parameters[23].Value = model.UpdateBy;
            parameters[24].Value = model.UpdateTime;
            parameters[25].Value = model.UpdateStationID;

            int rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (rowCount == 0) throw new Exception("更新“员工表”记录失败！");

            return true;
        }

        public bool Delete(int ID)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string condition)
        {
            throw new NotImplementedException();
        }

        public bool DeleteList(string IDlist)
        {
            throw new NotImplementedException();
        }

        public Employee GetModel(int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 EmployeeID,EmployeeCode,EmployeeOldCode,EmployeeName,Sex,PassWord,Address,Telephone,IDCard,RelationMan,RelationPhone,Relation,CellPhone,PostID,eEmail,StationID,Deposit,PDACode,POSCode,Sorting,IsDeleted,CreatBy,CreateStationID,CreateTime,UpdateBy,UpdateTime,UpdateStationID from RFD_PMS.dbo.Employee (nolock) ");
            strSql.Append(" where EmployeeID=@EmployeeID ");

            SqlParameter[] parameters = { new SqlParameter("@EmployeeID", SqlDbType.Int,4) };

            parameters[0].Value = ID;

            Employee model = new Employee();

            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);

            if (ds.Tables[0].Rows.Count > 0)
            {
                model = ConvertRowToObject(ds.Tables[0].Rows[0]);

                return model;
            }
            else
            {
                return null;
            }
        }

        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("select EmployeeID,EmployeeCode,EmployeeOldCode,EmployeeName,Sex,PassWord,Address,Telephone,IDCard,RelationMan,RelationPhone,Relation,CellPhone,PostID,eEmail,StationID,Deposit,PDACode,POSCode,Sorting,IsDeleted,CreatBy,CreateStationID,CreateTime,UpdateBy,UpdateTime,UpdateStationID from RFD_PMS.dbo.Employee (nolock) ");

            strSql.Append(" where ");

            if (strWhere.Trim().Length == 0)
            {
                strSql.Append(" 1=2 ");
            }
            else
            {
                strSql.Append(strWhere);
            }

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
        }

        public IList<Employee> GetObjects(string strWhere)
        {
            IList<Employee> employees = new List<Employee>();

            DataTable table = GetList(strWhere).Tables[0];

            DataRow dataRow = null;

            Employee employee = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                dataRow = table.Rows[i];

                employee = ConvertRowToObject(dataRow);

                employees.Add(employee);
            }

            return employees;
        }

        #endregion

        private Employee ConvertRowToObject(DataRow dataRow)
        {
            Employee employee = new Employee();

            employee.ID = DataConvert.ToInt(dataRow["EmployeeID"]);
            employee.EmployeeCode = DataConvert.ToString(dataRow["EmployeeCode"]);
            employee.EmployeeOldCode = DataConvert.ToString(dataRow["EmployeeOldCode"]);
            employee.EmployeeName = DataConvert.ToString(dataRow["EmployeeName"]);
            employee.Sex = DataConvert.ToBoolean(dataRow["Sex"]);
            employee.PassWord = DataConvert.ToString(dataRow["PassWord"]);
            employee.Address = DataConvert.ToString(dataRow["Address"]);
            employee.Telephone = DataConvert.ToString(dataRow["Telephone"]);
            employee.IDCard = DataConvert.ToString(dataRow["IDCard"]);
            employee.RelationMan = DataConvert.ToString(dataRow["RelationMan"]);
            employee.RelationPhone = DataConvert.ToString(dataRow["RelationPhone"]);
            employee.Relation = DataConvert.ToString(dataRow["Relation"]);
            employee.CellPhone = DataConvert.ToString(dataRow["CellPhone"]);
            employee.PostID = DataConvert.ToString(dataRow["PostID"]);
            employee.eEmail = DataConvert.ToString(dataRow["eEmail"]);
            employee.StationID = DataConvert.ToInt(dataRow["StationID"]);
            employee.Deposit = DataConvert.ToDecimal(dataRow["Deposit"]);
            employee.PDACode = DataConvert.ToString(dataRow["PDACode"]);
            employee.POSCode = DataConvert.ToString(dataRow["POSCode"]);
            employee.Sorting = DataConvert.ToInt(dataRow["Sorting"]);
            employee.IsDeleted = DataConvert.ToBoolean(dataRow["IsDeleted"]);
            employee.CreatBy = DataConvert.ToInt(dataRow["CreatBy"]);
            employee.CreateStationID = DataConvert.ToInt(dataRow["CreateStationID"]);
            employee.CreateTime = DataConvert.ToDateTime(dataRow["CreateTime"]);
            employee.UpdateBy = DataConvert.ToInt(dataRow["UpdateBy"]);
            employee.UpdateTime = DataConvert.ToDateTime(dataRow["UpdateTime"]);
            employee.UpdateStationID = DataConvert.ToInt(dataRow["UpdateStationID"]);

            return employee;
        }
    }
}

