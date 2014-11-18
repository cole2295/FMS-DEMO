using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
    /*
* ***********************
* 通用省市区域站点报表查询条件类
* Terry
* 2011-09-08
* ***********************
*/
    public class PCSReportcdt :BaseReportCondition
    {
        private string _provinceid;

        private string _cityid;

        private string _areaid;

        private string _stationid;

        /// <summary>
        /// 省份ID
        /// </summary>
        public string ProvinceID
        {
            get
            {
                return _provinceid;
            }
            set
            {
                if (value != null)
                {
                    _provinceid = value.Trim();
                }
            }
        }

        /// <summary>
        /// 城市ID
        /// </summary>
        public string CityID
        {
            get
            {
                return _cityid;
            }
            set
            {
                if (value != null)
                {
                    _cityid = value.Trim();
                }
            }
        }


        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaID
        {
            get
            {
                return  _areaid;
            }
            set
            {
                if (value != null)
                {
                    _areaid = value.Trim();
                }
            }
        }


        /// <summary>
        /// 站点ID
        /// </summary>
        public string StationID
        {
            get
            {
                return _stationid;
            }
            set
            {
                if (value != null)
                {
                    _stationid = value.Trim();
                }
            }
        }

    }




}
