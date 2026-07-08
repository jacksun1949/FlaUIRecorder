using System;
using System.Windows.Forms;
using FlaUIRecorder.Internal;
using System.Linq;

namespace FlaUIRecorder
{
    public class ActionListForm : Form
    {
        private ListView lstActions;
        private Button btnDelete;
        private Button btnEditComment;
        private RecordSession _session;

        public ActionListForm()
        {
            InitializeComponent();
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        }

        private void InitializeComponent()
        {
            this.Text = "Structured Actions";
            this.Size = new System.Drawing.Size(700, 420);
            lstActions = new ListView { Dock = DockStyle.Top, Height = 320, View = View.Details, FullRowSelect = true, GridLines = true };
            lstActions.Columns.Add("Time", 90);
            lstActions.Columns.Add("Type", 90);
            lstActions.Columns.Add("Target", 280);
            lstActions.Columns.Add("Comment", 200);
            btnDelete = new Button { Text = "Delete Selected", Dock = DockStyle.Left, Width = 120 };
            btnEditComment = new Button { Text = "Edit Comment", Dock = DockStyle.Left, Width = 120 };
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnEditComment);
            this.Controls.Add(lstActions);
            this.Controls.Add(panel);
            btnDelete.Click += BtnDelete_Click;
            btnEditComment.Click += BtnEditComment_Click;
        }

        public void Init(RecordSession session)
        {
            _session = session;
            RefreshList();
        }

        private void RefreshList()
        {
            lstActions.Items.Clear();
            if (_session?.Actions != null)
            {
                foreach (var a in _session.Actions)
                {
                    string target = !string.IsNullOrEmpty(a.AutomationId) ? a.AutomationId : a.Name;
                    string comment = string.IsNullOrEmpty(a.Comment) ? "" : a.Comment;
                    var item = new ListViewItem(new[] { a.Timestamp.ToString("HH:mm:ss"), a.Type.ToString(), target, comment });
                    lstActions.Items.Add(item);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstActions.SelectedIndices.Count > 0 && _session != null)
            {
                int idx = lstActions.SelectedIndices[0];
                _session.Actions.RemoveAt(idx);
                RefreshList();
            }
        }

        private void BtnEditComment_Click(object sender, EventArgs e)
        {
            if (lstActions.SelectedIndices.Count > 0 && _session != null)
            {
                int idx = lstActions.SelectedIndices[0];
                var action = _session.Actions[idx];
                using (var input = new CommentForm())
                {
                    if (input.ShowDialog(this) == DialogResult.OK)
                    {
                        action.Comment = input.Comment;
                        RefreshList();
                    }
                }
            }
        }
    }
}
