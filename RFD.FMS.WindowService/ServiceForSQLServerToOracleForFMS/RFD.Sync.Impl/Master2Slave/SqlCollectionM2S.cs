namespace RFD.Sync.Impl.Master2Slave
{
    public class SqlCollectionM2S
    {
        public static string sqlCodBaseInfo = @"SELECT TOP {1} ID,MediumID,WaybillNO,MerchantID,WaybillType,Flag,DeliverStationID,
                                                    TopCODCompanyID,WarehouseId,ExpressCompanyID,RfdAcceptTime,
                                                    RfdAcceptDate,FinalExpressCompanyID,DeliverTime,DeliverDate,
                                                    ReturnWareHouseID,ReturnExpressCompanyID,TotalAmount,PaidAmount,
                                                    NeedPayAmount,BackAmount,NeedBackAmount,AccountWeight,AreaID,AreaType,
                                                    BoxsNo,Address,CreateTime,UpdateTime,IsDeleted,ReturnTime,ReturnDate,
                                                    IsFare,Fare,FareFormula,OperateType,ProtectedPrice,DistributionCode,
                                                    CurrentDistributionCode,ComeFrom,IsCOD 
                                                from LMS_RFD.dbo.FMS_CODBaseInfo with(nolock,forceseek)
                                                WHERE 1 = 1 {0} ";

        public static string sqlCodBaseInfo_History = @"SELECT TOP {1} ID,MediumID,WaybillNO,MerchantID,WaybillType,Flag,DeliverStationID,
                                                    TopCODCompanyID,WarehouseId,ExpressCompanyID,RfdAcceptTime,
                                                    RfdAcceptDate,FinalExpressCompanyID,DeliverTime,DeliverDate,
                                                    ReturnWareHouseID,ReturnExpressCompanyID,TotalAmount,PaidAmount,
                                                    NeedPayAmount,BackAmount,NeedBackAmount,AccountWeight,AreaID,AreaType,
                                                    BoxsNo,Address,CreateTime,UpdateTime,IsDeleted,ReturnTime,ReturnDate,
                                                    IsFare,Fare,FareFormula,OperateType,ProtectedPrice,DistributionCode,
                                                    CurrentDistributionCode,ComeFrom,IsCOD 
                                                from LMS_RFD.dbo.FMS_CODBaseInfo with(nolock)
                                                WHERE 1 = 1 {0} ";


        public static string sqlIncomeBaseInfo = @"SELECT TOP {1} IncomeID,WaybillNo,WaybillType,MerchantID,ExpressCompanyID
                                                        ,FinalExpressCompanyID,DeliverStationID,TopCODCompanyID,RfdAcceptTime
                                                        ,DeliverTime,ReturnTime,ReturnExpressCompanyID,BackStationTime,BackStationStatus
                                                        ,ProtectedAmount,TotalAmount,PaidAmount,NeedPayAmount,BackAmount,NeedBackAmount
                                                        ,AccountWeight,AreaID,ReceiveAddress,SignType,InefficacyStatus,createtime,updatetime
                                                        ,ReceiveStationID,ReceiveDeliverManID,DistributionCode,CurrentDistributionCode
                                                        ,WayBillInfoWeight,SubStatus,AcceptType,CustomerOrder,OriginDepotNo,PeriodAccountCode
                                                        ,DeliverCode,IsDeleted,waybillcategory         
                                                    from LMS_RFD.dbo.FMS_IncomeBaseInfo with(nolock,forceseek)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlIncomeBaseInfo_History = @"SELECT TOP {1} IncomeID,WaybillNo,WaybillType,MerchantID,ExpressCompanyID
                                                        ,FinalExpressCompanyID,DeliverStationID,TopCODCompanyID,RfdAcceptTime
                                                        ,DeliverTime,ReturnTime,ReturnExpressCompanyID,BackStationTime,BackStationStatus
                                                        ,ProtectedAmount,TotalAmount,PaidAmount,NeedPayAmount,BackAmount,NeedBackAmount
                                                        ,AccountWeight,AreaID,ReceiveAddress,SignType,InefficacyStatus,createtime,updatetime
                                                        ,ReceiveStationID,ReceiveDeliverManID,DistributionCode,CurrentDistributionCode
                                                        ,WayBillInfoWeight,SubStatus,AcceptType,CustomerOrder,OriginDepotNo,PeriodAccountCode
                                                        ,DeliverCode,IsDeleted         
                                                    from LMS_RFD.dbo.FMS_IncomeBaseInfo with(nolock)
                                                    WHERE 1 = 1 {0} ";


        public static string sqlIncomeFeeInfo = @"SELECT TOP {1} IncomeFeeID,WaybillNO,IsAccount,AccountStandard,AccountFare,IsProtected
                                                        ,ProtectedStandard,ProtectedFee,IsReceive,ReceiveStandard,ReceiveFee,CreateBy,CreateTime
                                                        ,UpdateBy,UpdateTime,IsDeleted,TransferPayType,DeputizeAmount,POSReceiveStandard,POSReceiveFee
                                                        ,CashReceiveServiceStandard,CashReceiveServiceFee,POSReceiveServiceStandard,POSReceiveServiceFee
                                                        ,ExpressReceiveBasicDeduct,ExpressSendBasicDeduct,ExpressAreaDeduct,ExpressWeightDeduct,ProgramReceiveBasicDeduct
                                                        ,ProgramSendBasicDeduct,ProgramAreaDeduct,ProgramWeightDeduct,IsDeductAcount,AreaType 
                                                    from LMS_RFD.dbo.FMS_IncomeFeeInfo with(nolock,forceseek)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlIncomeFeeInfo_History = @"SELECT TOP {1} IncomeFeeID,WaybillNO,IsAccount,AccountStandard,AccountFare,IsProtected
                                                        ,ProtectedStandard,ProtectedFee,IsReceive,ReceiveStandard,ReceiveFee,CreateBy,CreateTime
                                                        ,UpdateBy,UpdateTime,IsDeleted,TransferPayType,DeputizeAmount,POSReceiveStandard,POSReceiveFee
                                                        ,CashReceiveServiceStandard,CashReceiveServiceFee,POSReceiveServiceStandard,POSReceiveServiceFee
                                                        ,ExpressReceiveBasicDeduct,ExpressSendBasicDeduct,ExpressAreaDeduct,ExpressWeightDeduct,ProgramReceiveBasicDeduct
                                                        ,ProgramSendBasicDeduct,ProgramAreaDeduct,ProgramWeightDeduct,IsDeductAcount,AreaType 
                                                    from LMS_RFD.dbo.FMS_IncomeFeeInfo with(nolock,forceseek)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlStationDailyFinanceDetails =
                                                    @"SELECT TOP {1} ID,EnterTime,WaybillNO,ReplaceTypeName,NeedPrice,NeedReturnPrice,PriceDiff
                                                        ,PriceReturnCash,Weight,WaybillType,StatusName,AcceptType,RejectReason,ResortReason,PostTime
                                                        ,DeliverManName,Comment,Status,CreateTime,StationID,Sources,MerchantID,OPType,PosNum,LmsId
                                                        ,DeductMoney,CustomerOrder,DailyTime,ExpandField,DeliverFee,Protectedprice,DeliverManID
                                                        ,IsChange,DistributionCode,CurrentDistributionCode,ComeFrom,IsDelete
                                                    from LMS_RFD.dbo.FMS_StationDailyFinanceDetails(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlStationDailyFinanceSum =
                                                    @"SELECT TOP {1} ID,DayReceiveOrderCount,DayReceiveNeedInSum,DayReceiveNeedOutSum,DayReceiveGoodsCount
                                                        ,DayOutOrderCount,PreDayResortCount,PreDayResortNeedInSum,PreDayResortNeedOutSum,DayTransferOrderCount
                                                        ,DayInStationNeedInSum,DayInStationNeedOutSum,DayNeedDeliverOrderCount,DayNeedDeliverInSum,DayNeedDeliverOutSum
                                                        ,CashSuccOrderCount,CashRealInSum,CashRealOutSum,PosSuccOrderCount,PosRealInSum,DeliverSuccRate,RejectOrderCount
                                                        ,AllRejectNeedInSum,AllRejectNeedOutSum,DayResortCount,DayResortNeedInSum,DayResortNeedOutSum,DayOutStationOrderCount
                                                        ,DayOutStationNeedInSum,DayOutStationNeedOutSum,ResortRate,StationID,Sources,MerchantID,DailyTime,CreateTime
                                                        ,UpdateTime,FinanceStatus,RealInCome,POSChecked,Remark,DayOutOrderSum,ExchangeOrderCount,ExchangeOrderSum,UpdateBy
                                                    from LMS_RFD.dbo.FMS_StationDailyFinanceSum(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlAreaExpressLevel =
                                            @"SELECT TOP {1} [AutoId],
            [AreaID]
           ,[ExpressCompanyID]
           ,[WareHouseID]
           ,[AreaType]
           ,[Enable]
           ,[EffectAreaType]
           ,[DoDate]
           ,[CreateBy]
           ,[CreateTime]
           ,[UpdateBy]
           ,[UpdateTime]
           ,[AuditStatus]
           ,[AuditBy]
           ,[AuditTime]
           ,[WareHouseType]
           ,[MerchantID]
           ,[ProductID]
           ,[ProductKid]
           ,[DistributionCode]
           ,[IsChange]
                                                    from AreaExpressLevel with(nolock,forceseek)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlAreaExpressLevelIncome =
                                            @"SELECT TOP {1} [AutoId]
      ,[AreaID]
      ,[MerchantID]
      ,[WareHouseID]
      ,[AreaType]
      ,[Enable]
      ,[EffectAreaType]
      ,[DoDate]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[AuditStatus]
      ,[AuditBy]
      ,[AuditTime]
      ,[ExpressCompanyID]
      ,[DistributionCode]
      ,[IsChange]
      ,GoodsCategoryCode
                                                    from AreaExpressLevelIncome(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlAreaExpressLevelIncomeLog =
                                    @"SELECT TOP {1} [LogID]
      ,[AreaID]
      ,[MerchantID]
      ,[WarehouseId]
      ,[AreaType]
      ,[LogText]
      ,[Enable]
      ,[CreateBy]
      ,[CreateTime]
      ,[ExpressCompanyID]
      ,[DistributionCode]
      ,[IsChange]
      ,GoodsCategoryCode
                                                    from AreaExpressLevelIncomeLog(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlAreaExpressLevelLog =
                            @"SELECT TOP {1} [LogID]
      ,[AreaID]
      ,[ExpressCompanyID]
      ,[WarehouseId]
      ,[AreaType]
      ,[LogText]
      ,[Enable]
      ,[CreateBy]
      ,[CreateTime]
      ,[WareHouseType]
      ,[MerchantID]
      ,[ProductID]
      ,[ProductKid]
      ,[DistributionCode]
      ,[IsChange]
                                                    from AreaExpressLevelLog(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlCODAccount =
                    @"SELECT TOP {1} [AccountID]
      ,[AccountNO]
      ,[ExpressCompanyID]
      ,[AccountDate]
      ,[AccountStatus]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[AuditBy]
      ,[AuditTime]
      ,[DeleteFlag]
      ,[DeliveryDateStr]
      ,[DeliveryDateEnd]
      ,[ReturnsDateStr]
      ,[ReturnsDateEnd]
      ,[VisitReturnsDateStr]
      ,[VisitReturnsDateEnd]
      ,[DeliveryHouse]
      ,[ReturnsHouse]
      ,[VisitReturnsHouse]
      ,[AccountType]
      ,[MerchantID]
      ,[IsDifference]
      ,[IsChange]
                                                    from FMS_CODAccount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlCODAccountDetailcs =
            @"SELECT TOP {1} [AccountDetailID]
      ,[AccountNO]
      ,[ExpressCompanyID]
      ,[WareHouseID]
      ,[AreaType]
      ,[DeliveryNum]
      ,[DeliveryVNum]
      ,[ReturnsNum]
      ,[ReturnsVNum]
      ,[VisitReturnsNum]
      ,[Formula]
      ,[DatumFare]
      ,[Allowance]
      ,[KPI]
      ,[POSPrice]
      ,[StrandedPrice]
      ,[IntercityLose]
      ,[OtherCost]
      ,[Fare]
      ,[DataType]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[DeleteFlag]
      ,[WareHouseType]
      ,[MerchantID]
      ,[AccountNum]
      ,[IsChange]
                                                    from FMS_CODAccountDetail(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlCODDeliveryCount =
            @"SELECT TOP {1} [AccountID]
      ,[AccountNO]
      ,[WareHouseID]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[Weight]
      ,[DeliveryType]
      ,[AccountDate]
      ,[FormCount]
      ,[Fare]
      ,[Formula]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[DeleteFlag]
      ,[WareHouseType]
      ,[MerchantID]
      ,[IsChange]
                                                    from FMS_CODDeliveryCount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlCODLine =
            @"SELECT TOP {1} [LineID]
      ,[CODLineNO]
      ,[ExpressCompanyID]
      ,[IsEchelon]
      ,[WareHouseID]
      ,[AreaType]
      ,[PriceFormula]
      ,[LineStatus]
      ,[AuditStatus]
      ,[AuditBy]
      ,[AuditTime]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[DeleteFlag]
      ,[WareHouseType]
      ,[MerchantID]
      ,[ProductID]
      ,[DistributionCode]
      ,[IsChange]
      ,[IsCOD]
      ,[Formula]
                                                    from FMS_CODLine(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlCODLineHistory =
            @"SELECT TOP {1} [LineID]
      ,[CODLineNO]
      ,[ExpressCompanyID]
      ,[IsEchelon]
      ,[WareHouseID]
      ,[AreaType]
      ,[PriceFormula]
      ,[LineStatus]
      ,[AuditStatus]
      ,[AuditBy]
      ,[AuditTime]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[DeleteFlag]
      ,[BakYear]
      ,[BakMonth]
      ,[WareHouseType]
      ,[MerchantID]
      ,[ProductID]
      ,[IsChange]
      ,[DistributionCode]
      ,[IsCOD]
      ,[Formula]
                                                    from FMS_CODLineHistory(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlCODLineWaitEffect =
            @"SELECT TOP {1} [LineID]
      ,[CODLineNO]
      ,[ExpressCompanyID]
      ,[IsEchelon]
      ,[WareHouseID]
      ,[AreaType]
      ,[PriceFormula]
      ,[LineStatus]
      ,[AuditStatus]
      ,[EffectDate]
      ,[AuditBy]
      ,[AuditTime]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[DeleteFlag]
      ,[WareHouseType]
      ,[MerchantID]
      ,[ProductID]
      ,[DistributionCode]
      ,[IsChange]
      ,[IsCOD]
      ,[Formula]
                                                    from FMS_CODLineWaitEffect(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlCODOperatorLog =
            @"SELECT TOP {1} [LogID]
      ,[PK_NO]
      ,[CreateBy]
      ,[CreateTime]
      ,[LogText]
      ,[LogType]
      ,[IsChange]
                                                    from FMS_CODOperatorLog(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlCODReturnsCount =
            @"SELECT TOP {1} [AccountID]
      ,[AccountNO]
      ,[WareHouseID]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[Weight]
      ,[ReturnsType]
      ,[AccountDate]
      ,[FormCount]
      ,[Fare]
      ,[Formula]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[DeleteFlag]
      ,[WareHouseType]
      ,[MerchantID]
      ,[IsChange]
                                                    from FMS_CODReturnsCount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlCodStatsLog =
            @"SELECT TOP {1} [CodStatsID]
      ,[StatisticsType]
      ,[ExpressCompanyID]
      ,[WareHouseID]
      ,[StatisticsDate]
      ,[IsSuccess]
      ,[Reasons]
      ,[Ip]
      ,[CreateTime]
      ,[UpdateTime]
      ,[WareHouseType]
      ,[MerchantID]
      ,[IsChange]
                                                    from FMS_CodStatsLog(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlCODVisitReturnsCount =
            @"SELECT TOP {1} [AccountID]
      ,[AccountNO]
      ,[WareHouseID]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[Weight]
      ,[AccountDate]
      ,[FormCount]
      ,[Fare]
      ,[Formula]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[DeleteFlag]
      ,[WareHouseType]
      ,[MerchantID]
      ,[IsChange]
                                                    from FMS_CODVisitReturnsCount(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlIncomeAccount =
            @"SELECT TOP {1} [AccountID]
      ,[AccountNO]
      ,[MerchantID]
      ,[AccountStatus]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[AuditBy]
      ,[AuditTime]
      ,[SearchDateStr]
      ,[SearchDateEnd]
      ,[IsDeleted]
      ,[IsChange]
                                                    from FMS_IncomeAccount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlIncomeAccountDetail =
            @"SELECT TOP {1} [DetailID]
      ,[AccountNO]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[DeliveryNum]
      ,[DeliveryVNum]
      ,[ReturnsNum]
      ,[ReturnsVNum]
      ,[VisitReturnsNum]
      ,[VisitReturnsVNum]
      ,[DeliveryStandard]
      ,[DeliveryFare]
      ,[DeliveryVStandard]
      ,[DeliveryVFare]
      ,[RetrunsStandard]
      ,[RetrunsFare]
      ,[ReturnsVStandard]
      ,[ReturnsVFare]
      ,[VisitReturnsStandard]
      ,[VisitReturnsFare]
      ,[VisitReturnsVStandard]
      ,[VisitReturnsVFare]
      ,[ProtectedStandard]
      ,[ProtectedFee]
      ,[ReceiveStandard]
      ,[ReceiveFee]
      ,[ReceivePOSStandard]
      ,[ReceivePOSFee]
      ,[OtherFee]
      ,[Fare]
      ,[DataType]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[IsDeleted]
      ,[OverAreaSubsidy]
      ,[KPI]
      ,[LostDeduction]
      ,[ResortDeduction]
      ,[StationId]
      ,[AccountCount]
      ,[IsChange]
                                                    from FMS_IncomeAccountDetail(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlIncomeDeliveryCount =
            @"SELECT TOP {1} [CountID]
      ,[AccountNO]
      ,[MerchantID]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[Weight]
      ,[CountType]
      ,[CountDate]
      ,[CountNum]
      ,[Fare]
      ,[Formula]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[IsDeleted]
      ,[StationID]
      ,[IsChange]
                                                    from FMS_IncomeDeliveryCount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlIncomeOtherFeeCount =
            @"SELECT TOP {1} [CountID]
      ,[AccountNO]
      ,[MerchantID]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[CountType]
      ,[CountDate]
      ,[ProtectedStandard]
      ,[ProtectedFee]
      ,[ReceiveStandard]
      ,[ReceiveFee]
      ,[ReceivePOSStandard]
      ,[ReceivePOSFee]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[IsDeleted]
      ,[StationID]
      ,[ServesStandard]
      ,[ServesFee]
      ,[POSServesStandard]
      ,[POSServesFee]
      ,[IsChange]
                                                    from FMS_IncomeOtherFeeCount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlIncomeReturnsCount =
            @"SELECT TOP {1} [CountID]
      ,[AccountNO]
      ,[MerchantID]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[Weight]
      ,[CountType]
      ,[CountDate]
      ,[CountNum]
      ,[Fare]
      ,[Formula]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[IsDeleted]
      ,[StationID]
      ,[IsChange]
                                                    from FMS_IncomeReturnsCount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlIncomeStatLog =
            @"SELECT TOP {1} [LogID]
      ,[StatisticsType]
      ,[MerchantID]
      ,[StationID]
      ,[ExpressCompanyID]
      ,[StatisticsDate]
      ,[IsSuccess]
      ,[Reasons]
      ,[Ip]
      ,[CreateTime]
      ,[UpdateTime]
      ,[IsChange]
                                                    from FMS_IncomeStatLog(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlIncomeVisitReturnsCount =
            @"SELECT TOP {1} [CountID]
      ,[AccountNO]
      ,[MerchantID]
      ,[ExpressCompanyID]
      ,[AreaType]
      ,[Weight]
      ,[CountType]
      ,[CountDate]
      ,[CountNum]
      ,[Fare]
      ,[Formula]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[IsDeleted]
      ,[StationID]
      ,[IsChange]
                                                    from FMS_IncomeVisitReturnsCount(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlMerchantDeliverFee =
            @"SELECT TOP {1} [ID]
      ,[MerchantID]
      ,[PaymentType]
      ,[PaymentPeriod]
      ,[DeliverFeeType]
      ,[DeliverFeePeriod]
      ,[FeeFactors]
      ,[IsUniformedFee]
      ,[BasicDeliverFee]
      ,[FormulaID]
      ,[FormulaParamters]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[UpdateCode]
      ,[AuditBy]
      ,[AuditTime]
      ,[AuditCode]
      ,[AuditResult]
      ,[Status]
      ,[RefuseFeeRate]
      ,[ReceiveFeeRate]
      ,[PaymentPeriodDate]
      ,[DeliverFeePeriodDate]
      ,[FirstWeight]
      ,[StatPramer]
      ,[AddWeightPrice]
      ,[FirstWeightPrice]
      ,[VolumeParmer]
      ,[ProtectedParmer]
      ,[VisitReturnsFee]
      ,[VisitChangeFee]
      ,[ReceivePOSFeeRate]
      ,[CreateBy]
      ,[CreateTime]
      ,[VisitReturnsVFee]
      ,[CashServiceFee]
      ,[POSServiceFee]
      ,[ReceiveFeeType]
      ,[ReceivePOSFeeType]
      ,[CashServiceType]
      ,[POSServiceType]
      ,[WeightType]
      ,[WeightValueRule]
      ,[DistributionCode]
      ,[IsChange]
      ,ExtraProtected
    ,ExtraRefuseFeeRate
    ,ExtraVisitReturnsFee
    ,ExtraVisitChangeFee
    ,ExtraVisitReturnsVFee
    ,ExtraReceiveFeeRate
    ,ExtraCashServiceFee
    ,ExtraPOSServiceFee
    ,ExtraReceivePOSFeeRate
    ,IsCategory
                                                    from FMS_MerchantDeliverFee(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlNoGenerateEx =
            @"SELECT TOP {1} [CurrentDate]
      ,[TabColCode]
      ,[LatestNo]
      ,[IsChange]
                                                    from FMS_NoGenerateEx(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlOperateLog =
            @"SELECT TOP {1} [OperateLogID]
      ,[BizOrderNo]
      ,[LogType]
      ,[Operation]
      ,[Operator]
      ,[OperatorId]
      ,[OperatorDept]
      ,[OperateTime]
      ,[Description]
      ,[Reasult]
      ,[Status]
      ,[IsSyn]
      ,[IsChange]
                                                    from FMS_OperateLog(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlSortingTransferDetail =
            @"SELECT TOP {1} [DetailKid]
      ,[MerchantID]
      ,[WaybillNo]
      ,[WaybillType]
      ,[SortingCenterID]
      ,[TSortingCenterID]
      ,[SoringMerchantID]
      ,[CreateCityID]
      ,[SortCityID]
      ,[DeliverStationID]
      ,[TopCODCompanyID]
      ,[ToStationTime]
      ,[OutBoundTime]
      ,[ReturnTime]
      ,[InSortingTime]
      ,[DistributionCode]
      ,[IsAccount]
      ,[AccountFare]
      ,[AccountFormula]
      ,[IsDeleted]
      ,[CreateTime]
      ,[UpdateTime]
      ,[IsChange]
      ,[ReturnSortingCenterID]
      ,[OutType]
      ,[IntoType]

        from FMS_SortingTransferDetail with(nolock,forceseek)
        WHERE 1 = 1 {0} ";
        public static string sqlStationDeliverFee =
            @"SELECT TOP {1} [ID]
      ,[MerchantID]
      ,[StationID]
      ,[BasicDeliverFee]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[UpdateCode]
      ,[AuditBy]
      ,[AuditTime]
      ,[AuditCode]
      ,[AuditResult]
      ,[Status]
      ,[ExpressCompanyID]
      ,[IsCenterSort]
      ,[AreaType]
      ,[CreateBy]
      ,[CreateTime]
      ,[IsDeleted]
      ,[DistributionCode]
      ,[IsChange]
      ,GoodsCategoryCode
      ,DeliverFee
      ,IsCod
      
                                                    from FMS_StationDeliverFee(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlTypeRelation =
            @"SELECT TOP {1} [TypeRelationKid]
      ,[TypeRelationName]
      ,[DistributionCode]
      ,[TypeCodeNo]
      ,[RelationNameID]
      ,[IsDelete]
      ,[CreateBy]
      ,[CreateTime]
      ,[UpdateBy]
      ,[UpdateTime]
      ,[IsChange]
                                                    from FMS_TypeRelation(nolock)
                                                    WHERE 1 = 1 {0} ";
        public static string sqlStatusCodeInfo =
            @"SELECT TOP {1} [CodeType]
      ,[CodeNo]
      ,[CodeDesc]
      ,[OrderBy]
      ,[Enabled]
      ,[CreatBy]
      ,[CreatDate]
      ,[UpdateBy]
      ,[UpdateDate]
      ,[DistributionCode]
      ,[IsChange]
                                                    from StatusCodeInfo(nolock)
                                                    WHERE 1 = 1 {0} ";

        public static string sqlFMSTableColumnDic =
            @"SELECT TOP {1} [TabColCode]
      ,[TableName]
      ,[ColumnName]
      ,[Remark]
      ,[IsChange]
                                                    from FMSTableColumnDic(nolock)
                                                    WHERE 1 = 1 {0} ";
    }
}
