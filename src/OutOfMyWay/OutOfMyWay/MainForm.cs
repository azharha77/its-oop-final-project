using System;
using System.Drawing;
using System.Windows.Forms;

namespace OutOfMyWay
{
    public class MainForm : Form
    {
        public MainForm()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Main Menu";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}