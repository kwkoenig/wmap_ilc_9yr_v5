using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace wmap_ilc_9yr_v5
{
    public partial class wmap_ilc_9yr_v5 : Form
    {
        double chosenMin = double.MaxValue, chosenMax = double.MinValue;
        double dataMin = double.MaxValue, dataMax = double.MinValue;
        double[,] data = new double[512, 512];
        double[,] normalized = new double[512, 512];
        double[] linearData = new double[3145728];
        Bitmap[] grabbed = new Bitmap[] { new Bitmap(512, 512), new Bitmap(512, 512) };
        string[] grabDescription = new string[2];
        int numGrabbed = 0;
        int toggleIndex = 0;
        bool disableEvents = false;

        public wmap_ilc_9yr_v5()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!ReadInData())
                Application.Exit();

            saveFileDialog1.InitialDirectory = Application.StartupPath;

            cbBasePixel.SelectedIndex = 4;
            cbScale.SelectedIndex = 1;
            cbNextGrab.SelectedIndex = 0;
            cbDiagonals.SelectedIndex = 0;
            numericUpDownForN.Enabled = false;
            TextBox numericTextBox = numericUpDownForN.Controls[1] as TextBox;
            numericTextBox.TextChanged += NumericTextBox_TextChanged;

            Render();
        }

        private void NumericTextBox_TextChanged(object sender, EventArgs e)
        {
            Render();
        }

        private bool ReadInData()
        {
            string inputFilePathAndName = @".\wmap_ilc_9yr_v5_t1.txt";
            if (!File.Exists(inputFilePathAndName))
            {
                MessageBox.Show(Properties.Resources.Lumbergh);
                return false;
            }
            using (StreamReader sr = new StreamReader(inputFilePathAndName))
            {
                for (int k = 0; k < 3145728; k++)
                    linearData[k] = double.Parse(sr.ReadLine());
            }
            return true;
        }

        private void cbBasePixel_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkRotate.Checked = false;
            //fill 2-D array with data.  512 X 512 = 262144
            int basePixel = cbBasePixel.SelectedIndex;
            int offSet = 262144 * basePixel;

            //https://healpix.jpl.nasa.gov/html/intronode3.htm

            long col, row;
            for (UInt32 A = 0; A < 262144; A++)
            {
                row = A & 1;
                col = (A & (1 << 17)) >> 9;
                for (int n = 1; n < 9; n++)
                {
                    row += (A & (1 << (2 * n))) >> n;
                    col += (A & (1 << (2 * n - 1))) >> n;
                }
                data[511 - row, 511 - col] = linearData[offSet++];

            }
            GetDataMaxMin();
            if (chosenMin == double.MaxValue && chosenMax == double.MinValue)
            {
                SetChosenMaxMinToDataMaxMin();
            }
            Normalize();
            Render();
        }

        void SetChosenMaxMinToDataMaxMin()
        {
            chosenMax = dataMax;
            chosenMin = dataMin;
            disableEvents = true;
            txtMax.Text = chosenMax.ToString("0.000");
            txtMin.Text = chosenMin.ToString("0.000");
            txtMax.ForeColor = txtMin.ForeColor = Color.Black;
            disableEvents = false;
        }

        void GetDataMaxMin()
        {
            dataMin = double.MaxValue;
            dataMax = double.MinValue;
            for (int col = 0; col < 512; col++)
                for (int row = 0; row < 512; row++)
                {
                    if (data[col, row] > dataMax)
                        dataMax = data[col, row];
                    if (data[col, row] < dataMin)
                        dataMin = data[col, row];
                }
            lblMax.Text = String.Format("Max ({0}):", dataMax.ToString("0.000"));
            lblMin.Text = String.Format("Min ({0}):", dataMin.ToString("0.000"));
        }

        void Normalize()
        {
            double maxDiff = chosenMax - chosenMin;
            for (int col = 0; col < 512; col++)
            {
                for (int row = 0; row < 512; row++)
                {
                    normalized[col, row] = (data[col, row] - chosenMin) / maxDiff;
                    if (normalized[col, row] > 1.0)
                        normalized[col, row] = 1.0;
                    else if (normalized[col, row] < 0.0)
                        normalized[col, row] = 0.0;
                }
            }
            if (chkReverse.Checked)
            {
                for (int col = 0; col < 512; col++)
                    for (int row = 0; row < 512; row++)
                        normalized[col, row] = 1.0 - normalized[col, row];
            }
        }

        bool ProcessChosenMaxMin()
        {
            double tempMax, tempMin;
            if (!double.TryParse(txtMax.Text, out tempMax))
            {
                txtMax.ForeColor = Color.Red;
                return false;
            }
            if (!double.TryParse(txtMin.Text, out tempMin))
            {
                txtMin.ForeColor = Color.Red;
                return false;
            }
            txtMax.ForeColor = txtMin.ForeColor = Color.Black;
            if (chosenMax == tempMax && chosenMin == tempMin)
                return false;
            chosenMax = tempMax;
            chosenMin = tempMin;
            Normalize();
            return true;
        }
        void Render()
        {
            Bitmap bmp = new Bitmap(512, 512);

            //Big-time performance gain with duplicate code

            if (chkColor.Checked)
            {
                switch (cbScale.SelectedIndex)
                {
                    case 0:
                        int N = Convert.ToInt32(numericUpDownForN.Value);
                        if (N % 2 == 1)
                        {
                            double[,] temp = new double[512, 512];
                            double minRaisedValue = Math.Pow(chosenMin, N);
                            double maxRaisedValue = Math.Pow(chosenMax, N);
                            double maxRaisedDiff = maxRaisedValue - minRaisedValue;
                            for (int row = 0; row < 512; row++)
                                for (int col = 0; col < 512; col++)
                                {
                                    temp[col, row] = (Math.Pow(data[col, row], N) - minRaisedValue) / maxRaisedDiff;
                                    if (temp[col, row] > 1.0)
                                        temp[col, row] = 1.0;
                                    else if (temp[col, row] < 0.0)
                                        temp[col, row] = 0.0;
                                    SetColorPixel(bmp, temp[col, row], col, row);
                                }
                        }
                        else
                        {
                            for (int row = 0; row < 512; row++)
                                for (int col = 0; col < 512; col++)
                                    SetColorPixel(bmp, Math.Pow(normalized[col, row], N), col, row);
                        }
                        break;
                    case 1:
                        for (int row = 0; row < 512; row++)
                            for (int col = 0; col < 512; col++)
                                SetColorPixel(bmp, normalized[col, row], col, row);
                        break;
                    case 2:
                        double logOfTwo = Math.Log(2.0);
                        for (int row = 0; row < 512; row++)
                            for (int col = 0; col < 512; col++)
                                SetColorPixel(bmp, Math.Log(normalized[col, row] + 1) / logOfTwo, col, row);
                        break;
                    case 3:
                        double eMinusOne = Math.Exp(1.0) - 1.0;
                        for (int row = 0; row < 512; row++)
                            for (int col = 0; col < 512; col++)
                                SetColorPixel(bmp, (Math.Exp(normalized[col, row]) - 1.0) / eMinusOne, col, row);
                        break;
                }
            }
            else
            {
                switch (cbScale.SelectedIndex)
                {
                    case 0:
                        int N = Convert.ToInt32(numericUpDownForN.Value);
                        if (N % 2 == 1)
                        {
                            double[,] temp = new double[512, 512];
                            double minRaisedValue = Math.Pow(chosenMin, N);
                            double maxRaisedValue = Math.Pow(chosenMax, N);
                            double maxRaisedDiff = maxRaisedValue - minRaisedValue;
                            for (int row = 0; row < 512; row++)
                                for (int col = 0; col < 512; col++)
                                {
                                    temp[col, row] = (Math.Pow(data[col, row], N) - minRaisedValue) / maxRaisedDiff;
                                    if (temp[col, row] > 1.0)
                                        temp[col, row] = 1.0;
                                    else if (temp[col, row] < 0.0)
                                        temp[col, row] = 0.0;
                                    SetBWPixel(bmp, temp[col, row], col, row);
                                }
                        }
                        else
                        {
                            for (int row = 0; row < 512; row++)
                                for (int col = 0; col < 512; col++)
                                    SetBWPixel(bmp, Math.Pow(normalized[col, row], N), col, row);
                        }
                        break;
                    case 1:
                        for (int row = 0; row < 512; row++)
                            for (int col = 0; col < 512; col++)
                                SetBWPixel(bmp, normalized[col, row], col, row);
                        break;
                    case 2:
                        double logOfTwo = Math.Log(2.0);
                        for (int row = 0; row < 512; row++)
                            for (int col = 0; col < 512; col++)
                                SetBWPixel(bmp, Math.Log(normalized[col, row] + 1) / logOfTwo, col, row);
                        break;
                    case 3:
                        double eMinusOne = Math.Exp(1.0) - 1.0;
                        for (int row = 0; row < 512; row++)
                            for (int col = 0; col < 512; col++)
                                SetBWPixel(bmp, Math.Exp(normalized[col, row] - 1) / eMinusOne, col, row);
                        break;
                }
            }

            switch (cbDiagonals.SelectedIndex)
            {
                case 0:
                    for (int rowCol = 0; rowCol < 512; rowCol++)
                    {
                        SetBWPixel(bmp, 0.0, rowCol, rowCol);
                        SetBWPixel(bmp, 0.0, rowCol, 511 - rowCol);
                    }
                    break;
                case 1:
                    for (int rowCol = 0; rowCol < 512; rowCol++)
                    {
                        SetBWPixel(bmp, 1.0, rowCol, rowCol);
                        SetBWPixel(bmp, 1.0, rowCol, 511 - rowCol);
                    }
                    break;
                case 2:
                    break;
            }

            pictureBox1.Image = bmp;
            DescribeImage();
        }

        private void SetColorPixel(Bitmap bmp, double A, int col, int row)
        {
            byte red = 0, green = 0, blue = 0;
            double scale = 255 * 5;
            if (A < .2)
            {
                red = 0;
                green = 0;
                blue = Convert.ToByte(A * scale);
            }
            else if (A < .4)
            {
                red = 0;
                green = Convert.ToByte(255 * (5 * A - 1));
                blue = 255;
            }
            else if (A < .6)
            {
                red = 0;
                green = 255;
                blue = Convert.ToByte(255 * (3 - 5 * A));
            }
            else if (A < .8)
            {
                red = Convert.ToByte(255 * (5 * A - 3));
                green = 255;
                blue = 0;
            }
            else
            {
                red = 255;
                green = Convert.ToByte(scale * (1 - A));
                blue = 0;
            }
            Color color = new Color();
            color = Color.FromArgb(red, green, blue);
            bmp.SetPixel(col, row, color);
        }

        private void SetBWPixel(Bitmap bmp, double A, int col, int row)
        {
            byte rgb;
            rgb = Convert.ToByte(A * 255.0);
            Color color = new Color();
            color = Color.FromArgb(rgb, rgb, rgb);
            bmp.SetPixel(col, row, color);
        }

        private void Render_Required(object sender, EventArgs e)
        {
            Render();
        }


        private void chkRotate_CheckedChanged(object sender, EventArgs e)
        {
            double temp;
            //swap rows
            for (int row = 0; row < 256; row++)
            {
                for (int col = 0; col < 512; col++)
                {
                    temp = data[col, row];
                    data[col, row] = data[col, 511 - row];
                    data[col, 511 - row] = temp;
                    temp = normalized[col, row];
                    normalized[col, row] = normalized[col, 511 - row];
                    normalized[col, 511 - row] = temp;
                }
            }
            // swap columns
            for (int col = 0; col < 256; col++)
            {
                for (int row = 0; row < 512; row++)
                {
                    temp = data[col, row];
                    data[col, row] = data[511 - col, row];
                    data[511 - col, row] = temp;
                    temp = normalized[col, row];
                    normalized[col, row] = normalized[511 - col, row];
                    normalized[511 - col, row] = temp;
                }
            }
            Render();
        }

        private void DescribeImage()
        {
            string scale = cbScale.Text;
            if (scale.Contains("Nth"))
            {
                int N = Convert.ToInt32(numericUpDownForN.Value);
                switch (N)
                {
                    case 2: scale = scale.Replace("Nth", "2nd"); break;
                    case 3: scale = scale.Replace("Nth", "3rd"); break;
                    default: scale = scale.Replace("N", N.ToString()); break;
                }
            }
            lblShowing.Text = string.Format("Showing Base Pixle {0} {1}{2}{3} {4} to {5}", cbBasePixel.Text, scale,
                chkRotate.Checked ? " Rotated" : "", chkReverse.Checked ? " Reversed" : "", chosenMin.ToString("0.000"), chosenMax.ToString("0.000"));
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            Bitmap bmp = pictureBox1.Image as Bitmap;
            int index = cbNextGrab.SelectedIndex;
            for (int col = 0; col < 512; col++)
                for (int row = 0; row < 512; row++)
                    grabbed[index].SetPixel(col, row, bmp.GetPixel(col, row));
            grabDescription[index] = lblShowing.Text.Replace("Showing", String.Format("Showing Grab {0}:", index));
            cbNextGrab.SelectedIndex = ++index % 2;
            if (!btnToggle.Enabled && ++numGrabbed > 1)
                btnToggle.Enabled = true;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            string showing = lblShowing.Text;
            int indexOfGrab = showing.IndexOf("Grab");
            toggleIndex = indexOfGrab < 0 ? 1 : Convert.ToInt32(showing.Substring(indexOfGrab + 5, 1));
            toggleIndex = ++toggleIndex % 2;
            pictureBox1.Image = grabbed[toggleIndex];
            lblShowing.Text = grabDescription[toggleIndex];
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (ProcessChosenMaxMin())
                Render();
        }

        private void txtMax_TextChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            if (ProcessChosenMaxMin())
                Render();
        }

        private void txtMin_TextChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            if (ProcessChosenMaxMin())
                Render();
        }

        private void chkReverseCheckedChanged(object sender, EventArgs e)
        {
            Normalize();
            Render();
        }

        private void CbDiagonals_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }

        private void NumericUpDownForN_Validated(object sender, EventArgs e)
        {
            MessageBox.Show("hey");
        }

        private void SaveFile(string suffix)
        {
            string fileName = lblShowing.Text;
            int BaseInFileName = fileName.IndexOf("Base");
            if (BaseInFileName >= 0)
            {
                int newLength = fileName.Length - BaseInFileName;
                fileName = fileName.Substring(BaseInFileName, newLength);
            }
            saveFileDialog1.FileName = string.Format("{0}.{1}", fileName, suffix);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                switch (suffix)
                {
                    case "bmp": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp); break;
                    case "png": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png); break;
                    case "jpg": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                }
            }
        }

        private void CbScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDownForN.Enabled = cbScale.SelectedIndex == 0;
            Render();
        }

        private void BMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile("bmp");
        }

        private void PNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile("png");
        }

        private void JPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile("jpg");
        }

        private void NumericUpDownForN_ValueChanged(object sender, EventArgs e)
        {
            Render();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SetChosenMaxMinToDataMaxMin();
            Normalize();
            Render();
        }

    }
}