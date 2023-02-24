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
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        private double _hue;
        private double _saturation;
        private double _brightness;
        private Color _selectedColor;

        public double Hue {
            get => _hue;
            set
            {
                _hue = Math.Clamp(value, 0, 360);
                Canvas.SetTop(hueCursor, (_hue / 360) * hueRectangle.Height);
                UpdateColor();
            } 
        }
        public double Brightness
        {
            get => _brightness;
            set
            {
                _brightness = Math.Clamp(value, 0, 1);
                Canvas.SetTop(SatBrCursor, ((1 - _brightness) * brRectangle.Height) - SatBrCursor.Height / 2);
                UpdateColor();
            }
        }
        public double Saturation
        {
            get => _saturation;
            set
            {
                _saturation = Math.Clamp(value, 0, 1);
                Canvas.SetLeft(SatBrCursor, (_saturation * satRectangle.Width) - SatBrCursor.Width / 2);
                UpdateColor();
            }
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;

            }
        }

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void SatBrMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                brRectangle.MouseMove += MoveSatBrCursor;
                brRectangle.CaptureMouse();
                MoveSatBrCursor(sender, e);
            }
        }
        private void SatBrMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                brRectangle.MouseMove -= MoveSatBrCursor;
                brRectangle.ReleaseMouseCapture();
            }
        }

        private void MoveSatBrCursor(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(satRectangle);
            Saturation = pos.X / satRectangle.Width;
            Brightness = 1 - (pos.Y / satRectangle.Height);

            //Change cursor color so it is always visible
            if (pos.Y > 128)
            {
                SatBrCursor.Stroke = Brushes.White;
            }
            else
            {
                SatBrCursor.Stroke = Brushes.Black;
            }  
        }

        private void HueMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                hueRectangle.MouseMove += MoveHueCursor;
                hueRectangle.CaptureMouse();
                MoveHueCursor(sender, e);
            }
        }
        private void HueMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                hueRectangle.MouseMove -= MoveHueCursor;
                hueRectangle.ReleaseMouseCapture();
            }
        }

        private void MoveHueCursor(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(hueRectangle);
            Hue = (pos.Y / hueRectangle.Height) * 360;
        }

        private void UpdateColor()
        {
            _selectedColor = CreateFromHsv(Hue, Saturation, Brightness);

            selectedColorDisplay.Background = new SolidColorBrush(_selectedColor);
            if (satRectangle.Fill is not LinearGradientBrush lgb) return;
            lgb.GradientStops[0].Color = CreateFromHsv(Hue, 1, 1);
        }
        

        public Color CreateFromHsv(double hue, double sat, double val)
        {
            var c = val * sat;
            var x = c * (1 - Math.Abs(((hue / 60) % 2) - 1));
            var m = val - c;

            (double, double, double) vl = hue switch
            {
                >= 0 and < 60 => (c, x, 0),
                >= 60 and < 120 => (x, c, 0),
                >= 120 and < 180 => (0, c, x),
                >= 180 and < 240 => (0, x, c),
                >= 240 and < 300 => (x, 0, c),
                _ => (c, 0, x)
            };

            return new Color()
            {
                A = 255,
                R = (byte)((vl.Item1 + m) * 255),
                G = (byte)((vl.Item2 + m) * 255),
                B = (byte)((vl.Item3 + m) * 255)
            };
        }


    }
}
