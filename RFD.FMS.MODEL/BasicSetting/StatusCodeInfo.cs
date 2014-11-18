using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL.BasicSetting
{
    [Serializable]
    public class StatusCodeInfo
    {
        private string codeType;
        private string codeNo;
        private string codeDesc;
        private bool enable = true;
        private string distributionCode;
        private int creatBy;
        private int updateBy;
        private int orderBy;

        public string CodeType
        {
            get { return codeType; }
            set { codeType = value; }
        }

        public string CodeNo
        {
            get { return codeNo; }
            set { codeNo = value; }
        }

        public string CodeDesc
        {
            get { return codeDesc; }
            set { codeDesc = value; }
        }

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        public string DistributionCode
        {
            get { return distributionCode; }
            set { distributionCode = value; }
        }

        public int CreatBy
        {
            get { return creatBy; }
            set { creatBy = value; }
        }

        public int UpdateBy
        {
            get { return updateBy; }
            set { updateBy = value; }
        }

        public int OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }
    }
}
