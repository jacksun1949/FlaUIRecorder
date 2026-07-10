using System;
using System.Drawing;
using System.Runtime.InteropServices;
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
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            _statusTimer.Interval = 1000;
            _statusTimer.Tick += StatusTimer_Tick;

            SetupHotkeyTooltips();
        }

        private void SetupHotkeyTooltips()
        {
            toolTip1.SetToolTip(btnPausePlay, "Pause/Resume (Ctrl+Shift+Alt+P)");
            toolTip1.SetToolTip(btnAddComment, "Add comment (Ctrl+Shift+Alt+C)");
            toolTip1.SetToolTip(btnInsertWait, "Insert wait (Ctrl+Shift+Alt+W)");
            toolTip1.SetToolTip(btnAddAssertion, "Add assertion (Ctrl+Shift+Alt+A)");
            toolTip1.SetToolTip(btnCancel, "Cancel recording (Ctrl+Shift+Alt+Esc)");
            toolTip1.SetToolTip(btnRestartHooks, "Restart input hooks");
            lblHotkeys.Text = "Hotkeys: P=pause C=comment W=wait A=assert E=element Esc=cancel";
        }

        public void Initialize(AutomationType automationType, CodeProviderFactory codeProviderFactory, string codeProviderName, MainForm mainForm, Process targetProcess)
        {
            _recorder = new Recorder(automationType, codeProviderFactory, codeProviderName, targetProcess);
            _recorder.ActionRecorded += Recorder_ActionRecorded;
            _recorder.StatusChanged += Recorder_StatusChanged;
            _mainForm = mainForm;
        }

        public System.Collections.Generic.List<RecordedAction> Actions => _recorder?.Actions ?? new System.Collections.Generic.List<RecordedAction>();

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RegisterHotkeys();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_HOTKEY)
            {
                switch (m.WParam.ToInt32())
                {
                    case NativeMethods.HOTKEY_PAUSE:
                        btnPausePlay_Click(this, EventArgs.Empty);
                        return;
                    case NativeMethods.HOTKEY_COMMENT:
                        btnAddComment_Click(this, EventArgs.Empty);
                        return;
                    case NativeMethods.HOTKEY_WAIT:
                        btnInsertWait_Click(this, EventArgs.Empty);
                        return;
                    case NativeMethods.HOTKEY_ASSERT:
                        btnAddAssertion_Click(this, EventArgs.Empty);
                        return;
                    case NativeMethods.HOTKEY_CANCEL:
                        btnCancel_Click(this, EventArgs.Empty);
                        return;
                    case NativeMethods.HOTKEY_ELEMENT_TREE:
                        ShowElementTree();
                        return;
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnClosed(EventArgs e)
        {
            UnregisterHotkeys();
            _statusTimer.Stop();
            _recorder?.Dispose();
            base.OnClosed(e);
            _mainForm = null;
        }

        private void RegisterHotkeys()
        {
            int mod = NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT | NativeMethods.MOD_ALT;
            NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_PAUSE, mod, 0x50);   // P
            NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_COMMENT, mod, 0x43); // C
            NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_WAIT, mod, 0x57);    // W
            NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ASSERT, mod, 0x41);  // A
            NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_CANCEL, mod, 0x1B);  // Esc
            NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ELEMENT_TREE, mod, 0x45); // E
        }

        private void UnregisterHotkeys()
        {
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_PAUSE);
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_COMMENT);
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_WAIT);
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_ASSERT);
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_CANCEL);
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_ELEMENT_TREE);
        }

        private void ShowElementTree()
        {
            if (_recorder == null)
                return;

            var element = _recorder.GetCurrentHoveredElement();
            using (var form = new ElementTreeForm(_recorder.Automation, _recorder.TargetProcessId, element))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.SelectedElement != null)
                    _recorder.SetSelectedElement(form.SelectedElement);
            }
        }

        public void ShowInLowerRightCorner()
        {
            Show();
            LocateInLowerRightCorner();
        }

        private void LocateInLowerRightCorner()
        {
            var mousePos = Cursor.Position;
            var screen = Screen.FromPoint(mousePos);
            var bounds = screen.WorkingArea;
            this.Location = new Point(bounds.Right - (this.Width + 5), bounds.Bottom - (this.Height + 5));
        }

        public void Record()
        {
            _recordingStart = DateTime.Now;
            _recorder.Record();
            _statusTimer.Start();
            this.toolTip1.SetToolTip(this.btnPausePlay, "Pause the recording (Ctrl+Shift+Alt+P)");
            btnPausePlay.ImageIndex = 0;
            UpdateTitle();
            UpdateStatusLabel();
        }

        public void Pause()
        {
            _recorder.Pause();
            _statusTimer.Stop();
            this.toolTip1.SetToolTip(this.btnPausePlay, "Resume recording (Ctrl+Shift+Alt+P)");
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
            {
                if (status.Contains("hook may not be active"))
                {
                    lblLastElement.ForeColor = Color.DarkRed;
                    btnRestartHooks.Visible = true;
                }
                lblLastElement.Text = status;
            }
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

        private void btnRestartHooks_Click(object sender, EventArgs e)
        {
            _recorder.RestartHooks();
            lblLastElement.ForeColor = SystemColors.ControlText;
        }
    }
}
