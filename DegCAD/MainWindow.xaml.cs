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
                    OpenFile(args[1]);
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
                return;
            }
            //Editor tab got selected
            ActiveEditor = openEditors[editorTabs.SelectedIndex].Item1;
        }

        private void EditorTabCloseClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not Tuple<Editor> ed) return;

            openEditors.Remove(new(ed.Item1));
        }
    }
}
