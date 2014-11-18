using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.Trace
{
    public class TrackModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? TrackTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TraceLevel Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }
    }
}
