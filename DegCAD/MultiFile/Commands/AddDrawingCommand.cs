using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.Commands
{
    public class AddDrawingCommand : IMFCommand
    {
        public MFItem? Execute()
        {
            if (AddDrawingDialog.GetEditor() is not Editor ed) return null;
            return new MFDrawing(ed);
        }
    }
}
