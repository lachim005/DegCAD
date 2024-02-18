using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.Commands
{
    public class AddTextCommand : IMFCommand
    {
        public MFItem? Execute()
        {
            return new MFText("Text");
        }
    }
}
