using System;
using System.Data;
using System.Text;
using RFD.FMS.DAL.Interface;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;

namespace RFD.FMS.DAL.Page
{
    [Serializable]
    public class QueryPager
    {
        public QueryPager(string sql)
        {
            this.sql = sql;

            PrepareSQL();
            PrepareRecord();
            PreparePageCount();
        }

        public QueryPager(string sql, string preName)
        {
            this.preName = preName;
            this.sql = sql;

            PrepareSQL();
            PrepareRecord();
            PreparePageCount();
        }

        private string sql = "";
        private string searchSql = "";
        private string searchCountSql = "";
        private string preName;
        private int allRecord;
        private int curPage = -1;
        private int perPageCount = 2;
        private int pageCount;

        public int PerPageCount
        {
            get { return perPageCount; }
            set 
            { 
                perPageCount = value;

                PreparePageCount();
            }
        }

        public int AllCount
        {
            get { return allRecord; }
        }

        public int CurPage
        {
            get { return curPage; }
            set 
            {
                if (value >= pageCount || value <= 0)
                {
                    throw new Exception("设置当前页的值非法");
                }

                curPage = value;
            }
        }

        public int PageCount
        {
            get { return pageCount; }
        }

        public bool HasNext()
        {
            if (curPage < pageCount - 1) return true;

            return false;
        }

        public bool HasPre()
        {
            if (curPage > 1) return true;

            return false;
        }

        public bool IsFrist()
        {
            if (curPage == 0) return true;

            return false;
        }

        public bool IsLast()
        {
            if (curPage == pageCount - 1) return true;

            return false;
        }

        public DataTable Next()
        {
            if (HasNext() == false) return null;

            curPage++;

            return DoQuery();
        }

        public DataTable Pre()
        {
            if (HasPre() == false) return null;

            curPage--;

            return DoQuery();
        }

        public DataTable Frist()
        {
            if (IsFrist() == true) return null;

            curPage = 0;

            return DoQuery();
        }

        public DataTable Last()
        {
            if (IsLast() == true) return null;

            curPage = pageCount - 1;

            return DoQuery();
        }

        public DataTable DoQuery()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("with result as(");
            builder.Append(searchSql);
            builder.Append(")");
            builder.Append("select top ");
            builder.Append(perPageCount);
            builder.Append(" * from result where result.rownum > ");
            builder.Append(curPage * perPageCount);

            DataSet dataSet = SqlHelper.ExecuteDataset(DaoBase.Connection, CommandType.Text, builder.ToString());

            if (dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }

            return null;
        }
		public DataTable DoQuery(int pageIndex,int pageSize)
		{
			StringBuilder builder = new StringBuilder();
			perPageCount = pageSize;
			curPage = pageIndex;
			builder.Append("with result as(");
			builder.Append(searchSql);
			builder.Append(")");
			builder.Append("select top ");
			builder.Append(perPageCount);
			builder.Append(" * from result where result.rownum > ");
			builder.Append(curPage * perPageCount);

			DataSet dataSet = SqlHelper.ExecuteDataset(DaoBase.Connection, CommandType.Text, builder.ToString());

			if (dataSet.Tables.Count > 0)
			{
				return dataSet.Tables[0];
			}

			return null;
		}
        private void PrepareRecord()
        {
            object objValue = SqlHelper.ExecuteScalar(DaoBase.ReadOnlyConnection, CommandType.Text, searchCountSql);

            allRecord = DataConvert.ToInt(objValue);
        }

        private void PreparePageCount()
        {
            pageCount = allRecord / perPageCount;

            if (allRecord % perPageCount > 0)
            {
                pageCount++;
            }
        }

        private void PrepareSQL()
        {
            searchSql = sql.Trim().ToLower();

            if (preName != null)
            {
                searchSql = searchSql.Insert(searchSql.IndexOf("from"), " ,ROW_NUMBER() over(order by " + preName + ".CreateTime desc) as rownum ");
            }
            else
            {
                searchSql = searchSql.Insert(searchSql.IndexOf("from"), " ,ROW_NUMBER() over(order by CreateTime desc) as rownum ");
            }
            
            searchCountSql = sql.Trim().ToLower();

            searchCountSql = searchCountSql.Substring(searchCountSql.IndexOf("from"));

            searchCountSql = "select count(*) " + searchCountSql;
        }
    }
}
