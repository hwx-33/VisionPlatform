using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Timer
{
    public partial class DrawTools: UserControl
    {
        public const int LINETOOLS = 1;
        public const int CIRCLETOOLS = 2;
        public const int RECTTOOLS = 3;
        public const int ERASOR = 4;

        int choose_flag = 0;
        int tools;

        Graphics g = img_box.CreateGraphics();
        Pen pen = new Pen(Color.Blue, 3);
        public DrawTools()
        {
            InitializeComponent();
        }

        private void 直线工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //改变鼠标样式为十字
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            choose_flag = 1;
            tools = LINETOOLS;
        }

        private void 矩形工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        public void  DrawLine(MouseEventArgs e)
        {
            Point pstart = e.Location;

        }
        public void DrawCircle()
        {

        }

        public void DrawRect()
        {

        }

        public void DrawPolygon()
        {

        }

        public void Erasor()
        {

        }
        private void img_box_MouseDown(object sender, MouseEventArgs e)
        {
            if (choose_flag != 0)
            {
                switch (tools)
                {
                    case LINETOOLS:
                        DrawLine(e);
                        break;

                    case CIRCLETOOLS:
                        DrawCircle();
                        break;

                    case RECTTOOLS:
                        DrawRect();
                        break;

                    default:
                        MessageBox.Show("错误工具类");
                        break;
                }
            }
        }

        private void img_box_Paint(object sender, PaintEventArgs e)
        {

        }

        private void img_box_MouseUp(object sender, MouseEventArgs e)
        {
            g.DrawLine(pen, pstart, pend);
        }
    }
}
