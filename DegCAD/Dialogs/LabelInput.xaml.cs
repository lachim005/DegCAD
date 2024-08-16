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
    /// Interaction logic for LabelInput.xaml
    /// </summary>
    public partial class LabelInput : Window
    {
        //Last values
        public static int lastFontSize = 16;

        public bool Canceled = true;

        public string LabelText => labelTextTbx.Text;
        public string Superscript => superscriptTbx.Text;
        public string Subscript => subscriptTbx.Text;
        public int TextSize => int.Parse(fontSizeTbx.Text);

        private TextBox lastFocusedTbx;

        public LabelInput(Window? owner = null)
        {
            owner = owner ?? Application.Current.MainWindow;
            InitializeComponent();
            labelTextTbx.Focus();
            lastFocusedTbx = labelTextTbx;
            AddSpecialCharacterButtons();

            //Sets the last inputed font size
            fontSizeTbx.Text = lastFontSize.ToString();

            if (Settings.OOBEState.labelInput)
            {
                OOBELabelText.OpenIfTrue(ref Settings.OOBEState.labelInput);
                OOBESuperscript.IsOpen = true;
                OOBESubscript.IsOpen = true;
            }
        }

        private void Confirm(object sender, ExecutedRoutedEventArgs e)
        {
            if (!int.TryParse(fontSizeTbx.Text, out int size))
            {
                MessageBox.Show("Neplatná velikost písma", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!string.IsNullOrEmpty(LabelText))
            {
                Canceled = false;
            }
            Close();
        }

        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void TbxGotFocus(object sender, RoutedEventArgs e)
        {
            lastFocusedTbx = (TextBox)sender;
        }

        private void AddSpecialCharacterButtons()
        {
            //Special characters
            //Item 1 - what will be added
            //Item 2 - button header
            var specialChars = new List<(string, string)>()
            {
                ("(", "("),
                (")", ")"),
                ("[", "["),
                ("]", "]"),
                ("{", "{"),
                ("}", "}"),
                ("\u0305", "A̅"),
                ("ρ", "ρ"),
                ("α", "α"),
                ("β", "β"),
                ("γ", "γ"),
                ("δ", "δ"),
                ("π", "π"),
                ("ϰ", "ϰ"),
            };

            foreach (var ch in specialChars)
            {
                //Creates a button for the character
                var btn = new Button() { Content = ch.Item2 };
                btn.Click += CharBtnClick;
                btn.Tag = ch.Item1;
                btn.Width = 35;
                btn.Height = 35;
                btn.Margin = new(5);
                specialCharsWp.Children.Add(btn);
            }
        }

        private void CharBtnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not string tag) return;

            AddSpecialCharacter(tag);
        }

        private void AddSpecialCharacter(string str)
        {
            //Adds a special character to the last focused textbox
            lastFocusedTbx.Text += str;
            lastFocusedTbx.Focus();
            lastFocusedTbx.CaretIndex = lastFocusedTbx.Text.Length;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            _ = int.TryParse(fontSizeTbx.Text, out lastFontSize);
        }
    }
}
