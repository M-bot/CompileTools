using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompileTools.GUI
{
    public partial class PalettePanel : Panel
    {
        private List<Color> colors = new List<Color>();

        public void Add(Color c)
        {
            colors.Add(c);
        }

        public PalettePanel()
        {
            InitializeComponent();
            Random r = new Random();
            for(int x = 0; x < 256; x++)
            {
                colors.Add(Color.FromArgb(0xFF, r.Next(0xFF), r.Next(0xFF), r.Next(0xFF)));
            }
        }

        public PalettePanel(IContainer container)
        {
            container.Add(this);
            Random r = new Random();
            for (int x = 0; x < 256; x++)
            {
                colors.Add(Color.FromArgb(0xFF, r.Next(0xFF), r.Next(0xFF), r.Next(0xFF)));
            }
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle r = new Rectangle(new Point(ClientRectangle.X, ClientRectangle.Y), new Size(ClientRectangle.Width / 16, ClientRectangle.Height / 16));
            for(int x = 0; x < 256; x++)
            {
                if( x < colors.Count)
                {
                    e.Graphics.FillRectangle(new SolidBrush(colors[x]), r);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Red, r);
                }
                r.X += ClientRectangle.Width / 16;
                if(r.X >= ClientRectangle.Width * 15 / 16)
                {
                    r.X = 0;
                    r.Y += ClientRectangle.Height / 16;
                }
            }
        }
    }
}
