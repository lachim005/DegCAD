using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace DegCAD.DebugMenu
{
    /// <summary>
    /// Interaction logic for ShowFileContents.xaml
    /// </summary>
    public partial class ShowFileContents : UserControl
    {
        public ShowFileContents()
        {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = false;
                return;
            }
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files is null) return;
            e.Handled = true;

            foreach (var file in files)
            {
                OpenFileAsync(file);
            }
        }

        private void OpenFileBtn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "DegCAD projekt|*.dgproj|Všechny soubory|*.*";
            if (ofd.ShowDialog() != true) return;

            OpenFileAsync(ofd.FileName);
        }

        private async void OpenFileAsync(string file)
        {
            await Task.Delay(1);

            List<Tuple<string, string>> files = new();

            try
            {
                var unpacked = EditorLoader.Unpack(file);
                var filesPaths = Directory.GetFiles(unpacked);
                foreach (var filePath in filesPaths)
                {
                    files.Add(new(
                        Path.GetFileName(filePath),
                        File.ReadAllText(filePath)
                        ));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při načítání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            homeScreen.Visibility = Visibility.Hidden;
            fileScreen.Visibility = Visibility.Visible;
            fileName.Text = file;
            fileTabs.ItemsSource = files;
        }
    }
}
