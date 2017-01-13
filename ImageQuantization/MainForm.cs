using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ImageQuantization.DS;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        List<RGBPixel> Nodes;
        RGBPixel[,] output;

        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ref ImageMatrix, pictureBox1);
                kClusters.Text = "";
            }
            txtWidth.Text = ImageOperations.GetWidth(ref ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ref ImageMatrix).ToString();
            Nodes = QuantizationByMST.DistincitColors(ref ImageMatrix);
            textBox1.Text = (QuantizationByMST.NumberOfNodes.ToString());
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void btnQuantize_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                List<Edge> MST = QuantizationByMST.Prim(Nodes);
                int K = 0;
                if (kClusters.Text.Length == 0)
                {
                    K = QuantizationByMST.AutoK(ref MST);
                    kClusters.Text = K.ToString();
                }
                else
                {
                    K = Convert.ToInt16(kClusters.Text);
                }
                List<List<RGBPixel>> Clusters = QuantizationByMST.k_Clusters(ref MST, Nodes, K);
                output = QuantizationByMST.NewColors(Clusters, ImageMatrix);
                ImageOperations.DisplayImage(ref output, pictureBox2);
            }
            else
            {
                QuantizationByK_Means.DistincitColors(ref ImageMatrix);
                textBox1.Text = (QuantizationByK_Means.NumberOfNodes.ToString());
                QuantizationByK_Means.kMeans(Convert.ToInt32(kClusters.Text));
                RGBPixel[,] Output = QuantizationByK_Means.Quantize(ref ImageMatrix);
                ImageOperations.DisplayImage(ref Output, pictureBox2);
            }
        }

        private void txtGaussSigma_TextChanged(object sender, EventArgs e)
        {

        }

        private void kClusters_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string FilePath = saveFileDialog1.FileName;
                ImageOperations.SaveAsBMP(ref output, FilePath);
                System.Windows.Forms.MessageBox.Show("Done");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label7.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label7.Visible = true;
        }
    }
}