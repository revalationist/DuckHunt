using System;
using System.Collections.Generic;
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
        public static int Mode = 0;
        public static Dictionary<string, Image> Sprites;

        public MainWindow()
        {
            InitializeComponent();
            //Main_Menu.Visibility = Visibility.Collapsed;
            //this.bg.Color = System.Windows.Media.Color.FromArgb(255, 63, 191, 255);
            this.Loaded += new RoutedEventHandler(Ready);

            




        }

        private void Ready(object sender, EventArgs e)
        {
            //GameBg.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Collapsed;

            // Cropping sprites, we use a spritesheet as it saves storage space and increases the reliability of the program (single images can't go missing for instance)

            Image source = new Image();
            BitmapImage tmp = new BitmapImage();
            tmp.BeginInit();
            tmp.UriSource = new Uri("sheet_b.png", UriKind.Relative);
            tmp.EndInit();

            source.Stretch = Stretch.Fill;
            source.Source = tmp;

            SpriteSheet main = new SpriteSheet(source);

            Image duckSrc = main.CropSpriteFrom(
                MakeTuple(100, 100), // when you crop an image, some whitespace remains
                MakeTuple(1722, 846) // so when we had a large image, the remaining bit was invisible
            ); // due to being offscreen

            AISprite duck = new AISprite(
                duckSrc,
                DuckClick,
                GameBg,
                Behaviour.Duck,
                100000 // ms
            );


            duck.Position = MakeTuple(0, 0);
            
            
        }

        private static Tuple<int, int> MakeTuple(int a, int b) // Shorthand for creating int, int tuples
        {
            return new Tuple<int, int>(a, b);
        }

        private void MenuOptionMEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label obj = sender as Label;
            obj.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));  //#EA9E24
        }

        private void MenuOptionMLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label obj = sender as Label;
            obj.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 252, 152, 56));
        }

        private void GameStart(int newMode)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            Mode = newMode;
            GameBg.Visibility = Visibility.Visible;
        }

        private void EnterGame1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GameStart(1);
        }

        private void EnterGame2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GameStart(2);
        }

        private void EnterGame3(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GameStart(3);
        }

        private void DuckClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // bang
        }
    }
}
