using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

        public Vector2 Position { get; set; }
        public Vector2 Size { get; private set; }

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _lblTbl.Foreground = new SolidColorBrush(value.Color);
                _subTbl.Foreground = new SolidColorBrush(value.Color);
                _supTbl.Foreground = new SolidColorBrush(value.Color);
            }
        }

        public IMongeItem LabeledObject { get; init; }

        public Label(string labelText, string subscript, string superscript, Vector2 position, Style style, IMongeItem labeledObject, ViewportLayer? vpl = null)
        {
            LabelText = labelText;
            Subscript = subscript;
            Superscript = superscript;
            Position = position;
            Style = style;
            LabeledObject = labeledObject;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        TextBlock _lblTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };
        TextBlock _supTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };
        TextBlock _subTbl = new() { FontFamily = new("Tahoma"), TextAlignment = TextAlignment.Right };


        public void Draw(ViewportLayer vpl)
        {
            double fontSize = 16 * vpl.Viewport.Scale;

            _lblTbl.FontSize = fontSize;
            _subTbl.FontSize = fontSize * .5;
            _supTbl.FontSize = fontSize * .5;


            var screenPos = vpl.Viewport.CanvasToScreen(Position);

            Canvas.SetTop(_lblTbl, screenPos.Y);
            Canvas.SetRight(_lblTbl, vpl.Canvas.ActualWidth - screenPos.X);
            Canvas.SetTop(_supTbl, screenPos.Y - fontSize * .1);
            Canvas.SetLeft(_supTbl, screenPos.X + fontSize * .05);
            Canvas.SetTop(_subTbl, screenPos.Y + fontSize * .6);
            Canvas.SetLeft(_subTbl, screenPos.X + fontSize * .05);
        }

        public void DrawLabeledObject(ViewportLayer gd, Style style)
        {
            LabeledObject.Draw(gd);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_lblTbl);
            vpl.Canvas.Children.Add(_supTbl);
            vpl.Canvas.Children.Add(_subTbl);
            LabeledObject.AddToViewportLayer(vpl);
        }

        public void SetVisibility(Visibility visibility)
        {
            _lblTbl.Visibility = visibility;
            _subTbl.Visibility = visibility;
            _supTbl.Visibility = visibility;
            LabeledObject.SetVisibility(visibility);
        }
    }
}
