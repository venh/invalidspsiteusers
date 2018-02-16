using System;
using System.IO;
using System.Text;
using System.Configuration;

namespace InvalidSiteUserDetails
{
    class LogHelper
    {
        const int splitNum = 1; // the num of  key, use to split column like tab
        const char splitChar = ','; // tab key,use to split column
        static string strSplitter = string.Empty;

        public static void LogUserDetails(string usrId, string usrFullName, string usrLoginName, string usrEmail)
        {
            string fPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["UserDetailLogName"]);
            strSplitter = ConfigurationManager.AppSettings["Splitter"];
            string[] arr = new string[] { usrId, usrFullName, usrLoginName, usrEmail };
            string strLogContent = string.Join(strSplitter, arr);
            string strFileHeader = ConfigurationManager.AppSettings["UserHeader"];
            if (DoesHeaderExist(fPath, strFileHeader))
                strLogContent = FormatLog(strLogContent, false, strFileHeader);
            else
                //strLogContent = FormatLog(strFileHeader, true, strFileHeader) + "\r\n" + FormatLog(strLogContent, false, strFileHeader);
                strLogContent = FormatLog(strFileHeader, true, strFileHeader) + "\r\n" + strLogContent;

            using (StreamWriter sw = new StreamWriter(fPath, true))
            {
                sw.WriteLine(strLogContent);
                sw.Flush();
                sw.Close();
            }
        }

        static string FormatLog(string inputStr, bool isHeader, string fileHeader)
        {
            string outputStr = string.Empty;
            string[] str = inputStr.Split('|');
            string[] header = fileHeader.Split(strSplitter.ToCharArray());
            string splitTab = GetSplitStr(splitNum, strSplitter.ToCharArray()[0]);
            StringBuilder sbOp = new StringBuilder(str.Length);
            for (int j = 0; j < str.Length; j++)
            {
                if (!isHeader)
                {
                    if (str[j].Length < header[j].Length)
                        str[j] = str[j].PadRight(header[j].Length);
                }
                sbOp.Append(str[j]);
                sbOp.Append(splitTab);
            }
            return sbOp.ToString().TrimEnd(splitChar);
        }

        static string GetSplitStr(int Num, char splitChar)
        {
            StringBuilder sbSplitTab = new StringBuilder(Num);
            for (int i = 1; i <= Num; i++)
            {
                sbSplitTab.Append(splitChar);
            }
            return sbSplitTab.ToString();
        }

        static bool DoesHeaderExist(string filePath, string fileHeader)
        {
            bool flag = false;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string headerLine = reader.ReadLine();
                    string[] arrFileHeader = fileHeader.Split('|');
                    if (string.IsNullOrEmpty(headerLine))
                    {
                        flag = false;
                        reader.Close();
                        return flag;
                    }
                    string[] arrHeaderLine = headerLine.Split(strSplitter.ToCharArray());
                    if (arrFileHeader[0] == arrHeaderLine[0])
                        flag = true;
                    else
                        flag = false;
                    reader.Close();
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }
    }
}
