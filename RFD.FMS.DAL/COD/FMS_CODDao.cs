using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.COD;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.COD;
using RFD.FMS.AdoNet;

namespace RFD.FMS.DAL.COD
{
    public class FMS_CODDao : SqlServerDao, IFMS_CODDao
	{
		public int AddWaybillLmsToMedium(LMS_Syn_FMS_COD model)
		{
			var strSql = new StringBuilder();
			strSql.Append(@" 
                          INSERT INTO LMS_RFD.dbo.LMS_Syn_FMS_COD
                          (
                            waybillno,
                            OperateType,
                            OperateTime,
                            IsSyn,
                            StationID,
                            Createby,
                            IsChange
                          )
                          values (  @waybillno,
                                    @OperateType,
                                    @OperateTime,
                                    @IsSyn,
                                    @StationID,
                                    @Createby,
                                    2
                                  )  ");
			SqlParameter[] parameters = {
					new SqlParameter("@waybillno", SqlDbType.BigInt,8),
					new SqlParameter("@OperateType", SqlDbType.Int,4),
					new SqlParameter("@OperateTime", SqlDbType.DateTime),
                    new SqlParameter("@IsSyn", SqlDbType.Bit,1),
					new SqlParameter("@StationID", SqlDbType.Int,4),
                    new SqlParameter("@Createby",SqlDbType.NVarChar,50) };
			parameters[0].Value = model.WayBillNO;
			parameters[1].Value = model.OperateType;
			parameters[2].Value = model.OperateTime;
			parameters[3].Value = 0;
			parameters[4].Value = model.StationID;
			parameters[5].Value = model.CreateBY;
			var rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
			return rowCount;
		}

        public int AddWaybillLmsToMediumV2(LMS_Syn_FMS_COD model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@" 
SET @num = -1
IF NOT EXISTS ( SELECT 1 FROM LMS_RFD.dbo.LMS_SYN_FMS_COD lsfc(NOLOCK) WHERE lsfc.WaybillNo=@WaybillNo AND lsfc.OperateType=@OperateType AND lsfc.StationID=@StationID AND lsfc.IsSyn NOT IN (3,4) ) 
BEGIN
      INSERT INTO LMS_RFD.dbo.LMS_Syn_FMS_COD
      (waybillno,OperateType,OperateTime,IsSyn,StationID,Createby,IsChange)
      select  @waybillno,@OperateType,@OperateTime,@IsSyn,@StationID,@Createby,2
        SET @num = 1
    END
ELSE 
BEGIN 
    SET @num = 0
END
");
            SqlParameter[] parameters = {
					new SqlParameter("@waybillno", SqlDbType.BigInt,8),
					new SqlParameter("@OperateType", SqlDbType.Int,4),
					new SqlParameter("@OperateTime", SqlDbType.DateTime),
                    new SqlParameter("@IsSyn", SqlDbType.Bit,1),
					new SqlParameter("@StationID", SqlDbType.Int,4),
                    new SqlParameter("@Createby",SqlDbType.NVarChar,50),
                    new SqlParameter("@num",SqlDbType.Int)};
            parameters[0].Value = model.WayBillNO;
            parameters[1].Value = model.OperateType;
            parameters[2].Value = model.OperateTime;
            parameters[3].Value = 0;
            parameters[4].Value = model.StationID;
            parameters[5].Value = model.CreateBY;
            parameters[6].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            var n = (int)parameters[6].Value;
            return n;
        }

		public int UpDateMediumForSyn(string ids, int isSyn)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@" 
                          UPDATE LMS_RFD.dbo.LMS_Syn_FMS_COD
                          SET
                            IsSyn = {0},
                            SynTime = GetDate(),
                            IsChange=2
                          WHERE
                               ID in ({1}) ", isSyn, ids);
			var rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString());
			return rowCount;
		}

		/// <summary>
		/// 根据同步类型，依据中间表从业务表取数
		/// </summary>
		/// <param name="topNumber"></param>
		/// <param name="synType"></param>
		/// <returns></returns>
		public DataTable SearchIdsForShip(string topNumber, string synType)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
                DECLARE @etime DATETIME
                SET @etime = DATEADD(minute, {0}, GETDATE())
                SELECT top {1}
                ID
                FROM LMS_RFD.dbo.LMS_Syn_FMS_COD syn (NOLOCK) 
                where 
                    syn.operatetime < @etime 
                and syn.IsSyn = {2} 
                and syn.OperateType<6  
                and syn.StationID>0
               ", -3, topNumber, synType);
			var dt = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

        public DataTable SearchIdsForShipBySynId(string SynIds)
        {
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
                SELECT
                ID
                FROM LMS_RFD.dbo.LMS_Syn_FMS_COD syn (nolock) 
                where syn.ID in ({0})
               ", SynIds);
            var dt = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
            return dt;
        }

		public DataTable SearchIdsForBack(string topNumber, string synType)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
DECLARE @etime DATETIME
            SET @etime = DATEADD(minute, {0}, GETDATE())

SELECT top {1}
       syn6.id,
       syn6.waybillno
FROM LMS_RFD.dbo.LMS_Syn_FMS_COD syn6 (Nolock) 
where syn6.operatetime<@etime 
and  syn6.IsSyn = {2} 
and syn6.OperateType = 6  
and syn6.StationID>0 
               ", -3, topNumber, synType);
			var dt = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public DataTable SearchIdsForBackAdvanced(string topNumber, string synType)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
                SELECT top {0}
                syn7.id,
                syn7.waybillno
                FROM LMS_RFD.dbo.LMS_Syn_FMS_COD syn7 (NOLOCK) 
                where syn7.IsSyn = {1} and syn7.OperateType = 7  and syn7.StationID>0
               ", topNumber, synType);
			var dt = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public DataTable SearchWaybillnosForBack(string topNumber)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(
                @"
select  ROW_NUMBER() over(order by [CreatTime]) as ID,
                       CreatTime,
                       WaybillNO,
                       backstatus,
                       Sources,
                       MerchantID,
                       ReturnWareHouse,
                       ReturnTime,
                       ReturnExpressCompanyId
                FROM   LMS_RFD.dbo.Waybill (NOLOCK)
                WHERE  CreatTime > '2012-01-01'
                       and backstatus in (6,7)
                       and merchantid<>8
                       and merchantid<>9

                 ");
			var dt = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public DataTable SearchInfoForShip(string ids)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
             SELECT 
                    wb.WaybillNO,
                    wb.WaybillType,
                    wb.MerchantID,
                    wb.sources,
                    wb.WarehouseId,
                    wb.CreatStation,
                    wb.CreatTime,
                    wb.DeliverTime,
                    ec.TopCODCompanyID,
                    ec.DistributionCode AS StationDistributionCode,
                    wsi.Amount,
                    wsi.PaidAmount,
                    wsi.NeedAmount,
                    wsi.TransferFee,
                    wsi.additionalprice,
                    wsi.NeedBackAmount,
                    wsi.FactBackAmount,
                    wi.WayBillInfoWeight, 
                    wi.WayBillInfoVolumeWeight,
                    pca.AreaID,
                    wbtsi.ReceiveProvince,
                    wbtsi.ReceiveCity,
                    wbtsi.ReceiveArea,
                    wbtsi.ReceiveAddress,
                    wi.WayBillBoxNo,
                    obex.OutBoundStation,
                    wsi.ProtectedPrice,
                    wb.DistributionCode,
                    wb.CurrentDistributionCode,
                    wb.ComeFrom,
                    syn.ID as MediumId,
                    syn.OperateType,
                    syn.OperateTime,
                    syn.StationID,
                    syn.IsSyn,
                    syn.SynTime,
                    syn.Createby
            FROM LMS_RFD.dbo.Waybill wb (NOLOCK)
            join LMS_RFD.dbo.LMS_Syn_FMS_COD syn(NOLOCK) on syn.waybillno=wb.waybillno  and syn.ID in ({0})
            JOIN rfd_pms.dbo.ExpressCompany ec(NOLOCK) ON  ec.ExpressCompanyID = syn.StationID
            JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON  wsi.WaybillNO = wb.WaybillNO
            JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON  wi.WaybillNO = wb.WaybillNO
            JOIN LMS_RFD.dbo.waybilltakesendinfo wbtsi(NOLOCK) ON  wbtsi.WaybillNO = wb.WaybillNO
          
            left join (select p.ProvinceName,c.CityName,a.AreaName,a.AreaID
            from RFD_PMS.dbo.Province p (nolock) join RFD_PMS.dbo.City c(nolock) on p.ProvinceID=c.ProvinceID join RFD_PMS.dbo.Area a(nolock) on c.CityID=a.CityID
            AND a.IsDeleted=0 AND c.IsDeleted=0 AND p.IsDeleted=0 )
            pca on wbtsi.ReceiveArea=pca.AreaName and wbtsi.Receivecity=pca.CityName and wbtsi.ReceiveProvince=pca.ProvinceName

            LEFT JOIN (SELECT ob.WaybillNO,ob.OutBoundStation
             FROM LMS_RFD.dbo.OutBound ob(NOLOCK)  
            JOIN RFD_PMS.dbo.ExpressCompany ex(NOLOCK) ON  ex.ExpressCompanyID = ob.OutBoundStation AND ex.DistributionCode = 'rfd'
            AND ob.OutStationType IN (3, 0)
            ) obex ON syn.WaybillNo = obex.WaybillNo
               ", ids);
			var dt = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		/// <summary>
		/// 逆向流程
		/// </summary>
		/// <returns></returns>
		public DataTable SearchInfoForBack(string ids)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(
                @"
             SELECT 
            wb.[WaybillNO],
            wb.[WaybillType],
            wb.BackStatus,
            wb.[sources],
            wb.[MerchantID],
            wb.[ReturnWareHouse],
            CASE WHEN ec1.CompanyFlag=2 THEN ec2.ExpressCompanyID ELSE wb.[ReturnExpressCompanyId] END ReturnExpressCompanyId,
            wb.[ReturnTime],
            wb.ComeFrom,
            wb.DeliverStationID,
            ec.TopCODCompanyID,
            wsi.FactBackAmount,
            syn.OperateType,
            syn.OperateTime,
            syn.ID,
            syn.StationID,
            syn.IsSyn,
            syn.SynTime,
            syn.CreateBy
            FROM  LMS_RFD.dbo.Waybill wb (NOLOCK)
            join LMS_RFD.dbo.LMS_Syn_FMS_COD syn(NOLOCK) on syn.waybillno=wb.waybillno and syn.ID in ( {0} )
            JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON  wsi.WaybillNO = wb.WaybillNO
            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=wb.DeliverStationID
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec1(NOLOCK) ON ec1.ExpressCompanyID=wb.ReturnExpressCompanyId AND ec1.CompanyFlag=2
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK) ON ec2.ExpressCompanyID=ec1.ParentID AND ec2.CompanyFlag=1
            ", ids);
			var dt = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public DataTable SearchInfoForBackAdvanced(string ids)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(
                @"
             SELECT 
            wb.[WaybillNO],
            wb.ComeFrom,
            syn.OperateType,
            syn.OperateTime,
            syn.ID,
            syn.StationID,
            syn.IsSyn,
            syn.SynTime,
            syn.CreateBy,
            ob.OutBoundStation
            FROM  LMS_RFD.dbo.Waybill wb (NOLOCK)
            join LMS_RFD.dbo.LMS_Syn_FMS_COD syn(NOLOCK) on syn.waybillno=wb.waybillno and syn.ID in  ( {0} )
            left JOIN LMS_RFD.dbo.outbound  ob (NOLOCK) on  ob.outboundid=wb.outboundid
            ", ids);
			var dt = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public int InsertForShip(FMS_CODBaseInfo model)
		{
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
SET @num = -1
IF NOT EXISTS ( SELECT  *
                FROM    LMS_RFD.dbo.FMS_CODBaseInfo (NOLOCK)
                WHERE   MediumID = @MediumID ) 
    BEGIN
        INSERT  INTO  LMS_RFD.dbo.FMS_CODBaseInfo
                ( [MediumID] ,
                  [WaybillNO] ,
                  [MerchantID] ,
                  [WaybillType] ,
                  [Flag] ,
                  [DeliverStationID] ,
                  [TopCODCompanyID] ,
                  [WarehouseId] ,
                  [ExpressCompanyID] ,
                  [RfdAcceptTime] ,
                  [RfdAcceptDate] ,
                  [FinalExpressCompanyID] ,
                  [DeliverTime] ,
                  [DeliverDate] ,
                  [TotalAmount] ,
                  [PaidAmount] ,
                  [NeedPayAmount] ,
                  [BackAmount] ,
                  [NeedBackAmount] ,
                  [AccountWeight] ,
                  [AreaID] ,
                  [BoxsNo] ,
                  [Address] ,
                  [CreateTime] ,
                  [IsFare] ,
                  [OperateType] ,
                  [ProtectedPrice] ,
                  [DistributionCode] ,
                  [CurrentDistributionCode],
                  [AreaType],
                  [IsChange],
                  [ComeFrom],
                  [IsCOD] 
                )
                SELECT  @MediumID ,
                        @WaybillNO ,
                        @MerchantID ,
                        @WaybillType ,
                        @Flag ,
                        @DeliverStationID ,
                        @TopCODCompanyID ,
                        @WarehouseId ,
                        @ExpressCompanyID ,
                        @RfdAcceptTime ,
                        @RfdAcceptDate ,
                        @FinalExpressCompanyID ,
                        @DeliverTime ,
                        @DeliverDate ,
                        @TotalAmount ,
                        @PaidAmount ,
                        @NeedPayAmount ,
                        @BackAmount ,
                        @NeedBackAmount ,
                        @AccountWeight ,
                        @AreaID ,
                        @BoxsNo ,
                        @Address ,
                        GETDATE() ,
                        @IsFare ,
                        @OperateType ,
                        @ProtectedPrice ,
                        @DistributionCode ,
                        @CurrentDistributionCode,
                        @AreaType,
                        @IsChange,
                        @ComeFrom,
                        @IsCOD
        SET @num = 1
    END
ELSE 
    BEGIN 
        SET @num = 0 
    END
           ");
            SqlParameter[] parameters = 
            {
                new SqlParameter("@MediumID", SqlDbType.BigInt,8),
                new SqlParameter("@waybillno", SqlDbType.BigInt,8),
                new SqlParameter("@MerchantID", SqlDbType.Int,4),
                new SqlParameter("@WaybillType", SqlDbType.NVarChar,20),
                new SqlParameter("@Flag", SqlDbType.Int,4),            
                new SqlParameter("@DeliverStationID", SqlDbType.Int,4),
                new SqlParameter("@TopCODCompanyID", SqlDbType.Int,4),
                new SqlParameter("@WarehouseId", SqlDbType.NVarChar,20),
                new SqlParameter("@ExpressCompanyID", SqlDbType.Int,4),
                new SqlParameter("@RfdAcceptTime", SqlDbType.DateTime),
                new SqlParameter("@RfdAcceptDate", SqlDbType.Date),
                new SqlParameter("@FinalExpressCompanyID", SqlDbType.Int,4),
                new SqlParameter("@DeliverTime", SqlDbType.DateTime),
                new SqlParameter("@DeliverDate", SqlDbType.Date),
                new SqlParameter("@TotalAmount", SqlDbType.Decimal),
                new SqlParameter("@PaidAmount", SqlDbType.Decimal),
                new SqlParameter("@NeedPayAmount", SqlDbType.Decimal),
                new SqlParameter("@BackAmount", SqlDbType.Decimal),
                new SqlParameter("@NeedBackAmount", SqlDbType.Decimal),
                new SqlParameter("@AccountWeight", SqlDbType.Decimal),
                new SqlParameter("@AreaID", SqlDbType.NVarChar,50),
                new SqlParameter("@BoxsNo", SqlDbType.NVarChar,30),
                new SqlParameter("@Address", SqlDbType.NVarChar,150),
                new SqlParameter("@IsFare",SqlDbType.Int,4),
                new SqlParameter("@OperateType",SqlDbType.Int,4),
                new SqlParameter("@ProtectedPrice", SqlDbType.Decimal),
                new SqlParameter("@DistributionCode",SqlDbType.NVarChar,50),
                new SqlParameter("@CurrentDistributionCode",SqlDbType.NVarChar,50),
                new SqlParameter("@AreaType",SqlDbType.Int), 
                new SqlParameter("@IsChange",SqlDbType.Bit),
                new SqlParameter("@num",SqlDbType.Int),
                new SqlParameter("@ComeFrom",SqlDbType.Int),
                new SqlParameter("@IsCOD",SqlDbType.Int)
            };

            parameters[0].Value = model.MediumID;
            parameters[1].Value = model.WaybillNO;
            parameters[2].Value = model.MerchantID;
            parameters[3].Value = model.WaybillType;
            parameters[4].Value = model.Flag;
            parameters[5].Value = model.DeliverStationID;
            parameters[6].Value = model.TopCODCompanyID;
            parameters[7].Value = model.WarehouseId;
            parameters[8].Value = model.ExpressCompanyID;
            parameters[9].Value = model.RfdAcceptTime;
            parameters[10].Value = model.RfdAcceptDate;
            parameters[11].Value = model.FinalExpressCompanyID;
            parameters[12].Value = model.DeliverTime;
            parameters[13].Value = model.DeliverDate;
            parameters[14].Value = model.TotalAmount;
            parameters[15].Value = model.PaidAmount;
            parameters[16].Value = model.NeedPayAmount;
            parameters[17].Value = model.BackAmount;
            parameters[18].Value = model.NeedBackAmount;
            parameters[19].Value = model.AccountWeight;
            parameters[20].Value = model.AreaID;
            parameters[21].Value = model.BoxsNo;
            parameters[22].Value = model.Address;
            parameters[23].Value = model.IsFare;
            parameters[24].Value = model.OperateType;
            parameters[25].Value = model.ProtectedPrice;
            parameters[26].Value = model.DistributionCode;
            parameters[27].Value = model.CurrentDistributionCode;
            parameters[28].Value = model.AreaType;
            parameters[29].Value = true;
            parameters[30].Direction = ParameterDirection.Output;
            parameters[31].Value = model.ComeFrom;
            parameters[32].Value = model.IsCOD;

            var rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            var n = (int)parameters[30].Value;

            return n;
		}

		public int UpdateForBack(FMS_CODBaseInfo model)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
            UPDATE LMS_RFD.dbo.FMS_CODBaseInfo
            SET [ReturnWareHouseID] = @ReturnWareHouseID,
                [ReturnExpressCompanyID] = @ReturnExpressCompanyID,
                [UpdateTime] = GETDATE(),
                [ReturnTime] = @ReturnTime,
                [ReturnDate] = @ReturnDate,
                [BackAmount] = @BackAmount,
                [IsChange] = @IsChange,
                DeliverStationID=@DeliverStationID,
                TopCODCompanyID=@TopCODCompanyID
            WHERE [MediumID] = @MediumID
            ");
			SqlParameter[] parameters = 
            {
                new SqlParameter("@ReturnWareHouseID", SqlDbType.NVarChar,20),
                new SqlParameter("@ReturnExpressCompanyID", SqlDbType.Int,4),
                new SqlParameter("@ReturnTime", SqlDbType.DateTime),
                new SqlParameter("@ReturnDate", SqlDbType.Date),
                new SqlParameter("@MediumID", SqlDbType.BigInt,8),
                new SqlParameter("@BackAmount",SqlDbType.Decimal),
                new SqlParameter("@IsChange",SqlDbType.Bit),
                new SqlParameter("@DeliverStationID",SqlDbType.Int),
                new SqlParameter("@TopCODCompanyID",SqlDbType.Int)
            };
			parameters[0].Value = model.ReturnWareHouseID;
			parameters[1].Value = model.ReturnExpressCompanyID;
			parameters[2].Value = model.ReturnTime;
			parameters[3].Value = model.ReturnDate;
			parameters[4].Value = model.MediumID;
		    parameters[5].Value = model.BackAmount;
            parameters[6].Value = true;
            parameters[7].Value = model.DeliverStationID;
            parameters[8].Value = model.TopCODCompanyID;

			var rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

			return rowCount;
		}

		public int AdvancedUpdateForBack(FMS_CODBaseInfo model)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
            UPDATE LMS_RFD.dbo.FMS_CODBaseInfo
            SET [DeliverTime] = @DeliverTime,
                [DeliverDate] = @DeliverDate,
                [UpdateTime] = GETDATE(),
                [IsChange] = 1
            WHERE [DeliverTime] is null and [WaybillNO] = @WaybillNO

            UPDATE LMS_RFD.dbo.FMS_CODBaseInfo
            SET [UpdateTime] = GETDATE(),
                [FinalExpressCompanyID] = @FinalExpressCompanyID,
                [IsChange]=1
            WHERE [FinalExpressCompanyID] is null and [WaybillNO] = @WaybillNO

            IF(@MerchantID NOT IN (8,9))
            BEGIN
            	UPDATE LMS_RFD.dbo.FMS_CODBaseInfo
                SET AccountWeight = @AccountWeight,IsChange=1 WHERE [WaybillNO] = @WaybillNO
            END
            ");
			SqlParameter[] parameters = {
                                            new SqlParameter("@DeliverTime", SqlDbType.DateTime),
                                            new SqlParameter("@DeliverDate", SqlDbType.Date),
                                            new SqlParameter("@FinalExpressCompanyID", SqlDbType.Int,4),
                                            new SqlParameter("@WaybillNO", SqlDbType.BigInt,8),
                                            new SqlParameter("@AccountWeight", SqlDbType.Decimal),
                                            new SqlParameter("@MerchantID", SqlDbType.Int)
                                        };
			parameters[0].Value = model.DeliverTime;
			parameters[1].Value = model.DeliverDate;
			parameters[2].Value = model.FinalExpressCompanyID;
			parameters[3].Value = model.WaybillNO;
            parameters[4].Value = model.AccountWeight;
            parameters[5].Value = model.MerchantID;
			var rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
			return rowCount;
        }

        public long SearchUpdateMediumId(long waybillno, string waybillType, string backStatus)
		{
            string sql = "";
            if (backStatus== "7")
            {
                sql = @"SELECT top 1 [ID] as MediumID
                FROM  LMS_RFD.dbo.LMS_SYN_FMS_COD lsfc(NOLOCK)
                where 
                lsfc.waybillno = @WaybillNO
                and operateType = 4
                AND lsfc.IsSyn NOT IN (3,4)
                order by operatetime DESC";
            }
            else
            {
                sql = @"SELECT top 1 [ID] as MediumID
                FROM  LMS_RFD.dbo.LMS_SYN_FMS_COD lsfc(NOLOCK)
                where 
                lsfc.waybillno = @WaybillNO
                and operateType in (1,3)
                AND lsfc.IsSyn NOT IN (3,4)
                order by operatetime DESC";
            }

            SqlParameter[] parameters ={
                                           new SqlParameter("@WaybillNO",SqlDbType.BigInt)
                                      };
            parameters[0].Value = waybillno;
            var dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, sql,parameters).Tables[0];
			if (dt == null || dt.Rows.Count <= 0)
			{
				return 0;
			}

			var num = Convert.ToInt64(dt.Rows[0][0]);
			return num;
		}



		public long SearchUpdateMediumIdTemp(long waybillno)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
            SELECT top 1 MediumID
            FROM  LMS_RFD.dbo.FMS_CODBaseInfo (NOLOCK)
            where 
            waybillno = {0}
            order by [CreateTime] desc
            ", waybillno);
			var dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString()).Tables[0];
			if (dt == null || dt.Rows.Count <= 0)
			{
				return 0;
			}
			var num = Convert.ToInt64(dt.Rows[0][0]);
			return num;
		}

		public DataTable SearchAnyInfoOnMedium(string sql)
		{
			var dt = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, sql).Tables[0];
			return dt;
		}

		public int UpdateDelete(string sql)
		{
			return SqlHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql);
		}

        public bool UpdateForInvalid(long waybillNo)
        {
            string sql = @"update fms_codbaseinfo set DeliverTime = getdate()
                    ,DeliverDate = convert(nvarchar(10),getdate(),120) 
                    where  DeliverTime is  null and waybillno =@waybillno";
            SqlParameter[] parameters ={
                                           new SqlParameter("@waybillno",SqlDbType.BigInt)
                                      };
            parameters[0].Value = waybillNo;
            return SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

		public DataTable SearchInfoForDeliverTimeAndOutBountStation(string ids)
		{
		    var sql = new StringBuilder();
sql.AppendFormat(@"                     
SELECT lsfc.ID,
       lsfc.waybillno,
       lsfc.OperateType,
       lsfc.StationID,
       ob.OutBoundStation,
       wb.DeliverTime,
	   wi.WayBillInfoWeight,
	   wb.MerchantID
FROM   LMS_RFD.dbo.LMS_SYN_FMS_COD lsfc(NOLOCK)
       JOIN LMS_RFD.dbo.Waybill wb(NOLOCK)
            ON  wb.WaybillNO = lsfc.WaybillNo
            AND lsfc.ID IN ({0})
       JOIN LMS_RFD.dbo.WaybillInfo wi(NOLOCK) ON wi.WaybillNO = wb.WaybillNO
       JOIN LMS_RFD.dbo.outbound ob(NOLOCK)
            ON  lsfc.WaybillNo = ob.WaybillNo
            AND ob.OutStationType IN (3, 0)
            AND lsfc.OperateType = 7
       JOIN RFD_PMS.dbo.ExpressCompany ex(NOLOCK)
            ON  ex.ExpressCompanyID = ob.OutBoundStation
            AND ex.DistributionCode = 'rfd'
            ", ids)  ;
			var dt = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql.ToString()).Tables[0];
			return dt;
		}

        public DataTable SearchIdsForDeliverTimeAndOutBountStation(string topNumber, string synType)
        {
            var sql = new StringBuilder();
            sql.Append(@"     
            select top (@num)
                 lsfc.ID
            from LMS_RFD.dbo.LMS_SYN_FMS_COD lsfc (NOLOCK) 
            where
                  lsfc.OperateType = @OperateType
                 and lsfc.IsSyn = @IsSyn
                 and lsfc.StationID> @StationID 
                 and lsfc.operatetime < @SubTime 
            ");
            SqlParameter[] parameters =
            {
              new SqlParameter("@num",SqlDbType.Int),
              new SqlParameter("@OperateType",SqlDbType.Int,4),
              new SqlParameter("@IsSyn",SqlDbType.Int,4), 
              new SqlParameter("@StationID",SqlDbType.Int,4),
              new SqlParameter("@SubTime",SqlDbType.DateTime)   
            };
            parameters[0].Value = Convert.ToInt32(topNumber);
            parameters[1].Value = 7;
            parameters[2].Value = Convert.ToInt32(synType);
            parameters[3].Value = 0;
            parameters[4].Value = Convert.ToDateTime(DateTime.Now.AddMinutes(-30));
            var dt = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, sql.ToString(),parameters).Tables[0];
            return dt;
        }


	    public int UpdateWaybillForCOD(long waybillno, int issyn)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@" 
                          UPDATE LMS_RFD.dbo.waybill
                          SET
                            IsSyn = {0},
                            SynTime = GetDate(),
                            IsChange=2
                          WHERE
                               waybillno in ({1}) ", issyn, waybillno);
			var rowCount = SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString());
			return rowCount;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStationByIDNOTwo(long id, long waybillno)
        {
            string sql = @"SELECT TOP 1 DistributionCode FROM LMS_RFD.dbo.LMS_SYN_FMS_COD lsfc(NOLOCK) 
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON lsfc.StationID=ec.ExpressCompanyID
WHERE lsfc.ID>@ID AND OperateType=3 AND WaybillNo=@WaybillNo
ORDER BY ID";
            SqlParameter[] parameters = {
                                            new SqlParameter("@ID",SqlDbType.BigInt), 
                                            new SqlParameter("@WaybillNo",SqlDbType.BigInt), 
                                        };
            parameters[0].Value = id;
            parameters[1].Value = waybillno;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql, parameters);

            if(ds==null || ds.Tables.Count<=0)
            {
                return "";
            }
            else
            {
                if(ds.Tables[0]==null || ds.Tables[0].Rows.Count<=0)
                {
                    return "";
                }
                else
                {
                    return ds.Tables[0].Rows[0]["DistributionCode"].ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStationByIDNOThird(long id, long waybillno)
        {
            string sql = @"SELECT TOP 1 DistributionCode,ID FROM LMS_RFD.dbo.LMS_SYN_FMS_COD lsfc(NOLOCK) 
JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON lsfc.StationID=ec.ExpressCompanyID
WHERE lsfc.ID<@ID AND OperateType=2 AND WaybillNo=@WaybillNo
ORDER BY ID DESC";
            SqlParameter[] parameters = {
                                            new SqlParameter("@ID",SqlDbType.BigInt), 
                                            new SqlParameter("@WaybillNo",SqlDbType.BigInt), 
                                        };
            parameters[0].Value = id;
            parameters[1].Value = waybillno;
            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql, parameters);

            if (ds == null || ds.Tables.Count <= 0)
            {
                return "";
            }
            else
            {
                if (ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return "";
                }
                else
                {
                    return ds.Tables[0].Rows[0]["DistributionCode"].ToString();
                }
            }
        }
	}
}
