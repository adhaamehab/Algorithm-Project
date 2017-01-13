using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization.DS
{
    public class DisjointSet
    {
        List<int> p;

        public DisjointSet(int N)
        {
            p = new List<int>(N);
            for (int i = 0; i < N; i++)
            {
                p[i] = i;
            }
        }

        public int Find(int i)
        {
            if(p[i] == i)
                return i;
            
            return p[i] = Find(p[i]);
        }

        public bool IsConnected(int i, int j)
        {
            return Find(i) == Find(j);
        }

        public void Connect(int i, int j)
        {
            if(! IsConnected(i, j))
            {
                int x = Find(i), y = Find(j);
                p[y] = x;
            }
        }

    }
}
