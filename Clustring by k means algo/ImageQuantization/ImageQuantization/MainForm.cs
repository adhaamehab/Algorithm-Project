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
            }
            txtWidth.Text = ImageOperations.GetWidth(ref ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ref ImageMatrix).ToString();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void btnQuantize_Click(object sender, EventArgs e)
        {
            Quantization.DistincitColors(ref ImageMatrix);
            textBox1.Text = (Quantization.NumberOfNodes.ToString());
            Quantization.kMeans(Convert.ToInt32(kClusters.Text), 1);
            RGBPixel[,]Output = Quantization.Quantize(ref ImageMatrix);
            ImageOperations.DisplayImage(ref Output, pictureBox2);
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
    }
}