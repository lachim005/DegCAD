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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The editor that the user has currently open
        /// </summary>
        public Editor? ActiveEditor { get; set; }
        private readonly ObservableCollection<Tuple<Editor>> openEditors = new();

        /// <summary>
        /// Used for the number after "Bez názvu" in new document names
        /// </summary>
        private int editorCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            cmdPallete.GenerateCommands(this);

            //Open editor is the user opens a file
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                if (System.IO.File.Exists(args[1]))
                {
                    OpenFileAsync(args[1]);
                }
            }

            editorTabs.ItemsSource = openEditors;
        }

        private void TabSwitched(object sender, SelectionChangedEventArgs e)
        {
            //Home or invalid tab got selected
            if (editorTabs.SelectedIndex < 0)
            {
                ActiveEditor = null;
                homePage.Visibility = Visibility.Visible;
                return;
            }
            //Editor tab got selected
            ActiveEditor = openEditors[editorTabs.SelectedIndex].Item1;
            homePage.Visibility = Visibility.Hidden;
        }

        private void EditorTabCloseClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not Tuple<Editor> ed) return;
            if (CanCloseEditor(ed.Item1))
                openEditors.Remove(ed);
        }

        private void WindowDrop(object sender, DragEventArgs e)
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

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < openEditors.Count; i++)
            {
                editorTabs.SelectedIndex = i;
                var current = openEditors[i].Item1;
                if (!CanCloseEditor(current))
                {
                    e.Cancel = true;
                }
            }
        }

        private void StartTabReorderDrag(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement f) return;
            DragDrop.DoDragDrop(f, f.DataContext, DragDropEffects.Move);
        }
        private void TabDrop(object sender, DragEventArgs e)
        {
            //Gets the dragged and dropped tab
            var dragTab = e.Data.GetData(typeof(Tuple<Editor>)) as Tuple<Editor>;
            if (dragTab is null) return;
            if ((sender as FrameworkElement)?.DataContext is not Tuple<Editor> dropTab) return;

            int dragIndex = openEditors.IndexOf(dragTab);
            int dropIndex = openEditors.IndexOf(dropTab);
            if (dragIndex == -1 || dropIndex == -1) return;
            if (dragIndex == dropIndex) return;

            openEditors.Move(dragIndex, dropIndex);
            editorTabs.SelectedIndex = dropIndex;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement f) return;
            if (f.DataContext is not Tuple<Editor> tab) return;
            if (e.ChangedButton == MouseButton.Middle)
            {
                openEditors.Remove(tab);
            }
        }

        private bool CanCloseEditor(Editor current)
        {
            if (!current.Changed) return true;
            var save = MessageBox.Show(
                this,
                $"Chcete uložit soubor {current.FileName}?",
                "DegCAD",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning
                );
            switch (save)
            {
                case MessageBoxResult.Yes:
                    if (current.FolderPath is null) OpenSaveFileDialog(current);
                    SaveEditorAsync(current);
                    return true;
                case MessageBoxResult.No:
                    return true;
                case MessageBoxResult.Cancel:
                    return false;
            }
            return false;
        }
    }
}
