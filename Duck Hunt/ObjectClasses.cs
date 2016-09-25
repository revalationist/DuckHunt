﻿using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

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

        private int _priority;

        public int Priority
        {
            get { return _priority; }
            set
            {
                _priority = _priority + value;
                Panel.SetZIndex(this.Img, Priority);
            }
        }

        private Tuple<int, int> _position; // backing field

        public Tuple<int, int> Position
        {
            get { return this._position; }
            set
            {
                
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(this.Img, value.Item1);
                    Canvas.SetTop(this.Img, value.Item2);
                });

                
                
                this._position = value;
            }
        }


        protected GameImageObj(Image i, Canvas parent)
        {
            this.Img = i;
            parent.Children.Add(Img);
            parent.InvalidateVisual();
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
                throw new ArgumentException(
                    $"Attempted illegal crop of image -- image has width {Img.Width} and height {Img.Height} but starting point of the crop was {start.Item1}, {start.Item2}");
            }
            if (!Img.CheckPixelValues(end))
            {
                throw new ArgumentException(
                    $"Attempted illegal crop of image -- image has width {Img.Width} and height {Img.Height} but ending point of the crop was {start.Item1 + end.Item1}, {start.Item2 + end.Item2}");
            }

            
            

            int offset1 = end.Item1;
            int offset2 = end.Item2;

            if (offset1 < 0 || offset2 < 0)
            {
                throw new ArgumentException(
                    $"Offset in crop operation was x, y: {offset1}, {offset2} -- at least one of these was below zero and was therefore invalid.");
            }


            CroppedBitmap cb = new CroppedBitmap(
                (BitmapSource)Img.Source,
                new Int32Rect(start.Item1, start.Item2, 
                    offset1, offset2)
                );

            Image output = new Image {Source = cb};
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
        public List<Image> frames { get; private set; }
        public int counter { get; set; }
        public int spriteIndex { get; set; }
        
        public void Move(Tuple<int,int> offset)
        {
            Tuple<int, int> result = ExtraMethods.TupleSum(this.Position, offset);
            this.Position = result;
        }

        public Image Resize(int factor)
        {
            /*public static Image resizeImage(Image imgToResize, Size size)
            {
                return (Image)(new Bitmap(imgToResize, size));
            }

            yourImage = resizeImage(yourImage, new Size(50,50));*/


            Image output = new Image();
            output.Width = Img.Width*factor;
            output.Height = Img.Height*factor;

            output.Source = Img.Source;
            return output;

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
            frames = null;
        }

        public Sprite(List<Image> states, MouseButtonEventHandler eventHandler, Canvas parent)
            : base(states[0], parent)
        {
            this.Img.MouseDown += eventHandler;
            frames = states;
            counter = 0;
            spriteIndex = 0;
            this.Img.Source = this.frames[spriteIndex].Source;
        }

    } // Not abstract as I may use the crosshair for this


    public class AISprite : Sprite
    {

        public Func<AISprite, int> OnDeath { get; set; }
        

        public string[] Args { get; set; }

       

        public Timer aTimer = new Timer();
        private bool _dead;

        public bool Dead
        {
            get
            {
                return _dead;
            }
            set
            {
                _dead = value;

                if (value == true) // if we're setting dead to true
                {
                    OnDeath(this);
                    aTimer.Enabled = false;
                }
            }
        }
        

        public AISprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent, ElapsedEventHandler ai, int update) 
            : base(i, eventHandler, parent)
        {
            Behaviour.Parents[aTimer] = this;
            //Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += ai;
            aTimer.Interval = update;
            aTimer.Enabled = true;
            aTimer.Start();
        }

        public AISprite(Image i, MouseButtonEventHandler eventHandler, Canvas parent, ElapsedEventHandler ai, int update, Func<AISprite, int> onDeathMethod)
           : base(i, eventHandler, parent)
        {
            Behaviour.Parents[aTimer] = this;
            //Timer aTimer = new Timer();
            aTimer.Elapsed += ai;
            aTimer.Interval = update;
            aTimer.Enabled = true;
            aTimer.Start();

            this.OnDeath = onDeathMethod;
        }

        public AISprite(List<Image> sprites, MouseButtonEventHandler eventHandler, Canvas parent, ElapsedEventHandler ai, int update)
            : base(sprites, eventHandler, parent)
        {
            Behaviour.Parents[aTimer] = this;
            //Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += ai;
            aTimer.Interval = update;
            aTimer.Enabled = true;
            aTimer.Start();
        }

        public AISprite(List<Image> sprites , MouseButtonEventHandler eventHandler, Canvas parent, ElapsedEventHandler ai, int update, Func<AISprite, int> onDeathMethod)
           : base(sprites, eventHandler, parent)
        {
            Behaviour.Parents[aTimer] = this;
            //Timer aTimer = new Timer();
            aTimer.Elapsed += ai;
            aTimer.Interval = update;
            aTimer.Enabled = true;
            aTimer.Start();

            this.OnDeath = onDeathMethod;
        }
    }

   

}
