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
        //Last values
        public static bool lastAxisDirectionLeft = true;

        ObservableCollection<PointInputStruct> inputValues = new() { new("","","","") };

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
        record PointInputStruct(string Label, string X, string Y, string Z);

        public PointCoordinateInputDialog()
        {
            InitializeComponent();


            coordInputIc.ItemsSource = inputValues;

            LoadLastValues();
        }

        private void LoadLastValues()
        {
            if (lastAxisDirectionLeft) reverseXDirection.IsChecked = true;
            else dontReverseXDirection.IsChecked = true;
        }

        private void SaveLastValues()
        {
            lastAxisDirectionLeft = reverseXDirection.IsChecked == true;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            foreach (var item in inputValues)
            {
                if (IsEmpty(item)) 
                    continue;

                if (!double.TryParse(item.X, out double x))
                {
                    MessageBox.Show("U každého bodu musí být vyplněna souřadnice X");
                    return;
                }

                bool hasOne = true;
                if (!double.TryParse(item.Y, out double y))
                {
                    y = double.NaN;
                    hasOne = false;
                }
                if (!double.TryParse(item.Z, out double z))
                {
                    if (!hasOne)
                    {
                        MessageBox.Show("U každého bodu musí být vyplněna alespoň jedna souřadnice Y nebo Z");
                        return;
                    }
                    z = double.NaN;
                }

                if (reverseXDirection.IsChecked == true)
                    x *= -1;

                Points.Add(new() { Label = item.Label, X = x, Y = y, Z = z });
            }
            if (Points.Count != 0) Canceled = false;
            Close();
        }

        private bool IsEmpty(PointInputStruct item)
        {
            if (string.IsNullOrWhiteSpace(item.Label) && string.IsNullOrWhiteSpace(item.X) && string.IsNullOrWhiteSpace(item.Y) && string.IsNullOrWhiteSpace(item.Z))
                return true;
            return false;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            SaveLastValues();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach(var item in inputValues) 
            { 
                if (IsEmpty(item))
                {
                    return;
                }
            }
            inputValues.Add(new("", "", "", ""));
        }
    }
}
