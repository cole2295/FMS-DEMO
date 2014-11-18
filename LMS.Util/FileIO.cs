using System.IO;
using System.Text;

namespace RFD.FMS.Util
{
	/// <summary>
	/// �ļ�I/O����
	/// </summary>
	public  class FileIO
	{
		/// <summary>
		/// ������������������ļ�
		/// </summary>
		/// <param name="bitArray">binary array</param>
		/// <param name="savePath">file directory</param>
		public static void SaveBytes(byte[] bitArray, string savePath)
		{
			FileStream fs = new FileStream(savePath, FileMode.Create);
			BinaryWriter bw = new BinaryWriter(fs);
			bw.Write(bitArray);
			fs.Close();
			fs.Dispose();
			bw.Close();
		}

		/// <summary>
		/// �����ı��ļ�
		/// </summary>
		/// <param name="fileName">file directory</param>
		/// <param name="txt">file content</param>
		/// <param name="encoding">encoding</param>
		public static void SaveTextFile(string fileName, string txt,Encoding encoding)
		{
			using (StreamWriter sw = new StreamWriter(fileName, false, encoding))
			{
				sw.Write(txt);
			}
		}
	}
}