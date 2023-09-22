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

namespace DegCAD.Guides
{
    /// <summary>
    /// Interaction logic for OpenGuideDialog.xaml
    /// </summary>
    public partial class OpenGuideDialog : Window
    {
        public OpenGuideDialog()
        {
            InitializeComponent();
        }

        private void OpenEditor(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        private void OpenGuide(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        public bool Result { get; set; } = false;

        public static bool OpenDialog()
        {
            OpenGuideDialog ogd = new();
            ogd.ShowDialog();
            return ogd.Result;
        }
    }
}
