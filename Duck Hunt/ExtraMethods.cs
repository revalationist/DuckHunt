using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Duck_Hunt
{
    public static class ExtraMethods
    {
        public static Tuple<int, int> TupleSum(Tuple<int, int> a, Tuple<int, int> b)
        {
            return new Tuple<int, int>(
                a.Item1 + b.Item1,
                a.Item2 + b.Item2
            );
        }

        public static Tuple<int, int> TupleDiff(Tuple<int, int> a, Tuple<int, int> b)
        {
            return new Tuple<int, int>(
                a.Item1 - b.Item1,
                a.Item2 - b.Item2
            );
        }

        public static bool CheckPixelValues(this Image i, Tuple<int, int> point)
        {

            // Validate any given point against an image.
            // That is, check that it's not outside boundaries
            // Assumes item1 in tuple is X co-ord of point, and item2 is Y co-ord

            if (point.Item1 > i.Width && point.Item1 >= 0)
            {
                return false;
            }
            
            if (point.Item2 > i.Height && point.Item2 >= 0)
            {
                return false;
            }
            return true;
        }


    }
}
