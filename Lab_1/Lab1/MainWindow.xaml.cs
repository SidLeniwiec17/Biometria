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
        public double aLowPas;
        public double aHighPas;
        public double aGaussPas;
        public PointCollection pointsX;
        public PointCollection pointsY;
        public ConcurrentBag<System.Windows.Point> pointsBagX;
        public ConcurrentBag<System.Windows.Point> pointsBagY;
        public PointCollection pointsR;
        public PointCollection pointsG;
        public PointCollection pointsB;
        public ConcurrentBag<System.Windows.Point> pointsBagR;
        public ConcurrentBag<System.Windows.Point> pointsBagG;
        public ConcurrentBag<System.Windows.Point> pointsBagB;


        public MainWindow()
        {
            InitializeComponent();
            darkBright = 120;
            contrast = 120;
            r = 3;
            rG = 3;
            hist = 3;
            aLowPas = 3;
            aHighPas = 3;
            aGaussPas = 3;
            pointsX = new PointCollection();
            pointsY = new PointCollection();
            pointsBagX = new ConcurrentBag<System.Windows.Point>();
            pointsBagY = new ConcurrentBag<System.Windows.Point>();
        }

        private void Load_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
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
            HideHistogram();
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

        public async Task RunHistNorm()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.HistNorm(originalBitmap);
            });
        }

        public async Task RunHistEQ()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.HistEQ(originalBitmap);
            });
        }

        public async Task RunLowPass()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.LowPass(originalBitmap, aLowPas);
            });
        }

        public async Task RunHighPass()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.HighPass(originalBitmap, aHighPas);
            });
        }

        public async Task RunGaussPass()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.Gauss(originalBitmap, aGaussPas);
            });
        }

        public async Task RunRobertsPass()
        {
            await Task.Run(() =>
            {
                newBmp = Methods.Roberts(originalBitmap);
            });
        }

        public async Task RunProjection()
        {
            await Task.Run(() =>
            {
                var answ = Methods.Projection(originalBitmap);
                newBmp = answ.Item1;

                foreach (var x in answ.Item2)
                    pointsBagR.Add(x);

                foreach (var x in answ.Item3)
                    pointsBagG.Add(x);

                foreach (var x in answ.Item4)
                    pointsBagB.Add(x);
            });
        }

        private async void DoInverse_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
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
            HideHistogram();
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
            HideHistogram();
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
            HideHistogram();
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
            HideHistogram();
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
            HideHistogram();
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
                int size = 0;
                if (2 * newBmp.Height + 100 > SystemParameters.VirtualScreenHeight)
                {
                    size = (int)(SystemParameters.VirtualScreenHeight - 150) / 2;
                }
                else
                {
                    size = newBmp.Height;
                }
                borderX.Visibility = Visibility.Visible;
                borderX.Height = size;
                polygonX.Points = pointsX;
                borderX.Header = "X";
                borderY.Visibility = Visibility.Visible;
                borderY.Height = size;
                polygonY.Points = pointsY;
                borderY.Header = "Y";
                HistCol.Width = new GridLength(1, GridUnitType.Star);
            }

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private void HideHistogram()
        {
            borderX.Visibility = Visibility.Collapsed;
            borderY.Visibility = Visibility.Collapsed;
            borderZ.Visibility = Visibility.Collapsed;
            polygonX.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            polygonY.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            Brow.Height = GridLength.Auto;
            HistCol.Width = GridLength.Auto;
        }

        private async void DoProjection_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;

            pointsR = new PointCollection();
            pointsG = new PointCollection();
            pointsB = new PointCollection();
            pointsBagR = new ConcurrentBag<System.Windows.Point>();
            pointsBagG = new ConcurrentBag<System.Windows.Point>();
            pointsBagB = new ConcurrentBag<System.Windows.Point>();

            await RunProjection();

            foreach (var x in pointsBagR)
                pointsR.Add(x);

            foreach (var x in pointsBagG)
                pointsG.Add(x);

            foreach (var x in pointsBagB)
                pointsB.Add(x);


            if (pointsR.Count > 0 && pointsG.Count > 0 && pointsB.Count > 0)
            {
                int size = 0;
                if(3 * newBmp.Height + 100> SystemParameters.VirtualScreenHeight)
                {
                    size = (int)(SystemParameters.VirtualScreenHeight - 150) / 3;
                }
                else
                {
                    size = newBmp.Height;
                }
                Brow.Height = Rrow.Height;
                borderX.Visibility = Visibility.Visible;
                borderX.Height = size;
                polygonX.Points = pointsR;
                borderX.Header = "R";
                polygonX.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                borderY.Visibility = Visibility.Visible;
                borderY.Height = size;
                polygonY.Points = pointsG;
                polygonY.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
                borderY.Header = "G";
                borderZ.Visibility = Visibility.Visible;
                borderZ.Height = size;
                polygonZ.Points = pointsB;
                polygonZ.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
                borderZ.Header = "B";
                
            }

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void NormHist_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunHistNorm();

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void EqHist_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunHistEQ();

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void LowFilter_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (double.TryParse(aLowPas_txt.Text, out aLowPas))
            {
                if (aLowPas < 0)
                {
                    MessageBox.Show("Incorrect 'a' level!");
                    return;
                }
            }
            await RunLowPass();

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void HighFilter_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (double.TryParse(aHighPas_txt.Text, out aHighPas))
            {
                if (aHighPas < 0)
                {
                    MessageBox.Show("Incorrect 'a' level!");
                    return;
                }
            }
            await RunHighPass();

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void GaussFilter_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            if (double.TryParse(aGaussPas_txt.Text, out aGaussPas))
            {
                if (aGaussPas < 0)
                {
                    MessageBox.Show("Incorrect 'a' level!");
                    return;
                }
            }
            await RunGaussPass();

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

        private async void Edges_Button(object sender, RoutedEventArgs e)
        {
            HideHistogram();
            if (newBmp == null)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunRobertsPass();

            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = Methods.ToBitmapSource(newBmp);
        }

    }
}

