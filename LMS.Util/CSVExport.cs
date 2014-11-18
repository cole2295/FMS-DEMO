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
    /// 导出文件的格式
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
        ///  替换特殊字符
        /// </summary>
        /// <param name="input">字符串</param>
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
        /// 根据数据列的列名取数据列的列索引
        /// </summary>
        /// <param name="dcc">数据列集合</param>
        /// <param name="columnName">数据列的列名</param>
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
        /// 编码格式
        /// </summary>
        private const string encodeStr = "GB2312";

        /// <summary>
        /// 导出CSV
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        public static void Export(DataTable dt, string fileName)
        {
            Export(dt, ExportFormat.CSV, HttpUtility.UrlEncode(fileName), Encoding.GetEncoding(encodeStr));
        }

        /// <summary>
        /// 导出CSV
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
        /// 导出SmartGridView的数据源的数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="exportFormat">导出文件的格式</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="encoding">编码</param>
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
        /// 导出SmartGridView的数据源的数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columnIndexList">导出的列索引数组</param>
        /// <param name="exportFormat">导出文件的格式</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="encoding">编码</param>
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
        /// 导出SmartGridView的数据源的数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columnNameList">导出的列的列名数组</param>
        /// <param name="exportFormat">导出文件的格式</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="encoding">编码</param>
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
        /// 导出SmartGridView的数据源的数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columnIndexList">导出的列索引数组</param>
        /// <param name="headers">导出的列标题数组</param>
        /// <param name="exportFormat">导出文件的格式</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="encoding">编码</param>
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
        /// 导出SmartGridView的数据源的数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columnNameList">导出的列的列名数组</param>
        /// <param name="headers">导出的列标题数组</param>
        /// <param name="exportFormat">导出文件的格式</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="encoding">编码</param>
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
        /// 导出SmartGridView的数据源的数据
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="headers">导出的表头数组</param>
        /// <param name="fields">导出的字段数组</param>
        /// <param name="exportFormat">导出文件的格式</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="encoding">编码</param>
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
        /// <param name="isTransforTxt">是否将纯数字导出转文本类型</param>
        public static void ExportFile(DataTable data, string[] columns, string[] ignoreColumns, string title, bool isTransforTxt)
        {
            DateTime now = DateTime.Now;
            string fileName = string.Format(title + "-{0}.csv", now.ToString("HHmmss") + now.Millisecond.ToString());
            //先打印标头
            StringBuilder strColu = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            int i = 0;

            var dt = data.Copy();
            //移除指定的列
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
                        strColu.Remove(strColu.Length - 1, 1);//移出掉最后一个,字符

                        sw.WriteLine(strColu);

                        foreach (DataRow dr in dt.Rows)
                        {
                            strValue.Remove(0, strValue.Length);//移出
                            for (i = 0; i <= dt.Columns.Count - 1; i++)
                            {
                                //((char)(9)).ToString() 转为文本类型
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
                            strValue.Remove(strValue.Length - 1, 1);//移出掉最后一个,字符
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
                //Alert("导出CSV失败！原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <param name="ignoreColumns"></param>
        /// <param name="title"></param>
        /// <param name="isTransforTxt">是否将纯数字导出转文本类型</param>
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
            strColu.Remove(strColu.Length - 1, 1);//移出掉最后一个,字符

            byte[] bytes1 = Encoding.GetEncoding("gb2312").GetBytes(strColu.ToString() + Environment.NewLine);
            curContext.Response.OutputStream.Write(bytes1, 0, bytes1.Length);
            curContext.Response.Flush();
            curContext.Response.Clear();

            foreach (DataRow dr in data.Rows)
            {
                strValue.Remove(0, strValue.Length);//移出
                for (i = 0; i <= data.Columns.Count - 1; i++)
                {
                    //((char)(9)).ToString() 转为文本类型
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
                strValue.Remove(strValue.Length - 1, 1);//移出掉最后一个,字符
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
        /// <param name="isTransforTxt">是否将纯数字导出转文本类型</param>
        public static string WriteFile(DataTable data, string[] columns, string[] ignoreColumns, string title, bool isTransforTxt)
        {

            DateTime now = DateTime.Now;
            string fileName = string.Format(title + "-{0}.csv", now.ToString("HHmmss") + now.Millisecond.ToString());
            //先打印标头
            StringBuilder strColu = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            HttpContext curContext = System.Web.HttpContext.Current;
            int i = 0;
            var webPath = string.Format("~/file/PDF/{0}/{1}/{2}/", now.Year, now.Month, now.Day);
            string dicName = curContext.Server.MapPath(webPath);
            //移除指定的列
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
                    strColu.Remove(strColu.Length - 1, 1); //移出掉最后一个,字符

                    sw.WriteLine(strColu);

                    foreach (DataRow dr in data.Rows)
                    {
                        strValue.Remove(0, strValue.Length); //移出
                        for (i = 0; i <= data.Columns.Count - 1; i++)
                        {
                            //((char)(9)).ToString() 转为文本类型
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
                        strValue.Remove(strValue.Length - 1, 1); //移出掉最后一个,字符
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
        /// <param name="isTransforTxt">是否将纯数字导出转文本类型</param>
        public static string WriteCSVFileByPath(DataTable data, string webPath, string fileName, bool isTransforTxt)
        {
            //先打印标头
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
                    strColu.Remove(strColu.Length - 1, 1); //移出掉最后一个,字符

                    sw.WriteLine(strColu);

                    foreach (DataRow dr in data.Rows)
                    {
                        strValue.Remove(0, strValue.Length); //移出
                        for (i = 0; i <= data.Columns.Count - 1; i++)
                        {
                            //((char)(9)).ToString() 转为文本类型
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
                        strValue.Remove(strValue.Length - 1, 1); //移出掉最后一个,字符
                        sw.WriteLine(strValue);
                    }
                }
            }
            return string.Format("{0}{1}", webPath, fileName);
        }

        /// <summary>
        /// 动态生成XSL，并写入XML流
        /// </summary>
        /// <param name="writer">XML流</param>
        /// <param name="headers">表头数组</param>
        /// <param name="fields">字段数组</param>
        /// <param name="exportFormat">导出文件的格式</param>
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
        /// 采用格式少的方式生成数据，数据量少，网络下载传输时比较方便，
        /// 同样以上面六万条10列的数据，只有6M左右。缺点是，超过65535，在office2003上就打不开了。
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
                            strRows.Append(Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy年MM月dd日") + "\t");
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
        /// 问题：因为这一种方案是以xml格式生成的excel，所以生成的文件特别大，
        /// 六万条10列的数据大约有30M，对网络传输不利，
        /// 在局域网内用还是不错的。
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
            //以下两部分是可选的
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

            //设置每行的标题行
            strTitleRow = "<Row ss:AutoFitHeight='0'>";
            for (int j = 0; j < colCount; j++)
            {
                strTitleRow += "<Cell><Data ss:Type=\"String\">" + dt.Columns[j].ColumnName + "</Data></Cell>";
            }
            strTitleRow += "</Row>";

            StringBuilder strRows = new StringBuilder();

            //在变长的字符操作方面stringbuilder的效率比string高得多
            int page = 1;    //分成的sheet数
            int cnt = 1;        //输入的记录数
            int sheetcolnum = 0;        //每个sheet的行数，其实就等于cnt+1
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
                        strRows.Append("<Cell><Data ss:Type=\"String\">" + dt.Rows[i][j].ToString().Trim().Replace("<", "《").Replace(">", "》") + "</Data></Cell>");
                    }
                }
                strRows.Append("</Row>");
                cnt++;

                //到设定行数时，要输出一页，防止office打不开，同时要注意string和stringbuilder的长度限制
                if (cnt >= pagerecords + 1)
                {
                    sheetcolnum = cnt + 1;
                    result += "<Worksheet ss:Name=\"Sheet" + page.ToString() + "\"><Table ss:ExpandedColumnCount=\"" + colCount.ToString() + "\" ss:ExpandedRowCount=\"" + sheetcolnum.ToString() + "\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultColumnWidth=\"104\" ss:DefaultRowHeight=\"13.5\">" + strTitleRow.ToString() + strRows.ToString() + "</Table></Worksheet>";
                    strRows.Remove(0, strRows.Length);
                    cnt = 1;                     //下一个sheet重新计数
                    page++;

                }
            }
            sheetcolnum = cnt + 1;
            result = result + "<Worksheet ss:Name='Sheet" + page.ToString() + "'><Table ss:ExpandedColumnCount='" + colCount.ToString() + "' ss:ExpandedRowCount='" + sheetcolnum.ToString() + "' x:FullColumns='1' x:FullRows='1' ss:DefaultColumnWidth='104' ss:DefaultRowHeight='13.5'>" + strTitleRow.ToString() + strRows.ToString() + "</Table></Worksheet></Workbook>";
            return result;
        }


        /// <summary>
        /// 导出Excel（目前调用的是带有HTML格式的，导出的文件比较大）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="FileName"></param>
        public static void DataTable2Excel(DataTable dt, string FileName)
        {
            try
            {

                string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + HttpUtility.UrlEncode(FileName) + ".xls";//设置导出文件的名称
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
        /// 导出Excel（目前调用的是带有HTML格式的，导出的文件比较大）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="FileName"></param>
        public static void DataTableToTxt(DataTable dt, string FileName)
        {
            try
            {
                string fileName = HttpUtility.UrlEncode(FileName) + ".txt";//设置导出文件的名称
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
            //批次日期	卡号	姓名	邮编	地址	手机号	邮寄号码	退件流水号	订单号	运单号
            //订单状态	订单追踪信息	签收人	收件人省市	收件人市	收件人区	配送站点	配送员	配送员手机号

            foreach (DataRow dr in dt.Rows)
            {
                string tipsAll = dr["邮寄号码"].ToString();

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
                //批次号:batchtime
                string customerOrderNo = dr["卡号"].ToString();
                string receiveName = dr["姓名"].ToString();
                string receivePost = dr["邮编"].ToString().Replace(";", ",");
                string receiveAddress = dr["地址"].ToString().Replace(";", ",");
                string receiveMobile = dr["手机号"].ToString().Replace(";", ",");
                //邮寄号码:tips
                //退件流水号:sendTimeType
                string customerOrder = dr["订单号"].ToString();
                string waybillno = dr["运单号"].ToString();
                string statusName = dr["订单状态"].ToString();
                string tracking = dr["订单追踪信息"].ToString().Replace(";", ",");
                string receiveBy = dr["签收人"].ToString().Replace(";", ",");

                string receiveProvince = dr["收件人省市"].ToString();
                string receiveCity = dr["收件人市"].ToString();
                string receiveArea = dr["收件人区"].ToString();
                string companyName = dr["配送站点"].ToString().Replace(";", ",");
                string employeeName = dr["配送员"].ToString().Replace(";", ",");
                string cellPhone = dr["配送员手机号"].ToString().Replace(";", ",");


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
