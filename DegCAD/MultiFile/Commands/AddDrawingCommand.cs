using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.MultiFile.Commands
{
    public class AddDrawingCommand : IMFCommand
    {
        public MFItem? Execute(Window owner)
        {
            if (AddDrawingDialog.GetEditor(owner) is not Editor ed) return null;
            return new MFDrawing(ed);
        }
    }
}
