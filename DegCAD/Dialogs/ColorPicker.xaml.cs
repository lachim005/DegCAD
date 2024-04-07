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
        private static bool lastTabRgb = false;

        public bool Saved { get; private set; } = false;
        public Color SelectedColor
        {
            get
            {
                if (pickerTabs.SelectedIndex == 0) return HSVSelectedColor;
                else return RGBSelectedColor;
            }
        }

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
                UpdateHsvTextInputs();
            }
        }
        #endregion

        #region RGB fields
        private byte _red;
        private byte _green;
        private byte _blue;
        private Color _rgbSelectedColor;

        public byte Red
        {
            get => _red;
            set
            {
                _red = value;
                Canvas.SetLeft(rCursor, value);
                UpdateRgbColor();
            }
        }
        public byte Green
        {
            get => _green;
            set
            {
                _green = value;
                Canvas.SetLeft(gCursor, value);
                UpdateRgbColor();
            }
        }
        public byte Blue
        {
            get => _blue;
            set
            {
                _blue = value;
                Canvas.SetLeft(bCursor, value);
                UpdateRgbColor();
            }
        }
        public Color RGBSelectedColor
        {
            get => _rgbSelectedColor;
            set
            {
                _rgbSelectedColor = value;
                Red = value.R;
                Green = value.G;
                Blue = value.B;
                UpdateRgbTextInputs();
                UpdateHexTextInput();
                UpdateRgbColor();
            }
        }
        #endregion

        public ColorPicker() : this(Colors.Red) { }

        public ColorPicker(Color color)
        {
            InitializeComponent();
            if (lastTabRgb)
            {
                pickerTabs.SelectedIndex = 1;   
                Loaded += (s, e) => RGBSelectedColor = color;
            } else
            {
                Loaded += (s, e) => HSVSelectedColor = color;
            }
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

            hsvSelectedColorDisplay.Background = new SolidColorBrush(_hsvSelectedColor);
            if (satRectangle.Fill is not LinearGradientBrush lgb) return;
            lgb.GradientStops[0].Color = CreateFromHsv(Hue, 100, 100);
        }
        private void UpdateHsvTextInputs()
        {
            int h = Hue, s = Saturation, b = Brightness;
            hTbx.Text = h.ToString();
            sTbx.Text = s.ToString();
            vTbx.Text = b.ToString();
        }
        private void HSVTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(hTbx.Text, out var hue)) Hue = hue;
            if (int.TryParse(sTbx.Text, out var sat)) Saturation = sat;
            if (int.TryParse(vTbx.Text, out var val)) Brightness = val;
        }
        #endregion

        #region RGB handlers
        private void RMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                rRectangle.MouseMove += MoveRCursor;
                rRectangle.CaptureMouse();
                MoveRCursor(sender, e);
            }
        }
        private void RMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                rRectangle.MouseMove -= MoveRCursor;
                rRectangle.ReleaseMouseCapture();
            }
        }

        private void MoveRCursor(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(rRectangle);
            Red = (byte)Math.Clamp(pos.X, 0, 255);
            UpdateRgbTextInputs();
            UpdateHexTextInput();
        }

        private void GMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                gRectangle.MouseMove += MoveGCursor;
                gRectangle.CaptureMouse();
                MoveGCursor(sender, e);
            }
        }
        private void GMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                gRectangle.MouseMove -= MoveGCursor;
                gRectangle.ReleaseMouseCapture();
            }
        }

        private void MoveGCursor(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(gRectangle);
            Green = (byte)Math.Clamp(pos.X, 0, 255);
            UpdateRgbTextInputs();
            UpdateHexTextInput();
        }

        private void BMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                bRectangle.MouseMove += MoveBCursor;
                bRectangle.CaptureMouse();
                MoveBCursor(sender, e);
            }
        }
        private void BMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                bRectangle.MouseMove -= MoveBCursor;
                bRectangle.ReleaseMouseCapture();
            }
        }

        private void MoveBCursor(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(bRectangle);
            Blue = (byte)Math.Clamp(pos.X, 0, 255);
            UpdateRgbTextInputs();
            UpdateHexTextInput();
        }

        private void UpdateRgbColor()
        {
            _rgbSelectedColor = Color.FromRgb(_red, _green, _blue);
            rgbSelectedColorDisplay.Background = new SolidColorBrush(_rgbSelectedColor);

            if (rRectangle.Fill is LinearGradientBrush rlgb)
            {
                rlgb.GradientStops[0].Color = Color.FromRgb(0, _green, _blue);
                rlgb.GradientStops[1].Color = Color.FromRgb(255, _green, _blue);
            }
            if (gRectangle.Fill is LinearGradientBrush glgb)
            {
                glgb.GradientStops[0].Color = Color.FromRgb(_red, 0, _blue);
                glgb.GradientStops[1].Color = Color.FromRgb(_red, 255, _blue);
            }
            if (bRectangle.Fill is LinearGradientBrush blgb)
            {
                blgb.GradientStops[0].Color = Color.FromRgb(_red, _green, 0);
                blgb.GradientStops[1].Color = Color.FromRgb(_red, _green, 255);
            }
        }

        private void UpdateRgbTextInputs()
        {
            byte r = Red, g = Green, b = Blue;
            rTbx.Text = r.ToString();
            gTbx.Text = g.ToString();
            bTbx.Text = b.ToString();
        }
        private void RGBTextChanged(object sender, TextChangedEventArgs e)
        {
            if (byte.TryParse(rTbx.Text, out var red)) Red = red;
            if (byte.TryParse(gTbx.Text, out var green)) Green = green;
            if (byte.TryParse(bTbx.Text, out var blue)) Blue = blue;
        }
        private void UpdateHexTextInput()
        {
            if (hexTbx.Text.Length > 0)
                hexTbx.Text = (hexTbx.Text[0] == '#') ? "#" : "";
            else
                hexTbx.Text = "#";
            hexTbx.Text += Red.ToString("x2") + Green.ToString("x2") + Blue.ToString("x2");
        }
        private void HexTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = hexTbx.Text.Trim('#');
            if (txt.Length == 6)
            {
                string rs = txt[..2];
                string gs = txt[2..4];
                string bs = txt[4..6];

                try
                {
                    var r = Convert.ToByte(rs, 16);
                    var g = Convert.ToByte(gs, 16);
                    var b = Convert.ToByte(bs, 16);
                    Red = r;
                    Green = g;
                    Blue = b;
                    UpdateRgbTextInputs();
                }
                catch
                {
                    return;
                }

            }
        }
        #endregion

        public static Color CreateFromHsv(double hue, double sat, double val)
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
        public static (int, int, int) ConvertToHsv(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);

            var delta = max - min;
            int hue = 0;
            if (max == r) hue = (int)(60 * (((g - b) / delta) % 6));
            else if (max == g) hue = (int)(60 * (((b - r) / delta) + 2));
            else if (max == b) hue = (int)(60 * (((r - g) / delta) + 4));

            int sat = 0;
            if (hue < 0) hue += 360;

            if (max != 0) sat = (int)(delta / max * 100);

            return (hue, sat, (int)(max * 100));
        }

        private void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pickerTabs.SelectedIndex == 0)
            {
                //HSV tab selected
                HSVSelectedColor = RGBSelectedColor;
            } else if (pickerTabs.SelectedIndex == 1)
            {
                //RGB tab selected
                RGBSelectedColor = HSVSelectedColor;
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            Saved = true;
            Close();
        }

        /// <summary>
        /// Opens the color picker dialog and lets the user change the given color
        /// </summary>
        /// <returns>The new color or the same color in case the user cancels the dialog</returns>
        public static Color EditColor(Color color)
        {
            ColorPicker picker = new ColorPicker(color);
            picker.ShowDialog();
            if (!picker.Saved) return color;
            return picker.SelectedColor;
        }

        /// <summary>
        /// Opens the color picker dialog and lets the user choose a color
        /// </summary>
        /// <returns>The selected color or null if the user cancels the dialog</returns>
        public static Color? GetColor()
        {
            ColorPicker picker = new ColorPicker();
            picker.ShowDialog();
            if (!picker.Saved) return null;
            return picker.SelectedColor;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lastTabRgb = pickerTabs.SelectedIndex == 1;
        }
    }
}
