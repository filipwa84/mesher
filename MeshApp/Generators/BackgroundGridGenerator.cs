using MeshApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshApp.Generators
{
    public class BackgroundGridGenerator
    {
        private readonly double _cSpacing;

        private readonly Zone _zone;

        private readonly List<Node> _sortedBoundaryNodes;

        private int _previousRunLastIndex = 0;

        public BackgroundGridGenerator(Zone zone)
        {
            _zone = zone;

            _cSpacing = 0.7 * zone.AverageBoundaryDomainNodeSpacing;
            _sortedBoundaryNodes = zone.Nodes.OrderBy(x => x.Y).ToList();
        }

        public void Generate()
        {
            var elements = _zone.BoundaryElements;
            _previousRunLastIndex = 0;

            for (var H = _zone.Ymin; H <= _zone.Ymax - _cSpacing; H = H + _zone.AverageBoundaryDomainNodeSpacing)
            {
                var intersectingElements = elements.Where(x => x.IsIntersecting(H)).OrderBy(x => x.GetPointOfIntersection(H).X).ToList();

                if (intersectingElements.Count % 2 != 0)
                    throw new Exception("Invalid number of line cuts!!!");

                
                for (var i = 0; i < intersectingElements.Count; i = i + 2)
                {                    
                    var element1 = intersectingElements[i];
                    var element2 = intersectingElements[i + 1];

                    var hNodes = GenerateNodesBetweenElements(H, _zone.AverageBoundaryDomainNodeSpacing, element1, element2);
                    
                    _zone.Nodes.AddRange(hNodes);
                    _zone.BackgroundNodes.AddRange(hNodes);                                        
                }
            }
        }

        private List<Node> GenerateNodesBetweenElements(double H, double spacing, BoundaryElement element1, BoundaryElement element2)
        {
            var hNodes = new List<Node>();
            var xStart = element1.GetPointOfIntersection(H).X + _cSpacing;

            var xEnd = element2.GetPointOfIntersection(H).X;

            for (double x = xStart; x <= xEnd; x = x + spacing)
            {
                if (!CanGenerateNode(x, H, _cSpacing)) continue;

                hNodes.Add(new Node
                {
                    X = x,
                    Y = H
                });
            }

            return hNodes;
        }

        private bool CanGenerateNode(double x, double y, double C)
        {
            for(var i = _previousRunLastIndex; i < _sortedBoundaryNodes.Count; i++)
            {
                var Pi = _sortedBoundaryNodes[i].X;
                var Qi = _sortedBoundaryNodes[i].Y;

                if (Qi < y - 2*C)
                {
                    _previousRunLastIndex = i;
                }

                if (Qi >= y + 2*C)
                {
                    return true;
                }

                var condition = (x - Pi) * (x - Pi)  + (y - Qi) * (y - Qi);

                if (condition < C * C)
                {
                    return false;
                }
                
            }

            return true;
        }

        
    }
}
