namespace Ruler
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class Ruler : Panel
    {
        private IContainer components = null;

        public Ruler()
        {
            this.InitializeComponent();
            this.isVertical = false;
            this.pixelsPerUnit = 50f;
            base.BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            int num;
            int num4;
            PointF tf;
            Point point;
            Point point2;
            base.OnPaint(pe);
            Graphics graphics = pe.Graphics;
            Pen pen = new Pen(Color.Black, 1f);
            Font font = new Font("Arial", 7f);
            if (!this.isVertical)
            {
                num = 1;
                float num2 = (this.pixelsPerUnit / 10f) - 1f;
                while (num2 < base.Width)
                {
                    int height;
                    if ((num % 10) == 0)
                    {
                        height = base.Height;
                        num4 = num / 10;
                        tf = new PointF();
                        if (num < 100)
                        {
                            tf.X = num2 - 8f;
                        }
                        else
                        {
                            tf.X = num2 - 12f;
                        }
                        tf.Y = base.Height / 3;
                        graphics.DrawString(num4.ToString(), font, new SolidBrush(Color.Black), tf);
                    }
                    else if ((num % 5) == 0)
                    {
                        height = base.Height / 3;
                    }
                    else if (this.pixelsPerUnit >= 25f)
                    {
                        height = base.Height / 5;
                    }
                    else
                    {
                        height = 0;
                    }
                    point = new Point();
                    point2 = new Point();
                    point.X = Convert.ToInt32(num2);
                    point2.X = Convert.ToInt32(num2);
                    point.Y = 0;
                    point2.Y = height;
                    graphics.DrawLine(pen, point, point2);
                    num2 += float.Parse(this.pixelsPerUnit.ToString()) / 10f;
                    num++;
                }
            }
            else
            {
                num = 0;
                float num5 = base.Height - 2;
                while (num5 > 0f)
                {
                    int width;
                    if ((num % 10) == 0)
                    {
                        width = 0;
                        num4 = num / 10;
                        tf = new PointF {
                            Y = num5 + 1f,
                            X = 1f
                        };
                        graphics.DrawString(num4.ToString(), font, new SolidBrush(Color.Black), tf);
                    }
                    else if ((num % 5) == 0)
                    {
                        width = ((base.Width * 2) / 3) - 1;
                    }
                    else if (this.pixelsPerUnit >= 25f)
                    {
                        width = ((base.Width * 4) / 5) - 1;
                    }
                    else
                    {
                        width = base.Width;
                    }
                    point = new Point();
                    point2 = new Point();
                    point.X = width;
                    point2.X = base.Width;
                    point.Y = Convert.ToInt32(num5);
                    point2.Y = Convert.ToInt32(num5);
                    graphics.DrawLine(pen, point, point2);
                    num5 -= float.Parse(this.pixelsPerUnit.ToString()) / 10f;
                    num++;
                }
            }
        }

        [Browsable(true)]
        public bool isVertical { get; set; }

        [Browsable(true)]
        public float pixelsPerUnit { get; set; }
    }
}

