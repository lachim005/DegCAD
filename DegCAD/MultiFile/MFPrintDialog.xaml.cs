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

        PrintQueueCollection queues;
        PrintQueue? selectedQueue;
        PrintCapabilities? capabilities;

        ObservableCollection<PageButtonModel> pages = new();
        ObservableCollection<SheetModel> sheets = new();

        int visibleSheet;


        public MFPrintDialog(MFEditor e)
        {
            InitializeComponent();

            var server = new PrintServer();
            queues = server.GetPrintQueues();
            foreach (var q in queues)
            {
                printersCbx.Items.Add(q.Name);
            }

            for (int i = 0; i < e.Pages.Count; i++)
            {
                pages.Add(new(e.Pages[i].Page, i));
            }

            pagesButtonsIc.ItemsSource = pages;
            printersCbx.SelectedIndex = 0;

            LoadLastValues();
        }

        private void LoadLastValues()
        {
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
        private void PrintClick(object sender, RoutedEventArgs e)
        {
            if (selectedQueue is null) return;
            if (capabilities is null) return;
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
                    item.PagePrintCopy.LockScale = null;
                    item.PagePrintCopy.Redraw();
                }
            }
            else
            {
                foreach (var item in pages)
                {
                    item.PagePrintCopy.LockScale = 1 * 96 / 25.4;
                    item.PagePrintCopy.Redraw();
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
                    ug.Children.Add(p.PagePrintCopy);
                    p.PagePrintCopy._loaded = true;
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
                    p.PagePrintCopy.Redraw();
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

        internal class PageButtonModel
        {
            public MFStaticPage Page { get; init; }
            public MFStaticPage PagePreviewCopy { get; init; }
            public MFStaticPage PagePrintCopy { get; init; }
            public int PageIndex { get; init; }
            public int PageNumber { get; init; }
            public bool ShouldPrint { get; set; } = true;

            public PageButtonModel(MFPage page, int index)
            {

                Page = new(page);
                if (App.Skin == Skin.Dark)
                {
                    Page.SwapWhiteAndBlack();
                }
                PagePreviewCopy = new(Page);
                PagePrintCopy = new(Page);
                PageIndex = index;
                PageNumber = index + 1;
            }
        }

        internal class SheetModel
        {
            public int Rows { get; init; }
            public int Columns { get; init; }

            public ObservableCollection<PageButtonModel> Pages { get; init; } = new();

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
                if (!page.ShouldPrint) continue;
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

        private void PageSelectionChanged(object sender, RoutedEventArgs e)
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

        private void CancelPrintClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StartPageReorderDrag(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement btn) return;
            if (btn.DataContext is not PageButtonModel pm) return;

            e.Handled = true;
            DragDrop.DoDragDrop(btn, pm, DragDropEffects.Move);
        }

        private void PageReorder(object sender, DragEventArgs e)
        {
            //Gets the dragged and dropped tab
            if (e.Data.GetData(typeof(PageButtonModel)) is not PageButtonModel dragPage) return;

            if (sender is not FrameworkElement f) return;
            if (f.DataContext is not PageButtonModel m) return;

            int dragIndex = pages.IndexOf(dragPage);
            int dropIndex = pages.IndexOf(m);
            if (dragIndex == dropIndex) return;
            if (dragIndex == -1 || dropIndex == -1) return;

            e.Handled = true;
            pages.Move(dragIndex, dropIndex);
            UpdatePreview();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            SaveLastValues();
        }
    }
}
