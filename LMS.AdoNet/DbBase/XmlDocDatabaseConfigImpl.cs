using System;
using System.Collections.Generic;
using System.Xml;
using RFD.FMS.Util.Security;

namespace RFD.FMS.AdoNet.DbBase
{
	public class XmlDocDatabaseConfigImpl : DatabaseConfiguration
	{
		public override IList<DatabaseModel> GetDatabases()
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(DbConfigFileName);

			IList<DatabaseModel> result = new List<DatabaseModel>();

			XmlNodeList databaseList = xmlDocument.GetElementsByTagName(DatabaseTagName);
			foreach (XmlNode xmlNode in databaseList)
			{
				if (xmlNode != null)
				{
					var database = new DatabaseModel
					               	{
					               		ConnectionString = DES.Decrypt3DES(xmlNode[ConnectionString].InnerText),
					               		DatabaseName = xmlNode[DatabaseName].InnerText,
					               		DatabaseSource =
					               			(DatabaseSource) Enum.Parse(typeof (DatabaseSource), xmlNode[EnumValue].InnerText),
					               		DatabaseType = (DatabaseType) Enum.Parse(typeof (DatabaseType), xmlNode[DbType].InnerText),
					               		WarehouseId = xmlNode[WarehouseId].InnerText,
					               		WebCookieValue = xmlNode[WebCookieValue].InnerText,
					               		CompanySource =
					               			(CompanySource) Enum.Parse(typeof (CompanySource), xmlNode[CompanySource].InnerText)
					               	};
					result.Add(database);
				}
			}
			return result;
		}
	}
}