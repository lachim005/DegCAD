using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DegCAD
{
    public class OOBEState
    {
        public bool newFile = true;
        public bool toolsPanel = true;
        public bool statusBar = true;
        public bool panViewport = true;
        public bool styleSelector = true;
        public bool labelInput = true;
        public bool guide = true;

        public long Serialize()
        {
            return
                ( newFile       ? 1 : 0) +
                ((toolsPanel    ? 1 : 0) << 1) +
                ((statusBar     ? 1 : 0) << 2) +
                ((panViewport   ? 1 : 0) << 3) +
                ((styleSelector ? 1 : 0) << 4) +
                ((labelInput    ? 1 : 0) << 5) +
                ((guide         ? 1 : 0) << 6);
        }

        public void Deserialize(long data)
        {
            newFile       = (data >> 0 & 1) == 1;
            toolsPanel    = (data >> 1 & 1) == 1;
            statusBar     = (data >> 2 & 1) == 1;
            panViewport   = (data >> 3 & 1) == 1;
            styleSelector = (data >> 4 & 1) == 1;
            labelInput    = (data >> 5 & 1) == 1;
            guide         = (data >> 6 & 1) == 1;
        }
    }
}
