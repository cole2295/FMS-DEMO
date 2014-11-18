using System;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using System.Collections;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.HtmlControls;
using RFD.FMS.Util;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.UserControl;
using RFD.SSO.WebClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RFD.FMS.Util.ControlHelper;
using System.Diagnostics;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.Main
{
    public class BasePage : Page
    {
        protected const string UPLOAD_FILE_PATH = "~/file/UpFile/";
        protected const string ERROR_FILE_PATH = "~/file/ErrorFiles/";
        protected const string TEMPLATE_FILE_PATH = "~/UpFile/";

        public LogHelper log = new LogHelper("表现层异常");

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //当前页面请求是否终止
            bool IsCurrRequestStop = false;

            try
            {
                if (!CookieUtil.ExistCookie(LoginUser.CookieUserID))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "__BasePageOnInitError__", "window.open( '../Login.aspx', '_top');", true);

                    return;
                }

                if (LoginType.IsSsoLogin)
                {
                    IsCurrRequestStop = true;
                    SsoCheck();
                }


                //员工ID
                Userid = LoginUser.Userid;
                //所在站点
                ExpressId = LoginUser.ExpressId;
                //所在站点
                ExpressName = LoginUser.ExpressName;
                ExpressCode = LoginUser.ExpressCode;
                //员工名
                UserName = LoginUser.UserName;
                //职工号
                UserCode = LoginUser.UserCode;

                DistributionCode = LoginUser.DistributionCode;
            }
            catch (Exception ex)
            {
                if (IsCurrRequestStop)
                {
                    return;
                }

                log.WriteException(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "__BasePageOnInitError__", "window.open( '../Login.aspx', '_top');", true);
            }

            //检测当前用户是否有页面的访问权限
            if (this.CheckUserPage() == false)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "__BasePageOnInitError__",
                                                   "window.open( '../Login.aspx', '_top');", true);
                return;
            }

           
        }

        private void SsoCheck()
        {
            SsoClient ssoClinet = new SsoClient();
            ssoClinet.ReURL();

            var uid = ssoClinet.QueryUidFromUrl();
            if (!ssoClinet.UserExist(uid))
            {
                ToLoginPage();
            }

            if (!ssoClinet.IsCurrentUser(uid))
            {
                ProcessLogin processLogin = new ProcessLogin();
                if (!ssoClinet.SetCurrentUser(uid, processLogin))
                {
                    ToLoginPage();
                }
                //永远记住，对COOKIE的赋值只有等下一次浏览请求时才能生效
                Response.Redirect(Request.RawUrl);
            }

        }

        private void ToLoginPage()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "__BasePageOnInitError__", "window.open( '../Login.aspx', '_top');", true);
        }

        /// <summary>
        /// 检测当前登录也是否正常
        /// </summary>
        /// <returns></returns>
        private bool CheckUserPage()
        {
            bool returnValue = false;
            string pagename = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            pagename = (pagename.Split('?'))[0].ToUpper().Trim();
            var menu = ServiceLocator.GetService<IRoleservice>();
            DataTable dt = menu.GetAllMenus();

            bool pageExistsInMenu = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string page = dt.Rows[i]["URLName"].ToString().ToUpper().Trim().TrimStart(' ').TrimStart('.');
                page = (page.Split('?'))[0];
                if (page == pagename)
                {
                    pageExistsInMenu = true;
                    DataTable menudt = menu.GetMenuListByUserID(Userid.ToString()).Tables[0];

                    for (int j = 0; j < menudt.Rows.Count; j++)
                    {
                        string userPage = menudt.Rows[j]["URL"].ToString().ToUpper().Trim().TrimStart(' ').TrimStart('.');
                        userPage = (userPage.Split('?'))[0];
                        if (userPage == pagename)
                        {
                            return true;
                        }
                    }
                    break;
                }
            }

            if (pageExistsInMenu == false)
            {
                returnValue = true;
            }
            return returnValue;
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    if (Request.UrlReferrer.ToString() != Request.Url.AbsoluteUri.ToString())
        //    {
        //        PrevPageUrl = Request.UrlReferrer.ToString();
        //    }
        //}


        #region 公用属性

        /// <summary>
        /// 员工ID
        /// </summary>
        public int Userid
        {
            get;
            set;
        }

        /// <summary>
        /// 员工代号
        /// </summary>
        public string UserCode
        {
            get;
            set;
        }

        /// <summary>
        /// 员工名称
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 单位编码
        /// </summary>
        public int ExpressId
        {
            get;
            set;
        }

        /// <summary>
        /// 单位代码
        /// </summary>
        public string ExpressCode
        {
            get;
            set;
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string ExpressName
        {
            get;
            set;
        }

        /// <summary>
        /// 配送商Code
        /// </summary>
        public string DistributionCode
        {
            get;
            set;
        }

        #endregion



        //public string PrevPageUrl
        //{
        //    get
        //    {
        //    if(ViewState["PrevPageUrl"]==null)
        //    {
        //        ViewState["PrevPageUrl"] = "";
        //    }
        //        return ViewState["PrevPageUrl"].ToString();
        //    }
        //    set
        //    {
        //        ViewState["PrevPageUrl"] = value;

        //    }

        //}
        /// <summary>
        /// JS提示框
        /// </summary>
        /// <param name="message"></param>
        public void Alert(string message)
        {
            string js = String.Format(" jAlert(\"{0}\");", message.Trim());

            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), js, true);
        }

        public void RunJS(string jsFunction)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), jsFunction, true);
        }

        public void Alert(UpdatePanel updateUI, string message)
        {
            string js = String.Format(" jAlert(\"{0}\");", message.Trim());

            System.Web.UI.ScriptManager.RegisterStartupScript(updateUI, updateUI.GetType(), Guid.NewGuid().ToString(), js, true);
        }

        public void RunJS(UpdatePanel updateUI, string jsFunction)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(updateUI, updateUI.GetType(), Guid.NewGuid().ToString(), jsFunction, true);
        }

        /// <summary>
        /// 触发JS
        /// </summary>
        /// <param name="js">js代码</param>
        /// <param name="key">关键字</param>
        /// <param name="startup">是否为StartUp时注册</param>
        public void RegisterScript(string js, string key, bool startup)
        {
            if (!ClientScript.IsStartupScriptRegistered(key))
            {
                if (startup)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), key, js, true);
                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), key, js, true);
                }
            }
        }
        /// <summary>
        /// 触发JS
        /// </summary>
        /// <param name="js">js代码</param>
        /// <param name="key">关键字</param>
        public void RegisterScript(string js, string key)
        {
            RegisterScript(js, key, true);
        }

        public void TurnMsg(string msg)
        {
            Response.Redirect(ResolveUrl("~/Msg.aspx?msg=") + msg);
        }

        /// <summary>
        /// 分页功能并实现绑定
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="pager">分页控件</param>
        /// <param name="grid">绑定的数据控件</param>
        protected void BindDataWithBuildPage(DataTable dataSource, Pager pager, GridView grid)
        {
            pager.RecordCount = dataSource.Rows.Count;
            PagedDataSource pds = new PagedDataSource();
            pds.AllowPaging = true;
            pds.PageSize = pager.PageSize;
            pds.CurrentPageIndex = pager.CurrentPageIndex - 1;
            pds.DataSource = dataSource.DefaultView;
            grid.DataSource = pds;
            grid.DataBind();
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columns">导出的列名(若为null,则默认导出dataSource的列)</param>
        /// <param name="fileName">导出的文件名</param>
        protected void ExportExcel(DataTable dataSource, string[] columns, string title)
        {
            ExportExcel(dataSource, columns, null, title);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="columns">导出的列名(若为null,则默认导出dataSource的列)</param>
        /// <param name="ignoreColumns">需要移除的列(若为null,则不移除)</param>
        /// <param name="title">导出的文件名</param>
        protected void ExportExcel(DataTable dataSource, string[] columns, string[] ignoreColumns, string title)
        {
            DateTime now = DateTime.Now;
            string dicName = Server.MapPath(string.Format("~/file/Excel/{0}/{1}/{2}/", now.Year, now.Month, now.Day));
            if (!Directory.Exists(dicName))
            {
                Directory.CreateDirectory(dicName);
            }

            var data = dataSource.Copy();

            data.Format(columns, ignoreColumns);

            CSVExport.DataTable2Excel(data, dicName + title);

            File.Delete(dicName + title);
        }
        /// <summary>
        /// 导出PDF
        /// </summary>
        /// <param name="data">数据源</param>
        /// <param name="columns">导出的列名(若为null,则默认导出dataSource的列)</param>
        /// <param name="title">导出的文件名</param>
        /// <param name="widths">列宽</param>
        protected void ExportPDF(DataTable data, string[] columns, string title, params int[] widths)
        {
            ExportPDF(data, columns, null, title, widths);
        }
        /// <summary>
        /// 导出PDF
        /// </summary>
        /// <param name="data">数据源</param>
        /// <param name="columns">导出的列名(若为null,则默认导出dataSource的列)</param>
        /// <param name="ignoreColumns">需要移除的列(若为null,则不移除)</param>
        /// <param name="title">导出的文件名</param>
        /// <param name="widths">列宽</param>
        protected void ExportPDF(DataTable dataSource, string[] columns, string[] ignoreColumns, string title, params int[] widths)
        {
            try
            {
                Rectangle r = new Rectangle(800, 480);
                Document document = new Document(r);
                DateTime now = DateTime.Now;
                string dicName = Server.MapPath(string.Format("~/file/PDF/{0}/{1}/{2}/", now.Year, now.Month, now.Day));
                if (!Directory.Exists(dicName))
                {
                    Directory.CreateDirectory(dicName);
                }
                string fileName = string.Format(title + "-{0}.pdf", now.ToString("HHmmss") + now.Millisecond.ToString());
                PdfWriter.GetInstance(document, new FileStream(dicName + fileName, FileMode.Create));
                document.Open();

                BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                //字体
                iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 8, Font.NORMAL, new BaseColor(0, 0, 0));
                iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bfChinese, 10, Font.BOLD, new BaseColor(0, 0, 0));
                iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bfChinese, 15, Font.BOLD, new BaseColor(0, 0, 0));
                //标题
                var pdfTitle = new Paragraph(title, fontTitle);
                pdfTitle.Alignment = iTextSharp.text.Rectangle.ALIGN_CENTER;
                document.Add(pdfTitle);

                //移除指定的列
                var data = dataSource.Copy();
                data.Format(columns, ignoreColumns);
                //添加内容
                PdfPTable table = new PdfPTable(data.Columns.Count);
                //添加空行
                table.AddCell(new PdfPCell(new Paragraph("", fontHeader))
                {
                    Border = Rectangle.NO_BORDER,
                    Colspan = data.Columns.Count
                });

                //添加列名
                if (columns == null || columns.Length == 0)
                {
                    //若没有传列名，则以dt的列名做为PDF的列名
                    foreach (DataColumn col in data.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Paragraph(col.ColumnName, fontHeader));
                        cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                        table.AddCell(cell);
                    }
                }
                else
                {
                    foreach (string col in columns)
                    {
                        PdfPCell cell = new PdfPCell(new Paragraph(col, fontHeader));
                        cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                        table.AddCell(cell);
                    }
                }
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        table.AddCell(new Phrase(data.Rows[i][j].ToString().Replace("&nbsp;", " "), fontChinese));
                    }
                }
                //如果未设置列宽，则默认平均分配data的列
                if (widths == null || widths.Length == 0)
                {
                    widths = new int[data.Columns.Count];
                    for (int i = 0; i < widths.Length; i++)
                    {
                        widths[i] = (int)r.Width / widths.Length;
                    }
                }
                table.SetWidths(widths);
                table.WidthPercentage = 100;
                document.Add(table);
                document.Close();
                FileStream fFileStream = new FileStream(dicName + fileName, FileMode.Open);
                long fFileSize = fFileStream.Length;
                Context.Response.ContentType = "application/octet-stream";
                Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(fileName), System.Text.Encoding.UTF8) + "\"");
                Context.Response.AddHeader("Content-Length", fFileSize.ToString());
                byte[] fFileBuffer = new byte[fFileSize];
                fFileStream.Read(fFileBuffer, 0, (int)fFileSize);
                fFileStream.Close();
                Context.Response.BinaryWrite(fFileBuffer);
                Context.Response.End();
                File.Delete(dicName + fileName);
            }

            catch (DocumentException de)
            {
                Alert("导出PDF失败，原因：" + de.Message);
            }
        }


        protected void ExportCSV(DataTable data, string[] columns, string[] ignoreColumns, string title)
        {
            ExportCSV(data, columns, ignoreColumns, title, new string[0] { });
        }

        /// <summary>
        /// 增加关于csv导出前缀丢失问题
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <param name="ignoreColumns"></param>
        /// <param name="title"></param>
        /// <param name="fixColumns">需要处理前缀的列名</param>
        protected void ExportCSV(DataTable data, string[] columns, string[] ignoreColumns, string title, string[] fixColumns)
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
                                long str = 0;
                                string strs = dr[i].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace(",", "");

                                if (long.TryParse(strs, out str) && strs.Length > 16)
                                {
                                    strs=((char)(9)).ToString() + str;
                                }

                                if (fixColumns != null && fixColumns.ToList().Contains(dt.Columns[i].ColumnName))
                                    strValue.Append(string.Format("\"=\"\"{0}\"\"\"", strs));
                                else
                                    strValue.Append(strs);

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
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("导出CSV失败！原因：" + ex.Message);
            }
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="uploadFile">上传文件控件</param>
        protected DataTable ImportExcel(HtmlInputFile uploadFile)
        {
            var fileName = string.Empty;
            try
            {
                DateTime now = DateTime.Now;
                var dicName = Server.MapPath(string.Format("~/file/Upload/{0}/{1}/{2}/", now.Year, now.Month, now.Day));
                Directory.CreateDirectory(dicName);
                fileName = uploadFile.PostedFile.FileName.Substring(uploadFile.PostedFile.FileName.LastIndexOf("\\") + 1);
                fileName = dicName + now.ToString("HHmmss") + now.Millisecond.ToString() + "-" + fileName;
                //开始导入
                uploadFile.PostedFile.SaveAs(fileName);
                //var ds = Excel.ExcelToDataSetFor03And07(fileName);
                var ds = Excel.ExcelToDataSetFor03And07(fileName);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("导入的表格无数据，请重新导入！");
                }
                if (File.Exists(fileName))
                {
                    return ds.Tables[0];
                }
                else
                {
                    throw new Exception("导入失败:请先下载模板到本地，然后填写相关数据再进行导入！");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Exception导入失败:"+ex.Message);
            }
            finally
            {
                //File.Delete(fileName);
            }
        }
        /// <summary>
        /// 绑定商家信息到控件上
        /// </summary>
        /// <param name="bindControl">绑定控件</param>
        public void BindMerchants(ListControl bindControl)
        {
            var service = ServiceLocator.GetService<IMerchantService>();
            var merchants = service.GetMerchants(DistributionCode);
            if (merchants != null)
            {
                bindControl.DataSource = merchants;
                bindControl.DataTextField = "MerchantName";
                bindControl.DataValueField = "ID";
                bindControl.DataBind();
                bindControl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--请选择--", ""));
            }
        }

        #region 获取URL参数

        protected string GetQueryString(string key)
        {
            return Request.QueryString[key] == null ? string.Empty : Request.QueryString[key].ToString();
        }

        #endregion
    }

    public class BaseUserControl : System.Web.UI.UserControl
    {
        public LogHelper log = new LogHelper("表现层异常");
        public BaseUserControl()
        {
            try
            {
                //员工ID
                Userid = Convert.ToInt32(CookieUtil.GetCookie("RFDLMSUserID"));
                //所在站点
                ExpressId = Convert.ToInt32(CookieUtil.GetCookie("RFDLMSExpressID"));
                //所在站点
                ExpressName = CookieUtil.GetCookie("RFDLMSExpressName");
                ExpressCode = CookieUtil.GetCookie("RFDLMSExpressCode");
                //员工名
                UserName = CookieUtil.GetCookie("RFDLMSUserName");
                //职工号
                UserCode = CookieUtil.GetCookie("RFDLMSUserCode");

            }
            catch (Exception ex)
            {
                log.WriteException(ex);

            }
        }

        #region 公用属性

        /// <summary>
        /// 员工ID
        /// </summary>
        public int Userid
        {
            get;
            set;
        }

        /// <summary>
        /// 员工代号
        /// </summary>
        public string UserCode
        {
            get;
            set;
        }

        /// <summary>
        /// 员工名称
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 单位编码
        /// </summary>
        public int ExpressId
        {
            get;
            set;
        }

        /// <summary>
        /// 单位代码
        /// </summary>
        public string ExpressCode
        {
            get;
            set;
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string ExpressName
        {
            get;
            set;
        }



        #endregion

        /// <summary>
        /// JS提示框
        /// </summary>
        /// <param name="message"></param>
        public void Alert(string message)
        {
            string js = "<script language=\"javascript\">\n alert(\"" + message.Trim() + "\");\n</script>";
            Page.ClientScript.RegisterStartupScript(Page.GetType(), Guid.NewGuid().ToString(), js);
        }

        public void Alert(UpdatePanel updateUI, string message)
        {
            string js = String.Format(" alert(\"{0}\");", message.Trim());

            System.Web.UI.ScriptManager.RegisterStartupScript(updateUI, updateUI.GetType(), Guid.NewGuid().ToString(), js, true);
        }

        public void RunJS(string jsFunction)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), jsFunction, true);
        }

        public void RunJS(UpdatePanel updateUI, string jsFunction)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(updateUI, updateUI.GetType(), Guid.NewGuid().ToString(), jsFunction, true);
        }

        public void TurnMsg(string msg)
        {
            Response.Redirect("~/Msg.aspx?msg=" + msg);
        }
    }
}
