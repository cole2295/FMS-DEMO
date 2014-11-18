using System;
using System.Collections.Generic;

namespace RFD.Sync.Impl.Tool
{
    /*
   * 同步查询参数
   */
    public class SyncSelectParam
    {
        private int _topN = 200;
        private int _record = 5;
        private DateTime? _startTime = null;
        private string _appSettingsKey = string.Empty;
        private readonly string _appDefaultSettingsKey = "default";

        /// <summary>
        /// 配置文件中appSettings的key值
        /// </summary>
        /// <param name="appSettingsKey"></param>
        public SyncSelectParam(string appSettingsKey)
        {
            _appSettingsKey = appSettingsKey;
        }

        /// <summary>
        /// 每次取多少条准备同步,默认200
        /// </summary>
        public int TopN
        {
            get
            {
                if (!int.TryParse(GetValueByConfig(_appSettingsKey + "_TOPN"), out _topN))
                {
                    if (!int.TryParse(GetValueByConfig(_appDefaultSettingsKey + "_TOPN"), out _topN))
                    {
                        _topN = 200;
                    }
                }

                return _topN;
            }
        }

        //public int DefaultRecord
        //{
        //    get
        //    {
        //        if (!int.TryParse(GetValueByConfig("_RECORD"), out _record))
        //        {
        //            if (!int.TryParse(GetValueByConfig(_appDefaultSettingsKey + "_RECORD"), out _record))
        //            {
        //                _record = 5;
        //            }
        //        }
        //        return _record;
        //    }
        //}

        /// <summary>
        /// 每次同步几条，默认5条
        /// </summary>
        public int Record
        {
            get
            {
                if (!int.TryParse(GetValueByConfig(_appSettingsKey + "_RECORD"), out _record))
                {
                    if (!int.TryParse(GetValueByConfig(_appDefaultSettingsKey + "_RECORD"), out _record))
                    {
                        _record = 5;
                    }
                }
                return _record;
            }
        }

        /// <summary>
        /// 取数据的开始时间点
        /// </summary>
        public DateTime? StartTime
        {
            get
            {
                DateTime dt;
                bool dateIsNull = false;
                if (!DateTime.TryParse(GetValueByConfig(_appSettingsKey + "_STARTTIME"), out dt))
                {
                    if (!DateTime.TryParse(GetValueByConfig(_appDefaultSettingsKey + "_STARTTIME"), out dt))
                    {
                        dateIsNull = true;
                    }
                }

                if (dateIsNull)
                {
                    _startTime = null;
                }
                else
                {
                    _startTime = dt;
                }

                return _startTime;
            }
        }

        /// <summary>
        /// Where条件
        /// </summary>
        public string Where
        {
            get
            {
                string where = string.Empty;

                where = GetValueByConfig(_appSettingsKey + "_WHERE");
                if (where == null)
                {
                    where = GetValueByConfig(_appDefaultSettingsKey + "_WHERE");
                }

                return where;
            }
        }

        private string GetValueByConfig(string settingKey)
        {
            string appsetting = string.Empty;
            try
            {
                appsetting = System.Configuration.ConfigurationManager.AppSettings[settingKey];
            }
            catch
            {
            }
            return appsetting;
        }
    }

    public class SyncSelectParamByConfig
    {
        private string _appSettingsKey = string.Empty;
        private Dictionary<string, SyncSelectParam> _list = null;
        private static SyncSelectParamByConfig _instance = null;

        private SyncSelectParamByConfig()
        {
            _list = new Dictionary<string, SyncSelectParam>();
        }

        public static SyncSelectParamByConfig Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new SyncSelectParamByConfig();
                }
                return _instance;
            }
        }

        public SyncSelectParam GetParam(string appSettingsKey)
        {
            if (!_list.ContainsKey(appSettingsKey))
            {
                _list.Add(appSettingsKey, new SyncSelectParam(appSettingsKey));
            }
            return _list[appSettingsKey];
        }
    }
}
