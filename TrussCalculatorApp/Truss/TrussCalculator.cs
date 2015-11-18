using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrussCalculatorApp
{
    class TrussCalculator
    {
        // Приймає список вузлів ферми, що необхідно обрахувати. Обраховані навантаження записуються прямо у переданий параметр, тому метод нічого не повертає
        public static void CalculateTruss(IEnumerable<TrussNode> truss)
        {
            // Сортування вузлів ферми за абсцисою
            var sortedTruss = truss.OrderBy(node => node.Point.X).ToList();
            // Отримання списку всіх стержнів ферми
            var joints = sortedTruss.SelectMany(n => n.Joints).Distinct();
            // Обрахування ферми за методом видалення вузлів, поки не залишиться необрахованих стержнів
            while (joints.Any(j => !j.Calculated))
            {
                // Вибір поточного вузла для видалення за критерієм наявності менше або рівно двох необрахованих стержнів
                TrussNode nodeToCalculate = sortedTruss.First(n => n.Joints.Count(j => !j.Calculated) <= 2);
                CalculateTrussNode(nodeToCalculate);
                sortedTruss.Remove(nodeToCalculate);
            }
        }
        // Метод обраховує ферму
        private static void CalculateTrussNode(TrussNode node)
        {
            foreach (TrussJoint j in node.Joints.Where(j => !j.Start.Equals(node.Point)))
                j.SwapPoints();
            // Отримання всіх необрахованих стержнів
            var unknownJoints = node.Joints.Where(j => !j.Calculated).ToArray();
            if (unknownJoints.Length > 2) throw new ArgumentException("Node has more than two uncalculated joints");
            // Обрахування двох невідомих стержнів
            if (unknownJoints.Length == 2)
            {
                Force f0 = unknownJoints[0].Force;
                Force f1 = unknownJoints[1].Force;

                var knownForces = node.Joints.Where(j => j.Calculated).Select(j => j.Force).Union(node.Forces).ToList();

                double negHorizontalForcesSum =
                    -knownForces.Sum(f => Math.Abs(f.Angle) == Math.PI/2 ? 0 : f.Magnitude*Math.Cos(f.Angle));
                double negVerticalForcesSum = -knownForces.Sum(f => f.Magnitude*Math.Sin(f.Angle));

                if (f0.Angle == 0)
                {
                    f1.Magnitude = negVerticalForcesSum/Math.Sin(f1.Angle);
                    f0.Magnitude = negHorizontalForcesSum - f1.Magnitude*Math.Cos(f1.Angle);
                }
                else if (Math.Abs(f0.Angle) == Math.PI/2)
                {
                    f1.Magnitude = negHorizontalForcesSum/Math.Cos(f1.Angle);
                    f0.Magnitude = negVerticalForcesSum - f1.Magnitude*Math.Sin(f1.Angle);
                }
                else if (f1.Angle == 0)
                {
                    f0.Magnitude = negVerticalForcesSum/Math.Sin(f0.Angle);
                    f1.Magnitude = negHorizontalForcesSum - f0.Magnitude*Math.Cos(f0.Angle);
                }
                else if (Math.Abs(f1.Angle) == Math.PI/2)
                {
                    f0.Magnitude = negHorizontalForcesSum/Math.Cos(f0.Angle);
                    f1.Magnitude = negVerticalForcesSum - f0.Magnitude*Math.Sin(f0.Angle);
                }
                else
                {
                    f1.Magnitude = (negVerticalForcesSum - negHorizontalForcesSum*Math.Tan(f0.Angle))/
                                   (Math.Sin(f1.Angle) - Math.Cos(f1.Angle)*Math.Tan(f0.Angle));
                    f0.Magnitude = (negHorizontalForcesSum - f1.Magnitude*Math.Cos(f1.Angle))/Math.Cos(f0.Angle);
                }

                unknownJoints[0].Calculated = true;
                unknownJoints[1].Calculated = true;

            }
            // Обрахування одного невідомого стержня
            else if (unknownJoints.Length == 1)
            {
                Force f0 = unknownJoints[0].Force;

                var knownForces = node.Joints.Where(j => j.Calculated).Select(j => j.Force).Union(node.Forces).ToList();

                double negHorizontalForcesSum =
                    -knownForces.Sum(f => Math.Abs(f.Angle) == Math.PI / 2 ? 0 : f.Magnitude * Math.Cos(f.Angle));
                double negVerticalForcesSum = -knownForces.Sum(f => f.Magnitude * Math.Sin(f.Angle));

                if (f0.Angle == 0)
                    f0.Magnitude = negHorizontalForcesSum;
                else if (Math.Abs(f0.Angle) == Math.PI/2)
                    f0.Magnitude = negVerticalForcesSum;
                else
                    f0.Magnitude = negHorizontalForcesSum/Math.Cos(f0.Angle);

                unknownJoints[0].Calculated = true;
            }
        }
    }
}