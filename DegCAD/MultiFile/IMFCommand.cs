using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.MultiFile
{
    public interface IMFCommand : ICommand
    {
        MFItem? Execute(Window owner);
    }
}
