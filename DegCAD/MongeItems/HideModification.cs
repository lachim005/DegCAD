using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    public class HideModification : Modification
    {
        public int CmdIndex { get; set; }
        public int ItemIndex { get; set; }

        public HideModification(int cmdIndex, int itemIndex)
        {
            CmdIndex = cmdIndex;
            ItemIndex = itemIndex;
        }

        public override IMongeItem Clone() => new HideModification(CmdIndex, ItemIndex);

        public override void Apply(Timeline tl)
        {
            tl.CommandHistory[CmdIndex].Items[ItemIndex].SetVisibility(System.Windows.Visibility.Collapsed);
        }

        public override void Remove(Timeline tl)
        {
            tl.CommandHistory[CmdIndex].Items[ItemIndex].SetVisibility(System.Windows.Visibility.Visible);
        }
    }
}
