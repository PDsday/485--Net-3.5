using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using View;

namespace _485刷机_Net_3._5
{
    public partial class Form8 : Form
    {
        static public int windowX = 0;
        static public int windowY = 0;

        static public byte[] file_data;
        public static byte[] CutoutFiles;                       //用于保存截取的bin文件

        public Form8()
        {
            InitializeComponent();

            this.filePath_textBox.AllowDrop = true;
        }

        #region 窗体UI逻辑

        /// <summary>
        /// 为保持和前工程窗口一致的属性，需要在窗口加载时，作基本参数，样式处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form8_Load(object sender, EventArgs e)
        {
            string msg;
            string IniFilesPath = Application.StartupPath + "\\ConnectString.ini";//ini文件的路径

            //读取注册表参数
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargResgistryKeyName, out msg);
            if (msg == "" | msg == string.Empty)
            {
                msg = "D:\\";
            }
            filePath_textBox.Text = msg;

            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, "CommentIP", out Commentclass.CommentIP);
            Console.WriteLine($"读出来的路径 = {Commentclass.CommentIP}.");
            if (Commentclass.CommentIP == "")
            {
                Commentclass.CommentIP = "192.168.1.15";
                this.ip_textBox.Text = Commentclass.CommentIP;
            }

            //读取init文件参数
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
            Commentclass.CommentMainBordCheckNum = uint.Parse(ReadINIFiles.ReadIniData("Message", "MainBordCheckCount", "None", IniFilesPath));
            Commentclass.CommentChargeBordCheckNum = uint.Parse(ReadINIFiles.ReadIniData("Message", "ChargeBordCheckCount", "None", IniFilesPath));

            //充电板目标文件的最小值
            Commentclass.ChargeBoardFileSizeMin = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "ChargeBordFileMin", "None", IniFilesPath)) * 1024;
            //充电板目标文件的最大值
            Commentclass.ChargeBoardFileSizeMax = uint.Parse(ReadINIFiles.ReadIniData("FileSizeSet", "ChargeBordFileMax", "None", IniFilesPath)) * 1024;
            //读取注册表的键值放在对应位置


            Admin_UI();
        }

        /// <summary>
        /// 注册表客户模式下隐藏住485刷机
        /// </summary>
        bool Admin_root;
        void Admin_UI()
        {
            string msg;
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName, out msg);
            Console.WriteLine("模式：" + msg);

            this.toolStripStatusLabel4.Visible = false;

            USE_log = !checkBox1.Checked;

            if (msg == "User_Client")
            {
                string temp = "注意！\r\n" +
                                "1、充电器以太网升级前，需打到手动模式！！！！\r\n" +
                                "2、充电器以太网升级使用驱动器X6A以太网接口；\r\n" +
                                "3、电脑端ip设置为192.168.1.XXX（大于20，小于254都可以）；\r\n" +
                                "4、上位机ip设置为192.168.1.拨码 + 10；\r\n";
                this.log_TextBox.AppendText(temp);
                this.log_TextBox.Select(19, 4);
                this.log_TextBox.SelectionFont = new Font("宋体", 13.5f, FontStyle.Underline | FontStyle.Bold);
                this.log_TextBox.SelectionColor = Color.Red;
                this.log_TextBox.Select(0, 0);
                //this.menuStrip1.Items[1].Visible = false;
                this.menuStrip1.Items[3].Visible = false;
                this.menuStrip1.Items[5].Visible = false;

                this.handshake1_textbox.Visible = false;
                this.handshake2_textbox.Visible = false;
                this.transport_textbox.Visible = false;
                this.reflash_textBox.Visible = false;
                this.WaitTime1_textBox.Visible = false;
                this.WaitTime2_textBox.Visible = false;
                this.WaitTime3_textBox.Visible = false;

                this.label4.Visible = false;
                this.label5.Visible = false;
                this.label6.Visible = false;
                this.label7.Visible = false;
                this.label8.Visible = false;
                this.label9.Visible = false;
                this.label10.Visible = false;

                this.checkBox1.Visible = false;

                USE_log = false;
                this.checkBox1.Checked = true;

                this.Width = 550;
                this.Height = 380;

                Admin_root = false;
            }
            else
            { 
                Admin_root = true;
                USE_log = false;
                this.checkBox1.Checked = true; //默认勾选
            }

        }

        /// <summary>
        /// 窗口跳转逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form8_Move(object sender, EventArgs e)
        {
            windowX = this.Location.X;
            windowY = this.Location.Y;
            //this.toolStripStatusLabel2.Text = fmwindowX.ToString() + "," + fmwindowY.ToString();

        }


        #endregion

        #region 窗体按键逻辑

        /// <summary>
        /// 文件浏览按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sreach_Button_Click(object sender, EventArgs e)
        {

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

            //照抄的，乱七八糟故弄玄虚写这么长，实际就是在注册表中创建一个键值方便存储地址
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName, out Commentclass.CommentReadSavePath);
            Console.WriteLine($"读出来的路径 = {Commentclass.CommentReadSavePath}.");

            Commentclass.CommentBackack = ReadFiles.CommentOpenFiles(Commentclass.CommentReadSavePath, out Commentclass.CommentRealyPath,"|*.bin");

            if (Commentclass.CommentBackack != "ok" && Commentclass.CommentBackack != "ds")
            {
                //MessageBoxMidle.Show(this, Commentclass.CommentBackack, "提示！");
                return;
            }
            else if (Commentclass.CommentBackack == "ds")
            {
                return;         //取消加载文件
            }
            else
            {
                filePath_textBox.Text = Commentclass.CommentRealyPath;
                //将路径存进去
                RegistryKeyLi.WriteRegistryKey(Commentclass.CommentRealyPath, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentRuningKyeName);
                Console.WriteLine($"存进去的路径 = {Commentclass.CommentRealyPath}.");
            }
        }



        /// <summary>
        /// 下载按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_Button_Click(object sender, EventArgs e)
        {
            //下载流程
            DialogResult messdr = MessageBoxMidle.Show(this, "请先打手动开关进入手动模式！\r\n是否已完成该操作？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (messdr != DialogResult.Yes)
            {
                return;
            }

            if (Admin_root)
            {
                log_TextBox.Text = string.Empty;
            }
            
            Console.WriteLine("下载中------");
            this.statelable.Text = "下载中...";
            log_data_update("下载中----\r\n");

            if (SocketLi.sockClient != null)
            {
                SocketLi.SocketBreakConnect(this.ip_textBox.Text);
                SocketLi.sockClient.Close(); //确保套接字处在一个关闭的状态
                SocketLi.sockClient = null;
            }
            

            Commentclass.CommentUpgrade = 0x02;

            Commentclass.CommentIP = ip_textBox.Text;
            RegistryKeyLi.WriteRegistryKey(Commentclass.CommentIP, Commentclass.CommentPublicResgistryKeyPath, "CommentIP");
            RegistryKeyLi.WriteRegistryKey(this.filePath_textBox.Text, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargResgistryKeyName);
            Commentclass.CommentBackack = SocketLi.SocketPingNet(Commentclass.CommentIP);

            if (Commentclass.CommentBackack == "no")
            {
                MessageBoxMidle.Show(this, "网络错误！请检查网络连接是否正常？IP地址设置是否正确？", "提示");
                log_data_update("网络错误！请检查网络连接是否正常？IP地址设置是否正确？\r\n");
                this.statelable.Text = "下载失败";
                return;
            }


            if (string.IsNullOrEmpty(filePath_textBox.Text))
            {
                MessageBoxMidle.Show(this, "请选择你要下载的充电器.Bin文件！", "提示！");
                log_data_update("请选择你要下载的充电器.Bin文件\r\n");
                this.statelable.Text = "下载失败";
                return;
            }

            //判断文件大小是否符合
            if (Commentclass.CommentUpgrade == 0X02)//充电板需要重新截取目标刷机文件
            {
                //从485刷机没有看到截取目标文件的操作，仅仅对文件大小做了判断，此处同理
                try
                {
                    //下面是抄的
                    FileStream fs = new FileStream(filePath_textBox.Text, FileMode.Open, FileAccess.Read);
                    byte[] arrfileSend = new byte[1024 * 1024 * 8];               //创建8M的缓存
                    int length = fs.Read(arrfileSend, 0, arrfileSend.Length);
                    byte[] arrfile = new byte[length];
                    fs.Close();    //关闭文件流，释放文件
                    Buffer.BlockCopy(arrfileSend, 0, arrfile, 0, length);           // 将arrfileSend复制到arrfile之中
                    file_data = new byte[length];
                    Buffer.BlockCopy(arrfileSend, 0, file_data, 0, length);          // 将arrfileSend复制到arrfile之中
                    Console.WriteLine($"filedata = {file_data.Length}.");


                    //加入文件大小判断
                    if (file_data.Length < (int)Commentclass.ChargeBoardFileSizeMin || file_data.Length > (int)Commentclass.ChargeBoardFileSizeMax)
                    {
                        MessageBoxMidle.Show("当前导入的程序文件有误！请重新导入正确的程序文件！", "文件错误");
                        log_data_update("当前导入的程序文件有误！请重新导入正确的程序文件\r\n");
                        return;
                    }

                    //截取充电板文件
                    string backack = ReadFiles.FragmentCode(Commentclass.ChargebordRedirectionAddr, Commentclass.ChargebordCodeStartAddr, (UInt64)file_data.Length,
                               Commentclass.ChargebordIniRecordBootloaderSize, Commentclass.ChargebordBootloaderHeadFlag, file_data, out CutoutFiles);
                    if (backack == "ok")
                    {
                        file_data = new byte[CutoutFiles.Length];
                        Buffer.BlockCopy(CutoutFiles, 0, file_data, 0, CutoutFiles.Length);
                    }
                    else
                    {
                        Console.WriteLine("文件截取App失败\r\n");
                        MessageBox.Show("截取文件App失败\r\n");
                        return;
                    }
                    

                    Commentclass.CommentFileLength = (uint)file_data.Length;

                    //计算发包的包数以及相关变量初始化
                    Commentclass.CommentStepI = 0x00;
                    Commentclass.CommentDuty = 0x00;
                    Commentclass.CommentPagenum = Commentclass.CommentFileLength / 128;
                    Commentclass.CommentRemaind = Commentclass.CommentFileLength % 128;
                    progressBar1.Maximum = (int)(Commentclass.CommentPagenum); //进度条的最大值
                    Console.WriteLine($" 总计数据包 Commentclass.CommentPagenum = {Commentclass.CommentPagenum+1}.");
                    log_data_update("总计数据包 Commentclass.CommentPagenum = " + (Commentclass.CommentPagenum+1).ToString() + "\r\n");

                    Console.WriteLine($" 不足一包的数据余量(byte) Commentclass.CommentRemaind  = {Commentclass.CommentRemaind}.");
                    log_data_update("数据余量(byte) Commentclass.CommentRemaind = " + Commentclass.CommentRemaind.ToString() + "\r\n");
                    //Console.WriteLine($"Commentclass.CommentFileData.Length = {Commentclass.CommentFileData.Length}.");

                    //文件操作完毕，通过单独线程进行通信对接
                    Thread RxThread = new Thread(SocketRxThread);
                    keep_running = true;
                    RxThread.Start();

                    Thread TxThread = new Thread(Eth_Communication);
                    TxThread.Start();

                    Disable_UI(); //禁止UI
                    //Eth_Communication();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);//显示内存操作或文件操作的报错
                }
            }


            //
        }

        #endregion


        #region 通信流程 

        //全程按ModBus_TCP执行
        //IAP跳转对接

        bool wait_step_sign; //接收处理标志，发送后一帧数据，将该标志设置为false，又接收线程正确接收后设置为true
        private void Eth_Communication()
        {
            ushort[] first_handshage_frame = { Convert.ToUInt16(this.handshake1_textbox.Text,16) };
            ushort[] second_handshage_frame = { Convert.ToUInt16(this.handshake2_textbox.Text, 16) };
            //套接字链接       
            string result = SocketLi.SocketConnect(Commentclass.CommentIP, Commentclass.CommentNetPort);
            if (result != "ok")
            {
                MessageBox.Show(result);
                this.keep_running = false;
                
                log_data_update("********本次下载失败！请检查系统供电、网络连接是否正常********\r\n");
                BeginInvoke(new Action(() => {
                    int wait_time = Convert.ToInt32(this.reflash_textBox.Text);
                    if (wait_time == 0) 
                    {
                        wait_time = 1;
                    }
                    this.reflash_timer.Interval = wait_time;
                    this.reflash_timer.Start();
                }));
                MessageBox.Show("本次升级失败，请检查系统供电、网络连接是否正常？\r\n 变桨系统重置中，请稍后再尝试重新下载......");
                return;
            }
            else
            {
                Commentclass.CommentTranspDataEnd = false;
                BeginInvoke(new Action(() => {
                    this.statelable.Text = "握手中...";
                }));
                if (Send_Handshake_Frame(Send_Msg, first_handshage_frame))
                {
                    //第一次握手与第二次握手间隔 1s
                    Console.WriteLine(WaitTime1_textBox.Text + "ms后下发第二帧握手帧\r\n");
                    log_data_update(WaitTime1_textBox.Text + "ms后下发第二帧握手帧\r\n");

                    Thread.Sleep(Convert.ToInt32(WaitTime1_textBox.Text));

                    if (Send_Handshake_Frame(Send_Msg, second_handshage_frame))
                    {
                        //第二次握手与透传帧间隔 5s 
                        BeginInvoke(new Action(() => {
                            this.statelable.Text = "二次握手...";
                        }));
                        Console.WriteLine(WaitTime2_textBox.Text + "ms后下发透传帧\r\n");
                        log_data_update(WaitTime2_textBox.Text + "ms后下发透传帧\r\n");
                        Thread.Sleep(Convert.ToInt32(WaitTime2_textBox.Text));

                        if (Send_Transparent_Frame())
                        {
                            BeginInvoke(new Action(() => {
                                this.statelable.Text = "即将下传数据包...";
                            }));
                            Console.WriteLine("可执行数据下传,"+ WaitTime3_textBox.Text + "ms后启动数据包下传\r\n");
                            log_data_update("可执行数据下传,"+ WaitTime3_textBox.Text + "ms后启动数据包下传\r\n");
                            Thread.Sleep(Convert.ToInt32(WaitTime3_textBox.Text)); 
                            
                            Commentclass.CommentBohead[0] = 0x01;
                            Commentclass.CommentBohead[1] = 0xFE;
                            BeginInvoke(new Action(() => {
                                this.statelable.Text = "程序下载中：0%";
                            }));
                            Send_Data_Frame();
                            Commentclass.CommentTranspDataEnd = true;
                            return;
                        }
                    }
                    Commentclass.CommentTranspDataEnd = true;
                }

                
                keep_running = false;
                MessageBox.Show("握手透传无响应");
                log_data_update("握手透传无响应\r\n");
                SocketLi.sockClient.Disconnect(false);
                SocketLi.sockClient.Close();


                log_data_update("********本次下载失败！请检查系统供电、网络连接是否正常********\r\n");
                BeginInvoke(new Action(() => {
                    int wait_time =  Convert.ToInt32(this.reflash_textBox.Text);
                    if (wait_time == 0)
                    {
                        wait_time = 1;
                    }
                    this.reflash_timer.Interval = wait_time;
                    this.reflash_timer.Start();
                }));
                MessageBox.Show("本次升级失败，请检查系统供电、网络连接是否正常？\r\n 变桨系统重置中，请稍后再尝试重新下载......");
            }




        }



        #region 握手帧和透传帧
        /// <summary>
        /// 发送10功能吗的握手帧
        /// </summary>
        /// <param name="function"></param>
        /// <param name="reg_value"></param>
        /// <returns></returns>
        bool Send_Handshake_Frame(Func<ushort[], bool> function, ushort[] reg_value)
        {
            try
            {
                int try_times = 5;    //最多尝试5次握手帧,更改后仅仅尝试一次
                int wait_times = 10;  //每次握手帧等待恢复为100ms
                while ((try_times--) > 0)
                {
                    wait_step_sign = false;
                    function(reg_value);
                    while (!wait_step_sign && ((wait_times--) > 0))
                    {
                        Thread.Sleep(100); //线程休眠等待
                    }

                    if (wait_step_sign == true)
                    {
                        //数据处理
                        if (Modbustcp.ComparerWriteMultipleRegisters2(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        bool Send_Msg(ushort[] Reg_Value)
        {
            try
            {
                Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, Commentclass.CommentAppResgitAddr, 0x01, Reg_Value);//10功能码
                Console.WriteLine(DateTime.Now.ToString("ss fff") + "发送：" + StartsCRC.zftring(Commentclass.CommentDisplayTemp));
                log_data_update("发送：" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送06功能码的透传帧
        /// </summary>
        /// <returns></returns>
        bool Send_Transparent_Frame()
        {
            try
            {
                ushort transparent_frame = Convert.ToUInt16(this.transport_textbox.Text, 16);

                int try_times = 5;    //最多尝试5次握手帧
                int wait_times = 10;  //每次握手帧等待恢复为100ms
                while ((try_times--) > 0)
                {
                    wait_step_sign = false;
                    Modbustcp.MasterWriteSingleRegister(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, transparent_frame);//06功能码
                    Console.WriteLine("发送：" + StartsCRC.zftring(Commentclass.CommentDisplayTemp));
                    log_data_update("发送：" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                    while (!wait_step_sign && ((wait_times--) > 0))
                    {
                        Thread.Sleep(100); //线程休眠等待
                    }

                    if (wait_step_sign == true)
                    {
                        //数据处理
                        if (Commentclass.CommentResEventTemp == transparent_frame)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion


        #region 数据接收线程

        bool keep_running = true;
        ushort MbtcpTid, ResgierAddress, ResgierQuality, ResgierValues;
        private void SocketRxThread()
        {

            while (keep_running)
            {
                try
                {
                    Commentclass.SocketBreakFlag = false;
                    Commentclass.CommentBackack = SocketLi.SocketResiveData(out Commentclass.CommentTempResive);
                    //Console.WriteLine($"Commentclass.CommentTempResive = {Commentclass.CommentTempResive.Length}.")
                    //Console.WriteLine("接收：" + StartsCRC.zftring(Commentclass.CommentTempResive));
                    //判断接收长度是否足字节
                    if (Commentclass.CommentTempResive.Length < 0x06)
                    {
                        continue;
                    }

                    //套接字接收有异常
                    if (Commentclass.CommentBackack != "ok")
                    {
                        continue;
                    }

                    Console.WriteLine("接收：" + StartsCRC.zftring(Commentclass.CommentTempResive));
                    log_data_update("接收：" + StartsCRC.zftring(Commentclass.CommentTempResive) + "\r\n\r\n");

                    //消息帧错误以太网回复异常码
                    if (Commentclass.CommentTempResive[2] != 0x00 || Commentclass.CommentTempResive[3] != 0x00 || Commentclass.CommentTempResive[6] != Commentclass.CommentMbAddress)     //不符合modbustcp协议格式,设备地址不符合,长度不对
                    {
                        MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);
                        Modbustcp.ExxeptionResponse(MbtcpTid, Commentclass.CommentMbAddress, Commentclass.CommentTempResive[7], 0x03);
                        Console.WriteLine("发送：" + StartsCRC.zftring(Commentclass.CommentDisplayTemp));
                        log_data_update("发送：" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                    }
                    else
                    {
                        switch (Commentclass.CommentTempResive[7])    //依据功能码进行接收数据拆解
                        {
                            case 0x03://读多个寄存器
                                MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);                                                              //事务处理标识符
                                ResgierAddress = (ushort)(Commentclass.CommentTempResive[8] * 256 + Commentclass.CommentTempResive[9]);
                                Commentclass.CommentBackAddress[0] = ResgierAddress;//寄存器地址
                                ResgierQuality = (ushort)(Commentclass.CommentTempResive[10] * 256 + Commentclass.CommentTempResive[11]);
                                Commentclass.CommentBackAddress[1] = ResgierQuality; //寄存器数量
                                wait_step_sign = true;
                                break;

                            case 0x06://写单个寄存器
                                MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);
                                ResgierAddress = (ushort)(Commentclass.CommentTempResive[8] * 256 + Commentclass.CommentTempResive[9]);
                                Commentclass.CommentBackAddress[0] = ResgierAddress;
                                ResgierValues = (ushort)(Commentclass.CommentTempResive[10] * 256 + Commentclass.CommentTempResive[11]);
                                Commentclass.CommentBackAddress[1] = ResgierValues;
                                Commentclass.CommentResEventTemp = Commentclass.CommentBackAddress[1];
                                wait_step_sign = true;
                                break;

                            case 0x10://写多个寄存器
                                MbtcpTid = (ushort)(Commentclass.CommentTempResive[0] * 256 + Commentclass.CommentTempResive[1]);
                                ResgierAddress = (ushort)(Commentclass.CommentTempResive[8] * 256 + Commentclass.CommentTempResive[9]);
                                Commentclass.CommentBackAddress[0] = ResgierAddress;//寄存器地址
                                ResgierQuality = (ushort)(Commentclass.CommentTempResive[10] * 256 + Commentclass.CommentTempResive[11]);
                                Commentclass.CommentBackAddress[1] = ResgierQuality;//寄存器数量
                                wait_step_sign = true;
                                break;

                            default:
                                break;

                        }
                    }

                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            Console.WriteLine("接收线程关闭");

        }

        #endregion





        //2、文件数据交付
        #region 数据帧
        void Send_Data_Frame()
        {
            try
            {

                Commentclass.CommentStepI = 0;
                while (true)
                {
                    

                    
                    BeginInvoke(new Action(() => {
                        this.progressBar1.Value = (int)Commentclass.CommentStepI;
                        double result = (double)(Math.Round((decimal)((double)Commentclass.CommentStepI / (double)Commentclass.CommentPagenum), 4));
                        this.statelable.Text = "程序下载中：" + string.Format("{0:#.00%}", result);
                        //this.label11.Text = string.Format("{0:P1}", result);
                    }));


                    if (Commentclass.CommentStepI < Commentclass.CommentPagenum) //完整128字节的一包数据
                    {
                        Console.WriteLine("正在发送第" + (Commentclass.CommentStepI + 1).ToString() + "包数据");
                        log_data_update("正在发送第" + (Commentclass.CommentStepI + 1).ToString() + "包数据\r\n");

                        Buffer.BlockCopy(file_data, (int)(Commentclass.CommentStepI * 128), Commentclass.CommentCodePackTemp, 0, 128);

                        Commentclass.CommentCrcValue = StartsCRC.CRC16(Commentclass.CommentCodePackTemp, 128);
                        Commentclass.CommentCrcCheck[0] = (byte)(Commentclass.CommentCrcValue >> 8);        //crch
                        Commentclass.CommentCrcCheck[1] = (byte)Commentclass.CommentCrcValue;               //crcl
                        Buffer.BlockCopy(Commentclass.CommentBohead, 0, Commentclass.CommentCodePack, 0, Commentclass.CommentBohead.Length);  //将Bohead放到Codepack之中
                        Buffer.BlockCopy(Commentclass.CommentCodePackTemp, 0, Commentclass.CommentCodePack, Commentclass.CommentBohead.Length, Commentclass.CommentCodePackTemp.Length);//把128个字节的代码包放到整体代码包之中
                        Buffer.BlockCopy(Commentclass.CommentCrcCheck, 0, Commentclass.CommentCodePack, (Commentclass.CommentBohead.Length + Commentclass.CommentCodePackTemp.Length), Commentclass.CommentCrcCheck.Length);//拼接CRC               
                        

                        Commentclass.CommentStepI = Commentclass.CommentStepI + 1;

                        Commentclass.CommentBackack = Modbustcp.ByteConcertUshortSum(Commentclass.CommentCodePack, out Commentclass.CommentTcpCodePack);

                        wait_step_sign = false;
                        Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x42, Commentclass.CommentTcpCodePack);


                    }
                    else if (Commentclass.CommentRemaind > 0) //不足128字节的一包数据
                    {
                        Console.WriteLine("正在发送第" + (Commentclass.CommentStepI + 1).ToString() + "包数据");
                        log_data_update("正在发送第" + (Commentclass.CommentStepI + 1).ToString() + "包数据\r\n");


                        Buffer.BlockCopy(file_data, (int)(Commentclass.CommentPagenum * 128), Commentclass.CommentCodePackTemp, 0, (int)Commentclass.CommentRemaind);
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
                        Commentclass.CommentBackack = Modbustcp.ByteConcertUshortSum(Commentclass.CommentCodePack, out Commentclass.CommentTcpCodePack);

                        wait_step_sign = false;
                        Modbustcp.MasterWriteMultipleRegisters(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, 0x42, Commentclass.CommentTcpCodePack);
                        Commentclass.CommentBohead[0] = (byte)(Commentclass.CommentBohead[0] + 1);
                        Commentclass.CommentBohead[1] = (byte)(0Xff - Commentclass.CommentBohead[0]);
                        Commentclass.CommentRemaind = 0x00;
                    }
                    else
                    {
                        Console.WriteLine("发送结束帧（5A5A）");
                        log_data_update("发送结束帧（5A5A）\r\n");

                        wait_step_sign = false;
                        Modbustcp.MasterWriteSingleRegister(Commentclass.CommentMbtcp_TID, Commentclass.CommentMbAddress, 0x00, Commentclass.TranspEnd);
                        log_data_update("发送:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                        if (Wait_For_Step_Sign())
                        {
                            
                            if (Commentclass.CommentResEventTemp != Commentclass.TranspEnd)
                            {
                                keep_running = false;
                                Console.WriteLine("补充数据包(5A5A)校验错误,下载终止");
                                SocketLi.sockClient.Close();
                                log_data_update("补充数据包（5A5A）校验错误,下载终止\r\n");

                                log_data_update("********本次下载失败！请检查系统供电、网络连接是否正常********\r\n");
                                BeginInvoke(new Action(() => {
                                    int wait_time = Convert.ToInt32(this.reflash_textBox.Text);
                                    if (wait_time == 0)
                                    {
                                        wait_time = 1;
                                    }
                                    this.reflash_timer.Interval = wait_time;
                                    this.reflash_timer.Start();
                                }));
                                MessageBox.Show("本次升级失败，请检查系统供电、网络连接是否正常？\r\n 变桨系统重置中，请稍后再尝试重新下载......");
                                return;
                            }
                            else
                            {
                                keep_running = false;
                                Console.WriteLine("下载完成");
                                SocketLi.sockClient.Close();
                                log_data_update("下载完成\r\n");
                                BeginInvoke(new Action(() => {
                                    this.statelable.Text = "下载完成！";
                                    this.Enable_UI();
                                }));
                                //MessageBox.Show("下载完成!");
                                return;
                            }
                        }
                        else
                        {
                            keep_running = false;
                            Console.WriteLine("补充数据包无回复，下载终止");
                            SocketLi.sockClient.Close();
                            log_data_update("补充数据包无回复，下载终止\r\n");

                            log_data_update("********本次下载失败！请检查系统供电、网络连接是否正常********\r\n");
                            BeginInvoke(new Action(() => {
                                int wait_time = Convert.ToInt32(this.reflash_textBox.Text);
                                if (wait_time == 0)
                                {
                                    wait_time = 1;
                                }
                                this.reflash_timer.Interval = wait_time;
                                this.reflash_timer.Start();
                            }));
                            MessageBox.Show("本次升级失败，请检查系统供电、网络连接是否正常？\r\n 变桨系统重置中，请稍后再尝试重新下载......");
                            return;
                        }
                    }

                    log_data_update("发送:" + StartsCRC.zftring(Commentclass.CommentDisplayTemp) + "\r\n");
                    
                    //发送数据帧后进行校验
                    if (Wait_For_Step_Sign())
                    {
                        if (!Modbustcp.ComparerWriteMultipleRegisters(Commentclass.CommentSendRegist, Commentclass.CommentBackAddress)) //校验不正确
                        {
                            keep_running = false;
                            Console.WriteLine("接收校验错误,下载终止");
                            SocketLi.sockClient.Close();
                            log_data_update("接收校验错误,下载终止\r\n\n");

                            log_data_update("********本次下载失败！请检查系统供电、网络连接是否正常********\r\n");
                            BeginInvoke(new Action(() => {
                                int wait_time = Convert.ToInt32(this.reflash_textBox.Text);
                                if (wait_time == 0)
                                {
                                    wait_time = 1;
                                }
                                this.reflash_timer.Interval = wait_time;
                                this.reflash_timer.Start();
                            }));
                            MessageBox.Show("本次升级失败，请检查系统供电、网络连接是否正常？\r\n 变桨系统重置中，请稍后再尝试重新下载......");
                            return;
                        }
                        else
                        {
                            keep_running = true;
                            Commentclass.CommentBohead[0] += 1;
                            Commentclass.CommentBohead[1] -= 1;

                           

                        }
                    }
                    else   //等不到回复
                    {
                        keep_running = false;
                        Console.WriteLine("无回复，下载终止");
                        log_data_update("无回复,下载终止\r\n\n");
                        SocketLi.sockClient.Close();


                        log_data_update("********本次下载失败！请检查系统供电、网络连接是否正常********\r\n");
                        BeginInvoke(new Action(() => {
                            int wait_time = Convert.ToInt32(this.reflash_textBox.Text);
                            if (wait_time == 0)
                            {
                                wait_time = 1;
                            }
                            this.reflash_timer.Interval = wait_time;
                            this.reflash_timer.Start();
                        }));
                        MessageBox.Show("本次升级失败，请检查系统供电、网络连接是否正常？\r\n 变桨系统重置中，请稍后再尝试重新下载......");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("数据帧交互出错");
                SocketLi.sockClient.Close();
                log_data_update("数据帧交互出错,下载终止\r\n\n");

                log_data_update("********本次下载失败！请检查系统供电、网络连接是否正常********\r\n");
                BeginInvoke(new Action(() => {
                    int wait_time = Convert.ToInt32(this.reflash_textBox.Text);
                    if (wait_time == 0)
                    {
                        wait_time = 1;
                    }
                    this.reflash_timer.Interval = wait_time;
                    this.reflash_timer.Start();
                }));
                keep_running = false;
                MessageBox.Show("本次升级失败，请检查系统供电、网络连接是否正常？\r\n 变桨系统重置中，请稍后再尝试重新下载......");
            }
        }
        #endregion

        //
        bool Wait_For_Step_Sign()
        {
            int wait_time = 300;
            while (!wait_step_sign && ((wait_time--) > 0))
            {
                Thread.Sleep(1);
                Console.WriteLine(wait_time);
            }

            if (wait_time <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

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

        #region 日志文本更新
        bool USE_log = false;  //true,不打印log false 打印log
        void log_data_update(string msg)
        {
           

            if (USE_log)
            {
                Invoke(new Action(() =>
                {
                    this.log_TextBox.AppendText(System.DateTime.Now.ToString("HH:mm:ss.fff") + ":" + msg);
                 //   this.log_TextBox.Text += System.DateTime.Now.ToString("HH:mm:ss.fff") + ":" + msg;
                    
                    this.log_TextBox.SelectionStart = this.log_TextBox.Text.Length;
                    this.log_TextBox.ScrollToCaret();
                }));
            }
            else
            { 
                string text = System.DateTime.Now.ToString("HH:mm:ss.fff") + ":" + msg;
                Console.WriteLine(text);
            }
            

        }
        #endregion

        #region 文本拖放
        private void Text_DragEnter(object sender, DragEventArgs e)                                         //获得“信息”
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;                                                              //重要代码：表明是所有类型的数据，比如文件路径
            else
                e.Effect = DragDropEffects.None;
        }

        private void Text_DragDrop(object sender, DragEventArgs e)                                          //解析信息
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();       //获得路径
            filePath_textBox.Text = path;                                                                            //由一个textBox显示路径
        }

       
        #endregion

        #region 日志打印与清除功能
        private void ContextStripMenuItem_Click(object sender, EventArgs e)
        {
            UInt16 tag = Convert.ToUInt16(((ToolStripMenuItem)sender).Tag.ToString());
            switch (tag)
            {
                case 0x00://清除
                    this.log_TextBox.Text = string.Empty;
                    break;
                case 0x01://导出文本
                    if (Commentclass.CommentTranspDataEnd)
                    {
                        string BackMessage = ControlFile.SaveTxtFilesFromTextBox(this.log_TextBox);
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

        #region 下载中禁止UI操作/启用UI操作

        bool Down_reflash = false;
        private void Form8_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Down_reflash == true)
            { 
                e.Cancel = true;
            }
        }

        

        void Disable_UI()
        { 
            this.menuStrip1.Enabled = false;
            this.groupBox1.Enabled = false;
            this.Down_reflash = true;
        }

        private void reflash_textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form8_Activated(object sender, EventArgs e)
        {
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, "CommentIP", out Commentclass.CommentIP);
            Console.WriteLine($"读出来的路径 = {Commentclass.CommentIP}.");
            if (Commentclass.CommentIP == "")
            {
                Commentclass.CommentIP = "192.168.1.15";
                
            }

            this.ip_textBox.Text = Commentclass.CommentIP;
        }

        private void ip_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            Commentclass.CommentIP = this.ip_textBox.Text;
            RegistryKeyLi.WriteRegistryKey(Commentclass.CommentIP, Commentclass.CommentPublicResgistryKeyPath, "CommentIP");
        }

        void Enable_UI()
        { 
            this.menuStrip1.Enabled = true;
            this.groupBox1.Enabled = true;
            this.Down_reflash = false;
        }

        private void reflash_timer_Tick(object sender, EventArgs e)
        {
            Enable_UI();
            this.statelable.Text = "下载失败";
            this.reflash_timer.Stop();
        }
        #endregion


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string msg;
            if (this.checkBox1.Checked == true)
            {
                //MessageBox.Show("将不再打印日志信息，下载速率增加");
                USE_log = false;

                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName, out msg);
                if (msg == "User_Client")
                {
                    this.Height = 260;

                }
            }
            else
            { 
                USE_log = true;
                this.Height = 500;
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName, out msg);
                if (msg == "User_Client")
                {
                    this.Height = 450;

                }
            }
        }

    }
}
