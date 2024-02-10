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
    /// Interaction logic for MFDrawing.xaml
    /// </summary>
    public partial class MFDrawing : MFItem
    {
        public Editor editor;
        public ViewPort vp;
        public double UnitSize { get; set; } = 10;
        private int _visibleItems;
        public int VisibleItems
        {
            get => _visibleItems;
            set
            {
                _visibleItems = value;
                var tl = vp.Timeline;
                if (_visibleItems > tl.CommandHistory.Count)
                {
                    while (tl.CanRedo && _visibleItems != tl.CommandHistory.Count)
                    {
                        tl.Redo();
                    }
                } else
                {
                    while (tl.CanUndo && _visibleItems != tl.CommandHistory.Count)
                    {
                        tl.Undo();
                    }
                }
            }
        }

        public MFDrawing(Editor editor)
        {
            InitializeComponent();
            this.editor = editor;
            vp = editor.viewPort.Clone();
            vp.CanZoom = false;
            content.Children.Add(vp);
            _visibleItems = editor.viewPort.Timeline.CommandHistory.Count;
        }

        public override void ViewUpdated(double offsetX, double offsetY, double scale)
        {
            vp.Scale = scale * MFPage.unitSize * UnitSize / ViewPort.unitSize;
            vp.Redraw();
        }

        public override MFItem Clone()
        {
            
            return new MFDrawing(editor.Clone());
        }
    }
}
