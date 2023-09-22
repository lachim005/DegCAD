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

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for FullscreenPresenter.xaml
    /// </summary>
    public partial class FullscreenPresenter : Window
    {
        public FullscreenPresenter(Control c)
        {
            InitializeComponent();
            contentGrid.Children.Add(c);
        }

        private void CloseButtonClick(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
