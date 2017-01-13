using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageQuantization.DS;

namespace ImageQuantization
{

    /// <summary>
    /// Quantizer by MST and PRIM algorithm
    /// </summary>
    public class QuantizationByMST
    {
        //
        static double Mean;
        static double STDdev;
        public static int NumberOfNodes;
        
       /// <summary>
       /// get all different color in image
       /// </summary>
       /// <param name="ImageMatrix"></param>
       /// <returns>List with all differnt color</returns>
        public static List<RGBPixel> DistincitColors(ref RGBPixel[,] ImageMatrix)
        {
            List<RGBPixel> Nodes = new List<RGBPixel>();
            bool[,,] AllColors = new bool[256, 256, 256];//all possible color

            NumberOfNodes = 0;
            for (int i = 0, N = ImageOperations.GetHeight(ref ImageMatrix); i < N; i++)
            {
                for (int j = 0, M = ImageOperations.GetWidth(ref ImageMatrix); j < M; j++)
                {
                    // color is found before this time ignore it 
                    if (AllColors[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] == false)
                    {

                        AllColors[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] = true;

                        Nodes.Add(ImageMatrix[i, j]);
                    }
                }
            }

            NumberOfNodes = Nodes.Count;  
            return Nodes;
        }
        /// <summary>
        /// construct a MST  from list of Nodes without building the whole graph 
        /// </summary>
        /// <param name="Nodes">List of distincit colors </param>
        /// <returns></returns>
        public static List<Edge> Prim(List<RGBPixel> Nodes)
        {
            bool[] vis = new bool[NumberOfNodes];
            double[] cost = new double[NumberOfNodes];
            
            
            for (int i = 0; i < NumberOfNodes; i++)
                cost[i] = Double.PositiveInfinity;

            List<Edge> MST = new List<Edge>();

            double TotalCost = 0.0;
            int v = -1;
            double minD = Double.PositiveInfinity;
            int[] parent = new int[NumberOfNodes];

            for (int i=0;i<NumberOfNodes; i++)
            {
                parent[i] = -1;
            }
                int u = 0;
            int r = 0, g = 0, b = 0;

            for(int i = 0; i < NumberOfNodes - 1; i++)
            {
                vis[u] = true;
                v = -1;
                minD = Double.PositiveInfinity;

                for(int j = 0; j < NumberOfNodes; j++)
                {
                    if (!vis[j])// node not visited
                    {
                        //calculate the eculidean distance
                        r = Nodes[u].red - Nodes[j].red;
                        g = Nodes[u].green - Nodes[j].green;
                        b = Nodes[u].blue - Nodes[j].blue;
                        
                        double d = Math.Sqrt((r * r) + (g * g) + (b * b));

                        
                        if(d < cost[j])
                        {
                            cost[j] = d;
                            parent[j] = u;
                        }
                        if(cost[j] < minD)
                        {
                            minD = cost[j];
                            v = j;
                        }
                    }
                }
                
                TotalCost += minD;
                u = v;
                // u is the next node
            }

            for(int i = 1; i < NumberOfNodes; i++)
            {
                MST.Add(new Edge(parent[i], i, cost[i]));
            }

            System.Windows.Forms.MessageBox.Show("MST Cost : " + TotalCost.ToString());

            Mean = TotalCost / (NumberOfNodes - 1);
            int N = NumberOfNodes - 1;
               //calculate standard deviation
            for (int i = 0; i < N; i++)
            {
                double temp = MST[i].Weight - Mean;
                STDdev += temp * temp;
            }
            STDdev /= N;
            STDdev = Math.Sqrt(STDdev);
            return MST;
        }

        public static int AutoK(ref List<Edge> MST)
        {
            int N = NumberOfNodes - 1;
            double mean = Mean;
            double newSD = STDdev;
            double curSD = 0.0;
            bool[] removed = new bool[N];
            int K = 0;

            while (Math.Abs(curSD - newSD) >= 0.0001)
            {
                double maxDif = 0.0;
                int x = -1;
                for (int i = 0; i < NumberOfNodes - 1; i++)
                {
                    if (removed[i]) continue;
                    double temp = MST[i].Weight - mean;
                    temp *= temp;
                    if (maxDif < temp)
                    {
                        maxDif = temp;
                        x = i;
                    }
                }
                if (x == -1) break;
                removed[x] = true;
                N--;
                K++;

                mean = 0.0;
                curSD = newSD;
                newSD = 0.0;
                for (int i = 0; i < NumberOfNodes - 1; i++)
                {
                    if (removed[i]) continue;
                    mean += MST[i].Weight;
                }
                mean /= N;

                for (int i = 0; i < NumberOfNodes - 1; i++)
                {
                    if (removed[i]) continue;
                    double temp = MST[i].Weight - mean;
                    newSD += temp * temp;
                }
                newSD /= N;
                newSD = Math.Sqrt(newSD);
            }

            return K + 1;
        }

        static List<int> BFS(int s, bool[] vis,  List<List<int>> adjList)
        {
            vis[s] = true;
            Queue<int> q = new Queue<int>();
            q.Enqueue(s);
            List<int> ret = new List<int>();
            ret.Add(s);
            while (q.Count != 0)
            {
                int u = q.Dequeue();
                for (int v = 0, N = adjList[u].Count; v < N; v++)
                {
                    if (vis[adjList[u][v]] == false)
                    {
                        vis[adjList[u][v]] = true;
                        q.Enqueue(adjList[u][v]);
                        ret.Add(adjList[u][v]);
                    }
                }
            }
            return ret;
        }
        
        
        /// <summary>
        /// split the MST into K cluster 
        /// </summary>
        /// <param name="MST"></param>
        /// <param name="Nodes"></param>
        /// <param name="K"></param>
        /// <returns>list of clusters each element has clusters color</returns>
        public static List<List<RGBPixel>> k_Clusters(ref List<Edge> MST,List<RGBPixel> Nodes, int K)
        {
            List<List<RGBPixel>> Clusters = new List<List<RGBPixel>>();

            for (int i = 0; i < K; i++)
            {
                Clusters.Add(new List<RGBPixel>());
            }

            List<List<int>> adjList = new List<List<int>>();


            for (int i = 0; i < NumberOfNodes; i++)
            {
                adjList.Add(new List<int>());
            }

            for (int i = 0; i < K - 1; i++)
            {
                double maxEdge = -1;
                int ii = -1;
                for (int j = 0, N = MST.Count; j < N; j++)
                {
                    if (MST[j].Weight > maxEdge)
                    {
                        maxEdge = MST[j].Weight;
                        ii = j;
                    }
                }
                MST[ii].Weight = -1;
            }

            for (int i = 0, M = MST.Count; i < M; i++)
            {
                if (MST[i].Weight == -1) continue;

                int from = MST[i].From;
                int to = MST[i].To;

                adjList[from].Add(to);
                adjList[to].Add(from);
            }
            
            bool[] V = new bool[NumberOfNodes];

            int k = 0;
            for (int i = 0; i < NumberOfNodes; i++)
            {
                if (V[i] == false)
                {
                    List<int> temp = BFS(i, V, adjList);
                    for (int j = 0, M = temp.Count; j < M; j++)
                    {
                        Clusters[k].Add(Nodes[temp[j]]);
                    }
                    k++;
                }
            }
            return Clusters;
        }
        
        
        /// <summary>
        /// calculate new color from cluster set of colors 
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns>new color</returns>
        static RGBPixel calcNewColor(List<RGBPixel> cluster)
        {
            int R = 0, G = 0, B = 0;
            for (int i = 0, N = cluster.Count; i < N; i++)
            {
                R += (cluster[i].red);
                G += (cluster[i].green);
                B += (cluster[i].blue);
            }
            R /= cluster.Count;
            G /= cluster.Count;
            B /= cluster.Count;

            return new RGBPixel(R, G, B);

        }

        /// <summary> 
        ///  change the colors in the original image with the new colors 
        /// </summary>
        /// <param name="Clusters"></param>
        /// <param name="ImageMatrix"></param>
        /// <returns></returns>
        public static RGBPixel[,] NewColors(List<List<RGBPixel>> Clusters, RGBPixel[,] ImageMatrix)
        {
            RGBPixel[,,] NewColor = new RGBPixel[256, 256, 256];
            for (int i = 0, M = Clusters.Count; i < M; i++)
            {
                RGBPixel color = calcNewColor(Clusters[i]);

                for (int j = 0, N = Clusters[i].Count; j < N; j++)
                {
                    NewColor[Clusters[i][j].red, Clusters[i][j].green, Clusters[i][j].blue] = color;
                }
            }

            RGBPixel[,] Image = new RGBPixel[ImageOperations.GetHeight(ref ImageMatrix), ImageOperations.GetWidth(ref ImageMatrix)];

            for (int i = 0, N = ImageOperations.GetHeight(ref ImageMatrix); i < N; i++)
            {
                for (int j = 0, M = ImageOperations.GetWidth(ref ImageMatrix); j < M; j++)
                {
                    Image[i, j] = NewColor[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue];
                }
            }

            return Image;
        }

    }
}