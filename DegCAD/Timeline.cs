using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DegCAD.TimelineElements;

namespace DegCAD
{
    /// <summary>
    /// Handles the history of commands
    /// </summary>
    public class Timeline : IEnumerable<TimelineItem>
    {
        public HistoryStack<TimelineItem> CommandHistory { get; private set; } = new();
        public Stack<TimelineItem> UndoneCommands { get; private set; } = new();

        public event EventHandler? TimelineChanged;

        public bool CanUndo => CommandHistory.Count > 1;
        public bool CanRedo => UndoneCommands.Count > 0;
        public int Count => CommandHistory.Count + UndoneCommands.Count;

        public void Undo()
        {
            //Can't undo if commands stack is empty
            //Axis is always first and shouldn't be undone
            if (CommandHistory.Count <= 1) return;

            var undoneCmd = CommandHistory.Pop();

            foreach (var item in undoneCmd.Items)
            {
                if (item is GeometryElement ge)
                {
                    ge.Visibility = System.Windows.Visibility.Hidden;
                }
                else if (item is IModification mod)
                {
                    mod.Remove(this);
                }
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
                if (item is GeometryElement ge)
                {
                    ge.Visibility = System.Windows.Visibility.Visible;
                }
                else if (item is IModification mod)
                {
                    mod.Apply(this);
                }
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
                    if (item is not GeometryElement ge) continue;
                    ge.RemoveFromViewportLayer();
                }
            }
            UndoneCommands.Clear();
            foreach (var it in cmd.Items)
            {
                if (it is not IModification mod) continue;
                mod.Apply(this);
            }
            TimelineChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetViewportLayer(ViewportLayer vpl)
        {
            foreach (var cmd in CommandHistory)
            {
                foreach (var item in cmd.Items)
                {
                    if (item is not GeometryElement ge) continue;
                    ge.AddToViewportLayer(vpl);
                }
            }
        }

        public Timeline Clone()
        {
            Timeline newTl = new();
            foreach (var cmd in CommandHistory)
            {
                var items = new ITimelineElement[cmd.Items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = cmd.Items[i].Clone();
                    if (items[i] is IModification mod)
                        mod.Apply(this);
                }
                newTl.AddCommand(new(items));
            }
            return newTl;
        }

        public void ShowItem(int index)
        {
            index = Math.Clamp(index, 0, CommandHistory.Count + UndoneCommands.Count - 1);
            if (index + 1 < CommandHistory.Count)
            {
                while (CommandHistory.Count > index + 1)
                {
                    Undo();
                }
                return;
            }

            while (CommandHistory.Count < index + 1)
            {
                Redo();
            }
            return;
        }

        public void UndoAll()
        {
            while (CanUndo) Undo();
        }
        public void RedoAll()
        {
            while (CanRedo) Redo();
        }

        public int IndexOf(TimelineItem item)
        {
            int index = 0;
            foreach (var i in CommandHistory)
            {
                if (ReferenceEquals(i, item)) return index;
                index++;
            }

            foreach (var i in UndoneCommands)
            {
                if (ReferenceEquals(i, item)) return index;
                index++;
            }

            return -1;
        }

        public void Move(int move, int moveTo)
        {
            var shownItems = IndexOf(CommandHistory.Peek());
            ShowItem(move);
            var item = CommandHistory.Pop();
            ShowItem(moveTo - 1);
            CommandHistory.Push(item);
            ShowItem(shownItems);
        }

        public IEnumerator<TimelineItem> GetEnumerator() => new TimelineEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new TimelineEnumerator(this);

        public TimelineItem this[int index]
        {
            get
            {
                if (index < CommandHistory.Count)
                {
                    return CommandHistory[index];
                }
                return UndoneCommands.ToArray()[index - CommandHistory.Count];
            }
        }
        public ITimelineElement this[int cmdIndex, int itemIndex]
        {
            get
            {
                return this[cmdIndex].Items[itemIndex];
            }
        }
    }

    public sealed class TimelineEnumerator(Timeline tl) : IEnumerator<TimelineItem>
    {
        public TimelineItem Current => tl[index];

        object IEnumerator.Current => Current;

        public int index = -1;

        readonly Timeline tl = tl;

        public void Dispose() { }

        public bool MoveNext()
        {
            index++;
            return index < tl.Count;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
