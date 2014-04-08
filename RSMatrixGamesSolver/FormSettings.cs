using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RSMatrixGamesSolver
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            row = (int)numericUpDown1.Value;
            col = (int)numericUpDown2.Value;
            this.Close();
        }

        public int Row
        {
            get { return row; }
        }
        public int Column
        {
            get { return col; }
        }

    }
}
