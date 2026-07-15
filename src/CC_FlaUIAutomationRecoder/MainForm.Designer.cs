namespace CC_FlaUIAutomationRecoder
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            rdbVersionUIA2 = new System.Windows.Forms.RadioButton();
            groupBox1 = new System.Windows.Forms.GroupBox();
            uiaTableLayout = new System.Windows.Forms.TableLayoutPanel();
            rdbVersionUIA3 = new System.Windows.Forms.RadioButton();
            btnStart = new System.Windows.Forms.Button();
            groupBox2 = new System.Windows.Forms.GroupBox();
            cboCodeProvider = new System.Windows.Forms.ComboBox();
            recorderProjectBindingSource = new System.Windows.Forms.BindingSource(components);
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            mnuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            mnuRecentProjects = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            groupBox3 = new System.Windows.Forms.GroupBox();
            targetAppTableLayout = new System.Windows.Forms.TableLayoutPanel();
            pathRowTableLayout = new System.Windows.Forms.TableLayoutPanel();
            rdbApplicationStart = new System.Windows.Forms.RadioButton();
            txtApplicationPath = new System.Windows.Forms.TextBox();
            btnApplicationBrowse = new System.Windows.Forms.Button();
            argsRowTableLayout = new System.Windows.Forms.TableLayoutPanel();
            lblApplicationArgs = new System.Windows.Forms.Label();
            txtApplicationArgs = new System.Windows.Forms.TextBox();
            processRowTableLayout = new System.Windows.Forms.TableLayoutPanel();
            rdbApplicationProcess = new System.Windows.Forms.RadioButton();
            cboApplicationProcess = new System.Windows.Forms.ComboBox();
            btnProcessRefresh = new System.Windows.Forms.Button();
            openApplicationDialog = new System.Windows.Forms.OpenFileDialog();
            openProjectDialog = new System.Windows.Forms.OpenFileDialog();
            saveProjectDialog = new System.Windows.Forms.SaveFileDialog();
            lstSessions = new System.Windows.Forms.ListBox();
            sessionsBindingSource = new System.Windows.Forms.BindingSource(components);
            label1 = new System.Windows.Forms.Label();
            mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            leftTableLayout = new System.Windows.Forms.TableLayoutPanel();
            topSettingsTableLayout = new System.Windows.Forms.TableLayoutPanel();
            startPanel = new System.Windows.Forms.Panel();
            rightTableLayout = new System.Windows.Forms.TableLayoutPanel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            statusLabelReady = new System.Windows.Forms.ToolStripStatusLabel();
            statusLabelErrors = new System.Windows.Forms.ToolStripStatusLabel();
            groupBox1.SuspendLayout();
            uiaTableLayout.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)recorderProjectBindingSource).BeginInit();
            menuStrip1.SuspendLayout();
            groupBox3.SuspendLayout();
            targetAppTableLayout.SuspendLayout();
            pathRowTableLayout.SuspendLayout();
            argsRowTableLayout.SuspendLayout();
            processRowTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sessionsBindingSource).BeginInit();
            mainTableLayout.SuspendLayout();
            leftTableLayout.SuspendLayout();
            topSettingsTableLayout.SuspendLayout();
            startPanel.SuspendLayout();
            rightTableLayout.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // rdbVersionUIA2
            // 
            rdbVersionUIA2.AutoSize = true;
            rdbVersionUIA2.Dock = System.Windows.Forms.DockStyle.Fill;
            rdbVersionUIA2.Location = new System.Drawing.Point(3, 3);
            rdbVersionUIA2.Name = "rdbVersionUIA2";
            rdbVersionUIA2.Size = new System.Drawing.Size(246, 36);
            rdbVersionUIA2.TabIndex = 0;
            rdbVersionUIA2.Text = "UIA2 Managed";
            rdbVersionUIA2.UseVisualStyleBackColor = true;
            rdbVersionUIA2.CheckedChanged += radioButton_CheckedChanged_UpdateDirty;
            // 
            // groupBox1
            // 
            groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(uiaTableLayout);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            groupBox1.MinimumSize = new System.Drawing.Size(180, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(8);
            groupBox1.Size = new System.Drawing.Size(268, 154);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "UIA Version";
            // 
            // uiaTableLayout
            // 
            uiaTableLayout.ColumnCount = 1;
            uiaTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiaTableLayout.Controls.Add(rdbVersionUIA2, 0, 0);
            uiaTableLayout.Controls.Add(rdbVersionUIA3, 0, 1);
            uiaTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            uiaTableLayout.Location = new System.Drawing.Point(8, 40);
            uiaTableLayout.Name = "uiaTableLayout";
            uiaTableLayout.RowCount = 2;
            uiaTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            uiaTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            uiaTableLayout.Size = new System.Drawing.Size(252, 106);
            uiaTableLayout.TabIndex = 0;
            // 
            // rdbVersionUIA3
            // 
            rdbVersionUIA3.AutoSize = true;
            rdbVersionUIA3.Checked = true;
            rdbVersionUIA3.Dock = System.Windows.Forms.DockStyle.Fill;
            rdbVersionUIA3.Location = new System.Drawing.Point(3, 45);
            rdbVersionUIA3.Name = "rdbVersionUIA3";
            rdbVersionUIA3.Size = new System.Drawing.Size(246, 58);
            rdbVersionUIA3.TabIndex = 1;
            rdbVersionUIA3.TabStop = true;
            rdbVersionUIA3.Text = "UIA3 Interop";
            rdbVersionUIA3.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            btnStart.AutoSize = true;
            btnStart.Location = new System.Drawing.Point(3, 8);
            btnStart.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(195, 42);
            btnStart.TabIndex = 2;
            btnStart.Text = "Start recording";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // groupBox2
            // 
            groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            groupBox2.Controls.Add(cboCodeProvider);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(280, 0);
            groupBox2.Margin = new System.Windows.Forms.Padding(0);
            groupBox2.MinimumSize = new System.Drawing.Size(200, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(8);
            groupBox2.Size = new System.Drawing.Size(506, 154);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Code provider";
            // 
            // cboCodeProvider
            // 
            cboCodeProvider.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", recorderProjectBindingSource, "CodeProvider", true));
            cboCodeProvider.Dock = System.Windows.Forms.DockStyle.Fill;
            cboCodeProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboCodeProvider.FormattingEnabled = true;
            cboCodeProvider.Location = new System.Drawing.Point(8, 40);
            cboCodeProvider.Name = "cboCodeProvider";
            cboCodeProvider.Size = new System.Drawing.Size(490, 40);
            cboCodeProvider.TabIndex = 0;
            // 
            // recorderProjectBindingSource
            // 
            recorderProjectBindingSource.DataSource = typeof(RecorderProject);
            recorderProjectBindingSource.BindingComplete += recorderProjectBindingSource_BindingComplete;
            recorderProjectBindingSource.CurrentItemChanged += recorderProjectBindingSource_CurrentItemChanged;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem1 });
            menuStrip1.Location = new System.Drawing.Point(8, 8);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(1346, 39);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuOpen, mnuSave, mnuSaveAs, mnuRecentProjects, toolStripSeparator1, exitToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(73, 35);
            toolStripMenuItem1.Text = "&File";
            // 
            // mnuOpen
            // 
            mnuOpen.Name = "mnuOpen";
            mnuOpen.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            mnuOpen.Size = new System.Drawing.Size(390, 44);
            mnuOpen.Text = "Open project";
            mnuOpen.Click += openProjectToolStripMenuItem_Click;
            // 
            // mnuSave
            // 
            mnuSave.Name = "mnuSave";
            mnuSave.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            mnuSave.Size = new System.Drawing.Size(390, 44);
            mnuSave.Text = "Save project";
            mnuSave.Click += mnuSave_Click;
            // 
            // mnuSaveAs
            // 
            mnuSaveAs.Name = "mnuSaveAs";
            mnuSaveAs.Size = new System.Drawing.Size(390, 44);
            mnuSaveAs.Text = "Save project as...";
            mnuSaveAs.Click += mnuSaveAs_Click;
            // 
            // mnuRecentProjects
            // 
            mnuRecentProjects.Name = "mnuRecentProjects";
            mnuRecentProjects.Size = new System.Drawing.Size(390, 44);
            mnuRecentProjects.Text = "Recent projects";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(387, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(390, 44);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // groupBox3
            // 
            groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            groupBox3.Controls.Add(targetAppTableLayout);
            groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox3.Location = new System.Drawing.Point(3, 169);
            groupBox3.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            groupBox3.MinimumSize = new System.Drawing.Size(0, 120);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(8);
            groupBox3.Size = new System.Drawing.Size(786, 263);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "Target application";
            // 
            // targetAppTableLayout
            // 
            targetAppTableLayout.ColumnCount = 1;
            targetAppTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            targetAppTableLayout.Controls.Add(pathRowTableLayout, 0, 0);
            targetAppTableLayout.Controls.Add(argsRowTableLayout, 0, 1);
            targetAppTableLayout.Controls.Add(processRowTableLayout, 0, 2);
            targetAppTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            targetAppTableLayout.Location = new System.Drawing.Point(8, 40);
            targetAppTableLayout.Name = "targetAppTableLayout";
            targetAppTableLayout.RowCount = 3;
            targetAppTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            targetAppTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            targetAppTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            targetAppTableLayout.Size = new System.Drawing.Size(770, 215);
            targetAppTableLayout.TabIndex = 0;
            // 
            // pathRowTableLayout
            // 
            pathRowTableLayout.ColumnCount = 3;
            pathRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            pathRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            pathRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            pathRowTableLayout.Controls.Add(rdbApplicationStart, 0, 0);
            pathRowTableLayout.Controls.Add(txtApplicationPath, 1, 0);
            pathRowTableLayout.Controls.Add(btnApplicationBrowse, 2, 0);
            pathRowTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            pathRowTableLayout.Location = new System.Drawing.Point(3, 3);
            pathRowTableLayout.Name = "pathRowTableLayout";
            pathRowTableLayout.RowCount = 1;
            pathRowTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            pathRowTableLayout.Size = new System.Drawing.Size(764, 54);
            pathRowTableLayout.TabIndex = 0;
            // 
            // rdbApplicationStart
            // 
            rdbApplicationStart.AutoSize = true;
            rdbApplicationStart.Checked = true;
            rdbApplicationStart.Dock = System.Windows.Forms.DockStyle.Top;
            rdbApplicationStart.Location = new System.Drawing.Point(3, 3);
            rdbApplicationStart.Name = "rdbApplicationStart";
            rdbApplicationStart.Size = new System.Drawing.Size(154, 36);
            rdbApplicationStart.TabIndex = 2;
            rdbApplicationStart.TabStop = true;
            rdbApplicationStart.Text = "Path:";
            rdbApplicationStart.UseVisualStyleBackColor = true;
            rdbApplicationStart.CheckedChanged += radioButton_CheckedChanged_UpdateDirty;
            // 
            // txtApplicationPath
            // 
            txtApplicationPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", recorderProjectBindingSource, "Executable", true));
            txtApplicationPath.Dock = System.Windows.Forms.DockStyle.Fill;
            txtApplicationPath.Location = new System.Drawing.Point(163, 3);
            txtApplicationPath.Name = "txtApplicationPath";
            txtApplicationPath.Size = new System.Drawing.Size(550, 39);
            txtApplicationPath.TabIndex = 4;
            txtApplicationPath.TextChanged += radioButton_CheckedChanged_UpdateDirty;
            // 
            // btnApplicationBrowse
            // 
            btnApplicationBrowse.AutoSize = true;
            btnApplicationBrowse.Dock = System.Windows.Forms.DockStyle.Top;
            btnApplicationBrowse.Location = new System.Drawing.Point(719, 3);
            btnApplicationBrowse.Name = "btnApplicationBrowse";
            btnApplicationBrowse.Size = new System.Drawing.Size(42, 42);
            btnApplicationBrowse.TabIndex = 7;
            btnApplicationBrowse.Text = "...";
            btnApplicationBrowse.UseVisualStyleBackColor = true;
            btnApplicationBrowse.Click += btnApplicationBrowse_Click;
            // 
            // argsRowTableLayout
            // 
            argsRowTableLayout.ColumnCount = 2;
            argsRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            argsRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            argsRowTableLayout.Controls.Add(lblApplicationArgs, 0, 0);
            argsRowTableLayout.Controls.Add(txtApplicationArgs, 1, 0);
            argsRowTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            argsRowTableLayout.Location = new System.Drawing.Point(3, 63);
            argsRowTableLayout.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            argsRowTableLayout.Name = "argsRowTableLayout";
            argsRowTableLayout.RowCount = 1;
            argsRowTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            argsRowTableLayout.Size = new System.Drawing.Size(764, 51);
            argsRowTableLayout.TabIndex = 1;
            // 
            // lblApplicationArgs
            // 
            lblApplicationArgs.AutoSize = true;
            lblApplicationArgs.Dock = System.Windows.Forms.DockStyle.Top;
            lblApplicationArgs.Location = new System.Drawing.Point(3, 3);
            lblApplicationArgs.Margin = new System.Windows.Forms.Padding(3);
            lblApplicationArgs.Name = "lblApplicationArgs";
            lblApplicationArgs.Size = new System.Drawing.Size(154, 32);
            lblApplicationArgs.TabIndex = 8;
            lblApplicationArgs.Text = "Args:";
            lblApplicationArgs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtApplicationArgs
            // 
            txtApplicationArgs.DataBindings.Add(new System.Windows.Forms.Binding("Text", recorderProjectBindingSource, "Arguments", true));
            txtApplicationArgs.Dock = System.Windows.Forms.DockStyle.Fill;
            txtApplicationArgs.Location = new System.Drawing.Point(163, 3);
            txtApplicationArgs.Name = "txtApplicationArgs";
            txtApplicationArgs.Size = new System.Drawing.Size(598, 39);
            txtApplicationArgs.TabIndex = 5;
            // 
            // processRowTableLayout
            // 
            processRowTableLayout.ColumnCount = 3;
            processRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            processRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            processRowTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            processRowTableLayout.Controls.Add(rdbApplicationProcess, 0, 0);
            processRowTableLayout.Controls.Add(cboApplicationProcess, 1, 0);
            processRowTableLayout.Controls.Add(btnProcessRefresh, 2, 0);
            processRowTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            processRowTableLayout.Location = new System.Drawing.Point(3, 123);
            processRowTableLayout.Name = "processRowTableLayout";
            processRowTableLayout.RowCount = 1;
            processRowTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            processRowTableLayout.Size = new System.Drawing.Size(764, 89);
            processRowTableLayout.TabIndex = 2;
            // 
            // rdbApplicationProcess
            // 
            rdbApplicationProcess.AutoSize = true;
            rdbApplicationProcess.Dock = System.Windows.Forms.DockStyle.Top;
            rdbApplicationProcess.Location = new System.Drawing.Point(3, 3);
            rdbApplicationProcess.Name = "rdbApplicationProcess";
            rdbApplicationProcess.Size = new System.Drawing.Size(154, 36);
            rdbApplicationProcess.TabIndex = 3;
            rdbApplicationProcess.Text = "Process:";
            rdbApplicationProcess.UseVisualStyleBackColor = true;
            // 
            // cboApplicationProcess
            // 
            cboApplicationProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            cboApplicationProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboApplicationProcess.FormattingEnabled = true;
            cboApplicationProcess.Location = new System.Drawing.Point(163, 3);
            cboApplicationProcess.Name = "cboApplicationProcess";
            cboApplicationProcess.Size = new System.Drawing.Size(558, 40);
            cboApplicationProcess.TabIndex = 6;
            cboApplicationProcess.SelectedIndexChanged += radioButton_CheckedChanged_UpdateDirty;
            cboApplicationProcess.Format += cboApplicationProcess_Format;
            // 
            // btnProcessRefresh
            // 
            btnProcessRefresh.AutoSize = true;
            btnProcessRefresh.Dock = System.Windows.Forms.DockStyle.Top;
            btnProcessRefresh.Image = (System.Drawing.Image)resources.GetObject("btnProcessRefresh.Image");
            btnProcessRefresh.Location = new System.Drawing.Point(727, 3);
            btnProcessRefresh.Name = "btnProcessRefresh";
            btnProcessRefresh.Size = new System.Drawing.Size(34, 40);
            btnProcessRefresh.TabIndex = 7;
            btnProcessRefresh.UseVisualStyleBackColor = true;
            btnProcessRefresh.Click += btnProcessRefresh_Click;
            // 
            // openApplicationDialog
            // 
            openApplicationDialog.FileName = "*.exe";
            openApplicationDialog.Filter = "Applications (*.exe)|*.exe";
            openApplicationDialog.Title = "Choose target application";
            // 
            // openProjectDialog
            // 
            openProjectDialog.FileName = "*.urp";
            openProjectDialog.Filter = "FlaUI Recorder projects (*.urp)|*.urp";
            openProjectDialog.Title = "Open an existing project";
            // 
            // saveProjectDialog
            // 
            saveProjectDialog.DefaultExt = "*.urp";
            saveProjectDialog.Filter = "FlaUI Recorder projects (*.urp)|*.urp";
            saveProjectDialog.Title = "Save recorder project";
            // 
            // lstSessions
            // 
            lstSessions.DataSource = sessionsBindingSource;
            lstSessions.Dock = System.Windows.Forms.DockStyle.Fill;
            lstSessions.FormattingEnabled = true;
            lstSessions.IntegralHeight = false;
            lstSessions.ItemHeight = 32;
            lstSessions.Location = new System.Drawing.Point(3, 38);
            lstSessions.Name = "lstSessions";
            lstSessions.Size = new System.Drawing.Size(520, 453);
            lstSessions.TabIndex = 6;
            lstSessions.DoubleClick += lstSessions_DoubleClick;
            // 
            // sessionsBindingSource
            // 
            sessionsBindingSource.DataMember = "Sessions";
            sessionsBindingSource.DataSource = recorderProjectBindingSource;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(3, 0);
            label1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(520, 32);
            label1.TabIndex = 7;
            label1.Text = "Recorded sessions";
            // 
            // mainTableLayout
            // 
            mainTableLayout.ColumnCount = 2;
            mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            mainTableLayout.Controls.Add(leftTableLayout, 0, 0);
            mainTableLayout.Controls.Add(rightTableLayout, 1, 0);
            mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            mainTableLayout.Location = new System.Drawing.Point(8, 47);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.Padding = new System.Windows.Forms.Padding(8);
            mainTableLayout.RowCount = 1;
            mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            mainTableLayout.Size = new System.Drawing.Size(1346, 516);
            mainTableLayout.TabIndex = 8;
            // 
            // leftTableLayout
            // 
            leftTableLayout.BackColor = System.Drawing.Color.Transparent;
            leftTableLayout.ColumnCount = 1;
            leftTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            leftTableLayout.Controls.Add(topSettingsTableLayout, 0, 0);
            leftTableLayout.Controls.Add(groupBox3, 0, 1);
            leftTableLayout.Controls.Add(startPanel, 0, 2);
            leftTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            leftTableLayout.Location = new System.Drawing.Point(11, 11);
            leftTableLayout.Name = "leftTableLayout";
            leftTableLayout.RowCount = 3;
            leftTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            leftTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            leftTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            leftTableLayout.Size = new System.Drawing.Size(792, 494);
            leftTableLayout.TabIndex = 0;
            // 
            // topSettingsTableLayout
            // 
            topSettingsTableLayout.ColumnCount = 2;
            topSettingsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            topSettingsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            topSettingsTableLayout.Controls.Add(groupBox1, 0, 0);
            topSettingsTableLayout.Controls.Add(groupBox2, 1, 0);
            topSettingsTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            topSettingsTableLayout.Location = new System.Drawing.Point(3, 3);
            topSettingsTableLayout.Name = "topSettingsTableLayout";
            topSettingsTableLayout.RowCount = 1;
            topSettingsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            topSettingsTableLayout.Size = new System.Drawing.Size(786, 154);
            topSettingsTableLayout.TabIndex = 0;
            // 
            // startPanel
            // 
            startPanel.AutoSize = true;
            startPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            startPanel.Controls.Add(btnStart);
            startPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            startPanel.Location = new System.Drawing.Point(3, 438);
            startPanel.Name = "startPanel";
            startPanel.Size = new System.Drawing.Size(786, 53);
            startPanel.TabIndex = 1;
            // 
            // rightTableLayout
            // 
            rightTableLayout.ColumnCount = 1;
            rightTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rightTableLayout.Controls.Add(label1, 0, 0);
            rightTableLayout.Controls.Add(lstSessions, 0, 1);
            rightTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            rightTableLayout.Location = new System.Drawing.Point(809, 11);
            rightTableLayout.Name = "rightTableLayout";
            rightTableLayout.RowCount = 2;
            rightTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            rightTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rightTableLayout.Size = new System.Drawing.Size(526, 494);
            rightTableLayout.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusLabelReady, statusLabelErrors });
            statusStrip1.Location = new System.Drawing.Point(8, 563);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1346, 41);
            statusStrip1.TabIndex = 9;
            statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelReady
            // 
            statusLabelReady.Name = "statusLabelReady";
            statusLabelReady.Size = new System.Drawing.Size(1224, 31);
            statusLabelReady.Spring = true;
            statusLabelReady.Text = "Ready";
            statusLabelReady.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelErrors
            // 
            statusLabelErrors.IsLink = true;
            statusLabelErrors.Name = "statusLabelErrors";
            statusLabelErrors.Size = new System.Drawing.Size(107, 31);
            statusLabelErrors.Text = "Errors: 0";
            statusLabelErrors.Click += ErrorStatusLabel_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1362, 612);
            Controls.Add(mainTableLayout);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new System.Drawing.Size(900, 520);
            Name = "MainForm";
            Padding = new System.Windows.Forms.Padding(8);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "FlaUI - Recorder";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            groupBox1.ResumeLayout(false);
            uiaTableLayout.ResumeLayout(false);
            uiaTableLayout.PerformLayout();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)recorderProjectBindingSource).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox3.ResumeLayout(false);
            targetAppTableLayout.ResumeLayout(false);
            pathRowTableLayout.ResumeLayout(false);
            pathRowTableLayout.PerformLayout();
            argsRowTableLayout.ResumeLayout(false);
            argsRowTableLayout.PerformLayout();
            processRowTableLayout.ResumeLayout(false);
            processRowTableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)sessionsBindingSource).EndInit();
            mainTableLayout.ResumeLayout(false);
            leftTableLayout.ResumeLayout(false);
            leftTableLayout.PerformLayout();
            topSettingsTableLayout.ResumeLayout(false);
            startPanel.ResumeLayout(false);
            startPanel.PerformLayout();
            rightTableLayout.ResumeLayout(false);
            rightTableLayout.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
