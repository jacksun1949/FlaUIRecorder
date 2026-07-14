namespace FlaUIRecorder
{
    partial class CommentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            txtComment = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(116, 63);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(125, 42);
            btnOk.TabIndex = 1;
            btnOk.Text = "&OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(257, 63);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(115, 42);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "&Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtComment
            // 
            txtComment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtComment.Location = new System.Drawing.Point(12, 12);
            txtComment.Name = "txtComment";
            txtComment.Size = new System.Drawing.Size(360, 39);
            txtComment.TabIndex = 0;
            txtComment.KeyUp += txtComment_KeyUp;
            // 
            // CommentForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(384, 117);
            ControlBox = false;
            Controls.Add(txtComment);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CommentForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Add comment";
            Load += CommentForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtComment;
    }
}