using DegCAD.TimelineElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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
using Point = System.Windows.Point;

namespace DegCAD
{
    /// <summary>
    /// Displays the current geometry and handles zooming and panning of the screen
    /// </summary>
    public partial class ViewPort : UserControl
    {
        public const double unitSize = 50;

        public const double ZoomFactor = 1.1;
        public const double MaxZoom = 5;
        public const double MinZoom = .1;

        private bool _allowLabelInteractions = true;

        /// <summary>
        /// The X offset of the canvas
        /// </summary>
        public double OffsetX { get; set; } = -1;
        /// <summary>
        /// The Y offset of the canvas
        /// </summary>
        public double OffsetY { get; set; } = -1;
        /// <summary>
        /// The zoom of the canvas
        /// </summary>
        public double Scale { get; set; } = 1;
        /// <summary>
        /// True if the user is currently panning the canvas
        /// </summary>
        byte Panning { get; set; }
        Vector2 mousePanBegin;
        Vector2 screenPanBegin;

        /// <summary>
        /// Triggers if the viewport gets changed in any way so the contents have to get redrawn
        /// </summary>
        public event EventHandler<VPMouseEventArgs>? ViewportChanged;
        /// <summary>
        /// Triggers when the user moves the mouse on the viewport and contains the mouse coordinates
        /// </summary>
        public event EventHandler<VPMouseEventArgs>? VPMouseMoved;

        /// <summary>
        /// Pixel width of the canvas
        /// </summary>
        public int CWidth => (int)ActualWidth;
        /// <summary>
        /// Pixel height of the canvas
        /// </summary>
        public int CHeight => (int)ActualHeight;

        public bool CanZoom { get; set; } = true;
        public bool AllowLabelInteractions
        {
            get => _allowLabelInteractions;
            set
            {
                _allowLabelInteractions = value;
                labelBlocker.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Timeline Timeline { get; init; }

        public ObservableCollection<ViewportLayer> Layers { get; init; } = new();

        public ViewPort() : this(new()) { }
        public ViewPort(Timeline timeline)
        {
            InitializeComponent();

            Timeline = timeline;

            Layers.CollectionChanged += LayersChanged;
            Timeline.TimelineChanged += TimelineChanged;
            ViewportChanged += ViewPortChanged;
            SizeChanged += ViewPortChanged;

            Layers.Add(new(this));
            Layers.Add(new(this));
            Layers.Add(new(this));
        }

        private void LayersChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            vpLayers.Children.Clear();
            foreach(var layer in Layers)
            {
                vpLayers.Children.Add(layer.Canvas);
            }
        }

        protected void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!CanZoom) return;

            //Calculates zooming
            Vector2 mousePos = e.GetPosition(this);

            Vector2 posBeforeZoom = ScreenToCanvas(mousePos);

            //Zooms in or out
            Scale *= 1 + (e.Delta / 500d);

            //Clamps zoom between minZoom and maxZoom
            Scale = Math.Clamp(Scale, MinZoom, MaxZoom);

            Vector2 posAfterZoom = ScreenToCanvas(mousePos);

            //Calculates canvas offset so the screen zooms around the cursor
            OffsetX += posBeforeZoom.X - posAfterZoom.X;
            OffsetY += posBeforeZoom.Y - posAfterZoom.Y;

            //Triggers the redraw
            ViewportChanged?.Invoke(this, new(ScreenToCanvas(mousePos), mousePos));

            e.Handled = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            //If the user hold the middle or right button, starts panning
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Right)
            {
                Point mousePos = e.GetPosition(this);

                Panning++;
                if (Panning > 1) return;

                mousePanBegin = mousePos;
                screenPanBegin = (OffsetX, OffsetY);
                //Adds the pan handler to the mouse move event to update the screen on mouse move
                MouseMove += Pan;
                CaptureMouse();
                e.Handled = true;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //If the middle or right button got released, stops panning
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Right)
            {
                Panning--;
                if (Panning > 0) return;
                MouseMove -= Pan;
                ReleaseMouseCapture();
                Vector2 mousePos = Mouse.GetPosition(this);
                ViewportChanged?.Invoke(this, new(ScreenToCanvas(mousePos), mousePos));
                e.Handled = true;
            }
        }

        private void Pan(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);

            OffsetX = screenPanBegin.X - ((mousePos.X - mousePanBegin.X) / unitSize / Scale);
            OffsetY = screenPanBegin.Y - ((mousePos.Y - mousePanBegin.Y) / unitSize / Scale);

            //Uncommenting this will probably result in better performance, but looks worse
            //(redraws the canvas only every couple of pixel so it look laggy)
            //if ((Math.Abs(mousePos.X - mousePanBegin.X) % 10) == 0 ||
            //    (Math.Abs(mousePos.Y - mousePanBegin.Y) % 10) == 0)
            ViewportChanged?.Invoke(this, new(ScreenToCanvas(mousePos), mousePos));
            e.Handled = true;
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (Panning < 1) return;
            Panning = 0;
            MouseMove -= Pan;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Vector2 mousePos = Mouse.GetPosition(this);
            ViewportChanged?.Invoke(this, new(ScreenToCanvas(mousePos), mousePos));
        }

        #region Point conversion
        /// <summary>
        /// Converts a point on the canvas into a point on the screen
        /// </summary>
        public Vector2 CanvasToScreen(Vector2 canvas)
        {
            Vector2 res = new();
            res.X = (canvas.X - OffsetX) * Scale * unitSize + (ActualWidth / 2);
            res.Y = (canvas.Y - OffsetY) * Scale * unitSize + (ActualHeight / 2);
            return res;
        }
        /// <summary>
        /// Converts a point on the screen into a point on the canvas
        /// </summary>
        public Vector2 ScreenToCanvas(Vector2 screen)
        {
            Vector2 res = new();
            res.X = ((screen.X - (ActualWidth / 2)) / unitSize / Scale) + OffsetX;
            res.Y = ((screen.Y - (ActualHeight / 2)) / unitSize / Scale) + OffsetY;
            return res;
        }
        #endregion

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            //Invokes VPMouseMoved with coordinates
            Vector2 mpos = e.GetPosition(this);
            VPMouseMoved?.Invoke(this, new(ScreenToCanvas(mpos), mpos));
        }

        private void TimelineChanged(object? sender, EventArgs e)
        {
            Redraw();
        }

        public void ViewPortChanged(object? sender, EventArgs e)
        {
            Redraw();
        }

        public void Redraw()
        {
            foreach (var cmd in Timeline.CommandHistory)
            {
                for (int i = 0; i < cmd.Items.Length; i++)
                {
                    if (cmd.Items[i] is not GeometryElement ge) continue;
                    ge.Draw();
                }
            }
        }

        public void CenterContent()
        {

            //Getting the corners
            double minX = 0; double minY = 0;
            double maxX = 0; double maxY = 0;
            void AddPoint(Vector2 pt)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Y > maxY) maxY = pt.Y;
            }
            foreach (var cmd in Timeline.CommandHistory)
            {
                foreach (var item in cmd.Items)
                {
                    if (item is not GeometryElement ge || !ge.IsVisible) continue;
                    if (item is Arc arc)
                    {
                        Vector2 rad = (arc.Circle.Radius, arc.Circle.Radius);
                        AddPoint(arc.Center + rad);
                        AddPoint(arc.Center - rad);
                    }
                    else if (item is Circle cir)
                    {
                        Vector2 rad = (cir.Circle2.Radius, cir.Circle2.Radius);
                        AddPoint(cir.Circle2.Center + rad);
                        AddPoint(cir.Circle2.Center - rad);
                    }
                    else if (item is CRSpline spline)
                    {
                        for (int i = 1; i < spline.Points.Count - 1; i++)
                        {
                            AddPoint(spline.Points[i]);
                        }
                    }
                    else if (item is TimelineElements.Ellipse ell)
                    {
                        AddPoint(ell.P1);
                        AddPoint(ell.P2);
                    }
                    else if (item is HalfLine hln)
                    {
                        AddPoint(hln.StartPoint);
                        AddPoint(hln.StartPoint + hln.Direction);
                    }
                    else if (item is Hyperbola hyp)
                    {
                        for (int i = 1; i < hyp.HyperbolaPoints.Count - 1; i++)
                        {
                            AddPoint(hyp.HyperbolaPoints[i]);
                        }
                    }
                    else if (item is InfiniteLine inf)
                    {
                        AddPoint(inf.StartPoint);
                        AddPoint(inf.StartPoint + inf.Direction);
                    }
                    else if (item is TimelineElements.Label lbl)
                    {
                        AddPoint(lbl.Position);
                    }
                    else if (item is LineProjection lne)
                    {
                        AddPoint(lne.Line.Point);
                        AddPoint(lne.Line.Point + lne.Line.DirectionVector);
                    }
                    else if (item is TimelineElements.LineSegment seg)
                    {
                        AddPoint(seg.P1);
                        AddPoint(seg.P2);
                    }
                    else if (item is Parabola pbl)
                    {
                        AddPoint(pbl.Vertex);
                        if (!pbl.Infinite) AddPoint(pbl.End);
                    }
                    else if (item is TimelineElements.Point pnt)
                    {
                        AddPoint(pnt.Coords);
                    }
                }
            }

            InvalidateVisual();
            double w = maxX - minX;
            double h = maxY - minY;

            if (CanZoom)
            {
                double scaleX = (ActualWidth / (w + 5) / unitSize);
                double scaleY = (ActualHeight / (h + 5) / unitSize);
                Scale = Math.Clamp(Math.Min(scaleX, scaleY), MinZoom, MaxZoom);
            }

            OffsetX = (maxX + minX) / 2; 
            OffsetY = (maxY + minY) / 2;

            Redraw();
        }
        public void SwapWhiteAndBlack()
        {
            foreach (var c in Timeline.CommandHistory.Concat(Timeline.UndoneCommands))
            {
                foreach (var i in c.Items)
                {
                    if (i is not GeometryElement ge) continue;
                    if (ge.Style.Color == Colors.Black)
                    {
                        ge.Style = new() { Color = Colors.White, LineStyle = ge.Style.LineStyle, Thickness = ge.Style.Thickness };
                    } else if (ge.Style.Color == Colors.White)
                    {
                        ge.Style = new() { Color = Colors.Black, LineStyle = ge.Style.LineStyle, Thickness = ge.Style.Thickness };
                    }
                }
            }
        }

        public ViewPort Clone()
        {
            var clonedTl = Timeline.Clone();
            ViewPort vp = new(clonedTl);
            clonedTl.SetViewportLayer(vp.Layers[1]);
            vp.OffsetX = OffsetX;
            vp.OffsetY = OffsetY;
            vp.CanZoom = CanZoom;
            vp.Scale = Scale;
            return vp;
        }
    }

    /// <summary>
    /// Event args containing mouse position on the canvas
    /// </summary>
    public class VPMouseEventArgs : EventArgs
    {
        public Vector2 CanvasPos { get; init; }
        public Vector2 ScreenPos { get; init; }
        public VPMouseEventArgs(Vector2 canvasPos, Vector2 screenPos)
        {
            CanvasPos = canvasPos;
            ScreenPos = screenPos;
        }
    }
}
