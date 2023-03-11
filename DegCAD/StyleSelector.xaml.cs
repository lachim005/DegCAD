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
                LineStyle = GetSelectedLineType()
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

        public List<Color> ColorPalette { get; init; } = new();

        public StyleSelector()
        {
            InitializeComponent();
            UpdateColorPalette();
            CurrentColor = Colors.Black;
        }

        public void AddDefaultColors()
        {
            ColorPalette.Add(Color.FromRgb(0, 0, 0));
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

        public int GetSelectedLineType()
        {
            //Dash
            if (lineType2.IsChecked == true) return 1;
            //Dotdash
            if (lineType3.IsChecked == true) return 2;
            //Solid
            return 0;
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
    }
}
