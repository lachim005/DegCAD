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
    /// Interaction logic for PageLayoutWindow.xaml
    /// </summary>
    public partial class PageLayoutWindow : Window
    {
        Timeline Timeline { get; init; }
        ViewPort ViewPort { get; init; }

        public PageLayoutWindow(Timeline tl)
        {
            InitializeComponent();
            Timeline = tl.Clone();

            ViewPort = new(Timeline);
            Timeline.SetViewportLayer(ViewPort.Layers[1]);
            vpBorder.Child = ViewPort;

            ViewPort.CanZoom = false;
        }

        private void PaperSizeChanged(object sender, TextChangedEventArgs e) => RecalculateSize();
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e) => RecalculateSize();
        private void WindowLoaded(object sender, RoutedEventArgs e) => RecalculateSize();
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            if (paperPresetCustom.IsChecked == true) 
            {
                paperWidth.IsEnabled = true;
                paperHeight.IsEnabled = true;
                return;
            };
            paperWidth.IsEnabled = false;
            paperHeight.IsEnabled = false;
            if (paperPresetA3.IsChecked == true)
            {
                paperWidth.Text = "297";
                paperHeight.Text = "420";
            }
            if (paperPresetA4.IsChecked == true)
            {
                paperWidth.Text = "210";
                paperHeight.Text = "297";
            }
            if (paperPresetA5.IsChecked == true)
            {
                paperWidth.Text = "148";
                paperHeight.Text = "210";
            }
            RecalculateSize();
        }

        private void RecalculateSize()
        {
            if (!IsLoaded) return;

            if (!double.TryParse(paperWidth.Text, out var w)) return;
            if (!double.TryParse(paperHeight.Text, out var h)) return;
            double scaleFactor = Math.Min(vpGrid.ActualHeight / h, vpGrid.ActualWidth / w);
            vpBorder.Height = h * scaleFactor;
            vpBorder.Width = w * scaleFactor;
            ViewPort.Scale = 10 * scaleFactor / ViewPort.unitSize;

            if (!double.TryParse(marginLeft.Text, out var ml)) return;
            if (!double.TryParse(marginTop.Text, out var mt)) return;
            if (!double.TryParse(marginRight.Text, out var mr)) return;
            if (!double.TryParse(marginBottom.Text, out var mb)) return;

            ml *= scaleFactor;
            mt *= scaleFactor;
            mr *= scaleFactor;
            mb *= scaleFactor;

            marginBorder.Margin = new(ml, mt, mr, mb);
            marginHighlightBorder.BorderThickness = new(ml, mt, mr, mb);
        }
    }
}
