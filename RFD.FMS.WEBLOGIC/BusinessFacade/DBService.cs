using System;
using System.ComponentModel;
using System.Data;
using LMS.FMS.Model;
using RFD.FMS.Util;
using RFD.FMS.DAL.Interface;
using RFD.FMS.DAL.Page;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using System.Collections.Generic;

namespace RFD.FMS.WEBLOGIC
{
	[DataObject]
    public class DBService
	{
        public static object ExecuteScalar(string sql)
        {
            return SqlHelper.ExecuteScalar(DaoBase.Connection, CommandType.Text, sql);
        }

        public static bool Add<T, V>(T objValue) where T : BaseDataModel<V>
        {
            BeforeAdd<V>(objValue);

            IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

            V id = dataAccess.Add(objValue);

            objValue.ID = id;

            return true;
        }

        public static T GetModel<T, V>(V ID) where T : BaseDataModel<V>
        {
            IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

            return dataAccess.GetModel(ID);
        }

        public static DataTable GetList<T, V>(string strWhere) where T : BaseDataModel<V>
        {
            IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

            DataSet dataSet = dataAccess.GetList(strWhere);

            if (dataSet.Tables.Count > 0) return dataSet.Tables[0];

            return null;
        }

        public static IList<T> GetObjects<T, V>(string strWhere) where T : BaseDataModel<V>
        {
            IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

            return dataAccess.GetObjects(strWhere);
        }

        public static bool Update<T, V>(T objValue) where T : BaseDataModel<V>
        {
            BeforeUpdate<V>(objValue);

            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            return dataAccess.Update(objValue);
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        public static bool Delete<T, V>(V id) where T : BaseDataModel<V>
        {
            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            try
            {
                return dataAccess.Delete(id);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        public static bool Delete<T, V>(T objValue) where T : BaseDataModel<V>
        {
            BeforeDelete(objValue);

            try
            {
                return Delete<T, V>(objValue.ID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        public static bool DeleteByCondition<T, V>(string condition) where T : BaseDataModel<V>
        {
            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            dataAccess.Delete(condition);

            return true;
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        public static bool Delete<T,V>(string condition) where T:BaseDataModel<V>
        {
            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            return dataAccess.Delete(condition);
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        public static bool DeleteList<T, V>(string IDlist) where T : BaseDataModel<V>
        {
            IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

            return dataAccess.DeleteList(IDlist);
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public static bool DeleteByStatus<T,V>(string condition) where T : BaseDataModel<V>
        {
            try
            {
                string typeName = typeof(T).Name;

                IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

                string sql = "update " + typeName + " set IsDelete = 1 where " + condition;

                SqlHelper.ExecuteNonQuery(RFD.FMS.AdoNet.DbBase.DaoBase.Connection, CommandType.Text, sql);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public static bool DeleteByStatus<T,V>(V id) where T : BaseDataModel<V>
        {
            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            T value = GetModel<T, V>(id);

            return DeleteByStatus<T,V>(value);
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public static bool DeleteByStatus<T,V>(T objValue) where T : BaseDataModel<V>
        {
            BeforeDelete<V>(objValue);

            return Update<T, V>(objValue);
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public static bool DeleteListByStatus<T, V>(string IDlist) where T : BaseDataModel<V>
        {
            try
            {
                string typeName = typeof(T).Name;

                string sql = "update " + typeName + " set IsDelete = 1 where ID in(" + IDlist + ")";

                SqlHelper.ExecuteNonQuery(RFD.FMS.AdoNet.DbBase.DaoBase.Connection, CommandType.Text, sql);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static QueryPager QueryByPager<T,V>(string condition)
        {
            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            IPager queryPager = dataAccess as IPager;

            QueryPager pager = new QueryPager(queryPager.GetQueryString(1,condition));

            return pager;
        }

        public static QueryPager QueryByPager<T, V>(string condition, string preName)
        {
            IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

            IPager queryPager = dataAccess as IPager;

            QueryPager pager = new QueryPager(queryPager.GetQueryString(1, condition), preName);

            return pager;
        }

        public static QueryPager QueryByPager<T,V>(string condition,int sqlType)
        {
            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            IPager queryPager = dataAccess as IPager;

            QueryPager pager = new QueryPager(queryPager.GetQueryString(sqlType, condition));

            return pager;
        }

        public static QueryPager QueryByPager<T, V>(string condition, int sqlType, string preName)
        {
            IDataAccess<T, V> dataAccess = ServiceLocator.GetObject<IDataAccess<T, V>>(typeof(T).Name);

            IPager queryPager = dataAccess as IPager;

            QueryPager pager = new QueryPager(queryPager.GetQueryString(sqlType, condition), preName);

            return pager;
        }

        public static DataTable GetAll<T,V>()
        {
            IDataAccess<T,V> dataAccess = ServiceLocator.GetObject<IDataAccess<T,V>>(typeof(T).Name);

            string where = "1=1";

            DataSet dataSet = dataAccess.GetList(where);

            if (dataSet.Tables.Count > 0) return dataSet.Tables[0];

            return null;
        }

        private static bool BeforeAdd<V>(BaseDataModel<V> dataModel)
        {
            dataModel.CreateDeptId = LoginUser.ExpressId;
            dataModel.UpdateDeptId = LoginUser.ExpressId;
            dataModel.CreateUser = LoginUser.Userid;
            dataModel.UpdateUser = LoginUser.Userid;
            dataModel.CreateTime = DateTime.Now;
            dataModel.UpdateTime = DateTime.Now;

            return true;
        }

        private static bool BeforeDelete<V>(BaseDataModel<V> dataModel)
        {
            dataModel.UpdateDeptId = LoginUser.ExpressId;
            dataModel.UpdateUser = LoginUser.Userid;
            dataModel.UpdateTime = DateTime.Now;
            dataModel.IsDelete = true;

            return true;
        }

        private static bool BeforeUpdate<V>(BaseDataModel<V> dataModel)
        {
            dataModel.UpdateDeptId = LoginUser.ExpressId;
            dataModel.UpdateUser = LoginUser.Userid;
            dataModel.UpdateTime = DateTime.Now;

            return true;
        }
	}
}
