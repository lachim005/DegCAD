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

        /// <summary>
        /// Gets a point from the user
        /// </summary>
        /// <param name="preview">Action that will get executed to redraw the preview of the inputed point</param>
        public async Task<Vector2> GetPoint(Action<Vector2, GeometryDrawer> preview)
        {
            //Saves the previewPoint handler so it can be unasigned later
            EventHandler<VPMouseEventArgs> previewPoint = (s, e) =>
            {
                PreviewGd.Clear();
                preview(e.CanvasPos, PreviewGd);
            };

            //Redraws the preview when the user moves the mouse or zooms/pans the viewport
            ViewPort.VPMouseMoved += previewPoint;
            ViewPort.ViewportChanged += previewPoint;

            //Used to await the click
            TaskCompletionSource<Vector2> result = new();

            //Saves the handler so it can be unasigned
            MouseButtonEventHandler viewPortClick = (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Vector2 res = Mouse.GetPosition(ViewPort);
                    result.SetResult(res);
                }
            };

            ViewPort.MouseDown += viewPortClick;

            //Draws the preview so it doesn't appear after the user moves their mouse
            preview(PreviewVp.ScreenToCanvas(Mouse.GetPosition(ViewPort)), PreviewGd);

            //Awaits the user click
            Vector2 mposClick = ViewPort.ScreenToCanvas(await result.Task);

            //Unasignes all events and clears the preview
            ViewPort.VPMouseMoved -= previewPoint;
            ViewPort.MouseDown -= viewPortClick;
            ViewPort.ViewportChanged -= previewPoint;
            PreviewGd.Clear();


            return mposClick;
        }
    }
}
