using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public interface IMFHistoryState : IDisposable
    {
        void ApplyState();
        IMFHistoryState GetOpositeState();
    }
}
