using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Xml;
using System.Web;
using System.Xml.Xsl;
using System.IO;
using LiquidEngine.Tools;
using RFD.FMS.Util.ControlHelper;
using System.Globalization;

namespace RFD.FMS.Util
{
    /// <summary>
    /// �����ļ��ĸ�ʽ
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        /// CSV
        /// </summary>
        CSV

    }

    public class CSVExport
    {
        /// <summary>
        ///  �滻�����ַ�
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns></returns>
        public static string ReplaceSpecialChars(string input)
        {
            // space 	-> 	_x0020_
            // %		-> 	_x0025_
            // #		->	_x0023_
            // &		->	_x0026_
            // /		->	_x002F_

            input = input.Replace(" ", "_x0020_")
                .Replace("%", "_x0025_")
                .Replace("#", "_x0023_")
                .Replace("&", "_x0026_")
                .Replace("/", "_x002F_");

            return input;
        }


        /// <summary>
        /// ���������е�����ȡ�����е�������
        /// </summary>
        /// <param name="dcc">�����м���</param>
        /// <param name="columnName">�����е�����</param>
        /// <returns></returns>
        public static int GetColumnIndexByColumnName(DataColumnCollection dcc, string columnName)
        {
            int result = -1;

            for (int i = 0; i < dcc.Count; i++)
            {
                if (dcc[i].ColumnName.ToLower() == columnName.ToLower())
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// �����ʽ
        /// </summary>
        private const string encodeStr = "GB2312";

        /// <summary>
        /// ����CSV
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        public static void Export(DataTable dt, string fileName)
        {
            Export(dt, ExportFormat.CSV, HttpUtility.UrlEncode(fileName), Encoding.GetEncoding(encodeStr));
        }

        /// <summary>
        /// ����CSV
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnIndexList"></param>
        /// <param name="headers"></param>
        /// <param name="fileName"></param>
        public static void Export(DataTable dt, int[] columnIndexList, string[] headers, string fileName)
        {
            Export(dt, columnIndexList, headers, ExportFormat.CSV, fileName, Encoding.GetEncoding(encodeStr));
        }

        /// <summary>
        /// ����SmartGridView������Դ������
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="exportFormat">�����ļ��ĸ�ʽ</param>
        /// <param name="fileName">����ļ���</param>
        /// <param name="encoding">����</param>
        public static void Export(DataTable dt, ExportFormat exportFormat, string fileName, Encoding encoding)
        {
            DataSet dsExport = new DataSet("Export");
            DataTable dtExport = dt.Copy();

            dtExport.TableName = "Values";
            dsExport.Tables.Add(dtExport);

            string[] headers = new string[dtExport.Columns.Count];
            string[] fields = new string[dtExport.Columns.Count];

            for (int i = 0; i < dtExport.Columns.Count; i++)
            {
                headers[i] = dtExport.Columns[i].ColumnName;

                fields[i] = ReplaceSpecialChars(dtExport.Columns[i].ColumnName);
            }

            Export(dsExport, headers, fields, exportFormat, fileName, encoding);
        }

        /// <summary>
        /// ����SmartGridView������Դ������
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="columnIndexList">����������������</param>
        /// <param name="exportFormat">�����ļ��ĸ�ʽ</param>
        /// <param name="fileName">����ļ���</param>
        /// <param name="encoding">����</param>
        public static void Export(DataTable dt, int[] columnIndexList, ExportFormat exportFormat, string fileName, Encoding encoding)
        {
            DataSet dsExport = new DataSet("Export");
            DataTable dtExport = dt.Copy();

            dtExport.TableName = "Values";
            dsExport.Tables.Add(dtExport);

            string[] headers = new string[columnIndexList.Length];
            string[] fields = new string[columnIndexList.Length];

            for (int i = 0; i < columnIndexList.Length; i++)
            {
                headers[i] = dtExport.Columns[columnIndexList[i]].ColumnName;
                fields[i] = ReplaceSpecialChars(dtExport.Columns[columnIndexList[i]].ColumnName);
            }

            Export(dsExport, headers, fields, exportFormat, fileName, encoding);
        }

        /// <summary>
        /// ����SmartGridView������Դ������
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="columnNameList">�������е���������</param>
        /// <param name="exportFormat">�����ļ��ĸ�ʽ</param>
        /// <param name="fileName">����ļ���</param>
        /// <param name="encoding">����</param>
        public static void Export(DataTable dt, string[] columnNameList, ExportFormat exportFormat, string fileName, Encoding encoding)
        {
            List<int> columnIndexList = new List<int>();
            DataColumnCollection dcc = dt.Columns;

            foreach (string s in columnNameList)
            {
                columnIndexList.Add(GetColumnIndexByColumnName(dcc, s));
            }

            Export(dt, columnIndexList.ToArray(), exportFormat, fileName, encoding);
        }

        /// <summary>
        /// ����SmartGridView������Դ������
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="columnIndexList">����������������</param>
        /// <param name="headers">�������б�������</param>
        /// <param name="exportFormat">�����ļ��ĸ�ʽ</param>
        /// <param name="fileName">����ļ���</param>
        /// <param name="encoding">����</param>
        public static void Export(DataTable dt, int[] columnIndexList, string[] headers, ExportFormat exportFormat, string fileName, Encoding encoding)
        {
            DataSet dsExport = new DataSet("Export");
            DataTable dtExport = dt.Copy();

            dtExport.TableName = "Values";
            dsExport.Tables.Add(dtExport);

            string[] fields = new string[columnIndexList.Length];

            for (int i = 0; i < columnIndexList.Length; i++)
            {
                fields[i] = ReplaceSpecialChars(dtExport.Columns[columnIndexList[i]].ColumnName);
            }

            Export(dsExport, headers, fields, exportFormat, fileName, encoding);
        }

        /// <summary>
        /// ����SmartGridView������Դ������
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="columnNameList">�������е���������</param>
        /// <param name="headers">�������б�������</param>
        /// <param name="exportFormat">�����ļ��ĸ�ʽ</param>
        /// <param name="fileName">����ļ���</param>
        /// <param name="encoding">����</param>
        public static void Export(DataTable dt, string[] columnNameList, string[] headers, ExportFormat exportFormat, string fileName, Encoding encoding)
        {
            List<int> columnIndexList = new List<int>();
            DataColumnCollection dcc = dt.Columns;

            foreach (string s in columnNameList)
            {
                columnIndexList.Add(GetColumnIndexByColumnName(dcc, s));
            }

            Export(dt, columnIndexList.ToArray(), headers, exportFormat, fileName, encoding);
        }

        /// <summary>
        /// ����SmartGridView������Դ������
        /// </summary>
        /// <param name="ds">����Դ</param>
        /// <param name="headers">�����ı�ͷ����</param>
        /// <param name="fields">�������ֶ�����</param>
        /// <param name="exportFormat">�����ļ��ĸ�ʽ</param>
        /// <param name="fileName">����ļ���</param>
        /// <param name="encoding">����</param>
        private static void Export(DataSet ds, string[] headers, string[] fields, ExportFormat exportFormat, string fileName, Encoding encoding)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ContentType = String.Format("text/{0}", exportFormat.ToString().ToLower());
            HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}.{1}", fileName, exportFormat.ToString().ToLower()));
            HttpContext.Current.Response.ContentEncoding = encoding;

            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, encoding);

            CreateStylesheet(writer, headers, fields, exportFormat);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            XmlDataDocument xmlDoc = new XmlDataDocument(ds);
            XslCompiledTransform xslTran = new XslCompiledTransform();
            xslTran.Load(new XmlTextReader(stream));

            System.IO.StringWriter sw = new System.IO.StringWriter();
            xslTran.Transform(xmlDoc, null, sw);

            HttpContext.Current.Response.Write(sw.ToString());
            sw.Close();
            writer.Close();
            stream.Close();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <param name="ignoreColumns"></param>
        /// <param name="title"></param>
        /// <param name="isTransforTxt">�Ƿ񽫴����ֵ���ת�ı�����</param>
        public static void ExportFile(DataTable data, string[] columns, string[] ignoreColumns, string title, bool isTransforTxt)
        {
            DateTime now = DateTime.Now;
            string fileName = string.Format(title + "-{0}.csv", now.ToString("HHmmss") + now.Millisecond.ToString());
            //�ȴ�ӡ��ͷ
            StringBuilder strColu = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            int i = 0;

            var dt = data.Copy();
            //�Ƴ�ָ������
            dt.Format(columns, ignoreColumns);

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms, Encoding.GetEncoding("GB2312")))
                    {
                        sw.AutoFlush = true;
                        for (i = 0; i <= dt.Columns.Count - 1; i++)
                        {
                            strColu.Append(dt.Columns[i].ColumnName);
                            strColu.Append(",");
                        }
                        strColu.Remove(strColu.Length - 1, 1);//�Ƴ������һ��,�ַ�

                        sw.WriteLine(strColu);

                        foreach (DataRow dr in dt.Rows)
                        {
                            strValue.Remove(0, strValue.Length);//�Ƴ�
                            for (i = 0; i <= dt.Columns.Count - 1; i++)
                            {
                                //((char)(9)).ToString() תΪ�ı�����
                                long str = 0;
                                string strs = dr[i].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace(",", "");
                                if (long.TryParse(strs, out str) && strs.Length > 16 && isTransforTxt)
                                {
                                    strValue.Append(((char)(9)).ToString() + str);
                                    //strValue.Append("'" + str);
                                }
                                else
                                {
                                    strValue.Append(strs);
                                }

                                strValue.Append(",");
                            }
                            strValue.Remove(strValue.Length - 1, 1);//�Ƴ������һ��,�ַ�
                            sw.WriteLine(strValue);
                        }
                        HttpContext curContext = System.Web.HttpContext.Current;
                        curContext.Response.ContentType = "application/vnd.ms-excel";
                        curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                        curContext.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(fileName), System.Text.Encoding.UTF8) + "\"");
                        curContext.Response.Charset = "GB2312";
                        curContext.Response.OutputStream.Write(ms.GetBuffer(), 0, (int)ms.Position);
                        curContext.Response.Flush();
                        curContext.Response.End();
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //Alert("����CSVʧ�ܣ�ԭ��" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <param name="ignoreColumns"></param>
        /// <param name="title"></param>
        /// <param name="isTransforTxt">�Ƿ񽫴����ֵ���ת�ı�����</param>
        public static void ExportFileToClient(DataTable data, string title, bool isTransforTxt,string guName,string guid)
        {
            DateTime now = DateTime.Now;
            string fileName = string.Format(title + "-{0}.csv", now.ToString("HHmmss") + now.Millisecond.ToString());
            StringBuilder strColu = new StringBuilder();
            StringBuilder strValue = new StringBuilder();

            HttpContext curContext = System.Web.HttpContext.Current;
            curContext.Response.ContentType = "application/vnd.ms-excel";
            //CookieUtil.AddCookie(guName, guid,false);
            curContext.Response.AppendCookie(new HttpCookie(guName, guid));//"fileDownloadToken"
            curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            curContext.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(fileName), System.Text.Encoding.UTF8) + "\"");
            curContext.Response.Charset = "GB2312";

            int i = 0;
            for (i = 0; i <= data.Columns.Count - 1; i++)
            {
                strColu.Append(data.Columns[i].ColumnName);
                strColu.Append(",");
            }
            strColu.Remove(strColu.Length - 1, 1);//�Ƴ������һ��,�ַ�

            byte[] bytes1 = Encoding.GetEncoding("gb2312").GetBytes(strColu.ToString() + Environment.NewLine);
            curContext.Response.OutputStream.Write(bytes1, 0, bytes1.Length);
            curContext.Response.Flush();
            curContext.Response.Clear();

            foreach (DataRow dr in data.Rows)
            {
                strValue.Remove(0, strValue.Length);//�Ƴ�
                for (i = 0; i <= data.Columns.Count - 1; i++)
                {
                    //((char)(9)).ToString() תΪ�ı�����
                    long str = 0;
                    string strs = dr[i].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace(",", "").Replace("\"","");
                    if (long.TryParse(strs, out str) && strs.Length > 16 && isTransforTxt)
                    {
                        strValue.Append(((char)(9)).ToString() + str);
                    }
                    else
                    {
                        strValue.Append(strs);
                    }
                    strValue.Append(",");
                }
                strValue.Remove(strValue.Length - 1, 1);//�Ƴ������һ��,�ַ�
                byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(strValue.ToString() + Environment.NewLine);

                curContext.Response.OutputStream.Write(bytes, 0, bytes.Length);
                curContext.Response.Flush();
                curContext.Response.Clear();
            }
            curContext.Response.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <param name="ignoreColumns"></param>
        /// <param name="title"></param>
        /// <param name="isTransforTxt">�Ƿ񽫴����ֵ���ת�ı�����</param>
        public static string WriteFile(DataTable data, string[] columns, string[] ignoreColumns, string title, bool isTransforTxt)
        {

            DateTime now = DateTime.Now;
            string fileName = string.Format(title + "-{0}.csv", now.ToString("HHmmss") + now.Millisecond.ToString());
            //�ȴ�ӡ��ͷ
            StringBuilder strColu = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            HttpContext curContext = System.Web.HttpContext.Current;
            int i = 0;
            var webPath = string.Format("~/file/PDF/{0}/{1}/{2}/", now.Year, now.Month, now.Day);
            string dicName = curContext.Server.MapPath(webPath);
            //�Ƴ�ָ������
            if (!Directory.Exists(dicName))
            {
                Directory.CreateDirectory(dicName);
            }
            data.Format(columns, ignoreColumns);
            using (FileStream fFileStream = new FileStream(dicName + fileName, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fFileStream, Encoding.GetEncoding("GB2312")))
                {
                    sw.AutoFlush = true;
                    for (i = 0; i <= data.Columns.Count - 1; i++)
                    {
                        strColu.Append(data.Columns[i].ColumnName);
                        strColu.Append(",");
                    }
                    strColu.Remove(strColu.Length - 1, 1); //�Ƴ������һ��,�ַ�

                    sw.WriteLine(strColu);

                    foreach (DataRow dr in data.Rows)
                    {
                        strValue.Remove(0, strValue.Length); //�Ƴ�
                        for (i = 0; i <= data.Columns.Count - 1; i++)
                        {
                            //((char)(9)).ToString() תΪ�ı�����
                            long str = 0;
                            string strs =
                                dr[i].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace
                                    (",", "");
                            if (long.TryParse(strs, out str) && strs.Length > 16 && isTransforTxt)
                            {
                                strValue.Append(((char)(9)).ToString() + str);
                                //strValue.Append("'" + str);
                            }
                            else
                            {
                                strValue.Append(strs);
                            }

                            strValue.Append(",");
                        }
                        strValue.Remove(strValue.Length - 1, 1); //�Ƴ������һ��,�ַ�
                        sw.WriteLine(strValue);
                    }
                }
            }
            return string.Format("{0}{1}", webPath, fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <param name="ignoreColumns"></param>
        /// <param name="title"></param>
        /// <param name="isTransforTxt">�Ƿ񽫴����ֵ���ת�ı�����</param>
        public static string WriteCSVFileByPath(DataTable data, string webPath, string fileName, bool isTransforTxt)
        {
            //�ȴ�ӡ��ͷ
            StringBuilder strColu = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            int i = 0;
            using (FileStream fFileStream = new FileStream(webPath +"\\"+ fileName+".csv", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fFileStream, Encoding.GetEncoding("GB2312")))
                {
                    sw.AutoFlush = true;
                    for (i = 0; i <= data.Columns.Count - 1; i++)
                    {
                        strColu.Append(data.Columns[i].ColumnName);
                        strColu.Append(",");
                    }
                    strColu.Remove(strColu.Length - 1, 1); //�Ƴ������һ��,�ַ�

                    sw.WriteLine(strColu);

                    foreach (DataRow dr in data.Rows)
                    {
                        strValue.Remove(0, strValue.Length); //�Ƴ�
                        for (i = 0; i <= data.Columns.Count - 1; i++)
                        {
                            //((char)(9)).ToString() תΪ�ı�����
                            long str = 0;
                            string strs =
                                dr[i].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace
                                    (",", "").Replace("\"", "");
                            if (long.TryParse(strs, out str) && strs.Length > 16 && isTransforTxt)
                            {
                                strValue.Append(((char)(9)).ToString() + str);
                                //strValue.Append("'" + str);
                            }
                            else
                            {
                                strValue.Append(strs);
                            }

                            strValue.Append(",");
                        }
                        strValue.Remove(strValue.Length - 1, 1); //�Ƴ������һ��,�ַ�
                        sw.WriteLine(strValue);
                    }
                }
            }
            return string.Format("{0}{1}", webPath, fileName);
        }

        /// <summary>
        /// ��̬����XSL����д��XML��
        /// </summary>
        /// <param name="writer">XML��</param>
        /// <param name="headers">��ͷ����</param>
        /// <param name="fields">�ֶ�����</param>
        /// <param name="exportFormat">�����ļ��ĸ�ʽ</param>
        private static void CreateStylesheet(XmlTextWriter writer, string[] headers, string[] fields, ExportFormat exportFormat)
        {
            const string ns = "http://www.w3.org/1999/XSL/Transform";
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("xsl", "stylesheet", ns);
            writer.WriteAttributeString("version", "1.0");
            writer.WriteStartElement("xsl:output");
            writer.WriteAttributeString("method", "text");
            writer.WriteAttributeString("version", "4.0");
            writer.WriteEndElement();

            // xsl-template
            writer.WriteStartElement("xsl:template");
            writer.WriteAttributeString("match", "/");

            // xsl:value-of for headers
            for (int i = 0; i < headers.Length; i++)
            {
                writer.WriteString("\"");
                writer.WriteStartElement("xsl:value-of");
                writer.WriteAttributeString("select", "'" + headers[i] + "'");
                writer.WriteEndElement(); // xsl:value-of
                writer.WriteString("\"");
                if (i != fields.Length - 1) writer.WriteString((exportFormat == ExportFormat.CSV) ? "," : "	");
            }

            // xsl:for-each
            writer.WriteStartElement("xsl:for-each");
            writer.WriteAttributeString("select", "Export/Values");
            writer.WriteString("\r\n");

            // xsl:value-of for data fields
            for (int i = 0; i < fields.Length; i++)
            {
                writer.WriteString("\"");
                writer.WriteStartElement("xsl:value-of");
                writer.WriteAttributeString("select", fields[i]);
                writer.WriteEndElement(); // xsl:value-of
                writer.WriteString("\"");
                if (i != fields.Length - 1) writer.WriteString((exportFormat == ExportFormat.CSV) ? "," : "	");
            }

            writer.WriteEndElement(); // xsl:for-each
            writer.WriteEndElement(); // xsl-template
            writer.WriteEndElement(); // xsl:stylesheet
        }

        /// <summary>
        /// ���ø�ʽ�ٵķ�ʽ�������ݣ��������٣��������ش���ʱ�ȽϷ��㣬
        /// ͬ��������������10�е����ݣ�ֻ��6M���ҡ�ȱ���ǣ�����65535����office2003�Ͼʹ򲻿��ˡ�
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string BuildExportHTML(DataTable dt)
        {
            string result = string.Empty;
            int readCnt = dt.Rows.Count;
            int colCount = dt.Columns.Count;

            const int pagerecords = 5000;
            string strTitleRow = "";
            for (int j = 0; j < colCount; j++)
            {
                strTitleRow += dt.Columns[j].ColumnName + "\t";
            }
            strTitleRow += "\r\n";

            StringBuilder strRows = new StringBuilder();
            int cnt = 1;
            for (int i = 0; i < readCnt; i++)
            {
                //strRows.Append("");
                for (int j = 0; j < colCount; j++)
                {
                    if (dt.Columns[j].DataType.Name == "DateTime" || dt.Columns[j].DataType.Name == "SmallDateTime")
                    {
                        if (dt.Rows[i][j].ToString() != string.Empty)
                        {
                            strRows.Append(Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy��MM��dd��") + "\t");
                        }
                        else
                            strRows.Append("\t");
                    }
                    else
                    {
                        strRows.Append(dt.Rows[i][j].ToString().Trim() + "\t");
                    }
                }
                strRows.Append("\r\n");
                cnt++;
                if (cnt >= pagerecords)
                {
                    result += strRows.ToString();
                    strRows.Remove(0, strRows.Length);
                    cnt = 1;
                }
            }
            result = strTitleRow + result + strRows;
            return result;
        }

        /// <summary>
        /// ���⣺��Ϊ��һ�ַ�������xml��ʽ���ɵ�excel���������ɵ��ļ��ر��
        /// ������10�е����ݴ�Լ��30M�������紫�䲻����
        /// �ھ��������û��ǲ���ġ�
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string BuildExportHTMLMoreSheet(DataTable dt)
        {
            string result = string.Empty;
            int readCnt = dt.Rows.Count;
            int colCount = dt.Columns.Count;

            const int pagerecords = 50000;
            result = "<?xml version=\"1.0\" encoding=\"gb2312\"?>";
            result += "<?mso-application progid=\"Excel.Sheet\"?>";
            result += "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" ";
            result += "xmlns:o=\"urn:schemas-microsoft-com:office:office\" ";
            result += "xmlns:x=\"urn:schemas-microsoft-com:office:excel\" ";
            result += "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" ";
            result += "xmlns:html=\"http://www.w3.org/TR/REC-html40\"> ";
            //�����������ǿ�ѡ��
            //result += "<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">";
            //result += "<Author>User</Author>";
            //result += "<LastAuthor>User</LastAuthor>";
            //result += "<Created>2009-03-20T02:15:12Z</Created>";
            //result += "<Company>Microsoft</Company>";
            //result += "<Version>12.00</Version>";
            //result += "</DocumentProperties>";

            //result += "<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            //result += "<WindowHeight>7815</WindowHeight>";
            //result += "<WindowWidth>14880</WindowWidth>";
            //result += "<WindowTopX>240</WindowTopX>";
            //result += "<WindowTopY>75</WindowTopY>";
            //result += "<ProtectStructure>False</ProtectStructure>";
            //result += "<ProtectWindows>False</ProtectWindows>";
            //result += "</ExcelWorkbook>";
            string strTitleRow = "";

            //����ÿ�еı�����
            strTitleRow = "<Row ss:AutoFitHeight='0'>";
            for (int j = 0; j < colCount; j++)
            {
                strTitleRow += "<Cell><Data ss:Type=\"String\">" + dt.Columns[j].ColumnName + "</Data></Cell>";
            }
            strTitleRow += "</Row>";

            StringBuilder strRows = new StringBuilder();

            //�ڱ䳤���ַ���������stringbuilder��Ч�ʱ�string�ߵö�
            int page = 1;    //�ֳɵ�sheet��
            int cnt = 1;        //����ļ�¼��
            int sheetcolnum = 0;        //ÿ��sheet����������ʵ�͵���cnt+1
            for (int i = 0; i < readCnt; i++)
            {
                strRows.Append("<Row ss:AutoFitHeight=\"0\">");
                for (int j = 0; j < colCount; j++)
                {

                    if (dt.Columns[j].DataType.Name == "DateTime" || dt.Columns[j].DataType.Name == "SmallDateTime")
                    {
                        if (dt.Rows[i][j].ToString() != string.Empty)
                        {
                            strRows.Append("<Cell><Data ss:Type=\"String\">" + Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "</Data></Cell>");
                        }
                        else
                            strRows.Append("<Cell><Data ss:Type=\"String\"></Data></Cell>");
                    }
                    else
                    {
                        strRows.Append("<Cell><Data ss:Type=\"String\">" + dt.Rows[i][j].ToString().Trim().Replace("<", "��").Replace(">", "��") + "</Data></Cell>");
                    }
                }
                strRows.Append("</Row>");
                cnt++;

                //���趨����ʱ��Ҫ���һҳ����ֹoffice�򲻿���ͬʱҪע��string��stringbuilder�ĳ�������
                if (cnt >= pagerecords + 1)
                {
                    sheetcolnum = cnt + 1;
                    result += "<Worksheet ss:Name=\"Sheet" + page.ToString() + "\"><Table ss:ExpandedColumnCount=\"" + colCount.ToString() + "\" ss:ExpandedRowCount=\"" + sheetcolnum.ToString() + "\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultColumnWidth=\"104\" ss:DefaultRowHeight=\"13.5\">" + strTitleRow.ToString() + strRows.ToString() + "</Table></Worksheet>";
                    strRows.Remove(0, strRows.Length);
                    cnt = 1;                     //��һ��sheet���¼���
                    page++;

                }
            }
            sheetcolnum = cnt + 1;
            result = result + "<Worksheet ss:Name='Sheet" + page.ToString() + "'><Table ss:ExpandedColumnCount='" + colCount.ToString() + "' ss:ExpandedRowCount='" + sheetcolnum.ToString() + "' x:FullColumns='1' x:FullRows='1' ss:DefaultColumnWidth='104' ss:DefaultRowHeight='13.5'>" + strTitleRow.ToString() + strRows.ToString() + "</Table></Worksheet></Workbook>";
            return result;
        }


        /// <summary>
        /// ����Excel��Ŀǰ���õ��Ǵ���HTML��ʽ�ģ��������ļ��Ƚϴ�
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="FileName"></param>
        public static void DataTable2Excel(DataTable dt, string FileName)
        {
            try
            {

                string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + HttpUtility.UrlEncode(FileName) + ".xls";//���õ����ļ�������
                HttpContext curContext = System.Web.HttpContext.Current;
                curContext.Response.ContentType = "application/vnd.ms-excel";
                curContext.Response.ContentEncoding = System.Text.Encoding.Default;
                curContext.Response.AppendHeader("Content-Disposition", ("attachment;filename=" + fileName));
                curContext.Response.Charset = "";
                curContext.Response.Write(BuildExportHTMLMoreSheet(dt));
                curContext.Response.Flush();
                curContext.Response.Close();
            }
            catch (Exception ex)
            {
                log4net.ILog logger = log4net.LogManager.GetLogger(@"DataTable2Excel");
                logger.Error(String.Format("DataTable2ExcelMessage:{0}\r\nStackTrace:{1}\r\nSource:{2}", ex.Message, ex.StackTrace, ex.Source));
            }

        }
        /// <summary>
        /// ����Excel��Ŀǰ���õ��Ǵ���HTML��ʽ�ģ��������ļ��Ƚϴ�
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="FileName"></param>
        public static void DataTableToTxt(DataTable dt, string FileName)
        {
            try
            {
                string fileName = HttpUtility.UrlEncode(FileName) + ".txt";//���õ����ļ�������
                HttpContext curContext = System.Web.HttpContext.Current;
                curContext.Response.ContentType = "text/plain";
                curContext.Response.ContentEncoding = System.Text.Encoding.Default;
                curContext.Response.AppendHeader("Content-Disposition", ("attachment;filename=" + fileName));
                curContext.Response.Charset = "";
                curContext.Response.Write(BuildExportTxt(dt));
                curContext.Response.Flush();
                //curContext.Response.End();
                curContext.Response.Close();
            }
            catch (Exception ex)
            {
                log4net.ILog logger = log4net.LogManager.GetLogger(@"DataTable2Excel");
                logger.Error(String.Format("DataTable2ExcelMessage:{0}\r\nStackTrace:{1}\r\nSource:{2}", ex.Message, ex.StackTrace, ex.Source));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static StringBuilder BuildExportTxt(DataTable dt)
        {
            StringBuilder result = new StringBuilder();
            //��������	����	����	�ʱ�	��ַ	�ֻ���	�ʼĺ���	�˼���ˮ��	������	�˵���
            //����״̬	����׷����Ϣ	ǩ����	�ռ���ʡ��	�ռ�����	�ռ�����	����վ��	����Ա	����Ա�ֻ���

            foreach (DataRow dr in dt.Rows)
            {
                string tipsAll = dr["�ʼĺ���"].ToString();

                string[] tipssplit = tipsAll.Split(';');

                string batchtime = "";
                string tips = "";
                string sendTimeType = "";
                if (tipssplit.Length == 3)
                {
                    batchtime = tipssplit[0];
                    tips = tipssplit[1];
                    sendTimeType = tipssplit[2];
                }
                //���κ�:batchtime
                string customerOrderNo = dr["����"].ToString();
                string receiveName = dr["����"].ToString();
                string receivePost = dr["�ʱ�"].ToString().Replace(";", ",");
                string receiveAddress = dr["��ַ"].ToString().Replace(";", ",");
                string receiveMobile = dr["�ֻ���"].ToString().Replace(";", ",");
                //�ʼĺ���:tips
                //�˼���ˮ��:sendTimeType
                string customerOrder = dr["������"].ToString();
                string waybillno = dr["�˵���"].ToString();
                string statusName = dr["����״̬"].ToString();
                string tracking = dr["����׷����Ϣ"].ToString().Replace(";", ",");
                string receiveBy = dr["ǩ����"].ToString().Replace(";", ",");

                string receiveProvince = dr["�ռ���ʡ��"].ToString();
                string receiveCity = dr["�ռ�����"].ToString();
                string receiveArea = dr["�ռ�����"].ToString();
                string companyName = dr["����վ��"].ToString().Replace(";", ",");
                string employeeName = dr["����Ա"].ToString().Replace(";", ",");
                string cellPhone = dr["����Ա�ֻ���"].ToString().Replace(";", ",");


                result.Append(batchtime + ";");
                result.Append(customerOrderNo + ";");
                result.Append(receiveName + ";");
                result.Append(receivePost + ";");
                result.Append(receiveAddress + ";");
                result.Append(receiveMobile + ";");


                result.Append(tips + ";");
                result.Append(sendTimeType + ";");
                result.Append(customerOrder + ";");
                result.Append(waybillno + ";");
                result.Append(statusName + ";");
                result.Append(tracking + ";");
                result.Append(receiveBy + ";");


                result.Append(receiveProvince + ";");
                result.Append(receiveCity + ";");
                result.Append(receiveArea + ";");
                result.Append(companyName + ";");
                result.Append(employeeName + ";");

                result.Append(cellPhone);
                result.Append("\r\n");


            }


            return result;
        }

    }
}
