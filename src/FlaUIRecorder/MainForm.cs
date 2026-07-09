using FlaUI.Core;
using FlaUIRecorder.CodeProvider.Common;
using FlaUIRecorder.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlaUIRecorder
{
    public partial class MainForm : Form
    {
        private CodeProviderFactory _providerFactory;
        private RecorderForm _recorderForm;
        private RecorderProject _currentProject;
        private AssemblyInformationalVersionAttribute _assemblyInformationalVersionAttribute;
        private MRUManager _mruManager;
        private Process _targetProcesStartedByRecorder;
        private RecordingAutosave _autosave;

        public MainForm()
        {
            InitializeComponent();

            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            ConfigureDpiAwareGroupBoxes();
            WireMutuallyExclusiveTargetRadioButtons();

            _assemblyInformationalVersionAttribute = Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
        }

        private void ConfigureDpiAwareGroupBoxes()
        {
            foreach (var groupBox in new[] { groupBox1, groupBox2, groupBox3 })
                groupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            argsRowTableLayout.ColumnStyles[0] = new ColumnStyle(SizeType.AutoSize);
        }

        // rdbApplicationStart ("Path:") and rdbApplicationProcess ("Process:") live in different
        // TableLayoutPanel containers, so WinForms does not auto-group them. Wire mutual exclusion
        // explicitly so only one target-application source can be active at a time.
        private void WireMutuallyExclusiveTargetRadioButtons()
        {
            rdbApplicationStart.CheckedChanged += (s, e) =>
            {
                if (rdbApplicationStart.Checked && rdbApplicationProcess.Checked)
                    rdbApplicationProcess.Checked = false;
            };
            rdbApplicationProcess.CheckedChanged += (s, e) =>
            {
                if (rdbApplicationProcess.Checked && rdbApplicationStart.Checked)
                    rdbApplicationStart.Checked = false;
            };
        }

        private AutomationType GetAutomationType()
        {
            return rdbVersionUIA2.Checked ? AutomationType.UIA2 : AutomationType.UIA3;
        }

        internal void CancelRecording()
        {
            _autosave?.Stop();
            CloseRecorderAndShowMainForm();
            _currentProject.Sessions.Remove(_currentProject.Sessions.Last());
            RecordingAutosave.DeleteAutosave(_currentProject.FileName);
        }

        internal void Finished(string code)
        {
            _autosave?.Stop();
            CloseRecorderAndShowMainForm();
            var recordSession = _currentProject.Sessions.Last();
            recordSession.Code = code;
            if (_recorderForm != null && _recorderForm.Actions.Count > 0)
            {
                recordSession.Actions.AddRange(_recorderForm.Actions);
            }
            _currentProject.IsDirty = true;
            RecordingAutosave.DeleteAutosave(_currentProject.FileName);

            recorderProjectBindingSource.EndEdit();

            if (_targetProcesStartedByRecorder != null)
                _targetProcesStartedByRecorder.CloseMainWindow();

            ShowRecordSession(recordSession);
        }

        private void ShowRecordSession(RecordSession recordSession)
        {
            using (var form = new CodeForm())
            {
                form.Init(recordSession);
                form.ShowDialog(this);
            }
        }

        private void CloseRecorderAndShowMainForm()
        {
            _recorderForm.Close();
            _recorderForm = null;
            WindowState = FormWindowState.Normal;
        }

        private void RefreshProcessList()
        {
            cboApplicationProcess.DataSource = Process.GetProcesses()
                .OrderBy(p => p.ProcessName)
                .ThenBy(p => p.Id)
                .ToArray();
            cboApplicationProcess.ValueMember = "Id";
        }

        private string GetExecutableByProcess(Process process)
        {
            string result = "";
            try
            {
                string Query = "SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = " + process.Id;

                using (ManagementObjectSearcher mos = new ManagementObjectSearcher(Query))
                {
                    using (ManagementObjectCollection moc = mos.Get())
                    {
                        result = (from mo in moc.Cast<ManagementObject>() select mo["ExecutablePath"]).First().ToString();
                    }
                }
            }
            catch //(Exception ex)
            {
                //ex.HandleException();
            }

            return result;
        }

        private Process StartAndWaitForTargetApplication(string executable, string args = null)
        {
            Process process;

            if (!string.IsNullOrEmpty(args))
                process = Process.Start(executable, args);
            else
                process = Process.Start(executable);

            Thread.Sleep(new TimeSpan(0, 0, 0, 1, 0)); // Wait at least one second to allow proper application start

            TimeSpan timeout = TimeSpan.FromMilliseconds(-1.0);
            FlaUI.Core.Tools.Retry.While(() =>
           {
               process.Refresh();
               // Check if process is still running before accessing MainWindowHandle
               if (process.HasExited)
                   throw new InvalidOperationException("The target application has exited before showing a main window.");
               return process.MainWindowHandle == IntPtr.Zero;
           }, timeout, new TimeSpan?(TimeSpan.FromMilliseconds(50.0)));

            return process;
        }

        private void LoadProject(string fileName)
        {
            try
            {
                _currentProject = ProjectSerializer.Load(fileName);
                LoadControlValuesFromProject();
                _currentProject.FileName = fileName;
                _currentProject.IsDirty = false;
                UpdateTitle();
                CheckAutosaveRecovery(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loading project failed.\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool SaveProject()
        {
            if (string.IsNullOrEmpty(_currentProject.FileName))
            {
                if (saveProjectDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var result = SaveProject(saveProjectDialog.FileName);

                    if (result)
                        _mruManager.AddRecentFile(saveProjectDialog.FileName);

                    return result;
                }
                else
                    return false;
            }
            else
            {
                return SaveProject(_currentProject.FileName);
            }
        }

        private bool SaveProject(string fileName)
        {
            if (cboCodeProvider.SelectedIndex == -1)
            {
                MessageBox.Show("Please choose a code provider.", "Missing Provider", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            if (rdbApplicationStart.Checked && !File.Exists(txtApplicationPath.Text))
            {
                if (String.IsNullOrEmpty(txtApplicationPath.Text))
                    MessageBox.Show("Please choose a target application.", "Missing application file", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    MessageBox.Show($"The selected application ({txtApplicationPath.Text}) does not exist.", "Invalid application file", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return false;
            }

            try
            {
                WriteControlValuesToProject();
                ProjectSerializer.Save(_currentProject, fileName);
                _currentProject.IsDirty = false;
                UpdateTitle();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void LoadControlValuesFromProject()
        {
            if (!String.IsNullOrEmpty(_currentProject.Executable))
                rdbApplicationStart.Checked = true;

            if (!String.IsNullOrEmpty(_currentProject.ProcessName))
            {
                var processes = (IEnumerable<Process>)cboApplicationProcess.DataSource;
                var item = processes.FirstOrDefault(p => p.ProcessName == _currentProject.ProcessName);

                if (item != null)
                {
                    cboApplicationProcess.SelectedItem = item;
                    rdbApplicationProcess.Checked = true;
                }
            }

            if (_currentProject.AutomationType == AutomationType.UIA2)
                rdbVersionUIA2.Checked = true;
            else
                rdbVersionUIA3.Checked = true;

            recorderProjectBindingSource.DataSource = _currentProject;
            //sessionsBindingSource.DataSource = _currentProject.Sessions;
        }

        private void WriteControlValuesToProject()
        {
            recorderProjectBindingSource.EndEdit();

            //_currentProject.Executable = txtApplicationPath.Text;
            _currentProject.AutomationType = GetAutomationType();

            if (cboCodeProvider.SelectedItem != null)
                _currentProject.CodeProvider = cboCodeProvider.SelectedItem.ToString();

            if (rdbApplicationProcess.Checked)
            {
                var process = (Process)cboApplicationProcess.SelectedItem;
                _currentProject.Executable = GetExecutableByProcess(process);
                _currentProject.ProcessName = process.ProcessName;
            }
        }

        public static void BringProcessToFront(Process process)
        {
            // Check if process is still running before accessing MainWindowHandle
            if (process.HasExited)
                throw new InvalidOperationException("Cannot bring an exited process to front.");
                
            IntPtr handle = process.MainWindowHandle;
            if (NativeMethods.IsIconic(handle))
            {
                NativeMethods.ShowWindow(handle, NativeMethods.SW_RESTORE);
            }

            NativeMethods.SetForegroundWindow(handle);
        }

        private void UpdateTitle()
        {
            Text = "FlaUI Recorder";

            if (_assemblyInformationalVersionAttribute != null)
            {
                Text += " v" + _assemblyInformationalVersionAttribute.InformationalVersion;
            }

            if (string.IsNullOrEmpty(_currentProject.FileName))
                Text += " - new";
            else
                Text += " - " + Path.GetFileNameWithoutExtension(_currentProject.FileName);

            if (_currentProject.IsDirty)
                Text += "*";
        }

        private void SetDirtyFlag()
        {
            if (_currentProject == null)
                return;

            _currentProject.IsDirty = true;
            UpdateTitle();
        }

        #region Form events

        private void MainForm_Load(object sender, EventArgs e)
        {
            _providerFactory = new CodeProviderFactory();
            cboCodeProvider.Items.AddRange(_providerFactory.GetAvailableProviderNames());
            if (cboCodeProvider.Items.Count > 0)
                cboCodeProvider.SelectedIndex = 0;

            RefreshProcessList();

            _currentProject = new RecorderProject();
            recorderProjectBindingSource.DataSource = _currentProject;

            // Add Export menu item dynamically
            var mnuExport = new ToolStripMenuItem("Export Project...");
            mnuExport.Click += (s, ev) => ExportProject();

            // Theme menu
            var mnuTheme = new ToolStripMenuItem("Theme");
            var mnuLight = new ToolStripMenuItem("Light");
            mnuLight.Click += (s, ev) => ThemeManager.ApplyTheme(this, AppTheme.Light);
            var mnuDark = new ToolStripMenuItem("Dark");
            mnuDark.Click += (s, ev) => ThemeManager.ApplyTheme(this, AppTheme.Dark);
            var mnuHighContrast = new ToolStripMenuItem("High Contrast");
            mnuHighContrast.Click += (s, ev) => ThemeManager.ApplyTheme(this, AppTheme.HighContrast);
            mnuTheme.DropDownItems.AddRange(new ToolStripItem[] { mnuLight, mnuDark, mnuHighContrast });

            if (menuStrip1.Items.Count > 0 && menuStrip1.Items[0] is ToolStripMenuItem fileMenu)
            {
                fileMenu.DropDownItems.Add(mnuExport);
            }
            if (menuStrip1.Items.Count > 0)
                menuStrip1.Items.Add(mnuTheme);
            _currentProject.IsDirty = false;
            UpdateTitle();
            InitializeMru();

            statusStrip1.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            RecorderErrorLog.ErrorRecorded += (s, args) => UpdateErrorStatus();
            UpdateErrorStatus();
        }

        private void UpdateErrorStatus()
        {
            if (statusLabelErrors == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateErrorStatus));
                return;
            }

            statusLabelErrors.Text = $"Errors: {RecorderErrorLog.Count}";
        }

        private void ErrorStatusLabel_Click(object sender, EventArgs e)
        {
            if (RecorderErrorLog.Count == 0)
            {
                MessageBox.Show(this, "No errors recorded.", "Error Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show(this, string.Join(Environment.NewLine, RecorderErrorLog.Errors), "Error Log", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void InitializeMru()
        {
            _mruManager = new MRUManager(mnuRecentProjects, "FlaUI Recorder", recentFileGotClicked_handler, null);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SaveProject())
                    return;

                var session = new RecordSession() { StartTime = DateTime.Now };
                _currentProject.Sessions.Add(session);
                Process process = null;

                if (rdbApplicationStart.Checked)
                {
                    process = StartAndWaitForTargetApplication(_currentProject.Executable, _currentProject.Arguments);
                    _targetProcesStartedByRecorder = process;
                }
                else
                {
                    process = (Process)cboApplicationProcess.SelectedItem;
                    _targetProcesStartedByRecorder = null;
                }

                BringProcessToFront(process);

                _recorderForm = new RecorderForm();
                _recorderForm.Initialize(_currentProject.AutomationType, _providerFactory, cboCodeProvider.SelectedItem.ToString(), this, process);
                _recorderForm.Record();
                _recorderForm.ShowInLowerRightCorner();
                this.WindowState = FormWindowState.Minimized;

                _autosave?.Dispose();
                _autosave = new RecordingAutosave();
                _autosave.Start(_currentProject, () => _recorderForm.Actions);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Starting recording failed.\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Remove the just added session since recording failed
                if (_currentProject.Sessions.Count > 0)
                    _currentProject.Sessions.Remove(_currentProject.Sessions.Last());
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnApplicationBrowse_Click(object sender, EventArgs e)
        {
            if (openApplicationDialog.ShowDialog(this) == DialogResult.OK)
                txtApplicationPath.Text = openApplicationDialog.FileName;
        }

        private void btnProcessRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void cboApplicationProcess_Format(object sender, ListControlConvertEventArgs e)
        {
            Process item = e.ListItem as Process;
            string title = string.Empty;
            try { title = item.MainWindowTitle; } catch { }
            if (string.IsNullOrEmpty(title))
                title = "(no title)";
            e.Value = $"{item.ProcessName} ({item.Id}) - {title}";
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsureCurrentProjectIsSaved())
                return;

            if (openProjectDialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadProject(openProjectDialog.FileName);
                _mruManager.AddRecentFile(openProjectDialog.FileName);
            }
        }

        private bool EnsureCurrentProjectIsSaved()
        {
            if (_currentProject.IsDirty)
            {
                var dialogResult = MessageBox.Show("The current project contains unsaved changes. Do you want to save these changes?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    if (!SaveProject())
                        return false;
                }
                else if (dialogResult == DialogResult.Cancel)
                    return false;
            }

            return true;
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            if (saveProjectDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (SaveProject(saveProjectDialog.FileName))
                    _mruManager.AddRecentFile(saveProjectDialog.FileName);
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void recentFileGotClicked_handler(object obj, EventArgs evt)
        {
            LoadProject(obj.ToString());
        }

        private void recorderProjectBindingSource_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            if (!_currentProject.IsDirty && (e.BindingCompleteState == BindingCompleteState.Success) && e.Binding.Control.Focused)
            {
                SetDirtyFlag();
            }
        }

        private void radioButton_CheckedChanged_UpdateDirty(object sender, EventArgs e)
        {
            SetDirtyFlag();
        }

        private void recorderProjectBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            SetDirtyFlag();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !EnsureCurrentProjectIsSaved())
                e.Cancel = true;
        }        

        private void lstSessions_DoubleClick(object sender, EventArgs e)
        {
            if (lstSessions.SelectedItem != null)
            {
                var session = (RecordSession)lstSessions.SelectedItem;
                if (session.Actions != null && session.Actions.Count > 0)
                {
                    var choice = MessageBox.Show("View structured actions (Yes) or raw generated code (No)?", "View Session", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (choice == DialogResult.Yes)
                    {
                        using (var form = new ActionListForm())
                        {
                            form.Init(session);
                            form.ShowDialog(this);
                        }
                        return;
                    }
                    else if (choice == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                ShowRecordSession(session);
            }
        }

        private void ExportProject()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select folder for exported project";
                if (folderDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                int totalActions = _currentProject.Sessions?.Sum(s => s.Actions?.Count ?? 0) ?? 0;
                ExportOptions options;
                using (var optionsDialog = new ExportProjectDialog(totalActions))
                {
                    if (optionsDialog.ShowDialog(this) != DialogResult.OK)
                        return;
                    options = optionsDialog.Options;
                }

                string projectName = string.IsNullOrEmpty(_currentProject.FileName) ? "RecordedAutomation" : Path.GetFileNameWithoutExtension(_currentProject.FileName);
                string exportDir = Path.Combine(folderDialog.SelectedPath, projectName);

                try
                {
                    _currentProject.CodeProvider = cboCodeProvider.SelectedItem?.ToString();
                    ProjectExporter.Export(_currentProject, exportDir, projectName, options);

                    var result = MessageBox.Show(
                        $"Exported project to {exportDir}\r\n\r\nOpen folder in Explorer?",
                        "Export Complete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = exportDir,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Export failed.\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CheckAutosaveRecovery(string fileName)
        {
            if (RecordingAutosave.TryLoadAutosave(fileName, out var autosaved))
            {
                var result = MessageBox.Show(this,
                    "An autosave file was found from a previous recording session. Restore it?",
                    "Recover Autosave",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _currentProject = autosaved;
                    _currentProject.FileName = fileName;
                    LoadControlValuesFromProject();
                    _currentProject.IsDirty = true;
                    UpdateTitle();
                }
                else
                {
                    RecordingAutosave.DeleteAutosave(fileName);
                }
            }
        }

        #endregion
    }
}
