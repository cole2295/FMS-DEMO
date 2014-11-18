using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace RFD.FMS.DAL.Interface
{
    public interface IDataAccess<T,V>
    {
        bool Exists(V ID);

        /// <summary>
        /// 增加一条数据
        /// </summary>
        V Add(T model);

        /// <summary>
        /// 更新一条数据
        /// </summary>
        bool Update(T model);

        /// <summary>
        /// 删除一条数据
        /// </summary>
        bool Delete(V ID);

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        bool Delete(string condition);

        /// <summary>
        /// 删除一条数据
        /// </summary>
        bool DeleteList(string IDlist);

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        T GetModel(V ID);

        /// <summary>
        /// 获得数据列表
        /// </summary>
        DataSet GetList(string strWhere);

        /// <summary>
        /// 根据查询条件获取对象集合
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <returns>返回的对象集合</returns>
        IList<T> GetObjects(string strWhere);
    }
}
