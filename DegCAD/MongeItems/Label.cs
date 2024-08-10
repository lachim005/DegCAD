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
    public class Label : GeometryElement, ISvgConvertable
    {
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

        public override void ShowStyle(Style style)
        {
            _lblTbl.Foreground = new SolidColorBrush(style.Color);
            _subTbl.Foreground = new SolidColorBrush(style.Color);
            _supTbl.Foreground = new SolidColorBrush(style.Color);
        }

        public GeometryElement LabeledObject { get; init; }

        public Label(string labelText, string subscript, string superscript, Vector2 position, Style style, GeometryElement labeledObject, ViewportLayer? vpl = null, int? fontSize = null)
        {
            FontSize = fontSize ?? Settings.DefaultLabelFontSize;
            LabelText = labelText;
            Subscript = subscript;
            Superscript = superscript;
            Position = position;
            Style = style;
            _prevStyle = style;
            LabeledObject = labeledObject;
            LabeledObject.Style = Style.HighlightStyle;
            LabeledObject.Visibility = Visibility.Hidden;

            if (vpl is not null) AddToViewportLayer(vpl);

            _lblTbl.MouseEnter += LabelMouseEnter;
            _lblTbl.MouseLeave += LabelMouseLeave;
            _lblTbl.MouseLeftButtonDown += LabelMouseLeftButtonDown;
        }

        TextBlock _lblTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };
        TextBlock _supTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };
        TextBlock _subTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };


        public override void Draw()
        {
            if (ViewportLayer is null) return;
            double fontSize = Math.Clamp(FontSize * ViewportLayer.Viewport.Scale, 0.01, 35000);

            _lblTbl.FontSize = fontSize;
            _subTbl.FontSize = fontSize * .5;
            _supTbl.FontSize = fontSize * .5;


            var screenPos = ViewportLayer.Viewport.CanvasToScreen(Position);

            Canvas.SetTop(_lblTbl, screenPos.Y);
            Canvas.SetRight(_lblTbl, ViewportLayer.Canvas.ActualWidth - screenPos.X);
            Canvas.SetTop(_supTbl, screenPos.Y - fontSize * .1);
            Canvas.SetLeft(_supTbl, screenPos.X + fontSize * .05);
            Canvas.SetTop(_subTbl, screenPos.Y + fontSize * .6);
            Canvas.SetLeft(_subTbl, screenPos.X + fontSize * .05);

            LabeledObject.Draw();
        }

        public override void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_lblTbl);
            vpl.Canvas.Children.Add(_supTbl);
            vpl.Canvas.Children.Add(_subTbl);
            LabeledObject.AddToViewportLayer(vpl);
            vpl.Canvas.MouseMove += CanvasMouseMove;
            ViewportLayer = vpl;
        }
        public override void RemoveFromViewportLayer()
        {
            if (ViewportLayer is null) return;
            ViewportLayer.Canvas.Children.Remove(_lblTbl);
            ViewportLayer.Canvas.Children.Remove(_supTbl);
            ViewportLayer.Canvas.Children.Remove(_subTbl);
            LabeledObject.RemoveFromViewportLayer();
            ViewportLayer.Canvas.MouseMove -= CanvasMouseMove;
            ViewportLayer = null;
        }

        protected override void SetVisibility(Visibility visibility)
        {
            _lblTbl.Visibility = visibility;
            _subTbl.Visibility = visibility;
            _supTbl.Visibility = visibility;
        }
        public override GeometryElement CloneElement() => new Label(LabelText, Subscript, Superscript, Position, Style, LabeledObject.CloneElement(), fontSize: FontSize);
        public string ToSvg()
        {
            if (ViewportLayer is null) return string.Empty;

            var t = Canvas.GetTop(_lblTbl);
            var r = ViewportLayer.Canvas.ActualWidth - Canvas.GetRight(_lblTbl);

            var text = $"<text x=\"{r}\" " +
                $"y=\"{t}\" " +
                $"fill=\"{Style.ToHex(Style.Color)}\" " +
                $"font-size=\"{_lblTbl.FontSize}\" " +
                $"dominant-baseline=\"hanging\" " +
                $"text-anchor=\"end\" " +
                $"font-family=\"Tahoma\">{LabelText}</text>\n";

            var subL = Canvas.GetLeft(_subTbl);
            var subT = Canvas.GetTop(_subTbl);

            text += $"<text x=\"{subL}\" " +
                $"y=\"{subT}\" " +
                $"fill=\"{Style.ToHex(Style.Color)}\" " +
                $"font-size=\"{_subTbl.FontSize}\" " +
                $"dominant-baseline=\"hanging\" " +
                $"font-family=\"Tahoma\">{Subscript}</text>\n";

            var supL = Canvas.GetLeft(_supTbl);
            var supT = Canvas.GetTop(_supTbl);

            text += $"<text x=\"{supL}\" " +
                $"y=\"{supT}\" " +
                $"fill=\"{Style.ToHex(Style.Color)}\" " +
                $"font-size=\"{_supTbl.FontSize}\" " +
                $"dominant-baseline=\"hanging\" " +
                $"font-family=\"Tahoma\">{Superscript}</text>\n";

            return text;

        }

        public bool IsOnLabel(Vector2 canvasPos)
        {
            if (ViewportLayer is null) return false;

            var t = Canvas.GetTop(_lblTbl);
            var r = ViewportLayer.Canvas.ActualWidth - Canvas.GetRight(_lblTbl);

            var l = r - _lblTbl.ActualWidth;
            var b = t + _lblTbl.ActualHeight;
            var pt = ViewportLayer.Viewport.CanvasToScreen(canvasPos);
            return pt.X >= l && pt.X <= r && pt.Y >= t && pt.Y <= b;
        }

        #region Handling labels

        Style _prevStyle;

        private void LabelMouseEnter(object sender, MouseEventArgs e)
        {
            LabeledObject.Visibility = Visibility.Visible;
            _prevStyle = Style;
            Style = Style.HighlightStyle;
        }
        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            LabeledObject.Visibility = Visibility.Hidden;
            Style = _prevStyle;
        }

        Vector2? dragStart = null;
        Vector2? dragStartPos = null;
        bool startedMoving = false;
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
                if (ViewportLayer is not null)
                    Draw();
            }

            if (ViewportLayer is null) return;
            dragStart = ViewportLayer.Viewport.ScreenToCanvas(e.GetPosition(ViewportLayer.Canvas));
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
                ViewportLayer?.Canvas.ReleaseMouseCapture();
                return;
            }
            if (ViewportLayer is null) return;

            Position = (Vector2)(dragStartPos + (ViewportLayer.Viewport.ScreenToCanvas(e.GetPosition(sender as Canvas)) - dragStart));

            //Snap to other labels
            if (Settings.SnapLabels)
            {
                var closestYDiff = double.MaxValue;
                foreach (var cmd in ViewportLayer.Viewport.Timeline.CommandHistory)
                {
                    foreach (var item in cmd.Items)
                    {
                        if (item is not Label lbl) continue;
                        if (ReferenceEquals(lbl, this)) continue;
                        var diff = Position - lbl.Position;
                        if (Math.Abs(diff.X - .3 / 16 * FontSize) < .4 / 16 * FontSize && Math.Abs(diff.Y) < .1 / 16 * FontSize)
                        {
                            if (closestYDiff > diff.Y)
                            {
                                closestYDiff = diff.Y;
                            }
                        }
                    }
                }
                if (closestYDiff != double.MaxValue)
                {
                    Position = (Position.X, Position.Y - closestYDiff);
                }
            }

            if (!startedMoving)
            {
                ViewportLayer.Canvas.CaptureMouse();
                startedMoving = true;
            }

            Draw();
        }
        #endregion
    }
}
