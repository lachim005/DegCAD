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
        ViewPort ViewPort { get; init; }

        public PageLayoutWindow(ViewPort vp)
        {
            InitializeComponent();

            ViewPort = vp.Clone();
            vpBorder.Child = ViewPort;

            ViewPort.CanZoom = false;
            ViewPort.ViewportChanged += (s, e) => RecalculatePosition();
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
            if (!double.TryParse(unitSize.Text, out var us)) return;
            double scaleFactor = Math.Min(vpGrid.ActualHeight / h, vpGrid.ActualWidth / w);
            vpBorder.Height = h * scaleFactor;
            vpBorder.Width = w * scaleFactor;
            ViewPort.Scale = us * scaleFactor / ViewPort.unitSize;
            ViewPort.Redraw();
            RecalculatePosition();

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

        private void RecalculatePosition()
        {
            if (!double.TryParse(unitSize.Text, out var us)) return;

            double px = -ViewPort.OffsetX * us;
            double py = -ViewPort.OffsetY * us;

            posLeftTbl.Text = Math.Round(px).ToString();
            posTopTbl.Text = Math.Round(py).ToString();

            if (!double.TryParse(paperWidth.Text, out var w)) return;
            if (!double.TryParse(paperHeight.Text, out var h)) return;

            double scaleFactor = Math.Min(vpGrid.ActualHeight / h, vpGrid.ActualWidth / w);

            double pgx = Math.Max(px * scaleFactor - 1, 0);
            double pgy = Math.Max(py * scaleFactor -1, 0);

            if (pgx > ViewPort.ActualWidth - 4)
            {
                pgx = ViewPort.ActualWidth - 4;
                leftPosEnd.Visibility = Visibility.Hidden;
                leftPosArrow.Visibility = Visibility.Visible;
            } else
            {
                leftPosEnd.Visibility = Visibility.Visible;
                leftPosArrow.Visibility = Visibility.Hidden;
            }

            if (pgy > ViewPort.ActualHeight - 4)
            {
                pgy = ViewPort.ActualHeight - 4;
                topPosEnd.Visibility = Visibility.Hidden;
                topPosArrow.Visibility = Visibility.Visible;
            }
            else
            {
                topPosEnd.Visibility = Visibility.Visible;
                topPosArrow.Visibility = Visibility.Hidden;
            }

            posLeftGrid.Width = pgx;
            posTopGrid.Height = pgy;
        }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
