using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshApp.Models
{
    public class BoundaryDomain
    {
        private BoundaryDomainDirection _domainDirection;

        public List<BoundaryElement> BoundaryElements { get; set; }
        public List<Node> Nodes { get; set; }
        public double AverageNodeSpacing { get; }



        

        public BoundaryDomain(List<Node> nodes, double spacing) 
        { 
            Nodes = RemoveDuplicates(nodes, spacing);

            BoundaryElements = GenerateElements(Nodes);

            AverageNodeSpacing = GetAverageNodeSpacing();
        }

        public List<Node> RemoveDuplicates(List<Node> nodes, double minSpacing)
        {
            for (int i = 0; i <= nodes.Count; i++)
            {
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    double distance = nodes[i].DistanceTo(nodes[j]);

                    if (distance < minSpacing)
                    {
                        nodes.RemoveAt(j);
                        j--;
                    }
                }
            }

            return nodes;
        }

        public List<BoundaryElement> GenerateElements(List<Node> nodes)
        {
            var elements = new List<BoundaryElement>();
            for(var i = 0; i< nodes.Count;i++)
            {
                if(i == nodes.Count - 1)
                {
                    var lastElement = new BoundaryElement(nodes[i], nodes[0]);
                    elements.Add(lastElement);
                    break;
                }
                
                var element = new BoundaryElement(nodes[i], nodes[i + 1]);
                elements.Add(element);
            }

            return elements;
        }

        public double GetAverageNodeSpacing()
        {
            return BoundaryElements.Average(x => x.Length);
        }

        public void ReverseDirection()
        {
            _domainDirection = _domainDirection == BoundaryDomainDirection.FirstDirection
                ? BoundaryDomainDirection.SecodDirection
                : BoundaryDomainDirection.FirstDirection;

            var firstNode = Nodes.FirstOrDefault();
            
            Nodes.Reverse();
            Nodes.Insert(0, firstNode);
            Nodes.RemoveAt(Nodes.Count - 1);
            BoundaryElements = GenerateElements(Nodes);

        }
    }

    public enum BoundaryDomainDirection
    {
        FirstDirection = 0,
        SecodDirection = 1,
    }
}
