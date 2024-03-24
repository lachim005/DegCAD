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
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        MainWindow mw;
        public HomeScreen(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();
        }

        private void NewPlaneClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(ProjectionType.Plane));
        }

        private void NewMongeClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(ProjectionType.Monge));
        }

        private void NewAxoClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(ProjectionType.Axonometry));
        }

        private void NewCompositionClick(object sender, RoutedEventArgs e)
        {
            mw.AddComposition(new(mw));
        }
    }
}
