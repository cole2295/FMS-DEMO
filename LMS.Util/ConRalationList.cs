using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace RFD.FMS.Util
{
     
        public class ConRelationList
        {
            private static ConRelationList _instance = null;
            private List<ConRelation> _conRelationList = null;

            private ConRelationList()
            {
                _conRelationList = GetConRelationFromXml();
            }

            public static ConRelationList Instance
            {
                get
                {
                    if (null == _instance)
                    {
                        _instance = new ConRelationList();
                    }
                    return _instance;
                }
            }

            public List<ConRelation> GetConRelationFromXml()
            {
                XmlDocument xDoc = new XmlDocument();

                try
                {
                    var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConRelation.xml");

                    xDoc.Load(xmlPath);
                }
                catch
                {
                    //throw new Exception("获取ConRelation.xml配置文件失败");
                }


                List<ConRelation> siteList = new List<ConRelation>();
                try
                {
                    foreach (XmlNode node in xDoc.SelectNodes("ConRelations/ConRelation"))
                    {
                        ConRelation conRelation = new ConRelation();
                        conRelation.LoginKey = node.Attributes["LoginKey"].Value;
                        conRelation.PageName = node.Attributes["PageName"].Value;
                        conRelation.DbCode = node.Attributes["DbCode"].Value; 

                        siteList.Add(conRelation);
                    }
                }
                catch
                {
                    throw new Exception("Sites.xml节点配置错误");
                }

                return siteList;
            }

            public ConRelation GetConInfo(string loginkey)
            {
                ConRelation conRelation = null;
                if (_conRelationList != null)
                {
                    conRelation = _conRelationList.FirstOrDefault(s => s.LoginKey == loginkey);
                }

                if (conRelation == null)
                {
                    throw new Exception(string.Format("未经验证的登录key{0}", loginkey));
                }

                return conRelation;
            }
        }
    
        
         public class ConRelation
        {
            /// <summary>
            /// 登录Key
            /// </summary>
            public string LoginKey
            {
                get;
                set;
            }

            /// <summary>
            /// 页面
            /// </summary>
            public string PageName
            {
                get;
                set;
            }

            /// <summary>
            /// 数据库
            /// </summary>
            public string DbCode
            {
                get;
                set;
            }

            
        }

    }

   