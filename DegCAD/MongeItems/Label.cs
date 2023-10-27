using DegCAD.Dialogs;
using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// A label labeling a thing that's supposed to be labeled
    /// </summary>
    public class Label : IMongeItem
    {
        private Style _style;
        private string _labelText = "";
        private string _subscript = "";
        private string _superscript = "";

        public string LabelText 
        {
            get => _labelText;
            set { 
                _labelText = value;
                _lblTbl.Text = _labelText;
            } 
        }
        public string Subscript
        {
            get => _subscript;
            set
            {
                _subscript = value;
                _subTbl.Text = _subscript;
            }
        }
        public string Superscript
        {
            get => _superscript;
            set
            {
                _superscript = value;
                _supTbl.Text = _superscript;
            }
        }

        public int FontSize { get; set; }

        public Vector2 Position { get; set; }

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricSegment2[] SnapableLines { get; } = new ParametricSegment2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _prevStyle = value;
                SetStyle(Style);
            }
        }

        private void SetStyle(Style style)
        {
            _lblTbl.Foreground = new SolidColorBrush(style.Color);
            _subTbl.Foreground = new SolidColorBrush(style.Color);
            _supTbl.Foreground = new SolidColorBrush(style.Color);
        }

        public IMongeItem LabeledObject { get; init; }

        public Label(string labelText, string subscript, string superscript, Vector2 position, Style style, IMongeItem labeledObject, ViewportLayer? vpl = null, int fontSize = 16)
        {
            FontSize = fontSize;
            LabelText = labelText;
            Subscript = subscript;
            Superscript = superscript;
            Position = position;
            Style = style;
            _prevStyle = style;
            LabeledObject = labeledObject;
            LabeledObject.Style = Style.HighlightStyle;
            LabeledObject.SetVisibility(Visibility.Hidden);

            if (vpl is not null) AddToViewportLayer(vpl);

            _lblTbl.MouseEnter += LabelMouseEnter;
            _lblTbl.MouseLeave += LabelMouseLeave;
            _lblTbl.MouseLeftButtonDown += LabelMouseLeftButtonDown;
        }

        TextBlock _lblTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };
        TextBlock _supTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };
        TextBlock _subTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };


        public void Draw()
        {
            if (_vpl is null) return;
            double fontSize = FontSize * _vpl.Viewport.Scale;

            _lblTbl.FontSize = fontSize;
            _subTbl.FontSize = fontSize * .5;
            _supTbl.FontSize = fontSize * .5;


            var screenPos = _vpl.Viewport.CanvasToScreen(Position);

            Canvas.SetTop(_lblTbl, screenPos.Y);
            Canvas.SetRight(_lblTbl, _vpl.Canvas.ActualWidth - screenPos.X);
            Canvas.SetTop(_supTbl, screenPos.Y - fontSize * .1);
            Canvas.SetLeft(_supTbl, screenPos.X + fontSize * .05);
            Canvas.SetTop(_subTbl, screenPos.Y + fontSize * .6);
            Canvas.SetLeft(_subTbl, screenPos.X + fontSize * .05);

            LabeledObject.Draw();
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_lblTbl);
            vpl.Canvas.Children.Add(_supTbl);
            vpl.Canvas.Children.Add(_subTbl);
            LabeledObject.AddToViewportLayer(vpl);
            vpl.Canvas.MouseMove += CanvasMouseMove;
            _vpl = vpl;
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_lblTbl);
            _vpl.Canvas.Children.Remove(_supTbl);
            _vpl.Canvas.Children.Remove(_subTbl);
            LabeledObject.RemoveFromViewportLayer();
            _vpl.Canvas.MouseMove -= CanvasMouseMove;
            _vpl = null;
        }

        public void SetVisibility(Visibility visibility)
        {
            _lblTbl.Visibility = visibility;
            _subTbl.Visibility = visibility;
            _supTbl.Visibility = visibility;
        }
        public bool IsVisible() => _lblTbl.Visibility == Visibility.Visible;
        public IMongeItem Clone() => new Label(LabelText, Subscript, Superscript, Position, _prevStyle, LabeledObject.Clone(), fontSize: FontSize);
        public bool IsOnLabel(Vector2 canvasPos)
        {
            if (_vpl is null) return false;

            var t = Canvas.GetTop(_lblTbl);
            var r = _vpl.Canvas.ActualWidth - Canvas.GetRight(_lblTbl);

            var l = r - _lblTbl.ActualWidth;
            var b = t + _lblTbl.ActualHeight;
            var pt = _vpl.Viewport.CanvasToScreen(canvasPos);
            return pt.X >= l && pt.X <= r && pt.Y >= t && pt.Y <= b;
        }

        #region Handling labels

        Style _prevStyle;

        private void LabelMouseEnter(object sender, MouseEventArgs e)
        {
            LabeledObject.SetVisibility(Visibility.Visible);
            SetStyle(Style.HighlightStyle);
        }
        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            LabeledObject.SetVisibility(Visibility.Hidden);
            SetStyle(_prevStyle);
        }

        Vector2? dragStart = null;
        Vector2? dragStartPos = null;
        bool startedMoving = false;
        ViewportLayer? _vpl;
        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //Rename label
                LabelInput lid = new();
                lid.labelTextTbx.Text = LabelText;
                lid.subscriptTbx.Text = Subscript;
                lid.superscriptTbx.Text = Superscript;
                lid.fontSizeTbx.Text = FontSize.ToString();
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    LabelText = lid.LabelText;
                    Subscript = lid.Subscript;
                    Superscript = lid.Superscript;
                    FontSize = lid.TextSize;
                }
                if (_vpl is not null)
                    Draw();
            }

            if (_vpl is null) return;
            dragStart = _vpl.Viewport.ScreenToCanvas(e.GetPosition(_vpl.Canvas));
            dragStartPos = Position;
            startedMoving = false;
        }
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart is null) return;
            if (dragStartPos is null) return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                dragStart = null;
                _vpl?.Canvas.ReleaseMouseCapture();
                return;
            }
            if (_vpl is null) return;

            Position = (Vector2)(dragStartPos + (_vpl.Viewport.ScreenToCanvas(e.GetPosition(sender as Canvas)) - dragStart));

            if (!startedMoving)
            {
                _vpl.Canvas.CaptureMouse();
                startedMoving = true;
            }

            Draw();
        }
        #endregion
    }
}
