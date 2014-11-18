using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.QueryStatistics;
using System.Configuration;
using RFD.FMS.MODEL;
using RFD.FMS.Domain.QueryStatistics;
using System.Data;
using RFD.FMS.MODEL.QueryStatistics;
using RFD.FMS.Util;

namespace RFD.FMS.ServiceImpl.QueryStatistics
{
    public class CODWaybillFeeCheckService : ICODWaybillFeeCheckService
    {
        private ICODWaybillFeeCheckDao Dao;

        public IDictionary<int, FMSCodDaily> GetCODDeliveryDaily(string startDate, string stopDate)
        {
            DataTable table = Dao.GetCODDeliveryDaily(startDate,stopDate);

            IDictionary<int, FMSCodDaily> dicValues = new Dictionary<int, FMSCodDaily>();

            DataRow row = null;

            int id = -1;
            string name = "";
            decimal fee = 0;

            FMSCodDaily model = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToInt(row["ID"]);
                name = DataConvert.ToString(row["Name"]);
                fee = DataConvert.ToDecimal(row["Fee"]);

                model = new FMSCodDaily();

                model.CompanyId = id;
                model.CompanyName = name;
                model.AllFee = fee;
                model.IsChecked = false;
                dicValues.Add(id, model);
            }

            return dicValues;
        }

        public IDictionary<int, FMSCODDetails> GetCODDeliveryDailyByStationID(string startDate, string stopDate, int StationID)
        {
            DataTable table =Dao.GetCODDeliveryDailyByStationID(startDate,stopDate,StationID);
            IDictionary<int, FMSCODDetails> dicValues = new Dictionary<int, FMSCODDetails>();

            DataRow row = null;

            int id = -1;
            string name = "";
            decimal fee = 0;
            Int64 waybillno = -1;
            FMSCODDetails model = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                waybillno= DataConvert.ToLong(row["WaybillNO"]);
                id = DataConvert.ToInt(row["ID"]);
                name = DataConvert.ToString(row["Name"]);
                fee = DataConvert.ToDecimal(row["Fee"]);

                model = new FMSCODDetails();
                model.WaybillNo = waybillno;
                model.CompanyId = id;
                model.CompanyName = name;
                model.Fee = fee;
                dicValues.Add(i, model);
            }

            return dicValues;
        }
        public IDictionary<int, FMSCodDaily> GetCODReturnsDaily(string startDate, string stopDate)
        {
            DataTable table = Dao.GetCODReturnsDaily(startDate, stopDate);

            IDictionary<int, FMSCodDaily> dicValues = new Dictionary<int, FMSCodDaily>();

            DataRow row = null;

            int id = -1;
            string name = "";
            decimal fee = 0;

            FMSCodDaily model = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToInt(row["ID"]);
                name = DataConvert.ToString(row["Name"]);
                fee = DataConvert.ToDecimal(row["Fee"]);

                model = new FMSCodDaily();

                model.CompanyId = id;
                model.CompanyName = name;
                model.AllFee = fee;
                model.IsChecked = false;

                dicValues.Add(id, model);
            }

            return dicValues;
        }
        public IDictionary<int, FMSCODDetails> GetCODReturnsDailyByStationID(string startDate, string stopDate, int StationID)
        {
            DataTable table = Dao.GetCODReturnsDailyByStationID(startDate, stopDate,StationID);
            IDictionary<int,FMSCODDetails> dicValues =  new Dictionary<int, FMSCODDetails>();
            DataRow row = null;

            int id = -1;
            string name = "";
            decimal fee = 0;
            Int64 waybillno = -1;
            FMSCODDetails model = null;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                waybillno = DataConvert.ToLong(row["WaybillNo"]);
                id = DataConvert.ToInt(row["ID"]);
                name = DataConvert.ToString(row["Name"]);
                fee = DataConvert.ToDecimal(row["Fee"]);

                model = new FMSCODDetails();
                model.WaybillNo = waybillno;
                model.CompanyId = id;
                model.CompanyName = name;
                model.Fee = fee;
                dicValues.Add(i, model);
            }
            return dicValues;
        }

        public IDictionary<int, FMSCodDaily> GetCODVisitDaily(string startDate, string stopDate)
        {
            DataTable table = Dao.GetCODVisitDaily(startDate, stopDate);

            IDictionary<int, FMSCodDaily> dicValues = new Dictionary<int, FMSCodDaily>();

            DataRow row = null;

            int id = -1;
            string name = "";
            decimal fee = 0;

            FMSCodDaily model = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];

                id = DataConvert.ToInt(row["ID"]);
                name = DataConvert.ToString(row["Name"]);
                fee = DataConvert.ToDecimal(row["Fee"]);

                model = new FMSCodDaily();

                model.CompanyId = id;
                model.CompanyName = name;
                model.AllFee = fee;
                model.IsChecked = false;

                dicValues.Add(id, model);
            }

            return dicValues;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="StationID"></param>
        /// <returns></returns>
        public IDictionary<int, FMSCODDetails> GetCODVisitDailyByStationID(string startDate, string stopDate, int StationID)
        {
            DataTable table = Dao.GetCODVisitDailyByStationID(startDate, stopDate, StationID);
            IDictionary<int, FMSCODDetails> dicValues = new Dictionary<int, FMSCODDetails>();
            DataRow row = null;

            int id = -1;
            string name = "";
            decimal fee = 0;
            Int64 waybillno = -1;
            FMSCODDetails model = null;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                waybillno = DataConvert.ToLong(row["WaybillNo"]);
                id = DataConvert.ToInt(row["ID"]);
                name = DataConvert.ToString(row["Name"]);
                fee = DataConvert.ToDecimal(row["Fee"]);

                model = new FMSCODDetails();
                model.WaybillNo = waybillno;
                model.CompanyId = id;
                model.CompanyName = name;
                model.Fee = fee;
                dicValues.Add(i, model);
            }
            return dicValues;
        }


      
    }
}
