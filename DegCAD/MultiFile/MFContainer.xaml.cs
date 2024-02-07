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
        public double CWidth { get; set; }
        public double CHeight { get; set; }
        public double CX { get; set; }
        public double CY { get; set; }

        public event EventHandler? Selected;
        public event EventHandler? Updated;
        public MFPage Page { get; set; }

        public MFContainer(MFPage page)
        {
            InitializeComponent();
            Page = page;
        }

        public void Deselect()
        {
            handles.Visibility = Visibility.Collapsed;
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Selected?.Invoke(this, EventArgs.Empty);
                handles.Visibility = Visibility.Visible;
                e.Handled = true;
            }
        }

        private void Handle1Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth - cd.X <= 5 || CHeight - cd.Y <= 5) return;
            CX += cd.X;
            CY += cd.Y;
            CWidth -= cd.X;
            CHeight -= cd.Y;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void Handle2Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;
            if (CHeight - vc <= 5) return;
            CY += vc;
            CHeight -= vc;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void Handle3Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth + cd.X <= 5 || CHeight - cd.Y <= 5) return;
            CY += cd.Y;
            CWidth += cd.X;
            CHeight -= cd.Y;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void Handle4Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;
            if (CWidth - hc <= 5 ) return;
            CX += hc;
            CWidth -= hc;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void Handle5Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;
            if (CWidth + hc <= 5) return;
            CWidth += hc;
            if (Math.Abs(CWidth - 100) < 5) CWidth = 100;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void Handle6Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth - cd.X <= 5 || CHeight + cd.Y <= 5) return;
            CX += cd.X;
            CWidth -= cd.X;
            CHeight += cd.Y;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void Handle7Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;
            if (CHeight + vc <= 5) return;
            CHeight += vc;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void Handle8Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth + cd.X <= 5 || CHeight + cd.Y <= 5) return;
            CWidth += cd.X;
            CHeight += cd.Y;
            Updated?.Invoke(this, EventArgs.Empty);
        }
        private void MoveDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            CX += cd.X;
            CY += cd.Y;
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
