using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _485刷机_Net_3._5
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] data = Create_03_Frame(((short)(Convert.ToInt16(this.textBox1.Text))), 0x0001);
            Console.WriteLine(StartsCRC.zftring(data));
            this.label2.Text = StartsCRC.zftring(data);

            if (SocketLi.SocketConnect(Commentclass.CommentIP, 502) == "ok")
            {
                SocketLi.SocketSendData(data, data.Length);
                SocketLi.SocketResiveData(out data);
                if (data == null)
                {
                    this.label7.Text = "无响应";
                    this.label3.ForeColor  = Color.Red;
                }
                else if (data.Length < 7)
                {
                    this.label7.Text = "响应错误";
                    this.label3.ForeColor = Color.Red;
                }
                else
                {
                    byte[] temp = new byte[2];
                    temp[0] = data[data.Length - 2];
                    temp[1] = data[data.Length - 1];
                    ushort re = (ushort)(temp[0] * 256 + temp[1]);
                    this.label3.Text = StartsCRC.zftring(data) ;
                    this.label7.Text = Convert.ToString(re);
                    this.label3.ForeColor = Color.Black;
                }

                SocketLi.SocketBreakConnect(Commentclass.CommentIP);
            }
            else
            {
                this.label7.Text = "网络错误";
            }


        }


        //MBAP
        public static int Transacation_identifier = 0x0001;    //一般情况下，每发一包数据，该报头需要+1，0000--FFFF循环，
        public static byte[] Protocol_identifier = new byte[2] { 0x00, 0x00 };       //ModBus-TCP为0x0000
        public static byte[] Lenth = new byte[2] { 0x00, 0x00 };                     //数据长度需要按实际Data进行计算
        public static byte Unit_identifier = 0x00;                                 //目前为星辰工程多为单机通讯，如需要特殊情况再按需修改





        /// <summary>
        /// 生成MBAP包头
        /// </summary>
        /// <returns></returns>
        private static byte[] Create_MBAP_Frame()
        {
            try
            {
                byte[] ret_byte = new byte[7];

                ret_byte[0] = (byte)(Transacation_identifier / 256);
                ret_byte[1] = (byte)Transacation_identifier;

                Buffer.BlockCopy(Protocol_identifier, 0, ret_byte, 2, Protocol_identifier.Length);
                Buffer.BlockCopy(Lenth, 0, ret_byte, 2, Protocol_identifier.Length);

                ret_byte[6] = Unit_identifier;


                if (Transacation_identifier++ > 0xFFFF)
                {
                    Transacation_identifier = 0x0001;
                }

                return ret_byte;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 生成03数据帧，访问保持寄存器的内容
        /// </summary>
        /// <param name="start_adress"></param>
        /// <param name="register_num"></param>
        /// <returns></returns>
        public static byte[] Create_03_Frame(short start_adress, short register_num)
        {
            try
            {
                byte[] result = null;

                byte[] mbap = Create_MBAP_Frame();

                #region data_frame
                byte[] data_frame = new byte[4];
                data_frame[0] = (byte)(start_adress / 256);
                data_frame[1] = (byte)start_adress;
                data_frame[2] = (byte)(register_num / 256);
                data_frame[3] = (byte)register_num;
                mbap[5] = (byte)(2 + data_frame.Length);
                #endregion
                result = new byte[mbap.Length + 1 + data_frame.Length];
                Buffer.BlockCopy(mbap, 0, result, 0, mbap.Length);                           //填充MBAP包头
                result[mbap.Length] = 0x03;                                                  //填充功能码
                Buffer.BlockCopy(data_frame, 0, result, mbap.Length + 1, data_frame.Length); //填充Data_Frame包
                return result;
            }
            catch
            {
                return null;
            }

        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(this.button1, EventArgs.Empty);
            }
        }
    }
}
