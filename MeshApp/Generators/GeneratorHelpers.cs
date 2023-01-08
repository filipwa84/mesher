using System;
using System.Collections.Generic;

namespace MeshApp.Geometries
{
    internal static class GeneratorHelpers
    {
        public static List<double> Linspace(double start, double end, int numPoints)
        {
            if (numPoints < 2)
            {
                throw new ArgumentException("Number of points must be at least 2", nameof(numPoints));
            }

            var points = new List<double>(numPoints);

            double step = (end - start) / (numPoints - 1);

            for (int i = 0; i < numPoints; i++)
            {
                double point = start + i * step;

                points.Add(point);
            }

            return points;
        }
    }
}