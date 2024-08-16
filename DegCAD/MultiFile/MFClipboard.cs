using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile
{
    public static class MFClipboard
    {
        private static MFContainer? _container;
        public static MFContainer? CopiedContainer
        {
            get => _container;
            set
            {
                _container = value?.Clone();
                Offset = 5;
            }
        }

        public static int Offset { get; set; }
    }
}
