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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using View;



namespace _485刷机_Net_3._5
{
    delegate void MessageMidelDelegate(string str);
    delegate void MessageDisplayDelegate(string str1, string str2);
    public partial class Form7 : Form
    {
        #region 变量定义区
        String serialPortName;                               //串口名
        public static byte[] filedata;                       //创建保存读取的文件数据的缓存空间
        public static byte[] SneCapre;                       //用于将发送的数据和接收的数据进行保存
        public static byte[] Cutoutfiledata;                 //用于保存截取回来的APP代码片段

        //string backack = "ok";                                 //反馈信息
        //int step_i = 0;                                      //发送的次数 
        //double duty = 0;                                     //发送文件的进度值
        //int pagenum = 0;                                     //计算需要发送多少次
        //int remaind = 0;                                     //计算是否发送次数不为整数
        //int cjremaind = 0;                                   //承接remaind
        public byte[] filedata_temp = new byte[128];         //承接BIN文件被按照128个字节分开后每次的数据 
        //static int i = 0;
        //int loadtime = 0;
        public static byte[] buffer;
        static int bufferAdr;                                   // 打开文件hex to bin缓存指针，用于计算bin的长度
        static bool CutoutActionFlag = true;                    //如果是.hex文件不需要进行文件截取的操作
        string commentpath = System.Windows.Forms.Application.StartupPath + ".\\ZKBIN.\\ZKBIN.bin";
        #endregion
        //窗口变量
        static public int windowX = 0;
        static public int windowY = 0;

        MessageDisplayDelegate LableMessageDisplay;
        MessageMidelDelegate MessageMidle_MsgLink;
        //public Monitor AboutDetial = new Monitor();  //调用子窗口
        ushort ResgierAddress, ResgierQuality, ResgierValues, MbtcpTid;

        #region 线程定义
        private bool RxIsRuning = true;                   //运行标志位
        private bool AcIsRuning = false;                  //运行标志位
        private bool StIsRuning = true;                   //运行标志位

        Thread RxExplaThread= null;                       //接收和剖析功能码事件的线程
        Thread AckTxRxThread = null;                      //应答式收发线程
        Thread StepConThread = null;                      //步骤判断线程

        #endregion

        System.Timers.Timer TranspCodeTimerout = new System.Timers.Timer(1200);//
        public string IniFilesPath = Application.StartupPath + "\\ConnectString.ini";//ini文件的路径
        public bool DspWaiteOneSecondFlag = false;              //DSP延时1s做缓冲

        /// <summary>
        /// 
        /// </summary>
        public Form7()
        {
            InitializeComponent();

            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.btnlabview, "支持文件*.bin拖放");
            MessageMidle_MsgLink = DataDisplayRecMsg;
            LableMessageDisplay = LableMessageDisplay_Msg;
        }
     
        #region 状态控制定时器，用于判断当前状态是否处于刷机状态
        private void StateControl_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Commentclass.CommentUpgradeState = true;
                Console.WriteLine($"StateControl_timer_Tick");
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, err.Message, "提示！");
            }
        }
        #endregion

        #region 命令超时判断定时器，用于判断响应是否超时
        private void AckTimerOut_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Commentclass.CommentAckTimerout = true;
                Console.WriteLine($"AckTimerOut_timer_Tick");
            }
            catch (Exception err)
            {
                MessageBox.Show(this, err.Message, "提示！");
            }
        }
        #endregion

        #region 用于应答式发送代码包超时判断
        private void TranspCodeTimerout_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!Commentclass.CommentAckContinue)
                {
                    TranspCodeTimerout.Stop();
                    RxExplaThread.Abort();
                    AckTxRxThread.Abort();
                    Array.Clear(Commentclass.CommentTempResive, 0, Commentclass.CommentTempResive.Length);//清零
                    Commentclass.CommentBackack = SocketLi.SocketBreakConnect(Commentclass.CommentIP);
                    Commentclass.SocketBreakFlag = true;
                    Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            Thread.Sleep(50);
                            Application.DoEvents();
                        }

                        if (FG == false)
                        {
                            if (Commentclass.CommentBackack == "no")
                            {
                                MessageBoxMidle.Show(this, "网络断开，请检查网络连接是否正常！", "提示！");
                            }
                            else
                            {
                                MessageBoxMidle.Show(this, "超时无响应，请重新下载！", "提示！");
                            }
                        }
                        else
                        {                            
                            if (Commentclass.CommentBackack == "no")
                            {
                                this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + "网络断开，请检查网络连接是否正常！" + "\r\n");
                            }
                            else
                            {
                                this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + "超时无响应，请重新下载！" + "\r\n");
                            }
                        }
                      
                        DownloadRate.Value = 0x00;
                        EnableButton();                        
                    })));
                    // Commentclass.CommentAckContinue = true;                   
                }
            }
            catch (ThreadAbortException ex)
            {
                ;//不处理
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, err.Message, "提示！");
            }
            //finally
            //{
            //    RxExplaThread.Abort();
            //    AckTxRxThread.Abort();
            //}
        }
        #endregion

        #region 用于以太网断开重连的而倒计时
        private void SocketConnectTimer_Tick(object sender, EventArgs e)
        {
            Commentclass.CommentSocketConnectTimerCount--;
            Invoke(LableMessageDisplay, "状态:", "以太网重连中，请等待:"+ Commentclass.CommentSocketConnectTimerCount.ToString()+"s");
        }
        #endregion

        #region 窗体加载
        private void Form7_Load(object sender, EventArgs e)
        {
            /*
            if (Commentclass.fmjump == 0x01)    //从窗口1切换过来
            {
                this.Location = new Point(Form1.windowX, Form1.windowY);
            }   
            if (Commentclass.fmjump == 0x02)
            {
                this.Location = new Point(Form2.fmwindowX, Form2.fmwindowY);
            }
            else if (Commentclass.fmjump == 0x03)
            {
                this.Location = new Point(Form3.fm3windowX, Form3.fm3windowY);
            }

            if (Commentclass.WinDey)
            {
                this.Dspbordenu1.Text = "主控板ARM升级";
                this.Chargebordenu.Text = "充电板ARM升级";                
                this.Dspbordenu1.Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                //this.Dspbordenu.Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.Chargebordenu.Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.Mainbordenu.Font = new System.Drawing.Font("楷体", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                一键擦除ToolStripMenuItem.Visible = false;
                menuStrip1.Items.Remove(Dspbordenu);

                string messageini = ReadINIFiles.ReadIniData("WarnMessage", "MainBordMessage", "None", IniFilesPath);
                string[] nametake3 = messageini.Split("\\r\\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                databox.Clear();
                foreach (string str in nametake3)
                {
                    databox.AppendText(str + "\r\n");
                }
                //ipaddres.Text ="已自动识别";
                if (databox.TextLength >= 5)
                {
                    databox.Select(17, 8);
                    databox.SelectionColor = Color.Red;
                }
                textBox1.Visible = false;
                textBox2.Visible = false;
                textBox3.Visible = false;
                pointlable.Text = "";
                pointlable.Enabled=false;
                
                //界面设计
                this.Icon = IconSelect.GetFileIcon(Application.StartupPath + "\\icon\\wendey.ico");
                this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
                this.Size = new Size(410, 367);
            }
            else
            {
                this.Dspbordenu1.Text = "主ARM升级(Eth)";
                this.Chargebordenu.Text = "充ARM升级";
                this.Dspbordenu1.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                //this.Dspbordenu.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.Chargebordenu.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.Mainbordenu.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                databox.Clear();
                databox.Text = "支持*.hex文件和合并*.bin文件刷机";
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                pointlable.Text = "点击开始循环测试";
                pointlable.BackColor = Color.Yellow;
                //界面设计
                this.Icon = IconSelect.GetFileIcon(Application.StartupPath + "\\icon\\wendey.ico");
                this.Text = ReadINIFiles.ReadIniData("UserConfig", "ApplicationName", "None", IniFilesPath);
                this.Size = new Size(410, 477);
            }

            if (Commentclass.OpenTheWindws == 0x01)
            {
                this.Location = new Point(Commentclass.windowXx, Commentclass.windowYy);
                Commentclass.OpenTheWindws = 0x00;
                Debug.WriteLine("位置校正....................");                
            }
           */
            //this.Text = ReadINIFiles.ReadIniData("ApplicationMessage", "Name", "None", IniFilesPath);
            //this.Size = new Size(526,223);
            //加上注册表读取路径,首先判断
            RegistryKeyLi.PreventCreadErr(Commentclass.CommentPublicResgistryKeyPath,Commentclass.CommentMainResgistryKeyName);     //主控板
            RegistryKeyLi.PreventCreadErr(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargResgistryKeyName);   //充电板
            RegistryKeyLi.PreventCreadErr(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentDspResgistryKeyName);     //DSP
            //Commentclass.CommentIP = ipaddres.Text;

           // versionlable.Text = "Version:"+ Commentclass.Version;
            TranspCodeTimerout.AutoReset = true;// 是否重复调用 Elapsed事件方法,如果为false 则Elapsed 事件方法就只会调用一次。这里就是TimeAction方法就只会调用一次
            TranspCodeTimerout.Elapsed += new System.Timers.ElapsedEventHandler(TranspCodeTimerout_Tick);

            //读取ini文件,获取主控板ARM的 BootloaderHeadFlag 、CodeStartAddr、RedirectionAddr
            var ReadIniTempFlag0 = ReadINIFiles.ReadIniData("Message", "MainBordBootloaderHeadFlagINI", "None", IniFilesPath);
            Console.WriteLine($"ZKReadIniTemp = {ReadIniTempFlag0}.");
            string[] nametake0 = ReadIniTempFlag0.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 16; i++)
            {
                Commentclass.MainbordBootloaderHeadFlag[i] = Convert.ToByte(nametake0[i], 16);
                Console.WriteLine($"ZKBootloaderHeadFlag = {Commentclass.MainbordBootloaderHeadFlag[i].ToString("x")}.");
            }
            //获取CodeStartAddr
            var ReadIniTempStartAddr0 = ReadINIFiles.ReadIniData("Message", "MainBordCodeStartAddrINI", "None", IniFilesPath);
            Commentclass.MainbordCodeStartAddr = Convert.ToUInt64(ReadIniTempStartAddr0, 16);
            Console.WriteLine($"ZKCodeStartAddr = {"0X" + Commentclass.MainbordCodeStartAddr.ToString("x")}.");

            //获取RedirectionAddr
            var ReadIniTempTionAddr0 = ReadINIFiles.ReadIniData("Message", "MainBordRedirectionAddrINI", "None", IniFilesPath);
            Commentclass.MainbordRedirectionAddr = Convert.ToUInt64(ReadIniTempTionAddr0, 16);
            Console.WriteLine($"ZKRedirectionAddr = {"0X" + Commentclass.MainbordRedirectionAddr.ToString("x")}.");
            Commentclass.MainbordIniRecordBootloaderSize = Commentclass.MainbordRedirectionAddr - Commentclass.MainbordCodeStartAddr;

            //读取ini文件,获取充电板ARM的 BootloaderHeadFlag 、CodeStartAddr、RedirectionAddr
            var ReadIniTempFlag1 = ReadINIFiles.ReadIniData("Message", "ChargeBordBootloaderHeadFlagINI", "None", IniFilesPath);
            Console.WriteLine($"CDReadIniTemp = {ReadIniTempFlag1}.");
            string[] nametake1 = ReadIniTempFlag1.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 16; i++)
            {
                Commentclass.ChargebordBootloaderHeadFlag[i] = Convert.ToByte(nametake1[i], 16);
                Console.WriteLine($"CDBootloaderHeadFlag = {Commentclass.ChargebordBootloaderHeadFlag[i].ToString("x")}.");
            }
            //获取CodeStartAddr
            var ReadIniTempStartAddr1 = ReadINIFiles.ReadIniData("Message", "ChargeBordCodeStartAddrINI", "None", IniFilesPath);
            Commentclass.ChargebordCodeStartAddr = Convert.ToUInt64(ReadIniTempStartAddr1, 16);
            Console.WriteLine($"CDCodeStartAddr = {"0X" + Commentclass.ChargebordCodeStartAddr.ToString("x")}.");
            //获取RedirectionAddr
            var ReadIniTempTionAddr1 = ReadINIFiles.ReadIniData("Message", "ChargeBordRedirectionAddrINI", "None", IniFilesPath);
            Commentclass.ChargebordRedirectionAddr = Convert.ToUInt64(ReadIniTempTionAddr1, 16);
            Console.WriteLine($"CDRedirectionAddr = {"0X" + Commentclass.ChargebordRedirectionAddr.ToString("x")}.");
            Commentclass.ChargebordIniRecordBootloaderSize = Commentclass.ChargebordRedirectionAddr - Commentclass.ChargebordCodeStartAddr;

            //0xff的校验个数
            Commentclass.CommentMainBordCheckNum = uint.Parse( ReadINIFiles.ReadIniData("Message", "MainBordCheckCount", "None", IniFilesPath));
            Commentclass.CommentChargeBordCheckNum = uint.Parse( ReadINIFiles.ReadIniData("Message", "ChargeBordCheckCount", "None", IniFilesPath));

            //////刷机目标文件的大小限制
            //读取刷机文件(主控板)大小限制参数
            //string data = ReadINIFiles.ReadIniData("FileSizeSet", "MainBordFileHexMin", "None", IniFilesPath);
                                                                                                       
            
            //充电板目标文件的最小值
            Commentclass.ChargeBoardFileSizeMin = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "ChargeBordFileMin", "None", IniFilesPath)) * 1024;
            //充电板目标文件的最大值
            Commentclass.ChargeBoardFileSizeMax = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "ChargeBordFileMax", "None", IniFilesPath)) * 1024;
            //DSP目标文件的最小值
            Commentclass.DSPBoardFileSizeMin = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "DSPBordFileMin", "None", IniFilesPath)) * 1024;
            //DSP目标文件的最大值
            Commentclass.DSPBoardFileSizeMax = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "DSPBordFileMax", "None", IniFilesPath)) * 1024;

            Commentclass.MainBoardFileHexSizeMin = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainHexMin", "None", IniFilesPath)) * 1024;
            //主控板目标文件的最大值
            Commentclass.MainBoardFileHexSizeMax = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainHexMax", "None", IniFilesPath)) * 1024;
            //读取刷机文件(主控板)大小限制参数
            Commentclass.MainBoardFileBinSizeMin = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainBinMin", "None", IniFilesPath)) * 1024;
            //主控板目标文件的最大值
            Commentclass.MainBoardFileBinSizeMax = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "MainBinMax", "None", IniFilesPath)) * 1024;

            //ContractUI();
            string maximalsize = ReadINIFiles.ReadIniData("MainBordCodeSizeLimti", "MaximalSize", "None", IniFilesPath);
            string minimalsize = ReadINIFiles.ReadIniData("MainBordCodeSizeLimti", "MinimalSize", "None", IniFilesPath);
            string[] para = maximalsize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] para1 = minimalsize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Commentclass.MainBordMaximalSize = Convert.ToUInt64(para[0]) * Convert.ToUInt64(para[1]);
            Commentclass.MainBordMinimalSize = Convert.ToUInt64(para1[0]) * Convert.ToUInt64(para1[1]);

            MainBoardUI();

            if (ReadINIFiles.ReadIniData("MonitorWindows", "Display", "None", IniFilesPath) == "TRUE")
            {
                AmpliyUI();
            }
            //ipaddres.Text = ReadINIFiles.ReadIniData("DspLinkData", "IpAddress", "None", IniFilesPath);
            Commentclass.CommentNetPort = int.Parse(ReadINIFiles.ReadIniData("DspLinkData", "NetPortNum", "None", IniFilesPath));
            Console.WriteLine($" Commentclass.CommentNetPort  = { Commentclass.CommentNetPort }.");

            Admin_UI();
        }

        /// <summary>
        /// 注册表客户模式下隐藏住485刷机
        /// </summary>
        bool Admin_root;
        void Admin_UI()
        {
            string msg;
            this.toolStripStatusLabel4.Visible = false;


            //this.pointlable.Visible = false;

            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName, out msg);
            Console.WriteLine("模式：" + msg);
            if (msg == "User_Client")
            {
                this.databox.Text = "注意：\r\n1、ARM以太网升级前，需打到手动模式！！！\r\n2、ARM以太网升级使用驱动器X6A以太网接口；\r\n3、电脑端ip设置为192.168.1.XXX（大于20，小于254都可以）；\r\n4、上位机ip设置为192.168.1.拨码+10；";

                Admin_root = false;
                this.databox.Select(19, 4);
                this.databox.SelectionFont = new Font("宋体", 13.5f, FontStyle.Underline | FontStyle.Bold);
                this.databox.SelectionColor = Color.Red;
                this.databox.Select(0, 0);

                //this.menuStrip1.Items[1].Visible = false;
                this.menuStrip1.Items[3].Visible = false;
                this.menuStrip1.Items[5].Visible = false;


                this.pointlable.Text = " ";
                this.pointlable.Enabled = false;



                this.Width = 550;
                this.Height = 380;
            }
            else
            { 
                Admin_root= true;
            }

        }

        #endregion

        #region 窗体切换


        private void Form7_Activated(object sender, EventArgs e)
        {
            ipaddres.Text = Commentclass.CommentIP;
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

        #region UI布局设计函数
        private void MainBoardUI()
        {
            try
            {
                Commentclass.CommentChekNum = Commentclass.CommentMainBordCheckNum;              
                //databox.Clear();
                //databox.Text = "支持*.hex文件和合并*.bin文件刷机";
                //statelable.Text = "注意：仅支持合并的.bin文件升级";
                Commentclass.CommentUpgrade = 0X00;     //升级对象主控板ARM
             //   this.Mainbordenu.Enabled = true;
             //   this.Chargebordenu.Enabled = true;
             //   this.Dspbordenu1.Enabled = true;
                this.ModelBox.Hide();
                this.label3.Hide();

                //把路径读出来
                Commentclass.CommentRuningKyeName = Commentclass.CommentMainResgistryKeyName;
                //读取路径，按读到的路径打开相应的文件路径
                string msg;
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName, out msg);
                Console.WriteLine($"读出来的路径 = {msg}.");
                if (Commentclass.CommentReadSavePath == "" | Commentclass.CommentReadSavePath == string.Empty)
                {
                    msg = "D:\\";
                }
                pathtextBox.Text = msg;

                //读取IP
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, "CommentIP", out Commentclass.CommentIP);
                Console.WriteLine($"读出来的路径 = {Commentclass.CommentIP}.");
                ipaddres.Text = Commentclass.CommentIP;
                if (Commentclass.CommentIP == "")
                {
                    Commentclass.CommentIP = "192.168.1.15";
                    
                }
                this.ipaddres.Text = Commentclass.CommentIP;
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, err.Message, "提示！");
            }
        }

        /*
        private void ChargeBoardUI()
        {
            try
            {
                Commentclass.CommentChekNum = Commentclass.CommentChargeBordCheckNum;
                databox.Clear();
                databox.Text = "当前刷机对象为充电板ARM，只支持合并的Bin文件刷机";
                statelable.Text = "注意：仅支持合并的.bin文件升级";
                Commentclass.CommentUpgrade = 0X02;     //升级对象充电板ARM
                this.Chargebordenu.Enabled = false;
                this.Mainbordenu.Enabled = true;
                this.Dspbordenu1.Enabled = true;
                this.ModelBox.Show();
                this.label3.Show();
                Mainbordenu.BackColor = System.Drawing.SystemColors.ControlLight;
                Chargebordenu.BackColor = System.Drawing.Color.Chartreuse;
                Dspbordenu1.BackColor = System.Drawing.SystemColors.ControlLight;

                //把路径读出来
                Commentclass.CommentRuningKyeName = Commentclass.CommentChargResgistryKeyName;
                //读取路径，按读到的路径打开相应的文件路径
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName, out Commentclass.CommentReadSavePath);
                Console.WriteLine($"读出来的路径 = {Commentclass.CommentReadSavePath}.");

                pathtextBox.Text = Commentclass.CommentReadSavePath;

            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, err.Message, "提示！");
            }
        }
        private void DSPBoardUI()
        {
            try
            {
                databox.Clear();
                databox.Text = "当前刷机对象为主控板DSP。";
                statelable.Text = "以太网程序升级";
                Commentclass.CommentUpgrade = 0X01;     //升级对象主控板DSP
                this.Dspbordenu1.Enabled = false;
                this.Chargebordenu.Enabled = true;
                this.Mainbordenu.Enabled = true;
                this.ModelBox.Hide();
                this.label3.Hide();
                Mainbordenu.BackColor = System.Drawing.SystemColors.ControlLight;
                Chargebordenu.BackColor = System.Drawing.SystemColors.ControlLight;
                Dspbordenu1.BackColor = System.Drawing.Color.Chartreuse;

                //把路径读出来
                Commentclass.CommentRuningKyeName = Commentclass.CommentDspResgistryKeyName;
                //读取路径，按读到的路径打开相应的文件路径
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName, out Commentclass.CommentReadSavePath);
                Console.WriteLine($"读出来的路径 = {Commentclass.CommentReadSavePath}.");

                pathtextBox.Text = Commentclass.CommentReadSavePath;

            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, err.Message, "提示！");
            }
        }
         */
        #endregion


        #region 按键处理函数
        private void btnclick(object sender, EventArgs e)
        {
            Int16 Vari = 0;
            Vari = Convert.ToInt16(((Button)sender).Tag.ToString());
            try
            {                
                switch (Vari)
                {
                    case 0://浏览                   
                        //首先判断刷机目标,然后决定文件夹的名称
                        Commentclass.CommentUpgrade = 0x00; //特定主控ARM
                        switch (Commentclass.CommentUpgrade)
                        {
                            case 0x00://主控板
                                Commentclass.CommentRuningKyeName = Commentclass.CommentMainResgistryKeyName;
                                break;
                            case 0x01://DSP
                                Commentclass.CommentRuningKyeName = Commentclass.CommentDspResgistryKeyName;
                                break;
                            case 0x02://充电板
                                Commentclass.CommentRuningKyeName = Commentclass.CommentChargResgistryKeyName;
                                break;
                        }
                        //读取路径，按读到的路径打开相应的文件路径
                        RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName, out Commentclass.CommentReadSavePath);
                        Console.WriteLine($"读出来的路径 = {Commentclass.CommentReadSavePath}.");

                        if (Admin_root)
                        {
                            Commentclass.CommentBackack = ReadFiles.CommentOpenFiles(Commentclass.CommentReadSavePath, out Commentclass.CommentRealyPath);
                        }
                        else
                        {
                            Commentclass.CommentBackack = ReadFiles.CommentOpenFiles(Commentclass.CommentReadSavePath, out Commentclass.CommentRealyPath,"|*.hex");
                        }
                        

                        if (Commentclass.CommentBackack != "ok" && Commentclass.CommentBackack != "ds")
                        {
                            //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                        }
                        else if (Commentclass.CommentBackack == "ds")
                        {
                            return;         //取消加载文件
                        }
                        else
                        {
                            pathtextBox.Text = Commentclass.CommentRealyPath;
                            //将路径存进去
                            RegistryKeyLi.WriteRegistryKey(Commentclass.CommentRealyPath, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName);
                            Console.WriteLine($"存进去的路径 = { Commentclass.CommentRealyPath}.");
                        }
                        break;
                    case 1:     //下载
                        {                          
                           MainPrarInit();                                                                                          //变量初始化函数
                           DisableButton();
                            if (Admin_root)
                                this.databox.Clear();
                           if(Commentclass.CommentUpgrade != 0x03)
                            {
                                textBox3.Text = (Convert.ToInt32(textBox3.Text) + 1).ToString();
                            }                          
                           Commentclass.CommentRealyPath = pathtextBox.Text;//获取路径
                           Commentclass.CommentIP = ipaddres.Text;
                           RegistryKeyLi.WriteRegistryKey(Commentclass.CommentIP, Commentclass.CommentPublicResgistryKeyPath, "CommentIP");
                           RegistryKeyLi.WriteRegistryKey(this.pathtextBox.Text, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainResgistryKeyName);
                            Commentclass.CommentBackack = SocketLi.SocketPingNet(Commentclass.CommentIP);
                           if (Commentclass.CommentBackack == "no")
                           {
                                if ( FG == false)
                                {
                                    MessageBoxMidle.Show(this, "网络错误！请检查网络连接是否正常？ip地址设置是否正确？", "提示");
                                }
                                else
                                {
                                    this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + "该IP地址无法访问，请填入正确IP！" + "\r\n");
                                    //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                    for (int i = 0; i < 100; i++)
                                    {
                                        Thread.Sleep(50);
                                        Application.DoEvents();
                                    }                                    
                                }
                                EnableButton();
                                return;
                           }
                            //路径框判空
                           if (string.IsNullOrEmpty(pathtextBox.Text))
                           {
                                EnableButton();
                                MessageBoxMidle.Show(this, "请选择你要下载的ARM.Hex文件！", "提示！");
                                return;
                           }

                           if (string.IsNullOrEmpty(Commentclass.CommentRealyPath))
                           {
                                EnableButton();
                                MessageBoxMidle.Show(this, "文件路径非法，请重新选择。", "提示！");
                                return;
                           }
                           else
                           {                           
                                Console.WriteLine($"Commentclass.CommentRealyPath = {Commentclass.CommentRealyPath}.");   //提取文件
                                Commentclass.CommentBackack = ReadFiles.CommentReadFiles(Commentclass.CommentRealyPath, out Commentclass.CommentFileData, out Commentclass.CommentFileLength);
                                if (Commentclass.CommentBackack != "ok")
                                {
                                    EnableButton();
                                    MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine($"Commentclass.CommentFileData = {Commentclass.CommentFileData.Length}.");
                                    Console.WriteLine($"Commentclass.CommentFileLength = {Commentclass.CommentFileLength}.");
                                    if (Commentclass.CommentFileData.Length != Commentclass.CommentFileLength)//再次确认文件文件大小与文件长度是否相同
                                    {
                                        EnableButton();
                                        MessageBoxMidle.Show(this, "刷机文件导入有误，请重新导入。", "提示！");
                                        return;
                                    }
                                }
                           }
                           #region
                           if (Commentclass.CommentUpgrade == 0X01)//刷机对象是DSP,需要重新计算烧录的Bin文件的大小
                           {
                                //判断DSP文件的大小是否合适
                                if (((uint)Commentclass.CommentFileData.Length >= Commentclass.DSPBoardFileSizeMin) && ((uint)Commentclass.CommentFileData.Length <= Commentclass.DSPBoardFileSizeMax))
                                {
                                    Commentclass.CommentBackack = ReadFiles.AbraseSectorNum(Commentclass.CommentFileData, Commentclass.CommentFileLength, out Commentclass.CommentDspAbraseNum, out Commentclass.CommentDspNewFilesSize);
                                    if (Commentclass.CommentBackack == "ok")
                                    {
                                        byte[] FilesDataTemp = new byte[Commentclass.CommentDspNewFilesSize];
                                        Buffer.BlockCopy(Commentclass.CommentFileData, 0, FilesDataTemp, 0, (int)Commentclass.CommentDspNewFilesSize);
                                        Commentclass.CommentFileData = new byte[Commentclass.CommentDspNewFilesSize];
                                        Buffer.BlockCopy(FilesDataTemp, 0, Commentclass.CommentFileData, 0, (int)Commentclass.CommentDspNewFilesSize);
                                        Console.WriteLine($"Commentclass.CommentFileData.Length = {Commentclass.CommentFileData.Length}.");
                                        Console.WriteLine($"Commentclass.CommentDspNewFilesSize = {Commentclass.CommentDspNewFilesSize}.");
                                        Console.WriteLine($"擦除扇区数 = {Commentclass.CommentDspAbraseNum}.");
                                        Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;
                                    }
                                    else
                                    {
                                        EnableButton();
                                        MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                        return;
                                    }
                                }
                                else
                                {
                                    EnableButton();
                                    MessageBoxMidle.Show(this, "刷机文件有误，请重新导入!", "提示!");
                                    return;
                                }
                           }
                           else if (Commentclass.CommentUpgrade == 0X00|| Commentclass.CommentUpgrade == 0X03)//主控板需要重新截取目标刷机文件
                           {
                                int length;
                                if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.hex文件
                                {
                                    //commentpath = System.Windows.Forms.Application.StartupPath + ".\\ZKBIN.\\ZKBIN.bin";
                                    CutoutActionFlag = false;
                                    HexFileRead(pathtextBox.Text);
                                    Commentclass.CommentFileData = new byte[bufferAdr];
                                    Array.Clear(Commentclass.CommentFileData, 0, Commentclass.CommentFileData.Length);
                                    Buffer.BlockCopy(buffer, 0, Commentclass.CommentFileData, 0, bufferAdr);    // 将buffer复制到filedata之中
                                    length = Commentclass.CommentFileData.Length;
                                    Console.WriteLine($"导入为hex文件");
                                }
                                else if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件
                                {
                                    commentpath = pathtextBox.Text;
                                    Console.WriteLine($"导入为bin文件");
                                    FileStream fs = new FileStream(commentpath, FileMode.Open, FileAccess.Read);
                                    byte[] arrfileSend = new byte[1024 * 1024 * 8];         //创建8M的缓存
                                    length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                                    byte[] arrfile = new byte[length];
                                    fs.Close();                                              //关闭文件流，释放文件
                                    Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);    // 将arrfileSend复制到arrfile之中
                                    Commentclass.CommentFileData = new byte[length];
                                    Buffer.BlockCopy(arrfileSend, 0, Commentclass.CommentFileData, 0, length);   // 将arrfileSend复制到arrfile之中
                                }
                                else
                                {
                                    EnableButton();
                                    MessageBoxMidle.Show("导入文件类型错误\r\n" + "请重新导入。", "文件错误");
                                    pathtextBox.Clear();
                                    return;
                                }
                                Console.WriteLine($"filedata = {Commentclass.CommentFileData.Length}.");
                               
                                
                                
                                
                                

                                if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件，截取后比对大小
                                {
                                    if (Commentclass.CommentFileData.Length < (int)Commentclass.MainBoardFileBinSizeMin || Commentclass.CommentFileData.Length > (int)Commentclass.MainBoardFileBinSizeMax)  //文件小于90k的和大于200k的都不行
                                    {
                                        EnableButton();
                                        MessageBoxMidle.Show("当前导入的程序文件有误！请重新导入正确的程序文件!", "文件错误");
                                        return;
                                    }

                                    Commentclass.CommentBackack = ReadFiles.FragmentCode(Commentclass.MainbordRedirectionAddr, Commentclass.MainbordCodeStartAddr, (UInt64)Commentclass.CommentFileData.Length,
                                                                                     Commentclass.MainbordIniRecordBootloaderSize, Commentclass.MainbordBootloaderHeadFlag, Commentclass.CommentFileData, out Commentclass.CutSectorFileData);

                                    if (Commentclass.CommentBackack == "ok")
                                    {
                                        Commentclass.CommentFileData = new byte[Commentclass.CutSectorFileData.Length];
                                        Buffer.BlockCopy(Commentclass.CutSectorFileData, 0, Commentclass.CommentFileData, 0, Commentclass.CutSectorFileData.Length);
                                        Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;
                                        //判断主控板的刷机文件是否合适
                                        if (((ulong)Commentclass.CommentFileData.Length < Commentclass.MainBoardFileBinSizeMin) || (ulong)Commentclass.CommentFileData.Length > Commentclass.MainBoardFileBinSizeMax)
                                        {

                                            MessageBoxMidle.Show(this, "当前导入的程序文件有误！请重新导入正确的程序文件!", "提示!");
                                            EnableButton();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        EnableButton();
                                        MessageBox.Show(Commentclass.CommentBackack + "程序文件截取失败，识别不到扇区HeadFlag信息！请选择正确的程序文件！", "cut_out_err");
                                        return;
                                    }
                                }
                                else if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.bin文件
                                {
                                    Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;
                                    //判断主控板的刷机文件是否合适
                                    if (((ulong)Commentclass.CommentFileData.Length < Commentclass.MainBoardFileHexSizeMin) || ((ulong)Commentclass.CommentFileData.Length > Commentclass.MainBoardFileHexSizeMax))
                                    {

                                        MessageBoxMidle.Show(this, "当前导入的程序文件有误！请重新导入正确的程序文件!", "提示!");
                                        EnableButton();
                                        return;
                                    }
                                }
                                

                                /*
                                if (Commentclass.CommentBackack == "ok")
                                {
                                    if (Path.GetExtension(pathtextBox.Text) == ".bin")//如果是.bin文件
                                    {
                                        Commentclass.CommentBackack = ReadFiles.FragmentCode(Commentclass.MainbordRedirectionAddr, Commentclass.MainbordCodeStartAddr, (UInt64)Commentclass.CommentFileData.Length,
                                                                                     Commentclass.MainbordIniRecordBootloaderSize, Commentclass.MainbordBootloaderHeadFlag, Commentclass.CommentFileData, out Commentclass.CutSectorFileData);


                                        Commentclass.CommentFileData = new byte[Commentclass.CutSectorFileData.Length];
                                        Buffer.BlockCopy(Commentclass.CutSectorFileData, 0, Commentclass.CommentFileData, 0, Commentclass.CutSectorFileData.Length);
                                        Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;
                                        //判断主控板的刷机文件是否合适
                                        if (((ulong)Commentclass.CommentFileData.Length < Commentclass.MainBoardFileBinSizeMin) || (ulong)Commentclass.CommentFileData.Length > Commentclass.MainBoardFileBinSizeMax)
                                        {

                                            MessageBoxMidle.Show(this, "当前导入的程序文件有误！请重新导入正确的程序文件!", "提示!");
                                            EnableButton();
                                            return;
                                        }
                                    }
                                    else if (Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.bin文件
                                    {
                                        //Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;
                                        //判断主控板的刷机文件是否合适
                                        if (((ulong)Commentclass.CommentFileData.Length < Commentclass.MainBoardFileHexSizeMin) || ((ulong)Commentclass.CommentFileData.Length > Commentclass.MainBoardFileHexSizeMax))
                                        {

                                            MessageBoxMidle.Show(this, "当前导入的程序文件有误！请重新导入正确的程序文件!", "提示!");
                                            EnableButton();
                                            return;
                                        }
                                    }







                                    ////导出Bin文件
                                    //FileStream fs = new FileStream("主控板截取文件.bin", FileMode.OpenOrCreate);
                                    //BinaryWriter binWriter = new BinaryWriter(fs);
                                    //binWriter.Write(Commentclass.CommentFileData, 0, Commentclass.CommentFileData.Length);
                                    //binWriter.Close();
                                    //fs.Close();
                                    //return;
                                }
                                else
                                {
                                    EnableButton();
                                    MessageBox.Show(Commentclass.CommentBackack + ":请检查是否为.或者合并.bin文件，或配置表是否符合当前目标文件", "cut_out_err");
                                    return;
                                }
                                */
                           }
                           else if (Commentclass.CommentUpgrade == 0X02)//充电板需要重新截取目标刷机文件
                           {
                                Console.WriteLine($"充电板截取目标刷机文件");
                                Commentclass.CommentBackack = ReadFiles.FragmentCode(Commentclass.ChargebordRedirectionAddr, Commentclass.ChargebordCodeStartAddr, (UInt64)Commentclass.CommentFileData.Length,
                                                                                    Commentclass.ChargebordIniRecordBootloaderSize, Commentclass.ChargebordBootloaderHeadFlag, Commentclass.CommentFileData, out Commentclass.CutSectorFileData);
                                if (Commentclass.CommentBackack == "ok")
                                {
                                    //重新覆盖刷机文件
                                    Commentclass.CommentFileData = new byte[Commentclass.CutSectorFileData.Length];
                                    Buffer.BlockCopy(Commentclass.CutSectorFileData, 0, Commentclass.CommentFileData, 0, Commentclass.CutSectorFileData.Length);
                                    Commentclass.CommentFileLength = (uint)Commentclass.CommentFileData.Length;

                                    //判断充电板的刷机文件是否合适
                                    if (((uint)Commentclass.CommentFileLength < Commentclass.ChargeBoardFileSizeMin) || ((uint)Commentclass.CommentFileLength > Commentclass.ChargeBoardFileSizeMax))
                                    {
                                        EnableButton();
                                        MessageBoxMidle.Show(this, "刷机文件有误，请重新导入!", "提示!");
                                        return;
                                    }
                                    ////导出bin文件
                                    //FileStream fs = new FileStream("充电板截取文件.bin", FileMode.OpenOrCreate);
                                    //BinaryWriter binWriter = new BinaryWriter(fs);
                                    //binWriter.Write(Commentclass.CommentFileData, 0, Commentclass.CommentFileData.Length);
                                    //binWriter.Close();
                                    //fs.Close();
                                    //return;
                                }
                                else
                                {
                                    EnableButton();
                                    MessageBox.Show(Commentclass.CommentBackack+":请检查是否为合并.bin文件，或配置表是否符合当前目标.bin文件", "cut_out_err");
                                    return;
                                }
                           }
                            #endregion

                            //提示
                            if (FG == false&& Commentclass.CommentUpgrade != 0x03)
                            {
                                DialogResult messdr = MessageBoxMidle.Show(this, "请先打手动开关进入手动模式！\r\n是否已完成该操作？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                                if (messdr == DialogResult.No)
                                {
                                    EnableButton();
                                    return;
                                }
                            }
     
                            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 10000));
                            listener.Start();
                            Commentclass.CommentBackack = SocketLi.SocketBreakConnect(Commentclass.CommentIP);
                            Thread.Sleep(20);
                            Commentclass.CommentBackack = SocketLi.SocketConnect(Commentclass.CommentIP, Commentclass.CommentNetPort);//连接以太网
                            listener.Stop();
                           if (Commentclass.CommentBackack == "ok")
                           {
                                Commentclass.SocketBreakFlag = false;
                           
                                //计算发包的包数以及相关变量初始化
                                Commentclass.CommentStepI = 0x00;
                                Commentclass.CommentDuty = 0x00;
                                Commentclass.CommentPagenum = Commentclass.CommentFileLength / 128;
                                Commentclass.CommentRemaind = Commentclass.CommentFileLength % 128;
                                //DownloadRate.Maximum = (int)(Commentclass.CommentPagenum-1 ); //进度条的最大值
                                Console.WriteLine($"  Commentclass.CommentPagenum = {  Commentclass.CommentPagenum}.");
                                Console.WriteLine($" Commentclass.CommentRemaind  = { Commentclass.CommentRemaind }.");
                                Console.WriteLine($"Commentclass.CommentFileData.Length = {Commentclass.CommentFileData.Length}.");

                                //开启以太网接收线程
                                RxExplaThread = new Thread(RxExplaThreadHandIqn);
                                RxExplaThread.IsBackground = true;
                                RxExplaThread.Start();

                                //开步骤判断线程                        
                                StepConThread = new Thread(StepConThreadHandIqn);
                                StepConThread.IsBackground = true;
                                Commentclass.CommentStepNow = 0x01;
                                StepConThread.Start();

                            }
                           else
                           {
                                if(FG == false)
                                {                                    
                                    MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                }
                                else
                                {
                                    this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + "以太网拒绝连接，请重新下载！" + "\r\n");
                                    //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                    for (int i = 0; i < 200; i++)
                                    {
                                        Thread.Sleep(50);
                                        Application.DoEvents();
                                    }
                                }
                                EnableButton();
                            }
                            break;
                        }
                }
            }
            catch (Exception err)
            {
                if (FG == false)
                {
                    MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                }
                else
                {
                    this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + Commentclass.CommentBackack + "\r\n");
                    //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                    for (int i = 0; i < 200; i++)
                    {
                        Thread.Sleep(50);
                        Application.DoEvents();
                    }
                }
                EnableButton();
            }
        }
        #endregion

        #region 链接到网址
        private void toolStripStatusLabe_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("www.stars.com.cn");
            //登录权限
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

        #region 界面切换
        private void HomeSwitcher(object sender,EventArgs e)
        {
            Int16 vari = 0;
            int winx = Commentclass.windowX;
            int winy = Commentclass.windowY;
            vari = Convert.ToInt16(((ToolStripMenuItem)sender).Tag.ToString());
            Console.WriteLine($"vari = {vari}.");
            switch (vari)
            {
                case 0x00://主控板
                    this.Refresh();
                    this.Hide();
                    Commentclass.fm3.Location = new Point(winx, winy);
                    Console.WriteLine($"winx = {winx}.");
                    Console.WriteLine($"winy = {winy}.");
                    Commentclass.fmjump = 0x07;  //从窗口1条过去
                    Commentclass.fm3.Show();
                    break;
                case 0x01://充电板
                    this.Refresh();
                    this.Hide();
                    Commentclass.fm2.Location = new Point(winx, winy);
                    Console.WriteLine($"winx = {winx}.");
                    Console.WriteLine($"winy = {winy}.");
                    Commentclass.fmjump = 0x07;  //从窗口1条过去
                    Commentclass.fm2.Show();
                    break;
                case 0x02://DSP
                    this.Refresh();
                    this.Hide();
                    Commentclass.fm1.Location = new Point(winx, winy);
                    Console.WriteLine($"winx = {winx}.");
                    Console.WriteLine($"winy = {winy}.");
                    Commentclass.fmjump = 0x07;  //从窗口1条过去
                    Commentclass.fm1.Show();
                    break;
                case 0x03://DSP
                    break;
            }     
        }
        #endregion

        #region 收缩界面
        private void ContractUI()
        {
            databox.Hide();
            groupBox2.Location = new Point(9, 30);
            this.Size = new Size(420, 295);
        }
        #endregion

        #region 放大界面
        private void AmpliyUI()
        {
            databox.Show();
            groupBox2.Location = new Point(11, 160);
            this.Size = new Size(415,355);
        }
        #endregion

        #region 窗体移动位置获取
        private void Form7_Move(object sender, EventArgs e)
        {
            Commentclass.windowX = this.Location.X;
            Commentclass.windowY = this.Location.Y;
            windowX = this.Location.X;
            windowY = this.Location.Y;
            Console.WriteLine($"winx = {Commentclass.windowX}.");
            Console.WriteLine($"winy = {Commentclass.windowY}.");
        }
        #endregion

        #region 线程
        #region 接收和剖析功能以及置起相关标志位
        private void RxExplaThreadHandIqn()
        {
           while (RxIsRuning)
           {
                try
                {
                    Commentclass.CommentBackack = SocketLi.SocketResiveData(out Commentclass.CommentTempResive);                   
                    Console.WriteLine($"Commentclass.CommentTempResive = {Commentclass.CommentTempResive.Length}.");
                    if (Commentclass.CommentTempResive.Length < 0x06)
                    {
                        //Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                        //{
                        //    this.databox.AppendText("Recv"+ "(" + DateTime.Now.ToString("ss fff")+ ")" + ":"  + StartsCRC.zftring(Commentclass.CommentTempResive) + "\r\n" + "\r\n");
                        //})));                       
                        //Array.Clear(Commentclass.CommentTempResive, 0, Commentclass.CommentTempResive.Length);//清零
                        return;
                    }
                    else
                    {
                        Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                        {
                            if(Admin_root)
                            this.databox.AppendText("Recv"+ "(" + DateTime.Now.ToString("ss fff")+ ")" + ":" + StartsCRC.zftring(Commentclass.CommentTempResive) + "\r\n" + "\r\n");
                        })));
                    }

                    if (Commentclass.CommentBackack == "ok")        //数据接收完成
                    {
                        //解析数据，并决定下一步
                        if (Commentclass.CommentTempResive[2] != 0x00 || Commentclass.CommentTempResive[3] != 0x00 || Commentclass.CommentTempResive[6] != Commentclass.CommentMbAddress)     //不符合modbustcp协议格式,设备地址不符合,长度不对
                        {
                            //消息帧错误以太网回复异常码
                            MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);
                            Modbustcp.ExxeptionResponse(MbtcpTid, Commentclass.CommentMbAddress, Commentclass.CommentTempResive[7], 0x03);
                        }
                        else
                        {
                            switch (Commentclass.CommentTempResive[7])    //功能码
                            {
                                case 0x03://读多个寄存器
                                    MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);                                                              //事务处理标识符
                                    ResgierAddress = (ushort)(Commentclass.CommentTempResive[8] * 256 + Commentclass.CommentTempResive[9]);
                                    Commentclass.CommentBackAddress[0] = ResgierAddress;//寄存器地址
                                    ResgierQuality = (ushort)(Commentclass.CommentTempResive[10] * 256 + Commentclass.CommentTempResive[11]);
                                    Commentclass.CommentBackAddress[1] = ResgierQuality; //寄存器数量
                                    break;

                                case 0x06://写单个寄存器
                                    MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);
                                    ResgierAddress = (ushort)(Commentclass.CommentTempResive[8] * 256 + Commentclass.CommentTempResive[9]);
                                    Commentclass.CommentBackAddress[0] = ResgierAddress;
                                    ResgierValues = (ushort)(Commentclass.CommentTempResive[10] * 256 + Commentclass.CommentTempResive[11]);
                                    Commentclass.CommentBackAddress[1] = ResgierValues;
                                    Commentclass.CommentResEventTemp = Commentclass.CommentBackAddress[1];                          //获取06功能码的数据内容
                                    if (AcIsRuning)
                                    {
                                        if (Commentclass.CommentResEventTemp == Commentclass.TranspEnd)//程序升级完成
                                        {
                                            TranspCodeTimerout.Stop();
                                            AckTxRxThread.Abort();//关闭线程                                              
                                            Commentclass.CommentResEventTemp = 0x0000;
                                            Thread.Sleep(20);
                                            Commentclass.CommentAckContinue = true;
                                            Commentclass.SocketBreakFlag = true;
                                            Commentclass.CommentBackack = SocketLi.SocketBreakConnect(Commentclass.CommentIP);
                                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                                            {
                                                for (int i = 0; i < 100; i++)
                                                {
                                                    Thread.Sleep(60);
                                                    Application.DoEvents();
                                                }
                                                Console.WriteLine("程序升级完成");
                                                Invoke(LableMessageDisplay, "状态:", "程序升级完成");
                                                DownloadRate.Value = 0x00;
                                                //
                                                textBox2.Text = (Convert.ToInt32(textBox2.Text) + 1).ToString();
                                                EnableButton();                                               
                                            })));
                                            RxExplaThread.Abort();
                                        }
                                    }
                                    break;

                                case 0x10://写多个寄存器
                                    MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);
                                    ResgierAddress = (ushort)(Commentclass.CommentTempResive[8] * 256 + Commentclass.CommentTempResive[9]);
                                    Commentclass.CommentBackAddress[0] = ResgierAddress;//寄存器地址
                                    ResgierQuality = (ushort)(Commentclass.CommentTempResive[10] * 256 + Commentclass.CommentTempResive[11]);
                                    Commentclass.CommentBackAddress[1] = ResgierQuality;//寄存器数量
                                    if (AcIsRuning)
                                    {
                                        if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress)&& Commentclass.CommentAckContinue == false)
                                        {
                                            // Thread.Sleep(5);
                                            //LiDealy.delayUs(0.5);                                            
                                            TranspCodeTimerout.Stop();                                         
                                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                                            {
                                                Console.WriteLine("接收响应");
                                                Console.WriteLine($"Commentclass.CommentAckContinue  = {Commentclass.CommentAckContinue }.");
                                            })));
                                            Commentclass.CommentAckContinue = true;
                                        }
                                    }
                                    break;
                            }
                            Array.Clear(Commentclass.CommentTempResive, 0, Commentclass.CommentTempResive.Length);//清零
                        }                       
                    }
                    else
                    {
                        Array.Clear(Commentclass.CommentTempResive, 0, Commentclass.CommentTempResive.Length);//清零
                        MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                    }


                }
                catch (ThreadAbortException ex)
                {
                    ;//不处理
                }
                catch (Exception err)
                {
                    //Array.Clear(Commentclass.CommentTempResive, 0, Commentclass.CommentTempResive.Length);//清零
                }
              
            }
           
        }
        #endregion

        #region 状态标签显示线程
        private void LableMessageDisplay_Msg(string str1, string str2)
        {

            //Invoke(LableMessageDisplay,"状态","");
            statelable.Text = str1 + str2;
        }
        #endregion

        #region 应答式收发线程
        private void AckTxRxThreadHandIqn()
        {           
            try
            {
                while(AcIsRuning)
                {
                    
                    //  Console.WriteLine($"代码包发送线程正在进行");
                    if (Commentclass.CommentAckContinue)
                    {
                        //先发送
                        if (Commentclass.CommentStepI < Commentclass.CommentPagenum)
                        {
                            Commentclass.CommentCodeDownloaeDuty = (((double)Commentclass.CommentStepI / (double)Commentclass.CommentPagenum) * 100);
                            Invoke(LableMessageDisplay, "程序下载中:", Commentclass.CommentCodeDownloaeDuty.ToString("0.00") + "%");
                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                            {
                                DownloadRate.Value = (int)(Commentclass.CommentCodeDownloaeDuty * 10);
                                //DownloadRate.Update();
                                Application.DoEvents();
                            })));

                            Buffer.BlockCopy(Commentclass.CommentFileData, (int)(Commentclass.CommentStepI * 128), Commentclass.CommentCodePackTemp, 0, 128);
                          
                            Commentclass.CommentCrcValue = StartsCRC.CRC16(Commentclass.CommentCodePackTemp, 128);
                            Commentclass.CommentCrcCheck[0] = (byte)(Commentclass.CommentCrcValue >> 8);        //crch
                            Commentclass.CommentCrcCheck[1] = (byte)Commentclass.CommentCrcValue;               //crcl
                            Buffer.BlockCopy(Commentclass.CommentBohead, 0, Commentclass.CommentCodePack, 0, Commentclass.CommentBohead.Length);  //将Bohead放到Codepack之中
                            Buffer.BlockCopy(Commentclass.CommentCodePackTemp, 0, Commentclass.CommentCodePack, Commentclass.CommentBohead.Length, Commentclass.CommentCodePackTemp.Length);//把128个字节的代码包放到整体代码包之中
                            Buffer.BlockCopy(Commentclass.CommentCrcCheck, 0, Commentclass.CommentCodePack, (Commentclass.CommentBohead.Length + Commentclass.CommentCodePackTemp.Length), Commentclass.CommentCrcCheck.Length);//拼接CRC               
                            
                            Commentclass.CommentStepI = Commentclass.CommentStepI + 1;
    
                            Commentclass.CommentBackack = Modbustcp.ByteConcertUshortSum(Commentclass.CommentCodePack, out Commentclass.CommentTcpCodePack);
                            //if (Commentclass.CommentBackack != "ok")
                            //{
                            //    MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示");
                            //    AckTxRxThread.Abort();//关闭线程
                            //}
                            //以太网发送 
                            Commentclass.CommentAckContinue = false;    //去做接收然后做解析
                            Console.WriteLine($"Commentclass.CommentAckContinue  = {Commentclass.CommentAckContinue }.");
                            TranspCodeTimerout.Start();//开启超时判断

                            Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x42, Commentclass.CommentTcpCodePack);
                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                            {
                                if (Admin_root)
                                    this.databox.AppendText("Send"+ "(" + DateTime.Now.ToString("ss fff")+ ")" + ":"  + StartsCRC.zftring(Commentclass.CommentDisplayTemp)+ "\r\n");
                            })));

                            Commentclass.CommentBohead[0] = (byte)(Commentclass.CommentBohead[0] + 1);
                            Commentclass.CommentBohead[1] = (byte)(0Xff - Commentclass.CommentBohead[0]);

                        }
                        else if (Commentclass.CommentRemaind > 0)
                        {
                            Buffer.BlockCopy(Commentclass.CommentFileData, (int)(Commentclass.CommentPagenum * 128), Commentclass.CommentCodePackTemp, 0, (int)Commentclass.CommentRemaind);
                            for (; Commentclass.CommentRemaind < 128; Commentclass.CommentRemaind++)//不够128个字节，补充0xff
                            {
                                Commentclass.CommentCodePackTemp[Commentclass.CommentRemaind] = 0xFF;
                            }
                            Commentclass.CommentCrcValue = StartsCRC.CRC16(Commentclass.CommentCodePackTemp, 128);
                            Commentclass.CommentCrcCheck[0] = (byte)(Commentclass.CommentCrcValue >> 8);//crch
                            Commentclass.CommentCrcCheck[1] = (byte)Commentclass.CommentCrcValue;//crcl
                            Buffer.BlockCopy(Commentclass.CommentBohead, 0, Commentclass.CommentCodePack, 0, Commentclass.CommentBohead.Length);  //将Bohead放到Codepack之中
                            Buffer.BlockCopy(Commentclass.CommentCodePackTemp, 0, Commentclass.CommentCodePack, Commentclass.CommentBohead.Length, Commentclass.CommentCodePackTemp.Length);//把128个字节的代码包放到整体代码包之中
                            Buffer.BlockCopy(Commentclass.CommentCrcCheck, 0, Commentclass.CommentCodePack, (Commentclass.CommentBohead.Length + Commentclass.CommentCodePackTemp.Length), Commentclass.CommentCrcCheck.Length);//拼接CRC
                                                                                                                                                                                                                                //将8位转换成为16位
                            Commentclass.CommentCodeDownloaeDuty = (((double)Commentclass.CommentStepI / (double)Commentclass.CommentPagenum) * 100);
                            
                            
                            Invoke(LableMessageDisplay, "程序下载中:", Commentclass.CommentCodeDownloaeDuty.ToString("0.00") + "%");

                            
                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                            {
                                DownloadRate.Value = (int)(Commentclass.CommentCodeDownloaeDuty * 10);
                                DownloadRate.Update();
                                Application.DoEvents();
                            })));
                            

                            Commentclass.CommentBackack = Modbustcp.ByteConcertUshortSum(Commentclass.CommentCodePack, out Commentclass.CommentTcpCodePack);
                            //if (Commentclass.CommentBackack != "ok")
                            //{
                            //    MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示");
                            //    AckTxRxThread.Abort();//关闭线程
                            //}
                            //以太网发送 
                            Commentclass.CommentAckContinue = false;    //去做接收然后做解析
                            Console.WriteLine($"Commentclass.CommentAckContinue  = {Commentclass.CommentAckContinue }.");
                            TranspCodeTimerout.Start();//开启超时判断

                            Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x42, Commentclass.CommentTcpCodePack);

                            
                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                            {
                                if (Admin_root)
                                    this.databox.AppendText("Send"+ "(" + DateTime.Now.ToString("ss fff")+ ")" + ":"  + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                            })));

                            Commentclass.CommentBohead[0] = (byte)(Commentclass.CommentBohead[0] + 1);
                            Commentclass.CommentBohead[1] = (byte)(0Xff - Commentclass.CommentBohead[0]);
                            Commentclass.CommentRemaind = 0x00;

                        }
                        else //告诉主控ARM代码包发送完毕
                        {
                            Commentclass.CommentAckContinue = false;
                            TranspCodeTimerout.Start();
                            Modbustcp.MasterWriteSingleRegister(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, Commentclass.TranspEnd);
                            
                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                            {
                                if (Admin_root)
                                    this.databox.AppendText("Send"+ "(" + DateTime.Now.ToString("ss fff")+ ")" + ":"  + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                            })));

                        }
                        //Console.WriteLine("123456789");
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                ;//不处理
            }
            catch (Exception err)
            {
                Commentclass.CommentAckContinue = false;    //去做接收然后做解析
                Console.WriteLine($"Commentclass.CommentAckContinue  = {Commentclass.CommentAckContinue }.");
                TranspCodeTimerout.Start();//开启超时判断
                Console.WriteLine($"step_i = {err.ToString()}.");
                //MessageBoxMidle.Show(this, err.Message, "");
            }  
            
        }
        #endregion

        #region 步骤判断执行线程（命令最多重发6次）
        private void StepConThreadHandIqn()
        {
            try
            {
                while (StIsRuning)
                {
                 //   Console.WriteLine($"步骤判断执行线程正在进行");
                    switch (Commentclass.CommentStepNow)
                    {
                        #region 0x00
                        case 0x00:      //判断主控ARM 是否处于刷机状态，如果不是则执行步骤0x01,否则执行步骤0x04
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Start();
                                })));                               
                                Commentclass.CommentTimerState = true;
                                Invoke(LableMessageDisplay, "状态:", "初始化等待...");
                            }
                            if (!Commentclass.CommentUpgradeState)
                            {
                                if (Commentclass.CommentResEventTemp == Commentclass.MainContrast)
                                {
                                    Commentclass.CommentResEventTemp = 0x0000;
                                    Commentclass.CommentStepNow = 0X04;
                                    Commentclass.CommentTimerState = false;
                                    Commentclass.CommentUpgradeState = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        StateControl_timer.Stop();
                                    })));
                                    
                                }
                            }
                            else
                            {
                                //判断如果刷机对象是主控与充电板则执行步骤0x01,刷机对象为DSP则执行

                                Commentclass.CommentStepNow = 0X01;
                                Commentclass.CommentTryCount = 0x00;
                                Commentclass.CommentUpgradeState = false;
                                Commentclass.CommentTimerState = false;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Stop();
                                })));
                            }
                            break;
                        #endregion
                        case 0x01:      //发送第一帧跳转命令，确认响应然后执行0x02,
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    if (Commentclass.CommentUpgrade == 0x01)//如果刷机对象是DSP
                                    {
                                        Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, Commentclass.CommentAppResgitAddr, 0x01, Commentclass.CommentOrderThrid);//10功能码
                                    }
                                    else//刷机对象是主控ARM和充电板ARM
                                    {
                                        Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, Commentclass.CommentAppResgitAddr, 0x01, Commentclass.CommentOrderFirst);//10功能码
                                    }
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        Console.WriteLine($"发送第一帧命令");

                                        Invoke(LableMessageDisplay, "状态:", "跳转命令发送中...");
                                        if (Admin_root)
                                            this.databox.AppendText("Send" + "(" + DateTime.Now.ToString("ss fff") + ")" + ":" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                    })));                          
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;

                                if (FG == false)
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        AckTimerOut_timer.Stop();
                                        if ((Commentclass.CommentBackAddress[0] & 0x0100) == 0x0100)
                                        {
                                            MessageBox.Show("ENPO为使能状态(DI0 = 1)，不允许以太网升级！", "提示");
                                        }
                                        MessageBoxMidle.Show(this, "与DSP第1次握手失败，请重新下载！", "提示！");//尝试主控板进入刷机第一帧命令无响应
                                        EnableButton();
                                    })));
                                }
                                else
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        AckTimerOut_timer.Stop();
                                        this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + "与DSP第1次握手失败，请重新下载！" + "\r\n");
                                        //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                        for (int i = 0; i < 100; i++)
                                        {
                                            Thread.Sleep(50);
                                            Application.DoEvents();
                                        }
                                        EnableButton();
                                    })));
    
                                }
      
                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress) && Commentclass.CommentTryCount > 0)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X02;
                                Commentclass.CommentTryCount = 0x00;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                })));

                            }

                            
                            break;
                        case 0x02:      //发送第二帧跳转命令，确认响应后执行0x12，去断开以太网重连
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    if (Commentclass.CommentUpgrade == 0x01)//如果刷机对象是DSP
                                    {
                                        Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, Commentclass.CommentAppResgitAddr, 0x01, Commentclass.CommentOrderFourt);//10功能码
                                    }
                                    else//刷机对象是主控板额充电板
                                    {
                                        Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, Commentclass.CommentAppResgitAddr, 0x01, Commentclass.CommentOrderSecon);//10功能码
                                    }
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        Console.WriteLine($"发送第二帧命令");
                                        if (Admin_root)
                                            this.databox.AppendText("Send" + "(" + DateTime.Now.ToString("ss fff") + ")" + ":" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                    })));
                                
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;

                                if (FG == false)
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        AckTimerOut_timer.Stop();
                                        MessageBoxMidle.Show(this, "与DSP第2次握手失败，请重新下载！", "提示！");//尝试主控板进入刷机第一帧命令无响应
                                        EnableButton();
                                    })));
                                }
                                else
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        AckTimerOut_timer.Stop();
                                        this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + "与DSP第2次握手失败，请重新下载！" + "\r\n");
                                        //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                        for (int i = 0; i < 100; i++)
                                        {
                                            Thread.Sleep(50);
                                            Application.DoEvents();
                                        }
                                        EnableButton();
                                    })));

                                }
     
                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress) && Commentclass.CommentTryCount > 0)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X04;
                                // Commentclass.CommentStepNow = 0X12;     //断开以太网重连                                   
                                Commentclass.CommentTryCount = 0x00;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                })));
                                //一键擦除
                                if (Commentclass.CommentUpgrade == 0x03)
                                {
                                    Commentclass.CommentUpgrade = 0x00;
                                    Commentclass.CommentStepNow = 0X10;
                                    Invoke(LableMessageDisplay, "状态:", "ARM程序擦除完成！");
                                }
                            }
                            break;
                        case 0x03:      //确认主控板是否处于刷机状态，执行0x04
                            if (!Commentclass.CommentTimerState)
                            {
                                StateControl_timer.Start();
                                Commentclass.CommentTimerState = true;
                            }
                            if (!Commentclass.CommentUpgradeState)
                            {
                                if (Commentclass.CommentResEventTemp == Commentclass.MainContrast)
                                {
                                    Commentclass.CommentResEventTemp = 0x0000;
                                    Commentclass.CommentStepNow = 0X04;
                                    Commentclass.CommentTimerState = false;
                                    Commentclass.CommentUpgradeState = false;
                                    StateControl_timer.Stop();
                                }
                            }
                            else
                            {
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentUpgradeState = false;
                                Commentclass.CommentTimerState = false;
                                StateControl_timer.Stop();
                                MessageBoxMidle.Show(this, "命令主控板ARM进入刷机状态失败。", "提示！");
                                EnableButton();
                            }
                            break;
                        case 0x04:      //判断刷机对象是否为自己如果是执行0x13 ，如果不是执行0x05
                            if (Commentclass.CommentUpgrade == 0x00|| Commentclass.CommentUpgrade == 0X03)//刷机对象为自己
                            {
                                Commentclass.CommentStepNow = 0x13;
                                Invoke(LableMessageDisplay, "状态:", "延时等待3S...");
                                for (int i = 0; i < 100; i++)
                                {
                                    Thread.Sleep(30);
                                    Application.DoEvents();
                                }
                            }
                            else
                            {
                                Commentclass.CommentStepNow = 0x05;
                            }
                            break;
                        case 0x05:      //告诉主控板ARM刷机对象,执行步骤0x06
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x03)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    switch (Commentclass.CommentUpgrade)
                                    {
                                        case 0x01:
                                            Modbustcp.MasterWriteSingleRegister(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, Commentclass.TranspAimdsp);//06功能码
                                            Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                            {
                                                if (Admin_root)
                                                    this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                            })));
                                           
                                            break;
                                        case 0x02:
                                            Modbustcp.MasterWriteSingleRegister(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, Commentclass.TranspAimcdb);//06功能码
                                            Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                            {
                                                if (Admin_root)
                                                    this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                            })));
                                            break;
                                    }
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                
                                Commentclass.CommentTryCount = 0x00;
                                Commentclass.CommentStepNow = 0x10;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                    MessageBoxMidle.Show(this, "告知主控ARM刷机对象超时。", "提示！");
                                    EnableButton();
                                })));

                            }
                            if (Commentclass.CommentResEventTemp == Commentclass.TranspAimdsp || Commentclass.CommentResEventTemp == Commentclass.TranspAimcdb)
                            {
                                Commentclass.CommentResEventTemp = 0x0000;
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentTryCount = 0x00;
                                AckTimerOut_timer.Stop();
                                Commentclass.CommentStepNow = 0x06;
                            }
                            break;
                        case 0x06:      //发送命令让主控进入透传状态，如果刷机对象是DSP则执行0x07,充电板执行0x0b

                            switch (Commentclass.CommentUpgrade)
                            {
                                case 0x01:
                                    //Commentclass.CommentStepNow = 0x07;
                                    Commentclass.CommentStepNow = 0x08;
                                    break;
                                case 0x02:
                                    Commentclass.CommentStepNow = 0x0b;
                                    break;
                            }
                            break;

                        case 0x07:      //判断DSP是否处于刷机状态，是的话执行0x09解锁DSP ,否则执行0x08发送第一帧跳转命令                                      
                                        //if (!Commentclass.CommentTimerState)
                                        //{
                                        //    StateControl_timer.Start();
                                        //    Commentclass.CommentTimerState = true;
                                        //}
                                        //if (!Commentclass.CommentUpgradeState)
                                        //{
                                        //    if (Commentclass.CommentResEventTemp == Commentclass.DspContrast)
                                        //    {
                                        //       // if (Commentclass.CommentyComTryAck < 0x06)
                                        //       // {
                                        //            Commentclass.CommentResEventTemp = 0x0000;
                                        //            Commentclass.CommentStepNow = 0X09;
                                        //            Commentclass.CommentTimerState = false;
                                        //            Commentclass.CommentUpgradeState = false;
                                        //            StateControl_timer.Stop();
                                        //           //Commentclass.CommentyComTryAck++;
                                        //       // }
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    //Commentclass.CommentyComTryAck = 0;
                                        //    Commentclass.CommentStepNow = 0X08;
                                        //    Commentclass.CommentUpgradeState = false;
                                        //    Commentclass.CommentTimerState = false;
                                        //    StateControl_timer.Stop();
                                        //}
                            break;
                        case 0x08:      //发送DSP跳转第一帧命令，执行0x14发送第二帧跳转命令
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                               
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x03, Commentclass.DSPCommentOrderFirst);//10功能码
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        if (Admin_root)
                                            this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                    })));
                                   
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;
                               
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                    MessageBoxMidle.Show(this, "尝试DSP进入刷机第一帧命令无响应。", "提示！");
                                    EnableButton();
                                })));

                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X14;
                                Commentclass.CommentTryCount = 0x00;
                                AckTimerOut_timer.Stop();
                            }
                            break;
                        case 0x09:      //DSP解锁，解锁成功就去擦除执行步骤：0x11
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x03, Commentclass.CommentDspOrderUnlock);//10功能码
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        if (Admin_root)
                                            this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                        Invoke(LableMessageDisplay, "状态:", "DSP解锁中...");
                                    })));
                     
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;
                                
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                    MessageBoxMidle.Show(this, "DSP解锁失败。", "提示！");
                                    EnableButton();
                                })));

                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress) && Commentclass.CommentTryCount > 0)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                    if (i < 2)
                                    {
                                        Commentclass.CommentBackAddress[i] = 0xffff;
                                    }
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X11;
                                Commentclass.CommentTryCount = 0x00;
                                Thread.Sleep(50);
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                })));
                               
                            }
                            break;
                        case 0x0a://确认DSP是否处于刷机模式，成功就去解锁操作，失败就失败 
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (!Commentclass.CommentUpgradeState)
                            {
                                if (Commentclass.CommentResEventTemp == Commentclass.DspContrast)
                                {
                                    Commentclass.CommentResEventTemp = 0x0000;
                                    Commentclass.CommentStepNow = 0X09;
                                    Commentclass.CommentTimerState = false;
                                    Commentclass.CommentUpgradeState = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        StateControl_timer.Stop();
                                    })));
                                    
                                }
                            }
                            else
                            {
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentUpgradeState = false;
                                Commentclass.CommentTimerState = false;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Stop();
                                })));
                            }
                            break;
                        case 0x0b:      //判断充电板是否处于刷机状态，如果是执行0xof  ，否则执行0x0c
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (!Commentclass.CommentUpgradeState)
                            {
                                if (Commentclass.CommentResEventTemp == Commentclass.CdbContrast)
                                {
                                    Commentclass.CommentResEventTemp = 0x0000;
                                    Commentclass.CommentStepNow = 0X0f;
                                    Commentclass.CommentTimerState = false;
                                    Commentclass.CommentUpgradeState = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        StateControl_timer.Stop();
                                    })));
                                }
                            }
                            else
                            {
                                Commentclass.CommentStepNow = 0X0C;
                                Commentclass.CommentUpgradeState = false;
                                Commentclass.CommentTimerState = false;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Stop();
                                })));
                            }

                            break;
                        case 0x0c:      //发送充电板第一帧跳转命令，执行0x0d
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x03, Commentclass.CommentCdbOrderFirst);//10功能码
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        if (Admin_root)
                                            this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                    })));
                                    
                                }
                                Invoke(LableMessageDisplay, "状态:", "充电器程序跳转中...");
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;
                                
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                    MessageBoxMidle.Show(this, "尝试充电板进入刷机第一帧命令无响应。", "提示！");
                                    EnableButton();
                                })));

                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X0D;
                                Commentclass.CommentTryCount = 0x00;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                })));
      
                            }

                            break;
                        case 0x0d:      //发送充电板第二帧跳转命令，执行0x0e
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x03, Commentclass.CommentCdbrderSecon);//10功能码
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        if (Admin_root)
                                            this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                    })));
                                    
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;
                                
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                    MessageBoxMidle.Show(this, "尝试充电板进入刷机第二帧命令无响应。", "提示！");
                                    EnableButton();
                                })));
         ;
                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X0e;
                                Commentclass.CommentTryCount = 0x00;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                })));
       
                            }
                            break;
                        case 0x0e:      //判断充电板是否处于刷机状态，是就执行0x0f
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (!Commentclass.CommentUpgradeState)
                            {
                                if (Commentclass.CommentResEventTemp == Commentclass.CdbContrast)
                                {
                                    Commentclass.CommentResEventTemp = 0x0000;
                                    Commentclass.CommentStepNow = 0X0F;
                                    Commentclass.CommentTimerState = false;
                                    Commentclass.CommentUpgradeState = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        StateControl_timer.Stop();
                                    })));
                                    
                                }
                            }
                            else
                            {
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentUpgradeState = false;
                                Commentclass.CommentTimerState = false;
                                
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    StateControl_timer.Stop();
                                    MessageBoxMidle.Show(this, "命令充电板进入刷机状态失败。", "提示！");
                                    EnableButton();
                                })));
                            }
                            break;
                        case 0x0f:      //开启应答式程序包发送线程
                            Invoke(LableMessageDisplay, "状态:", "准备代码传输...");
                            Commentclass.CommentStepNow = 0X00;
                            AcIsRuning = true;
                            AckTxRxThread = new Thread(AckTxRxThreadHandIqn);
                            AckTxRxThread.IsBackground = true;
                            AckTxRxThread.Start();
                            StepConThread.Abort();
                            break;
                        case 0x10:      //空任务
                            Commentclass.CommentStepNow = 0X00;
                            RxIsRuning = false;
                            RxExplaThread.Abort();
                            Array.Clear(Commentclass.CommentTempResive, 0, Commentclass.CommentTempResive.Length);//清零
                            Commentclass.SocketBreakFlag = true;
                            Commentclass.CommentBackack = SocketLi.SocketBreakConnect(Commentclass.CommentIP);
                            Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                            {
                                EnableButton();                              
                            })));
                            StepConThread.Abort();
                            break;
                        case 0x11:      //发送DSP擦除Flash的指令，擦除完成执行0x0f(先发送解锁命令)

                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                                Commentclass.CommentDspOrderErase[1] = Commentclass.CommentDspAbraseNum;    //擦除扇区数
                                                                                                            //计算CRC的值
                                Commentclass.CommentDspOrderErase[2] = StartsCRC.UnShortCRC16(Commentclass.CommentDspOrderErase, 0x02);
                                //以太网发送
                                Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x03, Commentclass.CommentDspOrderErase);//10功能码                           
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    if (Admin_root)
                                        this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                })));
                               
                                Invoke(LableMessageDisplay, "状态:", "Flash擦除中...");
                            }
                            if (Commentclass.CommentAckTimerout)
                            {
                                Invoke(LableMessageDisplay, "状态:", "Flash擦除中:" + Commentclass.CommentDspAbraseTimerOut.ToString() + "s");
                                Commentclass.CommentDspAbraseTimerOut++;//加一次两百毫秒
                                Commentclass.CommentAckTimerout = false;
                                if (Commentclass.CommentDspAbraseTimerOut == (20 * Commentclass.CommentDspAbraseNum + (Commentclass.CommentDspAbraseNum - 1) * 2))
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        AckTimerOut_timer.Stop();
                                    })));
                                    
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        DialogResult messdr = MessageBoxMidle.Show(this, "擦除超时！是否重试擦除？", "提示", MessageBoxButtons.YesNo);
                                        if (messdr == DialogResult.Yes)
                                        {
                                            Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x03, Commentclass.CommentDspOrderErase);//10功能码           
                                            if (Admin_root)
                                                this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                            AckTimerOut_timer.Start();
                                            Commentclass.CommentDspAbraseTimerOut = 0x00;
                                        }
                                        else
                                        {
                                            EnableButton();
                                            Commentclass.CommentAckTimerout = false;
                                            Commentclass.CommentTimerState = false;
                                            Commentclass.CommentStepNow = 0X10;
                                        }
                                    })));
         
                                }
                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                    //if (i < 2)
                                    //{
                                    //    Commentclass.CommentBackAddress[i] = 0xffff;
                                    //}
                                }
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                    WaiteOneSeconTimer.Start();
                                })));                    
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X15;//这里做了一个等待1s的延时，确保DSP反应过来
                            }

                            break;
                        case 0x12:      //跳转前断开以太网重连，跳转后重连，然后主控板再判断是否处于刷机模式
                            if (!Commentclass.SocketBreakFlag)
                            {
                                Thread.Sleep(20);
                                RxExplaThread.Abort();
                                Commentclass.CommentBackack = SocketLi.SocketBreakConnect(Commentclass.CommentIP);
                                Commentclass.SocketBreakFlag = true;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    SocketConnectTimer.Start();
                                })));
                                
                                Invoke(LableMessageDisplay, "状态:", "以太网重连中，请等待...");
                            }
                            if (Commentclass.CommentSocketConnectTimerCount == 0x00)//进行计时重连5s
                            {
                                Invoke(LableMessageDisplay, "状态:", "命令发送中...");
                                //先 ping 一下再连接
                                Commentclass.CommentBackack = SocketLi.SocketConnect(Commentclass.CommentIP, Commentclass.CommentNetPort);//连接以太网
                                if (Commentclass.CommentBackack == "ok")
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        SocketConnectTimer.Stop();
                                    })));
                                    
                                    Commentclass.CommentStepNow = 0X03;//(正确步骤)
                                                                       //Commentclass.CommentStepNow = 0X04;//(测试用)
                                    Commentclass.CommentSocketConnectTimerCount = 0x05;
                                    Commentclass.SocketBreakFlag = false;
                                    //开启以太网接收线程

                                    RxExplaThread = new Thread(RxExplaThreadHandIqn);
                                    RxExplaThread.IsBackground = true;
                                    RxExplaThread.Start();
                                }
                                else
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        SocketConnectTimer.Stop();
                                    })));
                                    
                                    Commentclass.CommentSocketConnectTimerCount = 0x05;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示");
                                    })));
                                    
                                    Commentclass.CommentStepNow = 0X10;
                                    // Commentclass.SocketBreakFlag = false;
                                }
                            }
                            if (Commentclass.CommentBackack != "ok")
                            {
                                Console.WriteLine($"step_i = {Commentclass.CommentBackack}.");
                                // MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                            }
                            break;
                        case 0x13:// 0x01aa;    
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    Modbustcp.MasterWriteSingleRegister(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, Commentclass.TranspMyself);//06功能码
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        if (Admin_root)
                                            this.databox.AppendText("Send" + "(" + DateTime.Now.ToString("ss fff") + ")" + ":" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                    })));
                                    
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;


                                if (FG == false)
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        AckTimerOut_timer.Stop();
                                        MessageBoxMidle.Show(this, "DSP未回复刷机对象命令，请重新下载！", "提示！");//尝试主控板进入刷机第一帧命令无响应
                                        EnableButton();
                                    })));
                                }
                                else
                                {
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        AckTimerOut_timer.Stop();
                                        this.textBox1.AppendText("(" + DateTime.Now.ToString("hh:mm:ss fff") + ")" + ":" + "DSP未回复刷机对象命令，请重新下载！" + "\r\n");
                                        //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                                        for (int i = 0; i < 100; i++)
                                        {
                                            Thread.Sleep(50);
                                            Application.DoEvents();
                                        }
                                        EnableButton();
                                    })));

                                }
                        
                            }
                            if (Commentclass.CommentResEventTemp == Commentclass.TranspMyself && Commentclass.CommentTryCount > 0x00)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X0F;
                                Commentclass.CommentTryCount = 0x00;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                })));                                    
                            }
                            break;
                        case 0x14://发送DSP第二帧跳转命令，失败则跳转0x10.,成功跳转到0x0a确认DSP是否在刷机模式
                            if (!Commentclass.CommentTimerState)
                            {
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Start();
                                })));
                                
                                Commentclass.CommentTimerState = true;
                            }
                            if (Commentclass.CommentTryCount < 0x06)
                            {
                                if (Commentclass.CommentAckTimerout)
                                {
                                    Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x03, Commentclass.DSPCommentOrderFirst);//10功能码
                                    Commentclass.CommentTryCount++;
                                    Commentclass.CommentAckTimerout = false;
                                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                    {
                                        if (Admin_root)
                                            this.databox.AppendText("Send:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                                    })));
                                   
                                }
                            }
                            else
                            {
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X10;
                                Commentclass.CommentTryCount = 0x00;
                                
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                    MessageBoxMidle.Show(this, "DSP跳转失败；\r\n请检查通信接线是否正确。", "提示！");
                                    EnableButton();
                                })));
                  
                            }
                            if (Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Commentclass.CommentSendRegist[i] = 0xff;
                                }
                                Commentclass.CommentAckTimerout = false;
                                Commentclass.CommentTimerState = false;
                                Commentclass.CommentStepNow = 0X0a;
                                Commentclass.CommentTryCount = 0x00;
                                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                                {
                                    AckTimerOut_timer.Stop();
                                })));
                            }
                            break;
                        case 0x15://延时1s，DSP专用，延时1s后开始执行DSP刷机

                            if (DspWaiteOneSecondFlag)
                            {
                                Commentclass.CommentStepNow = 0X0f;
                            }
                            break;
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                ;//不处理
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(this, err.Message, "");
            }       
        }

        #endregion
        #endregion

        #region 用来显示关闭线程的消息提示
        private void MessageMidle_Msg(string str)
        {
            MessageBoxMidle.Show(this, str, "提示！");
        }
        #endregion

        #region 数据显示的委托
        private void DataDisplayRecMsg(string str)
        {
            this.databox.AppendText(str + "\r\n");
        }
        #endregion

        #region 充电板透传与刷机模式下的切换
        private void ModelBox_Click(object sender, EventArgs e)
        {
            try 
            {
                Int16 vari = 0;
                vari = Convert.ToInt16(((ComboBox)sender).SelectedIndex.ToString());
                Console.WriteLine($"vari = {vari}.");
                switch (vari)
                {
                    case 0x00:      //透传模式
                        break;
                    case 0x01:      //单板模式
                        ModelBox.SelectedIndex = 0x00;
                        this.Hide();
                        if (Commentclass.fm2.IsDisposed)
                        {
                            Commentclass.fm2 = new Form2();  //充电板单板刷机模式
                        }
                        Commentclass.fm2.Show();
                        break;
                }
            }
            catch(Exception err)
            {
                MessageBoxMidle.Show(this, err.Message);
            }
        }
        #endregion

        #region 失能按键
        bool Is_loading;
        private void DisableButton()
        {
            Is_loading = true;
            menuStrip1.Enabled = false;
            btnlink.Enabled = false;
            btnlabview.Enabled = false;
            this.groupBox2.Enabled = false;
            this.groupBox3.Enabled = false;
        }
        #endregion

        #region 使能按键
        private void EnableButton()
        {
            Is_loading = false;
            btnlink.Enabled = true;
            btnlabview.Enabled = true;
            menuStrip1.Enabled = true;
            this.groupBox2.Enabled = true;
            this.groupBox3.Enabled = true;
            
        }
        #endregion

        #region 变量初始化函数
        public void MainPrarInit()
        {
            Commentclass.CommentUpgrade = 0X00;
            Commentclass.CommentBohead[0] = 0x01;
            Commentclass.CommentBohead[1] = 0xfe;
            Commentclass.CommentStepI = 0x00;
            AcIsRuning = false;
            RxIsRuning = true;
            Commentclass.CommentIP = ipaddres.Text;
            Commentclass.CommentAckContinue = true;
            Commentclass.CommentDspAbraseTimerOut = 0;
            Commentclass.CommentTryCount = 0;
            DspWaiteOneSecondFlag = false;
        }
        #endregion

        #region 回车函数
        private void ipaddres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ipaddres.Text == "1024")
                {
                    ipaddres.Text = "192.168.1.15";                   
                    AmpliyUI();
                    //if (AboutDetial.IsDisposed)
                    //{
                    //    AboutDetial = new Monitor();
                    //    AboutDetial.Location = new Point(Commentclass.windowX + 424, Commentclass.windowY);
                    //    AboutDetial.Show();
                    //}
                    //else
                    //{
                    //    AboutDetial.Location = new Point(Commentclass.windowX + 424, Commentclass.windowY);
                    //    AboutDetial.Show();
                    //}
                }
                else if (ipaddres.Text == "2048")
                {
                    ipaddres.Text = "192.168.1.15";
                    ContractUI();
                }
                Commentclass.CommentIP = ipaddres.Text;
                RegistryKeyLi.WriteRegistryKey(Commentclass.CommentIP, Commentclass.CommentPublicResgistryKeyPath, "CommentIP");
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

        /// <summary>
        /// 权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathtextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (pathtextBox.Text == "666666")
                {
                    pathtextBox.Clear();
                    if (Commentclass.set.IsDisposed)
                    {
                        Setting set = new Setting();
                        set.Show();
                    }
                    else
                    {
                        Commentclass.set.Show();
                    }
                }
                
            }
        }

        private void DisplayTimer_Tick(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 定时器1s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaiteOneSeconTimer_Tick(object sender, EventArgs e)
        {
            DspWaiteOneSecondFlag = true;
            WaiteOneSeconTimer.Stop();
        }

        private void 一键擦除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commentclass.CommentUpgrade = 0x03;
            btnlink.PerformClick();
        }

        #endregion

        #region 文件拖放
        private void pathtextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                pathtextBox.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                OpenFileDialog ofd = new OpenFileDialog();

                if (Path.GetExtension(pathtextBox.Text) == ".bin"|| Path.GetExtension(pathtextBox.Text) == ".hex")//如果是.bin文件
                {
                    //判断刷机目标，存进相应的路径
                    switch (Commentclass.CommentUpgrade)
                    {
                        case 0x00://主控板
                            Commentclass.CommentRuningKyeName = Commentclass.CommentMainResgistryKeyName;
                            break;
                        case 0x01://DSP
                            Commentclass.CommentRuningKyeName = Commentclass.CommentDspResgistryKeyName;
                            break;
                        case 0x02://充电板
                            Commentclass.CommentRuningKyeName = Commentclass.CommentChargResgistryKeyName;
                            break;
                        case 0x03://主控板擦除
                            Commentclass.CommentRuningKyeName = Commentclass.CommentMainResgistryKeyName;
                            break;
                    }

                    Commentclass.CommentRealyPath = pathtextBox.Text;
                
                    //将路径存进去
                    RegistryKeyLi.WriteRegistryKey(Commentclass.CommentRealyPath, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName);
                    Console.WriteLine($"存进去的路径 = { Commentclass.CommentRealyPath}.");

                    Console.WriteLine($"导入为bin/hex文件");
                }
                else if ( Path.GetExtension(pathtextBox.Text) != ".bin"|| Path.GetExtension(pathtextBox.Text) != ".hex")
                {
                    MessageBoxMidle.Show(this, "导入文件不是*.bin/*.hex文件！", "提示");
                    pathtextBox.Clear();
                    return;
                }
            }
            catch (Exception err)
            {

            }
        }

        #endregion

        #region 关闭窗体
        private void Form7_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnlink.Enabled == false)
                e.Cancel = true;
            else              
                Application.Exit();
        
        }
        #endregion

        #region 操作记录文本
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UInt16 tag = Convert.ToUInt16(((ToolStripMenuItem)sender).Tag.ToString());
            switch (tag)
            {
                case 0x00://清除
                    this.databox.Clear();
                    this.textBox1.Clear();
                    this.textBox2.Text="0";
                    this.textBox3.Text = "0";
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

        #region 循环测试 模拟按键点击
        public Boolean FG = false;

        Thread DDclickThread = null;
        private void pointlable_Click(object sender, EventArgs e)
        {
            if (FG == false)
            {
                //开步骤判断线程                        
                DDclickThread = new Thread(DDclick);
                DDclickThread.IsBackground = true;
                pointlable.BackColor = Color.Lime;
                DDclickThread.Start();
                FG = true;
            }
            else
            {
                pointlable.BackColor = Color.Yellow;
                FG = false;
                DDclickThread.Abort();                
            }
        }

        private void DDclick()
        {
            int k = 0;
            //Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
            //{
            //    textBox2.Text = "0";
            //})));
            while (FG == true)
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
                if (btnlink.Enabled == true)
                {
                    Invoke((new System.Action(() =>//c#3.0以后代替委托的新方法
                    {
                        //k++;
                        //textBox3.Text = k.ToString();
                        ////
                        //textBox3.Text = (Convert.ToInt32(textBox3.Text) + 1).ToString();
                        btnlink.PerformClick();
                    })));
                }
            }
        }
        #endregion

        private void ip_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            Commentclass.CommentIP = this.databox.Text;
            RegistryKeyLi.WriteRegistryKey(Commentclass.CommentIP, Commentclass.CommentPublicResgistryKeyPath, "CommentIP");
        }
    }
}
