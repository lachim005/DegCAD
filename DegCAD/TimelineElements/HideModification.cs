using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.TimelineElements
{
    public class HideModification(int cmdIndex, int itemIndex) : IModification
    {
        public int CmdIndex { get; set; } = cmdIndex;
        public int ItemIndex { get; set; } = itemIndex;

        public ITimelineElement Clone() => new HideModification(CmdIndex, ItemIndex);

        public void Apply(Timeline tl)
        {
            var item = tl.CommandHistory[CmdIndex].Items[ItemIndex];
            if (item is not GeometryElement ge) return;
            ge.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Remove(Timeline tl)
        {
            var item = tl.CommandHistory[CmdIndex].Items[ItemIndex];
            if (item is not GeometryElement ge) return;
            ge.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
