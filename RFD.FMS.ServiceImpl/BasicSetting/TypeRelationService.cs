using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.MODEL.BasicSetting;
using System.Text.RegularExpressions;
using System.Data;
using RFD.FMS.Domain.BasicSetting;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Util;
using RFD.FMS.MODEL;

namespace RFD.FMS.ServiceImpl.BasicSetting
{
    public class TypeRelationService : ITypeRelationService
    {
        private ITypeRelationDao _typeRelationDao;

        public List<TypeRelationModel> SearchRelationList(string typeSource, string distributionCode)
        {
            Match m = Regex.Match(typeSource, @"(Express|Merchant)");
            string typeName = string.Empty;
            if (m.Success)
            {
                typeName = m.Groups[0].Value.ToString();
            }
            else
            {
                throw new Exception("没有找到可查询的分类类型");
            }
            DataTable dt=new DataTable();
            switch (typeName)
            {
                case "Merchant":
                    dt = _typeRelationDao.SearchMerchantRelation(typeSource, distributionCode);
                    break;
                case "Express":
                    dt = _typeRelationDao.SearchExpressRelation(typeSource, distributionCode);
                    break;
                default:
                    break;
            }

            return TransforToTypeRelationModelList(dt);
        }

        private List<TypeRelationModel> TransforToTypeRelationModelList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return null;

            List<TypeRelationModel> list = new List<TypeRelationModel>();
            foreach (DataRow dr in dt.Rows)
            {
                TypeRelationModel model = new TypeRelationModel();
                model.CreateBy = dr["CreateBy"] == DBNull.Value ? 0 : int.Parse(dr["CreateBy"].ToString());
                model.UpdateBy = dr["UpdateBy"] == DBNull.Value ? 0 : int.Parse(dr["UpdateBy"].ToString());
                model.IsDelete = DataConvert.ToBoolean(dr["IsDelete"], false);
                model.RelationName = dr["RelationName"] == DBNull.Value ? "" : dr["RelationName"].ToString();
                model.RelationId = dr["RelationId"] == DBNull.Value ? 0 : int.Parse(dr["RelationId"].ToString());
                model.RelationNameID = dr["RelationNameID"] == DBNull.Value ? 0 : int.Parse(dr["RelationNameID"].ToString());
                model.CodeDesc = dr["CodeDesc"] == DBNull.Value ? "" : dr["CodeDesc"].ToString();
                model.TypeCodeNo = dr["TypeCodeNo"] == DBNull.Value ? "" : dr["TypeCodeNo"].ToString();
                model.DistributionCode = dr["DistributionCode"] == DBNull.Value ? "" : dr["DistributionCode"].ToString();
                model.TypeRelationName = dr["TypeRelationName"] == DBNull.Value ? "" : dr["TypeRelationName"].ToString();
                model.TypeRelationKid = dr["TypeRelationKid"] == DBNull.Value ? "" : dr["TypeRelationKid"].ToString();
                list.Add(model);
            }
            return list;
        }

        public bool AddTypeRelation(TypeRelationModel model)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                if (_typeRelationDao.Exists(model))
                {
                    return false;
                }
                IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                model.TypeRelationKid = iDGenerate.NewId("FMS_TypeRelation", "TypeRelationKid");
                if (!_typeRelationDao.Add(model)) return false;
                work.Complete();
                return true;
            }
        }

        public bool UpdateTypeRelation(TypeRelationModel model)
        {
            return _typeRelationDao.Update(model);
        }

        public bool DelTypeRelation(TypeRelationModel model)
        {
            return _typeRelationDao.Delete(model);
        }

        public bool SetTypeRelation(TypeRelationModel model)
        {
            using (IUnitOfWork work = TranScopeFactory.CreateOracleUnit())
            {
                TypeRelationModel modelExists = _typeRelationDao.GetModel(model);
                if (modelExists!=null && !string.IsNullOrEmpty(modelExists.TypeRelationKid))
                {
                    //更新
                    if(!_typeRelationDao.Update(model)) return false;
                }
                else
                {
                    //生成唯一编号
                    IIDGenerateService iDGenerate = ServiceLocator.GetService<IIDGenerateService>();
                    model.TypeRelationKid = iDGenerate.NewId("FMS_TypeRelation", "TypeRelationKid");
                    //新增
                    if (!_typeRelationDao.Add(model)) return false;
                }

                work.Complete();
                return true;
            }
        }

        public List<TypeRelationModel> SearchPeriodRelationList(string typeSource, string distributionCode,string relationName,ref PageInfo pi)
        {
            Match m = Regex.Match(typeSource, @"(Express|Merchant)");
            string typeName = string.Empty;
            if (m.Success)
            {
                typeName = m.Groups[0].Value.ToString();
            }
            else
            {
                throw new Exception("没有找到可查询的分类类型");
            }
            DataTable dt = new DataTable();
            switch (typeName)
            {
                case "Merchant":
                    dt = _typeRelationDao.SearchPeriodMerchantRelation(typeSource, distributionCode, relationName, ref pi);
                    break;
                case "Express":
                    dt = _typeRelationDao.SearchPeriodExpressRelation(typeSource, distributionCode, relationName, ref pi);
                    break;
                default:
                    break;
            }

            return TransforToTypeRelationModelList(dt);
        }
    }
}
