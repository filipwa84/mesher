using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshApp.Models
{
    public class Zone
    {
        public BoundaryDomain OuterDomain { get; set; }
        public List<BoundaryDomain> InnerDomains { get; set; }
        public List<BoundaryElement> BoundaryElements { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Node> BackgroundNodes { get; set; }

        public Zone(BoundaryDomain outerDomain, params BoundaryDomain[] innerDomains)
        {
            OuterDomain = outerDomain;
            InnerDomains = innerDomains.ToList();

            Nodes = GetNodes(OuterDomain, InnerDomains);
            BoundaryElements = GetBoundaryElements();
            BackgroundNodes = new List<Node>();
        }

        private List<Node> GetNodes(BoundaryDomain outerDomain, List<BoundaryDomain> innerDomains)
        {
            var nodes = new List<Node>();
            nodes.AddRange(outerDomain.Nodes);
            
            foreach(var domain in innerDomains)
            {
                nodes.AddRange(domain.Nodes);
            }

            return nodes;
        }

        private List<BoundaryElement> GetBoundaryElements()
        {
            var elements = new List<BoundaryElement>(OuterDomain.BoundaryElements);

            var innerElements = InnerDomains.Select(x => x.BoundaryElements);

            foreach (var e in innerElements)
            {
                elements.AddRange(e);
            }

            return elements;
        }

        public double AverageBoundaryDomainNodeSpacing => GetAverageZoneSpacing(this);
        public double Ymax => OuterDomain.BoundaryElements.Max(x => x.P.Y);
        public double Ymin => OuterDomain.BoundaryElements.Min(x => x.P.Y);

        private double GetAverageZoneSpacing(Zone zone)
        {
            var innerDomainsAverageSpacing = zone.InnerDomains.Select(x => x.AverageNodeSpacing).Average();

            var outerDomainAverageSpacing = zone.OuterDomain.AverageNodeSpacing;

            var domainSpacing = (innerDomainsAverageSpacing + outerDomainAverageSpacing) / 2.0;

            return domainSpacing;
        }
    }
}
