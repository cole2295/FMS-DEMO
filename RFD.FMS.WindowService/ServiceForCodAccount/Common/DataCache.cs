using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Caching;
using ServiceForCodAccount.Model;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace ServiceForCodAccount.Common
{
	/// <summary>
	/// 数据缓存
	/// </summary>
	public class DataCache
	{
		/// <summary>
		/// 获取可用线路价格 缓存10分钟
		/// </summary>
		/// <param name="_taskModel"></param>
		/// <returns></returns>
        public static DataTable GetCodLineTable(int expressCompanyId, int areaType, string distributionCode)
		{
            string cacheKey = string.Format("__CODLINE_CACHE_{0}_{1}_{2}", expressCompanyId, areaType, distributionCode);
			DataTable result = PageCache == null ? null : PageCache.Get(cacheKey) as DataTable;
            if (result != null && result.Rows.Count > 0)
				return result;

			try
			{
                IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();
                result = deliveryPriceService.GetCodLine(expressCompanyId, areaType, distributionCode);

				PageCache.Add(cacheKey, result, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
			}
			catch (Exception ex)
			{
				throw new Exception(cacheKey + "异常:" + ex.Message, ex);
			}
			return result;
		}

		/// <summary>
		/// 获取可用的价格历史 12小时
		/// </summary>
		/// <returns></returns>
		public static DataTable GetCodLineHistory(int year, int month, int expressCompanyId, int areaType, string distributionCode)
		{
            string cacheKey = string.Format("__CODLINEHISTORY_CACHE_{0}_{1}_{2}_{3}_{4}", year, month, expressCompanyId, areaType, distributionCode);
			DataTable result = PageCache == null ? null : PageCache.Get(cacheKey) as DataTable;
            if (result != null && result.Rows.Count>0)
				return result;

			try
			{
                IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();
                result = deliveryPriceService.GetCodLineHistory(year, month, expressCompanyId, areaType, distributionCode);

				PageCache.Add(cacheKey, result, null, DateTime.Now.AddHours(12), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
			}
			catch (Exception ex)
			{
				throw new Exception(cacheKey + "异常" + ex.Message, ex);
			}
			return result;
		}

		#region 配送商
		public static DataTable GetDistribution(string distributionCode)
		{
			string cacheKey = string.Format("_EXPRESSCOMPANYDISTRIBUTION_CACHE_{0}", distributionCode);
			DataTable result = PageCache == null ? null : PageCache.Get(cacheKey) as DataTable;
            if (result != null && result.Rows.Count > 0)
				return result;

			try
			{
                IExpressCompanyService expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
                result = expressCompanyService.GetDistribution(distributionCode);

				PageCache.Add(cacheKey, result, null, DateTime.Now.AddHours(12), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
			}
			catch (Exception ex)
			{
				throw new Exception(cacheKey + "异常" + ex.Message, ex);
			}
			return result;
		}
		#endregion

		#region 区域类型
		public static DataTable GetAreaType(int expressComapnyId, int merchantId)
		{
			string cacheKey = string.Format("_AREATYPE_CACHE_{0}_{1}", expressComapnyId, merchantId);
			DataTable result = PageCache == null ? null : PageCache.Get(cacheKey) as DataTable;
            if (result != null && result.Rows.Count > 0)
				return result;

			try
			{
                IAreaExpressLevelService areaExpressLevelService = ServiceLocator.GetService<IAreaExpressLevelService>();
                result = areaExpressLevelService.GetAreaType(expressComapnyId, merchantId);

				PageCache.Add(cacheKey, result, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
			}
			catch (Exception ex)
			{
				throw new Exception(cacheKey + "异常" + ex.Message, ex);
			}
			return result;
		}

		#endregion

        #region 如风达站点
        public static DataTable GetRFDSite()
        {
            string cacheKey = "__RFD_SITE";
            DataTable result = PageCache == null ? null : PageCache.Get(cacheKey) as DataTable;
            if (result != null && result.Rows.Count > 0)
                return result;

            try
            {
                IExpressCompanyService expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
                result = expressCompanyService.GetRFDSiteList("rfd").Tables[0];

                PageCache.Add(cacheKey, result, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            catch (Exception ex)
            {
                throw new Exception(cacheKey + "异常" + ex.Message, ex);
            }
            return result;
        }
        #endregion

        private static System.Web.Caching.Cache PageCache
		{
			get
			{
				return System.Web.HttpRuntime.Cache;
			}
		}
	}
}
