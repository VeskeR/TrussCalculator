using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrussCalculatorApp
{
    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }


        public Point()
            :this(0, 0)
        {
        }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }


        public override bool Equals(object obj)
        {
            return ToString() == obj.ToString();
        }
        public override string ToString()
        {
            return string.Format("X = {0}; Y = {1}", X, Y);
        }


        public static implicit operator Point(System.Windows.Point p)
        {
            Point point = new Point();
            point.X = p.X;
            point.Y = p.Y;
            return point;
        }

        public static implicit operator System.Windows.Point(Point p)
        {
            return new System.Windows.Point(p.X, p.Y);
        }
    }
}
