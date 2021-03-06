﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace wmap_ilc_9yr_v5
{
    public partial class wmap_ilc_9yr_v5 : Form
    {
        int basePixel;
        double[] chosenMax = new double[12];
        double[] chosenMin = new double[12];
        string[] chosenNames = new string[24];

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
        double dataMedian;
        List<Point> localMaxs, localMins;
        List<Color> localMaxColors, localMinColors;
        string spotString;

        public wmap_ilc_9yr_v5()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 12; i++)
            {
                chosenNames[2*i] = String.Format("{0}max", i);
                chosenNames[2 * i + 1] = String.Format("{0}min", i);
            }
            string settingsFileName = Application.ExecutablePath.Replace(".exe", ".settings.txt");
            if (!File.Exists(settingsFileName))
            {
                for (int i = 0; i < 12; i++)
                {
                    chosenMax[i] = Double.MinValue;
                    chosenMin[i] = Double.MaxValue;
                }
            }
            else
            {
                Dictionary<string, double> settings = new Dictionary<string, double>();
                double parsed;
                using (StreamReader sr = File.OpenText(settingsFileName))
                {
                    string s;
                    string[] splitted;
                    while ((s = sr.ReadLine()) != null)
                    {
                        splitted = s.Split(':');
                        if (splitted != null && splitted.Length == 2 && splitted[0].Length > 0 && splitted[1].Length > 0)
                        {
                            if (double.TryParse(splitted[1], out parsed))
                            {
                                settings.Add(splitted[0], parsed);
                            }
                        }
                    }
                }

                for (int i = 0; i < 12; i++)
                {
                    int mindex = 2 * i + 1;
                    int maxdex = 2 * i;
                    chosenMin[i] = (settings.ContainsKey(chosenNames[mindex])) ? settings[chosenNames[mindex]] : -0.19;
                    chosenMax[i] = (settings.ContainsKey(chosenNames[maxdex])) ? settings[chosenNames[maxdex]] : 0.18;
                }

            }

            byte[] data = Properties.Resources.wmap_ilc_9yr_v5_t1;
            float[] temp = new float[3145728];
            Buffer.BlockCopy(data, 0, temp, 0, data.Length);
            for (int k = 0; k < 3145728; k++)
                linearData[k] = Convert.ToDouble(temp[k]);

            this.Icon = Properties.Resources.icon;
            cbScale.SelectedIndex = 1;
            cbDiagonals.SelectedIndex = 2;
            cbNextGrab.SelectedIndex = 0;
            cbExtremaRegion.SelectedIndex = 0;

            //Fire things off
            disableEvents = false;
            cbBasePixel.SelectedIndex = 4;
        }

        private void cbBasePixel_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            disableEvents = true;
            basePixel = cbBasePixel.SelectedIndex;
            chkRotate.Checked = false;

            //fill 2-D array with data.  512 X 512 = 262144
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

            GetDataMaxMin(basePixel);
            txtMax.Text = chosenMax[basePixel].ToString("0.000");
            txtMin.Text = chosenMin[basePixel].ToString("0.000");
            SetPercentNudsFromChosen();
            Normalize();
            Render();
            disableEvents = false;
        }

        private void SetPercentNudsFromChosen()
        {
            double percent = 100.0 * chosenMax[basePixel] / dataMax;
            decimal forNud = percent > 100.0 ? 100 : percent < 0.0 ? 0 : Convert.ToDecimal(percent);
            nudPercentMax.Value = forNud;
            percent = 100.0 * chosenMin[basePixel] / dataMin;
            forNud = percent > 100.0 ? 100 : percent < 0.0 ? 0 : Convert.ToDecimal(percent);
            nudPercentMin.Value = forNud;
        }

        private void CbScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            numericUpDownForN.Enabled = cbScale.SelectedIndex == 0;
            Render();
            disableEvents = false;
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
            grabDescription[index] = lblShowing.Text.Replace("Showing", String.Format("Showing Grab {0}:", index == 0 ? "A" : "B"));
            grabbedInColor[index] = chkColor.Checked;
            if (miGrabToggle.Checked)
                cbNextGrab.SelectedIndex = ++index % 2;
            if (++numGrabbed > 1)
            {
                btnToggle.Enabled = true;
                btnOverlap.Enabled = grabbedInColor[0] == grabbedInColor[1];
            }
        }

        private void TxtMax_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || disableEvents)
                return;
            disableEvents = true;
            if (ProcessChosenMaxMin())
            {
                double percent = 100.0 * chosenMax[basePixel] / dataMax;
                decimal forNud = percent > 100.0 ? 100 : percent < 0.0 ? 0 : Convert.ToDecimal(percent);
                nudPercentMax.Value = forNud;
                Render();
            }
            disableEvents = false;
        }

        private void TxtMin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || disableEvents)
                return;
            disableEvents = true;
            if (ProcessChosenMaxMin())
            {
                double percent = 100.0 * chosenMin[basePixel] / dataMin;
                decimal forNud = percent > 100.0 ? 100 : percent < 0.0 ? 0 : Convert.ToDecimal(percent);
                nudPercentMin.Value = forNud;
                Render();
            }
            disableEvents = false;
        }

        private void NudPercentMin_ValueChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            txtMin.Text = (Convert.ToDouble(nudPercentMin.Value) * dataMin / 100.0).ToString("0.000");
            if (ProcessChosenMaxMin())
                Render();
            disableEvents = false;
        }

        private void NudPercentMax_ValueChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            txtMax.Text = (Convert.ToDouble(nudPercentMax.Value) * dataMax / 100.0).ToString("0.000");
            if (ProcessChosenMaxMin())
                Render();
            disableEvents = false;
        }

        private void NumericUpDownTextBox_TextChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            Render();
            disableEvents = false;
        }

        private void chkRotate_CheckedChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
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
            disableEvents = false;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            string showing = lblShowing.Text;
            if (showing.Contains("Grab"))
            {
                toggleIndex = ++toggleIndex % 2;
            }
            else
            {
                int baseIndex = showing.IndexOf("Base");
                showing = showing.Substring(baseIndex, showing.Length - baseIndex);
                for (int i = 0; i < 2; i++)
                {
                    string temp = grabDescription[i];
                    baseIndex = temp.IndexOf("Base");
                    temp = temp.Substring(baseIndex, temp.Length - baseIndex);
                    if (temp == showing)
                    {
                        toggleIndex = (i + 1) % 2;
                        break;
                    }
                }
            }
            chkColor.Checked = grabbedInColor[toggleIndex];
            pictureBox1.Image = grabbed[toggleIndex];
            lblShowing.Text = grabDescription[toggleIndex];
            DoFind();
            disableEvents = false;
        }

        private void chkReverseCheckedChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            Normalize();
            Render();
            disableEvents = false;
        }

        private void BMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(".bmp");
        }

        private void PNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(".png");
        }

        private void JPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(".jpg");
        }

        void GetDataMaxMin(int basePixel)
        {
            disableEvents = true;
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
            if (chosenMax[basePixel] == Double.MinValue)
                chosenMax[basePixel] = dataMax;
            if (chosenMin[basePixel] == Double.MaxValue)
                chosenMin[basePixel] = dataMin;
            disableEvents = false;
        }

        void Normalize()
        {
            double maxDiff = chosenMax[basePixel] - chosenMin[basePixel];
            for (int col = 0; col < 512; col++)
            {
                for (int row = 0; row < 512; row++)
                {
                    normalized[col, row] = (data[col, row] - chosenMin[basePixel]) / maxDiff;
                    if (normalized[col, row] > 1.0)
                        normalized[col, row] = 1.0;
                    else if (normalized[col, row] < 0.0)
                        normalized[col, row] = 0.0;
                }
            }
            double maxNegativeDiff = chosenMin[basePixel] - dataMedian;
            double maxPositiveDiff = chosenMax[basePixel] - dataMedian;

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
            if (chosenMax[basePixel] == tempMax && chosenMin[basePixel] == tempMin)
                return false;
            chosenMax[basePixel] = tempMax;
            chosenMin[basePixel] = tempMin;
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
                            for (int row = 0; row < 512; row++)
                                for (int col = 0; col < 512; col++)
                                {
                                    double value = Math.Pow(normPlusMinusOne[col, row], N) / 2.0 + 0.5;
                                    if (value < 0.0)
                                        value = 0.0;
                                    else if (value > 1.0)
                                        value = 1.0;
                                    SetBWPixel(bmp, value, col, row);
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
            DoFind();
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

        private void chkColor_CheckChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            nudTolerance.Value = chkColor.Checked ? (decimal)255 : (decimal)51;
            disableEvents = false;
            Render();
        }

        private void Find_Click(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            Bitmap bmp = pictureBox1.Image as Bitmap;
            bool maxsDrawn = localMaxs != null && chkLocalMaxs.Checked;
            bool minsDrawn = localMins != null && chkLocalMins.Checked;
            if (maxsDrawn)
            {
                for (int i = 0, len = localMaxs.Count; i < len; i++)
                    bmp.SetPixel(localMaxs[i].X, localMaxs[i].Y, localMaxColors[i]);
            }
            if (minsDrawn)
            {
                for (int i = 0, len = localMins.Count; i < len; i++)
                    bmp.SetPixel(localMins[i].X, localMins[i].Y, localMinColors[i]);
            }
            if (maxsDrawn || minsDrawn)
                pictureBox1.Image = bmp;

            DoFind();
            disableEvents = false;
        }

        private void DoFind()
        {
            int highPixels = 0, highSpots = 0, highSearched = 0;
            int lowPixels = 0, lowSpots = 0, lowSearched = 0;
            int tolerance = Convert.ToInt32(nudTolerance.Value);
            int upperLimit = 255 - tolerance;
            Bitmap bmp = pictureBox1.Image as Bitmap;
            Color currentColor;
            Color color;

            localMaxs = new List<Point>();
            localMins = new List<Point>();
            localMaxColors = new List<Color>();
            localMinColors = new List<Color>();

            int startRow = 0, endRow = 512, startCol = 0, endCol = 512;
            switch (cbExtremaRegion.Text)
            {
                case "All":
                    break;
                case "Top":
                    endRow = 256;
                    break;
                case "Bottom":
                    startRow = 256;
                    break;
                case "Left":
                    endCol = 256;
                    break;
                case "Right":
                    startCol = 256;
                    break;
                default:
                    break;
            }

            //Performance is king
            //Find high spots
            if (chkColor.Checked)
            {
                for (int row = startRow; row < endRow; row++)
                {
                    for (int col = startCol; col < endCol; col++)
                    {
                        ++highSearched;
                        currentColor = bmp.GetPixel(col, row);
                        if (currentColor.R == 255 && currentColor.G <= tolerance && currentColor.B == 0)
                        {
                            ++highPixels;
                            bool isNewSpot = true;
                            if (col > 0)
                            {
                                color = bmp.GetPixel(col - 1, row);
                                if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                    isNewSpot = false;
                                if (row > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                        isNewSpot = false;
                                }
                            }
                            if (row > 0)
                            {
                                if (col > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                        isNewSpot = false;
                                }
                                color = bmp.GetPixel(col, row - 1);
                                if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                    isNewSpot = false;
                                if (col < 511)
                                {
                                    color = bmp.GetPixel(col + 1, row - 1);
                                    if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                        isNewSpot = false;
                                }
                            }
                            if (isNewSpot && row > 0)
                            {
                                for (int nextRight = col + 1; nextRight < 512; nextRight++)
                                {
                                    color = bmp.GetPixel(nextRight, row);
                                    if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                    {
                                        color = bmp.GetPixel(nextRight, row - 1);
                                        if (color.R == 255 && color.G <= tolerance && color.B == 0)
                                        {
                                            isNewSpot = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if (isNewSpot)
                            {
                                if (GetLocalColorMax(col, row, bmp, tolerance))
                                    ++highSpots;
                            }
                        }
                    }
                }
            }
            else //BW
            {
                for (int row = startRow; row < endRow; row++)
                {
                    for (int col = startCol; col < endCol; col++)
                    {
                        ++highSearched;
                        currentColor = bmp.GetPixel(col, row);
                        if (currentColor.R >= upperLimit && currentColor.G >= upperLimit && currentColor.B >= upperLimit)
                        {
                            ++highPixels;
                            bool isNewSpot = true;
                            if (col > 0)
                            {
                                color = bmp.GetPixel(col - 1, row);
                                if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                    isNewSpot = false;
                                if (row > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                        isNewSpot = false;
                                }
                            }
                            if (row > 0)
                            {
                                if (col > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                        isNewSpot = false;
                                }
                                color = bmp.GetPixel(col, row - 1);
                                if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                    isNewSpot = false;
                                if (col < 511)
                                {
                                    color = bmp.GetPixel(col + 1, row - 1);
                                    if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                        isNewSpot = false;
                                }
                            }
                            if (isNewSpot && row > 0)
                            {
                                for (int nextRight = col + 1; nextRight < 512; nextRight++)
                                {
                                    color = bmp.GetPixel(nextRight, row);
                                    if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                    {
                                        color = bmp.GetPixel(nextRight, row - 1);
                                        if (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit)
                                        {
                                            isNewSpot = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if (isNewSpot)
                            {
                                if (GetLocalBWMax(col, row, bmp, upperLimit))
                                    ++highSpots;
                            }
                        }
                    }
                }
            }            
            //Find low spots
            if (chkColor.Checked)
            {
                for (int row = startRow; row < endRow; row++)
                {
                    for (int col = startCol; col < endCol; col++)
                    {
                        ++lowSearched;
                        color = bmp.GetPixel(col, row);
                        if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                        {
                            ++lowPixels;
                            bool isNewSpot = true;
                            if (col > 0)
                            {
                                color = bmp.GetPixel(col - 1, row);
                                if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                    isNewSpot = false;
                                if (row > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                        isNewSpot = false;
                                }
                            }
                            if (row > 0)
                            {
                                if (col > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                        isNewSpot = false;
                                }
                                color = bmp.GetPixel(col, row - 1);
                                if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                    isNewSpot = false;
                                if (col < 511)
                                {
                                    color = bmp.GetPixel(col + 1, row - 1);
                                    if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                        isNewSpot = false;
                                }
                            }
                            if (isNewSpot && row > 0)
                            {
                                for (int nextRight = col + 1; nextRight < 512; nextRight++)
                                {
                                    color = bmp.GetPixel(nextRight, row);
                                    if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                    {
                                        color = bmp.GetPixel(nextRight, row - 1);
                                        if (color.R == 0 && color.G == 0 && color.B <= tolerance)
                                        {
                                            isNewSpot = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if (isNewSpot)
                            {
                                if (GetLocalColorMin(col, row, bmp, tolerance))
                                    ++lowSpots;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int row = startRow; row < endRow; row++)
                {
                    for (int col = startCol; col < endCol; col++)
                    {
                        ++lowSearched;
                        color = bmp.GetPixel(col, row);
                        if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                        {
                            ++lowPixels;
                            bool isNewSpot = true;
                            if (col > 0)
                            {
                                color = bmp.GetPixel(col - 1, row);
                                if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                    isNewSpot = false;
                                if (row > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                        isNewSpot = false;
                                }
                            }
                            if (row > 0)
                            {
                                if (col > 0)
                                {
                                    color = bmp.GetPixel(col - 1, row - 1);
                                    if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                        isNewSpot = false;
                                }
                                color = bmp.GetPixel(col, row - 1);
                                if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                    isNewSpot = false;
                                if (col < 511)
                                {
                                    color = bmp.GetPixel(col + 1, row - 1);
                                    if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                        isNewSpot = false;
                                }
                            }
                            if (isNewSpot && row > 0)
                            {
                                for (int nextRight = col + 1; nextRight < 512; nextRight++)
                                {
                                    color = bmp.GetPixel(nextRight, row);
                                    if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                    {
                                        color = bmp.GetPixel(nextRight, row - 1);
                                        if (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance)
                                        {
                                            isNewSpot = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if (isNewSpot) {
                                if (GetLocalBWMin(col, row, bmp, tolerance))
                                    ++lowSpots;
                            }
                        }
                    }
                }
            }
            
            double highPercent = highSearched == 0 ? 0.0 : 100.0 * Convert.ToDouble(highPixels) / Convert.ToDouble(highSearched);
            double lowPercent = lowSearched == 0 ? 0.0 : 100.0 * Convert.ToDouble(lowPixels) / Convert.ToDouble(lowSearched);

            txtResults.Text = string.Format("{0} {1}, {2} {3} pixels ({4}%, {5}%)\r\n\r\n{6} {7}, {8} {9} spots"
                , highPixels, chkColor.Checked ? "red" : "white"
                , lowPixels, chkColor.Checked ? "blue" : "black"
                , highPercent.ToString("0.00000"), lowPercent.ToString("0.00000")
                , highSpots, chkColor.Checked ? "red" : "white"
                , lowSpots, chkColor.Checked ? "blue" : "black"
            );

            spotString = string.Format("{0}_{1}_{2}_{3}", highSpots, chkColor.Checked ? "red" : "white", lowSpots, chkColor.Checked ? "blue" : "black");

            if (chkLocalMaxs.Checked)
                foreach (Point point in localMaxs)
                    bmp.SetPixel(point.X, point.Y, Color.FromArgb(0, 0, 0));
            if (chkLocalMins.Checked)
                foreach (Point point in localMins)
                    bmp.SetPixel(point.X, point.Y, Color.FromArgb(255, 255, 255));
            if (chkLocalMaxs.Checked || chkLocalMins.Checked)
                pictureBox1.Image = bmp;
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
            lblShowing.Text = string.Format("{0} {1} Overlap", grabDescription[0].Replace("Grab A: ", ""), grabDescription[1].Replace("Showing Grab B:", "And"));
            DoFind();
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
            lblShowing.Text = string.Format("Showing Base Pixel {0} {1} {2} to {3}{4}{5}", cbBasePixel.Text, scale, chosenMax[basePixel].ToString("0.000"), chosenMin[basePixel].ToString("0.000"), chkRotate.Checked ? " Rotated" : "", chkReverse.Checked ? " Reversed" : "");
        }

        private void NumericUpDownForN_ValueChanged(object sender, EventArgs e)
        {
            if (disableEvents)
                return;
            disableEvents = true;
            Render();
            disableEvents = false;
        }

        private void SaveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = File.CreateText(Application.ExecutablePath.Replace(".exe", ".settings.txt")))
            {
                for (int i = 0; i < 12; i++)
                {
                    sw.WriteLine(string.Format("{0}:{1}", chosenNames[2 * i], chosenMax[i].ToString("0.000")));
                    sw.WriteLine(string.Format("{0}:{1}", chosenNames[2 * i + 1], chosenMin[i].ToString("0.000")));
                }
            }
        }

        private void ChkLocalMaxs_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap bmp = pictureBox1.Image as Bitmap;
            if (chkLocalMaxs.Checked)
            {
                Color black = Color.FromArgb(0, 0, 0);
                foreach (Point point in localMaxs)
                    bmp.SetPixel(point.X, point.Y, black);

            }
            else
            {
                for (int i = 0, len = localMaxs.Count; i < len; i++)
                    bmp.SetPixel(localMaxs[i].X, localMaxs[i].Y, localMaxColors[i]);
            }
            pictureBox1.Image = bmp;
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
            saveFileDialog1.FileName = string.Format("{0}{1}", fileName, extension);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                if (!fileName.EndsWith(extension))
                    fileName = string.Format("{0}{1}", fileName, extension);
                switch (extension)
                {
                    case ".bmp": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp); break;
                    case ".png": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png); break;
                    case ".jpg": pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                }
            }
        }

        private void SaveLocalExtrema(string maxsOrMins)
        {
            string extension = ".txt";
            string fileName = lblShowing.Text;
            int BaseInFileName = fileName.IndexOf("Base");
            if (BaseInFileName >= 0)
            {
                int newLength = fileName.Length - BaseInFileName;
                fileName = fileName.Substring(BaseInFileName, newLength).Replace(' ', '_');
            }
            saveFileDialog1.FileName = string.Format("{0}{1}{2}{3}{4}{5}{6}_{7}{8}", fileName, "_", cbExtremaRegion.Text, "_", maxsOrMins, "_tol_", nudTolerance.Value, spotString, extension);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                if (!fileName.EndsWith(extension))
                    fileName = string.Format("{0}{1}", fileName, extension);
            }
            List<Point> extrema = maxsOrMins == "Maxs" ? localMaxs : localMins;
            Point[] points = new Point[extrema.Count];
            extrema.CopyTo(points);
            Array.Sort(points, delegate (Point p1, Point p2) { return p1.X.CompareTo(p2.X); });
            using (StreamWriter sw = File.CreateText(fileName))
            {
                foreach (Point point in points)
                {
                    sw.WriteLine(string.Format("{0},{1}", point.X, point.Y));
                }
            }
        }

        private bool GetLocalColorMax(int topLeftX, int topLeftY, Bitmap bmp, int tolerance)
        {
            int startCol = topLeftX;
            int endCol = startCol;
            int row = topLeftY;
            bool foundOneInThisRow = true;
            Color color = bmp.GetPixel(startCol, topLeftY);
            int green;
            int minGreen = 256;
            Point localMax = new Point();

            // find endCol for the first row
            for (int col = startCol; col < 512 && IsRed(col, row, bmp, tolerance, out green); col++)
            {
                endCol = col;
                if (green < minGreen)
                {
                    minGreen = green;
                    localMax.X = col;
                    localMax.Y = row;
                }
            }
            // Search the next row from one less than the current start column
            if (startCol > 0)
                startCol--;
            for (; row < 512 && foundOneInThisRow; row++)
            {
                foundOneInThisRow = false;
                // Look here and left for the start column
                for (int col = startCol; col >= 0 && IsRed(col, row, bmp, tolerance, out green); col--)
                {
                    foundOneInThisRow = true;
                    startCol = col;
                    if (green < minGreen)
                    {
                        minGreen = green;
                        localMax.X = col;
                        localMax.Y = row;
                    }
                }
                // If startCol not found, look right up to 1 plus the previous end column
                if (!foundOneInThisRow)
                {
                    int lastToSearch = (endCol < 511) ? endCol + 1 : 511;
                    for (int col = startCol + 1; col < lastToSearch; col++)
                    {
                        if (IsRed(col, row, bmp, tolerance, out green))
                        {
                            foundOneInThisRow = true;
                            startCol = col;
                            if (green < minGreen)
                            {
                                minGreen = green;
                                localMax.X = col;
                                localMax.Y = row;
                            }
                            break;
                        }
                    }
                }
                // If startCol was found, find endCol
                if (foundOneInThisRow)
                {
                    for (int col = startCol + 1; col < 512 && IsRed(col, row, bmp, tolerance, out green); col++)
                    {
                        endCol = col;
                        if (green < minGreen)
                        {
                            minGreen = green;
                            localMax.X = col;
                            localMax.Y = row;
                        }
                    }
                }
                // Search the next row from one less than the current start column
                if (startCol > 0)
                    startCol--;
            }
            if (IsInRegion(localMax.X, localMax.Y))
            {
                foreach (Point max in localMaxs)
                {
                    if (max.X == localMax.X && max.Y == localMax.Y)
                        return false;
                }
                localMaxs.Add(localMax);
                localMaxColors.Add(bmp.GetPixel(localMax.X, localMax.Y));
                return true;
            }
            return false;
        }

        private bool IsInRegion (int col, int row)
        {
            int selectedIndex = cbExtremaRegion.SelectedIndex;
            if (selectedIndex < 5)
                return true;
            switch (selectedIndex)
            {
                case 5: //Top Left
                    return row <= 511 - col;
                case 6: //Bottom Right
                    return row >= 511 - col;
                case 7: //Top Right
                    return row <= col;
                case 8: //Bottom Left
                    return row >= col;
                default:
                    return true;
            }
        }

        private bool IsRed(int col, int row, Bitmap bmp, int tolerance, out int green)
        {
            Color color = bmp.GetPixel(col, row);
            green = color.G;
            return (color.R == 255 && color.G <= tolerance && color.B == 0);
        }

        private bool GetLocalBWMax(int topLeftX, int topLeftY, Bitmap bmp, int upperLimit)
        {
            int startCol = topLeftX;
            int endCol = startCol;
            int row = topLeftY;
            bool foundOneInThisRow = true;
            int green;
            int maxGreen = -1;
            Point localMax = new Point();

            // find endCol for the first row
            for (int col = startCol; col < 512 && IsWhite(col, row, bmp, upperLimit, out green); col++)
            {
                endCol = col;
                if (green > maxGreen)
                {
                    maxGreen = green;
                    localMax.X = col;
                    localMax.Y = row;
                }
            }
            // Search the next row from one less than the current start column
            if (startCol > 0)
                startCol--;
            for (; row < 512 && foundOneInThisRow; row++)
            {
                foundOneInThisRow = false;
                // Look here and left for the start column
                for (int col = startCol; col >= 0 && IsWhite(col, row, bmp, upperLimit, out green); col--)
                {
                    foundOneInThisRow = true;
                    startCol = col;
                    if (green > maxGreen)
                    {
                        maxGreen = green;
                        localMax.X = col;
                        localMax.Y = row;
                    }
                }
                // If startCol not found, look right up to 1 plus the previous end column
                if (!foundOneInThisRow)
                {
                    int lastToSearch = (endCol < 511) ? endCol + 1 : 511;
                    for (int col = startCol + 1; col < lastToSearch; col++)
                    {
                        if (IsWhite(col, row, bmp, upperLimit, out green))
                        {
                            foundOneInThisRow = true;
                            startCol = col;
                            if (green > maxGreen)
                            {
                                maxGreen = green;
                                localMax.X = col;
                                localMax.Y = row;
                            }
                            break;
                        }
                    }
                }
                // If startCol was found, find endCol
                if (foundOneInThisRow)
                {
                    for (int col = startCol + 1; col < 512 && IsWhite(col, row, bmp, upperLimit, out green); col++)
                    {
                        endCol = col;
                        if (green > maxGreen)
                        {
                            maxGreen = green;
                            localMax.X = col;
                            localMax.Y = row;
                        }
                    }
                }
                // Search the next row from one less than the current start column
                if (startCol > 0)
                    startCol--;
            }
            if (IsInRegion(localMax.X, localMax.Y))
            {
                foreach (Point max in localMaxs)
                {
                    if (max.X == localMax.X && max.Y == localMax.Y)
                        return false;
                }
                localMaxs.Add(localMax);
                localMaxColors.Add(bmp.GetPixel(localMax.X, localMax.Y));
                return true;
            }
            return false;
        }

        private void ChkLocalMins_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap bmp = pictureBox1.Image as Bitmap;
            if (chkLocalMins.Checked)
            {
                Color white = Color.FromArgb(255, 255, 255);
                foreach (Point point in localMins)
                    bmp.SetPixel(point.X, point.Y, white);

            }
            else
            {
                for (int i = 0, len = localMins.Count; i < len; i++)
                    bmp.SetPixel(localMins[i].X, localMins[i].Y, localMinColors[i]);
            }
            pictureBox1.Image = bmp;
        }

        private bool IsWhite(int col, int row, Bitmap bmp, int upperLimit, out int green)
        {
            Color color = bmp.GetPixel(col, row);
            green = color.G;
            return (color.R >= upperLimit && color.G >= upperLimit && color.B >= upperLimit);
        }

        private bool GetLocalColorMin(int topLeftX, int topLeftY, Bitmap bmp, int tolerance)
        {
            int startCol = topLeftX;
            int endCol = startCol;
            int row = topLeftY;
            bool foundOneInThisRow = true;
            int blue;
            int minblue = 256;
            Point localMin = new Point();

            // find endCol for the first row
            for (int col = startCol; col < 512 && IsBlue(col, row, bmp, tolerance, out blue); col++)
            {
                endCol = col;
                if (blue < minblue)
                {
                    minblue = blue;
                    localMin.X = col;
                    localMin.Y = row;
                }
            }
            // Search the next row from one less than the current start column
            if (startCol > 0)
                startCol--;
            for (++row; row < 512 && foundOneInThisRow; row++)
            {
                foundOneInThisRow = false;
                // Look here and left for the start column
                for (int col = startCol; col >= 0 && IsBlue(col, row, bmp, tolerance, out blue); col--)
                {
                    foundOneInThisRow = true;
                    startCol = col;
                    if (blue < minblue)
                    {
                        minblue = blue;
                        localMin.X = col;
                        localMin.Y = row;
                    }
                }
                // If startCol not found, look right up to 1 plus the previous end column
                if (!foundOneInThisRow)
                {
                    int lastToSearch = (endCol < 511) ? endCol + 1 : 511;
                    for (int col = startCol + 1; col < lastToSearch; col++)
                    {
                        if (IsBlue(col, row, bmp, tolerance, out blue))
                        {
                            foundOneInThisRow = true;
                            startCol = col;
                            if (blue < minblue)
                            {
                                minblue = blue;
                                localMin.X = col;
                                localMin.Y = row;
                            }
                            break;
                        }
                    }
                }
                // If startCol was found, find endCol
                if (foundOneInThisRow)
                {
                    for (int col = startCol + 1; col < 512 && IsBlue(col, row, bmp, tolerance, out blue); col++)
                    {
                        endCol = col;
                        if (blue < minblue)
                        {
                            minblue = blue;
                            localMin.X = col;
                            localMin.Y = row;
                        }
                    }
                }
                // Search the next row from one less than the current start column
                if (startCol > 0)
                    startCol--;
            }
            if (IsInRegion(localMin.X, localMin.Y))
            {
                foreach (Point Min in localMins)
                {
                    if (Min.X == localMin.X && Min.Y == localMin.Y)
                        return false;
                }
                localMins.Add(localMin);
                localMinColors.Add(bmp.GetPixel(localMin.X, localMin.Y));
                return true;
            }
            return false;
        }

        private void SaveLocalMaxsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLocalExtrema("Maxs");
        }

        private void SaveLocalMinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLocalExtrema("Mins");
        }

        private bool IsBlue(int col, int row, Bitmap bmp, int tolerance, out int blue)
        {
            Color color = bmp.GetPixel(col, row);
            blue = color.B;
            return (color.R == 0 && color.G == 0 && color.B <= tolerance);
        }

        private bool GetLocalBWMin(int topLeftX, int topLeftY, Bitmap bmp, int upperLimit)
        {
            int startCol = topLeftX;
            int endCol = startCol;
            int row = topLeftY;
            bool foundOneInThisRow = true;
            int green;
            int minGreen = 256;
            Point localMin = new Point();

            // find endCol for the first row
            for (int col = startCol; col < 512 && IsBlack(col, row, bmp, upperLimit, out green); col++)
            {
                endCol = col;
                if (green < minGreen)
                {
                    minGreen = green;
                    localMin.X = col;
                    localMin.Y = row;
                }
            }
            // Search the next row from one less than the current start column
            if (startCol > 0)
                startCol--;
            for (; row < 512 && foundOneInThisRow; row++)
            {
                foundOneInThisRow = false;
                // Look here and left for the start column
                for (int col = startCol; col >= 0 && IsBlack(col, row, bmp, upperLimit, out green); col--)
                {
                    foundOneInThisRow = true;
                    startCol = col;
                    if (green < minGreen)
                    {
                        minGreen = green;
                        localMin.X = col;
                        localMin.Y = row;
                    }
                }
                // If startCol not found, look right up to 1 plus the previous end column
                if (!foundOneInThisRow)
                {
                    int lastToSearch = (endCol < 511) ? endCol + 1 : 511;
                    for (int col = startCol + 1; col < lastToSearch; col++)
                    {
                        if (IsBlack(col, row, bmp, upperLimit, out green))
                        {
                            foundOneInThisRow = true;
                            startCol = col;
                            if (green < minGreen)
                            {
                                minGreen = green;
                                localMin.X = col;
                                localMin.Y = row;
                            }
                            break;
                        }
                    }
                }
                // If startCol was found, find endCol
                if (foundOneInThisRow)
                {
                    for (int col = startCol + 1; col < 512 && IsBlack(col, row, bmp, upperLimit, out green); col++)
                    {
                        endCol = col;
                        if (green < minGreen)
                        {
                            minGreen = green;
                            localMin.X = col;
                            localMin.Y = row;
                        }
                    }
                }
                // Search the next row from one less than the current start column
                if (startCol > 0)
                    startCol--;
            }
            if (IsInRegion(localMin.X, localMin.Y))
            {
                foreach (Point Min in localMins)
                {
                    if (Min.X == localMin.X && Min.Y == localMin.Y)
                        return false;
                }
                localMins.Add(localMin);
                localMinColors.Add(bmp.GetPixel(localMin.X, localMin.Y));
                return true;
            }
            return false;
        }

        private bool IsBlack(int col, int row, Bitmap bmp, int tolerance, out int green)
        {
            Color color = bmp.GetPixel(col, row);
            green = color.G;
            return (color.R <= tolerance && color.G <= tolerance && color.B <= tolerance);
        }
    }
}