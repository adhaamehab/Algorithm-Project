using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ImageQuantization.DS
{
    public class Graph<T>
    {
        protected List<T> vertices;

        public int maxVerts;

        public List<Edge> GraphEdges;

        //protected double[,] adjacencyMatrix;

        public List<List<double>> adjacencyMatrix;

        protected int[] verticesVisits;


        //intialize Graph with number of nodes

        public Graph()
        {

        }
        public Graph(List<T> v)
        {
            maxVerts = v.Count;
            vertices = v;
            adjacencyMatrix = new List<List<double>>();
            /*adjacencyMatrix = new List<List<double>>(100);
            for (int i = 0; i < adjacencyMatrix.Count; i++)
                adjacencyMatrix[i] = new List<double>(maxVerts);*/
            verticesVisits = new int[maxVerts];
            GraphEdges = new List<Edge>();
        }

        public void AttachEdge(int v1Index, int v2Index, double Weight)
        {
            adjacencyMatrix[v1Index][v2Index] = Weight;
            adjacencyMatrix[v2Index][v1Index] = Weight;
            Edge E = new Edge(v1Index, v2Index, Weight);
            GraphEdges.Add(E);
        }
    }
}