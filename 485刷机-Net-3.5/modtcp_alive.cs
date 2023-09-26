using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _485刷机_Net_3._5
{
    class modtcp_alive
    {
        #region 变量定义区
        static public byte[] Bin_Head = new byte[13] { 0X00, 0X00, 0X00, 0X00, 0X00, 0X89, 0X00, 0X6E, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static public byte[] Ack_Head = new byte[8] { 0X00, 0X00, 0X00, 0X00, 0X00, 0X03, 0X00, 0X6E };
        #endregion

        #region 事物处理标识符
        static public UInt16 SWBS = 0X0000;                                        //modbustcp的事物处理标识符
        static public UInt16 STARADDR = 0X0000;                                    //起始地址（++128）
        static public UInt16 REGITE = 0X0000;                                       //寄存器数量（包数+++1）
        static public byte Length = 0x82;                                           //字节长度
        #endregion

        #region ***********************发送部分的相关定义函数***************************

        #region 发送代码更新数据
        static public byte[] DSP_MODTCP_ARMM(byte[] arry)
        {
            SWBS++;
            if (SWBS > 0XFFFF)
                SWBS = 0x01;
            //事物处理标识符
            Bin_Head[0] = (byte)(SWBS / 256);
            Bin_Head[1] = (byte)(SWBS % 256);
            //起始地址
            Bin_Head[8] = (byte)(STARADDR / 256);
            Bin_Head[9] = (byte)(STARADDR % 256);
            //寄存器数量
            Bin_Head[10] = (byte)(REGITE / 256);
            Bin_Head[11] = (byte)(REGITE % 256);
            //字节长度
            Bin_Head[12] = Length;

            byte[] alldata = new byte[Bin_Head.Length + arry.Length];
            Buffer.BlockCopy(Bin_Head, 0, alldata, 0, Bin_Head.Length);
            Buffer.BlockCopy(arry, 0, alldata, Bin_Head.Length, arry.Length);
            STARADDR = (UInt16)(STARADDR + 128);
            REGITE++;
            return alldata;
        }
        #endregion

        #region 上位机应答命令  04
        static public byte[] DSP_ORDE_MOD(byte arry)
        {
            SWBS++;
            if (SWBS > 0XFFFF)
                SWBS = 0x01;
            Ack_Head[0] = (byte)(SWBS / 256);
            Ack_Head[1] = (byte)(SWBS % 256);
            byte[] alldata = new byte[Ack_Head.Length + 1];
            Buffer.BlockCopy(Ack_Head, 0, alldata, 0, Ack_Head.Length);
            alldata[Ack_Head.Length] = arry;
            return alldata;
        }
        #endregion

        #region 单片机响应命令函数
        static public byte[] BMCU_ORDE_MOD(byte[] arry)
        {
            SWBS++;
            if (SWBS > 0XFFFF)
                SWBS = 0x01;
            byte[] alldata = new byte[arry.Length];
            Buffer.BlockCopy(arry, 0, alldata, 0, arry.Length);
            alldata[0] = (byte)(SWBS / 256);
            alldata[1] = (byte)(SWBS % 256);
            return alldata;
        }
        #endregion

        #endregion

        #region ************************接收数据处理部分的相关定义函数***************************
        static public byte[] Res_Adjt_ARM(byte step, byte[] arry)
        {
            byte[] temp_return = new byte[2];

            if (step == 0x01)//第一步
            {
                if (arry[7] == 0x6D)     //功能码的判断
                {
                    temp_return[0] = arry[8];
                    temp_return[1] = arry[9];
                    return temp_return;
                }
                else if (arry[7] == 0x6E)
                {
                    temp_return[0] = arry[8];
                    temp_return[1] = arry[9];
                    return temp_return;
                }
            }
            //else if (step == 0x02)
            //{
            //    if (arry[7] == 0x6E)
            //    {
            //        temp_return[0] = arry[8];      
            //        temp_return[1] = arry[9];     
            //        return temp_return;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            else
            {
                return null;
            }
            return null;
        }
        #endregion
    }
}
