using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RFD.FMS.MODEL.COD
{
    /// <summary>
    /// FMS_CODBaseInfo
    /// </summary>
    [Serializable]
    public partial class FMS_CODBaseInfo
    {
        private Int64 _id;
        /// <summary>
        /// 主键ID
        /// </summary>		
        public Int64 ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private Int64 _mediumid;
        /// <summary>
        /// 中间表ID
        /// </summary>		
        public Int64 MediumID
        {
            get { return _mediumid; }
            set { _mediumid = value; }
        }
        private Int64 _waybillno;
        /// <summary>
        /// 运单号
        /// </summary>		
        public Int64 WaybillNO
        {
            get { return _waybillno; }
            set { _waybillno = value; }
        }
        private Int32 _merchantid;
        /// <summary>
        /// 商家ID
        /// </summary>		
        public Int32 MerchantID
        {
            get { return _merchantid; }
            set { _merchantid = value; }
        }
        private String _waybilltype;
        /// <summary>
        /// 0普通1上门换2上门退
        /// </summary>		
        public String WaybillType
        {
            get { return _waybilltype; }
            set { _waybilltype = value; }
        }
        private Int32 _flag;
        /// <summary>
        /// 冲红标志位0/1
        /// </summary>		
        public Int32 Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }
        private Int32 _deliverstationid;
        /// <summary>
        /// 配送站ID
        /// </summary>		
        public Int32 DeliverStationID
        {
            get { return _deliverstationid; }
            set { _deliverstationid = value; }
        }
        private Int32 _topcodcompanyid;
        /// <summary>
        /// 结算单位
        /// </summary>		
        public Int32 TopCODCompanyID
        {
            get { return _topcodcompanyid; }
            set { _topcodcompanyid = value; }
        }
        private String _warehouseid;
        /// <summary>
        /// 初始发货仓库
        /// </summary>		
        public String WarehouseId
        {
            get { return _warehouseid; }
            set { _warehouseid = value; }
        }
        private Int32? _expresscompanyid;
        /// <summary>
        /// 初始发货分拣中心
        /// </summary>		
        public Int32? ExpressCompanyID
        {
            get { return _expresscompanyid; }
            set { _expresscompanyid = value; }
        }
        private System.DateTime _rfdaccepttime;
        /// <summary>
        /// 接单时间
        /// </summary>		
        public System.DateTime RfdAcceptTime
        {
            get { return _rfdaccepttime; }
            set { _rfdaccepttime = value; }
        }
        private System.DateTime _rfdacceptdate;
        /// <summary>
        /// RfdAcceptDate
        /// </summary>		
        public System.DateTime RfdAcceptDate
        {
            get { return _rfdacceptdate; }
            set { _rfdacceptdate = value; }
        }
        private Int32? _finalexpresscompanyid;
        /// <summary>
        /// 末级发货分拣中心
        /// </summary>		
        public Int32? FinalExpressCompanyID
        {
            get { return _finalexpresscompanyid; }
            set { _finalexpresscompanyid = value; }
        }
        private System.DateTime? _delivertime;
        /// <summary>
        /// 末级发货时间
        /// </summary>		
        public System.DateTime? DeliverTime
        {
            get { return _delivertime; }
            set { _delivertime = value; }
        }
        private System.DateTime? _deliverdate;
        /// <summary>
        /// DeliverDate
        /// </summary>		
        public DateTime? DeliverDate
        {
            get { return _deliverdate; }
            set { _deliverdate = value; }
        }
        private String _returnwarehouseid;
        /// <summary>
        /// 返货仓库
        /// </summary>		
        public String ReturnWareHouseID
        {
            get { return _returnwarehouseid; }
            set { _returnwarehouseid = value; }
        }
        private Int32? _returnexpresscompanyid;
        /// <summary>
        /// 返货分拣中心
        /// </summary>		
        public Int32? ReturnExpressCompanyID
        {
            get { return _returnexpresscompanyid; }
            set { _returnexpresscompanyid = value; }
        }
        private System.Decimal _totalamount;
        /// <summary>
        /// 总价
        /// </summary>		
        public System.Decimal TotalAmount
        {
            get { return _totalamount; }
            set { _totalamount = value; }
        }
        private System.Decimal _paidamount;
        /// <summary>
        /// 已收
        /// </summary>		
        public System.Decimal PaidAmount
        {
            get { return _paidamount; }
            set { _paidamount = value; }
        }
        private System.Decimal _needpayamount;
        /// <summary>
        /// 应收
        /// </summary>		
        public System.Decimal NeedPayAmount
        {
            get { return _needpayamount; }
            set { _needpayamount = value; }
        }
        private System.Decimal _backamount;
        /// <summary>
        /// 已退
        /// </summary>		
        public System.Decimal BackAmount
        {
            get { return _backamount; }
            set { _backamount = value; }
        }
        private System.Decimal _needbackamount;
        /// <summary>
        /// 应退
        /// </summary>		
        public System.Decimal NeedBackAmount
        {
            get { return _needbackamount; }
            set { _needbackamount = value; }
        }
        private System.Decimal _accountweight;
        /// <summary>
        /// 结算重量
        /// </summary>		
        public System.Decimal AccountWeight
        {
            get { return _accountweight; }
            set { _accountweight = value; }
        }
        private String _areaid;
        /// <summary>
        /// 接收区域ID
        /// </summary>		
        public String AreaID
        {
            get { return _areaid; }
            set { _areaid = value; }
        }
        private Int32? _areatype;
        /// <summary>
        /// 支出区域类型(分配区域)
        /// </summary>		
        public Int32? AreaType
        {
            get { return _areatype; }
            set { _areatype = value; }
        }
        private String _boxsno;
        /// <summary>
        /// 包装箱型号
        /// </summary>		
        public String BoxsNo
        {
            get { return _boxsno; }
            set { _boxsno = value; }
        }
        private String _address;
        /// <summary>
        /// 收货地址
        /// </summary>		
        public String Address
        {
            get { return _address; }
            set { _address = value; }
        }
        private System.DateTime _createtime;
        /// <summary>
        /// 记录创建时间
        /// </summary>		
        public System.DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        private System.DateTime? _updatetime;
        /// <summary>
        /// 记录更新时间
        /// </summary>		
        public System.DateTime? UpdateTime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
        }
        private Boolean _isdeleted;
        /// <summary>
        /// 停用标志位
        /// </summary>		
        public Boolean IsDeleted
        {
            get { return _isdeleted; }
            set { _isdeleted = value; }
        }
        private System.DateTime? _returntime;
        /// <summary>
        /// ReturnTime
        /// </summary>		
        public System.DateTime? ReturnTime
        {
            get { return _returntime; }
            set { _returntime = value; }
        }
        private System.String _returndate;
        /// <summary>
        /// ReturnDate
        /// </summary>		
        public System.String ReturnDate
        {
            get { return _returndate; }
            set { _returndate = value; }
        }

        private int? _isFare;

        public int? IsFare
        {
            get { return _isFare; }
            set { _isFare = value; }
        }

        private decimal? _fare;

        public decimal? Fare
        {
            get { return _fare; }
            set { _fare = value; }
        }

        private string _fareFormula;

        public string FareFormula
        {
            get { return _fareFormula; }
            set { _fareFormula = value; }
        }

        private int? _operateType;

        public int? OperateType
        {
            get { return _operateType; }
            set { _operateType = value; }
        }

        private decimal _protectedPrice;

        public decimal ProtectedPrice
        {
            get { return _protectedPrice; }
            set { _protectedPrice = value; }
        }

        private string _distributionCode;

        public string DistributionCode
        {
            get { return _distributionCode; }
            set { _distributionCode = value; }
        }

        private string _currentDistributionCode;

        public string CurrentDistributionCode
        {
            get { return _currentDistributionCode; }
            set { _currentDistributionCode = value; }
        }

        private int _comeFrom;

        public int ComeFrom
        {
            get { return _comeFrom; }
            set { _comeFrom = value; }
        }

        private int _isCod;

        public int IsCOD
        {
            get { return _isCod; }
            set { _isCod = value; }
        }

        private int _errortype;
        /// <summary>
        /// 错误类型 0未计算、1已计算、2区域类型未找到、3配送价格未找到
        /// </summary>
        public int ErrorType
        {
            get { return _errortype; }
            set { _errortype = value; }
        }

        private int _wareHouseType;
        /// <summary>
        /// 仓库或分拣
        /// </summary>
        public int WareHouseType
        {
            get { return _wareHouseType; }
            set { _wareHouseType = value; }
        }

        private int _isEchelon;
        /// <summary>
        /// 是否区分仓库
        /// </summary>
        public int IsEchelon
        {
            get { return _isEchelon; }
            set { _isEchelon = value; }
        }
    }

    [Serializable]
    public class CodStatsLogModel
    {
        #region Model
        private long _codstatsid;
        private int _statisticstype;
        private int _expresscompanyid;
        private string _warehouseid;
        private DateTime _statisticsdate;
        private int _issuccess;
        private string _reasons = "";
        private string _ip = "";
        private DateTime _createtime = DateTime.Now;
        private DateTime _updatetime = DateTime.Now;
        private int _warehousetype;
        private int _deliverytype;
        private int _formcount;
        private int _merchantid;

        /// <summary>
        /// 
        /// </summary>
        public long CodStatsID
        {
            set { _codstatsid = value; }
            get { return _codstatsid; }
        }
        /// <summary>
        /// 1、发货，2、拒收，3、上门退货
        /// </summary>
        public int StatisticsType
        {
            set { _statisticstype = value; }
            get { return _statisticstype; }
        }
        /// <summary>
        /// 配送商
        /// </summary>
        public int ExpressCompanyID
        {
            set { _expresscompanyid = value; }
            get { return _expresscompanyid; }
        }
        /// <summary>
        /// 仓库
        /// </summary>
        public string WareHouseID
        {
            set { _warehouseid = value; }
            get { return _warehouseid; }
        }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatisticsDate
        {
            set { _statisticsdate = value; }
            get { return _statisticsdate; }
        }
        /// <summary>
        /// 0、未成功，1、成功
        /// </summary>
        public int IsSuccess
        {
            set { _issuccess = value; }
            get { return _issuccess; }
        }
        /// <summary>
        /// 未成功原因
        /// </summary>
        public string Reasons
        {
            set { _reasons = value; }
            get { return _reasons; }
        }
        /// <summary>
        /// 操作机IP
        /// </summary>
        public string Ip
        {
            set { _ip = value; }
            get { return _ip; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }

        /// <summary>
        /// 仓库类型：1、仓库，2、分拣中心，同时也可判断是否外单
        /// </summary>
        public int WareHouseType
        {
            get { return _warehousetype; }
            set { _warehousetype = value; }
        }

        /// <summary>
        /// 发货类型
        /// </summary>
        public int DeliveryType
        {
            set { _deliverytype = value; }
            get { return _deliverytype; }
        }

        /// <summary>
        /// 订单总数
        /// </summary>
        public int FormCount
        {
            get { return _formcount; }
            set { _formcount = value; }
        }

        public int MerchantID
        {
            get { return _merchantid; }
            set { _merchantid = value; }
        }
        #endregion Model
    }

    [Serializable]
    public class CodStatsModel
    {
        #region Model
        private long _accountid;
        private string _accountno;
        private string _warehouseid;
        private int _expresscompanyid;
        private int _areatype;
        private decimal _weight;
        private DateTime _accountdate;
        private int _formcount;
        private decimal _fare;
        private string _formula;
        private string _createby;
        private DateTime _createtime;
        private string _updateby;
        private DateTime _updatetime;
        private bool _deleteflag;
        private int _deliverytype;
        private int _returnstype;
        private int _warehousetype;
        private int _merchantid;

        /// <summary>
        /// 自动编号
        /// </summary>
        public long AccountID
        {
            set { _accountid = value; }
            get { return _accountid; }
        }
        /// <summary>
        /// 结算单号 TMS_CODAccountDetail中AccountNO
        /// </summary>
        public string AccountNO
        {
            set { _accountno = value; }
            get { return _accountno; }
        }
        /// <summary>
        /// 发货仓库
        /// </summary>
        public string WareHouseID
        {
            set { _warehouseid = value; }
            get { return _warehouseid; }
        }
        /// <summary>
        /// 承运商编号 配送公司
        /// </summary>
        public int ExpressCompanyID
        {
            set { _expresscompanyid = value; }
            get { return _expresscompanyid; }
        }
        /// <summary>
        /// 区域类型
        /// </summary>
        public int AreaType
        {
            set { _areatype = value; }
            get { return _areatype; }
        }
        /// <summary>
        /// 总重量
        /// </summary>
        public decimal Weight
        {
            set { _weight = value; }
            get { return _weight; }
        }
        /// <summary>
        /// 结算时间，具体到天
        /// </summary>
        public DateTime AccountDate
        {
            set { _accountdate = value; }
            get { return _accountdate; }
        }
        /// <summary>
        /// 订单总数量
        /// </summary>
        public int FormCount
        {
            set { _formcount = value; }
            get { return _formcount; }
        }
        /// <summary>
        /// 每天统计的基准运费
        /// </summary>
        public decimal Fare
        {
            set { _fare = value; }
            get { return _fare; }
        }
        /// <summary>
        /// 每天计算的运费的公式
        /// </summary>
        public string Formula
        {
            set { _formula = value; }
            get { return _formula; }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy
        {
            set { _createby = value; }
            get { return _createby; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 更改人
        /// </summary>
        public string UpdateBy
        {
            set { _updateby = value; }
            get { return _updateby; }
        }
        /// <summary>
        /// 更改时间
        /// </summary>
        public DateTime UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }
        /// <summary>
        /// 删除表示，当1是表示删除
        /// </summary>
        public bool DeleteFlag
        {
            set { _deleteflag = value; }
            get { return _deleteflag; }
        }

        /// <summary>
        /// 发货类型
        /// </summary>
        public int DeliveryType
        {
            set { _deliverytype = value; }
            get { return _deliverytype; }
        }

        /// <summary>
        /// 拒收类型
        /// </summary>
        public int ReturnsType
        {
            set { _returnstype = value; }
            get { return _returnstype; }
        }

        /// <summary>
        /// 仓库类型：1、仓库，2、分拣中心，同时也可判断是否外单
        /// </summary>
        public int WareHouseType
        {
            get { return _warehousetype; }
            set { _warehousetype = value; }
        }

        public int MerchantID
        {
            get { return _merchantid; }
            set { _merchantid = value; }
        }
        #endregion Model
    }
}
