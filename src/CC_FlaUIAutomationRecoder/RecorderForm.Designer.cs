namespace CC_FlaUIAutomationRecoder
{
    partial class RecorderForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecorderForm));
            this.btnPausePlay = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnAddComment = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnInsertWait = new System.Windows.Forms.Button();
            this.btnAddAssertion = new System.Windows.Forms.Button();
            this.btnRestartHooks = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblLastElement = new System.Windows.Forms.Label();
            this.lblHotkeys = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolbarFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.infoFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.toolbarFlow.SuspendLayout();
            this.infoFlow.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPausePlay
            // 
            this.btnPausePlay.ImageIndex = 0;
            this.btnPausePlay.ImageList = this.imageList2;
            this.btnPausePlay.Location = new System.Drawing.Point(12, 12);
            this.btnPausePlay.Name = "btnPausePlay";
            this.btnPausePlay.Size = new System.Drawing.Size(32, 32);
            this.btnPausePlay.TabIndex = 0;
            this.btnPausePlay.UseVisualStyleBackColor = true;
            this.btnPausePlay.Click += new System.EventHandler(this.btnPausePlay_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Pause_grey_32xLG.png");
            this.imageList2.Images.SetKeyName(1, "RecordScreen_32x.png");
            this.imageList2.Images.SetKeyName(2, "Stop_grey_32xLG.png");
            this.imageList2.Images.SetKeyName(3, "Comment_24x.png");
            this.imageList2.Images.SetKeyName(4, "FrameworkDesignStudio_32x.png");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Pause_grey_16x.png");
            this.imageList1.Images.SetKeyName(1, "Record_16x.png");
            this.imageList1.Images.SetKeyName(2, "Stop_grey_16x.png");
            this.imageList1.Images.SetKeyName(3, "AddComment_16x.png");
            this.imageList1.Images.SetKeyName(4, "GenerateRecordCode_16x.png");
            // 
            // btnAddComment
            // 
            this.btnAddComment.ImageIndex = 3;
            this.btnAddComment.ImageList = this.imageList2;
            this.btnAddComment.Location = new System.Drawing.Point(88, 12);
            this.btnAddComment.Name = "btnAddComment";
            this.btnAddComment.Size = new System.Drawing.Size(32, 32);
            this.btnAddComment.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnAddComment, "Add comment to the code");
            this.btnAddComment.UseVisualStyleBackColor = true;
            this.btnAddComment.Click += new System.EventHandler(this.btnAddComment_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.ImageIndex = 4;
            this.btnGenerate.ImageList = this.imageList2;
            this.btnGenerate.Location = new System.Drawing.Point(202, 12);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(32, 32);
            this.btnGenerate.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnGenerate, "Stops recording and generate code");
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ImageIndex = 2;
            this.btnCancel.ImageList = this.imageList2;
            this.btnCancel.Location = new System.Drawing.Point(50, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(32, 32);
            this.btnCancel.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnCancel, "Cancel the recording");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnInsertWait
            // 
            this.btnInsertWait.Location = new System.Drawing.Point(126, 12);
            this.btnInsertWait.Name = "btnInsertWait";
            this.btnInsertWait.Size = new System.Drawing.Size(32, 32);
            this.btnInsertWait.TabIndex = 4;
            this.btnInsertWait.Text = "W";
            this.toolTip1.SetToolTip(this.btnInsertWait, "Insert Wait (500ms)");
            this.btnInsertWait.UseVisualStyleBackColor = true;
            this.btnInsertWait.Click += new System.EventHandler(this.btnInsertWait_Click);
            // 
            // btnAddAssertion
            // 
            this.btnAddAssertion.Location = new System.Drawing.Point(164, 12);
            this.btnAddAssertion.Name = "btnAddAssertion";
            this.btnAddAssertion.Size = new System.Drawing.Size(32, 32);
            this.btnAddAssertion.TabIndex = 5;
            this.btnAddAssertion.Text = "A";
            this.toolTip1.SetToolTip(this.btnAddAssertion, "Add assertion for hovered element");
            this.btnAddAssertion.UseVisualStyleBackColor = true;
            this.btnAddAssertion.Click += new System.EventHandler(this.btnAddAssertion_Click);
            // 
            // btnRestartHooks
            // 
            this.btnRestartHooks.Location = new System.Drawing.Point(240, 12);
            this.btnRestartHooks.Name = "btnRestartHooks";
            this.btnRestartHooks.Size = new System.Drawing.Size(32, 32);
            this.btnRestartHooks.TabIndex = 8;
            this.btnRestartHooks.Text = "R";
            this.btnRestartHooks.UseVisualStyleBackColor = true;
            this.btnRestartHooks.Visible = false;
            this.btnRestartHooks.Click += new System.EventHandler(this.btnRestartHooks_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 0);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(259, 32);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Actions: 0 | Time: 00:00";
            // 
            // lblLastElement
            // 
            this.lblLastElement.AutoSize = true;
            this.lblLastElement.Location = new System.Drawing.Point(15, 35);
            this.lblLastElement.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblLastElement.Name = "lblLastElement";
            this.lblLastElement.Size = new System.Drawing.Size(77, 32);
            this.lblLastElement.TabIndex = 7;
            this.lblLastElement.Text = "Last: -";
            // 
            // lblHotkeys
            // 
            this.lblHotkeys.AutoSize = true;
            this.lblHotkeys.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.lblHotkeys.ForeColor = System.Drawing.Color.Gray;
            this.lblHotkeys.Location = new System.Drawing.Point(15, 70);
            this.lblHotkeys.Name = "lblHotkeys";
            this.lblHotkeys.Size = new System.Drawing.Size(615, 25);
            this.lblHotkeys.TabIndex = 9;
            this.lblHotkeys.Text = "Hotkeys: P=pause C=comment W=wait A=assert E=element Esc=cancel";
            // 
            // toolbarFlow
            // 
            this.toolbarFlow.AutoSize = true;
            this.toolbarFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.toolbarFlow.Controls.Add(this.btnPausePlay);
            this.toolbarFlow.Controls.Add(this.btnCancel);
            this.toolbarFlow.Controls.Add(this.btnAddComment);
            this.toolbarFlow.Controls.Add(this.btnInsertWait);
            this.toolbarFlow.Controls.Add(this.btnAddAssertion);
            this.toolbarFlow.Controls.Add(this.btnGenerate);
            this.toolbarFlow.Controls.Add(this.btnRestartHooks);
            this.toolbarFlow.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbarFlow.Location = new System.Drawing.Point(0, 0);
            this.toolbarFlow.Name = "toolbarFlow";
            this.toolbarFlow.Padding = new System.Windows.Forms.Padding(9, 9, 9, 3);
            this.toolbarFlow.Size = new System.Drawing.Size(725, 50);
            this.toolbarFlow.TabIndex = 10;
            this.toolbarFlow.WrapContents = false;
            // 
            // infoFlow
            // 
            this.infoFlow.AutoSize = true;
            this.infoFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.infoFlow.Controls.Add(this.lblStatus);
            this.infoFlow.Controls.Add(this.lblLastElement);
            this.infoFlow.Controls.Add(this.lblHotkeys);
            this.infoFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.infoFlow.Location = new System.Drawing.Point(0, 50);
            this.infoFlow.Name = "infoFlow";
            this.infoFlow.Padding = new System.Windows.Forms.Padding(12, 0, 12, 9);
            this.infoFlow.Size = new System.Drawing.Size(725, 102);
            this.infoFlow.TabIndex = 11;
            this.infoFlow.WrapContents = false;
            // 
            // RecorderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(725, 152);
            this.ControlBox = false;
            this.Controls.Add(this.infoFlow);
            this.Controls.Add(this.toolbarFlow);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RecorderForm";
            this.Text = "FlaUI Recorder";
            this.TopMost = true;
            this.toolbarFlow.ResumeLayout(false);
            this.infoFlow.ResumeLayout(false);
            this.infoFlow.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPausePlay;
        private System.Windows.Forms.Button btnAddComment;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnInsertWait;
        private System.Windows.Forms.Button btnAddAssertion;
        private System.Windows.Forms.Button btnRestartHooks;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblLastElement;
        private System.Windows.Forms.Label lblHotkeys;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.FlowLayoutPanel toolbarFlow;
        private System.Windows.Forms.FlowLayoutPanel infoFlow;
    }
}
