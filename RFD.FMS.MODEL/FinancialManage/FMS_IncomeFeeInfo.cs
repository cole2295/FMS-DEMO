using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace RFD.FMS.Model.FinancialManage
{
    /// <summary>
    /// 收入结算配送费明细表
    /// </summary>
    [Serializable]
    public partial class FMS_IncomeFeeInfo
    {
        private Int64 _incomefeeid;
        /// <summary>
        /// 自增列
        /// </summary>		
        public Int64 IncomeFeeID
        {
            get { return _incomefeeid; }
            set { _incomefeeid = value; }
        }
        private Int64 _waybillno;
        /// <summary>
        /// 订单号
        /// </summary>		
        public Int64 WaybillNO
        {
            get { return _waybillno; }
            set { _waybillno = value; }
        }
        private Int32 _isaccount;
        /// <summary>
        /// 计算状态，0未计算、1已计算，其他为错误标识
        /// </summary>		
        public Int32 IsAccount
        {
            get { return _isaccount; }
            set { _isaccount = value; }
        }
        private String _accountstandard;
        /// <summary>
        /// 普发/拒、上门换发/拒、上门退/拒结算标准
        /// </summary>		
        public String AccountStandard
        {
            get { return _accountstandard; }
            set { _accountstandard = value; }
        }
        private System.Decimal? _accountfare;
        /// <summary>
        /// 普发/拒、上门换发/拒、上门退/拒配送费
        /// </summary>		
        public System.Decimal? AccountFare
        {
            get { return _accountfare; }
            set { _accountfare = value; }
        }
        private Int32 _isprotected;
        /// <summary>
        /// 是否计算保价费
        /// </summary>		
        public Int32 IsProtected
        {
            get { return _isprotected; }
            set { _isprotected = value; }
        }
        private System.Decimal? _protectedstandard;
        /// <summary>
        /// 保价费结算标准
        /// </summary>		
        public System.Decimal? ProtectedStandard
        {
            get { return _protectedstandard; }
            set { _protectedstandard = value; }
        }
        private System.Decimal? _protectedfee;
        /// <summary>
        /// 保价费
        /// </summary>		
        public System.Decimal? ProtectedFee
        {
            get { return _protectedfee; }
            set { _protectedfee = value; }
        }
        private Int32 _isreceive;
        /// <summary>
        /// 是否计算代收货款手续费
        /// </summary>		
        public Int32 IsReceive
        {
            get { return _isreceive; }
            set { _isreceive = value; }
        }
        private System.Decimal? _receivestandard;
        /// <summary>
        /// 代收货款现金/POS结算标准
        /// </summary>		
        public System.Decimal? ReceiveStandard
        {
            get { return _receivestandard; }
            set { _receivestandard = value; }
        }
        private System.Decimal? _receivefee;
        /// <summary>
        /// 代收货款现金/POS手续费
        /// </summary>		
        public System.Decimal? ReceiveFee
        {
            get { return _receivefee; }
            set { _receivefee = value; }
        }
        private Int32? _createby;
        /// <summary>
        /// 创建人
        /// </summary>		
        public Int32? CreateBy
        {
            get { return _createby; }
            set { _createby = value; }
        }
        private System.DateTime? _createtime;
        /// <summary>
        /// 创建时间
        /// </summary>		
        public System.DateTime? CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        private Int32? _updateby;
        /// <summary>
        /// 更新人
        /// </summary>		
        public Int32? UpdateBy
        {
            get { return _updateby; }
            set { _updateby = value; }
        }
        private System.DateTime? _updatetime;
        /// <summary>
        /// 更新时间
        /// </summary>		
        public System.DateTime? UpdateTime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
        }
        private System.Byte _isdeleted;
        /// <summary>
        /// 删除标识
        /// </summary>		
        public System.Byte IsDeleted
        {
            get { return _isdeleted; }
            set { _isdeleted = value; }
        }
        private Int32? _transferpaytype;
        /// <summary>
        /// 配送费结算方式（1:现付;2:到付;3:月结）
        /// </summary>		
        public Int32? TransferPayType
        {
            get { return _transferpaytype; }
            set { _transferpaytype = value; }
        }
        private System.Decimal? _deputizeamount;
        /// <summary>
        /// 代收货款
        /// </summary>		
        public System.Decimal? DeputizeAmount
        {
            get { return _deputizeamount; }
            set { _deputizeamount = value; }
        }




        private System.Decimal? _pOSReceiveStandard;
        /// <summary>
        /// 代收货款POS手续费结算标准
        /// </summary>		
        public System.Decimal? POSReceiveStandard
        {
            get { return _pOSReceiveStandard; }
            set { _pOSReceiveStandard = value; }
        }
        private System.Decimal? _pOSReceiveFee;
        /// <summary>
        /// 代收货款POS手续费
        /// </summary>		
        public System.Decimal? POSReceiveFee
        {
            get { return _pOSReceiveFee; }
            set { _pOSReceiveFee = value; }
        }

        private System.Decimal? _cashReceiveServiceStandard;
        /// <summary>
        /// 代收货款现金服务费结算标准
        /// </summary>		
        public System.Decimal? CashReceiveServiceStandard
        {
            get { return _cashReceiveServiceStandard; }
            set { _cashReceiveServiceStandard = value; }
        }
        private System.Decimal? _cashReceiveServiceFee;
        /// <summary>
        /// 代收货款现金服务费
        /// </summary>		
        public System.Decimal? CashReceiveServiceFee
        {
            get { return _cashReceiveServiceFee; }
            set { _cashReceiveServiceFee = value; }
        }

        private System.Decimal? _pOSReceiveServiceStandard;
        /// <summary>
        /// 代收货款POS服务费结算标准
        /// </summary>		
        public System.Decimal? POSReceiveServiceStandard
        {
            get { return _pOSReceiveServiceStandard; }
            set { _pOSReceiveServiceStandard = value; }
        }
        private System.Decimal? _POSReceiveServiceFee;
        /// <summary>
        /// 代收货款POS服务费
        /// </summary>		
        public System.Decimal? POSReceiveServiceFee
        {
            get { return _POSReceiveServiceFee; }
            set { _POSReceiveServiceFee = value; }
        }

        private int _iscod;
        /// <summary>
        ///  是否区分代收
        /// </summary>		
        public System.Int32 ISCod
        {
            get { return _iscod; }
            set { _iscod = value; }
        }

        /// <summary>
        /// 区域类型
        /// </summary>
        [OptionalField]
        public int? AreaType;

        /// <summary>
        /// 计算重量
        /// </summary>
        [OptionalField]
        public decimal? AccountWeight;

        /// <summary>
        /// 是否区分代收
        /// </summary>
        [OptionalField]
        public int IsCod;

        /// <summary>
        /// 是否走配送商逻辑
        /// </summary>
        [OptionalField] 
        public int IsExpress;

        #region 提成相关字段


        private System.Decimal? _expressReceiveBasicDeduct;
        /// <summary>
        /// 快递取件基础提成
        /// </summary>		
        public System.Decimal? ExpressReceiveBasicDeduct
        {
            get { return _expressReceiveBasicDeduct; }
            set { _expressReceiveBasicDeduct = value; }
        }
        private System.Decimal? _expressSendBasicDeduct;
        /// <summary>
        /// 快递派基础提成
        /// </summary>		
        public System.Decimal? ExpressSendBasicDeduct
        {
            get { return _expressSendBasicDeduct; }
            set { _expressSendBasicDeduct = value; }
        }

        private System.Decimal? _expressAreaDeduct;
        /// <summary>
        /// 快递取件基础提成
        /// </summary>		
        public System.Decimal? ExpressAreaDeduct
        {
            get { return _expressAreaDeduct; }
            set { _expressAreaDeduct = value; }
        }
        private System.Decimal? _expressWeightDeduct;
        /// <summary>
        /// 快递派基础提成
        /// </summary>		
        public System.Decimal? ExpressWeightDeduct
        {
            get { return _expressWeightDeduct; }
            set { _expressWeightDeduct = value; }
        }

        private System.Decimal? _programReceiveBasicDeduct;
        /// <summary>
        /// 快递取件基础提成
        /// </summary>		
        public System.Decimal? ProgramReceiveBasicDeduct
        {
            get { return _programReceiveBasicDeduct; }
            set { _programReceiveBasicDeduct = value; }
        }
        private System.Decimal? _programSendBasicDeduct;
        /// <summary>
        /// 快递派基础提成
        /// </summary>		
        public System.Decimal? ProgramSendBasicDeduct
        {
            get { return _programSendBasicDeduct; }
            set { _programSendBasicDeduct = value; }
        }

        private System.Decimal? _programAreaDeduct;
        /// <summary>
        /// 快递取件基础提成
        /// </summary>		
        public System.Decimal? ProgramAreaDeduct
        {
            get { return _programAreaDeduct; }
            set { _programAreaDeduct = value; }
        }
        private System.Decimal? _programWeightDeduct;
        /// <summary>
        /// 快递派基础提成
        /// </summary>		
        public System.Decimal? ProgramWeightDeduct
        {
            get { return _programWeightDeduct; }
            set { _programWeightDeduct = value; }
        }

        private Int32? _isDeductAcount;
        /// <summary>
        /// 快递派基础提成
        /// </summary>		
        public Int32? IsDeductAcount
        {
            get { return _isDeductAcount; }
            set { _isDeductAcount = value; }
        }

        #endregion
    }

    [Serializable]
    public class IncomeFeeCountModel
    {
        Int32 MerchantID { get; set; }

        String MerchantName { get; set; }

        Int32 ExpressCompanyID { get; set; }

        String CompanyName { get; set; }

        String AreaID { get; set; }

        String ProvinceName { get; set; }

        String CityName { get; set; }

        String AreaName { get; set; }
    }
}
