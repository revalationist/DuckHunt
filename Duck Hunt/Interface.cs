using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Duck_Hunt
{
    public class Interface
    {
        // Basic elements
        public GameImageObj AmmoGaugeBG;
        public GameImageObj HitBG;
        public GameImageObj ScoreBG;
        
        // Notifications
        public GameImageObj Pause;
        public GameImageObj FlyAway;

        // Icons
        public GameImageObj Bullet;
        public GameImageObj WhiteDuck;
        public GameImageObj[] Bullets;
        public GameImageObj Bullet1;
        public GameImageObj Bullet2;
        public GameImageObj Bullet3;

        // Static labels
        public GameImageObj Shot;

        public Interface(SpriteSheet src, Canvas parent)
        {
            HitBG = new GameImageObj(
                src.CropSpriteFrom(Tuple.Create(183, 615), Tuple.Create(351, 63)),
                parent)
            {
                Position = Tuple.Create(183, 585),
                Priority = 40000
            };

            AmmoGaugeBG = new GameImageObj(
                src.CropSpriteFrom(Tuple.Create(63, 615), Tuple.Create(87, 63)),
                parent)
            {
                Position = Tuple.Create(63, 585),
                Priority = 1337
            };

            Shot = new GameImageObj(
                src.CropSpriteFrom(Tuple.Create(174, 762), Tuple.Create(63, 18)),
                parent)
            {
                Position = Tuple.Create(75, 620),
                Priority = 1338
            };

            Bullet1 = new GameImageObj(src.CropSpriteFrom(Tuple.Create(177, 735), Tuple.Create(12, 21)), parent) { Position = Tuple.Create(78, 593), Priority = 1339 };
            Bullet2 = new GameImageObj(src.CropSpriteFrom(Tuple.Create(177, 735), Tuple.Create(12, 21)), parent) { Position = Tuple.Create(78+23, 593), Priority = 1340};
            Bullet3 = new GameImageObj(src.CropSpriteFrom(Tuple.Create(177, 735), Tuple.Create(12, 21)), parent) { Position = Tuple.Create(78+23+23, 593), Priority = 1341};

            Bullets = new GameImageObj[3]
            {
                Bullet1, Bullet2, Bullet3
            };

        }
    }
}
