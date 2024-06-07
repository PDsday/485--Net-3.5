
namespace _485刷机_Net_3._5
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.cir_lode_Label2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.强制退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.ipaddres = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pathtextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.loadprogressBar = new System.Windows.Forms.ProgressBar();
            this.filebutton = new System.Windows.Forms.Button();
            this.downbutton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.timer6 = new System.Windows.Forms.Timer(this.components);
            this.timer7 = new System.Windows.Forms.Timer(this.components);
            this.databox = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.David_contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.清除数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.存入文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.DSP刷机 = new System.Windows.Forms.ToolStripMenuItem();
            this.以太网测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.主控板刷机 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.David_contextMenuStrip2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.statusStrip1, 2);
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.cir_lode_Label2,
            this.toolStripStatusLabel5,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4});
            this.statusStrip1.Location = new System.Drawing.Point(0, 397);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(609, 30);
            this.statusStrip1.TabIndex = 30;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(52, 25);
            this.toolStripStatusLabel1.Text = "DSP升级";
            // 
            // cir_lode_Label2
            // 
            this.cir_lode_Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.cir_lode_Label2.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cir_lode_Label2.Name = "cir_lode_Label2";
            this.cir_lode_Label2.Size = new System.Drawing.Size(408, 25);
            this.cir_lode_Label2.Spring = true;
            this.cir_lode_Label2.Text = "点击循环测试";
            this.cir_lode_Label2.Click += new System.EventHandler(this.toolStripStatusLabel2_Click);
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.ForeColor = System.Drawing.Color.Blue;
            this.toolStripStatusLabel5.IsLink = true;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(22, 25);
            this.toolStripStatusLabel5.Text = "(s)";
            this.toolStripStatusLabel5.Click += new System.EventHandler(this.toolStripStatusLabel5_Click);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(101, 25);
            this.toolStripStatusLabel3.Text = "Version:2024-5-6";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(15, 25);
            this.toolStripStatusLabel4.Text = "0";
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
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 38);
            this.label1.TabIndex = 32;
            this.label1.Text = "地址：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ipaddres
            // 
            this.ipaddres.AccessibleRole = System.Windows.Forms.AccessibleRole.IpAddress;
            this.ipaddres.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ipaddres.Location = new System.Drawing.Point(2, 2);
            this.ipaddres.Margin = new System.Windows.Forms.Padding(2);
            this.ipaddres.Name = "ipaddres";
            this.ipaddres.Size = new System.Drawing.Size(99, 21);
            this.ipaddres.TabIndex = 33;
            this.ipaddres.Text = "192.168.1.15";
            this.ipaddres.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ipaddres_KeyDown);
            this.ipaddres.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ipaddres_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(2, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 38);
            this.label2.TabIndex = 34;
            this.label2.Text = "文件：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pathtextBox
            // 
            this.pathtextBox.AllowDrop = true;
            this.tableLayoutPanel2.SetColumnSpan(this.pathtextBox, 2);
            this.pathtextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathtextBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pathtextBox.Location = new System.Drawing.Point(51, 48);
            this.pathtextBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.pathtextBox.Name = "pathtextBox";
            this.pathtextBox.Size = new System.Drawing.Size(463, 21);
            this.pathtextBox.TabIndex = 35;
            this.pathtextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragDrop);
            this.pathtextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragEnter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(2, 76);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 38);
            this.label4.TabIndex = 36;
            this.label4.Text = "进度：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // loadprogressBar
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.loadprogressBar, 2);
            this.loadprogressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadprogressBar.Location = new System.Drawing.Point(51, 83);
            this.loadprogressBar.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.loadprogressBar.Name = "loadprogressBar";
            this.loadprogressBar.Size = new System.Drawing.Size(463, 24);
            this.loadprogressBar.TabIndex = 37;
            // 
            // filebutton
            // 
            this.filebutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filebutton.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.filebutton.Location = new System.Drawing.Point(522, 43);
            this.filebutton.Margin = new System.Windows.Forms.Padding(5);
            this.filebutton.Name = "filebutton";
            this.filebutton.Size = new System.Drawing.Size(70, 28);
            this.filebutton.TabIndex = 38;
            this.filebutton.Text = "浏览";
            this.filebutton.UseVisualStyleBackColor = true;
            this.filebutton.Click += new System.EventHandler(this.filebutton_Click);
            // 
            // downbutton
            // 
            this.downbutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downbutton.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.downbutton.Location = new System.Drawing.Point(522, 81);
            this.downbutton.Margin = new System.Windows.Forms.Padding(5);
            this.downbutton.Name = "downbutton";
            this.downbutton.Size = new System.Drawing.Size(70, 28);
            this.downbutton.TabIndex = 39;
            this.downbutton.Text = "下载";
            this.downbutton.UseVisualStyleBackColor = true;
            this.downbutton.Click += new System.EventHandler(this.downbutton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // timer4
            // 
            this.timer4.Interval = 200;
            this.timer4.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // timer5
            // 
            this.timer5.Interval = 5000;
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // timer6
            // 
            this.timer6.Interval = 3500;
            this.timer6.Tick += new System.EventHandler(this.timer6_Tick);
            // 
            // timer7
            // 
            this.timer7.Interval = 1000;
            this.timer7.Tick += new System.EventHandler(this.timer7_Tick);
            // 
            // databox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.databox, 2);
            this.databox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databox.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.databox.Location = new System.Drawing.Point(2, 32);
            this.databox.Margin = new System.Windows.Forms.Padding(2);
            this.databox.Name = "databox";
            this.databox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.databox.Size = new System.Drawing.Size(605, 183);
            this.databox.TabIndex = 40;
            this.databox.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(133, 2);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(76, 21);
            this.textBox1.TabIndex = 41;
            this.textBox1.Text = "       X6A";
            // 
            // David_contextMenuStrip2
            // 
            this.David_contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.清除数据ToolStripMenuItem,
            this.存入文件ToolStripMenuItem});
            this.David_contextMenuStrip2.Name = "David_contextMenuStrip2";
            this.David_contextMenuStrip2.Size = new System.Drawing.Size(125, 48);
            // 
            // 清除数据ToolStripMenuItem
            // 
            this.清除数据ToolStripMenuItem.Name = "清除数据ToolStripMenuItem";
            this.清除数据ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.清除数据ToolStripMenuItem.Tag = "0";
            this.清除数据ToolStripMenuItem.Text = "清除数据";
            this.清除数据ToolStripMenuItem.Click += new System.EventHandler(this.ContextStripMenuItem_Click);
            // 
            // 存入文件ToolStripMenuItem
            // 
            this.存入文件ToolStripMenuItem.Name = "存入文件ToolStripMenuItem";
            this.存入文件ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.存入文件ToolStripMenuItem.Tag = "1";
            this.存入文件ToolStripMenuItem.Text = "导出文件";
            this.存入文件ToolStripMenuItem.Click += new System.EventHandler(this.ContextStripMenuItem_Click);
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Location = new System.Drawing.Point(51, 117);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(44, 26);
            this.textBox2.TabIndex = 43;
            this.textBox2.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(3, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 40);
            this.label3.TabIndex = 44;
            this.label3.Text = "成功:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(440, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 40);
            this.label5.TabIndex = 45;
            this.label5.Text = "总数:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox3.Location = new System.Drawing.Point(520, 117);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(44, 26);
            this.textBox3.TabIndex = 46;
            this.textBox3.Text = "0";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.67024F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.32976F));
            this.tableLayoutPanel1.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.databox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(609, 427);
            this.tableLayoutPanel1.TabIndex = 47;
            // 
            // menuStrip1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.menuStrip1, 2);
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DSP刷机,
            this.toolStripMenuItem1,
            this.toolStripMenuItem4,
            this.主控板刷机,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(609, 30);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.Menustrip_Change);
            // 
            // DSP刷机
            // 
            this.DSP刷机.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.DSP刷机.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.以太网测试ToolStripMenuItem,
            this.toolStripMenuItem5});
            this.DSP刷机.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DSP刷机.Name = "DSP刷机";
            this.DSP刷机.Size = new System.Drawing.Size(118, 26);
            this.DSP刷机.Tag = "0";
            this.DSP刷机.Text = "DSP以太网升级";
            // 
            // 以太网测试ToolStripMenuItem
            // 
            this.以太网测试ToolStripMenuItem.Name = "以太网测试ToolStripMenuItem";
            this.以太网测试ToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.以太网测试ToolStripMenuItem.Text = "以太网测试";
            this.以太网测试ToolStripMenuItem.Click += new System.EventHandler(this.以太网测试ToolStripMenuItem_Click_1);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(149, 22);
            this.toolStripMenuItem5.Text = "参数查询";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuItem1.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(118, 26);
            this.toolStripMenuItem1.Tag = "1";
            this.toolStripMenuItem1.Text = "充电器485升级";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuItem4.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(139, 26);
            this.toolStripMenuItem4.Tag = "2";
            this.toolStripMenuItem4.Text = "充电器以太网升级";
            // 
            // 主控板刷机
            // 
            this.主控板刷机.BackColor = System.Drawing.SystemColors.ControlLight;
            this.主控板刷机.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.主控板刷机.Name = "主控板刷机";
            this.主控板刷机.Size = new System.Drawing.Size(105, 26);
            this.主控板刷机.Tag = "3";
            this.主控板刷机.Text = "ARM 485升级";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuItem2.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(118, 26);
            this.toolStripMenuItem2.Tag = "4";
            this.toolStripMenuItem2.Text = "ARM以太网升级";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuItem3.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(94, 26);
            this.toolStripMenuItem3.Tag = "5";
            this.toolStripMenuItem3.Text = "DspBootLoad";
            this.toolStripMenuItem3.Visible = false;
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 220);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(603, 174);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBox3, 3, 3);
            this.tableLayoutPanel2.Controls.Add(this.pathtextBox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.loadprogressBar, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBox2, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.filebutton, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.downbutton, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(597, 154);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ipaddres);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(51, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(383, 32);
            this.panel1.TabIndex = 42;
            // 
            // Form3
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 427);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DSP283XX_以太网升级";
            this.Activated += new System.EventHandler(this.Form3_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.Load += new System.EventHandler(this.David_Form3_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragEnter);
            this.Move += new System.EventHandler(this.Form3_Move);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.David_contextMenuStrip2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel cir_lode_Label2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ipaddres;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox pathtextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar loadprogressBar;
        private System.Windows.Forms.Button filebutton;
        private System.Windows.Forms.Button downbutton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Timer timer4;
        private System.Windows.Forms.Timer timer5;
        private System.Windows.Forms.Timer timer6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 强制退出ToolStripMenuItem;
        private System.Windows.Forms.Timer timer7;
        private System.Windows.Forms.RichTextBox databox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ContextMenuStrip David_contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 清除数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 存入文件ToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem DSP刷机;
        private System.Windows.Forms.ToolStripMenuItem 以太网测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem 主控板刷机;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
    }
}