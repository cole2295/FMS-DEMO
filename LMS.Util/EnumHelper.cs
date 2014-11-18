using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RFD.FMS.Util
{
    public static class EnumHelper
    {
        /// <summary>
        /// Retrieve the description on the enum, e.g.
        /// [Description("Bright Pink")]
        /// BrightPink = 2,
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumStr"></param>
        /// <returns></returns>
        public static string GetDescription<T>(string enumStr)
        {
            Type type = typeof (T);
			return GetDescription(type, enumStr);
        }

		public static string GetDescription(Type type, string enumStr)
		{
			MemberInfo[] memInfos = type.GetMembers();

			MemberInfo memInfo = null;

			string temp = string.Empty;

			for (int i = 0; i < memInfos.Length; i++)
			{
				memInfo = memInfos[i];

				if (string.Compare(enumStr, memInfo.Name, true) == 0)
				{
					object[] attrs = memInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

					if (attrs != null && attrs.Length > 0)
					{
						temp = ((DescriptionAttribute)attrs[0]).Description;
					}
					return temp;
				}
			}

			throw new Exception("未知的枚举描述");
		}

        public static T GetValue<T>(string description)
        {
            Type type = typeof (T);

            MemberInfo[] memInfos = type.GetMembers();

            MemberInfo memInfo = null;

            string temp = "";

            for (int i = 0; i < memInfos.Length; i++)
            {
                memInfo = memInfos[i];

                object[] attrs = memInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    temp = ((DescriptionAttribute) attrs[0]).Description;

                    if (temp == description)
                    {
                        return (T) Enum.Parse(typeof (T), memInfo.Name, true);
                    }
                }
            }

            throw new Exception("未知的枚举描述");
        }

        public static int GetEnumValue<T>(string description)
        {
            Dictionary<int, string> dict = GetEnumValueAndDescriptions<T>();
            if (dict != null && dict.Count > 0)
            {
                List<int> keys = dict.Keys.ToList();
                List<string> values = dict.Values.ToList();
                for (int i = 0; i < dict.Count; i++)
                {
                    if (String.Compare(description, values[i], true) == 0)
                    {
                        return keys[i];
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取枚举字典信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumValueAndDescriptions<T>()
        {
            var rValue = new Dictionary<int, string>();
            Type type = typeof (T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            string[] names = Enum.GetNames(typeof (T));
            Array values = Enum.GetValues(typeof (T));
            if (fields.Length == names.Length && fields.Length == values.Length)
            {
                int intCnt = 0;
                foreach (object value in values)
                {
                    object[] attrs = fields[intCnt].GetCustomAttributes(typeof (DescriptionAttribute), false);
                    string description = string.Empty;
                    if (attrs != null && attrs.Length > 0)
                    {
                        description = ((DescriptionAttribute) attrs[0]).Description;
                    }
                    else
                    {
                        description = names[intCnt];
                    }
                    if (!rValue.ContainsKey((int) value))
                    {
                        rValue.Add((int) value, description);
                    }
                    intCnt += 1;
                }
            }
            return rValue;
        }
		/// <summary>  
		/// 得到对枚举的描述文本  
		/// </summary>  
		/// <param name="enumType">枚举类型</param>  
		/// <returns></returns>  
		public static string GetEnumText(Type enumType)
		{
			EnumDescription[] eds = (EnumDescription[])enumType.GetCustomAttributes(typeof(EnumDescription), false);
			if (eds.Length != 1)
				return string.Empty;
			return eds[0].EnumDisplayText;
		}  
    }
}