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
        #region HSV fields
        private int _hue;
        private int _saturation;
        private int _brightness;
        private Color _hsvSelectedColor;

        public int Hue
        {
            get => _hue;
            set
            {
                _hue = Math.Clamp(value, 0, 360);
                Canvas.SetTop(hueCursor, (_hue / 360.0) * hueRectangle.Height);
                UpdateHSVColor();
            }
        }
        public int Brightness
        {
            get => _brightness;
            set
            {
                _brightness = Math.Clamp(value, 0, 100);
                Canvas.SetTop(SatBrCursor, ((1 - _brightness / 100.0) * brRectangle.Height) - SatBrCursor.Height / 2);
                UpdateHSVColor();
            }
        }
        public int Saturation
        {
            get => _saturation;
            set
            {
                _saturation = Math.Clamp(value, 0, 100);
                Canvas.SetLeft(SatBrCursor, (_saturation / 100.0 * satRectangle.Width) - SatBrCursor.Width / 2);
                UpdateHSVColor();
            }
        }

        public Color HSVSelectedColor
        {
            get => _hsvSelectedColor;
            set
            {
                _hsvSelectedColor = value;
                (Hue, Saturation, Brightness) = ConvertToHsv(value);
                UpdateHSVColor();
            }
        } 
        #endregion

        public ColorPicker()
        {
            InitializeComponent();
        }

        #region HSV handlers
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
            Saturation = (int)(pos.X / satRectangle.Width * 100);
            Brightness = 100 - (int)(pos.Y / satRectangle.Height * 100);

            //Change cursor color so it is always visible
            if (pos.Y > 128)
            {
                SatBrCursor.Stroke = Brushes.White;
            }
            else
            {
                SatBrCursor.Stroke = Brushes.Black;
            }

            UpdateHsvTextInputs();
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
            Hue = (int)(pos.Y / hueRectangle.Height * 360);
            UpdateHsvTextInputs();
        }

        private void UpdateHSVColor()
        {
            _hsvSelectedColor = CreateFromHsv(Hue, Saturation, Brightness);

            selectedColorDisplay.Background = new SolidColorBrush(_hsvSelectedColor);
            if (satRectangle.Fill is not LinearGradientBrush lgb) return;
            lgb.GradientStops[0].Color = CreateFromHsv(Hue, 100, 100);
        }
        private void UpdateHsvTextInputs()
        {
            hTbx.Text = Hue.ToString();
            sTbx.Text = Saturation.ToString();
            vTbx.Text = Brightness.ToString();
        }
        private void HSVTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(hTbx.Text, out var hue)) Hue = hue;
            if (int.TryParse(sTbx.Text, out var sat)) Saturation = sat;
            if (int.TryParse(vTbx.Text, out var val)) Brightness = val;
        } 
        #endregion

        public Color CreateFromHsv(double hue, double sat, double val)
        {
            var c = val * sat / 10000;
            var x = c * (1 - Math.Abs((hue / 60.0 % 2) - 1));
            var m = val / 100 - c;

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
        public (int, int, int) ConvertToHsv(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);

            var delta = max - min;
            int hue = 0;
            if (max == r) hue = (int)(60 * ((g - b / delta) % 6));
            else if (max == g) hue = (int)(60 * (b - r / delta + 2));
            else if (max == b) hue = (int)(60 * (r - g / delta + 4));

            int sat = 0;
            if (max != 0) sat = (int)(delta / max * 100);

            return (hue, sat, (int)(max * 100));
        }

    }
}
