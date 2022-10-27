using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DegCAD
{
    /// <summary>
    /// Handles the history of commands
    /// </summary>
    public class Timeline
    {
        public Stack<IGeometryCommand> CommandHistory { get; private set; } = new();
        public Stack<IGeometryCommand> UndoneCommands { get; private set; } = new();

        public void Undo()
        {
            UndoneCommands.Push(CommandHistory.Pop());
        }

        public void Redo()
        {
            CommandHistory.Push(UndoneCommands.Pop());
        }

        public void AddCommand(IGeometryCommand cmd)
        {
            CommandHistory.Push(cmd);
            UndoneCommands.Clear();
        }
    }
}
