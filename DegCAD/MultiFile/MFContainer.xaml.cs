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
    /// Interaction logic for MFContainer.xaml
    /// </summary>
    public partial class MFContainer : UserControl
    {
        public const double minSize = 5;

        private bool _borderVisible = true;
        private MFSnapper _snapper;

        public double CWidth { get; set; } = 100;
        public double CHeight { get; set; } = 100;
        public double CX { get; set; }
        public double CY { get; set; }

        public event EventHandler? Selected;
        public event EventHandler? Deselected;
        public event EventHandler? Updating;
        public event EventHandler<TransformChange>? Updated;
        public MFPage Page { get; set; }
        public MFItem Item { get; init; }
        public bool IsSelected { get; private set; }

        private TransformChange trChange = new();

        public bool BorderVisible
        {
            get => _borderVisible;
            set
            {
                _borderVisible = value;
                containerBorder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public MFContainer(MFPage page, MFItem item)
        {
            InitializeComponent();
            Page = page;
            Item = item;
            item.Container = this;
            contentBorder.Child = item;
            _snapper = page.Snapper;
        }

        public void Deselect()
        {
            handles.Visibility = Visibility.Collapsed;
            IsSelected = false;
            Deselected?.Invoke(this, EventArgs.Empty);
        }
        public void Select()
        {
            IsSelected = true;
            Selected?.Invoke(this, EventArgs.Empty);
            handles.Visibility = Visibility.Visible;
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Select();
                e.Handled = true;
            }
        }

        private void Handle1Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCX(cd.X);
            OffsetCY(cd.Y);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle2Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;

            OffsetCY(vc);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle3Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCWidth(cd.X);
            OffsetCY(cd.Y);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle4Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;

            OffsetCX(hc);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle5Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;

            OffsetCWidth(hc);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle6Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCX(cd.X);
            OffsetCHeight(cd.Y);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle7Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;

            OffsetCHeight(vc);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle8Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            OffsetCWidth(cd.X);
            OffsetCHeight(cd.Y);

            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void MoveDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;

            var x1 = _snapper.TrySnapX(CX + cd.X);
            var x2 = _snapper.TrySnapX(CX + cd.X + CWidth) - CWidth;
            var y1 = _snapper.TrySnapY(CY + cd.Y);
            var y2 = _snapper.TrySnapY(CY + cd.Y + CHeight) - CHeight;

            if (x1 is not null && x2 is not null)
            {
                if (Math.Abs(x1.Value - CX - cd.X) > Math.Abs(x2.Value - CX - cd.X)) CX = x2.Value;
                else CX = x1.Value;
            }
            else if (x1 is not null && x2 is null) CX = x1.Value;
            else if (x1 is null && x2 is not null) CX = x2.Value;
            else CX += cd.X;


            if (y1 is not null && y2 is not null)
            {
                if (Math.Abs(y1.Value - CY - cd.Y) > Math.Abs(y2.Value - CY - cd.Y)) CY = y2.Value;
                else CY = y1.Value;
            }
            else if (y1 is not null && y2 is null) CY = y1.Value;
            else if (y1 is null && y2 is not null) CY = y2.Value;
            else CY += cd.Y;

            Updating?.Invoke(this, EventArgs.Empty);
        }

        private void OffsetCX(double offset)
        {
            var snapped = _snapper.SnapX(CX + offset);
            var diff = snapped - CX;
            if (CWidth - diff < minSize)
            {
                diff = CWidth - 5;
                snapped = diff + CX;
            }
            CX = snapped;
            CWidth -= diff;
        }
        private void OffsetCY(double offset)
        {
            var snapped = _snapper.SnapY(CY + offset);
            var diff = snapped - CY;
            if (CHeight - diff < minSize)
            {
                diff = CHeight - 5;
                snapped = diff + CY;
            }
            CY = snapped;
            CHeight -= diff;
        }
        private void OffsetCWidth(double offset)
        {
            CWidth = Math.Max(_snapper.SnapX(CWidth + offset + CX) - CX, minSize);
        }
        private void OffsetCHeight(double offset)
        {
            CHeight = Math.Max(_snapper.SnapY(CHeight + offset + CY) - CY, minSize);
        }

        private void TransformStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            trChange = new();
            trChange.SetStartVals(this);
            Page.Editor?.Timeline.AddState(new ContainerTransformState(this));
        }

        private void TransformCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            trChange.SetEndVals(this);
            Updated?.Invoke(this, trChange);
        }

        public MFContainer Clone()
        {
            return new(Page, Item.Clone()) { CX = CX, CY = CY, CWidth = CWidth, CHeight = CHeight};
        }
        public MFContainer Clone(MFPage page)
        {
            return new(page, Item.Clone()) { CX = CX, CY = CY, CWidth = CWidth, CHeight = CHeight };
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
