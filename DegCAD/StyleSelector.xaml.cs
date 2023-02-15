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
        public Style CurrentStyle { get => new() {
            Color = Colors.Black,
            LineStyle = GetSelectedLineType()
        };}

        public StyleSelector()
        {
            InitializeComponent();
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
    }
}
