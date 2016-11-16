using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Configuration;
using System.IO;
using System.Data;
using System.Reflection;
using System.Web;

namespace EM.FontBinding.Util
{
    public sealed class Comm
    {
        private static object logobj = new object();
        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string GetConfig(string Name)
        {
            string res = "";
            try
            {

                var tt = ConfigurationManager.AppSettings[Name].ToString();
                if (string.IsNullOrEmpty(tt))
                    return res;
                else
                    res = tt;
            }
            catch (Exception ex)
            {
                LogProvider.GetInstance().Error("GetConfig()未找到,参数名:" + Name);
                //log_write("catch", "GetConfig:" + ex.ToString());
            }
            return res;
        }
        public static bool HasConfig(string Name)
        {
            try
            {

                var tt = ConfigurationManager.AppSettings[Name];
                if (tt != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogProvider.GetInstance().Error("GetConfig()未找到,参数名:" + Name);
                return false;
              
            }

        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text"></param>
        //public static void log_write(string path, string text)
        //{
        //    try
        //    {

        //        lock (logobj)
        //        {
        //            string LogPath = path != "" ? GetConfig("LogPath").ToString() + path + "\\" : GetConfig("LogPath").ToString();
        //            if (!Directory.Exists(LogPath))
        //            {
        //                Directory.CreateDirectory(LogPath);
        //            }
        //            string filename = System.DateTime.Now.ToString("yyyyMMdd");
        //            string fullpath = LogPath + filename + ".txt";
        //            StreamWriter strWriter = new StreamWriter(fullpath, true, System.Text.Encoding.Default);
        //            if (!File.Exists(fullpath))
        //            {
        //                strWriter = File.CreateText(LogPath + filename);
        //            }
        //            strWriter.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + text);
        //            strWriter.Close();
        //        }
        //    }
        //    catch { }
        //}

        /// <summary>
        /// 抓取网页内容
        /// </summary>
        /// <param name="urlPath"></param>
        /// <param name="eCode"></param>
        /// <returns></returns>
        public static string GetHTTPInfo(string urlPath, string eCode)
        {
            string str = "";
            HttpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPath);
                request.Timeout = 10000;
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(eCode));
                str = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                //ex.Message += "GetHTTPInfo(" + urlPath + "):";
                throw ex;
                //log_write("catch", "GetHTTPInfo(" + urlPath + "):" + ex.ToString());
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return str;
        }

        /// <summary>
        /// 写文件（指定编码）
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="FileName"></param>
        /// <param name="text"></param>
        /// <param name="bm"></param>
        public static void WriteFileText(string Path, string FileName, string text, string bm)
        {
            try
            {
                Encoding code = Encoding.GetEncoding(bm);
                using (StreamWriter strWriter = new StreamWriter(Path + FileName, false, code))
                {
                    strWriter.WriteLine(text);
                    strWriter.Close();
                }
            }
            catch (Exception ex)
            {
                LogProvider.GetInstance().Error(ex);
                //log_write("catch", "WriteFileText:" + ex.ToString());
            }
        }

        public static void WriteFileText(string Path,  string text, string bm)
        {
            try
            {
                var direct = System.IO.Path.GetDirectoryName(Path);
                if (!System.IO.Directory.Exists(direct))
                {
                    System.IO.Directory.CreateDirectory(direct);//不存在就创建目录 
                } 
                Encoding code = Encoding.GetEncoding(bm);
                using (StreamWriter strWriter = new StreamWriter(Path, false, code))
                {
                    strWriter.WriteLine(text);
                    strWriter.Close();
                }
            }
            catch (Exception ex)
            {
                LogProvider.GetInstance().Error(ex);
                //log_write("catch", "WriteFileText:" + ex.ToString());
            }
        }

        /// <summary>
        /// 读取硬盘文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static string ReadFileText(string FilePath)
        {
            string StrRead = "";
            try
            {
                using (StreamReader streamopen = new StreamReader(FilePath, System.Text.Encoding.Default))
                {
                    StrRead = streamopen.ReadToEnd().ToString();
                    streamopen.Close();
                }
            }
            catch (Exception ex)
            {
                LogProvider.GetInstance().Error(ex);
                //log_write("catch", "ReadFile:" + FilePath + ":" + ex.ToString());
            }
            return StrRead;
        }

        /// <summary>
        /// 读取硬盘文件(指定编码)
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static string ReadFileText(string FilePath, string bm)
        {
            string StrRead = "";
            try
            {
                Encoding code = Encoding.GetEncoding(bm);
                using (StreamReader streamopen = new StreamReader(FilePath, code))
                {
                    StrRead = streamopen.ReadToEnd().ToString();
                    streamopen.Close();
                }
            }
            catch (Exception ex)
            {
                LogProvider.GetInstance().Error(ex);
                //log_write("catch", "ReadFile:" + FilePath + ":" + ex.ToString());
            }
            return StrRead;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CheckFileExist(string fileName)
        {
            return File.Exists(fileName);
        }

        /// <summary>
        /// 截取指定长度的中文字符串
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <param name="len">截取的长度(中文占2位)</param>
        /// <param name="cstr">省略的代替的字符</param>
        /// <returns></returns>
        public static string CutStr(string str, int len, string cstr)
        {
            if (str == null || str.Length == 0 || len <= 0)
            {
                return string.Empty;
            }
            if (cstr == null)
            {
                cstr = "";
            }
            int l = str.Length;

            #region 计算长度
            int clen = 0;
            while (clen < len && clen < l)
            {
                //每遇到一个中文，则将目标长度减一。
                if ((int)str[clen] > 128) { len--; }
                clen++;
            }
            #endregion

            if (clen < l)
            {
                return str.Substring(0, clen) + cstr;
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 字符串右侧补齐位数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="cstr"></param>
        /// <returns></returns>
        public static string PadRight(string str, int len, char cstr)
        {
            if (str == null || str.Length == 0 || len <= 0)
            {
                return string.Empty;
            }
            int l = str.Length;

            #region 计算长度
            int clen = 0;
            while (clen < len && clen < l)
            {
                //每遇到一个中文，则将目标长度减一。
                if ((int)str[clen] > 128) { len--; }
                clen++;
            }
            #endregion

            if (clen < l)
            {
                return str.Substring(0, clen) + cstr;
            }
            else
            {
                return str.PadRight(len, cstr);
            }
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="defval">默认值</param>
        /// <param name="unit">单位</param>
        /// <returns></returns>
        public static string FormkatValue(string str, string defval, string unit, string fixNum, bool isFix)
        {
            if (str == "-99999999") { return "-"; }
            string res = defval;
            if (str != "" && str != "-")
            {
                if (isFix)
                {
                    bool rtn = false;
                    string _res = FixAmt(str, defval, fixNum, out rtn);
                    if (rtn)
                    {
                        res = _res + unit;
                    }
                    else
                    {
                        res = str;
                    }
                }
                else
                {
                    double value;
                    if (Double.TryParse(str, out value))
                    {
                        res = value.ToString(fixNum) + unit;
                    }
                    else
                    {
                        res = str;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 成交量,成交额修复(只对正数进行判断)
        /// 1234 对应 1234
        /// 12345 对应 1.23万
        /// 123456 对应 12.3万
        /// 1234567 对应 123万
        /// 12345678 对应 1234万
        /// 123456789 对应 1.23亿
        /// 1234567890 对应 12.3亿
        /// 12345678900 对应 123亿
        /// </summary>
        public static string FixAmt(string str, string defval, string fixNum, out bool rtn)
        {
            double value;
            string result = defval;
            if (Double.TryParse(str, out value))
            {
                rtn = true;
                bool fs = value < 0 ? true : false;
                value = Math.Abs(value);
                if (value >= Math.Pow(10, 12))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 12)).ToString(fixNum), "万亿");
                }
                else if (value >= Math.Pow(10, 11))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 8)).ToString(fixNum), "亿");
                }
                else if (value >= Math.Pow(10, 10))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 8)).ToString(fixNum), "亿");
                }
                else if (value >= Math.Pow(10, 9))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 8)).ToString(fixNum), "亿");
                }
                else if (value >= Math.Pow(10, 8))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 8)).ToString(fixNum), "亿");
                }
                else if (value >= Math.Pow(10, 6))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 4)).ToString(fixNum), "万");
                }
                else if (value >= Math.Pow(10, 5))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 4)).ToString(fixNum), "万");
                }
                else if (value >= Math.Pow(10, 4))
                {
                    result = string.Format("{0}{1}", (value / Math.Pow(10, 4)).ToString(fixNum), "万");
                }
                else
                {
                    result = value.ToString(fixNum);
                }
                result = (fs ? "-" : "") + result;
            }
            else
            {
                rtn = false;
            }
            return result;
        }

    
    }
}
