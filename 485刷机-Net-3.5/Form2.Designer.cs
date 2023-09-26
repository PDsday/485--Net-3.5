
namespace _485刷机_Net_3._5
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.DSP刷机 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.主控板刷机 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.强制退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.cdserialbox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cdpoundbox = new System.Windows.Forms.ComboBox();
            this.cdloadbtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cdtextfile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cdviewbtn = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.timer6 = new System.Windows.Forms.Timer(this.components);
            this.timer7 = new System.Windows.Forms.Timer(this.components);
            this.timer8 = new System.Windows.Forms.Timer(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.databox = new System.Windows.Forms.RichTextBox();
            this.cdloadprogressBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DSP刷机,
            this.toolStripMenuItem1,
            this.主控板刷机,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(402, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // DSP刷机
            // 
            this.DSP刷机.BackColor = System.Drawing.SystemColors.ControlLight;
            this.DSP刷机.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DSP刷机.Name = "DSP刷机";
            this.DSP刷机.Size = new System.Drawing.Size(64, 20);
            this.DSP刷机.Tag = "0";
            this.DSP刷机.Text = "DSP升级";
            this.DSP刷机.Click += new System.EventHandler(this.DSP程序升级);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.Color.Chartreuse;
            this.toolStripMenuItem1.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(77, 20);
            this.toolStripMenuItem1.Tag = "1";
            this.toolStripMenuItem1.Text = "充ARM升级";
            // 
            // 主控板刷机
            // 
            this.主控板刷机.BackColor = System.Drawing.SystemColors.ControlLight;
            this.主控板刷机.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.主控板刷机.Name = "主控板刷机";
            this.主控板刷机.Size = new System.Drawing.Size(112, 20);
            this.主控板刷机.Tag = "2";
            this.主控板刷机.Text = "主ARM升级(485)";
            this.主控板刷机.Click += new System.EventHandler(this.主控板程序升级);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(112, 20);
            this.toolStripMenuItem2.Tag = "3";
            this.toolStripMenuItem2.Text = "主ARM升级(Eth)";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.强制退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 26);
            // 
            // 强制退出ToolStripMenuItem
            // 
            this.强制退出ToolStripMenuItem.Name = "强制退出ToolStripMenuItem";
            this.强制退出ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.强制退出ToolStripMenuItem.Text = "强制退出";
            this.强制退出ToolStripMenuItem.Click += new System.EventHandler(this.强制退出ToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(7, 190);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "端口：";
            // 
            // cdserialbox
            // 
            this.cdserialbox.FormattingEnabled = true;
            this.cdserialbox.Location = new System.Drawing.Point(46, 187);
            this.cdserialbox.Margin = new System.Windows.Forms.Padding(2);
            this.cdserialbox.Name = "cdserialbox";
            this.cdserialbox.Size = new System.Drawing.Size(92, 20);
            this.cdserialbox.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(179, 190);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "波特率：";
            // 
            // cdpoundbox
            // 
            this.cdpoundbox.FormattingEnabled = true;
            this.cdpoundbox.Items.AddRange(new object[] {
            "115200"});
            this.cdpoundbox.Location = new System.Drawing.Point(230, 187);
            this.cdpoundbox.Margin = new System.Windows.Forms.Padding(2);
            this.cdpoundbox.Name = "cdpoundbox";
            this.cdpoundbox.Size = new System.Drawing.Size(92, 20);
            this.cdpoundbox.TabIndex = 22;
            this.cdpoundbox.Text = "115200";
            this.cdpoundbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cdpoundbox_KeyDown);
            // 
            // cdloadbtn
            // 
            this.cdloadbtn.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cdloadbtn.Location = new System.Drawing.Point(328, 183);
            this.cdloadbtn.Margin = new System.Windows.Forms.Padding(2);
            this.cdloadbtn.Name = "cdloadbtn";
            this.cdloadbtn.Size = new System.Drawing.Size(62, 25);
            this.cdloadbtn.TabIndex = 23;
            this.cdloadbtn.Text = "下载";
            this.cdloadbtn.UseVisualStyleBackColor = true;
            this.cdloadbtn.Click += new System.EventHandler(this.cdloadbtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(7, 266);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 12);
            this.label3.TabIndex = 24;
            this.label3.Text = "文件：";
            // 
            // cdtextfile
            // 
            this.cdtextfile.AllowDrop = true;
            this.cdtextfile.Location = new System.Drawing.Point(46, 264);
            this.cdtextfile.Margin = new System.Windows.Forms.Padding(2);
            this.cdtextfile.Name = "cdtextfile";
            this.cdtextfile.Size = new System.Drawing.Size(276, 21);
            this.cdtextfile.TabIndex = 25;
            this.cdtextfile.DragDrop += new System.Windows.Forms.DragEventHandler(this.cdtextfile_DragDrop);
            this.cdtextfile.DragEnter += new System.Windows.Forms.DragEventHandler(this.cdtextfile_DragEnter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(7, 311);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "进度：";
            // 
            // cdviewbtn
            // 
            this.cdviewbtn.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cdviewbtn.Location = new System.Drawing.Point(328, 260);
            this.cdviewbtn.Margin = new System.Windows.Forms.Padding(2);
            this.cdviewbtn.Name = "cdviewbtn";
            this.cdviewbtn.Size = new System.Drawing.Size(62, 25);
            this.cdviewbtn.TabIndex = 28;
            this.cdviewbtn.Text = "浏览";
            this.cdviewbtn.UseVisualStyleBackColor = true;
            this.cdviewbtn.Click += new System.EventHandler(this.cdviewbtn_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4});
            this.statusStrip1.Location = new System.Drawing.Point(0, 337);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(402, 22);
            this.statusStrip1.TabIndex = 29;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(65, 17);
            this.toolStripStatusLabel1.Text = "充电板刷机";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(264, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusLabel3.Text = "Version";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(15, 17);
            this.toolStripStatusLabel4.Text = "0";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived_1);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 200;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // timer4
            // 
            this.timer4.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // timer5
            // 
            this.timer5.Interval = 1000;
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // timer6
            // 
            this.timer6.Enabled = true;
            this.timer6.Interval = 1500;
            this.timer6.Tick += new System.EventHandler(this.timer6_Tick);
            // 
            // timer7
            // 
            this.timer7.Tick += new System.EventHandler(this.timer7_Tick);
            // 
            // timer8
            // 
            this.timer8.Interval = 5000;
            this.timer8.Tick += new System.EventHandler(this.timer8_Tick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(7, 226);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 12);
            this.label5.TabIndex = 30;
            this.label5.Text = "模式：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "透传模式",
            "单板模式"});
            this.comboBox1.Location = new System.Drawing.Point(46, 223);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(92, 20);
            this.comboBox1.TabIndex = 31;
            this.comboBox1.Text = "透传模式";
            this.comboBox1.TextChanged += new System.EventHandler(this.comboBox1_TextChanged);
            // 
            // databox
            // 
            this.databox.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.databox.Location = new System.Drawing.Point(9, 21);
            this.databox.Margin = new System.Windows.Forms.Padding(2);
            this.databox.Name = "databox";
            this.databox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.databox.Size = new System.Drawing.Size(382, 158);
            this.databox.TabIndex = 41;
            this.databox.Text = "支持合并*.bin文件刷机。";
            this.databox.TextChanged += new System.EventHandler(this.databox_TextChanged);
            // 
            // cdloadprogressBar
            // 
            this.cdloadprogressBar.Location = new System.Drawing.Point(46, 305);
            this.cdloadprogressBar.Margin = new System.Windows.Forms.Padding(2);
            this.cdloadprogressBar.Name = "cdloadprogressBar";
            this.cdloadprogressBar.Size = new System.Drawing.Size(344, 18);
            this.cdloadprogressBar.TabIndex = 42;
            // 
            // Form2
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 359);
            this.Controls.Add(this.cdloadprogressBar);
            this.Controls.Add(this.databox);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.cdviewbtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cdtextfile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cdloadbtn);
            this.Controls.Add(this.cdpoundbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cdserialbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "星辰IAP-充电板";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.cdtextfile_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.cdtextfile_DragEnter);
            this.Move += new System.EventHandler(this.Form1_Move);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 主控板刷机;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem DSP刷机;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cdserialbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cdpoundbox;
        private System.Windows.Forms.Button cdloadbtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox cdtextfile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cdviewbtn;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Timer timer4;
        private System.Windows.Forms.Timer timer5;
        private System.Windows.Forms.Timer timer6;
        private System.Windows.Forms.Timer timer7;
        private System.Windows.Forms.Timer timer8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 强制退出ToolStripMenuItem;
        private System.Windows.Forms.RichTextBox databox;
        private System.Windows.Forms.ProgressBar cdloadprogressBar;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}