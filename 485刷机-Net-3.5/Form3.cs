using _485刷机_Net_3._5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using View;

namespace _485刷机_Net_3._5
{
    //声明委托
    delegate void RecMsgDelegate(string str);
    delegate void barDelegate(int str);
    public partial class Form3 : Form
    {
        RecMsgDelegate myRcvMsg;            //委托
        barDelegate barDelegate;

        #region 定义modbus格式命令
        //升级指令
        public byte[] mod_Upgradeorde = new byte[15] { 0X00, 0X01, 0X00, 0X00, 0X00, 0X09, 0X00, 0X6D, 0XC0, 0X07, 0X00, 0X01, 0X02, 0XAA, 0X55 };
        //跳转指令
        public byte[] mod_Jmpbtolorde = new byte[15] { 0X00, 0X01, 0X00, 0X00, 0X00, 0X09, 0X00, 0X6D, 0XC0, 0X07, 0X00, 0X01, 0X02, 0X55, 0XAA };
        //解锁指令
        public byte[] mod_Unlockorde = new byte[15] { 0X00, 0X01, 0X00, 0X00, 0X00, 0X09, 0X00, 0X6D, 0XC0, 0X07, 0X00, 0X01, 0X02, 0X50, 0XA0 };
        //擦除指令
        public byte[] mod_Ereaseorde = new byte[15] { 0X00, 0X01, 0X00, 0X00, 0X00, 0X09, 0X00, 0X6D, 0XC0, 0X07, 0X00, 0X01, 0X02, 0XA0, 0X50 };
        //跳转用户程序的命令
        public byte[] mod_Jmpapbegin = new byte[15] { 0X00, 0X01, 0X00, 0X00, 0X00, 0X09, 0X00, 0X6D, 0XC0, 0X07, 0X00, 0X01, 0X02, 0X5A, 0X5A };
        #endregion

        #region 以太网公共的变量以及函数的定义
        delegate void RecMsgDelegate(string str);   //声明委托
        //delegate void barDelegate(int str);         //声明委托
        private bool IsRuning = true;               //运行标志位
        Socket sockClient = null;                   //socket对象
        Thread thrClient = null;                    //接收线程
        Thread AckSend_Function = null;             //应答式接发线程
        #endregion

        #region 变量定义区
        public static byte[] filedata;                       //创建保存读取的文件数据的缓存空间
        public static byte[] SneCapre;                       //用于将发送的数据和接收的数据进行保存
        public byte[] CrcCheck = new byte[2];
        byte ErrTryCount = 0;                                //错误重发，最多三次
        int step_i = 0;                                      //发送的次数 
        double duty = 0;                                     //发送文件的进度值
        int pagenum = 0;                                     //计算需要发送多少次
        int remaind = 0;                                     //计算是否发送次数不为整数
        int cjremaind = 0;                                   //承接remaind
        public byte[] filedata_temp = new byte[128];         //承接BIN文件被按照128个字节分开后每次的数据 
        int flashnum = 0;                                    //需要擦除的扇区数
        int unlocktime = 0;
        int netconnect = 0;
        bool pingack_flag = false;                          //ping通的标志位
        bool breakflag = false;                             //断开以太网标志
        bool fivesflag = false;                             //5s定时到达标志位
        int fivecount = 0;

        #endregion

        #region 标志位定义区
        bool Connect = true;                                 //应答式接收的运行标志位
        bool System_RS = true;                               //判断系统能否继续进行 
        bool First_Run = true;                               //判断是否第一次点击下载
        int LinkCount = 0;                                  //连接无线重连次数
        int ssSeconds = 0;                                  //30S计时
        bool SocketOn = false;                               //判断Socket线程是否开启
        bool Erasflag = false;                               //擦除失败标志位
        bool Ulockflg = false;                               //解锁失败标志位
        bool step_send = false;
        bool powerdisplay = false;                           //隐藏显示某些东西
        bool firstopen = true;                               //上位机是否第一次打开
        bool canset = false;
        #endregion

        #region 步骤定义区
        bool Step_One = true;                                //第一步的状态
        bool Step_Tow = false;                               //第二步的状态
        bool Step_Thr = false;                               //第三步的状态
        bool Step_Fou = false;                               //第四步的状态
        bool Step_Fiv = false;                               //第五步的状态
        bool Step_Six = false;                               //第六步的状态
        #endregion

        #region 窗体位置
        static public int fm3windowX = 0;
        static public int fm3windowY = 0;
        #endregion

        #region 共用变量
        bool loadtimecount = false;                          //计时标志位
        int loadtime = 0;
        #endregion

        #region INI变量声明区
        public string str = Application.StartupPath + "\\ConnectString.ini";//该变量保存INI文件所在的具体物理位置
        public string strOne = "";
        public string IniFilesPath = Application.StartupPath + "\\ConnectString.ini";
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        public string ContentReader(string area, string key, string def)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            GetPrivateProfileString(area, key, def, stringBuilder, 1024, str);
            return stringBuilder.ToString();
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string mpAppName,
            string mpKeyName,
            string mpDefault,
            string mpFileName);

        #endregion

        public Form3()
        {
            InitializeComponent();
            myRcvMsg = RecMsg;
            barDelegate = Barcount;
            //databox.Hide();
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.filebutton, "支持文件*.bin拖放");
            this.databox.ContextMenuStrip = this.David_contextMenuStrip2;   
        }

        #region 擦除进度显示
        void Barcount(int time)
        {
            loadprogressBar.Invoke
            (
                //委托，托管无参数的任何方法
                new MethodInvoker
                (
                    delegate
                    {
                        //toolStripProgressBar1.Value = time;
                        string strText = "擦除中..........";
                        Font font = new Font("微软雅黑", (float)10, FontStyle.Regular);
                        PointF pointF = new PointF(this.loadprogressBar.Width / 2 - 10, this.loadprogressBar.Height / 2 - 10);
                        loadprogressBar.Value = time;
                        this.loadprogressBar.CreateGraphics().DrawString(strText, font, Brushes.Black, pointF);
                        Console.WriteLine($"time = {time}.");
                    }
                )
            );
        }
        #endregion

        #region 擦除标识
        private void display_run()
        {

            loadprogressBar.Maximum = 80 * flashnum;           //重定义进度条的最大值

        }
        #endregion

        #region 共用的连接服务端函数
        public void Comment_Net_Link()
        {
            try
            {
                IPAddress address = IPAddress.Parse(this.ipaddres.Text.Trim());                     //连接IP
                IPEndPoint Ipe = new IPEndPoint(address, 502);                                      //连接端口
                sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);       //将对象实例化

                sockClient.Connect(Ipe);
                //downbutton.Enabled = true;
                thrClient = new Thread(ReceiceMsg);
                thrClient.IsBackground = true;
                thrClient.Start();
                SocketOn = true;

            }
            catch 
            {
                enable_function();
            }
            finally
            {
              
            }
        }
        #endregion

        #region 以太网接收消息
        private void ReceiceMsg()
        {
            while (IsRuning)
            {
                try
                {
                    byte[] arrMsgRec = new byte[1024 * 1024 * 2];           //定义一个2M的缓冲区
                    int length = sockClient.Receive(arrMsgRec);
                    if (length == 0) return;
                    Console.WriteLine($"length = {length}.");

                    byte[] Resive_data = new byte[length];
                    Array.Copy(arrMsgRec, Resive_data, length);

                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                    {
                        if (powerdisplay)
                        {
                            databox.AppendText("接收：" + StartsCRC.zftring(Resive_data) + "\r\n\r\n");            //显示接收到的数据
                        }
                        if (Step_One)
                        {
                            byte[] temp_ston = new byte[2];
                            Buffer.BlockCopy(modtcp_alive.Res_Adjt_ARM(0x01, Resive_data), 0, temp_ston, 0, 2);
                            Console.WriteLine($"temp_ston[0] = {temp_ston[0]}.");
                            Console.WriteLine($"temp_ston[1] = {temp_ston[1]}.");
                            if (temp_ston[0] == 0XAA && temp_ston[1] == 0X55)
                            {
                                Step_One = false;
                                Step_Tow = false;
                                Step_Thr = true;
                                LinkCount = 0;
                                Console.WriteLine($"no ack no lock");
                            }
                            else if (temp_ston[0] == 0X00 && temp_ston[1] == 0X00)
                            {
                                Step_One = false;
                                Step_Tow = true;
                                LinkCount = 0;
                            }
                        }
                        else if (Step_Thr)
                        {
                            byte[] temp_ston = new byte[2];
                            Buffer.BlockCopy(modtcp_alive.Res_Adjt_ARM(0x01, Resive_data), 0, temp_ston, 0, 2);
                            if (temp_ston[0] == 0X00 && temp_ston[1] == 0X00)
                            {
                                Step_Thr = false;
                                Step_Fou = true;
                                LinkCount = 0;
                            }
                            else if (temp_ston[0] == 0X00 && temp_ston[1] == 0X0A)
                            {
                                Ulockflg = true;
                            }
                        }
                        else if (Step_Fou)
                        {
                            byte[] temp_ston = new byte[2];
                            Buffer.BlockCopy(modtcp_alive.Res_Adjt_ARM(0x01, Resive_data), 0, temp_ston, 0, 2);
                            if (temp_ston[0] == 0X00 && temp_ston[1] == 0X00)
                            {               
                                loadprogressBar.Maximum = pagenum - 1;                   //进度条的最大值初始化
                                Step_Fou = false;
                                Step_Fiv = true;
                                toolStripStatusLabel1.Text = "程序下载中.......";
                            }
                            else if (temp_ston[0] == 0X00 && temp_ston[1] == 0X16)
                            {
                                Erasflag = true;
                            }
                        }
                        else if (Step_Fiv)
                        {
                            //发包文件回传确认校验对错
                            byte[] temp_stow = new byte[2];
                            Buffer.BlockCopy(modtcp_alive.Res_Adjt_ARM(0x01, Resive_data), 0, temp_stow, 0, 2);

                            if (temp_stow[0] == 0X00 && temp_stow[1] == 0X00)
                            {
                                System_RS = true;
                                timer5.Stop();
                                Debug.WriteLine("传输正确！");

                                if (!powerdisplay)
                                    databox.AppendText(Commentclass.DSPStateMessageList[4] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");            //显示接收到的数据
                            }
                            else if (temp_stow[1] == 0X1E || temp_stow[1] == 0X28)         //编程失败直接退出
                            {
                                Close_SocketTr();               //关socket线程
                                timer5.Stop();
                                if (!powerdisplay)
                                    databox.AppendText(Commentclass.DSPStateMessageList[8] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");

                                MessageBoxMidle.Show(this, "程序升级失败！", "错误");
                                menuStrip1.Enabled = true;
                                toolStripStatusLabel1.Text = "程序升级失败！";
                            }
                            else if (temp_stow[1] == 0XFF && temp_stow[1] == 0XFF)          //校验失败，重发三次
                            {
                                if (ErrTryCount <= 3)
                                {
                                    ErrTryCount++;
                                    if (ErrTryCount > 1)
                                    {
                                        databox.AppendText(Commentclass.DSPStateMessageList[9] + DateTime.Now.ToString("hh:mm:ss") + "\r\n");
                                    }
                                    Debug.WriteLine("传输错误！");
                                    timer2.Start();                 //进入定时器重发3次操作
                                }
                                else
                                {
                                    Connect = false;
                                    timer5.Stop();
                                    timer2.Stop();                  //定时器溢出区判断校验是否通过
                                    AckSend_Function.Abort();       //关闭线程
                                    Close_SocketTr();               //关socket线程
                                    toolStripStatusLabel1.Text = "程序升级失败！";
                                    MessageBoxMidle.Show(this, "程序升级失败！", "错误");
                                    if (!powerdisplay)
                                        databox.AppendText(Commentclass.DSPStateMessageList[8] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                                    menuStrip1.Enabled = true;
                                    loadprogressBar.Value = 0;
                                    ErrTryCount = 0;
                                }
                            }

                        }
                    })));
                }
                catch (SocketException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
        #endregion

        #region 打开bin文件并保存到建立的数组之中
        private void filebutton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();

                if (string.IsNullOrEmpty(pathtextBox.Text))
                {
                    Debug.WriteLine("kong huo zhe wenjian jia bu cun zai ");
                    ofd.InitialDirectory = ("D:\\");
                }
                else
                {
                    int i = pathtextBox.Text.LastIndexOf("\\");//获取字符串最后一个斜杠的位置
                    string path = pathtextBox.Text.Substring(0, i);//取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                    ofd.InitialDirectory = (path);
                }
                ofd.Filter = "|*.bin";//设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.pathtextBox.Text = ofd.FileName;
                    WritePrivateProfileString(strOne, "DSPData_Source", pathtextBox.Text, str);
                    Debug.WriteLine("已打开文件");
                }
                else
                {
                    Debug.WriteLine("取消打开文件");
                    return;
                }
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show("文件路径错误，请检查文件路径格式。", "路径错误");
            }
            try
            {
                #region 作废
                //FileStream fs = new FileStream(pathtextBox.Text, FileMode.Open, FileAccess.Read);
                //byte[] arrfileSend = new byte[1024 * 1024 * 8];         //创建8M的缓存
                //int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                //byte[] arrfile = new byte[length];
                //fs.Close();                                              //关闭文件流，释放文件

                //Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);    // 将arrfileSend复制到arrfile之中
                //byte[] tempdata = new byte[length];
                //Buffer.BlockCopy(arrfile, 0, tempdata, 0, length);      // 将arrfile复制到tempdata之中
                //int index = tempdata.Length - 65536 - 1;                //tempdata.Length个8bit的数据

                ////=================文件处理代码部分===========================================
                //for (; index > 0;)
                //{
                //    if (tempdata[index] != 0)
                //    {
                //        index = index + 128;
                //        //toolStripStatusLabel1.Text ="擦除扇区数："+ (System.Math.Ceiling((double)index / (double)65536)).ToString();
                //        mod_Ereaseorde[14] = (byte)(mod_Ereaseorde[14] + (byte)System.Math.Ceiling((double)index / (double)65536));
                //        flashnum = (int)System.Math.Ceiling((double)index / (double)65536);
                //        //for (int ad = 0; ad <= index; ad++)
                //        //{
                //        //    Console.WriteLine($"filedata = {tempdata[ad]}.");
                //        //    Invoke(myRcvMsg, StartsCRC.byteToHexStr(tempdata[ad]));
                //        //}
                //        break;
                //    }
                //    index = index - 1;
                //}
                ////=================文件处理代码部分===========================================
                //filedata = new byte[index];
                //Buffer.BlockCopy(tempdata, 0, filedata, 0, index);   // 将arrfileSend复制到arrfile之中
                //Console.WriteLine($"filedata = {filedata.Length}.");
                //Console.WriteLine($"index = {index}.");
                ////for (int ad = 0; ad < index; ad++)
                ////{
                ////    //Console.WriteLine($"filedata = {filedata[ad]}.");
                ////    Invoke(myRcvMsg, StartsCRC.byteToHexStr(filedata[ad]));
                ////}
                ////Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);   // 将arrfileSend复制到arrfile之中
                ////Console.WriteLine($"filedata = {filedata.Length}.");
                //step_i = 0;                                              //初始化
                //duty = 0;                                                //初始化
                //pagenum = filedata.Length / 128;                                  //初始化  以128个字节为一个数据包发送数据
                //remaind = filedata.Length % 128;                                  //初始化

                //cjremaind = remaind;
                //Console.WriteLine($"remaind = {remaind}.");
                //Console.WriteLine($"pagenum = {pagenum}.");
                //loadprogressBar.Maximum = pagenum - 1;                   //进度条的最大值初始化
                //toolStripStatusLabel1.Text = "擦除扇区数：" + (System.Math.Ceiling((double)index / (double)65536)).ToString() + "  " + "文件已导入，请确认！";
                #endregion
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开文件失败！", "错误");
                toolStripStatusLabel1.Text = "文件导入失败，请检查！";
            }
        }
        #endregion

        #region 默认路径读取文件
        private void load_file_onthepath()
        {
            try
            {
                FileStream fs = new FileStream(pathtextBox.Text, FileMode.Open, FileAccess.Read);
                byte[] arrfileSend = new byte[1024 * 1024 * 8];             //创建8M的缓存
                int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                byte[] arrfile = new byte[length];
                fs.Close();                                                 //关闭文件流，释放文件
                mod_Ereaseorde[14] = 0x50;
                Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);       // 将arrfileSend复制到arrfile之中
                byte[] tempdata = new byte[length];
                Buffer.BlockCopy(arrfile, 0, tempdata, 0, length);          // 将arrfile复制到tempdata之中
                int index = tempdata.Length - 65536 - 1;                    //tempdata.Length个8bit的数据

                if (tempdata.Length < (int)Commentclass.DspNormalSize || tempdata.Length > (int)Commentclass.DspNormalSize)
                {
                    MessageBoxMidle.Show("导入文件大小错误，非DSP刷机文件！\r\n" + "请重新导入。", "文件错误");
                    enable_function();
                    return;
                }

                //提示
                DialogResult messdr = MessageBoxMidle.Show(this, "请先打手动开关进入手动模式！\r\n是否已完成该操作？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (messdr == DialogResult.No)
                {
                    enable_function();
                    return;
                }

                //================================文件处理代码部分===========================================
                for (; index > 0;)
                {
                    if (tempdata[index] != 0 && tempdata[index] != 0xff)
                    {
                        index = index + 128;           
                        mod_Ereaseorde[14] = (byte)(mod_Ereaseorde[14] + (byte)System.Math.Ceiling((double)index / (double)65536));
                        flashnum = (int)System.Math.Ceiling((double)index / (double)65536);
                        break;
                    }
                    index = index - 1;
                }
                //=================文件处理代码部分===========================================
                filedata = new byte[index];
                Buffer.BlockCopy(tempdata, 0, filedata, 0, index);   // 将arrfileSend复制到arrfile之中
                Console.WriteLine($"filedata = {filedata.Length}.");
                Console.WriteLine($"index = {index}.");

                //for (int ad = 0; ad < index; ad++)
                //{
                //    Console.WriteLine($"filedata = {filedata[ad]}.");
                //    Invoke(myRcvMsg, StartsCRC.byteToHexStr(filedata[ad]));
                //}
                //Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);   // 将arrfileSend复制到arrfile之中
                //Console.WriteLine($"filedata = {filedata.Length}.");

                step_i = 0;                                                       //初始化
                duty = 0;                                                         //初始化
                pagenum = filedata.Length / 128;                                  //初始化  以128个字节为一个数据包发送数据
                remaind = filedata.Length % 128;                                  //初始化

                cjremaind = remaind;
                Console.WriteLine($"remaind = {remaind}.");
                Console.WriteLine($"pagenum = {pagenum}.");
                loadprogressBar.Maximum = pagenum - 1;                             //进度条的最大值初始化
                toolStripStatusLabel1.Text = "擦除扇区数：" + (System.Math.Ceiling((double)index / (double)65536)).ToString() + "  " + "文件已导入，请确认！";
                timer4.Start();
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开文件失败！" , "错误");
                enable_function();
                toolStripStatusLabel1.Text = "打开文件失败，请检查！";
                return;
            }
        }
        #endregion

        #region 文件拖放
        private void pathtextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        #endregion

        #region 文件拖放
        private void pathtextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                pathtextBox.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                if (Path.GetExtension(pathtextBox.Text) != ".bin")//如果是.hex文件
                {
                    MessageBoxMidle.Show(this, "导入文件不是*.bin文件！", "提示");
                    pathtextBox.Clear();
                    return;
                }
                #region 作废
                    //OpenFileDialog ofd = new OpenFileDialog();
                    //if (Path.GetExtension(pathtextBox.Text) != ".bin")
                    //{
                    //    MessageBoxMidle.Show(this, "导入文件不是*.bin文件！", "提示");
                    //    pathtextBox.Clear();
                    //    return;
                    //}
                    //FileStream fs = new FileStream(pathtextBox.Text, FileMode.Open);
                    //byte[] arrfileSend = new byte[1024 * 1024 * 8];         //创建8M的缓存
                    //int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                    //byte[] arrfile = new byte[length];
                    //fs.Close();                                              //关闭文件流，释放文件
                    //Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);    // 将arrfileSend复制到arrfile之中
                    //byte[] tempdata = new byte[length];
                    //Buffer.BlockCopy(arrfile, 0, tempdata, 0, length);      // 将arrfile复制到tempdata之中
                    //int index = tempdata.Length - 65536 - 1;                //tempdata.Length个8bit的数据

                    ////=================文件处理代码部分===========================================
                    //for (; index > 0;)
                    //{
                    //    if (tempdata[index] != 0)
                    //    {
                    //        index = index + 128;
                    //        //toolStripStatusLabel1.Text = 
                    //        mod_Ereaseorde[14] = (byte)(mod_Ereaseorde[14] + (byte)System.Math.Ceiling((double)index / (double)65536));
                    //        flashnum = (int)System.Math.Ceiling((double)index / (double)65536);
                    //        //for (int ad = 0; ad <= index; ad++)
                    //        //{
                    //        //    Console.WriteLine($"filedata = {tempdata[ad]}.");
                    //        //    Invoke(myRcvMsg, StartsCRC.byteToHexStr(tempdata[ad]));
                    //        //}
                    //        break;
                    //    }
                    //    index = index - 1;
                    //}
                    ////=================文件处理代码部分===========================================
                    //filedata = new byte[index];
                    //Buffer.BlockCopy(tempdata, 0, filedata, 0, index);   // 将arrfileSend复制到arrfile之中
                    //Console.WriteLine($"filedata = {filedata.Length}.");
                    //Console.WriteLine($"index = {index}.");
                    ////for (int ad = 0; ad < index; ad++)
                    ////{
                    ////    //Console.WriteLine($"filedata = {filedata[ad]}.");
                    ////    Invoke(myRcvMsg, StartsCRC.byteToHexStr(filedata[ad]));
                    ////}
                    ////Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);   // 将arrfileSend复制到arrfile之中
                    ////Console.WriteLine($"filedata = {filedata.Length}.");
                    //step_i = 0;                                              //初始化
                    //duty = 0;                                                //初始化
                    //pagenum = filedata.Length / 128;                                  //初始化  以128个字节为一个数据包发送数据
                    //remaind = filedata.Length % 128;                                  //初始化

                    //cjremaind = remaind;
                    //Console.WriteLine($"remaind = {remaind}.");
                    //Console.WriteLine($"pagenum = {pagenum}.");
                    //loadprogressBar.Maximum = pagenum - 1;                   //进度条的最大值初始化
                    //toolStripStatusLabel1.Text = "擦除扇区数：" + (System.Math.Ceiling((double)index / (double)65536)).ToString() + "  " + "文件已导入，请确认！";

                    #endregion
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开文件失败！", "错误");
                toolStripStatusLabel1.Text = "打开文件失败，请检查";
            }
        }
        #endregion

        #region 应答式发送消息
        private void ACK_Sendfunction()
        {
            while (Connect)
            {
                Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                {
                    try
                    {
                        if (System_RS)
                        {
                            if (step_i < pagenum)
                            {
                                Buffer.BlockCopy(filedata, (step_i * 128), filedata_temp, 0, 128);
                                Console.WriteLine($"filedata = {filedata.Length}.");
                                Net_Work_Send(filedata_temp);              //发送128个字节数据
                                loadprogressBar.Value = step_i;
                                duty = (((double)step_i / (double)pagenum) * 100);
                                step_i = step_i + 1;
                                Console.WriteLine($"step_i = {step_i}.");
                                toolStripStatusLabel1.Text = "程序下载中:" + duty.ToString("0.00") + "%";
                            }
                            else if (cjremaind > 0)
                            {
                                //不够128，就填充0X1A一直填充满128个字节
                                Buffer.BlockCopy(filedata, (pagenum * 128), filedata_temp, 0, remaind);
                                for (; cjremaind < 128; cjremaind++)
                                {
                                    filedata_temp[cjremaind] = 0x00;
                                }
                                Net_Work_Send(filedata_temp);              //发送剩余的数据
                                duty = (((double)(step_i) / (double)pagenum) * 100);
                                toolStripStatusLabel1.Text = "程序下载中:" + duty.ToString("0.00") + "%";
                                loadprogressBar.Value = 0;
                                Connect = false;
                                //烧录完成处理函数
                                Action_End_Function();
                            }
                            else
                            {
                                loadprogressBar.Value = 0;
                                Connect = false;

                                //烧录完成处理函数
                                Action_End_Function();
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        enable_function();
                        MessageBoxMidle.Show(this, "详情：" + err.ToString(), "提示");
                    }
                })));
            }
        }
        #endregion

        #region 烧录完成处理函数
        private void Action_End_Function()
        {
            Step_Fiv = false;
            Step_Six = true;

            timer5.Stop();
            timer1.Start();
        }
        #endregion

        #region 点击发送事件
        private void downbutton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pathtextBox.Text))
            {
                MessageBoxMidle.Show(this, "请选择您要下载的Bin文件！", "发送文件提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                toolStripStatusLabel1.Text = "请选择需要下载的BIN文件！";
                return;
            }
            //保存IP
            Commentclass.CommentIP = ipaddres.Text;
            RegistryKeyLi.WriteRegistryKey(Commentclass.CommentIP, Commentclass.CommentPublicResgistryKeyPath, "CommentIP");

            disable_function();           
            databox.Show();
            databox.Clear();
            toolStripStatusLabel1.Text = "连接中...";
            load_file_onthepath();
        }
        #endregion

        #region 点击函数
        public void Click_Start()
        {
            if (!First_Run)
            {
                Systemtemp_int();  //如果不是第一次则要对变量初始化
                timer1.Start();      //判断DSP是否进入待烧录的模式
            }
            else
            {
                timer1.Start();     //判断DSP是否进入待烧录的模式
            }

            
           
        }
        #endregion

        #region 变量初始化函数
        public void Systemtemp_int()
        {
            step_i = 0;
            duty = 0;
            cjremaind = remaind;
            Connect = true;
            System_RS = true;
            Step_One = true;                                //第一步的状态
            Step_Tow = false;                               //第二步的状态
            Step_Thr = false;                               //第三步的状态
            Step_Fou = false;                               //第四步的状态
            Step_Fiv = false;                               //第五步的状态
            Step_Six = false;                               //第六步的状态
            ErrTryCount = 0;
            modtcp_alive.SWBS = 0x0000;
            modtcp_alive.REGITE = 0x0000;
            modtcp_alive.STARADDR = 0x0000;
            Ulockflg = false;
            Erasflag = false;
            step_send = false;
            ssSeconds = 0;
            loadtime = 0;
            pingack_flag = false;
            breakflag = false;
            fivesflag = false;
            fivecount = 0x00;
        }
        #endregion

        #region 以太网发送函数（无CRC校验）
        private void Net_Work_Send_Ncrc(byte[] data)
        {
            try  //星辰CRC校验
            {
                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                {
                    int len = 15;
                    byte[] Net_Sen_Temp = new byte[len];
                    Buffer.BlockCopy(modtcp_alive.BMCU_ORDE_MOD(data), 0, Net_Sen_Temp, 0, len);

                    if (powerdisplay)
                    {
                    Invoke(myRcvMsg, "发送：" + StartsCRC.zftring(Net_Sen_Temp));
                    }
                    sockClient.Send(Net_Sen_Temp);          //以太网发送数据
                    SneCapre = new byte[Net_Sen_Temp.Length];
                    Buffer.BlockCopy(Net_Sen_Temp, 0, SneCapre, 0, Net_Sen_Temp.Length);
                })));
            }
            catch
            {
                timer1.Stop();
                enable_function();
                MessageBoxMidle.Show(this, "数据发送错误！", "错误");
            }

        }
        #endregion

        #region 以太网发送单字节命令函数（无CRC校验）
        private void Net_Send_Single_Ncrc(byte data)
        {
            try  //星辰CRC校验
            {
                int len = 0x09;
                byte[] Net_Sen_Temp = new byte[len];
                Buffer.BlockCopy(modtcp_alive.DSP_ORDE_MOD(data), 0, Net_Sen_Temp, 0, len);
                sockClient.Send(Net_Sen_Temp);          //以太网发送数据
            }
            catch
            {
                enable_function();
                MessageBoxMidle.Show(this, "数据发送错误！", "错误");
            }

        }
        #endregion

        #region 以太网发送函数（CRC校验）
        private void Net_Work_Send(byte[] data)
        {
            try  //星辰CRC校验
            {
                if (System_RS)
                {
                    System_RS = false;                                                      //等待校验成功
                    int length = data.Length;                           //需要发送数据的长度
                    UInt16 CRC_Value = StartsCRC.CRC16(data, length);
                    CrcCheck[0] = Convert.ToByte(CRC_Value / 256);      //将CRC校验值写入发送的数据
                    CrcCheck[1] = Convert.ToByte(CRC_Value % 256);

                    //======在这里加上校验位=================================================
                    byte[] alldata = new byte[data.Length + CrcCheck.Length];
                    Buffer.BlockCopy(data, 0, alldata, 0, data.Length);                     //将data放到alldata之中
                    Buffer.BlockCopy(CrcCheck, 0, alldata, data.Length, CrcCheck.Length);   //将CrcCheck放到alldata之中
                    SneCapre = new byte[alldata.Length];
                    Buffer.BlockCopy(alldata, 0, SneCapre, 0, alldata.Length);              // 将data复制到SneCapre之中
                    Console.WriteLine($"alldata = {alldata.Length}");
                    //========================================================================
                    int len = 143;
                    byte[] Net_Sen_Temp = new byte[len];
                    Buffer.BlockCopy(modtcp_alive.DSP_MODTCP_ARMM(alldata), 0, Net_Sen_Temp, 0, len);
                    //Console.WriteLine($"modtcp_alive = {modtcp_alive.DSP_MODTCP_ARMM(alldata).Length}.");
                    if (powerdisplay)
                    {                        
                         Invoke(myRcvMsg, "发送：" + StartsCRC.zftring(Net_Sen_Temp));                        
                    }
                    timer5.Start();
                    sockClient.Send(Net_Sen_Temp);                 //以太网发送数据
                }
            }
            catch
            {
                timer1.Stop();
                enable_function();
                MessageBoxMidle.Show(this, "数据发送错误！", "错误");
            }

        }
        #endregion

        #region 实现升级、跳转、解锁、擦除
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {          
                if (Step_One)
                {
                    LinkCount = LinkCount + 1;
                    Net_Work_Send_Ncrc(mod_Upgradeorde);

                    if (!powerdisplay)
                        Invoke(myRcvMsg, Commentclass.DSPStateMessageList[0] + DateTime.Now.ToString("hh:mm:ss") + "\r\n");

                    loadtimecount = true;
                    step_send = true;
                    if (LinkCount == 4)
                    {
                        LinkCount = 0;
                        timer1.Stop();
                        enable_function();
                        MessageBoxMidle.Show(this, "无响应！\r\n请重试！", "提示");
                    }
                }
                else if (Step_Tow)
                {
                    #region 
                    if (!breakflag)
                    {
                        sockClient.Send(mod_Jmpbtolorde);          //以太网发送数据
                        try
                        {
                            sockClient.Shutdown(SocketShutdown.Both);
                        }
                        catch
                        {
                        }
                        try
                        {
                            sockClient.Close();
                        }
                        catch
                        {
                        }
                        if (!powerdisplay)
                            Invoke(myRcvMsg, Commentclass.DSPStateMessageList[1] + DateTime.Now.ToString("hh:mm:ss") + "\r\n");
                        toolStripStatusLabel1.Text = "等待跳转...";

                        SocketOn = false;

                        Console.WriteLine($"已经断开旧的连接");
                        thrClient.Abort(); //关闭线程
                        Console.WriteLine($"已经关闭接收线程");
                        timer7.Start();
                        breakflag = true;
                    }
                    #endregion
                    if (breakflag && fivesflag)
                    {
                        timer7.Stop();
                        RecMsg("以太网重连中----");
                        for (int i = 0; i < 6; i++)
                        {
                            Thread.Sleep(300);
                            Ping pingSend = new Ping();
                            PingReply reply = pingSend.Send(ipaddres.Text, 50);    //200

                            if (reply.Status == IPStatus.Success)
                            {
                                pingSend.Dispose();
                                Console.WriteLine($"ping成功了");
                                IPAddress address = IPAddress.Parse(this.ipaddres.Text.Trim());                     //连接IP
                                IPEndPoint Ipe = new IPEndPoint(address, 502);                                      //连接端口
                                sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);       //将对象实例化

                                sockClient.Connect(Ipe);
                                //downbutton.Enabled = true;
                                thrClient = new Thread(ReceiceMsg);
                                thrClient.IsBackground = true;
                                thrClient.Start();

                                SocketOn = true;
                                Step_Tow = false;
                                Step_Thr = true;
                                timer1.Start();
                                pingack_flag = true;
                                Console.WriteLine($"i = {i}.");
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"ping失败了");
                                Console.WriteLine($"i = {i}.");
                                pingSend.Dispose();
                                pingack_flag = false;
                               
                                // MessageBoxMidle.Show(this, "跳转网络重连无响应！\r\n请重试！", "提示");

                            }
                        }
                        if (!pingack_flag)
                        {
                            RecMsg("ping失败了");
                            Console.WriteLine("关闭Timer1");
                            timer1.Stop();
                            enable_function();
                            MessageBoxMidle.Show(this, "跳转后重连网络无响应！\r\n请重试！", "提示");
                            //GitHub提交全新
                            return;
                        }
                    }
                }
                else if (Step_Thr)
                {
                    if (LinkCount == 0)
                    {
                        LinkCount = LinkCount + 1;
                        Net_Work_Send_Ncrc(mod_Unlockorde);
                        if (!powerdisplay)
                            Invoke(myRcvMsg, Commentclass.DSPStateMessageList[2] + DateTime.Now.ToString("hh:mm:ss") + "\r\n");
                        toolStripStatusLabel1.Text = "解锁中...";
                    }
                    else if (Ulockflg)
                    {
                        if (LinkCount < 3)
                        {
                            LinkCount = LinkCount + 1;
                            Net_Work_Send_Ncrc(mod_Unlockorde);
                            Ulockflg = false;
                        }
                        else
                        {
                            LinkCount = 0;
                            timer1.Stop();
                           
                            DialogResult messdr = MessageBoxMidle.Show(this, "解锁失败！是否重试解锁？", "提示", MessageBoxButtons.YesNo);
                            if (messdr == DialogResult.Yes)
                            {
                                Console.WriteLine($"重试解锁");
                                timer1.Start();
                            }
                            else
                            {
                                enable_function();
                                Close_SocketTr();               //关socket线程
                                toolStripStatusLabel1.Text = "解锁失败...";
                            }
                        }
                    }
                    else
                    {
                        unlocktime++;
                        if (unlocktime == 60)
                        {
                            LinkCount = 0;
                            unlocktime = 0;
                            timer1.Stop();
                            DialogResult messdr = MessageBoxMidle.Show(this, "解锁失败！是否重试解锁？", "提示", MessageBoxButtons.YesNo);
                            if (messdr == DialogResult.Yes)
                            {
                                Console.WriteLine($"重试解锁");
                                timer1.Start();
                            }
                            else
                            {
                                enable_function();
                                toolStripStatusLabel1.Text = "解锁失败...";
                                Close_SocketTr();               //关socket线程
                            }
                        }
                    }
                }
                else if (Step_Fou)
                {

                    if (LinkCount == 0)
                    {
                        LinkCount = LinkCount + 1;
                        Net_Work_Send_Ncrc(mod_Ereaseorde);
                        if (!powerdisplay)
                            Invoke(myRcvMsg, Commentclass.DSPStateMessageList[3] + DateTime.Now.ToString("hh:mm:ss") + "\r\n");
                        toolStripStatusLabel1.Text = "擦除中...";
                        display_run();             //关于擦除的处理
                    }
                    else if (Erasflag)
                    {
                        if (LinkCount < 2)
                        {
                            LinkCount = LinkCount + 1;
                            Net_Work_Send_Ncrc(mod_Ereaseorde);
                            Erasflag = false;                      //要在数据接收那边进行处理
                        }
                        else
                        {
                            LinkCount = 0;           //从解锁开始
                            timer1.Stop();
                            DialogResult messdr = MessageBoxMidle.Show(this, "擦除失败！是否重试擦除？", "提示", MessageBoxButtons.YesNo);
                            if (messdr == DialogResult.Yes)
                            {
                                Step_Thr = true;
                                Step_Fou = false;
                                Console.WriteLine($"重试擦除");
                                timer1.Start();
                            }
                            else
                            {
                                enable_function();
                                toolStripStatusLabel1.Text = "擦除失败！";
                                Close_SocketTr();               //关socket线程
                            }
                        }
                    }
                    else
                    {
                        ssSeconds++;  //加一次500毫秒
                        Barcount(ssSeconds);            //委托
                        if (ssSeconds == (78 * flashnum + (flashnum - 1) * 2))
                        {
                            LinkCount = 0;
                            ssSeconds = 0;
                            Barcount(ssSeconds);            //委托
                            timer1.Stop();
                            DialogResult messdr = MessageBoxMidle.Show(this, "擦除超时！是否重试擦除？", "提示", MessageBoxButtons.YesNo);
                            if (messdr == DialogResult.Yes)
                            {
                                Step_Thr = true;
                                Step_Fou = false;
                                Console.WriteLine($"重试擦除");
                                timer1.Start();
                            }
                            else
                            {
                                enable_function();
                                toolStripStatusLabel1.Text = "擦除失败！";
                                loadprogressBar.Value = 0;
                                Close_SocketTr();               //关socket线程
                            }
                        }
                    }
                }
                else if (Step_Fiv)
                {
                    timer1.Stop();
                    Barcount(0);
                    Open_Threading();//开启线程
                }
                else if (Step_Six)
                {
                    timer1.Stop();
                    Net_Work_Send_Ncrc(mod_Jmpapbegin);
                    if (!powerdisplay)
                        Invoke(myRcvMsg,Commentclass.DSPStateMessageList[5] + DateTime.Now.ToString("hh:mm:ss") + "\r\n");
                    Close_SocketTr();
                    loadtimecount = false;
                    databox.AppendText(Commentclass.DSPStateMessageList[6] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");               
                    menuStrip1.Enabled = true;
                    toolStripStatusLabel1.Text = "程序升级完成!";
                    

                    if (IsCir_lode == true)
                    {
                        Dsp_lode_successfultimes += 1;
                        this.textBox2.Text = Dsp_lode_successfultimes.ToString();
                        Thread.Sleep(500);
                        First_Run = false;
                        //Click_Start();
                        timer4.Start();

                        return;
                    }

                    enable_function();
                }
                
            }
            catch (Exception err)
            {
                enable_function();
                MessageBoxMidle.Show(err.ToString());   //显示异常状态
                SocketOn = false;                       //异常处理
            }
        }
        #endregion

        #region 共用的关闭SOCKET线程的函数
        private void Close_SocketTr()
        {
            Connect = false;
            Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
            {
                thrClient.Abort(); //关闭线程
                sockClient.Shutdown(SocketShutdown.Both);
                sockClient.Close();
                SocketOn = false;
            })));
        }
        #endregion

        #region 共用的开启线程的函数
        private void Open_Threading()
        {
            AckSend_Function = new Thread(ACK_Sendfunction);
            AckSend_Function.IsBackground = true;
            AckSend_Function.Start();
            Console.WriteLine("线程开启！");
            toolStripStatusLabel1.Text = "传输线程开启.......";
            menuStrip1.Enabled = false;
        }
        #endregion

        #region 错误重发函数
        private void timer2_Tick(object sender, EventArgs e)
        {
            databox.AppendText("ERR：");
            timer2.Stop();
            System_RS = true;
            Net_Work_Send(filedata_temp);              //发送128个字节数据
        }
        #endregion

        #region 委托
        private void RecMsg(string str)
        {
          
             this.databox.AppendText(str + "\r\n");
            
            //this.databox.AppendText(str);
        }
        #endregion

        #region 以太网连接函数
        private void timer4_Tick(object sender, EventArgs e)
        {
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(ipaddres.Text, 100);

                netconnect = netconnect + 1;
                if (reply.Status == IPStatus.Success)
                {
                    pingSend.Dispose();
                    timer4.Stop();
                    netconnect = 0;
                    Comment_Net_Link();
                }
                else
                {  
                    pingSend.Dispose();
                    if (netconnect ==15)
                    {
                        timer4.Stop();
                        netconnect = 0;
                        enable_function();
                        databox.AppendText("连接失败，请检查网线与硬件是否正常。\r\n");
                    }
                }
                if (SocketOn)
                {
                    if (timer4.Enabled)
                    {
                        //timer4.Stop();
                       //ownbutton.Enabled = true;
                    }
                    if (this.IsCir_lode == true)
                    {
                        this.Dos_lode_sumtimes += 1;
                        this.textBox3.Text = Dos_lode_sumtimes.ToString();
                    }
                    Click_Start();
                    First_Run = false; //上位机不是首次打开
                }
                else
                {
                    return;
                }
            }

            catch { }
        }
        #endregion

        #region 杜绝无响应
        private void timer5_Tick(object sender, EventArgs e)
        {
            if (!System_RS)
            {
                timer5.Stop();
                Close_SocketTr();               //关socket线程
                MessageBoxMidle.Show(this, "发送数据无应答！", "提示！");
                enable_function();
                toolStripStatusLabel1.Text = "无响应！";
                loadprogressBar.Value = 0;
            }
        }
        #endregion

        #region 窗体加载函数
        private void Form3_Load(object sender, EventArgs e)
        {
            if (Commentclass.fmjump == 0x01)    //从窗口1切换过来
            {
                this.Location = new Point(Form1.windowX, Form1.windowY);
            }
            else if (Commentclass.fmjump == 0x02)   //从窗口2切换过来
            {
                this.Location = new Point(Form2.fmwindowX, Form2.fmwindowY);
            }
            if (Commentclass.fmjump == 0x07)    
            {
                this.Location = new Point(Form7.windowX, Form7.windowY);
            }

            if (Commentclass.fmjump == 0x04)
            {
                Commentclass.WinDey = true;             
            }
            
            if (Commentclass.WinDey)
            {
                //menuStrip1.Items.Remove(主控板刷机);
                menuStrip1.Items[2].Visible=false;
                this.Icon = IconSelect.GetFileIcon(Application.StartupPath + "\\icon\\wendey.ico");
                menuStrip1.Items[0].Text = "DSP升级";
                menuStrip1.Items[1].Text = "充电器ARM升级";
                menuStrip1.Items[3].Text = "主板板ARM升级";
                this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
                toolStripStatusLabel1.Text = "DSP升级";
                menuStrip1.Items[0].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[1].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[2].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[3].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                string messageini = ReadINIFiles.ReadIniData("WarnMessage", "DspBordMessage", "None", IniFilesPath);
                string[] nametake3 = messageini.Split("\\r\\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                databox.Clear();
                foreach (string str in nametake3)
                {
                    databox.AppendText(str + "\r\n");
                }
                //ipaddres.Enabled = false;
                //ipaddres.Hide();
                //label1.Hide();
                //ipaddres.Text ="已自动识别";
                if (databox.TextLength >= 5)
                {
                    databox.Select(15, 8);
                    databox.SelectionColor = Color.Red;
                }

                int X = ipaddres.Location.X;
                int y = ipaddres.Location.Y;
                //textBox1.Location = new Point(X, y);
                //textBox1.Show();
                //label1.Text = "网口：";
                //databox.Text = nametake3[0] + "\r\n" + nametake3[1] + "\r\n";
                //menuStrip1.Items.Remove(以太网测试ToolStripMenuItem);
                //DSP刷机.Enabled = false;
                以太网测试ToolStripMenuItem.Dispose();
            }
            else
            {
                this.Icon = IconSelect.GetFileIcon(Application.StartupPath + "\\icon\\wendey.ico");
                this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
                toolStripStatusLabel1.Text = "DSP升级";
                menuStrip1.Items[1].Text = "充ARM升级";
                menuStrip1.Items[3].Text = "主ARM升级(Eth)";
                menuStrip1.Items[0].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[1].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[2].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[3].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                //ipaddres.Enabled = false;
                //ipaddres.Hide();
                int X = ipaddres.Location.X;
                int y = ipaddres.Location.Y;
                //textBox1.Location = new Point(X, y);
                //textBox1.Show();                
                //label1.Text = "网口：";
            }
            textBox1.Hide();
            toolStripStatusLabel3.Text = Commentclass.Version;

            int filebutton_x = filebutton.Location.X;
            int filebutton_y = filebutton.Location.Y;
            //浏览
            filebutton.Location = new Point(filebutton_x, filebutton_y+2);
          
            this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);

            #region 判断是否存在INI文件，如果存在就显示           
            //此方法也可通过：str = System.AppDomain.CurrentDomain.BaseDirectory + @"ConnectString.ini";
            strOne = System.IO.Path.GetFileNameWithoutExtension(str);
            if (File.Exists(str))
            {
                string pathstr = ContentReader(strOne, "DSPData_Source", "");
                Commentclass.DspTryTimer = Convert.ToUInt16( ContentReader(strOne, "DSPTryConnectTimer", ""));
                Console.WriteLine($"  Commentclass.DspTryTimer = {  Commentclass.DspTryTimer}.");
                int i = pathstr.LastIndexOf("\\");//获取字符串最后一个斜杠的位置
                string path = pathstr.Substring(0, i);//取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                if (Directory.Exists(path))
                {
                    pathtextBox.Text = pathstr;
                }
                else
                {
                    pathtextBox.Text = "";
                }
            }
            else
            {
                Commentclass.DspTryTimer = 5;
            }
            #endregion

            string codesize = ReadINIFiles.ReadIniData("DspCodeSizeLimti", "NormalSize", "None", IniFilesPath);          
            string[] para = codesize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);        
            Commentclass.DspNormalSize = Convert.ToUInt64(para[0]) * Convert.ToUInt64(para[1]);
            Console.WriteLine($"DspNormalSize = {Commentclass.DspNormalSize}.");
            //timer6.Start();
            Console.WriteLine($"  heigth = {databox.Height}.");
            Console.WriteLine($"  weigth = {databox.Width}.");
            Console.WriteLine($"  x = {databox.Location.X}.");
            Console.WriteLine($"   y = {databox.Location.Y}.");

            //读取IP
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, "CommentIP", out Commentclass.CommentIP);
            Console.WriteLine($"读出来的路径 = {Commentclass.CommentIP}.");
            ipaddres.Text = Commentclass.CommentIP;

            databox.Location = new Point(9, 34);
            databox.Size = new Size(386, 140);

        }
        #endregion

        #region 窗体切换
        private void Form3_Activated(object sender, EventArgs e)
        {
            ipaddres.Text = Commentclass.CommentIP;
        }
        #endregion

        #region 充电板刷机窗口 
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x03;
            int winx = fm3windowX;
            int winy = fm3windowY;
            Commentclass.fm2.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm2.Show();
        }
        #endregion

        #region 主控板刷机窗口
        private void 主控板刷机_Click(object sender, EventArgs e)
        {
            this.Hide();
            int winx = fm3windowX;
            int winy = fm3windowY;
            Commentclass.fmjump = 0x03;
            Commentclass.fm1.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm1.Show();
        }
        #endregion

        #region 主控板刷机窗口(Eth)
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x03;  //从窗口1条过去
            int winx = fm3windowX;
            int winy = fm3windowY;
            Commentclass.fm7.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm7.Show();
        }
        #endregion

        #region 窗体位置获取
        private void Form3_Move(object sender, EventArgs e)
        {
            fm3windowX = this.Location.X;
            fm3windowY = this.Location.Y;
            //this.toolStripStatusLabel2.Text = fm3windowX.ToString() + "," + fm3windowY.ToString();
        }
        #endregion

        #region 窗体关闭函数
        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (downbutton.Enabled == false)
            {
                //DialogResult Result = MessageBoxMidle.Show(this, "是否退出？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //if (Result == DialogResult.OK)
                //{
                //    Application.Exit();
                //}
                //else
                //{
                    e.Cancel = true;
                //    return;
                //}
               // MessageBoxMidle.Show("","Yes",)
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        #region 时间显示函数
        private void timer6_Tick(object sender, EventArgs e)
        {
            canset = true;
            timer6.Stop();
        }
        #endregion

        private void timer3_Tick(object sender, EventArgs e)
        {

        }

        #region 回车按键函数
        private void ipaddres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (ipaddres.Text == "1024")
                    {
                        powerdisplay = true;
                    }
                    else if (ipaddres.Text == "2048")
                    {
                        powerdisplay = false;
                    }
                    else if (ipaddres.Text == "4096")
                    {
                        databox.Clear();
                    }
                    //ipaddres.Text = "192.168.1.15";
                    //保存IP
                    Commentclass.CommentIP = ipaddres.Text;
                    RegistryKeyLi.WriteRegistryKey(Commentclass.CommentIP, Commentclass.CommentPublicResgistryKeyPath, "CommentIP");
                }
                catch (Exception err)
                { MessageBox.Show("详情：" + err.ToString(), "警告"); }
            }
        }
        #endregion

        #region 强制退出
        private void 强制退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        private void disable_function()
        {
            menuStrip1.Enabled = false;
            downbutton.Enabled = false;
            filebutton.Enabled = false;
        }

        private void enable_function()
        {
            menuStrip1.Enabled = true;
            downbutton.Enabled = true;
            filebutton.Enabled = true;
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            if (breakflag)
            {
                fivecount++;
                if (fivecount == Commentclass.DspTryTimer)
                {
                    fivesflag = true;
                }
            }
        }
        //调出以太网测试窗口
        private void 以太网测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form5 fm5 = new Form5();
                fm5.Show();
            }
            catch
            { 
            
            }
        }

        private void toolStripStatusLabel5_Click(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (Commentclass.Lognin.IsDisposed)
                {
                    Login Lognin = new Login();
                    Lognin.Show();
                }
                else
                {
                    Commentclass.Lognin.Show();
                }
            }
            else
            {
                //System.Diagnostics.Process.Start("www.stars.com.cn");
            }
        }

        #region 清除databox显示或保存databox数据
        private void ContextStripMenuItem_Click(object sender, EventArgs e)
        {
            UInt16 tag = Convert.ToUInt16(((ToolStripMenuItem)sender).Tag.ToString());
            switch (tag)
            {
                case 0x00://清除
                    this.databox.Text = string.Empty;
                    break;
                case 0x01://导出文本
                    if (Commentclass.CommentTranspDataEnd)
                    {
                        string BackMessage = ControlFile.SaveTxtFilesFromTextBox(databox);
                        //if(!Commentclass.WinDey)
                        //{
                        //    BackMessage = ControlFile.SaveTxtFilesFromTextBox(textBox1);
                        //}    

                        if (BackMessage == "ok")
                        {
                            MessageBoxMidle.Show(this, "导出成功。", "提示");
                        }
                        else if (BackMessage == "qt")
                        {; }
                        else
                        {
                            MessageBoxMidle.Show(this, BackMessage, "提示");
                        }
                    }
                    else
                    {
                        MessageBoxMidle.Show(this, "数据更新中不能导出文件。", "错误");
                    }
                    break;
                case 0x02://关闭窗口

                    break;

            }
        }
        #endregion

        bool IsCir_lode = false;
        int Dsp_lode_successfultimes = 0;
        int Dos_lode_sumtimes = 0;
        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            if (IsCir_lode == false)
            {
                ((ToolStripStatusLabel)sender).BackColor = Color.Green;
                IsCir_lode = true;
            }
            else
            {
                ((ToolStripStatusLabel)sender).BackColor = Color.Yellow;
                IsCir_lode = false;
            }
            
        }
    }


}


