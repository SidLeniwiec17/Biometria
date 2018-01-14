using System;
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

        public static Bitmap DoPreprocessingThings(Bitmap originalBitmap, int[][] maskMatrix)
        {
            Bitmap tempPict = new Bitmap(Methods.Contrast(originalBitmap, 2.0f));
            tempPict = Methods.RomoveBorder(tempPict);
            tempPict = Methods.Opening(tempPict, maskMatrix);
            int counter = 1;
            while (counter > 0)
            {
                var pup = Methods.DoErosionPupil(tempPict);
                tempPict = pup.Item1;
                counter = pup.Item2;
            }
            return tempPict;
        }

        public static Bitmap KMM(Bitmap originalBitmap)
        {
            Bitmap tempPict = new Bitmap(originalBitmap);
            int[][] map = new int[tempPict.Size.Width][];

            bool isFinished = false;
            int pixelsRemoved = 0;


            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                map[x] = new int[tempPict.Size.Height];
            }

            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    System.Drawing.Color oldColour = tempPict.GetPixel(x, y);
                    map[x][y] = oldColour.R == Color.Black.R ? 1 : 0;
                }
            }

            while (isFinished == false)
            {
                pixelsRemoved = 0;

                for (int x = 1; x < tempPict.Size.Width - 1; x++)
                {
                    for (int y = 1; y < tempPict.Size.Height - 1; y++)
                    {
                        if (map[x][y] != 0 && IsEdgePixel(GetMask(map, x, y)))
                        {
                            map[x][y] = 2;
                        }
                    }
                }


                for (int x = 1; x < tempPict.Size.Width - 1; x++)
                {
                    for (int y = 1; y < tempPict.Size.Height - 1; y++)
                    {
                        if (map[x][y] != 0 && IsCornerPixel(GetMask(map, x, y)))
                        {
                            map[x][y] = 3;
                        }
                    }
                }


                for (int x = 1; x < tempPict.Size.Width - 1; x++)
                {
                    for (int y = 1; y < tempPict.Size.Height - 1; y++)
                    {
                        if (map[x][y] != 0 && HasPixelAdjacentNeighbours(GetMask(map, x, y)))
                        {
                            map[x][y] = 4;
                        }
                    }
                }

                for (int x = 1; x < tempPict.Size.Width - 1; x++)
                {
                    for (int y = 1; y < tempPict.Size.Height - 1; y++)
                    {
                        if (map[x][y] == 4)
                        {
                            map[x][y] = 0;
                            pixelsRemoved++;
                        }
                    }
                }

                bool smallFinished = false;
                int N = 2;

                while (smallFinished == false)
                {
                    for (int x = 0; x < tempPict.Size.Width; x++)
                    {
                        for (int y = 0; y < tempPict.Size.Height; y++)
                        {
                            if (map[x][y] == N)
                            {
                                if (DoDeletePixel(GetMask(map, x, y)))
                                {
                                    map[x][y] = 0;
                                    pixelsRemoved++;
                                }
                                else
                                {
                                    map[x][y] = 1;
                                }
                            }
                        }
                    }
                    if (N == 3)
                    {
                        smallFinished = true;
                    }
                    else
                    {
                        N = 3;
                    }

                }

                Console.WriteLine("Pixels removed : " + pixelsRemoved);
                if (pixelsRemoved == 0)
                {
                    isFinished = true;
                }
                //isFinished = IsOnePixWidh(map);
            }

            for (int x = 0; x < tempPict.Size.Width; x++)
            {
                for (int y = 0; y < tempPict.Size.Height; y++)
                {
                    Color color = map[x][y] == 0 ? System.Drawing.Color.White : System.Drawing.Color.Black;
                    tempPict.SetPixel(x, y, color);
                }
            }
            return tempPict;
        }

        public static bool IsOnePixWidh(int[][] picture)
        {
            return true;
        }

        public static int[][] GetMask(int[][] picture, int x, int y)
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 0, 0 };
            mask[1] = new int[] { 0, 0, 0 };
            mask[2] = new int[] { 0, 0, 0 };
            int ii = 0;
            for (int i = x - 1; i < x + 2; i++, ii++)
            {
                int jj = 0;
                for (int j = y - 1; j < y + 2; j++, jj++)
                {
                    mask[ii][jj] = picture[i][j];
                }
            }
            return mask;
        }

        public static bool IsEdgePixel(int[][] mask)
        {
            if (mask[1][1] != 0)
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (mask[x][y] == 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsCornerPixel(int[][] mask)
        {
            if (mask[1][1] != 0)
            {
                int whitesCounter = 0;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (mask[x][y] == 0)
                        {
                            whitesCounter++;
                        }
                    }
                }
                if (whitesCounter == 1 && (mask[0][0] == 0 || mask[2][0] == 0 || mask[0][2] == 0 || mask[2][2] == 0))
                {
                    return true;
                }
            }
            return false;
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

        public static bool HasPixelAdjacentNeighbours(int[][] mask)
        {
            if (mask[1][1] != 0)
            {
                int[] neigh = new int[] {
                    mask[0][0] , mask[1][0], mask[2][0], mask[2][1],
                    mask[2][2], mask[1][2], mask[0][2], mask[0][1]
                };
                int counter = 0;
                bool prev = false;
                for (int i = 0; i < neigh.Length; i++)
                {
                    if (neigh[i] != 0 && prev == true)
                    {
                        counter++;
                        if (counter > 4)
                        {
                            return false;
                        }
                    }
                    else if (neigh[i] == 0 && prev == true)
                    {
                        prev = false;
                        if (counter >= 2 && counter <= 4)
                        {
                            return true;
                        }
                        counter = 0;
                    }
                    else if (neigh[i] != 0 && prev == false)
                    {
                        counter = 1;
                        prev = true;
                    }
                }
                if (counter >= 2 && counter <= 4)
                {
                    return true;
                }
            }
            return false;
        }

        public static int CalculatePixelWeight(int[][] mask)
        {
            int[][] wages = new int[3][];
            wages[0] = new int[] { 128, 1, 2 };
            wages[1] = new int[] { 64, 0, 4 };
            wages[2] = new int[] { 32, 16, 8 };
            int sum = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        break;
                    }
                    else if (mask[x][y] != 0)
                    {
                        sum += wages[x][y];
                    }
                }
            }
            return sum;
        }

        public static bool DoDeletePixel(int[][] mask)
        {
            int[] deletionTable = new int[]{3,5,7,12,13,14,15,20,21,22,23,28,29,30,
                31,48,52,53,54,55,56,60,61,62,63,65,67,69,71,77,79,80,81,83,84,85,
                86,87,88,89,91,92,93,94,95,97,99,101,103,109,111,112,113,115,116,
                117,118,119,120,121,123,124,125,126,127,131,133,135,141,143,149,
                151,157,159,181,183,189,191,192,193,195,197,199,205,207,208,209,
                211,212,213,214,215,216,217,219,220,221,222,223,224,225,227,229,
                231,237,239,240,241,243,244,245,246,247,248,249,251,252,253,254,255
            };
            int sum = CalculatePixelWeight(mask);

            for (int i = 0; i < deletionTable.Length; i++)
            {
                if (sum == deletionTable[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}