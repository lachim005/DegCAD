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

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for StyleSelector.xaml
    /// </summary>
    public partial class StyleSelector : UserControl
    {
        private Color _currentColor;

        public Style CurrentStyle
        {
            get => new()
            {
                Color = CurrentColor,
                LineStyle = SelectedLineType,
                Thickness = SelectedThickness,
            };
        }

        Color CurrentColor
        { 
            get => _currentColor;
            set
            {
                _currentColor = value;
                currentColorDisplay.Background = new SolidColorBrush(value);
            }
        }

        public int SelectedLineType { get; private set; }
        public int SelectedThickness
        {
            get
            {
                return (int)thicknessSlider.Value - 1;
            }
        }

        public List<Color> ColorPalette { get; init; } = new();

        public StyleSelector()
        {
            InitializeComponent();
            UpdateColorPalette();
            if (App.Skin == Skin.Light) CurrentColor = Colors.Black;
            else CurrentColor = Colors.White;
        }

        public void AddDefaultColors()
        {
            ColorPalette.Add(App.Skin == Skin.Light ? Colors.Black : Colors.White);
            ColorPalette.Add(Color.FromRgb(153, 153, 153));
            ColorPalette.Add(Color.FromRgb(255, 0, 0));
            ColorPalette.Add(Color.FromRgb(255, 128, 0));
            ColorPalette.Add(Color.FromRgb(242, 203, 12));
            ColorPalette.Add(Color.FromRgb(67, 204, 0));
            ColorPalette.Add(Color.FromRgb(40, 204, 204));
            ColorPalette.Add(Color.FromRgb(0, 169, 255));
            ColorPalette.Add(Color.FromRgb(0, 0, 255));
            ColorPalette.Add(Color.FromRgb(134, 31, 186));
            ColorPalette.Add(Color.FromRgb(229, 68, 229));
            UpdateColorPalette();
        }

        public void UpdateColorPalette()
        {
            colorPaletteWp.Children.Clear();
            foreach (var col in ColorPalette)
            {
                Rectangle btn = new();
                btn.Margin = new(0, 0, 1, 1);
                btn.Fill = new SolidColorBrush(col);
                btn.Width = 15; btn.Height = 15;
                btn.Cursor = Cursors.Hand;
                btn.MouseLeftButtonDown += PaletteBtnClick;
                btn.Tag = col;
                colorPaletteWp.Children.Add(btn);
            }
        }

        public void SwapWhiteAndBlack()
        {
            for (int i = 0; i < ColorPalette.Count; i++)
            {
                if (ColorPalette[i] == Colors.Black)
                {
                    ColorPalette[i] = Colors.White;
                } else if (ColorPalette[i] == Colors.White)
                {
                    ColorPalette[i] = Colors.Black;
                }
            }
            if (CurrentColor == Colors.Black)
            {
                CurrentColor = Colors.White;
            } else if (CurrentColor == Colors.White)
            {
                CurrentColor = Colors.Black;
            }
            UpdateColorPalette();
        }

        private void PaletteBtnClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Rectangle rect) return;
            if (rect.Tag is not Color color) return;
            CurrentColor = color;
        }

        private void EditColorsBtnClick(object sender, RoutedEventArgs e)
        {
            Dialogs.EditColorsDialog.EditColors(ColorPalette);
            UpdateColorPalette();
        }

        private void LineTypeBtnClick(object sender, RoutedEventArgs e)
        {
            lineType1.IsEnabled = true;
            lineType2.IsEnabled = true;
            lineType3.IsEnabled = true;
            if (sender == lineType2)
            {
                SelectedLineType = 1;
                lineType2.IsEnabled = false;
            } else if (sender  == lineType3)
            {
                SelectedLineType = 2;
                lineType3.IsEnabled = false;
            } else
            {
                SelectedLineType = 0;
                lineType1.IsEnabled = false;
            }
        }

        private void ColorIndicatorClick(object sender, MouseButtonEventArgs e)
        {
            CurrentColor = Dialogs.ColorPicker.EditColor(CurrentColor);
        }
    }
}
