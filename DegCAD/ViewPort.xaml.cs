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
    /// Displays the current geometry and handles zooming and panning of the screen
    /// </summary>
    public partial class ViewPort : UserControl
    {
        const double unitSize = 50;

        const double ZoomFactor = 1.1;
        const double MaxZoom = 5;
        const double MinZoom = .1;

        /// <summary>
        /// The X offset of the canvas
        /// </summary>
        public double OffsetX { get; private set; } = -1;
        /// <summary>
        /// The Y offset of the canvas
        /// </summary>
        public double OffsetY { get; private set; } = -1;
        /// <summary>
        /// The zoom of the canvas
        /// </summary>
        public double Scale { get; private set; } = 1;
        /// <summary>
        /// True if the user is currently panning the canvas
        /// </summary>
        bool Panning { get; set; }
        Vector2 mousePanBegin;
        Vector2 screenPanBegin;

        /// <summary>
        /// Triggers if the viewport gets changed in any way so the contents have to get redrawn
        /// </summary>
        public event EventHandler? ViewportChanged;

        /// <summary>
        /// Pixel width of the canvas
        /// </summary>
        public int CWidth => WBmp.PixelWidth;
        /// <summary>
        /// Pixel height of the canvas
        /// </summary>
        public int CHeight => WBmp.PixelHeight;
        /// <summary>
        /// The writable bitmap that gets displayed on the screen
        /// </summary>
        public WriteableBitmap WBmp { get; private set; }


        public ViewPort()
        {
            InitializeComponent();
            //Create a new Writable bitmap and displays it
            WBmp = BitmapFactory.New((int)ActualWidth, (int)ActualHeight);
            imageDisplay.Source = WBmp;
        }

        protected void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Calculates zooming
            Vector2 mousePos = e.GetPosition(this);

            Vector2 posBeforeZoom = ScreenToCanvas(mousePos);

            //Zooms in or out
            if (e.Delta > 0)
            {
                Scale *= ZoomFactor;
                Scale *= ZoomFactor;
            }
            else
            {
                Scale /= ZoomFactor;
                Scale /= ZoomFactor;
            }

            //Clamps zoom between minZoom and maxZoom
            Scale = Math.Clamp(Scale, MinZoom, MaxZoom);

            Vector2 posAfterZoom = ScreenToCanvas(mousePos);

            //Calculates canvas offset so the screen zooms around the cursor
            OffsetX += posBeforeZoom.X - posAfterZoom.X;
            OffsetY += posBeforeZoom.Y - posAfterZoom.Y;

            //Triggers the redraw
            ViewportChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            //If the user hold the middle button, starts panning
            if (e.ChangedButton == MouseButton.Middle)
            {
                Point mousePos = e.GetPosition(this);

                Panning = true;
                mousePanBegin = mousePos;
                screenPanBegin = (OffsetX, OffsetY);
                //Adds the pan handler to the mouse move event to update the screen on mouse move
                MouseMove += Pan;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //If the middle button got released, stops panning
            if (e.ChangedButton == MouseButton.Middle)
            {
                Panning = false;
                MouseMove -= Pan;
                ViewportChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Pan(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);

            OffsetX = screenPanBegin.X - ((mousePos.X - mousePanBegin.X) / unitSize / Scale);
            OffsetY = screenPanBegin.Y - ((mousePos.Y - mousePanBegin.Y) / unitSize / Scale);

            //Uncommenting this will probably result in better performance, but looks worse
            //(redraws the canvas only every couple of pixel so it look laggy)
            //if ((Math.Abs(OffsetX - screenPanBegin.X) % 10) == 0 ||
            //    (Math.Abs(OffsetY - screenPanBegin.Y) % 10) == 0)
                ViewportChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            //Stops panning if the mouse leaves the canvas
            if (Panning)
            {
                Panning = false;
                MouseMove -= Pan;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //If the viewport gets resized, resizes the writable bitmap to fit it perfectly
            WBmp = BitmapFactory.New((int)ActualWidth, (int)ActualHeight);
            imageDisplay.Source = WBmp;
            ViewportChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Point conversion
        /// <summary>
        /// Converts a point on the canvas into a point on the screen
        /// </summary>
        public Vector2 CanvasToScreen(Vector2 canvas)
        {
            Vector2 res = new();
            res.X = (canvas.X - OffsetX) * Scale * unitSize;
            res.Y = (canvas.Y - OffsetY) * Scale * unitSize;
            return res;
        }
        /// <summary>
        /// Converts a point on the screen into a point on the canvas
        /// </summary>
        public Vector2 ScreenToCanvas(Vector2 screen)
        {
            Vector2 res = new();
            res.X = (screen.X / unitSize / Scale) + OffsetX;
            res.Y = (screen.Y / unitSize / Scale) + OffsetY;
            return res;
        }
        #endregion
    }
}
