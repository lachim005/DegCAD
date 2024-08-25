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
    /// Interaction logic for ReorderTimelineDialog.xaml
    /// </summary>
    public partial class ReorderTimelineDialog : Window
    {
        private readonly Editor editor;

        public ReorderTimelineDialog(Editor ed, Window owner)
        {
            Owner = owner;
            InitializeComponent();

            editor = ed;
        }
    }
}
