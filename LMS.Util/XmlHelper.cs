using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RFD.FMS.Util
{
    public class XmlHelper
    {
        public XmlHelper()
        {

        }

        public static XmlDocument LoadXml(string xmlContent)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent.Trim());
                return xmlDoc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XmlNodeList SearchXmlNodeList(XmlDocument xmlDoc, string searchXmlPath, string condition)
        {
            XmlNodeList xnList = xmlDoc.DocumentElement.SelectNodes(condition);
            return xnList;
        }

        /// <summary>
        /// 根据节点名称返回text
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static string GetXmlNodeTextByNodeName(XmlNode xn,string nodeName)
        {
            try
            {
                return xn.SelectSingleNode(nodeName).InnerText.Trim();
            }
            catch
            {
                return "";
            }
        }
    }
}
