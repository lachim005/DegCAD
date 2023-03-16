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
    /// Interaction logic for StatusBar.xaml
    /// </summary>
    public partial class EditorStatusBar : UserControl
    {
        public string CommandName
        {
            get => cmdName.Text; 
            set => cmdName.Text = value;
        }

        public string CommandHelp
        {
            get => cmdHelp.Text;
            set => cmdHelp.Text = value;
        }

        public EditorStatusBar()
        {
            InitializeComponent();
            HideCommandStatus();
        }

        public void ShowCommandStatus()
        {
            cmdName.Visibility = Visibility.Visible;    
            cmdHelp.Visibility = Visibility.Visible;
            cmdSeparator.Visibility = Visibility.Visible;
        }

        public void HideCommandStatus()
        {
            cmdName.Visibility = Visibility.Collapsed;
            cmdHelp.Visibility = Visibility.Collapsed;
            cmdSeparator.Visibility = Visibility.Collapsed;
        }
    }
}
