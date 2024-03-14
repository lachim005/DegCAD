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
using static System.Formats.Asn1.AsnWriter;

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFStaticPage.xaml
    /// </summary>
    public partial class MFStaticPage : UserControl
    {
        public bool _loaded = false;
        public double Scale { get; private set; }
        public double unitSize = MFPage.unitSize;
        public double PaperWidth { get; set; } = 210;
        public double PaperHeight { get; set; } = 297;
        public double? LockScale { get; set; } = null;
        public List<MFContainer> Items { get; init; } = new();

        public MFStaticPage(MFPage page)
        {
            InitializeComponent();

            PaperWidth = page.PaperWidth;
            PaperHeight = page.PaperHeight;

            foreach (var item in page.Items)
            {
                var clone = item.Clone();
                clone.BorderVisible = false;
                Items.Add(clone);
                pageItems.Children.Add(clone);
            }

            Redraw();
        }
        public MFStaticPage(MFStaticPage page)
        {
            InitializeComponent();

            PaperWidth = page.PaperWidth;
            PaperHeight = page.PaperHeight;

            foreach (var item in page.Items)
            {
                var clone = item.Clone();
                clone.BorderVisible = false;
                Items.Add(clone);
                pageItems.Children.Add(clone);
            }

            Redraw();
        }

        public void Redraw()
        {
            if (!_loaded) return;

            var scale = LockScale ?? Math.Min(ActualWidth / PaperWidth, ActualHeight / PaperHeight);

            paper.Width = PaperWidth * scale;
            paper.Height = PaperHeight * scale;

            Scale = scale / unitSize;

            foreach (var item in Items)
            {
                item.Width = item.CWidth * Scale * unitSize;
                item.Height = item.CHeight * Scale * unitSize;
                Canvas.SetLeft(item, item.CX * Scale * unitSize);
                Canvas.SetTop(item, item.CY * Scale * unitSize);

                item.Item.ViewUpdated(0, 0, Scale);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            Redraw();
        }

        public void SwapWhiteAndBlack()
        {
            foreach (var item in Items)
            {
                item.Item.SwapWhiteAndBlack();
            }
        }
    }
}
