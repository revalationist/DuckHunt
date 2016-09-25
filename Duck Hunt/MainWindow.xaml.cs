﻿using System;
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



        public static Image empty = new Image();
        public static int Mode = 0;
        public static Dictionary<string, Image> Sprites;
        public static List<Image> duckSprites = new List<Image>();



        public MainWindow()
        {
            InitializeComponent();
            //Main_Menu.Visibility = Visibility.Collapsed;
            //this.bg.Color = System.Windows.Media.Color.FromArgb(255, 63, 191, 255);
            this.Loaded += new RoutedEventHandler(Ready);

           



        }

        private void Ready(object sender, EventArgs e)
        {
            GameBg.Visibility = Visibility.Collapsed;
            //MainMenu.Visibility = Visibility.Collapsed;

            // Cropping sprites, we use a spritesheet as it saves storage space and increases the reliability of the program (single images can't go missing for instance)

            Image source = new Image();
            BitmapImage tmp = new BitmapImage();
            tmp.BeginInit();
            tmp.UriSource = new Uri(@"../../Resources/sheetx3.png", UriKind.Relative);
            tmp.EndInit();

            source.Stretch = Stretch.Fill;
            source.Source = tmp;

            SpriteSheet main = new SpriteSheet(source);


            duckSprites.Add(empty);

            duckSprites.Add(main.CropSpriteFrom(
                MakeTuple(399*2, 172*2),
                MakeTuple(40*2, 47*2)
            ));

            duckSprites.Add(main.CropSpriteFrom(
                MakeTuple(891, 351),
                MakeTuple(96, 87)
            ));

            duckSprites.Add(main.CropSpriteFrom(
                MakeTuple(991, 351),
                MakeTuple(77, 93)
            ));




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

            AISprite duck = new AISprite(
                duckSprites,
                DuckClick,
                GameBg,
                Behaviour.Duck,
                10// ms
            )
            {
                Position = MakeTuple(0, 0),
                Priority = 0
            };

            duck.Move(Tuple.Create(0, 100));

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
