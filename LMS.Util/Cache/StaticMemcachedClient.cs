using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vancl.LMS.Framework.Caching.Memcached;

namespace RFD.FMS.Util.Cache
{
    public static class StaticMemcachedClient
    {
        public static MemcachedClient SingletonMemcachedClient = new MemcachedClient();

        public static MemcachedClient ReCreateClient()
        {
            SingletonMemcachedClient = new MemcachedClient();
            return SingletonMemcachedClient;
        }
    }
}
