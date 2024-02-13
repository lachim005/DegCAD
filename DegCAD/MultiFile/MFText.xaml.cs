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

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFText.xaml
    /// </summary>
    public partial class MFText : MFItem
    {
        public string Text { get; set; } = "";
        public int TextontSize { get; set; } = 20;
        public VerticalAlignment VAlign { get; set; } = VerticalAlignment.Center;
        public HorizontalAlignment HAlign { get; set; } = HorizontalAlignment.Center;
        public bool Italic { get; set; } = true;
        public bool Bold { get; set; } = true;
        public bool Underline { get; set; } = true;
        public bool Strikethrough { get; set; } = true;

        public MFText(string text)
        {
            Text = text;
            DataContext = this;
            InitializeComponent();
        }

        public override void ViewUpdated(double offsetX, double offsetY, double scale)
        {
            textBlock.FontSize = scale * FontSize;
        }
        public override MFItem Clone()
        {
            return new MFText(Text);
        }
    }
}
