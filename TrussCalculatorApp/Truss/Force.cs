using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrussCalculatorApp
{
    class Force
    {
        public Vector2 Direction { get; set; }
        public double Magnitude { get; set; }
        public double Angle
        {
            get { return Direction.OxAngle; }
        }
        public Force(Vector2 v, double magnitude)
        {
            Direction = v;
            Magnitude = magnitude;
        }
        // Змінює напрям дії даної сили
        public void InverseDirection()
        {
            Direction = new Vector2(-Direction.X, -Direction.Y);
        }
    }
}
