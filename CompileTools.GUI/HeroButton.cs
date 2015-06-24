using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompileTools.GUI
{
    public class HeroButton : Button
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public HeroButton()
            : base()
        {
            FlatAppearance.BorderSize = 0;
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            FlatStyle = System.Windows.Forms.FlatStyle.Standard;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            if (selected)
            {
                Color a = Color.FromArgb(50, 0x64, 0x95, 0xED);
                pevent.Graphics.FillRectangle(new SolidBrush(a), this.ClientRectangle);
                ControlPaint.DrawBorder(pevent.Graphics, this.ClientRectangle,
                Color.CornflowerBlue, 1, ButtonBorderStyle.Solid,
                Color.CornflowerBlue, 1, ButtonBorderStyle.Solid,
                Color.CornflowerBlue, 1, ButtonBorderStyle.Solid,
                Color.CornflowerBlue, 1, ButtonBorderStyle.Solid);
            }
        }
    }
}
