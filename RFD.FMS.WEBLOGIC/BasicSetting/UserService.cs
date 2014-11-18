using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Util;
using RFD.FMS.WEBLOGIC;
using System.Text;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Model;


namespace RFD.FMS.WEBLOGIC.BasicSetting
{
	[DataObject]
    public class UserService : IUserService
	{
        private IUserDao _userDao;

		public UserService()
		{
			
		}

		/// <summary>
		/// 员工登录判定
		/// </summary>
		/// <param name="employee">根据职工号、员工密码、删除标志查询该员工是否存在。</param>
		/// <returns>如果存在返回True，否则返回false</returns>
		public bool UserLogIn(Employee employee)
		{
			//如果登录成功要给PageBase赋值
            DataTable dataTable = _userDao.UserLogIn(employee);

			if (dataTable.Rows.Count > 0)
			{
				CookieUtil.AddCookie(LoginUser.CookieUserID, dataTable.Rows[0]["EmployeeID"].ToString(), 18);
				CookieUtil.AddCookie(LoginUser.CookieUserCode, dataTable.Rows[0]["EmployeeCode"].ToString(), 18);
				CookieUtil.AddCookie(LoginUser.CookieUserName, dataTable.Rows[0]["EmployeeName"].ToString(), 18);
				CookieUtil.AddCookie(LoginUser.CookieExpressId, dataTable.Rows[0]["StationID"].ToString(), 18);
				CookieUtil.AddCookie(LoginUser.CookieExpressName, dataTable.Rows[0]["CompanyName"].ToString(), 18);
				CookieUtil.AddCookie(LoginUser.CookieExpressCode, dataTable.Rows[0]["ExpressCompanyCode"].ToString(), 18);
                CookieUtil.AddCookie(LoginUser.CookieDistributionCode, dataTable.Rows[0]["DistributionCode"].ToString(), 18);
				//string sid = userMappingSv.UpdateUserMapping(dataTable.Rows[0]["EmployeeCode"].ToString());

				//CookieUtil.AddCookie("RFDLMSUserSid", sid, 18);

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 查询员工列表
		/// </summary>
		/// <param name="model">查询条件</param>
		/// <returns></returns>
		public DataSet GetList(string where)
		{
            return _userDao.GetList(where);
		}


		/// <summary>
		/// 得到员工信息实体类
		/// </summary>
		/// <param name="EmployeeID">员工编号（内码）</param>
		/// <returns>实体类</returns>
		public Employee GetModel(int EmployeeID)
		{
            return _userDao.GetModel(EmployeeID);
		}

        public KeyValuePair<int,string> GetCompanyByUser(int userId)
        {
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
            DataTable table = expressCompanyService.GetCompanyByUserId(userId);

            if (table.Rows.Count == 0) return default(KeyValuePair<int,string>);

            int id = DataConvert.ToInt(table.Rows[0]["id"]);
            string name = DataConvert.ToString(table.Rows[0]["name"]);

            KeyValuePair<int, string> keyValue = new KeyValuePair<int, string>(id,name);

            return keyValue;
        }

        public UserTree<string> CreateDepartmentTree()
        {
            var rootNode = new UserTreeNode<string>()
            {
                ID = "-1",
                Name = "费用管理系统",
                PID = "0"
            };

            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();

            UserTree<string> userTree = new UserTree<string>();

            DataTable dt = expressCompanyService.GetAreaOrganizationCompany();

            var currentDistrictId = string.Empty;
            var currentProvinceId = string.Empty;
            var currentCityId = string.Empty;
            var currentFunDepartId = string.Empty;
            var currentExpressCompanyId = string.Empty;
            var currentDistrictIdNode = new UserTreeNode<string>();
            var currentProvinceIdNode = new UserTreeNode<string>();
            var currentCityIdNode = new UserTreeNode<string>();
            var currentExpressCompanyIdNode = new UserTreeNode<string>();
            var currentFunDepartIdNode = new UserTreeNode<string>();

            dt.AsEnumerable().ToList().ForEach(row =>
            {
                if (row["DistrictID"].ToString() != currentDistrictId)
                {
                    currentDistrictIdNode = CreateChildNode(rootNode, row["DistrictID"].ToString(), row["DistrictName"].ToString(), rootNode.ID.ToString(), "District");

                    currentDistrictId = row["DistrictID"].ToString();
                }

                if (DataConvert.ToString(row["funDepartID"]).Length > 0 && DataConvert.ToString(row["funDepartID"]) != currentFunDepartId)
                {
                    currentFunDepartIdNode = CreateChildNode(currentDistrictIdNode, row["funDepartID"].ToString(), row["funDepartName"].ToString(), row["DistrictID"].ToString(), "funDepart");

                    currentFunDepartId = row["funDepartID"].ToString();
                }

                if (row["ProvinceID"].ToString() != currentProvinceId)
                {
                    currentProvinceIdNode = CreateChildNode(currentDistrictIdNode, row["ProvinceID"].ToString(), row["ProvinceName"].ToString(), row["DistrictID"].ToString(), "Province");

                    currentProvinceId = row["ProvinceID"].ToString();
                }

                if (row["CityID"].ToString() != currentCityId)
                {
                    currentCityIdNode = CreateChildNode(currentProvinceIdNode, row["CityID"].ToString(), row["CityName"].ToString(), row["ProvinceID"].ToString(), "City");

                    currentCityId = row["CityID"].ToString();
                }

                if (row["ExpressCompanyID"].ToString() != currentExpressCompanyId)
                {
                    currentExpressCompanyIdNode = CreateChildNode(currentCityIdNode, row["ExpressCompanyID"].ToString(), row["CompanyName"].ToString(), row["CityID"].ToString(), "Station");

                    currentExpressCompanyId = row["ExpressCompanyID"].ToString();
                }
            });

            IDictionary<int, int> dicIDS = new Dictionary<int, int>();

            LoadDeliverCenter(rootNode, dicIDS);
            LoadPortageCenter(rootNode, dicIDS);
            LoadFunDepartment(rootNode, dicIDS);

            userTree.Roots.Add(rootNode);

            return userTree;
        }

        private void LoadFunDepartment(UserTreeNode<string> pNode, IDictionary<int, int> dicIDS)
        {
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();

            DataTable table = expressCompanyService.GetExpressCompanyByType(0);

            if (table == null) return;

            IDictionary<int, int> dicFlag = new Dictionary<int, int>();

            dicFlag.Add(0, 0);

            LoadOtherNode(table, CreateChildNode(pNode, "-1000", "职能部门", "0", "Department"), 0, -1000, dicFlag, dicIDS);
        }

        private void LoadDeliverCenter(UserTreeNode<string> pNode, IDictionary<int, int> dicIDS)
        {
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();

            DataTable table = expressCompanyService.GetExpressCompanyByType(1, 7);

            if (table == null) return;

            IDictionary<int, int> dicFlag = new Dictionary<int, int>();

            dicFlag.Add(1, 1);
            dicFlag.Add(7, 7);

            LoadOtherNode(table, CreateChildNode(pNode, "-2000", "分拣中心", "0", "Department"), 11, -2000, dicFlag, dicIDS);
        }

        private void LoadPortageCenter(UserTreeNode<string> pNode, IDictionary<int, int> dicIDS)
        {
            var expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();

            DataTable table = expressCompanyService.GetExpressCompanyByType(4, 8);

            if (table == null) return;

            IDictionary<int, int> dicFlag = new Dictionary<int, int>();

            dicFlag.Add(4, 4);
            dicFlag.Add(8, 8);

            LoadOtherNode(table, CreateChildNode(pNode, "-3000", "运输中心", "0", "Department"), 11, -3000, dicFlag, dicIDS);
        }

        private UserTreeNode<string> CreateChildNode(UserTreeNode<string> pNode, string id, string name, string pid, string tag)
        {
            UserTreeNode<string> tempNode = new UserTreeNode<string>()
            {
                ID = id,
                Name = name,
                PID = pid,
                Tag = tag
            };

            pNode.Childs.Add(tempNode);

            return tempNode;
        }

        /// <summary>
        /// 加载职能部门
        /// </summary>
        /// <param name="dataPermissionNode"></param>
        private void LoadOtherNode(DataTable table, UserTreeNode<string> pNode, int pid, int curPID, IDictionary<int,int> dicFlag, IDictionary<int, int> dicIDS)
        {
            DataRow dataRow = null;

            int tempPID = -1;
            int tempID = -1;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                dataRow = table.Rows[i];

                if (dicFlag.ContainsKey(DataConvert.ToInt(dataRow["flag"])) == true)
                {
                    tempPID = DataConvert.ToInt(dataRow["pid"], -1);

                    if (tempPID == pid)
                    {
                        tempID = DataConvert.ToInt(dataRow["id"]);

                        if (dicIDS.ContainsKey(tempID)) continue;

                        dicIDS.Add(tempID, tempID);

                        LoadOtherNode(table, CreateChildNode(pNode, tempID.ToString(), dataRow["name"].ToString(), curPID.ToString(), "Department"), tempID, tempID, dicFlag, dicIDS);
                    }
                }
            }
        }

        public string GetUserRoles(int userId)
        {
            DataTable table = _userDao.GetUserRoles(userId);

            DataRow row = null;

            StringBuilder builder = new StringBuilder();

            int id;
            string name;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToInt(row["id"]);
                name = DataConvert.ToString(row["name"]);

                builder.Append(id);

                if (i != table.Rows.Count - 1)
                {
                    builder.Append(",");
                }
            }

            return builder.ToString();
        }

        public DataSet GetSampList(Employee model)
        {
            return _userDao.GetSampList(model);
        }

        public IDictionary<int, string> GetUserRoles(string userCode)
        {
            return _userDao.GetUserRoles(userCode);
        }

        public DataSet GetSampList(string employCode)
        {
            return _userDao.GetSampList(employCode);
        }
	}
}
