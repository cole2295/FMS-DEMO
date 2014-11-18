using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.Cache
{
    public interface ICacheProvider
    {
        bool ContainKey(string argKey);

        bool ContainKey(string groupKey, string argKey);

        bool Exist(string argKey);

        bool Add(string argKey, object argValue);

        bool Add(string argKey, object argValue, DateTime argDateExpiration);

        bool Add<T>(string argKey, T entity) where T : class;

        bool Add<T>(string argKey, T entity, DateTime argDateExpiration) where T : class;

        bool Set(string argKey, object argValue);

        bool Set(string groupKey, string argKey, object argValue);

        bool Set(string argKey, object argValue, DateTime argDateExpiration);

        bool Set<T>(string argKey, T entity) where T : class;

        bool Set<T>(string argKey, T entity, DateTime argDateExpiration) where T : class;

        bool Replace(string argKey, object argValue);

        bool Replace(string argKey, object argValue, DateTime argDateExpiration);

        bool Replace<T>(string argKey, T entity) where T : class;

        bool Replace<T>(string argKey, T entity, DateTime argDateExpiration) where T : class;

        object Get(string argKey);

        T Get<T>(string argKey) where T : class;

        bool Remove(string argKey);

        bool ClearGroup(string groupKey);
    }
}
