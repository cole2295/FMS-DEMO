using System.Collections;

namespace RFD.FMS.Util.Context
{
	public class NeutralContext
	{
		private static readonly Hashtable contexts = new Hashtable();

		/// <summary>
		/// 获取key的值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static object Get(object key)
		{
			return contexts[key];
		}

		/// <summary>
		/// 设置指定key的值为value
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void Put(object key, object value)
		{
			if (contexts.Contains(key))
			{
				contexts[key] = value;
			}
			else
			{
				contexts.Add(key, value);
			}
		}

		/// <summary>
		/// 移除指定key值
		/// </summary>
		/// <param name="key"></param>
		public static void Remove(object key)
		{
			if (contexts.Contains(key))
			{
				contexts.Remove(key);
			}
		}
	}
}