using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
    public class PageParameterTransfer
    {
        IDictionary<string, string> parameters = new Dictionary<string, string>();

        public void SetValue(string name, string value)
        {
            if (parameters.ContainsKey(name) == false)
            {
                parameters.Add(name, value);
            }
            else
            {
                parameters[name] = value;
            }
        }

        public string GetValue(string name)
        {
            if (parameters.ContainsKey(name) == true)
            {
                return parameters[name];
            }

            return String.Empty;
        }

        public static PageParameterTransfer GetTransfer(string strParameter)
        {
            PageParameterTransfer transfer = new PageParameterTransfer();

            string[] arrayParameter = strParameter.Split('|');

            string parameter;

            string name;
            string value;

            for (int i = 0; i < arrayParameter.Length; i++)
            {
                parameter = arrayParameter[i];

                name = parameter.Split(':')[0];
                value = parameter.Split(':')[1];

                transfer.SetValue(name,value);
            }

            return transfer;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var item in parameters)
            {
                builder.Append(item.Key);
                builder.Append(":");
                builder.Append(item.Value);
                builder.Append("|");
            }

            string result = builder.Remove(builder.Length - 1,1).ToString();

            return result;
        }
    }
}
