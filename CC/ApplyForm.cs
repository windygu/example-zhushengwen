using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CCWin
{
    public partial class ApplyForm : Form
    {
        private bool mousePressed = false;
        private Point diff = new Point(0, 0);
        
        public ApplyForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        private void ApplyForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousePressed = true;

                Point p = new Point(e.X, e.Y);
                p = PointToScreen(p);
                diff.X = p.X - DesktopLocation.X;
                diff.Y = p.Y - DesktopLocation.Y;
            }
        }

        private void ApplyForm_MouseUp(object sender, MouseEventArgs e)
        {
            mousePressed = false;
        }

        private void ApplyForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePressed && (e.Button & MouseButtons.Left) != 0)
            {
                Point p = new Point(e.X, e.Y);
                p = PointToScreen(p);
                p.X -= diff.X;
                p.Y -= diff.Y;
                DesktopLocation = p;
            }
        }
    }
}
