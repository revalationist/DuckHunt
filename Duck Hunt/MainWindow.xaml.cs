using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Duck_Hunt
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Create an instance of several variables which we will use throughout the whole class.
        public static Image Empty = new Image();
        public static int Mode;
        public static Dictionary<string, Image> Sprites;
        public static List<Image> DuckSprites = new List<Image>();
        public Player Player = new Player();
        public Random RNG = new Random();
        public Interface gameInterface;


        public MainWindow()
        {
            InitializeComponent();

            Loaded += Ready; // Only start making classes and sprites once everything is ready.
        }

        private void Ready(object sender, EventArgs e)
        {
            

            GameBg.Visibility = Visibility.Collapsed;
            //MainMenu.Visibility = Visibility.Collapsed;

            #region ReadyPlayerOne

            

            #endregion

            #region Sprites - Duck

            // Cropping sprites, we use a spritesheet as it saves storage space and increases the reliability of the program (single images can't go missing for instance)

            SpriteSheet main = new SpriteSheet(this.Spritesheet); // I'm sorry for hiding it in the window, but the other methods of loading it were really complicated

            DuckSprites.Add(Empty);

            /* So, for some reason, the Behaviour method for cycling through sprites likes to ignore at least one sprite in the list.
             Believe me, I've spent hours trying to figure out why, to no end.
             So you might think: why not just add an empty value in class initialization so that it's less ugly?
             Well, if I do that it's starts ignoring a *different* value! I still have no idea why that happens, either.
             So if I did that, it was displaying an empty image. 
             
             This is the only way to make it work properly. It was much easier to just swallow my pride and do this,
             rather than figure out why it's actually broken. It even uses the correct number values to index, or at least
             according to the tracepoint. So I was stumped, and still am, regarding this issue.
             
             */

            // red top: 474
            // blue top: 603
            // green top 345

            // sorted: 
            //  green: 345
            //  red: 474
            //  blue: 603

            // -345
            // green: 0
            // red: 129
            // blue: 258

            // -129
            // 0
            // 129

            // Roll for duck colour
            int vPixelValue = 345 + 129*(RNG.Next(0, 3)); // Generate a number from 0 to 2


            DuckSprites.Add(main.CropSpriteFrom(
                Tuple.Create(798, vPixelValue), // Duck frame 1 (alive, moving)
                Tuple.Create(40 * 2, 47 * 2)
            ));

            DuckSprites.Add(main.CropSpriteFrom(
                Tuple.Create(891, vPixelValue), // Duck frame 2 (alive, moving)
                Tuple.Create(96, 87)
            ));

            DuckSprites.Add(main.CropSpriteFrom(
                Tuple.Create(991, vPixelValue), // Duck frame 3 (alive, moving)
                Tuple.Create(77, 93)
            ));


            #endregion

            gameInterface = new Interface(main, this.GameBg);


        }

        private void MenuOptionMEnter(object sender, MouseEventArgs e)
        {
            // When the user hovers their mouse over an option in the menu, make it white for responsive feedback.
            Label obj = sender as Label;
            if (obj != null)
                obj.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        private void MenuOptionMLeave(object sender, MouseEventArgs e)
        {
            // When their mouse leaves, return it to original colour. These methods are only listeners for the 3 game options.
            Label obj = sender as Label;
            if (obj != null)
                obj.Foreground = new SolidColorBrush(Color.FromArgb(255, 252, 152, 56));
        }

        private void GameStart(int newMode)
            // newMode is as yet unimplemented, but we could access it in behaviour using entity.instance.Mode
        {
            MainMenu.Visibility = Visibility.Collapsed;
            Mode = newMode;
            GameBg.Visibility = Visibility.Visible;

            AISprite duck = new AISprite(
                DuckSprites, // List of sprites to animate with
                DuckClick, // Click event handler
                GameBg, // Parent canvas
                Behaviour.DuckFlyUp, // AI function that makes it move/change sprites
                20, // Interval (in milliseconds) between calls to above AI function
                Behaviour.DuckDeath
            ) {Position = Tuple.Create(350, 350), Priority = 0}; // Define initial properties of this duck
        }

        private void EnterGame1(object sender, MouseButtonEventArgs e)
        {
            GameStart(1);
        } // Different modes

        private void EnterGame2(object sender, MouseButtonEventArgs e)
        {
            GameStart(2);
        }

        private void EnterGame3(object sender, MouseButtonEventArgs e)
        {
            GameStart(3);
        }

        private void DuckClick(object sender, MouseButtonEventArgs e)
        {
            if (Player.Bullets > 0)
            {
                AISprite entity = Behaviour.AIByImage[sender as Image];
                entity.Dead = true;
            }

        }

        /* 
         * N.B. Both DuckClick and Shoot get triggered when the player clicks, provided they clicked on a duck.
         * Shoot is only for cosmetic purposes (i.e. muzzle flash), it does not denote a 'miss' or anything like that.
         * DuckClick is the method we will use to handle a successful shot. */

        private void Shoot(object sender, MouseButtonEventArgs e)
        {
           
            if (Player.Bullets > 0)
                this.gameInterface.Bullets[Player.Bullets-1].Img.Visibility = Visibility.Hidden;
            else
                return;

            Player.Bullets--;
    
            BackgroundWorker UpdateUI = new BackgroundWorker // Define a new worker which do waiting for us.
            {
                WorkerSupportsCancellation = true, 
                WorkerReportsProgress = true       
            };

            /* Using this to wait is useful, because it means we run into less issues with
             * the UI threa; which may still be paused when it comes time to make the flash
             * invisible once again.
             *  */
            

            MuzzleFlash.Visibility = Visibility.Visible; // First make our flash visible

            UpdateUI.DoWork += UI_DoWork; // Then we tell our worker that it is to wait
            UpdateUI.RunWorkerCompleted += CancelUpdate; // And give it the cancel function which makes the flash invisible once again
            UpdateUI.RunWorkerAsync(); // Start the waiting period
        }

        private static void UI_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(75); // ...wait
        }


        private void CancelUpdate(object sender, RunWorkerCompletedEventArgs e)
        {
            MuzzleFlash.Visibility = Visibility.Collapsed; // Make flash invisible
            InvalidateVisual(); // Force window to update
        }
    }
}