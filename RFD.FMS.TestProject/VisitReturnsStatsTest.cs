using System.Collections.Generic;
using System.Data;
using RFD.FMS.DAL.Oracle.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.ServiceImpl.BasicSetting;
using RFD.FMS.Util;
using ServiceForCodAccount.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WindowsServiceInterface;

namespace RFD.FMS.TestProject
{
    
    
    /// <summary>
    ///这是 VisitReturnsStatsTest 的测试类，旨在
    ///包含所有 VisitReturnsStatsTest 单元测试
    ///</summary>
    [TestClass()]
    public class VisitReturnsStatsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///DealDetail 的测试
        ///</summary>
        [TestMethod()]
        public void DealDetailTest()
        {
            VisitReturnsStats target = new VisitReturnsStats(); // TODO: 初始化为适当的值
            TaskModel taskModel = new TaskModel() { 
                SyncStartTime="2012-09-26"
            }; // TODO: 初始化为适当的值
            target.DealDetail(taskModel);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }
        /// <summary>
        ///DealDetail 的测试
        ///</summary>
        [TestMethod()]
        public void DealTest()
        {
            IList<long> ids = new List<long>();
            WaybillStatusChangeLogDao dao=new WaybillStatusChangeLogDao();
            DataTable table = dao.GetSynWaybillLogs(Count);

            DataRow[] rows = null;

            DataRow row = null;

            WaybillStatusChangeLog model = null;

            foreach (var obServer in Observers)
            {
                rows = table.Select(obServer.GetSqlCondition());

                for (int i = 0; i < rows.Length; i++)
                {
                    row = rows[i];

                    model = dao.DataRowToObject(row);

                    bool flag = obServer.DoAction(model);

                    if (flag == false && obServer.IsFalseToRePush == true)
                    {
                        LogService.RecordFailseLog(model.ID, obServer.Key);
                    }

                    if (ids.Contains(model.ID) == false)
                    {
                        ids.Add(model.ID);
                    }
                }
            }

            long id = -1;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToLong(row["ID"]);

                LogService.UpdateSynStatus(id);
            }

            ReDeal();
        }
        private WaybillStatusChangeLogService LogService =new WaybillStatusChangeLogService() ;
        private IList<IWaybillStatusObserver> Observers;
        private int Count = 200;

        private void ReDeal()
        {
            try
            {
                WaybillStatusChangeLog model = null;

                foreach (var obServer in Observers)
                {
                    DataTable table = LogService.GetFailseLogByClassKey(obServer.Key);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        model = LogService.DataRowToObject(table.Rows[i]);

                        if (obServer.DoAction(model) == true)
                        {
                            LogService.UpdateFailseLog(DataConvert.ToLong(table.Rows[i]["RecordId"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RFD.FMS.ServiceImpl.Mail.Mail mail = new RFD.FMS.ServiceImpl.Mail.Mail();

                mail.SendMailToUser("财务推送失败日志重新推送错误", ex.Message + ex.StackTrace, "gaopengxiang@vancl.cn");
            }
        }

    }
}
