using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/***********************注册表********************************
 * 说明：将文件路径，窗体关闭等信息记录在电脑的内存
 *                                          2022/12/02--LTT
 *                                          
 *                                          
 *     路径定义：（1）保存配置文件注册表的路径： @"STARSH6\SETTING";                                        
 *                                     
 *     文件夹名称：                                     
 *                 （1）MainBordBootloaderHeadFlag：主控头部校验的16个参数
 *                 （2）MainBordCodeStartAddr：主控ARM程序的开始地址                     
 *                 （3）MainBordRedirectionAddr：主控ARM程序的跳转地址   
 *                 （4）ChargeBordBootloaderHeadFlag：充电板头部校验的16个数值
 *                 （5）ChargeBordCodeStartAddr：充电器程序的起始地址
 *                 （6）ChargeBordRedirectionAddr：充电器程序的跳转地址
 *                 （7）TargetUser：用户目标的名称
 *                 （8）Password：密码
 *                 （9）HSixRegFlag:注册表标志位
 *                                          
 **************************************************************/

namespace _485刷机_Net_3._5
{

    class RegistryKeyLi
    {

        #region 写注册表记录信息
        /****************************************************************************
         * 
         * 函数名称：WriteRegistryKey
         * 
         * 输入：FilesPath：   需要保存的字符串     
         *       ResgistryKeyPath：保存到注册表的路径
         *       ResgistryKeyName: 保存到注册表的文件夹名称
         * 返回：状态
         ******************************************************************************/
        public static string WriteRegistryKey(string FilesPath, string ResgistryKeyPath, string ResgistryKeyName)
        {
            string Result = "ok";
            try
            {
                Microsoft.Win32.RegistryKey MineSaveKey;
                MineSaveKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ResgistryKeyPath, true);
                if (MineSaveKey == null)
                {
                    MineSaveKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(ResgistryKeyPath);
                }
                MineSaveKey.SetValue(ResgistryKeyName, FilesPath);
            }
            catch (Exception err)
            {
                return err.Message;
            }
            return Result;
        }
        #endregion
        #region 读取注册表里面存储的路径信息
        /****************************************************************************
        * 
        * 函数名称：WriteRegistryKey
        * 
        * 输入：   ResgistryKeyPath：保存到注册表的路径
        *          ResgistryKeyName: 保存到注册表的文件夹名称
        *          
        * 返回:需要读取的信息
        ******************************************************************************/
        public static string ReadRegistryKey(string ResgistryKeyPath, string ResgistryKeyName, out string TargetMessage)
        {
            TargetMessage = "";
            string Result = "ok";
            try
            {
                Microsoft.Win32.RegistryKey MineReadKey;
                MineReadKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ResgistryKeyPath, true);
                TargetMessage = MineReadKey.GetValue(ResgistryKeyName).ToString();
                if (TargetMessage == null)
                {
                    TargetMessage = "D:\\";
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
            return Result;
        }
        #endregion
        #region 防止注册表不存在造成的错误
        /****************************************************************************
        * 
        * 函数名称：PreventCreadErr(用于注册表标志位的防错判断)
        * 
        * 输入：   ResgistryKeyPath：保存到注册表的路径
        *          ResgistryKeyName: 保存到注册表的文件夹名称
        *          
        * 返回:
        ******************************************************************************/
        public static string PreventCreadErr(string ResgistryKeyPath, string ResgistryKeyName)
        {
            string Result = "ok";
            try
            {
                Microsoft.Win32.RegistryKey PrevenRegisKey;
                PrevenRegisKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ResgistryKeyPath, true);
                if (PrevenRegisKey == null)
                {
                    PrevenRegisKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(ResgistryKeyPath);
                    PrevenRegisKey.SetValue(ResgistryKeyName, "NONE");
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
            return Result;
        }
        #endregion


    }
}
