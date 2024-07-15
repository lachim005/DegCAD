using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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

        public EditColorsDialog(IList<Color> colors)
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
            if (sender is not Button btn) return;
            if (btn.DataContext is not Tuple<Color, Brush> tuple) return;

            var newCol = ColorPicker.EditColor(tuple.Item1);
            colors[colors.IndexOf(tuple)] = new(newCol, new SolidColorBrush(newCol));
        }

        private void AddColor(object sender, RoutedEventArgs e)
        {
            var c = ColorPicker.GetColor();
            if (c is null) return;
            var newColor = (Color)c;
            colors.Add(new(newColor, new SolidColorBrush(newColor)));
        }
        private async void ImportColors(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Filter = "DegCAD projekt|*.dgproj|Všechny soubory|*.*"
            };
            if (ofd.ShowDialog() != true) return;
            Editor ed;
            try
            {
                ed = await EditorLoader.CreateFromFile(ofd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při načítání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach(var c in ed.styleSelector.ColorPalette)
            {
                colors.Add(new(c, new SolidColorBrush(c)));
            }
        }
        private void SaveColors(object sender, RoutedEventArgs e)
        {
            saveColors = true;
            Close();
        }
        private void CancelDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region Rearanging colors
        private void StartColorReorderDrag(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement f) return;
            DragDrop.DoDragDrop(f, f.DataContext, DragDropEffects.Move);
        }

        private void ColorDrop(object sender, DragEventArgs e)
        {
            //Gets the dragged and dropped color
            var dragColor = e.Data.GetData(typeof(Tuple<Color, Brush>)) as Tuple<Color, Brush>;
            if (dragColor is null) return;
            if ((sender as FrameworkElement)?.DataContext is not Tuple<Color, Brush> dropColor) return;

            int dragIndex = colors.IndexOf(dragColor);
            int dropIndex = colors.IndexOf(dropColor);
            if (dragIndex == -1 || dropIndex == -1) return;

            colors.Move(dragIndex, dropIndex);
        } 
        #endregion

        public static void EditColors(IList<Color> colors)
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