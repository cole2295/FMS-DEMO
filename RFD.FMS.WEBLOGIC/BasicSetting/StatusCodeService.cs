using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using RFD.FMS.Proxy.LMSRoleProxy;
using RFD.FMS.Util;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Domain.BasicSetting;
using System.Web.UI.WebControls;
using RFD.FMS.AdoNet.UnitOfWork;

namespace RFD.FMS.WEBLOGIC.BasicSetting
{
	public class StatusCodeService : IStatusCodeService
	{
		private IStatusCodeService OracleService;

		private IStatusCodeInfoDao _statusCodeInfoDao;

		public IList<string> GetAllType()
		{
			if (OracleService != null)
			{
				return OracleService.GetAllType();
			}
			return _statusCodeInfoDao.GetAllTypes();
		}

		public DataTable GetCodeItemByCodeType(string codeType, string distributionCode, bool isEnabled)
		{
			if (OracleService != null)
			{
				return OracleService.GetCodeItemByCodeType(codeType, distributionCode, isEnabled);
			}
			return _statusCodeInfoDao.GetStatusInfoByCodeType(codeType, distributionCode, isEnabled);
		}

		public IList<StatusCodeInfo> GetListByType(string codeType, string distributionCode, bool isEnabled)
		{
			if (OracleService != null)
			{
				return OracleService.GetListByType(codeType, distributionCode, isEnabled);
			}
			return _statusCodeInfoDao.GetListByType(codeType, distributionCode, isEnabled);
		}

		public int Exists(string codeType, string codeNo, string distributionCode)
		{
			if (OracleService != null)
			{
				return OracleService.Exists(codeType, codeNo, distributionCode);
			}
			return _statusCodeInfoDao.Exists(codeType, codeNo, distributionCode);
		}

		public bool Add(StatusCodeInfo codeInfo)
		{
			return _statusCodeInfoDao.Add(codeInfo);
		}

		public bool Update(StatusCodeInfo codeInfo)
		{
            if (OracleService != null)
            {
                return OracleService.Update(codeInfo);
            }

			using (IUnitOfWork work = TranScopeFactory.CreateUnit())
			{
				bool flag = _statusCodeInfoDao.Update(codeInfo);
				if (!flag) return false;

				work.Complete();
			}
			//if (OracleService != null)
			//{
			//    OracleService.Update(codeInfo);
			//}
			return true;
		}

		public bool Delete(string codeType, string codeNo, string updateBy)
		{
			return _statusCodeInfoDao.Delete(codeType, codeNo, updateBy);
		}

		public StatusCodeInfo GetModel(string codeType, string codeNo, string distributionCode, bool isEnabled)
		{
			if (OracleService != null)
			{
				return OracleService.GetModel(codeType, codeNo, distributionCode, isEnabled);
			}
			return _statusCodeInfoDao.GetModel(codeType, codeNo, distributionCode, isEnabled);
		}

		public void BindDropDownListByCodeType(DropDownList dropDownList, string text, string value, string codeType, string distributionCode)
		{
			if (OracleService != null)
			{
				OracleService.BindDropDownListByCodeType(dropDownList, text, value, codeType, distributionCode);
				return;
			}

			DataTable dt = _statusCodeInfoDao.GetStatusInfoByCodeType(codeType, distributionCode, true);
			if (dt == null || dt.Rows.Count <= 0)
				return;

			dropDownList.Items.Clear();
			dropDownList.DataSource = dt;
			dropDownList.DataTextField = "CodeDesc";
			dropDownList.DataValueField = "CodeNo";
			dropDownList.DataBind();
			if (!string.IsNullOrEmpty(text))
				dropDownList.Items.Insert(0, new ListItem(text, value));
		}

		public bool AddByRelationType(StatusCodeInfo codeInfo)
		{
            if (OracleService != null)
            {
                return OracleService.AddByRelationType(codeInfo);
            }

			using (IUnitOfWork work = TranScopeFactory.CreateUnit())
			{
				//»ñÈ¡±àºÅ¡¢ÅÅÐò
				string codeNo = string.Empty;
				int orderBy = 0;
				_statusCodeInfoDao.GetMaxNoAndOrderBy(codeInfo.CodeType, out codeNo, out orderBy);

				int codeNoInt = 0;
				if (!int.TryParse(codeNo, out codeNoInt))
					codeNoInt = 1;

				codeInfo.CodeNo = (codeNoInt + 1).ToString();
				codeInfo.OrderBy = orderBy + 1;
				bool flag = _statusCodeInfoDao.Add(codeInfo);
				if (!flag) return false;

				work.Complete();

			}

			//if (OracleService != null)
			//{
			//    OracleService.Add(codeInfo);
			//}

			return true;
		}
	}
}
