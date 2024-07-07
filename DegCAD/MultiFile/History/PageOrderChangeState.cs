using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class PageOrderChangeState : IMFHistoryState
    {
        MFEditor editor;

        public int index1;
        public int index2;

        public PageOrderChangeState(MFEditor editor, int index1, int index2)
        {
            this.editor = editor;
            this.index1 = index1;
            this.index2 = index2;
        }

        public void ApplyState()
        {
            editor.Pages.Move(index2, index1);
            editor.SelectPage(editor.Pages[index1].Page);
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new PageOrderChangeState(editor, index2, index1);
        }
    }
}
