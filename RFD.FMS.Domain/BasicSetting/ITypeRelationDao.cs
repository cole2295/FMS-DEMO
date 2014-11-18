using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;

namespace RFD.FMS.Domain.BasicSetting
{
    public interface ITypeRelationDao
    {
        DataTable SearchMerchantRelation(string typeSource, string distributionCode);

        DataTable SearchExpressRelation(string typeSource, string distributionCode);

        bool Exists(TypeRelationModel model);

        TypeRelationModel GetModel(TypeRelationModel model);

        bool Add(TypeRelationModel model);

        bool Update(TypeRelationModel model);

        bool Delete(TypeRelationModel model);

        DataTable SearchPeriodMerchantRelation(string typeSource, string distributionCode,string relationName, ref PageInfo pi);

        DataTable SearchPeriodExpressRelation(string typeSource, string distributionCode, string relationName, ref PageInfo pi);
    }
}
