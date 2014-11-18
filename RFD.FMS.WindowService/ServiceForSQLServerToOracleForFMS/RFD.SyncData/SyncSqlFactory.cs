using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RFD.SyncSQL
{
    /// <summary>
    /// 生成同步sql的工厂
    /// </summary>
    public class SyncSqlFactory
    {
        private string GetSql(List<DataRow> drs, SyncTable synTable)
        {
            StringBuilder sbColList = GetColList(synTable);

            string tmpTable = "T" + Guid.NewGuid().ToString().Replace("-", "");//"abcd";

            #region declareTable
            StringBuilder sbDECLARE = new StringBuilder();
            sbDECLARE.AppendLine();
            sbDECLARE.AppendLine(string.Format("DECLARE @{0} table(", tmpTable));
            bool firstLine = true;
            foreach (SyncCol col in synTable.Cols)
            {
                sbDECLARE.AppendLine(string.Format((firstLine ? "" : ",") + "{0} {1}", col.ColName, col.ColType));
                firstLine = false;
            }
            sbDECLARE.AppendLine(")");


            #endregion

            #region sbFillDate
            //insert into @aaa(id,a1)
            //          select 1,'a'
            //union all select 2,'c' ;
            StringBuilder sbFillDate = new StringBuilder();
            sbFillDate.AppendLine(string.Format("insert into @{0}({1})", tmpTable, sbColList.ToString()));

            firstLine = true;
            bool firstCol = true;
            string colValue = "";

            foreach (DataRow dr in drs)
            {
                sbFillDate.Append(firstLine ? "           select " : " union all select ");
                firstLine = false;
                firstCol = true;
                foreach (SyncCol col in synTable.Cols)
                {
                    if (string.IsNullOrEmpty(col.SettingValue))
                    {
                        if (dr[col.ColNameSelect] == DBNull.Value)
                        {
                            colValue = "null";
                        }
                        else
                        {
                            if (dr[col.ColNameSelect].GetType() == typeof(bool))
                            {
                                colValue = (bool)dr[col.ColNameSelect] ? "1" : "0";
                            }
                            else if(dr[col.ColNameSelect].GetType() == typeof(long) ||
                                dr[col.ColNameSelect].GetType() == typeof(int))
                            {
                                colValue = dr[col.ColNameSelect].ToString();
                            }
                            else
                            {
                                colValue = "'" + dr[col.ColNameSelect].ToString().Replace("'", "''") + "'";
                            }
                        }
                    }
                    else
                    {
                        if (col.ColType.ToLower() == "datetime" && col.SettingValue.ToLower() == "getdate()")
                        {
                            colValue = col.SettingValue;
                        }
                        else
                        {
                            colValue = "'" + col.SettingValue + "'";
                        }
                    }

                    sbFillDate.Append((firstCol ? " " : ", ") + colValue);
                    firstCol = false;
                }
                sbFillDate.AppendLine();
            }
            #endregion

            StringBuilder allSql = new StringBuilder();
            if (string.IsNullOrEmpty(synTable.UserDefinedSql))
            {
                #region update
                StringBuilder sbUpdate = new StringBuilder();
                StringBuilder sbUpdateOn = new StringBuilder();
                sbUpdate.AppendLine(" update o set ");

                bool isFirst = true;
                foreach (SyncCol col in synTable.Cols)
                {
                    if (col.OnlyInsert.HasValue && col.OnlyInsert.Value == OnlyINSERT.OnlyInsert)
                    {
                        continue;
                    }

                    if (col.IsKey)
                    {
                        sbUpdateOn.Append((sbUpdateOn.Length < 1 ? "" : " and") + string.Format(" o.{0} = f.{0}", col.ColName));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(col.CaseExp))
                        {
                            sbUpdate.AppendLine((isFirst ? "" : ",") + string.Format(" o.{0} = f.{0}", col.ColName));
                        }
                        else
                        {
                            sbUpdate.AppendLine((isFirst ? "" : ",") + string.Format(" o.{0} = {1}", col.ColName, col.CaseExp));
                        }
                        isFirst = false;
                    }
                }

                if (sbUpdateOn.Length < 1)
                {
                    throw new Exception("请设定主键");
                }

                sbUpdate.AppendLine(string.Format(" from {0} as o join @{1} as f on {2}", synTable.TableName, tmpTable, sbUpdateOn.ToString()));


                firstLine = true;
                if (synTable.SqlWhere != null)
                {
                    foreach (var v in synTable.SqlWhere)
                    {
                        sbUpdate.AppendLine((firstLine ? " where" : " and") + string.Format(" o.{0} {1}", v.Key, v.Value));
                        firstLine = false;
                    }
                }

                #endregion

                //insert into VABC(id,a1)
                //select id,a1 from @aaa f
                //where not exists(select 1 from VABC as t where t.id = f.id );

                #region insert
                StringBuilder sbInsert = new StringBuilder();
                sbInsert.AppendLine(string.Format("insert into {0}({1})", synTable.TableName, sbColList.ToString()));
                sbInsert.AppendLine(string.Format("select {0} from @{1} f", sbColList.ToString(), tmpTable));
                sbInsert.AppendLine(string.Format("where not exists(select 1 from {0} as o where {1})", synTable.TableName, sbUpdateOn.ToString()));

                #endregion

                //return sbDECLARE.ToString() + "\r\n" + sbFillDate.ToString() + "\r\n" + sbUpdate.ToString() + "\r\n" + sbInsert.ToString();
                allSql.AppendLine(sbDECLARE.ToString());
                allSql.AppendLine(sbFillDate.ToString());
                if (synTable.HowTableUpdate == HowTableUpdate.TableOnlyUpdate ||
                    synTable.HowTableUpdate == HowTableUpdate.TableInsertAndUpdate)
                {
                    allSql.AppendLine(sbUpdate.ToString());
                }
                if (synTable.HowTableUpdate == HowTableUpdate.TableOnlyInsert ||
                    synTable.HowTableUpdate == HowTableUpdate.TableInsertAndUpdate)
                {
                    allSql.AppendLine(sbInsert.ToString());
                }
            }
            else
            {
                string userSql = string.Format(synTable.UserDefinedSql, "@" + tmpTable);

                //return sbDECLARE.ToString() + "\r\n" + sbFillDate.ToString() + "\r\n" + userSql;
                allSql.AppendLine(sbDECLARE.ToString());
                allSql.AppendLine(sbFillDate.ToString());
                allSql.AppendLine(userSql.ToString());
            }

            if (string.IsNullOrEmpty(synTable.UserJoinSql))
            {
                return allSql.ToString();
            }
            else
            {
                return allSql.AppendLine(string.Format(synTable.UserJoinSql, "@" + tmpTable)).ToString();
            }
        }

        private string GetOraSql(List<DataRow> drs, SyncTable synTable)
        {
            StringBuilder sbAll = new StringBuilder();

            sbAll.AppendLine(string.Format(@" merge into {0} o 
                                    using (select * from (", synTable.TableName));

            var sbFillDate = new StringBuilder();

            bool firstLine = true;

            foreach (DataRow dr in drs)
            {
                sbFillDate.Append(firstLine ? " select " : " union all select ");

                bool firstCol = true;

                foreach (SyncCol col in synTable.Cols)
                {
                    string colValue = "";

                    if (dr[col.ColNameSelect] == DBNull.Value)
                    {
                        colValue = "null";
                    }
                    else
                    {
                        if (dr[col.ColNameSelect].GetType() == typeof(bool))
                        {
                            colValue = (bool)dr[col.ColNameSelect] ? "1" : "0";
                        }
                        else if (dr[col.ColNameSelect].GetType() == typeof(DateTime))
                        {
                            colValue = String.Format("to_date('{0}','yyyy/mm/dd hh24:mi:ss')", dr[col.ColNameSelect].ToString());
                        }
                        else
                        {
                            colValue = "'" + dr[col.ColNameSelect].ToString().Replace("'", "''") + "'";
                        }
                    }

                    sbFillDate.Append((firstCol ? " " : ", ") + colValue + (firstLine ? " " + col.ColDescName : ""));

                    firstCol = false;
                }

                sbFillDate.Append(" from dual ");
                sbFillDate.AppendLine();
                firstLine = false;
            }

            StringBuilder sbOn = new StringBuilder();

            sbOn.AppendLine(" )) f on (");

            firstLine = true;

            foreach (SyncCol col in synTable.Cols)
            {
                if (col.IsKey)
                {
                    sbOn.AppendLine(string.Format((firstLine ? "" : " and ") + " o.{0} = f.{1} ", col.ColDescName, col.ColDescName));

                    firstLine = false;
                }
            }

            sbOn.AppendLine(")");

            StringBuilder sbUpdate = new StringBuilder();
            sbUpdate.AppendLine(@"when matched then 
                              update set ");

            firstLine = true;

            foreach (SyncCol col in synTable.Cols)
            {
                if (!col.IsKey)
                {
                    sbUpdate.AppendLine((firstLine ? "" : ",") + string.Format(" o.{0} = f.{1}", col.ColDescName, col.ColDescName));

                    firstLine = false;
                }
            }

            StringBuilder sbInsert = new StringBuilder();
            sbInsert.AppendLine("when not matched then");
            StringBuilder sbInsCol = new StringBuilder();
            StringBuilder sbValuesCol = new StringBuilder();

            firstLine = true;

            foreach (SyncCol col in synTable.Cols)
            {
                sbInsCol.Append((firstLine ? "" : ", ") + col.ColDescName);

                sbValuesCol.Append((firstLine ? "" : ", ") + "f." + col.ColDescName);

                firstLine = false;
            }

            sbInsert.AppendLine(string.Format("insert ({0})", sbInsCol));
            sbInsert.AppendLine(string.Format("values ({0})", sbValuesCol));

            sbAll.AppendLine(sbFillDate.ToString());
            sbAll.AppendLine(sbOn.ToString());
            sbAll.AppendLine(sbUpdate.ToString());
            sbAll.AppendLine(sbInsert.ToString());

            return sbAll.ToString();
        }

        public List<KeyValuePair<string, string>> CreateSqlList(string dbtype, SyncTable synTable, List<DataRow> drs, int record)
        {
            if (record < 0)
            {
                throw new Exception("要同步的条数小于0");
            }

            if (drs == null || drs.Count < 1)
            {
                throw new Exception("数据源为空");
            }

            int rowsCount = drs.Count;

            if (record == 0)
            {
                record = rowsCount;
            }

            int totalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(rowsCount) / Convert.ToDouble(record)));

            List<DataRow> dataRowList = drs.ToList();

            List<KeyValuePair<string, string>> sqlList = new List<KeyValuePair<string, string>>();

            for (int i = 1; i <= totalPages; i++)
            {
                List<DataRow> pageSrc = (dataRowList.Skip((i - 1) * record).Take(record)).ToList();

                string keyFormCode = getFormCode(pageSrc, synTable);

                string valueSql = string.Empty;

                if (dbtype.ToUpper() == "MS")
                {
                    valueSql = GetSql(pageSrc, synTable);
                }

                if (dbtype.ToUpper() == "ORA")
                {
                    valueSql = GetOraSql(pageSrc, synTable);
                }

                sqlList.Add(new KeyValuePair<string, string>(keyFormCode, valueSql));
            }

            return sqlList;
        }

        public string getFormCode(List<DataRow> drs, SyncTable syncT)
        {
            StringBuilder sb = new StringBuilder();

            bool rowFirst = true;
            bool colFirst = true;
            foreach (DataRow dr in drs)
            {
                sb.Append(rowFirst ? "[" : ",[");
                rowFirst = false;
                colFirst = true;
                foreach (SyncCol col in syncT.Cols)
                {
                    if (col.IsKey)
                    {
                        sb.Append((colFirst ? "" : ",") + dr[col.ColNameSelect].ToString());
                    }
                }
                sb.Append("]");
            }

            return sb.ToString();
        }

        private StringBuilder GetColList(SyncTable synTable)
        {
            StringBuilder sbColList = new StringBuilder();
            bool firstLine = true;
            foreach (SyncCol col in synTable.Cols)
            {
                sbColList.Append((firstLine ? " " : ", ") + col.ColName);
                firstLine = false;
            }
            return sbColList;
        }
    }
}
