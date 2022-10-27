using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public interface IGeometryCommand
    {
        Task ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr);
        void Draw(GeometryDrawer gd);
    }
}
