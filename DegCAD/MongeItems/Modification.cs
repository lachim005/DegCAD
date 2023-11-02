using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.MongeItems
{
    public abstract class Modification : IMongeItem
    {
        public Vector2[] SnapablePoints => Array.Empty<Vector2>();

        public ParametricSegment2[] SnapableLines => Array.Empty<ParametricSegment2>();

        public Circle2[] SnapableCircles => Array.Empty<Circle2>();

        public Style Style { get; set; }

        public virtual void AddToViewportLayer(ViewportLayer vpl)
        {
            
        }

        public abstract IMongeItem Clone();
        public string ToSvg() => string.Empty;

        public virtual void Draw() { }

        public virtual void RemoveFromViewportLayer() { }

        public virtual void SetVisibility(Visibility visibility) { }
        public virtual bool IsVisible() => false;
        public abstract void Apply(Timeline tl);
        public abstract void Remove(Timeline tl);
    }
}
