using System;
using System.Drawing;
using System.Windows.Forms;
using FlaUIRecorder.Internal;

namespace FlaUIRecorder
{
    public partial class CodeForm : Form
    {
        private RecordSession _session;
        private Button _btnSave;
        private Button _btnCopy;

        public CodeForm()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(245, 245, 245);
            SetupButtons();
        }

        private void SetupButtons()
        {
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            _btnSave = new Button { Text = "Save Changes", Width = 110, Location = new Point(10, 8) };
            _btnCopy = new Button { Text = "Copy to Clipboard", Width = 130, Location = new Point(130, 8) };
            _btnSave.Click += BtnSave_Click;
            _btnCopy.Click += BtnCopy_Click;
            panel.Controls.Add(_btnSave);
            panel.Controls.Add(_btnCopy);
            Controls.Add(panel);
        }

        internal void Init(RecordSession recordSession)
        {
            _session = recordSession;
            txtCode.Text = recordSession.Code;
            txtCode.ReadOnly = false;
            Text = "Session " + recordSession.StartTime.ToString("g");
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_session != null)
            {
                _session.Code = txtCode.Text;
                MessageBox.Show(this, "Code saved to session.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCode.Text))
            {
                Clipboard.SetText(txtCode.Text);
                MessageBox.Show(this, "Code copied to clipboard.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
