using System;
using System.Drawing;
using System.Windows.Forms;
using FlaUI.Core;
using FlaUIRecorder.CodeProvider.Common;
using FlaUIRecorder.Internal;
using System.Diagnostics;

namespace FlaUIRecorder
{
    public partial class RecorderForm : Form
    {
        private Recorder _recorder;
        private MainForm _mainForm;
        private DateTime _recordingStart;
        private readonly Timer _statusTimer = new Timer();

        public RecorderForm()
        {
            InitializeComponent();
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.MinimumSize = this.Size;

            _statusTimer.Interval = 1000;
            _statusTimer.Tick += StatusTimer_Tick;
        }

        public void Initialize(AutomationType automationType, CodeProviderFactory codeProviderFactory, string codeProviderName, MainForm mainForm, Process targetProcess)
        {
            _recorder = new Recorder(automationType, codeProviderFactory, codeProviderName, targetProcess);
            _recorder.ActionRecorded += Recorder_ActionRecorded;
            _recorder.StatusChanged += Recorder_StatusChanged;
            _mainForm = mainForm;
        }

        public System.Collections.Generic.List<RecordedAction> Actions => _recorder?.Actions ?? new System.Collections.Generic.List<RecordedAction>();

        protected override void OnClosed(EventArgs e)
        {
            _statusTimer.Stop();
            base.OnClosed(e);
            _recorder?.Dispose();
            _mainForm = null;
        }

        public void ShowInLowerRightCorner()
        {
            Show();
            LocateInLowerRightCorner();
        }

        private void LocateInLowerRightCorner()
        {
            var bounds = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(bounds.Right - (this.Width + 5), bounds.Bottom - (this.Height + 5));
        }

        public void Record()
        {
            _recordingStart = DateTime.Now;
            _recorder.Record();
            _statusTimer.Start();
            this.toolTip1.SetToolTip(this.btnPausePlay, "Pause the recording");
            btnPausePlay.ImageIndex = 0;
            UpdateTitle();
            UpdateStatusLabel();
        }

        public void Pause()
        {
            _recorder.Pause();
            _statusTimer.Stop();
            this.toolTip1.SetToolTip(this.btnPausePlay, "Resumes the recording");
            btnPausePlay.ImageIndex = 1;
            UpdateTitle();
            UpdateStatusLabel();
        }

        private void UpdateTitle()
        {
            Text = "FlaUI Recorder - " + _recorder.State.ToString();
        }

        private void UpdateStatusLabel()
        {
            if (_recorder == null)
                return;

            var elapsed = DateTime.Now - _recordingStart;
            lblStatus.Text = $"Actions: {_recorder.Actions.Count} | Time: {elapsed:mm\\:ss}";
        }

        private void Recorder_ActionRecorded(object sender, RecordedAction action)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Recorder_ActionRecorded(sender, action)));
                return;
            }

            UpdateStatusLabel();

            if (action != null && action.Type != ActionType.Comment && action.Type != ActionType.Wait && action.Type != ActionType.KeyPress)
            {
                var label = !string.IsNullOrEmpty(action.Name) ? action.Name : action.AutomationId;
                if (!string.IsNullOrEmpty(label))
                    lblLastElement.Text = $"Last: {label}";
            }
        }

        private void Recorder_StatusChanged(object sender, string status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Recorder_StatusChanged(sender, status)));
                return;
            }

            if (!string.IsNullOrEmpty(status))
                lblLastElement.Text = status;
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            UpdateStatusLabel();
        }

        private void btnAddComment_Click(object sender, EventArgs e)
        {
            var resumeRecording = false;

            if (_recorder.State == RecorderState.Recording)
            {
                Pause();
                resumeRecording = true;
            }

            using (var form = new CommentForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    _recorder.AddComment(form.Comment);
            }

            if (resumeRecording)
                Record();
        }

        private void btnPausePlay_Click(object sender, EventArgs e)
        {
            if (_recorder.State == RecorderState.Paused)
                Record();
            else
                Pause();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _mainForm.CancelRecording();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            _mainForm.Finished(_recorder.GenerateCode());
        }

        private void btnInsertWait_Click(object sender, EventArgs e)
        {
            _recorder.InsertWait(500);
            UpdateStatusLabel();
        }

        private void btnAddAssertion_Click(object sender, EventArgs e)
        {
            var element = _recorder.GetCurrentHoveredElement();
            if (element == null)
            {
                MessageBox.Show(this, "Hover over the target element first, then click Add Assertion.", "No Element", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _recorder.AddAssertion(element);
            UpdateStatusLabel();
        }
    }
}
