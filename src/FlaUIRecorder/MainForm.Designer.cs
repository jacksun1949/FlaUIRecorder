namespace FlaUIRecorder
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.rdbVersionUIA2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.uiaTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.rdbVersionUIA3 = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboCodeProvider = new System.Windows.Forms.ComboBox();
            this.recorderProjectBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecentProjects = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.targetAppTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pathRowTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.rdbApplicationStart = new System.Windows.Forms.RadioButton();
            this.txtApplicationPath = new System.Windows.Forms.TextBox();
            this.btnApplicationBrowse = new System.Windows.Forms.Button();
            this.argsRowTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblApplicationArgs = new System.Windows.Forms.Label();
            this.txtApplicationArgs = new System.Windows.Forms.TextBox();
            this.processRowTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.rdbApplicationProcess = new System.Windows.Forms.RadioButton();
            this.cboApplicationProcess = new System.Windows.Forms.ComboBox();
            this.btnProcessRefresh = new System.Windows.Forms.Button();
            this.openApplicationDialog = new System.Windows.Forms.OpenFileDialog();
            this.openProjectDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveProjectDialog = new System.Windows.Forms.SaveFileDialog();
            this.lstSessions = new System.Windows.Forms.ListBox();
            this.sessionsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.leftTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topSettingsTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.startPanel = new System.Windows.Forms.Panel();
            this.rightTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabelReady = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelErrors = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.uiaTableLayout.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recorderProjectBindingSource)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.targetAppTableLayout.SuspendLayout();
            this.pathRowTableLayout.SuspendLayout();
            this.argsRowTableLayout.SuspendLayout();
            this.processRowTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sessionsBindingSource)).BeginInit();
            this.mainTableLayout.SuspendLayout();
            this.leftTableLayout.SuspendLayout();
            this.topSettingsTableLayout.SuspendLayout();
            this.startPanel.SuspendLayout();
            this.rightTableLayout.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdbVersionUIA2
            // 
            this.rdbVersionUIA2.AutoSize = true;
            this.rdbVersionUIA2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdbVersionUIA2.Location = new System.Drawing.Point(3, 3);
            this.rdbVersionUIA2.Name = "rdbVersionUIA2";
            this.rdbVersionUIA2.Size = new System.Drawing.Size(246, 36);
            this.rdbVersionUIA2.TabIndex = 0;
            this.rdbVersionUIA2.Text = "UIA2 Managed";
            this.rdbVersionUIA2.UseVisualStyleBackColor = true;
            this.rdbVersionUIA2.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged_UpdateDirty);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.uiaTableLayout);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.groupBox1.MinimumSize = new System.Drawing.Size(180, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox1.Size = new System.Drawing.Size(268, 154);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "UIA Version";
            // 
            // uiaTableLayout
            // 
            this.uiaTableLayout.ColumnCount = 1;
            this.uiaTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiaTableLayout.Controls.Add(this.rdbVersionUIA2, 0, 0);
            this.uiaTableLayout.Controls.Add(this.rdbVersionUIA3, 0, 1);
            this.uiaTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiaTableLayout.Location = new System.Drawing.Point(8, 40);
            this.uiaTableLayout.Name = "uiaTableLayout";
            this.uiaTableLayout.RowCount = 2;
            this.uiaTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.uiaTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.uiaTableLayout.Size = new System.Drawing.Size(252, 106);
            this.uiaTableLayout.TabIndex = 0;
            // 
            // rdbVersionUIA3
            // 
            this.rdbVersionUIA3.AutoSize = true;
            this.rdbVersionUIA3.Checked = true;
            this.rdbVersionUIA3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdbVersionUIA3.Location = new System.Drawing.Point(3, 45);
            this.rdbVersionUIA3.Name = "rdbVersionUIA3";
            this.rdbVersionUIA3.Size = new System.Drawing.Size(246, 58);
            this.rdbVersionUIA3.TabIndex = 1;
            this.rdbVersionUIA3.TabStop = true;
            this.rdbVersionUIA3.Text = "UIA3 Interop";
            this.rdbVersionUIA3.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.AutoSize = true;
            this.btnStart.Location = new System.Drawing.Point(3, 8);
            this.btnStart.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(181, 42);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start recording";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.cboCodeProvider);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(280, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.MinimumSize = new System.Drawing.Size(200, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox2.Size = new System.Drawing.Size(506, 154);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Code provider";
            // 
            // cboCodeProvider
            // 
            this.cboCodeProvider.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.recorderProjectBindingSource, "CodeProvider", true));
            this.cboCodeProvider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboCodeProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCodeProvider.FormattingEnabled = true;
            this.cboCodeProvider.Location = new System.Drawing.Point(8, 40);
            this.cboCodeProvider.Name = "cboCodeProvider";
            this.cboCodeProvider.Size = new System.Drawing.Size(490, 40);
            this.cboCodeProvider.TabIndex = 0;
            // 
            // recorderProjectBindingSource
            // 
            this.recorderProjectBindingSource.DataSource = typeof(FlaUIRecorder.RecorderProject);
            this.recorderProjectBindingSource.BindingComplete += new System.Windows.Forms.BindingCompleteEventHandler(this.recorderProjectBindingSource_BindingComplete);
            this.recorderProjectBindingSource.CurrentItemChanged += new System.EventHandler(this.recorderProjectBindingSource_CurrentItemChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(8, 8);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1346, 39);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpen,
            this.mnuSave,
            this.mnuSaveAs,
            this.mnuRecentProjects,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(73, 35);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // mnuOpen
            // 
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuOpen.Size = new System.Drawing.Size(390, 44);
            this.mnuOpen.Text = "Open project";
            this.mnuOpen.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuSave.Size = new System.Drawing.Size(390, 44);
            this.mnuSave.Text = "Save project";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuSaveAs
            // 
            this.mnuSaveAs.Name = "mnuSaveAs";
            this.mnuSaveAs.Size = new System.Drawing.Size(390, 44);
            this.mnuSaveAs.Text = "Save project as...";
            this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
            // 
            // mnuRecentProjects
            // 
            this.mnuRecentProjects.Name = "mnuRecentProjects";
            this.mnuRecentProjects.Size = new System.Drawing.Size(390, 44);
            this.mnuRecentProjects.Text = "Recent projects";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(387, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(390, 44);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox3.Controls.Add(this.targetAppTableLayout);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 169);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.groupBox3.MinimumSize = new System.Drawing.Size(0, 120);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox3.Size = new System.Drawing.Size(786, 263);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Target application";
            // 
            // targetAppTableLayout
            // 
            this.targetAppTableLayout.ColumnCount = 1;
            this.targetAppTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.targetAppTableLayout.Controls.Add(this.pathRowTableLayout, 0, 0);
            this.targetAppTableLayout.Controls.Add(this.argsRowTableLayout, 0, 1);
            this.targetAppTableLayout.Controls.Add(this.processRowTableLayout, 0, 2);
            this.targetAppTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetAppTableLayout.Location = new System.Drawing.Point(8, 40);
            this.targetAppTableLayout.Name = "targetAppTableLayout";
            this.targetAppTableLayout.RowCount = 3;
            this.targetAppTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.targetAppTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.targetAppTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.targetAppTableLayout.Size = new System.Drawing.Size(770, 215);
            this.targetAppTableLayout.TabIndex = 0;
            // 
            // pathRowTableLayout
            // 
            this.pathRowTableLayout.ColumnCount = 3;
            this.pathRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.pathRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pathRowTableLayout.Controls.Add(this.rdbApplicationStart, 0, 0);
            this.pathRowTableLayout.Controls.Add(this.txtApplicationPath, 1, 0);
            this.pathRowTableLayout.Controls.Add(this.btnApplicationBrowse, 2, 0);
            this.pathRowTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathRowTableLayout.Location = new System.Drawing.Point(3, 3);
            this.pathRowTableLayout.Name = "pathRowTableLayout";
            this.pathRowTableLayout.RowCount = 1;
            this.pathRowTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pathRowTableLayout.Size = new System.Drawing.Size(764, 54);
            this.pathRowTableLayout.TabIndex = 0;
            // 
            // rdbApplicationStart
            // 
            this.rdbApplicationStart.AutoSize = true;
            this.rdbApplicationStart.Checked = true;
            this.rdbApplicationStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.rdbApplicationStart.Location = new System.Drawing.Point(3, 3);
            this.rdbApplicationStart.Name = "rdbApplicationStart";
            this.rdbApplicationStart.Size = new System.Drawing.Size(154, 36);
            this.rdbApplicationStart.TabIndex = 2;
            this.rdbApplicationStart.TabStop = true;
            this.rdbApplicationStart.Text = "Path:";
            this.rdbApplicationStart.UseVisualStyleBackColor = true;
            this.rdbApplicationStart.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged_UpdateDirty);
            // 
            // txtApplicationPath
            // 
            this.txtApplicationPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.recorderProjectBindingSource, "Executable", true));
            this.txtApplicationPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtApplicationPath.Location = new System.Drawing.Point(163, 3);
            this.txtApplicationPath.Name = "txtApplicationPath";
            this.txtApplicationPath.Size = new System.Drawing.Size(553, 39);
            this.txtApplicationPath.TabIndex = 4;
            this.txtApplicationPath.TextChanged += new System.EventHandler(this.radioButton_CheckedChanged_UpdateDirty);
            // 
            // btnApplicationBrowse
            // 
            this.btnApplicationBrowse.AutoSize = true;
            this.btnApplicationBrowse.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnApplicationBrowse.Location = new System.Drawing.Point(722, 3);
            this.btnApplicationBrowse.Name = "btnApplicationBrowse";
            this.btnApplicationBrowse.Size = new System.Drawing.Size(39, 42);
            this.btnApplicationBrowse.TabIndex = 7;
            this.btnApplicationBrowse.Text = "...";
            this.btnApplicationBrowse.UseVisualStyleBackColor = true;
            this.btnApplicationBrowse.Click += new System.EventHandler(this.btnApplicationBrowse_Click);
            // 
            // argsRowTableLayout
            // 
            this.argsRowTableLayout.ColumnCount = 2;
            this.argsRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.argsRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.argsRowTableLayout.Controls.Add(this.lblApplicationArgs, 0, 0);
            this.argsRowTableLayout.Controls.Add(this.txtApplicationArgs, 1, 0);
            this.argsRowTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.argsRowTableLayout.Location = new System.Drawing.Point(3, 63);
            this.argsRowTableLayout.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.argsRowTableLayout.Name = "argsRowTableLayout";
            this.argsRowTableLayout.RowCount = 1;
            this.argsRowTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.argsRowTableLayout.Size = new System.Drawing.Size(764, 51);
            this.argsRowTableLayout.TabIndex = 1;
            // 
            // lblApplicationArgs
            // 
            this.lblApplicationArgs.AutoSize = true;
            this.lblApplicationArgs.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblApplicationArgs.Location = new System.Drawing.Point(3, 3);
            this.lblApplicationArgs.Margin = new System.Windows.Forms.Padding(3);
            this.lblApplicationArgs.Name = "lblApplicationArgs";
            this.lblApplicationArgs.Size = new System.Drawing.Size(154, 32);
            this.lblApplicationArgs.TabIndex = 8;
            this.lblApplicationArgs.Text = "Args:";
            this.lblApplicationArgs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtApplicationArgs
            // 
            this.txtApplicationArgs.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.recorderProjectBindingSource, "Arguments", true));
            this.txtApplicationArgs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtApplicationArgs.Location = new System.Drawing.Point(163, 3);
            this.txtApplicationArgs.Name = "txtApplicationArgs";
            this.txtApplicationArgs.Size = new System.Drawing.Size(598, 39);
            this.txtApplicationArgs.TabIndex = 5;
            // 
            // processRowTableLayout
            // 
            this.processRowTableLayout.ColumnCount = 3;
            this.processRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.processRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.processRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.processRowTableLayout.Controls.Add(this.rdbApplicationProcess, 0, 0);
            this.processRowTableLayout.Controls.Add(this.cboApplicationProcess, 1, 0);
            this.processRowTableLayout.Controls.Add(this.btnProcessRefresh, 2, 0);
            this.processRowTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processRowTableLayout.Location = new System.Drawing.Point(3, 123);
            this.processRowTableLayout.Name = "processRowTableLayout";
            this.processRowTableLayout.RowCount = 1;
            this.processRowTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.processRowTableLayout.Size = new System.Drawing.Size(764, 89);
            this.processRowTableLayout.TabIndex = 2;
            // 
            // rdbApplicationProcess
            // 
            this.rdbApplicationProcess.AutoSize = true;
            this.rdbApplicationProcess.Dock = System.Windows.Forms.DockStyle.Top;
            this.rdbApplicationProcess.Location = new System.Drawing.Point(3, 3);
            this.rdbApplicationProcess.Name = "rdbApplicationProcess";
            this.rdbApplicationProcess.Size = new System.Drawing.Size(154, 36);
            this.rdbApplicationProcess.TabIndex = 3;
            this.rdbApplicationProcess.Text = "Process:";
            this.rdbApplicationProcess.UseVisualStyleBackColor = true;
            // 
            // cboApplicationProcess
            // 
            this.cboApplicationProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboApplicationProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboApplicationProcess.FormattingEnabled = true;
            this.cboApplicationProcess.Location = new System.Drawing.Point(163, 3);
            this.cboApplicationProcess.Name = "cboApplicationProcess";
            this.cboApplicationProcess.Size = new System.Drawing.Size(558, 40);
            this.cboApplicationProcess.TabIndex = 6;
            this.cboApplicationProcess.SelectedIndexChanged += new System.EventHandler(this.radioButton_CheckedChanged_UpdateDirty);
            this.cboApplicationProcess.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cboApplicationProcess_Format);
            // 
            // btnProcessRefresh
            // 
            this.btnProcessRefresh.AutoSize = true;
            this.btnProcessRefresh.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnProcessRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnProcessRefresh.Image")));
            this.btnProcessRefresh.Location = new System.Drawing.Point(727, 3);
            this.btnProcessRefresh.Name = "btnProcessRefresh";
            this.btnProcessRefresh.Size = new System.Drawing.Size(34, 40);
            this.btnProcessRefresh.TabIndex = 7;
            this.btnProcessRefresh.UseVisualStyleBackColor = true;
            this.btnProcessRefresh.Click += new System.EventHandler(this.btnProcessRefresh_Click);
            // 
            // openApplicationDialog
            // 
            this.openApplicationDialog.FileName = "*.exe";
            this.openApplicationDialog.Filter = "Applications (*.exe)|*.exe";
            this.openApplicationDialog.Title = "Choose target application";
            // 
            // openProjectDialog
            // 
            this.openProjectDialog.FileName = "*.urp";
            this.openProjectDialog.Filter = "FlaUI Recorder projects (*.urp)|*.urp";
            this.openProjectDialog.Title = "Open an existing project";
            // 
            // saveProjectDialog
            // 
            this.saveProjectDialog.DefaultExt = "*.urp";
            this.saveProjectDialog.Filter = "FlaUI Recorder projects (*.urp)|*.urp";
            this.saveProjectDialog.Title = "Save recorder project";
            // 
            // lstSessions
            // 
            this.lstSessions.DataSource = this.sessionsBindingSource;
            this.lstSessions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSessions.FormattingEnabled = true;
            this.lstSessions.IntegralHeight = false;
            this.lstSessions.ItemHeight = 32;
            this.lstSessions.Location = new System.Drawing.Point(3, 38);
            this.lstSessions.Name = "lstSessions";
            this.lstSessions.Size = new System.Drawing.Size(520, 453);
            this.lstSessions.TabIndex = 6;
            this.lstSessions.DoubleClick += new System.EventHandler(this.lstSessions_DoubleClick);
            // 
            // sessionsBindingSource
            // 
            this.sessionsBindingSource.DataMember = "Sessions";
            this.sessionsBindingSource.DataSource = this.recorderProjectBindingSource;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(520, 32);
            this.label1.TabIndex = 7;
            this.label1.Text = "Recorded sessions";
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.ColumnCount = 2;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.mainTableLayout.Controls.Add(this.leftTableLayout, 0, 0);
            this.mainTableLayout.Controls.Add(this.rightTableLayout, 1, 0);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(8, 47);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.Padding = new System.Windows.Forms.Padding(8);
            this.mainTableLayout.RowCount = 1;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Size = new System.Drawing.Size(1346, 516);
            this.mainTableLayout.TabIndex = 8;
            // 
            // leftTableLayout
            // 
            this.leftTableLayout.BackColor = System.Drawing.Color.Transparent;
            this.leftTableLayout.ColumnCount = 1;
            this.leftTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftTableLayout.Controls.Add(this.topSettingsTableLayout, 0, 0);
            this.leftTableLayout.Controls.Add(this.groupBox3, 0, 1);
            this.leftTableLayout.Controls.Add(this.startPanel, 0, 2);
            this.leftTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftTableLayout.Location = new System.Drawing.Point(11, 11);
            this.leftTableLayout.Name = "leftTableLayout";
            this.leftTableLayout.RowCount = 3;
            this.leftTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.leftTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftTableLayout.Size = new System.Drawing.Size(792, 494);
            this.leftTableLayout.TabIndex = 0;
            // 
            // topSettingsTableLayout
            // 
            this.topSettingsTableLayout.ColumnCount = 2;
            this.topSettingsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.topSettingsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topSettingsTableLayout.Controls.Add(this.groupBox1, 0, 0);
            this.topSettingsTableLayout.Controls.Add(this.groupBox2, 1, 0);
            this.topSettingsTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topSettingsTableLayout.Location = new System.Drawing.Point(3, 3);
            this.topSettingsTableLayout.Name = "topSettingsTableLayout";
            this.topSettingsTableLayout.RowCount = 1;
            this.topSettingsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.topSettingsTableLayout.Size = new System.Drawing.Size(786, 154);
            this.topSettingsTableLayout.TabIndex = 0;
            // 
            // startPanel
            // 
            this.startPanel.AutoSize = true;
            this.startPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.startPanel.Controls.Add(this.btnStart);
            this.startPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startPanel.Location = new System.Drawing.Point(3, 438);
            this.startPanel.Name = "startPanel";
            this.startPanel.Size = new System.Drawing.Size(786, 53);
            this.startPanel.TabIndex = 1;
            // 
            // rightTableLayout
            // 
            this.rightTableLayout.ColumnCount = 1;
            this.rightTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightTableLayout.Controls.Add(this.label1, 0, 0);
            this.rightTableLayout.Controls.Add(this.lstSessions, 0, 1);
            this.rightTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightTableLayout.Location = new System.Drawing.Point(809, 11);
            this.rightTableLayout.Name = "rightTableLayout";
            this.rightTableLayout.RowCount = 2;
            this.rightTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightTableLayout.Size = new System.Drawing.Size(526, 494);
            this.rightTableLayout.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelReady,
            this.statusLabelErrors});
            this.statusStrip1.Location = new System.Drawing.Point(8, 563);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1346, 41);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelReady
            // 
            this.statusLabelReady.Name = "statusLabelReady";
            this.statusLabelReady.Size = new System.Drawing.Size(1224, 31);
            this.statusLabelReady.Spring = true;
            this.statusLabelReady.Text = "Ready";
            this.statusLabelReady.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelErrors
            // 
            this.statusLabelErrors.IsLink = true;
            this.statusLabelErrors.Name = "statusLabelErrors";
            this.statusLabelErrors.Size = new System.Drawing.Size(107, 31);
            this.statusLabelErrors.Text = "Errors: 0";
            this.statusLabelErrors.Click += new System.EventHandler(this.ErrorStatusLabel_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 612);
            this.Controls.Add(this.mainTableLayout);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(900, 520);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FlaUI - Recorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.uiaTableLayout.ResumeLayout(false);
            this.uiaTableLayout.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recorderProjectBindingSource)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.targetAppTableLayout.ResumeLayout(false);
            this.pathRowTableLayout.ResumeLayout(false);
            this.pathRowTableLayout.PerformLayout();
            this.argsRowTableLayout.ResumeLayout(false);
            this.argsRowTableLayout.PerformLayout();
            this.processRowTableLayout.ResumeLayout(false);
            this.processRowTableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sessionsBindingSource)).EndInit();
            this.mainTableLayout.ResumeLayout(false);
            this.leftTableLayout.ResumeLayout(false);
            this.leftTableLayout.PerformLayout();
            this.topSettingsTableLayout.ResumeLayout(false);
            this.startPanel.ResumeLayout(false);
            this.startPanel.PerformLayout();
            this.rightTableLayout.ResumeLayout(false);
            this.rightTableLayout.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbVersionUIA2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbVersionUIA3;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboCodeProvider;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnProcessRefresh;
        private System.Windows.Forms.ComboBox cboApplicationProcess;
        private System.Windows.Forms.TextBox txtApplicationPath;
        private System.Windows.Forms.RadioButton rdbApplicationProcess;
        private System.Windows.Forms.RadioButton rdbApplicationStart;
        private System.Windows.Forms.Button btnApplicationBrowse;
        private System.Windows.Forms.OpenFileDialog openApplicationDialog;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveAs;
        private System.Windows.Forms.OpenFileDialog openProjectDialog;
        private System.Windows.Forms.SaveFileDialog saveProjectDialog;
        private System.Windows.Forms.ListBox lstSessions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource recorderProjectBindingSource;
        private System.Windows.Forms.BindingSource sessionsBindingSource;
        private System.Windows.Forms.ToolStripMenuItem mnuRecentProjects;
        private System.Windows.Forms.Label lblApplicationArgs;
        private System.Windows.Forms.TextBox txtApplicationArgs;
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.TableLayoutPanel leftTableLayout;
        private System.Windows.Forms.TableLayoutPanel topSettingsTableLayout;
        private System.Windows.Forms.TableLayoutPanel rightTableLayout;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelReady;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelErrors;
        private System.Windows.Forms.TableLayoutPanel uiaTableLayout;
        private System.Windows.Forms.TableLayoutPanel targetAppTableLayout;
        private System.Windows.Forms.TableLayoutPanel pathRowTableLayout;
        private System.Windows.Forms.TableLayoutPanel argsRowTableLayout;
        private System.Windows.Forms.TableLayoutPanel processRowTableLayout;
        private System.Windows.Forms.Panel startPanel;
    }
}
