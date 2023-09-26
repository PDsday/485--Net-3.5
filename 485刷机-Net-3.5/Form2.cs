using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using View;

namespace _485刷机_Net_3._5
{
    public partial class Form2 : Form
    {
        RecMsgDelegatefm1 myRcvMsgfm2;            //委托
        String serialPortName;                    //串口名
        #region 窗体位置
        static public int fmwindowX = 0;
        static public int fmwindowY = 0;
        static public bool first_load = true;
        #endregion
        #region 步骤定义
        bool Step_One = true;                                   //第一步的状态
        bool Step_Tow = false;                                  //第二步的状态
        bool Step_Thr = false;                                  //第三步的状态
        bool Step_Fou = false;                                  //第三步的状态
        #endregion
        #region 判断字符定义
        byte ZifuC = 0X43;                                      //C                              
        byte ZifuA = 0X41;                                      //A
        #endregion
        #region 主控ARM指令
        public byte[] Readyoder = new byte[7] { 0X01, 0XC0, 0X07, 0XAA, 0X55, 0X64, 0X80 };           //让单片机进入就绪状态 ，第一次返回相同的指令内容作为校验
        public byte[] Readyoder1 = new byte[7] { 0X01, 0XC0, 0X07, 0X55, 0XAA, 0X79, 0X8F };           //让单片机进入就绪状态 返回字符C
        int Resive_step = 0;
        bool orde = false;
        #endregion
        #region 充电板ARM指令
        public byte[] filedata_temp = new byte[128];            //承接BIN文件被按照128个字节分开后每次的数据 
        public byte[] Bohead = new byte[3] { 0x01, 0x01, 0XFE };                                        //帧头/报数据/反报数据
        public byte[] CdReadyoder = new byte[7] { 0X01, 0XC0, 0X07, 0XAA, 0X55, 0X64, 0X80 };           //让单片机进入就绪状态 ，第一次返回相同的指令内容作为校验
        public byte[] CdReadyoder1 = new byte[7] { 0X01, 0XC0, 0X07, 0X55, 0XAA, 0X79, 0X8F };           //让单片机进入就绪状态 返回字符C
        public byte[] Checkdata = new byte[7] { 0X01, 0XC0, 0X07, 0X00, 0X00, 0X9C, 0X65 };
        public byte[] Overoder = new byte[1] { 0x04 };          //告诉单片机刷机文件的发送完毕,让其跳转
        public byte[] CrcCheck = new byte[2];                   //CRC校验位的高位和低位
        public static byte[] filedata;                          //创建保存读取的文件数据的缓存空间
        public static byte[] CutoutFiles;                       //用于保存截取的bin文件
        byte sendacount = 0;
        int step_i = 0;                                         //发送的次数 
        double duty = 0;                                         //发送文件的进度值
        int pagenum = 0;                                        //计算需要发送多少次
        int remaind = 0;                                        //计算是否发送次数不为整数
        int cjremaind = 0;                                      //承接remaind
        bool cdorde = false;
        byte OkFlag = 0X06;                                     //接收正确返回指令
        byte ErrFlag = 0X15;                                     //接收错误返回的指令；错误重发三次
        byte Timer3Jump = 0;                                    //让主控板跳转到APP多发几次确保成功
        byte ErrTryCount = 0;                                   //错误重发，最多三次
        int TryCount = 0;                                      //错误重发，最多5次
        #endregion
        #region 共用变量
        public static byte[] SneCapre;                       //用于将发送的数据和接收的数据进行保存
        bool System_RS = true;                               //判断系统能否继续进行 
        bool Connect = true;                                 //应答式接收的运行标志位
        bool First_Run = true;                               //判断是否第一次点击下载
        bool loadtimecount = false;                          //计时标志位
        bool powerdisplay = true;                           //隐藏某些
        bool cdsingle = false;                               //充电板单板刷机模式是否启动
        static int i = 0;
        int loadtime = 0;
        int cdlinkcount = 0;
        bool cansend = true;
        public string backack = "";
        #endregion
        #region 线程定义
        Thread AckSend_Function = null;                      //应答式接发线程
        #endregion
        #region INI变量声明区
        public string str = Application.StartupPath + "\\ConnectString.ini";//该变量保存INI文件所在的具体物理位置
        public string IniFilesPath = Application.StartupPath + "\\ConnectString.ini";
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

        public Form2()
        {
            InitializeComponent();
            //databox.Hide();
            myRcvMsgfm2 = RecMsg;
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.cdviewbtn, "支持文件*.bin拖放");
            this.databox.ContextMenuStrip = this.contextMenuStrip2;

            
            #region 判断是否存在INI文件，如果存在就显示
            //此方法也可通过：str = System.AppDomain.CurrentDomain.BaseDirectory + @"ConnectString.ini";
            strOne = System.IO.Path.GetFileNameWithoutExtension(str);
            if (File.Exists(str))
            {
                string pathstr = ContentReader(strOne, "CDARMData_Source", "");
                int i = pathstr.LastIndexOf("\\");//获取字符串最后一个斜杠的位置
                string path = pathstr.Substring(0, i);//取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                if (Directory.Exists(path))
                {
                    cdtextfile.Text = pathstr;
                }
                else
                {
                    cdtextfile.Text = "";
                }
            }
            #endregion
        }

        #region 窗体加载函数
        private void Form2_Load(object sender, EventArgs e)
        {
            if (Commentclass.fmjump == 0x01)    //从窗口1切换过来
            {
                this.Location = new Point(Form1.windowX, Form1.windowY);
            }
            else if (Commentclass.fmjump == 0x03)    //从窗口1切换过来
            {
                this.Location = new Point(Form3.fm3windowX, Form3.fm3windowY);
            }
            if (Commentclass.fmjump == 0x07)    //从窗口1切换过来
            {
                this.Location = new Point(Form7.windowX, Form7.windowY);
            }
           // Console.WriteLine("david--充电板循环下载");
            if (Commentclass.WinDey)
            {

                //menuStrip1.Items.Remove(主控板刷机);
                menuStrip1.Items[2].Visible=false;
                this.Icon = IconSelect.GetFileIcon(Application.StartupPath + "\\icon\\wendey.ico");
                menuStrip1.Items[1].Text = "充电器ARM升级";
                menuStrip1.Items[0].Text = "DSP升级";
                menuStrip1.Items[3].Text = "主控板ARM升级";
                this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
                comboBox1.Enabled = false;
                label5.Enabled = false;
                toolStripStatusLabel1.Text = "充ARM升级";

                menuStrip1.Items[0].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[1].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[2].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[3].Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                string messageini = ReadINIFiles.ReadIniData("WarnMessage", "ChargeBordMessage", "None", IniFilesPath);
                string[] nametake3 = messageini.Split("\\r\\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                databox.Clear();
                foreach (string str in nametake3)
                {
                    databox.AppendText(str + "\r\n");
                }
                int size = 8;
                int x = label1.Location.X;
                int y = label1.Location.Y;
                label1.Location = new Point(x, y + size);

                x = cdserialbox.Location.X;
                y = cdserialbox.Location.Y;
                cdserialbox.Location = new Point(x, y + size);

                x = label2.Location.X;
                y = label2.Location.Y;
                label2.Location = new Point(x, y + size);


                x = cdpoundbox.Location.X;
                y = cdpoundbox.Location.Y;
                cdpoundbox.Location = new Point(x, y + size);


                x = cdloadbtn.Location.X;
                y = cdloadbtn.Location.Y;
                cdloadbtn.Location = new Point(x, y + size);

                label5.Hide();
                comboBox1.Hide();
                label3.Location = new Point(7, 230 + size);
                cdtextfile.Location = new Point(45, 225 + size);
                cdviewbtn.Location = new Point(328, 223 + size);
                label4.Location = new Point(7, 265 + size);
                cdloadprogressBar.Location = new Point(45, 260 + size);

                this.Size = new Size(410, 367);

                Console.WriteLine($"label1x = {label1.Location.X}.");
                Console.WriteLine($"label1y = {label1.Location.Y}.");
                Console.WriteLine($"heigth = {databox.Height}.");
                Console.WriteLine($"weigth = {databox.Width}.");
                Console.WriteLine($"x = {databox.Location.X}.");
                Console.WriteLine($"y = {databox.Location.Y}.");

                //databox.Text = nametake3[0] + "\r\n" + nametake3[1] + "\r\n";
                cdpoundbox.Enabled = false;
                if (databox.TextLength >= 5)
                {
                    databox.Select(18, 10);
                    databox.SelectionColor = Color.Red;
                }

               
            }
            else
            {


                this.Icon = IconSelect.GetFileIcon(Application.StartupPath + "\\icon\\wendey.ico");
                this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
                toolStripStatusLabel1.Text = "充ARM升级";
                menuStrip1.Items[0].Text = "DSP升级";
                menuStrip1.Items[1].Text = "充ARM升级";
                menuStrip1.Items[2].Text = "主ARM升级(485)";
                menuStrip1.Items[3].Text = "主ARM升级(Eth)";
                menuStrip1.Items[0].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[1].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[2].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                menuStrip1.Items[3].Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));

                int size = 8;
                int x = label1.Location.X;
                int y = label1.Location.Y;
                label1.Location = new Point(x, y + size);

                x = cdserialbox.Location.X;
                y = cdserialbox.Location.Y;
                cdserialbox.Location = new Point(x, y + size);

                x = label2.Location.X;
                y = label2.Location.Y;
                label2.Location = new Point(x, y + size);

                x = cdpoundbox.Location.X;
                y = cdpoundbox.Location.Y;
                cdpoundbox.Location = new Point(x, y + size);

                x = cdloadbtn.Location.X;
                y = cdloadbtn.Location.Y;
                cdloadbtn.Location = new Point(x, y + size);

                label5.Hide();
                comboBox1.Hide();
                label3.Location = new Point(7, 230 + size);
                cdtextfile.Location = new Point(45, 225 + size);
                cdviewbtn.Location = new Point(328, 223 + size);
                label4.Location = new Point(7, 265 + size);
                cdloadprogressBar.Location = new Point(45, 260 + size);

                this.Size = new Size(410, 367);
            }
            databox.Location = new Point(9, 34);
            databox.Size = new Size(386, 140);

            //获取信息
            ////端口lable 
            //Commentclass.PortlablePointdata.x = label1.Location.X;
            //Commentclass.PortlablePointdata.y = label1.Location.Y;
            //Commentclass.PortlablePointdata.weith = label1.Width;
            //Commentclass.PortlablePointdata.heigth = label1.Height;

            //Console.WriteLine($"Commentclass.PortlablePointdata.x  = {Commentclass.PortlablePointdata.x };");
            //Console.WriteLine($"Commentclass.PortlablePointdata.y  = {Commentclass.PortlablePointdata.y };");
            //Console.WriteLine($"Commentclass.PortlablePointdata.weith  = {Commentclass.PortlablePointdata.weith };");
            //Console.WriteLine($"Commentclass.PortlablePointdata.heigth  = {Commentclass.PortlablePointdata.heigth };");

            ////端口combox
            //Commentclass.PortcomboxPointdata.x = cdserialbox.Location.X;
            //Commentclass.PortcomboxPointdata.y = cdserialbox.Location.Y;
            //Commentclass.PortcomboxPointdata.weith = cdserialbox.Width;
            //Commentclass.PortcomboxPointdata.heigth = cdserialbox.Height;

            //Console.WriteLine($"Commentclass.PortcomboxPointdata.x  = {Commentclass.PortcomboxPointdata.x };");
            //Console.WriteLine($"Commentclass.PortcomboxPointdata.y  = {Commentclass.PortcomboxPointdata.y };");
            //Console.WriteLine($"Commentclass.PortcomboxPointdata.weith  = {Commentclass.PortcomboxPointdata.weith };");
            //Console.WriteLine($"Commentclass.PortcomboxPointdata.heigth  = {Commentclass.PortcomboxPointdata.heigth };");

            ////波特率lable
            //Commentclass.BoundlablePointdata.x = label2.Location.X;
            //Commentclass.BoundlablePointdata.y = label2.Location.Y;
            //Commentclass.BoundlablePointdata.weith = label2.Width;
            //Commentclass.BoundlablePointdata.heigth = label2.Height;

            //Console.WriteLine($"Commentclass.BoundlablePointdata.x  = {Commentclass.BoundlablePointdata.x };");
            //Console.WriteLine($"Commentclass.BoundlablePointdata.y  = {Commentclass.BoundlablePointdata.y };");
            //Console.WriteLine($"Commentclass.BoundlablePointdata.weith  = {Commentclass.BoundlablePointdata.weith };");
            //Console.WriteLine($"Commentclass.BoundlablePointdata.heigth  = {Commentclass.BoundlablePointdata.heigth };");

            ////波特率combox
            //Commentclass.BoundcomboxPointdata.x = cdpoundbox.Location.X;
            //Commentclass.BoundcomboxPointdata.y = cdpoundbox.Location.Y;
            //Commentclass.BoundcomboxPointdata.weith = cdpoundbox.Width;
            //Commentclass.BoundcomboxPointdata.heigth = cdpoundbox.Height;

            //Console.WriteLine($"Commentclass.BoundcomboxPointdata.x  = {Commentclass.BoundcomboxPointdata.x };");
            //Console.WriteLine($"Commentclass.BoundcomboxPointdata.y  = {Commentclass.BoundcomboxPointdata.y };");
            //Console.WriteLine($"Commentclass.BoundcomboxPointdata.weith  = {Commentclass.BoundcomboxPointdata.weith };");
            //Console.WriteLine($"Commentclass.BoundcomboxPointdata.heigth  = {Commentclass.BoundcomboxPointdata.heigth };");

            ////文件lable
            //Commentclass.FilelablePointdata.x = label3.Location.X;
            //Commentclass.FilelablePointdata.y = label3.Location.Y;
            //Commentclass.FilelablePointdata.weith = label3.Width;
            //Commentclass.FilelablePointdata.heigth = label3.Height;

            //Console.WriteLine($"Commentclass.FilelablePointdata.x  = {Commentclass.FilelablePointdata.x };");
            //Console.WriteLine($"Commentclass.FilelablePointdata.y  = {Commentclass.FilelablePointdata.y };");
            //Console.WriteLine($"Commentclass.FilelablePointdata.weith  = {Commentclass.FilelablePointdata.weith };");
            //Console.WriteLine($"Commentclass.FilelablePointdata.heigth  = {Commentclass.FilelablePointdata.heigth };");

            ////文件textbox
            //Commentclass.FiletextboxPointdata.x = cdtextfile.Location.X;
            //Commentclass.FiletextboxPointdata.y = cdtextfile.Location.Y;
            //Commentclass.FiletextboxPointdata.weith = cdtextfile.Width;
            //Commentclass.FiletextboxPointdata.heigth = cdtextfile.Height;

            //Console.WriteLine($"Commentclass.FiletextboxPointdata.x  = {Commentclass.FiletextboxPointdata.x };");
            //Console.WriteLine($"Commentclass.FiletextboxPointdata.y  = {Commentclass.FiletextboxPointdata.y };");
            //Console.WriteLine($"Commentclass.FiletextboxPointdata.weith  = {Commentclass.FiletextboxPointdata.weith };");
            //Console.WriteLine($"Commentclass.FiletextboxPointdata.heigth  = {Commentclass.FiletextboxPointdata.heigth };");

            ////进度lable
            //Commentclass.PrglablePointdata.x = label4.Location.X;
            //Commentclass.PrglablePointdata.y = label4.Location.Y;
            //Commentclass.PrglablePointdata.weith = label4.Width;
            //Commentclass.PrglablePointdata.heigth = label4.Height;

            //Console.WriteLine($"Commentclass.PrglablePointdata.x  = {Commentclass.PrglablePointdata.x };");
            //Console.WriteLine($"Commentclass.PrglablePointdata.y  = {Commentclass.PrglablePointdata.y };");
            //Console.WriteLine($"Commentclass.PrglablePointdata.weith  = {Commentclass.PrglablePointdata.weith };");
            //Console.WriteLine($"Commentclass.PrglablePointdata.heigth  = {Commentclass.PrglablePointdata.heigth };");

            ////进度pragre
            //Commentclass.PrgformPointdata.x = cdloadprogressBar.Location.X;
            //Commentclass.PrgformPointdata.y = cdloadprogressBar.Location.Y;
            //Commentclass.PrgformPointdata.weith = cdloadprogressBar.Width;
            //Commentclass.PrgformPointdata.heigth = cdloadprogressBar.Height;

            //Console.WriteLine($"Commentclass.PrgformPointdata.x  = {Commentclass.PrgformPointdata.x };");
            //Console.WriteLine($"Commentclass.PrgformPointdata.y  = {Commentclass.PrgformPointdata.y };");
            //Console.WriteLine($"Commentclass.PrgformPointdata.weith  = {Commentclass.PrgformPointdata.weith };");
            //Console.WriteLine($"Commentclass.PrgformPointdata.heigth  = {Commentclass.PrgformPointdata.heigth };");


            ////下载
            //Commentclass.DownbtnPointdata.x = cdloadbtn.Location.X;
            //Commentclass.DownbtnPointdata.y = cdloadbtn.Location.Y;
            //Commentclass.DownbtnPointdata.weith = cdloadbtn.Width;
            //Commentclass.DownbtnPointdata.heigth = cdloadbtn.Height;


            //Console.WriteLine($"Commentclass.DownbtnPointdata.x  = {Commentclass.DownbtnPointdata.x };");
            //Console.WriteLine($"Commentclass.DownbtnPointdata.y  = {Commentclass.DownbtnPointdata.y };");
            //Console.WriteLine($"Commentclass.DownbtnPointdata.weith  = {Commentclass.DownbtnPointdata.weith };");
            //Console.WriteLine($"Commentclass.DownbtnPointdata.heigth  = {Commentclass.DownbtnPointdata.heigth };");

            ////浏览
            //Commentclass.ViewbtnPointdata.x = cdviewbtn.Location.X;
            //Commentclass.ViewbtnPointdata.y = cdviewbtn.Location.Y;
            //Commentclass.ViewbtnPointdata.weith = cdviewbtn.Width;
            //Commentclass.ViewbtnPointdata.heigth = cdviewbtn.Height;

            //Console.WriteLine($"Commentclass.ViewbtnPointdata.x  = {Commentclass.ViewbtnPointdata.x };");
            //Console.WriteLine($"Commentclass.ViewbtnPointdata.y  = {Commentclass.ViewbtnPointdata.y };");
            //Console.WriteLine($"Commentclass.ViewbtnPointdata.weith  = {Commentclass.ViewbtnPointdata.weith };");
            //Console.WriteLine($"Commentclass.ViewbtnPointdata.heigth  = {Commentclass.ViewbtnPointdata.heigth };");

            toolStripStatusLabel3.Text = Commentclass.Version;
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();//获取电脑上可用的串口号
            cdserialbox.Items.AddRange(ports);      //给端口号选择窗口添加数据
            cdserialbox.SelectedIndex = cdserialbox.Items.Count > 0 ? 0 : -1;  //如果里面有数据显示第零个
            first_load = false;

            this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);

            //读取ini文件,获取充电板ARM的 BootloaderHeadFlag 、CodeStartAddr、RedirectionAddr
            // var ReadIniTempFlag1 = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordBootloaderHeadFlagINI", "None", IniFilesPath);
            var ReadIniTempFlag1 = Commentclass.SettingMessageList[3];
            Console.WriteLine($"CDReadIniTemp = {ReadIniTempFlag1}.");
            string[] nametake1 = ReadIniTempFlag1.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 16; i++)
            {
                Commentclass.ChargebordBootloaderHeadFlag[i] = Convert.ToByte(nametake1[i], 16);
                Console.WriteLine($"CDBootloaderHeadFlag = {Commentclass.ChargebordBootloaderHeadFlag[i].ToString("x")}.");
            }
            //获取CodeStartAddr
            //var ReadIniTempStartAddr1 = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordCodeStartAddrINI", "None", IniFilesPath);
            var ReadIniTempStartAddr1 = Commentclass.SettingMessageList[4];
            Commentclass.ChargebordCodeStartAddr = Convert.ToUInt64(ReadIniTempStartAddr1, 16);
            Console.WriteLine($"CDCodeStartAddr = {"0X" + Commentclass.ChargebordCodeStartAddr.ToString("x")}.");
            //获取RedirectionAddr
            //var ReadIniTempTionAddr1 = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordRedirectionAddrINI", "None", IniFilesPath);
            var ReadIniTempTionAddr1 = Commentclass.SettingMessageList[5];
            Commentclass.ChargebordRedirectionAddr = Convert.ToUInt64(ReadIniTempTionAddr1, 16);
            Console.WriteLine($"CDRedirectionAddr = {"0X" + Commentclass.ChargebordRedirectionAddr.ToString("x")}.");
            Commentclass.ChargebordIniRecordBootloaderSize = Commentclass.ChargebordRedirectionAddr - Commentclass.ChargebordCodeStartAddr;

            string maximalsize = ReadINIFiles.ReadIniData("ChargeBordCodeSizeLimti", "MaximalSize", "None", IniFilesPath);
            string minimalsize = ReadINIFiles.ReadIniData("ChargeBordCodeSizeLimti", "MinimalSize", "None", IniFilesPath);
            string[] para = maximalsize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] para1 = minimalsize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Commentclass.ChargeBordMaximalSize = Convert.ToUInt64(para[0]) * Convert.ToUInt64(para[1]);
            Commentclass.ChargeBordMinimalSize = Convert.ToUInt64(para1[0]) * Convert.ToUInt64(para1[1]);

            Console.WriteLine($"ChargeBordMaximalSize = {Commentclass.ChargeBordMaximalSize}.");
            Console.WriteLine($"ChargeBordMinimalSize = {Commentclass.ChargeBordMinimalSize }.");

            timer5.Start();
        }
        #endregion

        #region 委托
        private void RecMsg(string str)
        {
            this.databox.AppendText(str + "\r\n");
        }
        #endregion

        #region 点击下载按钮
        private void cdloadbtn_Click(object sender, EventArgs e)
        {
            Commentclass.CommentSpecialChargeNumCheck = Convert.ToUInt16(Commentclass.SettingMessageList[9]);
            Console.WriteLine($" Commentclass.CommentSpecialChargeNumCheck = { Commentclass.CommentSpecialChargeNumCheck}.");
            Commentclass.CommentHeadCheckNumLength = Commentclass.CommentSpecialChargeNumCheck;
            First_Run = false;                                                  //上位机不是首次打开
            disable_function();
            databox.Show();
            databox.Clear();
            toolStripStatusLabel1.Text = "等待校验中.......";
            Step_Thr = true;            //用于判断充电板是否直接从IAP启动
            load_file_onthepath();
        }
        #endregion

        #region 默认路径读取文件
        private void load_file_onthepath()
        {
            try
            {
                FileStream fs = new FileStream(cdtextfile.Text, FileMode.Open, FileAccess.Read);
                byte[] arrfileSend = new byte[1024 * 1024 * 8];               //创建8M的缓存
                int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                byte[] arrfile = new byte[length];
                fs.Close();    //关闭文件流，释放文件
                Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);           // 将arrfileSend复制到arrfile之中
                filedata = new byte[length];
                Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);          // 将arrfileSend复制到arrfile之中
                Console.WriteLine($"filedata = {filedata.Length}.");

               
                //加入文件大小判断
                if (filedata.Length > (int)Commentclass.ChargeBordMaximalSize || filedata.Length < (int)Commentclass.ChargeBordMinimalSize)
                {
                    enable_function();
                    MessageBoxMidle.Show("导入文件大小错误，非充电器ARM刷机文件！\r\n" + "请重新导入。", "文件错误");
                    return;
                }
                //提示
                DialogResult messdr = MessageBoxMidle.Show(this, "请先打手动开关进入手动模式！\r\n是否已完成该操作？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (messdr == DialogResult.No)
                {
                    enable_function();
                    return;
                }

                //执行截取文件的操作
                backack = ReadFiles.FragmentCode(Commentclass.ChargebordRedirectionAddr, Commentclass.ChargebordCodeStartAddr, (UInt64)filedata.Length,
                                Commentclass.ChargebordIniRecordBootloaderSize, Commentclass.ChargebordBootloaderHeadFlag, filedata, out CutoutFiles);
                if (backack == "ok")
                {
                    filedata = new byte[CutoutFiles.Length];
                    Buffer.BlockCopy(CutoutFiles, 0, filedata, 0, CutoutFiles.Length);
                    length = filedata.Length;//重新获取文件的长度

                    ////导出Bin文件(验证用)
                    //FileStream fstemp = new FileStream("充电板截取文件.bin", FileMode.OpenOrCreate);
                    //BinaryWriter binWriter = new BinaryWriter(fstemp);
                    //binWriter.Write(filedata, 0, filedata.Length);
                    //binWriter.Close();
                    //fstemp.Close();
                    //return;

                    step_i = 0;              //初始化
                    duty = 0;                //初始化
                    pagenum = length / 128;  //初始化  以128个字节为一个数据包发送数据
                    remaind = length % 128;  //初始化
                    cjremaind = remaind;
                    Console.WriteLine($"remaind = {remaind}.");
                    cdloadprogressBar.Maximum = pagenum - 1; //进度条的最大值初始化
                    toolStripStatusLabel1.Text = "文件已导入，请确认！";
                    Click_Start();
                }
                else
                {
                    MessageBoxMidle.Show(this, backack, "错误");
                    enable_function();
                }
            }
            catch (Exception err)
            {
                enable_function();
                MessageBoxMidle.Show(this, "打开文件失败！", "错误");
            }

        }
        #endregion

        #region 点击开始函数
        private void Click_Start()
        {
            if (!First_Run)
            {
                Systemtemp_int();  //如果不是第一次则要对变量初始化
                toolStripStatusLabel1.Text = "参数初始化中...";
                if (!powerdisplay)
                    databox.AppendText(Commentclass.ChargeStateMessageList[0] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
            }
            if (serialPort1.IsOpen)
            {
                if (!cdsingle)
                {
                    timer1.Start();  //等待确认单片机是否处于IAP状态
                }
                else
                {
                    timer2.Start();
                }
            }
            else
            {
                if (Open_Serial())  //打开串口
                {
                    if (!cdsingle)
                    {
                        timer1.Start();  //等待确认单片机是否处于IAP状态
                    }
                    else
                    {
                        timer2.Start();
                    }
                }
                else
                {
                    return;
                }
               
            }
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
                    cdserialbox.Items.Clear();       //清除combox1里面的数据
                    cdserialbox.Items.AddRange(ports);   //给combox1添加新的数据
                    cdserialbox.SelectedIndex = cdserialbox.Items.Count > 0 ? 0 : -1;      //显示获取第一个串口 
                }

                else if (m.WParam.ToInt32() == 0x8000)
                {//usb串口连接上
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    cdserialbox.Items.Clear();
                    cdserialbox.Items.AddRange(ports);
                    cdserialbox.SelectedIndex = cdserialbox.Items.Count > 0 ? 0 : -1;  //获取显示第一个串口号
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
            if (string.IsNullOrEmpty(cdtextfile.Text))
            {
                MessageBoxMidle.Show(this, "请选择您要下载的Bin文件！", "发送文件提示");
                toolStripStatusLabel1.Text = "请选择需要下载的BIN文件！";
                enable_function();
                return false;
            }
            try
            {
                serialPort1.PortName = cdserialbox.Text;    //获取要打开的串口号
                serialPortName = cdserialbox.Text;          //将获取到的串口号存储起来
                serialPort1.BaudRate = int.Parse(cdpoundbox.Text);//获取波特率
                serialPort1.DataBits = int.Parse("8");      //设置数据位
                //===============设置停止位1================================================
                serialPort1.StopBits = StopBits.One;
                //=======================无奇偶校验=========================================
                serialPort1.Parity = Parity.None;
                serialPort1.Open();                         //打开串口
                return true;
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开失败,请检查串口！", "提示！");    //对话框显示打开失败
                toolStripStatusLabel1.Text = "串口打开失败！请检查串口！";
                enable_function();
                return false;
            }

        }
        #endregion

        #region 变量初始化函数
        public void Systemtemp_int()
        {
            Resive_step = 0;
            orde = false;
            sendacount = 0;
            step_i = 0;
            duty = 0;
            cjremaind = remaind;
            cdorde = false;
            Bohead[1] = 0x01;
            Bohead[2] = 0xFE;
            Connect = true;
            System_RS = true;
            cansend = true;
            if (!cdsingle)
            {
                Step_One = true;                                   //第一步的状态
                Step_Tow = false;                                  //第二步的状态
                Step_Thr = false;                                  //第三步的状态
                Step_Fou = false;                                  //第三步的状态
            }
            else
            {
                Step_One = false;                                   //第一步的状态
                Step_Tow = false;                                  //第二步的状态
                Step_Thr = true;                                  //第三步的状态
                Step_Fou = false;                                  //第三步的状态
            }
            Timer3Jump = 0;
            loadtimecount = false;
            loadtime = 0;
            LinkCount = 0;
            cdlinkcount = 0;
            TryCount = 0;
            if (!cdsingle)
            {
                cdpoundbox.Text = "115200";
            }
            else
            {
                cdpoundbox.Text = "19200";
            }
        }
        #endregion

        #region 定时器1函数：实现主控ARM的IAP跳转以及透传
        int LinkCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (!Step_One && Step_Tow)
            {
                if (sendacount < 0x03)
                {
                    sendacount++;
                    serialPort1.Write("A");  //发送三次A让主控进入透传状态
                    if (sendacount == 1)
                    {
                        if (!powerdisplay)
                            databox.AppendText(Commentclass.ChargeStateMessageList[3] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                    }
                }
                else
                {
                    Step_Tow = false;
                    Step_Thr = true;
                    timer1.Stop();
                    Debug.WriteLine("主控已进入透传状态");            //the likes printf
                    if (!powerdisplay)
                    {
                        databox.AppendText(Commentclass.ChargeStateMessageList[4] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        databox.AppendText(Commentclass.ChargeStateMessageList[5] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                    }
                    toolStripStatusLabel1.Text = "主控进入透传状态...";
                    timer2.Start();
                }
            }
            else//主控不处于IAP阶段，让主控进入IAP
            {
                if (!orde && Resive_step == 0)
                {
                    serialPort1.BaudRate = int.Parse("19200");  //切换波特率
                    cdpoundbox.Text = "19200";

                    Usart_Send_Ncrc(Readyoder);                          //主控ARM跳转IAP第一个命令，返回相同值
                    orde = true;
                    Debug.WriteLine("让主控进入IAP!");                   //the likes printf
                    toolStripStatusLabel1.Text = "等待主控进入IAP...";
                    if (!powerdisplay)
                        databox.AppendText(Commentclass.ChargeStateMessageList[1] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                }
                else if (orde && Resive_step == 1)
                {
                    Usart_Send_Ncrc(Readyoder1);
                    Thread.Sleep(10);

                    serialPort1.BaudRate = int.Parse("115200");  //切换波特率
                    cdpoundbox.Text = "115200";

                    if (!powerdisplay)//主控ARM跳转IAP第一个命令，返回相同值
                        databox.AppendText(Commentclass.ChargeStateMessageList[11] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                    Debug.WriteLine("TRY1!");                   //the likes printf
                    orde = false;
                }
                else if (orde && Resive_step == 0)
                {
                  
                    orde = true;
                    LinkCount = LinkCount + 1;
                    if (LinkCount <= 3)
                    {
                        Usart_Send_Ncrc(Readyoder);                          //主控ARM跳转IAP第一个命令，返回相同值
                    }
                    else if (LinkCount == 4)
                    {
                        serialPort1.BaudRate = int.Parse("115200");  //切换波特率
                        cdpoundbox.Text = "115200";
                        Thread.Sleep(10);
                    }
                    else if (4 < LinkCount && LinkCount <= 10)
                    {
                        Usart_Send_Ncrc(Readyoder);                          //主控ARM跳转IAP第一个命令，返回相同值
                    }
                    else
                    {
                        LinkCount = 0;
                        timer1.Stop();
                        serialPort1.Close();
                        databox.AppendText(Commentclass.ChargeStateMessageList[10] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        enable_function();
                        MessageBoxMidle.Show(this, "无响应！请重新点击下载。", "提示!!");               //弹出提示框
                    }
                    Debug.WriteLine("TRY!");                   //the likes printf
                }


            }
        }
        #endregion

        #region 串口接收函数
        private void serialPort1_DataReceived_1(object sender, SerialDataReceivedEventArgs e)
        {
            int len = serialPort1.BytesToRead;       //获取可以读取的字节数
            if (len == 0) return;                    //为啥有时候为0

            byte[] Resive_data = new byte[len];      //创建缓存的数据数组
            serialPort1.Read(Resive_data, 0, len);   //把数据读取到buff数组
            Invoke((new Action(() =>                 //c#3.0以后代替委托的新方法
            {
                if (powerdisplay)
                {
                    Invoke(myRcvMsgfm2, "接收：" + StartsCRC.zftring(Resive_data));
                }
                #region 第一步：判断主控ARM是否处于IAP模式 
                if (Step_One && len == 1 && Resive_step == 0 && Resive_data[0] == 0x43)
                {
                    Step_One = false;                                  //第一步结束，跳转到让充电板处于IAP状态
                    Step_Tow = true;
                    Debug.WriteLine("主控处于IAP模式");            //the likes printf
                    toolStripStatusLabel1.Text = "主控板ARM处于IAP模式...";
                    if (!powerdisplay)
                        databox.AppendText(Commentclass.ChargeStateMessageList[12] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                }
                else if (Step_One && len == 1 && Resive_data[0] == ZifuA && Resive_step == 0)
                {
                    timer1.Stop();
                    Step_One = false;
                    Step_Tow = false;
                    Step_Thr = false;
                    Step_Fou = true;
                    Open_Threading();
                    Debug.WriteLine("充电板已经进入IAP");            //the likes printf
                    toolStripStatusLabel1.Text = "充电器IAP启动.......";
                }
                else if (Step_Thr && len == 1 && Resive_data[0] == ZifuA && Resive_step == 0)
                {
                    Step_Thr = false;
                    Step_Fou = true;
                    timer2.Stop();
                    Open_Threading();
                    Debug.WriteLine("充电板已经进入IAP");            //the likes printf
                    toolStripStatusLabel1.Text = "充电器IAP启动.......";
                }
                else if (Step_One)
                {
                    Resive_step = Resive_step + 1;
                    if (Resive_step == 1)
                    {
                        if (Compare_Data(SneCapre, Resive_data))
                        {
                            System_RS = true;
                            Debug.WriteLine("比较相同");            //the likes printf
                        }
                        else
                        {
                            System_RS = true;
                            Resive_step = 0;
                            Debug.WriteLine("第一次响应错诶！");            //the likes printf
                        }
                    }
                    else if (Resive_step == 2)
                    {
                        if (Resive_data[0] == ZifuC)
                        {
                            System_RS = true;
                            Resive_step = 0;
                            Step_One = false;                                   //第一步结束，跳转到让充电板处于IAP状态
                            Step_Tow = true;
                            Debug.WriteLine("主控已经进入IAP");            //the likes printf
                            toolStripStatusLabel1.Text = "主控板ARM处于IAP模式.......";
                        }
                        else
                        {
                            Resive_step = 1;
                            Debug.WriteLine("第一1次响应错诶！");            //the likes printf
                        }
                    }
                }
                #endregion
                #region stepfour
                else if (Step_Thr)
                {
                    Resive_step = Resive_step + 1;
                    if (Resive_step == 1)
                    {
                        if (Compare_Data(Checkdata, Resive_data))
                        {
                            System_RS = true;
                            cdorde = true;
                            Debug.WriteLine("CD比较相同");            //the likes printf
                        }
                        else
                        {
                            Resive_step = 0;
                            Debug.WriteLine("CD第一次响应错诶！");            //the likes printf
                        }
                    }
                    else if (Resive_step == 2)
                    {
                        if (Resive_data[0] == ZifuA)
                        {
                            timer2.Stop();
                            System_RS = true;
                            Step_Thr = false;                                   //第一步结束，跳转到让充电板处于IAP状态
                            Step_Fou = true;
                            Open_Threading();                                   //开线程
                            Debug.WriteLine("充电板已经进入IAP");            //the likes printf
                        }
                        else
                        {
                            Resive_step = 1;
                            Debug.WriteLine("CD第一1次响应错！");            //the likes printf
                        }

                    }
                }
                #endregion
                #region diwubu 
                else if (Step_Fou)
                {
                    if (Resive_data[0] == OkFlag)
                    {
                        System_RS = true;
                        Debug.WriteLine("传输正确！");               //the likes printf
                        if (!powerdisplay)
                            databox.AppendText(Commentclass.ChargeStateMessageList[6] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        timer8.Stop();
                    }
                    else if (Resive_data[0] == ErrFlag)
                    {
                        //错误重发操作
                        if (ErrTryCount < 3)
                        {
                            ErrTryCount++;
                            databox.AppendText(Commentclass.ChargeStateMessageList[7] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                            toolStripStatusLabel1.Text = "传输错误！尝试重发：" + ErrTryCount.ToString();
                            timer4.Start();
                        }
                        else
                        {
                            Connect = false;
                            timer4.Stop();      //定时器溢出区判断校验是否通过
                            ErrTryCount = 0;
                            timer7.Start();
                        }
                    }
                }
                #endregion

            })));
        }
        #endregion

        #region 发送和接收的数据做比较
        public static bool Compare_Data(byte[] Send_vc, byte[] Rsive_vc)
        {
            try
            {
                if (Enumerable.SequenceEqual(Send_vc, Rsive_vc))  //按相同序列检查两数组的元素是否相同
                {
                    return true;  //相同返回true
                }
                else
                {
                    return false; //不同返回false
                }
            }
            catch
            {
                return false; //不同返回false
            }
        }
        #endregion

        #region 串口发送函数（CRC校验）
        private void Usart_Send(byte[] data)
        {
            try  //星辰CRC校验
            {
                if (cansend)
                {
                    int length = data.Length;                                           //需要发送数据的长度
                    UInt16 CRC_Value = StartsCRC.CRC16(data, length);
                    CrcCheck[0] = Convert.ToByte(CRC_Value / 256);                      //将CRC校验值写入发送的数据
                    CrcCheck[1] = Convert.ToByte(CRC_Value % 256);
                    //======在这里加上帧头、报数据、反报数据、校验位==============
                    byte[] alldata = new byte[Bohead.Length + data.Length + CrcCheck.Length];
                    Buffer.BlockCopy(Bohead, 0, alldata, 0, Bohead.Length);                                     //将Bohead放到alldata之中
                    Buffer.BlockCopy(data, 0, alldata, Bohead.Length, data.Length);                             //将data放到alldata之中
                    Buffer.BlockCopy(CrcCheck, 0, alldata, Bohead.Length + data.Length, CrcCheck.Length);       //将CrcCheck放到alldata之中
                    SneCapre = new byte[alldata.Length];
                    Buffer.BlockCopy(alldata, 0, SneCapre, 0, alldata.Length);      // 将data复制到SneCapre之中
                    if (powerdisplay)
                    {
                        Invoke(myRcvMsgfm2, "发送：" + StartsCRC.zftring(alldata));
                    }
                    serialPort1.Write(alldata, 0, alldata.Length);                  //串口发送数据
                                                                                    //在这里进行报数据和反报数据的处理
                    Bohead[1] = (byte)(Bohead[1] + 1);                              //报数据
                    Bohead[2] = (byte)(0xff - Bohead[1]);                           //反报数据
                    System_RS = false;                                              //等待校验成功
                    timer8.Start();
                }
            }
            catch
            {
                Connect = false;
                cansend = false;
                enable_function();
                MessageBoxMidle.Show(this, "检查通信线是否断开,\r\n" , "串口发送错误");            
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
                    serialPort1.Write(data, 0, data.Length);                //串口发送数据
                    SneCapre = new byte[data.Length];
                    Buffer.BlockCopy(data, 0, SneCapre, 0, data.Length);    // 将data复制到SneCapre之中
                    if (powerdisplay)
                    {
                        Invoke(myRcvMsgfm2, "发送：" + StartsCRC.zftring(data));
                    }
                    /*  System_RS = false;   */
                }//等待校验成功
            }
            catch
            {
                cansend = false;
                enable_function();
                MessageBoxMidle.Show(this, "串口发送错误！", "错误");
            }

        }
        #endregion

        #region 窗口关闭事件
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            if (cdloadbtn.Enabled == false)
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
                timer5.Stop();
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
                Application.Exit();
                //Console.WriteLine($"哈哈退出.");
            }
           
        }
        #endregion

        #region 打开Bin文件并保存到建立的数组中
        private void cdviewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (string.IsNullOrEmpty(cdtextfile.Text))
                {
                    ofd.InitialDirectory = ("D:\\");
                }
                else
                {
                    int i = cdtextfile.Text.LastIndexOf("\\");//获取字符串最后一个斜杠的位置
                    string path = cdtextfile.Text.Substring(0, i);//取当前目录的字符串第一个字符到最后一个斜杠所在位置。;
                    ofd.InitialDirectory = (path);
                }
                ofd.Filter = "|*.bin";//设置当前文件名筛选器字符串
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.cdtextfile.Text = ofd.FileName;
                    WritePrivateProfileString(strOne, "CDARMData_Source", cdtextfile.Text, str);
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
            //try
            //{
            //    FileStream fs = new FileStream(cdtextfile.Text, FileMode.Open);
            //    byte[] arrfileSend = new byte[1024 * 1024 * 8];               //创建8M的缓存
            //    int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
            //    byte[] arrfile = new byte[length];
            //    fs.Close();    //关闭文件流，释放文件
            //    Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);           // 将arrfileSend复制到arrfile之中
            //    filedata = new byte[length];
            //    Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);          // 将arrfileSend复制到arrfile之中
            //    Console.WriteLine($"filedata = {filedata.Length}.");
            //    //加入文件大小判断
            //    if (filedata.Length > 92160)
            //    {
            //        string filename = System.IO.Path.GetFileName(cdtextfile.Text);
            //        DialogResult messdr = MessageBoxMidle.Show(this, "当前操对象:\r\n  充电板ARM！\r\n\r\nBin文件名称：\r\n  " + filename + "\r\n\r\n请确认！", "BIN文件异常提示！", MessageBoxButtons.YesNo);
            //        if (messdr == DialogResult.No)
            //        {
            //            toolStripStatusLabel1.Text = "文件操作已中止，请重新导入！";
            //            return;
            //        }
            //    }
            //    step_i = 0;              //初始化
            //    duty = 0;                //初始化
            //    pagenum = length / 128;  //初始化  以128个字节为一个数据包发送数据
            //    remaind = length % 128;  //初始化
            //    cjremaind = remaind;
            //    Console.WriteLine($"remaind = {remaind}.");
            //    cdloadprogressBar.Maximum = pagenum - 1; //进度条的最大值初始化
            //    toolStripStatusLabel1.Text = "文件已导入，请确认！";
            //}
            //catch (Exception err)
            //{
            //    MessageBoxMidle.Show(this, "打开文件失败！" + err.ToString(), "错误");
            //}
        }
        #endregion

        #region 定时器2函数:实现充电板跳转到IAP
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Step_Thr)
            {
                if (!cdorde && cdlinkcount < 6)
                {
                    Usart_Send_Ncrc(CdReadyoder);                          //主控ARM跳转IAP第一个命令，返回相同值
                    cdlinkcount = cdlinkcount + 1;
                    Debug.WriteLine("让充电板进入IAP!");                   //the likes printf
                    toolStripStatusLabel1.Text = "等待充电器跳转IAP.......";
                }
                else if (cdorde && Resive_step == 1)
                {
                    if (TryCount < 5)
                    {
                        TryCount = TryCount + 1;
                        Usart_Send_Ncrc(CdReadyoder1);                          //主控ARM跳转IAP第一个命令，返回相同值
                        timer2.Interval = 3000;
                        Debug.WriteLine("让充电板进入IAP2!");
                    }
                    else
                    {
                        timer2.Stop();
                        serialPort1.Close();
                        databox.AppendText(Commentclass.ChargeStateMessageList[10] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                        enable_function();
                        MessageBoxMidle.Show(this, "无响应！请重新点击下载。", "提示!!");               //弹出提示框
                    }
                }
                else
                {

                    cdlinkcount = 0;
                    timer2.Stop();
                    serialPort1.Close();
                    databox.AppendText(Commentclass.ChargeStateMessageList[10] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                    enable_function();
                    MessageBoxMidle.Show(this, "无响应！请重新点击下载。", "提示!!");               //弹出提示框
                }

            }
            else if (Step_Fou)
            {
                timer2.Stop();
                toolStripStatusLabel1.Text = "充电器已跳转IAP...";
                Debug.WriteLine("充电板已经进入IAP");            //the likes printf
                Debug.WriteLine("定时器2已关闭");            //the likes printf        
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
            toolStripStatusLabel1.Text = "传输线程开启.......";
            loadtimecount = true;
            menuStrip1.Enabled = false;
        }
        #endregion

        #region 应答式发送消息的函数（放在线程里面）
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
                                Console.WriteLine($"filedata = {filedata.Length}.");

                                Usart_Send(filedata_temp);              //发送128个字节数据

                                cdloadprogressBar.Value = step_i;
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
                                    filedata_temp[cjremaind] = 0xFF;
                                }

                                Usart_Send(filedata_temp);              //发送剩余的数据

                                duty = (((double)(step_i) / (double)pagenum) * 100);
                                toolStripStatusLabel1.Text = "程序下载中:" + duty.ToString("0.00") + "%";
                                cdloadprogressBar.Value = 0;
                                Connect = false;

                                //烧录完成处理函数
                                //Action_End_Function();
                                timer3.Start();                  //开定时器5,发送烧录完成的标志位0x04
                            }
                            else
                            {
                                cdloadprogressBar.Value = 0;
                                Connect = false;
                                //烧录完成处理函数
                                //Action_End_Function();
                                timer3.Start();                  //开定时器5,发送烧录完成的标志位0x04
                            }

                        }
                    })));
                }
                catch (Exception err)
                {
                    enable_function();
                    MessageBoxMidle.Show(this, "程序升级失败！线程操作异常！", "错误");
                }
            }
        }
        #endregion

        #region 烧录完成处理函数
        private void Action_End_Function()
        {

            //timer3.Start();                  //开定时器5,发送烧录完成的标志位0x04
        }

        #endregion

        #region 定时器3，发送烧录完成的标志位
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (Timer3Jump < 3)
            {
                Usart_Send_Ncrc(Overoder);
                Timer3Jump++;
                toolStripStatusLabel1.Text = "等待主控跳转APP...";
                if (!powerdisplay)
                    databox.AppendText(Commentclass.ChargeStateMessageList[8] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
            }
            else
            {
                loadtimecount = false;
                AckSend_Function.Abort();       // 关闭线程
               // toolStripStatusLabel1.Text = "烧录完成！";
                databox.AppendText(Commentclass.ChargeStateMessageList[9] + DateTime.Now.ToString("hh:mm:ss") + "\r\n\r\n");
                toolStripStatusLabel1.Text = "程序升级完成！";
                timer3.Stop();
                timer8.Stop();
                enable_function();
                // MessageBoxMidle.Show(this, "烧录完成!!!", "提示!!");         //弹出提示框
                serialPort1.Close();
                menuStrip1.Enabled = true;
            }
            //Systemtemp_int();                                 //发完之后对变量初始化
        }
        #endregion

        #region 定时器4函数：进行错误数据的重发
        private void timer4_Tick(object sender, EventArgs e)
        {
           // Usart_Send(filedata_temp);              //发送128个字节数据
            timer4.Stop();
        }
        #endregion

        #region 时间显示函数
        private void timer5_Tick(object sender, EventArgs e)
        {
            //toolStripStatusLabel3.Text = "系统时间：" + DateTime.Now.ToString("hh:mm:ss");
            if (loadtimecount)
            {
                loadtime = loadtime + 1;
            }
           // databox.Focus();
            //databox.Select(databox.Text.Length, 0);
            //this.toolStripStatusLabel2.Text = "计时:" + loadtime.ToString("0.00") + "s";
        }
        #endregion

        #region 主控板窗体打开函数
        private void 主控板程序升级(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x02;  //从窗口2条过去
            int winx = fmwindowX;
            int winy = fmwindowY;
            Commentclass.fm1.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm1.Show();
        }
        #endregion

        #region DSP窗体打开函数
        private void DSP程序升级(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x02;  //从窗口2条过去
            int winx = fmwindowX;
            int winy = fmwindowY;
            Commentclass.fm3.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm3.Show();
        }
        #endregion

        #region Eth
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Commentclass.fmjump = 0x02;  //从窗口2条过去
            int winx = fmwindowX;
            int winy = fmwindowY;
            Commentclass.fm7.Location = new Point(winx, winy);
            Console.WriteLine($"winx = {winx}.");
            Console.WriteLine($"winy = {winy}.");
            Commentclass.fm7.Show();
        }
        #endregion

        #region 文件拖入
        private void cdtextfile_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                cdtextfile.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                OpenFileDialog ofd = new OpenFileDialog();

                if (Path.GetExtension(cdtextfile.Text) != ".bin")
                {
                    MessageBoxMidle.Show(this, "导入文件不是*.bin文件！", "提示");
                    cdtextfile.Clear();
                    return;
                }

                //FileStream fs = new FileStream(cdtextfile.Text, FileMode.Open);
                //byte[] arrfileSend = new byte[1024 * 1024 * 8];             //创建8M的缓存
                //int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                //byte[] arrfile = new byte[length];
                //fs.Close();                                                 //关闭文件流，释放文件
                //Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);       // 将arrfileSend复制到arrfile之中
                //filedata = new byte[length];
                //Buffer.BlockCopy(arrfileSend, 0, filedata, 0, length);      // 将arrfileSend复制到arrfile之中
                //Console.WriteLine($"filedata = {filedata.Length}.");
                ////加入文件大小判断
                //if (filedata.Length > 92160)
                //{
                //    string filename = System.IO.Path.GetFileName(cdtextfile.Text);
                //    DialogResult messdr = MessageBoxMidle.Show(this, "当前操对象:\r\n  充电板ARM！\r\n\r\nBin文件名称：\r\n  " + filename + "\r\n\r\n请确认！", "BIN文件异常提示！", MessageBoxButtons.YesNo);
                //    if (messdr == DialogResult.No)
                //    {
                //        toolStripStatusLabel1.Text = "文件操作已中止，请重新导入！";
                //        return;
                //    }
                //}

                //step_i = 0;                                                 //初始化
                //duty = 0;                                                   //初始化
                //pagenum = length / 128;                                     //初始化  以128个字节为一个数据包发送数据
                //remaind = length % 128;                                     //初始化
                //cjremaind = remaind;
                //Console.WriteLine($"remaind = {remaind}.");
                //cdloadprogressBar.Maximum = pagenum - 1;                    //进度条的最大值初始化
                //toolStripStatusLabel1.Text = "文件已导入，请确认！";
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, "打开文件失败！", "错误");
                toolStripStatusLabel1.Text = "文件导入失败，请检查！";
            }
        }
        #endregion

        #region 文件导入
        private void cdtextfile_DragEnter(object sender, DragEventArgs e)
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
        private void timer6_Tick(object sender, EventArgs e)
        {
            Pic_Vision();
        }
        #endregion

        #region 获取窗体的位置
        private void Form1_Move(object sender, EventArgs e)
        {
            fmwindowX = this.Location.X;
            fmwindowY = this.Location.Y;
            //this.toolStripStatusLabel2.Text = fmwindowX.ToString() + "," + fmwindowY.ToString();
        }

        #endregion

        #region 烧录失败处理函数
        private void timer7_Tick(object sender, EventArgs e)
        {
            timer7.Stop();
            AckSend_Function.Abort();   // 关闭线程
            serialPort1.Close();        //关闭串口
            MessageBoxMidle.Show(this, "程序升级失败！", "错误");
            enable_function();
            menuStrip1.Enabled = true;
        }
        #endregion

        #region 杜绝无应答
        private void timer8_Tick(object sender, EventArgs e)
        {
            if (!System_RS)
            {
                timer8.Stop();
                Connect = false;
                Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                {
                    AckSend_Function.Abort();   // 关闭线程
                    serialPort1.Close();        //关闭串口
                    toolStripStatusLabel1.Text = "程序升级失败！";
                    MessageBoxMidle.Show(this, "程序升级失败！", "错误");
                    enable_function();
                    cdloadprogressBar.Value = 0;
                    menuStrip1.Enabled = true;
                })));
            }
        }


        #endregion

        #region 回车函数
        private void cdpoundbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (cdpoundbox.Text == "1024")
                    {
                        powerdisplay = true;
                        cdpoundbox.Text = "115200";
                    }
                    else if (cdpoundbox.Text == "2048")
                    {
                        powerdisplay = false;
                        cdpoundbox.Text = "115200";
                    }
                    else if (cdpoundbox.Text == "4096")
                    {
                        databox.Clear();
                        cdpoundbox.Text = "115200";
                    }
                    else if (cdpoundbox.Text == "666666")
                    {
                        databox.AppendText("充电器单板刷机模式已开启！\r\n");
                        cdsingle = true;
                        cdpoundbox.Text = "19200";
                    }
                }
                catch (Exception err)
                { MessageBox.Show("详情：" + err.ToString(), "警告"); }
            }
        }
        #endregion

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "单板模式")
            {
                databox.AppendText("充电器单板刷机模式已开启！\r\n");
                cdsingle = true;
                cdpoundbox.Text = "19200";
            }
            else if (comboBox1.Text == "透传模式")
            {
                databox.AppendText("充电器透传刷机模式已开启！\r\n");
                cdsingle = false;
                cdpoundbox.Text = "115200";
            }
        }

        #region 强制退出
        private void 强制退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        private void disable_function()
        {
            menuStrip1.Enabled = false;
            cdloadbtn.Enabled = false;
            cdviewbtn.Enabled = false;
        }

        private void enable_function()
        {
            menuStrip1.Enabled = true;
            cdloadbtn.Enabled = true;
            cdviewbtn.Enabled = true;
        }

        private void databox_TextChanged(object sender, EventArgs e)
        {
            databox.Focus();
        }

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

    }
}
