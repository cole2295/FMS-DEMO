using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using RFD.FMS.AdoNet.UnitOfWork;
using RFD.FMS.Util;
using RFD.FMS.WEBLOGIC;
using System.Text;
using RFD.FMS.Util.ControlHelper;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;


namespace RFD.FMS.WEBLOGIC
{
	[DataObject]
	public class FtpService
	{
        //private static IList<string> fileTypes = new List<string> { "txt","doc","docx","xls","xlsx","bmp","jpeg","gif","pdf" };

        public static string DownLoadTemplateFile(string localPath, string fileName)
        {
            try
            {
                string localFilePath = localPath + Guid.NewGuid().ToString();

                string downFileName = StringUtil.SubString(fileName, '_', false);

                return DownLoad(localPath, localFilePath, downFileName, fileName, "TemplateFile");
            }
            catch (Exception ex)
            {
                LogService.Instance.WriteException(ex);

                return ex.Message;
            }
        }

        public static string UpLoadTemplateFile(HttpPostedFile fu, string fileName)
        {
            try
            {
                //string fileType = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();

                //if (fileTypes.Contains(fileType) == false)
                //{
                //    return "上载模版不支持" + fileType + "类型的文件,支持的类型有：txt,doc,docx,xls,xlsx,bmp,jpeg,gif,pdf";
                //}

                return UpLoad(fu, fileName, "TemplateFile");
            }
            catch (Exception ex)
            {
                LogService.Instance.WriteException(ex);

                return ex.Message;
            }
        }

        public static string DeleteTemplateFile(string fileName)
        {
			return String.Empty;
			//try
			//{
			//    return Delete(fileName, "TemplateFile");
			//}
			//catch (Exception ex)
			//{
			//    LogService.Instance.WriteException(ex);

			//    return ex.Message;
			//}
        }

        public static string DownLoadAttachmentFile(string localPath, string fileName)
        {
            try
            {
                string localFilePath = localPath + Guid.NewGuid().ToString();

                string downFileName = StringUtil.SubString(fileName, '_', false);

                return DownLoad(localPath, localFilePath, downFileName, fileName, "AttachmentFile");
            }
            catch (Exception ex)
            {
                LogService.Instance.WriteException(ex);

                return ex.Message;
            }
        }

        public static string UpLoadAttachmentFile(HttpPostedFile fu, ref string fileName)
        {
            //string fileType = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();

            //if (fileTypes.Contains(fileType) == false)
            //{
            //    return "上载附件不支持" + fileType + "类型的文件,支持的类型有：txt,doc,docx,xls,xlsx,bmp,jpeg,gif,pdf";
            //}

			string monthPath = MonthDirectory();

			string message = UpLoad(fu, fileName, "AttachmentFile" + "/" + monthPath);

			fileName = monthPath + "\\" + fileName;

            return message;
        }

        public static string DeleteAttachmentFile(string fileName)
        {
			return String.Empty;
            //return Delete(fileName, "AttachmentFile");
        }

		#region 公用上传文件操作 上传导入使用

		public static string UpLoadTempFile(FileUpload fu, ref string fileName)
		{
			fileName = Guid.NewGuid().ToString() + "_" + fileName;
			string flag = UpLoad(fu.PostedFile, fileName, "TempFile");
			return flag;
		}

		public static string DownLoadTempFile(string localPath, string fileName)
		{
			try
			{
				return DownLoad(localPath, fileName, "TempFile");
			}
			catch (Exception ex)
			{
				LogService.Instance.WriteException(ex);
				return null;
			}
		}

		public static string DeleteTempFile(string fileName)
		{
			return Delete(fileName, "TempFile");
		}

		/// <summary>
		/// 上传服务器 下载本地
		/// </summary>
		/// <param name="fu"></param>
		/// <param name="localPath"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string UploadFtpDownLocal(FileUpload fu, string localPath, ref string fileName)
		{
			try
			{
				string up = UpLoadTempFile(fu, ref fileName);
				if (!string.IsNullOrEmpty(up) || string.IsNullOrEmpty(fileName))
					return up;

				string down = DownLoadTempFile(localPath, fileName);
				if (!string.IsNullOrEmpty(down))
					return down;

				DeleteTempFile(fileName);
				fileName = localPath + fileName;

				return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		#endregion

        public static string DownLoad(string localPath, string localFilePath, string downToClientFileName, string fileName, string directory)
        {
            string errMessage = DownLoad(localPath, localFilePath, fileName, directory);

            if (errMessage != String.Empty) return errMessage;

            //下载文件
            System.IO.FileInfo file = new System.IO.FileInfo(localFilePath);

            errMessage = DownLoadFile(file, downToClientFileName);

            try
            {
                File.Delete(localFilePath);

                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string DownLoad(string localPath, string fileName, string directory)
        {
			return DownLoad(localPath, localPath + fileName, fileName, directory);
        }

		/// <summary>
		/// 下载文件到本地
		/// </summary>
		/// <param name="localPath">本地路径</param>
		/// <param name="localFilePath">本地文件路径</param>
		/// <param name="fileName">文件名称</param>
		/// <param name="directory">ftp目录</param>
		/// <returns></returns>
        public static string DownLoad(string localPath, string localFilePath, string fileName, string directory)
        {
            FtpFileOperationControl ftpOperator = new FtpFileOperationControl();

			//当文件名中存在部分路径时
			if(fileName.Contains("\\"))
			{
				string filePath = fileName.Substring(0,fileName.LastIndexOf("\\"));

				localPath += filePath;//改变本地路径
			}

            if (System.IO.Directory.Exists(localPath) == false)
            {
                System.IO.Directory.CreateDirectory(localPath);
            }

            ftpOperator.DownloadLocalPath = localFilePath;
            ftpOperator.DownLoadServerFileName = directory + "\\" + fileName;

            if (System.IO.File.Exists(localFilePath) == true)
            {
                System.IO.File.Delete(localFilePath);
            }

            try
            {
                ftpOperator.Do(FtpFileAction.Download);

                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

		public static string DownLoadFile(System.IO.FileInfo file, string downFileName)
		{
			try
			{
				HttpContext.Current.Response.ContentType = "application/ms-download";
				HttpContext.Current.Response.Clear();
				HttpContext.Current.Response.AddHeader("Content-Type", "application/octet-stream");
				HttpContext.Current.Response.Charset = "utf-8";
				HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(downFileName, System.Text.Encoding.UTF8));
				HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
				HttpContext.Current.Response.WriteFile(file.FullName);
				HttpContext.Current.Response.Flush();
				HttpContext.Current.Response.Clear();
                HttpContext.Current.ApplicationInstance.CompleteRequest();

                return String.Empty;
			}
			catch (Exception ex)
			{
                return ex.Message;
			}
		}

        public static string UpLoad(HttpPostedFile fu, string fileName, string directory)
        {
            FtpFileOperationControl ftpOperator = new FtpFileOperationControl();

            ftpOperator.UploadFileName = directory + "\\" + fileName;
            ftpOperator.UploadFileStream = fu.InputStream;
            ftpOperator.MakeDirName = directory;

            try
            {
                ftpOperator.Do(FtpFileAction.Upload);

                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Delete(string fileName, string directory)
        {
            FtpFileOperationControl ftpOperator = new FtpFileOperationControl();

            ftpOperator.DeleteFileName = directory + "\\" + fileName;

            try
            {
                ftpOperator.Do(FtpFileAction.Delete);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return String.Empty;
        }

		private static string MonthDirectory()
		{
			DateTime now = DateTime.Now;
			string pathName = now.Year.ToString() + now.Month.ToString();
			return pathName;
		}
	}
}
