using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DegCAD
{
    public abstract class GeometryElement : ITimelineElement
    {
        private Style _style;
        private Visibility _visibility;
        private readonly List<Shape> _shapes = [];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                ShowStyle(value);
            }
        }

        public Shape[] Shapes { get => _shapes.ToArray(); }
        public ViewportLayer? ViewportLayer { get; protected set; }
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                SetVisibility(value);
            }
        }
        public bool IsVisible => _visibility == Visibility.Visible;

        public ITimelineElement Clone() => CloneElement();
        public abstract GeometryElement CloneElement();
        public abstract void Draw();

        public virtual void ShowStyle(Style style)
        {
            foreach (var shape in Shapes)
            {
                shape.SetStyle(style);
            }
        }

        public virtual void AddToViewportLayer(ViewportLayer vpl)
        {
            foreach (var shape in Shapes)
            {
                vpl.Canvas.Children.Add(shape);
            }
            ViewportLayer = vpl;
        }
        public virtual void RemoveFromViewportLayer()
        {
            if (ViewportLayer is null) return;
            foreach (var shape in Shapes)
            {
                ViewportLayer.Canvas.Children.Remove(shape);
            }
            ViewportLayer = null;
        }

        protected virtual void SetVisibility(Visibility visibility)
        {
            foreach (var shape in Shapes)
            {
                shape.Visibility = visibility;
            }
        }

        protected void AddShape(Shape shape)
        {
            shape.IsHitTestVisible = false;
            _shapes.Add(shape);
            ShowStyle(_style);
        }
        protected void RemoveShape(Shape shape)
        {
            _shapes.Remove(shape);
        }
    }
}
