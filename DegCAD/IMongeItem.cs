using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD
{
    public interface IMongeItem
    {
        void Draw();
        public Vector2[] SnapablePoints { get; }
        public ParametricSegment2[] SnapableLines { get; }
        public Circle2[] SnapableCircles { get; }
        public Style Style { get; set; }
        public void AddToViewportLayer(ViewportLayer vpl);
        public void RemoveFromViewportLayer();
        public void SetVisibility(Visibility visibility);
        public bool IsVisible();
        public IMongeItem Clone();
        public string ToSvg();
    }
}
