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

        public List<Color> ColorPalette { get; init; } = new()
        {
            Colors.Black,
            Colors.Gray,
            Colors.DarkGray,
            Colors.LimeGreen,
            Colors.DodgerBlue,
            Colors.Red,
            Colors.Gold,
            Colors.Purple
        };

        public StyleSelector()
        {
            InitializeComponent();
            UpdateColorPalette();
            CurrentColor = Colors.DarkGray;
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

        }
    }
}
