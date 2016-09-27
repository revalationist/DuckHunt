using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Duck_Hunt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        // Create an instance of several variables which we will use throughout the whole class.
        public static Image empty = new Image();
        public static int Mode = 0;
        public static Dictionary<string, Image> Sprites;
        public static List<Image> duckSprites = new List<Image>();

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Ready); // Only start making classes and sprites once everything is ready.
        }

        private void Ready(object sender, EventArgs e)
        {
            GameBg.Visibility = Visibility.Collapsed;
            //MainMenu.Visibility = Visibility.Collapsed;

            // Cropping sprites, we use a spritesheet as it saves storage space and increases the reliability of the program (single images can't go missing for instance)

            Image source = new Image();
            BitmapImage tmp = new BitmapImage();
            tmp.BeginInit();
            tmp.UriSource = new Uri(@"../../Resources/sheetx3.png", UriKind.Relative); // The amount of hoops you have to jump through to get a uri into an Image control (programmatically) is appaling
            tmp.EndInit();

            source.Stretch = Stretch.Fill;
            source.Source = tmp;

            SpriteSheet main = new SpriteSheet(source); // Instance the spritesheet so now we can use its extension methods.


            duckSprites.Add(empty);

            /* So, for some reason, the Behaviour method for cycling through sprites likes to ignore at least one sprite in the list.
             Believe me, I've spent hours trying to figure out why, to no end.
             So you might think: why not just add an empty value in class initialization so that it's less ugly?
             Well, if I do that it's starts ignoring a *different* value! I still have no idea why that happens, either.
             So then I tried putting the empty part in different places, which didn't help as now it was displaying an empty image. */

            duckSprites.Add(main.CropSpriteFrom(
                Tuple.Create(399*2, 172*2), // Start
                Tuple.Create(40*2, 47*2)
            ));

            duckSprites.Add(main.CropSpriteFrom(
                Tuple.Create(891, 351),
                Tuple.Create(96, 87)
            ));

            duckSprites.Add(main.CropSpriteFrom(
                Tuple.Create(991, 351),
                Tuple.Create(77, 93)
            ));
        }

        private static Tuple<int, int> MakeTuple(int a, int b) // Shorthand for creating int, int tuples
        {
            return new Tuple<int, int>(a, b);
        }

        private void MenuOptionMEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // When the user hovers their mouse over an option in the menu, make it white for responsive feedback.
            Label obj = sender as Label;
            obj.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));  
        }

        private void MenuOptionMLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // When their mouse leaves, return it to original colour. These methods are only listeners for the 3 game options.
            Label obj = sender as Label;
            obj.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 252, 152, 56));
        }

        private void GameStart(int newMode) // newMode is as yet unimplemented, but we could access it in behaviour using entity.instance.Mode
        {
            MainMenu.Visibility = Visibility.Collapsed;
            Mode = newMode;
            GameBg.Visibility = Visibility.Visible;

            AISprite duck = new AISprite(
                duckSprites, // List of sprites to animate with
                DuckClick, // Click event handler
                GameBg, // Parent canvas
                Behaviour.Duck, // AI function that makes it move/change sprites
                20 // Interval (in milliseconds) between calls to above AI function
            ) { Position = Tuple.Create(350, 350), Priority = 0 }; // Define initial properties of this duck
        }

        private void EnterGame1(object sender, System.Windows.Input.MouseButtonEventArgs e) { GameStart(1); } // Different modes
        
        private void EnterGame2(object sender, System.Windows.Input.MouseButtonEventArgs e) { GameStart(2); }   
       
        private void EnterGame3(object sender, System.Windows.Input.MouseButtonEventArgs e) { GameStart(3); }

        private void DuckClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Shoot(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // bang
            // code for maybe successful shot goes here
            BackgroundWorker UIUpdater = new BackgroundWorker();

            UIUpdater.WorkerSupportsCancellation = true;
            UIUpdater.WorkerReportsProgress = true;

            MuzzleFlash.Visibility = Visibility.Visible;

            UIUpdater.DoWork += new DoWorkEventHandler(UI_DoWork);
            UIUpdater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(cancelUIUpdate);
            UIUpdater.RunWorkerAsync();
        }

        private void UI_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(250);
        }


        private void cancelUIUpdate(object sender, RunWorkerCompletedEventArgs e)
        {
            MuzzleFlash.Visibility = Visibility.Collapsed;
            this.InvalidateVisual();  
        }
    }
}
