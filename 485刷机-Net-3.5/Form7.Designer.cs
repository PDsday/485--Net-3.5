
namespace _485刷机_Net_3._5
{
    partial class Form7
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form7));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.清除数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出文本ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Mainbordenu = new System.Windows.Forms.ToolStripMenuItem();
            this.Chargebordenu = new System.Windows.Forms.ToolStripMenuItem();
            this.Dspbordenu = new System.Windows.Forms.ToolStripMenuItem();
            this.Dspbordenu1 = new System.Windows.Forms.ToolStripMenuItem();
            this.一键擦除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ModelBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pathtextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ipaddres = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DownloadRate = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.btnlink = new System.Windows.Forms.Button();
            this.btnlabview = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statelable = new System.Windows.Forms.ToolStripStatusLabel();
            this.pointlable = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.versionlable = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StateControl_timer = new System.Windows.Forms.Timer(this.components);
            this.AckTimerOut_timer = new System.Windows.Forms.Timer(this.components);
            this.SocketConnectTimer = new System.Windows.Forms.Timer(this.components);
            this.DisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.WaiteOneSeconTimer = new System.Windows.Forms.Timer(this.components);
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.databox = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.清除数据ToolStripMenuItem,
            this.导出文本ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            // 
            // 清除数据ToolStripMenuItem
            // 
            this.清除数据ToolStripMenuItem.Name = "清除数据ToolStripMenuItem";
            this.清除数据ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.清除数据ToolStripMenuItem.Tag = "0";
            this.清除数据ToolStripMenuItem.Text = "清除数据";
            this.清除数据ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // 导出文本ToolStripMenuItem
            // 
            this.导出文本ToolStripMenuItem.Name = "导出文本ToolStripMenuItem";
            this.导出文本ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.导出文本ToolStripMenuItem.Tag = "1";
            this.导出文本ToolStripMenuItem.Text = "导出文本";
            this.导出文本ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mainbordenu,
            this.Chargebordenu,
            this.Dspbordenu,
            this.Dspbordenu1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(404, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "升级对象选择";
            // 
            // Mainbordenu
            // 
            this.Mainbordenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Mainbordenu.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Mainbordenu.Name = "Mainbordenu";
            this.Mainbordenu.Size = new System.Drawing.Size(64, 20);
            this.Mainbordenu.Tag = "0";
            this.Mainbordenu.Text = "DSP升级";
            this.Mainbordenu.Click += new System.EventHandler(this.HomeSwitcher);
            // 
            // Chargebordenu
            // 
            this.Chargebordenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Chargebordenu.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Chargebordenu.Name = "Chargebordenu";
            this.Chargebordenu.Size = new System.Drawing.Size(77, 20);
            this.Chargebordenu.Tag = "1";
            this.Chargebordenu.Text = "充ARM升级";
            this.Chargebordenu.Click += new System.EventHandler(this.HomeSwitcher);
            // 
            // Dspbordenu
            // 
            this.Dspbordenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Dspbordenu.Font = new System.Drawing.Font("楷体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Dspbordenu.Name = "Dspbordenu";
            this.Dspbordenu.Size = new System.Drawing.Size(112, 20);
            this.Dspbordenu.Tag = "2";
            this.Dspbordenu.Text = "主ARM升级(485)";
            this.Dspbordenu.Click += new System.EventHandler(this.HomeSwitcher);
            // 
            // Dspbordenu1
            // 
            this.Dspbordenu1.BackColor = System.Drawing.Color.Chartreuse;
            this.Dspbordenu1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.一键擦除ToolStripMenuItem});
            this.Dspbordenu1.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Dspbordenu1.Name = "Dspbordenu1";
            this.Dspbordenu1.Size = new System.Drawing.Size(128, 20);
            this.Dspbordenu1.Tag = "3";
            this.Dspbordenu1.Text = "主ARM升级(Eth)";
            this.Dspbordenu1.Click += new System.EventHandler(this.HomeSwitcher);
            // 
            // 一键擦除ToolStripMenuItem
            // 
            this.一键擦除ToolStripMenuItem.Name = "一键擦除ToolStripMenuItem";
            this.一键擦除ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.一键擦除ToolStripMenuItem.Text = "一键擦除";
            this.一键擦除ToolStripMenuItem.Click += new System.EventHandler(this.一键擦除ToolStripMenuItem_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ModelBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.pathtextBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.ipaddres);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.DownloadRate);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btnlink);
            this.groupBox2.Controls.Add(this.btnlabview);
            this.groupBox2.Location = new System.Drawing.Point(9, 157);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(385, 138);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.DragDrop += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragDrop);
            this.groupBox2.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragEnter);
            // 
            // ModelBox
            // 
            this.ModelBox.Enabled = false;
            this.ModelBox.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ModelBox.FormattingEnabled = true;
            this.ModelBox.Items.AddRange(new object[] {
            "透传模式",
            "单板模式"});
            this.ModelBox.Location = new System.Drawing.Point(209, 18);
            this.ModelBox.Margin = new System.Windows.Forms.Padding(2);
            this.ModelBox.Name = "ModelBox";
            this.ModelBox.Size = new System.Drawing.Size(92, 20);
            this.ModelBox.TabIndex = 41;
            this.ModelBox.Text = "透传模式";
            this.ModelBox.TextChanged += new System.EventHandler(this.ModelBox_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(144, 22);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 12);
            this.label3.TabIndex = 40;
            this.label3.Text = "刷机模式：";
            // 
            // pathtextBox
            // 
            this.pathtextBox.AllowDrop = true;
            this.pathtextBox.ForeColor = System.Drawing.Color.Blue;
            this.pathtextBox.Location = new System.Drawing.Point(41, 62);
            this.pathtextBox.Margin = new System.Windows.Forms.Padding(2);
            this.pathtextBox.Name = "pathtextBox";
            this.pathtextBox.Size = new System.Drawing.Size(260, 21);
            this.pathtextBox.TabIndex = 39;
            this.pathtextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragDrop);
            this.pathtextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragEnter);
            this.pathtextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathtextBox_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(4, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 12);
            this.label2.TabIndex = 38;
            this.label2.Text = "文件：";
            // 
            // ipaddres
            // 
            this.ipaddres.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ipaddres.Location = new System.Drawing.Point(41, 19);
            this.ipaddres.Margin = new System.Windows.Forms.Padding(2);
            this.ipaddres.Name = "ipaddres";
            this.ipaddres.Size = new System.Drawing.Size(96, 21);
            this.ipaddres.TabIndex = 37;
            this.ipaddres.Text = "192.168.1.15";
            this.ipaddres.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ipaddres_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(4, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 36;
            this.label1.Text = "地址：";
            // 
            // DownloadRate
            // 
            this.DownloadRate.Location = new System.Drawing.Point(41, 102);
            this.DownloadRate.Margin = new System.Windows.Forms.Padding(2);
            this.DownloadRate.MarqueeAnimationSpeed = 1;
            this.DownloadRate.Maximum = 1000;
            this.DownloadRate.Name = "DownloadRate";
            this.DownloadRate.Size = new System.Drawing.Size(322, 18);
            this.DownloadRate.Step = 1;
            this.DownloadRate.TabIndex = 34;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(4, 102);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 12);
            this.label5.TabIndex = 33;
            this.label5.Text = "进度：";
            // 
            // btnlink
            // 
            this.btnlink.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnlink.Location = new System.Drawing.Point(322, 12);
            this.btnlink.Margin = new System.Windows.Forms.Padding(2);
            this.btnlink.Name = "btnlink";
            this.btnlink.Size = new System.Drawing.Size(59, 30);
            this.btnlink.TabIndex = 7;
            this.btnlink.Tag = "1";
            this.btnlink.Text = "下载";
            this.btnlink.UseVisualStyleBackColor = true;
            this.btnlink.Click += new System.EventHandler(this.btnclick);
            // 
            // btnlabview
            // 
            this.btnlabview.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnlabview.Location = new System.Drawing.Point(322, 54);
            this.btnlabview.Margin = new System.Windows.Forms.Padding(2);
            this.btnlabview.Name = "btnlabview";
            this.btnlabview.Size = new System.Drawing.Size(59, 32);
            this.btnlabview.TabIndex = 6;
            this.btnlabview.Tag = "0";
            this.btnlabview.Text = "浏览";
            this.btnlabview.UseVisualStyleBackColor = true;
            this.btnlabview.Click += new System.EventHandler(this.btnclick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statelable,
            this.pointlable,
            this.toolStripStatusLabel4,
            this.versionlable,
            this.toolStripStatusLabel5});
            this.statusStrip1.Location = new System.Drawing.Point(0, 416);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(404, 22);
            this.statusStrip1.TabIndex = 31;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statelable
            // 
            this.statelable.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statelable.Name = "statelable";
            this.statelable.Size = new System.Drawing.Size(71, 17);
            this.statelable.Text = "主板ARM升级";
            // 
            // pointlable
            // 
            this.pointlable.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pointlable.Name = "pointlable";
            this.pointlable.Size = new System.Drawing.Size(237, 17);
            this.pointlable.Spring = true;
            this.pointlable.Text = "点击开始循环测试";
            this.pointlable.Click += new System.EventHandler(this.pointlable_Click);
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.IsLink = true;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(23, 17);
            this.toolStripStatusLabel4.Text = "(S)";
            this.toolStripStatusLabel4.Click += new System.EventHandler(this.toolStripStatusLabe_Click);
            // 
            // versionlable
            // 
            this.versionlable.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.versionlable.Name = "versionlable";
            this.versionlable.Size = new System.Drawing.Size(47, 17);
            this.versionlable.Text = "Version";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(15, 17);
            this.toolStripStatusLabel5.Text = "0";
            // 
            // StateControl_timer
            // 
            this.StateControl_timer.Interval = 1500;
            this.StateControl_timer.Tick += new System.EventHandler(this.StateControl_timer_Tick);
            // 
            // AckTimerOut_timer
            // 
            this.AckTimerOut_timer.Interval = 800;
            this.AckTimerOut_timer.Tick += new System.EventHandler(this.AckTimerOut_timer_Tick);
            // 
            // SocketConnectTimer
            // 
            this.SocketConnectTimer.Interval = 1000;
            this.SocketConnectTimer.Tick += new System.EventHandler(this.SocketConnectTimer_Tick);
            // 
            // DisplayTimer
            // 
            this.DisplayTimer.Tick += new System.EventHandler(this.DisplayTimer_Tick);
            // 
            // WaiteOneSeconTimer
            // 
            this.WaiteOneSeconTimer.Interval = 500;
            this.WaiteOneSeconTimer.Tick += new System.EventHandler(this.WaiteOneSeconTimer_Tick);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(168, 386);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(49, 21);
            this.textBox2.TabIndex = 33;
            this.textBox2.Text = "0";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(284, 386);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(49, 21);
            this.textBox3.TabIndex = 34;
            this.textBox3.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(130, 392);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 42;
            this.label4.Text = "成功：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(244, 392);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 12);
            this.label6.TabIndex = 43;
            this.label6.Text = "总数：";
            // 
            // databox
            // 
            this.databox.ContextMenuStrip = this.contextMenuStrip1;
            this.databox.Font = new System.Drawing.Font("楷体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.databox.HideSelection = false;
            this.databox.Location = new System.Drawing.Point(9, 27);
            this.databox.Name = "databox";
            this.databox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.databox.Size = new System.Drawing.Size(385, 125);
            this.databox.TabIndex = 44;
            this.databox.Text = "支持*.hex文件和合并*.bin文件刷机";
            // 
            // textBox1
            // 
            this.textBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.textBox1.Font = new System.Drawing.Font("楷体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.HideSelection = false;
            this.textBox1.Location = new System.Drawing.Point(9, 301);
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(385, 79);
            this.textBox1.TabIndex = 45;
            this.textBox1.Text = "";
            // 
            // Form7
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 438);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.databox);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form7";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Windey 75A 以太网升级器 V1.2";
            this.Activated += new System.EventHandler(this.Form7_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form7_FormClosing);
            this.Load += new System.EventHandler(this.Form7_Load);
            this.Move += new System.EventHandler(this.Form7_Move);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Mainbordenu;
        private System.Windows.Forms.ToolStripMenuItem Chargebordenu;
        private System.Windows.Forms.ToolStripMenuItem Dspbordenu1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ProgressBar DownloadRate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnlink;
        private System.Windows.Forms.Button btnlabview;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statelable;
        private System.Windows.Forms.ToolStripStatusLabel pointlable;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel versionlable;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.TextBox pathtextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ipaddres;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ModelBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer StateControl_timer;
        private System.Windows.Forms.Timer AckTimerOut_timer;
        private System.Windows.Forms.Timer TransCodeTimerout;
        private System.Windows.Forms.Timer SocketConnectTimer;
        private System.Windows.Forms.Timer DisplayTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 清除数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出文本ToolStripMenuItem;
        private System.Windows.Forms.Timer WaiteOneSeconTimer;
        private System.Windows.Forms.ToolStripMenuItem Dspbordenu;
        private System.Windows.Forms.ToolStripMenuItem 一键擦除ToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox databox;
        private System.Windows.Forms.RichTextBox textBox1;
    }
}

