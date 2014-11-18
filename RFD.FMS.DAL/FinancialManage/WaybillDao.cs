using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.Util;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.FinancialManage;

namespace RFD.FMS.DAL.FinancialManage
{
    public class WaybillDao : SqlServerDao, IWaybillDao
    {
        public DataTable GetThirdWaybillDetails(ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref PageInfo pi, out DataTable amount)
        {
            #region 显示主要列

            var basicDetails = new StringBuilder();

            basicDetails.Append(@"  select
                wb.WaybillNo 运单号,
                wb.CustomerOrder 订单号,
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=2 AND CAST(StatusNO AS nvarchar(20))= wb.WaybillType) 运单类型,
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=1 and StatusNO=wb.Status) 运单状态,
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=5 AND CAST(StatusNO AS INT)= wb.BackStatus) 返货状态, 
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=308 AND CAST(StatusNO AS INT)= wb.InefficacyStatus) 无效状态, 
                wbtsi.ReceiveProvince 省份,
                wbtsi.ReceiveCity 城市,
                wbtsi.ReceiveArea 区县,
                wbtsi.ReceiveAddress 地址,
                mbi.merchantname  商家,
                ec.CompanyName 配送站,
                dc.Distributionname 配送商,
                ec2.companyname 发货分拣中心,
                isnull(wi.WayBillInfoWeight,0) 重量,
                isnull(wi.WayBillInfoVolumeWeight,0) 体积重量,
                case when isnull(wi.WayBillInfoWeight,0)>isnull(wi.WayBillInfoVolumeWeight,0) then isnull(wi.WayBillInfoWeight,0) else isnull(wi.WayBillInfoVolumeWeight,0) end 结算重量,
                wsi.NeedAmount 应收款,
                case wb.waybilltype  when '1' then  isnull(wsi.NeedBackAmount,0 )  when '2' then  isnull(wsi.returncash,0)  else  isnull(wsi.returncash,0)  end as 应退金额, 
                isnull(wsi.protectedprice,0) 保价金额,
                wsi.acceptType 支付方式, 
                case when wbs.SignStatus=4 then F.StatusName when wbs.SignStatus=5 then G.StatusName else '' end 滞留拒收原因,
                wb.CreatTime  接单时间,
                wbs.CreatTime 归班时间,
                wb.returntime 拒收入库时间,
                ael1.AreaType 收入区域类型
                ");
            #endregion 显示主要列

            #region 关联的主要表

            var relationDetails = new StringBuilder();

            relationDetails.Append(@" FROM  LMS_RFD.dbo.waybill wb (NOLOCK) ");

            relationDetails.Append(@" 
                JOIN RFD_PMS.dbo.MerchantBaseInfo mbi (NOLOCK) ON mbi.id = wb.merchantid ");

            relationDetails.Append(string.Format(@" and wb.sources=2 "));

            if (!string.IsNullOrEmpty(conditions.WaybillNO.ToString()) && conditions.WaybillNO > 0)
            {
                relationDetails.Append(string.Format(" And wb.waybillno = {0}", conditions.WaybillNO));
            }
            else if (!string.IsNullOrEmpty(conditions.Customerorder))
            {
                relationDetails.Append(string.Format(" And wb.CustomerOrder = '{0}'", conditions.Customerorder));
            }
            else
            {
                if (!string.IsNullOrEmpty(conditions.MerchantID))
                {
                    relationDetails.Append(string.Format(" And wb.Merchantid in ({0}) ", conditions.MerchantID));
                }

                if (!string.IsNullOrEmpty(conditions.OutCreatTimeBegin.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.creattime > '{0}' ", conditions.OutCreatTimeBegin));
                }

                if (!string.IsNullOrEmpty(conditions.OutCreatTimeEnd.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.creattime < '{0}' ", conditions.OutCreatTimeEnd));
                }

                if (!string.IsNullOrEmpty(conditions.InCreatTimeBegin.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.ReturnTime > '{0}' ", conditions.InCreatTimeBegin));
                }
                if (!string.IsNullOrEmpty(conditions.InCreatTimeEnd.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.ReturnTime < '{0}' ", conditions.InCreatTimeEnd));
                }

                if (!string.IsNullOrEmpty(conditions.SortingCenter))
                {
                    relationDetails.Append(string.Format(@"And wb.creatstation in ({0}) ", conditions.SortingCenter));
                }
                if (!string.IsNullOrEmpty(conditions.DeliverStationID))
                {
                    relationDetails.Append(string.Format(" and wb.DeliverStationID in ({0}) ", conditions.DeliverStationID));
                }
                if (!string.IsNullOrEmpty(conditions.WaybillStatus))
                {
                    relationDetails.Append(string.Format(" and wb.status = '{0}' ", conditions.WaybillStatus));
                }
                if (!string.IsNullOrEmpty(conditions.BackStatus))
                {
                    relationDetails.Append(string.Format(" AND wb.BackStatus = {0} ", Convert.ToInt32(conditions.BackStatus)));
                }
                if (!string.IsNullOrEmpty(conditions.WaybillType))
                {
                    relationDetails.Append(string.Format(" And wb.waybilltype = '{0}' ", conditions.WaybillType));
                }
                if (!string.IsNullOrEmpty(conditions.InefficacyStatus.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.InefficacyStatus = {0} ", conditions.InefficacyStatus));
                }
            }

            relationDetails.Append(@" 
                JOIN RFD_PMS.dbo.ExpressCompany ec2 (NOLOCK) on ec2.expresscompanyid = wb.creatstation
                JOIN RFD_PMS.dbo.ExpressCompany ec (NOLOCK) ON ec.ExpressCompanyID = wb.DeliverStationID ");

            relationDetails.Append(@"
                JOIN RFD_PMS.dbo.Distribution dc (NOLOCK) ON dc.Distributioncode=ec.Distributioncode
                JOIN LMS_RFD.dbo.WaybillInfo wi (NOLOCK) ON wi.WaybillNO = wb.WaybillNO 
                JOIN LMS_RFD.dbo.WaybillSignInfo wsi (NOLOCK) ON wsi.WaybillNO = wb.WaybillNO ");

            if (!string.IsNullOrEmpty(conditions.PaymentType))
            {
                relationDetails.Append(string.Format(" and wsi.AcceptType = '{0}' ", conditions.PaymentType));
            }

            if (!string.IsNullOrEmpty(conditions.BackStationTimeBegin.ToString())
                || !string.IsNullOrEmpty(conditions.BackStationTimeEnd.ToString()))
            {
                relationDetails.Append(@" join LMS_RFD.dbo.waybillBackStation wbs (nolock) on wbs.WaybillBackStationID = wsi.BackStationInofID ");
                relationDetails.Append(string.Format(" And wbs.CreatTime >'{0}' ", conditions.BackStationTimeBegin));
                relationDetails.Append(string.Format(" And wbs.CreatTime <'{0}' ", conditions.BackStationTimeEnd));
            }
            else
            {
                relationDetails.Append(" left join LMS_RFD.dbo.waybillBackStation wbs (nolock) on wbs.WaybillBackStationID = wsi.BackStationInofID ");
            }
            relationDetails.Append(@"
                 left join RFD_PMS.dbo.StatusInfo (nolock) F ON CAST(F.statusNo AS nvarchar(20))=wbs.remark and F.StatusTypeNO=21
                 left join RFD_PMS.dbo.StatusInfo (nolock) G ON CAST(G.statusNo AS nvarchar(20))=wbs.remark and G.StatusTypeNO=20");

            relationDetails.Append(
                @"  
                JOIN LMS_RFD.dbo.waybilltakesendinfo wbtsi (nolock) on wbtsi.WaybillNO=wb.WaybillNO
                LEFT JOIN (
                    SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a2.AreaID,a2.AreaName
					FROM RFD_PMS.dbo.Province AS p(NOLOCK) JOIN RFD_PMS.dbo.City AS c(NOLOCK) ON p.ProvinceID = c.ProvinceID AND p.IsDeleted=0 AND c.IsDeleted=0
                    JOIN RFD_PMS.dbo.Area AS a2(NOLOCK) ON c.CityID = a2.CityID AND a2.IsDeleted=0
				) pca ON pca.ProvinceName=wbtsi.ReceiveProvince AND pca.CityName=wbtsi.ReceiveCity AND pca.AreaName=wbtsi.ReceiveArea 
");

            relationDetails.Append(@" 
                LEFT JOIN AreaExpressLevelIncome ael1 (NOLOCK) ON ael1.AreaID = pca.AreaID
                                                         AND ael1.[Enable] IN (1,2) 
                                                         AND ael1.MerchantID=wb.MerchantID
                                                         AND ael1.WareHouseID=wb.creatstation
            ");
            relationDetails.Append(@" WHERE (1=1) ");
            if (!string.IsNullOrEmpty(conditions.AreaExpressLevel.ToString()))
            {
                if (conditions.AreaExpressLevel == 99)
                {
                    relationDetails.Append(@" AND ael1.AreaType IS NULL ");
                }
                else if (conditions.AreaExpressLevel > 0)
                    relationDetails.Append(string.Format(@" AND ael1.AreaType={0}", conditions.AreaExpressLevel.ToString()));
            }

            #endregion 关联的主要表

            amount = StatisticsForDetails(conditions);

            #region 查询显示的结果

            var totalSql = new StringBuilder();

            totalSql.Append(basicDetails);

            totalSql.Append(relationDetails);

            int itemCount = Convert.ToInt32(amount.Rows[0][0]);

            string resultSql = "";

            if (pageOrNo == true)
            {
                pi.SetItemCount(itemCount);
                int begin = pi.CurrentPageBeginItemIndex;
                int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;
                if (begin > 1)
                {
                    resultSql = String.Format(" SELECT * FROM ( SELECT  ROW_NUMBER() over (ORDER BY 运单号 asc) as 序号,allrecord.* FROM ( " + totalSql.ToString() + "  ) allrecord ) allrecordrowno WHERE 序号>={0} AND 序号<{1} ", begin, end);
                }
                else
                {
                    resultSql = String.Format(" SELECT * FROM (  SELECT  ROW_NUMBER() over (ORDER BY 运单号 asc) as 序号,allrecord.* FROM ( " + totalSql.ToString() + "  ) allrecord ) allrecordrowno WHERE 序号<{0} ", end);
                }

            }
            else
            {
                resultSql = totalSql.ToString();
            }

            return SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, resultSql.ToString()).Tables[0];

            #endregion 查询显示的结果
        }

        public DataTable StatisticsForDetails(ThirdPartyWaybillSearchConditons conditions)
        {
            #region 关联的主要表

            var relationDetails = new StringBuilder();

            relationDetails.Append(@" FROM  LMS_RFD.dbo.waybill wb (NOLOCK) ");

            relationDetails.Append(@" 
                JOIN RFD_PMS.dbo.MerchantBaseInfo mbi (NOLOCK) ON mbi.id = wb.merchantid ");

            relationDetails.Append(string.Format(@" and wb.sources=2 "));

            if (!string.IsNullOrEmpty(conditions.WaybillNO.ToString()) && conditions.WaybillNO > 0)
            {
                relationDetails.Append(string.Format(" And wb.waybillno = {0}", conditions.WaybillNO));
            }
            else if (!string.IsNullOrEmpty(conditions.Customerorder))
            {
                relationDetails.Append(string.Format(" And wb.CustomerOrder = '{0}'", conditions.Customerorder));
            }
            else
            {
                if (!string.IsNullOrEmpty(conditions.MerchantID))
                {
                    relationDetails.Append(string.Format(" And wb.Merchantid in ({0}) ", conditions.MerchantID));
                }

                if (!string.IsNullOrEmpty(conditions.OutCreatTimeBegin.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.creattime > '{0}' ", conditions.OutCreatTimeBegin));
                }

                if (!string.IsNullOrEmpty(conditions.OutCreatTimeEnd.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.creattime < '{0}' ", conditions.OutCreatTimeEnd));
                }

                if (!string.IsNullOrEmpty(conditions.InCreatTimeBegin.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.ReturnTime > '{0}' ", conditions.InCreatTimeBegin));
                }
                if (!string.IsNullOrEmpty(conditions.InCreatTimeEnd.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.ReturnTime < '{0}' ", conditions.InCreatTimeEnd));
                }

                if (!string.IsNullOrEmpty(conditions.SortingCenter))
                {
                    relationDetails.Append(string.Format(@"And wb.creatstation in ({0}) ", conditions.SortingCenter));
                }
                if (!string.IsNullOrEmpty(conditions.DeliverStationID))
                {
                    relationDetails.Append(string.Format(" and wb.DeliverStationID in ({0}) ", conditions.DeliverStationID));
                }
                if (!string.IsNullOrEmpty(conditions.WaybillStatus))
                {
                    relationDetails.Append(string.Format(" and wb.status = '{0}' ", conditions.WaybillStatus));
                }
                if (!string.IsNullOrEmpty(conditions.BackStatus))
                {
                    relationDetails.Append(string.Format(" AND wb.BackStatus = {0} ", Convert.ToInt32(conditions.BackStatus)));
                }
                if (!string.IsNullOrEmpty(conditions.WaybillType))
                {
                    relationDetails.Append(string.Format(" And wb.waybilltype = '{0}' ", conditions.WaybillType));
                }
                if (!string.IsNullOrEmpty(conditions.InefficacyStatus.ToString()))
                {
                    relationDetails.Append(string.Format(" And wb.InefficacyStatus = {0} ", conditions.InefficacyStatus));
                }
            }

            relationDetails.Append(@" 
                JOIN RFD_PMS.dbo.ExpressCompany ec2 (NOLOCK) on ec2.expresscompanyid = wb.creatstation
                JOIN RFD_PMS.dbo.ExpressCompany ec (NOLOCK) ON ec.ExpressCompanyID = wb.DeliverStationID ");

            relationDetails.Append(@"
                JOIN RFD_PMS.dbo.Distribution dc (NOLOCK) ON dc.Distributioncode=ec.Distributioncode
                JOIN LMS_RFD.dbo.WaybillInfo wi (NOLOCK) ON wi.WaybillNO = wb.WaybillNO 
                JOIN LMS_RFD.dbo.WaybillSignInfo wsi (NOLOCK) ON wsi.WaybillNO = wb.WaybillNO ");

            if (!string.IsNullOrEmpty(conditions.PaymentType))
            {
                relationDetails.Append(string.Format(" and wsi.AcceptType = '{0}' ", conditions.PaymentType));
            }

            if (!string.IsNullOrEmpty(conditions.BackStationTimeBegin.ToString())
                || !string.IsNullOrEmpty(conditions.BackStationTimeEnd.ToString()))
            {
                relationDetails.Append(@" join LMS_RFD.dbo.waybillBackStation wbs (nolock) on wbs.WaybillBackStationID = wsi.BackStationInofID ");
                relationDetails.Append(string.Format(" And wbs.CreatTime >'{0}' ", conditions.BackStationTimeBegin));
                relationDetails.Append(string.Format(" And wbs.CreatTime <'{0}' ", conditions.BackStationTimeEnd));
            }

            relationDetails.Append(
                @"  
                JOIN LMS_RFD.dbo.waybilltakesendinfo wbtsi (nolock) on wbtsi.WaybillNO=wb.WaybillNO
				LEFT JOIN (
                    SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a2.AreaID,a2.AreaName
					FROM RFD_PMS.dbo.Province AS p(NOLOCK) JOIN RFD_PMS.dbo.City AS c(NOLOCK) ON p.ProvinceID = c.ProvinceID AND p.IsDeleted=0 AND c.IsDeleted=0
                    JOIN RFD_PMS.dbo.Area AS a2(NOLOCK) ON c.CityID = a2.CityID AND a2.IsDeleted=0
				) pca ON pca.ProvinceName=wbtsi.ReceiveProvince AND pca.CityName=wbtsi.ReceiveCity AND pca.AreaName=wbtsi.ReceiveArea ");
            relationDetails.Append(@" 
                LEFT JOIN AreaExpressLevelIncome ael1 (NOLOCK) ON ael1.AreaID = pca.AreaID
                                                         AND ael1.[Enable] IN (1,2) 
                                                         AND ael1.MerchantID=wb.MerchantID
                                                         AND ael1.WareHouseID=wb.creatstation
            ");

            relationDetails.Append(@" WHERE (1=1)");
            if (!string.IsNullOrEmpty(conditions.AreaExpressLevel.ToString()))
            {
                if (conditions.AreaExpressLevel == 99)
                {
                    relationDetails.Append(@" AND ael1.AreaType IS NULL ");
                }
                else if (conditions.AreaExpressLevel > 0)
                    relationDetails.Append(string.Format(@" AND ael1.AreaType={0}", conditions.AreaExpressLevel.ToString()));
            }
            #endregion 关联的主要表

            #region 计数查询和当前查询的合计信息

            var numberSql = new StringBuilder();

            numberSql.Append(
                @" select 
                count(DISTINCT wb.WaybillNO) 订单量合计,
                count(case when isnull(wsi.NeedAmount,0)>0 then isnull(wsi.NeedAmount,0) end) as 应收订单量合计,
                sum(isnull(wsi.NeedAmount,0)) 应收款合计,
                count(case when (wb.waybilltype='1' AND isnull(wsi.NeedBackAmount,0 )>0) THEN isnull(wsi.NeedBackAmount,0)
					when (wb.waybilltype='2' AND isnull(wsi.returncash,0 )>0) THEN isnull(wsi.returncash,0)
                 END) as 应退订单量合计,
                sum(case wb.waybilltype  when '1' then  isnull(wsi.NeedBackAmount,0 )  when '2' then  isnull(wsi.returncash,0)  else  isnull(wsi.returncash,0)  end) 应退款合计,
                sum(isnull(wsi.protectedprice,0)) 保价金额合计,
                sum(case when isnull(wi.WayBillInfoWeight,0)>isnull(wi.WayBillInfoVolumeWeight,0) then isnull(wi.WayBillInfoWeight,0) else isnull(wi.WayBillInfoVolumeWeight,0) end) 结算重量合计
                ");

            numberSql.Append(relationDetails);

            return SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, numberSql.ToString()).Tables[0];

            #endregion 计数查询和当前查询的合计信息
        }

        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable GetThirdWaybillStat(ThirdPartyWaybillSearchConditons conditions)
        {
            #region 显示主要列

            var basicDetails = new StringBuilder();

            basicDetails.Append(@"  select ");
            if (conditions.TimeType == 1)
            {
                basicDetails.Append(@"CONVERT(NVARCHAR(20),wb.creattime,111) AS 接单日期,");
            }
            if (conditions.TimeType == 3)
            {
                basicDetails.Append(@"CONVERT(NVARCHAR(20),wb.ReturnTime,111) AS 拒收入库日期,");
            }
            if (conditions.TimeType == 2)
            {
                basicDetails.Append(@"CONVERT(NVARCHAR(20),wbs.CreatTime,111) AS 归班日期,");
            }
            basicDetails.Append(@"
                mbi.merchantname  商家,
				ec.CompanyName 配送站,
                dc.Distributionname 配送商,
                s.StatusName 运单类型,
                s1.StatusName 运单状态,
                ael1.AreaType 收入区域类型,
                COUNT(wb.WaybillNO) 订单总量, 
                SUM(case when isnull(wi.WayBillInfoWeight,0)>isnull(wi.WayBillInfoVolumeWeight,0) then isnull(wi.WayBillInfoWeight,0) else isnull(wi.WayBillInfoVolumeWeight,0)END)  结算重量,
                SUM(ISNULL(wsi.NeedAmount,0)) 应收款,
                SUM(case wb.waybilltype  when '1' then  isnull(wsi.NeedBackAmount,0 )  when '2' then  isnull(wsi.returncash,0)  else  isnull(wsi.returncash,0)  END) 应退金额,
                SUM(isnull(wsi.protectedprice,0)) 保价金额,
                wsi.acceptType 支付方式, 
                ec2.companyname 发货分拣中心
                ");
            #endregion 显示主要列

            #region 关联的主要表

            var relationDetails = new StringBuilder();

            relationDetails.Append(@" FROM  LMS_RFD.dbo.waybill wb (NOLOCK) ");

            relationDetails.Append(@"
                JOIN RFD_PMS.dbo.StatusInfo s(nolock) on s.statusTypeNO=2 AND CAST(s.StatusNO AS nvarchar(20))= wb.WaybillType
                JOIN RFD_PMS.dbo.StatusInfo s1(nolock) on s1.statusTypeNO=1 and s1.StatusNO=wb.Status 
                JOIN RFD_PMS.dbo.MerchantBaseInfo mbi (NOLOCK) ON mbi.id = wb.merchantid ");

            relationDetails.Append(string.Format(@" and wb.sources=2 "));

            if (!string.IsNullOrEmpty(conditions.MerchantID))
            {
                relationDetails.Append(string.Format(" And wb.Merchantid in ({0}) ", conditions.MerchantID));
            }

            if (!string.IsNullOrEmpty(conditions.OutCreatTimeBegin.ToString()))
            {
                relationDetails.Append(string.Format(" And wb.creattime > '{0}' ", conditions.OutCreatTimeBegin));
            }

            if (!string.IsNullOrEmpty(conditions.OutCreatTimeEnd.ToString()))
            {
                relationDetails.Append(string.Format(" And wb.creattime < '{0}' ", conditions.OutCreatTimeEnd));
            }

            if (!string.IsNullOrEmpty(conditions.InCreatTimeBegin.ToString()))
            {
                relationDetails.Append(string.Format(" And wb.ReturnTime > '{0}' ", conditions.InCreatTimeBegin));
            }
            if (!string.IsNullOrEmpty(conditions.InCreatTimeEnd.ToString()))
            {
                relationDetails.Append(string.Format(" And wb.ReturnTime < '{0}' ", conditions.InCreatTimeEnd));
            }

            if (!string.IsNullOrEmpty(conditions.SortingCenter))
            {
                relationDetails.Append(string.Format(@"And wb.creatstation in ({0}) ", conditions.SortingCenter));
            }
            if (!string.IsNullOrEmpty(conditions.DeliverStationID))
            {
                relationDetails.Append(string.Format(" and wb.DeliverStationID in ({0}) ", conditions.DeliverStationID));
            }
            if (!string.IsNullOrEmpty(conditions.WaybillStatus))
            {
                relationDetails.Append(string.Format(" and wb.status = '{0}' ", conditions.WaybillStatus));
            }
            if (!string.IsNullOrEmpty(conditions.BackStatus))
            {
                relationDetails.Append(string.Format(" AND wb.BackStatus = {0} ", Convert.ToInt32(conditions.BackStatus)));
            }
            if (!string.IsNullOrEmpty(conditions.WaybillType))
            {
                relationDetails.Append(string.Format(" And wb.waybilltype = '{0}' ", conditions.WaybillType));
            }
            if (!string.IsNullOrEmpty(conditions.InefficacyStatus.ToString()))
            {
                relationDetails.Append(string.Format(" And wb.InefficacyStatus = {0} ", conditions.InefficacyStatus));
            }

            relationDetails.Append(@" 
                JOIN RFD_PMS.dbo.ExpressCompany ec2 (NOLOCK) on ec2.expresscompanyid = wb.creatstation
                JOIN RFD_PMS.dbo.ExpressCompany ec (NOLOCK) ON ec.ExpressCompanyID = wb.DeliverStationID ");

            relationDetails.Append(@"
                JOIN RFD_PMS.dbo.Distribution dc (NOLOCK) ON dc.Distributioncode=ec.Distributioncode
                JOIN LMS_RFD.dbo.WaybillInfo wi (NOLOCK) ON wi.WaybillNO = wb.WaybillNO 
                JOIN LMS_RFD.dbo.WaybillSignInfo wsi (NOLOCK) ON wsi.WaybillNO = wb.WaybillNO ");

            if (!string.IsNullOrEmpty(conditions.PaymentType))
            {
                relationDetails.Append(string.Format(" and wsi.AcceptType = '{0}' ", conditions.PaymentType));
            }

            if (!string.IsNullOrEmpty(conditions.BackStationTimeBegin.ToString())
                || !string.IsNullOrEmpty(conditions.BackStationTimeEnd.ToString()))
            {
                relationDetails.Append(@" join LMS_RFD.dbo.waybillBackStation wbs (nolock) on wbs.WaybillBackStationID = wsi.BackStationInofID ");
                relationDetails.Append(string.Format(" And wbs.CreatTime >'{0}' ", conditions.BackStationTimeBegin));
                relationDetails.Append(string.Format(" And wbs.CreatTime <'{0}' ", conditions.BackStationTimeEnd));
            }
            else
            {
                relationDetails.Append(" left join LMS_RFD.dbo.waybillBackStation wbs (nolock) on wbs.WaybillBackStationID = wsi.BackStationInofID ");
            }

            relationDetails.Append(
                @"  
                JOIN LMS_RFD.dbo.waybilltakesendinfo wbtsi (nolock) on wbtsi.WaybillNO=wb.WaybillNO
                LEFT JOIN (
                    SELECT p.ProvinceID,p.ProvinceName,c.CityID,c.CityName,a2.AreaID,a2.AreaName
					FROM RFD_PMS.dbo.Province AS p(NOLOCK) JOIN RFD_PMS.dbo.City AS c(NOLOCK) ON p.ProvinceID = c.ProvinceID
                    JOIN RFD_PMS.dbo.Area AS a2(NOLOCK) ON c.CityID = a2.CityID
				) pca ON pca.ProvinceName=wbtsi.ReceiveProvince AND pca.CityName=wbtsi.ReceiveCity AND pca.AreaName=wbtsi.ReceiveArea 
");

            relationDetails.Append(@" 
                LEFT JOIN AreaExpressLevelIncome ael1 (NOLOCK) ON ael1.AreaID = pca.AreaID
                                                         AND ael1.[Enable] IN (1,2) 
                                                         AND ael1.MerchantID=wb.MerchantID
                                                         AND ael1.WareHouseID=wb.creatstation
            ");
            relationDetails.Append(@" WHERE (1=1)");
            if (!string.IsNullOrEmpty(conditions.AreaExpressLevel.ToString()))
            {
                if (conditions.AreaExpressLevel == 99)
                {
                    relationDetails.Append(@" AND ael1.AreaType IS NULL ");
                }
                else if (conditions.AreaExpressLevel > 0)
                    relationDetails.Append(string.Format(@" AND ael1.AreaType={0}", conditions.AreaExpressLevel.ToString()));
            }

            relationDetails.Append(@" GROUP BY ");
            if (conditions.TimeType == 1)
            {
                relationDetails.Append(@"CONVERT(NVARCHAR(20),wb.creattime,111),");
            }
            if (conditions.TimeType == 3)
            {
                relationDetails.Append(@"CONVERT(NVARCHAR(20),wb.ReturnTime,111),");
            }
            if (conditions.TimeType == 2)
            {
                relationDetails.Append(@"CONVERT(NVARCHAR(20),wbs.CreatTime,111),");
            }
            relationDetails.Append(@"
                mbi.merchantname,
				ec.CompanyName,
                dc.Distributionname,
                ael1.AreaType,
                ec2.companyname,
                s.StatusName,
                s1.StatusName,
                wsi.acceptType");

            #endregion 关联的主要表

            var totalSql = new StringBuilder();

            totalSql.Append(basicDetails);

            totalSql.Append(relationDetails);

            string resultSql = totalSql.ToString();

            return SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, resultSql.ToString()).Tables[0];
        }

        public DataTable GetThirdWaybillDetailsV2(ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref PageInfo pi, out DataTable amount)
        {
            int itemCount = 0;
            amount = null;
            if (pageOrNo)
            {
                amount = Summary(conditions);
                if (amount == null || amount.Rows.Count <= 0 || int.Parse(amount.Rows[0][0].ToString()) <= 0)
                    return null;

                itemCount = amount.Rows[0][0].ToString().TryGetInt();
            }
            string resultSql = Details(conditions, pageOrNo, ref pi, itemCount);

            DataSet ds = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, resultSql.ToString());
            if (ds == null || ds.Tables.Count <= 0)
                return null;
            else
                return ds.Tables[0];
        }

        public string CreateTempTable(ThirdPartyWaybillSearchConditons conditions)
        {
            var tempTableSql = new StringBuilder();
            tempTableSql.Append(@" SELECT  
                                   wb.WaybillNO ,
                                   wb.CustomerOrder,
                                   wb.Status,
                                   wb.backStatus,
                                   wb.InefficacyStatus,
                                   wb.creatstation ,
                                   wb.DeliverStationID ,
                                   wb.MerchantID,
                                   mbi.merchantname,
                                   wb.waybilltype,
                                   wb.CreatTime,
                                   wb.returntime,
                                   wbs.CreatTime AS backstationtime ,
                                   ISNULL(wsi.NeedAmount,0) AS NeedAmount,
                                   ISNULL(wsi.NeedBackAmount,0) AS NeedBackAmount,
                                   ISNULL(wsi.returncash,0) AS returncash,
                                   ISNULL(wsi.protectedprice,0) AS protectedprice,
                                   wsi.AcceptType,
                                   wi.WayBillInfoWeight,
                                   wi.WayBillInfoVolumeWeight                            
                                    ");
            tempTableSql.Append(@" INTO #tmp_details ");
            tempTableSql.Append(@" FROM LMS_RFD.dbo.waybill wb (NOLOCK) ");
            tempTableSql.Append(@" JOIN RFD_PMS.dbo.MerchantBaseInfo mbi ( NOLOCK ) ON mbi.id = wb.merchantid AND wb.sources = 2 ");

            if (!string.IsNullOrEmpty(conditions.WaybillNO.ToString()) && conditions.WaybillNO > 0)
            { tempTableSql.Append(string.Format(" And wb.waybillno = {0} ", conditions.WaybillNO)); }
            else if (!string.IsNullOrEmpty(conditions.Customerorder))
            { tempTableSql.Append(string.Format(" And wb.CustomerOrder = '{0}' ", conditions.Customerorder)); }
            else
            {
                if (!string.IsNullOrEmpty(conditions.MerchantID))
                { tempTableSql.Append(string.Format(" And wb.Merchantid in ({0}) ", conditions.MerchantID)); }
                if (!string.IsNullOrEmpty(conditions.OutCreatTimeBegin.ToString()))
                { tempTableSql.Append(string.Format(" And wb.creattime > '{0}' ", conditions.OutCreatTimeBegin)); }
                if (!string.IsNullOrEmpty(conditions.OutCreatTimeEnd.ToString()))
                { tempTableSql.Append(string.Format(" And wb.creattime < '{0}' ", conditions.OutCreatTimeEnd)); }
                if (!string.IsNullOrEmpty(conditions.InCreatTimeBegin.ToString()))
                { tempTableSql.Append(string.Format(" And wb.ReturnTime > '{0}' ", conditions.InCreatTimeBegin)); }
                if (!string.IsNullOrEmpty(conditions.InCreatTimeEnd.ToString()))
                { tempTableSql.Append(string.Format(" And wb.ReturnTime < '{0}' ", conditions.InCreatTimeEnd)); }
                if (!string.IsNullOrEmpty(conditions.SortingCenter))
                { tempTableSql.Append(string.Format(@"And wb.creatstation in ({0}) ", conditions.SortingCenter)); }
                if (!string.IsNullOrEmpty(conditions.DeliverStationID))
                { tempTableSql.Append(string.Format(" and wb.DeliverStationID in ({0}) ", conditions.DeliverStationID)); }
                if (!string.IsNullOrEmpty(conditions.WaybillStatus))
                { tempTableSql.Append(string.Format(" and wb.status = '{0}' ", conditions.WaybillStatus)); }
                if (!string.IsNullOrEmpty(conditions.BackStatus))
                { tempTableSql.Append(string.Format(" AND wb.BackStatus = {0} ", Convert.ToInt32(conditions.BackStatus))); }
                if (!string.IsNullOrEmpty(conditions.WaybillType))
                { tempTableSql.Append(string.Format(" And wb.waybilltype = '{0}' ", conditions.WaybillType)); }
                if (!string.IsNullOrEmpty(conditions.InefficacyStatus.ToString()))
                { tempTableSql.Append(string.Format(" And wb.InefficacyStatus = {0} ", conditions.InefficacyStatus)); }
            }
            tempTableSql.Append(@" JOIN LMS_RFD.dbo.WaybillSignInfo wsi ( NOLOCK ) ON  wsi.WaybillNO = wb.WaybillNO  ");
            if (!string.IsNullOrEmpty(conditions.PaymentType))
            { tempTableSql.Append(string.Format(" and wsi.AcceptType = '{0}' ", conditions.PaymentType)); }
            if (!string.IsNullOrEmpty(conditions.BackStationTimeBegin.ToString())
               || !string.IsNullOrEmpty(conditions.BackStationTimeEnd.ToString()))
            {
                tempTableSql.Append(@" JOIN LMS_RFD.dbo.WaybillBackStation wbs (NOLOCK) ON wbs.WaybillBackStationID = wsi.BackStationInofID  ");
                tempTableSql.Append(string.Format(" And wbs.CreatTime >'{0}' ", conditions.BackStationTimeBegin));
                tempTableSql.Append(string.Format(" And wbs.CreatTime <'{0}' ", conditions.BackStationTimeEnd));
            }
            else
            { tempTableSql.Append(@"Left JOIN LMS_RFD.dbo.WaybillBackStation wbs (NOLOCK) ON wbs.WaybillBackStationID = wsi.BackStationInofID  "); }
            tempTableSql.Append(@" JOIN LMS_RFD.dbo.WaybillInfo wi ( NOLOCK ) ON wi.WaybillNO = wb.WaybillNO  ");
            tempTableSql.Append(@"  /*临时表上创建索引*/
                                    CREATE INDEX ix_tmp_details_WaybillNO ON #tmp_details(WaybillNO)  ");
            return tempTableSql.ToString();
        }

        public DataTable Summary(ThirdPartyWaybillSearchConditons conditions)
        {
            var summarySql = new StringBuilder();
            summarySql.Append(CreateTempTable(conditions));
            summarySql.Append(@" /*汇总数据*/
            SELECT  COUNT(DISTINCT wb.WaybillNO) 订单量合计 ,
                    COUNT(CASE WHEN wb.NeedAmount > 0
                               THEN wb.NeedAmount
                          END) AS 应收订单量合计 ,
                    SUM(wb.NeedAmount) 应收款合计 ,
                    COUNT(CASE WHEN ( wb.waybilltype = '1'
                                      AND wb.NeedBackAmount > 0
                                    ) THEN wb.NeedBackAmount
                               WHEN ( wb.waybilltype = '2'
                                      AND wb.returncash > 0
                                    ) THEN wb.returncash
                          END) AS 应退订单量合计 ,
                    SUM(CASE wb.waybilltype
                          WHEN '1' THEN wb.NeedBackAmount
                          WHEN '2' THEN wb.returncash
                          ELSE wb.returncash
                        END) 应退款合计 ,
                    SUM(wb.protectedprice) 保价金额合计 ,
                    SUM(case when isnull(wb.WayBillInfoWeight,0) > isnull(wb.WayBillInfoVolumeWeight,0) then isnull(wb.WayBillInfoWeight,0) else isnull(wb.WayBillInfoVolumeWeight,0) END) 结算重量合计 ");
            var relationSql = new StringBuilder();
            relationSql.Append(@" FROM  #tmp_details wb ");
            relationSql.Append(@" JOIN RFD_PMS.dbo.ExpressCompany ec2 ( NOLOCK ) ON ec2.expresscompanyid = wb.creatstation  ");
            relationSql.Append(@" JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = wb.DeliverStationID ");
            relationSql.Append(@" JOIN RFD_PMS.dbo.Distribution dc ( NOLOCK ) ON dc.Distributioncode = ec.Distributioncode ");
            relationSql.Append(@" JOIN LMS_RFD.dbo.waybilltakesendinfo wbtsi ( NOLOCK ) ON wbtsi.WaybillNO = wb.WaybillNO ");
            relationSql.Append(@"  
                            LEFT JOIN ( SELECT  p.ProvinceID ,
                                                p.ProvinceName ,
                                                c.CityID ,
                                                c.CityName ,
                                                a2.AreaID ,
                                                a2.AreaName
                                        FROM    RFD_PMS.dbo.Province AS p ( NOLOCK )
                                                JOIN RFD_PMS.dbo.City AS c ( NOLOCK ) ON p.ProvinceID = c.ProvinceID
                                                                                  AND p.IsDeleted = 0
                                                                                  AND c.IsDeleted = 0
                                                JOIN RFD_PMS.dbo.Area AS a2 ( NOLOCK ) ON c.CityID = a2.CityID
                                                                                  AND a2.IsDeleted = 0
                                      ) pca ON pca.ProvinceName = wbtsi.ReceiveProvince
                                               AND pca.CityName = wbtsi.ReceiveCity
                                               AND pca.AreaName = wbtsi.ReceiveArea
                            LEFT JOIN AreaExpressLevelIncome ael1 ( NOLOCK ) ON ael1.AreaID = pca.AreaID
                                                                                AND ael1.[Enable] IN (
                                                                                1, 2 )
                                                                                AND ael1.MerchantID = wb.MerchantID
                                                                                AND ael1.WareHouseID = wb.creatstation  ");
            relationSql.Append(@" WHERE (1=1)");
            if (!string.IsNullOrEmpty(conditions.AreaExpressLevel.ToString()))
            {
                if (conditions.AreaExpressLevel == 99)
                {
                    relationSql.Append(@" AND ael1.AreaType IS NULL ");
                }
                else if (conditions.AreaExpressLevel > 0)
                    relationSql.Append(string.Format(@" AND ael1.AreaType={0}", conditions.AreaExpressLevel.ToString()));
            }
            var sql = new StringBuilder();
            sql.Append(summarySql);
            sql.Append(relationSql);
            sql.Append(@" /*删除临时表*/   DROP TABLE      #tmp_details ");
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnString, CommandType.Text, sql.ToString());
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        public string Details(ThirdPartyWaybillSearchConditons conditions, bool pageOrNo, ref PageInfo pi, int num)
        {
            var detailsSql = new StringBuilder();
            detailsSql.Append(@"  /*明细数据*/  select
                wb.WaybillNo 运单号,
                wb.CustomerOrder 订单号,
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=2 AND CAST(StatusNO AS nvarchar(20))= wb.WaybillType) 运单类型,
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=1 and StatusNO=wb.Status) 运单状态,
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=5 AND CAST(StatusNO AS INT)= wb.BackStatus) 返货状态, 
                (select StatusName from RFD_PMS.dbo.StatusInfo s(nolock) where s.statusTypeNO=308 AND CAST(StatusNO AS INT)= wb.InefficacyStatus) 无效状态, 
                wbtsi.ReceiveProvince 省份,
                wbtsi.ReceiveCity 城市,
                wbtsi.ReceiveArea 区县,
                wbtsi.ReceiveAddress 地址,
                wb.merchantname  商家,
                ec.CompanyName 配送站,
                dc.Distributionname 配送商,
                ec2.companyname 发货分拣中心,
                wb.WayBillInfoWeight 重量,
                wb.WayBillInfoVolumeWeight 体积重量,
                case when isnull(wb.WayBillInfoWeight,0) > isnull(wb.WayBillInfoVolumeWeight,0) then isnull(wb.WayBillInfoWeight,0) else isnull(wb.WayBillInfoVolumeWeight,0) end 结算重量,
                wb.NeedAmount 应收款,
                case wb.waybilltype  when '1' then  wb.NeedBackAmount  when '2' then  wb.returncash  else  wb.returncash  end as 应退金额, 
                wb.protectedprice 保价金额,
                wb.acceptType 支付方式, 
                wb.CreatTime  接单时间,
                wb.backstationtime 归班时间,
                wb.returntime 拒收入库时间,
                ael1.AreaType 收入区域类型
                ");
            var relationSql = new StringBuilder();
            relationSql.Append(@" FROM  #tmp_details wb ");
            relationSql.Append(@" JOIN RFD_PMS.dbo.ExpressCompany ec2 ( NOLOCK ) ON ec2.expresscompanyid = wb.creatstation  ");
            relationSql.Append(@" JOIN RFD_PMS.dbo.ExpressCompany ec ( NOLOCK ) ON ec.ExpressCompanyID = wb.DeliverStationID ");
            relationSql.Append(@" JOIN RFD_PMS.dbo.Distribution dc ( NOLOCK ) ON dc.Distributioncode = ec.Distributioncode ");
            relationSql.Append(@" JOIN LMS_RFD.dbo.waybilltakesendinfo wbtsi ( NOLOCK ) ON wbtsi.WaybillNO = wb.WaybillNO ");
            relationSql.Append(@"  
                            LEFT JOIN ( SELECT  p.ProvinceID ,
                                                p.ProvinceName ,
                                                c.CityID ,
                                                c.CityName ,
                                                a2.AreaID ,
                                                a2.AreaName
                                        FROM    RFD_PMS.dbo.Province AS p ( NOLOCK )
                                                JOIN RFD_PMS.dbo.City AS c ( NOLOCK ) ON p.ProvinceID = c.ProvinceID
                                                                                  AND p.IsDeleted = 0
                                                                                  AND c.IsDeleted = 0
                                                JOIN RFD_PMS.dbo.Area AS a2 ( NOLOCK ) ON c.CityID = a2.CityID
                                                                                  AND a2.IsDeleted = 0
                                      ) pca ON pca.ProvinceName = wbtsi.ReceiveProvince
                                               AND pca.CityName = wbtsi.ReceiveCity
                                               AND pca.AreaName = wbtsi.ReceiveArea
                            LEFT JOIN AreaExpressLevelIncome ael1 ( NOLOCK ) ON ael1.AreaID = pca.AreaID
                                                                                AND ael1.[Enable] IN (
                                                                                1, 2 )
                                                                                AND ael1.MerchantID = wb.MerchantID
                                                                                AND ael1.WareHouseID = wb.creatstation  ");
            relationSql.Append(@" WHERE (1=1)");
            if (!string.IsNullOrEmpty(conditions.AreaExpressLevel.ToString()))
            {
                if (conditions.AreaExpressLevel == 99)
                {
                    relationSql.Append(@" AND ael1.AreaType IS NULL ");
                }
                else if (conditions.AreaExpressLevel > 0)
                    relationSql.Append(string.Format(@" AND ael1.AreaType={0}", conditions.AreaExpressLevel.ToString()));
            }

            var totalSql = new StringBuilder();
            totalSql.Append(detailsSql);
            totalSql.Append(relationSql);
            string resultSql;
            if (pageOrNo)
            {
                pi.SetItemCount(num);
                int begin = pi.CurrentPageBeginItemIndex;
                int end = pi.CurrentPageBeginItemIndex + pi.CurrentPageItemCount;
                if (begin > 1)
                {
                    resultSql = String.Format(" SELECT * FROM ( SELECT  ROW_NUMBER() over (ORDER BY 运单号 asc) as 序号,allrecord.* FROM ( " + totalSql + "  ) allrecord ) allrecordrowno WHERE 序号>={0} AND 序号<{1} ", begin, end);
                }
                else
                {
                    resultSql = String.Format(" SELECT * FROM (  SELECT  ROW_NUMBER() over (ORDER BY 运单号 asc) as 序号,allrecord.* FROM ( " + totalSql + "  ) allrecord ) allrecordrowno WHERE 序号<{0} ", end);
                }

            }
            else
            {
                resultSql = String.Format(" SELECT  ROW_NUMBER() over (ORDER BY 运单号 asc) as 序号,allrecord.* FROM ( " + totalSql + "  ) allrecord ");
            }

            var finalSql = new StringBuilder();
            finalSql.Append(CreateTempTable(conditions));
            finalSql.Append(resultSql);
            finalSql.Append(@" /*删除临时表*/   DROP TABLE      #tmp_details ");
            return finalSql.ToString();
        }

        public  DataTable GetWaybillStatus(DateTime sTime,DateTime eTime)
        {
            string sql = @"declare @min int
                           declare @max int
                           select @min=min(id),@max=max(id) from LMS_RFD.dbo.LMS_WaybillStatusChangeLog with(nolock,forceseek)
                            where createtime>=@StartTime
                                  and createtime<@EndTime
	                      select waybillno,status,substatus from LMS_RFD.dbo.LMS_WaybillStatusChangeLog (nolock) 
                                  where id >=@min and id<=@max  ";
            SqlParameter []parameters = {
                                          new SqlParameter("@StartTime", SqlDbType.DateTime),
                                          new SqlParameter("@EndTime", SqlDbType.DateTime)
                                      };
            parameters[0].Value = sTime;
            parameters[1].Value = eTime;
            var ds =SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql, parameters);
            return ds.Tables[0];
        }

        public DataTable GetAllSortingData(Int64 waybillNo)
        {
            string strSql =
//                                @"
//                                  select w.WaybillNo,w.WaybillType,w.MerchantID,w.DeliverStationID,w.ReturnTime,w.DistributionCode,w.ReturnExpressCompanyID,w.ReturnWarehouse,w.CreatStation,ob1.OutBoundTime,ec1.TopCODCompanyID,
//                                               wis.IntoTime As IntoStationTime,ib1.IntoTime As InboundTime,ec.CityID as CreateCityID ,ec1.CityID as SortingCityID from LMS_RFD.dbo.Waybill w(NoLock)                
//                                               Left Join LMS_RFD.dbo.WaybillIntoStation wis(nolock) on w.waybillno = wis.waybillno
//                                               Left Join ( select Top 1 * from LMS_RFD.dbo.OutBound ob(nolock) where ob.WaybillNo = @WaybillNo )ob1
//                                               on w.waybillno = ob1.waybillno  
//                                               Left Join  ( select Top 1 * from  LMS_RFD.dbo.InBound ib(nolock) where ib.WaybillNo = @WaybillNo )ib1
//                                               on w.WaybillNo = ib1.WaybillNo
//                                               Left Join RFD_PMS.dbo.ExpressCompany ec(nolock) on w.CreatStation =ec.ExpressCompanyID 
//                                               Left Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on w.DeliverStationID =ec1.ExpressCompanyID 
//                                               where w.WaybillNo = @WaybillNo";
                @" select w.WaybillNo,w.WaybillType,w.MerchantID ,w.DistributionCode,w.DeliverStationID,ec.TopCODCompanyID from LMS_RFD.dbo.Waybill w(NoLock) 
                   Left Join RFD_PMS.dbo.ExpressCompany ec(Nolock) on w.DeliverStationID = ec.ExpressCompanyID where WaybillNo = @WaybillNo";
            SqlParameter[] parameters = { new SqlParameter("@WaybillNo", SqlDbType.BigInt) };
            parameters[0].Value = waybillNo;
            var dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            if (dt.Rows.Count == 0)
                return null;
            else
            {
                return dt;
            }
        }

        public DataTable GetSortingToStation(long waybillNo)
        {
            string strSql =
                @"  
                             select w.WaybillNo,w.WaybillType,w.MerchantID,wis.IntoStation,ec.TopCODCompanyID,w.DistributionCode,
                               wis.IntoTime,ob.OutBoundStation,ec.CityID from LMS_RFD.dbo.Waybill w(NoLock)
                               Join LMS_RFD.dbo.WaybillIntoStation wis(nolock) on w.WaybillNo = wis.WaybillNo  
                               Join LMS_RFD.dbo.OutBound ob(nolock) on w.OutboundID = ob.OutboundID 
                               Join RFD_PMS.dbo.ExpressCompany ec(nolock) on w.DeliverStationID =ec.ExpressCompanyID 
                               where wis.IntoStationType IN ('0','2') and w.WaybillNo = @WaybillNo
	              union all
				  select w.WaybillNo,w.WaybillType,w.MerchantID,wis.IntoStation,ec.TopCODCompanyID,w.DistributionCode,
                               wis.IntoTime,ob.OutBoundStation,ec.CityID from LMS_RFD.dbo.Waybill w(NoLock)
                               Join LMS_RFD.dbo.WaybillIntoStation wis(nolock) on w.WaybillNo = wis.WaybillNo 
                               Join LMS_RFD.dbo.OutBound ob(nolock) on w.OutboundID = ob.OutboundID 
                               Join RFD_PMS.dbo.ExpressCompany ec(nolock) on w.DeliverStationID =ec.ExpressCompanyID 
							   Join LMS_RFD.dbo.TurnStation ts(NOLOCK) ON ts.WaybillNO=w.WaybillNO AND ts.RecipcorpStation=wis.IntoStation AND ts.IsFast=0
                               where  wis.IntoStationType = '1' and w.WaybillNo = @WaybillNo
				  
							   ";

            SqlParameter[] parameters = { new SqlParameter("@WaybillNo", SqlDbType.BigInt) };
            parameters[0].Value = waybillNo;
            var dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            if (dt.Rows.Count == 0)
                return null;
            else
            {
                return dt;
            }
        }


        public DataTable GetSortingToCity(long waybillNo)
        {
            string strSql =
                 @" select w.WaybillNo,w.WaybillType,w.MerchantID,w.DeliverStationID,ob.OutBoundStation,w.DistributionCode,ob.ToStation,ec1.TopCODCompanyID,
                               ob.OutBoundTime,ec.CityID 
                               from LMS_RFD.dbo.Waybill w(NoLock)
                               Join LMS_RFD.dbo.OutBound ob(nolock) 
                               on w.WaybillNo = ob.WaybillNo  
                               Join RFD_PMS.dbo.ExpressCompany ec(nolock) on ob.OutBoundStation =ec.ExpressCompanyID
                               Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on w.DeliverStationID =ec1.ExpressCompanyID
                               where w.WaybillNo = @WaybillNo  and ob.OutStationType <> '1'";
            SqlParameter[] parameters = { new SqlParameter("@WaybillNo", SqlDbType.BigInt) };
            parameters[0].Value = waybillNo;
            var dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            if (dt.Rows.Count == 0)
                return null;
            else
            {


                return dt;


            }
        }


        public DataTable GetReturnToSortingCenter(long waybillNo)
        {
            string strSql =
                 @"select w.WaybillNo,w.WaybillType,w.MerchantID,w.DeliverStationID,w.DistributionCode,w.ReturnExpressCompanyId,w.ReturnTime,w.ReturnWarehouse,ec.TopCODCompanyID
                              
                               from LMS_RFD.dbo.Waybill w(NoLock) Join RFD_PMS.dbo.ExpressCompany ec(nolock) on w.DeliverStationID =ec.ExpressCompanyID
                               where w.WaybillNo =@WaybillNo ";

            SqlParameter[] parameters = { new SqlParameter("@WaybillNo", SqlDbType.BigInt) };
            parameters[0].Value = waybillNo;
            var dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt;
            }
        }




        public DataTable GetMerchantToSortingCenter(long waybillNo)
        {
            string strSql =
                 @"select w.WaybillNo,w.WaybillType,w.CreatStation,w.DistributionCode,w.MerchantID,w.DeliverStationID,ec1.TopCodCompanyID,
                               ib1.IntoTime,ec.CityID from LMS_RFD.dbo.Waybill w(NoLock)
                               Join  ( select Top 1 * from  LMS_RFD.dbo.InBound ib(nolock) where ib.WaybillNo = @WaybillNo  )ib1
                               on w.WaybillNo = ib1.WaybillNo  
                               Join RFD_PMS.dbo.ExpressCompany ec(nolock) on w.CreatStation =ec.ExpressCompanyID 
                               Join RFD_PMS.dbo.ExpressCompany ec1(nolock) on w.DeliverStationID =ec1.ExpressCompanyID 
                               where w.WaybillNo = @WaybillNo";

            SqlParameter[] parameters = { new SqlParameter("@WaybillNo", SqlDbType.BigInt) };
            parameters[0].Value = waybillNo;
            var dt = SqlHelper.ExecuteDataset(Connection, CommandType.Text, strSql, parameters).Tables[0];

            if (dt.Rows.Count == 0)
                return null;
            else
            {
                return dt;
            }
        }

        public int DeleteData(string minID ,string maxID)
        {
            string sql =
                "Update  RFD_FMS.dbo.FMS_SortingTransferDetail set IsDeleted =1 where DetailKid >=@minID and DetailKid <=@maxID";
            SqlParameter[] parameters = {
                                           new SqlParameter("@minID", SqlDbType.VarChar),
                                           new SqlParameter("@maxID", SqlDbType.VarChar)
                                       };
            parameters[0].Value = minID;
            parameters[1].Value = maxID;
            int result =SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, sql, parameters);

            return result;
        }

        public DataTable GetWaybillStatus (string waybillNos)
        {
            string sql = string.Format(@"select WaybillNo,Status,SubStatus from LMS_RFD.dbo.LMS_WaybillStatusChangeLog (nolock)
                           where WaybillNo in ({0})",waybillNos);
            var ds =SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql);
            return ds.Tables[0];
        }

        public DataTable GetCategoryInfo(string waybillnos)
        {
            string sql =
                string.Format(
                    @"select waybillno , waybillCategory from lms_rfd.dbo.waybillinfo where waybillno in ({0})",
                    waybillnos);
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql);
            return ds.Tables[0];
        }

        public DataTable MerchantInfo(string waybillnos)
        {
            string sql =
               string.Format(
                   @"select waybillno , merchantID from lms_rfd.dbo.waybill(nolock) where waybillno in ({0})",
                   waybillnos);
            var ds = SqlHelper.ExecuteDataset(ReadOnlyConnection, CommandType.Text, sql);
            return ds.Tables[0];
        }
    }
}
