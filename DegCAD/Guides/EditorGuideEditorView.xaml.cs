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

namespace DegCAD.Guides
{
    /// <summary>
    /// Interaction logic for EditorGuideEditorView.xaml
    /// </summary>
    public partial class EditorGuideEditorView : UserControl
    {
        private Guide guide;
        private ViewPort vp;
        private Timeline clonedTl;
        public EditorGuideEditorView(Timeline tl, Guide g)
        {
            InitializeComponent();
            guide = g;

            clonedTl = tl.Clone();
            clonedTl.UndoneCommands.Clear();

            vp = new(clonedTl);
            clonedTl.SetViewportLayer(vp.Layers[1]);
            vpBorder.Child = vp;
        }
    }
}
