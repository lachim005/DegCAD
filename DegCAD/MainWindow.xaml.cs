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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The editor that the user has currently open
        /// </summary>
        public Editor? ActiveEditor { get; set; }
        List<Editor> openEditors = new();

        public MainWindow()
        {
            InitializeComponent();
            cmdPallete.GenerateCommands(this);
        }

        private void AddEditor(Editor editor)
        {
            openEditors.Add(editor);
            editorTabs.Items.Add(new TabItem() { Header = "Bez názvu", Content = editor });
            editorTabs.SelectedIndex = editorTabs.Items.Count - 1;
        }

        private void CloseEditor(Editor editor)
        {
            var editorIndex = openEditors.IndexOf(editor);
            if (editorIndex == -1) return;
            editorTabs.Items.RemoveAt(editorIndex + 1);
            openEditors.RemoveAt(editorIndex);
            editorTabs.SelectedIndex = editorIndex;
            if (editorIndex + 1 < editorTabs.Items.Count)
                editorTabs.SelectedIndex += 1;
        }

        private void TabSwitched(object sender, SelectionChangedEventArgs e)
        {
            if (editorTabs.SelectedIndex < 1)
            {
                ActiveEditor = null;
                return;
            }
            ActiveEditor = openEditors[editorTabs.SelectedIndex - 1];
        }
    }
}
