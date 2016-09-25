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
        public static int w = 769; // So this would normally be bad practice but the window is a fixed size
        public static int h = 721; // Having defs here makes it easier to evaluate border conditions
        // because I don't have to access the MainWindow instance and deal with threads

        public static Dictionary<Timer, AISprite> Parents = new Dictionary<Timer, AISprite>();

        public static void Duck(object sender, ElapsedEventArgs e)
        {
            AISprite entity = Parents[sender as Timer];
            if (entity.counter > entity.Timer.Interval)
            {
                entity.counter = 0;

                entity.spriteIndex = entity.spriteIndex + entity.spriteIncrement;

                if (entity.spriteIndex >= entity.frames.Count-1 && entity.spriteIncrement > 0)
                {
                    entity.spriteIncrement = -1;
                    
                }
                if (entity.spriteIndex <= 0 && entity.spriteIncrement < 0)
                {
                    entity.spriteIncrement = 1;
                   
                }
                


                Application.Current?.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    entity.Img.Source = entity.frames[entity.spriteIndex].Source;
                }));

                entity.Instance?.InvalidateVisual();

                
            }
        

            entity.Move(entity.MovementDirection);

            if (entity.Position.Item1 + entity.Img.ActualWidth > w)
            {
                entity.MovementDirection = Tuple.Create(-1, 0);
            }

            if (entity.Position.Item1 < 0)
            {
                entity.MovementDirection = Tuple.Create(1, 0);
            }
           

            entity.counter++;
        }
    }
}
