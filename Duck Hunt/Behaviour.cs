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
        public static Dictionary<Timer, AISprite> Parents = new Dictionary<Timer, AISprite>();

        public static void Duck(object sender, ElapsedEventArgs e)
        {
            AISprite entity = Parents[sender as Timer];

            //if (entity.frames != null)
            //{
                if (entity.counter > entity.aTimer.Interval)
                {
                    entity.counter = 0;

                    entity.spriteIndex = entity.spriteIndex + 1;

                    if (entity.spriteIndex >= entity.frames.Count)
                    {
                        entity.spriteIndex = 0;
                    }

                    Application.Current?.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        entity.Img.Source = entity.frames[entity.spriteIndex].Source;
                    }));
                
                }
            //}

            entity.Move(Tuple.Create(1, 1));
            entity.counter++;
        }
    }
}
