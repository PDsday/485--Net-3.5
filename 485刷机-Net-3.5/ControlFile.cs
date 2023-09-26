using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _485刷机_Net_3._5
{
    /***********************文件夹与文件********************************
     * 说明：创造文件夹和生成文件
     *                                          2022/07/26--LTT
     ******************************************************************/

    class ControlFile
    {
        #region 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="name">文件夹名</param>
        public static string CreateFolder(string dirPath, string name)
        {
            string Result = "ok";

            foreach (string d in Directory.GetFileSystemEntries(dirPath))
            {
                if (File.Exists(dirPath + @"" + name))
                {
                    Console.WriteLine("创建文件夹 " + name + " 失败,文件夹已经存在");
                    Result = "ds";
                    return Result;
                }
            }
            DirectoryInfo info = new DirectoryInfo(dirPath);
            info.CreateSubdirectory(name);
            return Result;
        }
        #endregion

        #region 文本框导出txt文件
        //输入：文本框对象
        public static string SaveTxtFilesFromTextBox(RichTextBox textbox)
        {
            try
            {
                // "保存为"对话框
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "文本文件|*.txt";
                dialog.FileName = "文本";
                // 显示对话框
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 文件名
                    string fileName = dialog.FileName;
                    // 创建文件，准备写入
                    FileStream fs = File.Open(fileName,
                            FileMode.Create,
                            FileAccess.Write);
                    StreamWriter wr = new StreamWriter(fs);

                    // 逐行将textBox1的内容写入到文件中
                    foreach (string line in textbox.Lines)
                    {
                        wr.WriteLine(line);
                    }

                    // 关闭文件
                    wr.Flush();
                    wr.Close();
                    fs.Close();

                    return "ok";
                }
                else
                {
                    return "qt";
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
        #endregion


    }
}
