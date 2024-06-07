
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.ModelBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pathtextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ipaddres = new System.Windows.Forms.TextBox();
            this.DownloadRate = new System.Windows.Forms.ProgressBar();
            this.btnlabview = new System.Windows.Forms.Button();
            this.btnlink = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.DSP刷机 = new System.Windows.Forms.ToolStripMenuItem();
            this.以太网测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.主控板刷机 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
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
            this.导出文本ToolStripMenuItem.Text = "导出文件";
            this.导出文本ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(2, 192);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(607, 116);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.DragDrop += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragDrop);
            this.groupBox2.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragEnter);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.ModelBox, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.pathtextBox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ipaddres, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.DownloadRate, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnlabview, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnlink, 4, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(603, 98);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(258, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 32);
            this.label3.TabIndex = 40;
            this.label3.Text = "刷机模式：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ModelBox
            // 
            this.ModelBox.Enabled = false;
            this.ModelBox.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ModelBox.FormattingEnabled = true;
            this.ModelBox.Items.AddRange(new object[] {
            "透传模式",
            "单板模式"});
            this.ModelBox.Location = new System.Drawing.Point(332, 2);
            this.ModelBox.Margin = new System.Windows.Forms.Padding(2);
            this.ModelBox.Name = "ModelBox";
            this.ModelBox.Size = new System.Drawing.Size(92, 20);
            this.ModelBox.TabIndex = 41;
            this.ModelBox.Text = "透传模式";
            this.ModelBox.TextChanged += new System.EventHandler(this.ModelBox_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(2, 64);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 34);
            this.label5.TabIndex = 33;
            this.label5.Text = "进度：";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(2, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 32);
            this.label2.TabIndex = 38;
            this.label2.Text = "文件：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pathtextBox
            // 
            this.pathtextBox.AllowDrop = true;
            this.tableLayoutPanel2.SetColumnSpan(this.pathtextBox, 3);
            this.pathtextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathtextBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pathtextBox.Location = new System.Drawing.Point(51, 38);
            this.pathtextBox.Margin = new System.Windows.Forms.Padding(3, 6, 3, 2);
            this.pathtextBox.Name = "pathtextBox";
            this.pathtextBox.Size = new System.Drawing.Size(484, 21);
            this.pathtextBox.TabIndex = 39;
            this.pathtextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragDrop);
            this.pathtextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathtextBox_DragEnter);
            this.pathtextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathtextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 32);
            this.label1.TabIndex = 36;
            this.label1.Text = "地址：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ipaddres
            // 
            this.ipaddres.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ipaddres.Location = new System.Drawing.Point(50, 2);
            this.ipaddres.Margin = new System.Windows.Forms.Padding(2);
            this.ipaddres.Name = "ipaddres";
            this.ipaddres.Size = new System.Drawing.Size(96, 21);
            this.ipaddres.TabIndex = 37;
            this.ipaddres.Text = "192.168.1.15";
            this.ipaddres.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ipaddres_KeyDown);
            // 
            // DownloadRate
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.DownloadRate, 3);
            this.DownloadRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DownloadRate.Location = new System.Drawing.Point(51, 69);
            this.DownloadRate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.DownloadRate.MarqueeAnimationSpeed = 1;
            this.DownloadRate.Maximum = 1000;
            this.DownloadRate.Name = "DownloadRate";
            this.DownloadRate.Size = new System.Drawing.Size(484, 24);
            this.DownloadRate.Step = 1;
            this.DownloadRate.TabIndex = 34;
            // 
            // btnlabview
            // 
            this.btnlabview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnlabview.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnlabview.Location = new System.Drawing.Point(540, 34);
            this.btnlabview.Margin = new System.Windows.Forms.Padding(2);
            this.btnlabview.Name = "btnlabview";
            this.btnlabview.Size = new System.Drawing.Size(61, 28);
            this.btnlabview.TabIndex = 6;
            this.btnlabview.Tag = "0";
            this.btnlabview.Text = "浏览";
            this.btnlabview.UseVisualStyleBackColor = true;
            this.btnlabview.Click += new System.EventHandler(this.btnclick);
            // 
            // btnlink
            // 
            this.btnlink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnlink.Font = new System.Drawing.Font("楷体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnlink.Location = new System.Drawing.Point(540, 66);
            this.btnlink.Margin = new System.Windows.Forms.Padding(2);
            this.btnlink.Name = "btnlink";
            this.btnlink.Size = new System.Drawing.Size(61, 30);
            this.btnlink.TabIndex = 7;
            this.btnlink.Tag = "1";
            this.btnlink.Text = "下载";
            this.btnlink.UseVisualStyleBackColor = true;
            this.btnlink.Click += new System.EventHandler(this.btnclick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statelable,
            this.pointlable,
            this.toolStripStatusLabel4,
            this.versionlable,
            this.toolStripStatusLabel5});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(611, 30);
            this.statusStrip1.TabIndex = 31;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statelable
            // 
            this.statelable.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.statelable.Name = "statelable";
            this.statelable.Size = new System.Drawing.Size(52, 25);
            this.statelable.Text = "ARM升级";
            // 
            // pointlable
            // 
            this.pointlable.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pointlable.Name = "pointlable";
            this.pointlable.Size = new System.Drawing.Size(378, 25);
            this.pointlable.Spring = true;
            this.pointlable.Text = "点击开始循环测试";
            this.pointlable.Click += new System.EventHandler(this.pointlable_Click);
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.IsLink = true;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(23, 25);
            this.toolStripStatusLabel4.Text = "(S)";
            this.toolStripStatusLabel4.Click += new System.EventHandler(this.toolStripStatusLabe_Click);
            // 
            // versionlable
            // 
            this.versionlable.Font = new System.Drawing.Font("楷体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.versionlable.Name = "versionlable";
            this.versionlable.Size = new System.Drawing.Size(101, 25);
            this.versionlable.Text = "Version:2024-5-6";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(15, 25);
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
            this.textBox2.Location = new System.Drawing.Point(51, 70);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(49, 21);
            this.textBox2.TabIndex = 33;
            this.textBox2.Text = "0";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(350, 70);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(49, 21);
            this.textBox3.TabIndex = 34;
            this.textBox3.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(2, 67);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 25);
            this.label4.TabIndex = 42;
            this.label4.Text = "成功：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(301, 67);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 25);
            this.label6.TabIndex = 43;
            this.label6.Text = "总数：";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // databox
            // 
            this.databox.BackColor = System.Drawing.Color.White;
            this.databox.ContextMenuStrip = this.contextMenuStrip1;
            this.databox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databox.Font = new System.Drawing.Font("楷体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.databox.HideSelection = false;
            this.databox.Location = new System.Drawing.Point(3, 33);
            this.databox.Name = "databox";
            this.databox.ReadOnly = true;
            this.databox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.databox.Size = new System.Drawing.Size(605, 154);
            this.databox.TabIndex = 44;
            this.databox.Text = "";
            // 
            // textBox1
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.textBox1, 4);
            this.textBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("楷体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.HideSelection = false;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(593, 61);
            this.textBox1.TabIndex = 45;
            this.textBox1.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.databox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(611, 458);
            this.tableLayoutPanel1.TabIndex = 46;
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
            this.menuStrip1.Size = new System.Drawing.Size(611, 30);
            this.menuStrip1.TabIndex = 47;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.Menustrip_Change);
            // 
            // DSP刷机
            // 
            this.DSP刷机.BackColor = System.Drawing.SystemColors.ControlLight;
            this.DSP刷机.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.以太网测试ToolStripMenuItem});
            this.DSP刷机.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
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
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuItem1.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(118, 26);
            this.toolStripMenuItem1.Tag = "1";
            this.toolStripMenuItem1.Text = "充电器485升级";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuItem4.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(139, 26);
            this.toolStripMenuItem4.Tag = "2";
            this.toolStripMenuItem4.Text = "充电器以太网升级";
            // 
            // 主控板刷机
            // 
            this.主控板刷机.BackColor = System.Drawing.SystemColors.ControlLight;
            this.主控板刷机.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.主控板刷机.Name = "主控板刷机";
            this.主控板刷机.Size = new System.Drawing.Size(105, 26);
            this.主控板刷机.Tag = "3";
            this.主控板刷机.Text = "ARM 485升级";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.toolStripMenuItem2.Font = new System.Drawing.Font("楷体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(126, 26);
            this.toolStripMenuItem2.Tag = "4";
            this.toolStripMenuItem2.Text = "ARM 以太网升级";
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 313);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(605, 112);
            this.groupBox3.TabIndex = 46;
            this.groupBox3.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.textBox1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBox3, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBox2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label6, 2, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(599, 92);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // Form7
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 458);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form7";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "H6内部芯片升级_V1.3_Beta";
            this.Activated += new System.EventHandler(this.Form7_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form7_FormClosing);
            this.Load += new System.EventHandler(this.Form7_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ip_textBox_KeyUp);
            this.Move += new System.EventHandler(this.Form7_Move);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
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
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox databox;
        private System.Windows.Forms.RichTextBox textBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem DSP刷机;
        private System.Windows.Forms.ToolStripMenuItem 以太网测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem 主控板刷机;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
    }
}

