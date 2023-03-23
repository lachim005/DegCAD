using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for PointCoordinateInputDialog.xaml
    /// </summary>
    public partial class PointCoordinateInputDialog : Window
    {
        ObservableCollection<PointData> inputValues = new();

        /// <summary>
        /// False if the user inputed some points
        /// </summary>
        public bool Canceled { get; private set; } = true;
        /// <summary>
        /// The points that the user has inputed
        /// </summary>
        public List<PointData> Points { get; private set; } = new();

        /// <summary>
        /// Class holding data about a point. Used in the datagrid that the user enters points to
        /// </summary>
        public class PointData
        {
            public string Label { get; set; } = "";
            public double X { get; set; }
            public double Y { get; set; } = double.NaN;
            public double Z { get; set; } = double.NaN;
        }

        public PointCoordinateInputDialog()
        {
            InitializeComponent();

            //Uses CollectionViewSourve to add a filter tothe datagrid
            var itemSourceList = new CollectionViewSource() { Source = inputValues };
            ICollectionView Itemlist = itemSourceList.View;
            Itemlist.Filter = (o) =>
            {
                var pt = o as PointData;
                if (pt is null) return false;

                //X has to be a number
                if (!double.IsFinite(pt.X)) return false;
                //Y and Z can't be both NaN
                if (double.IsNaN(pt.Y) && double.IsNaN(pt.Z)) return false;

                return true;
            };

            coordinatesDtg.ItemsSource = Itemlist;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (inputValues.Count > 0)
            {
                var list = inputValues.ToList();
                if (reverseXDirection.IsChecked == true)
                {
                    foreach (var pt in list)
                    {
                        pt.X = -pt.X;
                    }
                }
                Points = list;
                Canceled = false;
            }
            Close();
        }
    }
}
