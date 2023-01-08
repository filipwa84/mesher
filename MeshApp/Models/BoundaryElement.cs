using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MeshApp.Models
{
    public class BoundaryElement
    {
        public Node P { get; set; }
        public Node Q { get; set; }
        public Vector PQ { get; }

        public double Length => GetLength();
        public double MinX => GetMinX();
        public double MinY => GetMinY();

        public BoundaryElement(Node nodeP, Node nodeQ)
        {
            P = nodeP;
            Q = nodeQ;

            PQ = new Vector(nodeP, nodeQ);
        }

        /// <summary>
        /// Checkes if there is an intersection between current element and the line y = H
        /// </summary>
        /// <param name="H"></param>
        /// <returns></returns>
        public bool IsIntersecting(double H)
        {
            var X1 = P.X;
            var Y1 = P.Y;

            var X2 = Q.X;
            var Y2 = Q.Y;

            var condition1 = (Y1 - H) * (Y2 - H) < 0;
            var condition2 = (Y1 - H) * (Y2 - H) == 0 && (H > Y1 || H > Y2);

            return condition1 || condition2;
        }

        /// <summary>
        /// Gets the point of intersection between current element and the line y = H
        /// </summary>
        /// <param name="H"></param>
        /// <returns></returns>
        public Node GetPointOfIntersection(double H)
        {
            var X1 = P.X;
            var Y1 = P.Y;

            var X2 = Q.X;
            var Y2 = Q.Y;

            var X = (H - Y1) * (X2 - X1) / (Y2 - Y1) + X1;

            return new Node
            {
                X = X,
                Y = H
            };
        }

        private double GetMinY()
        {
            var Y1 = P.Y;
            var Y2 = Q.Y;

            return Math.Min(Y1, Y2);
        }

        private double GetMinX()
        {
            var X1 = P.X;
            var X2 = Q.X;

            return Math.Min(X1, X2);
        }

        private double GetLength()
        {
            return Math.Sqrt(Math.Pow(Q.X - P.X, 2) + Math.Pow(Q.Y - P.Y, 2));
        }
    }

    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        private Node P { get; set; }
        private Node Q { get; set; }

        public Vector(Node p, Node q)
        {
            X = q.X - p.X;
            Y = q.Y - p.Y;

            P = p;
            Q = q;
        }
        public bool IsNodeToLeftOfPQ(Node node)
        {
            // Calculate the X and Y values of the node position relative to the starting point of the original vector
            var nodeVectorX = node.X - X;
            var nodeVectorY = node.Y - Y;

            // Calculate the cross product of the vector and the node position
            var crossProduct = (X * nodeVectorY) - (Y * nodeVectorX);

            // If the cross product is positive, the node is to the left of the vector
            return crossProduct > 0;
        }

        public double CrossWith(Vector vector)
        {
            return X * vector.Y - Y * vector.X;
        }

        public double AreaWith(Vector vector)
        {
            return 0.5 * CrossWith(vector);
        }

    }
}
