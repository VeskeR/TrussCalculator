using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TrussCalculatorApp
{
    class Vector2 : Point
    {
        // Повертає довижну вектора
        public double Magnitude
        {
            get { return Math.Sqrt(SqrMagnitude); }
        }
        // Повертає квадрат довжини вектора
        public double SqrMagnitude
        {
            get { return X*X + Y*Y; }
        }
        // Повертає новий нормалізований вектор, що вказує в тому ж напрямі, що і даний
        public Vector2 Normalized
        {
            get
            {
                double m = Magnitude;
                double x = X/m;
                double y = Y/m;
                return new Vector2(x, y);
            }
        }
        // Повертає кут нахилу вектора до осі Ох
        public double OxAngle
        {
            get { return Math.Atan2(Y, X); }
        }
        // Повертає кут нахилу вектора до осі Оу
        public double OyAngle
        {
            get { return Math.Atan2(X, Y); }
        }
        // Конструктори
        public Vector2()
        {
        }
        public Vector2(double x, double y)
            :base(x, y)
        {
        }
        public Vector2(Point start, Point end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }
        // Нормалізує даний вектор
        public void Normalize()
        {
            double m = Magnitude;
            double x = X/m;
            double y = Y/m;
            X = x;
            Y = y;
        }
    }
}
