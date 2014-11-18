using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Util;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.Service.BasicSetting;



namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class NoGenerate : INoGenerate 
    {
        private INoGenerateDao _noGenerateDao = ServiceLocator.GetService<INoGenerateDao>();
        /// <summary>
        /// 生成种子号
        /// </summary>
        /// <param name="NoType"></param>
        /// <returns></returns>
        public string GetLastNo(int NoType)
        {
            return _noGenerateDao.GetLastNo(NoType, DateTime.Now.ToString("yyyyMMdd"));
        }
		/// <summary>
		/// 生成种子号
		/// </summary>
		/// <param name="NoType"></param>
		/// <returns></returns>
		public string GetLastNo(int NoType,string fmt)
		{
            return _noGenerateDao.GetLastNo(NoType, DateTime.Now.ToString(fmt));
		}

		/// <summary>
		/// 生成种子号
		/// </summary>
		/// <param name="NoType"></param>
		/// <returns></returns>
		public string GetLastNoWithSeed(int NoType)
		{
            INoGenerateDao _noGenerateDao = new RFD.FMS.DAL.Oracle.BasicSetting.NoGenerateDao();
            return _noGenerateDao.GetLastNo(NoType);
		}

        /// <summary>
        ///生成单据号
        /// </summary>
        /// <param name="noType">单据名</param>
        /// <param name="length">种子长度</param>
        /// <param name="addDate">是否需要日期包含在单据号中</param>
        /// <returns></returns>
        public string GetOrderNo(int noType, int length, bool addDate,string fmt)
        {
            string no = GetLastNo(noType);

            StringBuilder sb = new StringBuilder(no);
            for (int i = 0; i < length - no.Length; i++)
            {
                sb.Insert(0, "0");
            }
            if (addDate)
            {
                sb.Insert(0, DateTime.Today.ToString(fmt));
            }
            return sb.ToString();
        }

		/// <summary>
		///生成单据号
		/// </summary>
		/// <param name="noType">单据名</param>
		/// <param name="length">种子长度</param>
		/// <param name="addDate">是否需要日期包含在单据号中</param>
		/// <returns></returns>
		public string GetOrderNoWithSeed(int noType, int length,string logo)
		{
			string no = GetLastNoWithSeed(noType);

			StringBuilder sb = new StringBuilder(no);
			for (int i = 0; i < length - no.Length; i++)
			{
				sb.Insert(0, "0");
			}
			return logo + sb.ToString();
		}

        /// <summary>
        ///生成单据号
        /// </summary>
        /// <param name="noType">单据名</param>
        /// <param name="length">种子长度</param>
        /// <param name="addDate">是否需要日期包含在单据号中</param>
        /// <returns></returns>
        public string GetOrderNoShortDate(int noType, int length, bool addDate,string fmt)
        {
            string no = GetLastNo(noType);

            StringBuilder sb = new StringBuilder(no);
            for (int i = 0; i < length - no.Length; i++)
            {
                sb.Insert(0, "0");
            }
            if (addDate)
            {
                sb.Insert(0, DateTime.Today.ToString(fmt));
            }
            return sb.ToString();
        }
    }

    public class IDGenerateService : IIDGenerateService
    {
        private INoGenerateDao _noGenerateDao = ServiceLocator.GetService<INoGenerateDao>();

        /// <summary>
        /// ID生成器
        /// </summary>
        /// <param name="dbflag">数据库</param>
        /// <param name="tableflag">表名</param>
        /// <param name="columnflag">列名</param>
        /// <returns></returns>
        public string NewId(string dbflag, string tableflag, string columnflag)
        {
            //DDCCCyymmddXXXXXXXX
            //长度：20
            //DD：LMS数据库编码。比如01表示子库1。
            //CCC：哪个表的的哪个字段。比如001表示InBound表中的InBoundKid字段。
            //yymmdd：日期。比如 2012年10月01日为121001
            //XXXXXXXXX:流水号，如123456789
            INoGenerateDao _noGenerateDao = new RFD.FMS.DAL.Oracle.BasicSetting.NoGenerateDao();
            if (_noGenerateDao == null)
            {
                throw new Exception("获得自增ID失败");
            }

            DateTime? yymmdd = null;
            string ccc;
            string sn = _noGenerateDao.GetLastNo(tableflag, columnflag, out yymmdd, out ccc);
            if (!yymmdd.HasValue)
            {
                throw new Exception("获得自增ID失败");
            }

            string newid = dbflag + ccc + yymmdd.Value.ToString("yyMMdd") + sn.PadLeft(9, '0');
            if (newid.Length != 20)
            {
                throw new Exception(string.Format("生成自增ID:{0}长度不对", newid));
            }
            return newid;
        }

        /// <summary>
        /// ID生成器
        /// </summary>
        /// <param name="tableflag">表名</param>
        /// <param name="columnflag">列名</param>
        /// <returns></returns>
        public string NewId(string tableflag, string columnflag)
        {
            return NewId(CurrentDbCode.DbCode, tableflag, columnflag);
        }

        public class CurrentDbCode
        {
            private const string _currDbCode = "fmsdbcode";

            public static string DbCode
            {
                get
                {
                    return "00";//CookieUtil.ExistCookie(_currDbCode) ? CookieUtil.GetCookie(_currDbCode) : 
                }
                //set
                //{
                //    CookieUtil.AddCookie(_currDbCode, value);
                //}
            }
            public static string ServiceDbCode
            {
                get
                {
                    return "00";
                }
                
            }
        }


        public string ServiceNewId(string tableflag, string columnflag)
        {
            return NewId(CurrentDbCode.ServiceDbCode, tableflag, columnflag);
        }
    }
}
