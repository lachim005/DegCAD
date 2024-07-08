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
        private int _selectedLineType;

        public Style CurrentStyle
        {
            get => new()
            {
                Color = CurrentColor,
                LineStyle = SelectedLineType,
                Thickness = SelectedThickness,
            };
        }

        public Color CurrentColor
        { 
            get => _currentColor;
            set
            {
                _currentColor = value;
                currentColorDisplay.Background = new SolidColorBrush(value);
            }
        }

        public int SelectedLineType { 
            get => _selectedLineType;
            set => ChangeSelectedLineType(value);
        }

        public int SelectedThickness
        {
            get
            {
                return (int)thicknessSlider.Value - 1;
            }
            set
            {
                thicknessSlider.Value = Math.Clamp(value + 1, 1, 5);
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
            
            if (sender == lineType2)
            {
                ChangeSelectedLineType(1);
                
            } else if (sender  == lineType3)
            {
                ChangeSelectedLineType(2);
            } else
            {
                ChangeSelectedLineType(0);
            }
        }

        public void ChangeSelectedLineType(int lineType)
        {
            lineType1.IsEnabled = true;
            lineType2.IsEnabled = true;
            lineType3.IsEnabled = true;
            _selectedLineType = lineType;
            switch (lineType)
            {
                case 1:
                    lineType2.IsEnabled = false;
                    break;
                case 2:
                    lineType3.IsEnabled = false;
                    break;
                default:
                    // Asigns _selectedLineType again in case an invalid number is entered
                    _selectedLineType = 0;
                    lineType1.IsEnabled = false;
                    break;
            }
        }

        private void ColorIndicatorClick(object sender, MouseButtonEventArgs e)
        {
            CurrentColor = Dialogs.ColorPicker.EditColor(CurrentColor);
        }
    }
}
