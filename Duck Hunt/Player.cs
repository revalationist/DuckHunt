using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duck_Hunt
{
    public class Player
    {
        private int _bullets;
        public int Bullets
        {
            get { return _bullets; }
            set
            {
                _bullets = value;
                // Update UI
            }
        }

        private int _score;
        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                // Update UI
            }
        }

        private int _ducksShot;
        public int DucksShot
        {
            get { return _ducksShot; }
            set
            {
                _ducksShot = value;
            }
        }

        private int _round;
        public int Round
        {
            get { return _round; }
            set
            {
                _round = value;
            }
        }


        public Player()
        {
            this.Bullets = 3;
            this.DucksShot = 0;
            this.Round = 1;
            this.Score = 0;
        }
    }
}
