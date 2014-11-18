using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.BasicSetting;
using Microsoft.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using RFD.FMS.Domain.AudiMgmt;

namespace RFD.FMS.DAL.AudiMgmt
{
	/*
  * (C)Copyright 2011-2012 如风达信息管理系统
  * 
  * 模块名称：银行POS机刷卡核对（数据层）
  * 说明：核对指定条件下的银行POS机刷卡信息
  * 作者：王勇
  * 创建日期：2011/07/22
  * 修改人：
  * 修改时间：
  * 修改记录：
  */
    public class BankPosCheckDao : SqlServerDao, IBankPosCheckDao
	{
        #region SQLSTRING

        private const string strExpressCompanySQl = @"SELECT ExpressCompanyID,ExpressCompanyOldID,ExpressCompanyCode,CompanyName,CompanyAllName,SimpleSpell,ProvinceID
                ,CityID,ParentID,CompanyFlag,MainContacter,LevelCode,MaxOrderLimit,IsReplacement,IsPOS,IsCod,IsPDA,Address,Email
                ,ContacterPhone,Deposit,Sorting,IsDeleted,CreatBy,CreatTime,CreatStationID,UpdateBy,UpdateTime,UpdateStationID
                ,IsOpen,ExpressCompanyVjiaOldID
                FROM RFD_PMS.dbo.ExpressCompany ec(NOLOCK) WHERE ec.ParentID=11 AND ec.CompanyFlag=1";


        private const string strSysDataList =
            @"  SELECT e.POSCode,'' as CardNO,w.WaybillNO,wbs.CreatTime,wbs.FactAmount,'' as Result,ec.CompanyName
                FROM LMS_RFD.dbo.Waybill w(NOLOCK) 
                INNER JOIN LMS_RFD.dbo.WaybillBackStation wbs(NOLOCK) ON w.WaybillNO=wbs.WaybillNO
                INNER JOIN LMS_RFD.dbo.WaybillSignInfo wsi(NOLOCK) ON wbs.WaybillBackStationID=wsi.BackStationInofID
                LEFT JOIN RFD_PMS.dbo.Employee e(NOLOCK) ON wbs.DeliverMan=e.EmployeeID
                LEFT JOIN RFD_PMS.dbo.ExpressCompany ec(NOLOCK) ON wbs.DeliverStation=ec.ExpressCompanyID
                WHERE  wbs.CreatTime BETWEEN @BeginTime AND @EndTime 
                AND wbs.AcceptType=@AcceptType ";
        #endregion

        /// <summary>
        /// 查询出所有的分拣中心
        /// </summary>
        /// <returns></returns>
        public DataTable GetOrderMoneyStoreInfo()
        {
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strExpressCompanySQl).Tables[0];
        }
        /// <summary>
        /// 查询需要核对的系统数据
        /// </summary>
        /// <param name="dtBegDate">开始时间</param>
        /// <param name="dtEndDate">结束时间</param>
        /// <param name="strStatioID">配送站</param>
        /// <returns></returns>
        public DataTable GetCheckData(SearchCondition condition)
        {
            var strSql = strSysDataList;

            if (condition.DeliverStation != -1)
            {
                strSql += " AND ec.ParentID = @ParentID ";
            }
            //参数列表
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@BeginTime", SqlDbType.DateTime) { Value = condition.BeginTime });
            paramList.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = condition.EndTime });
            paramList.Add(new SqlParameter("@AcceptType", SqlDbType.NVarChar, 20) { Value = EnumHelper.GetDescription(PaymentType.POS) });
            paramList.Add(new SqlParameter("@ParentID", SqlDbType.Int) { Value = condition.DeliverStation });
            return SqlHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, paramList.ToArray<SqlParameter>()).Tables[0];
        }
	}
}
