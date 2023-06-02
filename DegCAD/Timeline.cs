using System;
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
        public HistoryStack<TimelineItem> CommandHistory { get; private set; } = new();
        public Stack<TimelineItem> UndoneCommands { get; private set; } = new();

        public event EventHandler? TimelineChanged;

        public bool CanUndo => CommandHistory.Count > 0 && CommandHistory.Peek().Items[0] is not Axis;
        public bool CanRedo => UndoneCommands.Count > 0;

        public void Undo()
        {
            //User can't undo the axis
            if (CommandHistory.Peek().Items[0] is Axis) return;

            //Can't undo if commands stack is empty
            if (CommandHistory.Count == 0) return;

            var undoneCmd = CommandHistory.Pop();

            foreach (var item in undoneCmd.Items)
            {
                item.SetVisibility(System.Windows.Visibility.Hidden);
            }
            UndoneCommands.Push(undoneCmd);
            TimelineChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Redo()
        {
            //Can't redo if undone stack is empty
            if (UndoneCommands.Count == 0) return;

            var redoneCmd = UndoneCommands.Pop();

            foreach (var item in redoneCmd.Items)
            {
                item.SetVisibility(System.Windows.Visibility.Visible);
            }
            CommandHistory.Push(redoneCmd);
            TimelineChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddCommand(TimelineItem cmd)
        {
            CommandHistory.Push(cmd);
            foreach (var undoneCmd in UndoneCommands)
            {
                foreach (var item in undoneCmd.Items)
                {
                    item.RemoveFromViewportLayer();
                }
            }
            UndoneCommands.Clear();
            TimelineChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
