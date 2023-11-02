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
using System.Windows.Shapes;

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window
    {
        ViewPort vp;

        public ExportDialog(ViewPort vp)
        {
            InitializeComponent();
            this.vp = vp.Clone();
            this.vp.CanZoom = false;
            vpBorder.Child = this.vp;
        }

        private void VpGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeViewport();
        }

        private (double rawW, double rawH, double rawS)? GetDesiredSize()
        {
            double rawW = 100;
            double rawH = 100;
            double rawS = 1;

            if (fileTypeTabs.SelectedIndex == 0)
            {
                //Dealing with image
                if (imagePxSizeRb.IsChecked == true)
                {
                    //Pixel sizing
                    if (!double.TryParse(imagePxWidth.Text, out rawW) || rawW < .1) return null;
                    if (!double.TryParse(imagePxHeight.Text, out rawH) || rawH < .1) return null;
                    if (!double.TryParse(imagePxUnitSizeTbx.Text, out rawS) || rawS < .1) return null;
                }
                else
                {
                    //Milimeter sizing
                    if (!double.TryParse(imageDpiTbx.Text, out double dpi) || dpi < 1) return null;
                    if (!double.TryParse(imageMmWidth.Text, out double mmW) || mmW < .1) return null;
                    if (!double.TryParse(imageMmHeight.Text, out double mmH) || mmH < .1) return null;
                    if (!double.TryParse(imageMmUnitSizeTbx.Text, out double uSize) || uSize < .1) return null;
                    rawW = mmW / 25.4 * dpi;
                    rawH = mmH / 25.4 * dpi;
                    rawS = uSize / 25.4 * dpi;
                }
            }

            return (rawW, rawH, rawS);
        }
        private void ResizeViewport()
        {
            if (!IsLoaded) return;

            var desiredSize = GetDesiredSize();
            if (desiredSize is null) return;

            (double rawW, double rawH, double rawS) = desiredSize.Value;

            double scaleFactor = Math.Min(vpGrid.ActualHeight / rawH, vpGrid.ActualWidth / rawW);

            vp.Width = rawW * scaleFactor;
            vp.Height = rawH * scaleFactor;

            vp.Scale = rawS * scaleFactor / ViewPort.unitSize;
            vp.Redraw();
        }

        private void ImageTextChanged(object sender, TextChangedEventArgs e)
        {
            ResizeViewport();
        }

        private void ImageSizingTypeChanged(object sender, RoutedEventArgs e)
        {
            ResizeViewport();
        }

        private void CenterViewport(object sender, RoutedEventArgs e)
        {
            vp.CenterContent();
        }

        private void ImageFormatChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            imageTransparentBgChbx.Visibility = (imageFormatCbx.SelectedIndex > 0) ? Visibility.Visible : Visibility.Hidden;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ResizeViewport();
        }
    }
}
