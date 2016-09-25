using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Duck_Hunt
{
    public static class Behaviour
    {
        public static Dictionary<Timer, AISprite> Parents = new Dictionary<Timer, AISprite>();

        public static void Duck(object sender, ElapsedEventArgs e)
        {
            AISprite entity = Parents[sender as Timer];

            if (entity.frames != null)
            {
                if (entity.counter > entity.aTimer.Interval*100)
                {
                    entity.counter = 0;
                    int spriteIndex = entity.frames.IndexOf(entity.Img);
                    spriteIndex++;

                    if (spriteIndex >= entity.frames.Count)
                    {
                        spriteIndex = 0;
                    }

                    entity.Img = entity.frames[spriteIndex];
                }
            }

            entity.Move(Tuple.Create(1, 1));
            entity.counter++;
        }
    }
}
