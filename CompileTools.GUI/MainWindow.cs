using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompileTools.GUI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            List<HeroButton> sidebarButtons = new List<HeroButton>();
            sidebarButtons.Add(homeButton);
            sidebarButtons.Add(imageViewerButton);
            imageViewerButton.Selected = true;
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            
        }

        private void imageViewerButton_Click(object sender, EventArgs e)
        {

        }
    }
}