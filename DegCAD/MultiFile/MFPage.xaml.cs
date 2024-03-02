﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFPage.xaml
    /// </summary>
    public partial class MFPage : UserControl
    {
        public const double unitSize = 2;

        public const double ZoomFactor = 1.1;
        public const double MaxZoom = 50;
        public const double MinZoom = .05;

        private double _paperWidth = 210;
        private double _paperHeight = 297;

        private bool _bordersVisible = true;

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
        bool Panning { get; set; }
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

        public double PaperWidth
        {
            get => _paperWidth;
            set
            {
                _paperWidth = value;
                UpdatePaperSize();
            }
        }
        public double PaperHeight
        {
            get => _paperHeight;
            set
            {
                _paperHeight = value;
                UpdatePaperSize();
            }
        }

        public List<MFContainer> Items { get; init; } = new();
        private MFContainer? _selectedItem;

        public MFContainer? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                SelectionChanged?.Invoke(this, value);
            }
        }
        public event EventHandler<MFContainer?>? SelectionChanged;
        public event EventHandler<MFContainer?>? ContainerUpdating;
        public event EventHandler<TransformChange>? ContainerUpdated;

        public bool BordersVisible
        {
            get => _bordersVisible;
            set
            {
                _bordersVisible = value;
                foreach (var c in Items)
                {
                    c.BorderVisible = value;
                }
            }
        }

        public MFPage()
        {
            InitializeComponent();


            ViewportChanged += ViewPortChanged;
            SizeChanged += ViewPortChanged;
        }

        public void AddItem(MFContainer container)
        {
            Items.Add(container);
            canvas.Children.Add(container);
            container.Selected += ContainerSelected;
            container.Deselected += ContainerDeselected;
            container.Updating += OnContainerUpdating;
            container.Updated += OnContainerUpdated;
            container.BorderVisible = BordersVisible;
        }

        private void OnContainerUpdated(object? sender, TransformChange e)
        {
            ContainerUpdated?.Invoke(sender, e);
        }

        public void RemoveItem(MFContainer container)
        {
            Items.Remove(container);
            canvas.Children.Remove(container);
            container.Selected -= ContainerSelected;
            container.Deselected -= ContainerDeselected;
            container.Updating -= OnContainerUpdating;
            container.Updated -= OnContainerUpdated;
        }

        private void ContainerSelected(object? sender, EventArgs e)
        {
            if (sender is not MFContainer container) return;
            SelectedItem?.Deselect();
            SelectedItem = container;
        }
        private void ContainerDeselected(object? sender, EventArgs e)
        {
            if (sender is not MFContainer container) return;
            SelectedItem = null;
        }
        private void OnContainerUpdating(object? sender, EventArgs e)
        {
            Redraw();
            ContainerUpdating?.Invoke(this, sender as MFContainer);
        }

        protected void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!CanZoom) return;

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
            ViewportChanged?.Invoke(this, new(ScreenToCanvas(mousePos), mousePos));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            //If the user hold the middle or right button, starts panning
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Right)
            {
                Point mousePos = e.GetPosition(this);

                Panning = true;
                mousePanBegin = mousePos;
                screenPanBegin = (OffsetX, OffsetY);
                //Adds the pan handler to the mouse move event to update the screen on mouse move
                MouseMove += Pan;
                CaptureMouse();
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                SelectedItem?.Deselect();
                SelectedItem = null;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //If the middle or right button got released, stops panning
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Right)
            {
                Panning = false;
                MouseMove -= Pan;
                ReleaseMouseCapture();
                Vector2 mousePos = Mouse.GetPosition(this);
                ViewportChanged?.Invoke(this, new(ScreenToCanvas(mousePos), mousePos));
            }
        }

        private void Pan(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);

            OffsetX = screenPanBegin.X - ((mousePos.X - mousePanBegin.X) / unitSize / Scale);
            OffsetY = screenPanBegin.Y - ((mousePos.Y - mousePanBegin.Y) / unitSize / Scale);

            ViewportChanged?.Invoke(this, new(ScreenToCanvas(mousePos), mousePos));
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
            if (!IsLoaded) return;
            UpdatePaperSize();
            Canvas.SetLeft(paperBg, -OffsetX * Scale * unitSize);
            Canvas.SetTop(paperBg, -OffsetY * Scale * unitSize);

            foreach (var item in Items)
            {
                item.Width = item.CWidth * Scale * unitSize;
                item.Height = item.CHeight * Scale * unitSize;
                Canvas.SetLeft(item, (item.CX - OffsetX) * Scale * unitSize);
                Canvas.SetTop(item, (item.CY - OffsetY) * Scale * unitSize);

                item.Item.ViewUpdated(OffsetX, OffsetY, Scale);
            }
        }

        public void UpdatePaperSize()
        {
            paperBg.Width = PaperWidth * Scale * unitSize;
            paperBg.Height = PaperHeight * Scale * unitSize;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = false;
                return;
            }
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files is null) return;
            e.Handled = true;

            var mPos = ScreenToCanvas(e.GetPosition(this));
            int offset = 0;
            foreach (var file in files)
            {
                if (System.IO.Path.GetExtension(file) != ".dgproj") continue;

                var ed = await EditorLoader.CreateFromFile(file);


                AddItem(new(this, new MFDrawing(ed)) { CX = mPos.X + offset, CY = mPos.Y });
                offset += 100;
                Redraw();
            }
        }

        public double GetMaxY()
        {
            double max = 0;
            foreach(var cont in Items)
            {
                var newVal = cont.CY + cont.CHeight;
                if (newVal > max) max = newVal;
            }
            return max;
        }

        public void SwapWhiteAndBlack()
        {
            foreach (var item in Items)
            {
                item.Item.SwapWhiteAndBlack();
            }
        }
    }

    public class MFPageModel : INotifyPropertyChanged
    {
        private int _index;
        private bool _isButtonEnabled = true;

        public MFPage Page { get; init; }
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                PropertyChanged?.Invoke(this, new(nameof(Index)));
            }
        }
        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set
            {
                _isButtonEnabled = value;
                PropertyChanged?.Invoke(this, new(nameof(IsButtonEnabled)));
            }
        }

        public MFPageModel(MFPage page, int index = 0)
        {
            Page = page;
            Index = index;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}