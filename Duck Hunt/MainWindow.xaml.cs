using System;
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
        public static int mode = 0;

        public MainWindow()
        {
            InitializeComponent();
            //Main_Menu.Visibility = Visibility.Collapsed;
            this.bg.Color = System.Windows.Media.Color.FromArgb(255, 63, 191, 255);
            GameBG.Visibility = Visibility.Collapsed;
        }

        private void menuOptionMEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label obj = sender as Label;
            obj.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));  //#EA9E24
        }

        private void menuOptionMLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label obj = sender as Label;
            obj.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 252, 152, 56));
        }

        private void GameStart(int newMode)
        {
            Main_Menu.Visibility = Visibility.Collapsed;
            mode = newMode;
            GameBG.Visibility = Visibility.Visible;
        }

        private void enterGame1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GameStart(1);
        }

        private void enterGame2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GameStart(2);
        }

        private void enterGame3(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GameStart(3);
        }
    }
}
