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
        public MainWindow Instance { get; private set; }
        public Image Img { get; set; }
        public Canvas Parent { get; set; }


        public int Priority
        {
            get
            {
                return Priority;
            }
            set
            {
                Priority = Priority + value;
                Panel.SetZIndex(this.Img, Priority);
            }
        }


        public Tuple<int, int> Position
        {
            get
            {
                return Position;
            }
            set
            {
                Thickness tmp = this.Img.Margin;

                tmp.Left = value.Item1;
                tmp.Top = value.Item2;

                this.Img.Margin = tmp;
            }

        }
  
       

        public GameImageObj(Image i, Canvas parent)
        {
            this.Img = i;
            parent.Children.Add(Img);
            Window instance = Window.GetWindow(parent);
        }
    }

    class SpriteSheet
    {
        protected Image Img { get; private set; }
        public Tuple<int, int> Size { get; private set; } // This is a spritesheet - there is no reason to modify size
        

        public Image CropSpriteFrom(Tuple<int, int> start, Tuple<int, int> end)
        {
            // Check that the numbers given are valid
            if (!Img.CheckPixelValues(start))
            {
                throw new ArgumentException(string.Format("Attempted illegal crop of image -- image has width {0} and height {1} but starting point of the crop was {2}, {3}",
                    Img.Width,
                    Img.Height,

                    start.Item1,
                    start.Item2));
            }
            if (!Img.CheckPixelValues(end))
            {
                throw new ArgumentException(string.Format("Attempted illegal crop of image -- image has width {0} and height {1} but ending point of the crop was {2}, {3}",
                    Img.Width,
                    Img.Height,

                    end.Item1,
                    end.Item2));
            }

            Image output = new Image();
            output.Source = Img.Source;

            int offset1 = end.Item1;
            int offset2 = end.Item2;

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
            this.Img = input;
            Size = new Tuple<int, int>((int)input.Width, (int)input.Height);
        }
    }

    public class Sprite : GameImageObj
    {
        
        public void Move(Tuple<int,int> offset)
        {
            Tuple<int, int> result = ExtraMethods.TupleSum(this.Position, offset);
            this.Position = result;
        }

        public void Resize(int factor)
        {
            int w = Convert.ToInt32(this.Img.ActualWidth * factor);
            int h = Convert.ToInt32(this.Img.ActualHeight * factor);

            this.Img.Width = w;
            this.Img.Height = w;
        }

        public Sprite(Image i, Canvas parent) 
            : base(i, parent)
        {
            // do nothing, base class will handle it
        }

        public Sprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent) 
            : base(i, parent)
        {
            this.Img.MouseDown += eventHandler;
        }

    } // Not abstract as I may use the crosshair for this


    class AiSprite : Sprite
    {

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Update();
        }

        Timer _updateTimer = new Timer();
        

        public bool Dead
        {
            get
            {
                return Dead;
            }
            set
            {
                if (value == true) // if we're setting dead to true
                {
                    OnDeath();
                    _updateTimer.Enabled = false;
                }
            }
        }

        public Action OnDeath { get; set; }
        public Action Update { get; set; }

        public string[] Args { get; set; }
        

        public AiSprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent, Action ai, int update) 
            : base(i, eventHandler, parent)
        {
            this.Update = ai;
            Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _updateTimer.Interval = update;
            _updateTimer.Enabled = true;
        }

        public AiSprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent, Action ai, int update, Action onDeathMethod)
           : base(i, eventHandler, parent)
        {
            this.Update = ai;
            _updateTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _updateTimer.Interval=update;
            _updateTimer.Enabled=true;

            this.OnDeath = onDeathMethod;
        }
    }

   

}
