using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using View;

namespace _485刷机_Net_3._5
{
    public partial class Form4 : Form
    {
        public static string Userinipath = Application.StartupPath + "\\ConnectString.ini";//该变量保存INI文件所在的具体物理位置
        //        TargetUser = StarsFD
        //        Password = 090813
        //        TargetUser = WinDey
        //        Password = 2022
        public string RegReadTemp = "";
        public string IniFilesPath = Application.StartupPath + "\\ConnectString.ini";//ini文件的路径
        public string TempStr = "";//字符串变量
        public string ComPathStr = "";

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Move(object sender, EventArgs e)
        {
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.Opacity = 0.0;
            RegistryKeyLi.PreventCreadErr(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentHSixRegFlagResgistryKeyName);     //防止标志位出错

            //判断注册表的标志是否存在，如果不存在就读取INi
            RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentHSixRegFlagResgistryKeyName, out RegReadTemp);
            if (RegReadTemp == "NONE")//把ini写入注册表中，并把标志位写为“SUCCEED”
            {
                //注册表为空，先将ini作为默认同时写入注册表中
                //主控ARM 头部校验
                Commentclass.SettingMessageList[0] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordBootloaderHeadFlagINI", "None", IniFilesPath);
                //主控ARM程序起始地址
                Commentclass.SettingMessageList[1] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordCodeStartAddrINI", "None", IniFilesPath);
                //主控ARM程序的跳转地址
                Commentclass.SettingMessageList[2] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordRedirectionAddrINI", "None", IniFilesPath);

                //辅助ARM 头部校验
                Commentclass.SettingMessageList[3] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordBootloaderHeadFlagINI", "None", IniFilesPath);
                //辅助ARM程序起始地址
                Commentclass.SettingMessageList[4] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordCodeStartAddrINI", "None", IniFilesPath);
                //辅助ARM程序的跳转地址
                Commentclass.SettingMessageList[5] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordRedirectionAddrINI", "None", IniFilesPath);

                //主控ARM的0xff校验个数
                Commentclass.SettingMessageList[8] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBoardBootloaderCheckNum", "None", IniFilesPath);
                //辅助ARM的0xff校验个数
                Commentclass.SettingMessageList[9] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordBootloaderCheckNum", "None", IniFilesPath);
                //用户
                TempStr = ReadINIFiles.ReadIniData("UserConfig", "TargetUser", "None", IniFilesPath);
                switch (TempStr)
                {
                    case "User_StarsFD"://星辰
                        Commentclass.SettingMessageList[6] = "User_StarsFD";
                        Commentclass.SettingMessageList[7] = "090813";
                        break;
                    case "User_Client"://客户
                        Commentclass.SettingMessageList[6] = "User_Client";
                        Commentclass.SettingMessageList[7] = "2022";
                        break;
                    default:
                        break;
                }

                //写注册表
                //主控ARM 头部校验
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[0], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainHeadCheckResgistryKeyName);
                //主控ARM程序起始地址
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[1], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainStartAddrResgistryKeyName);
                //主控ARM程序的跳转地址
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[2], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainLoadAddrResgistryKeyName);
                //辅助ARM 头部校验
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[3], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeHeadCheckResgistryKeyName);
                //辅助ARM程序起始地址
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[4], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeStartAddrResgistryKeyName);
                //辅助ARM程序的跳转地址
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[5], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeLoadAddrResgistryKeyName);
                //用户
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[6], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName);
                //密码
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[7], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppPasswordResgistryKeyName);
                //主控ARM 的头部0xff的校验个数
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[8], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainBoardBootloaderCheckNum);
                //充电板ARM 的头部0xff的校验个数
                RegistryKeyLi.WriteRegistryKey(Commentclass.SettingMessageList[9], Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeBordBootloaderCheckNum);
                //标志位
                RegistryKeyLi.WriteRegistryKey("SUCCEED", Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentHSixRegFlagResgistryKeyName);

                Commentclass.ParaSource = "INI配置文件";
            }
            else if (RegReadTemp == "SUCCEED")//读注册表
            {
                //从注册表往回读
                //主控ARM 头部校验
                string Result = "";
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainHeadCheckResgistryKeyName, out Result);
                Commentclass.SettingMessageList[0] = Result;
                //主控ARM程序起始地址
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainStartAddrResgistryKeyName, out Result);
                Commentclass.SettingMessageList[1] = Result;
                //主控ARM程序的跳转地址
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainLoadAddrResgistryKeyName, out Result);
                Commentclass.SettingMessageList[2] = Result;

                //辅助ARM 头部校验
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeHeadCheckResgistryKeyName, out Result);
                Commentclass.SettingMessageList[3] = Result;
                //辅助ARM程序起始地址
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeStartAddrResgistryKeyName, out Result);
                Commentclass.SettingMessageList[4] = Result;
                //辅助ARM程序的跳转地址
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeLoadAddrResgistryKeyName, out Result);
                Commentclass.SettingMessageList[5] = Result;

                //用户
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName, out Result);
                Commentclass.SettingMessageList[6] = Result;
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppPasswordResgistryKeyName, out Result);
                Commentclass.SettingMessageList[7] = Result;

                //主控ARM 的头部校验的0xff的个数
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainBoardBootloaderCheckNum, out Result);
                Commentclass.SettingMessageList[8] = Result;

                //充电板ARM 的头部校验的0xff的个数
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeBordBootloaderCheckNum, out Result);
                Commentclass.SettingMessageList[9] = Result;

                Commentclass.ParaSource = "注册表";
            }
            else//使用INI
            {
                //注册表为空，将ini作为默认
                //主控ARM 头部校验
                Commentclass.SettingMessageList[0] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordBootloaderHeadFlagINI", "None", IniFilesPath);
                //主控ARM程序起始地址
                Commentclass.SettingMessageList[1] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordCodeStartAddrINI", "None", IniFilesPath);
                //主控ARM程序的跳转地址
                Commentclass.SettingMessageList[2] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordRedirectionAddrINI", "None", IniFilesPath);

                //辅助ARM 头部校验
                Commentclass.SettingMessageList[3] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordBootloaderHeadFlagINI", "None", IniFilesPath);
                //辅助ARM程序起始地址
                Commentclass.SettingMessageList[4] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordCodeStartAddrINI", "None", IniFilesPath);
                //辅助ARM程序的跳转地址
                Commentclass.SettingMessageList[5] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordRedirectionAddrINI", "None", IniFilesPath);

                //用户
                TempStr = ReadINIFiles.ReadIniData("UserConfig", "TargetUser", "None", IniFilesPath);
                switch (TempStr)
                {
                    case "User_StarsFD"://星辰
                        Commentclass.SettingMessageList[6] = "User_StarsFD";
                        Commentclass.SettingMessageList[7] = "090813";
                        break;
                    case "User_Client"://客户
                        Commentclass.SettingMessageList[6] = "User_Client";
                        Commentclass.SettingMessageList[7] = "2022";
                        break;
                    default:
                        break;
                }

                //主控ARM的0xff校验个数
                Commentclass.SettingMessageList[8] = ReadINIFiles.ReadIniData("MainBordMessage", "MainBoardBootloaderCheckNum", "None", IniFilesPath);
                //辅助ARM的0xff校验个数
                Commentclass.SettingMessageList[9] = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordBootloaderCheckNum", "None", IniFilesPath);

                Commentclass.ParaSource = "INI配置文件";
            }        


            Commentclass.User_APP = Commentclass.SettingMessageList[6];
            string[] nametake0 = Commentclass.User_APP.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Commentclass.Password = Commentclass.SettingMessageList[7];      
            if (nametake0[1] == Commentclass.LoginCheck[0] && Commentclass.Password == Commentclass.LoginCheck[1])//星辰用户
            {
                Program.isValidUser = 0x01;
                Commentclass.UserLevel = 0x01;
                this.Close();
            }
            else if (nametake0[1] == Commentclass.LoginCheck[2] && Commentclass.Password == Commentclass.LoginCheck[3])//客户
            {
                Commentclass.fmjump = 0x04;  //从窗口4条过去
                Program.isValidUser = 0x02;
                Commentclass.UserLevel = 0x00;
                this.Close();            
            }
            else
            {
                MessageBoxMidle.Show("用户名、密码有误", "错误");
                this.Close();
            }
            
        }
    }
}
