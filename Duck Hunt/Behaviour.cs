using System;
using System.Collections.Generic;
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
            //AISprite entity = Parents[sender as Timer];
            //entity.Move(Tuple.Create(1, 1));
        }
    }
}
