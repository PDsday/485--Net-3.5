using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace _485刷机_Net_3._5
{
    class ReadINIFiles
    {

        /***************************************ini文件结构简析************************************************
        * 
        * [section_1]----:节名
        * section_1_key=section_1_value--：数据
        *      |
        *     健名
        ******************************************************************************************************/
        #region API函数声明-必须放在类中
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
          string val, string filePath);
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
          string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region 读Ini文件
        /// <summary>
        /// 读取ini文件内容的方法
        /// </summary>
        /// <param name="Section">ini文件的节名</param>
        /// <param name="Key">ini文件对应节下的健名</param>
        /// <param name="NoText">ini文件对应节对应健下无内容时返回的值</param>
        /// <param name="iniFilePath">该ini文件的路径</param>
        /// <returns></returns>
        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))//判断当前ini文件是否存在
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }
        #endregion
        #region 写Ini文件
        /// <summary>
        /// 将内容写入指定的ini文件中
        /// </summary>
        /// <param name="Section">ini文件中的节名</param>
        /// <param name="Key">ini文件中的键</param>
        /// <param name="Value">要写入该键所对应的值</param>
        /// <param name="iniFilePath">ini文件路径</param>
        /// <returns></returns>
        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))//判断当前ini文件是否存在
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

    }
}
