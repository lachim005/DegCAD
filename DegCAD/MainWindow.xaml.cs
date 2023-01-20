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
        ObservableCollection<Editor> openEditors = new();

        private int editorCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            cmdPallete.GenerateCommands(this);
            openEditors.CollectionChanged += OpenEditorsChanged;
        }

        private void OpenEditorsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var index = editorTabs.SelectedIndex;
            while (editorTabs.Items.Count > 1)
            {
                editorTabs.Items.RemoveAt(1);
            }
            foreach (Editor editor in openEditors)
            {
                StackPanel stp = new() { Orientation = Orientation.Horizontal};
                stp.Children.Add(new TextBlock() { Text = editor.FileName, VerticalAlignment = VerticalAlignment.Center });
                Button closeBtn = new();
                closeBtn.Click += (s, e) => openEditors.Remove(editor);
                closeBtn.Content = "❌";
                closeBtn.Background = Brushes.Transparent;
                closeBtn.BorderThickness = new(0);
                closeBtn.Width = 20;
                closeBtn.Height = 20;
                stp.Children.Add(closeBtn);
                editorTabs.Items.Add(new TabItem() { Header = stp, Content = editor });
            }
            if (index > editorTabs.Items.Count - 1)
                index--;
            editorTabs.SelectedIndex = index;
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
