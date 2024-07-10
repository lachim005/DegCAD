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
using System.Windows.Shapes;

namespace DegCAD.DebugMenu
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        MainWindow _mainWindow;
        public DebugWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void ShowView(Control view, string title)
        {
            homeScreen.Visibility = Visibility.Collapsed;
            altViewGrid.Visibility = Visibility.Visible;
            altViewTitle.Content = title;
            altView.Child = view;
        }
        private void ExitView(object sender, RoutedEventArgs e)
        {
            altViewGrid.Visibility = Visibility.Collapsed;
            homeScreen.Visibility = Visibility.Visible;
            altView.Child = null;
        }

        private void ShowFileContents(object sender, RoutedEventArgs e)
        {
            ShowView(new ShowFileContents(), "Zobrazit obsah souboru");
        }

        private void CrashButtonClick(object sender, RoutedEventArgs e)
        {
            throw new Exception("Vyvoláno umělé spadnutí");
        }
    }
}
