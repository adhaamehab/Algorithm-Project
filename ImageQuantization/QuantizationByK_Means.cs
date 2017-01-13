using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageQuantization.DS;

namespace ImageQuantization
{
    /// <summary>
    /// Quantizer by K-Means Algorithm
    /// </summary>
    public class QuantizationByK_Means
    {
        public static int NumberOfNodes;
        static int[,,] IDcolor;
        static RGBPixel[] Nodes;

        /// <summary>
        /// exctract all distinict colors in the original image
        /// </summary>
        /// <param name="ImageMatrix"></param>
        public static void DistincitColors(ref RGBPixel[,] ImageMatrix)
        {

            bool[,,] AllColors = new bool[256, 256, 256];
            IDcolor = new int[256, 256, 256];//all possible colors
            NumberOfNodes = 0;

            for (int i = 0, N = ImageOperations.GetHeight(ref ImageMatrix); i < N; i++)
            {
                for (int j = 0, M = ImageOperations.GetWidth(ref ImageMatrix); j < M; j++)
                {
                    //if color is already found
                    if (AllColors[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] == false)
                    {
                        AllColors[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] = true;
                        NumberOfNodes++;
                    }
                }
            }
            Nodes = new RGBPixel[NumberOfNodes];
            int ii = 0;
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (AllColors[i, j, k] == true)
                        {
                            Nodes[ii] = new RGBPixel(i, j, k);
                            IDcolor[i, j, k] = ii;
                            ii++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// calculate the color representing  the cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns>RGBPixel</returns>
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
    
        
        /// <summary>
        /// calculate ec distance bew
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        static double Distance(ref RGBPixel p1, ref RGBPixel p2)
        {
            int r = p1.red - p2.red;
            int g = p1.green - p2.green;
            int b = p1.blue - p2.blue;

            return Math.Sqrt((r * r) + (g * g) + (b * b));
        }

        static RGBPixel[] mu;
        static int[] c;

        /// <summary>
        /// 1- Randomly intialize K clusters centriods.
        /// 2- Calculate the distance between color point and cluster centriod.
        /// 3- Assign the color to the cluster centriod whose distance from the cluster centriod is minimum of all the cluster centriods..
        /// 4- Recalculate the new clusters centriods.
        /// if new clusters centroids equals old one then stop, otherwise repeat from step 2
        /// </summary>
        /// <param name="K"></param>
        public static void kMeans(int K)
        {
            var result = Enumerable.Range(0, NumberOfNodes).OrderBy(g => Guid.NewGuid()).Take(K).ToArray();

            mu = new RGBPixel[K];
            c = new int[NumberOfNodes];

            for (int k = 0; k < K; k++) // O(K)
            {
                mu[k] = Nodes[result[k]]; 
            }

            while (true)
            {
                for (int i = 0; i < NumberOfNodes; i++)
                {
                    double minD = Double.PositiveInfinity;
                    int k = -1;
                    for (int j = 0; j < K; j++)
                    {
                        double D = Distance(ref mu[j], ref Nodes[i]);
                        if (D < minD)
                        {
                            minD = D;
                            k = j;
                        }
                    }
                    c[i] = k;
                }

                RGBPixel[] Nmu = new RGBPixel[K];

                for (int k = 0; k < K; k++)
                {
                    List<RGBPixel> temp = new List<RGBPixel>();
                    for (int i = 0; i < NumberOfNodes; i++)
                    {
                        if (c[i] == k)
                        {
                            temp.Add(Nodes[i]);
                        }
                    }
                    Nmu[k] = centroid(ref temp);
                }

                bool done = true;
                for (int k = 0; k < K; k++)
                {
                    if (mu[k] != Nmu[k])
                    {
                        done = false;
                    }
                }
                if (done) break;
                for (int k = 0; k < K; k++)
                {
                    mu[k] = Nmu[k];
                }
            }
        }

        /// <summary>
        /// 
        /// apply the kmean on the image
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <returns> new quantized image</returns>
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

