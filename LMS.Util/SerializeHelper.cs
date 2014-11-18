using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

namespace MOS.Common
{
    /// <summary>
    /// SerializeHelper ��ժҪ˵����
    /// </summary>
    public class SerializeHelper
    {
        public SerializeHelper()
        {
        }

        #region Soap ���л�����
        /// <summary>
        /// ���л��������л���������
        /// </summary>
        public static string SoapSerialize(object obj)
        {
            string result = "";
            if (obj != null)
            {
                try
                {
                    SoapFormatter formatter = new SoapFormatter();
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, obj);
                    result = Encoding.Unicode.GetString(stream.ToArray());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// Soap �����л�
        /// </summary>
        /// <returns></returns>
        public static object SoapDeserialize(string str)
        {
            object result = null;
            if (str != "")
            {
                try
                {
                    SoapFormatter formatter = new SoapFormatter();
                    MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(str));
                    result = formatter.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
        #endregion


        /// <summary>
        /// ��һ�� DataSet ���л��������ַ���
        /// </summary>
        public static string SerializeData(object obj)
        {
            StreamWriter sw = null;
            string serializeString = null;

            try
            {
                // ���´�����ܵ���ͷ�Σ���ѡ��ۿ�
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                MemoryStream memStream = new MemoryStream();
                sw = new StreamWriter(memStream);
                xmlSerializer.Serialize(sw, obj);
                memStream.Position = 0;
                serializeString = Encoding.UTF8.GetString(memStream.GetBuffer());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }

            return serializeString;
        }

        /// <summary>
        /// �������������ڻָ� DataSet
        /// </summary>
        /// <param name="serializedString">ͨ�� SerializeDataSet ���ɵ� string</param>
        /// <returns></returns>
        public static object DeserializeData(Type type, string serializedString)
        {
            //���������ǿմ���ֱ�ӷ���
            if (serializedString.Trim().Equals(string.Empty))
            {
                throw new Exception("��������л��ַ���Ϊ��");
            }

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                StringReader stringReader = new StringReader(serializedString);
                object obj = xmlSerializer.Deserialize(stringReader);

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// ���л�DataTable
        /// </summary>
        /// <param name="dataTable">�������ݵ�DataTable</param>
        /// <returns>���л���DataTable</returns>
        public static string GetXmlFormatDt(DataTable dataTable)
        {
            XmlSerializer ser = new XmlSerializer(dataTable.GetType());
            System.IO.MemoryStream mem = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mem, System.Text.Encoding.Default);
            ser.Serialize(writer, dataTable);
            writer.Close();
            return System.Text.Encoding.Default.GetString(mem.ToArray());
        }

        /// <summary>
        /// �����л�DataTable
        /// </summary>
        /// <param name="dt">���л���DataTable</param>
        /// <returns>DataTable</returns>
        public static DataSet GetDtFormatXml(string dt)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(DataSet));
            StreamReader mem = new StreamReader(new MemoryStream(System.Text.Encoding.Default.GetBytes(dt)), System.Text.Encoding.Default);
            return (DataSet)mySerializer.Deserialize(mem);
        }

        /// <summary>
        /// ���л�DataSet����ѹ��
        /// </summary>
        /// <param name="ds">����Դ</param>
        /// <param name="FilePath">���磺D:/DataBak/TestData.dat</param>
        public static bool DataSetSerializer(DataSet ds, string FilePath)
        {
            bool _state = true;
            try
            {
                IFormatter formatter = new BinaryFormatter();//����BinaryFormatter�����л�DataSet����
                MemoryStream ms = new MemoryStream();//�����ڴ�������
                formatter.Serialize(ms, ds);//��DataSet�������л����ڴ���
                byte[] buffer = ms.ToArray();//���ڴ�������д���ֽ�����
                ms.Close();//�ر��ڴ�������
                ms.Dispose();//�ͷ���Դ
                Stream _Stream = File.Open(FilePath, FileMode.Create);//�����ļ�
                GZipStream gzipStream = new GZipStream(_Stream, CompressionMode.Compress, true);//����ѹ������
                gzipStream.Write(buffer, 0, buffer.Length);//��ѹ���������д���ļ�
                gzipStream.Close();//�ر�ѹ����,����Ҫע�⣺һ��Ҫ�رգ�Ҫ��Ȼ��ѹ����ʱ������С��4K���ļ���ȡ�������ݣ�����4K���ļ���ȡ������
                gzipStream.Dispose();//�ͷŶ���
                _Stream.Flush();//�ͷ��ڴ�
                _Stream.Close();//�ر���
                _Stream.Dispose();//�ͷŶ���
            }
            catch (Exception ex)
            {
                _state = false;
            }
            return _state;
        }

        /// <summary>
        /// ���л�DataSet����ѹ��
        /// </summary>
        /// <param name="ds">����Դ</param>
        /// <param name="FilePath">���磺D:/DataBak/TestData.dat</param>
        public static bool DataSetSerializer1(DataSet ds, string FilePath)
        {
            bool _state = true;
            try
            {
                IFormatter formatter = new BinaryFormatter();//����BinaryFormatter�����л�DataSet����
                Stream _Stream = File.Open(FilePath, FileMode.Create);//�����ļ�
                formatter.Serialize(_Stream, ds);//��DataSet�������л����ļ�
                _Stream.Flush();//�ͷ��ڴ�
                _Stream.Close();//�ر���
                _Stream.Dispose();//�ͷŶ���
            }
            catch (Exception ex)
            {
                _state = false;
            }
            return _state;
        }

        /// <summary>
        /// �����л�ѹ����DataSet 
        /// </summary>
        /// <param name="FilePath">���磺D:/DataBak/TestData.dat</param>
        /// <returns></returns>
        public static DataSet DataSetDeserialize(string FilePath)
        {
            Stream _Stream = File.Open(FilePath, FileMode.Open);//���ļ�
            _Stream.Position = 0;//�����ļ�����λ�� 
            GZipStream gzipStream = new GZipStream(_Stream, CompressionMode.Decompress);//������ѹ����
            byte[] buffer = new byte[4096];//�������ݻ���
            int offset = 0;//�����ȡλ�� 
            MemoryStream ms = new MemoryStream();//�����ڴ��� 
            while ((offset = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, offset);//��ѹ�������д���ڴ���   
            }
            IFormatter formatter = new BinaryFormatter();//����BinaryFormatter�Է����л�DataSet����
            ms.Position = 0;//�����ڴ�����λ��
            DataSet ds;
            try
            {
                ds = (DataSet)formatter.Deserialize(ms);//�����л�   
            }
            catch
            {
                ds = null;
            }
            ms.Close();//�ر��ڴ���   
            ms.Dispose();//�ͷ���Դ 
            _Stream.Flush();//�ͷ��ڴ�
            _Stream.Close();//�ر��ļ���   
            _Stream.Dispose();//�ͷ���Դ   
            gzipStream.Close();//�رս�ѹ����   
            gzipStream.Dispose();//�ͷ���Դ   
            return ds;
        }

        /// <summary>
        /// �����л�δѹ����DataSet   
        /// </summary>
        /// <param name="FilePath">���磺D:/DataBak/TestData.dat</param>
        /// <returns></returns>
        public static DataSet DataSetDeserialize1(string FilePath)
        {
            Stream _Stream = File.Open(FilePath, FileMode.Open);//���ļ� 
            _Stream.Position = 0;//�����ļ�����λ�� 
            IFormatter formatter = new BinaryFormatter();//����BinaryFormatter�Է����л�DataSet���� 
            DataSet ds;
            try
            {
                ds = (DataSet)formatter.Deserialize(_Stream);//�����л�   
            }
            catch
            {
                ds = null;
            }
            _Stream.Flush();//�ͷ��ڴ�
            _Stream.Close();//�ر��ļ���   
            _Stream.Dispose();//�ͷ���Դ   
            return ds;
        }

        /// <summary>
        /// ��ӡ����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static  string PrintObject(object obj)
        {
            var objectStr = new StringBuilder();
            Type t = obj.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                object value = pi.GetValue(obj, null);
                string name = pi.Name;
                objectStr.AppendLine(string.Format("{0}:{1}", name, value));
            }
            return objectStr.ToString();
        }
    }
}
