using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Duck_Hunt
{
    public static class Behaviour
    {
        public static Dictionary<Image, AISprite> AIByImage = new Dictionary<Image, AISprite>();

        public static int Width = 769;
        // So this would normally be bad practice but the window is a fixed size. I have disallowed resizing it.

        public static int Height = 721; // Having defs here makes it easier to evaluate border conditions
        // because I don't have to access the MainWindow instance and deal with threads, it should also be faster.
        public static readonly int DuckSpeed = 9;

        public static Dictionary<Timer, AISprite> Parents = new Dictionary<Timer, AISprite>();

        public static void DuckFlyUp(object sender, ElapsedEventArgs e)
        {
            AISprite entity = Parents[sender as Timer];
            // we don't need any if(frames != null); this function is for a duck which is always animated.
            if (entity.Counter > 1)
            {
                entity.Counter = 0;

                entity.SpriteIndex = entity.SpriteIndex + entity.SpriteIncrement;

                // The aim of this code is to produce a sequence to loop through sprites.
                // Say we had 4 sprites, we want a pattern of: 
                // 0, 1, 2, 3, 2, 1, 0
                // This is because many of the animations in Duck Hunt (which I have a file rip for as the assets) rely on using earlier frames
                // to create a perfect loop.

                // The following if statements create this sequence by preventing the index from causing out of range errors, such as if it
                // were to go to -1 or 7 in the case of a list of last index 6. 

                if ((entity.SpriteIndex >= 3) && (entity.SpriteIncrement > 0))
                    entity.SpriteIncrement = -1;
                if ((entity.SpriteIndex <= 0) && (entity.SpriteIncrement < 0))
                    entity.SpriteIncrement = 1;

                // Reverse the direction. since we know if it's too high or low, we can say for certain what we want it to be
                // instead of just multiplying by -1. That should help with reliability and clarity.


                Application.Current?.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => { entity.Img.Source = entity.Frames[entity.SpriteIndex].Source; }));
                // Again, access new threads to allow us to fiddle with UI elements.

                entity.Instance?.InvalidateVisual(); // Force redraw just in case.
            }


            entity.Move(entity.MovementDirection);
            // The Move() method of Sprite already invokes a new thread, so no need for that here.

            /* The below is currently just example AI behaviour. It will be improved soon */

            // Entity AI behaviour to make it bounce off walls:

            if (entity.Position.Item1 < 0)
                entity.MovementDirection = Tuple.Create(DuckSpeed, entity.MovementDirection.Item2);
            if (entity.Position.Item1 > Width - entity.Img.ActualWidth)
                entity.MovementDirection = Tuple.Create(-DuckSpeed, entity.MovementDirection.Item2);

            if (entity.Position.Item2 < 0)
                entity.MovementDirection = Tuple.Create(entity.MovementDirection.Item1, DuckSpeed);
            if (entity.Position.Item2 > 400)
                entity.MovementDirection = Tuple.Create(entity.MovementDirection.Item1, -DuckSpeed);

            entity.Counter++;
            // Finally, increase the counter (for operations above such as changing the sprite occasionally
        }

        public static int DuckDeath(AISprite entity)
        {
            entity.Timer.Elapsed -= new ElapsedEventHandler(Behaviour.DuckFlyUp);
            entity.Timer.Elapsed += new ElapsedEventHandler(Behaviour.DuckDie);
            return 0;
        }

        public static void DuckDie(object sender, ElapsedEventArgs e)
        {

        }
    }

    
}

/* The actual behaviour: they fly up for a random amount of time, then start flying side to side, bobbing in the process.
 * Plan: have it fly up for a randomly generated number of ticks, then generate a random sine/cosine graph for it to follow as it flies.
 * If a random number is met while flying sideways, it will divert and go in the other direction */