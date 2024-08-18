using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SelectPaperSizePresetDialog.xaml
    /// </summary>
    public partial class SelectPaperSizePresetDialog : Window
    {
        public static bool lastLandscape = false;

        public PaperSizePreset? SelectedPreset { get; set; }
        public ObservableCollection<PaperSizePreset> Presets { get; set; }

        public SelectPaperSizePresetDialog(Window owner)
        {
            Owner = owner;
            InitializeComponent();
            Presets = new(Settings.PaperSizePresets);
            paperPresetsIC.ItemsSource = Presets;

            landscapeCbx.IsChecked = lastLandscape;
            if (lastLandscape)
            {
                SwapSizes();
            }
        }

        public static PaperSizePreset? GetPaperSizePreset(Window owner)
        {
            SelectPaperSizePresetDialog dialog = new(owner);
            dialog.ShowDialog();
            return dialog.SelectedPreset;
        }

        private void PresetClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not PaperSizePreset preset) return;
            SelectedPreset = preset;
            Close();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lastLandscape = landscapeCbx.IsChecked == true;
        }

        private void LandscapeChanged(object sender, RoutedEventArgs e)
        {
            SwapSizes();
        }

        private void SwapSizes()
        {
            foreach (var item in Presets)
            {
                var tmp = item.Height;
                item.Height = item.Width;
                item.Width = tmp;
            }
        }

        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
