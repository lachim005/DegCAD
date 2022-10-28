using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public interface IGeometryCommand
    {
        Task<TimelineItem> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr);
    }
}
