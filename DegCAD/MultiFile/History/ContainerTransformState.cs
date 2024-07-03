using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    class ContainerTransformState : IMFHistoryState
    {
        MFContainer container;
        public double x;
        public double y;
        public double w;
        public double h;

        public ContainerTransformState(MFContainer cont)
        {
            container = cont;
            x = container.CX;
            y = container.CY;
            w = container.CWidth;
            h = container.CHeight;
        }

        public void ApplyState()
        {
            container.CX = x;
            container.CY = y;
            container.CWidth = w;
            container.CHeight = h;
            container.Select();
        }

        public void Dispose()
        {
            
        }

        public IMFHistoryState GetOpositeState()
        {
            return new ContainerTransformState(container);
        }
    }
}
