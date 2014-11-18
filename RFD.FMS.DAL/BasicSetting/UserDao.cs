using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util.Security;
using Microsoft.ApplicationBlocks.Data;

using RFD.FMS.Util;
using System.Collections.Generic;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Model;

namespace RFD.FMS.DAL.BasicSetting
{
    /*
 * (C)Copyright 2011-2012 如风达信息管理系统
 * 
 * 模块名称：员工处理模块（数据层）
 * 说明：包括员工登录、员工注册、修改密码、初始化密码等内容
 * 作者：杨来旺
 * 创建日期：2011-03-0212:11:00
 * 修改人：
 * 修改时间：
 * 修改记录：
 */
    public class UserDao : SqlServerDao, IUserDao
    {

        /// <summary>
        /// 根据职工号判定是否已经存在该记录
        /// </summary>
        /// <param name="EmployeeCode"></param>
        /// <returns></returns>
        public bool Exists(string EmployeeCode)
        {
            string strSql = string.Format("select employeeid from RFD_PMS.dbo.Employee (nolock) where employeecode='{0}'", EmployeeCode);
            DataTable dataTable = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql).Tables[0];
            return dataTable.Rows.Count > 0 ? true : false;
        }

        /// <summary>
        /// 员工登录判定
        /// </summary>
        /// <param name="employee">根据职工号、员工密码、删除标志查询该员工是否存在。</param>
        /// <returns>返回员工信息</returns>
        public DataTable UserLogIn(Employee employee)
        {
            //根据职工号、员工密码、删除标志查询该员工是否存在。
            string sqlGetUserByIDAndName = "SELECT e.EmployeeID,e.EmployeeCode,e.EmployeeName,e.StationID,c.companyname,c.ExpressCompanyCode,c.DistributionCode FROM RFD_PMS.dbo.Employee e (NOLOCK),RFD_PMS.dbo.ExpressCompany c(nolock) WHERE e.StationID=c.expressCompanyid and  e.EmployeeCode='{0}' AND e.PassWord='{1}' And e.IsDeleted=0";
            sqlGetUserByIDAndName = string.Format(sqlGetUserByIDAndName, employee.EmployeeCode, MD5.Encrypt(employee.PassWord));
            DataTable dataTable =
                SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetUserByIDAndName).Tables[0];

            return dataTable;
        }

    	/// <summary>
        /// 查询员工列表
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <returns></returns>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select EmployeeID 序号,EmployeeCode 员工编号,EmployeeOldCode 员工旧编号,
                                            EmployeeName 员工名称,case Sex when 0 then '男' when 1 then '女' end 性别,Address 联系地址,CellPhone 手机,PostID 邮编,eEmail 电子邮箱,
                                            (select top 1 CompanyName from RFD_PMS.dbo.ExpressCompany p where p.ExpressCompanyID=StationID) 所在站点或部门,Deposit 押金,PDACode PDA号码,POSCode POS机号码, 
                                            Telephone 座机号,IDCard 身份证号,RelationMan 紧急联系人,RelationPhone 联系人电话,Relation 与联系人关系,
                                            case IsDeleted when 0  then '正常' when 1 then '已停用' end 停用标志  
                                            FROM RFD_PMS.dbo.Employee e(nolock) ");

            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
        }

        /// <summary>
        /// 得到员工信息实体类
        /// </summary>
        /// <param name="EmployeeID">员工编号（内码）</param>
        /// <returns>实体类</returns>
        public Employee GetModel(int EmployeeID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 EmployeeID,EmployeeCode,EmployeeOldCode,EmployeeName,Sex,PassWord,Address,Telephone,IDCard,RelationMan,RelationPhone,Relation,CellPhone,PostID,eEmail,StationID,Deposit,PDACode,POSCode,Sorting,IsDeleted,CreatBy,CreateStationID,CreateTime,UpdateBy,UpdateTime,UpdateStationID from RFD_PMS.dbo.Employee (nolock) ");
            strSql.Append(" where EmployeeID=@EmployeeID ");
            SqlParameter[] parameters = {
					new SqlParameter("@EmployeeID", SqlDbType.Int,4)};
            parameters[0].Value = EmployeeID;

            Employee model = new Employee();
            DataSet ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["EmployeeID"].ToString() != "")
                {
                    model.ID = int.Parse(ds.Tables[0].Rows[0]["EmployeeID"].ToString());
                }
                model.EmployeeCode = ds.Tables[0].Rows[0]["EmployeeCode"].ToString();
                model.EmployeeOldCode = ds.Tables[0].Rows[0]["EmployeeOldCode"].ToString();
                model.EmployeeName = ds.Tables[0].Rows[0]["EmployeeName"].ToString();
                if (ds.Tables[0].Rows[0]["Sex"].ToString() != "")
                {
                    if ((ds.Tables[0].Rows[0]["Sex"].ToString() == "1") || (ds.Tables[0].Rows[0]["Sex"].ToString().ToLower() == "true"))
                    {
                        model.Sex = true;
                    }
                    else
                    {
                        model.Sex = false;
                    }
                }
                model.PassWord = ds.Tables[0].Rows[0]["PassWord"].ToString();
                model.Address = ds.Tables[0].Rows[0]["Address"].ToString();
                model.Telephone = ds.Tables[0].Rows[0]["Telephone"].ToString();
                model.IDCard = ds.Tables[0].Rows[0]["IDCard"].ToString();
                model.RelationMan = ds.Tables[0].Rows[0]["RelationMan"].ToString();
                model.RelationPhone = ds.Tables[0].Rows[0]["RelationPhone"].ToString();
                model.Relation = ds.Tables[0].Rows[0]["Relation"].ToString();
                model.CellPhone = ds.Tables[0].Rows[0]["CellPhone"].ToString();
                model.PostID = ds.Tables[0].Rows[0]["PostID"].ToString();
                model.eEmail = ds.Tables[0].Rows[0]["eEmail"].ToString();
                if (ds.Tables[0].Rows[0]["StationID"].ToString() != "")
                {
                    model.StationID = int.Parse(ds.Tables[0].Rows[0]["StationID"].ToString());
                }
                if (ds.Tables[0].Rows[0]["Deposit"].ToString() != "")
                {
                    model.Deposit = decimal.Parse(ds.Tables[0].Rows[0]["Deposit"].ToString());
                }
                model.PDACode = ds.Tables[0].Rows[0]["PDACode"].ToString();
                model.POSCode = ds.Tables[0].Rows[0]["POSCode"].ToString();
                if (ds.Tables[0].Rows[0]["Sorting"].ToString() != "")
                {
                    model.Sorting = int.Parse(ds.Tables[0].Rows[0]["Sorting"].ToString());
                }
                if (ds.Tables[0].Rows[0]["IsDeleted"].ToString() != "")
                {
                    if ((ds.Tables[0].Rows[0]["IsDeleted"].ToString() == "1") || (ds.Tables[0].Rows[0]["IsDeleted"].ToString().ToLower() == "true"))
                    {
                        model.IsDeleted = true;
                    }
                    else
                    {
                        model.IsDeleted = false;
                    }
                }
                if (ds.Tables[0].Rows[0]["CreatBy"].ToString() != "")
                {
                    model.CreatBy = int.Parse(ds.Tables[0].Rows[0]["CreatBy"].ToString());
                }
                if (ds.Tables[0].Rows[0]["CreateStationID"].ToString() != "")
                {
                    model.CreateStationID = int.Parse(ds.Tables[0].Rows[0]["CreateStationID"].ToString());
                }
                if (ds.Tables[0].Rows[0]["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(ds.Tables[0].Rows[0]["CreateTime"].ToString());
                }
                if (ds.Tables[0].Rows[0]["UpdateBy"].ToString() != "")
                {
                    model.UpdateBy = int.Parse(ds.Tables[0].Rows[0]["UpdateBy"].ToString());
                }
                if (ds.Tables[0].Rows[0]["UpdateTime"].ToString() != "")
                {
                    model.UpdateTime = DateTime.Parse(ds.Tables[0].Rows[0]["UpdateTime"].ToString());
                }
                if (ds.Tables[0].Rows[0]["UpdateStationID"].ToString() != "")
                {
                    model.UpdateStationID = int.Parse(ds.Tables[0].Rows[0]["UpdateStationID"].ToString());
                }
                return model;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据部门ID查询部门员工信息
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public DataTable GetUserByDepartmentID(int departmentID)
        {
            string sqlGetEmployeeList = "SELECT EmployeeID as id,EmployeeCode,EmployeeOldCode,EmployeeName as name,Sex,PassWord,Address,CellPhone,PostID,eEmail,StationID,Deposit,PDACode,POSCode,Sorting,IsDeleted,CreatBy,CreateStationID,CreateTime,UpdateBy,UpdateTime,UpdateStationID FROM RFD_PMS.dbo.Employee (NOLOCK) WHERE  IsDeleted=0";

            sqlGetEmployeeList += string.Format(" and StationID={0} ", departmentID);
            
            sqlGetEmployeeList += string.Format(" order by EmployeeName ");

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, sqlGetEmployeeList).Tables[0];
        }

        public DataTable GetUserRoles(int userId)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("select rl.RoleID as id,rl.RoleName as name from RFD_PMS.dbo.Employee us(NOLOCK) inner join RFD_PMS.dbo.EmployeeRole userRole(NOLOCK) on us.EmployeeID = userRole.EmployeeID ");
            builder.Append(" inner join RFD_PMS.dbo.Role rl(NOLOCK) on userRole.RoleID = rl.RoleID where us.EmployeeID =");
            builder.Append(userId);

            DataSet dataSet = SqlHelper.ExecuteDataset(Connection, CommandType.Text, builder.ToString());

            if (dataSet.Tables.Count > 0) return dataSet.Tables[0];

            return null;
        }

		/// <summary>
		/// 根据登录账号获取用户角色列表
		/// </summary>
		/// <param name="userCode">登录账号</param>
		/// <returns></returns>
		public IDictionary<int, string> GetUserRoles(string userCode)
		{
			IDictionary<int, string> roles = new Dictionary<int, string>();
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat(@"SELECT r.RoleID, r.RoleName FROM RFD_PMS.dbo.ROLE r(NOLOCK) 
                             INNER JOIN RFD_PMS.dbo.EMPLOYEEROLE er(NOLOCK) 
                             ON r.RoleID = er.RoleID 
                             INNER JOIN RFD_PMS.dbo.EMPLOYEE e(NOLOCK)
                             ON e.EmployeeID = er.EmployeeID AND e.EmployeeCode = '{0}'", userCode);
			var ds = SqlHelper.ExecuteDataset(Connection, CommandType.Text, builder.ToString());
			if (ds != null && ds.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					roles.Add(dr["RoleID"].ConvertToInt(), dr["RoleName"].ToString());
				}
			}
			return roles;
		}

        /// <summary>
        /// 查询简单员工列表 zjw 2011-06-13
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <returns></returns>
        public DataSet GetSampList(Employee model)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append(@"select EmployeeID 序号,EmployeeCode 员工编码,EmployeeName 员工名称,
                                            (select top 1 CompanyName from RFD_PMS.dbo.ExpressCompany p where p.ExpressCompanyID=StationID) 所在部门
                                            FROM RFD_PMS.dbo.Employee e(nolock) Where 1=1 ");

            if (!string.IsNullOrEmpty(model.EmployeeCode))
            {
                strSql.Append(string.Format(" And EmployeeCode='{0}'", model.EmployeeCode));
            }

            if (!string.IsNullOrEmpty(model.EmployeeName))
            {
                strSql.Append(string.Format(" And EmployeeName='{0}'", model.EmployeeName));
            }

            strSql.Append(model.IsDeleted ? " And IsDeleted=1 " : " And  IsDeleted=0 ");

            if (model.StationID > -1)
            {
                strSql.Append(string.Format(" And StationID={0}", model.StationID));
            }

            strSql.Append(" And DistributionCode = '" + AppConfigHelper.AppSettings("RfdCode") + "'");

            strSql.Append(
                @" union  select EmployeeID 序号,EmployeeCode 员工编码,EmployeeName 员工名称,
                   (select top 1 CompanyName from RFD_PMS.dbo.ExpressCompany p(NOLOCK) where p.ExpressCompanyID=StationID) 所在部门 FROM RFD_PMS.dbo.Employee e(nolock)
                   left join RFD_PMS.dbo.ExpressCompany p(nolock)  on  p.ExpressCompanyID=StationID 
                        Where  p.CompanyFlag=4");

            if (!string.IsNullOrEmpty(model.EmployeeCode))
            {
                strSql.Append(string.Format(" And EmployeeCode='{0}'", model.EmployeeCode));
            }

            if (!string.IsNullOrEmpty(model.EmployeeName))
            {
                strSql.Append(string.Format(" And EmployeeName='{0}'", model.EmployeeName));
            }

            if (!string.IsNullOrEmpty(model.SortStr))
            {
                strSql.Append(model.SortStr);
            }

            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
        }

        public DataSet GetSampList(string employCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select EmployeeID ,EmployeeCode ,EmployeeName 
                                            FROM RFD_PMS.dbo.Employee e(nolock)  left join RFD_PMS.dbo.ExpressCompany p(nolock)  on  p.ExpressCompanyID=StationID 
                        Where  p.CompanyFlag=4 ");
            if (!string.IsNullOrEmpty(employCode))
            {
                strSql.Append(string.Format(" And EmployeeCode='{0}' or EmployeeName='{0}'", employCode));
            }
            return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString());
        }

        public bool UpdateIsChangeStatus()
        {
            string sql = "update LMS_RFD.dbo.FMS_CODBaseInfo set IsChange=1 where ID in (select top 1000 ID from LMS_RFD.dbo.FMS_CODBaseInfo baseInfo(nolock) where IsChange=0)";

            int count = SqlHelper.ExecuteNonQuery(Connection,CommandType.Text,sql);

            if (count == 0) return true;

            return false;
        }

        public bool DeleteOldData()
        {
            string sql = "delete RFD_FMS.dbo.FMS_CODBaseInfo where ID in (select top 200 ID from RFD_FMS.dbo.FMS_CODBaseInfo(nolock))";

            int count = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql);

            if (count == 0) return true;

            return false;
        }
    }
}
