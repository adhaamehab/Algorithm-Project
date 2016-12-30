using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization.DS
{
    public class Edge : IComparable<Edge>
    {
        public int From, To;
        public double Weight;

        public Edge(int from, int to, double weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public static bool operator >(Edge e1, Edge e2)
        {
            return (e1.Weight > e2.Weight);
        }

        public static bool operator <(Edge e1, Edge e2)
        {
            return (e1.Weight < e2.Weight);
        }
        
        public int CompareTo(Edge other)
        {
            return Weight.CompareTo(other.Weight);
        }

    }
}
