using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrussCalculatorApp
{
    class TrussJoint
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        // Сила прикладена до стержня
        public Force Force { get; set; }
        public double Angle
        {
            get { return Force.Angle; }
        }
        // Позначає чи обрахований стержень
        public bool Calculated { get; set; }
        // Конструктори
        public TrussJoint(Point start, Point end)
            :this(start, end, new Force(new Vector2(start, end), 0))
        {
        }
        public TrussJoint(Point start, Point end, Force force)
        {
            Start = start;
            End = end;
            Force = force;
            Calculated = false;
        }
        // Міняє початок і кінець стержня місцями
        public void SwapPoints()
        {
            Point t = Start;
            Start = End;
            End = t;
            Force.InverseDirection();
        }
    }
}
