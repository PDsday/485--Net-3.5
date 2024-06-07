using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using View;

namespace _485刷机_Net_3._5
{
    

    delegate void RecMsgDelegatefm1(string str);
    public partial class Form1 : Form
    {
        RecMsgDelegatefm1 myRcvMsgfm1;            //委托

        #region 变量定义区
        String serialPortName;                               //串口名
        public static byte[] filedata;                       //创建保存读取的文件数据的缓存空间
        public static byte[] SneCapre;                       //用于将发送的数据和接收的数据进行保存
        public static byte[] Cutoutfiledata;                 //用于保存截取回来的APP代码片段

        string backack = "ok";                                 //反馈信息
        int step_i = 0;                                      //发送的次数 
        double duty = 0;                                     //发送文件的进度值
        int pagenum = 0;                                     //计算需要发送多少次
        int remaind = 0;                                     //计算是否发送次数不为整数
        int cjremaind = 0;                                   //承接remaind
        public byte[] filedata_temp = new byte[128];         //承接BIN文件被按照128个字节分开后每次的数据 
        static int i = 0;
        int loadtime = 0;
        public static byte[] buffer;
        static int bufferAdr;                                   // 打开文件hex to bin缓存指针，用于计算bin的长度
        static bool CutoutActionFlag = true;                    //如果是.hex文件不需要进行文件截取的操作
        string commentpath = System.Windows.Forms.Application.StartupPath + ".\\ZKBIN.\\ZKBIN.bin";
        #endregion

        #region 窗体位置
        static public int windowX = 0;
        static public int windowY = 0;
        #endregion

        #region 标志位定义区
        bool Connect = true;                                 //应答式接收的运行标志位
        bool System_RS = true;                               //判断系统能否继续进行 
        bool  Step_One = true;                                //第一步的状态
        bool Step_Tow = false;                               //第二步的状态
        bool orde = false;
        bool First_Run = true;                               //判断是否第一次点击下载
        int LinkCount = 0;                                   //连接无线重连次数
        bool powerdisplay = false;                           //隐藏某些显示
        bool loadtimecount = false;                          //计时标志位
        bool yuchuliflag = false;
        bool cansend = true;
        bool ClearFlash = false;                             //一键擦除Flash
        #endregion

        #region 指令定义区
        public byte[] Bohead = new byte[3] { 0x01, 0x01, 0XFE };                                        //帧头/报数据/反报数据
        public byte[] Readyoder = new byte[7]  { 0X01, 0XC0, 0X07, 0XAA, 0X55, 0X64, 0X80 };             //让单片机进入就绪状态 ，第一次返回相同的指令内容作为校验
        public byte[] Readyoder1 = new byte[7] { 0X01, 0XC0, 0X07, 0X55, 0XAA, 0X79, 0X8F };            //让单片机进入就绪状态 ，第一次返回相同的指令内容作为校验
        public byte[] EraseFlash = new byte[7] { 0X03, 0XC0, 0X07, 0X40, 0X07, 0XA5, 0X4D };            //单片机擦除Flash指令
        public byte[] Overoder = new byte[1] { 0x04 };                                                  //告诉单片机刷机文件的发送完毕,让其跳转
        public byte[] CrcCheck = new byte[2];                                                           //CRC校验位的高位和低位
        public byte OkFlag = 0X06;                                                                      //接收正确返回指令
        public byte ErrFlag = 0X15;                                                                     //接收错误返回的指令；错误重发三次
        int Resive_step = 0;
        byte ErrTryCount = 0;                                                                           //错误重发，最多三次
        #endregion

        #region 线程定义
        Thread AckSend_Function = null;                      //应答式接发线程
        #endregion

        #region INI变量声明区
        public string str = Application.StartupPath + "\\ConnectString.ini";//该变量保存INI文件所在的具体物理位置
        public string IniFilesPath = Application.StartupPath + "\\ConnectString.ini";//该变量保存INI文件所在的具体物理位置
        public string strOne = "";
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
        
        public Form1()
        {
            InitializeComponent();
            myRcvMsgfm1 = RecMsg;
            //databox.Hide();
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.filebutton, "支持文件*.bin|*.hex拖放");
            this.databox.ContextMenuStrip = this.contextMenuStrip2;



            #region 判断是否存在INI文件，如果存在就显示        
            //此方法也可通过：str = System.AppDomain.CurrentDomain.BaseDirectory + @"ConnectString.ini";
            strOne = System.IO.Path.GetFileNameWithoutExtension(str);
            if (File.Exists(str))
            {
                string pathstr = ContentReader(strOne, "ZKARMData_Source", "");
                int i = pathstr.LastIndexOf("\\");//获取字符串最后一个斜杠的位置
                string path = pathstr.Substring(0, i);//取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                if (Directory.Exists(path))
                {
                   // pathtextBox.Text = pathstr;
                    Console.WriteLine($"ini文件存在");

                }
                else
                {
                    //MessageBoxMidle.Show(".ini配置文件导入错误，禁止进行刷机操作。\r\n" + "请联系软件人员。", "严重错误！");
                  //  pathtextBox.Text = "";
                    //Console.WriteLine($"ini文件不存在");
                }
            }
            #endregion
        }

        #region 窗体加载函数
        private void Form1_Load(object sender, EventArgs e)
        {
            StackTrace st = new StackTrace(new StackFrame(true));
            DavidDebug.showMsg("切换ARM485下载界面",st);
            try
            {
                if (Commentclass.fmjump == 0x02)
                {
                    this.Location = new Point(Form2.fmwindowX, Form2.fmwindowY);
                }
                else if (Commentclass.fmjump == 0x03)
                {
                    this.Location = new Point(Form3.fm3windowX, Form3.fm3windowY);
                }
                if (Commentclass.fmjump == 0x07)    
                {
                    this.Location = new Point(Form7.windowX, Form7.windowY);
                }

                //界面设计
                this.Icon = IconSelect.GetFileIcon(Application.StartupPath + "\\icon\\wendey.ico");
                this.Size = new Size(410, 367);
                Refreshui();
                //端口lable
                label1.Location = new Point(Commentclass.PortlablePointdata.x, Commentclass.PortlablePointdata.y);
                label1.Size = new Size(Commentclass.PortlablePointdata.weith, Commentclass.PortlablePointdata.heigth);

                //端口combox
                serialcomboBox.Location = new Point(Commentclass.PortcomboxPointdata.x, Commentclass.PortcomboxPointdata.y);
                serialcomboBox.Size = new Size(Commentclass.PortcomboxPointdata.weith, Commentclass.PortcomboxPointdata.heigth);

                //波特率lable
                label2.Location = new Point(Commentclass.BoundlablePointdata.x, Commentclass.BoundlablePointdata.y);
                label2.Size = new Size(Commentclass.BoundlablePointdata.weith, Commentclass.BoundlablePointdata.heigth);

                //波特率combox
                boundcomboBox.Location = new Point(Commentclass.BoundcomboxPointdata.x, Commentclass.BoundcomboxPointdata.y);
                boundcomboBox.Size = new Size(Commentclass.BoundcomboxPointdata.weith, Commentclass.BoundcomboxPointdata.heigth);

                //文件lable
                label3.Location = new Point(Commentclass.FilelablePointdata.x, Commentclass.FilelablePointdata.y);
                label3.Size = new Size(Commentclass.FilelablePointdata.weith, Commentclass.FilelablePointdata.heigth);

                //文件textbox
                pathtextBox.Location = new Point(Commentclass.FiletextboxPointdata.x, Commentclass.FiletextboxPointdata.y);
                pathtextBox.Size = new Size(Commentclass.FiletextboxPointdata.weith, Commentclass.FiletextboxPointdata.heigth);

                //进度lable
                label4.Location = new Point(Commentclass.PrglablePointdata.x, Commentclass.PrglablePointdata.y);
                label4.Size = new Size(Commentclass.PrglablePointdata.weith, Commentclass.PrglablePointdata.heigth);

                //进度pragre
                loadprogressBar.Location = new Point(Commentclass.PrgformPointdata.x, Commentclass.PrgformPointdata.y);
                loadprogressBar.Size = new Size(Commentclass.PrgformPointdata.weith, Commentclass.PrgformPointdata.heigth);

                //下载
                loadbutton.Location = new Point(Commentclass.DownbtnPointdata.x, Commentclass.DownbtnPointdata.y);
                loadbutton.Size = new Size(Commentclass.DownbtnPointdata.weith, Commentclass.DownbtnPointdata.heigth);

                //浏览
                filebutton.Location = new Point(Commentclass.ViewbtnPointdata.x, Commentclass.ViewbtnPointdata.y);
                filebutton.Size = new Size(Commentclass.ViewbtnPointdata.weith, Commentclass.ViewbtnPointdata.heigth);

              
                toolStripStatusLabel3.Text = Commentclass.Version;
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();//获取电脑上可用的串口号
                serialcomboBox.Items.AddRange(ports);      //给端口号选择窗口添加数据
                serialcomboBox.SelectedIndex = serialcomboBox.Items.Count > 0 ? 0 : -1;  //如果里面有数据显示第零个


                this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
                //Console.WriteLine("David--"+IniFilesPath);


                //进行相关的信息录入
                //读取ini文件,获取主控板ARM的 BootloaderHeadFlag 、CodeStartAddr、RedirectionAddr
                //var ReadIniTempFlag0 = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordBootloaderHeadFlagINI", "None", IniFilesPath);
                var ReadIniTempFlag0 = Commentclass.SettingMessageList[0];
               // Console.WriteLine($"ZKReadIniTemp = {ReadIniTempFlag0}.");
                string[] nametake0 = ReadIniTempFlag0.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < 16; i++)
                {
                    Commentclass.MainbordBootloaderHeadFlag[i] = Convert.ToByte(nametake0[i], 16);
                //    Console.WriteLine($"ZKBootloaderHeadFlag = {Commentclass.MainbordBootloaderHeadFlag[i].ToString("x")}.");
                }

                //获取CodeStartAddr
                //var ReadIniTempStartAddr0 = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordCodeStartAddrINI", "None", IniFilesPath);
                var ReadIniTempStartAddr0 = Commentclass.SettingMessageList[1];
                Commentclass.MainbordCodeStartAddr = Convert.ToUInt64(ReadIniTempStartAddr0, 16);
              //  Console.WriteLine($"ZKCodeStartAddr = {"0X" + Commentclass.MainbordCodeStartAddr.ToString("x")}.");

                //获取RedirectionAddr
                //var ReadIniTempTionAddr0 = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordRedirectionAddrINI", "None", IniFilesPath);
                var ReadIniTempTionAddr0 = Commentclass.SettingMessageList[2];
                Commentclass.MainbordRedirectionAddr = Convert.ToUInt64(ReadIniTempTionAddr0, 16);
             //   Console.WriteLine($"ZKRedirectionAddr = {"0X" + Commentclass.MainbordRedirectionAddr.ToString("x")}.");
                Commentclass.MainbordIniRecordBootloaderSize = Commentclass.MainbordRedirectionAddr - Commentclass.MainbordCodeStartAddr;



                //是否开启0xff校验
                string enable = ReadINIFiles.ReadIniData("ENABLECHECK", "AdvancedVerification", "None", IniFilesPath);
                Console.WriteLine($"enable = {enable}.");
                if (enable == "false")
                {
                    Commentclass.ChexkActionEnable = false;
                }

                //读取刷机文件(主控板)大小限制参数
                string maximalsize = ReadINIFiles.ReadIniData("MainBordCodeSizeLimti", "MaximalSize", "None", IniFilesPath);
                string minimalsize = ReadINIFiles.ReadIniData("MainBordCodeSizeLimti", "MinimalSize", "None", IniFilesPath);
                string[] para = maximalsize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string[] para1 = minimalsize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                Commentclass.MainBordMaximalSize = Convert.ToUInt64(para[0]) * Convert.ToUInt64(para[1]);
                Commentclass.MainBordMinimalSize = Convert.ToUInt64(para1[0]) * Convert.ToUInt64(para1[1]);

                databox.Location = new Point(9, 34);
                databox.Size = new Size(386, 140);

              //  Console.WriteLine($"MainBordMaximalSize = {Commentclass.MainBordMaximalSize}.");
              //  Console.WriteLine($"MainBordMinimalSize = {Commentclass.MainBordMinimalSize }.");
                // Debug.WriteLine("lokokom");               //the likes printf      
                timer4.Start();
                //toolStripMenuItem1.BackColor = System.Drawing.Color.Chartreuse;
                //boundcomboBox.Enabled = true;
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, err.ToString(), "错误");
            }
        }


        private void David_Form1_Load(object sender, EventArgs e)
        {
            //fmjump重新规定 ：00          01           02              03             04           05 
            //            DSP升级    充ARM485     充电ARM(Eth)       主ARM485       主ARM(Eth)    DspBootload
            //             Form3        Form2        Form8（新）       From1           From7        外部exe
            //无处理
            Commentclass.MainBoardFileHexSizeMin = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainHexMin", "None", IniFilesPath)) * 1024;
            //主控板目标文件的最大值
            Commentclass.MainBoardFileHexSizeMax = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainHexMax", "None", IniFilesPath)) * 1024;
            //读取刷机文件(主控板)大小限制参数
            Commentclass.MainBoardFileBinSizeMin = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainBinMin", "None", IniFilesPath)) * 1024;
            //主控板目标文件的最大值
            Commentclass.MainBoardFileBinSizeMax = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainBinMax", "None", IniFilesPath)) * 1024;


            #region 原Init文件处理
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();//获取电脑上可用的串口号
            serialcomboBox.Items.AddRange(ports);      //给端口号选择窗口添加数据
            serialcomboBox.SelectedIndex = serialcomboBox.Items.Count > 0 ? 0 : -1;  //如果里面有数据显示第零个


            this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
            //Console.WriteLine("David--"+IniFilesPath);

           

            //进行相关的信息录入
            //读取ini文件,获取主控板ARM的 BootloaderHeadFlag 、CodeStartAddr、RedirectionAddr
            //var ReadIniTempFlag0 = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordBootloaderHeadFlagINI", "None", IniFilesPath);
            var ReadIniTempFlag0 = Commentclass.SettingMessageList[0];
            // Console.WriteLine($"ZKReadIniTemp = {ReadIniTempFlag0}.");
            string[] nametake0 = ReadIniTempFlag0.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 16; i++)
            {
                Commentclass.MainbordBootloaderHeadFlag[i] = Convert.ToByte(nametake0[i], 16);
                //    Console.WriteLine($"ZKBootloaderHeadFlag = {Commentclass.MainbordBootloaderHeadFlag[i].ToString("x")}.");
            }

            //获取CodeStartAddr
            //var ReadIniTempStartAddr0 = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordCodeStartAddrINI", "None", IniFilesPath);
            var ReadIniTempStartAddr0 = Commentclass.SettingMessageList[1];
            Commentclass.MainbordCodeStartAddr = Convert.ToUInt64(ReadIniTempStartAddr0, 16);
            //  Console.WriteLine($"ZKCodeStartAddr = {"0X" + Commentclass.MainbordCodeStartAddr.ToString("x")}.");

            //获取RedirectionAddr
            //var ReadIniTempTionAddr0 = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordRedirectionAddrINI", "None", IniFilesPath);
            var ReadIniTempTionAddr0 = Commentclass.SettingMessageList[2];
            Commentclass.MainbordRedirectionAddr = Convert.ToUInt64(ReadIniTempTionAddr0, 16);
            //   Console.WriteLine($"ZKRedirectionAddr = {"0X" + Commentclass.MainbordRedirectionAddr.ToString("x")}.");
            Commentclass.MainbordIniRecordBootloaderSize = Commentclass.MainbordRedirectionAddr - Commentclass.MainbordCodeStartAddr;



            //是否开启0xff校验
            string enable = ReadINIFiles.ReadIniData("ENABLECHECK", "AdvancedVerification", "None", IniFilesPath);
            Console.WriteLine($"enable = {enable}.");
            if (enable == "false")
            {
                Commentclass.ChexkActionEnable = false;
            }

            



            databox.Location = new Point(9, 34);
            databox.Size = new Size(386, 140);

            //  Console.WriteLine($"MainBordMaximalSize = {Commentclass.MainBordMaximalSize}.");
            //  Console.WriteLine($"MainBordMinimalSize = {Commentclass.MainBordMinimalSize }.");
            // Debug.WriteLine("lokokom");               //the likes printf      
            timer4.Start();
            //toolStripMenuItem1.BackColor = System.Drawing.Color.Chartreuse;
            //boundcomboBox.Enabled = true;
            #endregion

            Commentclass.CommentRuningKyeName = Commentclass.CommentMainResgistryKeyName;
            //读取路径，按读到的路径打开相应的文件路径
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName, out Commentclass.CommentReadSavePath);
            Console.WriteLine($"读出来的路径 = {Commentclass.CommentReadSavePath}.");
            if (Commentclass.CommentReadSavePath == "" | Commentclass.CommentReadSavePath == string.Empty)
            {
                Commentclass.CommentReadSavePath = "D:\\";
            }
            pathtextBox.Text = Commentclass.CommentReadSavePath;

            Admin_UI();
        }

        /// <summary>
        /// 注册表客户模式下隐藏住485刷机
        /// </summary>
        void Admin_UI()
        {
            this.boundcomboBox.Visible = false;
            this.label2.Visible = false;
            string msg;
            this.toolStripStatusLabel4.Visible = false;
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName, out msg);
            Console.WriteLine("模式：" + msg);
            if (msg == "User_Client")
            {
                this.menuStrip1.Items[1].Visible = false;
                this.menuStrip1.Items[3].Visible = false;
                this.menuStrip1.Items[5].Visible = false;

                

                this.Width = 450;
            }

        }

        #region 取代旧版的窗口跳转逻辑
        private void Menustrip_Change(object sender, ToolStripItemClickedEventArgs e)
        {

            //fmjump重新规定 ：00          01           02              03             04           05 
            //            DSP升级    充ARM485     充电ARM(Eth)       主ARM485       主ARM(Eth)    DspBootload
            //             Form3        Form2        Form8（新）       From1           From7        外部exe

            Console.WriteLine(e.ClickedItem);

            switch (e.ClickedItem.Text)
            {
                case "DSP以太网升级":
                    if (Commentclass.fmjump != 0x00)
                    {
                        Commentclass.fm3.Location = new Point(this.Location.X, this.Location.Y);
                        Commentclass.fm3.Show();
                        Commentclass.fmjump = 0x00;
                        this.Hide();
                    }
                    break;
                case "充电器485升级":
                    if (Commentclass.fmjump != 0x01)
                    {
                        Commentclass.fm2.Location = new Point(this.Location.X, this.Location.Y);
                        Commentclass.fm2.Show();
                        Commentclass.fmjump = 0x01;
                        this.Hide();
                    }
                    break;
                case "充电器以太网升级":
                    if (Commentclass.fmjump != 0x02)
                    {
                        Commentclass.fm8.Location = new Point(this.Location.X, this.Location.Y);
                        Console.WriteLine(this.Location.X.ToString() + "   " + this.Location.Y.ToString());
                        Commentclass.fm8.Show();
                        Commentclass.fm8.Location = new Point(this.Location.X, this.Location.Y);
                        Commentclass.fmjump = 0x02;
                        this.Hide();
                    }
                    break;
                case "ARM 485升级":
                    if (Commentclass.fmjump != 0x03)
                    {
                        Commentclass.fm1.Location = new Point(this.Location.X, this.Location.Y);
                        Commentclass.fm1.Show();
                        Commentclass.fmjump = 0x03;
                        this.Hide();
                    }
                    break;
                case "ARM以太网升级":
                    if (Commentclass.fmjump != 0x04)
                    {
                        Commentclass.fm7.Location = new Point(this.Location.X, this.Location.Y);
                        Commentclass.fm7.Show();
                        Commentclass.fmjump = 0x04;
                        this.Hide();
                    }
                    break;
                case "DspBootLoad":
                    {
                        //暂定
                    }

                    break;
                default:
                    break;
            }



        }

        #endregion
        #endregion

        #region 委托
        private void RecMsg(string str)
        {
          this.databox.AppendText(str + "\r\n");
        }
        #endregion

        #region  热拔插函数
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.WParam.ToInt32() == 0x8004)
                { //uab拔出串口
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames(); //重新获取串口号
                    serialcomboBox.Items.Clear();       //清除combox1里面的数据
                    serialcomboBox.Items.AddRange(ports);   //给combox1添加新的数据
                    serialcomboBox.SelectedIndex = serialcomboBox.Items.Count > 0 ? 0 : -1;      //显示获取第一个串口 
                }

                else if (m.WParam.ToInt32() == 0x8000)
                {//usb串口连接上
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    serialcomboBox.Items.Clear();
                    serialcomboBox.Items.AddRange(ports);
                    serialcomboBox.SelectedIndex = serialcomboBox.Items.Count > 0 ? 0 : -1;  //获取显示第一个串口号
                }
                base.WndProc(ref m);
            }
            catch 
            {
                
            }
        }
        #endregion

        #region  打开串口函数
        private bool Open_Serial()
        {
            if (string.IsNullOrEmpty(pathtextBox.Text))
            {
                if (!ClearFlash)
                {
                  //  Console.WriteLine("david--");
                    MessageBoxMidle.Show(this, "请选择您要下载的Bin文件！", "发送文件提示");
                    toolStripStatusLabel1.Text = "请选择需要下载的BIN文件！";
                    return false;
                }
            }
            try
            {
                serialPort1.PortName = serialcomboBox.Text; //获取要打开的串口号
                serialPortName = serialcomboBox.Text;       //将获取到的串口号存储起来
                serialPort1.BaudRate = int.Parse(boundcomboBox.Text);//获取波特率
                serialPort1.DataBits = int.Parse("8");               //设置数据位
                //===============设置停止位1================================================
                serialPort1.StopBits = StopBits.One;
                //=======================无奇偶校验=========================================
                serialPort1.Parity = Parity.None;
                serialPort1.Open();                                 //打开串口
                return true;
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开失败，请检查串口！", "提示！");//对话框显示打开失败
                toolStripStatusLabel1.Text = "串口打开失败！请检查串口！";
                enable_function();
                return false;
            }

        }
        #endregion

        #region 打开Bin文件并保存到建立的数组中
        private void filebutton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (string.IsNullOrEmpty(pathtextBox.Text))
                {
                    ofd.InitialDirectory = ("D:\\");
                }
                else
                {
                    //方便切换bin文件的时候，打开文件夹的初始位置就在bin文件所在文件夹，而不是再次从主磁盘开始
                    int i = pathtextBox.Text.LastIndexOf("\\");//获取字符串最后一个斜杠的位置
                    string path = pathtextBox.Text.Substring(0, i);//取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                    ofd.InitialDirectory = (path);
                }
                ofd.Filter = "bin文件;hex文件|*.bin;*.hex";//设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.pathtextBox.Text = ofd.FileName;
                    if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.hex文件
                    {
                        commentpath = System.Windows.Forms.Application.StartupPath + ".\\ZKBIN.\\ZKBIN.bin";
                        Console.WriteLine($"导入为hex文件");
                    }
                    else if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件
                    {
                        commentpath = pathtextBox.Text;
                        Console.WriteLine($"导入为bin文件");
                    }
                    WritePrivateProfileString(strOne, "ZKARMData_Source", pathtextBox.Text, str);
                    Console.WriteLine($"path = {pathtextBox.Text}.");
                }
                else
                {
                    Debug.WriteLine("取消打开文件");            //the likes printf
                    return;
                }
            }
            catch (Exception err)
            {
                //MessageBoxMidle.Show("文件路径错误，请检查文件路径格式。", "路径错误");
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = ("D:\\");
                ofd.Filter = "bin文件;hex文件|*.bin;*.hex";//设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.pathtextBox.Text = ofd.FileName;
                    if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.hex文件
                    {
                        commentpath = System.Windows.Forms.Application.StartupPath + ".\\ZKBIN.\\ZKBIN.bin";
                        Console.WriteLine($"导入为hex文件");
                    }
                    else if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件
                    {
                        commentpath = pathtextBox.Text;
                        Console.WriteLine($"导入为bin文件");
                    }
                    WritePrivateProfileString(strOne, "ZKARMData_Source", pathtextBox.Text, str);
                    Console.WriteLine($"path = {pathtextBox.Text}.");
                }
                else
                {
                    Debug.WriteLine("取消打开文件");            //the likes printf
                    return;
                }
            }
        }
        #endregion

        #region 开始下载程序
        private void loadbutton_Click_1(object sender, EventArgs e)
        {
            
            StackTrace st = new StackTrace(new StackFrame(true));
            DavidDebug.showMsg("点击下载按键", st);
            Commentclass.CommentSpecialMainNumCheck = Convert.ToUInt16(Commentclass.SettingMessageList[8]);
            //Console.WriteLine($" Commentclass.CommentSpecialMainNumCheck = { Commentclass.CommentSpecialMainNumCheck}.");
            Commentclass.CommentHeadCheckNumLength = Commentclass.CommentSpecialMainNumCheck;
            CutoutActionFlag = true;
            databox.Clear();
            disable_function();
            //if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.hex文件
            //{
            //    HexFileRead(pathtextBox.Text);
            //    filedata = new byte[bufferAdr];
            //    Array.Clear(filedata, 0, filedata.Length);
            //    Buffer.BlockCopy(buffer, 0, filedata, 0, bufferAdr);    // 将buffer复制到filedata之中
            //    //File.Delete(commentpath);   //先删除文件
            //    //FileStream fsWrite = new FileStream(commentpath, FileMode.OpenOrCreate, FileAccess.Write);//再创建
            //    //fsWrite.Write(buffer, 0, bufferAdr);
            //    //fsWrite.Close();                //释放生成的bin文件

            //}
            //加一个确定波特率的函数
            First_Run = false; //上位机不是首次打开
            //databox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            databox.Show();
            toolStripStatusLabel1.Text = "等待校验中...";

            //保存文件地址
            RegistryKeyLi.WriteRegistryKey(this.pathtextBox.Text, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainResgistryKeyName);

            load_file_onthepath();
        }
        #endregion

        #region 默认路径读取文件
        int length;
        private void load_file_onthepath()
        {
            try
            {
                if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.hex文件
                {
                    //commentpath = System.Windows.Forms.Application.StartupPath + ".\\ZKBIN.\\ZKBIN.bin";
                    CutoutActionFlag = false;
                    HexFileRead(pathtextBox.Text);
                    filedata = new byte[bufferAdr];
                    Array.Clear(filedata, 0, filedata.Length);
                    Buffer.BlockCopy(buffer, 0, filedata, 0, bufferAdr);    // 将buffer复制到filedata之中
                    length = filedata.Length;
                  //  Console.WriteLine($"导入为hex文件");
                }           
                else if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件
                {
                    commentpath = pathtextBox.Text;
                //    Console.WriteLine($"导入为bin文件");
                    FileStream fs = new FileStream(commentpath, FileMode.Open, FileAccess.Read);
                    byte[] arrfileSend = new byte[1024 * 1024 * 8];         //创建8M的缓存
                    length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                    byte[] arrfile = new byte[length];
                    fs.Close();                                              //关闭文件流，释放文件
                    Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);    // 将arrfileSend复制到arrfile之中
                    filedata = new byte[length];
                    Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);   // 将arrfileSend复制到arrfile之中
                }
                else
                {
                    enable_function();
                    MessageBoxMidle.Show("请选择要下载的ARM.hex文件！！" , "文件错误");
                    pathtextBox.Clear();
                    return;
                }
                Console.WriteLine($"filedata = {filedata.Length}.");

                /*
                if (filedata.Length < (int)Commentclass.MainBordMinimalSize || filedata.Length > (int)Commentclass.MainBordMaximalSize)  //文件小于90k的和大于200k的都不行
                {
                    enable_function();
                    MessageBoxMidle.Show("导入文件大小错误，非主控板ARM刷机文件！\r\n" + "请重新导入。", "文件错误");
                    return;
                }
                */
                //根据文件后缀名做新的大小判断
                if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件
                {
                    //Commentclass.CommentFileData = new byte[Commentclass.CutSectorFileData.Length];
                    //Buffer.BlockCopy(Commentclass.CutSectorFileData, 0, Commentclass.CommentFileData, 0, Commentclass.CutSectorFileData.Length);
                    //Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;
                    //判断主控板的刷机文件是否合适
                    if (((uint)filedata.Length < Commentclass.MainBoardFileBinSizeMin) || ((uint)filedata.Length > Commentclass.MainBoardFileBinSizeMax))
                    {
                        enable_function();
                        MessageBoxMidle.Show(this, "当前导入的程序文件有误！请重新导入正确的程序文件!", "提示!");
                        return;
                    }
                }
                else if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.bin文件
                {
                    //Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;
                    //判断主控板的刷机文件是否合适
                    if (((uint)filedata.Length < Commentclass.MainBoardFileHexSizeMin) || ((uint)filedata.Length > Commentclass.MainBoardFileHexSizeMax))
                    {
                        enable_function();
                        MessageBoxMidle.Show(this, "当前导入的程序文件有误！请重新导入正确的程序文件!", "提示!");
                        return;
                    }
                }

                //提示
                DialogResult messdr = MessageBoxMidle.Show(this, "请先打手动开关进入手动模式！\r\n是否已完成该操作？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (messdr == DialogResult.No)
                {
                    enable_function();
                    return;
                }

                //截取代码
                if (CutoutActionFlag)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    DavidDebug.showMsg("bin文件，需要截取", st);
                    backack = ReadFiles.FragmentCode(Commentclass.MainbordRedirectionAddr, Commentclass.MainbordCodeStartAddr, (UInt64)filedata.Length,
                              Commentclass.MainbordIniRecordBootloaderSize, Commentclass.MainbordBootloaderHeadFlag, filedata, out Cutoutfiledata);
                   // Console.WriteLine($"截取文件.");
                }
                if (backack == "ok")
                {
                    //重新覆盖原来的刷机文件
                    if (CutoutActionFlag)
                    {
                        filedata = new byte[Cutoutfiledata.Length];
                        Buffer.BlockCopy(Cutoutfiledata, 0, filedata, 0, Cutoutfiledata.Length);
                        length = filedata.Length;//重新获取文件的长度
                    }

                    ////导出Bin文件(验证用)
                    //FileStream fstemp = new FileStream("主控板截取文件.bin", FileMode.OpenOrCreate);
                    //BinaryWriter binWriter = new BinaryWriter(fstemp);
                    //binWriter.Write(filedata, 0, filedata.Length);
                    //binWriter.Close();
                    //fstemp.Close();
                    //return;

                    step_i = 0;                                              //初始化
                    duty = 0;                                                //初始化
                    pagenum = length / 128;                                  //初始化  以128个字节为一个数据包发送数据
                    remaind = length % 128;                                  //初始化
                    cjremaind = remaind;
                //   Console.WriteLine($"remaind = {remaind}.");
                //    Console.WriteLine($"pagenum = {pagenum}.");
                    loadprogressBar.Maximum = pagenum - 1;                   //进度条的最大值初始化
                    toolStripStatusLabel1.Text = "文件已导入，请确认！";
                    Click_Start();
                }
                else
                {
                    MessageBoxMidle.Show(this, backack , "错误");
                    enable_function();
                }
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开文件失败！", "错误");
                enable_function();
                toolStripStatusLabel1.Text = "文件导入失败，请检查！";
            }
        }
        #endregion

        #region 点击函数
        public void Click_Start()
        {
            if (this.Iscir_lode == true)
            {
                this.Lode_sumtimes += 1;
                this.textBox3.Text = this.Lode_sumtimes.ToString();
            }
            if (!First_Run)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                DavidDebug.showMsg("首次运行，初始化参数，开启串口，启动定时器1", st);
                Systemtemp_int();  //如果不是第一次则要对变量初始化
            }

            if (serialPort1.IsOpen)
            {
               
                timer1.Start();     //确认主机是否处于IAP
            }
            else
            {
                if (Open_Serial())  //打开串口
                {
                    timer1.Start(); //确认主控是否处于IAP
                }
                else
                {
                    return;
                }
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
            Step_One = true;
            Step_Tow = false;
            orde = false;
            Bohead[1] = 0x01;
            Bohead[2] = 0xFE;
            Resive_step = 0;
            ErrTryCount = 0;
            loadtimecount = false;
            loadtime = 0;
            LinkCount = 0;
            boundcomboBox.Text = "115200";
            yuchuliflag = false;
            cansend = true;
            ClearFlash = false;
        }
        #endregion

        #region 串口发送函数(CRC校验)

        private void Usart_Send(byte[] data)
        {
            try  //星辰CRC校验
            {
                if (cansend)
                {
                    int length = data.Length;   //需要发送数据的长度
                    UInt16 CRC_Value = StartsCRC.CRC16(data, length);
                    CrcCheck[0] = Convert.ToByte(CRC_Value / 256);                   //将CRC校验值写入发送的数据
                    CrcCheck[1] = Convert.ToByte(CRC_Value % 256);
                    //======在这里加上帧头、报数据、反报数据、校验位==============
                    byte[] alldata = new byte[Bohead.Length + data.Length + CrcCheck.Length];
                    Buffer.BlockCopy(Bohead, 0, alldata, 0, Bohead.Length);  //将Bohead放到alldata之中
                    Buffer.BlockCopy(data, 0, alldata, Bohead.Length, data.Length);  //将data放到alldata之中
                    Buffer.BlockCopy(CrcCheck, 0, alldata, Bohead.Length + data.Length, CrcCheck.Length);  //将CrcCheck放到alldata之中
                    SneCapre = new byte[alldata.Length];
                    Buffer.BlockCopy(alldata, 0, SneCapre, 0, alldata.Length);   // 将data复制到SneCapre之中
                    if (powerdisplay)
                    {
                        Invoke(myRcvMsgfm1, "发送：" + StartsCRC.zftring(alldata));
                    }
                    serialPort1.Write(alldata, 0, alldata.Length);  //串口发送数据
                                                                    //在这里进行报数据和反报数据的处理
                    Bohead[1] = (byte)(Bohead[1] + 1);         //报数据
                    Bohead[2] = (byte)(0xff - Bohead[1]);     //反报数据
                    System_RS = false;          //等待校验成功
                    timer7.Start();
                }
            }
            catch
            {
                cansend = false;
                Connect = false;
                enable_function();
                if (!ClearFlash)
                {
                    MessageBoxMidle.Show(this, "检查通信线是否断开,重新执行刷机操作\r\n", "串口发送错误");
                }
                else
                {
                    MessageBoxMidle.Show(this, "检查通信线是否断开,重新执行擦除操作\r\n", "串口发送错误");
                }
                step_i = 0;
                menuStrip1.Enabled = true;
            }
        }
        #endregion

        #region 串口发送函数（无CRC校验）
        private void Usart_Send_Ncrc(byte[] data)
        {
            try  //星辰CRC校验
            {
                if (cansend)
                {
                    serialPort1.Write(data, 0, data.Length);  //串口发送数据
                    SneCapre = new byte[data.Length];
                    Buffer.BlockCopy(data, 0, SneCapre, 0, data.Length);   // 将data复制到SneCapre之中
                    if (powerdisplay)
                    {
                        Invoke(myRcvMsgfm1, "发送：" + StartsCRC.zftring(SneCapre));
                    }
                    //System_RS = false;          //等待校验成功
                }
            }
            catch
            {
                cansend = false;
                enable_function();
                if (!ClearFlash)
                {
                    MessageBoxMidle.Show(this, "检查通信线是否断开,重新执行刷机操作\r\n", "串口发送错误");
                }
                else
                {
                    MessageBoxMidle.Show(this, "检查通信线是否断开,重新执行擦除操作\r\n", "串口发送错误");
                }
            }

        }
        #endregion

        #region 发送的和接收的做比较
        public static bool Compare_Data(byte[] Send_vc, byte[] Rsive_vc)
        {
            try
            {
                Console.WriteLine(BitConverter.ToString(Rsive_vc).Replace("-", ""));
                if (Enumerable.SequenceEqual(Send_vc, Rsive_vc))  //按相同序列检查两数组的元素是否相同
                    return true;  //相同返回true
                else
                    return false; //不同返回false 
            }
            catch
            {
                return false; //不同返回false
                //MessageBoxMidle.Show("校验错误！", "错误");
            }
        }
        #endregion

        #region 应答式发送消息函数
        private void ACK_Sendfunction()
        {
            while (Connect)
            {
                try
                {
                    Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                    {
                        if (System_RS)
                        {
                            if (step_i < pagenum)
                            {
                                Buffer.BlockCopy(filedata, (step_i * 128), filedata_temp, 0, 128);
                            //    Console.WriteLine($"filedata = {filedata.Length}.");
                                Usart_Send(filedata_temp);              //发送128个字节数据
                                loadprogressBar.Value = step_i;
                                duty = (((double)step_i / (double)pagenum) * 100);
                                step_i = step_i + 1;
                             //   Console.WriteLine($"step_i = {step_i}.");
                                toolStripStatusLabel1.Text = "程序下载中:" + duty.ToString("0.00") + "%";

                            }
                            else if (cjremaind > 0)
                            {
                                //不够128，就填充0X1A一直填充满128个字节
                                Buffer.BlockCopy(filedata, (pagenum * 128), filedata_temp, 0, remaind);
                                for (; cjremaind < 128; cjremaind++)
                                {
                                    filedata_temp[cjremaind] = 0x1A;
                                }
                                Usart_Send(filedata_temp);              //发送剩余的数据
                                duty = (((double)(step_i) / (double)pagenum) * 100);
                                toolStripStatusLabel1.Text = "程序下载中:" + duty.ToString("0.00") + "%";

                                loadprogressBar.Value = 0;
                                Connect = false;
                                //烧录完成处理函数
                                timer2.Start();
                                //Action_End_Function();
                            }
                            else
                            {
                                loadprogressBar.Value = 0;
                                Connect = false;
                                //烧录完成处理函数
                                timer2.Start();
                                //Action_End_Function();
                            }
                        }
                    })));
                }
                catch (Exception err)
                {
                    enable_function();
                    MessageBoxMidle.Show(this, "失败！" + err.ToString(), "错误");
                    break;
                }
            }
        }
        #endregion

        #region 烧录完成处理函数
        private void Action_End_Function()
        {
            //loadtimecount = false;           //
            //AckSend_Function.Abort();       // 关闭线程

        }
        #endregion

        #region 串口接收函数以及数据处理 串口接收事件
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            int len = serialPort1.BytesToRead;          //获取可以读取的字节数
            if (len == 0) return;                       //为啥有时候为0
            byte[] Resive_data = new byte[len];         //创建缓存的数据数组
            serialPort1.Read(Resive_data, 0, len);      //把数据读取到buff数组
            Invoke((new Action(() =>                    //c#3.0以后代替委托的新方法
            {
                //Console.WriteLine("Daivd---Resive:"+BitConverter.ToString(Resive_data).Replace("-", ""));
                if (powerdisplay)
                {
                    Invoke(myRcvMsgfm1, "接收：" + StartsCRC.zftring(Resive_data));
                }
                #region 第一步
                if (Step_One && Resive_step == 0 && len == 1)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    DavidDebug.showMsg("LinkCounte==" + Convert.ToString(Resive_data[0],16) + " --串口接收响应，Step_one&&Resive_step判断首字节",st);
                    if (Resive_data[0] == 0X43)    //发送擦除指令
                    {
                        Usart_Send_Ncrc(EraseFlash);        //发送擦除指令
                        yuchuliflag = true;
                        if (!powerdisplay)
                            databox.AppendText(Commentclass.MainStateMessageList[3] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        Debug.WriteLine("1主控进入IAP");
                    }
                    else if (Resive_data[0] == 0x07)
                    {
                        System_RS = true;
                        Step_One = false;
                        Step_Tow = true;
                        if (!powerdisplay)
                            databox.AppendText(Commentclass.MainStateMessageList[4] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        if (ClearFlash)//只是一键擦除的功能
                        {
                            timer1.Stop();
                            timer6.Start();
                        }
                        Debug.WriteLine("1主控进入IAP");
                    }
                    else if (Resive_data[0] == 0x08)
                    {
                        if (!powerdisplay)
                            databox.AppendText(Commentclass.MainStateMessageList[5] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        if (ClearFlash)//只是一键擦除的功能
                        {
                            databox.AppendText("擦除失败，重新擦除-------------" + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                            timer1.Stop();
                            timer6.Start();
                        }
                        Debug.WriteLine("1主控进入IAP");
                    } 
                }
                if (Step_One && !yuchuliflag)
                {
                    Resive_step = Resive_step + 1;
                    if (Resive_step == 1)
                    {
                        
                        if (Compare_Data(SneCapre, Resive_data))        //判断是否符合校验
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            DavidDebug.showMsg("LinkCounte==" + Convert.ToString(LinkCount) + " --串口接收响应，判断全字节正确", st);
                            System_RS = true;
                            orde = true;
                            Debug.WriteLine("比较相同");
                            if (!powerdisplay)
                                databox.AppendText(Commentclass.MainStateMessageList[1] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        }
                        else
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            DavidDebug.showMsg("LinkCounte==" + Convert.ToString(LinkCount) + " --串口接收响应，判断全字节错误", st);
                            System_RS = true;
                            Resive_step = 0;
                            Debug.WriteLine("第一次响应错诶！");
                        }
                    }
                    else if (Resive_step == 2)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        DavidDebug.showMsg("LinkCounte==" + Convert.ToString(LinkCount) + " --串口接收响应，判断单字节", st);
                        if (Resive_data[0] == 0X43)    //发送擦除指令
                        {
                            Usart_Send_Ncrc(EraseFlash);        //发送擦除指令
                            Resive_step = 1;
                            if (!powerdisplay)
                                databox.AppendText(Commentclass.MainStateMessageList[3] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                            Debug.WriteLine("1主控进入IAP");
                        }
                        else if (Resive_data[0] == 0x07)
                        {
                            System_RS = true;
                            Step_One = false;
                            Step_Tow = true;
                            if (!powerdisplay)
                                databox.AppendText(Commentclass.MainStateMessageList[4] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                            Debug.WriteLine("1主控进入IAP");
                            if (ClearFlash)//只是一键擦除的功能
                            {
                                timer1.Stop();
                                timer6.Start();
                            }
                        }
                        else if (Resive_data[0] == 0x08)
                        {
                            if (!powerdisplay)
                                databox.AppendText(Commentclass.MainStateMessageList[5] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                            Debug.WriteLine("1主控进入IAP");
                            if (ClearFlash)//只是一键擦除的功能
                            {
                                databox.AppendText("擦除失败，重新擦除-------------" + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                                timer1.Stop();
                                timer6.Start();
                            }
                        }
                        else
                        {
                            Resive_step = 1;
                            orde = true;
                            Debug.WriteLine("第二次响应错诶！");
                            toolStripStatusLabel1.Text = "第二次响应无效，重连中...";
                        }
                    }
                }
                #endregion
                #region 第二步
                if (Step_Tow)
                {
                    if (Resive_data[0] == OkFlag)
                    {
                        System_RS = true;
                        if (!powerdisplay)
                            databox.AppendText(Commentclass.MainStateMessageList[1] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        Debug.WriteLine("传输正确！");
                        timer7.Stop();
                    }
                    else if (Resive_data[0] == ErrFlag)
                    {
                        if (ErrTryCount <= 3)
                        {
                            ErrTryCount++;
                            if(ErrTryCount >0x01)
                            {
                              databox.AppendText(Commentclass.MainStateMessageList[2] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                            }
                            Debug.WriteLine("传输错误！");
                            timer3.Start();
                        }       //进入定时器重发3次操作
                        else
                        {
                            Connect = false;
                            timer3.Stop();                  //定时器溢出区判断校验是否通过
                            timer6.Start();
                            ErrTryCount = 0;
                        }
                    }
                    //else if (Resive_data[0] == 0x07 || Resive_data[0] == 0x08)
                    //{
                    //    System_RS = true;
                    //}
                }
                #endregion
            })));
        }
        #endregion

        #region 窗体关闭事件
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loadbutton.Enabled == false)
            {
                //DialogResult Result = MessageBoxMidle.Show(this, "是否退出？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //if (Result == DialogResult.OK)
                //{
                //    timer5.Stop();
                //    if (serialPort1.IsOpen)
                //    {
                //        serialPort1.Close();
                //    }
                //    Application.Exit();
                //    Console.WriteLine($"退出.");
                //}
                //else
                //{
                e.Cancel = true;
                //}              
            }
            else
            {
                timer4.Stop();
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
                Application.Exit();
            }
        }
        #endregion

        #region 判断主控ARM是否为IAP，如果不是则让主控进入IAP time1_tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Step_One && !yuchuliflag)
            {
                if (!orde)
                {
                    if (LinkCount == 0)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        DavidDebug.showMsg("控制ARM进入IAP，调整串口波特率为19200", st);
                        serialPort1.BaudRate = int.Parse("19200");  //切换波特率
                        boundcomboBox.Text = "19200";
                    }
                    LinkCount = LinkCount + 1;
                    
                    if (LinkCount <= 3)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        DavidDebug.showMsg("LinkCounte=="+ Convert.ToString(LinkCount)+ " --发送Readyorder控制码",st);
                        Usart_Send_Ncrc(Readyoder);
                    }
                    else if (LinkCount == 4)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        DavidDebug.showMsg("LinkCounte==" + Convert.ToString(LinkCount) + " --修改串口波特率为115200",st);
                        serialPort1.BaudRate = int.Parse("115200");  //切换波特率
                        boundcomboBox.Text = "115200";
                        Thread.Sleep(10);
                    }
                    else if (4 < LinkCount && LinkCount <= 10)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        DavidDebug.showMsg("LinkCounte==" + Convert.ToString(LinkCount) + " --发送Readyorder控制码", st);
                        Usart_Send_Ncrc(Readyoder);
                    }
                    else
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        DavidDebug.showMsg("LinkCounte==" + Convert.ToString(LinkCount) + " --IAP响应失败", st);
                        LinkCount = 0;
                        timer1.Stop();
                        serialPort1.Close();
                        databox.AppendText(Commentclass.MainStateMessageList[8] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        enable_function();
                        if (!ClearFlash)
                        {
                            MessageBoxMidle.Show(this, "无响应！请重新点击下载。", "提示!!");               //弹出提示框
                        }
                        else
                        {
                            MessageBoxMidle.Show(this, "无响应！请重新擦除。", "提示!!");               //弹出提示框
                        }
                    }
                    Debug.WriteLine("让主控进入IAP!");                   //the likes printf

                    databox.AppendText(Commentclass.MainStateMessageList[0] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                    toolStripStatusLabel1.Text = "等待主控板ARM进入IAP...";
                }
                else if (orde && Resive_step == 1)
                {
                    LinkCount = LinkCount + 1;
                    if (LinkCount < 15)
                    {
                        if (boundcomboBox.Text != "115200")
                        {
                            serialPort1.BaudRate = int.Parse("19200");  //切换波特率
                        }
                        Usart_Send_Ncrc(Readyoder1);                    //主控ARM跳转IAP第一个命令，返回相同值
                        Thread.Sleep(10);
                        serialPort1.BaudRate = int.Parse("115200");     //切换波特率
                        orde = false;
                    }
                    else
                    {
                        timer1.Stop();
                        LinkCount = 0;
                        serialPort1.Close();
                        databox.AppendText(Commentclass.MainStateMessageList[9] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        MessageBoxMidle.Show(this, "连接失败！", "提示!!"); //弹出提示框
                        enable_function();
                    }
                }
            }
            else if (Step_Tow)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                DavidDebug.showMsg("IAP启动成功，开始下载！", st);
                timer1.Stop();
                boundcomboBox.Text = "115200";
                Open_Threading();//开启线程
            }         
        }
        #endregion

        #region 共用的开启线程的函数
        private void Open_Threading()
        {
            AckSend_Function = new Thread(ACK_Sendfunction);
            AckSend_Function.IsBackground = true;
            AckSend_Function.Start();
            Console.WriteLine("线程开启！");
            toolStripStatusLabel1.Text = "传输线程开启...";
            loadtimecount = true;           //开始下载计时
            menuStrip1.Enabled = false;
        }
        #endregion

        #region 发送完BIN文件告诉ARM跳转
        private void timer2_Tick(object sender, EventArgs e)
        {
            loadtimecount = false;
            Usart_Send_Ncrc(Overoder);
            Thread.Sleep(10);
            Usart_Send_Ncrc(Overoder);
            Thread.Sleep(10);
            Usart_Send_Ncrc(Overoder);
            if (!powerdisplay)
                databox.AppendText(Commentclass.MainStateMessageList[6] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
            databox.AppendText(Commentclass.MainStateMessageList[7] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
            toolStripStatusLabel1.Text = "程序升级完成！";
            timer2.Stop();
            enable_function();

            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
            {
                AckSend_Function.Abort();       // 关闭线程          
                menuStrip1.Enabled = true;
                serialPort1.Close();
            })));

            if (this.Iscir_lode == true)
            {
                this.Lode_successfultimes += 1;
                this.textBox2.Text = this.Lode_successfultimes.ToString();
                Thread.Sleep(500);
                Click_Start();

            }
            

        }
        #endregion

        #region 错误重发函数 Tim3_tick
        private void timer3_Tick(object sender, EventArgs e)
        {
            Usart_Send(filedata_temp);              //发送128个字节数据
            timer3.Stop();
        }
        #endregion

        #region 时间显示
        private void timer4_Tick(object sender, EventArgs e)
        {
            //this.toolStripStatusLabel3.Text = "系统时间：" + DateTime.Now.ToString("hh:mm:ss");
            if (loadtimecount)
            {
                loadtime = loadtime + 1;
            }
          //  this.toolStripStatusLabel2.Text = "计时:" + loadtime.ToString("0.00") + "s";
        }
        #endregion

        #region DSP窗体打开函数
        private void DSP程序升级(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x01;  //从窗口1条过去
            int winx = windowX;
            int winy = windowY;
            Commentclass.fm3.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm3.Show();
        }
        #endregion

        #region 2跳转到充电板1
        private void 充电板程序升级(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x01;  //从窗口1条过去
            int winx = windowX;
            int winy = windowY;
            Commentclass.fm2.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm2.Show();
        }
        #endregion

        #region 跳转到Eth
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x01;  //从窗口1条过去
            int winx = windowX;
            int winy = windowY;
            Commentclass.fm7.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm7.Show();
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
                OpenFileDialog ofd = new OpenFileDialog();

                if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.hex文件
                {
                   // commentpath = System.Windows.Forms.Application.StartupPath + ".\\ZKBIN.\\ZKBIN.bin";
                    Console.WriteLine($"导入为hex文件");
                }
                else if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件
                {
                   // commentpath = pathtextBox.Text;
                    Console.WriteLine($"导入为bin文件");
                }
                else if (Path.GetExtension(pathtextBox.Text) != ".hex")
                {
                    MessageBoxMidle.Show(this, "导入文件不是*.hex/*.bin类型文件！", "警告");
                    pathtextBox.Clear();
                    return;
                }
                //FileStream fs = new FileStream(pathtextBox.Text, FileMode.Open);
                //byte[] arrfileSend = new byte[1024 * 1024 * 8];         //创建8M的缓存
                //int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                //byte[] arrfile = new byte[length];
                //fs.Close();                                              //关闭文件流，释放文件

                //Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);    // 将arrfileSend复制到arrfile之中
                //filedata = new byte[length];
                //Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);   // 将arrfileSend复制到arrfile之中
                //Console.WriteLine($"filedata = {filedata.Length}.");

                ////加上文件大小判断
                //if (filedata.Length < 92160)
                //{
                //    string filename = System.IO.Path.GetFileName(pathtextBox.Text);
                //    DialogResult messdr = MessageBoxMidle.Show(this, "当前操对象:\r\n  主控板ARM！\r\n\r\nBin文件名称：\r\n  " + filename + "\r\n\r\n请确认！", "BIN文件异常提示！", MessageBoxButtons.YesNo);
                //    if (messdr == DialogResult.No)
                //    {
                //        toolStripStatusLabel1.Text = "文件操作已中止,请重新导入！";
                //        return;
                //    }
                //}
                //step_i = 0;                                              //初始化
                //duty = 0;                                                //初始化
                //pagenum = length / 128;                                  //初始化  以128个字节为一个数据包发送数据
                //remaind = length % 128;                                  //初始化
                //cjremaind = remaind;
                //Console.WriteLine($"remaind = {remaind}.");
                //loadprogressBar.Maximum = pagenum - 1;                   //进度条的最大值初始化
                //toolStripStatusLabel1.Text = "文件已导入，请确认！";
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开文件失败！" + err.ToString(), "错误");
                toolStripStatusLabel1.Text = "文件导入失败，请检查";
            }
        }
        #endregion

        #region 图片浏览
        public void Pic_Vision()
        {
            string path = Application.StartupPath + "\\icon\\";
            //Debug.WriteLine($"step_i = {path}");
            string[] str = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
            int Number = str.Length;
            if (i < Number)
            {
                string StrName = path + Path.GetFileName(str[i]).ToLower();
                if (File.Exists(StrName))
                {
                    FileStream filestream = new FileStream(StrName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    //this.pictureBox2.Image = Image.FromStream(filestream);
                    filestream.Close();
                }
                i += 1;
                if (i == Number)
                {
                    i = 0;
                }
            }
        }
        #endregion

        #region 图片刷新函数
        private void timer5_Tick(object sender, EventArgs e)
        {
           // Pic_Vision();
        }
        #endregion

        #region 链接到网址
        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
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
        #endregion

        #region 获取窗体的位置

        private void Form1_Move(object sender, EventArgs e)
        {
            windowX = this.Location.X;
            windowY = this.Location.Y;
          //  this.toolStripStatusLabel2.Text = windowX.ToString() + "," + windowY.ToString();
        }
        #endregion

        #region 烧录失败处理函数
        private void timer6_Tick(object sender, EventArgs e)
        {
            timer6.Stop();
            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
            {
                if (!ClearFlash)
                {
                    AckSend_Function.Abort();       //关闭线程
                }
                serialPort1.Close();            //关闭串口
                enable_function();
                if (!ClearFlash)
                {
                    MessageBoxMidle.Show(this, "程序升级失败！", "错误");
                }
                else
                {
                    databox.AppendText("程序擦除完成-----------------" + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                    ClearFlash = false;
                }
                menuStrip1.Enabled = true;
            })));
        }
        #endregion

        #region 杜绝无应答
        private void timer7_Tick(object sender, EventArgs e)
        {
            if (!System_RS)
            {
                timer7.Stop();
                Connect = false;
                Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                {
                    AckSend_Function.Abort();       //关闭线程
                    toolStripStatusLabel1.Text = "程序升级失败！！";
                    MessageBoxMidle.Show(this, "程序升级失败！", "错误");
                    enable_function();
                    loadprogressBar.Value = 0;
                    menuStrip1.Enabled = true;
                    serialPort1.Close();            //关闭串口
                })));
            }
        }

        #endregion

        #region 回车函数
        private void boundcomboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (boundcomboBox.Text == "1024")
                    {
                        powerdisplay = true;
                    }
                    else if (boundcomboBox.Text == "2048")
                    {
                        powerdisplay = false;
                    }
                    else if (boundcomboBox.Text == "4096")
                    {
                        databox.Clear();
                    }
                    boundcomboBox.Text = "115200";
                }
                catch (Exception err)
                { MessageBox.Show("详情：" + err.ToString(), "警告"); }
            }
        }
        #endregion

        #region Hex文件解析
        static public void HexFileRead(string filepath)
        {
            #region Hex文件解析
            string szLine;
            int startAdr;
            //int endAdr = 0;   //用于判断hex地址是否连续，不连续补充0xFF
            buffer = new byte[1024 * 1024 * 8];       // 打开文件hex to bin缓存8M大小
            bufferAdr = 0;

            if (filepath == "")
            {
                return;
            }
            FileStream fsRead = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader HexReader = new StreamReader(fsRead); //读取数据流
            while (true)
            {
                szLine = HexReader.ReadLine(); //读取Hex中一行
                if (szLine == null) { break; } //读取完毕，退出
                if (szLine.Substring(0, 1) == ":") //判断首字符是”:”
                {
                    if (szLine.Substring(1, 8) == "00000001") { break; } //文件结束标识
                    if ((szLine.Substring(8, 1) == "0") || (szLine.Substring(8, 1) == "1"))//直接解析数据类型标识为 : 00 和 01 的格式
                    {
                        int lineLenth;
                        string hexString;

                        hexString = szLine.Substring(1, 2);
                        lineLenth = Int32.Parse(hexString, System.Globalization.NumberStyles.HexNumber); // 获取一行的数据个数值

                        hexString = szLine.Substring(3, 4);
                        startAdr = Int32.Parse(hexString, System.Globalization.NumberStyles.HexNumber); // 获取地址值
                    //    Console.WriteLine($"startAdr = {startAdr}.");

                        //for (int i = 0; i < startAdr - endAdr; i++) // 补空位置
                        //{
                        //    hexString = "FF";
                        //    byte value = byte.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                        //    buffer[bufferAdr] = value;
                        //    bufferAdr++;
                        //}

                        for (int i = 0; i < lineLenth; i++) // hex转换为byte
                        {
                            hexString = szLine.Substring(i * 2 + 9, 2);
                            byte value = byte.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                            buffer[bufferAdr] = value;
                            bufferAdr++;
                        }
                        //endAdr = startAdr + lineLenth;
                    }
                }
            }
            #endregion
        }//从指定文件目录读取HEX文件并解析，放入缓存数组buffer中
        #endregion

        #region 强制退出
        private void 强制退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region 清除和文件保存功能
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

        private void disable_function()
        {
            menuStrip1.Enabled = false;
            loadbutton.Enabled = false;
            filebutton.Enabled = false;
            this.groupBox1.Enabled = false;
        }

        private void enable_function()
        {
            menuStrip1.Enabled = true;
            loadbutton.Enabled = true;
            filebutton.Enabled = true;
            this.groupBox1.Enabled = true;
        }

        #region 一键擦除功能
        private void 一键擦除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            step_i = 0;
            duty = 0;
            cjremaind = remaind;
            Connect = true;
            System_RS = true;
            Step_One = true;
            Step_Tow = false;
            orde = false;
            Bohead[1] = 0x01;
            Bohead[2] = 0xFE;
            Resive_step = 0;
            ErrTryCount = 0;
            loadtimecount = false;
            loadtime = 0;
            LinkCount = 0;
            boundcomboBox.Text = "115200";
            yuchuliflag = false;
            cansend = true;
            ClearFlash = true;
            databox.Clear();
            //Click_Start();
            if (serialPort1.IsOpen)
            {
                timer1.Start();     //确认主机是否处于IAP
            }
            else
            {
                if (Open_Serial())  //打开串口
                {
                    timer1.Start(); //确认主控是否处于IAP
                }
                else
                {
                    return;
                }
            }

        }
        #endregion

        private void databox_TextChanged(object sender, EventArgs e)
        {
            databox.Focus();
        }

        /// <summary>
        /// <David--当用户切换界面时，用来初始化控件的位置
        /// </summary>
        private void Refreshui()
        {
            Commentclass.PortlablePointdata.x = 7;
            Commentclass.PortlablePointdata.y = 198;
            Commentclass.PortlablePointdata.weith = 44;
            Commentclass.PortlablePointdata.heigth = 12;
            Commentclass.PortcomboxPointdata.x = 46;
            Commentclass.PortcomboxPointdata.y = 195;
            Commentclass.PortcomboxPointdata.weith = 92;
            Commentclass.PortcomboxPointdata.heigth = 20;
            Commentclass.BoundlablePointdata.x = 179;
            Commentclass.BoundlablePointdata.y = 198;
            Commentclass.BoundlablePointdata.weith = 57;
            Commentclass.BoundlablePointdata.heigth = 12;
            Commentclass.BoundcomboxPointdata.x = 230;
            Commentclass.BoundcomboxPointdata.y = 195;
            Commentclass.BoundcomboxPointdata.weith = 92;
            Commentclass.BoundcomboxPointdata.heigth = 20;
            Commentclass.FilelablePointdata.x = 7;
            Commentclass.FilelablePointdata.y = 238;
            Commentclass.FilelablePointdata.weith = 44;
            Commentclass.FilelablePointdata.heigth = 12;
            Commentclass.FiletextboxPointdata.x = 45;
            Commentclass.FiletextboxPointdata.y = 233;
            Commentclass.FiletextboxPointdata.weith = 276;
            Commentclass.FiletextboxPointdata.heigth = 21;
            Commentclass.PrglablePointdata.x = 7;
            Commentclass.PrglablePointdata.y = 273;
            Commentclass.PrglablePointdata.weith = 44;
            Commentclass.PrglablePointdata.heigth = 12;
            Commentclass.PrgformPointdata.x = 45;
            Commentclass.PrgformPointdata.y = 268;
            Commentclass.PrgformPointdata.weith = 344;
            Commentclass.PrgformPointdata.heigth = 18;
            Commentclass.DownbtnPointdata.x = 328;
            Commentclass.DownbtnPointdata.y = 191;
            Commentclass.DownbtnPointdata.weith = 62;
            Commentclass.DownbtnPointdata.heigth = 25;
            Commentclass.ViewbtnPointdata.x = 328;
            Commentclass.ViewbtnPointdata.y = 231;
            Commentclass.ViewbtnPointdata.weith = 62;
            Commentclass.ViewbtnPointdata.heigth = 25;
        }


        bool Iscir_lode = false;
        int Lode_sumtimes = 0;
        int Lode_successfultimes = 0;
        private void cirLode_statusLabel_Click(object sender, EventArgs e)
        {
            if (Iscir_lode == false)
            {
                ((ToolStripStatusLabel)sender).BackColor = Color.Green;
                Iscir_lode = true;
            }
            else
            {
                ((ToolStripStatusLabel)sender).BackColor = Color.Yellow;
                Iscir_lode = false;
            }
        }
    }

    public static class DavidDebug
    {
        public static void showMsg(string str, StackTrace st)
        {
            StackFrame sf = st.GetFrame(0);
            Console.WriteLine("Daivd--" + str + ":" + " function--" + sf.GetMethod().Name + " Line--" + sf.GetFileLineNumber());
        }
    }


}
