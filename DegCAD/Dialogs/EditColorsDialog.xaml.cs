using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for EditColorsDialog.xaml
    /// </summary>
    public partial class EditColorsDialog : Window
    {
        internal ObservableCollection<Tuple<Color, Brush>> colors;
        public bool saveColors = false;

        public EditColorsDialog(List<Color> colors)
        {
            InitializeComponent();

            this.colors = new();
            foreach (var c in colors) 
            {
                this.colors.Add(new(c, new SolidColorBrush(c)));
            }
            colorsList.ItemsSource = this.colors;
        }

        private void RemoveColor(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not Tuple<Color, Brush> tuple) return;
            colors.Remove(tuple);
        }

        private void EditColor(object sender, RoutedEventArgs e)
        {

        }

        private void AddColor(object sender, RoutedEventArgs e)
        {

        }
        private void SaveColors(object sender, RoutedEventArgs e)
        {
            saveColors = true;
            Close();
        }

        public static void EditColors(List<Color> colors)
        {
            var dialog = new EditColorsDialog(colors);
            dialog.ShowDialog();
            if (!dialog.saveColors) return;
            colors.Clear();
            foreach(var c in dialog.colors)
            {
                colors.Add(c.Item1);
            }
        }
    }

}