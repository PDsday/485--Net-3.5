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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "admin" && textBox2.Text == "9527")
            {
                if (Commentclass.set.IsDisposed)
                {
                    Setting set = new Setting();
                    set.Show();
                }
                else
                {
                    Commentclass.set.Show();
                }
                this.Close();
            }
            else
            {
                MessageBoxMidle.Show("账号或密码错误，请重试。", "错误");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "admin" && textBox2.Text == "9527")
                {
                    if (Commentclass.set.IsDisposed)
                    {
                        Setting set = new Setting();
                        set.Show();
                    }
                    else
                    {
                        Commentclass.set.Show();
                    }
                    this.Close();
                }
                else
                {
                    MessageBoxMidle.Show("账号或密码错误，请重试。", "错误");
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            int filebutton_x = button1.Location.X;
            int filebutton_y = button1.Location.Y;
          
            button1.Location = new Point(filebutton_x-4, filebutton_y + 2);

            filebutton_x = button2.Location.X;
            filebutton_y = button2.Location.Y;

            button2.Location = new Point(filebutton_x-4, filebutton_y + 2);
        }
    }
}
