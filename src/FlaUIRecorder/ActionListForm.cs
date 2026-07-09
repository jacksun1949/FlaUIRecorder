using System;
using System.Windows.Forms;
using FlaUIRecorder.Internal;
using System.Linq;
using System.Drawing;

namespace FlaUIRecorder
{
    public class ActionListForm : Form
    {
        private ListView lstActions;
        private Button btnDelete;
        private Button btnEditComment;
        private RecordSession _session;
        private ContextMenuStrip _contextMenu;
        private int _dragIndex = -1;

        public ActionListForm()
        {
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            InitializeComponent();
            this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void InitializeComponent()
        {
            this.Text = "Structured Actions";
            this.Size = new Size(700, 420);
            lstActions = new ListView
            {
                Dock = DockStyle.Top,
                Height = 320,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                AllowDrop = true
            };
            lstActions.Columns.Add("Time", 90);
            lstActions.Columns.Add("Type", 90);
            lstActions.Columns.Add("Target", 280);
            lstActions.Columns.Add("Comment", 200);
            lstActions.ItemDrag += LstActions_ItemDrag;
            lstActions.DragEnter += LstActions_DragEnter;
            lstActions.DragOver += LstActions_DragOver;
            lstActions.DragDrop += LstActions_DragDrop;

            _contextMenu = new ContextMenuStrip();
            var insertWait = new ToolStripMenuItem("Insert Wait Before");
            insertWait.Click += InsertWait_Click;
            var insertComment = new ToolStripMenuItem("Insert Comment Before");
            insertComment.Click += InsertComment_Click;
            var deleteItem = new ToolStripMenuItem("Delete");
            deleteItem.Click += (s, e) => BtnDelete_Click(s, e);
            _contextMenu.Items.AddRange(new ToolStripItem[] { insertWait, insertComment, deleteItem });
            lstActions.ContextMenuStrip = _contextMenu;

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

        private void LstActions_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _dragIndex = lstActions.SelectedIndices.Count > 0 ? lstActions.SelectedIndices[0] : -1;
            if (_dragIndex >= 0)
                lstActions.DoDragDrop(_dragIndex, DragDropEffects.Move);
        }

        private void LstActions_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void LstActions_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void LstActions_DragDrop(object sender, DragEventArgs e)
        {
            if (_session == null || _dragIndex < 0)
                return;

            var point = lstActions.PointToClient(new Point(e.X, e.Y));
            var targetItem = lstActions.GetItemAt(point.X, point.Y);
            if (targetItem == null)
                return;

            int targetIndex = targetItem.Index;
            if (targetIndex == _dragIndex)
                return;

            var action = _session.Actions[_dragIndex];
            _session.Actions.RemoveAt(_dragIndex);
            _session.Actions.Insert(targetIndex, action);
            RefreshList();
            lstActions.Items[targetIndex].Selected = true;
            _dragIndex = -1;
        }

        private void InsertWait_Click(object sender, EventArgs e)
        {
            InsertActionBefore(new RecordedAction { Type = ActionType.Wait, WaitDurationMs = 500, Timestamp = DateTime.Now });
        }

        private void InsertComment_Click(object sender, EventArgs e)
        {
            using (var form = new CommentForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    InsertActionBefore(new RecordedAction { Type = ActionType.Comment, Comment = form.Comment, Timestamp = DateTime.Now });
            }
        }

        private void InsertActionBefore(RecordedAction action)
        {
            if (lstActions.SelectedIndices.Count == 0 || _session == null)
                return;

            int idx = lstActions.SelectedIndices[0];
            _session.Actions.Insert(idx, action);
            RefreshList();
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
