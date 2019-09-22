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
        double[,] normPlusMinusOne = new double[512, 512];
        double[] linearData = new double[3145728];
        Bitmap[] grabbed = new Bitmap[] { new Bitmap(512, 512), new Bitmap(512, 512) };
        string[] grabDescription = new string[2];
        bool[] grabbedInColor = new bool[2];
        int numGrabbed = 0;
        int toggleIndex = -1;
        bool disableEvents = true;
        TextBox numericUpDownTextBox = null;
        double dataMedian;

        public wmap_ilc_9yr_v5()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] data = Properties.Resources.wmap_ilc_9yr_v5_t1;
            float[] temp = new float[3145728];
            Buffer.BlockCopy(data, 0, temp, 0, data.Length);
            for (int k = 0; k < 3145728; k++)
                linearData[k] = Convert.ToDouble(temp[k]);

            numericUpDownTextBox = numericUpDownForN.Controls[1] as TextBox;
            numericUpDownTextBox.TextChanged += NumericUpDownTextBox_TextChanged;
            this.Icon = Properties.Resources.icon;
            cbScale.SelectedIndex = 0;
            cbDiagonals.SelectedIndex = 2;
            cbNextGrab.SelectedIndex = 0;
            chosenMax = 0.2;
            chosenMin = -0.2;
            txtMax.Text = chosenMax.ToString("0.0");
            txtMin.Text = chosenMin.ToString("0.0");

            //Fire things off
            disableEvents = false;
            cbBasePixel.SelectedIndex = 4;
        }

        #region Event Handlers
        private void CbScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            numericUpDownForN.Enabled = cbScale.SelectedIndex == 0;
            Render();
        }

        private void CbDiagonals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            Render();
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            Bitmap bmp = pictureBox1.Image as Bitmap;
            int index = cbNextGrab.SelectedIndex;
            for (int col = 0; col < 512; col++)
                for (int row = 0; row < 512; row++)
                    grabbed[index].SetPixel(col, row, bmp.GetPixel(col, row));
            grabDescription[index] = lblShowing.Text.Replace("Showing", String.Format("Showing Grab {0}:", index));
            grabbedInColor[index] = chkColor.Checked;
            cbNextGrab.SelectedIndex = ++index % 2;
            if (++numGrabbed > 1)
            {
                btnToggle.Enabled = true;
                btnOverlap.Enabled = grabbedInColor[0] == grabbedInColor[1];
            }
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

        private void NumericUpDownForN_ValueChanged(object sender, EventArgs e)
        {
            int textBoxValue = Convert.ToInt32(numericUpDownTextBox.Text);
            int value = Convert.ToInt32(numericUpDownForN.Value);

            if (textBoxValue != value && textBoxValue % 2 != value % 2)
                numericUpDownForN.Value = textBoxValue;
        }

        private void NumericUpDownTextBox_TextChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;

            Render();
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
            double[] forMedian = new double[262144];
            int k = 0;
            for (row = 0; row < 512; row++)
                for (col = 0; col < 512; col++)
                    forMedian[k++] = data[col, row];
            Array.Sort(forMedian);
            dataMedian = (forMedian[131071] + forMedian[131072]) / 2.0;

            GetDataMaxMin();
            if (chosenMin == double.MaxValue && chosenMax == double.MinValue)
            {
                SetChosenMaxMinToDataMaxMin();
            }
            Normalize();
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
                    temp = normPlusMinusOne[col, row];
                    normPlusMinusOne[col, row] = normPlusMinusOne[col, 511 - row];
                    normPlusMinusOne[col, 511 - row] = temp;
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
                    temp = normPlusMinusOne[col, row];
                    normPlusMinusOne[col, row] = normPlusMinusOne[511 - col, row];
                    normPlusMinusOne[511 - col, row] = temp;
                }
            }
            Render();
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            toggleIndex = ++toggleIndex % 2;
            chkColor.Checked = grabbedInColor[toggleIndex];
            pictureBox1.Image = grabbed[toggleIndex];
            lblShowing.Text = grabDescription[toggleIndex];
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (ProcessChosenMaxMin())
                Render();
        }

        private void chkReverseCheckedChanged(object sender, EventArgs e)
        {
            Normalize();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            SetChosenMaxMinToDataMaxMin();
            Normalize();
            Render();
        }
        #endregion

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
            double maxNegativeDiff = chosenMin - dataMedian;
            double maxPositiveDiff = chosenMax - dataMedian;

            for (int col = 0; col < 512; col++)
            {
                for (int row = 0; row < 512; row++)
                {
                    if (data[col, row] < dataMedian)
                        normPlusMinusOne[col, row] = (dataMedian - data[col, row]) / maxNegativeDiff;
                    else
                        normPlusMinusOne[col, row] = (data[col, row] - dataMedian) / maxPositiveDiff;
                    if (normPlusMinusOne[col, row] > 1.0)
                        normPlusMinusOne[col, row] = 1.0;
                    else if (normPlusMinusOne[col, row] < -1.0)
                        normPlusMinusOne[col, row] = -1.0;
                }
            }
            if (chkReverse.Checked)
            {
                for (int col = 0; col < 512; col++)
                    for (int row = 0; row < 512; row++)
                    {
                        normalized[col, row] = 1.0 - normalized[col, row];
                        normPlusMinusOne[col, row] = -normPlusMinusOne[col, row];
                    }
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
                            for (int row = 0; row < 512; row++)
                                for (int col = 0; col < 512; col++)
                                {
                                    double value = Math.Pow(normPlusMinusOne[col, row], N) / 2.0 + 0.5;
                                    if (value < 0.0)
                                        value = 0.0;
                                    else if (value > 1.0)
                                        value = 1.0;
                                    SetColorPixel(bmp, value, col, row);
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

        private void BtnGo_Click(object sender, EventArgs e)
        {
            int startCol = Convert.ToInt32(nudCol.Value);
            int startRow = Convert.ToInt32(nudRow.Value);
            int width = Convert.ToInt32(nudWidth.Value);
            int height = Convert.ToInt32(nudHeight.Value);
            int endCol = startCol + width;
            if (endCol > 512) endCol = 512;
            int endRow = startRow + height;
            if (endRow > 512) endRow = 512;
            int found = 0, searched = 0;
            int tolerance = Convert.ToInt32(nudTolerance.Value);
            int upperLimit = 255 - tolerance;
            Bitmap bmp = pictureBox1.Image as Bitmap;
            Color color;

            //Performance is king
            if (cbFindType.SelectedIndex == 0)
            {
                if (chkColor.Checked)
                {
                    for (int row = startRow; row < endRow; row++)
                    {
                        for (int col = startCol; col < endCol; col++)
                        {
                            ++searched;
                            color = bmp.GetPixel(col, row);
                            if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                ++found;
                        }
                    }
                }
                else
                {
                    for (int row = startRow; row < endRow; row++)
                    {
                        for (int col = startCol; col < endCol; col++)
                        {
                            ++searched;
                            color = bmp.GetPixel(col, row);
                            if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                ++found;
                        }
                    }
                }
            }
            else
            {
                if (chkColor.Checked)
                {
                    for (int row = startRow; row < endRow; row++)
                    {
                        for (int col = startCol; col < endCol; col++)
                        {
                            ++searched;
                            color = bmp.GetPixel(col, row);
                            if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                ++found;
                        }
                    }
                }
                else
                {
                    for (int row = startRow; row < endRow; row++)
                    {
                        for (int col = startCol; col < endCol; col++)
                        {
                            ++searched;
                            color = bmp.GetPixel(col, row);
                            if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                ++found;
                        }
                    }
                }
            }
            double percent = searched == 0 ? 0.0 : 100.0 * Convert.ToDouble(found) / Convert.ToDouble(searched);
            txtResults.Text = string.Format("{0} of {1} ({2}%)", found, searched, percent.ToString("0.00000"));
        }

        private void BtnOverlap_Click(object sender, EventArgs e)
        {
            Bitmap overlap = new Bitmap(512, 512);
            int tolerance = Convert.ToInt32(nudTolerance.Value);
            Color color0, color1;
            if (grabbedInColor[0])
            {
                for (int row = 0; row < 512; row++)
                {
                    for (int col = 0; col < 512; col++)
                    {
                        color0 = grabbed[0].GetPixel(col, row);
                        color1 = grabbed[1].GetPixel(col, row);
                        if (color0.R == 255 && color1.R == 255 && color0.B == 0 && color1.B == 0 && color0.G <= tolerance && color1.G <= tolerance)
                            SetColorPixel(overlap, 1.0, col, row);
                        else if (color0.R == 0 && color1.R == 0 && color0.B <= tolerance && color1.B <= tolerance && color0.G == 0 && color1.G == 0)
                            SetColorPixel(overlap, 0.0, col, row);
                        else
                            SetColorPixel(overlap, 0.5, col, row);
                    }
                }
            }
            else
            {
                int upperLimit = 255 - tolerance;
                for (int row = 0; row < 512; row++)
                {
                    for (int col = 0; col < 512; col++)
                    {
                        color0 = grabbed[0].GetPixel(col, row);
                        color1 = grabbed[1].GetPixel(col, row);
                        if (color0.R >= upperLimit && color1.R >= upperLimit && color0.B >= upperLimit && color1.B >= upperLimit && color0.G >= upperLimit && color1.G >= upperLimit)
                            SetBWPixel(overlap, 1.0, col, row);
                        else if (color0.R <= tolerance && color1.R <= tolerance && color0.B <= tolerance && color1.B <= tolerance && color0.G <= tolerance && color1.G <= tolerance)
                            SetBWPixel(overlap, 0.0, col, row);
                        else
                            SetBWPixel(overlap, 0.5, col, row);
                    }
                }
            }
            pictureBox1.Image = overlap;
            lblShowing.Text = string.Format("{0} {1} Overlap", grabDescription[0].Replace("Grab 0: ", ""), grabDescription[1].Replace("Showing Grab 1:", "And"));
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
            lblShowing.Text = string.Format("Showing Base Pixel {0} {1} {2} to {3}{4}{5}", cbBasePixel.Text, scale, chosenMax.ToString("0.000"), chosenMin.ToString("0.000"), chkRotate.Checked ? " Rotated" : "", chkReverse.Checked ? " Reversed" : "");
        }

        private void SaveFile(string extension)
        {
            string fileName = lblShowing.Text;
            int BaseInFileName = fileName.IndexOf("Base");
            if (BaseInFileName >= 0)
            {
                int newLength = fileName.Length - BaseInFileName;
                fileName = fileName.Substring(BaseInFileName, newLength).Replace(' ', '_');
            }
            saveFileDialog1.FileName = string.Format("{0}.{1}", fileName, extension);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                switch (extension)
                {
                    case "bmp": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp); break;
                    case "png": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png); break;
                    case "jpg": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                }
            }
        }

    }
}