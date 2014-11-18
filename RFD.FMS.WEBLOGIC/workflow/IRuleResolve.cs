using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.WEBLOGIC
{
    public interface IRuleResolve
    {
        /// <summary>
        /// 把字符串转换为sql条件语句
        /// </summary>
        /// <param name="condition">普通字符串</param>
        /// <returns>sql条件语句</returns>
        string DoResolve(string condition);

        /// <summary>
        /// 把Sql条件语句转换成字符串
        /// </summary>
        /// <param name="sqlCondition">sql条件语句</param>
        /// <returns>字符串</returns>
        string UnDoResolve(string sqlCondition);
    }
}
