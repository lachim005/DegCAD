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
    public partial class PageLayoutWindow : UserControl
    {
        //Last values
        private static double lastW = 210;
        private static double lastH = 297;
        private static int lastPreset = 2;
        private static double lastMarginL = 10;
        private static double lastMarginT = 10;
        private static double lastMarginR = 10;
        private static double lastMarginB = 10;
        private static double lastUnitSize = 10;
        private static double lastOffsetX = 0;
        private static double lastOffsetY = 0;

        ViewPort ViewPort { get; init; }
        Vector2 _originOffset;

        public PageLayoutWindow(ViewPort vp)
        {
            InitializeComponent();

            ViewPort = vp.Clone();
            vpBorder.Child = ViewPort;

            ViewPort.CanZoom = false;
            ViewPort.ViewportChanged += (s, e) => RecalculatePosition();

            LoadLastValues();
            CalculateOriginOffset();
        }

        private void CalculateOriginOffset()
        {
            _originOffset = (0, 0);
            var axis = ViewPort.Timeline.CommandHistory[0].Items;

            //Axonometry given by triangle lengths
            if (axis.Length == 18)
            {
                //Gets the X monge point
                if (axis[8] is not MongeItems.Point pt) return;
                _originOffset = - pt.Coords;
                return;
            }
        }

        private void LoadLastValues()
        {
            paperWidth.Text = lastW.ToString();
            paperHeight.Text = lastH.ToString();
            switch (lastPreset)
            {
                case 1: paperPresetA5.IsChecked = true; break;
                case 2: paperPresetA4.IsChecked = true; break;
                case 3: paperPresetA3.IsChecked = true; break;
                default: paperPresetCustom.IsChecked = true; break;
            }
            marginLeft.Text = lastMarginL.ToString();
            marginTop.Text = lastMarginT.ToString();
            marginRight.Text = lastMarginR.ToString();
            marginBottom.Text = lastMarginB.ToString();
            unitSize.Text = lastUnitSize.ToString();
            ViewPort.OffsetX = lastOffsetX; 
            ViewPort.OffsetY = lastOffsetY;
        }
        private void SaveLastValues()
        {
            _ = double.TryParse(paperWidth.Text, out lastW);
            _ = double.TryParse(paperHeight.Text, out lastH);
            if (paperPresetA5.IsChecked == true) lastPreset = 1;
            else if (paperPresetA4.IsChecked == true) lastPreset = 2;
            else if (paperPresetA3.IsChecked == true) lastPreset = 3;
            else lastPreset = 0;
            _ = double.TryParse(marginLeft.Text, out lastMarginL);
            _ = double.TryParse(marginTop.Text, out lastMarginT);
            _ = double.TryParse(marginRight.Text, out lastMarginR);
            _ = double.TryParse(marginBottom.Text, out lastMarginB);
            _ = double.TryParse(unitSize.Text, out lastUnitSize);
            lastOffsetX = ViewPort.OffsetX; 
            lastOffsetY = ViewPort.OffsetY;
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

            if (!double.TryParse(paperWidth.Text, out var w) || w < .1) return;
            if (!double.TryParse(paperHeight.Text, out var h) || h < .1) return;
            if (!double.TryParse(unitSize.Text, out var us) || us < .1) return;
            double scaleFactor = Math.Min(vpGrid.ActualHeight / h, vpGrid.ActualWidth / w);
            vpBorder.Height = h * scaleFactor;
            vpBorder.Width = w * scaleFactor;
            ViewPort.Scale = us * scaleFactor / ViewPort.unitSize;
            ViewPort.Redraw();
            RecalculatePosition();

            if (!double.TryParse(marginLeft.Text, out var ml) || ml < 0 || ml >= w) return;
            if (!double.TryParse(marginTop.Text, out var mt) || mt < 0 || mt >= h) return;
            if (!double.TryParse(marginRight.Text, out var mr) || mr < 0 || mr >= w) return;
            if (!double.TryParse(marginBottom.Text, out var mb) || mb < 0 || mb >= h) return;

            ml *= scaleFactor;
            mt *= scaleFactor;
            mr *= scaleFactor;
            mb *= scaleFactor;

            marginBorder.Margin = new(ml, mt, mr, mb);
            marginHighlightBorder.BorderThickness = new(ml, mt, mr, mb);
        }

        private void RecalculatePosition()
        {
            if (!double.TryParse(unitSize.Text, out var us) || us < .1) return;

            double px = -(ViewPort.OffsetX + _originOffset.X) * us;
            double py = -(ViewPort.OffsetY + _originOffset.Y) * us;

            posLeftTbl.Text = Math.Round(px).ToString();
            posTopTbl.Text = Math.Round(py).ToString();

            if (!double.TryParse(paperWidth.Text, out var w) || w < .1) return;
            if (!double.TryParse(paperHeight.Text, out var h) || h < .1) return;

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

        private void CenterClick(object sender, RoutedEventArgs e)
        {
            ViewPort.CenterContent();
            RecalculatePosition();
        }

        private void UserControlUnloaded(object sender, RoutedEventArgs e)
        {
            SaveLastValues();
        }
    }
}
