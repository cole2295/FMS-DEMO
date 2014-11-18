using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RFD.FMS.Util
{
	public class ObjectSqlGenerator<T> where T : class
	{
		private T Obj;
		private Type typeInfo;
		private string TableName;
		//列名
		private List<string> ColNames;
		public ObjectSqlGenerator(T obj)
		{
			this.Obj = obj;
			typeInfo = obj.GetType();
			TableName = typeInfo.Name;
			ColNames = new List<string>();
			foreach (MemberInfo member in typeInfo.GetMembers())
			{
				if (member.MemberType == MemberTypes.Property)
				{
					ColNames.Add(member.Name);
				}

			}
			if (!ColNames.Contains("FormCode"))
			{
				throw new Exception("参数formcode缺失");
			}

		}


		public string GetUpdateString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("update {0} set ", TableName);

			foreach (string colName in ColNames)
			{
				if (colName == "FormCode")
				{
					continue;
				}
				PropertyInfo pi = typeInfo.GetProperty(colName);
				object data = pi.GetValue(Obj, null);
				if (data != null)
				{
					if ((pi.PropertyType == typeof(DateTime?)) || (pi.PropertyType == typeof(string)) || (pi.PropertyType == typeof(decimal?)))
					{
						sb.AppendFormat("{0}='{1}',", colName, data.ToString());
					}
					else
					{
						sb.AppendFormat("{0}={1},", colName, data.ToString());
					}
				}

			}
			return sb.ToString().TrimEnd(',') + " where FormCode=" + typeInfo.GetProperty("FormCode").GetValue(Obj, null);
		}
		public string GetInsertString()
		{
			StringBuilder valueStr = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("insert into {0}(", TableName);
			for (int index = 0; index < ColNames.Count; index++)
			{
				string colName = ColNames[index];
				sb.Append(colName);
				if (index != ColNames.Count - 1)
				{ sb.Append(","); }

			}
			sb.Append(") values ( ");

			for (int index = 0; index < ColNames.Count; index++)
			{
				string colName = ColNames[index];
				PropertyInfo pi = typeInfo.GetProperty(colName);
				object data = pi.GetValue(Obj, null);


				if (data == null)
				{
					sb.Append("NULL");

				}
				else
				{
					if ((pi.PropertyType == typeof(DateTime?)) || (pi.PropertyType == typeof(string)) || (pi.PropertyType == typeof(decimal?)))
					{


						sb.AppendFormat("'{0}'", data.ToString());

					}
					else
					{

						sb.Append(data);
					}
				}
				if (index != ColNames.Count - 1)
				{ sb.Append(","); }
			}
			sb.AppendFormat(")");
			return sb.ToString();

		}


	}


}
