using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class PageAddedState : IMFHistoryState
    {
        MFPage page;

        public MFPage? replacementPage;

        public PageAddedState(MFPage page)
        {
            this.page = page;
        }

        public void ApplyState()
        {
            if (page.Editor is not MFEditor ed) return;

            if (ReferenceEquals(page, ed.ActivePage))
            {
                // If the page that will be removed was selected, selects a different one
                ed.SelectPage(ed.Pages[0].Page);
            }

            ed.RemovePage(page);

            if (ed.Pages.Count == 0)
            {
                // Adds the replacement page if the undo removes the last page
                MFPage newPage = replacementPage ?? new(ed);
                ed.AddPage(newPage);
                ed.SelectPage(newPage);
            }
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new PageRemovedState(page);
        }
    }
}
