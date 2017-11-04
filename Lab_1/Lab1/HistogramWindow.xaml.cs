using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Lab1
{
    /// <summary>
    /// Interaction logic for HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow : Window
    {
        public PointCollection Points;
        public string Name;

        public HistogramWindow(PointCollection points, string name)
        {
            Points = points;
            Name = name;
            InitializeComponent();
            this.Title = Name;
            histogram.Points = Points;
        }
    }
}
