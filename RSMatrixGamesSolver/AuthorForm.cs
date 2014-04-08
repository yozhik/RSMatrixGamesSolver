using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RSMatrixGamesSolver
{
    public partial class AuthorForm : Form
    {
        public AuthorForm()
        {
            InitializeComponent();
        }

        private void AuthorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
