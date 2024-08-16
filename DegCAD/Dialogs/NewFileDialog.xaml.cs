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
    /// Interaction logic for NewFileDialog.xaml
    /// </summary>
    public partial class NewFileDialog : Window
    {
        MainWindow mw;

        public NewFileDialog(MainWindow mw)
        {
            Owner = mw;
            InitializeComponent();
            this.mw = mw;
        }

        private void NewPlaneClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(this, ProjectionType.Plane));
            Close();
        }

        private void NewMongeClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(this, ProjectionType.Monge));
            Close();
        }

        private void NewAxoClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(this, ProjectionType.Axonometry));
            Close();
        }

        private void NewCompositionClick(object sender, RoutedEventArgs e)
        {
            mw.AddComposition(new(mw));
            Close();
        }

        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
