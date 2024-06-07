using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;//调用外部的DLL
using System.Drawing;

namespace _485刷机_Net_3._5
{
    public struct Pointdata
    {
        public static int x;
        public static int y;
        public static int weith;
        public static int heigth;
    };
    class Commentclass
    {
        public static Form1 fm1 = new Form1();  //主控板
        public static Form2 fm2 = new Form2();  //充电板
        public static Form8 fm8 = new Form8();  //充电ARM（以太网）
        public static Form3 fm3 = new Form3();  //DSP升级
        public static Form7 fm7 = new Form7();  //DSP升级（以太网）
        public static Form4 fm4 = new Form4();  //界面选择
        public static Setting set = new Setting();
        public static Login Lognin = new Login();

        public static int fmjump = 0x00;        //用来判断是从哪里调到下一个用户程序
        public static string Version = "2023.08.15";
        public static UInt16 DspTryTimer = 5;   //DSP断开以太网的重连时间单位为“S”

        #region ARM截取文件相关变量
        public static bool ChexkActionEnable = true;//开启往回追溯0xff的校验标志
        public static byte[] MainbordBootloaderHeadFlag = new byte[16];//iap文件的前16个数据
        public static UInt64 MainbordCodeStartAddr = 0;                //代码包的起始地址
        public static UInt64 MainbordRedirectionAddr = 0;              //目标程序的执行地址
        public static UInt64 MainbordBootloaderPackSize = 0;           //引导程序部分的代码大小 = 目标程序的执行地址 - 代码包的起始地址；
        public static UInt64 MainbordIniRecordBootloaderSize = 0;

        public static byte[] ChargebordBootloaderHeadFlag = new byte[16];//iap文件的前16个数据
        public static UInt64 ChargebordCodeStartAddr = 0;                //代码包的起始地址
        public static UInt64 ChargebordRedirectionAddr = 0;              //目标程序的执行地址
        public static UInt64 ChargebordBootloaderPackSize = 0;           //引导程序部分的代码大小 = 目标程序的执行地址 - 代码包的起始地址；
        public static UInt64 ChargebordIniRecordBootloaderSize = 0;
        #endregion

        #region 文件大小限制值
        //
        public static UInt64 MainBordMaximalSize = 0;
        public static UInt64 MainBordMinimalSize = 0;
        //
        public static UInt64 ChargeBordMaximalSize = 0;
        public static UInt64 ChargeBordMinimalSize = 0;
        //
        public static UInt64 DspNormalSize = 0;
        #endregion

        #region 界面初始化用
        public static bool WinDey = false;
        public static string User_APP = "";
        public static string Password = "";
        public static string[] LoginListNum = {                                     //状态信息列表                             
                                "StarsFD",//用户0
                                "090813",//密码0
                                "Client",//用户1
                                "2022",};//密码
        public static List<string> LoginCheck = new List<string>(LoginListNum);
        #endregion

        #region  充电板界面布局
        //端口lable 
        public struct PortlablePointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //端口combox
        public struct PortcomboxPointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //波特率lable
        public struct BoundlablePointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //波特率combox
        public struct BoundcomboxPointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //文件lable
        public struct FilelablePointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //文件textbox
        public struct FiletextboxPointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //进度lable
        public struct PrglablePointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //进度pragre
        public struct PrgformPointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //下载
        public struct DownbtnPointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        //浏览
        public struct ViewbtnPointdata
        {
            public static int x;
            public static int y;
            public static int weith;
            public static int heigth;
        };
        #endregion

        #region 配置信息
        public static string[] SettingMessageListNum = {                                     //状态信息列表                             
                                "",//0：主控头部校验16个数值
                                "",//1：主控ARM程序的开始地址
                                "",//2：主控ARM程序的跳转地址
                                "",//3：充电板头部校验的16个数值
                                "",//4：充电器程序的起始地址
                                "",//5：充电器程序的跳转地址
                                "",//6：用户目标的名称
                                "",//7：密码
                                "",//8：主控板ARM0xff校验的个数
                                ""};//9：充电板ARM0xff校验的个数

        public static List<string> SettingMessageList = new List<string>(SettingMessageListNum);

        #region 注册表相关类
        public static string ParaSource = "INI配置文件";


        public static string CommentPublicResgistryKeyPath = @"STARSH6\SETTING";        //BIN文件注册表的公共路径

        //主控板头部校验路径
        public static string CommentMainHeadCheckResgistryKeyName = "MainBordBootloaderHeadFlag";
        //主控起始地址
        public static string CommentMainStartAddrResgistryKeyName = "MainBordCodeStartAddr";
        //主控跳转地址
        public static string CommentMainLoadAddrResgistryKeyName = "MainBordRedirectionAddr";
        //充电板头部校验
        public static string CommentChargeHeadCheckResgistryKeyName = "ChargeBordBootloaderHeadFlag";
        //充电板起始地址
        public static string CommentChargeStartAddrResgistryKeyName = "ChargeBordCodeStartAddr";
        //充电板跳转地址
        public static string CommentChargeLoadAddrResgistryKeyName = "ChargeBordRedirectionAddr";
        //用户目标
        public static string CommentAppTargetResgistryKeyName = "TargetUser";
        //密码
        public static string CommentAppPasswordResgistryKeyName = "Password";
        //标志位
        public static string CommentHSixRegFlagResgistryKeyName = "HSixRegFlag";

        //主控ARM 的0xff的校验数量
        public static string CommentMainBoardBootloaderCheckNum = "MainBoardBootloaderCheckNum";
        //充电板ARM的0xff的校验数量
        public static string CommentChargeBordBootloaderCheckNum = "ChargeBordBootloaderCheckNum";
        #endregion

        #endregion

        public static UInt16 CommentSpecialMainNumCheck = 200;//主控板的0Xff的校验个数,默认

        public static UInt16 CommentSpecialChargeNumCheck = 200;//充电板的0xff的校验个数，默认

        public static UInt16 CommentHeadCheckNumLength = 200; //默认

        public static UInt16 UserLevel = 0x00;//用户权限


        #region 充电器提示信息
        public static string[] ChargeStateMessageListNum = {                                     //状态信息列表                             
                                "参数初始化中-----------------",//0
                                "等待主控板ARM进入IAP---------",//1
                                "命令发送等待响应-------------",//2
                                "等待主控板ARM进入透传模式----",//3
                                "主控板ARM已进入透传模式------",//4
                                "等待充电器跳转IAP------------",//5
                                "校验正确---------------------",//6
                                "校验错误---------------------",//7
                                "等待主控板ARM跳转------------",//8
                                "程序升级完成!----------------",//9
                                "无响应-----------------------",//10
                                "命令发送等待响应-------------",//11
                                "主控板ARM处于IAP模式---------",//12
                                };

        public static List<string> ChargeStateMessageList = new List<string>(ChargeStateMessageListNum);
        #endregion

        #region 主控ARM 提示信息
        public static string[] MainStateMessageListNum = {                                     //状态信息列表                             
                                "等待主控板ARM进入IAP---------",//0
                                "校验正确---------------------",//1
                                "校验错误---------------------",//2
                                "擦除Flash--------------------",//3
                                "擦除成功---------------------",//4
                                "擦除失败---------------------",//5
                                "主控板ARM跳转命令已发送------",//6
                                "程序升级完成!----------------",//7
                                "无响应-----------------------",//8
                                "串口连接失败-----------------",//9
                                };
        public static List<string> MainStateMessageList = new List<string>(MainStateMessageListNum);
        #endregion

        #region DSP提示信息
        public static string[] DSPStateMessageListNum = {                                     //状态信息列表                             
                                "发送升级就绪指令-------------",//0
                                "发送跳转升级指令-------------",//1
                                "发送解锁指令-----------------",//2
                                "发送擦除指令-----------------",//3
                                "校验正确---------------------",//4
                                "等待跳转到用户程序-----------",//5
                                "程序升级完成!----------------",//6
                                "无响应-----------------------",//7
                                "程序升级失败-----------------",//8
                                "校验错误---------------------",//9
                                };
        public static List<string> DSPStateMessageList = new List<string>(DSPStateMessageListNum);
        #endregion

        //public static string Version = "2023/08/09";
        //public static Form2 fm2 = new Form2();  //充电板单板刷机模式
        //public static Form1 fm1 = new Form1();
        //public static Setting set = new Setting();
        //public static Login Lognin = new Login();
        public static int windowX = 0;//窗体位置
        public static int windowY = 0;//窗体位置
        public static int OpenTheWindws = 0x00;

        public static int windowXx = 0;//窗体位置
        public static int windowYy = 0;//窗体位置

        #region modbusrtu类相关的变量定义
        public static byte[] HMI_Tx_modbus = new byte[256];             //modbus发送数据使用的数据体
        public static byte[] CommentResive = new byte[256];             //将串口/以太网通信接收到的数据放到这里，给其他窗口进行使用
        public static ushort[] CommentReadWrite = new ushort[256];      //创建属于上位机的数据
        public static byte[] CommentTempResive;                         //用于以太网接收数据out处
        public static byte[] CommentCodePackTemp = new byte[128];       //代码包临时存放数组
        public static byte[] CommentCodePack = new byte[132];           //代码包
        public static ushort[] CommentTcpCodePack;                      //byte转ushort后的代码包
        public static byte[] CommentDisplayTemp;                        //显示用的数组

        public static ushort CommentMbtcp_TID = 0;                      //事务处理标识符：计数器
        public static byte CommentMbAddress = 0x01;                     //设备地址
        public static UInt16 CommentUpgrade = 0x00;                     //刷机对象，0x00：主控板ARM   0x01：主控板DSP   0x02：充电板ARM  0x03：主控板ARM一键擦除
        public static byte CommentStepNow = 0x00;                       //执行的步骤

        public static ushort CommentAppResgitAddr = 0XC007;
        public static ushort[] CommentOrderFirst = new ushort[1] { 0XAA55 };    //当刷机对象为主控板ARM和充电板ARM时，主控板跳转的第一帧命令0x10功能码;
        public static ushort[] CommentOrderSecon = new ushort[1] { 0X55AA };    //当刷机对象为主控板ARM和充电板ARM时，主控板跳转的第二帧命令0x10功能码;

        public static ushort[] CommentOrderThrid = new ushort[1] { 0XA5A5 };    //当刷机对象为dsp时，主控板跳转的第一帧命令0x10功能码;
        public static ushort[] CommentOrderFourt = new ushort[1] { 0X5A5A };    //当刷机对象为dsp时，主控板跳转的第二帧命令0x10功能码;

        public static ushort[] DSPCommentOrderFirst = new ushort[3] { 0xc007, 0XA5A5, 0x9BA1 };   //DSP刷机跳转的第一帧命令
        public static ushort[] DSPCommentOrderSecon = new ushort[3] { 0xc007, 0X5A5A, 0x86AE };   //DSP刷机跳转的第二帧命令

        public static ushort[] CommentCdbOrderFirst = new ushort[3] { 0xc007, 0XAA55, 0x6480 };    //充电板跳转的第一帧命令0x10功能码;
        public static ushort[] CommentCdbrderSecon = new ushort[3] { 0xc007, 0X55AA, 0x798f };    //充电板跳转的第二帧命令0x10功能码;

        public static ushort[] CommentDspOrderUnlock = new ushort[3] { 0xc007, 0x0000, 0x50a0 }; //dsp解锁命令，设备地址为0xaa在引导程序中修正
        public static ushort[] CommentDspOrderErase = new ushort[3] { 0xc008, 0x0000, 0xa050 }; //dsp的擦除指令，设备地址为0xaa在引导程序中修正

        public static ushort TranspMyself = 0x01aa;                                     //告诉主控板刷机对象为自己
        public static ushort TranspCommand = 0x0155;                                    //让主控板进入透传状态，功能码0x06;
        public static ushort TranspAimdsp = 0x015a;                                     //告诉主控板透对象为DSP，功能码0x06;
        public static ushort TranspAimcdb = 0x01a5;                                     //告诉主控板透传对象为充电板ARM，功能码0x06;
        public static ushort TranspEnd = 0x5a5a;                                        //告诉主控板程序包已经发完了，功能码0x06;
        public static ushort ClearFlash = 0x017f;                                       //告诉主控板要擦除Flash了（预留），功能码0x06;
        public static ushort MainContrast = 0x0041;                                     //用于判断主控板有没有进入到刷机状态时使用的比较变量，功能码0x06;
                                                                                        // public static ushort DspAbresaCom = 0x5000;                                     //用于告知DSP需要擦除的扇区数

        public static ushort CdbContrast = 0x0043;                                      //用于判断充电板板有没有进入到刷机状态时使用的比较变量，功能码0x06;
        public static ushort DspContrast = 0x0044;                                      //用于判端DSP板有没有进入到刷机状态时使用的比较变量，功能码0x06;

        public static byte[] CommentResEventTemps = new byte[4];                        //用于10功能码事件中解析，获取到的数据,起始地址以及寄存器数量
        public static ushort CommentResEventTemp = 0x0000;                              //用于06功能码事件中解析，获取到的数据

        public static bool CommentUpgradeState = false;                                 //用来判断是否处于刷机状态的标志位
        public static bool CommentAckTimerout = false;                                  //用来判断命令响应是否超时
        public static ushort CommentTryCount = 0x00;                                    //用来计算数据以及命令重发的次数
        public static bool CommentTimerState = false;                                   //判断定时器的开启状态
        public static bool CommentTranspCommand = false;                                //true:发送使对象进入刷机状态的目的已经实现
        public static bool CommentMainHavedTrans = false;                               //true:主控已进入透传状态

        public static byte CommentDspAbraseNum = 0x00;                                  //计算得到DSP要擦除的扇区数
        public static UInt32 CommentDspAbraseTimerOut = 0x00;                           //DSp擦除扇区的超时时间
        public static UInt32 CommentDspNewFilesSize = 0x00;                             //重新计算得到DS屏的刷机文件的大小

        public static UInt32 CommentSocketConnectTimerCount = 0x05;                     //以太网断开重连的计时，计时五秒
        public static byte CommentDisplayEventTarget = 0x00;                            //上位机数据框的显示对象
        public static bool SocketBreakFlag = false;                                     //以太网通信断开标志
        public static ushort[] CommentBackAddress = new ushort[2];                      //寄存器起始地址,以及寄存器数量

        public static byte[] CommentSendRegist = new byte[4] { 0xff, 0xff, 0xff, 0xff };    //发送时用来做寄存器起始地址的存放以及寄存器数量的

        public static int CommentyComTryAck = 0;

        #region 状态结构体
        public struct AboutMbState
        {
            public static int ReadMultipleRegistersFlag = 0;            //0x03功能码事件
            public static int WriteSingleRegisterFlag = 0;              //0x06功能码事件
            public static int WriteMultipleRegistersFlag = 0;           //0x10功能码事件
        };
        #endregion

        #endregion

        #region 文件操作类相关变量
        public static byte[] CommentFileData;                   //创建保存读取的文件数据的缓存空间
        public static byte[] CutSectorFileData;                 //用于保存主控ARM和充电板ARM的截取的Bin文件
        public static string CommentRealyPath = "";             //BIn文件的路径
        public static UInt32 CommentFileLength;                 //bin文件大小
        public static UInt32 CommentPagenum;                    //整的发包次数
        public static UInt32 CommentRemaind;                    //剩余多少个文件的数据
        public static UInt32 CommentStepI;                      //发包次数
        public static UInt32 CommentDuty;                       //进度条
        public static bool CommentTranspDataEnd = true;         //数据交互完成标志
        public static UInt32 CommentChekNum = 10;               //默认是10
        public static UInt32 CommentMainBordCheckNum = 10;
        public static UInt32 CommentChargeBordCheckNum = 10;
        #endregion

        #region DSP以太网刷机参数
        //#region ARM截取文件相关变量
        //public static byte[] MainbordBootloaderHeadFlag = new byte[16];//iap文件的前16个数据
        //public static UInt64 MainbordCodeStartAddr = 0;                //代码包的起始地址
        //public static UInt64 MainbordRedirectionAddr = 0;              //目标程序的执行地址
        //public static UInt64 MainbordBootloaderPackSize = 0;           //引导程序部分的代码大小 = 目标程序的执行地址 - 代码包的起始地址；
        //public static UInt64 MainbordIniRecordBootloaderSize = 0;

        //public static byte[] ChargebordBootloaderHeadFlag = new byte[16];//iap文件的前16个数据
        //public static UInt64 ChargebordCodeStartAddr = 0;                //代码包的起始地址
        //public static UInt64 ChargebordRedirectionAddr = 0;              //目标程序的执行地址
        //public static UInt64 ChargebordBootloaderPackSize = 0;           //引导程序部分的代码大小 = 目标程序的执行地址 - 代码包的起始地址；
        //public static UInt64 ChargebordIniRecordBootloaderSize = 0;
        //#endregion

        #region 目标文件大小限制变量
        public static UInt64 MainBoardFileSizeMin = 100 * 1024;//默认100kb
        public static UInt64 MainBoardFileSizeMax = 300 * 1024;//默认300kb
        public static UInt64 ChargeBoardFileSizeMin = 30 * 1024;//默认30kb
        public static UInt64 ChargeBoardFileSizeMax = 90 * 1024;//默认90kb

        public static UInt64 DSPBoardFileSizeMin = 512 * 1024;//默认512kb
        public static UInt64 DSPBoardFileSizeMax = 512 * 1024;//默认512kb

        public static UInt64 MainBoardFileHexSizeMin = 100 * 1024;//默认100kb
        public static UInt64 MainBoardFileHexSizeMax = 300 * 1024;//默认300kb
        public static UInt64 MainBoardFileBinSizeMin = 100 * 1024;//默认100kb
        public static UInt64 MainBoardFileBinSizeMax = 300 * 1024;//默认300kb
        #endregion

        #region 以太网通信相关
        public static string CommentIP = "192.168.1.15";                   //IP地址
        public static int CommentNetPort = 502;                         //网络端口
        #endregion

        #region 状态结构体
        public struct AboutState
        {
            public static string LinkOn = "连接";
            public static string LinkOff = "断开";
            public static string TimerOut = "超时";
        };
        #endregion

        public static string CommentBackack = "";                       //封装类的公共响应接收字符串,"ok" or other

        #region 关于位置的类（作废）
        public static int CommentPointx = 0;
        public static int CommentPointy = 0;
        #endregion

        #region 注册表相关类
        //public static string CommentPublicResgistryKeyPath = @"SHDQ\MAINBORD";        //BIN文件注册表的公共路径
        public static string CommentMainResgistryKeyName = "MAINFILEBIN";             //文件夹名称
        public static string CommentChargResgistryKeyName = "CHARGEFILEBIN";          //文件夹名称
        public static string CommentDspResgistryKeyName = "DSPFILEBIN";               //文件夹名称
        public static string CommentRuningKyeName = "";                               //通过刷机对象从而判断它的属值
        public static string CommentReadSavePath = "";                                //读到的路径
        #endregion

        #region 应答式收发相关变量
        public static byte[] CommentBohead = new byte[2] { 0x01, 0xfe };//报头数据包
        public static byte[] CommentCrcCheck = new byte[2];                //存放计算得到的CRC
        public static UInt16 CommentCrcValue = 0x00;                       //计算得到的Crc的值
        public static bool CommentAckContinue = true;                      //应答式发送的允许标志
        public static byte[] CommentAckCompare = new byte[4] { 0x00, 0x00, 0x00, 0x85 };//代码包正确返回代码，功能码10
        #endregion

        public static string[] StateMessageListNum = {                                     //状态信息列表                             
                                "状态：------发送第一帧跳转命令，等待响应。",//0
                                "状态：------发送第二帧跳转命令，等待响应。",//1
                                "状态：------------发送解锁命令，等待响应。",//2
                                "状态：------------发送擦除命令，等待响应。",//3
                                "状态：----------------------准备代码传输。",//4
                                "状态：--------------------------校验正确。",//5
                                "状态：--------------------------校验错误。",//6
                                "状态：----------------------程序升级完成。",//7
                                "状态：------响应失败，请重新执行刷机操作。"};//8

        public static List<string> StateMessageList = new List<string>(StateMessageListNum);
        public static Double CommentCodeDownloaeDuty = 0;                               //刷机进程百分比显示
                                                                                        //
        public static bool DisplayMonitor = false;    //是否显示监控窗口
        #endregion

        #region David新增
        public static bool IsHideMainBorde = true;
        public static bool IsAddDspBordLoad = false;

      
        [System.CLSCompliant(false)]
        // [System.Runtime.Versioning.SupportedOSPlatform("windows")]

        private static System.Diagnostics.Process p;
        public static void  menuItem4_ItemClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("David--用户点击");
            
            if (p == null)
            {
                string file_path = "PyQt5FlashLoader_ISP_DSP.exe";
                if (File.Exists(file_path))
                {
                   // MessageBox.Show("文件存在，正在打开");
                    /* p.StartInfo.UseShellExecute = false; //必需
 
                        p.StartInfo.RedirectStandardOutput = true;//输出参数设定
 
                        p.StartInfo.RedirectStandardInput = true;//传入参数设定
 
                        p.StartInfo.CreateNoWindow = true;
 
                        p.StartInfo.Arguments = @"D:\打包文件\dist\original RMS.csv";//参数以空格分隔，如果某个参数为空，可以传入””
                     */
                    p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = file_path;
                    p.StartInfo.WorkingDirectory = Application.StartupPath;
                    p.StartInfo.Arguments = "";
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.Start();
                }
                else
                {
                    MessageBox.Show("exe文件不程序运行目录下", "Tips");
                }

            }
            else
            {
                if (p.HasExited) //是否正在运行
                {
                    p.Start();
                }
            }

            if (p != null)
            {
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            }
            

        }


      

        #endregion
    }
}
