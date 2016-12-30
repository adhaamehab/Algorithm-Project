using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageQuantization.DS;

namespace ImageQuantization
{
    public class Quantization
    {
        public static int NumberOfNodes;
        static int[,,] IDcolor;
        static RGBPixel[] Nodes;
        public static void DistincitColors(ref RGBPixel[,] ImageMatrix)
        {
            bool[,,] AllColors = new bool[256, 256, 256];
            IDcolor = new int[256, 256, 256];
            NumberOfNodes = 0;
            for (int i = 0, N = ImageOperations.GetHeight(ref ImageMatrix); i < N; i++)
            {
                for (int j = 0, M = ImageOperations.GetWidth(ref ImageMatrix); j < M; j++)
                {
                    if (AllColors[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] == false)
                    {
                        AllColors[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] = true;
                        NumberOfNodes++;
                    }
                }
            }
            Nodes = new RGBPixel[NumberOfNodes];
            int ii = 0;
            for(int i = 0; i < 256; i++)
            {
                for(int j = 0; j < 256; j++)
                {
                    for(int k = 0; k < 256; k++)
                    {
                        if(AllColors[i,j,k] == true)
                        {
                            Nodes[ii] = new RGBPixel(i, j, k);
                            IDcolor[i, j, k] = ii;
                            ii++;
                        }
                    }
                }
            }
        }

        static RGBPixel centroid(ref List<RGBPixel> cluster)
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

        static double Distance(ref RGBPixel p1, ref RGBPixel p2)
        {
            int r = p1.red - p2.red;
            int g = p1.green - p2.green;
            int b = p1.blue - p2.blue;

            return Math.Sqrt((r * r) + (g * g) + (b * b));
        }

        static RGBPixel[] tmu;
        static int[] tc;
        public static double kMeans(int K)
        {
            var result = Enumerable.Range(0, NumberOfNodes).OrderBy(g => Guid.NewGuid()).Take(K).ToArray();

            tmu = new RGBPixel[K];
            tc = new int[NumberOfNodes];

            for(int k = 0; k < K; k++)
            {
                tmu[k] = Nodes[result[k]];
            }

            while (true)
            {
                for(int i = 0; i < NumberOfNodes; i++)
                {
                    double minD = Double.PositiveInfinity;
                    int k = -1;
                    for(int j = 0; j < K; j++)
                    {
                        double D = Distance(ref tmu[j], ref Nodes[i]);
                        if(D < minD)
                        {
                            minD = D;
                            k = j;
                        }
                    }
                    tc[i] = k;
                }

                RGBPixel[] Nmu = new RGBPixel[K];

                for(int k = 0; k < K; k++)
                {
                    List<RGBPixel> temp = new List<RGBPixel>();
                    for(int i = 0; i < NumberOfNodes; i++)
                    {
                        if(tc[i] == k)
                        {
                            temp.Add(Nodes[i]);
                        }
                    }
                    Nmu[k] = centroid(ref temp);
                }

                bool done = true;
                for(int k = 0; k < K; k++)
                {
                    if(tmu[k] != Nmu[k])
                    {
                        done = false;
                    }
                }
                if (done) break;
                for (int k = 0; k < K; k++)
                {
                    tmu[k] = Nmu[k];
                }
            }
            double ret = 0.0;
            for(int i = 0; i < NumberOfNodes; i++)
            {
                double D = Distance(ref Nodes[i], ref tmu[tc[i]]);
                ret += D * D;
            }
            ret /= NumberOfNodes;
            return ret;
        }

        static RGBPixel[] mu;
        static int[] c;
        public static void kMeans(int K, int N)
        {
            mu = new RGBPixel[K];
            c = new int[NumberOfNodes];
            double min = Double.PositiveInfinity;

            for(int i = 0; i < N; i++)
            {
                double temp = kMeans(K);
                if(temp < min)
                {
                    min = temp;
                    for(int k = 0; k < K; k++)
                    {
                        mu[k] = tmu[k];
                    }
                    for(int j = 0; j < NumberOfNodes; j++)
                    {
                        c[j] = tc[j];
                    }
                }
            }
        }

        public static RGBPixel[,] Quantize(ref RGBPixel[,] ImageMatrix)
        {
            RGBPixel[,] Image = new RGBPixel[ImageOperations.GetHeight(ref ImageMatrix), ImageOperations.GetWidth(ref ImageMatrix)];

            for (int i = 0, N = ImageOperations.GetHeight(ref ImageMatrix); i < N; i++)
            {
                for (int j = 0, M = ImageOperations.GetWidth(ref ImageMatrix); j < M; j++)
                {
                    Image[i, j] = mu[c[IDcolor[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue]]];
                }
            }
            return Image;
        }
    }
}

