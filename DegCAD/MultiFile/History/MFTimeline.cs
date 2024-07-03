using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class MFTimeline
    {
        public Stack<IMFHistoryState> History { get; init; } = new();
        public Stack<IMFHistoryState> RedoStack { get; init; } = new();

        public bool CanUndo => History.Count > 0;
        public bool CanRedo => RedoStack.Count > 0;

        public MFEditor Editor;

        public MFTimeline(MFEditor editor)
        {
            Editor = editor;
        }

        public void Undo()
        {
            if (!CanUndo) return;
            var state = History.Pop();
            RedoStack.Push(state.GetOpositeState());
            state.ApplyState();
            Editor.ActivePage.Redraw();
        }

        public void Redo()
        {
            if (!CanRedo) return;
            var state = RedoStack.Pop();
            History.Push(state.GetOpositeState());
            state.ApplyState();
            Editor.ActivePage.Redraw();
        }

        public void AddState(IMFHistoryState state)
        {
            History.Push(state);
            while (RedoStack.Count > 0)
            {
                RedoStack.Pop().Dispose();
            }
        }
    }
}
