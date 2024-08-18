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
    public partial class PointCoordinateInputDialog2D : Window
    {
        ObservableCollection<PointInputStruct> inputValues = new() { new("","","") };

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
            public double Y { get; set; }
        }
        record PointInputStruct(string Label, string X, string Y);

        public PointCoordinateInputDialog2D(Window owner)
        {
            Owner = owner;
            InitializeComponent();


            coordInputIc.ItemsSource = inputValues;
        }

        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void Confirm(object sender, ExecutedRoutedEventArgs e)
        {
            // Focuses on something because if the user presses enter and the current textbox doesn't lose focus, the binding doesn't update
            confirmBtn.Focus();

            foreach (var item in inputValues)
            {
                if (IsEmpty(item)) 
                    continue;

                if (!double.TryParse(item.X, out double x) || !double.TryParse(item.Y, out double y))
                {
                    MessageBox.Show(this, "U každého bodu musí být vyplněna souřadnice X a Y");
                    return;
                }

                Points.Add(new() { Label = item.Label, X = x, Y = y });
            }
            if (Points.Count != 0) Canceled = false;
            Close();
        }

        private bool IsEmpty(PointInputStruct item)
        {
            if (string.IsNullOrWhiteSpace(item.Label) && string.IsNullOrWhiteSpace(item.X) && string.IsNullOrWhiteSpace(item.Y))
                return true;
            return false;
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
            inputValues.Add(new("", "", ""));
        }
    }
}
