using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using System.Web;
using System.Text.RegularExpressions;

namespace RFD.FMS.Util
{
    /// <summary>
    /// ���÷���
    /// </summary>
    public class Common
    {
        /// <summary>
        /// �ж�DataSet��������
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public static bool DataSetIsEmpty(DataSet ds)
        {
            return (ds == null) || (ds.Tables.Count < 1) || DataTableIsEmpty(ds.Tables[0]);
        }

        /// <summary>
        /// �ж�DataTable��������
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static bool DataTableIsEmpty(DataTable dt)
        {
            return (dt == null) || (dt.Rows.Count < 1);
        }

        /// <summary>
        /// �ֶ�ֵ����ת��Ϊ�ַ���
        /// </summary>
        /// <param name="arr">�ֶ�ֵ����</param>
        /// <returns>�Զ���ƴ�ӵĲ�ѯ���</returns>
        public static string ValueArrayToQueryString(ArrayList arr)
        {
            if (arr == null || arr.Count == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string obj in arr)
            {
                sb.AppendFormat("'{0}'{1}", obj, (i < arr.Count - 1) ? "," : "");
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// �ֶ�ֵ����ת��Ϊ�ַ���
        /// </summary>
        /// <param name="arr">�ֶ�ֵ����</param>
        /// <param name="whatever">���ݾ�API����ֵ��������</param>
        /// <returns>�Զ���ƴ�ӵĲ�ѯ���</returns>
        public static string ValueArrayToQueryString(IList<string> arr, bool whatever)
        {
            return ValueArrayToQueryString(arr);
        }

        /// <summary>
        /// �ֶ�ֵ����ת��Ϊ�ַ���
        /// </summary>
        /// <param name="arr">�ֶ�ֵ����</param>
        /// <returns>�Զ���ƴ�ӵĲ�ѯ���</returns>
        public static string ValueArrayToQueryString(IList<string> arr)
        {
            if (arr == null || arr.Count == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string obj in arr)
            {
                sb.AppendFormat("'{0}'{1}", obj, (i < arr.Count - 1) ? "," : "");
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// DataTable��һ�в���Ĭ�����ƺ�ֵ(��"��ѡ��...","0")
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="headerValue"></param>
        /// <param name="headerText"></param>
        public static void DataTableAddHeader(DataTable dt, string headerValue, string headerText)
        {
            if (dt != null && dt.Columns.Count > 1)
            {
                DataRow dr = dt.NewRow();
                dr[0] = headerValue;
                dr[1] = headerText;
                dt.Rows.InsertAt(dr, 0);
            }
            else
            {
                throw new Exception("DataTable must be 2 columns");
            }
        }
        /// <summary>
        /// DataTable������һ�м�¼
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="value"></param>
        /// <param name="text"></param>
        public static void DataTableAddRow(DataTable dt, string value, string text)
        {
            if (dt != null && dt.Columns.Count > 1)
            {
                DataRow dr = dt.NewRow();
                dr[0] = value;
                dr[1] = text;
                dt.Rows.Add(dr);
            }
            else
            {
                throw new Exception("DataTable Add rows error.");
            }

        }
        /// <summary>
        /// �ϲ�DataSet�еĶ��DataTable�����ݵ���0��DataTable��Ҫ��DataTable�Ľṹ��ȫ��ͬ
        /// </summary>
        /// <param name="ds"></param>
        public static DataTable MegerDataTableInDataSet(DataSet ds)
        {
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                foreach (DataRow dr in ds.Tables[i].Rows)
                {
                    ds.Tables[0].ImportRow(dr);
                }
            }
            for (int i = 1; i < ds.Tables.Count; i++)
            {
                ds.Tables.RemoveAt(i);
            }
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        public static DataSet LongTimeExecuteDataSet(string connectionString, string commandText, CommandType commandType,
            SqlParameter[] parameters, int timeout)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(commandText, conn))
                {
                    da.SelectCommand.CommandTimeout = timeout;
                    da.SelectCommand.CommandType = commandType;
                    AttachParameters(da.SelectCommand, parameters);
                    da.Fill(ds);
                }
            }
            return ds;
        }

        public static int LongTimeExecuteNonQuery(string connectionString, string commandText, CommandType commandType,
            SqlParameter[] parameters, int timeout)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.CommandTimeout = timeout;
                cmd.CommandType = commandType;
                AttachParameters(cmd, parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public static string DataTableToSql(DataTable table, string insertSql)
        {
            var sb = new StringBuilder();
            foreach (DataRow row in table.Rows)
            {
                var values = new StringBuilder();
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (j > 0) values.Append(", ");
                    if (row.IsNull(j) || table.Columns[j].DataType.Name == "Byte[]")
                        values.Append("NULL");
                    else if (table.Columns[j].DataType == typeof(int) ||
                             table.Columns[j].DataType == typeof(decimal) ||
                             table.Columns[j].DataType == typeof(long) ||
                             table.Columns[j].DataType == typeof(double) ||
                             table.Columns[j].DataType == typeof(float) ||
                             table.Columns[j].DataType == typeof(byte))
                        values.Append(row[j].ToString());
                    else
                        values.AppendFormat("'{0}'",
                            row[j].ToString().Replace("\\", "\\\\").Replace("'", "''"));
                }
                sb.AppendFormat(insertSql, row[0], values);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// �ֶ�ֵ����ת��Ϊ�ַ���
        /// </summary>
        /// <param name="arr">�ֶ�ֵ���� int</param>
        /// <returns>�Զ���ƴ�ӵĲ�ѯ���</returns>
        public static string ValueIntArrayToQueryString(IList<int> arr)
        {
            if (arr == null || arr.Count == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (int obj in arr)
            {
                sb.AppendFormat("{0}{1}", obj, (i < arr.Count - 1) ? "," : "");
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// ��datatableת��Ϊmodellist
        /// ���ô˺����뱣֤һ�¼��㣺
        /// 1.model���ڲ�������model��
        /// 2.model���޲ι��캯��
        /// 3.model����������datatable������һ�£������ִ�Сд��
        /// </summary>
        /// <typeparam name="T">model������</typeparam>
        /// <param name="dt">datatable</param>
        /// <returns></returns>
        public static List<T> ConvertDataTableToModelList<T>(DataTable dt) where T : new()
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> lstReturn = new List<T>();
            DataColumnCollection dcc = dt.Columns;
            foreach (DataRow dr in dt.Rows)
            {
                lstReturn.Add(ConvertDataRowToModel<T>(dr, dcc));
            }
            return lstReturn;
        }

        /// <summary>
        /// ��datarowת��Ϊmodel
        /// ���ô˺����뱣֤һ�¼��㣺
        /// 1.model���ڲ�������model��
        /// 2.model���޲ι��캯��
        /// 3.model����������datatable������һ�£������ִ�Сд��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="dcc"></param>
        /// <returns></returns>
        public static T ConvertDataRowToModel<T>(DataRow dr, DataColumnCollection dcc) where T : new()
        {
            if (dr == null)
            {
                return default(T);
            }
            T t = new T();
            PropertyInfo[] pis = t.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                foreach (DataColumn dc in dcc)
                {
                    if (dc.ColumnName.ToLower() == pi.Name.ToLower())
                    {

                        if (dr[dc] != null && dr[dc] != DBNull.Value)
                        {
                            Type type = pi.PropertyType;
                            if (type.Name.ToLower().Contains("nullable"))
                            {
                                type = Nullable.GetUnderlyingType(type);
                            }
                            pi.SetValue(t, Convert.ChangeType(dr[dc], type), null);
                        }
                        break;
                    }
                }
            }
            return t;
        }

        public static bool IsEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }

        public static bool JudgeDateAreaByDay(DateTime dateS, DateTime dateE)
        {
            TimeSpan day = dateE - dateS;
            return day.TotalDays < 0 ? true : false;
        }
        public static string GetCurrentPage()
        {
            return HttpContext.Current.Request.Url.ToString().Split(new string[] { ".aspx" }, StringSplitOptions.RemoveEmptyEntries).GetValue(0).ToString().Substring(HttpContext.Current.Request.Url.ToString().Split(new string[] { ".aspx" }, StringSplitOptions.RemoveEmptyEntries).GetValue(0).ToString().LastIndexOf('/') + 1)
            + ".aspx";
        }

        /// <summary>
        /// Dictionary ��key����
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <typeparam name="?"></typeparam>
        /// <param name="dic"></param>
        public static void SortDictionary(ref Dictionary<int, string> dic)
        {
            List<int> keys = new List<int>();
            List<string> values = new List<string>();
            int i = 0;
            foreach (int item in dic.Keys)
            {
                keys.Add(item);
                if (!values.Contains(dic[item]))
                    values.Add(dic[item]);
                i++;
            }
            values.Sort(); while (keys.Count != values.Count)
            {
                values.Add(values[values.Count - 1] + 1);

            }
            for (int j = 0; j < i; j++)
            {
                dic[keys[j]] = values[j];
            }

        }

        /// <summary>
        /// �ж������Ƿ���ڣ�ֵ�Ƿ�Ϊnull
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool JudgeColumnContains(DataTable dt, DataRow dr, string columnName)
        {
            if (dt.Columns.Contains(columnName) && dr[columnName] != DBNull.Value)
                return true;

            return false;
        }

        public static bool IsNumeric(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// oracle in �󶨱���
        /// </summary>
        /// <param name="columnName">��ѯ�ֶ�����</param>
        /// <param name="parameterName">�������� ����Ҫ��</param>
        /// <param name="isNotIn">trueΪnot in��falseΪin</param>
        /// <param name="isOr">trueΪor��falseΪand</param>
        /// <returns></returns>
        public static string GetOracleInParameterWhereSql(string columnName,string parameterName,bool isNotIn,bool isOr)
        {
            var inOrNotinStr = isNotIn ? " NOT IN " : " IN ";
            var orOrAndStr = isOr ? " OR " : " AND ";
            //string inWhereSql = " {0} {1} {2} (select regexp_substr(:{3},'[^,]+',1,level) as value_id from dual connect by level<=length(trim(translate(:{3},translate(:{3},',',' '),' ')))+1) ";
            string inWhereSql = @" {0} {1} {2} (
                SELECT SUBSTR(inlist,
                              INSTR(inlist, ',', 1, LEVEL) + 1,
                              INSTR(inlist, ',', 1, LEVEL + 1) -
                              INSTR(inlist, ',', 1, LEVEL) - 1) AS value_str
                  FROM (SELECT ',' ||
                               :{3} || ',' AS inlist
                          FROM DUAL)
                CONNECT BY LEVEL <= LENGTH(:{3}) -
                           LENGTH(REPLACE(','||:{3},
                                                   ',',
                                                   '')) + 1)
                ";
            return string.Format(inWhereSql, orOrAndStr, columnName, inOrNotinStr, parameterName);
        }

        /// <summary>
        /// ������ȡ���� �������룬2��3�ϡ�7��8��
        /// </summary>
        /// <param name="weightrule">ȡ�ع���</param>
        /// <param name="weightType">ȡ������</param>
        /// <param name="weight">ʵ������</param>
        /// <returns></returns>
        public static decimal GetWeightByRule(int weightrule,int weightType, decimal weight)
        {
            if (weightrule == 9) return weight;//ԭ����
            decimal merchantweight = 0.0m;
            if (weightrule == 0)//��������
            {
                merchantweight = Math.Round(weight, 2);
            }
            else if (weightrule == 1)// ����
            {
                decimal newWeight = weight - (Math.Floor(weight));
                if (weight <= 1.000m && weightType==3)//����1.00KG ֻ��ѡ��ȡ������
                {
                    merchantweight = 1.000m;
                }
                if (newWeight >= 0.000m && newWeight < 0.300m)//N.000~N.299 2��
                {
                    merchantweight = (Math.Floor(weight));
                }
                else if (newWeight >= 0.300m && newWeight < 0.800m)//N.300~N.799 3�ϣ�7��
                {
                    merchantweight = (Math.Floor(weight)) + 0.5m;
                }
                else if (newWeight >= 0.800m && newWeight <= 0.9999m)//N.800~N.999 8��
                {
                    merchantweight = (Math.Floor(weight)) + 1;
                }
            }
            return merchantweight;
        }

        public  static string ReadSortingCenterID
        {
           get
           {
               try
               {
                   string CenterID = ConfigurationManager.AppSettings["SortingCenterID"];
                   if (string.IsNullOrEmpty(CenterID))
                   {
                       return string.Empty;
                   }
                   else
                   {
                       return CenterID;
                   }
               }
               catch (Exception)
               {

                   return string.Empty;
               }
               return null;
           }
        }

        public  static  int ReadCount
        {
            get
            {
                try
                {
                    string Count = ConfigurationManager.AppSettings["InSortingCount"];
                    return DataConvert.ToInt(Count);
                }
                catch (Exception)
                {

                    return 50;
                }
            }
        }
    }
}