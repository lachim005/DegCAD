using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
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
        //Last values
        private static bool lastPxScaling = true;
        private static double lastPxW = 500; 
        private static double lastPxH = 700;
        private static double lastPxUnitSize = 30;
        private static double lastMmW = 210; 
        private static double lastMmH = 297; 
        private static double lastMmUnitSize = 10;
        private static double lastDpi = 72;
        private static int lastFormat = 2;
        private static bool lastTransparentBg = false;
        private static double lastOffsetX = 0;
        private static double lastOffsetY = 0;
        private static bool dark = false;

        ViewPort vp;

        public ExportDialog(ViewPort vp, Window owner)
        {
            Owner = owner;
            InitializeComponent();
            this.vp = vp.Clone();
            this.vp.CanZoom = false;
            this.vp.AllowLabelInteractions = false;

            if (App.Skin == Skin.Dark)
            {
                this.vp.SwapWhiteAndBlack();
            }

            if (dark)
            {
                // Inverts dark because checking darkCbx will invert it again
                dark = !dark;
                darkCbx.IsChecked = true;
            }

            vpBorder.Child = this.vp;

            LoadLastValues();
        }

        private void LoadLastValues()
        {
            if (lastPxScaling)
            {
                imagePxSizeRb.IsChecked = true;
            } else
            {
                imageMmSizeRb.IsChecked = true;
            }
            imagePxWidth.Text = lastPxW.ToString();
            imagePxHeight.Text = lastPxH.ToString();
            imagePxUnitSizeTbx.Text = lastPxUnitSize.ToString();
            imageMmWidth.Text = lastMmW.ToString();
            imageMmHeight.Text = lastMmH.ToString();
            imageMmUnitSizeTbx.Text = lastMmUnitSize.ToString();
            imageDpiTbx.Text = lastDpi.ToString();
            imageFormatCbx.SelectedIndex = lastFormat;
            imageTransparentBgChbx.IsChecked = lastTransparentBg;
            vp.OffsetX = lastOffsetX;
            vp.OffsetY = lastOffsetY;
        }
        private void SaveLastValues()
        {
            lastPxScaling = imagePxSizeRb.IsChecked == true;
            _ = double.TryParse(imagePxWidth.Text, out lastPxW);
            _ = double.TryParse(imagePxHeight.Text, out lastPxH);
            _ = double.TryParse(imagePxUnitSizeTbx.Text, out lastPxUnitSize);
            _ = double.TryParse(imageMmWidth.Text, out lastMmW);
            _ = double.TryParse(imageMmHeight.Text, out lastMmH);
            _ = double.TryParse(imageMmUnitSizeTbx.Text, out lastMmUnitSize);
            _ = double.TryParse(imageDpiTbx.Text, out lastDpi);
            lastFormat = imageFormatCbx.SelectedIndex;
            lastTransparentBg = imageTransparentBgChbx.IsChecked == true;
            lastOffsetX = vp.OffsetX;
            lastOffsetY = vp.OffsetY;
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
            imageTransparentBgChbx.Visibility = (imageFormatCbx.SelectedIndex > 1) ? Visibility.Visible : Visibility.Hidden;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ResizeViewport();
        }

        private void ExportImage(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsLoaded) return;

            (string format, bool image) = imageFormatCbx.SelectedIndex switch
            {
                0 => ("jpg", true),
                1 => ("bmp", true),
                3 => ("gif", true),
                4 => ("svg", false),
                _ => ("png", true)
            };

            if (GetSaveLocation($"Obrázek formátu {format}|*.{format}", $"export.{format}") is not string filePath) return;

            var desiredSize = GetDesiredSize();
            if (desiredSize is null) return;

            (double w, double h, double scale) = desiredSize.Value;

            if (image)
            {
                SaveImage(format, w, h, scale, filePath);
            }
            else if (format == "svg")
            {
                SaveSvg(w, h, scale, filePath);
            }

            Close();
        }

        private void SaveImage(string format, double w, double h, double scale, string filePath)
        { 
            var clonedVp = vp.Clone();
            clonedVp.Scale = scale / ViewPort.unitSize;
            clonedVp.Width = w;
            clonedVp.Height = h;
            if (imageTransparentBgChbx.IsChecked == false || imageFormatCbx.SelectedIndex <= 1) clonedVp.Background = dark ? new SolidColorBrush(Color.FromRgb(38, 38, 38)) : Brushes.White;

            Viewbox vb = new();
            vb.Child = clonedVp;
            vb.Measure(new(w, h));
            vb.Arrange(new(0, 0, w, h));
            vb.UpdateLayout();

            RenderTargetBitmap rtb = new((int)w, (int)h, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(clonedVp);

            BitmapEncoder encoder = format switch
            {
                "jpg" => new JpegBitmapEncoder(),
                "bmp" => new BmpBitmapEncoder(),
                "gif" => new GifBitmapEncoder(),
                _ => new PngBitmapEncoder(),
            };

            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using FileStream fs = File.Open(filePath, FileMode.Create);
            encoder.Save(fs);
        }

        private void SaveSvg(double w, double h, double scale, string filePath)
        {
            using StreamWriter sw = new(filePath);
            sw.Write(SvgExporter.ExportSvg(vp, w, h, scale, imageTransparentBgChbx.IsChecked == true, dark));
        }

        private string? GetSaveLocation(string formats, string filename)
        {
            SaveFileDialog sfd = new();
            sfd.Filter = formats;
            sfd.FileName = filename;

            if (sfd.ShowDialog() != true) return null;

            return sfd.FileName;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            SaveLastValues();
        }

        private void darkCbx_Checked(object sender, RoutedEventArgs e)
        {
            vp.SwapWhiteAndBlack();
            dark = !dark;
            vpBorder.Background = dark ? new SolidColorBrush(Color.FromRgb(38, 38, 38)) : Brushes.White;
        }

        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
