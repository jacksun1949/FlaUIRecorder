using System;
using System.Windows.Forms;

namespace FlaUIRecorder
{
    public class ExportProjectDialog : Form
    {
        private readonly RadioButton _rdbFlaUI50;
        private readonly RadioButton _rdbFlaUI40;
        private readonly CheckBox _chkPageObjects;
        private readonly CheckBox _chkScreenshotOnFailure;
        private readonly CheckBox _chkContinueOnError;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        public ExportOptions Options { get; private set; }

        public ExportProjectDialog(int totalActions)
        {
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new System.Drawing.Font("Segoe UI", 9F);

            Text = "Export Project Options";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new System.Drawing.Size(420, 250);

            var lblFlaUI = new Label { Text = "Target FlaUI version (exported project only):", Location = new System.Drawing.Point(12, 12), AutoSize = true };
            _rdbFlaUI50 = new RadioButton
            {
                Text = "FlaUI 5.0.0 (net8.0-windows, recommended)",
                Location = new System.Drawing.Point(28, 34),
                AutoSize = true,
                Checked = true
            };
            _rdbFlaUI40 = new RadioButton
            {
                Text = "FlaUI 4.0 (net472, legacy)",
                Location = new System.Drawing.Point(28, 56),
                AutoSize = true
            };

            _chkPageObjects = new CheckBox
            {
                Text = "Generate Page Object model",
                Location = new System.Drawing.Point(12, 88),
                AutoSize = true,
                Checked = totalActions > 10
            };
            _chkScreenshotOnFailure = new CheckBox
            {
                Text = "Capture screenshot on failure in exported code",
                Location = new System.Drawing.Point(12, 112),
                AutoSize = true,
                Checked = true
            };
            _chkContinueOnError = new CheckBox
            {
                Text = "Continue on error (log failures, don't stop)",
                Location = new System.Drawing.Point(12, 136),
                AutoSize = true,
                Checked = true
            };

            _btnOk = new Button { Text = "Export", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(240, 195), Width = 75 };
            _btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(325, 195), Width = 75 };

            Controls.AddRange(new Control[] { lblFlaUI, _rdbFlaUI50, _rdbFlaUI40, _chkPageObjects, _chkScreenshotOnFailure, _chkContinueOnError, _btnOk, _btnCancel });
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                Options = new ExportOptions
                {
                    FlaUIVersion = _rdbFlaUI50.Checked ? ExportOptions.FlaUIVersion50 : ExportOptions.FlaUIVersion40,
                    TargetFramework = _rdbFlaUI50.Checked ? "net8.0-windows" : "net472",
                    GeneratePageObjects = _chkPageObjects.Checked,
                    CaptureScreenshotOnFailure = _chkScreenshotOnFailure.Checked,
                    ContinueOnError = _chkContinueOnError.Checked
                };
            }
            base.OnFormClosing(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ExportProjectDialog
            // 
            this.Name = "ExportProjectDialog";
            this.ResumeLayout(false);

        }
    }
}
