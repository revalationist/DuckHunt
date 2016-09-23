using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Duck_Hunt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Sprite s = new Sprite(
                new Image()
                {
                    Source = new BitmapImage(
                        new Uri(@"F:\Users\jay0\Desktop\the people.PNG")
                        )
                },
                
                this.Main_Menu);

            
        }
    }
}
