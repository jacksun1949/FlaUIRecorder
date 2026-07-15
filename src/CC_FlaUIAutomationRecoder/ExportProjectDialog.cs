using System;
using System.Windows.Forms;

namespace CC_FlaUIAutomationRecoder
{
    public partial class ExportProjectDialog : Form
    {
        public ExportOptions Options { get; private set; }

        public ExportProjectDialog(int totalActions)
        {
            InitializeComponent();

            // Set Page Object checkbox default based on action count
            _chkPageObjects.Checked = totalActions > 10;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                Options = new ExportOptions
                {
                    FlaUIVersion = _rdbFlaUI50.Checked ? ExportOptions.FlaUIVersion50 : ExportOptions.FlaUIVersion40,
                    TargetFramework = _rdbFlaUI50.Checked ? "net7.0-windows" : "net472",
                    GeneratePageObjects = _chkPageObjects.Checked,
                    CaptureScreenshotOnFailure = _chkScreenshotOnFailure.Checked,
                    ContinueOnError = _chkContinueOnError.Checked,
                    GenerateHtmlReport = _chkHtmlReport.Checked
                };
            }
            base.OnFormClosing(e);
        }

    }
}
