using System;
using System.Windows.Forms;

namespace FlaUIRecorder
{
    public class ExportProjectDialog : Form
    {
        private readonly RadioButton _rdbFlaUI12;
        private readonly RadioButton _rdbFlaUI40;
        private readonly CheckBox _chkPageObjects;
        private readonly CheckBox _chkScreenshotOnFailure;
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
            ClientSize = new System.Drawing.Size(420, 220);

            var lblFlaUI = new Label { Text = "Target FlaUI version (exported project only):", Location = new System.Drawing.Point(12, 12), AutoSize = true };
            _rdbFlaUI12 = new RadioButton
            {
                Text = "FlaUI 1.2.0 (net461, compatible with recorder)",
                Location = new System.Drawing.Point(28, 34),
                AutoSize = true,
                Checked = true
            };
            _rdbFlaUI40 = new RadioButton
            {
                Text = "FlaUI 4.0 (net472, modern API — see export comments)",
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

            _btnOk = new Button { Text = "Export", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(240, 175), Width = 75 };
            _btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(325, 175), Width = 75 };

            Controls.AddRange(new Control[] { lblFlaUI, _rdbFlaUI12, _rdbFlaUI40, _chkPageObjects, _chkScreenshotOnFailure, _btnOk, _btnCancel });
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                Options = new ExportOptions
                {
                    FlaUIVersion = _rdbFlaUI40.Checked ? ExportOptions.FlaUIVersion40 : ExportOptions.FlaUIVersion12,
                    TargetFramework = _rdbFlaUI40.Checked ? "net472" : "net461",
                    GeneratePageObjects = _chkPageObjects.Checked,
                    CaptureScreenshotOnFailure = _chkScreenshotOnFailure.Checked
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
            this.ClientSize = new System.Drawing.Size(630, 278);
            this.Name = "ExportProjectDialog";
            this.ResumeLayout(false);

        }
    }
}
