using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for PaperSizesPresetEditor.xaml
    /// </summary>
    public partial class PaperSizesPresetEditor : Window
    {
        public ObservableCollection<PaperSizePreset> Presets { get; init; }
        public bool Save { get; set; } = false;

        public PaperSizesPresetEditor(List<PaperSizePreset> presets)
        {
            InitializeComponent();

            Presets = new(presets);

            paperPresetsIC.ItemsSource = this.Presets;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement f) return;
            DragDrop.DoDragDrop(f, f.DataContext, DragDropEffects.Move | DragDropEffects.Scroll);
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            //Gets the dragged and dropped color
            var dragPaper = e.Data.GetData(typeof(PaperSizePreset)) as PaperSizePreset;
            if (dragPaper is null) return;
            if ((sender as FrameworkElement)?.DataContext is not PaperSizePreset dropPaper) return;

            int dragIndex = Presets.IndexOf(dragPaper);
            int dropIndex = Presets.IndexOf(dropPaper);
            if (dragIndex == -1 || dropIndex == -1) return;

            Presets.Move(dragIndex, dropIndex);
        }

        private void DeletePresetClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not PaperSizePreset preset) return;

            Presets.Remove(preset);
        }

        private void AddPresetClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(presetNameTbx.Text))
            {
                MessageBox.Show("Zadejte platný název", img: MessageBoxImage.Error);
                return;
            }
            if (!double.TryParse(paperWidthTbx.Text, out var width) || width < 1)
            {
                MessageBox.Show("Zadejte platnou šířku papíru", img: MessageBoxImage.Error);
                return;
            }
            if (!double.TryParse(paperHeightTbx.Text, out var height) || height < 1)
            {
                MessageBox.Show("Zadejte platnou výšku papíru", img: MessageBoxImage.Error);
                return;
            }
            
            Presets.Add(new(presetNameTbx.Text, width, height));
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Save = true;
            Close();
        }
    }
}
