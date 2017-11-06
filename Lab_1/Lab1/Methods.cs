using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab1
{
    public class Methods
    {
        public static Bitmap GrayScale(Bitmap btm)
        {

            Bitmap tempPict = new Bitmap(btm);
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    var value = (oldColour.R + oldColour.G + oldColour.B) / 3;
                    newColor = System.Drawing.Color.FromArgb(value, value, value);
                    tempPict.SetPixel(x, y, newColor);
                }
            }

            return tempPict;
        }

        public static Bitmap Inverse(Bitmap btm)
        {

            Bitmap tempPict = new Bitmap(btm);
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    newColor = System.Drawing.Color.FromArgb(255 - oldColour.R, 255 - oldColour.G, 255 - oldColour.B);
                    tempPict.SetPixel(x, y, newColor);
                }
            }

            return tempPict;
        }

        public static Bitmap DarkBright(Bitmap btm, int darkBright)
        {

            Bitmap tempPict = new Bitmap(btm);
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    int R = oldColour.R + darkBright;
                    int G = oldColour.G + darkBright;
                    int B = oldColour.B + darkBright;
                    newColor = System.Drawing.Color.FromArgb(FromInterval(R), FromInterval(G), FromInterval(B));
                    tempPict.SetPixel(x, y, newColor);
                }
            }

            return tempPict;
        }
        public static Bitmap Contrast(Bitmap btm, float contrast)
        {

            Bitmap tempPict = new Bitmap(btm);
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    int R = (int)(contrast * (oldColour.R - 127)) + 127;
                    int G = (int)(contrast * (oldColour.G - 127)) + 127;
                    int B = (int)(contrast * (oldColour.B - 127)) + 127;
                    newColor = System.Drawing.Color.FromArgb(FromInterval(R), FromInterval(G), FromInterval(B));
                    tempPict.SetPixel(x, y, newColor);
                }
            }

            return tempPict;
        }

        public static Bitmap GlobBin(Bitmap btm, int r)
        {
            Bitmap tempPict = new Bitmap(btm);
            tempPict = GrayScale(tempPict);
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    int R = oldColour.R <= r ? 0 : 255;
                    newColor = System.Drawing.Color.FromArgb(R, R, R);
                    tempPict.SetPixel(x, y, newColor);
                }
            }

            return tempPict;
        }

        public static Bitmap HistNorm(Bitmap btm)
        {
            Bitmap tempPict = new Bitmap(btm);
            tempPict = GrayScale(tempPict);
            double min = 255;
            double max = 0;
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    var pix = tempPict.GetPixel(x, y);
                    if (pix.R > max)
                    {
                        max = pix.R;
                    }
                    if (pix.R < min)
                    {
                        min = pix.R;
                    }
                }
            }
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    int C = (int)((255.0 / (max - min)) * ((double)tempPict.GetPixel(x, y).R - min));
                    var newColor = System.Drawing.Color.FromArgb(C, C, C);
                    tempPict.SetPixel(x, y, newColor);
                }
            }
            return tempPict;
        }

        public static Bitmap HistEQ(Bitmap btm)
        {
            Bitmap tempPict = new Bitmap(btm);
            tempPict = GrayScale(tempPict);
            int[] VecB = new int[255];
            double[] D = new double[255];
            double[] LUT = new double[255];
            for (int i = 0; i < 255; i++)
            {
                VecB[i] = 0;
                D[i] = 0.0;
                LUT[i] = 0.0;
            }
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    var pix = tempPict.GetPixel(x, y);
                    VecB[pix.B]++;
                }
            }
            double totPixes = btm.Height * btm.Width;
            double currSum = 0;
            for (int i = 0; i < 255; i++)
            {
                currSum += VecB[i];
                D[i] = currSum / totPixes;
                LUT[i] = ((D[i] - D[0]) / (1.0 - D[0])) * (255 - 1);
            }
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    int C = (int)LUT[tempPict.GetPixel(x, y).R];
                    var newColor = System.Drawing.Color.FromArgb(C, C, C);
                    tempPict.SetPixel(x, y, newColor);
                }
            }
            return tempPict;
        }

        public static Bitmap LowPass(Bitmap btm, double a)
        {
            Bitmap tempPict = new Bitmap(btm);
            double[,] wages = new double[3, 3] { { 1, 1, 1 }, { 1, a, 1 }, { 1, 1, 1 } };

            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    double sumR = 0.0;
                    double sumG = 0.0;
                    double sumB = 0.0;
                    double dividor = 0.0;
                    for (int x2 = -1; x2 < 2; x2++)
                    {
                        for (int y2 = -1; y2 < 2; y2++)
                        {
                            if (x + x2 >= 0 && y + y2 >= 0 && x + x2 < btm.Width && y + y2 < btm.Height)
                            {
                                double currWage = wages[x2 + 1, y2 + 1];
                                sumR += btm.GetPixel(x + x2, y + y2).R * currWage;
                                sumG += btm.GetPixel(x + x2, y + y2).G * currWage;
                                sumB += btm.GetPixel(x + x2, y + y2).B * currWage;
                                dividor += currWage;
                            }
                        }
                    }
                    var newColor = System.Drawing.Color.FromArgb((int)(sumR / dividor), (int)(sumG / dividor), (int)(sumB / dividor));
                    tempPict.SetPixel(x, y, newColor);
                }
            }
            return tempPict;
        }

        public static Bitmap LocalBin(Bitmap btm, int r)
        {
            Bitmap pict = new Bitmap(btm);
            pict = GrayScale(pict);
            Bitmap tempPict = new Bitmap(btm);
            int globT = tempPict.GetPixel(0, 0).R;
            for (int x = 0; x < pict.Size.Width; x++)
            {
                for (int y = 0; y < pict.Size.Height; y++)
                {
                    System.Drawing.Color newColor, oldColour;
                    oldColour = tempPict.GetPixel(x, y);
                    Tuple<int, int> minMax = GetMinMax(r, x, y, pict);
                    int T = 0;
                    if (Math.Abs(minMax.Item2 - minMax.Item1) < 10)
                    {
                        T = globT;
                    }
                    else
                    {
                        T = (minMax.Item1 + minMax.Item2) / 2;
                        globT = (globT + T) / 2;
                    }
                    int R = oldColour.R <= T ? 0 : 255;
                    newColor = System.Drawing.Color.FromArgb(R, R, R);
                    pict.SetPixel(x, y, newColor);
                }
            }

            return pict;
        }

        private static Tuple<int, int> GetMinMax(int neighbours, int x, int y, Bitmap bmp)
        {
            Tuple<int, int> answer = new Tuple<int, int>(0, 0);
            int min = 255, max = -255;

            for (int i = -neighbours; i < neighbours; i++)
            {
                for (int j = -neighbours; j < neighbours; j++)
                {
                    if (x + i >= 0 && x + i < bmp.Width && y + j >= 0 && y + j < bmp.Height)
                    {
                        System.Drawing.Color pixel = bmp.GetPixel(x + i, y + j);
                        if (pixel.R < min)
                        {
                            min = pixel.R;
                        }
                        if (pixel.R > max)
                        {
                            max = pixel.R;
                        }
                    }
                }
            }
            return new Tuple<int, int>(min, max);

        }

        public static Tuple<Bitmap, PointCollection, PointCollection> Hist(Bitmap btm, int r)
        {
            Bitmap pict = new Bitmap(btm);
            pict = GrayScale(pict);
            pict = GlobBin(pict, r);
            int[] VecX = new int[btm.Width];
            int[] VecY = new int[btm.Height];
            for (int i = 0; i < (btm.Width > btm.Height ? btm.Width : btm.Height); i++)
            {
                if (i < btm.Width)
                {
                    VecX[i] = 0;
                }
                if (i < btm.Height)
                {
                    VecY[i] = 0;
                }
            }
            for (int x = 0; x < pict.Size.Width; x++)
            {
                for (int y = 0; y < pict.Size.Height; y++)
                {
                    if (pict.GetPixel(x, y).R == 0)
                    {
                        VecX[x]++;
                        VecY[y]++;
                    }
                }
            }

            PointCollection pointX = CreateHistogramPoints(VecX, 0);
            PointCollection pointY = CreateHistogramPoints(VecY, 1);
            Tuple<Bitmap, PointCollection, PointCollection> tuple = new Tuple<Bitmap, PointCollection, PointCollection>(pict, pointX, pointY);
            return tuple;
        }

        public static Tuple<Bitmap, PointCollection, PointCollection, PointCollection> Projection(Bitmap btm)
        {
            Bitmap pict = new Bitmap(btm);
            int[] VecR = new int[255];
            int[] VecG = new int[255];
            int[] VecB = new int[255];
            for (int i = 0; i < 255; i++)
            {
                VecR[i] = 0;
                VecG[i] = 0;
                VecB[i] = 0;
            }
            for (int x = 0; x < pict.Size.Width; x++)
            {
                for (int y = 0; y < pict.Size.Height; y++)
                {
                    var pix = pict.GetPixel(x, y);
                    VecR[pix.R]++;
                    VecG[pix.G]++;
                    VecB[pix.B]++;
                }
            }

            PointCollection pointR = CreateProjectionPoints(VecR);
            PointCollection pointG = CreateProjectionPoints(VecG);
            PointCollection pointB = CreateProjectionPoints(VecB);
            Tuple<Bitmap, PointCollection, PointCollection, PointCollection> tuple = new Tuple<Bitmap, PointCollection, PointCollection, PointCollection>(pict, pointR, pointG, pointB);
            return tuple;
        }

        private static PointCollection CreateProjectionPoints(int[] values)
        {
            int max = values.Max();
            PointCollection points = new PointCollection();

            points.Add(new System.Windows.Point(0, max));
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new System.Windows.Point(i, max - values[i]));
            }
            points.Add(new System.Windows.Point(values.Length - 1, max));

            return points;
        }

        private static PointCollection CreateHistogramPoints(int[] values, int flag)
        {
            int max = values.Max();
            PointCollection points = new PointCollection();

            if (flag == 0)
            {
                points.Add(new System.Windows.Point(0, max));
                for (int i = 0; i < values.Length; i++)
                {
                    points.Add(new System.Windows.Point(i, max - values[i]));
                }
                points.Add(new System.Windows.Point(values.Length - 1, max));
            }
            else
            {
                points.Add(new System.Windows.Point(0, 0));
                for (int i = 0; i < values.Length; i++)
                {
                    points.Add(new System.Windows.Point(values[i], i));
                }
                points.Add(new System.Windows.Point(0, values.Length - 1));
            }
            return points;
        }

        private static int FromInterval(int col)
        {
            if (col > 255)
                return 255;
            if (col < 0)
                return 0;
            else
                return col;
        }

        public static System.Windows.Media.ImageSource ToBitmapSource(Bitmap p_bitmap)
        {
            IntPtr hBitmap = p_bitmap.GetHbitmap();
            System.Windows.Media.ImageSource wpfBitmap;
            try
            {
                wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                //p_bitmap.Dispose();
            }
            return wpfBitmap;
        }
    }
}
