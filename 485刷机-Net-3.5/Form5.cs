using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _485刷机_Net_3._5
{
    public partial class Form5 : Form
    {
        public int Step_Now = 0;
        public bool IsRuning = true;
        public int TryCountTime = 0;
        Socket sockClient = null;                   //socket对象
        Thread thrClient = null;                    //接收线程

        public byte[] ZHILING1 = new byte[12] { 0x00, 0x02, 0x00, 0x00, 0x00, 0x06, 0x00, 0x03, 0x00, 0xD6, 0x00, 0x01 };//00 02 00 00 00 05 00 03 02 00 00
        public byte[] ZHILING2 = new byte[12] { 0x00, 0x03, 0x00, 0x00, 0x00, 0x06, 0x00, 0x03, 0x00, 0xD3, 0x00, 0x01 };//00 03 00 00 00 05 00 03 02 00 00
                                                                                                                         // 0  1  2  3  4  5  6  7  8  9  10
        public byte[] CheckData = new byte[6] { 0x05, 0x00, 0x03, 0x02, 0x00, 0x00 };
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            //通过ImageLocation设置网络图片
            //this.pictureBox1.ImageLocation = Application.StartupPath + "\\icon\\ok.ico";
            //this.pictureBox1.ImageLocation = Application.StartupPath + "\\icon\\err.ico";
            this.pictureBox1.ImageLocation = Application.StartupPath + "\\icon\\waiter.ico";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        //任务梯度执行
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
          
                switch (Step_Now)
                {
                    case 0x00://先ping通
                        if (TryCountTime < 3)
                        {
                            TryCountTime++;
                            Ping pingSend = new Ping();
                            PingReply reply = pingSend.Send(Commentclass.CommentIP, 50);    //200
                            if (reply.Status == IPStatus.Success)
                            {
                                Step_Now = 0x01;
                                TryCountTime = 0;
                            }
                            pingSend.Dispose();

                        }
                        else
                        {
                            TryCountTime = 0;
                            timer1.Stop();
                            this.pictureBox1.ImageLocation = Application.StartupPath + "\\icon\\err.ico";
                        }
                        break;
                    case 0x01://连接服务器
                        IPAddress address = IPAddress.Parse(Commentclass.CommentIP);                     //连接IP
                        IPEndPoint Ipe = new IPEndPoint(address, 502);                                      //连接端口
                        sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);       //将对象实例化
                        sockClient.Connect(Ipe);
                        thrClient = new Thread(ReceiceMsg);
                        thrClient.IsBackground = true;
                        thrClient.Start();
                        Step_Now = 2;
                        break;
                    case 0x02://开始查询第一帧
                        Net_Work_Send_Ncrc(ZHILING1);
                        break;
                    case 0x03://开始查询第二帧
                        Net_Work_Send_Ncrc(ZHILING2);
                        break;
                    case 0x04://断开连接
                        sockClient.Shutdown(SocketShutdown.Both);
                        sockClient = null;
                        thrClient.Abort(); //关闭线程
                        Step_Now = 5;
                        break;
                    case 0x05://一直在线ping
                        this.pictureBox1.ImageLocation = Application.StartupPath + "\\icon\\ok.ico";
                        Ping pingSend1 = new Ping();
                        PingReply reply1 = pingSend1.Send(Commentclass.CommentIP, 50);    //200
                        if (reply1.Status == IPStatus.Success)
                        {
                            Step_Now = 0x05;
                            pingSend1.Dispose();
                        }
                        else
                        {
                            timer1.Stop();
                            Step_Now = 0x00;
                            label3.Text = "00 00 00 00 00 00 00 00 00 00 00 00";
                            label4.Text = "00 00 00 00 00 00 00 00 00 00 00";
                            this.pictureBox1.ImageLocation = Application.StartupPath + "\\icon\\waiter.ico";
                        }
                        break;
                }
            }
            catch(Exception err)
            {
                timer1.Stop();
                Step_Now = 0x00;
               //sockClient.Shutdown(SocketShutdown.Both);
                sockClient = null;
                thrClient= null; //关闭线程
                this.pictureBox1.ImageLocation = Application.StartupPath + "\\icon\\err.ico";
                //MessageBox.Show(err.Message);
            }
        }
        #region 以太网接收消息
        private void ReceiceMsg()
        {
            while (IsRuning)
            {
                try
                {
                    byte[] arrMsgRec = new byte[1024 * 1024 * 2];           //定义一个2M的缓冲区
                    int length = sockClient.Receive(arrMsgRec);
                    if (length == 0) return;
                    //Console.WriteLine($"length = {length}.");
                    byte[] Resive_data = new byte[length];
                    Array.Copy(arrMsgRec, Resive_data, length);

                    Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                    {
                        label4.Text = StartsCRC.zftring(Resive_data);
                        for (int i = 0; i < 6; i++)
                        {
                            if (CheckData[i] != Resive_data[i + 5])
                            {   

                                break;
                            }
                        }
                        Step_Now++;
                    })));
                }
                catch (SocketException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
        #endregion
        #region 以太网发送函数（无CRC校验）
        private void Net_Work_Send_Ncrc(byte[] data)
        {
            try  //星辰CRC校验
            {
                Invoke((new System.Action(() =>                     //c#3.0以后代替委托的新方法
                {
                    label3.Text = StartsCRC.zftring(data);
                    sockClient.Send(data);                          //以太网发送数据
                    //显示在lable上
                })));
            }
            catch
            {            
              
            }

        }
        #endregion

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            Step_Now = 0x00;
            // sockClient.Shutdown(SocketShutdown.Both);
            sockClient = null;
            thrClient = null; //关闭线程
            Commentclass.fm3.Show();
            this.Dispose();
        }
    }
}
