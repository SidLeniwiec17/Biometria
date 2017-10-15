using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
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

namespace Lab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Bitmap newBmp;
        public Bitmap originalBitmap;
        public int darkBright;
        public float contrast;
        public int r;
        public int rG;
        public int hist;
        public PointCollection pointsX;
        public PointCollection pointsY;
        public ConcurrentBag<System.Windows.Point> pointsBagX;
        public ConcurrentBag<System.Windows.Point> pointsBagY;

        public MainWindow()
        {
            InitializeComponent();
            darkBright = 120;
            contrast = 120;
            r = 3;
            rG = 3;
            hist = 3;
            pointsX = new PointCollection();
            pointsY = new PointCollection();
            pointsBagX = new ConcurrentBag<System.Windows.Point>();
            pointsBagY = new ConcurrentBag<System.Windows.Point>();
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

        private async void DoGray_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunGrayScale();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);

        }

        public async Task RunGrayScale()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.GrayScale(originalBitmap);
            });
        }

        public async Task RunInverse()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.Inverse(originalBitmap);
            });
        }

        public async Task RunDarkBright()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.DarkBright(originalBitmap, darkBright);
            });
        }
        public async Task RunContrast()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.Contrast(originalBitmap, contrast);
            });
        }

        public async Task RunGlobBin()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.GlobBin(originalBitmap, r);
            });
        }

        public async Task RunLocalBin()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.LocalBin(originalBitmap, rG);
            });
        }

        public async Task RunHist()
        {
            await Task.Run(() =>
            {
                var answ = Methods.Hist(originalBitmap, hist);
                newBmp = answ.Item1;

                foreach (var x in answ.Item2)
                    pointsBagX.Add(x);

                foreach (var x in answ.Item3)
                    pointsBagY.Add(x);
            });
        }

        private async void DoInverse_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunInverse();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void DarkBright_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (int.TryParse(dark.Text, out darkBright))
            {
                if (darkBright <= -255 || darkBright >= 255)
                {
                    MessageBox.Show("Incorrect darkBright!");
                    return;
                }
            }
            await RunDarkBright();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void Contrast_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (float.TryParse(contrast_txt.Text, out contrast))
            {
                if (contrast == 0)
                {
                    MessageBox.Show("Incorrect contrast!");
                    return;
                }
            }
            await RunContrast();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void GlobalBin_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (int.TryParse(r_txt.Text, out r))
            {
                if (r < 0)
                {
                    MessageBox.Show("Incorrect local binarization level!");
                    return;
                }
            }
            await RunGlobBin();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void LocalBin_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (int.TryParse(rg_txt.Text, out rG))
            {
                if (rG < 0)
                {
                    MessageBox.Show("Incorrect global binarization level!");
                    return;
                }
            }
            await RunLocalBin();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void Histogram_Button(object sender, RoutedEventArgs e)
        {
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (int.TryParse(hist_txt.Text, out hist))
            {
                if (hist < 0)
                {
                    MessageBox.Show("Incorrect local binarization level!");
                    return;
                }
            }

            pointsX = new PointCollection();
            pointsY = new PointCollection();
            pointsBagX = new ConcurrentBag<System.Windows.Point>();
            pointsBagY = new ConcurrentBag<System.Windows.Point>();

            await RunHist();

            foreach (var x in pointsBagX)
                pointsX.Add(x);

            foreach (var x in pointsBagY)
                pointsY.Add(x);

            if (pointsX.Count > 0 && pointsY.Count > 0)
            {
                HistogramWindow histogramX = new HistogramWindow(pointsX, "X", newBmp.Width, newBmp.Height);
                HistogramWindow histogramY = new HistogramWindow(pointsY, "Y", newBmp.Width, newBmp.Height);
                histogramX.Show();
                histogramY.Show();
            }

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }
    }
}
