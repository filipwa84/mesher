using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshApp.Models
{
    public class Node
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double DistanceTo(Node remoteNode)
        {
            return Math.Sqrt(Math.Pow(remoteNode.X - X, 2) + Math.Pow(remoteNode.Y - Y, 2));
        }
    }
}
