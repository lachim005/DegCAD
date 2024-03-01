using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DegCAD.MultiFile
{
    public abstract class MFItem : UserControl
    {
        public abstract void ViewUpdated(double offsetX, double offsetY, double scale);
        public abstract MFItem Clone();
        public abstract void SwapWhiteAndBlack();
    }
}
