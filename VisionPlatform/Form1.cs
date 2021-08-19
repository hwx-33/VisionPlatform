using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionPlatform
{
    public partial class Form1 : Form
    {
        private Point startPoint;   //直线起点
        private Point endPoint;    //直线终点
        private bool isDown;        //鼠标按下标志
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = new Point(e.X, e.Y);
                isDown = true;
            }
        }
        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            //绘制曲线
            e.Graphics.DrawLine(new Pen(Color.Black, 1), startPoint, endPoint);
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                //绘制曲线
                //Graphics graphics = CreateGraphics();
                //graphics.DrawLine(new Pen(Color.Black, 1), startPoint, e.Location);

                //记录终点
                endPoint = e.Location;
                Refresh();

            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
        }

        public Form1()
        {
            InitializeComponent();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
