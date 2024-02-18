using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile
{
    public interface IMFCommand : ICommand
    {
        void Execute();
    }
}
