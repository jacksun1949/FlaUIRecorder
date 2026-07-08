using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlaUIRecorder
{
    public partial class CommentForm : Form
    {
        public CommentForm()
        {
            InitializeComponent();
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        }

        private void CommentForm_Load(object sender, EventArgs e)
        {
            txtComment.Focus();
        }

        public string Comment { get => txtComment.Text; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void txtComment_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                this.DialogResult = DialogResult.OK;
        }
    }
}
