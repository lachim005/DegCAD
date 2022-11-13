﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DegCAD.MongeItems;

namespace DegCAD
{
    /// <summary>
    /// Handles the history of commands
    /// </summary>
    public class Timeline
    {
        public Stack<TimelineItem> CommandHistory { get; private set; } = new();
        public Stack<TimelineItem> UndoneCommands { get; private set; } = new();

        public void Undo()
        {
            //User can't undo the axis
            if (CommandHistory.Peek().Items[0] is Axis) return;

            UndoneCommands.Push(CommandHistory.Pop());
        }

        public void Redo()
        {
            CommandHistory.Push(UndoneCommands.Pop());
        }

        public void AddCommand(TimelineItem cmd)
        {
            CommandHistory.Push(cmd);
            UndoneCommands.Clear();
        }
    }
}
