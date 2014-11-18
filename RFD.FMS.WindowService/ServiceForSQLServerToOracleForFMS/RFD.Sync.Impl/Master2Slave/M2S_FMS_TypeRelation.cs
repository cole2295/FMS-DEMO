using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Quartz;
using RFD.Message;
using RFD.Sync.AdoNet;
using RFD.Sync.Impl.Dao;
using RFD.Sync.Impl.Tool;
using RFD.SyncSQL;

namespace RFD.Sync.Impl.Master2Slave
{
    public class M2S_FMS_TypeRelation : SyncBase
    {
        private SyncSelectParam ssParam = SyncSelectParamByConfig.Instance.GetParam("FMS_TypeRelation");

        protected override DataSet GetSyncData()
        {
            SyncDao _dao = new SyncDao();

            DataSet ds = _dao.GetDataSet(DBStr.DbFromConnString, string.Format(SqlCollectionM2S.sqlTypeRelation, ssParam.Where, ssParam.TopN));

            return ds;
        }

        protected override SyncTable GetSynStructFrom()
        {
            string userSql = @" UPDATE o SET o.ischange = 0
                                from FMS_TypeRelation as o join {0} as f on o.TypeRelationKid = f.TypeRelationKid
                                where o.ischange=1";

            SyncTable t = new SyncTable(userSql);

            t.Cols.Add(new SyncCol("TypeRelationKid", "varchar(20)", true));

            return t;
        }

        protected override SyncTable GetSynStructTo()
        {
            SyncTable t = new SyncTable("FMS_TypeRelation", HowTableUpdate.TableInsertAndUpdate);
            t.Cols.Add(new SyncCol("TypeRelationKid", "varchar(20)", true));
            t.Cols.Add(new SyncCol("TypeRelationName", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("DistributionCode", "nvarchar(50)"));
            t.Cols.Add(new SyncCol("TypeCodeNo", "varchar(20)"));
            t.Cols.Add(new SyncCol("RelationNameID", "int"));
            t.Cols.Add(new SyncCol("IsDelete", "bit"));
            t.Cols.Add(new SyncCol("CreateBy", "int"));
            t.Cols.Add(new SyncCol("CreateTime", "datetime"));
            t.Cols.Add(new SyncCol("UpdateBy", "int"));
            t.Cols.Add(new SyncCol("UpdateTime", "datetime"));

            return t;
        }

        protected override string GetClassName()
        {
            return "M2S_FMS_TypeRelation";
        }
    }
}
