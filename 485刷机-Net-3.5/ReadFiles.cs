using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace _485刷机_Net_3._5
{
    class ReadFiles
    {
        #region 主控板ARM和充电板ARM截取Bin文件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RedirectionAddr">目标程序的执行地址 </param>
        /// <param name="CodeStartAddr">代码包的起始地址 </param>
        /// <param name="CodeFileSize">目标刷机文件的源文件大小 </param>
        /// <param name="BootloaderHeadFlag"> 引导文件的头标志位 </param>
        /// <param name="SourceFileData"> 刷机的目标文件 </param>
        /// <param name="filedata"> 输出截取的目标代码片段 </param>
        /// <param name="IniRecordBootloaderSize"> 用于简单的自校验的参数 </param>
        /// <returns></returns>
        public static string FragmentCode(UInt64 RedirectionAddr, UInt64 CodeStartAddr, UInt64 CodeFileSize, UInt64 IniRecordBootloaderSize, byte[] BootloaderHeadFlag, byte[] SourceFileData, out byte[] filedata)
        {
            UInt64 BootloaderPackSize = RedirectionAddr - CodeStartAddr;
            UInt64 CodeLength = CodeFileSize - BootloaderPackSize;
            Console.WriteLine($"BootloaderPackSize = {BootloaderPackSize}.");
            Console.WriteLine($"CodeLength = {CodeLength}.");
            Console.WriteLine($"CodeFileSize = {CodeFileSize}.");
            byte[] ResultFileData = new byte[CodeLength];//用来保存截取文件的数组
            string AckMessage = "ok";
            filedata = new byte[CodeLength];

            try
            {
                //for (int y = 0; y < 16; y++)
                //{
                //    Console.WriteLine($"SourceFileData = {SourceFileData[y]}.");
                //    Console.WriteLine($"BootloaderHeadFlag = {BootloaderHeadFlag[y]}.");
                //}
                //判断引导程序的前16个数据是否一致
                //for (int i = 0; i < 16; i++)
                //{
                //    if (BootloaderHeadFlag[i] != SourceFileData[i])
                //    {
                //        //Console.WriteLine($"BootloaderHeadFlag[i] = {BootloaderHeadFlag[i]}.");
                //        //Console.WriteLine($"SourceFileData[i] = {SourceFileData[i]}.");
                //        AckMessage = "head_err";
                //        return AckMessage;
                //    }
                //}
                //if (Commentclass.ChexkActionEnable)
                //{     
                    ////判断某一段是否有0xff
                    //for (UInt64 k = (UInt64)(BootloaderPackSize - Commentclass.CommentHeadCheckNumLength); k < BootloaderPackSize; k++)
                    //{
                    //    if (SourceFileData[k] != 0xff)
                    //    {
                    //        AckMessage = "head_0xff_err:当前可能不是合并目标文件，请确认。";
                    //        return AckMessage;
                    //    }
                    //    Console.WriteLine($"SourceFileData = {SourceFileData[k]}.");
                    //}
                //}
                //开始截取
                Buffer.BlockCopy(SourceFileData, (int)BootloaderPackSize, ResultFileData, 0, ResultFileData.Length);
                //返回截取的文件
                //filedata = new byte[ResultFileData.Length];
                Buffer.BlockCopy(ResultFileData, 0, filedata, 0, ResultFileData.Length);
                Array.Clear(ResultFileData, 0, ResultFileData.Length);                               //清除

                //自校验截取的代码包是否正确
                if ((CodeFileSize - (UInt64)ResultFileData.Length) != IniRecordBootloaderSize)
                {
                    Console.WriteLine($"IniRecordBootloaderSize = {IniRecordBootloaderSize}.");
                    Console.WriteLine($"IniRecordBootloaderSize0 = {(CodeFileSize - (UInt64)ResultFileData.Length)}.");
                    // MessageBox.Show("自校验错误");
                    AckMessage = "checkself_err";
                    return AckMessage;
                }

                return AckMessage;
            }
            catch (Exception err)
            {
                return err.Message;
            }

        }
        #endregion

        #region 读取文件
        public static string CommentReadFiles(string filepath, out byte[] filedata, out UInt32 filesize)
        {
            string Result = "ok";
            filedata = new byte[1];                                  //初始化
            filesize = 0x00;                                         //初始化
            byte[] Tempfiledata = new byte[1024 * 1024 * 8];         //创建8M的缓存
            try
            {
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                int length = fs.Read(Tempfiledata, 0, Tempfiledata.Length);
                filedata = new byte[length];
                fs.Close();
                Buffer.BlockCopy(Tempfiledata, 0, filedata, 0, length);    // 将arrfileSend复制到arrfile之中
                filesize = (UInt32)length;
            }
            catch (Exception err)
            {
                return err.Message;
            }

            return Result;
        }
        #endregion

        #region 打开文件
        public static string CommentOpenFiles(string failpath, out string realypath)
        {
            string Result = "ok";
            string Cancel = "ds";
            realypath = "D:\\";
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (string.IsNullOrEmpty(failpath))
                {
                    ofd.InitialDirectory = ("D:\\");
                }
                else
                {
                    int i = failpath.LastIndexOf("\\");             //获取字符串最后一个斜杠的位置
                    string path = failpath.Substring(0, i);         //取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                    ofd.InitialDirectory = (path);
                }
                ofd.Filter = "|*.bin;*.hex";                               //设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    realypath = ofd.FileName;
                    // WritePrivateProfileString(strOne, "DSPData_Source", pathtextBox.Text, str);
                    Debug.WriteLine("已打开文件");
                }
                else
                {
                    realypath = " ";
                    return Cancel;
                    Debug.WriteLine("取消打开文件");
                }
            }
            catch (Exception err)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = ("D:\\");
                ofd.Filter = "|*.bin;*.hex";                               //设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    realypath = ofd.FileName;
                    // WritePrivateProfileString(strOne, "DSPData_Source", pathtextBox.Text, str);
                    Debug.WriteLine("已打开文件");
                }
                else
                {
                    realypath = " ";
                    return Cancel;
                    Debug.WriteLine("取消打开文件");
                }
                return err.Message;
                return err.Message;
            }
            return Result;
        }

        public static string CommentOpenFiles(string failpath, out string realypath,string filter)
        {
            string Result = "ok";
            string Cancel = "ds";
            realypath = "D:\\";
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (string.IsNullOrEmpty(failpath))
                {
                    ofd.InitialDirectory = ("D:\\");
                }
                else
                {
                    int i = failpath.LastIndexOf("\\");             //获取字符串最后一个斜杠的位置
                    string path = failpath.Substring(0, i);         //取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                    ofd.InitialDirectory = (path);
                }
                ofd.Filter = filter;                               //设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    realypath = ofd.FileName;
                    // WritePrivateProfileString(strOne, "DSPData_Source", pathtextBox.Text, str);
                    Debug.WriteLine("已打开文件");
                }
                else
                {
                    realypath = " ";
                    return Cancel;
                    Debug.WriteLine("取消打开文件");
                }
            }
            catch (Exception err)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = ("D:\\");
                ofd.Filter = filter;                               //设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    realypath = ofd.FileName;
                    // WritePrivateProfileString(strOne, "DSPData_Source", pathtextBox.Text, str);
                    Debug.WriteLine("已打开文件");
                }
                else
                {
                    realypath = " ";
                    return Cancel;
                    Debug.WriteLine("取消打开文件");
                }
                return err.Message;

            }
            return Result;
        }
        #endregion

        #region 防止Bin文件误操作导入
        /****************************************************************************
        * 
        * 函数名称：
        * 
        * 输入：FilesSize：文件大小            
        *       TargetObject：刷机对象
        *       
        * 返回：无
        ******************************************************************************/
        //public static string DocumentJudgment(UInt32 FilesSize, UInt16 TargetObject)
        //{
        //    string ResultOne = "yes";
        //    string ResultTwo = "no";
        //    try
        //    {
        //        switch (TargetObject)
        //        {
        //            case 0x00://主控板ARM
        //                ResultOne = "yes";
        //                //if (FilesSize < 92160 || FilesSize > 204800)//文件小于90k的和大于200k的都不行
        //                //{
        //                //    return ResultTwo;
        //                //}
        //                break;
        //            case 0x01://主控板DSP
        //                if (FilesSize != 524288)
        //                {
        //                    return ResultTwo;
        //                }
        //                break;
        //            case 0x02://充电板
        //                if (FilesSize > 92160)//文件大于90k
        //                {

        //                    return ResultTwo;
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        return err.Message;
        //    }
        //    return ResultOne;
        //}
        #endregion

        #region DSP需要擦除扇区计算，以及计算出重新需要提取刷机文件的大小
        /****************************************************************************
         * 
         * 函数名称：
         * 
         * 输入：FilseData：读取的文件数据      
         *       FilesSize：文件大小
         *       Num：需要擦除的扇区数
         *       NewFilesSize：计算出来的新的刷机文件的大小
         * 返回：无
         ******************************************************************************/
        public static string AbraseSectorNum(byte[] FilseData, UInt32 FilesSize, out byte Num, out UInt32 NewFilesSize)
        {
            string Result = "ok";
            Num = 0x00;             //初始化
            NewFilesSize = 0x00;    //初始化
            UInt32 index = FilesSize - 65536 - 1;
            try
            {
                for (; index > 0;)
                {
                    if (FilseData[index] != 0x00 && FilseData[index] != 0xff)
                    {
                        index = index + 128;
                        NewFilesSize = index;       //返回新的文件需要规定的文件大小
                        Num = (byte)((byte)System.Math.Ceiling((double)index / (double)65536));
                        break;
                    }
                    index = index - 1;
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
            return Result;
        }
        #endregion

        #region 主控板ARM和充电板ARM截取Bin文件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RedirectionAddr">目标程序的执行地址 </param>
        /// <param name="CodeStartAddr">代码包的起始地址 </param>
        /// <param name="CodeFileSize">目标刷机文件的源文件大小 </param>
        /// <param name="BootloaderHeadFlag"> 引导文件的头标志位 </param>
        /// <param name="SourceFileData"> 刷机的目标文件 </param>
        /// <param name="filedata"> 输出截取的目标代码片段 </param>
        /// <param name="IniRecordBootloaderSize"> 用于简单的自校验的参数 </param>
        /// <returns></returns>
        //public static string FragmentCode(UInt64 RedirectionAddr, UInt64 CodeStartAddr, UInt64 CodeFileSize, UInt64 IniRecordBootloaderSize, byte[] BootloaderHeadFlag, byte[] SourceFileData, out byte[] filedata)
        //{
        //    UInt64 BootloaderPackSize = RedirectionAddr - CodeStartAddr;
        //    UInt64 CodeLength = CodeFileSize - BootloaderPackSize;
        //    Console.WriteLine($"BootloaderPackSize = {BootloaderPackSize}.");
        //    Console.WriteLine($"CodeLength = {CodeLength}.");
        //    Console.WriteLine($"CodeFileSize = {CodeFileSize}.");
        //    byte[] ResultFileData = new byte[CodeLength];//用来保存截取文件的数组
        //    string AckMessage = "ok";
        //    filedata = new byte[CodeLength];

        //    try
        //    {
        //        //for (int y = 0; y < 16; y++)
        //        //{
        //        //    Console.WriteLine($"SourceFileData = {SourceFileData[y]}.");
        //        //    Console.WriteLine($"BootloaderHeadFlag = {BootloaderHeadFlag[y]}.");
        //        //}
        //        //判断引导程序的前16个数据是否一致
        //        // for (int i = 0; i < 16; i++)
        //        //{
        //        //     if (BootloaderHeadFlag[i] != SourceFileData[i])
        //        //     {
        //        //         //Console.WriteLine($"BootloaderHeadFlag[i] = {BootloaderHeadFlag[i]}.");
        //        //         //Console.WriteLine($"SourceFileData[i] = {SourceFileData[i]}.");
        //        //         AckMessage = "head_err";
        //        //         return  AckMessage;
        //        //     }
        //        //}
        //        //判断某一段是否有0xff
        //        for (UInt64 k = (UInt64)(BootloaderPackSize - Commentclass.CommentChekNum); k < BootloaderPackSize; k++)
        //        {
        //            if (SourceFileData[k] != 0xff)
        //            {
        //                AckMessage = "0xff_err";
        //                return AckMessage;
        //            }
        //            Console.WriteLine($"SourceFileData = {SourceFileData[k]}.");
        //        }

        //        //开始截取
        //        Buffer.BlockCopy(SourceFileData, (int)BootloaderPackSize, ResultFileData, 0, ResultFileData.Length);
        //        //返回截取的文件
        //        //filedata = new byte[ResultFileData.Length];
        //        Buffer.BlockCopy(ResultFileData, 0, filedata, 0, ResultFileData.Length);
        //        Array.Clear(ResultFileData, 0, ResultFileData.Length);                               //清除

        //        //自校验截取的代码包是否正确
        //        if ((CodeFileSize - (UInt64)ResultFileData.Length) != IniRecordBootloaderSize)
        //        {
        //            Console.WriteLine($"IniRecordBootloaderSize = {IniRecordBootloaderSize}.");
        //            Console.WriteLine($"IniRecordBootloaderSize0 = {(CodeFileSize - (UInt64)ResultFileData.Length)}.");
        //            // MessageBox.Show("自校验错误");
        //            AckMessage = "checkself_err";
        //            return AckMessage;
        //        }

        //        return AckMessage;
        //    }
        //    catch (Exception err)
        //    {
        //        return err.Message;
        //    }

        //}
        #endregion




    }
}
