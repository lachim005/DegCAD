using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class PageRemovedState : IMFHistoryState
    {
        MFPage page;

        public int index;
        public bool wasLast;

        public PageRemovedState(MFPage page)
        {
            this.page = page;
            index = page.Editor?.Pages.First((pm) => ReferenceEquals(pm.Page, page)).Index - 1 ?? 0;
            wasLast = page.Editor?.Pages.Count == 1;
        }

        public void ApplyState()
        {
            if (wasLast)
            {
                // Removes the replacement page that gets added when the user removes the last page
                page.Editor?.Pages.RemoveAt(0);
            }
            page.Editor?.AddPage(page, index);
            page.Editor?.SelectPage(page);
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new PageAddedState(page) { replacementPage = page.Editor?.ActivePage };
        }
    }
}
