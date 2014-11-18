using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL;

namespace RFD.FMS.Service.BasicSetting
{
    public interface ITypeRelationService
    {
        List<TypeRelationModel> SearchRelationList(string typeSource, string distributionCode);

        bool AddTypeRelation(TypeRelationModel model);

        bool UpdateTypeRelation(TypeRelationModel model);

        bool DelTypeRelation(TypeRelationModel model);

        bool SetTypeRelation(TypeRelationModel model);

        List<TypeRelationModel> SearchPeriodRelationList(string typeSource, string distributionCode,string relationName,ref PageInfo pi);
    }
}
