using System;
using System.Windows.Controls;

namespace Duck_Hunt
{
    public static class ExtraMethods
    {
        public static Tuple<int, int> Add(this Tuple<int, int> a, Tuple<int, int> b)
        {
            return Tuple.Create(a.Item1 + b.Item1, a.Item2 + b.Item2);
        }

        // Simple sum function for tuples, this is used to calculate the resultant position after an offset operation in Sprite.Move.

        public static bool CheckPixelValues(this Image i, Tuple<int, int> point)
            // Did not feel like this needed to be included in the SpriteSheet class
            // Also, it could have more uses than just cropping, so I left it as static.
        {
            // Validate any given point against an image.
            // That is, check that it's not outside boundaries
            // Assumes item1 in tuple is X co-ord of point, and item2 is Y co-ord

            if ((point.Item1 > i.Width) && (point.Item1 >= 0))
                return false;

            if ((point.Item2 > i.Height) && (point.Item2 >= 0))
                return false;

            return true; // Returning in if statements will automatically prevent this block here from executing.
            // So basically, this means 'if nothing's wrong, go ahead and return true'
        }
    }
}