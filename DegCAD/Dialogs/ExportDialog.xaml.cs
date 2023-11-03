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

        private void ExportImageClick(object sender, RoutedEventArgs e)
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
            if (imageTransparentBgChbx.IsChecked == false || imageFormatCbx.SelectedIndex <= 1) clonedVp.Background = Brushes.White;

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
            var clonedVp = vp.Clone();
            clonedVp.Scale = scale / ViewPort.unitSize;
            clonedVp.Width = w;
            clonedVp.Height = h;
            if (imageTransparentBgChbx.IsChecked == false || imageFormatCbx.SelectedIndex <= 1) clonedVp.Background = Brushes.White;

            Viewbox vb = new();
            vb.Child = clonedVp;
            vb.Measure(new(w, h));
            vb.Arrange(new(0, 0, w, h));
            vb.UpdateLayout();

            //Changes the culture so doubles will be written with dots
            var prevCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            using StreamWriter sw = new(filePath);
            sw.WriteLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"{w}\" height=\"{h}\" fill=\"none\">");
            if (imageTransparentBgChbx.IsChecked == false)
            {
                sw.WriteLine($"\t<rect width=\"{w}\" height=\"{h}\" fill=\"white\"/>");
            }

            foreach(var cmd in clonedVp.Timeline.CommandHistory)
            {
                foreach (var item in cmd.Items)
                {
                    if (!item.IsVisible()) continue;
                    sw.WriteLine("\t" + item.ToSvg());
                }
            }
            sw.WriteLine("</svg>");


            CultureInfo.CurrentCulture = prevCulture;
        }

        private string? GetSaveLocation(string formats, string filename)
        {
            SaveFileDialog sfd = new();
            sfd.Filter = formats;
            sfd.FileName = filename;

            if (sfd.ShowDialog() != true) return null;

            return sfd.FileName;
        }
    }
}
