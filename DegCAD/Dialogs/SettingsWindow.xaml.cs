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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private ObservableCollection<Color> DefaultColors { get; set; }

        public SettingsWindow()
        {
            DataContext = this;

            InitializeComponent();
            LoadSettings();

            defaultColorsDisplayIC.ItemsSource = DefaultColors;
        }

        private void LoadSettings()
        {
            darkModeChbx.IsChecked = Settings.DarkMode;

            defaultMongeXDirectionReverse.IsChecked = Settings.DefaultMongeXDirectionLeft;
            defaultLabelFontSizeTbx.Text = Settings.DefaultLabelFontSize.ToString();
            DefaultColors = new(Settings.DefaultColors);
            repeatCommandsCbx.IsChecked = Settings.RepeatCommands;
            nameNewObjectsCbx.IsChecked = Settings.NameNewItems;

            alertGuides.IsChecked = Settings.AlertGuides;
            alertNewVersions.IsChecked = Settings.AlertNewVersions;
            snapLabels.IsChecked = Settings.SnapLabels;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void SaveSettings()
        {
            // Default colors have to be set first because setting dark mode could change them
            Settings.DefaultColors.Clear();
            Settings.DefaultColors.AddRange(DefaultColors);

            Settings.DarkMode = darkModeChbx.IsChecked == true;

            Settings.DefaultMongeXDirectionLeft = defaultMongeXDirectionReverse.IsChecked == true;
            if (int.TryParse(defaultLabelFontSizeTbx.Text, out int fs)) Settings.DefaultLabelFontSize = fs;
            Settings.RepeatCommands = repeatCommandsCbx.IsChecked == true;
            Settings.NameNewItems = nameNewObjectsCbx.IsChecked == true;

            Settings.AlertGuides = alertGuides.IsChecked == true;
            Settings.AlertNewVersions = alertNewVersions.IsChecked == true;
            Settings.SnapLabels = snapLabels.IsChecked == true;

            Settings.SaveSettings();
        }

        public static void OpenDialog()
        {
            SettingsWindow sw = new();
            sw.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditColorsDialog.EditColors(DefaultColors);
        }
    }
}
