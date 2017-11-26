using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Bitmap newBmp;
        public Bitmap originalBitmap;
        public int maskMode;

        public MainWindow()
        {
            InitializeComponent();
            maskMode = 1;
        }

        private void Load_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "pic";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (dialog.ShowDialog() == true)
            {
                img.Source = new BitmapImage(new Uri(dialog.FileName));
                ori.Source = new BitmapImage(new Uri(dialog.FileName));
                newBmp = (Bitmap)Bitmap.FromFile(dialog.FileName);
                originalBitmap = (Bitmap)Bitmap.FromFile(dialog.FileName);
            }
        }

        public async Task RunTreshold()
        {
            await Task.Run(() =>
            {
                int treshold = 0;
                var answer = Methods.GetGreyScaleTreshold(originalBitmap);
                newBmp = answer.Item1;
                treshold = answer.Item2;
            });
        }

        private async void Treshold_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunTreshold();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private void MashMode_Button(object sender, RoutedEventArgs e)
        {

        }

        private void Erosion_Button(object sender, RoutedEventArgs e)
        {

        }

        private void Dilation_Button(object sender, RoutedEventArgs e)
        {

        }
    }
}
