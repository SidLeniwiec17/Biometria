﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Lab3
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
                    var value = (0.2 * (double)oldColour.R) + (0.7 * (double)oldColour.G) + (0.1 * (double)oldColour.B);
                    newColor = System.Drawing.Color.FromArgb((int)value, (int)value, (int)value);
                    tempPict.SetPixel(x, y, newColor);
                }
            }

            return tempPict;
        }

        public static Tuple<Bitmap, int> GetGreyScaleTreshold(Bitmap btm, float scale = 1.0f)
        {
            double treshold = 0;
            double hw = btm.Height * btm.Width;
            Bitmap tempPict = new Bitmap(btm);
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    var value = (0.2 * (double)oldColour.R) + (0.7 * (double)oldColour.G) + (0.1 * (double)oldColour.B);
                    newColor = System.Drawing.Color.FromArgb((int)value, (int)value, (int)value);
                    tempPict.SetPixel(x, y, newColor);
                    treshold += (double)(value / hw);
                }
            }
            treshold += 5;
            treshold = (double)treshold * scale;

            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    newColor = oldColour.R >= (int)treshold ? System.Drawing.Color.White : System.Drawing.Color.Black;
                    tempPict.SetPixel(x, y, newColor);
                }
            }
            return new Tuple<Bitmap, int>(tempPict, (int)treshold);
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

        private static int FromInterval(int col)
        {
            if (col > 255)
                return 255;
            if (col < 0)
                return 0;
            else
                return col;
        }

        public static Bitmap Opening(Bitmap btm, int[][] mask)
        {
            var binaryBitmap = GetGreyScaleTreshold(btm);
            Bitmap oriBtm = binaryBitmap.Item1;
            Bitmap tempPict = new Bitmap(binaryBitmap.Item1);

            tempPict = DoErosion(tempPict, mask);
            return DoDilation(tempPict, mask);
        }

        public static Bitmap DoErosion(Bitmap oriBtm, int[][] mask)
        {
            int maskHalf = (mask.Length - 1) / 2;
            Bitmap tempPict = new Bitmap(oriBtm);
            int counter = 0;
            for (int x = 0; x < oriBtm.Size.Width; x++)
            {
                for (int y = 0; y < oriBtm.Size.Height; y++)
                {
                    System.Drawing.Color oldColour = oriBtm.GetPixel(x, y);
                    if ((mask[maskHalf][maskHalf] == 1 && oldColour.R == 0))
                    {
                        bool doesItFit = true;
                        for (int x2 = -maskHalf; x2 < maskHalf; x2++)
                        {
                            for (int y2 = -maskHalf; y2 < maskHalf; y2++)
                            {
                                if (x + x2 >= 0 && y + y2 >= 0 && x + x2 < oriBtm.Width && y + y2 < oriBtm.Height)
                                {
                                    if (mask[x2 + maskHalf][y2 + maskHalf] == 1)
                                    {
                                        System.Drawing.Color currCol = oriBtm.GetPixel(x + x2, y + y2);
                                        if (currCol.R != 0)
                                        {
                                            doesItFit = false;
                                        }
                                    }
                                }
                            }
                        }
                        if (!doesItFit)
                        {
                            counter++;
                            tempPict.SetPixel(x, y, System.Drawing.Color.White);
                        }
                    }
                }
            }
            Console.WriteLine("DoErosion :" + counter + " / " + oriBtm.Size.Width * oriBtm.Size.Height + " pixels changed");
            return tempPict;
        }

        public static Bitmap DoDilation(Bitmap oriBtm, int[][] mask)
        {
            int maskHalf = (mask.Length - 1) / 2;
            Bitmap tempPict = new Bitmap(oriBtm);
            int counter = 0;
            for (int x = 0; x < oriBtm.Size.Width; x++)
            {
                for (int y = 0; y < oriBtm.Size.Height; y++)
                {
                    System.Drawing.Color oldColour = oriBtm.GetPixel(x, y);
                    if ((mask[maskHalf][maskHalf] == 1 && oldColour.R == 0) || (mask[maskHalf][maskHalf] == 0 && oldColour.R == 255))
                    {
                        for (int x2 = -maskHalf; x2 < maskHalf; x2++)
                        {
                            for (int y2 = -maskHalf; y2 < maskHalf; y2++)
                            {
                                if (x + x2 >= 0 && y + y2 >= 0 && x + x2 < oriBtm.Width && y + y2 < oriBtm.Height)
                                {
                                    if (mask[x2 + maskHalf][y2 + maskHalf] == 1)
                                    {
                                        System.Drawing.Color currCol = oriBtm.GetPixel(x + x2, y + y2);
                                        if (currCol.R != 0)
                                        {
                                            counter++;
                                        }
                                        tempPict.SetPixel(x + x2, y + y2, System.Drawing.Color.Black);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("DoDilation :" + counter + " / " + oriBtm.Size.Width * oriBtm.Size.Height + " pixels changed");
            return tempPict;
        }

        public static Tuple<Bitmap, int> DoErosionPupil(Bitmap btm)
        {
            Bitmap tempPict = new Bitmap(btm);
            int counter = 0;
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    System.Drawing.Color oldColour = btm.GetPixel(x, y);
                    if (oldColour.R == 0)
                    {
                        int neighCounter = 0;

                        for (int x2 = -1; x2 <= 1; x2++)
                        {
                            for (int y2 = -1; y2 <= 1; y2++)
                            {
                                if (x + x2 >= 0 && x + x2 < btm.Width && y + y2 >= 0 && y + y2 < btm.Height)
                                {
                                    var col = btm.GetPixel(x + x2, y + y2);
                                    if (col.R == 0)
                                    {
                                        neighCounter++;
                                    }
                                }
                            }
                        }

                        if (neighCounter < 3)
                        {
                            counter++;
                            tempPict.SetPixel(x, y, System.Drawing.Color.White);
                        }
                    }
                }
            }
            Console.WriteLine("DoErosionPupil: " + counter + " / " + btm.Width * btm.Height + " pixels removed");
            return new Tuple<Bitmap, int>(tempPict, counter);
        }
    }
}