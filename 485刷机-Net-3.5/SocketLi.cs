using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace _485刷机_Net_3._5
{
   

    class SocketLi
    {
        public static Socket sockClient = null;                   //socket对象

        #region 以太网连接函数
        /****************************************************************************
        * 
        * 函数名称：SocketConnect
        * 
        * 输入：TargetIP：目标IP               
        *       TargetPort：目标端口号
        *       
        * 返回：无
        ******************************************************************************/
        public static string SocketConnect(string TargetIP,int TargetPort)
        {
            string Result = "ok";
            try
            {
                if (SocketPingNet(TargetIP) == "ok")
                {
                    IPAddress address = IPAddress.Parse(TargetIP);                     //连接IP
                    IPEndPoint Ipe = new IPEndPoint(address, TargetPort);                                      //连接端口
                    sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);       //将对象实例化
                    sockClient.Connect(Ipe);
                }
                else
                {
                    Result = "无法连接该网络端口";
                    return Result;
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
            return Result;
        }
        #endregion

        #region 以太网断开连接函数
        public static string SocketBreakConnect(string TargetIP)
        {
            try
            {
                if (SocketPingNet(TargetIP) == "ok")
                {
                    sockClient.Shutdown(SocketShutdown.Both);
                    return "ok"; 
                    sockClient = null;
                }
                else
                {
                    return "no";
                }
                //sockClient.Close();
            }
            catch (Exception err)
            {
                return err.Message;
            }
            
        }
        #endregion

        #region 以太网发送数据
        public static string SocketSendData(byte[] data,int length)
        {
            string Result = "ok";
            byte[] SendData = new byte[length];
            try
            {
                if (!Commentclass.SocketBreakFlag)
                {
                    Buffer.BlockCopy(data, 0, SendData, 0, length);
                    sockClient.Send(SendData);          //以太网发送数据
                }
            }
            catch (Exception err)
            {
               return err.Message;
            }
            return Result;
        }
        #endregion

        #region 以太网接收数据
        public static string SocketResiveData(out byte[] data)
        {
            data = new byte[1];     //初始化
            string Result = "ok";
            string NoResult = "Null";
            try
            {
                if (!Commentclass.SocketBreakFlag)
                {
                    byte[] arrMsgRec = new byte[1024 * 1024 * 2];           //定义一个2M的缓冲区
                    int length = sockClient.Receive(arrMsgRec);
                    // Console.WriteLine("接收：" + StartsCRC.zftring(arrMsgRec));
                    //Console.WriteLine($"以太网测试：length = {length}.");
                    if (length == 0)
                        return NoResult;
                    
                    data = new byte[length];
                    Array.Copy(arrMsgRec, data, length);
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
            return Result;
        }
        #endregion

        #region Ping网络函数
        public static string SocketPingNet(string TargetIP)
        {
            Ping pingSend = new Ping();
            PingReply reply = pingSend.Send(TargetIP, 100);
            if (reply.Status == IPStatus.Success)
            {
                return "ok";
            }
            else
            {
                return "no";
            }

        }
        #endregion

        #region tcplistener线程监听端口
        private void tcplistener(string TargetIP, int TargetPort)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(TargetIP), TargetPort);
            listener.Start();
        }
        #endregion
    }
}
