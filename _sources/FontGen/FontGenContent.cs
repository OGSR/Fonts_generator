using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FontGen
{
    public partial class FontGenContent : Form
    {
        public FontGenContent()
        {
            InitializeComponent();
        }

        private void FontGenContent_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (Tag != null)
            {
                ((FontGenForm)Tag).SetContent(txtContent.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            }
        }

        public void SetText(string[] lines)
        {
            txtContent.Text = string.Join(Environment.NewLine, lines);
        }
    }
}
