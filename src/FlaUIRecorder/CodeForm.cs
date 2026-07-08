using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlaUIRecorder.Internal;

namespace FlaUIRecorder
{
    public partial class CodeForm : Form
    {
        public CodeForm()
        {
            InitializeComponent();
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        }
        
        internal void Init(RecordSession recordSession)
        {
            txtCode.Text = recordSession.Code;
            Text = "Session " + recordSession.StartTime.ToString("g");
        }
    }
}
