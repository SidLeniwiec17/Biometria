using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab2
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

        public static Tuple<Bitmap, int> GetGreyScaleTreshold(Bitmap btm, int tresh = -1)
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
            if (tresh != -1)
            {
                treshold = (double)tresh;
            }
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);
                    newColor = oldColour.R >= treshold ? System.Drawing.Color.White : System.Drawing.Color.Black;
                    tempPict.SetPixel(x, y, newColor);
                }
            }
            return new Tuple<Bitmap, int>(tempPict, (int)treshold);
        }

        public static Bitmap Dilation(Bitmap btm, int[][] mask)
        {
            var binaryBitmap = GetGreyScaleTreshold(btm);
            Bitmap oriBtm = binaryBitmap.Item1;
            Bitmap tempPict = new Bitmap(binaryBitmap.Item1);

            return DoDilation(tempPict, mask);
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

        public static Bitmap Erosion(Bitmap btm, int[][] mask, int tresh = -1)
        {
            var binaryBitmap = GetGreyScaleTreshold(btm, tresh);
            Bitmap oriBtm = binaryBitmap.Item1;
            Bitmap tempPict = new Bitmap(binaryBitmap.Item1);

            return DoErosion(tempPict, mask);
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

        public static Bitmap Opening(Bitmap btm, int[][] mask)
        {
            var binaryBitmap = GetGreyScaleTreshold(btm);
            Bitmap oriBtm = binaryBitmap.Item1;
            Bitmap tempPict = new Bitmap(binaryBitmap.Item1);

            tempPict = DoErosion(tempPict, mask);
            return DoDilation(tempPict, mask);
        }

        public static Bitmap Closing(Bitmap btm, int[][] mask)
        {
            var binaryBitmap = GetGreyScaleTreshold(btm);
            Bitmap oriBtm = binaryBitmap.Item1;
            Bitmap tempPict = new Bitmap(binaryBitmap.Item1);

            tempPict = DoDilation(tempPict, mask);
            return DoErosion(tempPict, mask);
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

        public static Bitmap RomoveBorder(Bitmap btm)
        {
            Bitmap tempPict = new Bitmap(btm);
            for (int x = 0; x < tempPict.Width; x++)
            {
                tempPict.SetPixel(x, 0, System.Drawing.Color.White);
                tempPict.SetPixel(x, tempPict.Height - 1, System.Drawing.Color.White);
            }

            for (int y = 0; y < tempPict.Height; y++)
            {
                tempPict.SetPixel(0, y, System.Drawing.Color.White);
                tempPict.SetPixel(btm.Width - 1, y, System.Drawing.Color.White);
            }

            return tempPict;
        }

        public static Bitmap ChangeColor(Bitmap btm, System.Drawing.Color old, System.Drawing.Color newCol)
        {
            Bitmap tempPict = new Bitmap(btm);
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour, newColor;
                    oldColour = tempPict.GetPixel(x, y);

                    if (oldColour.R == old.R)
                    {
                        newColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        newColor = newCol;
                    }
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

        public static Bitmap ProjectionNorm(Bitmap btm)
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

        public static Bitmap ProjectionEQ(Bitmap btm)
        {
            Bitmap tempPict = new Bitmap(btm);
            tempPict = GrayScale(tempPict);
            int[] VecB = new int[256];
            double[] D = new double[256];
            double[] LUT = new double[256];
            for (int i = 0; i < 256; i++)
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
            for (int i = 0; i < 256; i++)
            {
                currSum += VecB[i];
                D[i] = currSum / totPixes;
                LUT[i] = ((D[i] - D[0]) / (1.0 - D[0])) * (256 - 1);
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

        public static Tuple<PointCollection, PointCollection> Projection(Bitmap btm, int r)
        {
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
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    if (btm.GetPixel(x, y).R == 0)
                    {
                        VecX[x]++;
                        VecY[y]++;
                    }
                }
            }

            PointCollection pointX = CreateProjectionPoints(VecX, 0);
            PointCollection pointY = CreateProjectionPoints(VecY, 1);
            Tuple<PointCollection, PointCollection> tuple = new Tuple<PointCollection, PointCollection>(pointX, pointY);
            return tuple;
        }

        private static PointCollection CreateProjectionPoints(int[] values, int flag)
        {
            PointCollection points = new PointCollection();

            if (flag == 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    points.Add(new System.Windows.Point(i, values[i]));
                }
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    points.Add(new System.Windows.Point(values[i], i));
                }
            }
            return points;
        }

        /*public static List<System.Drawing.Point> GetPupilCoords(Bitmap tempPict)
        {
            List<System.Drawing.Point> pupilPoints = new List<System.Drawing.Point>();
            Tuple<PointCollection, PointCollection> projection = Projection(tempPict, 0); // X, Y
            PointCollection projectionX = GetDropsInProjection(projection.Item1);
            PointCollection projectionY = GetDropsInProjection(projection.Item2);
            return pupilPoints;
        }

        public static PointCollection GetDropsInProjection(PointCollection points)
        {
            double max = 0;
            double totalBlack = 0;
            foreach (var p in points)
            {
                totalBlack += p.Y;
                if (p.Y > max)
                {
                    max = p.Y;
                }
            }
            double mean = totalBlack / points.Count;
            //int diff = ((int)(max - mean)) > (int)mean ? ((int)(max - mean)) : (int)mean;
            int diff = (int)mean;

            List<System.Windows.Point> jumps = new List<System.Windows.Point>();
            List<System.Windows.Point> drops = new List<System.Windows.Point>();
            bool lastDrop = false;
            for (int i = 0; i < points.Count - 1; i++)
            {
                
                if (Math.Abs(points[i].Y - points[i + 1].Y) > diff)
                {
                    if (points[i].Y > points[i + 1].Y)
                    {
                        lastDrop = true;
                        break;
                    }
                    else
                    {
                        lastDrop = false;
                        break;
                    }
                }
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (Math.Abs(points[i].Y - points[i + 1].Y) > diff)
                {
                    if (points[i].Y > points[i + 1].Y && !lastDrop)
                    {
                        drops.Add(points[i + 1]);
                        lastDrop = true;
                    }
                    else if (lastDrop)
                    {
                        jumps.Add(points[i]);
                        lastDrop = false;
                    }
                }
            }
            if (jumps.Count > 0 && drops.Count > 0)
            {
                bool isJumpFirst = jumps[0].X > drops[0].X ? true : false;
                int jumpIndx = 0;
                int dropIndx = 0;

                for (int i = 0; i < points.Count; i++)
                {
                    if (i < jumps[jumpIndx].X && isJumpFirst)
                    {
                        points[i] = new System.Windows.Point(i, 0);
                    }
                    else if (i == jumps[jumpIndx].X && isJumpFirst)
                    {
                        jumpIndx++;
                        isJumpFirst = false;
                    }
                    else if (i == drops[dropIndx].X && !isJumpFirst)
                    {
                        isJumpFirst = true;
                    }
                }
            }
            return points;
        }*/

        public static Bitmap RemoveSingleNoises(Bitmap btm)
        {
            Bitmap tempPict = new Bitmap(btm);
            int counter = 0;
            int maskHalf = 1;
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    System.Drawing.Color oldColour = btm.GetPixel(x, y);
                    if (oldColour.R == 0)
                    {
                        bool isAlone = true;

                        for (int x2 = -1; x2 <= 1; x2++)
                        {
                            for (int y2 = -1; y2 <= 1; y2++)
                            {
                                if (x + x2 >= 0 && x + x2 < btm.Width && y + y2 >= 0 && y + y2 < btm.Height)
                                {
                                    var col = btm.GetPixel(x + x2, y + y2);
                                    if (col.R == 0)
                                    {
                                        isAlone = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (isAlone)
                        {
                            tempPict.SetPixel(x, y, System.Drawing.Color.White);
                        }
                    }
                }
            }
            Console.WriteLine(counter + " / " + btm.Width * btm.Height + " pixels removed");
            return tempPict;
        }

        private static Bitmap FloodFill(Bitmap bmp, System.Drawing.Point pt, System.Drawing.Color targetColor, System.Drawing.Color replacementColor)
        {
            Stack<System.Drawing.Point> pixels = new Stack<System.Drawing.Point>();
            Bitmap tempPict = new Bitmap(bmp);
            pixels.Push(pt);

            while (pixels.Count > 0)
            {
                System.Drawing.Point a = pixels.Pop();
                if (a.X < tempPict.Width && a.X >= 0 &&
                        a.Y < tempPict.Height && a.Y >= 0)//make sure we stay within bounds
                {

                    if (tempPict.GetPixel(a.X, a.Y).R == targetColor.R)
                    {
                        tempPict.SetPixel(a.X, a.Y, replacementColor);
                        pixels.Push(new System.Drawing.Point(a.X - 1, a.Y));
                        pixels.Push(new System.Drawing.Point(a.X + 1, a.Y));
                        pixels.Push(new System.Drawing.Point(a.X, a.Y - 1));
                        pixels.Push(new System.Drawing.Point(a.X, a.Y + 1));
                    }
                }
            }
            return tempPict;
        }

        public static Bitmap Iris(Bitmap btm, int[][] mask)
        {
            Bitmap tempPict = new Bitmap(btm);
            tempPict = FindPupil(btm, mask);
            return tempPict;
        }

        public static Bitmap SobelEdgeDetect(Bitmap original)
        {
            Bitmap b = original;
            Bitmap bb = original;
            int width = b.Width;
            int height = b.Height;
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

            int[,] allPixR = new int[width, height];
            int[,] allPixG = new int[width, height];
            int[,] allPixB = new int[width, height];

            int limit = 128 * 128;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    allPixR[i, j] = b.GetPixel(i, j).R;
                    allPixG[i, j] = b.GetPixel(i, j).G;
                    allPixB[i, j] = b.GetPixel(i, j).B;
                }
            }

            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc, gc, bc;
            for (int i = 1; i < b.Width - 1; i++)
            {
                for (int j = 1; j < b.Height - 1; j++)
                {

                    new_rx = 0;
                    new_ry = 0;
                    new_gx = 0;
                    new_gy = 0;
                    new_bx = 0;
                    new_by = 0;
                    rc = 0;
                    gc = 0;
                    bc = 0;

                    for (int wi = -1; wi < 2; wi++)
                    {
                        for (int hw = -1; hw < 2; hw++)
                        {
                            rc = allPixR[i + hw, j + wi];
                            new_rx += gx[wi + 1, hw + 1] * rc;
                            new_ry += gy[wi + 1, hw + 1] * rc;

                            gc = allPixG[i + hw, j + wi];
                            new_gx += gx[wi + 1, hw + 1] * gc;
                            new_gy += gy[wi + 1, hw + 1] * gc;

                            bc = allPixB[i + hw, j + wi];
                            new_bx += gx[wi + 1, hw + 1] * bc;
                            new_by += gy[wi + 1, hw + 1] * bc;
                        }
                    }
                    if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                        bb.SetPixel(i, j, System.Drawing.Color.Black);

                    //bb.SetPixel (i, j, Color.FromArgb(allPixR[i,j],allPixG[i,j],allPixB[i,j]));
                    else
                        bb.SetPixel(i, j, System.Drawing.Color.White);
                }
            }
            return bb;

        }

        public static Tuple<System.Drawing.Point, int> HoughCircle(Bitmap btm)
        {
            Bitmap tempPict = new Bitmap(btm);
            int maxR = btm.Height /4;
            int[,,] A = new int[btm.Width, btm.Height, maxR];
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    for (int r = 0; r < maxR; r++)
                    {
                        A[x, y, r] = 0;
                    }
                }
            }


            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    if (btm.GetPixel(x, y).R == System.Drawing.Color.Black.R)
                    {
                        for (int r = 0; r < maxR; r++)
                        {
                            for (int t = 0; t < 360; t++)
                            {
                                var a = (int)(x - (r * Math.Cos(t * Math.PI / 180)));
                                var b = (int)(y - (r * Math.Sin(t * Math.PI / 180)));
                                if (a >= 0 && b >= 0 && a < btm.Width && b < btm.Height)
                                {
                                    A[a, b, r] += 1;
                                }
                            }
                        }
                    }
                }
            }

            int currMax = 0;
            int curX = 0, curY = 0, curR = 0;
            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    for (int r = 0; r < maxR; r++)
                    {
                        if (A[x, y, r] > currMax)
                        {
                            currMax = A[x, y, r];
                            curX = x;
                            curY = y;
                            curR = r;
                        }
                    }
                }
            }

            return new Tuple<System.Drawing.Point, int>(new System.Drawing.Point(curX, curY), curR);
        }

        public static Bitmap FindPupil(Bitmap btm, int[][] mask)
        {
            Bitmap tempPict = new Bitmap(btm);
            tempPict = ProjectionNorm(btm);
            tempPict = Erosion(tempPict, mask, 15);
            tempPict = RomoveBorder(tempPict);
            int counter = 1;
            while (counter > 0)
            {
                var pup = DoErosionPupil(tempPict);
                tempPict = pup.Item1;
                counter = pup.Item2;
            }
            tempPict = Dilation(tempPict, mask);
            tempPict = RomoveBorder(tempPict);
            tempPict = FloodFill(tempPict, new System.Drawing.Point(0, 0), System.Drawing.Color.White, System.Drawing.Color.FromArgb(100, 100, 100));
            tempPict = ChangeColor(tempPict, System.Drawing.Color.FromArgb(100, 100, 100), System.Drawing.Color.Black);
            tempPict = SobelEdgeDetect(tempPict);
            var Answ = HoughCircle(tempPict);

            try
            {
                tempPict.SetPixel(Answ.Item1.X, Answ.Item1.Y, System.Drawing.Color.Red);
                tempPict.SetPixel(Answ.Item1.X + 1, Answ.Item1.Y, System.Drawing.Color.Red);
                tempPict.SetPixel(Answ.Item1.X - 1, Answ.Item1.Y, System.Drawing.Color.Red);
                tempPict.SetPixel(Answ.Item1.X, Answ.Item1.Y + 1, System.Drawing.Color.Red);
                tempPict.SetPixel(Answ.Item1.X, Answ.Item1.Y - 1, System.Drawing.Color.Red);
            }
            catch (Exception ex) { }
            return tempPict;
        }
    }
}
