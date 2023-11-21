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
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public bool Canceled { get; private set; } = true;
        public string StringResult { get; private set; } = string.Empty;
        public int IntResult { get; private set; } = 0;
        public double DoubleResult { get; private set; } = 0;
        Type _type;

        private InputBox(Type type)
        {
            InitializeComponent();
            _type = type;
            textbox.Focus();
        }

        private static InputBox SpawnDialog(Type t, string unit, string title)
        {
            InputBox ib = new(t);
            ib.unitTbl.Text = unit;
            ib.Title = title;
            ib.ShowDialog();
            return ib;
        }
        public static double? InputDouble(string unit = "", string title = "Zadejte hodnotu")
        {
            var ib = SpawnDialog(Type.Double, unit, title);
            if (ib.Canceled) return null;
            return ib.DoubleResult;
        }
        public static string? InputString(string unit = "", string title = "Zadejte hodnotu")
        {
            var ib = SpawnDialog(Type.String, unit, title);
            if (ib.Canceled) return null;
            return ib.StringResult;
        }
        public static int? InputInt(string unit = "", string title = "Zadejte hodnotu")
        {
            var ib = SpawnDialog(Type.Int, unit, title);
            if (ib.Canceled) return null;
            return ib.IntResult;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            Submit();
        }

        private void Submit()
        {
            switch (_type)
            {
                case Type.String:
                    StringResult = textbox.Text;
                    break;
                case Type.Int:
                    if (!int.TryParse(textbox.Text, out var i))
                    {
                        MessageBox.Show("Zadána neplatná hodnota", img: MessageBoxImage.Error);
                        return;
                    }
                    IntResult = i;
                    break;
                case Type.Double:
                    if (!double.TryParse(textbox.Text, out var d))
                    {
                        MessageBox.Show("Zadána neplatná hodnota", img: MessageBoxImage.Error);
                        return;
                    }
                    DoubleResult = d;
                    break;
            }
            Canceled = false;
            Close();
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Submit();
            } else if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private enum Type
        {
            String,
            Int,
            Double
        }
    }
}
