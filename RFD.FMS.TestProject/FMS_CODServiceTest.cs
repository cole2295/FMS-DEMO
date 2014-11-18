using RFD.FMS.WEBLOGIC.COD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RFD.FMS.Domain.COD;
using RFD.FMS.Service.COD;
using RFD.FMS.Util;

namespace RFD.FMS.TestProject
{
    
    
    /// <summary>
    ///这是 FMS_CODServiceTest 的测试类，旨在
    ///包含所有 FMS_CODServiceTest 单元测试
    ///</summary>
    [TestClass()]
    public class FMS_CODServiceTest
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
        ///LmsSynFmsForShipBySynId 的测试 operatorType小于6的 测试
        ///</summary>
        [TestMethod()]
        public void LmsSynFmsForShipBySynIdTest()
        {
            //IFMS_CODDao dao = null; // TODO: 初始化为适当的值
            IFMS_CODService target = ServiceLocator.GetService<IFMS_CODService>(); // TODO: 初始化为适当的值
            string synId = "32670542,32670533,32670534,32670535,32670536,32670537,32670538,32670539,32670540,32670541,32670554,32670555,32670556,32670553,32670557"; // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            actual = target.LmsSynFmsForShipBySynId(synId);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///SynForDeliverTimeAndOutBountStationForTest 的测试 operatorType=7的测试
        ///</summary>
        [TestMethod()]
        public void SynForDeliverTimeAndOutBountStationForTestTest()
        {
            IFMS_CODDao dao = null; // TODO: 初始化为适当的值
            IFMS_CODService target = ServiceLocator.GetService<IFMS_CODService>(); // TODO: 初始化为适当的值
            string ids = "32670546,32670548,32670550,32670552,32670547,32670545,32670543,32670551,32670549,32670544"; // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            actual = target.SynForDeliverTimeAndOutBountStationForTest(ids);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///LmsSynFmsForBackTest 的测试operatorType=6的测试
        ///</summary>
        [TestMethod()]
        public void LmsSynFmsForBackTestTest()
        {
            IFMS_CODDao dao = null; // TODO: 初始化为适当的值
            IFMS_CODService target = ServiceLocator.GetService<IFMS_CODService>(); // TODO: 初始化为适当的值
            string ids = "32670567,32670560,32670559,32670561,32670562,32670563,32670558,32670564,32670565,32670566"; // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            actual = target.LmsSynFmsForBackTest(ids);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
