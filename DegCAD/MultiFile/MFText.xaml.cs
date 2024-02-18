using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFText.xaml
    /// </summary>
    public partial class MFText : MFItem, INotifyPropertyChanged
    {
        private string _text = "";
        private VerticalAlignment _vAlign = VerticalAlignment.Center;
        private TextAlignment _hAlign = TextAlignment.Center;
        private bool _italic;
        private bool _bold;
        private bool _underline;
        private bool _strikethrough;
        private Color _color = (App.Skin == Skin.Light) ? Colors.Black : Colors.White;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new(nameof(Text)));
            }
        }
        public int TextFontSize { get; set; } = 12;
        public VerticalAlignment VAlign
        {
            get => _vAlign;
            set
            {
                _vAlign = value;
                PropertyChanged?.Invoke(this, new(nameof(VAlign)));
            }
        }
        public TextAlignment HAlign
        {
            get => _hAlign;
            set
            {
                _hAlign = value;
                PropertyChanged?.Invoke(this, new(nameof(HAlign)));
            }
        }
        public bool Italic
        {
            get => _italic;
            set
            {
                _italic = value;
                PropertyChanged?.Invoke(this, new(nameof(Italic)));
            }
        }
        public bool Bold
        {
            get => _bold;
            set
            {
                _bold = value;
                PropertyChanged?.Invoke(this, new(nameof(Bold)));
            }
        }
        public bool Underline
        {
            get => _underline;
            set
            {
                _underline = value;
                PropertyChanged?.Invoke(this, new(nameof(Underline)));
            }
        }
        public bool Strikethrough
        {
            get => _strikethrough;
            set
            {
                _strikethrough = value;
                PropertyChanged?.Invoke(this, new(nameof(Strikethrough)));
            }
        }
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                PropertyChanged?.Invoke(this, new(nameof(Color)));
            }
        }

        public MFText(string text)
        {
            Text = text;
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override void ViewUpdated(double offsetX, double offsetY, double scale)
        {
            textBlock.FontSize = scale * TextFontSize;
        }
        public override MFItem Clone()
        {
            return new MFText(Text);
        }
    }
}
