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
using System.Drawing.Printing;
using System.Printing;
using System.Security.Policy;

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for PrintDialog.xaml
    /// </summary>
    public partial class PrintDialog : Window
    {
        PrintQueueCollection queues;
        PrintQueue? selectedQueue;
        PrintCapabilities? capabilities;
        Editor editor;

        ViewPort vp;

        public PrintDialog(Editor e)
        {
            InitializeComponent();

            var server = new PrintServer();
            queues = server.GetPrintQueues();
            foreach (var q in queues)
            {
                printersCbx.Items.Add(q.Name);
            }

            vp = e.viewPort.Clone();
            vp.CanZoom = false;

            vpSv.Child = vp;

            editor = e;
            printersCbx.SelectedIndex = 0;
        }

        private void PrinterChanged(object sender, SelectionChangedEventArgs e)
        {
            paperCbx.Items.Clear();
            selectedQueue = null;

            if (printersCbx.SelectedIndex == -1) return;

            PrintQueue? pq = null;
            foreach (var q in queues)
            {
                if (q.Name == printersCbx.SelectedItem.ToString())
                    pq = q;
            }

            if (pq is null) return;

            selectedQueue = pq;
            capabilities = pq.GetPrintCapabilities();

            var hasA4 = false;
            foreach (var pap in capabilities.PageMediaSizeCapability)
            {
                paperCbx.Items.Add(pap.PageMediaSizeName);
                if (pap.PageMediaSizeName == PageMediaSizeName.ISOA4)
                {
                    hasA4 = true;
                }
            }
            if (hasA4)
                paperCbx.SelectedItem = PageMediaSizeName.ISOA4;
            else
                paperCbx.SelectedIndex = 0;
        }

        private void PrintClick(object sender, RoutedEventArgs e)
        {
            if (selectedQueue is null) return;
            if (capabilities is null) return;
            if (!double.TryParse(unitSizeTbx.Text, out double unitSize) || unitSize <= .1) return;
            if (GetPaperSize() is not PageMediaSize size) return;

            if (!int.TryParse(copyCountTbx.Text, out int copyCount) || copyCount < 1)
            {
                MessageBox.Show("Neplatný počet kopií", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PrintTicket pt = selectedQueue.UserPrintTicket;
            pt.PageMediaSize = size;

            double w = pt.PageMediaSize.Width.GetValueOrDefault(0);
            double h = pt.PageMediaSize.Height.GetValueOrDefault(0);

            var clonedVp = vp.Clone();
            clonedVp.Scale = unitSize * 96d / 25.4 / ViewPort.unitSize;
            clonedVp.Width = w;
            clonedVp.Height = h;
            
            Viewbox vb = new();
            vb.Child = clonedVp;
            vb.Measure(new(w,h));
            vb.Arrange(new(0,0,w,h));
            vb.UpdateLayout();



            selectedQueue.UserPrintTicket.PageMediaSize = size;
            selectedQueue.UserPrintTicket.CopyCount = copyCount;

            //Get color
            OutputColor? color = OutputColor.Monochrome;
            foreach (var c in capabilities.OutputColorCapability)
            {
                if (c < color)
                {
                    color = c;
                }
            }
            selectedQueue.UserPrintTicket.OutputColor = color;

            var writer = PrintQueue.CreateXpsDocumentWriter(selectedQueue);
            writer.Write(clonedVp, selectedQueue.UserPrintTicket);
            Close();
        }

        private void PaperSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            ResizePaperPreview();
        }
        private void VpGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePaperPreview();
        }

        private void ResizePaperPreview()
        {
            if (!IsLoaded) return;
            if (selectedQueue is null) return;
            if (!double.TryParse(unitSizeTbx.Text, out double unitSize) || unitSize <= .1) return;
            if (GetPaperSize() is not PageMediaSize size) return;

            PrintTicket pt = selectedQueue.UserPrintTicket;
            pt.PageMediaSize = size;

            double w = pt.PageMediaSize.Width.GetValueOrDefault(0);
            double h = pt.PageMediaSize.Height.GetValueOrDefault(0);

            double scaleFactor = Math.Min(vpGrid.ActualHeight / h, vpGrid.ActualWidth / w);

            vp.Width = w * scaleFactor;
            vp.Height = h * scaleFactor;

            vp.Scale = unitSize * 96d * scaleFactor / 25.4 / ViewPort.unitSize;
            vp.Redraw();
        }


        private PageMediaSize? GetPaperSize()
        {
            if (paperCbx.SelectedItem is not PageMediaSizeName sizeName) return null;
            if (capabilities is null) return null;

            foreach (var pap in capabilities.PageMediaSizeCapability)
            {
                if (pap.PageMediaSizeName == sizeName)
                    return pap;
            }

            return null;
        }

        private void UnitSizeTbxTextChanged(object sender, TextChangedEventArgs e)
        {      
            ResizePaperPreview();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ResizePaperPreview();
        }

        private void CenterClick(object sender, RoutedEventArgs e)
        {
            vp.CenterContent();
        }
    }
}
