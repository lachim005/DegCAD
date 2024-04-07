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
    public partial class NewDrawingDialog : Window
    {
        public ProjectionType? ProjectionType { get; set; } = null;

        public NewDrawingDialog()
        {
            InitializeComponent();
        }

        private void planeBtn_Click(object sender, RoutedEventArgs e)
        {
            ProjectionType = DegCAD.ProjectionType.Plane;
            Close();
        }

        private void mongeBtn_Click(object sender, RoutedEventArgs e)
        {
            ProjectionType = DegCAD.ProjectionType.Monge;
            Close();
        }

        private void axonometryBtn_Click(object sender, RoutedEventArgs e)
        {
            ProjectionType = DegCAD.ProjectionType.Axonometry;
            Close();
        }
    }
}
