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
