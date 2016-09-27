using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Duck_Hunt
{
    public static class Behaviour
    {
        public static int w = 769; // So this would normally be bad practice but the window is a fixed size. I have disallowed resizing it.
        public static int h = 721; // Having defs here makes it easier to evaluate border conditions
        // because I don't have to access the MainWindow instance and deal with threads, it should also be faster.
        public static int DuckSpeed = 3;


        public static Dictionary<Timer, AISprite> Parents = new Dictionary<Timer, AISprite>();

        public static void Duck(object sender, ElapsedEventArgs e)
        {
            AISprite entity = Parents[sender as Timer]; // we don't need any if(frames != null); this function is for a duck which is always animated.
            if (entity.counter > 8)
            {
                entity.counter = 0;

                entity.spriteIndex = entity.spriteIndex + entity.spriteIncrement;

                // The aim of this code is to produce a sequence to loop through sprites.
                // Say we had 4 sprites, we want a pattern of: 
                // 0, 1, 2, 3, 2, 1, 0
                // This is because many of the animations in Duck Hunt (which I have a file rip for as the assets) rely on using earlier frames
                // to create a perfect loop.

                // The following if statements create this sequence by preventing the index from causing out of range errors, such as if it
                // were to go to -1 or 7 in the case of a list of last index 6. 

                if (entity.spriteIndex >= entity.frames.Count-1 && entity.spriteIncrement > 0)
                    { entity.spriteIncrement = -1; }
                if (entity.spriteIndex <= 0 && entity.spriteIncrement < 0)
                    { entity.spriteIncrement = 1; }

                // Reverse the direction. since we know if it's too high or low, we can say for certain what we want it to be
                // instead of just multiplying by -1. That should help with reliability and clarity.


                Application.Current?.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background, 
                    new Action(() => { entity.Img.Source = entity.frames[entity.spriteIndex].Source; })); // Again, access new threads to allow us to fiddle with UI elements.

                entity.Instance?.InvalidateVisual(); // Force redraw just in case.

                
            }
        

            entity.Move(entity.MovementDirection); // The Move() method of Sprite already invokes a new thread, so no need for that here.

            /* The below is currently just example AI behaviour. It will be improved soon */

            /*if (entity.Position.Item2 < 0 || entity.Position.Item2 + entity.Img.ActualWidth > 450) // check if we hit the right wall of the window. We're only moving on X for now,
                { entity.MovementDirection = Tuple.Create(entity.MovementDirection.Item1, entity.MovementDirection.Item2 *-1); } // so that's all that's needed.
            if (entity.Position.Item1 < 0 || entity.Position.Item1 + entity.Img.ActualWidth > w)
                { entity.MovementDirection = Tuple.Create(entity.MovementDirection.Item1 * -1, entity.MovementDirection.Item2); } // Same goes if we hit the left wall.*/

            if (entity.Position.Item2 < 0) { entity.MovementDirection = Tuple.Create(entity.MovementDirection.Item1, DuckSpeed); }
            if (entity.Position.Item2 > 400) { entity.MovementDirection = Tuple.Create(entity.MovementDirection.Item1, -DuckSpeed); }

            if (entity.Position.Item1 < 0) { entity.MovementDirection = Tuple.Create(DuckSpeed, entity.MovementDirection.Item2); }
            if (entity.Position.Item1 > w - entity.Img.ActualWidth) { entity.MovementDirection = Tuple.Create(-DuckSpeed, entity.MovementDirection.Item2); }

            


            entity.counter++; // Finally, increase the counter (for operations above such as changing the sprite occasionally
        }
    }
}
