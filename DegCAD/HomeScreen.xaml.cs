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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        MainWindow mw;
        public HomeScreen(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            // Sets the greeting depending on the time of day
            greetingsTextblock.Text = DateTime.Now.Hour switch
            {
                (< 4) or (>= 18) => "Dobrý večer,",
                < 11 => "Dobré ráno,",
                < 13 => "Dobré poledne,",
                _ => "Dobré odpoledne,"
            } + " vítejte v DegCADu!";

            recentFilesIC.ItemsSource = Settings.RecentFiles.Files;
        }

        private void NewPlaneClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(ProjectionType.Plane));
        }

        private void NewMongeClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(ProjectionType.Monge));
        }

        private void NewAxoClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(ProjectionType.Axonometry));
        }

        private void NewCompositionClick(object sender, RoutedEventArgs e)
        {
            mw.AddComposition(new(mw));
        }

        private void RemoveRecentFile(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;
            Settings.RecentFiles.RemoveFile(rf.Path);
        }

        private void OpenRecentFileInBackground(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;

            mw.OpenFileAsync(rf.Path, false);
        }

        private void RecentFileClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;

            mw.OpenFileAsync(rf.Path);
        }

        private void RecentFilePreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Open the recent file in the background on middle click
            if (e.ChangedButton != MouseButton.Middle) return;
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;

            mw.OpenFileAsync(rf.Path, false);
            e.Handled = true;
        }

        private void ClearRecentFilesClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete vymazat historii souborů?", "DegCAD", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Settings.RecentFiles.Clear();
            }
        }
    }
}
