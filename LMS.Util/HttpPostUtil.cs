using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Web;

namespace RFD.FMS.Util
{
    public class HttpPostUtil
    {
        public static string Post(string content, string pInfo, bool readResponse)
        {
            WebRequest req = WebRequest.Create(pInfo);
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            req.Headers.Add("Accept-Encoding:gzip, deflate");
            req.Headers.Add("Accept-Language:zh-cn,en-us");
            req.Timeout = 300000;
            req.Method = "post";
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            req.ContentLength = bytes.Length;
            Stream os = null;
            try
            {
                os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                string responseData = string.Empty; // will be used to store our uncompressed page content
                if (readResponse)
                {
                    WebResponse response = req.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    Encoding encode = Encoding.GetEncoding("utf-8");
                    var reader = new StreamReader(dataStream, encode);
                    string sResponseHeader = response.Headers["Content-Encoding"]; // get response header
                    if (!string.IsNullOrEmpty(sResponseHeader))
                    {
                        if (sResponseHeader.ToLower().Contains("gzip"))
                        {
                            byte[] b = DecompressGzip(dataStream);
                            responseData = Encoding.UTF8.GetString(b);
                        }
                        else if (sResponseHeader.ToLower().Contains("deflate"))
                        {
                            byte[] b = DecompressDeflate(dataStream);
                            responseData = Encoding.UTF8.GetString(b);
                        }
                    }
                    else
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
                os.Close();
                req.GetResponse().Close();
                return responseData;
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }
        }

        private static byte[] DecompressGzip(Stream streamInput)
        {
            Stream streamOutput = new MemoryStream();
            int iOutputLength = 0;
            try
            {
                var readBuffer = new byte[4096];
                // read from input stream and write to gzip stream
                using (var streamGZip = new GZipStream(streamInput, CompressionMode.Decompress))
                {
                    int i;
                    while ((i = streamGZip.Read(readBuffer, 0, readBuffer.Length)) != 0)
                    {
                        streamOutput.Write(readBuffer, 0, i);
                        iOutputLength = iOutputLength + i;
                    }
                }
            }
            catch (Exception ex)
            {
                // todo: handle exception
                throw ex;
            }
            // read uncompressed data from output stream into a byte array
            var buffer = new byte[iOutputLength];
            streamOutput.Position = 0;
            streamOutput.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        private static byte[] DecompressDeflate(Stream streamInput)
        {
            Stream streamOutput = new MemoryStream();
            int iOutputLength = 0;
            try
            {
                var readBuffer = new byte[4096];
                // read from input stream and write to gzip stream
                using (var deflateStream = new DeflateStream(streamInput, CompressionMode.Decompress))
                {
                    int i;
                    while ((i = deflateStream.Read(readBuffer, 0, readBuffer.Length)) != 0)
                    {
                        streamOutput.Write(readBuffer, 0, i);
                        iOutputLength = iOutputLength + i;
                    }
                }
            }
            catch (Exception ex)
            {
                // todo: handle exception
                throw ex;
            }
            // read uncompressed data from output stream into a byte array
            var buffer = new byte[iOutputLength];
            streamOutput.Position = 0;
            streamOutput.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}