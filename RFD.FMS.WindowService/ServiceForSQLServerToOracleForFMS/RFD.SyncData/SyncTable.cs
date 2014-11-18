using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RFD.SyncSQL
{
    public class SyncTable
    {
        private List<SyncCol> _cols = new List<SyncCol>();
        private string _tableName = string.Empty;
        private HowTableUpdate _howTableUpdate = HowTableUpdate.TableInsertAndUpdate;
        private string _userDefinedSql = string.Empty;
        private string _userJoinSql = string.Empty;



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableName">要同步表</param>
        /// <param name="tableOnlyUpdate">是否只生成update语句</param>
        public SyncTable(string tableName, HowTableUpdate howTableUpdate)
        {
            _tableName = tableName;
            _howTableUpdate = howTableUpdate;
        }

        /// <summary>
        /// 构造函数,自定义SQL
        /// </summary>
        /// <param name="userDefinedSql"></param>
        public SyncTable(string userDefinedSql)
        {
            _userDefinedSql = userDefinedSql;
            _howTableUpdate = HowTableUpdate.TableInsertAndUpdate;
        }

        public string TableName
        {
            get
            {
                return _tableName;
            }
        }

        public HowTableUpdate HowTableUpdate
        {
            get
            {
                return _howTableUpdate;
            }
        }

        public List<SyncCol> Cols
        {
            get
            {
                return _cols;
            }
            set
            {
                _cols = value;
            }
        }

        public List<KeyValuePair<string, string>> SqlWhere
        {
            get;
            set;
        }

        /// <summary>
        /// 用户自定义SQL
        /// </summary>
        public string UserDefinedSql
        {
            get
            {
                return _userDefinedSql;
            }
        }

        /// <summary>
        /// 在系统生成的基础上拼接SQL
        /// </summary>
        public string UserJoinSql
        {
            get
            {
                return _userJoinSql;
            }
            set
            {
                _userJoinSql = value;
            }
        }
    }

    public class SyncCol
    {
        private string _colName;
        private string _colDescName;
        private string _colNameSelect;
        private string _colType;
        private bool _isKey;
        private string _settingValue;
        private string _caseExp = "";

        private OnlyINSERT? _onlyInsert = null;

        public SyncCol()
        {
        }

        public SyncCol(string colName, string colType)
        {
            _colName = colName;
            _colType = colType;
        }

        public SyncCol(KeyValuePair<string, string> colName, string colType)
        {
            _colName = colName.Key;
            _colNameSelect = colName.Value;
            _colType = colType;
        }

        public SyncCol(KeyValuePair<string, string> colName, string colType, bool isKey)
        {
            _colName = colName.Key;
            _colNameSelect = colName.Value;
            _colType = colType;
            _isKey = isKey;
        }

        public SyncCol(string colName, string colType, bool isKey)
        {
            _colName = colName;
            _colType = colType;
            _isKey = isKey;
        }

        public SyncCol(string colName, string descName, string colType, bool isKey)
        {
            _colName = colName;
            _colDescName = descName;
            _colType = colType;
            _isKey = isKey;
        }

        public SyncCol(string colName, string colType, OnlyINSERT onlyInsert)
        {
            _colName = colName;
            _colType = colType;
            _onlyInsert = onlyInsert;
        }

        public SyncCol(string colName, string colType, string settingValue)
        {
            _colName = colName;
            _colType = colType;
            _settingValue = settingValue;
        }

        public SyncCol(string colName, string colType, bool isKey, string settingValue)
        {
            _colName = colName;
            _colType = colType;
            _isKey = isKey;
            _settingValue = settingValue;
        }

        public SyncCol(string colName, string colType, string cWhen, string cThen, string cElse)
        {
            _colName = colName;
            _colType = colType;
            _caseExp = string.Format(" (case when {0} then {1} else {2} end)", cWhen, cThen, cElse);
        }

        public string ColDescName 
        {
            get 
            {
                if (String.IsNullOrEmpty(_colDescName)) return ColName;

                return _colDescName;
            } 
        }

        public string ColName
        {
            get
            {
                return _colName;
            }
        }

        public string ColNameSelect
        {
            get
            {
                if (string.IsNullOrEmpty(_colNameSelect))
                {
                    return _colName;
                }

                return _colNameSelect;
            }
        }

        public string ColType
        {
            get
            {
                return _colType;
            }
        }

        public bool IsKey
        {
            get
            {
                return _isKey;
            }
            set
            {
                _isKey = value;
            }
        }

        public string SettingValue
        {
            get
            {
                return _settingValue;
            }
            set
            {
                _settingValue = value;
            }
        }

        public string CaseExp
        {
            get
            {
                return _caseExp;
            }
        }

        public OnlyINSERT? OnlyInsert
        {
            get { return _onlyInsert; }
            set { _onlyInsert = value; }
        }
    }

    /// <summary>
    /// 一条对应多条
    /// </summary>
    public class SyncDataSingle
    {
        private DataRow _dataRow = null;
        private List<DataRow> _subDataRows = null;

        public SyncDataSingle(DataRow dataRow, List<DataRow> subDataRows)
        {
            _dataRow = dataRow;
            _subDataRows = subDataRows;
        }

        public DataRow DataRow
        {
            get
            {
                return _dataRow;
            }
        }

        public List<DataRow> SubDataRows
        {
            get
            {
                return _subDataRows;
            }
            set
            {
                _subDataRows = value;
            }
        }
    }

    /// <summary>
    /// 多条对应多条
    /// </summary>
    public class SyncDataMulti
    {
        List<DataRow> _mainRows;
        List<DataRow> _subRows;

        public SyncDataMulti(List<DataRow> mainRows, List<DataRow> subRows)
        {
            _mainRows = mainRows;
            _subRows = subRows;
        }

        public List<DataRow> mainRows
        {
            get
            {
                return _mainRows;
            }
        }

        public List<DataRow> subRows
        {
            get
            {
                return _subRows;
            }
        }
    }

    public class SyncDataManager
    {
        private List<SyncDataSingle> _list = null;
        private DataSet _ds = null;

        public SyncDataManager(DataSet ds)
        {
            if (null == ds || ds.Tables.Count != 2 || ds.Relations.Count != 1)
            {
                throw new Exception("请确认dataset中的table有父子关系");
            }

            _list = new List<SyncDataSingle>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                _list.Add(new SyncDataSingle(dr, dr.GetChildRows(ds.Relations[0]).ToList()));
            }
            _ds = ds;
        }

        public List<SyncDataSingle> GetList
        {
            get
            {
                return _list;
            }
        }

        public SyncDataMulti GetPagingData(int page, int record)
        {
            List<DataRow> _mainRow = new List<DataRow>();
            List<DataRow> _subRow = new List<DataRow>();

            List<SyncDataSingle> pageSrc = (_list.Skip((page - 1) * record).Take(record)).ToList();

            foreach (var p in pageSrc)
            {
                _mainRow.Add(p.DataRow);
                _subRow.AddRange(p.SubDataRows);
            }

            return new SyncDataMulti(_mainRow, _subRow);
        }

        public SyncDataMulti AllData
        {
            get
            {
                return new SyncDataMulti(_ds.Tables[0].Select().ToList(), _ds.Tables[1].Select().ToList());
            }
        }
    }

    public enum OnlyINSERT
    {
        OnlyInsert
    }

    public enum HowTableUpdate
    {
        TableOnlyInsert,
        TableOnlyUpdate,
        TableInsertAndUpdate
    }
}
