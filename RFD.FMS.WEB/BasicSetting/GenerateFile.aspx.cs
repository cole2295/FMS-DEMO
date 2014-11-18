using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.WEB.PMSOpenService;

namespace RFD.FMS.WEB.BasicSetting 
{
    public partial class GenerateFile : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sysName = Request.QueryString["sysname"];
            string menuName = Request.QueryString["menuname"];
            string ftpFileName = Request.QueryString["ftpfile"];
            if (!string.IsNullOrEmpty(sysName) && !string.IsNullOrEmpty(menuName))
            {
                if (string.IsNullOrEmpty(ftpFileName))
                {
                    
                    PermissionOpenServiceClient client = new PermissionOpenServiceClient();

                    var opreationGuid = client.GetAllOperationGuide(sysName, menuName);
                    if (opreationGuid != null && opreationGuid.Length > 0)
                    {
                        ftpFileName = opreationGuid[0].FilePath;
                    }
                    else
                    {
                        return;
                    }
                    
                }
                
                var fileName = Path.GetFileName(ftpFileName);

                string ftppath = string.Format(@"{0}/{1}/{2}/", DateTime.Now.Year, DateTime.Now.Month,
                                                        DateTime.Now.Day);

                string dicstr = string.Format("../file/Help/{0}/", ftppath);


                string dicName =
                    HttpContext.Current.Server.MapPath(dicstr);



                if (!Directory.Exists(dicName))
                {
                    Directory.CreateDirectory(dicName);

                }
                if (!File.Exists(dicName + fileName))
                {
                    try
                    {
                        var c = new FtpFileOperationControl();
                        c.DownloadLocalPath = dicName + fileName;
                        c.DownLoadServerFullFileName = ftpFileName;
                        c.Do(FtpFileAction.Download);
                    }
                    catch
                    {
                        return;
                    }
                        
                }




                try
                {
                    if (!File.Exists(dicName + fileName))
                    {
                        HttpContext.Current.Response.End();
                        return;
                    }
                    //保存PDF
                    FileStream fFileStream = new FileStream(dicName + fileName, FileMode.Open);
                    long fFileSize = fFileStream.Length;

                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(fileName), System.Text.Encoding.UTF8) + "\"");
                    HttpContext.Current.Response.AddHeader("Content-Length", fFileSize.ToString());
                    byte[] fFileBuffer = new byte[fFileSize];
                    fFileStream.Read(fFileBuffer, 0, (int)fFileSize);

                    HttpContext.Current.Response.BinaryWrite(fFileBuffer);
                    fFileStream.Flush();
                    fFileStream.Close();
                        
                }
                finally
                {
                    HttpContext.Current.Response.End();
                }
                    
                
                 


            }



            

        }

        [WebMethod]
        public static object GetHelpFile(string type, string sysname, string menuname)
        {
            if (type=="help"&&!string.IsNullOrEmpty(sysname) && !string.IsNullOrEmpty(menuname))
            {
                try
                {
                    
                    PermissionOpenServiceClient client = new PermissionOpenServiceClient();
                    var sysnamedecode = HttpUtility.UrlEncode(sysname);
                    var menunamedecode = HttpUtility.UrlEncode(menuname);
                    var opreationGuid = client.GetAllOperationGuide(sysname, menuname);
                    if (opreationGuid != null && opreationGuid.Length > 0)
                    {
                        //help.NavigateUrl = "../BasicSetting/GenerateFile.aspx?sysname=" + sysnamedecode + "&menuname=" +
                        //                   menunamedecode;
                            return new { done = true,ftpfile=opreationGuid[0].FilePath};
                    }
                    else
                    {
                        return new { done = false};
                          
                    }
                    
                }
                catch 
                {

                    return new { done = false};
                }
                      


            }
             return new { done = false};
             
            
        }
    }
}