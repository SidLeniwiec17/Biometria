using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public static class Mask
    {
        public static int[][] GetMask(int index)
        {
            switch (index)
            {
                case 0:
                    return Square();
                case 1:
                    return Cross();
                case 2:
                    return Horizontal();
                case 3:
                    return Vertical();
                case 4:
                    return Top();
                case 5:
                    return Bottom();
                case 6:
                    return Left();
                case 7:
                    return Right();
            }
            return Cross();
        }

        public static int[][] Square()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 1, 1, 1 };
            mask[1] = new int[] { 1, 1, 1 };
            mask[2] = new int[] { 1, 1, 1 };
            return mask;
        }

        public static int[][] Cross()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 1, 0 };
            mask[1] = new int[] { 1, 1, 1 };
            mask[2] = new int[] { 0, 1, 0 };
            return mask;
        }

        public static int[][] Horizontal()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 0, 0 };
            mask[1] = new int[] { 1, 1, 1 };
            mask[2] = new int[] { 0, 0, 0 };
            return mask;
        }

        public static int[][] Vertical()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 1, 0 };
            mask[1] = new int[] { 0, 1, 0 };
            mask[2] = new int[] { 0, 1, 0 };
            return mask;
        }

        public static int[][] Top()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 1, 0 };
            mask[1] = new int[] { 0, 1, 0 };
            mask[2] = new int[] { 0, 0, 0 };
            return mask;
        }

        public static int[][] Bottom()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 0, 0 };
            mask[1] = new int[] { 0, 1, 0 };
            mask[2] = new int[] { 0, 1, 0 };
            return mask;
        }

        public static int[][] Left()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 0, 0 };
            mask[1] = new int[] { 1, 1, 0 };
            mask[2] = new int[] { 0, 0, 0 };
            return mask;
        }

        public static int[][] Right()
        {
            int[][] mask = new int[3][];
            mask[0] = new int[] { 0, 0, 0 };
            mask[1] = new int[] { 0, 1, 1 };
            mask[2] = new int[] { 0, 0, 0 };
            return mask;
        }
    }
}
