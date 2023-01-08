using MeshApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshApp.Geometries
{
    public static class GeometryGenerator
    {
        public static List<Node> Circle(double radius, int numberOfPoints)
        {
            var angles = GeneratorHelpers.Linspace(0, Math.PI * 2, numberOfPoints);

            var nodes = angles.Select(angle => new Node
            {
                X = radius * Math.Cos(angle),
                Y = radius * Math.Sin(angle)
            });

            return nodes.ToList();
        }

        public static List<Node> Rectangle(double width, int numberOfPoints)
        {
            var nodes = new List<Node>();

            if (numberOfPoints % 2 != 0)
            {
                numberOfPoints++;
            }

            var sidePoints = GeneratorHelpers.Linspace(-width / 2, width / 2, numberOfPoints / 4);

            nodes.AddRange(sidePoints.Select(x => new Node
            {
                X = x,
                Y = width / 2
            }));

            nodes.AddRange(sidePoints.Select(y => new Node
            {
                X = width / 2,
                Y = -y
            }));

            nodes.AddRange(sidePoints.Select(x => new Node
            {
                X = -x,
                Y = -width / 2
            }));

            nodes.AddRange(sidePoints.Select(x => new Node
            {
                X = -width / 2,
                Y = x
            }));

            return nodes.DistinctBy(x => new { x.X, x.Y }).ToList();
        }
    }
}
