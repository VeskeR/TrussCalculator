using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrussCalculatorApp
{
    class TrussNode
    {
        public Point Point { get; private set; }
        public List<Force> Forces { get; set; }
        public List<TrussJoint> Joints { get; set; }


        public TrussNode(double x, double y)
            :this(new Point(x, y))
        {
        }
        public TrussNode(Point p)
        {
            Point = p;
            Forces = new List<Force>();
            Joints = new List<TrussJoint>();
        }
    }
}
