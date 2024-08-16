using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.MultiFile.Commands
{
    public class AddTextCommand : IMFCommand
    {
        public MFItem? Execute(Window owner)
        {
            return new MFText("Text");
        }
    }
}
