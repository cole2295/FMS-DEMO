using System.Data;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.BasicSetting;


namespace RFD.FMS.DAL.BasicSetting
{
	/*
 * (C)Copyright 2011-2012 如风达信息管理系统
 * 
 * 模块名称：菜单设置模块（数据层）
 * 说明：包括菜单查询等
 * 作者：杨来旺
 * 创建日期：2011-04-02 10:19:00
 * 修改人：
 * 修改时间：
 * 修改记录：
 */
    public class MenuDao : SqlServerDao, IMenuDao
	{
		/// <summary>
		/// 根据员工编号获取菜单
		/// </summary>
		/// <param name="UserID">员工编号</param>
		/// <returns></returns>
		public DataSet GetMenuListByUserID(string UserID)
		{
//            string strSql = @"select distinct m.MenuID,MenuName,URL,MenuGroup,MenuLevel,MainSortBy,Sorting,SystemID from RFD_PMS.dbo.EmployeeRole e(nolock),RFD_PMS.dbo.RoleMenu rm(nolock),RFD_PMS.dbo.Menu m(nolock) 
//                      where e.RoleID=rm.RoleID and rm.menuID=m.menuID and e.isdeleted=0 and rm.isdeleted=0 and m.IsDelete=0 and e.employeeID={0} order by MainSortBy,Sorting";

		    string strSql =
		        @"select distinct m.MenuName,m.URL,m.MenuGroup,m.MenuLevel,m.MainSortBy,m.Sorting ,m.systemID,s.StatusName SystemName from RFD_PMS.dbo.EmployeeRole e(nolock),RFD_PMS.dbo.RoleMenu rm(nolock),RFD_PMS.dbo.Menu m(nolock) 
                     , RFD_PMS.dbo.StatusInfo s(nolock) 
                    where e.RoleID=rm.RoleID and rm.menuID=m.menuID and e.isdeleted=0  
                    and s.statustype='系统名称'and s.StatusNO=cast(m.SystemID as nvarchar(20)) and s.isdelete=0
                    and rm.isdeleted=0 and m.IsDelete=0 and e.employeeID={0} 
                    order by m.MainSortBy,m.Sorting";
			strSql = string.Format(strSql, UserID);
			return SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql);
		}

        /// <summary>
        /// 返回所有菜单
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllMenus()
        {
            var strSql = string.Format(@"SELECT  MenuID ,
                                    MenuName ,
                                    ParentID ,
                                    MenuLevel,
                                    URL URLName
                            FROM    RFD_PMS.dbo.Menu (NOLOCK) m where m.IsDelete=0
                                    ");
            return SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql).Tables[0];
        }
	}
}
