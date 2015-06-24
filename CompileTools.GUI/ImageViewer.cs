using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace CompileTools.GUI
{
    public partial class ImageViewer : UserControl
    {
        protected bool validData;
        string path;
        protected Image image;
        protected Thread getImageThread;

        public ImageViewer()
        {
            InitializeComponent();
            inputPictureBox.AllowDrop = true;
        }

        private void inputPictureBox_DragDrop(object sender, DragEventArgs e)
        {
            if (validData)
            {
                while (getImageThread.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                }
                inputPictureBox.Image = image;
            }
        }

        private void inputPictureBox_DragEnter(object sender, DragEventArgs e)
        {
            string filename;
            validData = GetFilename(out filename, e);
            if (validData)
            {
                path = filename;
                getImageThread = new Thread(new ThreadStart(LoadImage));
                getImageThread.Start();
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void inputPictureBox_DragLeave(object sender, EventArgs e)
        {

        }

        private void inputPictureBox_DragOver(object sender, DragEventArgs e)
        {

        }

        protected void LoadImage()
        {
            image = new Bitmap(path);
        }

        private bool GetFilename(out string filename, DragEventArgs e)
        {
            bool ret = false;
            filename = String.Empty;
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                Array data = ((IDataObject)e.Data).GetData("FileDrop") as Array;
                if (data != null)
                {
                    if ((data.Length == 1) && (data.GetValue(0) is String))
                    {
                        filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                       {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }
    }
}
