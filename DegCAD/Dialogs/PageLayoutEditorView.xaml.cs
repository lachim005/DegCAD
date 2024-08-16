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

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for PageLayoutEditorView.xaml
    /// </summary>
    public partial class PageLayoutEditorView : UserControl, IChangesWithDarkMode
    {
        private static double lastPaperWidth = 210;
        private static double lastPaperHeight = 297;
        private static double lastMarginLeft = 10;
        private static double lastMarginTop = 10;
        private static double lastMarginRight = 10;
        private static double lastMarginBottom = 10;
        private static double lastUnitSize = 10;
        private static double lastOffsetX = 0;
        private static double lastOffsetY = 0;

        private double originX = 0;
        private double originY = 0;

        ViewPort vp;

        public PageLayoutEditorView(ViewPort vp)
        {
            InitializeComponent();
            this.vp = vp.Clone();
            this.vp.CanZoom = false;
            this.vp.AllowLabelInteractions = false;
            vpBorder.Child = this.vp;
            this.vp.ViewportChanged += OnViewportChanged;
            

            paperHeightTbx.Text = lastPaperHeight.ToString();
            paperWidthTbx.Text = lastPaperWidth.ToString();
            marginLeftTbx.Text = lastMarginLeft.ToString();
            marginTopTbx.Text = lastMarginTop.ToString();
            marginRightTbx.Text = lastMarginRight.ToString();
            marginBottomTbx.Text = lastMarginBottom.ToString();
            unitSizeTbx.Text = lastUnitSize.ToString();
            this.vp.OffsetX = lastOffsetX;
            this.vp.OffsetY = lastOffsetY;

            CalculateOriginOffset();
        }

        private void CalculateOriginOffset()
        {
            Vector2 originOffset = (0, 0);
            var axis = vp.Timeline.CommandHistory[0].Items;

            //Axonometry given by triangle lengths
            if (axis.Length == 18)
            {
                //Gets the X monge point
                if (axis[8] is not TimelineElements.Point pt) return;
                originOffset = -pt.Coords;
            }

            originX = originOffset.X;
            originY = originOffset.Y;
        }

        private void OnViewportChanged(object? sender, VPMouseEventArgs e)
        {
            RecalculateDimensions();
        }

        private void SelectPaperPresetClick(object sender, RoutedEventArgs e)
        {
            var preset = SelectPaperSizePresetDialog.GetPaperSizePreset(Window.GetWindow(this));
            if (preset is null) return;
            paperWidthTbx.Text = preset.Width.ToString();
            paperHeightTbx.Text = preset.Height.ToString();
        }

        private void RecalculateView()
        {
            if (!IsLoaded) return;

            // Setting paper size
            if (!double.TryParse(paperWidthTbx.Text, out double w) || w < 1) return;
            if (!double.TryParse(paperHeightTbx.Text, out double h) || h < 1) return;

            aspectRatioBox.AspectWidth = w;
            aspectRatioBox.AspectHeight = h;

            // Setting scale according to unit size
            if (!double.TryParse(unitSizeTbx.Text, out var unitSize) || unitSize < .1) return;

            var scale = aspectRatioBox.Scale;

            vp.Scale = scale * unitSize / ViewPort.unitSize;
            vp.Redraw();

            // Setting margins
            if (!double.TryParse(marginLeftTbx.Text, out var marginLeft) || marginLeft < 0) return;
            if (!double.TryParse(marginTopTbx.Text, out var marginTop) || marginTop < 0) return;
            if (!double.TryParse(marginRightTbx.Text, out var marginRight) || marginRight < 0) return;
            if (!double.TryParse(marginBottomTbx.Text, out var marginBottom) || marginBottom < 0) return;

            Thickness margin = new(marginLeft * scale, marginTop * scale, marginRight * scale, marginBottom * scale);
            marginBorder.Margin = margin;
            marginFillBorder.BorderThickness = margin;

            RecalculateDimensions();
        }

        private void RecalculateDimensions()
        {
            if (!IsLoaded) return;

            if (!double.TryParse(unitSizeTbx.Text, out var unitSize) || unitSize < .1) return;

            var xMili = -(vp.OffsetX + originX) * unitSize;
            var yMili = -(vp.OffsetY + originY) * unitSize;
            var xPixel = Math.Round(xMili * aspectRatioBox.Scale + 2);
            var yPixel = Math.Round(yMili * aspectRatioBox.Scale + 1);

            xDistLabel.Text = Math.Round(xMili).ToString();
            yDistLabel.Text = Math.Round(yMili).ToString();

            if (xPixel <= aspectRatioBox.ActualWidth)
            {
                xDistGrid.Width = Math.Max(xPixel, 2);
                xDistArrow.Visibility = Visibility.Collapsed;
                xDistDash.Visibility = Visibility.Visible;
            } else
            {
                xDistGrid.Width = aspectRatioBox.ActualWidth;
                xDistArrow.Visibility = Visibility.Visible;
                xDistDash.Visibility = Visibility.Collapsed;
            }

            if (yPixel <= aspectRatioBox.ActualHeight)
            {
                yDistGrid.Height = Math.Max(yPixel, 2);
                yDistArrow.Visibility = Visibility.Collapsed;
                yDistDash.Visibility = Visibility.Visible;
            }
            else
            {
                yDistGrid.Height = aspectRatioBox.ActualHeight;
                yDistArrow.Visibility = Visibility.Visible;
                yDistDash.Visibility = Visibility.Collapsed;
            }

        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateView();
        }

        private void OnAspectRatioBoxSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateView();
        }

        public void SwapWhiteAndBlack()
        {
            vp.SwapWhiteAndBlack();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RecalculateView();
        }

        private void CenterContentClick(object sender, RoutedEventArgs e)
        {
            vp.CenterContent();
            RecalculateDimensions();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _ = double.TryParse(paperWidthTbx.Text, out lastPaperWidth);
            _ = double.TryParse(paperHeightTbx.Text, out lastPaperHeight);
            _ = double.TryParse(marginLeftTbx.Text, out lastMarginLeft);
            _ = double.TryParse(marginTopTbx.Text, out lastMarginTop);
            _ = double.TryParse(marginRightTbx.Text, out lastMarginRight);
            _ = double.TryParse(marginBottomTbx.Text, out lastMarginBottom);
            _ = double.TryParse(unitSizeTbx.Text, out lastUnitSize);
            lastOffsetX = vp.OffsetX;
            lastOffsetY = vp.OffsetY;
        }
    }
}
