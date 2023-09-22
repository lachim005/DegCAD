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

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for PrintDialog.xaml
    /// </summary>
    public partial class PrintDialog : Window
    {
        PrintQueueCollection queues;
        PrintQueue? selectedQueue;
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

            vpSv.Child = vp;

            editor = e;
        }

        private void PrinterChanged(object sender, SelectionChangedEventArgs e)
        {
            colorCbx.Items.Clear();
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
            var c = pq.GetPrintCapabilities();

            foreach (var col in c.OutputColorCapability)
            {
                colorCbx.Items.Add(col);
            }
            foreach (var pap in c.PageMediaSizeCapability)
            {
                paperCbx.Items.Add(pap);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedQueue is null) return;

            var size = paperCbx.SelectedItem as PageMediaSize;
            if (size is null) return;
            var color = colorCbx.SelectedItem as OutputColor?;
            if (color is null) return;


            selectedQueue.UserPrintTicket.PageMediaSize = size;
            selectedQueue.UserPrintTicket.OutputColor = color;
            //selectedQueue.UserPrintTicket.PageOrientation = PageOrientation.Landscape;

            var writer = PrintQueue.CreateXpsDocumentWriter(selectedQueue);
            writer.Write(vp, selectedQueue.UserPrintTicket);
        }

        private void PaperSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedQueue is null) return;

            var size = paperCbx.SelectedItem as PageMediaSize;
            if (size is null) return;
            selectedQueue.UserPrintTicket.PageMediaSize = size;


            PrintTicket pt = selectedQueue.UserPrintTicket;
            Double printableWidth = pt.PageMediaSize.Width.Value;
            Double printableHeight = pt.PageMediaSize.Height.Value;


            vp.Width = printableWidth;
            vp.Height = printableHeight;

        }
    }
}
