using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

// Transformation methods and such to make my life easier.

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


    public class GameImageObj // Used for interfaces
    {
        private Tuple<int, int> _position;  // Backing field: if you try and use a get; set; accessor without using the default (which generates this for you)
                                            // you will find a neat example of recursion in your get statement, which causes a stackoverflow

        private int _priority;

        protected GameImageObj(Image i, Canvas parent) // struct
        {
            Img = i; // Set the source image object
            parent.Children.Add(Img); // Add it to the canvas to make sure it displays
            parent.InvalidateVisual(); // Force redraw of window
            Window instance = Window.GetWindow(parent); // Get instance
        }

        public MainWindow Instance { get; protected set; }
        public Image Img { get; set; }
        public Canvas Parent { get; set; }
        
        public int Priority
        {
            get { return _priority; }
            set
            {
                _priority = _priority + value;
                Panel.SetZIndex(Img, Priority);
            }
        }

        public Tuple<int, int> Position
        {
            get { return _position; }
            set
            {
                try
                {
                    Application.Current?.Dispatcher.Invoke(
                        () => // We need to make a call to the main thread to modify UI elements.
                        {
                            Canvas.SetLeft(Img, value.Item1);
                            Canvas.SetTop(Img, value.Item2);
                        });
                }
                catch (TaskCanceledException)
                {
                }

                // This happens if the user times their application closing with us trying to do the above.
                // It happens a lot, but we can just do nothing here -- if they're already closing, it doesn't matter what we do now.

                _position = value; // update backing field.
            }
        }
    }

    public class SpriteSheet
    {
        public SpriteSheet(Image input) // Struct, simply define an image to modify and a size to check input against.
        {
            Img = input;
            Size = new Tuple<int, int>((int) input.Width, (int) input.Height);
        }

        protected Image Img { get; } // No reason to change the image, as the spritesheet is not at all dynamic.
        public Tuple<int, int> Size { get; private set; } // This is a spritesheet - there is no reason to modify size


        public Image CropSpriteFrom(Tuple<int, int> start, Tuple<int, int> end)
        {
            // Check that the numbers given are valid
            if (!Img.CheckPixelValues(start))
                // Check ExtMethods.cs for definition of this. It essentially ensures that we don't go out of bounds with the crop.
                throw new ArgumentException( // and, if we do, provides a more detailed exception.
                    $"Attempted illegal crop of image -- image has width {Img.Width} and height {Img.Height} but starting point of the crop was {start.Item1}, {start.Item2}");
            if (!Img.CheckPixelValues(end))
                throw new ArgumentException(
                    $"Attempted illegal crop of image -- image has width {Img.Width} and height {Img.Height} but ending point of the crop was {start.Item1 + end.Item1}, {start.Item2 + end.Item2}");

            if ((end.Item1 < 0) || (end.Item2 < 0))
                throw new ArgumentException(
                    $"Offset in crop operation was x, y: {end.Item1}, {end.Item2} -- at least one of these was below zero and was therefore invalid.");


            CroppedBitmap cb = new CroppedBitmap(
                (BitmapSource) Img.Source,
                new Int32Rect(start.Item1, start.Item2,
                    end.Item1, end.Item2) // Define a rectangle to crop with
            );

            Image output = new Image {Source = cb}; // Use our cropped image source to make a new object, which we return
            return output;
                // We do not use ref paramaters otherwise we'd be modifying the sheet and making it useless for further cropping
        }
    }

    public class Sprite : GameImageObj
        // Inherit from the base image object, which defined such paramaters as the core image and position
    {
        /*public Image Resize(int factor)
        {
            /*public static Image resizeImage(Image imgToResize, Size size)
            {
                return (Image)(new Bitmap(imgToResize, size));
            }

            yourImage = resizeImage(yourImage, new Size(50,50));


            Image output = new Image();
            output.Width = Img.Width*factor;
            output.Height = Img.Height*factor;

            output.Source = Img.Source;
            return output;

        } 
        * Could not get resize to work due to some issues with Image.Width behaving weirdly and giving me NaN or -2147m, for some reason.
        * Image.ActualWidth was always 0 as well.
        * So I decided to instead resize images in external programs to bring to an acceptable res for a 1080p screen. That should also be faster 
        
             */


        public Sprite(Image i, Canvas parent) // The most basic overload of the constructor
            : base(i, parent)
        {
            // do nothing, base class will handle it
        }

        public Sprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent)
            // This overload we use if we want to have a basic response to clicking.
            : base(i, parent)
        {
            Img.MouseDown += eventHandler;
            Frames = null;
        }

        public Sprite(List<Image> states, MouseButtonEventHandler eventHandler, Canvas parent)
            // This overload we use if we want it to have a response to clicking AND animation.
            : base(states[0], parent)
        {
            Img.MouseDown += eventHandler;
            Frames = states;
            Img.Source = Frames[1].Source;
        }

        public List<Image> Frames { get; }


        public void Move(Tuple<int, int> offset)
            // Sprite just adds more functionality relating to easy movement, by providing relative operations.
        {
            Position = Position.Add(offset);
        }
    } 


    public class AISprite : Sprite // A class that builds on Sprite to provide more advanced AI functions.
    {
        private bool _dead;

        public int SpriteIncrement = 1;
            // So here we have a few variables to use when we only have a Timer and a Sprite in Behaviour.cs.

        public Timer Timer = new Timer();

        public AISprite(List<Image> sprites, MouseButtonEventHandler eventHandler, Canvas parent, ElapsedEventHandler ai,
                int update)
            // overload 1, if we don't want our sprite to have a reaction to death
            // usually meaning it can't die, e.g. the dog
            : base(sprites, eventHandler, parent)
        {
            _init(ai, update);
        }

        public AISprite(List<Image> sprites, MouseButtonEventHandler eventHandler, Canvas parent, ElapsedEventHandler ai,
                int update,
                Func<AISprite, int> onDeathMethod)
            // overload 2, we use this for ducks, because they need to do something when they die
            // (change the animation and award the player points)
            : base(sprites, eventHandler, parent)
        {
            _init(ai, update);

            OnDeath = onDeathMethod;
        }

        public Func<AISprite, int> OnDeath { get; set; }

        public Tuple<int, int> MovementDirection { get; set; }
        public int Counter { get; set; }
        public int SpriteIndex { get; set; }
        // Since we could be talking about any sprite, values defined in Behaviour.Duck could be anything.
        // I've added these to make them personalized to the sprites:
        /*
         * MovementDirection = used in Behaviour.Duck to see how much the sprite should move every tick. We change this when, for example, we hit one of the window borders.
         * 
         * counter = the number of times the timer has been triggered. Used to have an event happen once every 100 timer 'ticks' for instance (which is actually how the sprite changing works)
         * as we need one event to happen a lot more often than another (movement vs sprite changes)
         * 
         * spriteIndex = the index in the List<image> frames which gives the image currently being used by the Sprite
         * spriteIncrement = since Duck Hunt's sprites don't form a perfect loop unless you play them once forwards and then again backwards, we need to go like this
         * 0, 1, 2, 3, 2, 1, 0
         * 
         * we use this value, which can be 1 or -1, to determine which direction we're going into the list. */

        public bool Dead
        {
            get { return _dead; }
            set
            {
                _dead = value;

                if (value) // if we're now dead
                {
                    OnDeath(this);
                    Timer.Enabled = false;
                    //RIP
                }
            }
        }


        private void _init(ElapsedEventHandler ai, int update)
            // Had a lot of operations needed at the start of this class given the amount of work needed to get a timer running.
            // Because we were using overloads, it's easier to pack this into a simple function rather than copy and paste it a number of times.
        {
            Behaviour.Parents[Timer] = this;
            //Timer aTimer = new System.Timers.Timer();
            Timer.Elapsed += ai;
            Timer.Interval = update;
            Timer.Enabled = true;
            Timer.Start();

            Counter = 0;
            SpriteIndex = 0;

            MovementDirection = Tuple.Create(Behaviour.DuckSpeed, Behaviour.DuckSpeed);
        }
    }
}