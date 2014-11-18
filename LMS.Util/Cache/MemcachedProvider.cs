using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vancl.LMS.Framework.Caching.Memcached;

namespace RFD.FMS.Util.Cache
{
    public class MemcachedProvider : ICacheProvider
    {
        #region ICacheProvider 成员

        private static MemcachedClient _client = StaticMemcachedClient.SingletonMemcachedClient;
        private const string RfdMemcachedGroupKey = @"RfdMemcachedGroupKey";

        private void ExceptionHandle()
        {
            _client = StaticMemcachedClient.ReCreateClient();
        }

        public bool ContainKey(string argKey)
        {
            return _client.Get(argKey) == null;
        }

        #region GroupMethod
        private List<string> GetGroupKeyList(string groupKey)
        {
            groupKey = string.Format("{0}_{1}", RfdMemcachedGroupKey, groupKey);
            var keyList = _client.Get(groupKey) as List<string>;
            if (keyList == null)
            {
                keyList = new List<string>();
                _client.Set(groupKey, keyList);
            }
            return keyList;
        }

        private bool SetGroupKeyList(string groupKey, string argKey)
        {
            groupKey = string.Format("{0}_{1}", RfdMemcachedGroupKey, groupKey);
            var groupList = GetGroupKeyList(groupKey);
            groupList.Add(argKey);
            return _client.Set(groupKey, groupList);
        }

        private bool ClearGroupKeyList(string groupKey)
        {
            groupKey = string.Format("{0}_{1}", RfdMemcachedGroupKey, groupKey);
            return _client.Remove(groupKey);
        }
        #endregion

        public bool ContainKey(string groupKey, string argKey)
        {
            return GetGroupKeyList(groupKey).Contains(argKey);
        }

        public bool Exist(string argKey)
        {
            var flag = Add(argKey, string.Empty);
            if (flag)
                Remove(argKey);
            return !flag;
        }

        public bool Add(string argKey, object argValue)
        {
            return _client.Add(argKey, argValue);
        }

        public bool Add(string argKey, object argValue, DateTime argDateExpiration)
        {
            return _client.Add(argKey, argValue, argDateExpiration);
        }

        public bool Add<T>(string argKey, T entity) where T : class
        {
            return _client.Add(argKey, entity);
        }

        public bool Add<T>(string argKey, T entity, DateTime argDateExpiration) where T : class
        {
            return _client.Add(argKey, entity, argDateExpiration);
        }

        public bool Set(string argKey, object argValue)
        {
            return _client.Set(argKey, argValue);
        }


        public bool Set(string groupKey, string argKey, object argValue)
        {
            if (!ContainKey(groupKey, argKey))
            {
                SetGroupKeyList(groupKey, argKey);
            }
            return _client.Set(argKey, argValue);
        }

        public bool Set(string argKey, object argValue, DateTime argDateExpiration)
        {
            return _client.Set(argKey, argValue, argDateExpiration);
        }

        public bool Set<T>(string argKey, T entity) where T : class
        {
            return _client.Set(argKey, entity);
        }

        public bool Set<T>(string argKey, T entity, DateTime argDateExpiration) where T : class
        {
            return _client.Set(argKey, entity, argDateExpiration);
        }

        public bool Replace(string argKey, object argValue)
        {
            return _client.Replace(argKey, argValue);
        }

        public bool Replace(string argKey, object argValue, DateTime argDateExpiration)
        {
            return _client.Replace(argKey, argValue, argDateExpiration);
        }

        public bool Replace<T>(string argKey, T entity) where T : class
        {
            return _client.Replace(argKey, entity);
        }

        public bool Replace<T>(string argKey, T entity, DateTime argDateExpiration) where T : class
        {
            return _client.Replace(argKey, entity, argDateExpiration);
        }

        public object Get(string argKey)
        {
            return _client.Get(argKey);
        }

        public T Get<T>(string argKey) where T : class
        {
            try
            {
                var obj = (T)_client.Get(argKey);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Remove(string argKey)
        {
            return _client.Remove(argKey);
        }

        public bool ClearGroup(string groupKey)
        {
            var groupList = GetGroupKeyList(groupKey);
            if (groupList != null)
                groupList.ForEach(item => Remove(item));
            return ClearGroupKeyList(groupKey);
        }

        #endregion

    }
}
