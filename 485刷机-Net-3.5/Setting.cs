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
    public partial class Setting : Form
    {
        public string IniFilesPath = Application.StartupPath + "\\ConnectString.ini";//ini文件的路径
        public string TempStr = "";//字符串变量
        public string ComPathStr = "";
        public Setting()
        {
            InitializeComponent();
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.textBox1, "输入密码后回车确认。");
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            try
            {
                //判断注册表的标志是否存在，如果不存在就读取INi
                RegistryKeyLi.ReadRegistryKey(Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentHSixRegFlagResgistryKeyName, out TempStr);
                if (TempStr != "SUCCEED")//ini
                {
                    //主控ARM 头部校验个数
                    textBox2.Text = ReadINIFiles.ReadIniData("MainBordMessage", "MainBoardBootloaderCheckNum", "None", IniFilesPath);
                    //主控ARM程序起始地址
                    textBox5.Text = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordCodeStartAddrINI", "None", IniFilesPath);
                    //主控ARM程序的跳转地址
                    textBox6.Text = ReadINIFiles.ReadIniData("MainBordMessage", "MainBordRedirectionAddrINI", "None", IniFilesPath);

                    //辅助ARM 头部校验
                    textBox3.Text = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordBootloaderCheckNum", "None", IniFilesPath);
                    //辅助ARM程序起始地址
                    textBox8.Text = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordCodeStartAddrINI", "None", IniFilesPath);
                    //辅助ARM程序的跳转地址
                    textBox7.Text = ReadINIFiles.ReadIniData("ChargeBordMessage", "ChargeBordRedirectionAddrINI", "None", IniFilesPath);

                    //参数来源
                    TempStr = "INI配置文件";
                    textBox9.Text = TempStr;

                    //用户
                    TempStr = ReadINIFiles.ReadIniData("UserConfig", "TargetUser", "None", IniFilesPath);
                    switch (TempStr)
                    {
                        case "User_StarsFD"://星辰
                            comboBox1.SelectedIndex = 0;
                            break;
                        case "User_Client"://客户
                            comboBox1.SelectedIndex = 1;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //主控ARM 头部校验
                    textBox2.Text = Commentclass.SettingMessageList[8];
                    //主控ARM程序起始地址
                    textBox5.Text = Commentclass.SettingMessageList[1];
                    //主控ARM程序的跳转地址
                    textBox6.Text = Commentclass.SettingMessageList[2];

                    //辅助ARM 头部校验
                    textBox3.Text = Commentclass.SettingMessageList[9];
                    //辅助ARM程序起始地址
                    textBox8.Text = Commentclass.SettingMessageList[4];
                    //辅助ARM程序的跳转地址
                    textBox7.Text = Commentclass.SettingMessageList[5];

                    //参数来源
                    TempStr = "注册表";
                    textBox9.Text = TempStr;

                    //用户
                    TempStr = Commentclass.SettingMessageList[6]; ;
                    switch (TempStr)
                    {
                        case "User_StarsFD"://星辰
                            comboBox1.SelectedIndex = 0;
                            break;
                        case "User_Client"://客户
                            comboBox1.SelectedIndex = 1;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(err.Message, "err");
            }
        }

        //保存到注册表
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult messdr = MessageBoxMidle.Show(this, "确认保存修改？", "提示", MessageBoxButtons.YesNo);
                if (messdr == DialogResult.Yes)
                {                  
                    //保存主控ARM
                    TempStr = textBox2.Text;
                    RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainBoardBootloaderCheckNum);
                    //保存主控ARM 程序的起始地址
                    TempStr = textBox5.Text;
                    RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainStartAddrResgistryKeyName);
                    //保存主控ARM的程序的跳转地址
                    TempStr = textBox6.Text;
                    RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentMainLoadAddrResgistryKeyName);

                    //保存辅助ARM 
                    TempStr = textBox3.Text;
                    RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeBordBootloaderCheckNum);
                    //保存辅助ARM 的程序的起始地址
                    TempStr = textBox8.Text;
                    RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeStartAddrResgistryKeyName);
                    //保存辅助ARM 的程序的跳转地址
                    TempStr = textBox7.Text;
                    RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentChargeLoadAddrResgistryKeyName);

                    ////上位机名称
                    //TempStr = textBox9.Text;
                    //ReadINIFiles.WriteIniData("UserConfig", "ApplicationName", TempStr, IniFilesPath);

                    //用户权限
                    int varty = comboBox1.SelectedIndex;
                    switch (varty)
                    {
                        case 0://星辰
                            TempStr = "User_StarsFD";
                            RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName);
                            TempStr = "090813";
                            RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppPasswordResgistryKeyName);
                            break;
                        case 1://用户
                            TempStr = "User_Client";
                            RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppTargetResgistryKeyName);
                            TempStr = "2022";
                            RegistryKeyLi.WriteRegistryKey(TempStr, Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentAppPasswordResgistryKeyName);
                            break;
                        default:
                            break;
                    }
                    //标志位
                    RegistryKeyLi.WriteRegistryKey("SUCCEED", Commentclass.CommentPublicResgistryKeyPath, Commentclass.CommentHSixRegFlagResgistryKeyName);
                    MessageBoxMidle.Show("保存成功，请重启上位机生效修改。", "提示");
                    this.Close();
                }
                else
                {
                    MessageBoxMidle.Show("保存操作已取消。", "提示");
                }
            }
            catch (Exception err)
            {
                MessageBoxMidle.Show(err.Message, "保存失败");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult messdr = MessageBoxMidle.Show(this, "           确认退出修改？", "提示", MessageBoxButtons.YesNo);
            if (messdr == DialogResult.Yes)
            {
                this.Close();
            }
        }

        /// <summary>
        /// 获取修改权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_DoubleClick(object sender, EventArgs e)
        {
            //MessageBoxMidle.Show(this, "请输入开发者权限密码，获取修改权限。", "提示");  
        }
         /// <summary>
         /// 获取修改权限
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "221205")
                {
                    MessageBoxMidle.Show(this, "开发者权限获取成功。", "提示");
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    textBox5.Enabled = true;
                    textBox6.Enabled = true;
                    textBox8.Enabled = true;
                    textBox7.Enabled = true;
                }
                else
                {
                    MessageBoxMidle.Show(this, "密码输入错误，请重新输入。", "错误");
                }
            }
        }
    }
}
