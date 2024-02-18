using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public enum FileType
    {
        None = 0b1,
        Plane = 0b10,
        Monge = 0b100,
        Axonometry = 0b1000,
        MultiFile = 0b10000
    }
}
