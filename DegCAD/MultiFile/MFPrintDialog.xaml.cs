using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFPrintDialog.xaml
    /// </summary>
    public partial class MFPrintDialog : Window
    {
        //Last values
        private static bool lastValuesSet = false;
        private static int lastPrinter = 0;
        private static int lastPaperSize = 0;
        private static bool lastLandscape = false;
        private static int lastPagesPerSheet = 0;
        private static int lastScale = 0;
        private static int lastDuplexing = 0;
        private static string lastCopyCount = "1";
        private static int lastCollation = 1;
        private static string lastPagesToPrint = "";

        PrintQueueCollection queues;
        PrintQueue? selectedQueue;
        PrintCapabilities? capabilities;

        ObservableCollection<PageModel> pages = new();
        ObservableCollection<SheetModel> sheets = new();

        MFEditor editor;

        int visibleSheet;


        public MFPrintDialog(MFEditor e, Window owner)
        {
            Owner = owner;
            InitializeComponent();

            var server = new PrintServer();
            var defaultPrinter = LocalPrintServer.GetDefaultPrintQueue();
            queues = server.GetPrintQueues();

            int index = 0;
            int defaultIndex = 0;
            foreach (var q in queues)
            {
                printersCbx.Items.Add(q.Name);
                if (q.Name == defaultPrinter.Name) defaultIndex = index;
                index++;
            }

            printersCbx.SelectedIndex = defaultIndex;
            editor = e;

            LoadLastValues();
        }

        private void LoadLastValues()
        {
            pagesToPrintTbx.Text = lastPagesToPrint;
            UpdatePagesToPrint();

            if (!lastValuesSet) return;
            printersCbx.SelectedIndex = lastPrinter;
            paperCbx.SelectedIndex = lastPaperSize;
            landscapeChbx.IsChecked = lastLandscape;
            pagesPerSheetCbx.SelectedIndex = lastPagesPerSheet;
            scaleCbx.SelectedIndex = lastScale;
            duplexingCbx.SelectedIndex = lastDuplexing;
            copyCountTbx.Text = lastCopyCount;
            collationCbx.SelectedIndex = lastCollation;
        }

        private void SaveLastValues()
        {
            lastPrinter = printersCbx.SelectedIndex;
            lastPaperSize = paperCbx.SelectedIndex;
            lastLandscape = landscapeChbx.IsChecked == true;
            lastPagesPerSheet = pagesPerSheetCbx.SelectedIndex;
            lastScale = scaleCbx.SelectedIndex;
            lastDuplexing = duplexingCbx.SelectedIndex;
            lastCopyCount = copyCountTbx.Text;
            lastCollation = collationCbx.SelectedIndex;
            lastValuesSet = true;
            lastPagesToPrint = pagesToPrintTbx.Text;
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

            // Sets collation cbx
            collateOptionYes.Visibility = Visibility.Collapsed;
            collateOptionNo.Visibility = Visibility.Collapsed;
            collateOptionUnknown.Visibility = Visibility.Collapsed;
            if (capabilities.CollationCapability.Count == 0)
            {
                collationCbx.Visibility = Visibility.Collapsed;
                collationLbl.Visibility = Visibility.Collapsed;
            }
            else
            {
                collationCbx.Visibility = Visibility.Visible;
                collationLbl.Visibility = Visibility.Visible;
                collationCbx.SelectedIndex = 2;
                foreach (var cc in capabilities.CollationCapability)
                {
                    switch (cc)
                    {
                        case Collation.Collated:
                            collateOptionYes.Visibility = Visibility.Visible;
                            collationCbx.SelectedIndex = 0;
                            break;
                        case Collation.Uncollated:
                            collateOptionNo.Visibility = Visibility.Visible;
                            if (collationCbx.SelectedIndex != 0)
                            collationCbx.SelectedIndex = 1;
                            break;
                    }
                }
            }

            // Sets duplexing cbx
            duplexingOptionOne.Visibility = Visibility.Collapsed;
            duplexingOptionTwoLong.Visibility = Visibility.Collapsed;
            duplexingOptionTwoShort.Visibility = Visibility.Collapsed;
            duplexingOptionUnknown.Visibility = Visibility.Collapsed;
            if (capabilities.DuplexingCapability.Count == 0)
            {
                duplexingCbx.Visibility = Visibility.Collapsed;
                duplexingLbl.Visibility = Visibility.Collapsed;
            }
            else
            {
                duplexingCbx.Visibility = Visibility.Visible;
                duplexingLbl.Visibility = Visibility.Visible;
                duplexingCbx.SelectedIndex = 3;
                foreach (var dc in capabilities.DuplexingCapability)
                {
                    switch (dc)
                    {
                        case Duplexing.OneSided:
                            duplexingOptionOne.Visibility = Visibility.Visible;
                            duplexingCbx.SelectedIndex = 0;
                            break;
                        case Duplexing.TwoSidedLongEdge:
                            duplexingOptionTwoLong.Visibility = Visibility.Visible;
                            break;
                        case Duplexing.TwoSidedShortEdge:
                            duplexingOptionTwoShort.Visibility = Visibility.Visible;
                            break;
                    }
                }
            }
        }
        private void Print(object sender, ExecutedRoutedEventArgs e)
        {
            // Focuses on the print button in case the user was editing pages to print, because these only update when the textbox loses focus
            printBtn.Focus();

            if (selectedQueue is null) return;
            if (capabilities is null) return;
            if (GetPaperSize() is not PageMediaSize size) return;

            if (!int.TryParse(copyCountTbx.Text, out int copyCount) || copyCount < 1)
            {
                MessageBox.Show(this, "Neplatný počet kopií", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PrintTicket pt = selectedQueue.UserPrintTicket;
            pt.PageMediaSize = size;

            double w = pt.PageMediaSize.Width.GetValueOrDefault(0);
            double h = pt.PageMediaSize.Height.GetValueOrDefault(0);


            pt.PageMediaSize = size;
            pt.CopyCount = copyCount;
            if (GetCollation() is Collation coll) pt.Collation = coll;
            if (GetDuplexing() is Duplexing dupl) pt.Duplexing = dupl;
            var writer = PrintQueue.CreateXpsDocumentWriter(selectedQueue);

            //Get color
            OutputColor? color = OutputColor.Monochrome;
            foreach (var c in capabilities.OutputColorCapability)
            {
                if (c < color)
                {
                    color = c;
                }
            }
            pt.OutputColor = color;


            if (scaleCbx.SelectedIndex == 0)
            {
                foreach (var item in pages)
                {
                    item.Page.LockScale = null;
                    item.Page.Redraw();
                }
            }
            else
            {
                foreach (var item in pages)
                {
                    item.Page.LockScale = 1 * 96 / 25.4;
                    item.Page.Redraw();
                }
            }

            FixedDocument doc = new();
            


            foreach (var s in sheets)
            {
                FixedPage page = new()
                {
                    Width = w,
                    Height = h
                };

                UniformGrid ug = new()
                {
                    Rows = s.Rows,
                    Columns = s.Columns,
                    Width = w,
                    Height = h
                };

                if (landscapeChbx.IsChecked == true)
                {
                    ug.Width = h;
                    ug.Height = w;

                    TransformGroup tg = new();
                    tg.Children.Add(new RotateTransform(90));
                    tg.Children.Add(new TranslateTransform(w, 0));
                    ug.RenderTransform = tg;
                }

                foreach (var p in s.Pages)
                {
                    ug.Children.Add(p.Page);
                    p.Page._loaded = true;
                }

                Viewbox vb = new()
                {
                    Child = ug
                };
                page.Children.Add(vb);
                vb.Measure(new(w, h));
                vb.Arrange(new(0, 0,w, h));
                vb.UpdateLayout();


                foreach (var p in s.Pages)
                {
                    p.Page.Redraw();
                }

                PageContent pc = new();
                ((IAddChild)pc).AddChild(page);
                doc.Pages.Add(pc);
            }

            writer.Write(doc.DocumentPaginator, pt);

            Close();
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

        private Collation? GetCollation()
        {
            if (collationCbx.Visibility == Visibility.Collapsed) return null;
            else return collationCbx.SelectedIndex switch
            {
                0 => Collation.Collated,
                1 => Collation.Uncollated,
                _ => null
            };
        }
        private Duplexing? GetDuplexing()
        {
            if (duplexingCbx.Visibility == Visibility.Collapsed) return null;
            else return duplexingCbx.SelectedIndex switch
            {
                0 => Duplexing.OneSided,
                1 => Duplexing.TwoSidedLongEdge,
                2 => Duplexing.TwoSidedShortEdge,
                _ => null
            };
        }

        internal class PageModel
        {
            public MFStaticPage Page { get; init; }
            public MFStaticPage PagePreviewCopy { get; init; }

            public PageModel(MFPage page)
            {

                Page = new(page);
                if (App.Skin == Skin.Dark)
                {
                    Page.SwapWhiteAndBlack();
                }
                PagePreviewCopy = new(Page);
            }
        }

        internal class SheetModel
        {
            public int Rows { get; init; }
            public int Columns { get; init; }

            public ObservableCollection<PageModel> Pages { get; init; } = new();

            public SheetModel(int rows, int columns)
            {
                Rows = rows;
                Columns = columns;
            }
        }

        private void UpdatePreview()
        {
            if (GetPaperSize() is not PageMediaSize size) return;

            var w = size.Width.GetValueOrDefault(100);
            var h = size.Height.GetValueOrDefault(100);

            if (landscapeChbx.IsChecked == true)
            {
                (w, h) = (h, w);
            }

            paperBox.AspectWidth = w;
            paperBox.AspectHeight = h;

            int cols = 1, rows = 1;
            switch (pagesPerSheetCbx.SelectedIndex)
            {
                case 1:
                    if (landscapeChbx.IsChecked == true) cols = 2;
                    else rows = 2;
                    break;
                case 2:
                    rows = 2;
                    cols = 2;
                    break;
                case 3:
                    rows = 3;
                    cols = 3;
                    break;
            }

            sheets.Clear();

            SheetModel sm = new(rows, cols);

            foreach(var page in pages)
            {
                if (sm.Pages.Count == rows * cols)
                {
                    sheets.Add(sm);
                    sm = new(rows, cols);
                }

                sm.Pages.Add(page);
            }
            sheets.Add(sm);
            sheetCountTbl.Text = sheets.Count.ToString();
            ShowSheet(0);

            if (scaleCbx.SelectedIndex == 0)
            {
                foreach (var item in pages)
                {
                    item.PagePreviewCopy.LockScale = null;
                    item.PagePreviewCopy.Redraw();
                }
            } else
            {
                foreach (var item in pages)
                {
                    item.PagePreviewCopy.LockScale = paperBox.Scale * 96 / 25.4;
                    item.PagePreviewCopy.Redraw();
                }
            }
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void CheckboxChanged(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void PaperBoxSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (scaleCbx.SelectedIndex == 0) return;

            foreach (var item in pages)
            {
                item.PagePreviewCopy.LockScale = paperBox.Scale * 96 / 25.4;
                item.PagePreviewCopy.Redraw();
            }
        }

        private void ShowSheet(int sheetIndex)
        {
            visibleSheet = sheetIndex;
            sheetIc.DataContext = sheets[sheetIndex];

            nextSheetBtn.IsEnabled = !(visibleSheet == 0);
            previousSheetBtn.IsEnabled = !(visibleSheet + 1 >= sheets.Count);

            currentSheetTbl.Text = (sheetIndex + 1).ToString();
        }

        private void PreviousSheet(object sender, RoutedEventArgs e)
        {
            ShowSheet(visibleSheet - 1);
        }

        private void NextSheet(object sender, RoutedEventArgs e)
        {
            ShowSheet(visibleSheet + 1);
        }

        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            SaveLastValues();
        }

        private void PagesSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdatePagesToPrint();
        }

        private void UpdatePagesToPrint()
        {
            List<int> pages = [];
            bool valid = true;
            var vals = pagesToPrintTbx.Text.Split(',');
            int pageCount = editor.Pages.Count;
            // Parse the pages string
            foreach (var v in vals)
            {
                if (v.Contains('-'))
                {
                    // Range
                    var range = v.Split('-', 2);
                    if (!int.TryParse(range[0], out int rangeStart)
                        || rangeStart > pageCount
                        || rangeStart < 1)
                    {
                        valid = false;
                        break;
                    }
                    if (!int.TryParse(range[1], out int rangeEnd)
                        || rangeEnd > pageCount
                        || rangeEnd < 1
                        || rangeEnd < rangeStart)
                    {
                        valid = false;
                        break;
                    }

                    while (rangeStart <= rangeEnd)
                    {
                        pages.Add(rangeStart++);
                    }
                }
                else
                {
                    // Single page
                    if (!int.TryParse(v, out int page)
                        || page > pageCount
                        || page < 1)
                    {
                        valid = false;
                        break;
                    }
                    pages.Add(page);
                }
            }

            this.pages.Clear();

            if (!valid)
            {
                // Uses default page order
                foreach (var page in editor.Pages)
                {
                    this.pages.Add(new(page.Page));
                }
                UpdatePreview();
                return;
            }

            foreach (var page in pages)
            {
                this.pages.Add(new(editor.Pages[page - 1].Page));
            }
            UpdatePreview();
        }
    }
}
