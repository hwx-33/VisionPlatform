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
                Graphics graphics = CreateGraphics();
                graphics.DrawLine(new Pen(Color.Black, 1), startPoint, e.Location);

                //记录终点
                //endPoint = e.Location;
                //Refresh();

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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void GetCross(double center_x,double center_y)
        {
            double POINT1_X = 1;
            double POINT1_Y = 2;
            double POINT2_X = 3;
            double POINT2_Y = 4;
            double POINT3_X = 5;
            double POINT3_Y = 6;
            double POINT4_X = 7;
            double POINT4_Y = 8;
            double a = (POINT2_X - POINT1_X) * (center_y - POINT1_Y) - (center_x - POINT1_X) * (POINT2_Y - POINT1_Y);
            double b = (POINT4_X - POINT3_X) * (center_y - POINT3_Y) - (center_x - POINT3_X) * (POINT4_Y - POINT3_Y);
            double c = (POINT3_X - POINT2_X) * (center_y - POINT2_Y) - (center_x - POINT2_X) * (POINT3_Y - POINT2_Y);
            double d = (POINT1_X - POINT4_X) * (center_y - POINT4_Y) - (center_x - POINT4_X) * (POINT1_Y - POINT4_Y);
            if(a*b >= 0 && c*d >=0)
            {
                //OK
            }
            else
            {
                //NG
            }
        }

    }
}
