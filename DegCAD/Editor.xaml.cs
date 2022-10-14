using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public GeometryInputManager InputMgr { get; set; }



        public Editor()
        {
            InitializeComponent();
            GeometryDrawer = new(viewPort);
            InputMgr = new(viewPort, previewVP);
            viewPort.ViewportChanged += Redraw;
            viewPort.ViewportChanged += MovePreviewVP;
        }

        private void MovePreviewVP(object? sender, EventArgs e)
        {
            previewVP.Scale = viewPort.Scale;
            previewVP.OffsetX = viewPort.OffsetX;
            previewVP.OffsetY = viewPort.OffsetY;
        }

        private void Redraw(object? sender, EventArgs e)
        {
            GeometryDrawer.Clear();
            Axis.Draw(GeometryDrawer);
        }

        public void ExecuteCommand(IGeometryCommand command)
        {
            Debug.WriteLine($"Executing command: {command}");
            command.ExecuteAsync(InputMgr.PreviewGd, InputMgr);
        }
    }
}
