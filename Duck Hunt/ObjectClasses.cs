using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;
using System.Timers;

// Transformation methods and such to make my life easier.
// Scale factor 3

namespace Duck_Hunt
{
    /*interface GameImageInterface
    {
        void SetPosition(Tuple<int, int> newPos);
        Tuple<int, int> GetPosition();

        void SetSize(Tuple<int, int> newSize);
        Tuple<int, int> GetSize();
    }

    interface SpriteInterface : GameImageInterface
    {
        void Move(Tuple<int, int> offset);
        void Resize(Tuple<int, int> change);
    }*/
    
        // These interfaces were used to implement mandatory methods,
        // but the only mandatory methods were soon replaced by acessors.
        // Seeing as you can't define fields in an interface, they were deprecated.



    public abstract class GameImageObj
    {
        public MainWindow instance { get; private set; }
        public Image img { get; set; }
        public Canvas parent { get; set; }


        public int priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = priority + value;
                Panel.SetZIndex(this.img, priority);
            }
        }


        public Tuple<int, int> position
        {
            get
            {
                return position;
            }
            set
            {
                Thickness tmp = this.img.Margin;

                tmp.Left = value.Item1;
                tmp.Top = value.Item2;

                this.img.Margin = tmp;
            }

        }
  
        public Tuple<int, int> size
        {
            get
            {
                return new Tuple<int, int>((int)this.img.Width, (int)this.img.Height);
            }
            set
            {
                this.img.Width = value.Item1;
                this.img.Height = value.Item2;
            }
        }

        public GameImageObj(Image i, Canvas parent)
        {
            this.img = i;
            parent.Children.Add(img);
            Window instance = Window.GetWindow(parent);
        }
    }

    class SpriteSheet
    {
        protected Image img { get; private set; }
        public Tuple<int, int> size { get; private set; } // This is a spritesheet - there is no reason to modify size
        

        public Image CropSpriteFrom(Tuple<int, int> start, Tuple<int, int> end)
        {
            // Check that the numbers given are valid
            if (!img.CheckPixelValues(start))
            {
                throw new ArgumentException(string.Format("Attempted illegal crop of image -- image has width {0} and height {1} but starting point of the crop was {2}, {3}",
                    img.Width,
                    img.Height,

                    start.Item1,
                    start.Item2));
            }
            if (!img.CheckPixelValues(end))
            {
                throw new ArgumentException(string.Format("Attempted illegal crop of image -- image has width {0} and height {1} but ending point of the crop was {2}, {3}",
                    img.Width,
                    img.Height,

                    end.Item1,
                    end.Item2));
            }

            Image output = img;

            int offset1 = ExtraMethods.TupleDiff(start, end).Item1;
            int offset2 = ExtraMethods.TupleDiff(start, end).Item2;

            if (offset1 < 0 || offset2 < 0)
            {
                throw new ArgumentException(string.Format("Difference between tuples in crop operation was {0}, {1} -- at least one of these was below zero and was therefore invalid.", offset1, offset2));
            }


            RectangleGeometry clipGeometry = new RectangleGeometry();
            clipGeometry.Rect = new Rect(
                start.Item1, start.Item2,

                offset1, offset2
                );
            output.Clip = clipGeometry;

            return output;
        }

        public SpriteSheet(Image input)
        {
            this.img = input;
            size = new Tuple<int, int>((int)input.Width, (int)input.Height);
        }
    }

    public abstract class Sprite : GameImageObj
    {
        public MouseButtonEventHandler onClick
        {
            get
            {
                return this.onClick;
            }
            set
            {
                this.img.MouseDown -= this.onClick;
                this.img.MouseDown += value;
            }
        }
        public void Move(Tuple<int,int> offset)
        {
            Tuple<int, int> result = ExtraMethods.TupleSum(this.position, offset);
            this.position = result;
        }

        public void Resize(Tuple<int, int> change)
        {
            Tuple<int, int> result = ExtraMethods.TupleSum(this.size, change);
            if (result.Item1 > 0 && result.Item2 > 0)
            {
                this.size = (ExtraMethods.TupleSum(this.size, change));
            }
            else
            {
                throw new System.ArgumentException("Cannot set negative size", "Sprite.Resize", 
                    new System.ArithmeticException("Sum of existing size and change tuples had a resultant tuple in which at least 1 value was less than zero.") 
                    );
            }
        }

        public Sprite(Image i, Canvas parent) 
            : base(i, parent)
        {
            // do nothing, base class will handle it
        }

        public Sprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent) 
            : base(i, parent)
        {
            this.onClick = eventHandler;
        }

    }// This class is a bit useless because the AISprite class (which inherits from it) is used for any scenario in which this would be useful
    // That said, if I just had AISprite built on from GameImageObj it'd be less structured and readable.
    // It just so happens that in this particular game, Duck Hunt, there are no objects that have to move but in a simple way.
    // The ducks all need their own pathfinding and dead/alive states, and the dog needs animations.
    // If there was, say, an image that changed with user input in a non-complex fashion, the Sprite class could be used in lieu of an AISprite
    // as the logic there is not complicated enough to warrant a timer with modular AI functions and death functions.
    // 
    // In summary, it's abstract seeing as it's only used to derive the AISprite class, ever, due to the nature of the game.
    // That is, there are only two moving objects and both require complex AI so need custom methods.


    class AISprite : Sprite
    {

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            update();
        }

        Timer updateTimer = new Timer();
        

        public bool dead
        {
            get
            {
                return dead;
            }
            set
            {
                if (value == true) // if we're setting dead to true
                {
                    onDeath();
                    updateTimer.Enabled = false;
                }
            }
        }

        public Action onDeath { get; set; }
        public Action update { get; set; }

        public string[] args { get; set; }
        

        public AISprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent, Action AI, int update) 
            : base(i, eventHandler, parent)
        {
            this.update = AI;
            Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            updateTimer.Interval = update;
            updateTimer.Enabled = true;
        }

        public AISprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent, Action AI, int update, Action onDeath_method)
           : base(i, eventHandler, parent)
        {
            this.update = AI;
            updateTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            updateTimer.Interval=update;
            updateTimer.Enabled=true;

            this.onDeath = onDeath_method;
        }
    }

   

}
