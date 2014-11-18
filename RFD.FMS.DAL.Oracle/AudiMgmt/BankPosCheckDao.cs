using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using RFD.FMS.AdoNet.DbBase;
using RFD.FMS.MODEL.BasicSetting;
using Oracle.ApplicationBlocks.Data;
using RFD.FMS.MODEL.Enumeration;
using RFD.FMS.Util;
using RFD.FMS.AdoNet;
using Oracle.DataAccess.Client;
using RFD.FMS.Domain.AudiMgmt;

namespace RFD.FMS.DAL.Oracle.AudiMgmt
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
    public class BankPosCheckDao : OracleDao, IBankPosCheckDao
	{
        #region SQLSTRING

        private const string strExpressCompanySQl = @"SELECT ExpressCompanyID,ExpressCompanyOldID,ExpressCompanyCode,CompanyName,CompanyAllName,SimpleSpell,ProvinceID
                ,CityID,ParentID,CompanyFlag,MainContacter,LevelCode,MaxOrderLimit,IsReplacement,IsPOS,IsCod,IsPDA,Address,Email
                ,ContacterPhone,Deposit,Sorting,IsDeleted,CreatBy,CreatTime,CreatStationID,UpdateBy,UpdateTime,UpdateStationID
                ,IsOpen,ExpressCompanyVjiaOldID
                FROM ExpressCompany ec WHERE ec.ParentID=11 AND ec.CompanyFlag=1";


        private const string strSysDataList =
            @"  SELECT e.POSCode,'' as CardNO,w.WaybillNO,wbs.CreatTime,wbs.FactAmount,'' as Result,ec.CompanyName
                FROM Waybill w 
                INNER JOIN WaybillBackStation wbs ON w.WaybillNO=wbs.WaybillNO
                INNER JOIN WaybillSignInfo wsi ON wbs.WaybillBackStationID=wsi.BackStationInofID
                LEFT JOIN Employee e ON wbs.DeliverMan=e.EmployeeID
                LEFT JOIN ExpressCompany ec ON wbs.DeliverStation=ec.ExpressCompanyID
                WHERE  wbs.CreatTime BETWEEN :BeginTime AND :EndTime 
                AND wbs.AcceptType=:AcceptType ";
        #endregion

        /// <summary>
        /// 查询出所有的分拣中心
        /// </summary>
        /// <returns></returns>
        public DataTable GetOrderMoneyStoreInfo()
        {
            return OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strExpressCompanySQl).Tables[0];
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
                strSql += " AND ec.ParentID = :ParentID ";
            }

            //参数列表
            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter(":BeginTime", OracleDbType.Date) { Value = condition.BeginTime });
            paramList.Add(new OracleParameter(":EndTime", OracleDbType.Date) { Value = condition.EndTime });
            paramList.Add(new OracleParameter(":AcceptType", OracleDbType.Varchar2, 40) { Value = EnumHelper.GetDescription(PaymentType.POS) });
            paramList.Add(new OracleParameter(":ParentID", OracleDbType.Decimal) { Value = condition.DeliverStation });

            return OracleHelper.ExecuteDataset(LMSReadOnlyConnection, CommandType.Text, strSql, paramList.ToArray<OracleParameter>()).Tables[0];
        }
	}
}
