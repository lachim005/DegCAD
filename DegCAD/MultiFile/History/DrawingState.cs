using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class DrawingState : IMFHistoryState
    {
        MFDrawing drawing;

        public bool positionLocked;
        public int visibleItems;
        public double unitSize;
        public double offsetX;
        public double offsetY;

        public DrawingState(MFDrawing drawing)
        {
            this.drawing = drawing;
            positionLocked = drawing.PositionLocked;
            visibleItems = drawing.VisibleItems;
            unitSize = drawing.UnitSize;

            offsetX = drawing.Viewport.OffsetX;
            offsetY = drawing.Viewport.OffsetY;
        }

        public void ApplyState()
        {
            drawing.PositionLocked = positionLocked;
            drawing.VisibleItems = visibleItems;
            drawing.UnitSize = unitSize;

            drawing.Viewport.OffsetX = offsetX;
            drawing.Viewport.OffsetY = offsetY;

            drawing.Container?.Select();
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new DrawingState(drawing);
        }
    }
}
