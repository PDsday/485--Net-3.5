using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
/***********************ModbusTcp功能码***********************
 * （1）01：读1路或者多路开关量线圈输出状态
 * （2）02：读1路或者多路开关量状态输入
 * （3）03：读多个寄存器
 * （4）05：写1路开关量输出
 * （5）06：写单个寄存器
 * （6）10：写多个寄存器 
 *                                          2022/07/26--LTT
 **************************************************************/
namespace _485刷机_Net_3._5
{
    /* ----------------------- Defines ------------------------------------------*/
    /* ----------------------- MBAP Header --------------------------------------*/
    /*
     * <------------------------ MODBUS TCP/IP ADU(1) ------------------------->
     *              <----------- MODBUS PDU (1') ---------------->
     *  +-----------+---------------+------------------------------------------+
     *  | TID | PID | Length | UID  |Code | Data                               |
     *  +-----------+---------------+------------------------------------------+
     *  |     |     |        |      |                                           
     * (2)   (3)   (4)      (5)    (6)                                          
     * (2)  ... MB_TCP_TID          = 0 (Transaction Identifier - 2 Byte) 
     * (3)  ... MB_TCP_PID          = 2 (Protocol Identifier - 2 Byte)
     * (4)  ... MB_TCP_LEN          = 4 (Number of bytes - 2 Byte)
     * (5)  ... MB_TCP_UID          = 6 (Unit Identifier - 1 Byte)
     * (6)  ... MB_TCP_FUNC         = 7 (Modbus Function Code)
     * (1)  ... Modbus TCP/IP Application Data Unit
     * (1') ... Modbus Protocol Data Unit
     * ----------------------------------------------------------------------------
     * | 事务处理标识符 |   协议标识    |     长度      |     单元标识符           |
     * |---------------------------------------------------------------------------|
     * |    2个字节     |    2个字节    |      2个字节  |        1个字节           |
     * -----------------------------------------------------------------------------
     */
    class Modbustcp
    {
        #region 功能码10：写多个寄存器（主）
        /****************************************************************************
         * 
         * 函数名称：
         * 
         * 输入：uinTid：计数值                
         *       uinUid：设备地址
         *       Regsvalue：数据
         *       Regaddress：寄存器起始地址
         *       Regquantity：寄存器数量    
         * 
         * 返回：无
         ******************************************************************************/
        public static void MasterWriteMultipleRegisters(ushort uinTid,byte uinUid,ushort Regaddress,ushort Regquantity, ushort[] Regsvalue)
        {
            ushort mblength = 0;
            Commentclass.HMI_Tx_modbus[0]  = (byte)(uinTid >> 8);      //事务处理标识符H
            Commentclass.HMI_Tx_modbus[1]  = (byte)uinTid;             //事务处理标识符L
            Commentclass.HMI_Tx_modbus[2]  = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[3]  = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[6]  = uinUid;                   //设备地址
            Commentclass.HMI_Tx_modbus[7]  = 0x10;                     //功能码
            Commentclass.HMI_Tx_modbus[8]  = (byte)(Regaddress >> 8);  //寄存器起始地址H
            Commentclass.HMI_Tx_modbus[9]  = (byte)Regaddress;         //寄存器起始地址L
            Commentclass.HMI_Tx_modbus[10] = (byte)(Regquantity >> 8); //寄存器数量
            Commentclass.HMI_Tx_modbus[11] = (byte)Regquantity;        //寄存器数量
            Commentclass.HMI_Tx_modbus[12] = (byte)(Regquantity * 2);  //所写数据内容长度
            mblength = 7;                                              //数据长度
            for (int i = 0; i < Regquantity; i++)
            {
                Commentclass.HMI_Tx_modbus[13 + i * 2] = (byte)(Regsvalue[i] >> 8);
                Commentclass.HMI_Tx_modbus[14 + i * 2] = (byte)Regsvalue[i];
                mblength += 2;
            }
            Commentclass.HMI_Tx_modbus[4] = (byte)(mblength >> 8);
            Commentclass.HMI_Tx_modbus[5] = (byte)mblength;

            Commentclass.CommentSendRegist[0] = Commentclass.HMI_Tx_modbus[8];
            Commentclass.CommentSendRegist[1] = Commentclass.HMI_Tx_modbus[9];
            Commentclass.CommentSendRegist[2] = Commentclass.HMI_Tx_modbus[10];
            Commentclass.CommentSendRegist[3] = Commentclass.HMI_Tx_modbus[11];
            //以太网发送
            SocketLi.SocketSendData(Commentclass.HMI_Tx_modbus, (mblength+0x06));
            //Console.WriteLine($"mblength = {(mblength + 0x06)}.");
            #region 告知显示线程
            Commentclass.CommentDisplayTemp = new byte[(mblength + 0x06)];
            Array.Copy(Commentclass.HMI_Tx_modbus, Commentclass.CommentDisplayTemp, (mblength + 0x06));
            Commentclass.CommentDisplayEventTarget = 0x01;
            #endregion
        }
        #endregion

        #region 功能码10：写多个寄存器（从）
        /****************************************************************************
         * 
         * 函数名称：
         * 
         * 输入：uinTid：计数值                
         *       uinUid：设备地址
         *       Regaddress：寄存器起始地址
         *       Regquantity：寄存器数量    
         * 
         * 返回：无
         ******************************************************************************/
        public static void SlaveWriteMultipleRegisters(ushort uinTid, byte uinUid, ushort Regaddress, ushort Regquantity)
        {
            ushort mblength = 0;
            Commentclass.HMI_Tx_modbus[0] = (byte)(uinTid >> 8);      //事务处理标识符H
            Commentclass.HMI_Tx_modbus[1] = (byte)uinTid;             //事务处理标识符L
            Commentclass.HMI_Tx_modbus[2] = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[3] = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[6] = uinUid;                   //设备地址
            Commentclass.HMI_Tx_modbus[7] = 0x10;                     //功能码
            Commentclass.HMI_Tx_modbus[8] = (byte)(Regaddress >> 8);  //寄存器起始地址H
            Commentclass.HMI_Tx_modbus[9] = (byte)Regaddress;         //寄存器起始地址L
            Commentclass.HMI_Tx_modbus[10] = (byte)(Regquantity >> 8);//寄存器数量
            Commentclass.HMI_Tx_modbus[11] = (byte)Regquantity;       //寄存器数量
            mblength = 0x06;                                             //数据长度
            Commentclass.HMI_Tx_modbus[4] = (byte)(mblength >> 8);
            Commentclass.HMI_Tx_modbus[5] = (byte)mblength;

            //以太网发送
            SocketLi.SocketSendData(Commentclass.HMI_Tx_modbus, (mblength + 0x06));
            #region 告知显示线程
            Commentclass.CommentDisplayTemp = new byte[(mblength + 0x06)];
            Array.Copy(Commentclass.HMI_Tx_modbus, Commentclass.CommentDisplayTemp, (mblength + 0x06));
            Commentclass.CommentDisplayEventTarget = 0x01;
            #endregion
        }
        #endregion

        #region 功能码0x06:写单个寄存器（主）
        /****************************************************************************
        * 
        * 函数名称：
        * 
        * 输入：uinTid：计数值                
        *       uinUid：设备地址
        *       Regsvalue：数据
        *       Regaddress：寄存器起始地址   
        * 返回：无
        ******************************************************************************/
        public static void MasterWriteSingleRegister(ushort uinTid, byte uinUid, ushort Regaddress, ushort Regsvalue)
        {
            ushort mblength = 0;

            Commentclass.HMI_Tx_modbus[0] = (byte)(uinTid >> 8);      //事务处理标识符H
            Commentclass.HMI_Tx_modbus[1] = (byte)uinTid;             //事务处理标识符L
            Commentclass.HMI_Tx_modbus[2] = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[3] = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[6] = uinUid;                   //设备地址
            Commentclass.HMI_Tx_modbus[7] = 0x06;                     //功能码
            Commentclass.HMI_Tx_modbus[8] = (byte)(Regaddress >> 8);  //寄存器起始地址H
            Commentclass.HMI_Tx_modbus[9] = (byte)Regaddress;         //寄存器起始地址L
            Commentclass.HMI_Tx_modbus[10] = (byte)(Regsvalue >> 8);  //数据内容H
            Commentclass.HMI_Tx_modbus[11] = (byte)Regsvalue;         //数据内容L
            mblength = 0X06;                                          //数据长度
            Commentclass.HMI_Tx_modbus[4] = (byte)(mblength >> 8);
            Commentclass.HMI_Tx_modbus[5] = (byte)mblength;

            //以太网发送
            SocketLi.SocketSendData(Commentclass.HMI_Tx_modbus, (mblength+0x06));
            #region 告知显示线程
            Commentclass.CommentDisplayTemp = new byte[(mblength + 0x06)];
            Array.Copy(Commentclass.HMI_Tx_modbus, Commentclass.CommentDisplayTemp, (mblength + 0x06));
            Commentclass.CommentDisplayEventTarget = 0x01;
            #endregion
        }
        #endregion

        #region 功能码0x06:写单个寄存器（从）
        /****************************************************************************
        * 
        * 函数名称：
        * 
        * 输入：uinTid：计数值                
        *       uinUid：设备地址
        *       Regsvalue：所写的数据
        *       Regaddress：寄存器起始地址   
        * 返回：无
        ******************************************************************************/
        public static void SlaveWriteSingleRegister(ushort uinTid, byte uinUid, ushort Regaddress, ushort Regsvalue)
        {
            ushort mblength = 0;
            Commentclass.HMI_Tx_modbus[0] = (byte)(uinTid >> 8);      //事务处理标识符H
            Commentclass.HMI_Tx_modbus[1] = (byte)uinTid;             //事务处理标识符L
            Commentclass.HMI_Tx_modbus[2] = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[3] = 0X00;                     //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[6] = uinUid;                   //设备地址
            Commentclass.HMI_Tx_modbus[7] = 0x06;                     //功能码
            Commentclass.HMI_Tx_modbus[8] = (byte)(Regaddress >> 8);  //寄存器起始地址H
            Commentclass.HMI_Tx_modbus[9] = (byte)Regaddress;         //寄存器起始地址L
            Commentclass.HMI_Tx_modbus[10] = (byte)(Regsvalue >> 8);  //数据内容H
            Commentclass.HMI_Tx_modbus[11] = (byte)Regsvalue;         //数据内容L
            mblength = 0X06;                                          //数据长度

            Commentclass.CommentReadWrite[Regaddress] = (byte)(Regsvalue >> 8);
            Commentclass.CommentReadWrite[Regaddress + 1] = (byte)Regsvalue;

            Commentclass.HMI_Tx_modbus[4] = (byte)(mblength >> 8);
            Commentclass.HMI_Tx_modbus[5] = (byte)mblength;

            //以太网发送
            SocketLi.SocketSendData(Commentclass.HMI_Tx_modbus, (mblength + 0x06));
            #region 告知显示线程
            Commentclass.CommentDisplayTemp = new byte[(mblength + 0x06)];
            Array.Copy(Commentclass.HMI_Tx_modbus, Commentclass.CommentDisplayTemp, (mblength + 0x06));
            Commentclass.CommentDisplayEventTarget = 0x01;
            #endregion
        }
        #endregion

        #region  功能码03：读多个寄存器(主)
        /****************************************************************************
        * 
        * 函数名称：
        * 
        * 输入：uinTid：计数值                
        *       uinUid：设备地址
        *       Regaddress：寄存器起始地址   
        *       Regquantity：寄存器数量   
        * 返回：无
        ******************************************************************************/
        public static void MasterReadMultipleRegisters(ushort uinTid, byte uinUid, ushort Regaddress, ushort Regquantity)
        {
            ushort mblength = 0;
            Commentclass.HMI_Tx_modbus[0] = (byte)(uinTid >> 8);        //事务处理标识符H
            Commentclass.HMI_Tx_modbus[1] = (byte)uinTid;               //事务处理标识符L
            Commentclass.HMI_Tx_modbus[2] = 0X00;                       //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[3] = 0X00;                       //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[6] = uinUid;                     //设备地址
            Commentclass.HMI_Tx_modbus[7] = 0x03;                       //功能码
            Commentclass.HMI_Tx_modbus[8] = (byte)(Regaddress >> 8);    //寄存器起始地址H
            Commentclass.HMI_Tx_modbus[9] = (byte)Regaddress;           //寄存器起始地址L
            Commentclass.HMI_Tx_modbus[10] = (byte)(Regquantity >> 8);  //寄存器数量H
            Commentclass.HMI_Tx_modbus[11] = (byte)Regquantity;         //寄存器数量L
            mblength = 0X06;
            Commentclass.HMI_Tx_modbus[4] = (byte)(mblength >> 8);
            Commentclass.HMI_Tx_modbus[5] = (byte)mblength;
            //以太网发送
            SocketLi.SocketSendData(Commentclass.HMI_Tx_modbus, (mblength + 0x06));
            #region 告知显示线程
            Commentclass.CommentDisplayTemp = new byte[(mblength + 0x06)];
            Array.Copy(Commentclass.HMI_Tx_modbus, Commentclass.CommentDisplayTemp, (mblength + 0x06));
            Commentclass.CommentDisplayEventTarget = 0x01;
            #endregion
        }
        #endregion

        #region  功能码03：读多个寄存器(从)
        /****************************************************************************
        * 
        * 函数名称：
        * 
        * 输入：uinTid：计数值                
        *       uinUid：设备地址
        *       Regaddress：寄存器起始地址   
        *       Regquantity：寄存器数量   
        * 返回：无
        ******************************************************************************/
        public static void SlaveReadMultipleRegisters(ushort uinTid, byte uinUid, ushort Regaddress, ushort Regquantity)
        {
            ushort mblength = 0;
            Commentclass.HMI_Tx_modbus[0] = (byte)(uinTid >> 8);        //事务处理标识符H
            Commentclass.HMI_Tx_modbus[1] = (byte)uinTid;               //事务处理标识符L
            Commentclass.HMI_Tx_modbus[2] = 0X00;                       //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[3] = 0X00;                       //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[6] = uinUid;                     //设备地址
            Commentclass.HMI_Tx_modbus[7] = 0x03;                       //功能码
            Commentclass.HMI_Tx_modbus[8] = (byte)(Regquantity * 2);    //返回的数据的长度
            mblength = 0X03;

            for (int i = 0; i < Regquantity; i++)
            {
                Commentclass.HMI_Tx_modbus[9 + i * 2] = (byte)(Commentclass.CommentReadWrite[i+ Regaddress] >> 8);
                Commentclass.HMI_Tx_modbus[10 + i * 2] = (byte)(Commentclass.CommentReadWrite[i + Regaddress]);
                mblength += 2;
            }
           
            Commentclass.HMI_Tx_modbus[4] = (byte)(mblength >> 8);
            Commentclass.HMI_Tx_modbus[5] = (byte)mblength;
            //以太网发送
            SocketLi.SocketSendData(Commentclass.HMI_Tx_modbus, (mblength + 0x06));
            #region 告知显示线程
            Commentclass.CommentDisplayTemp = new byte[(mblength + 0x06)];
            Array.Copy(Commentclass.HMI_Tx_modbus, Commentclass.CommentDisplayTemp, (mblength + 0x06));
            Commentclass.CommentDisplayEventTarget = 0x01;
            #endregion
        }
        #endregion

        #region 通信异常处理
        /****************************************************************************
        * 
        * 函数名称：
        * 
        * 输入：uinTid：计数值                
        *       uinUid：设备地址
        *       uinFcd：功能码
        *       uinEcd：异常码   
        * 返回：无
        ******************************************************************************/
        public static void ExxeptionResponse(ushort uinTid, byte uinUid, byte uinFcd, byte uinEcd)
        {
            ushort mblength = 0;
            Commentclass.HMI_Tx_modbus[0] = (byte)(uinTid >> 8);        //事务处理标识符H
            Commentclass.HMI_Tx_modbus[1] = (byte)uinTid;               //事务处理标识符L
            Commentclass.HMI_Tx_modbus[2] = 0X00;                       //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[3] = 0X00;                       //Modbustcp格式默认为0x00
            Commentclass.HMI_Tx_modbus[6] = uinUid;                     //设备地址
            Commentclass.HMI_Tx_modbus[7] = (byte)(uinFcd | 0x80);      //功能码+0x80
            Commentclass.HMI_Tx_modbus[8] = uinEcd;                     //异常码
            mblength = 0x03;
            Commentclass.HMI_Tx_modbus[4] = (byte)(mblength >> 8);
            Commentclass.HMI_Tx_modbus[5] = (byte)mblength;
            //以太网发送
            SocketLi.SocketSendData(Commentclass.HMI_Tx_modbus, (mblength + 0x06));
            #region 告知显示线程
            Commentclass.CommentDisplayTemp = new byte[(mblength + 0x06)];
            Array.Copy(Commentclass.HMI_Tx_modbus, Commentclass.CommentDisplayTemp, (mblength + 0x06));
            Commentclass.CommentDisplayEventTarget = 0x01;
            #endregion
        }
        #endregion

        #region ModbusTcp接收处理函数
        /****************************************************************************
       * 
       * 函数名称：
       * 
       * 输入：data：接收到数据              
       *
       *       Regsvalue：返回处理后的数据
       *       
       * 返回：无（单独一个线程调用它）
       ******************************************************************************/
        public static string ModbusResivePoll(byte[] data,int RealyLength)
        {
            string Result = "ok";
            ushort ResgierAddress, ResgierQuality, ResgierValues,MbtcpTid;
            int TempRealyLength  = (ushort)(data[4]<<8);
            TempRealyLength |= (ushort)data[4] & 0xff;
            try
            {
                //先校验设备地址，数据长度以及modbus协议格式
                //if (data[2] != 0x00 || data[3] != 0x00 || data[6] != Commentclass.CommentMbAddress || TempRealyLength != (RealyLength - 6))     //不符合modbustcp协议格式,设备地址不符合,长度不对
                if (data[2] != 0x00 || data[3] != 0x00 || data[6] != Commentclass.CommentMbAddress )     //不符合modbustcp协议格式,设备地址不符合,长度不对
                {
                    //消息帧错误以太网回复异常码
                    MbtcpTid = (ushort)(data[0] * 256 + data[1]);
                    ExxeptionResponse(MbtcpTid, Commentclass.CommentMbAddress, data[7],0x03);
                }
                else
                {
                    switch (data[7])    //功能码
                    {
                        case 0x03://读多个寄存器
                            MbtcpTid = (ushort)(data[0] * 256 + data[1]);                                                              //事务处理标识符
                            ResgierAddress = (ushort)(data[8] * 256 + data[9]);
                            Commentclass.CommentBackAddress[0] = ResgierAddress;//寄存器地址
                            ResgierQuality = (ushort)(data[10] * 256 + data[11]);
                            Commentclass.CommentBackAddress[1] = ResgierQuality; //寄存器数量
                           // SlaveReadMultipleRegisters(MbtcpTid, Commentclass.CommentMbAddress, ResgierAddress, ResgierQuality);       //回复
                            Commentclass.AboutMbState.ReadMultipleRegistersFlag = 0x01;
                            Commentclass.AboutMbState.WriteMultipleRegistersFlag = 0x00;
                            Commentclass.AboutMbState.WriteSingleRegisterFlag = 0x00;
                            break;

                        case 0x06://写单个寄存器
                            MbtcpTid = (ushort)(data[0] * 256 + data[1]);
                            ResgierAddress = (ushort)(data[8] * 256 + data[9]);
                            Commentclass.CommentBackAddress[0] = ResgierAddress;
                            ResgierValues = (ushort)(data[10] * 256 + data[11]);
                            Commentclass.CommentBackAddress[1] = ResgierValues;
                          //  SlaveWriteSingleRegister(MbtcpTid, Commentclass.CommentMbAddress, ResgierAddress, ResgierValues);           //回复
                            Commentclass.AboutMbState.ReadMultipleRegistersFlag = 0x00;
                            Commentclass.AboutMbState.WriteMultipleRegistersFlag = 0x00;
                            Commentclass.AboutMbState.WriteSingleRegisterFlag = 0x01;
                            break;

                        case 0x10://写多个寄存器
                            MbtcpTid = (ushort)(data[0] * 256 + data[1]);
                            ResgierAddress = (ushort)(data[8] * 256 + data[9]);
                            Commentclass.CommentBackAddress[0] = ResgierAddress;
                            ResgierQuality = (ushort)(data[10] * 256 + data[11]);
                            Commentclass.CommentBackAddress[1] = ResgierQuality;
                          //  SlaveWriteMultipleRegisters(MbtcpTid, Commentclass.CommentMbAddress, ResgierAddress, ResgierQuality);       //回复
                            Commentclass.AboutMbState.ReadMultipleRegistersFlag = 0x00;
                            Commentclass.AboutMbState.WriteMultipleRegistersFlag = 0x01;
                            Commentclass.AboutMbState.WriteSingleRegisterFlag = 0x00;
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }

            return Result;
        }
        #endregion

        #region 10功能码数据比较类01
        public static  bool ComparerWriteMultipleRegisters(byte[] data, ushort[] regs)
        {
            byte one = (byte)(regs[0] >> 8);
            byte two = (byte)regs[0];
            byte three = (byte)(regs[1] >> 8);
            byte four= (byte)regs[1];
            if ((one == data[0] | ((one & 0x01)==0x00) ) && three == data[2] && four == data[3])
            {
                return true;
            }
            else
            {
                //MessageBox.Show("ENPO为使能状态(DI0 = 1)，不允许以太网升级！","提示");
                return false;
            }
        }

        public static bool ComparerWriteMultipleRegisters2(byte[] data, ushort[] regs)
        {
            byte one = (byte)(regs[0] >> 8);
            byte two = (byte)regs[0];
            byte three = (byte)(regs[1] >> 8);
            byte four = (byte)regs[1];
            if ((one == data[0] | ((one & 0x01) == 0x00)) && three == data[2] && four == data[3])
            {
                return true;
            }
            else
            {
                MessageBox.Show("ENPO为使能状态(DI0 = 1)，不允许以太网升级！", "提示");
                return false;
            }
        }


        #endregion

        #region 10功能码数据比较类02
        public static bool ComparerWriteMultipleRegistersByte(byte[] data, byte[] regs)
        {
            if (data.Length != regs.Length)
            {
                return false;
            }
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != regs[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 将byte[]转换成为ushort[]
        public static string ByteConcertUshortSum(byte[] AimeByte, out ushort[] ConcertResut)
        {
            string Result = "ok";
            ConcertResut = new ushort[1];
            int LengthTemp = 0;
            int ConcertLmit = 0;
            try
            {
                LengthTemp = (AimeByte.Length / 2);
                ConcertLmit = LengthTemp;

                if ((AimeByte.Length % 2) > 0)
                {
                    LengthTemp = LengthTemp + 1;
                }
                ConcertResut = new ushort[LengthTemp];
                for (int i = 0; i < ConcertLmit; i++)
                {
                    ConcertResut[i] = (ushort)(AimeByte[i*2] * 256 + AimeByte[i*2 + 1]);
                }
                if (ConcertLmit != LengthTemp)
                {
                    ConcertResut[LengthTemp - 1] = AimeByte[AimeByte.Length - 1];
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
