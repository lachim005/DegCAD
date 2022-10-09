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
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        public GeometryDrawer GeometryDrawer { get; set; }



        public Editor()
        {
            InitializeComponent();
            GeometryDrawer = new(viewPort);
        }

        private void Redraw(object? sender, EventArgs e)
        {
            GeometryDrawer.Clear();
            Axis.Draw(GeometryDrawer);
        }
    }
}
