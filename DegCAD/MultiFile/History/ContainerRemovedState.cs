using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class ContainerRemovedState : IMFHistoryState
    {
        MFContainer container;
        public int index;

        public ContainerRemovedState(MFContainer container)
        {
            this.container = container;
            index = container.Page.Items.IndexOf(container);
        }

        public void ApplyState()
        {
            container.Page.AddItem(container, index);

            container.Select();
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new ContainerAddedState(container);
        }
    }
}
