using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Domain.COD;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.COD;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Util;

namespace RFD.FMS.DAL.Oracle.COD
{
    public class FMS_CODDao : OracleDao, IFMS_CODDao
	{
		public int AddWaybillLmsToMedium(LMS_Syn_FMS_COD model)
		{
			var strSql = new StringBuilder();
			strSql.Append(@" 
                          INSERT INTO LMS_Syn_FMS_COD
                          (
                            waybillno,
                            OperateType,
                            OperateTime,
                            IsSyn,
                            StationID,
                            Createby,
                            IsChange
                          )
                          values (  :waybillno,
                                    :OperateType,
                                    :OperateTime,
                                    :IsSyn,
                                    :StationID,
                                    :Createby,
                                     3
                                  )  ");
			OracleParameter[] parameters = {
					new OracleParameter(":waybillno", OracleDbType.Decimal),
					new OracleParameter(":OperateType", OracleDbType.Decimal,4),
					new OracleParameter(":OperateTime", OracleDbType.Date),
                    new OracleParameter(":IsSyn", OracleDbType.Decimal),
					new OracleParameter(":StationID", OracleDbType.Decimal,4),
                    new OracleParameter(":Createby",OracleDbType.Varchar2,100) };
			parameters[0].Value = model.WayBillNO;
			parameters[1].Value = model.OperateType;
			parameters[2].Value = model.OperateTime;
			parameters[3].Value = 0;
			parameters[4].Value = model.StationID;
			parameters[5].Value = model.CreateBY;
			var rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
			return rowCount;
		}

        public int AddWaybillLmsToMediumV2(LMS_Syn_FMS_COD model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@" 
SET :num = -1
IF NOT EXISTS ( SELECT 1 FROM LMS_SYN_FMS_COD lsfc WHERE lsfc.WaybillNo=:WaybillNo AND lsfc.OperateType=:OperateType AND lsfc.StationID=:StationID AND lsfc.IsSyn NOT IN (3,4) ) 
BEGIN
      INSERT INTO LMS_Syn_FMS_COD
      (waybillno,OperateType,OperateTime,IsSyn,StationID,Createby,IsChange)
      select  :waybillno,:OperateType,:OperateTime,:IsSyn,:StationID,:Createby,3
        SET :num = 1
    END
ELSE 
BEGIN 
    SET :num = 0
END
");
            OracleParameter[] parameters = {
					new OracleParameter(":waybillno", OracleDbType.Decimal),
					new OracleParameter(":OperateType", OracleDbType.Decimal,4),
					new OracleParameter(":OperateTime", OracleDbType.Date),
                    new OracleParameter(":IsSyn", OracleDbType.Decimal),
					new OracleParameter(":StationID", OracleDbType.Decimal,4),
                    new OracleParameter(":Createby",OracleDbType.Varchar2,100),
                    new OracleParameter(":num",OracleDbType.Decimal)};
            parameters[0].Value = model.WayBillNO;
            parameters[1].Value = model.OperateType;
            parameters[2].Value = model.OperateTime;
            parameters[3].Value = 0;
            parameters[4].Value = model.StationID;
            parameters[5].Value = model.CreateBY;
            parameters[6].Direction = ParameterDirection.Output;
            OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            var n = (int)parameters[6].Value;
            return n;
        }

		public int UpDateMediumForSyn(string ids, int isSyn)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@" 
                          UPDATE LMS_Syn_FMS_COD
                          SET
                            IsSyn = {0},
                            SynTime = SysDate,
                            IsChange=3
                          WHERE
                               ID in ({1}) ", isSyn, ids);
			var rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString());
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
                DECLARE :etime DATETIME
                SET :etime = DATEADD(minute, {0}, SysDate)
                SELECT 
                ID
                FROM LMS_Syn_FMS_COD syn (readpast) 
                where 
                    syn.operatetime < :etime 
                and syn.IsSyn = {2} 
                and syn.OperateType<6  
                and syn.StationID>0
                and RowNum <= {1}
               ", -3, topNumber, synType);
			var dt = OracleHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

        public DataTable SearchIdsForShipBySynId(string SynIds)
        {
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
                SELECT
                ID
                FROM LMS_RFD.dbo.LMS_Syn_FMS_COD syn 
                where syn.IsSyn = 0
                and syn.OperateType<6  
                and syn.StationID>0
                and syn.ID in ({0})
               ", SynIds);
            var dt = OracleHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
            return dt;
        }

		public DataTable SearchIdsForBack(string topNumber, string synType)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
DECLARE :etime DATETIME
            SET :etime = DATEADD(minute, {0}, SysDate)
SELECT syn6.id,
       syn6.waybillno
FROM LMS_Syn_FMS_COD syn6  
where syn6.operatetime<:etime 
and  syn6.IsSyn = {2} 
and syn6.OperateType = 6  
and syn6.StationID>0 
and RowNum <= {1}
               ", -3, topNumber, synType);
			var dt = OracleHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public DataTable SearchIdsForBackAdvanced(string topNumber, string synType)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
                SELECT syn7.id,syn7.waybillno
                FROM LMS_Syn_FMS_COD syn7 (readpast) 
                where syn7.IsSyn = {1} 
                    and syn7.OperateType = 7  
                    and syn7.StationID>0
                    and RowNum <= {0}
               ", topNumber, synType);
			var dt = OracleHelper.ExecuteDataset(ConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public DataTable SearchWaybillnosForBack(string topNumber)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(
                @"
select  ROW_NUMBER() over(order by CreatTime) as ID,
                       CreatTime,
                       WaybillNO,
                       backstatus,
                       Sources,
                       MerchantID,
                       ReturnWareHouse,
                       ReturnTime,
                       ReturnExpressCompanyId
                FROM   Waybill 
                WHERE  CreatTime > '2012-01-01'
                       and backstatus in (6,7)
                       and merchantid<>8
                       and merchantid<>9

                 ");
			var dt = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
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
            FROM Waybill wb 
            join LMS_Syn_FMS_COD syn(readpast) on syn.waybillno=wb.waybillno  and syn.ID in ({0})
            JOIN ExpressCompany ec ON  ec.ExpressCompanyID = syn.StationID
            JOIN WaybillSignInfo wsi ON  wsi.WaybillNO = wb.WaybillNO
            JOIN WaybillInfo wi ON  wi.WaybillNO = wb.WaybillNO
            JOIN waybilltakesendinfo wbtsi ON  wbtsi.WaybillNO = wb.WaybillNO
          
            left join (select p.ProvinceName,c.CityName,a.AreaName,a.AreaID
            from Province p  join City c on p.ProvinceID=c.ProvinceID join Area a on c.CityID=a.CityID
            AND a.IsDeleted=0 AND c.IsDeleted=0 AND p.IsDeleted=0 )
            pca on wbtsi.ReceiveArea=pca.AreaName and wbtsi.Receivecity=pca.CityName and wbtsi.ReceiveProvince=pca.ProvinceName

            LEFT JOIN (SELECT ob.WaybillNO,ob.OutBoundStation
             FROM OutBound ob  
            JOIN ExpressCompany ex ON  ex.ExpressCompanyID = ob.OutBoundStation AND ex.DistributionCode = 'rfd'
            AND ob.OutStationType IN (3, 0)
            ) obex ON syn.WaybillNo = obex.WaybillNo
               ", ids);
			var dt = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
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
            wb.WaybillNO,
            wb.WaybillType,
            wb.BackStatus,
            wb.sources,
            wb.MerchantID,
            wb.ReturnWareHouse,
            CASE WHEN ec1.CompanyFlag=2 THEN ec2.ExpressCompanyID ELSE wb.[ReturnExpressCompanyId] END ReturnExpressCompanyId,
            wb.ReturnTime,
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
            FROM  Waybill wb 
            join LMS_Syn_FMS_COD syn(readpast) on syn.waybillno=wb.waybillno and syn.ID in ( {0} )
            JOIN WaybillSignInfo wsi ON  wsi.WaybillNO = wb.WaybillNO
            JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON ec.ExpressCompanyID=wb.DeliverStationID
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec1(NOLOCK) ON ec1.ExpressCompanyID=wb.ReturnExpressCompanyId AND ec1.CompanyFlag=2
            LEFT JOIN RFD_PMS.dbo.ExpressCompany ec2(NOLOCK) ON ec2.ExpressCompanyID=ec1.ParentID AND ec2.CompanyFlag=1
            ", ids);
			var dt = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public DataTable SearchInfoForBackAdvanced(string ids)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(
                @"
             SELECT 
            wb.WaybillNO,
            wb.ComeFrom,
            syn.OperateType,
            syn.OperateTime,
            syn.ID,
            syn.StationID,
            syn.IsSyn,
            syn.SynTime,
            syn.CreateBy,
            ob.OutBoundStation
            FROM  Waybill wb 
            join LMS_Syn_FMS_COD syn(readpast) on syn.waybillno=wb.waybillno and syn.ID in  ( {0} )
            left JOIN outbound  ob  on  ob.outboundid=wb.outboundid
            ", ids);
			var dt = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, strSql.ToString()).Tables[0];
			return dt;
		}

		public int InsertForShip(FMS_CODBaseInfo model)
		{
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"
declare v_count decimal;
begin
 :num := -1;
 SELECT  count(*) into v_count FROM    FMS_CODBaseInfo WHERE   MediumID = :MediumID ;
 if(v_count<=0) then
    BEGIN
        INSERT  INTO  FMS_CODBaseInfo
                ( InfoID,
                  MediumID,
                  WaybillNO,
                  MerchantID,
                  WaybillType,
                  Flag,
                  DeliverStationID,
                  TopCODCompanyID,
                  WarehouseId,
                  ExpressCompanyID,
                  RfdAcceptTime,
                  RfdAcceptDate,
                  FinalExpressCompanyID,
                  DeliverTime,
                  DeliverDate,
                  TotalAmount,
                  PaidAmount,
                  NeedPayAmount,
                  BackAmount,
                  NeedBackAmount,
                  AccountWeight,
                  AreaID,
                  BoxsNo,
                  Address,
                  CreateTime,
                  IsFare,
                  OperateType,
                  ProtectedPrice,
                  DistributionCode,
                  CurrentDistributionCode,
                  AreaType,
                  IsChange,
                  ComeFrom,
                  IsCOD,
                  IsDeleted 
                )
                SELECT  :InfoID,
                        :MediumID ,
                        :WaybillNO ,
                        :MerchantID ,
                        :WaybillType ,
                        :Flag ,
                        :DeliverStationID ,
                        :TopCODCompanyID ,
                        :WarehouseId ,
                        :ExpressCompanyID ,
                        :RfdAcceptTime ,
                        :RfdAcceptDate ,
                        :FinalExpressCompanyID ,
                        :DeliverTime ,
                        :DeliverDate ,
                        :TotalAmount ,
                        :PaidAmount ,
                        :NeedPayAmount ,
                        :BackAmount ,
                        :NeedBackAmount ,
                        :AccountWeight ,
                        :AreaID ,
                        :BoxsNo ,
                        :Address ,
                        SysDate ,
                        :IsFare ,
                        :OperateType ,
                        :ProtectedPrice ,
                        :DistributionCode ,
                        :CurrentDistributionCode,
                        :AreaType,
                        :IsChange,
                        :ComeFrom,
                        :IsCOD,
                        0
                        FROM dual;
        :num := 1;
    END;
    ELSE 
    BEGIN 
        :num := 0;
    END;
 end if;
end;
           ");
            OracleParameter[] parameters = 
            {
                new OracleParameter(":MediumID", OracleDbType.Decimal),
                new OracleParameter(":waybillno", OracleDbType.Decimal),
                new OracleParameter(":MerchantID", OracleDbType.Decimal),
                new OracleParameter(":WaybillType", OracleDbType.Varchar2),
                new OracleParameter(":Flag", OracleDbType.Decimal),            
                new OracleParameter(":DeliverStationID", OracleDbType.Decimal),
                new OracleParameter(":TopCODCompanyID", OracleDbType.Decimal),
                new OracleParameter(":WarehouseId", OracleDbType.Varchar2,40),
                new OracleParameter(":ExpressCompanyID", OracleDbType.Decimal),
                new OracleParameter(":RfdAcceptTime", OracleDbType.Date),
                new OracleParameter(":RfdAcceptDate", OracleDbType.Date),
                new OracleParameter(":FinalExpressCompanyID", OracleDbType.Decimal),
                new OracleParameter(":DeliverTime", OracleDbType.Date),
                new OracleParameter(":DeliverDate", OracleDbType.Date),
                new OracleParameter(":TotalAmount", OracleDbType.Decimal),
                new OracleParameter(":PaidAmount", OracleDbType.Decimal),
                new OracleParameter(":NeedPayAmount", OracleDbType.Decimal),
                new OracleParameter(":BackAmount", OracleDbType.Decimal),
                new OracleParameter(":NeedBackAmount", OracleDbType.Decimal),
                new OracleParameter(":AccountWeight", OracleDbType.Decimal),
                new OracleParameter(":AreaID", OracleDbType.Varchar2,100),
                new OracleParameter(":BoxsNo", OracleDbType.Varchar2,60),
                new OracleParameter(":Address", OracleDbType.Varchar2,300),
                new OracleParameter(":IsFare",OracleDbType.Decimal,4),
                new OracleParameter(":OperateType",OracleDbType.Decimal,4),
                new OracleParameter(":ProtectedPrice", OracleDbType.Decimal),
                new OracleParameter(":DistributionCode",OracleDbType.Varchar2,100),
                new OracleParameter(":CurrentDistributionCode",OracleDbType.Varchar2,100),
                new OracleParameter(":AreaType",OracleDbType.Decimal), 
                new OracleParameter(":IsChange",OracleDbType.Decimal),
                new OracleParameter(":num",OracleDbType.Decimal),
                new OracleParameter(":ComeFrom",OracleDbType.Decimal),
                new OracleParameter(":IsCOD",OracleDbType.Decimal),
                new OracleParameter(":InfoID",OracleDbType.Decimal),
            };
            model.ID = GetIdNew("SEQ_FMS_CODBASEINFO");
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
            parameters[33].Value = model.ID;
            var rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

            var n = DataConvert.ToInt(parameters[30].Value);

            return n;
		}

		public int UpdateForBack(FMS_CODBaseInfo model)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
            UPDATE FMS_CODBaseInfo
            SET ReturnWareHouseID= :ReturnWareHouseID,
                ReturnExpressCompanyID= :ReturnExpressCompanyID,
                UpdateTime= SysDate,
                ReturnTime= :ReturnTime,
                ReturnDate= :ReturnDate,
                BackAmount= :BackAmount,
                IsChange= :IsChange,
                DeliverStationID=:DeliverStationID,
                TopCODCompanyID=:TopCODCompanyID
            WHERE MediumID= :MediumID
            ");
			OracleParameter[] parameters = 
            {
                new OracleParameter(":ReturnWareHouseID", OracleDbType.Varchar2,40),
                new OracleParameter(":ReturnExpressCompanyID", OracleDbType.Decimal,4),
                new OracleParameter(":ReturnTime", OracleDbType.Date),
                new OracleParameter(":ReturnDate", OracleDbType.Date),
                new OracleParameter(":MediumID", OracleDbType.Decimal),
                new OracleParameter(":BackAmount",OracleDbType.Decimal),
                new OracleParameter(":IsChange",OracleDbType.Decimal),
                new OracleParameter(":DeliverStationID",OracleDbType.Decimal),
                new OracleParameter(":TopCODCompanyID",OracleDbType.Decimal)
            };
			parameters[0].Value = model.ReturnWareHouseID;
			parameters[1].Value = model.ReturnExpressCompanyID;
            parameters[2].Value = DateTime.Parse(DataConvert.ToDateTime(model.ReturnTime).ToString());
            parameters[3].Value = DateTime.Parse(DataConvert.ToDateTime(model.ReturnDate).ToString());
			parameters[4].Value = model.MediumID;
		    parameters[5].Value = model.BackAmount;
            parameters[6].Value = 1;
            parameters[7].Value = model.DeliverStationID;
            parameters[8].Value = model.TopCODCompanyID;

			var rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);

			return rowCount;
		}

		public int AdvancedUpdateForBack(FMS_CODBaseInfo model)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@"
            begin
                :n :=0;
                UPDATE FMS_CODBaseInfo
                SET DeliverTime= :DeliverTime,
                    DeliverDate= to_date(to_char(:DeliverDate,'yyyy-mm-dd'),'yyyy-mm-dd'),
                    UpdateTime= SysDate,
                    IsChange= 1
                WHERE DeliverTime is null and WaybillNO= :WaybillNO;
                :n := :n+sql%rowcount;
                UPDATE FMS_CODBaseInfo
                SET UpdateTime= SysDate,
                    FinalExpressCompanyID= :FinalExpressCompanyID,
                    IsChange=1
                WHERE FinalExpressCompanyID is null and WaybillNO= :WaybillNO;
                :n := :n+sql%rowcount;
                UPDATE FMS_CODBaseInfo
                SET AccountWeight = :AccountWeight,IsChange=1 WHERE WaybillNO= :WaybillNO and MerchantID NOT IN (8,9);
                :n := :n+sql%rowcount;
            end;
            ");
			OracleParameter[] parameters = {
                                            new OracleParameter(":DeliverTime", OracleDbType.Date),
                                            new OracleParameter(":DeliverDate", OracleDbType.Date),
                                            new OracleParameter(":FinalExpressCompanyID", OracleDbType.Decimal),
                                            new OracleParameter(":WaybillNO", OracleDbType.Decimal),
                                            new OracleParameter(":AccountWeight", OracleDbType.Decimal),
                                            new OracleParameter(":n", OracleDbType.Decimal)
                                        };
			parameters[0].Value = model.DeliverTime;
			parameters[1].Value = model.DeliverDate;
			parameters[2].Value = model.FinalExpressCompanyID;
			parameters[3].Value = model.WaybillNO;
            parameters[4].Value = model.AccountWeight;
            parameters[5].Direction = ParameterDirection.Output;
			OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString(), parameters);
            var rowCount = DataConvert.ToInt(parameters[5].Value,0);
			return rowCount;
        }

        public long SearchUpdateMediumId(long waybillno, string waybillType, string backStatus)
		{
            string sql = "";
            if (backStatus== "7")
            {
                sql = @"SELECT ID] as MediumID
                FROM  LMS_SYN_FMS_COD lsfc
                where 
                lsfc.waybillno = :WaybillNO
                and operateType = 4
                AND lsfc.IsSyn NOT IN (3,4)
                order by operatetime DESC";
            }
            else
            {
                sql = @"SELECT top 1 ID as MediumID
                FROM  LMS_SYN_FMS_COD lsfc
                where 
                lsfc.waybillno = :WaybillNO
                and operateType in (1,3)
                AND lsfc.IsSyn NOT IN (3,4)
                order by operatetime DESC";
            }

            OracleParameter[] parameters ={
                                           new OracleParameter(":WaybillNO",OracleDbType.Decimal)
                                      };
            parameters[0].Value = waybillno;
            var dt = OracleHelper.ExecuteDataset(Connection, CommandType.Text, sql,parameters).Tables[0];
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
            FROM  FMS_CODBaseInfo 
            where 
            waybillno = {0}
            order by CreateTime] desc
            ", waybillno);
			var dt = OracleHelper.ExecuteDataset(Connection, CommandType.Text, strSql.ToString()).Tables[0];
			if (dt == null || dt.Rows.Count <= 0)
			{
				return 0;
			}
			var num = Convert.ToInt64(dt.Rows[0][0]);
			return num;
		}

		public DataTable SearchAnyInfoOnMedium(string sql)
		{
			var dt = OracleHelper.ExecuteDataset(ConnString, CommandType.Text, sql).Tables[0];
			return dt;
		}

		public int UpdateDelete(string sql)
		{
			return OracleHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql);
		}

        public bool UpdateForInvalid(long waybillNo)
        {
            string sql = @"update fms_codbaseinfo set DeliverTime = sysdate
                    ,DeliverDate = to_date(to_char(sysdate,'yyyy-mm-dd'),'yyyy-mm-dd') 
                    where  DeliverTime is  null and waybillno =:waybillno";
            OracleParameter[] parameters ={
                                           new OracleParameter(":waybillno",OracleDbType.Decimal)
                                      };
            parameters[0].Value = waybillNo;
            return OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters) > 0;
        }

		public DataTable SearchInfoForDeliverTimeAndOutBountStation(string ids)
		{
//            select 
//     lsfc.ID, 
//     lsfc.waybillno,
//     lsfc.OperateType,
//     lsfc.StationID,
//     ob.OutBoundStation,
//     wb.DeliverTime
//from LMS_SYN_FMS_COD lsfc  
//JOIN Waybill wb  on wb.WaybillNO = lsfc.WaybillNo 
//     and lsfc.ID in ({0})
//JOIN OutBound ob  on ob.OutBoundID = wb.OutBoundID
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
FROM   LMS_SYN_FMS_COD lsfc
       JOIN Waybill wb
            ON  wb.WaybillNO = lsfc.WaybillNo
            AND lsfc.ID IN ({0})
       JOIN WaybillInfo wi ON wi.WaybillNO = wb.WaybillNO
       JOIN outbound ob
            ON  lsfc.WaybillNo = ob.WaybillNo
            AND ob.OutStationType IN (3, 0)
            AND lsfc.OperateType = 7
       JOIN ExpressCompany ex
            ON  ex.ExpressCompanyID = ob.OutBoundStation
            AND ex.DistributionCode = 'rfd'
            ", ids)  ;
			var dt = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql.ToString()).Tables[0];
			return dt;
		}

//        public DataTable SearchIdsForDeliverTimeAndOutBountStation(string topNumber, string synType)
//        {
//            var sql = new StringBuilder();
//            sql.AppendFormat(@"
//DECLARE :etime DATETIME
//SET :etime = DATEADD(minute, -3, SysDate)      
//select top {0} 
//     lsfc.ID
//from LMS_SYN_FMS_COD lsfc  
//where
//      lsfc.OperateType =7 
//     and lsfc.IsSyn ={1}
//     and lsfc.StationID>0 
//    and lsfc.operatetime < :etime 
//", topNumber,synType);
//            var dt = OracleHelper.ExecuteDataset(ConnString, CommandType.Text, sql.ToString()).Tables[0];
//            return dt;
//        }

        public DataTable SearchIdsForDeliverTimeAndOutBountStation(string topNumber, string synType)
        {
            var sql = new StringBuilder();
            sql.Append(@"     
            select top (:num)
                 lsfc.ID
            from LMS_SYN_FMS_COD lsfc  
            where
                  lsfc.OperateType = :OperateType
                 and lsfc.IsSyn = :IsSyn
                 and lsfc.StationID> :StationID 
                 and lsfc.operatetime < :SubTime 
            ");
            OracleParameter[] parameters =
            {
              new OracleParameter(":num",OracleDbType.Decimal),
              new OracleParameter(":OperateType",OracleDbType.Decimal,4),
              new OracleParameter(":IsSyn",OracleDbType.Decimal,4), 
              new OracleParameter(":StationID",OracleDbType.Decimal,4),
              new OracleParameter(":SubTime",OracleDbType.Date)   
            };
            parameters[0].Value = Convert.ToInt32(topNumber);
            parameters[1].Value = 7;
            parameters[2].Value = Convert.ToInt32(synType);
            parameters[3].Value = 0;
            parameters[4].Value = Convert.ToDateTime(DateTime.Now.AddMinutes(-30));
            var dt = OracleHelper.ExecuteDataset(ConnString, CommandType.Text, sql.ToString(),parameters).Tables[0];
            return dt;
        }


	    public int UpdateWaybillForCOD(long waybillno, int issyn)
		{
			var strSql = new StringBuilder();
			strSql.AppendFormat(@" 
                          UPDATE waybill
                          SET
                            IsSyn = {0},
                            SynTime = SysDate,
                            IsChange=3
                          WHERE
                               waybillno in ({1}) ", issyn, waybillno);
			var rowCount = OracleHelper.ExecuteNonQuery(Connection, CommandType.Text, strSql.ToString());
			return rowCount;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetStationByIDNOTwo(long id, long waybillno)
        {
            string sql = @"SELECT TOP 1 DistributionCode FROM LMS_SYN_FMS_COD lsfc 
JOIN ExpressCompany ec ON lsfc.StationID=ec.ExpressCompanyID
WHERE lsfc.ID>:ID AND OperateType=3 AND WaybillNo=:WaybillNo
ORDER BY ID";
            OracleParameter[] parameters = {
                                            new OracleParameter(":ID",OracleDbType.Decimal), 
                                            new OracleParameter(":WaybillNo",OracleDbType.Decimal), 
                                        };
            parameters[0].Value = id;
            parameters[1].Value = waybillno;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql, parameters);

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
            string sql = @"SELECT DistributionCode,ID FROM LMS_SYN_FMS_COD lsfc 
JOIN ExpressCompany ec ON lsfc.StationID=ec.ExpressCompanyID
WHERE lsfc.ID<:ID AND OperateType=2 AND WaybillNo=:WaybillNo 
ORDER BY ID DESC";
            OracleParameter[] parameters = {
                                            new OracleParameter(":ID",OracleDbType.Decimal), 
                                            new OracleParameter(":WaybillNo",OracleDbType.Decimal), 
                                        };
            parameters[0].Value = id;
            parameters[1].Value = waybillno;
            DataSet ds = OracleHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql, parameters);

            if (ds == null || ds.Tables.Count <= 0)
            {
                return "";
            }
            else
            {
                if (ds.Tables[0]== null || ds.Tables[0].Rows.Count <= 0)
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
