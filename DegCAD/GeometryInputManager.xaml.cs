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
    /// Interaction logic for GeometryInputManager.xaml
    /// </summary>
    public partial class GeometryInputManager : UserControl
    {
        public ViewPort PreviewVp { get; set; }
        public ViewPort ViewPort { get; set; }
        public GeometryDrawer PreviewGd { get; set; }
        public Style PreviewStyle { get; set; } = new Style() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };

        public GeometryInputManager(ViewPort viewPort, ViewPort previewVp)
        {
            InitializeComponent();
            PreviewGd = new(previewVp);
            ViewPort = viewPort;
            PreviewVp = previewVp;
        }
    }
}
