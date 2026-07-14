namespace FlaUIRecorder
{
    partial class ExportProjectDialog
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
            lblFlaUI = new System.Windows.Forms.Label();
            _rdbFlaUI50 = new System.Windows.Forms.RadioButton();
            _rdbFlaUI40 = new System.Windows.Forms.RadioButton();
            _chkPageObjects = new System.Windows.Forms.CheckBox();
            _chkScreenshotOnFailure = new System.Windows.Forms.CheckBox();
            _chkContinueOnError = new System.Windows.Forms.CheckBox();
            _chkHtmlReport = new System.Windows.Forms.CheckBox();
            _btnOk = new System.Windows.Forms.Button();
            _btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblFlaUI
            // 
            lblFlaUI.AutoSize = true;
            lblFlaUI.Location = new System.Drawing.Point(12, 7);
            lblFlaUI.Name = "lblFlaUI";
            lblFlaUI.Size = new System.Drawing.Size(477, 32);
            lblFlaUI.TabIndex = 0;
            lblFlaUI.Text = "Target FlaUI version (exported project only):";
            // 
            // _rdbFlaUI50
            // 
            _rdbFlaUI50.AutoSize = true;
            _rdbFlaUI50.Checked = true;
            _rdbFlaUI50.Location = new System.Drawing.Point(28, 42);
            _rdbFlaUI50.Name = "_rdbFlaUI50";
            _rdbFlaUI50.Size = new System.Drawing.Size(510, 36);
            _rdbFlaUI50.TabIndex = 1;
            _rdbFlaUI50.TabStop = true;
            _rdbFlaUI50.Text = "FlaUI 5.0.0 (net7.0-windows, recommended)";
            _rdbFlaUI50.UseVisualStyleBackColor = true;
            // 
            // _rdbFlaUI40
            // 
            _rdbFlaUI40.AutoSize = true;
            _rdbFlaUI40.Location = new System.Drawing.Point(28, 78);
            _rdbFlaUI40.Name = "_rdbFlaUI40";
            _rdbFlaUI40.Size = new System.Drawing.Size(310, 36);
            _rdbFlaUI40.TabIndex = 2;
            _rdbFlaUI40.Text = "FlaUI 4.0 (net472, legacy)";
            _rdbFlaUI40.UseVisualStyleBackColor = true;
            // 
            // _chkPageObjects
            // 
            _chkPageObjects.AutoSize = true;
            _chkPageObjects.Location = new System.Drawing.Point(12, 113);
            _chkPageObjects.Name = "_chkPageObjects";
            _chkPageObjects.Size = new System.Drawing.Size(353, 36);
            _chkPageObjects.TabIndex = 3;
            _chkPageObjects.Text = "Generate Page Object model";
            _chkPageObjects.UseVisualStyleBackColor = true;
            // 
            // _chkScreenshotOnFailure
            // 
            _chkScreenshotOnFailure.AutoSize = true;
            _chkScreenshotOnFailure.Checked = true;
            _chkScreenshotOnFailure.CheckState = System.Windows.Forms.CheckState.Checked;
            _chkScreenshotOnFailure.Location = new System.Drawing.Point(12, 150);
            _chkScreenshotOnFailure.Name = "_chkScreenshotOnFailure";
            _chkScreenshotOnFailure.Size = new System.Drawing.Size(549, 36);
            _chkScreenshotOnFailure.TabIndex = 4;
            _chkScreenshotOnFailure.Text = "Capture screenshot on failure in exported code";
            _chkScreenshotOnFailure.UseVisualStyleBackColor = true;
            // 
            // _chkContinueOnError
            // 
            _chkContinueOnError.AutoSize = true;
            _chkContinueOnError.Checked = true;
            _chkContinueOnError.CheckState = System.Windows.Forms.CheckState.Checked;
            _chkContinueOnError.Location = new System.Drawing.Point(12, 187);
            _chkContinueOnError.Name = "_chkContinueOnError";
            _chkContinueOnError.Size = new System.Drawing.Size(497, 36);
            _chkContinueOnError.TabIndex = 5;
            _chkContinueOnError.Text = "Continue on error (log failures, don't stop)";
            _chkContinueOnError.UseVisualStyleBackColor = true;
            // 
            // _chkHtmlReport
            // 
            _chkHtmlReport.AutoSize = true;
            _chkHtmlReport.Checked = true;
            _chkHtmlReport.CheckState = System.Windows.Forms.CheckState.Checked;
            _chkHtmlReport.Location = new System.Drawing.Point(12, 222);
            _chkHtmlReport.Name = "_chkHtmlReport";
            _chkHtmlReport.Size = new System.Drawing.Size(331, 36);
            _chkHtmlReport.TabIndex = 5;
            _chkHtmlReport.Text = "Generate HTML test report";
            _chkHtmlReport.UseVisualStyleBackColor = true;
            // 
            // _btnOk
            // 
            _btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            _btnOk.Location = new System.Drawing.Point(394, 259);
            _btnOk.Name = "_btnOk";
            _btnOk.Size = new System.Drawing.Size(174, 43);
            _btnOk.TabIndex = 6;
            _btnOk.Text = "Export";
            _btnOk.UseVisualStyleBackColor = true;
            // 
            // _btnCancel
            // 
            _btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _btnCancel.Location = new System.Drawing.Point(585, 255);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new System.Drawing.Size(177, 47);
            _btnCancel.TabIndex = 7;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            // 
            // ExportProjectDialog
            // 
            AcceptButton = _btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = _btnCancel;
            ClientSize = new System.Drawing.Size(778, 314);
            Controls.Add(_btnCancel);
            Controls.Add(_btnOk);
            Controls.Add(_chkHtmlReport);
            Controls.Add(_chkContinueOnError);
            Controls.Add(_chkScreenshotOnFailure);
            Controls.Add(_chkPageObjects);
            Controls.Add(_rdbFlaUI40);
            Controls.Add(_rdbFlaUI50);
            Controls.Add(lblFlaUI);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExportProjectDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Export Project Options";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblFlaUI;
        private System.Windows.Forms.RadioButton _rdbFlaUI50;
        private System.Windows.Forms.RadioButton _rdbFlaUI40;
        private System.Windows.Forms.CheckBox _chkPageObjects;
        private System.Windows.Forms.CheckBox _chkScreenshotOnFailure;
        private System.Windows.Forms.CheckBox _chkContinueOnError;
        private System.Windows.Forms.CheckBox _chkHtmlReport;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
    }
}
