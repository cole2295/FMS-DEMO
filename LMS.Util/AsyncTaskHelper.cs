using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using iTextSharp.text;

namespace RFD.FMS.Util
{
    public class AsyncTaskHelper<T>
    {
        private readonly TaskFactory _taskFactory = new TaskFactory();
        public CancellationTokenSource Cancellation = new CancellationTokenSource();
        public Dictionary<string, Task<T>> Tasks = new Dictionary<string, Task<T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        public void Add(string name, Func<T> func)
        {
            if (Tasks.ContainsKey(name) == false)
            {
                var task = _taskFactory.StartNew<T>(func);
                Tasks.Add(name, task);
            }
            else
            {
                throw new ArgumentException("已存在此标识任务！");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, T> RunAll()
        {
            var tasks = Tasks.Values.ToList();
            var results = new Dictionary<string, T>();
            //var task = _taskFactory.ContinueWhenAll<DataTable>(tasks, TasksEnded, CancellationToken.None);
            try
            {
                Task.WaitAll(tasks.ConvertAll(t => t as Task).ToArray());
                Tasks.ToList().ForEach(t => results.Add(t.Key, t.Value.Result));
            }
            catch (AggregateException e)
            {
                var exceptionStr = new StringBuilder();
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    exceptionStr.AppendFormat("\n-------------------------------------------------\n{0}", e.InnerExceptions[j]);
                }
                throw new Exception(exceptionStr.ToString());
            }
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object RunAllAndMerge()
        {
            var dataList = RunAll();
            if (typeof(T) == typeof(DataTable))
            {
                DataTable dt = null;
                dataList.ToList().ForEach(data =>
                    {
                        var tempDt = data.Value as DataTable;
                        if (tempDt != null)
                            if (dt == null)
                            {
                                dt = tempDt;
                            }
                            else
                            {
                                foreach (DataRow dr in tempDt.Rows)
                                {
                                    dt.ImportRow(dr);
                                }
                                dt.AcceptChanges();
                            }
                    });
                return dt;
            }
            else if (typeof(IList).IsAssignableFrom(typeof(T)))
            {
                IList list = new ArrayList();
                dataList.ToList().ForEach(data => list.Add(data.Value));
                return list;
            }
            else
            {
                throw new Exception("非集合对象不可合并数据！");
            }
        }
    }
}
