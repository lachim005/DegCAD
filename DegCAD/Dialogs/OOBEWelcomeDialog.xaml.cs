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

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for OOBEWelcomeDialog.xaml
    /// </summary>
    public partial class OOBEWelcomeDialog : Window
    {
        public OOBEWelcomeDialog()
        {
            InitializeComponent();
        }

        private void SkipClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Next1Click(object sender, RoutedEventArgs e)
        {
            page1.Visibility = Visibility.Collapsed;
            page2.Visibility = Visibility.Visible;
        }
        private void Next2Click(object sender, RoutedEventArgs e)
        {
            page2.Visibility = Visibility.Collapsed;
#if RELEASE_PORTABLE
            portableDisclaimer.Visibility = Visibility.Visible;
#else
            page3.Visibility = Visibility.Visible;
#endif
        }
        private void NextPortable(object sender, RoutedEventArgs e)
        {
            portableDisclaimer.Visibility = Visibility.Collapsed;
            page3.Visibility = Visibility.Visible;
        }

        private void LightThemeChecked(object sender, RoutedEventArgs e)
        {
            Settings.DarkMode = false;
        }

        private void DarkThemeChecked(object sender, RoutedEventArgs e)
        {
            Settings.DarkMode = true;
        }
    }
}
