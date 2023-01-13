using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DegCAD
{
    public class LabelManager
    {
        public Timeline Timeline { get; init; }
        public GeometryDrawer GeometryDrawer { get; init; }
        public ViewPort ViewPort { get; init; }
        public Editor Editor { get; init; }

        public LabelManager(Timeline timeline, GeometryDrawer geometryDrawer, ViewPort viewPort, Editor editor)
        {
            Timeline = timeline;
            GeometryDrawer = geometryDrawer;
            ViewPort = viewPort;
            Editor = editor;

            ViewPort.MouseMove += ViewPortMouseMove;
            ViewPort.MouseDown += ViewPortMouseDown;
            ViewPort.MouseUp += ViewPortMouseUp;
            ViewPort.MouseLeave += ViewPortMouseLeave;
        }

        public MongeItems.Label? HoveredLabel { get; private set; }
        private Vector2 startMoveOffset;
        public bool MovingLabel { get; private set; } = false;
        private Style hoverStyle = new() { Color = Colors.Red };

        private void ViewPortMouseMove(object sender, MouseEventArgs e)
        {
            if (MovingLabel)
            {
                MoveLabel(sender, e);
                return;
            }
            DetectHoveredLabel(e.GetPosition(ViewPort));
        }

        private void DetectHoveredLabel(Vector2 screenPos)
        {
            if (Editor.ExecutingCommand) return;

            var pos = ViewPort.ScreenToCanvas(screenPos);

            MongeItems.Label? hoveredLabel = null;

            //Find a label that the cursor is over
            foreach (var cmd in Timeline.CommandHistory)
            {
                foreach (var item in cmd.Items)
                {
                    if (item is not MongeItems.Label label) continue;
                    var endPoint = label.Position + label.Size;
                    if (label.Position.X < pos.X && pos.X < endPoint.X && label.Position.Y < pos.Y && pos.Y < endPoint.Y)
                    {
                        hoveredLabel = label;
                        goto FoundLabel;
                    }
                }
            }
        FoundLabel:
            //If the label wasn't found
            if (hoveredLabel is null)
            {
                //If there was a previously hovered lable, redraws so it isn't highlighted
                if (HoveredLabel is not null)
                {
                    Editor.Redraw();
                    HoveredLabel = null;
                }
                return;
            }
            //If the hovered label has changed, redraws so the old one isn't highlighted
            if (HoveredLabel != hoveredLabel)
            {
                Editor.Redraw();
                HoveredLabel = hoveredLabel;
            }
            //Highlights the label with the mouse over it
            hoveredLabel.Draw(GeometryDrawer, hoverStyle);
            hoveredLabel.DrawLabeledObject(GeometryDrawer, hoverStyle);
        }
        private void MoveLabel(object sender, MouseEventArgs e)
        {
            //Gets the canvas position of the mouse
            var pos = ViewPort.ScreenToCanvas(e.GetPosition(ViewPort));
            //Move label
            if (HoveredLabel is null) return;
            HoveredLabel.Position = pos + startMoveOffset;
            Editor.Redraw();
            HoveredLabel.Draw(GeometryDrawer, hoverStyle);
            HoveredLabel.DrawLabeledObject(GeometryDrawer, hoverStyle);
            return;
        }
        private void ViewPortMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (HoveredLabel is null) return;

            var canvasPos = ViewPort.ScreenToCanvas(e.GetPosition(ViewPort));
            startMoveOffset = HoveredLabel.Position - canvasPos;
            MovingLabel = true;
        }
        private void ViewPortMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            StopMoving(e);
        }
        private void ViewPortMouseLeave(object sender, MouseEventArgs e)
        {
            StopMoving(e);
        }
        private void StopMoving(MouseEventArgs e)
        {
            if (!MovingLabel) return;
            MovingLabel = false;
            Editor.Redraw();
            DetectHoveredLabel(e.GetPosition(ViewPort));
        }
    }
}
