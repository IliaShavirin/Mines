using System;
using System.Windows.Forms;
using Mines.Controllers;

namespace Mines
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MapController.Init(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}