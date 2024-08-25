using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    internal class StringBuilderWithIndent
    {
        public StringBuilder StringBuilder { get; set; } = new();

        public int IndentLevel { get; set; } = 0;
        public int IndentWidth { get; set; } = 4;

        public void Indent() => IndentLevel++;
        public void Unindent() => IndentLevel--;

        public void AppendLine(string s)
        {
            StringBuilder.AppendLine(string.Empty.PadRight(IndentLevel * IndentWidth) + s);
        }

        public override string ToString() => StringBuilder.ToString();
    }
}
