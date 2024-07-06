using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class ContainerAddedState : IMFHistoryState
    {
        MFContainer container;

        public ContainerAddedState(MFContainer container)
        {
            this.container = container;
        }

        public void ApplyState()
        {
            container.Page.Editor?.SelectPage(container.Page);

            if (container.IsSelected)
            {
                container.Deselect();
            }

            container.Page.RemoveItem(container);
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new ContainerRemovedState(container);
        }
    }
}
