using DegCAD.MultiFile.History;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MFSelectionRect.xaml
    /// </summary>
    public partial class MFSelectionRect : UserControl
    {
        private MFSnapper _snapper;
        private TransformChange trChange = new();
        public MFPage Page { get; init; }

        public MFContainer? Cont { get; set; }

        public MFSelectionRect(MFSnapper snapper, MFPage page)
        {
            InitializeComponent();
            _snapper = snapper;
            Page = page;
        }

        private void Handle1Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCX(cd.X);
            OffsetCY(cd.Y);

            Cont?.InvokeUpdating();
        }
        private void Handle2Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;

            OffsetCY(vc);

            Cont?.InvokeUpdating();
        }
        private void Handle3Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCWidth(cd.X);
            OffsetCY(cd.Y);

            Cont?.InvokeUpdating();
        }
        private void Handle4Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;

            OffsetCX(hc);

            Cont?.InvokeUpdating();
        }
        private void Handle5Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;

            OffsetCWidth(hc);

            Cont?.InvokeUpdating();
        }
        private void Handle6Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCX(cd.X);
            OffsetCHeight(cd.Y);

            Cont?.InvokeUpdating();
        }
        private void Handle7Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;

            OffsetCHeight(vc);

            Cont?.InvokeUpdating();
        }
        private void Handle8Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCWidth(cd.X);
            OffsetCHeight(cd.Y);

            Cont?.InvokeUpdating();
        }
        private void MoveDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Cont is null) return;
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            var x1 = _snapper.TrySnapX(Cont.CX + cd.X);
            var x2 = _snapper.TrySnapX(Cont.CX + cd.X + Cont.CWidth) - Cont.CWidth;
            var y1 = _snapper.TrySnapY(Cont.CY + cd.Y);
            var y2 = _snapper.TrySnapY(Cont.CY + cd.Y + Cont.CHeight) - Cont.CHeight;

            if (x1 is not null && x2 is not null)
            {
                if (Math.Abs(x1.Value - Cont.CX - cd.X) > Math.Abs(x2.Value - Cont.CX - cd.X)) Cont.CX = x2.Value;
                else Cont.CX = x1.Value;
            }
            else if (x1 is not null && x2 is null) Cont.CX = x1.Value;
            else if (x1 is null && x2 is not null) Cont.CX = x2.Value;
            else Cont.CX += cd.X;


            if (y1 is not null && y2 is not null)
            {
                if (Math.Abs(y1.Value - Cont.CY - cd.Y) > Math.Abs(y2.Value - Cont.CY - cd.Y)) Cont.CY = y2.Value;
                else Cont.CY = y1.Value;
            }
            else if (y1 is not null && y2 is null) Cont.CY = y1.Value;
            else if (y1 is null && y2 is not null) Cont.CY = y2.Value;
            else Cont.CY += cd.Y;

            Cont?.InvokeUpdating();
        }

        private void OffsetCX(double offset)
        {
            if (Cont is null) return;

            var snapped = _snapper.SnapX(Cont.CX + offset);
            var diff = snapped - Cont.CX;
            if (Cont.CWidth - diff < MFContainer.minSize)
            {
                diff = Cont.CWidth - 5;
                snapped = diff + Cont.CX;
            }
            Cont.CX = snapped;
            Cont.CWidth -= diff;
        }
        private void OffsetCY(double offset)
        {
            if (Cont is null) return;

            var snapped = _snapper.SnapY(Cont.CY + offset);
            var diff = snapped - Cont.CY;
            if (Cont.CHeight - diff < MFContainer.minSize)
            {
                diff = Cont.CHeight - 5;
                snapped = diff + Cont.CY;
            }
            Cont.CY = snapped;
            Cont.CHeight -= diff;
        }
        private void OffsetCWidth(double offset)
        {
            if (Cont is null) return;
            Cont.CWidth = Math.Max(_snapper.SnapX(Cont.CWidth + offset + Cont.CX) - Cont.CX, MFContainer.minSize);
        }
        private void OffsetCHeight(double offset)
        {
            if (Cont is null) return;
            Cont.CHeight = Math.Max(_snapper.SnapY(Cont.CHeight + offset + Cont.CY) - Cont.CY, MFContainer.minSize);
        }

        private void TransformStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (Cont is null) return;
            trChange = new();
            trChange.SetStartVals(Cont);
            Page.Editor?.Timeline.AddState(new ContainerTransformState(Cont));
        }

        private void TransformCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (Cont is null) return;
            trChange.SetEndVals(Cont);
            Cont.InvokeUpdated(trChange);
        }

    }

    public record TransformChange()
    {
        public void SetStartVals(MFContainer cont)
        {
            startX = cont.CX;
            startY = cont.CY;
            startW = cont.CWidth;
            startH = cont.CHeight;
        }

        public void SetEndVals(MFContainer cont)
        {
            endX = cont.CX;
            endY = cont.CY;
            endW = cont.CWidth;
            endH = cont.CHeight;
        }

        public double startX;
        public double startY;
        public double startW;
        public double startH;
        public double endX;
        public double endY;
        public double endW;
        public double endH;
    }
}
